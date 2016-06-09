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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class XYDiagram3DSeriesViewBaseControl : SeriesViewControlBase {
		XYDiagram3DSeriesViewBaseController controller;
		public XYDiagram3DSeriesViewBaseControl() {
			InitializeComponent();
		}
		protected override void InitializeTabs() {
			base.InitializeTabs();
			controller = XYDiagram3DSeriesViewBaseController.CreateInstance(View as XYDiagram3DSeriesViewBase, Series);
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			if(controller == null)
				return;
			ChartUserControl generalControl = controller.GetGeneralOptionsControl();
			if(generalControl != null) {
				generalControl.Dock = DockStyle.Fill;
				tbGeneral.Controls.Add(generalControl);
			}
			else
				tbcPagesControl.TabPages.Remove(tbGeneral);
			appearanceControl.Initialize(controller);
		}
		protected override void InitializeTags() {
			base.InitializeTags();
			if(controller == null)
				return;
			ChartUserControl generalControl = controller.GetGeneralOptionsControl();
			if(generalControl != null)
				tbGeneral.Tag = GetPageTabByName("General");
			tbAppearance.Tag = GetPageTabByName("Appearance");
		}
	}
	internal abstract class XYDiagram3DSeriesViewBaseController : SeriesViewBaseController {
		public static XYDiagram3DSeriesViewBaseController CreateInstance(XYDiagram3DSeriesViewBase view, SeriesBase series) {
			FullStackedSplineArea3DSeriesView fullStackedSplineAreaView = view as FullStackedSplineArea3DSeriesView;
			if(fullStackedSplineAreaView != null)
				return new FullStackedSplineArea3DSeriesViewController(fullStackedSplineAreaView, series);
			StackedSplineArea3DSeriesView stackedSplineAreaView = view as StackedSplineArea3DSeriesView;
			if(stackedSplineAreaView != null)
				return new StackedSplineArea3DSeriesViewController(stackedSplineAreaView, series);
			SplineArea3DSeriesView splineAreaView = view as SplineArea3DSeriesView;
			if(splineAreaView != null)
				return new SplineArea3DSeriesViewController(splineAreaView, series);
			StepArea3DSeriesView stepAreaView = view as StepArea3DSeriesView;
			if (stepAreaView != null)
				return new StepArea3DSeriesViewController(stepAreaView, series);
			Area3DSeriesView areaView = view as Area3DSeriesView;
			if(areaView != null)
				return new Area3DSeriesViewController(areaView, series);
			Spline3DSeriesView splineView = view as Spline3DSeriesView;
			if(splineView != null)
				return new Spline3DSeriesViewController(splineView, series);
			StepLine3DSeriesView stepLineView = view as StepLine3DSeriesView;
			if(stepLineView != null)
				return new StepLine3DSeriesViewController(stepLineView, series);
			Line3DSeriesView lineView = view as Line3DSeriesView;
			if(lineView != null)
				return new Line3DSeriesViewController(lineView, series);
			SideBySideStackedBar3DSeriesView sideBySideStackedBarView = view as SideBySideStackedBar3DSeriesView;
			if (sideBySideStackedBarView != null)
				return new SideBySideStackedBar3DSeriesViewController(sideBySideStackedBarView, series);
			SideBySideFullStackedBar3DSeriesView sideBySideFullStackedBarView = view as SideBySideFullStackedBar3DSeriesView;
			if (sideBySideFullStackedBarView != null)
				return new SideBySideStackedBar3DSeriesViewController(sideBySideFullStackedBarView, series);
			Bar3DSeriesView barView = view as Bar3DSeriesView;
			if(barView != null)
				return new Bar3DSeriesViewController(barView, series);
			return null;
		}
		public virtual Line3DSeriesView LineMarkerOptions { get { return null; } }
		public XYDiagram3DSeriesViewBaseController(XYDiagram3DSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public virtual ChartUserControl GetGeneralOptionsControl() {
			return null;
		}
	}
	internal class SeriesView3DColorEachSupportBaseController : XYDiagram3DSeriesViewBaseController {
		SeriesView3DColorEachSupportBase ColorEachView { get { return (SeriesView3DColorEachSupportBase)base.View; } }
		public override bool ColorEachSupported { get { return true; } }
		public override bool ColorEach { get { return ColorEachView.ColorEach; } set { ColorEachView.ColorEach = value; } }
		public SeriesView3DColorEachSupportBaseController(SeriesView3DColorEachSupportBase view, SeriesBase series) : base(view, series) { }
	}
	internal class Bar3DSeriesViewController : SeriesView3DColorEachSupportBaseController {
		protected Bar3DSeriesView BarView { get { return (Bar3DSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return BarView.FillStyle; } }
		public Bar3DSeriesViewController(Bar3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Bar3DGeneralOptionsControl control = new Bar3DGeneralOptionsControl();
			control.Initialize(BarView);
			return control;
		}
	}
	internal class SideBySideStackedBar3DSeriesViewController : Bar3DSeriesViewController {
		public SideBySideStackedBar3DSeriesViewController(Bar3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SideBySideStackedBar3DGeneralOptionsControl control = new SideBySideStackedBar3DGeneralOptionsControl();
			control.Initialize(BarView, Series);
			return control;
		}
	}
	internal class Line3DSeriesViewController : XYDiagram3DSeriesViewBaseController {
		Line3DSeriesView LineView { get { return (Line3DSeriesView)base.View; } }
		public override bool ColorEachSupported { get { return false; } }
		public override Line3DSeriesView LineMarkerOptions { get { return LineView; } }
		public Line3DSeriesViewController(Line3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Line3DGeneralOptionsControl control = new Line3DGeneralOptionsControl();
			control.Initialize(LineView);
			return control;
		}
	}
	internal class StepLine3DSeriesViewController : Line3DSeriesViewController {
		public StepLine3DSeriesViewController(StepLine3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			StepLine3DGeneralOptionsControl control = new StepLine3DGeneralOptionsControl();
			control.Initialize((StepLine3DSeriesView)base.View);
			return control;
		}
	}
	internal class Spline3DSeriesViewController : Line3DSeriesViewController {
		public Spline3DSeriesViewController(Spline3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Spline3DGeneralOptionsControl control = new Spline3DGeneralOptionsControl();
			control.Initialize((Spline3DSeriesView)base.View);
			return control;
		}
	}
	internal class Area3DSeriesViewController : Line3DSeriesViewController {
		public Area3DSeriesViewController(Area3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Area3DGeneralOptionsControl control = new Area3DGeneralOptionsControl();
			control.Initialize((Area3DSeriesView)base.View);
			return control;
		}
	}
	internal class StepArea3DSeriesViewController : Area3DSeriesViewController {
		public StepArea3DSeriesViewController(StepArea3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			StepArea3DGeneralOptionsControl control = new StepArea3DGeneralOptionsControl();
			control.Initialize((StepArea3DSeriesView)base.View);
			return control;
		}
	}
	internal class SplineArea3DSeriesViewController : Area3DSeriesViewController {
		public SplineArea3DSeriesViewController(SplineArea3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SplineArea3DGeneralOptionsControl control = new SplineArea3DGeneralOptionsControl();
			control.Initialize((SplineArea3DSeriesView)base.View);
			return control;
		}
	}
	internal class StackedSplineArea3DSeriesViewController : Area3DSeriesViewController {
		public StackedSplineArea3DSeriesViewController(StackedSplineArea3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SplineArea3DGeneralOptionsControl control = new SplineArea3DGeneralOptionsControl();
			control.Initialize((StackedSplineArea3DSeriesView)base.View);
			return control;
		}
	}
	internal class FullStackedSplineArea3DSeriesViewController : Area3DSeriesViewController {
		public FullStackedSplineArea3DSeriesViewController(FullStackedSplineArea3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			SplineArea3DGeneralOptionsControl control = new SplineArea3DGeneralOptionsControl();
			control.Initialize((FullStackedSplineArea3DSeriesView)base.View);
			return control;
		}
	}
}
