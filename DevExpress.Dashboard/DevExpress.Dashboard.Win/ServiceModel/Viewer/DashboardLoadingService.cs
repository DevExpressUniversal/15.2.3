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
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardLoadingService {
		bool IsDashboardInitializing { get; }
		event EventHandler<DashboardLoadingEventArgs> DashboardLoad;
		event EventHandler<DashboardLoadingEventArgs> DashboardUnload;
		event EventHandler DashboardBeginInitialize;
		event EventHandler DashboardEndInitialize;
		void OnDashboardLoad(Dashboard dashboard);
		void OnDashboardUnload(Dashboard dashboard);
		void OnDashboardBeginInitialize();
		void OnDashboardEndInitialize();
	}
	public class DashboardLoadingService : IDashboardLoadingService {
		public bool IsDashboardInitializing { get; private set; }
		public event EventHandler<DashboardLoadingEventArgs> DashboardLoad;
		public event EventHandler<DashboardLoadingEventArgs> DashboardUnload;
		public event EventHandler DashboardBeginInitialize;
		public event EventHandler DashboardEndInitialize;
		public void OnDashboardLoad(Dashboard dashboard) {
			if (dashboard != null && DashboardLoad != null)
				DashboardLoad(this, new DashboardLoadingEventArgs(dashboard));
		}
		public void OnDashboardUnload(Dashboard dashboard) {
			if (dashboard != null && DashboardUnload != null)
				DashboardUnload(this, new DashboardLoadingEventArgs(dashboard));
		}
		public void OnDashboardBeginInitialize() {
			IsDashboardInitializing = true;
			if (DashboardBeginInitialize != null)
				DashboardBeginInitialize(this, EventArgs.Empty);
		}
		public void OnDashboardEndInitialize() {			
			if (DashboardEndInitialize != null)
				DashboardEndInitialize(this, EventArgs.Empty);
			IsDashboardInitializing = false;
		}
	}
	public class DashboardLoadingEventArgs : EventArgs {
		public Dashboard Dashboard { get; private set; }
		public DashboardLoadingEventArgs(Dashboard dashboard) {
			Dashboard = dashboard;
		}
	}
}
