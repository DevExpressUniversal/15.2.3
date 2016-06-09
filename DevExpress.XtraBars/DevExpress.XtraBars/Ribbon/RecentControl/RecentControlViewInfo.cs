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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentControlViewInfo : BaseStyleControlViewInfo {
		AppearanceObject paintAppearanceTitleCaption;
		int titleTextHeight;
		int splitterPos;
		BaseDesignTimeManager designTimeManager;
		bool isAnimation;
		bool isVScrollVisible;
		RecentItemViewInfoBase dragItem;
		RecentItemViewInfoBase dropItem;
		ItemLocation dropIndicatorLocation;
		public RecentControlViewInfo(RecentItemControl owner)
			: base(owner) {
			this.paintAppearanceTitleCaption = CreatePaintAppearance();
			this.splitterPos = -1;
		}
		public bool IsVScrollVisible { get { return isVScrollVisible && !isAnimation; } }
		protected internal int VScrollOffset { get { return RecentControl.ScrollerPosition; } }
		protected internal BaseDesignTimeManager DesignTimeManager { get { return designTimeManager; } }
		public RecentItemControl RecentControl { get { return OwnerControl as RecentItemControl; } }
		public bool ShowTitle { get { return RecentControl.ShowTitle; } }
		public bool ShowSplitter { get { return RecentControl.ShowSplitter; } }
		AppearanceDefault defaultAppearanceTitleCaption = null;
		public virtual AppearanceDefault DefaultAppearanceTitleCaption {
			get {
				if(defaultAppearanceTitleCaption == null) defaultAppearanceTitleCaption = CreateDefaultTitleCaptionAppearance();
				return defaultAppearanceTitleCaption;
			}
		}
		protected AppearanceDefault CreateDefaultTitleCaptionAppearance() {
			AppearanceDefault appearance = new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window), GetDefaultFont());
			SkinElement element = GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryGroupCaption);
			if(element != null) {
				element.ApplyForeColorAndFont(appearance);
			}
			appearance.Font = new Font("Segoe UI", 20F);
			appearance.FontStyleDelta = FontStyle.Regular;
			return appearance;
		}
		protected internal SkinElement GetRibbonSkinElement(string elementName) {
			return RibbonSkins.GetSkin(LookAndFeel)[elementName];
		}
		public virtual AppearanceObject PaintAppearanceTitleCaption {
			get { return paintAppearanceTitleCaption; }
			set {
				paintAppearanceTitleCaption = value;
				OnPaintAppearanceChanged();
			}
		}
		public void SetAppearanceDirty() {
			RecentControl.MainPanel.ViewInfo.SetAppearanceDirty();
			RecentControl.ContentPanel.ViewInfo.SetAppearanceDirty();
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			AppearanceHelper.Combine(PaintAppearanceTitleCaption, new AppearanceObject[] { (RecentControl.Appearances as RecentAppearanceCollection).Title, Appearance, StyleController != null ? StyleController.Appearance : null }, DefaultAppearanceTitleCaption);
			this.paintAppearanceTitleCaption.TextOptions.RightToLeft = RightToLeft;
		}
		protected Padding TitlePadding {
			get{
				if(RecentControl.TitlePadding != Padding.Empty) return RecentControl.TitlePadding;
				return GetSkinTitlePadding();
			}
		}
		Padding GetSkinTitlePadding() {
			return new Padding(43, 19, 4, 30);
		}
		protected int SplitterPos {
			get {
				if(splitterPos == -1) splitterPos = RecentControl.SplitterPosition; 
				return splitterPos; 
			}
			set {
				int newVal = RecentControl.CheckSpliterPosition(value);
				if(SplitterPos == newVal) return;
				splitterPos = newVal;
				OnSplitterPositionChanged();
			}
		}
		void OnSplitterPositionChanged() {
			RecentControl.Refresh(); 
			RecentControl.RaiseSplitterPositionChanged();
		}
		int CheckSplitterPos(int newValue) {
			int val = newValue;
			if(newValue > Bounds.Right - GetPanelMinWidth(RecentControl.ContentPanel)) val = Bounds.Right - GetPanelMinWidth(RecentControl.ContentPanel);
			if(newValue < GetPanelMinWidth(RecentControl.MainPanel)) val = GetPanelMinWidth(RecentControl.MainPanel);
			return val;
		}
		public Rectangle TitleBounds { get; private set; }
		public Rectangle TitleCaptionBounds { get; private set; }
		public Rectangle MainPanelBounds { get; private set; }
		public Rectangle ContentPanelBounds { get; private set; }
		public Rectangle SplitterBounds { get; private set; }
		public Rectangle SplitterAreaBounds { get; private set; }
		public Rectangle PanelsAreaBounds { get; private set; }
		public RecentItemViewInfoBase DragItem { get { return dragItem; } }
		internal void SetDragItem(RecentItemViewInfoBase value) { this.dragItem = value; }
		public RecentItemViewInfoBase DropItem { get { return dropItem; } }
		internal void SetDropItem(RecentItemViewInfoBase value) { this.dropItem = value; }
		public ItemLocation DropIndicatorLocation { get { return dropIndicatorLocation; } }
		internal void SetDropIndicatorLocation(ItemLocation value) { dropIndicatorLocation = value; }
		protected override void CalcConstants() {
			base.CalcConstants();
			this.titleTextHeight = CalcTitleTextHeight();
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			this.designTimeManager = new BaseDesignTimeManager(this, RecentControl.Site);
			AdjustContent(bounds);
			TitleBounds = CalcTitleBounds();
			TitleCaptionBounds = CalcTitleCaptionBounds();
			PanelsAreaBounds = CalcPanelsAreaBounds();
			SplitterBounds = CalcSplitterBounds();
			SplitterAreaBounds = CalcSplitterAreaBounds();
			MainPanelBounds = CalcMainPanelBounds();
			ContentPanelBounds = CalcContentPanelBounds();
			this.isVScrollVisible = CalcScrollBarVisibility();
			if(isVScrollVisible) UpdateViewPanelBounds();
		}
		void UpdateViewPanelBounds() {
			Rectangle rect = ContentPanelBounds;
			rect.Width -= RecentControl.ScrollController.VScrollWidth;
			ContentPanelBounds = rect;
			RecentControl.ContentPanel.ViewInfo.CalcViewInfo(rect);
		}
		protected Rectangle CalcPanelsAreaBounds() {
			return new Rectangle(ContentRect.X, Math.Max(TitleBounds.Bottom, ContentRect.Y), ContentRect.Width, ContentRect.Height - TitleBounds.Height);
		}
		protected bool CalcScrollBarVisibility() {
			return CalcPanelScrollBarVisibility(RecentControl.MainPanel) || CalcPanelScrollBarVisibility(RecentControl.ContentPanel);
		}
		bool CalcPanelScrollBarVisibility(RecentPanelBase panel) {
			return CalcPanelScrollMax(panel) > MainPanelBounds.Height;
		}
		Rectangle CalcMainPanelBounds() {
			Rectangle rect = new Rectangle(PanelsAreaBounds.X, PanelsAreaBounds.Y, SplitterPos - GetSplitterWidth() / 2 - 1, PanelsAreaBounds.Height);
			RecentControl.MainPanel.ViewInfo.CalcViewInfo(rect);
			return rect;
		}
		Rectangle CalcContentPanelBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.X = ShowSplitter ? SplitterBounds.Right : MainPanelBounds.Right;
			rect.Y = PanelsAreaBounds.Y;
			rect.Width = PanelsAreaBounds.Right - MainPanelBounds.Right;
			if(ShowSplitter) rect.Width -= SplitterBounds.Width;
			rect.Height = PanelsAreaBounds.Height;
			RecentControl.ContentPanel.ViewInfo.CalcViewInfo(rect);
			return rect;
		}
		Rectangle CalcSplitterBounds() {
			if(!ShowSplitter) return Rectangle.Empty;
			return new Rectangle(SplitterPos - GetSplitterWidth() / 2, Math.Max(TitleBounds.Bottom, ContentRect.Y), GetSplitterWidth(), ContentRect.Height - TitleBounds.Height);
		}
		Rectangle CalcSplitterAreaBounds() {
			if(SplitterBounds == Rectangle.Empty) return Rectangle.Empty;
			return new Rectangle(SplitterBounds.X - 2, SplitterBounds.Y, SplitterBounds.Width + 4, SplitterBounds.Height);
		}
		private int GetSplitterWidth() {
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetSplitterInfo()).Width;
		}
		protected internal SkinElementInfo GetSplitterInfo() {
			SkinElement elem = CommonSkins.GetSkin(RecentControl.LookAndFeel.ActiveLookAndFeel)["LabelLineVert"];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)["LabelLineVert"];
			return new SkinElementInfo(elem, SplitterBounds);
		}
		protected Rectangle CalcTitleCaptionBounds() {
			Rectangle captBounds = new Rectangle();
			captBounds.X = TitleBounds.X + TitlePadding.Left;
			captBounds.Y = TitleBounds.Y + TitlePadding.Top;
			captBounds.Size = PaintAppearanceTitleCaption.CalcTextSizeInt(GInfo.Graphics, RecentControl.Title, ContentRect.Width - TitlePadding.Horizontal);
			return captBounds;
		}
		protected Rectangle CalcTitleBounds() {
			if(!RecentControl.ShowTitle) return Rectangle.Empty;
			return new Rectangle(ContentRect.X, ContentRect.Y, ContentRect.Width, CalcTitleTextHeight() + TitlePadding.Vertical);
		}
		protected int CalcTitleTextHeight() {
			return PaintAppearanceTitleCaption.CalcTextSizeInt(GInfo.Graphics, "Wg", 0).Height;
		}
		protected virtual void AdjustContent(Rectangle bounds) {
			Padding padding = OwnerControl.Padding;
			this.fContentRect.X += padding.Left;
			this.fContentRect.Width -= (padding.Left + padding.Right);
			this.fContentRect.Y += padding.Top;
			this.fContentRect.Height -= (padding.Top + padding.Bottom);
		}
		internal RecentControlHitInfo CalcHitInfo(Point point) {
			RecentControlHitInfo hitInfo = new RecentControlHitInfo();
			hitInfo.HitPoint = point;
			if(hitInfo.ContainsSet(TitleBounds, RecentControlHitTest.Title)) return hitInfo;
			if(hitInfo.ContainsSet(SplitterAreaBounds, RecentControlHitTest.Splitter)) return hitInfo;
			if(hitInfo.ContainsSet(MainPanelBounds, RecentControlHitTest.MainPanel)) {
				hitInfo.Panel = RecentControl.MainPanel;
				return RecentControl.MainPanel.ViewInfo.CalcHitInfo(hitInfo);
			}
			if(hitInfo.ContainsSet(ContentPanelBounds, RecentControlHitTest.ContentPanel)) {
				hitInfo.Panel = RecentControl.ContentPanel;
				return RecentControl.ContentPanel.ViewInfo.CalcHitInfo(hitInfo);
			}
			hitInfo.SetHitTest(RecentControlHitTest.None);
			return hitInfo;
		}
		internal void UpdateSplitterPosition(int loc) {
			SplitterPos = LocToPos(loc);
		}
		int LocToPos(int loc) {
			return loc;
		}
		int GetPanelMinWidth(RecentPanelBase panel) {
			return panel.ViewInfo.CalcMinWidth();
		}
		#region Animation
		internal void AddTransitionAnimation() {
			ClearTrnsitionAnimation();
			RecentTransitionAnimationInfo info;
			int start = RecentControl.ContentPanel.ViewInfo.PanelContentRect.Left + AnimationLength;
			int end = RecentControl.ContentPanel.ViewInfo.PanelContentRect.Left;
			info = new RecentTransitionAnimationInfo(RecentControl, ContentPanelBounds.Left + AnimationLength, ContentPanelBounds.Left, AnimationLength, RecentControl as IXtraAnimationListener);
			XtraAnimator.Current.AddAnimation(info);
		}
		void ClearTrnsitionAnimation() {
			if(XtraAnimator.Current.Get(RecentControl, RecentControl.AnimationId) == null)
				return;
			XtraAnimator.Current.Animations.Remove(RecentControl, RecentControl.AnimationId);
		}
		public int AnimationLength { get { return 50; } }
		protected internal bool IsAnimation { get { return isAnimation; } }
		internal void OnAnimation() {
			this.isAnimation = true;
		}
		internal void OnEndAnimation() {
			this.isAnimation = false;
		}
		#endregion
		#region scrollers
		internal ScrollArgs CalcVScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Minimum = 0;
			args.Maximum = CalcScrollMax();
			args.LargeChange = MainPanelBounds.Height; 
			args.SmallChange = 13;
			args.Value = RecentControl.ScrollerPosition;
			return args;
		}
		int CalcScrollMax() { 
			return Math.Max(CalcPanelScrollMax(RecentControl.MainPanel), CalcPanelScrollMax(RecentControl.ContentPanel));
		}
		int CalcPanelScrollMax(RecentPanelBase panel) {
			if(panel.Items.Count == 0) return 0;
			return panel.Items[panel.Items.Count-1].ViewInfo.Bounds.Bottom - Math.Min(panel.ViewInfo.CaptionBounds.Top, panel.Items[0].ViewInfo.Bounds.Top) + panel.PanelPadding.Vertical;
		}
		protected internal int CalcVSmallChange() {
			return RecentControl.ScrollController.VScroll.SmallChange;
		}
		#endregion
		internal ToolTipControlInfo GetToolTipInfo(Point point) {
			RecentItemBase item = CalcHitInfo(point).Item;
			if(item == null) return null;
			ToolTipControlInfo info = new ToolTipControlInfo();
			info.SuperTip = item.SuperTip;
			info.Object = item;
			info.ToolTipType = ToolTipType.SuperTip;
			return info;
		}
		internal void ResetSplitterPos() {
			splitterPos = -1;
		}
	}
	public class RecentTransitionAnimationInfo : BaseAnimationInfo {
		int startPos, endPos;
		IXtraAnimationListener[] listeners;
		RecentItemControl recentControl;
		SplineAnimationHelper splineHelper;
		public RecentTransitionAnimationInfo(RecentItemControl recentControl, int startPos, int endPos, int lenght, params IXtraAnimationListener[] listeners)
			: base(recentControl, recentControl.AnimationId, 5, (int)(TimeSpan.TicksPerMillisecond * lenght / 1)) {
			this.splineHelper = new SplineAnimationHelper();
			this.recentControl = recentControl;
			this.startPos = startPos;
			this.endPos = endPos;
			this.CurrentPos = startPos;
			this.listeners = listeners;
			Array.ForEach(Listeners, listener => listener.OnAnimation(this));
		}
		public int StartPos { get { return startPos; } }
		public int EndPos { get { return endPos; } }
		public int CurrentPos { get; private set; }
		public IXtraAnimationListener[] Listeners { get { return listeners; } }
		public RecentItemControl RecentControl { get { return recentControl; } }
		public override void FrameStep() {
			double k = ((double)(CurrentFrame)) / FrameCount;
			CurrentPos = (int)((EndPos - StartPos) * k + StartPos);
			if(IsFinalFrame) {
				CurrentPos = EndPos;
			}
			RecentControl.GetViewInfo().IsReady = false;
			RecentControl.Invalidate(InvalidatedRectangle);
			if(IsFinalFrame) {
				Array.ForEach(Listeners, listener => listener.OnEndAnimation(this));
			}
		}
		protected Rectangle InvalidatedRectangle {
			get {
				Rectangle bounds = RecentControl.GetViewInfo().ContentPanelBounds;
				return bounds;
			}
		}
	}
	public enum RecentControlHitTest { None, Title, Splitter, MainPanel, ContentPanel, Item, PinButton }
	public class RecentControlHitInfo {
		Point hitPoint = Point.Empty;
		RecentControlHitTest hitTest = RecentControlHitTest.None;
		RecentItemBase item;
		RecentPanelBase panel;
		public RecentControlHitInfo() { }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public RecentControlHitTest HitTest { get { return hitTest; } }
		public RecentItemBase Item {
			get { return item; }
			set {
				item = value;
			}
		}
		public RecentPanelBase Panel { get { return panel; } set { panel = value; } }
		public bool InItem {
			get {
				return HitTest == RecentControlHitTest.Item || HitTest == RecentControlHitTest.PinButton;
			}
		}
		public bool InPanel {
			get {
				return HitTest == RecentControlHitTest.MainPanel || HitTest == RecentControlHitTest.ContentPanel || InItem;
			}
		}
		internal bool ContainsSet(Rectangle bounds, RecentControlHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		internal void SetHitTest(RecentControlHitTest hitTest) {
			this.hitTest = hitTest;
		}
	}
	public class RecentStrackPanelViewInfo : RecentPanelViewInfoBase {
		public RecentStrackPanelViewInfo(RecentStackPanel panel) : base(panel) { }
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			CalcItemsViewInfo();
		}
		protected void CalcItemsViewInfo() {
			int top = ContentBounds.Y - Panel.RecentControl.GetViewInfo().VScrollOffset;
			foreach(RecentItemBase item in Panel.Items) {
				if(!item.Visible) continue;
				item.ViewInfo.CalcViewInfo(CalcItemBestRect(item, top));
				top += item.ViewInfo.Bounds.Height + ItemsIndent + item.Margin.Vertical;
			}
		}
		Rectangle CalcItemBestRect(RecentItemBase item, int top) {
			Rectangle bestRect = new Rectangle(ContentBounds.X + item.Margin.Left, top + item.Margin.Top, ContentBounds.Width - item.Margin.Horizontal - 1, item.ViewInfo.CalcBestSize(ContentBounds.Width - item.Margin.Horizontal - 1).Height);
			RecentControlContainerItem container = item as RecentControlContainerItem;
			if(container != null && container.FillSpace) {
				int itemHeight = ContentBounds.Height - CalcItemsHeight();
				bestRect.Height = itemHeight > 0 ? itemHeight : 0;
			}
			return bestRect;
		}
		int CalcItemsHeight() {
			int height = 0;
			foreach(RecentItemBase item in Panel.Items) {
				if(item is RecentControlContainerItem) continue;
				if(!item.Visible) continue;
				height += item.ViewInfo.CalcBestSize(ContentBounds.Width - item.Margin.Horizontal).Height + ItemsIndent + item.Margin.Vertical;
			}
			return height;
		}
	}
	public class RecentTablePanelViewInfo : RecentPanelViewInfoBase {
		public RecentTablePanelViewInfo(RecentTablePanel panel) : base(panel) { }
		public RecentTablePanel TablePanel { get { return Panel as RecentTablePanel; } }
	}
}
