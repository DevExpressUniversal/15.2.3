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
using DevExpress.Utils.Menu;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class TileBarViewInfo : TileControlViewInfo {
		public TileBarViewInfo(ITileControl control) : base(control) { }
		public TileBar OwnerCore { get { return Owner as TileBar; } }
		protected override int ClipBoundsTopIndent { get { return 0; } }
		protected override int TileHoverAnimationLength { get { return 1; } }
		protected override int SelectionWidth { get { return Math.Max(OwnerCore.SelectionBorderWidth, 1); } }
		public override int GroupTextToItemsIndent { get { return Owner.Properties.ShowGroupText ? OwnerCore.GroupTextToItemsIndent : 0; } }
		protected internal SkinElement GetSkinElement(string elementName) {
			SkinElement elem = MetroUISkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[elementName];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DefaultSkinProvider.Default)[elementName];
			return elem;
		}
		protected override AppearanceDefault DefaultAppearanceGroupText {
			get {
				if(OwnerCore == null) return base.DefaultAppearanceGroupText;
				return InvertedGroupTextAppearance ? DefaultAppearanceGroupTextInverted : DefaultAppearanceGroupTextCore;
			}
		}
		bool InvertedGroupTextAppearance { get { return OwnerCore.InvertedGroupTextAppearance; } }
		AppearanceDefault DefaultAppearanceGroupTextCore {
			get {
				AppearanceDefault res = new AppearanceDefault(GetGroupTextForeColor(false), Color.Empty, Color.Empty, Color.Empty);
				res.FontSizeDelta = 1;
				return res;
			}
		}
		AppearanceDefault DefaultAppearanceGroupTextInverted {
			get {
				AppearanceDefault res = new AppearanceDefault(GetGroupTextForeColor(true), Color.Empty, Color.Empty, Color.Empty);
				res.FontSizeDelta = 1;
				return res;
			}
		}
		Color GetGroupTextForeColor(bool inverted) {
			if(OwnerCore == null) return Color.Empty;
			if(inverted)
				return OwnerCore.GetItemForeColor();
			return OwnerCore.GetItemBackColor();
		}
		protected override void PressItem(TileControlHitInfo newInfo) {
			TileBarHitInfo hitInfo = newInfo as TileBarHitInfo;
			TileBarItemViewInfo itemInfo = hitInfo.ItemInfo as TileBarItemViewInfo;
			if(hitInfo == null || itemInfo == null) return;
			if(hitInfo.InDropDown) {
			}
			else
				hitInfo.ItemInfo.Item.OnItemPress();
			hitInfo.ItemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(itemInfo.Bounds);
		}
		protected internal void ProcessDropDownPress(TileBarItemViewInfo itemInfo) {
			if(itemInfo.DropDownInPressedState) {
				CloseDropDown();
			}
			else {
				ShowDropDownWindow(itemInfo);
			}
		}
		public void ShowDropDownWindow(TileBarItemViewInfo itemInfo) {
			if(IsItemDropDownAlreadyOpened(itemInfo)) return;
			CloseDropDown();
			ShowDropDownWindowCore(itemInfo);
			UpdateBeakPosition(itemInfo);
		}
		bool IsItemDropDownAlreadyOpened(TileBarItemViewInfo itemInfo) {
			if(DropDownWindow == null) return false;
			var owner = DropDownWindow.DropDownOwner;
			if(owner != null && owner == itemInfo) return true;
			return false;
		}
		void ShowDropDownWindowCore(TileBarItemViewInfo itemInfo) {
			TileBarDropDownShowingEventArgs e = OwnerCore.RaiseDropDownShowing(itemInfo.ItemCore, itemInfo.ItemCore.DropDownControl);
			if(e.Cancel) 
				return;
			itemInfo.ItemCore.DropDownControl = e.DropDownContainer;
			if(itemInfo.ItemCore.DropDownControl == null && itemInfo.ItemCore.TileBar == null) 
				return;
			if(itemInfo.DropDownWindow.TryToShowToolWindow()) {
				itemInfo.DropDownInPressedState = true;
				DropDownWindow = itemInfo.DropDownWindow;
				if(OwnerCore.ParentPopup != null)
					OwnerCore.ParentPopup.ChildWindow = DropDownWindow;
				EnsureChildWindow(itemInfo.ItemCore);
				OnTileNavPaneDrowDownShown(itemInfo.DropDownWindow);
			}
		}
		void OnTileNavPaneDrowDownShown(TileBarWindow window) {
			if(window == null || window.DropDownOwner == null || !window.DropDownOwner.IsTileNavPane) return;
			var item = window.DropDownOwner as TileNavElement;
			if(item != null && item.TileNavPane != null)
				item.TileNavPane.OnDropDownShown(item);
		}
		void EnsureChildWindow(TileBarItem item) {
			if(item.TileBar == null || item.TileBar.ParentPopup == null) return;
			item.TileBar.ParentPopup.ChildWindow = DropDownWindow;
		}
		void UpdateBeakPosition(TileBarItemViewInfo itemInfo) {
			Point pt;
			if(!itemInfo.ShouldDrawDropDown) {
				HideBeak();
				return;
			}
			if(OwnerCore.Orientation == Orientation.Horizontal)
				pt = new Point(itemInfo.DropDownBounds.X + (int)(itemInfo.DropDownBounds.Width / 2), 0);
			else
				pt = new Point(0, itemInfo.DropDownBounds.Y + (int)(itemInfo.DropDownBounds.Height / 2));
			Beak = new BeakInfo() { Position = pt, ItemInfo = itemInfo };
			Owner.Invalidate(BeakBounds);
		}
		void HideBeak() {
			Beak = new BeakInfo() { Position = Point.Empty, ItemInfo = null };
			Owner.Invalidate(BeakBounds);
		}
		protected override void UnPressItem(TileControlHitInfo oldInfo) {
			oldInfo.ItemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(oldInfo.ItemInfo.Bounds);
		}
		public override AppearanceObject GetAppearance(TileItemViewInfo item) {
			if(item.IsPressed)
				return Owner.AppearanceItem.Pressed;
			if(item.IsHovered)
				return Owner.AppearanceItem.Hovered;
			if(item.IsSelected)
				return Owner.AppearanceItem.Selected;
			return Owner.AppearanceItem.Normal;
		}
		protected internal AppearanceObject GetDropDownAppearance(TileBarItemViewInfo item) {
			if(item.DropDownPressed || item.DropDownInPressedState) 
				return Owner.AppearanceItem.Pressed;
			if(item.DropDownHovered) 
				return Owner.AppearanceItem.Hovered;
			if(item.IsSelected)
				return Owner.AppearanceItem.Selected;
			return Owner.AppearanceItem.Normal;
		}
		protected override TileControlHitInfo CreateHitInfo() {
			return new TileBarHitInfo();
		}
		protected override void OnHoverInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
			TileBarHitInfo newHit = newInfo as TileBarHitInfo;
			TileBarHitInfo oldHit = oldInfo as TileBarHitInfo;
			if(oldHit != null && oldHit.InItem && oldHit.ItemInfo != null)
				UpdateAndRepaintItem(oldHit.ItemInfo);
			if(newHit != null && newHit.InItem && newHit.ItemInfo != null)
				UpdateAndRepaintItem(newHit.ItemInfo);
		}
		void UpdateAndRepaintItem(TileItemViewInfo itemInfo) {
			itemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(itemInfo.Bounds);
		}
		protected internal Color GetDropDownWindowBackColor(TileBarItemViewInfo itemInfo) {
			if(itemInfo == null) return Color.Empty;
			if(itemInfo.ItemCore.DropDownOptions.BeakColor != Color.Empty) return itemInfo.ItemCore.DropDownOptions.BeakColor;
			if(OwnerCore.DropDownOptions.BeakColor != Color.Empty) return OwnerCore.DropDownOptions.BeakColor;
			if(itemInfo.DropDownContent != null) return itemInfo.DropDownContent.BackColor;
			return OwnerCore.DropDownOptions.BeakColor;
		}
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new TileBarLayoutCalculator(viewInfo);
		}
		protected internal bool BackArrowVisible {
			get { return Offset != 0; }
		}
		protected internal bool ForwardArrowVisible {
			get { return Offset != MaxOffset; }
		}
		const float scrollButtonsHoverOpacity = 0.3f;
		const int scrollButtonsWidth = 24;
		protected override Rectangle CalcBackArrowBounds(Graphics g) {
			var r = new Rectangle(ClientBounds.Location, GetScrollButtonSize());
			if(Owner.Properties.ShowText)
				r.Y = GroupsClipBounds.Y;
			return r;
		}
		protected override Rectangle CalcForwardArrowBounds(Graphics g) {
			var r = new Rectangle(Point.Empty, GetScrollButtonSize());
			if(OwnerCore.Orientation == Orientation.Horizontal) {
				r.X = ClientBounds.Right - scrollButtonsWidth;
				r.Y = Owner.Properties.ShowText ? GroupsClipBounds.Y : ClientBounds.Y;
			}
			else {
				r.X = ClientBounds.X;
				r.Y = ClientBounds.Bottom - scrollButtonsWidth;
			}
			return r;
		}
		protected virtual Size GetScrollButtonSize() {
			Size s = Size.Empty;
			if(Owner.Properties.Orientation == Orientation.Horizontal) {
				int h = Owner.Properties.ShowText ? ClientBounds.Height - GroupsClipBounds.Y : ClientBounds.Height;
				s = new Size(scrollButtonsWidth, h);
			}
			else {
				s = new Size(ClientBounds.Width, scrollButtonsWidth);
			}
			return s;
		}
		protected internal float BackArrowHoverOpacity { get; set; }
		protected internal float ForwardArrowHoverOpacity { get; set; }
		protected internal Color GetScrollArrowHoverColor(float opacity) {
			return Color.FromArgb((int)(0.75f * 255.0f * opacity), 255, 255, 255);
		}
		Image RotateByOrientation(Image input) {
			if(OwnerCore.Orientation == Orientation.Horizontal)
				return input;
			else {
				Image rotated = input.Clone() as Image;
				rotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
				return rotated;
			}
		}
		Image scrollBackGlyph;
		protected internal Image ScrollBackGlyph {
			get {
				if(scrollBackGlyph == null)
					scrollBackGlyph = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileBar.Images.scrollarrowB.png", typeof(TileBar).Assembly);
				return RotateByOrientation(scrollBackGlyph);
			}
		}
		protected internal Point ScrollBackGlyphPosition {
			get {
				int x = BackArrowBounds.X;
				int y = BackArrowBounds.Y;
				x = x + (int)(BackArrowBounds.Width / 2) - (int)(ScrollBackGlyph.Width / 2);
				y = y + (int)(BackArrowBounds.Height / 2) - (int)(ScrollBackGlyph.Height / 2);
				return new Point(x, y);
			}
		}
		Image scrollForwardGlyph;
		protected internal Image ScrollForwardGlyph {
			get {
				if(scrollForwardGlyph == null)
					scrollForwardGlyph = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileBar.Images.scrollarrowF.png", typeof(TileBar).Assembly);
				return RotateByOrientation(scrollForwardGlyph);
			}
		}
		protected internal Point ScrollForwardGlyphPosition {
			get {
				int x = ForwardArrowBounds.X;
				int y = ForwardArrowBounds.Y;
				x = x + (int)(ForwardArrowBounds.Width / 2) - (int)(ScrollForwardGlyph.Width / 2);
				y = y + (int)(ForwardArrowBounds.Height / 2) - (int)(ScrollForwardGlyph.Height / 2);
				return new Point(x, y);
			}
		}
		protected override void OnPressedInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
			if(oldInfo.InItem) {
				UnPressItem(oldInfo);
			}
			else if(oldInfo.InBackArrow || oldInfo.InForwardArrow) {
				StopScroll();
			}
			if(newInfo.InItem) {
				PressItem(newInfo);
			}
			else if(newInfo.InBackArrow) {
				OnBackArrowPress();
			}
			else if(newInfo.InForwardArrow) {
				OnForwardArrowPress();
			}
		}
		void OnBackArrowPress() {
			if(Owner.IsDesignMode)
				OnBackScroll();
			else
				(Owner.Navigator as TileBarNavigator).MovePageNoItemFocus(true);
		}
		void OnForwardArrowPress() {
			if(Owner.IsDesignMode)
				OnForwardScroll();
			else
				(Owner.Navigator as TileBarNavigator).MovePageNoItemFocus(false);
		}
		protected override bool CheckBackArrowHit(TileControlHitInfo hitInfo) {
			if(BackArrowVisible && BackArrowBounds.Contains(hitInfo.HitPoint)) {
				hitInfo.ContainsSet(BackArrowBounds, TileControlHitTest.BackArrow);
				return true;
			}
			return false;
		}
		protected override bool CheckForwardArrowHit(TileControlHitInfo hitInfo) {
			if(ForwardArrowVisible && ForwardArrowBounds.Contains(hitInfo.HitPoint)) {
				hitInfo.ContainsSet(ForwardArrowBounds, TileControlHitTest.ForwardArrow);
				return true;
			}
			return false;
		}
		public virtual bool CanDrawScrollButtons { get { return base.ScrollMode == TileControlScrollMode.ScrollButtons; } }
		protected override void OnOffsetChanged(int prevOffset) {
			MakeBeakOffset(prevOffset);
			base.OnOffsetChanged(prevOffset);
		}
		void MakeBeakOffset(int prevOffset) {
			if(!ShouldDrawBeak || prevOffset == 0) return;
			int delta = Offset - prevOffset;
			var side = GetDropDownSide();
			if(side == DropDownSide.Top || side == DropDownSide.Bottom)
				Beak.Position = new Point(Beak.Position.X - delta, 0);
			else
				Beak.Position = new Point(0, Beak.Position.Y - delta);
		}
		protected internal TileBarWindow DropDownWindow { get; set; }
		protected internal void CloseDropDown() {
			if(DropDownWindow == null)
				return;
			if(DropDownWindow.HideDropDown())
				DropDownWindow = null;
			Owner.Invalidate(BeakBounds);
		}
		protected internal bool ShouldDrawBeak {
			get { return DropDownWindow != null && Beak != null; } 
		}
		public BeakInfo Beak { get; set; }
		protected internal Rectangle BeakBounds {
			get {
				if(Beak == null) return Rectangle.Empty;
				return GetBeakBounds(Beak.Position); }
		}
		Rectangle GetBeakBounds(Point beakPosition) {
			var side = GetDropDownSide();
			switch(side) { 
				case DropDownSide.Top:
					return new Rectangle(beakPosition.X - 7, Owner.ClientRectangle.Top, 15, 8);
				case DropDownSide.Bottom:
					return new Rectangle(beakPosition.X - 7, Owner.ClientRectangle.Bottom - 8, 15, 8);
				case DropDownSide.Left:
					return new Rectangle(Owner.ClientRectangle.Left, beakPosition.Y - 8, 8, 15); 
				default:
					return new Rectangle(Owner.ClientRectangle.Right - 8, beakPosition.Y - 8, 8, 15); 
			}
		}
		protected internal Point[] GetBeakPoints() {
			Point[] result = new Point[3];
			var side = GetDropDownSide();
			switch(side) {
				case DropDownSide.Top:
					result[0] = new Point(Beak.Position.X - 7, Owner.ClientRectangle.Top);
					result[1] = new Point(Beak.Position.X + 7, Owner.ClientRectangle.Top);
					result[2] = new Point(Beak.Position.X, Owner.ClientRectangle.Top + 8);
					break;
				case DropDownSide.Bottom:
					result[0] = new Point(Beak.Position.X - 7, Owner.ClientRectangle.Bottom);
					result[1] = new Point(Beak.Position.X, Owner.ClientRectangle.Bottom - 8);
					result[2] = new Point(Beak.Position.X + 7, Owner.ClientRectangle.Bottom);
					break;
				case DropDownSide.Left:
					result[0] = new Point(Owner.ClientRectangle.Left, Beak.Position.Y - 7);
					result[1] = new Point(Owner.ClientRectangle.Left + 8, Beak.Position.Y);
					result[2] = new Point(Owner.ClientRectangle.Left, Beak.Position.Y + 7);
					break;
				case DropDownSide.Right:
					result[0] = new Point(Owner.ClientRectangle.Right - 8, Beak.Position.Y);
					result[1] = new Point(Owner.ClientRectangle.Right, Beak.Position.Y - 7);
					result[2] = new Point(Owner.ClientRectangle.Right, Beak.Position.Y + 7);
					break;
			}
			return result;
		}
		protected internal enum DropDownSide { Left, Right, Top, Bottom }
		protected internal DropDownSide GetDropDownSide() {
			var sd = OwnerCore.DropDownShowDirection;
			if(OwnerCore.Orientation == Orientation.Horizontal) {
				if(OwnerCore.Dock == DockStyle.Bottom)
					return sd == ShowDirection.Normal ? DropDownSide.Top : DropDownSide.Bottom;
				else
					return sd == ShowDirection.Normal ? DropDownSide.Bottom : DropDownSide.Top;
			}
			if(OwnerCore.Dock == DockStyle.Right)
				return sd == ShowDirection.Normal ? DropDownSide.Left : DropDownSide.Right;
			else
				return sd == ShowDirection.Normal ? DropDownSide.Right : DropDownSide.Left;
		}
		public override HorzAlignment HorizontalContentAlignment {
			get {
				if(Owner.Properties.HorizontalContentAlignment == HorzAlignment.Default)
					return HorzAlignment.Near;
				return Owner.Properties.HorizontalContentAlignment;
			}
		}
		AppearanceObject shadowAppearance;
		protected internal AppearanceObject ShadowAppearance {
			get {
				if(shadowAppearance == null)
					shadowAppearance = CreateShadowAppearance();
				return shadowAppearance;
			}
		}
		protected virtual AppearanceObject CreateShadowAppearance() {
			AppearanceObject result = new AppearanceObject();
			result.BackColor = Color.FromArgb(35, Color.Black);
			result.BackColor2 = Color.FromArgb(0, Color.Black);
			result.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			return result;
		}
		public virtual void ResetDropDownAppearances() {
			foreach(TileBarGroupViewInfo g in Groups) {
				foreach(TileBarItemViewInfo i in g.Items) {
					i.ResetDropDownAppearance();
				}
			}
		}
		protected override int CalcGroupLocVertCenter(Rectangle groupsBounds) {
			int y = base.CalcGroupLocVertCenter(groupsBounds);
			y -= (GroupTextToItemsIndent / 2) + (Groups[0].TextBounds.Height / 2);
			return y;
		}
		public Color ScrollArrowsColor { get { return GetModifiedColor(OwnerCore.BackColor, -0.4f, 0.4f); } }
		internal static Color GetModifiedColor(Color source) {
			return GetModifiedColor(source, itemDarkerFactor, itemLighterFactor);
		}
		internal static Color GetModifiedColor(Color source, float darkerFactor, float lighterFactor) {
			if(ColorIsBright(source))
				return ChangeBrightness(source, darkerFactor);
			return ChangeBrightness(source, lighterFactor);
		}
		const float itemDarkerFactor = -0.10f;
		const float itemLighterFactor = 0.3f;
		protected internal static bool ColorIsBright(Color color) {
			float lightness = 0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B;
			return lightness > 128f;
		}
		internal static Color ChangeBrightness(Color color, float factor) { 
			float r = (float)color.R;
			float g = (float)color.G;
			float b = (float)color.B;
			if(factor > 0) {
				r = (255 - r) * factor + r;
				g = (255 - g) * factor + g;
				b = (255 - b) * factor + b;
			}
			else {
				factor = 1 + factor;
				r *= factor;
				g *= factor;
				b *= factor;
			}
			return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
		}
		protected override ITileControlDesignManager CreateDTManager() {
			return new TileBarDesignTimeManagerBase(OwnerCore.Site.Component, OwnerCore);
		}
		public bool CanDrawShadows { get { return OwnerCore.Handler.State != TileControlHandlerState.DragMode; } }
		protected override Rectangle GetVisualEffectsClientBounds() {
			return ClipRectByArrowButtons(ClientBounds);
		}
		Rectangle ClipRectByArrowButtons(Rectangle rect) {
			if(IsHorizontal) {
				int x = rect.X;
				int r = rect.Right;
				if(BackArrowVisible) x = CalcBackArrowBounds(null).Right;
				if(ForwardArrowVisible) r = CalcForwardArrowBounds(null).X;
				return new Rectangle(x, rect.Y, r - x, rect.Height);
			}
			else {
				int y = rect.Y;
				int b = rect.Bottom;
				if(BackArrowVisible) y = CalcBackArrowBounds(null).Bottom;
				if(ForwardArrowVisible) b = CalcForwardArrowBounds(null).Y;
				return new Rectangle(rect.X, y, rect.Width, b - y);
			}
		}
		new public int GetScaledHorz(int w) {
			return base.GetScaledHorz(w);
		}
		protected internal void ScrollToStartVertical() {
			int delta = 0;
			if(Groups.Count > 0) {
				delta = Groups[0].TotalBounds.Y - GroupsContentBounds.Y;
			}
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, this);
			XtraAnimator.Current.AddAnimation(new TileOffsetAnimationInfo(this, OffsetAnimationLength, Offset, Offset + delta));
		}
	}
	public class TileBarItemViewInfo : TileItemViewInfo, ITileBarWindowOwner {
		public TileBarItem ItemCore { get { return (TileBarItem)Item; } }
		protected TileBar ControlCore { get { return ControlInfo.Owner as TileBar; } }
		protected internal TileBarViewInfo ControlInfoCore { get { return ControlInfo as TileBarViewInfo; } }
		public TileBarItemViewInfo(TileItem item) : base(item) { }
		public override void ResetDefaultAppearance() {
			base.ResetDefaultAppearance();
			ResetDropDownAppearance();
		}
		public override Color GetSelectionColor() {
			if(ControlCore.SelectionColorMode == SelectionColorMode.UseItemBackColor)
				return ItemNormalAppearance.BackColor;
			return base.GetSelectionColor();
		}
		public override Rectangle SelectionBounds {
			get {
				int w = ControlCore.SelectionBorderWidth;
				int i = w % 2 == 0 ? 0 : 1;
				int doubleIndent = frameIndent * 2;
				int halfW = w / 2;
				return new Rectangle(Bounds.X - halfW - i - frameIndent, Bounds.Y - halfW - i - frameIndent,
					Bounds.Width + w + 1 + doubleIndent, Bounds.Height + w + 1 + doubleIndent);
			}
		}
		const int frameIndent = 2;
		public bool ShouldShowItemShadow {
			get {
				if(ItemCore.ShowItemShadow != DefaultBoolean.Default) return ItemCore.ShowItemShadow.ToBoolean(false);
				return ControlCore.ShowItemShadow;
			}
		}
		AppearanceObject itemNormalAppearance;
		protected internal AppearanceObject ItemNormalAppearance {
			get {
				if(itemNormalAppearance == null)
					itemNormalAppearance = ControlInfoCore.OwnerCore.GetItemNormalAppearance(ItemCore);
				return itemNormalAppearance;
			}
		}
		AppearanceObject itemSelectedAppearance;
		protected internal AppearanceObject ItemSelectedAppearance {
			get {
				if(itemSelectedAppearance == null)
					itemSelectedAppearance = ControlInfoCore.OwnerCore.GetItemSelectedAppearance(ItemCore);
				return itemSelectedAppearance;
			}
		}
		protected internal Color DropDownSplitLineColor {
			get {
				AppearanceObject res = new AppearanceObject();
				AppearanceHelper.Combine(res, new AppearanceObject[] { GetAppearance(), ControlInfo.GetAppearance(this) }, DefaultAppearance);
				return Color.FromArgb(splitterAlpha, res.ForeColor);
			}
		}
		const int splitterAlpha = 70;
		public virtual bool ShouldDrawDropDown {
			get {
				if(ItemCore.ShowDropDownButton != DefaultBoolean.Default)
					return ItemCore.ShowDropDownButton.ToBoolean(false);
				return ItemCore.DropDownControl != null || ItemCore.TileBar != null;
			}
		}
		public virtual int DropDownWidth {
			get { 
				int w = ControlCore == null ? TileBar.dropDownButtonWidthDefault : ControlCore.DropDownButtonWidth;
				w = ControlInfoCore.GetScaledHorz(w);
				return w;
			}
		}
		public virtual Rectangle DropDownBounds {
			get {
				if(!ShouldDrawDropDown) return Rectangle.Empty;
				if(ControlInfo.IsRightToLeft)
					return new Rectangle(Bounds.X, Bounds.Y, DropDownWidth, Bounds.Height); 
				return new Rectangle(Bounds.X + (Bounds.Width - DropDownWidth), Bounds.Y, DropDownWidth, Bounds.Height); 
			}
		}
		int DropDownSplitLineX {
			get {
				int x = ControlInfo.IsRightToLeft ? DropDownBounds.Right - 1 : DropDownBounds.Left;
				return x;
			}
		}
		public Point DropDownSplitLineStartPt { get { return new Point(DropDownSplitLineX, DropDownBounds.Y + 7); } }
		public Point DropDownSplitLineEndPt { get { return new Point(DropDownSplitLineX, DropDownBounds.Y + DropDownBounds.Height - 7); } }
		TileBarHitInfo HoverInfoCore {
			get { return GroupInfo.ControlInfo.HoverInfo as TileBarHitInfo; }
		}
		TileBarHitInfo PressedInfoCore {
			get { return GroupInfo.ControlInfo.PressedInfo as TileBarHitInfo; }
		}
		protected internal bool DropDownHovered {
			get {
				TileBarHitInfo hitInfo = HoverInfoCore;
				if(hitInfo == null)
					return false;
				return hitInfo.InItem && hitInfo.ItemInfo.Item == Item && hitInfo.InDropDown;
			}
		}
		protected internal bool DropDownPressed {
			get {
				TileBarHitInfo hitInfo = PressedInfoCore;
				if(hitInfo == null)
					return false;
				return hitInfo.InItem && hitInfo.ItemInfo.Item == Item && hitInfo.InDropDown;
			}
		}
		bool dropDownPressedState;
		protected internal bool DropDownInPressedState {
			get { return dropDownPressedState; }
			set {
				if(dropDownPressedState == value)
					return;
				dropDownPressedState = value;
				ForceUpdateAppearanceColors();
				if(ControlInfo != null && ControlInfo.Owner != null)
					ControlInfo.Owner.Invalidate(DropDownBounds);
			}
		}
		protected override Rectangle CalcContentBounds(Rectangle bounds) {
			var result = base.CalcContentBounds(bounds);
			if(!ShouldDrawDropDown) return result;
			if(ControlInfo.IsRightToLeft)
				return CalcContentBoundsRTL(result);
			return CalcContentBoundsLTR(result);
		}
		Rectangle CalcContentBoundsLTR(Rectangle bounds) {
			bounds.Width += ItemPadding.Right;
			int delta = Math.Max(DropDownWidth, ItemPadding.Right);
			bounds.Width -= delta;
			return bounds;
		}
		Rectangle CalcContentBoundsRTL(Rectangle bounds) {
			bounds.Width += ItemPadding.Left;
			bounds.X -= ItemPadding.Left;
			int delta = Math.Max(DropDownWidth, ItemPadding.Left);
			bounds.Width -= delta;
			bounds.X += delta;
			return bounds;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(ControlCore == null) return base.CreateDefaultAppearance();
			if(IsHovered || IsPressed)
				return GetModifiedDefaultAppearance();
			if(IsSelected){
				return GetDefaultFromNormal();
			}
			return DefaultAppearanceNormal;
		}
		AppearanceDefault GetDefaultFromNormal() {
			AppearanceDefault res = new AppearanceDefault();
			res.Assign(DefaultAppearanceNormal);
			res.ForeColor = ItemNormalAppearance.ForeColor;
			res.BackColor = ItemNormalAppearance.BackColor;
			res.BackColor2 = ItemNormalAppearance.BackColor2;
			res.BorderColor = ItemNormalAppearance.BorderColor;
			res.Font = ItemNormalAppearance.Font;
			res.GradientMode = ItemNormalAppearance.GradientMode;
			res.HAlignment = ItemNormalAppearance.HAlignment;
			return res;
		}
		protected AppearanceDefault GetModifiedDefaultAppearance() {
			AppearanceDefault res = new AppearanceDefault();
			res.Assign(DefaultAppearanceNormal);
			if(IsSelected)
				PatchDefaultBackColor(res, ItemSelectedAppearance);
			else
				PatchDefaultBackColor(res, ItemNormalAppearance);
			return res;
		}
		protected virtual void PatchDefaultBackColor(AppearanceDefault appDef, AppearanceObject appObj) {
			appDef.BackColor = TileBarViewInfo.GetModifiedColor(appObj.BackColor);
			if(!appObj.BackColor2.IsEmpty) appDef.BackColor2 = TileBarViewInfo.GetModifiedColor(appObj.BackColor2);
		}
		SkinElement GetSkinElement(string elementName) {
			SkinElement elem = MetroUISkins.GetSkin(ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel)[elementName];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DefaultSkinProvider.Default)[elementName];
			return elem;
		}
		AppearanceDefault DefaultAppearanceNormal {
			get { return ControlInfoCore.OwnerCore.GetDefaultAppearanceItem(ItemCore); }
		}
		AppearanceObject dropDownAppearance;
		protected internal AppearanceObject DropDownAppearance {
			get {
				if(dropDownAppearance == null)
					dropDownAppearance = CreateDropDownPaintAppearance();
				return dropDownAppearance;
			}
		}
		AppearanceDefault dropDownDefaultAppearance;
		protected internal AppearanceDefault DropDownAppearanceDefault {
			get {
				if(dropDownDefaultAppearance == null)
					dropDownDefaultAppearance = CreateDropDownAppearanceDefault();
				return dropDownDefaultAppearance;
			}
		}
		private AppearanceDefault CreateDropDownAppearanceDefault() {
			if(DropDownInPressedState || DropDownHovered || DropDownPressed) 
				return GetModifiedDefaultAppearance();
			if(IsSelected)
				return GetDefaultFromNormal();
			return DefaultAppearanceNormal;
		}
		protected override AppearanceObject GetPressedAppearance() {
			AppearanceObject frameAppearance = new AppearanceObject();
			if(Item.CurrentFrame != null) frameAppearance = Item.CurrentFrame.Appearance;
			AppearanceObject pressed = new AppearanceObject();
			AppearanceHelper.Combine(pressed, new AppearanceObject[] { 
					Item.AppearanceItem.Pressed, ControlInfo.Owner.AppearanceItem.Pressed, frameAppearance }, DefaultAppearance);
			return pressed;
		}
		public override bool IsHovered {
			get { return base.IsHovered && !DropDownHovered; }
		}
		public override bool IsPressed {
			get {
				TileBarHitInfo hitInfo = PressedInfoCore;
				if(hitInfo == null)
					return base.IsPressed;
				return base.IsPressed && !hitInfo.InDropDown;
			}
		}
		public override AppearanceObject GetAppearance() {
			if(IsPressed) 
				return Item.AppearanceItem.Pressed;
			if(IsHovered) 
				return Item.AppearanceItem.Hovered;
			if(IsSelected)
				return Item.AppearanceItem.Selected;
			return Item.AppearanceItem.Normal;
		}
		protected AppearanceObject CreateDropDownPaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { GetDropDownAppearance(), ((TileBarViewInfo)GroupInfo.ControlInfo).GetDropDownAppearance(this) }, DropDownAppearanceDefault);
			return res;
		}
		private AppearanceObject GetDropDownAppearance() {
			if(DropDownPressed || DropDownInPressedState) {
				return Item.AppearanceItem.Pressed;
			}
			if(DropDownHovered) {
				return Item.AppearanceItem.Hovered;
			}
			if(IsSelected)
				return Item.AppearanceItem.Selected;
			return Item.AppearanceItem.Normal;
		}
		public override void ForceUpdateAppearanceColors() {
			dropDownAppearance = null;
			dropDownDefaultAppearance = null;
			itemNormalAppearance = null;
			itemSelectedAppearance = null;
			base.ForceUpdateAppearanceColors();
		}
		Image dropDownGlyph;
		protected internal Image DropDownGlyph {
			get {
				if(dropDownGlyph == null)
					dropDownGlyph = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileBar.Images.DropDownButton.png", typeof(TileBar).Assembly);
				return RotateByDropDownDirection(dropDownGlyph);
			}
		}
		Image RotateByDropDownDirection(Image input) {
			var dropside = ControlInfoCore.GetDropDownSide();
			if(dropside == TileBarViewInfo.DropDownSide.Top) {
				Image rotated = input.Clone() as Image;
				rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
				input = null;
				return rotated;
			}
			return input;
		}
		protected internal Point DropDownGlyphPosition {
			get {
				int x = DropDownBounds.X;
				int y = DropDownBounds.Y;
				x = x + (int)(DropDownBounds.Width / 2) - (int)(DropDownGlyph.Width / 2);
				y = y + (int)(DropDownBounds.Height / 2) - (int)(DropDownGlyph.Height / 2);
				return new Point(x, y);
			}
		}
		Control dropDownContent;
		public Control DropDownContent {
			get {
				if(dropDownContent == null)
					dropDownContent = CreateDropDownContent();
				return dropDownContent; 
			}
		}
		protected virtual Control CreateDropDownContent() {
			if(ItemCore.DropDownControl != null)
				return ItemCore.DropDownControl;
			if(ItemCore.TileBar != null)
				return ItemCore.TileBar;
			return null;
		}
		TileBarWindow dropDownWindow;
		public TileBarWindow DropDownWindow {
			get {
				if(dropDownWindow == null)
					dropDownWindow = CreateDropDownWindow();
				UpdateDropDownLocation(dropDownWindow);
				return dropDownWindow;
			}
		}
		void UpdateDropDownLocation(TileBarWindow dropDownWindow) {
			if(dropDownWindow == null || ItemCore.DropDownOwner != null) return;
			dropDownWindow.Options.Location = GetDropDownLocation();
		}
		Point GetDropDownLocation() { 
			var side = ControlInfoCore.GetDropDownSide();
			switch(side) { 
				case TileBarViewInfo.DropDownSide.Top:
					return new Point(0, -GetDropDownDimension(false));
				case TileBarViewInfo.DropDownSide.Bottom:
					return new Point(0, ControlInfoCore.Owner.Control.Height);
				case TileBarViewInfo.DropDownSide.Left:
					return new Point(-GetDropDownDimension(true), 0);
				default:
					return new Point(ControlInfoCore.Owner.Control.Width, 0);
			}
		}
		int GetDropDownDimension(bool iswidth) {
			if(ItemCore.DropDownOptions.AutoHeight != DefaultBoolean.Default) {
				if(ItemCore.DropDownOptions.AutoHeight.ToBoolean(false))
					return iswidth ? ItemCore.DropDownControl.Width : ItemCore.DropDownControl.Height;
				else
					return GetDropDownDimensionCore(iswidth);
			}
			if(ControlCore.DropDownOptions.AutoHeight != DefaultBoolean.Default) {
				if(ControlCore.DropDownOptions.AutoHeight.ToBoolean(false))
					return iswidth ? ItemCore.DropDownControl.Width : ItemCore.DropDownControl.Height;
				else
					return GetDropDownDimensionCore(iswidth);
			}
			return GetDropDownDimensionCore(iswidth);
		}
		int GetDropDownDimensionCore(bool iswidth) {
			int dimension = iswidth ? ControlCore.Width : ControlCore.Height;
			if(ItemCore.DropDownOptions.Height != 0) return ItemCore.DropDownOptions.Height;
			return ControlCore.DropDownOptions.Height == 0 ? dimension : ControlCore.DropDownOptions.Height;
		}
		int GetDropDownHeight() {
			if(ControlCore.Orientation == Orientation.Horizontal)
				return GetDropDownDimension(false);
			return GetDropDownDimension(true);
		}
		protected virtual TileBarWindow CreateDropDownWindow() {
			TileBarWindow w = new TileBarWindow(ControlCore, GetOwner());
			w.ApplyOptions();
			return w;
		}
		ITileBarWindowOwner GetOwner() {
			if(ItemCore.DropDownOwner != null)
				return ItemCore.DropDownOwner;
			return this;
		}
		FlyoutPanelOptions ToolWindowOptions {
			get {
				FlyoutPanelOptions opt = new FlyoutPanelOptions();
				opt.AnchorType = PopupToolWindowAnchor.Manual;
				opt.Location = GetDropDownLocation();
				opt.AnimationType = PopupToolWindowAnimation.Fade;
				return opt;
			}
		}
		bool GetCloseOnOuterClick() {
			if(ItemCore.DropDownOptions.CloseOnOuterClick != DefaultBoolean.Default)
				return ItemCore.DropDownOptions.CloseOnOuterClick.ToBoolean(true);
			return ControlCore.DropDownOptions.CloseOnOuterClick.ToBoolean(true);
		}
		protected override TileItemElementViewInfo CreateElementInfo(TileItemViewInfo itemInfo, TileItemElement elem) {
			return new TileBarItemElementViewInfo(itemInfo, elem);
		}
		protected internal void ResetDropDownAppearance() {
			dropDownAppearance = null;
			dropDownDefaultAppearance = null;
			itemNormalAppearance = null;
			itemSelectedAppearance = null;
		}
		Color GetDropDownBackColor() {
			TileBarDropDownOptions itemOptions = ItemCore.DropDownOptions;
			Color tileColor = ControlCore.GetItemNormalAppearance(ItemCore).BackColor;
			Color beakColor = itemOptions.BeakColor.IsEmpty ? ControlCore.DropDownOptions.BeakColor : itemOptions.BeakColor;
			if(itemOptions.BackColorMode == BackColorMode.UseTileBackColor)
				return tileColor;
			if(itemOptions.BackColorMode == BackColorMode.UseBeakColor)
				return beakColor;
			if(ControlCore.DropDownOptions.BackColorMode == BackColorMode.UseTileBackColor)
				return tileColor;
			if(ControlCore.DropDownOptions.BackColorMode == BackColorMode.UseBeakColor)
				return beakColor;
			return Color.Empty;
		}
		void ITileBarWindowOwner.OnDropDownClosed() {
			this.DropDownInPressedState = false;
			ControlInfoCore.DropDownWindow = null;
			ControlInfoCore.Owner.Invalidate(ControlInfoCore.BeakBounds);
		}
		Orientation ITileBarWindowOwner.Orientation { get { return ControlInfo.Owner.Properties.Orientation; } }
		TileBarWindow ITileBarWindowOwner.GetDropDown() { return DropDownWindow; }
		Control ITileBarWindowOwner.GetDropDownContent() { return DropDownContent; } 
		FlyoutPanelOptions ITileBarWindowOwner.DropDownOptions { get { return ToolWindowOptions; } } 
		bool ITileBarWindowOwner.CloseOnOuterClick { get { return GetCloseOnOuterClick(); } }
		int ITileBarWindowOwner.DropDownHeight { get { return GetDropDownHeight(); } }
		Color ITileBarWindowOwner.DropDownBackColor { get { return GetDropDownBackColor(); } }
		bool ITileBarWindowOwner.IsTileNavPane { get { return false; } }
		void ITileBarWindowOwner.UpdateTileBar(TileBar tb) { }
	}
	class TileBarItemElementViewInfo : TileItemElementViewInfo {
		public TileBarItemElementViewInfo(TileItemViewInfo itemInfo, TileItemElement element) : base(itemInfo, element) { }
		public override TileItemContentAlignment GetAlignment(int index, TileItemContentAlignment alignemnt) {
			if(alignemnt != TileItemContentAlignment.Default)
				return alignemnt;
			switch(index) {
				case 0: return TileItemContentAlignment.BottomLeft;
				case 1: return TileItemContentAlignment.BottomRight;
				case 2: return TileItemContentAlignment.TopLeft;
				case 3: return TileItemContentAlignment.TopRight;
			}
			return TileItemContentAlignment.MiddleCenter;
		}
		protected override TileItemContentAlignment GetImageAlignment() {
			TileItemContentAlignment res = Element.ImageAlignment != TileItemContentAlignment.Default ? Element.ImageAlignment : Item.ImageAlignment;
			if(res == TileItemContentAlignment.Default) res = Item.Control.Properties.ItemImageAlignment;
			return res == TileItemContentAlignment.Default ? TileItemContentAlignment.TopLeft : res;
		}
	}
	class TileBarGroupViewInfo : TileGroupViewInfo {
		public TileBarGroupViewInfo(TileGroup group) : base(group) { }
		protected override void CalcDropDownHitInfo(TileItemViewInfo tileItemInfo, TileControlHitInfo hitInfo) {
			TileBarHitInfo hit = hitInfo as TileBarHitInfo;
			TileBarItemViewInfo itemInfo = tileItemInfo as TileBarItemViewInfo;
			if(hit == null || itemInfo == null)
				return;
			if(itemInfo.DropDownBounds != Rectangle.Empty && itemInfo.DropDownBounds.Contains(hit.HitPoint))
				hit.InDropDown = true;
			else
				hit.InDropDown = false;
		}
	}
	public class TileBarHitInfo : TileControlHitInfo {
		protected internal bool InDropDown { get; set; }
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			TileBarHitInfo hitInfo = obj as TileBarHitInfo;
			if(hitInfo == null || hitInfo.HitTest != HitTest)
				return false;
			if(hitInfo.HitTest == TileControlHitTest.Control)
				return hitInfo.ViewInfo == ViewInfo;
			if(hitInfo.HitTest == TileControlHitTest.Group || hitInfo.HitTest == TileControlHitTest.GroupCaption)
				return hitInfo.GroupInfo == GroupInfo;
			if(hitInfo.ItemInfo == ItemInfo && hitInfo.InDropDown == InDropDown)
				return true;
			return false;
		}
	}
	public class TileBarLayoutItem : TileControlLayoutItem {
		protected override Point CalcLocation(TileGroupLayoutInfo info) {
			if(!ControlInfo.IsRightToLeft) {
				if(ItemInfo.IsLarge) return info.Location;
				return new Point(info.Location.X + info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
			else {
				if(ItemInfo.IsLarge) {
					int width = ControlInfo.GetItemSize(ItemInfo).Width;
					return new Point(info.Location.X - width, info.Location.Y);
				}
				int xpos = info.Location.X - ControlInfo.ItemSize;
				return new Point(xpos - info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
		}
	}
	public class TileBarLayoutCalculator : TileControlLayoutCalculator {
		public TileBarLayoutCalculator(TileControlViewInfo viewInfo) : base(viewInfo) { }
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new TileBarLayoutCalculator(viewInfo);
		}
		protected override TileControlLayoutItem GetNewLayoutItemCore(Rectangle bounds, TileItem item, TileItemViewInfo itemInfo, TileItemPosition position) {
			return new TileBarLayoutItem() { Bounds = bounds, Item = item, ItemInfo = itemInfo, Position = position };
		}
		protected override Point LayoutGroupHorizontal(TileControlLayoutGroup group, Point start) {
			TileGroupLayoutInfo info = new TileGroupLayoutInfo();
			info.StartLocation = start;
			info.Location = start;
			info.ColumnX = info.Location.X;
			info.BottomY = start.Y;
			foreach(TileControlLayoutItem item in group.Items) {
				if(!item.ItemInfo.ShouldProcessItem) continue;
				item.LayoutItem(info);
				info.BottomY = Math.Max(info.BottomY, item.Bounds.Bottom);
				item.Position = info.ItemPosition.Clone();
				MovePositionNext(info, item, group);
			}
			Point end = new Point(info.Location.X, start.Y);
			if(ViewInfo.IsRightToLeft)
				group.Bounds = new Rectangle(new Point(end.X, start.Y), new Size(start.X - end.X, info.BottomY - start.Y));
			else
				group.Bounds = new Rectangle(start, new Size(end.X - start.X, info.BottomY - start.Y));
			if(group.Bounds.Size.IsEmpty && ViewInfo.Owner.IsDesignMode) {
				end = LayoutEmptyGroupInDesignTime(group, start);
			}
			return end;
		}
		void MovePositionNext(TileGroupLayoutInfo info, TileControlLayoutItem item, TileControlLayoutGroup group) {
			int x;
			bool hasNextItem = group.GetNextItem(item) != null;
			if(ViewInfo.IsRightToLeft) {
				x = item.Bounds.X;
				x -= hasNextItem ? item.ControlInfo.IndentBetweenItems : 0;
			}
			else {
				x = item.Bounds.X + item.Bounds.Width;
				x += hasNextItem ? item.ControlInfo.IndentBetweenItems : 0;
			}
			info.Location = new Point(x, info.Location.Y);
		}
	}
	public class BeakInfo {
		public Point Position { get; set; }
		public TileBarItemViewInfo ItemInfo { get; set; }
	}
	public class TileBarDesignTimeManagerBase : BaseDesignTimeManager, ITileControlDesignManager {
		public BaseDesignTimeManager GetBase() { return this; }
		public object GetGroup() { return this.TileBarGroup; }
		public object GetItem() { return this.TileBarItem; }
		public TileBarDesignTimeManagerBase(IComponent component, TileBar tileBar)
			: base(component, tileBar.Site) {
			TileBar = tileBar;
			Component = component;
			ComponentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
		}
		public void ComponentChanged(IComponent comp) {
			if(ComponentChangeService != null && comp != null)
				ComponentChangeService.OnComponentChanged(comp, null, null, null);
		}
		void OnComponentRemoved(object sender, ComponentEventArgs e) {
			TileBar tileBar = e.Component as TileBar;
			if(tileBar != null) {
				while(tileBar.Groups.Count > 0)
					RemoveGroup(tileBar.Groups[0] as TileBarGroup);
			}
			else if(e.Component is TileBarItem)
				RemoveItem((TileBarItem)e.Component, false);
			else if(e.Component is TileBarGroup)
				RemoveGroup((TileBarGroup)e.Component, false);
		}
		public ITileControl TileBar { get; private set; }
		public IComponent Component { get; private set; }
		public TileBarGroup TileBarGroup {
			get {
				if(Component is TileBarGroup)
					return (TileBarGroup)Component;
				if(TileBarItem != null)
					return TileBarItem.Group as TileBarGroup;
				ICollection coll = SelectionService.GetSelectedComponents();
				foreach(IComponent comp in coll) {
					if(comp is TileBarGroup)
						return (TileBarGroup)comp;
				}
				return null;
			}
		}
		public TileBarItem TileBarItem {
			get {
				if(Component is TileBarItem)
					return (TileBarItem)Component;
				ICollection coll = SelectionService.GetSelectedComponents();
				foreach(IComponent comp in coll) {
					if(comp is TileBarItem)
						return (TileBarItem)comp;
				}
				return null;
			}
		}
		INameCreationService nameService;
		bool isSelectedCore;
		bool IsSelected { get { return isSelectedCore; } }
		void OnSelectChange(bool value) {
			if(IsSelected && !value)
				TileBar.Invalidate(TileBar.ClientRectangle);
			isSelectedCore = value;
		}
		public INameCreationService NameService {
			get {
				if(nameService == null)
					nameService = TileBar.Site.GetService(typeof(INameCreationService)) as INameCreationService;
				return nameService;
			}
		}
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = GetComponentChangeService();
				return componentChangeService;
			}
		}
		IComponentChangeService GetComponentChangeService() {
			Control tileBarOwner = ((TileBar)TileBar).Owner as Control;
			if(tileBarOwner != null)
				return tileBarOwner.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			return TileBar.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
		}
		public virtual void OnAddTileGroupClick() {
			if(TileBar.DebuggingState)
				return;
			TileBarGroup group = new TileBarGroup();
			TileBar.Groups.Add(group);
			if(TileBar.Container != null)
				TileBar.Container.Add(group);
			group.Name = NameService.CreateName(TileBar.Container, typeof(TileBarGroup));
			ComponentChangeService.OnComponentChanged(TileBar, null, null, null);
		}
		public void OnRemoveTileGroupClick(object group) {
			if(group is TileBarGroup) OnRemoveTileGroupClick(group as TileBarGroup);
		}
		public virtual void OnRemoveTileGroupClick(TileBarGroup group) {
			RemoveGroup(group);
		}
		public virtual void OnAddTileItemClick() {
			OnAddTileItemClick(false);
		}
		public virtual void OnAddLargeTileItemClick() {
			OnAddTileItemClick(true);
		}
		public void OnAddTileItemClick(object islarge) {
			if(islarge is Boolean) {
				OnAddTileItemClick((bool)islarge);
				return;
			}
			if(islarge is TileItemSize) {
				TileBarItemSize size = ((TileItemSize)islarge) == TileItemSize.Wide ? TileBarItemSize.Wide : TileBarItemSize.Medium;
				OnAddTileItemClick(size);
			}
		}
		public virtual void OnAddTileItemClick(bool isLarge) {
			OnAddTileItemClick(isLarge ? TileBarItemSize.Wide : TileBarItemSize.Medium);
		}
		public virtual void OnAddTileItemClick(TileBarItemSize itemType) {
			if(TileBarGroup == null || TileBar.DebuggingState)
				return;
			TileBarItem item = CreateTileItem(itemType);
			item.Id = ((TileBar)TileBarGroup.Control).GetNextId();
			item.Name = NameService.CreateName(TileBar.Container, typeof(TileBarItem));
			item.Text = item.Name;
			TileBarGroup.Items.Add(item);
			if(TileBarGroup.Container != null)
				TileBarGroup.Container.Add(item);
			ComponentChangeService.OnComponentChanged(TileBarGroup, null, null, null);
		}
		public static TileBarItem CreateTileItem(TileBarItemSize itemSize) {
			TileBarItem item = new TileBarItem();
			item.ItemSize = itemSize;
			return item;
		}
		protected override void OnDesignTimeSelectionChanged(object sender, EventArgs e) {
			base.OnDesignTimeSelectionChanged(sender, e);
			OnSelectChange(SelectionService.GetComponentSelected(TileBar) || TileBarItem != null || TileBarGroup != null);
		}
		public virtual void RemoveGroup(TileBarGroup group) { RemoveGroup(group, true); }
		public virtual void RemoveGroup(TileBarGroup group, bool removeFromContainer) {
			if(TileBar.DebuggingState || group == null)
				return;
			while(group.Items.Count > 0) {
				RemoveItem(group.Items[0] as TileBarItem, true);
			}
			if(removeFromContainer && group.Control != null)
				group.Control.Container.Remove(group);
			if(group.Control != null)
				group.Control.Groups.Remove(group);
			ComponentChangeService.OnComponentChanged(TileBar, null, null, null);
		}
		public virtual void RemoveItem(TileBarItem item) { RemoveItem(item, true); }
		public virtual void RemoveItem(TileBarItem item, bool removeFromContainer) {
			if(TileBar.DebuggingState)
				return;
			TileBarGroup group = item.Group as TileBarGroup;
			if(removeFromContainer && item.Group != null && item.Group.Control != null)
				item.Group.Control.Container.Remove(item);
			if(item.Group != null)
				item.Group.Items.Remove(item);
			if(group != null)
				ComponentChangeService.OnComponentChanged(group, null, null, null);
			else
				ComponentChangeService.OnComponentChanged(TileBar, null, null, null);
		}
		public virtual void OnRemoveTileItemClick(object item) {
			if(item is TileBarItem) OnRemoveTileItemClick(item as TileBarItem);
		}
		public virtual void OnRemoveTileItemClick(TileBarItem item) {
			if(item == null)
				return;
			RemoveItem(item);
		}
		protected virtual void OnAddTileGroupClick(object sender, EventArgs e) {
			OnAddTileGroupClick();
		}
		protected virtual void OnRemoveTileGroupClick(object sender, EventArgs e) {
			OnRemoveTileGroupClick(((TileGroupViewInfo)((DXMenuItem)sender).Tag).Group as TileBarGroup);
		}
		protected virtual void OnAddMediumTileItemClick(object sender, EventArgs e) {
			OnAddTileItemClick();
		}
		protected virtual void OnAddWideTileItemClick(object sender, EventArgs e) {
			OnAddLargeTileItemClick();
		}
		protected virtual void OnRemoveTileItemClick(object sender, EventArgs e) {
			OnRemoveTileItemClick(((TileBarItemViewInfo)((DXMenuItem)sender).Tag).ItemCore);
		}
		public virtual void ShowGroupMenu(TileControlHitInfo hitInfo) {
			DXPopupMenu popupMenu = new DXPopupMenu();
			FillDesignTimePopupMenu(popupMenu, true, hitInfo);
			MenuManagerHelper.Standard.ShowPopupMenu(popupMenu, TileBar.Control, hitInfo.HitPoint);
		}
		public virtual void ShowItemMenu(TileControlHitInfo hitInfo) {
			DXPopupMenu popupMenu = new DXPopupMenu();
			FillDesignTimePopupMenu(popupMenu, false, hitInfo);
			MenuManagerHelper.Standard.ShowPopupMenu(popupMenu, TileBar.Control, hitInfo.HitPoint);
		}
		protected virtual void FillDesignTimePopupMenu(DXPopupMenu popupMenu, bool isGroupMenu, TileControlHitInfo hitInfo) {
			DXMenuItem addGroup = new DXMenuItem() { Caption = "Add Group", Tag = hitInfo.GroupInfo };
			addGroup.Click += new EventHandler(OnAddTileGroupClick);
			addGroup.Enabled = !TileBar.DebuggingState;
			DXMenuItem removeGroup = new DXMenuItem() { Caption = "Remove Group", Tag = hitInfo.GroupInfo };
			removeGroup.Click += new EventHandler(OnRemoveTileGroupClick);
			removeGroup.Enabled = !TileBar.DebuggingState;
			DXMenuItem addMediumItem = new DXMenuItem() { Caption = "Add Medium Item", Tag = hitInfo.ItemInfo };
			addMediumItem.Click += new EventHandler(OnAddMediumTileItemClick);
			addMediumItem.Enabled = !TileBar.DebuggingState;
			DXMenuItem addWideItem = new DXMenuItem() { Caption = "Add Wide Item", Tag = hitInfo.ItemInfo };
			addWideItem.Click += new EventHandler(OnAddWideTileItemClick);
			addWideItem.Enabled = !TileBar.DebuggingState;
			DXMenuItem removeItem = new DXMenuItem() { Caption = "Remove Item", Tag = hitInfo.ItemInfo };
			removeItem.Click += new EventHandler(OnRemoveTileItemClick);
			removeItem.Enabled = !TileBar.DebuggingState;
			popupMenu.Items.Add(addGroup);
			popupMenu.Items.Add(removeGroup);
			popupMenu.Items.Add(addMediumItem);
			popupMenu.Items.Add(addWideItem);
			if(!isGroupMenu) {
				popupMenu.Items.Add(removeItem);
			}
		}
		public void OnEditElementsCollectionClickCore(object tile) {
			if(tile is TileBarItem) OnEditElementsCollectionClickCore(tile as TileBarItem);
		}
		public void OnEditFramesCollectionClickCore(object tile) { }
		public void OnEditTileTemplateClickCore(object tile) { }
		public virtual void OnEditElementsCollectionClickCore(TileBarItem item) { }
		public void DrawSelectionBounds(GraphicsCache cache, Rectangle bounds, Color color) {
			using(Pen pen = new Pen(Color.Red)) {
				pen.DashPattern = new float[] { 5.0f, 5.0f };
				cache.Graphics.DrawRectangle(pen, bounds);
			}
		}
		public virtual void FireChanged() {
			ComponentChangeService.OnComponentChanged(TileBar, null, null, null);
		}
	}
}
