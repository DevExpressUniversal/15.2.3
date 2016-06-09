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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShowColumnWidthFormCommand
	public class ShowColumnWidthFormCommand : ShowColumnWidthFormCommandBase {
		public ShowColumnWidthFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatColumnWidth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatColumnWidth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatColumnWidthDescription; } }
		#endregion
		protected internal override void ShowForm(IColumnWidthCalculationService service) {
			if (service == null)
				return;
			ColumnWidthViewModel viewModel = new ColumnWidthViewModel(Control);
			viewModel.Value = GetWidth(service);
			Control.ShowColumnWidthForm(viewModel);
		}
		protected internal float? GetWidth(IColumnWidthCalculationService service) {
			float defaultColumnWidth = service.CalculateDefaultColumnWidthInChars(ActiveSheet, DocumentModel.MaxDigitWidthInPixels);
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			float columnWidth = CalculateColumnWidth(selectedRanges[0].LeftColumnIndex, defaultColumnWidth);
			foreach (CellRange selectedRange in selectedRanges)
				for (int i = selectedRange.LeftColumnIndex; i <= selectedRange.RightColumnIndex; i++) {
					float currentColumnWidth = CalculateColumnWidth(i, defaultColumnWidth);
					if (currentColumnWidth != columnWidth)
						return null;
				}
			return columnWidth;
		}
		float CalculateColumnWidth(int columnIndex, float defaultColumnWidth) {
			Column column = ActiveSheet.Columns.TryGetColumn(columnIndex);
			if (column == null)
				return defaultColumnWidth;
			if (column.IsHidden)
				return 0;
			return column.IsCustomWidth || column.Width != 0 ? column.Width : defaultColumnWidth;
		}
		protected internal override void ApplyChangesCore(float widthInCharacters) {
			IColumnWidthCalculationService service = GetColumnWidthService();
			if (service == null)
				return;
			float defaultColumnWidth = service.CalculateDefaultColumnWidthInChars(ActiveSheet, DocumentModel.MaxDigitWidthInPixels);
			foreach (CellRange selectedRange in ActiveSheet.Selection.SelectedRanges)
				for (int i = selectedRange.LeftColumnIndex; i <= selectedRange.RightColumnIndex; i++) {
					ApplyColumnWidthCore(i, widthInCharacters, defaultColumnWidth);
				}
		}
		void ApplyColumnWidthCore(int columnIndex, float value, float defaultColumnWidth) {
			Column column = ActiveSheet.Columns.TryGetColumn(columnIndex);
			bool hasCustomWidth = column != null && column.IsCustomWidth;
			if (hasCustomWidth || value != defaultColumnWidth) {
				column = ActiveSheet.Columns.GetIsolatedColumn(columnIndex);
				column.SetCustomWidth(value);
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class ShowColumnWidthFormContextMenuCommand : ShowColumnWidthFormCommand {
		public ShowColumnWidthFormContextMenuCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatColumnWidthContextMenuItem; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = IsEntireColumnSelection();
			state.Visible = state.Enabled;
		}
		bool IsEntireColumnSelection() {
			int count = ActiveSheet.Selection.SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				if (!ActiveSheet.Selection.SelectedRanges[i].IsColumnRangeInterval())
					return false;
			return count > 0;
		}
	}
}
