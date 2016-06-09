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
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Printing.Exports;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Platform.Wpf.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.Xpf.Printing {
	public abstract class PrintingSystemPreviewModel : DocumentPreviewModelBase, IDisposable {
		#region Fields
		internal const int DefaultPageCacheSize = 20;
		IPageSettingsConfiguratorService pageSettingsConfiguratorService;
		IPrintService printService;
		IExportSendService exportSendService;
		IHighlightingService highlightingService;
		PagesCache pagesCache;
		bool isCreating;
		bool isExporting = false;
		bool disposed;
		bool firstPageDisplayed;
		bool isReflectorPositionChanged = false;
#if !SL
		bool isSaving;
#endif
		#endregion
		#region Constructors
		public PrintingSystemPreviewModel()
			: this(new PageSettingsConfiguratorService(),
				   new PrintService(),
				   new ExportSendService(),
				   new HighlightingService()) {
		}
		internal PrintingSystemPreviewModel(IPageSettingsConfiguratorService pageSettingsConfiguratorService,
											IPrintService printService,
											IExportSendService exportSendService,
											IHighlightingService highlightingService) {
			this.pageSettingsConfiguratorService = pageSettingsConfiguratorService;
			this.printService = printService;
			this.exportSendService = exportSendService;
			this.highlightingService = highlightingService;
			pagesCache = new PagesCache(DefaultPageCacheSize);
		}
		#endregion
		#region Properties
#if DEBUGTEST
		internal PagesCache TEST_GetPagesCache() {
			return pagesCache;
		}
#endif
		public int PageCacheSize {
			get {
				return pagesCache.CacheSize;
			}
			set {
				if(value != pagesCache.CacheSize) {
					pagesCache = new PagesCache(value);
				}
			}
		}
		protected internal abstract PrintingSystemBase PrintingSystem { get; }
		protected internal bool IsSaving {
			get { return isSaving; }
			set {
				if(isSaving == value)
					return;
				isSaving = value;
				RaiseAllCommandsCanExecuteChanged();
			}
		}
		protected bool IsExporting {
			get {
				return isExporting;
			}
			set {
				isExporting = value;
				RaiseAllCommandsCanExecuteChanged();
				RaisePropertyChanged(() => ProgressVisibility);
			}
		}
		public override bool IsCreating {
			get {
				return isCreating;
			}
			protected set {
				isCreating = value;
				RaisePropertyChanged(() => ProgressMarqueeVisibility);
				RaisePropertyChanged(() => IsCreating);
				RaisePropertyChanged(() => ProgressVisibility);
				RaisePropertyChanged(() => ProgressValue);
				RaisePropertyChanged(() => IsEmptyDocument);
				if(isCreating)
					IsDocumentMapToggledDuringDocumentCreating = true;
				ToggleDocumentMap();
				if(!isCreating)
					IsDocumentMapToggledDuringDocumentCreating = false;
				RaiseAllCommandsCanExecuteChanged();
			}
		}
		protected bool BuildPagesComplete {
			get {
				return PrintingSystem != null && !IsCreating && PrintingSystem.Document.Pages.Count != 0;
			}
		}
		protected bool IsReflectorPositionChanged {
			get {
				return isReflectorPositionChanged;
			}
			set {
				isReflectorPositionChanged = value;
				RaisePropertyChanged(() => ProgressValue);
				RaisePropertyChanged(() => ProgressMaximum);
			}
		}
		internal IExportSendService ExportSendService { get { return exportSendService; } set { exportSendService = value; } }
		#endregion
		#region Methods
		protected abstract void CreateDocument(bool buildPagesInBackground);
#if SL
		public void ShowPageSetupDialog() {
			pageSettingsConfiguratorService.Configure(PrintingSystem.PageSettings);
		}
#else
		public void PrintDirect() {
			CreateDocumentIfEmpty(false);
			printService.PrintDirect(CreatePaginator(), PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()), PrintingSystem.Document.Name, true);
		}
		public void PrintDirect(string printerName) {
			CreateDocumentIfEmpty(false);
			printService.PrintDirect(CreatePaginator(), PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()), PrintingSystem.Document.Name, printerName, true);
		}
		public void PrintDirect(PrintQueue printQueue) {
			CreateDocumentIfEmpty(false);
			printService.PrintDirect(CreatePaginator(), PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()), PrintingSystem.Document.Name, printQueue, true);
		}
		public bool? PrintDialog() {
			CreateDocumentIfEmpty(false);
			return printService.PrintDialog(CreatePaginator(), PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()), PrintingSystem.Document.Name, true);
		}
		public bool? ShowPageSetupDialog(Window ownerWindow) {
			bool? dialogResult = pageSettingsConfiguratorService.Configure(PrintingSystem.PageSettings, ownerWindow);
			if(dialogResult == true) {
				ClearCache();
			}
			return dialogResult;
		}
#endif
		void CreateDocumentIfEmpty(bool buildPagesInBackground) {
			if(PrintingSystem.Document.PageCount == 0) {
				CreateDocument(buildPagesInBackground);
			}
		}
		void ProgressReflector_PositionChanged(object sender, EventArgs e) {
			ProgressReflector progressReflector = (ProgressReflector)sender;
			IsReflectorPositionChanged = true;
#if !SL
			if(progressReflector.PositionCore == progressReflector.MaximumCore && IsSaving)
				IsSaving = false;
#endif
#if !SL     // TODO: review
			if(PrintingSystem.Document.State == DocumentState.Created) {
				TimeSpan timeout = new TimeSpan(100000);
				Dispatcher.CurrentDispatcher.Invoke(new ThreadStart(delegate { }), timeout, DispatcherPriority.Background);
			}
#endif
		}
		protected override void OnCurrentPageIndexChanged() {
			UpdateCurrentPageContent();
		}
		void PrintingSystemBase_BeforeBuildPages(object sender, EventArgs e) {
			ClearCache();
			IsCreating = true;
		}
		void PrintingSystem_DocumentChanged(object sender, EventArgs e) {
			if(PageCount > 0 && !firstPageDisplayed) {
				firstPageDisplayed = true;
				CurrentPageIndex = 0;
				UpdateCurrentPageContent();
			}
			RaisePropertyChanged(() => PageCount);
			RaiseNavigationCommandsCanExecuteChanged();
		}
		void PrintingSystem_AfterBuildPages(object sender, EventArgs e) {
			IsCreating = false;
			if(PageCount != 0) {
				pagesCache.Clear();
				UpdateCurrentPageContent();
			}
		}
		protected void ToggleDocumentMap() {
			RaisePropertyChanged(() => DocumentMapRootNode);
			commands[PrintingSystemCommand.DocumentMap].RaiseCanExecuteChanged();
			IsDocumentMapVisible = CanShowDocumentMap(null);
		}
		protected void UpdateCurrentPageContent() {
			RaisePropertyChanged(() => PageContent);
			RaisePropertyChanged(() => PageViewWidth);
			RaisePropertyChanged(() => PageViewHeight);
		}
		protected void OnSourceChanging() {
			if(PrintingSystem != null) {
				UnhookPrintingSystem();
			}
			ClearCache();
		}
		protected void OnSourceChanged() {
			if(PrintingSystem == null)
				return;
			HookPrintingSystem();
			if(PageCount > 0) {
				CurrentPageIndex = 0;
			}
			if(!PrintingSystem.ExportOptions.Options.ContainsKey(typeof(XpsExportOptions)))
				PrintingSystem.ExportOptions.Options.Add(typeof(XpsExportOptions), new XpsExportOptions());
			RaiseAllPropertiesChanged();
			RaiseAllCommandsCanExecuteChanged();
		}
		protected virtual void HookPrintingSystem() {
			PrintingSystem.DocumentChanged += PrintingSystem_DocumentChanged;
			PrintingSystem.BeforeBuildPages += PrintingSystemBase_BeforeBuildPages;
			PrintingSystem.AfterBuildPages += PrintingSystem_AfterBuildPages;
			PrintingSystem.ProgressReflector.PositionChanged += ProgressReflector_PositionChanged;
#if !SL
			PrintingSystem.ReplaceService<XpsExportServiceBase>(new XpsExportService(CreatePaginator()));
			BackgroundServiceHelper.ReplaceBackgroundService(PrintingSystem);
			var cancellationService = PrintingSystem.GetService<ICancellationService>();
			if(cancellationService != null) {
				cancellationService.StateChanged += cancellationService_StateChanged;
			}
#endif
		}
		protected virtual void UnhookPrintingSystem() {
			PrintingSystem.DocumentChanged -= PrintingSystem_DocumentChanged;
			PrintingSystem.BeforeBuildPages -= PrintingSystemBase_BeforeBuildPages;
			PrintingSystem.AfterBuildPages -= PrintingSystem_AfterBuildPages;
			PrintingSystem.ProgressReflector.PositionChanged -= ProgressReflector_PositionChanged;
#if !SL
			PrintingSystem.RemoveService(typeof(IBackgroundService));
			var cancellationService = PrintingSystem.GetService<ICancellationService>();
			if(cancellationService != null) {
				cancellationService.StateChanged -= cancellationService_StateChanged;
			}
#endif
		}
#if !SL
		void cancellationService_StateChanged(object sender, EventArgs e) {
			var cancellationService = (ICancellationService)sender;
			IsCreating = cancellationService.CanBeCanceled() || PrintingSystem.Document.IsCreating;
		}
		protected DocumentPaginator CreatePaginator() {
			DocumentPaginator paginator = new DelegatePaginator(VisualizePage, () => PageCount);
			return paginator;
		}
#endif
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					if(PrintingSystem != null) {
						UnhookPrintingSystem();
					}
				}
				disposed = true;
			}
		}
#if !SL
		ExportOptionsBase GetExportOptions(ExportFormat format) {
			switch(format) {
				case ExportFormat.Pdf: return PrintingSystem.ExportOptions.Options[typeof(PdfExportOptions)];
				case ExportFormat.Htm: return PrintingSystem.ExportOptions.Options[typeof(HtmlExportOptions)];
				case ExportFormat.Mht: return PrintingSystem.ExportOptions.Options[typeof(MhtExportOptions)];
				case ExportFormat.Rtf: return PrintingSystem.ExportOptions.Options[typeof(RtfExportOptions)];
				case ExportFormat.Xls: return PrintingSystem.ExportOptions.Options[typeof(XlsExportOptions)];
				case ExportFormat.Xlsx: return PrintingSystem.ExportOptions.Options[typeof(XlsxExportOptions)];
				case ExportFormat.Csv: return PrintingSystem.ExportOptions.Options[typeof(CsvExportOptions)];
				case ExportFormat.Txt: return PrintingSystem.ExportOptions.Options[typeof(TextExportOptions)];
				case ExportFormat.Image: return PrintingSystem.ExportOptions.Options[typeof(ImageExportOptions)];
				case ExportFormat.Xps: return PrintingSystem.ExportOptions.Options[typeof(XpsExportOptions)];
				default: throw new ArgumentException("format");
			}
		}
#endif
		private bool IsProgressReflectorComplited() {
			return PrintingSystem.ProgressReflector.PositionCore == PrintingSystem.ProgressReflector.MaximumCore;
		}
		private bool IsProgressMarquee() {
			if(PrintingSystem.ProgressReflector.Ranges.Count != 0)
				return float.IsNaN((float)PrintingSystem.ProgressReflector.Ranges[0]);
			else
				return false;
		}
		#endregion
		#region DocumentPreviewModelBase overrides
		public override bool IsScaleVisible {
			get { return true; }
		}
		public override bool IsSearchVisible {
			get { return true; }
		}
		public override int PageCount {
			get {
				if(PrintingSystem == null)
					return 0;
				return PrintingSystem.Document.Pages.Count;
			}
		}
		protected override FrameworkElement GetPageContent() {
			if(PageCount == 0)
				return null;
			FrameworkElement pageContent = null;
			FrameworkElement pageCacheEntry = pagesCache.GetPage(CurrentPageIndex);
			if(pageCacheEntry == null) {
				try {
					pageContent = VisualizePage(CurrentPageIndex);
					pagesCache.AddPage(pageContent, CurrentPageIndex);
					IsIncorrectPageContent = false;
				} catch(XamlParseException) {
					IsIncorrectPageContent = true;
				}
			} else {
				pageContent = pageCacheEntry;
			}
			return pageContent;
		}
		protected abstract FrameworkElement VisualizePage(int pageIndex);
		public override bool ProgressMarqueeVisibility {
			get {
				return IsCreating && IsProgressMarquee();
			}
		}
		public override bool ProgressVisibility {
			get {
				return (!IsProgressReflectorComplited() && !ProgressMarqueeVisibility && IsCreating) || IsExporting;
			}
		}
		public override int ProgressMaximum {
			get {
				if(PrintingSystem == null)
					return 0;
				return PrintingSystem.ProgressReflector.MaximumCore;
			}
		}
		public override int ProgressValue {
			get {
				if(PrintingSystem == null)
					return 0;
				return PrintingSystem.ProgressReflector.PositionCore;
			}
		}
		public override DocumentMapTreeViewNode DocumentMapRootNode {
			get {
				if(PrintingSystem == null)
					return null;
				return DocumentMapTreeViewNodeBuilder.Build(PrintingSystem.Document);
			}
		}
		protected override IHighlightingService HighlightingService {
			get {
				return highlightingService;
			}
		}
		protected override bool CanShowSearchPanel(object parameter) {
#if SL
			return BuildPagesComplete && !IsExporting;
#else
			return BuildPagesComplete && !IsExporting && !IsSaving;
#endif
		}
#if !SILVERLIGHT
		protected override void Print(object parameter) {
			PrintDialog();
		}
#endif
		protected override bool CanPrint(object parameter) {
#if SL
			return BuildPagesComplete && !IsExporting;
#else
			return BuildPagesComplete && !IsExporting && !IsSaving;
#endif
		}
#if !SL
		protected override void PrintDirect(object parameter) {
			if (parameter is string)
				PrintDirect((string)parameter);
			else if (parameter is PrintQueue)
				PrintDirect((PrintQueue)parameter);
			else
				PrintDirect();
		}
		protected override bool CanPrintDirect(object parameter) {
			return BuildPagesComplete && !IsExporting && !IsSaving;
		}
		protected override void Export(object parameter) {
			ExportFormat format = parameter != null ?
				(ExportFormat)Enum.Parse(typeof(ExportFormat), parameter.ToString()) :
				ExportFormatConverter.ToExportFormat(PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat);
			IsExporting = true;
			try {
				exportSendService.Export(PrintingSystem, DialogService.GetParentWindow(), GetExportOptions(format), DialogService);
				PrintingSystem.ExportOptions.PrintPreview.DefaultExportFormat = ExportFormatConverter.ToExportCommand(format);
			} finally {
				IsExporting = false;
			}
		}
		protected override bool CanExport(object parameter) {
			return BuildPagesComplete && !IsExporting && !IsSaving;
		}
		protected override void Send(object parameter) {
			ExportFormat format = parameter != null ?
				(ExportFormat)Enum.Parse(typeof(ExportFormat), parameter.ToString()) :
				ExportFormatConverter.ToExportFormat(PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat);
			IsExporting = true;
			try {
				exportSendService.SendFileByEmail(PrintingSystem, new EmailSender(), DialogService.GetParentWindow(), GetExportOptions(format), DialogService);
				PrintingSystem.ExportOptions.PrintPreview.DefaultSendFormat = ExportFormatConverter.ToSendCommand(format);
			} finally {
				IsExporting = false;
			}
		}
		protected override bool CanSend(object parameter) {
			return BuildPagesComplete && !IsExporting && !IsSaving;
		}
#endif
		protected override bool CanStop(object parameter) {
			return IsCreating;
		}
		protected override void PageSetup(object parameter) {
#if SL
			ShowPageSetupDialog();
#else
			ShowPageSetupDialog(DialogService.GetParentWindow());
#endif
		}
		protected override bool CanPageSetup(object parameter) {
#if SL
			return BuildPagesComplete && PrintingSystem.Document.CanChangePageSettings && !IsExporting;
#else
			return BuildPagesComplete && PrintingSystem.Document.CanChangePageSettings && !IsExporting && !IsSaving;
#endif
		}
		protected override void Stop(object parameter) {
			PrintingSystem.Document.StopPageBuilding();
		}
#if !SL
		protected override void Open(object parameter) {
			string caption = PreviewLocalizer.GetString(PreviewStringId.OpenFileDialog_Title);
			string filter = String.Format(PreviewLocalizer.GetString(PreviewStringId.OpenFileDialog_Filter), NativeFormatOptionsController.NativeFormatExtension);
			using(Stream stream = DialogService.ShowOpenFileDialog(caption, filter)) {
				if(stream != null) {
					try {
						PrintingSystem.LoadDocument(stream);
					} catch {
						DialogService.ShowError(PreviewLocalizer.GetString(PreviewStringId.Msg_CannotLoadDocument), PrintingLocalizer.GetString(PrintingStringId.Error));
					} finally {
						IsCreating = false;
						ClearCache();
						commands[PrintingSystemCommand.DocumentMap].RaiseCanExecuteChanged();
						if(!CanShowDocumentMap(null)) {
							IsDocumentMapVisible = false;
						}
						commands[PrintingSystemCommand.Parameters].RaiseCanExecuteChanged();
						if(!CanShowParametersPanel(null)) {
							IsParametersPanelVisible = false;
						}
						if(CurrentPageIndex == 0)
							UpdateCurrentPageContent();
						else
							CurrentPageIndex = 0;
					}
				}
			}
		}
		protected override bool CanOpen(object parameter) {
			return PrintingSystem != null && !IsCreating && !IsExporting && !IsSaving;
		}
		protected override void Save(object parameter) {
			string caption = PreviewLocalizer.GetString(PreviewStringId.SaveDlg_Title);
			string filter = String.Format(
				"{0} (*{1})|*{1}",
				PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterNativeFormat),
				NativeFormatOptionsController.NativeFormatExtension);
			string initialDirectory = PrintingSystem.ExportOptions.PrintPreview.DefaultDirectory;
			string fileName = PrintingSystem.ExportOptions.PrintPreview.DefaultFileName;
			using(Stream stream = DialogService.ShowSaveFileDialog(caption, filter, 0, initialDirectory, fileName)) {
				if(stream != null) {
					IsSaving = true;
					PrintingSystem.SaveDocument(stream);
				}
			}
		}
		protected override bool CanSave(object parameter) {
			return BuildPagesComplete && !IsExporting && !IsSaving;
		}
#endif
		protected override bool CanShowDocumentMap(object parameter) {
			return BuildPagesComplete && PrintingSystem.Document.BookmarkNodes.Count != 0;
		}
		protected void ClearCache() {
			pagesCache.Clear();
			firstPageDisplayed = false;
		}
		protected override bool IsHitTestResult(FrameworkElement element) {
			return VisualHelper.GetIsVisualBrickBorder(element);
		}
		#endregion
	}
}
