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
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Painters;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RibbonStatusBarInfoArgs : ObjectInfoArgs {
		RibbonStatusBarViewInfo viewInfo;
		public RibbonStatusBarInfoArgs(RibbonStatusBarViewInfo viewInfo, GraphicsCache cache)
			: base(cache) {
			this.viewInfo = viewInfo;
		}
		public RibbonStatusBarInfoArgs(RibbonStatusBarViewInfo viewInfo, GraphicsCache cache, Rectangle bounds, ObjectState state)
			: base(cache, bounds, state) {
			this.viewInfo = viewInfo;
		}
		public RibbonStatusBarInfoArgs(RibbonStatusBarViewInfo viewInfo)
			: base() {
			this.viewInfo = viewInfo;
		}
		public RibbonStatusBarViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class RibbonStatusBarPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonStatusBarInfoArgs se = e as RibbonStatusBarInfoArgs;
			if(se == null) return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, se.ViewInfo.GetStatusBarDrawInfo());
			SkinElementInfo rInfo = se.ViewInfo.GetRightStatusBarDrawInfo();
			if(!rInfo.Bounds.IsEmpty)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, rInfo);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, se.ViewInfo.GetSizeGripInfo());
			foreach(RibbonItemViewInfo itemInfo in se.ViewInfo.Items) {
				RibbonItemViewInfoCalculator.DrawItem(e.Cache, itemInfo);
			}
			DrawDesignerRects(se);
		}
		void DrawDesignerRect(GraphicsCache cache, Rectangle rect) {
			Pen pen = new Pen(Color.Red);
			pen.DashPattern = new float[] { 5.0f, 5.0f };
			cache.DrawRectangle(pen, rect);
		}
		protected virtual void DrawDesignerRects(RibbonStatusBarInfoArgs e) {
			if(!e.ViewInfo.StatusBar.IsDesignMode) return;
			DrawDesignerRect(e.Cache, e.ViewInfo.DesignerLeftRect);
			DrawDesignerRect(e.Cache, e.ViewInfo.DesignerRightRect);
		}
	}
	public class RibbonStatusBarViewInfo : BaseRibbonViewInfo {
		RibbonItemViewInfoCollection items;
		RibbonStatusBar statusBar;
		Rectangle sizeGripBounds;
		Rectangle designerLeftRect, designerRightRect;
		public RibbonStatusBarViewInfo(RibbonStatusBar statusBar) {
			this.items = new RibbonItemViewInfoCollection(this);
			this.statusBar = statusBar;
			this.sizeGripBounds = Rectangle.Empty;
			this.designerLeftRect = Rectangle.Empty;
			this.designerRightRect = Rectangle.Empty;
		}
		protected override RibbonItemViewInfo FindItem(IRibbonItem item, Rectangle bounds) { return Items.Find(item, bounds); }
		protected internal override void OnItemChanged(RibbonItemViewInfo item) {
			StatusBar.Refresh();
		}
		protected override BaseHandler Handler { get { return StatusBar.Handler; } }
		protected override int ItemAnimationLength {
			get { return StatusBar.Ribbon.ItemAnimationLength; }
		}
		public override RibbonAppearances PaintAppearance {
			get {
				if(StatusBar.Ribbon == null) return new RibbonAppearances(null);
				return StatusBar.Ribbon.ViewInfo.PaintAppearance;
			}
		}
		public RibbonForm Form {
			get {
				RibbonForm form = (StatusBar != null ? StatusBar.FindForm() : null) as RibbonForm;
				return form != null && form.StatusBar == StatusBar ? form : null;
			}
		}
		public virtual bool IsAllowRibbonFormBehavior {
			get { return Form != null && StatusBar.Parent == Form && !Form.IsGlassForm && Form.WindowState == FormWindowState.Normal && Form.FormBorderStyle != FormBorderStyle.None; } 
		}
		internal override bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(StatusBar); } }
		public override bool IsDesignMode { get { return StatusBar != null && StatusBar.IsDesignMode; } }
		protected internal override BaseRibbonDesignTimeManager CreateDesignTimeManager() { return new RibbonStatusBarDesignTimeManager(StatusBar); }
		public override ISkinProvider Provider {
			get { return StatusBar.Ribbon == null ? UserLookAndFeel.Default : StatusBar.Ribbon.ViewInfo.Provider; } 
		}
		public override void Invalidate(RibbonItemViewInfo itemInfo) {
			Invalidate(itemInfo.Bounds);
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.None) return;
			if(hitInfo.InItem) {
				Invalidate(hitInfo.ItemInfo);
			}
		}
		protected override RibbonItemViewInfoCalculator CreateItemCalculator() { return new RibbonItemStatusBarViewInfoCalculator(this); }
		protected internal virtual int DefaultItemIndent {
			get {
				return RibbonSkins.GetSkin(Provider).Properties.GetInteger(RibbonSkins.OptIndentBetweenStatusBarColumns);
			}
		}
		public override Control OwnerControl { get { return StatusBar; } }
		public override RibbonBarManager Manager { get { return StatusBar.Manager; } }
		public RibbonStatusBar StatusBar { get { return statusBar; } }
		public RibbonItemViewInfoCollection Items { get { return items; } }
		public Rectangle SizeGripBounds { get { return sizeGripBounds; } }
		public SkinElementInfo GetSizeGripInfo() {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinSizeGrip], SizeGripBounds);
			if(!StatusBar.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			info.RightToLeft = IsRightToLeft;
			return info;
		}
		public Rectangle DesignerLeftRect { get { return designerLeftRect; } }
		public Rectangle DesignerRightRect { get { return designerRightRect; } }
		protected virtual void CalcDesignerRects() {
			if(!StatusBar.IsDesignMode) return;
			this.designerLeftRect = GetLeftPartBounds();
			this.designerRightRect = GetRightPartBounds();
			if(this.designerLeftRect == Rectangle.Empty) this.designerLeftRect = new Rectangle(0, Bounds.Y, 50, Bounds.Height);
			if(this.designerRightRect.Width == 0) this.designerRightRect = new Rectangle(Bounds.Right - 50, Bounds.Y, 50, Bounds.Height);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			if(StatusBar.Ribbon == null) return;
			Clear();
			GInfo.AddGraphics(null);
			try {
				CalcConstants();
				Bounds = bounds;
				ContentBounds = CalcContentBounds();
				this.sizeGripBounds = CalcSizeGripBounds();
				CreateItemsViewInfo();
				int bestWidth = CalcBestWidth();
				if(ContentBounds.Width < bestWidth) Reduce(bestWidth);
				LayoutItems();
				CalcDesignerRects();
				UpdateItemHotInfo(HotObject);
				RemoveInvisibleAnimatedItems();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			CheckStatusBarRTL();
			IsReady = true;
		}
		#region RightToLeft
		protected virtual void CheckStatusBarRTL() {
			if(!IsRightToLeft) return;
			CheckDesignerRectsRTL();
			CheckItemsRTL();
			this.sizeGripBounds = BarUtilites.ConvertBoundsToRTL(this.sizeGripBounds, Bounds);
		}
		protected virtual void CheckItemsRTL() {
			if(!IsRightToLeft) return;
			foreach(RibbonItemViewInfo item in Items) {
				item.Bounds = BarUtilites.ConvertBoundsToRTL(item.Bounds, Bounds);
			}
		}
		protected virtual void CheckDesignerRectsRTL() {
			if(!IsRightToLeft) return;
			this.designerLeftRect = BarUtilites.ConvertBoundsToRTL(this.designerLeftRect, Bounds);
			this.designerRightRect = BarUtilites.ConvertBoundsToRTL(this.designerRightRect, Bounds);
		}
		#endregion
		protected virtual void RemoveInvisibleAnimatedItems() {
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				EditorAnimationInfo info = XtraAnimator.Current.Animations[i] as EditorAnimationInfo;
				if(info == null || info.AnimatedObject != StatusBar) continue;
				if(!HasAnimatedItem(info))
					XtraAnimator.RemoveObject(StatusBar, XtraAnimator.Current.Animations[i].AnimationId);
			}
		}
		protected virtual bool HasAnimatedItem(EditorAnimationInfo info) {
			if(HasAnimatedItem(info, Items)) return true;
			return false;
		}
		public override BarItemLink GetLinkByPoint(Point pt, bool enterButtonGroup) {
			if(Items == null) return null;
			foreach(RibbonItemViewInfo item in Items) {
				RibbonButtonGroupItemViewInfo buttonInfo = item as RibbonButtonGroupItemViewInfo;
				if(buttonInfo != null && enterButtonGroup) return buttonInfo.GetLinkByPoint(pt);
				if(item.Bounds.Contains(pt)) return item.Item as BarItemLink;
			}
			return null;
		}
		protected virtual bool CanShowSizeGrip {
			get {
				if(!StatusBar.ShowSizeGrip) return false;
				Form form = StatusBar.Parent as Form;
				if(form == null || form.WindowState == FormWindowState.Maximized) return false;
				return form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow;
			}
		}
		protected internal virtual Size GetSizeGripSize() {
			if(!CanShowSizeGrip) return Size.Empty;
			SkinElementInfo info = GetSizeGripInfo();
			if(info != null && info.Element.Image != null && info.Element.Image.Image != null) return info.Element.Image.Image.Size;
			return Size.Empty;
		}
		protected internal virtual Point SizeGripIndent { 
			get {
				if(IsAllowRibbonFormBehavior) return new Point(1,1);
				return new Point(3, 3); 
			} 
		}
		protected virtual Rectangle CalcSizeGripBoundsInStandardFotm() {
			Size gripSize = GetSizeGripSize();
			if(gripSize.IsEmpty)
				return Rectangle.Empty;
			return new Rectangle(new Point(Bounds.Right - gripSize.Width - SizeGripIndent.X, Math.Max(Bounds.Top, Bounds.Bottom - gripSize.Height - SizeGripIndent.Y)), gripSize);
		}
		protected virtual Rectangle CalcSizeGripBoundsInRibbonForm() {
			Rectangle res = Rectangle.Empty;
			Size gripSize = GetSizeGripSize();
			if(gripSize.IsEmpty || ContentBounds.Width < gripSize.Width) return Rectangle.Empty;
			int delta = Form == null ? 0 : (Form.Width - Form.ClientRectangle.Width) / 2;
			return new Rectangle(Bounds.Right - gripSize.Width - SizeGripIndent.X, Math.Max(Bounds.Top, Bounds.Bottom - delta - gripSize.Height - SizeGripIndent.Y), gripSize.Width, gripSize.Height);
		}
		protected internal virtual Rectangle CalcSizeGripBounds() {
			if(IsAllowRibbonFormBehavior)
				return CalcSizeGripBoundsInRibbonForm();
			return CalcSizeGripBoundsInStandardFotm();
		}
		protected internal virtual void Reduce(int bestWidth) {
			int deltaWidth;
			for(; ContentBounds.Width < bestWidth; ) {
				deltaWidth = ReduceLeftItems();
				if(deltaWidth == 0) deltaWidth = ReduceRightItems();
				if(deltaWidth == 0) break;
				bestWidth -= deltaWidth;
			}
		}
		public virtual SkinElementInfo GetStatusBarInfo() {
			SkinElementInfo info = new SkinElementInfo(GetStatusBarElement(), Bounds);
			info.RightToLeft = IsRightToLeft;
			if(IsAllowRibbonFormBehavior && !Form.IsWindowActive) {
				if(info.Element.Image != null && info.Element.Image.ImageCount > 2)
					info.ImageIndex = 2;
			}
			if(!StatusBar.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetRightStatusBarInfo() {
			SkinElementInfo res = new SkinElementInfo(GetStatusBarElement(), Bounds);
			res.RightToLeft = IsRightToLeft;
			res.ImageIndex = 1;
			if(IsAllowRibbonFormBehavior && !Form.IsWindowActive) {
				if(res.Element.Image != null && res.Element.Image.ImageCount > 2)
					res.ImageIndex = 3;
			}
			if(!StatusBar.Enabled)
				res.Attributes = PaintHelper.RibbonDisabledAttributes;
			return res;
		}
		public virtual SkinElementInfo GetStatusBarDrawInfo() {
			SkinElementInfo info = GetStatusBarInfo();
			info.RightToLeft = IsRightToLeft;
			if(!IsAllowRibbonFormBehavior) return info;
			Rectangle bounds = info.Bounds;
			bounds.X -= GetFormFrameLeftWidth();
			bounds.Width += (GetFormFrameLeftWidth() + GetFormFrameRightWidth());
			info.Bounds = bounds;
			if(!StatusBar.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetRightStatusBarDrawInfo() {
			SkinElementInfo info = GetRightStatusBarInfo();
			info.RightToLeft = IsRightToLeft;
			info.Bounds = GetRightPartBounds();
			if(!IsAllowRibbonFormBehavior) return info;
			Rectangle bounds = info.Bounds;
			bounds.Width += (GetFormFrameRightWidth());
			info.Bounds = bounds;
			if(!StatusBar.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual SkinElement GetStatusBarElement() {
			if(IsAllowRibbonFormBehavior) return RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinStatusBarFormBackground];
			return RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinStatusBarBackground];
		}
		protected internal virtual int CalcRightContentIndent() {
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetRightStatusBarInfo(), new Rectangle(0, 0, 0, 0)).Width; 
		}
		protected internal override Rectangle CalcContentBounds() {
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetStatusBarInfo());
			rect.Width -= GetSizeGripSize().Width + 1;
			return rect;
		}
		protected int GetItemCount(bool leftItems) {
			int res = 0;
			for(int n = 0; n < Items.Count; n++) {
				if(IsRightAlign(Items[n].Item) != leftItems) res++;
			}
			return res;
		}
		protected internal int CalcBestWidth() {
			int bestWidth = 0;
			for(int itemIndex = Items.Count - 1; itemIndex >= 0; itemIndex--) {
				bestWidth += Items[itemIndex].CalcBestSize().Width;
			}
			bestWidth += (DefaultItemIndent * (Math.Max(GetItemCount(true) - 1, 0) + Math.Max(GetItemCount(false) - 1, 0)));
			bestWidth += CalcRightContentIndent();
			return bestWidth;
		}
		protected internal virtual int ReduceItems(bool leftItems) {
			int bestWidth = 0;
			for(int itemIndex = Items.Count - 1; itemIndex >= 0; itemIndex--) {
				if(IsRightAlign(Items[itemIndex].Item) == leftItems) continue;
				bestWidth = RemoveItem(itemIndex);
				if(GetItemCount(leftItems) > 0) bestWidth += DefaultItemIndent;
				break;
			}
			return bestWidth;
		}
		protected internal int ReduceLeftItems() { return ReduceItems(true);  }
		protected internal virtual int ReduceRightItems() { return ReduceItems(false); }
		protected internal Rectangle GetLeftPartBounds() {
			if(GetItemCount(true) == 0) return Rectangle.Empty;
			int endp = 0;
			for(int itemIndex = 0; itemIndex < Items.Count; itemIndex++) {
				if(IsRightAlign(Items[itemIndex].Item)) continue;
				endp = Math.Max(endp, Items[itemIndex].Bounds.Right);
			}
			return new Rectangle(0, Bounds.Y, endp, Bounds.Height);
		}
		protected internal Rectangle GetRightPartBounds() {
			if(GetItemCount(false) == 0) return Rectangle.Empty;
			int beginp = 0;
			for(int itemIndex = 0; itemIndex < Items.Count; itemIndex ++) { 
				if(!IsRightAlign(Items[itemIndex].Item)) continue;
				if(IsRightToLeft)
					beginp = Items[itemIndex].Bounds.Right + CalcRightContentIndent() / 2;
				else
					beginp = Items[itemIndex].Bounds.Left - CalcRightContentIndent() / 2;
				break;
			}
			if(IsRightToLeft)
				return new Rectangle(-GetFormFrameLeftWidth(), Bounds.Y, Bounds.X + beginp, Bounds.Height);
			return new Rectangle(beginp, Bounds.Y, Bounds.Right - beginp, Bounds.Height);
		}
		protected internal virtual int RemoveItem(int itemIndex) {
			int bestWidth = Items[itemIndex].CalcBestSize().Width;
			BarStaticItemLink link = Items[itemIndex].Item as BarStaticItemLink;
			if(link == null || link.Item.AutoSize != BarStaticItemSize.Spring)
			Items.RemoveAt(itemIndex);
			return bestWidth;
		}
		protected internal virtual int LayoutLeftItems() {
			Point location = ContentBounds.Location;
			Size bestSize = Size.Empty;
			bool hasItems = false;
			for(int itemIndex = 0; itemIndex < Items.Count; itemIndex++) {
				if(IsRightAlign(Items[itemIndex].Item)) continue;
				bestSize = Items[itemIndex].CalcBestSize();
				Items[itemIndex].Bounds = new Rectangle(new Point(location.X, location.Y + (ContentBounds.Height - bestSize.Height) / 2), bestSize);
				location.X += bestSize.Width + DefaultItemIndent;
				hasItems = true;
			}
			if(hasItems)
				location.X -= DefaultItemIndent + ContentBounds.Location.X;
			return location.X;
		}
		protected internal virtual int LayoutRightItems() {
			Point location = new Point(ContentBounds.Right, ContentBounds.Top);
			Size bestSize = Size.Empty;
			bool hasItems = false;
			for(int itemIndex = Items.Count - 1; itemIndex >= 0 ; itemIndex--) {
				if(!IsRightAlign(Items[itemIndex].Item)) continue;
				bestSize = Items[itemIndex].CalcBestSize();
				location.X -= bestSize.Width;
				Items[itemIndex].Bounds = new Rectangle(new Point(location.X, location.Y + (ContentBounds.Height - bestSize.Height) / 2), bestSize);
				location.X -= DefaultItemIndent;
				hasItems = true;
			}
			if(hasItems)
				location.X += DefaultItemIndent;
			return ContentBounds.Right - location.X;
		}
		protected internal virtual void LayoutItems() {
			int leftWidth = LayoutLeftItems();
			int rightWidth = LayoutRightItems();
			int freeSpace = ContentBounds.Width - leftWidth - rightWidth - CalcRightContentIndent();
			LayoutAutoFitItems(freeSpace);
		}
		protected virtual int CalcSpringItems() {
			int res = 0;
			for(int i = 0; i < Items.Count; i++) {
				RibbonItemViewInfo item = Items[i];
				if(item.SpringAllow)
					res++;
			}
			return res;
		}
		protected virtual void LayoutAutoFitItems(int freeSpace) {
			if(freeSpace == 0)
				return;
			int springItems = CalcSpringItems();
			if(springItems == 0)
				return;
			int itemAdditionWidth = freeSpace / springItems;
			LayoutAutoFitItemsLeft(itemAdditionWidth);
			LayoutAutoFitItemsRight(itemAdditionWidth);
		}
		protected virtual void LayoutAutoFitItemsLeft(int itemAdditionWidth) {
			int processedItems = 0;
			for(int i = 0; i < Items.Count; i++) {
				RibbonItemViewInfo item = Items[i];
				if(IsRightAlign(Items[i].Item)) continue;
				int addWidth = !item.SpringAllow? 0: itemAdditionWidth;
					item.Bounds = new Rectangle(item.Bounds.X + processedItems * itemAdditionWidth, item.Bounds.Y, item.Bounds.Width + addWidth, item.Bounds.Height);
				if(item.SpringAllow)
					processedItems++;   
				item.GetInfo().CalcViewInfo(GInfo.Graphics, item);
			}
		}
		protected virtual void LayoutAutoFitItemsRight(int itemAdditionWidth) {
			int processedItems = 0;
			for(int i = Items.Count - 1; i >= 0; i--) {
				RibbonItemViewInfo item = Items[i];
				if(!IsRightAlign(Items[i].Item)) continue;
				int addWidth = !item.SpringAllow ? 0 : itemAdditionWidth;
				item.Bounds = new Rectangle(item.Bounds.X - processedItems * itemAdditionWidth - addWidth, item.Bounds.Y, item.Bounds.Width + addWidth, item.Bounds.Height);
				if(item.SpringAllow)
					processedItems++;
				item.GetInfo().CalcViewInfo(GInfo.Graphics, item);
			}
		}
		protected internal virtual bool IsRightAlign(IRibbonItem item) {
			BarItemLink link = item as BarItemLink;
			if(link == null || link.Alignment != BarItemLinkAlignment.Right) return false;
			return true;
		}
		protected internal override RibbonItemViewInfo CreateItemViewInfo(IRibbonItem item) {
			RibbonItemViewInfo vi = Manager.CreateItemViewInfo(this, item);
			BarEditItemLink link = item as BarEditItemLink;
			if(link == null || IsDesignMode) return vi;
			BarAnimatedItemsHelper.UpdateAnimatedItem(StatusBar, AnimationInvoker, vi as RibbonEditItemViewInfo);
			return vi;
		}
		CustomAnimationInvoker animationInvoker;
		public CustomAnimationInvoker AnimationInvoker { 
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(StatusBar.OnAnimation);
				return animationInvoker;
			} 
		}
		protected internal virtual void CreateItemsViewInfo() {
			ClearItemsViewInfo();
			int leftICount = 0, rightICount = 0;
			List<BarItemLink> links = InplaceLinksHelper.GetLinks(StatusBar.Manager, StatusBar.ItemLinks, StatusBar.Ribbon.AllowInplaceLinks, StatusBar.IsDesignMode, (link) => ShouldCreateItemInfo(link));
			for(int itemIndex = 0; itemIndex < links.Count; itemIndex++) {
				IRibbonItem item = links[itemIndex];
				if(!ShouldCreateItemInfo(links[itemIndex])) continue;
				RibbonItemViewInfo itemInfo = CreateItemViewInfo(links[itemIndex]);
				if(itemInfo == null) continue;
				itemInfo.Owner = this;
				itemInfo.AllowedStyles &= ~RibbonItemStyles.Large;
				itemInfo.CalculateOwnHeight = StatusBar.AutoHeight;
				if(IsRightAlign(item)) {
					if(rightICount++ > 0 && item.BeginGroup)
						Items.Add(CreateSeparatorViewInfo(item));
				}
				else {
					if(leftICount++ > 0 && item.BeginGroup)
						Items.Add(CreateSeparatorViewInfo(item));
				}
				Items.Add(itemInfo);
			}
		}
		public override RibbonHitInfo CalcHitInfo(Point hitPoint) {
			RibbonHitInfo res = CreateHitInfo();
			res.HitPoint = hitPoint;
			if(!res.ContainsSet(Bounds, RibbonHitTest.StatusBar)) return res;
			res.StatusBar = StatusBar;
			for(int itemIndex = 0; itemIndex < Items.Count; itemIndex++) {
				if(Items[itemIndex].Bounds.Contains(res.HitPoint)) {
					Items[itemIndex].CalcHitInfo(res);
					continue;
				}
			}
			return res;
		}
		int CalcAutoHeight() { 
			int height = 0;
			for(int i = 0; i < Items.Count; i++) {
				height = Math.Max(height, Items[i].CalcBestSize().Height);
			}
			return height;
		}
		public virtual int CalcBestHeight() {
			if(StatusBar.Ribbon == null) return 20;
			GInfo.AddGraphics(null);
			try {
				CalcConstants();
				int height = Math.Max(ButtonHeight, StatusBar.AutoHeight ? CalcAutoHeight() : ButtonHeight);
				return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetStatusBarInfo(), new Rectangle(0, 0, 0, height)).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual void ClearItemsViewInfo() { Items.Clear(); }
		protected internal virtual RibbonSeparatorItemViewInfo CreateSeparatorViewInfo(IRibbonItem item) {
			return new RibbonSeparatorItemViewInfo(this, item);
		}
		protected internal override void Clear() {
			IsReady = false;
			Items.Clear();
		}
		protected internal bool InSizeGrip(Point pt) {
			if(StatusBar == null || !CanShowSizeGrip) return false;
			Rectangle rect = SizeGripBounds;
			if(rect.IsEmpty || pt.Y < rect.Y) return false;
			return (!IsRightToLeft && pt.X >= rect.X) || (IsRightToLeft && pt.X <= rect.Right);
		}
	}
}
