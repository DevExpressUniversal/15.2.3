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
	internal partial class RadarSeriesViewControl : SeriesViewControlBase {
		RadarSeriesViewController controller;
		public RadarSeriesViewControl() {
			InitializeComponent();
		}
		protected override void InitializeTabs() {
			base.InitializeTabs();
			controller = RadarSeriesViewController.CreateInstance(View as RadarSeriesViewBase, Series);
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			if(controller == null)
				return;
			ChartUserControl generalOptionsControl = controller.GetGeneralOptionsControl();
			if(generalOptionsControl != null) {
				generalOptionsControl.Dock = DockStyle.Fill;
				tbGeneral.Controls.Add(generalOptionsControl);
			}
			else
				tbcPagesControl.TabPages.Remove(tbGeneral);
			appearanceControl.Initialize(controller);
			if(controller.Border != null)
				borderControl.Initialize(controller.Border);
			else
				tbcPagesControl.TabPages.Remove(tbBorder);
			if(controller.Shadow != null)
				shadowControl.Initialize(controller.Shadow);
			else
				tbcPagesControl.TabPages.Remove(tbShadow);
			ChartUserControl markerControl = controller.GetMarkerControl();
			if(markerControl != null) {
				markerControl.Dock = DockStyle.Fill;
				tbMarker.Controls.Add(markerControl);
			}
			else
				tbcPagesControl.TabPages.Remove(tbMarker);
		}
		protected override void InitializeTags() {
			base.InitializeTags();
			if(controller == null)
				return;
			ChartUserControl generalOptionsControl = controller.GetGeneralOptionsControl();
			if(generalOptionsControl != null)
				tbGeneral.Tag = GetPageTabByName("General");
			tbAppearance.Tag = GetPageTabByName("Appearance");
			if(controller.Border != null)
				tbBorder.Tag = GetPageTabByName("Border");
			if(controller.Shadow != null)
				tbShadow.Tag = GetPageTabByName("Shadow");
			ChartUserControl markerControl = controller.GetMarkerControl();
			if(markerControl != null)
				tbMarker.Tag = GetPageTabByName("Marker");
		}
	}
	internal class RadarSeriesViewController : SeriesViewBaseController {
		public static RadarSeriesViewController CreateInstance(RadarSeriesViewBase view, SeriesBase series) {
			RadarAreaSeriesView areaView = view as RadarAreaSeriesView;
			if(areaView != null)
				return new RadarAreaSeriesViewController(areaView, series);
			RadarLineSeriesView lineView = view as RadarLineSeriesView;
			if(lineView != null)
				return new RadarLineSeriesViewController(lineView, series);
			RadarPointSeriesView pointView = view as RadarPointSeriesView;
			if(pointView != null)
				return new RadarPointSeriesViewController(pointView, series);
			return null;
		}
		RadarSeriesViewBase RadarView { get { return (RadarSeriesViewBase)base.View; } }
		public override bool ColorEachSupported { get { return true; } }
		public override bool ColorEach { get { return RadarView.ColorEach; } set { RadarView.ColorEach = value; } }
		public virtual Shadow Shadow { get { return null; } }
		public virtual BorderBase Border { get { return null; } }
		public RadarSeriesViewController(RadarSeriesViewBase view, SeriesBase series) : base(view, series) { }
		public virtual ChartUserControl GetGeneralOptionsControl() {
			return null;
		}
		public virtual ChartUserControl GetMarkerControl() {
			return null;
		}
	}
	internal class RadarPointSeriesViewController : RadarSeriesViewController {
		RadarPointSeriesView PointView { get { return (RadarPointSeriesView)base.View; } }
		public override Shadow Shadow { get { return PointView.Shadow; } }
		public RadarPointSeriesViewController(RadarPointSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			MarkerBaseControl control = new MarkerBaseControl();
			control.Initialize(PointView.PointMarkerOptions);
			return control;
		}
	}
	internal class RadarLineSeriesViewController : RadarPointSeriesViewController {
		RadarLineSeriesView LineView { get { return (RadarLineSeriesView)base.View; } }
		public override LineStyle LineStyle { get { return LineView.LineStyle; } }
		public RadarLineSeriesViewController(RadarLineSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetGeneralOptionsControl() {
			RadarLineGeneralOptionsControl control = new RadarLineGeneralOptionsControl();
			control.Initialize(LineView);
			return control;
		}
		public override ChartUserControl GetMarkerControl() {
			MarkerControl control = new MarkerControl();
			control.Initialize(LineView.LineMarkerOptions, LineView);
			return control;
		}
	}
	internal class RadarAreaSeriesViewController : RadarPointSeriesViewController {
		RadarAreaSeriesView AreaView { get { return (RadarAreaSeriesView)base.View; } }
		public override FillStyleBase FillStyle { get { return AreaView.FillStyle; } }
		public override BorderBase Border { get { return AreaView.Border; } }
		public RadarAreaSeriesViewController(RadarAreaSeriesView view, SeriesBase series) : base(view, series) { }
		public override ChartUserControl GetMarkerControl() {
			MarkerControl control = new MarkerControl();
			control.Initialize(AreaView.MarkerOptions, AreaView);
			return control;
		}
	}
}
