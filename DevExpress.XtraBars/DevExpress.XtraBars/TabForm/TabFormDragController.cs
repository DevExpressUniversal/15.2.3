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

using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Paint;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class DragMasterBase {
		[ThreadStatic]
		static DragLayeredWindowBase window = null;
		bool dragInProgress;
		Control parent;
		public DragMasterBase(Control parent) {
			this.dragInProgress = false;
			this.parent = parent;
		}
		internal DragLayeredWindowBase DragWindow {
			get {
				if(window == null) window = CreateDragWindow();
				return window;
			}
		}
		internal Control Parent { get { return parent; } }
		protected virtual DragLayeredWindowBase CreateDragWindow() {
			return new DragLayeredWindowBase(parent);
		}
		public void StartDrag(Bitmap bmp, Point startPoint) {
			StopDrag();
			dragInProgress = true;
			DragWindow.ShowDrag(startPoint, bmp);
			window.Image = bmp;
		}
		public virtual void StopDrag() {
			StopDragCore();
		}
		protected void StopDragCore() {
			this.dragInProgress = false;
			DragWindow.HideDrag();
		}
		public void DoDrag(Point p) {
			if(!dragInProgress) return;
			DragWindow.MoveDrag(p);
		}
		public void CancelDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
		public void EndDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
	}
	public class DragLayeredWindowBase : DXLayeredWindowEx {
		bool dragging;
		Point hotSpot;
		Control parent;
		Bitmap image;
		public DragLayeredWindowBase(Control parent)
			: base() {
			this.parent = parent;
			this.image = null;
			Alpha = (byte)(DevExpress.Utils.DragDrop.DragWindow.DefaultOpacity * 255);
		}
		public Bitmap Image { get { return image; } set { image = value; } }
		protected override void DrawCore(GraphicsCache cache) {
			cache.Graphics.Clear(Color.Transparent);
			if(Image != null) cache.Graphics.DrawImage(Image, Point.Empty);
		}
		protected override void OnDisposing() {
			this.image = null;
			base.OnDisposing();
		}
		public void ShowDrag(Point p, Bitmap bitmap) {
			Image = bitmap;
			if(bitmap == null) {
				HideDrag();
				return;
			}
			else {
				Size = bitmap.Size;
				hotSpot = new Point(bitmap.Size.Width / 2, bitmap.Size.Height / 2);
			}
			dragging = true;
			Create(ParentHandle);
			InternalMoveBitmap(p);
			Update();
		}
		protected IntPtr ParentHandle {
			get {
				if(parent != null && parent.IsHandleCreated)
					return parent.Handle;
				return IntPtr.Zero;
			}
		}
		protected void InternalMoveBitmap(Point p) {
			p.Offset(-hotSpot.X, -hotSpot.Y);
			Show(p);
		}
		public bool MoveDrag(Point p) {
			if(!dragging) return false;
			InternalMoveBitmap(p);
			return true;
		}
		public bool HideDrag() {
			if(!dragging) return false;
			Hide();
			Image = null;
			dragging = false;
			return true;
		}
		public Point HotSpot {
			get { return hotSpot; }
			set {
				hotSpot = value;
			}
		}
	}
	public class TabFormDragWindow : DragLayeredWindowBase, ISupportXtraAnimationEx {
		Bitmap formImage, dragImage;
		public TabFormDragWindow(Control parent)
			: base(parent) {
			this.formImage = null;
			this.dragImage = null;
		}
		public Bitmap FormImage { get { return formImage; } set { formImage = value; } }
		public Bitmap DragImage { get { return dragImage; } set { dragImage = value; } }
		protected override void DrawCore(GraphicsCache cache) {
			cache.Graphics.Clear(Color.Transparent);
			if(DragImage != null) cache.Graphics.DrawImage(DragImage, Point.Empty);
		}
		protected override void OnDisposing() {
			this.formImage = null;
			base.OnDisposing();
		}
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Control ISupportXtraAnimation.OwnerControl { get { return null; } }
		void ISupportXtraAnimationEx.OnEndAnimation(BaseAnimationInfo info) {
			isInAnimation = false;
			RunThumbnailAnimation();
			if(!isInAnimation && !formThumbShown) HideDrag();
		}
		void ISupportXtraAnimationEx.OnFrameStep(BaseAnimationInfo info) {
			if(Image == null || FormImage == null) return;
			double value = ((DoubleSplineAnimationInfo)info).Value;
			Size newSize = new Size((int)(Image.Size.Width + (FormImage.Size.Width - Image.Size.Width) * value), (int)(Image.Size.Height + (FormImage.Size.Height - Image.Size.Height) * value));
			DragImage = new Bitmap(newSize.Width, newSize.Height);
			using(Graphics g = Graphics.FromImage(DragImage)) {
				g.DrawImage(FormImage, new Rectangle(Point.Empty, newSize), 0, 0, FormImage.Width, FormImage.Height, GraphicsUnit.Pixel, XPaint.CreateImageAttributesWithOpacity((float)value));
			}
			Size = newSize;
			Invalidate();
		}
		void RunThumbnailHiding() {
			formThumbShown = false;
			isInAnimation = true;
			XtraAnimator.Current.AddAnimation(new DoubleSplineAnimationInfo(this, this, 1, 0, 200));
		}
		void RunFormThumbnailShowing() {
			formThumbShown = true;
			isInAnimation = true;
			XtraAnimator.Current.AddAnimation(new DoubleSplineAnimationInfo(this, this, 0, 1, 200));
		}
		bool isInAnimation = false;
		bool shouldShowFormThumb = false;
		bool formThumbShown = false;
		public void RunThumbnailAnimation() {
			if(!ShouldStartAnimation())
				return;
			isInAnimation = true;
			if(shouldShowFormThumb) RunFormThumbnailShowing();
			else RunThumbnailHiding();
		}
		bool ShouldStartAnimation() {
			if(isInAnimation) return false;
			return (shouldShowFormThumb && !formThumbShown) || (!shouldShowFormThumb && formThumbShown);
		}
		internal void AddThumbnailHidingAnimation() {
			shouldShowFormThumb = false;
			if(!isInAnimation) RunThumbnailAnimation();
		}
		internal void AddThumbnailShowingAnimation() {
			shouldShowFormThumb = true;
			if(!isInAnimation) RunThumbnailAnimation();
		}
		public void ResetAnimationInfo() {
			isInAnimation = shouldShowFormThumb = formThumbShown = false;
		}
	}
	public class TabFormDragMaster : DragMasterBase {
		public TabFormDragMaster(Control parent) : base(parent) { }
		public void StartDrag(Bitmap pageBitmap, Bitmap formBitmap, Point startPoint) {
			StartDrag(pageBitmap, startPoint);
			TabFormDragWindow.DragImage = pageBitmap;
			TabFormDragWindow.FormImage = formBitmap;
			AddThumbnailShowingAnimation();
		}
		public override void StopDrag() {
			AddThumbnailHidingAnimation();
		}
		void AddThumbnailHidingAnimation() {
			TabFormDragWindow.AddThumbnailHidingAnimation();
		}
		void AddThumbnailShowingAnimation() {
			TabFormDragWindow.AddThumbnailShowingAnimation();
		}
		TabFormDragWindow TabFormDragWindow { get { return (TabFormDragWindow)DragWindow; } }
		protected override DragLayeredWindowBase CreateDragWindow() {
			return new TabFormDragWindow(Parent);
		}
		public void EndDrag(bool immedeately) {
			if(immedeately) {
				StopDragCore();
				TabFormDragWindow.ResetAnimationInfo();
			}
			else EndDrag();
		}
	}
	public class TabFormDragController {
		TabFormControlBase control;
		TabFormDragMaster dragMaster;
		bool isDragging;
		public TabFormDragController(TabFormControlBase control) {
			Reset();
			this.isDragging = false;
			this.control = control;
			this.dragMaster = new TabFormDragMaster(control);
		}
		public TabFormControlBase Control {
			get { return control; }
		}
		TabFormDragMaster DragMaster {
			get { return dragMaster; }
		}
		public virtual void OnStartDragging(TabFormPageViewInfo pageInfo) {
			PageInfo = pageInfo;
			this.isDragging = true;
			DragMaster.StartDrag(CreatePageBitmap(), CreateFormBitmap(), Cursor.Position);
		}
		public Bitmap CreatePageBitmap() {
			TabFormControlPainter pagePainter = Control.ViewInfo.Painter as TabFormControlPainter;
			if(PageInfo == null || pagePainter == null) return null;
			Bitmap bmp = Navigation.ElementBitmapPainter.Draw(cache => { CreatePageBitmapCore(cache, pagePainter); }, PageInfo.Bounds.Size, PageInfo.CurrentBounds.Location);
			return bmp;
		}
		protected void CreatePageBitmapCore(GraphicsCache cache, TabFormControlPainter pagePainter) {
			pagePainter.DrawPage(cache, Control, PageInfo);
		}
		public Bitmap CreateFormBitmap() {
			TabForm form = ((TabFormControl)Control).TabForm;
			Bitmap formBitmap = new Bitmap(form.Width, form.Height);
			form.DrawToBitmap(formBitmap, new Rectangle(0, 0, form.Width, form.Height));
			int height = 150;
			int width = height * form.Width / form.Height;
			Bitmap bmp = new Bitmap(width, height);
			using(Graphics g = Graphics.FromImage(bmp)) {
				g.DrawImage(formBitmap, new Rectangle(Point.Empty, bmp.Size));
			}
			return bmp;
		}
		public virtual void OnDragging(Point p) {
			DragMaster.DoDrag(p);
		}
		protected internal TabFormPageViewInfo PageInfo { get; set; }
		public virtual void EndDrag(bool immedeately) {
			DragMaster.EndDrag(immedeately);
			this.isDragging = false;
		}
		public bool IsDragging { get { return isDragging; } }
		public void Reset() {
			PageInfo = null;
		}
	}
}
