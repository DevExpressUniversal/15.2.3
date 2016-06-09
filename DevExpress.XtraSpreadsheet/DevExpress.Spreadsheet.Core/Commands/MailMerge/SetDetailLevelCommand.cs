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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SetDetailLevelCommand
	public class SetDetailLevelCommand :MailMergeCommand {
		#region fields
		const int DetailRange = -1;
		const int noRangeSelected = -2;
		MailMergeOptions options;
		CellRange selectedRange;
		int selectedIndex;
		#endregion
		public SetDetailLevelCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeSetDetailLevel; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailLevelCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeSetDetailLevelCommand; } }
		public override string ImageName { get { return "SetDetailLevel"; } }
		public virtual bool AllowFullSelectRange { get { return false; } }
		protected MailMergeOptions MailMergeOptions { get { return options; } }
		#endregion
		protected internal override void ExecuteCore() {
			ActiveSheet.Workbook.BeginUpdateFromUI();
			try {
				if (selectedIndex == noRangeSelected)
					return;
				string stringIndex = options.DetailLevels.Count.ToString();
				SetSheetDefinedNameValue(MailMergeDefinedNames.DetailLevel + stringIndex, GetAbsoluteSelectedRange());
				SetSheetDefinedNameValue(MailMergeDefinedNames.DetailDataMember + stringIndex, string.Empty);
				Control.InnerControl.RaiseUpdateUI();
				Control.InnerControl.Owner.Redraw();
			}
			finally {
				ActiveSheet.Workbook.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			selectedIndex = noRangeSelected;
			options = new MailMergeOptions(DocumentModel);
			SheetViewSelection selection = ActiveSheet.Selection;
			state.Enabled = !selection.IsAllSelected && (selection.SelectedRanges.Count == 1) && DetailedLevelSelect();
		}
		bool DetailedLevelSelect() {
			selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (options.DetailRange != null && ValidSelectionByRange(options.DetailRange, selectedRange)) {
				selectedIndex = DetailRange;
				foreach (CellRangeBase detailLevel in options.DetailLevels)
					if (ValidSelectionByRange(detailLevel, selectedRange))
						selectedIndex = options.DetailLevels.IndexOf(detailLevel);
					else
						if (detailLevel.Intersects(selectedRange))
							return false;
			}
			return selectedIndex != noRangeSelected;
		}
		bool ValidSelectionByRange(CellRangeBase range, CellRangeBase selectedRange) {
			if (range.Includes(selectedRange))
				return AllowFullSelectRange || !range.EqualsPosition(selectedRange);
			return false;
		}
		ParsedExpression GetAbsoluteSelectedRange() {
			CellPosition topLeft = new CellPosition(selectedRange.LeftColumnIndex, selectedRange.TopRowIndex, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(selectedRange.RightColumnIndex, selectedRange.BottomRowIndex, PositionType.Absolute, PositionType.Absolute);
			CellRangeBase copyOfRange = new CellRange(ActiveSheet, topLeft, bottomRight);
			ParsedExpression result = new ParsedExpression();
			BasicExpressionCreator.CreateCellRangeExpression(result, copyOfRange, BasicExpressionCreatorParameter.ShouldCreate3d, OperandDataType.Reference, copyOfRange.Worksheet.Workbook.DataContext);
			return result;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region EditingMailMergeMasterDetailCommandGroup
	public class EditingMailMergeMasterDetailCommandGroup :SpreadsheetCommandGroup {
		public EditingMailMergeMasterDetailCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingMailMergeMasterDetailCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingMailMergeMasterDetailCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingMailMergeMasterDetailCommandGroup; } }
		public override string ImageName { get { return "SetDetailLevel"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			base.ForceExecute(state);
			Control.InnerControl.RaiseUpdateUI();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			state.Enabled = options.DataMembers.Count > 0;
			if (!state.Enabled) {
				SheetViewSelection selection = ActiveSheet.Selection;
				state.Enabled = !selection.IsAllSelected && (selection.SelectedRanges.Count == 1) && ValidDetailLevelSelect(options);
			}
		}
		bool ValidDetailLevelSelect(MailMergeOptions options) {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (options.DetailRange != null && options.DetailRange.Includes(selectedRange)) {
				bool result = true;
				foreach (CellRangeBase detailLevel in options.DetailLevels)
					if (detailLevel.Includes(selectedRange))
						result = detailLevel != selectedRange;
					else if (detailLevel.Intersects(selectedRange))
						return false;
				return result;
			}
			return false;
		}
	}
	#endregion
}
