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
	public class PivotShowGrandTotalsCommand : DashboardItemCommand<PivotDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowGrandTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowGrandTotalsDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowGrandTotals; } }
		public override string ImageName { get { return "PivotShowGrandTotals"; } }
		public PivotShowGrandTotalsCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public class PivotShowColumnGrandTotalsCommand : DashboardItemInteractionCommand<PivotDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowColumnGrandTotals; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowColumnGrandTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowColumnGrandTotalsDescription; } }
		public PivotShowColumnGrandTotalsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(PivotDashboardItem item) {
			return item.ShowColumnGrandTotals;
		}
		protected override IHistoryItem CreateHistoryItem(PivotDashboardItem dashboardItem, bool enabled) {
			return new PivotShowColumnGrandTotalsHistoryItem(dashboardItem);
		}
	}
	public class PivotShowRowGrandTotalsCommand : DashboardItemInteractionCommand<PivotDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.PivotShowRowGrandTotals; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPivotShowRowGrandTotalsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPivotShowRowGrandTotalsDescription; } }
		public PivotShowRowGrandTotalsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(PivotDashboardItem item) {
			return item.ShowRowGrandTotals;
		}
		protected override IHistoryItem CreateHistoryItem(PivotDashboardItem dashboardItem, bool enabled) {
			return new PivotShowRowGrandTotalsHistoryItem(dashboardItem);
		}
	}
}
