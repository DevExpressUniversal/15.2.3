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

using System.Drawing.Design;
using System.ComponentModel;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.Controls;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using System;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Drawing.Animation;
using System.ComponentModel.Design;
namespace DevExpress.XtraBars.Ribbon {
	[ToolboxItem(false), DesignTimeVisible(true),
		Designer("DevExpress.XtraBars.Ribbon.Design.RibbonMiniToolbarDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
		DesignerCategory("Component"),
		Description("Displays mini toolbar that can be displayed at any position of screen.")]
	public class RibbonMiniToolbar : Component {
		private static readonly object hiding = new object();
		RibbonMiniToolbarCollection collection;
		RibbonMiniToolbarItemLinkCollection itemLinks;
		RibbonMiniToolbarOpacityOptions opacityOptions;
		Point showPoint;
		Control parentControl;
		bool enabled;
		ContentAlignment alignment;
		bool allowShowingWhenPopupIsOpen;
		object tag;
		bool toolbarUsed;
		PopupMenu popupMenu;
		public RibbonMiniToolbar(IContainer container) : this() {
			container.Add(this);
		}
		public RibbonMiniToolbar() {
			this.opacityOptions = new RibbonMiniToolbarOpacityOptions();
			this.itemLinks = CreateItemLinks();
			this.parentControl = null;
			this.enabled = false;
			this.allowShowingWhenPopupIsOpen = false;
			this.alignment = ContentAlignment.BottomRight;
			this.tag = null;
			this.toolbarUsed = false;
			this.popupMenu = null;
			this.AllowToolTips = true;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(Collection != null)
					Collection.Remove(this);
				ResetForm();
			}
		}
		internal int GetTransparencyDistance() { return ToolbarUsed ? OpacityOptions.TransparencyDistanceWhenBarHovered : OpacityOptions.TransparencyDistance; }
		protected virtual RibbonMiniToolbarItemLinkCollection CreateItemLinks() {
			return new RibbonMiniToolbarItemLinkCollection(this);
		}
		protected internal RibbonMiniToolbarCollection Collection {
			get { return collection; }
			set {
				if(Collection == value)
					return;
				if(Collection != null)
					Collection.Remove(this);
				collection = value;
				if(Collection != null)
					Collection.Add(this);
			}
		}
		bool ShouldSerializeOpacityOptions() { return OpacityOptions != RibbonMiniToolbarOpacityOptions.Default; }
		void ResetOpacityOptions() { OpacityOptions = RibbonMiniToolbarOpacityOptions.Default; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonMiniToolbarOpacityOptions OpacityOptions {
			get { return opacityOptions; }
			set {
				if(OpacityOptions == value)
					return;
				opacityOptions = value;
				OnOpacityOptionsChanged();
			}
		}
		[DefaultValue(true)]
		public bool AllowToolTips { get; set; }
		protected internal bool ToolbarUsed {
			get { return toolbarUsed; }
			set { toolbarUsed = value; }
		}
		protected virtual void OnOpacityOptionsChanged() {
			UpdateVisibility(Control.MousePosition);
		}
		[DefaultValue((string)null)]
		public PopupMenu PopupMenu {
			get { return popupMenu; }
			set {
				if(PopupMenu == value)
					return;
				OnPopupMenuChanging();
				popupMenu = value;
				OnPopupMenuChanged();
			}
		}
		protected virtual void OnPopupMenuChanging() {
			if(PopupMenu != null && PopupMenu.RibbonToolbar == this) {
				PopupMenu.RibbonToolbar = null;
			}
		}
		protected virtual void OnPopupMenuChanged() {
			if(PopupMenu != null)
				PopupMenu.RibbonToolbar = this;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DevExpress.Utils.Design.InheritableCollection]
		public RibbonMiniToolbarItemLinkCollection ItemLinks { get { return itemLinks; } }
		public RibbonControl Ribbon { get { return Collection != null ? Collection.Ribbon : null; } }
		bool ShouldSerializeTag() { return Tag != null; }
		void ResetTag() { Tag = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonMiniToolbarTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		bool ShouldSerializeAlignment() { return Alignment != ContentAlignment.BottomRight; }
		void ResetAlignment() { Alignment = ContentAlignment.BottomRight; }
		public ContentAlignment Alignment {
			get { return alignment; }
			set {
				if(Alignment == value)
					return;
				alignment = value;
				OnAlignmentChanged();
			}
		}
		[DefaultValue(false)]
		public bool AllowShowingWhenPopupIsOpen {
			get { return allowShowingWhenPopupIsOpen; }
			set {
				if(AllowShowingWhenPopupIsOpen == value)
					return;
				allowShowingWhenPopupIsOpen = value;
				OnAllowShowingWhenPopupIsOpenChanged();
			}
		}
		protected virtual void OnAllowShowingWhenPopupIsOpenChanged() {
			UpdateVisibility(Control.MousePosition);
		}
		protected virtual void OnAlignmentChanged() {
			UpdateLocation();
		}
		protected virtual void UpdateLocation() {
			Form.UpdateToolbarLocation();
		}
		protected internal virtual Point ShowPoint {
			get { return showPoint; }
			set {
				if(ShowPoint == value)
					return;
				showPoint = value;
				OnShowPointChanged();
			}
		}
		protected virtual void OnShowPointChanged() {
			Form.ShouldUpdateToolbarBounds = true;
			Form.CheckToolbarBounds();
		}
		internal bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value)
					return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		protected virtual void OnEnabledChanged() {
			if(!Enabled)
				UpdateVisibility(Point.Empty);
		}
		protected internal Size CalcBestSize() {
			Form.ToolbarControl.UpdateToolbarSize();
			return Form.ToolbarControl.Size;
		}
		public void Show(Point pt) {
			this.toolbarUsed = false;
			if(PopupMenu != null && !PopupMenu.Visible) {
				Enabled = true;
				PopupMenu.ShowPopup(pt);
			}
			else {
				Control parent = GetParentControl();
				if(parent != null)
					ShowPoint = parent.PointToClient(pt);
				Enabled = true;
				UpdateVisibility(pt, parent);
			}
		}
		public void Hide() {
			if(!Enabled)
				return;
			if(!GetHiding())
				return;
			Enabled = false;
		}
		[DefaultValue((string)null)]
		public Control ParentControl {
			get { return parentControl; }
			set {
				if(ParentControl == value)
					return;
				OnParentControlChanging();
				parentControl = value;
				OnParentControlChanged();
			}
		}
		protected internal Control GetParentControl() {
			if(ParentControl != null)
				return ParentControl;
			if(Ribbon != null)
				return Ribbon.FindForm();
			return null;
		}
		RibbonMiniToolbarPopupForm form;
		protected internal RibbonMiniToolbarPopupForm Form {
			get {
				if(form == null)
					form = CreateForm();
				return form;
			}
		}
		internal void ResetForm() {
			if(form != null) {
				form.Close();
				form.Dispose();
			}
			form = null;
		}
		protected internal RibbonMiniToolbarControl ToolbarControl {
			get {
				return Form == null ? null : Form.ToolbarControl;
			}
		}
		protected virtual RibbonMiniToolbarPopupForm CreateForm() {
			return new RibbonMiniToolbarPopupForm(this);
		}
		protected virtual void OnParentControlChanging() {
			if(ParentControl != null)
				ParentControl.LocationChanged -= new EventHandler(OnParentControlLocationChanged);
		}
		protected virtual void OnParentControlChanged() {
			if(ParentControl != null) {
				ParentControl.LocationChanged += new EventHandler(OnParentControlLocationChanged);
				if(Ribbon != null && !Ribbon.IsDesignMode)
					Form.UpdateToolbarBounds();
			}
		}
		protected virtual void OnParentControlLocationChanged(object sender, EventArgs e) {
			Form.UpdateToolbarLocation();
		}
		protected internal void UpdateVisibility(Point pt) {
			UpdateVisibility(pt, null);
		}
		protected internal void UpdateVisibility(Point pt, Control control) {
			if(Ribbon == null || Ribbon.IsDesignMode)
				return;
			Form.UpdateToolbarVisibility(pt, control);
		}
		protected internal virtual void OnControllerChanged() {
			ToolbarControl.Refresh();
			Form.ShouldUpdateToolbarBounds = true;
		}
		public event CancelEventHandler Hiding {
			add { Events.AddHandler(hiding, value); }
			remove { Events.RemoveHandler(hiding, value); }
		}
		void RaiseHiding(CancelEventArgs e) {
			CancelEventHandler handler = Events[hiding] as CancelEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual bool GetHiding() {
			CancelEventArgs e = new CancelEventArgs();
			RaiseHiding(e);
			return !e.Cancel;
		}
		internal bool ContainsLink(BarItemLink newLink) {
			if(newLink.RibbonItemInfo == null) return false;
			return ToolbarControl.ViewInfo.Items.Contains(newLink.RibbonItemInfo) || ToolbarControl.ViewInfo.Items.Contains(newLink.RibbonItemInfo.Owner as RibbonButtonGroupItemViewInfo);
		}
		public override string ToString() {
			string name = base.ToString();
			if(Site != null && !string.IsNullOrEmpty(Site.Name)) {
				name = Site.Name;
			}
			return name;
		}
	}
	public class RibbonMiniToolbarItemLinkCollection : BaseRibbonItemLinkCollection {
		RibbonMiniToolbar toolbar;
		public RibbonMiniToolbarItemLinkCollection(RibbonMiniToolbar toolbar) {
			this.toolbar = toolbar;
		}
		public RibbonMiniToolbar Toolbar { get { return toolbar; } }
		public override RibbonControl Ribbon { get { return Toolbar.Ribbon; } }
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			Toolbar.Form.ShouldUpdateToolbarBounds = true;
			Toolbar.Form.ToolbarControl.ViewInfo.IsReady = false;
		}
		#region TODO
		protected override DevExpress.Utils.VisualEffects.ISupportAdornerUIManager GetVisualEffectsOwner() { return null; }
		protected override bool GetVisulEffectsVisible() { return false; }
		#endregion
	}
	public class RibbonMiniToolbarControl : ControlBase, ICustomBarControl, IToolTipControlClient, IBarObject, ISupportXtraAnimation {
		RibbonMiniToolbar toolbar;
		RibbonMiniToolbarViewInfo viewInfo;
		RibbonMiniToolbarPainter painter;
		RibbonMiniToolbarHandler handler;
		public RibbonMiniToolbarControl(RibbonMiniToolbar toolbar) {
			this.toolbar = toolbar;
			ToolTipController.DefaultController.AddClientControl(this);
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.Selectable, false);
		}
		public RibbonMiniToolbar Toolbar { get { return toolbar; } }
		public RibbonControl Ribbon { get { return Toolbar.Ribbon; } }
		public RibbonMiniToolbarPopupForm ToolbarForm { get { return Toolbar.Form; } }
		protected internal RibbonMiniToolbarViewInfo ViewInfo { 
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			} 
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ToolTipController.DefaultController.RemoveClientControl(this);
			}
			base.Dispose(disposing);
		}
		protected virtual RibbonMiniToolbarViewInfo CreateViewInfo() {
			return new RibbonMiniToolbarViewInfo(this);
		}
		protected RibbonMiniToolbarPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual RibbonMiniToolbarPainter CreatePainter() {
			return new RibbonMiniToolbarPainter();
		}
		protected internal RibbonMiniToolbarHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected virtual RibbonMiniToolbarHandler CreateHandler() {
			return new RibbonMiniToolbarHandler(this);
		}
		protected internal virtual void UpdateToolbarSize() {
			ViewInfo.CalcViewInfo(ClientRectangle);
			Size = ViewInfo.Bounds.Size;
		}
		protected internal virtual void OnAnimation(BaseAnimationInfo info) {
			if(DesignMode) return;
			BarAnimatedItemsHelper.Animate(this, Ribbon.Manager, info, ViewInfo.AnimationInvoker);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			CheckViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Painter.Draw(cache, ViewInfo);
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Refresh();
		}
		protected virtual void CheckViewInfo() {
			if(!ViewInfo.IsReady) {
				ViewInfo.CalcViewInfo(ClientRectangle);
			}
		}
		public override void Refresh() {
			ViewInfo.IsReady = false;
			Invalidate();
		}
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
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			Handler.OnKeyPress(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			Handler.OnKeyUp(e);
		}
		#endregion Handler
		#region ICustomBarControl Members
		void ICustomBarControl.ProcessKeyDown(KeyEventArgs e) {
			Handler.OnKeyDown(e);
		}
		bool ICustomBarControl.ProcessKeyPress(KeyPressEventArgs e) {
			return false;
		}
		bool ICustomBarControl.IsNeededKey(KeyEventArgs e) {
			return Handler.IsNeededKey(e);
		}
		bool ICustomBarControl.IsInterceptKey(KeyEventArgs e) {
			return Handler.IsInterceptKey(e);
		}
		void ICustomBarControl.ProcessKeyUp(KeyEventArgs e) {
			Handler.OnKeyUp(e);
		}
		Control ICustomBarControl.Control {
			get { return this; }
		}
		#endregion
		#region IBarObject Members
		bool IBarObject.IsBarObject {
			get { return true; }
		}
		BarManager IBarObject.Manager {
			get { return Ribbon.Manager; }
		}
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			Point p = PointToClient(new Point(e.X, e.Y));
			RibbonHitInfo info = ViewInfo.CalcHitInfo(p);
			if(info.InItem)
				return BarMenuCloseType.None;
			if(ClientRectangle.Contains(p))
				return BarMenuCloseType.AllExceptMiniToolbars;
			return BarMenuCloseType.All;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(Ribbon.Manager.SelectionInfo.ActiveEditor != null) {
				if(Ribbon.Manager.SelectionInfo.OpenedPopups.Count == 0) return true;
			}
			return false; 
		}
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) {
			return true;
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		#region IToolTipControlClient Members
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(!ViewInfo.IsRibbonFormActive) return null;
			ToolTipControlInfo res = ViewInfo.GetToolTipInfo(point);
			if(res != null) res.ToolTipType = ToolTipType.SuperTip;
			return res;
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return !DesignMode && Toolbar.AllowToolTips; }
		}
		#endregion
	}
	public class RibbonMiniToolbarPopupForm : CustomTopForm, IBarObject, DevExpress.Utils.IFocusablePopupForm, ISupportToolTipsForm {
		RibbonMiniToolbar toolbar;
		bool shouldUpdateToolbarBounds = false;
		public RibbonMiniToolbarPopupForm(RibbonMiniToolbar toolbar)
			: base() {
			this.toolbar = toolbar;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			Controls.Add(new RibbonMiniToolbarControl(Toolbar));
			Opacity = 0;
			Visible = false;
		}
		protected override void SetVisibleCore(bool newVisible) {
			if(Visible == newVisible) {
				SetVisibleCoreBase(newVisible);
			}
			else 
				base.SetVisibleCore(newVisible);
		}
		protected internal bool ShouldUpdateToolbarBounds { get { return shouldUpdateToolbarBounds; } set { shouldUpdateToolbarBounds = value; } }
		public RibbonMiniToolbar Toolbar { get { return toolbar; } }
		public RibbonControl Ribbon { get { return Toolbar.Ribbon; } }
		protected internal RibbonMiniToolbarControl ToolbarControl { get { return (RibbonMiniToolbarControl)Controls[0]; } }
		protected internal void UpdateToolbarBounds() {
			ToolbarControl.UpdateToolbarSize();
			UpdateToolbarSize();
			UpdateToolbarLocation();
		}
		protected virtual void UpdateToolbarSize() {
			Size = ToolbarControl.Size;
		}
		void ResetToolbar() {
			ToolbarControl.Handler.UpdateHotObject(new DXMouseEventArgs(MouseButtons.None, 1, -10000, -10000, 0), true);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) {
				HideImageForms();
				if(Toolbar.GetTransparencyDistance() > 0)
					Toolbar.Hide();
			}
		}
		private void HideImageForms() {
			foreach(RibbonItemViewInfo itemInfo in ToolbarControl.ViewInfo.Items) {
				InRibbonGalleryRibbonItemViewInfo galleryInfo = itemInfo as InRibbonGalleryRibbonItemViewInfo;
				if(galleryInfo != null)
					galleryInfo.GalleryInfo.Gallery.HideImageForms(null, true);
			}
		}
		protected virtual Point UpdateLocationByAlignment(Point pt) {
			switch(Toolbar.Alignment) {
				case ContentAlignment.TopLeft: pt.Offset(-Size.Width, -Size.Height); break;
				case ContentAlignment.TopCenter: pt.Offset(-Size.Width / 2, -Size.Height); break;
				case ContentAlignment.TopRight: pt.Offset(0, -Size.Height); break;
				case ContentAlignment.MiddleLeft: pt.Offset(-Size.Width, -Size.Height / 2); break;
				case ContentAlignment.MiddleCenter: pt.Offset(-Size.Width / 2, -Size.Height / 2); break;
				case ContentAlignment.MiddleRight: pt.Offset(0, -Size.Height / 2); break;
				case ContentAlignment.BottomLeft: pt.Offset(-Size.Width, 0); break;
				case ContentAlignment.BottomCenter: pt.Offset(-Size.Width / 2, 0); break;
			}
			return pt;
		}
		bool IsAnyPopupOpen {
			get { return Ribbon.Manager.SelectionInfo.OpenedPopups.Count > 0; }
		}
		protected internal virtual bool ToolbarHasOpenedPopups() {
			foreach(IPopup popup in Ribbon.Manager.SelectionInfo.OpenedPopups) {
				if(ToolbarControl.ViewInfo.ContainsLink(popup.OwnerLink)) {
					return true;
				}
			}
			return false;
		}
		protected internal virtual void UpdateToolbarLocation() {
			Point loc = Point.Empty;
			Control parentControl = Toolbar.GetParentControl();
			if(parentControl != null && parentControl.IsHandleCreated)
				loc = parentControl.PointToScreen(Toolbar.ShowPoint);
			else return;
			loc = UpdateLocationByAlignment(loc);
			loc = UpdateLocationByScreen(loc);
			Location = loc;
		}
		protected virtual Point UpdateLocationByScreen(Point loc) {
			Rectangle rect = new Rectangle(loc, Size);
			Screen sc = Screen.FromRectangle(rect);
			if(rect.X < sc.WorkingArea.X)
				rect.X = sc.WorkingArea.X;
			if(rect.Right > sc.WorkingArea.Right)
				rect.X -= (rect.Right - sc.WorkingArea.Right);
			if(rect.Y < sc.WorkingArea.Y)
				rect.Y = sc.WorkingArea.Y;
			if(rect.Bottom > sc.WorkingArea.Bottom)
				rect.Y -= rect.Bottom - sc.WorkingArea.Bottom;
			return rect.Location;
		}
		protected internal virtual void CheckToolbarBounds() {
			if(!ToolbarControl.ViewInfo.IsReady || ShouldUpdateToolbarBounds) {
				UpdateToolbarBounds();
				ShouldUpdateToolbarBounds = !ToolbarControl.ViewInfo.IsReady;
			}
		}
		bool ContainsPoint(Control control, Point pt) {
			return control.RectangleToScreen(control.ClientRectangle).Contains(pt);
		}
		protected virtual bool ShouldHideToolbar(double dist, Point pt, Control control) {
			if(!Toolbar.AllowShowingWhenPopupIsOpen && IsAnyPopupOpen) {
				return !ToolbarHasOpenedPopups();
			}
			if(control is IPopup && ContainsPoint(control, pt))
				return true;
			int hidingDistance = Toolbar.GetTransparencyDistance();
			return Ribbon.IsKeyboardActive || Ribbon.KeyTipManager.Show || !Toolbar.Enabled || dist > hidingDistance;
		}
		void ShowForm() {
			Visible = true;
		}
		protected internal void UpdateToolbarVisibility(Point pt, Control control) {
			CheckToolbarBounds();
			double dist = CalcDistance(pt);
			if(!Toolbar.ToolbarUsed && dist < Toolbar.OpacityOptions.OpacityDistance)
				Toolbar.ToolbarUsed = true;
			if(Toolbar.Enabled && ShouldStayVisible(dist, pt, control)) {
				Opacity = 0.99;
				ShowForm();
			}
			else if(ShouldHideToolbar(dist, pt, control)) {
				ResetToolbar();
				if(dist > Toolbar.GetTransparencyDistance())
					Visible = false;
				Opacity = 0;
			}
			else {
				if(dist > Toolbar.GetTransparencyDistance())
					Opacity = 0.0;
				else
					Opacity = Math.Min(0.99, Math.Sin((Math.PI / 2) * (1.0 - dist / (Toolbar.GetTransparencyDistance() - Toolbar.OpacityOptions.OpacityDistance))));
				ShowForm();
			}
		}
		protected virtual bool ShouldStayVisible(double distance, Point pt, Control control) {
			if(!Toolbar.OpacityOptions.AllowTransparency)
				return true;
			if(!Toolbar.AllowShowingWhenPopupIsOpen && IsAnyPopupOpen)
				return ToolbarHasOpenedPopups();
			if(Ribbon.Manager.ActiveEditor != null && Ribbon.Manager.ActiveEditor.Parent == ToolbarControl)
				return true;
			if(ToolbarHasOpenedPopups())
				return true;
			if(control is IPopup && ContainsPoint(control, pt))
				return false;
			if(distance < Toolbar.OpacityOptions.OpacityDistance)
				return true;
			if(Ribbon.IsKeyboardActive || Ribbon.KeyTipManager.Show)
				return false;
			return false;
		}
		double CalcDistance(Point pt) {
			const int TopLeft = 0x00, TopCenter = 0x01, TopRight = 0x02,
				MiddleLeft = 0x04, MiddleCenter = 0x05, MiddleRight = 0x06,
				BottomLeft = 0x08, BottomCenter = 0x09, BottomRight = 0x0a;
			int areaCode = 0;
			if(pt.X > Bounds.X)
				areaCode |= pt.X > Bounds.Right ? 0x02 : 0x01;
			if(pt.Y > Bounds.Y)
				areaCode |= pt.Y > Bounds.Bottom ? 0x08 : 0x04;
			switch(areaCode) {
				case MiddleCenter: return 0.0;
				case TopLeft: return Math.Sqrt((pt.X - Bounds.X) * (pt.X - Bounds.X) + (pt.Y - Bounds.Y) * (pt.Y - Bounds.Y));
				case TopRight: return Math.Sqrt((pt.X - Bounds.Right) * (pt.X - Bounds.Right) + (pt.Y - Bounds.Y) * (pt.Y - Bounds.Y));
				case BottomLeft: return Math.Sqrt((pt.X - Bounds.X) * (pt.X - Bounds.X) + (pt.Y - Bounds.Bottom) * (pt.Y - Bounds.Bottom));
				case BottomRight: return Math.Sqrt((pt.X - Bounds.Right) * (pt.X - Bounds.Right) + (pt.Y - Bounds.Bottom) * (pt.Y - Bounds.Bottom));
				case TopCenter: return Bounds.Y - pt.Y;
				case BottomCenter: return pt.Y - Bounds.Bottom;
				case MiddleLeft: return Bounds.X - pt.X;
				case MiddleRight: return pt.X - Bounds.Right;
			}
			return double.MaxValue;
		}
		#region IBarObject Members
		bool IBarObject.IsBarObject {
			get { return true; }
		}
		BarManager IBarObject.Manager {
			get { return Ribbon.Manager; }
		}
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return ((IBarObject)ToolbarControl).ShouldCloseMenuOnClick(e, child);
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			return ((IBarObject)ToolbarControl).ShouldCloseOnOuterClick(control, e);
		}
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) {
			return ((IBarObject)ToolbarControl).ShouldCloseOnLostFocus(newFocus);
		}
		#endregion
		bool IFocusablePopupForm.AllowFocus {
			get { return false; }
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return true;
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return true; }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonMiniToolbarOpacityOptions {
		static RibbonMiniToolbarOpacityOptions _default;
		public static RibbonMiniToolbarOpacityOptions Default {
			get {
				if(_default == null)
					_default = new RibbonMiniToolbarOpacityOptions(11, 44, 176);
				return _default;
			}
		}
		int transparencyDistanceWhenBarHovered;
		int transparencyDistance;
		int opacityDistance;
		bool allowTransaprency;
		public RibbonMiniToolbarOpacityOptions() {
			this.opacityDistance = Default.OpacityDistance;
			this.transparencyDistance = Default.TransparencyDistance;
			this.transparencyDistanceWhenBarHovered = Default.transparencyDistanceWhenBarHovered;
			this.allowTransaprency = true;
		}
		public RibbonMiniToolbarOpacityOptions(int opacityDistance, int transparencyDistance, int transparencyDistanceWhenBarHovered) {
			this.opacityDistance = opacityDistance;
			this.transparencyDistance = transparencyDistance;
			this.transparencyDistanceWhenBarHovered = transparencyDistanceWhenBarHovered;
			this.allowTransaprency = true;
		}
		[DefaultValue(11)]
		public int OpacityDistance {
			get { return opacityDistance; }
			set {
				if(value >= TransparencyDistance)
					value = TransparencyDistance - 1;
				opacityDistance = value; 
			}
		}
		[DefaultValue(44)]
		public int TransparencyDistance {
			get { return transparencyDistance; }
			set {
				if(value < OpacityDistance)
					value = OpacityDistance + 1;
				transparencyDistance = value;
				if(TransparencyDistanceWhenBarHovered < value)
					TransparencyDistanceWhenBarHovered = value + 1;
			}
		}
		[DefaultValue(176)]
		public int TransparencyDistanceWhenBarHovered {
			get { return transparencyDistanceWhenBarHovered; }
			set {
				if(value > 0 && value < TransparencyDistance)
					value = TransparencyDistance + 1;
				transparencyDistanceWhenBarHovered = value;
				if(TransparencyDistance >= value)
					TransparencyDistance = value - 1;
			}
		}
		[DefaultValue(true)]
		public bool AllowTransparency {
			get { return allowTransaprency; }
			set { allowTransaprency = value; }
		}
		public override string ToString() {
			return "{" + OpacityDistance + ", " + TransparencyDistance + ", " + TransparencyDistanceWhenBarHovered + "}";
		}
		public override int GetHashCode() {
			return OpacityDistance ^ TransparencyDistance ^ TransparencyDistanceWhenBarHovered;
		}
		public override bool Equals(object obj) {
			RibbonMiniToolbarOpacityOptions op = obj as RibbonMiniToolbarOpacityOptions;
			if(op == null)
				return base.Equals(obj);
			return op.OpacityDistance == OpacityDistance && op.TransparencyDistance == TransparencyDistance && op.TransparencyDistanceWhenBarHovered == TransparencyDistanceWhenBarHovered;
		}
	}
}
