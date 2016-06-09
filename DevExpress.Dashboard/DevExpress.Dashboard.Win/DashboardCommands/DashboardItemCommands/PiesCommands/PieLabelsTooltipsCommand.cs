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
using DevExpress.Utils.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Commands {
	public class PieLabelsTooltipsCommand : DashboardItemCommand<PieDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltips; } }
		public override string ImageName { get { return "PieLabelsTooltips"; } }
		public PieLabelsTooltipsCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public abstract class PieLabelsTooltipsValueTypeCommand : DashboardItemInteractionCommand<PieDashboardItem> {
		protected abstract PieValueType ValueType { get; }
		protected PieLabelsTooltipsValueTypeCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override bool CheckDashboardItem(PieDashboardItem item) {
			return item.TooltipContentType == ValueType;
		}
		protected override IHistoryItem CreateHistoryItem(PieDashboardItem dashboardItem, bool enabled) {
			return new PieTooltipContentTypeHistoryItem(dashboardItem, ValueType);
		}
	}
	public class PieLabelsTooltipsNoneCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsNoneCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsNoneDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsNone; } }
		protected override PieValueType ValueType { get { return PieValueType.None; } }
		public PieLabelsTooltipsNoneCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsArgumentCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsArgument; } }
		protected override PieValueType ValueType { get { return PieValueType.Argument; } }
		public PieLabelsTooltipsArgumentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsValueCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsValueCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsValueDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsValue; } }
		protected override PieValueType ValueType { get { return PieValueType.Value; } }
		public PieLabelsTooltipsValueCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsPercentCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.Percent; } }
		public PieLabelsTooltipsPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsValueAndPercentCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsValueAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsValueAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsValueAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ValueAndPercent; } }
		public PieLabelsTooltipsValueAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsArgumentAndValueCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndValueCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndValueDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsArgumentAndValue; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentAndValue; } }
		public PieLabelsTooltipsArgumentAndValueCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsArgumentAndPercentCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsArgumentAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentAndPercent; } }
		public PieLabelsTooltipsArgumentAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsTooltipsArgumentValueAndPercentCommand : PieLabelsTooltipsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentValueAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsTooltipsArgumentValueAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsTooltipsArgumentValueAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentValueAndPercent; } }
		public PieLabelsTooltipsArgumentValueAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
