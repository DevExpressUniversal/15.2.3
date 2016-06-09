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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
using DevExpress.Export.Xl;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsChartExporter
	public partial class XlsChartExporter {
		#region Fields
		XlsDateAxisCalculator axisCalculator;
		int[] axisLinesCrc = new int[4];
		#endregion
		void ResetAxisLinesCrc() {
			for (int i = 0; i < 4; i++)
				axisLinesCrc[i] = 0;
		}
		XlsChartTextObjectLinkType GetObjectLinkType(AxisDataType axisType) {
			if (axisType == AxisDataType.Agrument)
				return XlsChartTextObjectLinkType.ArgumentAxis;
			else if (axisType == AxisDataType.Value)
				return XlsChartTextObjectLinkType.ValueAxis;
			return XlsChartTextObjectLinkType.SeriesAxis;
		}
		#region AxisParent
		void WriteAxesUsed(bool hasSecondaryAxes) {
			XlsCommandChartAxesUsed command = new XlsCommandChartAxesUsed();
			command.HasSecondaryAxisGroup = hasSecondaryAxes;
			command.Write(StreamWriter);
		}
		void WriteAxisGroup(AxisGroup axisGroup) {
			isPrimaryAxisGroup = object.ReferenceEquals(axisGroup, chart.PrimaryAxes);
			WriteAxisParent();
			WriteBegin();
			WriteAxisPos();
			for (axisIndex = 0; axisIndex < axisGroup.Count; axisIndex++) {
				AxisBase axis = axisGroup[axisIndex];
				ResetAxisLinesCrc();
				axis.Visit(this);
			}
			for (axisIndex = 0; axisIndex < axisGroup.Count; axisIndex++) {
				AxisBase axis = axisGroup[axisIndex];
				WriteAttachedLabelForTitle(axis.Title, GetObjectLinkType(axis.AxisType));
			}
			if (isPrimaryAxisGroup) {
				WritePlotArea();
				WritePlotAreaFrame();
			}
			foreach (IChartView view in chart.Views) {
				if (view.Axes == axisGroup) {
					viewIndex = viewIndexTable[view];
					view.Visit(this);
				}
			}
			WriteAxisGroupEndBlock();
			WriteEnd();
		}
		#endregion
		#region Axes
		void WriteAxisParent() {
			XlsCommandChartAxisParent command = new XlsCommandChartAxisParent();
			command.Group = isPrimaryAxisGroup ? XlsAxisGroupType.Primary : XlsAxisGroupType.Secondary;
			command.Write(StreamWriter);
		}
		void WriteAxisPos() {
			XlsCommandChartPos command = new XlsCommandChartPos();
			command.TopLeftMode = XlsChartPosMode.Parent;
			command.BottomRightMode = XlsChartPosMode.Parent;
			command.Left = 0;
			command.Top = 0;
			command.Width = 4000;
			command.Height = 4000;
			command.Write(StreamWriter);
		}
		void WriteAxisType(AxisDataType axisType) {
			XlsCommandChartAxis command = new XlsCommandChartAxis();
			command.AxisType = axisType;
			command.Write(StreamWriter);
		}
		void WriteAxisCatSerRange(CategoryAxis axis) {
			XlsCommandChartAxisCatSerRange command = new XlsCommandChartAxisCatSerRange();
			command.Reverse = axis.Scaling.Orientation == AxisOrientation.MaxMin;
			command.TickLabelSkip = axis.TickLabelSkip;
			command.TickMarkSkip = axis.TickMarkSkip;
			AxisBase crossesAxis = axis.CrossesAxis;
			command.CategoryCrossValue = (int)crossesAxis.CrossesValue;
			command.IsMaxCross = crossesAxis.Crosses == AxisCrosses.Max;
			ValueAxis valueAxis = crossesAxis as ValueAxis;
			if (valueAxis != null)
				command.IsCrossBetween = valueAxis.CrossBetween == AxisCrossBetween.Between;
			command.Write(StreamWriter);
		}
		void WriteAxisCatSerRange(DateAxis axis) {
			XlsCommandChartAxisCatSerRange command = new XlsCommandChartAxisCatSerRange();
			command.Reverse = axis.Scaling.Orientation == AxisOrientation.MaxMin;
			command.TickLabelSkip = 1;
			command.TickMarkSkip = 1;
			command.CategoryCrossValue = ConvertDate(axisCalculator.CrossesValue, axisCalculator.BaseTimeUnit) - ConvertDate(axisCalculator.Minimum, axisCalculator.BaseTimeUnit) + 1;
			command.IsMaxCross = axis.CrossesAxis.Crosses == AxisCrosses.Max;
			ValueAxis valueAxis = axis.CrossesAxis as ValueAxis;
			if (valueAxis != null)
				command.IsCrossBetween = valueAxis.CrossBetween == AxisCrossBetween.Between;
			command.Write(StreamWriter);
		}
		void WriteAxisCatSerRange(SeriesAxis axis) {
			XlsCommandChartAxisCatSerRange command = new XlsCommandChartAxisCatSerRange();
			command.Reverse = axis.Scaling.Orientation == AxisOrientation.MaxMin;
			command.TickLabelSkip = axis.TickLabelSkip;
			command.TickMarkSkip = axis.TickMarkSkip;
			AxisBase crossesAxis = axis.CrossesAxis;
			command.CategoryCrossValue = 0;
			command.IsMaxCross = axis.Crosses == AxisCrosses.Max;
			ValueAxis valueAxis = crossesAxis as ValueAxis;
			if (valueAxis != null)
				command.IsCrossBetween = valueAxis.CrossBetween == AxisCrossBetween.Between;
			command.Write(StreamWriter);
		}
		void WriteAxisExt(CategoryAxis axis) {
			XlsCommandChartAxisExt command = new XlsCommandChartAxisExt();
			command.IsDateAxis = false;
			command.Write(StreamWriter);
		}
		void WriteAxisExt(DateAxis axis) {
			XlsCommandChartAxisExt command = new XlsCommandChartAxisExt();
			command.IsDateAxis = true;
			command.Auto = axis.Auto;
			command.AutoMax = !axis.Scaling.FixedMax;
			command.AutoMin = !axis.Scaling.FixedMin;
			command.AutoBase = axis.BaseTimeUnit == TimeUnits.Auto;
			command.AutoMajor = false;
			command.AutoMinor = false;
			TimeUnits baseUnits = axisCalculator.BaseTimeUnit;
			command.BaseTimeUnit = baseUnits;
			command.MajorTimeUnit = axisCalculator.MajorTimeUnit;
			command.MinorTimeUnit = axisCalculator.MinorTimeUnit;
			command.MaxDate = ConvertDate(axisCalculator.Maximum, baseUnits);
			command.MinDate = ConvertDate(axisCalculator.Minimum, baseUnits);
			command.MajorDateTick = (int)axisCalculator.MajorUnit;
			command.MinorDateTick = (int)axisCalculator.MinorUnit;
			command.CrossDate = ConvertDate(axisCalculator.CrossesValue, baseUnits);
			command.AutoCross = axis.CrossesAxis.Crosses == AxisCrosses.AutoZero;
			command.Write(StreamWriter);
		}
		void WriteAxisValueRange(ValueAxis axis) {
			XlsCommandChartAxisValueRange command = new XlsCommandChartAxisValueRange();
			bool isLogScale = axis.Scaling.LogScale;
			command.MinValue = ConvertLog10Value(axis.Scaling.Min, isLogScale);
			command.MaxValue = ConvertLog10Value(axis.Scaling.Max, isLogScale);
			command.MajorUnit = ConvertLog10Value(axis.MajorUnit, isLogScale);
			command.MinorUnit = ConvertLog10Value(axis.MinorUnit, isLogScale);
			command.AutoMin = !axis.Scaling.FixedMin;
			command.AutoMax = !axis.Scaling.FixedMax;
			command.AutoMajor = !axis.FixedMajorUnit;
			command.AutoMinor = !axis.FixedMinorUnit;
			command.IsLogScale = axis.Scaling.LogScale;
			command.Reversed = axis.Scaling.Orientation == AxisOrientation.MaxMin;
			AxisBase crossesAxis = axis.CrossesAxis;
			command.CrossesValue = ConvertLog10Value(crossesAxis.CrossesValue, isLogScale);
			command.AutoCross = crossesAxis.Crosses == AxisCrosses.AutoZero;
			command.IsMaxCross = crossesAxis.Crosses == AxisCrosses.Max;
			command.Write(StreamWriter);
		}
		void WriteAxisAXM(DisplayUnitOptions displayUnit) {
			if (displayUnit.UnitType == DisplayUnitType.None || !displayUnit.ShowLabel)
				return;
			WriteYMult(displayUnit);
			WriteAttachedLabelForDisplayUnits(displayUnit);
		}
		void WriteYMult(DisplayUnitOptions displayUnit) {
			XlsCommandChartAxisYMult command = new XlsCommandChartAxisYMult();
			DisplayUnitType unitType = displayUnit.UnitType;
			command.UnitType = unitType;
			command.ShowLabel = displayUnit.ShowLabel;
			command.CustomUnit = unitType == DisplayUnitType.Custom ? displayUnit.CustomUnit : ConvertDisplayUnitType(unitType);
			command.Write(StreamWriter);
		}
		double ConvertDisplayUnitType(DisplayUnitType unitType) {
			switch (unitType) {
				case DisplayUnitType.Hundreds: return 100;
				case DisplayUnitType.Thousands: return 1000;
				case DisplayUnitType.TenThousands: return 10000;
				case DisplayUnitType.HundredThousands: return 100000;
				case DisplayUnitType.Millions: return 1000000;
				case DisplayUnitType.TenMillions: return 10000000;
				case DisplayUnitType.HundredMillions: return 100000000;
				case DisplayUnitType.Billions: return 1000000000;
				case DisplayUnitType.Trillions: return 1000000000000;
			}
			return 0;
		}
		double ConvertLog10Value(double value, bool isLogScale) {
			if (value <= 0.0 || !isLogScale)
				return value;
			value = Math.Log10(value);
			return (value < 0.0) ? Math.Floor(value) : Math.Ceiling(value);
		}
		int ConvertDate(double dateValue, TimeUnits timeUnits) {
			if (timeUnits == TimeUnits.Days)
				return (int)dateValue;
			WorkbookDataContext context = DocumentModel.DataContext;
			DateTime baseDate = context.FromDateTimeSerial(0);
			DateTime date = context.FromDateTimeSerial(dateValue);
			if (timeUnits == TimeUnits.Months)
				return (date.Year - baseDate.Year) * 12 + date.Month - 1;
			return date.Year - baseDate.Year;
		}
		void WriteAxisCategoryLabels(CategoryAxis axis) {
			XlsCommandChartAxisCatLabels command = new XlsCommandChartAxisCatLabels();
			command.LabelAlign = axis.LabelAlign;
			command.LabelOffset = axis.LabelOffset;
			command.AutoCatLabel = axis.TickLabelSkip == 1;
			command.Write(StreamWriter);
		}
		void WriteAxisProperties(AxisBase axis) {
			WriteAxisNumberFormat(axis);
			WriteAxisTicks(axis);
			WriteFontX(axis.TextProperties.Paragraphs, ChartElement.Axis);
			WriteAxisLines(axis);
			WriteAxisAreaFormat(axis);
			WriteAxisShapeFormats(axis);
		}
		void WriteAxisNumberFormat(AxisBase axis) {
			if (axis.NumberFormat.SourceLinked)
				return;
			XlsCommandChartAxisNumberFormat command = new XlsCommandChartAxisNumberFormat();
			command.Value = (short)ExportStyleSheet.GetNumberFormatId(axis.NumberFormat.NumberFormatId);
			command.Write(StreamWriter);
		}
		void WriteAxisTicks(AxisBase axis) {
			Palette palette = axis.DocumentModel.StyleSheet.Palette;
			XlsCommandChartAxisTick command = new XlsCommandChartAxisTick();
			command.MajorTickMark = axis.MajorTickMark;
			command.MinorTickMark = axis.MinorTickMark;
			command.TickLabelPos = axis.TickLabelPos;
			DrawingFillType labelFillType = axis.ShapeProperties.Fill.FillType;
			command.IsTransparent = labelFillType == DrawingFillType.Automatic || labelFillType == DrawingFillType.None;
			DrawingSolidFill textFill = null;
			if (axis.TextProperties.Paragraphs.Count > 0 && !axis.TextProperties.Paragraphs[0].ParagraphProperties.IsDefault)
				textFill = axis.TextProperties.Paragraphs[0].ParagraphProperties.DefaultCharacterProperties.Fill as DrawingSolidFill;
			if (textFill != null) {
				command.TextColorIndex = palette.GetPaletteNearestColorIndex(textFill.Color.FinalColor);
				command.TextColor = palette[command.TextColorIndex];
				command.AutoColor = false;
			}
			else {
				command.TextColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.TextColor = palette.DefaultChartForegroundColor;
				command.AutoColor = true;
			}
			command.AutoMode = true;
			command.VerticalText = axis.TextProperties.BodyProperties.VerticalText;
			command.RotationAngle = axis.TextProperties.BodyProperties.Rotation;
			command.AutoRotate = axis.TextProperties.BodyProperties.IsDefault;
			command.TextReadingOrder = XlReadingOrder.Context;
			command.Write(StreamWriter);
		}
		void WriteAxisLines(AxisBase axis) {
			WriteAxisLine(axis);
			if (axis.ShowMajorGridlines)
				WriteAxisLine(1, axis.MajorGridlines, ChartElement.MajorGridlines);
			if (axis.ShowMinorGridlines)
				WriteAxisLine(2, axis.MinorGridlines, ChartElement.MinorGridlines);
			if (Chart.Is3DChart && axis.AxisType != AxisDataType.Series)
				WriteAxisLine(3, 
					axis.AxisType == AxisDataType.Agrument ? Chart.BackWall.ShapeProperties : Chart.Floor.ShapeProperties,
					axis.AxisType == AxisDataType.Agrument ? ChartElement.Walls : ChartElement.Floor);
		}
		void WriteAxisLine(AxisBase axis) {
			XlsCommandChartAxisLine command = new XlsCommandChartAxisLine();
			command.Value = 0;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, axis.ShapeProperties);
			helper.Element = ChartElement.Axis;
			helper.WriteContent(axis.Delete);
			axisLinesCrc[0] = Crc32.CrcValue;
		}
		void WriteAxisLine(int lineIndex, ShapeProperties shapeProperties, ChartElement element) {
			XlsCommandChartAxisLine command = new XlsCommandChartAxisLine();
			command.Value = (short)lineIndex;
			command.Write(StreamWriter);
			ResetCrc32();
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			helper.WriteContent();
			axisLinesCrc[lineIndex] = Crc32.CrcValue;
		}
		void WriteAxisAreaFormat(AxisBase axis) {
			if (Chart.Is3DChart && axis.AxisType != AxisDataType.Series) {
				Crc32.CrcValue = axisLinesCrc[3];
				XlsChartAreaFormatExportHelper helper = new XlsChartAreaFormatExportHelper(this, axis.AxisType == AxisDataType.Agrument ? Chart.BackWall.ShapeProperties : Chart.Floor.ShapeProperties);
				helper.Element = axis.AxisType == AxisDataType.Agrument ? ChartElement.Walls : ChartElement.Floor;
				helper.WriteContent();
				axisLinesCrc[3] = Crc32.CrcValue;
			}
		}
		void WriteAxisShapeFormats(AxisBase axis) {
			if (!axis.Delete) {
				Crc32.CrcValue = axisLinesCrc[0];
				WriteAxisStartBlock(axis);
				WriteShapeFormat(axis.ShapeProperties, XlsChartDefs.AxisContext);
			}
			if (axis.ShowMajorGridlines) {
				Crc32.CrcValue = axisLinesCrc[1];
				WriteAxisStartBlock(axis);
				WriteShapeFormat(axis.MajorGridlines, XlsChartDefs.MajorGridlinesContext);
			}
			if (axis.ShowMinorGridlines) {
				Crc32.CrcValue = axisLinesCrc[2];
				WriteAxisStartBlock(axis);
				WriteShapeFormat(axis.MinorGridlines, XlsChartDefs.MinorGridlinesContext);
			}
			if (Chart.Is3DChart && axis.AxisType != AxisDataType.Series) {
				Crc32.CrcValue = axisLinesCrc[3];
				WriteAxisStartBlock(axis);
				WriteShapeFormat(axis.AxisType == AxisDataType.Agrument ? Chart.BackWall.ShapeProperties : Chart.Floor.ShapeProperties, XlsChartDefs.SurfaceContext);
			}
		}
		void WriteAxisExtProperties(CategoryAxis axis) {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.CategoryAxis;
			extProperties.Items.Add(new XlsChartExtPropNoMultiLvlLbl() { Value = axis.NoMultilevelLabels });
			if (axis.TickLabelSkip == 1)
				extProperties.Items.Add(new XlsChartExtPropTickLabelSkip() { Value = axis.TickLabelSkip });
			if (axis.TickMarkSkip == 1)
				extProperties.Items.Add(new XlsChartExtPropTickMarkSkip() { Value = axis.TickMarkSkip });
			WriteExtProperties(extProperties);
		}
		void WriteAxisExtProperties(DateAxis axis) {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.CategoryAxis;
			if (axis.MajorTimeUnit == TimeUnits.Auto)
				extProperties.Items.Add(new XlsChartExtPropMajorUnit() { Value = axisCalculator.AutoMajorUnit });
			if (axis.MinorTimeUnit == TimeUnits.Auto)
				extProperties.Items.Add(new XlsChartExtPropMinorUnit() { Value = axisCalculator.AutoMinorUnit });
			if (axis.BaseTimeUnit == TimeUnits.Auto)
				extProperties.Items.Add(new XlsChartExtPropBaseTimeUnit() { Value = TimeUnitToCode(axisCalculator.AutoBaseTimeUnit) });
			if (!axis.NumberFormat.SourceLinked)
				extProperties.Items.Add(new XlsChartExtPropFormatCode() { Value = axis.NumberFormat.NumberFormatCode });
			if (axis.MajorTimeUnit == TimeUnits.Auto)
				extProperties.Items.Add(new XlsChartExtPropMajorTimeUnit() { Value = TimeUnitToCode(axisCalculator.AutoMajorTimeUnit) });
			if (axis.MinorTimeUnit == TimeUnits.Auto)
				extProperties.Items.Add(new XlsChartExtPropMinorTimeUnit() { Value = TimeUnitToCode(axisCalculator.AutoMinorTimeUnit) });
			if (extProperties.Items.Count > 0)
				WriteAxisStartBlock(axis);
			WriteExtProperties(extProperties);
		}
		void WriteAxisExtProperties(ValueAxis axis) {
			if (!axis.Scaling.LogScale || (axis.Scaling.LogBase == 10.0))
				return;
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.ValueAxis;
			if(axis.Scaling.FixedMax)
				extProperties.Items.Add(new XlsChartExtPropScaleMax() { Value = axis.Scaling.Max });
			if (axis.Scaling.FixedMin)
				extProperties.Items.Add(new XlsChartExtPropScaleMin() { Value = axis.Scaling.Min });
			extProperties.Items.Add(new XlsChartExtPropLogBase() { Value = axis.Scaling.LogBase });
			WriteAxisStartBlock(axis);
			WriteExtProperties(extProperties);
		}
		void WriteAxisExtProperties(SeriesAxis axis) {
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.CategoryAxis;
			if (axis.TickLabelSkip == 1)
				extProperties.Items.Add(new XlsChartExtPropTickLabelSkip() { Value = axis.TickLabelSkip });
			if (axis.TickMarkSkip == 1)
				extProperties.Items.Add(new XlsChartExtPropTickMarkSkip() { Value = axis.TickMarkSkip });
			if(extProperties.Items.Count > 0)
				WriteAxisStartBlock(axis);
			WriteExtProperties(extProperties);
		}
		int TimeUnitToCode(TimeUnits units) {
			switch (units) {
				case TimeUnits.Days: return 0x060;
				case TimeUnits.Months: return 0x061;
				case TimeUnits.Years: return 0x062;
			}
			return 0;
		}
		#endregion
		#region IAxisVisitor Members
		public void Visit(CategoryAxis axis) {
			WriteAxisType(AxisDataType.Agrument);
			WriteBegin();
			WriteAxisCatSerRange(axis);
			WriteAxisExt(axis);
			WriteAxisStartBlock(axis);
			WriteAxisCategoryLabels(axis);
			WriteAxisProperties(axis);
			WriteAxisExtProperties(axis);
			WriteAxisEndBlock();
			WriteEnd();
		}
		public void Visit(ValueAxis axis) {
			WriteAxisType(axisIndex == 0 ? AxisDataType.Agrument : AxisDataType.Value);
			WriteBegin();
			WriteAxisValueRange(axis);
			WriteAxisAXM(axis.DisplayUnit);
			WriteAxisProperties(axis);
			WriteAxisExtProperties(axis);
			WriteAxisEndBlock();
			WriteEnd();
		}
		public void Visit(DateAxis axis) {
			axisCalculator = new XlsDateAxisCalculator(axis);
			axisCalculator.Calculate();
			WriteAxisType(AxisDataType.Agrument);
			WriteBegin();
			WriteAxisCatSerRange(axis);
			WriteAxisExt(axis);
			WriteAxisProperties(axis);
			WriteAxisExtProperties(axis);
			WriteAxisEndBlock();
			WriteEnd();
		}
		public void Visit(SeriesAxis axis) {
			WriteAxisType(AxisDataType.Series);
			WriteBegin();
			WriteAxisCatSerRange(axis);
			WriteAxisProperties(axis);
			WriteAxisExtProperties(axis);
			WriteAxisEndBlock();
			WriteEnd();
		}
		#endregion
	}
	#endregion
	#region XlsDateAxisCalculator
	public class XlsDateAxisCalculator {
		#region Fields
		DateAxis axis;
		#endregion
		public XlsDateAxisCalculator(DateAxis axis) {
			this.axis = axis;
		}
		#region Properties
		public DateAxis Axis { get { return axis; } }
		public double AutoMaximum { get; private set; }
		public double AutoMinimum { get; private set; }
		public double AutoMajorUnit { get; private set; }
		public double AutoMinorUnit { get; private set; }
		public TimeUnits AutoBaseTimeUnit { get; private set; }
		public TimeUnits AutoMajorTimeUnit { get; private set; }
		public TimeUnits AutoMinorTimeUnit { get; private set; }
		public double Maximum { get { return axis.Scaling.FixedMax ? axis.Scaling.Max : AutoMaximum; } }
		public double Minimum { get { return axis.Scaling.FixedMin ? axis.Scaling.Min : AutoMinimum; } }
		public double MajorUnit { get { return axis.FixedMajorUnit ? axis.MajorUnit : AutoMajorUnit; } }
		public double MinorUnit { get { return axis.FixedMinorUnit ? axis.MinorUnit : AutoMinorUnit; } }
		public TimeUnits BaseTimeUnit { get { return axis.BaseTimeUnit != TimeUnits.Auto ? axis.BaseTimeUnit : AutoBaseTimeUnit; } }
		public TimeUnits MajorTimeUnit { get { return axis.MajorTimeUnit != TimeUnits.Auto ? axis.MajorTimeUnit : AutoMajorTimeUnit; } }
		public TimeUnits MinorTimeUnit { get { return axis.MinorTimeUnit != TimeUnits.Auto ? axis.MinorTimeUnit : AutoMinorTimeUnit; } }
		public double CrossesValue {
			get {
				AxisBase crossesAxis = axis.CrossesAxis;
				if (crossesAxis.Crosses == AxisCrosses.Max)
					return Maximum;
				if (crossesAxis.Crosses == AxisCrosses.AtValue)
					return crossesAxis.CrossesValue;
				return Minimum;
			}
		}
		#endregion
		public void Calculate() {
			SetupDefaultValues();
			ChartDataReference dataRef = GetArguments() as ChartDataReference;
			if (dataRef != null) {
				List<double> values = new List<double>((int)dataRef.ValuesCount);
				for (int i = 0; i < dataRef.ValuesCount; i++) {
					VariantValue dataValue = dataRef.GetValue(i);
					if (dataValue.IsNumeric)
						values.Add(dataValue.NumericValue);
				}
				if (values.Count == 0)
					return;
				values.Sort();
				AutoMinimum = values[0];
				AutoMaximum = values[values.Count - 1];
				double minDelta = double.MaxValue;
				double prevValue = values[0];
				for (int i = 1; i < values.Count; i++) {
					double value = values[i];
					double delta = value - prevValue;
					if(delta > 0)
						minDelta = Math.Min(minDelta, delta);
					prevValue = value;
				}
				if (minDelta >= 365) {
					AutoBaseTimeUnit = TimeUnits.Years;
					AutoMajorTimeUnit = TimeUnits.Years;
					AutoMinorTimeUnit = TimeUnits.Years;
					AutoMinimum = TruncToUnits(AutoMinimum, TimeUnits.Years);
					AutoMaximum = TruncToUnits(AutoMaximum, TimeUnits.Years);
				}
				else if (minDelta >= 28) {
					AutoBaseTimeUnit = TimeUnits.Months;
					AutoMajorTimeUnit = TimeUnits.Months;
					AutoMinorTimeUnit = TimeUnits.Months;
					AutoMinimum = TruncToUnits(AutoMinimum, TimeUnits.Months);
					AutoMaximum = TruncToUnits(AutoMaximum, TimeUnits.Months);
				}
				SetupMajorUnit();
			}
		}
		void SetupDefaultValues() {
			AutoMaximum = 1.0;
			AutoMinimum = 0.0;
			AutoMajorUnit = 1.0;
			AutoMinorUnit = 1.0;
			AutoBaseTimeUnit = TimeUnits.Days;
			AutoMajorTimeUnit = TimeUnits.Days;
			AutoMinorTimeUnit = TimeUnits.Days;
		}
		IDataReference GetArguments() {
			Chart chart = axis.Parent as Chart;
			if (chart == null)
				return null;
			AxisGroup axisGroup = null;
			if (chart.PrimaryAxes.Contains(axis))
				axisGroup = chart.PrimaryAxes;
			else if (chart.SecondaryAxes.Contains(axis))
				axisGroup = chart.SecondaryAxes;
			if (axisGroup == null)
				return null;
			ISeries series = null;
			foreach (IChartView view in chart.Views) {
				if (object.ReferenceEquals(view.Axes, axisGroup) && view.Series.Count > 0) {
					series = view.Series[0];
					break;
				}
			}
			if(series == null)
				return null;
			return series.Arguments;
		}
		double TruncToUnits(double value, TimeUnits units) {
			WorkbookDataContext context = axis.Parent.DocumentModel.DataContext;
			VariantValue val = new VariantValue();
			val.NumericValue = value;
			DateTime date = val.ToDateTime(context);
			if(units == TimeUnits.Months)
				date = new DateTime(date.Year, date.Month, 1);
			if (units == TimeUnits.Years)
				date = new DateTime(date.Year, 1, 1);
			val.SetDateTime(date, context);
			return val.NumericValue;
		}
		double GetAproxAxisLengthInPoints() {
			Chart chart = (Chart)axis.Parent;
			Legend legend = chart.Legend;
			double result;
			if (axis.Position == AxisPosition.Bottom || axis.Position == AxisPosition.Top) {
				result = chart.DocumentModel.UnitConverter.ModelUnitsToPointsF(chart.Width);
				if (legend.Visible && !legend.Overlay && 
					(legend.Position == LegendPosition.Left || 
					legend.Position == LegendPosition.Right || 
					legend.Position == LegendPosition.TopRight))
					result *= 0.75;
			}
			else {
				result = chart.DocumentModel.UnitConverter.ModelUnitsToPointsF(chart.Height);
				if (legend.Visible && !legend.Overlay &&
					(legend.Position == LegendPosition.Top ||
					legend.Position == LegendPosition.Bottom))
					result *= 0.8;
				if (chart.Title.Visible)
					result *= 0.75;
			}
			return result;
		}
		void SetupMajorUnit() {
			int maxAxisLabelsCount = (int)(GetAproxAxisLengthInPoints() / 20); 
			if (SetupMajorUnitInDays(maxAxisLabelsCount))
				return;
			if (SetupMajorUnitInMonths(maxAxisLabelsCount))
				return;
			int years = ConvertDate(AutoMaximum, TimeUnits.Years) - ConvertDate(AutoMinimum, TimeUnits.Years) + 1;
			int unit = 1;
			while ((years / unit) > maxAxisLabelsCount)
				unit++;
			AutoMajorUnit = unit;
		}
		bool SetupMajorUnitInDays(int maxAxisLabelsCount) {
			if (AutoMajorTimeUnit == TimeUnits.Days) {
				int days = (int)(AutoMaximum - AutoMinimum) + 1;
				if (days > maxAxisLabelsCount) {
					if ((days / 2) > maxAxisLabelsCount) {
						if ((days / 7) > maxAxisLabelsCount) {
							AutoMajorTimeUnit = TimeUnits.Months;
							AutoMinorTimeUnit = TimeUnits.Months;
						}
						else
							AutoMajorUnit = 7;
					}
					else
						AutoMajorUnit = 2;
				}
			}
			return AutoMajorTimeUnit == TimeUnits.Days;
		}
		bool SetupMajorUnitInMonths(int maxAxisLabelsCount) {
			if (AutoMajorTimeUnit == TimeUnits.Months) {
				int months = ConvertDate(AutoMaximum, TimeUnits.Months) - ConvertDate(AutoMinimum, TimeUnits.Months) + 1;
				if (months > maxAxisLabelsCount) {
					if ((months / 2) > maxAxisLabelsCount) {
						AutoMajorTimeUnit = TimeUnits.Years;
						AutoMinorTimeUnit = TimeUnits.Years;
					}
					else
						AutoMajorUnit = 2;
				}
			}
			return AutoMajorTimeUnit == TimeUnits.Months;
		}
		int ConvertDate(double dateValue, TimeUnits timeUnits) {
			if (timeUnits == TimeUnits.Days)
				return (int)dateValue;
			WorkbookDataContext context = axis.Parent.DocumentModel.DataContext;
			DateTime baseDate = context.FromDateTimeSerial(0);
			DateTime date = context.FromDateTimeSerial(dateValue);
			if (timeUnits == TimeUnits.Months)
				return (date.Year - baseDate.Year) * 12 + date.Month - 1;
			return date.Year - baseDate.Year;
		}
	}
	#endregion
}
