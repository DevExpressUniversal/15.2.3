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
using System.IO;
using System.ComponentModel;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native.Printing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Printing;
using DevExpress.Snap.Core.Options;
using DevExpress.XtraRichEdit.API.Native;
using ApiMailMergeOptions = DevExpress.XtraRichEdit.API.Native.MailMergeOptions;
namespace DevExpress.Snap.Core.Native {
	public interface ISnapDocumentServer : IRichEditDocumentServer, ISnapDriver {
		#region Snap Mail Merge
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMergeStarted event instead.")]
		new event MailMergeStartedEventHandler MailMergeStarted;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMergeRecordStarted event instead.")]
		new event MailMergeRecordStartedEventHandler MailMergeRecordStarted;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMergeRecordFinished event instead.")]
		new event MailMergeRecordFinishedEventHandler MailMergeRecordFinished;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMergeFinished event instead.")]
		new event MailMergeFinishedEventHandler MailMergeFinished;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the CreateSnapMailMergeExportOptions method instead.")]
		new ApiMailMergeOptions CreateMailMergeOptions();
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(Document document);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(ApiMailMergeOptions options, Document targetDocument);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(IRichEditDocumentServer documentServer);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(string fileName, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(Stream stream, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMerge method instead.")]
		new void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format);
		SnapMailMergeExportOptions CreateSnapMailMergeExportOptions();
		void SnapMailMerge(SnapDocument document);
		void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument);
		void SnapMailMerge(ISnapDocumentServer documentServer);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		void SnapMailMerge(SnapMailMergeExportOptions options, ISnapDocumentServer targetDocumentServer);
		void SnapMailMerge(string fileName, DocumentFormat format);
		void SnapMailMerge(Stream stream, DocumentFormat format);
		void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format);
		void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format);
		event EventHandler AsynchronousOperationStarted;
		event EventHandler AsynchronousOperationFinished;
		event SnapMailMergeStartedEventHandler SnapMailMergeStarted;
		event SnapMailMergeRecordStartedEventHandler SnapMailMergeRecordStarted;
		event SnapMailMergeRecordFinishedEventHandler SnapMailMergeRecordFinished;
		event SnapMailMergeFinishedEventHandler SnapMailMergeFinished;
		#endregion
		event BeforeConversionEventHandler BeforeConversion;
	}
	public class InnerSnapDocumentServer : InnerRichEditDocumentServer, ISnapDocumentServer {
		DocumentDataSources docDataSources;
		public InnerSnapDocumentServer(IInnerSnapDocumentServerOwner owner)
			: base(owner) {
		}
		public InnerSnapDocumentServer(IInnerRichEditDocumentServerOwner owner, SnapDocumentModel documentModel)
			: base(owner, documentModel) {
		}
		#region Events
		#region BeforeConversion
		BeforeConversionEventHandler onBeforeConversion;
		public event BeforeConversionEventHandler BeforeConversion { add { onBeforeConversion += value; } remove { onBeforeConversion -= value; } }
		protected internal virtual void RaiseBeforeConversion() {
			if (onBeforeConversion != null)
				onBeforeConversion(Owner, new BeforeConversionEventArgs(Document));
		}
		#endregion
		#region BeforeDataSourceExport
		BeforeDataSourceExportEventHandler onBeforeDataSourceExport;
		public event BeforeDataSourceExportEventHandler BeforeDataSourceExport { add { onBeforeDataSourceExport += value; } remove { onBeforeDataSourceExport -= value; } }
		protected internal virtual void RaiseBeforeDataSourceExport(BeforeDataSourceExportEventArgs e) {
			if (onBeforeDataSourceExport != null)
				onBeforeDataSourceExport(this, e);
		}
		#endregion
		#region AfterDataSourceImport
		AfterDataSourceImportEventHandler onAfterDataSourceImport;
		public event AfterDataSourceImportEventHandler AfterDataSourceImport { add { onAfterDataSourceImport += value; } remove { onAfterDataSourceImport -= value; } }
		protected internal virtual void RaiseAfterDataSourceImport(AfterDataSourceImportEventArgs e) {
			if (onAfterDataSourceImport != null)
				onAfterDataSourceImport(this, e);
		}
		#endregion
		#region DataSourceChanged
		event EventHandler dataSourceChanged;
		public event EventHandler DataSourceChanged { add { dataSourceChanged += value; } remove { dataSourceChanged -= value; } }
		protected virtual void RaiseDataSourceChanged() {
			if (dataSourceChanged != null)
				dataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#region AsynchronousOperationStarted
		event EventHandler asynchronousOperationStarted;
		public event EventHandler AsynchronousOperationStarted { add { asynchronousOperationStarted += value; } remove { asynchronousOperationStarted = Delegate.Remove(asynchronousOperationStarted, value) as EventHandler; } }
		protected virtual void RaiseAsynchronousOperationStarted() {
			if (dataSourceChanged != null)
				dataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#region AsynchronousOperationFinished
		event EventHandler asynchronousOperationFinished;
		public event EventHandler AsynchronousOperationFinished { add { asynchronousOperationFinished += value; } remove { asynchronousOperationFinished = Delegate.Remove(asynchronousOperationFinished, value) as EventHandler; } }
		protected virtual void RaiseAsynchronousOperationFinished() {
			if (asynchronousOperationFinished != null)
				asynchronousOperationFinished(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeStarted
		SnapMailMergeStartedEventHandler onSnapMailMergeStarted;
		public event SnapMailMergeStartedEventHandler SnapMailMergeStarted { add { onSnapMailMergeStarted += value; } remove { onSnapMailMergeStarted -= value; } }
		protected internal virtual void RaiseSnapMailMergeStarted(SnapMailMergeStartedEventArgs args) {
			if (onSnapMailMergeStarted != null)
				onSnapMailMergeStarted(Owner, args);
		}
		#endregion
		#region SnapMailMergeRecordStarted
		SnapMailMergeRecordStartedEventHandler onSnapMailMergeRecordStarted;
		public event SnapMailMergeRecordStartedEventHandler SnapMailMergeRecordStarted { add { onSnapMailMergeRecordStarted += value; } remove { onSnapMailMergeRecordStarted -= value; } }
		protected internal virtual void RaiseSnapMailMergeRecordStarted(SnapMailMergeRecordStartedEventArgs args) {
			if (onSnapMailMergeRecordStarted != null)
				onSnapMailMergeRecordStarted(Owner, args);
		}
		#endregion
		#region SnapMailMergeRecordFinished
		SnapMailMergeRecordFinishedEventHandler onSnapMailMergeRecordFinished;
		public event SnapMailMergeRecordFinishedEventHandler SnapMailMergeRecordFinished { add { onSnapMailMergeRecordFinished += value; } remove { onSnapMailMergeRecordFinished -= value; } }
		protected internal virtual void RaiseSnapMailMergeRecordFinished(SnapMailMergeRecordFinishedEventArgs args) {
			if (onSnapMailMergeRecordFinished != null)
				onSnapMailMergeRecordFinished(Owner, args);
		}
		#endregion
		#region SnapMailMergeFinished
		SnapMailMergeFinishedEventHandler onSnapMailMergeFinished;
		public event SnapMailMergeFinishedEventHandler SnapMailMergeFinished { add { onSnapMailMergeFinished += value; } remove { onSnapMailMergeFinished -= value; } }
		protected internal virtual void RaiseSnapMailMergeFinished(SnapMailMergeFinishedEventArgs args) {
			if (onSnapMailMergeFinished != null)
				onSnapMailMergeFinished(Owner, args);
		}
		#endregion
		#region DocumentClosing
		DocumentClosingEventHandler onDocumentClosing;
		public new event DocumentClosingEventHandler DocumentClosing { add { onDocumentClosing += value; } remove { onDocumentClosing -= value; } }
		protected internal override bool RaiseDocumentClosing() {
			if (onDocumentClosing != null) {
				DocumentDataSources interimDataSources = new DocumentDataSources(DocumentModel);
				DocumentClosingEventArgs e = new DocumentClosingEventArgs(interimDataSources);
				onDocumentClosing(this, e);
				if (e.Cancel) return false;
				if (e.Handled)
					SaveDataSources(e.InterimDataSources);
				else
					docDataSources = null;
			}
			return true;
		}
		#endregion
		#region EmptyDocumentCreated
		DocumentImportedEventHandler onEmptyDocumentCreated;
		public new event DocumentImportedEventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal override void RaiseEmptyDocumentCreated() {
			base.RaiseEmptyDocumentCreated();
			DocumentImportedEventArgs e = new DocumentImportedEventArgs(docDataSources);
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(this, e);
			LoadDataSource(e.InterimDataSources);
		}
		#endregion
		#region DocumentLoaded
		DocumentImportedEventHandler onDocumentLoaded;
		public new event DocumentImportedEventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal override void RaiseDocumentLoaded() {
			base.RaiseDocumentLoaded();
			DocumentImportedEventArgs e = new DocumentImportedEventArgs(docDataSources);
			if (onDocumentLoaded != null)
				onDocumentLoaded(this, e);
			LoadDataSource(e.InterimDataSources);
		}
		#endregion
		void SaveDataSources(DocumentDataSources interimDataSources) {
			docDataSources = new DocumentDataSources();
			for (int i = 0; i < interimDataSources.DataSources.Count; i++)
				if (interimDataSources.DataSources[i].DataSource != null)
					docDataSources.DataSources.AddWithReplace(interimDataSources.DataSources[i]);
		}
		void LoadDataSource(DocumentDataSources interimDataSources) {
			if (interimDataSources == null)
				return;
			for (int i = 0; i < interimDataSources.DataSources.Count; i++)
				if (interimDataSources.DataSources[i].DataSource != null) {
					if (!DocumentModel.ShouldChangeDataSourceName(interimDataSources.DataSources[i].DataSourceName))
						DocumentModel.DataSources.AddWithReplace(interimDataSources.DataSources[i]);
					else {
						string name = DocumentModel.GetNewDataSourceName(interimDataSources.DataSources[i].DataSourceName);
						DocumentModel.DataSources.Add(new DataSourceInfo(name, interimDataSources.DataSources[i].DataSource));
					}
				}
		}
		#endregion
		public new IInnerSnapDocumentServerOwner Owner { get { return (IInnerSnapDocumentServerOwner)base.Owner; } }
		public new SnapDocument Document { get { return (SnapDocument)base.Document; } }
		protected internal new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected internal new SnapNativeDocument NativeDocument { get { return (SnapNativeDocument)base.NativeDocument; } }
		public override void BeginInitialize() {
			base.BeginInitialize();
		}
		void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			DocumentModel.OnNamedDataSourceChanged(e);
			RaiseDataSourceChanged();
		}
		void OnDataSourceCollectionChanged(object sender, EventArgs e) {
			DocumentModel.OnDataSourceChanged();
			RaiseDataSourceChanged();
		}
		protected internal override XtraRichEdit.API.Native.Implementation.NativeDocument CreateNativeDocument() {
			return new SnapNativeDocument(DocumentModel.MainPieceTable, this);
		}
		protected override DocumentModel CreateDocumentModelCore() {
			return new SnapDocumentModel(new ServerDataSourceDispatcher(), SnapDocumentFormatsDependecies.CreateDocumentFormatsDependencies());
		}
		protected internal override CopySelectionManager CreateCopySelectionManager() {
			return new SnapCopySelectionManager(this);
		}
		protected internal override RichEditPrinter CreateRichEditPrinter() {
			return new SnapPrinter(this);
		}
		public byte[] SnxBytes {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.SnxBytes;
				else
					return null;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.SnxBytes = value;
			}
		}
		#region Snap Mail Merge
		#region SnapMailMergeActiveRecordChanging
		EventHandler onSnapMailMergeActiveRecordChanging;
		public event EventHandler SnapMailMergeActiveRecordChanging { add { onSnapMailMergeActiveRecordChanging += value; } remove { onSnapMailMergeActiveRecordChanging -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanging() {
			if (onSnapMailMergeActiveRecordChanging != null)
				onSnapMailMergeActiveRecordChanging(Owner, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeActiveRecordChanged
		EventHandler onSnapMailMergeActiveRecordChanged;
		public event EventHandler SnapMailMergeActiveRecordChanged { add { onSnapMailMergeActiveRecordChanged += value; } remove { onSnapMailMergeActiveRecordChanged -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanged() {
			if (onSnapMailMergeActiveRecordChanged != null)
				onSnapMailMergeActiveRecordChanged(Owner, EventArgs.Empty);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event EventHandler ActiveRecordChanging {
			add { base.ActiveRecordChanging += value; }
			remove { base.ActiveRecordChanging -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event EventHandler ActiveRecordChanged {
			add { base.ActiveRecordChanged += value; }
			remove { base.ActiveRecordChanged -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event CustomizeMergeFieldsEventHandler CustomizeMergeFields {
			add { base.CustomizeMergeFields += value; }
			remove { base.CustomizeMergeFields -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeStartedEventHandler MailMergeStarted {
			add { base.MailMergeStarted += value; }
			remove { base.MailMergeStarted -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeRecordStartedEventHandler MailMergeRecordStarted {
			add { base.MailMergeRecordStarted += value; }
			remove { base.MailMergeRecordStarted -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeRecordFinishedEventHandler MailMergeRecordFinished {
			add { base.MailMergeRecordFinished += value; }
			remove { base.MailMergeRecordFinished -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeFinishedEventHandler MailMergeFinished {
			add { base.MailMergeFinished += value; }
			remove { base.MailMergeFinished -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new ApiMailMergeOptions CreateMailMergeOptions() {
			return base.CreateMailMergeOptions();
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(Document document) {
			base.MailMerge(document);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, Document targetDocument) {
			base.MailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(IRichEditDocumentServer documentServer) {
			base.MailMerge(documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			base.MailMerge(options, targetDocumentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(string fileName, DocumentFormat format) {
			base.MailMerge(fileName, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(Stream stream, DocumentFormat format) {
			base.MailMerge(stream, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format) {
			base.MailMerge(options, fileName, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format) {
			base.MailMerge(options, stream, format);
		}
		public SnapMailMergeExportOptions CreateSnapMailMergeExportOptions() {
			NativeSnapMailMergeExportOptions result = new NativeSnapMailMergeExportOptions(DocumentModel, this);
			SnapDocumentModel model = (SnapDocumentModel)DocumentModel;
			result.CopyFrom(model.SnapMailMergeVisualOptions);
			return result;
		}
		public void SnapMailMerge(SnapDocument document) {
			SnapMailMerge(CreateSnapMailMergeExportOptions(), document);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument) {
			Guard.ArgumentNotNull(targetDocument, "targetDocument");
			SnapMailMerge(options, ((SnapNativeDocument)targetDocument).DocumentModel);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(ISnapDocumentServer documentServer) {
			SnapMailMerge(CreateSnapMailMergeExportOptions(), documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(SnapMailMergeExportOptions options, ISnapDocumentServer targetDocumentServer) {
			Guard.ArgumentNotNull(targetDocumentServer, "targetDocumentServer");
			SnapMailMerge(options, (SnapDocumentModel)targetDocumentServer.Model.DocumentModel);
		}
		public void SnapMailMerge(string fileName, DocumentFormat format) {
			SnapMailMerge(CreateSnapMailMergeExportOptions(), fileName, format);
		}
		public void SnapMailMerge(Stream stream, DocumentFormat format) {
			SnapMailMerge(CreateSnapMailMergeExportOptions(), stream, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				using (SnapDocumentModel resultDocumentModel = (SnapDocumentModel)DocumentModel.CreateNew()) {
					CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
					resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
					resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
					SnapMailMerge(options, resultDocumentModel);
					SaveDocumentCore(resultDocumentModel, stream, format, fileName);
					resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
				}
			}
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format) {
			using (SnapDocumentModel resultDocumentModel = (SnapDocumentModel)DocumentModel.CreateNew()) {
				CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
				resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
				resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
				SnapMailMerge(options, resultDocumentModel);
				SaveDocumentCore(resultDocumentModel, stream, format, String.Empty);
				resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
			}
		}
		protected internal bool SnapMailMerge(SnapMailMergeExportOptions options, SnapDocumentModel targetModel) {
			return SnapMailMerge(options, targetModel, new ProgressIndication(DocumentModel));
		}
		protected internal bool SnapMailMerge(SnapMailMergeExportOptions options, SnapDocumentModel targetModel, IProgressIndicationService progressIndication) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			Guard.ArgumentNotNull(options, "options");
			Guard.ArgumentNotNull(progressIndication, "progressIndication");
			bool result = false;
			targetModel.BeginUpdate();
			targetModel.InternalAPI.CreateNewDocument();
			try {
				targetModel.DocumentSaveOptions.CurrentFileName = DocumentModel.DocumentSaveOptions.CurrentFileName;
				SnapMailMergeHelper mergeHelper = new SnapMailMergeHelper(DocumentModel, options, progressIndication);
				mergeHelper.ExecuteMailMerge(targetModel);
				result = !mergeHelper.IsCancellationRequested;
				targetModel.DocumentSaveOptions.CurrentFileName = String.Empty;
			}
			finally {
				targetModel.EndUpdate();
			}
			return result;
		}
		#endregion
		protected internal override void SubscribeDocumentModelEvents() {
			base.SubscribeDocumentModelEvents();
			DocumentModel.BeforeConversion += OnBeforeConversion;
			DocumentModel.BeforeDataSourceExport += OnBeforeDataSourceExport;
			DocumentModel.AfterDataSourceImport += OnAfterDataSourceImport;
			DocumentModel.DataSourceChanged += OnDocumentModelDataSourceChanged;
			DocumentModel.AsynchronousOperationStarted += OnDocumentModelAsynchronousOperationStarted;
			DocumentModel.AsynchronousOperationFinished += OnDocumentModelAsynchronousOperationFinished;
			DocumentModel.SnapMailMergeStarted += OnSnapMailMergeStarted;
			DocumentModel.SnapMailMergeRecordStarted += OnSnapMailMergeRecordStarted;
			DocumentModel.SnapMailMergeRecordFinished += OnSnapMailMergeRecordFinished;
			DocumentModel.SnapMailMergeFinished += OnSnapMailMergeFinished;
			DocumentModel.SnapMailMergeActiveRecordChanging += OnSnapMailMergeActiveRecordChanging;
			DocumentModel.SnapMailMergeActiveRecordChanged += OnSnapMailMergeActiveRecordChanged;
		}
		protected internal override void UnsubscribeDocumentModelEvents() {
			base.UnsubscribeDocumentModelEvents();
			DocumentModel.BeforeConversion -= OnBeforeConversion;
			DocumentModel.BeforeDataSourceExport -= OnBeforeDataSourceExport;
			DocumentModel.AfterDataSourceImport -= OnAfterDataSourceImport;
			DocumentModel.DataSourceChanged -= OnDocumentModelDataSourceChanged;
			DocumentModel.AsynchronousOperationStarted -= OnDocumentModelAsynchronousOperationStarted;
			DocumentModel.AsynchronousOperationFinished -= OnDocumentModelAsynchronousOperationFinished;
			DocumentModel.SnapMailMergeStarted -= OnSnapMailMergeStarted;
			DocumentModel.SnapMailMergeRecordStarted -= OnSnapMailMergeRecordStarted;
			DocumentModel.SnapMailMergeRecordFinished -= OnSnapMailMergeRecordFinished;
			DocumentModel.SnapMailMergeFinished -= OnSnapMailMergeFinished;
			DocumentModel.SnapMailMergeActiveRecordChanging -= OnSnapMailMergeActiveRecordChanging;
			DocumentModel.SnapMailMergeActiveRecordChanged -= OnSnapMailMergeActiveRecordChanged;
		}
		protected override void OnOptionsMailMergeChanged() {
		}
		void OnBeforeConversion(object sender, EventArgs e) {
			RaiseBeforeConversion();
		}
		void OnDocumentModelDataSourceChanged(object sender, EventArgs e) {
			RaiseDataSourceChanged();
		}
		void OnBeforeDataSourceExport(object sender, BeforeDataSourceExportEventArgs e) {
			RaiseBeforeDataSourceExport(e);
		}
		void OnAfterDataSourceImport(object sender, AfterDataSourceImportEventArgs e) {
			RaiseAfterDataSourceImport(e);
		}
		void OnDocumentModelAsynchronousOperationStarted(object sender, EventArgs e) {
			RaiseAsynchronousOperationStarted();
		}
		void OnDocumentModelAsynchronousOperationFinished(object sender, EventArgs e) {
			RaiseAsynchronousOperationFinished();
		}
		protected internal virtual void OnSnapMailMergeStarted(object sender, SnapMailMergeStartedEventArgs e) {
			RaiseSnapMailMergeStarted(e);
		}
		protected internal virtual void OnSnapMailMergeRecordStarted(object sender, SnapMailMergeRecordStartedEventArgs e) {
			RaiseSnapMailMergeRecordStarted(e);
		}
		protected internal virtual void OnSnapMailMergeRecordFinished(object sender, SnapMailMergeRecordFinishedEventArgs e) {
			RaiseSnapMailMergeRecordFinished(e);
		}
		protected internal virtual void OnSnapMailMergeFinished(object sender, SnapMailMergeFinishedEventArgs e) {
			RaiseSnapMailMergeFinished(e);
		}
		protected internal virtual void OnSnapMailMergeActiveRecordChanging(object sender, EventArgs e) {
			RaiseSnapMailMergeActiveRecordChanging();
		}
		protected internal virtual void OnSnapMailMergeActiveRecordChanged(object sender, EventArgs e) {
			RaiseSnapMailMergeActiveRecordChanged();
		}
		protected internal override bool CanCloseExistingDocumentCore() {
			if (!base.CanCloseExistingDocumentCore())
				return false;
			return Document.DataSource == null && Document.DataSources.Count == 0;
		}
	}
}
