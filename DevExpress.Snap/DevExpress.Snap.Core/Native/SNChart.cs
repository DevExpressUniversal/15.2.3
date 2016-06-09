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
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.Snap.Core.Native {
	public class SNChart : Chart{
		readonly IDataSourceDisplayNameProvider displayNameProvider;
		public static readonly object DefaultSeriesTag = new object();
		public SNChart(IChartContainer container)
			: base(container) {
			this.displayNameProvider = (IDataSourceDisplayNameProvider)Container.GetService(typeof(IDataSourceDisplayNameProvider));
		}
		public IDataSourceDisplayNameProvider DisplayNameProvider { get { return displayNameProvider; } }
		void SaveSeriesDataBindings() {
			foreach(Series series in DataContainer.Series)
				series.Tag = DisplayNameProvider.GetDataSourceName(series.DataSource);
		}
		public SeriesDataBindingList GetSeriesDataBindings() {
			SaveSeriesDataBindings();
			SeriesDataBindingList bindingList = new SeriesDataBindingList();
			for(int i = 0; i < DataContainer.Series.Count; i++)
				bindingList.Add((string)DataContainer.Series[i].Tag, i);
			return bindingList;
		}
		public string GetChartDataBindingName() {
			return DisplayNameProvider.GetDataSourceName(Container.DataProvider.DataSource);
		}
		public void SetSeriesDataBindings(SeriesDataBindingList bindings) {
			for(int i = 0; i < bindings.Count; i++) {
				SeriesDataBinding binding = bindings[i];
				if(binding.IsBound && binding.SeriesIndex < DataContainer.Series.Count)
					DataContainer.Series[binding.SeriesIndex].Tag = binding.DataSourceName;
			}
		}
		public void AssignSeriesDatasources(IDataSourceDispatcher dataSourceDispatcher) {
			foreach(Series series in DataContainer.Series) {
				if (series.Tag != null && series.Tag != DefaultSeriesTag)
					series.DataSource = dataSourceDispatcher.GetDataSource((string)series.Tag);
			}
		}
		public void AssignChartDataSource(IDataSourceDispatcher dataSourceDispatcher, string dataSourceName) {
			if(String.IsNullOrEmpty(dataSourceName))
				this.DataContainer.DataSource = dataSourceDispatcher.DefaultDataSource;
			else
				this.DataContainer.DataSource = dataSourceDispatcher.GetDataSource(dataSourceName);
		}
		bool OnlyDefaultSeriesIsPresent() {
			return DataContainer.Series.Count == 1 && DataContainer.Series[0].Tag == DefaultSeriesTag;
		}
		public void AddValues(SNDataInfo[] dataInfos) {
			string argumentDataMember = OnlyDefaultSeriesIsPresent() || DataContainer.Series.Count == 0 ? null : DataContainer.Series[0].ArgumentDataMember;
			DataContainer.Series.Clear();
			foreach(SNDataInfo info in dataInfos)
				AddNewSeries(argumentDataMember, info.Member, info.DisplayName, info.Source);
		}
		public void AddArgument(SNDataInfo dataInfo) {
			if(OnlyDefaultSeriesIsPresent() || DataContainer.Series.Count == 0) {
				DataContainer.Series.Clear();
				AddNewSeries(dataInfo.Member, null, null, dataInfo.Source);
			} else {
				foreach(Series series in DataContainer.Series)
					series.ArgumentDataMember = dataInfo.Member;
			}
		}
		void AddNewSeries(string argumentDataMember, string valueDataMember, string name, object dataSource) {
			Series series = new Series();
			((ISupportInitialize)series).BeginInit();
			if(valueDataMember != null) {
				series.ValueDataMembers.AddRange(valueDataMember);
				series.Name = name;
			}
			series.ArgumentDataMember = argumentDataMember;
			series.DataSource = dataSource;
			DataContainer.Series.Add(series);
			((ISupportInitialize)series).EndInit();
		}
		#region Chart data members getters
		public List<string> GetUniqueValues() {
			List<String> result = new List<string>();
			foreach (var series in DataContainer.Series.Cast<Series>())
				foreach (string value in series.ValueDataMembers)
					if (!string.IsNullOrEmpty(value))
						result.Add(GetDisplayName(series.DataSource, value));
			return result.Distinct().ToList();
		}
		public List<string> GetUniqueArguments() {
			List<string> result = new List<string>();
			foreach (var series in DataContainer.Series.Cast<Series>())
				if (!string.IsNullOrEmpty(series.ArgumentDataMember))
					result.Add(GetDisplayName(series.DataSource, series.ArgumentDataMember));
			return result.Distinct().ToList();
		}
		string GetDisplayName(object dataSource, string dataMember) {
			IRelatedDataBrowser dataBrowser = ChartContainer.DataProvider.DataContext.GetDataBrowser(dataSource, dataMember, true) as IRelatedDataBrowser;
			return (dataBrowser != null) ? dataBrowser.RelatedProperty.DisplayName : dataMember;
		}
		#endregion
	}
}
