#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.Data.ChartDataSources;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.ExpressApp.Chart {
	public class ChartControlContainer : IDisposable {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool EnableRangeAutoAdjust = true;
		private IChartContainer chartContainer;
		private string defaultSettings;
		private IChartDataSourceProvider dataSourceProvider;
		private bool? isAutoRange = false;
		private bool inDataChanged = false;
		private void dataSourceProvider_DataChanged(object sender, DataChangedEventArgs e) {
			if(!inDataChanged) {
				try {
					inDataChanged = true;
					OnDataSourceChanged();
				}
				finally {
					inDataChanged = false;
				}
			}
		}
		private void InitializeDefaultSettings() {
			MemoryStream ms = new MemoryStream();
			chartContainer.Chart.SaveLayout(ms);
			defaultSettings = Encoding.UTF8.GetString(ms.ToArray());
		}
		private void LoadChartSettings(string settings) {
			if(chartContainer != null && chartContainer.Chart != null && settings.Contains(@"ChartXmlSerializer")) {
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(settings));
				chartContainer.Chart.LoadLayout(ms);
				chartContainer.Chart.OptionsPrint.SizeMode = XtraCharts.Printing.PrintSizeMode.Zoom;
			}
		}
		private void LoadChartSettingsFromStorageProvider(ISettingsProvider settingsStorageProvider) {
			if(settingsStorageProvider != null && !string.IsNullOrEmpty(settingsStorageProvider.Settings)) {
				LoadChartSettings(settingsStorageProvider.Settings);
			}
		}
		private void OnDataSourceChanged() {
			if(chartContainer != null && dataSourceProvider != null) {
				ResetAutoRange();
				LoadChartSettings(defaultSettings);
				DefaultInitialization(dataSourceProvider);
			}
		}
		protected void ResetAutoRange() {
			if(!EnableRangeAutoAdjust) { return; }
			if(isAutoRange.HasValue && isAutoRange.Value && ChartContainer.Chart.Diagram is XYDiagram) {
				(ChartContainer.Chart.Diagram as XYDiagram).AxisX.VisualRange.Auto = true;
			}
			isAutoRange = null;
		}
		protected void UpdateAutoRange() {
			if(!EnableRangeAutoAdjust) { return; }
			if(chartContainer.Chart == null || chartContainer.Chart.Diagram == null || isAutoRange.HasValue) {
				return;
			}
			XYDiagram diagram = chartContainer.Chart.Diagram as XYDiagram;
			if(diagram != null) {
				isAutoRange = diagram.AxisX.VisualRange.Auto;
			}
			else {
				isAutoRange = false;
			}
		}
		protected void AdjustRange() {
			if(!EnableRangeAutoAdjust) { return; }
			UpdateAutoRange();
			if(isAutoRange.HasValue && isAutoRange.Value && ChartContainer.Chart.Diagram is XYDiagram) {
				XYDiagram diagram = (XYDiagram)chartContainer.Chart.Diagram;
				diagram.AxisX.WholeRange.Auto = true;
				diagram.AxisX.VisualRange.Auto = true;
			}
		}
		private void chartContainer_BoundDataChanged(object sender, EventArgs e) {
			AdjustRange();
		}
		protected virtual void DefaultInitialization(IChartDataSourceProvider dataSourceProvider) {
			chartContainer.Chart.Series.Clear();
			if(dataSourceProvider is ISeriesCreator) {
				chartContainer.Chart.Series.AddRange(((ISeriesCreator)(dataSourceProvider)).CreateSeries());
			}
			else if(dataSourceProvider is IChartDataSource) {
				chartContainer.Chart.DataContainer.SeriesDataMember = ((IChartDataSource)dataSourceProvider).SeriesDataMember;
				chartContainer.Chart.DataContainer.SeriesTemplate.ArgumentDataMember = ((IChartDataSource)dataSourceProvider).ArgumentDataMember;
				chartContainer.Chart.DataContainer.SeriesTemplate.ValueDataMembers[0] = ((IChartDataSource)dataSourceProvider).ValueDataMember;
				if(((IChartDataSource)dataSourceProvider).DateTimeArgumentMeasureUnit.HasValue) {
					chartContainer.Chart.DataContainer.SeriesTemplate.ArgumentScaleType = ScaleType.DateTime;
				}
				chartContainer.Chart.DataContainer.SeriesTemplate.Label.ResolveOverlappingMode = ResolveOverlappingMode.HideOverlapped;
			}
			LoadChartSettingsFromStorageProvider(EditValue as ISettingsProvider);
			chartContainer.Chart.DataContainer.DataSource = dataSourceProvider.DataSource;
			if(chartContainer.DataProvider != null)
				chartContainer.DataProvider.DataSource = dataSourceProvider.DataSource;
			if(dataSourceProvider is IChartTitleProvider) {
				chartContainer.Chart.Titles.Clear();
				#region Titles
				ChartTitle title = new ChartTitle();
				title.Text = ((IChartTitleProvider)dataSourceProvider).Title;
				title.Font = new System.Drawing.Font(title.Font.FontFamily, 12);
				chartContainer.Chart.Titles.AddRange(new DevExpress.XtraCharts.ChartTitle[] { title });
				#endregion
			}
			AdjustRange();
		}
		protected void ClearSettings() {
			ISettingsProvider settingsStorageProvider = EditValue as ISettingsProvider;
			if(settingsStorageProvider != null) {
				if(settingsStorageProvider is ChartSource) {
					((ChartSource)settingsStorageProvider).ResetSettings();
				}
				else {
					settingsStorageProvider.Settings = null;
				}
				OnDataSourceChanged();
			}
		}
		protected void SaveChartSettings() {
			ResetAutoRange();
			ISettingsProvider settingsStorageProvider = EditValue as ISettingsProvider;
			if(chartContainer != null && chartContainer.Chart != null && settingsStorageProvider != null) {
				MemoryStream ms = new MemoryStream();
				chartContainer.Chart.SaveLayout(ms);
				settingsStorageProvider.Settings = Encoding.UTF8.GetString(ms.ToArray());
			}
			AdjustRange();
		}
		public ChartControlContainer(IChartContainer chartContainer) {
			this.chartContainer = chartContainer;
			InitializeDefaultSettings();
			if(chartContainer.DataProvider != null)
				chartContainer.DataProvider.BoundDataChanged += new BoundDataChangedEventHandler(chartContainer_BoundDataChanged);
		}
		public IChartContainer ChartContainer {
			get { return chartContainer; }
		}
		public IChartDataSourceProvider EditValue {
			get { return dataSourceProvider; }
			set {
				if(!object.Equals(dataSourceProvider, value)) {
					if(dataSourceProvider != null) {
						dataSourceProvider.DataChanged -= new DataChangedEventHandler(dataSourceProvider_DataChanged);
					}
					dataSourceProvider = value;
					OnDataSourceChanged();
					if(dataSourceProvider != null) {
						dataSourceProvider.DataChanged += new DataChangedEventHandler(dataSourceProvider_DataChanged);
					}
				}
			}
		}
		public virtual object Control {
			get { return ChartContainer; }
		}
		public virtual void Dispose() {
			if(dataSourceProvider != null) {
				dataSourceProvider.DataChanged -= new DataChangedEventHandler(dataSourceProvider_DataChanged);
				dataSourceProvider = null;
			}
			if(chartContainer != null) {
				if(chartContainer.DataProvider != null)
					chartContainer.DataProvider.BoundDataChanged -= new BoundDataChangedEventHandler(chartContainer_BoundDataChanged);
				if(chartContainer is IDisposable) {
					((IDisposable)chartContainer).Dispose();
				}
				chartContainer = null;
			}
		}
#if DebugTest
		public void DebugTest_SaveSettings() {
			SaveChartSettings();
		}
#endif
	}
}
