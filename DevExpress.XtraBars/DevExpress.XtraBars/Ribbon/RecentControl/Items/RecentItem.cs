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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Ribbon {
	public enum RecentPinItemGlyphAlignment { Top, Center }
	public enum RecentPinButtonVisibility { Always, Never, AutoHidden }
	public class RecentPinItem : RecentTextGlyphItemBase {
		private static readonly object pinButtonCheckedChanged = new object();
		string description;
		bool showDescription;
		bool pinButtonChecked;
		RecentPinItemGlyphAlignment glyphAlignment;
		RecentPinButtonVisibility pinButtonVisibility;
		public RecentPinItem()
			: base() {
			this.showDescription = true;
			this.pinButtonChecked = false;
			this.glyphAlignment = RecentPinItemGlyphAlignment.Top;
			this.pinButtonVisibility = RecentPinButtonVisibility.AutoHidden;
			}
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RecentPinItemAppearances Appearances { get { return base.Appearances as RecentPinItemAppearances;} } 
		protected override BaseAppearanceCollection CreateAppearanceCollection() {
			return new RecentPinItemAppearances();
		}
		[DefaultValue("")]
		public string Description {
			get { return description; }
			set {
				if(Description == value) return;
				description = value;
				OnItemChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowDescription {
			get { return showDescription; }
			set {
				if(ShowDescription == value) return;
				showDescription = value;
				OnItemChanged();
			}
		}
	   [DefaultValue(RecentPinButtonVisibility.AutoHidden)]
		public RecentPinButtonVisibility PinButtonVisibility {
			get { return pinButtonVisibility; }
			set {
				if(PinButtonVisibility == value) return;
				pinButtonVisibility = value;
				OnItemChanged();
			}
		}
		[DefaultValue(false)]
		public bool PinButtonChecked {
			get { return pinButtonChecked; }
			set {
				if(PinButtonChecked == value) return;
				pinButtonChecked = value;
				RaisePinButtonCheckedChanged();
				OnItemChanged();
			}
		}
		[DefaultValue(RecentPinItemGlyphAlignment.Top)]
		public RecentPinItemGlyphAlignment GlyphAlignment {
			get { return glyphAlignment; }
			set { 
				if(GlyphAlignment == value) return;
				glyphAlignment = value;
				OnItemChanged();
			}
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentPinItemViewInfo(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentPinItemPainter();
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentPinItemHandler(this);
		}
		public event RecentItemEventHandler PinButtonCheckedChanged {
			add { Events.AddHandler(pinButtonCheckedChanged, value); }
			remove { Events.RemoveHandler(pinButtonCheckedChanged, value); }
		}
		protected internal void RaisePinButtonCheckedChanged() {
			RecentItemEventHandler handler = Events[pinButtonCheckedChanged] as RecentItemEventHandler;
			if(handler != null)
				handler(this, new RecentItemEventArgs(this));
		}
	}
	public class RecentPinItemAppearances : BaseRecentItemAppearanceCollection {
		AppearanceObject descriptionNormal, descriptionHovered, descriptionPressed;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.descriptionNormal = CreateAppearance("DescriptionNormal");
			this.descriptionHovered = CreateAppearance("DescriptionHovered");
			this.descriptionPressed = CreateAppearance("DescriptionPressed");
		}
		void ResetDescriptionNormal() { DescriptionNormal.Reset(); }
		bool ShouldSerializeDescriptionNormal() { return DescriptionNormal.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DescriptionNormal { get { return descriptionNormal; } }
		void ResetDescriptionHovered() { DescriptionHovered.Reset(); }
		bool ShouldSerializeDescriptionHovered() { return DescriptionHovered.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DescriptionHovered { get { return descriptionHovered; } }
		void ResetDescriptionPressed() { DescriptionPressed.Reset(); }
		bool ShouldSerializeDescriptionPressed() { return DescriptionPressed.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DescriptionPressed { get { return descriptionPressed; } }
		public override string ToString() {
			return string.Empty;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentPinItemViewInfo : RecentTextGlyphItemViewInfoBase {
		bool pinButtonHovered;
		bool pinButtonPressed;
		public RecentPinItemViewInfo(RecentPinItem item)
			: base(item) {
				this.pinButtonHovered = false;
				this.pinButtonPressed = false;
		}
		public RecentPinItem RecentItem { get { return Item as RecentPinItem; } }
		public Size DescriptionSize { get; private set; }
		public Size PinButtonSize { get; private set; }
		public Rectangle DescriptionBounds { get; private set; }
		public Rectangle PinButtonBounds { get; private set; }
		public bool PinChecked { get { return RecentItem.PinButtonChecked; } }
		public bool IsPinButtonPressed {
			get { return pinButtonPressed; }
			set {
				if(IsPinButtonPressed == value)
					return;
				pinButtonPressed = value;
				Item.RecentControl.Invalidate(Bounds);
			}
		}
		public bool IsPinButtonHovered {
			get { return pinButtonHovered; }
			set {
				if(IsPinButtonHovered == value)
					return;
				pinButtonHovered = value;
				Item.RecentControl.Invalidate(Bounds);
			}
		}
		public bool IsPinButtonVisible { 
			get {
				switch(RecentItem.PinButtonVisibility) {
					case RecentPinButtonVisibility.Never:
						return false;
					case RecentPinButtonVisibility.Always:
						return true;
					case RecentPinButtonVisibility.AutoHidden:
						if (IsHovered || IsSelected || PinChecked) return true;
						break;
				}
				return false;
			} 
		}
		#region Appearance
		protected override BaseAppearanceCollection CreatePaintAppearances() {
			return new RecentPinItemAppearances();
		}
		protected override BaseRecentItemAppearanceCollection ControlAppearances {
			get { return (Item.RecentControl.Appearances as RecentAppearanceCollection).RecentItem; }
		}
		protected override BaseRecentItemAppearanceCollection ItemAppearanceCollection {
			get { return RecentItem.Appearances as BaseRecentItemAppearanceCollection; }
		}
		protected override AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault app = new AppearanceDefault();
			ApplyBaseForeColor(app);
			app.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
			app.FontStyleDelta = System.Drawing.FontStyle.Regular;
			AppearanceDefault appearance = new AppearanceDefault();
			SkinElement element = Item.RecentControl.GetViewInfo().GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryItemSubCaption);
			if(element != null) {
				element.ApplyForeColorAndFont(appearance);
			}
			appearance.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
			appearance.FontStyleDelta = FontStyle.Regular;
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", app),
				new AppearanceDefaultInfo("ItemHovered", app),
				new AppearanceDefaultInfo("ItemPressed", app),
				new AppearanceDefaultInfo("DescriptionNormal", appearance),
				new AppearanceDefaultInfo("DescriptionHovered", appearance),
				new AppearanceDefaultInfo("DescriptionPressed", appearance)
			};
		}
		protected internal AppearanceObject GetDescriptionPaintAppearance() {
			if(IsPressed) return GetPaintAppearance("DescriptionPressed");
			if(IsHovered) return GetPaintAppearance("DescriptionHovered");
			return GetPaintAppearance("DescriptionNormal");
		}
		#endregion
		protected override Size CalcBestSizeCore(int width) {
			Size size = base.CalcBestSizeCore(width);
			if(RecentItem.GlyphAlignment == RecentPinItemGlyphAlignment.Center)
				size.Height = Math.Max(GlyphSize.Height, CaptionTextSize.Height + DescriptionSize.Height);
			else
				size.Height += DescriptionSize.Height;
			PinButtonSize = CalcPinButtonSize();
			BestSize = new Size(Math.Max(CaptionTextSize.Width, DescriptionSize.Width) + GlyphSize.Width + PinButtonSize.Width, Math.Max(size.Height, PinButtonSize.Height));
			return BestSize;
		}
		protected override void CalcTextSizes(int width) {
			base.CalcTextSizes(width);
			DescriptionSize = CalcDescriptionSize();
		}
		Size CalcDescriptionSize() {
			return PaintAppearance.GetAppearance("DescriptionNormal").CalcTextSizeInt(Item.RecentControl.GetViewInfo().GInfo.Graphics, RecentItem.Description, 0);
		}
		protected override System.Windows.Forms.Padding GetSkinItemPadding() {
			return new System.Windows.Forms.Padding(5);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			PinButtonBounds = CalcPinButtonBounds();
			DescriptionBounds = CalcDescriptionBounds();
		}
		Rectangle CalcDescriptionBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.X = CaptionTextBounds.X;
			rect.Y = CaptionTextBounds.Bottom;
			rect.Width = Math.Min(DescriptionSize.Width, RecentItem.PinButtonVisibility != RecentPinButtonVisibility.Never ? PinButtonBounds.Left - rect.X : ContentBounds.Right - rect.X);
			rect.Height = DescriptionSize.Height;
			return rect;
		}
		Rectangle CalcPinButtonBounds() {
			if(RecentItem.PinButtonVisibility == RecentPinButtonVisibility.Never) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.X = ContentBounds.Right - PinButtonSize.Width;
			rect.Y = ContentBounds.Y + (ContentBounds.Height - PinButtonSize.Height) / 2;
			rect.Size = PinButtonSize;
			return rect;
		}
		protected override Rectangle CalcCaptionTextBounds() {
			Rectangle rect = base.CalcCaptionTextBounds();
			if(RecentItem.GlyphAlignment == RecentPinItemGlyphAlignment.Top)
				rect.Height = Math.Max(CaptionTextSize.Height, GlyphSize.Height);
			else rect.Height = ContentBounds.Height - DescriptionSize.Height;
			rect.Width = Math.Min(rect.Width, ContentBounds.Right - PinButtonSize.Width - rect.X);
			return rect;
		}
		protected internal override RecentControlHitInfo CalcHitInfo(RecentControlHitInfo hitInfo) {
			RecentControlHitInfo itemHitInfo = base.CalcHitInfo(hitInfo);
			itemHitInfo.ContainsSet(PinButtonBounds, RecentControlHitTest.PinButton);
			return itemHitInfo;
		}
		protected internal override int CalcMinWidth() {
			int min = base.CalcMinWidth();
			min += PinButtonSize.Width;
			return min;
		}
		public Size CalcPinButtonSize() {
			SkinElementInfo info = GetPinButtonInfo();
			return ObjectPainter.CalcObjectMinBounds(Item.RecentControl.GetViewInfo().GInfo.Graphics, SkinElementPainter.Default, info).Size;
		}
		public SkinElementInfo GetPinButtonInfo() {
			SkinElement element = GetPinButtonElement();
			SkinElementInfo info = new SkinElementInfo(element, PinButtonBounds);
			ObjectState state = CalcPinButtonState();
			if(PinChecked)
				info.ImageIndex = state == ObjectState.Normal ? 0 : 1;
			else
				info.ImageIndex = state == ObjectState.Normal ? 2 : 3;
			info.BackAppearance.BackColor = CommonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel).Colors[CommonColors.ControlText];
			return info;
		}
		private ObjectState CalcPinButtonState() {
			if(IsPinButtonPressed) return ObjectState.Pressed;
			if(IsPinButtonHovered) return ObjectState.Hot;
			return ObjectState.Normal;
		}
		SkinElement GetPinButtonElement() {
			SkinElement element = RibbonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel)["PinUnPinButton"];
			if(element == null)
				element = RibbonSkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)["PinUnPinButton"];
			return element;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentPinItemPainter : RecentTextGlyphItemPainterBase {
		protected override void DrawItemCore(GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) {
			base.DrawItemCore(cache, itemBaseInfo);
			RecentPinItemViewInfo recentItemInfo = itemBaseInfo as RecentPinItemViewInfo;
			DrawDescription(cache, recentItemInfo);
			if(recentItemInfo.IsPinButtonVisible)
				DrawPinButton(cache, recentItemInfo);
		}
		void DrawPinButton(GraphicsCache cache, RecentPinItemViewInfo fileInfo) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, fileInfo.GetPinButtonInfo());
		}
		void DrawDescription(GraphicsCache cache, RecentPinItemViewInfo fileInfo) {
			fileInfo.GetDescriptionPaintAppearance().DrawString(cache, fileInfo.RecentItem.Description, fileInfo.DescriptionBounds);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentPinItemHandler : RecentItemHandlerBase {
		public RecentPinItemHandler(RecentPinItem item) : base(item) { }
		public RecentPinItem RecentItem { get { return Item as RecentPinItem; } }
		public RecentPinItemViewInfo RecentItemInfo { get { return RecentItem.ViewInfo as RecentPinItemViewInfo; } }
		public override bool OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(RecentItemInfo.PinButtonBounds.Contains(e.Location))
				RecentItemInfo.IsPinButtonPressed = true;
			return base.OnMouseDown(e);
		}
		public override bool OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(RecentItemInfo.IsPinButtonPressed)
				if(RecentItemInfo.PinButtonBounds.Contains(e.Location)) {
					RecentItemInfo.IsPinButtonPressed = false;
					ClickPinButton();
					return true;
				}
			return base.OnMouseUp(e);
		}
		protected void ClickPinButton() {
			if(RecentItem.OwnerPanel.MovePinnedItemsUp) {
				PinItemMoveHelper helper = new PinItemMoveHelper(RecentItem);
				helper.MovePinItemWithCheckedChanged();
			}
			else RecentItem.PinButtonChecked = !RecentItem.PinButtonChecked;
			RecentItemInfo.IsPinButtonHovered = false;
		}
		public override bool OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			if(RecentItemInfo.PinButtonBounds.Contains(e.Location))
				RecentItemInfo.IsPinButtonHovered = true;
			else {
				RecentItemInfo.IsPinButtonHovered = false;
				RecentItemInfo.IsPinButtonPressed = false;
			}
			return base.OnMouseMove(e);
		}
	}
	public class PinItemMoveHelper {
		RecentPinItem recentItem;
		RecentItemCollection items;
		RecentItemCollection Items { get { return items; } }
		public PinItemMoveHelper(RecentPinItem recentItem) {
			this.recentItem = recentItem;
			items = recentItem.OwnerPanel.Items;
		}
		int separatorIndex = -1;
		int startGroupIndex = -1;
		int endGroupIndex = -1;
		bool isGroupStarted = false;
		bool isItemFounded = false;
		public void MovePinItemWithCheckedChanged() {
			CalcPinItemGroupParams();
			recentItem.PinButtonChecked = !recentItem.PinButtonChecked;
			MovePinItem();
		}
		void CalcPinItemGroupParams() {
			for(int i = 0; i < Items.Count; i++) {
				if(recentItem == Items[i]) isItemFounded = true;
				if(!isGroupStarted) {
					if(Items[i] is RecentPinItem) {
						isGroupStarted = true;
						startGroupIndex = i;
					}
					continue;
				}
				if(CheckInGroupItem(i)) break;
			}
			if(endGroupIndex == -1)
				endGroupIndex = Items.Count - 1;
		}
		bool CheckInGroupItem(int index) {
			RecentItemBase item = Items[index];
			RecentPinItem prevPinItem = Items[index - 1] as RecentPinItem;
			RecentPinItem pinItem = item as RecentPinItem;
			if(pinItem != null) return RecentItemIsPin(pinItem, prevPinItem, index);
			RecentSeparatorItem separator = item as RecentSeparatorItem;
			if(separator != null) return RecentItemIsSeparator(prevPinItem, index);
			isGroupStarted = false;
			if(isItemFounded) {
				endGroupIndex = index - 1;
				return true;
			}
			return false;
		}
		bool RecentItemIsPin(RecentPinItem pinItem, RecentPinItem prevPinItem, int index) {
			if(pinItem.PinButtonChecked) {
				if(prevPinItem != null && prevPinItem.PinButtonChecked) return false;
				if(isItemFounded && pinItem != recentItem) {
					endGroupIndex = index - 1;
					return true;
				}
				startGroupIndex = index;
				return false;
			}
			if(prevPinItem != null && !prevPinItem.PinButtonChecked) return false;
			if(Items[index - 1] is RecentSeparatorItem) return false;
			if(isItemFounded && pinItem != recentItem) {
				endGroupIndex = index - 1;
				return true;
			}
			startGroupIndex = index;
			return false;
		}
		bool RecentItemIsSeparator(RecentPinItem prevPinItem, int index) {
			if(prevPinItem.PinButtonChecked && Items.Count > index + 1) {
				RecentPinItem nextPin = Items[index + 1] as RecentPinItem;
				if(nextPin != null && !nextPin.PinButtonChecked) {
					separatorIndex = index;
					return false;
				}
			}
			if(isItemFounded) {
				endGroupIndex = index - 1;
				return true;
			}
			isGroupStarted = false;
			return false;
		}
		void MovePinItem() {
			if(HasNoGroup) return;
			if(GroupHasSeparator)
				CorrectionSeparator(recentItem.PinButtonChecked ? endGroupIndex - 1 : startGroupIndex + 1);
			else
				CorrectionItems();
		}
		bool HasNoGroup { get { return endGroupIndex == startGroupIndex; } }
		bool GroupHasSeparator { get { return separatorIndex > startGroupIndex; } }
		void CorrectionSeparator(int groupIndex) {
			if(separatorIndex != groupIndex) {
				Items.Remove(recentItem);
				Items.Insert(separatorIndex, recentItem);
			}
			else Items.RemoveAt(separatorIndex);
		}
		void CorrectionItems() {
			if(recentItem.PinButtonChecked)
				MovePinItemChecked();
			else
				MovePinItemUnChecked();
		}
		void MovePinItemChecked() {
			Items.Insert(startGroupIndex, new RecentSeparatorItem());
			Items.Remove(recentItem);
			Items.Insert(startGroupIndex, recentItem);
		}
		void MovePinItemUnChecked() {
			Items.Remove(recentItem);
			Items.Insert(endGroupIndex, recentItem);
			Items.Insert(endGroupIndex, new RecentSeparatorItem());
		}
	}
}
