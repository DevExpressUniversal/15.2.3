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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	internal partial class LegendControl : ChartUserControl {
		Chart chart;
		Legend legend;
		public LegendControl() {
			InitializeComponent();
		}
		void UpdateControls() {
			bool isControlsEnabled = legend.Visibility != DefaultBoolean.False;
			markerControl.Enabled = isControlsEnabled;
			backgroundControl.Enabled = isControlsEnabled;
			shadowControl.Enabled = isControlsEnabled;
			legendGeneralControl.Enabled = isControlsEnabled;
			textControl.Enabled = isControlsEnabled;
			appearanceControl.Enabled = isControlsEnabled;
			interiorControl.Enabled = isControlsEnabled;
			SimpleDiagramSeriesViewBase view = chart.DataContainer.SeriesTemplate.View as SimpleDiagramSeriesViewBase;
			chVisible.Enabled = view == null || !PivotGridDataSourceUtils.HasDataSource(chart.DataContainer.PivotGridDataSourceOptions) || !chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled;
		}
		void chVisible_CheckStateChanged(object sender, EventArgs e) {
			legend.Visibility = CheckEditHelper.GetCheckEditState(chVisible);
			UpdateControls();
		}
		void InitializeTags() {			
			tbGeneral.Tag = LegendPageTab.General;
			tbAppearance.Tag = LegendPageTab.Appearance;
			tbInterior.Tag = LegendPageTab.Interior;
			tbMarker.Tag = LegendPageTab.Marker;
			tbText.Tag = LegendPageTab.Text;
			tbBorder.Tag = LegendPageTab.Border;
			tbShadow.Tag = LegendPageTab.Shadow;
		}
		void HideInvisibleTabs(LegendPageTabCollection filterTabs) {
			for (int i = tbcPageControl.TabPages.Count - 1; i >= 0; i--) {
				XtraTabPage page = tbcPageControl.TabPages[i];
				if (page.Tag == null)
					continue;
				if (filterTabs.Contains((LegendPageTab)page.Tag))
					tbcPageControl.TabPages.Remove(page);
			}
		}
		public void Initialize(UserLookAndFeel lookAndFeel, Chart chart, LegendPageTabCollection hiddenPages, Chart originalChart) {
			this.chart = chart;
			this.legend = chart.Legend;
			InitializeTags();
			HideInvisibleTabs(hiddenPages);
			CheckEditHelper.SetCheckEditState(chVisible, legend.Visibility);
			textControl.Initialize(lookAndFeel, legend);
			appearanceControl.Initialize(legend);
			legendGeneralControl.Initialize(chart);
			backgroundControl.Initialize(legend, originalChart);
			shadowControl.Initialize(legend.Shadow);
			markerControl.Initialize(legend);
			interiorControl.Initialize(legend);
			UpdateControls();
			tbcPageControl.SelectedTabPageIndex = 0;
		}
	}
}
