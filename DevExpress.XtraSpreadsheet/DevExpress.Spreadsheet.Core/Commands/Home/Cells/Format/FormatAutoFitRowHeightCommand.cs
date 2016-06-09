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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatAutoFitRowHeightCommand
	public class FormatAutoFitRowHeightCommand : SpreadsheetSelectedRangesCommand {
		public FormatAutoFitRowHeightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAutoFitRowHeight; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAutoFitRowHeight; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAutoFitRowHeightDescription; } }
		#endregion
		protected internal override void Modify(CellRange range) {
			ActiveSheet.TryBestFitRow(range);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Row.Resize, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatRowsLocked);
			if (state.Enabled)
				ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Row.AutoFit);
		}
	}
	#endregion
	#region FormatAutoFitRowHeightUsingMouseCommand
	public class FormatAutoFitRowHeightUsingMouseCommand : FormatAutoFitWidthUsingMouseCommandBase {
		#region Fields
		int rowIndex = -1;
		#endregion
		public FormatAutoFitRowHeightUsingMouseCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAutoFitRowHeightUsingMouse; } }
		protected internal int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		#endregion
		protected internal override bool IsRangeInterval(CellRange range) {
			return range.IsRowRangeInterval();
		}
		protected internal override bool IsContainsItem(CellRange range) {
			return range.TopLeft.Row <= RowIndex && range.BottomRight.Row >= RowIndex;
		}
		protected internal override void AutoFitSelectedItem(CellRange range) {
			ActiveSheet.TryBestFitRow(range);
		}
		protected internal override void AutoFitOnlyGeneralItem() {
			ActiveSheet.TryBestFitRow(RowIndex);
		}
	}
	#endregion
}
