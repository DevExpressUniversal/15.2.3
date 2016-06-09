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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SetRangeCommand
	public abstract class SetRangeCommand :MailMergeCommand {
		protected SetRangeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected abstract string RangeName { get; }
		protected abstract bool NeedDetailRange { get; }
		protected virtual bool AllowIntersectDetailRange { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			ActiveSheet.Workbook.BeginUpdateFromUI();
			try {
				CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
				ParsedExpression expression = GenerateRangeExpression(selectedRange);
				SetFunctionalComment(SetSheetDefinedNameValue(RangeName, expression));
				Control.InnerControl.RaiseUpdateUI();
				Control.InnerControl.Owner.Redraw();
			}
			finally {
				ActiveSheet.Workbook.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			state.Enabled = true;
			if (NeedDetailRange)
				state.Enabled = options.DetailRange != null;
			if (state.Enabled) {
				SheetViewSelection selection = ActiveSheet.Selection;
				state.Enabled = !selection.IsAllSelected && (selection.SelectedRanges.Count == 1);
				if (!AllowIntersectDetailRange && state.Enabled)
					state.Enabled = options.DetailRange == null || !options.DetailRange.Intersects(selection.SelectedRanges[0]);
			}
		}
		protected virtual void SetFunctionalComment(DefinedName definedName) {
		}
		ParsedExpression GenerateRangeExpression(CellRangeBase range) {
			CellPosition topLeftCell = new CellPosition(range.TopLeft.Column, range.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRightCell = new CellPosition(range.BottomRight.Column, range.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
			CellRangeBase copyOfRange = new CellRange(range.Worksheet, topLeftCell, bottomRightCell);
			ParsedExpression result = new ParsedExpression();
			BasicExpressionCreator.CreateCellRangeExpression(result, copyOfRange, BasicExpressionCreatorParameter.ShouldCreate3d, OperandDataType.Reference, range.Worksheet.Workbook.DataContext);
			return result;
		}
	}
	#endregion
}
