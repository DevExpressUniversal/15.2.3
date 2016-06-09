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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Server {
	public class InteractivityManager : IDisposable {
		public static IInteractivitySession CreateSession(DashboardItem dashboardItem) {			
			RangeFilterDashboardItem rangeFilter = dashboardItem as RangeFilterDashboardItem;
			if (rangeFilter != null)
				return new RangeFilterInteractivitySession(rangeFilter);
			FilterElementDashboardItem filterElement = dashboardItem as FilterElementDashboardItem;
			if (filterElement != null) {
				if (filterElement.IsSingleSelection)
					return new FilterElementSingleInteractivitySession(filterElement);
				else
					return new FilterElementMultipleInteractivitySession(filterElement);
			}
			GeoPointMapDashboardItemBase geoPointMap = dashboardItem as GeoPointMapDashboardItemBase;
			if(geoPointMap != null)
				return new GeoPointMapInteractivitySession(geoPointMap);
			DataDashboardItem dataDashboardItem = dashboardItem as DataDashboardItem;
			if (dataDashboardItem != null)
				return new InteractivitySession(dataDashboardItem);
			return null;
		}
		readonly Locker serviceOperationLocker = new Locker();
		readonly Dictionary<string, IInteractivitySession> sessions = new Dictionary<string, IInteractivitySession>();
		Dashboard dashboard;
		public event EventHandler<SingleFilterDefaultValueEventArgs> SingleFilterDefaultValue;
		public event EventHandler<FilterElementDefaultValuesEventArgs> FilterElementDefaultValues;
		public event EventHandler<RangeFilterDefaultValueEventArgs> RangeFilterDefaultValue;
		public void Initialize(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			if (this.dashboard != null)
				throw new InvalidOperationException();
			this.dashboard = dashboard;
			dashboard.DashboardItemChanged += OnDashboardItemChanged;
			dashboard.ItemCollectionChanged += OnItemCollectionChanged;
			dashboard.DataSourceDataChanged += OnDataDataSourceDataChanged;
			dashboard.DashboardItemComponentNameChanged += OnDashboardItemComponentNameChanged;
		}
		public void Dispose() {
			if (dashboard != null) {
				dashboard.DashboardItemChanged -= OnDashboardItemChanged;
				dashboard.ItemCollectionChanged -= OnItemCollectionChanged;
				dashboard.DataSourceDataChanged -= OnDataDataSourceDataChanged;
				dashboard.DashboardItemComponentNameChanged -= OnDashboardItemComponentNameChanged;
				dashboard = null;
			}
		}
		public IInteractivitySession GetSession(string itemName) {
			IInteractivitySession session = null;
			if (!sessions.TryGetValue(itemName, out session)) {
				session = CreateSession(dashboard.Items[itemName]);
				if (session != null) {
					sessions[itemName] = session;
					ISingleFilterDefaultValueInquirer singleValueInquirer = session.SingleFilterDefaultValueInquirer;
					if (singleValueInquirer != null)
						singleValueInquirer.SingleFilterDefaultValue += OnSingleFilterDefaultValue;
					IFilterElementDefaultValuesInquirer filterValuesInquirer = session.FilterElementDefaultValuesInquirer;
					if (filterValuesInquirer != null)
						filterValuesInquirer.FilterElementDefaultValues += OnFilterElementDefaultValues;
					IRangeFilterDefaultValueInquirer rangeValueInquirer = session.RangeFilterDefaultValueInquirer;
					if (rangeValueInquirer != null)
						rangeValueInquirer.RangeFilterDefaultValue += OnRangeFilterDafaultValue;
				}
			}
			return session;
		}
		public void BeginServiceOperation() {
			serviceOperationLocker.Lock();
		}
		public void EndServiceOperation() {
			serviceOperationLocker.Unlock();
		}
		void OnDashboardItemChanged(object sender, DashboardItemChangedEventArgs e) {
			if (!serviceOperationLocker.IsLocked) {
				if (e.InnerArgs.Reason == ChangeReason.RawData || e.InnerArgs.Reason == ChangeReason.ClientData || e.InnerArgs.Reason == ChangeReason.Interactivity)
					RemoveSession(e.DashboardItem.ComponentName);
			}
		}
		void OnItemCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<DashboardItem> e) {
			foreach (DashboardItem removedItem in e.RemovedItems)
				RemoveSession(removedItem.ComponentName);
		}
		void OnDataDataSourceDataChanged(object sender, DataSourceChangedEventArgs e) {
			if (!serviceOperationLocker.IsLocked) {
				IEnumerable<DashboardItem> items = dashboard.Items.OfType<DataDashboardItem>().Where(item => item.DataSource == e.DataSource);
				foreach (DashboardItem item in items)
					RemoveSession(item.ComponentName);
			}
		}
		void OnDashboardItemComponentNameChanged(object sender, ComponentNameChangedEventArgs e) {
			IInteractivitySession session = null;
			if (sessions.TryGetValue(e.OldComponentName, out session)) {
				sessions.Remove(e.OldComponentName);
				sessions.Add(e.NewComponentName, session);
			}
		}
		void RemoveSession(string itemName) {
			IInteractivitySession session;
			if (sessions.TryGetValue(itemName, out session)) {
				ISingleFilterDefaultValueInquirer defaultValueInquirer = session.SingleFilterDefaultValueInquirer;
				if (defaultValueInquirer != null)
					defaultValueInquirer.SingleFilterDefaultValue -= OnSingleFilterDefaultValue;
				IFilterElementDefaultValuesInquirer filterValuesInquirer = session.FilterElementDefaultValuesInquirer;
				if (filterValuesInquirer != null)
					filterValuesInquirer.FilterElementDefaultValues -= OnFilterElementDefaultValues;
				IRangeFilterDefaultValueInquirer rangeValueInquirer = session.RangeFilterDefaultValueInquirer;
				if (rangeValueInquirer != null)
					rangeValueInquirer.RangeFilterDefaultValue -= OnRangeFilterDafaultValue;
				sessions.Remove(itemName);
			}			
		}
		void OnSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e) {
			if (SingleFilterDefaultValue != null) {
				e.SetDashboard(dashboard);
				SingleFilterDefaultValue(this, e);
			}
		}
		void OnFilterElementDefaultValues(object sender, FilterElementDefaultValuesEventArgs e) {
			if (FilterElementDefaultValues != null)
				FilterElementDefaultValues(this, e);
		}
		void OnRangeFilterDafaultValue(object sender, RangeFilterDefaultValueEventArgs e) {
			if (RangeFilterDefaultValue != null)
				RangeFilterDefaultValue(this, e);
		}
	}
}
