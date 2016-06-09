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
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<AxisPosition, string> AxisPositionTable = CreateAxisPositionTable();
		internal static Dictionary<AxisCrosses, string> AxisCrossesTable = CreateAxisCrossesTable();
		internal static Dictionary<TickMark, string> TickMarkTable = CreateTickMarkTable();
		internal static Dictionary<AxisOrientation, string> AxisOrientationTable = CreateAxisOrientationTable();
		internal static Dictionary<TickLabelPosition, string> TickLabelPositionTable = CreateTickLabelPositionTable();
		internal static Dictionary<LabelAlignment, string> LabelAlignmentTable = CreateLabelAlignmentTable();
		internal static Dictionary<AxisCrossBetween, string> CrossBetweenTable = CreateCrossBetweenTable();
		internal static Dictionary<DisplayUnitType, string> DisplayUnitTypeTable = CreateDisplayUnitTypeTable();
		internal static Dictionary<TimeUnits, string> TimeUnitTable = CreateTimeUnitTable();
		static Dictionary<LabelAlignment, string> CreateLabelAlignmentTable() {
			Dictionary<LabelAlignment, string> result = new Dictionary<LabelAlignment, string>();
			result.Add(LabelAlignment.Center, "ctr");
			result.Add(LabelAlignment.Left, "l");
			result.Add(LabelAlignment.Right, "r");
			return result;
		}
		static Dictionary<TickLabelPosition, string> CreateTickLabelPositionTable() {
			Dictionary<TickLabelPosition, string> result = new Dictionary<TickLabelPosition, string>();
			result.Add(TickLabelPosition.High, "high");
			result.Add(TickLabelPosition.Low, "low");
			result.Add(TickLabelPosition.NextTo, "nextTo");
			result.Add(TickLabelPosition.None, "none");
			return result;
		}
		static Dictionary<AxisOrientation, string> CreateAxisOrientationTable() {
			Dictionary<AxisOrientation, string> result = new Dictionary<AxisOrientation, string>();
			result.Add(AxisOrientation.MaxMin, "maxMin");
			result.Add(AxisOrientation.MinMax, "minMax");
			return result;
		}
		static Dictionary<TickMark, string> CreateTickMarkTable() {
			Dictionary<TickMark, string> result = new Dictionary<TickMark, string>();
			result.Add(TickMark.Cross, "cross");
			result.Add(TickMark.Inside, "in");
			result.Add(TickMark.None, "none");
			result.Add(TickMark.Outside, "out");
			return result;
		}
		static Dictionary<AxisCrosses, string> CreateAxisCrossesTable() {
			Dictionary<AxisCrosses, string> result = new Dictionary<AxisCrosses, string>();
			result.Add(AxisCrosses.AutoZero, "autoZero");
			result.Add(AxisCrosses.Min, "min");
			result.Add(AxisCrosses.Max, "max");
			return result;
		}
		static Dictionary<AxisPosition, string> CreateAxisPositionTable() {
			Dictionary<AxisPosition, string> result = new Dictionary<AxisPosition, string>();
			result.Add(AxisPosition.Bottom, "b");
			result.Add(AxisPosition.Left, "l");
			result.Add(AxisPosition.Right, "r");
			result.Add(AxisPosition.Top, "t");
			return result;
		}
		static Dictionary<AxisCrossBetween, string> CreateCrossBetweenTable() {
			Dictionary<AxisCrossBetween, string> result = new Dictionary<AxisCrossBetween, string>();
			result.Add(AxisCrossBetween.Between, "between");
			result.Add(AxisCrossBetween.Midpoint, "midCat");
			return result;
		}
		static Dictionary<DisplayUnitType, string> CreateDisplayUnitTypeTable() {
			Dictionary<DisplayUnitType, string> result = new Dictionary<DisplayUnitType, string>();
			result.Add(DisplayUnitType.Billions, "billions");
			result.Add(DisplayUnitType.HundredMillions, "hundredMillions");
			result.Add(DisplayUnitType.Hundreds, "hundreds");
			result.Add(DisplayUnitType.HundredThousands, "hundredThousands");
			result.Add(DisplayUnitType.Millions, "millions");
			result.Add(DisplayUnitType.TenMillions, "tenMillions");
			result.Add(DisplayUnitType.TenThousands, "tenThousands");
			result.Add(DisplayUnitType.Thousands, "thousands");
			result.Add(DisplayUnitType.Trillions, "trillions");
			return result;
		}
		static Dictionary<TimeUnits, string> CreateTimeUnitTable() {
			Dictionary<TimeUnits, string> result = new Dictionary<TimeUnits, string>();
			result.Add(TimeUnits.Days, "days");
			result.Add(TimeUnits.Months, "months");
			result.Add(TimeUnits.Years, "years");
			return result;
		}
		#endregion
		#region AxisIdTable
		Dictionary<AxisBase, int> axisIdTable = new Dictionary<AxisBase, int>();
		void PrepareAxisIdTable(Chart chart) {
			axisIdTable.Clear();
			int axisId = 1;
			foreach (AxisBase item in chart.PrimaryAxes)
				axisIdTable.Add(item, axisId++);
			foreach (AxisBase item in chart.SecondaryAxes)
				axisIdTable.Add(item, axisId++);
		}
		protected internal void RegisterAxisId(AxisBase axis, int id) {
			axisIdTable.Add(axis, id);
		}
		#endregion
		protected internal void GenerateAxisGroupContent(AxisGroup axisGroup, bool reverse) {
			AxisExportWalker walker = new AxisExportWalker(this);
			int count = axisGroup.Count;
			for (int i = 0; i < count; i++) {
				int index = reverse ? count - i - 1 : i;
				axisGroup[index].Visit(walker);
			}
		}
		#region AxisShared
		void GenerateAxisSharedContent(AxisBase axis) {
			GenerateChartSimpleIntAttributeTag("axId", axisIdTable[axis]);
			GenerateScalingContent(axis.Scaling);
			GenerateChartSimpleBoolAttributeTag("delete", axis.Delete);
			GenerateChartSimpleStringAttributeTag("axPos", AxisPositionTable[axis.Position]);
			if (axis.ShowMajorGridlines)
				GenerateAxisGridlines("majorGridlines", axis.MajorGridlines);
			if (axis.ShowMinorGridlines)
				GenerateAxisGridlines("minorGridlines", axis.MinorGridlines);
			GenerateChartTitleContent(axis.Title);
			GenerateNumberFormatContent(axis.NumberFormat);
			if (axis.MajorTickMark != TickMark.Cross)
				GenerateChartSimpleStringAttributeTag("majorTickMark", TickMarkTable[axis.MajorTickMark]);
			if (axis.MinorTickMark != TickMark.Cross)
				GenerateChartSimpleStringAttributeTag("minorTickMark", TickMarkTable[axis.MinorTickMark]);
				GenerateChartSimpleStringAttributeTag("tickLblPos", TickLabelPositionTable[axis.TickLabelPos]);
			GenerateChartShapeProperties(axis.ShapeProperties);
			GenerateTextPropertiesContent(axis.TextProperties);
			GenerateChartSimpleIntAttributeTag("crossAx", axisIdTable[axis.CrossesAxis]);
			if (axis.Crosses == AxisCrosses.AtValue)
				GenerateChartSimpleDoubleAttributeTag("crossesAt", axis.CrossesValue);
			else
				GenerateChartSimpleStringAttributeTag("crosses", AxisCrossesTable[axis.Crosses]);
		}
		#endregion
		#region CategoryAxis
		internal void GenerateAxisContent(CategoryAxis axis) {
			WriteStartElement("catAx", DrawingMLChartNamespace);
			try {
				GenerateAxisSharedContent(axis);
				GenerateChartSimpleBoolAttributeTag("auto", axis.Auto);
				GenerateChartSimpleStringAttributeTag("lblAlgn", LabelAlignmentTable[axis.LabelAlign]);
				GenerateChartSimpleIntAttributeTag("lblOffset", axis.LabelOffset);
				if (axis.TickLabelSkip > 1)
					GenerateChartSimpleIntAttributeTag("tickLblSkip", axis.TickLabelSkip);
				if (axis.TickMarkSkip > 1)
					GenerateChartSimpleIntAttributeTag("tickMarkSkip", axis.TickMarkSkip);
				GenerateChartSimpleBoolAttributeTag("noMultiLvlLbl", axis.NoMultilevelLabels);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region ValueAxis
		internal void GenerateAxisContent(ValueAxis axis) {
			WriteStartElement("valAx", DrawingMLChartNamespace);
			try {
				GenerateAxisSharedContent(axis);
				GenerateChartSimpleStringAttributeTag("crossBetween", CrossBetweenTable[axis.CrossBetween]);
				if (axis.FixedMajorUnit)
					GenerateChartSimpleDoubleAttributeTag("majorUnit", axis.MajorUnit);
				if (axis.FixedMinorUnit)
					GenerateChartSimpleDoubleAttributeTag("minorUnit", axis.MinorUnit);
				GenerateDisplayUnitsContent(axis.DisplayUnit);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region DateAxis
		internal void GenerateAxisContent(DateAxis axis) {
			WriteStartElement("dateAx", DrawingMLChartNamespace);
			try {
				GenerateAxisSharedContent(axis);
				GenerateChartSimpleBoolAttributeTag("auto", axis.Auto);
				GenerateChartSimpleIntAttributeTag("lblOffset", axis.LabelOffset);
				if (axis.BaseTimeUnit != TimeUnits.Auto)
					GenerateChartSimpleStringAttributeTag("baseTimeUnit", TimeUnitTable[axis.BaseTimeUnit]);
				if (axis.FixedMajorUnit)
					GenerateChartSimpleDoubleAttributeTag("majorUnit", axis.MajorUnit);
				if (axis.MajorTimeUnit != TimeUnits.Auto)
					GenerateChartSimpleStringAttributeTag("majorTimeUnit", TimeUnitTable[axis.MajorTimeUnit]);
				if (axis.FixedMinorUnit)
					GenerateChartSimpleDoubleAttributeTag("minorUnit", axis.MinorUnit);
				if (axis.MinorTimeUnit != TimeUnits.Auto)
					GenerateChartSimpleStringAttributeTag("minorTimeUnit", TimeUnitTable[axis.MinorTimeUnit]);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region SeriesAxis
		internal void GenerateAxisContent(SeriesAxis axis) {
			WriteStartElement("serAx", DrawingMLChartNamespace);
			try {
				GenerateAxisSharedContent(axis);
				if (axis.TickLabelSkip > 1)
					GenerateChartSimpleIntAttributeTag("tickLblSkip", axis.TickLabelSkip);
				if (axis.TickMarkSkip > 1)
					GenerateChartSimpleIntAttributeTag("tickMarkSkip", axis.TickMarkSkip);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region Major/Minor gridlines
		void GenerateAxisGridlines(string tag, ShapeProperties shapeProperties) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				GenerateChartShapeProperties(shapeProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
	#region AxisExportWalker
	public class AxisExportWalker : IAxisVisitor {
		readonly OpenXmlExporter exporter;
		public AxisExportWalker(OpenXmlExporter exporter) {
			this.exporter = exporter;
		}
		#region IAxisVisitor Members
		public void Visit(CategoryAxis axis) {
			exporter.GenerateAxisContent(axis);
		}
		public void Visit(ValueAxis axis) {
			exporter.GenerateAxisContent(axis);
		}
		public void Visit(DateAxis axis) {
			exporter.GenerateAxisContent(axis);
		}
		public void Visit(SeriesAxis axis) {
			exporter.GenerateAxisContent(axis);
		}
		#endregion
	}
	#endregion
}
