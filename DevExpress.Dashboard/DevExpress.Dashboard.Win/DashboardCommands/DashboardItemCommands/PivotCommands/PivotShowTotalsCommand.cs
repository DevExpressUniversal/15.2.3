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

using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public class PivotShowTotalsCommand : DashboardItemCommand<PivotDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowTotalsDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowTotals; } }
		public override string ImageName { get { return "PivotShowTotals"; } }
		public PivotShowTotalsCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public class PivotShowColumnTotalsCommand : DashboardItemInteractionCommand<PivotDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowColumnTotals; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowColumnTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowColumnTotalsDescription; } }
		public PivotShowColumnTotalsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(PivotDashboardItem item) {
			return item.ShowColumnTotals;
		}
		protected override IHistoryItem CreateHistoryItem(PivotDashboardItem dashboardItem, bool enabled) {
			return new PivotShowColumnTotalsHistoryItem(dashboardItem);
		}
	}
	public class PivotShowRowTotalsCommand : DashboardItemInteractionCommand<PivotDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowRowTotals; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowRowTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowRowTotalsDescription; } }
		public PivotShowRowTotalsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(PivotDashboardItem item) {
			return item.ShowRowTotals;
		}
		protected override IHistoryItem CreateHistoryItem(PivotDashboardItem dashboardItem, bool enabled) {
			return new PivotShowRowTotalsHistoryItem(dashboardItem);
		}
	}
}
