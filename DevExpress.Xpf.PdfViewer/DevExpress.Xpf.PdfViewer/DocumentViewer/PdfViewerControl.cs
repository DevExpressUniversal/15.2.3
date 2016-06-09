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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Localization;
using DevExpress.Pdf.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.DocumentViewer.Extensions;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PdfViewer.Extensions;
using DevExpress.Xpf.PdfViewer.Helpers;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.PdfViewer.Themes;
using DevExpress.Xpf.PdfViewer.UI;
using DevExpress.Xpf.Utils;
using Clipboard = System.Windows.Clipboard;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using IDocument = DevExpress.Xpf.DocumentViewer.IDocument;
using Size = System.Drawing.Size;
namespace DevExpress.Xpf.PdfViewer {
	public enum CursorModeType {
		HandTool = 0,
		SelectTool = 1,
		MarqueeZoom = 2
	}
	public class GetDocumentPasswordEventArgs : RoutedEventArgs {
		public string Path { get; private set; }
		public SecureString Password { get; set; }
		public GetDocumentPasswordEventArgs(string path)
			: base(PdfViewerControl.GetDocumentPasswordEvent) {
			Path = path;
		}
	}
	public delegate void GetDocumentPasswordEventHandler(DependencyObject d, GetDocumentPasswordEventArgs e);
	public class UriOpeningEventArgs : RoutedEventArgs {
		public Uri Uri { get; private set; }
		public bool Cancel { get; set; }
		public UriOpeningEventArgs(Uri uri)
			: base(PdfViewerControl.UriOpeningEvent) {
			Uri = uri;
		}
	}
	public class AttachmentOpeningEventArgs : RoutedEventArgs {
		public PdfFileAttachment Attachment { get; private set; }
		public bool Cancel { get; set; }
		public AttachmentOpeningEventArgs(PdfFileAttachment attachment)
			: base(PdfViewerControl.AttachmentOpeningEvent) {
			Attachment = attachment;
		}
	}
	public delegate void AttachmentOpeningEventHandler(DependencyObject d, AttachmentOpeningEventArgs e);
	public delegate void ReferencedDocumentOpeningEventHandler(DependencyObject d, ReferencedDocumentOpeningEventArgs e);
	public class ReferencedDocumentOpeningEventArgs : RoutedEventArgs {
		public string DocumentSource { get; private set; }
		public bool OpenInNewWindow { get; private set; }
		public bool Cancel { get; set; }
		public ReferencedDocumentOpeningEventArgs(string documentSource, bool openInNewWindow)
			: base(PdfViewerControl.ReferencedDocumentOpeningEvent) {
			DocumentSource = documentSource;
			OpenInNewWindow = openInNewWindow;
		}
	}
	public delegate void UriOpeningEventHandler(DependencyObject d, UriOpeningEventArgs e);
	public class SelectionEventArgs : RoutedEventArgs {
		public PdfDocumentPosition DocumentPosition { get; private set; }
		public SelectionEventArgs(PdfDocumentPosition position) {
			DocumentPosition = position;
		}
	}
	public delegate void DocumentClosingEventHandler(DependencyObject d, DocumentClosingEventArgs e);
	public class DocumentClosingEventArgs : RoutedEventArgs {
		public bool Cancel { get; set; }
		public MessageBoxResult? SaveDialogResult { get; set; }
		public DocumentClosingEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
	}
	public delegate void SelectionEventHandler(DependencyObject d, SelectionEventArgs e);
	[DXToolboxBrowsable(true)]
	public class PdfViewerControl : DocumentViewerControl, IPdfViewer {
		public static readonly DependencyProperty AsyncDocumentLoadProperty;
		public static readonly DependencyProperty AllowCachePagesProperty;
		public static readonly DependencyProperty CacheSizeProperty;
		public static readonly DependencyProperty HighlightSelectionColorProperty;
		public static readonly DependencyProperty CaretColorProperty;
		public static readonly DependencyProperty PagePaddingProperty;
		public static readonly DependencyProperty AllowCurrentPageHighlightingProperty;
		public static readonly DependencyProperty CursorModeProperty;
		public static readonly DependencyProperty ShowStartScreenProperty;
		public static readonly DependencyProperty RecentFilesProperty;
		public static readonly DependencyProperty NumberOfRecentFilesProperty;
		public static readonly DependencyProperty ShowOpenFileOnStartScreenProperty;
		internal static readonly DependencyPropertyKey HasSelectionPropertyKey;
		public static readonly DependencyProperty HasSelectionProperty;
		public static readonly DependencyProperty MaxPrintingDpiProperty;
		public static readonly DependencyProperty DocumentCreatorProperty;
		public static readonly DependencyProperty DocumentProducerProperty;
		public static readonly DependencyProperty DetachStreamOnLoadCompleteProperty;
		public static readonly DependencyProperty InteractionProviderProperty;
		public static readonly DependencyProperty ActualInteractionProviderProperty;
		static readonly DependencyPropertyKey ActualInteractionProviderPropertyKey;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty PrintPreviewDialogTemplateProperty;
		public static readonly DependencyProperty OutlinesViewerSettingsProperty;
		public static readonly DependencyProperty SaveFileDialogTemplateProperty;
		public static readonly DependencyProperty PageLayoutProperty;
		static readonly DependencyPropertyKey HasOutlinesPropertyKey;
		public static readonly DependencyProperty HasOutlinesProperty;
		static readonly DependencyPropertyKey ActualAttachmentsViewerSettingsPropertyKey;
		public static readonly DependencyProperty ActualAttachmentsViewerSettingsProperty;
		public static readonly DependencyProperty AttachmentsViewerSettingsProperty;
		static readonly DependencyPropertyKey HasAttachmentsPropertyKey;
		public static readonly DependencyProperty HasAttachmentsProperty;
		public static RoutedEvent CursorModeChangedEvent;
		public static RoutedEvent UriOpeningEvent;
		public static RoutedEvent GetDocumentPasswordEvent;
		public static RoutedEvent SelectionStartedEvent;
		public static RoutedEvent SelectionEndedEvent;
		public static RoutedEvent SelectionContinuedEvent;
		public static RoutedEvent DocumentLoadedEvent;
		public static RoutedEvent ReferencedDocumentOpeningEvent;
		public static RoutedEvent DocumentClosingEvent;
		public static RoutedEvent PageLayoutChangedEvent;
		public static RoutedEvent AttachmentOpeningEvent;
		static PdfViewerControl() {
			Type ownerType = typeof(PdfViewerControl);
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(HorizontalAlignment.Center));
			DetachStreamOnLoadCompleteProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, bool>(owner => owner.DetachStreamOnLoadComplete, false);
			AsyncDocumentLoadProperty = DependencyProperty.Register("AsyncDocumentLoad", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			HighlightSelectionColorProperty = DependencyPropertyManager.Register("HighlightSelectionColor", typeof(Color), ownerType, new FrameworkPropertyMetadata(Color.FromArgb(89, 96, 152, 192)));
			CaretColorProperty = DependencyPropertyManager.Register("CaretColor", typeof(Color), ownerType, new FrameworkPropertyMetadata(Colors.Black));
			PagePaddingProperty = DependencyPropertyManager.Register("PagePadding", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(5)));
			AllowCurrentPageHighlightingProperty = DependencyPropertyManager.Register("AllowCurrentPageHighlighting", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			CursorModeProperty = DependencyPropertyManager.Register("CursorMode", typeof(CursorModeType), ownerType,
				new FrameworkPropertyMetadata(CursorModeType.SelectTool, FrameworkPropertyMetadataOptions.None, (d, args) => ((PdfViewerControl)d).OnCursorModeChanged((CursorModeType)args.NewValue)));
			RecentFilesProperty = DependencyPropertyManager.Register("RecentFiles", typeof(ObservableCollection<RecentFileViewModel>), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			NumberOfRecentFilesProperty = DependencyPropertyManager.Register("NumberOfRecentFiles", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			ShowOpenFileOnStartScreenProperty = DependencyPropertyManager.Register("ShowOpenFileOnStartScreen", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			HasSelectionPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasSelection", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasSelectionProperty = HasSelectionPropertyKey.DependencyProperty;
			ShowStartScreenProperty = DependencyPropertyManager.Register("ShowStartScreen", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (obj, args) => ((PdfViewerControl)obj).OnShowStartScreenChanged((bool?)args.NewValue)));
			MaxPrintingDpiProperty = DependencyPropertyManager.Register("MaxPrintingDpi", typeof(int), ownerType,
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None, null, (o, value) => (int)value < 0 ? 0 : value));
			DocumentCreatorProperty = DependencyPropertyManager.Register("DocumentCreator", typeof(string), ownerType);
			DocumentProducerProperty = DependencyPropertyManager.Register("DocumentProducer", typeof(string), ownerType);
			AllowCachePagesProperty = DependencyPropertyManager.Register("AllowCachePages", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (d, e) => ((PdfViewerControl)d).AllowCachePagesChanged((bool)e.NewValue)));
			CacheSizeProperty = DependencyPropertyManager.Register("CacheSize", typeof(int), ownerType,
				new FrameworkPropertyMetadata(300000000, FrameworkPropertyMetadataOptions.None,
					(obj, args) => ((PdfViewerControl)obj).OnCacheSizeChanged((int)args.NewValue)));
			InteractionProviderProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, InteractionProvider>(owner => owner.InteractionProvider, null);
			ActualInteractionProviderPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfViewerControl, InteractionProvider>(owner => owner.ActualInteractionProvider, null,
				(control, oldValue, newValue) => control.ActualInteractionProviderChanged(oldValue, newValue));
			ActualInteractionProviderProperty = ActualInteractionProviderPropertyKey.DependencyProperty;
			IsReadOnlyProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, bool>(owner => owner.IsReadOnly, false);
			PrintPreviewDialogTemplateProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, DataTemplate>(owner => owner.PrintPreviewDialogTemplate, null);
			SaveFileDialogTemplateProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, DataTemplate>(owner => owner.SaveFileDialogTemplate, null);
			OutlinesViewerSettingsProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, PdfOutlinesViewerSettings>(control => control.OutlinesViewerSettings, null,
				(control, oldValue, newValue) => control.OutlinesViewerSettingsChanged(oldValue, newValue));
			PageLayoutProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, PdfPageLayout>(owner => owner.PageLayout, PdfPageLayout.OneColumn, (d, oldValue, newValue) => d.OnPageLayoutChanged(newValue));
			CursorModeChangedEvent = EventManager.RegisterRoutedEvent("CursorModeChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			GetDocumentPasswordEvent = EventManager.RegisterRoutedEvent("GetDocumentPassword", RoutingStrategy.Direct, typeof(GetDocumentPasswordEventHandler), ownerType);
			UriOpeningEvent = EventManager.RegisterRoutedEvent("UriOpening", RoutingStrategy.Direct,
				typeof(UriOpeningEventHandler), ownerType);
			ReferencedDocumentOpeningEvent = EventManager.RegisterRoutedEvent("ReferencedDocumentOpening", RoutingStrategy.Direct,
				typeof(ReferencedDocumentOpeningEventHandler), ownerType);
			DocumentLoadedEvent = EventManager.RegisterRoutedEvent("DocumentLoaded", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			DocumentClosingEvent = EventManager.RegisterRoutedEvent("DocumentClosing", RoutingStrategy.Direct, typeof(DocumentClosingEventHandler), ownerType);
			PageLayoutChangedEvent = EventManager.RegisterRoutedEvent("PageLayoutChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			HasOutlinesPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfViewerControl, bool>(owner => owner.HasOutlines, false);
			HasOutlinesProperty = HasOutlinesPropertyKey.DependencyProperty;
			ActualAttachmentsViewerSettingsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfViewerControl, PdfAttachmentsViewerSettings>(
				owner => owner.ActualAttachmentsViewerSettings, null, (owner, oldValue, newValue) => owner.ActualAttachmentsViewerSettingsChanged(oldValue, newValue));
			ActualAttachmentsViewerSettingsProperty = ActualAttachmentsViewerSettingsPropertyKey.DependencyProperty;
			AttachmentsViewerSettingsProperty = DependencyPropertyRegistrator.Register<PdfViewerControl, PdfAttachmentsViewerSettings>(owner => owner.AttachmentsViewerSettings,
				null, (control, oldValue, newValue) => control.AttachmentsViewerSettingsChanged(oldValue, newValue));
			HasAttachmentsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfViewerControl, bool>(owner => owner.HasAttachments, false);
			HasAttachmentsProperty = HasAttachmentsPropertyKey.DependencyProperty;
			SelectionStartedEvent = EventManager.RegisterRoutedEvent("SelectionStarted", RoutingStrategy.Direct, typeof(SelectionEventHandler), ownerType);
			SelectionEndedEvent = EventManager.RegisterRoutedEvent("SelectionEnded", RoutingStrategy.Direct, typeof(SelectionEventHandler), ownerType);
			SelectionContinuedEvent = EventManager.RegisterRoutedEvent("SelectionContinued", RoutingStrategy.Direct, typeof(SelectionEventHandler), ownerType);
			AttachmentOpeningEvent = EventManager.RegisterRoutedEvent("AttachmentOpening", RoutingStrategy.Direct, typeof(AttachmentOpeningEventHandler), ownerType);
		}
		public bool HasAttachments {
			get { return (bool)GetValue(HasAttachmentsProperty); }
			private set { SetValue(HasAttachmentsPropertyKey, value); }
		}
		public bool HasOutlines {
			get { return (bool)GetValue(HasOutlinesProperty); }
			private set { SetValue(HasOutlinesPropertyKey, value); }
		}
		public PdfOutlinesViewerSettings OutlinesViewerSettings {
			get { return (PdfOutlinesViewerSettings)GetValue(OutlinesViewerSettingsProperty); }
			set { SetValue(OutlinesViewerSettingsProperty, value); }
		}
		public DataTemplate SaveFileDialogTemplate {
			get { return (DataTemplate)GetValue(SaveFileDialogTemplateProperty); }
			set { SetValue(SaveFileDialogTemplateProperty, value); }
		}
		public DataTemplate PrintPreviewDialogTemplate {
			get { return (DataTemplate)GetValue(PrintPreviewDialogTemplateProperty); }
			set { SetValue(PrintPreviewDialogTemplateProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public bool AsyncDocumentLoad {
			get { return (bool)GetValue(AsyncDocumentLoadProperty); }
			set { SetValue(AsyncDocumentLoadProperty, value); }
		}
		public InteractionProvider InteractionProvider {
			get { return (InteractionProvider)GetValue(InteractionProviderProperty); }
			set { SetValue(InteractionProviderProperty, value); }
		}
		public InteractionProvider ActualInteractionProvider {
			get { return (InteractionProvider)GetValue(ActualInteractionProviderProperty); }
			private set { SetValue(ActualInteractionProviderPropertyKey, value); }
		}
		public bool DetachStreamOnLoadComplete {
			get { return (bool)GetValue(DetachStreamOnLoadCompleteProperty); }
			set { SetValue(DetachStreamOnLoadCompleteProperty, value); }
		}
		public CursorModeType CursorMode {
			get { return (CursorModeType)GetValue(CursorModeProperty); }
			set { SetValue(CursorModeProperty, value); }
		}
		public int CacheSize {
			get { return (int)GetValue(CacheSizeProperty); }
			set { SetValue(CacheSizeProperty, value); }
		}
		public bool AllowCachePages {
			get { return (bool)GetValue(AllowCachePagesProperty); }
			set { SetValue(AllowCachePagesProperty, value); }
		}
		public Color HighlightSelectionColor {
			get { return (Color)GetValue(HighlightSelectionColorProperty); }
			set { SetValue(HighlightSelectionColorProperty, value); }
		}
		public Color CaretColor {
			get { return (Color)GetValue(CaretColorProperty); }
			set { SetValue(CaretColorProperty, value); }
		}
		public Thickness PagePadding {
			get { return (Thickness)GetValue(PagePaddingProperty); }
			set { SetValue(PagePaddingProperty, value); }
		}
		public bool AllowCurrentPageHighlighting {
			get { return (bool)GetValue(AllowCurrentPageHighlightingProperty); }
			set { SetValue(AllowCurrentPageHighlightingProperty, value); }
		}
		public ObservableCollection<RecentFileViewModel> RecentFiles {
			get { return (ObservableCollection<RecentFileViewModel>)GetValue(RecentFilesProperty); }
			set { SetValue(RecentFilesProperty, value); }
		}
		public int NumberOfRecentFiles {
			get { return (int)GetValue(NumberOfRecentFilesProperty); }
			set { SetValue(NumberOfRecentFilesProperty, value); }
		}
		public bool ShowOpenFileOnStartScreen {
			get { return (bool)GetValue(ShowOpenFileOnStartScreenProperty); }
			set { SetValue(ShowOpenFileOnStartScreenProperty, value); }
		}
		public bool HasSelection {
			get { return (bool)GetValue(HasSelectionProperty); }
			internal set { SetValue(HasSelectionPropertyKey, value); }
		}
		public bool? ShowStartScreen {
			get { return (bool?)GetValue(ShowStartScreenProperty); }
			set { SetValue(ShowStartScreenProperty, value); }
		}
		public int MaxPrintingDpi {
			get { return (int)GetValue(MaxPrintingDpiProperty); }
			set { SetValue(MaxPrintingDpiProperty, value); }
		}
		public string DocumentCreator {
			get { return (string)GetValue(DocumentCreatorProperty); }
			set { SetValue(DocumentCreatorProperty, value); }
		}
		public string DocumentProducer {
			get { return (string)GetValue(DocumentProducerProperty); }
			set { SetValue(DocumentProducerProperty, value); }
		}
		public PdfPageLayout PageLayout {
			get { return (PdfPageLayout)GetValue(PageLayoutProperty); }
			set { SetValue(PageLayoutProperty, value); }
		}
		public PdfAttachmentsViewerSettings ActualAttachmentsViewerSettings {
			get { return (PdfAttachmentsViewerSettings)GetValue(ActualAttachmentsViewerSettingsProperty); }
			protected set { SetValue(ActualAttachmentsViewerSettingsPropertyKey, value); }
		}
		public PdfAttachmentsViewerSettings AttachmentsViewerSettings {
			get { return (PdfAttachmentsViewerSettings)GetValue(AttachmentsViewerSettingsProperty); }
			set { SetValue(AttachmentsViewerSettingsProperty, value); }
		}
		public ICommand PrintDocumentCommand { get; private set; }
		public ICommand ShowPropertiesCommand { get; private set; }
		public ICommand SetCursorModeCommand { get; private set; }
		public ICommand SelectionCommand { get; private set; }
		public ICommand OpenRecentDocumentCommand { get; private set; }
		public ICommand SelectAllCommand { get; private set; }
		public ICommand UnselectAllCommand { get; private set; }
		public ICommand SaveAsCommand { get; private set; }
		public ICommand CopyCommand { get; private set; }
		public ICommand OpenDocumentFromWebCommand { get; private set; }
		public ICommand ImportFormDataCommand { get; private set; }
		public ICommand ExportFormDataCommand { get; private set; }
		public ICommand SetPageLayoutCommand { get; private set; }
		public ICommand ShowCoverPageCommand { get; private set; }
		public ICommand OpenAttachmentCommand { get; private set; }
		public ICommand SaveAttachmentCommand { get; private set; }
		public event RoutedEventHandler CursorModeChanged {
			add { AddHandler(CursorModeChangedEvent, value); }
			remove { RemoveHandler(CursorModeChangedEvent, value); }
		}
		public event RoutedEventHandler PageLayoutChanged {
			add { AddHandler(PageLayoutChangedEvent, value); }
			remove { RemoveHandler(PageLayoutChangedEvent, value); }
		}
		public event GetDocumentPasswordEventHandler GetDocumentPassword {
			add { AddHandler(GetDocumentPasswordEvent, value); }
			remove { RemoveHandler(GetDocumentPasswordEvent, value); }
		}
		public event ReferencedDocumentOpeningEventHandler ReferencedDocumentOpening {
			add { AddHandler(ReferencedDocumentOpeningEvent, value); }
			remove { RemoveHandler(ReferencedDocumentOpeningEvent, value); }
		}
		public event UriOpeningEventHandler UriOpening {
			add { AddHandler(UriOpeningEvent, value); }
			remove { RemoveHandler(UriOpeningEvent, value); }
		}
		public event RoutedEventHandler DocumentLoaded {
			add { AddHandler(DocumentLoadedEvent, value); }
			remove { RemoveHandler(DocumentLoadedEvent, value); }
		}
		public event SelectionEventHandler SelectionStarted {
			add { AddHandler(SelectionStartedEvent, value); }
			remove { RemoveHandler(SelectionStartedEvent, value); }
		}
		public event SelectionEventHandler SelectionEnded {
			add { AddHandler(SelectionEndedEvent, value); }
			remove { RemoveHandler(SelectionEndedEvent, value); }
		}
		public event SelectionEventHandler SelectionContinued {
			add { AddHandler(SelectionContinuedEvent, value); }
			remove { RemoveHandler(SelectionContinuedEvent, value); }
		}
		public event DocumentClosingEventHandler DocumentClosing {
			add { AddHandler(DocumentClosingEvent, value); }
			remove { RemoveHandler(DocumentClosingEvent, value); }
		}
		public event AttachmentOpeningEventHandler AttachmentOpening {
			add { AddHandler(AttachmentOpeningEvent, value); }
			remove { RemoveHandler(AttachmentOpeningEvent, value); }
		}
		public new IPdfDocument Document {
			get { return (IPdfDocument)GetValue(DocumentProperty); }
		}
		IPdfViewerController ViewerController {
			get { return ActualInteractionProvider; }
		}
		new PdfBehaviorProvider ActualBehaviorProvider {
			get { return (PdfBehaviorProvider)base.ActualBehaviorProvider; }
		}
		new PdfOutlinesViewerSettings ActualDocumentMapSettings {
			get { return base.ActualDocumentMapSettings as PdfOutlinesViewerSettings; }
			set { base.ActualDocumentMapSettings = value; }
		}
		internal new PdfPresenterControl DocumentPresenter { get { return base.DocumentPresenter as PdfPresenterControl; } }
		internal bool IsShowCoverPage { get { return PageLayout == PdfPageLayout.TwoColumnRight || PageLayout == PdfPageLayout.TwoPageRight; } }
		readonly Locker documentClosingLocker = new Locker();
		public PdfViewerControl() {
			DefaultStyleKey = typeof(PdfViewerControl);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			ActualInteractionProvider = CreateInteractionProvider();
			ActualAttachmentsViewerSettings = CreateDefaultAttachmentsViewerSettings();
		}
		protected override void InitializeCommands() {
			base.InitializeCommands();
			PrintDocumentCommand = DelegateCommandFactory.Create(Print, CanPrint);
			ShowPropertiesCommand = DelegateCommandFactory.Create(ShowProperties, CanShowProperties);
			SetCursorModeCommand = DelegateCommandFactory.Create<CursorModeType>(SetCursorMode, CanSetCursorMode);
			SelectionCommand = DelegateCommandFactory.Create<PdfSelectionCommand>(Select, CanSelect);
			OpenRecentDocumentCommand = DelegateCommandFactory.Create<object>(OpenRecentDocument, CanOpenRecentDocument);
			SelectAllCommand = DelegateCommandFactory.Create(SelectAll, CanSelectAll);
			UnselectAllCommand = DelegateCommandFactory.Create(UnselectAll, CanUnselectAll);
			SaveAsCommand = DelegateCommandFactory.Create(SaveAs, CanSaveAs);
			CopyCommand = DelegateCommandFactory.Create(Copy, CanCopy);
			OpenDocumentFromWebCommand = DelegateCommandFactory.Create(OpenDocumentFromWeb);
			ImportFormDataCommand = DelegateCommandFactory.Create(ImportFormData, CanImportFormData);
			ExportFormDataCommand = DelegateCommandFactory.Create(ExportFormData, CanExportFormData);
			SetPageLayoutCommand = DelegateCommandFactory.Create<PdfPageLayout>(SetPageLayout, CanSetPageLayout);
			ShowCoverPageCommand = DelegateCommandFactory.Create(ShowCoverPage, CanShowCoverPage);
			OpenAttachmentCommand = DelegateCommandFactory.Create<object>(OpenAttachment, CanOpenAttachment);
			SaveAttachmentCommand = DelegateCommandFactory.Create<object>(SaveAttachment, CanSaveAttachment);
		}
		protected virtual InteractionProvider CreateInteractionProvider() {
			return new InteractionProvider();
		}
		protected override BehaviorProvider CreateBehaviorProvider() {
			return new PdfBehaviorProvider();
		}
		protected override CommandProvider CreateCommandProvider() {
			return new PdfCommandProvider();
		}
		protected override PropertyProvider CreatePropertyProvider() {
			return new PdfPropertyProvider();
		}
		protected override DocumentMapSettings CreateDefaultDocumentMapSettings() {
			return new PdfOutlinesViewerSettings();
		}
		protected virtual PdfAttachmentsViewerSettings CreateDefaultAttachmentsViewerSettings() {
			return new PdfAttachmentsViewerSettings();
		}
		protected virtual void ActualAttachmentsViewerSettingsChanged(PdfAttachmentsViewerSettings oldValue, PdfAttachmentsViewerSettings newValue) {
			oldValue.Do(x => x.Release());
			newValue.Do(x => x.Initialize(this));
		}
		protected virtual void OpenAttachment(object parameter) {
			var fileAttachment = parameter as PdfAttachmentViewModel;
			var documentViewModel = Document as PdfDocumentViewModel;
			documentViewModel.With(x => x.DocumentStateController).Do(x => x.OpenFileAttachment(fileAttachment.With(y => y.FileAttachment)));
		}
		protected virtual bool CanOpenAttachment(object parameter) {
			return parameter != null;
		}
		protected virtual void SaveAttachment(object parameter) {
			var fileAttachment = parameter as PdfAttachmentViewModel;
			var documentViewModel = Document as PdfDocumentViewModel;
			documentViewModel.With(x => x.DocumentStateController).Do(x => x.SaveFileAttachment(fileAttachment.With(y => y.FileAttachment)));
		}
		protected virtual bool CanSaveAttachment(object parameter) {
			return parameter != null;
		}
		protected override void ExecuteNavigate(object parameter) {
			base.ExecuteNavigate(parameter);
			var target = parameter as PdfTarget;
			if (target != null) {
				Navigate(target);
				return;
			}
			var outline = parameter as PdfOutlineViewerNode;
			if (outline != null) {
				Navigate(outline);
				return;
			}
		}
		protected virtual void OnPageLayoutChanged(PdfPageLayout newValue) {
			AssignDocumentPresenterProperties();
			RaiseEvent(new RoutedEventArgs(PageLayoutChangedEvent));
		}
		protected virtual void ActualInteractionProviderChanged(InteractionProvider oldValue, InteractionProvider newValue) {
			oldValue.Do(x => x.DocumentViewer = null);
			newValue.Do(x => x.DocumentViewer = this);
		}
		protected override IDocument CreateDocument(object source) {
			var document = new PdfDocumentViewModel(ViewerController);
			document.RequestPassword += OnDocumentRequestPassword;
			document.DocumentProgressChanged += OnDocumentProgressChanged;
			return document;
		}
		protected override void LoadDocument(object source) {
			base.LoadDocument(source);
			var document = (PdfDocumentViewModel)Document;
			if (AsyncDocumentLoad) {
				SplashScreenService.Do(x => x.ShowSplashScreen());
				document.LoadDocument(source, DetachStreamOnLoadComplete);
			}
			else
				document.LoadDocumentSync(source, DetachStreamOnLoadComplete);
		}
		protected override void CloseDocument() {
			base.CloseDocument();
			HasOutlines = false;
			HasAttachments = false;
		}
		void OnDocumentProgressChanged(object sender, DocumentProgressChangedEventArgs args) {
			var documentViewModel = (sender as PdfDocumentViewModel);
			if (documentViewModel == null)
				return;
			if (documentViewModel.IsLoadingFailed && !documentViewModel.IsCancelled) {
				SplashScreenService.Do(x => x.HideSplashScreen());
				DXMessageBoxHelper.Show(this, string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageLoadingError), documentViewModel.FilePath), PdfViewerLocalizer.GetString(PdfViewerStringId.MessageErrorCaption), MessageBoxButton.OK, MessageBoxImage.Error);
				CloseDocument();
				return;
			}
			if (documentViewModel.IsCancelled) {
				SplashScreenService.Do(x => x.HideSplashScreen());
				documentClosingLocker.DoIfNotLocked(CloseDocument);
				return;
			}
			if (args.IsCompleted) {
				OnDocumentLoadedInternal(documentViewModel);
				return;
			}
			SplashScreenService.Do(x => x.SetSplashScreenProgress(args.Progress, args.TotalProgress));
		}
		void OnDocumentLoadedInternal(PdfDocumentViewModel model) {
			SplashScreenService.Do(x => x.HideSplashScreen());
			PageCount = model.With(x => x.Pages).Return(x => x.Count(), () => PageCount);
			HasOutlines = CalcHasOutlines(model);
			HasAttachments = CalcHasAttachments(model);
			ActualDocumentMapSettings.RaiseInvalidate();
			ActualAttachmentsViewerSettings.RaiseInvalidate();
			((PdfPropertyProvider)PropertyProvider).IsFormDataPageVisible = CanExportFormData();
			CommandManager.InvalidateRequerySuggested();
			RaiseEvent(new RoutedEventArgs(DocumentLoadedEvent));
			OnDocumentLoaded();
		}
		bool CalcHasAttachments(IPdfDocument model) {
			return !ActualAttachmentsViewerSettings.HideAttachmentsViewer && model.If(x => x.HasAttachments).ReturnSuccess();
		}
		bool CalcHasOutlines(IPdfDocument model) {
			return !ActualDocumentMapSettings.HideOutlinesViewer && model.If(x => x.HasOutlines).ReturnSuccess();
		}
		void OnCacheSizeChanged(int newValue) {
			AssignDocumentPresenterProperties();
		}
		protected internal bool RaiseRequestOpeningReferencedDocumentSource(string documentPath, bool openInNewWindow, PdfTarget target) {
			var e = new ReferencedDocumentOpeningEventArgs(documentPath, openInNewWindow);
			RaiseEvent(e);
			return !e.Handled || !e.Cancel;
		}
		protected virtual void OnDocumentLoaded() {
		}
		protected override string GetOpenFileFilter() {
			return PdfViewerLocalizer.GetString(PdfViewerStringId.PdfFileFilter);
		}
		protected virtual bool CanSetCursorMode(CursorModeType cursorMode) {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void SetCursorMode(CursorModeType cursorMode) {
			CursorMode = cursorMode;
		}
		protected virtual bool CanSaveAs() {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void SaveAs() {
			string fileName = GetDocumentName(DocumentSource);
			try {
				SaveDocumentInternal(fileName);
			}
			catch {
				DXMessageBoxHelper.Show(this, string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSaveAsError), fileName), PdfViewerLocalizer.GetString(PdfViewerStringId.MessageErrorCaption), MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		protected virtual void SetPageLayout(PdfPageLayout pageLayout) {
			if ((pageLayout == PdfPageLayout.TwoColumnLeft || pageLayout == PdfPageLayout.TwoPageLeft) && IsShowCoverPage)
				PageLayout = pageLayout == PdfPageLayout.TwoPageLeft ? PdfPageLayout.TwoPageRight : PdfPageLayout.TwoColumnRight;
			else
				PageLayout = pageLayout;
		}
		protected virtual bool CanSetPageLayout(PdfPageLayout pageLayout) {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void ShowCoverPage() {
			switch (PageLayout) {
				case PdfPageLayout.TwoColumnLeft:
					PageLayout = PdfPageLayout.TwoColumnRight;
					break;
				case PdfPageLayout.TwoColumnRight:
					PageLayout = PdfPageLayout.TwoColumnLeft;
					break;
				case PdfPageLayout.TwoPageLeft:
					PageLayout = PdfPageLayout.TwoPageRight;
					break;
				case PdfPageLayout.TwoPageRight:
					PageLayout = PdfPageLayout.TwoPageLeft;
					break;
			}
		}
		protected virtual bool CanShowCoverPage() {
			return Document.Return(x => x.IsLoaded, () => false) && !(PageLayout == PdfPageLayout.OneColumn || PageLayout == PdfPageLayout.SinglePage);
		}
		protected internal ISplashScreenService SplashScreenService { get { return ((ISupportServices)ActualCommandProvider).ServiceContainer.GetService<ISplashScreenService>(); } }
		void SaveDocumentInternal(string fileName) {
			AssignableServiceHelper2<PdfViewerControl, SaveFileDialogService>.DoServiceAction(this, SaveFileDialogTemplate.Return(TemplateHelper.LoadFromTemplate<SaveFileDialogService>, CreateDefaultSaveFileDialogService), service => {
				var saveFileDialog = service as ISaveFileDialogService;
				if ((saveFileDialog as SaveFileDialogService).Return(x => !x.IsPropertySet(SaveFileDialogService.DefaultFileNameProperty), () => false))
					saveFileDialog.Do(x => x.DefaultFileName = fileName);
				if (!saveFileDialog.Return(x => x.ShowDialog(), () => false))
					return;
				var file = saveFileDialog.File;
				string newFileName = Path.Combine(file.DirectoryName, file.Name);
				SaveDocument(newFileName);
			});
		}
		void SaveDocument(Stream stream, PdfSaveOptions options) {
			CheckOperationAvailability();
			SaveDocument(stream, String.Empty, options);
			DocumentSource = stream;
		}
		void SaveDocument(string path, PdfSaveOptions options) {
			var documentViewModel = Document as PdfDocumentViewModel;
			var document = documentViewModel.PdfDocument;
			string tmpFilePath = null;
			try {
				tmpFilePath = Path.GetTempFileName();
				new FileInfo(tmpFilePath).Attributes = FileAttributes.Temporary;
				if (!String.IsNullOrEmpty(path)) {
					using (new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
						using (FileStream stream = new FileStream(tmpFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
							SaveDocument(stream, path, options);
					}
					File.Copy(tmpFilePath, path, true);
					DocumentSource = path;
				}
			}
			finally {
				if (tmpFilePath != null)
					try {
						File.Delete(tmpFilePath);
					}
					catch {
					}
			}
		}
		void SaveDocument(Stream stream, string path, PdfSaveOptions options) {
			var documentViewModel = Document as PdfDocumentViewModel;
			var document = documentViewModel.PdfDocument;
			document.Creator = DocumentCreator;
			document.Producer = DocumentProducer;
			DoWithDocumentProgress(() => PdfDocumentWriter.Write(stream, document, options), path, PdfViewerLocalizer.GetString(PdfViewerStringId.SavingDocumentCaption));
		}
		void DoWithDocumentProgress(Action action, string message, string title) {
			var documentViewModel = Document as PdfDocumentViewModel;
			PdfProgressChangedEventHandler handler = (d, e) => SplashScreenService.SetSplashScreenProgress(e.ProgressValue, 100);
			try {
				documentViewModel.With(x => x.PdfDocument).Do(x => x.ProgressChanged += handler);
				SplashScreenService.ShowSplashScreen();
				action();
			}
			finally {
				documentViewModel.With(x => x.PdfDocument).Do(x => x.ProgressChanged -= handler);
				SplashScreenService.HideSplashScreen();
			}
		}
		protected virtual void Copy() {
			if (Document.With(x => x.SelectionResults) == null)
				return;
			if (Document.SelectionResults.ContentType == PdfDocumentContentType.Text) {
				Clipboard.SetText(Document.SelectionResults.Text);
			}
			else if (Document.SelectionResults.ContentType == PdfDocumentContentType.Image) {
				var image = Document.SelectionResults.GetImage(90 * (int)PageRotation);
				if (image != null)
					Clipboard.SetImage(image);
			}
		}
		protected virtual bool CanCopy() {
			return HasSelection;
		}
		protected virtual void OpenDocumentFromWeb() {
			DXDialogWindow window = new DXDialogWindow(PdfViewerLocalizer.GetString(PdfViewerStringId.OpenDocumentFromWebCaption), MessageBoxButton.OKCancel) {
				SizeToContent = SizeToContent.WidthAndHeight,
				Owner = LayoutHelper.GetTopLevelVisual(this) as Window,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ResizeMode = ResizeMode.NoResize,
				ShowIcon = false
			};
			var editor = new AddressControl();
			var viewModel = new AddressViewModel();
			editor.DataContext = viewModel;
			window.Content = editor;
			ThemeManager.SetThemeName(window, ThemeHelper.GetEditorThemeName(this));
			var result = window.ShowDialog();
			if (result == true) {
				Uri resultUri;
				if (Uri.TryCreate(viewModel.Url, UriKind.Absolute, out resultUri))
					DocumentSource = resultUri;
				else
					DXMessageBoxHelper.Show(this, string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageIncorrectUrl), viewModel.Url), PdfViewerLocalizer.GetString(PdfViewerStringId.MessageErrorCaption), MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		protected virtual bool CanNavigate(PdfTarget target) {
			return target != null && Document.Return(x => x.IsLoaded, () => false);
		}
		public virtual void Navigate(PdfTarget target) {
			if (!CanNavigate(target))
				return;
			DocumentPresenter.Do(x => x.ScrollIntoView(target));
		}
		protected virtual bool CanNavigate(PdfOutlineViewerNode node) {
			return node != null && Document.Return(x => x.IsLoaded, () => false);
		}
		public virtual void Navigate(PdfOutlineViewerNode node) {
			if (!CanNavigate(node))
				return;
			Document.NavigateToOutline(node);
			if (ActualDocumentMapSettings.ActualHideAfterUse)
				ActualDocumentMapSettings.OutlinesViewerState = PdfOutlinesViewerState.Collapsed;
		}
		bool CanImportFormData() {
			return Document.Return(x => x.IsLoaded && x.HasInteractiveForm && !IsReadOnly, () => false);
		}
		bool CanExportFormData() {
			return Document.Return(x => x.IsLoaded && x.HasInteractiveForm, () => false);
		}
		#region API
		public void ImportFormData() {
			if (!CanImportFormData())
				return;
			ActualInteractionProvider.ValueEditingController.CommitEditor();
			using (OpenFileDialog openDialog = new OpenFileDialog()) {
				openDialog.Filter = PdfViewerLocalizer.GetString(PdfViewerStringId.FormDataFileFilter);
				openDialog.RestoreDirectory = true;
				if (openDialog.ShowDialog() == DialogResult.OK) {
					string path = openDialog.FileName;
					bool isXml = path.EndsWith("xml", true, CultureInfo.InvariantCulture);
					if (!string.IsNullOrEmpty(path)) {
						try {
							PdfFormData data = new PdfFormData(path);
							((PdfDocumentViewModel)Document).ApplyFormData(data);
						}
						catch {
							string messageText = string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageImportError), path, isXml ? "XML" : "FDF");
							DXMessageBoxHelper.Show(this, messageText, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageErrorCaption), MessageBoxButton.OK, MessageBoxImage.Warning);
						}
					}
				}
			}
		}
		public void ExportFormData() {
			if (!CanExportFormData())
				return;
			ActualInteractionProvider.ValueEditingController.CommitEditor();
			using (SaveFileDialog saveDialog = new SaveFileDialog()) {
				saveDialog.Filter = PdfViewerLocalizer.GetString(PdfViewerStringId.FormDataFileFilter);
				saveDialog.RestoreDirectory = true;
				string path = (Document as PdfDocumentViewModel).With(x => x.FilePath);
				if (!string.IsNullOrEmpty(path))
					saveDialog.FileName = Path.GetFileNameWithoutExtension(path);
				if (saveDialog.ShowDialog() == DialogResult.OK) {
					try {
						((PdfDocumentViewModel)Document).SaveFormData(saveDialog.FileName, saveDialog.FileName.EndsWith("xml") ? PdfFormDataFormat.Xml : PdfFormDataFormat.Fdf);
					}
					catch {
						string messageText = string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageExportError), saveDialog.FileName);
						DXMessageBoxHelper.Show(this, messageText, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageErrorCaption), MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
			}
		}
		public void SaveDocument(Stream stream) {
			SaveDocument(stream, new PdfSaveOptions());
		}
		public void SaveDocument(string path) {
			SaveDocument(path, new PdfSaveOptions());
		}
		protected virtual SaveFileDialogService CreateDefaultSaveFileDialogService() {
			return new SaveFileDialogService() {
				DefaultExt = PdfViewerLocalizer.GetString(PdfViewerStringId.PdfFileExtension),
				Filter = PdfViewerLocalizer.GetString(PdfViewerStringId.PdfFileFilter)
			};
		}
		public BitmapSource CreateBitmap(int index) {
			if (!IsDocumentContainPages())
				return null;
			var page = Document.Pages.ElementAt(index);
			return CreateBitmap(index, (int)Math.Max(page.PageSize.Width, page.PageSize.Height));
		}
		public BitmapSource CreateBitmap(int index, int largestEdgeLength) {
			if (IsDocumentContainPages())
				return Document.CreateBitmap(index, largestEdgeLength);
			return null;
		}
		[Obsolete("Use the Print(PdfPrinterSettings printerSettings, bool showPrintStatus = true, int maxPrintingDpi = 0) overload of this method instead.")]
		public virtual void Print(PrinterSettings printerSettings, bool showPrintStatus = true, int maxPrintingDpi = 0) {
			CheckOperationAvailability();
			if (printerSettings == null)
				printerSettings = new PrinterSettings() { PrintRange = PrintRange.AllPages };
			if (CanPrint())
				Document.Print(new PdfPrinterSettings(printerSettings), CurrentPageNumber, showPrintStatus, maxPrintingDpi);
		}
		public virtual void Print(PdfPrinterSettings printerSettings, bool showPrintStatus = true, int maxPrintingDpi = 0) {
			if (printerSettings == null)
				printerSettings = new PdfPrinterSettings();
			if (CanPrint())
				Document.Print(printerSettings, CurrentPageNumber, showPrintStatus, maxPrintingDpi);
		}
		public virtual void Print() {
			CheckOperationAvailability();
			PrintInternal();
		}
		protected internal void Print(IEnumerable<int> pages) {
			PrintInternal(pages);
		}
		void PrintInternal(IEnumerable<int> pages = null) {
			if (CanPrint()) {
				ActualInteractionProvider.ValueEditingController.CloseEditor();
				string title = PdfViewerLocalizer.GetString(PdfViewerStringId.PrintDialogTitle);
				var documentViewModel = (PdfDocumentViewModel)Document;
				var printCommand =
					new DelegateCommand<PdfPrintDialogViewModel>(
						viewModel => PdfDocumentPrinter.Print(documentViewModel.DocumentState, documentViewModel.FilePath, CurrentPageNumber, viewModel.PrinterSettings, true),
						viewModel => viewModel.EnableToPrint);
				DialogService service = TemplateHelper.LoadFromTemplate<DialogService>(PrintPreviewDialogTemplate) ?? CreateDefaultPrintPreviewDialogService();
				AssignableServiceHelper2<PdfViewerControl, DialogService>.DoServiceAction(this, service, x => {
					x.ShowDialog(
						printCommand,
						PdfViewerLocalizer.GetString(PdfViewerStringId.PrintDialogPrintButtonCaption),
						null,
						PdfViewerLocalizer.GetString(PdfViewerStringId.CancelButtonCaption),
						title,
						vm => new PrintDialogViewModel(CreatePrintViewModel(documentViewModel, title, UpdatePrinterSettings((PdfPrinterSettings)vm, pages))),
						() => new PdfPrinterSettings());
				});
			}
		}
		PdfPrinterSettings UpdatePrinterSettings(PdfPrinterSettings settings, IEnumerable<int> pages) {
			if (pages != null && pages.Any())
				settings.PageNumbers = pages.ToArray();
			return settings;
		}
		DialogService CreateDefaultPrintPreviewDialogService() {
			return new PdfDialogService() {
				DialogWindowStartupLocation = WindowStartupLocation.CenterScreen,
				ViewTemplate = (DataTemplate)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.PdfPrintEditorTemplate, ThemeName = ThemeHelper.GetEditorThemeName(this) }),
				DialogStyle = (Style)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.PdfPrintDialogStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) })
			};
		}
		PdfPrintDialogViewModel CreatePrintViewModel(PdfDocumentViewModel documentViewModel, string title, PdfPrinterSettings printerSettings) {
			return new PdfPrintDialogViewModel(documentViewModel.DocumentState, new Size(427, 518), CurrentPageNumber, MaxPrintingDpi, 
				msg => DXMessageBoxHelper.Show(this, msg, title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK, printerSettings);
		}
		public Point ConvertDocumentPositionToPixel(PdfDocumentPosition position) {
			return DocumentPresenter.Return(x => x.ConvertDocumentPositionToPoint(position), () => new Point(0, 0));
		}
		public PdfDocumentPosition ConvertPixelToDocumentPosition(Point point) {
			return DocumentPresenter.With(x => x.ConvertPointToDocumentPosition(point));
		}
		public string GetText(PdfDocumentPosition start, PdfDocumentPosition end) {
			CheckOperationAvailability();
			return Document.GetText(start, end);
		}
		public string GetText(PdfDocumentArea area) {
			CheckOperationAvailability();
			return Document.GetText(area);
		}
		public void SelectAll() {
			CheckOperationAvailability();
			Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectAllText });
		}
		public void UnselectAll() {
			CheckOperationAvailability();
			Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.ClearSelection });
		}
		public void Select(PdfDocumentArea area) {
			CheckOperationAvailability();
			Document.PerformSelection(new PdfDocumentSelectionParameter() { Area = area, SelectionAction = PdfSelectionAction.SelectViaArea });
		}
		public PdfHitTestResult HitTest(Point point) {
			CheckOperationAvailability();
			return DocumentPresenter.With(x => x.HitTest(point));
		}
		#endregion
		void OnThemeChanged(object sender, EventArgs e) {
			CurrentPageNumber = 1;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			ThemeManager.AddThemeChangedHandler(this, OnThemeChanged);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			ThemeManager.RemoveThemeChangedHandler(this, OnThemeChanged);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateStartScreenVisible();
			if (RecentFiles == null)
				RecentFiles = new ObservableCollection<RecentFileViewModel>();
		}
		bool CanOpenRecentDocument(object document) {
			return true;
		}
		void OpenRecentDocument(object document) {
			DocumentSource = document;
		}
		bool CanShowProperties() {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		void ShowProperties() {
			PdfDocumentProperties pdfDocumentPropertiesModel = (PdfDocumentProperties)Document.GetDocumentProperties();
			var page = (PdfPageViewModel)Document.Pages.ElementAtOrDefault(CurrentPageNumber - 1);
			if (page != null)
				pdfDocumentPropertiesModel.PageSize = String.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.PageSize), page.InchPageSize.Width, page.InchPageSize.Height);
			DXDialog dialog = new DXDialog(PdfViewerLocalizer.GetString(PdfViewerStringId.PropertiesCaption), DialogButtons.Ok) {
				Owner = LayoutHelper.GetTopLevelVisual(this) as Window,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				SizeToContent = SizeToContent.Height,
				ResizeMode = ResizeMode.NoResize,
				ShowIcon = false,
				Width = 470
			};
			ThemeManager.SetThemeName(dialog, ThemeHelper.GetEditorThemeName(this));
			dialog.Content = new PropertiesControl { DataContext = pdfDocumentPropertiesModel };
			dialog.ShowDialog();
		}
		bool CanPrint() {
			return Document.Return(x => x.IsLoaded, () => false) && Document.Pages.Any();
		}
		void CheckOperationAvailability() {
			if (!Document.Return(x => x.IsLoaded, () => false))
				throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
		}
		protected override void FindNextText(TextSearchParameter parameter) {
			var results = Document.PerformSearch(parameter);
			if (results.Status == PdfTextSearchStatus.Finished)
				DXMessageBoxHelper.Show(this, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSearchFinished), PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSearchCaption), MessageBoxButton.OK, MessageBoxImage.Information);
			else if (results.Status == PdfTextSearchStatus.NotFound)
				DXMessageBoxHelper.Show(this, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSearchFinishedNoMatchesFound), PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSearchCaption), MessageBoxButton.OK, MessageBoxImage.Information);
		}
		void Select(PdfSelectionCommand command) {
			switch (command) {
				case PdfSelectionCommand.MoveLeft:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveLeft });
					break;
				case PdfSelectionCommand.MoveUp:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveUp });
					break;
				case PdfSelectionCommand.MoveRight:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveRight });
					break;
				case PdfSelectionCommand.MoveDown:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveDown });
					break;
				case PdfSelectionCommand.MoveLineStart:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveLineStart });
					break;
				case PdfSelectionCommand.MoveLineEnd:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveLineEnd });
					break;
				case PdfSelectionCommand.MoveDocumentStart:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveDocumentStart });
					break;
				case PdfSelectionCommand.MoveDocumentEnd:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveDocumentEnd });
					break;
				case PdfSelectionCommand.MoveNextWord:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MoveNextWord });
					break;
				case PdfSelectionCommand.MovePreviousWord:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.MovePreviousWord });
					break;
				case PdfSelectionCommand.SelectLeft:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectLeft });
					break;
				case PdfSelectionCommand.SelectUp:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectUp });
					break;
				case PdfSelectionCommand.SelectRight:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectRight });
					break;
				case PdfSelectionCommand.SelectDown:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectDown });
					break;
				case PdfSelectionCommand.SelectLineStart:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectLineStart });
					break;
				case PdfSelectionCommand.SelectLineEnd:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectLineEnd });
					break;
				case PdfSelectionCommand.SelectDocumentStart:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectDocumentStart });
					break;
				case PdfSelectionCommand.SelectDocumentEnd:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectDocumentEnd });
					break;
				case PdfSelectionCommand.SelectNextWord:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectNextWord });
					break;
				case PdfSelectionCommand.SelectPreviousWord:
					Document.PerformSelection(new PdfDocumentSelectionParameter() { SelectionAction = PdfSelectionAction.SelectPreviousWord });
					break;
				default:
					throw new NotImplementedException();
			}
		}
		bool CanSelect(PdfSelectionCommand command) {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		bool CanSelectAll() {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		bool CanUnselectAll() {
			return Document.Return(x => x.IsLoaded, () => false);
		}
		protected virtual void OnShowStartScreenChanged(bool? newValue) {
			UpdateStartScreenVisible();
		}
		protected virtual void OnCursorModeChanged(CursorModeType cursorModeType) {
			RaiseEvent(new RoutedEventArgs(CursorModeChangedEvent));
		}
		protected override void AssignDocumentPresenterProperties() {
			base.AssignDocumentPresenterProperties();
			DocumentPresenter.Do(x => {
				x.AllowCachePages = AllowCachePages;
				x.CacheSize = CacheSize;
				switch (PageLayout) {
					case PdfPageLayout.OneColumn:
					case PdfPageLayout.SinglePage:
						x.PageDisplayMode = PageDisplayMode.Single;
						x.ColumnsCount = 1;
						x.ShowCoverPage = false;
						break;
					case PdfPageLayout.TwoColumnLeft:
					case PdfPageLayout.TwoPageLeft:
						x.PageDisplayMode = PageDisplayMode.Columns;
						x.ColumnsCount = 2;
						x.ShowCoverPage = false;
						break;
					case PdfPageLayout.TwoColumnRight:
					case PdfPageLayout.TwoPageRight:
						x.PageDisplayMode = PageDisplayMode.Columns;
						x.ColumnsCount = 2;
						x.ShowCoverPage = true;
						break;
				}
				x.ShowSingleItem = PageLayout == PdfPageLayout.TwoPageLeft || PageLayout == PdfPageLayout.TwoPageRight || PageLayout == PdfPageLayout.SinglePage;
			});
		}
		protected override void OnZoomFactorChanged(double oldValue, double newValue) {
			base.OnZoomFactorChanged(oldValue, newValue);
			DocumentPresenter.Do(x => x.Update());
		}
		protected override void ReleaseDocument(IDocument document) {
			(document as PdfDocumentViewModel).Do(x => x.RequestPassword -= OnDocumentRequestPassword);
			(document as PdfDocumentViewModel).Do(x => x.DocumentProgressChanged -= OnDocumentProgressChanged);
			(document as IDisposable).Do(x => x.Dispose());
		}
		protected override void OnDocumentSourceChanged(object oldValue, object newValue) {
			if (documentClosingLocker.IsLocked)
				return;
			if (oldValue == null) {
				base.OnDocumentSourceChanged(oldValue, newValue);
				return;
			}
			var documentClosingArgs = RaiseDocumentClosing();
			if (documentClosingArgs.Cancel) {
				documentClosingLocker.DoLockedAction(() => DocumentSource = oldValue);
				return;
			}
			var documentViewModel = Document as PdfDocumentViewModel;
			if (documentViewModel.Return(x => x.IsLoaded && x.DocumentStateController.IsDocumentModified, () => false)) {
				var saveDialogResult = documentClosingArgs.SaveDialogResult;
				if (!saveDialogResult.HasValue)
					saveDialogResult = DXMessageBoxHelper.Show(this, PdfViewerLocalizer.GetString(PdfViewerStringId.SaveChangesMessage), PdfViewerLocalizer.GetString(PdfViewerStringId.SaveChangesCaption), MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
				if (saveDialogResult.Value == MessageBoxResult.Yes) {
					string fileName = GetDocumentName(oldValue);
					SaveDocumentInternal(fileName);
				}
				else if (saveDialogResult.Value == MessageBoxResult.Cancel) {
					documentClosingLocker.DoLockedAction(() => DocumentSource = oldValue);
					return;
				}
			}
			if (!documentViewModel.Return(x => x.IsLoaded, () => true))
				documentClosingLocker.DoLockedAction(documentViewModel.CancelLoadDocument);
			base.OnDocumentSourceChanged(oldValue, newValue);
		}
		DocumentClosingEventArgs RaiseDocumentClosing() {
			var args = new DocumentClosingEventArgs(DocumentClosingEvent);
			RaiseEvent(args);
			return args;
		}
		SecureString GetPassword(string path, int tryNumber) {
			const int maxDocumentNameLength = 25;
			string documentName = String.IsNullOrEmpty(path) ? PdfCoreLocalizer.GetString(PdfCoreStringId.DefaultDocumentName) : Path.GetFileName(path);
			if (documentName.Length > maxDocumentNameLength)
				documentName = documentName.Substring(0, maxDocumentNameLength) + "...";
			if (tryNumber > 1)
				DXMessageBoxHelper.Show(this, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageIncorrectPassword), documentName, MessageBoxButton.OK, MessageBoxImage.Information);
			var eventArgs = new GetDocumentPasswordEventArgs(path);
			RaiseEvent(eventArgs);
			if (eventArgs.Handled)
				return eventArgs.Password;
			var passwordModel = new PasswordViewModel { Password = null };
			ShowGetPasswordDialog(passwordModel, documentName);
			return passwordModel.Password;
		}
		void ShowGetPasswordDialog(PasswordViewModel passwordModel, string documentName) {
			DXDialog dialog = new DXDialog(String.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageDocumentIsProtected), documentName)) {
				Owner = LayoutHelper.GetTopLevelVisual(this) as Window,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				SizeToContent = SizeToContent.WidthAndHeight,
				ResizeMode = ResizeMode.NoResize
			};
			ThemeManager.SetThemeName(dialog, ThemeHelper.GetEditorThemeName(this));
			dialog.Content = new PasswordControl { DataContext = passwordModel };
			if (dialog.ShowDialog() != true)
				(Document as PdfDocumentViewModel).Do(x => x.CancelLoadDocument());
		}
		protected override void OnDocumentChanged(IDocument oldValue, IDocument newValue) {
			base.OnDocumentChanged(oldValue, newValue);
			UpdateStartScreenVisible();
			CommandManager.InvalidateRequerySuggested();
		}
		protected override void OnCommandProviderChanged(CommandProvider oldValue, CommandProvider newValue) {
			if (newValue.GetType() != typeof(PdfCommandProvider))
				CommandBarStyle = CommandBarStyle.None;
		}
		void OnDocumentRequestPassword(object sender, RequestPasswordEventArgs args) {
			args.Password = GetPassword(args.Path, args.TryNumber);
			args.Handled = true;
		}
		void UpdateStartScreenVisible() {
			((PdfPropertyProvider)PropertyProvider).IsStartScreenVisible = Document == null && (ShowStartScreen.HasValue ? ShowStartScreen.Value : CommandBarStyle == CommandBarStyle.None);
		}
		string GetDocumentName(object documentSource) {
			if (documentSource is string) {
				return Path.GetFileName((string)documentSource);
			}
			if (documentSource is Uri) {
				Uri documentSourceUri = documentSource as Uri;
				return documentSourceUri.AbsolutePath.Split('/').Last();
			}
			if (documentSource is FileStream) {
				FileStream documentFileStream = documentSource as FileStream;
				return Path.GetFileName(documentFileStream.Name);
			}
			else
				return string.Empty;
		}
		void AddDocumentToRecentFiles(object documentSource) {
			var fileName = GetDocumentName(documentSource);
			if (string.IsNullOrEmpty(fileName))
				return;
			RecentFileViewModel recentFile = new RecentFileViewModel {
				Name = fileName,
				DocumentSource = documentSource is FileStream ? ((FileStream)documentSource).Name : documentSource,
				Command = OpenRecentDocumentCommand
			};
			if (RecentFiles == null)
				RecentFiles = new ObservableCollection<RecentFileViewModel>();
			if (RecentFiles.Contains(recentFile)) {
				RecentFiles.Remove(recentFile);
			}
			RecentFiles.Add(recentFile);
		}
		protected void AllowCachePagesChanged(bool newValue) {
			AssignDocumentPresenterProperties();
		}
		protected internal virtual void UpdateSelection() {
			HasSelection = Document.HasSelection;
		}
		protected internal bool AllowAccessToPublicHyperlink(Uri uri) {
			var e = new UriOpeningEventArgs(uri);
			RaiseEvent(e);
			if (e.Handled)
				return !e.Cancel;
			string message = string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSecurityWarningUriOpening), uri.AbsoluteUri);
			var result = DXMessageBoxHelper.Show(this, message, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSecurityWarningCaption), MessageBoxButton.YesNo, MessageBoxImage.Information);
			return result == MessageBoxResult.Yes;
		}
		PdfDocumentProcessorHelper IPdfViewer.GetDocumentProcessorHelper() {
			if (Document != null && ((PdfDocumentViewModel)Document).DocumentState == null)
				return null;
			var formData = ((PdfDocumentViewModel)Document).With(x => x.GetFormData());
			return new PdfDocumentProcessorHelper(SaveDocument, SaveDocument, formData);
		}
		protected virtual void OutlinesViewerSettingsChanged(PdfOutlinesViewerSettings oldValue, PdfOutlinesViewerSettings newValue) {
			ActualDocumentMapSettings = newValue ?? (PdfOutlinesViewerSettings)CreateDefaultDocumentMapSettings();
		}
		protected internal virtual void UpdateHasOutlines(bool newValue) {
			HasOutlines = CalcHasOutlines(Document);
		}
		protected virtual void AttachmentsViewerSettingsChanged(PdfAttachmentsViewerSettings oldValue, PdfAttachmentsViewerSettings newValue) {
			ActualAttachmentsViewerSettings = newValue ?? CreateDefaultAttachmentsViewerSettings();
		}
		protected internal virtual void UpdateHasAttachments(bool newValue) {
			HasAttachments = CalcHasAttachments(Document);
		}
		protected internal virtual string SaveAttachmentInternal(PdfFileAttachment attachment) {
			if (ActualAttachmentsViewerSettings.ActualHideAfterUse)
				ActualAttachmentsViewerSettings.AttachmentsViewerState = PdfAttachmentsViewerState.Collapsed;
			using (SaveFileDialog saveDialog = new SaveFileDialog()) {
				saveDialog.FileName = attachment.FileName;
				if (saveDialog.ShowDialog() == DialogResult.OK)
					return saveDialog.FileName;
			}
			return null;
		}
		protected internal virtual bool OpenAttachmentInternal(PdfFileAttachment attachment) {
			var e = new AttachmentOpeningEventArgs(attachment);
			RaiseEvent(e);
			if (e.Handled)
				return !e.Cancel; 
			if (ActualAttachmentsViewerSettings.ActualHideAfterUse)
				ActualAttachmentsViewerSettings.AttachmentsViewerState = PdfAttachmentsViewerState.Collapsed;
			string message = string.Format(PdfViewerLocalizer.GetString(PdfViewerStringId.MessageFileAttachmentOpening), attachment.FileName);
			var result = DXMessageBoxHelper.Show(this, message, PdfViewerLocalizer.GetString(PdfViewerStringId.MessageSecurityWarningCaption), MessageBoxButton.YesNo, MessageBoxImage.Information);
			return result == MessageBoxResult.Yes;
		}
	}
	class PasswordViewModel {
		public SecureString Password { get; set; }
	}
	class AddressViewModel {
		public string Url { get; set; }
	}
	public class DocumentLoadingViewModel : BindableBase {
		long currentProgress;
		long totalProgress;
		string message;
		public long CurrentProgress {
			get { return currentProgress; }
			set { SetProperty(ref currentProgress, value, () => CurrentProgress); }
		}
		public long TotalProgress {
			get { return totalProgress; }
			set { SetProperty(ref totalProgress, value, () => TotalProgress); }
		}
		public string Message {
			get { return message; }
			set { SetProperty(ref message, value, () => Message); }
		}
		public DocumentLoadingViewModel() {
			TotalProgress = 1;
			CurrentProgress = 0;
		}
	}
	public class SecureStringToStringConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string inputValue = value as string;
			if (string.IsNullOrEmpty(inputValue))
				return new SecureString();
			SecureString secureString = new SecureString();
			foreach (char chr in inputValue)
				secureString.AppendChar(chr);
			return secureString;
		}
	}
	public class PdfDialogService : DialogService {
		public static readonly DependencyProperty ContentProperty;
		static PdfDialogService() {
			ContentProperty = DependencyPropertyRegistrator.Register<PdfDialogService, object>(owner => owner.Content, null);
		}
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
	public class PdfPropertyProvider : PropertyProvider {
		bool isFormDataPageVisible;
		bool isStartScreenVisible;
		public bool IsFormDataPageVisible {
			get { return isFormDataPageVisible; }
			protected internal set { SetProperty(ref isFormDataPageVisible, value, () => IsFormDataPageVisible); }
		}
		public bool IsStartScreenVisible {
			get { return isStartScreenVisible; }
			protected internal set { SetProperty(ref isStartScreenVisible, value, () => IsStartScreenVisible); }
		}
	}
}
