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
using System.Collections.Specialized;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Web.Office.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal;
namespace DevExpress.Web.Office {
	public class SpreadsheetDocumentInfo : OfficeDocumentBase<SpreadsheetWorkSession> {
		public SpreadsheetDocumentInfo(SpreadsheetWorkSession workSession) : base(workSession)  { }
		protected internal Guid WorkSessionGuid { get { return WorkSession.ID; } }
		public DevExpress.Spreadsheet.DocumentFormat DocumentFormat { get { return WorkSession.CalculateDocumentFormat(); } }
		public override bool Modified { get { return WorkSession.Modified; } }
		public override DateTime? LastModifyTime { get { return WorkSession.LastModifyTime; } }
		public override void SaveCopy(string filePath) {
			WorkSession.WebSpreadsheetControlDocumentProxy.Save(new DocumentContentContainer(filePath));
		}
		public override byte[] SaveCopy() {
			return SaveCopy(DevExpress.Spreadsheet.DocumentFormat.Undefined);
		}
		public byte[] SaveCopy(DevExpress.Spreadsheet.DocumentFormat documentFormat) {
			var arrayDocumentContainer = DocumentContentContainer.GetEmptyArrayContainer(documentFormat.ToString());
			WorkSession.WebSpreadsheetControlDocumentProxy.Save(arrayDocumentContainer);
			return arrayDocumentContainer.Array;
		}
		public override void SaveCopy(Stream stream) {
			SaveCopy(stream, DevExpress.Spreadsheet.DocumentFormat.Undefined);
		}
		public void SaveCopy(Stream stream, DevExpress.Spreadsheet.DocumentFormat documentFormat) {
			WorkSession.WebSpreadsheetControlDocumentProxy.Save(new DocumentContentContainer(stream, documentFormat.ToString(), null));
		}
	}
}
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	using System.Web.Caching;
	using DevExpress.Web.Office.Internal;
	using DevExpress.XtraSpreadsheet.Commands;
	using DevExpress.Web.Office;
	using System.Collections;
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SpreadsheetWorkSessionRegistrationAttribute : WorkSessionRegistrationAttributeBase {
		protected override void RegisterWorkSessionFactory() {
			SpreadsheetWorkSession.RegisterInFactory();
		}
	}
	public interface IWebSpreadsheetControlDocumentProxy : IDisposable {
		WebSpreadsheetControl WebSpreadsheetControl { get; }
		DocumentModel DocumentModel { get; }
		void CreateNewDocument();
		bool LoadDocument(DocumentContentContainer documentContentContainer);
		bool LoadDocument(HibernationChamber hibernationChamber, string documentPath, DocumentFormat currentFormat);
		void Save(DocumentContentContainer documentContentContainer);
		byte[] CloneDocument();
		IWorkbook Document { get; }
		DocumentFormat CalculateDocumentFormat();
		XtraSpreadsheet.Model.Worksheet ActiveSheet { get; }
	}
	public class WebSpreadsheetControlDocumentProxy : IWebSpreadsheetControlDocumentProxy {
		readonly object locker = new object();
		WebSpreadsheetControl webSpreadsheetControl;
		public WebSpreadsheetControlDocumentProxy() {
			webSpreadsheetControl = new WebSpreadsheetControl();
		}
		public WebSpreadsheetControl WebSpreadsheetControl { get { return webSpreadsheetControl; } }
		public DocumentModel DocumentModel { get { return WebSpreadsheetControl.DocumentModel; } }
		public IWorkbook Document { get { return WebSpreadsheetControl.Document; } }
		public XtraSpreadsheet.Model.Worksheet ActiveSheet { get { return DocumentModel.ActiveSheet; } }
		public void CreateNewDocument() {
			webSpreadsheetControl.CreateNewDocument();
		}
		protected DocumentFormat ParseDocumentFormat(DocumentContentContainer documentContentContainer) {
			string formatName = documentContentContainer.FormatName;
			var documentFormat = ParseDocumentFormatName(formatName);
			if(documentFormat != DocumentFormat.Undefined)
				return documentFormat;
			else if(documentContentContainer.IsFile)
				return DocumentModel.AutodetectDocumentFormat(documentContentContainer.PathOrID);
			return DocumentFormat.Undefined;
		}
		protected DocumentFormat ParseDocumentFormatName(string formatName) {
			int formatValue;
			if(int.TryParse(formatName, out formatValue))
				return new DocumentFormat(formatValue);
			return DocumentFormat.Undefined;
		}
		public bool LoadDocument(DocumentContentContainer documentContentContainer) {
			DocumentFormat format = ParseDocumentFormat(documentContentContainer);
			lock (locker) {
				if(documentContentContainer.IsFile)
					return webSpreadsheetControl.LoadDocument(documentContentContainer.PathOrID, format);
				else if(documentContentContainer.IsArray)
					return webSpreadsheetControl.LoadDocument(documentContentContainer.Array, format);
				else if(documentContentContainer.IsStream)
					return webSpreadsheetControl.LoadDocument(documentContentContainer.Stream, format);
			}
			return false;
		}
		public bool LoadDocument(HibernationChamber hibernationChamber, string documentPath, DocumentFormat currentFormat) {
			DocumentFormat documentFormat = ParseDocumentFormatName(hibernationChamber.FormatName);
			bool success = webSpreadsheetControl.LoadDocument(hibernationChamber.Data, documentFormat);
			if(success) {
				InnerSpreadsheetDocumentServer.SetupSaveOptions(Document.Options.Save, documentPath, currentFormat);
			}
			return success;
		}
		bool IsTemplateFormat(DocumentFormat documentFormat) {
			return documentFormat == DocumentFormat.Xlt || documentFormat == DocumentFormat.Xltx || documentFormat == DocumentFormat.Xltm;
		}
		public void Save(DocumentContentContainer documentContentContainer) {
			DocumentFormat format = ParseDocumentFormat(documentContentContainer);
			if(format == DocumentFormat.Undefined)
				format = CalculateDocumentFormat();
			lock (locker) {
				if(documentContentContainer.IsFile)
					webSpreadsheetControl.SaveDocument(documentContentContainer.PathOrID, format);
				else if(documentContentContainer.IsArray)
					documentContentContainer.Array = webSpreadsheetControl.SaveDocument(format);
				else if(documentContentContainer.IsStream)
					webSpreadsheetControl.SaveDocument(documentContentContainer.Stream, format);
			}
		}
		public DocumentFormat CalculateDocumentFormat() {
			var defaultFormat = Document.Options.Save.DefaultFormat;
			var currentFormat = Document.Options.Save.CurrentFormat;
			return currentFormat != DocumentFormat.Undefined ? currentFormat : defaultFormat;
		}
		public byte[] CloneDocument() {
			lock (locker) {
				byte[] buffer = webSpreadsheetControl.SaveDocument(CalculateDocumentFormat());
				return buffer;
			}
		}
		#region IDisposable Members
		public void Dispose() {
			webSpreadsheetControl.Dispose();
		}
		#endregion
	}
	public class SpreadsheetWorkSession : WorkSessionBase<object> {
		const string WorkSessionDocumentTypeName = "spreadsheet";
		internal static void RegisterInFactory() {
			WorkSessionFactories.Register(WorkSessionDocumentTypeName, id => new SpreadsheetWorkSession(null, id));
		}
		static SpreadsheetWorkSession() {
			RegisterInFactory();
		}
		internal SpreadsheetWorkSession(DocumentContentContainer documentContentContainer, Guid id)
			: base(documentContentContainer, id, null) {
#if DebugTest
			if(Document != null) {
#endif
				Document.ModifiedChanged += OnDocumentModifiedChanged;
				Document.ContentChanged += Document_ContentChanged;
#if DebugTest
			}
#endif
		}
		void Document_ContentChanged(object sender, EventArgs e) {
			RefreshLastModifyTime();
		}
		void OnDocumentModifiedChanged(object sender, EventArgs e) {
			OnModifiedChanged(Modified);
		}
		IWebSpreadsheetControlDocumentProxy webSpreadsheetControlDocumentProxy;
		internal IWebSpreadsheetControlDocumentProxy WebSpreadsheetControlDocumentProxy {
			get {
				OnBeforeDocumentAccess();
				if(webSpreadsheetControlDocumentProxy == null)
					webSpreadsheetControlDocumentProxy = GetCreateWebSpreadsheetControlDocumentProxy();
				return webSpreadsheetControlDocumentProxy;
			}
		}
		internal WebSpreadsheetControl WebSpreadsheetControl { get { return WebSpreadsheetControlDocumentProxy.WebSpreadsheetControl; } }
		public DocumentModel DocumentModel { get { return WebSpreadsheetControlDocumentProxy.DocumentModel; } }
		public IWorkbook Document { get { return WebSpreadsheetControlDocumentProxy.Document; } }
		public XtraSpreadsheet.Model.Worksheet ActiveSheet { get { return WebSpreadsheetControlDocumentProxy.ActiveSheet; } }
		public DocumentFormat CalculateDocumentFormat() { return WebSpreadsheetControlDocumentProxy.CalculateDocumentFormat(); }
		protected override bool DocumentControlExists { get { return this.webSpreadsheetControlDocumentProxy != null; } }
		protected override bool GetModified() {
			return Document.Modified; 
		}
		protected override void SetModified(bool value) {
			Document.Modified = value;
		}
		protected virtual IWebSpreadsheetControlDocumentProxy GetCreateWebSpreadsheetControlDocumentProxy() {
			return new WebSpreadsheetControlDocumentProxy();
		}
		protected override void CreateModelCore(DocumentContentContainer documentContentContainer, object settings) {
			if(documentContentContainer == null || string.IsNullOrEmpty(documentContentContainer.PathOrID))
				CreateNewDocument();
			else {
				LoadDocument(documentContentContainer);
			}
		}
		protected override void CreateNewDocument() {
			RefreshLastTimeActivity();
			WebSpreadsheetControlDocumentProxy.CreateNewDocument();
			OnNewDocumentCreated();
		}
		protected override void LoadDocument(DocumentContentContainer documentContentContainer) {
			RefreshLastTimeActivity();
			WebSpreadsheetControlDocumentProxy.LoadDocument(documentContentContainer);
			OnNewDocumentLoaded(documentContentContainer);
		}
		protected override void SaveAs(DocumentContentContainer documentContentContainer) {
			RefreshLastTimeActivity();
			WebSpreadsheetControlDocumentProxy.Save(documentContentContainer);
		}
		protected override WorkSessionBase Clone(Guid newWorkSessionID, string documentPathOrID) {
			RefreshLastTimeActivity();
			byte[] buffer = WebSpreadsheetControlDocumentProxy.CloneDocument();
			string formatName = WebSpreadsheetControlDocumentProxy.CalculateDocumentFormat().ToString();
			DocumentContentContainer tempDocumentContentContainer = new DocumentContentContainer(buffer, formatName, documentPathOrID);
			WorkSessionBase clone = new SpreadsheetWorkSession(tempDocumentContentContainer, newWorkSessionID);
			return clone;
		}
		internal string ProcessCustomCommand(SpreadsheetRenderHelper renderHelper, WebSpreadsheetCommandBase command) {
			WebSpreadsheetCommandWrapper commandWrapper = WebSpreadsheetCommands.GetCommandWrapperByCommand(command);
			ProcessCommand(renderHelper, commandWrapper);
			DocumentHandlerResponse handlerResult = commandWrapper.GetResult(renderHelper);
			if(handlerResult != null)
				return handlerResult.GetResponseResult().ToString();
			return string.Empty;
		}
		internal string ProcessCustomCommand(SpreadsheetRenderHelper renderHelper, SpreadsheetCommand command) {
			WebSpreadsheetCommandWrapper commandWrapper = WebSpreadsheetCommands.GetCommandWrapperByCommand(command);
			ProcessCommand(renderHelper, commandWrapper);
			DocumentHandlerResponse handlerResult = commandWrapper.GetResult(renderHelper);
			if(handlerResult != null)
				return handlerResult.GetResponseResult().ToString();
			return string.Empty;
		}
		internal string ProcessCallbackCommand(NameValueCollection nameValueCollection, bool anotherDocumentOpened) {
			DocumentHandlerResponse commandResult = ProcessRequestCore(nameValueCollection, anotherDocumentOpened);
			if(commandResult != null)
				return commandResult.GetResponseResult().ToString();
			return string.Empty;
		}
		protected override DocumentHandlerResponse ProcessRequestCore(NameValueCollection nameValueCollection) {
			return ProcessRequestCore(nameValueCollection, false);
		}
		protected DocumentHandlerResponse ProcessRequestCore(NameValueCollection nameValueCollection, bool anotherDocumentOpened) {
			var renderHelper = new SpreadsheetRenderHelper(this, nameValueCollection);
			try {
				WebSpreadsheetCommandWrapper commandWrapper = WebSpreadsheetCommands.GetCommandFromContext(renderHelper);
				ProcessCommand(renderHelper, commandWrapper, anotherDocumentOpened);
				return commandWrapper.GetResult(renderHelper);
			}
			catch(Exception exc) {
				JSONDocumentHandlerResponse errorResponse = new JSONDocumentHandlerResponse();
				string errorText = CommonUtils.RaiseCallbackErrorInternal(DocumentInfo, exc);
				errorResponse.ContentEncoding = DefaultCommandGetResultImpl.RequestContentEncoding;
				errorResponse.ContentType = DocumentRequestManager.DefaultResponseContentType;
				var errorResult = new Hashtable();
				errorResult[CallbackResultProperties.Error] = new Hashtable() {
					{ CallbackResultProperties.ErrorMessage, errorText }
				};
				errorResponse.ResponseResult = HtmlConvertor.ToJSON(errorResult);
				return errorResponse;
			}
		}
		private void ProcessCommand(SpreadsheetRenderHelper renderHelper, WebSpreadsheetCommandWrapper commandWrapper, bool anotherDocumentOpened) {
			OnBeforeCommandProcessing(renderHelper, anotherDocumentOpened);
			CommandProcessing(commandWrapper);
			OnAfterCommandProcessing(renderHelper);
		}
		private void ProcessCommand(SpreadsheetRenderHelper renderHelper, WebSpreadsheetCommandWrapper commandWrapper) {
			ProcessCommand(renderHelper, commandWrapper, false);
		}
		private void OnBeforeCommandProcessing(SpreadsheetRenderHelper renderHelper, bool anotherDocumentOpened) {
			renderHelper.LoadClientState(anotherDocumentOpened);
			EnsureCurrentWebRangesKnowsTilesSize();
			ActiveSheet.WebRanges.BeginUpdate(renderHelper.CachedTiles);
		}
		private void CommandProcessing(WebSpreadsheetCommandWrapper commandWrapper) {
			if(commandWrapper != null) {
				commandWrapper.Execute();
			}
		}
		private void OnAfterCommandProcessing(SpreadsheetRenderHelper renderHelper) {
			EnsureCurrentWebRangesKnowsTilesSize();
			AddModifiedTiles(ActiveSheet.WebRanges.EndUpdate());
			renderHelper.OnAfterCommandProcessing();
		}
		private void EnsureCurrentWebRangesKnowsTilesSize() {
			ActiveSheet.WebRanges.SetTilesSize(SpreadsheetRenderHelper.TileColCount, SpreadsheetRenderHelper.TileRowCount);
		}
		protected override void CloseCore() {
			DisposeDocumentProxy();
		}
		void DisposeDocumentProxy() {
			if(this.webSpreadsheetControlDocumentProxy != null) {
				WebSpreadsheetControlDocumentProxy.Dispose();
				this.webSpreadsheetControlDocumentProxy = null;
			}
		}
		void AddModifiedTiles(List<CellRange> tilesModifiedFromEndUpdate) {
		}
		protected override IDocumentInfo GetCreateDocumentInfo() {
			return new SpreadsheetDocumentInfo(this);
		}
		protected override void SaveInTheSameFileCore() {
			Document.SaveDocument(GetCurrentDocumentFilePath());
			Document.Modified = false;
		}
		protected override string GetCurrentDocumentFilePath() {
			return Document.Path;
		}
		enum SpreadsheetHibernationMetadataKeys  { DocumentPath, CurrentFormat }
		protected override string GetWorkSessionDocumentTypeName() { return WorkSessionDocumentTypeName; } 
		protected override HibernationChamber SaveToHibernationChamber() {
			var documentFormat = DocumentFormat.Xlsx;
			var hibernationChamber = new HibernationChamber(Document.SaveDocument(documentFormat), documentFormat.ToString());
			hibernationChamber.Metadata.Add(SpreadsheetHibernationMetadataKeys.DocumentPath.ToString(), Document.Path);
			hibernationChamber.Metadata.Add(SpreadsheetHibernationMetadataKeys.CurrentFormat.ToString(), Document.Options.Save.CurrentFormat.ToString());
			return hibernationChamber;
		}
		protected override void UnloadDocumentFromMemory() {
			DisposeDocumentProxy();
		}
		protected override bool RestoreFromHibernationChamber(HibernationChamber hibernationChamber) {
			var documentPath = hibernationChamber.Metadata[SpreadsheetHibernationMetadataKeys.DocumentPath.ToString()];
			var currentFormatString = hibernationChamber.Metadata[SpreadsheetHibernationMetadataKeys.CurrentFormat.ToString()];
			var currentFormat = new DocumentFormat(int.Parse(currentFormatString));
			var success = WebSpreadsheetControlDocumentProxy.LoadDocument(hibernationChamber, documentPath, currentFormat);
			return success;
		}
	}
	public static class SpreadsheetWorkSessionsUtils {
		public static Guid OpenWorkSession(Guid currentWorkSessionID, DocumentContentContainer documentContentContainer) {
			return WorkSessions.OpenWorkSession(currentWorkSessionID, documentContentContainer, (Guid newGuid) => new SpreadsheetWorkSession(documentContentContainer, newGuid));
		}
		public static Guid SaveAsWorkSession(Guid currentWorkSessionID, DocumentContentContainer documentContentContainer) {
			return WorkSessions.SaveAsWorkSession(currentWorkSessionID, documentContentContainer);
		}
	}
}
