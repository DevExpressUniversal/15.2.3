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

#if UNCOMMENT
using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using UnionRange = DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeUnionRange;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	internal class NativeUnionRange  : Range {
		readonly List<NativeRange> innerList;
		readonly NativeWorksheet worksheet;
		internal NativeUnionRange(NativeWorksheet worksheet, string[] references) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(references, "references");
			this.worksheet = worksheet;
			if(references.Length == 0)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorZeroCountRanges));
			this.innerList = new List<NativeRange>();
			for (int i = 0; i < references.Length; i++)
				innerList.Add((NativeRange)worksheet.CreateRange(references[i]));
		}
		internal NativeUnionRange(NativeWorksheet worksheet, List<Model.CellRangeBase> ranges) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(ranges, "ranges");
			this.worksheet = worksheet;
			this.innerList = new List<NativeRange>();
			foreach(Model.CellRangeBase range in ranges) {
				Model.CellRange modelCellRange = range as Model.CellRange;
				if(modelCellRange != null) {
					innerList.Add(new NativeRange(modelCellRange, worksheet));
				}
			}
		}
		#region Properties
		internal NativeRange FirstRange { get { return innerList[0]; } }
		public int Count { get { return innerList.Count; } }
		public int FirstColumnIndex { get { return FirstRange.FirstColumnIndex; } }
		public int FirstRowIndex { get { return FirstRange.FirstRowIndex; } }
		public int LastColumnIndex { get { return FirstRange.LastColumnIndex; } }
		public int LastRowIndex { get { return FirstRange.LastRowIndex; } }
		public int ColumnCount { get { return FirstRange.ColumnCount; } }
		public int RowCount { get { return FirstRange.RowCount; } }
		public string DefinedName {
			get {
				return worksheet.GetDefinedNameFromRange(this);
			}
			set {
				string refersTo = GetReferenceA1(true, true, true, null); 
				worksheet.ModelWorksheet.CreateDefinedName(value, refersTo);
			}
		}
		public Worksheet Worksheet { get { return worksheet; } }
		public Style Formatting { get { return ((RangeBase)FirstRange).Formatting; } }
		public bool IsMerged { get { return FirstRange.IsMerged; } }
		public Cell this[int rowIndex, int columnIndex] { get { return FirstRange[rowIndex, columnIndex]; } }
		public Cell this[int position] { get { return ((Range)FirstRange)[position]; } }
		public bool HasArrayFormula {
			get {
				foreach(Range range in innerList)
					if(range.HasArrayFormula)
						return true;
				return false;
			}
		}
		#region ColumnWidthInCharacters
		public double? ColumnWidthInCharacters {
			get { return FirstRange.ColumnWidthInCharacters; }
			set {
				foreach (NativeRange range in innerList)
					range.ColumnWidthInCharacters = value;
			}
		}
		#endregion
		#region ColumnWidth
		public double? ColumnWidth {
			get { return FirstRange.ColumnWidth; }
			set {
				foreach (NativeRange range in innerList)
					range.ColumnWidth = value;
			}
		}
		#endregion
		#region RowHeight
		public double? RowHeight {
			get { return FirstRange.RowHeight; }
			set {
				foreach (NativeRange range in innerList)
					range.RowHeight = value;
			}
		}
		#endregion
		#region Formula
		public string Formula {
			get { return FirstRange.Formula; }
			set {
				foreach (NativeRange range in innerList)
					range.Formula = value;
			}
		}
		#endregion
		#region ArrayFormula
		public string ArrayFormula {
			get { return FirstRange.ArrayFormula; }
			set {
				foreach (NativeRange range in innerList)
					range.ArrayFormula = value;
			}
		}
		#endregion
		#region Value
		public CellValue Value {
			get { return FirstRange.Value; }
			set {
				foreach (NativeRange range in innerList)
					range.Value = value;
			}
		}
		#endregion
		#region Style
		public Style Style {
			get { return FirstRange.Style; }
			set {
				foreach (NativeRange range in innerList)
					range.Style = value;
			}
		}
		#endregion
		#region FillColor
		public Color FillColor {
			get { return FirstRange.FillColor; }
			set {
				foreach (NativeRange range in innerList)
					range.FillColor = value;
			}
		}
		#endregion
		#region FontName
		public string FontName {
			get { return FirstRange.FontName; }
			set {
				foreach (NativeRange range in innerList)
					range.FontName = value;
			}
		}
		#endregion
		#region FontSize
		public double FontSize {
			get { return FirstRange.FontSize; }
			set {
				foreach (NativeRange range in innerList)
					range.FontSize = value;
			}
		}
		#endregion
		#region FontColor
		public Color FontColor {
			get { return FirstRange.FontColor; }
			set {
				foreach (NativeRange range in innerList)
					range.FontColor = value;
			}
		}
		#endregion
		#region FontBold
		public bool FontBold {
			get { return FirstRange.FontBold; }
			set {
				foreach (NativeRange range in innerList)
					range.FontBold = value;
			}
		}
		#endregion
		#region FontItalic
		public bool FontItalic {
			get { return FirstRange.FontItalic; }
			set {
				foreach (NativeRange range in innerList)
					range.FontItalic = value;
			}
		}
		#endregion
		#region NumberFormat
		public string NumberFormat {
			get { return FirstRange.NumberFormat; }
			set {
				foreach (NativeRange range in innerList)
					range.NumberFormat = value;
			}
		}
		#endregion
		#region VerticalAlignment
		public VerticalAlignment VerticalAlignment {
			get { return FirstRange.VerticalAlignment; }
			set {
				foreach (NativeRange range in innerList)
					range.VerticalAlignment = value;
			}
		}
		#endregion
		#region HorizontalAlignment
		public HorizontalAlignment HorizontalAlignment {
			get { return FirstRange.HorizontalAlignment; }
			set {
				foreach (NativeRange range in innerList)
					range.HorizontalAlignment = value;
			}
		}
		#endregion
		#region Ranges
		public List<Range> Ranges {
			get {
				List<Range> result = new List<Range>();
				foreach (NativeRange range in innerList)
					result.Add((Range)range);
				return result;
			}
		}
		#endregion
		#endregion
		public DevExpress.XtraSpreadsheet.Model.CellUnion ModelRange {
			get {
				List<Model.CellRangeBase> modelCellRanges = new List<Model.CellRangeBase>();
				foreach (NativeRange nativeRange in this.innerList) {
					modelCellRanges.Add(nativeRange.ModelRange);
				}
				DevExpress.XtraSpreadsheet.Model.CellUnion modelUnionRange = new Model.CellUnion(modelCellRanges);
				return modelUnionRange;
			}
		}
		#region GroupColumn
		public void GroupColumns(bool collapse) {
			foreach (NativeRange range in innerList)
				range.GroupColumns(collapse);
		}
		#endregion
		#region UnGroupColumn
		public void UnGroupColumns(bool unhideCollapsed) {
			foreach (NativeRange range in innerList)
				range.UnGroupColumns(unhideCollapsed);
		}
		#endregion
		#region GetMergedRanges
		public IList<Range> GetMergedRanges() {
			List<Range> result = new List<Range>();
			foreach(Range range in innerList)
				result.AddRange(range.GetMergedRanges());
			return result;
		}
		#endregion
		#region GroupRow
		public void GroupRows(bool collapse) {
			foreach (NativeRange range in innerList)
				range.GroupRows(collapse);
		}
		#endregion
		#region UnGroupRow
		public void UnGroupRows(bool unhideCollapsed) {
			foreach (NativeRange range in innerList)
				range.UnGroupRows(unhideCollapsed);
		}
		#endregion
		public void GroupColumns() {
			GroupColumns(false);
		}
		#region UnGroupColumn
		public void UnGroupColumns() {
			UnGroupColumns(false);
		}
		#endregion
		#region GroupRow
		public void GroupRows() {
			GroupRows(false);
		}
		#endregion
		#region UnGroupRow
		public void UnGroupRows() {
			UnGroupRows(false);
		}
		#endregion
		#region SetInsideBorders
		public void SetInsideBorders(Color color, BorderLineStyle style) {
			foreach (NativeRange range in innerList)
				range.SetInsideBorders(color, style);
		}
		#endregion
		#region BeginUpdateFormatting
		public Style BeginUpdateFormatting() {
			return FirstRange.BeginUpdateFormatting();
		}
		#endregion
		#region EndUpdateFormatting
		public void EndUpdateFormatting(Style newFormatting) {
			FirstRange.EndUpdateFormatting(newFormatting);
		}
		#endregion
		#region Insert
		public void Insert() {
			Insert(InsertCellsMode.ShiftCellsRight);
		}
		public void Insert(InsertCellsMode mode) {
			foreach (NativeRange range in innerList)
				range.Insert(mode);
		}
		#endregion
		#region Delete
		public void Delete() {
			Delete(DeleteMode.ShiftCellsLeft);
		}
		public void Delete(DeleteMode mode) {
			foreach (NativeRange range in innerList)
				range.Delete(mode);
		}
		#endregion
		#region GetReference
		public string GetReferenceA1() {
			StringBuilder sb = new StringBuilder();
			char separator = ((NativeWorksheet)Worksheet).ModelWorkbook.DataContext.GetListSeparator();
			foreach (NativeRange range in innerList) {
				sb.Append(range.GetReferenceA1());
				sb.Append(separator);
			}
			sb.Remove(sb.Length-1, 1); 
			return sb.ToString();
		}
		public string GetReferenceR1C1(bool rowAbsolute, bool columnAbsolute, bool sheetDefinition, Cell baseCell) {
			string result = String.Empty;
			char separator = ((NativeWorksheet)Worksheet).ModelWorkbook.DataContext.GetListSeparator();
			foreach (NativeRange range in innerList)
				result += range.GetReferenceR1C1(rowAbsolute, columnAbsolute, sheetDefinition, baseCell) + separator;
			return result.Substring(0, result.Length - 1);
		}
		public string GetReferenceA1(bool rowAbsolute, bool columnAbsolute, bool sheetDefinition, Cell baseCell) {
			string result = String.Empty;
			char separator = ((NativeWorksheet)Worksheet).ModelWorkbook.DataContext.GetListSeparator();
			foreach (NativeRange range in innerList)
				result += range.GetReferenceA1(rowAbsolute, columnAbsolute, sheetDefinition) + separator;
			return result.Substring(0, result.Length - 1);
		}
		public string GetReferenceR1C1(bool rowAbsolute, bool columnAbsolute, bool sheetDefinition) {
			string result = String.Empty;
			char separator = ((NativeWorksheet)Worksheet).ModelWorkbook.DataContext.GetListSeparator();
			foreach (NativeRange range in innerList)
				result += range.GetReferenceR1C1(rowAbsolute, columnAbsolute, sheetDefinition) + separator;
			return result.Substring(0, result.Length - 1);
		}
		public string GetReferenceA1(bool rowAbsolute, bool columnAbsolute, bool sheetDefinition) {
			string result = String.Empty;
			char separator = ((NativeWorksheet)Worksheet).ModelWorkbook.DataContext.GetListSeparator();
			foreach (NativeRange range in innerList)
				result += range.GetReferenceA1(rowAbsolute, columnAbsolute, sheetDefinition) + separator;
			return result.Substring(0, result.Length - 1);
		}
		#endregion
		#region Intersect
		public Range Intersect(RangeBase other) {
			return FirstRange.Intersect(other);
		}
		#endregion
		#region IsIntersect
		public bool IsIntersecting(RangeBase other) {
			return FirstRange.IsIntersecting(other);
		}
		#endregion
		#region Offset
		public Range Offset(int rowCount, int columnCount) {
			return FirstRange.Offset(rowCount, columnCount);
		}
		#endregion
		#region Merge
		public void Merge() {
			foreach (NativeRange range in innerList)
				range.Merge();
		}
		#endregion
		#region UnMerge
		public void UnMerge() {
			foreach (NativeRange range in innerList)
				range.UnMerge();
		}
		#endregion
		#region CopyFrom
		public void CopyFrom(Range source) {
			this.CopyFrom(source, PasteSpecial.All);
		}
		public void CopyFrom(Range source, PasteSpecial pasteType) {
			if (source is UnionRange)
				DevExpress.XtraSpreadsheet.Utils.SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorSourceRangeCannotBeUnionRange);
			Model.Worksheet targetModelWorksheet = this.ModelRange.Worksheet as Model.Worksheet;
			DevExpress.XtraSpreadsheet.Model.CellRange modelRange = (source as NativeRange).ModelRange;
			Model.Worksheet sourceWorksheet = modelRange.Worksheet as Model.Worksheet;
			Model.RangeCopyOperation operation = new Model.RangeCopyOperation(targetModelWorksheet,
														sourceWorksheet,
														Model.ModelPasteSpecialFlags.All,
														modelRange,
														this.ModelRange);
			operation.Execute();
		}
		#endregion
		#region SetAllBorders
		public void SetAllBorders(Color color, BorderLineStyle style) {
			foreach (NativeRange range in innerList)
				range.SetAllBorders(color, style);
		}
		#endregion
		#region SetOutsideBorders
		public void SetOutsideBorders(Color color, BorderLineStyle style) {
			foreach (NativeRange range in innerList)
				range.SetOutsideBorders(color, style);
		}
		#endregion
		#region SetSharedFormula
		public void SetSharedFormula(int sharedFormulaIndex) {
			foreach (NativeRange range in innerList)
				range.SetSharedFormula(sharedFormulaIndex);
		}
		#endregion
		#region GetEnumerator
		IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() {
			return ((IEnumerable<Cell>)FirstRange).GetEnumerator();
		}
		#endregion
		#region System.Collections.IEnumerable.GetEnumerator
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)FirstRange).GetEnumerator();
		}
		#endregion
	}
}
#endif
