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
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ConditionalFormattingDataBarsCommandGroup
	public class ConditionalFormattingDataBarsCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingDataBarsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarsCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsCommandGroupDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidBlueDataBar"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingDataBarsGradientFillCommandGroup
	public class ConditionalFormattingDataBarsGradientFillCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingDataBarsGradientFillCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarsGradientFillCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsGradientFillCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ConditionalFormattingDataBarsSolidFillCommandGroup
	public class ConditionalFormattingDataBarsSolidFillCommandGroup : SpreadsheetCommandGroup {
		public ConditionalFormattingDataBarsSolidFillCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarsSolidFillCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarsSolidFillCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region ApplyDataBarConditionalFormatting (abstract class)
	public abstract class ApplyDataBarConditionalFormatting : ApplyConditionalFormattingCommand {
		protected ApplyDataBarConditionalFormatting(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected abstract Color Color { get; }
		protected abstract bool IsGradientFill { get; }
		protected virtual Color NegativeColor { get { return DXColor.Red; } }
		protected Color Blue { get { return DXColor.FromArgb(0x63, 0x8E, 0xC6); } }
		protected Color Green { get { return DXColor.FromArgb(0x63, 0xC3, 0x84); } }
		protected Color Red { get { return DXColor.FromArgb(0xFF, 0x55, 0x5A); } }
		protected Color Orange { get { return DXColor.FromArgb(0xFF, 0xB6, 0x28); } }
		protected Color LightBlue { get { return DXColor.FromArgb(0x00, 0x8A, 0xEF); } }
		protected Color Purple { get { return DXColor.FromArgb(0xD6, 0x00, 0x7B); } }
		#endregion
		protected override ConditionalFormatting CreateConditionalFormatting(Worksheet sheet, CellRangeBase range) {
			DataBarConditionalFormatting result = new DataBarConditionalFormatting(sheet);
			result.Color = Color;
			result.LowBound = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.AutoMin, 0);
			result.HighBound = new ConditionalFormattingValueObject(sheet, ConditionalFormattingValueObjectType.AutoMax, 0);
			result.NegativeValueColor = NegativeColor;
			result.AxisColor = DXColor.Black;
			result.MinLength = 0;
			result.MaxLength = 100;
			result.GradientFill = IsGradientFill;
			if (IsGradientFill) {
				result.BorderColor = Color;
				result.NegativeValueBorderColor = NegativeColor;
			}
			result.SetCellRange(range);
			return result;
		}
	}
	#endregion
	#region ConditionalFormattingDataBarGradientBlue
	public class ConditionalFormattingDataBarGradientBlue : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientBlue(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientBlue; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientBlue; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientBlueDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientBlueDataBar"; } }
		protected override Color Color { get { return Blue; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidBlue
	public class ConditionalFormattingDataBarSolidBlue : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidBlue(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidBlue; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidBlue; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidBlueDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidBlueDataBar"; } }
		protected override Color Color { get { return Blue; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
	#region ConditionalFormattingDataBarGradientGreen
	public class ConditionalFormattingDataBarGradientGreen : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientGreen(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientGreenDataBar"; } }
		protected override Color Color { get { return Green; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidGreen
	public class ConditionalFormattingDataBarSolidGreen : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidGreen(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidGreen; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidGreen; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidGreenDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidGreenDataBar"; } }
		protected override Color Color { get { return Green; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
	#region ConditionalFormattingDataBarGradientRed
	public class ConditionalFormattingDataBarGradientRed : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientRed(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientRedDataBar"; } }
		protected override Color Color { get { return Red; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidRed
	public class ConditionalFormattingDataBarSolidRed : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidRed(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidRed; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidRed; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidRedDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidRedDataBar"; } }
		protected override Color Color { get { return Red; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
	#region ConditionalFormattingDataBarGradientOrange
	public class ConditionalFormattingDataBarGradientOrange : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientOrange(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientOrange; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientOrange; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientOrangeDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientOrangeDataBar"; } }
		protected override Color Color { get { return Orange; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidOrange
	public class ConditionalFormattingDataBarSolidOrange : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidOrange(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidOrange; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidOrange; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidOrangeDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidOrangeDataBar"; } }
		protected override Color Color { get { return Orange; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
	#region ConditionalFormattingDataBarGradientLightBlue
	public class ConditionalFormattingDataBarGradientLightBlue : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientLightBlue(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientLightBlue; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientLightBlue; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientLightBlueDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientLightBlueDataBar"; } }
		protected override Color Color { get { return LightBlue; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidLightBlue
	public class ConditionalFormattingDataBarSolidLightBlue : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidLightBlue(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidLightBlue; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidLightBlue; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidLightBlueDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidLightBlueDataBar"; } }
		protected override Color Color { get { return LightBlue; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
	#region ConditionalFormattingDataBarGradientPurple
	public class ConditionalFormattingDataBarGradientPurple : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarGradientPurple(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarGradientPurple; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientPurple; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarGradientPurpleDescription; } }
		public override string ImageName { get { return "ConditionalFormattingGradientPurpleDataBar"; } }
		protected override Color Color { get { return Purple; } }
		protected override bool IsGradientFill { get { return true; } }
	}
	#endregion
	#region ConditionalFormattingDataBarSolidPurple
	public class ConditionalFormattingDataBarSolidPurple : ApplyDataBarConditionalFormatting {
		public ConditionalFormattingDataBarSolidPurple(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ConditionalFormattingDataBarSolidPurple; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidPurple; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ConditionalFormattingDataBarSolidPurpleDescription; } }
		public override string ImageName { get { return "ConditionalFormattingSolidPurpleDataBar"; } }
		protected override Color Color { get { return Purple; } }
		protected override bool IsGradientFill { get { return false; } }
	}
	#endregion
}
