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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraEditors {
	public static class TileItemCalculator {
		[Obsolete("Use ImageToTextAlignmentToItemLocation"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ItemLocation ImageToTextAlignemtToItemLocation(TileControlImageToTextAlignment alignment) {
			return ImageToTextAlignmentToItemLocation(alignment);
		}
		public static ItemLocation ImageToTextAlignmentToItemLocation(TileControlImageToTextAlignment alignment) {
			switch(alignment) {
				case TileControlImageToTextAlignment.Top:
					return ItemLocation.Top;
				case TileControlImageToTextAlignment.Left:
					return ItemLocation.Left;
				case TileControlImageToTextAlignment.Right:
					return ItemLocation.Right;
				case TileControlImageToTextAlignment.Bottom:
					return ItemLocation.Bottom;
			}
			return ItemLocation.Default;
		}
		public static Rectangle LayoutContent(Rectangle rect, Size size, TileItemContentAlignment align, Point pt) {
			return LayoutContent(rect, size, align, pt, false, false);
		}
		public static Rectangle LayoutContent(Rectangle rect, Size size, TileItemContentAlignment align, Point pt, bool stretchHorizontal, bool stretchVertical) {
			Rectangle res = new Rectangle(rect.Location, size);
			int centerX = rect.X + (rect.Width - res.Width) / 2;
			int centerY = rect.Y + (rect.Height - res.Height) / 2;
			int rightX = rect.Right - res.Width;
			int bottomY = rect.Bottom - res.Height;
			switch(align) {
				case TileItemContentAlignment.Manual:
					res.X = rect.X + pt.X;
					res.Y = rect.Y + pt.Y;
					break;
				case TileItemContentAlignment.TopLeft:
					res.X += pt.X;
					res.Y += pt.Y;
					break;
				case TileItemContentAlignment.TopCenter:
					res.X = centerX + pt.X;
					res.Y += pt.Y;
					break;
				case TileItemContentAlignment.TopRight:
					res.X = rightX + pt.X;
					res.Y += pt.Y;
					break;
				case TileItemContentAlignment.MiddleLeft:
					res.Y = centerY + pt.Y;
					res.X += pt.X;
					break;
				case TileItemContentAlignment.MiddleCenter:
					res.X = centerX + pt.X;
					res.Y = centerY + pt.Y;
					break;
				case TileItemContentAlignment.MiddleRight:
					res.X = rightX + pt.X;
					res.Y = centerY + pt.Y;
					break;
				case TileItemContentAlignment.BottomLeft:
					res.Y = bottomY + pt.Y;
					res.X += pt.X;
					break;
				case TileItemContentAlignment.BottomCenter:
					res.X = centerX + pt.X;
					res.Y = bottomY + pt.Y;
					break;
				case TileItemContentAlignment.BottomRight:
					res.X = rightX + pt.X;
					res.Y = bottomY + pt.Y;
					break;
			}
			if(stretchHorizontal)
				res.Width = rect.Right - res.X;
			if(stretchVertical)
				res.Height = rect.Bottom - res.Y;
			return res;
		}
		public static Rectangle OffsetRect(Rectangle rect, int deltaX, int deltaY) {
			rect.Offset(deltaX, deltaY);
			return rect;
		}
	}
	public class TileItemElementViewInfo : IAnchored {
		public TileItemElementViewInfo() {
			DrawImage = true;
			DrawText = true;
			PaintAppearance = new FrozenAppearance();
		}
		public TileItemElementViewInfo(TileItemViewInfo itemInfo, TileItemElement element)
			: this() {
			ItemInfo = itemInfo;
			Element = element;
		}
		public TileControlViewInfo ControlInfo { get { return ItemInfo == null ? null : ItemInfo.ControlInfo; } }
		public bool IsReady { get; private set; }
		public TileItemContentAlignment Alignment { get; set; }
		public string Text { get { return Element.GetDisplayText(); } }
		public StringInfo StringInfo { get; set; }
		public Rectangle TextBounds { get; set; }
		public Rectangle BackgroundBounds { get; set; }
		public Point Location { get { return GetScaledPoint(Element.TextLocation); } }
		public Point ImageLocation { get { return GetScaledPoint(Element.ImageLocation); } }
		public int MaxWidth { get { return GetScaledWidth(Element.MaxWidth); } }
		public int Width { get { return GetScaledWidth(Element.Width); } }
		public int Height { get { return GetScaledHeight(Element.Height); } }
		public AppearanceObject PaintAppearance { get; set; }
		public Rectangle ImageBounds { get; set; }
		public Rectangle ImageContentBounds { get; set; }
		public Size ImageSize { get { return GetScaledSize(Element.ImageSize); } }
		internal Items2Panel ItemsPanel { get; set; }
		public Rectangle EntireElementBounds { get { return CalcEntireElementBounds(); } }
		public AppearanceObject PaintAppearanceBackground { get; set; }
		public TileItemElement Element { get; protected internal set; }
		public TileItemViewInfo ItemInfo { get; protected internal set; }
		public TileItem Item { get { return ItemInfo.Item; } }
		public bool DrawImage { get; set; }
		public bool DrawText { get; set; }
		internal bool ImagePrerendered { get; set; }
		internal bool TextPrerendered { get; set; }
		public bool AllowGlyphSkinning { get { return ItemInfo.AllowGlyphSkinning; } }
		public bool AnimateImage {
			get {
				if(Element.AnimateTransition == DefaultBoolean.Default)
					return ItemInfo.AnimateImage;
				return Element.AnimateTransition == DefaultBoolean.True;
			}
		}
		public bool AnimateText {
			get {
				if(Element.AnimateTransition == DefaultBoolean.Default)
					return ItemInfo.AnimateText;
				return Element.AnimateTransition == DefaultBoolean.True;
			}
		}
		public TileItemImageScaleMode ImageScaleMode {
			get {
				return Element.ImageScaleMode != TileItemImageScaleMode.Default ? Element.ImageScaleMode : ControlInfo.Owner.Properties.ItemImageScaleMode;
			}
		}
		public bool ShouldDrawSimpleTextBackground {
			get {
				var app = GetCurrentBackgroundAppearance();
				return !app.BackColor.IsEmpty;
			}
		}
		bool IsRightToLeft {
			get {
				if(ItemInfo == null || ItemInfo.GroupInfo == null || ItemInfo.GroupInfo.ControlInfo == null)
					return false;
				return ItemInfo.GroupInfo.ControlInfo.IsRightToLeft;
			}
		}
		public virtual TileItemContentAlignment GetAlignment(int index, TileItemContentAlignment alignment) {
			if(alignment != TileItemContentAlignment.Default)
				return alignment;
			switch(index) {
				case 0: return TileItemContentAlignment.TopLeft;
				case 1: return TileItemContentAlignment.TopRight;
				case 2: return TileItemContentAlignment.BottomLeft;
				case 3: return TileItemContentAlignment.BottomRight;
			}
			return TileItemContentAlignment.MiddleCenter;
		}
		public bool UseOptimizedImage {
			get { return Element.OptimizedImage != null; }
		}
		public Image Image {
			get {
				if(Element.ImageUri != null && Element.ImageUri.IsInitialized) 
					return GetImageByUri();
				var img = Element.GetDisplayImage();
				if(img != null)
					return img;
				if(Item.Control == null)
					return null;
				return ImageCollection.GetImageListImage(Item.Control.Images, Element.ImageIndex);
			}
		}
		protected virtual Image GetImageByUri() {
			if(Element.ImageUri.HasDefaultImage) return Element.ImageUri.GetDefaultImage();
			if(Element.ImageUri.HasLargeImage) return Element.ImageUri.GetLargeImage();
			if(Element.ImageUri.HasImage) return Element.ImageUri.GetImage();
			return null;
		}
		Point GetScaledPoint(Point input) {
			if(input != Point.Empty) {
				SizeF scale = GetScaleFactor();
				if(scale.Width == scale.Height && scale.Width == 1.0f)
					return input;
				int x = input.X + ItemInfo.ItemPadding.Left;
				int y = input.Y + ItemInfo.ItemPadding.Top;
				x = ((int)(x * scale.Width)) - ItemInfo.ItemPadding.Left;
				y = ((int)(y * scale.Height)) - ItemInfo.ItemPadding.Top;
				return new Point(x, y);
			}
			return input;
		}
		protected Size GetScaledSize(Size input) {
			return Element.GetScaledSize(input);
		}
		protected int GetScaledWidth(int input) {
			if(input != -1 && input != 0) {
				SizeF scale = GetScaleFactor();
				if(scale.Width == scale.Height && scale.Width == 1.0f)
					return input;
				return (int)(input * scale.Width);
			}
			return input;
		}
		protected int GetScaledHeight(int input) {
			SizeF scale = GetScaleFactor();
			if(scale.Height == 1.0f)
				return input;
			return (int)(input * scale.Height);
		}
		SizeF GetScaleFactor() { return Element.GetScaleFactor(); }
		bool IsControlScaled { get { return Element.GetScaleFactor() != new SizeF(1, 1); } }
		protected internal Size ImageNotCroppedSize {
			get { return imageNotCroppedSize; }
			set { imageNotCroppedSize = value; }
		}
		Size imageNotCroppedSize;
		protected internal virtual Size GetImageSize() {
			Size boundsSize = ImageSize != Size.Empty ? ImageSize : ItemInfo.ContentBounds.Size;
			return Image == null ? Size.Empty : TileItemImageScaleModeHelper.ScaleImage(boundsSize, Image.Size, ImageScaleMode, out imageNotCroppedSize);
		}
		public TileItemContentAlignment ImageAlignment {
			get { return GetImageAlignment(); }
		}
		protected virtual TileItemContentAlignment GetImageAlignment() {
			TileItemContentAlignment res = Element.ImageAlignment != TileItemContentAlignment.Default ? Element.ImageAlignment : Item.ImageAlignment;
			if(res == TileItemContentAlignment.Default) res = ControlInfo.Owner.Properties.ItemImageAlignment;
			return res == TileItemContentAlignment.Default ? TileItemContentAlignment.BottomLeft : res;
		}
		protected internal virtual void UpdateItemsPanel() {
			ItemsPanel = new Items2Panel();
			ItemsPanel.HorizontalIndent = Element.ImageToTextIndent;
			ItemsPanel.VerticalIndent = Element.ImageToTextIndent;
			ItemsPanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content1Location = TileItemCalculator.ImageToTextAlignmentToItemLocation(ImageToTextAlignment);
			Size panelSize = ItemsPanel.CalcBestSize(GetImageSize(), TextBounds.Size);
			Rectangle panelRect = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, panelSize, ImageAlignment, ImageLocation, Element.StretchHorizontal, Element.StretchVertical);
			Rectangle content1Bounds = Rectangle.Empty, content2Bounds = Rectangle.Empty;
			ItemsPanel.ArrangeItems(panelRect, GetImageSize(), TextBounds.Size, ref content1Bounds, ref content2Bounds);
			ImageBounds = content1Bounds;
			TextBounds = content2Bounds;
		}
		protected internal virtual void UpdateContentElementPaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { GetAppearance(), ItemInfo.GetAppearance(), ItemInfo.GroupInfo.ControlInfo.GetAppearance(ItemInfo) }, ItemInfo.DefaultAppearance);
			if(res.TextOptions.WordWrap == WordWrap.Default)
				res.TextOptions.WordWrap = WordWrap.Wrap;
			if(res.TextOptions.Trimming == Trimming.Default)
				res.TextOptions.Trimming = Trimming.EllipsisCharacter;
			PaintAppearance = res;
			PaintAppearanceBackground = GetCurrentBackgroundAppearance();
		}
		protected virtual StringInfo CalcHtmlText(Size maxTextSize) {
			return CalcHtmlText(new Rectangle(0, 0, maxTextSize.Width, maxTextSize.Height));
		}
		protected virtual StringInfo CalcHtmlText() {
			return CalcHtmlText(new Rectangle(0, 0, ItemInfo.ContentBounds.Width, 0));
		}
		protected virtual StringInfo CalcHtmlTextConsiderImageBounds() {
			return CalcHtmlText(new Rectangle(Point.Empty, new Size(ItemInfo.ContentBounds.Right - TextBounds.X, ItemInfo.ContentBounds.Height)));
		}
		protected virtual StringInfo CalcHtmlText(Rectangle bounds) {
			return StringPainter.Default.Calculate(ItemInfo.GroupInfo.ControlInfo.GInfo.Graphics, PaintAppearance, Text, bounds, TileItemViewInfo.GetGdiPaint(ItemInfo.ControlInfo));
		}
		protected virtual Size CalcSimpleTextSize(Size maxTextSize) {
			SizeF size = ItemInfo.GroupInfo.ControlInfo.GInfo.Graphics.MeasureString(Text, PaintAppearance.Font, maxTextSize.Width);
			return new Size((int)size.Width + 1, (int)size.Height + 1);
		}
		protected virtual Size CalcSimpleTextSize() {
			SizeF size = ItemInfo.GroupInfo.ControlInfo.GInfo.Graphics.MeasureString(Text, PaintAppearance.Font, ItemInfo.ContentBounds.Width);
			return new Size((int)size.Width + 1, (int)size.Height + 1);
		}
		protected internal AppearanceObject GetAppearance() {
			if(ItemInfo.IsPressed)
				return GetAppearancePressed();
			if(ItemInfo.IsHovered)
				return GetAppearanceHovered();
			if(ItemInfo.IsSelected)
				return GetAppearanceSelected();
			return Element.Appearance.Normal;
		}
		protected virtual AppearanceObject GetAppearancePressed() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { 
					Element.Appearance.Pressed, ItemInfo.Item.AppearanceItem.Pressed, ItemInfo.ControlInfo.Owner.AppearanceItem.Pressed,
					Element.Appearance.Normal, ItemInfo.Item.AppearanceItem.Normal, ItemInfo.ControlInfo.Owner.AppearanceItem.Normal });
			return res;
		}
		protected virtual AppearanceObject GetAppearanceHovered() {
			AppearanceObject elementSelected = new AppearanceObject();
			AppearanceObject itemSelected = new AppearanceObject();
			AppearanceObject controlSelected = new AppearanceObject();
			if(ItemInfo.IsSelected) {
				elementSelected = Element.Appearance.Selected;
				itemSelected = ItemInfo.Item.AppearanceItem.Selected;
				controlSelected = ItemInfo.ControlInfo.Owner.AppearanceItem.Selected;
			}
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { 
					Element.Appearance.Hovered, ItemInfo.Item.AppearanceItem.Hovered, ItemInfo.ControlInfo.Owner.AppearanceItem.Hovered,
					elementSelected, itemSelected, controlSelected,
					Element.Appearance.Normal, ItemInfo.Item.AppearanceItem.Normal, ItemInfo.ControlInfo.Owner.AppearanceItem.Normal });
			return res;
		}
		protected virtual AppearanceObject GetAppearanceSelected() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { 
					Element.Appearance.Selected, ItemInfo.Item.AppearanceItem.Selected, ItemInfo.ControlInfo.Owner.AppearanceItem.Selected,
					Element.Appearance.Normal, ItemInfo.Item.AppearanceItem.Normal, ItemInfo.ControlInfo.Owner.AppearanceItem.Normal });
			return res;
		}
		public TileControlImageToTextAlignment ImageToTextAlignment {
			get {
				TileControlImageToTextAlignment res = Element.ImageToTextAlignment != TileControlImageToTextAlignment.Default ? Element.ImageToTextAlignment : Item.ImageToTextAlignment;
				return res != TileControlImageToTextAlignment.Default ? res : TileControlImageToTextAlignment.None;
			}
		}
		protected virtual Size GetMaxTextSize() {
			Point imageSpace = Element.ImageAlignment == TileItemContentAlignment.Manual ? ImageLocation : Point.Empty;
			Point textSpace = Element.TextAlignment == TileItemContentAlignment.Manual ? Location : Point.Empty;
			if(MaxWidth > 0) {
				if((ItemInfo.ContentBounds.Width - textSpace.X) < MaxWidth)
					return new Size(ItemInfo.ContentBounds.Width - textSpace.X, ItemInfo.ContentBounds.Height - textSpace.Y);
				else
					return new Size(MaxWidth, ItemInfo.ContentBounds.Height - textSpace.Y);
			}
			if(ImageToTextAlignment == TileControlImageToTextAlignment.None) {
				return new Size(ItemInfo.ContentBounds.Width - textSpace.X, ItemInfo.ContentBounds.Height - textSpace.Y);
			}
			Size imageSize = GetImageSize();
			if(ImageToTextAlignment == TileControlImageToTextAlignment.Top || ImageToTextAlignment == TileControlImageToTextAlignment.Bottom) {
				return new Size(ItemInfo.ContentBounds.Width, ItemInfo.ContentBounds.Height - imageSize.Height - imageSpace.Y - Element.ImageToTextIndent);
			}
			return new Size(ItemInfo.ContentBounds.Width - imageSize.Width - imageSpace.X - Element.ImageToTextIndent, ItemInfo.ContentBounds.Height);
		}
		protected internal void ForceCalcViewInfo() {
			bool releaseGraphics = false;
			if(ItemInfo.GroupInfo.ControlInfo.GInfo.Graphics == null) {
				releaseGraphics = true;
				ItemInfo.GroupInfo.ControlInfo.GInfo.AddGraphics(null);
			}
			IsReady = false;
			CalcViewInfo();
			if(releaseGraphics) {
				ItemInfo.GroupInfo.ControlInfo.GInfo.ReleaseGraphics();
			}
		}
		protected virtual Rectangle CheckUpdateTextBoundsSize(Rectangle textBounds) {
			if(Height > 0)
				textBounds.Height = Height;
			if(Width > 0)
				textBounds.Width = Width;
			return textBounds;
		}
		public bool CanDrawBackground {
			get {
				if(BackgroundBounds.IsEmpty || PaintAppearanceBackground == null) return false;
				return !PaintAppearanceBackground.BackColor.IsEmpty || !PaintAppearanceBackground.BackColor2.IsEmpty;
			}
		}
		public AppearanceObject GetCurrentBackgroundAppearance() {
			if(ItemInfo.IsPressed) {
				AppearanceObject res = new AppearanceObject();
				AppearanceHelper.Combine(res, new AppearanceObject[] { Element.Appearance.Pressed, Element.Appearance.Normal });
				return res;
			}
			if(ItemInfo.IsHovered) {
				AppearanceObject elementSelected = new AppearanceObject();
				if(ItemInfo.IsSelected)
					elementSelected = Element.Appearance.Selected;
				AppearanceObject res = new AppearanceObject();
				AppearanceHelper.Combine(res, new AppearanceObject[] { Element.Appearance.Hovered, elementSelected, Element.Appearance.Normal });
				return res;
			}
			if(ItemInfo.IsSelected) {
				AppearanceObject res = new AppearanceObject();
				AppearanceHelper.Combine(res, new AppearanceObject[] { Element.Appearance.Selected, Element.Appearance.Normal });
				return res;
			}
			return Element.Appearance.Normal;
		}
		protected virtual Rectangle CalcBackgroundBounds(Rectangle bounds) {
			if(Element.StretchHorizontal) {
				bounds.X = ItemInfo.Bounds.Left;
				bounds.Width = ItemInfo.Bounds.Width;
			}
			if(Element.StretchVertical) {
				bounds.Y = ItemInfo.Bounds.Top;
				bounds.Height = ItemInfo.Bounds.Height;
			}
			return bounds;
		}
		protected virtual TileItemElementViewInfo GetAnchorElementInfo() {
			if(Element.AnchorElement != null) {
				return GetElementInfoByElement(Element.AnchorElement);
			}
			return null;
		}
		TileItemElementViewInfo anchorElementInfo;
		TileItemElementViewInfo AnchorElementInfo {
			get {
				if(anchorElementInfo == null)
					anchorElementInfo = GetAnchorElementInfo();
				return anchorElementInfo;
			}
		}
		TileItemElementViewInfo GetElementInfoByElement(TileItemElement element) {
			if(ItemInfo == null || ItemInfo.Elements == null) return null;
			foreach(var elementInfo in ItemInfo.Elements) {
				if(elementInfo.Element.Equals(element))
					return elementInfo;
			}
			return null;
		}
		bool ShouldCalcEmptyText {
			get {
				return string.IsNullOrEmpty(Text) &&
					   (AnchorElementInfo != null || Element.AnchorChildsList.Count > 0) &&
					   Image == null;
			}
		}
		public virtual void CalcViewInfo() {
			if(IsReady) return;
			if(AnchorElementInfo != null && !AnchorElementInfo.IsReady)
				AnchorElementInfo.CalcViewInfo();
			UpdateContentElementPaintAppearance();
			if(!string.IsNullOrEmpty(Element.Text)) {
				if(ItemInfo.AllowHtmlText) {
					TextBounds = new Rectangle(Point.Empty, GetMaxTextSize());
					TextBounds = CheckUpdateTextBoundsSize(TextBounds);
					StringInfo = CalcHtmlText(TextBounds.Size);
					Rectangle rect = CheckUpdateTextBoundsSize(StringInfo.Bounds);
					StringInfo = CalcHtmlText(rect);
					rect = CheckUpdateTextBoundsSize(StringInfo.Bounds);
					TextBounds = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, rect.Size, GetAlignment(Item.Elements.IndexOf(Element), Element.TextAlignment), Location, Element.StretchHorizontal, Element.StretchVertical);
					StringInfo.SetLocation(TextBounds.Location);
					BackgroundBounds = CalcBackgroundBounds(TextBounds);
				}
				else {
					TextBounds = new Rectangle(Point.Empty, CalcSimpleTextSize(GetMaxTextSize()));
					TextBounds = CheckUpdateTextBoundsSize(TextBounds);
					TextBounds = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, TextBounds.Size, GetAlignment(Item.Elements.IndexOf(Element), Element.TextAlignment), Location, Element.StretchHorizontal, Element.StretchVertical);
					BackgroundBounds = CalcBackgroundBounds(TextBounds);
				}
			}
			else {
				if(Width > 0 || Height > 0) {
					var emptyTextBounds = CheckUpdateTextBoundsSize(Rectangle.Empty);
					emptyTextBounds = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, emptyTextBounds.Size, GetAlignment(Item.Elements.IndexOf(Element), Element.TextAlignment), Location, Element.StretchHorizontal, Element.StretchVertical);
					BackgroundBounds = CalcBackgroundBounds(emptyTextBounds);
				}
				if(ShouldCalcEmptyText)
					CalcEmptyText();
			}
			ImageBounds = new Rectangle(Point.Empty, GetImageSize());
			if(ImageToTextAlignment == TileControlImageToTextAlignment.None)
				ImageBounds = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, ImageBounds.Size, ImageAlignment, ImageLocation, false, false);
			else {
				UpdateItemsPanel();
				if(TextBounds.Right > ItemInfo.ContentBounds.Right ||
					TextBounds.Left < ItemInfo.ContentBounds.Left) {
					if(TextBounds.Right > ItemInfo.ContentBounds.Right)
						TextBounds = new Rectangle(TextBounds.X, TextBounds.Y, ItemInfo.ContentBounds.Right - TextBounds.X, ItemInfo.ContentBounds.Height);
					else
						TextBounds = new Rectangle(ItemInfo.ContentBounds.X, TextBounds.Y, TextBounds.Right - ItemInfo.ContentBounds.Left, ItemInfo.ContentBounds.Height);
					StringInfo = CalcHtmlTextConsiderImageBounds();
					TextBounds = StringInfo.Bounds;
					UpdateItemsPanel();
				}
				if(ItemInfo.AllowHtmlText) {
					StringInfo = CalcHtmlText(TextBounds);
				}
				BackgroundBounds = CalcBackgroundBounds(TextBounds);
			}
			LayoutToAnchor();
			EnsureTextBoundsAfterAnchoring();
			EnsureRTL();
			ImageContentBounds = ImageBounds;
			IsReady = true;
		}
		void EnsureTextBoundsAfterAnchoring() {
			if(AnchorElementInfo == null) return;
			if(MaxWidth > 0 || Width > 0) return;
			if(TextBounds.Right <= ItemInfo.ContentBounds.Right &&
				TextBounds.Left >= ItemInfo.ContentBounds.Left) return;
			if(TextBounds.Right > ItemInfo.ContentBounds.Right)
				TextBounds = new Rectangle(TextBounds.X, TextBounds.Y, ItemInfo.ContentBounds.Right - TextBounds.X, ItemInfo.ContentBounds.Height);
			else
				TextBounds = new Rectangle(ItemInfo.ContentBounds.X, TextBounds.Y, TextBounds.Right - ItemInfo.ContentBounds.Left, ItemInfo.ContentBounds.Height);
			StringInfo = CalcHtmlText(TextBounds);
			TextBounds = new Rectangle(TextBounds.Location, StringInfo.Bounds.Size);
			if(Image != null) UpdateItemsPanel();
			if(ItemInfo.AllowHtmlText) {
				StringInfo = CalcHtmlText(TextBounds);
			}
			BackgroundBounds = CalcBackgroundBounds(TextBounds);
			LayoutToAnchor();
		}
		protected virtual void EnsureRTL() {
			if(!IsRightToLeft ||
			   ((IAnchored)this).AnchorElement != null) return;
			if(!TextBounds.IsEmpty) {
				TextBounds = ReflectRect(TextBounds);
				if(StringInfo != null)
					StringInfo.SetLocation(TextBounds.Location);
			}
			if(!ImageBounds.IsEmpty) {
				ImageBounds = ReflectRect(ImageBounds);
			}
			if(!BackgroundBounds.IsEmpty) {
				BackgroundBounds = ReflectRect(BackgroundBounds);
			}
		}
		Rectangle ReflectRect(Rectangle inputRect) {
			int relativeX = inputRect.X - ItemInfo.ContentBounds.Left;
			int newX = ItemInfo.ContentBounds.Right - relativeX - inputRect.Width;
			return new Rectangle(new Point(newX, inputRect.Y), inputRect.Size);
		}
		void CalcEmptyText() {
			TextBounds = new Rectangle(0, 0, 0, PaintAppearance.CalcTextSizeInt(ItemInfo.ControlInfo.GInfo.Graphics, "Wg", 0).Height);
			TextBounds = CheckUpdateTextBoundsSize(TextBounds);
			TextBounds = TileItemCalculator.LayoutContent(ItemInfo.ContentBounds, TextBounds.Size, GetAlignment(Item.Elements.IndexOf(Element), Element.TextAlignment), Location, Element.StretchHorizontal, Element.StretchVertical);
			BackgroundBounds = CalcBackgroundBounds(TextBounds);
		}
		private void LayoutToAnchor() {
			Rectangle result = AnchorLayoutHelper.LayoutToAnchor(this);
			int dx = EntireElementBounds.X - result.X;
			int dy = EntireElementBounds.Y - result.Y;
			TextBounds = new Rectangle(new Point(TextBounds.Location.X - dx, TextBounds.Location.Y - dy), TextBounds.Size);
			ImageBounds = new Rectangle(new Point(ImageBounds.Location.X - dx, ImageBounds.Location.Y - dy), ImageBounds.Size);
			if(!string.IsNullOrEmpty(Text) || ShouldCalcEmptyText)
				BackgroundBounds = CalcBackgroundBounds(TextBounds);
			if(ItemInfo.AllowHtmlText && StringInfo != null) {
				StringInfo.SetLocation(TextBounds.Location);
			}
		}
		protected virtual Rectangle CalcEntireElementBounds() {
			if(!TextBounds.Size.IsEmpty && !ImageBounds.Size.IsEmpty) {
				int x = Math.Min(TextBounds.X, ImageBounds.X);
				int y = Math.Min(TextBounds.Y, ImageBounds.Y);
				int xr = Math.Max(TextBounds.Right, ImageBounds.Right);
				int yr = Math.Max(TextBounds.Bottom, ImageBounds.Bottom);
				return new Rectangle(x, y, xr - x, yr - y);
			}
			if(!TextBounds.Size.IsEmpty) return TextBounds;
			if(!ImageBounds.Size.IsEmpty) return ImageBounds;
			return Rectangle.Empty;
		}
		protected internal virtual void MakeOffset(int deltaX, int deltaY) {
			TextBounds = TileItemCalculator.OffsetRect(TextBounds, deltaX, deltaY);
			ImageBounds = TileItemCalculator.OffsetRect(ImageBounds, deltaX, deltaY);
			ImageContentBounds = TileItemCalculator.OffsetRect(ImageContentBounds, deltaX, deltaY);
			BackgroundBounds = TileItemCalculator.OffsetRect(BackgroundBounds, deltaX, deltaY);
			if(ItemInfo.AllowHtmlText && StringInfo != null) {
				StringInfo.SetLocation(TextBounds.Location);
			}
		}
		Rectangle IAnchored.AnchorBounds {
			get {
				return AnchorElementInfo == null ? Rectangle.Empty : (AnchorElementInfo as IAnchored).Bounds;
			}
		}
		IAnchored IAnchored.AnchorElement {
			get { return AnchorElementInfo; }
		}
		AnchorAlignment IAnchored.AnchorAlignment {
			get { return Element.AnchorAlignment; }
		}
		int IAnchored.AnchorIndent {
			get { return Element.AnchorIndent; }
		}
		Point IAnchored.AnchorOffset {
			get { return Element.AnchorOffset; }
		}
		Rectangle IAnchored.Bounds {
			get { return EntireElementBounds; }
		}
		bool IAnchored.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		public virtual bool ShouldDrawImageBorder {
			get { return Image != null && Element.ImageBorder == TileItemElementImageBorderMode.SingleBorder; }
		}
		public virtual bool UseAppearanceForImageBorder {
			get { return Element.ImageBorderColor.IsEmpty; }
		}
		public virtual Pen ImageBorderPen {
			get { return new Pen(Element.ImageBorderColor); }
		}
	}
	public class TileItemViewInfo : ISupportContextItems {
		public TileItem Item { get; private set; }
		public bool ShouldProcessItem {
			get { return Item.Visible; }
		}
		public TileGroupViewInfo GroupInfo { get; protected internal set; }
		public TileItemViewInfo(TileItem item) {
			Item = item;
			DropSide = TileControlDropSide.None;
			RenderImageOpacity = 1.0f;
			HoverOpacity = 0.0f;
			DrawImage = true;
			DrawText = true;
			DrawBackgroundImage = true;
			ForceIsHovered = null;
			AllowItemCheck = true;
		}
		public bool AllowGlyphSkinning {
			get {
				return Item.AllowGlyphSkinning != DefaultBoolean.Default ?
					Item.AllowGlyphSkinning.ToBoolean(false) : ControlInfo.Owner.AllowGlyphSkinning;
			}
		}
		public Size Size {
			get { return ControlInfo.GetItemSize(this); }
		}
		internal List<Point> RandomSegments { get; set; }
		protected internal virtual void GenerateRandomSegments() {
			List<Point> list = new List<Point>();
			RandomSegments = new List<Point>(SegmentColumn * SegmentRow);
			for(int i = 0; i < SegmentColumn; i++) {
				for(int j = 0; j < SegmentRow; j++) {
					list.Add(new Point(i, j));
				}
			}
			Random r = new Random();
			while(list.Count > 0) {
				int t = r.Next(list.Count);
				RandomSegments.Add(list[t]);
				list.RemoveAt(t);
			}
		}
		protected internal int SegmentSize { get { return ControlInfo.ItemSize / 5; } }
		protected internal int SegmentRow { get { return GetCount(Bounds.Height, SegmentSize); } }
		protected internal int SegmentColumn { get { return GetCount(Bounds.Width, SegmentSize); } }
		int GetCount(int size, int itemSize) {
			int res = size / itemSize;
			if(size % itemSize != 0)
				res += 1;
			return res;
		}
		public TileItemContentAnimationType CurrentContentAnimationType { get; internal set; }
		public TileControlViewInfo ControlInfo { get { return GroupInfo.ControlInfo; } }
		public Image BackgroundImage {
			get {
				if(Item != null)
					return Item.GetDisplayBackgroundImage();
				return null;
			}
		}
		public bool UseOptimizedBackgroundImage {
			get { return Item != null && Item.OptimizedBackgroundImage != null; }
		}
		public int LargeItemWidth {
			get { return GroupInfo.ControlInfo.Owner.Properties.LargeItemWidth; }
		}
		public int RowCount {
			get {
				switch(Item.ItemSize) {
					case TileItemSize.Large:
						return Math.Max(2, Item.RowCount);
					case TileItemSize.Wide:
						return Item.RowCount;
					default: return 1;
				}
			}
		}
		public Color SelectedColor { get { return Color.FromArgb(153, 1, 2, 0); } }
		public Color HoverColor {
			get {
				return Color.FromArgb((int)(0.75f * 255.0f * HoverOpacity), 255, 255, 255);
			}
		}
		ImageAttributes hoverAttributes;
		public ImageAttributes HoverAttributes {
			get {
				if(hoverAttributes == null) {
					hoverAttributes = new ImageAttributes();
				}
				ColorMatrix mat = new ColorMatrix();
				mat.Matrix33 = HoverOpacity;
				hoverAttributes.SetColorMatrix(mat);
				return hoverAttributes;
			}
		}
		protected internal virtual SkinElement GetItemSelectedElement() {
			SkinElement elem = EditorsSkins.GetSkin(GroupInfo.ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTileItemSelected];
			if(elem == null)
				elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinTileItemSelected];
			return elem;
		}
		protected internal virtual SkinElement GetItemCheckedElement() {
			SkinElement elem = EditorsSkins.GetSkin(GroupInfo.ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTileItemChecked];
			if(elem == null)
				elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinTileItemChecked];
			return elem;
		}
		Rectangle GetInflatedRect(SkinElement elem) {
			Rectangle rect = Bounds;
			int indent = elem.Properties.GetInteger(EditorsSkins.OptTileItemBorderIndent);
			rect.Inflate(indent, indent);
			return rect;
		}
		public virtual SkinElementInfo GetItemHoveredInfo() {
			SkinElement elem = GetItemSelectedElement();
			if(elem == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(elem, GetInflatedRect(elem));
			info.ImageIndex = 0;
			if((IsHovered && HoverOpacity < 1.0f) || (!IsHovered && HoverOpacity > 0.0f))
				info.Attributes = HoverAttributes;
			return info;
		}
		public virtual SkinElementInfo GetItemSelectedInfo() {
			SkinElement elem = GetItemSelectedElement();
			if(elem == null)
				return null;
			SkinElementInfo info = new SkinElementInfo(elem, GetInflatedRect(elem));
			if(IsSelected)
				info.ImageIndex = 1;
			return info;
		}
		public virtual SkinElementInfo GetItemCheckedInfo() {
			SkinElement elem = GetItemCheckedElement();
			if(elem == null)
				return null;
			var info = new SkinElementInfo(elem, GetInflatedRect(elem));
			info.RightToLeft = ControlInfo.IsRightToLeft;
			return info;
		}
		public bool IsKeyboardSelected { get { return Item.Control.Navigator.SelectedItem != null && Item.Control.Navigator.SelectedItem.Item.Item == Item; } }
		public bool IsSelected { get { return GroupInfo.ControlInfo.Owner.Properties.AllowSelectedItem && GroupInfo.ControlInfo.Owner.SelectedItem == Item; } }
		internal bool? ForceIsHovered { get; set; }
		public virtual bool IsHovered { get { return ForceIsHovered.HasValue ? ForceIsHovered.Value : GroupInfo.ControlInfo.HoverInfo.InItem && GroupInfo.ControlInfo.HoverInfo.ItemInfo.Item == Item; } }
		public float HoverOpacity { get; internal set; }
		public bool IsVisible { get { return GroupInfo.ControlInfo.Bounds.IntersectsWith(Bounds); } }
		public bool IsFullyVisible { get { return GroupInfo.ControlInfo.GroupsContentBounds.Contains(Bounds); } }
		public bool IsChangingContent { get; internal set; }
		public virtual bool IsPressed { get { return GroupInfo.ControlInfo.PressedInfo.InItem && GroupInfo.ControlInfo.PressedInfo.ItemInfo == this; } }
		bool forceUseBoundsAnimation;
		internal bool ForceUseBoundsAnimation {
			get { return forceUseBoundsAnimation; }
			set { forceUseBoundsAnimation = value; }
		}
		public virtual Rectangle SelectionBounds {
			get {
				return new Rectangle(Bounds.X - 2, Bounds.Y - 2, Bounds.Width + 5, Bounds.Height + 5);
			}
		}
		public Rectangle Bounds { get; internal set; }
		internal Rectangle PrevBounds { get; private set; }
		public Point DragOrigin { get { return new Point(RenderImageBounds.X + RenderImageBounds.Width / 2, RenderImageBounds.Y + RenderImageBounds.Height / 2); } }
		public Point Origin { get { return new Point(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2); } }
		protected internal Rectangle PrevRenderImageBounds { get; set; }
		public bool IsHoveringAnimation { get; set; }
		public bool IsInTransition { get; internal set; }
		public Rectangle NextTransitionBounds { get; internal set; }
		public Rectangle PrevTransitionBounds { get; internal set; }
		public List<TileItemElementViewInfo> Elements { get; protected internal set; }
		internal List<TileItemElementViewInfo> PrevItems { get; set; }
		public virtual bool AllowHtmlText {
			get {
				DefaultBoolean res = Item.AllowHtmlText;
				if(res == DefaultBoolean.Default)
					return GroupInfo.ControlInfo.Owner.Properties.AllowHtmlDraw;
				return res == DefaultBoolean.False ? false : true;
			}
		}
		public Rectangle PressedBounds {
			get {
				Rectangle rect = Bounds;
				rect.Inflate(-ControlInfo.PressedItemBoundsDelta, -ControlInfo.PressedItemBoundsDelta);
				return rect;
			}
		}
		public Rectangle DragStateBounds {
			get {
				Rectangle rect = Bounds;
				rect.Inflate(-ControlInfo.DragItemBoundsScaleDelta, -ControlInfo.DragItemBoundsScaleDelta);
				return rect;
			}
		}
		internal void SavePrevState() {
			PrevBounds = Bounds;
			PrevRenderImageBounds = RenderImageBounds;
			PrevPosition = Position.Clone();
			PrevItems = new List<TileItemElementViewInfo>();
			foreach(TileItemElementViewInfo info in Elements) {
				PrevItems.Add(info);
			}
		}
		protected virtual internal bool ShouldUseTransition {
			get {
				if(ForceUseBoundsAnimation)
					return false;
				if(!ControlInfo.IsHorizontal) {
					if(PrevPosition.Row != Position.Row)
						return PrevPosition.Column != Position.GroupColumn;
					return false;
				}
				if(IsLarge)
					return PrevPosition.GroupColumn != Position.GroupColumn;
				TileItemViewInfo itemInfo = GroupInfo.Items.GetItem(PrevPosition.Row, PrevPosition.GroupColumn, 0);
				return PrevPosition.GroupColumn != Position.GroupColumn || (!IsLarge && PrevBounds.X != Bounds.X && PrevPosition.Row != Position.Row);
			}
		}
		bool shouldRemoveFromCache = true;
		protected internal bool ShouldRemoveFromCache {
			get { return shouldRemoveFromCache; }
			set { shouldRemoveFromCache = value; }
		}
		public bool AllowAnimation {
			get {
				return Item != null ? !Item.IsDisposed : false;
			}
		}
		public virtual bool AllowSelectAnimation {
			get {
				return Item != null ? Item.GetAllowSelectAnimation() : false;
			}
		}
		public TileItemPosition Position { get; set; }
		public TileItemPosition PrevPosition { get; set; }
		public TileControlDropSide DropSide { get; internal set; }
		public Rectangle ContentBounds { get; private set; }
		public Rectangle DropBounds { get; private set; }
		public Rectangle OuterBounds { get; private set; }
		bool useRenderImage = false;
		public bool UseRenderImage {
			get { return useRenderImage; }
			set {
				if(!value && NeedUpdateAppearance) {
					ClearAppearance();
					NeedUpdateAppearance = false;
				}
				useRenderImage = value;
			}
		}
		public bool AllowItemCheck { get; set; }
		protected internal bool NeedUpdateAppearance { get; set; }
		internal bool DrawImage { get; set; }
		internal bool DrawText { get; set; }
		internal bool DrawBackgroundImage { get; set; }
		internal bool AnimateImage { get; set; }
		public bool RenderToBitmap { get; set; }
		bool animateBackgroundImage;
		internal bool AnimateBackgroundImage {
			get { return animateBackgroundImage; }
			set { animateBackgroundImage = value; }
		}
		bool animateText;
		internal bool AnimateText {
			get { return animateText; }
			set { animateText = value; }
		}
		internal bool DrawOnlyText { get { return DrawText && !DrawImage && !DrawBackgroundImage; } }
		internal bool DrawOnlyBackgroundImage { get { return !DrawText && !DrawImage && DrawBackgroundImage; } }
		bool isDragging = false;
		public bool IsDragging {
			get { return isDragging; }
			set {
				isDragging = value;
				RenderImageOpacity = IsDragging ? ControlInfo.DragItemOpacity : 1.0f;
			}
		}
		public float RenderImageOpacity { get; internal set; }
		public Image RenderImage {
			get { return Item.RenderImage; }
			set { Item.RenderImage = value; }
		}
		public Image PrevRenderImage {
			get { return Item.PrevRenderImage; }
			set { Item.PrevRenderImage = value; }
		}
		Rectangle renderImageBounds;
		public Rectangle RenderImageBounds {
			get { return renderImageBounds; }
			set {
				if(IsDragging && value.Width == 104) {
				}
				renderImageBounds = value;
			}
		}
		Rectangle OptimizedImageBounds { get { return IsPressed ? PressedBounds : DragStateBounds; } }
		bool useOptimizedRenderImage;
		public bool UseOptimizedRenderImage {
			get { return useOptimizedRenderImage; }
			set {
				if(value)
					OptimizedRenderImage = GetOptimizedRenderImage(RenderImage, OptimizedImageBounds);
				else
					OptimizedRenderImage = null;
				useOptimizedRenderImage = value;
			}
		}
		Image optimizedRenderImage;
		public Image OptimizedRenderImage {
			get { return optimizedRenderImage; }
			set {
				if(optimizedRenderImage == value) return;
				if(optimizedRenderImage != null) optimizedRenderImage.Dispose();
				optimizedRenderImage = value;
			}
		}
		internal Image GetOptimizedRenderImage(Image image, Rectangle bounds) {
			Bitmap imgResult = new Bitmap(bounds.Width, bounds.Height);
			using(Graphics g = Graphics.FromImage(imgResult)) {
				g.DrawImage(image, new Rectangle(0, 0, bounds.Width, bounds.Height));
			}
			return imgResult;
		}
		public Rectangle BackgroundImageContentBounds { get; private set; }
		public virtual Color GetSelectionColor() {
			return Item.Control.SelectionColor;
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) {
					paintAppearance = CreatePaintAppearance();
				}
				return paintAppearance;
			}
		}
		AppearanceObject paintAppearanceForDisabledItems;
		public AppearanceObject PaintAppearanceForDisabledItems {
			get {
				if(paintAppearanceForDisabledItems == null) {
					paintAppearanceForDisabledItems = CreatePaintAppearance();
					paintAppearanceForDisabledItems.BackColor = Color.FromArgb(127, 255, 255, 255);
					paintAppearanceForDisabledItems.BackColor2 = Color.FromArgb(127, 255, 255, 255);
				}
				return paintAppearanceForDisabledItems;
			}
		}
		public virtual void ForceUpdateAppearanceColors() {
			this.defaultAppearance = null;
			this.paintAppearance = null;
			if(this.Item.Control == null)
				return;
			UpdateContentElementsPaintAppearance();
			ForceUpdateAppearanceColorsCore();
		}
		protected virtual void UpdateContentElementsPaintAppearance() {
			for(int i = 0; i < Elements.Count; i++) {
				Elements[i].UpdateContentElementPaintAppearance();
				Elements[i].ForceCalcViewInfo();
			}
		}
		protected internal void ForceUpdateAppearanceColorsCore() {
			if(AllowHtmlText) {
				foreach(TileItemElementViewInfo info in Elements) {
					if(info != null && info.StringInfo != null)
						info.StringInfo.UpdateAppearanceColors(info.PaintAppearance);
				}
			}
		}
		AppearanceDefault defaultAppearance;
		public AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			if(IsHovered)
				return new AppearanceDefault(Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 23, 147, 75), Color.FromArgb(255, 48, 169, 101), Color.FromArgb(255, 27, 173, 92), AppearanceObject.DefaultFont);
			return new AppearanceDefault(Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 23, 147, 75), Color.FromArgb(255, 48, 169, 101), Color.FromArgb(255, 27, 173, 92), AppearanceObject.DefaultFont);
		}
		public virtual AppearanceObject GetAppearance() {
			if(IsPressed)
				return GetPressedAppearance();
			if(IsHovered)
				return GetHoveredAppearance();
			if(IsSelected)
				return GetSelectedAppearance();
			return GetNormalAppearance();
		}
		protected virtual AppearanceObject GetPressedAppearance() {
			AppearanceObject frameAppearance = new AppearanceObject();
			if(Item.CurrentFrame != null) frameAppearance = Item.CurrentFrame.Appearance;
			AppearanceObject extraAppearance = GetPressedAppearanceExt();
			AppearanceObject pressed = new AppearanceObject();
			AppearanceHelper.Combine(pressed, new AppearanceObject[] { 
					Item.AppearanceItem.Pressed, ControlInfo.Owner.AppearanceItem.Pressed,
					frameAppearance,
					extraAppearance,
					Item.AppearanceItem.Normal, ControlInfo.Owner.AppearanceItem.Normal }, DefaultAppearance);
			return pressed;
		}
		protected virtual AppearanceObject GetPressedAppearanceExt() { return AppearanceObject.EmptyAppearance; }
		protected virtual AppearanceObject GetSelectedAppearanceExt() { return AppearanceObject.EmptyAppearance; }
		protected virtual AppearanceObject GetHoveredAppearanceExt() { return AppearanceObject.EmptyAppearance; }
		protected virtual AppearanceObject GetNormalAppearanceExt() { return AppearanceObject.EmptyAppearance; }
		protected virtual AppearanceObject GetSelectedAppearance() {
			AppearanceObject frameAppearance = new AppearanceObject();
			if(Item.CurrentFrame != null) frameAppearance = Item.CurrentFrame.Appearance;
			AppearanceObject extraAppearance = GetSelectedAppearanceExt();
			AppearanceObject selected = new AppearanceObject();
			AppearanceHelper.Combine(selected, new AppearanceObject[] { 
				Item.AppearanceItem.Selected, ControlInfo.Owner.AppearanceItem.Selected,
				frameAppearance, 
				extraAppearance,
				Item.AppearanceItem.Normal, ControlInfo.Owner.AppearanceItem.Normal });
			return selected;
		}
		protected virtual AppearanceObject GetHoveredAppearance() {
			AppearanceObject frameAppearance = new AppearanceObject();
			AppearanceObject itemSelected = new AppearanceObject();
			AppearanceObject controlSelected = new AppearanceObject();
			AppearanceObject extraAppearance = GetHoveredAppearanceExt();
			AppearanceObject res = new AppearanceObject();
			if(IsSelected) {
				itemSelected = Item.AppearanceSelected;
				controlSelected = ControlInfo.Owner.AppearanceItem.Selected;
			}
			if(Item.CurrentFrame != null) frameAppearance = Item.CurrentFrame.Appearance;
			AppearanceHelper.Combine(res, new AppearanceObject[] { 
				Item.AppearanceItem.Hovered, ControlInfo.Owner.AppearanceItem.Hovered,
				frameAppearance, itemSelected, controlSelected,
				extraAppearance,
				Item.AppearanceItem.Normal, ControlInfo.Owner.AppearanceItem.Normal });
			return res;
		}
		protected virtual AppearanceObject GetNormalAppearance() {
			AppearanceObject frameAppearance = new AppearanceObject();
			if(Item.CurrentFrame != null) frameAppearance = Item.CurrentFrame.Appearance;
			AppearanceObject extraAppearance = GetNormalAppearanceExt();
			AppearanceObject normal = new AppearanceObject();
			AppearanceHelper.Combine(normal, new AppearanceObject[] { frameAppearance, extraAppearance, Item.AppearanceItem.Normal });
			return normal;
		}
		protected virtual AppearanceObject CreatePaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { GetAppearance(), GroupInfo.ControlInfo.GetAppearance(this) }, DefaultAppearance);
			if(res.TextOptions.WordWrap == WordWrap.Default)
				res.TextOptions.WordWrap = WordWrap.Wrap;
			return res;
		}
		public TileItemContentShowMode TextShowMode {
			get {
				TileItemContentShowMode res = Item.TextShowMode != TileItemContentShowMode.Default ? Item.TextShowMode : ControlInfo.Owner.Properties.ItemTextShowMode;
				return res == TileItemContentShowMode.Default ? TileItemContentShowMode.Always : res;
			}
		}
		public TileItemContentAlignment BackgroundImageAlignment {
			get {
				TileItemContentAlignment res = Item.BackgroundImageAlignment != TileItemContentAlignment.Default ? Item.BackgroundImageAlignment : ControlInfo.Owner.Properties.ItemBackgroundImageAlignment;
				return res == TileItemContentAlignment.Default ? TileItemContentAlignment.TopLeft : res;
			}
		}
		public bool CanDrawHoverOverlay {
			get {
				if(!HoverAppearanceBackColorIsEmpty) return false;
				if(GroupInfo.ControlInfo.Owner.Handler.TouchState != TileControlHandler.TileControlTouchState.Inactive) return false;
				if(!IsHovered && HoverOpacity == 0.0f) return false;
				return true;
			}
		}
		bool HoverAppearanceBackColorIsEmpty {
			get {
				AppearanceObject hover = new AppearanceObject();
				AppearanceHelper.Combine(hover, new AppearanceObject[] { Item.AppearanceItem.Hovered, ControlInfo.Owner.AppearanceItem.Hovered });
				return hover.BackColor.IsEmpty;
			}
		}
		protected virtual TileItemElementViewInfo CreateElementInfo(TileItemViewInfo itemInfo, TileItemElement elem) {
			return new TileItemElementViewInfo() { ItemInfo = itemInfo, Element = elem };
		}
		public bool IsSmall {
			get { return (GroupInfo != null) ? ControlInfo.IsSmallItem(Item) : Item.GetIsSmall(); }
		}
		public bool IsLarge {
			get { return (GroupInfo != null) ? ControlInfo.IsLargeItem(Item) : Item.GetIsLarge(); }
		}
		public bool IsMedium {
			get { return (GroupInfo != null) ? ControlInfo.IsMediumItem(Item) : Item.GetIsMedium(); }
		}
		public virtual void LayoutItem(Rectangle bounds) {
			bool releaseGraphics = false;
			if(GroupInfo.ControlInfo.GInfo.Graphics == null) {
				releaseGraphics = true;
				GroupInfo.ControlInfo.GInfo.AddGraphics(null);
			}
			Bounds = bounds;
			if(!GroupInfo.ControlInfo.ShouldMakeTransition)
				RenderImageBounds = Bounds;
			OuterBounds = CalcOuterBounds(Bounds);
			DropBounds = CalcDropBounds(Bounds);
			ContentBounds = CalcContentBounds(Bounds);
			Elements = new List<TileItemElementViewInfo>();
			for(int i = 0; i < Item.Elements.Count; i++)
				Elements.Add(CreateElementInfo(this, Item.Elements[i]));
			foreach(var elementInfo in Elements)
				elementInfo.CalcViewInfo();
			BackgroundImageContentBounds = TileItemCalculator.LayoutContent(Bounds, GetBackgroundImageSize(), BackgroundImageAlignment, Point.Empty);
			CalcContextButtonsViewInfo();
			if(releaseGraphics) {
				GroupInfo.ControlInfo.GInfo.ReleaseGraphics();
			}
		}
		Rectangle PrevDisplayBounds { get; set; }
		protected virtual void CalcContextButtonsViewInfo() {
			if(((ISupportContextItems)this).ContextItems == null)
				return;
			if(PrevDisplayBounds != ((ISupportContextItems)this).DisplayBounds) {
				ContextButtonsViewInfo.InvalidateViewInfo();
			}
			PrevDisplayBounds = ((ISupportContextItems)this).DisplayBounds;
			ContextButtonsViewInfo.CalcItems();
		}
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ((ISupportContextItems)this).Options, this);
				return contextButtonsViewInfo;
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = new ContextItemCollectionHandler(ContextButtonsViewInfo);
				return contextButtonsHandler;
			}
		}
		protected virtual Rectangle CalcOuterBounds(Rectangle bounds) {
			SkinElementInfo selInfo = GetItemSelectedInfo();
			SkinElementInfo checkInfo = GetItemCheckedInfo();
			Rectangle res1 = selInfo != null ? selInfo.Bounds : Rectangle.Empty;
			Rectangle res2 = checkInfo != null ? checkInfo.Bounds : Rectangle.Empty;
			int x = Math.Min(Bounds.X, Math.Min(res1.X, res2.X));
			int y = Math.Min(Bounds.Y, Math.Min(res1.Y, res2.Y));
			int width = Math.Max(Bounds.Right - x, Math.Max(res1.Right - x, res2.Right - x));
			int height = Math.Max(Bounds.Bottom - y, Math.Max(res1.Bottom - y, res2.Bottom - y));
			return new Rectangle(x, y, width, height);
		}
		static XPaint GdiPaint = new XPaint();
		static XPaint GdiPaintMixed = new XPaintMixed();
		internal static XPaint GetGdiPaint(TileControlViewInfo controlInfo) {
			if(controlInfo == null || controlInfo.UseAdvancedTextRendering) return GdiPaintMixed;
			return GdiPaint;
		}
		public TileItemImageScaleMode BackgroundImageScaleMode {
			get {
				return Item.BackgroundImageScaleMode != TileItemImageScaleMode.Default ? Item.BackgroundImageScaleMode : ControlInfo.Owner.Properties.ItemBackgroundImageScaleMode;
			}
		}
		protected virtual Size GetBackgroundImageSize() {
			if(BackgroundImage != null)
				return TileItemImageScaleModeHelper.ScaleImage(Bounds.Size, Item.BackgroundImage.Size, BackgroundImageScaleMode);
			return Size.Empty;
		}
		protected virtual Rectangle CalcDropBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			rect.Inflate(GroupInfo.ControlInfo.IndentBetweenItems / 2, GroupInfo.ControlInfo.IndentBetweenItems / 2);
			return rect;
		}
		public Padding ItemPadding {
			get { return GetItemPaddingCore(); }
		}
		protected virtual Padding GetItemPaddingCore() {
			return Item.Padding == TileItem.DefaultPadding ? ControlInfo.Owner.Properties.ItemPadding : Item.Padding;
		}
		protected virtual Rectangle CalcContentBounds(Rectangle bounds) {
			Rectangle res = bounds;
			res.X += ItemPadding.Left;
			res.Y += ItemPadding.Top;
			res.Width -= ItemPadding.Horizontal;
			res.Height -= ItemPadding.Vertical;
			return res;
		}
		protected internal virtual void MakeOffset(int deltaX, int deltaY) {
			Bounds = TileItemCalculator.OffsetRect(Bounds, deltaX, deltaY);
			if(!ControlInfo.ShouldMakeTransition)
				RenderImageBounds = TileItemCalculator.OffsetRect(RenderImageBounds, deltaX, deltaY);
			OuterBounds = TileItemCalculator.OffsetRect(OuterBounds, deltaX, deltaY);
			DropBounds = CalcDropBounds(Bounds);
			ContentBounds = CalcContentBounds(Bounds);
			BackgroundImageContentBounds = TileItemCalculator.OffsetRect(BackgroundImageContentBounds, deltaX, deltaY);
			for(int i = 0; i < Elements.Count; i++) {
				Elements[i].MakeOffset(deltaX, deltaY);
			}
			if(((ISupportContextItems)this).ContextItems != null)
				ContextButtonsViewInfo.InvalidateViewInfo();
		}
		protected internal virtual void ClearAppearance() {
			this.paintAppearance = null;
		}
		protected internal virtual void PrepareForContentChanging(TileItemFrame frame) {
			PrevRenderImage = null;
			ControlInfo.PrepareItemForDrawingInImage(this, frame);
			PrevRenderImage = GroupInfo.ControlInfo.RenderItemToBitmap(this);
			DrawBackgroundImage = true;
			DrawImage = true;
			DrawText = true;
			Item.SavePrevText();
		}
		protected internal virtual void LayoutItemVertical(Point point) {
			LayoutItem(new Rectangle(point, Bounds.Size));
		}
		public virtual void ResetDefaultAppearance() { defaultAppearance = null; }
		protected internal void ResetHover() {
			HoverOpacity = 0.0f;
			ForceIsHovered = null;
		}
		public virtual bool ShouldDrawBorder {
			get {
				var mode = GetBorderVisibility();
				switch(mode) {
					case TileItemBorderVisibility.Always: return true;
					case TileItemBorderVisibility.Never: return false;
					default: return Item.BackgroundImage == null;
				}
			}
		}
		TileItemBorderVisibility GetBorderVisibility() {
			if(Item.BorderVisibility != TileItemBorderVisibility.Auto)
				return Item.BorderVisibility;
			return ControlInfo.Owner.Properties.ItemBorderVisibility;
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return Bounds; }
		}
		bool ISupportContextItems.CloneItems {
			get { return true; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return ControlInfo.Owner.ContextButtons; }
		}
		Control ISupportContextItems.Control {
			get { return ControlInfo.Owner.Control; }
		}
		bool ISupportContextItems.DesignMode { get { return ControlInfo.Owner.IsDesignMode; } }
		Rectangle ISupportContextItems.DisplayBounds {
			get { return Bounds; }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return Bounds; }
		}
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return 3;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return ControlInfo.Owner.LookAndFeel.ActiveLookAndFeel; }
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return ControlInfo.Owner.ContextButtonOptions; }
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			e.DataItem = Item;
			ControlInfo.Owner.RaiseContextItemClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ControlInfo.Owner.RaiseCustomContextButtonToolTip(Item, e);
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem contextItem) {
			ControlInfo.Owner.RaiseContextButtonCustomize(Item, contextItem);
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			if(ControlInfo.Owner.Control == null) return;
			ControlInfo.Owner.Control.Invalidate(rect);
		}
		void ISupportContextItems.Update() {
			if(ControlInfo.Owner.Control == null) return;
			ControlInfo.Owner.Control.Update();
		}
		void ISupportContextItems.Redraw() {
			if(ControlInfo.Owner.Control == null) return;
			ControlInfo.Owner.Invalidate(ControlInfo.Owner.ClientRectangle);
		}
		protected internal virtual Rectangle GetVisualEffectBounds() {
			return Bounds;
		}
	}
	public class TileItemPosition {
		public TileItemPosition(int groupColumn, int row, int column, int subRow, int subColumn) {
			GroupColumn = groupColumn;
			Row = row;
			Column = column;
			SubRow = subRow;
			SubColumn = subColumn;
		}
		public TileItemPosition() { }
		public int GroupColumn { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public int SubColumn { get; set; }
		public int SubRow { get; set; }
		public TileGroupViewInfo GroupInfo { get; set; }
		public TileItemPosition Clone() { return new TileItemPosition(GroupColumn, Row, Column, SubRow, SubColumn); }
		public void Assign(TileItemPosition pos) {
			GroupColumn = pos.GroupColumn;
			Row = pos.Row;
			Column = pos.Column;
			SubColumn = pos.SubColumn;
			SubRow = pos.SubRow;
		}
		public override string ToString() {
			return String.Format("GroupColumn = {0}; Row = {1}; Column = {2}; SubRow = {3}; SubColumn = {4}", GroupColumn, Row, Column, SubRow, SubColumn);
		}
	}
	public class TileGroupLayoutInfo {
		public TileGroupLayoutInfo() {
			ItemPosition = new TileItemPosition();
			PrevItemPosition = new TileItemPosition();
		}
		public Point StartLocation { get; set; }
		public int ColumnX { get; set; }
		public int ColumnWidth { get; set; }
		public Point Location { get; set; }
		public int BottomY { get; set; }
		public int MaxColumnWidth { get; set; }
		public TileItemPosition ItemPosition;
		public TileItemPosition PrevItemPosition;
	}
	public class TileGroupViewInfo {
		public TileGroup Group { get; private set; }
		public TileControlViewInfo ControlInfo { get; protected internal set; }
		public TileGroupViewInfo(TileGroup group) {
			Group = group;
			Group.GroupInfo = this;
			CreateItems();
		}
		internal virtual bool ShouldDrawDragItemPlacement {
			get {
				return ControlInfo.DragItem != null &&
					Group.Items.Count == 0 && Group.Control.IsDesignMode &&
					ControlInfo.DropInfo != null &&
					ControlInfo.DropInfo.GroupInfo == this && ControlInfo.DropInfo.InsertIntoGroup;
			}
		}
		protected TileControlLayoutGroup GetLayoutGroup() {
			foreach(TileControlLayoutGroup group in Group.Control.ViewInfo.Calculator.Groups) {
				if(group.Group == Group)
					return group;
			}
			return null;
		}
		protected virtual void CreateItems() {
			TileControlLayoutGroup group = GetLayoutGroup();
			foreach(TileControlLayoutItem item in group.Items) {
				TileItemViewInfo itemInfo = item.Item.Control.ViewInfo.GetItemFromCache(item.Item);
				item.ItemInfo = itemInfo;
				itemInfo.GroupInfo = this;
				itemInfo.Item.ItemInfo = itemInfo;
				itemInfo.ClearAppearance();
				Items.Add(itemInfo);
			}
		}
		TileItemViewInfoCollection items;
		public TileItemViewInfoCollection Items {
			get {
				if(items == null)
					items = CreateItemsCollection();
				return items;
			}
		}
		public bool IsVisible { get { return ControlInfo.Bounds.IntersectsWith(Bounds); } }
		protected virtual TileItemViewInfoCollection CreateItemsCollection() {
			return new TileItemViewInfoCollection(this);
		}
		public Rectangle Bounds { get; internal set; }
		public Rectangle DesignTimeBounds {
			get { return Rectangle.Inflate(Bounds, 4, 4); }
		}
		public Rectangle TotalBounds {
			get {
				Rectangle bounds = Bounds;
				if(Group != null && Group.Control != null && Group.Control.IsDesignMode)
					bounds = DesignTimeBounds;
				if(!ControlInfo.Owner.Properties.ShowGroupText)
					return bounds;
				return new Rectangle(bounds.X, TextBounds.Y, bounds.Width, bounds.Bottom - TextBounds.Y);
			}
		}
		public Rectangle OuterBounds {
			get {
				Rectangle rect = Bounds;
				rect.Inflate(ControlInfo.Owner.Properties.IndentBetweenGroups / 2, ControlInfo.Owner.Properties.IndentBetweenGroups / 2);
				return rect;
			}
		}
		public Point Origin {
			get {
				return new Point(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
			}
		}
		public Rectangle TextBounds { get; protected internal set; }
		protected internal virtual Point CalcTextBounds(Point start, ref Rectangle textBounds) {
			if(ControlInfo.Owner.Properties.ShowGroupText) {
				Size size = new Size(ControlInfo.AppearanceGroupText.CalcTextSize(ControlInfo.GInfo.Graphics, Group.Text, 0).ToSize().Width, ControlInfo.GroupTextHeight);
				if(ControlInfo.IsRightToLeft)
					textBounds = new Rectangle(new Point(start.X - size.Width, start.Y), size);
				else
					textBounds = new Rectangle(start, size);
				start.Y += ControlInfo.GroupTextHeight + ControlInfo.GroupTextToItemsIndent;
			}
			else {
				textBounds = Rectangle.Empty;
			}
			return start;
		}
		protected TileItemViewInfo GetPrevItem(TileItemViewInfo current) {
			int pos = Items.IndexOf(current);
			if(Group.Control.IsDesignMode)
				return pos > 0 ? Items[pos - 1] : null;
			for(int i = pos - 1; i >= 0; i--) {
				if(Items[i].Item.Visible) return Items[i];
			}
			return null;
		}
		protected TileItemViewInfo GetNextItem(TileItemViewInfo current) {
			int currentIndex = Items.IndexOf(current);
			if(currentIndex < 0 || currentIndex + 1 >= Items.Count)
				return null;
			return Items[currentIndex + 1];
		}
		protected virtual bool ShouldStartNewRow(TileGroupLayoutInfo info, TileItemViewInfo itemInfo) {
			if(ControlInfo.IsVertical)
				return ShouldStartNewRowVertical(info, itemInfo);
			return ShouldStartNewRowHorizontal(info, itemInfo);
		}
		protected virtual bool ShouldStartNewRowVertical(TileGroupLayoutInfo info, TileItemViewInfo itemInfo) {
			if(ControlInfo.Owner.Properties.ColumnCount == 0) {
				TileItemViewInfo prevItem = GetPrevItem(itemInfo);
				return prevItem != null && prevItem.Bounds.Right + ControlInfo.IndentBetweenItems + itemInfo.Bounds.Width > ControlInfo.ContentBounds.Right;
			}
			int filledColumn = CalcFilledColumnCount(info.ItemPosition.Row, itemInfo);
			int requestedColumn = ControlInfo.IsLargeItem(itemInfo.Item) ? 2 : 1;
			return (filledColumn + requestedColumn) > ControlInfo.Owner.Properties.ColumnCount;
		}
		protected virtual int CalcFilledColumnCount(int row, TileItemViewInfo itemInfo) {
			var res = 0;
			var itemIndex = Items.IndexOf(itemInfo);
			for(int i = (itemIndex - 1); i >= 0; i--) {
				if(Items[i].Position.Row != row) {
					break;
				}
				res += ControlInfo.IsLargeItem(Items[i].Item) ? 2 : 1;
			}
			return res;
		}
		protected virtual bool ShouldStartNewRowHorizontal(TileGroupLayoutInfo info, TileItemViewInfo itemInfo) {
			TileItemViewInfo prevItem = GetPrevItem(itemInfo);
			if(info.ItemPosition.Column == 2) return true;
			if(info.ItemPosition.Column == 1 && ControlInfo.IsLargeItem(itemInfo.Item)) return true;
			if(prevItem != null && ControlInfo.IsLargeItem(prevItem.Item)) return true;
			return false;
		}
		protected virtual bool ShouldStartNewGroupColumn(TileGroupLayoutInfo info, TileItemViewInfo itemInfo) {
			if(!ShouldStartNewRow(info, itemInfo))
				return false;
			int prevRowCount = GetPrevItem(itemInfo) != null ? GetPrevItem(itemInfo).RowCount : 0;
			int currentRowCount = info.ItemPosition.Row;
			return (currentRowCount > 0 || prevRowCount > 0) && (currentRowCount + itemInfo.RowCount >= Math.Min(ControlInfo.Owner.Properties.RowCount, ControlInfo.AvailableRowCount));
		}
		protected internal virtual void CalcHitInfo(TileControlHitInfo hitInfo) {
			if(hitInfo.ContainsSet(TextBounds, TileControlHitTest.GroupCaption))
				return;
			foreach(TileItemViewInfo itemInfo in Items) {
				if(!itemInfo.ShouldProcessItem) continue;
				Rectangle bounds = hitInfo.CheckDropBounds ? itemInfo.DropBounds : itemInfo.Bounds;
				if(hitInfo.ContainsSet(bounds, TileControlHitTest.Item)) {
					hitInfo.ItemInfo = itemInfo;
					CalcDropDownHitInfo(itemInfo, hitInfo);
					return;
				}
			}
		}
		protected internal virtual void CalcDropDownHitInfo(TileItemViewInfo itemInfo, TileControlHitInfo hitInfo) { }
		protected virtual void MakeTextBoundsOffset(int deltaX, int deltaY) {
			TextBounds = new Rectangle(TextBounds.X + deltaX, TextBounds.Y + deltaY, TextBounds.Width, TextBounds.Height);
		}
		protected internal virtual void MakeOffset(int deltaX, int deltaY) {
			Bounds = new Rectangle(Bounds.X + deltaX, Bounds.Y + deltaY, Bounds.Width, Bounds.Height);
			foreach(TileItemViewInfo itemInfo in Items) {
				if(!itemInfo.ShouldProcessItem) continue;
				itemInfo.MakeOffset(deltaX, deltaY);
			}
			MakeTextBoundsOffset(deltaX, deltaY);
		}
		protected internal virtual int CalcBestRowCount() {
			TileGroupRowCountHelper rowCountHelper = new TileGroupRowCountHelper(Group.Groups.Owner.Properties.Orientation, Group.Groups.Owner.Properties.ColumnCount);
			foreach(TileItemViewInfo itemInfo in Items) {
				if(!itemInfo.ShouldProcessItem) continue;
				float itemSpace = TileGroupRowCountHelper.GetSpaceValue(itemInfo);
				rowCountHelper.Add(itemSpace, itemInfo.RowCount);
			}
			return rowCountHelper.RowCount;
		}
		protected internal virtual void UpdateTextWidth() {
			if(TextBounds.Size.IsEmpty)
				return;
			int constraint = Bounds.Width;
			if((ControlInfo != null && ControlInfo.Owner != null) && ControlInfo.Owner.Properties.Orientation == Orientation.Vertical) {
				constraint = ControlInfo.ContentBounds.Right - TextBounds.X;
			}
			TextBounds = new Rectangle(TextBounds.X, TextBounds.Y, Math.Min(TextBounds.Width, constraint), TextBounds.Height);
		}
		public virtual void LayoutGroup(TileControlLayoutCalculator calculator, TileControlLayoutGroup group) {
			Bounds = group.Bounds;
			TextBounds = group.TextBounds;
			foreach(TileControlLayoutItem item in group.Items) {
				item.ItemInfo.Position = item.Position.Clone();
				item.ItemInfo.LayoutItem(item.Bounds);
			}
		}
		public bool CanDrawCaption {
			get {
				if(ControlInfo.Owner.IsDesignMode)
					return ControlInfo.Owner.Properties.ShowGroupText;
				return ControlInfo.Owner.Properties.ShowGroupText && !Bounds.Size.IsEmpty;
			}
		}
		protected internal void OnVisibilityChanged() {
			ResetItemsHover();
		}
		void ResetItemsHover() {
			if(Items.Count < 1) return;
			foreach(TileItemViewInfo itemInfo in Items) {
				itemInfo.ResetHover();
			}
		}
	}
	public class TileItemViewInfoCollection : CollectionBase {
		public TileGroupViewInfo GroupInfo { get; private set; }
		public TileItemViewInfoCollection(TileGroupViewInfo groupInfo) {
			GroupInfo = groupInfo;
		}
		public int Add(TileItemViewInfo item) { return List.Add(item); }
		public void Insert(int index, TileItemViewInfo item) { List.Insert(index, item); }
		public void Remove(TileItemViewInfo item) { List.Remove(item); }
		public int IndexOf(TileItemViewInfo item) { return List.IndexOf(item); }
		public void Contains(TileItemViewInfo item) { List.Contains(item); }
		public TileItemViewInfo this[int index] { get { return (TileItemViewInfo)List[index]; } set { List[index] = value; } }
		public TileItemViewInfo GetItemFromLeft(TileItemViewInfo item) {
			if(item.Position.Column == 0 || item.IsLarge)
				return null;
			int itemIndex = IndexOf(item);
			return this[itemIndex - 1];
		}
		public TileItemViewInfo GetItemFromTop(TileItemViewInfo item, int subColumn) {
			if(item.Position.Row == 0)
				return null;
			int itemIndex = IndexOf(item) - 1;
			if(item.Position.Row == this[itemIndex].Position.Row)
				itemIndex--;
			if(this[itemIndex].Position.Column > subColumn)
				itemIndex--;
			if(itemIndex < 0)
				return null;
			return this[itemIndex];
		}
		public TileItemViewInfo GetItemFromBottom(TileItemViewInfo item, int subColumn) {
			int itemIndex = IndexOf(item) + 1;
			if(itemIndex >= Count)
				return null;
			if(this[itemIndex].Position.Row == item.Position.Row)
				itemIndex++;
			if(itemIndex >= Count)
				return null;
			if(this[itemIndex].Position.Column < subColumn)
				itemIndex++;
			if(itemIndex >= Count)
				return null;
			return this[itemIndex];
		}
		public TileItemViewInfo GetItemFromRight(TileItemViewInfo item) {
			if(item.IsLarge || item.Position.Column == 1)
				return null;
			int itemIndex = IndexOf(item) + 1;
			if(itemIndex >= Count || this[itemIndex].Position.Column != 1)
				return null;
			return this[itemIndex];
		}
		public TileItemViewInfo GetItem(int row, int column, int subColumn) {
			foreach(TileItemViewInfo itemInfo in this) {
				if(itemInfo.Position.Row == row && itemInfo.Position.GroupColumn == column && itemInfo.Position.Column == subColumn)
					return itemInfo;
			}
			return null;
		}
		public virtual TileItemViewInfo GetItemByPoint(Point pt) {
			foreach(TileItemViewInfo itemInfo in this) {
				if(itemInfo.Bounds.Contains(pt))
					return itemInfo;
			}
			return null;
		}
	}
	internal class TileGroupRowCountHelper {
		public TileGroupRowCountHelper(Orientation orientation, int colCount) {
			this.orientation = orientation;
			this.colCount = colCount;
		}
		int colCount;
		Orientation orientation;
		public int RowCount { get; set; }
		float FirstSpace { get; set; }
		float SecondSpace { get; set; }
		public void Add(float itemSpace, int delta) {
			if(orientation == Orientation.Horizontal)
				AddHorizontal(itemSpace, delta);
			else
				AddVertical(itemSpace);
		}
		void AddHorizontal(float itemSpace, int delta) {
			if(FirstSpace + itemSpace > 1) {
				if(SecondSpace + itemSpace > 1) {
					RowCount += delta;
					SecondSpace = 0;
					if(itemSpace < 2)
						FirstSpace = itemSpace;
					else
						FirstSpace = 0;
				}
				else {
					SecondSpace += itemSpace;
					if(SecondSpace >= 1) {
						SecondSpace = 0;
						FirstSpace = 0;
					}
				}
			}
			else FirstSpace += itemSpace;
			if(FirstSpace - itemSpace == 0)
				RowCount += delta;
		}
		int countInRow = 0;
		void AddVertical(float itemSpace) {
			if(colCount == 1) {
				RowCount++;
				return;
			}
			if(countInRow == 0 || countInRow + itemSpace > colCount) {
				countInRow = 0;
				RowCount++;
			}
			if(countInRow + itemSpace > colCount)
				RowCount++;
			countInRow++;
			if(itemSpace > 1)
				countInRow++;
			if(countInRow >= colCount)
				countInRow = 0;
		}
		protected internal static float GetSpaceValue(TileItemViewInfo itemInfo) {
			switch(itemInfo.Item.ItemSize) {
				case TileItemSize.Small:
					return 0.25f;
				case TileItemSize.Medium:
					return 1f;
				default:
					return 2f;
			}
		}
	}
	public class TileGroupViewInfoCollection : CollectionBase {
		public TileControlViewInfo ViewInfo { get; private set; }
		public TileGroupViewInfoCollection(TileControlViewInfo controlViewInfo) {
			ViewInfo = controlViewInfo;
		}
		public int Add(TileGroupViewInfo group) { return List.Add(group); }
		public void Insert(int index, TileGroupViewInfo group) { List.Insert(index, group); }
		public void Remove(TileGroupViewInfo group) { List.Remove(group); }
		public int IndexOf(TileGroupViewInfo group) { return List.IndexOf(group); }
		public bool Contains(TileGroupViewInfo group) { return List.Contains(group); }
		public TileGroupViewInfo this[int index] { get { return (TileGroupViewInfo)List[index]; } set { List[index] = value; } }
	}
	public interface ITileControlDesignManager {
		DevExpress.Utils.Design.BaseDesignTimeManager GetBase();
		object GetGroup();
		object GetItem();
		void ShowItemMenu(TileControlHitInfo hitInfo);
		void ShowGroupMenu(TileControlHitInfo hitInfo);
		bool IsComponentSelected(object obj);
		void SelectComponent(object component);
		void FireChanged();
		void DrawSelectionBounds(GraphicsCache cache, Rectangle rect, Color color);
		void DrawSelection(GraphicsCache cache, Rectangle rect, Color color);
		void ComponentChanged(IComponent component);
		void OnAddTileGroupClick();
		void OnRemoveTileGroupClick(object group);
		void OnAddTileItemClick();
		void OnAddTileItemClick(object itemsize);
		void OnRemoveTileItemClick(object tile);
		void OnEditElementsCollectionClickCore(object tile);
		void OnEditFramesCollectionClickCore(object tile);
		void OnEditTileTemplateClickCore(object tile);
	}
	public class TileControlViewInfo : ISupportAdornerUIManager {
		protected internal virtual int PressedItemBoundsAnimationLength { get { return 50; } }
		protected internal virtual int DragModeItemBoundsAnimationLength { get { return 100; } }
		protected internal virtual int DragMoveItemBoundsAnimationLength { get { return 50; } }
		protected internal virtual int PressedItemBoundsDelta { get { return 2; } }
		protected internal virtual int DragItemBoundsAnimationDelta { get { return 14; } }
		protected internal virtual int DragItemGroupBoundsAnimationDelta { get { return 62; } }
		protected internal virtual int DragItemBoundsScaleDelta { get { return 8; } }
		protected internal virtual float DragItemOpacity { get { return 0.5f; } }
		protected internal virtual float BackgroundImageOffsetKoeff { get { return 0.1f; } }
		protected internal virtual int ScrollAreaWidth { get { return 24; } }
		protected internal virtual int TileItemArrivalAnimationLength { get { return 300; } }
		protected internal virtual int TileItemArrivalDeltaX { get { return 64; } }
		protected internal virtual int TileItemArrivalDeltaY { get { return 32; } }
		protected internal virtual int TileItemArrivalScaleDelta { get { return 24; } }
		protected internal virtual int TileITemArrivalDelay { get { return 100; } }
		protected internal virtual int TileItemTransitionLength { get { return 400; } }
		protected internal virtual int OffsetAnimationLength { get { return 200; } }
		internal static int ItemContentAnimationLength = 800;
		protected internal virtual int ScrollValue { get { return 30; } }
		protected internal virtual int SelectionWidth { get { return 2; } }
		public ITileControl Owner { get; private set; }
		public TileControlViewInfo(ITileControl control) {
			Owner = control;
			BackArrowState = ObjectState.Normal;
			ForwardArrowState = ObjectState.Normal;
			ShouldResetSelectedItem = true;
			UseAdvancedTextRendering = true;
		}
		public virtual bool IsRightToLeft {
			get {
				if(Owner != null && Owner is TileControl) {
					return (Owner as TileControl).IsRightToLeft;
				}
				return false;
			}
		}
		public virtual bool ShouldReverseScrollDueRTL {
			get { return IsRightToLeft && Owner.Properties.Orientation == Orientation.Horizontal; }
		}
		public virtual bool ShouldResetSelectedItem { get; set; }
		Image backgroundImageStretchedCore;
		public Image BackgroundImageStretched {
			get {
				if(backgroundImageStretchedCore == null) backgroundImageStretchedCore = PrepareBackgroundImage();
				return backgroundImageStretchedCore;
			}
		}
		public virtual Point GetBackgroundImageLocation(Size imageSize) {
			if(Owner.Control.BackgroundImageLayout == ImageLayout.Stretch)
				return Owner.Control.ClientRectangle.Location;
			Size client = Owner.Control.ClientSize;
			Point pt = Point.Empty;
			pt.X += (client.Width / 2) - (imageSize.Width / 2);
			pt.Y += (client.Height / 2) - (imageSize.Height / 2);
			return pt;
		}
		Image PrepareBackgroundImage() {
			if(Owner.BackgroundImage == null) return null;
			if(Owner.Control.BackgroundImageLayout == ImageLayout.Stretch)
				return PrepareBackgroundImageStretched();
			return PrepareBackgroundImageZoomed();
		}
		Image PrepareBackgroundImageStretched() {
			int w = Owner.Control.ClientSize.Width;
			int h = Owner.Control.ClientSize.Height;
			return GetScaledBackgroundImage(w, h);
		}
		Image PrepareBackgroundImageZoomed() {
			int w = Owner.Control.ClientSize.Width;
			int h = Owner.Control.ClientSize.Height;
			int imgW = Owner.BackgroundImage.Width;
			int imgH = Owner.BackgroundImage.Height;
			float scaleX = (float)w / imgW;
			float scaleY = (float)h / imgH;
			float scale = Math.Min(scaleX, scaleY);
			w = Math.Min((int)(imgW * scale), w);
			h = Math.Min((int)(imgH * scale), h);
			return GetScaledBackgroundImage(w, h);
		}
		Image GetScaledBackgroundImage(int width, int height) {
			Image img = new Bitmap(width, height);
			using(Graphics g = Graphics.FromImage(img)) {
				g.DrawImage(Owner.BackgroundImage, new Rectangle(0, 0, width, height));
			}
			return img;
		}
		ITileControlDesignManager designTimeManager;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ITileControlDesignManager DesignTimeManager {
			get {
				if(designTimeManager == null)
					designTimeManager = CreateDTManager();
				return designTimeManager;
			}
			set { designTimeManager = value; }
		}
		protected virtual ITileControlDesignManager CreateDTManager() {
			return new TileControlDesignTimeManagerBase(Owner.Site.Component, Owner);
		}
		#region Group Highlighting
		public bool ShouldDrawRectangleNewGroup {
			get {
				return DragItem != null && DropTargetInfo != null && DropTargetInfo.NearestGroup != null;
			}
		}
		public bool ShouldDrawGroupHighlight {
			get {
				return DragItem != null && DragItem.IsDragging && Owner.Properties.AllowGroupHighlighting;
			}
		}
		protected internal bool ShouldDrawGroupHighlighting(TileGroupViewInfo groupInfo) {
			return ShouldDrawGroupHighlight && (groupInfo.Items.Count > 0 || Owner.IsDesignMode);
		}
		protected virtual bool CanDropItem { get { return DropInfo != null; } }
		protected virtual bool CanDropHomeGroup { get { return !CanDropItem ? false : DropInfo.GroupInfo == null; } }
		protected virtual int GroupHighlightingPadding { get { return 10; } }
		public Color HighLightColor(Point pt, TileGroupViewInfo groupInfo, TileControlInfoArgs e) {
			if(groupInfo.TotalBounds.Contains(pt) && e.ViewInfo.DropInfo != null)
				return Owner.Properties.AppearanceGroupHighlighting.HoverColorGroup;
			return Owner.Properties.AppearanceGroupHighlighting.StandardColorGroup;
		}
		public Rectangle RectangleDropGroup {
			get {
				if(!CanDropItem) return Rectangle.Empty;
				Rectangle rect = CanDropHomeGroup ? DragItem.GroupInfo.TotalBounds : DropInfo.GroupInfo.TotalBounds;
				return Rectangle.Inflate(rect, GroupHighlightingPadding, GroupHighlightingPadding);
			}
		}
		public Rectangle CalcGroupHighlightedRectangle(TileGroupViewInfo groupInfo) {
			Rectangle rect = groupInfo.TotalBounds;
			rect.Inflate(GroupHighlightingPadding, GroupHighlightingPadding);
			return rect;
		}
		public Rectangle CalcNewGroupHighlightedBounds {
			get { return IsHorizontal ? CalcHorizontalNewGroupHighlightedBounds() : CalcVerticalNewGroupHighlightedBounds(); }
		}
		protected virtual Rectangle CalcHorizontalNewGroupHighlightedBounds() {
			TileGroupViewInfo groupInfo = DropTargetInfo.NearestGroupInfo;
			int indent = Owner.Properties.IndentBetweenGroups;
			int x = DropTargetInfo.GroupDropSide == TileControlDropSide.Left ? groupInfo.Bounds.X - indent + GroupHighlightingPadding : groupInfo.Bounds.Right + GroupHighlightingPadding;
			return new Rectangle(x, groupInfo.Bounds.Y, indent - GroupHighlightingPadding * 2, CalcMaxGroupSize().Height);
		}
		protected bool IsSingletonGroupItem {
			get { return DragItem.GroupInfo.Group.Items.Count == 1; }
		}
		protected virtual Rectangle CalcVerticalNewGroupHighlightedBounds() {
			TileGroupViewInfo groupInfo = DropTargetInfo.NearestGroupInfo;
			int indent = Owner.Properties.IndentBetweenGroups;
			int y = DropTargetInfo.GroupDropSide == TileControlDropSide.Top ? groupInfo.TotalBounds.Y - indent + GroupHighlightingPadding : groupInfo.TotalBounds.Bottom + GroupHighlightingPadding;
			return new Rectangle(groupInfo.Bounds.X, y, CalcMaxGroupSize().Width, indent - GroupHighlightingPadding * 2);
		}
		protected Size CalcMaxGroupSize() {
			Size max = new Size();
			for(int i = 0; i < Groups.Count; i++) {
				if(max.Height <= Groups[i].TotalBounds.Height) max.Height = Groups[i].TotalBounds.Height + 2 * GroupHighlightingPadding;
				if(max.Width <= Groups[i].TotalBounds.Width) max.Width = Groups[i].TotalBounds.Width + 2 * GroupHighlightingPadding;
			}
			return max;
		}
		#endregion
		public bool IsHorizontal {
			get { return Owner.Properties.Orientation == Orientation.Horizontal; }
		}
		public bool IsVertical {
			get { return Owner.Properties.Orientation == Orientation.Vertical; }
		}
		public virtual Color CheckBorderColor { get { return Color.FromArgb(0x80, 0, 0, 0); } }
		public virtual int CheckBorderWidth { get { return 4; } }
		TileGroupViewInfoCollection groups;
		public TileGroupViewInfoCollection Groups {
			get {
				if(groups == null)
					groups = CreateGroupsCollection();
				return groups;
			}
		}
		public Point BackgroundImageStartPoint {
			get {
				return new Point(Bounds.X - (int)(Offset * BackgroundImageOffsetKoeff), Bounds.Y);
			}
		}
		protected virtual TileGroupViewInfoCollection CreateGroupsCollection() {
			return new TileGroupViewInfoCollection(this);
		}
		public BorderStyles BorderStyle {
			get { return Owner.BorderStyle; }
		}
		BorderPainter borderPainter;
		public BorderPainter BorderPainter {
			get {
				if(borderPainter == null)
					borderPainter = BorderHelper.GetPainter(BorderStyle, Owner.LookAndFeel.ActiveLookAndFeel);
				return borderPainter;
			}
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null)
					paintAppearance = new AppearanceObject();
				return paintAppearance;
			}
		}
		protected internal AppearanceDefault DefaultAppearance {
			get { return GetDefaultAppearance(); }
		}
		protected virtual AppearanceDefault GetDefaultAppearance() {
			return new AppearanceDefault(Owner.BackColor);
		}
		AppearanceObject appearanceText;
		public AppearanceObject AppearanceText {
			get {
				if(appearanceText == null)
					appearanceText = CreateAppearanceText();
				return appearanceText;
			}
		}
		public StringFormat AppearanceTextStringFormat {
			get {
				var format = AppearanceText.GetStringFormat();
				if(IsRightToLeft)
					format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				else
					format.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
				return format;
			}
		}
		protected virtual AppearanceDefault DefaultAppearanceText {
			get {
				AppearanceDefault res = new AppearanceDefault(CommonSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.ControlText), Color.Empty, Color.Empty, Color.Empty);
				res.Font = new Font(AppearanceObject.ControlAppearance.Font.FontFamily, 26);
				return res;
			}
		}
		AppearanceObject appearanceGroupText;
		public AppearanceObject AppearanceGroupText {
			get {
				if(appearanceGroupText == null)
					appearanceGroupText = CreateAppearanceGroupText();
				return appearanceGroupText;
			}
		}
		protected virtual AppearanceDefault DefaultAppearanceGroupText {
			get {
				SkinElement element = MetroUISkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[MetroUISkins.SkinHeader];
				AppearanceDefault res = new AppearanceDefault(element.Color.GetForeColor(), Color.Empty, Color.Empty, Color.Empty);
				res.Font = new Font(AppearanceObject.ControlAppearance.Font.FontFamily, 18);
				return res;
			}
		}
		protected virtual AppearanceObject CreateAppearanceGroupText() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { Owner.AppearanceGroupText }, DefaultAppearanceGroupText);
			if(res.TextOptions.Trimming == Trimming.Default)
				res.TextOptions.Trimming = Trimming.EllipsisCharacter;
			return res;
		}
		protected virtual AppearanceObject CreateAppearanceText() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { Owner.AppearanceText }, DefaultAppearanceText);
			return res;
		}
		public virtual void ClearBorderPainter() {
			this.borderPainter = null;
		}
		public Rectangle Bounds { get; private set; }
		public Rectangle ContentBounds { get; private set; }
		public Rectangle GroupsContentBounds { get; private set; }
		protected virtual int ClipBoundsTopIndent { get { return 5; } }
		public Rectangle GroupsClipBounds {
			get {
				return new Rectangle(Bounds.X, GroupsContentBounds.Y - ClipBoundsTopIndent, Bounds.Width, Bounds.Bottom - GroupsContentBounds.Top);
			}
		}
		protected int GetScaledVert(int value) {
			return (int)(value * Owner.ScaleFactor.Height);
		}
		protected int GetScaledHorz(int value) {
			return (int)(value * Owner.ScaleFactor.Width);
		}
		public Rectangle TextBounds { get; private set; }
		public int ItemSize {
			get { return GetScaledHorz(Owner.Properties.ItemSize); }
		}
		public int SmallItemSize {
			get { return (ItemSize - IndentBetweenItems) / 2; }
		}
		public virtual bool IsSmallItem(TileItem item) {
			return item.GetIsSmall();
		}
		public virtual bool IsLargeItem(TileItem item) {
			return item.GetIsLarge();
		}
		public virtual bool IsMediumItem(TileItem item) {
			return item.GetIsMedium();
		}
		public virtual Size GetItemSize(TileItemViewInfo itemInfo) {
			int baseItemSize = ItemSize;
			if(IsMediumItem(itemInfo.Item))
				return new Size(baseItemSize, baseItemSize);
			int smallItemSize = SmallItemSize;
			if(IsSmallItem(itemInfo.Item))
				return new Size(smallItemSize, smallItemSize);
			int largeItemWidth = Owner.Properties.LargeItemWidth < 0 ?
				baseItemSize * 2 + IndentBetweenItems : Owner.Properties.LargeItemWidth;
			largeItemWidth = Owner.Properties.LargeItemWidth < 0 ? largeItemWidth : GetScaledHorz(largeItemWidth);
			int largeItemHeight = ItemSize * itemInfo.RowCount;
			if(itemInfo.RowCount > 1)
				largeItemHeight += (itemInfo.RowCount - 1) * IndentBetweenItems;
			return new Size(largeItemWidth, largeItemHeight);
		}
		public virtual bool IsReady { get; protected internal set; }
		public void SetDirty() { this.IsReady = false; }
		GraphicsInfo gInfo;
		public GraphicsInfo GInfo {
			get {
				if(gInfo == null)
					gInfo = new GraphicsInfo();
				return gInfo;
			}
		}
		internal bool ShouldResumeAnimationsAfterExitDragMode {
			get;
			set;
		}
		internal bool HasAnimations(TileItemAnimationType type) {
			return HasAnimationsCore<TileItemBaseAnimationInfo>(type);
		}
		protected internal bool HasArrivalAnimation {
			get {
				return HasAnimationsCore<TileItemArrivalAnimationInfo>(null);
			}
		}
		bool HasAnimationsCore<T>(TileItemAnimationType? type) where T : TileItemBaseAnimationInfo {
			foreach(BaseAnimationInfo info in XtraAnimator.Current.Animations) {
				T itemInfo = info as T;
				if(type != null) {
					if(itemInfo != null && itemInfo.ItemAnimationType == type)
						return true;
				}
				else {
					if(itemInfo != null)
						return true;
				}
			}
			return false;
		}
		protected internal virtual Component GetTileComponent(TileItem tile) {
			return tile;
		}
		Dictionary<TileItem, TileItemViewInfo> itemViewInfoCache;
		protected Dictionary<TileItem, TileItemViewInfo> ItemViewInfoCache {
			get {
				if(itemViewInfoCache == null)
					itemViewInfoCache = new Dictionary<TileItem, TileItemViewInfo>();
				return itemViewInfoCache;
			}
		}
		protected virtual void ClearItemsCache() {
			ItemViewInfoCache.Clear();
		}
		public bool ShouldMakeTransition { get; set; }
		public Rectangle ClientBounds { get; private set; }
		public int GroupTextHeight { get; protected internal set; }
		protected internal TileControlLayoutCalculator Calculator {
			get;
			set;
		}
		protected internal TileControlLayoutCalculator PrevCalculator {
			get;
			set;
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			GInfo.AddGraphics(null);
			try {
				ResetAppearance();
				Bounds = bounds;
				CalcConstants();
				ClientBounds = CalcClientBounds(Bounds);
				ContentBounds = CalcContentBounds(ClientBounds);
				TextBounds = CalcTextBounds(ContentBounds);
				GroupsContentBounds = CalcGroupsContentBounds(ContentBounds, TextBounds);
				CalcGroupsLayoutCore();
				UpdateScrollParams();
				UpdateGroupsByScroll();
				IsReady = true;
				if(ShouldMakeTransition) {
					PrepareItemsForExitDragMode();
				}
				UpdateNavigationGrid();
			}
			finally {
				if(Owner.IsDesignMode)
					Owner.UpdateSmartTag();
				GInfo.ReleaseGraphics();
				UpdateVisualEffects(UpdateAction.EndUpdate);
			}
		}
		protected virtual void CalcGroupsLayoutCore() {
			CreateLayout();
			ClearGroups();
			CreateGroups();
			RealRowCount = CalcRealRowCount();
			LayoutGroups();
			UpdateGroupsLayout();
			UpdateGroupsTextWidth();
		}
		protected virtual TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new TileControlLayoutCalculator(viewInfo);
		}
		protected virtual void CreateLayout() {
			Calculator = GetNewLayoutCalculator(this);
			Calculator.CreateLayoutInfo(DragItem, DropTargetInfo);
		}
		protected virtual void SaveLayout() {
			PrevCalculator = Calculator.Clone();
		}
		protected TileItemViewInfo GetDragItemNewViewInfo() {
			if(DragItem == null)
				return null;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(itemInfo.Item == DragItem.Item)
						return itemInfo;
				}
			}
			return null;
		}
		protected internal virtual TileControlDropItemInfo GetDropItemInfo(TileControlDropInfo dropInfo) {
			if(dropInfo == null)
				return new TileControlDropItemInfo();
			TileControlDropSide dropSide = dropInfo.GetDropSide();
			TileGroupViewInfo groupInfo = dropInfo.ItemInfo != null ? dropInfo.ItemInfo.GroupInfo : dropInfo.GroupInfo;
			TileItemViewInfo dropItemInfo = dropInfo.ItemInfo != null ? dropInfo.ItemInfo : dropInfo.NearestItemInfo;
			if(dropItemInfo != null && IsSmallItem(dropItemInfo.Item) && !IsSmallItem(dropInfo.DragItem.Item)) {
				dropItemInfo = FindFirstItemInColumn(dropItemInfo);
			}
			if(dropItemInfo != null && IsLargeItem(dropInfo.DragItem.Item) && IsHorizontal) {
				dropItemInfo = FindFirstItemInRow(dropItemInfo);
			}
			int dropIndex = -1;
			if(groupInfo != null)
				dropIndex = dropItemInfo != null ? groupInfo.Items.IndexOf(dropItemInfo) : groupInfo.Items.Count;
			if(dropSide == TileControlDropSide.Right)
				dropIndex++;
			else if(dropSide == TileControlDropSide.Bottom) {
				TileItemViewInfo item = groupInfo.Items.GetItemFromBottom(dropItemInfo, 0);
				if(item == null)
					dropIndex = groupInfo.Items.Count;
				else
					dropIndex = item.GroupInfo.Items.IndexOf(item);
			}
			else if(dropSide == TileControlDropSide.Top || dropSide == TileControlDropSide.Left) {
				if(IsLargeItem(dropInfo.DragItem.Item)) {
					TileItemViewInfo item = groupInfo.Items.GetItemFromLeft(dropItemInfo);
					if(item != null)
						dropIndex = item.GroupInfo.Items.IndexOf(item);
				}
			}
			TileItemViewInfo dragItemInfo = GetDragItemNewViewInfo();
			if(dropItemInfo != null && dragItemInfo != null && dropItemInfo.Item == dragItemInfo.Item)
				return null;
			TileControlDropItemInfo res = new TileControlDropItemInfo();
			if(groupInfo != null && dropIndex >= 0 && dropIndex < groupInfo.Items.Count)
				res.DropItem = groupInfo.Items[dropIndex].Item;
			res.Group = groupInfo == null ? null : groupInfo.Group;
			res.NearestGroupInfo = dropInfo.NearestGroupInfo;
			res.GroupDropSide = dropInfo.GroupDropSide;
			return res;
		}
		private TileItemViewInfo FindFirstItemInRow(TileItemViewInfo dropItemInfo) {
			for(int i = dropItemInfo.GroupInfo.Items.IndexOf(dropItemInfo) - 1; i >= 0; i--) {
				TileItemViewInfo itemInfo = dropItemInfo.GroupInfo.Items[i];
				if(itemInfo.Position.Row != dropItemInfo.Position.Row)
					break;
				if(itemInfo.Position.Column == 0 && itemInfo.Position.SubColumn == 0 && itemInfo.Position.SubRow == 0)
					return itemInfo;
			}
			return dropItemInfo;
		}
		private TileItemViewInfo FindFirstItemInColumn(TileItemViewInfo dropItemInfo) {
			for(int i = dropItemInfo.GroupInfo.Items.IndexOf(dropItemInfo) - 1; i >= 0; i--) {
				TileItemViewInfo itemInfo = dropItemInfo.GroupInfo.Items[i];
				if(itemInfo.Position.Column != dropItemInfo.Position.Column || itemInfo.Position.Row != dropItemInfo.Position.Row)
					break;
				if(itemInfo.Position.SubColumn == 0 && itemInfo.Position.SubRow == 0)
					return itemInfo;
			}
			return dropItemInfo;
		}
		protected virtual void ResetAppearance() {
			this.appearanceGroupText = null;
			this.appearanceText = null;
		}
		protected virtual void UpdateGroupsTextWidth() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				groupInfo.UpdateTextWidth();
			}
		}
		protected virtual void CalcConstants() {
			GroupTextHeight = Owner.Properties.ShowGroupText ? AppearanceGroupText.CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Height : 0;
		}
		protected virtual int TextToGroupsIndent { get { return 16; } }
		protected virtual Rectangle CalcGroupsContentBounds(Rectangle contentBounds, Rectangle textBounds) {
			if(textBounds.Size.IsEmpty || !Owner.Properties.ShowText)
				return contentBounds;
			return new Rectangle(contentBounds.X, textBounds.Bottom + TextToGroupsIndent, contentBounds.Width, contentBounds.Bottom - textBounds.Bottom - TextToGroupsIndent);
		}
		protected virtual Rectangle CalcTextBounds(Rectangle bounds) {
			Size res = new Size(bounds.Width, AppearanceText.CalcTextSize(GInfo.Graphics, Owner.Text, bounds.Width).ToSize().Height);
			return new Rectangle(bounds.Location, res);
		}
		protected virtual void UpdateNavigationGrid() {
			if(Owner.Navigator.GetNavigationGridCore() != null) {
				Owner.Navigator.UpdateNavigationGrid();
			}
		}
		protected virtual Rectangle CalcClientBounds(Rectangle rect) {
			return BorderPainter.GetObjectClientRectangle(new BorderObjectInfoArgs(GInfo.Cache, PaintAppearance, rect));
		}
		public virtual HorzAlignment HorizontalContentAlignment {
			get {
				if(Owner.Properties.HorizontalContentAlignment == HorzAlignment.Default)
					return HorzAlignment.Center;
				return Owner.Properties.HorizontalContentAlignment;
			}
		}
		public virtual VertAlignment VerticalContentAlignment {
			get {
				if(Owner.Properties.VerticalContentAlignment == VertAlignment.Default) {
					return IsHorizontal ? VertAlignment.Center : VertAlignment.Top;
				}
				return Owner.Properties.VerticalContentAlignment;
			}
		}
		Rectangle GetGroupsBounds() {
			Rectangle groupsBounds = Rectangle.Empty;
			if(Groups.Count > 0) {
				if(IsHorizontal && IsRightToLeft)
					groupsBounds = new Rectangle(Groups[Groups.Count - 1].TotalBounds.Location, CalcBestContentSize());
				else
					groupsBounds = new Rectangle(Groups[0].TotalBounds.Location, CalcBestContentSize());
			}
			return groupsBounds;
		}
		protected virtual void UpdateGroupsLayout() {
			Rectangle groupsBounds = GetGroupsBounds();
			if(groupsBounds.IsEmpty)
				return;
			if(groupsBounds.Width < GroupsContentBounds.Width) {
				if(HorizontalContentAlignment == HorzAlignment.Center) {
					groupsBounds.X = GroupsContentBounds.X + (GroupsContentBounds.Width - groupsBounds.Width) / 2;
				}
				else if(HorizontalContentAlignment == HorzAlignment.Far) {
					if(IsRightToLeft)
						groupsBounds.X = GroupsContentBounds.X;
					else
						groupsBounds.X = GroupsContentBounds.Right - groupsBounds.Width;
				}
			}
			if(groupsBounds.Height < GroupsContentBounds.Height) {
				if(VerticalContentAlignment == VertAlignment.Center) {
					groupsBounds.Y = CalcGroupLocVertCenter(groupsBounds);
				}
				else if(VerticalContentAlignment == VertAlignment.Bottom) {
					groupsBounds.Y = GroupsContentBounds.Bottom - groupsBounds.Height;
				}
			}
			if(!CheckNeedUpdateGroupsLayout(groupsBounds))
				return;
			if(IsRightToLeft)
				UpdateGroupsLayout(new Point(groupsBounds.X - (GroupsContentBounds.Right - groupsBounds.Width), groupsBounds.Y - GroupsContentBounds.Y));
			else
				UpdateGroupsLayout(new Point(groupsBounds.X - GroupsContentBounds.X, groupsBounds.Y - GroupsContentBounds.Y));
		}
		bool CheckNeedUpdateGroupsLayout(Rectangle groupsBounds) {
			if(IsRightToLeft)
				return new Point(groupsBounds.Right, groupsBounds.Top) != new Point(GroupsContentBounds.Right, GroupsContentBounds.Top);
			else
				return groupsBounds.Location != GroupsContentBounds.Location;
		}
		protected virtual int CalcGroupLocVertCenter(Rectangle groupsBounds) {
			return GroupsContentBounds.Y + (GroupsContentBounds.Height - groupsBounds.Height) / 2;
		}
		protected virtual void UpdateGroupsLayoutInDragDrop() {
			UpdateGroupsLayout(new Point(ContentLocation.X - Groups[0].TotalBounds.X, ContentLocation.Y - Groups[0].TotalBounds.Y));
		}
		protected virtual void UpdateGroupsLayout(Point pt) {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				groupInfo.MakeOffset(pt.X, pt.Y);
			}
		}
		protected virtual Size CalcBestContentSize() {
			Size res = new Size();
			if(IsHorizontal) {
				if(IsRightToLeft)
					res.Width = Groups[0].TotalBounds.Right - Groups[Groups.Count - 1].TotalBounds.Left;
				else
					res.Width = Groups[Groups.Count - 1].TotalBounds.Right - Groups[0].TotalBounds.Left;
			}
			if(IsVertical)
				res.Height = Groups[Groups.Count - 1].TotalBounds.Bottom - Groups[0].TotalBounds.Top;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(IsHorizontal)
					res.Height = Math.Max(groupInfo.TotalBounds.Height, res.Height);
				if(IsVertical)
					res.Width = Math.Max(groupInfo.TotalBounds.Width, res.Width);
			}
			return res;
		}
		public virtual int GroupTextToItemsIndent { get { return Owner.Properties.ShowGroupText ? 16 : 0; } }
		protected internal virtual int AvailableRowCount {
			get { return (GroupsContentBounds.Height - GroupTextHeight - GroupTextToItemsIndent + IndentBetweenItems) / (GetItemHeight() + IndentBetweenItems); }
		}
		protected virtual int CalcRealRowCount() {
			if(Owner.Properties.Orientation == Orientation.Vertical)
				return CalcBestRowCount();
			int realRowCount = Math.Min(Owner.Properties.RowCount, AvailableRowCount);
			realRowCount = Math.Min(realRowCount, CalcBestRowCount());
			return Math.Max(1, realRowCount);
		}
		protected virtual int CalcBestRowCount() {
			int res = 0;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				res = Math.Max(res, groupInfo.CalcBestRowCount());
			}
			return res;
		}
		protected virtual void PrepareItemsForTransition() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.ShouldProcessItem || DragItem == itemInfo) continue;
					if(Bounds.IntersectsWith(itemInfo.PrevRenderImageBounds) || Bounds.IntersectsWith(itemInfo.Bounds)) {
						AddItemTransitionAnimation(itemInfo);
					}
				}
			}
			ShouldMakeTransition = false;
		}
		protected virtual void AddItemTransitionAnimation(TileItemViewInfo itemInfo) {
			itemInfo.RenderImageBounds = itemInfo.PrevRenderImageBounds;
			if(itemInfo.ShouldUseTransition) {
				AddItemBoundsAnimation(itemInfo, itemInfo.DragStateBounds, true, TileItemAnimationBehavior.HoldEnd);
				return;
			}
			AddItemBoundsAnimation(itemInfo, itemInfo.DragStateBounds, TileItemAnimationBehavior.HoldEnd);
		}
		protected void ClearOffset() {
			offset = 0;
		}
		int offset;
		public int Offset {
			get { return offset; }
			set {
				value = ConstraintOffset(value);
				if(Offset == value)
					return;
				int prevOffset = Offset;
				offset = value;
				if(IsReady) {
					OnOffsetChanged(prevOffset);
					UpdateVisualEffects(UpdateAction.Update);
				}
			}
		}
		protected internal virtual int MinOffset { get { return 0; } }
		protected internal virtual int MaxOffset {
			get {
				if(IsHorizontal)
					return Math.Max(0, ContentBestWidth - GroupsContentBounds.Width);
				return Math.Max(0, ContentBestHeight - GroupsContentBounds.Height);
			}
		}
		public virtual int ConstraintOffset(int offset) {
			return Math.Min(MaxOffset, Math.Max(MinOffset, offset));
		}
		protected virtual void UpdateGroupsByScroll() {
			OnOffsetChanged(0);
		}
		protected virtual void OnOffsetChanged(int prevOffset) {
			int delta = Offset - prevOffset;
			if(ShouldReverseScrollDueRTL) delta = -delta;
			int xoffset = 0;
			int yoffset = 0;
			if(IsHorizontal) {
				xoffset = -delta;
				ContentLocation = new Point(ContentLocation.X - delta, ContentLocation.Y);
			}
			else {
				yoffset = -delta;
				ContentLocation = new Point(ContentLocation.X, ContentLocation.Y - delta);
			}
			MakeGroupsOffset(xoffset, yoffset);
			UpdateCalculatorGroupsOffset();
			DoWindowScroll(-delta);
		}
		protected virtual bool UseOptimizedScrolling {
			get {
				return
					!Owner.IsDesignMode &&
					Owner.BackgroundImage == null &&
					Owner.ScrollMode != TileControlScrollMode.ScrollButtons &&
					Owner.Handler.State != TileControlHandlerState.DragMode;
			}
		}
		protected virtual void DoWindowScroll(int delta) {
			if(UseOptimizedScrolling && IsReady) {
				if(Owner.Properties.Orientation == Orientation.Horizontal)
					DoWindowScrollCore(delta, ClientBounds.X, true);
				else
					DoWindowScrollCore(delta, GroupsClipBounds.Y, false);
			}
			else {
				Owner.Invalidate(ClientBounds);
			}
		}
		protected virtual void DoWindowScrollCore(int delta, int start, bool horz) {
			int position = delta < 0 ? -delta + start : start;
			if(horz)
				WindowScroller.ScrollHorizontal(Owner.Control, GroupsClipBounds, position, delta);
			else
				WindowScroller.ScrollVertical(Owner.Control, GroupsClipBounds, position, delta);
		}
		protected virtual void MakeGroupsOffset(int xoffset, int yoffset) {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				groupInfo.MakeOffset(xoffset, yoffset);
			}
		}
		protected virtual void UpdateCalculatorGroupsOffset() {
			foreach(TileControlLayoutGroup group in Calculator.Groups) {
				group.Bounds = group.GroupInfo.Bounds;
				foreach(TileControlLayoutItem item in group.Items) {
					item.Bounds = item.ItemInfo.Bounds;
				}
			}
		}
		protected int ContentBestWidth {
			get {
				if(Groups.Count == 0)
					return 0;
				return CalcContentBestWidth();
			}
		}
		protected virtual int CalcContentBestWidth() {
			if(IsRightToLeft)
				return Groups[0].Bounds.Right - Groups[Groups.Count - 1].Bounds.Left;
			return Groups[Groups.Count - 1].Bounds.Right - Groups[0].Bounds.X;
		}
		protected int ContentBestHeight {
			get {
				if(Groups.Count == 0)
					return 0;
				return CalcContentBestHeight();
			}
		}
		protected virtual int CalcContentBestHeight() {
			return Groups[Groups.Count - 1].TotalBounds.Bottom - Groups[0].TotalBounds.Y;
		}
		protected virtual TileControlScrollMode ForceScrollMode { get { return TileControlScrollMode.Default; } }
		protected internal virtual TileControlScrollMode ScrollMode {
			get {
				if(Owner.IsDesignMode && (Owner.ScrollMode == TileControlScrollMode.Default || Owner.ScrollMode == TileControlScrollMode.None))
					return TileControlScrollMode.ScrollButtons;
				if(ForceScrollMode != TileControlScrollMode.Default)
					return ForceScrollMode;
				return Owner.ScrollMode == TileControlScrollMode.Default ? DefaultScrollMode : Owner.ScrollMode;
			}
		}
		protected virtual TileControlScrollMode DefaultScrollMode { get { return TileControlScrollMode.None; } }
		protected virtual void UpdateScrollParams() {
			if(ScrollMode == TileControlScrollMode.None) {
				UpdateScrollParamsNoScroll();
			}
			else if(ScrollMode == TileControlScrollMode.ScrollButtons) {
				UpdateScrollParamsButtons();
			}
			else {
				UpdateScrollParamsScrollBar();
			}
		}
		protected virtual void CreateScrollBar() {
			if(ScrollMode != TileControlScrollMode.ScrollBar)
				return;
			if(Owner.ScrollBar == null) {
				if(IsHorizontal)
					Owner.ScrollBar = CreateScrollBarCore(new HScrollBar());
				else
					Owner.ScrollBar = CreateScrollBarCore(new VScrollBar());
			}
			if(!Owner.ContainsControl(Owner.ScrollBar)) {
				Owner.AddControl(Owner.ScrollBar);
			}
		}
		protected internal virtual ScrollBarBase CreateScrollBarCore(ScrollBarBase scrollBase) {
			return ScrollBarBase.ApplyUIMode(scrollBase);
		}
		protected virtual void UpdateScrollParamsScrollBar() {
			CreateScrollBar();
			if(Owner.ScrollBar == null) return;
			Owner.ScrollBar.Minimum = 0;
			Owner.ScrollBar.LookAndFeel.ParentLookAndFeel = Owner.LookAndFeel;
			if(IsHorizontal) {
				Owner.ScrollBar.SetVisibility(ContentBestWidth >= GroupsContentBounds.Width);
				Owner.ScrollBar.Maximum = ContentBestWidth;
				Owner.ScrollBar.LargeChange = Math.Min(GroupsContentBounds.Width, Owner.ScrollBar.Maximum);
			}
			else {
				Owner.ScrollBar.SetVisibility(ContentBestHeight >= GroupsContentBounds.Height);
				Owner.ScrollBar.Maximum = ContentBestHeight;
				Owner.ScrollBar.LargeChange = Math.Min(GroupsContentBounds.Height, Owner.ScrollBar.Maximum);
			}
			Owner.ScrollBar.Bounds = CalcScrollBarBounds();
			Owner.ScrollBar.SmallChange = Owner.Properties.ItemSize + Owner.Properties.IndentBetweenItems;
		}
		protected virtual Rectangle CalcScrollBarBounds() {
			if(IsHorizontal)
				return new Rectangle(ClientBounds.X, ClientBounds.Bottom - Owner.ScrollBar.GetDefaultHorizontalScrollBarHeight(), ClientBounds.Width, Owner.ScrollBar.Height);
			int y = Owner.Properties.ShowText ? GroupsClipBounds.Top : ClientBounds.Y;
			int h = Owner.Properties.ShowText ? ClientBounds.Height - GroupsClipBounds.Top : ClientBounds.Height;
			return new Rectangle(ClientBounds.Right - Owner.ScrollBar.GetDefaultVerticalScrollBarWidth(), y, Owner.ScrollBar.Width, h);
		}
		public float BackArrowOpacity { get; private set; }
		public float ForwardArrowOpacity { get; private set; }
		public Rectangle BackArrowBounds {
			get;
			private set;
		}
		public Rectangle ForwardArrowBounds {
			get;
			private set;
		}
		public ObjectState BackArrowState {
			get;
			protected internal set;
		}
		public ObjectState ForwardArrowState {
			get;
			protected internal set;
		}
		protected internal virtual void UpdateScrollParamsButtons() {
			RemoveScrollBar();
			BackArrowBounds = CalcBackArrowBounds(GInfo.Graphics);
			ForwardArrowBounds = CalcForwardArrowBounds(GInfo.Graphics);
			BackArrowOpacity = CalcArrowOpacity();
			ForwardArrowOpacity = CalcArrowOpacity();
		}
		public virtual void UpdateButtonsState(Point point, MouseButtons buttons) {
			if(ScrollMode != TileControlScrollMode.ScrollButtons)
				return;
			BackArrowState = GetLeftButtonState(point, buttons);
			ForwardArrowState = GetRightButtonState(point, buttons);
		}
		protected virtual ObjectState GetLeftButtonState(Point point, MouseButtons buttons) {
			if(Owner.Position == 0)
				return ObjectState.Disabled;
			return GetButtonState(point, BackArrowOpacity, BackArrowBounds, buttons);
		}
		protected virtual ObjectState GetRightButtonState(Point point, MouseButtons buttons) {
			if(Owner.Position == MaxOffset)
				return ObjectState.Disabled;
			return GetButtonState(point, ForwardArrowOpacity, ForwardArrowBounds, buttons);
		}
		protected virtual ObjectState GetButtonState(Point point, float opacity, Rectangle bounds, MouseButtons buttons) {
			return opacity > 0.0f && bounds.Contains(point) && buttons == MouseButtons.Left ? ObjectState.Pressed : ObjectState.Normal;
		}
		#region Animation
		protected virtual void StartAnimation(float start, float end) {
			BaseAnimationInfo animInfo = CreateButtonsOpacityAnimationInfo(start, end);
			XtraAnimator.Current.AddAnimation(animInfo);
		}
		protected virtual BaseAnimationInfo CreateButtonsOpacityAnimationInfo(float start, float end) {
			return new FloatAnimationInfo(XtraAnimationObject, AnimationId, Owner.ScrollButtonFadeAnimationTime, start, end);
		}
		protected virtual void StopAnimation() {
			XtraAnimator.RemoveObject(XtraAnimationObject, AnimationId);
		}
		protected float CalcArrowOpacity() {
			float opacity = CalcArrowOpacityCore();
			if(!ShouldStartAnimation(opacity)) return opacity;
			StopAnimation();
			StartAnimation(currentOpacity, opacity);
			prevOpacity = opacity;
			return opacity;
		}
		protected virtual float CalcArrowOpacityCore() {
			if((IsHorizontal && ContentBestWidth < GroupsContentBounds.Width) || (IsVertical && ContentBestHeight < GroupsContentBounds.Height))
				return 0.0f;
			Point pt = Owner.PointToClient(Control.MousePosition);
			return Owner.ClientRectangle.Contains(pt) ? 1f : 0f;
		}
		float prevOpacity = 0;
		protected bool ShouldStartAnimation(float opacity) {
			return Math.Abs(opacity - prevOpacity) > 0.01; 
		}
		public float ScrollArrowsCurrentOpacity { get { return currentOpacity; } }
		float currentOpacity = 0;
		protected virtual float CalcCurrentArrowOpacity() {
			float opacity = CalcArrowOpacity();
			FloatAnimationInfo info = XtraAnimator.Current.Get(XtraAnimationObject, AnimationId) as FloatAnimationInfo;
			if(info == null) return opacity;
			if(!info.IsStarted) return this.currentOpacity;
			this.currentOpacity = info.Value;
			return this.currentOpacity;
		}
		object animationId = null;
		protected object AnimationId {
			get {
				if(this.animationId == null) this.animationId = new object();
				return animationId;
			}
		}
		protected ISupportXtraAnimation XtraAnimationObject { get { return Owner as ISupportXtraAnimation; } }
		#endregion
		ImageAttributes GetOpacityAttributes(float opacity) {
			ColorMatrix m = new ColorMatrix();
			m.Matrix33 = opacity;
			ImageAttributes attr = new ImageAttributes();
			attr.SetColorMatrix(m);
			return attr;
		}
		protected internal SkinElementInfo GetBackArrowInfo() {
			string skinName = EditorsSkins.SkinSliderLeftArrow;
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[skinName];
			if(elem == null)
				elem = CommonSkins.GetSkin(DefaultSkinProvider.Default)[CommonSkins.SkinButton];
			SkinElementInfo info = new SkinElementInfo(elem, BackArrowBounds);
			info.ImageIndex = -1;
			info.State = BackArrowState;
			info.Attributes = GetOpacityAttributes(CalcCurrentArrowOpacity());
			return info;
		}
		protected internal SkinElementInfo GetForwardArrowInfo() {
			string skinName = EditorsSkins.SkinSliderRightArrow;
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[skinName];
			if(elem == null)
				elem = CommonSkins.GetSkin(DefaultSkinProvider.Default)[CommonSkins.SkinButton];
			SkinElementInfo info = new SkinElementInfo(elem, ForwardArrowBounds);
			info.ImageIndex = -1;
			info.State = ForwardArrowState;
			info.Attributes = GetOpacityAttributes(CalcCurrentArrowOpacity());
			return info;
		}
		protected internal Size GetBackArrowSize(Graphics g) {
			Size res = ObjectPainter.CalcObjectMinBounds(g, SkinElementPainter.Default, GetBackArrowInfo()).Size;
			if(IsVertical)
				return new Size(res.Height, res.Width);
			return res;
		}
		protected internal Size GetForwardArrowSize(Graphics g) {
			Size res = ObjectPainter.CalcObjectMinBounds(g, SkinElementPainter.Default, GetForwardArrowInfo()).Size;
			if(IsVertical)
				return new Size(res.Height, res.Width);
			return res;
		}
		int ArrowIndent { get { return 16; } }
		protected virtual Rectangle CalcForwardArrowBounds(Graphics g) {
			Rectangle r = new Rectangle(Point.Empty, GetForwardArrowSize(g));
			if(IsHorizontal) {
				r.X = GroupsContentBounds.Right - ArrowIndent - r.Width;
				r.Y = GroupsContentBounds.Y + (GroupsContentBounds.Height - r.Height) / 2;
			}
			else {
				r.X = GroupsContentBounds.X + (GroupsContentBounds.Width - r.Width) / 2;
				r.Y = GroupsContentBounds.Bottom - ArrowIndent - r.Height;
			}
			return r;
		}
		protected virtual Rectangle CalcBackArrowBounds(Graphics g) {
			Rectangle r = new Rectangle(Point.Empty, GetBackArrowSize(g));
			if(IsHorizontal) {
				r.X = GroupsContentBounds.X + ArrowIndent;
				r.Y = GroupsContentBounds.Y + (GroupsContentBounds.Height - r.Height) / 2;
			}
			else {
				r.X = GroupsContentBounds.X + (GroupsContentBounds.Width - r.Width) / 2;
				r.Y = GroupsContentBounds.Y + ArrowIndent;
			}
			return r;
		}
		protected virtual void RemoveScrollBar() {
			if(Owner.ScrollBar != null && Owner.ContainsControl(Owner.ScrollBar))
				Owner.ScrollBar = null;
		}
		protected virtual void UpdateScrollParamsNoScroll() {
			RemoveScrollBar();
		}
		protected virtual void LayoutGroups(bool calcFullLayout) {
			Calculator = GetNewLayoutCalculator(this);
			Calculator.CalcGroupsLayout(DragItem, DropTargetInfo);
			if(calcFullLayout) {
				foreach(TileControlLayoutGroup group in Calculator.Groups) {
					group.GroupInfo.LayoutGroup(Calculator, group);
				}
			}
		}
		protected virtual void LayoutGroups() {
			LayoutGroups(true);
		}
		protected internal virtual Point GetStartPoint() {
			return IsRightToLeft ? new Point(GroupsContentBounds.Right, GroupsContentBounds.Top) : GroupsContentBounds.Location;
		}
		public int RealRowCount { get; protected internal set; }
		public int RealColumnCount { get; protected internal set; }
		protected virtual Rectangle CalcContentBounds(Rectangle rect) {
			CreateScrollBar();
			rect = ConstraintByScrollBar(rect);
			Padding p = Owner.Properties.Padding;
			rect.X += p.Left;
			rect.Y += p.Top;
			rect.Width -= p.Horizontal;
			rect.Height -= p.Vertical;
			rect = ConstraintPadding(rect);
			return rect;
		}
		protected virtual Rectangle ConstraintByScrollBar(Rectangle rect) {
			if(ScrollMode == TileControlScrollMode.ScrollBar && Owner.ScrollBar != null && Owner.ScrollBar.Visible) {
				if(!Owner.ScrollBar.IsOverlapScrollBar) {
					if(IsHorizontal)
						rect.Height -= Owner.ScrollBar.Height;
					else
						rect.Width -= Owner.ScrollBar.Width;
				}
			}
			return rect;
		}
		protected virtual Rectangle ConstraintPadding(Rectangle rect) {
			ITileControlProperties prop = Owner.Properties;
			if(!prop.AllowSelectedItem) return rect;
			int selectionW = GetSelectionBorderWidth();
			rect.X += selectionW;
			rect.Y += selectionW;
			rect.Width -= selectionW * 2;
			rect.Height -= selectionW * 2;
			return rect;
		}
		const int defSelectFrameWidth = 2;
		int GetSelectionBorderWidth() {
			SkinElement elem = EditorsSkins.GetSkin(Owner.LookAndFeel)[EditorsSkins.SkinTileItemSelected];
			if(elem == null)
				return defSelectFrameWidth;
			return elem.Properties.GetInteger(EditorsSkins.OptTileItemBorderIndent);
		}
		protected virtual void CreateGroups() {
			foreach(TileControlLayoutGroup group in Calculator.Groups) {
				TileGroupViewInfo groupInfo = group.Group.CreateViewInfo();
				group.GroupInfo = groupInfo;
				groupInfo.ControlInfo = this;
				Groups.Add(groupInfo);
			}
		}
		protected void ClearGroups() {
			Groups.Clear();
		}
		public virtual int IndentBetweenItems { get { return Owner.Properties.IndentBetweenItems; } }
		protected virtual void CalcHitInfo(TileControlHitInfo hitInfo) {
			if(!hitInfo.ContainsSet(Bounds, TileControlHitTest.Control))
				return;
			hitInfo.ViewInfo = this;
			if(CheckTextHit(hitInfo) || CheckBackArrowHit(hitInfo) || CheckForwardArrowHit(hitInfo))
				return;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(hitInfo.ContainsSet(groupInfo.TotalBounds, TileControlHitTest.Group)) {
					hitInfo.GroupInfo = groupInfo;
					groupInfo.CalcHitInfo(hitInfo);
				}
			}
		}
		protected virtual bool CheckTextHit(TileControlHitInfo hitInfo) {
			if(!Owner.Properties.ShowText) return false;
			Rectangle headerRect = new Rectangle(0, 0, GroupsClipBounds.Width, GroupsClipBounds.Top);
			return headerRect.Contains(hitInfo.HitPoint);
		}
		protected virtual bool CheckBackArrowHit(TileControlHitInfo hitInfo) {
			if(BackArrowOpacity > 0.0f && BackArrowBounds.Contains(hitInfo.HitPoint)) {
				hitInfo.HitTest = TileControlHitTest.BackArrow;
				return true;
			}
			return false;
		}
		protected virtual bool CheckForwardArrowHit(TileControlHitInfo hitInfo) {
			if(ForwardArrowOpacity > 0.0f && ForwardArrowBounds.Contains(hitInfo.HitPoint)) {
				hitInfo.HitTest = TileControlHitTest.ForwardArrow;
				return true;
			}
			return false;
		}
		protected internal virtual TileControlHitInfo CreateHitInfo() {
			return new TileControlHitInfo();
		}
		public virtual TileControlHitInfo CalcHitInfo(Point point, bool checkDragBounds) {
			TileControlHitInfo hitInfo = CreateHitInfo();
			hitInfo.CheckDropBounds = checkDragBounds;
			hitInfo.HitPoint = point;
			CalcHitInfo(hitInfo);
			return hitInfo;
		}
		public virtual TileControlHitInfo CalcHitInfo(Point point) {
			return CalcHitInfo(point, false);
		}
		TileControlHitInfo pressedInfo = TileControlHitInfo.Empty;
		public void SetPressedInfoCore(TileControlHitInfo info) {
			if(info == null)
				info = TileControlHitInfo.Empty;
			this.pressedInfo = info;
		}
		public TileControlHitInfo PressedInfo {
			get { return pressedInfo; }
			set {
				if(value == null)
					value = TileControlHitInfo.Empty;
				if(PressedInfo.Equals(value))
					return;
				TileControlHitInfo oldInfo = PressedInfo;
				pressedInfo = value;
				OnPressedInfoChanged(oldInfo, PressedInfo);
			}
		}
		TileControlHitInfo hoverInfo = TileControlHitInfo.Empty;
		public void SetHoverInfoCore(TileControlHitInfo info) {
			if(info == null)
				info = TileControlHitInfo.Empty;
			this.hoverInfo = info;
		}
		public TileControlHitInfo HoverInfo {
			get { return hoverInfo; }
			set {
				if(value == null)
					value = TileControlHitInfo.Empty;
				if(HoverInfo.Equals(value))
					return;
				TileControlHitInfo oldInfo = HoverInfo;
				hoverInfo = value;
				OnHoverInfoChanged(oldInfo, HoverInfo);
			}
		}
		public Rectangle LeftScrollAreaBounds {
			get {
				if(IsHorizontal)
					return new Rectangle(Bounds.X, Bounds.Y, ScrollAreaWidth, Bounds.Height);
				else
					return new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, ScrollAreaWidth);
			}
		}
		public Rectangle RightScrollAreaBounds {
			get {
				if(IsHorizontal)
					return new Rectangle(Bounds.Right - ScrollAreaWidth, Bounds.Y, ScrollAreaWidth, Bounds.Height);
				else
					return new Rectangle(Bounds.X, Bounds.Bottom - ScrollAreaWidth, Bounds.Width, ScrollAreaWidth);
			}
		}
		System.Windows.Forms.Timer Timer { get; set; }
		MethodInvoker ScrollMethod { get; set; }
		protected internal virtual void StopScroll() {
			if(Timer != null)
				Timer.Dispose();
			Timer = null;
		}
		protected internal void StartBackScroll() {
			StartScrollTimer(new MethodInvoker(OnBackScroll));
		}
		protected internal void StartForwardScroll() {
			StartScrollTimer(new MethodInvoker(OnForwardScroll));
		}
		protected virtual void StartScrollTimer(MethodInvoker scrollMethod) {
			if(Timer != null && Timer.Enabled)
				return;
			ScrollMethod = scrollMethod;
			Timer = new System.Windows.Forms.Timer();
			Timer.Interval = 30;
			Timer.Tick += new EventHandler(OnScrollTimerTick);
			Timer.Start();
		}
		protected virtual void OnBackScroll() {
			Owner.Position -= ScrollValue;
			if(Owner.Position == MinOffset)
				StopScroll();
			UpdateDragItemBoundsAfterScroll();
		}
		protected virtual void OnForwardScroll() {
			Owner.Position += ScrollValue;
			if(Owner.Position == MaxOffset)
				StopScroll();
			UpdateDragItemBoundsAfterScroll();
		}
		void UpdateDragItemBoundsAfterScroll() {
			if(DragItem != null)
				if(Owner.Handler.TouchState == TileControlHandler.TileControlTouchState.Drag)
					Owner.Handler.UpdateDragItemBounds(Owner.Handler.TouchDragPoint);
				else
					Owner.Handler.UpdateDragItemBounds(Owner.PointToClient(System.Windows.Forms.Control.MousePosition));
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if(HasAnimations(TileItemAnimationType.Arrival)) {
				return;
			}
			ScrollMethod.Invoke();
		}
		protected virtual void OnHoverInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
			if(oldInfo.ItemInfo != null) {
				AddItemHoverAnimation(oldInfo.ItemInfo, false);
			}
			if(newInfo.ItemInfo != null) {
				AddItemHoverAnimation(newInfo.ItemInfo, true);
			}
		}
		protected internal virtual int TileHoverAnimationLength {
			get { return 1000; }
		}
		protected internal virtual void AddItemHoverAnimation(TileItemViewInfo itemInfo, bool isHovered) { AddItemHoverAnimation(itemInfo, isHovered, false); }
		protected internal virtual void AddItemHoverAnimation(TileItemViewInfo itemInfo, bool isHovered, bool forceAddAnimation) {
			if(itemInfo == null || itemInfo.Item == null || itemInfo.Item.Group == null)
				return;
			if(!itemInfo.AllowSelectAnimation)
				return;
			if(!Owner.Properties.AllowItemHover)
				return;
			TileItemContentAnimationInfoBase anim = XtraAnimator.Current.Get(Owner.AnimationHost, itemInfo.Item) as TileItemContentAnimationInfoBase;
			if(anim != null && !forceAddAnimation) {
				anim.DelayedIsHovered = isHovered;
				anim.ShouldStartHoverAnimation = true;
				return;
			}
			var boundsanim = XtraAnimator.Current.Get(Owner.AnimationHost, itemInfo.Item) as TileItemBoundsAnimationInfo;
			if(boundsanim != null) {
				return;
			}
			float prevHoverOpacity = itemInfo.HoverOpacity;
			itemInfo.IsHoveringAnimation = false;
			itemInfo.ForceIsHovered = !isHovered;
			itemInfo.HoverOpacity = isHovered ? 0.0f : 1.0f;
			itemInfo.ForceUpdateAppearanceColors();
			itemInfo.PrevRenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.ForceIsHovered = isHovered;
			itemInfo.HoverOpacity = isHovered ? 1.0f : 0.0f;
			itemInfo.ForceUpdateAppearanceColors();
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.ForceIsHovered = null;
			itemInfo.HoverOpacity = prevHoverOpacity;
			itemInfo.ForceUpdateAppearanceColors();
			itemInfo.IsHoveringAnimation = true;
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, itemInfo.Item.HoverAnimationId);
			XtraAnimator.Current.AddAnimation(new TileHoverOpacityAnimationInfo(itemInfo, TileHoverAnimationLength, isHovered));
		}
		protected virtual void OnPressedInfoChanged(TileControlHitInfo oldInfo, TileControlHitInfo newInfo) {
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
				StartBackScroll();
			}
			else if(newInfo.InForwardArrow) {
				StartForwardScroll();
			}
		}
		protected virtual void PressItem(TileControlHitInfo newInfo) {
			canStartPressAnimation = true;
			if(newInfo.ItemInfo.Item.Enabled)
				newInfo.ItemInfo.Item.OnItemPress();
			newInfo.ItemInfo.ForceUpdateAppearanceColors();
			if(Owner.Properties.AllowSelectedItem) {
				RemoveHoverAnimation(newInfo.ItemInfo, false);
				return;
			}
			RemoveAnimation(newInfo.ItemInfo);
			if(canStartPressAnimation)					  
				AddPressItemAnimation(newInfo);
		}
		protected internal void RemoveAnimation(TileItemViewInfo itemInfo) {
			if(itemInfo.IsChangingContent)
				FinishContentChangingAnimation(itemInfo);
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, itemInfo.Item);
		}
		protected internal bool canStartPressAnimation;
		protected virtual void UnPressItem(TileControlHitInfo oldInfo) {
			canStartPressAnimation = false;
			oldInfo.ItemInfo.ForceUpdateAppearanceColors();
			Owner.Invalidate(oldInfo.ItemInfo.Bounds);
			if(Owner.Properties.AllowSelectedItem)
				return;
			RemoveAnimation(oldInfo.ItemInfo);
			AddUnPressItemAnimation(oldInfo);
		}
		protected virtual void AddUnPressItemAnimation(TileControlHitInfo hitInfo) {
			if(Owner.IsDesignMode)
				return;
			if(!hitInfo.ItemInfo.AllowSelectAnimation)
				return;
			FinishContentChangingAnimation(hitInfo.ItemInfo);
			if(hitInfo.ItemInfo.RenderImage == null)
				hitInfo.ItemInfo.RenderImage = RenderItemToBitmap(hitInfo.ItemInfo);
			XtraAnimator.Current.AddAnimation(new TileItemBoundsAnimationInfo(hitInfo.ItemInfo, PressedItemBoundsAnimationLength, TileItemAnimationBehavior.Simple, TileItemAnimationType.Unpress, hitInfo.ItemInfo.Bounds, false) { StartContentOffset = this.Offset });
		}
		protected virtual void FinishContentChangingAnimation(TileItemViewInfo itemInfo) {
			TileItemContentAnimationInfoBase info = XtraAnimator.Current.Get(Owner.AnimationHost, itemInfo.Item) as TileItemContentAnimationInfoBase;
			if(info != null) {
				XtraAnimator.Current.Animations.Remove(info);
				itemInfo.UseRenderImage = false;
				itemInfo.IsChangingContent = false;
				itemInfo.PrevRenderImage = null;
				Owner.Invalidate(itemInfo.Bounds);
			}
		}
		protected virtual void AddPressItemAnimation(TileControlHitInfo hitInfo) {
			RemoveHoverAnimation(hitInfo.ItemInfo, false);
			if(Owner.IsDesignMode)
				return;
			if(!hitInfo.ItemInfo.AllowSelectAnimation)
				return;
			FinishContentChangingAnimation(hitInfo.ItemInfo);
			hitInfo.ItemInfo.RenderImage = RenderItemToBitmap(hitInfo.ItemInfo);
			hitInfo.ItemInfo.RenderImageBounds = hitInfo.ItemInfo.Bounds;
			XtraAnimator.Current.AddAnimation(new TileItemBoundsAnimationInfo(hitInfo.ItemInfo, PressedItemBoundsAnimationLength, TileItemAnimationBehavior.HoldEnd, TileItemAnimationType.Press, hitInfo.ItemInfo.PressedBounds, false));
		}
		protected internal virtual Bitmap RenderItemToBitmap(TileItemViewInfo itemInfo) {
			Bitmap bmp = new Bitmap(itemInfo.Bounds.Width, itemInfo.Bounds.Height);
			bool oldValue = itemInfo.UseRenderImage;
			itemInfo.UseRenderImage = false;
			try {
				itemInfo.RenderToBitmap = true;
				using(Graphics gimage = Graphics.FromImage(bmp)) {
					if(UseAdvancedTextRendering)
						DoRenderBuffered(itemInfo, bmp.Size, gimage);
					else
						DoRenderNoBuffered(itemInfo, gimage);
				}
			}
			finally {
				itemInfo.RenderToBitmap = false;
				itemInfo.UseRenderImage = oldValue;
				itemInfo.AllowItemCheck = true;
			}
			return bmp;
		}
		void DoRenderBuffered(TileItemViewInfo itemInfo, Size size, Graphics gimage) {
			using(BufferedGraphics bfg = BufferedGraphicsManager.Current.Allocate(gimage, new Rectangle(Point.Empty, size))) {
				Graphics graphics = bfg.Graphics;
				using(GraphicsCache cache = new GraphicsCache(graphics)) {
					RenderItemToBitmapCore(itemInfo, cache);
					bfg.Render();
				}
			}
		}
		void DoRenderNoBuffered(TileItemViewInfo itemInfo, Graphics gimage) {
			using(GraphicsCache cache = new GraphicsCache(gimage)) {
				RenderItemToBitmapCore(itemInfo, cache);
			}
		}
		protected void RenderItemToBitmapCore(TileItemViewInfo itemInfo, GraphicsCache cache) {
			cache.Graphics.TranslateTransform(-itemInfo.Bounds.X, -itemInfo.Bounds.Y);
			TileControlInfoArgs e = new TileControlInfoArgs(cache, this);
			cache.Graphics.Clear(Color.Transparent);
			if(itemInfo.PaintAppearance.BackColor.A < 255 ||
				(!itemInfo.PaintAppearance.BackColor2.IsEmpty && itemInfo.PaintAppearance.BackColor2.A < 255))
				cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			Owner.SourcePainter.DrawItem(e, itemInfo);
		}
		protected internal Point ContentLocation { get; set; }
		protected internal virtual void PrepareItemsForEnterDragMode() {
			ShouldResumeAnimationsAfterExitDragMode = true;
			SuspendContentAnimation();
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(itemInfo.IsVisible) {
						if(!itemInfo.IsDragging)
							AddEnterDragModeItemAnimation(itemInfo);
						else
							PrepareDraggedItem(itemInfo);
					}
					else
						PrepareEnterDragModeItemWithoutAnimation(itemInfo);
				}
			}
		}
		protected virtual void PrepareDraggedItem(TileItemViewInfo itemInfo) {
			RemoveHoverAnimation(itemInfo, false);
			itemInfo.UseRenderImage = false;
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.UseRenderImage = true;
			itemInfo.RenderImageBounds = itemInfo.Bounds;
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds, int milliseconds) {
			AddItemBoundsAnimation(itemInfo, endBounds, false, milliseconds, TileItemAnimationBehavior.Simple);
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds) {
			AddItemBoundsAnimation(itemInfo, endBounds, false, TileItemTransitionLength, TileItemAnimationBehavior.Simple);
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds, TileItemAnimationBehavior behavior) {
			AddItemBoundsAnimation(itemInfo, endBounds, false, TileItemTransitionLength, behavior);
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds, bool useTransition) {
			AddItemBoundsAnimation(itemInfo, endBounds, useTransition, TileItemTransitionLength, TileItemAnimationBehavior.Simple);
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds, bool useTransition, TileItemAnimationBehavior behavior) {
			AddItemBoundsAnimation(itemInfo, endBounds, useTransition, TileItemTransitionLength, behavior);
		}
		protected virtual void AddItemBoundsAnimation(TileItemViewInfo itemInfo, Rectangle endBounds, bool useTransition, int milliseconds, TileItemAnimationBehavior behavior) {
			if(!itemInfo.AllowAnimation)
				return;
			if(itemInfo.RenderImage == null)
				itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.UseRenderImage = true;
			BaseAnimationInfo info = null;
			if(!useTransition)
				info = new TileItemBoundsSplineAnimationInfo(itemInfo, milliseconds, behavior, TileItemAnimationType.Arrival, endBounds, false);
			else
				info = new TileItemTransitionAnimationInfo(itemInfo, milliseconds, behavior, TileItemAnimationType.Arrival, endBounds);
			XtraAnimator.Current.AddAnimation(info);
		}
		public virtual void PrepareItemsForExitDragMode() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(itemInfo.IsVisible) {
						itemInfo.UseOptimizedRenderImage = false;
						RemoveTransitionAnimation(itemInfo);
						AddItemBoundsAnimation(itemInfo, itemInfo.Bounds);
					}
					else
						PrepareExitDragModeItemWithoutAnimation(itemInfo);
				}
			}
			ShouldMakeTransition = false;
		}
		protected virtual void RemoveTransitionAnimation(TileItemViewInfo itemInfo) {
			if(itemInfo == null) return;
			var anim = XtraAnimator.Current.Get(Owner.AnimationHost, itemInfo.Item);
			if(anim is TileItemBoundsAnimationInfo)
				XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, itemInfo.Item);
			itemInfo.IsInTransition = false;
		}
		protected virtual void AddExitDragModeItemAnimation(TileItemViewInfo itemInfo) {
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			XtraAnimator.Current.AddAnimation(new TileItemBoundsAnimationInfo(itemInfo, DragModeItemBoundsAnimationLength, TileItemAnimationBehavior.Simple, TileItemAnimationType.ExitDrag, itemInfo.Bounds, true));
		}
		private void PrepareEnterDragModeItemWithoutAnimation(TileItemViewInfo itemInfo) {
			if(itemInfo.IsDragging)
				return;
			itemInfo.UseRenderImage = true;
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.RenderImageBounds = itemInfo.DragStateBounds;
			itemInfo.UseOptimizedRenderImage = true;
		}
		private void PrepareExitDragModeItemWithoutAnimation(TileItemViewInfo itemInfo) {
			itemInfo.UseRenderImage = false;
			itemInfo.UseOptimizedRenderImage = false;
		}
		protected internal virtual void RemoveHoverAnimation(TileItemViewInfo itemInfo) {
			RemoveHoverAnimation(itemInfo, true);
		}
		protected internal virtual void RemoveHoverAnimation(TileItemViewInfo itemInfo, bool resetOpacity) {
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, itemInfo.Item.HoverAnimationId);
			itemInfo.IsHoveringAnimation = false;
			if(resetOpacity) itemInfo.HoverOpacity = 0.0f;
		}
		protected virtual void AddEnterDragModeItemAnimation(TileItemViewInfo itemInfo) {
			RemoveHoverAnimation(itemInfo);
			FinishContentChangingAnimation(itemInfo);
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.RenderImageBounds = itemInfo.Bounds;
			XtraAnimator.Current.AddAnimation(new TileItemBoundsAnimationInfo(itemInfo, DragModeItemBoundsAnimationLength, TileItemAnimationBehavior.HoldEnd, TileItemAnimationType.EnterDrag, itemInfo.DragStateBounds, true));
		}
		private List<TileItemViewInfo> GetVisibleItems() {
			List<TileItemViewInfo> res = new List<TileItemViewInfo>();
			bool wasItemVisible = false;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.IsVisible)
					continue;
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.ShouldProcessItem) continue;
					if(!itemInfo.IsVisible) {
						if(wasItemVisible)
							break;
					}
					if(itemInfo.IsVisible)
						res.Add(itemInfo);
					wasItemVisible = true;
				}
			}
			return res;
		}
		protected internal TileItemViewInfo DragItem {
			get { return AllowDragCore ? PressedInfo.ItemInfo : null; }
		}
		public virtual Point PointToContent(Point point) {
			return point;
		}
		protected internal TileControlDropInfo GetDropInfo(TileItemViewInfo dragItem) {
			TileItemViewInfo itemInfo = GetDropItemByPoint(dragItem.DragOrigin);
			TileControlDropInfo info = new TileControlDropInfo();
			info.DragItem = dragItem;
			info.Position = GetPositionByPoint(dragItem.DragOrigin);
			if(itemInfo != null) {
				info.ItemInfo = itemInfo;
			}
			else {
				info.NearestItemInfo = GetNearestItemByPoint(dragItem.DragOrigin);
				info.GroupInfo = GetDropGroupInfoByPoint(dragItem.DragOrigin);
				if(info.GroupInfo == null) {
					info.NearestGroupInfo = GetNearestGroupByPoint(dragItem.DragOrigin);
					info.CalcDropGroupSide(dragItem.DragOrigin, AllGroupsAreEmpty, Owner.Properties.Orientation);
				}
				info.InsertIntoGroup = true;
			}
			return info;
		}
		private TileGroupViewInfo GetNearestGroupByPoint(System.Drawing.Point point) {
			if(AllGroupsAreEmpty) {
				if(Groups.Count > 0)
					return Groups[0];
			}
			Orientation orientation = Owner.Properties.Orientation;
			int boundsXY;
			int pointXY;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(orientation == Orientation.Horizontal) {
					boundsXY = groupInfo.Bounds.X;
					pointXY = point.X;
				}
				else {
					boundsXY = groupInfo.Bounds.Y;
					pointXY = point.Y;
				}
				if(boundsXY >= pointXY &&
					((groupInfo.Items.Count > 0) && !(groupInfo.Items.Count == 1 && groupInfo.Items[0].Item == DragItem.Item)))
					return groupInfo;
			}
			return GetLastNonEmptyGroup();
		}
		protected internal TileGroupViewInfo GetLastNonEmptyGroup() {
			if(Groups.Count > 0)
				for(int i = Groups.Count - 1; i > -1; i--) {
					if(Groups[i].Items.Count > 0)
						return Groups[i];
				}
			return null;
		}
		protected internal bool AllGroupsAreEmpty {
			get {
				foreach(TileGroupViewInfo groupInfo in Groups) {
					if(groupInfo.Items.Count > 0)
						return false;
				}
				return true;
			}
		}
		private TileGroupViewInfo GetDropGroupInfoByPoint(System.Drawing.Point point) {
			for(int i = 0; i < Groups.Count; i++) {
				if(Owner.Properties.Orientation == Orientation.Horizontal) {
					if(Groups[i].Bounds.X <= point.X && Groups[i].Bounds.Right >= point.X)
						return Groups[i];
				}
				else if(Groups[i].Bounds.Y <= point.Y && Groups[i].Bounds.Bottom >= point.Y)
					return Groups[i];
			}
			return null;
		}
		private TileGroupViewInfo GetGroupInfoByPoint(System.Drawing.Point point) {
			for(int i = 0; i < Groups.Count; i++) {
				if(Groups[i].Bounds.Contains(point))
					return Groups[i];
			}
			return null;
		}
		protected virtual List<TileControlDropInfo> GetDropInfoFromGroups(TileItemViewInfo dragItem) {
			List<TileControlDropInfo> res = new List<TileControlDropInfo>();
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(groupInfo.Bounds.Contains(dragItem.DragOrigin) && groupInfo.Group.Items.Count == 0 && Owner.IsDesignMode) {
					res.Add(new TileControlDropInfo() { GroupInfo = groupInfo, InsertIntoGroup = true });
					break;
				}
				if(groupInfo.OuterBounds.Contains(dragItem.DragOrigin) && !groupInfo.Bounds.Contains(dragItem.DragOrigin)) {
					AddItemsInGroupToDropInfo(groupInfo, res);
					int groupIndex = GetSecondDropGroupIndex(groupInfo, dragItem);
					if(groupIndex < 0 || groupIndex >= Groups.Count)
						break;
					AddItemsInGroupToDropInfo(Groups[groupIndex], res);
				}
			}
			return res;
		}
		protected int GetSecondDropGroupIndex(TileGroupViewInfo groupInfo, TileItemViewInfo dragItem) {
			int groupIndex = Groups.IndexOf(groupInfo);
			if(IsHorizontal)
				return groupIndex + (groupInfo.Origin.X < dragItem.DragOrigin.X ? +1 : -1);
			return groupIndex + (groupInfo.Origin.Y < dragItem.DragOrigin.Y ? +1 : -1);
		}
		protected virtual void AddItemsInGroupToDropInfo(TileGroupViewInfo groupInfo, List<TileControlDropInfo> res) {
			foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
				if(!itemInfo.ShouldProcessItem) continue;
				if(itemInfo.IsDragging)
					continue;
				res.Add(new TileControlDropInfo() { ItemInfo = itemInfo, GroupInfo = groupInfo });
			}
		}
		protected internal virtual List<TileControlDropInfo> GetDropInfoForLargeItemVertical(TileItemViewInfo dragItem) {
			List<TileControlDropInfo> res = new List<TileControlDropInfo>();
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Bounds.Contains(dragItem.DragOrigin))
					continue;
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.DropBounds.Contains(dragItem.DragOrigin) || itemInfo.IsDragging)
						continue;
					TileControlDropInfo dropInfo = new TileControlDropInfo() { ItemInfo = itemInfo };
					res.Add(dropInfo);
					TileItemViewInfo item2 = null;
					if(dropInfo.GetDropSide() == TileControlDropSide.Top) {
						item2 = groupInfo.Items.GetItemFromTop(itemInfo, 0);
					}
					else {
						item2 = groupInfo.Items.GetItemFromBottom(itemInfo, 0);
					}
					if(item2 != null && !item2.IsDragging)
						res.Add(new TileControlDropInfo() { ItemInfo = item2 });
				}
			}
			return res;
		}
		protected internal virtual List<TileControlDropInfo> GetDropInfoForLargeItem(TileItemViewInfo dragItem) {
			List<TileControlDropInfo> res = new List<TileControlDropInfo>();
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Bounds.Contains(dragItem.DragOrigin))
					continue;
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.ShouldProcessItem) continue;
					if(!itemInfo.DropBounds.Contains(dragItem.DragOrigin) || itemInfo.IsDragging)
						continue;
					TileControlDropInfo dropInfo = new TileControlDropInfo() { ItemInfo = itemInfo };
					res.Add(dropInfo);
					TileItemViewInfo item2 = null, item3 = null, item4 = null;
					if(dropInfo.GetDropSide() == TileControlDropSide.Top) {
						item2 = groupInfo.Items.GetItemFromTop(itemInfo, 0);
						item3 = groupInfo.Items.GetItemFromTop(itemInfo, 1);
					}
					else {
						item2 = groupInfo.Items.GetItemFromBottom(itemInfo, 0);
						item3 = groupInfo.Items.GetItemFromBottom(itemInfo, 1);
					}
					item4 = itemInfo.Position.Column == 0 ? groupInfo.Items.GetItemFromRight(itemInfo) : groupInfo.Items.GetItemFromLeft(itemInfo);
					if(item2 != null && !item2.IsDragging)
						res.Add(new TileControlDropInfo() { ItemInfo = item2 });
					if(item3 != null && !item3.IsDragging)
						res.Add(new TileControlDropInfo() { ItemInfo = item3 });
					if(item4 != null && !item4.IsDragging)
						res.Add(new TileControlDropInfo() { ItemInfo = item4 });
					return res;
				}
			}
			return res;
		}
		protected internal virtual List<TileControlDropInfo> GetDropInfoForSmallItem(TileItemViewInfo dragItem) {
			List<TileControlDropInfo> res = new List<TileControlDropInfo>();
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Bounds.Contains(dragItem.DragOrigin))
					continue;
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.ShouldProcessItem) continue;
					if(!itemInfo.DropBounds.Contains(dragItem.DragOrigin) || itemInfo.IsDragging)
						continue;
					TileControlDropInfo dropInfo = new TileControlDropInfo() { ItemInfo = itemInfo };
					res.Add(dropInfo);
					TileItemViewInfo itemInfo2 = null;
					if(IsHorizontal)
						itemInfo2 = GetSecondSmallTileItemViewInfo(groupInfo, dropInfo, itemInfo, dragItem);
					else
						itemInfo2 = GetSecondSmallTileItemViewInfoVertical(groupInfo, itemInfo, dragItem);
					if(itemInfo2 == null || itemInfo2.IsDragging)
						return res;
					res.Add(new TileControlDropInfo() { ItemInfo = itemInfo2 });
					return res;
				}
			}
			return res;
		}
		private TileItemViewInfo GetSecondSmallTileItemViewInfo(TileGroupViewInfo groupInfo, TileControlDropInfo dropInfo, TileItemViewInfo itemInfo, TileItemViewInfo dragItem) {
			int itemIndex = groupInfo.Items.IndexOf(itemInfo);
			TileItemViewInfo itemInfo2 = null;
			TileItemViewInfo res = null;
			if(IsLargeItem(itemInfo.Item)) {
				itemIndex += dropInfo.GetDropSide() == TileControlDropSide.Top ? -1 : +1;
				itemInfo2 = itemIndex < 0 || itemIndex >= groupInfo.Items.Count ? null : groupInfo.Items[itemIndex];
				if(itemInfo2 != null && IsLargeItem(itemInfo2.Item) && !itemInfo2.IsDragging && itemInfo2.Position.GroupColumn == itemInfo.Position.GroupColumn)
					res = itemInfo2;
				return res;
			}
			if((itemInfo.Position.Column == 0 && dropInfo.GetDropSide() == TileControlDropSide.Left) ||
				(itemInfo.Position.Column == 1 && dropInfo.GetDropSide() == TileControlDropSide.Right))
				return res;
			res = itemInfo.Position.Column == 0 ? groupInfo.Items.GetItemFromRight(itemInfo) : groupInfo.Items.GetItemFromLeft(itemInfo);
			return res;
		}
		private static TileItemViewInfo GetSecondSmallTileItemViewInfoVertical(TileGroupViewInfo groupInfo, TileItemViewInfo itemInfo, TileItemViewInfo dragItem) {
			int itemIndex = groupInfo.Items.IndexOf(itemInfo);
			TileItemViewInfo itemInfo2 = null;
			Point centerOfDropInfo = new Point(itemInfo.DropBounds.X + itemInfo.DropBounds.Width / 2, itemInfo.DropBounds.Y);
			if(dragItem.DragOrigin.X < centerOfDropInfo.X) {
				if(itemIndex > 0)
					itemInfo2 = groupInfo.Items[itemIndex - 1];
			}
			else {
				if(itemIndex < groupInfo.Items.Count - 1)
					itemInfo2 = groupInfo.Items[itemIndex + 1];
			}
			if(itemInfo2 != null && itemInfo.Position.Row != itemInfo2.Position.Row)
				return null;
			return itemInfo2;
		}
		protected internal TileControlDropInfo DropInfo { get; set; }
		protected virtual Rectangle GetBoundsByDropSide(TileControlDropInfo info, TileControlDropSide side) {
			Rectangle rect = info.ItemInfo.DragStateBounds;
			int offset = info.GroupInfo != null ? DragItemGroupBoundsAnimationDelta : DragItemBoundsAnimationDelta;
			switch(side) {
				case TileControlDropSide.Top:
					rect.Offset(0, offset);
					break;
				case TileControlDropSide.Bottom:
					rect.Offset(0, -offset);
					break;
				case TileControlDropSide.Left:
					rect.Offset(offset, 0);
					break;
				case TileControlDropSide.Right:
					rect.Offset(-offset, 0);
					break;
			}
			return rect;
		}
		TileControlDropItemInfo dropTargetInfo;
		public TileControlDropItemInfo DropTargetInfo {
			get { return dropTargetInfo; }
			protected internal set {
				if(DropTargetInfo == value)
					return;
				dropTargetInfo = value;
				OnDropTargetInfoChanged();
			}
		}
		protected virtual void OnDropTargetInfoChanged() {
			ShouldMakeTransition = true;
			CacheItems();
			SaveLayout();
			LayoutGroups(false);
			UpdateGroupsLayoutInDragDrop();
			if(Calculator.IsLayoutChanged(PrevCalculator)) {
				ApplyNewLayout();
				PrepareItemsForTransition();
			}
			else {
				ShouldMakeTransition = false;
			}
		}
		protected virtual void ApplyNewLayout() {
			ClearGroups();
			CreateGroups();
			foreach(TileControlLayoutGroup group in Calculator.Groups) {
				foreach(TileControlLayoutItem item in group.Items) {
					item.ItemInfo.Bounds = item.Bounds;
					item.ItemInfo.Position = item.Position.Clone();
				}
				group.GroupInfo.TextBounds = group.TextBounds;
				group.GroupInfo.Bounds = group.Bounds;
			}
			UpdateGroupsLayoutInDragDrop();
		}
		protected internal virtual TileItemPosition GetPositionByPoint(Point pt) {
			TileItemPosition position = new TileItemPosition() { GroupColumn = -1, Row = -1, Column = -1 };
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Bounds.Contains(pt))
					continue;
				for(int i = 0; ; i++) {
					int x = groupInfo.Bounds.X - IndentBetweenItems / 2 + i * (ItemSize + IndentBetweenItems);
					if(x >= groupInfo.Bounds.Right + IndentBetweenItems / 2)
						break;
					for(int j = 0; ; j++) {
						int y = groupInfo.Bounds.Y - IndentBetweenItems / 2 + j * (ItemSize + IndentBetweenItems);
						if(y >= groupInfo.Bounds.Bottom + IndentBetweenItems / 2)
							break;
						Rectangle rect = new Rectangle(x, y, ItemSize + IndentBetweenItems, ItemSize + IndentBetweenItems);
						if(rect.Contains(pt)) {
							position.GroupInfo = groupInfo;
							position.Row = j;
							position.Column = i % 2;
							position.GroupColumn = i / 2;
							position.SubRow = pt.X > x + SmallItemSize ? 1 : 0;
							position.SubColumn = pt.Y > y + SmallItemSize ? 1 : 0;
							return position;
						}
					}
				}
			}
			return position;
		}
		protected internal virtual TileItemPosition GetPositionByRect(Rectangle bounds) {
			TileItemPosition position = new TileItemPosition() { GroupColumn = -1, Row = -1, Column = -1 };
			float square = 0.0f;
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Bounds.IntersectsWith(bounds))
					continue;
				for(int i = 0; ; i++) {
					int x = groupInfo.Bounds.X + i * ItemSize;
					if(i > 0) x += (i - 1) * IndentBetweenItems;
					if(x >= groupInfo.Bounds.Right)
						break;
					for(int j = 0; ; j++) {
						int y = groupInfo.Bounds.Y + j * ItemSize;
						if(j > 0) y += (j - 1) * IndentBetweenItems;
						if(y >= groupInfo.Bounds.Bottom)
							break;
						Rectangle rect = new Rectangle(x, y, ItemSize, ItemSize);
						if(!rect.IntersectsWith(bounds))
							continue;
						rect.Intersect(bounds);
						float s = rect.Width * rect.Height;
						if(s > square) {
							square = s;
							position.Row = j;
							position.Column = i % 2;
							position.GroupColumn = i / 2;
						}
					}
				}
			}
			return position;
		}
		protected internal virtual TileItemViewInfo GetItemByPosition(TileItemPosition pos) {
			foreach(TileItemViewInfo itemInfo in pos.GroupInfo.Items) {
				if(itemInfo.Position.GroupColumn == pos.GroupColumn && itemInfo.Position.Column == pos.Column && itemInfo.Position.Row == pos.Row)
					return itemInfo;
			}
			return null;
		}
		protected internal virtual TileItemViewInfo GetDropItemByPoint(Point pt) {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.OuterBounds.Contains(pt))
					continue;
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(itemInfo.DropBounds.Contains(pt))
						return itemInfo;
				}
			}
			return null;
		}
		protected internal virtual TileItemViewInfo GetNearestItemByPoint(Point pt) {
			TileItemViewInfo vi = GetDropItemByPoint(pt);
			if(vi != null)
				return vi;
			TileItemPosition pos = GetPositionByPoint(pt);
			if(pos.GroupInfo == null)
				return null;
			foreach(TileItemViewInfo itemInfo in pos.GroupInfo.Items) {
				if(itemInfo.Position.GroupColumn < pos.GroupColumn)
					continue;
				if(itemInfo.Position.GroupColumn > pos.GroupColumn)
					return itemInfo;
				if(itemInfo.Position.Row < pos.Row)
					continue;
				if(itemInfo.Position.Row > pos.Row)
					return itemInfo;
				if(itemInfo.Position.Column < pos.Column)
					continue;
				if(itemInfo.Position.Column > pos.Column)
					return itemInfo;
			}
			return null;
		}
		protected internal virtual void UpdateDropItems(TileItemViewInfo dragItem) {
			DropInfo = GetDropInfo(dragItem);
			if(DropInfo != null && DropInfo.ItemInfo != null && DropInfo.ItemInfo.Item == DragItem.Item)
				return;
			DropTargetInfo = GetDropItemInfo(DropInfo);
		}
		protected virtual void MoveItemByDropSide(TileControlDropInfo dinfo, TileControlDropSide side) {
			if(dinfo.ItemInfo == null)
				return;
			if(dinfo.ItemInfo.IsDragging)
				return;
			dinfo.ItemInfo.UseRenderImage = true;
			TileItemBoundsAnimationInfo info = XtraAnimator.Current.Get(Owner.AnimationHost, dinfo.ItemInfo.Item) as TileItemBoundsAnimationInfo;
			if(info != null && info.EndBounds == GetBoundsByDropSide(dinfo, side))
				return;
			if(!dinfo.ItemInfo.IsVisible) {
				MoveItemByDropSideWithoutAnimation(dinfo, side);
			}
			else {
				XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, dinfo.ItemInfo.Item);
				XtraAnimator.Current.AddAnimation(new TileItemBoundsAnimationInfo(dinfo.ItemInfo, DragMoveItemBoundsAnimationLength, TileItemAnimationBehavior.HoldEnd, TileItemAnimationType.MoveDrag, GetBoundsByDropSide(dinfo, side), false));
			}
		}
		protected virtual void MoveItemByDropSideWithoutAnimation(TileControlDropInfo dinfo, TileControlDropSide side) {
			dinfo.ItemInfo.RenderImageBounds = GetBoundsByDropSide(dinfo, side);
		}
		protected internal bool HasNonDragMoveAnimations {
			get {
				foreach(BaseAnimationInfo info in XtraAnimator.Current.Animations) {
					if(info.AnimatedObject != Owner.AnimationHost)
						continue;
					TileItemBoundsAnimationInfo boundsAnim = info as TileItemBoundsAnimationInfo;
					if(boundsAnim == null || boundsAnim.ItemAnimationType == TileItemAnimationType.MoveDrag)
						continue;
					return true;
				}
				return false;
			}
		}
		protected internal virtual void AnimateAppearance() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.IsVisible)
					continue;
				AddGroupArrivalAnimation(groupInfo);
			}
		}
		protected virtual void AddGroupArrivalAnimation(TileGroupViewInfo groupInfo) {
			foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
				if(!itemInfo.ShouldProcessItem) continue;
				AddItemArrivalAnimation(itemInfo);
			}
		}
		protected virtual void AddItemArrivalAnimation(TileItemViewInfo itemInfo) {
			itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
			itemInfo.RenderImageBounds = GetArrivalBounds(itemInfo);
			int ms = TileITemArrivalDelay * Groups.IndexOf(itemInfo.GroupInfo);
			TileItemArrivalAnimationInfo info = new TileItemArrivalAnimationInfo(itemInfo, TileItemArrivalAnimationLength, TileItemAnimationBehavior.Simple, TileItemAnimationType.Arrival, itemInfo.Bounds, true, ms);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected virtual Rectangle GetArrivalBounds(TileItemViewInfo itemInfo) {
			Rectangle rect = itemInfo.Bounds;
			rect.Offset(TileItemArrivalDeltaX, TileItemArrivalDeltaY);
			rect.Inflate(-TileItemArrivalScaleDelta, -TileItemArrivalScaleDelta);
			return rect;
		}
		protected internal virtual void AddItemToCache(TileItemViewInfo itemInfo) {
			if(ItemViewInfoCache.ContainsKey(itemInfo.Item))
				return;
			ItemViewInfoCache[itemInfo.Item] = itemInfo;
		}
		protected internal virtual TileItemViewInfo GetItemFromCache(TileItem item) {
			if(ItemViewInfoCache.ContainsKey(item))
				return (TileItemViewInfo)ItemViewInfoCache[item];
			TileItemViewInfo itemInfo = CreateItemViewInfo(item);
			ItemViewInfoCache.Add(item, itemInfo);
			return itemInfo;
		}
		protected internal virtual bool ItemCacheContains(TileItem item) {
			return ItemViewInfoCache.ContainsKey(item);
		}
		protected virtual TileItemViewInfo CreateItemViewInfo(TileItem item) {
			return item.CreateViewInfo(); ;
		}
		protected internal virtual void RemoveItemFromCache(TileItem item) {
			if(item != null && ItemViewInfoCache.ContainsKey(item))
				itemViewInfoCache.Remove(item);
		}
		public virtual void CacheItems() {
			foreach(TileGroupViewInfo groupInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
					if(!itemInfo.ShouldProcessItem) continue;
					itemInfo.SavePrevState();
					AddItemToCache(itemInfo);
				}
			}
		}
		public virtual void MakeVisible(TileItemViewInfo item) {
			bool alignRightEdge = false;
			if(IsHorizontal) {
				alignRightEdge = item.Bounds.Right > ContentBounds.Right;
			}
			else {
				alignRightEdge = item.Bounds.Bottom > ContentBounds.Bottom;
			}
			MakeVisible(item, alignRightEdge);
		}
		bool IsContentBoundsContains(TileItemViewInfo item) {
			return IsBoundsContainsCore(GroupsContentBounds, item);
		}
		protected bool IsBoundsContainsCore(Rectangle bounds, TileItemViewInfo item) {
			int left = item.Bounds.Left;
			int right = item.Bounds.Right;
			int top = item.Bounds.Top;
			int bottom = item.Bounds.Bottom;
			if(bounds.Contains(item.Bounds))
				return true;
			if(IsHorizontal) {
				if((bounds.Contains(new Rectangle(left, top, right - left, 1))) ||
					(bounds.Contains(new Rectangle(left, bottom, right - left, 1))))
					return true;
			}
			else {
				if((bounds.Contains(new Rectangle(left, top, 1, bottom - top))) ||
					(bounds.Contains(new Rectangle(right, top, 1, bottom - top))))
					return true;
			}
			return false;
		}
		protected internal virtual void MakeVisible(TileItemViewInfo item, bool alignRightEdge) {
			if(IsContentBoundsContains(item))
				return;
			int delta = 0;
			if(alignRightEdge) {
				delta = IsHorizontal ? item.Bounds.Right - GroupsContentBounds.Right : item.Bounds.Bottom - GroupsContentBounds.Bottom;
			}
			else {
				delta = IsHorizontal ? item.Bounds.X - GroupsContentBounds.X : item.Bounds.Y - GroupsContentBounds.Y;
			}
			delta = ShouldReverseScrollDueRTL ? -delta : delta;
			XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, this);
			XtraAnimator.Current.AddAnimation(new TileOffsetAnimationInfo(this, OffsetAnimationLength, Offset, Offset + delta));
		}
		protected internal virtual void AddChangeItemTextAnimation(TileItemViewInfo itemInfo, TileItemFrame frame) {
			bool releaseGraphics = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				releaseGraphics = true;
			}
			try {
				itemInfo.LayoutItem(itemInfo.Bounds);
				XtraAnimator.Current.AddAnimation(CreateItemContentAnimation(itemInfo, frame));
			}
			finally {
				if(releaseGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual void PrepareItemForDrawingInImage(TileItemViewInfo itemInfo, TileItemFrame frame) {
			TileItemContentAnimationType animType = GetItemContentAnimationType(itemInfo, frame);
			if(animType == TileItemContentAnimationType.Fade ||
				animType == TileItemContentAnimationType.SegmentedFade ||
				animType == TileItemContentAnimationType.RandomSegmentedFade) {
				itemInfo.DrawBackgroundImage = true;
				itemInfo.DrawImage = true;
				itemInfo.DrawText = true;
				itemInfo.AllowItemCheck = false;
			}
			else {
				itemInfo.DrawBackgroundImage = true;
				itemInfo.DrawImage = false;
				itemInfo.DrawText = false;
				itemInfo.AllowItemCheck = true;
			}
		}
		protected internal virtual void AddChangeItemContentAnimation(TileItemViewInfo itemInfo, TileItemFrame frame) {
			bool releaseGraphics = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				releaseGraphics = true;
			}
			try {
				itemInfo.ClearAppearance();
				itemInfo.LayoutItem(itemInfo.Bounds);
				if(Owner.Handler.State == TileControlHandlerState.DragMode)
					return;
				PrepareItemForDrawingInImage(itemInfo, frame);
				itemInfo.RenderImage = RenderItemToBitmap(itemInfo);
				itemInfo.DrawImage = true;
				itemInfo.DrawText = true;
				itemInfo.DrawBackgroundImage = true;
				if(itemInfo.IsPressed) {
					Owner.Invalidate(itemInfo.Bounds);
					return;
				}
				XtraAnimator.Current.Animations.Remove(Owner.AnimationHost, itemInfo.Item);
				itemInfo.IsChangingContent = true;
				XtraAnimator.Current.AddAnimation(CreateItemContentAnimation(itemInfo, frame));
			}
			finally {
				if(releaseGraphics)
					GInfo.ReleaseGraphics();
			}
		}
		protected TileItemContentAnimationInfoBase CreateItemContentAnimation(TileItemViewInfo itemInfo, TileItemFrame frame) {
			TileItemContentAnimationType animType = GetItemContentAnimationType(itemInfo, frame);
			itemInfo.CurrentContentAnimationType = animType;
			switch(animType) {
				case TileItemContentAnimationType.Default:
				case TileItemContentAnimationType.ScrollTop:
				case TileItemContentAnimationType.ScrollDown:
				case TileItemContentAnimationType.ScrollLeft:
				case TileItemContentAnimationType.ScrollRight:
					return new TileItemScrollContentAnimationInfo(itemInfo, animType, ItemContentAnimationLength, frame.AnimateText, frame.AnimateBackgroundImage, frame.AnimateImage);
				case TileItemContentAnimationType.Fade:
				case TileItemContentAnimationType.SegmentedFade:
				case TileItemContentAnimationType.RandomSegmentedFade:
					return new TileItemFadeContentAnimationInfo(itemInfo, animType, ItemContentAnimationLength, frame.AnimateText, frame.AnimateBackgroundImage, frame.AnimateImage);
			}
			return null;
		}
		protected TileItemContentAnimationType GetItemContentAnimationType(TileItemViewInfo itemInfo, TileItemFrame frame) {
			if(frame.Animation != TileItemContentAnimationType.Default)
				return frame.Animation;
			if(itemInfo.Item.ContentAnimation != TileItemContentAnimationType.Default)
				return itemInfo.Item.ContentAnimation;
			return Owner.Properties.ItemContentAnimation;
		}
		protected internal virtual void SuspendContentAnimation() {
			foreach(TileGroup group in Owner.Groups) {
				foreach(TileItem item in group.Items) {
					if(item.CurrentFrame != null) {
						if(item.ItemInfo != null)
							item.ItemInfo.UseRenderImage = false;
						item.Timer.Stop();
					}
				}
			}
		}
		protected internal virtual void ResumeContentAnimation() {
			if(Owner.IsAnimationSuspended)
				return;
			foreach(TileGroup group in Owner.Groups) {
				foreach(TileItem item in group.Items) {
					if(item.CurrentFrame != null) {
						item.TimerStartCore();
					}
				}
			}
		}
		public virtual AppearanceObject GetAppearance(TileItemViewInfo item) {
			if(item.IsPressed)
				return GetPressedAppearance();
			if(item.IsHovered)
				return GetHoveredAppearance(item);
			if(item.IsSelected)
				return GetSelectedAppearance();
			return Owner.AppearanceItem.Normal;
		}
		protected virtual AppearanceObject GetPressedAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { Owner.AppearanceItem.Pressed, Owner.AppearanceItem.Normal }, DefaultAppearance);
			return res;
		}
		protected virtual AppearanceObject GetHoveredAppearance(TileItemViewInfo item) {
			AppearanceObject controlSelected = new AppearanceObject();
			AppearanceObject res = new AppearanceObject();
			if(item != null && item.IsSelected) {
				controlSelected = Owner.AppearanceItem.Selected;
			}
			AppearanceHelper.Combine(res, new AppearanceObject[] { Owner.AppearanceItem.Hovered, controlSelected, Owner.AppearanceItem.Normal });
			return res;
		}
		protected virtual AppearanceObject GetSelectedAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { Owner.AppearanceItem.Selected, Owner.AppearanceItem.Normal });
			return res;
		}
		internal static int TouchCheckReturnAnimationLength = 100;
		protected internal virtual void AddCheckItemByTouchAnimation(TileItemViewInfo itemInfo) {
			AddItemBoundsAnimation(itemInfo, itemInfo.Bounds, TouchCheckReturnAnimationLength);
		}
		public virtual void CheckParentColors() {
			AppearanceDefault res = LookAndFeelHelper.CheckEmptyBackColor(Owner.LookAndFeel.ActiveLookAndFeel, DefaultAppearance, Owner.Control);
			PaintAppearance.BackColor = res.BackColor;
		}
		protected internal void ClearDropInfo() {
			this.dropTargetInfo = null;
			DropInfo = null;
		}
		protected internal virtual void ResetBackgroundImage() {
			if(backgroundImageStretchedCore != null) backgroundImageStretchedCore.Dispose();
			backgroundImageStretchedCore = null;
		}
		public virtual bool AllowDragCore { get { return Owner.IsDesignMode ? true : Owner.Properties.AllowDrag; } }
		protected internal virtual int GetItemHeight() { return ItemSize; }
		public void StopItemContentAnimations() {
			foreach(TileGroupViewInfo gInfo in Groups) {
				foreach(TileItemViewInfo itemInfo in gInfo.Items) {
					if(itemInfo == null || itemInfo.Item == null) continue;
					itemInfo.Item.FreeContentAnimationTimer();
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseAdvancedTextRendering { get; set; }
		#region ISupportAdornerUIManager Members
		UpdateActionEventHandler changed;
		event UpdateActionEventHandler ISupportAdornerUIManager.Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
		IntPtr ISupportAdornerUIManager.Handle {
			get { return Owner.Handle; }
		}
		bool ISupportAdornerUIManager.IsHandleCreated {
			get { return Owner.IsHandleCreated; }
		}
		void ISupportAdornerUIManager.UpdateVisualEffects(UpdateAction action) {
			UpdateVisualEffects(action);
		}
		protected internal void UpdateVisualEffects(UpdateAction action) {
			if(changed == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			changed(Owner, e);
		}
		Rectangle ISupportAdornerUIManager.ClientRectangle {
			get { return GetVisualEffectsClientBounds(); }
		}
		protected virtual Rectangle GetVisualEffectsClientBounds() { return ClientBounds; }
		#endregion
	}
	public enum TileControlHitTest { Control, Group, GroupCaption, Item, BackArrow, ForwardArrow }
	public class TileControlHitInfo {
		static TileControlHitInfo empty = new TileControlHitInfo();
		public static TileControlHitInfo Empty { get { return empty; } }
		public bool CheckDropBounds { get; set; }
		public Point HitPoint { get; internal set; }
		public TileControlHitTest HitTest { get; internal set; }
		public bool ContainsSet(Rectangle rect, TileControlHitTest hitTest) {
			if(rect.Contains(HitPoint)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public TileControlViewInfo ViewInfo { get; internal set; }
		public TileGroupViewInfo GroupInfo { get; internal set; }
		public TileItemViewInfo ItemInfo { get; internal set; }
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			TileControlHitInfo hitInfo = obj as TileControlHitInfo;
			if(hitInfo == null || hitInfo.HitTest != HitTest)
				return false;
			if(hitInfo.HitTest == TileControlHitTest.Control)
				return hitInfo.ViewInfo == ViewInfo;
			if(hitInfo.HitTest == TileControlHitTest.Group || hitInfo.HitTest == TileControlHitTest.GroupCaption)
				return hitInfo.GroupInfo == GroupInfo;
			return hitInfo.ItemInfo == ItemInfo;
		}
		public bool InItem { get { return HitTest == TileControlHitTest.Item && ItemInfo != null; } }
		public bool InGroup { get { return HitTest == TileControlHitTest.Group && GroupInfo != null; } }
		public bool InBackArrow { get { return HitTest == TileControlHitTest.BackArrow; } }
		public bool InForwardArrow { get { return HitTest == TileControlHitTest.ForwardArrow; } }
	}
	public enum TileControlDropSide { Top = 0, Left = 1, Before = 1, Right = 2, After = 2, Bottom = 3, None = 4 }
	public class TileControlDropInfo {
		public TileItemViewInfo DragItem { get; internal set; }
		public TileControlDropSide GroupDropSide { get; internal set; }
		public TileGroupViewInfo NearestGroupInfo { get; internal set; }
		public TileItemViewInfo NearestItemInfo { get; internal set; }
		public TileItemViewInfo ItemInfo { get; internal set; }
		public TileGroupViewInfo GroupInfo { get; internal set; }
		public TileItemPosition Position { get; internal set; }
		public bool InsertIntoGroup { get; internal set; }
		bool IsLeftItem() {
			if(DragItem.IsSmall && ItemInfo.IsSmall)
				return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn &&
					DragItem.Position.Row == ItemInfo.Position.Row &&
					DragItem.Position.Column == ItemInfo.Position.Column &&
					DragItem.Position.SubRow == ItemInfo.Position.SubRow &&
					DragItem.Position.SubColumn == (ItemInfo.Position.SubColumn - 1);
			return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn && DragItem.Position.Row == ItemInfo.Position.Row && DragItem.Position.Column < ItemInfo.Position.Column;
		}
		bool IsRightItem() {
			if(DragItem.IsSmall && ItemInfo.IsSmall)
				return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn &&
					DragItem.Position.Row == ItemInfo.Position.Row &&
					DragItem.Position.Column == ItemInfo.Position.Column &&
					DragItem.Position.SubRow == ItemInfo.Position.SubRow &&
					DragItem.Position.SubColumn == (ItemInfo.Position.SubColumn + 1);
			return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn && DragItem.Position.Row == ItemInfo.Position.Row && DragItem.Position.Column > ItemInfo.Position.Column;
		}
		bool IsBottomItem() {
			if(DragItem.IsSmall && ItemInfo.IsSmall)
				return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn &&
					DragItem.Position.Row == ItemInfo.Position.Row &&
					DragItem.Position.Column == ItemInfo.Position.Column &&
					DragItem.Position.SubRow == (ItemInfo.Position.SubRow + 1) &&
					DragItem.Position.SubColumn == ItemInfo.Position.SubColumn;
			return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn && DragItem.Position.Row == ItemInfo.Position.Row + 1;
		}
		bool IsTopItem() {
			if(DragItem.IsSmall && ItemInfo.IsSmall)
				return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn &&
					DragItem.Position.Row == ItemInfo.Position.Row &&
					DragItem.Position.Column == ItemInfo.Position.Column &&
					DragItem.Position.SubRow == (ItemInfo.Position.SubRow - 1) &&
					DragItem.Position.SubColumn == ItemInfo.Position.SubColumn;
			return DragItem.Position.GroupColumn == ItemInfo.Position.GroupColumn && DragItem.Position.Row == ItemInfo.Position.Row - 1;
		}
		public TileControlDropSide GetDropSide() {
			if(ItemInfo == null)
				return TileControlDropSide.None;
			if(IsLeftItem())
				return TileControlDropSide.Right;
			if(IsRightItem())
				return TileControlDropSide.Left;
			if(IsTopItem())
				return TileControlDropSide.Bottom;
			if(IsBottomItem())
				return TileControlDropSide.Top;
			return TileControlDropSide.Left;
		}
		public virtual void CalcDropGroupSide(Point point, bool allGroupsAreEmpty, Orientation orientation) {
			if(allGroupsAreEmpty) {
				GroupDropSide = TileControlDropSide.Right;
				return;
			}
			if(NearestGroupInfo == null)
				return;
			if(orientation == Orientation.Horizontal) {
				GroupDropSide = point.X < NearestGroupInfo.Bounds.X + NearestGroupInfo.Bounds.Width / 2 ? TileControlDropSide.Left : TileControlDropSide.Right;
				return;
			}
			GroupDropSide = point.Y < NearestGroupInfo.Bounds.Y + NearestGroupInfo.Bounds.Height / 2 ? TileControlDropSide.Top : TileControlDropSide.Bottom;
		}
	}
	public enum TileItemAnimationBehavior { Simple, HoldEnd }
	public enum TileItemAnimationType { EnterDrag, ExitDrag, Press, Unpress, MoveDrag, Arrival, Content, Hover }
	public class TileItemBaseAnimationInfo : BaseAnimationInfo {
		public TileItemBaseAnimationInfo(TileItemViewInfo item, object animationId, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, bool invalidateEntireArea)
			: base(item.GroupInfo.ControlInfo.Owner.AnimationHost, animationId, (int)(TimeSpan.TicksPerMillisecond * milliseconds / 100), 100) {
			Item = item;
			Item.UseRenderImage = true;
			AnimationBehavior = animBehavior;
			InvalidateEntireArea = invalidateEntireArea;
			ItemAnimationType = animType;
		}
		public TileItemBaseAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, bool invalidateEntireArea)
			: this(item, item.Item, milliseconds, animBehavior, animType, invalidateEntireArea) {
		}
		public TileItemAnimationBehavior AnimationBehavior { get; private set; }
		public TileItemAnimationType ItemAnimationType { get; private set; }
		public TileItemViewInfo Item { get; private set; }
		public override void FrameStep() {
			FrameStepCore();
			Invalidate();
			if(IsFinalFrame)
				OnAnimationComplete();
		}
		public bool InvalidateEntireArea { get; private set; }
		protected virtual void FrameStepCore() {
		}
		protected virtual void Invalidate() {
			if(InvalidateEntireArea)
				Item.GroupInfo.ControlInfo.Owner.Invalidate(Item.GroupInfo.ControlInfo.Owner.ClientRectangle);
			else
				Item.GroupInfo.ControlInfo.Owner.Invalidate(Item.OuterBounds);
		}
		protected virtual void OnAnimationComplete() {
			Item.ForceUseBoundsAnimation = false;
			if(AnimationBehavior == TileItemAnimationBehavior.Simple) {
				Item.UseRenderImage = false;
			}
			if(Item.GroupInfo.ControlInfo.ShouldResumeAnimationsAfterExitDragMode &&
				!Item.GroupInfo.ControlInfo.HasAnimations(TileItemAnimationType.ExitDrag) &&
				Item.GroupInfo.ControlInfo.Owner.Handler.State != TileControlHandlerState.DragMode) {
				Item.GroupInfo.ControlInfo.ResumeContentAnimation();
				Item.GroupInfo.ControlInfo.ShouldResumeAnimationsAfterExitDragMode = false;
			}
		}
	}
	public class TileItemBoundsAnimationInfo : TileItemBaseAnimationInfo {
		public TileItemBoundsAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, Rectangle endBounds, bool invalidateEntireArea)
			: this(item, milliseconds, animBehavior, animType, 0, false, invalidateEntireArea) {
			EndBounds = endBounds;
			StartBounds = Item.RenderImageBounds;
		}
		public TileItemBoundsAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, int delta, bool isGrow) : this(item, milliseconds, animBehavior, animType, delta, isGrow, false) { }
		public TileItemBoundsAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, int delta, bool isGrow, bool invalidateEntireArea)
			: base(item, milliseconds, animBehavior, animType, invalidateEntireArea) {
			IsGrow = isGrow;
			MaxDelta = delta;
			Rectangle rect = Item.Bounds;
			int dt = IsGrow ? MaxDelta : -MaxDelta;
			rect.Inflate(dt, dt);
			EndBounds = rect;
			StartBounds = Item.Bounds;
			StartContentOffset = -1;
		}
		protected Rectangle PrevFrameBounds { get; set; }
		public int StartContentOffset { get; set; }
		public bool IsGrow { get; private set; }
		public int MaxDelta { get; set; }
		public Rectangle EndBounds { get; private set; }
		public Rectangle StartBounds { get; private set; }
		Rectangle GetInvalidBounds(bool respectOffset) {
			Rectangle rect = new Rectangle(Math.Min(StartBounds.X, EndBounds.X), Math.Min(StartBounds.Y, EndBounds.Y), 0, 0);
			rect.Width = Math.Max(StartBounds.Right, EndBounds.Right) - rect.X;
			rect.Height = Math.Max(StartBounds.Bottom, EndBounds.Bottom) - rect.Y;
			if(respectOffset) rect = PatchLocationByContentOffset(rect);
			return rect;
		}
		Rectangle PatchLocationByContentOffset(Rectangle rect) {
			if(StartContentOffset == -1) return rect;
			var controlInfo = Item.ControlInfo;
			if(controlInfo == null) return rect;
			int delta = StartContentOffset - controlInfo.Offset;
			if(controlInfo.Owner.Properties.Orientation == Orientation.Horizontal) {
				rect.X += delta;
			}
			else {
				rect.Y += delta;
			}
			return rect;
		}
		protected override void Invalidate() {
			if(InvalidateEntireArea) {
				Item.GroupInfo.ControlInfo.Owner.Invalidate(Item.GroupInfo.ControlInfo.Owner.ClientRectangle);
			}
			else {
				Item.GroupInfo.ControlInfo.Owner.Invalidate(PrevFrameBounds);
				Item.GroupInfo.ControlInfo.Owner.Invalidate(GetInvalidBounds(true));
				PrevFrameBounds = GetInvalidBounds(false);
			}
		}
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) {
				Item.UseOptimizedRenderImage = AnimationBehavior == TileItemAnimationBehavior.HoldEnd;
				k = 1.0f;
			}
			Rectangle rect = Rectangle.Empty;
			rect.X = StartBounds.X + (int)(k * (EndBounds.X - StartBounds.X));
			rect.Y = StartBounds.Y + (int)(k * (EndBounds.Y - StartBounds.Y));
			rect.Width = StartBounds.Width + (int)(k * (EndBounds.Width - StartBounds.Width));
			rect.Height = StartBounds.Height + (int)(k * (EndBounds.Height - StartBounds.Height));
			Item.RenderImageBounds = rect;
		}
	}
	public class TileItemBoundsSplineAnimationInfo : TileItemBoundsAnimationInfo {
		public TileItemBoundsSplineAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, Rectangle endBounds, bool invalidateEntireArea)
			: base(item, milliseconds, animBehavior, animType, endBounds, invalidateEntireArea) {
			SplineHelperX = new SplineAnimationHelper();
			SplineHelperY = new SplineAnimationHelper();
			SplineHelperWidth = new SplineAnimationHelper();
			SplineHelperHeight = new SplineAnimationHelper();
			SplineHelperX.Init(StartBounds.X, EndBounds.X, 1.0f);
			SplineHelperY.Init(StartBounds.Y, EndBounds.Y, 1.0f);
			SplineHelperWidth.Init(StartBounds.Width, EndBounds.Width, 1.0f);
			SplineHelperHeight.Init(StartBounds.Height, EndBounds.Height, 1.0f);
		}
		protected SplineAnimationHelper SplineHelperX { get; private set; }
		protected SplineAnimationHelper SplineHelperY { get; private set; }
		protected SplineAnimationHelper SplineHelperWidth { get; private set; }
		protected SplineAnimationHelper SplineHelperHeight { get; private set; }
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0f;
			Rectangle rect = new Rectangle();
			rect.X = (int)SplineHelperX.CalcSpline(k);
			rect.Y = (int)SplineHelperY.CalcSpline(k);
			rect.Width = (int)SplineHelperWidth.CalcSpline(k);
			rect.Height = (int)SplineHelperHeight.CalcSpline(k);
			Item.RenderImageBounds = rect;
		}
	}
	public class TileItemArrivalAnimationInfo : TileItemBoundsSplineAnimationInfo {
		public TileItemArrivalAnimationInfo(TileItemViewInfo item, int milliseconds, TileItemAnimationBehavior animBehavior, TileItemAnimationType animType, Rectangle endBounds, bool invalidateEntireArea, int delayMilliseconds)
			: base(item, milliseconds + delayMilliseconds, animBehavior, animType, endBounds, invalidateEntireArea) {
			DelayMilliseconds = delayMilliseconds;
			Item.RenderImageOpacity = 0.0f;
			SplineOpacity = new SplineAnimationHelper();
			SplineOpacity.Init(0, 1, 1.0f);
		}
		public int DelayMilliseconds { get; private set; }
		protected SplineAnimationHelper SplineOpacity { get; private set; }
		protected override void FrameStepCore() {
			if((CurrentTick - BeginTick) < DelayMilliseconds * TimeSpan.TicksPerMillisecond) {
				Item.RenderImageOpacity = 0.0f;
				return;
			}
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0f;
			Item.RenderImageOpacity = (float)SplineOpacity.CalcSpline(k);
			Rectangle rect = new Rectangle();
			rect.X = (int)SplineHelperX.CalcSpline(k);
			rect.Y = (int)SplineHelperY.CalcSpline(Math.Min(1.0f, k * 1.3f));
			rect.Width = (int)SplineHelperWidth.CalcSpline(k);
			rect.Height = (int)SplineHelperHeight.CalcSpline(k);
			Item.RenderImageBounds = rect;
		}
		protected override void OnAnimationComplete() {
			Item.RenderImageOpacity = 1.0f;
			base.OnAnimationComplete();
		}
	}
	public class TileItemTransitionAnimationInfo : TileItemBoundsAnimationInfo {
		public TileItemTransitionAnimationInfo(TileItemViewInfo itemInfo, int milliseconds, TileItemAnimationBehavior behavior, TileItemAnimationType animType, Rectangle endBounds)
			: base(itemInfo, milliseconds, behavior, animType, endBounds, true) {
			SplineHelper = new SplineAnimationHelper();
			SplineHelper.Init(0.0f, 1.0f, 1.0f);
		}
		protected bool IsLeftToRight {
			get {
				if(Item.ControlInfo.IsVertical) {
					if(Item.PrevPosition.Row >= Item.Position.Row)
						return false;
					if(Item.PrevPosition.GroupColumn != (Item.ControlInfo.RealColumnCount - 1))
						return false;
					return Item.PrevPosition.GroupColumn == (Item.ControlInfo.RealColumnCount - 1) && Item.Position.GroupColumn == 0;
				}
				return Item.PrevPosition.GroupColumn == Item.Position.GroupColumn && Item.PrevPosition.Row < Item.Position.Row;
			}
		}
		protected bool IsRightToLeft {
			get {
				if(Item.ControlInfo.IsVertical) {
					if(Item.PrevPosition.Row > Item.Position.Row)
						return false;
					if(Item.PrevPosition.GroupColumn != 0)
						return false;
					return Item.Position.GroupColumn == (Item.ControlInfo.RealColumnCount - 1);
				}
				return Item.PrevPosition.GroupColumn == Item.Position.GroupColumn && Item.PrevPosition.Row > Item.Position.Row;
			}
		}
		protected bool IsTopToBottom {
			get {
				if(Item.ControlInfo.IsVertical) {
					if(Item.PrevPosition.Row < Item.Position.Row)
						return false;
					if(Item.PrevPosition.GroupColumn != (Item.ControlInfo.RealColumnCount - 1))
						return true;
					return Item.PrevPosition.GroupColumn == (Item.ControlInfo.RealColumnCount - 1) && Item.Position.GroupColumn != 0;
				}
				return Item.PrevPosition.GroupColumn < Item.Position.GroupColumn;
			}
		}
		protected bool IsBottomToTop {
			get {
				if(Item.ControlInfo.IsVertical) {
					if(Item.PrevPosition.Row > Item.Position.Row)
						return false;
					if(Item.PrevPosition.GroupColumn != 0)
						return true;
					return Item.PrevPosition.GroupColumn == 0 && Item.Position.GroupColumn != (Item.ControlInfo.RealColumnCount - 1);
				}
				return Item.PrevPosition.GroupColumn > Item.Position.GroupColumn;
			}
		}
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			Item.IsInTransition = true;
			if(IsFinalFrame) {
				k = 1.0f;
				Item.IsInTransition = false;
				if(AnimationBehavior == TileItemAnimationBehavior.Simple)
					Item.UseRenderImage = false;
				else
					Item.RenderImageBounds = EndBounds;
			}
			float pos = (float)SplineHelper.CalcSpline(k);
			if(IsLeftToRight) {
				CalcItemBoundsLeftToRight(pos);
			}
			else if(IsRightToLeft) {
				CalcItemBoundsRightToLeft(pos);
			}
			else if(IsTopToBottom) {
				CalcItemBoundsTopToBottom(pos);
			}
			else if(IsBottomToTop) {
				CalcItemBoundsBottomToTop(pos);
			}
		}
		public Rectangle PrevBounds { get { return Item.UseRenderImage ? Item.PrevRenderImageBounds : Item.PrevBounds; } }
		public Rectangle Bounds { get { return EndBounds; } }
		private void CalcItemBoundsBottomToTop(float pos) {
			Rectangle rect = PrevBounds;
			rect.Y -= (int)(PrevBounds.Height * pos);
			Item.PrevTransitionBounds = rect;
			rect = Bounds;
			rect.Y += (int)(Bounds.Height * (1.0f - pos));
			Item.NextTransitionBounds = rect;
		}
		private void CalcItemBoundsTopToBottom(float pos) {
			Rectangle rect = PrevBounds;
			rect.Y += (int)(PrevBounds.Height * pos);
			Item.PrevTransitionBounds = rect;
			rect = Bounds;
			rect.Y -= (int)(Bounds.Height * (1.0f - pos));
			Item.NextTransitionBounds = rect;
		}
		private void CalcItemBoundsRightToLeft(float pos) {
			Rectangle rect = PrevBounds;
			rect.X -= (int)(PrevBounds.Width * pos);
			Item.PrevTransitionBounds = rect;
			rect = Bounds;
			rect.X += (int)(Bounds.Width * (1.0f - pos));
			Item.NextTransitionBounds = rect;
		}
		private void CalcItemBoundsLeftToRight(float pos) {
			Rectangle rect = PrevBounds;
			rect.X += (int)(PrevBounds.Width * pos);
			Item.PrevTransitionBounds = rect;
			rect = Bounds;
			rect.X -= (int)(Bounds.Width * (1.0f - pos));
			Item.NextTransitionBounds = rect;
		}
		protected SplineAnimationHelper SplineHelper { get; private set; }
	}
	public class TileOffsetAnimationInfo : BaseAnimationInfo {
		public TileOffsetAnimationInfo(TileControlViewInfo viewInfo, int milliseconds, int start, int end)
			: base(viewInfo.Owner.AnimationHost, viewInfo, (int)TimeSpan.TicksPerMillisecond * milliseconds / 100, 100) {
			Start = start;
			End = end;
			ViewInfo = viewInfo;
			SplineHelper = new SplineAnimationHelper();
			SplineHelper.Init(Start, End, 1.0f);
		}
		public TileControlViewInfo ViewInfo { get; private set; }
		public int Start { get; private set; }
		public int End { get; private set; }
		protected SplineAnimationHelper SplineHelper { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame)
				k = 1.0f;
			ViewInfo.Owner.Position = (int)SplineHelper.CalcSpline(k);
		}
	}
	public class TileHoverOpacityAnimationInfo : TileItemBaseAnimationInfo {
		public TileHoverOpacityAnimationInfo(TileItemViewInfo itemInfo, int milliseconds, bool isHovered)
			: base(itemInfo, itemInfo.Item.HoverAnimationId, milliseconds, TileItemAnimationBehavior.Simple, TileItemAnimationType.Hover, false) {
			Item.UseRenderImage = false;
			IsHovered = isHovered;
			SplineHelper = new SplineAnimationHelper();
			if(IsHovered)
				SplineHelper.Init(0.0f, 1.0f, 1.0f);
			else
				SplineHelper.Init(1.0f, 0.0f, 1.0f);
		}
		public bool IsHovered { get; private set; }
		protected SplineAnimationHelper SplineHelper { get; private set; }
		protected override void FrameStepCore() {
			base.FrameStepCore();
			if(IsFinalFrame) {
				Item.IsHoveringAnimation = false;
				Item.ForceUpdateAppearanceColors();
				Item.HoverOpacity = IsHovered ? 1.0f : 0.0f;
				return;
			}
			else {
				Item.IsHoveringAnimation = true;
			}
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame)
				k = 1.0f;
			Item.HoverOpacity = (float)Math.Max(0.0, Math.Min(1.0, SplineHelper.CalcSpline(k)));
		}
	}
	public class TileItemContentAnimationInfoBase : TileItemBaseAnimationInfo {
		public TileItemContentAnimationInfoBase(TileItemViewInfo itemInfo, TileItemContentAnimationType contentAnimationType, int milliseconds, bool animateText, bool animateBackgroundImage, bool animateImage)
			: base(itemInfo, milliseconds, TileItemAnimationBehavior.Simple, TileItemAnimationType.Content, false) {
			SplineHelper = new SplineAnimationHelper();
			SplineHelper.Init(0, 1.0f, 1.0f);
			Item.AnimateText = animateText;
			Item.AnimateBackgroundImage = animateBackgroundImage;
			Item.AnimateImage = animateImage;
			ContentAnimationType = contentAnimationType;
		}
		protected internal bool ShouldStartHoverAnimation { get; set; }
		protected internal bool DelayedIsHovered { get; set; }
		protected SplineAnimationHelper SplineHelper { get; private set; }
		public TileItemContentAnimationType ContentAnimationType { get; private set; }
		protected override void OnAnimationComplete() {
			base.OnAnimationComplete();
			Item.PrevRenderImage = null;
			if(ShouldStartHoverAnimation) {
				Item.GroupInfo.ControlInfo.AddItemHoverAnimation(Item, DelayedIsHovered, true);
			}
		}
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) {
				Item.IsChangingContent = false;
				Item.AnimateText = false;
				Item.AnimateImage = false;
				Item.AnimateBackgroundImage = false;
				k = 1.0f;
			}
			FrameStepCore(k);
		}
		protected virtual void FrameStepCore(float k) {
		}
	}
	public class TileItemScrollContentAnimationInfo : TileItemContentAnimationInfoBase {
		public TileItemScrollContentAnimationInfo(TileItemViewInfo itemInfo, TileItemContentAnimationType contentAnimationType, int milliseconds, bool animateText, bool animateBackgroundImage, bool animateImage)
			: base(itemInfo, contentAnimationType, milliseconds, animateText, animateBackgroundImage, animateImage) {
		}
		protected override void FrameStepCore(float k) {
			Rectangle rect = Item.Bounds;
			switch(ContentAnimationType) {
				case TileItemContentAnimationType.Default:
				case TileItemContentAnimationType.ScrollTop:
					rect.Y += (int)(Item.Bounds.Height * (1.0f - (SplineHelper.CalcSpline(k))));
					Item.RenderImageBounds = rect;
					rect.Y -= Item.Bounds.Height;
					Item.PrevRenderImageBounds = rect;
					break;
				case TileItemContentAnimationType.ScrollDown:
					rect.Y -= (int)(Item.Bounds.Height * (1.0f - (SplineHelper.CalcSpline(k))));
					Item.RenderImageBounds = rect;
					rect.Y += Item.Bounds.Height;
					Item.PrevRenderImageBounds = rect;
					break;
				case TileItemContentAnimationType.ScrollRight:
					rect.X -= (int)(Item.Bounds.Width * (1.0f - (SplineHelper.CalcSpline(k))));
					Item.RenderImageBounds = rect;
					rect.X += Item.Bounds.Width;
					Item.PrevRenderImageBounds = rect;
					break;
				case TileItemContentAnimationType.ScrollLeft:
					rect.X += (int)(Item.Bounds.Width * (1.0f - (SplineHelper.CalcSpline(k))));
					Item.RenderImageBounds = rect;
					rect.X -= Item.Bounds.Width;
					Item.PrevRenderImageBounds = rect;
					break;
			}
		}
	}
	public class TileItemFadeContentAnimationInfo : TileItemContentAnimationInfoBase {
		public TileItemFadeContentAnimationInfo(TileItemViewInfo itemInfo, TileItemContentAnimationType contentAnimationType, int milliseconds, bool animateText, bool animateBackgroundImage, bool animateImage)
			: base(itemInfo, contentAnimationType, milliseconds, animateText, animateBackgroundImage, animateImage) {
			if(Item.CurrentContentAnimationType == TileItemContentAnimationType.RandomSegmentedFade) {
				Item.GenerateRandomSegments();
			}
		}
		protected override void FrameStepCore(float k) {
			base.FrameStepCore(k);
			Item.PrevRenderImageBounds = Item.RenderImageBounds = Item.Bounds;
			Item.RenderImageOpacity = k;
		}
	}
	internal static class TileItemImageScaleModeHelper {
		public static Size ScaleImage(Size size, Size imageSize, TileItemImageScaleMode scaleMode) {
			switch(scaleMode) {
				case TileItemImageScaleMode.Default:
				case TileItemImageScaleMode.NoScale:
					return imageSize;
				case TileItemImageScaleMode.Stretch:
					return size;
				case TileItemImageScaleMode.StretchHorizontal:
					return new Size(size.Width, Math.Min(size.Height, imageSize.Height));
				case TileItemImageScaleMode.StretchVertical:
					return new Size(Math.Min(imageSize.Width, size.Width), size.Height);
				case TileItemImageScaleMode.ZoomInside:
				case TileItemImageScaleMode.ZoomOutside:
				case TileItemImageScaleMode.Squeeze:
					if(imageSize.Width < size.Width && imageSize.Height < size.Height)
						if(scaleMode == TileItemImageScaleMode.Squeeze)
							return imageSize;
					float scaleX = ((float)size.Width) / imageSize.Width;
					float scaleY = ((float)size.Height) / imageSize.Height;
					float currScale;
					if(scaleMode == TileItemImageScaleMode.ZoomInside || scaleMode == TileItemImageScaleMode.Squeeze)
						currScale = scaleX > scaleY ? scaleY : scaleX;
					else
						currScale = scaleX > scaleY ? scaleX : scaleY;
					Size s = new Size(
						(int)(imageSize.Width * currScale + 0.5f),
						(int)(imageSize.Height * currScale + 0.5f));
					return s;
			}
			return imageSize;
		}
		public static Size ScaleImage(Size size, Size imageSize, TileItemImageScaleMode scaleMode, out Size notCroppedSize) {
			notCroppedSize = Size.Empty;
			switch(scaleMode) {
				case TileItemImageScaleMode.Default:
				case TileItemImageScaleMode.NoScale:
					return imageSize;
				case TileItemImageScaleMode.Stretch:
					return size;
				case TileItemImageScaleMode.StretchHorizontal:
					notCroppedSize = new Size(size.Width, imageSize.Height);
					return new Size(size.Width, Math.Min(size.Height, imageSize.Height));
				case TileItemImageScaleMode.StretchVertical:
					notCroppedSize = new Size(imageSize.Width, size.Height);
					return new Size(Math.Min(imageSize.Width, size.Width), size.Height);
				case TileItemImageScaleMode.ZoomInside:
				case TileItemImageScaleMode.ZoomOutside:
				case TileItemImageScaleMode.Squeeze:
					if(imageSize.Width < size.Width && imageSize.Height < size.Height)
						if(scaleMode == TileItemImageScaleMode.Squeeze)
							return imageSize;
					float scaleX = ((float)size.Width) / imageSize.Width;
					float scaleY = ((float)size.Height) / imageSize.Height;
					float currScale;
					if(scaleMode == TileItemImageScaleMode.ZoomInside || scaleMode == TileItemImageScaleMode.Squeeze)
						currScale = scaleX > scaleY ? scaleY : scaleX;
					else
						currScale = scaleX > scaleY ? scaleX : scaleY;
					int w = (int)(imageSize.Width * currScale + 0.5f);
					int h = (int)(imageSize.Height * currScale + 0.5f);
					notCroppedSize = new Size(w, h);
					return new Size(
						Math.Min(w, size.Width),
						Math.Min(h, size.Height));
			}
			return imageSize;
		}
	}
	public class TileControlLayoutGroup {
		public TileControlLayoutGroup() {
			Items = new List<TileControlLayoutItem>();
		}
		public TileGroup Group { get; set; }
		public TileGroupViewInfo GroupInfo { get; set; }
		public List<TileControlLayoutItem> Items { get; set; }
		public Rectangle Bounds { get; set; }
		public Rectangle TextBounds { get; set; }
		public TileControlLayoutItem GetNextItem(TileControlLayoutItem current) {
			int currentIndex = Items.IndexOf(current);
			if(currentIndex < 0 || currentIndex + 1 >= Items.Count)
				return null;
			return Items[currentIndex + 1];
		}
	}
	public class TileControlLayoutItem {
		public TileControlLayoutItem() {
			Position = new TileItemPosition();
		}
		public TileItem Item { get; set; }
		public TileItemViewInfo ItemInfo { get; set; }
		public TileItemPosition Position { get; set; }
		public Rectangle Bounds { get; set; }
		public void LayoutItem(TileGroupLayoutInfo info) {
			Bounds = new Rectangle(CalcLocation(info), ItemInfo.Size);
		}
		public TileControlViewInfo ControlInfo { get { return ItemInfo.ControlInfo; } }
		protected virtual Point CalcLocation(TileGroupLayoutInfo info) {
			if(!ControlInfo.IsRightToLeft) {
				if(ItemInfo.IsLarge) return info.Location;
				if(ItemInfo.IsSmall) {
					int x = info.Location.X;
					x += info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems);
					x += info.ItemPosition.SubColumn * (ControlInfo.SmallItemSize + ControlInfo.IndentBetweenItems);
					return new Point(x, info.Location.Y + info.ItemPosition.SubRow * (ControlInfo.SmallItemSize + ControlInfo.IndentBetweenItems));
				}
				return new Point(info.Location.X + info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
			else {
				if(ItemInfo.IsLarge) return new Point(info.Location.X - (ControlInfo.ItemSize * 2) - ControlInfo.IndentBetweenItems, info.Location.Y);
				if(ItemInfo.IsSmall) {
					int x = info.Location.X - ControlInfo.SmallItemSize;
					x -= info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems);
					x -= info.ItemPosition.SubColumn * (ControlInfo.SmallItemSize + ControlInfo.IndentBetweenItems);
					return new Point(x, info.Location.Y + info.ItemPosition.SubRow * (ControlInfo.SmallItemSize + ControlInfo.IndentBetweenItems));
				}
				int xpos = info.Location.X - ItemInfo.Size.Width;
				return new Point(xpos - info.ItemPosition.Column * (ControlInfo.ItemSize + ControlInfo.IndentBetweenItems), info.Location.Y);
			}
		}
		internal void LayoutItemVertical(Point point) {
			Bounds = new Rectangle(point, ItemInfo.Size);
		}
	}
	public class TileControlDropItemInfo {
		public TileControlDropSide GroupDropSide { get; set; }
		public TileItem DropItem { get; set; }
		public TileGroup Group { get; set; }
		public bool InsertNewGroupBeforeGroup { get; set; }
		public TileGroupViewInfo NearestGroupInfo { get; set; }
		public TileGroup NearestGroup { get { return NearestGroupInfo == null ? null : NearestGroupInfo.Group; } }
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public int Index {
			get {
				if(DropItem == null)
					return -1;
				return DropItem.Group.Items.IndexOf(DropItem);
			}
		}
		public override bool Equals(object obj) {
			TileControlDropItemInfo itemInfo = obj as TileControlDropItemInfo;
			if(itemInfo == null)
				return false;
			return itemInfo.DropItem == DropItem && itemInfo.Group == Group;
		}
	}
	public class TileControlLayoutCalculator {
		public TileControlLayoutCalculator(TileControlViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		public TileControlViewInfo ViewInfo { get; private set; }
		public virtual bool IsLayoutChanged(TileControlLayoutCalculator calc) {
			if(calc == null)
				return true;
			if(calc.Groups.Count != Groups.Count)
				return true;
			for(int i = 0; i < Groups.Count; i++) {
				if(Groups[i].Items.Count != calc.Groups[i].Items.Count)
					return true;
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					if(Groups[i].Items[j].Item != calc.Groups[i].Items[j].Item)
						return true;
				}
			}
			return false;
		}
		protected virtual void LayoutGroupsHorizontal() {
			Point loc = ViewInfo.GetStartPoint();
			foreach(TileControlLayoutGroup group in Groups) {
				loc.X = LayoutGroup(group, loc).X;
				if(group.Items.Count == 0 && !ViewInfo.Owner.IsDesignMode) continue;
				if(ViewInfo.IsRightToLeft)
					loc.X -= ViewInfo.Owner.Properties.IndentBetweenGroups;
				else
					loc.X += ViewInfo.Owner.Properties.IndentBetweenGroups;
			}
		}
		protected virtual void LayoutGroupsVertical() {
			Point loc = ViewInfo.GetStartPoint();
			ViewInfo.RealColumnCount = 0;
			foreach(TileControlLayoutGroup group in Groups) {
				loc.Y = LayoutGroup(group, loc).Y;
				if(group.Items.Count == 0 && !ViewInfo.Owner.IsDesignMode) continue;
				loc.Y += ViewInfo.Owner.Properties.IndentBetweenGroups;
			}
		}
		protected virtual void LayoutGroups() {
			bool shouldRemoveGraphics = false;
			if(ViewInfo.GInfo.Graphics == null) {
				ViewInfo.GInfo.AddGraphics(null);
				shouldRemoveGraphics = true;
			}
			try {
				if(ViewInfo.IsHorizontal)
					LayoutGroupsHorizontal();
				else
					LayoutGroupsVertical();
			}
			finally {
				if(shouldRemoveGraphics)
					ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected virtual Point LayoutGroup(TileControlLayoutGroup group, Point loc) {
			Rectangle rect = Rectangle.Empty;
			loc = group.GroupInfo.CalcTextBounds(loc, ref rect);
			group.TextBounds = rect;
			if(ViewInfo.IsHorizontal)
				return LayoutGroupHorizontal(group, loc);
			return LayoutGroupVertical(group, loc);
		}
		protected virtual int CalcFilledColumnCount(int row, TileControlLayoutGroup group, TileControlLayoutItem itemInfo) {
			var res = 0;
			var itemIndex = group.Items.IndexOf(itemInfo);
			for(int i = (itemIndex - 1); i >= 0; i--) {
				if(group.Items[i].Position.Row != row) {
					break;
				}
				res += group.Items[i].ItemInfo.IsLarge ? 2 : 1;
			}
			return res;
		}
		protected virtual void MovePositionVertical(TileGroupLayoutInfo info, TileControlLayoutGroup group, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			info.PrevItemPosition = info.ItemPosition.Clone();
			info.ColumnWidth += item.Bounds.Width;
			info.BottomY = Math.Max(info.BottomY, item.Bounds.Bottom);
			if(nextItem == null || CanStartNewRowVertical(info, group, item, nextItem)) {
				info.MaxColumnWidth = Math.Max(info.ColumnWidth, info.MaxColumnWidth);
				info.ColumnWidth = 0;
				ViewInfo.RealColumnCount = Math.Max(ViewInfo.RealColumnCount, item.Position.GroupColumn + (item.ItemInfo.IsLarge ? 2 : 1));
				info.ItemPosition.GroupColumn = 0;
				info.ItemPosition.Row++;
				info.Location = new Point(info.StartLocation.X, info.BottomY + ViewInfo.IndentBetweenItems);
				return;
			}
			if(ViewInfo.IsRightToLeft)
				info.Location = new Point(info.Location.X - item.Bounds.Width - ViewInfo.IndentBetweenItems, info.Location.Y);
			else
				info.Location = new Point(info.Location.X + item.Bounds.Width + ViewInfo.IndentBetweenItems, info.Location.Y);
			info.ColumnWidth += ViewInfo.IndentBetweenItems;
			info.ItemPosition.GroupColumn += item.ItemInfo.IsLarge ? 2 : 1;
		}
		protected virtual bool CanStartNewRowVertical(TileGroupLayoutInfo info, TileControlLayoutGroup group, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			if(ViewInfo.Owner.Properties.ColumnCount == 0) {
				if(ViewInfo.IsRightToLeft)
					return item.Bounds.Left - ViewInfo.IndentBetweenItems - nextItem.ItemInfo.Size.Width < ViewInfo.ContentBounds.Left;
				else
					return item.Bounds.Right + ViewInfo.IndentBetweenItems + nextItem.ItemInfo.Size.Width > ViewInfo.ContentBounds.Right;
			}
			item.Position.Row = info.ItemPosition.Row;
			int filledColumn = CalcFilledColumnCount(info.ItemPosition.Row, group, nextItem);
			int requestedColumn = nextItem.ItemInfo.IsLarge ? 2 : 1;
			return (filledColumn + requestedColumn) > ViewInfo.Owner.Properties.ColumnCount;
		}
		protected virtual Point LayoutGroupVertical(TileControlLayoutGroup group, Point start) {
			TileGroupLayoutInfo info = new TileGroupLayoutInfo();
			info.Location = start;
			info.StartLocation = start;
			info.ColumnX = info.Location.X;
			info.BottomY = start.Y;
			foreach(TileControlLayoutItem item in group.Items) {
				if(!item.ItemInfo.ShouldProcessItem) continue;
				item.Bounds = new Rectangle(Point.Empty, item.ItemInfo.Size);
				if(ViewInfo.IsRightToLeft)
					item.LayoutItemVertical(new Point(info.Location.X - item.Bounds.Width, info.Location.Y));
				else
					item.LayoutItemVertical(info.Location);
				item.Position = info.ItemPosition.Clone();
				MovePositionVertical(info, group, item, group.GetNextItem(item));
			}
			ViewInfo.RealColumnCount = Math.Max(info.ItemPosition.GroupColumn, ViewInfo.RealColumnCount);
			Point end = new Point(start.X, info.BottomY);
			if(ViewInfo.IsRightToLeft)
				group.Bounds = new Rectangle(new Point(start.X - info.MaxColumnWidth, start.Y), new Size(info.MaxColumnWidth, info.BottomY - start.Y));
			else
				group.Bounds = new Rectangle(start, new Size(info.MaxColumnWidth, info.BottomY - start.Y));
			if(group.Bounds.Size.IsEmpty && ViewInfo.Owner.IsDesignMode) {
				end = LayoutEmptyGroupInDesignTime(group, start);
			}
			return end;
		}
		protected virtual Point LayoutGroupHorizontal(TileControlLayoutGroup group, Point start) {
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
				MovePositionHorizontal(info, item, group.GetNextItem(item));
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
		protected Point LayoutEmptyGroupInDesignTime(TileControlLayoutGroup group, Point start) {
			Point end = start;
			group.Bounds = new Rectangle(start, new Size(ViewInfo.ItemSize, ViewInfo.ItemSize));
			end.Offset(ViewInfo.ItemSize, ViewInfo.ItemSize);
			return end;
		}
		protected virtual bool CanStartNewRow(TileGroupLayoutInfo info, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			int currentRowCount = info.ItemPosition.Row + item.ItemInfo.RowCount;
			int newRowCount = currentRowCount + nextItem.ItemInfo.RowCount;
			int availableRowCount = Math.Min(ViewInfo.Owner.Properties.RowCount, ViewInfo.AvailableRowCount);
			return newRowCount <= availableRowCount;
		}
		bool GroupColumnHasChanged(TileGroupLayoutInfo info) {
			return info.ItemPosition.GroupColumn > info.PrevItemPosition.GroupColumn;
		}
		bool RowHasChanged(TileGroupLayoutInfo info) {
			return info.ItemPosition.Row > info.PrevItemPosition.Row;
		}
		bool ColumnHasChanged(TileGroupLayoutInfo info) {
			return info.ItemPosition.Column > info.PrevItemPosition.Column;
		}
		bool SubColumnHasChanged(TileGroupLayoutInfo info) {
			return info.ItemPosition.SubColumn > info.PrevItemPosition.SubColumn;
		}
		bool SubRowHasChanged(TileGroupLayoutInfo info) {
			return info.ItemPosition.SubRow > info.PrevItemPosition.SubRow;
		}
		protected internal virtual void MovePositionHorizontal(TileGroupLayoutInfo info, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			MovePositionHorizontalCore(info, item, nextItem);
			if(!ViewInfo.IsRightToLeft)
				info.ColumnWidth = Math.Max(info.ColumnWidth, item.Bounds.Right - info.ColumnX);
			else
				info.ColumnWidth = Math.Max(info.ColumnWidth, info.ColumnX - item.Bounds.Left);
			if(GroupColumnHasChanged(info) || nextItem == null) {
				info.MaxColumnWidth = Math.Max(info.ColumnWidth, info.MaxColumnWidth);
				if(!ViewInfo.IsRightToLeft)
					info.ColumnX += info.MaxColumnWidth + (nextItem == null ? 0 : ViewInfo.IndentBetweenItems);
				else
					info.ColumnX -= info.MaxColumnWidth + (nextItem == null ? 0 : ViewInfo.IndentBetweenItems);
				info.MaxColumnWidth = 0;
				info.ColumnWidth = 0;
				info.Location = new Point(info.ColumnX, info.StartLocation.Y);
			}
			else if(RowHasChanged(info)) {
				info.Location = new Point(info.ColumnX, info.StartLocation.Y + info.ItemPosition.Row * (ViewInfo.GetItemHeight() + ViewInfo.IndentBetweenItems));
				info.MaxColumnWidth = Math.Max(info.ColumnWidth, info.MaxColumnWidth);
				info.ColumnWidth = 0;
			}
			else if(SubRowHasChanged(info)) {
				info.MaxColumnWidth = Math.Max(info.MaxColumnWidth, info.ColumnWidth);
				info.ColumnWidth = 0;
			}
		}
		protected internal virtual void MovePositionHorizontalCore(TileGroupLayoutInfo info, TileControlLayoutItem item, TileControlLayoutItem nextItem) {
			if(nextItem == null || item == null)
				return;
			info.PrevItemPosition.Assign(info.ItemPosition);
			if(item.ItemInfo.IsSmall && nextItem.ItemInfo.IsSmall) {
				if(info.ItemPosition.SubColumn == 0) {
					info.ItemPosition.SubColumn++;
					return;
				}
				info.ItemPosition.SubColumn = 0;
				if(info.ItemPosition.SubRow == 0) {
					info.ItemPosition.SubRow++;
					return;
				}
			}
			info.ItemPosition.SubColumn = 0;
			info.ItemPosition.SubRow = 0;
			if((item.ItemInfo.IsSmall || item.ItemInfo.IsMedium) && (nextItem.ItemInfo.IsSmall || nextItem.ItemInfo.IsMedium)) {
				if(info.ItemPosition.Column == 0) {
					info.ItemPosition.Column++;
					return;
				}
			}
			info.ItemPosition.Column = 0;
			if(CanStartNewRow(info, item, nextItem)) {
				info.ItemPosition.Row += item.ItemInfo.RowCount;
				return;
			}
			info.ItemPosition.Row = 0;
			info.ItemPosition.GroupColumn++;
		}
		public virtual void CreateLayoutInfo(TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			if(Groups != null)
				return;
			Groups = CreateLayoutInfoCore(dragItem, dropInfo);
		}
		protected virtual List<TileControlLayoutGroup> CreateLayoutInfoCore(TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			List<TileControlLayoutGroup> res = new List<TileControlLayoutGroup>();
			foreach(TileGroup group in ViewInfo.Owner.Groups) {
				if(!ShouldLayoutGroup(group))
					continue;
				res.Add(CreateGroupLayoutInfo(group, dragItem, dropInfo));
			}
			return res;
		}
		protected virtual bool ShouldLayoutGroup(TileGroup group) {
			return group.Visible;
		}
		protected virtual void GetItemsRange(TileGroup group, ref int startIndex, ref int endIndex) {
			startIndex = 0;
			endIndex = group.Items.Count - 1;
		}
		protected virtual TileControlLayoutGroup CreateGroupLayoutInfo(TileGroup group, TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			TileControlLayoutGroup layoutInfo = new TileControlLayoutGroup() { Group = group, GroupInfo = group.GroupInfo };
			int start = 0;
			int end = 0;
			GetItemsRange(group, ref start, ref end);
			for(int n = start; n <= end; n++) {
				var item = group.Items[n];
				if(!CanCreateItem(item, dragItem)) continue;
				if(dragItem != null && dropInfo != null && dropInfo.Group == group && dropInfo.DropItem == item) {
					layoutInfo.Items.Add(GetNewLayoutItem(dragItem.Item, dragItem));
				}
				layoutInfo.Items.Add(GetNewLayoutItem(item, item.ItemInfo));
			}
			if(dropInfo != null && dropInfo.DropItem != null) {
				if(dragItem != null && group == dropInfo.Group && dropInfo.Index >= group.Items.Count) {
					layoutInfo.Items.Add(GetNewLayoutItem(dragItem.Item, dragItem));
				}
			}
			return layoutInfo;
		}
		protected virtual bool CanCreateItem(TileItem item, TileItemViewInfo dragItem) {
			if(!item.Visible) return false;
			if(dragItem != null && item == dragItem.Item && ViewInfo.Owner.Handler.State == TileControlHandlerState.DragMode) return false;
			return true;
		}
		public List<TileControlLayoutGroup> Groups { get; private set; }
		public void CalcGroupsLayout(TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			CreateLayoutInfo(dragItem, dropInfo);
			LayoutGroups();
		}
		protected virtual TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new TileControlLayoutCalculator(viewInfo);
		}
		protected TileControlLayoutItem GetNewLayoutItem(Rectangle bounds, TileItem item, TileItemPosition position) {
			return GetNewLayoutItemCore(bounds, item, item.ItemInfo, position);
		}
		protected TileControlLayoutItem GetNewLayoutItem(TileItem item, TileItemViewInfo itemInfo) {
			return GetNewLayoutItemCore(Rectangle.Empty, item, itemInfo, null);
		}
		protected virtual TileControlLayoutItem GetNewLayoutItemCore(Rectangle bounds, TileItem item, TileItemViewInfo itemInfo, TileItemPosition position) {
			return new TileControlLayoutItem() { Bounds = bounds, Item = item, ItemInfo = itemInfo, Position = position };
		}
		public virtual TileControlLayoutCalculator Clone() {
			TileControlLayoutCalculator calc = GetNewLayoutCalculator(ViewInfo);
			calc.Groups = new List<TileControlLayoutGroup>();
			for(int i = 0; i < Groups.Count; i++) {
				TileControlLayoutGroup group = new TileControlLayoutGroup() { Bounds = Groups[i].Bounds, Group = Groups[i].Group, GroupInfo = Groups[i].GroupInfo };
				calc.Groups.Add(group);
				foreach(TileControlLayoutItem item in Groups[i].Items) {
					TileControlLayoutItem item2 = GetNewLayoutItem(item.Bounds, item.Item, item.Position.Clone());
					group.Items.Add(item2);
				}
			}
			return calc;
		}
		public virtual TileItemViewInfo GetItemAfterDragItem() {
			foreach(TileControlLayoutGroup group in Groups) {
				for(int i = 0; i < group.Items.Count; i++) {
					if(group.Items[i].ItemInfo.Item != ViewInfo.DragItem.Item)
						continue;
					i++;
					if(i < group.Items.Count)
						return group.Items[i].ItemInfo;
					return null;
				}
			}
			return null;
		}
	}
}
