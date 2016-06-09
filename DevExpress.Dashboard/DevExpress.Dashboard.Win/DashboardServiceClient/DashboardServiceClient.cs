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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Service;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using System.IO;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public class DashboardServiceClient {
		readonly IDashboardService dashboardService;
		readonly IServiceProvider serviceProvider;
		readonly Locker serviceOperationLocker = new Locker();
		readonly Locker deferredOperationsLocker = new Locker();
		readonly Queue<IClientOperation> deferredOperations = new Queue<IClientOperation>();
		string sessionId;
		string sessionContext;
		public bool InServiceOperation { get { return serviceOperationLocker.IsLocked; } }
		public bool IsDesignMode { get; set; }
		public event EventHandler<ServiceOperationCompletedEventArgs> OperationSucceed;
		public event EventHandler<ServiceOperationFailedEventArgs> OperationFailed;
		public DashboardServiceClient(IDashboardService dashboardService, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(dashboardService, "dashboardService");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.dashboardService = dashboardService;
			this.serviceProvider = serviceProvider;
		}
		public InitializeResult Initialize(Hashtable clientState, bool calculateHiddenTotals) {
			InitializeResult result = null;
			BeginServiceOperation();
			try {
				InitializeSessionArgs args = new InitializeSessionArgs() {
					IsDesignMode = IsDesignMode,
					ClientState = clientState,
					Settings = new SessionSettings {
						CalculateHiddenTotals = calculateHiddenTotals
					}
				};
				result = dashboardService.Initialize(args);
			} finally {
				EndServiceOperation();
			}
			sessionId = result.SessionId;
			sessionContext = result.Context;
			if (CheckServiceResult(result))
				return result;
			else
				return null;
		}
		public RefreshResult Refresh(Hashtable clientState, IEnumerable<IParameter> parameters, bool forceRefresh) {
			RefreshClientOperation operation = new RefreshClientOperation(clientState, parameters, forceRefresh);
			ExecuteOperation(operation, IsDesignMode);
			RefreshResult result = operation.Result;
			if (CheckServiceResult(result))
				return result;
			else
				return null;
		}
		public void Export(ExportInfo info, Stream stream, Hashtable clientState) {
			ExportClientOperation operation = new ExportClientOperation(clientState, info, stream);
			ExecuteOperation(operation, IsDesignMode);
			CheckServiceResult(operation.Result);
		}
		public IReportHolder GetExportReport(ExportInfo info, Hashtable clientState) {
			GetExportReportOperation operation = new GetExportReportOperation(clientState, info);
			ExecuteOperation(operation, IsDesignMode);
			GetExportReportResult result = operation.Result;
			if (CheckServiceResult(result))
				return result.ReportHolder;
			else
				return null;
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string name, IList columnValues, IList rowValues, IList<string> columnNames) {
			GetUnderlyingDataClientAction action = new GetUnderlyingDataClientAction(columnValues, rowValues, columnNames);
			ClientActionsOperation operation = new ClientActionsOperation(name, action, null);
			ExecuteOperation(operation, IsDesignMode);
			IList<UnderlyingData> underlyingData = operation.Result.UnderlyingData;
			return underlyingData != null && underlyingData.Count > 0 ? (DashboardUnderlyingDataSet)underlyingData[0].Data : null;
		}
		public void ReloadData(bool suppressWaitForm, IEnumerable<IParameter> parameters) {
			EnqueueDeferredOperation(new ReloadDataClientOperation(suppressWaitForm, parameters));
		}
		public void SetMasterFilter(string dashboardItemName, IEnumerable<IEnumerable<object>> rows) {
			SetMasterFilterClientAction action = new SetMasterFilterClientAction(rows);
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, action, null);
			EnqueueDeferredOperation(operation);
		}
		public void SetRange(string dashboardItemName, object minimum, object maximum) {
			SetRangeClientAction action = new SetRangeClientAction(minimum, maximum);
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, action, null);
			EnqueueDeferredOperation(operation);
		}
		public void ClearMasterFilter(string dashboardItemName, bool isRangeFilter) {
			ClearMasterFilterClientAction action = new ClearMasterFilterClientAction(isRangeFilter);
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, action, null);
			EnqueueDeferredOperation(operation);
		}
		public void DrillDown(string dashboardItemName, IEnumerable<object> row, bool setMasterFilter) {
			List<ClientActionBase> actions = new List<ClientActionBase>();
			if(setMasterFilter)
				actions.Add(new SetMasterFilterClientAction(new[] { row }));
			actions.Add(new DrillDownClientAction(row));
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, actions, null);
			EnqueueDeferredOperation(operation);
		}
		public void DrillUp(string dashboardItemName, bool clearMasterFilter) {
			List<ClientActionBase> actions = new List<ClientActionBase>();
			if(clearMasterFilter)
				actions.Add(new ClearMasterFilterClientAction(false));
			actions.Add(new DrillUpClientAction());
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, actions, null);
			EnqueueDeferredOperation(operation);
		}
		public void SetSelectedElementIndex(string dashboardItemName, int elementIndex) {
			SetSelectedElementIndexClientAction action = new SetSelectedElementIndexClientAction(elementIndex);
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, action, null);
			EnqueueDeferredOperation(operation);
		}
		public void RequestData(string dashboardItemName, Hashtable clientState) {
			RequestDataClientAction action = new RequestDataClientAction();
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, action, clientState);
			EnqueueDeferredOperation(operation);
		}
		public void ExpandValue(string dashboardItemName, params object[] parameters) {
			List<object> pars = parameters.ToList();
			pars.Add(true);
			ExpandValueClientAction action = new ExpandValueClientAction(pars.ToArray());
			ClientActionsOperation operation = new ClientActionsOperation(dashboardItemName, new[] { action }, null);
			EnqueueDeferredOperation(operation);
		}
		public void BeginDeferredOperation() {
			deferredOperationsLocker.Lock();
		}
		public void EndDeferredOperation() {
			deferredOperationsLocker.Unlock();
			ExecuteDeferredOperations();
		}
		void EnqueueDeferredOperation(IClientOperation operation) {
			if (!deferredOperationsLocker.IsLocked) 
				deferredOperations.Clear();
			deferredOperations.Enqueue(operation);
			ExecuteDeferredOperations();
		}
		void ExecuteDeferredOperations() {
			if(!deferredOperationsLocker.IsLocked) {
				Dictionary<string, DashboardPaneContent> paneContent = new Dictionary<string, DashboardPaneContent>();
				List<ClientActionNotification> notifications = new List<ClientActionNotification>();
				List<DimensionFilterValues> masterFilterValues = new List<DimensionFilterValues>();
				ServiceOperationFailedEventArgs operationFailedArgs = null;
				while(deferredOperations.Count > 0) {
					IClientOperation operation = deferredOperations.Dequeue();
					ExecuteOperation(operation, IsDesignMode);
					if(operation.Result.ResultCode == DashboardServiceResultCode.Success) {
						if(operation.Result.PaneContent != null) {
							foreach(DashboardPaneContent content in operation.Result.PaneContent)
								paneContent[content.Name] = content;
						}
						IList<ClientActionNotification> operationNotifications = operation.Notifications;
						if(operationNotifications != null)
							notifications.AddRange(operationNotifications);
						if(operation.Result.MasterFilterValues != null) {
							masterFilterValues.AddRange(operation.Result.MasterFilterValues);
						}
					} else {
						operationFailedArgs = CreateOperationFailedEventArgs(operation.Result);
						break;
					}
				}
				if(operationFailedArgs == null) {
					if(OperationSucceed != null)
						OperationSucceed(this, new ServiceOperationCompletedEventArgs(paneContent, notifications, masterFilterValues));
				} else
					RaiseOperationFailed(operationFailedArgs);
			}
		}
		void ExecuteOperation(IClientOperation operation, bool isDesignMode) {
			BeginServiceOperation();
			try {
				operation.Execute(dashboardService, sessionId, sessionContext, isDesignMode);
			} finally {
				EndServiceOperation();
			}
			sessionContext = operation.Result.Context;
		}
		void BeginServiceOperation() {
			var uiLocker = serviceProvider.RequestService<UILocker>();
			if (uiLocker != null)
				uiLocker.Lock();
			serviceOperationLocker.Lock();
		}
		void EndServiceOperation() {
			serviceOperationLocker.Unlock();
			var uiLocker = serviceProvider.RequestService<UILocker>();
			if (uiLocker != null)
				uiLocker.Unlock();
		}
		bool CheckServiceResult(DashboardServiceResult result) {
			if (result.ResultCode == DashboardServiceResultCode.Success)
				return true;
			else {
				ServiceOperationFailedEventArgs args = CreateOperationFailedEventArgs(result);
				if (args != null)
					RaiseOperationFailed(args);
				return false;
			}
		}
		ServiceOperationFailedEventArgs CreateOperationFailedEventArgs(DashboardServiceResult result) {
			switch (result.ResultCode) {
				case DashboardServiceResultCode.InternalError:
				case DashboardServiceResultCode.DashboardNotFound:
				case DashboardServiceResultCode.DashboardNotRelevant:
					return new ServiceOperationFailedEventArgs((ServiceOperationErrorType)result.ResultCode, result.Error.Message);
				default:
					return null;
			}
		}
		void RaiseOperationFailed(ServiceOperationFailedEventArgs args) {
			if (OperationFailed != null)
				OperationFailed(this, args);
		}
	}
}
