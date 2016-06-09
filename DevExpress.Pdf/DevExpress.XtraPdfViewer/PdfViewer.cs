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
using System.IO;
using System.Security;
using System.Reflection;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.About;
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraPdfViewer.Commands;
using DevExpress.XtraPdfViewer.Controls;
using DevExpress.XtraPdfViewer.Bars;
using DevExpress.XtraPdfViewer.Forms;
using DevExpress.XtraPdfViewer.Native;
using DevExpress.XtraPdfViewer.Interop;
using DevExpress.XtraPdfViewer.Localization;
namespace DevExpress.XtraPdfViewer {
	public enum PdfContentMarginMode { Dynamic, Static };
	public enum PdfNavigationPaneVisibility { Default, Hidden, Collapsed, Visible, Expanded };
	[
#if !DEBUG
#endif // DEBUG
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabCommon),
	Description("Displays PDF files in WinForms applications without the need to install any third-party software on end user machines."),
	Designer("DevExpress.XtraPdfViewer.Design.PdfViewerDesigner," + AssemblyInfo.SRAssemblyXtraPdfViewerDesign),
	TypeConverter("DevExpress.XtraPdfViewer.Design.PdfViewerTypeConverter," + AssemblyInfo.SRAssemblyXtraPdfViewerDesign),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "PdfViewer.bmp")
	]
	public class PdfViewer : XtraUserControl, ICommandAwareControl<PdfViewerCommandId>, IServiceProvider, IPdfViewer {
		const long defaultImageCacheSize = 300;
		const int defaultContentMinMargin = 10;
		const float caretVisibilityMargin = 100;
		const int maximumMessageBoxTextLength = 100;
		internal const PdfZoomMode DefaultZoomMode = PdfZoomMode.ActualSize;
		static readonly Color defaultHighlightedFormFieldColor = Color.FromArgb(204, 215, 255);
		static readonly object passwordRequested = new object();
		static readonly object documentClosing = new object();
		static readonly object documentChanged = new object();
		static readonly object currentPageChanged = new object();
		static readonly object zoomChanged = new object();
		static readonly object selectionStarted = new object();
		static readonly object selectionContinued = new object();
		static readonly object selectionEnded = new object();
		static readonly object findDialogVisibilityChanged = new object();
		static readonly object pageSetupDialogShowing = new object();
		static readonly object popupMenuShowing = new object();
		static readonly object scrollPositionChanged = new object();
		static readonly object referencedDocumentOpening = new object();
		static readonly object uriOpening = new object();
		static readonly object fileAttachmentOpening = new object();
		static readonly object navigationPaneVisibilityChanged = new object();
		static readonly object formFieldValueChanging = new object();
		static readonly object formFieldValueChanged = new object();
		static readonly Image outlineViewerImage = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraPdfViewer.Images.Bookmark.png"));
		static readonly Image attachmentViewerImage = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraPdfViewer.Images.Attachment.png"));
		static PdfViewer() {
		}
		static PdfFormDataFormat GetFormat(string fileName) {
			switch (Path.GetExtension(fileName)) {
				case ".xml":
					return PdfFormDataFormat.Xml;
				case ".fdf":
					return PdfFormDataFormat.Fdf;
				case ".xfdf":
					return PdfFormDataFormat.Xfdf;
				default:
					return PdfFormDataFormat.Txt;
			}
		}
		public static void About() {
			AboutHelper.Show(ProductKind.DXperienceWin, new ProductStringInfoWin(ProductInfoHelper.WinPdfViewer));
		}
		readonly Container components = new Container();
		readonly PopupMenu popupMenu = new PopupMenu();
		readonly PdfPageCache pageCache = new PdfPageCache();
		readonly PdfViewerClickController clickController = new PdfViewerClickController();
		readonly BarAndDockingController barAndDockingController = new BarAndDockingController();
		readonly PdfDocumentViewer viewer;
		readonly PdfViewerCommandRepository commands;
		readonly PdfFontStorage fontStorage = new PdfFontStorage();
		readonly PdfViewerController viewerController;
		readonly NavigationPane navigationPane = new NavigationPane();
		readonly NavigationPage outlineViewerNavigationPage = new NavigationPage();
		readonly NavigationPage attachmentsViewerNavigationPage = new NavigationPage();
		readonly PdfSplitter splitter;
		readonly PdfOutlineViewerControl outlineViewerControl;
		readonly PdfAttachmentsViewerControl attachmentsViewerControl;
		PdfNavigationPaneVisibility navigationPaneInitialVisibility;
		PdfNavigationPaneVisibility navigationPaneVisibility = PdfNavigationPaneVisibility.Hidden;
		string documentCreator = String.Empty;
		string documentProducer = String.Empty;
		int passwordAttemptsLimit = 1;
		PdfZoomMode zoomMode = DefaultZoomMode;
		int rotationAngle;
		bool highlightFormFields;
		Color highlightedFormFieldColor = defaultHighlightedFormFieldColor;
		PdfContentMarginMode contentMarginMode;
		int contentMinMargin = defaultContentMinMargin;
		bool detachStreamAfterLoadComplete;
		long imageCacheSize = defaultImageCacheSize;
		int maxPrintingDpi;
		bool showPrintStatusDialog = true;
		bool acceptsTab = true;
		IDXMenuManager menuManager;
		PdfDocument document;
		Stream documentStream;
		PdfDocumentState documentState;
		PdfDocumentStateController documentStateController;
		PdfDocumentProperties documentProperties;
		PdfOutlineViewerSettings outlineViewerSettings;
		string defaultDocumentDirectory = String.Empty;
		string documentFilePath = String.Empty;
		string documentFullFilePath = String.Empty;
		long fileSize;
		float actualZoom = 1;
		PdfFindToolWindow findToolWindow;
		Color selectionColor;
		bool cancelSaveOperation;
		bool readOnly;
		EventHandler onBeforeDispose;
		EventHandler onUpdateUI;
		PdfSelection Selection { get { return documentState == null ? null : documentState.SelectionState.Selection; } }
		Bitmap SelectedBitmap {
			get {
				PdfImageSelection imageSelection = Selection as PdfImageSelection;
				return imageSelection == null ? null :
					PdfImageSelectionCommandInterpreter.GetSelectionBitmap(document.Pages[imageSelection.Highlights[0].PageIndex], imageSelection,
						documentState.ImageDataStorage, rotationAngle, PdfRenderingCommandInterpreter.DefaultDpi);
			}
		}
		PdfModifierKeys ActualModifierKeys {
			get {
				Keys modifierKeys = ModifierKeys;
				PdfModifierKeys modifiers = PdfModifierKeys.None;
				if (modifierKeys.HasFlag(Keys.Control))
					modifiers |= PdfModifierKeys.Control;
				if (modifierKeys.HasFlag(Keys.Shift))
					modifiers |= PdfModifierKeys.Shift;
				if (modifierKeys.HasFlag(Keys.Alt))
					modifiers |= PdfModifierKeys.Alt;
				return modifiers;
			}
		}
		PdfFindToolWindow FindToolWindow {
			get {
				if (findToolWindow == null) {
					findToolWindow = new PdfFindToolWindow(this);
					findToolWindow.VisibleChanged += OnFindDialogVisibleChanged;
				}
				return findToolWindow;
			}
		}
		bool HasOutlineViewerNodes { get { return documentState != null && documentState.OutlineViewerNodes.Count > 0; } }
		bool CanSetNavigationPaneVisibility { get { return Width > NavigationPaneMinWidth + NavigationPane.StickyWidth; } }
		internal PdfDocumentViewer Viewer { get { return viewer; } }
		internal PdfPageCache PageCache { get { return pageCache; } }
		internal Color SelectionColor { get { return selectionColor; } }
		internal IPdfViewerNavigationController NavigationController { get { return viewerController.NavigationController; } }
		internal PdfDocumentViewStateHistoryController HistoryController { get { return viewerController.HistoryController; } }
		internal PdfCaret Caret { get { return documentState == null || documentState.SelectionState == null ? null : documentState.SelectionState.Caret; } }
		internal PdfDocument Document {
			get { return document; }
			set {
				if (CanCloseDocument())
					SetDocument(value, String.Empty, null, false);
			}
		}
		internal PdfDocumentState DocumentState { get { return documentState; } }
		internal PdfDocumentStateController DocumentStateController { get { return documentStateController; } }
		internal bool HasCaret { get { return Caret != null; } }
		internal bool HasFocus { get { return documentStateController != null && documentStateController.HasFocus; } }
		internal bool CanPrint { get { return document != null && document.Pages.Count > 0; } }
		internal PdfDataSelector DataSelector { get { return documentStateController == null ? null : documentStateController.DataSelector; } }
		internal NavigationPane NavigationPane { get { return navigationPane; } }
		internal PdfOutlineViewerControl OutlineViewerControl { get { return outlineViewerControl; } }
		internal int FindToolWindowHeight { get { return (findToolWindow == null || !findToolWindow.Visible) ? 0 : findToolWindow.Size.Height; } }
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerNavigationPaneInitialVisibility"),
#endif
		Category(PdfCategories.Navigation),
		DefaultValue(PdfNavigationPaneVisibility.Default)
		]
		public PdfNavigationPaneVisibility NavigationPaneInitialVisibility {
			get { return navigationPaneInitialVisibility; }
			set { navigationPaneInitialVisibility = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PdfNavigationPaneVisibility NavigationPaneVisibility {
			get { return navigationPaneVisibility; }
			set {
				if (value == PdfNavigationPaneVisibility.Default)
					throw new ArgumentOutOfRangeException("NavigationPaneVisibility", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageIncorrectNavigationPaneVisibility));
				if (CanSetNavigationPaneVisibility || navigationPaneVisibility == PdfNavigationPaneVisibility.Hidden || value == PdfNavigationPaneVisibility.Hidden
					|| value == PdfNavigationPaneVisibility.Collapsed) {
					if (!CanSetNavigationPaneVisibility && value != PdfNavigationPaneVisibility.Hidden)
						value = PdfNavigationPaneVisibility.Collapsed;
					SetNavigationPaneVisibility(value);
					if (navigationPaneVisibility != value) {
						navigationPaneVisibility = value;
						RaiseNavigationPaneVisibilityChanged();
					}
				}
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerNavigationPaneWidth"),
#endif
		Category(PdfCategories.Layout),
		DefaultValue(0)
		]
		public int NavigationPaneWidth {
			get { return navigationPane.RegularSize.Width; }
			set {
				Size navigationPaneRegularSize = navigationPane.RegularSize;
				if (value == 0 && navigationPaneVisibility == PdfNavigationPaneVisibility.Visible)
					value = Width / 5;
				if (value != 0 && value < NavigationPaneMinWidth)
					value = NavigationPaneMinWidth;
				navigationPane.RegularSize = new Size(value, navigationPane.Size.Height);
			}
		}
		[Browsable(false)]
		public int NavigationPaneMinWidth {
			get {
				int minNavigationPaneWidth = 0;
				INavigationPane iNavigationPane = navigationPane as INavigationPane;
				if (iNavigationPane != null) {
					minNavigationPaneWidth = iNavigationPane.RegularMinSize.Width;
				}
				return minNavigationPaneWidth;
			}
		}
		[
		Category(PdfCategories.Document),
		DefaultValue("")
		]
		public string DocumentCreator {
			get { return documentCreator; }
			set { documentCreator = value; }
		}
		[
		Category(PdfCategories.Document),
		DefaultValue("")
		]
		public string DocumentProducer {
			get { return documentProducer; }
			set { documentProducer = value; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerPasswordAttemptsLimit"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(1)
		]
		public int PasswordAttemptsLimit {
			get { return passwordAttemptsLimit; }
			set { passwordAttemptsLimit = value; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerZoomMode"),
#endif
		Category(PdfCategories.Navigation),
		DefaultValue(PdfZoomMode.ActualSize),
		RefreshProperties(RefreshProperties.All)
		]
		public PdfZoomMode ZoomMode {
			get { return zoomMode; }
			set {
				zoomMode = value;
				viewer.UpdateZoomMode(zoomMode);
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerRotationAngle"),
#endif
		Category(PdfCategories.Layout),
		DefaultValue(0)
		]
		public int RotationAngle {
			get { return rotationAngle; }
			set {
				value = PdfPageTreeNode.NormalizeRotate(value);
				if (!PdfPageTreeNode.CheckRotate(value))
					throw new ArgumentOutOfRangeException("RotationAngle", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageRotate));
				rotationAngle = value;
				if (document != null) {
					int oldPageNumber = CurrentPageNumber;
					documentState.RotationAngle = rotationAngle;
					RegisterCurrentDocumentViewState(PdfNavigationMode.Rotation);
					if (CurrentPageNumber != oldPageNumber)
						CurrentPageNumber = oldPageNumber;
				}
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerHighlightFormFields"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(false)
		]
		public bool HighlightFormFields {
			get { return highlightFormFields; }
			set {
				highlightFormFields = value;
				if (documentState != null)
					documentState.HighlightFormFields = value;
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerHighlightedFormFieldColor"),
#endif
		Category(PdfCategories.Behavior)
		]
		public Color HighlightedFormFieldColor {
			get { return highlightedFormFieldColor; }
			set {
				highlightedFormFieldColor = value;
				if (documentState != null)
					documentState.HighlightedFormFieldColor = value;
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerContentMarginMode"),
#endif
		Category(PdfCategories.Layout),
		DefaultValue(PdfContentMarginMode.Dynamic),
		RefreshProperties(RefreshProperties.All)
		]
		public PdfContentMarginMode ContentMarginMode {
			get { return contentMarginMode; }
			set {
				contentMarginMode = value;
				viewer.UpdateAll();
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerContentMinMargin"),
#endif
		Category(PdfCategories.Layout),
		DefaultValue(defaultContentMinMargin)
		]
		public int ContentMinMargin {
			get { return contentMinMargin; }
			set {
				contentMinMargin = value;
				viewer.UpdateAll();
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerDetachStreamAfterLoadComplete"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(false)
		]
		public bool DetachStreamAfterLoadComplete {
			get { return detachStreamAfterLoadComplete; }
			set { detachStreamAfterLoadComplete = value; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerImageCacheSize"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(defaultImageCacheSize)
		]
		public long ImageCacheSize {
			get { return imageCacheSize; }
			set {
				imageCacheSize = value;
				if (documentState != null)
					documentState.ImageDataStorage.UpdateLimit(imageCacheSize);
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerMaxPrintingDpi"),
#endif
		Category(PdfCategories.Printing),
		DefaultValue(0)
		]
		public int MaxPrintingDpi {
			get { return maxPrintingDpi; }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("MaxPrintingDpi", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageIncorrectMaxPrintingDpi));
				maxPrintingDpi = value;
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerShowPrintStatusDialog"),
#endif
		Category(PdfCategories.Printing),
		DefaultValue(true)
		]
		public bool ShowPrintStatusDialog {
			get { return showPrintStatusDialog; }
			set { showPrintStatusDialog = value; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerAcceptsTab"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(true)
		]
		public bool AcceptsTab {
			get { return acceptsTab; }
			set {
				acceptsTab = value;
				outlineViewerControl.TabStop = acceptsTab;
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerMenuManager"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(null)
		]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set {
				if (value != menuManager) {
					menuManager = value;
					RibbonControl ribbon = menuManager as RibbonControl;
					if (ribbon == null) {
						popupMenu.Ribbon = null;
						BarManager barManager = menuManager as BarManager;
						if (barManager != null)
							popupMenu.Manager = barManager;
					}
					else {
						popupMenu.Ribbon = ribbon;
						popupMenu.Manager = ribbon.Manager;
					}
					if (popupMenu.Manager != null)
						popupMenu.Manager.Controller = barAndDockingController;
				}
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerDefaultDocumentDirectory"),
#endif
		Category(PdfCategories.Document),
		Editor("DevExpress.XtraPdfViewer.Design.PdfDefaultDocumentDirectoryEditor," + AssemblyInfo.SRAssemblyXtraPdfViewerDesign, typeof(UITypeEditor)),
		DefaultValue(""),
		RefreshProperties(RefreshProperties.All)
		]
		public string DefaultDocumentDirectory {
			get { return defaultDocumentDirectory; }
			set { defaultDocumentDirectory = value; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerDocumentFilePath"),
#endif
		Category(PdfCategories.Document),
		Editor("DevExpress.XtraPdfViewer.Design.PdfDocumentFilePathEditor," + AssemblyInfo.SRAssemblyXtraPdfViewerDesign, typeof(UITypeEditor)),
		DefaultValue(""),
		RefreshProperties(RefreshProperties.All)
		]
		public string DocumentFilePath {
			get { return documentFilePath; }
			set { SetDocumentFilePath(value, false); }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerZoomFactor"),
#endif
		Category(PdfCategories.Navigation),
		RefreshProperties(RefreshProperties.All)
		]
		public float ZoomFactor {
			get { return viewer.Zoom * 100; }
			set {
				float zoomValue = (float)value / 100;
				float minZoom = viewer.MinZoom;
				if (zoomValue < minZoom)
					zoomValue = minZoom;
				float maxZoom = viewer.MaxZoom;
				if (zoomValue > maxZoom)
					zoomValue = maxZoom;
				viewer.Zoom = zoomValue;
			}
		}
		[Browsable(false)]
		public bool IsDocumentOpened { get { return document != null; } }
		[Browsable(false)]
		public bool IsDocumentChanged {
			get {
				if (documentStateController == null)
					return false;
				documentStateController.CommitCurrentEditor();
				return documentStateController.IsDocumentModified;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PdfOutlineViewerSettings OutlineViewerSettings {
			get { return outlineViewerSettings; }
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerCurrentPageNumber"),
#endif
		Category(PdfCategories.Page),
		DefaultValue(1)
		]
		public int CurrentPageNumber {
			get {
				IDocument document = viewer.Document;
				return (document == null || document.IsEmpty) ? 1 : viewer.SelectedPageIndex + 1;
			}
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException("CurrentPageNumber", XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageCurrentPageNumberOutOfRange));
				int currentPageNumber = CurrentPageNumber;
				viewer.SelectedPageIndex = value - 1;
				if (CurrentPageNumber != currentPageNumber)
					RegisterCurrentDocumentViewState(PdfNavigationMode.Page);
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerPageCount"),
#endif
		Category(PdfCategories.Page)
		]
		public int PageCount {
			get {
				IDocument document = viewer.Document;
				return document == null ? 0 : document.Pages.Count;
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerHandTool"),
#endif
		Category(PdfCategories.Navigation),
		DefaultValue(false)
		]
		public bool HandTool {
			get { return viewer.HandTool; }
			[SecuritySafeCritical]
			set {
				viewer.HandTool = value;
				PdfDataSelector dataSelector = DataSelector;
				if (dataSelector != null) {
					dataSelector.HideCaret();
					CaretInterop.DestroyCaret();
				}
			}
		}
		[
#if !SL
	DevExpressXtraPdfViewerLocalizedDescription("PdfViewerReadOnly"),
#endif
		Category(PdfCategories.Behavior),
		DefaultValue(false)
		]
		public bool ReadOnly {
			get { return readOnly; }
			set { readOnly = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PdfDocumentProperties DocumentProperties {
			get {
				CheckOperationAvailability();
				if (documentProperties == null)
					documentProperties = new PdfDocumentProperties(documentFullFilePath, fileSize, document);
				return documentProperties;
			}
		}
		[Browsable(false)]
		public float MinZoomFactor { get { return viewer.MinZoom * 100; } }
		[Browsable(false)]
		public float MaxZoomFactor { get { return viewer.MaxZoom * 100; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float HorizontalScrollPosition {
			get { return viewer.HorizontalScrollPosition; }
			set { viewer.HorizontalScrollPosition = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float VerticalScrollPosition {
			get { return viewer.VerticalScrollPosition; }
			set { viewer.VerticalScrollPosition = value; }
		}
		[Browsable(false)]
		public bool IsFindDialogVisible { get { return findToolWindow != null && findToolWindow.Visible; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PdfFindDialogOptions FindDialogOptions {
			get { return FindToolWindow.FindDialogOptions; }
			set { FindToolWindow.FindDialogOptions = value; }
		}
		[Browsable(false)]
		public bool HasSelection {
			get {
				PdfSelection selection = Selection;
				return selection != null && selection.Highlights.Count != 0;
			}
		}
		public event PdfPasswordRequestedEventHandler PasswordRequested {
			add { Events.AddHandler(passwordRequested, value); }
			remove { Events.RemoveHandler(passwordRequested, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerDocumentClosing")]
#endif
		public event PdfDocumentClosingEventHandler DocumentClosing {
			add { Events.AddHandler(documentClosing, value); }
			remove { Events.RemoveHandler(documentClosing, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerDocumentChanged")]
#endif
		public event PdfDocumentChangedEventHandler DocumentChanged {
			add { Events.AddHandler(documentChanged, value); }
			remove { Events.RemoveHandler(documentChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerCurrentPageChanged")]
#endif
		public event PdfCurrentPageChangedEventHandler CurrentPageChanged {
			add { Events.AddHandler(currentPageChanged, value); }
			remove { Events.RemoveHandler(currentPageChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerZoomChanged")]
#endif
		public event PdfZoomChangedEventHandler ZoomChanged {
			add { Events.AddHandler(zoomChanged, value); }
			remove { Events.RemoveHandler(zoomChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerSelectionStarted")]
#endif
		public event PdfSelectionPerformedEventHandler SelectionStarted {
			add { Events.AddHandler(selectionStarted, value); }
			remove { Events.RemoveHandler(selectionStarted, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerSelectionContinued")]
#endif
		public event PdfSelectionPerformedEventHandler SelectionContinued {
			add { Events.AddHandler(selectionContinued, value); }
			remove { Events.RemoveHandler(selectionContinued, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerSelectionEnded")]
#endif
		public event PdfSelectionPerformedEventHandler SelectionEnded {
			add { Events.AddHandler(selectionEnded, value); }
			remove { Events.RemoveHandler(selectionEnded, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerFindDialogVisibilityChanged")]
#endif
		public event PdfFindDialogVisibilityChangedEventHandler FindDialogVisibilityChanged {
			add { Events.AddHandler(findDialogVisibilityChanged, value); }
			remove { Events.RemoveHandler(findDialogVisibilityChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerPageSetupDialogShowing")]
#endif
		public event PdfPageSetupDialogShowingEventHandler PageSetupDialogShowing {
			add { Events.AddHandler(pageSetupDialogShowing, value); }
			remove { Events.RemoveHandler(pageSetupDialogShowing, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerPopupMenuShowing")]
#endif
		public event PdfPopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(popupMenuShowing, value); }
			remove { Events.RemoveHandler(popupMenuShowing, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerScrollPositionChanged")]
#endif
		public event PdfScrollPositionChangedEventHandler ScrollPositionChanged {
			add { Events.AddHandler(scrollPositionChanged, value); }
			remove { Events.RemoveHandler(scrollPositionChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerReferencedDocumentOpening")]
#endif
		public event PdfReferencedDocumentOpeningEventHandler ReferencedDocumentOpening {
			add { Events.AddHandler(referencedDocumentOpening, value); }
			remove { Events.RemoveHandler(referencedDocumentOpening, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerUriOpening")]
#endif
		public event PdfUriOpeningEventHandler UriOpening {
			add { Events.AddHandler(uriOpening, value); }
			remove { Events.RemoveHandler(uriOpening, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerUriOpening")]
#endif
		public event PdfFileAttachmentOpeningEventHandler FileAttachmentOpening {
			add { Events.AddHandler(fileAttachmentOpening, value); }
			remove { Events.RemoveHandler(fileAttachmentOpening, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerNavigationPaneVisibilityChanged")]
#endif
		public event PdfNavigationPaneVisibilityChangedEventHandler NavigationPaneVisibilityChanged {
			add { Events.AddHandler(navigationPaneVisibilityChanged, value); }
			remove { Events.RemoveHandler(navigationPaneVisibilityChanged, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerFormFieldValueChanging")]
#endif
		public event PdfFormFieldValueChangingEventHandler FormFieldValueChanging {
			add { Events.AddHandler(formFieldValueChanging, value); }
			remove { Events.RemoveHandler(formFieldValueChanging, value); }
		}
#if !SL
	[DevExpressXtraPdfViewerLocalizedDescription("PdfViewerFormFieldValueChanged")]
#endif
		public event PdfFormFieldValueChangedEventHandler FormFieldValueChanged {
			add { Events.AddHandler(formFieldValueChanged, value); }
			remove { Events.RemoveHandler(formFieldValueChanged, value); }
		}
		public PdfViewer() {
			BarManager barManager = new BarManager(components);
			barManager.Form = this;
			MenuManager = barManager;
			viewer = new PdfDocumentViewer(this);
			barAndDockingController.LookAndFeel.ParentLookAndFeel = viewer.LookAndFeel;
			viewerController = new PdfViewerController(this);
			commands = new PdfViewerCommandRepository(this);
			viewer.Dock = DockStyle.Fill;
			viewer.ShowPageMargins = false;
			viewer.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			viewer.SelectedPageChanged += new EventHandler(OnSelectedPageChanged);
			viewer.ZoomChanged += new EventHandler(OnZoomChanged);
			viewer.MouseUp += new MouseEventHandler(OnViewerContextMenu);
			viewer.Click += new EventHandler(OnViewerClick);
			viewer.DoubleClick += new EventHandler(OnViewerDoubleClick);
			viewer.KeyDown += new KeyEventHandler(OnViewerKeyDown);
			ViewControl viewControl = viewer.ViewControl;
			viewControl.GotFocus += new EventHandler(OnViewControlGotFocus);
			viewControl.LostFocus += new EventHandler(OnViewControlLostFocus);
			viewControl.MouseMove += new MouseEventHandler(OnViewControlMouseMove);
			viewControl.MouseEnter += new EventHandler(OnViewControlMouseEnter);
			viewControl.MouseHover += new EventHandler(OnViewControlMouseHover);
			viewControl.MouseLeave += new EventHandler(OnViewControlMouseLeave);
			viewControl.MouseDown += new MouseEventHandler(OnViewControlMouseDown);
			viewControl.MouseUp += new MouseEventHandler(OnViewControlMouseUp);
			viewControl.MouseCaptureChanged += new EventHandler(OnViewControlMouseCaptureChanged);
			viewControl.MouseClick += new MouseEventHandler(OnViewControlMouseClick);
			viewControl.MouseDoubleClick += new MouseEventHandler(OnViewControlMouseDoubleClick);
			viewControl.KeyDown += new KeyEventHandler(OnViewControlKeyDown);
			viewControl.KeyUp += new KeyEventHandler(OnViewControlKeyUp);
			viewControl.KeyPress += new KeyPressEventHandler(OnViewControlKeyPress);
			LookAndFeel.StyleChanged += new EventHandler(OnStyleChanged);
			UpdateSkin();
			splitter = new PdfSplitter(this, navigationPane);
			outlineViewerControl = new PdfOutlineViewerControl(this);
			outlineViewerControl.TabStop = acceptsTab;
			outlineViewerControl.Dock = DockStyle.Fill;
			outlineViewerSettings = new PdfOutlineViewerSettings(outlineViewerControl.TreeList);
			outlineViewerNavigationPage.Controls.Add(outlineViewerControl);
			outlineViewerNavigationPage.Image = outlineViewerImage;
			outlineViewerNavigationPage.Text = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.NavigationPaneOutlinesPageCaption);
			outlineViewerNavigationPage.LookAndFeel.ParentLookAndFeel = viewer.LookAndFeel;
			outlineViewerNavigationPage.BackgroundPadding = new Padding(0);
			attachmentsViewerControl = new PdfAttachmentsViewerControl(this);
			attachmentsViewerControl.TabStop = acceptsTab;
			attachmentsViewerControl.Dock = DockStyle.Fill;
			attachmentsViewerNavigationPage.Controls.Add(attachmentsViewerControl);
			attachmentsViewerNavigationPage.Image = attachmentViewerImage;
			attachmentsViewerNavigationPage.Text = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.NavigationPaneAttachmentsPageCaption);
			attachmentsViewerNavigationPage.LookAndFeel.ParentLookAndFeel = viewer.LookAndFeel;
			attachmentsViewerNavigationPage.BackgroundPadding = new Padding(0);
			attachmentsViewerNavigationPage.PageVisible = true;
			navigationPane.LookAndFeel.ParentLookAndFeel = viewer.LookAndFeel;
			navigationPane.Visible = false;
			navigationPane.TabStop = false;
			navigationPane.Pages.AddRange(new NavigationPage[] { outlineViewerNavigationPage, attachmentsViewerNavigationPage });
			navigationPane.StateChanged += new StateChangedEventHandler(OnNavigationPaneStateChanged);
			navigationPane.Dock = DockStyle.Left;
			navigationPane.SelectedPageChanged += new SelectedPageChangedEventHandler(OnSelectedPageChanged);
			navigationPane.AllowResize = false;
			Controls.Add(viewer);
			Controls.Add(splitter);
			Controls.Add(navigationPane);
		}
		public void LoadDocument(Stream stream) {
			try {
				LoadDocument(stream, String.Empty, false);
			}
			catch (PdfIncorrectPasswordException) {
			}
		}
		public void LoadDocument(string path) {
			LoadDocument(path, false);
		}
		public void SaveDocument(Stream stream) {
			SaveDocument(stream, new PdfSaveOptions());
		}
		public void SaveDocument(string path) {
			SaveDocument(path, new PdfSaveOptions());
		}
		public void CloseDocument() {
			Document = null;
		}
		public SizeF GetPageSize(int pageNumber) {
			CheckPageNumber(pageNumber);
			PdfPage page = document.Pages[pageNumber - 1];
			double inchFactor = page.UserUnit / 72;
			PdfRectangle cropBox = page.CropBox;
			float width = (float)(cropBox.Width * inchFactor);
			float height = (float)(cropBox.Height * inchFactor);
			int rotate = page.Rotate;
			return (rotate == 90 || rotate == 270) ? new SizeF(height, width) : new SizeF(width, height);
		}
		public void Print(PdfPrinterSettings pdfPrinterSettings) {
			CheckOperationAvailability();
			documentStateController.CommitCurrentEditor();
			if (pdfPrinterSettings == null)
				pdfPrinterSettings = new PdfPrinterSettings();
			if (maxPrintingDpi != 0 && maxPrintingDpi < pdfPrinterSettings.PrintingDpi)
				pdfPrinterSettings.PrintingDpi = maxPrintingDpi;
			if (CanPrint)
				PdfDocumentPrinter.Print(documentState, documentFilePath, CurrentPageNumber, pdfPrinterSettings, showPrintStatusDialog);
		}
		public void Print() {
			CheckOperationAvailability();
			documentStateController.CommitCurrentEditor();
			if (CanPrint) {
				PdfPageSetupDialogShowingEventArgs args = new PdfPageSetupDialogShowingEventArgs();
				ShowPageSetupDialog(args);
			}
		}
		[Obsolete("Use the Print(PdfPrinterSettings pdfPrinterSettings) overload of this method instead.")]
		public void Print(PrinterSettings printerSettings) {
			Print(new PdfPrinterSettings(printerSettings));
		}
		public Bitmap CreateBitmap(int pageNumber, int largestEdgeLength) {
			CheckPageNumber(pageNumber);
			if (largestEdgeLength < 1)
				throw new ArgumentOutOfRangeException("largestEdgeLength", String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectLargestEdgeLength)));
			return PdfViewerCommandInterpreter.GetBitmap(documentState, pageNumber - 1, largestEdgeLength, PdfRenderMode.Print);
		}
		public void ScrollVertical(int amount) {
			viewer.ScrollVertical(amount);
		}
		public void ScrollHorizontal(int amount) {
			viewer.ScrollHorizontal(amount);
		}
		public PdfTextSearchResults FindText(string text, PdfTextSearchParameters parameters, Func<int, bool> terminate) {
			if (documentStateController == null)
				return new PdfTextSearchResults(null, 0, null, null, PdfTextSearchStatus.NotFound);
			PdfTextSearchResults results = documentStateController.FindText(text, parameters, CurrentPageNumber, terminate);
			return results;
		}
		public PdfTextSearchResults FindText(string text, PdfTextSearchParameters parameters) {
			return FindText(text, parameters, null);
		}
		public PdfTextSearchResults FindText(string text) {
			return FindText(text, null, null);
		}
		public void ShowFindDialog() {
			ShowFindDialog(null);
		}
		public void ShowFindDialog(PdfFindDialogOptions findDialogOptions) {
			ShowFindDialog((PdfFindDialogOptions?)findDialogOptions);
		}
		public void HideFindDialog(bool immediate) {
			if (findToolWindow != null && findToolWindow.Visible)
				findToolWindow.HideToolWindow(immediate);
		}
		public void HideFindDialog() {
			HideFindDialog(false);
		}
		public bool ShowDocumentClosingWarning() {
			DialogResult result = XtraMessageBox.Show(LookAndFeel, ParentForm, XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageDocumentClosing),
				XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageDocumentClosingCaption), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			switch (result) {
				case DialogResult.Yes:
					return SaveDocument();
				case DialogResult.No:
					return true;
				default:
					return false;
			}
		}
		public PointF GetClientPoint(PdfDocumentPosition documentPosition) {
			if (documentPosition == null)
				throw new ArgumentNullException("documentPosition");
			CheckOperationAvailability();
			return viewer.DocumentToClient(documentPosition);
		}
		public PdfDocumentPosition GetDocumentPosition(PointF clientPoint) {
			return GetDocumentPosition(clientPoint, true);
		}
		public PdfDocumentPosition GetDocumentPosition(PointF clientPoint, bool inPageBounds) {
			CheckOperationAvailability();
			PdfDocumentPosition pos = inPageBounds ? viewer.ClientToDocument(clientPoint) : viewer.GetDocumentPosition(clientPoint);
			return pos.PageNumber == 0 ? null : pos;
		}
		public void Select(RectangleF clientRectangle) {
			viewer.Select(clientRectangle);
		}
		public void Select(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			if (startPosition == null)
				throw new ArgumentNullException("startPosition");
			if (endPosition == null)
				throw new ArgumentNullException("endPosition");
			DataSelector.SelectText(startPosition, endPosition);
		}
		public void ClearSelection() {
			PdfDataSelector dataSelector = DataSelector;
			if (dataSelector != null)
				dataSelector.ClearSelection();
		}
		public void SelectAllText() {
			try {
				PdfDataSelector dataSelector = DataSelector;
				if (dataSelector != null)
					dataSelector.SelectAllText();
			}
			catch {
			}
		}
		public PdfDocumentContent GetContentInfo(PointF clientPoint) {
			PdfDataSelector dataSelector = DataSelector;
			return dataSelector == null ? null : dataSelector.GetContentInfo(viewer.ClientToDocument(clientPoint));
		}
		public PdfSelectionContent GetSelectionContent() {
			PdfTextSelection textSelection = Selection as PdfTextSelection;
			if (textSelection != null)
				return new PdfSelectionContent(PdfSelectionContentType.Text, null, textSelection.Text);
			Bitmap selectedBitmap = SelectedBitmap;
			return selectedBitmap == null ? new PdfSelectionContent(PdfSelectionContentType.None, null, null) : new PdfSelectionContent(PdfSelectionContentType.Image, selectedBitmap, null);
		}
		public void CreateRibbon() {
			Control parent = Parent;
			RibbonForm ribbonForm = parent as RibbonForm;
			RibbonControl ribbon = FindRibbon();
			if (ribbon == null) {
				ribbon = new RibbonControl();
				ribbon.BeginInit();
				PdfBarsUtils.SetupRibbon(ribbon);
				parent.Controls.Add(ribbon);
				ribbon.EndInit();
			}
			if (ribbonForm != null)
				ribbonForm.Ribbon = ribbon;
			CreateBars();
			MenuManager = ribbon;
		}
		public void CreateBars() {
			BarDockControl dockControl = FindControl(typeof(BarDockControl)) as BarDockControl;
			BarManager barManager = dockControl == null ? null : dockControl.Manager;
			if (barManager == null) {
				barManager = new BarManager();
				barManager.Form = ParentForm;
				MenuManager = barManager;
			}
			new PdfBarsGenerator(this, barManager).AddNewBars(PdfBarsUtils.BarCreators, String.Empty, BarInsertMode.Add);
			RaiseUpdateUI();
		}
		internal void LoadDocument(string path, bool resetRotateAndZoom) {
			FileStream stream = null;
			try {
				stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				LoadDocument(stream, path, resetRotateAndZoom);
				documentStream = stream;
			}
			catch (Exception e) {
				if (!detachStreamAfterLoadComplete && stream != null)
					stream.Dispose();
				if (!(e is PdfIncorrectPasswordException))
					throw;
			}
			finally {
				if (stream != null && detachStreamAfterLoadComplete)
					stream.Dispose();
			}
		}
		internal void SetDocumentFilePath(string filePath, bool resetRotateAndZoom) {
			if (String.IsNullOrEmpty(filePath))
				CloseDocument();
			else if (filePath != documentFilePath)
				LoadDocument(filePath, resetRotateAndZoom);
		}
		internal void ImportFormData() {
			if (Document != null) {
				documentStateController.CommitCurrentEditor();
				using (OpenFileDialog openDialog = new OpenFileDialog()) {
					openDialog.Filter = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FormDataFileFilter);
					openDialog.RestoreDirectory = true;
					if (openDialog.ShowDialog() == DialogResult.OK) {
						string path = openDialog.FileName;
						if (!String.IsNullOrEmpty(path))
							try {
								PdfFormData data = new PdfFormData(path);
								documentState.FormData.Apply(data);
							}
							catch {
								XtraMessageBox.Show(viewer.LookAndFeel, viewer.ParentForm, String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageImportError), path, GetFormat(path).ToString()),
										XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}
					}
				}
			}
		}
		internal void ExportFormData() {
			if (Document != null) {
				documentStateController.CommitCurrentEditor();
				using (SaveFileDialog saveDialog = new SaveFileDialog()) {
					saveDialog.Filter = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.FormDataFileFilter);
					saveDialog.RestoreDirectory = true;
					string path = DocumentFilePath;
					if (!String.IsNullOrEmpty(path)) {
						saveDialog.FileName = Path.GetFileNameWithoutExtension(path);
					}
					switch (saveDialog.ShowDialog()) {
						case DialogResult.OK:
							try {
								documentState.FormData.Save(saveDialog.FileName, GetFormat(saveDialog.FileName));
							}
							catch {
								XtraMessageBox.Show(viewer.LookAndFeel, viewer.ParentForm, String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageExportError), saveDialog.FileName),
									XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							}
							break;
					}
				}
			}
		}
		internal bool SaveDocument() {
			if (Document != null)
				using (SaveFileDialog saveDialog = new SaveFileDialog()) {
					saveDialog.Filter = XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.PDFFileFilter);
					if (!String.IsNullOrEmpty(defaultDocumentDirectory))
						saveDialog.InitialDirectory = defaultDocumentDirectory;
					saveDialog.RestoreDirectory = true;
					string path = DocumentFilePath;
					if (!String.IsNullOrEmpty(path))
						saveDialog.FileName = path;
					switch (saveDialog.ShowDialog()) {
						case DialogResult.OK:
							path = saveDialog.FileName;
							try {
								SaveDocument(path);
							}
							catch {
								XtraMessageBox.Show(viewer.LookAndFeel, viewer.ParentForm, String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSaveAsError), path),
									XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
								return false;
							}
							break;
						default:
							return false;
					}
				}
			return true;
		}
		internal void CreatePageViews(PdfDocument document, PdfDocumentState documentState) {
			PdfViewerDocument viewerDocument = new PdfViewerDocument(pageCache);
			IList<IPage> pages = viewerDocument.Pages;
			int pageIndex = 0;
			foreach (PdfPage page in document.Pages) {
				PdfRectangle cropBox = page.CropBox;
				PdfRectangle bleedBox = page.BleedBox;
				double width;
				double height;
				double visibleWidth;
				double visibleHeight;
				double leftMargin;
				double topMargin;
				switch (PdfPageTreeNode.NormalizeRotate(page.Rotate + rotationAngle)) {
					case 90:
						width = cropBox.Height;
						height = cropBox.Width;
						visibleWidth = bleedBox.Height;
						visibleHeight = bleedBox.Width;
						leftMargin = cropBox.Bottom - bleedBox.Bottom;
						topMargin = bleedBox.Left - cropBox.Left;
						break;
					case 180:
						width = cropBox.Width;
						height = cropBox.Height;
						visibleWidth = bleedBox.Width;
						visibleHeight = bleedBox.Height;
						leftMargin = cropBox.Right - bleedBox.Right;
						topMargin = cropBox.Bottom - bleedBox.Bottom;
						break;
					case 270:
						width = cropBox.Height;
						height = cropBox.Width;
						visibleWidth = bleedBox.Height;
						visibleHeight = bleedBox.Width;
						leftMargin = bleedBox.Top - cropBox.Top;
						topMargin = cropBox.Right - bleedBox.Right;
						break;
					default:
						width = cropBox.Width;
						height = cropBox.Height;
						visibleWidth = bleedBox.Width;
						visibleHeight = bleedBox.Height;
						leftMargin = cropBox.Right - bleedBox.Right;
						topMargin = cropBox.Bottom - bleedBox.Bottom;
						break;
				}
				double userUnit = page.UserUnit;
				double xMultiplier = userUnit * PdfDocumentViewer.DocumentToViewerFactorX;
				double yMultiplier = userUnit * PdfDocumentViewer.DocumentToViewerFactorY;
				RectangleF visibleRect = new RectangleF((float)(leftMargin * xMultiplier), (float)(topMargin * yMultiplier), (float)(visibleWidth * xMultiplier), (float)(visibleHeight * yMultiplier));
				pages.Add(new PdfPageView(this, documentState, pageIndex++, Size.Round(new SizeF((float)(width * xMultiplier), (float)(height * yMultiplier))), visibleRect));
			}
			viewer.Document = viewerDocument;
			viewer.UpdateZoomMode(zoomMode);
		}
		internal void ShowRectangleOnPage(PdfRectangleAlignMode alignMode, int pageIndex, PdfRectangle rectangleBounds) {
			ViewManager viewManager = viewer.ViewManager;
			RectangleF rectangle = GetRectangleToShow(rectangleBounds.Left, rectangleBounds.Top, rectangleBounds.Right, rectangleBounds.Bottom, pageIndex);
			RectangleF visibleRect = viewer.VisibleRect;
			float x = rectangle.X;
			float y = rectangle.Y;
			switch (alignMode) {
				case PdfRectangleAlignMode.Center:
					if (!visibleRect.Contains(rectangle)) {
						HistoryController.PerformLockedOperation(() => {
							x -= CalculateCenteredRectangleScrollOffset(visibleRect.Width, rectangle.Width);
							y -= CalculateCenteredRectangleScrollOffset(visibleRect.Height, rectangle.Height);
							UpdateScrollBars(x, y);
						});
						RegisterCurrentDocumentViewState(PdfNavigationMode.Position);
					}
					break;
				case PdfRectangleAlignMode.Edge:
					HistoryController.PerformLockedOperation(() => {
						float scrollPositionX = CalculateScrollPosition(visibleRect.Left, x, visibleRect.Right, rectangle.Right);
						float scrollPositionY = CalculateScrollPosition(visibleRect.Top, y, visibleRect.Bottom, rectangle.Bottom);
						UpdateScrollBars(scrollPositionX, scrollPositionY);
					});
					RegisterCurrentDocumentViewState(PdfNavigationMode.Selection);
					break;
			}
		}
		internal void ShowDocumentPosition(PdfTarget target) {
			if (Document == null)
				return;
			HistoryController.PerformLockedOperation(() => {
				PdfDocumentViewerViewManager viewManager = (PdfDocumentViewerViewManager)viewer.ViewManager;
				PdfPage page = Document.Pages[target.PageIndex];
				PdfPoint scrollPosition = new PdfPoint(0, 0);
				switch (target.Mode) {
					case PdfTargetMode.Fit:
					case PdfTargetMode.FitBBox:
						scrollPosition = CalculatePageTopLeftCorner(page);
						ZoomMode = PdfZoomMode.PageLevel;
						break;
					case PdfTargetMode.FitHorizontally:
					case PdfTargetMode.FitBBoxHorizontally:
						scrollPosition = new PdfPoint(0, CalculateScrollPositionOnPage(target.X, target.Y, page).Y);
						ZoomMode = PdfZoomMode.FitToWidth;
						break;
					case PdfTargetMode.XYZ:
						scrollPosition = CalculateScrollPositionOnPage(target.X, target.Y, page);
						double? zoom = target.Zoom;
						if (zoom.HasValue) {
							ZoomMode = zoomMode;
							ZoomFactor = (float)(zoom.Value * 100);
						}
						break;
					case PdfTargetMode.FitVertically:
					case PdfTargetMode.FitBBoxVertically:
						double x = CalculateScrollPositionOnPage(target.X, target.Y, page).X;
						double y = CalculatePageTopLeftCorner(page).Y;
						scrollPosition = new PdfPoint(x, y);
						ZoomFactor = 100 * viewManager.ClientHeight / ((float)page.CropBox.Height * PdfRenderingCommandInterpreter.DpiFactor);
						break;
					case PdfTargetMode.FitRectangle:
						double left = target.X.Value - page.CropBox.Left;
						double top = target.Y.Value - page.CropBox.Bottom;
						if (target.Width == 0 || target.Height == 0) {
							scrollPosition = new PdfPoint(left, top);
							break;
						}
						float clientWidth = viewManager.ClientWidth / PdfRenderingCommandInterpreter.DpiFactor;
						float clientHeight = viewManager.ClientHeight / PdfRenderingCommandInterpreter.DpiFactor;
						bool isHorizontalView = rotationAngle == 90 || rotationAngle == 270;
						double width = target.Width;
						double height = target.Height;
						float zoomedClientWidth = clientWidth;
						float zoomedClientHeight = clientHeight;
						if (isHorizontalView) {
							width = target.Height;
							height = target.Width;
							zoomedClientWidth = clientHeight;
							zoomedClientHeight = clientWidth;
						}
						ZoomFactor = 100 * (float)Math.Min(clientWidth / width, clientHeight / height);
						zoomedClientWidth /= viewer.Zoom;
						zoomedClientHeight /= viewer.Zoom;
						PointF offset = CalculateCenteredRectangleScrollOffset(zoomedClientWidth, (float)target.Width, zoomedClientHeight, (float)target.Height);
						left += offset.X;
						top += offset.Y;
						scrollPosition = new PdfPoint(left, top);
						break;
				}
				PointF point = viewer.DocumentToViewer(new PdfDocumentPosition(target.PageIndex + 1, scrollPosition));
				UpdateScrollBars(point.X, point.Y);
			});
			RegisterCurrentDocumentViewState(PdfNavigationMode.Position);
		}
		internal void ShowPageSetupDialog(PdfPageSetupDialogShowingEventArgs e) {
			using (PdfPageSetupDialog form = new PdfPageSetupDialog()) {
				Size formSize = form.Size;
				Size minimumFormSize = form.MinimumSize;
				e.FormSize = form.Size;
				e.MinimumFormSize = minimumFormSize;
				OnPageSetupDialogShowing(e);
				form.Initialize(documentState, documentFilePath, this, e.PrinterSettings);
				Size newFormSize = e.FormSize;
				if (newFormSize != formSize)
					form.Size = new Size(Math.Max(newFormSize.Width, minimumFormSize.Width), Math.Max(newFormSize.Height, minimumFormSize.Height));
				try {
					if (form.ShowDialog(this) == DialogResult.OK)
						Print(form.PrinterSettings);
				}
				catch {
				}
			}
		}
		internal void RegisterCurrentDocumentViewState(PdfNavigationMode navigationMode) {
			HistoryController.RegisterCurrentDocumentViewState(navigationMode);
		}
		internal void HandleScrolling() {
			RegisterCurrentDocumentViewState(PdfNavigationMode.Scroll);
			HandleMouseMoving(PointToClient(MousePosition), MouseButtons);
			OnScrollPositionChanged(new PdfScrollPositionChangedEventArgs(HorizontalScrollPosition, VerticalScrollPosition));
		}
		internal void RaiseUpdateUI() {
			RibbonControl ribbon = FindRibbon();
			if (ribbon != null) {
				foreach (RibbonPage page in ribbon.Pages) {
					PdfFormDataRibbonPage formDataPage = page as PdfFormDataRibbonPage;
					if (formDataPage != null)
						formDataPage.Visible = document != null && document.AcroForm != null && document.AcroForm.Fields != null && document.AcroForm.Fields.Count != 0;
				}
			}
			if (onUpdateUI != null)
				onUpdateUI(this, EventArgs.Empty);
			ICommandUIState state = new DefaultCommandUIState();
			foreach (BarItemLink itemLink in popupMenu.ItemLinks) {
				BarItem item = itemLink.Item;
				PdfViewerCommand command = item.Tag as PdfViewerCommand;
				if (command != null) {
					command.UpdateUIState(state);
					item.Enabled = state.Enabled;
				}
			}
		}
		internal void CancelSaveOperation() {
			cancelSaveOperation = true;
		}
		internal void RaisePopupMenuShowing(PdfPopupMenuShowingEventArgs args) {
			OnPopupMenuShowing(args);
		}
		internal void RaiseFileAttachmentOpening(PdfFileAttachmentOpeningEventArgs args) {
			OnFileAttachmentOpening(args);
			if (Events[fileAttachmentOpening] == null || !(args.Cancel || args.Handled)) {
				string fileName = args.FileAttachment.FileName;
				if (fileName.Length > maximumMessageBoxTextLength)
					fileName = fileName.Substring(0, maximumMessageBoxTextLength - 3) + "...";
				if (XtraMessageBox.Show(LookAndFeel, ParentForm, String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSecurityWarningFileAttachmentOpening), fileName), XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSecurityWarningCaption), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
					args.Cancel = true;
					return;
				}
			}
		}
		internal void RaiseUriOpening(PdfUriOpeningEventArgs args) {
			OnUriOpening(args);
			if (Events[uriOpening] == null || !(args.Cancel || args.Handled)) {
				string uri = args.Uri.AbsoluteUri;
				if (uri.Length > maximumMessageBoxTextLength)
					uri = uri.Substring(0, maximumMessageBoxTextLength - 3) + "...";
				if (XtraMessageBox.Show(LookAndFeel, ParentForm, String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSecurityWarningUriOpening), uri), XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageSecurityWarningCaption), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
					args.Cancel = true;
					return;
				}
			}
		}
		internal void RaiseSelectionStarted(PdfDocumentPosition position) {
			OnSelectionStarted(new PdfSelectionEventArgs(position));
		}
		internal void RaiseSelectionContinued(PdfDocumentPosition position) {
			OnSelectionContinued(new PdfSelectionEventArgs(position));
		}
		internal void RaiseSelectionEnded(PdfDocumentPosition position) {
			OnSelectionEnded(new PdfSelectionEventArgs(position));
		}
		bool ShouldSerializeZoomFactor() {
			return zoomMode == PdfZoomMode.Custom;
		}
		bool ShouldSerializeHighlightedFormFieldColor() {
			return highlightedFormFieldColor != defaultHighlightedFormFieldColor;
		}
		void CheckOperationAvailability() {
			if (document == null)
				throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
		}
		void CheckPageNumber(int pageNumber) {
			CheckOperationAvailability();
			if (pageNumber < 1 || pageNumber > PageCount)
				throw new ArgumentOutOfRangeException("pageNumber", String.Format(XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageIncorrectPageNumber)));
		}
		void UpdateScrollBars(float x, float y) {
			ViewManager viewManager = viewer.ViewManager;
			PointF previousPosition = viewManager.ScrollPos;
			viewManager.SetHorizScroll(x);
			viewManager.SetVertScroll(y - FindToolWindowHeight * viewer.VerticalScreenToDocumentFactor);
			if (previousPosition != viewManager.ScrollPos)
				HandleScrolling();
		}
		PointF CalculateCenteredRectangleScrollOffset(float zoomedClientWidth, float rectangleWidth, float zoomedClientHeight, float rectangleHeight) {
			float offsetX = rectangleWidth;
			float offsetY = rectangleHeight;
			offsetX += (rotationAngle == 0 || rotationAngle == 90) ? -zoomedClientWidth : zoomedClientWidth;
			offsetY += (rotationAngle == 0 || rotationAngle == 270) ? -zoomedClientHeight : zoomedClientHeight;
			return new PointF(offsetX / 2, -offsetY / 2);
		}
		float CalculateCenteredRectangleScrollOffset(float max, float min) {
			return (max > min) ? (max - min) / 2 : 0;
		}
		PdfPoint CalculatePageTopLeftCorner(PdfPage page) {
			PdfRectangle cropBox = page.CropBox;
			PdfPoint pageTopLeftCorner = page.GetTopLeftCorner(rotationAngle);
			return new PdfPoint(pageTopLeftCorner.X - cropBox.Left, pageTopLeftCorner.Y - cropBox.Bottom);
		}
		PdfPoint CalculateScrollPositionOnPage(double? scrollPositionX, double? scrollPositionY, PdfPage page) {
			PdfPoint currentScrollPosition = new PdfPoint(0, 0);
			if (!scrollPositionX.HasValue || !scrollPositionY.HasValue)
				currentScrollPosition = GetCurrentScrollPosition(page);
			PdfRectangle cropBox = page.CropBox;
			double positionX = scrollPositionX ?? currentScrollPosition.X;
			double positionY = scrollPositionY ?? currentScrollPosition.Y;
			if (scrollPositionX.HasValue)
				positionX -= cropBox.Left;
			if (scrollPositionY.HasValue)
				positionY -= cropBox.Bottom;
			return new PdfPoint(positionX, positionY);
		}
		PdfPoint GetCurrentScrollPosition(PdfPage page) {
			PdfDocumentViewerViewManager viewManager = (PdfDocumentViewerViewManager)viewer.ViewManager;
			PointF location = viewManager.GetPageRect(viewManager.Pages[CurrentPageNumber - 1]).Location;
			PdfPoint currentScrollPosition = new PdfPoint(viewManager.ScrollPos.X - location.X, viewManager.ScrollPos.Y - location.Y);
			return page.FromUserSpace(currentScrollPosition, PdfDocumentViewer.ClientToDocumentFactorX, PdfDocumentViewer.ClientToDocumentFactorY, rotationAngle);
		}
		float CalculateScrollPosition(float currentScrollPosition, float newScrollPosition, float currentMaxPosition, float newMaxPosition) {
			if (newScrollPosition < currentScrollPosition)
				return Math.Max(newScrollPosition - caretVisibilityMargin / actualZoom, 0);
			if (newMaxPosition <= currentMaxPosition)
				return currentScrollPosition;
			return Math.Max(currentScrollPosition - currentMaxPosition + newMaxPosition + caretVisibilityMargin / actualZoom, 0);
		}
		RectangleF GetRectangleToShow(double left, double top, double right, double bottom, int pageIndex) {
			int pageNumber = pageIndex + 1;
			PointF topLeft = viewer.DocumentToViewer(new PdfDocumentPosition(pageNumber, new PdfPoint(left, top)));
			PointF bottomRight = viewer.DocumentToViewer(new PdfDocumentPosition(pageNumber, new PdfPoint(right, bottom)));
			float x = Math.Min(topLeft.X, bottomRight.X);
			float y = Math.Min(bottomRight.Y, topLeft.Y);
			return RectangleF.FromLTRB(x, y, Math.Max(bottomRight.X, topLeft.X), Math.Max(bottomRight.Y, topLeft.Y));
		}
		void UnloadDocument(bool clearViewerDocument) {
			fontStorage.Clear();
			pageCache.Clear();
			PdfInteractiveForm form = document == null ? null : document.AcroForm;
			if (form != null) {
				form.FormFieldValueChanged -= OnInteractiveFormFieldValueChanged;
				form.FormFieldValueChanging -= OnInteractiveFormFieldValueChanging;
			}
			if (documentStateController != null) {
				documentStateController.Dispose();
				documentStateController = null;
			}
			if (documentState != null) {
				documentState.Dispose();
				documentState = null;
			}
			ClearDocumentStream();
			documentProperties = null;
			if (clearViewerDocument)
				viewer.Document = null;
		}
		void ClearDocumentStream() {
			if (documentStream != null) {
				documentStream.Dispose();
				documentStream = null;
			}
		}
		void RaiseFindDialogVisibilityChanged() {
			OnFindDialogVisibilityChanged(new PdfFindDialogVisibilityChangedEventArgs(findToolWindow.Visible));
		}
		void RaiseNavigationPaneVisibilityChanged() {
			OnNavigationPaneVisibilityChanged(new PdfNavigationPaneVisibilityChangedEventArgs(navigationPaneVisibility));
		}
		void DestroyFindDialog() {
			if (findToolWindow != null) {
				findToolWindow.VisibleChanged -= OnFindDialogVisibleChanged;
				findToolWindow.Close();
				RaiseFindDialogVisibilityChanged();
				findToolWindow.Dispose();
				findToolWindow = null;
			}
		}
		void RaiseDocumentChanged(Stream stream) {
			DestroyFindDialog();
			OnDocumentChanged(new PdfDocumentChangedEventArgs(documentFilePath, String.IsNullOrEmpty(documentFilePath) ? stream : null));
		}
		bool CanCloseDocument() {
			if (document == null)
				return true;
			if (documentStateController != null)
				documentStateController.CommitCurrentEditor();
			PdfDocumentClosingEventArgs args = new PdfDocumentClosingEventArgs(documentStateController.IsDocumentModified);
			if (Events[documentClosing] != null) {
				OnDocumentClosing(args);
				return !args.Cancel;
			}
			return !documentStateController.IsDocumentModified || ShowDocumentClosingWarning();
		}
		void SetDocument(PdfDocument newDocument, string newDocumentFilePath, Stream stream, bool resetRotateAndZoom) {
			if (!Object.ReferenceEquals(newDocument, document)) {
				if (newDocument == null) {
					UnloadDocument(true);
					document = null;
					fileSize = 0;
					documentFilePath = String.Empty;
					documentFullFilePath = String.Empty;
					SetNavigationPaneInitialVisibility(PdfNavigationPaneVisibility.Hidden);
					outlineViewerControl.ClearOutlineViewerNodes();
					RaiseUpdateUI();
				}
				else {
					PdfDocumentState documentState = new PdfDocumentState(newDocument, fontStorage, imageCacheSize);
					IList<PdfOutlineViewerNode> outlineViewerNodes = documentState.OutlineViewerNodes;
					documentState.RotationAngle = rotationAngle;
					documentState.HighlightFormFields = highlightFormFields;
					documentState.HighlightedFormFieldColor = highlightedFormFieldColor;
					PdfDocumentStateController documentStateController = new PdfDocumentStateController(viewerController, documentState);
					CreatePageViews(newDocument, documentState);
					UnloadDocument(false);
					fileSize = stream == null ? 0 : stream.Length;
					document = newDocument;
					documentFilePath = newDocumentFilePath;
					documentFullFilePath = String.IsNullOrEmpty(documentFilePath) ? String.Empty : Path.GetFullPath(documentFilePath);
					this.documentState = documentState;
					this.documentStateController = documentStateController;
					PdfDataSelector dataSelector = documentStateController.DataSelector;
					dataSelector.SetZoomFactor(viewer.Zoom);
					viewer.SelectedPageIndex = 0;
					outlineViewerControl.SetOutlineViewerNodes(outlineViewerNodes);
					attachmentsViewerControl.InvalidateAttachments();
					outlineViewerNavigationPage.PageVisible = outlineViewerNodes.Count > 0;
					SetNavigationPaneInitialVisibility(navigationPaneInitialVisibility);
					if (outlineViewerNavigationPage.PageVisible)
						navigationPane.SelectedPage = outlineViewerNavigationPage;
					else
						navigationPane.SelectedPage = attachmentsViewerNavigationPage;
					navigationPane.LayoutChanged();
					PdfInteractiveForm form = document.AcroForm;
					if (form != null) {
						form.FormFieldValueChanging += OnInteractiveFormFieldValueChanging;
						form.FormFieldValueChanged += OnInteractiveFormFieldValueChanged;
					}
				}
				viewer.UpdatePageView();
				if (resetRotateAndZoom) {
					RotationAngle = 0;
					ZoomMode = PdfViewer.DefaultZoomMode;
				}
				HorizontalScrollPosition = 0.5f;
				VerticalScrollPosition = 0.0f;
				HistoryController.Clear();
				RaiseDocumentChanged(stream);
			}
		}
		void SetNavigationPaneInitialVisibility(PdfNavigationPaneVisibility visibility) {
			if (!attachmentsViewerNavigationPage.PageVisible && !outlineViewerNavigationPage.PageVisible)
				NavigationPaneVisibility = PdfNavigationPaneVisibility.Hidden;
			else
				if (visibility == PdfNavigationPaneVisibility.Default)
					if (outlineViewerNavigationPage.PageVisible)
						NavigationPaneVisibility = PdfNavigationPaneVisibility.Visible;
					else
						NavigationPaneVisibility = PdfNavigationPaneVisibility.Collapsed;
				else
					NavigationPaneVisibility = visibility;
		}
		void SetNavigationPaneVisibility(PdfNavigationPaneVisibility visibility) {
			if (visibility != PdfNavigationPaneVisibility.Hidden) {
				splitter.Visible = true;
				navigationPane.Visible = true;
			}
			switch (visibility) {
				case PdfNavigationPaneVisibility.Expanded:
					navigationPane.State = NavigationPaneState.Expanded;
					break;
				case PdfNavigationPaneVisibility.Collapsed:
					navigationPane.State = NavigationPaneState.Collapsed;
					break;
				case PdfNavigationPaneVisibility.Hidden:
					navigationPane.Visible = false;
					splitter.Visible = false;
					navigationPane.Enabled = true;
					break;
				case PdfNavigationPaneVisibility.Visible:
					navigationPane.State = NavigationPaneState.Default;
					SetNavigationPaneDefaultSize();
					break;
			}
		}
		void SetNavigationPaneDefaultSize() {
			if (NavigationPaneWidth == 0)
				NavigationPaneWidth = Width / 5;
			int maxNavigationPaneDefaultSize = Width - NavigationPane.StickyWidth - 1;
			if (NavigationPaneWidth > maxNavigationPaneDefaultSize)
				NavigationPaneWidth = maxNavigationPaneDefaultSize;
		}
		void LoadDocument(Stream stream, string path, bool resetRotateAndZoom) {
			Cursor cursor = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				if (CanCloseDocument())
					SetDocument(PdfDocumentReader.Read(stream, detachStreamAfterLoadComplete, (n) => GetPassword(path, n)), path, stream, resetRotateAndZoom);
			}
			finally {
				Cursor.Current = cursor;
			}
		}
		void OnSaveProgressChanged(object sender, PdfProgressChangedEventArgs e) {
			SplashScreenManager splashScreenManager = SplashScreenManager.Default;
			splashScreenManager.SendCommand(WaitFormCommand.CmdId, e.ProgressValue);
			splashScreenManager.SendCommand(WaitFormCommand.CancelId, this);
			if (cancelSaveOperation)
				throw new PdfCancelSaveOperationException();
		}
		PdfObjectCollection SaveDocument(Stream stream, string path, PdfSaveOptions options) {
			PdfObjectCollection result = null;
			document.Creator = documentCreator;
			document.Producer = documentProducer;
			cancelSaveOperation = false;
			try {
				SplashScreenManager.ShowForm(this, typeof(PdfProgressForm), true, true, ParentFormState.Locked);
				document.ProgressChanged += OnSaveProgressChanged;
				result = PdfDocumentWriter.Write(stream, document, options);
			}
			finally {
				document.ProgressChanged -= OnSaveProgressChanged;
				SplashScreenManager.CloseForm();
			}
			fileSize = stream.Length;
			documentFilePath = path;
			documentFullFilePath = String.IsNullOrEmpty(documentFilePath) ? String.Empty : Path.GetFullPath(documentFilePath);
			documentStateController.IsDocumentModified = false;
			ClearDocumentStream();
			RaiseDocumentChanged(stream);
			return result;
		}
		void SaveDocument(Stream stream, PdfSaveOptions options) {
			CheckOperationAvailability();
			documentStateController.CommitCurrentEditor();
			PdfObjectCollection savedObjects = SaveDocument(stream, String.Empty, options);
			if (!detachStreamAfterLoadComplete) {
				documentStream = stream;
				document.UpdateObjects(savedObjects);
			}
		}
		void SaveDocument(string path, PdfSaveOptions options) {
			if (!String.IsNullOrEmpty(path)) {
				string tmpFilePath = null;
				bool arePathsEqual = Path.GetFullPath(path) == documentFilePath;
				bool useTempFile = File.Exists(path) || arePathsEqual;
				CheckOperationAvailability();
				documentStateController.CommitCurrentEditor();
				try {
					PdfObjectCollection savedObjects = null;
					using (Stream stream = arePathsEqual ? Stream.Null : new FileStream(path, useTempFile ? FileMode.Open : FileMode.Create, FileAccess.ReadWrite)) {
						Stream tempFileStream = null;
						if (useTempFile) {
							tmpFilePath = Path.GetTempFileName();
							new FileInfo(tmpFilePath).Attributes = FileAttributes.Temporary;
							tempFileStream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.ReadWrite);
						}
						using (Stream savedStream = tempFileStream ?? stream)
							savedObjects = SaveDocument(savedStream, path, options);
					}
					if (useTempFile)
						File.Copy(tmpFilePath, path, true);
					if (!detachStreamAfterLoadComplete) {
						documentStream = new FileStream(path, FileMode.Open, FileAccess.Read);
						savedObjects.UpdateStream(documentStream);
						document.UpdateObjects(savedObjects);
					}
				}
				catch (Exception e) {
					if (!useTempFile)
						try {
							File.Delete(path);
						}
						catch {
						}
					if (!(e is PdfCancelSaveOperationException))
						throw;
				}
				finally {
					if (useTempFile)
						try {
							File.Delete(tmpFilePath);
						}
						catch {
						}
				}
			}
		}
		void ShowFindDialog(PdfFindDialogOptions? findDialogOptions) {
			if (document != null) {
				PdfFindToolWindow findToolWindow = FindToolWindow;
				if (findDialogOptions.HasValue)
					findToolWindow.FindDialogOptions = findDialogOptions.Value;
				if (!findToolWindow.Visible && findToolWindow.CanShow)
					findToolWindow.ShowToolWindow();
			}
		}
		void MoveCaret(PdfMovementDirection direction) {
			PdfDataSelector dataSelector = DataSelector;
			if (dataSelector != null)
				dataSelector.MoveCaret(direction);
		}
		void SelectWithKeyboard(PdfMovementDirection direction) {
			PdfDataSelector dataSelector = DataSelector;
			if (dataSelector != null)
				dataSelector.SelectWithCaret(direction);
		}
		void ExecuteShortcuts(KeyEventArgs e) {
			Keys keyCode = e.KeyCode;
			switch (e.Modifiers) {
				case Keys.Control | Keys.Shift:
					switch (keyCode) {
						case Keys.Add:
							RotationAngle += 90;
							break;
						case Keys.Subtract:
							RotationAngle -= 90;
							break;
					}
					break;
				case Keys.Control:
					if (document != null)
						switch (keyCode) {
							case Keys.A:
								SelectAllText();
								break;
							case Keys.C:
							case Keys.Insert:
								if (HasSelection)
									CopyToClipboard();
								break;
						}
					break;
			}
		}
		PdfMouseAction CreateMouseAction(int clickCount, PdfDocumentPosition position, MouseButtons mouseButtons, Point point) {
			PdfMouseButton button = PdfMouseButton.None;
			switch (mouseButtons) {
				case MouseButtons.Left:
					button = PdfMouseButton.Left;
					break;
				case MouseButtons.Right:
					button = PdfMouseButton.Right;
					break;
				case MouseButtons.Middle:
					button = PdfMouseButton.Middle;
					break;
			}
			return new PdfMouseAction(position, button, ActualModifierKeys, clickCount, !viewer.ClientRectangle.Contains(point));
		}
		void HandleMouseMoving(Point point, MouseButtons mouseButtons) {
			if (document != null) {
				try {
					documentStateController.MouseMove(CreateMouseAction(clickController.ClickCount, viewer.ClientToDocument(point), mouseButtons, point));
				}
				catch { }
			}
			viewer.UpdateCursor(point);
		}
		void UpdateSkin() {
			Skin skin = PdfViewerSkins.GetSkin(LookAndFeel);
			selectionColor = skin == null ? Color.FromArgb(89, 96, 152, 192) : skin[PdfViewerSkins.Selection].Color.BackColor;
			viewer.InvalidateView();
		}
		Control FindControl(Type t) {
			Control parent = Parent;
			if (parent != null)
				foreach (Control control in parent.Controls)
					if (t.IsAssignableFrom(control.GetType()))
						return control;
			return null;
		}
		RibbonControl FindRibbon() {
			RibbonForm ribbonForm = Parent as RibbonForm;
			RibbonControl ribbon = ribbonForm == null ? null : ribbonForm.Ribbon;
			return ribbon ?? FindControl(typeof(RibbonControl)) as RibbonControl;
		}
		SecureString GetPassword(string path, int tryNumber) {
			const int maxDocumentNameLength = 25;
			string documentName = String.IsNullOrEmpty(path) ? PdfCoreLocalizer.GetString(PdfCoreStringId.DefaultDocumentName) : Path.GetFileName(path);
			if (documentName.Length > maxDocumentNameLength)
				documentName = documentName.Substring(0, maxDocumentNameLength) + "...";
			if ((PdfPasswordRequestedEventHandler)Events[passwordRequested] == null) {
				if (tryNumber > 1)
					XtraMessageBox.Show(LookAndFeel, ParentForm,
						XtraPdfViewerLocalizer.GetString(XtraPdfViewerStringId.MessageIncorrectPassword), documentName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				using (PdfPasswordForm passwordForm = new PdfPasswordForm(documentName))
					if (passwordForm.ShowDialog() == DialogResult.OK) {
						SecureString password = new SecureString();
						foreach (char c in passwordForm.Password)
							password.AppendChar(c);
						return password;
					}
				return null;
			}
			else {
				if (tryNumber > passwordAttemptsLimit)
					return null;
				PdfPasswordRequestedEventArgs args = new PdfPasswordRequestedEventArgs(documentName, tryNumber);
				OnPasswordRequested(args);
				return args.Password;
			}
		}
		void OnInteractiveFormFieldValueChanging(object sender, PdfInteractiveFormFieldValueChangingEventArgs e) {
			PdfFormFieldValueChangingEventArgs args = new PdfFormFieldValueChangingEventArgs(e.FieldName, e.OldValue, e.NewValue);
			OnFormFieldValueChanging(args);
			e.NewValue = args.NewValue;
			e.Cancel = args.Cancel;
		}
		void OnInteractiveFormFieldValueChanged(object sender, PdfInteractiveFormFieldValueChangedEventArgs e) {
			PdfFormFieldValueChangedEventArgs args = new PdfFormFieldValueChangedEventArgs(e.FieldName, e.OldValue, e.NewValue);
			OnFormFieldValueChanged(args);
		}
		void OnSelectedPageChanged(object sender, EventArgs e) {
			OnCurrentPageChanged(new PdfCurrentPageChangedEventArgs(CurrentPageNumber, PageCount));
			RaiseUpdateUI();
		}
		void OnZoomChanged(object sender, EventArgs e) {
			float newZoom = viewer.Zoom;
			if (newZoom != actualZoom) {
				actualZoom = newZoom;
				pageCache.Clear();
			}
			zoomMode = viewer.ZoomMode;
			RegisterCurrentDocumentViewState(PdfNavigationMode.Zoom);
			OnZoomChanged(new PdfZoomChangedEventArgs(ZoomFactor, ZoomMode));
			PdfDataSelector dataSelector = DataSelector;
			if (dataSelector != null)
				dataSelector.SetZoomFactor(newZoom);
			RaiseUpdateUI();
		}
		void OnViewerContextMenu(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				BarItemLinkCollection links = popupMenu.ItemLinks;
				popupMenu.ClearLinks();
				popupMenu.BeginInit();
				try {
					BarManager barManager = popupMenu.Manager;
					if (HasSelection && !HandTool) {
						popupMenu.AddItem(new PdfCopyCommand(this).CreateContextMenuBarItem(barManager));
					}
					else {
						popupMenu.AddItem(new PdfSelectToolCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfHandToolCommand(this).CreateContextMenuBarItem(barManager));
						int rotateGroupShift = 0;
						PdfDocumentViewStateHistoryController historyController = HistoryController;
						if (historyController.IsTherePreviousState) {
							rotateGroupShift++;
							popupMenu.AddItem(new PdfPreviousViewCommand(this).CreateContextMenuBarItem(barManager));
						}
						if (historyController.IsThereNextState) {
							rotateGroupShift++;
							popupMenu.AddItem(new PdfNextViewCommand(this).CreateContextMenuBarItem(barManager));
						}
						popupMenu.AddItem(new PdfRotatePageClockwiseCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfRotatePageCounterclockwiseCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfPrintFileCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfFindTextCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfSelectAllCommand(this).CreateContextMenuBarItem(barManager));
						popupMenu.AddItem(new PdfShowDocumentPropertiesCommand(this).CreateContextMenuBarItem(barManager));
						links[2].BeginGroup = true;
						if (rotateGroupShift > 0)
							links[rotateGroupShift + 2].BeginGroup = true;
						links[rotateGroupShift + 4].BeginGroup = true;
						links[rotateGroupShift + 6].BeginGroup = true;
						links[rotateGroupShift + 7].BeginGroup = true;
					}
				}
				finally {
					popupMenu.EndInit();
				}
				OnPopupMenuShowing(new PdfPopupMenuShowingEventArgs(popupMenu, PdfPopupMenuKind.PageContent));
				popupMenu.ShowPopup(Control.MousePosition);
			}
		}
		void OnViewerClick(object sender, EventArgs e) {
			OnClick(e);
		}
		void OnViewerDoubleClick(object sender, EventArgs e) {
			OnDoubleClick(e);
		}
		void OnViewerKeyDown(object sender, KeyEventArgs e) {
			ExecuteShortcuts(e);
		}
		void OnViewControlGotFocus(object sender, EventArgs e) {
			viewer.InvalidateView();
		}
		[SecuritySafeCritical]
		void OnViewControlLostFocus(object sender, EventArgs e) {
			CaretInterop.DestroyCaret();
		}
		void OnViewControlMouseMove(object sender, MouseEventArgs e) {
			Point location = e.Location;
			HandleMouseMoving(location, e.Button);
			OnMouseMove(e);
		}
		void OnViewControlMouseEnter(object sender, EventArgs e) {
			OnMouseEnter(e);
		}
		void OnViewControlMouseHover(object sender, EventArgs e) {
			OnMouseHover(e);
		}
		void OnViewControlMouseLeave(object sender, EventArgs e) {
			OnMouseLeave(e);
		}
		void OnViewControlMouseDown(object sender, MouseEventArgs e) {
			if (document != null) {
				clickController.Click(e);
				try {
					documentStateController.MouseDown(CreateMouseAction(clickController.ClickCount, viewer.ClientToDocument(e.Location), e.Button, e.Location));
				}
				catch { }
			}
			OnMouseDown(e);
		}
		void OnViewControlMouseUp(object sender, MouseEventArgs e) {
			Point location = e.Location;
			if (document != null) {
				try {
					documentStateController.MouseUp(CreateMouseAction(clickController.ClickCount, viewer.ClientToDocument(location), e.Button, location));
				}
				catch { }
			}
			viewer.UpdateCursor(location);
			OnMouseUp(e);
		}
		void OnViewControlMouseCaptureChanged(object sender, EventArgs e) {
			OnMouseCaptureChanged(e);
		}
		void OnViewControlMouseClick(object sender, MouseEventArgs e) {
			OnMouseClick(e);
		}
		void OnViewControlMouseDoubleClick(object sender, MouseEventArgs e) {
			OnMouseDoubleClick(e);
		}
		void OnViewControlKeyDown(object sender, KeyEventArgs e) {
			OnKeyDown(e);
			if (!e.Handled) {
				if (e.Modifiers == Keys.Alt)
					switch (e.KeyCode) {
						case Keys.Left:
							HistoryController.GoToPreviousState();
							break;
						case Keys.Right:
							HistoryController.GoToNextState();
							break;
					}
				else {
					ExecuteShortcuts(e);
					if (documentState != null && !HandTool) {
						if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
							documentStateController.SubmitFocus();
						if (HasCaret)
							try {
								switch (e.Modifiers) {
									case Keys.Control:
										switch (e.KeyCode) {
											case Keys.Right:
												MoveCaret(PdfMovementDirection.NextWord);
												break;
											case Keys.Left:
												MoveCaret(PdfMovementDirection.PreviousWord);
												break;
											case Keys.Home:
												MoveCaret(PdfMovementDirection.DocumentStart);
												break;
											case Keys.End:
												MoveCaret(PdfMovementDirection.DocumentEnd);
												break;
										}
										break;
									case Keys.Control | Keys.Shift:
										switch (e.KeyCode) {
											case Keys.Right:
												SelectWithKeyboard(PdfMovementDirection.NextWord);
												break;
											case Keys.Left:
												SelectWithKeyboard(PdfMovementDirection.PreviousWord);
												break;
											case Keys.Home:
												SelectWithKeyboard(PdfMovementDirection.DocumentStart);
												break;
											case Keys.End:
												SelectWithKeyboard(PdfMovementDirection.DocumentEnd);
												break;
											case Keys.A:
												ClearSelection();
												break;
										}
										break;
									case Keys.Shift:
										switch (e.KeyCode) {
											case Keys.Down:
												SelectWithKeyboard(PdfMovementDirection.Down);
												break;
											case Keys.Up:
												SelectWithKeyboard(PdfMovementDirection.Up);
												break;
											case Keys.Left:
												SelectWithKeyboard(PdfMovementDirection.Left);
												break;
											case Keys.Right:
												SelectWithKeyboard(PdfMovementDirection.Right);
												break;
											case Keys.Home:
												SelectWithKeyboard(PdfMovementDirection.LineStart);
												break;
											case Keys.End:
												SelectWithKeyboard(PdfMovementDirection.LineEnd);
												break;
										}
										break;
									case Keys.None:
										switch (e.KeyCode) {
											case Keys.Down:
												MoveCaret(PdfMovementDirection.Down);
												break;
											case Keys.Up:
												MoveCaret(PdfMovementDirection.Up);
												break;
											case Keys.Left:
												MoveCaret(PdfMovementDirection.Left);
												break;
											case Keys.Right:
												MoveCaret(PdfMovementDirection.Right);
												break;
											case Keys.Home:
												MoveCaret(PdfMovementDirection.LineStart);
												break;
											case Keys.End:
												MoveCaret(PdfMovementDirection.LineEnd);
												break;
											case Keys.Escape:
												PdfDataSelector dataSelector = DataSelector;
												if (dataSelector != null)
													dataSelector.HideCaret();
												break;
										}
										break;
								}
							}
							catch { }
					}
				}
				RaiseUpdateUI();
			}
		}
		void OnViewControlKeyUp(object sender, KeyEventArgs e) {
			OnKeyUp(e);
		}
		void OnViewControlKeyPress(object sender, KeyPressEventArgs e) {
			OnKeyPress(e);
		}
		void OnStyleChanged(object sender, EventArgs e) {
			UpdateSkin();
		}
		void OnFindDialogVisibleChanged(object sender, EventArgs e) {
			RaiseFindDialogVisibilityChanged();
		}
		void OnSelectedPageChanged(object sender, SelectedPageChangedEventArgs e) {
			EnsureAttachments();
		}
		void EnsureAttachments() {
			if (navigationPane.Visible && navigationPane.SelectedPage == attachmentsViewerNavigationPage && navigationPaneVisibility != PdfNavigationPaneVisibility.Collapsed)
				attachmentsViewerControl.EnsureAttachments();
		}
		void OnNavigationPaneStateChanged(object sender, StateChangedEventArgs e) {
			splitter.Visible = true;
			if (!CanSetNavigationPaneVisibility && e.OldState == NavigationPaneState.Expanded
			   && (e.State == NavigationPaneState.Default || e.State == NavigationPaneState.Regular)) {
				navigationPane.State = NavigationPaneState.Collapsed;
			}
			else {
				switch (e.State) {
					case NavigationPaneState.Expanded:
						navigationPaneVisibility = PdfNavigationPaneVisibility.Expanded;
						EnsureAttachments();
						break;
					case NavigationPaneState.Collapsed:
						navigationPaneVisibility = PdfNavigationPaneVisibility.Collapsed;
						splitter.Visible = false;
						navigationPane.Enabled = CanSetNavigationPaneVisibility;
						break;
					case NavigationPaneState.Default:
					case NavigationPaneState.Regular:
						navigationPaneVisibility = PdfNavigationPaneVisibility.Visible;
						SetNavigationPaneDefaultSize();
						EnsureAttachments();
						break;
				}
				RaiseNavigationPaneVisibilityChanged();
			}
		}
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
		public virtual void CopyToClipboard() {
			PdfSelection selection = Selection;
			PdfTextSelection textSelection = selection as PdfTextSelection;
			if (textSelection != null) {
				string text = textSelection.Text;
				if (String.IsNullOrEmpty(text))
					Clipboard.Clear();
				else
					Clipboard.SetText(text);
			}
			Bitmap selectedBitmap = SelectedBitmap;
			if (selectedBitmap != null) {
				Clipboard.SetImage(selectedBitmap);
				selectedBitmap.Dispose();
			}
		}
		protected internal virtual void OnReferencedDocumentOpening(PdfReferencedDocumentOpeningEventArgs e) {
			PdfReferencedDocumentOpeningEventHandler handler = (PdfReferencedDocumentOpeningEventHandler)Events[referencedDocumentOpening];
			if (handler != null)
				handler(this, e);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			RaiseUpdateUI();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (navigationPane != null && navigationPane.State != NavigationPaneState.Collapsed && navigationPane.Width > 0 && Width > 0) {
				if (Width <= NavigationPaneMinWidth)
					navigationPane.State = NavigationPaneState.Collapsed;
				else if (navigationPane.Width >= Width - NavigationPane.StickyWidth)
					navigationPane.State = NavigationPaneState.Expanded;
			}
			if (CanSetNavigationPaneVisibility)
				navigationPane.Enabled = true;
			else if (navigationPane.State == NavigationPaneState.Collapsed)
				navigationPane.Enabled = false;
			navigationPane.LayoutChanged();
		}
		protected virtual void OnPasswordRequested(PdfPasswordRequestedEventArgs e) {
			PdfPasswordRequestedEventHandler handler = (PdfPasswordRequestedEventHandler)Events[passwordRequested];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnDocumentClosing(PdfDocumentClosingEventArgs e) {
			PdfDocumentClosingEventHandler handler = (PdfDocumentClosingEventHandler)Events[documentClosing];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnDocumentChanged(PdfDocumentChangedEventArgs e) {
			PdfDocumentChangedEventHandler handler = (PdfDocumentChangedEventHandler)Events[documentChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnCurrentPageChanged(PdfCurrentPageChangedEventArgs e) {
			PdfCurrentPageChangedEventHandler handler = (PdfCurrentPageChangedEventHandler)Events[currentPageChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnZoomChanged(PdfZoomChangedEventArgs e) {
			PdfZoomChangedEventHandler handler = (PdfZoomChangedEventHandler)Events[zoomChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnSelectionStarted(PdfSelectionEventArgs e) {
			PdfSelectionPerformedEventHandler handler = (PdfSelectionPerformedEventHandler)Events[selectionStarted];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnSelectionContinued(PdfSelectionEventArgs e) {
			PdfSelectionPerformedEventHandler handler = (PdfSelectionPerformedEventHandler)Events[selectionContinued];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnSelectionEnded(PdfSelectionEventArgs e) {
			PdfSelectionPerformedEventHandler handler = (PdfSelectionPerformedEventHandler)Events[selectionEnded];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnFindDialogVisibilityChanged(PdfFindDialogVisibilityChangedEventArgs e) {
			PdfFindDialogVisibilityChangedEventHandler handler = (PdfFindDialogVisibilityChangedEventHandler)Events[findDialogVisibilityChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPageSetupDialogShowing(PdfPageSetupDialogShowingEventArgs e) {
			PdfPageSetupDialogShowingEventHandler handler = (PdfPageSetupDialogShowingEventHandler)Events[pageSetupDialogShowing];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnPopupMenuShowing(PdfPopupMenuShowingEventArgs e) {
			PdfPopupMenuShowingEventHandler handler = (PdfPopupMenuShowingEventHandler)Events[popupMenuShowing];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnScrollPositionChanged(PdfScrollPositionChangedEventArgs e) {
			PdfScrollPositionChangedEventHandler handler = (PdfScrollPositionChangedEventHandler)Events[scrollPositionChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnUriOpening(PdfUriOpeningEventArgs e) {
			PdfUriOpeningEventHandler handler = (PdfUriOpeningEventHandler)Events[uriOpening];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnFileAttachmentOpening(PdfFileAttachmentOpeningEventArgs e) {
			PdfFileAttachmentOpeningEventHandler handler = (PdfFileAttachmentOpeningEventHandler)Events[fileAttachmentOpening];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnNavigationPaneVisibilityChanged(PdfNavigationPaneVisibilityChangedEventArgs e) {
			PdfNavigationPaneVisibilityChangedEventHandler handler = (PdfNavigationPaneVisibilityChangedEventHandler)Events[navigationPaneVisibilityChanged];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnFormFieldValueChanging(PdfFormFieldValueChangingEventArgs e) {
			PdfFormFieldValueChangingEventHandler handler = (PdfFormFieldValueChangingEventHandler)Events[formFieldValueChanging];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnFormFieldValueChanged(PdfFormFieldValueChangedEventArgs e) {
			PdfFormFieldValueChangedEventHandler handler = (PdfFormFieldValueChangedEventHandler)Events[formFieldValueChanged];
			if (handler != null)
				handler(this, e);
		}
		[SecuritySafeCritical]
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				UnloadDocument(true);
				clickController.Dispose();
				viewerController.Dispose();
				viewer.SelectedPageChanged -= new EventHandler(OnSelectedPageChanged);
				viewer.ZoomChanged -= new EventHandler(OnZoomChanged);
				viewer.MouseUp -= new MouseEventHandler(OnViewerContextMenu);
				viewer.Click -= new EventHandler(OnViewerClick);
				viewer.DoubleClick -= new EventHandler(OnViewerDoubleClick);
				viewer.KeyDown -= new KeyEventHandler(OnViewerKeyDown);
				ViewControl viewControl = viewer.ViewControl;
				viewControl.GotFocus -= new EventHandler(OnViewControlGotFocus);
				viewControl.LostFocus -= new EventHandler(OnViewControlLostFocus);
				viewControl.MouseMove -= new MouseEventHandler(OnViewControlMouseMove);
				viewControl.MouseEnter -= new EventHandler(OnViewControlMouseEnter);
				viewControl.MouseHover -= new EventHandler(OnViewControlMouseHover);
				viewControl.MouseLeave -= new EventHandler(OnViewControlMouseLeave);
				viewControl.MouseDown -= new MouseEventHandler(OnViewControlMouseDown);
				viewControl.MouseUp -= new MouseEventHandler(OnViewControlMouseUp);
				viewControl.MouseCaptureChanged -= new EventHandler(OnViewControlMouseCaptureChanged);
				viewControl.MouseClick -= new MouseEventHandler(OnViewControlMouseClick);
				viewControl.MouseDoubleClick -= new MouseEventHandler(OnViewControlMouseDoubleClick);
				viewControl.KeyDown -= new KeyEventHandler(OnViewControlKeyDown);
				viewControl.KeyUp -= new KeyEventHandler(OnViewControlKeyUp);
				viewControl.KeyPress -= new KeyPressEventHandler(OnViewControlKeyPress);
				UserLookAndFeel lookAndFeel = LookAndFeel;
				if (lookAndFeel != null)
					lookAndFeel.StyleChanged -= new EventHandler(OnStyleChanged);
				barAndDockingController.Dispose();
				popupMenu.Dispose();
				fontStorage.Dispose();
				DestroyFindDialog();
				outlineViewerControl.Dispose();
				attachmentsViewerControl.Dispose();
				navigationPane.StateChanged -= new StateChangedEventHandler(OnNavigationPaneStateChanged);
				navigationPane.SelectedPageChanged -= new SelectedPageChangedEventHandler(OnSelectedPageChanged);
				navigationPane.Dispose();
				components.Dispose();
			}
		}
		PdfDocumentProcessorHelper IPdfViewer.GetDocumentProcessorHelper() {
			return documentState == null ? null : new PdfDocumentProcessorHelper(SaveDocument, SaveDocument, documentState.FormData);
		}
		#region ICommandAwareControl<PdfCommandId> implementation
		CommandBasedKeyboardHandler<PdfViewerCommandId> ICommandAwareControl<PdfViewerCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<PdfViewerCommandId>.BeforeDispose {
			add { onBeforeDispose += value; }
			remove { onBeforeDispose -= value; }
		}
		event EventHandler ICommandAwareControl<PdfViewerCommandId>.UpdateUI {
			add { onUpdateUI += value; }
			remove { onUpdateUI -= value; }
		}
		Command ICommandAwareControl<PdfViewerCommandId>.CreateCommand(PdfViewerCommandId id) {
			return commands.CreateCommand(id);
		}
		bool ICommandAwareControl<PdfViewerCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<PdfViewerCommandId>.CommitImeContent() {
		}
		void ICommandAwareControl<PdfViewerCommandId>.Focus() {
			Focus();
		}
		#endregion
	}
}
