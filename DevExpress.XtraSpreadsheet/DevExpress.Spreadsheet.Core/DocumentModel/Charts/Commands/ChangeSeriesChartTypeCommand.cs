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
using DevExpress.Utils;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChangeSeriesChartTypeBuilderBase (abstract class)
	public abstract class ChangeSeriesChartTypeBuilderBase {
		#region Fields
		readonly ChartViewType viewType;
		ISeries targetSeries;
		ISeries newSeries;
		IChartView newView;
		#endregion
		protected ChangeSeriesChartTypeBuilderBase(ChartViewType viewType) {
			this.viewType = viewType;
		}
		#region Properties
		protected Chart Chart { get { return TargetSeries.View.Parent as Chart; } }
		protected ChartViewCollection Views { get { return Chart.Views; } }
		protected internal ISeries TargetSeries {
			get { return targetSeries; }
			set {
				Guard.ArgumentNotNull(value, "Serie");
				targetSeries = value;
			}
		}
		protected internal IChartView TargetView { get { return TargetSeries.View; } }
		protected internal ISeries NewSeries { get { return newSeries; } protected set { newSeries = value; } }
		protected internal IChartView NewView { get { return newView; } protected set { newView = value; } }
		protected internal bool IsCreateView { get { return TargetView.ViewType != viewType; } }
		#endregion
		public void Build() {
			if (Chart == null)
				return;
			if (IsCreateView) 
				CreateNewView(viewType);
			else
				SetNewSeries(targetSeries);
		}
		protected virtual void CreateNewView(ChartViewType viewType) {
			CreateNewView();
			CopySeries();
			Views.Clear();
			Views.Add(newView);
			ModifyChart();
		}
		void CreateNewView() {
			newView = CreateViewInstance();
			ModifyNewViewProperties();
			newView.CopyFromWithoutSeries(TargetView);
			newView.Axes = Chart.PrimaryAxes;
		}
		void ModifyChart() {
			ChartBuilderBase builder = ChartBuilderFactory.GetBuilder(NewView.ChartType);
			if (builder == null)
				return;
			builder.SetupAxes(Chart);
			if (NewView.Is3DView)
				builder.SetupView3D(Chart.View3D);
		}
		void SetNewSeries(ISeries series) {
			newView = series.View;
			ModifyNewViewProperties();
			newSeries = series;
			ModifyNewSeriesProperties();
		}
		#region CopySeries
		void CopySeries() {
			IList<ISeries> allSeries = GetAllSeries();
			CopySeriesCore(allSeries, newView.Series);
		}
		IList<ISeries> GetAllSeries() {
			List<ISeries> result = new List<ISeries>();
			int viewCount = Views.Count;
			for (int i = 0; i < viewCount; i++)
				result.AddRange(GetNewViewSeries(Views[i].Series));
			return result;
		}
		IList<ISeries> GetNewViewSeries(SeriesCollection oldViewSeries) {
			IList<ISeries> result = new List<ISeries>();
			int oldSeriesCount = oldViewSeries.Count;
			for (int j = 0; j < oldSeriesCount; j++) {
				ISeries oldSeries = oldViewSeries[j];
				ISeries newSeries = newView.CreateSeriesInstance();
				newSeries.CopyFrom(oldSeries);
				if (newSeries.Order == TargetSeries.Order) 
					SetNewSeries(newSeries);
				result.Add(newSeries);
			}
			return result;
		}
		void CopySeriesCore(IList<ISeries> from, SeriesCollection to) {
			int count = from.Count;
			for (int i = 0; i < count; i++)
				to.Add(from[i]);
		}
		#endregion
		protected virtual void ModifyNewViewProperties() {
		}
		protected virtual void ModifyNewSeriesProperties() {
		}
		protected abstract IChartView CreateViewInstance();
	}
	#endregion
	#region ChangeArea3DChartTypeBuilder
	public class ChangeArea3DChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly ChartGrouping grouping;
		public ChangeArea3DChartTypeBuilder(ChartGrouping grouping)
			: base(ChartViewType.Area3D) {
			this.grouping = grouping;
		}
		protected override void ModifyNewViewProperties() {
			Area3DChartView view = NewView as Area3DChartView;
			if (view != null)
				view.Grouping = grouping;
		}
		protected override IChartView CreateViewInstance() {
			return new Area3DChartView(Chart);
		}
	}
	#endregion
	#region ChangeBar3DChartTypeBuilder
	public class ChangeBar3DChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		#region Fields
		readonly BarChartDirection direction;
		readonly BarChartGrouping grouping;
		readonly BarShape shape;
		#endregion
		public ChangeBar3DChartTypeBuilder(BarChartDirection direction, BarChartGrouping grouping, BarShape shape)
			: base(ChartViewType.Bar3D) {
			this.direction = direction;
			this.grouping = grouping;
			this.shape = shape;
		}
		protected override void ModifyNewViewProperties() {
			Bar3DChartView view = NewView as Bar3DChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.BarDirection = direction;
					view.Grouping = grouping;
					if (!(TargetView is Bar3DChartView))
						view.Shape = shape;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override void ModifyNewSeriesProperties() {
			BarSeries barSeries = NewSeries as BarSeries;
			if (barSeries != null)
				barSeries.Shape = shape;
		}
		protected override IChartView CreateViewInstance() {
			return new Bar3DChartView(Chart);
		}
	}
	#endregion
	#region ChangeBubbleChartTypeBuilder
	public class ChangeBubbleChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool bubble3D;
		public ChangeBubbleChartTypeBuilder(bool bubble3D)
			: base(ChartViewType.Bubble) {
			this.bubble3D = bubble3D;
		}
		protected override void ModifyNewViewProperties() {
			BubbleChartView view = NewView as BubbleChartView;
			if (view != null)
				view.Bubble3D = bubble3D;
		}
		protected override void ModifyNewSeriesProperties() {
			BubbleSeries series = NewSeries as BubbleSeries;
			if (series != null)
				series.Bubble3D = bubble3D;
		}
		protected override IChartView CreateViewInstance() {
			return new BubbleChartView(Chart);
		}
	}
	#endregion
	#region ChangeLine3DChartTypeBuilder
	public class ChangeLine3DChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		public ChangeLine3DChartTypeBuilder()
			: base(ChartViewType.Line3D) {
		}
		protected override IChartView CreateViewInstance() {
			return new Line3DChartView(Chart);
		}
	}
	#endregion
	#region ChangePieChartTypeBuilder
	public class ChangePieChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool exploded;
		public ChangePieChartTypeBuilder(bool exploded)
			: base(ChartViewType.Pie) {
			this.exploded = exploded;
		}
		protected override void ModifyNewViewProperties() {
			PieChartView view = NewView as PieChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.Exploded = exploded;
					view.VaryColors = true;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override IChartView CreateViewInstance() {
			return new PieChartView(Chart);
		}
	}
	#endregion
	#region ChangePie3DChartTypeBuilder
	public class ChangePie3DChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool exploded;
		public ChangePie3DChartTypeBuilder(bool exploded)
			: base(ChartViewType.Pie3D) {
			this.exploded = exploded;
		}
		protected override void ModifyNewViewProperties() {
			Pie3DChartView view = NewView as Pie3DChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.Exploded = exploded;
					view.VaryColors = true;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override IChartView CreateViewInstance() {
			return new Pie3DChartView(Chart);
		}
	}
	#endregion
	#region ChangeDoughnutChartTypeBuilder
	public class ChangeDoughnutChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool exploded;
		public ChangeDoughnutChartTypeBuilder(bool exploded)
			: base(ChartViewType.Doughnut) {
			this.exploded = exploded;
		}
		protected override void ModifyNewViewProperties() {
			DoughnutChartView view = NewView as DoughnutChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.Exploded = exploded;
					view.VaryColors = true;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override IChartView CreateViewInstance() {
			return new DoughnutChartView(Chart);
		}
	}
	#endregion
	#region ChangeOfPieChartTypeBuilder
	public class ChangeOfPieChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly ChartOfPieType ofPieType;
		public ChangeOfPieChartTypeBuilder(ChartOfPieType ofPieType)
			: base(ChartViewType.OfPie) {
			this.ofPieType = ofPieType;
		}
		protected override void ModifyNewViewProperties() {
			OfPieChartView view = NewView as OfPieChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.OfPieType = ofPieType;
					view.VaryColors = true;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override IChartView CreateViewInstance() {
			return new OfPieChartView(Chart);
		}
	}
	#endregion
	#region ChangeScatterChartTypeBuilder
	public class ChangeScatterChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly ScatterChartStyle style;
		public ChangeScatterChartTypeBuilder(ScatterChartStyle style)
			: base(ChartViewType.Scatter) {
			this.style = style;
		}
		protected override void ModifyNewViewProperties() {
			ScatterChartView view = NewView as ScatterChartView;
			if (view != null)
				view.ScatterStyle = style;
		}
		protected override void ModifyNewSeriesProperties() {
			ScatterSeries scatterSeries = NewSeries as ScatterSeries;
			if (scatterSeries != null)
				scatterSeries.SetScatterStyle(style);
		}
		protected override IChartView CreateViewInstance() {
			return new ScatterChartView(Chart);
		}
	}
	#endregion
	#region ChangeStockChartTypeBuilder
	public class ChangeStockChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool showUpDownBars;
		public ChangeStockChartTypeBuilder(bool showUpDownBars)
			: base(ChartViewType.Stock) {
			this.showUpDownBars = showUpDownBars;
		}
		protected override void ModifyNewViewProperties() {
			StockChartView view = NewView as StockChartView;
			if (view != null)
				view.ShowUpDownBars = showUpDownBars;
		}
		protected override IChartView CreateViewInstance() {
			return new StockChartView(Chart);
		}
	}
	#endregion
	#region ChangeSurfaceChartTypeBuilder
	public class ChangeSurfaceChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool wireframe;
		public ChangeSurfaceChartTypeBuilder(bool wireframe)
			: base(ChartViewType.Surface) {
			this.wireframe = wireframe;
		}
		protected override void ModifyNewViewProperties() {
			SurfaceChartView view = NewView as SurfaceChartView;
			if (view != null)
				view.Wireframe = wireframe;
		}
		protected override IChartView CreateViewInstance() {
			return new SurfaceChartView(Chart);
		}
	}
	#endregion
	#region ChangeSurface3DChartTypeBuilder
	public class ChangeSurface3DChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly bool wireframe;
		public ChangeSurface3DChartTypeBuilder(bool wireframe)
			: base(ChartViewType.Surface3D) {
			this.wireframe = wireframe;
		}
		protected override void ModifyNewViewProperties() {
			Surface3DChartView view = NewView as Surface3DChartView;
			if (view != null)
				view.Wireframe = wireframe;
		}
		protected override IChartView CreateViewInstance() {
			return new Surface3DChartView(Chart);
		}
	}
	#endregion
	#region ChangeRadarChartTypeBuilder
	public class ChangeRadarChartTypeBuilder : ChangeSeriesChartTypeBuilderBase {
		readonly RadarChartStyle style;
		public ChangeRadarChartTypeBuilder(RadarChartStyle style)
			: base(ChartViewType.Radar) {
			this.style = style;
		}
		protected override void ModifyNewViewProperties() {
			RadarChartView view = NewView as RadarChartView;
			if (view != null)
				view.RadarStyle = style;
		}
		protected override void ModifyNewSeriesProperties() {
			if (style == RadarChartStyle.Filled)
				return;
			RadarSeries radarSeries = NewSeries as RadarSeries;
			if (radarSeries != null)
				radarSeries.Marker.Symbol = style == RadarChartStyle.Standard ? MarkerStyle.None : MarkerStyle.Auto;
		}
		protected override IChartView CreateViewInstance() {
			return new RadarChartView(Chart);
		}
	}
	#endregion
	#region ChangeCombinedSeriesChartTypeBuilderBase (abstract class)
	public abstract class ChangeCombinedSeriesChartTypeBuilderBase : ChangeSeriesChartTypeBuilderBase {
		protected ChangeCombinedSeriesChartTypeBuilderBase(ChartViewType viewType)
			: base(viewType) {
		}
		bool IsCompatibleTargetView {
			get {
				ChartViewType viewType = TargetView.ViewType;
				return
					viewType == ChartViewType.Area ||
					viewType == ChartViewType.Bar ||
					viewType == ChartViewType.Line;
			}
		}
		protected override void CreateNewView(ChartViewType viewType) {
			if (IsCompatibleTargetView) {
				IChartView view = Views.TryGetView(viewType, TargetView.Axes);
				if (view == null) {
					NewView = CreateViewInstance();
					NewView.CopyFromWithoutSeries(TargetView);
					NewView.Axes = TargetView.Axes;
					Views.Insert(GetNewViewIndex(), NewView);
				} else 
					NewView = view;
				ModifyNewViewProperties();
				AddNewSeries();
			} else
				base.CreateNewView(viewType);
		}
		void AddNewSeries() {
			NewSeries = NewView.CreateSeriesInstance();
			NewSeries.CopyFrom(TargetSeries);
			TargetView.Series.Remove(TargetSeries);
			NewView.Series.Add(NewSeries);
			if (TargetView.Series.Count == 0)
				Views.Remove(TargetView);
		}
		protected int GetNewViewIndex() {
			int result = TryGetNewIndexByNewViewType();
			return result != -1 ? result : GetDefaultNewViewIndex();
		}
		int TryGetNewIndexByNewViewType() {
			AxisGroup newViewAxes = NewView.Axes;
			ChartViewType newViewType = NewView.ViewType;
			if (newViewAxes == Chart.SecondaryAxes) {
				int primaryIndex = Views.TryGetViewIndex(newViewType, Chart.PrimaryAxes);
				if (primaryIndex != -1)
					return primaryIndex + 1;
			}
			if (newViewAxes == Chart.PrimaryAxes) {
				int secondaryIndex = Views.TryGetViewIndex(newViewType, Chart.SecondaryAxes);
				if (secondaryIndex != -1)
					return secondaryIndex;
			}
			return -1;
		}
		protected abstract int GetDefaultNewViewIndex();
	}
	#endregion
	#region ChangeAreaChartTypeBuilder
	public class ChangeAreaChartTypeBuilder : ChangeCombinedSeriesChartTypeBuilderBase {
		readonly ChartGrouping grouping;
		public ChangeAreaChartTypeBuilder(ChartGrouping grouping)
			: base(ChartViewType.Area) {
			this.grouping = grouping;
		}
		protected override void ModifyNewViewProperties() {
			AreaChartView view = NewView as AreaChartView;
			if (view != null)
				view.Grouping = grouping;
		}
		protected override IChartView CreateViewInstance() {
			return new AreaChartView(Chart);
		}
		protected override int GetDefaultNewViewIndex() {
			return 0;
		}
	}
	#endregion
	#region ChangeBarChartTypeBuilder
	public class ChangeBarChartTypeBuilder : ChangeCombinedSeriesChartTypeBuilderBase {
		#region Fields
		readonly BarChartDirection direction;
		readonly BarChartGrouping grouping;
		#endregion
		public ChangeBarChartTypeBuilder(BarChartDirection direction, BarChartGrouping grouping)
			: base(ChartViewType.Bar) {
			this.direction = direction;
			this.grouping = grouping;
		}
		protected override void ModifyNewViewProperties() {
			BarChartView view = NewView as BarChartView;
			if (view != null) {
				view.BeginUpdate();
				try {
					view.BarDirection = direction;
					view.Grouping = grouping;
					if (grouping == BarChartGrouping.Stacked || grouping == BarChartGrouping.PercentStacked)
						view.Overlap = 100;
				} finally {
					view.EndUpdate();
				}
			}
		}
		protected override IChartView CreateViewInstance() {
			return new BarChartView(Chart);
		}
		protected override int GetDefaultNewViewIndex() {
			int secondaryIndex = Views.TryGetViewIndex(ChartViewType.Area, Chart.SecondaryAxes);
			if (secondaryIndex != -1)
				return secondaryIndex + 1;
			int primaryIndex = Views.TryGetViewIndex(ChartViewType.Area, Chart.PrimaryAxes);
			if (primaryIndex != -1)
				return primaryIndex + 1;
			return 0;
		}
	}
	#endregion
	#region ChangeLineChartTypeBuilder
	public class ChangeLineChartTypeBuilder : ChangeCombinedSeriesChartTypeBuilderBase {
		#region Fields
		readonly ChartGrouping grouping;
		readonly bool showMarker;
		#endregion
		public ChangeLineChartTypeBuilder(ChartGrouping grouping, bool showMarker)
			: base(ChartViewType.Line) {
			this.grouping = grouping;
			this.showMarker = showMarker;
		}
		protected override void ModifyNewViewProperties() {
			LineChartView view = NewView as LineChartView;
			if (view != null) {
				 view.BeginUpdate();
				 try {
					 view.Grouping = grouping;
					 view.ShowMarker = showMarker;
				 } finally {
					 view.EndUpdate();
				 } 
			}
		}
		protected override IChartView CreateViewInstance() {
			return new LineChartView(Chart);
		}
		protected override int GetDefaultNewViewIndex() {
			return Views.Count;
		}
	}
	#endregion
	#region ChangeSeriesChartTypeFactory
	public static class ChangeSeriesChartTypeFactory {
		static Dictionary<ChartType, ChangeSeriesChartTypeBuilderBase> builderTable = GetBuilderTable();
		static Dictionary<ChartType, ChangeSeriesChartTypeBuilderBase> GetBuilderTable() {
			Dictionary<ChartType, ChangeSeriesChartTypeBuilderBase> result = new Dictionary<ChartType, ChangeSeriesChartTypeBuilderBase>();
			result.Add(ChartType.Area, new ChangeAreaChartTypeBuilder(ChartGrouping.Standard));
			result.Add(ChartType.AreaStacked, new ChangeAreaChartTypeBuilder(ChartGrouping.Stacked));
			result.Add(ChartType.AreaFullStacked, new ChangeAreaChartTypeBuilder(ChartGrouping.PercentStacked));
			result.Add(ChartType.Area3D, new ChangeArea3DChartTypeBuilder(ChartGrouping.Standard));
			result.Add(ChartType.Area3DStacked, new ChangeArea3DChartTypeBuilder(ChartGrouping.Stacked));
			result.Add(ChartType.Area3DFullStacked, new ChangeArea3DChartTypeBuilder(ChartGrouping.PercentStacked));
			result.Add(ChartType.BarStacked, new ChangeBarChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked));
			result.Add(ChartType.BarClustered, new ChangeBarChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered));
			result.Add(ChartType.BarFullStacked, new ChangeBarChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked));
			result.Add(ChartType.Bar3DStacked, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Box));
			result.Add(ChartType.Bar3DClustered, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Box));
			result.Add(ChartType.Bar3DFullStacked, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Box));
			result.Add(ChartType.Bar3DStackedCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Cone));
			result.Add(ChartType.Bar3DClusteredCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Cone));
			result.Add(ChartType.Bar3DFullStackedCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Cone));
			result.Add(ChartType.Bar3DStackedCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Cylinder));
			result.Add(ChartType.Bar3DClusteredCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Cylinder));
			result.Add(ChartType.Bar3DFullStackedCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Cylinder));
			result.Add(ChartType.Bar3DStackedPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Stacked, BarShape.Pyramid));
			result.Add(ChartType.Bar3DClusteredPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.Clustered, BarShape.Pyramid));
			result.Add(ChartType.Bar3DFullStackedPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Bar, BarChartGrouping.PercentStacked, BarShape.Pyramid));
			result.Add(ChartType.ColumnStacked, new ChangeBarChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Stacked));
			result.Add(ChartType.ColumnClustered, new ChangeBarChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Clustered));
			result.Add(ChartType.ColumnFullStacked, new ChangeBarChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked));
			result.Add(ChartType.Column3DStandard, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Box));
			result.Add(ChartType.Column3DStacked, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Box));
			result.Add(ChartType.Column3DClustered, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Box));
			result.Add(ChartType.Column3DFullStacked, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Box));
			result.Add(ChartType.Column3DStandardCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Cone));
			result.Add(ChartType.Column3DStackedCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Cone));
			result.Add(ChartType.Column3DClusteredCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Cone));
			result.Add(ChartType.Column3DFullStackedCone, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Cone));
			result.Add(ChartType.Column3DStandardCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Cylinder));
			result.Add(ChartType.Column3DStackedCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Cylinder));
			result.Add(ChartType.Column3DClusteredCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Cylinder));
			result.Add(ChartType.Column3DFullStackedCylinder, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Cylinder));
			result.Add(ChartType.Column3DStandardPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Standard, BarShape.Pyramid));
			result.Add(ChartType.Column3DStackedPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Stacked, BarShape.Pyramid));
			result.Add(ChartType.Column3DClusteredPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.Clustered, BarShape.Pyramid));
			result.Add(ChartType.Column3DFullStackedPyramid, new ChangeBar3DChartTypeBuilder(BarChartDirection.Column, BarChartGrouping.PercentStacked, BarShape.Pyramid));
			result.Add(ChartType.Line, new ChangeLineChartTypeBuilder(ChartGrouping.Standard, false));
			result.Add(ChartType.LineStacked, new ChangeLineChartTypeBuilder(ChartGrouping.Stacked, false));
			result.Add(ChartType.LineFullStacked, new ChangeLineChartTypeBuilder(ChartGrouping.PercentStacked, false));
			result.Add(ChartType.LineMarker, new ChangeLineChartTypeBuilder(ChartGrouping.Standard, true));
			result.Add(ChartType.LineStackedMarker, new ChangeLineChartTypeBuilder(ChartGrouping.Stacked, true));
			result.Add(ChartType.LineFullStackedMarker, new ChangeLineChartTypeBuilder(ChartGrouping.PercentStacked, true));
			result.Add(ChartType.Line3D, new ChangeLine3DChartTypeBuilder());
			result.Add(ChartType.Pie, new ChangePieChartTypeBuilder(false));
			result.Add(ChartType.Pie3D, new ChangePie3DChartTypeBuilder(false));
			result.Add(ChartType.PieExploded, new ChangePieChartTypeBuilder(true));
			result.Add(ChartType.Pie3DExploded, new ChangePie3DChartTypeBuilder(true));
			result.Add(ChartType.Doughnut, new ChangeDoughnutChartTypeBuilder(false));
			result.Add(ChartType.DoughnutExploded, new ChangeDoughnutChartTypeBuilder(true));
			result.Add(ChartType.PieOfPie, new ChangeOfPieChartTypeBuilder(ChartOfPieType.Pie));
			result.Add(ChartType.BarOfPie, new ChangeOfPieChartTypeBuilder(ChartOfPieType.Bar));
			result.Add(ChartType.ScatterMarkers, new ChangeScatterChartTypeBuilder(ScatterChartStyle.Marker));
			result.Add(ChartType.ScatterSmoothMarkers, new ChangeScatterChartTypeBuilder(ScatterChartStyle.SmoothMarker));
			result.Add(ChartType.ScatterSmooth, new ChangeScatterChartTypeBuilder(ScatterChartStyle.Smooth));
			result.Add(ChartType.ScatterLine, new ChangeScatterChartTypeBuilder(ScatterChartStyle.Line));
			result.Add(ChartType.ScatterLineMarkers, new ChangeScatterChartTypeBuilder(ScatterChartStyle.LineMarker));
			result.Add(ChartType.StockHighLowClose, new ChangeStockChartTypeBuilder(false));
			result.Add(ChartType.StockOpenHighLowClose, new ChangeStockChartTypeBuilder(true));
			result.Add(ChartType.Surface, new ChangeSurfaceChartTypeBuilder(false));
			result.Add(ChartType.SurfaceWireframe, new ChangeSurfaceChartTypeBuilder(true));
			result.Add(ChartType.Surface3D, new ChangeSurface3DChartTypeBuilder(false));
			result.Add(ChartType.Surface3DWireframe, new ChangeSurface3DChartTypeBuilder(true));
			result.Add(ChartType.Bubble, new ChangeBubbleChartTypeBuilder(false));
			result.Add(ChartType.Bubble3D, new ChangeBubbleChartTypeBuilder(true));
			result.Add(ChartType.Radar, new ChangeRadarChartTypeBuilder(RadarChartStyle.Standard));
			result.Add(ChartType.RadarMarkers, new ChangeRadarChartTypeBuilder(RadarChartStyle.Marker));
			result.Add(ChartType.RadarFilled, new ChangeRadarChartTypeBuilder(RadarChartStyle.Filled));
			return result;
		}
		static bool IsCompatibleChartType(ChartType type) {
			return
				type == ChartType.Area ||
				type == ChartType.AreaFullStacked ||
				type == ChartType.AreaStacked ||
				type == ChartType.BarClustered ||
				type == ChartType.BarFullStacked ||
				type == ChartType.BarStacked ||
				type == ChartType.ColumnClustered ||
				type == ChartType.ColumnFullStacked ||
				type == ChartType.ColumnStacked ||
				type == ChartType.Line ||
				type == ChartType.LineFullStacked ||
				type == ChartType.LineFullStackedMarker ||
				type == ChartType.LineMarker ||
				type == ChartType.LineStacked ||
				type == ChartType.LineStackedMarker;
		}
		static bool IsStockChartType(ChartType type) {
			return
				type == ChartType.StockHighLowClose ||
				type == ChartType.StockOpenHighLowClose ||
				type == ChartType.StockVolumeHighLowClose ||
				type == ChartType.StockVolumeOpenHighLowClose;
		}
		static bool IsSurfaceChartType(ChartType type) {
			return
				type == ChartType.Surface ||
				type == ChartType.SurfaceWireframe ||
				type == ChartType.Surface3D ||
				type == ChartType.Surface3DWireframe;
		}
		internal static ChangeSeriesChartTypeBuilderBase TryGetBuilder(ChartType chartType) {
			if (HasBuilder(chartType))
				return builderTable[chartType];
			return null;
		}
		internal static bool HasBuilder(ChartType chartType) {
			return builderTable.ContainsKey(chartType);
		}
		internal static bool IsCompatibleChartTypes(ChartType oldType, ChartType newType) {
			return IsCompatibleChartType(oldType) && IsCompatibleChartType(newType);
		}
		internal static bool CheckChangeChartTypeError(ChartType oldType, ChartType newType, bool isOneSeries) {
			return CheckChangeCombinedChartTypeError(oldType, newType) || CheckChangeSurfaceChartTypeError(oldType, newType, isOneSeries);
		}
		internal static bool CheckChangeCombinedChartTypeError(ChartType oldType, ChartType newType) { 
			return (!IsStockChartType(oldType) && IsStockChartType(newType)) || IsStockChartType(oldType) || IsSurfaceChartType(oldType);
		}
		internal static bool CheckChangeSurfaceChartTypeError(ChartType oldType, ChartType newType, bool isOneSeries) {
			return (!IsSurfaceChartType(oldType) && IsSurfaceChartType(newType)) && isOneSeries;
		}
	}
	#endregion
	#region ChangeSeriesChartTypeCommand
	public class ChangeSeriesChartTypeCommand : ErrorHandledWorksheetCommand {
		#region Static Members
		internal static bool CheckChangeCompatibleChartType(ISeries series, ChartType newType) {
			return
				CheckExecute(series, newType) && ChangeSeriesChartTypeFactory.HasBuilder(newType) &&
				ChangeSeriesChartTypeFactory.IsCompatibleChartTypes(series.ChartType, newType);
		}
		static bool CheckExecute(ISeries series, ChartType newType) {
			return series.View.IsContained && series.IsContained && series.ChartType != newType;
		}
		#endregion
		#region Fields
		readonly ISeries targetSeries;
		readonly ChartType newChartType;
		ISeries newSeries;
		#endregion
		public ChangeSeriesChartTypeCommand(IErrorHandler errorHandler, ISeries targetSeries, ChartType newChartType)
			: base(targetSeries.View.Parent.DocumentModel, errorHandler) {
			this.targetSeries = targetSeries;
			this.newSeries = targetSeries;
			this.newChartType = newChartType;
		}
		#region Properties
		protected internal ISeries NewSeries { get { return newSeries; } set { newSeries = value; } }
		IChartView TargetView { get { return targetSeries.View; } }
		#endregion
		protected internal override void ExecuteCore() {
			ChangeSeriesChartTypeBuilderBase builder = ChangeSeriesChartTypeFactory.TryGetBuilder(newChartType);
			builder.TargetSeries = targetSeries;
			builder.Build();
			if (builder.IsCreateView)
				ChangeAllSeriesOrdersCommand.Execute(ErrorHandler, targetSeries.View.Parent);
			HistoryItem item = new ChangeChartSeriesHistoryItem(this, targetSeries, builder.NewSeries);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region Validate
		protected internal override bool Validate() {
			ChartType oldChartType = targetSeries.ChartType;
			if (!CheckExecute(targetSeries, newChartType))
				return false;
			if (ChangeSeriesChartTypeFactory.CheckChangeCombinedChartTypeError(oldChartType, newChartType))
				return HandleError(new ModelErrorInfo(ModelErrorType.SomeChartTypesCannotBeCombinedWithOtherChartTypes));
			bool isOneTargetSeries = TargetView.Series.Count == 1 && TargetView.Parent.Views.Count == 1;
			if (ChangeSeriesChartTypeFactory.CheckChangeSurfaceChartTypeError(oldChartType, newChartType, isOneTargetSeries))
				return HandleError(new ModelErrorInfo(ModelErrorType.SurfaceChartMustContainAtLeastTwoSeries));
			return ChangeSeriesChartTypeFactory.HasBuilder(newChartType);
		}
		#endregion
	}
	#endregion
	#region ChangeChartSeriesHistoryItem
	public class ChangeChartSeriesHistoryItem : HistoryItem {
		#region Fields
		readonly ChangeSeriesChartTypeCommand command;
		readonly ISeries oldValue;
		readonly ISeries newValue;
		#endregion
		public ChangeChartSeriesHistoryItem(ChangeSeriesChartTypeCommand command, ISeries oldValue, ISeries newValue)
			: base(command.DocumentModel) {
			this.command = command;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			command.NewSeries = oldValue;
		}
		protected override void RedoCore() {
			command.NewSeries = newValue;
		}
	}
	#endregion
}
