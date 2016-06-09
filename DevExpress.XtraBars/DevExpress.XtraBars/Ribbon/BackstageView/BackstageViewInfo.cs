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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class BackstageViewInfo {
		BackstageViewItemViewInfoCollection items;
		BackstageViewControl control;
		BackstageViewItemViewInfo hotItem, pressedItem, selectedItem;
		BaseDesignTimeManager designTimeManager;
		Rectangle bounds, contentBounds, rightPaneContentBounds, leftPaneBounds, leftPaneContentBounds, rightPaneBounds, itemsBounds;
		GraphicsInfo gInfo;
		bool isReady, isActiveCore;
		IBackstageViewPrefilterMessageController prefilterMessageController;
		AppearanceObject paintAppearance;
		AppearanceDefault defaultAppearance;
		bool isVScrollVisible;
		int itemBottomLine;
		public BackstageViewInfo(BackstageViewControl control) {
			this.control = control;
			this.gInfo = new GraphicsInfo();
			this.FreezeContent = false;
			this.isActiveCore = false;
			this.ShouldUsePostponedStyleChanging = false;
			this.prefilterMessageController = CreatePrefilterMessageController();
			this.isVScrollVisible = false;
		}
		public bool IsVScrollVisible { get { return isVScrollVisible; } }
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) {
					paintAppearance = CreatePaintAppearance();
					UpdatePaintAppearance();
				}
				return paintAppearance;
			}
		}
		private void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Control.Appearance, Control.ParentAppearance }, DefaultAppearance);
			PaintAppearance.TextOptions.RightToLeft = IsRightToLeft;
		}
		protected internal AppearanceObject PaintAppearanceCore { get { return paintAppearance; } }
		public AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected internal virtual void SetAppearanceDirty() {
			this.paintAppearance = null;
			this.defaultAppearance = null;
			foreach(BackstageViewItemBaseViewInfo item in Items) {
				item.SetAppearanceDirty();
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) {
				if(Control.ParentBackstageView != null) 
					return new AppearanceDefault(Control.ParentAppearance.ForeColor, Control.ParentAppearance.BackColor);
				return new AppearanceDefault(GetNormalForeColor(), GetNormalBackColor(), Color.Empty, Color.Empty);
			}
			return new AppearanceDefault();
		}
		const string foreColor = "ForeColor";
		const string backColor = "BackColor";
		Color GetNormalBackColor() {
			var elem = GetBackgroundInfo().Element;
			if(elem != null && elem.Properties.ContainsProperty(backColor))
				return elem.Properties.GetColor(backColor);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
		}
		Color GetNormalForeColor() {
			var elem = GetBackgroundInfo().Element;
			if(elem != null && elem.Properties.ContainsProperty(foreColor))
				return elem.Properties.GetColor(foreColor);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		protected internal SkinElement GetDefaultSkinElement(string elementName) {
			SkinElement elem = MetroUISkins.GetSkin(Control.GetController().LookAndFeel.ActiveLookAndFeel)[elementName];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DefaultSkinProvider.Default)[elementName];
			return elem;
		}
		protected virtual AppearanceObject CreatePaintAppearance() {
			return new AppearanceObject(DefaultAppearance);
		}
		protected virtual AppearanceObject GetAppearanceByState(ObjectState state) {
			return Control.GetController().AppearancesBackstageView.BackstageView;
		}
		protected internal virtual IBackstageViewPrefilterMessageController CreatePrefilterMessageController() {
			return new BackstageViewPrefilterMessageController(Control);
		}
		public BackstageViewControl Control { get { return control; } }
		public GraphicsInfo GInfo { get { return gInfo; } }
		public IBackstageViewPrefilterMessageController PrefilterMessageController { get { return this.prefilterMessageController; } }
		public virtual void CalcViewInfo(Rectangle bounds) {
			if(IsReady)
				return;
			GInfo.AddGraphics(null);
			try {
				this.bounds = bounds;
				CalcLeftToRightRects();
				if(IsRightToLeft){
					RotateRects();
				}
				this.rightPaneContentBounds = RightPaneBounds;
				this.designTimeManager = new BaseDesignTimeManager(this, Control.Site);
				if(!FreezeContent) {
					CalcItemsViewInfo();
					this.itemBottomLine = CalcBottomLine();
					this.isVScrollVisible = CalcScrollBarVisibility();
					UpdateRightPaneContentBounds();
					UpdateTabContentBounds();
					if(DelayedSelectedItem != null) {
						BackstageViewItemViewInfo itemInfo = GetItemInfo(DelayedSelectedItem) as BackstageViewItemViewInfo;
						if(itemInfo != null)
							SelectedItem = itemInfo;
						DelayedSelectedItem = null;
					}
				}
				IsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual int CalcBottomLine() {
			return Items.Count != 0 ? Items[Items.Count - 1].Bounds.Bottom : 0;
		}
		protected virtual void UpdateRightPaneContentBounds() {
			if(!IsVScrollVisible) return;
			if(IsRightToLeft)
				this.rightPaneContentBounds.X += Control.ScrollController.VScrollWidth;
			this.rightPaneContentBounds.Width -= Control.ScrollController.VScrollWidth;
		}
		protected bool CalcScrollBarVisibility() {
			if(Items.Count != 0) 
				if(Items[Items.Count - 1].Bounds.Bottom - Items[0].Bounds.Top > LeftPaneContentBounds.Height) return true;
			return false;
		}
		protected virtual void CalcLeftToRightRects() {
			this.contentBounds = CalcContentBounds();
			CreateItemsViewInfo();
			leftPaneBounds = CalcLeftPaneBounds();
			rightPaneBounds = CalcRightPaneBounds();
			leftPaneContentBounds = CalcLeftPaneContentBounds();
		}
		public virtual bool ShouldActivateKeyTips {
			get { return Control.ShouldShowKeyTips && !Control.DesignModeCore; }
		}
		protected virtual bool FreezeContent { get; set; }
		public virtual BackstageViewHitInfo CalcHitInfo(Point hitPoint) {
			return null;
		}
		protected internal virtual BackstageViewHitInfo CreateHitInfo() {
			return new BackstageViewHitInfo();
		}
		protected virtual void UpdateTabContentBounds() {
			if(ActiveTabContentControl != null) ActiveTabContentControl.Bounds = RightPaneContentBounds;
		}
		protected BackstageViewClientControl ActiveTabContentControl {
			get {
				if(Control.SelectedTab == null) return null;
				return Control.SelectedTab.ContentControl;
			}
		}
		protected internal virtual Rectangle CalcRightPaneBounds() {
			return new Rectangle(LeftPaneBounds.Right, LeftPaneBounds.Y, ContentBounds.Width - LeftPaneBounds.Width, ContentBounds.Height);
		}
		protected internal BackstageViewItemViewInfo HotItem {
			get { return hotItem; }
			set {
				if(HotItem == value)
					return;
				BackstageViewItemViewInfo prev = HotItem;
				hotItem = value;
				OnHotItemChanged(prev);
			}
		}
		protected internal BackstageViewItemViewInfo PressedItem {
			get { return pressedItem; }
			set {
				if(PressedItem == value)
					return;
				BackstageViewItemViewInfo prev = PressedItem;
				pressedItem = value;
				OnPressedItemChanged(prev);
			}
		}
		protected internal virtual BackstageViewItemViewInfo SelectedItem {
			get { return selectedItem; }
			set {
				if(SelectedItem == value)
					return;
				BackstageViewItemViewInfo prev = SelectedItem;
				selectedItem = value;
				OnSelectedItemChanged(prev);
			}
		}
		public void Invalidate(Rectangle rect) { Control.Invalidate(rect); }
		public void Invalidate(BackstageViewItemViewInfo item) {
			if(item == null)
				return;
			Control.Invalidate(item.Bounds); 
		}
		BackstageViewItem delayedSelectedItem = null;
		protected BackstageViewItem DelayedSelectedItem {
			get { return delayedSelectedItem; }
			set { delayedSelectedItem = value; }
		}
		protected virtual void OnSelectedItemChanged(BackstageViewItemViewInfo prev) {
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(SelectedItem != null) {
				SelectedItem.SetAppearanceDirty();
				Invalidate(SelectedItem);
			}
			BackstageViewTabItem tabItem = SelectedItem == null ? null : SelectedItem.Item as BackstageViewTabItem;
			if(tabItem != null && DelayedSelectedItem == null) {
				if(Control.SelectedTab != tabItem)
					DelayedSelectedItem = tabItem;
				Control.SelectedTab = tabItem;
			}
		}
		protected virtual void OnHotItemChanged(BackstageViewItemViewInfo prev) {
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(HotItem != null) {
				((BackstageViewItem)HotItem.Item).RaiseItemHover();
				HotItem.SetAppearanceDirty();
				Invalidate(HotItem);
			}
			else {
				Control.RaiseHighlightedItemChanged(null);
			}
		}
		protected virtual void OnPressedItemChanged(BackstageViewItemViewInfo prev) {
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(PressedItem != null) {
				if(PressedItem.Item.IsTabItem && PressedItem.Item.Enabled)
					SelectedItem = PressedItem;
				PressedItem.SetAppearanceDirty();
				((BackstageViewItem)PressedItem.Item).RaiseItemPressed();
				Invalidate(PressedItem);
			}
		}
		public virtual BackstageViewItemViewInfo GetItemByPoint(Point pt) {
			return GetItemByPoint(pt, false) as BackstageViewItemViewInfo;
		}
		public virtual BackstageViewItemBaseViewInfo GetItemByPoint(Point pt, bool includeSeparators) {
			if(!LeftPaneContentBounds.Contains(pt)) return null;
			foreach(BackstageViewItemBaseViewInfo item in Items) {
				BackstageViewItemViewInfo itemInfo = item as BackstageViewItemViewInfo;
				if((itemInfo != null || includeSeparators) && item.Bounds.Contains(pt))
					return item;
			}
			return null;
		}
		protected virtual Rectangle CalcContentBounds() {
			if(Control.Ribbon == null)
				return Bounds;
			SkinElementInfo info = GetBackgroundInfo();
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
		}
		protected virtual int IndentBetweenItems {
			get {
				if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) return 0;
				object obj = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptBackstageViewIndentBetweenItems];
				return obj == null ? 0 : (int)obj;
			}
		}
		protected virtual void CalcItemsViewInfo() {
			int top = LeftPaneContentBounds.Y - VScrollOffset;
			itemsBounds = LeftPaneContentBounds;
			foreach(BackstageViewItemBaseViewInfo item in Items) { 
				item.CalcViewInfo(new Rectangle(LeftPaneContentBounds.X + item.Indent, top, LeftPaneContentBounds.Width - item.Indent * 2, item.BestSize.Height));
				top += item.BestSize.Height + IndentBetweenItems;
			}
			if(Items.Count > 0)
				top -= IndentBetweenItems;
			itemsBounds = new Rectangle(ItemsBounds.X, ItemsBounds.Y, ItemsBounds.Width, top - LeftPaneContentBounds.Y);
		}
		protected internal virtual Rectangle CalcLeftPaneContentBounds() {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) return LeftPaneBounds;
			SkinElementInfo info = GetLeftPaneInfo();
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
		}
		public BackstageViewItemViewInfoCollection Items {
			get {
				if(items == null)
					items = CreateItems();
				return items;
			}
		}
		protected virtual BackstageViewItemViewInfoCollection CreateItems() {
			return new BackstageViewItemViewInfoCollection(this);
		}
		protected virtual void CreateItemsViewInfo() {
			ClearItemsViewInfo();
			if(FreezeContent) return;
			foreach(BackstageViewItemBase item in Control.Items) {
				if(!item.IsVisible)
					continue;
				BackstageViewButtonItem buttonItem = item as BackstageViewButtonItem;
				BackstageViewTabItem tabItem = item as BackstageViewTabItem;
				BackstageViewItemSeparator separator = item as BackstageViewItemSeparator;
				if(buttonItem != null)
					Items.Add(CreateButtonItemInfo(buttonItem));
				else if(tabItem != null)
					Items.Add(CreateTabItemInfo(tabItem));
				else if(separator != null)
					Items.Add(CreateSeparatorInfo(separator));
			}
		}
		protected virtual BackstageViewItemBaseViewInfo CreateSeparatorInfo(BackstageViewItemSeparator separator) {
			return new BackstageViewItemSeparatorViewInfo(separator);
		}
		protected virtual BackstageViewTabItemViewInfo CreateTabItemInfo(BackstageViewTabItem tabItem) {
			return new BackstageViewTabItemViewInfo(tabItem);
		}
		protected virtual BackstageViewButtonItemViewInfo CreateButtonItemInfo(BackstageViewButtonItem buttonItem) {
			return new BackstageViewButtonItemViewInfo(buttonItem);
		}
		protected virtual void ClearItemsViewInfo() {
			Items.Clear();
		}
		protected virtual void RotateRects() {
			leftPaneBounds = BarUtilites.ConvertBoundsToRTL(leftPaneBounds, Bounds);
			leftPaneContentBounds = BarUtilites.ConvertBoundsToRTL(leftPaneContentBounds, Bounds);
			rightPaneBounds = BarUtilites.ConvertBoundsToRTL(rightPaneBounds, Bounds);
		}
		public Rectangle RotateRect(Rectangle rect) {
			return new Rectangle(Bounds.Right - rect.X - rect.Width, rect.Y, rect.Width, rect.Height);
		}
		protected virtual Rectangle CalcLeftPaneBounds() {
			return new Rectangle(ContentBounds.Left, ContentBounds.Top, CalcLeftPaneWidth(), ContentBounds.Height);
		}
		protected virtual int CalcLeftPaneWidth() {
			int itemsWidth = CalcItemsBestWidth();
			SkinElementInfo info = GetLeftPaneInfo();
			int paneWidth = Control.GetPaintStyle() != BackstageViewPaintStyle.Flat ? ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(0, 0, itemsWidth, 10)).Width : itemsWidth;
			int res = Math.Max(paneWidth, GetLeftPaneMinWidth());
			if(Control.LeftPaneMaxWidth > 0)
				res = Math.Min(Control.LeftPaneMaxWidth, res);
			return res;
		}
		protected virtual int GetLeftPaneMinWidth() {
			if(Control.LeftPaneMinWidth >= 0)
				return Control.LeftPaneMinWidth;
			if(Control.GetPaintStyle() != BackstageViewPaintStyle.Flat) return 132;
			return 10;
		}
		protected virtual int CalcItemsBestWidth() {
			int width = 0;
			foreach(BackstageViewItemBaseViewInfo item in Items) {
				width = Math.Max(item.CalcBestSize().Width + item.Indent * 2, width);
			}
			return width;
		}
		public bool IsReady {
			get { return isReady; }
			set { isReady = value; }
		}
		protected internal bool IsColored {
			get {
				object obj = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptIsColoredBackstageView];
				if(obj == null)
					return false;
				return (bool)obj;
			}
		}
		protected internal bool IsRightToLeft { get { return Control.IsRightToLeft; } }
		protected internal virtual SkinElement GetBackstageViewImageElement() {
			return RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinBackstageViewImage];
		}
		protected internal bool HasSkinBackgroundImage {
			get { 
				SkinElement elem = GetBackstageViewImageElement();
				return elem != null && elem.Image != null;
			}
		}
		protected internal virtual Rectangle GetBackgroundImageBounds(Control client) {
			Rectangle imageBounds = Rectangle.Empty;
			if(Control.Image != null) {
				imageBounds.Width = Control.Image.Width;
				imageBounds.Height = Control.Image.Height;
			}
			else {
				SkinElement elem = GetBackstageViewImageElement();
				if(elem != null && elem.Image != null) {
					imageBounds.Width = elem.Image.GetImageBounds(0).Width;
					imageBounds.Height = elem.Image.GetImageBounds(0).Height;
				}
			}
			imageBounds.X = Control.IsRightToLeft ? client.ClientRectangle.X : client.ClientRectangle.Right - imageBounds.Width;
			imageBounds.Y = client.ClientRectangle.Bottom - imageBounds.Height;
			return imageBounds;
		}
		protected internal virtual SkinElementInfo GetBackgroundImage(Control client) {
			SkinElement elem = GetBackstageViewImageElement();
			if(elem == null || elem.Image == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(elem, GetBackgroundImageBounds(client));
			info.RightToLeft = IsRightToLeft;
			if(IsColored)
				info.ImageIndex = (int)Control.ColorScheme;
			return info;
		}
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ContentBounds { get { return contentBounds; } }
		public Rectangle LeftPaneBounds { get { return leftPaneBounds; } }
		public Rectangle RightPaneBounds { get { return rightPaneBounds; } }
		public Rectangle ItemsBounds { get { return itemsBounds; } }
		public Rectangle RightPaneContentBounds { get { return rightPaneContentBounds; } }
		public int ItemBottomLine { get { return itemBottomLine; } }
		protected internal Rectangle DropBounds { 
			get {
				Rectangle res = ItemsBounds;
				res.Inflate(0, 3);
				return res;	
			} 
		}
		public Rectangle LeftPaneContentBounds { get { return leftPaneContentBounds; } }
		protected internal int VScrollOffset { get { return Control.ScrollerPosition; } }
		BackstageViewItemBaseViewInfo dragItem;
		BackstageViewItemBaseViewInfo dropItem;
		ItemLocation dropIndicatorLocation;
		public BackstageViewItemBaseViewInfo DragItem { get { return dragItem; } }
		internal void SetDragItem(BackstageViewItemBaseViewInfo value) { this.dragItem = value; }
		public BackstageViewItemBaseViewInfo DropItem { get { return dropItem; } }
		internal void SetDropItem(BackstageViewItemBaseViewInfo value) { this.dropItem = value; }
		public ItemLocation DropIndicatorLocation { get { return dropIndicatorLocation; } }
		internal void SetDropIndicatorLocation(ItemLocation value) { dropIndicatorLocation = value; }
		protected internal virtual SkinElementInfo GetBackgroundInfo() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinApplicationButtonContainerControl], Bounds);
			if(IsColored)
				info.ImageIndex = (int)Control.ColorScheme;
			info.RightToLeft = IsRightToLeft;
			return info;
		}
		protected internal virtual SkinElementInfo GetLeftPaneInfo() {
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinBackstageViewControlLeftPane], LeftPaneBounds);
			res.RightToLeft = IsRightToLeft;
			return res;
		}
		protected internal virtual bool AllowMouseClick(Control control, Point mousePosition) {
			return true;
		}
		public virtual BackstageViewShowRibbonItems BackstageViewShowRibbonItems {
			get { return BackstageViewShowRibbonItems.None; }
		}
		#region Transition Animation
		protected internal virtual void AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction direction) {
		}
		protected internal virtual void ClearTransitionAnimation() {
		}
		#endregion
		public ISkinProvider Provider { get { return Control.GetController().LookAndFeel; } }
		protected internal virtual BackstageViewItemBaseViewInfo GetItemInfo(BackstageViewItemBase item) {
			foreach(BackstageViewItemBaseViewInfo itemInfo in Items) {
				if(itemInfo.Item == item)
					return itemInfo;
			}
			return null;
		}
		protected internal BaseDesignTimeManager DesignTimeManager { get { return designTimeManager; } }
		protected internal virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			BackstageViewItemViewInfo itemInfo = GetItemByPoint(point);
			if(itemInfo == null)
				return null;
			ToolTipControlInfo info = new ToolTipControlInfo();
			info.SuperTip = ((BackstageViewItem)itemInfo.Item).SuperTip;
			info.Object = itemInfo.Item;
			info.ToolTipType = ToolTipType.SuperTip;
			return info;
		}
		protected internal virtual void OnShowing() {
			isActiveCore = true;
		}
		protected internal virtual void OnHidding() {
			isActiveCore = false;
		}
		protected internal virtual void OnHided() {
		}
		protected internal virtual void OnAnimation(BaseAnimationInfo info) {
		}
		protected internal virtual void OnEndAnimation(BaseAnimationInfo info) {
			if(ShouldUsePostponedStyleChanging && Control != null) {
				ShouldUsePostponedStyleChanging = false;
				Control.UpdateViewCore();
			}
		}
		protected internal virtual bool IsAnimationActive { get { return false; } }
		public bool ShouldUsePostponedStyleChanging { get; set; }
		protected internal virtual bool OnWmNcHitTest(ref Message msg) {
			return false;
		}
		protected internal virtual void OnBeginSizingCore() { }
		protected internal virtual void OnEndSizingCore() { }
		protected internal virtual bool CanUpdateBoundsCore(int width, int height) {
			return true;
		}
		protected internal virtual bool CanUpdateSizeCore(MouseEventArgs e) {
			return true;
		}
		protected internal virtual bool CanDoFullScreen(Point pt) {
			return false;
		}
		public virtual bool UseRibbonItemsWithBackstageView {
			get { return false; }
		}
		public virtual bool SizingMode { get { return false; } }
		public bool IsActive { get { return isActiveCore; } }
		internal ScrollArgs CalcVScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Minimum = 0;
			args.Maximum = CalcScrollMax();
			args.LargeChange = LeftPaneContentBounds.Height;
			args.SmallChange = 13;
			args.Value = Control.ScrollerPosition;
			return args;
		}
		int CalcScrollMax() {
			if(Items.Count == 0) return 0;
			return Items[Items.Count - 1].Bounds.Bottom - Items[0].Bounds.Top;
		}
		protected internal int CalcVSmallChange() {
			return Control.ScrollController.VScroll.SmallChange;
		}
	}
	public enum BackstageViewHitTest {
		None, 
		BackButton
	}
	public class BackstageViewHitInfo {
		BackstageViewHitTest hitTest;
		public BackstageViewHitInfo() {
			hitTest = BackstageViewHitTest.None;
		}
		public Point HitPoint {
			get;
			set;
		}
		public bool InBackButton {
			get { return HitTest == BackstageViewHitTest.BackButton; }
		}
		public bool ContainsSet(Rectangle bounds, BackstageViewHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		public override bool Equals(object obj) {
			BackstageViewHitInfo hitInfo = obj as BackstageViewHitInfo;
			if(hitInfo == null)
				return false;
			return hitInfo.HitTest == HitTest;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public BackstageViewHitTest HitTest { get { return hitTest; } }
	}
	public class Office2013BackstageViewInfo : BackstageViewInfo {
		Rectangle headerStripBounds, backButtonBounds, headerBounds;
		BackstageViewBackButtonInfo buttonInfo;
		BackstageViewBackButtonPainter backButtonPainter;
		BackstageViewHitInfo hotObject;
		static readonly int TransitionAnimationLength = 300;
		Size minSize;
		bool sizingMode, isAnimationActiveCore;
		public Office2013BackstageViewInfo(BackstageViewControl control)
			: base(control) {
			this.buttonInfo = CreateBackButtonInfo();
			this.backButtonPainter = CreateBackButtonPainter();
			this.hotObject = CreateHitInfo();
			this.sizingMode  = this.isAnimationActiveCore = false;
		}
		protected override void CalcLeftToRightRects() {
			backButtonBounds = CalcBackButtonBounds();
			base.CalcLeftToRightRects();
			headerBounds = CalcHeaderBounds();
			this.headerStripBounds = CalcHeaderStripBounds();
			this.minSize = CalcMinSize();
		}
		protected override void RotateRects() {
			base.RotateRects();
			headerBounds = BarUtilites.ConvertBoundsToRTL(headerBounds, Bounds);
			backButtonBounds = BarUtilites.ConvertBoundsToRTL(backButtonBounds, Bounds);
		}
		protected internal override IBackstageViewPrefilterMessageController CreatePrefilterMessageController() {
			return new Office2013StyleBackstageViewPrefilterMessageController(Control);
		}
		protected virtual BackstageViewBackButtonPainter CreateBackButtonPainter() {
			return new BackstageViewBackButtonPainter();
		}
		protected virtual BackstageViewBackButtonInfo CreateBackButtonInfo() {
			return new BackstageViewBackButtonInfo(this);
		}
		protected virtual Rectangle CalcHeaderStripBounds() {
			Rectangle bounds = new Rectangle(LeftPaneBounds.Right, 0, 0, RightPaneContentVerticalOffset + 1);
			bounds.Width = Math.Max(0, Bounds.Right - LeftPaneBounds.Right);
			return bounds;
		}
		protected internal virtual SkinElement BackButtonSkinElement {
			get {
				SkinElement element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinBackstageViewBackButton];
				if(element != null)
					return element;
				return CommonSkins.GetSkin(Provider)[CommonSkins.SkinBackButton];
			}
		}
		protected internal override void AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction direction) {
			ClearTransitionAnimation();
			BackstageViewTransitionAnimationInfo info;
			if(IsRightToLeft)
				info = new BackstageViewTransitionAnimationInfo(Control, direction, Bounds.Right - LeftPaneBounds.Width, Bounds.Right , TransitionAnimationLength, Control.Ribbon, Control, Control.Parent as IXtraAnimationListener);
			else info = new BackstageViewTransitionAnimationInfo(Control, direction, -LeftPaneBounds.Width, 0, TransitionAnimationLength, Control.Ribbon, Control, Control.Parent as IXtraAnimationListener);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected internal override void ClearTransitionAnimation() {
			if(XtraAnimator.Current.Get(Control, Control.AnimationId) == null)
				return;
			XtraAnimator.Current.Animations.Remove(Control, Control.AnimationId);
		}
		protected override int CalcLeftPaneWidth() {
			if(!IsAnimationActive)
				return base.CalcLeftPaneWidth();
			return AnimationLeftPaneWidthCore;
		}
		protected int AnimationLeftPaneWidthCore {
			get {
				BackstageViewTransitionAnimationInfo info = XtraAnimator.Current.Get(Control, Control.AnimationId) as BackstageViewTransitionAnimationInfo;
				if(info == null) return 0;
				return Math.Abs(info.EndPos - info.StartPos);
			}
		}
		protected internal override bool IsAnimationActive { get { return isAnimationActiveCore; } }
		protected internal override void OnAnimation(BaseAnimationInfo info) {
			this.isAnimationActiveCore = true;
			base.OnAnimation(info);
			FreezeContent = true;
			if(ActiveTabContentControl == null) return;
			ActiveTabContentControl.Visible = false;
			ActiveTabContentControl.Bounds = new Rectangle(LeftPaneBounds.Width, RightPaneContentBounds.Y, RightPaneContentBounds.Width, RightPaneContentBounds.Height);
		}
		protected internal override void OnEndAnimation(BaseAnimationInfo info) {
			base.OnEndAnimation(info);
			BackstageViewTransitionAnimationInfo taInfo = info as BackstageViewTransitionAnimationInfo;
			FreezeContent = false;
			Control.Update();
			if(ActiveTabContentControl != null) ActiveTabContentControl.Visible = true;
			this.isAnimationActiveCore = false;
			IsReady = false;
			Control.Invalidate();
			if(taInfo != null && taInfo.AnimationListenerControl != null) {
				taInfo.AnimationListenerControl.OnAnimationFinished();
			}
		}
		protected internal virtual Rectangle CalcBackButtonBounds() {
			return new Rectangle(DefaultBackButtonLocation, BackButtonSize);
		}
		protected internal virtual Size BackButtonSize {
			get { return BackButtonSkinElement.Image.GetImageBounds(0).Size; }
		}
		protected override Rectangle CalcContentBounds() {
			Rectangle bounds = base.CalcContentBounds();
			if(!IsAnimationActive)
				return bounds;
			BackstageViewTransitionAnimationInfo info = (BackstageViewTransitionAnimationInfo)XtraAnimator.Current.Get(Control, Control.AnimationId);
			int left = 0;
			int width = bounds.Width;
			if(IsRightToLeft)
				left = info.DirectionInfo == BackstageViewTransitionAnimationInfo.Direction.LeftToRight ? bounds.X + info.StartPos - info.CurrentPos : bounds.X + info.CurrentPos - info.EndPos;
			else
				left = info.DirectionInfo == BackstageViewTransitionAnimationInfo.Direction.LeftToRight ? bounds.X + info.CurrentPos : bounds.X - (info.CurrentPos - info.StartPos);
			return new Rectangle(left, bounds.Y, width, bounds.Height);
		}
		protected internal override Rectangle CalcLeftPaneContentBounds() {
			Rectangle bounds = base.CalcLeftPaneContentBounds();
			bounds.Y += LeftPaneContentVerticalOffset;
			bounds.Height -= LeftPaneContentVerticalOffset;
			return bounds;
		}
		protected internal override Rectangle CalcRightPaneBounds() {
			Rectangle bounds = base.CalcRightPaneBounds();
			bounds.Y += RightPaneContentVerticalOffset;
			bounds.Height -= RightPaneContentVerticalOffset;
			bounds.Inflate(-1, -1);
			return bounds;
		}
		public override BackstageViewHitInfo CalcHitInfo(Point hitPoint) {
			BackstageViewHitInfo hitInfo = CreateHitInfo();
			hitInfo.HitPoint = hitPoint;
			if(hitInfo.ContainsSet(BackButtonBounds, BackstageViewHitTest.BackButton)) 
				return hitInfo;
			return hitInfo;
		}
		protected internal bool ShouldDrawBorder {
			get {
				if(Control == null || Control.Parent == null)
					return true;
				var popupForm = Control.Parent.FindForm() as RibbonOffice2013BackstageViewContainerControl.ContentPopupForm;
				return popupForm != null ? !popupForm.IsTopFormMaximized : true;
			}
		}
		protected internal virtual ObjectInfoArgs GetBackButtonInfo() {
			BackButtonInfo.State = GetBackButtonState();
			BackButtonInfo.Bounds = BackButtonBounds;
			return BackButtonInfo;
		}
		protected virtual ObjectState GetBackButtonState() {
			ObjectState state = ObjectState.Normal;
			if(HotObject.HitTest == BackstageViewHitTest.BackButton)
				state |= ObjectState.Hot;
			return state;
		}
		protected BackstageViewBackButtonInfo BackButtonInfo { get { return buttonInfo; } }
		public virtual BackstageViewHitInfo HotObject {
			get { return hotObject; }
			set {
				if(hotObject.Equals(value))
					return;
				BackstageViewHitInfo prev = HotObject;
				hotObject = value;
				OnHotObjectChanged(prev, HotObject);
			}
		}
		protected virtual void OnHotObjectChanged(BackstageViewHitInfo prev, BackstageViewHitInfo current) {
			Invalidate(prev);
			Invalidate(current);
		}
		public virtual void Invalidate(BackstageViewHitInfo hitInfo) {
			if(hitInfo.HitTest == BackstageViewHitTest.None)
				return;
			if(hitInfo.HitTest == BackstageViewHitTest.BackButton) {
				Invalidate(BackButtonBounds);
				return;
			}
		}
		protected internal override bool AllowMouseClick(Control control, Point mousePosition) {
			return true;
		}
		public bool CanDrawTitle(Office2013BackstageViewInfo vi) {
			if((BackstageViewShowRibbonItems & BackstageViewShowRibbonItems.Title) == 0 || vi.Control.DesignModeCore || vi.IsAnimationActive)
				return false;
			RibbonCaptionPainter cp = vi.GetCaptionPainter();
			return cp != null;
		}
		public override BackstageViewShowRibbonItems BackstageViewShowRibbonItems {
			get {
				BackstageViewShowRibbonItems res = Control.BackstageViewShowRibbonItems;
				if(res == BackstageViewShowRibbonItems.Default)
					res = BackstageViewShowRibbonItems.All;
				return res;
			}
		}
		public virtual RibbonCaptionPainter GetCaptionPainter() {
			if(Control.Ribbon == null || Control.Ribbon.ViewInfo.Form == null)
				return null;
			return Control.Ribbon.ViewInfo.Caption.CaptionPainter as RibbonCaptionPainter;
		}
		protected virtual Point DefaultBackButtonLocation {
			get { return new Point(ContentBounds.X + 21, 20); }
		}
		protected virtual int LeftPaneContentVerticalOffset {
			get { return Math.Max(Control.Office2013StyleOptions.LeftPaneContentVerticalOffset, BackButtonBounds.Bottom + 14); }
		}
		protected virtual int RightPaneContentVerticalOffset {
			get { return Control.Office2013StyleOptions.RightPaneContentVerticalOffset; }
		}
		protected internal override bool OnWmNcHitTest(ref Message msg) {
			if(ShouldApplyTransparency(ref msg)) {
				msg.Result = new IntPtr(-1); 
				return true;
			}
			return false;
		}
		protected virtual bool ShouldApplyTransparency(ref Message msg) {
			if(Control.Ribbon == null)
				return false;
			if(!IsFadeInAnimationProgress)
				return false;
			Point pt = Control.PointToClient(msg.LParam.PointFromLParam());
			return Control.Ribbon.ViewInfo.ApplicationButton.Bounds.Contains(pt);
		}
		protected bool IsFadeInAnimationProgress {
			get {
				if(!IsAnimationActive)
					return false;
				BackstageViewTransitionAnimationInfo info = XtraAnimator.Current.Get(Control, Control.AnimationId) as BackstageViewTransitionAnimationInfo;
				if(info == null)
					return false;
				return info.DirectionInfo == BackstageViewTransitionAnimationInfo.Direction.LeftToRight;
			}
		}
		protected internal virtual Rectangle CalcHeaderBounds() {
			Rectangle headerBounds = new Rectangle();
			headerBounds.X = RightPaneBounds.Left;
			if(Control.Ribbon == null || Control.Ribbon.ViewInfo.Form == null) {
				headerBounds.Y = 3;
				headerBounds.Width = RightPaneBounds.Width;
				headerBounds.Height = Math.Max(ContentBounds.Height - RightPaneBounds.Height, 0);
			}
			else {
				var ribbonViewInfo = Control.Ribbon.ViewInfo;
				var buttonsBounds = CalcButtonsBounds();
				headerBounds.Y = ribbonViewInfo.Caption.TextBounds.Y;
				headerBounds.Width = RightPaneBounds.Width - buttonsBounds.Width;
				headerBounds.Height = ribbonViewInfo.Caption.TextBounds.Height;
			}
			return headerBounds;
		}
		protected internal virtual Size CalcMinSize() {
			if(Control.Ribbon == null) {
				return new Size(LeftPaneBounds.Width, 10);
			}
			var ribbonViewInfo = Control.Ribbon.ViewInfo;
			var buttonsBounds = CalcButtonsBounds();
			var borders = CalcFormBorderSizes();
			var height = ribbonViewInfo.Caption.Bounds.Height + borders;
			var width = LeftPaneBounds.Width + buttonsBounds.Width + borders;
			return new Size(width, height);
		}
		protected internal virtual Rectangle CalcButtonsBounds() {
			if(Control.DesignModeCore || Control.Ribbon == null || Control.Ribbon.ViewInfo.Form == null) {
				return Rectangle.Empty;
			}
			RibbonForm form = Control.Ribbon.ViewInfo.Form;
			if(form.FormPainter == null || form.FormPainter.Buttons.ButtonsBounds.IsEmpty)
				return Rectangle.Empty;
			FormCaptionButtonCollection buttons = form.FormPainter.Buttons;
			Rectangle buttonsBounds = new Rectangle();
			buttonsBounds.X = buttons.ButtonsBounds.X + ContentBounds.Width - form.Width;
			buttonsBounds.Y = buttons.ButtonsBounds.Y + ContentBounds.Height - form.Height;
			buttonsBounds.Width = buttons.ButtonsBounds.Width;
			buttonsBounds.Height = buttons.ButtonsBounds.Height;
			if(Control.Ribbon.ShowFullScreenButton == DefaultBoolean.False) {
				buttonsBounds.Width += 2;
				return buttonsBounds;
			}
			buttonsBounds.Width = buttonsBounds.Width - buttons[0].Bounds.Width;
			return buttonsBounds;
		}
		protected internal virtual int CalcFormBorderSizes() {
			if(Control.Ribbon == null || Control.Ribbon.ViewInfo.Form == null)
				return 0;
			var ribbonFormSize = Control.Ribbon.ViewInfo.Form.Size;
			var clientSize = Control.Ribbon.ViewInfo.Form.ClientSize;
			return (ribbonFormSize.Width - clientSize.Width) / 2;
		}
		protected internal override bool CanUpdateBoundsCore(int dWidth, int dHeight) {
			return ((ContentBounds.Width + dWidth >= MinSize.Width) && (ContentBounds.Height + dHeight >= MinSize.Height));
		}
		protected internal override bool CanUpdateSizeCore(MouseEventArgs e) {
			return TopForm != null ? IsFormSizable(TopForm) : false;
		}
		static bool IsFormSizable(Form form) {
			if(form.WindowState == FormWindowState.Maximized) return false;
			return form.FormBorderStyle == FormBorderStyle.Sizable || form.FormBorderStyle == FormBorderStyle.SizableToolWindow;
		}
		double opacity;
		protected internal override void OnBeginSizingCore() {
			this.opacity = TopForm.Opacity; 
			TopForm.Opacity = 0;
			Control.SuspendLayout();
			this.sizingMode = true;
			if(Control != null)
				Control.Region = null;
			if(PopupContainer != null)
				PopupContainer.Region = null;
		}
		protected internal override void OnEndSizingCore() {
			var ribbon = Control.Ribbon;
			TopForm.Location = PopupContainer.Location;
			TopForm.Size = PopupContainer.Size;
			if(ribbon != null)
				ribbon.Update();
			this.sizingMode = false;
			TopForm.Opacity = this.opacity;
			if(ribbon != null)
				ribbon.ApplicationButtonContentControl.UpdateContentBounds();
			Control.ResumeLayout(true);
		}
		public Rectangle BackButtonBounds { get { return backButtonBounds; } }
		public BackstageViewBackButtonPainter BackButtonPainter { get { return backButtonPainter; } }
		public Rectangle HeaderBounds { get { return headerBounds; } }
		public Rectangle HeaderStripBounds { get { return headerStripBounds; } }
		public Size MinSize { get { return minSize; } }
		public override bool SizingMode { get { return sizingMode; } }
		protected Form TopForm {
			get {
				if(Control.Ribbon == null) return null;
				return Control.Ribbon.FindForm();
			}
		}
		public virtual bool ShouldDrawHeaderStrip {
			get { return Control.Office2013StyleOptions.HeaderBackColor != Color.Empty; }
		}
		public Color HeaderBackColor { get { return Control.Office2013StyleOptions.HeaderBackColor; } }
		public override bool UseRibbonItemsWithBackstageView {
			get { return RibbonSkins.GetSkin(Provider).Properties.GetBoolean(RibbonSkins.OptUseRibbonItemsWithBackstageView); }
		}
		public bool ShouldDrawFormButtons {
			get {
				if(!UseRibbonItemsWithBackstageView)
					return false;
				return SizingMode && !IsAnimationActive;
			}
		}
		protected internal override bool CanDoFullScreen(Point pt) {
			var rect = new Rectangle(LeftPaneBounds.Width, LeftPaneBounds.Y, RightPaneBounds.Right, RightPaneBounds.Y);
			return rect.Contains(pt);
		}
		protected Form PopupContainer { get { return Control.Parent as Form; } }
	}
	public class BackstageViewBackButtonInfo : ObjectInfoArgs {
		Office2013BackstageViewInfo viewInfo;
		public BackstageViewBackButtonInfo(Office2013BackstageViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public SkinElementInfo GetInfo() {
			SkinElementInfo info = new SkinElementInfo(viewInfo.BackButtonSkinElement, Bounds);
			info.State = State;
			info.ImageIndex = CalcImageIndex(State);
			info.RightToLeft = viewInfo.IsRightToLeft;
			return info;
		}
		protected virtual int CalcImageIndex(ObjectState state) {
			if(state == ObjectState.Hot) return 1;
			return 0;
		}
		protected Office2013BackstageViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class BackstageViewTransitionAnimationInfo : BaseAnimationInfo {
		Direction direction;
		int startPos, endPos;
		IBackstageViewAnimationListener animationListenerControl;
		IXtraAnimationListener[] listeners;
		BackstageViewControl backstageView;
		SplineAnimationHelper splineHelper;
		public BackstageViewTransitionAnimationInfo(BackstageViewControl backstageView, Direction direction, int startPos, int endPos, int lenght, IBackstageViewAnimationListener animationListenerControl, params IXtraAnimationListener[] listeners)
			: base(backstageView, backstageView.AnimationId, 10, (int)(TimeSpan.TicksPerMillisecond * lenght / 10)) {
			this.splineHelper = new SplineAnimationHelper();
			this.backstageView = backstageView;
			this.direction = direction;
			this.startPos = startPos;
			this.endPos = endPos;
			CurrentPos = startPos;
			SplineHelper.Init(StartPos, EndPos, 1);
			this.animationListenerControl = animationListenerControl;
			this.listeners = listeners;
			Array.ForEach(Listeners, listener => listener.OnAnimation(this));
		}
		public override void FrameStep() {
			double k = ((double)(CurrentFrame)) / FrameCount;
			CurrentPos = (int)SplineHelper.CalcSpline(k);
			if(IsFinalFrame) {
				CurrentPos = EndPos;
			}
			BackstageView.ViewInfo.IsReady = false;
			BackstageView.Invalidate(InvalidatedRectangle);
			if(IsFinalFrame) {
				Array.ForEach(Listeners, listener => listener.OnEndAnimation(this));
			}
		}
		protected Rectangle InvalidatedRectangle {
			get {
				Rectangle bounds = BackstageView.ViewInfo.LeftPaneBounds;
				int x = BackstageView.IsRightToLeft ? StartPos : EndPos;
				return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
			}
		}
		public Direction DirectionInfo { get { return direction; } }
		public int StartPos { get { return startPos; } }
		public int EndPos { get { return endPos; } }
		public int CurrentPos { get; private set; }
		public SplineAnimationHelper SplineHelper { get { return splineHelper; } }
		public IXtraAnimationListener[] Listeners { get { return listeners; } }
		public BackstageViewControl BackstageView { get { return backstageView; } }
		public bool FadeIn { get { return DirectionInfo == Direction.LeftToRight; } }
		public bool FadeOut { get { return DirectionInfo == Direction.RightToLeft; } }
		public IBackstageViewAnimationListener AnimationListenerControl { get { return animationListenerControl; } }
		public enum Direction { LeftToRight, RightToLeft  }
	}
	public class BackstageViewItemViewInfoCollection : CollectionBase { 
		BackstageViewInfo owner;
		public BackstageViewItemViewInfoCollection(BackstageViewInfo owner) {
			this.owner = owner;
		}
		public BackstageViewInfo Owner { get { return owner; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			BackstageViewItemBaseViewInfo itemInfo = (BackstageViewItemBaseViewInfo)value;
			itemInfo.Items = this;
		}
		public BackstageViewItemBaseViewInfo this[int index] { get { return (BackstageViewItemBaseViewInfo)List[index]; } set { List[index] = value; } }
		public int Add(BackstageViewItemBaseViewInfo item) { return List.Add(item); }
		public void Insert(int index, BackstageViewItemBaseViewInfo item) { List.Insert(index, item); }
		public bool Contains(BackstageViewItemBaseViewInfo item) { return List.Contains(item); }
		public int IndexOf(BackstageViewItemBaseViewInfo item) { return List.IndexOf(item); }
		public void Remove(BackstageViewItemBaseViewInfo item) { List.Remove(item); }
	}
	public class BackstageViewItemBaseViewInfo {
		BackstageViewItemBase item;
		BackstageViewItemViewInfoCollection items;
		Rectangle bounds;
		Size bestSize;
		AppearanceObject paintAppearance;
		AppearanceDefault defaultAppearance;
		public BackstageViewItemBaseViewInfo(BackstageViewItemBase item) {
			this.item = item;
		}
		public BackstageViewItemViewInfoCollection Items {
			get { return items; }
			set { items = value; }
		}
		public virtual BackstageViewItemBasePainter Painter { get { return new BackstageViewItemBasePainter(); } }
		public Size BestSize { get { return bestSize; } }
		protected void SetBestSize(Size value) { this.bestSize = value; }
		public BackstageViewItemBase Item { get { return item; } }
		public Rectangle Bounds { get { return bounds; } }
		protected void SetBounds(Rectangle value) { this.bounds = value; }
		public virtual SkinElementInfo GetItemInfo() { return null; }
		public BackstageViewInfo ViewInfo { get { return Items.Owner; } }
		public virtual int Indent { get { return 0; } }
		public BackstageViewControl Control { get { return ViewInfo.Control; } }
		public virtual Size CalcBestSize() {
			SkinElementInfo info = GetItemInfo();
			if(info == null || info.Element == null)
				SetBestSize(Size.Empty);
			else 
				SetBestSize(ObjectPainter.CalcObjectMinBounds(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info).Size);
			return BestSize;
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			SetBounds(bounds);
		}
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) {
					paintAppearance = CreatePaintAppearance();
					UpdatePaintAppearance();
				}
				return paintAppearance;
			}
		}
		protected internal AppearanceObject PaintAppearanceCore { get { return paintAppearance; } }
		public AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected internal virtual void SetAppearanceDirty() {
			this.paintAppearance = null;
			this.defaultAppearance = null;
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = (AppearanceDefault)Control.ViewInfo.DefaultAppearance.Clone();
			if(Control.GetPaintStyle() != BackstageViewPaintStyle.Flat){
				ObjectState state = CalcItemState();
				SkinElementInfo info = GetItemInfo();
				if(info.Element != null) {
					info.Element.ApplyForeColorAndFont(res);
					res.ForeColor = GetColorByState(info.Element, state);
				}
			}
			return res;
		}
		protected internal virtual AppearanceObject GetAppearance() {
			ObjectState state = CalcItemState();
			return GetAppearance(state);
		}
		protected internal virtual AppearanceObject GetAppearance(ObjectState state) {
			return null;
		}
		protected virtual ObjectState CalcItemState() {
			return ObjectState.Normal;
		}
		protected virtual Color GetColorByState(SkinElement elem, ObjectState state) {
			object obj = elem.Properties["ForeColor" + state + Control.ColorScheme];
			if(obj != null) {
				Color c = (Color)obj;
				if(c.IsSystemColor)
					return elem.Owner.GetSystemColor(c);
				return c;
			}
			Color res = elem.GetForeColor(state);
			if(state == ObjectState.Disabled) {
				Skin skin = elem.Owner;
				if(skin.Colors.Contains(RibbonSkins.SkinForeColorDisabledInBackstageView))
					return skin.Colors[RibbonSkins.SkinForeColorDisabledInBackstageView];
				res = elem.Owner.GetSystemColor(SystemColors.GrayText);
				if(!res.IsEmpty)
					return res;
			}
			res = res.IsEmpty ? elem.Color.ForeColor : res;
			return res.IsSystemColor ? elem.Owner.GetSystemColor(res) : res;
		}
		protected virtual AppearanceObject CreatePaintAppearance() {
			return new AppearanceObject(DefaultAppearance);
		}
		protected virtual AppearanceObject GetAppearanceByState(ObjectState state) {
			return null;
		}
		protected virtual AppearanceObject GetItemAppearanceByState(ObjectState state) {
			return null;
		}
		protected virtual void UpdatePaintAppearance() { 
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { GetAppearance(), Control.ViewInfo.PaintAppearance}, DefaultAppearance);
			if(PaintAppearance.TextOptions.Trimming == Trimming.Default)
				PaintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
	}
	public class BackstageViewItemSeparatorViewInfo : BackstageViewItemBaseViewInfo { 
		public BackstageViewItemSeparatorViewInfo(BackstageViewItemSeparator item) : base(item) { }
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinBackstageViewSeparator], Bounds);
			info.RightToLeft = Control.IsRightToLeft;
			return info;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = base.CreateDefaultAppearance();
			res.BackColor = Control.ViewInfo.PaintAppearance.ForeColor;
			return res;
		}
		protected override AppearanceObject GetItemAppearanceByState(ObjectState state) {
			return (Item as BackstageViewItemSeparator).Appearance;
		}
		protected override AppearanceObject GetAppearanceByState(ObjectState state) {
			return Control.GetController().AppearancesBackstageView.Separator;
		}
		protected internal override AppearanceObject GetAppearance() {
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] {
					GetItemAppearanceByState(ObjectState.Normal), GetAppearanceByState(ObjectState.Normal) }, DefaultAppearance);
			return appearance;
		}
		public override Size CalcBestSize() {
			Size res = base.CalcBestSize();
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat) {
				res.Height = 1;
				SetBestSize(res);
			}
			return res;
		}
	}
	public class BackstageViewItemViewInfo : BackstageViewItemBaseViewInfo {
		Items2Panel panel;
		Rectangle contentBounds, glyphBounds, captionBounds;
		StringInfo captionInfo;
		public BackstageViewItemViewInfo(BackstageViewItemBase item) : base(item) {
		}
		protected override AppearanceObject GetItemAppearanceByState(ObjectState state) {
			switch(state) {
				case ObjectState.Hot:
					return Item.AppearanceHover;
				case ObjectState.Disabled:
					return Item.AppearanceDisabled;
			}
			return Item.Appearance;
		}
		protected override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(CaptionInfo != null)
				CaptionInfo.UpdateAppearanceColors(PaintAppearance);
		}
		#region Appearances
		AppearanceDefault defaultAppearanceNormal;
		public AppearanceDefault DefaultAppearanceNormal {
			get {
				if(defaultAppearanceNormal == null)
					defaultAppearanceNormal = CreateDefaultAppearanceNormal();
				return defaultAppearanceNormal;
			}
		}
		AppearanceDefault CreateDefaultAppearanceNormal() {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat)
				return new AppearanceDefault(Control.ViewInfo.PaintAppearance.ForeColor, Control.ViewInfo.PaintAppearance.BackColor); 
			return DefaultAppearance;
		}
		AppearanceDefault defaultAppearanceHovered;
		protected internal virtual AppearanceDefault DefaultAppearanceHovered {
			get {
				if(defaultAppearanceHovered == null) {
					defaultAppearanceHovered = CreateDefaultAppearanceHovered();
				}
				return defaultAppearanceHovered;
			}
		}
		private AppearanceDefault CreateDefaultAppearanceHovered() {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat)
				return new AppearanceDefault(GetHoveredForeColor(), GetHoveredBackColor(), Color.Empty, Color.Empty);
			return DefaultAppearance;
		}
		AppearanceDefault defaultAppearanceSelected;
		protected internal virtual AppearanceDefault DefaultAppearanceSelected {
			get {
				if(defaultAppearanceSelected == null) {
					defaultAppearanceSelected = CreateDefaultAppearanceSelected();
				}
				return defaultAppearanceSelected;
			}
		}
		AppearanceDefault CreateDefaultAppearanceSelected() {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat)
				return new AppearanceDefault(GetSelectedForeColor(), GetSelectedBackColor(), Color.Empty, Color.Empty);
			return DefaultAppearance;
		}
		protected internal override void SetAppearanceDirty() {
			base.SetAppearanceDirty();
			defaultAppearanceNormal = null;
			defaultAppearanceHovered = null;
			defaultAppearanceSelected = null;
		}
		const string foreColor = "ForeColor";
		const string foreColorHot = "ForeColorHot";
		const string foreColorSel = "ForeColorSelected";
		const string backColor = "BackColor";
		const string backColorHot = "BackColorHot";
		const string backColorSel = "BackColorSelected";
		Color GetHoveredForeColor() {
			if(Control.ParentBackstageView == null) {
				var elem = GetItemInfo().Element;
				if(elem != null && elem.Properties.ContainsProperty(foreColorHot))
					return elem.Properties.GetColor(foreColorHot);
				return Control.ViewInfo.GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
			}
			return DefaultAppearance.ForeColor;
		}
		Color GetHoveredBackColor() {
			return ControlPaint.LightLight(Control.ViewInfo.PaintAppearance.BackColor);
		}
		Color GetSelectedForeColor() {
			if(Control.ParentBackstageView == null) {
				var elem = GetItemInfo().Element;
				if(elem != null && elem.Properties.ContainsProperty(foreColorSel))
					return elem.Properties.GetColor(foreColorSel);
				return Control.ViewInfo.GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
			}
			return DefaultAppearance.ForeColor;
		}
		Color GetSelectedBackColor() {
			return ControlPaint.Light(ControlPaint.LightLight(Control.ViewInfo.PaintAppearance.BackColor));
		}
		Color GetDisabledControlColor() {
			Color c = CommonSkins.GetSkin(Control.GetController().LookAndFeel.ActiveLookAndFeel).Colors["DisabledControl"];
			if(c.IsEmpty)
				c = CommonSkins.GetSkin(DefaultSkinProvider.Default).Colors["DisabledControl"];
			return c;
		}
		protected internal override AppearanceObject GetAppearance(ObjectState state) {
			if(state == ObjectState.Disabled)
				return Item.AppearanceDisabled;
			else if(state == ObjectState.Hot)
				return GetAppearanceHovered();
			else if(state == ObjectState.Pressed)
				return GetAppearanceSelected();
			return GetAppearanceNormal();
		}
		protected internal AppearanceObject GetAppearanceHovered() {
			AppearanceObject hovered = new AppearanceObject();
			AppearanceHelper.Combine(hovered, new AppearanceObject[] {
					GetItemAppearanceByState(ObjectState.Hot), GetAppearanceByState(ObjectState.Hot) }, DefaultAppearanceHovered);
			return hovered;
		}
		protected internal AppearanceObject GetAppearanceSelected() {
			AppearanceObject selected = new AppearanceObject();
			AppearanceHelper.Combine(selected, new AppearanceObject[] {
					GetItemAppearanceByState(ObjectState.Pressed), GetAppearanceByState(ObjectState.Pressed) }, DefaultAppearanceSelected);
			return selected;
		}
		protected internal AppearanceObject GetAppearanceNormal() {
			AppearanceObject normal = new AppearanceObject();
			AppearanceHelper.Combine(normal, new AppearanceObject[] { GetItemAppearanceByState(ObjectState.Normal), GetAppearanceByState(ObjectState.Normal) }, DefaultAppearanceNormal); 
			return normal;
		}
		#endregion
		protected internal StringInfo CaptionInfo { get { return captionInfo; } }
		public new BackstageViewItem Item { get { return (BackstageViewItem)base.Item; } }
		protected Items2Panel Panel {
			get {
				if(panel == null)
					panel = new Items2Panel();
				return panel;
			}
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			this.contentBounds = CalcContentBounds(Bounds);
			UpdatePaintAppearance();
			ArrangeGlyphAndCaption(ContentBounds, GlyphBounds.Size, CaptionBounds.Size, ref this.glyphBounds, ref this.captionBounds);
			CalcCaptionInfo();
		}
		protected virtual Rectangle CalcContentBounds(Rectangle bounds) {
			if(Control.GetPaintStyle() != BackstageViewPaintStyle.Flat) {
				SkinElementInfo info = GetItemInfo();
				if(info == null)
					return bounds;
				return ObjectPainter.GetObjectClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info);
			}
			bounds.Inflate(-10, -3);
			return bounds;
		}
		protected virtual void ArrangeGlyphAndCaption(Rectangle contentBounds, Size glyphSize, Size captionSize, ref Rectangle glyphBounds, ref Rectangle captionBounds) {
			Panel.ArrangeItems(contentBounds, glyphSize, captionSize, ref glyphBounds, ref captionBounds);
		}
		public override Size CalcBestSize() {
			this.glyphBounds.Size = CalcGlyphSize();
			this.captionBounds.Size = CalcTextSize();
			UpdatePanelProperties();
			SetBestSize(SizeFromContentSize(Panel.CalcBestSize(GlyphBounds.Size, CaptionBounds.Size)));
			return BestSize;
		}
		protected virtual void CalcCaptionInfo() {
			if(Item.AllowHtmlString)
				this.captionInfo = StringPainter.Default.Calculate(ViewInfo.GInfo.Graphics, PaintAppearance, Item.Caption, CaptionBounds);
		}
		protected virtual Size SizeFromContentSize(Size size) {
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat)
				return new Size(size.Width + 2 * 15, size.Height + 2 * 15);
			SkinElementInfo info = GetItemInfo();
			if(info == null) return size;
			return ObjectPainter.CalcBoundsByClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(Point.Empty, size)).Size;
		}
		protected virtual void UpdatePanelProperties() {
			Panel.StretchContent2 = GetCaptionStretch();
			Panel.Content1Location = GetGlyphLocation();
			Panel.Content1HorizontalAlignment = GetGlyphHorizontalAlignment();
			Panel.Content1VerticalAlignment = GetGlyphVerticalAlignment();
			Panel.Content2HorizontalAlignment = GetCaptionHorizontalAlignment();
			Panel.Content2VerticalAlignment = GetCaptionVerticalAlignment();
			Panel.HorizontalIndent = GetGlyphToCaptionIndent();
			Panel.VerticalIndent = GetGlyphToCaptionIndent();
			Panel.Content1Padding = GetContent1Padding();
			Panel.Content2Padding = GetContent2Padding();
			Panel.HorizontalPadding = GetHorizontalPadding();
			Panel.VerticalPadding = GetVerticalPadding();
		}
		protected virtual Padding GetVerticalPadding() {
			if(Item.Control.ItemsContentPadding.All != 0) {
				return Item.Control.ItemsContentPadding;
			}
			else return Panel.VerticalPadding;
		}
		protected virtual Padding GetHorizontalPadding() {
			if(Item.Control.ItemsContentPadding.All != 0) {
				return Item.Control.ItemsContentPadding;
			}
			else return Panel.HorizontalPadding;
		}
		protected virtual Padding GetContent1Padding() {
			if(Item.Control.ItemsContentPadding.All != 0) {
				return Item.Control.ItemsContentPadding;
			}
			else return Panel.Content1Padding;
		}
		protected virtual Padding GetContent2Padding() {
			if(Item.Control.ItemsContentPadding.All != 0) {
				return Item.Control.ItemsContentPadding;
			}
			else return Panel.Content2Padding;
		}
		protected virtual int GetGlyphToCaptionIndent() {
			int res = 0;
			if(Item.Control.GlyphToCaptionIndent >= 0)
				res = Item.Control.GlyphToCaptionIndent;
			else {
				object obj = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptBackstageViewControlItemGlyphToCaptionIndent];
				res = obj == null? 0: (int)obj;
			}
			return (int)(res * RibbonSkins.GetSkin(ViewInfo.Provider).DpiScaleFactor);
		}
		protected virtual bool GetCaptionStretch() {
			if(Control.Ribbon != null && Control.Ribbon.IsOfficeTablet)
				return false;
			if(Control.GetPaintStyle() == BackstageViewPaintStyle.Flat && Control.IsParentBackstageView)
				return false;
			return true;
		}
		protected virtual ItemLocation GetGlyphLocation() {
			if(Item.GlyphLocation != ItemLocation.Default)
				return Item.GlyphLocation;
			ItemLocation res = Item.Control.GlyphLocation;
			if(res == ItemLocation.Default && Control.Ribbon != null && Control.Ribbon.IsOfficeTablet)
				return ItemLocation.Top;
			if(res == ItemLocation.Default && Control.GetPaintStyle() == BackstageViewPaintStyle.Flat && Control.IsParentBackstageView)
				return ItemLocation.Top;
			if(res == ItemLocation.Default && Control.IsRightToLeft)
				return ItemLocation.Right;
			return res;
		}
		protected virtual ItemHorizontalAlignment GetGlyphHorizontalAlignment() {
			if(Item.GlyphHorizontalAlignment != ItemHorizontalAlignment.Default)
				return Item.GlyphHorizontalAlignment;
			if(Item.Control.GlyphHorizontalAlignment != ItemHorizontalAlignment.Default)
				return Item.Control.GlyphHorizontalAlignment;
			if(Control.Ribbon != null && Control.Ribbon.IsOfficeTablet)
				return ItemHorizontalAlignment.Center;
			if(Item.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat && Control.IsParentBackstageView)
				return ItemHorizontalAlignment.Center;
			if(Item.Control.IsRightToLeft)
				return ItemHorizontalAlignment.Right;
			return ItemHorizontalAlignment.Left;
		}
		protected virtual ItemHorizontalAlignment GetCaptionHorizontalAlignment() {
			if(Item.CaptionHorizontalAlignment != ItemHorizontalAlignment.Default)
				return Item.CaptionHorizontalAlignment;
			ItemHorizontalAlignment res = Item.Control.CaptionHorizontalAlignment;
			if(res == ItemHorizontalAlignment.Default && Control.Ribbon != null && Control.Ribbon.IsOfficeTablet)
				return Item.Control.IsRightToLeft ? ItemHorizontalAlignment.Right : ItemHorizontalAlignment.Left;
			if(res == ItemHorizontalAlignment.Default && Item.Control.GetPaintStyle() == BackstageViewPaintStyle.Flat && Control.IsParentBackstageView)
				return ItemHorizontalAlignment.Center;
			if(res == ItemHorizontalAlignment.Default && Item.Control.IsRightToLeft)
				return ItemHorizontalAlignment.Right;
			return res;
		}
		protected virtual ItemVerticalAlignment GetGlyphVerticalAlignment() {
			if(Item.GlyphVerticalAlignment != ItemVerticalAlignment.Default)
				return Item.GlyphVerticalAlignment;
			if(Item.Control.GlyphVerticalAlignment != ItemVerticalAlignment.Default)
				return Item.Control.GlyphVerticalAlignment;
			return ItemVerticalAlignment.Center;
		}
		protected virtual ItemVerticalAlignment GetCaptionVerticalAlignment() {
			if(Item.CaptionVerticalAlignment != ItemVerticalAlignment.Default)
				return Item.CaptionVerticalAlignment;
			return Item.Control.CaptionVerticalAlignment;
		}
		protected virtual Size CalcTextSize() {
			if(Item.AllowHtmlString)
				return CalcHtmlTextSize();
			return CalcSimpleTextSize();
		}
		protected virtual Size CalcSimpleTextSize() {
			return PaintAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, Item.Caption, 0).ToSize();
		}
		protected virtual Size CalcHtmlTextSize() {
			this.captionInfo = StringPainter.Default.Calculate(ViewInfo.GInfo.Graphics, PaintAppearance, Item.Caption, 0);
			return captionInfo.Bounds.Size;
		}
		protected virtual Size CalcGlyphSize() {
			Image img = GetGlyph();
			return img == null ? Size.Empty : img.Size;
		}
		protected internal virtual Image GetGlyph() {
			ObjectState state = CalcItemState();
			Image normalStateImage = GetGlyph(Item.Glyph, Control.Images, Item.ImageIndex);
			Image res = null;
			switch(state) { 
				case ObjectState.Normal:
					return normalStateImage;
				case ObjectState.Hot:
					res = GetGlyph(Item.GlyphHover, Control.Images, Item.ImageIndexHover);
					return res != null ? res : normalStateImage;
				case ObjectState.Pressed:
					res = GetGlyph(Item.GlyphPressed, Control.Images, Item.ImageIndexPressed);
					return res != null ? res : normalStateImage;
				case ObjectState.Disabled:
					res = GetGlyph(Item.GlyphDisabled, Control.Images, Item.ImageIndexDisabled);
					return res != null ? res : normalStateImage;
			}
			return null;
		}
		protected virtual Image GetGlyph(Image image, object images, int imageIndex) {
			if(image != null)
				return image;
			return ImageCollection.GetImageListImage(images, imageIndex);
		}
		public Rectangle ContentBounds { get { return contentBounds; } }
		public Rectangle GlyphBounds { get { return glyphBounds; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		protected override ObjectState CalcItemState() {
			if(!Item.Enabled)
				return ObjectState.Disabled;
			if(!ViewInfo.Control.DesignModeCore) {
				if(ViewInfo.PressedItem == this)
					return ObjectState.Pressed;
				if(ViewInfo.SelectedItem == this || ViewInfo.HotItem == this)
					return ObjectState.Hot;
			}
			return ObjectState.Normal;
		}
	}
	public class BackstageViewButtonItemViewInfo : BackstageViewItemViewInfo {
		public BackstageViewButtonItemViewInfo(BackstageViewButtonItem item) : base(item) { }
		public new BackstageViewButtonItem Item { get { return (BackstageViewButtonItem)base.Item; } }
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinBackstageViewControlButton], Bounds);
			if(ViewInfo.IsColored) {
				info.ImageIndex = RibbonPainter.ImageIndexByColorScheme(ViewInfo.Control.ColorScheme, CalcItemState());
			}
			else {
				info.ImageIndex = -1;
				info.State = CalcItemState();
			}
			info.RightToLeft = Control.IsRightToLeft;
			return info;
		}
		public override int Indent {
			get {
				if(Control.GetPaintStyle() != BackstageViewPaintStyle.Flat) {
					object obj = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptBackstageViewButtonIndent];
					if(obj != null)
						return (int)obj;
				}
				return base.Indent;
			}
		}
		protected override AppearanceObject GetAppearanceByState(ObjectState state) {
			switch(state) { 
				case ObjectState.Hot:
					return Control.GetController().AppearancesBackstageView.ButtonHover;
				case ObjectState.Pressed:
					return Control.GetController().AppearancesBackstageView.ButtonPressed;
				case ObjectState.Disabled:
					return Control.GetController().AppearancesBackstageView.ButtonDisabled;
			}
			return Control.GetController().AppearancesBackstageView.Button;
		}
		protected override AppearanceObject GetItemAppearanceByState(ObjectState state) {
			if(state == ObjectState.Pressed)
				return Item.AppearancePressed;
			return base.GetItemAppearanceByState(state);
		}
	}
	public class BackstageViewTabItemViewInfo : BackstageViewItemViewInfo {
		Rectangle arrowBounds;
		internal static int TextToArrowIndent = 12;
		public BackstageViewTabItemViewInfo(BackstageViewTabItem item) : base(item) { }
		public new BackstageViewTabItem Item { get { return (BackstageViewTabItem)base.Item; } }
		public override BackstageViewItemBasePainter Painter {
			get {
				return new BackstageViewTabItemPainter();
			}
		}
		protected override ObjectState CalcItemState() {
			if(!Item.Enabled)
				return ObjectState.Disabled;
			if(!ViewInfo.Control.DesignModeCore && (ViewInfo.SelectedItem == this || Control.SelectedTab == Item))
				return ObjectState.Pressed;
			return base.CalcItemState();
		}
		protected internal override void SetAppearanceDirty() {
			base.SetAppearanceDirty();
			if(Item.HasClientBackstageView) {
				foreach(BackstageViewControl bsv in Item.GetClientBackstageViewControls()) {
					if(bsv.GetViewInfo() != null) bsv.GetViewInfo().SetAppearanceDirty();
				}
			}
		}
		public override Size CalcBestSize() {
			Size sz = base.CalcBestSize();
			SkinElementInfo arrowInfo = GetArrowInfo();
			this.arrowBounds.Size = ObjectPainter.CalcObjectMinBounds(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, arrowInfo).Size;
			sz.Width += TextToArrowIndent + ArrowBounds.Width;
			return sz;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			if(Control.IsRightToLeft)
				this.arrowBounds.X = bounds.X;
			else this.arrowBounds.X = bounds.Right - ArrowBounds.Width;
			this.arrowBounds.Y = bounds.Y + (BestSize.Height - ArrowBounds.Height) / 2;
		}
		public Rectangle ArrowBounds { get { return arrowBounds; } }
		public override SkinElementInfo GetItemInfo() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinBackstageViewControlTab], Bounds);
			if(ViewInfo.IsColored) {
				info.ImageIndex = RibbonPainter.ImageIndexByColorScheme(ViewInfo.Control.ColorScheme, CalcItemState());
			}
			else {
				info.ImageIndex = -1;
				info.State = CalcItemState();
			}
			info.RightToLeft = Control.IsRightToLeft;
			return info;
		}
		public virtual SkinElementInfo GetArrowInfo() {
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinBackstageViewControlTabArrow], ArrowBounds);
			res.RightToLeft = Control.IsRightToLeft;
			return res;
		}
		protected override AppearanceObject GetAppearanceByState(ObjectState state) {
			switch(state) {
				case ObjectState.Hot:
					return Control.GetController().AppearancesBackstageView.TabHover;
				case ObjectState.Pressed:
					return Control.GetController().AppearancesBackstageView.TabSelected;
				case ObjectState.Disabled:
					return Control.GetController().AppearancesBackstageView.TabDisabled;
			}
			return Control.GetController().AppearancesBackstageView.Tab;
		}
		protected override AppearanceObject GetItemAppearanceByState(ObjectState state) {
			if(state == ObjectState.Pressed)
				return Item.AppearanceSelected;
			return base.GetItemAppearanceByState(state);
		}
	}
}
