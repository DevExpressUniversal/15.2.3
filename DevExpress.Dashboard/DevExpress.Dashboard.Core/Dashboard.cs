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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.DashboardCommon.Server;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	[
#if !DEBUG && !DXPORTABLE
#endif
	DXToolboxItem(false),
#if !DXPORTABLE
	InitAssemblyResolver,
	Designer(TypeNames.DashboardComponentDesigner),
	Designer(TypeNames.DashboardComponentDesigner, typeof(IRootDesigner)),
#endif
	DesignerSerializer(TypeNames.DashboardCodeSerializer, TypeNames.TypeCodeDomSerializer)
	]
	public class Dashboard : Component, IDashboard, ISupportInitialize, IServiceProvider, IFilterGroupOwner, IFiltersProvider, IColorSchemeContext, IChangeService,
		IActualParametersProvider, IDataSourceInfoProvider {
		const string xmlDashboard = "Dashboard";
		const string xmlDataConnections = "DataConnections";
		const string xmlLayout = "Layout";
		const string xmlCurrencyCulture = "CurrencyCulture";
		const string xmlTitle = "Title";
		const string xmlUserData = "UserData";
		const string xmlDesignerAutomaticUpdates = "DesignerAutomaticUpdates";
		const bool DefaultDesignerAutomaticUpdates = true;
		readonly CollectionPrefixCaptionGenerator<DashboardItem> dashboardItemCaptionGenerator;
		readonly CollectionPrefixCaptionGenerator<DashboardItemGroup> groupCaptionGenerator;
		readonly CollectionPrefixCaptionGenerator<IDashboardDataSource> dataSourceCaptionGenerator;
		readonly DataSourceCollection dataSources = new DataSourceCollection();
		readonly List<DataConnectionBase> dataConnections = new List<DataConnectionBase>();
		readonly RepositoryItemListXmlSerializer<DataConnectionBase> dataConnectionsSerializer;
		readonly DashboardItemCollection items = new DashboardItemCollection();
		readonly DashboardItemGroupCollection groups = new DashboardItemGroupCollection();
		readonly DashboardParameterCollection parameters = new DashboardParameterCollection();
		readonly CollectionPrefixNameGenerator<DashboardParameter> parameterNameGenerator;
		readonly EmptyFilter dashboardFilter;
		readonly Locker loadingLocker = new Locker();
		readonly Locker updateLocker = new Locker();
		readonly Locker dataSourceSyncLocker = new Locker();
		readonly ColorSchemeContainer colorSchemeContainer;
		DashboardTitle title;
		DashboardLayoutGroup layoutRoot;
		string currencyCultureName;
		bool isDisposed;
		DashboardRuntimeComponentNameGenerator runtimeNameGenerator;
		XElement userData;
		IFilterGroup filterGroup;
		bool enableAutomaticUpdates = DefaultDesignerAutomaticUpdates;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsDisposed { get { return isDisposed; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardParameters"),
#endif
 Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.DashboardParameterCollectionEditor, typeof(UITypeEditor))
		]
		public DashboardParameterCollection Parameters { get { return parameters; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutRoot"),
#endif
		Browsable(false),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public DashboardLayoutGroup LayoutRoot { 
			get { return layoutRoot; } 
			set {
				if(value != layoutRoot) {
					UnsubscribeLayoutEvents();
					layoutRoot = value;
					SubscribeLayoutEvents();
					if(!Loading)
						RaiseLayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardDataSources"),
#endif
 Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.DataSourceCollectionEditor, typeof(UITypeEditor))
		]
		public DataSourceCollection DataSources { get { return dataSources; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public List<DataConnectionBase> DataConnections { get { return dataConnections; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItems"),
#endif
 Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.DashboardItemCollectionEditor, typeof(UITypeEditor))
		]
		public DashboardItemCollection Items { get { return items; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardGroups"),
#endif
 Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.DashboardItemGroupCollectionEditor, typeof(UITypeEditor))
		]
		public DashboardItemGroupCollection Groups { get { return groups; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardTitle"),
#endif
 Category(CategoryNames.General),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public DashboardTitle Title { get { return title; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardCurrencyCultureName"),
#endif
 Category(CategoryNames.General),
		Editor(TypeNames.CurrencyEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string CurrencyCultureName {
			get { return currencyCultureName; }
			set {
				if(value != currencyCultureName) {
					SetCurrencyCultureName(value, !Loading);
					if(!Loading)
						((IChangeService)this).OnChanged(new ChangedEventArgs(ChangeReason.ClientData, null, null));
				}
			}
		}
		[
		 Category(CategoryNames.General),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true),
		]
		public bool EnableAutomaticUpdates {
			get { return enableAutomaticUpdates; }
			set {
				if(enableAutomaticUpdates != value) {
					enableAutomaticUpdates = value;
					RaiseEnableAutomaticUpdatesChanged(new EnableAutomaticUpdatesEventArgs(enableAutomaticUpdates));
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardColorScheme"),
#endif
		Editor(TypeNames.DashboardColorSchemeEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ColorScheme ColorScheme { get { return colorSchemeContainer.Scheme; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardUserData"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public XElement UserData {
			get { return userData; }
			set { userData = value; }
		}
		internal CollectionPrefixCaptionGenerator<DashboardItem> DashboardItemCaptionGenerator { get { return dashboardItemCaptionGenerator; } }
		internal CollectionPrefixCaptionGenerator<DashboardItemGroup> GroupCaptionGenerator { get { return groupCaptionGenerator; } }
		internal CollectionPrefixCaptionGenerator<IDashboardDataSource> DataSourceCaptionGenerator { get { return dataSourceCaptionGenerator; } }
		internal ColorSchemeContainer ColorSchemeContainer { get { return colorSchemeContainer; } }
		internal bool Loading { get { return loadingLocker.IsLocked; } }
		internal IEnumerable<DashboardItem> ItemsAndGroups {
			get {
				foreach (DashboardItem item in items)
					yield return item;
				foreach (DashboardItemGroup group in groups)
					yield return group;
			}
		}
		internal IEnumerable<IDashboardComponent> DashboardComponents {
			get {
				foreach(DashboardItem item in items)
					yield return item;
				foreach(DashboardItemGroup group in groups)
					yield return group;
				foreach(IDashboardDataSource dataSource in dataSources)
					yield return dataSource;
			}
		}
		internal bool IsEmpty { get { return items.Count == 0 && groups.Count == 0; } }
		internal bool IsVSDesignMode { get { return Helper.IsComponentVSDesignMode(this); } }
		internal bool IsDataSourceSyncLocked { get { return dataSourceSyncLocker.IsLocked; } }
		internal bool LockColorUpdates { get; set; }
		internal IEnumerable<DashboardItem> InteractivityOrderedItems { get { return dashboardFilter.GetAffected().OfType<DashboardItem>(); } }
		public event EventHandler<NotifyingCollectionChangedEventArgs<IDashboardDataSource>> DataSourceCollectionChanged;
		public event EventHandler<NotifyingCollectionChangedEventArgs<DashboardItemGroup>> GroupCollectionChanged;		
		public event EventHandler<NotifyingCollectionChangedEventArgs<DashboardItem>> ItemCollectionChanged;
		public event EventHandler<NotifyingCollectionChangedEventArgs<DashboardParameter>> ParameterCollectionChanged;
		public event EventHandler DashboardLoading;
		public event DashboardDataLoadingEventHandler DataLoading;
		public event DashboardConfigureDataConnectionEventHandler ConfigureDataConnection;
		public event DashboardCustomFilterExpressionEventHandler CustomFilterExpression;
		public event CustomParametersEventHandler CustomParameters;
		public event ValidateDashboardCustomSqlQueryEventHandler ValidateCustomSqlQuery;
		public event DashboardConnectionErrorEventHandler ConnectionError;
		public event DashboardOptionsChangedEventHandler OptionsChanged;
		internal event EventHandler TitleChanged;
		internal event EventHandler LoadCompleted;
		internal event EventHandler LayoutChanged;
		internal event EventHandler ColoringChanged;
		internal event EventHandler<DataSourceChangedEventArgs> RequestDataSourceFill;
		internal event EventHandler<DataSourceChangedEventArgs> DataSourceDataChanged;
		internal event EventHandler<DataSourceChangedEventArgs> DataSourceCaptionChanged;
		internal event EventHandler<DashboardItemChangedEventArgs> DashboardItemChanged;
		internal event EventHandler<ComponentNameChangedEventArgs> DashboardItemComponentNameChanged;
		internal event EventHandler<ComponentNameChangedEventArgs> DataSourceComponentNameChanged;
		internal event EventHandler<DataSourceCalcFieldCollectionChangedEventArgs> DataSourceCalculatedFieldCollectionChanged;
		internal event EventHandler<DataSourceCalculatedFieldChangedEventArgs> DataSourceCalculatedFieldChanged;
		internal event EventHandler<DataSourceCalcFieldCollectionChangedEventArgs> DataSourceCalculatedFieldsCorrupted;
		internal event EventHandler DashboardChanged;
		internal event EventHandler UpdateCanceled;
		internal event EventHandler<EnableAutomaticUpdatesEventArgs> EnableAutomaticUpdatesChanged;
		internal event EventHandler<RequestMasterFilterParametersEventArgs> RequestMasterFilterParameters;
		internal event EventHandler<RequestDrillDownParametersEventArgs> RequestDrillDownParameters;
		internal event EventHandler<RequestDrillDownControllerEventArgs> RequestDrillDownController;
		internal event EventHandler<RangeFilterRangeChangedEventArgs> RangeFilterRangeChanged;
		public Dashboard() {
#if !DEBUG && !DXPORTABLE
#endif
			dataConnectionsSerializer = new RepositoryItemListXmlSerializer<DataConnectionBase>(xmlDataConnections, XmlRepository.DataConnectionRepository) { List = dataConnections };
			runtimeNameGenerator = new DashboardRuntimeComponentNameGenerator(items, groups, dataSources);
			dashboardItemCaptionGenerator = new CollectionPrefixCaptionGenerator<DashboardItem>(items);
			groupCaptionGenerator = new CollectionPrefixCaptionGenerator<DashboardItemGroup>(groups);
			dataSourceCaptionGenerator = new CollectionPrefixCaptionGenerator<IDashboardDataSource>(dataSources);
			parameterNameGenerator = new CollectionPrefixNameGenerator<DashboardParameter>(parameters, string.Empty);
			title = CreateDashboardTitle();
			dashboardFilter = new EmptyFilter();
			filterGroup = CreateFilterGroup();
			colorSchemeContainer = new ColorSchemeContainer(this);
			dataSources.CollectionChanged += OnDataSourcesChanged;
			items.CollectionChanged += OnItemsChanged;
			groups.CollectionChanged += OnGroupsChanged;
			parameters.CollectionChanged += OnParametersChanged;
			Disposed += (sender, e) => isDisposed = true;
			SetCurrencyCultureName(Helper.DefaultCurrencyCultureName, false);
		}	 
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			BeginLoading();
			filterGroup.BeginUpdate();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			EndInit(null, false);
		}
		[Obsolete("The Dashboard.AddDataSource method is obsolete now. Use the Dashboard.DataSources.Add method instead.")]
		public void AddDataSource(string name, object data) {
#if !DXPORTABLE
			dataSources.Add(new DashboardObjectDataSource(name, data));
#endif
		}
		public void SaveToXml(string filePath) {
			using(Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				SaveToXml(stream);
		}
		public void SaveToXml(Stream stream) {
			XmlHelper.CheckStream(stream);
			XElement rootElement = new XElement(xmlDashboard);
			SaveCurrencyCultureToXml(rootElement);
			SaveTitleToXml(rootElement);
			SaveDesignerAutomaticUpdates(rootElement);
			dataConnectionsSerializer.SaveToXml(rootElement);
			dataSources.SaveToXml(rootElement);
			SaveParametersToXml(rootElement);			
			items.SaveToXml(rootElement);
			groups.SaveToXml(rootElement);
			ColorScheme.SaveToXml(rootElement);
			SaveLayoutToXml(rootElement);
			SetUserDataElement(rootElement, userData);
			XmlHelper.SaveXmlToStream(stream, rootElement);
		}
		public void LoadFromXml(string filePath) {
			LoadFromXml(filePath, null);
		}
		public void LoadFromXml(Stream stream) {
			LoadFromXml(stream, null);
		}
		public static XElement LoadUserDataFromXml(string filePath) {
			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				return LoadUserDataFromXml(stream);
		}
		public static XElement LoadUserDataFromXml(Stream stream) {
			return GetUserData(GetDashboardRootNode(stream));
		}
		[Obsolete("The BeginUpdateLayout method is obsolete now. Use the BeginUpdate method instead.")]
		public void BeginUpdateLayout() {
			BeginUpdate();
		}
		[Obsolete("The EndUpdateLayout method is obsolete now. Use the EndUpdate method instead.")]
		public void EndUpdateLayout() {
			EndUpdate();
		}
		public void BeginUpdate() {
			LockUpdate();
		}
		public void EndUpdate() {
			UnlockUpdate();
			RaiseDashboardChanged();
		}
		public DashboardItemGroup CreateGroup() {
			DashboardItemGroup group = new DashboardItemGroup();
			groups.Add(group);
			return group;
		}
		public void RebuildLayout() {
			RebuildLayout(1, 1);
		}
		public void RebuildLayout(int clientWidth, int clientHeight) {
			if (clientWidth <= 0)
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageInvalidLayoutClientWidth), "clientWidth");
			if (clientHeight <= 0)
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageInvalidLayoutClientHeight), "clientHeight");
			LayoutRoot = new DashboardLayoutCreator(clientWidth, clientHeight, layoutRoot, Items, Groups).LayoutRoot;
		}
		internal void BeginLoading() {
			loadingLocker.Lock();
		}
		internal void EndLoading() {
			loadingLocker.Unlock();
		}
		internal void LoadFromXml(string filePath, DashboardState state) {
			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				LoadFromXml(stream, state);
		}
		internal void LoadFromXml(Stream stream, DashboardState state) {
			XmlHelper.CheckStream(stream);
			XElement rootElement = GetDashboardRootNode(stream);
			BeginInit();
			ClearDashboard(false);
			try {
				LoadDesignerAutomaticUpdatesFromXml(rootElement);
				LoadCurrencyCultureFromXml(rootElement);
				LoadTitleFromXml(rootElement);
				dataConnectionsSerializer.LoadFromXml(rootElement);
				dataSources.LoadFromXml(rootElement);
				items.LoadFromXml(rootElement);
				groups.LoadFromXml(rootElement);
				ColorScheme.LoadFromXml(rootElement);
				LoadLayoutFromXml(rootElement);
				LoadParametersFromXml(rootElement);
				userData = GetUserData(rootElement);
			}
			finally {
				EndInit(state, true);
			}
		}
		internal DashboardItemViewModel CreateViewModel(string itemName) {
			return GetDashboardItem(itemName).CreateViewModel();
		}
		internal DashboardItemCaptionViewModel CreateCaptionViewModel(string itemName) {
			return GetDashboardItem(itemName).CreateCaptionViewModel();
		}
		internal DashboardTitleViewModel CreateTitleViewModel() {
			return title.CreateViewModel();
		}
		internal ConditionalFormattingModel CreateConditionalFormattingModel(string itemName) {
			return GetDashboardItem(itemName).CreateConditionalFormattingModel();
		}
		internal IList<DashboardParameterViewModel> GetParameterViewModels(IActualParametersProvider provider) {
			if(Parameters.Count == 0)
				return new List<DashboardParameterViewModel>();
			List<DashboardParameterViewModel> list = new List<DashboardParameterViewModel>(Parameters.Count);
			foreach(DashboardParameter parameter in Parameters)
				list.Add(parameter.CreateViewModel(provider));
			return list;
		}
		internal DashboardItem GetDashboardItem(string itemName) {
			return items.ContainsName(itemName) ? items[itemName] : groups[itemName];
		}
		internal IList<DashboardItem> GetDashboardItems() {
			return GetItemsAndGroups();
		}
		internal CriteriaOperator RaiseCustomFilterExpression(CustomFilterExpressionEventArgs eventArgs) {
			DashboardCustomFilterExpressionEventArgs dashboardEventArgs = new DashboardCustomFilterExpressionEventArgs(DataSources.FindFirst(ds => ds.ComponentName == eventArgs.DataSourceComponentName), eventArgs.TableName) { FilterExpression = eventArgs.FilterExpression };
			OnCustomFilterExpression(dashboardEventArgs);
			return dashboardEventArgs.FilterExpression;
		}
		internal void RaiseValidateCustomSqlQuery(ValidateDashboardCustomSqlQueryEventArgs eventArgs) {
			OnValidateCustomSqlQuery(eventArgs);
		}
		internal IEnumerable<IParameter> RaiseCustomParameters(CustomParametersEventArgs eventArgs) {
			OnCustomParameters(eventArgs);
			return eventArgs.Parameters;
		}
		internal void RaiseConfigureDataConnection(DashboardConfigureDataConnectionEventArgs eventArgs) {
			OnConfigureDataConnection(eventArgs);
		}
		internal void RaiseConnectionError(DashboardConnectionErrorEventArgs eventArgs) {
			OnConnectionError(eventArgs);
		}
		internal void RaiseDashboardLoading() {
			OnDashboardLoading();
		}
		internal object RaiseDataLoading(IDashboardDataSource dataSource) {
			DashboardDataLoadingEventArgs eventArgs = new DashboardDataLoadingEventArgs(dataSource);
			OnDataLoading(eventArgs);
			return eventArgs.Data;
		}
		internal void RaiseDashboardItemComponentNameChanged(string oldComponentName, string newComponentName) {
			if (!Loading && DashboardItemComponentNameChanged != null)
				DashboardItemComponentNameChanged(this, new ComponentNameChangedEventArgs(oldComponentName, newComponentName));
		}
		internal void OnTitleChanged() {
			if (!Loading)
				RaiseTitleChanged();
		}
		internal void OnDashboardItemChanged(DashboardItem dashboardItem, ChangedEventArgs e) {
			RaiseDashboardItemChanged(dashboardItem, e);
		}
		internal void OnRangeFilterRangeChanged(RangeFilterDashboardItem rangeFilter) {
			if (RangeFilterRangeChanged != null)
				RangeFilterRangeChanged(this, new RangeFilterRangeChangedEventArgs(rangeFilter.ComponentName, rangeFilter.MinValue, rangeFilter.MaxValue));
		}
		internal void FillDataSource(IDashboardDataSource dataSource) {
			if (RequestDataSourceFill != null)
				RequestDataSourceFill(this, new DataSourceChangedEventArgs(dataSource));
			RaiseDashboardChanged();
		}
		internal void SetCalculateTotals(bool calculateTotals) {
			foreach (DataDashboardItem item in items.OfType<DataDashboardItem>())
				item.SetCalculateTotals(calculateTotals);
		}
		internal void SyncronizeDataConnections() {
#if !DXPORTABLE
			dataConnections.Clear();
			foreach (IDashboardDataSource dataSource in dataSources) {
				IDataProvider dataProvider = dataSource.DataProvider;
				if (dataProvider != null) {
					DataConnectionBase dataConnection = dataProvider.Connection;
					if (!dataConnections.Contains(dataConnection))
						dataConnections.Add(dataConnection);
				}
			}
#endif
		}
		internal TService GetService<TService>() where TService : class {
			return ((IServiceProvider)this).GetService<TService>();
		}
		internal void CancelUpdate() {
			UnlockUpdate();
			RaiseUpdateCanceled();
		}
		internal void LockDataSourceSync() {
			dataSourceSyncLocker.Lock();
		}
		internal void UnlockDataSourceSync() {
			dataSourceSyncLocker.Unlock();
		}
		internal IMasterFilterParameters GetMasterFilterParameters(string dashboardItemName) {
			if (RequestMasterFilterParameters != null) {
				var args = new RequestMasterFilterParametersEventArgs(dashboardItemName);
				RequestMasterFilterParameters(this, args);
				return args.Parameters;
			}
			return null;
		}
		internal IDrillDownParameters GetDrillDownParameters(string dashboardItemName) {
			if (RequestDrillDownParameters != null) {
				var args = new RequestDrillDownParametersEventArgs(dashboardItemName);
				RequestDrillDownParameters(this, args);
				return args.Parameters;
			}
			return null;
		}
		internal IDrillDownController GetDrillDownController(string dashboardItemName) {
			if (RequestDrillDownController != null) {
				var args = new RequestDrillDownControllerEventArgs(dashboardItemName);
				RequestDrillDownController(this, args);
				return args.Controller;
			}
			return null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				items.CollectionChanged -= OnItemsChanged;
				groups.CollectionChanged -= OnGroupsChanged;
				dataSources.CollectionChanged -= OnDataSourcesChanged;
				parameters.CollectionChanged -= OnParametersChanged;
				ClearDashboard(true);
				ClearRuntimeNameGenerator();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnConfigureDataConnection(DashboardConfigureDataConnectionEventArgs eventArgs) {
			if(ConfigureDataConnection != null) 
				ConfigureDataConnection(this, eventArgs);
		}
		protected virtual void OnCustomFilterExpression(DashboardCustomFilterExpressionEventArgs eventArgs) {
			if(CustomFilterExpression != null) 
				CustomFilterExpression(this, eventArgs);
		}
		protected virtual void OnCustomParameters(CustomParametersEventArgs eventArgs) {
			if(CustomParameters != null)
				CustomParameters(this, eventArgs);
		}
		protected virtual void OnValidateCustomSqlQuery(ValidateDashboardCustomSqlQueryEventArgs eventArgs) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, eventArgs);
		}
		protected virtual void OnConnectionError(DashboardConnectionErrorEventArgs eventArgs) {
			if(ConnectionError != null) 
				ConnectionError(this, eventArgs);
		}
		protected virtual void OnDashboardLoading() {
			if(DashboardLoading != null)
				DashboardLoading(this, EventArgs.Empty);
		}
		protected virtual void OnDataLoading(DashboardDataLoadingEventArgs eventArgs) {
			if(DataLoading != null)
				DataLoading(this, eventArgs);
		}
		protected virtual void OnDataSourceCollectionChanged(NotifyingCollectionChangedEventArgs<IDashboardDataSource> e) {
			if(DataSourceCollectionChanged != null)
				DataSourceCollectionChanged(this, e);
			RaiseDashboardChanged();
		}
		protected virtual void OnGroupCollectionChanged(NotifyingCollectionChangedEventArgs<DashboardItemGroup> e) {
			if(GroupCollectionChanged != null)
				GroupCollectionChanged(this, e);
			RaiseDashboardChanged();
		}
		protected virtual void OnItemCollectionChanged(NotifyingCollectionChangedEventArgs<DashboardItem> e) {
			if(ItemCollectionChanged != null)
				ItemCollectionChanged(this, e);
			RaiseDashboardChanged();
		}
		void OnItemsChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItem> e) {
			BeginUpdate();
			try {
				foreach (DashboardItem item in e.RemovedItems)
					item.Dashboard = null;
				foreach (DashboardItem item in e.AddedItems) {
					if (item.IsGroup)
						throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectDashboardItemGroupAssign));
					item.Dashboard = this;
				}
				if (!Loading) {
					if (!IsVSDesignMode) {
						List<IDashboardDataSource> dataSourcesToAdd = new List<IDashboardDataSource>();
						foreach (DataDashboardItem dataDashboardItem in e.AddedItems.OfType<DataDashboardItem>()) {
							IDashboardDataSource dataSource = dataDashboardItem.DataSource;
							if (dataSource != null && !dataSourcesToAdd.Contains(dataSource) && !dataSources.Contains(dataSource))
								dataSourcesToAdd.Add(dataSource);
						}
						if (dataSourcesToAdd.Count > 0)
							dataSources.AddRange(dataSourcesToAdd);
					}
					OnItemCollectionChanged(e);
				}
			}
			finally {
				if (Loading)
					CancelUpdate();
				else
					EndUpdate();
			}
		}
		void OnGroupsChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItemGroup> e) {
			BeginUpdate();
			try {
				foreach (DashboardItemGroup group in e.RemovedItems)
					group.Dashboard = null;
				foreach (DashboardItemGroup group in e.AddedItems)
					group.Dashboard = this;
				if (!Loading) {
					if (!IsVSDesignMode)
						foreach (DashboardItem dashboardItem in items)
							if (dashboardItem.Group != null && e.RemovedItems.Contains(dashboardItem.Group))
								dashboardItem.Group = null;
					OnGroupCollectionChanged(e);
				}
			}
			finally {
				if (Loading)
					CancelUpdate();
				else
					EndUpdate();
			}
		}
		void OnParametersChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardParameter> e) {
			if (!Loading) {
				if (ParameterCollectionChanged != null)
					ParameterCollectionChanged(this, e);
				RaiseTitleChanged();
			}
		}
		void OnDataSourcesChanged(object sender, NotifyingCollectionChangedEventArgs<IDashboardDataSource> e) {
			if (!Loading && !IsVSDesignMode)
				SyncronizeDataConnections();
			foreach (IDashboardDataSource ds in e.RemovedItems) {
				ds.SetDashboard(null);
				ds.SetParameters(null);
				IDashboardDataSourceInternal dataSourceInternal = ds.GetDataSourceInternal();
				if (dataSourceInternal != null) {
					dataSourceInternal.DataSourceDataChanged -= OnDataSourceDataChanged;
					dataSourceInternal.NameChanged -= OnDataSourceNameChanged;
					dataSourceInternal.CaptionChanged -= OnDataSourceCaptionChanged;
					dataSourceInternal.DataProcessingModeChanged -= OnDataProcessingModeChanged;
					ICalculatedFieldsController calculatedFieldsController = ds.GetCalculatedFieldsController();
					if (calculatedFieldsController != null) {
						calculatedFieldsController.CalculatedFieldCollectionChanged -= OnDataSourceCalculatedFieldCollectionChanged;
						calculatedFieldsController.CalculatedFieldChanged -= OnDataSourceCalculatedFieldChanged;
						calculatedFieldsController.CalculatedFieldsCorrupted -= OnDataSourceCalculatedFieldsCorrupted;
					}
				}
			}
			foreach (IDashboardDataSource ds in e.AddedItems) {
				ds.SetDashboard(this);
				ds.SetParameters(Parameters);
				IDashboardDataSourceInternal dataSourceInternal = ds.GetDataSourceInternal();
				if (dataSourceInternal != null) {
					dataSourceInternal.DataSourceDataChanged += OnDataSourceDataChanged;
					dataSourceInternal.NameChanged += OnDataSourceNameChanged;
					dataSourceInternal.CaptionChanged += OnDataSourceCaptionChanged;
					dataSourceInternal.DataProcessingModeChanged += OnDataProcessingModeChanged;
					ICalculatedFieldsController calculatedFieldsController = ds.GetCalculatedFieldsController();
					if (calculatedFieldsController != null) {
						calculatedFieldsController.CalculatedFieldCollectionChanged += OnDataSourceCalculatedFieldCollectionChanged;
						calculatedFieldsController.CalculatedFieldChanged += OnDataSourceCalculatedFieldChanged;
						calculatedFieldsController.CalculatedFieldsCorrupted += OnDataSourceCalculatedFieldsCorrupted;
					}
				}
			}
			if (!IsDataSourceSyncLocked && !IsVSDesignMode)
				foreach (DataDashboardItem dashboardItem in items.OfType<DataDashboardItem>())
					if (dashboardItem.DataSource != null && e.RemovedItems.Contains(dashboardItem.DataSource))
						dashboardItem.DataSource = null;
			if (!Loading)
				OnDataSourceCollectionChanged(e);
		}
		void OnDataSourceCalculatedFieldsCorrupted(object sender, NotifyingCollectionChangedEventArgs<CalculatedField> e) {
			if(!Loading && DataSourceCalculatedFieldsCorrupted != null) { 
				ICalculatedFieldsController calcFields = (ICalculatedFieldsController)sender;
				DataSourceCalculatedFieldsCorrupted(this, new DataSourceCalcFieldCollectionChangedEventArgs(calcFields.DataSource, e));
			}
		}
		void OnDataSourceCalculatedFieldChanged(object sender, CalculatedFieldChangedEventArgs e) {
			if(!Loading && DataSourceCalculatedFieldChanged != null) {
				ICalculatedFieldsController calcFields = (ICalculatedFieldsController)sender;
				DataSourceCalculatedFieldChanged(this, new DataSourceCalculatedFieldChangedEventArgs(calcFields.DataSource, e));
			}
		}
		void OnDataSourceCalculatedFieldCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<CalculatedField> e) {
			if(!Loading && DataSourceCalculatedFieldCollectionChanged != null) {
				ICalculatedFieldsController calcFields = (ICalculatedFieldsController)sender;
				DataSourceCalculatedFieldCollectionChanged(this, new DataSourceCalcFieldCollectionChangedEventArgs(calcFields.DataSource, e));
			}
		}
		void OnDataSourceCaptionChanged(object sender, EventArgs e) {
			if(!Loading && DataSourceCaptionChanged != null)
				DataSourceCaptionChanged(this, new DataSourceChangedEventArgs((IDashboardDataSource)sender));
		}
		void OnDataSourceDataChanged(object sender, DataChangedEventArgs e) {
			IDashboardDataSource dataSource = sender as IDashboardDataSource;
			if(dataSource != null && DataSourceDataChanged != null)
				DataSourceDataChanged(this, new DataSourceChangedEventArgs(dataSource));
			RaiseDashboardChanged();
		}
		void OnDataProcessingModeChanged(object sender, DataProcessingModeChangedEventArgs e) {
			if(!Loading)
				FillDataSource(e.DataSource);
		}
		void OnDataSourceNameChanged(object sender, NameChangedEventArgs e) {
			if (!Loading && DataSourceComponentNameChanged != null)
				DataSourceComponentNameChanged(this, new ComponentNameChangedEventArgs(e.OldName, e.NewName));
		}
		void OnMasterFilterChanged(object sender, EventArgs e) {
			OnTitleChanged();
		}
		void OnLayoutChanged(object sender, EventArgs e) {
			RaiseLayoutChanged();
		}
		void RaiseEnableAutomaticUpdatesChanged(EnableAutomaticUpdatesEventArgs eventArgs) {
			if(EnableAutomaticUpdatesChanged != null)
				EnableAutomaticUpdatesChanged(this, eventArgs);
			if(enableAutomaticUpdates)
				RaiseDashboardChanged();
		}
		void RaiseLayoutChanged() {
			if (LayoutChanged != null)
				LayoutChanged(this, EventArgs.Empty);
			RaiseDashboardChanged();
		}
		void RaiseDashboardItemChanged(DashboardItem dashboardItem, ChangedEventArgs e) {
			if (DashboardItemChanged != null)
				DashboardItemChanged(this, new DashboardItemChangedEventArgs(dashboardItem, e));
			RaiseDashboardChanged();
		}
		void RaiseColoringChanged() {
			if (ColoringChanged != null)
				ColoringChanged(this, EventArgs.Empty);
			RaiseDashboardChanged();
		}
		void RaiseTitleChanged() {
			if (TitleChanged != null)
				TitleChanged(this, EventArgs.Empty);
			RaiseDashboardChanged();
		}
		void RaiseDashboardChanged() {
			if (!updateLocker.IsLocked) {
				RaiseOptionsChanged();
				if (DashboardChanged != null)
					DashboardChanged(this, EventArgs.Empty);
			}
		}
		void RaiseOptionsChanged() {
			LockUpdate();
			try {
				if (OptionsChanged != null)
					OptionsChanged(this, new DashboardOptionsChangedEventArgs());
			}
			finally {
				UnlockUpdate();
			}
		}
		void RaiseUpdateCanceled() {
			if (!updateLocker.IsLocked && UpdateCanceled != null)
				UpdateCanceled(this, EventArgs.Empty);
		}
		void SubscribeLayoutEvents() {
			if (layoutRoot != null)
				layoutRoot.Changed += OnLayoutChanged;
		}
		void UnsubscribeLayoutEvents() {
			if (layoutRoot != null)
				layoutRoot.Changed -= OnLayoutChanged;
		}
		void SaveDesignerAutomaticUpdates(XElement rootElement) {
			if(EnableAutomaticUpdates!= DefaultDesignerAutomaticUpdates)
				rootElement.Add(new XAttribute(xmlDesignerAutomaticUpdates, EnableAutomaticUpdates));
		}
		void SaveCurrencyCultureToXml(XElement rootElement) {
			if (currencyCultureName != null)
				rootElement.Add(new XAttribute(xmlCurrencyCulture, currencyCultureName));
		}
		void SaveTitleToXml(XElement rootElement) {
			XElement titleElement = new XElement(xmlTitle);
			title.SaveToXml(titleElement);
			rootElement.Add(titleElement);
		}
		void SaveLayoutToXml(XElement rootElement) {
			if (layoutRoot != null)
				rootElement.Add(DashboardLayoutSerializer.SaveLayoutToXml(layoutRoot));
		}
		void SaveParametersToXml(XElement rootElement) {
			if (parameters.Count > 0)
				parameters.SaveToXml(rootElement);
		}
		void LoadCurrencyCultureFromXml(XElement rootElement) {
			string restoredCurrencyCultureName = XmlHelper.GetAttributeValue(rootElement, xmlCurrencyCulture);
			SetCurrencyCultureName(restoredCurrencyCultureName, false);
		}
		void LoadDesignerAutomaticUpdatesFromXml(XElement rootElement) {
			string designerAutomaticUpdatesAttr = XmlHelper.GetAttributeValue(rootElement, xmlDesignerAutomaticUpdates);
			if(!string.IsNullOrEmpty(designerAutomaticUpdatesAttr))
				enableAutomaticUpdates = XmlHelper.FromString<bool>(designerAutomaticUpdatesAttr);			
		}
		void LoadTitleFromXml(XElement rootElement) {
			XElement titleElement = rootElement.Element(xmlTitle);
			if (titleElement != null)
				title.LoadFromXml(titleElement);
		}
		void LoadLayoutFromXml(XElement rootElement) {
			XElement layoutElement = rootElement.Element(xmlLayout);
			if (layoutElement != null) {
				List<DashboardFlatLayoutItem> flatLayoutItems = new List<DashboardFlatLayoutItem>();
				foreach (XElement layoutItemElement in layoutElement.Elements(DashboardFlatLayoutItem.XmlLayoutItem))
					flatLayoutItems.Add(DashboardFlatLayoutItem.LoadFromXml(layoutItemElement, items));
				if (flatLayoutItems.Count != items.Count)
					throw new XmlException();
				if (flatLayoutItems.Count > 0)
					layoutRoot = new FlatLayoutConverter(flatLayoutItems).LayoutRoot;
			}
			XElement layoutTreeElement = rootElement.Element(DashboardLayoutSerializer.XmlLayoutTree);
			if (layoutTreeElement != null)
				LayoutRoot = (DashboardLayoutGroup)DashboardLayoutSerializer.LoadLayoutFromXml(layoutTreeElement, new DashboardLayoutFactory());
		}
		void LoadParametersFromXml(XElement rootElement) {
			parameters.LoadFromXml(rootElement);
		}
		void EndInit(DashboardState state, bool isXmlLoading) {
			filterGroup.EndUpdate();
			if (layoutRoot != null)
				layoutRoot.OnEndLoading(GetItemsAndGroups());
#if !DXPORTABLE
			foreach (IDashboardDataSource ds in dataSources) {
#pragma warning disable 0618
				DataSource dataSource = ds as DataSource;
#pragma warning restore 0618
				if (dataSource != null)
					dataSource.SetConnection(dataConnections);
			}	
#endif
			foreach (DashboardParameter dashboardParameter in Parameters)
				dashboardParameter.DataBind(DataSources);
			if (isXmlLoading) {
				foreach (DashboardItem item in items)
					item.EnsureConsistecy(this);
				foreach (ColorSchemeEntry entry in ColorScheme)
					entry.EnsureConsistency(this);
			}
			colorSchemeContainer.OnEndLoading();
			foreach (DataDashboardItem item in InteractivityOrderedItems.OfType<DataDashboardItem>())
				item.UpdateDataMembersOnLoading();
			EndLoading();
			if (LoadCompleted != null)
				LoadCompleted(this, EventArgs.Empty);
		}
		void LockUpdate() {
			updateLocker.Lock();
		}
		void UnlockUpdate() {
			updateLocker.Unlock();
		}
		static XElement GetDashboardRootNode(Stream stream) {
			return XmlHelper.LoadXmlFromStream(stream, xmlDashboard);
		}
		static XElement GetUserData(XElement rootElement) {
			XElement userDataContainer = rootElement.Element(xmlUserData);
			if (userDataContainer != null)
				return userDataContainer.Elements().First();
			else
				return null;
		}
		static void SetUserDataElement(XElement rootElement, XElement userData) {
			XElement userDataContainer = rootElement.Element(xmlUserData);
			if (userDataContainer != null)
				userDataContainer.Remove();
			if (userData != null)
				rootElement.Add(new XElement(xmlUserData, userData));
		}
		void ClearRuntimeNameGenerator() {
			if(runtimeNameGenerator != null) {
				runtimeNameGenerator.Dispose();
				runtimeNameGenerator = null;
			}
		}	   
		IList<DashboardItem> GetItemsAndGroups() {
			List<DashboardItem> itemsAndGroups = new List<DashboardItem>(items);
			itemsAndGroups.AddRange(groups);
			return itemsAndGroups;
		}
		IFilterGroup CreateFilterGroup() {
			IFilterGroup group = new FilterGroup();
			group.InputFilter = dashboardFilter;
			group.FilterChanged += OnMasterFilterChanged;
			return group;
		}
		DashboardTitle CreateDashboardTitle() {
			return new DashboardTitle(this);
		}
		void ClearDashboard(bool isLoadXml) {
			filterGroup = CreateFilterGroup();
			if (isLoadXml)
				filterGroup.BeginUpdate();
			layoutRoot = null;
			ColorSchemeContainer.Reset();
			List<DashboardItem> itemsToDispose = new List<DashboardItem>(items);
			items.Clear();
			foreach (DashboardItem itemToDispose in itemsToDispose)
				itemToDispose.Dispose();
			List<DashboardItemGroup> groupsToDispose = new List<DashboardItemGroup>(groups);
			groups.Clear();
			foreach (DashboardItemGroup groupToDispose in groupsToDispose)
				groupToDispose.Dispose();
			List<IDashboardDataSource> dataSourcesToDispose = new List<IDashboardDataSource>(dataSources);
			dataSources.Clear();
			foreach (IDashboardDataSource dataSourceToDispose in dataSourcesToDispose)
				dataSourceToDispose.Dispose();
			title = CreateDashboardTitle();
			parameters.Clear();
		}
		void SetCurrencyCultureName(string currencyCultureName, bool throwException) {
			if(currencyCultureName == null) {
				this.currencyCultureName = null;
			}
			else {
				try {
					CultureInfoExtensions.CreateSpecificCulture(currencyCultureName);
				}
				catch(Exception e) {
					if(throwException)
						throw new InvalidCultureNameException(currencyCultureName, e);
					this.currencyCultureName = null;
				}
				this.currencyCultureName = currencyCultureName;
			}
		}
		#region IServiceProvider
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		#endregion
		#region IChangeService
		event EventHandler<ChangedEventArgs> IChangeService.Changed { add { } remove { } }
		void IChangeService.OnChanged(ChangedEventArgs e) {
			if (e.Reason == ChangeReason.Coloring)
				RaiseColoringChanged();
			else {
				BeginUpdate();
				try {
					foreach (DashboardItem item in items)
						RaiseDashboardItemChanged(item, e);
				}
				finally {
					EndUpdate();
				}
			}
		}
		#endregion
		#region IFilterGroupOwner
		IFilterGroup IFilterGroupOwner.FilterGroup { get { return filterGroup; } }
		#endregion
		#region IFiltersProvider implementation
		IEnumerable<string> IFiltersProvider.FilterItemNames { get { return filterGroup.GetFilterValuesProviders(null).Cast<DataDashboardItem>().Select(item => item.ComponentName); } }
		#endregion
		#region IColorSchemeContext
		bool IColorSchemeContext.Loading { get { return Loading; } }
		IChangeService IColorSchemeContext.ChangeService { get { return this; } }
		bool IColorSchemeContext.IsChangeLocked { get { return false; } }
		#endregion
		#region IActualParametersProvider
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			return parameters;
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			throw new InvalidOperationException();
		}
		#endregion
		#region IDataSourceInfoProvider
		DataSourceInfo IDataSourceInfoProvider.GetDataSourceInfo(string dataSourceName, string dataMember) {
			IDashboardDataSource dataSource = DataSources.FirstOrDefault(_dataSource => _dataSource.ComponentName == dataSourceName);
			if (dataSource != null)
				return new DataSourceInfo(dataSource, dataMember);
			return null;
		}
		IList<DataSourceInfo> IDataSourceInfoProvider.GetAllDataSourceInfo() {
			return DataSources.SelectMany(ds => ds.GetDataSets().Select(s => new DataSourceInfo(ds, s))).ToList();
		}
		#endregion
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class CollectionPrefixCaptionGenerator<T> : ObjectPrefixNameGeneratorBase<T>, IDisposable where T : class, IDashboardComponent, ISupportPrefix {
		const string DefaultSeparator = " ";
		readonly NotifyingCollection<T> collection;
		protected override IEnumerable<string> Names { get { return collection.Select(item=> item.Name); } }
		public CollectionPrefixCaptionGenerator(NotifyingCollection<T> collection)
			: this(collection, DefaultSeparator) {
		}
		public CollectionPrefixCaptionGenerator(NotifyingCollection<T> collection, string separator)
			: base(separator) {
				this.collection = collection;			
			   SubscribeCollectionEvents();
		}
		public void Dispose() {
			UnsubscribeCollectionEvents();			
		}
		string GetName(T item) {
			return item.Name;
		}
		void SetName(T item, string name) {
			item.Name = name;
		}
		void SubscribeCollectionEvents() {
			collection.BeforeItemAdded += OnBeforeItemAdded;		
		}
		void UnsubscribeCollectionEvents() {
			collection.BeforeItemAdded -= OnBeforeItemAdded;
		}   
		void OnBeforeItemAdded(object sender, NotifyingCollectionBeforeItemAddedEventArgs<T> e) {
			string name = GetName(e.Item);
			if(string.IsNullOrEmpty(name))
				SetName(e.Item, GenerateName(e.Item));			
		}
	}
}
