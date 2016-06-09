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
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.Controls;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesListBoxControl : XtraUserControl {
		class SeriesListBoxItem : ImageListBoxItem {
			public SeriesBase Series { get { return (SeriesBase)Value; } }
			public SeriesListBoxItem(SeriesBase series, int imageIndex) : base(series, imageIndex) {}
			public override string ToString() {
				return (Value is Series) ? base.ToString() : ChartLocalizer.GetString(ChartStringId.AutocreatedSeriesName);
			}
		}
		Font boldListBoxFont;
		Locker changeLocker = new Locker();
		Chart chart;
		DataContainer DataContainer { get { return chart == null ? null : chart.DataContainer; } }
		Chart originalChart;
		bool allowSeriesTemplate;
		int prevTtooltipIndex = -1;
		public event EventHandler SelectedSeriesChanged;
		public SeriesBase SelectedSeries {
			get { return lvSeries.SelectedItem == null ? null : ((SeriesListBoxItem)lvSeries.SelectedItem).Series; }
		}
		public SeriesListBoxControl() {
			InitializeComponent();
			btnUp.Image = ResourceImageHelper.CreateImageFromResources(ImageResourcesUtils.WizardImagePath + "up.png", Assembly.GetAssembly(typeof(Chart)));
			btnUp.ImageLocation = ImageLocation.MiddleCenter;
			btnDown.Image = ResourceImageHelper.CreateImageFromResources(ImageResourcesUtils.WizardImagePath + "down.png", Assembly.GetAssembly(typeof(Chart)));
			btnDown.ImageLocation = ImageLocation.MiddleCenter;
			ttController.SetToolTipIconType(lvSeries, ToolTipIconType.Exclamation);
		}
		public void Initialize(Chart chart, Chart originalChart) {
			this.chart = chart;
			this.originalChart = originalChart;
			SyncListView();
			SetSelectedIndex(0);
		}
		public void SetSelectedIndex(int index) {
			if (index >= 0 && index < lvSeries.ItemCount)
				lvSeries.SelectedIndex = index;
		}
		public void SetSelectedSeries(SeriesBase series) {
			foreach (SeriesListBoxItem item in lvSeries.Items)
				if (item.Series == series) {
					lvSeries.SelectedItem = item;
					return;
				}
		}
		public void UpdateControls() {
			Series series = SelectedSeries as Series;
			if (series == null) {
				btnRemove.Enabled = false;
				btnCopy.Enabled = false;
				btnUp.Enabled = false;
				btnDown.Enabled = false;
			}
			else {
				btnRemove.Enabled = true;
				btnCopy.Enabled = true;
				int index = chart.Series.IndexOf(series);
				btnUp.Enabled = index > 0 && !series.IsAutoCreated && !chart.Series[index - 1].IsAutoCreated;
				btnDown.Enabled = !series.IsAutoCreated && index < chart.Series.Count - 1; 
			}
		}
		public void SetSeriesName(string name) {
			Series series = SelectedSeries as Series;
			if (series != null && !changeLocker.IsLocked) {
				changeLocker.Lock();
				try {
					series.Name = name;
					SyncListView();
					SetSelectedSeries(series);
				}
				finally {
					changeLocker.Unlock();
				}
			}
		}
		public bool ChangeCurrentViewType(ViewType viewType) {
			SeriesBase series = SelectedSeries;
			if (!changeLocker.IsLocked && series != null && SeriesViewFactory.GetViewType(series.View) != viewType) {
				try {
					series.ChangeView(viewType);
				}
				catch (ArgumentException ex) {
					MessageBox(ex.Message);
					return false;
				}
				changeLocker.Lock();
				try {
	   				SyncListView();
					SetSelectedSeries(series);
				}
				finally {
					changeLocker.Unlock();
				}
			}
			return true;
		}
		public void AllowChangeSeries(bool allow) {
			pnlChangePosition.Visible = allow;
			pnlChangeCount.Visible = allow;
			allowSeriesTemplate = allow;
		}
		public void SyncListView() {
			changeLocker.Lock();
			lvSeries.BeginUpdate();
			lvSeries.Items.BeginUpdate();
			try {
				lvSeries.Items.Clear();
				seriesImages.Images.Clear();
				if (allowSeriesTemplate && BindingHelper.HasBoundSeries(chart))
					AddSeriesToList(DataContainer.SeriesTemplate);
				foreach (Series series in chart.Series)
					if (!series.IsAutoCreated)
						AddSeriesToList(series);
			}
			finally {
				lvSeries.Items.EndUpdate();
				lvSeries.EndUpdate();
				changeLocker.Unlock();
			}
		}
		void MessageBox(string message) {
			originalChart.Container.ShowErrorMessage(message, ChartLocalizer.GetString(ChartStringId.WizErrorMessageTitle));
		}
		void AddSeriesToList(SeriesBase series) {
			seriesImages.Images.Add(ImageResourcesUtils.GetImageFromResources(series.View, SeriesViewImageType.SmallImage));
			lvSeries.Items.Add(new SeriesListBoxItem(series, seriesImages.Images.Count - 1));
		}
		private void lvSeries_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			SeriesBase series = (SeriesBase)e.Item;
			if (boldListBoxFont == null)
				boldListBoxFont = new Font(e.Appearance.Font, FontStyle.Bold);
			if (!(series is Series))
				e.Appearance.Font = boldListBoxFont;
		}
		private void lvSeries_MouseMove(object sender, MouseEventArgs e) {
			if (!Object.ReferenceEquals(sender, lvSeries))
				return;
			int index = lvSeries.IndexFromPoint(e.Location);
			if (index != prevTtooltipIndex) {
				prevTtooltipIndex = index;
				if (index > -1) {
					Series series = lvSeries.Items[index].Value as Series;
					SeriesIncompatibilityStatistics incompatibilityStatistics = CommonUtils.GetSeriesIncompatibilityStatistics(chart);
					if (series != null && incompatibilityStatistics.IsSeriesIncompatible(series) && chart.Container.ShowDesignerHints) {
						SeriesIncompatibilityInfo info = incompatibilityStatistics[series];
						ToolTipControlInfo ttInfo = new ToolTipControlInfo(lvSeries,
							ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesHeader) + SeriesIncompatibilityHelper.ConstructMessage(info), ToolTipIconType.Exclamation);
						ToolTipControllerShowEventArgs args = new ToolTipControllerShowEventArgs(lvSeries, null);
						args.SuperTip.Items.Add(ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesHeader) + SeriesIncompatibilityHelper.ConstructMessage(info));
						args.ToolTipType = ToolTipType.SuperTip;
						ttController.ShowHint(args);
						return;
					}
				}
				ttController.HideHint();
			}
		}
		private void lvSeries_SelectedIndexChanged(object sender, System.EventArgs e) {
			if (SelectedSeriesChanged != null && !changeLocker.IsLocked) {
				changeLocker.Lock();
				try {
					SelectedSeriesChanged(sender, e);
				}
				finally {
					changeLocker.Unlock();
				}
			}
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			SeriesBase series = DataContainer.SeriesTemplate;
			Series newSeries = ChartElementFactory.CreateEmptySeries(chart.Series.GenerateName(), chart, series.View);
			newSeries.ArgumentScaleType = DataContainer.SeriesTemplate.ArgumentScaleType;
			newSeries.ValueScaleType = DataContainer.SeriesTemplate.ValueScaleType;
			DevExpress.XtraCharts.Design.ChartDesignHelper.InitializeDefaultGanttScaleType(newSeries);
			chart.Series.Add(newSeries);
			SyncListView();
			SetSelectedSeries(newSeries);
		}
		private void btnCopy_Click(object sender, EventArgs e) {
			Series series = SelectedSeries as Series;
			if (series != null) {
				SeriesCollection collection = chart.Series;
				Series newSeries = (Series)series.Clone();
				newSeries.Name = collection.GenerateName();
				collection.Add(newSeries);
				SyncListView();
				SetSelectedSeries(newSeries);
			}
		}
		private void btnRemove_Click(object sender, System.EventArgs e) {
			Series series = SelectedSeries as Series;
			if (series != null) {
				int index = lvSeries.SelectedIndex;
				chart.Series.Remove(series);
				SyncListView();
				if (index >= lvSeries.ItemCount)
					index = lvSeries.ItemCount - 1;
				if (index == -1) {
					btnAdd.Focus();
					SelectedSeriesChanged(this, EventArgs.Empty);
				}
				else if (lvSeries.SelectedIndex != index)
					SetSelectedIndex(index);
				else {
					UpdateControls();
					SelectedSeriesChanged(this, EventArgs.Empty);
				}
			}
		}
		private void btnUp_Click(object sender, System.EventArgs e) {
			Series series = SelectedSeries as Series;
			if (series != null) {
				int seriesIndex = chart.Series.IndexOf(series);
				if (seriesIndex > 0) {
					chart.Series.Swap(seriesIndex - 1, seriesIndex);
					SyncListView();
					SetSelectedSeries(series);
				}
			}
		}
		private void btnDown_Click(object sender, System.EventArgs e) {
			Series series = SelectedSeries as Series;
			if (series != null) {
				int seriesIndex = chart.Series.IndexOf(series);
				if (seriesIndex < chart.Series.Count - 1) {
					chart.Series.Swap(seriesIndex, seriesIndex + 1);
					SyncListView();
					SetSelectedSeries(series);
				}
			}
		}
	}
}
