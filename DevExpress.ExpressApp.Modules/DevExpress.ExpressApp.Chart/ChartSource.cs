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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.ChartDataSources;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraCharts;
namespace DevExpress.ExpressApp.Chart {
	public class ChartSource : IChartDataSourceProvider, ISettingsProvider, ICustomizationEnabledProvider, ISeriesCreator, IDisposable {
		IModelListView listViewModel;
		IList chartDataSource;
		private void bindingList_ListChanged(object sender, ListChangedEventArgs e) {
			RaiseDataChanged();
		}
		private void RaiseDataChanged() {
			if(DataChanged != null) {
				DataChanged(chartDataSource, new DataChangedEventArgs(DataChangedType.ItemsChanged));
			}
		}
		private string GetArgumentPropertyName() {
			string argumentPropertyName = GetGroupPropertyName();
			if(!string.IsNullOrEmpty(argumentPropertyName)) {
				return argumentPropertyName;
			}
			else {
				IList<IModelColumn> visibleColumns = listViewModel.Columns.GetVisibleColumns();
				if(visibleColumns.Count > 0) {
					return visibleColumns[0].PropertyName;
				}
			}
			return null;
		}
		private string GetValuePropertyName() {
			foreach(IModelColumn visibleColumn in listViewModel.Columns.GetVisibleColumns()) {
				if(visibleColumn.Summary.Count > 0) {
					return visibleColumn.PropertyName;
				}
			}
			return null;
		}
		private string GetGroupPropertyName() {
			int groupIndex = int.MaxValue;
			string propertyName = null; ;
			foreach(IModelColumn column in listViewModel.Columns) {
				if(column.GroupIndex >= 0 && groupIndex > column.GroupIndex) {
					groupIndex = column.GroupIndex;
					propertyName = column.PropertyName;
				}
			}
			if(groupIndex != int.MaxValue) {
				return propertyName;
			}
			return null;
		}
		private string ConvertGridSummary(string propertyName) {
			if(listViewModel.Columns[propertyName].Summary.Count >0 && listViewModel.Columns[propertyName].Summary[0].SummaryType != SummaryType.Custom) {
				if(listViewModel.Columns[propertyName].Summary[0].SummaryType == SummaryType.Count) {
					return "COUNT()";
				}
				else {
					return listViewModel.Columns[propertyName].Summary[0].SummaryType.ToString().ToUpper() + "([" + propertyName + "])";
				}
			}
			return "COUNT()";
		}
		public ChartSource(IList dataSource, IModelListView listViewModel) {
			this.chartDataSource = dataSource;
			IBindingList bindingList = dataSource as IBindingList;
			if(bindingList != null) {
				bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
			}
			this.listViewModel = listViewModel;
		}
		public object DataSource {
			get { return chartDataSource; }
		}
		public string Settings {
			get { return ((IModelChartListView)listViewModel).ChartSettings.Settings; }
			set { ((IModelChartListView)listViewModel).ChartSettings.Settings = value; }
		}
		public void ResetSettings() {
			((DevExpress.ExpressApp.Model.Core.ModelNode)(((IModelChartListView)listViewModel).ChartSettings)).ClearValue("Settings");
		}
		public bool CustomizationEnabled {
			get { return ((IModelChartListView)listViewModel).ChartSettings.CustomizationEnabled; }
			set { ((IModelChartListView)listViewModel).ChartSettings.CustomizationEnabled = value; }
		}
		public event DataChangedEventHandler DataChanged;
		public Series[] CreateSeries() {
			string argumentPropertyName = GetArgumentPropertyName();
			string valuePropertyName = GetValuePropertyName();
			DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series(argumentPropertyName, DevExpress.XtraCharts.ViewType.Bar);
			series.ArgumentDataMember = argumentPropertyName;
			if(!string.IsNullOrEmpty(valuePropertyName)) {
				series.ValueDataMembers.AddRange(new string[] { valuePropertyName });
				series.SummaryFunction = ConvertGridSummary(valuePropertyName);
			}
			else {
				series.SummaryFunction = "COUNT()";
			}
			return new Series[] { series };
		}
		public void Dispose() {
			if(chartDataSource != null) {
				IBindingList bindingList = chartDataSource as IBindingList;
				if(bindingList != null) {
					bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
				}
				chartDataSource = null;
			}
		}
#if DebugTest
		public void DebugTest_RaiseDataChanged() {
			RaiseDataChanged();
		}
		public string DebugTest_GetArgumentPropertyName() {
			return GetArgumentPropertyName();
		}
		public string DebugTest_GetValuePropertyName() {
			return GetValuePropertyName();
		}
		public string DebugTest_GetGroupPropertyName() {
			return GetGroupPropertyName();
		}
		public string DebugTest_ConvertGridSummary(string propertyName) {
			return ConvertGridSummary(propertyName);
		}
#endif
	}
}
