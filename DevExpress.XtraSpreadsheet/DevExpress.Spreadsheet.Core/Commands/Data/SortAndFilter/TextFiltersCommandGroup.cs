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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region FilterTextCommandGroup
	public class FilterTextCommandGroup : DataSpecificFilterCommandGroupBase {
		public FilterTextCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterTextFiltersCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTextFilters; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTextFiltersDescription; } }
		#endregion
		protected internal override bool IsValidDataForFilter(SuggestedFilterTypeInfo filterTypeInfo) {
			return filterTypeInfo.IsTextFilter;
		}
	}
	#endregion
	#region DataSpecificFilterCommandGroupBase (abstract class)
	public abstract class DataSpecificFilterCommandGroupBase : SpreadsheetCommandGroup {
		protected DataSpecificFilterCommandGroupBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.AutoFiltersLocked);
			DataSortOrFilterAccessor accessor = new DataSortOrFilterAccessor(DocumentModel);
			accessor.GetSortOrFilterRange();
			if (accessor.Filter != null) {
				SuggestedFilterTypeInfo filterTypeInfo = new SuggestedFilterTypeInfo();
				filterTypeInfo.Calculate(accessor.Filter, ActiveSheet.Selection.ActiveCell.Column);
				state.Visible = IsValidDataForFilter(filterTypeInfo);
			}
			else
				state.Visible = false;
		}
		protected internal abstract bool IsValidDataForFilter(SuggestedFilterTypeInfo filterTypeInfo);
	}
	#endregion
	public class SuggestedFilterTypeInfo {
		public bool IsDateTimeFilter { get; private set; }
		public bool IsNumberFilter { get; private set; }
		public bool IsTextFilter { get; private set; }
		public void Calculate(AutoFilterBase filter, int modelColumnIndex) {
			CellRange range = filter.GetFilterColumnDataRange(modelColumnIndex);
			int maxCells = 255;
			int textCells = 0;
			int numberCells = 0;
			int dateTimeCells = 0;
			foreach (ICellBase cell in range.GetExistingCellsEnumerable()) {
				if (IsTextCell(cell))
					textCells++;
				else if (IsDateTimeCell(cell))
					dateTimeCells++;
				else if (IsNumberCell(cell))
					numberCells++;
				maxCells--;
				if (maxCells <= 0)
					break;
			}
			if (textCells > 0)
				IsTextFilter = true;
			if (numberCells > 0)
				IsNumberFilter = true;
			if (dateTimeCells > 0)
				IsDateTimeFilter = true;
		}
		static bool IsTextCell(ICellBase cell) {
			return cell.Value.IsText;
		}
		static bool IsNumberCell(ICellBase cell) {
			return cell.Value.IsNumeric;
		}
		public static bool IsDateTimeCell(ICellBase cellBase) {
			if (cellBase == null || !IsNumberCell(cellBase))
				return false;
			Cell cell = cellBase as Cell;
			if (cell == null)
				return false;
			return cell.ActualFormat.IsDateTime;
		}
		public static string GetCellText(DocumentModel documentModel, ICellBase cellBase) {
			if (cellBase == null)
				return String.Empty;
			Cell cell = cellBase as Cell;
			if (cell != null)
				return cell.Text;
			else
				return cellBase.Value.ToText(documentModel.DataContext).GetTextValue(documentModel.SharedStringTable);
		}
	}
}
