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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract partial class SeriesAbstractOptionsControl : ElementsOptionsControlBase {
		ElementsOptionsControlBase viewOptionsControl;
		SeriesViewModelBase viewModelBase;
		Dictionary<ElementsOptionsControlBase, LayoutControlItem> layoutItems;
		LayoutControlItem currentLayoutItem;
		SeriesViewOptionsControlFactory factory;
		IView SeriesViewOptionsControl { get { return viewOptionsControl as IView; } }
		protected abstract LayoutControlGroup Panel { get; }
		protected abstract LayoutControl LayoutControl { get; }
		public SeriesAbstractOptionsControl() {
			InitializeComponent();
			UpdateColors();
			this.factory = new SeriesViewOptionsControlFactory();
			this.layoutItems = new Dictionary<ElementsOptionsControlBase, LayoutControlItem>();
			AddDependence<bool>("Visible", "LabelsVisibility", visible => visible);
			AddDependence<bool>("Visible", "ShowInLegend", visible => visible);
		}
		void UpdateViewControlVisibility() {
			Panel.Visibility = ((DesignerSeriesModelBase)Model).Visible ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		protected override void UpdateActivity(string parameter) {
			base.UpdateActivity(parameter);
			if(SeriesViewOptionsControl != null)
				SeriesViewOptionsControl.UpdateActivity(parameter);
		}
		internal override void UpdateView() {
			base.UpdateView();
			UpdateViewControlVisibility();
			if (SeriesViewOptionsControl != null)
				SeriesViewOptionsControl.UpdateData();
			UpdatePaneControl();
			UpdateAxisControl();
			UpdateSwiftPlotAxisControl();
		}
		void UpdatePaneControl() {
			DesignerChartModel chart = viewModelBase.FindParent<DesignerChartModel>();
			if (chart == null)
				return;
			XYDiagram2DModel diagramModel = chart.Diagram as XYDiagram2DModel;
			if (diagramModel == null)
				return;
			PaneControl paneControl = viewOptionsControl.GetControl<PaneControl>("Pane");
			if (paneControl == null)
				return;
			paneControl.DiagramModel = diagramModel;
			paneControl.UpdateComboBox();
		}
		void UpdateAxisControl() {
			DesignerChartModel chart = viewModelBase.FindParent<DesignerChartModel>();
			if (chart == null)
				return;
			XYDiagram2DModel diagramModel = chart.Diagram as XYDiagram2DModel;
			if (diagramModel == null)
				return;
			AxisControl axisXControl = viewOptionsControl.GetControl<AxisControl>("AxisX");
			if (axisXControl == null)
				return;
			AxisControl axisYControl = viewOptionsControl.GetControl<AxisControl>("AxisY");
			if (axisXControl == null)
				return;
			axisYControl.DiagramModel = diagramModel;
			axisXControl.DiagramModel = diagramModel;
			axisYControl.UpdateComboBox();
			axisXControl.UpdateComboBox();
		}
		void UpdateSwiftPlotAxisControl() {
			DesignerChartModel chart = viewModelBase.FindParent<DesignerChartModel>();
			if (chart == null)
				return;
			SwiftPlotDiagramModel diagramModel = chart.Diagram as SwiftPlotDiagramModel;
			if (diagramModel == null)
				return;
			SwiftPlotAxisControl axisXControl = viewOptionsControl.GetControl<SwiftPlotAxisControl>("AxisX");
			if (axisXControl == null)
				return;
			SwiftPlotAxisControl axisYControl = viewOptionsControl.GetControl<SwiftPlotAxisControl>("AxisY");
			if (axisXControl == null)
				return;
			axisYControl.DiagramModel = diagramModel;
			axisXControl.DiagramModel = diagramModel;
			axisYControl.UpdateComboBox();
			axisXControl.UpdateComboBox();
		}
		internal override void LoadModel(DesignerChartElementModelBase model) {
			base.LoadModel(model);
			if (Panel == null)
				return;
			if (viewOptionsControl != null && viewModelBase != null)
				SeriesViewOptionsControl.RemoveListener(viewModelBase);
			if (currentLayoutItem != null)
				currentLayoutItem.Visibility = LayoutVisibility.Never;
			viewModelBase = GetViewModel(model as DesignerSeriesModelBase);
			viewOptionsControl = factory.GetOptionsControl(viewModelBase);
			((System.ComponentModel.ISupportInitialize)(LayoutControl)).BeginInit();
			LayoutControl.SuspendLayout();
			LayoutControlItem layoutItem;
			if (layoutItems.ContainsKey(viewOptionsControl)) {
				layoutItem = layoutItems[viewOptionsControl];
			}
			else {
				Size viewOptionsControlSize = viewOptionsControl.Size;
				this.Controls.Add(viewOptionsControl);
				LayoutControl.Controls.Add(viewOptionsControl);
				layoutItem = new LayoutControlItem();
				layoutItem.SizeConstraintsType = SizeConstraintsType.Default;
				layoutItem.MaxSize = new Size(0, 0);
				layoutItem.Padding = new XtraLayout.Utils.Padding(0);
				layoutItem.MinSize = new Size(viewOptionsControlSize.Width / 2, viewOptionsControlSize.Height);
				Panel.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutItem });
				layoutItem.Control = viewOptionsControl;
				layoutItem.TextVisible = false;
				layoutItems.Add(viewOptionsControl, layoutItem);
			}
			SeriesViewOptionsControl.AddListener(viewModelBase);
			SeriesViewOptionsControl.LoadModel(viewModelBase);
			currentLayoutItem = layoutItem;
			currentLayoutItem.Visibility = LayoutVisibility.Always;
			((System.ComponentModel.ISupportInitialize)(LayoutControl)).EndInit();
			LayoutControl.ResumeLayout(false);
			UpdatePaneControl();
			UpdateAxisControl();
			UpdateSwiftPlotAxisControl();
		}
		SeriesViewModelBase GetViewModel(DesignerSeriesModelBase designerSeriesModel) {
			if (designerSeriesModel == null)
				return null;
			return designerSeriesModel.View;
		}
	}
	public class SeriesViewOptionsControlFactory {
		readonly Dictionary<Type, ElementsOptionsControlBase> optionsControls = new Dictionary<Type, ElementsOptionsControlBase>();
		public SeriesViewOptionsControlFactory() {
			optionsControls = new Dictionary<Type, ElementsOptionsControlBase>();
		}
		public ElementsOptionsControlBase GetOptionsControl(SeriesViewModelBase viewModelBase) {
			Type type = viewModelBase.GetType();
			if (optionsControls.ContainsKey(type))
				return optionsControls[type];
			ElementsOptionsControlBase optionsControl = CreateOptionsControl(viewModelBase);
			if (optionsControl != null)
				optionsControls.Add(type, optionsControl);
			return optionsControl;
		}
		internal static ElementsOptionsControlBase CreateOptionsControl(SeriesViewModelBase viewModelBase) {
			if (viewModelBase == null)
				return null;
			Type viewType = viewModelBase.GetType();
			if (typeof(SideBySideStackedBarViewModel).IsAssignableFrom(viewType))
				return new SideBySideStackedBarSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SideBySideFullStackedBarViewModel).IsAssignableFrom(viewType))
				return new SideBySideStackedBarSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(GanttViewModel).IsAssignableFrom(viewType))
				return new GanntSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RangeBarViewModel).IsAssignableFrom(viewType))
				return new RangeBarSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(BarViewModel).IsAssignableFrom(viewType))
				return new BarSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SideBySideStackedBar3DViewModel).IsAssignableFrom(viewType))
				return new SideBySideStackedBar3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SideBySideFullStackedBar3DViewModel).IsAssignableFrom(viewType))
				return new SideBySideStackedBar3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Bar3DViewModel).IsAssignableFrom(viewType))
				return new Bar3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(PointViewModel).IsAssignableFrom(viewType))
				return new PointSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(BubbleViewModel).IsAssignableFrom(viewType))
				return new BubbleSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StepLineViewModel).IsAssignableFrom(viewType))
				return new StepLineSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SplineViewModel).IsAssignableFrom(viewType))
				return new SplineSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StackedSplineAreaViewModel).IsAssignableFrom(viewType))
				return new StackedSplineAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(FullStackedSplineAreaViewModel).IsAssignableFrom(viewType))
				return new StackedSplineAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StackedAreaViewModel).IsAssignableFrom(viewType))
				return new StackedAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(FullStackedAreaViewModel).IsAssignableFrom(viewType))
				return new StackedAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StepAreaViewModel).IsAssignableFrom(viewType))
				return new StepAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SplineAreaViewModel).IsAssignableFrom(viewType))
				return new SplineAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RangeAreaViewModel).IsAssignableFrom(viewType))
				return new RangeAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(AreaViewBaseModel).IsAssignableFrom(viewType))
				return new AreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(LineViewModel).IsAssignableFrom(viewType))
				return new LineSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SwiftPlotViewModel).IsAssignableFrom(viewType))
				return new SwiftPlotSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StepLine3DViewModel).IsAssignableFrom(viewType))
				return new StepLine3dSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Spline3DViewModel).IsAssignableFrom(viewType))
				return new Spline3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StepArea3DViewModel).IsAssignableFrom(viewType))
				return new StepArea3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(SplineArea3DViewModel).IsAssignableFrom(viewType))
				return new SplineArea3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StackedSplineArea3DViewModel).IsAssignableFrom(viewType))
				return new SplineArea3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(FullStackedSplineArea3DViewModel).IsAssignableFrom(viewType))
				return new SplineArea3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RangeArea3DViewModel).IsAssignableFrom(viewType))
				return new Area3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Area3DViewModel).IsAssignableFrom(viewType))
				return new Area3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Line3DViewModel).IsAssignableFrom(viewType))
				return new Line3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(DoughnutViewModel).IsAssignableFrom(viewType))
				return new DoughnutSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(NestedDoughnutViewModel).IsAssignableFrom(viewType))
				return new NestedDoughnutSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(PieViewModel).IsAssignableFrom(viewType))
				return new PieSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Doughnut3DViewModel).IsAssignableFrom(viewType))
				return new Doughnut3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Pie3DViewModel).IsAssignableFrom(viewType))
				return new Pie3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(FunnelViewModel).IsAssignableFrom(viewType))
				return new FunnelSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(Funnel3DViewModel).IsAssignableFrom(viewType))
				return new Funnel3DSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RadarAreaViewModel).IsAssignableFrom(viewType))
				return new RadarAreaSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RadarLineViewModel).IsAssignableFrom(viewType))
				return new RadarLineSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(RadarPointViewModel).IsAssignableFrom(viewType))
				return new RadarPointSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(StockViewModel).IsAssignableFrom(viewType))
				return new StokeSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			if (typeof(CandleStickViewModel).IsAssignableFrom(viewType))
				return new CandleStickSeriesViewOptionsControl() { Dock = DockStyle.Fill };
			return null;
		}
	}
	public class SeriesProxyOptionsControl : SeriesAbstractOptionsControl {
		protected override LayoutControl LayoutControl {
			get { throw new NotImplementedException(); }
		}
		protected override LayoutControlGroup Panel {
			get { throw new NotImplementedException(); }
		}
	}
}
