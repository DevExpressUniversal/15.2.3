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

using DevExpress.Charts.Native;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class XYDiagramSeriesViewBaseControl : SeriesViewControlBase {
		XYDiagram2DSeriesViewBaseController controller;
		public XYDiagramSeriesViewBaseControl() {
			InitializeComponent();
		}
		protected override void InitializeTabs() {
			base.InitializeTabs();
			controller = XYDiagram2DSeriesViewBaseController.CreateInstance(View as XYDiagram2DSeriesViewBase, Series);
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			if (controller != null) {
				generalControl.Initialize(controller, Chart);
				if (controller.AppearanceSupported)
					appearanceControl.Initialize(controller);
				else
					tbcPagesControl.TabPages.Remove(tbAppearance);
				ChartUserControl borderControl = controller.GetBorderControl();
				if (borderControl == null)
					tbcPagesControl.TabPages.Remove(tbBorder);
				else {
					borderControl.Dock = DockStyle.Fill;
					tbBorder.Controls.Add(borderControl);
				}
				if (controller.Shadow == null)
					tbcPagesControl.TabPages.Remove(tbShadow);
				else
					shadowControl.Initialize(controller.Shadow);
				ChartUserControl markerControl = controller.GetMarkerControl();
				if (markerControl == null) 
					tbcPagesControl.TabPages.Remove(tbMarker);
				else {
					markerControl.Dock = DockStyle.Fill;
					tbMarker.Controls.Add(markerControl);
				}
				if (controller.MinMarker == null)
					tbcPagesControl.TabPages.Remove(tbMinMarker);
				else
					minMarkerControl.Initialize(controller.MinMarker, controller.View);
				if (controller.MaxMarker == null)
					tbcPagesControl.TabPages.Remove(tbMaxMarker);
				else
					maxMarkerControl.Initialize(controller.MaxMarker, controller.View);
				if (controller.Marker1 == null)
					tbcPagesControl.TabPages.Remove(tbMarker1);
				else
					marker1Control.Initialize(controller.Marker1, controller.View);
				if (controller.Marker2 == null)
					tbcPagesControl.TabPages.Remove(tbMarker2);
				else
					marker2Control.Initialize(controller.Marker2, controller.View);
				indicatorsControl.Initialize(controller.View.Indicators, controller.View, Chart);
			}
		}
		protected override void InitializeTags() {
			base.InitializeTags();
			if (controller != null) {
				tbGeneral.Tag = GetPageTabByName("General");
				if (controller.AppearanceSupported)
					tbAppearance.Tag = GetPageTabByName("Appearance");
				ChartUserControl borderControl = controller.GetBorderControl();
				if (borderControl != null)
					tbBorder.Tag = GetPageTabByName("Border");
				if (controller.Shadow != null)
					tbShadow.Tag = GetPageTabByName("Shadow");
				ChartUserControl markerControl = controller.GetMarkerControl();
				if (markerControl != null)
					tbMarker.Tag = GetPageTabByName("Marker");
				if (controller.MinMarker != null)
					tbMinMarker.Tag = GetPageTabByName("MinMarker");
				if (controller.MaxMarker != null)
					tbMaxMarker.Tag = GetPageTabByName("MaxMarker");
				if (controller.Marker1 != null)
					tbMarker1.Tag = GetPageTabByName("Marker1");
				if (controller.Marker2 != null)
					tbMarker2.Tag = GetPageTabByName("Marker2");
				tbIndicators.Tag = GetPageTabByName("Indicators");
			}
		}
		public override void SelectHitTestElement(ChartElement element) {
			Indicator indicator = element as Indicator;
			if (indicator != null) {
				indicatorsControl.SelectIndicator(indicator);
				SelectedTabHandle = tbIndicators.Tag;
			}
		}
	}
	internal static class SeriesViewControllerHelper {
		public static ChartUserControl GetSplineGeneralOptionsControl(ISplineSeriesView view) {
			SplineGeneralOptionsControl control = new SplineGeneralOptionsControl();
			control.Initialize(view);
			return control;
		}
	}
	internal abstract class XYDiagram2DSeriesViewBaseController : SeriesViewBaseController {
		public static XYDiagram2DSeriesViewBaseController CreateInstance(XYDiagram2DSeriesViewBase view, SeriesBase series) {
			RangeBarSeriesView rangeBarView = view as RangeBarSeriesView;
			if (rangeBarView != null)
				return new RangeBarSeriesViewController(rangeBarView, series);
			SideBySideStackedBarSeriesView sideBySideStackedBarView = view as SideBySideStackedBarSeriesView;
			if (sideBySideStackedBarView != null)
				return new SideBySideStackedBarSeriesViewController(sideBySideStackedBarView, series);
			SideBySideFullStackedBarSeriesView sideBySideFullStackedBarView = view as SideBySideFullStackedBarSeriesView;
			if  (sideBySideFullStackedBarView != null)
				return new SideBySideStackedBarSeriesViewController(sideBySideFullStackedBarView, series);
			BarSeriesView barView = view as BarSeriesView;
			if (barView != null)
				return new BarSeriesViewController(barView, series);
			FullStackedSplineAreaSeriesView fullStackeSplineAreaView = view as FullStackedSplineAreaSeriesView;
			if( fullStackeSplineAreaView != null)
				return new FullStackedSplineAreaSeriesViewController(fullStackeSplineAreaView, series);
			StackedSplineAreaSeriesView stackedSplineAreaView = view as StackedSplineAreaSeriesView;
			if (stackedSplineAreaView != null)
				return new StackedSplineAreaSeriesViewController(stackedSplineAreaView, series);
			SplineAreaSeriesView splineAreaView = view as SplineAreaSeriesView;
			if (splineAreaView != null)
				return new SplineAreaSeriesViewController(splineAreaView, series);
			FullStackedAreaSeriesView fullStackedAreaView = view as FullStackedAreaSeriesView;
			if (fullStackedAreaView != null)
				return new FullStackedAreaSeriesViewController(fullStackedAreaView, series);
			StackedAreaSeriesView stackedAreaView = view as StackedAreaSeriesView;
			if (stackedAreaView != null)
				return new StackedAreaSeriesViewController(stackedAreaView, series);
			StepAreaSeriesView stepAreaView = view as StepAreaSeriesView;
			if (stepAreaView != null)
				return new StepAreaSeriesViewController(stepAreaView, series);
			RangeAreaSeriesView rangeAreaView = view as RangeAreaSeriesView;
			if (rangeAreaView != null)
				return new RangeAreaSeriesViewController(rangeAreaView, series);
			AreaSeriesView areaView = view as AreaSeriesView;
			if (areaView != null)
				return new AreaSeriesViewController(areaView, series);
			SplineSeriesView splineView = view as SplineSeriesView;
			if (splineView != null)
				return new SplineSeriesViewController(splineView, series);
			StepLineSeriesView stepLineView = view as StepLineSeriesView;
			if (stepLineView != null)
				return new StepLineSeriesViewController(stepLineView, series);
			LineSeriesView lineView = view as LineSeriesView;
			if (lineView != null)
				return new LineSeriesViewController(lineView, series);
			PointSeriesView pointView = view as PointSeriesView;
			if (pointView != null)
				return new PointSeriesViewController(pointView, series);
			BubbleSeriesView bubbleView = view as BubbleSeriesView;
			if (bubbleView != null)
				return new BubbleSeriesViewController(bubbleView, series);
			SwiftPlotSeriesView swiftPlotView = view as SwiftPlotSeriesView;
			if (swiftPlotView != null)
				return new SwiftPlotSeriesViewController(swiftPlotView, series);
			FinancialSeriesViewBase financialView = view as FinancialSeriesViewBase;
			if (financialView != null)
				return new FinancialSeriesViewController(financialView, series);
			SeriesViewColorEachSupportBase colorEachView = view as SeriesViewColorEachSupportBase;
			if (colorEachView != null)
				return new SeriesViewColorEachSupportBaseController(colorEachView, series);
			return null;
		}
		public new XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)base.View; } }
		public virtual Shadow Shadow { get { return null; } }
		public virtual Marker MinMarker { get { return null; } }
		public virtual Marker MaxMarker { get { return null; } }
		public virtual Marker Marker1 { get { return null; } }
		public virtual Marker Marker2 { get { return null; } }
		public virtual bool AppearanceSupported { get { return true; } }
		public XYDiagram2DSeriesViewBaseController(XYDiagram2DSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public virtual ChartUserControl GetGeneralOptionsControl() {
			return null;
		}
		public virtual ChartUserControl GetMarkerControl() {
			return null;
		}
		public virtual ChartUserControl GetBorderControl() {
			return null;
		}
		public abstract void SetAxisX(Axis2D axisX);
		public abstract void SetAxisY(Axis2D axisY);
		public abstract void SetPane(XYDiagramPaneBase pane);
	}
	internal class SwiftPlotSeriesViewController : XYDiagram2DSeriesViewBaseController {
		SwiftPlotSeriesView SwiftView { get { return (SwiftPlotSeriesView)base.View; } }
		public override bool ColorEachSupported { get { return false; } }
		public override LineStyle LineStyle { get { return SwiftView.LineStyle; } }
		public SwiftPlotSeriesViewController(SwiftPlotSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SwiftPlotGeneralOptionsControl control = new SwiftPlotGeneralOptionsControl();
			control.Initialize(SwiftView);
			return control;
		}
		public override void SetAxisX(Axis2D axisX) {
			SwiftView.AxisX = (SwiftPlotDiagramAxisXBase)axisX;
		}
		public override void SetAxisY(Axis2D axisY) {
			SwiftView.AxisY = (SwiftPlotDiagramAxisYBase)axisY;
		}
		public override void SetPane(XYDiagramPaneBase pane) {
			SwiftView.Pane = pane;
		}
	}
	internal abstract class XYDiagramSeriesViewBaseController : XYDiagram2DSeriesViewBaseController {
		XYDiagramSeriesViewBase XYView { get { return (XYDiagramSeriesViewBase)base.View; } }
		public XYDiagramSeriesViewBaseController(XYDiagramSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public override void SetAxisX(Axis2D axisX) {
			XYView.AxisX = (AxisXBase)axisX;
		}
		public override void SetAxisY(Axis2D axisY) {
			XYView.AxisY = (AxisYBase)axisY;
		}
		public override void SetPane(XYDiagramPaneBase pane) {
			XYView.Pane = pane;
		}
	}
	internal class SeriesViewColorEachSupportBaseController : XYDiagramSeriesViewBaseController {
		SeriesViewColorEachSupportBase ColorEachView { get { return (SeriesViewColorEachSupportBase)base.View; } }
		public override bool ColorEachSupported { get { return true; } }
		public override bool ColorEach { get { return ColorEachView.ColorEach; } set { ColorEachView.ColorEach = value; } }
		public SeriesViewColorEachSupportBaseController(SeriesViewColorEachSupportBase view, SeriesBase series) : base(view, series) { }
	}
	internal class BarSeriesViewController : SeriesViewColorEachSupportBaseController {
		protected BarSeriesView BarView { get { return (BarSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return BarView.FillStyle; } }
		public override Shadow Shadow { get { return BarView.Shadow; } }
		public BarSeriesViewController(BarSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			BarGeneralOptionsControl control = new BarGeneralOptionsControl();
			control.Initialize(BarView);
			return control;
		}
		public override ChartUserControl GetBorderControl() {
			BorderControl control = new BorderControl();
			control.Initialize(BarView.Border);
			return control;
		}
	}
	internal class SideBySideStackedBarSeriesViewController : BarSeriesViewController {
		public SideBySideStackedBarSeriesViewController(BarSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SideBySideStackedBarGeneralOptionsControl control = new SideBySideStackedBarGeneralOptionsControl();
			control.Initialize(BarView, Series);
			return control;
		}
	}
	internal class RangeBarSeriesViewController : BarSeriesViewController {
		RangeBarSeriesView RangeBarView { get { return (RangeBarSeriesView)base.View; } }
		public override Marker MinMarker { get { return RangeBarView.MinValueMarker; } }
		public override Marker MaxMarker { get { return RangeBarView.MaxValueMarker; } }
		public RangeBarSeriesViewController(RangeBarSeriesView view, SeriesBase series) : base(view, series) { }
	}
	internal class PointSeriesViewBaseController : SeriesViewColorEachSupportBaseController {
		public override Shadow Shadow { get { return ((PointSeriesViewBase)base.View).Shadow; } }
		public PointSeriesViewBaseController(PointSeriesViewBase view, SeriesBase series) : base(view, series) { }
	}
	internal class PointSeriesViewController : PointSeriesViewBaseController {
		public PointSeriesViewController(PointSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			MarkerBaseControl control = new MarkerBaseControl();
			control.Initialize(((PointSeriesView)base.View).PointMarkerOptions);
			return control;
		}
	}
	internal class BubbleSeriesViewController : PointSeriesViewBaseController {
		BubbleSeriesView BubbleView { get { return (BubbleSeriesView)base.View; } }
		public BubbleSeriesViewController(BubbleSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			BubbleGeneralOptionsControl control = new BubbleGeneralOptionsControl();
			control.Initialize(BubbleView);
			return control;
		}
		public override ChartUserControl GetMarkerControl() {
			MarkerBaseControl control = new MarkerBaseControl();
			control.Initialize(BubbleView.BubbleMarkerOptions);
			return control;
		}
	}
	internal class LineSeriesViewController : PointSeriesViewBaseController {
		LineSeriesView LineView { get { return (LineSeriesView)base.View; } }
		public override LineStyle LineStyle { get { return LineView.LineStyle; } }
		public LineSeriesViewController(LineSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			MarkerControl control = new MarkerControl();
			control.Initialize(LineView.LineMarkerOptions, LineView);
			return control;
		}
	}
	internal class StepLineSeriesViewController : LineSeriesViewController {
		public StepLineSeriesViewController(StepLineSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			StepLineGeneralOptionsControl control = new StepLineGeneralOptionsControl();
			control.Initialize((StepLineSeriesView)base.View);
			return control;
		}
	}
	internal class SplineSeriesViewController : LineSeriesViewController {
		public SplineSeriesViewController(SplineSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			return SeriesViewControllerHelper.GetSplineGeneralOptionsControl((SplineSeriesView)base.View);
		}
	}
	internal class AreaSeriesViewController : LineSeriesViewController {
		AreaSeriesView AreaView { get { return (AreaSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return AreaView.FillStyle; } }
		public override LineStyle LineStyle { get { return null; } }
		public AreaSeriesViewController(AreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetBorderControl() {
			BorderControl control = new BorderControl();
			control.Initialize(AreaView.Border);
			return control;
		}
	}
	internal class StackedAreaSeriesViewController : AreaSeriesViewController {
		public override bool ColorEachSupported { get { return false; } }
		public StackedAreaSeriesViewController(StackedAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			return null;
		}
	}
	internal class FullStackedAreaSeriesViewController : StackedAreaSeriesViewController {
		public FullStackedAreaSeriesViewController(FullStackedAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetBorderControl() {
			return null;
		}
	}
	internal class StepAreaSeriesViewController : AreaSeriesViewController {
		public StepAreaSeriesViewController(StepAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			StepLineGeneralOptionsControl control = new StepLineGeneralOptionsControl();
			control.Initialize((StepAreaSeriesView)base.View);
			return control;
		}
	}
	internal class SplineAreaSeriesViewController : AreaSeriesViewController {
		public SplineAreaSeriesViewController(SplineAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			return SeriesViewControllerHelper.GetSplineGeneralOptionsControl((SplineAreaSeriesView)base.View);
		}
	}
	internal class StackedSplineAreaSeriesViewController : StackedAreaSeriesViewController {
		public StackedSplineAreaSeriesViewController(StackedSplineAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			return SeriesViewControllerHelper.GetSplineGeneralOptionsControl((StackedSplineAreaSeriesView)base.View);
		}
	}
	internal class FullStackedSplineAreaSeriesViewController : FullStackedAreaSeriesViewController {
		public FullStackedSplineAreaSeriesViewController(FullStackedSplineAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			return SeriesViewControllerHelper.GetSplineGeneralOptionsControl((FullStackedSplineAreaSeriesView)base.View);
		}
	}
	internal class RangeAreaSeriesViewController : AreaSeriesViewController {
		RangeAreaSeriesView RangeAreaView { get { return (RangeAreaSeriesView)base.View; } }
		public override Marker Marker1 { get { return RangeAreaView.Marker1; } }
		public override Marker Marker2 { get { return RangeAreaView.Marker2; } }
		public RangeAreaSeriesViewController(RangeAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			return null;
		}
		public override ChartUserControl GetBorderControl() {
			RangeBorderControl control = new RangeBorderControl();
			control.Initialize(RangeAreaView.Border1, RangeAreaView.Border2);
			return control;
		}
	}
	internal class FinancialSeriesViewController : XYDiagramSeriesViewBaseController {
		FinancialSeriesViewBase FinancialView { get { return (FinancialSeriesViewBase)base.View; } }
		public override bool ColorEachSupported { get { return false; } }
		public override bool AppearanceSupported { get { return false; } }
		public override Shadow Shadow { get { return FinancialView.Shadow; } }
		public FinancialSeriesViewController(FinancialSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			FinancialGeneralOptionsControl control = new FinancialGeneralOptionsControl();
			control.Initialize(FinancialView);
			return control;
		}
	}
}
