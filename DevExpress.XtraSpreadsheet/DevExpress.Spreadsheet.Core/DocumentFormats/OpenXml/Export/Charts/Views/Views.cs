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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<BarChartDirection, string> BarChartDirectionTable = CreateBarChartDirectionTable();
		internal static Dictionary<BarChartGrouping, string> BarChartGroupingTable = CreateBarChartGroupingTable();
		internal static Dictionary<BarShape, string> BarChartShapeTable = CreateBarChartShapeTable();
		internal static Dictionary<DataLabelPosition, string> DataLabelPositionTable = CreateDataLabelPositionTable();
		internal static Dictionary<MarkerStyle, string> MarkerStyleTable = CreateMarkerStyleTable();
		internal static Dictionary<ErrorBarDirection, string> ErrorBarDirectionTable = CreateErrorBarDirectionTable();
		internal static Dictionary<ErrorBarType, string> ErrorBarTypeTable = CreateErrorBarTypeTable();
		internal static Dictionary<ErrorValueType, string> ErrorValueTypeTable = CreateErrorValueTypeTable();
		internal static Dictionary<ChartGrouping, string> ChartGroupingTable = CreateChartGroupingTable();
		internal static Dictionary<SizeRepresentsType, string> SizeRepresentsTable = CreateSizeRepresentsTable();
		static Dictionary<BarChartDirection, string> CreateBarChartDirectionTable() {
			Dictionary<BarChartDirection, string> result = new Dictionary<BarChartDirection, string>();
			result.Add(BarChartDirection.Column, "col");
			result.Add(BarChartDirection.Bar, "bar");
			return result;
		}
		static Dictionary<BarChartGrouping, string> CreateBarChartGroupingTable() {
			Dictionary<BarChartGrouping, string> result = new Dictionary<BarChartGrouping, string>();
			result.Add(BarChartGrouping.Clustered, "clustered");
			result.Add(BarChartGrouping.PercentStacked, "percentStacked");
			result.Add(BarChartGrouping.Stacked, "stacked");
			result.Add(BarChartGrouping.Standard, "standard");
			return result;
		}
		static Dictionary<BarShape, string> CreateBarChartShapeTable() {
			Dictionary<BarShape, string> result = new Dictionary<BarShape, string>();
			result.Add(BarShape.Box, "box");
			result.Add(BarShape.Cone, "cone");
			result.Add(BarShape.ConeToMax, "coneToMax");
			result.Add(BarShape.Cylinder, "cylinder");
			result.Add(BarShape.Pyramid, "pyramid");
			result.Add(BarShape.PyramidToMax, "pyramidToMax");
			return result;
		}
		static Dictionary<DataLabelPosition, string> CreateDataLabelPositionTable() {
			Dictionary<DataLabelPosition, string> result = new Dictionary<DataLabelPosition, string>();
			result.Add(DataLabelPosition.BestFit, "bestFit");
			result.Add(DataLabelPosition.Bottom, "b");
			result.Add(DataLabelPosition.Center, "ctr");
			result.Add(DataLabelPosition.InsideBase, "inBase");
			result.Add(DataLabelPosition.InsideEnd, "inEnd");
			result.Add(DataLabelPosition.Left, "l");
			result.Add(DataLabelPosition.OutsideEnd, "outEnd");
			result.Add(DataLabelPosition.Right, "r");
			result.Add(DataLabelPosition.Top, "t");
			return result;
		}
		static Dictionary<MarkerStyle, string> CreateMarkerStyleTable() {
			Dictionary<MarkerStyle, string> result = new Dictionary<MarkerStyle, string>();
			result.Add(MarkerStyle.Circle, "circle");
			result.Add(MarkerStyle.Dash, "dash");
			result.Add(MarkerStyle.Diamond, "diamond");
			result.Add(MarkerStyle.Dot, "dot");
			result.Add(MarkerStyle.None, "none");
			result.Add(MarkerStyle.Picture, "picture");
			result.Add(MarkerStyle.Plus, "plus");
			result.Add(MarkerStyle.Square, "square");
			result.Add(MarkerStyle.Star, "star");
			result.Add(MarkerStyle.Triangle, "triangle");
			result.Add(MarkerStyle.X, "x");
			return result;
		}
		static Dictionary<ErrorBarDirection, string> CreateErrorBarDirectionTable() {
			Dictionary<ErrorBarDirection, string> result = new Dictionary<ErrorBarDirection, string>();
			result.Add(ErrorBarDirection.X, "x");
			result.Add(ErrorBarDirection.Y, "y");
			return result;
		}
		static Dictionary<ErrorBarType, string> CreateErrorBarTypeTable() {
			Dictionary<ErrorBarType, string> result = new Dictionary<ErrorBarType, string>();
			result.Add(ErrorBarType.Both, "both");
			result.Add(ErrorBarType.Minus, "minus");
			result.Add(ErrorBarType.Plus, "plus");
			return result;
		}
		static Dictionary<ErrorValueType, string> CreateErrorValueTypeTable() {
			Dictionary<ErrorValueType, string> result = new Dictionary<ErrorValueType, string>();
			result.Add(ErrorValueType.Custom, "cust");
			result.Add(ErrorValueType.FixedValue, "fixedVal");
			result.Add(ErrorValueType.Percentage, "percentage");
			result.Add(ErrorValueType.StandardDeviation, "stdDev");
			result.Add(ErrorValueType.StandardError, "stdErr");
			return result;
		}
		static Dictionary<ChartGrouping, string> CreateChartGroupingTable() {
			Dictionary<ChartGrouping, string> result = new Dictionary<ChartGrouping, string>();
			result.Add(ChartGrouping.PercentStacked, "percentStacked");
			result.Add(ChartGrouping.Stacked, "stacked");
			result.Add(ChartGrouping.Standard, "standard");
			return result;
		}
		static Dictionary<SizeRepresentsType, string> CreateSizeRepresentsTable() {
			Dictionary<SizeRepresentsType, string> result = new Dictionary<SizeRepresentsType, string>();
			result.Add(SizeRepresentsType.Area, "area");
			result.Add(SizeRepresentsType.Width, "w");
			return result;
		}
		#endregion
		protected internal void GenerateChartViews(ChartViewCollection views) {
			ChartViewsExportWalker walker = new ChartViewsExportWalker(this);
			foreach (IChartView view in views)
				view.Visit(walker);
		}
		protected internal void GenerateAxisGroupRef(IChartView view) {
			foreach (AxisBase axis in view.Axes)
				GenerateChartSimpleIntAttributeTag("axId", axisIdTable[axis]);
			if (view.Is3DView && (view.Axes.Count == 2))
				GenerateChartSimpleIntAttributeTag("axId", 0);
		}
	}
	#region ChartViewsExportWalker
	public class ChartViewsExportWalker : IChartViewVisitor {
		readonly OpenXmlExporter exporter;
		public ChartViewsExportWalker(OpenXmlExporter exporter) {
			this.exporter = exporter;
		}
		#region IChartViewVisitor Members
		public void Visit(Area3DChartView view) {
			exporter.GenerateArea3DChartView(view);
		}
		public void Visit(AreaChartView view) {
			exporter.GenerateAreaChartView(view);
		}
		public void Visit(Bar3DChartView view) {
			exporter.GenerateBar3DChartView(view);
		}
		public void Visit(BarChartView view) {
			exporter.GenerateBarChartView(view);
		}
		public void Visit(BubbleChartView view) {
			exporter.GenerateBubbleChartView(view);
		}
		public void Visit(DoughnutChartView view) {
			exporter.GenerateDoughnutChartView(view);
		}
		public void Visit(Line3DChartView view) {
			exporter.GenerateLine3DChartView(view);
		}
		public void Visit(LineChartView view) {
			exporter.GenerateLineChartView(view);
		}
		public void Visit(OfPieChartView view) {
			exporter.GenerateOfPieChartView(view);
		}
		public void Visit(Pie3DChartView view) {
			exporter.GeneratePie3DChartView(view);
		}
		public void Visit(PieChartView view) {
			exporter.GeneratePieChartView(view);
		}
		public void Visit(RadarChartView view) {
			exporter.GenerateRadarChartView(view);
		}
		public void Visit(ScatterChartView view) {
			exporter.GenerateScatterChartView(view);
		}
		public void Visit(StockChartView view) {
			exporter.GenerateStockChartView(view);
		}
		public void Visit(Surface3DChartView view) {
			exporter.GenerateSurface3DChartView(view);
		}
		public void Visit(SurfaceChartView view) {
			exporter.GenerateSurfaceChartView(view);
		}
		#endregion
	}
	#endregion
}
