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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess;
using System;
using System.Collections.Generic;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		readonly DashboardServiceClient serviceClient;
		WinDashboardService service;
		internal DashboardServiceClient ServiceClient { get { return serviceClient; } }
		internal event EventHandler<DashboardLoadedServerEventArgs> DashboardServiceDashboardLoaded;
		void InitializeDashboardService() {
			service.CustomFilterExpressionEvent += OnDashboardServiceCustomFilterExpression;
			service.DataLoadingEvent += OnDashboardServiceDataLoading;
			service.ConfigureDataConnectionEvent += OnDashboardServiceConfigureDataConnection;
			service.ConnectionErrorEvent += OnDashboardServiceConnectionError;
			service.CustomParametersEvent += OnDashboardServiceCustomParameters;
			service.AllowLoadUnusedDataSourcesEvent += OnDashboardServiceAllowLoadUnusedDataSources;
			service.DashboardLoadedEvent += OnDashboardServiceDashboardLoaded;
			service.SingleFilterDefaultValue += OnDashboardSingleFilterDefaultValue;
			service.FilterElementDefaultValues += OnFilterElementDefaultValues;
			service.ValidateCustomSqlQuery += OnValidateCustomSqlQuery;
			service.RangeFilterDefaultValue += OnRangeFilterDefaultValue;
			serviceClient.OperationSucceed += OnServiceClientOperationSucceed;
			serviceClient.OperationFailed += OnServiceClientOperationFailed;
		}
		void OnDashboardServiceCustomFilterExpression(object sender, CustomFilterExpressionServerEventArgs e) {
			if (CustomFilterExpression != null) {
				CustomFilterExpressionEventArgs args = new CustomFilterExpressionEventArgs(e.DataSourceComponentName, e.DataSourceName, e.TableName) {
					FilterExpression = e.FilterExpression
				};
				CustomFilterExpression(this, args);
				e.FilterExpression = args.FilterExpression;
			}
		}
		void OnDashboardServiceDataLoading(object sender, DataLoadingServerEventArgs e) {
			if (DataLoading != null) {
				DataLoadingEventArgs args = new DataLoadingEventArgs(e.DataSourceComponentName, e.DataSourceName) {
					Data = e.Data
				};
				DataLoading(this, args);
				e.Data = args.Data;
			}
		}
		void OnDashboardServiceConfigureDataConnection(object sender, ConfigureDataConnectionServerEventArgs e) {
			if (ConfigureDataConnection != null) {
				DashboardConfigureDataConnectionEventArgs args = new DashboardConfigureDataConnectionEventArgs(e.ConnectionName, e.DataSourceName,  e.ConnectionParameters);
				ConfigureDataConnection(this, args);
				e.ConnectionParameters = args.ConnectionParameters;
			}
		}
		void OnDashboardServiceConnectionError(object sender, ConnectionErrorServerEventArgs e) {
			if (!e.Handled && ConnectionError != null) {
				DashboardConnectionErrorEventArgs args = new DashboardConnectionErrorEventArgs(e.ConnectionName,  e.DataSourceName,  e.ConnectionParameters, e.Exception) {
					Handled = e.Handled,
					Cancel = e.Cancel
				};
				ConnectionError(this, args);
				e.Handled = args.Handled;
				e.Cancel = args.Cancel;
				e.ConnectionParameters = args.ConnectionParameters;
			}
		}
		void OnDashboardServiceCustomParameters(object sender, CustomParametersServerEventArgs e) {
			if (CustomParameters != null) {
				CustomParametersEventArgs args = new CustomParametersEventArgs(e.Parameters);
				CustomParameters(this, args);
				e.Parameters = args.Parameters;
			}
		}
		void OnDashboardServiceAllowLoadUnusedDataSources(object sender, AllowLoadUnusedDataSourcesServerEventArgs e) {
			e.Allow = populateUnusedDataSources || isDesignMode;
		}
		void OnDashboardServiceDashboardLoaded(object sender, DashboardLoadedServerEventArgs e) {
			if (DashboardServiceDashboardLoaded != null)
				DashboardServiceDashboardLoaded(this, e);
		}
		void OnDashboardSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e) {
			if (SingleFilterDefaultValue != null)
				SingleFilterDefaultValue(sender, e);
		}
		void OnFilterElementDefaultValues(object sender, FilterElementDefaultValuesEventArgs e) {
			if (FilterElementDefaultValues != null)
				FilterElementDefaultValues(this, e);
		}
		void OnRangeFilterDefaultValue(object sender, RangeFilterDefaultValueEventArgs e) {
			if (RangeFilterDefaultValue != null)
				RangeFilterDefaultValue(this, e);
		}
		void OnValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryEventArgs e) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
		void OnServiceClientOperationSucceed(object sender, ServiceOperationCompletedEventArgs e) {
			MasterFilterSetEventArgs[] precalculatedArgs = new MasterFilterSetEventArgs[e.Notifications.Count];
			if (MasterFilterSet != null) {
				for (int i = 0; i < e.Notifications.Count; i++) {
					ClientActionNotification notification = e.Notifications[i];
					if (notification.NotificationType == ClientActionNotificationType.MasterFilterSet) {
						DataDashboardItemViewer dataItemViewer = (DataDashboardItemViewer)FindDashboardItemViewer(notification.DashboardItemName);
						DashboardDataSet selection = dataItemViewer.GetCurrentSelection(notification.MasterFilterRows);
						precalculatedArgs[i] = new MasterFilterSetEventArgs(notification.DashboardItemName, selection, null);
					}
				}
			}
			foreach (KeyValuePair<string, DashboardPaneContent> pair in e.PaneContent)
				RefreshDashboardItemViewer(pair.Value);
			UpdateDashboardTitle(e.MasterFilterValues);
			for (int i = 0; i < e.Notifications.Count; i++) {
				ClientActionNotification notification = e.Notifications[i];
				switch (notification.NotificationType) {
					case ClientActionNotificationType.MasterFilterSet:
						if (MasterFilterSet != null)
							MasterFilterSet(this, precalculatedArgs[i]);
						break;
					case ClientActionNotificationType.RangeSet:
						if (MasterFilterSet != null) {
							RangeFilterDashboardItemViewer rangeViewer = (RangeFilterDashboardItemViewer)FindDashboardItemViewer(notification.DashboardItemName);
							RangeFilterSelection selection = rangeViewer.GetCurrentRange(notification.Minimum, notification.Maximum);
							MasterFilterSet(this, new MasterFilterSetEventArgs(notification.DashboardItemName, null, selection));
						}
						break;
					case ClientActionNotificationType.MasterFilterCleared:
						if (MasterFilterCleared != null)
							MasterFilterCleared(this, new MasterFilterClearedEventArgs(notification.DashboardItemName));
						break;
					case ClientActionNotificationType.DrillDownPerformed:
						if (DrillDownPerformed != null) {
							DataDashboardItemViewer dataItemViewer = (DataDashboardItemViewer)FindDashboardItemViewer(notification.DashboardItemName);
							int drillDownLevel = dataItemViewer.GetDrillDownLevel();
							DashboardDataSet drillDownValues = dataItemViewer.GetDrillDownDataSet();
							DrillDownPerformed(this, new DrillActionEventArgs(notification.DashboardItemName, drillDownLevel, drillDownValues));
						}
						break;
					case ClientActionNotificationType.DrillUpPerformed:
						if (DrillUpPerformed != null) {
							DataDashboardItemViewer dataItemViewer = (DataDashboardItemViewer)FindDashboardItemViewer(notification.DashboardItemName);
							int drillDownLevel = dataItemViewer.GetDrillDownLevel();
							DashboardDataSet drillDownValues = dataItemViewer.GetDrillDownDataSet();
							DrillUpPerformed(this, new DrillActionEventArgs(notification.DashboardItemName, drillDownLevel, drillDownValues));
						}
						break;
				};
			}
		}
		void OnServiceClientOperationFailed(object sender, ServiceOperationFailedEventArgs e) {
			if (e.ErrorType == ServiceOperationErrorType.DashboardNotRelevant)
				throw new DashboardLockedException();
			else
				throw new DashboardInternalException(e.ErrorMessage);
		}
	}
}
