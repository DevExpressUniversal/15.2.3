#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Threading;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardCommon.Server {
	public class DataManager : IDisposable {
		public static DataSourceModel CreateDataSourceModel(DataSourceInfo dataSourceInfo, IList fakeDataSource) {
			IDashboardDataSource dataSource = dataSourceInfo.DataSource;
			string dataMember = dataSourceInfo.DataMember;
			bool fakeData = fakeDataSource != null;
			DataSourceModel dataSourceModel = new DataSourceModel(dataSource, dataMember, fakeData ? fakeDataSource : dataSource.GetListSource(dataMember), dataSource.GetPivotDataSource(dataMember));
			dataSourceModel.FakeData = fakeData;
			return dataSourceModel;
		}
		readonly IDataSessionFactory dataSessionFactory;
		readonly Dashboard dashboard;
		readonly Dictionary<string, IDataSession> dataSessions = new Dictionary<string, IDataSession>();
		public bool InServiceOperation { get; set; }
		public bool IsDesignMode { get; set; }
		bool AllowDataCalculation { get { return (dashboard.EnableAutomaticUpdates || !IsDesignMode); } }
		public DataManager(IDataSessionFactory dataSessionFactory, Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			Guard.ArgumentNotNull(dataSessionFactory, "dataSessionFactory");
			this.dataSessionFactory = dataSessionFactory;
			this.dashboard = dashboard;
			dashboard.DashboardItemChanged += OnDashboardItemChanged;
			dashboard.ItemCollectionChanged += OnItemCollectionChanged;
			dashboard.DataSourceDataChanged += OnDashboardDataSourceDataChanged;
			dashboard.DashboardItemComponentNameChanged += OnDashboardItemComponentNameChanged;
		}
		public void Dispose() {
			dashboard.DashboardItemChanged -= OnDashboardItemChanged;
			dashboard.ItemCollectionChanged -= OnItemCollectionChanged;
			dashboard.DataSourceDataChanged -= OnDashboardDataSourceDataChanged;
			dashboard.DashboardItemComponentNameChanged -= OnDashboardItemComponentNameChanged;
			foreach(IDataSession dataSession in dataSessions.Values)
				dataSession.Dispose();
			dataSessions.Clear();
		}
		public DashboardUnderlyingDataSet CalculateUnderlyingDataSet(string itemName, DataSourceInfo dataSourceInfo, IList fakeDataSource, UnderlyingDataQuery<SliceDataQuery> query) {
			return GetDataSession(itemName, dataSourceInfo, fakeDataSource).GetUnderlyingData(query);
		}
		public DashboardClientData CalculateClientData(IEnumerable<string> itemsNames, IActualParametersProvider parameters, IColoringCacheProvider coloringCacheProvider, CancellationToken token, IDictionary<string, IDictionary<string, object>> expandFilter, IEnumerable<string> itemsToIgnore, Action<string, MultiDimensionalDataProvider> updateSelectionCallback) {
			IDictionary<string, MultidimensionalDataDTO> data = dashboard.InteractivityOrderedItems
				.OfType<DataDashboardItem>()
				.Where(item => itemsNames.Contains(item.ComponentName))
				.ToDictionary(item => item.ComponentName,
					item => GetData(item, parameters, coloringCacheProvider, token, expandFilter, itemsToIgnore, updateSelectionCallback));
			return new DashboardClientData(data);
		}
		MultidimensionalDataDTO GetData(DevExpress.DashboardCommon.DataDashboardItem item, IActualParametersProvider parameters, IColoringCacheProvider coloringCacheProvider, CancellationToken token, IDictionary<string, IDictionary<string, object>> expandFilter, IEnumerable<string> itemsToIgnore, Action<string, MultiDimensionalDataProvider> updateSelectionCallback) {
			DataQueryResult queryResult = DataQueryResult.Empty;
			ColorRepository coloringCache = null;
			SliceDataQuery query = ((ISliceDataQueryProvider)item).GetDataQuery(parameters);
			if(itemsToIgnore == null || !itemsToIgnore.Contains(item.ComponentName)) {
				string itemType = Helper.GetDashboardItemType(item);
				if(query != null) {
					IDataSession session = GetDataSession(item.ComponentName, new DataSourceInfo(item.DataSource, item.DataMember), item.FakeDataSource);
					queryResult = session.GetData(query, token);
				}
				coloringCache = coloringCacheProvider != null && AllowDataCalculation ? coloringCacheProvider.GetActualColoringCache(item.ComponentName) : null;
			}
			IDictionary<string, object> filter = expandFilter == null ? null : expandFilter[item.ComponentName];
			MultidimensionalDataDTO dataDTO = PrepareData(item, query, parameters, coloringCache, token, filter, queryResult);
			if (updateSelectionCallback != null)
				updateSelectionCallback(item.ComponentName, new MultiDimensionalDataProvider(item, dataDTO, parameters));
			return dataDTO;
		}
		MultidimensionalDataDTO PrepareData(ISliceDataQueryProvider item, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, CancellationToken token, IDictionary<string, object> expandFilter, DataQueryResult queryResult) {						
			DataStorage storage = queryResult.HierarchicalData.Storage;
			storage = expandFilter != null ? DataDashboardItem.GetFilteredStorage(expandFilter, storage, true) : storage;
			queryResult.HierarchicalData.Storage = storage;
			return item.PrepareData(queryResult, query, parameters, coloringCache, expandFilter);
		}
		public IDataSession GetDataSession(string itemName, DataSourceInfo dataSourceInfo, IList fakeDataSource) {
			if(string.IsNullOrEmpty(itemName) || fakeDataSource != null)
				return DataSessionFactory.Default.RequestSession(CreateDataSourceModel(dataSourceInfo, fakeDataSource));
			else {
				IDataSession session;
				if(!dataSessions.TryGetValue(itemName, out session)) {
					session = dataSessionFactory.RequestSession(CreateDataSourceModel(dataSourceInfo, fakeDataSource));
					dataSessions.Add(itemName, session);
				}
				return session;
			}
		}
		void RemoveDataSessions(IEnumerable<string> itemNames) {
			foreach(var itemName in itemNames) {
				IDataSession dataSession;
				if(dataSessions.TryGetValue(itemName, out dataSession)) {
					dataSession.Dispose();
					dataSessions.Remove(itemName);
				}
			}
		}
		void OnDashboardItemChanged(object sender, DashboardItemChangedEventArgs e) {
			if(!InServiceOperation) {
				if(e.InnerArgs.Reason == ChangeReason.RawData)
					if(e.DashboardItem is ISliceDataQueryProvider)
						RemoveDataSessions(new[] { e.DashboardItem.ComponentName });
			}
		}
		void OnItemCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItem> e) {
			RemoveDataSessions(e.RemovedItems.OfType<DataDashboardItem>().Select(item => item.ComponentName));
		}
		void OnDashboardDataSourceDataChanged(object sender, DataSourceChangedEventArgs e) {
			RemoveDataSessions(dashboard.Items.OfType<DataDashboardItem>().Where(item => item.DataSource == e.DataSource).Select(item => item.ComponentName));
		}
		void OnDashboardItemComponentNameChanged(object sender, ComponentNameChangedEventArgs e) {
			IDataSession session;
			if (dataSessions.TryGetValue(e.OldComponentName, out session)) {
				dataSessions.Remove(e.OldComponentName);
				dataSessions.Add(e.NewComponentName, session);
			}
		}
	}
}
