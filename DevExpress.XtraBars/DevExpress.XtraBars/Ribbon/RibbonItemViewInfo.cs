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
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.Utils.Text;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public delegate Size DCalcSize(Graphics graphics, RibbonItemViewInfo item);
	public delegate void DCalcViewInfo(Graphics graphics, RibbonItemViewInfo item);
	public delegate void DDrawItem(GraphicsCache cache, RibbonItemViewInfo item);
	public delegate SkinElementInfo DGetElementInfo(RibbonItemViewInfo item);
	public class RibbonItemInfo {
		DCalcSize calcSize;
		DCalcViewInfo calcViewInfo;
		DDrawItem draw, drawText;
		DGetElementInfo getSkinInfo;
		DGetElementInfo getSkinInfo2;
		public RibbonItemInfo(DCalcSize calcSize, DCalcViewInfo calcViewInfo, DDrawItem draw, DDrawItem drawText, DGetElementInfo getSkinInfo) : this(calcSize, calcViewInfo, draw, drawText, getSkinInfo, null) { }
		public RibbonItemInfo(DCalcSize calcSize, DCalcViewInfo calcViewInfo, DDrawItem draw, DDrawItem drawText, DGetElementInfo getSkinInfo, DGetElementInfo getSkinInfo2) {
			this.calcSize = calcSize;
			this.calcViewInfo = calcViewInfo;
			this.draw = draw;
			this.drawText = drawText;
			this.getSkinInfo = getSkinInfo;
			this.getSkinInfo2 = getSkinInfo2;
		}
		public DGetElementInfo GetSkinInfo { get { return getSkinInfo; } }
		public DGetElementInfo GetSkinInfo2 { get { return getSkinInfo2; } }
		public DCalcSize CalcSize { get { return calcSize; } }
		public DCalcViewInfo CalcViewInfo { get { return calcViewInfo; } }
		public DDrawItem Draw { get { return draw; } }
		public DDrawItem DrawText { get { return drawText; } }
	}
	public class RibbonItemTextWrapper {
		RibbonControlStyle ribbonStyle;
		public RibbonItemTextWrapper(RibbonControlStyle ribbonStyle) {
			this.ribbonStyle = ribbonStyle;
		}
		RibbonControlStyle RibbonStyle { get { return ribbonStyle; } }
		public virtual string UnWrapString(string text) {
			if(text == null || text.Length == 0) return string.Empty;
			if(text.IndexOfAny(new char[] { '\xd', '\xa' }) == -1) return text;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int skippedCarets = -1;
			for(int n = 0; n < text.Length; n++) {
				char ch = text[n];
				if(ch == '\xd' || ch == '\xa') {
					if(++skippedCarets == 1) sb.Append(' ');
					continue;
				}
				skippedCarets = 0;
				sb.Append(ch);
			}
			return sb.ToString();
		}
		public virtual string[] WrapString(string text) {
			return WrapString(text, false);
		}
		public virtual string[] WrapString(string text, bool forceWrap) {
			string[] lines = new string[] { string.Empty, string.Empty };
			if(text == null || text.Length == 0) return lines;
			lines[0] = text;
			if(RibbonStyle == RibbonControlStyle.MacOffice)
				return lines;
			return BarItemLinkTextHelper.WrapString(text, forceWrap);
		}
	}
	public class RibbonItemViewInfo {
		IRibbonItem item;
		BarLinkRectangles rects;
		BaseRibbonViewInfo viewInfo;
		bool isReady = false;
		bool shouldWrapText;
		RibbonItemStyles allowedStyles = RibbonItemStyles.All;
		Image extraGlyph;
		object owner = null;
		int rowCount = 2;
		AppearanceObject paintAppearance = null;
		public RibbonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) {
			this.shouldWrapText = true;
			this.item = item;
			this.viewInfo = viewInfo;
			UpdateItem();
			this.HtmlStringInfo = null;
		}
		protected internal StringInfo HtmlStringInfo { get; set; }
		public virtual HorzAlignment TextAlignment { get { return HorzAlignment.Near; } }
		protected internal bool CalculateOwnHeight { get; set; }
		protected internal virtual void UpdatePaintAppearance() {
			paintAppearance = new AppearanceObject();
			BarItemLink link = Item as BarItemLink;
			if(link == null) {
				paintAppearance = ViewInfo.PaintAppearance.Item;
			} else
				UpdatePaintAppearanceCore(paintAppearance, link.Item);
		}
		protected AppearanceObject GetRibbonInfoAppearance(ObjectState state) {
			if(!Enabled)
				return ViewInfo.PaintAppearance.ItemDisabled;
			if((state & ObjectState.Pressed) != 0)
				return ViewInfo.PaintAppearance.ItemPressed;
			if((state & ObjectState.Hot) != 0)
				return ViewInfo.PaintAppearance.ItemHovered;
			return ViewInfo.PaintAppearance.Item;
		}
		protected virtual AppearanceObject GetDefaultPaintApperance(ObjectState linkState) {
			AppearanceObject itemOwnerAppearance = GetRibbonInfoAppearance(linkState);
			SkinElementInfo info = GetInfo().GetSkinInfo(this);
			AppearanceObject defAppearance = info == null || info.Element == null ? itemOwnerAppearance : info.Element.GetForeColorAppearance(itemOwnerAppearance, Enabled ? ObjectState.Normal : ObjectState.Disabled);
			Skin skin = RibbonSkins.GetSkin(ViewInfo.Provider);
			if(ViewInfo is RibbonStatusBarViewInfo) {
				defAppearance = skin[RibbonSkins.SkinStatusBarButton].GetForeColorAppearance(defAppearance, linkState);
			}
			defAppearance = CheckDefaultAppearanceItemArrow(linkState, defAppearance);
			if(Owner is RibbonQuickAccessToolbarViewInfo) {
				defAppearance = (AppearanceObject)defAppearance.Clone();
				RibbonViewInfo vi = ViewInfo as RibbonViewInfo;
				if(vi.Form != null && skin.Colors.Contains(RibbonSkins.SkinForeColorInCaptionQuickAccessToolbar) && vi.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above)
					defAppearance.ForeColor = Enabled ? skin.Colors[RibbonSkins.SkinForeColorInCaptionQuickAccessToolbar] : skin.Colors[RibbonSkins.SkinForeColorDisabledInCaptionQuickAccessToolbar];
				else if(vi.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above && skin.Colors.Contains(RibbonSkins.SkinForeColorInTopQuickAccessToolbar))
					defAppearance.ForeColor = Enabled ? skin.Colors[RibbonSkins.SkinForeColorInTopQuickAccessToolbar] : skin.Colors[RibbonSkins.SkinForeColorDisabledInTopQuickAccessToolbar];
				else if(skin.Colors.Contains(RibbonSkins.SkinForeColorInPageHeader))
					defAppearance.ForeColor = Enabled ? skin.Colors[RibbonSkins.SkinForeColorInBottomQuickAccessToolbar] : skin.Colors[RibbonSkins.SkinForeColorDisabledInBottomQuickAccessToolbar];
			}
			else if(Owner is RibbonPageHeaderViewInfo) {
				defAppearance = (AppearanceObject)defAppearance.Clone();
				defAppearance.ForeColor = Enabled ? skin.Colors[RibbonSkins.SkinForeColorInPageHeader] : skin.Colors[RibbonSkins.SkinForeColorDisabledInPageHeader];
			}
			return defAppearance;
		}
		protected virtual AppearanceObject CheckDefaultAppearanceItemArrow(ObjectState linkState, AppearanceObject defAppearance) {
			if(linkState == ObjectState.Hot && ViewInfo.HotObject.HitTest == RibbonHitTest.ItemDrop) {
				Color color = Color.Empty;
				if(ViewInfo is RibbonStatusBarViewInfo)
					color = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinStatusBarButton].Properties.GetColor("ForeColorHot2");
				else 
					color = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinButton].Properties.GetColor("ForeColorHot2");
				if(!color.IsEmpty) {
					defAppearance = (AppearanceObject)defAppearance.Clone();
					defAppearance.ForeColor = color;
				}
			}
			return defAppearance;
		}
		protected virtual AppearanceObject GetDefaultPaintApperance() {
			return GetDefaultPaintApperance(CalcState());
		}
		protected virtual void UpdatePaintAppearanceCore(AppearanceObject destAppearance, BarItem item) {
			AppearanceHelper.Combine(destAppearance, GetItemAppearance(item), GetDefaultPaintApperance());
		}
		protected virtual AppearanceObject GetItemAppearance(BarItem item) {
			return GetItemAppearance(item, false);
		}
		protected virtual AppearanceObject GetItemAppearance(BarItem item, bool useForArrow) {
			ObjectState state = CalcState();
			if(!Enabled)
				return item.ItemAppearance.Disabled;
			if((state & ObjectState.Pressed) != 0) {
				if((ViewInfo.PressedObject.HitTest == RibbonHitTest.ItemDrop && !useForArrow) || 
					(ViewInfo.PressedObject.HitTest != RibbonHitTest.ItemDrop && useForArrow))
					return item.ItemAppearance.Hovered;
				return item.ItemAppearance.Pressed;
			}
			if((state & ObjectState.Hot) != 0)
				return item.ItemAppearance.Hovered;
			return item.ItemAppearance.Normal;
		}
		public virtual void CheckViewInfo(Graphics graphics) {
			if(IsReady) return;
			if(ViewInfo != null) graphics = ViewInfo.GInfo.AddGraphics(graphics);
			GetInfo().CalcViewInfo(graphics, this);
			IsReady = true;
			if(ViewInfo != null) ViewInfo.GInfo.ReleaseGraphics();
			BarItemLink link = Item as BarItemLink;
			if(link != null) link.GlyphBounds = GlyphBounds;
		}
		public virtual RibbonItemViewInfo Find(IRibbonItem item, Rectangle bounds) {
			if(Bounds == bounds) return this;
			return null;
		}
		public virtual int RowCount { get { return rowCount; } set { rowCount = value; } }
		public virtual bool CanKeyboardSelect { get { return true; } }
		public virtual Size ImageSize { get { return ViewInfo.ImageSize; } }
		public virtual Image ExtraGlyph { get { return extraGlyph; } set { extraGlyph = value; } }
		public virtual bool ShouldWrapText { get { return shouldWrapText; } set { shouldWrapText = value; } }
		public virtual bool IsImageExists {
			get {
				if(ExtraGlyph != null) return true;
				BarItemLink link = Item as BarItemLink;
				if(link == null) return false;
				return link.IsImageExist;
			}
		}
		public virtual bool CanAnimate {
			get {
				BarItemLink link = Item as BarItemLink;
				if(link != null && link.Item == null)
					return false;
				return Enabled;
			}
		}
		public bool Enabled { get { return item.Enabled && OwnerControl.Enabled; } }
		public BarLinkRectangles Rects {
			get {
				if(rects == null) rects = new BarLinkRectangles();
				return rects;
			}
		}
		protected internal virtual void OnItemChanged() {
			if(ViewInfo != null) ViewInfo.OnItemChanged(this);
		}
		public virtual void Invalidate() {
			if(ViewInfo != null) ViewInfo.Invalidate(this);
		}
		public virtual void InvalidateOwner() {
			Invalidate();
		}
		public virtual void CalcHitInfo(RibbonHitInfo hitInfo) {
			hitInfo.SetItem(this);
		}
		protected internal virtual void UpdateItem() {
			if(Item == null) return;
			BarItemLink link = Item as BarItemLink;
			if(link != null) link.RibbonItemInfo = this;
			AllowedStyles = Item.AllowedStyles;
		}
		protected internal virtual Control OwnerControl { get { return ViewInfo != null ? ViewInfo.OwnerControl : null; } }
		public virtual Point PointToScreen(Point p) {
			if(OwnerControl == null || !OwnerControl.IsHandleCreated) return p;
			return OwnerControl.PointToScreen(p);
		}
		public virtual Point PointToClient(Point p) {
			if(OwnerControl == null || !OwnerControl.IsHandleCreated) return p;
			return OwnerControl.PointToClient(p);
		}
		protected internal DevExpress.XtraBars.ViewInfo.BarLinkViewInfo GetLinkViewInfo() {
			BarItemLink link = Item as BarItemLink;
			if(link != null) {
				DevExpress.XtraBars.ViewInfo.BarLinkViewInfo res = link.CreateViewInfo();
				if(res != null) res.Bounds = Bounds;
				return res;
			}
			return null;
		}
		protected internal object GetOwner() {
			if(OwnerButtonGroup == null) return Owner;
			return OwnerButtonGroup.GetOwner();
		}
		public virtual RibbonPageGroupViewInfo OwnerPageGroup { get { return Owner as RibbonPageGroupViewInfo; } }
		public virtual object Owner {
			get { return owner; }
			set { 
				owner = value;
				UpdateItem();
			}
		}
		public RibbonButtonGroupItemViewInfo OwnerButtonGroup { get { return Owner as RibbonButtonGroupItemViewInfo; } }
		public virtual RibbonItemViewInfoCalculator ItemCalculator { get { return ViewInfo.ItemCalculator; } }
		public virtual string Text { get { return Item.Text; } }
		public virtual AppearanceObject GetPaintAppearance() {
			SkinElementInfo si = GetItemInfo();
			ObjectState state = ViewInfo.OwnerControl.Enabled && Enabled? si.State : ObjectState.Disabled;
			return GetPaintAppearanceCore(si, state);
		}
		protected virtual AppearanceObject GetPaintAppearanceCore(SkinElementInfo elementInfo, ObjectState state) {
			if(state != ObjectState.Disabled && Appearance.Options.UseForeColor) return Appearance;
			return elementInfo.Element.GetForeColorAppearance(Appearance, state);
		}
		public virtual AppearanceObject Appearance { 
			get {
				if(paintAppearance == null)
					UpdatePaintAppearance();
				return paintAppearance;
			} 
		}
		public BaseRibbonViewInfo ViewInfo { get { return viewInfo; } }
		public IRibbonItem Item { get { return item; } }
		public virtual Rectangle GlyphBounds { 
			get { return Rects[BarLinkParts.Glyph]; } 
			set { Rects[BarLinkParts.Glyph] = value; } 
		}
		public virtual Rectangle CaptionBounds { get { return Rects[BarLinkParts.Caption]; } set { Rects[BarLinkParts.Caption] = value; } }
		public virtual Rectangle Bounds { get { return Rects[BarLinkParts.Bounds]; } set { Rects[BarLinkParts.Bounds] = value; } }
		public virtual Size CalcBestSize() {
			UpdatePaintAppearance();
			return GetInfo().CalcSize(ViewInfo.GInfo.Graphics, this); 
		}
		protected virtual bool IsAllowState(RibbonItemStyles state) {
			return (AllowedStyles & state) != 0;
		}
		public virtual RibbonItemStyles AllowedStyles { 
			get { return allowedStyles; }
			set { allowedStyles = value; }
		}
		public virtual bool IsSeparator { get { return false; } }
		public virtual bool IsLargeButton { get { return Item.IsLargeButton; } }
		protected virtual bool IsItemHotTracked {
			get {
				RibbonControl r = ViewInfo.OwnerControl as RibbonControl;
				if(r != null && r.IsSystemLink(Item as BarItemLink)) {
					return ViewInfo.HotObject.Item == Item;
				}
				return ViewInfo.HotObject.ItemInfo == this && ViewInfo.Manager.SelectionInfo.HighlightedLink == Item;
			}
		}
		protected virtual bool CanDrawHotTracked { get { return Item != null && Item.Enabled; } }
		protected internal virtual ObjectState CalcState() {
			ObjectState state = ObjectState.Normal;
			if(!Enabled) {
				state = ObjectState.Disabled;
				if(ViewInfo.KeyboardActiveInfo.ItemInfo == this) state |= ObjectState.Hot;
				return state;
			}
			if(IsItemHotTracked && CanDrawHotTracked) state |= ObjectState.Hot;
			if(ViewInfo.PressedObject.ItemInfo == this) state |= ObjectState.Pressed;
			if(Item.IsChecked) state |= ObjectState.Selected;
			if(ViewInfo.KeyboardActiveInfo.ItemInfo == this) state |= ObjectState.Hot;
			return state;
		}
		public virtual bool IsReady { get { return isReady; } set { isReady = value; } }
		public virtual void Clear() {
			this.isReady = false;
			if(this.rects != null) this.rects.Clear();
		}
		public virtual SkinElementInfo GetItemInfo() {
			SkinElementInfo info = GetInfo().GetSkinInfo(this);
			info.Bounds = Bounds;
			info.State = CalcState();
			info.ImageIndex = CalcImageIndex(info.State & (~ObjectState.Selected)); 
			return info;
		}
		protected virtual int CalcImageIndex(ObjectState state) {
			int res = -1;
			if(state == ObjectState.Hot) res = 1;
			if((state & ObjectState.Pressed) != 0) res = 2;
			return res;
		}
		public virtual RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcButtonNoTextSize,
				ItemCalculator.Helper.CalcButtonNoTextViewInfo,
				ItemCalculator.Helper.DrawButton,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
		protected internal virtual bool IsDroppedDown {
			get {
				if(!Item.IsDroppedDown) return false;
				return true;
			}
		}
		protected internal virtual RibbonItemViewInfo GetNavItem(int delta) { return this; }
		public virtual bool ShowImageInToolbar { get { return true;} }
		public bool IsInHeaderArea { get { return (Owner is RibbonQuickAccessToolbarViewInfo || Owner is RibbonPageHeaderViewInfo); } }
		protected internal void Refresh() {
			if(OwnerControl != null) OwnerControl.Update();
		}
		public virtual bool SpringAllow { 
			get {
				ISpringLink spLink = Item as ISpringLink;
				return spLink != null && spLink.SpringAllow;
			} 
		}
		internal void SetPaintAppearance(AppearanceObject appearance) {
			this.paintAppearance = appearance;
		}
	}
	public class RibbonSeparatorItemViewInfo : RibbonItemViewInfo {
		RibbonPageGroupViewInfo groupInfo;
		RibbonStatusBarViewInfo statusBarInfo;
		RibbonQuickAccessToolbarViewInfo quickAccessToolbarInfo;
		RibbonPageHeaderViewInfo headerInfo;
		RibbonMiniToolbarViewInfo toolbarInfo;
		public RibbonSeparatorItemViewInfo(RibbonMiniToolbarViewInfo toolbarInfo, IRibbonItem item)
			: base(toolbarInfo.ViewInfo, item) {
			this.toolbarInfo = toolbarInfo;
		}
		public RibbonSeparatorItemViewInfo(RibbonPageHeaderViewInfo headerInfo, IRibbonItem item) : base(headerInfo.ViewInfo, item) {
			this.headerInfo = headerInfo;
		}
		public RibbonSeparatorItemViewInfo(RibbonPageGroupViewInfo groupInfo, IRibbonItem item)
			: base(groupInfo.ViewInfo, item) {
			this.groupInfo = groupInfo;
		}
		public RibbonSeparatorItemViewInfo(RibbonStatusBarViewInfo statusBarInfo, IRibbonItem item) : base(statusBarInfo, item) {
			this.statusBarInfo = statusBarInfo;
		}
		public RibbonSeparatorItemViewInfo(RibbonQuickAccessToolbarViewInfo quickAccessToolbarInfo)
			: base(quickAccessToolbarInfo.ViewInfo, null) {
			this.quickAccessToolbarInfo = quickAccessToolbarInfo;
		}
		public override bool SpringAllow {
			get {
				return false;
			}
		}
		public override bool CanKeyboardSelect { get { return false; } }
		public override bool CanAnimate { get { return false; } }
		protected internal override void UpdateItem() { }
		protected internal RibbonPageHeaderViewInfo HeaderInfo { get { return headerInfo; } }
		protected internal RibbonPageGroupViewInfo GroupInfo { get { return groupInfo; } }
		protected internal RibbonStatusBarViewInfo StatusBarInfo { get { return statusBarInfo; } }
		protected internal RibbonQuickAccessToolbarViewInfo ToolbarInfo { get { return quickAccessToolbarInfo; } }
		protected internal RibbonMiniToolbarViewInfo MiniToolbarInfo { get { return toolbarInfo; } }
		protected internal Rectangle OwnerContentBounds {
			get {
				if(GroupInfo != null) return groupInfo.ContentBounds;
				if(StatusBarInfo != null) return StatusBarInfo.ContentBounds;
				if(ToolbarInfo != null) return ToolbarInfo.ContentBounds;
				return Rectangle.Empty;
			}
		}
		public override void CalcHitInfo(RibbonHitInfo hitInfo) {
			hitInfo.SetItem(this, RibbonHitTest.ItemSeparator);
		}
		public override bool IsSeparator { get { return true; } }
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcVertSeparatorSize,
				ItemCalculator.Helper.CalcVertSeparatorViewInfo,
				ItemCalculator.Helper.DrawVertSeparator,
				null,
				ItemCalculator.Helper.GetVertSeparatorElementInfo);
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = GetInfo().GetSkinInfo(this);
			info.Bounds = Bounds;
			return info;
		}
	}
	public class RibbonButtonGroupItemViewInfo : RibbonItemViewInfo {
		RibbonItemViewInfoCollection items;
		Size bestSize;
		public RibbonButtonGroupItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
			this.items = new RibbonItemViewInfoCollection(this);
			CreateItems();
		}
		protected internal virtual ButtonGroupsLayout GetGroupsLayout() {
			BarButtonGroup group = (BarButtonGroup)((BarButtonGroupLink)Item).Item;
			if(group.ButtonGroupsLayout != ButtonGroupsLayout.Default)
				return group.ButtonGroupsLayout;
			if(group.Ribbon != null && group.Ribbon.ButtonGroupsLayout != ButtonGroupsLayout.Default)
				return group.Ribbon.ButtonGroupsLayout;
			return ButtonGroupsLayout.Auto;
		}
		public override int RowCount {
			get {
				ButtonGroupsLayout layout = GetGroupsLayout();
				if(layout == ButtonGroupsLayout.ThreeRows) 
					return 3;
				if(layout == ButtonGroupsLayout.TwoRows)
					return 2;
				return base.RowCount;
			}
			set {
				base.RowCount = value;
			}
		}
		public override RibbonItemViewInfo Find(IRibbonItem item, Rectangle bounds) {
			if(Bounds.Contains(bounds)) {
				CheckViewInfo(null);
				return Items.Find(item, bounds);
			}
			return null;
		}
		public override bool CanAnimate { get { return false; } }
		protected virtual void CreateItems() {
			if(Items.Count > 0) return;
			IRibbonItem[] children = Item.GetChildren();
			if(children == null) return;
			foreach(IRibbonItem item in children) {
				BarItemLink link = item as BarItemLink;
				if(link != null && link.Item.Visibility == BarItemVisibility.Never && !ViewInfo.IsDesignMode) continue;
				RibbonItemViewInfo itemInfo = ViewInfo.CreateItemViewInfo(item);
				itemInfo.Owner = this;
				Items.Add(itemInfo);
			}
		}
		public override Size CalcBestSize() {
			UpdatePaintAppearance();
			if(bestSize == Size.Empty) bestSize = base.CalcBestSize();
			return bestSize;
		}
		public virtual BarItemLink GetLinkByPoint(Point pt) {
			if(Items.Count == 0 && Bounds.Contains(pt)) return Item as BarItemLink;
			foreach(RibbonItemViewInfo itemInfo in Items) {
				if(itemInfo is RibbonSeparatorItemViewInfo) continue;
				if(itemInfo.Bounds.Contains(pt)) return itemInfo.Item as BarItemLink;
			}
			return null;
		}
		public override bool IsLargeButton { get { return false; } }
		public override void CalcHitInfo(RibbonHitInfo hitInfo) {
			foreach(RibbonItemViewInfo itemInfo in Items) {
				if(hitInfo.ContainsSet(itemInfo.Bounds, RibbonHitTest.Item)) {
					itemInfo.CalcHitInfo(hitInfo);
					return;
				}
			}
			if(Items.Count == 0) {
				hitInfo.ContainsSet(Bounds, RibbonHitTest.Item);
				hitInfo.SetItem(this);
			}
		}
		public RibbonItemViewInfoCollection Items { get { return items; } }
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcButtonGroupSize,
				ItemCalculator.Helper.CalcButtonGroupViewInfo,
				ItemCalculator.Helper.DrawButtonGroup,
				null,
				ItemCalculator.Helper.GetButtonGroupElementInfo);
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = GetInfo().GetSkinInfo(this);
			info.Bounds = Bounds;
			return info;
		}
		protected override int CalcImageIndex(ObjectState state) {
			int res = 0;
			if(state == ObjectState.Hot) res = 1;
			if((state & ObjectState.Pressed) != 0) res = 2;
			if(Item.IsChecked) res += 3;
			return res;
		}
	}
	public class RibbonStaticItemViewInfo : RibbonItemViewInfo {
		public RibbonStaticItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		protected override int CalcImageIndex(ObjectState state) {
			return 0; 
		}
		protected internal override ObjectState CalcState() {
			ObjectState state = base.CalcState();
			BarStaticItemLink link = Item as BarStaticItemLink;
			if(link != null) {
				if(state.HasFlag(ObjectState.Hot) && CheckAppearance(link.Item.ItemAppearance.Hovered)) return state;
				if(state.HasFlag(ObjectState.Pressed) && CheckAppearance(link.Item.ItemAppearance.Pressed)) return state;
			}
			return state & (~(ObjectState.Hot | ObjectState.Pressed));
		}
		bool CheckAppearance(AppearanceObject appearance) {
			AppearanceOptions opt = appearance.Options;
			return opt.UseBackColor || opt.UseBorderColor || opt.UseFont || opt.UseForeColor || opt.UseImage || opt.UseTextOptions;
		}
		public override HorzAlignment TextAlignment {
			get { 
				BarStaticItem barItem = (Item as BarStaticItemLink).Item;
				if(barItem.TextAlignment == StringAlignment.Center)
					return HorzAlignment.Center;
				if(barItem.TextAlignment == StringAlignment.Far)
					return HorzAlignment.Far;
				return HorzAlignment.Near;
			}
		}
		public override bool CanKeyboardSelect { get { return false; } }
		public override bool CanAnimate { get { return false; } }
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcStaticItemSize,
				ItemCalculator.Helper.CalcStaticItemViewInfo,
				ItemCalculator.Helper.DrawButton,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetStaticElementInfo);
		}
		public override bool ShowImageInToolbar {
			get {
				BarStaticItemLink link = Item as BarStaticItemLink;
				if(link == null) return base.ShowImageInToolbar;
				return link.Item.ShowImageInToolbar;
			}
		}
		bool EmptyGlyphInToolbar {
			get { 
				BarStaticItemLink link = Item as BarStaticItemLink;
				if(IsInHeaderArea && !ShowImageInToolbar) return true;
				return false;
			}
		}
		public override Size ImageSize {
			get {
				if(EmptyGlyphInToolbar) return Size.Empty;
				return base.ImageSize;
			}
		}
	}
	public class RibbonEditItemViewInfo : RibbonItemViewInfo {
		BaseEditViewInfo editViewInfo;
		BaseEditPainter editPainter;
		public RibbonEditItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
			EditViewInfo.EditValue = ((BarEditItemLink)Item).EditValue;
			ChangeGlyphAlignment();
			EditViewInfo.RightToLeft = viewInfo.IsRightToLeft;
		}
		public override bool CanAnimate { get { return false; } }
		public override Image ExtraGlyph { get { return null; } }
		public RepositoryItem Edit { get { return Item.Edit; } }
		public BaseEditViewInfo EditViewInfo {
			get {
				if(editViewInfo == null) {
					editViewInfo = Edit.CreateViewInfo();
					editViewInfo.InplaceType = DevExpress.XtraEditors.Controls.InplaceType.Bars;
					editViewInfo.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
					editViewInfo.Item.UseParentBackground = ((BarItemLink)Item).Ribbon != null && ((BarItemLink)Item).Ribbon.TransparentEditors;
					editViewInfo.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Full;
				}
				return editViewInfo;
			}
		}
		public BaseEditPainter EditPainter {
			get {
				if(editPainter == null) editPainter = Edit.CreatePainter();
				return editPainter;
			}
		}
		protected override AppearanceObject GetDefaultPaintApperance() {
			ObjectState state = CalcState();
			if((state & ObjectState.Disabled) == 0) 
				state = ObjectState.Normal;
			return GetDefaultPaintApperance(state);
		}
		BarManager Manager { get { return ViewInfo.Manager; } }
		protected virtual Color GetEditItemBackColor(ObjectState state) {
			return ViewInfo.ItemCalculator.GetEditItemBackColor(state);
		}
		protected virtual bool ShouldUpdateBackgroundColor { 
			get {
				if(!(EditViewInfo is TextEditViewInfo)) return false;
				if(EditViewInfo.State == ObjectState.Normal && !EditViewInfo.Item.Appearance.Options.UseBackColor) return true;
				if(EditViewInfo.State == ObjectState.Disabled && !EditViewInfo.Item.AppearanceDisabled.Options.UseBackColor) return true;
				return false;
			}
		}
		public virtual void UpdateEditLookAndFeel() {
			if(EditViewInfo.LookAndFeel != Manager.PaintStyle.LinksLookAndFeel && EditViewInfo.LookAndFeel.UseDefaultLookAndFeel) {
				EditViewInfo.LookAndFeel = Manager.PaintStyle.LinksLookAndFeel;
			}
		}
		protected virtual void ChangeGlyphAlignment() {
			RibbonControl control = OwnerControl as RibbonControl;
			if(control != null && control.AutoSizeItems) {
				BaseRepositoryItemCheckEdit checkEdit = Edit as BaseRepositoryItemCheckEdit;
				if(checkEdit != null)
					checkEdit.GlyphAlignment = HorzAlignment.Far;
			}
		}
		public override void CheckViewInfo(Graphics graphics) {
			base.CheckViewInfo(graphics);			
			if(EditViewInfo is ToggleSwitchViewInfo) {
				Size toggleSize = CalcToggleSwitchViewInfoSize(EditViewInfo as ToggleSwitchViewInfo);
				if(EditBounds.Width < toggleSize.Width) (Item as BarEditItemLink).Item.Width = toggleSize.Width;
				if(EditBounds.Height < toggleSize.Height) (Item as BarEditItemLink).Item.EditHeight = toggleSize.Height;				
			}
		}
		protected virtual Size CalcToggleSwitchViewInfoSize(ToggleSwitchViewInfo info) {
			int constFocus = 2 * ToggleSwitchViewInfo.FocusRectThin;
			int width = info.CheckInfo.GlyphRect.Width + constFocus;
			int height = info.CheckInfo.GlyphRect.Height + constFocus;
			return new Size(width, height);
		}
		public virtual void UpdateEditViewInfo(Graphics graphics, Point p) {
			EditViewInfo.EditValue = ItemCalculator.GetEditItemValue(this);
			EditViewInfo.AllowDrawFocusRect = ((BarEditItemLink)Item).Item.Edit.AllowFocused;
			EditViewInfo.AllowOverridedState = !Item.Enabled;
			if(!Item.Enabled)
				EditViewInfo.OverridedState = ObjectState.Disabled;
			EditViewInfo.UpdatePaintAppearance();
			using(AppearanceObject obj = (AppearanceObject)EditViewInfo.PaintAppearance.Clone()) {
				AppearanceHelper.Combine(EditViewInfo.PaintAppearance, new AppearanceObject[] { obj, ViewInfo.PaintAppearance.Editor });
			}
			EditViewInfo.CalcViewInfo(graphics, Control.MouseButtons, p, EditViewInfo.Bounds);
			if(ShouldUpdateBackgroundColor)
				EditViewInfo.PaintAppearance.BackColor = GetEditItemBackColor(EditViewInfo.State);
		}
		public Rectangle EditBounds { get { return EditViewInfo.Bounds; } }
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcEditItemSize,
				ItemCalculator.Helper.CalcEditItemViewInfo,
				ItemCalculator.Helper.DrawEditItem,
				ItemCalculator.Helper.DrawEditItemText,
				ItemCalculator.Helper.GetEditElementInfo);
		}
	}
	public class RibbonCheckItemViewInfo : RibbonButtonItemViewInfo {
		Size checkSize;
		BaseCheckObjectPainter checkPainter;
		public RibbonCheckItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) : base(viewInfo, item) {
			this.checkSize = Size.Empty;
			this.checkPainter = CreateCheckPainter();
		}
		public override RibbonItemStyles AllowedStyles {
			get {
				if(GetCheckBoxVisibility() == CheckBoxVisibility.None || !base.AllowedStyles.HasFlag(RibbonItemStyles.Large)) return base.AllowedStyles;
				var style = (base.AllowedStyles & ~RibbonItemStyles.Large);
				if(style == RibbonItemStyles.Default) style = RibbonItemStyles.SmallWithText;
				return style;
			}
			set {
				if(GetCheckBoxVisibility() == CheckBoxVisibility.None || !value.HasFlag(RibbonItemStyles.Large)) {
					base.AllowedStyles = value;
					return;
				}
				var style = (value & ~RibbonItemStyles.Large);
				if(style == RibbonItemStyles.Default) style = RibbonItemStyles.SmallWithText;
				base.AllowedStyles = style;
			}
		}
		public BaseCheckObjectPainter CheckPainter { get { return checkPainter; } }
		protected virtual BaseCheckObjectPainter CreateCheckPainter() {
			var lookAndFeel = ViewInfo.Manager.GetController().LookAndFeel.ActiveLookAndFeel;
			return CheckPainterHelper.GetPainter(lookAndFeel);
		}
		protected virtual void EnsureCheckPainter() {
			if(ViewInfo.Manager == null) return;
			this.checkPainter = CreateCheckPainter();
		}
		protected internal override void UpdatePaintAppearance() {
			EnsureCheckPainter();
			base.UpdatePaintAppearance();
		}
		public virtual Rectangle CheckBounds {
			get { return Rects[BarLinkParts.Checkbox]; }
			set { Rects[BarLinkParts.Checkbox] = value; }
		}
		public virtual CheckBoxVisibility GetCheckBoxVisibility() {
			var itemLink = Item as BarCheckItemLink;
			if(itemLink == null) return CheckBoxVisibility.None;
			var checkItem = itemLink.Item as BarCheckItem;
			if(checkItem == null) return CheckBoxVisibility.None;
			return checkItem.CheckBoxVisibility;
		}
		internal virtual Size GetCheckSize() {
			if(!checkSize.IsEmpty) return checkSize;
			BarCheckItemLink checkButtonLink = Item as BarCheckItemLink;
			if(checkButtonLink == null) return Size.Empty;
			BarCheckButtonLinkViewInfo checkButtonLinkViewInfo = checkButtonLink.LinkViewInfo as BarCheckButtonLinkViewInfo;
			if(checkButtonLinkViewInfo == null) return Size.Empty;
			checkSize = checkButtonLinkViewInfo.DrawParameters.GetCheckSize();
			return checkSize;
		}
		public override Size CalcBestSize() {
			var res = base.CalcBestSize();
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return res;
			var checkSize = GetCheckSize();
			res.Width += checkSize.Width + ItemCalculator.CaptionHGlyphIndent;
			return res;
		}
		protected internal override ObjectState CalcState() {
			var res = base.CalcState();
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return res;
			if(Item.IsChecked && res.HasFlag(ObjectState.Disabled)) res |= ObjectState.Selected;
			return res;
		}
		protected override AppearanceObject GetPaintAppearanceCore(SkinElementInfo elementInfo, ObjectState state) {
			if(state == ObjectState.Selected) {
				return elementInfo.Element.GetForeColorAppearance(Appearance, state);
			}
			return base.GetPaintAppearanceCore(elementInfo, state);
		}
		protected virtual bool ShouldUseNoTextDelegates {
			get {
				if(GetCheckBoxVisibility() == CheckBoxVisibility.None || AllowedStyles.HasFlag(RibbonItemStyles.SmallWithoutText)) return true;
				return false;
			}
		}
		public override RibbonItemInfo GetInfo() {
			if(CurrentLevel == RibbonItemStyles.Large || CurrentLevel == RibbonItemStyles.SmallWithText) return base.GetInfo();
			return new RibbonItemInfo(
				ShouldUseNoTextDelegates ? ItemCalculator.Helper.CalcButtonNoTextSize : ItemCalculator.Helper.CalcButtonSize,
				ShouldUseNoTextDelegates ? ItemCalculator.Helper.CalcButtonNoTextViewInfo : ItemCalculator.Helper.CalcButtonViewInfo,
				ItemCalculator.Helper.DrawButton,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
	}
	public class RibbonButtonItemViewInfo : RibbonItemViewInfo {
		int level;
		RibbonItemStyles[] levels = null;
		Size largeSize, smallWithTextSize, smallWithoutTextSize;
		Rectangle pushBounds;
		public RibbonButtonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
			level = 0;
		}
		protected internal override ObjectState CalcState() {
			ObjectState state = base.CalcState();
			if(state == ObjectState.Disabled)
				return state;
			BarButtonItemLink link = Item as BarButtonItemLink;
			if(link != null && link.IsPopupVisible && link.LinkViewInfo.LinkState == BarLinkState.Highlighted)
				return ObjectState.Hot;
			return state;
		}
		public bool ContainStyle(RibbonItemStyles style) {
			for(int i = 0; i < levels.Length; i++) {
				if(levels[i] == style) return true;
			}
			return false;
		}
		public RibbonItemStyles[] Levels {
			get {
				if(levels == null) levels = CreateLevels();
				return levels;
			}
		}
		public override Size CalcBestSize() {
			if(CurrentLevel == RibbonItemStyles.Large) {
				if(largeSize == Size.Empty) largeSize = base.CalcBestSize();
				return largeSize;	
			}
			else if(CurrentLevel == RibbonItemStyles.SmallWithText) {
				if(smallWithTextSize == Size.Empty) smallWithTextSize = base.CalcBestSize();
				return smallWithTextSize;
			}
			else if(CurrentLevel == RibbonItemStyles.SmallWithoutText) {
				if(smallWithoutTextSize == Size.Empty) smallWithoutTextSize = base.CalcBestSize();
				return smallWithoutTextSize;
			}
			return base.CalcBestSize();
		}
		public Rectangle PushBounds { get { return pushBounds; } set { pushBounds = value; } }
		public RibbonItemStyles CurrentLevel {
			get { 
				return Levels.Length == 0 || Level >= Levels.Length ? RibbonItemStyles.SmallWithoutText : Levels[Level]; 
			}
		}
		protected virtual RibbonItemStyles[] CreateLevels() {
			ArrayList list = new ArrayList();
			if(Item.IsLargeButton && IsAllowState(RibbonItemStyles.Large) && OwnerButtonGroup == null && !ViewInfo.IsOfficeTablet) {
				list.Add(RibbonItemStyles.Large);
			}
			if(Text != string.Empty && IsAllowState(RibbonItemStyles.SmallWithText)) list.Add(RibbonItemStyles.SmallWithText);
			if((IsImageExists && IsAllowState(RibbonItemStyles.SmallWithoutText)) || list.Count == 0) 
				list.Add(RibbonItemStyles.SmallWithoutText);
			return list.ToArray(typeof(RibbonItemStyles)) as RibbonItemStyles[];
		}
		public int MaxLevel { get { return Math.Max(0, Levels.Length - 1); } }
		public void ResetLevel() { level = 0; }
		public int Level { get { return level; } }
		public int NextLevel() {
			if(level < MaxLevel) level++;
			return level;
		}
		public int PrevLevel() {
			if(level > 0) level--;
			return level;
		}
		public bool IsMaxLevel { get { return Level == MaxLevel; } }
		public override bool IsLargeButton { get { return CurrentLevel == RibbonItemStyles.Large; } }
		public override RibbonItemInfo GetInfo() {
			switch(CurrentLevel) {
				case RibbonItemStyles.Large :
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcLargeButtonSize,
						ItemCalculator.Helper.CalcLargeButtonViewInfo,
						ItemCalculator.Helper.DrawLargeButton,
						ItemCalculator.Helper.DrawLargeButtonText,
						ItemCalculator.Helper.GetLargeButtonElementInfo);
				case RibbonItemStyles.SmallWithText:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcButtonSize,
						ItemCalculator.Helper.CalcButtonViewInfo,
						ItemCalculator.Helper.DrawButton,
						ItemCalculator.Helper.DrawButtonText,
						ItemCalculator.Helper.GetButtonElementInfo);
			}
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcButtonNoTextSize,
				ItemCalculator.Helper.CalcButtonNoTextViewInfo,
				ItemCalculator.Helper.DrawButton,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
		protected override int CalcImageIndex(ObjectState state) {
			int res = 0;
			if((state & ObjectState.Hot) != 0) res = 1;
			if((state & ObjectState.Pressed) != 0) res = 2;
			if(Item.IsChecked) res += 3;
			return res;
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = base.GetItemInfo();
			if(Item.IsLargeButton && ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice){
				info.Bounds = PushBounds;
			}
			return info;	
		}
	}
	public class RibbonToggleSwitchItemViewInfo : RibbonButtonItemViewInfo {
		public RibbonToggleSwitchItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) : base(viewInfo, item) { }
		public override RibbonItemInfo GetInfo() {
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcToggleSwitchItemSize,
				ItemCalculator.Helper.CalcToggleSwitchItemViewInfo,
				ItemCalculator.Helper.DrawToggleSwitchItem,
				ItemCalculator.Helper.DrawButtonText,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
		public Rectangle ToggleBounds { get; set; }
		public Rectangle ToggleContentBounds { get; set; }
		public Rectangle ThumbBounds { get; set; }
	}
	public class RibbonDropDownItemViewInfo : RibbonButtonItemViewInfo {
		Rectangle arrowBounds;
		public RibbonDropDownItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		public virtual bool AllowDrawDropDownArrow {
			get {
				BarButtonItemLink link = Item as BarButtonItemLink;
				if (link != null) return link.Item.ActAsDropDown && link.Item.AllowDrawArrow;
				BarSubItemLink subItemLink = Item as BarSubItemLink;
				if (subItemLink != null) {
					BarSubItem subItem = (BarSubItem)subItemLink.Item;
					return subItem.IsNeedOpenArrow;
				}
				return true;
			}
		}
		bool IsPageGroupContentButton { 
			get {
				return OwnerPageGroup != null && OwnerPageGroup.ContentButtonLink == Item;
			} 
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo res = base.GetItemInfo();
			if(IsDroppedDown) res.ImageIndex = 3;
			if(ViewInfo.GetRibbonStyle() != RibbonControlStyle.MacOffice || !AllowDrawDropDownArrow || IsPageGroupContentButton)
				return res;
			if(!PushBounds.IsEmpty) {
				Rectangle rect = PushBounds;
				rect.Width += ItemCalculator.CalcArrowWidth(ViewInfo.GInfo.Graphics);
				res.Bounds = rect;
			}
			return res;
		}
		public Rectangle ArrowBounds { get { return arrowBounds; } set { arrowBounds = value; } }
		public override RibbonItemInfo GetInfo() {
			switch(CurrentLevel) {
				case RibbonItemStyles.Large:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcLargeDropDownButtonSize,
						ItemCalculator.Helper.CalcLargeButtonViewInfo,
						ItemCalculator.Helper.DrawLargeDropDownButton,
						ItemCalculator.Helper.DrawLargeDropDownButtonText,
						ItemCalculator.Helper.GetLargeButtonElementInfo);
				case RibbonItemStyles.SmallWithText:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcDropDownButtonSize,
						ItemCalculator.Helper.CalcDropDownButtonViewInfo,
						ItemCalculator.Helper.DrawDropDownButton,
						ItemCalculator.Helper.DrawDropDownButtonText,
						ItemCalculator.Helper.GetButtonElementInfo);
			}
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcDropDownButtonNoTextSize,
				ItemCalculator.Helper.CalcDropDownButtonNoTextViewInfo,
				ItemCalculator.Helper.DrawDropDownButtonNoText,
				null,
				ItemCalculator.Helper.GetButtonElementInfo);
		}
	}
	public class RibbonCheckDropDownButtonItemViewInfo : RibbonSplitButtonItemViewInfo {
		public RibbonCheckDropDownButtonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		protected bool IsPressedState(ObjectState state, bool isDropDownSkinElement) {
			if(Item.Enabled && Item.IsChecked && ViewInfo.HotObject.ItemInfo == this) {
				if(ViewInfo.HotObject.HitTest == RibbonHitTest.ItemDrop && !isDropDownSkinElement) return true;
				if(ViewInfo.HotObject.HitTest == RibbonHitTest.Item && isDropDownSkinElement) return true;
			}
			return Item.IsChecked && state != ObjectState.Hot && state != ObjectState.Disabled;
		}
		protected override int GetImageIndex(SkinElementInfo info) {
			int index = base.GetImageIndex(info);
			if(IsPressedState(info.State, false)) return 2;
			return index;
		}
		public override SkinElementInfo GetDropDownInfo() {
			SkinElementInfo info = base.GetDropDownInfo();
			if(IsPressedState(info.State, true)) info.ImageIndex = 2;
			return info;
		}
	}
	public class RibbonSplitButtonItemViewInfo : RibbonButtonItemViewInfo {
		public RibbonSplitButtonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) {
		}
		public virtual Rectangle PushButtonBounds { get { return Rects[BarLinkParts.PushButton]; } set { Rects[BarLinkParts.PushButton] = value; } }
		public virtual Rectangle DropButtonBounds { get { return Rects[BarLinkParts.DropButton]; } set { Rects[BarLinkParts.DropButton] = value; } }
		public override void CalcHitInfo(RibbonHitInfo hitInfo) {
			base.CalcHitInfo(hitInfo);
			if(DropButtonBounds.Contains(hitInfo.HitPoint)) {
				hitInfo.SetItem(this, RibbonHitTest.ItemDrop);
				return;
			}
		}
		AppearanceObject dropDownAppearance;
		public virtual AppearanceObject DropDownAppearance {
			get {
				if(dropDownAppearance == null)
					UpdateDropDownPaintAppearance();
				return dropDownAppearance;
			}
		}
		Color LightenColor(Color color, float alpha) { 
			return Color.FromArgb(255, 
				(int)(color.R * alpha + (1 - alpha) * 255),
				(int)(color.G * alpha + (1 - alpha) * 255),
				(int)(color.B * alpha + (1 - alpha) * 255)
				);
		}
		protected override void UpdatePaintAppearanceCore(AppearanceObject destAppearance, BarItem item) {
			base.UpdatePaintAppearanceCore(destAppearance, item);
			if(CalcState() == ObjectState.Hot && ViewInfo.HotObject.HitTest == RibbonHitTest.ItemDrop) {
				if(destAppearance.BackColor.A > 0)
					destAppearance.BackColor = LightenColor(destAppearance.BackColor, 0.7f);
				if(destAppearance.BackColor2.A > 0)
					destAppearance.BackColor2 = LightenColor(destAppearance.BackColor2, 0.7f);
			}
		}
		protected internal override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			UpdateDropDownPaintAppearance();
		}
		protected internal virtual void UpdateDropDownPaintAppearance() {
			dropDownAppearance = new AppearanceObject();
			BarItemLink link = Item as BarItemLink;
			if(link == null) {
				dropDownAppearance = ViewInfo.PaintAppearance.Item;
			}
			else
				UpdateDropDownPaintAppearanceCore(dropDownAppearance, link.Item);
		}
		protected virtual void UpdateDropDownPaintAppearanceCore(AppearanceObject dropDownAppearance, BarItem item) {
			AppearanceHelper.Combine(dropDownAppearance, GetItemAppearance(item, true), GetDefaultPaintApperance());
			if(CalcState() == ObjectState.Hot && ViewInfo.HotObject.HitTest != RibbonHitTest.ItemDrop) {
				if(dropDownAppearance.BackColor.A > 0)
					dropDownAppearance.BackColor = LightenColor(dropDownAppearance.BackColor, 0.7f);
				if(dropDownAppearance.BackColor2.A > 0)
					dropDownAppearance.BackColor2 = LightenColor(dropDownAppearance.BackColor2, 0.7f);
			}
		}
		protected virtual AppearanceDefault GetDefaultDropDownPaintApperance() {
			return new AppearanceDefault();
		}
		protected internal override ObjectState CalcState() {
			ObjectState state = ObjectState.Normal;
			if(!Item.Enabled)
				return ObjectState.Disabled;
			if(CanDrawHotTracked && (IsItemHotTracked || ViewInfo.PressedObject.ItemInfo == this)) state |= ObjectState.Hot;
			if(ViewInfo.PressedObject.ItemInfo == this && ViewInfo.PressedObject.HitTest != RibbonHitTest.ItemDrop) state |= ObjectState.Pressed;
			if(ViewInfo.KeyboardActiveInfo.ItemInfo == this) state |= ObjectState.Hot;
			return state;
		}
		protected internal virtual ObjectState CalcDropState() {
			ObjectState state = ObjectState.Normal;
			if(CanDrawHotTracked && IsItemHotTracked) state |= ObjectState.Hot;
			if(ViewInfo.PressedObject.ItemInfo == this && ViewInfo.PressedObject.HitTest == RibbonHitTest.ItemDrop) state |= ObjectState.Pressed;
			if(ViewInfo.KeyboardActiveInfo.ItemInfo == this) state |= ObjectState.Hot | ObjectState.Pressed;
			if(IsDroppedDown) state |= ObjectState.Pressed;
			return state;
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = GetInfo().GetSkinInfo(this);
			info.Bounds = PushButtonBounds;
			info.State = CalcState();
			info.ImageIndex = GetImageIndex(info);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected virtual int GetImageIndex(SkinElementInfo info) {
			int imageIndex = 0;
			if(info.State == ObjectState.Hot) {
				imageIndex = 3;
				if(ViewInfo.HotObject.HitTest == RibbonHitTest.Item) imageIndex = 1;
			}
			if((info.State & ObjectState.Pressed) != 0) {
				imageIndex = 2;
			}
			if(IsDroppedDown) imageIndex = 3;
			return imageIndex;
		}
		public virtual SkinElementInfo GetDropDownInfo() { 
			SkinElementInfo info = GetInfo().GetSkinInfo2(this);
			info.State = CalcDropState();
			info.ImageIndex = 0;
			if(info.State == ObjectState.Hot) {
				info.ImageIndex = 3;
				if(ViewInfo.HotObject.HitTest == RibbonHitTest.ItemDrop) info.ImageIndex = 1;
			}
			if((info.State & ObjectState.Pressed) != 0) {
				info.ImageIndex = 2;
			}
			if(IsDroppedDown) info.ImageIndex = 2; 
			info.Bounds = DropButtonBounds;
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		public override RibbonItemInfo GetInfo() {
			switch(CurrentLevel) {
				case RibbonItemStyles.Large :
					DGetElementInfo getElementInfo = ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice ? ItemCalculator.Helper.GetSplitButtonElementInfo : ItemCalculator.Helper.GetLargeSplitButtonElementInfo;
					DGetElementInfo getElementInfo2 = ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice ? ItemCalculator.Helper.GetSplitButtonElementInfo2 : ItemCalculator.Helper.GetLargeSplitButtonElementInfo2;
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcLargeSplitButtonSize,
						ItemCalculator.Helper.CalcLargeSplitButtonViewInfo,
						ItemCalculator.Helper.DrawLargeSplitButton,
						ItemCalculator.Helper.DrawLargeSplitButtonText,
						getElementInfo,
						getElementInfo2);
				case RibbonItemStyles.SmallWithText:
					return new RibbonItemInfo(
						ItemCalculator.Helper.CalcSplitButtonSize,
						ItemCalculator.Helper.CalcSplitButtonViewInfo,
						ItemCalculator.Helper.DrawSplitButton,
						ItemCalculator.Helper.DrawSplitButtonText,
						ItemCalculator.Helper.GetSplitButtonElementInfo,
						ItemCalculator.Helper.GetSplitButtonElementInfo2);
			}
			return new RibbonItemInfo(
				ItemCalculator.Helper.CalcSplitButtonNoTextSize,
				ItemCalculator.Helper.CalcSplitButtonNoTextViewInfo,
				ItemCalculator.Helper.DrawSplitButton,
				ItemCalculator.Helper.DrawSplitButtonText,
				ItemCalculator.Helper.GetSplitButtonElementInfo,
				ItemCalculator.Helper.GetSplitButtonElementInfo2);
		}
	}
	public class InRibbonGalleryRibbonItemViewInfo : RibbonItemViewInfo {
		InRibbonGalleryViewInfo galleryInfo;
		public InRibbonGalleryRibbonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) : base(viewInfo, item) {
			this.galleryInfo = CreateRibbonGalleryViewInfo();
			GalleryInfo.ItemInfo = this;
			GalleryLink.ViewInfo = this;
		}
		protected virtual InRibbonGalleryViewInfo CreateRibbonGalleryViewInfo() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return new MacStyleInRibbonGalleryViewInfo(GalleryItem.Gallery, this);
			return new InRibbonGalleryViewInfo(GalleryItem.Gallery);
		}
		public BaseGallery Gallery { get { return GalleryInfo == null ? null : GalleryInfo.Gallery; } }
		public InRibbonGalleryViewInfo GalleryInfo { get { return galleryInfo; } }
		public RibbonGalleryBarItemLink GalleryLink { get { return Item as RibbonGalleryBarItemLink; } }
		public RibbonGalleryBarItem GalleryItem { get { return GalleryLink.Item; } }
		public override Size CalcBestSize() {
			if(((RibbonPageGroupViewInfo)Owner).ViewInfo.Panel.InScroll)
				return Bounds.Size;
			UpdatePaintAppearance();
			return galleryInfo.CalcGalleryBestSize();
		}
		protected internal override void OnItemChanged() {
			IsReady = false;
			base.OnItemChanged();
		}
		public override bool IsReady { 
			get { return GalleryInfo == null ? false : GalleryInfo.IsReady; } 
			set {
				if(GalleryInfo == null || value) return;
				GalleryInfo.IsReady = false;
				GalleryInfo.IsCalculateBestSize = true;
			} 
		}
		public virtual Size CalcSize(Graphics graphics, RibbonItemViewInfo viewInfo) {
			InRibbonGalleryRibbonItemViewInfo gInfo = viewInfo as InRibbonGalleryRibbonItemViewInfo;
			if(gInfo == null) return Size.Empty;
			return gInfo.GalleryInfo.CalcVisibleSize(gInfo.GalleryInfo.Gallery.RowCount, gInfo.GalleryInfo.ActualColCount);
		}
		public virtual void CalcViewInfo(Graphics graphics, RibbonItemViewInfo viewInfo) {
			InRibbonGalleryRibbonItemViewInfo gInfo = viewInfo as InRibbonGalleryRibbonItemViewInfo;
			if(gInfo == null) return;
			gInfo.GalleryInfo.CalcViewInfo(viewInfo.Bounds);
		}
		public InRibbonGalleryControlButtons ControlButtons {
			get {
				if(!ViewInfo.HotObject.InGallery) return InRibbonGalleryControlButtons.None;
				switch(ViewInfo.HotObject.HitTest) {
					case RibbonHitTest.GalleryLeftButton:
						return InRibbonGalleryControlButtons.LeftButton;
					case RibbonHitTest.GalleryRightButton:
						return InRibbonGalleryControlButtons.RightButton;
					case RibbonHitTest.GalleryUpButton:
						return InRibbonGalleryControlButtons.UpButton;
					case RibbonHitTest.GalleryDownButton:
						return InRibbonGalleryControlButtons.DownButton;
					case RibbonHitTest.GalleryDropDownButton:
						return InRibbonGalleryControlButtons.DropDownButton;
					default:
						return InRibbonGalleryControlButtons.None;
				}
			}
		}
		public virtual void DrawItem(GraphicsCache cache, RibbonItemViewInfo viewInfo) {
			InRibbonGalleryRibbonItemViewInfo gInfo = viewInfo as InRibbonGalleryRibbonItemViewInfo;
			if(gInfo == null) return;
			InRibbonImageGalleryInfoArgs infoArgs = gInfo.GalleryInfo.CreateGalleryInfo(ViewInfo) as InRibbonImageGalleryInfoArgs;
			gInfo.GalleryInfo.CheckViewInfo();
			if(((RibbonGalleryBarItemLink)Item).LastMakeVisibleItem != null)
				((RibbonGalleryBarItemLink)Item).MakeVisible(((RibbonGalleryBarItemLink)Item).LastMakeVisibleItem);
			infoArgs.ControlButtons = ControlButtons;
			gInfo.GalleryInfo.Gallery.Painter.Draw(cache, infoArgs);
			if(!viewInfo.Enabled) {
				cache.FillRectangle(cache.GetSolidBrush(Color.FromArgb(50, 127, 127, 127)), viewInfo.Bounds);
			}
		}
		public virtual void DrawText(GraphicsCache cache, RibbonItemViewInfo viewInfo) {
			InRibbonGalleryRibbonItemViewInfo gInfo = viewInfo as InRibbonGalleryRibbonItemViewInfo;
			if(gInfo == null) return;
			ImageGalleryInfoArgs infoArgs = gInfo.GalleryInfo.CreateGalleryInfo(ViewInfo);
			gInfo.GalleryInfo.CheckViewInfo();
			gInfo.GalleryInfo.Gallery.Painter.DrawText(cache, infoArgs);
		}
		public virtual SkinElementInfo GetSkinInfo(RibbonItemViewInfo viewInfo) {
			return null;
		}
		RibbonItemInfo info;
		public override RibbonItemInfo GetInfo() {
			if(info == null)info =  new RibbonItemInfo(new DCalcSize(CalcSize), 
										new DCalcViewInfo(CalcViewInfo), 
										new DDrawItem(DrawItem),
										new DDrawItem(DrawText),
										new DGetElementInfo(GetSkinInfo));
			return info;
		}
		public override Rectangle Bounds {
			get { return base.Bounds; }
			set {
				if(base.Bounds == value) return;
				base.Bounds = value;
				GalleryInfo.Bounds = Bounds;
				GalleryInfo.IsReady = false;
			}
		}
		public override void CalcHitInfo(RibbonHitInfo hitInfo) {
			hitInfo.ContainsSet(GalleryInfo.Bounds, RibbonHitTest.Gallery);
			hitInfo.GalleryInfo = GalleryInfo;
			hitInfo.SetItem(this);
			GalleryInfo.CalcHitInfo(hitInfo);
		}
	}
	public class RibbonExpandCollapseItemViewInfo : RibbonButtonItemViewInfo {
		public RibbonExpandCollapseItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) : base(viewInfo, item) { }
		public override Image ExtraGlyph {
			get {
				RibbonControl ribbon = ViewInfo.OwnerControl as RibbonControl;
				if(ribbon == null)
					return base.ExtraGlyph;
				RibbonPageHeaderViewInfo header = ribbon.ViewInfo.Header;
				return ribbon.Minimized ? header.GetExpandButtonGlyphImage(header.GetSystemLinkState((BarItemLink)Item)) : header.GetCollapseButtonGlyphImage(header.GetSystemLinkState((BarItemLink)Item));
			}
			set { }
		}
	}
	public class AutoHiddenPagesMenuItemViewInfo : RibbonButtonItemViewInfo {
		public AutoHiddenPagesMenuItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) : base(viewInfo, item) { }
		public override Image ExtraGlyph {
			get {
				RibbonControl ribbon = ViewInfo.OwnerControl as RibbonControl;
				if(ribbon == null)
					return base.ExtraGlyph;
				RibbonPageHeaderViewInfo header = ribbon.ViewInfo.Header;
				return ribbon.ViewInfo.Header.GetExpandButtonGlyphImage(header.GetSystemLinkState((BarItemLink)Item));
			}
			set { }
		}
	}
	public class SkinRibbonGalleryRibbonItemViewInfo : InRibbonGalleryRibbonItemViewInfo {
		public SkinRibbonGalleryRibbonItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item)
			: base(viewInfo, item) { }
	}
}
