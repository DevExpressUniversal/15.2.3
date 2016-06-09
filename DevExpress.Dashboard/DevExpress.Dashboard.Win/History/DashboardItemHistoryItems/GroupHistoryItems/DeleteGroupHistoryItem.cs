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
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class DeleteGroupHistoryItem : DashboardItemLayoutHistoryItem {
		readonly List<DashboardItem> removedItems = new List<DashboardItem>();
		readonly DashboardItemGroup itemGroup;
		int groupIndex;
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemDeleteItem), itemGroup.Name); } }
		protected override DashboardItem RemovedDashboardItem { get { return itemGroup; } }
		protected override IEnumerable<string> PropertyNames { get { return new[] { "Groups", "Items" }; } }
		public DeleteGroupHistoryItem(DashboardItemGroup dashboardItem) {
			this.itemGroup = dashboardItem;
		}
		protected override void PerformUndoAction(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			dashboard.Groups.Insert(groupIndex, itemGroup);
			foreach(DashboardItem item in removedItems)
				dashboard.Items.Add(item);
			removedItems.Clear();
		}
		protected override void PerformRedoAction(DashboardDesigner designer) {
			Dashboard dashboard = designer.Dashboard;
			groupIndex = dashboard.Groups.IndexOf(itemGroup);
			foreach(DashboardItem item in itemGroup.Items)
				removedItems.Add(item);
			foreach(DashboardItem item in removedItems) {
				dashboard.Items.Remove(item);
				RemoveDashboardItemFromDesignerHost(dashboard, item);
			}
			dashboard.Groups.Remove(itemGroup);
		}
	}
}
