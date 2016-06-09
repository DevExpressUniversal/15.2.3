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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.IO;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.Parameters;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
#if SL
using System.ServiceModel;
using HitTestHelper = DevExpress.Xpf.Core.HitTestHelper;
#else
using HitTestHelper = System.Windows.Media.VisualTreeHelper;
using System.Windows.Media;
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Printing.Parameters.Models;
#endif
namespace DevExpress.Xpf.Printing {
	public abstract class DocumentPreviewModelBase : PreviewModelBase, IDocumentPreviewModel {
		#region static
		static void BringIntoView(FrameworkElement element) {
			if(element == null)
				throw new ArgumentNullException("element");
			IScrollInfo scrollInfo = LayoutHelper.FindParentObject<IScrollInfo>(element);
			if(scrollInfo == null)
				return;
			scrollInfo.MakeVisible(element, Rect.Empty);
		}
		#endregion
		#region Fields and Properties
		readonly InputController inputController;
		int currentPageIndex = -1;
		bool isDocumentMapVisible;
		bool isParametersPanelVisible;
		bool isSearchPanelVisible;
		bool requiredScrollingToHighlightedElement;
		bool isDocumentMapVisibilityToggledOn = true;
		bool isOpenButtonVisible = true;
		bool isSaveButtonVisible = true;
		bool isSendVisible = true;
#if SILVERLIGHT
		PrintMode defaultPrintMode = PrintMode.Native;
#endif
		DocumentMapTreeViewNode documentMapSelectedNode;
		HighlightingPriority highLightingPriority = HighlightingPriority.None;
		BrickInfo foundBrickInfo;
		public event EventHandler<PreviewClickEventArgs> PreviewClick;
		public event EventHandler<PreviewClickEventArgs> PreviewMouseMove;
#if !SILVERLIGHT
		public event EventHandler<PreviewClickEventArgs> PreviewDoubleClick;
#endif
		public override InputController InputController {
			get { return inputController; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool IsDocumentMapToggledDuringDocumentCreating { get; set; }
		public abstract bool IsEmptyDocument { get; }
		#endregion
		#region Constructors
		public DocumentPreviewModelBase() {
			CreateCommands();
			inputController = new DocumentPreviewInputController() { Model = this };
		}
		#endregion
		#region IDocumentPreviewModel Members
#if SILVERLIGHT
		public PrintMode DefaultPrintMode {
			get { return defaultPrintMode; }
			set {
				if(Application.Current.IsRunningOutOfBrowser)
					return;
				defaultPrintMode = value;
			}
		}
#endif
		public abstract bool ProgressVisibility { get; }
		public abstract int ProgressMaximum { get; }
		public abstract int ProgressValue { get; }
		public abstract bool ProgressMarqueeVisibility { get; }
		public abstract DocumentMapTreeViewNode DocumentMapRootNode { get; }
		public abstract ParametersModel ParametersModel { get; }
		protected ExportFormat DefaultExportFormat {
			get {
				return ExportFormatConverter.ToExportFormat(DocumentExportOptions.PrintPreview.DefaultExportFormat);
			}
			set {
				DocumentExportOptions.PrintPreview.DefaultExportFormat = ExportFormatConverter.ToExportCommand(value);
			}
		}
#if !SILVERLIGHT
		protected ExportFormat DefaultSendFormat {
			get {
				return ExportFormatConverter.ToExportFormat(DocumentExportOptions.PrintPreview.DefaultSendFormat);
			}
			set {
				DocumentExportOptions.PrintPreview.DefaultSendFormat = ExportFormatConverter.ToSendCommand(value);
			}
		}
#endif
#if SILVERLIGHT
		public abstract void CreateDocument();
#endif
		protected override void ProcessPageContent(FrameworkElement pageContent) {
			switch(highLightingPriority) {
				case HighlightingPriority.None:
					if(HighlightingService != null)
						HighlightingService.HideHighlighting();
					break;
				case HighlightingPriority.DocumentMap:
					if(DocumentMapSelectedNode != null && CurrentPageIndex == DocumentMapSelectedNode.PageIndex)
						HighlightElement(pageContent, DocumentMapSelectedNode.AssociatedElementTag);
					break;
				case HighlightingPriority.Search:
					if(FoundBrickInfo != null && !string.IsNullOrEmpty(FoundBrickInfo.BrickTag))
						HighlightElement(pageContent, FoundBrickInfo.BrickTag);
					break;
				default:
					throw new NotSupportedException("HighLightingPriority");
			}
		}
		public int CurrentPageIndex {
			get {
				return currentPageIndex;
			}
			set {
				if(value < 0 || value >= PageCount) {
					DialogService.ShowError(
						string.Format(PreviewLocalizer.GetString(PreviewStringId.Msg_GoToNonExistentPage), value + 1),
						PrintingLocalizer.GetString(PrintingStringId.Error));
					return;
				}
				SetCurrentPageIndex(value);
			}
		}
		protected internal void SetCurrentPageIndex(int value) {
			if(currentPageIndex == value)
				return;
			currentPageIndex = value;
			OnCurrentPageIndexChanged();
			RaisePropertyChanged(() => CurrentPageIndex);
			RaisePropertyChanged(() => CurrentPageNumber);
			RaisePropertyChanged(() => PageContent);
			RaisePropertyChanged(() => PageViewWidth);
			RaisePropertyChanged(() => PageViewHeight);
			RaiseNavigationCommandsCanExecuteChanged();
		}
		public virtual bool IsScaleVisible {
			get { return false; }
		}
		public virtual bool IsSearchVisible {
			get { return false; }
		}
		public virtual bool IsSetWatermarkVisible {
			get { return false; }
		}
		public int CurrentPageNumber {
			get {
				return CurrentPageIndex + 1;
			}
			set {
				CurrentPageIndex = value - 1;
			}
		}
		protected virtual IHighlightingService HighlightingService {
			get {
				return null;
			}
		}
		public virtual bool IsDocumentMapVisible {
			get {
				return isDocumentMapVisible;
			}
			set {
				if(!IsDocumentMapToggledDuringDocumentCreating)
					isDocumentMapVisibilityToggledOn = value;
				if((!CanShowDocumentMap(null) || !isDocumentMapVisibilityToggledOn) && value)
					return;
				isDocumentMapVisible = value;
				RaisePropertyChanged(() => IsDocumentMapVisible);
			}
		}
		public virtual bool IsOpenButtonVisible { get { return isOpenButtonVisible; } set { isOpenButtonVisible = value; } }
		public virtual bool IsSaveButtonVisible { get { return isSaveButtonVisible; } set { isSaveButtonVisible = value; } }
		public virtual bool IsSendVisible { get { return isSendVisible; } set { isSendVisible = value; } }
		public virtual bool IsParametersPanelVisible {
			get {
				return isParametersPanelVisible;
			}
			set {
				if(!CanShowParametersPanel(null) && value)
					return;
				isParametersPanelVisible = value;
				RaisePropertyChanged(() => IsParametersPanelVisible);
			}
		}
		public bool IsSearchPanelVisible {
			get { return isSearchPanelVisible; }
			set {
				if(!CanShowSearchPanel(null) && value)
					return;
				isSearchPanelVisible = value;
				RaisePropertyChanged(() => IsSearchPanelVisible);
			}
		}
		public DocumentMapTreeViewNode DocumentMapSelectedNode {
			get {
				return documentMapSelectedNode;
			}
			set {
				documentMapSelectedNode = value;
				if(HighlightingService != null) {
					HighlightingService.HideHighlighting();
				}
				if(value == null)
					return;
				requiredScrollingToHighlightedElement = true;
				highLightingPriority = HighlightingPriority.DocumentMap;
				CurrentPageIndex = DocumentMapSelectedNode.PageIndex >= 0 ? DocumentMapSelectedNode.PageIndex : 0;
				RaisePropertyChanged(() => DocumentMapSelectedNode);
				RaisePropertyChanged(() => PageContent);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BrickInfo FoundBrickInfo {
			get { return foundBrickInfo; }
			set {
				foundBrickInfo = value;
				if(foundBrickInfo == null)
					highLightingPriority = HighlightingPriority.None;
				else {
					if(!string.IsNullOrEmpty(foundBrickInfo.BrickTag)) {
						requiredScrollingToHighlightedElement = true;
						highLightingPriority = HighlightingPriority.Search;
						CurrentPageIndex = foundBrickInfo.PageIndex >= 0 ? foundBrickInfo.PageIndex : 0;
					} else
						highLightingPriority = HighlightingPriority.None;
					RaisePropertyChanged(() => PageContent);
				}
			}
		}
		public ICommand PrintCommand {
			get {
				return commands[PrintingSystemCommand.Print];
			}
		}
#if SILVERLIGHT
		public ICommand PrintPdfCommand {
			get {
				return commands[PrintingSystemCommand.PrintPdf];
			}
		}
#endif
		public ICommand FirstPageCommand {
			get {
				return commands[PrintingSystemCommand.ShowFirstPage];
			}
		}
		public ICommand PreviousPageCommand {
			get {
				return commands[PrintingSystemCommand.ShowPrevPage];
			}
		}
		public ICommand NextPageCommand {
			get {
				return commands[PrintingSystemCommand.ShowNextPage];
			}
		}
		public ICommand LastPageCommand {
			get {
				return commands[PrintingSystemCommand.ShowLastPage];
			}
		}
		public ICommand ExportCommand {
			get {
				return commands[PrintingSystemCommand.ExportFile];
			}
		}
		public ICommand WatermarkCommand {
			get {
				return commands[PrintingSystemCommand.Watermark];
			}
		}
#if SL
		public ICommand ExportToWindowCommand {
			get {
				return commands[PrintingSystemCommand.ExportFileToWindow];
			}
		}
#endif
		public ICommand StopCommand {
			get {
				return commands[PrintingSystemCommand.StopPageBuilding];
			}
		}
		public ICommand ToggleDocumentMapCommand {
			get {
				return commands[PrintingSystemCommand.DocumentMap];
			}
		}
		public ICommand ToggleParametersPanelCommand {
			get {
				return commands[PrintingSystemCommand.Parameters];
			}
		}
		public ICommand PageSetupCommand {
			get {
				return commands[PrintingSystemCommand.PageSetup];
			}
		}
		public ICommand ScaleCommand {
			get {
				return commands[PrintingSystemCommand.Scale];
			}
		}
		public ICommand ToggleSearchPanelCommand {
			get {
				return commands[PrintingSystemCommand.Find];
			}
		}
#if !SILVERLIGHT
		public ICommand PrintDirectCommand {
			get {
				return commands[PrintingSystemCommand.PrintDirect];
			}
		}
		public ICommand SendCommand {
			get {
				return commands[PrintingSystemCommand.SendFile];
			}
		}
		public ICommand OpenCommand {
			get {
				return commands[PrintingSystemCommand.Open];
			}
		}
		public ICommand SaveCommand {
			get {
				return commands[PrintingSystemCommand.Save];
			}
		}
#else
		public ICommand RefreshCommand {
			get {
				return commands[PrintingSystemCommand.Refresh];
			}
		}
#endif
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void HandlePreviewMouseLeftButtonUp(MouseButtonEventArgs e, FrameworkElement source) {
			FrameworkElement element = GetHitTestResult(e, source);
			if(element != null) {
				string navigationPair = PreviewClickHelper.GetNavigationPair(element);
				if(!string.IsNullOrEmpty(navigationPair)) {
					int pageIndex = Convert.ToInt32(DocumentMapTreeViewNodeHelper.GetPageIndexByTag(navigationPair));
					if(pageIndex >= 0 && pageIndex < PageCount)
						FoundBrickInfo = new BrickInfo(navigationPair, pageIndex);
				}
			}
			if(PreviewClick != null) {
				string tag = element != null ? PreviewClickHelper.GetTag(element) : null;
				PreviewClick(this, new PreviewClickEventArgs(tag, element));
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void HandlePreviewMouseLeftButtonDown(MouseButtonEventArgs e, FrameworkElement source) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void HandlePreviewMouseMove(MouseEventArgs e, FrameworkElement source) {
			if(PreviewMouseMove != null) {
				FrameworkElement element = GetHitTestResult(e, source);
				string tag = element != null ? PreviewClickHelper.GetTag(element) : null;
				PreviewMouseMove(this, new PreviewClickEventArgs(tag, element));
			}
		}
#if !SILVERLIGHT
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void HandlePreviewDoubleClick(MouseEventArgs e, FrameworkElement source) {
			if(PreviewDoubleClick != null) {
				FrameworkElement element = GetHitTestResult(e, source);
				string tag = element != null ? PreviewClickHelper.GetTag(element) : null;
				PreviewDoubleClick(this, new PreviewClickEventArgs(tag, element));
			}
		}
#endif
		FrameworkElement GetHitTestResult(MouseEventArgs e, FrameworkElement source) {
			UIElement topLevelVisual = LayoutHelper.GetTopLevelVisual(source) as UIElement;
			if(topLevelVisual == null)
				return null;
			FrameworkElement result = null;
			HitTestHelper.HitTest(topLevelVisual, d => {
				FrameworkElement element = d as FrameworkElement;
				if(element != null && IsHitTestResult(element)) {
					if(element.FindIsInParents(source)) {
						result = element;
					}
				}
				return HitTestFilterBehavior.Continue;
			}, res => HitTestResultBehavior.Continue, new PointHitTestParameters(e.GetPosition(topLevelVisual)));
			return result;
		}
		protected virtual bool IsHitTestResult(FrameworkElement control) {
			return false;
		}
#if SILVERLIGHT
		public abstract void Clear();
		protected void PrintMethodSelector(object parameter, Action defaultPrintAction, Action pdfPrintAction) {
			if(parameter != null) {
				DefaultPrintMode = PrintMode.Native;
				defaultPrintAction();
			} else if(DefaultPrintMode == PrintMode.Native) defaultPrintAction(); 
			else pdfPrintAction();
		}
#endif
		protected abstract void Print(object parameter);
		protected abstract bool CanPrint(object parameter);
		protected abstract void PageSetup(object parameter);
		protected abstract bool CanPageSetup(object parameter);
		protected abstract void Scale(object parameter);
		protected abstract bool CanScale(object parameter);
		protected abstract void Stop(object parameter);
		protected abstract bool CanStop(object parameter);
		protected abstract bool CanShowDocumentMap(object parameter);
		protected abstract bool CanShowParametersPanel(object parameter);
		protected abstract bool CanShowSearchPanel(object parameter);
#if !SILVERLIGHT
		protected abstract void PrintDirect(object parameter);
		protected abstract bool CanPrintDirect(object parameter);
		protected abstract void Send(object parameter);
		protected abstract bool CanSend(object parameter);
		protected abstract void Open(object parameter);
		protected abstract bool CanOpen(object parameter);
		protected abstract void Save(object parameter);
		protected abstract bool CanSave(object parameter);
#else
		protected abstract void Refresh(object parameter);
		protected abstract bool CanRefresh(object parameter);
#endif
		protected virtual void Export(object parameter) {
			Export(parameter, BeginExport, Export, EndExport);
		}
		protected virtual bool CanExport(object parameter) {
			var format = GetExportFormatByExportParameter(parameter);
			bool canExport = PageCount > 0 && DocumentExportOptions != null;
			if(!canExport)
				return false;
			var exportModes = DocumentExportModes;
			switch(format) {
				case ExportFormat.Csv: return canExport;
				case ExportFormat.Htm: return canExport && exportModes != null && !exportModes.Html.IsEmpty();
				case ExportFormat.Image: return canExport && exportModes != null && !exportModes.Image.IsEmpty();
				case ExportFormat.Mht: return canExport && exportModes != null && !exportModes.Html.IsEmpty();
				case ExportFormat.Pdf: return canExport;
				case ExportFormat.Rtf: return canExport && exportModes != null && !exportModes.Rtf.IsEmpty();
				case ExportFormat.Txt: return canExport;
				case ExportFormat.Xls: return canExport && exportModes != null && !exportModes.Xls.IsEmpty();
				case ExportFormat.Xlsx: return canExport && exportModes != null && !exportModes.Xlsx.IsEmpty();
				case ExportFormat.Xps: return canExport;
				case ExportFormat.Prnx: return canExport;
				default: throw new ArgumentException("format");
			}
		}
#if SL
		protected virtual void ExportToWindow(object parameter) {
			Export(parameter, BeginExportToWindow, ExportToWindow, EndExportToWindow);
		}
		protected abstract void ExportToWindow(ExportFormat format);
		protected abstract void PrintPdf(object Parameter);
#endif
		protected abstract void SetWatermark(object parameter);
		protected abstract bool CanSetWatermark(object parameter);
		void Export(object parameter, Action<ExportFormat> beginExport, Action<ExportFormat> doExport, Action endExport) {
			var format = GetExportFormatByExportParameter(parameter);
			Debug.Assert(PageCount > 0);
			DefaultExportFormat = format;
			ExportOptionsConfiguratorService.Completed += ExportOptionsConfiguratorService_Completed;
			ExportOptionsConfiguratorService.Context = new ExportOptionContext(format, beginExport, doExport, endExport);
			ExportOptionsConfiguratorService.Configure(DocumentExportOptions.GetByFormat(format), DocumentExportOptions.PrintPreview, DocumentExportModes, DocumentHiddenOptions);
		}
#if SL
		protected virtual bool CanExportToWindow(object parameter) {
			return Application.Current.Host.Settings.EnableHTMLAccess && CanExport(parameter);
		}
#endif
		void HighlightElement(FrameworkElement pageContent, string brickTag) {
			if(HighlightingService == null || pageContent == null)
				return;
			HighlightingService.HideHighlighting();
			new OnLoadedScheduler().Schedule(() => {
				FrameworkElement target = LayoutHelper.FindElement(pageContent, x => (string)x.Tag == brickTag);
				if(target != null) {
					if(requiredScrollingToHighlightedElement) {
						requiredScrollingToHighlightedElement = false;
						new OnLoadedScheduler().Schedule(() => BringIntoView(target), target);
					}
					HighlightingService.ShowHighlighting(pageContent, target);
				}
			}, pageContent);
		}
		protected virtual void GoToFirstPage(object parameter) {
			CurrentPageIndex = 0;
		}
		bool CanGoToFirstPage(object parameter) {
			return CurrentPageIndex > 0;
		}
		void GoToPreviousPage(object parameter) {
			CurrentPageIndex--;
		}
		bool CanGoToPreviousPage(object parameter) {
			return CurrentPageIndex > 0;
		}
		void GoToNextPage(object parameter) {
			CurrentPageIndex++;
		}
		bool CanGoToNextPage(object parameter) {
			return CurrentPageIndex + 1 < PageCount;
		}
		void GoToLastPage(object parameter) {
			CurrentPageIndex = PageCount - 1;
		}
		bool CanGoToLastPage(object parameter) {
			return CurrentPageIndex < PageCount - 1;
		}
		void ToggleIsDocumentMapVisible(object parameter) {
			IsDocumentMapVisible = !IsDocumentMapVisible;
			if(!IsDocumentMapVisible && HighlightingService != null) {
				if(highLightingPriority == HighlightingPriority.DocumentMap) {
					HighlightingService.HideHighlighting();
					highLightingPriority = HighlightingPriority.None;
				}
			} else {
				RaisePropertyChanged(() => PageContent);
			}
		}
		void ToggleIsParametersPanelVisible(object parameter) {
			IsParametersPanelVisible = !IsParametersPanelVisible;
		}
		void ToggleSearchPanelVisibility(object parameter) {
			IsSearchPanelVisible = !IsSearchPanelVisible;
			if(!IsSearchPanelVisible && HighlightingService != null) {
				if(highLightingPriority == HighlightingPriority.Search) {
					HighlightingService.HideHighlighting();
					highLightingPriority = HighlightingPriority.None;
				}
			}
		}
		#endregion
		void CreateCommands() {
			commands.Add(
				PrintingSystemCommand.Print,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => Print(parameter)),
					CanPrint, false));
			commands.Add(
				PrintingSystemCommand.PageSetup,
				DelegateCommandFactory.Create<object>(PageSetup, CanPageSetup, false));
			commands.Add(
				PrintingSystemCommand.ShowFirstPage,
				DelegateCommandFactory.Create<object>(GoToFirstPage, CanGoToFirstPage, false));
			commands.Add(
				PrintingSystemCommand.ShowPrevPage,
				DelegateCommandFactory.Create<object>(GoToPreviousPage, CanGoToPreviousPage, false));
			commands.Add(
				PrintingSystemCommand.ShowNextPage,
				DelegateCommandFactory.Create<object>(GoToNextPage, CanGoToNextPage, false));
			commands.Add(
				PrintingSystemCommand.ShowLastPage,
				DelegateCommandFactory.Create<object>(GoToLastPage, CanGoToLastPage, false));
			commands.Add(
				PrintingSystemCommand.Scale,
				DelegateCommandFactory.Create<object>(Scale, CanScale, false));
			commands.Add(
				PrintingSystemCommand.ExportFile,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => Export(parameter)),
					CanExport, false));
#if SL
			commands.Add(
				PrintingSystemCommand.ExportFileToWindow,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => ExportToWindow(parameter)),
					CanExportToWindow, false));
			commands.Add(
				PrintingSystemCommand.PrintPdf,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => PrintPdf(parameter)),
					CanExportToWindow, false));
#endif
			commands.Add(
				PrintingSystemCommand.Watermark,
				DelegateCommandFactory.Create<object>(SetWatermark, CanSetWatermark, false));
			commands.Add(
				PrintingSystemCommand.StopPageBuilding,
				DelegateCommandFactory.Create<object>(Stop, CanStop, false));
			commands.Add(
				PrintingSystemCommand.DocumentMap,
				DelegateCommandFactory.Create<object>(ToggleIsDocumentMapVisible, CanShowDocumentMap, false));
			commands.Add(
				PrintingSystemCommand.Parameters,
				DelegateCommandFactory.Create<object>(ToggleIsParametersPanelVisible, CanShowParametersPanel, false));
			commands.Add(
				PrintingSystemCommand.Find,
				DelegateCommandFactory.Create<object>(ToggleSearchPanelVisibility, CanShowSearchPanel, false));
#if !SILVERLIGHT
			commands.Add(
				PrintingSystemCommand.PrintDirect,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => PrintDirect(parameter)),
					CanPrintDirect, false));
			commands.Add(
				PrintingSystemCommand.SendFile,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => Send(parameter)),
					CanSend, false));
			commands.Add(
				PrintingSystemCommand.Open,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => Open(parameter)),
					CanOpen, false));
			commands.Add(
				PrintingSystemCommand.Save,
				DelegateCommandFactory.Create<object>(
					parameter => SafeCommandHandler(() => Save(parameter)),
					CanSave, false));
#else
			commands.Add(PrintingSystemCommand.Refresh, DelegateCommandFactory.Create<object>(Refresh, CanRefresh, false));
#endif
		}
		protected void RaiseNavigationCommandsCanExecuteChanged() {
			commands[PrintingSystemCommand.ShowFirstPage].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.ShowPrevPage].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.ShowNextPage].RaiseCanExecuteChanged();
			commands[PrintingSystemCommand.ShowLastPage].RaiseCanExecuteChanged();
		}
		protected virtual void OnCurrentPageIndexChanged() {
		}
		IExportOptionsConfiguratorService exportOptionsConfiguratorService;
		protected abstract ExportOptions DocumentExportOptions { get; set; }
		protected abstract AvailableExportModes DocumentExportModes { get; }
		protected abstract List<ExportOptionKind> DocumentHiddenOptions { get; }
		void ExportOptionsConfiguratorService_Completed(object sender, AsyncCompletedEventArgs e) {
			ExportOptionsConfiguratorService.Completed -= ExportOptionsConfiguratorService_Completed;
			if(e.Cancelled || e.Error != null)
				return;
			var context = ExportOptionsConfiguratorService.Context;
			var format = context.Format;
			if(context.BeginExportAction != null)
				context.BeginExportAction(format);
			if(context.ExportAction != null)
				context.ExportAction(format);
			if(context.EndExportAction != null)
				context.EndExportAction();
		}
		protected abstract void Export(ExportFormat format);
		protected Stream ShowSaveExportedFileDialog(ExportFormat format) {
			var options = DocumentExportOptions.GetByFormat(format);
			var controller = ExportOptionsControllerBase.GetControllerByOptions(options);
			var filterIndex = Array.IndexOf(controller.FileExtensions, controller.GetFileExtension(options)) + 1;
			return DialogService.ShowSaveFileDialog(
				PreviewLocalizer.GetString(PreviewStringId.SaveDlg_Title),
				controller.Filter,
				filterIndex,
				null,
				DocumentExportOptions.PrintPreview.DefaultFileName);
		}
		protected virtual void BeginExport(ExportFormat format) {
		}
		protected virtual void EndExport() {
		}
#if SL
		protected virtual void BeginExportToWindow(ExportFormat format) {
		}
		protected virtual void EndExportToWindow() {
		}
#endif
		protected virtual IExportOptionsConfiguratorService ExportOptionsConfiguratorService {
			get {
				if(exportOptionsConfiguratorService == null)
					exportOptionsConfiguratorService = new ExportOptionsConfiguratorService();
				return exportOptionsConfiguratorService;
			}
			set {
				exportOptionsConfiguratorService = value;
			}
		}
#if !SL
		protected virtual DataContext GetDataContext() {
			return null;
		}
#endif
		protected ExportFormat GetExportFormatByExportParameter(object parameter) {
			return parameter == null ? DefaultExportFormat : (ExportFormat)Enum.Parse(typeof(ExportFormat), (string)parameter, false);
		}
	}
}
