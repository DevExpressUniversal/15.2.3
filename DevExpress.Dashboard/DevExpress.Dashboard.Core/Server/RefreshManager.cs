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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Server {
	public enum DataLoadingStrategy {
		None,
		RequireFill,
		DataIsReady,
	}
	[Flags]
	public enum GeneralRefreshType {
		None = 0,
		Layout = 1,
		Title = 2,
		Parameters = 4,
		Coloring = 8
	}
	public class RefreshManager : IDisposable {
		static ContentType ChangeReasonToContentType(ChangeReason reason) {
			switch(reason) {
				case ChangeReason.InteractivityCrossDataSource:
					return ContentType.ActionModel;
				case ChangeReason.Caption:
					return ContentType.CaptionViewModel;
				case ChangeReason.View:
				case ChangeReason.MapFile:
					return ContentType.ViewModel;
				case ChangeReason.Interactivity:
				case ChangeReason.ClientData:
				case ChangeReason.DashboardItemLoadCompleted:
					return ContentType.FullContent;
				default:
					return ContentType.FullContentNoActionModel;
			}
		}
		readonly Locker serviceOperationLocker = new Locker();
		readonly Dictionary<string, ContentType> itemsToRefresh = new Dictionary<string, ContentType>();
		readonly Dictionary<string, DataLoadingStrategy> dataSourcesToLoad = new Dictionary<string, DataLoadingStrategy>();
		readonly List<string> detailItems = new List<string>();
		GeneralRefreshType generalRefreshType = GeneralRefreshType.None;
		Dashboard dashboard;
		public bool DeferUpdates { get; set; }
		public GeneralRefreshType GeneralRefreshType { get { return generalRefreshType; } }
		public bool IsEmpty {
			get {
				return (itemsToRefresh.Count == 0 && dataSourcesToLoad.Count == 0 && generalRefreshType == Server.GeneralRefreshType.None ) 
					||  DeferUpdates;
			}
		}
		public List<string> DetailItems { get { return detailItems; } }
		public RefreshManager() {
		}
		public RefreshManager(Dashboard dashboard) {
			Initialize(dashboard, false);
		}
		public void Initialize(Dashboard dashboard, bool IsDesignMode) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			if (this.dashboard != null)
				throw new InvalidOperationException();
			this.dashboard = dashboard;
			dashboard.DashboardItemChanged += OnDashboardItemChanged;
			dashboard.ItemCollectionChanged += OnItemCollectionChanged;
			dashboard.GroupCollectionChanged += OnGroupCollectionChanged;
			dashboard.DataSourceCollectionChanged += OnDataSourceCollectionChanged;
			dashboard.RequestDataSourceFill += OnRequestDataSourceFill;
			dashboard.DataSourceDataChanged += OnDataSourceDataChanged;
			dashboard.LayoutChanged += OnLayoutChanged;
			dashboard.TitleChanged += OnTitleChanged;
			dashboard.ColoringChanged += OnColoringChanged;
			dashboard.UpdateCanceled += OnDashboardUpdateCanceled;
			dashboard.DashboardItemComponentNameChanged += OnDashboardItemComponentNameChanged;
			dashboard.DataSourceComponentNameChanged += OnDataSourceComponentNameChanged;
			dashboard.ParameterCollectionChanged += OnParametersCollectionChanged;
			dashboard.EnableAutomaticUpdatesChanged += OnDesignerAutomaticUpdatesChanged;
			if(!dashboard.EnableAutomaticUpdates && IsDesignMode)
				SetAllDataForDeferUpdates();			
		}
		void SetAllDataSourcesToRefresh() {
			foreach(IDashboardDataSource dataSource in dashboard.DataSources)
				SetDataSourceLoadingStrategy(dataSource.ComponentName, DataLoadingStrategy.RequireFill);
		}
		void SetAllItemsToRefresh() {
			foreach(DataDashboardItem item in dashboard.Items.OfType<DataDashboardItem>())
				SetItemToRefresh(item.ComponentName, ContentType.FullContent);
		}
		void OnDesignerAutomaticUpdatesChanged(object sender, EnableAutomaticUpdatesEventArgs e) {
			if(e.EnableAutomaticUpdates) 
				SetAllItemsToRefresh();			
			DeferUpdates = !e.EnableAutomaticUpdates;
		}
		void OnParametersCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardParameter> e) {
			AddParametersToRefresh();
		}
		public void SetAllDataForDeferUpdates() {
			DeferUpdates = true;
			SetAllItemsToRefresh();
			SetAllDataSourcesToRefresh();
		}
		public void Dispose() {
			if (dashboard != null) {
				dashboard.DashboardItemChanged -= OnDashboardItemChanged;
				dashboard.ItemCollectionChanged -= OnItemCollectionChanged;
				dashboard.GroupCollectionChanged -= OnGroupCollectionChanged;
				dashboard.DataSourceCollectionChanged -= OnDataSourceCollectionChanged;
				dashboard.RequestDataSourceFill -= OnRequestDataSourceFill;
				dashboard.DataSourceDataChanged -= OnDataSourceDataChanged;
				dashboard.LayoutChanged -= OnLayoutChanged;
				dashboard.TitleChanged -= OnTitleChanged;
				dashboard.ColoringChanged -= OnColoringChanged;
				dashboard.UpdateCanceled -= OnDashboardUpdateCanceled;
				dashboard.DashboardItemComponentNameChanged -= OnDashboardItemComponentNameChanged;
				dashboard.DataSourceComponentNameChanged -= OnDataSourceComponentNameChanged;
				dashboard.ParameterCollectionChanged -= OnParametersCollectionChanged;
				dashboard.EnableAutomaticUpdatesChanged -= OnDesignerAutomaticUpdatesChanged;
				dashboard = null;
			}
		}
		public void Clear() {
			Clear(false);
		}
		public void Clear(bool forceClear) 
		{
			if(!DeferUpdates || forceClear) {
				itemsToRefresh.Clear();
				detailItems.Clear();
				dataSourcesToLoad.Clear();
				generalRefreshType = GeneralRefreshType.None;
			}
		}
		public ContentType GetItemContentType(string itemName) {
			ContentType contentType;
			return itemsToRefresh.TryGetValue(itemName, out contentType) ? contentType : ContentType.Empty;
		}
		public DataLoadingStrategy GetDataLoadingStrategy(string dataSourceName) {
			DataLoadingStrategy strategy;
			return dataSourcesToLoad.TryGetValue(dataSourceName, out strategy) ? strategy : DataLoadingStrategy.None;
		}
		public void BeginServiceOperation() {
			serviceOperationLocker.Lock();
		}
		public void EndServiceOperation() {
			serviceOperationLocker.Unlock();
		}
		void SetItemToRefresh(string componentName, ContentType contentType) {
			ContentType prevContentType = ContentType.Empty;
			bool found = itemsToRefresh.TryGetValue(componentName, out prevContentType);
			ContentType newContentType = prevContentType | contentType;
			if(found)
				itemsToRefresh[componentName] = newContentType;
			else
				itemsToRefresh.Add(componentName, newContentType);
		}
		void SetDataSourceLoadingStrategy(string dataSourceName, DataLoadingStrategy dataLoadingStrategy) {						
			if(dataSourcesToLoad.ContainsKey(dataSourceName))
				dataSourcesToLoad[dataSourceName] = dataLoadingStrategy;
			else
				dataSourcesToLoad.Add(dataSourceName, dataLoadingStrategy);
		}
		void OnLayoutChanged(object sender, EventArgs e) {
			AddLayoutToRefresh();
		}
		void OnTitleChanged(object sender, EventArgs e) {
			if (!serviceOperationLocker.IsLocked)
				AddTitleToRefresh();
		}
		void OnColoringChanged(object sender, EventArgs e) {
			AddColoringToRefresh(null);
		}
		void OnDashboardItemChanged(object sender, DashboardItemChangedEventArgs e) {
			if (!serviceOperationLocker.IsLocked) {
				if (e.InnerArgs.Reason != ChangeReason.DashboardItemGroup) {
					DataDashboardItem dataDashboardItem = e.DashboardItem as DataDashboardItem;
					if (dataDashboardItem != null) {
						if (e.InnerArgs.Reason == ChangeReason.UseGlobalColoring ||
							dataDashboardItem.IsGloballyColored &&
							(e.InnerArgs.Reason == ChangeReason.Coloring || e.InnerArgs.Reason == ChangeReason.ClientData || e.InnerArgs.Reason == ChangeReason.RawData ||
							e.InnerArgs.Reason == ChangeReason.DashboardItemLoadCompleted || e.InnerArgs.Reason == ChangeReason.Interactivity)) {
							AddColoringToRefresh(new[] { dataDashboardItem });
						}
						if (e.InnerArgs.Reason == ChangeReason.RawData ||e.InnerArgs.Reason == ChangeReason.ClientData || e.InnerArgs.Reason == ChangeReason.Interactivity || 
							e.InnerArgs.Reason == ChangeReason.InteractivityCrossDataSource) {
							foreach (string detailName in dataDashboardItem.GetAffectedDashboardItemsByMasterFilterActions()) {
								SetItemToRefresh(detailName, ContentType.FullContentNoActionModel);
								detailItems.Add(detailName);
							}
						}
						if (e.InnerArgs.Reason == ChangeReason.Interactivity || (dataDashboardItem.IsMasterFilterEnabled && e.InnerArgs.Reason == ChangeReason.ClientData))
							AddTitleToRefresh();
					}
					SetItemToRefresh(e.DashboardItem.ComponentName, ChangeReasonToContentType(e.InnerArgs.Reason));
				}
				else
					AddLayoutToRefresh();
			}				
		}		
		void OnItemCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItem> e) {
			foreach (DashboardItem removedItem in e.RemovedItems)
				itemsToRefresh.Remove(removedItem.ComponentName);
			foreach(DashboardItem addedItem in e.AddedItems)
				SetItemToRefresh(addedItem.ComponentName, ContentType.FullContent);
			if(e.AddedItems.Union(e.RemovedItems).OfType<DataDashboardItem>().Any(item => item.IsGloballyColored))
				AddColoringToRefresh(e.AddedItems);
			AddLayoutToRefresh();
		}
		void OnGroupCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItemGroup> e) {
			foreach (DashboardItemGroup removedGroup in e.RemovedItems)
				itemsToRefresh.Remove(removedGroup.ComponentName);
			foreach(DashboardItemGroup addedGroup in e.AddedItems)
				SetItemToRefresh(addedGroup.ComponentName, ContentType.FullContent);
			AddLayoutToRefresh();
		}
		void OnDataSourceCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<IDashboardDataSource> e) {
			foreach (IDashboardDataSource dataSource in e.RemovedItems)
				dataSourcesToLoad.Remove(dataSource.ComponentName);
			if (dashboard.IsVSDesignMode)
				foreach (IDashboardDataSource dataSource in e.AddedItems)
					dataSourcesToLoad[dataSource.ComponentName] = DataLoadingStrategy.RequireFill;
		}
		void OnRequestDataSourceFill(object sender, DataSourceChangedEventArgs e) {
			string dsComponentName = e.DataSource.ComponentName;
			dataSourcesToLoad[dsComponentName] = DataLoadingStrategy.RequireFill; 
			UpdateDashboadItemsByDataSource(dsComponentName);
		}
		void OnDataSourceDataChanged(object sender, DataSourceChangedEventArgs e) {
			if (!serviceOperationLocker.IsLocked) {
				string dsComponentName = e.DataSource.ComponentName;
				if (dashboard.IsVSDesignMode)
					dataSourcesToLoad[dsComponentName] = DataLoadingStrategy.RequireFill; 
				else
					dataSourcesToLoad[dsComponentName] = DataLoadingStrategy.DataIsReady;
				UpdateDashboadItemsByDataSource(dsComponentName);
			}
		}
		void OnDashboardUpdateCanceled(object sender, EventArgs e) {
			Clear();
		}
		void OnDashboardItemComponentNameChanged(object sender, ComponentNameChangedEventArgs e) {
			ContentType contentType;
			if (itemsToRefresh.TryGetValue(e.OldComponentName, out contentType)) {
				itemsToRefresh.Remove(e.OldComponentName);
				itemsToRefresh.Add(e.NewComponentName, contentType);
			}
		}
		void OnDataSourceComponentNameChanged(object sender, ComponentNameChangedEventArgs e) {
			DataLoadingStrategy strategy;
			if (dataSourcesToLoad.TryGetValue(e.OldComponentName, out strategy)) {
				dataSourcesToLoad.Remove(e.OldComponentName);
				dataSourcesToLoad.Add(e.NewComponentName, strategy);
			}
		}
		void UpdateDashboadItemsByDataSource(string dsComponentName) {
			List<string> dashboardItemsToRefresh = new List<string>();
			foreach (DataDashboardItem dataDashboardItem in dashboard.Items.OfType<DataDashboardItem>()) {
				if (dataDashboardItem.DataSource != null) {
					if (dataDashboardItem.DataSource.ComponentName == dsComponentName) {
						dashboardItemsToRefresh.Add(dataDashboardItem.ComponentName);
						dashboardItemsToRefresh.AddRange(dataDashboardItem.GetAffectedDashboardItemsByMasterFilterActions());
					}
				}
			}
			foreach (string dashboadItemComponentName in dashboardItemsToRefresh.Distinct())
				SetItemToRefresh(dashboadItemComponentName, ContentType.FullContent);
		}		
		void AddLayoutToRefresh() {
			generalRefreshType |= Server.GeneralRefreshType.Layout;
		}
		void AddTitleToRefresh() {
			generalRefreshType |= Server.GeneralRefreshType.Title;
		}
		void AddParametersToRefresh() {
			generalRefreshType |= Server.GeneralRefreshType.Parameters;
		}
		void AddColoringToRefresh(IEnumerable<DashboardItem> excludedItems) {
			generalRefreshType |= Server.GeneralRefreshType.Coloring;
			foreach (var dashboardItem in dashboard.Items.OfType<DataDashboardItem>().Where(item => item.IsGloballyColored))
				if (excludedItems == null || !excludedItems.Contains(dashboardItem))
					SetItemToRefresh(dashboardItem.ComponentName, ContentType.FullContent);
		}
	}
}
