#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ConditionalFormattingColorScalesCommandGroup
	public class ConditionalFormattingColorScalesCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingColorScalesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScalesCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScalesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScalesCommandGroupDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreenYellowRed"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingColorScaleGreenYellowRedCommand
	public class ConditionalFormattingColorScaleGreenYellowRedCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleGreenYellowRedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellowRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreenYellowRed"; } }
		protected override Color ColorLow { get { return Green; } }
		protected override Color ColorMiddle { get { return Yellow; } }
		protected override Color ColorHigh { get { return Red; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleRedYellowGreenCommand
	public class ConditionalFormattingColorScaleRedYellowGreenCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleRedYellowGreenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleRedYellowGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedYellowGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedYellowGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingRedYellowGreen"; } }
		protected override Color ColorLow { get { return Red; } }
		protected override Color ColorMiddle { get { return Yellow; } }
		protected override Color ColorHigh { get { return Green; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleGreenWhiteRedCommand
	public class ConditionalFormattingColorScaleGreenWhiteRedCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleGreenWhiteRedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhiteRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreenWhiteRed"; } }
		protected override Color ColorLow { get { return Green; } }
		protected override Color ColorMiddle { get { return White; } }
		protected override Color ColorHigh { get { return Red; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleRedWhiteGreenCommand
	public class ConditionalFormattingColorScaleRedWhiteGreenCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleRedWhiteGreenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingRedWhiteGreen"; } }
		protected override Color ColorLow { get { return Red; } }
		protected override Color ColorMiddle { get { return White; } }
		protected override Color ColorHigh { get { return Green; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleBlueWhiteRedCommand
	public class ConditionalFormattingColorScaleBlueWhiteRedCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleBlueWhiteRedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleBlueWhiteRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleBlueWhiteRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleBlueWhiteRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingBlueWhiteRed"; } }
		protected override Color ColorLow { get { return Blue; } }
		protected override Color ColorMiddle { get { return White; } }
		protected override Color ColorHigh { get { return Red; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleRedWhiteBlueCommand
	public class ConditionalFormattingColorScaleRedWhiteBlueCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleRedWhiteBlueCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteBlue; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteBlue; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteBlueDescription; } }
		public override string ImageName { get { return "ConditionalFormattingRedWhiteBlue"; } }
		protected override Color ColorLow { get { return Red; } }
		protected override Color ColorMiddle { get { return White; } }
		protected override Color ColorHigh { get { return Blue; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleWhiteRedCommand
	public class ConditionalFormattingColorScaleWhiteRedCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleWhiteRedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingWhiteRed"; } }
		protected override Color ColorLow { get { return White; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return Red; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleRedWhiteCommand
	public class ConditionalFormattingColorScaleRedWhiteCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleRedWhiteCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhite; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhite; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleRedWhiteDescription; } }
		public override string ImageName { get { return "ConditionalFormattingRedWhite"; } }
		protected override Color ColorLow { get { return Red; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return White; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleGreenWhiteCommand
	public class ConditionalFormattingColorScaleGreenWhiteCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleGreenWhiteCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhite; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhite; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenWhiteDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreenWhite"; } }
		protected override Color ColorLow { get { return Green; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return White; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleWhiteGreenCommand
	public class ConditionalFormattingColorScaleWhiteGreenCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleWhiteGreenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleWhiteGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingWhiteGreen"; } }
		protected override Color ColorLow { get { return White; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return Green; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleGreenYellowCommand
	public class ConditionalFormattingColorScaleGreenYellowCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleGreenYellowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellow; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellow; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleGreenYellowDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGreenYellow"; } }
		protected override Color ColorLow { get { return Green; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return Yellow; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingColorScaleYellowGreenCommand
	public class ConditionalFormattingColorScaleYellowGreenCommand : Apply3PointColorScaleConditionalFormattingCommand {
		public ConditionalFormattingColorScaleYellowGreenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingColorScaleYellowGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleYellowGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingColorScaleYellowGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingYellowGreen"; } }
		protected override Color ColorLow { get { return Yellow; } }
		protected override Color ColorMiddle { get { return DXColor.Empty; } }
		protected override Color ColorHigh { get { return Green; } }
		#endregion
	}
	#endregion
}
