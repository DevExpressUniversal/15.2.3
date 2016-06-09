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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PlotAreaDestination
	public class PlotAreaDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("catAx", OnCategoryAxis);
			result.Add("dateAx", OnDateAxis);
			result.Add("layout", OnLayout);
			result.Add("serAx", OnSeriesAxis);
			result.Add("valAx", OnValueAxis);
			result.Add("dTable", OnDataTable);
			result.Add("spPr", OnShapeProperties);
			result.Add("area3DChart", OnArea3DChart);
			result.Add("areaChart", OnAreaChart);
			result.Add("bar3DChart", OnBar3DChart);
			result.Add("barChart", OnBarChart);
			result.Add("bubbleChart", OnBubbleChart);
			result.Add("doughnutChart", OnDoughnutChart);
			result.Add("line3DChart", OnLine3DChart);
			result.Add("lineChart", OnLineChart);
			result.Add("ofPieChart", OnOfPieChart);
			result.Add("pie3DChart", OnPie3DChart);
			result.Add("pieChart", OnPieChart);
			result.Add("radarChart", OnRadarChart);
			result.Add("scatterChart", OnScatterChart);
			result.Add("stockChart", OnStockChart);
			result.Add("surface3DChart", OnSurface3DChart);
			result.Add("surfaceChart", OnSurfaceChart);
			return result;
		}
		static PlotAreaDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PlotAreaDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Chart chart;
		readonly List<ChartAxisImportInfo> axisList;
		readonly Dictionary<int, AxisBase> axisByAxisId;
		readonly Dictionary<int, AxisGroup> axisGroupByAxisId;
		readonly Dictionary<IChartView, List<int>> axisIdListByView;
		int axisPerViewCount;
		bool emptySeriesAxis;
		#endregion
		public PlotAreaDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
			this.chart = chart;
			this.axisList = new List<ChartAxisImportInfo>();
			this.axisByAxisId = new Dictionary<int, AxisBase>();
			this.axisGroupByAxisId = new Dictionary<int, AxisGroup>();
			this.axisIdListByView = new Dictionary<IChartView, List<int>>();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal List<ChartAxisImportInfo> AxisList { get { return axisList; } }
		#endregion
		#region Handlers
		static Destination OnCategoryAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PlotAreaDestination thisDestination = GetThis(importer);
			return new CategoryAxisDestination(importer, new CategoryAxis(thisDestination.chart), thisDestination.axisList);
		}
		static Destination OnValueAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PlotAreaDestination thisDestination = GetThis(importer);
			return new ValueAxisDestination(importer, new ValueAxis(thisDestination.chart), thisDestination.axisList);
		}
		static Destination OnDateAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PlotAreaDestination thisDestination = GetThis(importer);
			return new DateAxisDestination(importer, new DateAxis(thisDestination.chart), thisDestination.axisList);
		}
		static Destination OnSeriesAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PlotAreaDestination thisDestination = GetThis(importer);
			return new SeriesAxisDestination(importer, new SeriesAxis(thisDestination.chart), thisDestination.axisList);
		}
		static Destination OnDataTable(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataTableDestination(importer, GetThis(importer).chart.DataTable);
		}
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).chart.PlotArea.Layout);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).chart.PlotArea.ShapeProperties);
		}
		static Destination OnArea3DChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Area3DChartView view = new Area3DChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new Area3DChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnAreaChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AreaChartView view = new AreaChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new AreaChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnBar3DChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Bar3DChartView view = new Bar3DChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new Bar3DChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnBarChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartView view = new BarChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new BarChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnBubbleChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BubbleChartView view = new BubbleChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new BubbleChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnDoughnutChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DoughnutChartView view = new DoughnutChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new DoughnutChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnLine3DChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Line3DChartView view = new Line3DChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new Line3DChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnLineChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineChartView view = new LineChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new LineChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnOfPieChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = new OfPieChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new OfPieChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnPie3DChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Pie3DChartView view = new Pie3DChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new Pie3DChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnPieChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PieChartView view = new PieChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new PieChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnRadarChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			RadarChartView view = new RadarChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new RadarChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnScatterChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScatterChartView view = new ScatterChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new ScatterChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnStockChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StockChartView view = new StockChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new StockChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnSurface3DChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Surface3DChartView view = new Surface3DChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new SurfaceChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		static Destination OnSurfaceChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SurfaceChartView view = new SurfaceChartView(GetThis(importer).chart);
			GetThis(importer).RegisterView(view);
			return new SurfaceChartViewDestination(importer, view, GetThis(importer).GetAxesIdList(view));
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			ProcessAxes();
		}
		protected internal virtual void ProcessAxes() {
			CheckChartConsistency();
			BindAxesToAxisGroups();
			BindViewsToAxisGroups();
			SetupAxesCrosses();
			NormalizeSecondaryAxisGroup();
		}
		#region Internals
		int GetAxisCount(AxisGroupType axisGroupType) {
			switch (axisGroupType) {
				case AxisGroupType.CategoryValue:
				case AxisGroupType.XY:
					return 2;
				case AxisGroupType.CategoryValueSeries:
					return 3;
			}
			return 0;
		}
		bool IsCompatibleAxisGroups(AxisGroupType first, AxisGroupType second) {
			if (first == AxisGroupType.Empty && second == AxisGroupType.Empty)
				return true;
			if (first == AxisGroupType.CategoryValue && second == AxisGroupType.CategoryValue)
				return true;
			if (first == AxisGroupType.CategoryValue && second == AxisGroupType.XY)
				return true;
			if (first == AxisGroupType.XY && second == AxisGroupType.CategoryValue)
				return true;
			if (first == AxisGroupType.XY && second == AxisGroupType.XY)
				return true;
			return false;
		}
		void CheckChartConsistency() {
			int viewsCount = chart.Views.Count;
			if (viewsCount < 1)
				Importer.ThrowInvalidFile("chart without views");
			if (viewsCount > 1) {
				for (int i = 0; i < viewsCount; i++) {
					if (chart.Views[i].Is3DView)
						Importer.ThrowInvalidFile("chart with multiple 3D views");
				}
			}
			IChartView firstView = chart.Views[0];
			AxisGroupType axisGroupType = firstView.AxesType;
			for (int i = 1; i < viewsCount; i++) {
				if (!IsCompatibleAxisGroups(chart.Views[i].AxesType, axisGroupType))
					Importer.ThrowInvalidFile("chart views with incompatible axis group types");
			}
			int axisCount = axisList.Count;
			this.axisPerViewCount = GetAxisCount(axisGroupType);
			if (axisPerViewCount == 0) {
				if (axisCount != 0)
					Importer.ThrowInvalidFile(string.Format("expected axis count {0}, actual {1}", axisPerViewCount, axisCount));
			}
			else {
				if (axisCount == 0)
					Importer.ThrowInvalidFile(string.Format("expected axis count {0}, actual {1}", axisPerViewCount, axisCount));
				int axisGroupsCount = axisCount / axisPerViewCount;
				emptySeriesAxis = CheckEmptySeriesAxis(axisGroupType);
				if ((axisGroupsCount != 1 && axisGroupsCount != 2 || (axisCount != axisGroupsCount * axisPerViewCount && firstView.ViewType != ChartViewType.Surface)) && !emptySeriesAxis)
					Importer.ThrowInvalidFile("invalid number of axis");
			}
		}
		bool CheckEmptySeriesAxis(AxisGroupType axisGroupType) {
			if (axisGroupType != AxisGroupType.CategoryValueSeries)
				return false;
			int count = axisList.Count;
			if (count == 3)
				return false;
			AxisDataType firstAxisType = axisList[0].Axis.AxisType;
			AxisDataType secondAxisType = axisList[1].Axis.AxisType;
			bool hasAgrumentAxis = firstAxisType == AxisDataType.Agrument || secondAxisType == AxisDataType.Agrument;
			bool hasValueAxis = firstAxisType == AxisDataType.Value || secondAxisType == AxisDataType.Value;
			return hasAgrumentAxis && hasValueAxis;
		}
		void BindAxesToAxisGroups() {
			for (int i = 0; i < axisList.Count; i++) {
				ChartAxisImportInfo axisInfo = axisList[i];
				axisByAxisId.Add(axisInfo.Id, axisInfo.Axis);
				AxisGroup axisGroup = i < axisPerViewCount ? chart.PrimaryAxes : chart.SecondaryAxes;
				axisGroup.AddWithoutHistoryAndNotifications(axisInfo.Axis);
				axisGroupByAxisId.Add(axisInfo.Id, axisGroup);
			}
		}
		void BindViewsToAxisGroups() {
			foreach (KeyValuePair<IChartView, List<int>> viewAxes in axisIdListByView) {
				IChartView view = viewAxes.Key;
				List<int> axisIdList = viewAxes.Value;
				int count = axisIdList.Count;
				if (count != axisPerViewCount && view.ViewType != ChartViewType.Surface && !emptySeriesAxis)
					Importer.ThrowInvalidFile(string.Format("{0} expected axis count {1}, actual {2}", view.ViewType, axisPerViewCount, count));
				if (count > 0) {
					AxisGroup axisGroup;
					if (!axisGroupByAxisId.TryGetValue(axisIdList[0], out axisGroup))
						Importer.ThrowInvalidFile("unable to find axis group by axisId");
					view.Axes = axisGroup;
					int axisIdCount = emptySeriesAxis ? count : axisPerViewCount;
					for (int i = 1; i < axisIdCount; i++) 
						if (!axisGroupByAxisId.TryGetValue(axisIdList[i], out axisGroup) || !Object.ReferenceEquals(axisGroup, view.Axes))
							Importer.ThrowInvalidFile("corrupted axes/views references");
				}
				else
					view.Axes = chart.PrimaryAxes;
			}
		}
		void SetupAxesCrosses() {
			foreach (ChartAxisImportInfo axisInfo in axisList) {
				AxisBase crossesAxis;
				if (!axisByAxisId.TryGetValue(axisInfo.CrossesAxisId, out crossesAxis))
					Importer.ThrowInvalidFile("unable to find crosses axis");
				axisInfo.Axis.CrossesAxis = crossesAxis;
			}
			foreach (ChartAxisImportInfo axisInfo in axisList) {
				AxisBase axis = axisInfo.Axis;
				if (axis.CrossesAxis == null || Object.ReferenceEquals(axis, axis.CrossesAxis))
					Importer.ThrowInvalidFile("invalid axis crossing");
			}
		}
		void NormalizeSecondaryAxisGroup() {
			foreach (IChartView view in chart.Views) {
				if (view.Axes == chart.SecondaryAxes) {
					List<int> axisIdList = axisIdListByView[view];
					ChartAxisImportInfo axisInfo = axisList[axisPerViewCount];
					if(axisIdList[0] != axisInfo.Id)
						chart.SecondaryAxes.Reverse();
					return;
				}
			}
		}
		void RegisterView(IChartView view) {
			this.chart.Views.AddCore(view);
			this.axisIdListByView.Add(view, new List<int>());
		}
		List<int> GetAxesIdList(IChartView view) {
			return this.axisIdListByView[view];
		}
		#endregion
	}
	#endregion
}
