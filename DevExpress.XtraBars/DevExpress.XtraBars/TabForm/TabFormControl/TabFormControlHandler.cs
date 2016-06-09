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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabFormControlHandler {
		TabFormAnimator animator;
		TabFormControlBase owner;
		TabFormHitInfoAssistant hitInfoAssistant;
		Point startDragLocation;
		bool isDragging;
		public TabFormControlHandler(TabFormControlBase owner) {
			this.owner = owner;
			this.startDragLocation = Point.Empty;
			this.isDragging = false;
			this.animator = new TabFormAnimator(owner);
			this.hitInfoAssistant = new TabFormHitInfoAssistant();
		}
		TabFormControlBase Owner { get { return owner; } }
		TabForm TabForm {
			get {
				TabFormControl ctrl = Owner as TabFormControl;
				if(ctrl == null) return null;
				return ctrl.TabForm;
			}
		}
		internal TabFormPageViewInfo MovingPageInfo { get; set; }
		TabFormPageViewInfo hotPage;
		internal TabFormPageViewInfo HotPage {
			get { return hotPage; }
			set {
				if(hotPage == value)
					return;
				TabFormPageViewInfo prev = hotPage;
				hotPage = value;
				OnHotPageChanged(prev, hotPage);
				Owner.Invalidate();
			}
		}
		void OnHotPageChanged(TabFormPageViewInfo prev, TabFormPageViewInfo next) {
			if(prev != null) Owner.ViewInfo.UpdatePagePaintAppearance(prev);
			if(next != null) Owner.ViewInfo.UpdatePagePaintAppearance(next);
		}
		protected internal void UpdateSelectedPage(TabFormPage prevSelectedPage) {
			int index = Owner.Pages.IndexOf(prevSelectedPage);
			int newIndex = index - 1;
			while(newIndex > -1) {
				if(Owner.Pages[newIndex].CanSelect()) {
					Owner.SelectedPage = Owner.Pages[newIndex];
					return;
				}
				newIndex--;
			}
			newIndex = index + 1;
			while(newIndex < Owner.Pages.Count) {
				if(Owner.Pages[newIndex].CanSelect()) {
					Owner.SelectedPage = Owner.Pages[newIndex];
					return;
				}
				newIndex++;
			}
			Owner.SelectedPage = null;
		}
		public void OnMouseUp(MouseEventArgs e) {
			this.isDragging = false;
			if(DragController.IsDragging) DragController.EndDrag(true);
			if(MovingPageInfo != null) {
				Owner.Capture = false;
				if(ShouldDisposePage(e)) {
					DisposePage();
					return;
				}
				TabForm selForm = GetFormByCursorPosition();
				if(TabForm != selForm && selForm != null) {
					MovePageInAnotherForm(selForm);
				}
				else {
					if(!MovingRectangle.Contains(e.Location)) {
						if(AllowDragDrop) MovePageInNewForm();
						else Owner.Invalidate();
					}
					else {
						MovePageWithinForm(e, MovingPageInfo);
					}
					MovingPageInfo = null;
				}
			}
			CheckCloseButtonState(e, HotPage, HotPage);
			Animator.ResetAnimationInfo();
			Owner.LayoutChanged();
		}
		protected void DisposePage() {
			TabFormPage page = Owner.SelectedPage;
			UpdateSelectedPage(page);
			page.Dispose();
			Owner.Pages.Remove(page);
			MovingPageInfo = null;
			Animator.ResetAnimationInfo();
			Owner.LayoutChanged();
		}
		protected bool ShouldDisposePage(MouseEventArgs e) {
			if(Owner.SelectedPage == null) return false;
			TabFormPageViewInfo info = Owner.ViewInfo.GetPageInfo(Owner.SelectedPage);
			return info != null && info.Page.ShouldShowCloseButton() && info.CloseButtonState == ObjectState.Pressed && info.GetCloseButtonBounds().Contains(e.Location);
		}
		bool AllowDragDrop {
			get { return !Owner.IsDesignMode && TabForm != null && TabForm.TopLevel && Owner.AllowMoveTabsToOuterForm && Owner.AllowMoveTabs; }
		}
		public bool OnMouseDown(MouseEventArgs e) {
			this.startDragLocation = e.Location;
			for(int i = 0; i < Owner.ViewInfo.PageInfos.Count; i++) {
				TabFormPageViewInfo pageInfo = Owner.ViewInfo.PageInfos[i];
				if(!pageInfo.Page.GetEnabled()) continue;
				if(HitInfoAssistant.IsInPage(pageInfo, e.Location)) {
					Owner.SelectedPage = pageInfo.Page;
					MovingPageInfo = Owner.ViewInfo.GetPageInfo(Owner.SelectedPage);
					CheckCloseButtonState(e, HotPage, MovingPageInfo);
					Owner.Capture = true;
					if(Owner.IsDesignMode && !Owner.DesignManager.IsComponentSelected(pageInfo.Page)) {
						Owner.DesignManager.SelectComponent(pageInfo.Page);
					}
					return true;
				}
			}
			if(Owner.ViewInfo.ShouldShowAddPage() && Owner.ViewInfo.AddPageInfo.Bounds.Contains(e.Location)) {
				Owner.AddNewPage();
				Owner.LayoutChanged();
				return true;
			}
			return false;
		}
		static readonly Point InvalidMousePoint = new Point(-10000, -10000);
		bool ShouldStartDragging(Point current) {
			if(current.Equals(InvalidMousePoint) || !Owner.AllowMoveTabs)
				return false;
			return Math.Abs(current.X - this.startDragLocation.X) > 7 || Math.Abs(current.Y - this.startDragLocation.Y) > 7;
		}
		public void OnMouseMove(MouseEventArgs e) {
			TabFormPageViewInfo prevHotPage = HotPage;
			HotPage = GetHotPage(e.Location);
			CheckCloseButtonState(e, prevHotPage, HotPage);
			TabFormPageViewInfo pageInfo = MovingPageInfo;
			if(pageInfo != null) {
				if(!isDragging) {
					if(!ShouldStartDragging(e.Location)) return;
					isDragging = true;
				}
				Animator.SetAnimationInfo(pageInfo.CurrentBounds);
				if(MovingRectangle.Contains(e.Location)) {
					UpdatePageBounds(e);
					if(DragController.IsDragging) DragController.EndDrag(false);
				}
				else if(AllowDragDrop) {
					if(DragController.IsDragging)
						DragController.OnDragging(Owner.PointToScreen(e.Location));
					else DragController.OnStartDragging(MovingPageInfo);
				}
				Animator.RunAnimation();
			}
		}
		protected void CheckCloseButtonState(MouseEventArgs e, TabFormPageViewInfo prevHotPage, TabFormPageViewInfo newHotPage) {
			if(!object.Equals(prevHotPage, HotPage) && prevHotPage != null)
				prevHotPage.CloseButtonState = ObjectState.Normal;
			if(newHotPage != null) {
				if(newHotPage.CloseButtonState == ObjectState.Pressed && e.Button == MouseButtons.Left) return;
				if(newHotPage.GetCloseButtonBounds().Contains(e.Location)) {
					if(e.Button == MouseButtons.Left)
						newHotPage.CloseButtonState = ObjectState.Pressed;
					else newHotPage.CloseButtonState = ObjectState.Hot;
				}
				else newHotPage.CloseButtonState = ObjectState.Normal;
			}
		}
		public void OnMouseLeave() {
			if(HotPage != null && HotPage.CloseButtonState == ObjectState.Hot)
				HotPage.CloseButtonState = ObjectState.Normal;
			HotPage = null;
		}
		TabFormDragController dragController;
		TabFormDragController DragController {
			get {
				if(dragController == null) {
					dragController = new TabFormDragController(Owner);
				}
				return dragController;
			}
		}
		TabFormPageViewInfo GetHotPage(Point location) {
			if(MovingPageInfo == null) {
				foreach(TabFormPageViewInfo pageInfo in Owner.ViewInfo.PageInfos) {
					if(!pageInfo.Page.GetEnabled()) continue;
					if(HitInfoAssistant.IsInPage(pageInfo, location)) {
						return pageInfo;
					}
				}
				if(Owner.ViewInfo.ShouldShowAddPage() && Owner.ViewInfo.AddPageInfo.Bounds.Contains(location)) {
					return Owner.ViewInfo.AddPageInfo;
				}
			}
			return null;
		}
		Rectangle MovingRectangle { get { return new Rectangle(Point.Empty, Owner.Size); } }
		internal TabFormAnimator Animator { get { return animator; } }
		protected void MovePageInAnotherForm(TabForm selForm) {
			TabFormPage page = MovingPageInfo.Page;
			UpdateSelectedPage(page);
			MovingPageInfo = null;
			Owner.Pages.Remove(page);
			selForm.TabFormControl.Pages.Add(page);
			selForm.TabFormControl.SelectedPage = page;
			if(Owner.Pages.Count == 0)
				StartFormDisposing();
			selForm.Activate();
		}
		protected void MovePageWithinForm(MouseEventArgs e, TabFormPageViewInfo pageInfo) {
			int newIndex = GetPositionByLocation(pageInfo);
			int oldIndex = Owner.Pages.IndexOf(pageInfo.Page);
			if(newIndex == oldIndex) return;
			Owner.Pages.Move(newIndex, pageInfo.Page);
		}
		protected internal void MovePageInNewForm() {
			UpdateSelectedPage(MovingPageInfo.Page);
			MovePageInNewForm(MovingPageInfo.Page, Cursor.Position);
		}
		protected internal void MovePageInNewForm(TabFormPage page, Point location) {
			Owner.Pages.Remove(page);
			TabForm newForm = CreateNewTabForm();
			newForm.TabFormControl.Pages.Add(page);
			newForm.TabFormControl.SelectedPage = page;
			newForm.StartPosition = FormStartPosition.Manual;
			newForm.Location = location;
			Owner.RaiseOuterFormCreated(new OuterFormCreatedEventArgs(newForm));
			newForm.Show();
			if(Owner.Pages.Count == 0) StartFormDisposing();
		}
		protected XtraBars.TabForm CreateNewTabForm() {
			OuterFormCreatingEventArgs args = new OuterFormCreatingEventArgs();
			Owner.RaiseOuterFormCreating(args);
			if(args.Form != null)
				return args.Form;
			TabForm newForm = new TabForm();
			newForm.CreateTabFormControl(false);
			return newForm;
		}
		int GetPositionByLocation(TabFormPageViewInfo current) {
			bool moveRight = false;
			for(int i = 0; i < Owner.ViewInfo.PageInfos.Count; i++) {
				int index = Owner.IsRightToLeft ? Owner.ViewInfo.PageInfos.Count - 1 - i : i;
				TabFormPageViewInfo pageInfo = Owner.ViewInfo.PageInfos[index];
				if(pageInfo.Equals(current)) {
					moveRight = true;
					continue;
				}
				if(current.CurrentBounds.Left < pageInfo.CurrentBounds.Right + Owner.ViewInfo.GetDistanceBetweenTabs() - current.CurrentBounds.Width / 2) {
					int pageIndex = pageInfo.Page.GetIndex();
					if(!moveRight) return pageIndex;
					if(Owner.IsRightToLeft)
						return pageIndex + 1;
					return pageIndex - 1;
				}
			}
			if(Owner.IsRightToLeft)
				return 0;
			return Owner.ViewInfo.PageInfos.Count - 1;
		}
		internal void StartFormDisposing() {
			NativeMethods.PostMessage(TabForm.Handle, 0x10, IntPtr.Zero, IntPtr.Zero);
		}
		protected TabForm GetFormByCursorPosition() {
			Point location = Cursor.Position;
			int z = int.MaxValue;
			TabForm res = null;
			foreach(Form form in Application.OpenForms) {
				if(!form.Visible) continue;
				if(form is TabForm && form.IsHandleCreated && form.Bounds.Contains(location)) {
					int formZ = GetFormZOrder(form);
					if(formZ < z) {
						z = formZ;
						res = form as TabForm;
					}
				}
			}
			return res;
		}
		protected int GetFormZOrder(Form form) {
			IntPtr handle = form.Handle;
			int z = 0;
			do {
				z++;
				handle = NativeMethods.GetWindow(handle, NativeMethods.GW_HWNDPREV);
			} while(handle != IntPtr.Zero);
			return z;
		}
		protected void UpdatePageBounds(MouseEventArgs e) {
			MovingPageInfo.UpdateCurrentLeft(e.X - MovingPageInfo.CurrentBounds.Width / 2);
			if(!Owner.IsInAnimation) Owner.Invalidate();
		}
		TabFormHitInfoAssistant HitInfoAssistant { get { return hitInfoAssistant; } }
		protected internal void UpdateHitInfoPattern() {
			if(Owner.ViewInfo == null) return;
			HitInfoAssistant.UpdatePagePattern(Owner.ViewInfo.GetPageSkinElement(), Owner.LookAndFeel.ActiveLookAndFeel, Owner.ViewInfo.CalcBestPageHeight());
		}
	}
	public class TabFormHitInfoAssistant {
		public bool IsInPage(TabFormPageViewInfo pageInfo, Point p){
			if(pageInfo.CurrentBounds.Contains(p)){
				Point pagePoint = new Point(p.X - pageInfo.CurrentBounds.X, p.Y - pageInfo.CurrentBounds.Y);
				if(pagePoint.X < leftPartWidth - 1){
					return leftPartPath.IsVisible(pagePoint);
				}
				if(pagePoint.X <= pageInfo.CurrentBounds.Width - rightPartWidth){
					return true;
				}
				int middlePartWidth = pageInfo.CurrentBounds.Width - rightPartWidth - leftPartWidth;
				pagePoint.X -= middlePartWidth;
				return rightPartPath.IsVisible(pagePoint);
			}
			return false;
		}
		public void UpdatePagePattern(SkinElement elem, UserLookAndFeel lookAndFeel, int pageHeight){
			if(elem == null || elem.Image == null) return;
			this.leftPartWidth = elem.Image.SizingMargins.Left;
			this.rightPartWidth = elem.Image.SizingMargins.Right;
			Bitmap patternBitmap = CreatePatternBitmap(pageHeight, elem);
			this.leftPartPath = CreateLeftPartPath(patternBitmap, elem.Image.SizingMargins.Left);
			this.rightPartPath = CreateRightPartPath(patternBitmap, elem.Image.SizingMargins.Right);
		}
		int leftPartWidth;
		int rightPartWidth;
		GraphicsPath leftPartPath;
		GraphicsPath rightPartPath;
		static GraphicsPath CreateLeftPartPath(Bitmap patternBitmap, int width) {
			List<Point> points = new List<Point>();
			for(int i = 0; i < patternBitmap.Height; i++) {
				for(int j = 0; j < width; j++) {
					if(patternBitmap.GetPixel(j, i).A > 0) {
						points.Add(new Point(j, i));
						break;
					}
					if(j == width - 1) {
						points.Add(new Point(j, i));
					}
				}
			}
			points.Add(new Point(width - 1, patternBitmap.Height - 1));
			points.Add(new Point(width - 1, 0));
			GraphicsPath path = new GraphicsPath();
			path.AddPolygon(points.ToArray());
			return path;
		}
		static GraphicsPath CreateRightPartPath(Bitmap patternBitmap, int width) {
			List<Point> points = new List<Point>();
			for(int i = 0; i < patternBitmap.Height; i++) {
				for(int j = patternBitmap.Width - 1; j > patternBitmap.Width - width - 1; j--) {
					if(patternBitmap.GetPixel(j, i).A > 0) {
						points.Add(new Point(j, i));
						break;
					}
					if(j == patternBitmap.Width - width) {
						points.Add(new Point(j, i));
					}
				}
			}
			points.Add(new Point(patternBitmap.Width - width, patternBitmap.Height - 1));
			points.Add(new Point(patternBitmap.Width - width, 0));
			GraphicsPath path = new GraphicsPath();
			path.AddPolygon(points.ToArray());
			return path;
		}
		static Bitmap CreatePatternBitmap(int pageHeight, SkinElement elem) {
			Bitmap patternBitmap = new Bitmap(elem.Image.SizingMargins.Width, pageHeight);
			using(Graphics g = Graphics.FromImage(patternBitmap)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					g.Clear(Color.Transparent);
					SkinElementInfo info = new SkinElementInfo(elem, new Rectangle(Point.Empty, patternBitmap.Size));
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
				}
			}
			return patternBitmap;
		}
	}
}
