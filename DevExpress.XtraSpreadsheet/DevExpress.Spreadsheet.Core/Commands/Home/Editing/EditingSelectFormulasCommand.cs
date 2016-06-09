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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Office.Services.Implementation;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region EditingSelectFormulasCommand
	public class EditingSelectFormulasCommand : EditingSelectSpecificCellsCommand {
		public EditingSelectFormulasCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Fields
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingSelectFormulas; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingSelectFormulas; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingSelectFormulasDescription; } }
		#endregion
		protected override Predicate<ICellBase> GetPredicate() {
			return Predicate;
		}
		static bool Predicate(ICellBase baseCell) {
			Cell cell = baseCell as Cell;
			if (cell == null)
				return false;
			return cell.HasFormula;
		}
	}
	#endregion
	#region EditingSelectSpecificCellsCommand (abstract class)
	public abstract class EditingSelectSpecificCellsCommand : SpreadsheetSelectionCommand {
		protected EditingSelectSpecificCellsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool ExpandsSelection { get { return false; } }
		protected internal override bool ChangeSelection() {
			CellMergeHelper helper = new CellMergeHelper();
			foreach (ICellBase cell in GetCells())
				helper.Add(cell);
			IList<CellRangeBase> ranges = helper.GetMergedRanges();
			if (ranges == null || ranges.Count <= 0) {
				IThreadSyncService service = InnerControl.GetService<IThreadSyncService>();
				if (service != null)
					service.EnqueueInvokeInUIThread(new Action(delegate { Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_NoCellsWereFound)); }));
				return false;
			}
			Selection.SetSelectedRanges(ranges, false);
			return true;
		}
		protected virtual IEnumerable<ICellBase> GetCells() {
			return new Enumerable<ICellBase>(new FilteredEnumerator<ICellBase>(new EntireWorksheetExistingCellRangeEnumerator(ActiveSheet), GetPredicate()));
		}
		protected abstract Predicate<ICellBase> GetPredicate();
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class CellMergeHelper {
		CellRange currentRange;
		Dictionary<int, CellRangeGroup> rangeGroups = new Dictionary<int, CellRangeGroup>();
		public void Add(ICellBase cell) {
			if (currentRange != null && TryMerge(currentRange, cell))
				return;
			currentRange = cell.GetRange();
			AddToGroup(currentRange);
		}
		public List<CellRangeBase> GetMergedRanges() {
			List<CellRangeBase> result = new List<CellRangeBase>();
			foreach (CellRangeGroup group in rangeGroups.Values) {
				group.Merge();
				result.AddRange(group);
			}
			return result;
		}
		void AddToGroup(CellRange range) {
			int key = range.TopLeft.Column;
			if (!rangeGroups.ContainsKey(key)) {
				CellRangeGroup group = new CellRangeGroup();
				group.Add(range);
				rangeGroups.Add(key, group);
			}
			else
				rangeGroups[key].Add(range);
		}
		bool TryMerge(CellRange range, ICellBase cell) {
			bool canMerge = cell.RowIndex == range.TopLeft.Row && cell.ColumnIndex == (range.BottomRight.Column + 1);
			if (canMerge)
				range.BottomRight = new CellPosition(cell.ColumnIndex, cell.RowIndex);
			return canMerge;
		}
	}
	public class CellRangeGroup : List<CellRange> {
		public CellRangeGroup()
			: base() {
		}
		public void Merge() {
			for (int i = Count - 1; i > 0; i--) {
				CellRange range = this[i - 1];
				if (TryMerge(range, this[i]))
					RemoveAt(i);
			}
		}
		bool TryMerge(CellRange thisRange, CellRange range) {
			bool canMerge = thisRange.BottomRight.Column == range.BottomRight.Column && thisRange.BottomRight.Row == (range.TopLeft.Row - 1);
			if (canMerge)
				thisRange.BottomRight = range.BottomRight;
			return canMerge;
		}
	}
}
