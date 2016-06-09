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
using System.Diagnostics;
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertRemoveSheetElementCommandBase (abstract class)
	public abstract class InsertRemoveSheetElementCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected InsertRemoveSheetElementCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			if (InnerControl.IsInplaceEditorActive)
				return;
			IModelErrorInfo error = CanModify();
			if (error != null) {
				if (ErrorHandler.HandleError(error) == ErrorHandlingResult.Abort)
					return;
			}
			DocumentModel.BeginUpdateFromUI();
			try {
				List<CellRange> sortedRanges = GetSortedRanges(ActiveSheet.Selection.SelectedRanges);
				List<Rectangle> bounds = new List<Rectangle>();
				CalculateSelectedBounds(bounds, sortedRanges);
				int count = bounds.Count - 1;
				for (int i = count; i >= 0; i--) {
					Modify(bounds[i]);
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal virtual List<CellRange> GetSortedRanges(IList<CellRange> ranges) {
			List<CellRange> result = new List<CellRange>();
			result.AddRange(ranges);
			result.Sort(CompareRanges);
			return result;
		}
		protected internal abstract int CompareRanges(CellRange x, CellRange y);
		protected internal abstract void CalculateSelectedBounds(List<Rectangle> bounds, IList<CellRange> ranges);
		protected internal abstract void Modify(Rectangle bounds);
		protected internal bool CheckOperationEnabled() {
			if (InnerControl.IsInplaceEditorActive)
				return false;
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			foreach (CellRange range in selectedRanges) {
				Debug.Assert(range.RangeType != CellRangeType.UnionRange, "Incorrect range type");
				if (!CheckOperationEnabledCore(range))
					return false;
			}
			return true;
		}
		protected internal IModelErrorInfo CanModify() {
			CellRangeBase selectedRange = ActiveSheet.Selection.AsRange();
			if (selectedRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)selectedRange;
				if (union.HasNonConsistantRanges())
					return new ModelErrorInfo(ModelErrorType.DiffRangeTypesCanNotBeChanged);
				if (union.HasIntersectedRanges())
					return new ModelErrorInfo(ModelErrorType.IntersectedRangesCanNotBeChanged);
			}
			return CanModifyCore(selectedRange);
		}
		protected internal abstract IModelErrorInfo CanModifyCore(CellRangeBase range);
		protected internal abstract bool CheckOperationEnabledCore(CellRange range);
	}
	#endregion
	#region InsertRemoveSheetRowsCommandBase (abstract class)
	public abstract class InsertRemoveSheetRowsCommandBase : InsertRemoveSheetElementCommandBase {
		protected InsertRemoveSheetRowsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override int CompareRanges(CellRange x, CellRange y) {
			int topRowX = x.TopLeft.Row;
			int topRowY = y.TopLeft.Row;
			if (topRowX == topRowY)
				return 0;
			if (topRowX < topRowY)
				return 1;
			return -1;
		}
		protected internal override void CalculateSelectedBounds(List<Rectangle> bounds, IList<CellRange> ranges) {
			if (ranges.Count == 0)
				return;
			CellRange topRange = GetTopRange(ranges);
			ranges.Remove(topRange);
			int count = ranges.Count;
			int topRow = topRange.TopLeft.Row;
			int bottomRow = topRange.BottomRight.Row;
			for (int i = count - 1; i >= 0; i--) {
				CellRange currentRange = ranges[i];
				int currentBottomRow = currentRange.BottomRight.Row;
				if (currentRange.TopLeft.Row <= bottomRow && currentBottomRow >= bottomRow) {
					bottomRow = currentBottomRow;
					ranges.Remove(currentRange);
				}
			}
			bounds.Add(new Rectangle(0, topRow, 0, bottomRow - topRow));
			CalculateSelectedBounds(bounds, ranges);
		}
		protected internal virtual CellRange GetTopRange(IList<CellRange> ranges) {
			int count = ranges.Count;
			Debug.Assert(count > 0);
			CellRange topRange = ranges[0];
			for (int i = 1; i < count; i++) {
				CellRange currentRange = ranges[i];
				int currentTopRow = currentRange.TopLeft.Row;
				int currentBottomRow = currentRange.BottomRight.Row;
				int topRow = topRange.TopLeft.Row;
				int bottomRow = topRange.BottomRight.Row;
				if (currentTopRow < topRow || (currentTopRow == topRow && currentBottomRow > bottomRow))
					topRange = currentRange;
			}
			return topRange;
		}
		protected internal override void Modify(Rectangle bounds) {
			ModifyCore(bounds.Top, bounds.Bottom - bounds.Top + 1);
		}
		protected internal override IModelErrorInfo CanModifyCore(CellRangeBase range) {
			return CanModifyRow(range, NotificationChecks.All);
		}
		protected internal override bool CheckOperationEnabledCore(CellRange range) {
			NotificationChecks fastCheck = NotificationChecks.AutoFilter | NotificationChecks.MergedCells | NotificationChecks.Table;
			return !HasEntireColumnSelected(range) && CanModifyRow(range, fastCheck) == null;
		}
		protected internal bool HasEntireColumnSelected(CellRangeBase range) {
			return range.TopLeft.Row == 0 && range.BottomRight.Row == (range.Worksheet as Worksheet).MaxRowCount - 1;
		}
		protected internal IModelErrorInfo CanModifyRow(CellRangeBase range, NotificationChecks checks) {
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			CellIntervalRange rowRange = CellIntervalRange.CreateRowInterval(ActiveSheet, topRow, PositionType.Relative, bottomRow, PositionType.Relative);
			return CanModifyRowCore(rowRange, checks);
		}
		protected internal abstract void ModifyCore(int startIndex, int count);
		protected internal abstract IModelErrorInfo CanModifyRowCore(CellIntervalRange rowRange, NotificationChecks checks);
	}
	#endregion
	#region InsertSheetRowsCommand
	public class InsertSheetRowsCommand : InsertRemoveSheetRowsCommandBase {
		public InsertSheetRowsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertSheetRows; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetRows; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsDescription; } }
		public override string ImageName { get { return "InsertSheetRows"; } }
		public InsertCellsFormatMode FormatMode { get { return InsertCellsFormatMode.FormatAsPrevious; } }
		#endregion
		protected internal override void ModifyCore(int startIndex, int count) {
			ActiveSheet.InsertRows(startIndex, count, FormatMode, ErrorHandler);
		}
		protected internal override IModelErrorInfo CanModifyRowCore(CellIntervalRange rowRange, NotificationChecks checks) {
			return DocumentModel.CanRangeInsert(rowRange, InsertCellMode.ShiftCellsDown, FormatMode, checks);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Row.Insert, CheckOperationEnabled());
			ApplyActiveSheetProtection(state, !Protection.InsertRowsLocked);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region InsertSheetRowsContextMenuItemCommand
	public class InsertSheetRowsContextMenuItemCommand : InsertSheetRowsCommand {
		public InsertSheetRowsContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertSheetRowsContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetRowsContextMenuItemDescription; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled &= ActiveSheet.Selection.AsRange().IsRowRangeInterval();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
