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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatNumberIncreaseDecimalCommand
	public class FormatNumberIncreaseDecimalCommand : FormatNumberIncreaseDecreaseDecimalCommand {
		public FormatNumberIncreaseDecimalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatNumberIncreaseDecimal; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatNumberIncreaseDecimal; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatNumberIncreaseDecimalDescription; } }
		public override string ImageName { get { return "FormatNumberIncreaseDecimal"; } }
		#endregion
		protected internal override string ProcessFormat(string formatString) {
			return NumberFormat.IncreaseDecimal(formatString);
		}
	}
	#endregion
	#region FormatNumberIncreaseDecreaseDecimalCommand (abstract class)
	public abstract class FormatNumberIncreaseDecreaseDecimalCommand : ChangeSelectedCellsNumberFormatCommand {
		protected FormatNumberIncreaseDecreaseDecimalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected internal override string FormatString { get { return String.Empty; } }
		#endregion
		protected internal abstract string ProcessFormat(string formatString);
		protected internal override string GetNewValue(IValueBasedCommandUIState<string> state) {
			return state.Value;
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				return;
			string actualFormatString = valueBasedState.Value;
			if (string.IsNullOrEmpty(actualFormatString)) {
				ICell cell = GetActiveCell();
				CellRange mergedCell = ActiveSheet.MergedCells.GetMergedCellRange(cell);
				if (mergedCell != null) {
					cell = ActiveSheet.MergedCells.TryGetCell(mergedCell);
					if (cell == null)
						return;
				}
				string cellDisplayValue = GetCellDisplayValue(cell, mergedCell);
				if (string.IsNullOrEmpty(cellDisplayValue))
					return;
				actualFormatString = CreateNumberFormatByCellDisplayValue(cellDisplayValue);
			}
			valueBasedState.Value = ProcessFormat(actualFormatString);
			base.ModifyDocumentModelCore(state);
		}
		string GetCellDisplayValue(ICell cell, CellRange mergedCell) {
			VariantValue cellValue = cell.Value;
			if (!cellValue.IsNumeric)
				return null;
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = new Layout.Engine.CellFormatStringMeasurer(cell);
			parameters.AvailableSpaceWidth = GetCellTextWidth(cell, mergedCell);
			NumberFormatResult result = NumberFormat.Generic.Format(cellValue, DocumentModel.DataContext, parameters);
			return result.Text;
		}
		int GetCellTextWidth(ICell cell, CellRange mergedCell) {
			IColumnWidthCalculationService columnWidthCalculator = DocumentModel.GetService<IColumnWidthCalculationService>();
			int width = columnWidthCalculator.CalculateColumnWidth(ActiveSheet, cell.ColumnIndex, DocumentModel.MaxDigitWidth, DocumentModel.MaxDigitWidthInPixels);
			if (mergedCell != null)
				for (int i = mergedCell.LeftColumnIndex + 1; i <= mergedCell.RightColumnIndex; ++i)
					width += columnWidthCalculator.CalculateColumnWidth(ActiveSheet, i, DocumentModel.MaxDigitWidth, DocumentModel.MaxDigitWidthInPixels);
			int fourPixelsPadding = (int)Math.Round(DocumentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(4, DocumentModel.DpiX));
			return Math.Max(0, width - fourPixelsPadding);
		}
		string CreateNumberFormatByCellDisplayValue(string text) {
			int expIndex = text.IndexOfInvariantCultureIgnoreCase("E");
			if (expIndex >= 0)
				text = text.Remove(expIndex, text.Length - expIndex);
			int index = text.IndexOf(DocumentModel.DataContext.Culture.NumberFormat.NumberDecimalSeparator);
			int decimalPlaces = index >= 0 ? (text.Length - index - 1) : 0;
			string result = "0";
			if (decimalPlaces > 0)
				result += "." + new string('0', decimalPlaces);
			if (expIndex > 0)
				result += "E+00";
			return result;
		}
	}
	#endregion
}
