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

using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CollapseExpandCommandBase (abstract)
	public abstract class CollapseExpandGroupModelCommandBase : SpreadsheetModelCommand {
		#region static
		public static CollapseExpandGroupModelCommandBase CreateInstance(bool rows, Worksheet worksheet, int startIndex, int endIndex, int buttonIndex) {
			if (rows)
				return new CollapseExpandRowGroupCommand(worksheet, startIndex, endIndex, buttonIndex);
			else
				return new CollapseExpandColumnGroupCommand(worksheet, startIndex, endIndex, buttonIndex);
		}
		#endregion
		#region Fields
		int startIndex;
		int endIndex;
		int buttonIndex;
		#endregion
		protected CollapseExpandGroupModelCommandBase(Worksheet worksheet, int startIndex, int endIndex, int buttonIndex)
			: base(worksheet) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.buttonIndex = buttonIndex;
		}
		#region Properties
		public bool Collapse { get; set; }
		protected int Level { get; set; }
		protected bool IsCollapsed { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			Worksheet.NeedRowUnhideNotificated = false;
			Worksheet.NeedColumnUnhideNotificated = false;
			StartExecute(buttonIndex);
			int from = endIndex < buttonIndex ? endIndex : startIndex;
			int step = endIndex < buttonIndex ? -1 : 1;
			for (int i = from; i <= endIndex && i >= startIndex; i += step)
				if (Collapse || AllowUnhide(i))
					Modify(i);
			ResetCache();
			Worksheet.NeedRowUnhideNotificated = true;
			Worksheet.NeedColumnUnhideNotificated = true;
		}
		protected abstract void Modify(int index);
		protected abstract void StartExecute(int index);
		protected abstract bool AllowUnhide(int index);
		protected abstract void ResetCache();
	}
	#endregion
	#region CollapseExpandColumnGroupCommand
	public class CollapseExpandColumnGroupCommand : CollapseExpandGroupModelCommandBase {
		public CollapseExpandColumnGroupCommand(Worksheet worksheet, int startIndex, int endIndex, int buttonIndex)
			: base(worksheet, startIndex, endIndex, buttonIndex) {
		}
		protected override void Modify(int index) {
				if (Collapse)
					Worksheet.HideColumns(index, index, false);
				else
					Worksheet.UnhideColumns(index, index, false);
		}
		protected override void StartExecute(int index) {
			Worksheet.Columns.GetIsolatedColumn(index).IsCollapsed = Collapse;
		}
		protected override bool AllowUnhide(int index) {
			Column currentColumn = Worksheet.Columns.GetIsolatedColumn(index);
			if (Level == 0 || currentColumn.OutlineLevel <= Level || !IsCollapsed) {
				Level = currentColumn.OutlineLevel;
				IsCollapsed = currentColumn.IsCollapsed;
				return true;
			}
			return false;
		}
		protected override void ResetCache() {
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region CollapseExpandRowGroupCommand
	public class CollapseExpandRowGroupCommand : CollapseExpandGroupModelCommandBase {
		public CollapseExpandRowGroupCommand(Worksheet worksheet, int startIndex, int endIndex, int buttonIndex)
			: base(worksheet, startIndex, endIndex, buttonIndex) {
		}
		protected override void Modify(int index) {
			Row row = Worksheet.Rows.GetRow(index);
			if (row.IsHidden != Collapse) {
				if (Collapse)
					Worksheet.HideRows(index, index, false);
				else
					Worksheet.UnhideRows(index, index, false);
			}
		}
		protected override void StartExecute(int index) {
			Worksheet.Rows.GetRow(index).IsCollapsed = Collapse;
		}
		protected override bool AllowUnhide(int index) {
			Row currentRow = Worksheet.Rows.GetRow(index);
			if (Level == 0 || currentRow.OutlineLevel <= Level || !IsCollapsed) {
				Level = currentRow.OutlineLevel;
				IsCollapsed = currentRow.IsCollapsed;
				return true;
			}
			return false;
		}
		protected override void ResetCache() {
			Worksheet.RowGroupCache = null;
		}
	}
	#endregion
	#region GroupUngroupCommandBase (abstract)
	public abstract class GroupUngroupCommandBase : SpreadsheetModelCommand {
		#region static
		public static GroupUngroupCommandBase CreateGroupInstance(bool rows, Worksheet worksheet, CellRange range) {
			if (rows)
				return new GroupRowsCommand(worksheet, range.TopRowIndex, range.BottomRowIndex);
			else
				return new GroupColumnsCommand(worksheet, range.LeftColumnIndex, range.RightColumnIndex);
		}
		#endregion
		#region Fields
		protected internal static int MaxGroupIndex = 7;
		protected internal static int MinGroupIndex = 0;
		int startIndex;
		int endIndex;
		#endregion
		protected GroupUngroupCommandBase(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}
		#region Properties
		public int StartIndex { get { return startIndex; } }
		public int EndIndex { get { return endIndex; } }
		public bool Collapse { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			for (int i = StartIndex; i <= EndIndex; i++)
				Modify(i);
			ResetCache();
			DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout);
		}
		public IColumnRange GetReadonlyColumnRange(int index) {
			return Worksheet.Columns.GetReadonlyColumnRange(index);
		}
		public Column GetIsolatedColumn(int index) {
			return Worksheet.Columns.GetIsolatedColumn(index);
		}
		protected internal abstract void Modify(int index);
		protected abstract void ResetCache();
		protected internal override bool Validate() {
			return Worksheet.PivotTables.GetRangesItems(Worksheet.Selection.SelectedRanges, true).Count == 0;
		}
	}
	#endregion
	#region GroupRowsCommand
	public class GroupRowsCommand : GroupUngroupCommandBase {
		public GroupRowsCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			int index = Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow ? EndIndex + 1 : StartIndex - 1;
			if (IndicesChecker.CheckIsRowIndexValid(index)) {
				Row row = Worksheet.Rows.TryGetRow(index);
				bool isCollapsed = row == null ? false : row.IsCollapsed;
				if(isCollapsed != Collapse)
					Worksheet.Rows[index].IsCollapsed = Collapse;
			}
		}
		protected internal override void Modify(int index) {
			Row row = Worksheet.Rows[index];
			if (row.OutlineLevel >= MaxGroupIndex)
				return;
			row.OutlineLevel++;
			if (Collapse && !row.IsHidden) {
				Worksheet.HideRows(index, index, false);
			}
			this.Worksheet.Properties.FormatProperties.OutlineLevelRow = Math.Max(row.OutlineLevel, this.Worksheet.Properties.FormatProperties.OutlineLevelRow);
		}
		protected override void ResetCache() {
			Worksheet.RowGroupCache = null;
		}
	}
	#endregion
	#region GroupColumnsCommand
	public class GroupColumnsCommand : GroupUngroupCommandBase {
		public GroupColumnsCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			int index = Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight ? EndIndex + 1 : StartIndex - 1;
			if (IndicesChecker.CheckIsColumnIndexValid(index)) {
				bool isCollapsed = GetReadonlyColumnRange(index).IsCollapsed;
				if(isCollapsed != Collapse)
					GetIsolatedColumn(index).IsCollapsed = Collapse;
			}
		}
		protected internal override void Modify(int index) {
			if (GetReadonlyColumnRange(index).OutlineLevel >= MaxGroupIndex)
				return;
			Column column = GetIsolatedColumn(index);
			column.OutlineLevel++;
			if (Collapse)
				Worksheet.HideColumns(index, index, false);
			this.Worksheet.Properties.FormatProperties.OutlineLevelCol = Math.Max(column.OutlineLevel, this.Worksheet.Properties.FormatProperties.OutlineLevelCol);
		}
		protected override void ResetCache() {
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region AutoOutlineCommand
	public class AutoCreateOutlineCommand : SpreadsheetModelCommand {
		#region fields
		readonly CellRange range;
		List<GroupItem> rowGroups;
		List<GroupItem> columnGroups;
		#endregion
		public AutoCreateOutlineCommand(Worksheet worksheet, CellRange range)
			: base(worksheet) {
			this.range = range;
		}
		#region Properties
		protected virtual bool Rows { get { return true; } }
		protected virtual bool Columns { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			ProcessCreatedGroups();
		}
		protected internal override bool Validate() {
			if (range == null)
				return false;
			rowGroups = new List<GroupItem>();
			columnGroups = new List<GroupItem>();
			foreach (ICell cell in range.GetExistingCellsEnumerable()) {
				CellRange rangeRef = GetRangeReference(cell);
				if (rangeRef == null)
					continue;
				CellPosition position = cell.Position;
				if (Rows && rangeRef.Width == 1 && position.Column == rangeRef.LeftColumnIndex && (position.Row > rangeRef.BottomRowIndex || position.Row < rangeRef.TopRowIndex))
					CreateRowGroup(position.Row, rangeRef.TopRowIndex, rangeRef.BottomRowIndex);
				if (Columns && rangeRef.Height == 1 && position.Row == rangeRef.TopRowIndex && (position.Column > rangeRef.RightColumnIndex || position.Column < rangeRef.LeftColumnIndex))
					CreateColumnGroup(position.Column, rangeRef.LeftColumnIndex, rangeRef.RightColumnIndex);
			}
			return rowGroups.Count > 0 || columnGroups.Count > 0;
		}
		void ProcessCreatedGroups() {
			bool clearColumns = columnGroups.Count > 0 && Worksheet.Properties.FormatProperties.OutlineLevelCol > 0;
			bool clearRows = rowGroups.Count > 0 && Worksheet.Properties.FormatProperties.OutlineLevelRow > 0;
			if (clearColumns || clearRows) {
				ClearWorksheetOutlineCommand command = new ClearWorksheetOutlineCommand(Worksheet, clearColumns, clearRows);
				command.Execute();
			}
			if (rowGroups.Count > 0) {
				foreach (GroupItem group in rowGroups) {
					GroupRowsCommand command = new GroupRowsCommand(Worksheet, group.Start, group.End);
					command.Execute();
				}
				Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow = !rowGroups[0].ButtonBeforeStart;
				Worksheet.RowGroupCache = null;
			}
			if (columnGroups.Count > 0) {
				foreach (GroupItem group in columnGroups) {
					GroupColumnsCommand command = new GroupColumnsCommand(Worksheet, group.Start, group.End);
					command.Execute();
				}
				Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight = !columnGroups[0].ButtonBeforeStart;
				Worksheet.ColumnGroupCache = null;
			}
		}
		void CreateRowGroup(int buttonPosition, int start, int end) {
			bool buttonBeforeStart = buttonPosition < start;
			GroupItem group = new GroupItem(true, start, end, 0, buttonBeforeStart);
			if (!rowGroups.Contains(group))
				rowGroups.Add(group);
		}
		void CreateColumnGroup(int buttonPosition, int start, int end) {
			bool buttonBeforeStart = buttonPosition < start;
			GroupItem group = new GroupItem(false, start, end, 0, buttonBeforeStart);
			if (!columnGroups.Contains(group))
				columnGroups.Add(group);
		}
		CellRange GetRangeReference(ICell cell) {
			if (!cell.HasFormula)
				return null;
			WorkbookDataContext context = Worksheet.DataContext;
			context.PushCurrentCell(cell);
			try {
				FormulaReferencedRangeRPNVisitor visitor = new FormulaReferencedRangeRPNVisitor(Worksheet.SheetId, 1, 1, context);
				CellRange result = visitor.Perform(cell.Formula.Expression) as CellRange;
				if (result == null)
					return null;
				CellRange topLeftRef = GetRangeReference(Worksheet[result.TopLeft]);
				CellRange bottomRightRef = GetRangeReference(Worksheet[result.BottomRight]);
				if (topLeftRef != null && !result.TopLeft.EqualsPosition(topLeftRef.TopLeft))
					result.TopLeft = new CellPosition(Math.Min(result.LeftColumnIndex, topLeftRef.LeftColumnIndex), Math.Min(result.TopRowIndex, topLeftRef.TopRowIndex));
				if (bottomRightRef != null && !bottomRightRef.BottomRight.EqualsPosition(result.BottomRight))
					result.BottomRight = new CellPosition(Math.Max(result.RightColumnIndex, bottomRightRef.RightColumnIndex), Math.Max(result.BottomRowIndex, bottomRightRef.BottomRowIndex));
				return result;
			}
			finally {
				context.PopCurrentCell();
			}
		}
	}
	#endregion
	#region AutoRowsOutlineCommand
	public class AutoCreateRowsOutlineCommand : AutoCreateOutlineCommand {
		public AutoCreateRowsOutlineCommand(Worksheet worksheet, int startRow, int endRow)
			: base(worksheet, CellIntervalRange.CreateRowInterval(worksheet, startRow, PositionType.Absolute, endRow, PositionType.Absolute)) {
		}
		#region Properties
		protected override bool Rows { get { return true; } }
		protected override bool Columns { get { return false; } }
		#endregion
	}
	#endregion
	#region AutoColumnsOutlineCommand
	public class AutoCreateColumnsOutlineCommand : AutoCreateOutlineCommand {
		public AutoCreateColumnsOutlineCommand(Worksheet worksheet, int startColumn, int endColumn)
			: base(worksheet, CellIntervalRange.CreateColumnInterval(worksheet, startColumn, PositionType.Absolute, endColumn, PositionType.Absolute)) {
		}
		#region Properties
		protected override bool Rows { get { return false; } }
		protected override bool Columns { get { return true; } }
		#endregion
	}
	#endregion
	#region UngroupCommandBase (abstract)
	public abstract class UngroupCommandBase : GroupUngroupCommandBase {
		#region static
		public static UngroupCommandBase CreateUngroupInstance(bool rows, Worksheet worksheet, CellRange range) {
			if (rows)
				return new UngroupRowsCommand(worksheet, range.TopRowIndex, range.BottomRowIndex);
			else
				return new UngroupColumnsCommand(worksheet, range.LeftColumnIndex, range.RightColumnIndex);
		}
		#endregion
		protected UngroupCommandBase(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		#region Properties
		public bool UnhideCollapsed { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			for (int i = StartIndex; i <= EndIndex; i++)
				Modify(i);
			UpdateCollapsed();
			UpdateSheetProperties();
			ResetCache();
			DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout);
		}
		protected abstract void UpdateSheetProperties();
		protected abstract void UpdateCollapsed();
	}
	#endregion
	#region UngroupRowsCommand
	public class UngroupRowsCommand : UngroupCommandBase {
		public UngroupRowsCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		protected internal override void Modify(int index) {
			Row row = Worksheet.Rows[index];
			if (row.OutlineLevel <= MinGroupIndex)
				return;
			row.OutlineLevel--;
			if (row.IsHidden && UnhideCollapsed)
				row.IsHidden = false;
		}
		protected override void UpdateCollapsed() {
			int index = Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow ? EndIndex + 1 : StartIndex - 1;
			if (IndicesChecker.CheckIsRowIndexValid(index)) {
				Row row = Worksheet.Rows.TryGetRow(index);
				bool isCollapsed = row == null ? false : row.IsCollapsed;
				if(isCollapsed)
					Worksheet.Rows[index].IsCollapsed = false;
			}
		}
		protected override void UpdateSheetProperties() {
			int maxLevel = 0;
			foreach (Row row in this.Worksheet.Rows.GetExistingRows())
				maxLevel = Math.Max(maxLevel, row.OutlineLevel);
			this.Worksheet.Properties.FormatProperties.OutlineLevelRow = maxLevel;
		}
		protected override void ResetCache() {
			Worksheet.RowGroupCache = null;
		}
	}
	#endregion
	#region UngroupColumnsCommand
	public class UngroupColumnsCommand : UngroupCommandBase {
		public UngroupColumnsCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		protected internal override void Modify(int index) {
			if (GetReadonlyColumnRange(index).OutlineLevel <= MinGroupIndex)
				return;
			Column column = GetIsolatedColumn(index);
			column.OutlineLevel--;
			if (column.IsHidden && UnhideCollapsed)
				Worksheet.UnhideColumns(index, index, false);
		}
		protected override void UpdateCollapsed() {
			int index = Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight ? EndIndex + 1 : StartIndex - 1;
			if (IndicesChecker.CheckIsColumnIndexValid(index)) {
				if(GetReadonlyColumnRange(index).IsCollapsed)
					GetIsolatedColumn(index).IsCollapsed = false;
			}
		}
		protected override void UpdateSheetProperties() {
			int maxLevel = 0;
			foreach (Column column in this.Worksheet.Columns.GetExistingColumns())
				maxLevel = Math.Max(maxLevel, column.OutlineLevel);
			this.Worksheet.Properties.FormatProperties.OutlineLevelCol = maxLevel;
		}
		protected override void ResetCache() {
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region ClearOutlineCommand  (abstract)
	public abstract class ClearOutlineCommandBase : SpreadsheetModelCommand {
		#region static
		public static ClearOutlineCommandBase CreateInstance(Worksheet worksheet, CellRangeBase range) {
			if (range != null) {
				if (range.RangeType != CellRangeType.UnionRange && (range as CellRange).IsWholeWorksheetRange())
					return new ClearWorksheetOutlineCommand(worksheet);
				if (range.RangeType == CellRangeType.UnionRange || range.Height != 1 || range.Width != 1)
					return new ClearRangeOutlineCommand(worksheet, range);
			}
			return new ClearWorksheetOutlineCommand(worksheet);
		}
		#endregion
		#region fields
		int startIndex;
		int endIndex;
		bool clearInterval;
		List<int> indicies;
		#endregion
		protected ClearOutlineCommandBase(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.clearInterval = true;
		}
		protected ClearOutlineCommandBase(Worksheet worksheet, List<int> indicies)
			: base(worksheet) {
			this.indicies = indicies;
			this.clearInterval = false;
		}
		protected internal override void ExecuteCore() {
			if (NeedClear()) {
				if (clearInterval)
					for (int i = startIndex; i <= endIndex; i++)
						Modify(i);
				else
					foreach (int index in indicies)
						Modify(index);
				UpdateSheetProperties();
				ResetCache();
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout);
			}
		}
		protected abstract bool NeedClear();
		protected abstract void Modify(int index);
		protected abstract void UpdateSheetProperties();
		protected abstract void ResetCache();
	}
	#endregion
	#region ClearRowsOutlineCommand
	public class ClearRowsOutlineCommand : ClearOutlineCommandBase {
		public ClearRowsOutlineCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		public ClearRowsOutlineCommand(Worksheet worksheet, List<int> indicies)
			: base(worksheet, indicies) {
		}
		protected override bool NeedClear() {
			return this.Worksheet.Properties.FormatProperties.OutlineLevelRow > 0;
		}
		protected override void Modify(int index) {
			Row row = this.Worksheet.Rows.TryGetRow(index);
			if (row != null && row.OutlineLevel > 0)
				row.OutlineLevel = 0;
		}
		protected override void UpdateSheetProperties() {
			int maxLevel = 0;
			foreach (Row row in this.Worksheet.Rows.GetExistingRows())
				maxLevel = Math.Max(maxLevel, row.OutlineLevel);
			this.Worksheet.Properties.FormatProperties.OutlineLevelRow = maxLevel;
		}
		protected internal override bool Validate() {
			return Worksheet.Properties.FormatProperties.OutlineLevelRow > 0 && base.Validate();
		}
		protected override void ResetCache() {
			Worksheet.RowGroupCache = null;
		}
	}
	#endregion
	#region ClearColumnsOutlineCommand
	public class ClearColumnsOutlineCommand : ClearOutlineCommandBase {
		public ClearColumnsOutlineCommand(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet, startIndex, endIndex) {
		}
		public ClearColumnsOutlineCommand(Worksheet worksheet, List<int> indicies)
			: base(worksheet, indicies) {
		}
		protected override bool NeedClear() {
			return this.Worksheet.Properties.FormatProperties.OutlineLevelCol > 0;
		}
		protected override void Modify(int index) {
			Column column = this.Worksheet.Columns.TryGetColumn(index);
			if (column != null && column.OutlineLevel > 0) {
				if (column.StartIndex != column.EndIndex)
					column = this.Worksheet.Columns.GetIsolatedColumn(index);
				column.OutlineLevel = 0;
			}
		}
		protected override void UpdateSheetProperties() {
			int maxLevel = 0;
			foreach (Column column in this.Worksheet.Columns.GetExistingColumns())
				maxLevel = Math.Max(maxLevel, column.OutlineLevel);
			this.Worksheet.Properties.FormatProperties.OutlineLevelCol = maxLevel;
		}
		protected internal override bool Validate() {
			return Worksheet.Properties.FormatProperties.OutlineLevelCol > 0 && base.Validate();
		}
		protected override void ResetCache() {
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region ClearRangeOutlineCommand
	public class ClearRangeOutlineCommand : ClearOutlineCommandBase {
		#region fields
		bool clearAllRows;
		bool clearAllColumns;
		List<int> clearedRows;
		List<int> clearedColumns;
		#endregion
		public ClearRangeOutlineCommand(Worksheet worksheet, CellRangeBase range)
			: base(worksheet, 0, 0) {
			InitializeClearedFields();
			if (range.RangeType == CellRangeType.UnionRange)
				CollectClearedFields((range as CellUnion).InnerCellRanges);
			else
				CollectClearedFields(new List<CellRangeBase> { range });
		}
		protected internal override void ExecuteCore() {
			ClearOutlineCommandBase command = null;
			if (clearAllColumns || clearAllRows)
				command = new ClearWorksheetOutlineCommand(Worksheet, clearAllColumns, clearAllRows);
			if (command != null) {
				command.Execute();
				command = null;
			}
			if (!clearAllColumns)
				command = new ClearColumnsOutlineCommand(Worksheet, clearedColumns);
			else if (!clearAllRows)
				command = new ClearRowsOutlineCommand(Worksheet, clearedRows);
			if (command != null)
				command.Execute();
		}
		protected override bool NeedClear() {
			return true;
		}
		protected override void Modify(int index) { }
		protected override void UpdateSheetProperties() {
		}
		protected override void ResetCache() {
		}
		void CollectClearedFields(List<CellRangeBase> ranges) {
			foreach (CellRange range in ranges) {
				if (!clearAllRows)
					CollectRowInterval(range);
				if (!clearAllColumns)
					CollectColumnInterval(range);
				if (clearAllRows && clearAllColumns)
					break;
			}
		}
		void CollectRowInterval(CellRange range) {
			if (range.IsColumnRangeInterval()) {
				clearAllRows = true;
				return;
			}
			CollectInterval(clearedRows, range.TopRowIndex, range.BottomRowIndex);
		}
		void CollectColumnInterval(CellRange range) {
			if (range.IsRowRangeInterval()) {
				clearAllColumns = true;
				return;
			}
			CollectInterval(clearedColumns, range.LeftColumnIndex, range.RightColumnIndex);
		}
		void InitializeClearedFields() {
			clearAllRows = false;
			clearAllColumns = false;
			clearedRows = new List<int>();
			clearedColumns = new List<int>();
		}
		void CollectInterval(List<int> intervalsIndicies, int startIndex, int endIndex) {
			for (int i = startIndex; i <= endIndex; i++)
				if (!intervalsIndicies.Contains(i))
					intervalsIndicies.Add(i);
		}
	}
	#endregion
	#region ClearWorksheetOutlineCommand
	public class ClearWorksheetOutlineCommand : ClearOutlineCommandBase {
		#region fields
		bool clearColumns;
		bool clearRows;
		#endregion
		public ClearWorksheetOutlineCommand(Worksheet worksheet)
			: this(worksheet, true, true) {
		}
		public ClearWorksheetOutlineCommand(Worksheet worksheet, bool clearColumns, bool clearRows)
			: base(worksheet, 0, 0) {
			this.clearColumns = clearColumns;
			this.clearRows = clearRows;
		}
		protected internal override void ExecuteCore() {
			if (this.Worksheet.Properties.FormatProperties.OutlineLevelCol > 0 && clearColumns)
				foreach (Column column in this.Worksheet.Columns.GetExistingColumns())
					if (column.OutlineLevel > 0) {
						Worksheet.UnhideColumns(column.StartIndex, column.EndIndex, false);
						column.OutlineLevel = 0;
					}
			if (this.Worksheet.Properties.FormatProperties.OutlineLevelRow > 0 && clearRows)
				foreach (Row row in this.Worksheet.Rows.GetExistingRows())
					if (row.OutlineLevel > 0) {
						Worksheet.UnhideRows(row.Index, row.Index, false);
						row.OutlineLevel = 0;
					}
			if (NeedClear()) {
				UpdateSheetProperties();
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout);
			}
		}
		protected override bool NeedClear() {
			return this.Worksheet.Properties.FormatProperties.OutlineLevelRow > 0 || this.Worksheet.Properties.FormatProperties.OutlineLevelCol > 0;
		}
		protected override void Modify(int index) { }
		protected override void UpdateSheetProperties() {
			if (clearColumns)
				this.Worksheet.Properties.FormatProperties.OutlineLevelCol = 0;
			if (clearRows)
				this.Worksheet.Properties.FormatProperties.OutlineLevelRow = 0;
		}
		protected override void ResetCache() {
			Worksheet.RowGroupCache = null;
			Worksheet.ColumnGroupCache = null;
		}
	}
	#endregion
	#region SubtotalModelCommand
	public class SubtotalModelCommand : ErrorHandledWorksheetCommand {
		#region fields
		readonly CellRangeBase selection;
		CellRange range;
		#endregion
		public SubtotalModelCommand(Worksheet worksheet, IErrorHandler errorHandler, CellRangeBase range)
			: base(worksheet, errorHandler) {
			this.selection = range;
		}
		#region Properties
		public int ChangedColumnIndex { get; set; }
		public List<int> SubTotalColumnIndices { get; set; }
		public int FunctionType { get; set; }
		public string Text { get; set; }
		public bool NeedInsertTextColumn { get; set; }
		public int InsertedRowsCount { get; set; }
		public bool PageBreakBeetwenGroups { get; set; }
		#endregion
		protected internal override bool Validate() {
			range = selection as CellRange;
			if (selection.RangeType == CellRangeType.UnionRange)
				return HandleError(new ModelErrorInfo(ModelErrorType.UnionRangeNotAllowed));
			if (Worksheet.PivotTables.ContainsItemsInRange(range, true))
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotTableSubtotalListPlace));
			return base.Validate();
		}
		protected internal override void ExecuteCore() {
			bool sumsBelow = Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow;
			int replaceCorrection = NeedInsertTextColumn ? 0 : 1;
			NeedInsertTextColumn = NeedInsertTextColumn && SubTotalColumnIndices.Contains(ChangedColumnIndex);
			int posibleInsertedRowCount = GetPosibleInsertedRowCount();
			List<int> insertedRows = GetInsertedRows(sumsBelow, range.GetSubColumnRange(ChangedColumnIndex + replaceCorrection, ChangedColumnIndex + replaceCorrection), posibleInsertedRowCount);
			int insertedGrandRow = sumsBelow ? range.BottomRowIndex + 1 : range.TopRowIndex;
			int grandCorrection = sumsBelow ? 0 : 1;
			if (NeedInsertTextColumn)
				Worksheet.InsertColumns(range.LeftColumnIndex, 1, InsertCellsFormatMode.FormatAsPrevious, ErrorHandler);
			int textColumn = SubTotalColumnIndices.Contains(ChangedColumnIndex) ? range.LeftColumnIndex : range.LeftColumnIndex + ChangedColumnIndex;
			if (posibleInsertedRowCount == -1 || posibleInsertedRowCount >= insertedRows.Count + 1)
				InsertSubtotal(insertedGrandRow, range.TopRowIndex + grandCorrection, range.BottomRowIndex + grandCorrection, textColumn, string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Subtotal_Grand), Text), sumsBelow, sumsBelow);
			else
				grandCorrection = 0;
			for (int i = 0; i < insertedRows.Count; i++) {
				int row = insertedRows[i];
				string value = GetCellText(Worksheet[NeedInsertTextColumn ? range.LeftColumnIndex + ChangedColumnIndex + 1 : textColumn, row + (sumsBelow ? -1 : 1)]);
				int startRow = GetStartRow(insertedRows, i, sumsBelow);
				int endRow = GetEndRow(insertedRows, i, sumsBelow);
				bool needInsertPageBreak = sumsBelow ? i != 0 : i != insertedRows.Count - 1;
				InsertSubtotal(row + grandCorrection, startRow + grandCorrection, endRow + grandCorrection, textColumn, string.Format("{0} {1}", value, Text), sumsBelow, needInsertPageBreak);
			}
			InsertedRowsCount = insertedRows.Count + 1;
		}
		int GetPosibleInsertedRowCount() {
			foreach (Row row in range.Worksheet.Rows.GetExistingVisibleRows(0, IndicesChecker.MaxRowIndex, true))
				return IndicesChecker.MaxRowIndex - row.Index;
			return -1;
		}
		int GetStartRow(List<int> insertedRows, int currentIndex, bool sumsBelow) {
			if (sumsBelow)
				return currentIndex == insertedRows.Count - 1 ? range.TopRowIndex : insertedRows[currentIndex + 1];
			else
				return insertedRows[currentIndex] + 1;
		}
		int GetEndRow(List<int> insertedRows, int currentIndex, bool sumsBelow) {
			if (sumsBelow)
				return insertedRows[currentIndex] - 1;
			else
				return currentIndex == 0 ? range.BottomRowIndex + 1 : insertedRows[currentIndex - 1];
		}
		void InsertSubtotal(int row, int startRow, int endRow, int textColumn, string text, bool sumsBelow, bool needInsertPageBreak) {
			Worksheet.InsertRows(row, 1, sumsBelow ? InsertCellsFormatMode.FormatAsPrevious : InsertCellsFormatMode.FormatAsNext, ErrorHandler);
			Worksheet.Rows.GetRow(row).OutlineLevel = Worksheet.Rows.GetRow(row + (sumsBelow ? -1 : 1)).OutlineLevel;
			foreach (int subColumn in SubTotalColumnIndices) {
				int column = range.LeftColumnIndex + subColumn + (NeedInsertTextColumn ? 1 : 0);
				CellRange subtotalRange = CutRange(new CellRange(Worksheet, new CellPosition(column, startRow), new CellPosition(column, endRow)));
				string listSeparator = Worksheet.DataContext.Culture.TextInfo.ListSeparator;
				ICell subTotalCell = Worksheet[column, row];
				subTotalCell.FormulaBody = string.Format("={0}({1}{2}{3})", XtraSpreadsheetFunctionNameLocalizer.GetString(XtraSpreadsheetFunctionNameStringId.Subtotal), FunctionType, listSeparator, subtotalRange.ToString());
				subTotalCell.ClearFormat();
			}
			ICell cell = Worksheet[textColumn, row];
			cell.Value = text;
			cell.Font.Bold = true;
			GroupRowsCommand command = new GroupRowsCommand(Worksheet, startRow, endRow);
			command.Execute();
			if (needInsertPageBreak && PageBreakBeetwenGroups)
				Worksheet.RowBreaks.Add(sumsBelow ? endRow + 2 : startRow - 1);
			Worksheet.AutoFilter.ReApplyFilter();
		}
		CellRange CutRange(CellRange cellRange) {
			for (int i = cellRange.TopRowIndex; i <= cellRange.BottomRowIndex; i++) {
				ICell cell = Worksheet.TryGetCell(cellRange.LeftColumnIndex, i);
				if (cell != null && cell.Value != VariantValue.Empty) {
					if (cellRange.TopRowIndex != i)
						cellRange.TopRowIndex = i;
					break;
				}
			}
			for (int i = cellRange.BottomRowIndex; i >= cellRange.TopRowIndex; i--) {
				ICell cell = Worksheet.TryGetCell(cellRange.LeftColumnIndex, i);
				if (cell != null && cell.Value != VariantValue.Empty) {
					if (cellRange.BottomRowIndex != i)
						cellRange.BottomRowIndex = i;
					break;
				}
			}
			return cellRange;
		}
		List<int> GetInsertedRows(bool sumsBelow, CellRange range, int posibleInsertedRowCount) {
			List<int> result = new List<int>();
			string previous = string.Empty;
			int previousPosition = -1;
			foreach (Cell cell in range.GetExistingCellsEnumerable(!sumsBelow)) {
				if (posibleInsertedRowCount < 1)
					return result;
				string current = GetCellText(cell);
				if (previousPosition != -1 && current != previous && !string.IsNullOrEmpty(previous)) {
					if (sumsBelow)
						result.Insert(0, previousPosition + 1);
					else result.Add(previousPosition);
					posibleInsertedRowCount--;
				}
				previousPosition = cell.RowIndex;
				previous = current;
			}
			if (posibleInsertedRowCount < 1)
				return result;
			if (sumsBelow)
				result.Insert(0, previousPosition + 1);
			else result.Add(previousPosition);
			return result;
		}
		string GetCellText(ICell cell) {
			CellRange mergedCell = Worksheet.MergedCells.FindMergedCell(cell.ColumnIndex, cell.RowIndex);
			if (mergedCell == null)
				return cell.Text;
			return Worksheet.MergedCells.GetMergedCellText(mergedCell);
		}
	}
	#endregion
	#region SubtotalRemoveCommand
	public class SubtotalRemoveCommand : ErrorHandledWorksheetCommand {
		#region fields
		readonly CellRange range;
		List<int> removedRowIndices;
		bool hasTextColumn;
		#endregion
		public SubtotalRemoveCommand(Worksheet worksheet, CellRange range, IErrorHandler errorHandler)
			: base(worksheet, errorHandler) {
			this.range = range;
			this.hasTextColumn = true;
		}
		#region Properties
		public bool HasTextColumn { get { return hasTextColumn; } }
		#endregion
		protected internal override void ExecuteCore() {
			foreach (int rowIndex in removedRowIndices)
				Worksheet.RemoveRows(rowIndex, 1, ErrorHandler);
			range.BottomRowIndex -= removedRowIndices.Count;
			ClearRowsOutlineCommand command = new ClearRowsOutlineCommand(Worksheet, range.TopRowIndex, range.BottomRowIndex);
			command.Execute();
		}
		protected internal override bool Validate() {
			removedRowIndices = new List<int>();
			for (int i = range.Height - 1; i >= 0; i--)
				foreach (Cell cell in range.GetSubRowRange(i, i).GetExistingCellsEnumerable())
					if (cell.HasFormula && ContainsSubtotal(cell)) {
						if (cell.Position.Column != range.LeftColumnIndex)
							hasTextColumn = false;
						removedRowIndices.Add(cell.RowIndex);
						break;
					}
			return removedRowIndices.Count > 0;
		}
		bool ContainsSubtotal(Cell cell) {
			FormulaFindFunctionVisitor visitor = new FormulaFindFunctionVisitor(cell.Formula.Expression);
			return visitor.FindFunction(0x0158);
		}
	}
	#endregion
	#region ShowDetailCommandBase (abstract)
	public abstract class ShowDetailCommandBase : SpreadsheetModelCommand {
		#region fields
		readonly CellRange range;
		#endregion
		protected ShowDetailCommandBase(Worksheet worksheet, CellRange range)
			: base(worksheet) {
			this.range = range;
		}
		#region Properties
		public CellRange Range { get { return range; } }
		protected abstract bool IsRow { get; }
		protected abstract bool ButtonBeforeStart { get; }
		protected abstract int MaxModelIndexValue { get; }
		#endregion
		protected internal override void ExecuteCore() {
			int startIndex = GetStartIndex(range);
			if (!ButtonBeforeStart) 
				startIndex--;
			startIndex = Math.Max(startIndex, 0);
			int endIndex = GetEndIndex(range);
			if (ButtonBeforeStart)
				endIndex++;
			endIndex = Math.Min(endIndex, MaxModelIndexValue);
			GroupList groupList = GetGroupCollectror().StrongCollectGroups(startIndex, endIndex);
			foreach (GroupItem group in groupList.Groups)
				if (group.Collapsed && group.Level == groupList.MinCollapsedLevel && (group.End >= startIndex))
					ShowGroup(group.Start, group.End, group.ButtonPosition);
		}
		void ShowGroup(int start, int end, int buttonPosition) {
			CollapseExpandGroupModelCommandBase command = CollapseExpandGroupModelCommandBase.CreateInstance(IsRow, Worksheet, start, end, buttonPosition);
			command.Collapse = false;
			command.Execute();
		}
		protected abstract int GetStartIndex(CellRange range);
		protected abstract int GetEndIndex(CellRange range);
		protected abstract GroupCollector GetGroupCollectror();
	}
	#endregion
	#region ShowColumnDetailCommand
	public class ShowColumnDetailCommand : ShowDetailCommandBase {
		public ShowColumnDetailCommand(Worksheet worksheet, CellRange range)
			: base(worksheet, range) {
		}
		#region Properties
		protected override bool IsRow { get { return false; } }
		protected override bool ButtonBeforeStart { get { return !Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight; } }
		protected override int MaxModelIndexValue { get { return IndicesChecker.MaxColumnIndex; } }
		#endregion
		protected override int GetStartIndex(CellRange range) {
			return range.LeftColumnIndex;
		}
		protected override int GetEndIndex(CellRange range) {
			return range.RightColumnIndex;
		}
		protected override GroupCollector GetGroupCollectror() {
			return new ColumnGroupCollector(Worksheet);
		}
	}
	#endregion
	#region ShowRowDetailCommand
	public class ShowRowDetailCommand : ShowDetailCommandBase {
		public ShowRowDetailCommand(Worksheet worksheet, CellRange range)
			: base(worksheet, range) {
		}
		#region Properties
		protected override bool IsRow { get { return true; } }
		protected override bool ButtonBeforeStart { get { return !Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow; } }
		protected override int MaxModelIndexValue { get { return IndicesChecker.MaxRowIndex; } }
		#endregion
		protected override int GetStartIndex(CellRange range) {
			return range.TopRowIndex;
		}
		protected override int GetEndIndex(CellRange range) {
			return range.BottomRowIndex;
		}
		protected override GroupCollector GetGroupCollectror() {
			return new RowGroupCollector(Worksheet);
		}
	}
	#endregion
	#region HideDetailCommandBase (abstract)
	public abstract class HideDetailCommandBase : SpreadsheetModelCommand {
		#region fields
		readonly CellRange range;
		#endregion
		protected HideDetailCommandBase(Worksheet worksheet, CellRange range)
			: base(worksheet) {
			this.range = range;
		}
		#region Properties
		public CellRange Range { get { return range; } }
		protected abstract bool IsRow { get; }
		protected abstract bool ButtonBeforeStart { get; }
		protected abstract int MaxModelIndexValue { get; }
		#endregion
		protected internal override void ExecuteCore() {
			int startIndex = GetStartIndex(range);
			int endIndex = GetEndIndex(range);
			GroupList groupList = new GroupList(Worksheet, IsRow);
			for (int i = startIndex; i <= endIndex; i++)
				groupList.ProcessLevel(GetOutlineLevel(i), i, IsHidenElement(i));
			bool collapsed = true;
			if (groupList.Groups.Count > 0) {
				foreach (GroupItem group in groupList.Groups) {
					if (group.Collapsed)
						continue;
					else
						collapsed = false;
					if (group.Start == startIndex)
						group.Start = FindGroupStart(group.Level, startIndex);
					HideGroup(group.Start, group.End, group.ButtonPosition);
				}
			}
			if (groupList.Groups.Count == 0 || collapsed) {
				GroupItem group = FindGroup(startIndex);
				if (group != null)
					HideGroup(group.Start, group.End, group.ButtonPosition);
			}
		}
		GroupItem FindGroup(int modelIndex) {
			int level = GetOutlineLevel(modelIndex);
			int start = FindGroupStart(level, modelIndex);
			int end = FindGroupEnd(level, modelIndex);
			if (end > -1 && end >= start)
				return new GroupItem(IsRow, start, end, level, IsRow ? !Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow : !Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight);
			else
				return null;
		}
		void HideGroup(int start, int end, int buttonPosition) {
			CollapseExpandGroupModelCommandBase command = CollapseExpandGroupModelCommandBase.CreateInstance(IsRow, Worksheet, start, end, buttonPosition);
			command.Collapse = true;
			command.Execute();
		}
		int FindGroupStart(int level, int modelIndex) {
			if (level == 0) {
				level = GetOutlineLevel(modelIndex - (ButtonBeforeStart ? -1 : 1));
				if (level != 0) {
					if (ButtonBeforeStart)
						return modelIndex + 1;
					else modelIndex--;
				}
			}
			for (int i = modelIndex; i >= 0; i--)
				if (GetOutlineLevel(i) < level)
					return i + 1;
			return 0;
		}
		int FindGroupEnd(int level, int modelIndex) {
			if (level == 0) {
				level = GetOutlineLevel(modelIndex - (ButtonBeforeStart ? -1 : 1));
				if (level != 0) {
					if (!ButtonBeforeStart)
						return modelIndex - 1;
					else modelIndex++;
				}
			}
			for (int i = modelIndex + 1; i < MaxModelIndexValue; i++)
				if (GetOutlineLevel(i) < level)
					return i - 1;
			return -1;
		}
		protected abstract int GetStartIndex(CellRange range);
		protected abstract int GetEndIndex(CellRange range);
		protected abstract int GetOutlineLevel(int modelIndex);
		protected abstract bool IsHidenElement(int modelIndex);
	}
	#endregion
	#region HideColumnDetailCommand
	public class HideColumnDetailCommand : HideDetailCommandBase {
		public HideColumnDetailCommand(Worksheet worksheet, CellRange range)
			: base(worksheet, range) {
		}
		#region Properties
		protected override bool IsRow { get { return false; } }
		protected override bool ButtonBeforeStart { get { return !Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight; } }
		protected override int MaxModelIndexValue { get { return IndicesChecker.MaxColumnIndex; } }
		#endregion
		protected override int GetStartIndex(CellRange range) {
			return range.LeftColumnIndex;
		}
		protected override int GetEndIndex(CellRange range) {
			return range.RightColumnIndex;
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Column column = Worksheet.Columns.TryGetColumn(modelIndex);
			return column == null ? 0 : column.OutlineLevel;
		}
		protected override bool IsHidenElement(int modelIndex) {
			Column column = Worksheet.Columns.TryGetColumn(modelIndex);
			return column == null ? false : column.IsHidden;
		}
	}
	#endregion
	#region HideRowDetailCommand
	public class HideRowDetailCommand : HideDetailCommandBase {
		public HideRowDetailCommand(Worksheet worksheet, CellRange range)
			: base(worksheet, range) {
		}
		#region Properties
		protected override bool IsRow { get { return true; } }
		protected override bool ButtonBeforeStart { get { return !Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow; } }
		protected override int MaxModelIndexValue { get { return IndicesChecker.MaxRowIndex; } }
		#endregion
		protected override int GetStartIndex(CellRange range) {
			return range.TopRowIndex;
		}
		protected override int GetEndIndex(CellRange range) {
			return range.BottomRowIndex;
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Row row = Worksheet.Rows.TryGetRow(modelIndex);
			return row == null ? 0 : row.OutlineLevel;
		}
		protected override bool IsHidenElement(int modelIndex) {
			Row row = Worksheet.Rows.TryGetRow(modelIndex);
			return row == null ? false : row.IsHidden;
		}
	}
	#endregion
	#region OutlineLevelCommandBase (abstract)
	public abstract class OutlineLevelCommandBase : SpreadsheetModelCommand {
		#region Fields
		int startIndex;
		int endIndex;
		int step;
		#endregion
		protected OutlineLevelCommandBase(Worksheet worksheet, int level)
			: base(worksheet) {
			Level = level;
		}
		public static OutlineLevelCommandBase CreateInstance(bool rows, Worksheet worksheet, int targetLevel) {
			if (rows)
				return new OutlineLevelRowCommand(worksheet, targetLevel);
			else
				return new OutlineLevelColumnCommand(worksheet, targetLevel);
		}
		public int Level { get; private set; }
		protected int StartIndex { get { return startIndex; } set { startIndex = value; } }
		protected int EndIndex { get { return endIndex; } set { endIndex = value; } }
		protected int Step { get { return step; } set { step = value; } }
		protected int PreviousLevel { get; set; }
		protected internal override void BeginExecute() {
			base.BeginExecute();
			Worksheet.NeedRowUnhideNotificated = false;
			Worksheet.NeedColumnUnhideNotificated = false;
		}
		protected internal override void EndExecute() {
			Worksheet.NeedRowUnhideNotificated = true;
			Worksheet.NeedColumnUnhideNotificated = true;
			base.EndExecute();
		}
		protected internal override void ExecuteCore() {
			StartExecute();
			int from = step == -1 ? endIndex : startIndex;
			for (int i = from; i <= endIndex && i >= startIndex; i += step)
				Modify(i);
			FinishExecute();
		}
		protected abstract void StartExecute();
		protected abstract void Modify(int index);
		protected abstract void FinishExecute();
	}
	#endregion
	#region OutlineLevelColumnCommand
	public class OutlineLevelColumnCommand : OutlineLevelCommandBase {
		public OutlineLevelColumnCommand(Worksheet worksheet, int level)
			: base(worksheet, level) {
		}
		protected override void StartExecute() {
			StartIndex = Worksheet.Columns.First.StartIndex;
			EndIndex = Worksheet.Columns.Last.EndIndex;
			if (Worksheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight) {
				EndIndex = Math.Min(IndicesChecker.MaxColumnIndex, EndIndex + 1);
				Step = 1;
			}
			else {
				StartIndex = Math.Max(0, StartIndex - 1);
				Step = -1;
			}
		}
		protected override void Modify(int index) {
			int innerIndex = Worksheet.Columns.TryGetColumnIndex(index);
			if (innerIndex < 0) {
				if (PreviousLevel == Level) {
					Column currentColumn = Worksheet.Columns.GetIsolatedColumn(index);
					currentColumn.IsCollapsed = true;
				}
				PreviousLevel = 0;
			}
			else {
				Column currentColumn = Worksheet.Columns.GetIsolatedColumn(index);
				if (currentColumn.OutlineLevel < Level) {
					if (currentColumn.OutlineLevel > 0)
						Worksheet.UnhideColumns(index, index, false);
					currentColumn.IsCollapsed = PreviousLevel == Level;
				}
				else
					Worksheet.HideColumns(index, index, false);
				PreviousLevel = currentColumn.OutlineLevel;
			}
		}
		protected override void FinishExecute() {
			Worksheet.ColumnGroupCache = null;
		}
		protected internal override bool Validate() {
			return Worksheet.Properties.FormatProperties.OutlineLevelCol > 0;
		}
	}
	#endregion
	#region OutlineLevelRowCommand
	public class OutlineLevelRowCommand : OutlineLevelCommandBase {
		public OutlineLevelRowCommand(Worksheet worksheet, int level)
			: base(worksheet, level) {
		}
		protected override void StartExecute() {
			StartIndex = Worksheet.Rows.First.Index;
			EndIndex = Worksheet.Rows.Last.Index;
			if (Worksheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow) {
				EndIndex = Math.Min(IndicesChecker.MaxRowIndex, EndIndex + 1);
				Step = 1;
			}
			else {
				StartIndex = Math.Max(0, StartIndex - 1);
				Step = -1;
			}
		}
		protected override void Modify(int index) {
			Row currentRow = Worksheet.Rows.TryGetRow(index);
			if (currentRow == null) {
				if (PreviousLevel == Level) {
					currentRow = Worksheet.Rows.GetRow(index);
					currentRow.IsCollapsed = true;
				}
				PreviousLevel = 0;
			}
			else {
				if (currentRow.OutlineLevel < Level) {
					if (currentRow.OutlineLevel > 0)
						Worksheet.UnhideRows(index, index, false);
					currentRow.IsCollapsed = PreviousLevel == Level;
				}
				else
					Worksheet.HideRows(index, index, false);
				PreviousLevel = currentRow.OutlineLevel;
			}
		}
		protected override void FinishExecute() {
			Worksheet.RowGroupCache = null;
			Worksheet.Rows.CheckForEmptyRows(StartIndex, EndIndex);
		}
		protected internal override bool Validate() {
			return Worksheet.Properties.FormatProperties.OutlineLevelRow > 0;
		}
	}
	#endregion
}
