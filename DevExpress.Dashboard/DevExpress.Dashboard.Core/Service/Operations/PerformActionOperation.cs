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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Service {
	public class PerformActionOperation : SessionOperation<PerformActionArgs> {
		public PerformActionOperation(IDashboardServer server, IDashboardServiceAdminHandlers admin, PerformActionArgs args)
			: base(server, admin, args) {
		}
		List<AffectedItemInfo> ExecuteAction(bool dataBindingApplied, ActionInfo actionInfo, DashboardServerResult serverResult, DashboardSession session, DashboardServiceResult result) {
			string itemName = actionInfo.ItemName;
			ActionType actionType = actionInfo.ActionType;
			object[] parameters = actionInfo.Parameters;
			List<AffectedItemInfo> affectedItemsInfo = dataBindingApplied ?
				null :
				DashboardServiceOperationHelper.GetAffectedItemsInfo(session, itemName, actionType);
			if(dataBindingApplied)
				session.PrepareAction(itemName, actionType);
			switch(actionType) {
				case ActionType.SetMasterFilter:
					session.SetMasterFilter(itemName, ValuesSetHelper.FromFlatRowList(parameters));
					break;
				case ActionType.SetMultipleValuesMasterFilter:
					session.SetMultipleValuesMasterFilter(itemName, ValuesSetHelper.FromFlatRowList(parameters));
					break;
				case ActionType.ClearMasterFilter:
					session.ClearMasterFilter(itemName);
					break;
				case ActionType.DrillDown:
					ExecuteActionInternal(session, affectedItemsInfo, () => session.PerformDrillDown(itemName, parameters[0]));
					break;
				case ActionType.DrillUp:
					ExecuteActionInternal(session, affectedItemsInfo, () => session.PerformDrillUp(itemName));					
					break;
				case ActionType.SetSelectedElementIndex:
					session.SetSelectedElementIndex(itemName, (int)parameters[0]);
					break;
				case ActionType.ExpandValue:
					ExecuteActionInternal(session, affectedItemsInfo, () => {
						object[] values = ((IEnumerable)parameters[0]).Cast<object>().ToArray();
						bool isColumn = (bool)parameters[1];
						bool isExpand = (bool)parameters[2];
						bool isRequestData = (bool)parameters[3];
						bool forceFullData = parameters.Length > 4 ? (bool)parameters[4] : false;
						MultidimensionalDataDTO partialDataSource = session.ExpandValue(itemName, isColumn, values, isExpand, isRequestData, forceFullData);
						if(affectedItemsInfo != null) {
							AffectedItemInfo info = affectedItemsInfo.Find(info_ => info_.ItemName == itemName);
							info.PartialDataSource = partialDataSource;
							info.Parameters = new object[] { values, isColumn };
						}
					});
					break;
				case ActionType.DataRequest:
					session.ProcessDataRequest(itemName);
					break;
				case ActionType.GetDrillThroughData:
					ExecuteGetDrillThroughData(itemName, parameters, result, session);
					break;
			}
			return affectedItemsInfo ?? DashboardServiceOperationHelper.GetDataBoundAffectedItemsInfo(session);
		}
		protected override IList<AffectedItemInfo> ExecuteInternal(bool dataBindingApplied, DashboardServiceResult result, DashboardServerResult serverResult, DashboardSessionState sessionState) {
			DashboardSession session = serverResult.Session;
			Dictionary<string, AffectedItemInfo> resultDictionary = new Dictionary<string, AffectedItemInfo>();
			IEnumerable<ActionInfo> preparedActions = PrepareActions(Args.ActionInfo);
			foreach(ActionInfo actionInfo in preparedActions) {
				List<AffectedItemInfo> items = ExecuteAction(dataBindingApplied, actionInfo, serverResult, session, result);
				foreach(AffectedItemInfo item in items)
					if(item.ContentType != ContentType.Empty)
						resultDictionary[item.ItemName] = item;
			}
			return resultDictionary.Values.ToList<AffectedItemInfo>();
		}
		IEnumerable<ActionInfo> PrepareActions(IEnumerable<ActionInfo> actions) {
			IEnumerable<ActionInfo> preparedActions = new List<ActionInfo>(actions);
			ActionType[] masterFilterTypes = new[] { ActionType.SetMasterFilter, ActionType.SetMultipleValuesMasterFilter, ActionType.ClearMasterFilter };
			preparedActions = PrepareActions(preparedActions, masterFilterTypes);
			preparedActions = PrepareActions(preparedActions, new[] { ActionType.DataRequest });
			return preparedActions;
		}
		IEnumerable<ActionInfo> PrepareActions(IEnumerable<ActionInfo> actions, ActionType[] actionTypes) {
			Dictionary<string, ActionInfo> lastActions = new Dictionary<string, ActionInfo>();
			foreach(ActionInfo action in actions) {
				if(actionTypes.Contains(action.ActionType))
					lastActions[action.ItemName] = action;
			}
			List<ActionInfo> preparedActions = new List<ActionInfo>(actions);
			foreach(ActionInfo action in actions) {
				if(actionTypes.Contains(action.ActionType)) {
					if(action != lastActions[action.ItemName])
						preparedActions.Remove(action);
				}
			}
			return preparedActions;
		}
		void ExecuteGetDrillThroughData(string itemName, object[] parameters, DashboardServiceResult result, DashboardSession session) {
			IList columnValues = (IList)parameters[0];
			IList rowValues = (IList)parameters[1];
			IList untypedColumnNames = (IList)parameters[2];
			UnderlyingData underlyingData;
			List<string> columnNames = null;
			try {
				if(untypedColumnNames != null) {
					columnNames = new List<string>();
					foreach(object name in untypedColumnNames)
						columnNames.Add(name.ToString());
				}
				underlyingData = session.GetUnderlyingData(itemName, columnValues, rowValues, columnNames);
				result.UnderlyingData.Add(underlyingData);
			}
			catch(Exception e) {
				string exceptionString = e.ToString();
				result.UnderlyingData.Add(new UnderlyingData {
					Data = null,
					DataMembers = null,
					ErrorMessage = string.IsNullOrEmpty(exceptionString)
						? DashboardLocalizer.GetString(DashboardStringId.MessageDashboardInternalError)
						: string.Format(DashboardLocalizer.GetString(DashboardStringId.MessageInternalError), exceptionString)
				});
			}
		}
		void ExecuteActionInternal(DashboardSession session, List<AffectedItemInfo> affectedItemsInfo, Action action) {
			if(session.EnableAutomaticUpdates || !Args.IsDesignMode)
				action();
			else {
				foreach(AffectedItemInfo info in affectedItemsInfo)
					info.ContentType = ContentType.ViewModel;
			}
		}	   
	}
}
