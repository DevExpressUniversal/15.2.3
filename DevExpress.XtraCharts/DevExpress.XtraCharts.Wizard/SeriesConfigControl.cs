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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraTab;
using System.Collections;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesConfigControl : SplitterWizardControlBase {
		#region inner classes
		internal class SeriesViewContainer {
			ViewType viewType;
			public ViewType ViewType { get { return viewType; } }
			public SeriesViewContainer(ViewType viewType) {
				this.viewType = viewType;
			}
			public override string ToString() {
				return SeriesViewFactory.GetStringID(viewType);
			}
		}
		#endregion
		string seriesViewTypeErrorText;
		FilterSeriesTypesCollection filterCollection;
		SeriesBase SelectedSeries { get { return lvSeries.SelectedSeries; } }
		public XtraTabControl TabControl { get { return tbcOptions; } }
		public SeriesConfigControl() {
			InitializeComponent();
			lvSeries.AllowChangeSeries(true);
		}
		bool IsAutoBindingSettingsDisabled(SeriesBase series) {
			return Chart == null || !PivotGridDataSourceUtils.IsAutoBindingSettingsUsed(Chart.DataContainer.PivotGridDataSourceOptions, series);
		}
		bool IsSimpleSeriesAutomaticSettingsDisabled(SeriesBase series) {
			return !(series.View is SimpleDiagramSeriesViewBase) || Chart == null || !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(Chart.DataContainer.PivotGridDataSourceOptions, series);
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
		void InitializeSortByControl() {
			SeriesBase selectedSeries = SelectedSeries;
			if (selectedSeries != null) {
				cbSortBy.Properties.Items.Clear();
				int valuesCount = selectedSeries.ValueDataMembers.Count;
				cbSortBy.Properties.Items.Add(ChartLocalizer.GetString(ChartStringId.ArgumentMember));
				for (int i = 0; i < valuesCount; i++)
					cbSortBy.Properties.Items.Add(selectedSeries.View.GetValueCaption(i));
			}
		}
		void UpdateGeneralControls() {
			lvSeries.UpdateControls();
			SeriesBase selectedSeries = SelectedSeries;
			if (selectedSeries == null) {
				txtSeriesName.Enabled = false;
				txtSeriesName.EditValue = String.Empty;
				chVisible.Checked = false;
				chVisible.Enabled = false;
				chCheckableInLegend.Checked = false;
				chCheckableInLegend.Enabled = false;
				chCheckedInLegend.Checked = false;
				chCheckedInLegend.Enabled = false;
				grSeriesSettings.Enabled = false;
				pnlEditors.Enabled = false;
			}
			else {
				Series series = selectedSeries as Series;
				if (series == null) {
					txtSeriesName.Enabled = false;
					txtSeriesName.EditValue = ChartLocalizer.GetString(ChartStringId.AutocreatedSeriesName);
				}
				else {
					txtSeriesName.Enabled = true;
					txtSeriesName.EditValue = series.Name;
				}
				chVisible.Enabled = true;
				chVisible.Checked = selectedSeries.Visible;
				chCheckableInLegend.Checked = SelectedSeries.CheckableInLegend;
				chCheckableInLegend.Enabled = SelectedSeries.ShowInLegend && Chart.Legend.UseCheckBoxes;
				chCheckedInLegend.Checked = SelectedSeries.CheckedInLegend;
				chCheckedInLegend.Enabled = SelectedSeries.CheckableInLegend && SelectedSeries.ShowInLegend && Chart.Legend.UseCheckBoxes;
				grSeriesSettings.Enabled = true;
				pnlEditors.Enabled = true;
			}
		}
		void UpdateScaleControls() {
			SeriesBase selectedSeries = SelectedSeries;
			if (selectedSeries == null) {
				cbArgumentScaleType.Enabled = false;
				cbArgumentScaleType.SelectedIndex = -1;
				cbValueScaleType.Enabled = false;
				cbValueScaleType.SelectedIndex = -1;
			}
			else {
				cbArgumentScaleType.Enabled = true;
				cbArgumentScaleType.SelectedIndex = WizardScaleTypeHelper.GetArgumentScaleTypeIndex(selectedSeries.ArgumentScaleType);
				cbValueScaleType.Enabled = true;
				cbValueScaleType.SelectedIndex = WizardScaleTypeHelper.GetValueScaleTypeIndex(selectedSeries.ValueScaleType);
			}
			bool isAutoBindingSettingsNotUsed = IsAutoBindingSettingsDisabled(selectedSeries);
			pnlArgumentScaleType.Enabled = isAutoBindingSettingsNotUsed;
			pnlValueScaleType.Enabled = isAutoBindingSettingsNotUsed;
		}
		void UpdateSortControls() {
			InitializeSortByControl();
			SeriesBase selectedSeries = SelectedSeries;
			if (selectedSeries == null) {
				cbSortBy.Text = String.Empty;
				cbSortOrder.Text = String.Empty;
			}
			else {
				cbSortOrder.SelectedIndex = (int)selectedSeries.SeriesPointsSorting;
				cbSortBy.SelectedIndex = (int)selectedSeries.SeriesPointsSortingKey;
			}
		}
		void UpdateSelectViewTypeControl() {
			SeriesBase series = SelectedSeries;
			if (series != null) {
				ViewType viewType = SeriesViewFactory.GetViewType(series.View);
				foreach (ImageComboBoxItem item in this.cbSeriesViewType.Properties.Items)
					if (((SeriesViewContainer)item.Value).ViewType == viewType) {
						cbSeriesViewType.SelectedItem = item;
						return;
					}
			}
		}
		void UpdateTopNOptionsPage() {
			SeriesBase series = SelectedSeries;
			if (series == null)
				return;
			ITopNOptions topNOptions = (ITopNOptions)series;
			if (!topNOptions.ShouldApplyTopNOptions) {
				chTopNOptionsEnabled.Enabled = false;
				chTopNOptionsEnabled.Checked = false;
			}
			else {
				chTopNOptionsEnabled.Enabled = true;
				chTopNOptionsEnabled.Checked = series.TopNOptions.Enabled;
			}
		}
		void UpdateTopNOptionsControl() {
			SeriesBase series = SelectedSeries;
			if (series != null) {
				ITopNOptions topNOptions = (ITopNOptions)series;
				if (!topNOptions.ShouldApplyTopNOptions)
					tbTopNOptions.PageVisible = false;
				else {
					tbTopNOptions.PageVisible = true;
					series.TopNOptions.Enabled = chTopNOptionsEnabled.Checked;
					topNOptionsControl.Enabled = chTopNOptionsEnabled.Checked;
					topNOptionsControl.Initialize(series.TopNOptions, topNOptions.ShouldUseTopNOthers);
				}
			}
		}
		void UpdatePointOptionsControl() {
			if (SelectedSeries == null)
				return;
			IViewArgumentValueOptions viewOptions = (IViewArgumentValueOptions)SelectedSeries.View;
			if (viewOptions.IsSupportedPointOptions)
				legendPointOptionsControl.Initialize(SelectedSeries);
			tbLegendTextPattern.PageVisible = viewOptions.IsSupportedPointOptions;
		}
		void UpdateLegendText() {
			if (SelectedSeries == null)
				return;
			txtLegendText.Text = SelectedSeries.LegendText;
			pnlLegendText.Enabled = SelectedSeries.ShowInLegend && IsSimpleSeriesAutomaticSettingsDisabled(SelectedSeries);
		}
		void UpdateShowInLegend() {
			if (SelectedSeries == null)
				return;
			chShowInLegend.Checked = SelectedSeries.ShowInLegend;
			chShowInLegend.Enabled = IsSimpleSeriesAutomaticSettingsDisabled(SelectedSeries);
		}
		void UpdateControls() {
			UpdateGeneralControls();
			UpdateScaleControls();
			UpdateSortControls();
			UpdateSelectViewTypeControl();
			UpdatePointOptionsControl();
			UpdateTopNOptionsPage();
			UpdateTopNOptionsControl();
			UpdateLegendText();
			UpdateShowInLegend();
		}
		void lvSeries_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateControls();
		}
		void txtSeriesName_EditValueChanged(object sender, EventArgs e) {
			lvSeries.SetSeriesName(txtSeriesName.EditValue.ToString());
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.Visible = chVisible.Checked;
			lvSeries.SyncListView();
		}
		void cbSeriesViewType_SelectedIndexChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			ImageComboBoxItem item = (ImageComboBoxItem)cbSeriesViewType.SelectedItem;
			try {
				SeriesScaleTypeUtils.CheckScaleTypes(SelectedSeries, SeriesViewFactory.CreateInstance(((SeriesViewContainer)item.Value).ViewType));
				if (lvSeries.ChangeCurrentViewType(((SeriesViewContainer)item.Value).ViewType)) {
					UpdateControls();
					SetValidState();
					seriesViewTypeErrorText = "";
				}
			}
			catch (Exception ex) {
				SetInvalidState();
				seriesViewTypeErrorText = ex.Message;
			}
			((PopupBaseEdit)sender).ErrorText = seriesViewTypeErrorText;
		}
		void cbSeriesViewType_Validating(object sender, CancelEventArgs e) {
			if (InvalidState) {
				e.Cancel = true;
				((PopupBaseEdit)sender).ErrorText = seriesViewTypeErrorText;
			}
		}
		void cbSortBy_SelectedIndexChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.SeriesPointsSortingKey = (SeriesPointKey)cbSortBy.SelectedIndex;
		}
		void cbSortOrder_SelectedIndexChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SortingMode sorting = (SortingMode)cbSortOrder.SelectedIndex;
			SelectedSeries.SeriesPointsSorting = sorting;
			cbSortBy.Enabled = sorting != SortingMode.None;
			InitializeSortByControl();
		}
		void cbArgumentScaleType_Validating(object sender, CancelEventArgs e) {
			if (SelectedSeries == null)
				return;
			SetInvalidState();
			ScaleType scaleType = WizardScaleTypeHelper.GetArgumentScaleType(cbArgumentScaleType.SelectedIndex);
			e.Cancel = !DataBindingControl.ValidateArgumentScaleType(cbArgumentScaleType, SelectedSeries, scaleType);
		}
		void cbArgumentScaleType_Validated(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.ArgumentScaleType = WizardScaleTypeHelper.GetArgumentScaleType(cbArgumentScaleType.SelectedIndex);
			lvSeries.SyncListView();
			UpdatePointOptionsControl();
			UpdateTopNOptionsControl();
			SetValidState();
		}
		void cbValueScaleType_Validating(object sender, CancelEventArgs e) {
			if (SelectedSeries == null)
				return;
			SetInvalidState();
			ScaleType scaleType = WizardScaleTypeHelper.GetValueScaleType(cbValueScaleType.SelectedIndex);
			e.Cancel = !DataBindingControl.ValidateValueScaleType(cbValueScaleType, SelectedSeries, scaleType);
		}
		void cbValueScaleType_Validated(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.ValueScaleType = WizardScaleTypeHelper.GetValueScaleType(cbValueScaleType.SelectedIndex);
			lvSeries.SyncListView();
			UpdatePointOptionsControl();
			UpdateTopNOptionsPage();
			UpdateTopNOptionsControl();
			SetValidState();
		}
		void tbcOptions_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if (SelectedSeries == null)
				return;
			IViewArgumentValueOptions viewOptions = (IViewArgumentValueOptions)SelectedSeries.View;
			if (tbcOptions.SelectedTabPage == tbLegendTextPattern && 
				SelectedSeries != null && 
				viewOptions.IsSupportedPointOptions)
				legendPointOptionsControl.Initialize(SelectedSeries);
		}
		void txtLegendText_EditValueChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.LegendText = txtLegendText.Text;
		}
		void chTopNOptionsEnabled_CheckedChanged(object sender, EventArgs e) {
			UpdateTopNOptionsControl();
		}
		void chShowInLegend_CheckedChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.ShowInLegend = chShowInLegend.Checked;
			UpdateLegendText();
			chCheckableInLegend.Enabled = SelectedSeries.ShowInLegend && Chart.Legend.UseCheckBoxes;
			chCheckedInLegend.Enabled = SelectedSeries.ShowInLegend && Chart.Legend.UseCheckBoxes;
		}
		void chCheckableInLegend_CheckedChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.CheckableInLegend = chCheckableInLegend.Checked;
			chCheckedInLegend.Enabled = SelectedSeries.CheckableInLegend && SelectedSeries.ShowInLegend && Chart.Legend.UseCheckBoxes;
		}
		void chCheckedInLegend_CheckedChanged(object sender, EventArgs e) {
			if (SelectedSeries == null)
				return;
			SelectedSeries.CheckedInLegend = chCheckedInLegend.Checked;
		}
		internal void InitializeTags() {
			tbGeneral.Tag = SeriesPageTab.General;
			tbLegendTextPattern.Tag = SeriesPageTab.LegendTextPattern;
			tbSeriesOptions.Tag = SeriesPageTab.SeriesOptions;
			tbTopNOptions.Tag = SeriesPageTab.TopNOptions;
		}
		internal void InitializeTabControl(XtraTabControl tabControl, IList filter) {
			for (int i = tabControl.TabPages.Count - 1; i >= 0; i--) {
				XtraTabPage page = tabControl.TabPages[i];
				if (page.Tag == null)
					continue;
				if (filter.Contains(page.Tag))
					tabControl.TabPages.Remove(page);
			}
			if (tabControl.TabPages.Count > 0)
				tabControl.SelectedTabPageIndex = 0;
		}
		public override void InitializeChart(WizardFormBase form) {
			InitializeTags();
			base.InitializeChart(form);
			InitializeTabControl(tbcOptions, ((WizardSeriesPage)WizardPage).HiddenPageTabs);
			filterCollection = form.FilterSeriesCollection;
			InitializeSelectViewTypeControl();
			lvSeries.Initialize(form.Chart, form.OriginalChart);
			lvSeries.SetSelectedIndex(0);
			InitializeSortByControl();
			UpdateControls();
		}
	}
}
