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
	public class PieLabelsDataLabelsCommand : DashboardItemCommand<PieDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabels; } }
		public override string ImageName { get { return "PieLabelsDataLabels"; } }
		public PieLabelsDataLabelsCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public abstract class PieLabelsDataLabelsValueTypeCommand : DashboardItemInteractionCommand<PieDashboardItem> {
		protected abstract PieValueType ValueType { get; }
		protected PieLabelsDataLabelsValueTypeCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override bool CheckDashboardItem(PieDashboardItem item) {
			return item.LabelContentType == ValueType;
		}
		protected override IHistoryItem CreateHistoryItem(PieDashboardItem dashboardItem, bool enabled) {
			return new PieLabelContentTypeHistoryItem(dashboardItem, ValueType);
		}
	}
	public class PieLabelsDataLabelsNoneCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsNoneCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsNoneDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsNone; } }
		protected override PieValueType ValueType { get { return PieValueType.None; } }
		public PieLabelsDataLabelsNoneCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsArgumentCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsArgument; } }
		protected override PieValueType ValueType { get { return PieValueType.Argument; } }
		public PieLabelsDataLabelsArgumentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsValueCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsValueCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsValueDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsValue; } }
		protected override PieValueType ValueType { get { return PieValueType.Value; } }
		public PieLabelsDataLabelsValueCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsPercentCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.Percent; } }
		public PieLabelsDataLabelsPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsValueAndPercentCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsValueAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsValueAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsValueAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ValueAndPercent; } }
		public PieLabelsDataLabelsValueAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsArgumentAndValueCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndValueCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndValueDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsArgumentAndValue; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentAndValue; } }
		public PieLabelsDataLabelsArgumentAndValueCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsArgumentAndPercentCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsArgumentAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentAndPercent; } }
		public PieLabelsDataLabelsArgumentAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieLabelsDataLabelsArgumentValueAndPercentCommand : PieLabelsDataLabelsValueTypeCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentValueAndPercentCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandPieLabelsDataLabelsArgumentValueAndPercentDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.PieLabelsDataLabelsArgumentValueAndPercent; } }
		protected override PieValueType ValueType { get { return PieValueType.ArgumentValueAndPercent; } }
		public PieLabelsDataLabelsArgumentValueAndPercentCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
