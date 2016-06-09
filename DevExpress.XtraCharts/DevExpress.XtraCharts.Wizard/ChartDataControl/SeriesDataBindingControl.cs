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
using System.Collections;
using System.ComponentModel.Design;
using DevExpress.Data.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesDataBindingControl : InternalWizardControlBase {
		Chart chart;
		IDesignerHost designerHost = null;
		IServiceProvider serviceProvider;
		Series SelectedSeries { get { return lvSeries.SelectedSeries as Series; } }
		object SeriesDataSource {
			get {
				Series series = SelectedSeries;
				return series == null ? null : SeriesDataBindingUtils.GetDataSource(series);
			}
		}
		bool DataSourceExists { get { return SeriesDataSource != null; } }
		string ChartDataMember {
			get {
				Series series = SelectedSeries;
				return series == null ? String.Empty : SeriesDataBindingUtils.GetDataMember(series);
			}
		}
		public SeriesDataBindingControl() {
			InitializeComponent();
			lvSeries.AllowChangeSeries(false);
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			chart = form.Chart;
			serviceProvider = form.ServiceProvider;
			designerHost = form.DesignerHost;
			dataBindingControl.Initialize(form.FormLayout, chart, form.ServiceProvider);
			if (chart.Container.ControlType != ChartContainerType.XRControl)
				pnlDataMember.Visible = false;
			IDataSourceCollectorService dataSourceCollectorService = serviceProvider.GetService(typeof(IDataSourceCollectorService)) as IDataSourceCollectorService;
			if (designerHost == null && dataSourceCollectorService == null)
				cbDataSource.Enabled = false;
			lvSeries.Initialize(form.Chart, form.OriginalChart);
			UpdateControls();
		}
		public override void Release() {
			base.Release();
			dataBindingControl.Shutdown();
		}
		void UpdateFilterControl() {
			Series series = SelectedSeries;
			if (series == null || !DataSourceExists) {
				beFilters.Enabled = false;
				beFilters.EditValue = ChartLocalizer.GetString(ChartStringId.WizDataFiltersDisabled);
			}
			else {
				beFilters.Enabled = true;
				int count = series.DataFilters.Count;
				beFilters.EditValue = count == 0 ? ChartLocalizer.GetString(ChartStringId.WizSpecifyDataFilters) :
					String.Format(ChartLocalizer.GetString(ChartStringId.WizDataFiltersEntered), count);
			}
		}
		void UpdateControls() {
			WizardDataBindingHelper.UpdateDataSourceComboBox(cbDataSource, chart.Container, serviceProvider, SeriesDataSource);
			Series series = SelectedSeries;
			if (series == null) {
				panel.Enabled = false;
				grSeriesTemplate.Enabled = false;
			}
			else {
				panel.Enabled = true;
				grSeriesTemplate.Enabled = true;
			}
			dataBindingControl.SetSeries(series);
			UpdateFilterControl();
		}
		void lvSeries_SelectedIndexChanged(object sender, EventArgs e) {
			Series series = SelectedSeries;
			if (series != null) {
				lvSeries.UpdateControls();
				cbChartDataMember.EditValue = ChartDataMember;
				UpdateControls();
			}
		}
		void cbDataSource_SelectedIndexChanged(object sender, EventArgs e) {
			Series series = SelectedSeries;
			if (series != null) {
				DataSourceComboBoxItem item = cbDataSource.SelectedItem as DataSourceComboBoxItem;
				if (item != null) {
					series.DataSource = item.DataSource == chart.DataContainer.ActualDataSource ? null : item.DataSource;
					dataBindingControl.SetSeries(series);
					UpdateFilterControl();
				}
			}
		}
		void cbChartDataMember_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if(chart == null) 
				return;
			IChartDataProvider dataProvider = chart.Container.DataProvider;
			e.DisplayText = BindingHelper.GetDataMemberName(dataProvider != null ? dataProvider.DataContext : null, SeriesDataSource, String.Empty, (string)e.Value);
		}
		void beFilters_ButtonClick(object sender, ButtonPressedEventArgs e) {
			Series series = SelectedSeries;
			if (series != null) {
				using (DataFilterCollectionForm form = new DataFilterCollectionForm(series.DataFilters)) {
					form.ShowDialog();
					UpdateFilterControl();
				}
			}
		}
		void dataBindingControl_ValuableBindingChanged(object sender, EventArgs e) {
			lvSeries.SyncListView();
		}
	}
}
