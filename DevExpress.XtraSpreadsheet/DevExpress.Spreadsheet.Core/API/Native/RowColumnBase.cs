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
using DevExpress.Office;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region Row
	public interface Row : Range {
		int Index { get; }
		bool Visible { get; set; }
		void Insert(); 
		void Delete(); 
		int GroupLevel { get; }
		string Heading { get; }
		double Height { get; set; }
		new Cell this[int columnIndex] { get; }
		Cell this[string columnHeading] { get; }
		void AutoFit();
	}
	#endregion
	#region Column
	public interface Column : Range {
		int Index { get; }
		bool Visible { get; set; }
		void Insert(); 
		void Delete(); 
		int GroupLevel { get; }
		string Heading { get; }
		double WidthInCharacters { get; set; }
		int WidthInPixels { get; set; }
		double Width { get; set; }
		new Cell this[int rowIndex] { get; }
		Cell this[string rowHeading] { get; }
		void AutoFit();
	}
	#endregion
	#region RowCollection
	public interface RowCollection  {
		void Insert(int index);
		void Insert(int index, int count);
		void Remove(int index);
		void Remove(int index, int count);
		int LastUsedIndex { get; }
		void Group(int first, int last, bool collapse);
		void UnGroup(int first, int last, bool unhideCollapsed);
		void ClearOutline(int first, int last);
		void ClearOutline();
		void AutoOutline();
		void AutoFit(int first, int last);
		void Hide(int first, int last);
		void Unhide(int first, int last);
		Row this[int rowIndex] { get; }
		Row this[string rowHeading] { get; }
	}
	#endregion
	#region ColumnCollection
	public interface ColumnCollection  {
		void Insert(int index);
		void Insert(int index, int count);
		void Remove(int index);
		void Remove(int index, int count);
		int LastUsedIndex { get; }
		void Group(int first, int last, bool collapse);
		void UnGroup(int first, int last, bool unhideCollapsed);
		void ClearOutline(int first, int last);
		void ClearOutline();
		void AutoOutline();
		void AutoFit(int first, int last);
		void Hide(int first, int last);
		void Unhide(int first, int last);
		Column this[int columnIndex] { get; }
		Column this[string columnHeading] { get; }
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using ModelSearchOptions = DevExpress.XtraSpreadsheet.Model.ModelSearchOptions;
	using DevExpress.Compatibility.System.Drawing;
	#region NativeRowColumnBase (interface)
	public interface INativeRowColumnBase {
		void Invalidate();
		void CheckCanDelete(int count);
	}
	#endregion
	#region NativeColumnRowCollectionBase (abstract)
	abstract partial class NativeColumnRowCollectionBase<T> where T : INativeRowColumnBase {
		const int maxCacheSize = 16;
		const int cacheDelta = maxCacheSize / 4;
		readonly NativeWorksheet nativeWorksheet;
		Dictionary<int, T> cachedItems = new Dictionary<int, T>();
		List<int> mruKeys = new List<int>();
		protected readonly object syncRoot = new object();
		protected NativeColumnRowCollectionBase(NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			this.nativeWorksheet = nativeWorksheet;
		}
		public abstract int LastUsedIndex { get; }
		public NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		protected Dictionary<int, T> CachedItems { get { return cachedItems; } }
		public T this[int index] {
			get {
				CheckIndex(index);
				return GetRowCore(index);
			}
		}
		protected internal T GetRowCore(int index) {
			lock (syncRoot) {
				T result;
				if (!cachedItems.TryGetValue(index, out result)) {
					RemoveLeastRecentlyUsed();
					result = CreateNativeObject(index);
					cachedItems.Add(index, result);
					mruKeys.Insert(0, index);
					return result;
				}
				UpdateMostRecentlyUsed(index);
				return result;
			}
		}
		void UpdateMostRecentlyUsed(int key) {
			int index = mruKeys.IndexOf(key);
			if (index < 0)
				return;
			mruKeys.RemoveAt(index);
			mruKeys.Insert(0, key);
		}
		void RemoveLeastRecentlyUsed() {
			int mruKeysCount = mruKeys.Count;
			if (mruKeysCount < maxCacheSize)
				return;
			int startIndex = mruKeysCount - cacheDelta;
			for (int i = 0; i < cacheDelta; i++) {
				int key = mruKeys[i + startIndex];
				cachedItems[key].Invalidate();
				cachedItems.Remove(key);
			}
			mruKeys.RemoveRange(mruKeysCount - cacheDelta, cacheDelta);
		}
		public T this[string pos] {
			get {
				int index = ParsePosition(pos);
				return this[index];
			}
		}
		public void Insert(int position) {
			CheckIndex(position);
			Invalidate();
			InsertModelItems(position, 1);
		}
		protected internal abstract void InsertModelItems(int position, int count);
		public void Insert(int position, int count) {
			CheckIndex(position);
			Invalidate();
			InsertModelItems(position, count);
		}
		public void Remove(int position) {
			Remove(position, 1);
		}
		public void Remove(int position, int count) {
			if (count <= 0)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorectCount));
			RemoveCore(position, count);
			DeleteModelItems(position, count);
		}
		void RemoveCore(int position, int count) {
			int startIndex = position + count - 1;
			for (int i = startIndex; i >= position; i--) {
				CheckIndex(position);
				RemoveApiObject(position, count);
			}
		}
		public void Invalidate() {
			lock (syncRoot) {
				foreach (T value in cachedItems.Values)
					value.Invalidate();
				cachedItems.Clear();
				mruKeys.Clear();
			}
		}
		protected internal abstract T CreateNativeObject(int index);
		protected internal abstract void DeleteModelItems(int position, int count);
		protected internal abstract int ParsePosition(string pos);
		public abstract void CheckIndex(int index);
		public void RemoveApiObject(int position, int count) {
			T deletedItem = this[position];
			deletedItem.CheckCanDelete(count);
			RemoveApiObjectCore(position);
		}
		protected virtual void RemoveApiObjectCore(int position) {
		}
	}
	#endregion
	#region NativeColumnCollection
	partial class NativeColumnCollection : NativeColumnRowCollectionBase<NativeColumn>, ColumnCollection {
		public NativeColumnCollection(NativeWorksheet nativeWorksheet)
			: base(nativeWorksheet) {
		}
		public override int LastUsedIndex {
			get {
				Range range = NativeWorksheet.GetPrintableRange();
				return (range != null) ? range.RightColumnIndex : 0;
			}
		}
		protected internal override NativeColumn CreateNativeObject(int index) {
			return new NativeColumn(NativeWorksheet, index);
		}
		protected internal override int ParsePosition(string columnHeading) {
			bool useR1C1Style = false; 
			return NativeCellReferenceParser.GetColumnIndexByHeading(columnHeading, useR1C1Style);
		}
		protected internal override void InsertModelItems(int position, int count) {
			NativeWorksheet.InsertColumns(position, count);
		}
		#region ColumnCollection Members
		Column ColumnCollection.this[int columnIndex] {
			get { return this[columnIndex]; }
		}
		Column ColumnCollection.this[string pos] {
			get { return this[pos]; }
		}
		#endregion
		public override void CheckIndex(int index) {
			NativeIndicesChecker.CheckColumnIndex(index);
		}
		public void Group(int first, int last, bool collapse) {
			CheckIndex(first);
			CheckIndex(last);
			Model.GroupColumnsCommand command = new Model.GroupColumnsCommand(NativeWorksheet.ModelWorksheet, first, last);
			command.Collapse = collapse;
			command.Execute();
		}
		public void UnGroup(int first, int last, bool unhideCollapsed) {
			CheckIndex(first);
			CheckIndex(last);
			Model.UngroupColumnsCommand command = new Model.UngroupColumnsCommand(NativeWorksheet.ModelWorksheet, first, last);
			command.UnhideCollapsed = unhideCollapsed;
			command.Execute();
		}
		public void AutoOutline() {
			Model.AutoCreateColumnsOutlineCommand command = new Model.AutoCreateColumnsOutlineCommand(NativeWorksheet.ModelWorksheet, 0, LastUsedIndex);
			command.Execute();
		}
		public void ClearOutline(int start, int end) {
			Model.ClearColumnsOutlineCommand command = new Model.ClearColumnsOutlineCommand(NativeWorksheet.ModelWorksheet, start, end);
			command.Execute();
		}
		public void ClearOutline() {
			Model.ClearWorksheetOutlineCommand command = new Model.ClearWorksheetOutlineCommand(NativeWorksheet.ModelWorksheet, true, false);
			command.Execute();
		}
		protected internal override void DeleteModelItems(int position, int count) {
			NativeWorksheet.ModelWorksheet.RemoveColumns(position, count, ApiErrorHandler.Instance);
		}
		public void AutoFit(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			Model.CellIntervalRange columnsRange = Model.CellIntervalRange.CreateColumnInterval(modelWorksheet, first, Model.PositionType.Absolute, last, Model.PositionType.Absolute);
			modelWorksheet.TryBestFitColumn(columnsRange, DevExpress.XtraSpreadsheet.Layout.Engine.ColumnBestFitMode.None);
		}
		public void Hide(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			Model.IModelErrorInfo errorInfo = modelWorksheet.HideColumns(first, last);
			ApiErrorHandler.Instance.HandleError(errorInfo);
		}
		public void Unhide(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			modelWorksheet.UnhideColumns(first, last);
		}
	}
	#endregion
	#region NativeRowCollection
	partial class NativeRowCollection : NativeColumnRowCollectionBase<NativeRow>, RowCollection {
		public NativeRowCollection(NativeWorksheet nativeWorksheet)
			: base(nativeWorksheet) {
		}
		protected internal override NativeRow CreateNativeObject(int index) {
			return new NativeRow(NativeWorksheet, index);
		}
		protected internal override int ParsePosition(string rowHeading) {
			return NativeCellReferenceParser.GetRowIndexByHeading(rowHeading);
		}
		public override int LastUsedIndex {
			get {
				DevExpress.XtraSpreadsheet.Model.Worksheet worksheet = NativeWorksheet.ModelWorksheet;
				if (worksheet.Rows.Count == 0)
					return -1;
				return worksheet.Rows.Last.Index;
			}
		}
		protected internal override void InsertModelItems(int position, int count) {
			NativeWorksheet.InsertRows(position, count);
		}
		#region ColumnCollection Members
		Row RowCollection.this[int columnIndex] {
			get { return this[columnIndex]; }
		}
		Row RowCollection.this[string pos] {
			get { return this[pos]; }
		}
		#endregion
		public override void CheckIndex(int index) {
			NativeIndicesChecker.CheckRowIndex(index);
		}
		public void Group(int start, int end, bool collapse) {
			CheckIndex(start);
			CheckIndex(end);
			Model.GroupRowsCommand command = new Model.GroupRowsCommand(NativeWorksheet.ModelWorksheet, start, end);
			command.Collapse = collapse;
			command.Execute();
		}
		public void UnGroup(int start, int end, bool unHideCollapsed) {
			CheckIndex(start);
			CheckIndex(end);
			Model.UngroupRowsCommand command = new Model.UngroupRowsCommand(NativeWorksheet.ModelWorksheet, start, end);
			command.UnhideCollapsed = unHideCollapsed;
			command.Execute();
		}
		public void AutoOutline() {
			Model.AutoCreateRowsOutlineCommand command = new Model.AutoCreateRowsOutlineCommand(NativeWorksheet.ModelWorksheet, 0, LastUsedIndex);
			command.Execute();
		}
		public void ClearOutline(int start, int end) {
			CheckIndex(start);
			CheckIndex(end);
			Model.ClearRowsOutlineCommand command = new Model.ClearRowsOutlineCommand(NativeWorksheet.ModelWorksheet, start, end);
			command.Execute();
		}
		public void ClearOutline() {
			Model.ClearWorksheetOutlineCommand command = new Model.ClearWorksheetOutlineCommand(NativeWorksheet.ModelWorksheet, false, true);
			command.Execute();
		}
		protected internal override void DeleteModelItems(int position, int count) {
			NativeWorksheet.ModelWorksheet.RemoveRows(position, count, ApiErrorHandler.Instance);
		}
		protected override void RemoveApiObjectCore(int position) {
			lock (syncRoot) {
				foreach (int key in CachedItems.Keys)
					if (key >= position)
						CachedItems[key].Invalidate();
			}
		}
		public void ClearCachedItems(int deletedColumnIndex) {
			lock (syncRoot) {
				foreach (NativeRow row in CachedItems.Values)
					row.CellCollection.Invalidate(deletedColumnIndex);
			}
		}
		public void ClearCachedItems_ShiftCellsLeft(int firstColumnIndex, int firstRowIndex, int lastRowIndex) {
			lock (syncRoot) {
				foreach (NativeRow row in CachedItems.Values) {
					if (row.Index >= firstRowIndex && row.Index < lastRowIndex)
						row.CellCollection.Invalidate(firstColumnIndex);
					else if (row.Index == lastRowIndex) {
						row.CellCollection.Invalidate(firstColumnIndex);
						return;
					}
				}
			}
		}
		public void ClearCachedItems_ShiftCellsUp(int firstColumnIndex, int lastColumnIndex, int firstRowIndex) {
			lock (syncRoot) {
				foreach (NativeRow row in CachedItems.Values) {
					if (row.Index >= firstRowIndex)
						row.CellCollection.Invalidate(firstColumnIndex, lastColumnIndex);
				}
			}
		}
		public void AutoFit(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			Model.CellIntervalRange rowsRange = Model.CellIntervalRange.CreateRowInterval(modelWorksheet, first, Model.PositionType.Absolute, last, Model.PositionType.Absolute);
			modelWorksheet.TryBestFitRow(rowsRange);
		}
		public void Hide(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			Model.IModelErrorInfo errorInfo = modelWorksheet.HideRows(first, last);
			ApiErrorHandler.Instance.HandleError(errorInfo);
		}
		public void Unhide(int first, int last) {
			CheckIndex(first);
			CheckIndex(last);
			Model.Worksheet modelWorksheet = NativeWorksheet.ModelWorksheet;
			modelWorksheet.UnhideRows(first, last);
		}
	}
	#endregion
	#region NativeRowColumnBase
	abstract partial class NativeRowColumnBase : IFormatBaseAccessor, Formatting {
		#region fields
		readonly NativeWorksheet nativeWorksheet;
		Model.CellIntervalRange modelRange;
		readonly int index;
		readonly NativeRange nativeCellRange;
		#endregion
		protected NativeRowColumnBase(NativeWorksheet nativeWorksheet, int index) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Guard.ArgumentNonNegative(index, "index");
			this.nativeWorksheet = nativeWorksheet;
			this.index = index;
			this.modelRange = CreateIntervalRange();
			this.nativeCellRange = new NativeRange(modelRange, nativeWorksheet);
		}
		#region Properties
		public NativeWorksheet NativeWorksheet { [DebuggerStepThrough] get { return nativeWorksheet; } }
		public ModelWorksheet ModelWorksheet { [DebuggerStepThrough] get { return nativeWorksheet.ModelWorksheet; } }
		public Model.CellIntervalRange ModelRange { [DebuggerStepThrough] get { return modelRange; } }
		public NativeRange NativeCellsRange { [DebuggerStepThrough] get { return nativeCellRange; } }
		public Style Style { get { return NativeCellsRange.Style; } set { NativeCellsRange.Style = value; } }
		protected Model.DocumentModel DocumentModel { get { return ModelWorksheet.Workbook; } }
		#region RowColumnBase members
		public int Index { get { return index; } }
		#endregion
		public AreasCollection Areas { get { return new NativeAreasCollection(NativeCellsRange); } }
		#endregion
		public void SetInsideBorders(Color color, BorderLineStyle style) {
			nativeCellRange.SetInsideBorders(color, style);
		}
		protected abstract Model.CellIntervalRange CreateIntervalRange();
		#region BeginUpdateFormatting / End
		public Formatting BeginUpdateFormatting() {
			return NativeCellsRange.BeginUpdateFormatting();
		}
		public void EndUpdateFormatting(Formatting newFormatting) {
			NativeCellsRange.EndUpdateFormatting(newFormatting);
		}
		#endregion
		#region IFormatBaseAccessor Members
		public abstract Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get; }
		public abstract Model.IFormatBaseBatchUpdateable ReadWriteFormat { get; }
		#endregion
		#region Formatting
		Formatting Format {
			get {
				return NativeCellsRange;
			}
		}
		string Formatting.NumberFormat { get { return Format.NumberFormat; } set { Format.NumberFormat = value; } }
		Alignment Formatting.Alignment { get { return Format.Alignment; } }
		SpreadsheetFont Formatting.Font { get { return Format.Font; } }
		Borders Formatting.Borders { get { return Format.Borders; } }
		Fill Formatting.Fill { get { return Format.Fill; } }
		Protection Formatting.Protection { get { return Format.Protection; } }
		StyleFlags Formatting.Flags { get { return Format.Flags; } }
		bool Formatting.Equals(object obj) {
			return Format.Equals(obj);
		}
		void Formatting.BeginUpdate() {
			Format.BeginUpdate();
		}
		void Formatting.EndUpdate() {
			Format.EndUpdate();
		}
		#endregion
		public Color FillColor { get { return Format.Fill.BackgroundColor; } set { Format.Fill.BackgroundColor = value; } }
		public string FontName { get { return Format.Font.Name; } set { Format.Font.Name = value; } }
		public double FontSize { get { return Format.Font.Size; } set { Format.Font.Size = value; } }
		public Color FontColor { get { return Format.Font.Color; } set { Format.Font.Color = value; } }
		#region FontBold
		public bool FontBold {
			get {
				return FontStyleConverter.IsBold(this.Format.Font.FontStyle);
			}
			set {
				Formatting formatting = this.Format;
				formatting.Font.FontStyle = FontStyleConverter.SetBold(formatting.Font.FontStyle, value);
			}
		}
		#endregion
		#region FontItalic
		public bool FontItalic {
			get {
				return FontStyleConverter.IsItalic(this.Format.Font.FontStyle);
			}
			set {
				Formatting formatting = this.Format;
				formatting.Font.FontStyle = FontStyleConverter.SetItalic(formatting.Font.FontStyle, value);
			}
		}
		#endregion
		public string NumberFormat { get { return Format.NumberFormat; } set { Format.NumberFormat = value; } }
		public SpreadsheetVerticalAlignment VerticalAlignment { get { return Format.Alignment.Vertical; } set { Format.Alignment.Vertical = value; } }
		public SpreadsheetHorizontalAlignment HorizontalAlignment { get { return Format.Alignment.Horizontal; } set { Format.Alignment.Horizontal = value; } }
		public Range CurrentRegion { get { return NativeCellsRange.CurrentRegion; } }
		public IEnumerable<Cell> Search(string text) {
			ModelSearchOptions modelOptions = nativeWorksheet.CreateDefaultSearchOptions();
			modelOptions.Range = modelRange.Clone();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		public IEnumerable<Cell> Search(string text, SearchOptions options) {
			ModelSearchOptions modelOptions = nativeWorksheet.ConvertSearchOptions(options);
			modelOptions.Range = modelRange.Clone();
			return nativeWorksheet.SearchCore(text, modelOptions);
		}
		#region Intersect
		public Range Intersect(Range other) {
			return NativeCellsRange.Intersect(other);
		}
		#endregion
		#region Union
		public Range Union(Range other) {
			if (other == null)
				return new NativeRange(NativeCellsRange.ModelRange, NativeCellsRange.NativeWorksheet);
			return NativeRangeReferenceParseHelper.CreateUnion(NativeCellsRange, other);
		}
		#endregion
		#region ExistingCells
		public IEnumerable<Cell> ExistingCells {
			get {
				IEnumerator<Model.ICellBase> modelEnumerator = modelRange.GetExistingCellsEnumerator(false);
				return new Enumerable<Cell>(new EnumeratorConverter<Model.ICellBase, Cell>(modelEnumerator, ConvertModelCellToApiCell));
			}
		}
		Cell ConvertModelCellToApiCell(Model.ICellBase cell) {
			return nativeWorksheet[cell.RowIndex, cell.ColumnIndex];
		}
		#endregion
		#region Dependents/Precedents
		public IList<Range> Precedents {
			get {
				Model.CellRangeBase precedentsRange = DocumentModel.CalculationChain.GetPrecedents(ModelRange, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> DirectPrecedents {
			get {
				Model.CellRangeBase precedentsRange = DocumentModel.CalculationChain.GetDirectPrecedents(ModelRange, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, precedentsRange);
			}
		}
		public IList<Range> Dependents {
			get {
				Model.CellRangeBase dependentsRange = DocumentModel.CalculationChain.GetDependents(ModelRange, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, dependentsRange);
			}
		}
		public IList<Range> DirectDependents {
			get {
				Model.CellRangeBase dependentsRange = DocumentModel.CalculationChain.GetDirectDependents(ModelRange, true);
				return NativeRange.ConvertModelCellRangeBaseToListOfApiRanges(NativeWorksheet.NativeWorkbook, dependentsRange);
			}
		}
		#endregion
		#region Calculate
		public void Calculate() {
			DocumentModel.CalculationChain.CalculateRange(ModelRange);
		}
		#endregion
	}
	#endregion
	#region NativeRow
	partial class NativeRow : NativeRowColumnBase, INativeRowColumnBase, Row {
		readonly NativeCellCollectionWithCache cellCollection;
		public NativeRow(NativeWorksheet nativeWorksheet, int rowIndex)
			: base(nativeWorksheet, rowIndex) {
			this.cellCollection = new NativeCellCollectionWithCache(ModelRange, nativeWorksheet);
		}
		#region Properties
		protected internal NativeCellCollectionWithCache CellCollection { get { return cellCollection; } }
		public Range Range { get { return new NativeRange(CreateIntervalRange(), NativeWorksheet); } }
		Model.Row ReadOnlyModelRow {
			get {
				Model.Row row = ModelWorksheet.Rows.TryGetRow(Index);
				if (row == null)
					return new Model.Row(Index, ModelWorksheet);
				return row;
			}
		}
		Model.Row ReadWriteModelRow {
			get {
				return ModelWorksheet.Rows[Index];
			}
		}
		#region Height
		public double Height {
			get { return NativeWorksheet.GetRowHeight(Index); }
			set {
				NativeWorksheet.BeginUpdate();
				try {
					ReadWriteModelRow.SetCustomHeight(NativeWorksheet.NativeWorkbook.UnitsToModelUnitsF((float)value));
				}
				finally {
					NativeWorksheet.EndUpdate();
				}
			}
		}
		#endregion
		#region Visible
		public bool Visible {
			get {
				return ReadOnlyModelRow.IsVisible;
			}
			set {
				if (Visible == value)
					return;
				if (value)
					ModelWorksheet.UnhideRows(Index, Index);
				else
					ModelWorksheet.HideRows(Index, Index);
			}
		}
		#endregion
		#region GroupLevel
		public int GroupLevel { get { return ReadOnlyModelRow.OutlineLevel; } }
		#endregion
		public string GetReferenceA1() {
			return ModelRange.ToString(DocumentModel.DataContext);
		}
		#region Name
		string Range.Name {
			get {
				return NativeWorksheet.GetDefinedNameFromRange(this);
			}
			set {
				string reference = String.Concat('=', NativeCellsRange.GetReferenceA1(
					ReferenceElement.RowAbsolute |
					ReferenceElement.ColumnAbsolute |
					ReferenceElement.IncludeSheetName));
				NativeWorksheet.DefinedNames.Add(value, reference);
			}
		}
		#endregion
		#region Heading
		public string Heading { get { return (Index + 1).ToString(); } }
		#endregion
		public Worksheet Worksheet { get { return NativeWorksheet; } }
		public string Formula { get { return NativeCellsRange.Formula; } set { NativeCellsRange.Formula = value; } }
		public string FormulaInvariant { get { return NativeCellsRange.FormulaInvariant; } set { NativeCellsRange.FormulaInvariant = value; } }
		public string ArrayFormula { get { return NativeCellsRange.ArrayFormula; } set { NativeCellsRange.ArrayFormula = value; } }
		public string ArrayFormulaInvariant { get { return NativeCellsRange.ArrayFormulaInvariant; } set { NativeCellsRange.ArrayFormulaInvariant = value; } }
		public CellValue Value { get { return NativeCellsRange.Value; } set { NativeCellsRange.Value = value; } }
		public bool IsMerged {
			get { return NativeCellsRange.IsMerged; }
		}
		public Cell this[int columnIndex] {
			get {
				if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, "columnIndex");
				return NativeWorksheet.GetCell(Index, columnIndex);
			}
		}
		#region Item
		public Cell this[string columnHeading] {
			get {
				int columnIndex = Model.CellReferenceParser.ParseColumnPartA1Style(columnHeading);
				if (columnIndex == Int32.MinValue)
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnHeading, "columnName");
				return this[columnIndex];
			}
		}
		#endregion
		public bool HasFormula { get { return NativeCellsRange.HasFormula; } }
		public bool HasArrayFormula { get { return NativeCellsRange.HasArrayFormula; } }
		public override Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get { return ReadOnlyModelRow; } }
		public override Model.IFormatBaseBatchUpdateable ReadWriteFormat { get { return ReadWriteModelRow; } }
		int Range.TopRowIndex { get { return Index; } }
		int Range.BottomRowIndex { get { return Index; } }
		int Range.LeftColumnIndex { get { return 0; } }
		int Range.RightColumnIndex { get { return ModelWorksheet.MaxColumnCount - 1; } }
		int Range.RowCount { get { return 1; } }
		int Range.ColumnCount { get { return ModelWorksheet.MaxColumnCount; } }
		double Range.ColumnWidthInCharacters { get { return NativeCellsRange.ColumnWidthInCharacters; } set { NativeCellsRange.ColumnWidthInCharacters = value; } }
		double Range.ColumnWidth { get { return NativeCellsRange.ColumnWidth; } set { NativeCellsRange.ColumnWidth = value; } }
		double Range.RowHeight { get { return NativeCellsRange.RowHeight; } set { NativeCellsRange.RowHeight = value; } }
		Cell Range.this[int rowOffset, int columnOffset] { get { return NativeCellsRange[rowOffset, columnOffset]; } }
		#endregion
		#region CreateIntervalRange
		protected override Model.CellIntervalRange CreateIntervalRange() {
			return Model.CellIntervalRange.CreateRowInterval(NativeWorksheet.ModelWorksheet, Index, Model.PositionType.Absolute, Index, Model.PositionType.Absolute);
		}
		#endregion
		#region GetCell
		public Cell GetCell(ModelCellKey key) {
			return CellCollection[key];
		}
		#endregion
		#region Delete
		public void Insert() {
			NativeWorksheet.Rows.Insert(Index);
		}
		public void Delete() {
			NativeWorksheet.Rows.Remove(Index);
		}
		#endregion
		Model.CellRange GetThisAsModelCellRange() {
			return ModelRange;
		}
		NativeRange GetThisAsNativeRange() {
			return NativeCellsRange;
		}
		string Range.GetReferenceA1() {
			return GetThisAsModelCellRange().ToString();
		}
		string Range.GetReferenceA1(ReferenceElement flags) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceA1(flags);
		}
		string Range.GetReferenceR1C1(ReferenceElement flags, Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(flags, baseCell);
		}
		string Range.GetReferenceR1C1(Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(baseCell);
		}
		Range Range.GetRangeWithAbsoluteReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithAbsoluteReference();
		}
		Range Range.GetRangeWithRelativeReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithRelativeReference();
		}
		public void SetValue(object value) {
			this.Value = CellValue.FromObject(value);
		}
		#region GetMergedRanges
		public IList<Range> GetMergedRanges() {
			return NativeCellsRange.GetMergedRanges();
		}
		#endregion
		#region IsIntersect
		public bool IsIntersecting(Range other) {
			return NativeCellsRange.IsIntersecting(other);
		}
		#endregion
		#region Offset
		public Range Offset(int rowCount, int columnCount) {
			return NativeCellsRange.Offset(Index, columnCount);
		}
		#endregion
		#region Merge
		public void Merge() {
			NativeCellsRange.Merge();
		}
		#endregion
		#region UnMerge
		public void UnMerge() {
			NativeCellsRange.UnMerge();
		}
		#endregion
		void Range.CopyFrom(Range source) {
			(this as Range).CopyFrom(source, PasteSpecial.All);
		}
		void Range.CopyFrom(Range source, PasteSpecial pasteType) {
			this.NativeWorksheet.CopyRange(source, this.ModelRange, pasteType);
		}
		void Range.MoveTo(Range target) {
			this.NativeWorksheet.MoveRangeTo(this.ModelRange, target);
		}
		#region Invalidate
		public void Invalidate() {
			cellCollection.Invalidate();
			cellCollection.ClearCachedItems();
		}
		#endregion
		#region INativeRowColumnBase.CheckCanDelete
		void INativeRowColumnBase.CheckCanDelete(int count) {
			int sheetId = ModelWorksheet.SheetId;
			bool isNonIntersectsWithArray = ModelWorksheet.ArrayFormulaRanges.CanRangeRemove(ModelRange, Model.RemoveCellMode.ShiftCellsUp);
			if (!isNonIntersectsWithArray)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray);
			Model.CellIntervalRange deletedRange = Model.CellIntervalRange.CreateRowInterval(ModelWorksheet, Index, Model.PositionType.Absolute, Index + count - 1, Model.PositionType.Absolute);
			bool isNonIntersectsWithTableHeader = ModelWorksheet.Tables.CanRangeRemove(deletedRange, Model.RemoveCellMode.ShiftCellsUp);
			if (!isNonIntersectsWithTableHeader)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveTableHeader);
		}
		#endregion
		public void AutoFit() {
			NativeWorksheet.ModelWorksheet.TryBestFitRow(this.Index);
		}
		public IEnumerator<Cell> GetEnumerator() {
			IEnumerable<Cell> enumerable = NativeCellsRange;
			return enumerable.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			IEnumerable<Cell> enumerable = NativeCellsRange;
			return enumerable.GetEnumerator();
		}
		public override string ToString() {
			return String.Format("Row: {0} worksheet:\"{1}\"", GetReferenceA1(), Worksheet.Name);
		}
	}
	#endregion
	#region NativeColumn
	partial class NativeColumn : NativeRowColumnBase, INativeRowColumnBase, Column {
		public NativeColumn(NativeWorksheet NativeWorksheet, int index)
			: base(NativeWorksheet, index) {
		}
		#region Properties
		public Cell this[int rowIndex] {
			get {
				if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, "rowIndex");
				return NativeWorksheet.GetCell(rowIndex, Index);
			}
		}
		#region Item
		public Cell this[string rowHeading] {
			get {
				int rowIndex = Model.CellReferenceParser.ParseRowPartA1Style(rowHeading);
				if (rowIndex == Int32.MinValue)
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeading, "rowName");
				return this[rowIndex];
			}
		}
		#endregion
		#region WidthInCharacters
		public double WidthInCharacters {
			get {
				return NativeWorksheet.GetColumnWidthInCharacters(Index);
			}
			set {
				NativeWorksheet.BeginUpdate();
				try {
					GetModelColumn().SetCustomWidth((float)value);
				}
				finally {
					NativeWorksheet.EndUpdate();
				}
			}
		}
		#endregion
		#region WidthInPixels
		public int WidthInPixels {
			get {
				float maxDigitWidth = ModelWorksheet.Workbook.MaxDigitWidth;
				float maxDigitWidthInPixels = ModelWorksheet.Workbook.MaxDigitWidthInPixels;
				float layoutWidth = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>().CalculateColumnWidth(ModelWorksheet, Index, maxDigitWidth, maxDigitWidthInPixels);
				float modelWidth = ModelWorksheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutWidth);
				return NativeWorksheet.NativeWorkbook.DocumentModel.UnitConverter.ModelUnitsToPixels((int)Math.Round(modelWidth, 0), Model.DocumentModel.Dpi);
			}
			set {
				NativeWorksheet.BeginUpdate();
				try {
					float modelWidth = NativeWorksheet.NativeWorkbook.ModelWorkbook.UnitConverter.PixelsToModelUnits(value, Model.DocumentModel.Dpi);
					float layoutWidth = NativeWorksheet.NativeWorkbook.ModelWorkbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelWidth);
					Model.IColumnWidthCalculationService service = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>();
					float widthInCharacters = service.ConvertLayoutsToCharacters(ModelWorksheet, layoutWidth, value);
					widthInCharacters = service.RemoveGaps(ModelWorksheet, widthInCharacters);
					GetModelColumn().SetCustomWidth(widthInCharacters);
				}
				finally {
					NativeWorksheet.EndUpdate();
				}
			}
		}
		#endregion
		#region Width
		public double Width {
			get { return NativeWorksheet.GetColumnWidth(Index); }
			set {
				this.NativeWorksheet.BeginUpdate();
				try {
					float modelWidth = NativeWorksheet.NativeWorkbook.UnitsToModelUnitsF((float)value);
					float layoutWidth = NativeWorksheet.NativeWorkbook.ModelWorkbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelWidth);
					int inPixels = NativeWorksheet.NativeWorkbook.ModelWorkbook.UnitConverter.ModelUnitsToPixels((int)modelWidth, Model.DocumentModel.Dpi);
					Model.IColumnWidthCalculationService service = ModelWorksheet.Workbook.GetService<Model.IColumnWidthCalculationService>();
					float widthInCharacters = service.ConvertLayoutsToCharacters(ModelWorksheet, layoutWidth, inPixels);
					widthInCharacters = service.RemoveGaps(ModelWorksheet, widthInCharacters);
					GetModelColumn().SetCustomWidth(widthInCharacters);
				}
				finally {
					NativeWorksheet.EndUpdate();
				}
			}
		}
		#endregion
		public bool HasArrayFormula { get { return NativeCellsRange.HasArrayFormula; } }
		public bool HasFormula { get { return NativeCellsRange.HasFormula; } }
		#region Visible
		public bool Visible {
			get {
				return GetReadonlyModelColumn().IsVisible;
			}
			set {
				if (Visible == value)
					return;
				if (value)
					ModelWorksheet.UnhideColumns(Index, Index);
				else
					ModelWorksheet.HideColumns(Index, Index);
			}
		}
		#endregion
		public int GroupLevel { get { return GetReadonlyModelColumn().OutlineLevel; } }
		public string GetReferenceA1() {
			Model.CellIntervalRange range = Model.CellIntervalRange.CreateColumnInterval(GetReadonlyModelColumn().Sheet, Index, Model.PositionType.Absolute, Index, Model.PositionType.Absolute);
			return range.ToString(NativeWorksheet.ModelWorkbook.DataContext);
		}
		public Worksheet Worksheet { get { return NativeWorksheet; } }
		public string Formula {
			get { return NativeCellsRange.Formula; }
			set { NativeCellsRange.Formula = value; }
		}
		public string FormulaInvariant {
			get { return NativeCellsRange.FormulaInvariant; }
			set { NativeCellsRange.FormulaInvariant = value; }
		}
		public string ArrayFormula {
			get { return NativeCellsRange.ArrayFormula; }
			set { NativeCellsRange.ArrayFormula = value; }
		}
		public string ArrayFormulaInvariant {
			get { return NativeCellsRange.ArrayFormulaInvariant; }
			set { NativeCellsRange.ArrayFormulaInvariant = value; }
		}
		public CellValue Value {
			get { return NativeCellsRange.Value; }
			set { NativeCellsRange.Value = value; }
		}
		public bool IsMerged { get { return NativeCellsRange.IsMerged; } }
		public override Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get { return GetReadonlyModelColumn(); } }
		public override Model.IFormatBaseBatchUpdateable ReadWriteFormat { get { return GetModelColumn(); } }
		#endregion
		#region Range properties
		string Range.Name {
			get {
				return NativeWorksheet.GetDefinedNameFromRange(this);
			}
			set {
				string reference = String.Concat('=', GetThisAsNativeRange().GetReferenceA1(ReferenceElement.ColumnAbsolute | ReferenceElement.RowAbsolute | ReferenceElement.IncludeSheetName));
				NativeWorksheet.DefinedNames.Add(value, reference);
			}
		}
		Model.CellRange GetThisAsModelCellRange() {
			return ModelRange;
		}
		NativeRange GetThisAsNativeRange() {
			return NativeCellsRange;
		}
		string Range.GetReferenceA1() {
			return GetThisAsModelCellRange().ToString();
		}
		string Range.GetReferenceA1(ReferenceElement flags) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceA1(flags);
		}
		string Range.GetReferenceR1C1(ReferenceElement flags, Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(flags, baseCell);
		}
		string Range.GetReferenceR1C1(Cell baseCell) {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetReferenceR1C1(baseCell);
		}
		Range Range.GetRangeWithAbsoluteReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithAbsoluteReference();
		}
		Range Range.GetRangeWithRelativeReference() {
			NativeRange nativeRange = GetThisAsNativeRange();
			return nativeRange.GetRangeWithRelativeReference();
		}
		int Range.TopRowIndex { get { return 0; } }
		int Range.BottomRowIndex { get { return NativeWorksheet.ModelWorksheet.MaxRowCount - 1; } }
		int Range.LeftColumnIndex { get { return Index; } }
		int Range.RightColumnIndex { get { return Index; } }
		int Range.RowCount { get { return NativeWorksheet.ModelWorksheet.MaxRowCount; } }
		int Range.ColumnCount { get { return 1; } }
		double Range.ColumnWidth { get { return NativeCellsRange.ColumnWidth; } set { NativeCellsRange.ColumnWidth = value; } }
		double Range.ColumnWidthInCharacters { get { return NativeCellsRange.ColumnWidthInCharacters; } set { NativeCellsRange.ColumnWidthInCharacters = value; } }
		double Range.RowHeight { get { return NativeCellsRange.RowHeight; } set { NativeCellsRange.RowHeight = value; } }
		Cell Range.this[int rowOffset, int columnOffset] { get { return NativeCellsRange[rowOffset, columnOffset]; } }
		#endregion
		public void SetValue(object value) {
			this.Value = CellValue.FromObject(value);
		}
		public void Insert() {
			NativeWorksheet.Columns.Insert(Index);
		}
		#region Delete
		public void Delete() {
			Invalidate();
			NativeWorksheet.Columns.Remove(Index);
		}
		#endregion
		#region GetMergedRanges
		public IList<Range> GetMergedRanges() {
			return NativeCellsRange.GetMergedRanges();
		}
		#endregion
		#region IsIntersect
		public bool IsIntersecting(Range other) {
			return NativeCellsRange.IsIntersecting(other);
		}
		#endregion
		#region Offset
		public Range Offset(int rowCount, int columnCount) {
			return NativeCellsRange.Offset(rowCount, columnCount);
		}
		#endregion
		#region Merge
		public void Merge() {
			NativeCellsRange.Merge();
		}
		#endregion
		#region UnMerge
		public void UnMerge() {
			NativeCellsRange.UnMerge();
		}
		#endregion
		void Range.CopyFrom(Range source) {
			(this as Range).CopyFrom(source, PasteSpecial.All);
		}
		void Range.CopyFrom(Range source, PasteSpecial pasteType) {
			this.NativeWorksheet.CopyRange(source, this.ModelRange, pasteType);
		}
		void Range.MoveTo(Range target) {
			this.NativeWorksheet.MoveRangeTo(this.ModelRange, target);
		}
		#region Invalidate
		public void Invalidate() {
		}
		#endregion
		#region CreateIntervalRange
		protected override Model.CellIntervalRange CreateIntervalRange() {
			return Model.CellIntervalRange.CreateColumnInterval(NativeWorksheet.ModelWorksheet, Index, Model.PositionType.Absolute, Index, Model.PositionType.Absolute);
		}
		#endregion
		#region INativeRowColumnBase.CheckCanDelete
		void INativeRowColumnBase.CheckCanDelete(int count) {
			int sheetId = ModelWorksheet.SheetId;
			bool isIntersectsWithArray = ModelWorksheet.ArrayFormulaRanges.CanRangeRemove(ModelRange, Model.RemoveCellMode.ShiftCellsUp);
			if (!isIntersectsWithArray)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray);
		}
		#endregion
		public Model.Column GetReadonlyModelColumn() { return NativeWorksheet.ModelWorksheet.Columns.GetColumnRangeForReading(Index); }
		public Model.Column GetModelColumn() { return NativeWorksheet.ModelWorksheet.Columns.GetIsolatedColumn(Index); }
		#region Range Members
		#endregion
		#region Heading
		public string Heading {
			get {
				if (this.Worksheet.Workbook.DocumentSettings.R1C1ReferenceStyle)
					return (Index + 1).ToString();
				else
					return Model.CellReferenceParser.ColumnIndexToString(Index);
			}
		}
		#endregion
		public void AutoFit() {
			NativeWorksheet.ModelWorksheet.TryBestFitColumn(this.Index, Layout.Engine.ColumnBestFitMode.None);
		}
		public IEnumerator<Cell> GetEnumerator() {
			IEnumerable<Cell> enumerable = NativeCellsRange;
			return enumerable.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			IEnumerable<Cell> enumerable = NativeCellsRange;
			return enumerable.GetEnumerator();
		}
		public override string ToString() {
			return String.Format("Column: {0} worksheet:\"{1}\"", GetReferenceA1(), Worksheet.Name);
		}
	}
	#endregion
	#region NativeCellReferenceParser
	public static class NativeCellReferenceParser {
		public static int GetRowIndexByHeading(string rowHeading) {
			int index = Model.CellReferenceParser.ParseRowPartA1Style(rowHeading);
			if (index == Int32.MinValue)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeading, "rowName");
			return index;
		}
		public static int GetColumnIndexByHeading(string columnHeading, bool R1C1ReferenceStyle) {
			int index = R1C1ReferenceStyle ? Model.CellReferenceParser.ParseRowPartA1Style(columnHeading) : Model.CellReferenceParser.ParseColumnPartA1Style(columnHeading);
			if (index == Int32.MinValue)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnHeading, "columnName");
			return index;
		}
	}
	#endregion
	#region NativeIndicesChecker
	public static class NativeIndicesChecker {
		public static void CheckRowIndex(int rowIndex) {
			CheckNegative(rowIndex);
			if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
				ThrowException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, rowIndex);
		}
		public static void CheckColumnIndex(int columnIndex) {
			CheckNegative(columnIndex);
			if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
				ThrowException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, columnIndex);
		}
		static void CheckNegative(int index) {
			if (index < 0)
				ThrowException(XtraSpreadsheetStringId.Msg_ErrorNegativeIndexNotAllowed, index);
		}
		static void ThrowException(XtraSpreadsheetStringId stringId, int index) {
			throw new ArgumentOutOfRangeException(XtraSpreadsheetLocalizer.GetString(stringId), index.ToString());
		}
	}
	#endregion
}
