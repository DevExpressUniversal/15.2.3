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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon.Gallery;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Gesture;
using DevExpress.Data.Helpers;
using DevExpress.Data;
namespace DevExpress.XtraBars.Ribbon {
	[DXToolboxItem(true), Designer("DevExpress.XtraBars.Ribbon.Design.GalleryControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	 ProvideProperty("PopupContextMenu", typeof(Control)),
	 Description("Represents a control capable of displaying images, while allowing you to categorize them into groups."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "GalleryControl")
]
	public class GalleryControl : BaseStyleControl, IBarAndDockingControllerClient, ISupportInitialize, IToolTipControlClient, IMouseWheelSupport, IGestureClient {
		GalleryControlGallery gallery;
		BarAndDockingController controller;
		int designGalleryGroupIndex;
		int designGalleryItemIndex;
		GalleryControlClient client;
		IDXMenuManager menuManager;
		public static void About() {
		}
		static GalleryControl() {
		}
		public GalleryControl() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.Selectable | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserMouse, false);
		}
		protected override void OnDockChanged(EventArgs e) {
			base.OnDockChanged(e);
			Gallery.LayoutChanged();
			LayoutChanged();
		}
		[Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlGallery"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GalleryControlGallery Gallery {
			get {
				if(gallery == null)
					gallery = CreateGallery();
				return gallery;
			}
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			Gallery.RefreshGallery();
		}
		internal void SetGallery(GalleryControlGallery gallery) {
			this.gallery = gallery;
			this.gallery.GalleryControl = this;
		}
		protected internal virtual bool AllowFocus { get { return true; } }
		protected override Size DefaultSize { get { return new Size(120, 95); } }
		GalleryControlDesignTimeManager designTimeManager;
		protected internal GalleryControlDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManager == null)
					designTimeManager = CreateDesignTimeManager();
				return designTimeManager;
			}
		}
		protected internal virtual GalleryControlDesignTimeManager CreateDesignTimeManager() { return new GalleryControlDesignTimeManager(this, Site); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int DesignGalleryGroupIndex { get { return designGalleryGroupIndex; } set { designGalleryGroupIndex = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int DesignGalleryItemIndex { get { return designGalleryItemIndex; } set { designGalleryItemIndex = value; } }
		#region HideProperties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return Gallery.BackColor; }
			set { Gallery.BackColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		#endregion
		protected virtual GalleryControlGallery CreateGallery() {
			return new GalleryControlGallery(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceObject Appearance {
			get { return base.Appearance; }
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlController"),
#endif
 DefaultValue((string)null)]
		public virtual BarAndDockingController Controller {
			get { return controller; }
			set {
				if(Controller == value) return;
				GetControllerInternal().RemoveClient(this);
				this.controller = value;
				GetControllerInternal().AddClient(this);
				OnControllerChanged();
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryControlMenuManager"),
#endif
 DefaultValue((string)null)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		protected internal bool DesignModeCore {
			get { return DesignMode; }
		}
		protected internal GalleryItem KeyboardSelectedItem {
			get {
				StandaloneGalleryViewInfo info = Gallery.ViewInfo;
				if(info != null && info.KeyboardSelectedItem != null) {
					return info.KeyboardSelectedItem.Item;
				}
				return null;
			}
			set {
				StandaloneGalleryViewInfo info = Gallery.ViewInfo;
				if(info != null)
					info.MoveItem(value);
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			GalleryControlClient c = e.Control as GalleryControlClient;
			if(c != null) {
				c.GalleryControl = this;
				this.client = c;
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			GalleryControlClient c = e.Control as GalleryControlClient;
			if(c != null) {
				c.GalleryControl = null;
				this.client = null;
			}
		}
		protected internal BaseControlViewInfo ViewInfoCore { get { return ViewInfo; } }
		BarAndDockingController GetControllerInternal() {
			return controller != null ? controller : BarAndDockingController.Default;
		}
		public virtual BarAndDockingController GetController() {
			return Controller == null || Controller.Disposing ? BarAndDockingController.Default : Controller;
		}
		protected virtual void OnControllerChanged() {
			Gallery.RefreshGallery();
			Refresh();
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			Gallery.RefreshGallery();
			base.OnLookAndFeelChanged(sender, e);
		}
		protected internal virtual void AccessibleNotifyClients(AccessibleEvents accessibleEvents, int objectIndex, int childIndex) {
			AccessibilityNotifyClients(accessibleEvents, objectIndex, childIndex);
		}
		protected GalleryControlHandlerBase Handler { get { return Gallery.Handler; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				CheckViewInfo();
				base.OnMouseDown(ee);
				if(ee.Handled) return;
				Handler.OnMouseDown(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				CheckViewInfo();
				base.OnMouseUp(ee);
				if(ee.Handled) return;
				Handler.OnMouseUp(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				CheckViewInfo();
				base.OnMouseMove(ee);
				if(ee.Handled) return;
				Handler.OnMouseMove(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected internal virtual void OnMouseWheelCore(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				CheckViewInfo();
				base.OnMouseWheel(ee);
				if(ee.Handled) return;
				Handler.OnMouseWheel(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				CheckViewInfo();
				base.OnMouseEnter(ee);
				if(ee.Handled) return;
				Handler.OnMouseEnter(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				CheckViewInfo();
				base.OnMouseLeave(ee);
				if(ee.Handled) return;
				Handler.OnMouseLeave(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override bool IsInputKey(Keys keyData) {
			bool res = base.IsInputKey(keyData);
			if(res)
				return res;
			return keyData == Keys.Home ||
				keyData == Keys.PageUp ||
				keyData == Keys.Up ||
				keyData == Keys.Down ||
				keyData == Keys.PageDown ||
				keyData == Keys.End ||
				keyData == Keys.Left ||
				keyData == Keys.Right;
		}
		protected internal virtual void OnKeyDownCore(KeyEventArgs e) {
			CheckViewInfo();
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			OnKeyDownCore(e);
		}
		protected internal virtual void OnKeyPressCore(KeyPressEventArgs e) {
			CheckViewInfo();
			base.OnKeyPress(e);
			if(e.Handled) return;
			Handler.OnKeyPress(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			OnKeyPressCore(e);
		}
		protected internal virtual void OnKeyUpCore(KeyEventArgs e) {
			CheckViewInfo();
			base.OnKeyUp(e);
			if(e.Handled) return;
			Handler.OnKeyUp(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			OnKeyUpCore(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(AllowUpdateLayoutOnVisibleChanged && Visible) Gallery.LayoutChanged();
		}
		protected virtual bool AllowUpdateLayoutOnVisibleChanged { get { return true; } }
		protected override void OnPaint(PaintEventArgs e) {
			if(this.shouldCloseFilterMenu) {
				Gallery.CloseMenu();
				this.shouldCloseFilterMenu = false;
			}
			CheckViewInfo();
			base.OnPaint(e);
		}
		protected virtual void GalleryPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				ImageGalleryInfoArgs info = Gallery.ViewInfo.CreateGalleryInfo(null);
				info.DrawContent = false;
				Gallery.Painter.Draw(cache, info);
			}
		}
		protected internal virtual Rectangle ContentRect { get { return ViewInfo.ClientRect; } }
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			Gallery.RefreshGallery();
		}
		internal GalleryControlClient GetClientCore() { return client; }
		protected internal GalleryControlClient Client {
			get {
				if(client == null) {
					this.client = CreateClient();
					Controls.Add(client);
				}
				return client;
			}
		}
		protected virtual GalleryControlClient CreateClient() { return new GalleryControlClient(this); }
		protected internal virtual void CheckViewInfo() {
			if(ViewInfo.IsReady && Gallery.ViewInfo.IsReady) {
				return;
			}
			GraphicsInfo info = new GraphicsInfo();
			info.AddGraphics(null);
			try {
				ViewInfo.IsReady = false;
				UpdateViewInfo(info.Graphics);
				UpdateClientBounds();
			}
			finally { info.ReleaseGraphics(); }
		}
		protected virtual void UpdateClientBounds() {
			Client.Bounds = Gallery.ViewInfo.ControlClientBounds;
		}
		public RibbonHitInfo CalcHitInfo(Point pt) {
			return Gallery.ViewInfo.CalcHitInfo(pt);
		}
		#region IBarAndDockingControllerClient Members
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			OnControllerChanged();
		}
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			Controller = null;
		}
		#endregion
		#region ISupportInitialize Members
		bool isLoading = false;
		public void BeginInit() {
			isLoading = true;
		}
		public void EndInit() {
			isLoading = false;
		}
		public override bool IsLoading { get { return isLoading; } }
		#endregion
		#region IToolTipControlClient Members
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return Gallery.GetToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return ShowToolTips; }
		}
		#endregion
		public override Size GetPreferredSize(Size proposedSize) {
			Size res = ((GalleryControlViewInfo)ViewInfo).CalcGalleryBestSize(proposedSize);
			if(Gallery.GetAutoSize() == GallerySizeMode.Vertical)
				res.Width = Math.Min(res.Width, proposedSize.Width);
			return res;
		}
		protected override void OnAutoSizeChanged(EventArgs e) {
			AdjustSize();
			base.OnAutoSizeChanged(e);
		}
		protected void BaseSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override Size CalcSizeableMinSize() {
			if(Gallery.GetAutoSize() == GallerySizeMode.None)
				return Size.Empty;
			else if(Gallery.GetAutoSize() == GallerySizeMode.Vertical)
				return ((GalleryControlViewInfo)ViewInfo).CalcGalleryBestSize(new Size(Width, Height));
			else if(Gallery.GetAutoSize() == GallerySizeMode.Both)
				return ((GalleryControlViewInfo)ViewInfo).Bounds.Size;
			return base.CalcSizeableMinSize();
		}
		protected override Size CalcSizeableMaxSize() {
			if(Gallery.GetAutoSize() == GallerySizeMode.None)
				return Size.Empty;
			if(Gallery.GetAutoSize() == GallerySizeMode.Vertical ||
				Gallery.GetAutoSize() == GallerySizeMode.Both)
				return CalcSizeableMinSize();
			return base.CalcSizeableMaxSize();
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			this.isInLayoutControl = null;
		}
		bool? isInLayoutControl = null;
		internal bool IsInLayoutControl {
			get {
				if(isInLayoutControl.HasValue)
					return isInLayoutControl.Value;
				if(Parent == null)
					isInLayoutControl = false;
				else isInLayoutControl = Parent.GetType().Name.Contains("LayoutControl");
				return isInLayoutControl.Value;
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			BaseSetBoundsCore(x, y, width, height, specified);
		}
		protected internal virtual void OnGalleryAutoSizeChanged() {
			AutoSize = Gallery.GetAutoSize() != GallerySizeMode.None;
			AdjustSize();
			Gallery.RefreshGallery();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Gallery.RefreshGallery();
			RaiseSizeableChanged();
		}
		bool skipUpdateBoundsInSetBoundsCore = false;
		protected internal virtual Size CalcGalleryBestSize() {
			Gallery.ViewInfo.Reset();
			return ((GalleryControlViewInfo)ViewInfo).CalcGalleryBestSize();
		}
		protected internal virtual Size CalcGalleryBestSize(Size sz) {
			Gallery.ViewInfo.Reset();
			return ((GalleryControlViewInfo)ViewInfo).CalcGalleryBestSize(sz);
		}
		protected internal virtual void AdjustSize() {
			if(!AutoSize)
				return;
			skipUpdateBoundsInSetBoundsCore = true;
			try {
				Size = CalcGalleryBestSize();
			}
			finally {
				skipUpdateBoundsInSetBoundsCore = false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GalleryControlDesignTimeManager GetDesignTimeManager() { return DesignTimeManager; }
		internal void FireGalleryChanged() {
			if(!DesignMode) return;
			if(IsLoading) return;
			OnFireGalleryChanged();
		}
		protected virtual void OnFireGalleryChanged() {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		#region DragEvents
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(!DesignMode) {
				base.OnQueryContinueDrag(e);
				return;
			}
			DesignTimeManager.DragManager.ItemOnQueryContinueDrag(e, this);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(!DesignMode) {
				base.OnGiveFeedback(e);
				return;
			}
			DesignTimeManager.DragManager.ItemOnGiveFeedback(e, this);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if(!DesignMode) {
				base.OnDragEnter(e);
				return;
			}
			DesignTimeManager.DragManager.FireDoDragging = false;
			if(!DesignTimeManager.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected override void OnDragLeave(EventArgs e) {
			if(!DesignMode) {
				base.OnDragLeave(e);
				return;
			}
			DesignTimeManager.DragManager.FireDoDragging = true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(!DesignMode) {
				base.OnDragDrop(e);
				return;
			}
			DesignTimeManager.DragManager.StopDragging(this, e.Effect, false);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!DesignMode) {
				base.OnDragOver(e);
				return;
			}
			if(!DesignTimeManager.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
			DesignTimeManager.DragManager.DoDragging(this, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
		}
		#endregion
		bool shouldCloseFilterMenu = false;
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
			const int WM_EXITMENULOOP = 0x0212;
			if(m.Msg == WM_EXITMENULOOP) {
				shouldCloseFilterMenu = true;
			}
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected internal virtual void InvalidateGallery() {
			Invalidate();
			Client.Invalidate();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new GalleryControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new GalleryControlObjectPainer();
		}
		internal BaseGalleryViewInfo GetGalleryViewInfo() {
			return Gallery.ViewInfo;
		}
		protected internal void ResetViewInfo() {
			ViewInfo.IsReady = false;
			Gallery.ViewInfo.Reset();
		}
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			GestureAllowArgs[] res = new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
			return res;
		}
		IntPtr IGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		int yOverPan = 0;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				yOverPan = 0;
				return;
			}
			if(delta.Y == 0) return;
			if(!Gallery.ScrollBarVisible) return;
			int prevTopY = Gallery.ScrollYPosition;
			Gallery.ScrollYPosition -= delta.Y;
			if(prevTopY == Gallery.ScrollYPosition) {
				yOverPan += delta.Y;
			}
			else {
				yOverPan = 0;
				Gallery.ScrollBar.Update();
			}
			overPan.Y = yOverPan;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		#endregion
		internal void RaiseSizeableChangedCore() {
			RaiseSizeableChanged();
		}
	}
	[ToolboxItem(false),
	Designer("DevExpress.XtraBars.Ribbon.Design.GalleryControlClientDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	TypeConverter("DevExpress.XtraBars.TypeConverters.GalleryControlClientTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class GalleryControlClient : ContainerControl, IMouseWheelSupport, ITransparentBackgroundManager {
		GalleryControl galleryControl;
		public GalleryControlClient() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
		}
		public GalleryControlClient(GalleryControl galleryControl)
			: this() {
			this.galleryControl = galleryControl;
			if(GalleryControl.Container != null)
				GalleryControl.Container.Add(this);
		}
		protected internal virtual void GalleryPaint(PaintEventArgs e) {
			if(!Gallery.ViewInfo.IsReady)
				return;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				cache.Graphics.TranslateTransform(-Gallery.ViewInfo.ControlClientBounds.X, -Gallery.ViewInfo.ControlClientBounds.Y);
				ImageGalleryInfoArgs info = Gallery.ViewInfo.CreateGalleryInfo(null);
				info.DrawFilter = false;
				Gallery.Painter.Draw(cache, info);
				cache.Graphics.ResetTransform();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name { get { return base.Name; } set { base.Name = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Point Location { get { return base.Location; } set { base.Location = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Size Size { get { return base.Size; } set { base.Size = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new DockStyle Dock { get { return base.Dock; } set { base.Dock = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		bool ShouldSerializeControls() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control.ControlCollection Controls { get { return base.Controls; } }
		public GalleryControl GalleryControl {
			get { return galleryControl; }
			set {
				if(GalleryControl == value)
					return;
				OnGalleryControlChanging();
				galleryControl = value;
				OnGalleryControlChanged();
			}
		}
		protected virtual void OnGalleryControlChanging() {
			if(GalleryControl != null)
				GalleryControl.Controls.Remove(this);
		}
		protected virtual void OnGalleryControlChanged() {
			if(GalleryControl == null)
				return;
			if(GalleryControl.GetClientCore() != null && GalleryControl.GetClientCore() != this)
				GalleryControl.Controls.Remove(GalleryControl.Client);
			if(GalleryControl != null)
				GalleryControl.Controls.Add(this);
			GalleryControl.Refresh();
		}
		public GalleryControlGallery Gallery { get { return GalleryControl.Gallery; } }
		protected virtual void CheckViewInfo() {
			GalleryControl.CheckViewInfo();
		}
		protected override void OnPaint(PaintEventArgs e) {
			CheckViewInfo();
			GalleryPaint(e);
			RaisePaintEvent(this, e);
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_NCHITTEST) {
				m.Result = new IntPtr(DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTTRANSPARENT);
				return;
			}
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			GalleryControl.OnKeyDownCore(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			GalleryControl.OnKeyPressCore(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			GalleryControl.OnKeyUpCore(e);
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			GalleryControl.OnMouseWheelCore(e);
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			foreach(GalleryItemGroup group in Gallery.Groups) {
				if(group.CaptionControl == e.Control) {
					group.CaptionControl = null;
					break;
				}
			}
		}
		#region ITransparentBackgroundManager Members
		Color GetForeColor() {
			Color res = Gallery.ViewInfo.PaintAppearance.GroupCaption.ForeColor;
			if(res.IsSystemColor)
				return RibbonSkins.GetSkin(Gallery.Provider).GetSystemColor(res);
			return res;
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetForeColor();
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetForeColor();
		}
		#endregion
	}
	public class GalleryControlViewInfo : BaseStyleControlViewInfo {
		Rectangle backgroundRect;
		public GalleryControlViewInfo(BaseStyleControl owner) : base(owner) { }
		public GalleryControl GalleryControl { get { return (GalleryControl)Owner; } }
		public virtual GalleryControlGallery Gallery { get { return GalleryControl.Gallery; } }
		protected internal Rectangle BackgroundRect { get { return backgroundRect; } }
		Padding GetGalleryControlPadding() {
			return GalleryControl.Padding == Padding.Empty ? new Padding(1) : GalleryControl.Padding;
		}
		protected override void CalcClientRect(Rectangle bounds) {
			base.CalcClientRect(bounds);
			this.backgroundRect = ClientRect;
			Padding padding = GetGalleryControlPadding();
			this.fClientRect = new Rectangle(ClientRect.X + padding.Left, ClientRect.Y + padding.Top, ClientRect.Width - padding.Horizontal, ClientRect.Height - padding.Vertical);
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			if(!Gallery.ViewInfo.IsReady) {
				this.bestFitCore = CalcBestFitCore(g);
				if(GalleryControl.IsInLayoutControl)
					Gallery.ViewInfo.IsReady = false;
				Gallery.ViewInfo.CalcViewInfo(ClientRect);
			}
		}
		protected Size CalcSizeFromClientSize(Size client) {
			Size sz = client;
			Padding padding = GetGalleryControlPadding();
			sz.Width += padding.Horizontal;
			sz.Height += padding.Vertical;
			return BorderPainter.CalcBoundsByClientRectangle(null, BorderPainter, GetBorderArgs(Bounds), new Rectangle(Point.Empty, sz)).Size;
		}
		protected Size CalcClientSize(Size size) {
			Size res = size;
			Padding padding = GetGalleryControlPadding();
			res.Width -= padding.Horizontal;
			res.Height -= padding.Vertical;
			return BorderPainter.GetObjectClientRectangle(null, BorderPainter, GetBorderArgs(new Rectangle(Point.Empty, res))).Size;
		}
		public virtual Size CalcGalleryBestSize(int row, int col) {
			try {
				Size sz = Gallery.ViewInfo.CalcGalleryBestSize(row, col);
				sz = CalcSizeFromClientSize(sz);
				return sz;
			}
			finally {
				Gallery.ViewInfo.IsReady = false;
			}
		}
		public virtual Size CalcGalleryBestSize() {
			return CalcGalleryBestSize(Gallery.RowCount, Gallery.ColumnCount);
		}
		public virtual Size CalcGalleryBestSize(Size sz) {
			Size res = ((GalleryControlGalleryViewInfo)Gallery.ViewInfo).CalcGalleryBestSizeCore(sz, false);
			res = CalcSizeFromClientSize(res);
			return res;
		}
		Size bestFitCore = Size.Empty;
		public override Size CalcBestFit(Graphics g) {
			if(IsReady && Gallery.ViewInfo.IsReady && !bestFitCore.IsEmpty) return bestFitCore;
			return CalcBestFitCore(g);
		}
		public virtual Size CalcBestFitCore(Graphics g) {
			if(Gallery.AutoSize != GallerySizeMode.Vertical) {
				if(!Gallery.ViewInfo.IsReady) {
					Gallery.ViewInfo.ColCount = Gallery.GetMaxColumnCount();
					Size size = CalcSizeFromClientSize(new Size(100, 100));
					size = new Size(OwnerControl.ClientSize.Width - size.Width + 100, OwnerControl.ClientSize.Height - size.Height + 100);
					Gallery.ViewInfo.CalcViewInfo(new Rectangle(Point.Empty, size));
					Gallery.ViewInfo.ColCount = Gallery.ColumnCount;
				}
				return CalcGalleryBestSize(Gallery.GetMaxRowCount(), Gallery.GetMaxColumnCount());
			}
			return CalcGalleryBestSize(OwnerControl.ClientSize);
		}
	}
	public class GalleryControlObjectPainer : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			GalleryControlViewInfo vi = (GalleryControlViewInfo)info.ViewInfo;
			ImageGalleryInfoArgs galleryInfoArgs = vi.Gallery.ViewInfo.CreateGalleryInfo(null);
			galleryInfoArgs.DrawContent = false;
			vi.Gallery.Painter.Draw(info.Cache, galleryInfoArgs);
		}
	}	
	class GalleryControlCriteriaProvider : SearchControlCriteriaProviderBase {
		protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
			return DevExpress.Data.Filtering.DxFtsContainsHelper.Create(new string[] { "Caption" }, result, args.FilterCondition);
		}
		protected override Data.Helpers.FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args) {
			return new DevExpress.Data.Helpers.FindSearchParser().Parse(args.SearchText);
		}
		protected override void DisposeCore() {
		}
	}
}
