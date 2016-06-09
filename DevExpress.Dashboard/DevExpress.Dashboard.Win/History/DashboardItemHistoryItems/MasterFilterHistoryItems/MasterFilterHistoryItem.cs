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
	public abstract class MasterFilterHistoryItem : ToggleStateHistoryItem<DataDashboardItem> {
		readonly DashboardItemMasterFilterMode previousMasterFilterMode;
		bool crossDataSourceFiltering;
		DashboardItemMasterFilterMode NextMasterFilterMode { get { return NewState ? MasterFilterMode : DashboardItemMasterFilterMode.None; } }
		protected abstract DashboardItemMasterFilterMode MasterFilterMode { get; }
		protected MasterFilterHistoryItem(DataDashboardItem dashboardItem, bool state)
			: base(dashboardItem, state) {
			crossDataSourceFiltering = !state && dashboardItem.IsMasterFilterCrossDataSource;
			previousMasterFilterMode = dashboardItem.MasterFilterMode;
		}
		protected override void PerformRedo() {
			DashboardItem.MasterFilterMode = NextMasterFilterMode;
			if(crossDataSourceFiltering)
				DashboardItem.IsMasterFilterCrossDataSource = false;
		}
		protected override void PerformUndo() {
			DashboardItem.MasterFilterMode = previousMasterFilterMode;
			if(crossDataSourceFiltering)
				DashboardItem.IsMasterFilterCrossDataSource = true;
		}
	}
	public class SingleMasterFilterHistoryItem : MasterFilterHistoryItem {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemSingleMasterFilter; } }
		protected override DashboardItemMasterFilterMode MasterFilterMode { get { return DashboardItemMasterFilterMode.Single; } }
		public SingleMasterFilterHistoryItem(DataDashboardItem dashboardItem, bool enabled)
			: base(dashboardItem, enabled) {
		}
	}
	public class MultipleMasterFilterHistoryItem : MasterFilterHistoryItem {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemMultipleMasterFilter; } }
		protected override DashboardItemMasterFilterMode MasterFilterMode { get { return DashboardItemMasterFilterMode.Multiple; } }
		public MultipleMasterFilterHistoryItem(DataDashboardItem dashboardItem, bool enabled)
			: base(dashboardItem, enabled) {
		}
	}
	public class DrillDownHistoryItem : ToggleStateHistoryItem<DataDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemDrillDown; } }
		public DrillDownHistoryItem(DataDashboardItem dashboardItem, bool enabled)
			: base(dashboardItem, enabled) {
		}
		protected override void PerformRedo() {
			DashboardItem.IsDrillDownEnabled = NewState;
		}
		protected override void PerformUndo() {
			DashboardItem.IsDrillDownEnabled = !NewState;
		}
	}
}
