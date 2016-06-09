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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Utils;
using System.Reflection;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraBars.Ribbon {
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class RibbonStatusBarItemLinkCollection : BaseRibbonItemLinkCollection {
		RibbonStatusBar statusBar;
		public RibbonStatusBarItemLinkCollection(RibbonStatusBar statusBar) {
			this.statusBar = statusBar;
		}
		protected internal override object Owner { get { return this; } }
		public RibbonStatusBar StatusBar { get { return statusBar; } }
		public override RibbonControl Ribbon { get { return StatusBar.Ribbon; } }
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			StatusBar.OnLinksChanged(e);
		}
		protected internal override bool IsMergedState {
			get {
				return BaseIsMergedState;
			}
		}
		protected override bool GetVisulEffectsVisible() {
			return Ribbon != null && StatusBar.RealVisible;
		}
		protected override DevExpress.Utils.VisualEffects.ISupportAdornerUIManager GetVisualEffectsOwner() { return StatusBar; }
	}
	public class RibbonStatusBarMergeEventArgs : EventArgs {
		RibbonStatusBar mergedChild;
		public RibbonStatusBarMergeEventArgs(RibbonStatusBar mergedChild) {
			this.mergedChild = mergedChild;
		}
		public RibbonStatusBar MergedChild { get { return mergedChild; } }
	}
	public delegate void RibbonStatusBarMergeEventHandler(object sender, RibbonStatusBarMergeEventArgs e);
	[DXToolboxItem(true),
	 Designer("DevExpress.XtraBars.Ribbon.Design.RibbonStatusBarDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	 Description("Represents a status bar that must be used along with a RibbonControl."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "RibbonStatusBar")
	]
	public class RibbonStatusBar : ControlBase, ICustomBarControl, IBarObject, ISupportXtraAnimation, IToolTipControlClient, IEditorBackgroundProvider, ISupportAdornerUIManager {
		private static readonly object merge = new object();
		private static readonly object unMerge = new object();
		RibbonControl ribbon;
		RibbonStatusBarViewInfo viewInfo;
		RibbonStatusBarPainter painter;
		BarItemLinkCollection itemLinks;
		RibbonStatusBar mergedStatusBarCore;
		bool showSizeGrip;
		bool autoHeight = false;
		public RibbonStatusBar() : this(null) { }
		public RibbonStatusBar(RibbonControl ribbon) {
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.Selectable, false);
			this.ribbon = ribbon;
			if(Ribbon != null)Ribbon.StatusBar = this;
			this.itemLinks = new RibbonStatusBarItemLinkCollection(this);
			this.showSizeGrip = true;
			this.Dock = DockStyle.Bottom;
			base.TabStop = false;
			ToolTipController.DefaultController.AddClientControl(this);
			HideWhenMerging = DefaultBoolean.Default;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeParentEvents();
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
			}
			base.Dispose(disposing);
		}
		RibbonStatusBarAccessible accessibleStatusBar = null;
		[Browsable(false)]
		public RibbonStatusBarAccessible AccessibleStatusBar {
			get {
				if(accessibleStatusBar == null) accessibleStatusBar = CreateAccessibleStatusBar();
				return accessibleStatusBar;
			}
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean HideWhenMerging {
			get;
			set;
		}
		protected virtual RibbonStatusBarAccessible CreateAccessibleStatusBar() { return new RibbonStatusBarAccessible(this); }
		protected override AccessibleObject CreateAccessibilityInstance() { return AccessibleStatusBar.Accessible; }
		public void AccessibleNotifyClients(AccessibleEvents accEvents, int objectId, int childId) { 
			AccessibilityNotifyClients(accEvents, objectId, childId);
		}
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			return AccessibleStatusBar.Accessible;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonStatusBarDock"),
#endif
 DefaultValue(DockStyle.Bottom)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual bool IsDesignMode { get { return DesignMode; } }
		[Browsable(false)]
		public RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value) return;
				if(Ribbon != null) Ribbon.Disposed -= new EventHandler(OnRibbonDisposed);
				ribbon = value;
				this.handler = null;
				if(Ribbon != null) {
					Ribbon.Disposed += new EventHandler(OnRibbonDisposed);
					Refresh();
					Ribbon.StatusBar = this;
				}
			}
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			Refresh();
		}
		protected internal virtual RibbonStatusBar MergedStatusBar { 
			get { return this.mergedStatusBarCore; } 
		}
		public virtual void MergeStatusBar(RibbonStatusBar mergedStatusBar) {
			UnMergeStatusBar();
			this.mergedStatusBarCore = mergedStatusBar;
			if(MergedStatusBar == null) return;
			ItemLinks.Merge(MergedStatusBar.ItemLinks);
			RaiseMerge(new RibbonStatusBarMergeEventArgs(MergedStatusBar));
			UpdateBarVisibilityInMerge(true);
			Refresh();
		}
		public virtual void UnMergeStatusBar() {
			if(MergedStatusBar == null) return;
			UpdateBarVisibilityInMerge(false);
			RibbonStatusBar merged = MergedStatusBar;
			ItemLinks.UnMerge();
			this.mergedStatusBarCore = null;
			RaiseUnMerge(new RibbonStatusBarMergeEventArgs(merged));
		}
		protected internal bool RealVisible {
			get {
				var method = typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic);
				return (bool)method.Invoke(this, new object[] { 2 }); 
			}
		}
		protected internal void SetVisible(bool visible) {
			Visible = visible;
			if(Ribbon == null || Ribbon.RibbonForm == null) return;
			Ribbon.RibbonForm.ForceStyleChanged();
		}
		protected internal virtual void UpdateIsMdiChildStatusBar() {
			if(Manager == null) return;
			Form form = Manager.GetForm();
			IsMdiChildStatusBar = ((form != null) && form.IsMdiChild) || Manager.GetIsMdiChildManager();
		}
		bool isMdiChildStatusBar = false;
		protected bool IsMdiChildStatusBar {
			get { return isMdiChildStatusBar; }
			set {
				if(isMdiChildStatusBar == value)
					return;
				isMdiChildStatusBar = value;
				OnIsMdiChildStatusBarChanged();
			}
		}
		protected virtual void OnIsMdiChildStatusBarChanged() {
			if(!GetHideWhenMerging())
				return;
			bool actualVisibility = !IsMdiChildStatusBar;
			Form form = Manager.GetForm();
			if(!actualVisibility && form != null && !(form is Docking2010.FloatDocumentForm)) {
				RibbonMdiMergeStyle mergeStyle;
				if(!TryGetParentRibbonMdiMergeStyle(form, out mergeStyle) || mergeStyle == RibbonMdiMergeStyle.Never)
					return;
				if(MergeOnlyWhenMaximized(mergeStyle))
					actualVisibility = (form.WindowState != FormWindowState.Maximized);
			}
			Docking2010.Views.BaseDocument document = Manager.GetDocument();
			if(!actualVisibility && document != null) {
				RibbonMdiMergeStyle mergeStyle;
				if(!TryGetParentRibbonMdiMergeStyle(document, out mergeStyle) || mergeStyle == RibbonMdiMergeStyle.Never)
					return;
				Docking2010.DocumentContainer container = Manager.GetDocumentContainer();
				if(container != null) {
					if(MergeOnlyWhenMaximized(mergeStyle))
						actualVisibility = !container.IsMaximized;
				}
			}
			Visible = actualVisibility;
		}
		void UpdateBarVisibilityInMerge(bool merged) {
			if(!GetHideWhenMerging())
				return;
			if(MergedStatusBar != null) {
				if(MergeOnlyWhenMaximized(Ribbon.MdiMergeStyle))
					MergedStatusBar.Visible = !merged;
				else if(Ribbon.MdiMergeStyle == RibbonMdiMergeStyle.Always)
					MergedStatusBar.Visible = false;
			}
		}
		bool MergeOnlyWhenMaximized(RibbonMdiMergeStyle style) {
			return style == RibbonMdiMergeStyle.Default || style == RibbonMdiMergeStyle.OnlyWhenMaximized;
		}
		bool TryGetParentRibbonMdiMergeStyle(Docking2010.Views.BaseDocument document, out RibbonMdiMergeStyle mergeStyle) {
			mergeStyle = RibbonMdiMergeStyle.Always;
			var manager = (document != null) ? document.Manager : null;
			if(manager != null && manager.GetContainer() != null) {
				RibbonBarManager parentManager = BarManager.FindManager(manager.GetContainer()) as RibbonBarManager;
				if(parentManager != null && parentManager.Ribbon != null) {
					mergeStyle = parentManager.Ribbon.MdiMergeStyle;
					if(!parentManager.Ribbon.HasMergeEventSubscription)
						mergeStyle = RibbonMdiMergeStyle.Never;
					return true;
				}
			}
			return false;
		}
		bool TryGetParentRibbonMdiMergeStyle(Form form, out RibbonMdiMergeStyle mergeStyle) {
			mergeStyle = RibbonMdiMergeStyle.Default;
			if(form != null && form.MdiParent != null) {
				RibbonBarManager parentManager = BarManager.FindManager(form.MdiParent) as RibbonBarManager;
				if(parentManager != null && parentManager.Ribbon != null) {
					mergeStyle = parentManager.Ribbon.MdiMergeStyle;
					if(!parentManager.Ribbon.HasMergeEventSubscription)
						mergeStyle = RibbonMdiMergeStyle.Never;
					return true;
				}
			}
			return false;
		}
		bool GetHideWhenMerging() {
			if(HideWhenMerging == DefaultBoolean.Default)
				return Manager == null || Manager.HideBarsWhenMerging;
			return HideWhenMerging != DefaultBoolean.False;
		}
		void OnRibbonDisposed(object sender, EventArgs e) {
			Ribbon = null;
		}
		[Browsable(false), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
		public RibbonBarManager Manager { get { return Ribbon == null ? null : Ribbon.Manager; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DevExpress.Utils.Design.InheritableCollection]
		public BarItemLinkCollection ItemLinks { get { return itemLinks; } }
		protected internal virtual RibbonStatusBarViewInfo CreateViewInfo() { return new RibbonStatusBarViewInfo(this); }
		protected internal virtual RibbonStatusBarPainter CreatePainter() { return new RibbonStatusBarPainter(); }
		protected internal virtual RibbonStatusBarPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			RibbonForm form = FindForm() as RibbonForm;
			if(form != null && form.StatusBar == null) form.StatusBar = this;
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_NCHITTEST) {
				Point p = PointToClient(new Point(m.LParam.ToInt32()));
				if(ViewInfo.InSizeGrip(p)) {
					m.Result = new IntPtr(ViewInfo.IsRightToLeft ? RibbonCaptionViewInfo.HT.HTBOTTOMLEFT : RibbonCaptionViewInfo.HT.HTBOTTOMRIGHT);
					return;
				}
				if(ViewInfo.IsAllowRibbonFormBehavior) {
					if(p.Y > Height - 3) {
						m.Result = new IntPtr(RibbonCaptionViewInfo.HT.HTTRANSPARENT);
						return;
					}
				}
			}
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected internal virtual bool InDesignerRect(Point pt) {
			if(ViewInfo == null) return false;
			return ViewInfo.DesignerLeftRect.Contains(pt) || ViewInfo.DesignerRightRect.Contains(pt);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RibbonStatusBarViewInfo ViewInfo {
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text {
			get { return string.Empty; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { }
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if((specified & BoundsSpecified.Height) != 0 || specified == BoundsSpecified.None) {
				int prev = height;
				height = ViewInfo.CalcBestHeight();
				y -= (height - prev);
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		[System.ComponentModel.Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonStatusBarShowSizeGrip"),
#endif
 DefaultValue(true)]
		public bool ShowSizeGrip {
			get { return showSizeGrip; }
			set {
				if(ShowSizeGrip == value) return;
				showSizeGrip = value;
				Refresh();
			}
		}
		[System.ComponentModel.Category("Behavior"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonStatusBarAutoHeight"),
#endif
 DefaultValue(false)]
		public bool AutoHeight {
			get { return autoHeight; }
			set {
				if(AutoHeight == value) return;
				autoHeight = value;
				CheckHeight();
			}
		}
		public RibbonHitInfo CalcHitInfo(Point point) { return ViewInfo.CalcHitInfo(point); }
		protected internal virtual void CheckViewInfo() {
			if(!ViewInfo.IsReady) ViewInfo.CalcViewInfo(ClientRectangle);
		}
		public void LayoutChanged() { Refresh(); }
		public override void Refresh() {
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			ViewInfo.IsReady = false;
			CheckHeight();
			Invalidate();
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			Refresh();
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			if(Ribbon == null) {
				OnPaintNoRibbon(e);
				return;
			}
			CheckViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Painter.DrawObject(new RibbonStatusBarInfoArgs(ViewInfo, cache, ViewInfo.Bounds, ObjectState.Normal));
			}
			RaisePaintEvent(this, e);
		}
		protected virtual void OnPaintNoRibbon(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ControlPaint.DrawButton(e.Graphics, ClientRectangle, ButtonState.Flat);
				Rectangle r = Rectangle.Inflate(ClientRectangle, -2, -1);
				AppearanceObject app = new AppearanceObject(new AppearanceDefault(Color.Red, Color.Transparent, HorzAlignment.Center, VertAlignment.Center));
				app.Font = cache.GetFont(app.Font, FontStyle.Bold);
				app.DrawString(cache, "Please add a RibbonControl onto the form.", r);
			}
		}
		void CheckHeight() {
			if(!AutoHeight) return;
			Form form = FindForm();
			if(form != null && form.WindowState == FormWindowState.Minimized)
				return;
			ViewInfo.CalcViewInfo(ClientRectangle);
			Height = ViewInfo.CalcBestHeight();
		}
		protected internal virtual void OnLinksChanged(CollectionChangeEventArgs e) {
			Refresh();
		}
		Form parentForm;
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			CheckHeight();
			UnsubscribeParentEvents();
			parentForm = FindForm();
			SubscribeParentEvents();
			UpdateIsMdiChildStatusBar();
		}
		void UnsubscribeParentEvents() {
			if(parentForm != null) 
				parentForm.ParentChanged -= OnFormParentChanged;
		}
		void SubscribeParentEvents() {
			if(parentForm != null) 
				parentForm.ParentChanged += OnFormParentChanged;
		}
		protected virtual void OnFormParentChanged(object sender, EventArgs e) {
			UpdateIsMdiChildStatusBar();
		}
		#region IBarObject Members
		bool IBarObject.IsBarObject { get { return true; } }
		BarManager IBarObject.Manager { get { return Manager; } }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) { return RibbonShouldCloseMenuOnClick(e, child); }
		protected virtual BarMenuCloseType RibbonShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			Point p = PointToClient(new Point(e.X, e.Y));
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(p);
			if(hitInfo.InItem) return BarMenuCloseType.None;
			return BarMenuCloseType.All;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(Manager.SelectionInfo.ActiveEditor != null) {
				if(Manager.SelectionInfo.OpenedPopups.Count == 0) return true;
			}
			return false; 
		}
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return true; }
		#endregion
		#region ICustomBarControl Members
		void ICustomBarControl.ProcessKeyDown(KeyEventArgs e) {
		}
		bool ICustomBarControl.IsNeededKey(KeyEventArgs e) {
			return false;
		}
		bool ICustomBarControl.IsInterceptKey(KeyEventArgs e) {
			return false;
		}
		void ICustomBarControl.ProcessKeyUp(KeyEventArgs e) { }
		Control ICustomBarControl.Control { get { return this; } }
		bool ICustomBarControl.ProcessKeyPress(KeyPressEventArgs e) { return false; }
		#endregion
		#region Handler
		protected override void OnLostCapture() {
			base.OnLostCapture();
			Handler.OnLostCapture();
		}
		internal virtual DXMouseEventArgs CreateDXMouseEventArgs(MouseEventArgs e) { return DXMouseEventArgs.GetMouseArgs(e); }
		internal virtual DXMouseEventArgs CreateDXMouseEventArgs(Control control, EventArgs e) { return DXMouseEventArgs.GetMouseArgs(control, e); }
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = CreateDXMouseEventArgs(e);
			try {
				base.OnMouseDown(ee);
				if(ee.Handled) return;
				Handler.OnMouseDown(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = CreateDXMouseEventArgs(e);
			try {
				base.OnMouseUp(ee);
				if(ee.Handled) return;
				Handler.OnMouseUp(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = CreateDXMouseEventArgs(e);
			try {
				base.OnMouseMove(ee);
				if(ee.Handled) return;
				Handler.OnMouseMove(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			if(Ribbon == null) return;
			DXMouseEventArgs ee = CreateDXMouseEventArgs(this, e);
			try {
				base.OnMouseEnter(ee);
				if(ee.Handled) return;
				Handler.OnMouseEnter(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			if(Ribbon == null) return;
			DXMouseEventArgs ee = CreateDXMouseEventArgs(this, e);
			try {
				base.OnMouseLeave(ee);
				if(ee.Handled) return;
				Handler.OnMouseLeave(ee);
			}
			finally {
				ee.Sync();
			}
		}
		DevExpress.XtraBars.Ribbon.Handler.BaseHandler handler;
		protected internal DevExpress.XtraBars.Ribbon.Handler.BaseHandler Handler {
			get {
				if(handler == null) {
					if(Ribbon == null) 
						handler = new EmptyHandler();
					else
						handler = CreateHandler();
				}
				return handler;
			}
		}
		protected virtual DevExpress.XtraBars.Ribbon.Handler.BaseHandler CreateHandler() { return new RibbonStatusBarHandler(this); }
		#endregion Handler
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate { get { return !DesignMode; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		#endregion
		ToolTipController toolTipController;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonStatusBarToolTipController"),
#endif
DefaultValue(null)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				} else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return !DesignMode; } }
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			return ViewInfo.GetToolTipInfo(point);
		}
		bool IEditorBackgroundProvider.DrawBackground(Control ctrl, GraphicsCache cache) {
			Rectangle rightBounds = ViewInfo.GetRightPartBounds();
			SkinElementInfo info = null;
			info = ViewInfo.GetStatusBarInfo();
			info.Bounds = ctrl.RectangleToClient(RectangleToScreen(info.Bounds));
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			if(rightBounds.Contains(ctrl.Bounds)) {
				info = ViewInfo.GetRightStatusBarDrawInfo();
				info.Bounds = ctrl.RectangleToClient(RectangleToScreen(info.Bounds));
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
			return true;
		}
		protected internal virtual void OnAnimation(BaseAnimationInfo info) {
			if(IsDesignMode) return;
			BarAnimatedItemsHelper.Animate(this, Manager, info, ViewInfo.AnimationInvoker);
		}
		#region DragEvents
		bool ProcessDragOperations { get { return Manager != null && Manager.Helper != null && Manager.Helper.DragManager != null; } }
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(!ProcessDragOperations)
				return;
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, this);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(!ProcessDragOperations)
				return;
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, this);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if(!ProcessDragOperations) {
				e.Effect = DragDropEffects.None;
				return;
			}
			Manager.Helper.DragManager.FireDoDragging = false;
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected override void OnDragLeave(EventArgs e) {
			if(!ProcessDragOperations)
				return;
			Manager.Helper.DragManager.FireDoDragging = true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(!ProcessDragOperations)
				return;
			Manager.Helper.DragManager.StopDragging(this, e.Effect, false);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!ProcessDragOperations) {
				e.Effect = DragDropEffects.None;
				return;
			}
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
			Manager.Helper.DragManager.DoDragging(this, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
		}
		#endregion
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonStatusBarMerge")]
#endif
		public event RibbonStatusBarMergeEventHandler Merge {
			add { Events.AddHandler(merge, value); }
			remove { Events.RemoveHandler(merge, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonStatusBarUnMerge")]
#endif
		public event RibbonStatusBarMergeEventHandler UnMerge {
			add { Events.AddHandler(unMerge, value); }
			remove { Events.RemoveHandler(unMerge, value); }
		}
		protected virtual void RaiseMerge(RibbonStatusBarMergeEventArgs e) {
			RibbonStatusBarMergeEventHandler handler = Events[merge] as RibbonStatusBarMergeEventHandler;
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseUnMerge(RibbonStatusBarMergeEventArgs e) {
			RibbonStatusBarMergeEventHandler handler = Events[unMerge] as RibbonStatusBarMergeEventHandler;
			if(handler != null) handler(this, e);
		}
		#region ISupportAdornerUIManager Members
		readonly static object updateVisualEffects = new object();
		event UpdateActionEventHandler ISupportAdornerUIManager.Changed {
			add { Events.AddHandler(updateVisualEffects, value); }
			remove { Events.RemoveHandler(updateVisualEffects, value); }
		}
		void ISupportAdornerUIManager.UpdateVisualEffects(UpdateAction action) { UpdateVisualEffects(action); }
		protected void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[updateVisualEffects] as UpdateActionEventHandler;
			if(handler == null) return;
			handler(this, new UpdateActionEvendArgs(action));
		}
		#endregion
	}
}
