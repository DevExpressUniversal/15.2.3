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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheType
	public enum PivotCacheType {
		Consolidation,
		External,
		Scenario,
		Worksheet,
	}
	#endregion
	#region IPivotCacheSource
	public interface IPivotCacheSource {
		PivotCacheType Type { get; }
		IModelErrorInfo CheckSourceBeforeRefresh(WorkbookDataContext context);
		IPivotCacheRefreshResponse GetRefreshedData(WorkbookDataContext context);
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
		void UpdateExpression(ReplaceThingsPRNVisitor walker, DocumentModel documentModel);
		void OnTableConvertedToRange(string tableName, DocumentModel documentModel);
		string GetExpressionString(WorkbookDataContext сontext);
	}
	#endregion
	#region PivotCacheSourceWorksheet
	public class PivotCacheSourceWorksheet : IPivotCacheSource {
		#region Static
		public static PivotCacheSourceWorksheet CreateInstance(string reference, WorkbookDataContext context) {
			ParsedExpression expression = context.ParseExpression(reference, OperandDataType.Reference, false);
			return CreateInstance(expression, context);
		}
		public static PivotCacheSourceWorksheet CreateInstance(ParsedExpression expression, WorkbookDataContext context) {
			PivotCacheSourceWorksheetExpressionCorrector corrector = new PivotCacheSourceWorksheetExpressionCorrector(context);
			expression = corrector.Process(expression);
			if (expression == null)
				return null;
			return CreateInstanceCore(expression);
		}
		public static PivotCacheSourceWorksheet CreateInstanceCore(ParsedExpression expression) {
			return new PivotCacheSourceWorksheet(expression);
		}
		#endregion
		ParsedExpression expression;
		PivotCacheSourceWorksheet(ParsedExpression expression) {
			SetSourceCore(expression);
		}
		public ParsedExpression Expression { get { return expression; } }
		public PivotCacheType Type { get { return PivotCacheType.Worksheet; } }
		#region CheckSourceBeforeRefresh
		public IModelErrorInfo CheckSourceBeforeRefresh(WorkbookDataContext context) {
			return CheckSourceBeforeRefreshCore(context, false);
		}
		internal IModelErrorInfo CheckSourceBeforeRefreshCore(WorkbookDataContext context, bool isConsolidation) {
			if (!isConsolidation)
				if (expression.Count == 1) {
					IParsedThing ptg = expression[0];
					if (!(ptg is ParsedThingTableExt)) { 
						ParsedThingTable tablePtg = ptg as ParsedThingTable;
						if (tablePtg != null) {
							Table table = context.Workbook.GetTableByName(tablePtg.TableName);
							return table == null ? new ModelErrorInfo(ModelErrorType.InvalidReference) : null;
						}
					}
				}
			CellRange sourceRange = GetRange(context);
			if (sourceRange == null)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			if (sourceRange.Height < 2)
				return new ModelErrorInfo(ModelErrorType.ErrorCommandRequiresAtLeastTwoRows);
			int startColumn = 0;
			if (isConsolidation) {
				if (sourceRange.Width < 2)
					return new ModelErrorInfo(ModelErrorType.ErrorCommandRequiresAtLeastTwoRows); 
				int height = sourceRange.Height;
				for (int i = 1; i < height; ++i)
					if (IsEmptyCell(0, i, sourceRange, context))
						return new ModelErrorInfo(ModelErrorType.PivotTableFieldNameIsInvalid);
				startColumn = 1;
			}
			int width = sourceRange.Width;
			for (int i = startColumn; i < width; ++i)
				if (IsEmptyCell(i, 0, sourceRange, context))
					return new ModelErrorInfo(ModelErrorType.PivotTableFieldNameIsInvalid);
			return null;
		}
		bool IsEmptyCell(int relativeColumn, int relativeRow, CellRange range, WorkbookDataContext context) {
			ICellBase cell = range.TryGetCellRelative(relativeColumn, relativeRow);
			return cell == null || cell.Value.IsEmpty || cell.Value.IsText && string.IsNullOrEmpty(cell.Value.GetTextValue(context.Workbook.SharedStringTable));
		}
		#endregion
		#region GetRefreshedData
		public IPivotCacheRefreshResponse GetRefreshedData(WorkbookDataContext context) {
			if (expression.Count == 1) {
				IParsedThing ptg = expression[0];
				if (!(ptg is ParsedThingTableExt)) { 
					ParsedThingTable tablePtg = ptg as ParsedThingTable;
					if (tablePtg != null) {
						int startColumn, endColumn;
						Table table = GetTableInfo(context, tablePtg, out startColumn, out endColumn);
						return new PivotCacheSourceWorksheetRefreshResponseTable(table, startColumn, endColumn);
					}
				}
			}
			CellRange range = GetRange(context);
			return new PivotCacheSourceWorksheetRefreshResponseRange(range);
		}
		Table GetTableInfo(WorkbookDataContext context, ParsedThingTable tablePtg, out int startColumn, out int endColumn) {
			Table table = context.Workbook.GetTableByName(tablePtg.TableName);
			startColumn = 0;
			endColumn = table.Columns.Count - 1;
			if (!string.IsNullOrEmpty(tablePtg.ColumnStart)) {
				startColumn = table.Columns[tablePtg.ColumnStart].FindColumnIndex();
				endColumn = startColumn;
			}
			if (!string.IsNullOrEmpty(tablePtg.ColumnEnd))
				endColumn = table.Columns[tablePtg.ColumnEnd].FindColumnIndex();
			return table;
		}
		internal CellRange GetRange(WorkbookDataContext context) { 
			CellRange range = GetRangeCore(context);
			if (range == null)
				return null;
			if (range.Worksheet as Worksheet == null) 
				return null;
			return range;
		}
		internal CellRange GetRangeCore(WorkbookDataContext context) { 
			context.PushCurrentWorksheet(context.Workbook.ActiveSheet);
			context.PushCurrentCell(0, 0);
			try {
				VariantValue rangeValue = expression.Evaluate(context);
				if (rangeValue.IsCellRange) {
					CellRangeBase range = rangeValue.CellRangeValue;
					if (range != null && range.RangeType != CellRangeType.UnionRange)
						return range.GetFirstInnerCellRange();
				}
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
			return null;
		}
		#endregion
		#region Notification
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			UpdateExpression(context.Visitor, context.Range.Worksheet.Workbook.DataContext.Workbook);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			WorkbookDataContext dataContext = context.Range.Worksheet.Workbook.DataContext;
			ReplaceThingsPRNVisitor visitor;
			if (context.Mode == RemoveCellMode.ShiftCellsLeft)
				visitor = new PivotCacheSourceWorksheetRemoveSheftLeftVisitor(context, dataContext);
			else if (context.Mode == RemoveCellMode.ShiftCellsUp)
				visitor = new PivotCacheSourceWorksheetRemoveSheftUpVisitor(context, dataContext);
			else
				visitor = new PivotCacheSourceWorksheetRemoveDefaultVisitor(context, dataContext);
			UpdateExpression(visitor, dataContext.Workbook);
		}
		public void UpdateExpression(ReplaceThingsPRNVisitor walker, DocumentModel documentModel) {
			if (Expression == null)
				return;
			ParsedExpression updatedExpression = walker.Process(Expression.Clone());
			if (walker.FormulaChanged)
				HistoryHelper.SetValue(documentModel, this.expression, updatedExpression, SetSourceCore);
		}
		public void OnTableConvertedToRange(string tableName, DocumentModel documentModel) {
			if (Expression.Count != 1)
				return;
			IParsedThing ptg = Expression[0];
			if (ptg is ParsedThingTableExt)
				return;
			ParsedThingTable tablePtg = ptg as ParsedThingTable;
			if (tablePtg == null)
				return;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(tableName, tablePtg.TableName) == 0) {
				int startColumn, endColumn;
				Table table = GetTableInfo(documentModel.DataContext, tablePtg, out startColumn, out endColumn);
				if (!table.HasHeadersRow)
					return;
				CellRange range = table.Range.GetSubColumnRange(startColumn, endColumn);
				SheetDefinition sheetDefinition = new SheetDefinition(table.Worksheet.Name);
				int sheetDefinitionIndex = documentModel.DataContext.RegisterSheetDefinition(sheetDefinition);
				ParsedThingArea3d area = new ParsedThingArea3d(range, sheetDefinitionIndex, OperandDataType.Reference);
				ParsedExpression newExpression = new ParsedExpression();
				newExpression.Add(area);
				HistoryHelper.SetValue(documentModel, this.expression, newExpression, SetSourceCore);
			}
		}
		#endregion
		internal void SetSourceCore(ParsedExpression expression) {
			Guard.ArgumentNotNull(expression, "expression");
			this.expression = expression;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)Type, expression.GetHashCode());
		}
		public override bool Equals(object obj) {
			PivotCacheSourceWorksheet source = obj as PivotCacheSourceWorksheet;
			if (source == null)
				return false;
			return Expression.Equals(source.Expression);
		}
		#region IPivotCacheSource Members
		public string GetExpressionString(WorkbookDataContext context) {
			context.PushCurrentWorksheet(context.Workbook.ActiveSheet);
			context.PushCurrentCell(0, 0);
			try {
				return Expression.BuildExpressionString(context);
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		#endregion
	}
	#endregion
	#region PivotCacheSourceWorksheetExpressionCorrector
	public class PivotCacheSourceWorksheetExpressionCorrector : ParsedThingVisitor {
		readonly WorkbookDataContext context;
		ParsedExpression expression;
		bool validExpression;
		int index;
		public PivotCacheSourceWorksheetExpressionCorrector(WorkbookDataContext context) {
			this.context = context;
		}
		public override ParsedExpression Process(ParsedExpression expression) {
			if (expression == null)
				return null;
			this.expression = expression;
			this.validExpression = true;
			for (index = 0; index < expression.Count; ++index)
				expression[index].Visit(this);
			return validExpression ? expression : null;
		}
		public override void Visit(ParsedThingArea thing) {
			IWorksheet sheet = context.CurrentWorksheet;
			if (sheet == null) {
				validExpression = false;
				return;
			}
			SheetDefinition sheetDefinition = new SheetDefinition(sheet.Name);
			int sheetDefinitionIndex = context.RegisterSheetDefinition(sheetDefinition);
			CellRange range = new CellRange(thing.CellRange.Worksheet, thing.CellRange.TopLeft.AsAbsolute(), thing.CellRange.BottomRight.AsAbsolute());
			expression[index] = new ParsedThingArea3d(range, sheetDefinitionIndex, OperandDataType.Reference);
		}
		public override void Visit(ParsedThingArea3d thing) {
			CellRange range = new CellRange(thing.CellRange.Worksheet, thing.CellRange.TopLeft.AsAbsolute(), thing.CellRange.BottomRight.AsAbsolute());
			thing.CellRange = range;
		}
		public override void Visit(ParsedThingTable thing) {
			if (expression.Count == 1)
				thing.IncludedRows = TableRowEnum.NotDefined;
		}
	}
	#endregion
	#region PivotCacheSourceWorksheetRemoveRangeVisitors
	static class PivotCacheSourceWorksheetRemoveRangeVisitorHelper {
		public static bool ShouldProcess(ParsedThingArea3d thing, CellRange removableRange, WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			IWorksheet sheet = context.Workbook.GetSheetByName(sheetDefinition.SheetNameStart);
			thing.CellRange = new CellRange(sheet, thing.TopLeft, thing.BottomRight); 
			return !removableRange.ContainsRange(thing.CellRange);
		}
		public static bool ShouldProcess(ParsedThingTable thing, CellRange removableRange, WorkbookDataContext context) {
			Table modelTable = context.Workbook.GetTableByName(thing.TableName);
			return modelTable != null;
		}
		public static bool ShouldProcess(ParsedThingTableExt thing) {
			return false;
		}
	}
	public class PivotCacheSourceWorksheetRemoveSheftLeftVisitor : RemovedShiftLeftRPNVisitor {
		public PivotCacheSourceWorksheetRemoveSheftLeftVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTable thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing))
				base.Visit(thing);
		}
	}
	public class PivotCacheSourceWorksheetRemoveSheftUpVisitor : RemovedShiftUpRPNVisitor {
		public PivotCacheSourceWorksheetRemoveSheftUpVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTable thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing))
				base.Visit(thing);
		}
	}
	public class PivotCacheSourceWorksheetRemoveDefaultVisitor : RemovedDefaultRPNVisitor {
		public PivotCacheSourceWorksheetRemoveDefaultVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTable thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing, NotificationContext.Range, DataContext))
				base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			if (PivotCacheSourceWorksheetRemoveRangeVisitorHelper.ShouldProcess(thing))
				base.Visit(thing);
		}
	}
	#endregion
	#region PivotCacheSourceWorksheetHelper
	public class PivotCacheSourceWorksheetHelper : ParsedThingVisitor {
		public static PivotCacheSourceWorksheet GetSource(string sheetName, string namedRange, string reference, string bookFileName, WorkbookDataContext context) {
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(bookFileName)) {
				sb.Append("'[");
				sb.Append(bookFileName);
				sb.Append("]");
			}
			if (!string.IsNullOrEmpty(sheetName)) {
				if (sb.Length == 0)
					sb.Append("'");
				sb.Append(sheetName);
			}
			if (sb.Length > 0)
				sb.Append("'!");
			if (!string.IsNullOrEmpty(reference)) {
				if (sb.Length == 0)
					return null;
				sb.Append(reference);
			}
			if (!string.IsNullOrEmpty(namedRange))
				sb.Append(namedRange);
			return PivotCacheSourceWorksheet.CreateInstance(sb.ToString(), context);
		}
		bool processed;
		WorkbookDataContext context;
		public void GenerateValues(PivotCacheSourceWorksheet source, WorkbookDataContext context) {
			this.context = context;
			if (source.Expression.Count == 1) {
				Range = source.GetRangeCore(context);
				if (Range != null) { 
					source.Expression[0].Visit(this);
					if (processed)
						return;
				}
			}
			context.PushImportExportMode(false); 
			try {
				Name = source.Expression.BuildExpressionString(context);
			}
			finally {
				context.PopImportExportMode();
			}
		}
		public string Book { get; private set; }
		public string Sheet { get; private set; }
		public string Reference { get; private set; }
		public string Name { get; private set; }
		public CellRange Range { get; private set; }
		public void ProcessArea(ParsedThingArea thing) {
			processed = true;
			Range = thing.CellRange;
			Reference = thing.CellRange.ToString(false);
		}
		public void ProcessName(ParsedThingName thing) {
			processed = true;
			Name = thing.DefinedName;
		}
		public void ProcessTable(ParsedThingTable thing) {
			processed = true;
			StringBuilder sb = new StringBuilder();
			thing.BuildExpressionStringCore(sb, context);
			Name = sb.ToString().Replace("[]", string.Empty);
		}
		public void ProcessSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			Sheet = sheetDefinition.SheetNameStart;
			if (sheetDefinition.IsExternalReference) {
				ExternalWorkbook book = sheetDefinition.GetExternalBook(context);
				Book = book.FilePath;
			}
		}
		public override void Visit(ParsedThingArea3d thing) {
			ProcessSheetDefinition(thing.SheetDefinitionIndex);
			ProcessArea(thing);
		}
		public override void Visit(ParsedThingName thing) {
			ProcessName(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			ProcessSheetDefinition(thing.SheetDefinitionIndex);
			ProcessName(thing);
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTable(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			ProcessSheetDefinition(thing.SheetDefinitionIndex);
			ProcessTable(thing);
		}
	}
	#endregion
	#region PivotCacheSourceConsolidation Page & RangeSet
	public class PivotCacheSourceConsolidationPage {
		readonly List<string> itemNames = new List<string>(); 
		public List<string> ItemNames { get { return itemNames; } }
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(itemNames);
		}
		public override bool Equals(object obj) {
			PivotCacheSourceConsolidationPage page = obj as PivotCacheSourceConsolidationPage;
			if (page == null)
				return false;
			if (ItemNames.Count != page.ItemNames.Count)
				return false;
			for (int i = 0; i < ItemNames.Count; ++i)
				if (itemNames[i] != page.ItemNames[i])
					return false;
			return true;
		}
	}
	public class PivotCacheSourceConsolidationRangeSet {
		int pageFieldItemIndex1;
		int pageFieldItemIndex2;
		int pageFieldItemIndex3;
		int pageFieldItemIndex4;
		PivotCacheSourceWorksheet sourceReference;
		public PivotCacheSourceConsolidationRangeSet(PivotCacheSourceWorksheet sourceReference, int index1, int index2, int index3, int index4) {
			Guard.ArgumentNotNull(sourceReference, "sourceReference");
			this.sourceReference = sourceReference;
			this.pageFieldItemIndex1 = index1;
			this.pageFieldItemIndex2 = index2;
			this.pageFieldItemIndex3 = index3;
			this.pageFieldItemIndex4 = index4;
		}
		public PivotCacheSourceWorksheet SourceReference { get { return sourceReference; } }
		public int PageFieldItemIndex1 { get { return pageFieldItemIndex1; } }
		public int PageFieldItemIndex2 { get { return pageFieldItemIndex2; } }
		public int PageFieldItemIndex3 { get { return pageFieldItemIndex3; } }
		public int PageFieldItemIndex4 { get { return pageFieldItemIndex4; } }
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(PageFieldItemIndex1, PageFieldItemIndex2, PageFieldItemIndex3, PageFieldItemIndex4, SourceReference.GetHashCode());
		}
		public override bool Equals(object obj) {
			PivotCacheSourceConsolidationRangeSet rangeSet = obj as PivotCacheSourceConsolidationRangeSet;
			if (rangeSet == null)
				return false;
			if (PageFieldItemIndex1 != rangeSet.PageFieldItemIndex1 || PageFieldItemIndex2 != rangeSet.PageFieldItemIndex2 ||
				PageFieldItemIndex3 != rangeSet.PageFieldItemIndex3 || PageFieldItemIndex4 != rangeSet.PageFieldItemIndex4)
				return false;
			return SourceReference.Equals(rangeSet.SourceReference);
		}
	}
	#endregion
	#region PivotCacheSourceConsolidation
	public class PivotCacheSourceConsolidation : IPivotCacheSource {
		readonly List<PivotCacheSourceConsolidationPage> pages = new List<PivotCacheSourceConsolidationPage>();
		readonly List<PivotCacheSourceConsolidationRangeSet> rangeSets = new List<PivotCacheSourceConsolidationRangeSet>();
		bool autoPage; 
		#region Properties
		public PivotCacheType Type { get { return PivotCacheType.Consolidation; } }
		public bool AutoPage { get { return autoPage; } set { autoPage = value; } }
		public List<PivotCacheSourceConsolidationPage> Pages { get { return pages; } }
		public List<PivotCacheSourceConsolidationRangeSet> RangeSets { get { return rangeSets; } }
		#endregion
		public IModelErrorInfo CheckSourceBeforeRefresh(WorkbookDataContext context) {
			if (RangeSets.Count == 0)
				return new ModelErrorInfo(ModelErrorType.InvalidReference);
			for (int j = 0; j < RangeSets.Count; ++j) {
				PivotCacheSourceWorksheet source = RangeSets[j].SourceReference;
				IModelErrorInfo error = source.CheckSourceBeforeRefreshCore(context, true);
				if (error != null)
					return error;
			}
			return null;
		}
		public IPivotCacheRefreshResponse GetRefreshedData(WorkbookDataContext context) {
			List<CellRange> ranges = new List<CellRange>(RangeSets.Count);
			for (int i = 0; i < RangeSets.Count; ++i)
				ranges.Add(RangeSets[i].SourceReference.GetRange(context));
			return new PivotCacheSourceConsolidationRefreshResponse(ranges, this);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = 0; i < RangeSets.Count; ++i)
				RangeSets[i].SourceReference.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = 0; i < RangeSets.Count; ++i)
				RangeSets[i].SourceReference.OnRangeRemoving(context);
		}
		public void UpdateExpression(ReplaceThingsPRNVisitor walker, DocumentModel documentModel) {
			for (int i = 0; i < RangeSets.Count; ++i)
				RangeSets[i].SourceReference.UpdateExpression(walker, documentModel);
		}
		public void OnTableConvertedToRange(string tableName, DocumentModel documentModel) {
			for (int i = 0; i < RangeSets.Count; ++i)
				RangeSets[i].SourceReference.OnTableConvertedToRange(tableName, documentModel);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(Type.GetHashCode(), AutoPage.GetHashCode(), Pages.GetHashCode(), RangeSets.GetHashCode());
		}
		public override bool Equals(object obj) {
			PivotCacheSourceConsolidation source = obj as PivotCacheSourceConsolidation;
			if (source == null)
				return false;
			if (AutoPage != source.AutoPage || Pages.Count != source.Pages.Count || RangeSets.Count != source.RangeSets.Count)
				return false;
			for (int i = 0; i < Pages.Count; ++i)
				if (!Pages[i].Equals(source.Pages[i]))
					return false;
			for (int i = 0; i < RangeSets.Count; ++i)
				if (!RangeSets[i].Equals(source.RangeSets[i]))
					return false;
			return true;
		}
		#region IPivotCacheSource Members
		public string GetExpressionString(WorkbookDataContext сontext) {
			return "Invalid reference";
		}
		#endregion
	}
	#endregion
	#region PivotCacheSourceExternal
	public class PivotCacheSourceExternal : IPivotCacheSource {
		public int ConnectionId { get; set; }
		public PivotCacheType Type { get { return PivotCacheType.External; } }
		public IPivotCacheRefreshResponse GetRefreshedData(WorkbookDataContext context) {
			throw new ArgumentException("not implemented");
		}
		public IModelErrorInfo CheckSourceBeforeRefresh(WorkbookDataContext context) {
			return new ModelErrorInfo(ModelErrorType.PivotCacheSourceTypeIsInvalid);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) { }
		public void OnRangeRemoving(RemoveRangeNotificationContext context) { }
		public void UpdateExpression(ReplaceThingsPRNVisitor walker, DocumentModel documentModel) { }
		public void OnTableConvertedToRange(string tableName, DocumentModel documentModel) { }
		public override int GetHashCode() {
			return ConnectionId;
		}
		public override bool Equals(object obj) {
			PivotCacheSourceExternal source = obj as PivotCacheSourceExternal;
			if (source == null)
				return false;
			return ConnectionId == source.ConnectionId;
		}
		#region IPivotCacheSource Members
		public string GetExpressionString(WorkbookDataContext сontext) {
			return "Invalid reference";
		}
		#endregion
	}
	#endregion
	#region PivotCacheSourceScenario
	public class PivotCacheSourceScenario : IPivotCacheSource {
		public PivotCacheType Type { get { return PivotCacheType.Scenario; } }
		public IPivotCacheRefreshResponse GetRefreshedData(WorkbookDataContext context) {
			throw new ArgumentException("not implemented");
		}
		public IModelErrorInfo CheckSourceBeforeRefresh(WorkbookDataContext context) {
			return new ModelErrorInfo(ModelErrorType.PivotCacheSourceTypeIsInvalid);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) { }
		public void OnRangeRemoving(RemoveRangeNotificationContext context) { }
		public void UpdateExpression(ReplaceThingsPRNVisitor walker, DocumentModel documentModel) { }
		public void OnTableConvertedToRange(string tableName, DocumentModel documentModel) { }
		public override int GetHashCode() {
			return 0;
		}
		public override bool Equals(object obj) {
			PivotCacheSourceScenario source = obj as PivotCacheSourceScenario;
			return source != null;
		}
		#region IPivotCacheSource Members
		public string GetExpressionString(WorkbookDataContext сontext) {
			return "Invalid reference";
		}
		#endregion
	}
	#endregion
	#region IPivotCacheRefreshResponse
	public interface IPivotCacheRefreshResponse : IDisposable {
		IEnumerable<IPivotCacheRecord> GetRecordsEnumerable();
		IEnumerable<IResponseField> GetFieldsEnumerable();
		bool AllowsMultiThreadedAccess { get; }
		List<PivotTableRefreshDataBucket> GetDataBuckets();
	}
	#endregion
	#region PivotCacheSourceWorksheetRefreshResponse
	public abstract class PivotCacheSourceWorksheetRefreshResponse<T> : IPivotCacheRefreshResponse {
		readonly T source;
		protected PivotCacheSourceWorksheetRefreshResponse(T source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
		}
		protected T Source { get { return source; } }
		public bool AllowsMultiThreadedAccess { get { return true; } }
		public List<PivotTableRefreshDataBucket> GetDataBuckets() {
			CellRange recordsRange = GetRecordsRange();
			int maxRowsPerBucket = 1024;
			int height = recordsRange.Height;
			int currentRow = 0;
			int topRowIndex = recordsRange.TopRowIndex;
			int leftColumnIndex = recordsRange.LeftColumnIndex;
			int rightColumnIndex = recordsRange.RightColumnIndex;
			List<PivotTableRefreshDataBucket> result = new List<PivotTableRefreshDataBucket>(height / maxRowsPerBucket + 1);
			while (currentRow < height) {
				CellPosition topLeft = new CellPosition(leftColumnIndex, topRowIndex + currentRow);
				currentRow = Math.Min(currentRow + maxRowsPerBucket, height);
				CellPosition bottomRight = new CellPosition(rightColumnIndex, topRowIndex + currentRow - 1);
				CellRange bucketRange = new CellRange(recordsRange.Worksheet, topLeft, bottomRight);
				PivotTableRefreshDataBucket bucket = new PivotTableRefreshDataBucket(bucketRange);
				result.Add(bucket);
			}
			return result;
		}
		public IEnumerable<IPivotCacheRecord> GetRecordsEnumerable() {
			CellRange recordsRange = GetRecordsRange();
			ICellTable worksheet = recordsRange.Worksheet;
			int lastRow = Math.Min(worksheet.Rows.LastRowIndex + 1, recordsRange.BottomRowIndex);
			int leftColumn = recordsRange.LeftColumnIndex;
			int rightColumn = recordsRange.RightColumnIndex;
			int cacheFieldsCount = recordsRange.Width;
			foreach (IRow row in worksheet.Rows.GetExistingRows(recordsRange.TopRowIndex, lastRow, false)) {
				yield return CreateRecord(row, leftColumn, rightColumn, cacheFieldsCount);
			}
		}
		IPivotCacheRecord CreateRecord(IRow row, int leftColumn, int rightColumn, int cacheFieldsCount) {
			IPivotCacheRecordValue[] values = new IPivotCacheRecordValue[cacheFieldsCount];
			int lastColumnIndex = leftColumn;
			foreach (ICell cell in row.Cells.GetExistingCells(leftColumn, rightColumn, false)) {
				int cellColumnIndex = cell.ColumnIndex;
				for (int i = lastColumnIndex; i < cellColumnIndex; i++) { 
					values[i] = PivotCacheRecordOrdinalEmptyValue.Instance;
				}
				values[cellColumnIndex - leftColumn] = GetRecordValue(cell);
				lastColumnIndex = cellColumnIndex + 1;
			}
			for (int i = lastColumnIndex; i <= rightColumn; i++)
				values[i] = PivotCacheRecordOrdinalEmptyValue.Instance;
			return new PivotCacheRecord(values);
		}
		protected IPivotCacheRecordValue GetRecordValue(ICell cell) {
			if (cell == null)
				return PivotCacheRecordOrdinalEmptyValue.Instance;
			VariantValue value = cell.Value;
			switch (value.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return PivotCacheRecordOrdinalEmptyValue.Instance;
				case VariantValueType.Boolean:
					return new PivotCacheRecordOrdinalBooleanValue(value.BooleanValue);
				case VariantValueType.Error:
					return new PivotCacheRecordOrdinalErrorValue(value.ErrorValue);
				case VariantValueType.Numeric:
					if (cell.Format.IsDateTime)
						return new PivotCacheRecordOrdinalDateTimeValue(value.ToDateTime(cell.Context));
					return new PivotCacheRecordOrdinalNumericValue(value.NumericValue);
				case VariantValueType.SharedString:
				case VariantValueType.InlineText:
					return new PivotCacheRecordOrdinalCharacterValue(value.GetTextValue(cell.Worksheet.Workbook.SharedStringTable));
				case VariantValueType.Array:
				case VariantValueType.CellRange:
					throw new InvalidOperationException();
				default:
					throw new ArgumentException("Unknown VariantValueType" + value.Type);
			}
		}
		protected abstract CellRange GetRecordsRange();
		public abstract IEnumerable<IResponseField> GetFieldsEnumerable();
		public void Dispose() {
		}
	}
	#endregion
	#region PivotCacheSourceWorksheetRefreshResponseRange
	public class PivotCacheSourceWorksheetRefreshResponseRange : PivotCacheSourceWorksheetRefreshResponse<CellRange> {
		public PivotCacheSourceWorksheetRefreshResponseRange(CellRange range)
			: base(range) {
		}
		protected override CellRange GetRecordsRange() {
			return Source.GetSubRowRange(1, Source.Height - 1);
		}
		public override IEnumerable<IResponseField> GetFieldsEnumerable() {
			CellRange headersRow = Source.GetSubRowRange(0, 0);
			foreach (VariantValue headerValue in (new Enumerable<VariantValue>(headersRow.GetValuesEnumerator()))) {
				string name = VariantValueToText.ToString(headerValue, headersRow.Worksheet.Workbook.DataContext);
				yield return new ResponseField(name);
			}
		}
	}
	#endregion
	#region PivotCacheSourceWorksheetRefreshResponseTable
	public class PivotCacheSourceWorksheetRefreshResponseTable : PivotCacheSourceWorksheetRefreshResponse<Table> {
		readonly int startColumn;
		readonly int endColumn;
		public PivotCacheSourceWorksheetRefreshResponseTable(Table table, int startColumn, int endColumn)
			: base(table) {
			this.startColumn = startColumn;
			this.endColumn = endColumn;
		}
		protected override CellRange GetRecordsRange() {
			CellRange recordsRange = Source.GetDataRange();
			return recordsRange.GetSubColumnRange(startColumn, endColumn);
		}
		public override IEnumerable<IResponseField> GetFieldsEnumerable() {
			for (int i = startColumn; i <= endColumn; ++i) {
				string name = VariantValueToText.ToString(Source.Columns[i].Name, Source.Worksheet.DataContext);
				yield return new ResponseField(name);
			}
		}
	}
	#endregion
	#region PivotCacheSourceConsolidationRefreshResponse
	public class PivotCacheSourceConsolidationRefreshResponse : IPivotCacheRefreshResponse {
		readonly PivotCacheSourceConsolidation source;
		readonly List<CellRange> ranges;
		public PivotCacheSourceConsolidationRefreshResponse(List<CellRange> ranges, PivotCacheSourceConsolidation source) {
			Guard.ArgumentNotNull(ranges, "ranges");
			this.ranges = ranges;
			this.source = source;
		}
		public bool AllowsMultiThreadedAccess { get { return false; } }
		public IEnumerable<IPivotCacheRecord> GetRecordsEnumerable() {
			for (int i = 0; i < ranges.Count; ++i) {
				PivotCacheSourceConsolidationRangeSet rangeSet = source.RangeSets[i];
				CellRange range = ranges[i];
				int lastRow = Math.Min(range.Worksheet.Rows.LastRowIndex + 1, range.BottomRowIndex);
				int rightColumn = range.RightColumnIndex;
				for (int j = range.TopRowIndex + 1; j <= lastRow; ++j)
					for (int k = range.LeftColumnIndex + 1; k <= rightColumn; ++k)
						yield return CreateRecord(range, k, j, source.Pages, rangeSet);
			}
		}
		IPivotCacheRecord CreateRecord(CellRange range, int valueColumn, int valueRow, List<PivotCacheSourceConsolidationPage> pages, PivotCacheSourceConsolidationRangeSet rangeSet) {
			int pagesCount = pages.Count;
			int fieldsCount = 3 + Math.Min(pages.Count, 4);
			IPivotCacheRecordValue[] values = new IPivotCacheRecordValue[fieldsCount];
			Worksheet sheet = range.Worksheet as Worksheet;
			values[0] = GetRecordValue(sheet, range.LeftColumnIndex, valueRow); 
			values[1] = GetRecordValue(sheet, valueColumn, range.TopRowIndex); 
			values[2] = GetRecordValue(sheet, valueColumn, valueRow); 
			if (pagesCount > 0)
				values[3] = GetPageValue(pages, 0, rangeSet.PageFieldItemIndex1); 
			if (pagesCount > 1)
				values[4] = GetPageValue(pages, 1, rangeSet.PageFieldItemIndex2); 
			if (pagesCount > 2)
				values[5] = GetPageValue(pages, 2, rangeSet.PageFieldItemIndex3); 
			if (pagesCount > 3)
				values[6] = GetPageValue(pages, 3, rangeSet.PageFieldItemIndex4); 
			return new PivotCacheRecord(values);
		}
		protected IPivotCacheRecordValue GetRecordValue(Worksheet sheet, int columnIndex, int rowIndex) {
			ICell cell = sheet.TryGetCell(columnIndex, rowIndex);
			if (cell == null)
				return PivotCacheRecordOrdinalEmptyValue.Instance;
			VariantValue value = cell.Value;
			switch (value.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return PivotCacheRecordOrdinalEmptyValue.Instance;
				case VariantValueType.Boolean:
					return new PivotCacheRecordOrdinalBooleanValue(value.BooleanValue);
				case VariantValueType.Error:
					return new PivotCacheRecordOrdinalErrorValue(value.ErrorValue);
				case VariantValueType.Numeric:
					if (cell.Format.IsDateTime)
						return new PivotCacheRecordOrdinalDateTimeValue(value.ToDateTime(cell.Context));
					return new PivotCacheRecordOrdinalNumericValue(value.NumericValue);
				case VariantValueType.SharedString:
				case VariantValueType.InlineText:
					return new PivotCacheRecordOrdinalCharacterValue(value.GetTextValue(cell.Worksheet.Workbook.SharedStringTable));
				case VariantValueType.Array:
				case VariantValueType.CellRange:
					throw new InvalidOperationException();
				default:
					throw new ArgumentException("Unknown VariantValueType" + value.Type);
			}
		}
		IPivotCacheRecordValue GetPageValue(List<PivotCacheSourceConsolidationPage> pages, int pageIndex, int itemIndex) {
			if (itemIndex < 0)
				return PivotCacheRecordOrdinalEmptyValue.Instance;
			string itemName = pages[pageIndex].ItemNames[itemIndex];
			return new PivotCacheRecordCharacterValue(itemName);
		}
		public List<PivotTableRefreshDataBucket> GetDataBuckets() {
			throw new InvalidOperationException("GetDataBuckets can be called only for multithread supproted data sources");
		}
		public IEnumerable<IResponseField> GetFieldsEnumerable() {
			yield return new ResponseField("Row");
			yield return new ResponseField("Column");
			yield return new ResponseField("Value");
			for (int i = 1; i <= source.Pages.Count; ++i)
				yield return new ResponseField("Page" + i);
		}
		public void Dispose() {
		}
	}
	#endregion
	#region IResponceField
	public interface IResponseField {
		string Name { get; }
	}
	#endregion
	#region ResponceField
	public class ResponseField : IResponseField {
		readonly string name;
		public ResponseField(string name) {
			this.name = name;
		}
		public string Name { get { return name; } }
	}
	#endregion
	#region PivotTableRefreshDataBucket
	public class PivotTableRefreshDataBucket {
		readonly CellRange range;
		public PivotTableRefreshDataBucket(CellRange range) {
			this.range = range;
		}
		public IEnumerable<IPivotCacheRecord> GetRecordsEnumerable(PivotCacheFieldsCollection cacheFields) {
			CellRange recordsRange = range;
			ICellTable worksheet = recordsRange.Worksheet;
			int lastRow = Math.Min(worksheet.Rows.LastRowIndex + 1, recordsRange.BottomRowIndex);
			int leftColumn = recordsRange.LeftColumnIndex;
			int rightColumn = recordsRange.RightColumnIndex;
			int cacheFieldsCount = recordsRange.Width;
			int previousRowIndex = recordsRange.TopRowIndex;
			foreach (IRow row in worksheet.Rows.GetExistingRows(recordsRange.TopRowIndex, lastRow, false)) {
				int currentRowIndex = row.Index;
				for (int i = previousRowIndex; i < currentRowIndex; i++)
					yield return CreateEmptyRecord(cacheFieldsCount, cacheFields);
				yield return CreateRecord(row, leftColumn, rightColumn, cacheFieldsCount, cacheFields);
				previousRowIndex = currentRowIndex + 1;
			}
			for (int i = previousRowIndex; i <= lastRow; i++)
				yield return CreateEmptyRecord(cacheFieldsCount, cacheFields);
		}
		private IPivotCacheRecord CreateEmptyRecord(int cacheFieldsCount, PivotCacheFieldsCollection cacheFields) {
			IPivotCacheRecordValue[] values = new IPivotCacheRecordValue[cacheFieldsCount];
			for (int i = 0; i < cacheFieldsCount; i++)
				values[i] = (PivotCacheRecordOrdinalEmptyValue.Instance).ToSharedItemThreadSafe(cacheFields[i]);
			return new PivotCacheRecord(values);
		}
		IPivotCacheRecord CreateRecord(IRow row, int leftColumn, int rightColumn, int cacheFieldsCount, PivotCacheFieldsCollection cacheFields) {
			IPivotCacheRecordValue[] values = new IPivotCacheRecordValue[cacheFieldsCount];
			int lastColumnIndex = leftColumn;
			foreach (ICell cell in row.Cells.GetExistingCells(leftColumn, rightColumn, false)) {
				int cellColumnIndex = cell.ColumnIndex;
				for (int i = lastColumnIndex - leftColumn; i < cellColumnIndex - leftColumn; i++) 
					values[i] = CreateEmptyValue(cacheFields[i]);
				int fieldIndex = cellColumnIndex - leftColumn;
				values[fieldIndex] = GetRecordValue(cell).ToSharedItemThreadSafe(cacheFields[fieldIndex]);
				lastColumnIndex = cellColumnIndex + 1;
			}
			for (int i = lastColumnIndex - leftColumn; i <= rightColumn - leftColumn; i++)
				values[i] = CreateEmptyValue(cacheFields[i]);
			return new PivotCacheRecord(values);
		}
		private static PivotCacheRecordSharedItemsIndexValue CreateEmptyValue(IPivotCacheField cacheField) {
			return (PivotCacheRecordOrdinalEmptyValue.Instance).ToSharedItemThreadSafe(cacheField);
		}
		protected IPivotCacheRecordValue GetRecordValue(ICell cell) {
			if (cell == null)
				return PivotCacheRecordOrdinalEmptyValue.Instance;
			VariantValue value = cell.Value;
			switch (value.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return PivotCacheRecordOrdinalEmptyValue.Instance;
				case VariantValueType.Boolean:
					return new PivotCacheRecordOrdinalBooleanValue(value.BooleanValue);
				case VariantValueType.Error:
					return new PivotCacheRecordOrdinalErrorValue(value.ErrorValue);
				case VariantValueType.Numeric:
					if (cell.Format.IsDateTime)
						return new PivotCacheRecordOrdinalDateTimeValue(value.ToDateTime(cell.Context));
					return new PivotCacheRecordOrdinalNumericValue(value.NumericValue);
				case VariantValueType.SharedString:
				case VariantValueType.InlineText:
					return new PivotCacheRecordOrdinalCharacterValue(value.GetTextValue(cell.Worksheet.Workbook.SharedStringTable));
				case VariantValueType.Array:
				case VariantValueType.CellRange:
					throw new InvalidOperationException();
				default:
					throw new ArgumentException("Unknown VariantValueType" + value.Type);
			}
		}
	}
	#endregion
}
