#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using ExportOptions = DevExpress.XtraPrinting.ExportOptions;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class DocumentMediator : MediatorBase<DocumentId, StoredDocument>, IDocumentMediator {
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly AutoDisposableList openedStreams = new AutoDisposableList();
		readonly ISerializationService serializationService;
		readonly IPrintingSystemFactory printingSystemFactory;
		readonly IBinaryDataStorageService binaryDataStorageService;
		Document virtualDocument;
		ExportOptionKind? assignedHiddenOptions;
		AvailableExportModes assignedExportModes;
		byte[] prnx;
		byte[] documentExportOptions;
		ServiceFault documentFault;
		bool isDocumentRelatedDataChanged;
		CultureInfo documentCultureInfo;
		CultureInfo documentCultureUIInfo;
		bool disposed;
		protected Document VirtualDocument {
			get {
				if(virtualDocument == null) {
					virtualDocument = LoadDocumentCore(PrintingSystemAccessor.LoadVirtualDocument);
				}
				return virtualDocument;
			}
		}
		public DocumentMediator(
			ISerializationService serializationService,
			IPrintingSystemFactory printingSystemFactory,
			IDALService dalService,
			IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider)
			: base(dalService) {
			Guard.ArgumentNotNull(serializationService, "serializationService");
			Guard.ArgumentNotNull(printingSystemFactory, "printingSystemFactory");
			Guard.ArgumentNotNull(binaryDataStorageServiceProvider, "binaryDataStorageServiceProvider");
			this.serializationService = serializationService;
			this.printingSystemFactory = printingSystemFactory;
			this.binaryDataStorageService = binaryDataStorageServiceProvider.GetService();
		}
		#region IDocumentBuilderMediator Members
		public string DocumentName {
			get { return VirtualDocument.Name; }
		}
		public CultureInfo DocumentCultureInfo {
			get {
				if(documentCultureInfo == null) {
					if(string.IsNullOrEmpty(Entity.RelatedData.DocumentCulture)) {
						return Thread.CurrentThread.CurrentCulture;
					} else {
						documentCultureInfo = CultureInfo.CreateSpecificCulture(Entity.RelatedData.DocumentCulture);
					}
				}
				return documentCultureInfo;
			}
		}
		public CultureInfo DocumentCultureUIInfo {
			get {
				if(documentCultureUIInfo == null) {
					if(string.IsNullOrEmpty(Entity.RelatedData.DocumentUICulture)) {
						return Thread.CurrentThread.CurrentUICulture;
					} else {
						documentCultureUIInfo = CultureInfo.CreateSpecificCulture(Entity.RelatedData.DocumentUICulture);
					}
				}
				return documentCultureUIInfo;
			}
		}
		public RequestingAction DocumentRequestingAction {
			get {
				Entity.TaskRequestingAction.Reload();
				return Entity.TaskRequestingAction.Value;
			}
			set { Entity.TaskRequestingAction.Value = value; }
		}
		public ServiceFault DocumentFault {
			get { return documentFault ?? Entity.RelatedData.Fault.FromStored(); }
			set {
				documentFault = value;
				isDocumentRelatedDataChanged = true;
			}
		}
		public Document LoadFullDocument() {
			return LoadDocumentCore((ps, stream) => ps.LoadDocument(stream));
		}
		public Document LoadPages(int[] pageIndexes) {
			foreach(var pageIndex in pageIndexes) {
				DocumentAccessor.LoadPage(VirtualDocument, pageIndex);
			}
			return VirtualDocument;
		}
		public void SetDocument(Document document) {
			using(var stream = new MemoryStream()) {
				PrintingSystemAccessor.SaveIndependentPages(document.PrintingSystem, stream);
				prnx = stream.ToArray();
			}
			var availableExportModes = document.AvailableExportModes;
			assignedExportModes = new AvailableExportModes(
				availableExportModes.Rtf,
				availableExportModes.Html.Exclude(HtmlExportMode.DifferentFiles),
				availableExportModes.Image.Exclude(ImageExportMode.DifferentFiles),
				availableExportModes.Xls.Exclude(XlsExportMode.DifferentFiles),
				availableExportModes.Xlsx.Exclude(XlsxExportMode.DifferentFiles));
			isDocumentRelatedDataChanged = true;
		}
		public void SetWatermark(Watermark watermark) {
			var serializedWatermark = serializationService.Serialize(watermark);
			var watermarkExternalKey = binaryDataStorageService.Create(serializedWatermark, Session);
			Entity.RelatedData.WatermarkExternalKey = watermarkExternalKey;
			isDocumentRelatedDataChanged = true;
		}
		public virtual T DoPageAction<T>(int pageIndex, Func<Page, T> action) {
			if(pageIndex < 0 || pageIndex >= Entity.PageCount) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			DocumentAccessor.LoadPage(VirtualDocument, pageIndex);
			var page = VirtualDocument.Pages[pageIndex];
			var result = action(page);
			try {
				UpdateStoredDocumentLastAccessDateTimeIfNeeded();
				Entity.SmartSave();
			} catch(LockingException) {
				Logger.Error("Can't save ({0}) - LockException", Entity.DocumentIdValue);
				Entity.Reload();
				result = DoPageAction(pageIndex, action);
			}
			return result;
		}
		public void SetExportOptions(ExportOptions exportOptions) {
			using(var ms = new MemoryStream()) {
				exportOptions.SaveToStream(ms);
				documentExportOptions = ms.ToArray();
			}
			assignedHiddenOptions = exportOptions.HiddenOptions.Count != 0
				? exportOptions.HiddenOptions.Aggregate((result, value) => result | value)
				: default(ExportOptionKind);
			isDocumentRelatedDataChanged = true;
		}
		public void SetDrillDownKeys(Dictionary<string, bool> drillDownKeys) {
			if(drillDownKeys != null) {
				byte[] serializedDrillDownKeys = serializationService.Serialize(drillDownKeys);
				string drillDownKeysExternalKey = binaryDataStorageService.Create(serializedDrillDownKeys, Session);
				Entity.RelatedData.DrillDownKeysExternalKey = drillDownKeysExternalKey;
			} else {
				Entity.RelatedData.DrillDownKeysExternalKey = null;
			}
			isDocumentRelatedDataChanged = true;
		}
		public DocumentMapTreeViewNode DocumentMap {
			get {
				if(Entity.PageCount == 0) {
					return null;
				}
				return DocumentMapTreeViewNodeBuilder.Build(VirtualDocument);
			}
		}
		public byte[] SerializedPageData {
			get {
				if(Entity.PageCount == 0) {
					return null;
				}
				return DoFirstPageAction(page => serializationService.Serialize(page.PageData));
			}
		}
		public byte[] SerializedWatermark {
			get { return binaryDataStorageService.LoadBytes(Entity.RelatedData.WatermarkExternalKey, Session); }
		}
		public byte[] SerializedExportOptions {
			get { return binaryDataStorageService.LoadBytes(Entity.RelatedData.ExportOptionsExternalKey, Session); }
		}
		public Dictionary<string, bool> DrillDownKeys {
			get {
				var serializedDrillDownKeys = binaryDataStorageService.LoadBytes(Entity.RelatedData.DrillDownKeysExternalKey, Session);
				if(serializedDrillDownKeys == null) {
					return new Dictionary<string, bool>();
				}
				return serializationService.DeserializeDrillDownKeys(serializedDrillDownKeys);
			}
		}
		public virtual AvailableExportModes DocumentAvailableExportModes {
			get {
				var exportModes = Entity.RelatedData.ExportModes;
				return new AvailableExportModes(
					exportModes.GetByKind<RtfExportMode>(SupportedExportOptionKind.Rtf),
					exportModes.GetByKind<HtmlExportMode>(SupportedExportOptionKind.Html),
					exportModes.GetByKind<ImageExportMode>(SupportedExportOptionKind.Image),
					exportModes.GetByKind<XlsExportMode>(SupportedExportOptionKind.Xls),
					exportModes.GetByKind<XlsxExportMode>(SupportedExportOptionKind.Xlsx));
			}
		}
		public int PageCount {
			get { return Entity.PageCount; }
			set { Entity.PageCount = value; }
		}
		public virtual PagesRequestInformation EnqueuePagesRequest(int[] pageIndexes, PageCompatibility compatibility, int? addition) {
			var pageRequestInformation = new PagesRequestInformation(Session) {
				Compatibility = compatibility,
				Addition = addition,
				SerializedPageIndexes = serializationService.Serialize(pageIndexes),
				DocumentId = Entity.DocumentId,
				CreationTime = DateTime.UtcNow
			};
			return pageRequestInformation;
		}
		public virtual PagesRequestInformation DequeuePagesRequest() {
			var requests = PagesRequestInformation.FindUnansweredByDocument(Entity.DocumentId, Session);
			return requests.FirstOrDefault();
		}
		public override void Save() {
			SaveCore(0);
		}
		public override void ReloadEntity() {
			ClearPrivateData();
			base.ReloadEntity();
		}
		public override void Delete() {
			if(Entity.IsPermanent) {
				throw new FaultException(Messages.CanNotRemoveDocument);
			}
			var relatedData = Entity.RelatedData;
			if(relatedData.ExportOptionsExternalKey != null) {
				binaryDataStorageService.Delete(relatedData.ExportOptionsExternalKey, Session);
			}
			if(relatedData.PrnxExternalKey != null) {
				binaryDataStorageService.Delete(relatedData.PrnxExternalKey, Session);
			}
			if(relatedData.WatermarkExternalKey != null) {
				binaryDataStorageService.Delete(relatedData.WatermarkExternalKey, Session);
			}
			if(relatedData.DrillDownKeysExternalKey != null) {
				binaryDataStorageService.Delete(relatedData.DrillDownKeysExternalKey, Session);
			}
			base.Delete();
		}
		#endregion
		#region IDisposable Members
		public override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposed || !disposing) {
				return;
			}
			openedStreams.Dispose();
			if(virtualDocument != null && virtualDocument.PrintingSystem != null) {
				virtualDocument.PrintingSystem.Dispose();
			}
			disposed = true;
		}
		#endregion
		protected override StoredDocument CreateEntity(DocumentId identity) {
			return StoredDocument.Create(identity, Session);
		}
		protected override StoredDocument GetEntityById(DocumentId identity) {
			return StoredDocument.GetById(identity, Session);
		}
		protected virtual Document LoadDocumentCore(Action<PrintingSystemBase, Stream> loadDocumentAction) {
			var documentLayout = prnx ?? binaryDataStorageService.LoadBytes(Entity.RelatedData.PrnxExternalKey, Session);
			var stream = new MemoryStream(documentLayout);
			openedStreams.Add(stream);
			var ps = printingSystemFactory.CreatePrintingSystem();
			loadDocumentAction(ps, stream);
			return ps.Document;
		}
		void UpdateStoredDocumentLastAccessDateTimeIfNeeded() {
			var minDiff = TimeSpan.FromSeconds(1);
			var now = DateTime.UtcNow;
			if(Entity.LastModifiedTime - now > minDiff) {
				Entity.LastModifiedTime = now;
			}
		}
		T DoFirstPageAction<T>(Func<Page, T> action) {
			return DoPageAction(0, action);
		}
		void SaveCore(int recursionCount) {
			var relatedData = Entity.RelatedData;
			if(prnx != null && relatedData.PrnxExternalKey == null) {
				relatedData.PrnxExternalKey = binaryDataStorageService.Create(prnx, Session);
			}
			if(documentExportOptions != null && relatedData.ExportOptionsExternalKey == null) {
				relatedData.ExportOptionsExternalKey = binaryDataStorageService.Create(documentExportOptions, Session);
			}
			if(documentFault != null) {
				relatedData.Fault = documentFault.ToStored(Session);
			}
			if(assignedHiddenOptions != null) {
				Entity.HiddenOptions = assignedHiddenOptions.Value;
			}
			if(assignedExportModes != null) {
				relatedData.ExportModes.AddRange(assignedExportModes.ToBoxed(relatedData, Session));
			}
			Entity.TaskRequestingAction.SmartSave();
			var documentRelatedData = Entity.RelatedData;
			if(isDocumentRelatedDataChanged || Session.IsNewObject(relatedData)) {
				var currentThread = Thread.CurrentThread;
				documentRelatedData.DocumentCulture = currentThread.CurrentCulture.IetfLanguageTag;
				documentRelatedData.DocumentUICulture = currentThread.CurrentUICulture.IetfLanguageTag;
				Session.Save(documentRelatedData);
			}
			try {
				UpdateStoredDocumentLastAccessDateTimeIfNeeded();
				Entity.SmartSave();
				Session.CommitChanges();
			} catch(Exception e) {
				if(!(e is LockingException || e is ObjectDisposedException)) {
					throw;
				}
				const int MaxAttempts = 5;
				if(recursionCount >= MaxAttempts) {
					throw;
				}
				Entity.Reload();
				Entity.TaskRequestingAction.Reload();
				SaveCore(recursionCount + 1);
				return;
			}
			ClearPrivateData();
		}
		void ClearPrivateData() {
			prnx = null;
			documentExportOptions = null;
			documentFault = null;
			assignedHiddenOptions = null;
			assignedExportModes = null;
			isDocumentRelatedDataChanged = false;
		}
	}
}
