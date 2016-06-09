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
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DefinedNameEventHandler
	public delegate void NameChangedEventHandler(object sender, NameChangedEventArgs e);
	#endregion
	#region NameChangedEventArgs
	public class NameChangedEventArgs : EventArgs {
		#region Fields
		string name;
		string oldName;
		#endregion
		public NameChangedEventArgs(string name, string oldName) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Guard.ArgumentIsNotNullOrEmpty(oldName, "oldName");
			this.name = name;
			this.oldName = oldName;
		}
		#region Properites
		public string Name { get { return name; } }
		public string OldName { get { return oldName; } }
		#endregion
	}
	#endregion
	#region CacheChangedEventHandler
	public delegate void CacheChangedEventHandler(object sender, CacheChangedEventArgs e);
	#endregion
	#region CacheChangedEventArgs
	public class CacheChangedEventArgs : EventArgs {
		PivotCache cache;
		public CacheChangedEventArgs(PivotCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		public PivotCache Cache { get { return cache; } }
	}
	#endregion
	#region CellRangeChangedEventArgsBase
	public abstract class CellRangeChangedEventArgsBase<T> : EventArgs where T : CellRangeBase {
		#region Fields
		T oldRange;
		T newRange;
		#endregion
		protected CellRangeChangedEventArgsBase(T oldRange, T newRange) {
			Guard.ArgumentNotNull(oldRange, "oldRange");
			Guard.ArgumentNotNull(newRange, "newRange");
			this.oldRange = oldRange;
			this.newRange = newRange;
		}
		#region Properites
		public T OldRange { get { return oldRange; } }
		public T NewRange { get { return newRange; } }
		#endregion
	}
	#endregion
	#region CellRangeChangedEventHandler
	public delegate void CellRangeChangedEventHandler(object sender, CellRangeChangedEventArgs e);
	#endregion
	#region CellRangeChangedEventArgs
	public class CellRangeChangedEventArgs : CellRangeChangedEventArgsBase<CellRange> {
		public CellRangeChangedEventArgs(CellRange oldRange, CellRange newRange)
			: base(oldRange, newRange) {
		}
	}
	#endregion
	#region CellRangeBaseChangedEventHandler
	public delegate void CellRangeBaseChangedEventHandler(object sender, CellRangeBaseChangedEventArgs e);
	#endregion
	#region CellRangeBaseChangedEventArgs
	public class CellRangeBaseChangedEventArgs : CellRangeChangedEventArgsBase<CellRangeBase> {
		public CellRangeBaseChangedEventArgs(CellRangeBase oldRange, CellRangeBase newRange)
			: base(oldRange, newRange) {
		}
	}
	#endregion
	#region DefinedNameWorksheetAdd
	public delegate void DefinedNameWorksheetAddEventHandler(object sender, DefinedNameWorksheetAddEventArgs e);
	public class DefinedNameWorksheetAddEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly DefinedName definedName;
		public DefinedNameWorksheetAddEventArgs(Worksheet worksheet, DefinedName definedName)
			: base(worksheet) {
			Guard.ArgumentNotNull(definedName, "definedName");
			this.definedName = definedName;
		}
		public DefinedName DefinedName { get { return definedName; } }
	}
	#endregion
	#region DefinedNameWorksheetRemove
	public delegate void DefinedNameWorksheetRemoveEventHandler(object sender, DefinedNameWorksheetRemoveEventArgs e);
	public class DefinedNameWorksheetRemoveEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly DefinedName definedName;
		public DefinedNameWorksheetRemoveEventArgs(Worksheet worksheet, DefinedName definedName)
			: base(worksheet) {
			Guard.ArgumentNotNull(definedName, "definedName");
			this.definedName = definedName;
		}
		public DefinedName DefinedName { get { return definedName; } }
	}
	#endregion
	#region DefinedNameWorksheetCollectionClear
	public delegate void DefinedNameWorksheetCollectionClearEventHandler(object sender, DefinedNameWorksheetCollectionClearEventArgs e);
	public class DefinedNameWorksheetCollectionClearEventArgs : WorksheetsObjectChangedEventArgsBase {
		public DefinedNameWorksheetCollectionClearEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region DefinedNameWorkbookAdd
	public delegate void DefinedNameWorkbookAddEventHandler(object sender, DefinedNameWorkbookAddEventArgs e);
	public class DefinedNameWorkbookAddEventArgs : WorkbooksObjectChangedEventArgsBase {
		readonly DefinedName definedName;
		public DefinedNameWorkbookAddEventArgs(DocumentModel workbook, DefinedName definedName)
			: base(workbook) {
			Guard.ArgumentNotNull(definedName, "definedName");
			this.definedName = definedName;
		}
		public DefinedName DefinedName { get { return definedName; } }
	}
	#endregion
	#region DefinedNameWorkbookRemove
	public delegate void DefinedNameWorkbookRemoveEventHandler(object sender, DefinedNameWorkbookRemoveEventArgs e);
	public class DefinedNameWorkbookRemoveEventArgs : WorkbooksObjectChangedEventArgsBase {
		readonly DefinedName definedName;
		public DefinedNameWorkbookRemoveEventArgs(DocumentModel workbook, DefinedName definedName)
			: base(workbook) {
			Guard.ArgumentNotNull(definedName, "definedName");
			this.definedName = definedName;
		}
		public DefinedName DefinedName { get { return definedName; } }
	}
	#endregion
	#region DefinedNameWorkbookCollectionClear
	public delegate void DefinedNameWorkbookCollectionClearEventHandler(object sender, DefinedNameWorkbookCollectionClearEventArgs e);
	public class DefinedNameWorkbookCollectionClearEventArgs : WorkbooksObjectChangedEventArgsBase {
		public DefinedNameWorkbookCollectionClearEventArgs(DocumentModel workbook)
			: base(workbook) {
		}
	}
	#endregion
	#region AfterDefinedNameRenamed
	public delegate void AfterDefinedNameRenamedEventHandler(object sender, AfterDefinedNameRenamedEventArgs e);
	public class AfterDefinedNameRenamedEventArgs : WorkbooksObjectChangedEventArgsBase {
		readonly DefinedName name;
		readonly string oldName;
		readonly string newName;
		public AfterDefinedNameRenamedEventArgs(DocumentModel workbook, DefinedName name, string oldName, string newName)
			: base(workbook) {
			this.name = name;
			this.oldName = oldName;
			this.newName = newName;
		}
		public DefinedName DefinedName { get { return name; } }
		public string OldName { get { return oldName; } }
		public string NewName { get { return newName; } }
	}
	#endregion
	#region WorksheetCollectionChangedEventHandler
	public delegate void WorksheetCollectionChangedEventHandler(object sender, WorksheetCollectionChangedEventArgs e);
	#endregion
	#region ExternalLinksCollectionChangedEventArgs
	public delegate void ExternalLinksCollectionChangedEventHandler(object sender, ExternalLinksCollectionChangedEventArgs args);
	public class ExternalLinksCollectionChangedEventArgs : EventArgs {
		ExternalLink newExternalLink;
		public ExternalLinksCollectionChangedEventArgs(ExternalLink newExternalLink) {
			Guard.ArgumentNotNull(newExternalLink, "newExternalLink");
			this.newExternalLink = newExternalLink;
		}
		public ExternalLink NewExternalLink { get { return newExternalLink; } }
	}
	#endregion
	#region WorkbooksObjectChangedEventArgsBase (abstract class)
	public abstract class WorkbooksObjectChangedEventArgsBase : EventArgs {
		readonly DocumentModel workbook;
		protected WorkbooksObjectChangedEventArgsBase(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
		}
		public DocumentModel Workbook { get { return workbook; } }
	}
	#endregion
	#region WorksheetCollectionChangedEventArgs
	public class WorksheetCollectionChangedEventArgs : WorksheetsObjectChangedEventArgs {
		public WorksheetCollectionChangedEventArgs(Worksheet sheet, int sheetIndex)
			: base(sheet, sheetIndex) {
		}
	}
	#endregion
	#region WorksheetsObjectChangedEventArgs
	public class WorksheetsObjectChangedEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly int index;
		public WorksheetsObjectChangedEventArgs(Worksheet worksheet, int index)
			: base(worksheet) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#endregion
	#region WorksheetsObjectChangedEventArgsBase (abstract class)
	public abstract class WorksheetsObjectChangedEventArgsBase : EventArgs {
		readonly Worksheet worksheet;
		protected WorksheetsObjectChangedEventArgsBase(Worksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.worksheet = worksheet;
		}
		public Worksheet Worksheet { get { return worksheet; } }
	}
	#endregion
	#region WorksheetMoved
	public delegate void WorksheetMovedEventHandler(object sender, WorksheetMovedEventArgs e);
	public class WorksheetMovedEventArgs : EventArgs {
		readonly Worksheet worksheet;
		readonly int oldIndex;
		readonly int newIndex;
		public WorksheetMovedEventArgs(Worksheet worksheet, int oldIndex, int newIndex) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.worksheet = worksheet;
			this.oldIndex = oldIndex;
			this.newIndex = newIndex;
		}
		public Worksheet Worksheet { get { return worksheet; } }
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
	}
	#endregion
	#region HyperlinkCollectionChanged
	public delegate void HyperlinkCollectionChangedEventHandler(object sender, HyperlinkCollectionChangedEventArgs e);
	public class HyperlinkCollectionChangedEventArgs : WorksheetsObjectChangedEventArgs {
		public HyperlinkCollectionChangedEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
#if !SL
	#region HyperlinkFormShowingEventHandler
	public delegate void HyperlinkFormShowingEventHandler(object sender, HyperlinkFormShowingEventArgs e);
	#endregion
	#region HyperlinkFormShowingEventArgs
	public class HyperlinkFormShowingEventArgs : FormShowingEventArgs {
		readonly HyperlinkFormControllerParameters controllerParameters;
		public HyperlinkFormShowingEventArgs(HyperlinkFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public HyperlinkFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
#endif
	#region StyleCollectionChanged
	public delegate void StyleCollectionChangedEventHandler(object sender, StyleCollectionChangedEventArgs args);
	public class StyleCollectionChangedEventArgs : EventArgs {
		CellStyleBase newStyle;
		int styleIndex;
		public StyleCollectionChangedEventArgs(CellStyleBase newStyle, int styleIndex) {
			Guard.ArgumentNotNull(newStyle, "newStyle");
			Guard.ArgumentNonNegative(styleIndex, "styleIndex");
			this.newStyle = newStyle;
			this.styleIndex = styleIndex;
		}
		public CellStyleBase NewStyle { get { return newStyle; } }
		public int StyleIndex { get { return styleIndex; } }
	}
	#endregion
	#region TableStyleCollectionChangedEventArgs
	public delegate void TableStyleCollectionChangedEventHandler(object sender, TableStyleCollectionChangedEventArgs args);
	public class TableStyleCollectionChangedEventArgs : EventArgs {
		TableStyle newStyle;
		public TableStyleCollectionChangedEventArgs(TableStyle newStyle) {
			Guard.ArgumentNotNull(newStyle, "newStyle");
			this.newStyle = newStyle;
		}
		public TableStyle NewStyle { get { return newStyle; } }
	}
	#endregion
	#region TableStyleCollectionClearedEventArgs
	public delegate void TableStyleCollectionClearedEventHandler(object sender, TableStyleCollectionClearedEventArgs args);
	public class TableStyleCollectionClearedEventArgs : EventArgs {
		TableStyleElementIndexTableType tableType;
		public TableStyleCollectionClearedEventArgs(TableStyleElementIndexTableType tableType) {
			this.tableType = tableType;
		}
		public TableStyleElementIndexTableType TableType { get { return tableType; } }
	}
	#endregion
	#region BeforeRowRemove
	public delegate void BeforeRowRemoveEventHandler(object sender, BeforeRowRemoveEventArgs e);
	public class BeforeRowRemoveEventArgs : WorksheetsObjectChangedEventArgs {
		public BeforeRowRemoveEventArgs(Worksheet worksheet, int index, int deletedRowsCount)
			: base(worksheet, index) {
			DeletedRowsCount = deletedRowsCount;
		}
		public int DeletedRowsCount { get; set; }
	}
	public delegate void BeforeRowsClearedEventHandler(object sender, BeforeRowsClearedEventArgs e);
	public class BeforeRowsClearedEventArgs : EventArgs {
		public BeforeRowsClearedEventArgs(Worksheet sheet) {
			Worksheet = sheet;
		}
		public Worksheet Worksheet { get; set; }
	}
	#endregion
	#region ColumnRemoved
	public delegate void ColumnRemovedEventHandler(object sender, ColumnRemovedEventArgs e);
	public class ColumnRemovedEventArgs : WorksheetsObjectChangedEventArgs {
		public ColumnRemovedEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
	#region ShiftCellsLeft
	public delegate void ShiftCellsLeftEventHandler(object sender, ShiftCellsLeftEventArgs e);
	public class ShiftCellsLeftEventArgs : EventArgs {
		int firstColumnIndex;
		Worksheet worksheet;
		int firstRowIndex;
		int lastRowIndex;
		public ShiftCellsLeftEventArgs(Worksheet worksheet, int firstColumnIndex, int firstRowIndex, int lastRowIndex) {
			Guard.ArgumentNonNegative(firstColumnIndex, "firstColumnIndex");
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.firstColumnIndex = firstColumnIndex;
			this.worksheet = worksheet;
			this.firstRowIndex = firstRowIndex;
			this.lastRowIndex = lastRowIndex;
		}
		public int FirstColumnIndex { get { return firstColumnIndex; } set { firstColumnIndex = value; } }
		public Worksheet Worksheet { get { return worksheet; } set { worksheet = value; } }
		public int FirstRowIndex { get { return firstRowIndex; } set { firstRowIndex = value; } }
		public int LastRowIndex { get { return lastRowIndex; } set { lastRowIndex = value; } }
	}
	#endregion
	#region ShiftCellsUp
	public delegate void ShiftCellsUpEventHandler(object sender, ShiftCellsUpEventArgs e);
	public class ShiftCellsUpEventArgs : EventArgs {
		int firstColumnIndex;
		Worksheet worksheet;
		int lastColumnIndex;
		int firstRowIndex;
		public ShiftCellsUpEventArgs(Worksheet worksheet, int firstColumnIndex, int lastColumnIndex, int firstRowIndex) {
			Guard.ArgumentNonNegative(firstColumnIndex, "firstColumnIndex");
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.firstColumnIndex = firstColumnIndex;
			this.worksheet = worksheet;
			this.lastColumnIndex = lastColumnIndex;
			this.firstRowIndex = firstRowIndex;
		}
		public int FirstColumnIndex { get { return firstColumnIndex; } set { firstColumnIndex = value; } }
		public Worksheet Worksheet { get { return worksheet; } set { worksheet = value; } }
		public int LastColumnIndex { get { return lastColumnIndex; } set { lastColumnIndex = value; } }
		public int FirstRowIndex { get { return firstRowIndex; } set { firstRowIndex = value; } }
	}
	#endregion
	#region MergedCellsCollectionChangedEventArgs
	public delegate void MergedCellsCollectionChangedEventHandler(object sender, MergedCellsCollectionChangedEventArgs e);
	public class MergedCellsCollectionChangedEventArgs : WorksheetsObjectChangedEventArgsBase {
		CellRange mergedCellRange;
		public MergedCellsCollectionChangedEventArgs(Worksheet worksheet, CellRange mergedCellRange)
			: base(worksheet) {
			this.mergedCellRange = mergedCellRange;
		}
		public CellRange MergedCellRange { get { return mergedCellRange; } }
	}
	public delegate void MergedCellsCollectionClearedEventHandler(object sender, MergedCellsCollectionClearedEventArgs e);
	public class MergedCellsCollectionClearedEventArgs : WorksheetsObjectChangedEventArgsBase {
		public MergedCellsCollectionClearedEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region ArrayFormulaAddEventArgs
	public delegate void ArrayFormulaAddEventHandler(object sender, ArrayFormulaAddEventArgs e);
	public class ArrayFormulaAddEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly ArrayFormula arrayFormula;
		public ArrayFormulaAddEventArgs(Worksheet worksheet, ArrayFormula arrayFormula)
			: base(worksheet) {
			Guard.ArgumentNotNull(arrayFormula, "arrayFormula");
			this.arrayFormula = arrayFormula;
		}
		public ArrayFormula ArrayFormula { get { return arrayFormula; } }
	}
	#endregion
	#region ArrayFormulaRemoveEventArgs
	public delegate void ArrayFormulaRemoveAtEventHandler(object sender, ArrayFormulaRemoveEventArgs e);
	public class ArrayFormulaRemoveEventArgs : WorksheetsObjectChangedEventArgs {
		public ArrayFormulaRemoveEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
	#region ArrayFormulaCollectionClearEventArgs
	public delegate void ArrayFormulaCollectionClearEventHandler(object sender, ArrayFormulaCollectionClearEventArgs e);
	public class ArrayFormulaCollectionClearEventArgs : WorksheetsObjectChangedEventArgsBase {
		public ArrayFormulaCollectionClearEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region DrawingObjectsCollection
	public delegate void DrawingObjectsCollectionChangedEventHandler(object sender, DrawingObjectsCollectionChangedEventArgs args);
	public class DrawingObjectsCollectionChangedEventArgs : EventArgs {
		#region Fields
		readonly IDrawingObject drawing;
		#endregion
		public DrawingObjectsCollectionChangedEventArgs(IDrawingObject drawing) {
			Guard.ArgumentNotNull(drawing, "drawing");
			this.drawing = drawing;
		}
		public IDrawingObject Drawing { get { return drawing; } }
	}
	#endregion
	#region PictureInserted
	public delegate void DrawingInsertedEventHandler(object sender, DrawingInsertedEventArgs args);
	public class DrawingInsertedEventArgs : DrawingObjectsCollectionChangedEventArgs {
		#region Fields
		readonly int index;
		#endregion
		public DrawingInsertedEventArgs(int index, IDrawingObject drawing)
			: base(drawing) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#endregion
	#region ChartCollection
	public delegate void ChartCollectionChangedEventHandler(object sender, ChartCollectionChangedEventArgs args);
	public class ChartCollectionChangedEventArgs : EventArgs {
		#region Fields
		readonly Chart chart;
		#endregion
		public ChartCollectionChangedEventArgs(Chart chart) {
			Guard.ArgumentNotNull(chart, "chart");
			this.chart = chart;
		}
		public Chart Chart { get { return chart; } }
	}
	#endregion
	#region DocumentUpdateCompleteEventHandler
	public delegate void DocumentUpdateCompleteEventHandler(object sender, DocumentUpdateCompleteEventArgs e);
	#endregion
	#region DocumentUpdateCompleteEventArgs
	public class DocumentUpdateCompleteEventArgs : EventArgs {
		readonly DocumentModelDeferredChanges deferredChanges;
		public DocumentUpdateCompleteEventArgs(DocumentModelDeferredChanges deferredChanges) {
			Guard.ArgumentNotNull(deferredChanges, "deferredChanges");
			this.deferredChanges = deferredChanges;
		}
		public DocumentModelDeferredChanges DeferredChanges { get { return deferredChanges; } }
	}
	#endregion
	#region DocumentContentSettedEventHandler
	public delegate void DocumentContentSettedEventHandler(object sender, DocumentContentSettedEventArgs e);
	#endregion
	#region DocumentContentSettedEventArgs
	public class DocumentContentSettedEventArgs : EventArgs {
		readonly DocumentModelChangeType changeType;
		public DocumentContentSettedEventArgs(DocumentModelChangeType changeType) {
			this.changeType = changeType;
		}
		public DocumentModelChangeType ChangeType { get { return changeType; } }
	}
	#endregion
	#region ConditionalFormattingRemoveAt
	public delegate void ConditionalFormattingRemoveInsertNoApiInvalidationEventHandler(object sender, ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs e);
	public class ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs : EventArgs {
		int oldIndex;
		int newIndex;
		public ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs(int oldIndex, int newIndex) {
			Guard.ArgumentNonNegative(newIndex, "NewIndex");
			Guard.ArgumentNonNegative(oldIndex, "OldIndex");
			this.newIndex = newIndex;
			this.oldIndex = oldIndex;
		}
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
	}
	#endregion
	#region CommentRunAdd
	public delegate void CommentRunAddEventHandler(object sender, CommentRunAddEventArgs e);
	public class CommentRunAddEventArgs : EventArgs {
		readonly CommentRun run;
		readonly Comment comment;
		public CommentRunAddEventArgs(Comment comment, CommentRun run) {
			Guard.ArgumentNotNull(comment, "comment");
			Guard.ArgumentNotNull(run, "run");
			this.comment = comment;
			this.run = run;
		}
		public Comment Comment { get { return comment; } }
		public CommentRun Run { get { return run; } }
	}
	#endregion
	#region CommentRunRemoveAt
	public delegate void CommentRunRemoveAtEventHandler(object sender, CommentRunRemoveAtEventArgs e);
	public class CommentRunRemoveAtEventArgs : EventArgs {
		readonly Comment comment;
		readonly int index;
		public CommentRunRemoveAtEventArgs(Comment comment, int index) {
			Guard.ArgumentNotNull(comment, "comment");
			Guard.ArgumentNonNegative(index, "index");
			this.comment = comment;
			this.index = index;
		}
		public Comment Comment { get { return comment; } }
		public int Index { get { return index; } }
	}
	#endregion
	#region CommentRunInsert
	public delegate void CommentRunInsertEventHandler(object sender, CommentRunInsertEventArgs e);
	public class CommentRunInsertEventArgs : CommentRunAddEventArgs {
		readonly int index;
		public CommentRunInsertEventArgs(Comment comment, CommentRun run, int index)
			: base(comment, run) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#endregion
	#region CommentRunCollectionClear
	public delegate void CommentRunCollectionClearEventHandler(object sender, CommentRunCollectionClearEventArgs e);
	public class CommentRunCollectionClearEventArgs : EventArgs {
		readonly Comment comment;
		public CommentRunCollectionClearEventArgs(Comment comment) {
			Guard.ArgumentNotNull(comment, "comment");
			this.comment = comment;
		}
		public Comment Comment { get { return comment; } }
	}
	#endregion
	#region CommentAdd
	public delegate void CommentAddEventHandler(object sender, CommentAddEventArgs e);
	public class CommentAddEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly Comment comment;
		public CommentAddEventArgs(Worksheet worksheet, Comment comment)
			: base(worksheet) {
			Guard.ArgumentNotNull(comment, "comment");
			this.comment = comment;
		}
		public Comment Comment { get { return comment; } }
	}
	#endregion
	#region CommentRemoveAt
	public delegate void CommentRemoveAtEventHandler(object sender, CommentRemoveAtEventArgs e);
	public class CommentRemoveAtEventArgs : WorksheetsObjectChangedEventArgs {
		public CommentRemoveAtEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
	#region CommentClear
	public delegate void CommentCollectionClearEventHandler(object sender, CommentCollectionClearEventArgs e);
	public class CommentCollectionClearEventArgs : WorksheetsObjectChangedEventArgsBase {
		public CommentCollectionClearEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region HyperlinkAdd
	public delegate void HyperlinkAddEventHandler(object sender, HyperlinkAddEventArgs e);
	public class HyperlinkAddEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly ModelHyperlink hyperlink;
		public HyperlinkAddEventArgs(Worksheet worksheet, ModelHyperlink hyperlink)
			: base(worksheet) {
			Guard.ArgumentNotNull(hyperlink, "hyperlink");
			this.hyperlink = hyperlink;
		}
		public ModelHyperlink Hyperlink { get { return hyperlink; } }
	}
	#endregion
	#region HyperlinkRemoveAt
	public delegate void HyperlinkRemoveAtEventHandler(object sender, HyperlinkRemoveAtEventArgs e);
	public class HyperlinkRemoveAtEventArgs : WorksheetsObjectChangedEventArgs {
		public HyperlinkRemoveAtEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
	#region HyperlinkClear
	public delegate void HyperlinkCollectionClearEventHandler(object sender, HyperlinkCollectionClearEventArgs e);
	public class HyperlinkCollectionClearEventArgs : WorksheetsObjectChangedEventArgsBase {
		public HyperlinkCollectionClearEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region TableAdd
	public delegate void TableAddEventHandler(object sender, TableAddEventArgs e);
	public class TableAddEventArgs : WorksheetsObjectChangedEventArgsBase {
		readonly Model.Table table;
		public TableAddEventArgs(Worksheet worksheet, Model.Table table)
			: base(worksheet) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		public Model.Table Table { get { return table; } }
	}
	#endregion
	#region TableRemoveAt
	public delegate void TableRemoveAtEventHandler(object sender, TableRemoveAtEventArgs e);
	public class TableRemoveAtEventArgs : WorksheetsObjectChangedEventArgs {
		public TableRemoveAtEventArgs(Worksheet worksheet, int index)
			: base(worksheet, index) {
		}
	}
	#endregion
	#region TableClear
	public delegate void TableCollectionClearEventHandler(object sender, TableCollectionClearEventArgs e);
	public class TableCollectionClearEventArgs : WorksheetsObjectChangedEventArgsBase {
		public TableCollectionClearEventArgs(Worksheet worksheet)
			: base(worksheet) {
		}
	}
	#endregion
	#region TableColumnAdd
	public delegate void TableColumnAddEventHandler(object sender, TableColumnAddEventArgs e);
	public class TableColumnAddEventArgs : EventArgs {
		readonly Model.TableColumn tableColumn;
		readonly int index;
		public TableColumnAddEventArgs(Model.TableColumn tableColumn, int index) {
			Guard.ArgumentNotNull(tableColumn, "tableColumn");
			this.tableColumn = tableColumn;
			this.index = index;
		}
		public Model.TableColumn TableColumn { get { return tableColumn; } }
		public int Index { get { return index; } }
	}
	#endregion
	#region TableColumnRemoveAt
	public delegate void TableColumnRemoveAtEventHandler(object sender, TableColumnRemoveAtEventArgs e);
	public class TableColumnRemoveAtEventArgs : EventArgs {
		readonly Model.Table table;
		readonly int index;
		public TableColumnRemoveAtEventArgs(Model.Table table, int index) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.index = index;
		}
		public Model.Table Table { get { return table; } }
		public int Index { get { return index; } }
	}
	#endregion
	#region CustomFunctionEvaluate
	public delegate void CustomFunctionEvaluateEventHandler(object sender, CustomFunctionEvaluateEventArgs e);
	public class CustomFunctionEvaluateEventArgs : EventArgs {
		#region Fields
		readonly string name;
		readonly System.Collections.Generic.IList<VariantValue> arguments;
		VariantValue result;
		#endregion
		public CustomFunctionEvaluateEventArgs(string name, IList<VariantValue> arguments) {
			this.name = name;
			this.arguments = arguments;
		}
		public VariantValue Result { get { return result; } set { result = value; } }
		public string Name { get { return name; } }
		public System.Collections.Generic.IList<VariantValue> Arguments { get { return arguments; } }
	}
	#endregion
	#region AutoFilterRangeChanged
	public delegate void AutoFilterRangeChangedEventHandler(object sender, EventArgs e);
	public class AutoFilterRangeChangedEventArgs : EventArgs {
		readonly AutoFilterColumnCollection filterColumns;
		public AutoFilterRangeChangedEventArgs(AutoFilterColumnCollection filterColumns) {
			this.filterColumns = filterColumns;
		}
		public AutoFilterColumnCollection FilterColumns { get { return filterColumns; } }
	}
	#endregion
	#region ParametersCollection
	public delegate void ParametersCollectionChangedEventHandler(object sender, ParametersCollectionChangedEventArgs args);
	public class ParametersCollectionChangedEventArgs : EventArgs {
		#region Fields
		readonly SpreadsheetParameter parameter;
		#endregion
		public ParametersCollectionChangedEventArgs(SpreadsheetParameter parameter) {
			Guard.ArgumentNotNull(parameter, "parameter");
			this.parameter = parameter;
		}
		public SpreadsheetParameter Parameter { get { return parameter; } }
	}
	#endregion
	#region ParameterInserted
	public delegate void ParameterInsertedEventHandler(object sender, ParameterInsertedEventArgs args);
	public class ParameterInsertedEventArgs : ParametersCollectionChangedEventArgs {
		#region Fields
		readonly int index;
		#endregion
		public ParameterInsertedEventArgs(int index, SpreadsheetParameter parameter)
			: base(parameter) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#endregion
}
