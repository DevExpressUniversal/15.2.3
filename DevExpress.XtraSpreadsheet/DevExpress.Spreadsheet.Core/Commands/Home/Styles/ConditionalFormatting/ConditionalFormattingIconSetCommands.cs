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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ConditionalFormattingIconSetsCommandGroup
	public class ConditionalFormattingIconSetsCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingIconSetsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetsCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsCommandGroupDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrows5"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingIconSetsDirectionalCommandGroup
	public class ConditionalFormattingIconSetsDirectionalCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingIconSetsDirectionalCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetsDirectionalCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsDirectionalCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingIconSetsShapesCommandGroup
	public class ConditionalFormattingIconSetsShapesCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingIconSetsShapesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetsShapesCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsShapesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsShapesCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingIconSetsIndicatorsCommandGroup
	public class ConditionalFormattingIconSetsIndicatorsCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingIconSetsIndicatorsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetsIndicatorsCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsIndicatorsCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingIconSetsRatingsCommandGroup
	public class ConditionalFormattingIconSetsRatingsCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingIconSetsRatingsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetsRatingsCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetsRatingsCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingIconSetArrows3ColoredCommand
	public class ConditionalFormattingIconSetArrows3ColoredCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows3ColoredCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Colored; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3Colored; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3ColoredDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrows3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Arrows3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetArrows3GrayedCommand
	public class ConditionalFormattingIconSetArrows3GrayedCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows3GrayedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Grayed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3Grayed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows3GrayedDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrowsGray3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.ArrowsGray3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetArrows4ColoredCommand
	public class ConditionalFormattingIconSetArrows4ColoredCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows4ColoredCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Colored; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4Colored; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4ColoredDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrows4"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Arrows4; } }
		protected override int IconsCount { get { return 4; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetArrows4GrayedCommand
	public class ConditionalFormattingIconSetArrows4GrayedCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows4GrayedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Grayed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4Grayed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows4GrayedDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrowsGray4"; } }
		protected override IconSetType IconSetType { get { return IconSetType.ArrowsGray4; } }
		protected override int IconsCount { get { return 4; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetArrows5ColoredCommand
	public class ConditionalFormattingIconSetArrows5ColoredCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows5ColoredCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Colored; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5Colored; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5ColoredDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrows5"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Arrows5; } }
		protected override int IconsCount { get { return 5; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetArrows5GrayedCommand
	public class ConditionalFormattingIconSetArrows5GrayedCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetArrows5GrayedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Grayed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5Grayed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetArrows5GrayedDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetArrowsGray5"; } }
		protected override IconSetType IconSetType { get { return IconSetType.ArrowsGray5; } }
		protected override int IconsCount { get { return 5; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetTriangles3Command
	public class ConditionalFormattingIconSetTriangles3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetTriangles3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetTriangles3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTriangles3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTriangles3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetTriangles3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Triangles3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetTrafficLights3Command
	public class ConditionalFormattingIconSetTrafficLights3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetTrafficLights3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetTrafficLights3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.TrafficLights13; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetTrafficLights3RimmedCommand
	public class ConditionalFormattingIconSetTrafficLights3RimmedCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetTrafficLights3RimmedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3Rimmed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3Rimmed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights3RimmedDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetTrafficLightsRimmed3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.TrafficLights23; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetTrafficLights4Command
	public class ConditionalFormattingIconSetTrafficLights4Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetTrafficLights4Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights4; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights4; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetTrafficLights4Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetTrafficLights4"; } }
		protected override IconSetType IconSetType { get { return IconSetType.TrafficLights4; } }
		protected override int IconsCount { get { return 4; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetSigns3Command
	public class ConditionalFormattingIconSetSigns3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetSigns3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetSigns3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSigns3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSigns3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetSigns3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Signs3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetRedToBlackCommand
	public class ConditionalFormattingIconSetRedToBlackCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetRedToBlackCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetRedToBlack; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRedToBlack; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRedToBlackDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetRedToBlack4"; } }
		protected override IconSetType IconSetType { get { return IconSetType.RedToBlack4; } }
		protected override int IconsCount { get { return 4; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetSymbols3CircledCommand
	public class ConditionalFormattingIconSetSymbols3CircledCommand : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetSymbols3CircledCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3Circled; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3Circled; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3CircledDescription; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetSymbolsCircled3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Symbols3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetSymbols3Command
	public class ConditionalFormattingIconSetSymbols3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetSymbols3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetSymbols3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetSymbols3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Symbols23; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetFlags3Command
	public class ConditionalFormattingIconSetFlags3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetFlags3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetFlags3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetFlags3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetFlags3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetFlags3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Flags3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetStars3Command
	public class ConditionalFormattingIconSetStars3Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetStars3Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetStars3; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetStars3; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetStars3Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetStars3"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Stars3; } }
		protected override int IconsCount { get { return 3; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetRatings4Command
	public class ConditionalFormattingIconSetRatings4Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetRatings4Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetRatings4; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings4; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings4Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetRating4"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Rating4; } }
		protected override int IconsCount { get { return 4; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetRatings5Command
	public class ConditionalFormattingIconSetRatings5Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetRatings5Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetRatings5; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings5; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetRatings5Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetRating5"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Rating5; } }
		protected override int IconsCount { get { return 5; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetQuarters5Command
	public class ConditionalFormattingIconSetQuarters5Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetQuarters5Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetQuarters5; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetQuarters5; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetQuarters5Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetQuarters5"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Quarters5; } }
		protected override int IconsCount { get { return 5; } }
		#endregion
	}
	#endregion
	#region ConditionalFormattingIconSetBoxes5Command
	public class ConditionalFormattingIconSetBoxes5Command : ApplyIconSetConditionalFormattingCommand {
		public ConditionalFormattingIconSetBoxes5Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingIconSetBoxes5; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetBoxes5; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingIconSetBoxes5Description; } }
		public override string ImageName { get { return "ConditionalFormattinsIconSetBoxes5"; } }
		protected override IconSetType IconSetType { get { return IconSetType.Boxes5; } }
		protected override int IconsCount { get { return 5; } }
		#endregion
	}
	#endregion
}
