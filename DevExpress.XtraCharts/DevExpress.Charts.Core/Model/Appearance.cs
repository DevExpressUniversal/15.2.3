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

using System.Collections.Generic;
namespace DevExpress.Charts.Model {
	public class ChartAppearanceOptions : ModelElement {
		ChartAppearance chartAppearance;
		LegendAppearance legendAppearance;
		DiagrammAppearance diagrammAppearance;
		SeriesAppearance seriesAppearance;
		public ChartAppearance ChartAppearance {
			get { return chartAppearance; }
			set {
				if (chartAppearance != value) {
					UpdateElementParent(chartAppearance, null);
					chartAppearance = value;
					UpdateElementParent(chartAppearance, this);
					NotifyParent(this, "ChartAppearance", value);
				}
			}
		}
		public LegendAppearance LegendAppearance {
			get { return legendAppearance; }
			set {
				if (legendAppearance != value) {
					UpdateElementParent(legendAppearance, null);
					legendAppearance = value;
					UpdateElementParent(legendAppearance, this);
					NotifyParent(this, "LegendAppearance", value);
				}
			}
		}
		public DiagrammAppearance DiagrammAppearance {
			get { return diagrammAppearance; }
			set {
				if (diagrammAppearance != value) {
					UpdateElementParent(diagrammAppearance, null);
					diagrammAppearance = value;
					UpdateElementParent(diagrammAppearance, this);
					NotifyParent(this, "DiagrammAppearance", value);
				}
			}
		}
		public SeriesAppearance SeriesAppearance {
			get { return seriesAppearance; }
			set {
				if (seriesAppearance != value) {
					UpdateElementParent(seriesAppearance, null);
					seriesAppearance = value;
					UpdateElementParent(seriesAppearance, this);
					NotifyParent(this, "SeriesAppearance", value);
				}
			}
		}
	}
	public class ChartAppearance : ModelElement {
		ColorARGB backColor;
		Border border;
		FillStyle fillStyle;
		RectangleIndents padding;
		ChartTitleAppearance titleAppearance;
		public ColorARGB BackColor {
			get { return backColor; }
			set {
				if (backColor != value) {
					backColor = value;
					NotifyParent(this, "BackColor", value);
				}
			}
		}
		public Border Border {
			get { return border; }
			set {
				if (border != value) {
					UpdateElementParent(border, null);
					border = value;
					UpdateElementParent(border, this);
					NotifyParent(this, "Border", value);
				}
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
		public RectangleIndents Padding {
			get { return padding; }
			set {
				if (padding != value) {
					UpdateElementParent(padding, null);
					padding = value;
					UpdateElementParent(padding, this);
					NotifyParent(this, "Padding", value);
				}
			}
		}
		public ChartTitleAppearance TitleAppearance {
			get { return titleAppearance; }
			set {
				if (titleAppearance != value) {
					UpdateElementParent(titleAppearance, null);
					titleAppearance = value;
					UpdateElementParent(titleAppearance, this);
					NotifyParent(this, "TitleAppearance", value);
				}
			}
		}
	}
	public class LegendAppearance : ModelElement {
		ColorARGB backColor;
		Border border;
		ColorARGB textColor;
		FillStyle fillStyle;
		Shadow shadow;
		RectangleIndents margins;
		RectangleIndents padding;
		public ColorARGB BackColor {
			get { return backColor; }
			set {
				if (backColor != value) {
					backColor = value;
					NotifyParent(this, "BackColor", value);
				}
			}
		}
		public Border Border {
			get { return border; }
			set {
				if (border != value) {
					UpdateElementParent(border, null);
					border = value;
					UpdateElementParent(border, this);
					NotifyParent(this, "Border", value);
				}
			}
		}
		public ColorARGB TextColor {
			get { return textColor; }
			set {
				if (textColor != value) {
					textColor = value;
					NotifyParent(this, "TextColor", value);
				}
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
		public Shadow Shadow {
			get { return shadow; }
			set {
				if (shadow != value) {
					UpdateElementParent(shadow, null);
					shadow = value;
					UpdateElementParent(shadow, this);
					NotifyParent(this, "Shadow", value);
				}
			}
		}
		public RectangleIndents Margins {
			get { return margins; }
			set {
				if (margins != value) {
					UpdateElementParent(margins, null);
					margins = value;
					UpdateElementParent(margins, this);
					NotifyParent(this, "Margins", value);
				}
			}
		}
		public RectangleIndents Padding {
			get { return padding; }
			set {
				if (padding != value) {
					UpdateElementParent(padding, null);
					padding = value;
					UpdateElementParent(padding, this);
					NotifyParent(this, "Padding", value);
				}
			}
		}
	}
	public class DiagrammAppearance : ModelElement {
		ColorARGB backColor;
		ColorARGB borderColor;
		bool borderVisible;
		AxisAppearance axesAppeanrance;
		FillStyle fillStyle;
		Shadow shadow;
		RectangleIndents margins;
		public ColorARGB BackColor {
			get { return backColor; }
			set {
				if (backColor != value) {
					backColor = value;
					NotifyParent(this, "BackColor", value);
				}
			}
		}
		public ColorARGB BorderColor {
			get { return borderColor; }
			set {
				if (borderColor != value) {
					borderColor = value;
					NotifyParent(this, "BorderColor", value);
				}
			}
		}
		public bool BorderVisible {
			get { return borderVisible; }
			set {
				if (borderVisible != value) {
					borderVisible = value;
					NotifyParent(this, "BorderVisible", value);
				}
			}
		}
		public AxisAppearance AxesAppearance {
			get { return axesAppeanrance; }
			set {
				if (axesAppeanrance != value) {
					UpdateElementParent(axesAppeanrance, null);
					axesAppeanrance = value;
					UpdateElementParent(axesAppeanrance, this);
					NotifyParent(this, "AxesAppeanrance", value);
				}
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
		public Shadow Shadow {
			get { return shadow; }
			set {
				if (shadow != value) {
					UpdateElementParent(shadow, null);
					shadow = value;
					UpdateElementParent(shadow, this);
					NotifyParent(this, "Shadow", value);
				}
			}
		}
		public RectangleIndents Margins {
			get { return margins; }
			set {
				if (margins != value) {
					UpdateElementParent(margins, null);
					margins = value;
					UpdateElementParent(margins, this);
					NotifyParent(this, "Margins", value);
				}
			}
		}
	}
	public class AxisAppearance : ModelElement {
		ColorARGB color;
		int thickness = 1;
		bool interlaced;
		ColorARGB interlacedColor;
		FillStyle interlacedFillStyle;
		ColorARGB gridLinesColor;
		LineStyle gridLinesLineStyle;
		ColorARGB gridLinesMinorColor;
		LineStyle gridLinesMinorLineStyle;
		ColorARGB labelTextColor;
		AxisTitleAppearance titleAppearance;
		public ColorARGB Color {
			get { return color; }
			set {
				if (color != value) {
					color = value;
					NotifyParent(this, "Color", value);
				}
			}
		}
		public int Thickness {
			get { return thickness; }
			set {
				if ( thickness!= value) {
					thickness = value;
					NotifyParent(this, "Thickness", value);
				}
			}
		}
		public bool Interlaced {
			get { return interlaced; }
			set {
				if (interlaced != value) {
					interlaced = value;
					NotifyParent(this, "Interlaced", value);
				}
			}
		}
		public FillStyle InterlacedFillStyle {
			get { return interlacedFillStyle; }
			set {
				if (interlacedFillStyle != value) {
					UpdateElementParent(interlacedFillStyle, null);
					interlacedFillStyle = value;
					UpdateElementParent(interlacedFillStyle, this);
					NotifyParent(this, "InterlacedFillStyle", value);
				}
			}
		}
		public ColorARGB InterlacedColor {
			get { return interlacedColor; }
			set {
				if (interlacedColor != value) {
					interlacedColor = value;
					NotifyParent(this, "InterlacedColor", value);
				}
			}
		}
		public ColorARGB GridLinesColor{
			get { return gridLinesColor; }
			set {
				if (gridLinesColor != value) {
					gridLinesColor = value;
					NotifyParent(this, "GridLinesColor", value);
				}
			}
		}
		public LineStyle GridLinesLineStyle {
			get { return gridLinesLineStyle; }
			set {
				if (gridLinesLineStyle != value) {
					UpdateElementParent(gridLinesLineStyle, null);
					gridLinesLineStyle = value;
					UpdateElementParent(gridLinesLineStyle, this);
					NotifyParent(this, "GridLinesLineStyle", value);
				}
			}
		}
		public ColorARGB GridLinesMinorColor{
			get { return gridLinesMinorColor; }
			set {
				if (gridLinesMinorColor != value) {
					gridLinesMinorColor = value;
					NotifyParent(this, "GridLinesMinorColor", value);
				}
			}
		}
		public LineStyle GridLinesMinorLineStyle {
			get { return gridLinesMinorLineStyle; }
			set {
				if (gridLinesMinorLineStyle != value) {
					UpdateElementParent(gridLinesMinorLineStyle, null);
					gridLinesMinorLineStyle = value;
					UpdateElementParent(gridLinesMinorLineStyle, this);
					NotifyParent(this, "GridLinesMinorLineStyle", value);
				}
			}
		}
		public ColorARGB LabelTextColor {
			get { return labelTextColor; }
			set {
				if (labelTextColor != value) {
					labelTextColor = value;
					NotifyParent(this, "LabelTextColor", value);
				}
			}
		}
		public AxisTitleAppearance TitleAppearance {
			get { return titleAppearance; }
			set {
				if (titleAppearance != value) {
					UpdateElementParent(titleAppearance, null);
					titleAppearance = value;
					UpdateElementParent(titleAppearance, this);
					NotifyParent(this, "TitleAppearance", value);
				}
			}
		}
	}
	public class SeriesAppearance : ModelElement {
		ColorARGB color = ColorARGB.Empty;
		Border border;
		FillStyle fillStyle;
		Shadow shadow;
		LineStyle lineStyle;
		MarkerAppearance markerAppearance;
		SeriesLabelAppearance labelAppearance;
		public ColorARGB Color {
			get { return color; }
			set {
				if (color != value) {
					color = value;
					NotifyParent(this, "Color", value);
				}
			}
		}
		public Border Border {
			get { return border; }
			set {
				if (border != value) {
					UpdateElementParent(border, null);
					border = value;
					UpdateElementParent(border, this);
					NotifyParent(this, "Border", value);
				}
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
		public Shadow Shadow {
			get { return shadow; }
			set {
				if (shadow != value) {
					UpdateElementParent(shadow, null);
					shadow = value;
					UpdateElementParent(shadow, this);
					NotifyParent(this, "Shadow", value);
				}
			}
		}
		public LineStyle LineStyle {
			get { return lineStyle; }
			set {
				if (lineStyle != value) {
					UpdateElementParent(lineStyle, null);
					lineStyle = value;
					UpdateElementParent(lineStyle, this);
					NotifyParent(this, "LineStyle", value);
				}
			}
		}
		public MarkerAppearance MarkerAppearance {
			get { return markerAppearance; }
			set {
				if (markerAppearance != value) {
					UpdateElementParent(markerAppearance, null);
					markerAppearance = value;
					UpdateElementParent(markerAppearance, this);
					NotifyParent(this, "MarkerAppearance", value);
				}
			}
		}
		public SeriesLabelAppearance LabelAppearance {
			get { return labelAppearance; }
			set {
				if (labelAppearance != value) {
					UpdateElementParent(labelAppearance, null);
					labelAppearance = value;
					UpdateElementParent(labelAppearance, this);
					NotifyParent(this, "LabelAppearance", value);
				}
			}
		}
	}
	public class MarkerAppearance : ModelElement {
		ColorARGB borderColor;
		bool borderVisible;
		FillStyle fillStyle;
		public ColorARGB BorderColor {
			get { return borderColor; }
			set {
				if (borderColor != value) {
					borderColor = value;
					NotifyParent(this, "BorderColor", value);
				}
			}
		}
		public bool BorderVisible {
			get { return borderVisible; }
			set {
				if (borderVisible != value) {
					borderVisible = value;
					NotifyParent(this, "BorderVisible", value);
				}
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
	}
	public class SeriesLabelAppearance : ModelElement {
		ColorARGB backColor;
		Border border;
		FillStyle fillStyle;
		ColorARGB lineColor;
		int lineLength;
		LineStyle lineStyle;
		bool lineVisible;
		Shadow shadow;
		ColorARGB textColor;
		public ColorARGB BackColor {
			get { return backColor; }
			set {
				if (backColor != value) {
					backColor = value;
					NotifyParent(this, "BackColor", value);
				}
			}
		}
		public Border Border {
			get { return border; }
			set {
				UpdateElementParent(border, null);
				border = value;
				UpdateElementParent(border, this);
				NotifyParent(this, "Border", value);
			}
		}
		public FillStyle FillStyle {
			get { return fillStyle; }
			set {
				if (fillStyle != value) {
					UpdateElementParent(fillStyle, null);
					fillStyle = value;
					UpdateElementParent(fillStyle, this);
					NotifyParent(this, "FillStyle", value);
				}
			}
		}
		public ColorARGB LineColor {
			get { return lineColor; }
			set {
				if (lineColor != value) {
					lineColor = value;
					NotifyParent(this, "LineColor", value);
				}
			}
		}
		public int LineLength {
			get { return lineLength; }
			set {
				if (lineLength != value) {
					lineLength = value;
					NotifyParent(this, "LineLength", value);
				}
			}
		}
		public LineStyle LineStyle {
			get { return lineStyle; }
			set {
				if (lineStyle != value) {
					UpdateElementParent(lineStyle, null);
					lineStyle = value;
					UpdateElementParent(lineStyle, this);
					NotifyParent(this, "LineStyle", value);
				}
			}
		}
		public bool LineVisible {
			get { return lineVisible; }
			set {
				if (lineVisible != value) {
					lineVisible = value;
					NotifyParent(this, "LineVisible", value);
				}
			}
		}
		public Shadow Shadow {
			get { return shadow; }
			set {
				if (shadow != value) {
					UpdateElementParent(shadow, null);
					shadow = value;
					UpdateElementParent(shadow, this);
					NotifyParent(this, "Shadow", value);
				}
			}
		}
		public ColorARGB TextColor {
			get { return textColor; }
			set {
				if (textColor != value) {
					textColor = value;
					NotifyParent(this, "TextColor", value);
				}
			}
		}
	}
	public abstract class TitleAppearanceBase : ModelElement {
		ColorARGB textColor;
		public ColorARGB TextColor {
			get { return textColor; }
			set {
				if (textColor != value) {
					textColor = value;
					NotifyParent(this, "TextColor", value);
				}
			}
		}
	}
	public class AxisTitleAppearance : TitleAppearanceBase {
	}
	public class ChartTitleAppearance : TitleAppearanceBase {
		int indent = 5;
		public int Indent {
			get { return indent; }
			set {
				if (indent != value) {
					indent = value;
					NotifyParent(this, "Indent", value);
				}
			}
		}
	}
	public class Palette : ModelElement {
		readonly List<PaletteEntry> entries;
		public List<PaletteEntry> Entries {
			get { return entries; }
		}
		public Palette(ModelElement parent) : base(parent) {
			this.entries = new List<PaletteEntry>();
		}
		public Palette() {
			this.entries = new List<PaletteEntry>();
		}
		public Palette(List<PaletteEntry> entries) {
			this.entries = entries;
		}
	}
	public struct PaletteEntry {
		ColorARGB color;
		ColorARGB color2;
		public ColorARGB Color {
			get { return color; }
		}
		public ColorARGB Color2 {
			get { return color2; }
		}
		public PaletteEntry(ColorARGB color, ColorARGB color2) {
			this.color = color;
			this.color2 = color2;
		}
		public PaletteEntry(ColorARGB color) : this(color, ColorARGB.Empty) {
		}
	}
	public class Border : ModelElement {
		int thickness = 1;
		ColorARGB color = ColorARGB.Empty;
		public int Thickness {
			get { return thickness; }
			set {
				if (thickness != value) {
					thickness = value;
					NotifyParent(this, "Thickness", value);
				}
			}
		}
		public ColorARGB Color {
			get { return color; }
			set {
				if (color != value) {
					color = value;
					NotifyParent(this, "Color", value);
				}
			}
		}
		public Border()
			: this(null) {
		}
		public Border(ModelElement parent)
			: base(parent) {
		}
	}
	public class Shadow : ModelElement {
		ColorARGB color = ColorARGB.Empty;
		int size = 2;
		public ColorARGB Color {
			get { return color; }
			set {
				if (color != value) {
					color = value;
					NotifyParent(this, "Color", value);
				}
			}
		}
		public int Size {
			get { return size; }
			set {
				if (size != value) {
					size = value;
					NotifyParent(this, "Size", value);
				}
			}
		}
	}
	public enum FillMode {
		Empty,
		Solid,
		Gradient
	}
	public enum GradientMode {
		TopToBottom,
		BottomToTop,
		LeftToRight,
		RightToLeft,
		TopLeftToBottomRight,
		BottomRightToTopLeft,
		TopRightToBottomLeft,
		BottomLeftToTopRight,
		FromCenterHorizontal,
		ToCenterHorizontal,
		FromCenterVertical,
		ToCenterVertical
	}
	public enum DashStyle {
		Empty,
		Solid,
		Dash,
		Dot,
		DashDot,
		DashDotDot
	}
	public class FillOptions : ModelElement {
		ColorARGB color2;
		GradientMode gradientMode;
		public ColorARGB Color2 {
			get { return color2; }
			set {
				if (color2 != value) {
					color2 = value;
					NotifyParent(this, "Color2", value);
				}
			}
		}
		public GradientMode GradientMode {
			get { return gradientMode; }
			set {
				if (gradientMode != value) {
					gradientMode = value;
					NotifyParent(this, "GradientMode", value);
				}
			}
		}
	}
	public class FillStyle : ModelElement {
		FillMode fillMode;
		FillOptions options;
		public FillMode FillMode {
			get { return fillMode; }
			set {
				if (fillMode != value) {
					fillMode = value;
					NotifyParent(this, "FillMode", value);
				}
			}
		}
		public FillOptions Options {
			get { return options; }
			set {
				if (options != value) {
					UpdateElementParent(options, null);
					options = value;
					UpdateElementParent(options, this);
					NotifyParent(this, "Options", value);
				}
			}
		}
	}
	public class LineStyle : ModelElement {
		int thickness;
		DashStyle dashStyle;
		public int Thickness {
			get { return thickness; }
			set {
				if (thickness != value) {
					thickness = value;
					NotifyParent(this, "Thickness", value);
				}
			}
		}
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				if (dashStyle != value) {
					dashStyle = value;
					NotifyParent(this, "DashStyle", value);
				}
			}
		}
	}
	public class RectangleIndents : ModelElement {
		public const int Undefined = -1;
		int left;
		int top;
		int right;
		int bottom;
		public int Left {
			get { return left; }
			set {
				if (left != value) {
					left = value;
					NotifyParent(this, "Left", value);
				}
			}
		}
		public int Top {
			get { return top; }
			set {
				if (top != value) {
					top = value;
					NotifyParent(this, "Top", value);
				}
			}
		}
		public int Right {
			get { return right; }
			set {
				if (right != value) {
					right = value;
					NotifyParent(this, "Right", value);
				}
			}
		}
		public int Bottom {
			get { return bottom; }
			set {
				if (bottom != value) {
					bottom = value;
					NotifyParent(this, "Bottom", value);
				}
			}
		}
		public int All {
			get { return left == top && left == right && left == bottom ? left : Undefined; }
			set {
				Left = value;
				Top = value;
				Right = value;
				Bottom = value;
			}
		}
		public RectangleIndents(int left, int top, int right, int bottom) {
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
		public RectangleIndents(int all)
			: this(all, all, all, all) {
		}
		public RectangleIndents()
			: this(0) {
		}
	}
}
