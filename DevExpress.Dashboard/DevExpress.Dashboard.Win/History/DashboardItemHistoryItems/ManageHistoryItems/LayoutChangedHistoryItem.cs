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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.Layout;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public class LayoutChangedHistoryItem : IHistoryItem {
		readonly Dashboard dashboard;
		readonly DashboardLayoutGroup previousLayout;
		readonly DashboardLayoutGroup nextLayout;
		readonly Dictionary<DashboardItem, DashboardItemGroup> previousDashboardItemsGroups = new Dictionary<DashboardItem, DashboardItemGroup>();
		readonly Dictionary<DashboardItem, DashboardItemGroup> nextDashboardItemsGroups = new Dictionary<DashboardItem, DashboardItemGroup>();
		public string Caption { get { return  DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemLayoutChanged); } }
		public LayoutChangedHistoryItem(Dashboard dashboard, DashboardLayoutGroup layout) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			Guard.ArgumentNotNull(layout, "layout");
			this.dashboard = dashboard;
			this.previousLayout = dashboard.LayoutRoot;
			this.nextLayout = layout;
			foreach(DashboardItem dashboardItem in dashboard.Items)
				previousDashboardItemsGroups.Add(dashboardItem, dashboardItem.Group);
			foreach(DashboardLayoutItem layoutItem in nextLayout.GetItemsRecursive())
				nextDashboardItemsGroups.Add(layoutItem.DashboardItem, (DashboardItemGroup)FindFirstVisibleGroup(layoutItem).DashboardItem);
		}
		public void Undo(DashboardDesigner designer) {
			SetDashboardLayout(previousLayout, nextLayout, previousDashboardItemsGroups, nextDashboardItemsGroups); 
		}
		public void Redo(DashboardDesigner designer) {
			SetDashboardLayout(nextLayout, previousLayout, nextDashboardItemsGroups, previousDashboardItemsGroups);
		}
		void SetProperty(object component, string propertyName, object newValue, object oldValue) {
			PropertyDescriptor property = Helper.GetProperty(component, propertyName);
			IComponentChangeService changeService = dashboard.GetService<IComponentChangeService>();
			if(changeService != null)
				changeService.OnComponentChanging(component, property);
			property.SetValue(component, newValue);
			if(changeService != null)
				changeService.OnComponentChanged(component, property, oldValue, newValue);
		}
		void SetDashboardLayout(DashboardLayoutGroup newLayout, DashboardLayoutGroup oldLayout, 
			Dictionary<DashboardItem,  DashboardItemGroup> newGroups, Dictionary<DashboardItem, DashboardItemGroup> oldGroups) {			
			DesignerTransaction transaction = null;
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			if(designerHost != null && !designerHost.InTransaction)
				transaction = designerHost.CreateTransaction(Caption);
			try {
				dashboard.BeginUpdate();
				try {
					SetProperty(dashboard, "LayoutRoot", newLayout, oldLayout);
					foreach(DashboardItem dashboardItem in dashboard.Items) {
						DashboardItemGroup newGroup = newGroups[dashboardItem];
						DashboardItemGroup oldGroup = oldGroups[dashboardItem];
						if(newGroup != oldGroup) {
							SetProperty(dashboardItem, "Group", newGroup, oldGroup);
						}
					}
				} finally {
					dashboard.EndUpdate();
				}
			} finally {
				if(transaction != null)
					transaction.Commit();
			}
		}
		DashboardLayoutGroup FindFirstVisibleGroup(DashboardLayoutItem layoutItem) { 
			DashboardLayoutGroup parent = layoutItem.Parent;
			while(parent != parent.Root && parent.DashboardItem == null)
				parent = parent.Parent;
			return parent;
		}
	}
}
