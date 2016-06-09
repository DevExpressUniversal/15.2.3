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
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public abstract class AccordionElementBaseViewInfo : ISupportContextItems, ISupportXtraAnimation {
		object animationObject = new object();
		public AccordionElementBaseViewInfo(AccordionControlElement item, AccordionControlViewInfo viewInfo) {
			Element = item;
			ControlInfo = viewInfo;
			InnerContentHeight = ContentTopIndent = 0;
			if(Element.Style == ElementStyle.Item)
				CheckInnerContentHeight();
		}
		protected internal void SubscribeOnElementEvents() {
			if(Element == null) return;
			Element.ExpandedChanged += OnExpandedChanged;
		}
		protected internal void UnsubscribeOnElementEvents() {
			if(Element == null) return;
			Element.ExpandedChanged -= OnExpandedChanged;
		}
		protected internal bool IsInAnimation {
			get { return Element.IsInAnimation; }
			set { Element.IsInAnimation = value; }
		}
		protected void OnExpandedChanged(object sender, EventArgs e) {
			ClearPaintAppearance();
			OnExpandedChangedCore();
		}
		protected internal virtual void OnExpandedChangedCore() {
			if(Element.Style == ElementStyle.Item && Element.ContentContainer == null) return;
			if(Element.LockAnimation || ControlInfo.AccordionControl.IsUpdate) return;
			AccordionElementBaseViewInfo collapsingElement = GetCollapsingElement();
			if(collapsingElement != null) {
				collapsingElement.ClearPaintAppearance();
				RunDoubleAnimation(collapsingElement);
				return;
			}
			RunAnimation(null);
		}
		protected AccordionElementBaseViewInfo GetCollapsingElement() {
			if(ControlInfo.AccordionControl.GetExpandElementMode() == ExpandElementMode.Single) {
				return ControlInfo.Helper.GetNonParentExpandedElement(Element, ControlInfo.ElementsInfo);
			}
			if(ControlInfo.AccordionControl.GetExpandElementMode() == ExpandElementMode.SingleItem) {
				if(Element.Style == ElementStyle.Item) return ControlInfo.Helper.GetExpandedItem(this);
			}
			return null;
		}
		protected void RunDoubleAnimation(AccordionElementBaseViewInfo exItem) {
			Element.IsInAnimation = true;
			exItem.Element.LockAnimation = true;
			exItem.IsInAnimation = true;
			CollapseItem(exItem);
			if(exItem.Element.OwnerElement == null || exItem.Element.OwnerElement.Expanded)
				RunAnimation(exItem);
			else {
				exItem.IsInAnimation = false;
				RunAnimation(null);
			}
			exItem.Element.LockAnimation = false;
		}
		protected void CollapseItem(AccordionElementBaseViewInfo exItem) {
			exItem.Element.BeginUpdate();
			try {
				exItem.Element.Expanded = false;
			}
			finally { exItem.Element.EndUpdate(); }
		}
		protected internal virtual void RunAnimation(AccordionElementBaseViewInfo collapsingItem) {
			if(ControlInfo.AccordionControl.AnimationType == AnimationType.None || ControlInfo.AccordionControl.IsDesignMode || ControlInfo.ForceExpanding) {
				ImmediatelyFinishAnimation(collapsingItem);
				return;
			}
			CheckAnimationAddedTop(collapsingItem);
			ControlInfo.CollapsingElement = collapsingItem;
			if(ControlInfo.AccordionControl.AnimationType == AnimationType.Spline)
				XtraAnimator.Current.AddAnimation(new AccordionControlSplineAnimationInfo(this, animationObject, collapsingItem, Expanded, 300));
			else XtraAnimator.Current.AddAnimation(new AccordionControlAnimationInfo(this, animationObject, collapsingItem, Expanded, 300));
		}
		protected void ImmediatelyFinishAnimation(AccordionElementBaseViewInfo collapsingItem) {
			ControlInfo.HideControls();
			OnAnimationComplete(true);
			if(collapsingItem != null) collapsingItem.IsInAnimation = false;
			Element.IsInAnimation = false;
			ControlInfo.IsInAnimation = false;
		}
		protected void CheckAnimationAddedTop(AccordionElementBaseViewInfo collapsingItem) {
			if(collapsingItem != null && collapsingItem.ContentContainerHeight > ContentContainerHeight)
				ControlInfo.AnimationAddedTop = ContentContainerHeight - collapsingItem.ContentContainerHeight;
			if(!Expanded) {
				AccordionGroupViewInfo groupInfo = this as AccordionGroupViewInfo;
				if(groupInfo != null) {
					ControlInfo.AnimationAddedTop = -groupInfo.CalcGroupExpandedHeight();
				}
				else ControlInfo.AnimationAddedTop = -ContentContainerHeight;
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = CreateContextButtonsHandler();
				return contextButtonsHandler;
			}
		}
		protected virtual ContextItemCollectionHandler CreateContextButtonsHandler() {
			return new ContextItemCollectionHandler(ContextButtonsViewInfo);
		}
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = CreateContextButtonsViewInfo();
				return contextButtonsViewInfo;
			}
		}
		protected virtual ContextItemCollectionViewInfo CreateContextButtonsViewInfo() {
			return new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems.Clone() as ContextItemCollection, ContextButtonOptions, this);
		}
		protected virtual ContextItemCollectionOptions ContextButtonOptions { get { return ControlInfo.AccordionControl.ContextButtonsOptions; } }
		public AccordionControlElement Element { get; private set; }
		Items2Panel itemsPanel;
		protected Items2Panel ItemsPanel {
			get {
				if(itemsPanel == null)
					itemsPanel = CreateItemsPanel();
				return itemsPanel;
			}
		}
		public AccordionControlViewInfo ControlInfo { get; private set; }
		protected Items2Panel CreateItemsPanel() {
			Items2Panel panel = new Items2Panel();
			panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
			panel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			panel.Content1Location = ItemLocation.Left;
			panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Left;
			panel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			return panel;
		}
		protected internal Rectangle RealHeaderBounds { get; set; }
		protected internal Rectangle RealHeaderContentBounds { get; set; }
		protected internal Rectangle RealContextButtonsBounds { get; set; }
		protected internal Rectangle RealImageBounds { get; set; }
		protected internal Rectangle RealTextBounds { get; set; }
		protected internal Rectangle RealExpandCollapseButtonBounds { get; set; }
		protected internal Size StringSize { get; set; }
		protected internal virtual bool Expanded { get { return Element.Expanded; } }
		public virtual void CalcViewInfo(Point location, int width) {
			this.elementContentMinHeight = CalcElementContentMinHeight(width);
			RealHeaderBounds = CalcHeaderBounds(location, width);
			RealHeaderContentBounds = CalcHeaderContentBounds();
			CalcImageAndTextBounds();
			CalcContextButtons();
			CheckBounds();
			if(Element != null) Element.Bounds = RealHeaderBounds;
			CheckHeaderControlBounds();
			if(Element.Style == ElementStyle.Item) CheckContentContainerBounds();
		}
		int elementContentMinHeight = 0;
		protected internal int ElementContentMinHeight { get { return elementContentMinHeight; } }
		protected virtual int CalcElementContentMinHeight(int width) {
			int contentHeight = CalcImageAndTextBestHeight(width);
			if(Element.HeaderControl != null && !ControlInfo.IsMinimized) {
				contentHeight = Math.Max(contentHeight, Element.HeaderControl.Height);
			}
			contentHeight = Math.Max(contentHeight, ContextButtonsViewInfo.GetButtonsSizeByAlignment(ContextItemAlignment.CenterFar).Height);
			if(!ShouldShowExpandButtons) return contentHeight;
			return Math.Max(contentHeight, CalcExpandCollapseButtonSize().Height);
		}
		protected virtual int CalcImageAndTextBestHeight(int width) {
			Size imageSize = CalcImageBestSize();
			int textMaxWidth = CalcTextMaxWidth(width, imageSize);
			RaiseCustomElementText();
			if(textMaxWidth == 0 || (imageSize != Size.Empty && ControlInfo.IsMinimized)) {
				StringSize = Size.Empty;
				return imageSize.Height;
			}
			int textWidth = ControlInfo.IsMinimized ? 0 : textMaxWidth;
			StringSize = ControlInfo.CalcStringSize(Element, DisplayText, textWidth);
			return Math.Max(StringSize.Height, imageSize.Height);
		}
		protected virtual int CalcTextMaxWidth(int width, Size imageSize) {
			int textMaxWidth = CalcHeaderWidth(width);
			textMaxWidth -= GetHeaderInfo().Element.ContentMargins.ToPadding().Horizontal;
			if(imageSize != Size.Empty) {
				if(ControlInfo.IsMinimized)
					return 0;
				textMaxWidth = Math.Max(0, textMaxWidth - imageSize.Width - Element.TextToImageDistance);
			}
			if(!ControlInfo.IsMinimized) {
				int contextButtonsWidth = CalcContextButtonsWidth();
				textMaxWidth -= contextButtonsWidth;
				if(Element.HeaderControl != null) {
					textMaxWidth -= Element.HeaderControl.Width;
					if(contextButtonsWidth > 0) textMaxWidth -= Element.HeaderControlToContextButtonsDistance;
				}
			}
			if(ShouldShowExpandButtons) {
				int expandButtonWidth = CalcExpandCollapseButtonSize().Width;
				if(expandButtonWidth > 0) {
					textMaxWidth -= expandButtonWidth + ExpandCollapseButtonIndent;
				}
			}
			return Math.Max(0, textMaxWidth);
		}
		protected internal Size CalcImageBestSize() {
			if(Element.GetOriginalImage() != null) {
				return CalcMinImageSize();
			}
			return Size.Empty;
		}
		protected internal bool ShouldShowExpandButtons {
			get {
				if(ControlInfo.IsMinimized) return false;
				return (ControlInfo.AccordionControl.ShowGroupExpandButtons && Element.Style == ElementStyle.Group) || Element.HasContentContainer;
			}
		}
		protected Size CalcExpandCollapseButtonSize() {
			if(ControlInfo.UseOffice2003Style)
				return Office2003PaintHelper.ExpandCollapseButtonSize;
			if(ControlInfo.UseFlatStyle)
				return FlatPaintHelper.ExpandCollapseButtonSize;
			SkinElement elem = GetExpandCollapseSkinElement();
			if(elem == null) return Size.Empty;
			return AccordionControlHelper.CalcExpandCollapseButtonSize(elem);
		}
		protected void RaiseCustomElementText() {
			CustomElementTextEventArgs custom = new CustomElementTextEventArgs(Text, this);
			ControlInfo.AccordionControl.RaiseCustomElementText(custom);
			DisplayText = custom.Text;
		}
		protected Size CalcMinImageSize() {
			if(this.thumbnailImage != null) return ImageSize;
			int minHeight = CalcImageMinHeight();
			return CalcImageSize(Element.GetOriginalImage().Size, minHeight);
		}
		protected int CalcImageMinHeight() {
			int minHeight = CalcHeaderHeight() - GetHeaderInfo().Element.ContentMargins.ToPadding().Vertical;
			minHeight = Math.Max(minHeight, (int)PaintAppearance.CalcTextSize(ControlInfo.GInfo.Cache, "Wq", 0).Height);
			if(Element.HeaderControl == null) return minHeight;
			return Math.Max(minHeight, Element.HeaderControl.Height);
		}
		protected internal string DisplayText { get; set; }
		protected internal void CheckContentContainerBounds() {
			if(Element.ContentContainer == null) return;
			Element.ContentContainer.Width = RealHeaderBounds.Width;
			Element.ContentContainer.Location = new Point(RealHeaderBounds.X, RealHeaderBounds.Bottom);
		}
		protected void CheckInnerContentHeight() {
			if(Element.ContentContainer == null || !Element.Expanded) {
				InnerContentHeight = 0;
			}
			else InnerContentHeight = Element.ContentContainer.Height;
		}
		protected internal Point LayoutItem(Point current) {
			if(!Element.GetVisible()) return current;
			CalcViewInfo(current, ControlInfo.ContentRect.Width);
			current.Y = CalcItemHeight();
			current.Y += GetContentContainerHeight();
			return current;
		}
		protected int CalcItemHeight() {
			if((Element.OwnerElement == null || Element.OwnerElement.IsInAnimation) && ItemAnimationMaxHeight != -1)
				return Math.Min(RealHeaderBounds.Y + ItemAnimationMaxHeight, RealHeaderBounds.Bottom);
			return RealHeaderBounds.Bottom;
		}
		protected void CheckBounds() {
			RealHeaderBounds = ControlInfo.Helper.CheckBounds(RealHeaderBounds);
			RealHeaderContentBounds = ControlInfo.Helper.CheckBounds(RealHeaderContentBounds);
			RealTextBounds = ControlInfo.Helper.CheckBounds(RealTextBounds);
			RealImageBounds = ControlInfo.Helper.CheckBounds(RealImageBounds);
			RealExpandCollapseButtonBounds = ControlInfo.Helper.CheckBounds(RealExpandCollapseButtonBounds);
			RealContextButtonsBounds = ControlInfo.Helper.CheckBounds(RealContextButtonsBounds);
		}
		protected internal void CheckHeaderControlBounds() {
			if(Element.HeaderControl == null || !Element.HeaderVisible || RealHeaderContentBounds.Height == 0) return;
			Element.HeaderControl.Location = new Point(GetHeaderControlLeft, RealHeaderContentBounds.Y + (RealHeaderContentBounds.Height - Element.HeaderControl.Height) / 2);
		}
		protected internal virtual void CalcContextButtons() {
			if(ControlInfo.IsMinimized) {
				RealContextButtonsBounds = Rectangle.Empty;
				return;
			}
			int x;
			if(Image == null || Element.TextPosition == TextPosition.AfterImage) x = TextBounds.Right;
			else x = ImageBounds.Right;
			RealContextButtonsBounds = new Rectangle(x, RealHeaderContentBounds.Y, RealHeaderContentBounds.Right - x, RealHeaderContentBounds.Height);
			ContextButtonsViewInfo.Refresh();
		}
		protected virtual Size CalcImageSize(Size imageSize, int maxImageHeight) {
			if(Element.ImageLayoutMode == ImageLayoutMode.OriginalSize)
				return imageSize;
			Size maxSize = new Size(imageSize.Width, maxImageHeight);
			if(Element.ImageLayoutMode == ImageLayoutMode.Stretch) {
				float k = (float)imageSize.Height / maxSize.Height;
				return new Size((int)(imageSize.Width / k), (int)(imageSize.Height / k));
			}
			if(maxSize.Height < imageSize.Height || maxSize.Width < imageSize.Width) {
				float k = Math.Max((float)imageSize.Height / maxSize.Height, (float)imageSize.Width / maxSize.Width);
				return new Size((int)(imageSize.Width / k), (int)(imageSize.Height / k));
			}
			return imageSize;
		}
		private void CalcImageAndTextBounds() {
			Rectangle image = Rectangle.Empty, text = Rectangle.Empty;
			if(Element.TextPosition == TextPosition.BeforeImage) {
				ItemsPanel.ArrangeItems(RealHeaderContentBounds, StringSize, ImageSize, ref text, ref image);
				if(StringSize != Size.Empty) {
					image.X += Element.TextToImageDistance;
				}
			}
			else {
				ItemsPanel.ArrangeItems(RealHeaderContentBounds, ImageSize, StringSize, ref image, ref text);
				if(ImageSize != Size.Empty) {
					text.X += Element.TextToImageDistance;
				}
			}
			text.Width = Math.Min(text.Width, RealHeaderContentBounds.Width);
			text.Y = RealHeaderContentBounds.Y + (RealHeaderContentBounds.Height - text.Height) / 2;
			if(ControlInfo.IsMinimized) {
				text.X = RealHeaderContentBounds.X + (RealHeaderContentBounds.Width - text.Width) / 2;
				image.X = RealHeaderContentBounds.X + (RealHeaderContentBounds.Width - image.Width) / 2;
			}
			RealTextBounds = text;
			RealImageBounds = image;
		}
		protected virtual Rectangle CalcHeaderContentBoundsCore() {
			if(ControlInfo.UseOffice2003Style) return Office2003PaintHelper.CalcContentBounds(RealHeaderBounds);
			if(ControlInfo.UseFlatStyle) return FlatPaintHelper.CalcContentBounds(RealHeaderBounds);
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetHeaderInfo());
		}
		protected virtual Rectangle CalcHeaderContentBounds() {
			Rectangle contentRect = CalcHeaderContentBoundsCore();
			if(Element.Style == ElementStyle.Item && !Element.HasContentContainer) return contentRect;
			Point skinOffset = Point.Empty;
			RealExpandCollapseButtonBounds = CalcExpandCollapseButtonBounds(contentRect, ref skinOffset);
			if(RealExpandCollapseButtonBounds.Width != 0) {
				int expandButtonWidth = RealExpandCollapseButtonBounds.Width + ExpandCollapseButtonIndent;
				if(!IsRootElement) {
					contentRect.X += expandButtonWidth + skinOffset.X;
					contentRect.Width -= skinOffset.X + expandButtonWidth;
				}
				else contentRect.Width += skinOffset.X - expandButtonWidth;
			}
			return contentRect;
		}
		const int ExpandCollapseButtonIndent = 3;
		protected Rectangle CalcExpandCollapseButtonBounds(Rectangle content, ref Point offset) {
			if(!ShouldShowExpandButtons) return Rectangle.Empty;
			Size size;
			if(ControlInfo.UseOffice2003Style) {
				size = Office2003PaintHelper.ExpandCollapseButtonSize;
			}
			else if(ControlInfo.UseFlatStyle) {
				size = FlatPaintHelper.ExpandCollapseButtonSize;
			}
			else {
				SkinElement elem = GetExpandCollapseSkinElement();
				if(elem == null) return Rectangle.Empty;
				size = AccordionControlHelper.CalcExpandCollapseButtonSize(elem);
				offset = elem.Offset.Offset;
			}
			int x = IsRootElement ? content.Right - size.Width : content.X;
			return new Rectangle(x + offset.X, content.Y + (content.Height - size.Height) / 2 + offset.Y, size.Width, size.Height);
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null)
					paintAppearance = ControlInfo.CreateElementPaintAppearance(Element, State);
				return paintAppearance;
			}
		}
		protected internal void ClearPaintAppearance() {
			this.paintAppearance = null;
		}
		protected internal virtual void ClearAppearances() {
			ClearPaintAppearance();
		}
		public ObjectState State {
			get {
				if(!Element.GetEnabled())
					return ObjectState.Disabled;
				if(ControlInfo.PressedInfo.ItemInfo == this || ControlInfo.AccordionControl.SelectedElement == Element)
					return ObjectState.Pressed;
				if(ControlInfo.HoverInfo.ItemInfo == this)
					return ObjectState.Hot;
				return ObjectState.Normal;
			}
		}
		protected internal virtual Rectangle CalcHeaderBounds(Point location, int width) {
			int x = location.X + CalcHeaderX();
			Rectangle bounds = new Rectangle(x, location.Y, CalcHeaderWidth(width), CalcHeaderHeight());
			return bounds;
		}
		protected int CalcHeaderHeight(int height, SkinElementInfo headerInfo, int margins) {
			height = Math.Max(height - margins, ElementContentMinHeight) + margins;
			return Math.Max(height, headerInfo.Element.Size.MinSize.Height);
		}
		protected int CalcHeaderWidth(int width) {
			if(ControlInfo.ShouldKeepContentForScrollBar())
				width -= ControlInfo.Scrollers.VScrollBar.Bounds.Width;
			return width - CalcHeaderX();
		}
		protected internal abstract int CalcHeaderX();
		protected GraphicsInfo GInfo { get { return ControlInfo.GInfo; } }
		public string Text { get { return Element.Text; } }
		Image thumbnailImage;
		public Image Image {
			get {
				if(thumbnailImage == null)
					thumbnailImage = CreateThumbnailImage();
				return thumbnailImage;
			}
		}
		Image CreateThumbnailImage() {
			Image img = Element.GetOriginalImage();
			if(img != null) return CreateThumbnailImageCore(img);
			return null;
		}
		Image CreateThumbnailImageCore(Image image) {
			Size size = CalcImageSize(image.Size, HeaderContentBounds.Height);
			Bitmap thumb = new Bitmap(size.Width, size.Height);
			using(Graphics g = Graphics.FromImage(thumb)) {
				Rectangle destRect = new Rectangle(Point.Empty, size);
				Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
				g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
			}
			return thumb;
		}
		Image animationImage;
		Image AnimationImage {
			get {
				if(animationImage == null)
					animationImage = CreateAnimationImage();
				return animationImage;
			}
		}
		protected internal Image GetImage() {
			if(ControlInfo.IsInAnimation) return AnimationImage;
			return Image;
		}
		protected Size ImageSize {
			get {
				if(Image == null) return Size.Empty;
				return Image.Size;
			}
		}
		protected Image CreateAnimationImage() {
			Bitmap bitmapFinal;
			Bitmap img = (Bitmap)Image;
			if(img == null) return null;
			bitmapFinal = img.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			return bitmapFinal;
		}
		protected internal virtual int CalcHeaderHeight() {
			if(!Element.HeaderVisible) return 0;
			return GetItemHeight();
		}
		protected virtual int GetItemHeight() {
			int itemHeight = GetItemHeightCore();
			SkinElementInfo headerInfo = GetHeaderInfo();
			int verticalMargins = headerInfo.Element.ContentMargins.Bottom + headerInfo.Element.ContentMargins.Top;
			itemHeight = CalcHeaderHeight(itemHeight, headerInfo, verticalMargins);
			if(Element.HeaderControl == null) return itemHeight;
			return Math.Max(itemHeight, Element.HeaderControl.Height);
		}
		protected abstract int GetItemHeightCore();
		protected internal virtual int GetContentContainerHeight() {
			if(!Expanded || Element.Style == ElementStyle.Group) return 0;
			return ContentContainerHeight;
		}
		protected internal virtual int ContentContainerHeight {
			get {
				if(Element.ContentContainer == null) return 0;
				return Element.ContentContainer.Height;
			}
		}
		protected internal int InnerContentHeight {
			get {
				if(!Element.HeaderVisible && Element.GetVisible() && Element.Style == ElementStyle.Item) {
					if(Element.ContentContainer != null)
						return Element.ContentContainer.Height;
				}
				return Element.InnerContentHeight;
			}
			set {
				Element.InnerContentHeight = value;
			}
		}
		protected internal virtual int ContentTopIndent { get; set; }
		protected internal Rectangle GetAnimationBounds(Rectangle bounds) {
			if(ControlInfo.ShouldDrawClearControl) return bounds;
			Rectangle rect = bounds;
			rect.Y += ContentTopIndent + ControlInfo.AnimationIndentIncrement;
			return rect;
		}
		public Rectangle HeaderBounds { get { return GetAnimationBounds(RealHeaderBounds); } }
		public Rectangle ExpandCollapseButtonBounds { get { return GetAnimationBounds(RealExpandCollapseButtonBounds); } }
		public Rectangle ImageBounds { get { return GetAnimationBounds(RealImageBounds); } }
		public Rectangle HeaderContentBounds { get { return GetAnimationBounds(RealHeaderContentBounds); } }
		public Rectangle ContextButtonsBounds { get { return GetAnimationBounds(RealContextButtonsBounds); } }
		public Rectangle TextBounds { get { return GetAnimationBounds(RealTextBounds); } }
		public int GetHeaderControlLeft {
			get {
				int buttonsWidth = CalcContextButtonsWidth();
				int indent = buttonsWidth == 0 ? 0 : Element.HeaderControlToContextButtonsDistance;
				if(ControlInfo.AccordionControl.IsRightToLeft)
					return HeaderContentBounds.X + indent + buttonsWidth;
				return HeaderContentBounds.Right - indent - buttonsWidth - Element.HeaderControl.Width;
			}
		}
		protected int CalcContextButtonsWidth() {
			return ContextButtonsViewInfo.GetButtonsSizeByAlignment(ContextItemAlignment.CenterFar).Width;
		}
		protected internal virtual SkinElementInfo GetHeaderInfo() {
			SkinElement elem = ControlInfo.GetHeaderSkinElement(Element.Style, IsRootElement);
			SkinElementInfo res = new SkinElementInfo(elem, HeaderBounds);
			res.ImageIndex = -1;
			res.State = State;
			return res;
		}
		protected internal virtual bool IsRootElement { get { return Element.OwnerElement == null; } }
		#region ContextButtons
		Rectangle ISupportContextItems.ActivationBounds {
			get { return ContextButtonsBounds; }
		}
		bool ISupportContextItems.CloneItems {
			get { return true; }
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return ContextItemsCore; }
		}
		protected virtual ContextItemCollection ContextItemsCore {
			get { return null; }
		}
		System.Windows.Forms.Control ISupportContextItems.Control {
			get { return ControlInfo.AccordionControl; }
		}
		bool ISupportContextItems.DesignMode {
			get { return ControlInfo.AccordionControl.IsDesignMode; }
		}
		Rectangle ISupportContextItems.DisplayBounds {
			get { return ContextButtonsBounds; }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return ContextButtonsBounds; }
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
		DevExpress.LookAndFeel.UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return ControlInfo.LookAndFeel.ActiveLookAndFeel; }
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return ControlInfo.AccordionControl.ContextButtonsOptions; }
		}
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			ControlInfo.AccordionControl.RaiseContextItemClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ControlInfo.AccordionControl.RaiseCustomContextButtonToolTip(new AccordionControlContextButtonToolTipEventArgs(Element, e));
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
			ControlInfo.AccordionControl.RaiseContextButtonCustomize(new AccordionControlContextButtonCustomizeEventArgs(Element, item));
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			ControlInfo.AccordionControl.Invalidate(rect);
		}
		void ISupportContextItems.Redraw() {
			ControlInfo.AccordionControl.Invalidate();
		}
		void ISupportContextItems.Update() {
			ControlInfo.AccordionControl.Update();
		}
		#endregion
		protected internal virtual void ProcessMouseClick() {
			if(ControlInfo.IsInAnimation || !Element.GetEnabled()) return;
			bool handled = Element.RaiseElementClick();
			if(handled) return;
			if(ControlInfo.IsMinimized) {
				ControlInfo.OnMinimizedElementClick(Element);
				return;
			}
			if(Element.CanExpandElement) Element.InvertExpanded();
			if(Element.AccordionControl != null) {
				Element.AccordionControl.SetSelectedElement(Element);
			}
		}
		public bool CanAnimate { get { return true; } }
		public System.Windows.Forms.Control OwnerControl { get { return ControlInfo.AccordionControl; } }
		protected internal int ItemAnimationMaxHeight = -1;
		protected internal virtual void OnStartAnimation() {
			indentToStartAnimation = ControlInfo.ContentTopIndent;
			ControlInfo.HideControls();
		}
		protected internal virtual void CalcItemsIndent() { }
		protected internal virtual void OnAnimation(SlideAnimationInfo info) {
			InnerContentHeight = info.ContentHeight;
			CalcItemsIndent();
			if(!info.ShouldUpdateAnimationInc) return;
			if(Expanded) {
				if(RealHeaderBounds.Bottom + ContentTopIndent + info.ContentHeight > ControlInfo.ContentRect.Height) {
					ControlInfo.AnimationIndentIncrement = ControlInfo.ContentRect.Height - (RealHeaderBounds.Bottom + ContentTopIndent + info.ContentHeight);
				}
				int contentHeight = GetContentHeight();
				int collapsingElementContentHeight = ControlInfo.CollapsingElement == null ? 0 : ControlInfo.CollapsingElement.GetContentHeight();
				if(ControlInfo.ContentBottom + ControlInfo.AnimationIndentIncrement - collapsingElementContentHeight + info.CollapsingItemHeight - contentHeight + info.ContentHeight < ControlInfo.ContentRect.Height) {
					ControlInfo.AnimationIndentIncrement = Math.Min(-ControlInfo.ContentTopIndent, ControlInfo.ContentRect.Height - (ControlInfo.ContentBottom - collapsingElementContentHeight + info.CollapsingItemHeight - contentHeight + info.ContentHeight));
				}
			}
			else {
				if(ControlInfo.ContentBottom + info.ContentHeight < ControlInfo.ContentRect.Height) {
					ControlInfo.AnimationIndentIncrement = Math.Min(-ControlInfo.ContentTopIndent, ControlInfo.ContentRect.Height - (ControlInfo.ContentBottom + info.ContentHeight));
				}
			}
		}
		protected virtual int GetContentHeight() {
			return ContentContainerHeight;
		}
		protected internal virtual void OnAnimationComplete(bool shouldRefresh) {
			RemoveQuery();
			if(!Expanded && Element.ContentContainer != null && Element.Style == ElementStyle.Item)
				RaiseItemContentContainerHidden();
			OnAnimationCompleteCore(shouldRefresh);
			ExecuteNextQuery();
		}
		protected void ExecuteNextQuery() {
			ExpandCollapseElementList list = ControlInfo.ElementsToExpandCollapse;
			while(list.Count > 0) {
				if(list[0].Element.Expanded) {
					if(list[0].NewState == AccordionElementState.Expanded) {
						list.Remove(list[0]);
						continue;
					}
				}
				else {
					if(list[0].NewState == AccordionElementState.Collapsed) {
						list.Remove(list[0]);
						continue;
					}
				}
				list[0].Element.InvertExpandedCore();
				return;
			}
		}
		protected void RemoveQuery() {
			ExpandCollapseElementList list = ControlInfo.ElementsToExpandCollapse;
			for(int i = 0; i < list.Count; i++) {
				if(list[i].Element.Equals(Element)) {
					list.Remove(list[i]);
					return;
				}
			}
		}
		protected void RaiseExpandStateChanged() {
			ControlInfo.AccordionControl.RaiseExpandStateChanged(new ExpandStateChangedEventArgs(Element));
		}
		protected internal virtual void OnAnimationCompleteCore(bool shouldRefresh) {
			if(!shouldRefresh) {
				RaiseExpandStateChanged();
				return;
			}
			ControlInfo.CollapsingElement = null;
			ControlInfo.ContentTopIndent = indentToStartAnimation + ControlInfo.AnimationIndentIncrement;
			ControlInfo.AnimationIndentIncrement = 0;
			foreach(AccordionGroupViewInfo groupInfo in ControlInfo.ElementsInfo) {
				groupInfo.ContentTopIndent = 0;
			}
			ControlInfo.AnimationAddedTop = 0;
			if(shouldRefresh) {
				ControlInfo.Scrollers.UnlockUpdateScrollBar();
				ControlInfo.AccordionControl.Refresh();
				ControlInfo.UpdateScrollBars();
			}
			ControlInfo.ShowVisibleControls();
			if(Expanded && Element.ContentContainer != null) InnerContentHeight = Element.ContentContainer.Height;
			if(!Expanded) InnerContentHeight = 0;
			indentToStartAnimation = 0;
			RaiseExpandStateChanged();
		}
		int indentToStartAnimation = 0;
		protected internal void CreateImageFromHeaderControl() {
			if(Element.HeaderControl == null) return;
			CheckHeaderControlBounds();
			Element.HeaderControlImage = AccordionControlPainter.GetControlImage(Element.HeaderControl);
		}
		protected void RaiseItemContentContainerHidden() {
			if(Element.AccordionControl == null) return;
			Element.AccordionControl.RaiseItemContentContainerHidden(new ItemContentContainerHiddenEventArgs(Element));
		}
		protected internal SkinElementInfo GetExpandCollapseButtonInfo() {
			SkinElement elem = GetExpandCollapseSkinElement();
			SkinElementInfo res = new SkinElementInfo(elem, ExpandCollapseButtonBounds);
			if(ControlInfo.AccordionControl.IsRightToLeft) res.RightToLeft = true;
			if(!IsRootElement && !IsAccordionGroupSkinElement) res.ImageIndex = Expanded ? 1 : 0;
			else {
				if(State == ObjectState.Hot && ControlInfo.HoverInfo.HitTest == AccordionControlHitTest.Button) res.ImageIndex = 1;
				if(State == ObjectState.Pressed && ControlInfo.HoverInfo.HitTest == AccordionControlHitTest.Button) res.ImageIndex = 2;
			}
			res.State = State;
			return res;
		}
		protected SkinElement GetExpandCollapseSkinElement() {
			if(Element.Style == ElementStyle.Item && !Element.HasContentContainer) return null;
			SkinElement elem = Expanded ? GetExpandedSkinElement() : GetCollapsedSkinElement();
			return elem;
		}
		protected SkinElement GetCollapsedSkinElement() {
			if(IsRootElement) {
				SkinElement elem = AccordionControlSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinRootGroupOpenButton];
				if(elem != null) return elem;
				return NavBarSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[NavBarSkins.SkinGroupOpenButton];
			}
			SkinElement skin = AccordionControlSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinGroupOpenButton];
			if(skin != null) return skin;
			return GridSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinPlusMinus];
		}
		protected bool IsAccordionGroupSkinElement {
			get {
				SkinElement skin = AccordionControlSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinGroupOpenButton];
				return skin != null;
			}
		}
		protected SkinElement GetExpandedSkinElement() {
			if(IsRootElement) {
				SkinElement elem = AccordionControlSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinRootGroupCloseButton];
				if(elem != null) return elem;
				return NavBarSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[NavBarSkins.SkinGroupCloseButton];
			}
			SkinElement skin = AccordionControlSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinGroupCloseButton];
			if(skin != null) return skin;
			return GridSkins.GetSkin(ControlInfo.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinPlusMinus];
		}
	}
	public class AccordionItemViewInfo : AccordionElementBaseViewInfo {
		public AccordionItemViewInfo(AccordionControlElement item, AccordionGroupViewInfo parentElementInfo)
			: base(item, parentElementInfo.ViewInfo) {
			ParentElementInfo = parentElementInfo;
		}
		public AccordionGroupViewInfo ParentElementInfo { get; private set; }
		protected override int GetItemHeightCore() {
			if(ControlInfo.IsInAnimation) return HeaderBounds.Height;
			if(!Element.IsHeightUpdated)
				return ControlInfo.AccordionControl.ItemHeight;
			return Element.Height;
		}
		protected override ContextItemCollection ContextItemsCore {
			get { return ControlInfo.AccordionControl.ItemContextButtons; }
		}
		protected internal override void CalcItemsIndent() {
			int indent = Expanded ? -(Element.ContentContainer.Height - InnerContentHeight) : InnerContentHeight;
			indent += ContentTopIndent;
			int indexInParent = ParentElementInfo.ElementsInfo.IndexOf(this);
			ParentElementInfo.SetIndentForItems(indent, indexInParent + 1);
			ControlInfo.AccordionControl.Invalidate(true);
		}
		protected internal override int CalcHeaderX() {
			if(ParentElementInfo == null || ParentElementInfo.ParentGroup == null) return 0;
			return ParentElementInfo.CalcHeaderX() + ControlInfo.GetChildItemOffset();
		}
	}
	public class AccordionGroupViewInfo : AccordionElementBaseViewInfo {
		public AccordionGroupViewInfo(AccordionControlElement group, AccordionControlViewInfo viewInfo)
			: base(group, viewInfo) {
			ViewInfo = viewInfo;
			this.hasSeparator = false;
		}
		protected override int GetItemHeightCore() {
			if(ControlInfo.IsInAnimation) return HeaderBounds.Height;
			if(!Element.IsHeightUpdated)
				return ControlInfo.AccordionControl.GroupHeight;
			return Element.Height;
		}
		protected override ContextItemCollection ContextItemsCore {
			get {
				if(Element.Style == ElementStyle.Item)
					return ControlInfo.AccordionControl.ItemContextButtons;
				return ControlInfo.AccordionControl.GroupContextButtons;
			}
		}
		public AccordionControlViewInfo ViewInfo { get; private set; }
		Dictionary<AccordionControlElement, AccordionElementBaseViewInfo> CachedItems { get; set; }
		public List<AccordionElementBaseViewInfo> ElementsInfo { get; set; }
		protected internal virtual void CreateElements() {
			CacheItems();
			ElementsInfo = new List<AccordionElementBaseViewInfo>();
			foreach(AccordionControlElement item in Element.Elements) {
				AccordionElementBaseViewInfo itemInfo = CreateItemInfo(item);
				ElementsInfo.Add(itemInfo);
				if(itemInfo is AccordionGroupViewInfo) {
					((AccordionGroupViewInfo)itemInfo).ParentGroup = this;
					((AccordionGroupViewInfo)itemInfo).CreateElements();
				}
				itemInfo.SubscribeOnElementEvents();
			}
		}
		protected internal override void ClearAppearances() {
			base.ClearAppearances();
			if(ElementsInfo == null) return;
			foreach(AccordionElementBaseViewInfo item in ElementsInfo) {
				item.ClearAppearances();
			}
		}
		protected virtual AccordionElementBaseViewInfo CreateItemInfo(AccordionControlElement element) {
			if(CachedItems != null && CachedItems.ContainsKey(element))
				return CachedItems[element];
			if(element.Style == ElementStyle.Item) return new AccordionItemViewInfo(element, this);
			return new AccordionGroupViewInfo(element, ControlInfo);
		}
		private void CacheItems() {
			if(ElementsInfo == null) return;
			CachedItems = new Dictionary<AccordionControlElement, AccordionElementBaseViewInfo>();
			foreach(AccordionElementBaseViewInfo item in ElementsInfo) {
				item.UnsubscribeOnElementEvents();
				CachedItems.Add(item.Element, item);
			}
		}
		protected internal override int ContentTopIndent {
			get {
				return base.ContentTopIndent;
			}
			set {
				base.ContentTopIndent = value;
				if(ElementsInfo != null)
				foreach(AccordionElementBaseViewInfo item in ElementsInfo) {
					item.ContentTopIndent = value;
				}
			}
		}
		protected internal Point LayoutGroup(Point current) {
			if(Element.Style == ElementStyle.Item) {
				current = LayoutItem(current);
				return current;
			}
			if(!Element.GetVisible()) return current;
			CalcViewInfo(current, ControlInfo.ContentRect.Width);
			current.Y = RealHeaderBounds.Bottom;
			int maxBottom = InnerContentHeight + current.Y;
			if(ElementsInfo == null || ControlInfo.IsMinimized || ControlInfo.IsInMinimizeAnimation)
				return current;
			foreach(AccordionElementBaseViewInfo itemInfo in ElementsInfo) {
				AccordionGroupViewInfo childGroupInfo = itemInfo as AccordionGroupViewInfo;
				if(childGroupInfo != null) {
					if(childGroupInfo.Element.OwnerElements == null || childGroupInfo.Element.OwnerElements.Element.Expanded || IsInAnimation)
						current = childGroupInfo.LayoutGroup(current);
				}
				else {
					AccordionItemViewInfo itemVi = itemInfo as AccordionItemViewInfo;
					itemVi.ItemAnimationMaxHeight = -1;
					if(Expanded || IsInAnimation) {
						current = itemVi.LayoutItem(current);
					}
				}
			}
			return current;
		}
		protected internal AccordionGroupViewInfo ParentGroup { get; set; }
		protected internal int GetGroupLevel {
			get {
				int level = 0;
				AccordionGroupViewInfo group = this;
				while(group.ParentGroup != null) {
					level++;
					group = group.ParentGroup;
				}
				return level;
			}
		}
		protected internal override bool IsRootElement { get { return ParentGroup == null; } }
		protected internal override void CalcItemsIndent() {
			SetItemsMaxHeight();
			int indent = CalcIndent();
			indent += ContentTopIndent;
			SetIndentForParentItem(indent);
			ControlInfo.AccordionControl.Invalidate();
			lastInnerContentHeight = InnerContentHeight;
		}
		protected int CalcIndent() {
			AccordionElementBaseViewInfo nextItemInfo = ControlInfo.Helper.GetNextItemInfo(this);
			if(nextItemInfo == null) return 0;
			int indent = nextItemInfo.IsRootElement ? ControlInfo.GetDistanceBetweenRootGroups() : 0;
			return -((nextItemInfo.RealHeaderBounds.Y - RealHeaderBounds.Bottom - indent) - InnerContentHeight);
		}
		protected internal void SetIndentForItems(int indent, int startIndex) {
			for(int i = startIndex; i < ElementsInfo.Count; i++) {
				ElementsInfo[i].ContentTopIndent = indent;
				ElementsInfo[i].CalcContextButtons();
			}
			SetIndentForParentItem(indent);
		}
		protected internal void SetIndentForParentItem(int indent) {
			AccordionGroupViewInfo parent = ParentGroup;
			if(parent == null) {
				SetIndentForItems(indent, ControlInfo.ElementsInfo);
				return;
			}
			int indexInParent = parent.ElementsInfo.IndexOf(this);
			ParentGroup.SetIndentForItems(indent, indexInParent + 1);
		}
		protected void SetIndentForItems(int indent, List<AccordionGroupViewInfo> items) {
			int groupIndex = items.IndexOf(this);
			if(items.Count > groupIndex + 1) {
				for(int i = groupIndex + 1; i < items.Count; i++) {
					items[i].ContentTopIndent = indent;
					items[i].CalcContextButtons();
				}
			}
		}
		int lastInnerContentHeight = -1;
		protected internal virtual int CalcGroupExpandedHeight() {
			if(Element.Style == ElementStyle.Item) return ContentContainerHeight;
			int height = 0;
			if(ElementsInfo == null) return 0;
			foreach(AccordionElementBaseViewInfo element in ElementsInfo) {
				if(!element.Element.GetVisible()) continue;
				AccordionItemViewInfo itemInfo = element as AccordionItemViewInfo;
				if(itemInfo != null) {
					height += itemInfo.CalcHeaderHeight();
					height += itemInfo.GetContentContainerHeight();
				}
				else {
					AccordionGroupViewInfo groupInfo = element as AccordionGroupViewInfo;
					if(groupInfo.Expanded) height += groupInfo.CalcGroupExpandedHeight();
					height += groupInfo.CalcHeaderHeight();
				}
			}
			return height;
		}
		protected internal override void OnAnimationCompleteCore(bool shouldRefresh) {
			ResetItemsMaxHeight();
			base.OnAnimationCompleteCore(shouldRefresh);
		}
		protected override int GetContentHeight() {
			if(Element.Style == ElementStyle.Group) return CalcGroupExpandedHeight();
			return base.GetContentHeight();
		}
		protected void SetItemsMaxHeight() {
			int height = RealHeaderBounds.Bottom;
			if(ElementsInfo == null) return;
			foreach(AccordionElementBaseViewInfo itemInfo in ElementsInfo) {
				if(IsInAnimation)
					itemInfo.ItemAnimationMaxHeight = Math.Max(0, InnerContentHeight - height);
				else itemInfo.ItemAnimationMaxHeight = -1;
				height += itemInfo.RealHeaderBounds.Height + itemInfo.GetContentContainerHeight();
			}
		}
		protected void ResetItemsMaxHeight() {
			if(ElementsInfo == null) return;
			foreach(AccordionElementBaseViewInfo itemInfo in ElementsInfo) {
				itemInfo.ItemAnimationMaxHeight = -1;
			}
		}
		protected internal override int CalcHeaderX() {
			if(GetGroupLevel <= 1)
				return 0;
			return ControlInfo.GetChildItemOffset() * (GetGroupLevel - 1);
		}
		bool hasSeparator;
		internal bool HasSeparator {
			get { return hasSeparator; }
			set { hasSeparator = value; }
		}
	}
	public class AccordionControlViewInfo : BaseStyleControlViewInfo, ISupportXtraAnimation {
		AccordionControlScrollers scrollers;
		ObjectState expandButtonState;
		Rectangle expandButtonRect;
		AccordionDefaultAppearances defaultAppearances;
		public AccordionControlViewInfo(AccordionControl owner)
			: base(owner) {
			this.expandButtonState = ObjectState.Normal;
			ResetElementsToExpandCollapse();
			IsInAnimation = IsInMinimizeAnimation = false;
			ContentTopIndent = ContentBottom = AnimationIndentIncrement = AnimationAddedTop = 0;
			DropTargets = new AccordionDropTargetArgs();
			ForceExpanding = false;
			this.expandButtonRect = Rectangle.Empty;
			this.defaultAppearances = new AccordionDefaultAppearances(this);
		}
		protected internal AccordionControlScrollers Scrollers {
			get {
				if(scrollers == null)
					scrollers = new AccordionControlScrollers(this);
				return scrollers;
			}
		}
		protected internal bool ShouldDrawClearControl = false;
		public void UpdateScrollBars() {
			Scrollers.DisplayScrollBars();
		}
		protected internal bool UseOffice2003Style {
			get {
				ActiveLookAndFeelStyle style = LookAndFeel.ActiveLookAndFeel.ActiveStyle;
				return style == ActiveLookAndFeelStyle.Office2003 || style == ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		protected internal bool UseFlatStyle {
			get {
				ActiveLookAndFeelStyle style = LookAndFeel.ActiveLookAndFeel.ActiveStyle;
				return style == ActiveLookAndFeelStyle.Flat || style == ActiveLookAndFeelStyle.Style3D || style == ActiveLookAndFeelStyle.UltraFlat;
			}
		}
		protected internal bool ShouldOptimizeAnimation {
			get {
				return AccordionControl.ShouldOptimizeAnimation && IsInAnimation && !UseOffice2003Style && !UseFlatStyle;
			}
		}
		protected internal bool IsTouchScrollBarMode {
			get {
				if(AccordionControl.IsDesignMode || AccordionControl.ScrollBarMode == ScrollBarMode.Hidden) return false;
				if(WindowsFormsSettings.ScrollUIMode == XtraEditors.ScrollUIMode.Touch || AccordionControl.ScrollBarMode == ScrollBarMode.Touch)
					return true;
				if(AccordionControl.ScrollBarMode != ScrollBarMode.Default || WindowsFormsSettings.ScrollUIMode == XtraEditors.ScrollUIMode.Desktop)
					return false;
				return LookAndFeel.ActiveLookAndFeel.GetTouchUI();
			}
		}
		protected internal Color GetBackColor() {
			if(AccordionControl.Appearance.AccordionControl.Options.UseBackColor)
				return AccordionControl.Appearance.AccordionControl.BackColor;
			if(UseOffice2003Style) {
				return Office2003PaintHelper.BackColor;
			}
			if(UseFlatStyle) {
				return FlatPaintHelper.BackColor;
			}
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinBackground];
			if(elem != null) return elem.Color.GetBackColor();
			elem = NavBarSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavBarSkins.SkinBackground];
			return elem.Color.GetBackColor();
		}
		protected internal Color GetForeColor() {
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinItem];
			if(elem != null) return elem.Color.GetForeColor();
			return NavPaneSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinItem].Color.GetForeColor();
		}
		protected override void CalcClientRect(Rectangle bounds) {
			base.CalcClientRect(bounds);
			CreateElementInfos();
			CalcExpandButtonBounds();
			LayoutGroups();
		}
		internal void InvertAccordionExpanded() {
			CheckExpandedWidth();
			XtraAnimator.Current.AddAnimation(new AccordionControlExpandAnimationInfo(this, this, IsMinimized, 100));
		}
		private void CheckExpandedWidth() {
			if(AccordionControl.OptionsMinimizing.NormalWidth == -1 && AccordionControl.OptionsMinimizing.State == AccordionControlState.Normal)
				defaultExpandedWidth = AccordionControl.Width;
		}
		int defaultExpandedWidth = 0;
		internal int GetExpandedWidth() {
			if(AccordionControl.OptionsMinimizing.NormalWidth == -1)
				return defaultExpandedWidth;
			return AccordionControl.OptionsMinimizing.NormalWidth;
		}
		public override Rectangle ContentRect {
			get {
				if(!AccordionControl.ShouldShowFilterControl()) return ClientRect;
				int searchControlHeight = AccordionControl.GetFilterControl().Bounds.Height;
				return new Rectangle(ClientRect.X, ClientRect.Y + searchControlHeight, ClientRect.Width, ClientRect.Height - searchControlHeight);
			}
		}
		protected internal void ResetPressedInfo() {
			PressedInfo = AccordionControlHitInfo.Empty;
		}
		protected internal void ResetHoverInfo() {
			HoverInfo = AccordionControlHitInfo.Empty;
		}
		AccordionControlHitInfo hoverInfo = AccordionControlHitInfo.Empty;
		public AccordionControlHitInfo HoverInfo {
			get { return hoverInfo; }
			set {
				if(HoverInfo.Equals(value))
					return;
				AccordionControlHitInfo prev = HoverInfo;
				hoverInfo = value;
				OnHoverInfoChanged(prev, HoverInfo);
			}
		}
		protected internal ObjectState ExpandButtonState {
			get { return expandButtonState; }
			set {
				if(ExpandButtonState == value)
					return;
				expandButtonState = value;
				if(AccordionControl.OptionsMinimizing.AllowMinimizing) {
					AccordionControl.Invalidate();
					UpdateExpandButton();
				}
			}
		}
		protected internal Rectangle ExpandButtonRect { get { return expandButtonRect; } }
		private static int HoverAnimationLength = 100;
		private void OnHoverInfoChanged(AccordionControlHitInfo prev, AccordionControlHitInfo current) {
			if(prev.ItemInfo != null && prev.ItemInfo.State != ObjectState.Disabled) {
				if(prev.ItemInfo.ContextButtonsHandler.ViewInfo != null) {
					prev.ItemInfo.ContextButtonsHandler.OnMouseLeave(EventArgs.Empty);
				}
				prev.ItemInfo.ClearPaintAppearance();
				AddElementStateChangingAnimation(prev, ObjectState.Hot);
			}
			if(current.ItemInfo != null && current.ItemInfo.State != ObjectState.Disabled) {
				current.ItemInfo.ClearPaintAppearance();
				AddElementStateChangingAnimation(current, ObjectState.Normal);
			}
		}
		protected void AddElementStateChangingAnimation(AccordionControlHitInfo hitInfo, ObjectState state) {
			XtraAnimator.Current.Animations.Remove(this, hitInfo.ItemInfo);
			ObjectPaintInfo foreInfo = GetForeInfo(hitInfo.ItemInfo.RealHeaderBounds, state);
			Bitmap bmp = XtraAnimator.Current.CreateBitmap(GetBackInfo(hitInfo.ItemInfo.RealHeaderBounds, state), foreInfo);
			if(bmp != null) {
				AccordionBitmapAnimationInfo animInfo = new AccordionBitmapAnimationInfo(this, hitInfo.ItemInfo, HoverAnimationLength, foreInfo.Bounds, bmp, new BitmapAnimationImageCallback(OnGetItemBitmap));
				XtraAnimator.Current.AddAnimation(animInfo);
			}
		}
		ObjectPaintInfo GetBackInfo(Rectangle animatedRect, ObjectState state) {
			return new ObjectPaintInfo(new AccordionControlObjectPainter(this), new ObjectInfoArgs(null, animatedRect, state));
		}
		ObjectPaintInfo GetForeInfo(Rectangle animatedRect, ObjectState state) {
			return new ObjectPaintInfo(new AccordionControlObjectPainter(this), new ObjectInfoArgs(null, animatedRect, state));
		}
		Bitmap OnGetItemBitmap(BaseAnimationInfo info) {
			AccordionElementBaseViewInfo itemInfo = info.AnimationId as AccordionElementBaseViewInfo;
			if(itemInfo == null) return null;
			return XtraAnimator.Current.CreateBitmap(GetForeInfo(itemInfo.RealHeaderBounds, itemInfo.State));
		}
		AccordionControlHitInfo pressedInfo = AccordionControlHitInfo.Empty;
		public AccordionControlHitInfo PressedInfo {
			get { return pressedInfo; }
			set {
				if(PressedInfo.Equals(value))
					return;
				AccordionControlHitInfo prev = PressedInfo;
				pressedInfo = value;
				OnPressedInfoChanged(prev, PressedInfo);
			}
		}
		protected void OnPressedInfoChanged(AccordionControlHitInfo prev, AccordionControlHitInfo current) {
			if(prev.ItemInfo != null && prev.ItemInfo.State != ObjectState.Disabled) {
				prev.ItemInfo.ClearPaintAppearance();
				AddElementStateChangingAnimation(prev, ObjectState.Pressed);
			}
			if(current.ItemInfo != null && current.ItemInfo.State != ObjectState.Disabled) {
				current.ItemInfo.ClearPaintAppearance();
				AddElementStateChangingAnimation(current, ObjectState.Hot);
			}
		}
		protected internal void ClearPaintAppearance() {
			DefaultAppearances.Update();
			foreach(AccordionGroupViewInfo group in ElementsInfo) {
				group.ClearAppearances();
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(AccordionControl.GetFilterControl() != null)
				AccordionControl.GetFilterControl().UpdateAppearance();
		}
		protected internal virtual void LayoutGroups() {
			bool isGraphicsAdded = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				isGraphicsAdded = true;
			}
			try {
				LayoutGroupsCore();
			}
			finally {
				if(isGraphicsAdded) GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual void LayoutGroupsCore() {
			Point current = GetTopLeftPoint();
			bool prevElementHasText = false;
			bool isFirstVisibleElement = true;
			foreach(AccordionGroupViewInfo groupInfo in ElementsInfo) {
				current = groupInfo.LayoutGroup(current);
				if(groupInfo.Element.GetVisible()) {
					if(IsMinimized) {
						bool currentElementHasText = (groupInfo.TextBounds.Size != Size.Empty);
						current.Y = CheckElementTop(groupInfo, current.Y, currentElementHasText, prevElementHasText, isFirstVisibleElement);
						prevElementHasText = currentElementHasText;
					}
					isFirstVisibleElement = false;
					current.Y += GetDistanceBetweenRootGroups();
				}
			}
			current.Y -= GetDistanceBetweenRootGroups();
			ContentBottom = current.Y - ContentRect.Y;
			if(!IsInAnimation) UpdateScrollBars();
		}
		int CheckElementTop(AccordionGroupViewInfo groupInfo, int top, bool currentElementHasText, bool prevElementHasText, bool isFirstVisibleElement) {
			if(!currentElementHasText && !prevElementHasText && IsMinimized) {
				top -= GetSeparatorHeight();
				groupInfo.HasSeparator = false;
			}
			else groupInfo.HasSeparator = !isFirstVisibleElement;
			return top;
		}
		protected internal virtual int CalcCollapsedBestWidth() {
			bool isGraphicsAdded = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				isGraphicsAdded = true;
			}
			try {
				int contentBestWidth = 0;
				foreach(AccordionControlElement element in AccordionControl.Elements) {
					int elementBestWidth = 0;
					Image image = element.GetOriginalImage();
					if(element.ImageLayoutMode == ImageLayoutMode.OriginalSize && image != null)
						elementBestWidth = image.Width;
					else elementBestWidth = CalcStringSize(element, "Wg", 0).Height;
					contentBestWidth = Math.Max(contentBestWidth, elementBestWidth);
				}
				SkinElement elem = GetMinimizedSkinElement();
				if(elem == null) return contentBestWidth;
				int paddingHorizontal = elem.ContentMargins.ToPadding().Horizontal;
				return paddingHorizontal + contentBestWidth;
			}
			finally {
				if(isGraphicsAdded) GInfo.ReleaseGraphics();
			}
		}
		protected internal SkinElement GetMinimizedSkinElement() {
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinMinimizedElement];
			if(elem != null) return elem;
			return AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinItem];
		}
		protected internal virtual void CalcExpandButtonBounds() {
			SkinElement elem = GetExpandButtonSkinElement();
			if(elem == null)
				return;
			SkinElementInfo elemInfo = new SkinElementInfo(elem, Rectangle.Empty);
			int expandButtonWidth = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, AccordionSkinElementPainter.Default, elemInfo).Width;
			Rectangle elementsRect = CalcElementsRect();
			int expandButtonLeft = IsRightExpandButton() ? elementsRect.Right - expandButtonWidth : elementsRect.X;
			this.expandButtonRect = new Rectangle(expandButtonLeft, elementsRect.Y, expandButtonWidth, elementsRect.Height);
		}
		protected Rectangle CalcElementsRect() {
			Rectangle rect = ContentRect;
			if(!ShouldKeepContentForScrollBar())
				return rect;
			int scrollWidth = Scrollers.VScrollBar.Bounds.Width;
			rect.Width -= scrollWidth;
			if(AccordionControl.IsRightToLeft)
				rect.X += scrollWidth;
			return rect;
		}
		protected internal virtual Size CalcArrowSize() {
			SkinElementInfo arrowInfo = new SkinElementInfo(GetExpandArrowSkinElement(), Rectangle.Empty);
			Size arrowSize = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, arrowInfo).Size;
			return new Size(arrowSize.Height, arrowSize.Width);
		}
		protected internal SkinElement GetExpandArrowSkinElement() {
			return RibbonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[RibbonSkins.SkinButtonArrow];
		}
		protected internal virtual bool ShouldRotateExpandButton() {
			return IsRightExpandButton() ^ IsMinimized;
		}
		internal bool ShouldKeepContentForScrollBar() {
			if(IsMinimized) return false;
			ScrollBarMode scrollMode = AccordionControl.ScrollBarMode;
			if(scrollMode == ScrollBarMode.AutoCollapse)
				return Scrollers.VScrollVisible;
			return scrollMode != ScrollBarMode.Hidden && !IsTouchScrollBarMode;
		}
		bool IsRightExpandButton() {
			bool isDockRight = (AccordionControl.Dock == System.Windows.Forms.DockStyle.Right);
			bool invertButton = (AccordionControl.OptionsMinimizing.MinimizeButtonMode == MinimizeButtonMode.Inverted);
			return isDockRight ^ invertButton;
		}
		protected internal SkinElement GetExpandButtonSkinElement() {
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinExpandButton];
			if(elem == null) {
				Skin defSkin = GetDefaultSkin();
				return defSkin[AccordionControlSkins.SkinExpandButton];
			}
			return elem;
		}
		protected Skin GetDefaultSkin() {
			bool isDarkSkin = FrameHelper.IsDarkSkin(LookAndFeel.ActiveLookAndFeel);
			Skin defSkin = GetDefaultSkinCore(isDarkSkin);
			return defSkin;
		}
		protected Skin GetDefaultSkinCore(bool isDarkSkin) {
			if(isDarkSkin)
				return SkinManager.Default.GetSkin(SkinProductId.AccordionControl, "DevExpress Dark Style");
			return SkinManager.Default.GetSkin(SkinProductId.AccordionControl, "DevExpress Style");
		}
		protected internal SkinElement GetSeparatorSkinElement() {
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinSeparator];
			if(elem != null) return elem;
			return CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabelLine];
		}
		protected internal int GetSeparatorHeight() {
			SkinElement elem = GetSeparatorSkinElement();
			return elem.Size.MinSize.Height + GetSeparatorPadding(elem).Height;
		}
		protected internal Rectangle GetSeparatorRect(AccordionElementBaseViewInfo elementInfo) {
			SkinElement elem = GetSeparatorSkinElement();
			SkinPaddingEdges padding = GetSeparatorPadding(elem);
			int width = elementInfo.HeaderBounds.Width - padding.Left - padding.Right;
			if(width <= 0)
				return Rectangle.Empty;
			int height = elem.Size.MinSize.Height;
			return new Rectangle(elementInfo.HeaderBounds.X + padding.Left, elementInfo.HeaderBounds.Y - height - padding.Bottom, width, height);
		}
		protected SkinPaddingEdges GetSeparatorPadding(SkinElement elem) {
			return elem.Properties.GetPadding("Padding", new SkinPaddingEdges());
		}
		protected internal int GetDistanceBetweenRootGroups() {
			if(IsMinimized)
				return GetSeparatorHeight();
			if(AccordionControl.DistanceBetweenRootGroups != -1)
				return AccordionControl.DistanceBetweenRootGroups;
			Skin skin = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel);
			return skin.Properties.GetInteger("DistanceBetweenRootGroups", 0);
		}
		protected internal int GetChildItemOffset() {
			Skin skin = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel);
			return skin.Properties.GetInteger("ChildItemOffset", 0);
		}
		protected internal System.Windows.Forms.Padding GetContentContainerPadding() {
			Skin skin = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel);
			SkinPaddingEdges padding = skin.Properties.GetPadding("ContentContainerPadding", new SkinPaddingEdges());
			return padding.ToPadding();
		}
		protected Point GetTopLeftPoint() {
			Point location = ContentRect.Location;
			location.Y += ContentTopIndent;
			return location;
		}
		protected internal AccordionDropTargetArgs DropTargets { get; set; }
		protected internal double GestureInertia { get; set; }
		protected internal int AddedAnimationHeight { get; set; }
		protected internal int AnimationAddedTop { get; set; }
		protected internal bool IsInAnimation { get; set; }
		protected internal bool IsInMinimizeAnimation { get; set; }
		protected internal int ContentTopIndent { get; set; }
		protected internal int AnimationIndentIncrement { get; set; }
		protected internal int ContentBottom { get; set; }
		protected internal void UpdateContentTop(int newContentTop) {
			int maxContentTop = ContentTopIndent - ContentBottom + ContentRect.Height;
			ContentTopIndent = Math.Min(0, Math.Max(newContentTop, maxContentTop));
			AccordionControl.ForceLayoutChanged();
		}
		protected internal void CheckContentTop() {
			UpdateContentTop(ContentTopIndent);
		}
		public AccordionControl AccordionControl { get { return (AccordionControl)Owner; } }
		List<AccordionGroupViewInfo> elementsInfo;
		public List<AccordionGroupViewInfo> ElementsInfo {
			get {
				if(elementsInfo == null)
					elementsInfo = new List<AccordionGroupViewInfo>();
				return elementsInfo;
			}
		}
		protected internal void CreateElementInfos() {
			CachePrevElements();
			this.elementsInfo.Clear();
			foreach(AccordionControlElement element in AccordionControl.Elements) {
				CreateRootElementInfo(element);
			}
		}
		protected void CreateRootElementInfo(AccordionControlElement element) {
			AccordionGroupViewInfo elementInfo = CreateGroupInfo(element);
			ElementsInfo.Add(elementInfo);
			elementInfo.SubscribeOnElementEvents();
			if(elementInfo.Element.Style == ElementStyle.Item) return;
			elementInfo.CreateElements();
		}
		protected AccordionGroupViewInfo CreateGroupInfo(AccordionControlElement group) {
			if(CachedElements != null && CachedElements.ContainsKey(group))
				return CachedElements[group];
			return new AccordionGroupViewInfo(group, this);
		}
		Dictionary<AccordionControlElement, AccordionGroupViewInfo> CachedElements { get; set; }
		protected void CachePrevElements() {
			if(ElementsInfo == null)
				return;
			CachedElements = new Dictionary<AccordionControlElement, AccordionGroupViewInfo>();
			foreach(AccordionGroupViewInfo group in ElementsInfo) {
				group.UnsubscribeOnElementEvents();
				CachedElements.Add(group.Element, group);
			}
		}
		public void ClearCache() {
			AccordionControlHelper.ForEachElementInfo(elemInfo => { elemInfo.UnsubscribeOnElementEvents(); }, ElementsInfo);
			if(CachedElements != null) CachedElements.Clear();
			if(ElementsInfo != null) ElementsInfo.Clear();
		}
		public virtual AccordionControlHitInfo CalcHitInfo(Point point) {
			AccordionControlHitInfo res = new AccordionControlHitInfo() { HitPoint = point };
			CalcHitInfo(res);
			return res;
		}
		protected void CalcHitInfo(AccordionControlHitInfo res) {
			if(AccordionControl.OptionsMinimizing.AllowMinimizing && ExpandButtonRect.Contains(res.HitPoint)) {
				res.HitTest = AccordionControlHitTest.ExpandButton;
				return;
			}
			if(Scrollers.VScrollVisible && Scrollers.VScrollBar.Bounds.Contains(res.HitPoint)) {
				res.HitTest = AccordionControlHitTest.ScrollBar;
				return;
			}
			foreach(AccordionGroupViewInfo group in ElementsInfo) {
				if(group.Element.GetVisible() && group.RealHeaderBounds.Contains(res.HitPoint)) {
					res.ItemInfo = group;
					if(group.ExpandCollapseButtonBounds.Contains(res.HitPoint)) {
						res.HitTest = AccordionControlHitTest.Button;
						return;
					}
					res.HitTest = AccordionControlHitTest.Group;
					return;
				}
				if(group.Element.Style == ElementStyle.Item) continue;
				if(!IsMinimized && CalcHitInfoInGroup(res, group)) return;
			}
		}
		protected bool CalcHitInfoInGroup(AccordionControlHitInfo res, AccordionGroupViewInfo group) {
			if(group.Expanded) {
				foreach(AccordionElementBaseViewInfo item in group.ElementsInfo) {
					if(item.Element.GetVisible() && item.RealHeaderBounds.Contains(res.HitPoint)) {
						res.ItemInfo = item;
						AccordionGroupViewInfo groupInfo = item as AccordionGroupViewInfo;
						if(groupInfo != null) {
							if(groupInfo.ExpandCollapseButtonBounds.Contains(res.HitPoint)) {
								res.HitTest = AccordionControlHitTest.Button;
								return true;
							}
							res.HitTest = AccordionControlHitTest.Group;
						}
						else res.HitTest = AccordionControlHitTest.Item;
						return true;
					}
					if(item is AccordionGroupViewInfo) {
						if(CalcHitInfoInGroup(res, item as AccordionGroupViewInfo)) return true;
					}
				}
			}
			return false;
		}
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		System.Windows.Forms.Control ISupportXtraAnimation.OwnerControl { get { return AccordionControl; } }
		AccordionControlHelper helper;
		protected internal AccordionControlHelper Helper {
			get {
				if(helper == null)
					helper = new AccordionControlHelper(this);
				return helper;
			}
		}
		protected internal void ShowVisibleControls() {
			Helper.ShowVisibleControls();
		}
		protected internal void HideControls() {
			ShouldDrawClearControl = true;
			bool lockDrawing = false;
			if(AccordionControl.IsHandleCreated) {
				DrawingLocker.LockDrawing(AccordionControl.Handle);
				lockDrawing = true;
			}
			try {
				Helper.HideControls();
			}
			finally {
				ShouldDrawClearControl = false;
				if(lockDrawing) DrawingLocker.UnlockDrawing(AccordionControl.Handle);
			}
		}
		protected internal bool ForceExpanding { get; set; }
		protected internal AccordionElementBaseViewInfo CollapsingElement { get; set; }
		protected internal ExpandCollapseElementList ElementsToExpandCollapse { get; set; }
		public void ResetElementsToExpandCollapse() {
			ElementsToExpandCollapse = new ExpandCollapseElementList();
		}
		public bool IsMinimized {
			get {
				return AccordionControl.OptionsMinimizing.AllowMinimizing && AccordionControl.OptionsMinimizing.State == AccordionControlState.Minimized;
			}
		}
		protected Size CalcStringSizeCore(AppearanceObject appearance, string text, int textMaxWidth) {
			if(AccordionControl.AllowHtmlText) {
				return StringPainter.Default.Calculate(GInfo.Graphics, appearance, text, textMaxWidth).Bounds.Size;
			}
			return appearance.CalcTextSize(GInfo.Cache, text, textMaxWidth).ToSize();
		}
		static ObjectState[] objectStateValues = (ObjectState[])Enum.GetValues(typeof(ObjectState));
		protected internal virtual Size CalcStringSize(AccordionControlElement elem, string text, int textMaxWidth) {
			int bestWidth = 0, bestHeight = 0;
			foreach(ObjectState state in objectStateValues) {
				AppearanceObject obj = CreateElementPaintAppearance(elem, state);
				if(!obj.Options.UseFont && state != ObjectState.Normal)
					continue;
				Size size = CalcStringSizeCore(obj, text, textMaxWidth);
				bestWidth = Math.Max(bestWidth, size.Width);
				bestHeight = Math.Max(bestHeight, size.Height);
			}
			if(IsMinimized)
				return new Size(bestHeight, bestWidth + 1);
			return new Size(bestWidth, bestHeight);
		}
		public virtual AppearanceObject CreateElementPaintAppearance(AccordionControlElement elem, ObjectState state) {
			AppearanceObject obj = new AppearanceObject(GetAppearanceByState(elem, state), DefaultAppearances.GetAppearance(elem, state));
			if(UseOffice2003Style || UseFlatStyle) obj.FontStyleDelta = FontStyle.Bold;
			if(obj.TextOptions.WordWrap == WordWrap.Default) obj.TextOptions.WordWrap = WordWrap.Wrap;
			if(AccordionControl.IsRightToLeft) obj.TextOptions.RightToLeft = true;
			return obj;
		}
		AccordionDefaultAppearances DefaultAppearances { get { return defaultAppearances; } }
		protected virtual AppearanceObject GetAppearanceByState(AccordionControlElement elem, ObjectState state) {
			AppearanceObject res = new AppearanceObject();
			AppearanceObject[] combine = new AppearanceObject[] { GetAppearance(elem, state), GetElementBaseAppearance(elem, state) };
			AppearanceHelper.Combine(res, combine);
			return res;
		}
		protected AppearanceObject GetElementBaseAppearance(AccordionControlElement elem, ObjectState state) {
			if(elem.Style == ElementStyle.Group)
				return GetGroupBaseAppearance(state);
			return GetItemBaseAppearance(elem, state);
		}
		protected internal AppearanceObject GetAppearance(AccordionControlElement elem, ObjectState state) {
			if(state == ObjectState.Disabled) {
				return elem.Appearance.Disabled;
			}
			if(state == ObjectState.Hot) {
				return elem.Appearance.Hovered;
			}
			if(state == ObjectState.Pressed) {
				return elem.Appearance.Pressed;
			}
			return elem.Appearance.Normal;
		}
		protected AppearanceObject GetItemBaseAppearance(AccordionControlElement elem, ObjectState state) {
			if(state == ObjectState.Disabled) {
				if(elem.HasContentContainer) return AccordionControl.Appearance.ItemWithContainer.Disabled;
				return AccordionControl.Appearance.Item.Disabled;
			}
			if(state == ObjectState.Hot) {
				if(elem.HasContentContainer) return AccordionControl.Appearance.ItemWithContainer.Hovered;
				return AccordionControl.Appearance.Item.Hovered;
			}
			if(state == ObjectState.Pressed) {
				if(elem.HasContentContainer) return AccordionControl.Appearance.ItemWithContainer.Pressed;
				return AccordionControl.Appearance.Item.Pressed;
			}
			if(elem.HasContentContainer) return AccordionControl.Appearance.ItemWithContainer.Normal;
			return AccordionControl.Appearance.Item.Normal;
		}
		protected AppearanceObject GetGroupBaseAppearance(ObjectState state) {
			if(state == ObjectState.Disabled) {
				return AccordionControl.Appearance.Group.Disabled;
			}
			if(state == ObjectState.Hot) {
				return AccordionControl.Appearance.Group.Hovered;
			}
			if(state == ObjectState.Pressed) {
				return AccordionControl.Appearance.Group.Pressed;
			}
			return AccordionControl.Appearance.Group.Normal;
		}
		protected internal virtual SkinElement GetHeaderSkinElement(ElementStyle elementStyle, bool isRootElement) {
			if(IsMinimized) {
				SkinElement elem = GetMinimizedSkinElement();
				if(elem != null) return elem;
			}
			if(isRootElement) {
				SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinRootGroup];
				if(elem != null) return elem;
				return NavBarSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavBarSkins.SkinGroupHeader];
			}
			if(elementStyle == ElementStyle.Item) {
				SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinItem];
				if(elem != null) return elem;
				return NavPaneSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinItem];
			}
			SkinElement skin = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinGroup];
			if(skin != null) return skin;
			return NavPaneSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinItem];
		}
		AccordionExpandButtonWindow expandButton;
		protected internal void ShowExpandButton() {
			if(ExpandButtonRect.Size == Size.Empty)
				return;
			if(this.expandButton == null)
				this.expandButton = new AccordionExpandButtonWindow(AccordionControl);
			Point loc = AccordionControl.PointToScreen(ExpandButtonRect.Location);
			this.expandButton.Show(loc, AccordionControlPainter.CreateExpandButtonBitmap(this));
		}
		protected internal void UpdateExpandButton() {
			if(this.expandButton == null) return;
			this.expandButton.NewImage = AccordionControlPainter.CreateExpandButtonBitmap(this);
		}
		protected internal void DisposeExpandButton() {
			if(this.expandButton == null) return;
			this.expandButton.Dispose();
			this.expandButton = null;
		}
		protected internal virtual void OnMinimizedElementClick(AccordionControlElement elem) {
			if(elem.Elements.Count == 0) {
				ResetPressedInfo();
				ResetHoverInfo();
				return;
			}
			AccordionControlForm form = CreatePopupForm(elem);
			ShowPopupForm(form, elem);
		}
		protected internal virtual void ShowPopupForm(AccordionControlForm form, AccordionControlElement elem) {
			Point loc = AccordionControl.PointToScreen(new Point(elem.Bounds.Right, elem.Bounds.Top));
			Rectangle workingArea = Screen.GetWorkingArea(loc);
			if(loc.Y + form.Height > workingArea.Bottom)
				loc.Y = workingArea.Bottom - form.Height;
			if(ShouldShowPopupLeft) {
				if(loc.X - elem.Bounds.Width - form.Width > workingArea.Left)
					loc.X -= elem.Bounds.Width + form.Width;
			}
			else {
				if(loc.X + form.Width > workingArea.Right)
					loc.X -= elem.Bounds.Width + form.Width;
			}
			form.Show(loc);
		}
		protected internal virtual AccordionControlForm CreatePopupForm(AccordionControlElement elem) {
		   return new AccordionControlForm(AccordionControl, elem.Elements, false);
		}
		protected internal bool ShouldShowPopupLeft {
			get { return AccordionControl.Dock == DockStyle.Right ^ AccordionControl.IsRightToLeft; }
		}
	}
}
