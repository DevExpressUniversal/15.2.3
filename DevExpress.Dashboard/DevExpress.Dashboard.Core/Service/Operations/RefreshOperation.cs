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
using DevExpress.DashboardCommon.Server;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Service {
	public class RefreshOperation : SessionOperation<RefreshArgs> {
		protected override string Context { get { return Args.Context; } }
		public RefreshOperation(IDashboardServer server, IDashboardServiceAdminHandlers admin, RefreshArgs args) 
			: base(server, admin, args) {
		}
		protected override DashboardSessionState GetSessionState() {
			DashboardSessionState sessionState = base.GetSessionState();
			sessionState.DashboardState = null;
			return sessionState;
		}
		protected override void CheckAllowability(DashboardSession session) {
		}
		protected override IList<AffectedItemInfo> ExecuteInternal(bool dataBindingApplied, DashboardServiceResult result, DashboardServerResult serverResult, DashboardSessionState sessionState) {
			List<AffectedItemInfo> affectedInfo = new List<AffectedItemInfo>();
			DashboardSession session = serverResult.Session;					
			RefreshManager refreshManager = session.RefreshManager;
			List<string> dataSourcesToFill = new List<string>();
			IList<string> dataSourceNames = session.GetDataSourceNames();
			foreach (string dataSourceName in dataSourceNames) {
				DataLoadingStrategy loadingStrategy = refreshManager.GetDataLoadingStrategy(dataSourceName);
				if (loadingStrategy != DataLoadingStrategy.None) {
					if (loadingStrategy == DataLoadingStrategy.RequireFill)
						dataSourcesToFill.Add(dataSourceName);
				}
			}
			if(dataSourcesToFill.Count > 0 && ShoulRefreshData(session))
				if(!session.ReloadData(dataSourcesToFill, Args)) {
					session.DisableAutomaticUpdates();
					Args.ForceRefreshAllItemData = false;
				}
			session.ApplyExternalDataSourceSchema(dataSourceNames.Where(dataSourceName => !dataSourcesToFill.Contains(dataSourceName)));
			if (refreshManager.GeneralRefreshType.HasFlag(GeneralRefreshType.Coloring))
				session.ResetDashboardColoringCache(null);
			RefreshResult refreshResult = (RefreshResult)result;
			if (refreshManager.GeneralRefreshType.HasFlag(GeneralRefreshType.Layout))
				refreshResult.RootPane = session.GetRootPane();
			if (refreshManager.GeneralRefreshType.HasFlag(GeneralRefreshType.Title))
				refreshResult.TitleViewModel = session.GetTitleViewModel();
			if(refreshManager.GeneralRefreshType.HasFlag(GeneralRefreshType.Parameters))
				refreshResult.DashboardParameters= session.GetParametersViewModel();
			if (ShoulRefreshData(session)) {
				affectedInfo.AddRange(session.GetItemNames().Select(itemName =>
					new AffectedItemInfo(itemName,
						Args.ForceRefreshAllItemData ? ContentType.CompleteDataSource | ContentType.ViewModel : refreshManager.GetItemContentType(itemName))));
			}
			else {
				affectedInfo.AddRange(session.GetItemNames().Select(itemName =>
					new AffectedItemInfo(itemName, refreshManager.DetailItems.Contains(itemName) ? ContentType.Empty : refreshManager.GetItemContentType(itemName))));
			}
			foreach (string groupName in session.GetGroupNames())
				affectedInfo.Add(new AffectedItemInfo(groupName, refreshManager.GetItemContentType(groupName)));			
			refreshManager.Clear(Args.ForceRefreshAllItemData);
			return affectedInfo;
		}
		bool ShoulRefreshData(DashboardSession session) {
			return session.EnableAutomaticUpdates || !Args.IsDesignMode || Args.ForceRefreshAllItemData;
		}
	}
}
