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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
#if !DXPORTABLE
using DevExpress.DashboardCommon.Native.PerfomanceMonitor;
using System.Threading;
#endif
namespace DevExpress.DashboardCommon.Service {
	public abstract class DashboardServiceOperation<T> where T : DashboardServiceOperationArgs {
		readonly IDashboardServer server;
		readonly IDashboardServiceAdminHandlers admin;
		readonly T args;
		protected T Args { get { return args; } }
		protected abstract string Context { get; }
		protected IDashboardServer Server { get { return server; } }
		protected DashboardServiceOperation(IDashboardServer server, IDashboardServiceAdminHandlers admin, T args) {
			this.server = server;
			this.admin = admin;
			this.args = args;
		}
		protected abstract DashboardServerResult RequestSession();
		protected virtual DashboardSessionState GetSessionState() {
			return BinarySerializer.Deserialize<DashboardSessionState>(Context);
		}
		protected virtual void CheckAllowability(DashboardSession session) {
			session.CheckAllowability();
		}
		protected abstract IList<AffectedItemInfo> ExecuteInternal(bool dataBindingApplied, DashboardServiceResult result, DashboardServerResult serverResult, DashboardSessionState sessionState);
		public void Execute(DashboardServiceResult result) {
#if !DXPORTABLE
			using(var ev = PfMonitor.Current.Event(this.GetType().Name)) {
#endif
				result.RequestMarker = Args.RequestMarker;
				try {
					DashboardSessionState sessionState = GetSessionState();
					DashboardServerResult serverResult = RequestSession();
					DashboardSession session = serverResult.Session;
					lock (session.SyncObject) {
						SubscribeAdminEvents(session);
						session.BeginServiceOperation();
						try {
							bool dataBindingApplied = session.Initialize(sessionState, Args.IsDesignMode);
							session.ClientState = Args.ClientState;
							CheckAllowability(session);
							DashboardClientData clientData = null;
							IList<AffectedItemInfo> affectedItems = ExecuteInternal(dataBindingApplied, result, serverResult, sessionState);
							if (affectedItems != null) {
								IEnumerable<string> itemsToIgnore = null;
								if(!(args.ForceRefreshAllItemData || session.EnableAutomaticUpdates) && Args.IsDesignMode)
									itemsToIgnore = affectedItems.Select(item => item.ItemName);
								else
									session.EnsureDashboardColoringCache();
								bool canceled;
								clientData = CalculateClientData(session, affectedItems, itemsToIgnore, out canceled);
								List<DashboardPaneContent> paneContent = new List<DashboardPaneContent>();
								foreach (AffectedItemInfo affectedItem in affectedItems)
									paneContent.Add(DashboardServiceOperationHelper.CreatePaneContent(session, affectedItem, clientData,
										(itemsToIgnore!=null && itemsToIgnore.Contains(affectedItem.ItemName)) ||
										canceled
										));
								result.PaneContent = paneContent;
							}
							result.Context = BinarySerializer.Serialize(session.GetState());
							IList<DimensionFilterValues> masterFilterValues = session.GetMasterFilterValues(true).ToList();
							if (masterFilterValues.Count > 0)
								result.MasterFilterValues = masterFilterValues;
							result.ClientState = session.ClientState;
						} finally {
							session.EndServiceOperation();
							UnsubscribeAdminEvents(session);
						}
					}
					result.ResultCode = DashboardServiceResultCode.Success;
				} catch (DashboardNotFoundException) {
					result.ResultCode = DashboardServiceResultCode.DashboardNotFound;
					result.Context = null;
					result.Error = new Exception(DashboardLocalizer.GetString(DashboardStringId.MessageDashboardNotFoundError));
				} catch (DashboardNotRelevantException) {
					result.ResultCode = DashboardServiceResultCode.DashboardNotRelevant;
					result.Context = null;
					result.Error = new Exception(DashboardLocalizer.GetString(DashboardStringId.MessageDashboardNotRelevantError));
				} catch (Exception e) {
					result.ResultCode = DashboardServiceResultCode.InternalError;
					result.InternalErrorType = e.GetType().Name;
					result.Context = null;
					string exceptionString = e.ToString();
					result.Error = new Exception(!string.IsNullOrEmpty(exceptionString) ? exceptionString : DashboardLocalizer.GetString(DashboardStringId.MessageDashboardInternalError));
				}
#if !DXPORTABLE
			};
#endif
		}
		void SubscribeAdminEvents(DashboardSession session) {
			session.SingleFilterDefaultValue += admin.OnSingleFilterDefaultValue;
			session.FilterElementDefaultValues += admin.OnFilterElementDefaultValues;
			session.RangeFilterDefaultValue += admin.OnRangeFilterDefaultValue;
			session.ConfigureDataConnection += admin.OnConfigureDataConnection;
			session.CustomFilterExpression += admin.OnCustomFilterExpression;
			session.CustomParameters += admin.OnCustomParameters;
			session.DataLoading += admin.OnDataLoading;
			session.DashboardLoading += admin.OnDashboardLoading;
			session.DashboardLoaded += admin.OnDashboardLoaded;
			session.ConnectionError += admin.OnConnectionError;
			session.AllowLoadUnusedDataSources += admin.OnAllowLoadUnusedDataSources;
			session.DashboardUnloading += admin.OnDashboardUnloading;
			session.RequestCustomizationServices += admin.OnRequestCustomizationServices;
			session.RequestAppConfigPatcherService += admin.OnRequestAppConfigPatcherService;
			session.RequestWaitFormActivator += admin.OnRequestWaitFormActivator;
			session.RequestErrorHandler += admin.OnRequestErrorHandler;
			session.RequestUnderlyingDataFormat += admin.OnRequestUnderlyingDataFormat;
			session.RequestDataLoader += admin.OnRequestDataLoader;
			session.ValidateCustomSqlQuery += admin.OnValidateCustomSqlQuery;
		}
		void UnsubscribeAdminEvents(DashboardSession session) {
			session.SingleFilterDefaultValue -= admin.OnSingleFilterDefaultValue;
			session.FilterElementDefaultValues -= admin.OnFilterElementDefaultValues;
			session.RangeFilterDefaultValue -= admin.OnRangeFilterDefaultValue;
			session.ConfigureDataConnection -= admin.OnConfigureDataConnection;
			session.CustomFilterExpression -= admin.OnCustomFilterExpression;
			session.CustomParameters -= admin.OnCustomParameters;
			session.DataLoading -= admin.OnDataLoading;
			session.DashboardLoading -= admin.OnDashboardLoading;
			session.DashboardLoaded -= admin.OnDashboardLoaded;
			session.ConnectionError -= admin.OnConnectionError;
			session.AllowLoadUnusedDataSources -= admin.OnAllowLoadUnusedDataSources;
			session.DashboardUnloading -= admin.OnDashboardUnloading;
			session.RequestCustomizationServices -= admin.OnRequestCustomizationServices;
			session.RequestAppConfigPatcherService -= admin.OnRequestAppConfigPatcherService;
			session.RequestWaitFormActivator -= admin.OnRequestWaitFormActivator;
			session.RequestErrorHandler -= admin.OnRequestErrorHandler;
			session.RequestUnderlyingDataFormat -= admin.OnRequestUnderlyingDataFormat;
			session.RequestDataLoader -= admin.OnRequestDataLoader;
			session.ValidateCustomSqlQuery -= admin.OnValidateCustomSqlQuery;
		}
		DashboardClientData CalculateClientData(DashboardSession session, IList<AffectedItemInfo> affectedItems, IEnumerable<string> itemsToIgnore, out bool canceled) {
			IEnumerable<string> itemsToCalculate = affectedItems.Where(item => item.ContentType.HasFlag(ContentType.CompleteDataSource)).Select(item => item.ItemName);
			return session.CalculateClientData(itemsToCalculate, itemsToIgnore, out canceled);
		}
	}
	public class AffectedItemInfo {
		public string ItemName { get; private set; }
		public ContentType ContentType { get; set; }
		public MultidimensionalDataDTO PartialDataSource { get; set; }
		public object[] Parameters { get; set; }
		public AffectedItemInfo(string itemName, ContentType contentType) {
			ItemName = itemName;
			ContentType = contentType;
		}
	}
	public class DashboardNotRelevantException : Exception {
	}
}
