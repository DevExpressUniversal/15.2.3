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
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public abstract class PieStyleCommand : DashboardItemInteractionCommand<PieDashboardItem> {
		protected abstract PieType Type { get; }
		protected PieStyleCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(PieDashboardItem item) {
			return item.PieType == Type;
		}
		protected override IHistoryItem CreateHistoryItem(PieDashboardItem dashboardItem, bool enabled) {
			return new PieStyleHistoryItem(dashboardItem, Type);
		}
	}
	public class PieStylePieCommand : PieStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieStylePieCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieStylePieDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieStylePie; } }
		public override string ImageName { get { return "PieStylePie"; } }
		protected override PieType Type { get { return PieType.Pie; } }
		public PieStylePieCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieStyleDonutCommand : PieStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieStyleDonutCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieStyleDonutDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieStyleDonut; } }
		public override string ImageName { get { return "PieStyleDonut"; } }
		protected override PieType Type { get { return PieType.Donut; } }
		public PieStyleDonutCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
