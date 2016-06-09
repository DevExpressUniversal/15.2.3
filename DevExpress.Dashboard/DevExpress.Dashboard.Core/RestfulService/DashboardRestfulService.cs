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
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Service;
using DevExpress.Data;
using DevExpress.Utils.IoC;
using DevExpress.Utils;
using DevExpress.DashboardCommon.Server;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public static class DashboardDependencies {
		static readonly Dictionary<Type, Type> types = new Dictionary<Type, Type>();
		static readonly Dictionary<Type, Type> singletons = new Dictionary<Type, Type>();
		static DashboardDependencies() {
			types.Add(typeof(IDashboardItemFactory), typeof(DashboardFactory));
			types.Add(typeof(IDataSourceFactory), typeof(DashboardFactory));
			types.Add(typeof(IColorRepositoryFactory), typeof(DashboardFactory));
			singletons.Add(typeof(IDataServer), typeof(DataServer));
			singletons.Add(typeof(IColoringServer), typeof(ColoringServer));
		}
		public static void RegisterTypes(Action<Type, Type> registerAction) {
			RegisterTypes(types, registerAction);
		}
		public static void RegisterSingletons(Action<Type, Type> registerAction) {
			RegisterTypes(singletons, registerAction);
		}
		static void RegisterTypes(IDictionary<Type, Type> types, Action<Type, Type> registerAction) {
			foreach (KeyValuePair<Type, Type> pair in types)
				registerAction(pair.Key, pair.Value);
		}
	}
	public class DashboardRestfulService : IDashboardAdminService, IDashboardClientService {
		#region Statics
		static void ApplyItemState(DataDashboardItem dataDashboardItem, StateModel stateModel) {
			if(dataDashboardItem.ElementContainer != null) {
				dataDashboardItem.ElementContainer.SelectedElementIndex = stateModel.SelectedElementIndex;
			}
			IRaiseClusterizationRequestItem clusterizedItem = dataDashboardItem as IRaiseClusterizationRequestItem;
			if(clusterizedItem != null && stateModel.ClusterizationInfo != null) {
				clusterizedItem.UpdateMapInfo(stateModel.ClusterizationInfo);
			}
			DrillDownHelper helper = new DrillDownHelper(dataDashboardItem);
			helper.SetDrillDownState(stateModel);
			IPivotDashboardItem pivotItem = dataDashboardItem as IPivotDashboardItem;
			ExpandingModel expandingModel = stateModel.ExpandingModel;
			if(pivotItem != null && expandingModel != null) {
				pivotItem.ApplyExpandingState(expandingModel.PivotExpandCollapseState, expandingModel.Expand, expandingModel.IsColumn);
			}
			dataDashboardItem.ExternalFilter = GetExternalFilter(stateModel.Filters);
		}
		static DashboardPaneContent CreateItemPaneContent(DashboardItem item, HierarchicalDataParams itemDataParams, IActualParametersProvider provider, object[] partialContentParams) {
			if(item == null) return new DashboardPaneContent();
			DataDashboardItem dataDashboardItem = item as DataDashboardItem;
			HierarchicalItemData itemData = (dataDashboardItem == null || itemDataParams == null) ? null : new HierarchicalItemData {
				MetaData = dataDashboardItem.GetMetadata(provider),
				DataStorageDTO = itemDataParams.Storage.GetDTO(),
				SortOrderSlices = itemDataParams.SortOrderSlices
			};
			return new DashboardPaneContent {
				Name = item.ComponentName,
				Type = Helper.GetDashboardItemType(item),
				Group = item.GroupName,
				ContentType = partialContentParams != null ? ContentType.PartialDataSource : ContentType.FullContent,
				AxisNames = dataDashboardItem != null ? dataDashboardItem.GetAxisNames() : new string[0],
				DimensionIds = dataDashboardItem != null ? dataDashboardItem.GetDimensionIds() : new string[0],
				ViewModel = item.CreateViewModel(),
				CaptionViewModel = item.CreateCaptionViewModel(),
				ConditionalFormattingModel = item.CreateConditionalFormattingModel(),
				ItemData = itemData,
				Parameters = partialContentParams,
				ActionModel = null,
				DataSource = null,
				DataSourceMembers = null,
				SelectedValues = null,
				DrillDownValues = null,
				DrillDownUniqueValues = null
			};
		}
		static FilterCombinator GetExternalFilter(IEnumerable<ItemFilterModel> itemFilterModels) {
			if(itemFilterModels == null) return null;
			List<IFilter> externalFilters = new List<IFilter>();
			foreach(ItemFilterModel ifm in itemFilterModels) {
				OrderedDictionary<Dimension, IList<object>> valueStore = new OrderedDictionary<Dimension, IList<object>>();
				UnboundFilterValues unboundFilterValues = new UnboundFilterValues(valueStore, ifm.IsExcludingAllFilter);
				if(ifm.DimensionFilters != null) {
					foreach(DimensionFilterModel dfm in ifm.DimensionFilters) {
						Dimension dimension = dfm.Model.CreateInstance();
						if(dfm.Range != null) {
							unboundFilterValues.Ranges[dimension] = dfm.Range;
						} else {
							valueStore.Add(dimension, dfm.FilterValues);
						}
					}
				}
				externalFilters.Add(new ExternalFilter(unboundFilterValues));
			}
			return new FilterCombinator(externalFilters);
		}
		#endregion
		readonly IDashboardItemFactory dashboardItemFactory;
		readonly IDataSourceFactory dataSourceFactory;
		readonly IDataServer dataServer;
		readonly IColoringServer coloringServer;
		public DashboardRestfulService(IDashboardItemFactory dashboardItemFactory, IDataSourceFactory dataSourceFactory, IDataServer dataServer, IColoringServer coloringServer) {
			this.dashboardItemFactory = dashboardItemFactory;
			this.dataSourceFactory = dataSourceFactory;
			this.dataServer = dataServer;
			this.coloringServer = coloringServer;
		}
		IDashboardItemFactory GetDashboardItemFactory(XDocument dashboardDoc) {
			return dashboardDoc != null ? new OnSpotDashboardFactory(dashboardDoc) : dashboardItemFactory;
		}
		#region IDashboardClientService Members
		public DashboardPaneContent GetItemModel(string dashboardId, string itemId, StateModel stateModel, XDocument dashboardDoc = null) {
			DashboardItem dashboardItem = GetDashboardItemFactory(dashboardDoc).CreateDashboardItem(dashboardId, itemId);
			DataDashboardItem dataDashboardItem = dashboardItem as DataDashboardItem;
			if (dataDashboardItem != null && !string.IsNullOrEmpty(dataDashboardItem.DataSourceName)) {
				IEnumerable<IParameter> parameters = ParameterInfoWrapper.Wrap(stateModel.Parameters);
				ServerResult<DataServerSession> dataServerResult = dataServer.GetSession(dashboardId, dataDashboardItem.DataSourceName, dataDashboardItem.DataMember, parameters);
				dataDashboardItem.DataSource = dataServerResult.Session.DataSource; 
				ApplyItemState(dataDashboardItem, stateModel);
				DataSessionProvider dataSessionProvider = new DataSessionProvider(parameters);
				SliceDataQuery sliceDataQuery = ((ISliceDataQueryProvider)dataDashboardItem).GetDataQuery(dataSessionProvider);
				DataQueryResult dataQueryResult = dataServerResult.Session.GetItemData(sliceDataQuery, dataSessionProvider);
				DataSourceProvider dataSourceProvider = new DataSourceProvider(dataServer, dashboardId, parameters);
				ColorRepository coloringCache = coloringServer.GetColoringCache(dashboardId, dataDashboardItem, dataSourceProvider, dataSessionProvider);
				IDictionary<string, object> filter = null;
				IPivotDashboardItem pivotItem = dataDashboardItem as IPivotDashboardItem;
				ExpandingModel expandingModel = stateModel.ExpandingModel;
				object[] partialContentParams = null;
				if(pivotItem != null && expandingModel != null && expandingModel.Expand != null) {
					filter = pivotItem.GetExpandFilter(expandingModel.IsColumn, expandingModel.Expand, true);
					partialContentParams = new object[] { expandingModel.Expand, expandingModel.IsColumn };
				}
				MultidimensionalDataDTO multiDataDTO = ((ISliceDataQueryProvider)dataDashboardItem).PrepareData(dataQueryResult, sliceDataQuery, dataSessionProvider, coloringCache, filter);
				return CreateItemPaneContent(dashboardItem, multiDataDTO.HierarchicalDataParams, dataSessionProvider, partialContentParams);
			}
			else
				return CreateItemPaneContent(dashboardItem, null, null, null);
		}
		public DataSourceNodeBase GetFieldList(string dashboardId, string dataSourceName, string dataMember) {
			var parameters = new List<IParameter>(); 
			ServerResult<DataServerSession> dataServerResult = dataServer.GetSession(dashboardId, dataSourceName, dataMember, parameters);
			IDashboardDataSource dataSource = dataServerResult.Session.DataSource;
			if (dataSource != null) {
				return dataSource.GetDataSourceSchema(dataMember).RootNode;
			} else
				throw new Exception(string.Format("DataSource '{0}' does not found in dashboard '{1}'", dataSourceName, dashboardId));
		}
		public List<ViewModel.ParameterValueViewModel> GetParameterValues(string dashboardId, string dataSourceName, string dataMember, string valueMember, string displayMember) {
			var parameters = new List<IParameter>(); 
			ServerResult<DataServerSession> dataServerResult = dataServer.GetSession(dashboardId, dataSourceName, dataMember, parameters);
			IDashboardDataSource dataSource = dataServerResult.Session.DataSource;
			if (dataSource != null) {
				return dataSource.GetParameterValues(valueMember, displayMember, dataMember, new ParametersProvider(parameters));
			} else
				throw new Exception(string.Format("DataSource '{0}' does not found in dashboard '{1}'", dataSourceName, dashboardId));
		}
		#endregion
	}
}
