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
using System.Linq;
using System.Text;
using DevExpress.Utils.Internal;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.Ribbon {
	public class RadialMenuWindow : DXLayeredWindowEx, ISupportXtraAnimation, IXtraObjectWithBounds, IXtraAnimationListener {
		protected internal static object BoundAnimationId = new object();
		protected internal static object TextFadeAnimationId = new object();
		static int AnimationKoeff = 1;
		public RadialMenuWindow(RadialMenu menu) {
			Menu = menu;
		}
		public RadialMenu Menu { get; private set; }
		public bool Visible { get; protected set; }
		bool CaptureInternal {
			get { return NativeMethods.GetCapture() == Handle; }
			set {
				if(CaptureInternal != value) {
					if(value) NativeMethods.SetCapture(Handle);
					else NativeMethods.ReleaseCapture();
				}
			}
		}
		protected override Control CheckOwner() {
			Control control = base.CheckOwner();
			if(control != null && control.IsHandleCreated)
				return control;
			return null;
		}
		protected internal virtual void MakeCollapsed(bool animated) {
			UpdateSize(animated);
		}
		protected internal Point Location { get; set; }
		protected internal virtual void ShowPopup(Point point) {
			if(!IsCreated) {
				CreatePopupCore();
			}
			if(Menu.IsCustomizationMode)
				UpdateSize(false);
			Location = CalcLocation(point);
			Show(Location);
			Update();
			Visible = true;
		}
		protected virtual void CreatePopupCore() {
			Control topControl = GetTopControl();
			if(topControl == null) return;
			Create(topControl);
			Init();
		}
		protected internal Control GetTopControl() {
			if(Menu.Manager == null)
				return null;
			if(Menu.Manager.GetForm() != null)
				return Menu.Manager.GetForm();
			if(Menu.Manager.GetUserControl() != null)
				return Menu.Manager.GetUserControl();
			return null;
		}
		protected virtual Point CalcLocation(Point point) {
			point = new Point(point.X - Size.Width / 2, point.Y - Size.Height / 2);
			Screen screen = Screen.FromPoint(point);
			Rectangle formBounds = new Rectangle(point, Size);
			if(formBounds.Left < screen.WorkingArea.Left) {
				point.X = screen.WorkingArea.Left;
			}
			if(formBounds.Right > screen.WorkingArea.Right) {
				point.X = screen.WorkingArea.Right - formBounds.Width;
			}
			if(formBounds.Top < screen.WorkingArea.Top) {
				point.Y = screen.WorkingArea.Top;
			}
			if(formBounds.Bottom > screen.WorkingArea.Bottom) {
				point.Y = screen.WorkingArea.Bottom - formBounds.Height;
			}
			return point;
		}
		protected internal void UpdateMaximumSize() {
			Point centerPoint = new Point(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
			Size = ViewInfo.CalcMaximumSize(null);
			Location = new Point(centerPoint.X - Size.Width / 2, centerPoint.Y - Size.Height / 2);
			if(Menu.State == RadialMenuState.Expanded)
				ClientBounds = new Rectangle(Point.Empty, Size);
			UpdateViewInfo();
			if(Visible)
				Show(Location);
		}
		protected internal void UpdateSize() { UpdateSize(false); }
		protected internal void UpdateSize(bool animated) {
			Size newSize = ViewInfo.CalcBestSize(Menu.ActualLinksHolder);
			Point newLocation = new Point(Size.Width / 2 - newSize.Width / 2, Size.Height / 2 - newSize.Height / 2);
			if(!animated) {
				ClientBounds = new Rectangle(newLocation, newSize);
				OnBoundsAnimationCompleted();
			} else {
				ViewInfo.RenderContentToImage();
				if(Menu.State == RadialMenuState.Expanded) {
					CheckAnimation(RadialMenuWindow.BoundAnimationId, false);
					XtraAnimator.Current.AddAnimation(new BoundsAnimationInfo(this, this, RadialMenuWindow.BoundAnimationId, ClientBounds, new Rectangle(newLocation, newSize), 200 * AnimationKoeff, true, false));
				}
				else {
					CheckAnimation(RadialMenuWindow.BoundAnimationId, true);
					XtraAnimator.Current.AddAnimation(new BoundsAnimationInfo(this, this, RadialMenuWindow.BoundAnimationId, new Rectangle(newLocation, newSize), ClientBounds, 200 * AnimationKoeff, true, true));
				}
			}
		}
		protected virtual void CheckAnimation(object animationId, bool reverseSpline) {
			BoundsAnimationInfo info = XtraAnimator.Current.Animations[this, animationId] as BoundsAnimationInfo;
			if(info != null && reverseSpline != info.ReverseSpline)
				info.ForceLastFrameStep();
		}
		private void ClearSize() {
			Size = new Size(1, 1);
		}
		protected internal virtual void MakeExpanded() {
			UpdateSize(true);
		}
		#region ISupportXtraAnimation Members
		public bool CanAnimate {
			get { return Visible; }
		}
		public Control OwnerControl {
			get { return null; }
		}
		#endregion
		RadialMenuViewInfo viewInfo;
		protected internal RadialMenuViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected virtual RadialMenuViewInfo CreateViewInfo() {
			return new RadialMenuViewInfo(this);
		}
		#region Release Resources
		public override void DestroyHandle() {
			base.DestroyHandle();
		}
		public virtual void ReleaseAnimations() {
			AnimationInfoCollection col = XtraAnimator.Current.Animations;
			for(int i = 0; i < col.Count; i++) {
				BaseAnimationInfo info = col[i];
				if(object.ReferenceEquals(info.AnimatedObject, this)) {
					XtraAnimator.Current.Animations.Remove(info);
					--i;
				}
			}
		}
		#endregion
		RadialMenuPainter painter;
		protected internal RadialMenuPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual RadialMenuPainter CreatePainter() {
			return new RadialMenuPainter(ViewInfo);
		}
		Rectangle clientBounds;
		public Rectangle ClientBounds {
			get { return clientBounds; }
			set {
				if(ClientBounds == value)
					return;
				clientBounds = value;
				OnClientBoundsChanged();
			}
		}
		protected internal virtual void UpdateViewInfo() {
			ViewInfo.IsReady = false;
		}
		protected virtual void OnClientBoundsChanged() {
			UpdateViewInfo();
			Invalidate();
		}
		#region IXtraObjectWithBounds Members
		Rectangle IXtraObjectWithBounds.AnimatedBounds {
			get { return ClientBounds; }
			set { ClientBounds = value; }
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo anim) {
			XtraAnimator.Current.AddAnimation(new TextOpacityAnimationInfo(this, RadialMenuWindow.TextFadeAnimationId, 100, 500 * AnimationKoeff, 0.0f, 1.0f, true));
			OnBoundsAnimationCompleted();
		}
		protected virtual void OnBoundsAnimationCompleted() {
			if(!Menu.ShouldClose) return;
			bool raiseCloseUp = Menu.IsPopupActive && Menu.Manager != null && !Menu.Manager.IsDesignMode;
			DestroyWindow();
			if(raiseCloseUp) Menu.OnPopupClosed();
		}
		protected virtual void DestroyWindow() {
			Visible = false;
			Menu.OnHidingComplete();
			if(Menu.IsCustomizationMode) {
				Hide();
				return;
			}
			DestroyHandle();
		}
		#endregion
		protected virtual void CheckViewInfo(Graphics g) {
			if(!ViewInfo.IsReady)
				ViewInfo.CalcViewInfo(g, ClientBounds);
		}
		protected internal virtual void Init() {
			ClearSize();
			Size = ViewInfo.CalcMaximumSize(null);
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_LBUTTONDOWN || m.Msg == MSG.WM_RBUTTONDOWN) {
				Point pt = Docking2010.WinAPIHelper.GetPoint(m.LParam);
				OnMouseDown(new MouseEventArgs(m.Msg == MSG.WM_LBUTTONDOWN ? MouseButtons.Left : MouseButtons.Right, 1, pt.X, pt.Y, 0));
			}
			else if(m.Msg == MSG.WM_LBUTTONUP || m.Msg == MSG.WM_RBUTTONUP) {
				Point pt = Docking2010.WinAPIHelper.GetPoint(m.LParam);
				OnMouseUp(new MouseEventArgs(m.Msg == MSG.WM_LBUTTONUP ? MouseButtons.Left : MouseButtons.Right, 1, pt.X, pt.Y, 0));
			}
			else if(m.Msg == MSG.WM_MOUSEMOVE) {
				Point pt = Docking2010.WinAPIHelper.GetPoint(m.LParam);
				OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
			}
			else if(m.Msg == MSG.WM_MOUSELEAVE) {
				OnMouseLeave();
			}
			base.WndProc(ref m);
		}
		protected override void NCHitTest(ref Message m) {
			m.Result = new IntPtr(DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTCLIENT);
		}
		RadialMenuWindowHandler handler;
		protected internal RadialMenuWindowHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected virtual RadialMenuWindowHandler CreateHandler() {
			return new RadialMenuWindowHandler(this);
		}
		protected internal virtual void OnMouseLeave() {
			isMouseEnterRaised = false;
			Handler.OnMouseLeave();
			if(!Menu.IsCustomizationMode) CaptureInternal = false;
		}
		protected internal virtual void OnMouseEnter() {
			isMouseEnterRaised = true;
			Handler.OnMouseEnter();
			if(!Menu.IsCustomizationMode) CaptureInternal = true;
		}
		protected virtual void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
		}
		protected virtual void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
		}
		protected virtual void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
		}
		#region IXtraAnimationListener Members
		void IXtraAnimationListener.OnAnimation(BaseAnimationInfo info) {
			if(info.AnimationId != RadialMenuWindow.BoundAnimationId) {
				Invalidate();
			}
		}
		void IXtraAnimationListener.OnEndAnimation(BaseAnimationInfo info) {
			if(info.AnimationId == RadialMenuViewInfo.ContentChangeAnimationId) {
				ViewInfo.IsReady = false;
				Invalidate();
			}
		}
		private bool IsLinkAnimation(BaseAnimationInfo info, object animationId) {
			foreach(BarItemLink link in Menu.ItemLinks) {
				LinkAnimationId linkId = new LinkAnimationId(link, animationId);
				if(info.AnimationId.Equals(linkId))
					return true;
			}
			return false;
		}
		#endregion
		bool isMouseEnterRaised;
		protected internal virtual void UpdateHoverInfo(Point point) {
			if(Menu.IsTransparentBackground) return;
			if(ShouldRaiseOnMouseLeave(point)) OnMouseLeave();
			if(ShouldRaiseOnMouseEnter(point)) OnMouseEnter();
		}
		protected virtual bool ShouldRaiseOnMouseEnter(Point pt) {
			if(isMouseEnterRaised) return false;
			if(Menu.State == RadialMenuState.Expanded) return Menu.IsPointInEllipse(pt);
			return Menu.IsPointInGlyphEllipse(pt);
		}
		protected virtual bool ShouldRaiseOnMouseLeave(Point pt) {
			if(!isMouseEnterRaised) return false;
			if(Menu.State == RadialMenuState.Expanded) return !Menu.IsPointInEllipse(pt);
			return !Menu.IsPointInGlyphEllipse(pt);
		}
		protected internal virtual void OnActualLinksHolderChanged(BarLinksHolder prev, BarLinksHolder next) {
			ViewInfo.OnActualLinksHolderChanged(prev, next);
		}
		protected internal void ForceUpdateViewInfo() {
			UpdateViewInfo();
			CheckViewInfo(null);
		}
		protected internal virtual void OnMenuRadiusChanged() {
			if(!Visible)
				return;
			Point centerPoint = new Point(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
			UpdateViewInfo();
			Size = ViewInfo.CalcMaximumSize(null);
			Location = CalcLocation(centerPoint);
			Size bestSize = ViewInfo.CalcBestSize(Menu.ItemLinks);
			Point newLocation = new Point((Size.Width - bestSize.Width) / 2, (Size.Height - bestSize.Height) / 2);
			ClientBounds = new Rectangle(newLocation, bestSize);
			Show(Location);
			ForceUpdateViewInfo();
		}
		protected virtual bool CanDraw {
			get {
				if(Menu != null && Menu.Manager == null)
					return false;
				return true;
			}
		}
		protected internal virtual void ClearLinks() {
			ViewInfo.ClearLinks();
			UpdateViewInfo();
		}
		#region Drawing
		protected override void DrawCore(GraphicsCache cache) {
			if(!CanDraw)
				return;
			using(Image img = new Bitmap(Bounds.Width, Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				using(Graphics g = Graphics.FromImage(img)) {
					CheckViewInfo(g);
					using(GraphicsCache gCache = new GraphicsCache(g))
						Painter.Draw(new RadialMenuGraphicsInfoArgs(gCache, ClientBounds, ViewInfo));
				}
				cache.Graphics.DrawImageUnscaled(img, Point.Empty);
			}
		}
		#endregion
	}
}
