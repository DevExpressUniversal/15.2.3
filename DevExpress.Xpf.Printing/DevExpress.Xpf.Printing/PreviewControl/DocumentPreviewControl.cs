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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using DevExpress.Xpf.Printing.PreviewControl;
using System.Linq;
using System.Windows.Media;
using System.Collections.Specialized;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DocumentMapSettings = DevExpress.Xpf.Printing.PreviewControl.Native.DocumentMapSettings;
using DevExpress.XtraReports;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.Mvvm.UI;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Mvvm;
using System.IO;
using IDocument = DevExpress.Xpf.DocumentViewer.IDocument;
using IDocumentViewModel = DevExpress.Xpf.Printing.PreviewControl.Native.IDocumentViewModel;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Themes;
using DevExpress.XtraPrinting.Native;
using DevExpress.Xpf.Printing.Exports;
using System.ComponentModel;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Printing {
	[DXToolboxBrowsable(true)]
	public class DocumentPreviewControl : DocumentViewerControl {
		static readonly Type ownerType = typeof(DocumentPreviewControl);
		public static readonly DependencyProperty CursorModeProperty;
		public static readonly DependencyProperty HighlightSelectionColorProperty;
		internal static readonly DependencyPropertyKey HasSelectionPropertyKey;
		public static readonly DependencyProperty HasSelectionProperty;
		internal static readonly DependencyPropertyKey ParametersModelPropertyKey;
		public static readonly DependencyProperty ParametersModelProperty;
		public static readonly DependencyProperty AutoShowParametersPanelProperty;
		public static readonly DependencyProperty AutoShowDocumentMapProperty;
		public static readonly DependencyProperty DialogServiceTemplateProperty;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		public static readonly DependencyProperty SaveFileDialogTemplateProperty;
		public static readonly DependencyProperty PageDisplayModeProperty;
		public static readonly DependencyProperty ColumnsCountProperty;
		public static readonly DependencyProperty HiddenExportFormatsProperty;
		public static readonly DependencyProperty RequestDocumentCreationProperty;
		public static RoutedEvent CursorModeChangedEvent;
		public static RoutedEvent DocumentPreviewMouseClickEvent;
		public static RoutedEvent DocumentPreviewMouseDoubleClickEvent;
		public static RoutedEvent DocumentPreviewMouseMoveEvent;
		public static RoutedEvent SelectionStartedEvent;
		public static RoutedEvent SelectionContinuedEvent;
		public static RoutedEvent SelectionEndedEvent;
		public CursorModeType CursorMode {
			get { return (CursorModeType)GetValue(CursorModeProperty); }
			set { SetValue(CursorModeProperty, value); }
		}
		public Color HighlightSelectionColor {
			get { return (Color)GetValue(HighlightSelectionColorProperty); }
			set { SetValue(HighlightSelectionColorProperty, value); }
		}
		public bool HasSelection {
			get { return (bool)GetValue(HasSelectionProperty); }
			private set { SetValue(HasSelectionPropertyKey, value); }
		}
		public ParametersModel ParametersModel {
			get { return (ParametersModel)GetValue(ParametersModelProperty); }
			private set { SetValue(ParametersModelPropertyKey, value); }
		}
		public bool AutoShowParametersPanel {
			get { return (bool)GetValue(AutoShowParametersPanelProperty); }
			set { SetValue(AutoShowParametersPanelProperty, value); }
		}
		public bool AutoShowDocumentMap {
			get { return (bool)GetValue(AutoShowDocumentMapProperty); }
			set { SetValue(AutoShowDocumentMapProperty, value); }
		}
		public DataTemplate DialogServiceTemplate {
			get {
				return (DataTemplate)GetValue(DialogServiceTemplateProperty);
			}
			set {
				SetValue(DialogServiceTemplateProperty, value);
			}
		}
		public DataTemplate MessageBoxServiceTemplate {
			get {
				return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty);
			}
			set {
				SetValue(MessageBoxServiceTemplateProperty, value);
			}
		}
		public DataTemplate SaveFileDialogTemplate {
			get { return (DataTemplate)GetValue(SaveFileDialogTemplateProperty); }
			set { SetValue(SaveFileDialogTemplateProperty, value); }
		}
		public PageDisplayMode PageDisplayMode {
			get { return (PageDisplayMode)GetValue(PageDisplayModeProperty); }
			set { SetValue(PageDisplayModeProperty, value); }
		}
		public int ColumnsCount {
			get { return (int)GetValue(ColumnsCountProperty); }
			set { SetValue(ColumnsCountProperty, value); }
		}
		public ObservableCollection<ExportFormat> HiddenExportFormats {
			get { return (ObservableCollection<ExportFormat>)GetValue(HiddenExportFormatsProperty); }
			set { SetValue(HiddenExportFormatsProperty, value); }
		}
		public bool RequestDocumentCreation {
			get { return (bool)GetValue(RequestDocumentCreationProperty); }
			set { SetValue(RequestDocumentCreationProperty, value); }
		}
		public event RoutedEventHandler CursorModeChanged {
			add { AddHandler(CursorModeChangedEvent, value); }
			remove { RemoveHandler(CursorModeChangedEvent, value); }
		}
		public event DocumentPreviewMouseEventHandler DocumentPreviewMouseClick {
			add { AddHandler(DocumentPreviewMouseClickEvent, value); }
			remove { RemoveHandler(DocumentPreviewMouseClickEvent, value); }
		}
		public event DocumentPreviewMouseEventHandler DocumentPreviewMouseDoubleClick {
			add { AddHandler(DocumentPreviewMouseDoubleClickEvent, value); }
			remove { RemoveHandler(DocumentPreviewMouseDoubleClickEvent, value); }
		}
		public event DocumentPreviewMouseEventHandler DocumentPreviewMoveClick {
			add { AddHandler(DocumentPreviewMouseMoveEvent, value); }
			remove { RemoveHandler(DocumentPreviewMouseMoveEvent, value); }
		}
		public event RoutedEventHandler SelectionStarted {
			add { AddHandler(SelectionStartedEvent, value); }
			remove { RemoveHandler(SelectionStartedEvent, value); }
		}
		public event RoutedEventHandler SelectionContinued {
			add { AddHandler(SelectionContinuedEvent, value); }
			remove { RemoveHandler(SelectionContinuedEvent, value); }
		}
		public event RoutedEventHandler SelectionEnded {
			add { AddHandler(SelectionEndedEvent, value); }
			remove { RemoveHandler(SelectionEndedEvent, value); }
		}
		protected internal new DocumentPresenterControl DocumentPresenter { get { return base.DocumentPresenter as DocumentPresenterControl; } }
		public new IDocumentViewModel Document { get { return (IDocumentViewModel)base.Document; } }
		public new DocumentMapSettings ActualDocumentMapSettings { get { return base.ActualDocumentMapSettings as DocumentMapSettings; } protected set { base.ActualDocumentMapSettings = value; } }
		public new DocumentCommandProvider ActualCommandProvider { get { return base.ActualCommandProvider as DocumentCommandProvider; } }
		#region ctor & Initialize
		static DocumentPreviewControl() {
			CursorModeProperty = DependencyPropertyManager.Register("CursorMode", typeof(CursorModeType), ownerType,
				new FrameworkPropertyMetadata(CursorModeType.SelectTool, FrameworkPropertyMetadataOptions.None, (d, args) => ((DocumentPreviewControl)d).OnCursorModeChanged((CursorModeType)args.NewValue)));
			CursorModeChangedEvent = EventManager.RegisterRoutedEvent("CursorModeChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			HighlightSelectionColorProperty = DependencyPropertyManager.Register("HighlightSelectionColor", typeof(Color), ownerType, 
				new FrameworkPropertyMetadata(Color.FromArgb(89, 96, 152, 192), (d,e)=>((DocumentPreviewControl)d).OnSelectionColorChanged((Color)e.NewValue)));
			HasSelectionPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasSelection", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasSelectionProperty = HasSelectionPropertyKey.DependencyProperty;
			ParametersModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ParametersModel", typeof(ParametersModel), ownerType, new FrameworkPropertyMetadata(null));
			ParametersModelProperty = ParametersModelPropertyKey.DependencyProperty;
			AutoShowParametersPanelProperty = DependencyPropertyManager.Register("AutoShowParametersPanel", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AutoShowDocumentMapProperty = DependencyPropertyManager.Register("AutoShowDocumentMap", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			DialogServiceTemplateProperty = DependencyPropertyRegistrator.Register<DocumentPreviewControl, DataTemplate>(owner => owner.DialogServiceTemplate, null);
			MessageBoxServiceTemplateProperty = DependencyPropertyRegistrator.Register<DocumentPreviewControl, DataTemplate>(owner => owner.MessageBoxServiceTemplate, null);
			SaveFileDialogTemplateProperty = DependencyPropertyRegistrator.Register<DocumentPreviewControl, DataTemplate>(owner => owner.SaveFileDialogTemplate, null);
			PageDisplayModeProperty = DependencyPropertyManager.Register("PageDisplayMode", typeof(PageDisplayMode), ownerType,
				new FrameworkPropertyMetadata(PageDisplayMode.Single, (d, e) => ((DocumentPreviewControl)d).AssignDocumentPresenterProperties()));
			ColumnsCountProperty = DependencyPropertyManager.Register("ColumnsCount", typeof(int), ownerType, 
				new FrameworkPropertyMetadata(1, (d, e) => ((DocumentPreviewControl)d).AssignDocumentPresenterProperties(), (d, value) => { return ((int)value) > 1 ? value : 1; }));
			HiddenExportFormatsProperty = DependencyPropertyRegistrator.Register<DocumentPreviewControl, ObservableCollection<ExportFormat>>(owner => owner.HiddenExportFormats,
				new ObservableCollection<ExportFormat>(), (d, o, n) => d.OnHiddenFormatsChanged(o, n), (d, n) => d.CoerceHiddenFormats(n));
			RequestDocumentCreationProperty = DependencyPropertyRegistrator.Register<DocumentPreviewControl, bool>(owner => owner.RequestDocumentCreation, false);
			DocumentPreviewMouseClickEvent = EventManager.RegisterRoutedEvent("DocumentPreviewMouseClick", RoutingStrategy.Direct, typeof(DocumentPreviewMouseEventHandler), ownerType);
			DocumentPreviewMouseDoubleClickEvent = EventManager.RegisterRoutedEvent("DocumentPreviewMouseDoubleClick", RoutingStrategy.Direct, typeof(DocumentPreviewMouseEventHandler), ownerType);
			DocumentPreviewMouseMoveEvent = EventManager.RegisterRoutedEvent("DocumentPreviewMouseMoveClick", RoutingStrategy.Direct, typeof(DocumentPreviewMouseEventHandler), ownerType);
			SelectionStartedEvent = EventManager.RegisterRoutedEvent("SelectionStarted", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			SelectionContinuedEvent = EventManager.RegisterRoutedEvent("SelectionContinued", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			SelectionEndedEvent = EventManager.RegisterRoutedEvent("SelectionEnded", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			BrickResolver.EnsureStaticConstructor();
		}
		public DocumentPreviewControl() {
			DefaultStyleKey = typeof(DocumentPreviewControl);
			CreateParametersModel();
			base.ActualDocumentMapSettings = CreateDefaultDocumentMapSettings();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		protected override Xpf.DocumentViewer.DocumentMapSettings CreateDefaultDocumentMapSettings() {
			return new DocumentMapSettings();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			var document = Document as DocumentViewModel;
			Unsubscribe(document);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			var document = Document as DocumentViewModel;
			Subscribe(document);
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			DocumentPresenter.Do(x => x.Focus());
		}
		#endregion
		#region commands
		public ICommand SetCursorModeCommand { get; private set; }
		public ICommand CopyCommand { get; private set; }
		public ICommand ToggleParametersPanelCommand { get; private set; }
		public ICommand ToggleDocumentMapCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
		public ICommand PrintCommand { get; private set; }
		public ICommand PrintDirectCommand { get; private set; }
		public ICommand PageSetupCommand { get; private set; }
		public ICommand ScaleCommand { get; private set; }
		public ICommand FirstPageCommand { get; private set; }
		public ICommand LastPageCommand { get; private set; }
		public ICommand ExportCommand { get; private set; }
		public ICommand SendCommand { get; private set; }
		public ICommand SetWatermarkCommand { get; private set; }
		public ICommand StopPageBuildingCommand { get; private set; }
		protected override void InitializeCommands() {
			base.InitializeCommands();
			SetCursorModeCommand = DelegateCommandFactory.Create<CursorModeType>(SetCursorMode, CanSetCursorMode);
			CopyCommand = DelegateCommandFactory.Create(Copy, CanCopy);
			ToggleParametersPanelCommand = DelegateCommandFactory.Create(ToggleParametersPanel, CanToggleParametersPanel);
			ToggleDocumentMapCommand = DelegateCommandFactory.Create(ToggleDocumentMap, CanToggleDocumentMap);
			SaveCommand = DelegateCommandFactory.Create(Save, CanSave);
			PrintCommand = DelegateCommandFactory.Create(Print, CanPrint);
			PrintDirectCommand = DelegateCommandFactory.Create<string>(PrintDirect, x => CanPrintDirect());
			PageSetupCommand = DelegateCommandFactory.Create(PageSetup, CanPageSetup);
			ScaleCommand = DelegateCommandFactory.Create(Scale, CanScale);
			FirstPageCommand = DelegateCommandFactory.Create(GoToFirstPage, CanGoToFirstPage);
			LastPageCommand = DelegateCommandFactory.Create(GoToLastPage, CanGoToLastPage);
			ExportCommand = DelegateCommandFactory.Create<ExportFormat?>(Export, CanExport);
			SendCommand = DelegateCommandFactory.Create<ExportFormat?>(Send, CanSend);
			SetWatermarkCommand = DelegateCommandFactory.Create(SetWatermark, CanSetWatermark);
			StopPageBuildingCommand = DelegateCommandFactory.Create(StopBuilding, CanStopBuilding);
		}
		protected override string GetOpenFileFilter() {
			return String.Format("{0} (*{1})|*{1}", PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterNativeFormat), NativeFormatOptionsController.NativeFormatExtension);
		}
		protected virtual void Save() {
			if (!CanSave())
				return;
			try {
				SaveInternal();
			} catch {
			}
		}
		void SaveInternal() {
			AssignableServiceHelper2<DocumentPresenterControl, SaveFileDialogService>.DoServiceAction(this, SaveFileDialogTemplate.Return(TemplateHelper.LoadFromTemplate<SaveFileDialogService>, CreateDefaultSaveFileDialogService), service => {
				service.InitialDirectory = Document.InitialDirectory;
				service.DefaultFileName = Document.DeafultFileName;
				var saveDialog = service as ISaveFileDialogService;
				if (!saveDialog.Return(x => x.ShowDialog(), () => false))
					return;
				var file = saveDialog.File;
				string filePath = Path.Combine(file.DirectoryName, file.Name);
				Document.Save(filePath);
			});
		}
		protected virtual bool CanSave() {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual void Print() {
			if (!CanPrint())
				return;
			var printModel = DocumentViewModel.GetPrintOptionsViewModel((DocumentViewModel)Document);
			PrintInternal(printModel);
		}
		void PrintInternal(PrintOptionsViewModel printOptions) {
			var executeCommand = DelegateCommandFactory.Create<PrintOptionsViewModel>(
					(model) => Document.Print(model), (model) => model.IsValid);
			AssignableServiceHelper2<DocumentPreviewControl, Core.DialogService>.DoServiceAction(this, DialogServiceTemplate.Return(TemplateHelper.LoadFromTemplate<Core.DialogService>, CreateDefaultDialogService), service => {
				service.ViewTemplate = (DataTemplate)FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.PrintDialogTemplate });
				service.ShowDialog(executeCommand, null, PrintingLocalizer.GetString(PrintingStringId.Print), printOptions, false);
			});
		}
		protected virtual bool CanPrint() {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual void PrintDirect(string printerName = null) {
			Document.Do(x => x.PrintDirect(printerName));
		}
		protected virtual bool CanPrintDirect() {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual void PageSetup() {
			if(Document == null)
				return;
			new PageSettingsConfiguratorService().Configure(Document.PageSettings, Window.GetWindow(this));
		}
		protected virtual bool CanPageSetup() {
			return DocumentSource.Return(x => x is ILink, () => false)
				&& Document.Return(x => x.IsCreated && x.CanChangePageSettings, () => false);
		}
		protected virtual void Scale() {
			if(!CanScale())
				return;
			var scaleModel = DocumentViewModel.GetScaleViewModel((DocumentViewModel)Document);
			ScaleInternal(scaleModel);
		}
		void ScaleInternal(ScaleOptionsViewModel scaleModel) {
			var executeCommand = DelegateCommandFactory.Create<ScaleOptionsViewModel>(
					(model) => Document.Scale(model),
					(model) => model.CanApply);
			AssignableServiceHelper2<DocumentPreviewControl, Core.DialogService>.DoServiceAction(this, DialogServiceTemplate.Return(TemplateHelper.LoadFromTemplate<Core.DialogService>, CreateDefaultDialogService), service => {
				service.ViewTemplate = (DataTemplate)FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.ScaleDialogTemplate });
				service.ShowDialog(
					executeCommand,
					null,
					PrintingLocalizer.GetString(PrintingStringId.Scaling),
					scaleModel);
			});
		}
		protected virtual bool CanScale() {
			return DocumentSource.Return(x => x is ILink, () => false)
				&& Document.Return(x => x.IsCreated && x.CanChangePageSettings
					&& !x.IsXpfLinkDocumentSource()
					&& !x.IsRemoteReportDocumentSorce(),
					() => false);
		}
		protected virtual void GoToFirstPage() {
			CurrentPageNumber = 1;
		}
		protected virtual bool CanGoToFirstPage() {
			return Document.Return(x => PageCount > 0 && CurrentPageNumber > 1, () => false);
		}
		protected virtual bool CanGoToLastPage() {
			return Document.Return(x => x.IsCreated && PageCount != 0 && CurrentPageNumber < this.PageCount, () => false);
		}
		protected virtual void GoToLastPage() {
			CurrentPageNumber = PageCount;
		}
		protected virtual bool CanExport(ExportFormat? format) {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual bool CanSend(ExportFormat? arg) {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual void Export(ExportFormat? format) {
			try {
				if(Document == null)
					throw new InvalidOperationException();
				var exportFormat = format.HasValue ? format.Value : Document.DefaultExportFormat;
				var exportOptionsViewModel = DocumentViewModel.GetExportViewModel((DocumentViewModel)Document, exportFormat, HiddenExportFormats);
				if (HiddenExportFormats.Contains(exportFormat))
					exportOptionsViewModel.ExportFormat = exportOptionsViewModel.AvailableExportFormats.First();
				var executeCommand = DelegateCommandFactory.Create<ExportOptionsViewModel>(
					(model) => Document.Export(model),
					(model) => !string.IsNullOrEmpty(model.FileName));
				ExportInternal(exportOptionsViewModel, executeCommand);
			} finally { }
		}
		protected virtual void Send(ExportFormat? format) {
			try {
				if(Document == null)
					throw new InvalidOperationException();
				var exportFormat = format.HasValue ? format.Value : Document.DefaultSendFormat;
				var sendOptionsViewModel = DocumentViewModel.GetSendViewModel((DocumentViewModel)Document, exportFormat, HiddenExportFormats);
				if (HiddenExportFormats.Contains(exportFormat))
					sendOptionsViewModel.ExportFormat = sendOptionsViewModel.AvailableExportFormats.First();
				var executeCommand = DelegateCommandFactory.Create<SendOptionsViewModel>(
					(model) => Document.Send(model),
					(model) => !string.IsNullOrEmpty(model.FileName));
				ExportInternal(sendOptionsViewModel, executeCommand);
			} finally {
			}
		}
		void ExportInternal(ExportOptionsViewModelBase exportOptionsViewModel, ICommand executeCommand) {
			if (exportOptionsViewModel.ShowOptionsBeforeExport) {
				AssignableServiceHelper2<DocumentPreviewControl, Core.DialogService>.DoServiceAction(this, DialogServiceTemplate.Return(TemplateHelper.LoadFromTemplate<Core.DialogService>, CreateDefaultDialogService), service => {
					service.ViewTemplate = exportOptionsViewModel.SettingsType == SettingsType.Send
					? (DataTemplate)FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.SendOptionsDialogTemplate })
					: (DataTemplate)FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.ExportOptionsDialogTemplate });
					service.ShowDialog(
							executeCommand,
							null,
							exportOptionsViewModel.SettingsType == SettingsType.Send ? PreviewLocalizer.GetString(PreviewStringId.TB_TTip_Send) : PreviewLocalizer.GetString(PreviewStringId.TB_TTip_Export),
							exportOptionsViewModel);
				});
			} else {
				executeCommand.Execute(null);
			}
		}
		protected virtual bool CanToggleParametersPanel() {
			return ParametersModel.HasVisibleParameters;
		}
		protected virtual void ToggleParametersPanel() { }
		protected virtual void ToggleDocumentMap() { }
		protected virtual bool CanToggleDocumentMap() {
			return Document.Return(x => x.HasBookmarks, () => false);
		}
		protected virtual bool CanSetCursorMode(CursorModeType cursorMode) {
			return Document.Return(x => x.IsCreated, () => false);
		}
		protected virtual void SetCursorMode(CursorModeType cursorMode) {
			CursorMode = cursorMode;
		}
		protected virtual bool CanCopy() {
			return HasSelection;
		}
		protected virtual void Copy() {
			DocumentPresenter.SelectionService.CopyToClipboard();
		}
		protected override bool CanClockwiseRotate() {
			return false;
		}
		protected override void ClockwiseRotate() {
			throw new NotSupportedException();
		}
		PrintingSystemBase PrintingSystem {
			get { return ((DocumentViewModel)Document).PrintingSystem; }
		}
		protected override void ExecuteNavigate(object parameter) {
			ShowFindTextCommand.Execute(false);
			var target = parameter as PreviewBookmarkNode;
			if (target.BookmarkNode.Pair.GetPage(PrintingSystem.Pages) != null) {
				DocumentPresenter.SelectionService.OnKillFocus();
				DocumentPresenter.NavigationStrategy.ShowBrick(target.BookmarkNode.Pair);
			}
		}
		protected override void FindNextText(TextSearchParameter parameter) {
			DocumentPresenter.SelectionService.ResetSelectedBricks();
			var result = Document.PerformSearch(parameter);
			if (result != null) {
				DocumentPresenter.SelectionService.SelectBrick(result.GetPage(PrintingSystem.Pages), result.GetBrick(PrintingSystem.Pages));
				DocumentPresenter.NavigationStrategy.ShowBrick(result);
				DocumentPresenter.Update();
			} else {
				AssignableServiceHelper2<DocumentPreviewControl, DXMessageBoxService>.DoServiceAction(this, MessageBoxServiceTemplate.Return(TemplateHelper.LoadFromTemplate<DXMessageBoxService>, CreateDefaultMessageBoxService), service => {
					service.ShowMessage("Finished searching throughout the document. No matches were found.", PreviewStringId.Msg_Caption.GetString(), MessageButton.OK, MessageIcon.Information);
				});
			}
		}
		protected override void ShowFindText(bool? show) {
			base.ShowFindText(show);
			if (show.HasValue && show.Value)
				DocumentPresenter.SelectionService.ResetSelectedBricks();
		}
		protected override bool CanZoomIn() {
			return Document.Return(x => x.IsLoaded && base.CanZoomIn(), () => false);
		}
		protected override bool CanZoomOut() {
			return Document.Return(x => x.IsLoaded && base.CanZoomOut(), () => false);
		}
		protected override bool CanSetZoomFactor(double zoomFactor) {
			return  Document.Return(x => x.IsLoaded && base.CanSetZoomFactor(zoomFactor), ()=> false);
		}
		protected override bool CanSetZoomMode(ZoomMode zoomMode) {
			return base.CanSetZoomMode(zoomMode);
		}
		protected virtual void SetWatermark() {
			var watermarkService = new WatermarkService();
			watermarkService.EditCompleted += (s, e) => {
				if(e.IsWatermarkAssigned == true) {
					Document.Do(x => x.SetWatermark(e.Watermark));
					DocumentPresenter.Update();
				}
			};
			watermarkService.Edit(Window.GetWindow(this), Document.Pages[CurrentPageNumber - 1].Page, PageCount, Document.Watermark);
		}
		protected virtual bool CanSetWatermark() {
			return Document.Return(x => x.IsCreated, () => false);
		}
		public override void OpenDocument(string filePath = null) {
			string path;
			if(!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
				path = filePath;
			} else {
				var openFileDialogService = OpenFileDialogTemplate.Return(TemplateHelper.LoadFromTemplate<OpenFileDialogService>, CreateDefaultOpenFileDialogService);
				IFileInfo fileInfo = null;
				AssignableServiceHelper2<DocumentViewerControl, OpenFileDialogService>.DoServiceAction(this, openFileDialogService, service => {
					IOpenFileDialogService openFileService = service;
					fileInfo = openFileService.ShowDialog() ? openFileService.Files.FirstOrDefault() : null;
				});
				if (fileInfo == null) {
					return;
				}
				path = Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
			}
			DocumentSource = path;
		}
		public virtual void StopBuilding() {
			if(CanStopBuilding())
				Document.StopPageBuilding();
		}
		public virtual bool CanStopBuilding() {
			return Document.Return(x => x.IsCreating, () => false);
		}
		#endregion
		protected override void OnCommandProviderChanged(CommandProvider oldValue, CommandProvider newValue) {
			if(!(newValue is DocumentCommandProvider))
				CommandBarStyle = CommandBarStyle.None;
		}
		protected override IDocument CreateDocument(object source) {
			DocumentViewModel model;
			if(source is IReport)
				model = new ReportDocumentViewModel();
			else
				model = new DocumentViewModel();
			model.Do(x => Subscribe(x));
			return model;
		}
		protected sealed override void LoadDocument(object source) {
			base.LoadDocument(source);
			((DocumentViewModel)Document).Load(source);
			ActualDocumentMapSettings = new DocumentMapSettings();
			ActualCommandProvider.Do(x=> x.UpdateCommands());
			if(!DesignerProperties.GetIsInDesignMode(this) && RequestDocumentCreation)
				Document.Do(x => x.CreateDocument());
		}
		protected override void ReleaseDocument(IDocument document) {
			base.ReleaseDocument(document);
			(document as DocumentViewModel).Do(x => {
				Unsubscribe(x);
			});
		}
		protected override void OnDocumentChanged(IDocument oldValue, IDocument newValue) {
			ParametersModel.AssignParameters(null);
			ActualCommandProvider.Do(x => x.UpdateCommands());
		}
		protected override void OnZoomFactorChanged(double oldValue, double newValue) {
			base.OnZoomFactorChanged(oldValue, newValue);
			DocumentPresenter.Do(x => x.Update());
		}
		protected virtual SaveFileDialogService CreateDefaultSaveFileDialogService() {
			return new SaveFileDialogService() {
				DefaultExt = NativeFormatOptionsController.NativeFormatExtension,
				Filter = GetOpenFileFilter()
			};
		}
		protected virtual Core.DialogService CreateDefaultDialogService() {
			return new Core.DialogService() {
				DialogStyle = (Style)FindResource(new NewDocumentViewerThemeKeyExtension() { ResourceKey = NewDocumentViewerThemeKeys.ExportOptionsDialogStyle }),
				DialogWindowStartupLocation = WindowStartupLocation.CenterScreen,
			};
		}
		protected virtual DXMessageBoxService CreateDefaultMessageBoxService() {
			return new DXMessageBoxService();
		}
		void Subscribe(DocumentViewModel documentViewModel) {
			Unsubscribe(documentViewModel);
			documentViewModel.Do(x => {
				x.Pages.CollectionChanged += OnPagesCollectionChanged;
				x.StartDocumentCreation += OnStartDocumentCreation;
				x.DocumentCreated += OnDocumentCreated;
				x.DocumentException += OnDocumentException;
				(x as ReportDocumentViewModel).Do(report => report.ReportParametersRecieved += OnReportParametersRecieved);
				x.Subscribe();
			});
		}
		void Unsubscribe(DocumentViewModel documentViewModel) {
			documentViewModel.Do(x => {
				x.Unsubscribe();
				x.Pages.CollectionChanged -= OnPagesCollectionChanged;
				x.StartDocumentCreation += OnStartDocumentCreation;
				x.DocumentCreated -= OnDocumentCreated;
				x.DocumentException -= OnDocumentException;
				(x as ReportDocumentViewModel).Do(report => report.ReportParametersRecieved -= OnReportParametersRecieved);
			});
		}
		void OnPagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			PageCount = Document.Pages.Count();
		}
		void OnReportParametersRecieved(object sender, ReportParametersRecievedEventArgs e) {
			ParametersModel.AssignParameters(e.Parameters);
			ParametersModel.LookUpValuesProvider = e.LookUpValuesProvider;
			ActualCommandProvider.Do(x => x.UpdateCommands());
		}
		void OnStartDocumentCreation(object sender, EventArgs e) {
			DocumentPresenter.Do(x => x.NavigationStrategy.ScrollToStartUp());
		}
		void OnDocumentCreated(object sender, EventArgs e) {
			ActualDocumentMapSettings = new DocumentMapSettings();
			ActualCommandProvider.Do(x=> x.UpdateCommands());
			DocumentPresenter.Do(x => {
				x.ItemsPanel.Do(panel=> panel.UpdateLayout());
				x.Update();
			});
		}
		protected virtual void OnDocumentException(object sender, DevExpress.XtraPrinting.ExceptionEventArgs e) {
			if (IsLoaded)
				AssignableServiceHelper2<DocumentPreviewControl, DXMessageBoxService>.DoServiceAction(this, MessageBoxServiceTemplate.Return(TemplateHelper.LoadFromTemplate<DXMessageBoxService>, CreateDefaultMessageBoxService), service => {
					service.Do(x => x.ShowMessage(e.Exception.Message, PrintingLocalizer.GetString(PrintingStringId.Error), MessageButton.OK, MessageIcon.Error));
				});
		}
		protected void OnCursorModeChanged(CursorModeType cursorModeType) {
			RaiseEvent(new RoutedEventArgs(CursorModeChangedEvent));
		}
		protected void OnSelectionColorChanged(Color color) {
			DocumentPresenter.Do(x=> x.HighlightSelectionColor = color);
		}
		ObservableCollection<ExportFormat> CoerceHiddenFormats(ObservableCollection<ExportFormat> hiddenFormats) {
			if (hiddenFormats == null)
				return new ObservableCollection<ExportFormat>();
			return new ObservableCollection<ExportFormat>(hiddenFormats.Distinct());
		}
		void OnHiddenFormatsChanged(ObservableCollection<ExportFormat> oldValue, ObservableCollection<ExportFormat> newValue) {
			oldValue.CollectionChanged -= OnHiddenFormatsCollectionChanged;
			newValue.CollectionChanged += OnHiddenFormatsCollectionChanged;
			ActualCommandProvider.Do(x => x.UpdateCommands());
		}
		void OnHiddenFormatsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ActualCommandProvider.Do(x => x.UpdateCommands());
		}
		protected override void AssignDocumentPresenterProperties() {
			base.AssignDocumentPresenterProperties();
			DocumentPresenter.Do(x => x.HighlightSelectionColor = HighlightSelectionColor);
			DocumentPresenter.Do(x => x.PageDisplayMode = PageDisplayMode);
			DocumentPresenter.Do(x => x.ColumnsCount = ColumnsCount);
		}
		void CreateParametersModel() {
			ParametersModel = ParametersModel.CreateParametersModel();
			ParametersModel.Submit += OnSubmitParameters;
		}
		void OnSubmitParameters(object sender, EventArgs e) {
			(Document as ReportDocumentViewModel).Do(x=> x.Submit(ParametersModel.Parameters.Select(p=> p.Parameter).ToList()));
		}
		protected override CommandProvider CreateCommandProvider() {
			return new DocumentCommandProvider();
		}
		protected internal void RaiseDocumentPreiewMouseClick(DocumentPreviewMouseEventArgs e) {
			e.RoutedEvent = DocumentPreviewMouseClickEvent;
			RaiseEvent(e);
		}
		protected internal void RaiseDocumentPreiewMouseDoubleClick(DocumentPreviewMouseEventArgs e) {
			e.RoutedEvent = DocumentPreviewMouseDoubleClickEvent;
			RaiseEvent(e);
		}
		protected internal void RaiseDocumentPreiewMouseMove(DocumentPreviewMouseEventArgs e) {
			e.RoutedEvent = DocumentPreviewMouseMoveEvent;
			RaiseEvent(e);
		}
		protected internal void RaiseSelectionStarted() {
			RaiseEvent(new RoutedEventArgs(SelectionStartedEvent));
		}
		protected internal void RaiseSelectionContinued() {
			RaiseEvent(new RoutedEventArgs(SelectionContinuedEvent));
		}
		protected internal void RaiseSelectionEnded() {
			RaiseEvent(new RoutedEventArgs(SelectionEndedEvent));
		}
		internal void UpdateSelection(bool hasSelection) {
			HasSelection = hasSelection;
		}
	}
	public enum CursorModeType {
		HandTool = 0,
		SelectTool = 1,
	}
}
