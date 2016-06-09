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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.Data.Export;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IXlsChartAxisContainer
	public interface IXlsChartAxisContainer {
		void Add(XlsChartAxisBuilder item);
	}
	#endregion
	#region XlsChartAxisGroupBuilder
	public class XlsChartAxisGroupBuilder : IXlsChartBuilder, IXlsChartAxisContainer, IXlsChartViewContainer, IXlsChartFrameContainer, IXlsChartTextContainer {
		#region Fields
		List<XlsChartAxisBuilder> axisBuilders = new List<XlsChartAxisBuilder>();
		List<XlsChartViewBuilder> viewBuilders = new List<XlsChartViewBuilder>();
		XlsChartFrame plotAreaFormat;
		List<XlsChartTextBuilder> axisTitles = new List<XlsChartTextBuilder>();
		#endregion
		#region Properties
		public XlsChartFrame PlotAreaFormat { get { return plotAreaFormat; } }
		public bool ApplyPlotAreaFormat { get; set; }
		public List<XlsChartTextBuilder> AxisTitles { get { return axisTitles; } }
		public List<XlsChartViewBuilder> ViewBuilders { get { return viewBuilders; } }
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			contentBuilder.AxisGroups.Add(this);
			bool isPrimaryAxisGroup = object.ReferenceEquals(contentBuilder.CurrentChart.PrimaryAxes, contentBuilder.CurrentAxisGroup);
			if (!isPrimaryAxisGroup && viewBuilders.Count == 0)
				return;
			if (isPrimaryAxisGroup)
				SetupPlotArea(contentBuilder.CurrentChart.PlotArea);
			bool isPercentStacked = IsPercentStacked();
			bool isXYView = IsXYView();
			foreach (XlsChartAxisBuilder item in axisBuilders)
				item.Build(contentBuilder, isPercentStacked, isXYView);
			AxisGroup axisGroup = contentBuilder.CurrentAxisGroup;
			SetupCrosses(axisGroup);
			SetupAxisTitles(contentBuilder, axisGroup);
			List<XlsChartViewBuilder> viewsByZOrder = new List<XlsChartViewBuilder>(viewBuilders);
			viewsByZOrder.Sort(CompareViewZOrder);
			foreach (XlsChartViewBuilder item in viewsByZOrder)
				item.Build(contentBuilder);
		}
		#endregion
		#region IXlsChartAxisContainer Members
		void IXlsChartAxisContainer.Add(XlsChartAxisBuilder item) {
			axisBuilders.Add(item);
		}
		#endregion
		#region IXlsChartViewContainer Members
		void IXlsChartViewContainer.Add(XlsChartViewBuilder item) {
			viewBuilders.Add(item);
		}
		#endregion
		#region IXlsChartFrameContainer Members
		void IXlsChartFrameContainer.Add(XlsChartFrame frame) {
			this.plotAreaFormat = frame;
		}
		#endregion
		#region IXlsChartTextContainer Members
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			this.axisTitles.Add(item);
		}
		#endregion
		TitleOptions GetAxisTitle(AxisGroup axisGroup, int index) {
			AxisBase axis = axisGroup.GetItem(index);
			if (axis != null)
				return axis.Title;
			return null;
		}
		void SetupAxisTitles(XlsContentBuilder contentBuilder, AxisGroup axisGroup) {
			if (axisGroup == null || axisGroup.Count == 0)
				return;
			foreach (XlsChartTextBuilder builder in axisTitles) {
				if (builder.ObjectLink.IsArgumentAxisTitle)
					builder.SetupTitle(contentBuilder, GetAxisTitle(axisGroup, 0));
				else if (builder.ObjectLink.IsValueAxisTitle)
					builder.SetupTitle(contentBuilder, GetAxisTitle(axisGroup, 1));
				else if (builder.ObjectLink.IsSeriesAxisTitle)
					builder.SetupTitle(contentBuilder, GetAxisTitle(axisGroup, 2));
			}
		}
		void SetupPlotArea(PlotArea plotArea) {
			if (ApplyPlotAreaFormat && PlotAreaFormat != null)
				PlotAreaFormat.SetupShapeProperties(plotArea.ShapeProperties);
			else {
				plotArea.ShapeProperties.Fill = DrawingFill.None;
				plotArea.ShapeProperties.Outline.Fill = DrawingFill.None;
			}
		}
		void SetupCrosses(AxisGroup axisGroup) {
			if (axisGroup == null || axisGroup.Count == 0)
				return;
			axisGroup[0].CrossesAxis = axisGroup[1];
			axisGroup[1].CrossesAxis = axisGroup[0];
			if (axisGroup.Count == 3)
				axisGroup[2].CrossesAxis = axisGroup[1];
			XlsChartAxisBuilder axisBuilder = FindAxisBuilder(axisGroup[0]);
			if (axisBuilder != null)
				axisBuilder.SetupCrosses(axisGroup[1]);
			axisBuilder = FindAxisBuilder(axisGroup[1]);
			if (axisBuilder != null) {
				axisBuilder.SetupCrosses(axisGroup[0]);
				if (axisGroup.Count == 3)
					axisBuilder.SetupCrosses(axisGroup[2]);
			}
		}
		XlsChartAxisBuilder FindAxisBuilder(AxisBase axis) {
			foreach (XlsChartAxisBuilder item in axisBuilders)
				if (object.ReferenceEquals(item.Axis, axis))
					return item;
			return null;
		}
		bool IsPercentStacked() {
			if (viewBuilders.Count > 0) {
				XlsChartViewBuilder viewBuilder = viewBuilders[0];
				if (viewBuilder.ViewType == ChartViewType.Bar || viewBuilder.ViewType == ChartViewType.Bar3D)
					return viewBuilder.BarGrouping == BarChartGrouping.PercentStacked;
				return viewBuilder.Grouping == ChartGrouping.PercentStacked;
			}
			return false;
		}
		bool IsXYView() {
			if (viewBuilders.Count > 0) {
				XlsChartViewBuilder viewBuilder = viewBuilders[0];
				return viewBuilder.ViewType == ChartViewType.Bubble || viewBuilder.ViewType == ChartViewType.Scatter;
			}
			return false;
		}
		int CompareViewZOrder(XlsChartViewBuilder x, XlsChartViewBuilder y) {
			if (x == null) {
				if (y == null)
					return 0;
				else
					return -1;
			}
			else {
				if (y == null)
					return 1;
				if (x.ZOrder < y.ZOrder)
					return -1;
				if (x.ZOrder > y.ZOrder)
					return 1;
				if (x.ViewIndex < y.ViewIndex)
					return -1;
				if (x.ViewIndex > y.ViewIndex)
					return 1;
				return 0;
			}
		}
	}
	#endregion
	#region XlsChartAxisBuilder
	public class XlsChartAxisBuilder : XlsChartLineFormatContainer, IXlsChartBuilder, IXlsChartAreaFormat, IXlsChartGraphicFormat, IXlsChartExtPropertyVisitor, IXlsChartFontX, IXlsChartTextFormatContainer, IXlsChartTextContainer {
		#region Fields
		bool isPercentStacked;
		bool isXYView;
		XlsChartAreaFormat areaFormat = new XlsChartAreaFormat();
		XlsChartGraphicFormat graphicFormat = new XlsChartGraphicFormat();
		XlsChartFontX fontX = new XlsChartFontX();
		AxisBase result = null;
		XlsChartTextFormat textFormat = null;
		XlsChartTextBuilder text;
		#endregion
		#region Properties
		public AxisDataType AxisType { get; set; }
		public bool IsDateAxis { get; set; }
		public AxisCrossBetween CrossBetween { get; set; }
		public AxisOrientation Orientation { get; set; }
		public AxisCrosses Crosses { get; set; }
		public double CrossesValue { get; set; }
		public int TickLabelSkip { get; set; }
		public int TickMarkSkip { get; set; }
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public double MajorUnit { get; set; }
		public double MinorUnit { get; set; }
		public bool Auto { get; set; }
		public bool AutoMin { get; set; }
		public bool AutoMax { get; set; }
		public TimeUnits BaseTimeUnit { get; set; }
		public TimeUnits MajorTimeUnit { get; set; }
		public TimeUnits MinorTimeUnit { get; set; }
		public bool IsLogScale { get; set; }
		public double LogBase { get; set; }
		public int LabelOffset { get; set; }
		public LabelAlignment LabelAlign { get; set; }
		public bool ApplyNumberFormat { get; set; }
		public int NumberFormatId { get; set; }
		public string NumberFormatCode { get; set; }
		public TickMark MajorTickMark { get; set; }
		public TickMark MinorTickMark { get; set; }
		public TickLabelPosition LabelPos { get; set; }
		public bool LabelTransparent { get; set; }
		public Color TextColor { get; set; }
		public bool LabelAutoColor { get; set; }
		public bool LabelAutoMode { get; set; }
		public DrawingTextVerticalTextType LabelVerticalText { get; set; }
		public int LabelRotationAngle { get; set; }
		public bool LabelAutoRotate { get; set; }
		public XlReadingOrder TextReadingOrder { get; set; }
		public DisplayUnitType UnitType { get; set; }
		public double CustomUnit { get; set; }
		public bool ShowUnitLabel { get; set; }
		public bool NoMultiLevelLabels { get; set; }
		public XlsChartAreaFormat AreaFormat { get { return areaFormat; } }
		public XlsChartGraphicFormat GraphicFormat { get { return graphicFormat; } }
		public IXlsChartAxisContainer Container { get; set; }
		public AxisBase Axis { get { return result; } }
		public XlsChartFontX FontX { get { return fontX; } }
		public XlsChartTextFormat TextFormat { get { return textFormat; } }
		public XlsChartTextBuilder Text { get { return text; } }
		#endregion
		public XlsChartAxisBuilder()
			: base() {
			Auto = true;
			AutoMin = true;
			AutoMax = true;
			TickLabelSkip = 1;
			TickMarkSkip = 1;
			Orientation = AxisOrientation.MinMax;
			LogBase = 10.0;
			LabelAlign = LabelAlignment.Center;
			LabelOffset = 100;
			BaseTimeUnit = TimeUnits.Auto;
			MajorTimeUnit = TimeUnits.Auto;
			MinorTimeUnit = TimeUnits.Auto;
			MajorTickMark = TickMark.Cross;
			MinorTickMark = TickMark.Cross;
			LabelPos = TickLabelPosition.NextTo;
			LabelTransparent = true;
			LabelAutoColor = true;
			LabelAutoMode = true;
			LabelAutoRotate = true;
			LabelVerticalText = DrawingTextVerticalTextType.Horizontal;
			LabelRotationAngle = 0;
			TextReadingOrder = XlReadingOrder.Context;
			UnitType = DisplayUnitType.None;
			ShowUnitLabel = false;
			NumberFormatCode = string.Empty;
		}
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (Container != null)
				Container.Add(this);
		}
		#endregion
		public void Build(XlsContentBuilder contentBuilder, bool isPercentStacked, bool isXYView) {
			if (contentBuilder.CurrentChart == null || contentBuilder.CurrentAxisGroup == null)
				return;
			this.isPercentStacked = isPercentStacked;
			this.isXYView = isXYView;
			this.result = CreateAxis(contentBuilder);
			SetupAxis(contentBuilder, result);
			contentBuilder.CurrentAxisGroup.Add(result);
		}
		#region Utils
		AxisBase CreateAxis(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			switch (AxisType) {
				case AxisDataType.Agrument:
					if (isXYView)
						return new ValueAxis(chart);
					if (IsDateAxis)
						return new DateAxis(chart);
					return new CategoryAxis(chart);
				case AxisDataType.Value:
					return new ValueAxis(chart);
				default:
					return new SeriesAxis(chart);
			}
		}
		public void SetupCrosses(AxisBase axis) {
			axis.BeginUpdate();
			try {
				axis.Crosses = Crosses;
				axis.CrossesValue = CrossesValue;
				SetupCrossBetween(axis as ValueAxis);
			}
			finally {
				axis.EndUpdate();
			}
		}
		void SetupCrossBetween(ValueAxis axis) {
			if (axis == null || AxisType != AxisDataType.Agrument || isXYView)
				return;
			axis.CrossBetween = CrossBetween;
		}
		void SetupAxis(XlsContentBuilder contentBuilder, AxisBase axis) {
			axis.BeginUpdate();
			try {
				axis.MajorTickMark = MajorTickMark;
				axis.MinorTickMark = MinorTickMark;
				axis.TickLabelPos = LabelPos;
				SetupScaling(axis.Scaling);
				SetupTickSkip(axis as AxisTickBase);
				SetupMMUnits(axis as AxisMMUnitsBase);
				SetupCategoryAxis(axis as CategoryAxis);
				SetupDateAxis(axis as DateAxis);
				SetupValueAxis(contentBuilder, axis as ValueAxis);
				SetupNumberFormat(axis);
				SetupTextProperties(contentBuilder, axis);
				SetupAxisLineFormat(axis);
				SetupMajorGridlinesFormat(axis);
				SetupMinorGridlinesFormat(axis);
				SetupSurfaceFormats(contentBuilder, axis);
			}
			finally {
				axis.EndUpdate();
			}
		}
		void SetupAxisLineFormat(AxisBase axis) {
			XlsChartShapeFormat shapeFormat = ShapeFormats.FindBy(XlsChartDefs.AxisContext);
			XlsChartLineFormat format = LineFormats[XlsChartDefs.AxisIndex];
			if (shapeFormat != null) {
				axis.Delete = false;
				shapeFormat.SetupShapeProperties(axis.ShapeProperties);
			}
			else if (format.Apply) {
				axis.Delete = !format.AxisVisible && format.LineStyle == XlsChartLineStyle.None;
				format.SetupShapeProperties(axis.ShapeProperties);
			}
		}
		void SetupMajorGridlinesFormat(AxisBase axis) {
			XlsChartShapeFormat shapeFormat = ShapeFormats.FindBy(XlsChartDefs.MajorGridlinesContext);
			XlsChartLineFormat format = LineFormats[XlsChartDefs.MajorGridlinesIndex];
			if (shapeFormat != null) {
				axis.ShowMajorGridlines = true;
				shapeFormat.SetupShapeProperties(axis.MajorGridlines);
			}
			else if (format.Apply) {
				axis.ShowMajorGridlines = format.LineStyle != XlsChartLineStyle.None;
				format.SetupShapeProperties(axis.MajorGridlines);
			}
		}
		void SetupMinorGridlinesFormat(AxisBase axis) {
			XlsChartShapeFormat shapeFormat = ShapeFormats.FindBy(XlsChartDefs.MinorGridlinesContext);
			XlsChartLineFormat format = LineFormats[XlsChartDefs.MinorGridlinesIndex];
			if (shapeFormat != null) {
				axis.ShowMinorGridlines = true;
				shapeFormat.SetupShapeProperties(axis.MinorGridlines);
			}
			else if (format.Apply) {
				axis.ShowMinorGridlines = format.LineStyle != XlsChartLineStyle.None;
				format.SetupShapeProperties(axis.MinorGridlines);
			}
		}
		void SetupSurfaceFormats(XlsContentBuilder contentBuilder, AxisBase axis) {
			Chart chart = contentBuilder.CurrentChart;
			XlsChartShapeFormat shapeFormat = ShapeFormats.FindBy(XlsChartDefs.SurfaceContext);
			XlsChartLineFormat format = LineFormats[XlsChartDefs.SurfaceIndex];
			if (AxisType == AxisDataType.Agrument) {
				if (shapeFormat != null) {
					shapeFormat.SetupShapeProperties(chart.BackWall.ShapeProperties);
					shapeFormat.SetupShapeProperties(chart.SideWall.ShapeProperties);
				}
				else {
					if (format.Apply) {
						format.SetupShapeProperties(chart.BackWall.ShapeProperties);
						format.SetupShapeProperties(chart.SideWall.ShapeProperties);
					}
					if (areaFormat.Apply) {
						areaFormat.SetupShapeProperties(chart.BackWall.ShapeProperties);
						areaFormat.SetupShapeProperties(chart.SideWall.ShapeProperties);
					}
				}
			}
			else if (AxisType == AxisDataType.Value) {
				if (shapeFormat != null) {
					shapeFormat.SetupShapeProperties(chart.Floor.ShapeProperties);
				}
				else {
					if (format.Apply)
						format.SetupShapeProperties(chart.Floor.ShapeProperties);
					if (areaFormat.Apply)
						areaFormat.SetupShapeProperties(chart.Floor.ShapeProperties);
				}
			}
		}
		void SetupTextProperties(XlsContentBuilder contentBuilder, AxisBase axis) {
			TextProperties properties = axis.TextProperties;
			if (TextFormat != null && TextFormat.IsValidCheckSum(contentBuilder, null, FontX)) {
				TextFormat.SetupTextProperties(properties);
				return;
			}
			if (!LabelAutoRotate) {
				properties.BodyProperties.VerticalText = LabelVerticalText;
				properties.BodyProperties.Rotation = LabelRotationAngle;
				if (LabelAutoColor) {
					DrawingTextParagraph paragraph = new DrawingTextParagraph(axis.DocumentModel);
					paragraph.ApplyParagraphProperties = true;
					paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
					properties.Paragraphs.Add(paragraph);
				}
			}
			if (!LabelAutoColor) {
				DrawingTextParagraph paragraph = new DrawingTextParagraph(axis.DocumentModel);
				paragraph.ApplyParagraphProperties = true;
				paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
				DrawingTextCharacterProperties defaultProperties = paragraph.ParagraphProperties.DefaultCharacterProperties;
				if (FontX.Apply)
					FontX.SetupParagraphDefaults(contentBuilder, defaultProperties);
				DrawingSolidFill fill = new DrawingSolidFill(axis.DocumentModel);
				fill.Color.OriginalColor.Rgb = TextColor;
				defaultProperties.Fill = fill;
				properties.Paragraphs.Add(paragraph);
			}
			if (!LabelAutoMode) {
			}
		}
		void SetupNumberFormat(AxisBase axis) {
			if (!ApplyNumberFormat)
				return;
			if (string.IsNullOrEmpty(NumberFormatCode)) {
				NumberFormat format = axis.DocumentModel.StyleSheet.NumberFormats[NumberFormatId];
				axis.NumberFormat.NumberFormatCode = format.FormatCode;
			}
			else
				axis.NumberFormat.NumberFormatCode = NumberFormatCode;
			axis.NumberFormat.SourceLinked = false;
		}
		void SetupScaling(ScalingOptions scaling) {
			scaling.BeginUpdate();
			try {
				scaling.Orientation = Orientation;
				scaling.Min = MinValue;
				scaling.Max = MaxValue;
				scaling.FixedMax = !AutoMax;
				scaling.FixedMin = !AutoMin;
				scaling.LogScale = IsLogScale;
				scaling.LogBase = LogBase;
			}
			finally {
				scaling.EndUpdate();
			}
		}
		void SetupCategoryAxis(CategoryAxis axis) {
			if (axis == null)
				return;
			axis.Auto = Auto;
			axis.LabelAlign = LabelAlign;
			axis.LabelOffset = LabelOffset;
			axis.NoMultilevelLabels = NoMultiLevelLabels;
		}
		void SetupDateAxis(DateAxis axis) {
			if (axis == null)
				return;
			axis.Auto = Auto;
			axis.BaseTimeUnit = BaseTimeUnit;
			axis.MajorTimeUnit = MajorTimeUnit;
			axis.MinorTimeUnit = MinorTimeUnit;
			axis.LabelOffset = LabelOffset;
		}
		void SetupValueAxis(XlsContentBuilder contentBuilder, ValueAxis axis) {
			if (axis == null)
				return;
			if (UnitType != DisplayUnitType.None) {
				DisplayUnitOptions displayUnit = axis.DisplayUnit;
				displayUnit.BeginUpdate();
				try {
					displayUnit.UnitType = UnitType;
					displayUnit.ShowLabel = ShowUnitLabel;
					if (displayUnit.UnitType == DisplayUnitType.Custom)
						displayUnit.CustomUnit = CustomUnit;
				}
				finally {
					displayUnit.EndUpdate();
				}
				text.SetupDisplayUnitOptions(contentBuilder, axis.DisplayUnit);
			}
			if (isPercentStacked && NumberFormatId == 0)
				NumberFormatId = 9;
		}
		void SetupMMUnits(AxisMMUnitsBase axis) {
			if (axis == null)
				return;
			axis.MajorUnit = MajorUnit;
			axis.MinorUnit = MinorUnit;
		}
		void SetupTickSkip(AxisTickBase axis) {
			if (axis == null)
				return;
			axis.TickLabelSkip = TickLabelSkip;
			axis.TickMarkSkip = TickMarkSkip;
		}
		#endregion
		#region IXlsChartAreaFormat Members
		bool IXlsChartAreaFormat.Apply {
			get { return areaFormat.Apply; }
			set { areaFormat.Apply = value; }
		}
		Color IXlsChartAreaFormat.ForegroundColor {
			get { return areaFormat.ForegroundColor; }
			set { areaFormat.ForegroundColor = value; }
		}
		Color IXlsChartAreaFormat.BackgroundColor {
			get { return areaFormat.BackgroundColor; }
			set { areaFormat.BackgroundColor = value; }
		}
		XlsChartFillType IXlsChartAreaFormat.FillType {
			get { return areaFormat.FillType; }
			set { areaFormat.FillType = value; }
		}
		bool IXlsChartAreaFormat.AutoColor {
			get { return areaFormat.AutoColor; }
			set { areaFormat.AutoColor = value; }
		}
		bool IXlsChartAreaFormat.InvertIfNegative {
			get { return areaFormat.InvertIfNegative; }
			set { areaFormat.InvertIfNegative = value; }
		}
		#endregion
		#region IXlsChartFontX Members
		bool IXlsChartFontX.Apply {
			get { return fontX.Apply; }
			set { fontX.Apply = value; }
		}
		int IXlsChartFontX.Index {
			get { return fontX.Index; }
			set { fontX.Index = value; }
		}
		#endregion
		#region IXlsChartGraphicFormat Members
		bool IXlsChartGraphicFormat.Apply {
			get { return graphicFormat.Apply; }
			set { graphicFormat.Apply = value; }
		}
		OfficeArtProperties IXlsChartGraphicFormat.ArtProperties {
			get { return graphicFormat.ArtProperties; }
			set { graphicFormat.ArtProperties = value; }
		}
		OfficeArtTertiaryProperties IXlsChartGraphicFormat.ArtTertiaryProperties {
			get { return graphicFormat.ArtTertiaryProperties; }
			set { graphicFormat.ArtTertiaryProperties = value; }
		}
		#endregion
		#region IXlsChartExtPropertyVisitor Members
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMax item) {
			if (IsLogScale) {
				AutoMax = false;
				MaxValue = item.Value;
			}
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMin item) {
			if (IsLogScale) {
				AutoMin = false;
				MinValue = item.Value;
			}
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropLogBase item) {
			if (IsLogScale)
				LogBase = item.Value;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStyle item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropThemeOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropColorMapOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropNoMultiLvlLbl item) {
			NoMultiLevelLabels = item.Value;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelSkip item) {
			TickLabelSkip = Math.Max(1, item.Value);
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickMarkSkip item) {
			TickMarkSkip = Math.Max(1, item.Value);
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorUnit item) {
			MajorUnit = 0.0; 
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorUnit item) {
			MinorUnit = 0.0; 
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelPos item) {
			if (item.Value == 0x005d)
				LabelAlign = LabelAlignment.Center;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBaseTimeUnit item) {
			BaseTimeUnit = TimeUnits.Auto;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFormatCode item) {
			ApplyNumberFormat = true;
			NumberFormatCode = item.Value;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorTimeUnit item) {
			MajorTimeUnit = TimeUnits.Auto;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorTimeUnit item) {
			MinorTimeUnit = TimeUnits.Auto;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShowDLblsOverMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBackWallThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFloorThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropDispBlankAs item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStartSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropEndSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShapeProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTextProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropOverlay item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPieCombo item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRightAngAxOff item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPerspective item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationX item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationY item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropHeightPercent item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropCultureCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropSymbol item) { }
		#endregion
		#region IXlsChartTextFormatContainer Members
		void IXlsChartTextFormatContainer.Add(XlsChartTextFormat properties) {
			textFormat = properties;
		}
		#endregion
		#region IXlsChartTextContainer Members
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			this.text = item;
		}
		#endregion
		protected override int GetIndexByObjContext(int objContext) {
			switch (objContext) {
				case XlsChartDefs.AxisContext:
					return XlsChartDefs.AxisIndex;
				case XlsChartDefs.MajorGridlinesContext:
					return XlsChartDefs.MajorGridlinesIndex;
				case XlsChartDefs.MinorGridlinesContext:
					return XlsChartDefs.MinorGridlinesIndex;
				case XlsChartDefs.SurfaceContext:
					return XlsChartDefs.SurfaceIndex;
			}
			return objContext;
		}
		protected override bool IsValidSpCheckSum(int index, int checkSum) {
			if (index == XlsChartDefs.SurfaceIndex && GraphicFormat.Apply)
				return true; 
			return base.IsValidSpCheckSum(index, checkSum);
		}
		protected override int CalcSpCheckSum(int index) {
			MsoCrc32Compute crc32 = new MsoCrc32Compute();
			crc32.CrcValue = base.CalcSpCheckSum(index);
			if (index == XlsChartDefs.SurfaceIndex) {
				if (GraphicFormat.Apply && AreaFormat.Apply && !AreaFormat.AutoColor)
					GraphicFormat.CalcSpCheckSum(crc32);
				else
					AreaFormat.CalcSpCheckSum(crc32);
			}
			return crc32.CrcValue;
		}
	}
	#endregion
}
