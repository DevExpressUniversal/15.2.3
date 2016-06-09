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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelSearchDirection
	public enum ModelSearchDirection {
		ByRows,
		ByColumns,
	}
	#endregion
	#region ModelSearchLookIn
	public enum ModelSearchLookIn {
		Formulas,
		ValuesAndFormulas,
		Values,
		Comments,
	}
	#endregion
	#region ModelSearchOptions
	public class ModelSearchOptions {
		DocumentModel documentModel;
		Worksheet sheet;
		CellRangeBase range;
		public DocumentModel DocumentModel {
			get { return documentModel; }
			set {
				documentModel = value;
				sheet = null;
				range = null;
			}
		}
		public Worksheet Sheet {
			get { return sheet; }
			set {
				sheet = value;
				if (sheet != null)
					documentModel = sheet.Workbook;
				else
					documentModel = null;
				range = null;
			}
		}
		public CellRangeBase Range {
			get { return range; }
			set {
				range = value;
				if (range != null) {
					sheet = range.Worksheet as Worksheet;
					if (sheet != null)
						documentModel = sheet.Workbook;
					else
						documentModel = null;
				}
				else {
					sheet = null;
					documentModel = null;
				}
			}
		}
		public ModelSearchDirection Direction { get; set; }
		public ModelSearchLookIn LookIn { get; set; }
		public bool MatchCase { get; set; }
		public bool MatchEntireCellContents { get; set; }
	}
	#endregion
	#region SpreadsheetSearchEngine
	public class SpreadsheetSearchEngine {
		public IEnumerator<ICell> Search(string text, ModelSearchOptions options) {
			if (options.LookIn == ModelSearchLookIn.Comments)
				return new EnumeratorConverter<Comment, ICell>(SearchComments(text, options), ConvertCommentToCell);
			else
				return SearchInCells(text, options);
		}
		public IEnumerator<Comment> SearchComments(string text, ModelSearchOptions options) {
			IEnumerator<Comment> nonFilteredComments = GetComments(options);
			SearchPredicate<Comment> predicate = CreateCommentPredicate(text, options);
			return new FilteredEnumerator<Comment>(nonFilteredComments, predicate.Calculate);
		}
		IEnumerator<ICell> SearchInCells(string text, ModelSearchOptions options) {
			IEnumerator<ICell> nonFilteredCells = GetCells(options);
			SearchPredicate<ICell> predicate = CreateCellPredicate(text, options);
			return new FilteredEnumerator<ICell>(nonFilteredCells, predicate.Calculate);
		}
		SearchPredicate<ICell> CreateCellPredicate(string text, ModelSearchOptions options) {
			SearchPredicate<ICell> result = CreateCellPredicateCore(options);
			result.Text = text;
			ApplyOptions(result, options);
			return result;
		}
		SearchPredicate<ICell> CreateCellPredicateCore(ModelSearchOptions options) {
			if (options.LookIn == ModelSearchLookIn.Formulas)
				return new CellSearchInValuesOrFormulasPredicate();
			else if (options.LookIn == ModelSearchLookIn.ValuesAndFormulas)
				return new CellSearchInValuesAndFormulasPredicate();
			else
				return new CellSearchInValuesPredicate();
		}
		SearchPredicate<Comment> CreateCommentPredicate(string text, ModelSearchOptions options) {
			SearchPredicate<Comment> result = new SearchCommentsPredicate();
			result.Text = text;
			ApplyOptions(result, options);
			return result;
		}
		void ApplyOptions<T>(SearchPredicate<T> predicate, ModelSearchOptions options) {
			predicate.MatchEntireCellContents = options.MatchEntireCellContents;
			predicate.MatchCase = options.MatchCase;
			predicate.Culture = options.DocumentModel.DataContext.Culture;
		}
		IEnumerator<ICell> GetCells(ModelSearchOptions options) {
			if (options.Range != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCellsByRows(options.Range);
				else
					return GetCellsByColumns(options.Range);
			}
			if (options.Sheet != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCellsByRows(options.Sheet);
				else
					return GetCellsByColumns(options.Sheet);
			}
			if (options.DocumentModel != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCellsByRows(options.DocumentModel);
				else
					return GetCellsByColumns(options.DocumentModel);
			}
			return new List<ICell>().GetEnumerator();
		}
		IEnumerator<Comment> GetComments(ModelSearchOptions options) {
			if (options.Range != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCommentsByRows(options.Range);
				else
					return GetCommentsByColumns(options.Range);
			}
			if (options.Sheet != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCommentsByRows(options.Sheet);
				else
					return GetCommentsByColumns(options.Sheet);
			}
			if (options.DocumentModel != null) {
				if (options.Direction == ModelSearchDirection.ByRows)
					return GetCommentsByRows(options.DocumentModel);
				else
					return GetCommentsByColumns(options.DocumentModel);
			}
			return new List<Comment>().GetEnumerator();
		}
		#region Cells and Comments enumerators
		#region Cells enumerators
		IEnumerator<ICell> GetCellsByRows(DocumentModel documentModel) {
			return new EntireWorkbookEnumerator<ICell>(documentModel, GetCellsByRows);
		}
		IEnumerator<ICell> GetCellsByRows(Worksheet sheet) {
			IEnumerator<ICellBase> existingCells = new EntireWorksheetExistingCellRangeEnumerator(sheet);
			return new EnumeratorConverter<ICellBase, ICell>(existingCells, ConvertToCell);
		}
		IEnumerator<ICell> GetCellsByRows(CellRangeBase range) {
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumerator(false);
			return new EnumeratorConverter<ICellBase, ICell>(existingCells, ConvertToCell);
		}
		IEnumerator<ICell> GetCellsByColumns(DocumentModel documentModel) {
			return new EntireWorkbookEnumerator<ICell>(documentModel, GetCellsByColumns);
		}
		IEnumerator<ICell> GetCellsByColumns(Worksheet sheet) {
			return GetCellsByColumns(CalculateSearchRange(sheet));
		}
		IEnumerator<ICell> GetCellsByColumns(CellRangeBase range) {
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumeratorByColumns();
			return new EnumeratorConverter<ICellBase, ICell>(existingCells, ConvertToCell);
		}
		#endregion
		ICell ConvertCommentToCell(Comment comment) {
			ICell cell = comment.Worksheet.TryGetCell(comment.Reference.Column, comment.Reference.Row);
			if (cell == null)
				return new FakeCell(comment.Reference, comment.Worksheet);
			else
				return cell;
		}
		#region Comments enumerators
		IEnumerator<Comment> GetCommentsByRows(DocumentModel documentModel) {
			return new EntireWorkbookEnumerator<Comment>(documentModel, GetCommentsByRows);
		}
		IEnumerator<Comment> GetCommentsByRows(Worksheet sheet) {
			List<Comment> result = GetComments(sheet);
			result.Sort(CommentsByRowsComparer.Instance);
			return result.GetEnumerator();
		}
		IEnumerator<Comment> GetCommentsByRows(CellRangeBase range) {
			List<Comment> result = GetComments(range);
			result.Sort(CommentsByRowsComparer.Instance);
			return result.GetEnumerator();
		}
		IEnumerator<Comment> GetCommentsByColumns(DocumentModel documentModel) {
			return new EntireWorkbookEnumerator<Comment>(documentModel, GetCommentsByColumns);
		}
		IEnumerator<Comment> GetCommentsByColumns(Worksheet sheet) {
			List<Comment> result = GetComments(sheet);
			result.Sort(CommentsByColumnsComparer.Instance);
			return result.GetEnumerator();
		}
		IEnumerator<Comment> GetCommentsByColumns(CellRangeBase range) {
			List<Comment> result = GetComments(range);
			result.Sort(CommentsByColumnsComparer.Instance);
			return result.GetEnumerator();
		}
		List<Comment> GetComments(Worksheet sheet) {
			List<Comment> result = new List<Comment>();
			result.AddRange(sheet.Comments.InnerList);
			return result;
		}
		List<Comment> GetComments(CellRangeBase range) {
			List<Comment> result = new List<Comment>();
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet == null)
				return result;
			CommentCollection comments = sheet.Comments;
			int count = comments.Count;
			for (int i = 0; i < count; i++) {
				CellPosition position = comments[i].Reference;
				if (range.ContainsCell(position.Column, position.Row))
					result.Add(comments[i]);
			}
			return result;
		}
		#endregion
		#endregion
		CellRange CalculateSearchRange(Worksheet sheet) {
			Row lastRow = sheet.Rows.Last;
			if (lastRow == null)
				return new CellRange(sheet, new CellPosition(0, 0), new CellPosition(0, 0));
			int maxColumnIndex = 0;
			foreach (Row row in sheet.Rows)
				maxColumnIndex = Math.Max(maxColumnIndex, row.LastColumnIndex);
			return new CellRange(sheet, new CellPosition(0, 0), new CellPosition(maxColumnIndex, lastRow.Index));
		}
		ICell ConvertToCell(ICellBase info) {
			return (ICell)info;
		}
	}
	#endregion
	#region SpreadsheetSearchHelper
	public static class SpreadsheetSearchHelper {
		internal static CellRange GetSearchRange(Worksheet activeSheet) {
			CellRange searchRange = activeSheet.Selection.ActiveRange;
			if (searchRange.CellCount == 1 || IsMergedCellSelected(activeSheet, searchRange))
				searchRange = new CellRange(activeSheet, new CellPosition(0, 0), new CellPosition(activeSheet.MaxColumnCount - 1, activeSheet.MaxRowCount - 1));
			return searchRange;
		}
		static bool IsMergedCellSelected(Worksheet activeSheet, CellRange activeRange) {
			CellRange result = activeSheet.MergedCells.FindMergedCell(activeRange.LeftColumnIndex, activeRange.TopRowIndex);
			if (result == null)
				return false;
			return result.Equals(activeRange);
		}
		public static bool SearchInSingleSelection(Worksheet activeSheet, FindReplaceViewModel viewModel) {
			SheetViewSelection selection = activeSheet.Selection;
			CellRange searchRange = GetSearchRange(activeSheet);
			CellPosition position = FindNextCycled(activeSheet, viewModel, selection.ActiveCell, searchRange);
			if (!position.EqualsPosition(CellPosition.InvalidValue)) {
				selection.SetActiveCellSmart(position);
				return true;
			}
			else
				return false;
		}
		public static bool SearchInMultiSelection(Worksheet activeSheet, FindReplaceViewModel viewModel) {
			SheetViewSelection selection = activeSheet.Selection;
			int i = selection.ActiveRangeIndex;
			bool allowFromToBeResult = false;
			for (; ; ) {
				CellRange searchRange = selection.SelectedRanges[i];
				CellPosition activeCell = searchRange.ContainsCell(selection.ActiveCell.Column, selection.ActiveCell.Row) ? selection.ActiveCell : searchRange.TopLeft;
				CellPosition position = TryFindNextInRange(activeSheet, viewModel, CalculateSearchSubrangeInitialPosition(activeSheet, activeCell, searchRange, viewModel), activeCell, allowFromToBeResult);
				if (!position.EqualsPosition(CellPosition.InvalidValue)) {
					selection.SetActiveCellSmart(position);
					return true;
				}
				i++;
				i %= selection.SelectedRanges.Count;
				if (i == selection.ActiveRangeIndex)
					return false;
				allowFromToBeResult = true;
			}
		}
		static CellPosition FindNextCycled(Worksheet activeSheet, FindReplaceViewModel viewModel, CellPosition from, CellRange searchRange) {
			CellPosition position = TryFindNextInRange(activeSheet, viewModel, CalculateSearchSubrangeInitialPosition(activeSheet, from, searchRange, viewModel), from, false);
			if (!position.EqualsPosition(CellPosition.InvalidValue))
				return position;
			return TryFindNextInRange(activeSheet, viewModel, searchRange, searchRange.TopLeft, true);  
		}
		static CellRange CalculateSearchSubrangeInitialPosition(Worksheet activeSheet, CellPosition activeCell, CellRange searchRange, FindReplaceViewModel viewModel) {
			CellPosition topLeft = CalculateSearchSubrangeTopLeftPosition(activeCell, searchRange, viewModel);
			return new CellRange(activeSheet, topLeft, searchRange.BottomRight);
		}
		static CellPosition CalculateSearchSubrangeTopLeftPosition(CellPosition activeCell, CellRange searchRange, FindReplaceViewModel viewModel) {
			if (viewModel.StringToSearchBy(viewModel.SearchBy) == DevExpress.Spreadsheet.SearchBy.Rows)
				return new CellPosition(searchRange.TopLeft.Column, activeCell.Row);
			else
				return new CellPosition(activeCell.Column, searchRange.TopLeft.Row);
		}
		public static IEnumerator<ICell> FindAll(Worksheet activeSheet, FindReplaceViewModel viewModel) {
			CellRange searchRange = GetSearchRange(activeSheet);
			return FindAll(activeSheet, viewModel, searchRange);
		}
		public static IEnumerator<ICell> FindAll(Worksheet activeSheet, FindReplaceViewModel viewModel, CellRange searchRange) {
			SpreadsheetSearchEngine engine = new SpreadsheetSearchEngine();
			ModelSearchOptions options = CreateSearchOptions(activeSheet, viewModel, searchRange);
			IEnumerator<ICell> enumerator = engine.Search(viewModel.FindWhat, options);
			return enumerator;
		}
		static CellPosition TryFindNextInRange(Worksheet activeSheet, FindReplaceViewModel viewModel, CellRange searchRange, CellPosition from, bool allowFromToBeResult) {
			IEnumerator<ICell> enumerator = FindAll(activeSheet, viewModel, searchRange);
			return TryFindNextInAllFound(enumerator, from, allowFromToBeResult);
		}
		public static CellPosition TryFindNextInAllFound(IEnumerator<ICell> enumerator, CellPosition from, bool allowFromToBeResult) {
			while (enumerator.MoveNext()) {
				ICell currentCell = enumerator.Current;
				if (currentCell.RowIndex >= from.Row && currentCell.ColumnIndex >= from.Column)
					break;
			}
			if (enumerator.Current == null)
				return CellPosition.InvalidValue;
			if (allowFromToBeResult || !enumerator.Current.Position.EqualsPosition(from))
				return enumerator.Current.Position;
			if (!enumerator.MoveNext())
				return CellPosition.InvalidValue;
			return enumerator.Current.Position;
		}
		internal static ModelSearchOptions CreateSearchOptions(Worksheet activeSheet, FindReplaceViewModel viewModel, CellRange range) {
			ModelSearchOptions result = new ModelSearchOptions();
			if (range != null)
				result.Range = range;
			else
				result.Sheet = activeSheet;
			result.MatchCase = viewModel.MatchCase;
			result.MatchEntireCellContents = viewModel.MatchEntireCellContents;
			result.Direction = (ModelSearchDirection)viewModel.StringToSearchBy(viewModel.SearchBy);
			result.LookIn = (ModelSearchLookIn)viewModel.StringToSearchIn(viewModel.SearchIn);
			return result;
		}
	}
	#endregion
	#region CommentsByRowsComparer
	public class CommentsByRowsComparer : IComparer<Comment> {
		static readonly CommentsByRowsComparer instance = new CommentsByRowsComparer();
		public static CommentsByRowsComparer Instance { get { return instance; } }
		public int Compare(Comment x, Comment y) {
			int result = Comparer<int>.Default.Compare(x.Reference.Row, y.Reference.Row);
			if (result == 0)
				return Comparer<int>.Default.Compare(x.Reference.Column, y.Reference.Column);
			else
				return result;
		}
	}
	#endregion
	#region CommentsByColumnsComparer
	public class CommentsByColumnsComparer : IComparer<Comment> {
		static readonly CommentsByColumnsComparer instance = new CommentsByColumnsComparer();
		public static CommentsByColumnsComparer Instance { get { return instance; } }
		public int Compare(Comment x, Comment y) {
			int result = Comparer<int>.Default.Compare(x.Reference.Column, y.Reference.Column);
			if (result == 0)
				return Comparer<int>.Default.Compare(x.Reference.Row, y.Reference.Row);
			else
				return result;
		}
	}
	#endregion
	#region SearchPredicate<T> (abstract class)
	public abstract class SearchPredicate<T> {
		public string Text { get; set; }
		public CultureInfo Culture { get; set; }
		public bool MatchCase { get; set; }
		public bool MatchEntireCellContents { get; set; }
		protected bool Calculate(string where) {
			if (MatchEntireCellContents)
				return Culture.CompareInfo.Compare(where, Text, MatchCase ? CompareOptions.None : CompareOptions.IgnoreCase) == 0;
			else
				return Culture.CompareInfo.IndexOf(where, Text, MatchCase ? CompareOptions.None : CompareOptions.IgnoreCase) >= 0;
		}
		public abstract bool Calculate(T obj);
	}
	#endregion
	#region CellSearchInValuesAndFormulasPredicate
	public class CellSearchInValuesAndFormulasPredicate : SearchPredicate<ICell> {
		public override bool Calculate(ICell obj) {
			if (obj.HasFormula) {
				if (Calculate(obj.FormulaBody))
					return true;
			}
			return Calculate(obj.Text);
		}
	}
	#endregion
	#region CellSearchInValuesOrFormulasPredicate
	public class CellSearchInValuesOrFormulasPredicate : SearchPredicate<ICell> {
		public override bool Calculate(ICell obj) {
			if (obj.HasFormula)
				return Calculate(obj.FormulaBody);
			return Calculate(obj.Text);
		}
	}
	#endregion
	#region CellSearchInValuesPredicate
	public class CellSearchInValuesPredicate : SearchPredicate<ICell> {
		public override bool Calculate(ICell obj) {
			return Calculate(obj.Text);
		}
	}
	#endregion
	#region SearchCommentsPredicate
	public class SearchCommentsPredicate : SearchPredicate<Comment> {
		public override bool Calculate(Comment obj) {
			return Calculate(obj.GetPlainText());
		}
	}
	#endregion
	#region EntireWorkbookEnumerator<T> (abstract class)
	public delegate IEnumerator<T> CreateSheetEnumeratorAction<T>(Worksheet sheet);
	public class EntireWorkbookEnumerator<T> : IEnumerator<T> {
		readonly DocumentModel documentModel;
		readonly CreateSheetEnumeratorAction<T> createSheetEnumerator;
		IEnumerator<T> sheetEnumerator;
		T current;
		int sheetIndex;
		public EntireWorkbookEnumerator(DocumentModel documentModel, CreateSheetEnumeratorAction<T> createSheetEnumerator) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(createSheetEnumerator, "createSheetEnumerator");
			this.documentModel = documentModel;
			this.createSheetEnumerator = createSheetEnumerator;
		}
		T IEnumerator<T>.Current { get { return current; } }
		void IDisposable.Dispose() {
			sheetEnumerator.Dispose();
		}
		object IEnumerator.Current { get { return current; } }
		bool IEnumerator.MoveNext() {
			if (sheetIndex >= documentModel.Sheets.Count)
				return false;
			for (; ; ) {
				if (sheetEnumerator == null)
					sheetEnumerator = createSheetEnumerator(documentModel.Sheets[sheetIndex]);
				if (sheetEnumerator.MoveNext()) {
					this.current = sheetEnumerator.Current;
					return true;
				}
				sheetIndex++;
				if (sheetIndex >= documentModel.Sheets.Count)
					return false;
				sheetEnumerator = null; 
			}
		}
		void IEnumerator.Reset() {
			sheetEnumerator = null;
			current = default(T);
			sheetIndex = 0;
		}
	}
	#endregion
	#region FilteredEnumerator<T>
	public class FilteredEnumerator<T> : IEnumerator<T> {
		readonly IEnumerator<T> enumerator;
		readonly Predicate<T> predicate;
		T current;
		public FilteredEnumerator(IEnumerator<T> enumerator, Predicate<T> predicate) {
			Guard.ArgumentNotNull(enumerator, "enumerator");
			Guard.ArgumentNotNull(predicate, "predicate");
			this.enumerator = enumerator;
			this.predicate = predicate;
		}
		T IEnumerator<T>.Current { get { return current; } }
		void IDisposable.Dispose() {
			enumerator.Dispose();
		}
		object IEnumerator.Current { get { return current; } }
		bool IEnumerator.MoveNext() {
			while (enumerator.MoveNext()) {
				if (predicate(enumerator.Current)) {
					current = enumerator.Current;
					return true;
				}
			}
			return false;
		}
		void IEnumerator.Reset() {
			enumerator.Reset();
			current = default(T);
		}
	}
	#endregion
}
