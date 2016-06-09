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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Services;
namespace DevExpress.XtraMap.Drawing {
	public abstract class LegendViewInfoItemBase {
		string text;
		string format;
		double value = double.NaN;
		Rectangle textRect;
		Color color;
		string actualText;
		protected internal string ActualText { get { return actualText; } }
		public string Format {
			get { return format; }
			set {
				if(format != value) {
					format = value;
					UpdateActualText();
				}
			}
		}
		public double Value {
			get { return value; }
			set {
				if(this.value != value) {
					this.value = value;
					UpdateActualText();
				}
			}
		}
		public string Text {
			get { return text; }
			set {
				if(string.Equals(text, value))
					return;
				this.text = value;
				UpdateActualText();
			}
		}
		void UpdateActualText() {
			if(String.IsNullOrEmpty(Text)) {
				String text = !double.IsNaN(value) ? Value.ToString(format, CultureInfo.CurrentCulture) : String.Empty;
				SetTextInternal(text);
			} else
				this.actualText = Text;
		}
		internal void SetTextInternal(string value) {
			this.actualText = value != null ? value : string.Empty;
		}
		public Rectangle TextRect {
			get { return textRect; }
			set {
				if(!Rectangle.Equals(textRect, value))
					textRect = value;
			}
		}
		public Color Color {
			get { return color; }
			set {
				if(!Color.Equals(color, value))
					color = value;
			}
		}
		protected internal string GetActualText() {
			if(!String.IsNullOrEmpty(Text))
				return Text;
			return !double.IsNaN(Value) ? Value.ToString(format, CultureInfo.CurrentCulture) : String.Empty;
		}
	}
	public abstract class LegendViewInfoItem : LegendViewInfoItemBase {
		Rectangle contentRect;
		public Rectangle ContentRect {
			get { return contentRect; }
			set {
				if (!Rectangle.Equals(contentRect, value))
					contentRect = value;
			}
		}
		public abstract void DrawItemContent(GraphicsCache cache, Brush brush, Point offset);
	}
	public class ImageLegendViewInfoItem : LegendViewInfoItem {
		object imageList;
		int imageIndex;
		public object ImageList {
			get { return imageList; }
			set {
				if (imageList == value)
					return;
				imageList = value;
			}
		}
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if (imageIndex == value)
					return;
				imageIndex = value;
			}
		}
		public override void DrawItemContent(GraphicsCache cache, Brush brush, Point offset) {
			ImageCollection.DrawImageListImage(cache, ImageList, ImageIndex, RectUtils.Offset(ContentRect, offset));
		}
	}
	public class ColorLegendViewInfoItem : LegendViewInfoItem {
		Rectangle riskRect;
		public Rectangle RiskRect {
			get { return riskRect; }
			set {
				if(!Rectangle.Equals(riskRect, value))
					riskRect = value;
			}
		}
		public override void DrawItemContent(GraphicsCache cache, Brush brush, Point offset) {
			cache.FillRectangle(cache.GetSolidBrush(Color), RectUtils.Offset(ContentRect, offset));
			cache.FillRectangle(brush, RectUtils.Offset(RiskRect, offset));
		}
	}
	public class SizeLegendViewInfoItem : LegendViewInfoItemBase {
		int markerSize;
		Rectangle markerRect;
		Rectangle tickMarkRect;
		public int MarkerSize {
			get { return markerSize; }
			set {
				if(markerSize != value)
					markerSize = value;
			}
		}
		public Rectangle MarkerRect {
			get { return markerRect; }
			set {
				if(!Rectangle.Equals(markerRect, value))
					markerRect = value;
			}
		}
		public Rectangle TickMarkRect {
			get { return tickMarkRect; }
			set {
				if(!Rectangle.Equals(tickMarkRect, value))
					tickMarkRect = value;
			}
		}
	}
	public abstract class LegendViewInfo : OverlayViewInfoBase, IViewInfoSupportAlignment, ISupportIndexOverlay {
		const int MaxLegendIndex = 1000;
		public static LegendViewInfo CreateViewInfo(MapLegendBase legend, InnerMap map, SizeLegendPainter sizePainter, ColorLegendPainter colorPainter) {
			ColorListLegend listLegend = legend as ColorListLegend;
			if(listLegend != null)
				return new ColorListLegendViewInfo(listLegend, map, colorPainter);
			ColorScaleLegend scaleLegend = legend as ColorScaleLegend;
			if(scaleLegend != null)
				return new ColorScaleLegendViewInfo(scaleLegend, map, colorPainter);
			SizeLegend sizeLegend = legend as SizeLegend;
			if(sizeLegend != null) {
				if(sizeLegend.Type == SizeLegendType.Inline)
					return new InlineSizeLegendViewInfo(sizeLegend, map, sizePainter);
				return new NestedSizeLegendViewInfo(sizeLegend, map, sizePainter);
			}
			return null;
		}
		public const int BoundsOffset = 20;
		public const int ItemPadding = 16;
		public const int MinItemHeight = 16;
		public const int MinItemWidth = ItemPadding * 2;
		readonly MapLegendBase legend;
		int legendIndex;
		LegendViewInfoItemBase[] colorLegendItems;
		IColorizerLegendFormatService formatService;
		public override bool CanStore { get { return true; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultLegendAppearance; } }
		protected override MapStyleCollection UserAppearance { get { return LegendUserAppearance; } }
		protected IColorizerLegendFormatService FormatService { get { return formatService; } }
		protected TextElementStyle ItemTextStyle { get { return ((LegendAppearance)PaintAppearance).ItemStyle; } }
		internal int LegendIndex {
			get { return legendIndex; }
			set {
				if(value > MaxLegendIndex || value < 0)
					throw new ArgumentException("LegendIndex");
				legendIndex = value;
			}
		}
		public LegendAlignment LegendAlignment { get; set; }
		public string LegendRangeStopsFormat { get; set; }
		public string Header { get; set; }
		public string Description { get; set; }
		public LegendAppearance LegendUserAppearance { get; set; }
		public Rectangle HeaderRect { get; protected set; }
		public Rectangle DescriptionRect { get; protected set; }
		public override ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.Legend; } }
		public override bool Printable { get { return true; } }
		public MapLegendBase Legend { get { return legend; } }
		public LegendViewInfoItemBase[] ColorLegendItems {
			get { return colorLegendItems; }
			protected set {
				if(colorLegendItems == value)
					return;
				colorLegendItems = value;
			}
		}
		#region IViewInfoSupportAlignment implementation
		Rectangle IViewInfoSupportAlignment.LayoutRect { get { return Bounds; } set { Bounds = value; } }
		ContentAlignment IViewInfoSupportAlignment.Alignment { get { return (ContentAlignment)Legend.Alignment; } }
		Orientation IViewInfoSupportAlignment.JoiningOrientation { get { return Orientation.Horizontal; } }
		#endregion
		#region ISupportIndexOverlay
		int ISupportIndexOverlay.Index {
			get { return LegendIndex; }
			set { LegendIndex = value; }
		}
		int ISupportIndexOverlay.MaxIndex {
			get { return MaxLegendIndex; }
		}
		#endregion
		protected LegendViewInfo(MapLegendBase legend, InnerMap map, LegendPainter painter)
			: base(map, new LegendAppearance(null), painter) {
			this.legend = legend;
			this.formatService = Map.GetService<IColorizerLegendFormatService>();
		}
		protected override void ValidateOverlay() {
			ValidateOnEmptyItems();
		}
		protected void ValidateOnEmptyItems() {
			if(colorLegendItems == null || colorLegendItems.Length == 0) 
				ResetBounds();
		}
		protected virtual Size CalculateHeaderDescription(Graphics gr, int maxTextWidth, int height) {
			LegendAppearance paintAppearance = (LegendAppearance)PaintAppearance;
			TextElementStyle headerTextStyle = paintAppearance.HeaderStyle;
			TextElementStyle descriptionTextStyle = paintAppearance.DescriptionStyle;
			Size headerTextSize = MapUtils.CalcStringPixelSize(gr, Header, headerTextStyle.Font);
			Size descriptionTextSize = MapUtils.CalcStringPixelSize(gr, Description, descriptionTextStyle.Font);
			maxTextWidth = headerTextSize.Width > descriptionTextSize.Width ? headerTextSize.Width : descriptionTextSize.Width;
			if(string.IsNullOrEmpty(Header))
				HeaderRect = Rectangle.Empty;
			else {
				HeaderRect = new Rectangle(ItemPadding, height, headerTextSize.Width, headerTextSize.Height);
				height += HeaderRect.Height + ItemPadding;
			}
			if(string.IsNullOrEmpty(Description))
				DescriptionRect = Rectangle.Empty;
			else {
				DescriptionRect = new Rectangle(ItemPadding, height - ItemPadding / 2, descriptionTextSize.Width, descriptionTextSize.Height);
				height += DescriptionRect.Height + ItemPadding;
			}
			return new Size(maxTextWidth, height);
		}
		protected virtual void CalculateBounds(Rectangle controlBounds, int height, int width) {
			Bounds = new Rectangle(0, 0, width + ItemPadding, height + 3 * ItemPadding / 2);
			ClientBounds = new Rectangle(GetXOffset((ContentAlignment)legend.Alignment), GetYOffset((ContentAlignment)legend.Alignment), width, height + ItemPadding / 2);
		}
		int GetYOffset(ContentAlignment alignment) {
			if(ContentAlignmentUtils.BottomAligned(alignment))
				return 0;
			if(ContentAlignmentUtils.MiddleAligned(alignment))
				return ItemPadding / 2;
			return ItemPadding;
		}
		int GetXOffset(ContentAlignment alignment) {
			if(ContentAlignmentUtils.RightAligned(alignment))
				return 0;
			if(ContentAlignmentUtils.CenterAligned(alignment))
				return ItemPadding / 2;
			return ItemPadding;
		}
		protected void FormatLegendItemText(MapLegendBase legend, MapLegendItemBase legendItem, LegendViewInfoItemBase viewInfoItem) {
			if(formatService != null)
				viewInfoItem.SetTextInternal(formatService.FormatLegendItem(legend, legendItem));
		}
		protected Size CalculateTextSize(Graphics gr, string text) {
			Size textSize = MapUtils.CalcStringPixelSize(gr, text, ItemTextStyle.Font);
			textSize.Height = Math.Max(textSize.Height, MinItemHeight);
			textSize.Width = Math.Max(textSize.Width, MinItemWidth);
			return textSize;
		}
		protected internal virtual void Initialize(int legendIndex) {
			LegendIndex = legendIndex;
			Header = Legend.Header;
			Description = Legend.Description;
			LegendAlignment = Legend.Alignment;
			LegendUserAppearance = Legend.Appearance;
			LegendRangeStopsFormat = Legend.RangeStopsFormat;
		}
	}
	public class ColorListLegendViewInfo : LegendViewInfo {
		protected override int KeyInternal { get { return 8192; } }
		public new ColorListLegend Legend { get { return (ColorListLegend)base.Legend; } }
		public ColorListLegendViewInfo(ColorListLegend legend, InnerMap map, ColorLegendPainter painter)
			: base(legend, map, painter) {
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			CalculateColorListOverlay(gr, controlBounds);
		}
		internal void CalculateColorListOverlay(Graphics gr, Rectangle controlBounds) {
			int height = ItemPadding;
			int boundsWidth = 0;
			Size headerSize = CalculateHeaderDescription(gr, boundsWidth, height);
			int itemContentWidth = GetContentWidth();
			boundsWidth = headerSize.Width + ItemPadding;
			height = headerSize.Height;
			IList<MapLegendItemBase> legendItems = Legend.GetItems();
			ColorLegendItems = new LegendViewInfoItem[legendItems.Count];
			int count = legendItems.Count;
			for(int i = 0; i < count; i++) {
				int itemIndex = Legend.SortOrder == LegendItemsSortOrder.Descending ? i : (count - i) - 1;
				LegendViewInfoItem item = CreateLegendViewInfoItem(legendItems[itemIndex]);
				Size textSize = CalculateTextSize(gr, item.ActualText);
				Size contentSize = new Size(itemContentWidth, GetContentHeight(item, textSize.Height));
				item.ContentRect = new Rectangle(new Point(ItemPadding, MathUtils.GetRelativeCenter(height, contentSize.Height, textSize.Height)), contentSize);
				item.TextRect = new Rectangle(item.ContentRect.X + item.ContentRect.Width + ItemPadding, MathUtils.GetRelativeCenter(height, textSize.Height, contentSize.Height), textSize.Width, textSize.Height);
				boundsWidth = Math.Max(boundsWidth, itemContentWidth + ItemPadding * 2 + textSize.Width);
				ColorLegendItems[i] = item;
				height += Math.Max(item.TextRect.Height, item.ContentRect.Height) + ItemPadding / 2;
			}
			CalculateBounds(controlBounds, height, boundsWidth + ItemPadding);
		}
		int GetContentHeight(LegendViewInfoItem item, int textHeight) {
			ImageLegendViewInfoItem imageItem = item as ImageLegendViewInfoItem;
			return imageItem != null ? ImageCollection.GetImageListSize(Legend.ImageList).Height : textHeight;
		}
		internal int GetContentWidth() {
			Size size = ImageCollection.GetImageListSize(Legend.ImageList);
			return size.IsEmpty ? MinItemWidth : size.Width;
		}
		LegendViewInfoItem CreateLegendViewInfoItem(MapLegendItemBase legendItem) {
			ColorLegendItem colorItem = legendItem as ColorLegendItem;
			if (colorItem == null || colorItem.ImageIndex == -1 || Legend.ImageList == null)
				return CreateColorViewInfoItem(legendItem);
			Image img = ImageCollection.GetImageListImage(Legend.ImageList, colorItem.ImageIndex);
			if (img == null)
				return CreateColorViewInfoItem(colorItem);
			return CreateImageViewInfoItem(colorItem, Legend.ImageList, colorItem.ImageIndex);
		}
		ImageLegendViewInfoItem CreateImageViewInfoItem(ColorLegendItem legendItem, object imageList, int imageIndex) {
			ImageLegendViewInfoItem item = new ImageLegendViewInfoItem();
			item.Text = legendItem.Text;
			item.Value = legendItem.Value;
			item.Format = LegendRangeStopsFormat;
			item.ImageIndex = imageIndex;
			item.ImageList = imageList;
			return item;
		}
		ColorLegendViewInfoItem CreateColorViewInfoItem(MapLegendItemBase legendItem) {
			ColorLegendViewInfoItem item = new ColorLegendViewInfoItem(); 
			item.Text = legendItem.Text;
			item.Value = legendItem.Value; 
			item.Format = LegendRangeStopsFormat;
			item.Color = legendItem.Color;
			FormatLegendItemText(Legend, legendItem, item);
			return item;
		}
	}
	public class ColorScaleLegendViewInfo : LegendViewInfo {
		const int RiskWidth = 1;
		const int RiskPadding = 5;
		protected override int KeyInternal { get { return 4096; } }
		public new ColorScaleLegend Legend { get { return (ColorScaleLegend)base.Legend; } }
		public ColorScaleLegendViewInfo(ColorScaleLegend legend, InnerMap map, ColorLegendPainter painter)
			: base(legend, map, painter) {
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			controlBounds = CalculateScaleLegendOverlay(gr, controlBounds);
		}
		internal Rectangle CalculateScaleLegendOverlay(Graphics gr, Rectangle controlBounds) {
			int boundsHeight = ItemPadding;
			int boundsWidth = ItemPadding;
			Size headerSize = CalculateHeaderDescription(gr, boundsWidth, boundsHeight);
			boundsWidth = headerSize.Width;
			boundsHeight = headerSize.Height;
			int colorRectWidth = FindMaxItemWidth(gr);
			IList<MapLegendItemBase> legendItems = Legend.GetItems();
			ColorLegendItems = new ColorLegendViewInfoItem[legendItems.Count];
			int itemsHeight = ItemPadding;
			int itemsWidth = ItemPadding;
			for(int i = 0; i < legendItems.Count; i++) {
				ColorLegendViewInfoItem item = new ColorLegendViewInfoItem() { Text = legendItems[i].Text, Value = legendItems[i].Value, Format = LegendRangeStopsFormat };
				FormatLegendItemText(Legend, legendItems[i], item);
				Size textSize = CalculateTextSize(gr, item.ActualText);
				textSize.Width += RiskPadding;
				item.TextRect = new Rectangle(itemsWidth + RiskPadding, boundsHeight,
											  textSize.Width, textSize.Height);
				Rectangle riskRect = item.TextRect;
				riskRect.X -= RiskPadding;
				riskRect.Width = RiskWidth;
				riskRect.Height -= RiskWidth;
				item.RiskRect = riskRect;
				item.ContentRect = new Rectangle(itemsWidth, boundsHeight + textSize.Height, colorRectWidth, ItemPadding);
				itemsWidth += textSize.Width > colorRectWidth ? textSize.Width : colorRectWidth;
				int h = item.TextRect.Height + ItemPadding;
				itemsHeight = itemsHeight < h ? h : itemsHeight;
				item.Color = legendItems[i].Color;
				ColorLegendItems[i] = item;
			}
			boundsHeight += itemsHeight + ItemPadding;
			boundsWidth = boundsWidth < itemsWidth ? itemsWidth : boundsWidth;
			boundsWidth += ItemPadding;
			CalculateBounds(controlBounds, boundsHeight, boundsWidth);
			return controlBounds;
		}
		int FindMaxItemWidth(Graphics gr) {
			int maxItemWidth = 0;
			IList<MapLegendItemBase> legendItems = Legend.GetItems();
			for(int i = 0; i < legendItems.Count; i++) {
				string text = legendItems[i].GetActualText(FormatService, LegendRangeStopsFormat);
				Size textSize = CalculateTextSize(gr, text);
				maxItemWidth = Math.Max(maxItemWidth, textSize.Width);
			}
			return maxItemWidth + RiskPadding;
		}
	}
	public abstract class SizeLegendViewInfo : LegendViewInfo {
		TemplateGeometryType templateType;
		protected TemplateGeometryType TemplateType { get { return templateType; } }
		public new SizeLegend Legend { get { return (SizeLegend)base.Legend; } }
		public new SizeLegendPainter Painter { get { return (SizeLegendPainter)base.Painter; } }
		protected SizeLegendViewInfo(SizeLegend legend, InnerMap map, SizeLegendPainter painter)
			: base(legend, map, painter) {
			this.templateType = CalculateTemplateType(legend);
		}
		TemplateGeometryType CalculateTemplateType(SizeLegend legend) {
			VectorItemsLayer layer = legend.DataProvider as VectorItemsLayer;
			if(layer == null)
				return TemplateGeometryType.Circle;
			ITemplateGeometryProvider provider = layer.Data as ITemplateGeometryProvider;
			return provider != null ? provider.TemplateType : TemplateGeometryType.Circle;
		}
		Color CalculateItemColor(SizeLegendItem item) {
			if(MapUtils.IsColorEmpty(item.Color))
				return ((MapElementStyle)PaintAppearance[MapAppearanceNames.LegendItem]).Fill;
			return item.Color;
		}
		protected abstract void CalculateItemsOverlay(Graphics gr, Rectangle controlBounds, IList<MapLegendItemBase> legendItems, int boundsHeight, int boundsWidth);
		protected internal SizeLegendViewInfoItem CreateViewInfoItem(SizeLegendItem bubbleItem) {
			Color itemColor = CalculateItemColor(bubbleItem);
			SizeLegendViewInfoItem item = new SizeLegendViewInfoItem() {
				Text = bubbleItem.Text, Value = bubbleItem.Value,
				Format = LegendRangeStopsFormat, Color = itemColor
			};
			return item;
		}
		protected internal void CalculateBubbleOverlay(Graphics gr, Rectangle controlBounds) {
			int height = ItemPadding;
			int boundsWidth = 0;
			Size headerSize = CalculateHeaderDescription(gr, boundsWidth, height);
			boundsWidth = headerSize.Width + ItemPadding;
			height = headerSize.Height;
			IList<MapLegendItemBase> legendItems = Legend.GetItems();
			CalculateItemsOverlay(gr, controlBounds, legendItems, height, boundsWidth);
		}
		protected internal override void Initialize(int legendIndex) {
			base.Initialize(legendIndex);
			if(Painter != null)
				Painter.UpdateTemplate(templateType);
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			CalculateBubbleOverlay(gr, controlBounds);
		}
	}
	public class InlineSizeLegendViewInfo : SizeLegendViewInfo {
		protected override int KeyInternal { get { return 2048; } }
		public InlineSizeLegendViewInfo(SizeLegend legend, InnerMap map, SizeLegendPainter painter)
			: base(legend, map, painter) {
		}
		void ApplyAlignment(int textX, int maxMarkerWidth) {
			int halfMaxMarkerWidth = maxMarkerWidth / 2;
			int count = ColorLegendItems.Length;
			for(int i = 0; i < count; i++) {
				SizeLegendViewInfoItem item = (SizeLegendViewInfoItem)ColorLegendItems[i];
				Rectangle rect = item.TextRect;
				rect.X = textX;
				item.TextRect = rect;
				if(item.MarkerRect.Width < maxMarkerWidth) {
					rect = item.MarkerRect;
					rect.X = rect.X + halfMaxMarkerWidth - rect.Width / 2;
					item.MarkerRect = rect;
				}
				item.TickMarkRect = CalculateTickMarkBounds(item);
			}
		}
		protected override void CalculateItemsOverlay(Graphics gr, Rectangle controlBounds, IList<MapLegendItemBase> legendItems, int boundsHeight, int boundsWidth) {
			ColorLegendItems = new SizeLegendViewInfoItem[legendItems.Count];
			int maxTextX = 0;
			int maxMarkerWidth = 0;
			for(int i = 0; i < legendItems.Count; i++) {
				SizeLegendItem bubbleItem = (SizeLegendItem)legendItems[i];
				SizeLegendViewInfoItem item = CreateViewInfoItem(bubbleItem);
				FormatLegendItemText(Legend, legendItems[i], item);
				Size textSize = MapUtils.CalcStringPixelSize(gr, item.ActualText, ItemTextStyle.Font);
				item.MarkerRect = new Rectangle(ItemPadding, boundsHeight, bubbleItem.MarkerSize, bubbleItem.MarkerSize);
				int textY = RectUtils.GetCenter(item.MarkerRect).Y - textSize.Height / 2;
				maxTextX = Math.Max(maxTextX, item.MarkerRect.Right + ItemPadding);
				item.TextRect = new Rectangle(maxTextX, textY, textSize.Width, textSize.Height);
				boundsWidth = Math.Max(boundsWidth, bubbleItem.MarkerSize + ItemPadding * 2 + textSize.Width);
				ColorLegendItems[i] = item;
				boundsHeight += Math.Max(textSize.Height / 2, bubbleItem.MarkerSize) + ItemPadding / 2;
				maxMarkerWidth = Math.Max(maxMarkerWidth, bubbleItem.MarkerSize);
			}
			ApplyAlignment(maxTextX, maxMarkerWidth);
			CalculateBounds(controlBounds, boundsHeight, boundsWidth + ItemPadding);
		}
		Rectangle CalculateTickMarkBounds(SizeLegendViewInfoItem item) {
			if(!Legend.ShowTickMarks)
				return Rectangle.Empty;
			int left = item.MarkerRect.Right + 2;
			int right = item.TextRect.Left - 2;
			int top = RectUtils.GetVerticalCenter(item.MarkerRect);
			return new Rectangle(left, top, right - left, 1);
		}
	}
	public class NestedSizeLegendViewInfo : SizeLegendViewInfo {
		protected override int KeyInternal { get { return 1024; } }
		public NestedSizeLegendViewInfo(SizeLegend legend, InnerMap map, SizeLegendPainter painter)
			: base(legend, map, painter) {
		}
		Point CalculateTangencyPoint(int maxMarkerSize, int height) {
			return new Point(ItemPadding + maxMarkerSize / 2, height + maxMarkerSize);
		}
		int GetMaxMarkerWidth(IList<MapLegendItemBase> legendItems) {
			return legendItems.Count > 0 ? ((SizeLegendItem)legendItems[legendItems.Count - 1]).MarkerSize : 0;
		}
		Rectangle CalculateMarkerRect(Point markersTangencyPoint, int markerSize) {
			return new Rectangle(markersTangencyPoint.X - markerSize / 2, markersTangencyPoint.Y - markerSize, markerSize, markerSize);
		}
		Rectangle CalculateTextRect(Size textSize, Rectangle markerRect, int maxMarkerSize) {
			return new Rectangle(new Point(maxMarkerSize + 2 * ItemPadding, markerRect.Top - textSize.Height / 2), textSize);
		}
		int GetMaxItemTextWidth() {
			return ColorLegendItems.Count() > 0 ? ColorLegendItems[ColorLegendItems.Count() - 1].TextRect.Width : 0;
		}
		protected override void CalculateItemsOverlay(Graphics gr, Rectangle controlBounds, IList<MapLegendItemBase> legendItems, int boundsHeight, int boundsWidth) {
			ColorLegendItems = new SizeLegendViewInfoItem[legendItems.Count];
			int maxMarkerSize = GetMaxMarkerWidth(legendItems);
			Point markersTangencyPoint = CalculateTangencyPoint(maxMarkerSize, boundsHeight);
			for(int i = 0; i < legendItems.Count; i++) {
				SizeLegendItem bubbleItem = (SizeLegendItem)legendItems[i];
				SizeLegendViewInfoItem item = CreateViewInfoItem(bubbleItem);
				FormatLegendItemText(Legend, legendItems[i], item);
				item.MarkerRect = CalculateMarkerRect(markersTangencyPoint, bubbleItem.MarkerSize);
				Size textSize = MapUtils.CalcStringPixelSize(gr, item.ActualText, ItemTextStyle.Font);
				item.TextRect = CalculateTextRect(textSize, item.MarkerRect, maxMarkerSize);
				item.TickMarkRect = CalculateTickMarkBounds(item);
				ColorLegendItems[i] = item;
			}
			boundsWidth = Math.Max(boundsWidth, maxMarkerSize + ItemPadding * 2 + GetMaxItemTextWidth());
			boundsHeight = markersTangencyPoint.Y + ItemPadding / 2;
			CalculateBounds(controlBounds, boundsHeight, boundsWidth + ItemPadding);
		}
		Rectangle CalculateTickMarkBounds(SizeLegendViewInfoItem item) {
			if(!Legend.ShowTickMarks)
				return Rectangle.Empty;
			int left = RectUtils.GetHorizontalCenter(item.MarkerRect);
			int right = item.TextRect.Left;
			int top = item.MarkerRect.Top;
			return new Rectangle(left, top, right - left, 1);
		}
	}
	public abstract class LegendPainter : OverlayViewInfoPainter {
		protected LegendPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		void DrawBackground(GraphicsCache cache, LegendViewInfo viewInfo, BackgroundStyle bgStyle) {
			cache.FillRectangle(bgStyle.Fill, viewInfo.ClientBounds);
		}
		protected abstract void DrawItems(GraphicsCache cache, LegendViewInfoItemBase[] items, MapElementStyle itemStyle, Point offset);
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			LegendViewInfo vi = (LegendViewInfo)viewInfo;
			LegendAppearance paintAppearance = (LegendAppearance)(viewInfo.PaintAppearance);
			BackgroundStyle bgStyle = paintAppearance.BackgroundStyle;
			TextElementStyle headerStyle = paintAppearance.HeaderStyle;
			TextElementStyle descriptionStyle = paintAppearance.DescriptionStyle;
			DrawBackground(cache, vi, bgStyle);
			Brush headerBrush = cache.GetSolidBrush(headerStyle.TextColor);
			Brush descriptionBrush = cache.GetSolidBrush(descriptionStyle.TextColor);
			cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			MapUtils.TextPainter.DrawString(cache, vi.Header, headerStyle.Font, headerBrush, RectUtils.Offset(vi.HeaderRect, vi.ClientBounds.Location), StringFormat.GenericTypographic);
			MapUtils.TextPainter.DrawString(cache, vi.Description, descriptionStyle.Font, descriptionBrush, RectUtils.Offset(vi.DescriptionRect, vi.ClientBounds.Location), StringFormat.GenericTypographic);
			if(vi.ColorLegendItems != null)
				DrawItems(cache, vi.ColorLegendItems, paintAppearance.ItemStyle, vi.ClientBounds.Location);
		}
	}
	public class ColorLegendPainter : LegendPainter {
		public ColorLegendPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		protected override void DrawItems(GraphicsCache cache, LegendViewInfoItemBase[] items, MapElementStyle itemStyle, Point offset) {
			Brush itemBrush = cache.GetSolidBrush(itemStyle.Fill);
			Brush textBrush = cache.GetSolidBrush(itemStyle.TextColor);
			for(int i = 0; i < items.Count(); i++) {
				LegendViewInfoItem legendItem = (LegendViewInfoItem)items[i];
				legendItem.DrawItemContent(cache, itemBrush, offset);
				MapUtils.TextPainter.DrawString(cache, legendItem.ActualText, itemStyle.Font, textBrush, RectUtils.Offset(legendItem.TextRect, offset), StringFormat.GenericTypographic);
			}
		}
	}
	public class SizeLegendPainter : LegendPainter, IDisposable {
		GraphicsPath templatePath = new GraphicsPath();
		void CreateTransform(Graphics graphics, Rectangle rectangle) {
			graphics.TranslateTransform(rectangle.Left, rectangle.Top);
			graphics.ScaleTransform(rectangle.Width, rectangle.Height);
		}
		public SizeLegendPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		#region IDisposable members
		bool isDisposed;
		void Dispose(bool disposing) {
			if(templatePath != null) {
				templatePath.Dispose();
				templatePath = null;
			}
			if(disposing && !isDisposed) {
				isDisposed = true;
			}
		}
		~SizeLegendPainter() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		void ClearTransform(Graphics graphics) {
			graphics.ResetTransform();
		}
		void DrawMarker(Graphics graphics, Rectangle rectangle, Brush brush, Pen pen) {
			CreateTransform(graphics, rectangle);
			graphics.FillPath(brush, templatePath);
			graphics.DrawPath(pen, templatePath);
			ClearTransform(graphics);
		}
		void DrawTickMarks(GraphicsCache cache, Rectangle rect, Pen pen) {
			if(rect != Rectangle.Empty) cache.Graphics.DrawLine(pen, rect.Location, new Point(rect.Right, rect.Top));
		}
		internal void UpdateTemplate(TemplateGeometryType templateType) {
			TemplateGeometryBase geometry = TemplateGeometryBase.CreateTemplate(templateType);
			MapUnit[] templateVertices = geometry.Points;
			templatePath.Reset();
			templatePath.AddPolygon(MapUtils.MapUnitsToPointsF(templateVertices));
		}
		protected override void DrawItems(GraphicsCache cache, LegendViewInfoItemBase[] items, MapElementStyle itemStyle, Point offset) {
			Brush textBrush = cache.GetSolidBrush(itemStyle.TextColor);
			cache.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			for(int i = items.Length - 1; i >= 0; i--) {
				SizeLegendViewInfoItem legendItem = (SizeLegendViewInfoItem)items[i];
				Brush markerBrush = cache.GetSolidBrush(items[i].Color);
				Pen markerPen = cache.GetPen(itemStyle.TextColor, 0);
				DrawMarker(cache.Graphics, RectUtils.Offset(legendItem.MarkerRect, offset), markerBrush, markerPen);
				DrawTickMarks(cache, RectUtils.Offset(legendItem.TickMarkRect, offset), markerPen);
				MapUtils.TextPainter.DrawString(cache, legendItem.ActualText, itemStyle.Font, textBrush, RectUtils.Offset(legendItem.TextRect, offset), StringFormat.GenericTypographic);
			}
		}
	}
}
