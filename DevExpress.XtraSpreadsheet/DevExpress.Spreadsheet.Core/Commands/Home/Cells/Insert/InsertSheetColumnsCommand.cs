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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertRemoveSheetColumnsCommandBase (abstract class)
	public abstract class InsertRemoveSheetColumnsCommandBase : InsertRemoveSheetElementCommandBase {
		protected InsertRemoveSheetColumnsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override int CompareRanges(CellRange x, CellRange y) {
			int leftColumnX = x.TopLeft.Column;
			int leftColumnY = y.TopLeft.Column;
			if (leftColumnX == leftColumnY)
				return 0;
			if (leftColumnX < leftColumnY)
				return 1;
			return -1;
		}
		protected internal override void CalculateSelectedBounds(List<Rectangle> bounds, IList<CellRange> ranges) {
			if (ranges.Count == 0)
				return;
			CellRange leftRange = GetLeftRange(ranges);
			ranges.Remove(leftRange);
			int count = ranges.Count;
			int leftColumn = leftRange.TopLeft.Column;
			int rightColumn = leftRange.BottomRight.Column;
			for (int i = count - 1; i >= 0; i--) {
				CellRange currentRange = ranges[i];
				int currentRightColumn = currentRange.BottomRight.Column;
				if (currentRange.TopLeft.Column <= rightColumn && currentRightColumn >= rightColumn) {
					rightColumn = currentRightColumn;
					ranges.Remove(currentRange);
				}
			}
			bounds.Add(new Rectangle(leftColumn, 0, rightColumn - leftColumn, 0));
			CalculateSelectedBounds(bounds, ranges);
		}
		protected internal virtual CellRange GetLeftRange(IList<CellRange> ranges) {
			int count = ranges.Count;
			Debug.Assert(count > 0);
			CellRange leftRange = ranges[0];
			for (int i = 1; i < count; i++) {
				CellRange currentRange = ranges[i];
				int currentLeftColumn = currentRange.TopLeft.Column;
				int currentRightColumn = currentRange.BottomRight.Column;
				int leftColumn = leftRange.TopLeft.Column;
				int rightColumn = leftRange.BottomRight.Column;
				if (currentLeftColumn < leftColumn || (currentLeftColumn == leftColumn && currentRightColumn > rightColumn))
					leftRange = currentRange;
			}
			return leftRange;
		}
		protected internal override void Modify(Rectangle bounds) {
			ModifyCore(bounds.Left, bounds.Right - bounds.Left + 1);
		}
		protected internal override bool CheckOperationEnabledCore(CellRange range) {
			NotificationChecks fastCheck = NotificationChecks.AutoFilter | NotificationChecks.MergedCells | NotificationChecks.Table;
			return !HasEntireRowSelected(range) && CanModifyColumn(range, fastCheck) == null;
		}
		protected internal override IModelErrorInfo CanModifyCore(CellRangeBase range) {
			return CanModifyColumn(range, NotificationChecks.All);
		}
		protected internal bool HasEntireRowSelected(CellRange range) {
			return range.TopLeft.Column == 0 && range.BottomRight.Column == DocumentModel.ActiveSheet.MaxColumnCount - 1;
		}
		protected internal IModelErrorInfo CanModifyColumn(CellRangeBase range, NotificationChecks checks) {
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			CellIntervalRange columnRange = CellIntervalRange.CreateColumnInterval(ActiveSheet, leftColumn, PositionType.Relative, rightColumn, PositionType.Relative);
			return CanModifyColumnCore(columnRange, checks);
		}
		protected internal abstract void ModifyCore(int startIndex, int count);
		protected internal abstract IModelErrorInfo CanModifyColumnCore(CellIntervalRange columnRange, NotificationChecks checks);
	}
	#endregion
	#region InsertSheetColumnsCommand
	public class InsertSheetColumnsCommand : InsertRemoveSheetColumnsCommandBase {
		public InsertSheetColumnsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertSheetColumns; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetColumns; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsDescription; } }
		public override string ImageName { get { return "InsertSheetColumns"; } }
		public InsertCellsFormatMode FormatMode { get { return InsertCellsFormatMode.FormatAsPrevious; } }
		#endregion
		protected internal override void ModifyCore(int startIndex, int count) {
			ActiveSheet.InsertColumns(startIndex, count, FormatMode, ErrorHandler);
		}
		protected internal override IModelErrorInfo CanModifyColumnCore(CellIntervalRange columnRange, NotificationChecks checks) {
			return DocumentModel.CanRangeInsert(columnRange, InsertCellMode.ShiftCellsRight, FormatMode, checks);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.Insert, CheckOperationEnabled());
			ApplyActiveSheetProtection(state, !Protection.InsertColumnsLocked);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region InsertSheetColumnsContextMenuItemCommand
	public class InsertSheetColumnsContextMenuItemCommand : InsertSheetColumnsCommand {
		public InsertSheetColumnsContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertSheetColumnsContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_InsertSheetColumnsContextMenuItemDescription; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled &= ActiveSheet.Selection.AsRange().IsColumnRangeInterval();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
