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

using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraCharts.Commands {
	public abstract class CreateChartCommandBase : ChartCommand {
		public const int RandomPointsCounts = 5;
		public CreateChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected abstract ViewType ChartViewType { get; }
		protected virtual bool DiagramRotated { get { return false; } }
		void CreateFakeSeries() {
			foreach (Series series in Chart.Series) {
				ChangeSeriesType(series, ChartViewType);
			}
		}
		protected virtual void ChangeSeriesType(SeriesBase series, ViewType viewType) {
			series.ChangeView(viewType);
		}
		protected virtual void CreateFakeChart() {
			Series series1 = new Series();
			Series series2 = new Series();
			Chart.Series.Add(series1);
			Chart.Series.Add(series2);
		}
		protected override void ExecuteCore(ICommandUIState state) {
			if (Chart.Series == null || Chart.Series.Count == 0) {
				CreateFakeChart();
				CreateFakeSeries();
			}
			else
				foreach (Series series in Chart.Series)
					if (!series.IsAutoCreated)
						ChangeSeriesType(series, ChartViewType);
			if (DataContainer.SeriesTemplate != null)
				ChangeSeriesType(DataContainer.SeriesTemplate, ChartViewType);
			XYDiagram diagram = Chart.Diagram as XYDiagram;
			if (diagram != null)
				diagram.Rotated = DiagramRotated;
		}
	}
	public abstract class CreateBar3DChartCommandBase : CreateChartCommandBase {
		public CreateBar3DChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected virtual Bar3DModel Model { get { return Bar3DModel.Box; } }
		protected override void ChangeSeriesType(SeriesBase series, ViewType viewType) {
			base.ChangeSeriesType(series, viewType);
			Bar3DSeriesView bar3DView = series.View as Bar3DSeriesView;
			if (bar3DView != null)
				bar3DView.Model = Model;
		}
	}
	public abstract class CreateConeBar3DChartCommandBase : CreateBar3DChartCommandBase {
		public CreateConeBar3DChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override Bar3DModel Model { get { return Bar3DModel.Cone; } }
	}
	public abstract class CreatePyramidBar3DChartCommandBase : CreateBar3DChartCommandBase {
		public CreatePyramidBar3DChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override Bar3DModel Model { get { return Bar3DModel.Pyramid; } }
	}
	public abstract class CreateCylinderBar3DChartCommandBase : CreateBar3DChartCommandBase {
		public CreateCylinderBar3DChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override Bar3DModel Model { get { return Bar3DModel.Cylinder; } }
	}
	public abstract class CreateRotatedBarChartCommandBase : CreateChartCommandBase {
		public CreateRotatedBarChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override bool DiagramRotated { get { return true; } }
	}
	public abstract class CreateSimpleDiagramChartCommandBase : CreateChartCommandBase {
		public CreateSimpleDiagramChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override void CreateFakeChart() {
			Series series = new Series("", ChartViewType);
			Chart.Series.Add(series);
		}
	}
	public abstract class CreateFinancialChartCommandBase : CreateChartCommandBase {
		public CreateFinancialChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override void CreateFakeChart() {
			Series series = new Series("", ChartViewType);
			Chart.Series.Add(series);
		}
	}
	public abstract class CreateLineChartCommandBase : CreateChartCommandBase {
		public CreateLineChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override void CreateFakeChart() {
			Series series1 = new Series("", ChartViewType);
			Series series2 = new Series("", ChartViewType);
			Series series3 = new Series("", ChartViewType);
			Chart.Series.Add(series1);
			Chart.Series.Add(series2);
			Chart.Series.Add(series3);
		}
	}
	public abstract class CreatePolarChartCommandBase : CreateChartCommandBase {
		public CreatePolarChartCommandBase(IChartContainer control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			foreach (Series series in Chart.Series)
				if (series.ActualArgumentScaleType == ScaleType.Qualitative && series.Points.Count != 0)
					state.Enabled = false;
		}
	}
}
