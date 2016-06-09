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
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShowRowHeightFormCommand
	public class ShowRowHeightFormCommand : SpreadsheetCommand {
		public ShowRowHeightFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatRowHeight; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatRowHeight; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatRowHeightDescription; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms) {
					IColumnWidthCalculationService service = DocumentModel.GetService<IColumnWidthCalculationService>();
					if (service == null)
						return;
					RowHeightViewModel viewModel = new RowHeightViewModel(Control);
					viewModel.Value = GetHeight(service);
					Control.ShowRowHeightForm(viewModel);
				}
				else {
					IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
					if (valueBasedState != null)
						SetHeight(valueBasedState.Value);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal float? GetHeight(IColumnWidthCalculationService service) {
			float defaultRowHeight = service.CalculateDefaultRowHeightInPoints(ActiveSheet);
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			float rowHeight = CalculateRowHeight(service, selectedRanges[0].TopRowIndex, defaultRowHeight);
			foreach (CellRange selectedRange in selectedRanges)
				for (int i = selectedRange.TopRowIndex; i <= selectedRange.BottomRowIndex; i++) {
					float currentRowHeight = CalculateRowHeight(service, i, defaultRowHeight);
					if (currentRowHeight != rowHeight)
						return null;
				}
			return rowHeight;
		}
		float CalculateRowHeight(IColumnWidthCalculationService service, int rowIndex, float defaultRowHeight) {
			Row row = ActiveSheet.Rows.TryGetRow(rowIndex);
			if (row == null)
				return defaultRowHeight;
			if (row.IsHidden)
				return 0;
			if (row.IsCustomHeight)
				return DocumentModel.UnitConverter.ModelUnitsToPointsF(row.Height);
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToPointsF(service.CalculateRowHeight(ActiveSheet, rowIndex));
		}
		public bool Validate(float heightInPoints) {
			if (heightInPoints < 0 || heightInPoints > 409) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeight));
				return false;
			}
			return true;
		}
		void SetHeight(int heightInPixels) {
			float heightInModelUnits = DocumentModel.UnitConverter.PixelsToModelUnits(heightInPixels, DocumentModel.Dpi);
			ApplyChangesCore(heightInModelUnits);
		}
		public void ApplyChanges(float heightInPoints) {
			float heightInModelUnits = DocumentModel.UnitConverter.PointsToModelUnitsF(heightInPoints);
			ApplyChangesCore(heightInModelUnits);
		}
		public void ApplyChangesCore(float heightInModelUnits) {
			DocumentModel.BeginUpdateFromUI();
			try {
				int count = ActiveSheet.Selection.SelectedRanges.Count;
				for (int i = 0; i < count; i++) {
					ApplyRowHeightToRange(ActiveSheet.Selection.SelectedRanges[i], heightInModelUnits);
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void ApplyRowHeightToRange(CellRange range, float heightInModelUnits) {
			if (range.TopLeft.Row == 0 && range.BottomRight.Row == IndicesChecker.MaxRowCount - 1) {
				ApplyRowHeightToExistingRows(range, heightInModelUnits);
				SheetFormatProperties properties = ActiveSheet.Properties.FormatProperties;
				properties.DefaultRowHeight = heightInModelUnits;
				properties.IsCustomHeight = true;
			}
			else
				ApplyRowHeightToRangeSimple(range, heightInModelUnits);
		}
		void ApplyRowHeightToRangeSimple(CellRange range, float value) {
			int firstRowIndex = range.TopLeft.Row;
			int lastRowIndex = range.BottomRight.Row;
			ActiveSheet.UnhideRows(firstRowIndex, lastRowIndex);
			for (int rowIndex = firstRowIndex; rowIndex <= lastRowIndex; rowIndex++) {
				Row current = ActiveSheet.Rows[rowIndex];
				ApplyRowHeightCore(current, value);
			}
		}
		void ApplyRowHeightToExistingRows(CellRange range, float value) {
			foreach (Row row in ActiveSheet.Rows.GetExistingRows(range.TopLeft.Row, range.BottomRight.Row, false))
				ApplyRowHeightCore(row, value);
		}
		void ApplyRowHeightCore(Row row, float value) {
			row.SetCustomHeight(value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatRowsLocked);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class ShowRowHeightFormContextMenuCommand : ShowRowHeightFormCommand {
		public ShowRowHeightFormContextMenuCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatRowHeightContextMenuItem; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = IsEntireRowSelection();
			state.Visible = state.Enabled;
		}
		bool IsEntireRowSelection() {
			int count = ActiveSheet.Selection.SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				if (!ActiveSheet.Selection.SelectedRanges[i].IsRowRangeInterval())
					return false;
			return count > 0;
		}
	}
}
