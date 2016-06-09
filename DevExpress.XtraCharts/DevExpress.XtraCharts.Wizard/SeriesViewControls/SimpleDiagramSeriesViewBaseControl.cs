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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class SimpleDiagramSeriesViewBaseControl : SeriesViewControlBase, ISupportNestedDoughnutPositionUpdate {
		const int explodedPointsIndex = 3;
		const int titlesIndex = 4;
		SimpleDiagramSeriesViewBaseController controller;
		public SimpleDiagramSeriesViewBaseControl() {
			InitializeComponent();
		}
		#region ISupportNestedDoughnutPositionUpdate implementation
		IEnumerable<ISupportNestedDoughnutPositionUpdate> ISupportNestedDoughnutPositionUpdate.Children { get { return null; } }
		void ISupportNestedDoughnutPositionUpdate.PositionUpdated(bool isOutside) {
			UpdateTabsVisibility(isOutside);
		}
		#endregion
		SeriesViewPageTab GetTitlesPageTabName() {
			return GetPageTabByName("Titles");
		}
		void UpdateTabsVisibility(bool showTabs) {
			int tabsCount = tbcPagesControl.TabPages.Count;
			if (tabsCount > 0) {
				UpdateTabVisibility(tbExplodedPoints, showTabs, explodedPointsIndex);
				UpdateTabVisibility(tbTitles, showTabs, titlesIndex);
			}
		}
		void UpdateTabVisibility(XtraTab.XtraTabPage tab, bool showTab, int tabIndex) {
			bool tabVisible = tab.TabControl != null;
			if (!showTab && tabVisible)
				tbcPagesControl.TabPages.Remove(tab);
			else if (showTab && !tabVisible)
				tbcPagesControl.TabPages.Insert(tabIndex, tab);
		}
		protected override void InitializeTabs() {
			base.InitializeTabs();
			controller = SimpleDiagramSeriesViewBaseController.CreateInstance(View as SimpleDiagramSeriesViewBase, Series);
			controller.RegisterUpdatableElement(this);
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
			if(controller.AppearanceSupported)
				appearanceControl.Initialize(controller);
			else
				tbcPagesControl.TabPages.Remove(tbAppearance);
			if(controller.Border != null)
				borderControl.Initialize(controller.Border);
			else
				tbcPagesControl.TabPages.Remove(tbBorder);
			if(controller.ExplodedOptions != null)
				explodedPointsControl.Initialize(controller.ExplodedOptions, Series);
			else
				tbcPagesControl.TabPages.Remove(tbExplodedPoints);
			titlesControl.Initialize(controller.View, Chart);
			titlesControl.Enabled = !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(Chart.DataContainer.PivotGridDataSourceOptions, Series);
		}
		protected override void InitializeTags() {
			base.InitializeTags();
			if(controller == null)
				return;
			ChartUserControl generalControl = controller.GetGeneralOptionsControl();
			if(generalControl != null)
				tbGeneral.Tag = GetPageTabByName("General");
			if(controller.AppearanceSupported)
				tbAppearance.Tag = GetPageTabByName("Appearance");
			if(controller.Border != null)
				tbBorder.Tag = GetPageTabByName("Border");
			if(controller.ExplodedOptions != null)
				tbExplodedPoints.Tag = GetPageTabByName("ExplodedPoints");
			tbTitles.Tag = GetTitlesPageTabName();
		}
		public override void SelectHitTestElement(ChartElement element) {
			if(element is SeriesTitle) {
				titlesControl.SelectTitle(element as SeriesTitle);
				SelectedTabHandle = GetTitlesPageTabName();
			}
		}
	}
	internal class SimpleDiagramSeriesViewBaseController : SeriesViewBaseController {
		public static SimpleDiagramSeriesViewBaseController CreateInstance(SimpleDiagramSeriesViewBase view, SeriesBase series) {
			Funnel3DSeriesView funnel3DView = view as Funnel3DSeriesView;
			if(funnel3DView != null)
				return new Funnel3DSeriesViewController(funnel3DView, series);
			FunnelSeriesView funnelView = view as FunnelSeriesView;
			if(funnelView != null)
				return new FunnelSeriesViewController(funnelView, series);
			Doughnut3DSeriesView doughnut3DView = view as Doughnut3DSeriesView;
			if(doughnut3DView != null)
				return new Doughnut3DSeriesViewController(doughnut3DView, series);
			Pie3DSeriesView pie3DView = view as Pie3DSeriesView;
			if(pie3DView != null)
				return new Pie3DSeriesViewController(pie3DView, series);
			NestedDoughnutSeriesView nestedDoughnutSeriesView = view as NestedDoughnutSeriesView;
			if (nestedDoughnutSeriesView != null)
				return new NestedDoughnutSeriesViewController(nestedDoughnutSeriesView, series);
			DoughnutSeriesView doughnutView = view as DoughnutSeriesView;
			if(doughnutView != null)
				return new DoughnutSeriesViewController(doughnutView, series);
			PieSeriesView pieView = view as PieSeriesView;
			if(pieView != null)
				return new PieSeriesViewController(pieView, series);
			return null;
		}
		public new SimpleDiagramSeriesViewBase View { get { return (SimpleDiagramSeriesViewBase)base.View; } }
		public override bool ColorEachSupported { get { return false; } }
		public override bool ColorSupported { get { return false; } }
		public virtual bool AppearanceSupported { get { return true; } }
		public virtual PieSeriesViewBase ExplodedOptions { get { return null; } }
		public virtual BorderBase Border { get { return null; } }
		public SimpleDiagramSeriesViewBaseController(SimpleDiagramSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public virtual ChartUserControl GetGeneralOptionsControl() {
			return null;
		}
		public virtual void RegisterUpdatableElement(ISupportNestedDoughnutPositionUpdate element) { }
	}
	internal class PieSeriesViewBaseController : SimpleDiagramSeriesViewBaseController {
		public override PieSeriesViewBase ExplodedOptions { get { return (PieSeriesViewBase)base.View; } }
		public PieSeriesViewBaseController(PieSeriesViewBase view, SeriesBase series) : base(view, series) { }
	}
	internal class PieSeriesViewController : PieSeriesViewBaseController {
		PieSeriesView PieView { get { return (PieSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return PieView.FillStyle; } }
		public override BorderBase Border { get { return PieView.Border; } }
		public PieSeriesViewController(PieSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			PieGeneralOptionsControl control = new PieGeneralOptionsControl();
			control.Initialize(PieView);
			return control;
		}
	}
	internal class DoughnutSeriesViewController : PieSeriesViewController {
		public DoughnutSeriesViewController(DoughnutSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			DoughnutGeneralOptionsControl control = new DoughnutGeneralOptionsControl();
			control.Initialize((DoughnutSeriesView)base.View);
			return control;
		}
	}
	internal class NestedDoughnutSeriesViewController : PieSeriesViewController {
		NestedDoughnutPositionUpdateBehavior updateBehavior;
		public NestedDoughnutSeriesViewController(NestedDoughnutSeriesView view, SeriesBase series) : base(view, series) {
			updateBehavior = new NestedDoughnutPositionUpdateBehavior(view);
		}
		public override ChartUserControl GetGeneralOptionsControl() {
			NestedDoughnutGeneralOptionsControl control = new NestedDoughnutGeneralOptionsControl();
			control.Initialize((NestedDoughnutSeriesView)base.View, Series, updateBehavior);
			updateBehavior.Update();
			return control;
		}
		public override void RegisterUpdatableElement(ISupportNestedDoughnutPositionUpdate element) {
			updateBehavior.RegisterUpdatableElement(element);
		}
	}
	internal class Pie3DSeriesViewController : PieSeriesViewBaseController {
		Pie3DSeriesView PieView { get { return (Pie3DSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return PieView.PieFillStyle; } }
		public Pie3DSeriesViewController(Pie3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Pie3DGeneralOptionsControl control = new Pie3DGeneralOptionsControl();
			control.Initialize(PieView);
			return control;
		}
	}
	internal class Doughnut3DSeriesViewController : Pie3DSeriesViewController {
		public Doughnut3DSeriesViewController(Doughnut3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Doughnut3DGeneralOptionsControl control = new Doughnut3DGeneralOptionsControl();
			control.Initialize((Doughnut3DSeriesView)base.View);
			return control;
		}
	}
	internal class FunnelSeriesViewController : SimpleDiagramSeriesViewBaseController {
		FunnelSeriesView FunnelView { get { return (FunnelSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return FunnelView.FillStyle; } }
		public override BorderBase Border { get { return FunnelView.Border; } }
		public FunnelSeriesViewController(FunnelSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			FunnelGeneralOptionsControl control = new FunnelGeneralOptionsControl();
			control.Initialize(FunnelView);
			return control;
		}
	}
	internal class Funnel3DSeriesViewController : SimpleDiagramSeriesViewBaseController {
		public override bool AppearanceSupported { get { return false; } }
		public Funnel3DSeriesViewController(Funnel3DSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			Funnel3DGeneralOptionsControl control = new Funnel3DGeneralOptionsControl();
			control.Initialize((Funnel3DSeriesView)base.View);
			return control;
		}
	}
	internal interface ISupportNestedDoughnutPositionUpdate {
		IEnumerable<ISupportNestedDoughnutPositionUpdate> Children { get; }
		void PositionUpdated(bool isOutside);
	}
	internal class NestedDoughnutPositionUpdateBehavior {
		DevExpress.Charts.Native.INestedDoughnutSeriesView view;
		List<ISupportNestedDoughnutPositionUpdate> supportUpdateElements = new List<ISupportNestedDoughnutPositionUpdate>();
		public NestedDoughnutPositionUpdateBehavior(DevExpress.Charts.Native.INestedDoughnutSeriesView view) {
			this.view = view;
		}
		public void RegisterUpdatableElement(ISupportNestedDoughnutPositionUpdate element) {
			supportUpdateElements.Add(element);
			IEnumerable<ISupportNestedDoughnutPositionUpdate> children = element.Children;
			if (children != null)
				supportUpdateElements.AddRange(children);
		}
		public void Update() {
			foreach (ISupportNestedDoughnutPositionUpdate element in supportUpdateElements) {
				element.PositionUpdated(view.IsOutside.Value);
			}
		}
	}
}
