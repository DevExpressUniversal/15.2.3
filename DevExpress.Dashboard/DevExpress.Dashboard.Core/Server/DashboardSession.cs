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
using System.Drawing;
using System.Linq;
using System.Threading;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Layout;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
#if !DXPORTABLE
using DevExpress.DataAccess.Wizard.Native;
#endif
using DevExpress.Compatibility.System.Drawing;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Excel;
namespace DevExpress.DashboardCommon.Server {
	public sealed class DashboardSession : IDisposable, IDataConnectionParametersProvider, IErrorHandler, IColoringCacheProvider, IDataSessionProvider, IActualParametersProvider, 
		IDashboardCustomSqlQueryValidator {
		public static DashboardClientData PerformCalculateClientDataAction(Dashboard dashboard, IWaitFormActivator waitFormActivator, Func<CancellationToken, DashboardClientData> action, out bool canceled) {
			try {
				waitFormActivator.ShowWaitForm(true, false, true);
				CancellationToken token = CancellationToken.None;
#if !DXPORTABLE
				CancellationTokenSource cts = new CancellationTokenSource();
				CancellationTokenHook hook = new CancellationTokenHook(cts);
				waitFormActivator.SetWaitFormObject(hook);
				waitFormActivator.SetWaitFormCaption(Localization.DashboardLocalizer.GetString(Localization.DashboardStringId.DashboardDataUpdating));
				waitFormActivator.EnableCancelButton(true);
				token = cts.Token;
#endif
				canceled = false;
				token.Register((d) => {
					((Dashboard)d).EnableAutomaticUpdates = false;				
				},
				  dashboard);
				DashboardClientData clientData = action(token);
				canceled = token.IsCancellationRequested;
				return clientData;
			} finally {
				waitFormActivator.CloseWaitForm();
			}
		}
		readonly object syncObject = new object();
		readonly RefreshManager refreshManager = new RefreshManager();
		readonly InteractivityManager interactivityManager = new InteractivityManager();
		ColorRepository dashboardColoringCache;
		string dashboardId;
		Dashboard dashboard;
		List<DataLoaderError> dataLoaderErrors;
		IEnumerable<DashboardParameterInfo> dashboardParameters;
		string dataVersion;
		SessionSettings settings;
		Hashtable clientState;
		DataManager dataManager;
		public object SyncObject { get { return syncObject; } }
		public string DashboardId { get { return dashboardId; } }
		public bool IsEmpty { get { return dashboard == null; } }
		public List<DataLoaderError> DataLoaderErrors { get { return dataLoaderErrors; } }
		public string DataVersion { get { return dataVersion; } }
		public Hashtable ClientState {
			get { return clientState; }
			set {
				clientState = value;
				OnUpdateClientState();
			}
		}
		public bool EnableAutomaticUpdates { get { return dashboard.EnableAutomaticUpdates; } }
		public RefreshManager RefreshManager { get { return refreshManager; } }
		public DataManager DataManager { get { return dataManager; } }
		public event EventHandler<SingleFilterDefaultValueEventArgs> SingleFilterDefaultValue;
		public event EventHandler<FilterElementDefaultValuesEventArgs> FilterElementDefaultValues;
		public event EventHandler<RangeFilterDefaultValueEventArgs> RangeFilterDefaultValue;
		public event EventHandler<DataLoadingServerEventArgs> DataLoading;
		public event EventHandler<ConfigureDataConnectionServerEventArgs> ConfigureDataConnection;
		public event EventHandler<CustomFilterExpressionServerEventArgs> CustomFilterExpression;
		public event EventHandler<CustomParametersServerEventArgs> CustomParameters;
		public event EventHandler<DashboardLoadingServerEventArgs> DashboardLoading;
		public event EventHandler<DashboardLoadedServerEventArgs> DashboardLoaded;
		public event EventHandler<ConnectionErrorServerEventArgs> ConnectionError;
		public event EventHandler<AllowLoadUnusedDataSourcesServerEventArgs> AllowLoadUnusedDataSources;
		public event EventHandler<DashboardUnloadingEventArgs> DashboardUnloading;
		public event EventHandler<RequestCustomizationServicesEventArgs> RequestCustomizationServices;
		public event EventHandler<RequestWaitFormActivatorEventArgs> RequestWaitFormActivator;
		public event EventHandler<RequestErrorHandlerEventArgs> RequestErrorHandler;
		public event EventHandler<RequestUnderlyingDataFormatEventArgs> RequestUnderlyingDataFormat;
		public event EventHandler<RequestDataLoaderEventArgs> RequestDataLoader;
		public event EventHandler<RequestAppConfigPatcherServiceEventArgs> RequestAppConfigPatcherService;
		public event EventHandler<ValidateDashboardCustomSqlQueryEventArgs> ValidateCustomSqlQuery;
		public DashboardSession() {
			this.dataVersion = String.Empty;
			interactivityManager.SingleFilterDefaultValue += (s, e) => {
				if (SingleFilterDefaultValue != null)
					SingleFilterDefaultValue(s, e);
			};
			interactivityManager.FilterElementDefaultValues += (s, e) => {
				if (FilterElementDefaultValues != null)
					FilterElementDefaultValues(s, e);
			};
			interactivityManager.RangeFilterDefaultValue += (s, e) => {
				if (RangeFilterDefaultValue != null)
					RangeFilterDefaultValue(s, e);
			};
		}
		public void Dispose() {
			lock(syncObject) {
				if(dashboard != null) {
					dashboard.RequestMasterFilterParameters -= OnRequestMasterFilterParameters;
					dashboard.RequestDrillDownParameters -= OnRequestDrillDownParameters;
					dashboard.RequestDrillDownController -= OnRequestDrillDownController;
					if(dataManager != null) {
						dataManager.Dispose();
						dataManager = null;
					}
					if(DashboardUnloading != null)
						DashboardUnloading(this, new DashboardUnloadingEventArgs(dashboard));
					dashboard = null;
				}
				refreshManager.Dispose();
				interactivityManager.Dispose();
			}
		}
		public DashboardState GetDashboardState() {
			DashboardState state = new DashboardState();
			foreach (DashboardItem dashboardItem in dashboard.Items) {
				DashboardItemState itemState = new DashboardItemState();
				dashboardItem.PrepareState(itemState);
				IMasterFilterController mfController = GetMasterFilterController(dashboardItem.ComponentName);
				if (mfController != null)
					mfController.PrepareMasterFilterState(itemState);
				IDrillDownController ddController = GetDrillDownController(dashboardItem.ComponentName);
				if (ddController != null)
					ddController.PrepareDrillDownState(itemState);
				state.SetDashboardItemState(dashboardItem, itemState);
			}
			return state;
		}
		public void SetDashboardState(DashboardState state) {
			if (state != null) {
				foreach (DashboardItem dashboardItem in dashboard.Items) {
					DashboardItemState itemState = state.GetDashboardItemState(dashboardItem) ?? new DashboardItemState();
					dashboardItem.SetState(itemState);
					IMasterFilterController mfController = GetMasterFilterController(dashboardItem.ComponentName);
					if (mfController != null) {
						mfController.SetMasterFilterState(itemState);
						IMasterFilterStateValidator mfValidator = mfController.Validator; 
						if (mfValidator != null)
							mfValidator.ValidateMasterFilterState(dashboard, this, this);
					}					
					IDrillDownController ddController = GetDrillDownController(dashboardItem.ComponentName);
					if (ddController != null)
						ddController.SetDrillDownState(itemState);
				}
			}
		}
		public void DisableAutomaticUpdates() {
			dashboard.EnableAutomaticUpdates = false;
		}
		public DashboardSessionState GetState() {
			DashboardState state = GetDashboardState();
			return new DashboardSessionState {
				DashboardState = state,
				DashboardParameters = dashboardParameters,
				DataVersion = dataVersion,
				DashboardId = dashboardId,
				SessionSettings = settings
			};
		}
		public IList<DashboardItem> GetDashboardItems() {
			return dashboard.GetDashboardItems();
		}
		public IList<string> GetItemNames() {
			return dashboard.Items.Select(item => item.ComponentName).ToArray();
		}
		public IList<string> GetGroupNames() {
			return dashboard.Groups.Select(group => group.ComponentName).ToArray();
		}
		public IList<string> GetDataBoundItemNames() {
			return dashboard.Items.Where(item => item is DataDashboardItem).Select(item => item.ComponentName).ToArray();
		}
		public IList<string> GetDataSourceNames() {
			return dashboard.DataSources.Select(dataSource => dataSource.ComponentName).ToArray();
		}
		public DashboardPane GetRootPane() {
			int clientWidth = 0;
			int clientHeight = 0;
			if(ClientState != null) {
				Hashtable defaultState = ClientState[""] as Hashtable;
				if(defaultState != null) {
					clientWidth = Helper.ConvertToInt32(defaultState["ClientWidth"]);
					clientHeight = Helper.ConvertToInt32(defaultState["ClientHeight"]);
				}
			}
			if(clientWidth <= 0 || clientHeight <= 0)
				clientWidth = clientHeight = 1;
			DashboardLayoutGroup ensuredLayout = new DashboardLayoutCreator(clientWidth, clientHeight, dashboard.LayoutRoot, dashboard.Items, dashboard.Groups).LayoutRoot;
			return DashboardLayoutConverter.CreatePanes(ensuredLayout);
		}
		public string GetItemGroup(string itemName) {
			if(dashboard.Items.ContainsName(itemName)) {
				DashboardItemGroup itemGroup = dashboard.Items[itemName].Group;
				return itemGroup != null ? itemGroup.ComponentName : null;
			}
			return null;
		}
		public DashboardItemViewModel GetViewModel(string itemName) {
			return dashboard.CreateViewModel(itemName);
		}
		public DashboardItemCaptionViewModel GetCaptionViewModel(string itemName) {
			return dashboard.CreateCaptionViewModel(itemName);
		}
		public ConditionalFormattingModel GetConditionalFormattingModel(string itemName) {
			return dashboard.CreateConditionalFormattingModel(itemName);
		}
		public DashboardTitleViewModel GetTitleViewModel() {
			return dashboard.CreateTitleViewModel();
		}
		public TitleLayoutViewModel GetTitleLayoutViewModel() {
			return dashboard.Title.CreateTitleLayoutViewModel();
		}
		public ExportData GetExportData() {
			ExportData exportData = new ExportData {
				ExportTitle = dashboard.Title.GetExportTitle(),
				ExportCaptions = new Dictionary<string, string>()
			};
			foreach(DashboardItem dashboardItem in dashboard.Items)
				exportData.ExportCaptions.Add(dashboardItem.ComponentName, dashboardItem.ElementCaption);
			foreach(DashboardItemGroup dashboardItemGroup in dashboard.Groups)
				exportData.ExportCaptions.Add(dashboardItemGroup.ComponentName, dashboardItemGroup.ElementCaption);
			return exportData;
		}
		public IList<DashboardParameterViewModel> GetParametersViewModel() {
			return dashboard.GetParameterViewModels(this);
		}
		public string[] GetAxisName(string itemName) {
			DataDashboardItem dataDashboardItem = dashboard.Items[itemName] as DataDashboardItem;
			if (dataDashboardItem != null)
				return dataDashboardItem.GetAxisNames();
			return new string[0];
		}
		public string[] GetDimensionIds(string itemName) {
			DataDashboardItem dataDashboardItem = dashboard.Items[itemName] as DataDashboardItem;
			if (dataDashboardItem != null)
				return dataDashboardItem.GetDimensionIds();
			return new string[0];
		}
		public ButtonState GetDrillUpButtonState(string itemName) {
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null && controller.IsDrillDownEnabled) {
				IDrillDownParameters parameters = GetDrillDownParameters(itemName);
				if(parameters != null)
					return parameters.CurrentDrillDownLevel > 0 ? ButtonState.Enabled : ButtonState.Disabled;
			}
			return ButtonState.Hidden;
		}
		public ButtonState GetClearMasterFilterButtonState(string itemName) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if (controller != null && controller.IsClearMasterFilterSupported)
				return controller.CanClearMasterFilter ? ButtonState.Enabled : ButtonState.Disabled; ;
			return ButtonState.Hidden;
		}
		public IList<string> GetSetMasterFilterAffectedItems(string itemName) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if (controller != null && controller.CanSetMasterFilter)
				return controller.AffectedItems;
			return null;
		}
		public IList<string> GetSetMultipleMasterFilterAffectedItems(string itemName) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if (controller != null && controller.CanSetMasterFilter && controller.IsMultipleMasterFilterEnabled)
				return controller.AffectedItems;
			return null;
		}
		public IList<string> GetClearMasterFilterAffectedItems(string itemName) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if (controller != null && controller.CanClearMasterFilter)
				return controller.AffectedItems;
			return null;
		}
		public IList<string> GetDrillDownAffectedItems(string itemName) {
			IList<string> affectedItems = null;
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null && controller.CanPerformDrillDown) {
				affectedItems = new List<string> { itemName };
				IMasterFilterController mfController = GetMasterFilterController(itemName);
				if(mfController != null && mfController.IsSingleMasterFilterEnabled)
					affectedItems.AddRange(mfController.AffectedItems);
			}
			return affectedItems != null && affectedItems.Count > 0 ? affectedItems : null;
		}
		public IList<string> GetDrillDownOnSelectedValueAffectedItems(string itemName) {
			IList<string> affectedItems = null;
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null && controller.CanPerformDrillDown)
				affectedItems = new string[] { itemName };
			return affectedItems != null && affectedItems.Count > 0 ? affectedItems : null;
		}
		public IList<string> GetDrillUpAffectedItems(string itemName) {
			IList<string> affectedItems = null;
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null && controller.CanPerformDrillUp) {
				affectedItems = new List<string> { itemName };
				IMasterFilterController mfController = GetMasterFilterController(itemName);
				if(mfController != null && mfController.IsSingleMasterFilterEnabled)
					affectedItems.AddRange(mfController.AffectedItems);
			}
			return affectedItems != null && affectedItems.Count > 0 ? affectedItems : null;
		}
		public IList<string> GetSetSelectedElementIndexAffectedItems(string itemName) {
			DashboardItem dashboardItem = dashboard.GetDashboardItem(itemName);
			IElementContainer elementContainer = dashboardItem.ElementContainer;
			if(elementContainer != null && elementContainer.ElementSelectionEnabled)
				return new string[] { dashboardItem.ComponentName };
			return null;
		}
		public IList<string> GetExpandValueAffectedItems(string itemName) {
			DashboardItem dashboardItem = dashboard.GetDashboardItem(itemName);
			if(dashboardItem is IPivotDashboardItem)
				return new string[] { dashboardItem.ComponentName };
			return null;
		}
		public IEnumerable<DimensionFilterValues> GetMasterFilterValues(bool truncate) {
			return GetMasterFilterValues(null, truncate);
		}
		public IEnumerable<DimensionFilterValues> GetMasterFilterValues(string itemName, bool truncate) {
			IFiltersProvider provider = GetMasterFilterItemsProvider(itemName);
			IEnumerable<DimensionFilterValues> dimensionFilterValues = provider.FilterItemNames.SelectMany(name => GetFilterValues(name));
			return MasterFilterValuesPresentationHelper.GetUniqueMasterFilterValues(dimensionFilterValues, truncate);
		}
		public IList<DimensionFilterValues> GetDrillDownValues(string itemName) {
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null)
				return controller.DrillDownValues;
			return null;
		}
		public IList<object> GetDrillDownUniqueValues(string itemName) {
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null)
				return controller.DrillDownUniqueValues;
			return null;
		}
		public string GetItemType(string itemName) {
			DashboardItem dashboardItem = dashboard.GetDashboardItem(itemName);
			return Helper.GetDashboardItemType(dashboardItem);
		}
		public HierarchicalMetadata GetMetadata(string itemName) {
			DashboardItem dashboardItem = dashboard.Items[itemName];
			DataDashboardItem dataDashboardItem = dashboardItem as DataDashboardItem;
			if(dataDashboardItem != null)
				return dataDashboardItem.GetMetadata(this);
			return null;
		}
		public IValuesSet GetSelectedValues(string itemName) {
			IValuesSet selectedValues = null;
			ISelectionController selectionController = GetSelectionController(itemName);
			if (selectionController != null)
				selectedValues = selectionController.Selection;
			return selectedValues ?? ValuesSetHelper.EmptyValuesSet();
		}
		public void SetMasterFilter(string itemName, IValuesSet values) {
			SetMasterFilterInternal(itemName, values);
		}
		public void SetMultipleValuesMasterFilter(string itemName, IValuesSet values) {
			SetMasterFilterInternal(itemName, values);
		}
		public void PerformDrillDown(string itemName, object value) {
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null) {
				if(controller.IsDrillDownAvailable(value))
					controller.PerformDrillDown(value); 
				else
					throw new InvalidOperationException("The master filtering should be applied before the drill-down is performed.");
			}
		}
		public void PerformDrillUp(string itemName) {
			IDrillDownController controller = GetDrillDownController(itemName);
			if (controller != null) {
				if(controller.IsDrillUpAvailable())
					controller.PerformDrillUp();
				else
					throw new InvalidOperationException("The master filtering should be reset before the drill-up is performed.");
			}
		}
		public void ClearMasterFilter(string itemName) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if (controller != null)
				controller.ClearMasterFilter();
		}
		public void SetSelectedElementIndex(string itemName, int index) {
			dashboard.Items[itemName].ElementContainer.SelectedElementIndex = index;
		}
		public void SetDataVersion(string dataVersion) {
			this.dataVersion = dataVersion;
		}
		public bool ReloadData(ReloadDataArgs args) {
			return ReloadData(null, args);
		}
		public bool ReloadData(IEnumerable<string> dataSourceComponentNames, ReloadDataArgs args) {
			if(args != null)
				dashboardParameters = args.Parameters;
			dataVersion = Guid.NewGuid().ToString();
			DataSourceLoadingResultType result = LoadData(dataSourceComponentNames, args);			
			ResetDashboardColoringCache(null);
			return result != DataSourceLoadingResultType.Aborted;
		}
		public void ApplyExternalDataSourceSchema(IEnumerable<string> dataSourceComponentNames) {
			IList<IDashboardDataSource> dataSources = PrepareDataSourceList(dataSourceComponentNames);
			if(dataSources.Count > 0)
				ApplyExternalDataSourceSchema(dataSources);
		}
		public bool EqualsParameters(IEnumerable<DashboardParameterInfo> parameters) {
			IEnumerable<IParameter> currentParameters = GetParameters();
			int currentCount = currentParameters != null ? currentParameters.Count() : 0;
			int applyingCount = parameters != null ? parameters.Count() : 0;
			if(applyingCount != currentCount)
				return false;
			foreach(DashboardParameterInfo newParameter in parameters) {
				IParameter par = currentParameters.Where(p => p.Name == newParameter.Name).FirstOrDefault();
				if(par == null || !object.Equals(par.Value, newParameter.Value))
					return false;
			}
			return true;
		}
		public MultidimensionalDataDTO ExpandValue(string itemName, bool isColumn, object[] values, bool expanded, bool isRequestData, bool forceFullData) {
			IPivotDashboardItem pivotItem = (IPivotDashboardItem)dashboard.Items[itemName];
			bool success = pivotItem.ChangeValueExpandState(isColumn, values, expanded);
			if(isRequestData && success) {
				IDictionary<string, IDictionary<string, object>> expandfilter =
					forceFullData ? null :
					new Dictionary<string, IDictionary<string, object>> {
						{
							itemName,
							pivotItem.GetExpandFilter(isColumn, values, false)
						}
					};
				DashboardClientData data = CalculateClientData(new string[] { itemName }, expandfilter);
				return data[itemName];
			} else {
				return null;
			}
		}
		public void ProcessDataRequest(string itemName) {
			IRaiseClusterizationRequestItem geoPointItem = dashboard.Items[itemName] as IRaiseClusterizationRequestItem;
			if(geoPointItem != null)
				geoPointItem.RaiseClusterizationRequest();
		}
		public void PrepareAction(string itemName, Service.ActionType actionType) {
			Service.ActionType[] masterFilterTypes = new[] { Service.ActionType.SetMasterFilter, Service.ActionType.SetMultipleValuesMasterFilter, Service.ActionType.ClearMasterFilter };
			if(masterFilterTypes.Contains(actionType)) {
				IRaiseClusterizationRequestItem geoPointItem = dashboard.Items[itemName] as IRaiseClusterizationRequestItem;
				if(geoPointItem != null)
					geoPointItem.CreateClusteringData();
			}
		}
		public void BeginServiceOperation() {
			refreshManager.BeginServiceOperation();
			interactivityManager.BeginServiceOperation();
		}
		public void EndServiceOperation() {
			refreshManager.EndServiceOperation();
			interactivityManager.EndServiceOperation();
		}
		public UnderlyingData GetUnderlyingData(string itemName, IList columnValues, IList rowValues, IList<string> columnNames) {
			DataDashboardItem item = (DataDashboardItem)dashboard.Items[itemName];
			ISliceDataQueryProvider provider = item;
			SliceDataQuery query = provider.GetDataQuery(this);
			UnderlyingDataQuery<SliceDataQuery> underlyingDataQuery = UnderlyingSliceDataQuery.CreateUnderlyingDataQuery(item, query, this, columnValues, rowValues, columnNames);
			ExternalSchemaConsumerHelper.ApplySchema(item.DataSource, dashboard.Items.OfType<DataDashboardItem>(), dashboard,
				new Dictionary<string, string[]> { 
				{
					ExternalSchemaConsumerHelper.SafeDataMember(item.DataMember),
					underlyingDataQuery.DataMembers.ToArray()
					}
				});
			DashboardUnderlyingDataSet dataSet = dataManager.CalculateUnderlyingDataSet(item.ComponentName, new DataSourceInfo(item.DataSource, item.DataMember), item.FakeDataSource, underlyingDataQuery);
			UnderlyingDataFormat? supportedFormat = null;
			if(RequestUnderlyingDataFormat != null) {
				RequestUnderlyingDataFormatEventArgs eventArgs = new RequestUnderlyingDataFormatEventArgs();
				RequestUnderlyingDataFormat(this, eventArgs);
				supportedFormat = eventArgs.SupportedFormat;
			}
			switch(supportedFormat ?? UnderlyingDataFormat.ListAndSchema) {
				case UnderlyingDataFormat.ListAndSchema:
					IList<string> dataMembers = dataSet.GetColumnNames();
					List<object> data = new List<object>();
					foreach(DashboardDataRow row in dataSet) {
						List<object> dataRow = new List<object>();
						foreach(string dataMember in dataMembers) {
							dataRow.Add(row[dataMember]);
						}
						data.Add(dataRow);
					}
					return new UnderlyingData {
						DataMembers = dataMembers.ToArray(),
						Data = data
					};
				case UnderlyingDataFormat.DataSet:
					return new UnderlyingData { Data = dataSet };
				default:
					throw new NotSupportedException();
			};
		}
		public DashboardClientData CalculateClientData(IEnumerable<string> itemsToCalculate) {
			bool canceled;
			return CalculateClientData(itemsToCalculate, null, null, out canceled);
		}
		public DashboardClientData CalculateClientData(IEnumerable<string> itemsToCalculate, IEnumerable<string> itemsToIgnore, out bool canceled) {
			return CalculateClientData(itemsToCalculate, null, itemsToIgnore, out canceled);
		}
		public DashboardClientData CalculateClientData(IEnumerable<string> itemsToCalculate, IDictionary<string, IDictionary<string, object>> expandfilter) {
			bool canceled;
			return CalculateClientData(itemsToCalculate,expandfilter, null, out canceled);
		}
		public DashboardClientData CalculateClientData(IEnumerable<string> itemsToCalculate, IDictionary<string, IDictionary<string, object>> expandfilter, IEnumerable<string> itemsToIgnore, out bool canceled) {
			IWaitFormActivator waitFormActivator = null;		 
				if(RequestWaitFormActivator != null) {
					RequestWaitFormActivatorEventArgs eventArgs = new RequestWaitFormActivatorEventArgs();
					RequestWaitFormActivator(this, eventArgs);
					waitFormActivator = eventArgs.WaitFormActivator;
				}
				if(waitFormActivator == null)
					waitFormActivator = EmptyWaitFormActivator.Instance;
				Func<CancellationToken, DashboardClientData> calculateClientData = (token) => {
					IEnumerable<IParameter> parameters = GetParameters();
					Action<string, MultiDimensionalDataProvider> updateSelectionCallback = (itemName, dataProvider) => {
						ISelectionController selectionController = GetSelectionController(itemName);
						if(selectionController != null)
							selectionController.UpdateSelection(dataProvider);
					};
					return dataManager.CalculateClientData(itemsToCalculate, this, this, token, expandfilter, itemsToIgnore, updateSelectionCallback);
				};
				return PerformCalculateClientDataAction(dashboard, waitFormActivator, calculateClientData, out canceled);
		}
		public IMasterFilterParameters GetMasterFilterParameters(string dashboardItemName) {
			IInteractivitySession interactivitySession = interactivityManager.GetSession(dashboardItemName);
			if (interactivitySession != null)
				return interactivitySession.MasterFilterController;
			return null;
		}
		public IDrillDownParameters GetDrillDownParameters(string dashboardItemName) {
			IInteractivitySession interactivitySession = interactivityManager.GetSession(dashboardItemName);
			if (interactivitySession != null)
				return interactivitySession.DrillDownController;
			return null;
		}
		public void CheckAllowability() {
			if (!refreshManager.IsEmpty)
				throw new DashboardNotRelevantException();
		}
		public IDrillDownController GetDrillDownController(string itemName) {
			IInteractivitySession session = interactivityManager.GetSession(itemName);
			return session != null ? session.DrillDownController : null;
		}
		public IMasterFilterController GetMasterFilterController(string itemName) {
			IInteractivitySession session = interactivityManager.GetSession(itemName);
			return session != null ? session.MasterFilterController : null;
		}
		public ISelectionController GetSelectionController(string itemName) {
			IInteractivitySession session = interactivityManager.GetSession(itemName);
			return session != null ? session.SelectionController : null;
		}
		public IRangeInteractivitySession GetRangeInteractivitySession(string itemName) {
			return interactivityManager.GetSession(itemName) as IRangeInteractivitySession;
		}
		void AssertClientDataEquals(IEnumerable<string> itemsToCalculate, DashboardClientData expected, DashboardClientData actual) {
			foreach(var name in itemsToCalculate) {
				DataStorage storage1 = expected[name] != null ? expected[name].HierarchicalDataParams.Storage : DataStorage.CreateEmpty();
				DataStorage storage2 = actual[name] != null ? actual[name].HierarchicalDataParams.Storage : DataStorage.CreateEmpty();
				DataDashboardItem item = dashboard.Items[name] as DataDashboardItem;
				if(item != null) {
				  ISliceDataQueryProvider provider = (ISliceDataQueryProvider)item;
				SliceDataQuery query = provider.GetDataQuery(this);
				bool isQueryValid = new SliceDataQueryValidator(query, new DataSourceModel(new DataSourceInfo(item.DataSource, item.DataMember), item.DataSource.GetListSource(item.DataMember), item.DataSource.GetPivotDataSource(item.DataMember))).Validate();
				if(isQueryValid)
					Helper.AssertStoragesAreEqual(query, storage1, storage2);
				else
					expected[name].HierarchicalDataParams = actual[name].HierarchicalDataParams;
			}
		}
		}
		public bool Initialize(DashboardSessionState state, bool isDesignMode) {
			bool shouldCreateDashboard = IsEmpty;
			bool shouldApplyDashboardState = true;
			if (shouldCreateDashboard) {
				this.dashboardId = state.DashboardId;
				this.settings = state.SessionSettings;
				this.dashboardParameters = state.DashboardParameters;
				CreateDashboard(isDesignMode);
				dashboard.SetCalculateTotals(settings.CalculateHiddenTotals);
			}
			else
				shouldApplyDashboardState = !(state.DashboardState == null || BinarySerializer.Serialize(GetState().DashboardState) == BinarySerializer.Serialize(state.DashboardState));
			bool shouldApplyDataBinding = (shouldCreateDashboard || state.DataVersion != DataVersion) && (dashboard.EnableAutomaticUpdates || !isDesignMode);
			if(shouldApplyDataBinding)
				if(!ReloadData(null)) {
					refreshManager.SetAllDataForDeferUpdates();
					dashboard.EnableAutomaticUpdates = false;
				}
			if (shouldApplyDashboardState)
				SetDashboardState(state.DashboardState);
			return shouldApplyDataBinding;
		}
		public void ResetDashboardColoringCache(ColorRepository cache) {
			dashboardColoringCache = cache;
		}
		public void EnsureDashboardColoringCache() {
			if(dashboardColoringCache == null) {
				DashboardColorSchemeProvider colorSchemeProvider = new DashboardColorSchemeProvider(dashboard.Items, this, dashboard);
				dashboardColoringCache = ColorSchemeGenerator.GenerateColoringCache(colorSchemeProvider, dashboard.ColorSchemeContainer.Repository, this);
			}
		}
		void CreateDashboard(bool isDesignMode) {
			IDashboardActivator activator = null;
			if(DashboardLoading != null) {
				DashboardLoadingServerEventArgs args = new DashboardLoadingServerEventArgs(dashboardId);
				DashboardLoading(this, args);
				activator = args.GetActivator();
			}
			if(activator == null)
				ThrowDashboardNotFoundException();
			dashboard = activator.CreateDashboard();
			if(dashboard == null)
				ThrowDashboardNotFoundException();
			dashboard.RaiseDashboardLoading();
			if(DashboardLoaded != null)
				DashboardLoaded(this, new DashboardLoadedServerEventArgs(dashboardId, dashboard));
			ConvertObsoleteDataSources();
			dashboard.RequestMasterFilterParameters += OnRequestMasterFilterParameters;
			dashboard.RequestDrillDownParameters += OnRequestDrillDownParameters;
			dashboard.RequestDrillDownController += OnRequestDrillDownController;
			refreshManager.Initialize(dashboard, isDesignMode);
			interactivityManager.Initialize(dashboard);
			dataManager = new DataManager(DataSessionFactory.Default, dashboard);			
		}
		void ConvertObsoleteDataSources() {
#if !DXPORTABLE
			DataSourceCollection dataSources = dashboard.DataSources;
#pragma warning disable 0618
			if(dataSources.Contains(dataSource => dataSource is DataSource)) {
#pragma warning restore 0618
				dashboard.BeginLoading();
				dashboard.BeginUpdate();
				dashboard.LockDataSourceSync();
				try {
					dataSources.BeginUpdate();
					try {
						for(int i = 0; i < dataSources.Count; i++) {
#pragma warning disable 0618
							DataSource oldDataSource = dataSources[i] as DataSource;
#pragma warning restore 0618
							if(oldDataSource != null) {
								DataSourceConverter converter = new DataSourceConverter(oldDataSource, dashboard);
								converter.Convert();
								IDashboardDataSource newDataSource = converter.NewDataSource;
								string dataMember = converter.NewDataMember;
								dataSources.RemoveAt(i);
								dataSources.Insert(i, newDataSource);
								IList<DataDashboardItem> dashboardItems = dashboard.Items.OfType<DataDashboardItem>().Where(item => item.DataSource == oldDataSource).ToList();
								foreach(DataDashboardItem item in dashboardItems) {
									item.LockChanging();
									try {
										item.DataSource = newDataSource;
										item.DataMember = dataMember;
									} finally {
										item.UnlockChanging();
										item.UpdateDataMembersOnLoading();
									}
								}
								IList<DynamicListLookUpSettings> lookupSettings = dashboard.Parameters.Select(parameter => parameter.LookUpSettings as DynamicListLookUpSettings).Where(settings => settings != null && settings.DataSource == oldDataSource).ToList();
								foreach(DynamicListLookUpSettings settings in lookupSettings) {
									settings.DataSource = newDataSource;
									settings.DataMember = dataMember;
								}
								Func<ColorSchemeEntry, bool> colorSchemeContainsOldDataSoure = entry => entry.DataSource == oldDataSource;
								IList<ColorSchemeEntry> colorSchemeEntries = dashboard.ColorScheme.Where(colorSchemeContainsOldDataSoure).Union(
																			 dashboard.Items.OfType<ChartDashboardItemBase>().SelectMany(item => item.ColorScheme.Where(colorSchemeContainsOldDataSoure))).ToList();
								foreach(ColorSchemeEntry entry in colorSchemeEntries) {
									entry.DataSource = newDataSource;
									entry.DataMember = dataMember;
								}
								if(colorSchemeEntries.Any()) {
									IList<ColorSchemeContainer> containers = new ColorSchemeContainer[] { dashboard.ColorSchemeContainer }.Union(
																				 dashboard.Items.OfType<ChartDashboardItemBase>().Select(item => item.ColorSchemeContainer)).ToList();
									foreach(ColorSchemeContainer repository in containers)
										repository.OnEndLoading();
								}
							}
						}
					} finally {
						dataSources.EndUpdate();
					}
				} finally {
					dashboard.UnlockDataSourceSync();
					dashboard.CancelUpdate();
					dashboard.EndLoading();
				}
			}
#endif
		}
		void OnRequestMasterFilterParameters(object sender, RequestMasterFilterParametersEventArgs e) {
			e.Parameters = GetMasterFilterParameters(e.DashboardItemName);
		}
		void OnRequestDrillDownController(object sender, RequestDrillDownControllerEventArgs e) {
			e.Controller = GetDrillDownController(e.DashboardItemName);
		}
		void OnRequestDrillDownParameters(object sender, RequestDrillDownParametersEventArgs e) {
			e.Parameters = GetDrillDownParameters(e.DashboardItemName);
		}
		void ThrowDashboardNotFoundException() {
			throw new DashboardNotFoundException();
		}
		void ApplyExternalDataSourceSchema(IList<IDashboardDataSource> dataSources) {
			foreach(IDashboardDataSource dataSource in dataSources) {
				ExternalSchemaConsumerHelper.ApplySchema(dataSource, dashboard.Items.OfType<DataDashboardItem>(), dashboard);
			}
		}
		IList<IDashboardDataSource> PrepareDataSourceList(IEnumerable<string> dataSourceComponentNames) {
			if(dataSourceComponentNames != null)
				return dashboard.DataSources.Where(dataSource => dataSourceComponentNames.Contains<string>(dataSource.ComponentName)).ToArray();
			return new List<IDashboardDataSource>(dashboard.DataSources);
		}
		DataSourceLoadingResultType LoadData(IEnumerable<string> dataSourceComponentNames, ReloadDataArgs args) {
			IList<IDashboardDataSource> dataSourcesToLoad = PrepareDataSourceList(dataSourceComponentNames);
			if(!IsAllowLoadUnusedDataSources()) {
				IEnumerable<IDashboardDataSource> usedDataSources = GetUsedDataSources();
				dataSourcesToLoad = dataSourcesToLoad.Where(dataSource => usedDataSources.Contains(dataSource)).ToArray();
			}
			if(dataSourcesToLoad.Count > 0) {
				ApplyExternalDataSourceSchema(dataSourcesToLoad);
				IDashboardDataLoader dataLoader = CreateDataLoader(args);
				return dataLoader.LoadData(dataSourcesToLoad, GetActualParameters()).ResultType;
			}
			return DataSourceLoadingResultType.Success;
		}
		IDashboardDataLoader CreateDataLoader(ReloadDataArgs args) {
			DataLoaderParameters dataLoaderParameters = CreateDataLoaderParameters(args);
			IDashboardDataLoader dataLoader = null;
			if(RequestDataLoader != null) {
			   RequestDataLoaderEventArgs eventArgs = new RequestDataLoaderEventArgs(dataLoaderParameters);
				RequestDataLoader(this, eventArgs);
				dataLoader = eventArgs.DataLoader;
			}
			if(dataLoader == null)
				dataLoader = new DashboardDataLoader(dataLoaderParameters);
			return dataLoader;
		}
		DataLoaderParameters CreateDataLoaderParameters(ReloadDataArgs args) {
			IDashboardSqlCustomizationService dataConnectionParametersDependenciesService = null;
#if !DXPORTABLE
			IDashboardExcelCustomizationService excelOptionsDependenciesService = null;
#endif
			IDataConnectionParametersProvider dataConnectionParametersProvider = null;
			IDataConnectionParametersProvider defaultDataConnectionParametersProvider = this;
			IConnectionStringsService appConfigPatcherService = null;
			IDashboardCustomSqlQueryValidator customSqlQueryValidator = this;
			IPlatformDependenciesService platformDependenciesService = null;
			if(RequestCustomizationServices != null) {
				RequestCustomizationServicesEventArgs eventArgs = new RequestCustomizationServicesEventArgs(defaultDataConnectionParametersProvider);
				RequestCustomizationServices(this, eventArgs);
				dataConnectionParametersDependenciesService = eventArgs.SqlCustomizationService;
#if !DXPORTABLE
				excelOptionsDependenciesService = eventArgs.ExcelCustomizationService;
#endif
				dataConnectionParametersProvider = eventArgs.DataConnectionParametersProvider;
				platformDependenciesService = eventArgs.PlatformDependenciesService;
			}
			if(dataConnectionParametersProvider == null)
				dataConnectionParametersProvider = defaultDataConnectionParametersProvider;
			if(RequestAppConfigPatcherService != null) {
				RequestAppConfigPatcherServiceEventArgs eventArgs = new RequestAppConfigPatcherServiceEventArgs();
				RequestAppConfigPatcherService(this, eventArgs);
				appConfigPatcherService = eventArgs.AppConfigPatcherService;
			}
			IWaitFormActivator waitActivator = null;
			bool suppressWaitForm = args != null && args.SuppressWaitForm;
			if(!suppressWaitForm && RequestWaitFormActivator != null) {
				RequestWaitFormActivatorEventArgs eventArgs = new RequestWaitFormActivatorEventArgs();
				RequestWaitFormActivator(this, eventArgs);
				waitActivator = eventArgs.WaitFormActivator;
			}
			if(waitActivator == null)
				waitActivator = EmptyWaitFormActivator.Instance;
			IErrorHandler errorHandler = null;
			if(RequestErrorHandler != null) {
				RequestErrorHandlerEventArgs eventArgs = new RequestErrorHandlerEventArgs();
				RequestErrorHandler(this, eventArgs);
				errorHandler = eventArgs.ErrorHandler;
			}
			if(errorHandler == null)
				errorHandler = this;
			return new DataLoaderParameters {
				PlatformDependenciesService = platformDependenciesService,
#if !DXPORTABLE
				ExcelCustomizationService = excelOptionsDependenciesService,
#endif
				SqlCustomizationService = dataConnectionParametersDependenciesService,
				DataConnectionParametersProvider = dataConnectionParametersProvider,
				WaitFormActivator = waitActivator,
				ErrorHandler = errorHandler,
				ConfigPatcherService = appConfigPatcherService,
				CustomSqlQueryValidator = customSqlQueryValidator
			};
		}
		bool IsAllowLoadUnusedDataSources() {
			if(AllowLoadUnusedDataSources != null) {
				AllowLoadUnusedDataSourcesServerEventArgs args = new AllowLoadUnusedDataSourcesServerEventArgs(dashboardId);
				AllowLoadUnusedDataSources(this, args);
				return args.Allow;
			}
			return false;
		}
		IEnumerable<IDashboardDataSource> GetUsedDataSources() {
			List<IDashboardDataSource> usedDataSources = new List<IDashboardDataSource>();
			usedDataSources.AddRange(dashboard.Items.OfType<DataDashboardItem>().Select(item => item.DataSource).Cast<IDashboardDataSource>());
			usedDataSources.AddRange(dashboard.Parameters.Select(p => {
				DynamicListLookUpSettings l = p.LookUpSettings as DynamicListLookUpSettings;
				return l != null ? l.DataSource : null;
			}));
			return usedDataSources.Distinct();
		}
		IEnumerable<IParameter> GetActualParameters() {
			IEnumerable<IParameter> parameters = GetParameters();
			CustomParametersEventArgs args = new CustomParametersEventArgs(parameters);
			dashboard.RaiseCustomParameters(args);
			if(args.Parameters != null)
				parameters = args.Parameters;
			if(CustomParameters != null) {
				CustomParametersServerEventArgs serverArgs = new CustomParametersServerEventArgs(dashboardId, parameters);
				CustomParameters(this, serverArgs);
				if(serverArgs.Parameters != null)
					parameters = serverArgs.Parameters;
			}
			return parameters;
		}
		IEnumerable<IParameter> GetParameters() {
			List<Parameter> parameters = null;
			if(dashboardParameters != null) {
				parameters = new List<Parameter>();
				foreach(Parameter parameter in dashboard.Parameters) {
					object value = parameter.Value;
					DashboardParameterInfo dashboardParameter = dashboardParameters.Where(dp => dp.Name == parameter.Name).FirstOrDefault();
					if(dashboardParameter != null)
						value = dashboardParameter.Value;
					parameters.Add(new Parameter(parameter.Name, parameter.Type, value, parameter.AllowNull));
				}
			}
			IEnumerable<IParameter> pars = parameters;
			if(pars == null)
				pars = dashboard.Parameters.Select(parameter => new Parameter(parameter.Name, parameter.Type, parameter.Value, parameter.AllowNull));
			return pars;
		}
		MapClusterizationRequestInfo GetMapClusterizationRequestInfoFromClientState(string itemName) {
			if(ClientState == null)
				return null;
			Hashtable itemClientState = ClientState[itemName] as Hashtable;
			if(itemClientState == null)
				return null;
			Hashtable viewport = (Hashtable)itemClientState["viewport"];
			Hashtable clientSize = (Hashtable)itemClientState["clientSize"];
			return new MapClusterizationRequestInfo() {
				Viewport = new MapViewportState() {
					LeftLongitude = Helper.ConvertToDouble(viewport["LeftLongitude"]),
					TopLatitude = Helper.ConvertToDouble(viewport["TopLatitude"]),
					RightLongitude = Helper.ConvertToDouble(viewport["RightLongitude"]),
					BottomLatitude = Helper.ConvertToDouble(viewport["BottomLatitude"])
				},
				ClientSize = new Size() {
					Width = Helper.ConvertToInt32(clientSize["width"]),
					Height = Helper.ConvertToInt32(clientSize["height"])
				}
			};
		}
		void OnUpdateClientState() {
			foreach(DashboardItem item in dashboard.Items) {
				IRaiseClusterizationRequestItem geoPointMap = item as IRaiseClusterizationRequestItem;
				if(geoPointMap != null) {
					MapClusterizationRequestInfo info = GetMapClusterizationRequestInfoFromClientState(((IDashboardComponent)geoPointMap).ComponentName);
					if(info != null)
						geoPointMap.UpdateMapInfo(info);
				}
			}
		}
		void SetMasterFilterInternal(string itemName, IValuesSet values) {
			IMasterFilterController controller = GetMasterFilterController(itemName);
			if(controller != null) {
				IMasterFilterDataPreparer dataPreparer = controller.DataPreparer;
				if(dataPreparer != null)
					dataPreparer.PrepareData(dashboard, this, this);
				controller.SetMasterFilter(values);
			}
		}
		IEnumerable<DimensionFilterValues> GetFilterValues(string filterName) {
			IInteractivitySession interactivitySession = interactivityManager.GetSession(filterName);
			IMasterFilterController masterFilterController = interactivitySession.MasterFilterController;
			return masterFilterController != null ? masterFilterController.GetFilterValues(dashboard, this) : new DimensionFilterValues[0];
		}
		IFiltersProvider GetMasterFilterItemsProvider(string itemName) {
			return string.IsNullOrEmpty(itemName) ? dashboard : ((IFiltersProvider)dashboard.GetDashboardItem(itemName));
		}
		#region IDataConnectionParametersProvider members
		IDBSchemaProvider IDataConnectionParametersProvider.GetDbSchemaProvider(SqlDataConnection dataConnection) {
			return new DBSchemaProvider();
		}
		object IDataConnectionParametersProvider.RaiseDataLoading(string dataSourceComponentName, string dataSourceName, object data) {
			IDashboardDataSource dataSource = dashboard.DataSources[dataSourceComponentName];
			if(dataSource == null || (dataSource != null && dataSource.GetIsDataLoadingSupported())) {
				if(DataLoading != null) {
					DataLoadingServerEventArgs args = new DataLoadingServerEventArgs(dashboardId, dataSourceComponentName, dataSourceName) {
						Data = data
					};
					DataLoading(this, args);
					return args.Data;
				}
				return data;
			} else
				return null;
		}
		CriteriaOperator IDataConnectionParametersProvider.RaiseCustomFilterExpression(CustomFilterExpressionEventArgs e) {
			CriteriaOperator criteriaOperator = dashboard.RaiseCustomFilterExpression(e);
			if(CustomFilterExpression != null) {
				CustomFilterExpressionServerEventArgs args = new CustomFilterExpressionServerEventArgs(dashboardId, e.DataSourceComponentName, e.DataSourceName, e.TableName) {
					FilterExpression = criteriaOperator
				};
				CustomFilterExpression(this, args);
				return args.FilterExpression;
			}
			return e.FilterExpression;
		}
		#endregion
		#region IDataConnectionParametersProvider members
		DataConnectionParametersBase IDataConnectionParametersProvider.RaiseConfigureDataConnection(string connectionName, string dataSourceName, DataConnectionParametersBase parameters) {
			DashboardConfigureDataConnectionEventArgs e = new DashboardConfigureDataConnectionEventArgs(connectionName, dataSourceName, parameters);
			dashboard.RaiseConfigureDataConnection(e);
			if(ConfigureDataConnection != null) {
				ConfigureDataConnectionServerEventArgs args = new ConfigureDataConnectionServerEventArgs(dashboardId, connectionName, dataSourceName) {
					ConnectionParameters = e.ConnectionParameters
				};
				ConfigureDataConnection(this, args);
				return args.ConnectionParameters;
			}
			return e.ConnectionParameters;
		}
		DataConnectionParametersBase IDataConnectionParametersProvider.RaiseHandleConnectionError(string dataSourceName, ConnectionErrorEventArgs args) {
			DashboardConnectionErrorEventArgs e = new DashboardConnectionErrorEventArgs(args.ConnectionName, dataSourceName, args.ConnectionParameters, args.Exception);
			dashboard.RaiseConnectionError(e);
			if(!e.Handled && !e.Cancel) {
				if(ConnectionError != null) {
					ConnectionErrorServerEventArgs serverArgs = new ConnectionErrorServerEventArgs(dashboardId, e.ConnectionName, dataSourceName, e.Exception) {
						Handled = e.Handled,
						Cancel = e.Cancel,
						ConnectionParameters = e.ConnectionParameters
					};
					ConnectionError(this, serverArgs);
					e.Handled = serverArgs.Handled;
					e.Cancel = serverArgs.Cancel;
					e.ConnectionParameters = serverArgs.ConnectionParameters;
				}
			}
			if(e.Cancel)
				e.ConnectionParameters = null;
			args.Handled = e.Handled;
			args.Cancel = e.Cancel;
			args.ConnectionParameters = e.ConnectionParameters;
			return e.ConnectionParameters;
		}
		#endregion
		#region IErrorHandler members
		void IErrorHandler.ShowDataSourceLoaderResultMessageBox(DataSourceLoadingResultType result) {
		}
		void IErrorHandler.ShowDataSourceLoadingErrors(List<DataLoaderError> dataLoaderErrors) {
			this.dataLoaderErrors = new List<DataLoaderError>(dataLoaderErrors.FindAll(error => error.Type == DataLoaderErrorType.Connection));
		}
		#endregion
		#region IColoringCacheProvider
		ColorRepository IColoringCacheProvider.GetLocalColoringCache(string itemName) {
			DataDashboardItem dataDashboardItem = dashboard.Items[itemName] as DataDashboardItem;
			if (dataDashboardItem != null) {
				DashboardItemColorSchemeProvider colorSchemeProvider = new DashboardItemColorSchemeProvider(dataDashboardItem, this, dashboard);
				return ColorSchemeGenerator.GenerateColoringCache(colorSchemeProvider, dataDashboardItem.ColorSchemeContainer.Repository, this);
			}
			return null;
		}
		ColorRepository IColoringCacheProvider.GetGlobalColoringCache() {
			return dashboardColoringCache;
		}
		ColorRepository IColoringCacheProvider.GetActualColoringCache(string itemName) {
			DataDashboardItem dataDashboardItem = dashboard.Items[itemName] as DataDashboardItem;
			if (dataDashboardItem != null && dataDashboardItem.IsColoringSupported) {
				if (dataDashboardItem.IsGloballyColored)
					return ((IColoringCacheProvider)this).GetGlobalColoringCache();
				return ((IColoringCacheProvider)this).GetLocalColoringCache(itemName);
			}
			return null;
		}
		#endregion
		#region IDataSessionProvider
		IDataSession IDataSessionProvider.GetDataSession(string itemName, DataSourceInfo dataSourceInfo, IList fakeDataSource) {
			return dataManager.GetDataSession(itemName, dataSourceInfo, fakeDataSource);
		}
		#endregion
 		#region IActualParametersProvider
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			return GetParameters();
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			return GetActualParameters();
		}
		#endregion
		#region IDashboardCustomSqlQueryValidator
		void IDashboardCustomSqlQueryValidator.Validate(ValidateDashboardCustomSqlQueryEventArgs e) {
			e.DashboardId = dashboardId;
			dashboard.RaiseValidateCustomSqlQuery(e);
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
		#endregion
	}
}
