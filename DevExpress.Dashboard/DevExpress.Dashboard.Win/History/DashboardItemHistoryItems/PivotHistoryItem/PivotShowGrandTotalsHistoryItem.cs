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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class PivotShowColumnGrandTotalsHistoryItem : ToggleStateHistoryItem<PivotDashboardItem> {
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), DashboardItem.Name); } }
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemPivotShowColumnGrandTotalsEnable : DashboardWinStringId.HistoryItemPivotShowColumnGrandTotalsDisable; } }
		public PivotShowColumnGrandTotalsHistoryItem(PivotDashboardItem dashboardItem)
			: base(dashboardItem, !dashboardItem.ShowColumnGrandTotals) {
		}
		protected override void PerformUndo() {
			DashboardItem.ShowColumnGrandTotals = !NewState;
		}
		protected override void PerformRedo() {
			DashboardItem.ShowColumnGrandTotals = NewState;
		}
	}
	public class PivotShowRowGrandTotalsHistoryItem : ToggleStateHistoryItem<PivotDashboardItem> {
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), DashboardItem.Name); } }
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemPivotShowRowGrandTotalsEnable : DashboardWinStringId.HistoryItemPivotShowRowGrandTotalsDisable; } }
		public PivotShowRowGrandTotalsHistoryItem(PivotDashboardItem dashboardItem)
			: base(dashboardItem, !dashboardItem.ShowRowGrandTotals) {
		}
		protected override void PerformUndo() {
			DashboardItem.ShowRowGrandTotals = !NewState;
		}
		protected override void PerformRedo() {
			DashboardItem.ShowRowGrandTotals = NewState;
		}
	}
	public class PivotShowColumnTotalsHistoryItem : ToggleStateHistoryItem<PivotDashboardItem> {
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), DashboardItem.Name); } }
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemPivotShowColumnTotalsEnable : DashboardWinStringId.HistoryItemPivotShowColumnTotalsDisable;  } }
		public PivotShowColumnTotalsHistoryItem(PivotDashboardItem dashboardItem)
			: base(dashboardItem, !dashboardItem.ShowColumnTotals) {
		}
		protected override void PerformUndo() {
			DashboardItem.ShowColumnTotals = !NewState;
		}
		protected override void PerformRedo() {
			DashboardItem.ShowColumnTotals = NewState;
		}
	}
	public class PivotShowRowTotalsHistoryItem : ToggleStateHistoryItem<PivotDashboardItem> {
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), DashboardItem.Name); } }
		protected override DashboardWinStringId CaptionId { get { return NewState ? DashboardWinStringId.HistoryItemPivotShowRowTotalsEnable : DashboardWinStringId.HistoryItemPivotShowRowTotalsDisable; } }
		public PivotShowRowTotalsHistoryItem(PivotDashboardItem dashboardItem)
			: base(dashboardItem, !dashboardItem.ShowRowTotals) {
		}
		protected override void PerformUndo() {
			DashboardItem.ShowRowTotals = !NewState;
		}
		protected override void PerformRedo() {
			DashboardItem.ShowRowTotals = NewState;
		}
	}
}
