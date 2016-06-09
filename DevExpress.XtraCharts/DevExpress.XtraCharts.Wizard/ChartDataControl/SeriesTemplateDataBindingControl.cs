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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraTab;
using DevExpress.Data.Design;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesTemplateDataBindingControl : InternalWizardControlBase {
		class SeriesViewContainer {
			ViewType viewType;
			public ViewType ViewType { get { return viewType; } }
			public SeriesViewContainer(ViewType viewType) {
				this.viewType = viewType;
			}
			public override string ToString() {
				return SeriesViewFactory.GetStringID(viewType);
			}
		}
		Chart chart;
		IDesignerHost designerHost = null;
		IServiceProvider serviceProvider;
		FilterSeriesTypesCollection filterCollection;
		DataContainer DataContainer { get { return chart == null ? null : chart.DataContainer; } }
		public SeriesBase SeriesTemplate { get { return DataContainer.SeriesTemplate; } }
		bool DataSourceExist { get { return DataContainer.ActualDataSource != null; } }
		public SeriesTemplateDataBindingControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			chart = form.Chart;
			designerHost = form.DesignerHost;
			serviceProvider = form.ServiceProvider;
			filterCollection = form.FilterSeriesCollection;
			dataBindingControl.Initialize(form.FormLayout, chart, form.ServiceProvider);
			IDataSourceCollectorService dataSourceCollectorService = serviceProvider.GetService(typeof(IDataSourceCollectorService)) as IDataSourceCollectorService;
			if (designerHost == null && dataSourceCollectorService == null)
				cbDataSource.Enabled = false;
			if (chart.Container.ControlType != ChartContainerType.XRControl)
				pnlDataMember.Visible = false;
			InitializePointSortingControl();
			InitializeSelectViewTypeControl();
			WizardDataBindingHelper.UpdateDataSourceComboBox(cbDataSource, chart.Container, serviceProvider, DataContainer.ActualDataSource);
			UpdateControls();
		}
		public override void Release() {
			base.Release();
			dataBindingControl.Shutdown();
		}
		void InitializePointSortingControl() {
			cbPointSortingKey.Properties.Items.Clear();
			int valuesCount = SeriesTemplate.ValueDataMembers.Count;
			cbPointSortingKey.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.ArgumentMember));
			for (int i = 0; i < valuesCount; i++)
				cbPointSortingKey.Properties.Items.Add(SeriesTemplate.View.GetValueCaption(i));
		}
		void InitializeSelectViewTypeControl() {
			ImageCollection imageList = new ImageCollection();
			imageList.TransparentColor = Color.Magenta;
			cbSeriesViewType.Properties.SmallImages = imageList;
			int i = 0;
			foreach (ViewType viewType in SeriesViewFactory.ViewTypes)
				if (!filterCollection.Contains(viewType)) {
					SeriesViewBase view = SeriesViewFactory.CreateInstance(viewType);
					imageList.Images.Add(ImageResourcesUtils.GetImageFromResources(view, SeriesViewImageType.SmallImage));
					ImageComboBoxItem item = new ImageComboBoxItem(new SeriesViewContainer(viewType), i++);
					cbSeriesViewType.Properties.Items.Add(item);
				}
		}
		void UpdateDataControls() {
			cbChartDataMember.EditValue = DataContainer.DataMember;
			cbSeriesDataMember.EditValue = DataContainer.SeriesDataMember;
			dataBindingControl.SetSeries(DataContainer.SeriesTemplate);
		}
		void UpdateSortAndFilterControls() {
			InitializePointSortingControl();
			cbPointSorting.SelectedIndex = (int)SeriesTemplate.SeriesPointsSorting;
			cbPointSortingKey.SelectedIndex = (int)SeriesTemplate.SeriesPointsSortingKey;
			cbPointSortingKey.Enabled = (int)SeriesTemplate.SeriesPointsSorting != 0;
			cbSeriesSorting.SelectedIndex = (int)chart.DataContainer.BoundSeriesSorting;
			beFilters.Enabled = DataSourceExist;
			if (DataSourceExist)
				beFilters.Text = SeriesTemplate.DataFilters.Count == 0 ? ChartLocalizer.GetString(ChartStringId.WizSpecifyDataFilters) :
					String.Format(ChartLocalizer.GetString(ChartStringId.WizDataFiltersEntered), SeriesTemplate.DataFilters.Count);
			else
				beFilters.Text = ChartLocalizer.GetString(ChartStringId.WizDataFiltersDisabled);
		}
		void UpdateControls() {
			UpdateDataControls();
			UpdateSortAndFilterControls();
			txtBeginText.Text = DataContainer.SeriesNameTemplate.BeginText;
			txtEndText.Text = DataContainer.SeriesNameTemplate.EndText;
			foreach (ImageComboBoxItem item in cbSeriesViewType.Properties.Items)
				if (((SeriesViewContainer)item.Value).ViewType == SeriesViewFactory.GetViewType(SeriesTemplate.View)) {
					cbSeriesViewType.SelectedItem = item;
					break;
				}
			cheAutoBindingSettings.Checked = chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled;
			cheAutoLayoutSettings.Checked = chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled;
			pnlSeriesName.Enabled = !PivotGridDataSourceUtils.HasDataSource(DataContainer.PivotGridDataSourceOptions) || !chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled;
		}
		bool ValidateDataMember(PopupBaseEdit edit, string chartDataMember) {
			string dataMember = (string)edit.EditValue;
			IChartDataProvider dataProvider = chart.Container.DataProvider;
			if (BindingHelper.CheckDataMember(dataProvider != null ? dataProvider.DataContext : null, DataContainer.ActualDataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember)))
				return true;
			edit.ErrorText = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), dataMember);
			return false;
		}
		void cbDataSource_SelectedIndexChanged(object sender, EventArgs e) {
			if (SeriesTemplate != null) {
				DataSourceComboBoxItem item = cbDataSource.SelectedItem as DataSourceComboBoxItem;
				if (item != null) {
					DataContainer.DataSource = item.DataSource;
					UpdateControls();
					XtraTabControl tabControl = WizardPage.ParentControl.Control.Parent as XtraTabControl;
					if (tabControl != null) {
						NewChartDataControl dataControl = tabControl.Parent as NewChartDataControl;
						if (dataControl != null)
							dataControl.UpdatePages();
					}
				}
			}
		}
		void CloseUp(object sender, CloseUpEventArgs e) {
			dataBindingControl.StopDataMemberPicker();
		}
		void cbChartDataMember_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if (chart != null) {
				IChartDataProvider dataProvider = chart.Container.DataProvider;
				e.DisplayText = BindingHelper.GetDataMemberName(dataProvider != null ? dataProvider.DataContext : null, DataContainer.ActualDataSource, String.Empty, (string)e.Value);
			}
		}
		void cbChartDataMember_QueryPopUp(object sender, CancelEventArgs e) {
			dataBindingControl.StartDataMemberPicker((PopupContainerEdit)sender, new ScaleType[0], true);
		}
		void cbChartDataMember_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			e.Value = dataBindingControl.GetPickerResult(false);
		}
		void cbChartDataMember_Validating(object sender, CancelEventArgs e) {
			string dataMember = (string)cbChartDataMember.EditValue;
			if (!String.IsNullOrEmpty(dataMember)) {
				SetInvalidState();
				e.Cancel = !ValidateDataMember(cbChartDataMember, String.Empty);
			}
		}
		void cbChartDataMember_Validated(object sender, EventArgs e) {
			DataContainer.DataMember = (string)cbChartDataMember.EditValue;
			SetValidState();
			UpdateDataControls();
		}
		void cbSeriesDataMember_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if (chart != null) {
				IChartDataProvider dataProvider = chart.Container.DataProvider;
				e.DisplayText = BindingHelper.GetDataMemberName(dataProvider != null ? dataProvider.DataContext : null, DataContainer.ActualDataSource, DataContainer.DataMember, (string)e.Value);
			}
		}
		void cbSeriesDataMember_QueryPopUp(object sender, CancelEventArgs e) {
			dataBindingControl.StartDataMemberPicker((PopupContainerEdit)sender, new ScaleType[0], false);
		}
		void cbSeriesDataMember_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			e.Value = BindingHelper.ExtractDataMember(dataBindingControl.GetPickerResult(true), DataContainer.DataMember);
		}
		void cbSeriesDataMember_Validating(object sender, CancelEventArgs e) {
			string dataMember = (string)cbSeriesDataMember.EditValue;
			if (!String.IsNullOrEmpty(dataMember)) {
				SetInvalidState();
				e.Cancel = !ValidateDataMember(cbSeriesDataMember, DataContainer.DataMember);
			}
		}
		void cbSeriesDataMember_Validated(object sender, EventArgs e) {
			DataContainer.SeriesDataMember = (string)cbSeriesDataMember.EditValue;
			SetValidState();
		}
		void cbSeriesViewType_Validating(object sender, CancelEventArgs e) {
			SetInvalidState();
			ImageComboBoxItem item = (ImageComboBoxItem)cbSeriesViewType.SelectedItem;
			try {
				SeriesScaleTypeUtils.CheckScaleTypes(SeriesTemplate, SeriesViewFactory.CreateInstance(((SeriesViewContainer)item.Value).ViewType));
			}
			catch (Exception ex) {
				((PopupBaseEdit)sender).ErrorText = ex.Message;
				e.Cancel = true;
			}
		}
		void cbSeriesViewType_Validated(object sender, EventArgs e) {
			ImageComboBoxItem item = (ImageComboBoxItem)cbSeriesViewType.SelectedItem;
			SeriesTemplate.ChangeView(((SeriesViewContainer)item.Value).ViewType);
			UpdateControls();
			SetValidState();
		}
		void txtBeginText_EditValueChanged(object sender, EventArgs e) {
			DataContainer.SeriesNameTemplate.BeginText = txtBeginText.Text;
		}
		void txtEndText_EditValueChanged(object sender, EventArgs e) {
			DataContainer.SeriesNameTemplate.EndText = txtEndText.Text;
		}
		void cbSeriesSorting_SelectedIndexChanged(object sender, EventArgs e) {
			chart.DataContainer.BoundSeriesSorting = (SortingMode)cbSeriesSorting.SelectedIndex;
		}
		void cbPointSorting_SelectedIndexChanged(object sender, EventArgs e) {
			SeriesTemplate.SeriesPointsSorting = (SortingMode)cbPointSorting.SelectedIndex;
			UpdateSortAndFilterControls();
		}
		void cbPointSortingKey_SelectedIndexChanged(object sender, EventArgs e) {
			SeriesTemplate.SeriesPointsSortingKey = (SeriesPointKey)cbPointSortingKey.SelectedIndex;
			UpdateSortAndFilterControls();
		}
		void beFilters_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (DataFilterCollectionForm form = new DataFilterCollectionForm(SeriesTemplate.DataFilters)) {
				form.ShowDialog();
			}
			UpdateSortAndFilterControls();
		}
		void SeriesTemplateDataBindingControl_Validating(object sender, CancelEventArgs e) {
			e.Cancel = InvalidState;
		}
		void cheAutoBindingSettings_CheckedChanged(object sender, EventArgs e) {
			chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled = cheAutoBindingSettings.Checked;
			UpdateControls();
		}
		void cheAutoLayoutSettings_CheckedChanged(object sender, EventArgs e) {
			chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled = cheAutoLayoutSettings.Checked;
		}
	}
}
