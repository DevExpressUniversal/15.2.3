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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	public class DataSourceSelectorPresenter : IDisposable {
		IServiceProvider serviceProvider;
		IDataSourceSelectorView view;
		public IDataSourceSelectorView View {
			get { return view; }
			set {
				if (value != view) {
					UnsubscribeViewEvents();
					view = value;
					SubscribeViewEvents();
					RefreshView();
					SelectDefaultDataSource();
				}
			}
		}
		public DataSourceInfo SelectedDataSourceInfo {
			get {
				if (view != null)
					return new DataSourceInfo(view.SelectedDataSource, view.SelectedDataMember);
				return null;
			}
			set {
				if (view != null) {
					if (value != SelectedDataSourceInfo) {
						Dashboard dashboard = Dashboard;
						if (dashboard != null && value != null) {
							if (!value.Equals(SelectedDataSourceInfo)) {
								if (value.DataSource == null || dashboard.DataSources.Contains(value.DataSource)) {
									SetDataSourceInfo(value.DataSource, value.DataMember);
									RaiseSelectionChanged();
								}
							}
						}
						else {
							SetDataSourceInfo(null, null);
							RaiseSelectionChanged();
						}
					}
				}
			}
		}
		Dashboard Dashboard {
			get {
				IDashboardOwnerService ownerService = serviceProvider.RequestService<IDashboardOwnerService>();
				if (ownerService != null)
					return ownerService.Dashboard;
				return null;
			}
		}
		bool AllowUpdate {
			get {
				if(serviceProvider != null)
					return !serviceProvider.RequestServiceStrictly<IDesignerUpdateService>().Suspended;
				return true;
			}
		}
		public event EventHandler<DataSourceSelectedEventArgs> DataSourceSelected;
		public void Initialize(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			if (this.serviceProvider == null) {
				this.serviceProvider = serviceProvider;
				SubscribeServiceEvents();
				SubscribeDashboardEvents(Dashboard);
				RefreshView();
				SelectDefaultDataSource();
			}
			else
				throw new InvalidOperationException();
		}
		public void Dispose() {
			UnsubscribeServiceEvents();
			UnsubscribeViewEvents();
			UnsubscribeDashboardEvents(Dashboard);
		}
		void SubscribeViewEvents() {
			if(view != null) {
				view.SelectedDataSourceChanged += OnSelectedDataSourceChanged;
				view.SelectedDataMemberChanged += OnSelectedDataMemberChanged;
			}
		}
		void UnsubscribeViewEvents() {
			if(view != null) {
				view.SelectedDataSourceChanged -= OnSelectedDataSourceChanged;
				view.SelectedDataMemberChanged -= OnSelectedDataMemberChanged;
			}
		}
		void SubscribeServiceEvents() {
			IDashboardLoadingService loadingService = serviceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardLoad += OnDashboardLoad;
			loadingService.DashboardUnload += OnDashboardUnload;
			loadingService.DashboardEndInitialize += OnDashboardEndInitialize;
			IDashboardDesignerSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			selectionService.DashboardItemSelected += OnDashboardItemSelected;
		}
		void UnsubscribeServiceEvents() {
			if (serviceProvider != null) {
				IDashboardLoadingService loadingService = serviceProvider.RequestService<IDashboardLoadingService>();
				if (loadingService != null) {
					loadingService.DashboardLoad -= OnDashboardLoad;
					loadingService.DashboardUnload -= OnDashboardUnload;
					loadingService.DashboardEndInitialize -= OnDashboardEndInitialize;
				}
				IDashboardDesignerSelectionService selectionService = serviceProvider.RequestService<IDashboardDesignerSelectionService>();
				if (selectionService != null)
					selectionService.DashboardItemSelected -= OnDashboardItemSelected;
			}
		}
		void SubscribeDashboardEvents(Dashboard dashboard) {
			if (dashboard != null) {
				dashboard.DataSourceCollectionChanged += OnDashboardDataSourceCollectionChanged;
				dashboard.DashboardItemChanged += OnDashboardItemChanged;
				dashboard.DataSourceDataChanged += OnDashboardDataSourceDataChanged;
				dashboard.DataSourceCaptionChanged += OnDashboardDataSourceCaptionChanged;
			}
		}
		void UnsubscribeDashboardEvents(Dashboard dashboard) {
			if (dashboard != null) {
				dashboard.DataSourceCollectionChanged -= OnDashboardDataSourceCollectionChanged;
				dashboard.DashboardItemChanged -= OnDashboardItemChanged;
				dashboard.DataSourceDataChanged -= OnDashboardDataSourceDataChanged;
				dashboard.DataSourceCaptionChanged -= OnDashboardDataSourceCaptionChanged;
			}
		}
		void OnSelectedDataSourceChanged(object sender, EventArgs e) {
			RefreshDataMembers();
			RaiseSelectionChanged();
		}
		void OnSelectedDataMemberChanged(object sender, EventArgs e) {
			RaiseSelectionChanged();
		}
		void OnDashboardLoad(object sender, DashboardLoadingEventArgs e) {
			SubscribeDashboardEvents(e.Dashboard);
		}
		void OnDashboardUnload(object sender, DashboardLoadingEventArgs e) {
			UnsubscribeDashboardEvents(e.Dashboard);
		}
		void OnDashboardEndInitialize(object sender, EventArgs e) {
			RefreshView();
			SelectFirstDataSource();
		}		
		void OnDashboardItemSelected(object sender, DashboardItemSelectedEventArgs e) {
			SelectDataSource(e.SelectedDashboardItem, false);
		}
		void OnDashboardDataSourceCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<IDashboardDataSource> e) {
			if(AllowUpdate) {
				RefreshView();
				if(e.AddedItems.Count > 0)
					SelectDataSource(e.AddedItems[e.AddedItems.Count - 1]);
				else if(SelectedDataSourceInfo != null && e.RemovedItems.Contains(SelectedDataSourceInfo.DataSource))
					SelectFirstDataSource();
			}
		}
		void OnDashboardItemChanged(object sender, DashboardItemChangedEventArgs e) {
			if(AllowUpdate) {
				IDashboardDesignerSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
				if(selectionService.SelectedDashboardItem == e.DashboardItem && e.InnerArgs.Reason == ChangeReason.RawData)
					SelectDataSource(e.DashboardItem, false);
			}
		}
		void OnDashboardDataSourceCaptionChanged(object sender, DataSourceChangedEventArgs e) {
			if(view != null && AllowUpdate && e.DataSource == view.SelectedDataSource)
				view.RenameSelectedItem();
		}
		void OnDashboardDataSourceDataChanged(object sender, DataSourceChangedEventArgs e) {
			if(view != null && AllowUpdate && e.DataSource == view.SelectedDataSource) {
				RefreshDataMembers();
				RaiseSelectionChanged();
			}
		}
		void RaiseSelectionChanged() {
			if(DataSourceSelected != null)
				DataSourceSelected(this, new DataSourceSelectedEventArgs(SelectedDataSourceInfo));
			IDashboardDesignerUpdateUIService uiService = serviceProvider.RequestServiceStrictly<IDashboardDesignerUpdateUIService>();
			uiService.OnUpdateUI();
		}
		void SetDataSourceInfo(IDashboardDataSource dataSource, string dataMember) {
			view.BeginUpdate();
			view.SelectedDataSource = dataSource;
			RefreshDataMembers();
			view.SelectedDataMember = dataMember;
			view.EndUpdate();
		}
		void SelectDefaultDataSource() {
			if (view != null) {
				IDashboardDesignerSelectionService selectionService = null;
				if (serviceProvider != null)
					selectionService = serviceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
				if (selectionService != null)
					SelectDataSource(selectionService.SelectedDashboardItem, true);
			}
		}
		void SelectDataSource(DashboardItem dashboardItem, bool allowFirst) {
			DataDashboardItem dataDashboardItem = dashboardItem as DataDashboardItem;
			if (dataDashboardItem != null && dataDashboardItem.DataSource != null)
				SelectDataSource(dataDashboardItem.DataSource, dataDashboardItem.DataMember);
			else if (allowFirst)
				SelectFirstDataSource();
		}
		void SelectFirstDataSource() {
			Dashboard dashboard = Dashboard;
			if(dashboard != null && dashboard.DataSources.Count > 0)
				SelectDataSource(dashboard.DataSources[0]);
			else
				SelectedDataSourceInfo = null;
		}
		void SelectDataSource(IDashboardDataSource dataSource) {
			SelectDataSource(dataSource, dataSource.GetDataSets().FirstOrDefault());
		}
		void SelectDataSource(IDashboardDataSource dataSource, string dataMember) {
			SelectedDataSourceInfo = new DataSourceInfo(dataSource, dataMember);
		}
		void RefreshView() {
			if (view != null) {
				Dashboard dashboard = Dashboard;
				if(dashboard != null) {
					view.BeginUpdate();
					view.RefreshDataSources(dashboard.DataSources);
					RefreshDataMembers();
					view.EndUpdate();
				}
			}
		}
		int GetImageIndex(string dataMember) {
			IDashboardDataSource dataSource = SelectedDataSourceInfo.DataSource;
			if(dataSource != null && dataSource.IsServerModeSupported && dataSource.DataProcessingMode == DataProcessingMode.Server) {
				return dataSource.IsSqlServerMode(dataMember) ? 1 : 0;
			}
			return -1;
		}
		void RefreshDataMembers() {
			IDashboardDataSource dataSource = view.SelectedDataSource;
			string selectedDataMember = view.SelectedDataMember;
			List<ImageComboBoxItem> dataMembers = new List<ImageComboBoxItem>();
			if(dataSource != null && dataSource.GetIsMultipleDataMemberSupported())
				foreach(string dataMember in dataSource.GetDataSets())
					dataMembers.Add(new ImageComboBoxItem(dataMember, GetImageIndex(dataMember)));
			view.RefreshDataMembers(dataMembers.OrderBy(dataMember => dataMember.Value));
			if(selectedDataMember != null && dataMembers.Exists(item => item.Value.ToString() == selectedDataMember))
				view.SelectedDataMember = selectedDataMember;
		}
	}
	public class DataSourceSelectedEventArgs : EventArgs {
		public IDashboardDataSource DataSource { get; private set; }
		public string DataMember { get; private set; }
		public DataSourceSelectedEventArgs(DataSourceInfo dataSourceInfo)
			: this(dataSourceInfo != null ? dataSourceInfo.DataSource : null, dataSourceInfo != null ? dataSourceInfo.DataMember : null) {
		}
		public DataSourceSelectedEventArgs(IDashboardDataSource dataSource, string dataMember) {
			DataSource = dataSource;
			DataMember = dataMember;
		}
	}
}
