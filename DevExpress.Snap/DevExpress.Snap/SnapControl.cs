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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Design;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Native;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Office.Internal;
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.MouseHandler;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Forms;
using DevExpress.Snap.Native;
using DevExpress.Snap.Native.HoverDecorators;
using DevExpress.Snap.Native.LayoutUI;
using DevExpress.Snap.Native.PageDecorators;
using DevExpress.Snap.Native.Services;
using DevExpress.Utils;
using DevExpress.Utils.UI;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraReports.Design;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Services;
using System.Drawing.Design;
using DevExpress.Snap.Services;
using DevExpress.Snap.Core.Services;
using DevExpress.Services;
using DevExpress.Office.Utils;
using Theme = DevExpress.Snap.Core.Native.Theme;
using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using DevExpress.Snap.Core.Options;
using Document = DevExpress.XtraRichEdit.API.Native.Document;
using ApiMailMergeOptions = DevExpress.XtraRichEdit.API.Native.MailMergeOptions;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Snap {
	[Designer("DevExpress.Snap.Design.SnapControlDesigner," + AssemblyInfo.SRAssemblySnapDesign, typeof(IDesigner))]
	[DXToolboxItem(true)]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	[ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "SnapControl.bmp")]
	[Description("A present-day solution to create user-friendly reporting applications based on a sterling and robust word processing engine.")]
	public class SnapControl : RichEditControl, ISnapControl, ISnapInnerControlOwner {
		public new static void About() {
		}
		protected override void Validate() {
		}
		protected override InnerRichEditControl CreateInnerControl() {
			return new InnerSnapControl(this);
		}
		protected internal new InnerSnapControl InnerControl { get { return (InnerSnapControl)base.InnerControl; } }
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlDocument")]
#endif
		public new SnapDocument Document { get { return (SnapDocument)base.Document; } }
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlDataSources"),
#endif
		DevExpress.XtraEditors.DXCategory(DevExpress.XtraEditors.CategoryName.Data)]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.Snap.Design.DataSourceInfoCollectionEditor," + AssemblyInfo.SRAssemblySnapDesign, typeof(UITypeEditor))]
		public DataSourceInfoCollection DataSources { get { return InnerControl != null ? InnerControl.DataSources : null; } }
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlDataSource"),
#endif
		AttributeProvider(typeof(IListSource))]
		[DefaultValue(null), DevExpress.XtraEditors.DXCategory(DevExpress.XtraEditors.CategoryName.Data)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public object DataSource {
			get {
				if (DataSources == null || DataSources.DefaultDataSourceInfo == null)
					return null;
				return DataSources.DefaultDataSourceInfo.DataSource;
			}
			set {
				if (DataSources == null)
					return;
				DataSources.SetDefaultDataSource(value);
			}
		}
		protected internal new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlOptions"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SnapControlOptions Options { get { return (SnapControlOptions)base.Options; } }
		[
#if !SL
	DevExpressSnapLocalizedDescription("SnapControlOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		protected override DocumentLayoutUnit DefaultLayoutUnit {
			get {
				return DocumentLayoutUnit.Document;
			}
		}
		[Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		#region SnxBytes
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
		#endregion
		#region Events
		static readonly object onReportStructureEditorFormShowing = new object();
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlReportStructureEditorFormShowing")]
#endif
		public event ReportStructureEditorFormShowingEventHandler ReportStructureEditorFormShowing {
			add { Events.AddHandler(onReportStructureEditorFormShowing, value); }
			remove { Events.RemoveHandler(onReportStructureEditorFormShowing, value); }
		}
		protected internal virtual void RaiseReportStructureEditorFormShowing(ReportStructureEditorFormShowingEventArgs e) {
			ReportStructureEditorFormShowingEventHandler handler = (ReportStructureEditorFormShowingEventHandler)Events[onReportStructureEditorFormShowing];
			if (handler != null)
				handler(this, e);
		}
		static readonly object onMailMergeExportFormShowing = new object();
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlMailMergeExportFormShowing")]
#endif
		public event MailMergeExportFormShowingEventHandler MailMergeExportFormShowing {
			add { Events.AddHandler(onMailMergeExportFormShowing, value); }
			remove { Events.RemoveHandler(onMailMergeExportFormShowing, value); }
		}
		protected internal virtual void RaiseMailMergeExportFormShowing(MailMergeExportFormShowingEventArgs e) {
			MailMergeExportFormShowingEventHandler handler = (MailMergeExportFormShowingEventHandler)Events[onMailMergeExportFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#region BeforeConversion
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlBeforeConversion")]
#endif
		public event BeforeConversionEventHandler BeforeConversion {
			add {
				if (InnerControl != null)
					InnerControl.BeforeConversion += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.BeforeConversion -= value;
			}
		}
		#endregion
		#region BeforeDataSourceExport
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlBeforeDataSourceExport")]
#endif
		public event BeforeDataSourceExportEventHandler BeforeDataSourceExport {
			add {
				if (InnerControl != null)
					InnerControl.BeforeDataSourceExport += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.BeforeDataSourceExport -= value;
			}
		}
		#endregion
		#region AfterDataSourceImport
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlAfterDataSourceImport")]
#endif
		public event AfterDataSourceImportEventHandler AfterDataSourceImport {
			add {
				if (InnerControl != null)
					InnerControl.AfterDataSourceImport += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.AfterDataSourceImport -= value;
			}
		}
		#endregion
		#region DataSourceChanged
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlDataSourceChanged")]
#endif
		public event DataSourceInfoChangedEventHandler DataSourceChanged {
			add {
				if (InnerControl != null)
					InnerControl.DataSourceChanged += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.DataSourceChanged -= value;
			}
		}
		#endregion
		#region AsynchronousOperationStarted
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlAsynchronousOperationStarted")]
#endif
		public event EventHandler AsynchronousOperationStarted {
			add {
				if (InnerControl != null)
					InnerControl.AsynchronousOperationStarted += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.AsynchronousOperationStarted -= value;
			}
		}
		#endregion
		#region AsynchronousOperationFinished
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlAsynchronousOperationFinished")]
#endif
		public event EventHandler AsynchronousOperationFinished {
			add {
				if (InnerControl != null)
					InnerControl.AsynchronousOperationFinished += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.AsynchronousOperationFinished -= value;
			}
		}
		#endregion
		#region SnapMailMergeActiveRecordChanging
		EventHandler onSnapMailMergeActiveRecordChanging;
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeActiveRecordChanging")]
#endif
		public event EventHandler SnapMailMergeActiveRecordChanging { add { onSnapMailMergeActiveRecordChanging += value; } remove { onSnapMailMergeActiveRecordChanging -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanging() {
			if (onSnapMailMergeActiveRecordChanging != null)
				onSnapMailMergeActiveRecordChanging(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeActiveRecordChanged
		EventHandler onSnapMailMergeActiveRecordChanged;
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeActiveRecordChanged")]
#endif
		public event EventHandler SnapMailMergeActiveRecordChanged { add { onSnapMailMergeActiveRecordChanged += value; } remove { onSnapMailMergeActiveRecordChanged -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanged() {
			if (onSnapMailMergeActiveRecordChanged != null)
				onSnapMailMergeActiveRecordChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeStarted
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeStarted")]
#endif
		public event SnapMailMergeStartedEventHandler SnapMailMergeStarted {
			add { if (InnerControl != null) InnerControl.SnapMailMergeStarted += value; }
			remove { if (InnerControl != null) InnerControl.SnapMailMergeStarted -= value; }
		}
		#endregion
		#region SnapMailMergeRecordStarted
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeRecordStarted")]
#endif
		public event SnapMailMergeRecordStartedEventHandler SnapMailMergeRecordStarted {
			add { if (InnerControl != null) InnerControl.SnapMailMergeRecordStarted += value; }
			remove { if (InnerControl != null) InnerControl.SnapMailMergeRecordStarted -= value; }
		}
		#endregion
		#region SnapMailMergeRecordFinished
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeRecordFinished")]
#endif
		public event SnapMailMergeRecordFinishedEventHandler SnapMailMergeRecordFinished {
			add { if (InnerControl != null) InnerControl.SnapMailMergeRecordFinished += value; }
			remove { if (InnerControl != null) InnerControl.SnapMailMergeRecordFinished -= value; }
		}
		#endregion
		#region SnapMailMergeFinished
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlSnapMailMergeFinished")]
#endif
		public event SnapMailMergeFinishedEventHandler SnapMailMergeFinished {
			add { if (InnerControl != null) InnerControl.SnapMailMergeFinished += value; }
			remove { if (InnerControl != null) InnerControl.SnapMailMergeFinished -= value; }
		}
		#endregion
		#region DocumentClosing
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlDocumentClosing")]
#endif
		public new event DocumentClosingEventHandler DocumentClosing {
			add {
				if (InnerControl != null)
					InnerControl.DocumentClosing += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.DocumentClosing -= value;
			}
		}
		#endregion
		#region DocumentLoaded
		DocumentImportedEventHandler onDocumentLoaded;
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlDocumentLoaded")]
#endif
		public new event DocumentImportedEventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected virtual void RaiseDocumentLoaded(DocumentImportedEventArgs e) {
			if (onDocumentLoaded != null)
				onDocumentLoaded(this, e);
		}
		#endregion
		#region EmptyDocumentCreated
		DocumentImportedEventHandler onEmptyDocumentCreated;
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlEmptyDocumentCreated")]
#endif
		public new event DocumentImportedEventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected virtual void RaiseEmptyDocumentCreated(DocumentImportedEventArgs e) {
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(this, e);
		}
		#endregion
		#endregion
		RichEditControlOptionsBase IInnerRichEditDocumentServerOwner.CreateOptions(InnerRichEditDocumentServer documentServer) {
			return new SnapControlOptions(documentServer);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
		bool ShouldSerializeDataSources() {
			if (DataSources.Count != 1)
				return true;
			DataSourceInfo info = DataSources[0];
			return info.DataSourceName != DataSourceInfoCollection.DefaultDataSourceInfoName || info.DataSource != null;
		}
		void ResetDataSources() {
			DataSources.Clear();			
		}
		protected internal bool IsSelectionInSNList() {
			return FieldsHelper.GetSelectedSNListField(DocumentModel) != null;
		}
		protected internal bool IsSNFieldSelected() {
			SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField(DocumentModel);
			return fieldInfo != null && fieldInfo.ParsedInfo is SNMergeFieldBase;
		}
		protected override void AddServicesPlatformSpecific() {
			base.AddServicesPlatformSpecific();
			SNMenuCommandService menuCommandService = new SNMenuCommandService(this);
			AddService(typeof(IXRMenuCommandService), menuCommandService);
			AddService(typeof(IMenuCommandService), menuCommandService);
			MenuCommandHandler menuCommandHandler = new MenuCommandHandler(this);
			menuCommandHandler.RegisterMenuCommands();
			AddService(typeof(MenuCommandHandler), menuCommandHandler);
			AddService(typeof(IDataContextService), new DataContextService(this.DocumentModel));
			AddService(typeof(IDataSourceCollectorService), new DataSourceCollectorService(this.DocumentModel));
			AddService(typeof(IMessageBoxService), new MessageBoxService());
			RemoveService(typeof(IToolTipService));
			AddService(typeof(IToolTipService), new SnapToolTipService(this));
			AddService(typeof(ILookAndFeelService), new LookAndFeelService());
			IWaitFormActivator waitFormActivator = new WaitFormActivator(FindForm(), typeof(WaitFormWithCancel));
			AddService(typeof(IWaitFormActivator), waitFormActivator);
			AddService(typeof(IParameterCreator), new ParameterCreator());
			AddService(typeof(ISnapMailMergeProgressIndicationService), new SnapProgressIndicationService(this));
		}
		void ISnapInnerControlOwner.BeginInvokeMethod(Delegate method) {
			BeginInvoke(method);
		}
		protected override RichEditPopupMenu RaisePopupMenuShowing(RichEditPopupMenu menu) {
			RichEditContentMenuBuilder builder = CreateSnapContentMenuBuilder();
			menu = (RichEditPopupMenu)builder.CreatePopupMenu();
			return base.RaisePopupMenuShowing(menu);
		}
		protected virtual RichEditContentMenuBuilder CreateSnapContentMenuBuilder() {
			if(WindowsFormsSettings.PopupMenuStyle == PopupMenuStyle.RadialMenu) {
				return new WinFormsSnapContentRadialMenuBuilder(this, new WinFormsRichEditMenuBuilderUIFactory());
			}
			return new WinFormsSnapContentMenuBuilder(this, new WinFormsRichEditMenuBuilderUIFactory());
		}
		protected override void CreateViewPainter(RichEditView view) {
			base.CreateViewPainter(view);
			ViewPainter.AddDecoratorPainter(new TemplateDecoratorPainter(view));
			ViewPainter.AddHoverPainter<ChartBoxHoverLayout, ChartBoxHoverPainter, ChartBox>();
			ViewPainter.AddHoverPainter<SparklineBoxHoverLayout, SparklineBoxHoverPainter, SparklineBox>();
			AddHoverLayoutItems();
			AddTableViewInfoController();
		}
		void AddHoverLayoutItems() {
			IHoverLayoutItemsService hoverLayoutItems = DocumentModel.GetService<IHoverLayoutItemsService>();
			hoverLayoutItems.AddIfNotPresent<ChartBox, ChartBoxHoverLayout>();
			hoverLayoutItems.AddIfNotPresent<SparklineBox, SparklineBoxHoverLayout>();
		}
		void AddTableViewInfoController() {
			ITableViewInfoControllerService controllerService = DocumentModel.GetService<ITableViewInfoControllerService>();
			controllerService.AddIfNotPresent<SnapDragExternalContentMouseHandlerState, DragExternalContentTableViewInfoController>();
		}
		protected override void SubscribeInnerControlEvents() {
			base.SubscribeInnerControlEvents();
			InnerControl.DataSources.DataSourceNameChanged += OnDataSourceNameChanged;
			InnerControl.SnapMailMergeActiveRecordChanging += OnSnapMailMergeActiveRecordChanging;
			InnerControl.SnapMailMergeActiveRecordChanged += OnSnapMailMergeActiveRecordChanged;
			InnerControl.EmptyDocumentCreated += OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded += OnDocumentLoaded;
		}
		protected override void UnsubscribeInnerControlEvents() {
			base.UnsubscribeInnerControlEvents();
			InnerControl.DataSources.DataSourceNameChanged -= OnDataSourceNameChanged;
			InnerControl.SnapMailMergeActiveRecordChanging -= OnSnapMailMergeActiveRecordChanging;
			InnerControl.SnapMailMergeActiveRecordChanged -= OnSnapMailMergeActiveRecordChanged;
			InnerControl.EmptyDocumentCreated -= OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded -= OnDocumentLoaded;
		}
		void OnDocumentLoaded(object sender, DocumentImportedEventArgs e) {
			RaiseDocumentLoaded(e);
		}
		void OnEmptyDocumentCreated(object sender, DocumentImportedEventArgs e) {
			RaiseEmptyDocumentCreated(e);
		}
		void OnDataSourceNameChanged(object sender, DataSourceNameChangedEventArgs e) {
			if (!IsDesignMode)
				throw new InvalidOperationException(SnapLocalizer.GetString(SnapStringId.Msg_CannotChangeDataSourceName));
		}
		protected internal virtual void OnSnapMailMergeActiveRecordChanging(object sender, EventArgs e) {
			RaiseSnapMailMergeActiveRecordChanging();
		}
		protected internal virtual void OnSnapMailMergeActiveRecordChanged(object sender, EventArgs e) {
			RaiseSnapMailMergeActiveRecordChanged();
		}
		protected internal new Rectangle GetPixelPhysicalBounds(PageViewInfo pageViewInfo, Rectangle logicalBounds) {
			return base.GetPixelPhysicalBounds(pageViewInfo, logicalBounds);
		}
		RichEditMouseHandler IInnerRichEditControlOwner.CreateMouseHandler() {
			SnapMouseHandler result = new SnapMouseHandler(this);
			return result;
		}
		protected override IRichEditCommandFactoryService CreateRichEditCommandFactoryService() {
			IRichEditCommandFactoryService baseService = base.CreateRichEditCommandFactoryService();
			return new SnapCommandFactoryService(this, baseService);
		}
		RichEditMouseHandlerStrategyFactory IRichEditControl.CreateRichEditMouseHandlerStrategyFactory() {
			return new WinFormsSnapMouseHandlerStrategyFactory();
		}
		public override IRichEditDocumentServer CreateDocumentServer() {
			return new InternalSnapDocumentServer(DocumentModel.DocumentFormatsDependencies);
		}
		#region ISnapControl Members
		DialogResult ISnapControl.ShowDataSourceSaveForm() {
			if (this.DocumentModel.DataSources.Count == 0)
				return DialogResult.None;
			bool nullDataSources = true;
			for (int i = 0; i < this.DocumentModel.DataSources.Count; i++) {
				if (this.DocumentModel.DataSources[i].DataSource != null) {
					nullDataSources = false;
					break;
				}
			}
			if (nullDataSources)
				return DialogResult.No;
			return XtraMessageBox.Show("Maintain the data source connections that exist in the currently open document?", "Import Template", MessageBoxButtons.YesNoCancel);
		}
		DialogResult ISnapControl.ShowChartDesigner(IChartContainer chartContainer) {
			return new DevExpress.XtraCharts.Designer.ChartDesigner(chartContainer).ShowDialog();
		}
		bool ISnapControl.ShowFilterStringEditorForm(DesignBinding binding, ref string filterString) {
			FilterStringEditorForm form = new FilterStringEditorForm(this, binding.DataSource, binding.DataMember, this.Document.Parameters, null);
			form.FilterString = filterString;
			System.Windows.Forms.DialogResult result = form.ShowDialog();
			filterString = form.FilterString;
			return result != System.Windows.Forms.DialogResult.Cancel;
		}
		void ISnapControl.ShowReportStructureEditorForm() {
			ReportStructureEditorFormControllerParameters controllerParameters = new ReportStructureEditorFormControllerParameters(this);
			ReportStructureEditorFormShowingEventArgs args = new ReportStructureEditorFormShowingEventArgs(controllerParameters);
			RaiseReportStructureEditorFormShowing(args);
			if (!args.Handled) {
				using (ReportStructureEditorForm form = new ReportStructureEditorForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					form.ShowDialog(this);
				}
			}
		}
		void ISnapControl.ShowMailMergeSortingForm() {
			SortingFormControllerBaseParameters controllerParameters = new SortingFormControllerBaseParameters(this);
			using (MailMergeSortingForm form = new MailMergeSortingForm(controllerParameters)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				form.ShowDialog(this);
			}
		}
		#region Snap Mail Merge
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
		void ISnapControl.ShowMailMergeExportOptionsForm(SnapMailMergeExportOptions properties, ShowMailMergeExportOptionsFormCallback callback, object data) {
			MailMergeExportFormControllerParameters controllerParameters = new MailMergeExportFormControllerParameters(this, properties);
			MailMergeExportFormShowingEventArgs args = new MailMergeExportFormShowingEventArgs(controllerParameters);
			RaiseMailMergeExportFormShowing(args);
			if (!args.Handled) {
				using (MailMergeExportRangeForm form = new MailMergeExportRangeForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = form.ShowDialog(this);
					if (result == DialogResult.OK)
						callback(form.Controller.Properties, data);
				}
			} else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.Properties, data);
			}
		}
		public SnapMailMergeExportOptions CreateSnapMailMergeExportOptions() {
			if (InnerControl != null) {
				NativeSnapMailMergeExportOptions result = new NativeSnapMailMergeExportOptions(DocumentModel, InnerControl);
				result.CopyFrom(DocumentModel.SnapMailMergeVisualOptions);
				return result;
			}
			return null;
		}
		public void SnapMailMerge(SnapDocument document) {
			if (InnerControl != null)
				SnapMailMerge(CreateSnapMailMergeExportOptions(), document);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument) {
			if (InnerControl != null) {
				Guard.ArgumentNotNull(targetDocument, "targetDocument");
				SnapMailMerge(options, ((SnapNativeDocument)targetDocument).DocumentModel);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(ISnapDocumentServer documentServer) {
			if (InnerControl != null)
				SnapMailMerge(CreateSnapMailMergeExportOptions(), documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(SnapMailMergeExportOptions options, ISnapDocumentServer targetDocumentServer) {
			if (InnerControl != null) {
				Guard.ArgumentNotNull(targetDocumentServer, "targetDocumentServer");
				SnapMailMerge(options, (SnapDocumentModel)targetDocumentServer.Model.DocumentModel);
			}
		}
		public void SnapMailMerge(string fileName, DocumentFormat format) {
			if (InnerControl != null)
				SnapMailMerge(CreateSnapMailMergeExportOptions(), fileName, format);
		}
		public void SnapMailMerge(Stream stream, DocumentFormat format) {
			if (InnerControl != null)
				SnapMailMerge(CreateSnapMailMergeExportOptions(), stream, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format) {
			if (InnerControl != null) {
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
					using (SnapDocumentModel resultDocumentModel = (SnapDocumentModel)DocumentModel.CreateNew()) {
						CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
						resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
						resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
						SnapMailMerge(options, resultDocumentModel);
						InnerRichEditDocumentServer.SaveDocumentCore(resultDocumentModel, stream, format, fileName);
						resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
					}
				}
			}
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format) {
			if (InnerControl != null) {
				using (SnapDocumentModel resultDocumentModel = (SnapDocumentModel)DocumentModel.CreateNew()) {
					CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
					resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
					resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
					SnapMailMerge(options, resultDocumentModel);
					InnerRichEditDocumentServer.SaveDocumentCore(resultDocumentModel, stream, format, String.Empty);
					resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
				}
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
				SnapMailMergeHelper mergeHelper = new SnapMailMergeHelper(DocumentModel, options);
				mergeHelper.ExecuteMailMerge(targetModel);
				result = !mergeHelper.IsCancellationRequested;
				targetModel.DocumentSaveOptions.CurrentFileName = String.Empty;
			} finally {
				targetModel.EndUpdate();
			}
			return result;
		}
		#endregion
		void ISnapControl.AddNewDataSource() {
			IMenuCommandService menuCommandService = GetService<IMenuCommandService>();
			if (menuCommandService != null)
				menuCommandService.GlobalInvoke(DataExplorerCommands.AddDataSource);
		}
		void ISnapControl.SaveTheme() {
			SaveThemeCore();
		}
		void ISnapControl.LoadTheme() {
			LoadThemeCore();
		}
		void ISnapControl.ShowGroupFieldsForm(object objectToChange, IServiceProvider serviceProvider) {
			EditorHelper.EditValue(objectToChange, TypeDescriptor.GetProperties(objectToChange)["GroupFields"], serviceProvider);
		}
		protected virtual void SaveThemeCore() {
			DocumentExportHelper exportHelper = new DocumentExportHelper(DocumentModel);
			SaveFileDialog saveDialog = exportHelper.CreateSaveFileDialog(CreateThemeFilter(), 0);
			if (exportHelper.ShowSaveFileDialog(saveDialog, this)) {
				using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
					DocumentModel.SaveCurrentTheme(stream, Path.GetFileNameWithoutExtension(saveDialog.FileName));
				}
			}
		}
		protected virtual void LoadThemeCore() {
			DocumentImportHelper importHelper = new DocumentImportHelper(DocumentModel);
			OpenFileDialog openDialog = importHelper.CreateOpenFileDialog(CreateThemeFilter());
			if (importHelper.ShowOpenFileDialog(openDialog, this)) {
				using (FileStream stream = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
					DocumentModel.LoadTheme(stream);
				}
			}
		}
		FileDialogFilterCollection CreateThemeFilter() {
			FileDialogFilterCollection filters = new FileDialogFilterCollection();
			filters.Add(new FileDialogFilter(SnapLocalizer.GetString(SnapStringId.FileFilterDescription_SnapThemeFiles), "snt"));
			return filters;
		}
		#region TableCellStyleFormShowing
		static readonly object onTableCellStyleFormShowing = new object();
#if !SL
	[DevExpressSnapLocalizedDescription("SnapControlTableCellStyleFormShowing")]
#endif
		public event TableCellStyleFormShowingEventHandler TableCellStyleFormShowing {
			add { Events.AddHandler(onTableCellStyleFormShowing, value); }
			remove { Events.RemoveHandler(onTableCellStyleFormShowing, value); }
		}
		protected internal virtual void RaiseTableCellStyleFormShowing(TableCellStyleFormShowingEventArgs e) {
			TableCellStyleFormShowingEventHandler handler = (TableCellStyleFormShowingEventHandler)Events[onTableCellStyleFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#endregion
		protected override DeleteTableCellsForm CreateDeleteTableCellsFrom(DeleteTableCellsFormControllerParameters controllerParameters) {
			return new SnapDeleteTableCellsForm(controllerParameters);
		}
		void ISnapControl.ShowTableCellStyleForm(TableCellStyle style) {
			this.ShowTableCellStyleForm(style);
		}
		internal virtual void ShowTableCellStyleForm(TableCellStyle style) {
			TableCellStyleFormControllerParameters controllerParameters;
			controllerParameters = new TableCellStyleFormControllerParameters(this, style);
			TableCellStyleFormShowingEventArgs args = new TableCellStyleFormShowingEventArgs((TableCellStyleFormControllerParameters)controllerParameters);
			RaiseTableCellStyleFormShowing(args);
			if (!args.Handled) {
				using (SnapTableStyleForm form = new SnapTableStyleForm(controllerParameters)) {
					form.ShowDialog(this);
				}
			}
		}
	}
}
