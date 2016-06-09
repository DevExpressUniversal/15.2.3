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
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ComboBoxTypeHistoryItem : DashboardItemHistoryItem<ComboBoxDashboardItem> {
		readonly ComboBoxDashboardItemType newType;
		readonly ComboBoxDashboardItemType prevType;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemComboBoxChangeType; } } 
		public ComboBoxTypeHistoryItem(ComboBoxDashboardItem dashboardItem, ComboBoxDashboardItemType newType)
			: base(dashboardItem) {
			this.newType = newType;
			this.prevType = dashboardItem.ComboBoxType;
		}
		protected override void PerformRedo() {
			DashboardItem.ComboBoxType = newType;
		}
		protected override void PerformUndo() {
			DashboardItem.ComboBoxType = prevType;
		}
	}
	public class ListBoxTypeHistoryItem : DashboardItemHistoryItem<ListBoxDashboardItem> {
		readonly ListBoxDashboardItemType newType;
		readonly ListBoxDashboardItemType prevType;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemListBoxChangeType; } }
		public ListBoxTypeHistoryItem(ListBoxDashboardItem dashboardItem, ListBoxDashboardItemType newType)
			: base(dashboardItem) {
			this.newType = newType;
			this.prevType = dashboardItem.ListBoxType;
		}
		protected override void PerformRedo() {
			DashboardItem.ListBoxType = newType;
		}
		protected override void PerformUndo() {
			DashboardItem.ListBoxType = prevType;
		}
	}
	internal class TreeViewTypeHistoryItem : DashboardItemHistoryItem<TreeViewDashboardItem> {
		readonly TreeViewDashboardItemType newType;
		readonly TreeViewDashboardItemType prevType;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemComboBoxChangeType; } }
		internal TreeViewTypeHistoryItem(TreeViewDashboardItem dashboardItem, TreeViewDashboardItemType newType)
			: base(dashboardItem) {
			this.newType = newType;
			this.prevType = dashboardItem.TreeViewType;
		}
		protected override void PerformRedo() {
			DashboardItem.TreeViewType = newType;
		}
		protected override void PerformUndo() {
			DashboardItem.TreeViewType = prevType;
		}
	}
	public class TreeViewAutoExpandHistoryItem : ToggleStateHistoryItem<TreeViewDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemTreeViewAutoExpandChanged; } }
		public TreeViewAutoExpandHistoryItem(TreeViewDashboardItem dashboardItem, bool expanded)
			: base(dashboardItem, expanded) {
		}
		protected override void PerformUndo() {
			DashboardItem.AutoExpandNodes = !NewState;
		}
		protected override void PerformRedo() {
			DashboardItem.AutoExpandNodes = NewState;
		}
	}
	public class FilterElementShowAllValueHistoryItem : ToggleStateHistoryItem<FilterElementDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemTreeViewAutoExpandChanged; } }
		public FilterElementShowAllValueHistoryItem(FilterElementDashboardItem dashboardItem, bool enabled)
			: base(dashboardItem, enabled) {
		}
		protected override void PerformRedo() {
			DashboardItem.ShowAllValue = NewState;
		}
		protected override void PerformUndo() {
			DashboardItem.ShowAllValue = !NewState;
		}
	}
}
