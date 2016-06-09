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
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChangeRangeBordersCommand
	public class ChangeRangeBordersCommand : SpreadsheetModelCommand {
		#region Fields
		readonly CellRangeBase ranges;
		readonly MergedBorderInfo info;
		#endregion
		public ChangeRangeBordersCommand(CellRangeBase ranges, MergedBorderInfo info)
			: base(ranges.Worksheet as Worksheet) {
			this.ranges = ranges;
			this.info = info;
		}
		#region Properties
		public CellRangeBase Ranges { get { return ranges; } }
		public MergedBorderInfo Info { get { return info; } }
		#endregion
		protected internal override bool Validate() {
			return info.HasBorderLines;
		}
		protected internal override void ExecuteCore() {
			RangeBordersModifierCollection modifiers = CreateModifiers();
			modifiers.Apply();
		}
		protected RangeBordersModifierCollection CreateModifiers() {
			RangeBordersModifierCollection result = new RangeBordersModifierCollection();
			AddModifiers(result, ranges);
			return result;
		}
		void AddModifiers(RangeBordersModifierCollection collection, CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = range as CellUnion;
				List<CellRangeBase> listRanges = union.InnerCellRanges;
				int count = listRanges.Count;
				for (int i = count - 1; i >= 0; i--)
					AddModifiers(collection, listRanges[i]);
			} else
				AddModifierForSingleRange(collection, range as CellRange);
		}
		void AddModifierForSingleRange(RangeBordersModifierCollection modifiers, CellRange range) {
			if (modifiers.ContainsRange(range, modifiers.Count))
				return;
			int leftIndex = range.LeftColumnIndex;
			int topIndex = range.TopRowIndex;
			int rightIndex = range.RightColumnIndex;
			int bottomIndex = range.BottomRowIndex;
			bool entireAllRowsSelected = leftIndex == 0 && rightIndex == IndicesChecker.MaxColumnCount - 1;
			bool entireAllColumnsSelected = topIndex == 0 && bottomIndex == IndicesChecker.MaxRowCount - 1;
			if (entireAllColumnsSelected) {
				CellRangeBase columnRange = ExcludeColumnsRanges(modifiers, range);
				if (columnRange != null)
					modifiers.AddColumnsModifier(columnRange, range, entireAllRowsSelected, info);
			} else if (entireAllRowsSelected) {
				CellRangeBase rowRange = ExcludeRowsRanges(modifiers, range);
				if (rowRange != null)
					modifiers.AddRowsModifier(rowRange, range, info);
			}
			else
				modifiers.Add(new RangeBordersModifier(range, info));
		}
		CellRangeBase ExcludeColumnsRanges(RangeBordersModifierCollection modifiers, CellRange range) {
			CellRangeBase result = range.Clone();
			int count = modifiers.Count;
			for (int i = 0; i < count; i++) {
				ColumnsBordersModifier columnsModifier = modifiers[i] as ColumnsBordersModifier;
				if (columnsModifier != null) 
					result = result.ExcludeRange(columnsModifier.Range); 
			}
			return result; 
		}
		CellRangeBase ExcludeRowsRanges(RangeBordersModifierCollection modifiers, CellRange range) {
			CellRangeBase result = range.Clone();
			int count = modifiers.Count;
			for (int i = 0; i < count; i++) {
				RowsBordersModifier rowsModifier = modifiers[i] as RowsBordersModifier;
				if (rowsModifier != null) 
					result = result.ExcludeRange(rowsModifier.Range); 
			}
			return result; 
		}
	}
	#endregion
	#region RangeBordersModifierCollection
	public class RangeBordersModifierCollection {
		IList<RangeBordersModifier> innerList = new List<RangeBordersModifier>();
		#region Properties
		protected internal int Count { get { return innerList.Count; } }
		protected internal RangeBordersModifier this[int index] { get { return innerList[index]; } }
		protected IList<RangeBordersModifier> InnerList { get { return innerList; } }
		#endregion
		protected internal void Add(RangeBordersModifier item) {
			innerList.Add(item);
		}
		#region AddRowsModifier
		protected internal void AddRowsModifier(CellRangeBase rowRange, CellRange sourceRange, MergedBorderInfo sourceInfo) {
			if (rowRange.RangeType == CellRangeType.UnionRange)
				AddUnionRowsModifier(rowRange as CellUnion, sourceRange, sourceInfo);
			else {
				RowsBordersModifier modifier = GetRowsModifier(rowRange as CellRange, sourceRange, sourceInfo);
				Add(modifier);
			}
		}
		void AddUnionRowsModifier(CellUnion cellUnion, CellRange sourceRange, MergedBorderInfo sourceInfo) {
			List<CellRangeBase> innerRanges = cellUnion.InnerCellRanges;
			int count = innerRanges.Count;
			for (int i = 0; i < count; i++)
				AddRowsModifier(innerRanges[i], sourceRange, sourceInfo);
		}
		RowsBordersModifier GetRowsModifier(CellRange rowRange, CellRange sourceRange, MergedBorderInfo sourceInfo) {
			MergedBorderInfo rowInfo = sourceInfo.GetRowInfo(rowRange, sourceRange);
			return new RowsBordersModifier(rowRange, rowInfo);
		}
		#endregion
		#region AddColumnsModifier
		protected internal void AddColumnsModifier(CellRangeBase columnRange, CellRange sourceRange, bool entireAllRowsSelected, MergedBorderInfo sourceInfo) {
			if (columnRange.RangeType == CellRangeType.UnionRange)
				AddUnionColumnsModifier(columnRange as CellUnion, sourceRange, entireAllRowsSelected, sourceInfo);
			else {
				ColumnsBordersModifier modifier = GetColumnsModifier(columnRange as CellRange, sourceRange, entireAllRowsSelected, sourceInfo);
				Add(modifier);
			}
		}
		void AddUnionColumnsModifier(CellUnion cellUnion, CellRange sourceRange, bool entireAllRowsSelected, MergedBorderInfo sourceInfo) {
			List<CellRangeBase> innerRanges = cellUnion.InnerCellRanges;
			int count = innerRanges.Count;
			for (int i = 0; i < count; i++)
				AddColumnsModifier(innerRanges[i], sourceRange, entireAllRowsSelected, sourceInfo);
		}
		ColumnsBordersModifier GetColumnsModifier(CellRange columnRange, CellRange sourceRange, bool entireAllRowsSelected, MergedBorderInfo sourceInfo) {
			MergedBorderInfo columnInfo = sourceInfo.GetColumnInfo(columnRange, sourceRange, entireAllRowsSelected);
			if (entireAllRowsSelected)
				return new SheetBordersModifier(columnRange, columnInfo);
			return new ColumnsBordersModifier(columnRange, columnInfo);
		}
		#endregion
		protected internal bool ContainsCell(ICell cell, int count) {
			return ContainsCell(cell.ColumnIndex, cell.RowIndex, count);
		}
		protected internal bool ContainsCell(int columnIndex, int rowIndex, int count) {
			for (int i = 0; i < count; i++)
				if (innerList[i].Range.ContainsCell(columnIndex, rowIndex))
					return true;
			return false;
		}
		protected internal bool ContainsRange(CellRange range, int count) {
			for (int i = 0; i < count; i++)
				if (innerList[i].Range.ContainsRange(range))
					return true;
			return false;
		}
		protected internal bool TryGetFirstModifierIndex(CellRange range, int count, out int result) {
			for (int i = 0; i < count; i++) 
				if (innerList[i].Range.ContainsRange(range)) {
					result = i;
					return true;
				}
			result = -1;
			return false;
		}
		protected internal void Apply() {
			int count = Count;
			for (int i = 0; i < count; i++) {
				RangeBordersModifier modifier = innerList[i];
				modifier.Apply(this, i);
			}
		}
	}
	#endregion
	#region InterceptCellsInfo
	public struct InterceptCellsInfo {
		public int ModifierIndex { get; set; }
		public CellRange Range { get; set; }
		public Row Row { get; set; }
		public Column Column { get; set; }
		public bool HasIntervalRange { get; set; }
	}
	#endregion
	#region InterceptCellsEnumeratorBase (absract class)
	public abstract class InterceptCellsEnumeratorBase : IEnumerator<InterceptCellsInfo> {
		readonly int modifierIndex;
		readonly RangeBordersModifierCollection modifiers;
		protected InterceptCellsEnumeratorBase(RangeBordersModifierCollection modifiers, int modifierIndex) {
			Guard.ArgumentNotNull(modifiers, "modifiers");
			Guard.ArgumentNonNegative(modifierIndex, "modifierIndex");
			Guard.ArgumentNotNull(modifiers[modifierIndex], "targetModifier");
			this.modifiers = modifiers;
			this.modifierIndex = modifierIndex;
			CreateExistingEnumerator();
			Reset();
		}
		#region Properties
		protected Worksheet Sheet { get { return TargetRange.Worksheet as Worksheet; } }
		protected CellRange TargetRange { get { return modifiers[modifierIndex].Range; } }
		protected RangeBordersModifierCollection Modifiers { get { return modifiers; } }
		protected int ModifierIndex { get { return modifierIndex; } }
		protected CellRange CurrentRange { get; set; }
		protected int CurrentModifierIndex { get; set; }
		protected abstract int StartFirstIndex { get; }
		protected abstract int EndFirstIndex { get; }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator<ICell> Members
		public InterceptCellsInfo Current { get { return CreateCurrentInfo(); } }
		public bool MoveNext() {
			if (MoveNextExistingEnumerator()) {
				if (!SetCurrent())
					MoveNext();
				else 
					return true;
			}
			return false;
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current { get { return Current; } }
		public void Reset() {
			CurrentRange = null;
			CurrentModifierIndex = -1;
			ResetExistingEnumerator();
		}
		#endregion
		bool EqualsBordersWithoutColor(XlBorderLineStyle firstStyle, int firstColorIndex, XlBorderLineStyle insideStyle, int insideColorIndex, XlBorderLineStyle lastStyle, int lastColorIndex) {
			return firstStyle == insideStyle && firstColorIndex == insideColorIndex && insideStyle == lastStyle && insideColorIndex == lastColorIndex;
		}
		protected bool SkipVerticalBorder(bool hasBorder, XlBorderLineStyle? style, int? colorIndex, IActualBorderInfo info) {
			return hasBorder ? EqualsBordersWithoutColor(info.LeftLineStyle, info.LeftColorIndex, style.Value, colorIndex.Value, info.RightLineStyle, info.RightColorIndex) : true;
		}
		protected bool SkipHorizontalBorder(bool hasBorder, XlBorderLineStyle? style, int? colorIndex, IActualBorderInfo info) {
			return hasBorder ? EqualsBordersWithoutColor(info.TopLineStyle, info.TopColorIndex, style.Value, colorIndex.Value, info.BottomLineStyle, info.BottomColorIndex) : true;
		}
		protected bool SkipBordersCore(MergedBorderInfo info, int index) { 
			return !info.HasBorders || !Modifiers[index].GetHasBorder(CurrentRange) || Modifiers.ContainsRange(CurrentRange, index);
		}
		protected abstract void CreateExistingEnumerator();
		protected abstract void ResetExistingEnumerator();
		protected abstract bool MoveNextExistingEnumerator();
		protected abstract bool SetCurrent();
		protected abstract InterceptCellsInfo CreateCurrentInfo();
	}
	#endregion
	#region ColumnInterceptCellsEnumerator
	public class ColumnInterceptCellsEnumerator : InterceptCellsEnumeratorBase {
		IEnumerator<Row> rows;
		public ColumnInterceptCellsEnumerator(RangeBordersModifierCollection modifiers, int modifierIndex)
			: base(modifiers, modifierIndex) {
		}
		#region Properties
		protected override int StartFirstIndex { get { return TargetRange.LeftColumnIndex; } }
		protected override int EndFirstIndex { get { return TargetRange.RightColumnIndex; } }
		#endregion
		protected override void CreateExistingEnumerator() {
			rows = Sheet.Rows.GetExistingRowsEnumerator(0, IndicesChecker.MaxRowCount - 1);
		}
		protected override void ResetExistingEnumerator() {
			rows.Reset();
		}
		protected override bool MoveNextExistingEnumerator() {
			return rows.MoveNext();
		}
		protected override bool SetCurrent() {
			Row row = rows.Current;
			int rowIndex = row.Index;
			CurrentRange = new CellRange(Sheet, new CellPosition(StartFirstIndex, rowIndex), new CellPosition(EndFirstIndex, rowIndex));
			int index;
			RangeBordersModifier modifier;
			if (Modifiers.TryGetFirstModifierIndex(CurrentRange, ModifierIndex, out index)) {
				modifier = Modifiers[index];
				if (modifier is RowsBordersModifier || SkipBorders(row, modifier.Info, ModifierIndex))
					return false;
				if (modifier is ColumnsBordersModifier) {
					CurrentModifierIndex = index;
					return true;
				}
				return false;
			}
			modifier = Modifiers[ModifierIndex];
			if (!SkipBorders(row, modifier.Info, ModifierIndex) && row.ApplyStyle) {
				CurrentModifierIndex = ModifierIndex;
				return true;
			}
			return false;
		}
		protected override InterceptCellsInfo CreateCurrentInfo() {
			InterceptCellsInfo result = new InterceptCellsInfo();
			result.Row = rows.Current;
			result.ModifierIndex = CurrentModifierIndex;
			result.Range = CurrentRange;
			result.HasIntervalRange = CurrentRange.TopRowIndex == 0 && CurrentRange.BottomRowIndex == IndicesChecker.MaxRowIndex;
			return result;
		}
		bool SkipBorders(Row row, MergedBorderInfo info, int index) {
			if (SkipBordersCore(info, index) || !row.ApplyBorder)
				return true;
			return
				SkipVerticalBorder(info.HasLeftBorder, info.LeftLineStyle, info.LeftColorIndex, row) &&
				SkipVerticalBorder(info.HasRightBorder, info.RightLineStyle, info.RightColorIndex, row) &&
				SkipVerticalBorder(info.HasVerticalBorder, info.VerticalLineStyle, info.VerticalColorIndex, row) &&
				SkipHorizontalBorder(info.HasHorizontalBorder, info.HorizontalLineStyle, info.HorizontalColorIndex, row);
		}
	}
	#endregion
	#region RowInterceptCellsEnumerator
	public class RowInterceptCellsEnumerator : InterceptCellsEnumeratorBase {
		IEnumerator<Column> columns;
		public RowInterceptCellsEnumerator(RangeBordersModifierCollection modifiers, int modifierIndex)
			: base(modifiers, modifierIndex) {
		}
		#region Properties
		protected override int StartFirstIndex { get { return TargetRange.TopRowIndex; } }
		protected override int EndFirstIndex { get { return TargetRange.BottomRowIndex; } }
		#endregion
		protected override void CreateExistingEnumerator() {
			columns = Sheet.Columns.GetExistingColumnsEnumerator(0, IndicesChecker.MaxColumnCount - 1, false);
		}
		protected override void ResetExistingEnumerator() {
			columns.Reset();
		}
		protected override bool MoveNextExistingEnumerator() {
			return columns.MoveNext();
		}
		protected override bool SetCurrent() {
			Column column = columns.Current;
			CurrentRange = new CellRange(Sheet, new CellPosition(column.StartIndex, StartFirstIndex), new CellPosition(column.EndIndex, EndFirstIndex));
			int index;
			RangeBordersModifier modifier;
			if (Modifiers.TryGetFirstModifierIndex(CurrentRange, ModifierIndex, out index)) {
				modifier = Modifiers[index];
				if (modifier is ColumnsBordersModifier && !SkipBorders(column, modifier.Info, index)) {
					CurrentModifierIndex = index;
					return true;
				}
				return false;
			}
			modifier = Modifiers[ModifierIndex];
			if (!SkipBordersCore(modifier.Info, ModifierIndex) && column.FormatIndex != column.DocumentModel.StyleSheet.DefaultCellFormatIndex) {
				CurrentModifierIndex = ModifierIndex;
				return true;
			}
			return false;
		}
		protected override InterceptCellsInfo CreateCurrentInfo() {
			InterceptCellsInfo result = new InterceptCellsInfo();
			result.Range = CurrentRange;
			result.Column = columns.Current;
			result.ModifierIndex = CurrentModifierIndex;
			result.HasIntervalRange = CurrentRange.LeftColumnIndex == 0 && CurrentRange.RightColumnIndex == IndicesChecker.MaxColumnIndex;
			return result;
		}
		bool SkipBorders(Column column, MergedBorderInfo info, int index) {
			if (SkipBordersCore(info, index) || column.FormatIndex == column.DocumentModel.StyleSheet.DefaultCellFormatIndex)
				return true;
			return
				SkipHorizontalBorder(info.HasTopBorder, info.TopLineStyle, info.TopColorIndex, column) &&
				SkipHorizontalBorder(info.HasBottomBorder, info.BottomLineStyle, info.BottomColorIndex, column) &&
				SkipHorizontalBorder(info.HasHorizontalBorder, info.HorizontalLineStyle, info.HorizontalColorIndex, column) &&
				SkipVerticalBorder(info.HasVerticalBorder, info.VerticalLineStyle, info.VerticalColorIndex, column);
		}
	}
	#endregion
	#region RangeBordersFormattingHelper
	public static class RangeBordersFormattingHelper {
		#region HasBorderFormatting
		internal static bool HasBorderFormatting(ICell cell) {
			return cell != null ? cell.ApplyBorder ? true : cell.Style.ApplyBorder : false;
		}
		internal static bool HasBorderFormatting(Column column) {
			return column != null ? column.ApplyBorder ? true : column.Style.ApplyBorder : false;
		}
		internal static bool HasBorderFormatting(Row row) {
			return row != null ? row.ApplyBorder ? true : row.Style.ApplyBorder : false;
		}
		#endregion
		internal static IActualBorderInfo GetFakeCellActualBorderInfo(Worksheet sheet, int columnIndex, int rowIndex) {
			Row row = sheet.Rows.TryGetRow(rowIndex);
			if (HasBorderFormatting(row))
				return row;
			Column column = sheet.Columns.TryGetColumn(columnIndex);
			if (HasBorderFormatting(column))
				return column;
			return sheet.Workbook.StyleSheet.CellStyles.Normal.ActualBorder;
		}
		internal static IActualBorderInfo GetRowActualBorderInfo(Worksheet sheet, int rowIndex) {
			Row row = sheet.Rows.TryGetRow(rowIndex);
			if (HasBorderFormatting(row))
				return row;
			return sheet.Workbook.StyleSheet.CellStyles.Normal.ActualBorder;
		}
		internal static IActualBorderInfo GetColumnActualBorderInfo(Worksheet sheet, int columnIndex) {
			Column column = sheet.Columns.TryGetColumn(columnIndex);
			if (HasBorderFormatting(column))
				return column;
			return sheet.Workbook.StyleSheet.CellStyles.Normal.ActualBorder;
		}
	}
	#endregion
	#region RangeBordersModifier
	public class RangeBordersModifier {
		#region Fields
		readonly CellRange range;
		readonly MergedBorderInfo info;
		#endregion
		public RangeBordersModifier(CellRange range, MergedBorderInfo info) {
			this.range = range;
			this.info = info;
			this.info.SetColorIndexes(Sheet.Workbook);
		}
		#region Properties
		protected Worksheet Sheet { get { return (range.Worksheet as Worksheet); } }
		protected internal MergedBorderInfo Info { get { return info; } }
		protected internal CellRange Range { get { return range; } }
		protected internal int? LeftColorIndex { get { return info.LeftColorIndex; } }
		protected internal int? RightColorIndex { get { return info.RightColorIndex; } }
		protected internal int? TopColorIndex { get { return info.TopColorIndex; } }
		protected internal int? BottomColorIndex { get { return info.BottomColorIndex; } }
		protected internal int? HorizontalColorIndex { get { return info.HorizontalColorIndex; } }
		protected internal int? VerticalColorIndex { get { return info.VerticalColorIndex; } }
		protected internal int? DiagonalColorIndex { get { return info.DiagonalColorIndex; } }
		protected internal Color? LeftColor { get { return info.LeftColor; } }
		protected internal Color? RightColor { get { return info.RightColor; } }
		protected internal Color? TopColor { get { return info.TopColor; } }
		protected internal Color? BottomColor { get { return info.BottomColor; } }
		protected internal Color? HorizontalColor { get { return info.HorizontalColor; } }
		protected internal Color? VerticalColor { get { return info.VerticalColor; } }
		protected internal Color? DiagonalColor { get { return info.DiagonalColor; } }
		protected internal XlBorderLineStyle? LeftLineStyle { get { return info.LeftLineStyle; } }
		protected internal XlBorderLineStyle? RightLineStyle { get { return info.RightLineStyle; } }
		protected internal XlBorderLineStyle? TopLineStyle { get { return info.TopLineStyle; } }
		protected internal XlBorderLineStyle? BottomLineStyle { get { return info.BottomLineStyle; } }
		protected internal XlBorderLineStyle? HorizontalLineStyle { get { return info.HorizontalLineStyle; } }
		protected internal XlBorderLineStyle? VerticalLineStyle { get { return info.VerticalLineStyle; } }
		protected internal XlBorderLineStyle? DiagonalUpLineStyle { get { return info.DiagonalUpLineStyle; } }
		protected internal XlBorderLineStyle? DiagonalDownLineStyle { get { return info.DiagonalDownLineStyle; } }
		protected internal bool HasLeftBorder { get { return info.HasLeftBorder; } }
		protected internal bool HasRightBorder { get { return info.HasRightBorder; } }
		protected internal bool HasTopBorder { get { return info.HasTopBorder; } }
		protected internal bool HasBottomBorder { get { return info.HasBottomBorder; } }
		protected internal bool HasVerticalBorder { get { return info.HasVerticalBorder; } }
		protected internal bool HasHorizontalBorder { get { return info.HasHorizontalBorder; } }
		protected internal bool HasDiagonalUpBorder { get { return info.HasDiagonalUpBorder; } }
		protected internal bool HasDiagonalDownBorder { get { return info.HasDiagonalDownBorder; } }
		protected internal bool HasOutlineBorders { get { return info.HasOutlineBorders; } }
		protected internal bool HasInsideBorders { get { return info.HasInsideBorders; } }
		protected internal bool HasDiagonalBorders { get { return info.HasDiagonalBorders; } }
		protected internal bool HasOutlineOrDiagonalBorders { get { return info.HasOutlineOrDiagonalBorders; } }
		protected internal bool HasVerticalRangeBorders { get { return info.HasVerticalRangeBorders; } }
		protected internal bool HasHorizontalRangeBorders { get { return info.HasHorizontalRangeBorders; } }
		protected internal bool HasBorders { get { return info.HasBorders; } }
		protected ColumnCollection Columns { get { return Sheet.Columns; } }
		protected IRowCollection Rows { get { return Sheet.Rows; } }
		protected CellStyleBase NormalStyle { get { return Sheet.Workbook.StyleSheet.CellStyles.Normal; } }
		#endregion
		#region TryGet(*)
		protected ICell TryGetCell(int columnIndex, int rowIndex) {
			return Sheet.TryGetCell(columnIndex, rowIndex);
		}
		protected Column TryGetColumn(int index) {
			return Columns.TryGetColumn(index);
		}
		protected Row TryGetRow(int index) {
			return Rows.TryGetRow(index);
		}
		#endregion
		protected void PrepareDefaultBorderInfo(BorderInfo info, IActualBorderInfo actualInfo) {
			info.LeftLineStyle = BorderSideAccessor.Left.GetLineStyle(actualInfo);
			info.LeftColorIndex = BorderSideAccessor.Left.GetLineColorIndex(actualInfo);
			info.RightLineStyle = BorderSideAccessor.Right.GetLineStyle(actualInfo);
			info.RightColorIndex = BorderSideAccessor.Right.GetLineColorIndex(actualInfo);
			info.TopLineStyle = BorderSideAccessor.Top.GetLineStyle(actualInfo);
			info.TopColorIndex = BorderSideAccessor.Top.GetLineColorIndex(actualInfo);
			info.BottomLineStyle = BorderSideAccessor.Bottom.GetLineStyle(actualInfo);
			info.BottomColorIndex = BorderSideAccessor.Bottom.GetLineColorIndex(actualInfo);
			SetDiagonalBorders(info, actualInfo.DiagonalUpLineStyle, actualInfo.DiagonalDownLineStyle, actualInfo.DiagonalColorIndex);
		}
		protected void PrepareDefaultCellBorderInfo(BorderInfo info, int columnIndex, int rowIndex) {
			IActualBorderInfo actualBorderInfo = RangeBordersFormattingHelper.GetFakeCellActualBorderInfo(Sheet, columnIndex, rowIndex);
			PrepareDefaultBorderInfo(info, actualBorderInfo);
		}
		protected void SetDiagonalBorders(BorderInfo info, XlBorderLineStyle upStyle, XlBorderLineStyle downStyle, int colorIndex) {
			info.DiagonalUp = HasStyle(upStyle);
			info.DiagonalDown = HasStyle(downStyle);
			if (info.DiagonalUp)
				info.DiagonalLineStyle = upStyle;
			if (info.DiagonalDown)
				info.DiagonalLineStyle = downStyle;
			info.DiagonalColorIndex = GetColorIndex(info.DiagonalLineStyle, colorIndex);
		}
		#region SetBorders
		protected internal void SetCellBorders(CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			if (!modifiers.ContainsCell(columnIndex, rowIndex, index)) {
				ICell cell = TryGetCell(columnIndex, rowIndex);
				BorderInfo info = new BorderInfo();
				if (cell != null)
					PrepareDefaultBorderInfo(info, cell.ActualBorder);
				else {
					PrepareDefaultCellBorderInfo(info, columnIndex, rowIndex);
					cell = GetCell(columnIndex, rowIndex);
				}
				PrepareCellBorderInfo(info, range, columnIndex, rowIndex, modifiers, index, true, true);
				SetBordersCore(cell, info);
			}
		}
		protected void PrepareCellBorderInfo(BorderInfo info, CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index, bool shouldClearNoneNearestRightBorder, bool shouldClearNoneNearestBottomBorder) {
			SetCellLeftBorder(info, range, columnIndex, rowIndex, modifiers, index, shouldClearNoneNearestRightBorder);
			SetCellRightBorder(info, range, columnIndex, rowIndex, modifiers, index);
			SetCellTopBorder(info, range, columnIndex, rowIndex, modifiers, index, shouldClearNoneNearestBottomBorder);
			SetCellBottomBorder(info, range, columnIndex, rowIndex, modifiers, index);
			SetCellDiagonalBorders(info);
		}
		void SetCellLeftBorder(BorderInfo info, CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index, bool shouldClearNoneNearestRightBorder) {
			if (columnIndex == range.LeftColumnIndex) {
				SetBorder(info, BorderInfo.LeftBorderAccessor, HasLeftBorder, LeftLineStyle, LeftColorIndex);
				bool hasBorder = columnIndex > 0 && HasLeftBorder && (shouldClearNoneNearestRightBorder ? HasStyle(LeftLineStyle.Value) : true);
				ClearCellBorder(columnIndex - 1, rowIndex, hasBorder, BorderInfo.RightBorderAccessor, PerformRightClear, modifiers, index);
			} else
				SetBorder(info, BorderInfo.LeftBorderAccessor, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
		}
		void SetCellRightBorder(BorderInfo info, CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			if (columnIndex == range.RightColumnIndex) {
				SetBorder(info, BorderInfo.RightBorderAccessor, HasRightBorder, RightLineStyle, RightColorIndex);
				bool hasBorder = columnIndex < IndicesChecker.MaxColumnCount - 1 && HasRightBorder && HasStyle(RightLineStyle.Value);
				ClearCellBorder(columnIndex + 1, rowIndex, hasBorder, BorderInfo.LeftBorderAccessor, PerformLeftClear, modifiers, index);
			} else
				SetBorder(info, BorderInfo.RightBorderAccessor, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
		}
		void SetCellTopBorder(BorderInfo info, CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index, bool shouldClearNoneNearestBottomBorder) {
			if (rowIndex == range.TopRowIndex) {
				SetBorder(info, BorderInfo.TopBorderAccessor, HasTopBorder, TopLineStyle, TopColorIndex);
				bool hasBorder = rowIndex > 0 && HasTopBorder && (shouldClearNoneNearestBottomBorder ? HasStyle(TopLineStyle.Value) : true);
				ClearCellBorder(columnIndex, rowIndex - 1, hasBorder, BorderInfo.BottomBorderAccessor, PerformBottomClear, modifiers, index);
			} else
				SetBorder(info, BorderInfo.TopBorderAccessor, HasHorizontalBorder, HorizontalLineStyle, HorizontalColorIndex);
		}
		void SetCellBottomBorder(BorderInfo info, CellRange range, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			if (rowIndex == range.BottomRowIndex) {
				SetBorder(info, BorderInfo.BottomBorderAccessor, HasBottomBorder, BottomLineStyle, BottomColorIndex);
				bool hasBorder = rowIndex < IndicesChecker.MaxRowCount - 1 && HasBottomBorder && HasStyle(BottomLineStyle.Value);
				ClearCellBorder(columnIndex, rowIndex + 1, hasBorder, BorderInfo.TopBorderAccessor, PerformTopClear, modifiers, index);
			} else
				SetBorder(info, BorderInfo.BottomBorderAccessor, HasHorizontalBorder, HorizontalLineStyle, HorizontalColorIndex);
		}
		protected void SetBordersCore(ICell icell, BorderInfo info) {
			icell.BeginUpdate();
			try {
				Cell cell = icell as Cell;
				CellFormat format = cell.GetInfoForModification() as CellFormat;
				format.ApplyBorderFormat(info);
				cell.ReplaceInfo(format, DocumentModelChangeActions.None); 
			} finally {
				icell.EndUpdate();
			}
		}
		protected void SetCellDiagonalBorders(BorderInfo info) {
			if (HasDiagonalUpBorder)
				SetDiagonalUpBorders(info, DiagonalUpLineStyle.Value, DiagonalColorIndex.Value);
			if (HasDiagonalDownBorder)
				SetDiagonalDownBorders(info, DiagonalDownLineStyle.Value, DiagonalColorIndex.Value);
		}
		void SetDiagonalUpBorders(BorderInfo info, XlBorderLineStyle style, int colorIndex) {
			info.DiagonalUp = HasStyle(style);
			if (style != XlBorderLineStyle.None)
				info.DiagonalLineStyle = style;
			info.DiagonalColorIndex = GetColorIndex(style, colorIndex);
		}
		void SetDiagonalDownBorders(BorderInfo info, XlBorderLineStyle style, int colorIndex) {
			info.DiagonalDown = HasStyle(style);
			if (style != XlBorderLineStyle.None)
				info.DiagonalLineStyle = style;
			info.DiagonalColorIndex = GetColorIndex(style, colorIndex);
		}
		protected void SetBorder(BorderInfo info, BorderInfoBorderAccessor accessor, bool hasBorder, XlBorderLineStyle? style, int? colorIndex) {
			if (hasBorder)
				SetBorderCore(info, accessor, style.Value, colorIndex.Value);
		}
		protected void SetBorderCore(BorderInfo info, BorderInfoBorderAccessor accessor, XlBorderLineStyle style, int colorIndex) {
			accessor.SetLineStyle(info, style);
			accessor.SetColorIndex(info, GetColorIndex(style, colorIndex));
		}
		int GetColorIndex(XlBorderLineStyle style, int colorIndex) {
			return HasStyle(style) ? colorIndex : ColorModelInfoCache.DefaultItemIndex;
		}
		protected bool HasStyle(XlBorderLineStyle style) {
			return style != XlBorderLineStyle.None;
		}
		protected void ClearCellBorder(int columnIndex, int rowIndex, bool hasBorder, BorderInfoBorderAccessor accessor, PerformClear performClear, RangeBordersModifierCollection modifiers, int index) {
			if (!hasBorder || modifiers.ContainsCell(columnIndex, rowIndex, index))
				return;
			ICell cell = TryGetCell(columnIndex, rowIndex);
			if (RangeBordersFormattingHelper.HasBorderFormatting(cell) && performClear(cell)) 
				ClearBorder(cell, accessor);
		}
		protected void ClearBorder(ICell cell, BorderInfoBorderAccessor accessor) {
			BorderInfo info = new BorderInfo();
			PrepareDefaultBorderInfo(info, cell.ActualBorder);
			ResetBorder(info, accessor);
			SetBordersCore(cell, info);
		}
		protected void ResetBorder(BorderInfo info, BorderInfoBorderAccessor accessor) {
			accessor.SetLineStyle(info, XlBorderLineStyle.None);
			accessor.SetColorIndex(info, ColorModelInfoCache.DefaultItemIndex);
		}
		protected delegate bool PerformClear(IActualBorderInfo info);
		protected bool PerformLeftClear(IActualBorderInfo info) {
			return info.LeftLineStyle != RightLineStyle.Value || info.LeftColorIndex != RightColorIndex.Value;
		}
		protected bool PerformRightClear(IActualBorderInfo info) {
			return info.RightLineStyle != LeftLineStyle.Value || info.RightColorIndex != LeftColorIndex.Value;
		}
		protected bool PerformTopClear(IActualBorderInfo info) {
			return info.TopLineStyle != BottomLineStyle.Value || info.TopColorIndex != BottomColorIndex.Value;
		}
		protected bool PerformBottomClear(IActualBorderInfo info) {
			return info.BottomLineStyle != TopLineStyle.Value || info.BottomColorIndex != TopColorIndex.Value;
		}
		#endregion
		#region ModifyCells
		protected void ModifyAllCells(RangeBordersModifierCollection modifiers, int index) {
			if (!GetHasBorder(range))
				return;
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			for (int columnIndex = leftColumnIndex; columnIndex <= rightColumnIndex; columnIndex++)
				for (int rowIndex = topRowIndex; rowIndex <= bottomRowIndex; rowIndex++)
					SetCellBorders(range, columnIndex, rowIndex, modifiers, index);
		}
		protected internal bool GetHasBorder(CellRange range) {
			bool entireVerticalVectorSelection = range.LeftColumnIndex == range.RightColumnIndex;
			bool entireHorizontalVectorSelection = range.TopRowIndex == range.BottomRowIndex;
			if (entireVerticalVectorSelection && entireHorizontalVectorSelection)
				return HasOutlineOrDiagonalBorders;
			if (entireVerticalVectorSelection)
				return HasVerticalRangeBorders;
			if (entireHorizontalVectorSelection)
				return HasHorizontalRangeBorders;
			return true;
		}
		#endregion
		protected ICell GetCell(int columnIndex, int rowIndex) {
			return Sheet.GetCellOrCreate(columnIndex, rowIndex);
		}
		protected internal virtual void Apply(RangeBordersModifierCollection modifiers, int index) {
			ModifyAllCells(modifiers, index);
		}
	}
	#endregion 
	#region RowColumnBordersModifierBase (abstract class)
	public abstract class RowColumnBordersModifierBase : RangeBordersModifier {
		protected RowColumnBordersModifierBase(CellRange range, MergedBorderInfo info)
			: base(range, info) {
		}
		protected internal override void Apply(RangeBordersModifierCollection modifiers, int index) {
			ApplyCore(modifiers, index);
			ModifyCells(modifiers, index);
		}
		protected internal virtual void ModifyCells(RangeBordersModifierCollection modifiers, int index) {
			ModifyExistingCells(modifiers, index);
			ModifyInterceptCells(modifiers, index);
			ModifyOutlineCells(modifiers, index);
		}
		void ModifyInterceptCells(RangeBordersModifierCollection modifiers, int index) {
			using (InterceptCellsEnumeratorBase interceptCells = CreateInterceptCellsEnumerator(modifiers, index)) {
				while (interceptCells.MoveNext()) {
					InterceptCellsInfo interceptInfo = interceptCells.Current;
					CellRange range = interceptInfo.Range;
					int modifierIndex = interceptInfo.ModifierIndex;
					bool hasIntervalRange = interceptInfo.HasIntervalRange;
					RowColumnBordersModifierBase modifier = modifiers[modifierIndex] as RowColumnBordersModifierBase;
					bool hasRowColumnIntercept = modifier.GetHasRowColumnIntercept(interceptInfo);
					if (hasRowColumnIntercept) {
						if (hasIntervalRange)
							modifier.ModifyInterceptedRowColumn(modifiers, modifierIndex, interceptInfo);
						else {
							bool shouldUseCellFormatting = !Range.ContainsRange(range);
							modifier.ModifyInterceptedCells(modifiers, modifierIndex, interceptInfo, shouldUseCellFormatting);
						}
					} else if (!hasIntervalRange)
						modifier.ModifyRowColumnCells(modifiers, modifierIndex, interceptInfo.Range);
				}
			}
		}
		void ModifyRowColumnCells(RangeBordersModifierCollection modifiers, int index, CellRange range) {
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			for (int columnIndex = leftColumnIndex; columnIndex <= rightColumnIndex; columnIndex++)
				for (int rowIndex = topRowIndex; rowIndex <= bottomRowIndex; rowIndex++) {
					ICell cell = Sheet.TryGetCell(columnIndex, rowIndex);
					if (cell != null) {
						BorderInfo borderInfo = new BorderInfo();
						PrepareDefaultBorderInfo(borderInfo, cell.ActualBorder);
						PrepareCellBorderInfo(borderInfo, Range, columnIndex, rowIndex, modifiers, index, false, false);
						SetBordersCore(cell, borderInfo);
					} else
						CreateRowColumnCellWithFormatting(columnIndex, rowIndex, modifiers, index);
				}
		}
		protected void ClearExistingCellsBorder(CellRange range, BorderInfoBorderAccessor accessor, PerformClear performClear, RangeBordersModifierCollection modifiers, int index) {
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumerator(false);
			while (existingCells.MoveNext()) {
				ICell cell = existingCells.Current as ICell;
				if (RangeBordersFormattingHelper.HasBorderFormatting(cell) && performClear(cell) && !modifiers.ContainsCell(cell, index)) 
					ClearBorder(cell, accessor);
			}
		}
		protected void ModifyExistingCells(RangeBordersModifierCollection modifiers, int index) {
			IEnumerator<ICellBase> existingCells = Range.GetExistingCellsEnumerator(false);
			while (existingCells.MoveNext()) {
				ICell cell = existingCells.Current as ICell;
				if (cell != null && !modifiers.ContainsCell(cell, index)) {
					BorderInfo info = new BorderInfo();
					PrepareDefaultBorderInfo(info, cell.ActualBorder);
					PrepareCellBorderInfo(info, Range, cell.ColumnIndex, cell.RowIndex, modifiers, index, true, true);
					SetBordersCore(cell, info);
				}
			}
		}
		protected abstract void ApplyCore(RangeBordersModifierCollection modifiers, int index);
		protected abstract void ModifyOutlineCells(RangeBordersModifierCollection modifiers, int index);
		#region Intercept
		protected abstract InterceptCellsEnumeratorBase CreateInterceptCellsEnumerator(RangeBordersModifierCollection modifiers, int index);
		protected abstract bool GetHasRowColumnIntercept(InterceptCellsInfo info);
		protected abstract void ModifyInterceptedRowColumn(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo);
		protected abstract void ModifyInterceptedCells(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo, bool shouldUseCellFormatting);
		protected abstract void CreateRowColumnCellWithFormatting(int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index);
		#endregion
	}
	#endregion 
	#region ColumnsBordersModifier
	public class ColumnsBordersModifier : RowColumnBordersModifierBase {
		public ColumnsBordersModifier(CellRange range, MergedBorderInfo info)
			: base(range, info) {
		}
		#region Properies
		protected int FirstIndex { get { return Range.LeftColumnIndex; } }
		protected int LastIndex { get { return Range.RightColumnIndex; } }
		#endregion
		protected override void ApplyCore(RangeBordersModifierCollection modifiers, int index) {
			if (FirstIndex == LastIndex) {
				if (HasVerticalRangeBorders) {
					Column column = GetIsolatedColumn(FirstIndex);
					SetBorders(column, HasLeftBorder, LeftLineStyle, LeftColorIndex, HasRightBorder, RightLineStyle, RightColorIndex);
					ClearLeftColumnBorder(modifiers, index);
				}
				return;
			}
			IList<Column> columnRangesEnsureExist = Columns.GetColumnRangesEnsureExist(FirstIndex, LastIndex);
			Column first = columnRangesEnsureExist[0];
			if (PerformCreateIsolatedColumn(first, BorderSideAccessor.Left, HasLeftBorder, LeftLineStyle, LeftColorIndex))
				first = GetIsolatedColumn(FirstIndex);
			SetBorders(first, HasLeftBorder, LeftLineStyle, LeftColorIndex, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
			ClearLeftColumnBorder(modifiers, index);
			Column last = columnRangesEnsureExist[columnRangesEnsureExist.Count - 1];
			if (PerformCreateIsolatedColumn(last, BorderSideAccessor.Right, HasRightBorder, RightLineStyle, RightColorIndex))
				last = GetIsolatedColumn(LastIndex);
			SetBorders(last, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex, HasRightBorder, RightLineStyle, RightColorIndex);
			ModifyInsideColumns();
		}
		Column GetIsolatedColumn(int index) {
			return Columns.GetIsolatedColumn(index);
		}
		void ClearLeftColumnBorder(RangeBordersModifierCollection modifiers, int index) {
			int columnIndex =  FirstIndex - 1;
			if (columnIndex >= 0 && HasLeftBorder) {
				Column column = TryGetColumn(columnIndex);
				if (column != null) {
					if (column.StartIndex != columnIndex)
						column = GetIsolatedColumn(columnIndex);
					if (RangeBordersFormattingHelper.HasBorderFormatting(column) && PerformRightClear(column))
						ClearBorder(column, BorderInfo.RightBorderAccessor);
				}
				CellRange range = new CellRange(Sheet, new CellPosition(columnIndex, 0), new CellPosition(columnIndex, IndicesChecker.MaxRowIndex));
				ClearExistingCellsBorder(range, BorderInfo.RightBorderAccessor, PerformRightClear, modifiers, index);
			}
		}
		void ClearBorder(Column column, BorderInfoBorderAccessor accessor) {
			BorderInfo info = new BorderInfo();
			PrepareDefaultBorderInfo(info, column.ActualBorder);
			ResetBorder(info, accessor);
			SetBordersCore(column, info);
		}
		void ModifyInsideColumns() {
			int startPosition = Columns.TryGetColumnIndex(FirstIndex);
			int endPosition = Columns.TryGetColumnIndex(LastIndex);
			if (startPosition >= 0 || endPosition >= 0 || startPosition <= endPosition)
				ModifyInsideColumnsCore(Columns.InnerList, startPosition + 1, endPosition - 1);
		}
		protected void ModifyInsideColumnsCore(List<Column> innerColumns, int startPosition, int endPosition) {
			for (int i = startPosition; i <= endPosition; i++)
				SetBorders(innerColumns[i], HasVerticalBorder, VerticalLineStyle, VerticalColorIndex, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
		}
		protected override void ModifyOutlineCells(RangeBordersModifierCollection modifiers, int index) {
			if (!Sheet.Rows.Contains(0))
				return;
			if (HasTopBorder && !HasHorizontalBorder)
				for (int i = FirstIndex; i <= LastIndex; i++)
					if (!modifiers.ContainsCell(i, 0, index)) {
						ICell cell = GetCell(i, 0);
						BorderInfo info = new BorderInfo();
						PrepareDefaultBorderInfo(info, cell.ActualBorder);
						SetBorderCore(info, BorderInfo.TopBorderAccessor, TopLineStyle.Value, TopColorIndex.Value);
						SetBordersCore(cell, info);
					}
		}
		protected bool PerformCreateIsolatedColumn(Column column, BorderSideAccessor acessor, bool hasOutlineBorder, XlBorderLineStyle? outlineStyle, int? outlineColorIndex) {
			if (column.StartIndex == column.EndIndex)
				return false;
			bool applyOutline = PerformChangeBorder(column, acessor, hasOutlineBorder, outlineStyle, outlineColorIndex);
			bool applyInside = PerformChangeBorder(column, acessor, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
			return applyOutline != applyInside;
		}
		bool PerformChangeBorder(IActualBorderInfo info, BorderSideAccessor accessor, bool hasOutlineBorder, XlBorderLineStyle? style, int? colorIndex) {
			return hasOutlineBorder && (accessor.GetLineStyle(info) != style.Value || accessor.GetLineColorIndex(info) != colorIndex.Value);
		}
		protected void SetBorders(Column column, bool hasLeftBorder, XlBorderLineStyle? leftLineStyle, int? leftColorIndex, bool hasRightBorder, XlBorderLineStyle? rightLineStyle, int? rightColorIndex) {
			BorderInfo info = new BorderInfo();
			PrepareDefaultBorderInfo(info, column.ActualBorder);
			SetBorder(info, BorderInfo.LeftBorderAccessor, hasLeftBorder, leftLineStyle, leftColorIndex);
			SetBorder(info, BorderInfo.RightBorderAccessor, hasRightBorder, rightLineStyle, rightColorIndex);
			SetColumnHorizontalBorders(info);
			SetCellDiagonalBorders(info);
			SetBordersCore(column, info);
		}
		void SetColumnHorizontalBorders(BorderInfo info) {
			if (HasHorizontalBorder) {
				SetBorderCore(info, BorderInfo.TopBorderAccessor, HorizontalLineStyle.Value, HorizontalColorIndex.Value);
				SetBorderCore(info, BorderInfo.BottomBorderAccessor, HorizontalLineStyle.Value, HorizontalColorIndex.Value);
			}
		}
		void SetBordersCore(Column column, BorderInfo info) {
			column.BeginUpdate();
			try {
				CellFormat format = column.GetInfoForModification(Column.CellFormatIndexAccessor) as CellFormat;
				format.ApplyBorderFormat(info);
				column.ReplaceInfo(Column.CellFormatIndexAccessor, format, DocumentModelChangeActions.None); 
			} finally {
				column.EndUpdate();
			}
		}
		#region Intercept
		protected override InterceptCellsEnumeratorBase CreateInterceptCellsEnumerator(RangeBordersModifierCollection modifiers, int index) {
			return new ColumnInterceptCellsEnumerator(modifiers, index);
		}
		protected override bool GetHasRowColumnIntercept(InterceptCellsInfo info) {
			return info.Row != null;
		}
		protected override void ModifyInterceptedRowColumn(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo) {
			CellRange range = interceptInfo.Range;
			IEnumerator<Column> columns = Columns.GetExistingColumnsEnumerator(range.LeftColumnIndex, range.RightColumnIndex, false);
			while (columns.MoveNext()) {
				BorderInfo borderInfo = new BorderInfo();
				Column column = columns.Current;
				Row row = interceptInfo.Row;
				PrepareDefaultInterceptCellsBorderInfo(borderInfo, column.Index, row, false);
				PrepareInterceptColumnBorderInfo(borderInfo, column.Index, row.Index, modifiers, index);
				SetBordersCore(column, borderInfo);
			}
		}
		protected override void ModifyInterceptedCells(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo, bool shouldUseCellFormatting) {
			CellRange range = interceptInfo.Range;
			int firstIndex = range.LeftColumnIndex;
			int lastIndex = range.RightColumnIndex;
			for (int columnIndex = firstIndex; columnIndex <= lastIndex; columnIndex++)
				SetInterceptCellsBorders(interceptInfo.Row, columnIndex, modifiers, index, shouldUseCellFormatting);
		}
		protected override void CreateRowColumnCellWithFormatting(int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			IActualBorderInfo defaultInfo = RangeBordersFormattingHelper.GetColumnActualBorderInfo(Sheet, columnIndex);
			BorderInfo borderInfo = new BorderInfo();
			PrepareDefaultBorderInfo(borderInfo, defaultInfo);
			PrepareCellBorderInfo(borderInfo, Range, columnIndex, rowIndex, modifiers, index, false, false);
				ICell cell = GetCell(columnIndex, rowIndex);
				SetBordersCore(cell, borderInfo);
		}
		void SetInterceptCellsBorders(Row row, int columnIndex, RangeBordersModifierCollection modifiers, int index, bool shouldUseCellFormatting) {
			int rowIndex = row.Index;
			ICell cell = TryGetCell(columnIndex, rowIndex);
			BorderInfo borderInfo = new BorderInfo();
			if (cell != null) {
				PrepareDefaultBorderInfo(borderInfo, cell.ActualBorder);
				PrepareInterceptColumnBorderInfo(borderInfo, columnIndex, rowIndex, modifiers, index);
				SetBordersCore(cell, borderInfo);
			} else {
				PrepareDefaultInterceptCellsBorderInfo(borderInfo, columnIndex, row, shouldUseCellFormatting);
				BorderInfo cloneDefaultInfo = borderInfo.Clone();
				PrepareInterceptColumnBorderInfo(borderInfo, columnIndex, rowIndex, modifiers, index);
				if (!cloneDefaultInfo.Equals(borderInfo)) {
					cell = GetCell(columnIndex, rowIndex);
					SetBordersCore(cell, borderInfo);
				}
			}
		}
		void PrepareInterceptColumnBorderInfo(BorderInfo info, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			SetColumnLeftBorder(info, columnIndex, rowIndex, modifiers, index);
			SetColumnRightBorder(info, columnIndex, modifiers, index);
			SetColumnHorizontalBorders(info);
			SetCellDiagonalBorders(info);
		}
		void SetColumnLeftBorder(BorderInfo info, int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			if (columnIndex == FirstIndex) {
				SetBorder(info, BorderInfo.LeftBorderAccessor, HasLeftBorder, LeftLineStyle, LeftColorIndex);
				ClearColumnCellRightBorder(columnIndex - 1, rowIndex, modifiers, index);
			} else
				SetBorder(info, BorderInfo.LeftBorderAccessor, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
		}
		void ClearColumnCellRightBorder(int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			if (columnIndex < 0 || !HasLeftBorder || modifiers.ContainsCell(columnIndex, rowIndex, index))
				return;
			ICell cell = TryGetCell(columnIndex, rowIndex);
			if (RangeBordersFormattingHelper.HasBorderFormatting(cell) && PerformRightClear(cell))
				ClearBorder(cell, BorderInfo.RightBorderAccessor);
			else if (cell == null) {
				IActualBorderInfo actualInfo = RangeBordersFormattingHelper.GetFakeCellActualBorderInfo(Sheet, columnIndex, rowIndex);
				if (PerformRightClear(actualInfo)) {
					BorderInfo borderInfo = new BorderInfo();
					PrepareDefaultBorderInfo(borderInfo, actualInfo);
					BorderInfo cloneDefaultInfo = borderInfo.Clone();
					ResetBorder(borderInfo, BorderInfo.RightBorderAccessor);
					if (!cloneDefaultInfo.Equals(borderInfo)) {
						cell = GetCell(columnIndex, rowIndex);
						SetBordersCore(cell, borderInfo);
					}
				}
			}
		}
		void SetColumnRightBorder(BorderInfo info, int columnIndex, RangeBordersModifierCollection modifiers, int index) {
			if (columnIndex == LastIndex) 
				SetBorder(info, BorderInfo.RightBorderAccessor, HasRightBorder, RightLineStyle, RightColorIndex);
			else
				SetBorder(info, BorderInfo.RightBorderAccessor, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
		}
		void PrepareDefaultInterceptCellsBorderInfo(BorderInfo info, int columnIndex, Row row, bool shouldUseCellFormatting) {
			IActualBorderInfo defaultInfo = RangeBordersFormattingHelper.GetFakeCellActualBorderInfo(Sheet, columnIndex, row.Index);
			info.LeftLineStyle = BorderSideAccessor.Left.GetLineStyle(defaultInfo);
			info.LeftColorIndex = BorderSideAccessor.Left.GetLineColorIndex(defaultInfo);
			info.RightLineStyle = BorderSideAccessor.Right.GetLineStyle(defaultInfo);
			info.RightColorIndex = BorderSideAccessor.Right.GetLineColorIndex(defaultInfo);
			if (!shouldUseCellFormatting) {
				info.TopLineStyle = GetLineStyle(row, BorderSideAccessor.Top, defaultInfo);
				info.TopColorIndex = GetColorIndex(row, BorderSideAccessor.Top, defaultInfo);
				info.BottomLineStyle = GetLineStyle(row, BorderSideAccessor.Bottom, defaultInfo);
				info.BottomColorIndex = GetColorIndex(row, BorderSideAccessor.Bottom, defaultInfo);
			} else {
				info.TopLineStyle = BorderSideAccessor.Top.GetLineStyle(defaultInfo);
				info.TopColorIndex = BorderSideAccessor.Top.GetLineColorIndex(defaultInfo);
				info.BottomLineStyle = BorderSideAccessor.Bottom.GetLineStyle(defaultInfo);
				info.BottomColorIndex = BorderSideAccessor.Bottom.GetLineColorIndex(defaultInfo);
			}
			SetDiagonalBorders(info, defaultInfo.DiagonalUpLineStyle, defaultInfo.DiagonalDownLineStyle, defaultInfo.DiagonalColorIndex);
		}
		XlBorderLineStyle GetLineStyle(Row row, BorderSideAccessor accessor, IActualBorderInfo defaultInfo) {
			IActualBorderInfo actualCellBorderInfo = GetActualRowCellBorderInfo(row, defaultInfo);
			return accessor.GetLineStyle(actualCellBorderInfo);
		}
		int GetColorIndex(Row row, BorderSideAccessor accessor, IActualBorderInfo defaultInfo) {
			IActualBorderInfo actualCellBorderInfo = GetActualRowCellBorderInfo(row, defaultInfo);
			return accessor.GetLineColorIndex(actualCellBorderInfo);
		}
		IActualBorderInfo GetActualRowCellBorderInfo(Row row, IActualBorderInfo defaultInfo) {
			return row.ActualApplyInfo.ApplyBorder ? row.ActualBorder : defaultInfo;
		}
		#endregion
	}
	#endregion
	#region SheetBordersModifier
	public class SheetBordersModifier : ColumnsBordersModifier {
		public SheetBordersModifier(CellRange range, MergedBorderInfo info)
			: base(range, info) {
		}
		protected internal override void Apply(RangeBordersModifierCollection modifiers, int index) {
			ApplyCore(modifiers, index);
			ModifyExistingRows(modifiers, index);
			ModifyCells(modifiers, index);
		}
		protected override void ApplyCore(RangeBordersModifierCollection modifiers, int index) {
			int startInsidePosition = 0;
			Column first = TryGetFirstColumn();
			if (first != null) {
				SetBorders(first, HasLeftBorder, LeftLineStyle, LeftColorIndex, HasVerticalBorder, VerticalLineStyle, VerticalColorIndex);
				startInsidePosition++;
			}
			int lastIndex = IndicesChecker.MaxColumnIndex;
			Columns.CreateColumnRangesEnsureExist(0, lastIndex);
			int endInsidePosition = Columns.TryGetColumnIndex(lastIndex);
			ModifyInsideColumnsCore(Columns.InnerList, startInsidePosition, endInsidePosition);
		}
		Column TryGetFirstColumn() {
			Column result = Columns.TryGetColumn(0);
			if (result != null)
				if (PerformCreateIsolatedColumn(result, BorderSideAccessor.Left, HasLeftBorder, LeftLineStyle, LeftColorIndex))
					return Columns.GetIsolatedColumn(0);
				else
					return result;
			IEnumerator<Row> existingRows = GetExistingRows();
			while (existingRows.MoveNext()) {
				Row row = existingRows.Current;
				if (row.FirstColumnIndex == 0)
					return Columns.GetIsolatedColumn(0);
			}
			return null;
		}
		protected internal IEnumerator<Row> GetExistingRows() {
			return Sheet.Rows.GetExistingRowsEnumerator(0, IndicesChecker.MaxRowIndex);
		}
		protected internal override void ModifyCells(RangeBordersModifierCollection modifiers, int index) {
			ModifyExistingCells(modifiers, index);
		}
		void ModifyExistingRows(RangeBordersModifierCollection modifiers, int index) {
			RowsBordersModifier.ModifyExistingRows(this, modifiers, index);
		}
	}
	#endregion
	#region RowsBordersModifier
	public class RowsBordersModifier : RowColumnBordersModifierBase {
		#region Static Members
		internal static void ModifyExistingRows(SheetBordersModifier sheetModifier, RangeBordersModifierCollection modifiers, int index) {
			RowsBordersModifier rowModifier = new RowsBordersModifier(sheetModifier.Range, sheetModifier.Info);
			IEnumerator<Row> existingRows = sheetModifier.GetExistingRows();
			while (existingRows.MoveNext()) {
				Row row = existingRows.Current;
				rowModifier.SetBorders(row, modifiers, index);
			}
		}
		#endregion
		public RowsBordersModifier(CellRange range, MergedBorderInfo info)
			: base(range, info) {
		}
		#region Properties
		int FirstIndex { get { return Range.TopRowIndex; } }
		int LastIndex { get { return Range.BottomRowIndex; } }
		#endregion
		protected override void ApplyCore(RangeBordersModifierCollection modifiers, int index) {
			IRowCollection rows = Sheet.Rows;
			if (FirstIndex == LastIndex) {
				if (HasHorizontalRangeBorders)
					SetBorders(rows[FirstIndex], modifiers, index);
				return;
			}
			for (int i = FirstIndex; i <= LastIndex; i++)
				SetBorders(rows[i], modifiers, index);
		}
		protected override void ModifyOutlineCells(RangeBordersModifierCollection modifiers, int index) {
			if (HasLeftBorder && !HasVerticalBorder)
				for (int i = FirstIndex; i <= LastIndex; i++)
					if (!modifiers.ContainsCell(0, i, index)) {
						ICell cell = GetCell(0, i);
						BorderInfo info = new BorderInfo();
						PrepareDefaultBorderInfo(info, cell);
						SetBorderCore(info, BorderInfo.LeftBorderAccessor, LeftLineStyle.Value, LeftColorIndex.Value);
						SetBordersCore(cell, info);
					}
		}
		void SetBorders(Row row, RangeBordersModifierCollection modifiers, int index) {
			SetBorders(row, FirstIndex, LastIndex, modifiers, index);
		}
		void SetBorders(Row row, int firstIndex, int lastIndex, RangeBordersModifierCollection modifiers, int index) {
			BorderInfo info = new BorderInfo();
			PrepareDefaultBorderInfo(info, row.ActualBorder);
			PrepareRowBorderInfo(info, row.Index, firstIndex, lastIndex, modifiers, index);					
			SetBordersCore(row, info);
		}
		void PrepareRowBorderInfo(BorderInfo info, int rowIndex, int firstIndex, int lastIndex, RangeBordersModifierCollection modifiers, int index) {
			SetRowTopBorder(info, rowIndex, firstIndex, modifiers, index);
			SetRowBottomBorder(info, rowIndex, lastIndex, modifiers, index);
			SetRowVerticalBorders(info);
			SetCellDiagonalBorders(info);
		}
		void SetRowTopBorder(BorderInfo info, int rowIndex, int firstIndex, RangeBordersModifierCollection modifiers, int index) {
			if (rowIndex == firstIndex) {
				SetBorder(info, BorderInfo.TopBorderAccessor, HasTopBorder, TopLineStyle, TopColorIndex);
				if (rowIndex > 0 && HasTopBorder)
					ClearTopRowBorder(modifiers, index);
			} else
				SetBorder(info, BorderInfo.TopBorderAccessor, HasHorizontalBorder, HorizontalLineStyle, HorizontalColorIndex);
		}
		void SetRowBottomBorder(BorderInfo info, int rowIndex, int lastIndex, RangeBordersModifierCollection modifiers, int index) {
			if (rowIndex == lastIndex)
				SetBorder(info, BorderInfo.BottomBorderAccessor, HasBottomBorder, BottomLineStyle, BottomColorIndex);
			else
				SetBorder(info, BorderInfo.BottomBorderAccessor, HasHorizontalBorder, HorizontalLineStyle, HorizontalColorIndex);
		}
		void SetRowVerticalBorders(BorderInfo info) {
			if (HasVerticalBorder) {
				SetBorderCore(info, BorderInfo.LeftBorderAccessor, VerticalLineStyle.Value, VerticalColorIndex.Value);
				SetBorderCore(info, BorderInfo.RightBorderAccessor, VerticalLineStyle.Value, VerticalColorIndex.Value);
			}
		}
		void ClearTopRowBorder(RangeBordersModifierCollection modifiers, int index) {
			int rowIndex = FirstIndex - 1;
			Row row = TryGetRow(rowIndex);
			if (RangeBordersFormattingHelper.HasBorderFormatting(row) && PerformBottomClear(row))
				ClearBorder(row, BorderInfo.BottomBorderAccessor);
			CellRange range = new CellRange(Sheet, new CellPosition(0, rowIndex), new CellPosition(IndicesChecker.MaxColumnIndex, rowIndex));
			ClearExistingCellsBorder(range, BorderInfo.BottomBorderAccessor, PerformBottomClear, modifiers, index);
		}
		void ClearBorder(Row row, BorderInfoBorderAccessor accessor) {
			BorderInfo info = new BorderInfo();
			PrepareDefaultBorderInfo(info, row.ActualBorder);
			ResetBorder(info, accessor);
			SetBordersCore(row, info);
		}
		void SetBordersCore(Row row, BorderInfo info) {
			row.BeginUpdate();
			try {
				CellFormat format = row.GetInfoForModification(Row.CellFormatIndexAccessor) as CellFormat;
				format.ApplyBorderFormat(info);
				row.ReplaceInfo(Row.CellFormatIndexAccessor, format, DocumentModelChangeActions.None); 
			} finally {
				row.EndUpdate();
			}
		}
		#region Intercept
		protected override InterceptCellsEnumeratorBase CreateInterceptCellsEnumerator(RangeBordersModifierCollection modifiers, int index) {
			return new RowInterceptCellsEnumerator(modifiers, index);
		}
		protected override bool GetHasRowColumnIntercept(InterceptCellsInfo info) {
			return info.Column != null;
		}
		protected override void ModifyInterceptedRowColumn(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo) {
			CellRange range = interceptInfo.Range;
			IEnumerator<Row> rows = Rows.GetExistingRowsEnumerator(range.TopRowIndex, range.BottomRowIndex);
			while (rows.MoveNext()) {
				BorderInfo borderInfo = new BorderInfo();
				Row row = rows.Current;
				int rowIndex = row.Index;
				PrepareDefaultInterceptCellsBorderInfo(borderInfo, interceptInfo.Column, false);
				PrepareRowBorderInfo(borderInfo, rowIndex, FirstIndex, LastIndex, modifiers, index);
				SetBordersCore(row, borderInfo);
			}
		}
		protected override void ModifyInterceptedCells(RangeBordersModifierCollection modifiers, int index, InterceptCellsInfo interceptInfo, bool shouldUseCellFormatting) {
			CellRange range = interceptInfo.Range;
			int firstIndex = range.TopRowIndex;
			int lastIndex = range.BottomRowIndex;
			for (int rowIndex = firstIndex; rowIndex <= lastIndex; rowIndex++)
				SetInterceptCellsBorders(interceptInfo.Column, rowIndex, modifiers, index, shouldUseCellFormatting);
		}
		protected override void CreateRowColumnCellWithFormatting(int columnIndex, int rowIndex, RangeBordersModifierCollection modifiers, int index) {
			IActualBorderInfo defaultInfo = RangeBordersFormattingHelper.GetRowActualBorderInfo(Sheet, rowIndex);
			BorderInfo borderInfo = new BorderInfo();
			PrepareDefaultBorderInfo(borderInfo, defaultInfo);
			BorderInfo cloneDefaultInfo = borderInfo.Clone();
			PrepareCellBorderInfo(borderInfo, Range, columnIndex, rowIndex, modifiers, index, false, false);
			if (!cloneDefaultInfo.Equals(borderInfo)) {
				ICell cell = GetCell(columnIndex, rowIndex);
				SetBordersCore(cell, borderInfo);
			}
		}
		void SetInterceptCellsBorders(Column column, int rowIndex, RangeBordersModifierCollection modifiers, int index, bool shouldUseCellFormatting) {
			int startColumnIndex = column.StartIndex;
			int endColumnIndex = column.EndIndex;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++) {
				ICell cell = Sheet.TryGetCell(columnIndex, rowIndex);
				BorderInfo borderInfo = new BorderInfo();
				if (cell != null) {
					PrepareDefaultBorderInfo(borderInfo, cell.ActualBorder);
					PrepareRowBorderInfo(borderInfo, rowIndex, FirstIndex, LastIndex, modifiers, index);
					SetBordersCore(cell, borderInfo);
				} else {
					PrepareDefaultInterceptCellsBorderInfo(borderInfo, column, shouldUseCellFormatting);
					PrepareRowBorderInfo(borderInfo, rowIndex, FirstIndex, LastIndex, modifiers, index);
					cell = GetCell(columnIndex, rowIndex);
					if (column.FormatIndex != column.DocumentModel.StyleSheet.DefaultCellFormatIndex) 
						cell.ApplyFormat(column.FormatInfo);
					SetBordersCore(cell, borderInfo);
				}
			}
		}
		void PrepareDefaultInterceptCellsBorderInfo(BorderInfo info, Column column, bool shouldUseCellFormatting) {
			IActualBorderInfo defaultInfo = GetActualColumnBorderInfo(column);
			if (!shouldUseCellFormatting) {
				info.LeftLineStyle = GetLineStyle(column, BorderSideAccessor.Left, defaultInfo);
				info.LeftColorIndex = GetColorIndex(column, BorderSideAccessor.Left, defaultInfo);
				info.RightLineStyle = GetLineStyle(column, BorderSideAccessor.Right, defaultInfo);
				info.RightColorIndex = GetColorIndex(column, BorderSideAccessor.Right, defaultInfo);
			} else {
				info.LeftLineStyle = BorderSideAccessor.Left.GetLineStyle(defaultInfo);
				info.LeftColorIndex = BorderSideAccessor.Left.GetLineColorIndex(defaultInfo);
				info.RightLineStyle = BorderSideAccessor.Right.GetLineStyle(defaultInfo);
				info.RightColorIndex = BorderSideAccessor.Right.GetLineColorIndex(defaultInfo);
			}
			info.TopLineStyle = BorderSideAccessor.Top.GetLineStyle(defaultInfo);
			info.TopColorIndex = BorderSideAccessor.Top.GetLineColorIndex(defaultInfo);
			info.BottomLineStyle = BorderSideAccessor.Bottom.GetLineStyle(defaultInfo);
			info.BottomColorIndex = BorderSideAccessor.Bottom.GetLineColorIndex(defaultInfo);
			SetDiagonalBorders(info, defaultInfo.DiagonalUpLineStyle, defaultInfo.DiagonalDownLineStyle, defaultInfo.DiagonalColorIndex);
		}
		IActualBorderInfo GetActualColumnBorderInfo(Column column) {
			if (RangeBordersFormattingHelper.HasBorderFormatting(column))
				return column;
			return NormalStyle.ActualBorder;
		}
		XlBorderLineStyle GetLineStyle(Column column, BorderSideAccessor accessor, IActualBorderInfo defaultInfo) {
			IActualBorderInfo actualCellBorderInfo = GetActualColumnCellBorderInfo(column, defaultInfo);
			return accessor.GetLineStyle(actualCellBorderInfo);
		}
		int GetColorIndex(Column column, BorderSideAccessor accessor, IActualBorderInfo defaultInfo) {
			IActualBorderInfo actualCellBorderInfo = GetActualColumnCellBorderInfo(column, defaultInfo);
			return accessor.GetLineColorIndex(actualCellBorderInfo);
		}
		IActualBorderInfo GetActualColumnCellBorderInfo(Column column, IActualBorderInfo defaultInfo) {
			return column.ActualApplyInfo.ApplyBorder ? column.ActualBorder : defaultInfo;
		}
		#endregion
		#region HasThickBorder //TODO
		#endregion
	}
	#endregion 
}
