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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum StockLevel {
		Low,
		High,
		Open,
		Close
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ReductionStockOptions : ChartElement {
		public static readonly Color DefaultColor = Color.Red;
		const StockLevel DefaultLevel = StockLevel.Close;
		Color color = DefaultColor;
		bool visible = true;
		StockLevel level = DefaultLevel;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ReductionStockOptionsColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ReductionStockOptions.Color"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return this.color; }
			set {
				if(value == Color.Empty)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectReductionColorValue));
				SendNotification(new ElementWillChangeNotification(this));
				this.color = value;
				RaiseControlChanged();
			}
		}			
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ReductionStockOptionsLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ReductionStockOptions.Level"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public StockLevel Level {
			get { return this.level; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.level = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ReductionStockOptionsVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ReductionStockOptions.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return this.visible; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.visible = value;
				RaiseControlChanged();
			}
		}
		internal ReductionStockOptions(ChartElement owner) : base(owner) {
		}
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Color")
				return ShouldSerializeColor();
			if(propertyName == "Level")
				return ShouldSerializeLevel();
			if(propertyName == "Visible")
				return ShouldSerializeVisible();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor() {
			return !color.Equals(DefaultColor);
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeLevel() {
			return this.level != DefaultLevel;
		}
		void ResetLevel() {
			Level = DefaultLevel;
		}
		bool ShouldSerializeVisible() {
			return !this.visible;
		}
		void ResetVisible() {
			Visible = true;
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ReductionStockOptions(null);
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColor() ||
				ShouldSerializeLevel() ||
				ShouldSerializeVisible();
		}
		public override string ToString() {
			return "(ReductionOptions)";
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			ReductionStockOptions options = obj as ReductionStockOptions;
			return options != null &&
				options.GetType().Equals(GetType()) &&
				options.color == this.color &&
				options.level == this.level &&
				options.visible == this.visible;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ReductionStockOptions options = obj as ReductionStockOptions;
			if(options == null)
				return;
			this.color = options.color;
			this.visible = options.visible;
			this.level = options.level;
		}
	}
	public abstract class FinancialSeriesViewBase : XYDiagramSeriesViewBase, IBarSeriesView, IFinancialSeriesView {
		const int LegendMarkerShadowSize = 1;	 
		const double DefaultLevelLineLength = 0.25;
		const int DefaultLineThickness = 2;
		internal const int ValueCount = 4;
		static readonly Color defaultColor = Color.Black;
		static readonly string[] valueCaptions = new String[ValueCount] {   ChartLocalizer.GetString(ChartStringId.LowValueMember),																   
																			ChartLocalizer.GetString(ChartStringId.HighValueMember),																   
																			ChartLocalizer.GetString(ChartStringId.OpenValueMember),																   
																			ChartLocalizer.GetString(ChartStringId.CloseValueMember)};
		internal static double GetLowValue(SeriesPoint seriesPoint) {
			return seriesPoint.Values[0];
		}
		internal static double GetHighValue(SeriesPoint seriesPoint) {
			return seriesPoint.Values[1];
		}
		internal static double GetOpenValue(SeriesPoint seriesPoint) {
			return seriesPoint.Values[2];
		}
		internal static double GetCloseValue(SeriesPoint seriesPoint) {
			return seriesPoint.Values[3];
		}
		double levelLineLength = DefaultLevelLineLength;
		LineStyle lineStyle;
		ReductionStockOptions reductionOptions;
		double IBarSeriesView.BarWidth { get { return levelLineLength * 2; } set { } }
		protected override Type PointInterfaceType { get { return typeof(IFinancialPoint); } }
		protected override int PixelsPerArgument { get { return 20; } }
		protected internal LineStyle LineStyle { get { return this.lineStyle; } }
		protected internal override int PointDimension { get { return ValueCount; } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal override Color ActualColor {
			get { return PaletteColorUsed ? defaultColor : Color; }
		}
		protected internal override ValueLevel[] SupportedValueLevels {
			get { return new ValueLevel[] { ValueLevel.Low, ValueLevel.High, ValueLevel.Open, ValueLevel.Close }; }
		}
		protected internal override ValueLevel DefaultValueLevel { get { return ValueLevel.Close; } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = "\nHigh: {HV" + GetDefaultFormat(Series.ValueScaleType) + "}\nLow: {LV" + GetDefaultFormat(Series.ValueScaleType) + "}" +
					("\nOpen: {OV" + GetDefaultFormat(Series.ValueScaleType) + "}") + ("\nClose: {CV" + GetDefaultFormat(Series.ValueScaleType) + "}");
				return argumentPattern + valuePattern;
			}
		}
		protected internal override string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.CloseValuePlaceholder + "}"; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(XYDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialSeriesViewBaseLevelLineLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialSeriesViewBase.LevelLineLength"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double LevelLineLength {
			get { return this.levelLineLength; }
			set {
				if(value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectStockLevelLineLengthValue));
				SendNotification(new ElementWillChangeNotification(this));
				this.levelLineLength = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialSeriesViewBaseLineThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialSeriesViewBase.LineThickness"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int LineThickness {
			get {
				return this.lineStyle.Thickness;
			}
			set {
				this.lineStyle.Thickness = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FinancialSeriesViewBaseReductionOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FinancialSeriesViewBase.ReductionOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ReductionStockOptions ReductionOptions { get { return this.reductionOptions; } }
		protected FinancialSeriesViewBase() {
			this.lineStyle = new LineStyle(this, DefaultLineThickness, false);
			this.reductionOptions = new ReductionStockOptions(this);
		}
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "LevelLineLength")
				return ShouldSerializeLevelLineLength();
			if(propertyName == "LineThickness")
				return ShouldSerializeLineThickness();
			if(propertyName == "ReductionOptions")
				return ShouldSerializeReductionOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLevelLineLength() {
			return LevelLineLength != DefaultLevelLineLength;
		}
		void ResetLevelLineLength() {
			LevelLineLength = DefaultLevelLineLength;
		}
		bool ShouldSerializeLineThickness() {
			return LineThickness != DefaultLineThickness;
		}
		void ResetLineThickness() {
			LineThickness = DefaultLineThickness;
		}
		bool ShouldSerializeReductionOptions() {
			return ReductionOptions.ShouldSerialize();
		}
		#endregion
		ProjectTransform CreateLegendTransform(Rectangle rect) {
			return new ProjectTransform(rect);
		}
		protected abstract void RenderFinancial(IRenderer renderer, FinancialSeriesPointLayout financialPointLayout, Color color);
		protected abstract FinancialSeriesPointLayout CreateSeriesPointLayout(RefinedPointData pointData, int lineThickness);
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint pointInfo) {
			IFinancialPoint financialPoint = pointInfo;
			yield return financialPoint.Low;
			yield return financialPoint.High;
			yield return financialPoint.Open;
			yield return financialPoint.Close;
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Max(Math.Max(financialPoint.Low, financialPoint.High), Math.Max(financialPoint.Open, financialPoint.Close));
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Min(Math.Min(financialPoint.Low, financialPoint.High), Math.Min(financialPoint.Open, financialPoint.Close));
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Min(Math.Min(Math.Abs(financialPoint.Low), Math.Abs(financialPoint.High)), Math.Min(Math.Abs(financialPoint.Open), Math.Abs(financialPoint.Close)));
		}
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			Rectangle inflatedRect = GraphicUtils.InflateRect(bounds, -1, -1);
			return GraphicUtils.RoundRectangle(CorrectRectangleByShadowSize(inflatedRect, LegendMarkerShadowSize));
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			if (!GraphicUtils.CheckIsSizePositive(bounds.Size))
				return;
			int indentH = (int)Math.Round(bounds.Width / 3.5);
			int stockOffset = (int)Math.Round(bounds.Height / 4.0);
			int levelLength = (int)Math.Round(bounds.Width / 7.5);
			int levelIndent = (int)Math.Round(bounds.Height / 4.5);
			int lineThickness = 2;
			MatrixTransform matrixTransform = CreateLegendTransform(bounds);
			Point low1 = new Point(indentH, stockOffset);
			Point high1 = new Point(low1.X, bounds.Height + 1);
			Point open1 = new Point(low1.X, low1.Y + levelIndent);
			Point close1 = new Point(low1.X, bounds.Height + 1 - levelIndent);
			FinancialSeriesPointLayout layout1 = CreateSeriesPointLayout(null, lineThickness);
			layout1.CalculateForLegend(matrixTransform, low1, high1, open1, close1, levelLength);
			Point low2 = new Point(bounds.Width + 1 - indentH, 0);
			Point high2 = new Point(low2.X, bounds.Height + 1 - stockOffset);
			Point open2 = new Point(low2.X, low2.Y + levelIndent);
			Point close2 = new Point(low2.X, high2.Y - levelIndent);
			FinancialSeriesPointLayout layout2 = CreateSeriesPointLayout(null, lineThickness);
			layout2.CalculateForLegend(matrixTransform, low2, high2, open2, close2, levelLength);
			FinancialDrawOptions financialDrawOptions = (FinancialDrawOptions)seriesPointDrawOptions;
			RenderShadow(renderer, bounds, layout1, financialDrawOptions);
			RenderShadow(renderer, bounds, layout2, financialDrawOptions);
			RenderFinancial(renderer, layout1, financialDrawOptions.Color);
			Color layout2Color = financialDrawOptions.ReductionOptions.Visible ? financialDrawOptions.ReductionOptions.Color : financialDrawOptions.Color;
			RenderFinancial(renderer, layout2, layout2Color);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return
				valueLevel == ValueLevelInternal.High ||
				valueLevel == ValueLevelInternal.Low ||
				valueLevel == ValueLevelInternal.Open ||
				valueLevel == ValueLevelInternal.Close;
		}
		protected RectangleF CorrectRectangleByShadowSize(RectangleF rect, int shadowSize) {
			return rect = new RectangleF(rect.Left, rect.Top, rect.Width - Shadow.GetActualSize(shadowSize), rect.Height - Shadow.GetActualSize(shadowSize));
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLevelLineLength() ||
				ShouldSerializeLineThickness() ||
				ShouldSerializeReductionOptions();
		}
		protected internal override void CalculateAnnotationsAnchorPointsLayout(XYDiagramAnchorPointLayoutList anchorPointLayoutList) {
			foreach (RefinedPointData pointData in anchorPointLayoutList.SeriesData) {
				SeriesPoint seriesPoint = pointData.SeriesPoint as SeriesPoint;
				RefinedPoint refinedPoint = pointData.RefinedPoint;
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					foreach (Annotation annotation in seriesPoint.Annotations) {
						XYDiagramMappingBase mapping = anchorPointLayoutList.GetMapping(refinedPoint.Argument, GetRefinedPointMax((RefinedPoint)refinedPoint), annotation.ScrollingSupported);
						if (mapping != null)
							anchorPointLayoutList.Add(new AnnotationLayout(annotation, mapping.GetScreenPointNoRound(refinedPoint.Argument, GetRefinedPointMax((RefinedPoint)refinedPoint)), refinedPoint));
					}
				}
				foreach (RefinedPoint child in pointData.RefinedPoint.Children) {
					seriesPoint = child.SeriesPoint as SeriesPoint;
					if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
						foreach (Annotation annotation in seriesPoint.Annotations) {
							XYDiagramMappingBase mapping = anchorPointLayoutList.GetMapping(refinedPoint.Argument, GetRefinedPointMax((RefinedPoint)refinedPoint), annotation.ScrollingSupported);
							if (mapping != null)
								anchorPointLayoutList.Add(new AnnotationLayout(annotation, mapping.GetScreenPointNoRound(refinedPoint.Argument, GetRefinedPointMax((RefinedPoint)refinedPoint)), pointData.RefinedPoint));
						}
					}
				}
			}
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			return mapping.GetScreenPointNoRound(pointData.RefinedPoint.Argument, ((IStackedPoint)pointData.RefinedPoint).MaxValue);
		}
		protected internal override object[] GenerateRandomValues(int indexOfRandomPoint, int randomPointCount, ScaleType valueScaleType) {
			double high = random.NextDouble();
			if (high < 0.5)
				high += 0.5;
			high = Math.Round(high * 10, 1);
			double low = random.NextDouble();
			if (low > 0.5)
				low -= 0.5;
			low = Math.Round(low * high, 1);
			double open = Math.Round(random.NextDouble() * (high - low) + low, 1);
			double close = Math.Round(random.NextDouble() * (high - low) + low, 1);
			return new object[] { low, high, open, close };
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			IFinancialPoint pointInfo = pointData.RefinedPoint;
			FinancialDrawOptions financialDrawOptions = pointData.DrawOptions as FinancialDrawOptions;
			if (pointInfo == null || financialDrawOptions == null)
				return null;
			int lineThickness = financialDrawOptions.LineThickness;
			bool isSelected = pointData.SelectionState != SelectionState.Normal;
			if (isSelected)
				lineThickness += 2;
			FinancialSeriesPointLayout pointLayout = CreateSeriesPointLayout(pointData, lineThickness);
			pointLayout.Calculate(diagramMapping, pointInfo.Argument,
				pointInfo.Low, pointInfo.High, pointInfo.Open, pointInfo.Close, financialDrawOptions);
			return pointLayout;
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipFinancialValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.FinancialViewPointPatterns;
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new StockSeriesLabel();
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			if (pointLayout != null)
				RenderFinancial(renderer, (FinancialSeriesPointLayout)pointLayout, drawOptions.Color);
		}
		public override string GetValueCaption(int index) {
			ChartDebug.Assert(index <= valueCaptions.Length - 1, "Invalid value caption index");
			return valueCaptions[index];
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			FinancialSeriesViewBase view = (FinancialSeriesViewBase)obj;
			return
				view.levelLineLength == this.levelLineLength &&
				view.lineStyle.Equals(this.lineStyle) &&
				view.reductionOptions.Equals(this.reductionOptions);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FinancialSeriesViewBase view = obj as FinancialSeriesViewBase;
			if (view == null)
				return;
			this.levelLineLength = view.levelLineLength;
			this.lineStyle.Assign(view.lineStyle);
			this.reductionOptions.Assign(view.reductionOptions);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class FinancialSeriesPointLayout : SeriesPointLayout {
		int lineThickness;
		int levelLineLengthOpen;
		int levelLineLengthClose;
		Point low;
		Point high;
		Point open;
		Point close;
		protected int LineThickness { get { return lineThickness; } }
		protected int LevelLineLengthOpen { get { return levelLineLengthOpen; } }
		protected int LevelLineLengthClose { get { return levelLineLengthClose; } }
		protected Point Low { get { return low; } }
		protected Point High { get { return high; } }
		protected Point Open { get { return open; } }
		protected Point Close { get { return close; } }
		protected int NearCorrection { get { return (lineThickness % 2 == 0 ? lineThickness : lineThickness - 1) / 2; } }
		protected int FarCorrection { get { return lineThickness % 2 == 0 ? lineThickness / 2 - 1 : (lineThickness - 1) / 2; } }
		public FinancialSeriesPointLayout(RefinedPointData pointData, int lineThickness) : base(pointData) {
			this.lineThickness = lineThickness;
		}
		protected abstract void CalculateInternal(MatrixTransform matrixTransform);
		public void Calculate(XYDiagramMappingBase diagramMapping, double argument, double lowValue, double highValue, double openValue, double closeValue, FinancialDrawOptions pointDrawOptions) {
			DiagramPoint ptCenter = diagramMapping.GetInterimPoint(argument, 0, false, true);
			DiagramPoint ptOpen = diagramMapping.GetInterimPoint(argument - pointDrawOptions.LevelLineLength, 0, false, true);
			DiagramPoint ptClose = diagramMapping.GetInterimPoint(argument + pointDrawOptions.LevelLineLength, 0, false, true);
			this.levelLineLengthOpen = (int)Math.Abs(ptCenter.X - ptOpen.X);
			this.levelLineLengthClose = (int)Math.Abs(ptCenter.X - ptClose.X);
			this.low = (Point)diagramMapping.GetInterimPoint(argument, lowValue, false, true);
			this.high = (Point)diagramMapping.GetInterimPoint(argument, highValue, false, true);
			this.open = (Point)diagramMapping.GetInterimPoint(argument, openValue, false, true);
			this.close = (Point)diagramMapping.GetInterimPoint(argument, closeValue, false, true);
			CalculateInternal(diagramMapping.Transform);
		}
		public void CalculateForLegend(MatrixTransform matrixTransform, Point low, Point high, Point open, Point close, int levelLineLength) {
			this.low = low;
			this.high = high;
			this.open = open;
			this.close = close;
			this.levelLineLengthOpen = levelLineLength;
			this.levelLineLengthClose = levelLineLength;
			CalculateInternal(matrixTransform);
		}
	}
}
