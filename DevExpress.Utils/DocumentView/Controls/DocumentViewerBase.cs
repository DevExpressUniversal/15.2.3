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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Preview;
using DevExpress.Skins;
using System.Collections.Generic;
namespace DevExpress.DocumentView.Controls {
	[ToolboxItem(false)]
	public class DocumentViewerBase : System.Windows.Forms.UserControl, ISupportLookAndFeel {
#if DEBUGTEST
		[Browsable(false)]
		public ScrollController Test_ScrollController { get { return scrollController; } }
		[Browsable(false)]
		public PageMarginList Test_Margins {
			get { return margins; }
		}
		[Browsable(false)]
		public ViewControl Test_ViewControl {
			get { return viewControl; }
		}
		[Browsable(false)]
		public bool IsMetricValueAssigned { get; private set; }
#endif
		#region inner classes
		class BackgroundService : IBackgroundService {
			public virtual void PerformAction() {
				Application.DoEvents();
			}
		}
		class WaitService : IWaitIndicator {
			object result = null;
			Action<Cursor> setCursor;
			public WaitService(Action<Cursor> setCursor) {
				this.setCursor = setCursor;
			}
			object IWaitIndicator.Show(string description) {
				if(result == null) {
					result = new object();
					setCursor(Cursors.WaitCursor);
					return result;
				}
				return null;
			}
			bool IWaitIndicator.Hide(object result) {
				if(this.result != null && ReferenceEquals(this.result, result)) {
					this.result = null;
					setCursor(Cursors.Default);
					return true;
				}
				return false;
			}
		}
		class EventLogger : IDisposable {
			Action<EventHandler> unsubscr;
			public bool EventLogged { get; private set; }
			public EventLogger(Action<EventHandler> subscr, Action<EventHandler> unsubscr) {
				EventLogged = false;
				this.unsubscr = unsubscr;
				subscr(Handler);
			}
			void Handler(object sender, EventArgs e) {
				EventLogged = true;
			}
			void IDisposable.Dispose() {
				unsubscr(Handler);
			}
		}
		abstract class PageViewerBase {
			public static PageViewerBase CreateInstance(DocumentViewerBase printControl, PageViewModes mode) {
				switch(mode) {
					case PageViewModes.PageWidth:
						return new PageWidthViewer(printControl);
					case PageViewModes.TextWidth:
						return new PageTextViewer(printControl);
					case PageViewModes.RowColumn:
						return new PageRowColumnViewer(printControl);
					case PageViewModes.Zoom:
						return new PageZoomViewer(printControl);
				}
				System.Diagnostics.Debug.Fail("Instance of PageViewerBase must be created");
				return null;
			}
			protected DocumentViewerBase printControl;
			protected ViewManager viewManager;
			public abstract PageViewModes ViewMode { get; }
			public PageViewerBase(DocumentViewerBase printControl) {
				this.printControl = printControl;
				this.viewManager = printControl.viewManager;
			}
			public abstract void SetPageView();
			public virtual void UpdateView() {
			}
		}
		class PageWidthViewer : PageViewerBase {
			public override PageViewModes ViewMode { get { return PageViewModes.PageWidth; } }
			public PageWidthViewer(DocumentViewerBase printControl)
				: base(printControl) {
			}
			public override void SetPageView() {
				float zoom = viewManager.GetMaxPageWidthZoomFactor();
				printControl.SetZoomFactor(zoom, true);
			}
		}
		class PageTextViewer : PageViewerBase {
			public override PageViewModes ViewMode { get { return PageViewModes.TextWidth; } }
			public PageTextViewer(DocumentViewerBase printControl)
				: base(printControl) {
			}
			public override void SetPageView() {
				RectangleF widestUsefulPageRect = viewManager.CalcWidestUsefulPageRect();
				float zoom = viewManager.GetWidthZoomFactor(widestUsefulPageRect.Width);
				printControl.SetZoomFactor(zoom, true);
				viewManager.SetHorizScroll(widestUsefulPageRect.Left);
			}
		}
		class PageZoomViewer : PageViewerBase {
			public override PageViewModes ViewMode { get { return PageViewModes.Zoom; } }
			public PageZoomViewer(DocumentViewerBase printControl)
				: base(printControl) {
			}
			public override void SetPageView() {
				printControl.SetZoomFactor(printControl.zoom, false);
			}
			public override void UpdateView() {
				printControl.UpdateViewRowsAndColumns(printControl.Zoom);
			}
		}
		class PageRowColumnViewer : PageViewerBase {
			public override PageViewModes ViewMode { get { return PageViewModes.RowColumn; } }
			public PageRowColumnViewer(DocumentViewerBase printControl)
				: base(printControl) {
			}
			public override void SetPageView() {
				printControl.SetPageView(printControl.viewColumns, printControl.viewRows);
			}
		}
		class GestureClient : IGestureClient {
			DocumentViewerBase control;
			Point localOverpan = Point.Empty;
			public GestureClient(DocumentViewerBase control) {
				this.control = control;
			}
			GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
				bool verticalScrollEnabled = ((DevExpress.XtraEditors.ScrollBarBase)(control.vScrollBar)).Enabled;
				GestureAllowArgs pan = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN + (verticalScrollEnabled ? GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY : 0) + GestureHelper.GC_PAN_WITH_GUTTER + GestureHelper.GC_PAN_WITH_INERTIA };
				return new GestureAllowArgs[] { pan, GestureAllowArgs.Zoom };
			}
			IntPtr IGestureClient.Handle {
				get { return control.ViewControl.Handle; }
			}
			void IGestureClient.OnBegin(GestureArgs info) {
				localOverpan = Point.Empty;
			}
			void IGestureClient.OnEnd(GestureArgs info) {
			}
			void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
					PointF scrollPos = control.ViewManager.ScrollPos;
					control.HandleDragging(-delta.X, -delta.Y);
					localOverpan.X = scrollPos.X == control.ViewManager.ScrollPos.X ? localOverpan.X + delta.X : 0;
					localOverpan.Y = scrollPos.Y == control.ViewManager.ScrollPos.Y ? localOverpan.Y + delta.Y : 0;
					overPan = localOverpan;
			}
			void IGestureClient.OnPressAndTap(GestureArgs info) {
			}
			void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
			}
			void IGestureClient.OnTwoFingerTap(GestureArgs info) {
			}
			void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
				control.Zoom += (float)zoomDelta - 1.0f;
			}
			IntPtr IGestureClient.OverPanWindowHandle {
				get { return GestureHelper.FindOverpanWindow(control); }
			}
			Point IGestureClient.PointToClient(Point p) {
				return control.ViewControl.PointToClient(p);
			}
		}
		#endregion
		#region Fields & Properties
		protected bool disposeRunning;
		PageMarginList margins;
		ViewControl viewControl;
		IPopupForm popupForm;
		float zoom = 1.0f;
		int viewRows = 1;
		int viewColumns = 1;
		GdiHashtable gdi;
		bool autoZoom;
		bool handTool;
		bool showPageMargins = true;
		bool continuous = true;
		bool isMetric = DevExpress.Utils.RegionalSettings.IsMetric;
		Point prevMousePos = Point.Empty;
		protected DevExpress.XtraEditors.HScrollBar hScrollBar;
		protected DevExpress.XtraEditors.VScrollBar vScrollBar;
		SimpleControl cornerPanel;
		protected Panel bottomPanel;
		string status = "";
		protected float fMinZoom = 0.1f;
		protected float fMaxZoom = 5f;
		BorderStyle borderStyle = BorderStyle.None;
		PageViewerBase pageViewer;
		ViewManager viewManager;
		ControlUserLookAndFeel userLookAndFeel;
		ScrollController scrollController;
		PrintControlInfo printControlInfo;
		bool verticalScrollLocked;
		protected IContainer components = new Container();
		IDocument document;
		object waitResult = null;
		protected Font BackgroundFont {
			get { return printControlInfo.BackgroundFont; }
		}
		protected Color BackgroundForeColor {
			get {
				return printControlInfo.BackgroundForeColor;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public IDocument Document {
			get {
				return document;
			}
			set {
				BeforeSetDocument();
				document = value;
				AfterSetDocument();
			}
		}
		protected virtual void BeforeSetDocument() {
			if(document != null)
				UnbindPrintingSystem();
		}
		protected virtual void AfterSetDocument() {
			if(document == null)
				InvalidateViewControl();
			else {
				HideWaitIndicator();
				OnDocumentChanged(EventArgs.Empty);
				OnZoomChanged(EventArgs.Empty);
				UpdateScrollBars();
				UpdatePageView();
				BindPrintingSystem();
				UpdateCommands();
			}
			if(CanSelectViewControl)
				ViewControl.Select();
		}
#pragma warning disable 1691
#pragma warning disable 809
#pragma warning restore 1691
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property value is ignored. To customize the PrintControl's appearance, use custom skins.")
		]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property value is ignored. To customize the PrintControl's appearance, use custom skins.")
		]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This event isn't called at all.")
		]
		protected override void OnBackgroundImageChanged(EventArgs e) { }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This event isn't called at all."),
		]
		protected override void OnBackgroundImageLayoutChanged(EventArgs e) { }
#pragma warning disable 1691
#pragma warning restore 809
#pragma warning restore 1691
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageLayoutChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageChanged { add { } remove { } }
		[
		Category(NativeSR.CatAppearance),
		]
		public int PageBorderWidth {
			get { return ControlInfo.PageBorderWidth; }
			set {
				if(ControlInfo.PageBorderWidth == value) return;
				ControlInfo.PageBorderWidth = value;
				RecreateHandle();
			}
		}
		[
		Category(NativeSR.CatAppearance),
		]
		public Color PageBorderColor {
			get { return ControlInfo.PageBorderColor; }
			set {
				if(ControlInfo.PageBorderColor == value) return;
				ControlInfo.PageBorderColor = value;
				RecreateHandle();
			}
		}
		[
		Category(NativeSR.CatAppearance),
		]
		public int SelectedPageBorderWidth {
			get { return ControlInfo.SelectedPageBorderWidth; }
			set {
				if(ControlInfo.SelectedPageBorderWidth == value) return;
				ControlInfo.SelectedPageBorderWidth = value;
				RecreateHandle();
			}
		}
		[
		Category(NativeSR.CatAppearance),
		]
		public Color SelectedPageBorderColor {
			get { return ControlInfo.SelectedPageBorderColor; }
			set {
				if(ControlInfo.SelectedPageBorderColor == value) return;
				ControlInfo.SelectedPageBorderColor = value;
				RecreateHandle();
			}
		}
		[
		Category(NativeSR.CatAppearance)
		]
		public PageBorderVisibility PageBorderVisibility {
			get {
				return ControlInfo.PageBorderVisibility;
			}
			set {
				ControlInfo.PageBorderVisibility = value;
				RecreateHandle();
			}
		}
		protected SkinPaddingEdges Edges {
			get { return ControlInfo.PagePaddingEdges; }
		}
		internal PrintControlInfo ControlInfo {
			get { return printControlInfo; }
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				if(borderStyle == BorderStyle.Fixed3D)
					cp.ExStyle |= Win32.WS_EX_CLIENTEDGE;
				else if(borderStyle == BorderStyle.FixedSingle)
					cp.Style |= Win32.WS_BORDER;
				return cp;
			}
		}
		[
		DefaultValue(BorderStyle.None),
		Category(NativeSR.CatAppearance),
		]
		public new BorderStyle BorderStyle {
			get {
				return borderStyle;
			}
			set {
				if(!Enum.IsDefined(typeof(BorderStyle), value)) {
					throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
				}
				if(borderStyle != value) {
					borderStyle = value;
					RecreateHandle();
				}
			}
		}
		[
		Category(NativeSR.CatPrinting),
		Localizable(true)
		]
		public bool IsMetric {
			get { return isMetric; }
			set {
#if DEBUGTEST
				IsMetricValueAssigned = true;
#endif
				isMetric = value;
			}
		}
		[Browsable(false)]
		public int MaxPageRows { get { return viewManager.GetViewRows(MinZoom); } }
		[Browsable(false)]
		public int MaxPageColumns { get { return viewManager.GetViewColumns(MinZoom); } }
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(true)
		]
		public bool ShowPageMargins {
			get { return showPageMargins; }
			set { showPageMargins = value; }
		}
		protected internal virtual bool CanChangePageMargins {
			get { return false; }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(false)
		]
		public bool AutoZoom {
			get { return autoZoom; }
			set { autoZoom = value; }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(1.0f)
		]
		public float Zoom {
			get {
				return zoom;
			}
			set {
				UpdateViewRowsAndColumns(value);
				SetViewMode(PageViewModes.Zoom);
				SetZoomFactor(value, false);
			}
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(5f),
		Browsable(false)
		]
		public float MaxZoom {
			get { return fMaxZoom; }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(0.1f),
		Browsable(false)
		]
		public float MinZoom {
			get { return fMinZoom; }
		}
		[Browsable(false)]
		public IPage SelectedPage {
			get { return viewManager.SelectedPage; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public int SelectedPageIndex {
			get { return viewManager.SelectedPagePlaceIndex; }
			set { viewManager.SelectedPagePlaceIndex = value; }
		}
		[Localizable(true), Category(NativeSR.CatAppearance)]
		public Color TooltipForeColor {
			get { return popupForm.AppearanceObject.ForeColor; }
			set { popupForm.AppearanceObject.ForeColor = value; }
		}
		[Localizable(true), Category(NativeSR.CatAppearance)]
		public Color TooltipBackColor {
			get { return popupForm.AppearanceObject.BackColor; }
			set { popupForm.AppearanceObject.BackColor = value; }
		}
		[Localizable(true), Category(NativeSR.CatAppearance)]
		public Font TooltipFont {
			get { return popupForm.AppearanceObject.Font; }
			set { popupForm.AppearanceObject.Font = value; }
		}
		[
		Localizable(true),
		Category(NativeSR.CatAppearance),
		]
		public new Color BackColor {
			get { return ControlInfo.BackColor; }
			set {
				ControlInfo.BackColor = value;
				RecreateHandle();
			}
		}
		[Localizable(true), Category(NativeSR.CatAppearance)]
		public new Color ForeColor {
			get { return ControlInfo.ForeColor; }
			set {
				ControlInfo.ForeColor = value;
				RecreateHandle();
			}
		}
		[Localizable(true), Category(NativeSR.CatAppearance), DefaultValue(true)]
		public bool ShowToolTips {
			get { return popupForm.ShowToolTips; }
			set { popupForm.ShowToolTips = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public bool HandTool {
			get { return handTool; }
			set {
				handTool = value;
				SetCursor(GetCursor(SelectedPage));
			}
		}
		[Browsable(false)]
		public int ViewColumns { get { return viewColumns; } }
		[Browsable(false)]
		public int ViewRows { get { return viewRows; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool Continuous {
			get { return continuous; }
			set {
				if(continuous != value) {
					continuous = value;
					viewManager = continuous ? new ContinuousViewManager(viewManager) : new ViewManager(viewManager);
					SetZoomFactor(zoom, false);
				}
			}
		}
		[Browsable(false)]
		public bool DocumentIsEmpty {
			get { return Document == null || Document.IsEmpty; }
		}
		[Browsable(false)]
		public bool DocumentIsCreating {
			get { return Document != null && Document.IsCreating; }
		}
		[Browsable(false)]
		public PageMarginList Margins {
			get { return margins; }
		}
		[Browsable(false)]
		public ViewControl ViewControl {
			get { return viewControl; }
		}
		[Browsable(false)]
		public ViewManager ViewManager {
			get { return viewManager; }
		}
		internal GdiHashtable Gdi {
			get { return gdi; }
		}
		[Browsable(false)]
		public IPopupForm PopupForm {
			get { return popupForm; }
		}
		[
		Category(NativeSR.CatPrinting),
		]
		public string Status {
			get {
				return !string.IsNullOrEmpty(status) ? status : PreviewStringId.Msg_EmptyDocument.GetString();
			}
			set { status = value; }
		}
		[Browsable(false)]
		public new bool DesignMode {
			get { return base.DesignMode; }
		}
		protected PageViewModes ViewMode { get { return pageViewer.ViewMode; } }
		protected virtual bool CanSelectViewControl { get { return ViewControl.CanSelect; } }
		#endregion
		#region Events
		private static readonly object SelectedPageChangedEvent = new object();
		private static readonly object DocumentChangedEvent = new object();
		private static readonly object ZoomChangedEvent = new object();
		[Category(NativeSR.CatPropertyChanged)]
		public event EventHandler SelectedPageChanged {
			add { Events.AddHandler(SelectedPageChangedEvent, value); }
			remove { Events.RemoveHandler(SelectedPageChangedEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event EventHandler DocumentChanged {
			add { Events.AddHandler(DocumentChangedEvent, value); }
			remove { Events.RemoveHandler(DocumentChangedEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event EventHandler ZoomChanged {
			add { Events.AddHandler(ZoomChangedEvent, value); }
			remove { Events.RemoveHandler(ZoomChangedEvent, value); }
		}
		protected virtual void OnBrickMouseDown(MouseEventArgs e) {
		}
		protected virtual void OnBrickMouseUp(MouseEventArgs e) {
		}
		protected virtual void OnBrickMouseMove(MouseEventArgs e) {
		}
		protected virtual void OnBrickClick(EventArgs e) {
		}
		protected virtual void OnBrickDoubleClick(EventArgs e) {
		}
		protected void OnDocumentChanged(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[DocumentChangedEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		protected virtual void OnZoomChanged(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[ZoomChangedEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		protected virtual void OnSelectedPageChanged(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[SelectedPageChangedEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		internal void RaiseSelectedPageChanged() {
			if(SelectedPage != null)
				UpdateScrollBars();
			UpdateCommands();
			OnSelectedPageChanged(EventArgs.Empty);
		}
		#endregion
		#region ISupportLookAndFeel
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		[
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public UserLookAndFeel LookAndFeel {
			get {
				if(userLookAndFeel == null) {
					userLookAndFeel = new ControlUserLookAndFeel(this);
				}
				return userLookAndFeel;
			}
		}
		#endregion //ISupportLookAndFeel
		public DocumentViewerBase() {
			InitializeComponent();
			viewManager = CreateViewManager(viewControl);
			SetViewMode(PageViewModes.Zoom, 1, 1);
			popupForm = CreatePopupForm();
			margins = new PageMarginList(this);
			gdi = new GdiHashtable();
			scrollController = CreateScrollController(hScrollBar, vScrollBar);
			SetLookAndFeel();
			LookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			printControlInfo = new PrintControlInfo(CreateBackgroundPreviewPainter(), CreatePageBorderPainter());
			printControlInfo.PageBorderVisibility = PrintControlInfo.DefaultPageBorderVisibility;
			SetCornerPanelBackground();
		}
		protected virtual ViewManager CreateViewManager(ViewControl viewControl) {
			return continuous ? new ContinuousViewManager(this, viewControl) : new ViewManager(this, viewControl);
		}
		protected virtual ScrollController CreateScrollController(DevExpress.XtraEditors.HScrollBar hScrollBar, DevExpress.XtraEditors.VScrollBar vScrollBar) {
			return new ScrollController(this, hScrollBar, vScrollBar);
		}
		protected virtual PageBorderPainter CreatePageBorderPainter() {
			return PrintingPaintStyles.GetPaintStyle(LookAndFeel).CreatePageBorderPainter(LookAndFeel);
		}
		protected virtual BackgroundPreviewPainter CreateBackgroundPreviewPainter() {
			return PrintingPaintStyles.GetPaintStyle(LookAndFeel).CreateBackgroundPreviewPainter(LookAndFeel);
		}
		void SetCornerPanelBackground() {
			cornerPanel.BackColor = GetFormBackgroundColor(cornerPanel.BackColor);
		}
		Color GetFormBackgroundColor(Color defaultColor) {
			DevExpress.Skins.Skin skin = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel);
			if(skin == null)
				return defaultColor;
			return skin[DevExpress.Skins.CommonSkins.SkinForm].Color.GetBackColor();
		}
		Control vdummy, hdummy;
		protected virtual void SetLookAndFeel() {
			vScrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			hScrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			cornerPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			ScrollBarBase.ApplyUIMode(vScrollBar);
			ScrollBarBase.ApplyUIMode(hScrollBar);
			if(ScrollBarBase.GetUIMode(vScrollBar) == ScrollUIMode.Touch) {
				vScrollBar.Visible = false;
				hScrollBar.Visible = false;
				if(!vScrollBar.IsOverlapScrollBar) {
					vdummy = new Control();
					hdummy = new Control();
					hdummy.Dock = DockStyle.Fill;
					vdummy.Dock = DockStyle.Right;
					vdummy.Width = vScrollBar.GetDefaultVerticalScrollBarWidth();
					hdummy.Height = hScrollBar.GetDefaultHorizontalScrollBarHeight();
					Controls.Add(vdummy);
					bottomPanel.Controls.Add(hdummy);
					bottomPanel.Controls.SetChildIndex(hdummy, 0);
				}
				else {
					bottomPanel.Visible = false;
					hScrollBar.Parent = this;
				}
				hScrollBar.Dock = DockStyle.None;
				vScrollBar.Dock = DockStyle.None;
				ApplyHVScrollbarBounds();
				hScrollBar.SetVisibility(true);
				vScrollBar.SetVisibility(true);
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
			ApplyHVScrollbarBounds();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			hScrollBar.OnAction(ScrollNotifyAction.MouseMove);
			vScrollBar.OnAction(ScrollNotifyAction.MouseMove);
		}
		private void ApplyHVScrollbarBounds() {
			if(IsHandleCreated)
				BeginInvoke(new MethodInvoker(() => { ApplyHVScrollbarBoundsCore(); }));
			else
				ApplyHVScrollbarBoundsCore();
		}
		private void ApplyHVScrollbarBoundsCore() {
			if(vScrollBar == null) return;
			if(ScrollBarBase.GetUIMode(vScrollBar) != ScrollUIMode.Touch) return;
			if(hdummy != null) {
				hScrollBar.Bounds = hdummy.Bounds;
				vScrollBar.Bounds = vdummy.Bounds;
				return;
			}
			System.Diagnostics.Debug.Assert(ReferenceEquals(this, ViewControl.Parent));
			Rectangle bounds = ViewControl.Bounds;
			bounds.X = bounds.Right - vScrollBar.GetDefaultVerticalScrollBarWidth();
			bounds.Width = vScrollBar.GetDefaultVerticalScrollBarWidth();
			vScrollBar.Bounds = bounds;
			bounds = ViewControl.Bounds;
			bounds.Y = bounds.Bottom - vScrollBar.GetDefaultHorizontalScrollBarHeight();
			bounds.Height = vScrollBar.GetDefaultHorizontalScrollBarHeight();
			bounds.Width -= vScrollBar.GetDefaultVerticalScrollBarWidth();
			hScrollBar.Bounds = bounds;
		}
		protected bool ShouldSerializeStatus() {
			return !string.IsNullOrEmpty(status);
		}
		protected bool ShouldSerializeBackColor() {
			return BackColor != Color.Empty;
		}
		protected bool ShouldSerializeForeColor() {
			return ForeColor != Color.Empty;
		}
		protected bool ShouldSerializePageBorderWidth() {
			return PageBorderWidth != PrintControlInfo.DefaultPageBorderWidth;
		}
		protected bool ShouldSerializePageBorderColor() {
			return PageBorderColor != PrintControlInfo.DefaultPageBorderColor;
		}
		protected bool ShouldSerializeSelectedPageBorderWidth() {
			return SelectedPageBorderWidth != PrintControlInfo.DefaultSelectedPageBorderWidth;
		}
		protected bool ShouldSerializeSelectedPageBorderColor() {
			return SelectedPageBorderColor != PrintControlInfo.DefaultSelectedPageBorderColor;
		}
		protected bool ShouldSerializePageBorderVisibility() {
			return PageBorderVisibility != PrintControlInfo.DefaultPageBorderVisibility;
		}
		bool ShouldSerializeTooltipFont() {
			return TooltipFont != AppearanceObject.DefaultFont;
		}
		bool ShouldSerializeTooltipBackColor() {
			return TooltipBackColor != Color.Empty;
		}
		bool ShouldSerializeTooltipForeColor() {
			return TooltipForeColor != Color.Empty;
		}
		protected void ResetPageBorderWidth() {
			PageBorderWidth = PrintControlInfo.NotSetWidth;
		}
		protected void ResetPageBorderColor() {
			PageBorderColor = Color.Empty;
		}
		protected void ResetSelectedPageBorderWidth() {
			SelectedPageBorderWidth = PrintControlInfo.NotSetWidth;
		}
		protected void ResetSelectedPageBorderColor() {
			SelectedPageBorderColor = Color.Empty;
		}
		protected void ResetPageBorderVisibility() {
			PageBorderVisibility = PrintControlInfo.DefaultPageBorderVisibility;
		}
		public override void ResetForeColor() {
			ForeColor = Color.Empty;
		}
		public override void ResetBackColor() {
			BackColor = Color.Empty;
		}
		void ResetTooltipFont() {
			TooltipFont = AppearanceObject.DefaultFont;
		}
		void ResetTooltipBackColor() {
			TooltipBackColor = Color.Empty;
		}
		void ResetTooltipForeColor() {
			TooltipForeColor = Color.Empty;
		}
		protected override void Dispose(bool disposing) {
			disposeRunning = true;
			try {
				if(disposing) {
					if(gdi != null) {
						gdi.Dispose();
						gdi = null;
					}
					if(Document != null) {
						UnbindPrintingSystem();
						Document = null;
					}
					if(popupForm != null && popupForm is IDisposable) {
						((IDisposable)popupForm).Dispose();
						popupForm = null;
					}
					if(userLookAndFeel != null) {
						userLookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
						userLookAndFeel.Dispose();
						userLookAndFeel = null;
					}
					if(scrollController != null) {
						scrollController.Dispose();
						scrollController = null;
					}
					if(components != null) {
						components.Dispose();
						components = null;
					}
					if(printControlInfo != null) {
						printControlInfo.Dispose();
						printControlInfo = null;
					}
				}
				base.Dispose(disposing);
			} finally {
				disposeRunning = false;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			if(!disposeRunning) {
				base.OnHandleCreated(e);
				UpdateCommands();
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			SetFocus();
		}
		protected virtual IPopupForm CreatePopupForm() {
			return new PopupForm(this);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			const int clientWidth = 472;
			const int clientHeight = 320;
			const int progressBarHeight = 8;
			this.viewControl = new ViewControl(this, new GestureClient(this));
			this.hScrollBar = new DevExpress.XtraEditors.HScrollBar();
			this.vScrollBar = new DevExpress.XtraEditors.VScrollBar();
			this.cornerPanel = new SimpleControl();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			this.viewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewControl.Size = new System.Drawing.Size(clientWidth, clientHeight);
			this.viewControl.TabIndex = 0;
			this.viewControl.TabStop = false;
			this.viewControl.Click += new System.EventHandler(this.view_Click);
			this.viewControl.Resize += new System.EventHandler(this.view_Resize);
			this.viewControl.Paint += new System.Windows.Forms.PaintEventHandler(this.view_Paint);
			this.viewControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnViewKeyDown);
			this.viewControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnViewKeyUp);
			this.viewControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.view_OnMouseUp);
			this.viewControl.DoubleClick += new System.EventHandler(this.view_OnDoubleClick);
			this.viewControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.view_OnMouseMove);
			this.viewControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.view_OnMouseDown);
			this.hScrollBar.Dock = DockStyle.Fill;
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.TabIndex = 1;
			this.hScrollBar.TabStop = false;
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(clientWidth, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(SystemInformation.VerticalScrollBarWidth, clientHeight);
			this.vScrollBar.TabIndex = 2;
			this.vScrollBar.TabStop = false;
			this.cornerPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.cornerPanel.Name = "hPanel";
			this.cornerPanel.Size = new System.Drawing.Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
			this.cornerPanel.TabIndex = 3;
			this.cornerPanel.TabStop = false;
			this.bottomPanel.BorderStyle = BorderStyle.None;
			this.bottomPanel.Controls.Add(this.hScrollBar);
			this.bottomPanel.Controls.Add(this.cornerPanel);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, clientHeight);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(clientWidth + SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
			this.bottomPanel.TabIndex = 4;
			this.AutoScaleMode = AutoScaleMode.None;
			this.Controls.Add(this.viewControl);
			this.Controls.Add(this.vScrollBar);
			this.Controls.Add(this.bottomPanel);
			this.Name = "PrintControl";
			this.Size = new System.Drawing.Size(clientWidth + SystemInformation.VerticalScrollBarWidth, clientHeight + progressBarHeight + SystemInformation.HorizontalScrollBarHeight);
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected void InvalidateViewControl() {
			viewControl.Invalidate();
		}
		protected virtual bool HandleKey(Keys key) {
			if(scrollController.HandleKey(key))
				return true;
			if(key == Keys.Escape)
				viewControl.EndMarginResize(false);
			return false;
		}
		protected virtual void UpdateCommands() {
		}
		protected virtual void BindPrintingSystem() {
			Document.Disposed += new EventHandler(OnDocumentDisposed);
			Document.PageBackgrChanged += new EventHandler(OnPageBackgrChanged);
			Document.DocumentChanged += new EventHandler(ps_DocumentChanged);
			Document.BeforeBuildPages += new EventHandler(ps_BeforeBuildPages);
			Document.AfterBuildPages += new EventHandler(ps_AfterBuildPages);
			Document.CreateDocumentException += new ExceptionEventHandler(ps_CreateDocumentException);
			Document.ReplaceService<IWaitIndicator>(new WaitService(SetCursor));
			Document.ReplaceService<IBackgroundService>(new BackgroundService());
		}
		protected virtual void OnDocumentDisposed(object sender, EventArgs e) {
			UnbindPrintingSystem();
			Document = null;
		}
		protected virtual void UnbindPrintingSystem() {
			Document.PageBackgrChanged -= new EventHandler(OnPageBackgrChanged);
			Document.Disposed -= new EventHandler(OnDocumentDisposed);
			Document.DocumentChanged -= new EventHandler(ps_DocumentChanged);
			Document.BeforeBuildPages -= new EventHandler(ps_BeforeBuildPages);
			Document.AfterBuildPages -= new EventHandler(ps_AfterBuildPages);
			Document.CreateDocumentException -= new ExceptionEventHandler(ps_CreateDocumentException);
			HideWaitIndicator();
			Document.RemoveService(typeof(IWaitIndicator));
			Document.RemoveService(typeof(IBackgroundService));
		}
		private void SetPageView() {
			System.Diagnostics.Debug.Assert(pageViewer != null);
			pageViewer.SetPageView();
			UpdateScrollBars();
		}
		public void SetPageView(PageViewModes viewMode) {
			SetViewMode(viewMode, 1, 1);
			SetPageView();
			viewManager.ShowSelectedPage();
		}
		public void ViewWholePage() {
			float zoom = viewManager.GetZoomFactor(1, 1);
			SetZoomFactor(zoom, false);
		}
		public void SelectFirstPage() {
			if(Document.Pages.Count > 0)
				viewManager.SetSelectedIndex(0, true);
			else
				viewManager.SelectedPagePlaceIndex = 0;
		}
		public void SelectLastPage() {
			viewManager.SelectedPagePlaceIndex = 100000;
		}
		public void SelectPrevPage() {
			viewManager.SelectedPagePlaceIndex--;
		}
		public void SelectNextPage() {
			viewManager.SelectedPagePlaceIndex++;
		}
		public void ScrollPageUp() {
			scrollController.LargeDecrementVertScroll();
		}
		public void ScrollPageDown() {
			scrollController.LargeIncrementVertScroll();
		}
		public void SetCursor(Cursor cursor) {
			if(viewControl.Cursor.Equals(cursor) == false)
				viewControl.Cursor = cursor;
		}
		public void SetPageView(int columns, int rows) {
			System.Diagnostics.Debug.Assert(pageViewer != null);
			SetViewMode(PageViewModes.RowColumn);
			if(columns <= 0 || rows <= 0)
				return;
			bool keepVerticalScroll = viewColumns == columns;
			viewColumns = Math.Min(columns, MaxPageColumns);
			viewRows = Math.Min(rows, MaxPageRows);
			float zoom = viewManager.GetZoomFactor(viewColumns, viewRows);
			SetZoomFactor(zoom, keepVerticalScroll);
		}
		private void ps_BeforeBuildPages(Object sender, EventArgs e) {
			viewManager.Reset();
			SetCursor(Cursors.Default);
			ShowWaitIndicator();
		}
		protected void ShowWaitIndicator() {
			object result = Document.GetService<IWaitIndicator>().TryShow(PreviewStringId.Msg_CreatingDocument.GetString());
			if(result != null) waitResult = result;
		}
		protected void HideWaitIndicator() {
			if(Document.GetService<IWaitIndicator>().TryHide(waitResult))
				waitResult = null;
		}
		private void ps_AfterBuildPages(Object sender, EventArgs e) {
			try {
				verticalScrollLocked = true;
				UpdateEverything();
			} finally {
				verticalScrollLocked = false;
			}
			HideWaitIndicator();
			OnDocumentChanged(EventArgs.Empty);
		}
		void ps_CreateDocumentException(object sender, ExceptionEventArgs args) {
			HideWaitIndicator();
			if(args.Handled)
				return;
			args.Handled = true;
			ShowException(args.Exception);
		}
		protected virtual void OnCommandVisibilityChanged(Object sender, EventArgs e) {
			UpdateCommands();
		}
		private void ps_DocumentChanged(object sender, EventArgs e) {
			try {
				verticalScrollLocked = DocumentIsCreating;
				UpdateEverything();
			} finally {
				verticalScrollLocked = false;
			}
			if(DocumentIsCreating && !DocumentIsEmpty)
				HideWaitIndicator();
			OnDocumentChanged(EventArgs.Empty);
		}
		protected virtual void UpdateEverything() {
			pageViewer.UpdateView();
			UpdatePageView();
			UpdateCommands();
		}
		protected virtual void OnPageBackgrChanged(Object sender, EventArgs e) {
			InvalidateViewControl();
		}
		public void ZoomIn() {
			Zoom = GetZoomFactor(1);
		}
		public void ZoomOut() {
			Zoom = GetZoomFactor(-1);
		}
		private float GetZoomFactor(float sign) {
			float val = (float)Math.Round(Zoom * 100f);
			float offset = (val < 100) ? 5 : 10;
			val += Math.Sign(sign) * offset;
			val = (float)Math.Round(val / 5) * 5;
			return Math.Max(10, Math.Min(val, 500)) / 100;
		}
		private void SetZoomFactor(float val, bool keepVerticalScroll) {
			float prevZoom = zoom;
			if(viewColumns > 1)
				SetMultiColumnZoom(val);
			else if((keepVerticalScroll || verticalScrollLocked) && Continuous) {
				float initialScrollY = ViewManager.ScrollPos.Y;
				SetViewControlCenterZoom(val);
				ViewManager.SetVertScroll(initialScrollY);
			} else
				SetViewControlCenterZoom(val);
			if(!FloatsComparer.Default.FirstEqualsSecond(prevZoom, zoom))
				OnZoomChanged(EventArgs.Empty);
			UpdateCommands();
			UpdateScrollBars();
			InvalidateViewControl();
		}
		void SetMultiColumnZoom(float val) {
			SetZoomFactorCore(val);
			viewManager.ShowSelectedPage();
		}
		void SetViewControlCenterZoom(float val) {
			Point viewScreenCenter = RectHelper.CenterOf(viewControl.RectangleToScreen(viewControl.ClientRectangle));
			IPage page = viewManager.FindPage(viewScreenCenter);
			if(page == null)
				page = viewManager.SelectedPage;
			PointF pageClientPoint = PointToPage(page, new PointF(viewControl.ClientSize.Width / 2f, viewControl.ClientSize.Height / 2f));
			SetZoomFactorCore(val);
			viewManager.ShowPagePoint(page, pageClientPoint);
		}
		void SetZoomFactorCore(float val) {
			zoom = Math.Min(MaxZoom, Math.Max(MinZoom, val));
			viewManager.SetPagePlace(viewColumns, viewRows);
		}
		void SetViewMode(PageViewModes value, int columns, int rows) {
			UpdateViewRowsAndColumns(columns, rows);
			SetViewMode(value);
		}
		void UpdateViewRowsAndColumns(float zoomValue) {
			int columns = viewManager.GetViewColumns(zoomValue);
			int rows = viewManager.GetViewRows(zoomValue);
			UpdateViewRowsAndColumns(columns, rows);
		}
		void UpdateViewRowsAndColumns(int columns, int rows) {
			viewColumns = columns;
			viewRows = rows;
		}
		void SetViewMode(PageViewModes value) {
			pageViewer = PageViewerBase.CreateInstance(this, value);
		}
		public void SetFocus() {
			viewControl.SetFocusInternal();
		}
		public void UpdateScrollBars() {
			scrollController.UpdateScrollBars();
		}
		public void UpdatePageView() {
			SetPageView();
			viewManager.ResetSelectedPageIndex();
			InvalidateViewControl();
		}
		private void ShowPagePoint(IPage page, PointF pt) {
			PointF pageClientPoint = PointToPage(page, pt);
			SetZoomFactor(1f, false);
			viewManager.ShowPagePoint(page, pageClientPoint);
			UpdateScrollBars();
		}
		PointF PointToPage(IPage page, PointF viewClientPoint) {
			if(page == null) return Point.Empty;
			RectangleF r = viewManager.GetPageRect(page);
			if(page == null) return Point.Empty;
			PointF pagePoint = PSUnitConverter.PixelToDoc(viewClientPoint, zoom, viewManager.ScrollPos);
			return PSNativeMethods.TranslatePointF(pagePoint, -r.X, -r.Y);
		}
		public void ShowPage(IPage page) {
			if(page == null)
				return;
			SelectedPageIndex = page.Index;
			viewManager.ShowPage(page);
			InvalidateViewControl();
			UpdateScrollBars();
		}
		protected internal virtual void ViewOnMouseWheelCore(MouseEventArgs e) {
			if(DocumentIsEmpty)
				return;
			if(ModifierKeys == Keys.Control)
				Zoom = GetZoomFactor(e.Delta);
			else
				scrollController.OnViewMouseWheel(e);
		}
		private void view_OnMouseMove(object sender, MouseEventArgs e) {
			this.OnMouseMove(e);
			if(DocumentIsEmpty)
				return;
			using(EventLogger logger = new EventLogger(h1 => ViewControl.CursorSet += h1, h2 => ViewControl.CursorSet -= h2)) {
				OnBrickMouseMove(e);
				IPage page = viewManager.FindPage(MousePosition);
				if(CurrentCursorEquals(DragCursors.HandDragCursor)) {
					int dx = prevMousePos.X - e.X;
					int dy = prevMousePos.Y - e.Y;
					prevMousePos = new Point(e.X, e.Y);
					HandleDragging(dx, dy);
				}
				if(!logger.EventLogged)
					SetCursor(GetCursor(page));
			}
		}
		void HandleDragging(int dx, int dy) {
			viewManager.OffsetHorzScroll(PSUnitConverter.PixelToDoc(dx, zoom));
			viewManager.OffsetVertScroll(PSUnitConverter.PixelToDoc(dy, zoom), false);
			UpdateScrollBars();
		}
		private void view_OnMouseDown(object sender, MouseEventArgs e) {
			this.OnMouseDown(e);
			if(DocumentIsEmpty)
				return;
			IPage page = viewManager.FindPage(MousePosition);
			if(viewManager.IsSelected(page))
				OnBrickMouseDown(e);
			if(e.Button != MouseButtons.Left || !prevMousePos.IsEmpty || !handTool ||
				!viewManager.PageContainsScreenPoint(page, MousePosition) || viewManager.IsWholePageVisible)
				return;
			prevMousePos = new Point(e.X, e.Y);
			SetCursor(GetCursor(page));
		}
		private void view_OnMouseUp(object sender, MouseEventArgs e) {
			this.OnMouseUp(e);
			if(DocumentIsEmpty)
				return;
			OnBrickMouseUp(e);
			prevMousePos = Point.Empty;
			SetCursor(GetCursor(viewManager.FindPage(MousePosition)));
		}
		void view_Click(System.Object sender, EventArgs e) {
			this.OnClick(e);
			if(DocumentIsEmpty || !CanHandleClick)
				return;
			IPage page = viewManager.FindPage(MousePosition);
			if(page == null) return;
			if(viewManager.IsSelected(page) && autoZoom) {
				HandlePageClick(page);
				return;
			}
			viewManager.SelectedPage = page;
			OnBrickClick(e);
		}
		protected virtual bool CanHandleClick {
			get {
				return !CurrentCursorEquals(DragCursors.HandCursor, DragCursors.HandDragCursor);
			}
		}
		void view_OnDoubleClick(object sender, EventArgs e) {
			this.OnDoubleClick(e);
			IPage page = viewManager.FindPage(MousePosition);
			if(DocumentIsEmpty || page == null)
				return;
			OnBrickDoubleClick(e);
		}
		private void HandlePageClick(IPage page) {
			if(CurrentCursorEquals(DragCursors.ZoomInCursor)) {
				Point pt = viewControl.PointToClient(MousePosition);
				ShowPagePoint(page, pt);
			} else if(viewRows != 0 && viewColumns != 0) {
				SetPageView(Math.Abs(viewColumns), Math.Abs(viewRows));
			} else
				ViewWholePage();
		}
		bool CurrentCursorEquals(params Cursor[] cursors) {
			for(int i = 0; i < cursors.Length; i++)
				if(Object.Equals(Cursor.Current, cursors[i]))
					return true;
			return false;
		}
		protected virtual void OnViewKeyDown(object sender, KeyEventArgs e) {
			if(!e.Handled && !DocumentIsEmpty)
				HandleKey(e.KeyCode);
		}
		protected virtual void OnViewKeyUp(object sender, KeyEventArgs e) {
		}
		protected virtual void view_Paint(System.Object sender, PaintEventArgs e) {
			ControlInfo.BackgroundText = DocumentIsEmpty && waitResult == null ? Status : "";
			ControlInfo.Bounds = ViewControl.DisplayRectangle;
			using(GraphicsCache cache = new GraphicsCache(e))
				ControlInfo.DrawPreviewBackground(cache);
			if(!DocumentIsEmpty) {
				e.Graphics.SmoothingMode = Zoom < 1.0 ? SmoothingMode.HighQuality : SmoothingMode.Default;
				e.Graphics.ScaleTransform(Zoom, Zoom);
				List<int> pageIndices = new List<int>();
				try {
					Document.BeforeDrawPages(this);
					DrawPages(e, item => pageIndices.Add(item.Index));
				} finally {
					Document.AfterDrawPages(this, pageIndices.ToArray());
				}
			}
		}
	   void DrawPages(PaintEventArgs e, Action<IPage> callback) {
			PageEnumerator pe = ViewManager.CreatePageEnumerator();
			RectangleF clipRectangle = PSUnitConverter.PixelToDoc(e.ClipRectangle, Zoom);
			RectangleF viewRectangle = PSUnitConverter.PixelToDoc(ViewControl.ClientRectangle, Zoom);
			while(pe.MoveNext()) {
				RectangleF rect = new RectangleF(pe.CurrentPlace.Location, pe.Current.PageSize);
				rect.Offset(-ViewManager.ScrollPos.X, -ViewManager.ScrollPos.Y);
				RectangleF rectWithBorders = InflateRectangle(rect, ControlInfo.PagePaddingEdges);
				if(viewRectangle.IntersectsWith(rectWithBorders)) callback(pe.Current);
				if(rectWithBorders.IsEmpty || clipRectangle.IntersectsWith(rectWithBorders) == false)
					continue;
				rect.X = DiscretizeFloat(rect.X);
				rect.Y = DiscretizeFloat(rect.Y);
				DrawBorder(e.Graphics, PSUnitConverter.DocToPixel(rect, Zoom), e, ViewManager.IsSelected(pe.Current));
				DrawPage(pe.Current, e.Graphics, rect.Location);
				if(ShowPageMargins && ViewManager.IsSelected(pe.Current) && !rect.IsEmpty)
					Margins.Draw(e.Graphics);
			}
		}
		float DiscretizeFloat(float val) {
			float f = PSUnitConverter.DocToPixel(val, Zoom);
			return PSUnitConverter.PixelToDoc((float)Math.Round(f), Zoom);
		}
		protected virtual void DrawPage(IPage page, Graphics graph, PointF position) {
			page.Draw(graph, PSUnitConverter.DocToPixel(position));
		}
		protected virtual void DrawBorder(Graphics graph, RectangleF rect, PaintEventArgs e, bool selected) {
			if(rect.IsEmpty)
				return;
			graph.ExecuteAndKeepState(() => {
				graph.ResetTransform();
				graph.PageUnit = GraphicsUnit.Display;
				ControlInfo.PageBounds = Rectangle.Round(rect);
				using(GraphicsCache cache = new GraphicsCache(e))
					if(selected)
						ControlInfo.DrawSelectedPageBorder(cache);
					else
						ControlInfo.DrawPageBorder(cache);
			});
		}
		protected static RectangleF InflateRectangle(RectangleF rect, DevExpress.Skins.SkinPaddingEdges edges) {
			return RectHelper.InflateRect(rect, PSUnitConverter.PixelToDoc(edges.Left), PSUnitConverter.PixelToDoc(edges.Top), PSUnitConverter.PixelToDoc(edges.Right), PSUnitConverter.PixelToDoc(edges.Bottom));
		}
		protected virtual void ShowException(Exception e) {
			throw e;
		}
		private void view_Resize(System.Object sender, EventArgs e) {
			if(disposeRunning || DocumentIsEmpty)
				return;
			ApplyHVScrollbarBounds();
			popupForm.HidePopup();
			SetPageView();
		}
		Cursor GetCursor(IPage page) {
			Cursor value = null;
			return TryGetCursor(page, ref value) ? value : Cursors.Default;
		}
		protected virtual bool TryGetCursor(IPage page, ref Cursor value) {
			if(page != null) {
				PointF pt = viewControl.PointToClient(MousePosition);
				if(viewManager.PageContainsPoint(page, pt)) {
					if(handTool)
						value = prevMousePos.IsEmpty ? DragCursors.HandCursor :
							DragCursors.HandDragCursor;
					if(AutoZoom)
						value = zoom < 1f ? DragCursors.ZoomInCursor :
							DragCursors.ZoomOutCursor;
				}
				if(ShowPageMargins && CanChangePageMargins) {
					MarginSide side = Margins.GetPointSide(pt);
					if(side != MarginSide.None)
						value = SideToCursor(side);
				}
			}
			return value != null;
		}
		static Cursor SideToCursor(MarginSide side) {
			return side == MarginSide.Left || side == MarginSide.Right ? Cursors.VSplit :
				side == MarginSide.Top || side == MarginSide.Bottom ? Cursors.HSplit :
				Cursors.Default;
		}
		private void OnLookAndFeelStyleChanged(object sender, System.EventArgs e) {
			ControlInfo.UpdatePainters(CreateBackgroundPreviewPainter(), CreatePageBorderPainter());
			this.SetPageView();
			SetCornerPanelBackground();
		}
	}
}
