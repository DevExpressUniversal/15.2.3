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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Ribbon {
	[DXToolboxItem(true)]
	public class RibbonApplicationButtonContainerControl : ContainerControl {
		RibbonControl ribbon;
		public RibbonApplicationButtonContainerControl(RibbonControl ribbon) {
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
			this.ribbon = ribbon;
		}
		protected internal RibbonControl Ribbon { get { return ribbon; } }
		protected Form Form { get { return ParentControl as Form; } set { ParentControl = value; } }
		protected Control ParentControl { get; set; }
		protected internal Form TopFormCore {
			get {
				if(ParentControl == null) return null;
				Form form = ParentControl as Form;
				return form != null ? form : ParentControl.FindForm();
			}
		}
		protected RibbonForm RibbonForm { get { return Form as RibbonForm; } }
		public virtual Control Content {
			get { return Controls.Count > 0 ? Controls[0] : null; }
			set {
				if(Content == value)
					return;
				OnAddContentControl(value);
				if(Content != null)
					Content.Dock = DockStyle.Fill;
			}
		}
		protected virtual void OnAddContentControl(Control control) {
			Controls.Clear();
			IBackstageViewControl bsvc = control as IBackstageViewControl;
			if(bsvc != null) {
				bsvc.OnBeforeShowing();
			}
			Controls.Add(control);
		}
		SkinElementInfo GetInfo() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(Ribbon.ViewInfo.Provider)[RibbonSkins.SkinApplicationButtonContainerControl], new Rectangle(Point.Empty, Size));
			if(Ribbon.ViewInfo.IsColored)
				info.ImageIndex = RibbonPainter.ImageIndexByColorScheme(Ribbon.ColorScheme, ObjectState.Normal, 1);
			return info;
		}
		protected Rectangle GetContentRect() {
			if(Content is BackstageViewControl)
				return ClientRectangle;
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				return ObjectPainter.GetObjectClientRectangle(gInfo.Graphics, SkinElementPainter.Default, GetInfo());
			}
			finally { gInfo.ReleaseGraphics(); }
		}
		public override Rectangle DisplayRectangle {
			get {
				return GetContentRect();
			}
		}
		bool statusBarVisible = false;
		bool GetStatusBarVisibility() {
			if(Ribbon.StatusBar == null) return false;
			return Ribbon.StatusBar.RealVisible;
		}
		void UpdateStatusBarVisibility(bool visible) {
			if(RibbonForm != null && !RibbonForm.IsGlassForm && Ribbon.StatusBar != null) {
				Ribbon.StatusBar.SetVisible(visible);
			}
			UpdateContentBounds();
		}
		void RestoreStatusBarVisibility() {
			if(Ribbon.StatusBar != null) {
				Ribbon.StatusBar.SetVisible(statusBarVisible);
			}
			UpdateContentBounds();
		}
		protected internal void CheckRibbonStatusBar() {
			if(RibbonForm == null || RibbonForm.StatusBar == null || !statusBarVisible) return;
			if(!ContentVisible) {
				RibbonForm.StatusBar.Visible = true;
				return;
			}
			RibbonForm.StatusBar.Visible = RibbonForm.IsGlassForm;
		}
		bool prevAutoScroll = false;
		public virtual void ShowContent(bool raiseShowing = true) {
			ParentControl = Ribbon.Parent;
			if(ParentControl == null)
				return;
			if(Form != null) {
				this.prevAutoScroll = Form.AutoScroll;
				Form.AutoScroll = false;
				Visible = false;
			}
			ParentControl.SizeChanged += new EventHandler(OnParentFormSizeChanged);
			statusBarVisible = GetStatusBarVisibility();
			UpdateStatusBarVisibility(false);
			ParentControl.Controls.Add(this);
			if(raiseShowing)
				RaiseOnShowing();
			BringToFront();
			Visible = true;
			BeginInvoke(new MethodInvoker(FocusContent));
		}
		protected internal virtual void RaiseOnShowing() {
			IBackstageViewControl bsvc = Content as IBackstageViewControl;
			if(bsvc != null) bsvc.OnShowing();
		}
		protected internal virtual void RaiseOnHiding() {
			IBackstageViewControl bsvc = Content as IBackstageViewControl;
			if(bsvc != null) bsvc.OnHiding();
		}
		protected internal virtual void RaiseOnHided() {
			IBackstageViewControl bsvc = Content as IBackstageViewControl;
			if(bsvc != null) bsvc.OnHided();
		}
		protected virtual void FocusContent() {
			Content.Focus();
		}
		int ApplicationContainerControlTopIndent {
			get {
				object res = RibbonSkins.GetSkin(Ribbon.ViewInfo.Provider).Properties[RibbonSkins.OptApplicationContainerControlTopIndent];
				if(res == null) return 0;
				return (int)res;
			}
		}
		protected internal virtual void UpdateContentBounds() {
			int indent = ApplicationContainerControlTopIndent;
			int clientHeight = ParentControl.DisplayRectangle.Height - (Ribbon.Top + Ribbon.ViewInfo.Header.Bounds.Bottom - Ribbon.ViewInfo.PanelYOffsetCore) - indent;
			Bounds = new Rectangle(0, ParentControl.DisplayRectangle.Top + Ribbon.Top + Ribbon.ViewInfo.Header.Bounds.Bottom - 1 + indent, Ribbon.Width, clientHeight);
			if(Content != null)
				Content.Bounds = DisplayRectangle;
			UpdateRegion();
		}
		protected virtual void UpdateRegion() {
			if(Content != null) Content.Region = null;
		}
		void OnParentFormSizeChanged(object sender, EventArgs e) {
			UpdateContentBounds();
		}
		public virtual void HideContent() {
			if(ParentControl == null)
				return;
			RaiseOnHiding();
			Ribbon.Focus();
			Visible = false;
			ParentControl.Controls.Remove(this);
			ParentControl.SizeChanged -= new EventHandler(OnParentFormSizeChanged);
			RestoreStatusBarVisibility();
			if(Form != null) {
				Form.AutoScroll = this.prevAutoScroll;
			}
			if(!SupportClosingAnimation) RaiseOnHided();
		}
		protected virtual bool SupportClosingAnimation { get { return false; } }
		public bool ContentVisible { get { return Parent != null; } }
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(Content is BackstageViewControl)
				return;
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetInfo());
			}
		}
	}
	public class RibbonOffice2013BackstageViewContainerControl : RibbonApplicationButtonContainerControl {
		ContentPopupForm popupForm;
		public RibbonOffice2013BackstageViewContainerControl(RibbonControl ribbon)
			: base(ribbon) {
			this.popupForm = CreateContentPopupForm();
		}
		protected virtual ContentPopupForm CreateContentPopupForm() {
			return new ContentPopupForm(this);
		}
		public override Control Content {
			get { return PopupForm.Content; }
		}
		public override void ShowContent(bool raiseShowing = true) {
			base.ShowContent(false);
			PopupForm.UpdateBackColor();
			PopupForm.ShowPopup();
			TopFormCore.BackColorChanged += OnTopFormBackColorChanged;
		}
		void OnTopFormBackColorChanged(object sender, EventArgs e) {
			PopupForm.UpdateBackColor();
		}
		public override void HideContent() {
			if(TopFormCore == null) return;
			XtraForm.SuppressDeactivation = true;
			try {
				base.HideContent();
				TopFormCore.BackColorChanged -= OnTopFormBackColorChanged; 
			}
			finally {
				XtraForm.SuppressDeactivation = false;
			}
		}
		protected override bool SupportClosingAnimation { get { return true; } }
		protected override void OnAddContentControl(Control control) {
			PopupForm.Controls.Clear();
			PopupForm.Controls.Add(control);
		}
		protected internal override void UpdateContentBounds() {
			if(!ShouldUpdateContentBounds || IsLockSizing)
				return;
			PopupForm.Bounds = CalcPopupFormBounds();
			UpdateRegion();
		}
		protected virtual bool IsLockSizing {
			get {
				if(RibbonForm == null)
					return false;
				return RibbonForm.LockSizing;
			}
		}
		protected virtual Rectangle CalcPopupFormBounds() {
			Rectangle bounds = TopFormCore.Bounds;
			if(!IsTopFormMaximized || IsFormBorderless)
				return bounds;
			Rectangle screenBounds = Screen.GetWorkingArea(TopFormCore);
			return Rectangle.Intersect(screenBounds, bounds);
		}
		protected bool IsTopFormMaximized {
			get {
				if(TopFormCore == null)
					return false;
				return TopFormCore.WindowState == FormWindowState.Maximized;
			}
		}
		protected bool IsFormBorderless {
			get { return Form != null && Form.FormBorderStyle == FormBorderStyle.None; }
		}
		protected virtual bool ShouldUpdateContentBounds {
			get { return BackstageViewInfo != null ? !BackstageViewInfo.SizingMode : true; }
		}
		protected Office2013BackstageViewInfo BackstageViewInfo {
			get {
				if(BackstageView == null)
					return null;
				return BackstageView.ViewInfo as Office2013BackstageViewInfo;
			}
		}
		protected override void UpdateRegion() {
			if(BackstageView == null) return;
			PopupForm.Region = GetRegion();
			Content.Region = GetRegion();
		}
		protected virtual Region GetRegion() {
			Region region = new Region(new Rectangle(Point.Empty, TopFormCore.Size));
			BackstageViewShowRibbonItems showItems = BackstageView.ViewInfo.BackstageViewShowRibbonItems;
			if(showItems == BackstageViewShowRibbonItems.None)
				return region;
			if(BackstageView.IsSkinCompatibleWithRibbonItems && (showItems & BackstageViewShowRibbonItems.FormButtons) != 0) {
				IEnumerable<Rectangle> itemsBounds = GetFormButtonsRegionRectangle();
				foreach(Rectangle bounds in itemsBounds) {
					region.Exclude(bounds);
				}
			}
			if(BackstageView.IsSkinCompatibleWithRibbonItems && (showItems & BackstageViewShowRibbonItems.PageHeaderItems) != 0) {
				Region headerItemsRegion = GetPageHeaderItemsRegion();
				if(headerItemsRegion != null) region.Exclude(headerItemsRegion);
			}
			return region;
		}
		protected virtual Rectangle GetTitleRegionRectangle() {
			Rectangle rect = Ribbon.ViewInfo.Caption.TextBounds;
			return CheckRegionBounds(rect);
		}
		protected virtual IEnumerable<Rectangle> GetFormButtonsRegionRectangle() {
			RibbonFormPainter formPainter = Ribbon.ViewInfo.Caption.FormPainter;
			if(formPainter == null)
				yield break;
			foreach(FormCaptionButton btn in formPainter.Buttons) {
				if(btn.Kind != FormCaptionButtonKind.FullScreen) {
					Rectangle bounds = btn.Bounds;
					yield return CheckRegionBounds(bounds);
				}
			}
		}
		protected virtual Region GetPageHeaderItemsRegion() {
			Rectangle headerBounds = GetPageHeaderItemsRegionRectangle();
			if(headerBounds.IsEmpty) return null;
			Region region = new Region(headerBounds);
			region.Exclude(GetRibbonExpandCollapseItemBounds());
			return region;
		}
		protected virtual Rectangle GetPageHeaderItemsRegionRectangle() {
			if(Ribbon.ViewInfo.Form == null || Ribbon.PageHeaderItemLinks.Count == 0) return Rectangle.Empty;
			Rectangle rect = CheckRegionBounds(Ribbon.ViewInfo.Header.PageHeaderItemsBounds);
			rect.Height--;
			return rect;
		}
		protected virtual Rectangle GetRibbonExpandCollapseItemBounds() {
			foreach(RibbonItemViewInfo viewInfo in Ribbon.ViewInfo.Header.PageHeaderItems) {
				RibbonExpandCollapseItemLink li = viewInfo.Item as RibbonExpandCollapseItemLink;
				if(li != null) return CheckRegionBounds(Rectangle.Inflate(li.Bounds, 0, 2));
			}
			return Rectangle.Empty;
		}
		protected virtual Rectangle CheckRegionBounds(Rectangle regionBounds) {
			Rectangle res = regionBounds;
			if(IsTopFormMaximized)
				res.Y -= BorderThickness;
			else res.X += BorderThickness;
			return res;
		}
		protected internal ContentPopupForm PopupForm { get { return this.popupForm; } }
		protected BackstageViewControl BackstageView { get { return Content as BackstageViewControl; } }
		protected int BorderThickness { get { return (TopFormCore.Width - TopFormCore.ClientSize.Width) / 2; } }
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(popupForm != null) {
					popupForm.Dispose();
					popupForm = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region PopupForm
		protected internal class ContentPopupForm : TopFormBase, IPopupControl, IMouseWheelContainer, IXtraAnimationListener {
			RibbonApplicationButtonContainerControl parent;
			public ContentPopupForm(RibbonApplicationButtonContainerControl parent) {
				this.parent = parent;
				SetStyle(ControlStyles.Selectable, false);
			}
			public void ShowPopup() {
				if(ParentFormCore != null) TopMost = ParentFormCore.TopMost;
				XtraForm.SuppressDeactivation = true;
				try {
					ForceCreateContentHandle();
					if(this.parent != null) {
						this.parent.RaiseOnShowing();
					}
					Show();
					ServiceObject.PopupShowing(this);
					ParentFormCore.AddOwnedForm(this);
					FocusFormControl(Content);
				}
				finally {
					XtraForm.SuppressDeactivation = false;
				}
			}
			bool isFormMinimized = false;
			bool isFormMaximized = false;
			protected override void OnLocationChanged(EventArgs e) {
				base.OnLocationChanged(e);				
				CheckFormWindowState();
			}
			protected virtual void CheckFormWindowState() {
				if(ParentFormCore.WindowState == FormWindowState.Minimized) {
					isFormMinimized = true;
				}
				if((isFormMinimized && (ParentFormCore.WindowState != FormWindowState.Minimized)) || (isFormMaximized && (ParentFormCore.WindowState != FormWindowState.Maximized))) {
					WindowState = FormWindowState.Normal;
					isFormMinimized = false;
					isFormMaximized = false;
					BeginInvoke(new MethodInvoker(ActivateContent));
				}
				if(ParentFormCore.WindowState == FormWindowState.Maximized) {
					isFormMaximized = true;
				}
			}
			protected void ActivateContent() {
				Activate();
				FocusFormControl(Content);
			}
			protected virtual void HidePopupCore() {
				Hide();
				if(ParentFormCore != null) {
					ServiceObject.PopupClosed(this);
					ParentFormCore.RemoveOwnedForm(this);
				}
				if(BackstageView != null) BackstageView.OnHided();
			}
			protected IBackstageViewControl BackstageView {
				get { return Content as IBackstageViewControl; }
			}
			protected virtual void ForceCreateContentHandle() {
				BackstageViewControl backstageView = Content as BackstageViewControl;
				if(backstageView == null)
					return;
				backstageView.ForceCreateHandle();
				backstageView.Visible = true;
			}
			public void UpdateBackColor() {
				BackColor = parent.BackColor;
			}
			public BackstageViewControl Content {
				get {
					if(Controls.Count != 1) return null;
					return Controls[0] as BackstageViewControl;
				}
			}
			protected Form ParentFormCore {
				get {
					if(parent == null) return null;
					return parent.TopFormCore;
				}
			}
			public bool IsTopFormMaximized {
				get {
					if(ParentFormCore == null)
						return false;
					return ParentFormCore.WindowState == FormWindowState.Maximized;
				}
			}
			protected virtual void FocusFormControl(Control control) {
				PopupHookHelper.FocusFormControl(control, ParentFormCore, ServiceObject);
			}
			static IPopupServiceControl popupServiceControl;
			public IPopupServiceControl ServiceObject {
				get {
					if(popupServiceControl == null) popupServiceControl = new ContentPopupFormHookPopupController();
					return popupServiceControl;
				}
			}
			#region IPopupControl
			bool IPopupControl.SuppressOutsideMouseClick { get { return false; } }
			bool IPopupControl.AllowMouseClick(Control control, Point mousePosition) {
				return Content.AllowMouseClick(control, mousePosition);
			}
			void IPopupControl.ClosePopup() {
				Content.CloseApplicationMenuCore();
			}
			Control IPopupControl.PopupWindow {
				get { return this; }
			}
			#endregion
			#region IXtraAnimationListener Members
			void IXtraAnimationListener.OnAnimation(BaseAnimationInfo info) {
				OnAnimationCore((BackstageViewTransitionAnimationInfo)info);
			}
			void IXtraAnimationListener.OnEndAnimation(BaseAnimationInfo info) {
				OnEndAnimationCore((BackstageViewTransitionAnimationInfo)info);
			}
			#endregion
			protected virtual void OnAnimationCore(BackstageViewTransitionAnimationInfo info) {
				if(info.FadeOut && !info.BackstageView.IsRightToLeft
					|| !info.FadeOut && info.BackstageView.IsRightToLeft)
					ResetRegionsCore();
			}
			protected virtual void OnEndAnimationCore(BackstageViewTransitionAnimationInfo info) {
				if(info.FadeOut && !info.BackstageView.IsRightToLeft
					|| !info.FadeOut && info.BackstageView.IsRightToLeft)
					HidePopupCore();
			}
			protected virtual void ResetRegionsCore() {
				Region = null;
				if(Content != null) Content.Region = null;
			}
			static readonly IntPtr SC_CLOSE = new IntPtr(0xF060);
			protected virtual bool OnWmSysCommand(Message msg) {
				if(msg.WParam == SC_CLOSE) {
					msg.Result = IntPtr.Zero;
					return true;
				}
				return false;
			}
			protected virtual void RedirectSysMessage(Message msg) {
				if(Content == null || Content.Ribbon == null) return;
				Form form = Content.Ribbon.FindForm();
				if(form != null && form.IsHandleCreated) NativeMethods.PostMessage(form.Handle, msg.Msg, msg.WParam, msg.LParam);
			}
			#region Disposing
			protected override void Dispose(bool disposing) {
				if(parent != null) parent = null;
				if(disposing) {
				}
				base.Dispose(disposing);
			}
			#endregion
			#region Hook Popup Controller
			public class ContentPopupFormHookPopupController : HookPopupController {
				public override void PopupShowing(IPopupControl popup) {
					Popups.Add(new ContentPopupFormHookPopup(popup));
				}
			}
			public class ContentPopupFormHookPopup : HookPopup {
				public ContentPopupFormHookPopup(IPopupControl popup)
					: base(popup) {
				}
				protected override bool PreFilterMessageCore(int Msg, IntPtr HWnd, IntPtr WParam) {
					if(Msg == MSG.WM_ACTIVATEAPP) {
						return true;
					}
					return base.PreFilterMessageCore(Msg, HWnd, WParam);
				}
			}
			#endregion
			#region IMouseWheelContainer
			void IMouseWheelContainer.BaseOnMouseWheel(MouseEventArgs e) {
				base.OnMouseWheel(e);
			}
			Control IMouseWheelContainer.Control {
				get { return this; }
			}
			#endregion
			protected override void OnMouseWheel(MouseEventArgs e) {
				MouseWheelHelper.DoMouseWheel(this, e);
			}
			protected override void WndProc(ref Message msg) {
				if(msg.Msg == MSG.WM_NCHITTEST) {
					msg.Result = new IntPtr(-1); 
					return;
				}
				if(msg.Msg == MSG.WM_SYSCOMMAND) {
					if(OnWmSysCommand(msg)) {
						RedirectSysMessage(msg);
						return;
					}
				}
				base.WndProc(ref msg);
			}
			protected override bool IsTopMost { get { return false; } }
		}
		#endregion
	}
}
