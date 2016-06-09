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
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardDesignerSelectionService {
		DashboardItem SelectedDashboardItem { get; set; }
		event EventHandler<DashboardItemSelectedEventArgs> DashboardItemSelected;
		void ForceDashboardItemSelection(DashboardItem dashboardItem);
	}
	public class DashboardDesignerSelectionService : IDashboardDesignerSelectionService, IDisposable {
		readonly IServiceProvider serviceProvider;
		public DashboardItem SelectedDashboardItem {
			get {
				IDashboardLayoutSelectionService layoutSelectionService = serviceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
				return FindDashboardItem(layoutSelectionService.SelectedItem);
			}
			set {
				if(value != SelectedDashboardItem)
					ForceDashboardItemSelection(value);
			}
		}
		public event EventHandler<DashboardItemSelectedEventArgs> DashboardItemSelected;
		public DashboardDesignerSelectionService(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.serviceProvider = serviceProvider;
			SubscribeServiceEvents();
		}
		public void Dispose() {
			UnsubscribeServiceEvents();
		}
		public void ForceDashboardItemSelection(DashboardItem dashboardItem) {
			IDashboardLayoutSelectionService layoutSelectionService = serviceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
			IDashboardLayoutControlItem layoutItem;
			if (dashboardItem != null) {
				IDashboardLayoutAccessService layoutAccessService = serviceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>();
				layoutItem = layoutAccessService.FindLayoutItem(dashboardItem.ComponentName);
			}
			else
				layoutItem = null;
			layoutSelectionService.SelectedItem = layoutItem;
		}
		void SubscribeServiceEvents() {
			IDashboardLayoutSelectionService layoutSelectionService = serviceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
			layoutSelectionService.ItemSelected += OnLayoutItemSelected;
		}
		void UnsubscribeServiceEvents() {
			IDashboardLayoutSelectionService layoutSelectionService = serviceProvider.RequestService<IDashboardLayoutSelectionService>();
			if (layoutSelectionService != null)
				layoutSelectionService.ItemSelected -= OnLayoutItemSelected;
		}
		void OnLayoutItemSelected(object sender, DashboardLayoutItemSelectedEventArgs e) {
			if (DashboardItemSelected != null) {				
				DashboardItem selectedDashboardItem = FindDashboardItem(e.SelectedItem);
				DashboardItemSelected(this, new DashboardItemSelectedEventArgs(selectedDashboardItem));
			}
		}
		DashboardItem FindDashboardItem(IDashboardLayoutControlItem layoutItem) {
			if (layoutItem != null) {
				IDashboardOwnerService ownerService = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				return ownerService.FindDashboardItemOrGroup(layoutItem.Name);
			}
			return null;
		}
	}
	public class DashboardItemSelectedEventArgs : EventArgs {
		public DashboardItem SelectedDashboardItem { get; private set; }
		public DashboardItemSelectedEventArgs(DashboardItem selectedDashboardItem) {
			SelectedDashboardItem = selectedDashboardItem;
		}
	}
}
