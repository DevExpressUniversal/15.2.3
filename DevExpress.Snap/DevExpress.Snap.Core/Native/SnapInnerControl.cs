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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native.Printing;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.Options;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
namespace DevExpress.Snap.Core.Native {
	public interface IInnerSnapDocumentServerOwner : IInnerRichEditDocumentServerOwner { }
	public class InnerSnapControl : InnerRichEditControl, ISnapDocumentServer {
		FieldHoverInfo hoveredMergeFieldInfo;
		readonly List<ILayoutUIElementViewInfo> activeUIElementViewInfos;
		readonly List<ILayoutUIElement> activeUIElements;
		readonly List<ILayoutUIElement> hotTrackUIElements;
		DataSourceInfoCollection dataSources;
		DocumentDataSources docDataSources;
		public InnerSnapControl(ISnapInnerControlOwner owner)
			: base(owner) {
			this.activeUIElementViewInfos = new List<ILayoutUIElementViewInfo>();
			this.activeUIElements = new List<ILayoutUIElement>();
			this.hotTrackUIElements = new List<ILayoutUIElement>();
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public new SnapDocument Document { get { return (SnapDocument)base.Document; } }
		public new ISnapInnerControlOwner Owner { get { return (ISnapInnerControlOwner)base.Owner; } }
		protected internal bool DrawTemplateDecorators { get; set; }
		protected internal bool ForceDrawTemplateDecorators { get; set; }
		public DataSourceInfoCollection DataSources { get { return dataSources; } }
		#region SnxBytes
		public byte[] SnxBytes {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.SnxBytes;
				return null;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.SnxBytes = value;
			}
		}
		#endregion
		public List<ILayoutUIElementViewInfo> ActiveUIElementViewInfos { get { return activeUIElementViewInfos; } }
		public List<ILayoutUIElement> ActiveUIElements { get { return activeUIElements; } }
		public List<ILayoutUIElement> HotTrackUIElements { get { return hotTrackUIElements; } }
		#region Events
		BeforeConversionEventHandler onBeforeConversion;
		public event BeforeConversionEventHandler BeforeConversion { add { onBeforeConversion += value; } remove { onBeforeConversion -= value; } }
		protected internal virtual void RaiseBeforeConversion() {
			if (onBeforeConversion != null)
				onBeforeConversion(Owner, new BeforeConversionEventArgs(Document));
		}
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
		DataSourceInfoChangedEventHandler dataSourceChanged;
		public event DataSourceInfoChangedEventHandler DataSourceChanged { add { dataSourceChanged += value; } remove { dataSourceChanged -= value; } }
		protected virtual void RaiseDataSourceChanged(DataSourceOwner owner) {
			if (dataSourceChanged != null)
				dataSourceChanged(this, new DataSourceInfoChangedEventArgs(owner));
		}
		#endregion
		#region AsynchronousOperationStarted
		event EventHandler asynchronousOperationStarted;
		public event EventHandler AsynchronousOperationStarted { add { asynchronousOperationStarted += value; } remove { asynchronousOperationStarted = Delegate.Remove(asynchronousOperationStarted, value) as EventHandler; } }
		protected virtual void RaiseAsynchronousOperationStarted() {
			if (asynchronousOperationStarted != null)
				asynchronousOperationStarted(this, EventArgs.Empty);
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
		#region DocumentClosing
		DocumentClosingEventHandler onDocumentClosing;
		public new event DocumentClosingEventHandler DocumentClosing { add { onDocumentClosing += value; } remove { onDocumentClosing -= value; } }
		protected internal override bool RaiseDocumentClosing() {
			bool shouldShowDataSourceSaveForm = DocumentModel.HasNonEmptyDataSource();
			DocumentDataSources interimDataSources = new DocumentDataSources(DocumentModel);
			if (onDocumentClosing != null) {
				DocumentClosingEventArgs e = new DocumentClosingEventArgs(interimDataSources);
				onDocumentClosing(this, e);
				if (e.Cancel) return false;
				if (e.Handled)
					shouldShowDataSourceSaveForm = false;
			}
			if (shouldShowDataSourceSaveForm) {
				ISnapControl control = this.Owner as ISnapControl;
				if (control != null) {
					DialogResult result = control.ShowDataSourceSaveForm();
					if (result == DialogResult.Cancel)
						return false;
					if (result == DialogResult.No) {
						docDataSources = null;
						return true;
					}
				}
			}
			SaveDataSources(interimDataSources);
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
		public override void BeginInitialize() {
			this.dataSources = new DataSourceInfoCollection(false);
			base.BeginInitialize();
		}
		public override void EndInitialize() {
			base.EndInitialize();
			SubscribeToEvents();
		}
		void SubscribeToEvents() {
			this.dataSources.DataSourceChanged += OnDataSourceChanged;
			this.dataSources.CollectionChanged += OnDataSourceCollectionChanged;
		}
		void UnsubscribeFromEvents() {
			this.dataSources.DataSourceChanged -= OnDataSourceChanged;
			this.dataSources.CollectionChanged -= OnDataSourceCollectionChanged;
		}
		void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			DocumentModel.OnNamedDataSourceChanged(e);
			RaiseDataSourceChanged(DataSourceOwner.Control);
		}
		void OnDataSourceCollectionChanged(object sender, EventArgs e) {
			DocumentModel.OnDataSourceChanged();
			RaiseDataSourceChanged(DataSourceOwner.Control);
		}
		protected override void OnOptionsMailMergeChanged() {
		}
		public virtual bool ExportDocument() {
			return ExportDocument(Owner);
		}
		public virtual bool ExportDocument(IWin32Window parent) {
			return ExportDocumentCore(parent, e => ((SnapDocumentExportManagerService)e).GetDefaultExporters(), DocumentModel.FileExportOptions);
		}
		protected internal override void LoadDocument(string fileName, DocumentFormat documentFormat, bool loadAsTemplate) {
			base.LoadDocument(fileName, documentFormat, loadAsTemplate);
			DocumentModel.FileExportOptions.CurrentFileName = DocumentModel.DocumentSaveOptions.CurrentFileName;
		}
		protected override XtraRichEdit.Model.DocumentModel CreateDocumentModelCore() {
			IDataSourceDispatcher dataSourceDispatcher;
			ISnapControl snapControl = Owner as ISnapControl;
			if (object.ReferenceEquals(snapControl, null))
				dataSourceDispatcher = new ServerDataSourceDispatcher();
			else
				dataSourceDispatcher = new ControlDataSourceDispatcher(snapControl);
			return new SnapDocumentModel(dataSourceDispatcher, SnapDocumentFormatsDependecies.CreateDocumentFormatsDependencies());
		}
		protected internal override XtraRichEdit.API.Native.Implementation.NativeDocument CreateNativeDocument() {
			return new SnapNativeDocument(DocumentModel.MainPieceTable, this);
		}
		protected override XtraRichEdit.Layout.Engine.BackgroundFormatter CreateBackgroundFormatter(XtraRichEdit.Layout.Engine.DocumentFormattingController controller) {
			return new SnapBackgroundFormatter(controller, Owner.CommentPadding);
		}
		protected internal override XtraRichEdit.Printing.RichEditPrinter CreateRichEditPrinter() {
			return new SnapPrinter(this);
		}
		protected internal override void InitializeDefaultViewKeyboardHandlers(XtraRichEdit.Keyboard.NormalKeyboardHandler keyboardHandler, Utils.KeyboardHandler.IKeyHashProvider provider) {
			base.InitializeDefaultViewKeyboardHandlers(keyboardHandler, provider);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.Control | Keys.Shift, SnapCommandId.CreateFieldForTemplate);
		}
		protected internal override void OnDragDrop(System.Windows.Forms.DragEventArgs e) {
			base.OnDragDrop(e);
			if (Owner != null)
				Owner.Focus();
		}
		protected internal override void OnSelectionChanged(object sender, EventArgs e) {
			base.OnSelectionChanged(sender, e);
			OnSelectionChangedCore();
		}
		protected internal override void OnReadOnlyChanged() {
			base.OnReadOnlyChanged();
			OnSelectionChangedCore();
		}
		void OnSelectionChangedCore() {
			Action method = delegate { OnSelectionChangedAsync(); };
			if (IsHandleCreated)
				Owner.BeginInvokeMethod(method);
		}
		void OnSelectionChangedAsync() {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField(this.DocumentModel);
			MergefieldField mergefieldField = fieldInfo == null ? null : calculator.ParseField(fieldInfo) as MergefieldField;
			UpdateSelectedFieldService(fieldInfo, mergefieldField);
			if (DocumentModel.SelectionInfo.CheckCurrentSnapBookmark()) {
				Owner.Redraw();
			}
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
		void UpdateSelectedFieldService(SnapFieldInfo fieldInfo, MergefieldField parsedField) {
			ISelectedFieldService service = GetService<ISelectedFieldService>();
			service.UpdateSelectedField(fieldInfo, parsedField);
		}
		protected internal virtual void OnCurrentFieldChanged(PieceTable pieceTable, Field field) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SnapFieldInfo fieldInfo = new SnapFieldInfo((SnapPieceTable)pieceTable, field);
			if (field != null)
				hoveredMergeFieldInfo = new FieldHoverInfo(calculator.ParseField(fieldInfo), fieldInfo);
		}
		protected internal virtual void OnCurrentFieldChanging(PieceTable pieceTable, Field field) {
			if (hoveredMergeFieldInfo != null && hoveredMergeFieldInfo.Field == field) {
				hoveredMergeFieldInfo = null;
			}
		}
		public override RichEditCommand CreatePasteDataObjectCommand(System.Windows.Forms.IDataObject dataObject) {
			return new SnapPasteDataObjectCoreCommand(Owner, dataObject);
		}
		public SnapSelectionInfo GetSelectionInfo() {
			return DocumentModel.SelectionInfo;
		}
		protected internal override CopySelectionManager CreateCopySelectionManager() {
			return new SnapCopySelectionManager(this);
		}
		protected internal override MouseCursorCalculator CreateMouseCursorCalculator() {
			return new SnapMouseCursorCalculator(this, ActiveView);
		}
		#region Snap Mail Merge
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
			DocumentModel.DataSourceChanged += OnDataSourceChanged;
			DocumentModel.AsynchronousOperationStarted += OnAsynchronousOperationStarted;
			DocumentModel.AsynchronousOperationFinished += OnAsynchronousOperationFinished;
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
			DocumentModel.DataSourceChanged -= OnDataSourceChanged;
			DocumentModel.AsynchronousOperationStarted -= OnAsynchronousOperationStarted;
			DocumentModel.AsynchronousOperationFinished -= OnAsynchronousOperationFinished;
			DocumentModel.SnapMailMergeStarted -= OnSnapMailMergeStarted;
			DocumentModel.SnapMailMergeRecordStarted -= OnSnapMailMergeRecordStarted;
			DocumentModel.SnapMailMergeRecordFinished -= OnSnapMailMergeRecordFinished;
			DocumentModel.SnapMailMergeFinished -= OnSnapMailMergeFinished;
			DocumentModel.SnapMailMergeActiveRecordChanging -= OnSnapMailMergeActiveRecordChanging;
			DocumentModel.SnapMailMergeActiveRecordChanged -= OnSnapMailMergeActiveRecordChanged;
		}
		protected internal virtual void OnBeforeConversion(object sender, EventArgs e) {
			RaiseBeforeConversion();
		}
		protected internal virtual void OnBeforeDataSourceExport(object sender, BeforeDataSourceExportEventArgs e) {
			RaiseBeforeDataSourceExport(e);
		}
		protected internal virtual void OnAfterDataSourceImport(object sender, AfterDataSourceImportEventArgs e) {
			RaiseAfterDataSourceImport(e);
		}
		protected internal virtual void OnDataSourceChanged(object sender, EventArgs e) {
			RaiseDataSourceChanged(DataSourceOwner.Document);
		}
		protected internal virtual void OnAsynchronousOperationStarted(object sender, EventArgs e) {
			RaiseAsynchronousOperationStarted();
		}
		protected internal virtual void OnAsynchronousOperationFinished(object sender, EventArgs e) {
			RaiseAsynchronousOperationFinished();
		}
		protected internal override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeFromEvents();
			}
			base.Dispose(disposing);
		}
		protected internal override bool CanCloseExistingDocumentCore() {
			if (!base.CanCloseExistingDocumentCore())
				return false;
			return Document.DataSource == null && Document.DataSources.Count == 0;
		}
	}
	public class FieldHoverInfo {
		readonly CalculatedFieldBase parsedInfo;
		readonly Field field;
		readonly PieceTable pieceTable;
		public FieldHoverInfo(CalculatedFieldBase parsedInfo, SnapFieldInfo fieldInfo)
			: this(parsedInfo, fieldInfo.Field, fieldInfo.PieceTable) {
		}
		public FieldHoverInfo(CalculatedFieldBase parsedInfo, Field field, PieceTable pieceTable) {
			Guard.ArgumentNotNull(field, "field");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.field = field;
			this.parsedInfo = parsedInfo;
		}
		public CalculatedFieldBase ParsedInfo { get { return parsedInfo; } }
		public Field Field { get { return field; } }
		public PieceTable PieceTable { get { return pieceTable; } }
	}
	public interface ILayoutToPhysicalBoundsConverter {
		Rectangle GetPixelPhysicalBounds(Rectangle layoutRect);
	}
}
