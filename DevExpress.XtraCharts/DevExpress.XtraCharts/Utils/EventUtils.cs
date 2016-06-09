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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using System.Collections;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	public enum CursorType {
		None,
		Move,
		Hand,
		Grab,
		Rotate,
		RotateDrag,
		SizeAll,
		SizeNESW,
		SizeNS,
		SizeNWSE,
		SizeWE,
		ZoomIn,
		ZoomLimit,
		ZoomOut
	}
	public enum ChartScrollDirection {
		Horizontal,
		Vertical,
		Both
	}
	public enum ChartScrollOrientation {
		AxisXScroll = ScrollingOrientation.AxisXScroll,
		AxisYScroll = ScrollingOrientation.AxisYScroll,
		BothAxesScroll = ScrollingOrientation.BothAxesScroll
	}
	public enum ChartScrollEventType {
		LargeDecrement = NavigationType.LargeDecrement,
		LargeIncrement = NavigationType.LargeIncrement,
		SmallDecrement = NavigationType.SmallDecrement,
		SmallIncrement = NavigationType.SmallIncrement,
		ThumbPosition = NavigationType.ThumbPosition,
		LeftButtonMouseDrag = NavigationType.LeftButtonMouseDrag,
		MiddleButtonMouseDrag = NavigationType.MiddleButtonMouseDrag,
		ArrowKeys = NavigationType.ArrowKeys,
		Gesture = NavigationType.Gesture
	}
	public enum ChartZoomEventType {
		ZoomIn = NavigationType.ZoomIn,
		ZoomOut = NavigationType.ZoomOut,
		ZoomUndo = NavigationType.ZoomUndo
	}
	[RuntimeObject]
	public class CrosshairElement {
		readonly RefinedPoint refinedPoint;
		readonly IRefinedSeries refinedSeries;
		readonly CrosshairLineElement lineElement;
		readonly CrosshairAxisLabelElement axisLabelElement;
		readonly CrosshairLabelElement labelElement;
		bool visible;
		internal IRefinedSeries RefinedSeries { get { return refinedSeries; } }
		internal RefinedPoint RefinedPoint { get { return refinedPoint; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementSeriesPoint")]
#endif
		public SeriesPoint SeriesPoint { get { return SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementSeries")]
#endif
		public Series Series { get { return (Series)refinedSeries.Series; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementLineElement")]
#endif
		public CrosshairLineElement LineElement { get { return lineElement; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementAxisLabelElement")]
#endif
		public CrosshairAxisLabelElement AxisLabelElement { get { return axisLabelElement; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementLabelElement")]
#endif
		public CrosshairLabelElement LabelElement { get { return labelElement; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairElementVisible")]
#endif
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		internal CrosshairElement(RefinedPoint refinedPoint, IRefinedSeries refinedSeries, CrosshairLineElement lineElement, CrosshairAxisLabelElement axisLabelElement, CrosshairLabelElement labelElement) {
			this.refinedSeries = refinedSeries;
			this.refinedPoint = refinedPoint;
			this.lineElement = lineElement;
			this.axisLabelElement = axisLabelElement;
			this.labelElement = labelElement;
			this.visible = true;
		}
	}
	[RuntimeObject]
	public class CrosshairLineElement {
		bool visible;
		Color color;
		readonly LineStyle lineStyle;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLineElementVisible")]
#endif
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLineElementColor")]
#endif
		public Color Color {
			get { return color; }
			set { color = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLineElementLineStyle")]
#endif
		public LineStyle LineStyle { get { return lineStyle; } }
		internal CrosshairLineElement(Color color, LineStyle lineStyle, bool visible) {
			this.visible = visible;
			this.color = color;
			this.lineStyle = lineStyle;
		}
	}
	[RuntimeObject]
	public abstract class BaseCrosshairLabelElement {
		bool visible;
		string text;
		Color textColor;
		Font font;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BaseCrosshairLabelElementVisible")]
#endif
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BaseCrosshairLabelElementText")]
#endif
		public string Text {
			get { return text; }
			set { text = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BaseCrosshairLabelElementTextColor")]
#endif
		public Color TextColor {
			get { return textColor; }
			set { textColor = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BaseCrosshairLabelElementFont")]
#endif
		public Font Font {
			get { return font; }
			set { font = value; }
		}
		internal BaseCrosshairLabelElement(string text, Color textColor, Font font, bool visible) {
			this.font = font;
			this.text = text;
			this.textColor = textColor;
			this.visible = visible;
		}
		internal BaseCrosshairLabelElement() { }
	}
	public class CrosshairAxisLabelElement : BaseCrosshairLabelElement, ISupportTextAntialiasing {
		object axisValue;
		Color backColor;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelElementAxisValue")]
#endif
		public object AxisValue {
			get { return axisValue; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairAxisLabelElementBackColor")]
#endif
		public Color BackColor {
			get { return backColor; }
			set { backColor = value; }
		}
		internal CrosshairAxisLabelElement(object axisValue, string text, Color backColor, Color textColor, Font font, bool visible)
			: base(text, textColor, font, visible) {
			this.axisValue = axisValue;
			this.backColor = backColor;
		}
		#region ISupportTextAntialiasing implementation
		DefaultBoolean ISupportTextAntialiasing.EnableAntialiasing { get { return DefaultBoolean.Default; } }
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return false; } }
		bool ISupportTextAntialiasing.Rotated { get { return false; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return backColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return RectangleFillStyle.Empty; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return null; } }
		#endregion
	}
	public class CrosshairGroupHeaderElement : BaseCrosshairLabelElement {
		readonly List<SeriesPoint> seriesPoints;
		internal bool ActualVisibility { get { return !string.IsNullOrEmpty(Text) && Visible; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairGroupHeaderElementSeriesPoints")]
#endif
		public IEnumerable<SeriesPoint> SeriesPoints { get { return seriesPoints; } }
		internal CrosshairGroupHeaderElement(List<SeriesPoint> seriesPoints, string text, Color textColor, Font font)
			: base(text, textColor, font, true) {
			this.seriesPoints = seriesPoints;
		}
	}
	public class CrosshairLabelElement : BaseCrosshairLabelElement {
		Image markerImage;
		ChartImageSizeMode markerImageSizeMode;
		Size markerSize;
		Color markerColor;
		bool markerVisible;
		bool textVisible;
		string headerText;
		string footerText;
		internal bool Empty { get { return !Visible && String.IsNullOrEmpty(headerText) && String.IsNullOrEmpty(footerText); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementMarkerImage")]
#endif
		public Image MarkerImage {
			get { return markerImage; }
			set { markerImage = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementMarkerImageSizeMode")]
#endif
		public ChartImageSizeMode MarkerImageSizeMode {
			get { return markerImageSizeMode; }
			set { markerImageSizeMode = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementMarkerSize")]
#endif
		public Size MarkerSize {
			get { return markerSize; }
			set { markerSize = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementMarkerColor")]
#endif
		public Color MarkerColor {
			get { return markerColor; }
			set { markerColor = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementMarkerVisible")]
#endif
		public bool MarkerVisible {
			get { return markerVisible; }
			set { markerVisible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementTextVisible")]
#endif
		public bool TextVisible {
			get { return textVisible; }
			set { textVisible = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementHeaderText")]
#endif
		public string HeaderText {
			get { return headerText; }
			set { headerText = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("CrosshairLabelElementFooterText")]
#endif
		public string FooterText {
			get { return footerText; }
			set { footerText = value; }
		}
		internal CrosshairLabelElement(string text, Color textColor, Font font, Image markerImage, ChartImageSizeMode markerImageSizeMode, Size markerSize, Color markerColor,
			bool markerVisible, bool textVisible, bool visible)
			: base(text, textColor, font, visible) {
			this.markerImage = markerImage;
			this.markerImageSizeMode = markerImageSizeMode;
			this.markerSize = markerSize;
			this.markerColor = markerColor;
			this.markerVisible = markerVisible;
			this.textVisible = textVisible;
		}
	}
	public delegate void HotTrackEventHandler(object sender, HotTrackEventArgs e);
	public delegate void CustomDrawSeriesEventHandler(object sender, CustomDrawSeriesEventArgs e);
	public delegate void CustomDrawSeriesPointEventHandler(object sender, CustomDrawSeriesPointEventArgs e);
	public delegate void CustomDrawAxisLabelEventHandler(object sender, CustomDrawAxisLabelEventArgs e);
	public delegate void CustomPaintEventHandler(object sender, CustomPaintEventArgs e);
	public delegate void ChartScroll3DEventHandler(object sender, ChartScroll3DEventArgs e);
	public delegate void ChartScrollEventHandler(object sender, ChartScrollEventArgs e);
	public delegate void ChartZoomEventHandler(object sender, ChartZoomEventArgs e);
	public delegate void ChartZoom3DEventHandler(object sender, ChartZoom3DEventArgs e);
	public delegate void BoundDataChangedEventHandler(object sender, EventArgs e);
	public delegate void PieSeriesPointExplodedEventHandler(object sender, PieSeriesPointExplodedEventArgs e);
	public delegate void QueryCursorEventHandler(object sender, QueryCursorEventArgs e);
	public delegate void CustomizeAutoBindingSettingsEventHandler(object sender, EventArgs e);
	public delegate void CustomizeXAxisLabelsEventHandler(object sender, CustomizeXAxisLabelsEventArgs e);
	public delegate void CustomizeResolveOverlappingModeEventHandler(object sender, CustomizeResolveOverlappingModeEventArgs e);
	public delegate void CustomizeSimpleDiagramLayoutEventHandler(object sender, CustomizeSimpleDiagramLayoutEventArgs e);
	public delegate void PivotGridSeriesExcludedEventHandler(object sender, PivotGridSeriesExcludedEventArgs e);
	public delegate void PivotGridSeriesPointsExcludedEventHandler(object sender, PivotGridSeriesPointsExcludedEventArgs e);
	public delegate void CustomizeLegendEventHandler(object sender, CustomizeLegendEventArgs e);
	public delegate void CustomDrawCrosshairEventHandler(object sender, CustomDrawCrosshairEventArgs e);
	public delegate void AxisRangeChangedEventHandler(object sender, AxisRangeChangedEventArgs e);
	public delegate void LegendItemCheckedEventHandler(object sender, LegendItemCheckedEventArgs e);
	public delegate void SelectedItemsChangedEventHandler(object sender, SelectedItemsChangedEventArgs e);
	public class SelectedItemsChangedEventArgs : EventArgs {
		SelectedItemsChangedAction action;
		IList newItems;
		IList oldItems;
		public SelectedItemsChangedAction Action { get { return action; } }
		public IList NewItems { get { return newItems; } }
		public IList OldItems { get { return oldItems; } }
		internal SelectedItemsChangedEventArgs(SelectedItemsChangedAction action, IList newItems, IList oldItems)
			: base() {
			this.action = action;
			this.newItems = newItems;
			this.oldItems = oldItems; 
		}
	}
	public class HotTrackEventArgs : EventArgs {
		object obj;
		object additionalObj;
		bool cancel;
		ChartHitInfo hitInfo;
		public object Object { get { return this.obj; } }
		public object AdditionalObject { get { return additionalObj; } }
		public bool Cancel { get { return this.cancel; } set { this.cancel = value; } }
		public ChartHitInfo HitInfo { get { return this.hitInfo; } }
		internal HotTrackEventArgs(object obj, object additionalObj, ChartHitInfo hitInfo)
			: base() {
			this.obj = obj;
			this.additionalObj = additionalObj;
			this.hitInfo = hitInfo;
		}
	}
	public class CustomDrawSeriesEventArgs : EventArgs {
		bool disposeLegendFont;
		bool disposeLegendMarkerImage;
		bool legendMarkerVisible;
		bool legendTextVisible;
		Color legendTextColor;
		DrawOptions drawOptions;
		DrawOptions legendDrawOptions;
		Font legendFont;
		Image legendMarkerImage;
		ChartImageSizeMode legendMarkerImageSizeMode;
		Series series;
		Size legendMarkerSize;
		string legendText;
		public bool DisposeLegendFont { get { return disposeLegendFont; } set { disposeLegendFont = value; } }
		public bool DisposeLegendMarkerImage { get { return disposeLegendMarkerImage; } set { disposeLegendMarkerImage = value; } }
		public bool LegendMarkerVisible { get { return legendMarkerVisible; } set { legendMarkerVisible = value; } }
		public bool LegendTextVisible { get { return legendTextVisible; } set { legendTextVisible = value; } }
		public Color LegendTextColor { get { return legendTextColor; } set { legendTextColor = value; } }
		public DrawOptions SeriesDrawOptions { get { return drawOptions; } }
		public DrawOptions LegendDrawOptions { get { return legendDrawOptions; } }
		public Font LegendFont { get { return legendFont; } set { legendFont = value; } }
		public Image LegendMarkerImage { get { return legendMarkerImage; } set { legendMarkerImage = value; } }
		public ChartImageSizeMode LegendMarkerImageSizeMode { get { return legendMarkerImageSizeMode; } set { legendMarkerImageSizeMode = value; } }
		public Series Series { get { return series; } }
		public Size LegendMarkerSize { get { return legendMarkerSize; } set { legendMarkerSize = value; } }
		public string LegendText { get { return legendText; } set { legendText = value; } }
		internal CustomDrawSeriesEventArgs(DrawOptions drawOptions, Series series, string legendText, bool legendTextVisible,
			Color legendTextColor, Font legendFont, bool legendMarkerVisible, Size legendMarkerSize, DrawOptions legendDrawOptions,
			Image legendMarkerImage, ChartImageSizeMode legendMarkerImageSizeMode) {
			this.drawOptions = drawOptions;
			this.series = series;
			this.legendText = legendText;
			this.legendTextVisible = legendTextVisible;
			this.legendTextColor = legendTextColor;
			this.legendFont = legendFont;
			this.legendMarkerVisible = legendMarkerVisible;
			this.legendMarkerSize = legendMarkerSize;
			this.legendDrawOptions = legendDrawOptions;
			this.legendMarkerImage = legendMarkerImage;
			this.legendMarkerImageSizeMode = legendMarkerImageSizeMode;
		}
	}
	public enum SelectionState {
		Normal,
		HotTracked,
		Selected
	}
	public class CustomDrawSeriesPointEventArgs : CustomDrawSeriesEventArgs {
		SeriesPoint seriesPoint;
		string labelText;
		string secondLabelText;
		SelectionState selectionState;
		public SeriesPoint SeriesPoint { get { return seriesPoint; } }
		public string LabelText { get { return labelText; } set { labelText = value; } }
		public string SecondLabelText { get { return secondLabelText; } set { secondLabelText = value; } }
		public SelectionState SelectionState { get { return selectionState; } set { selectionState = value; } }
		internal CustomDrawSeriesPointEventArgs(DrawOptions drawOptions, Series series, SeriesPoint seriesPoint, string labelText,
			string secondLabelText, string legendText, bool legendTextVisible, Color legendTextColor, Font legendFont,
			bool legendMarkerVisible, Size legendMarkerSize, DrawOptions legendDrawOptions, Image legendMarkerImage,
			ChartImageSizeMode legendMarkerImageSizeMode, SelectionState selectionState)
			: base(drawOptions, series, legendText, legendTextVisible, legendTextColor,
				legendFont, legendMarkerVisible, legendMarkerSize, legendDrawOptions, legendMarkerImage, legendMarkerImageSizeMode) {
			this.seriesPoint = seriesPoint;
			this.labelText = labelText;
			this.secondLabelText = secondLabelText;
			this.selectionState = selectionState;
		}
	}
	public class CustomDrawAxisLabelEventArgs : EventArgs {
		readonly AxisLabelItemBase item;
		public AxisLabelItemBase Item { get { return item; } }
		internal CustomDrawAxisLabelEventArgs(AxisLabelItemBase item) {
			this.item = item;
		}
	}
	public class CustomPaintEventArgs : EventArgs {
		readonly Graphics graphics;
		readonly Rectangle bounds;
		public Graphics Graphics { get { return graphics; } }
		public Rectangle Bounds { get { return bounds; } }
		internal CustomPaintEventArgs(Graphics graphics, Rectangle bounds) {
			this.graphics = graphics;
			this.bounds = bounds;
		}
	}
	public class ChartScroll3DEventArgs : EventArgs {
		readonly ChartScrollDirection scrollDirection;
		readonly double oldHorizontalScrollPercent;
		readonly double newHorizontalScrollPercent;
		readonly double oldVerticalScrollPercent;
		readonly double newVerticalScrollPercent;
		public ChartScrollDirection ScrollDirection { get { return scrollDirection; } }
		public double OldHorizontalScrollPercent { get { return oldHorizontalScrollPercent; } }
		public double NewHorizontalScrollPercent { get { return newHorizontalScrollPercent; } }
		public double OldVerticalScrollPercent { get { return oldVerticalScrollPercent; } }
		public double NewVerticalScrollPercent { get { return newVerticalScrollPercent; } }
		internal ChartScroll3DEventArgs(ChartScrollDirection scrollDirection, double oldHorizontalScrollPercent, double newHorizontalScrollPercent, double oldVerticalScrollPercent, double newVerticalScrollPercent) {
			this.scrollDirection = scrollDirection;
			this.oldHorizontalScrollPercent = oldHorizontalScrollPercent;
			this.newHorizontalScrollPercent = newHorizontalScrollPercent;
			this.oldVerticalScrollPercent = oldVerticalScrollPercent;
			this.newVerticalScrollPercent = newVerticalScrollPercent;
		}
	}
	public struct RangeInfo {
		readonly double min;
		readonly double max;
		readonly object minValue;
		readonly object maxValue;
		public double Min { get { return min; } }
		public double Max { get { return max; } }
		public object MinValue { get { return minValue; } }
		public object MaxValue { get { return maxValue; } }
		internal RangeInfo(AxisRangeInfo info) {
			min = info.Min;
			max = info.Max;
			minValue = info.MinValue;
			maxValue = info.MaxValue;
		}
	}
	public class ChartScrollEventArgs : EventArgs {
		ChartScrollOrientation scrollOrientation;
		ChartScrollEventType type;
		RangeInfo oldXRange;
		RangeInfo oldYRange;
		RangeInfo newXRange;
		RangeInfo newYRange;
		AxisBase axisX;
		AxisBase axisY;
		public ChartScrollOrientation ScrollOrientation { get { return scrollOrientation; } }
		public ChartScrollEventType Type { get { return type; } }
		[Obsolete("This property is obsolete now. Use OldXRange instead."),]
		public AxisRange OldAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use OldYRange instead."),]
		public AxisRange OldAxisYRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewXRange instead."),]
		public AxisRange NewAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewYRange instead."),]
		public AxisRange NewAxisYRange { get { return null; } }
		public RangeInfo OldXRange { get { return oldXRange; } }
		public RangeInfo OldYRange { get { return oldYRange; } }
		public RangeInfo NewXRange { get { return newXRange; } }
		public RangeInfo NewYRange { get { return newYRange; } }
		public AxisBase AxisX { get { return axisX; } }
		public AxisBase AxisY { get { return axisY; } }
		internal ChartScrollEventArgs(ChartScrollOrientation scrollOrientation, ChartScrollEventType type, AxisRangeInfo oldAxisXRange, AxisRangeInfo oldAxisYRange, AxisRangeInfo newAxisXRange, AxisRangeInfo newAxisYRange, AxisBase axisX, AxisBase axisY) {
			this.scrollOrientation = scrollOrientation;
			this.type = type;
			this.axisX = axisX;
			this.axisY = axisY;
			this.oldXRange = new RangeInfo(oldAxisXRange);
			this.oldYRange = new RangeInfo(oldAxisYRange);
			this.newXRange = new RangeInfo(newAxisXRange);
			this.newYRange = new RangeInfo(newAxisYRange);
		}
	}
	public class ChartZoom3DEventArgs : EventArgs {
		readonly int oldZoomPercent;
		readonly int newZoomPercent;
		readonly ChartZoomEventType type;
		public int OldZoomPercent { get { return oldZoomPercent; } }
		public int NewZoomPercent { get { return newZoomPercent; } }
		public ChartZoomEventType Type { get { return type; } }
		internal ChartZoom3DEventArgs(int oldZoomPercent, int newZoomPercent, ChartZoomEventType type) {
			this.oldZoomPercent = oldZoomPercent;
			this.newZoomPercent = newZoomPercent;
			this.type = type;
		}
	}
	public class ChartZoomEventArgs : EventArgs {
		readonly ChartZoomEventType type;
		RangeInfo oldXRange;
		RangeInfo oldYRange;
		RangeInfo newXRange;
		RangeInfo newYRange;
		AxisBase axisX;
		AxisBase axisY;
		public ChartZoomEventType Type { get { return type; } }
		[Obsolete("This property is obsolete now. Use OldXRange instead."),]
		public AxisRange OldAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use OldYRange instead."),]
		public AxisRange OldAxisYRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewXRange instead."),]
		public AxisRange NewAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewYRange instead."),]
		public AxisRange NewAxisYRange { get { return null; } }
		public RangeInfo OldXRange { get { return oldXRange; } }
		public RangeInfo OldYRange { get { return oldYRange; } }
		public RangeInfo NewXRange { get { return newXRange; } }
		public RangeInfo NewYRange { get { return newYRange; } }
		public AxisBase AxisX { get { return axisX; } }
		public AxisBase AxisY { get { return axisY; } }
		internal ChartZoomEventArgs(ChartZoomEventType type, AxisRangeInfo oldAxisXRange, AxisRangeInfo oldAxisYRange, AxisRangeInfo newAxisXRange, AxisRangeInfo newAxisYRange, AxisBase axisX, AxisBase axisY) {
			this.type = type;
			this.axisX = axisX;
			this.axisY = axisY;
			this.oldXRange = new RangeInfo(oldAxisXRange);
			this.oldYRange = new RangeInfo(oldAxisYRange);
			this.newXRange = new RangeInfo(newAxisXRange);
			this.newYRange = new RangeInfo(newAxisYRange);
		}
	}
	public class PieSeriesPointExplodedEventArgs : EventArgs {
		Series series;
		SeriesPoint seriesPoint;
		bool exploded;
		bool dragged;
		public Series Series { get { return series; } }
		public SeriesPoint Point { get { return seriesPoint; } }
		public bool Exploded { get { return exploded; } }
		public bool Dragged { get { return dragged; } }
		internal PieSeriesPointExplodedEventArgs(Series series, SeriesPoint seriesPoint, bool exploded, bool dragged)
			: base() {
			this.series = series;
			this.seriesPoint = seriesPoint;
			this.exploded = exploded;
			this.dragged = dragged;
		}
	}
	public class AxisRangeChangedEventArgs : EventArgs {
		readonly AxisBase axis;
		readonly ValueChangeInfo<object> minChange;
		readonly ValueChangeInfo<object> maxChange;
		readonly ValueChangeInfo<double> minInternalChange;
		readonly ValueChangeInfo<double> maxInternalChange;
		bool cancel = false;
		public AxisBase Axis { get { return axis; } }
		public ValueChangeInfo<object> MinChange { get { return minChange; } }
		public ValueChangeInfo<object> MaxChange { get { return maxChange; } }
		public ValueChangeInfo<double> MinInternalChange { get { return minInternalChange; } }
		public ValueChangeInfo<double> MaxInternalChange { get { return maxInternalChange; } }
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
		internal AxisRangeChangedEventArgs(AxisBase axis,
										   ValueChangeInfo<object> minChange,
										   ValueChangeInfo<object> maxChange,
										   ValueChangeInfo<double> minInternalChange,
										   ValueChangeInfo<double> maxInternalChange) {
			this.axis = axis;
			this.minChange = minChange;
			this.maxChange = maxChange;
			this.minInternalChange = minInternalChange;
			this.maxInternalChange = maxInternalChange;
			this.cancel = false;
		}
	}
	public class ValueChangeInfo<T> {
		readonly T oldValue;
		readonly T newValue;
		public T OldValue {
			get {
				return oldValue;
			}
		}
		public T NewValue {
			get {
				return newValue;
			}
		}
		internal ValueChangeInfo(T value) {
			this.oldValue = value;
			this.newValue = value;
		}
		internal ValueChangeInfo(T oldValue, T newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public abstract class AxisScaleChangedEventArgs : EventArgs {
		readonly AxisBase axis;
		readonly ValueChangeInfo<ScaleMode> scaleModeChange;
		public AxisBase Axis {
			get {
				return axis;
			}
		}
		public ValueChangeInfo<ScaleMode> ScaleModeChange {
			get {
				return scaleModeChange;
			}
		}
		public AxisScaleChangedEventArgs(AxisBase axis, ValueChangeInfo<ScaleMode> scaleModeChange) {
			this.axis = axis;
			this.scaleModeChange = scaleModeChange;
		}
	}
	public class DateTimeScaleChangedEventArgs : AxisScaleChangedEventArgs {
		readonly ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange;
		readonly ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange;
		readonly ValueChangeInfo<double> gridSpacingChange;
		public ValueChangeInfo<DateTimeMeasureUnit> MeasureUnitChange {
			get {
				return measureUnitChange;
			}
		}
		public ValueChangeInfo<DateTimeGridAlignment> GridAlignmentChange {
			get {
				return gridAlignmentChange;
			}
		}
		public ValueChangeInfo<double> GridSpacingChange {
			get {
				return gridSpacingChange;
			}
		}
		public DateTimeScaleChangedEventArgs(AxisBase axis,
											 ValueChangeInfo<ScaleMode> scaleModeChange,
											 ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange,
											 ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange,
											 ValueChangeInfo<double> gridSpacingChange)
			: base(axis, scaleModeChange) {
			this.measureUnitChange = measureUnitChange;
			this.gridAlignmentChange = gridAlignmentChange;
			this.gridSpacingChange = gridSpacingChange;
		}
	}
	public class NumericScaleChangedEventArgs : AxisScaleChangedEventArgs {
		readonly ValueChangeInfo<double> measureUnitChange;
		readonly ValueChangeInfo<double> gridAlignmentChange;
		readonly ValueChangeInfo<double> gridSpacingChange;
		public ValueChangeInfo<double> MeasureUnitChange {
			get {
				return measureUnitChange;
			}
		}
		public ValueChangeInfo<double> GridAlignmentChange {
			get {
				return gridAlignmentChange;
			}
		}
		public ValueChangeInfo<double> GridSpacingChange {
			get {
				return gridSpacingChange;
			}
		}
		public NumericScaleChangedEventArgs(AxisBase axis,
											ValueChangeInfo<ScaleMode> scaleModeChange,
											ValueChangeInfo<double> measureUnitChange,
											ValueChangeInfo<double> gridAlignmentChange,
											ValueChangeInfo<double> gridSpacingChange)
			: base(axis, scaleModeChange) {
			this.measureUnitChange = measureUnitChange;
			this.gridAlignmentChange = gridAlignmentChange;
			this.gridSpacingChange = gridSpacingChange;
		}
	}
	public class QueryCursorEventArgs : EventArgs {
		Cursor cursor;
		CursorType cursorType;
		public Cursor Cursor { get { return cursor; } set { cursor = value; } }
		public CursorType CursorType { get { return cursorType; } }
		internal QueryCursorEventArgs(Cursor cursor, CursorType cursorType) {
			this.cursor = cursor;
			this.cursorType = cursorType;
		}
	}
	public class CustomizeXAxisLabelsEventArgs : EventArgs {
		readonly AxisBase axis;
		bool staggered;
		public AxisBase Axis { get { return axis; } }
		public bool Staggered {
			get { return staggered; }
			set { staggered = value; }
		}
		internal CustomizeXAxisLabelsEventArgs(AxisBase axis, bool staggered) {
			this.axis = axis;
			this.staggered = staggered;
		}
	}
	public class CustomizeResolveOverlappingModeEventArgs : EventArgs {
		ResolveOverlappingMode resolveOverlappingMode;
		public ResolveOverlappingMode ResolveOverlappingMode { get { return resolveOverlappingMode; } set { resolveOverlappingMode = value; } }
		internal CustomizeResolveOverlappingModeEventArgs(ResolveOverlappingMode resolveOverlappingMode) {
			this.resolveOverlappingMode = resolveOverlappingMode;
		}
	}
	public class CustomizeSimpleDiagramLayoutEventArgs : EventArgs {
		int dimension;
		LayoutDirection layoutDirection;
		public int Dimension { get { return dimension; } set { dimension = value; } }
		public LayoutDirection LayoutDirection { get { return layoutDirection; } set { layoutDirection = value; } }
		internal CustomizeSimpleDiagramLayoutEventArgs(int dimension, LayoutDirection layoutDirection) {
			this.dimension = dimension;
			this.layoutDirection = layoutDirection;
		}
	}
	public class CustomizeLegendEventArgs : EventArgs {
		double maxHorizontalPercentage;
		double maxVerticalPercentage;
		readonly Legend legend;
		public double MaxHorizontalPercentage { get { return maxHorizontalPercentage; } set { maxHorizontalPercentage = value; } }
		public double MaxVerticalPercentage { get { return maxVerticalPercentage; } set { maxVerticalPercentage = value; } }
		public Legend Legend { get { return legend; } }
		internal CustomizeLegendEventArgs(Legend legend, double maxHorizontalPercentage, double maxVerticalPercentage) {
			this.legend = legend;
			this.maxHorizontalPercentage = maxHorizontalPercentage;
			this.maxVerticalPercentage = maxVerticalPercentage;
		}
	}
	public class PivotGridSeriesExcludedEventArgs : EventArgs {
		int actualSeriesCount;
		int availableSeriesCount;
		public int ActualSeriesCount { get { return actualSeriesCount; } }
		public int AvailableSeriesCount { get { return availableSeriesCount; } }
		internal PivotGridSeriesExcludedEventArgs(int actualSeriesCount, int availableSeriesCount) {
			this.actualSeriesCount = actualSeriesCount;
			this.availableSeriesCount = availableSeriesCount;
		}
	}
	public class PivotGridSeriesPointsExcludedEventArgs : EventArgs {
		readonly Series series;
		int actualSeriesPointCount;
		int availableSeriesPointCount;
		public Series Series { get { return series; } }
		public int ActualSeriesPointCount { get { return actualSeriesPointCount; } }
		public int AvailableSeriesPointCount { get { return availableSeriesPointCount; } }
		internal PivotGridSeriesPointsExcludedEventArgs(Series series, int actualSeriesPointCount, int availableSeriesPointCount) {
			this.series = series;
			this.actualSeriesPointCount = actualSeriesPointCount;
			this.availableSeriesPointCount = availableSeriesPointCount;
		}
	}
	public class CrosshairElementGroup {
		readonly CrosshairGroupHeaderElement headerElement;
		readonly List<CrosshairElement> crosshairElements;
		readonly CrosshairLabelInfoEx label;
		internal CrosshairLabelInfoEx Label { get { return label; } }
		public CrosshairGroupHeaderElement HeaderElement { get { return headerElement; } }
		public IList<CrosshairElement> CrosshairElements { get { return crosshairElements; } }
		internal CrosshairElementGroup(CrosshairGroupHeaderElement headerElement, List<CrosshairElement> crosshairElements, CrosshairLabelInfoEx label) {
			this.headerElement = headerElement;
			this.crosshairElements = crosshairElements;
			this.label = label;
		}
	}
	public class CustomDrawCrosshairEventArgs : EventArgs {
		readonly List<CrosshairElementGroup> crosshairElementGroups;
		readonly CrosshairLineElement crosshairLineElement;
		readonly List<CrosshairAxisLabelElement> crosshairAxisLabelElements;
		[
		Obsolete("The CustomDrawCrosshairEventArgs.CrosshairGroupHeaderElements property is now obsolete. Use the CustomDrawCrosshairEventArgs.CrosshairElementGroups.HeaderElement property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public IEnumerable<CrosshairGroupHeaderElement> CrosshairGroupHeaderElements {
			get {
				foreach (CrosshairElementGroup elementGroup in crosshairElementGroups)
					yield return elementGroup.HeaderElement;
			}
		}
		[
		Obsolete("The CustomDrawCrosshairEventArgs.CrosshairElements property is now obsolete. Use the CustomDrawCrosshairEventArgs.CrosshairElementGroups.CrosshairElements property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public IEnumerable<CrosshairElement> CrosshairElements {
			get {
				foreach (CrosshairElementGroup elementGroup in crosshairElementGroups)
					foreach (CrosshairElement element in elementGroup.CrosshairElements)
						yield return element;
			}
		}
		public IList<CrosshairElementGroup> CrosshairElementGroups { get { return crosshairElementGroups; } }
		public CrosshairLineElement CrosshairLineElement { get { return crosshairLineElement; } }
		public IEnumerable<CrosshairAxisLabelElement> CrosshairAxisLabelElements { get { return crosshairAxisLabelElements; } }
		internal CustomDrawCrosshairEventArgs(List<CrosshairElementGroup> crosshairElementGroups, CrosshairLineElement lineElement, List<CrosshairAxisLabelElement> axisLabelElements) {
			this.crosshairElementGroups = crosshairElementGroups;
			this.crosshairLineElement = lineElement;
			this.crosshairAxisLabelElements = axisLabelElements;
		}
	}
	public class LegendItemCheckedEventArgs : EventArgs {
		readonly ChartElement checkedElement;
		readonly bool newCheckState;
		public ChartElement CheckedElement {
			get { return checkedElement; }
		}
		public bool NewCheckState {
			get { return newCheckState; }
		}
		internal LegendItemCheckedEventArgs(ChartElement checkedElement, bool newCheckState) {
			this.checkedElement = checkedElement;
			this.newCheckState = newCheckState;
		}
	}
}
