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
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;
namespace DevExpress.DashboardWin.ServiceModel.Design {
	public class DashboardDesignerBehaviorDesignTime : DashboardDesignerSelectionBehavior, IDashboardSelectionService, IDisposable {
		readonly List<string> itemNames = new List<string>();
		readonly IDesignerHost designerHost;
		public bool IsDashboardSelected { get { return DesignSelectionService.GetSelectedComponents().OfType<Dashboard>().Any(); } }		
		bool HasDesignSelection { get { return DesignSelectionService.GetSelectedComponents().Count > 0; } }
		Dashboard Dashboard { get { return OwnerService.Dashboard; } }
		ISelectionService DesignSelectionService { get { return (ISelectionService)designerHost.GetService(typeof(ISelectionService)); } }
		IDashboardViewerInvalidateService InvalidateService { get { return ServiceProvider.RequestServiceStrictly<IDashboardViewerInvalidateService>(); } }
		IDashboardLoadingService LoadingService { get { return ServiceProvider.RequestServiceStrictly<IDashboardLoadingService>(); } }
		public DashboardDesignerBehaviorDesignTime(IServiceProvider serviceProvider, IDesignerHost designerHost)
			: base(serviceProvider) {
			Guard.ArgumentNotNull(designerHost, "designerHost");
			this.designerHost = designerHost;
			designerHost.TransactionOpened += OnTransactionOpened;
			designerHost.TransactionClosed += OnTransactionClosed;
			SubscribeDesignSelectionChangedEvent();
			IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			selectionService.DashboardItemSelected += OnDashboardItemSelected;
			IDashboardLoadingService loadingService = ServiceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardBeginInitialize += OnDashboardBeginInitialize;
		}
		public void Dispose() {
			designerHost.TransactionOpened -= OnTransactionOpened;
			designerHost.TransactionClosed -= OnTransactionClosed;
			UnsubscribeDesignSelectionChangedEvent();
			IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestService<IDashboardDesignerSelectionService>();
			if (selectionService != null)
				selectionService.DashboardItemSelected -= OnDashboardItemSelected;
			IDashboardLoadingService loadingService = ServiceProvider.RequestService<IDashboardLoadingService>();
			if (loadingService != null)
				loadingService.DashboardBeginInitialize -= OnDashboardBeginInitialize;
		}
		public override void SetDefaultSelection() {
			base.SetDefaultSelection();
			if (!HasDesignSelection)
				SelectDashboard();
		}
		public void SelectDashboard() {
			SetDesignSelection(Dashboard);
		}
		void SubscribeDesignSelectionChangedEvent() {
			DesignSelectionService.SelectionChanged += OnDesignSelectionChanged;
		}
		void UnsubscribeDesignSelectionChangedEvent() {
			ISelectionService designSelectionService = DesignSelectionService;
			if (designSelectionService != null)
				designSelectionService.SelectionChanged -= OnDesignSelectionChanged;
		}
		void OnTransactionOpened(object sender, EventArgs e) {
			if (!LoadingService.IsDashboardInitializing) {
				Dashboard dashboard = Dashboard;
				if (dashboard != null) {
					dashboard.BeginUpdate(); 
					itemNames.AddRange(Dashboard.ItemsAndGroups.Select(item => item.ComponentName));
				}
			}
		}
		[SecuritySafeCritical]
		void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if (!LoadingService.IsDashboardInitializing) {
				Dashboard dashboard = Dashboard;
				if (dashboard != null) {
					bool isDestroying = dashboard.DashboardComponents.Any(component => component.Site == null);
					if (isDestroying)
						dashboard.CancelUpdate(); 
					else
						dashboard.EndUpdate(); 
					if (e.TransactionCommitted && e.LastTransaction) {
						List<string> currentItemNames = new List<string>(dashboard.ItemsAndGroups.Select(item => item.ComponentName));
						List<string> addedItemNames = currentItemNames.FindAll(item => !itemNames.Contains(item));
						SelectDashboardItemOnUpdate(addedItemNames);
						itemNames.Clear();
					}
				}
			}
		}
		void OnDesignSelectionChanged(object sender, EventArgs args) {
			if (!HasDesignSelection)
				SelectDashboard();
			else if (IsDashboardSelected)
				SelectionService.SelectedDashboardItem = null;
			InvalidateService.InvalidateViewer();
		}
		void OnDashboardItemSelected(object sender, DashboardItemSelectedEventArgs e) {
			if(e.SelectedDashboardItem != null)
				SetDesignSelection(e.SelectedDashboardItem);
		}
		void OnDashboardBeginInitialize(object sender, EventArgs e) {
			UnsubscribeDesignSelectionChangedEvent();
			if (HasDesignSelection)
				SetDesignSelection(null);
			SubscribeDesignSelectionChangedEvent();
		}
		void SetDesignSelection(object obj) {
			DesignSelectionService.SetSelectedComponents(obj != null ? new object[] { obj } : null, SelectionTypes.Replace);
		}
	}
}
