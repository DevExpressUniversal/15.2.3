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
using System.Globalization;
using DevExpress.Office.Utils;
using DevExpress.Office;
using ApiExternalWorkbook = DevExpress.Spreadsheet.ExternalWorkbook;
using ExternalDefinedName = DevExpress.Spreadsheet.ExternalDefinedName;
using ApiExternalWorksheet = DevExpress.Spreadsheet.ExternalWorksheet;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model.External {
	#region ExternalLink
	public class ExternalLink {
		#region Fields
		ExternalWorkbook workbook;
		#endregion
		public ExternalLink(DocumentModel hostWorkbook) {
			this.workbook = new ExternalWorkbook(hostWorkbook);
		}
		public ExternalLink(ExternalWorkbook workbook) {
			this.workbook = workbook;
		}
		#region Properties
		public ExternalWorkbook Workbook { get { return workbook; } }
		public bool IsExternalWorkbook { get { return !IsDataSource; } } 
		public bool IsDataSource { get { return workbook is DdeExternalWorkbook; } }
		#endregion
		public void CopyFrom(ExternalLink source) {
			ExternalWorkbook sourceWorkbook = source.Workbook;
			ExternalWorkbook targetWorkbook = this.Workbook;
			targetWorkbook.CopyFrom(sourceWorkbook);
		}
	}
	#endregion
	#region ExternalLinkInfo
	public class ExternalLinkInfo {
		public ExternalLink ExternalLink { get; set; }
		public int Index { get; set; }
		public ExternalLinkInfo(External.ExternalLink externalLink, int index) {
			this.ExternalLink = externalLink;
			this.Index = index;
		}
	}
	#endregion
	#region ExternalLinkCollection
	public class ExternalLinkCollection : IEnumerable<ExternalLink> {
		#region Fields
		readonly DocumentModel documentModel;
		readonly List<ExternalLink> innerList;
		#endregion
		public ExternalLinkCollection(DocumentModel documentModel) {
			this.innerList = new List<ExternalLink>();
			this.documentModel = documentModel;
		}
		#region Properties
		public List<ExternalLink> InnerList { get { return innerList; } }
		public int Count { get { return innerList.Count; } }
		public ExternalLink this[int index] {
			get {
				if (index >= innerList.Count)
					return null;
				return innerList[index];
			}
		}
		public ExternalLink this[string name] {
			get {
				int index = IndexOf(name);
				if (index >= 0)
					return innerList[index];
				return null;
			}
		}
		#endregion
		public void Add(ExternalLink link) {
			ExternalWorkbookAddCommand command = new ExternalWorkbookAddCommand(link, link.Workbook, null);
			command.Execute();
		}
		protected internal void AddCore(ExternalLink link, int position, bool modifyReferences) {
			ExternalLinkInsertHistoryItem historyItem = new ExternalLinkInsertHistoryItem(documentModel, link, position, modifyReferences);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void Clear() {
			innerList.Clear();
		}
		public int IndexOf(string name) {
			for (int i = 0; i < innerList.Count; i++)
				if (StringExtensions.CompareInvariantCultureIgnoreCase(innerList[i].Workbook.FilePath, name) == 0)
					return i;
			return -1;
		}
		protected internal int IndexOf(ExternalWorkbook workbook) {
			if (workbook != null) {
				for (int i = 0; i < innerList.Count; i++)
					if (object.ReferenceEquals(workbook, innerList[i].Workbook))
						return i;
			}
			return -1;
		}
		public int IndexOf(ExternalLink link) {
			return innerList.IndexOf(link);
		}
		public void RemoveAt(int index, bool moveReferences) {
			ExternalLinkRemovedHistoryItem historyItem = new ExternalLinkRemovedHistoryItem(documentModel, index, moveReferences);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void CopyFrom(ExternalLinkCollection source, DocumentModel targetHostDocumentModel) {
			this.innerList.Clear();
			int sourceExternalLinksCollectionCount = source.Count;
			for (int index = 0; index < sourceExternalLinksCollectionCount; index++) {
				ExternalLink sourceExternalLink = source[index];
				ExternalLink targetExternalLink = new ExternalLink(targetHostDocumentModel);
				targetExternalLink.CopyFrom(sourceExternalLink);
			}
		}
		#region IEnumerable<ExternalLink> Members
		public IEnumerator<ExternalLink> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		public ExternalLinkInfo GetInfoByName(string name) {
			int index = IndexOf(name);
			if (index >= 0)
				return new ExternalLinkInfo(innerList[index], index);
			return null;
		}
	}
	#endregion
	#region ExternalWorkbook
	public class ExternalWorkbook : IModelWorkbook, IDocumentModelPart {
		#region Fields
		readonly DocumentModel hostWorkbook;
		ExternalWorksheetCollection sheets;
		ExternalDefinedNameCollection definedNames;
		string filePath;
		SheetDefinitionCollection sheetDefinitions;
		#endregion
		public ExternalWorkbook(DocumentModel hostWorkbook) {
			Guard.ArgumentNotNull(hostWorkbook, "hostWorkbook");
			this.hostWorkbook = hostWorkbook;
			Initialize();
		}
		#region Properties
		public ExternalWorksheetCollection Sheets { get { return sheets; } }
		public ExternalDefinedNameCollection DefinedNames { get { return definedNames; } }
		public virtual string FilePath { get { return filePath; } set { filePath = value; } }
		public SheetDefinitionCollection SheetDefinitions { get { return sheetDefinitions; } }
		#endregion
		protected internal virtual void Initialize() {
			this.sheets = new ExternalWorksheetCollection(this);
			this.definedNames = new ExternalDefinedNameCollection(hostWorkbook);
			this.sheetDefinitions = new SheetDefinitionCollection();
			this.filePath = String.Empty;
		}
		#region IWorkbookBase Members
		DefinedNameCollectionBase IModelWorkbook.DefinedNames { get { return definedNames; } }
		IWorksheet IModelWorkbook.GetSheetByName(string name) {
			return Sheets[name];
		}
		public List<IWorksheet> GetSheets(string startSheetName, string endSheetName) {
			List<ExternalWorksheet> list = sheets.GetRange(startSheetName, endSheetName);
			return GetSheetsCore(list);
		}
		public List<IWorksheet> GetSheets() {
			return GetSheetsCore(sheets.GetRange());
		}
		List<IWorksheet> GetSheetsCore(List<ExternalWorksheet> target) {
			if (target == null)
				return null;
			List<IWorksheet> resultList = new List<IWorksheet>();
			foreach (ExternalWorksheet sheet in target)
				resultList.Add(sheet);
			return resultList;
		}
		public IWorksheet GetSheetById(int sheetId) {
			return sheets[sheetId];
		}
		public IWorksheet GetSheetByIndex(int index) {
			return sheets[index];
		}
		public int SheetCount {
			get { return sheets.Count; }
		}
		public int CreateNextSheetId() {
			return hostWorkbook.CreateNextSheetId();
		}
		public CultureInfo Culture { get { return hostWorkbook.Culture; } set { Exceptions.ThrowInternalException(); } }
		public int ContentVersion { get { return hostWorkbook.ContentVersion; } }
		public CalculationChain CalculationChain { get { return hostWorkbook.CalculationChain; } }
		public SharedStringTable SharedStringTable { get { return hostWorkbook.SharedStringTable; } }
		public StyleSheet StyleSheet { get { return hostWorkbook.StyleSheet; } }
		public int IncrementContentVersion() {
			return hostWorkbook.IncrementContentVersion();
		}
		public ExternalWorkbook GetExternalWorkbookByIndex(int index) {
			return hostWorkbook.GetExternalWorkbookByIndex(index);
		}
		public ExternalWorkbook GetExternalWorkbookByName(string name) {
			return hostWorkbook.GetExternalWorkbookByName(name);
		}
		public WorkbookDataContext DataContext { get { return hostWorkbook.DataContext; } }
		public Table GetTableByName(string name) {
			return null;
		}
		public void CheckDefinedName(string name, int scopedSheetId) {
			if (!WorkbookDataContext.IsIdent(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidDefinedName);
			if (scopedSheetId < 0) {
				if (definedNames.Contains(name))
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
			}
			else
				foreach (ExternalWorksheet sheet in Sheets)
					if (sheet.SheetId == scopedSheetId && sheet.DefinedNames.Contains(name))
						SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
		}
		#endregion
		#region IDocumentModelPart Members
		public IDocumentModel DocumentModel { get { return hostWorkbook; } }
		#endregion
		public void CopyFrom(ExternalWorkbook source) {
			this.definedNames.CopyFrom(source.DefinedNames);
			this.filePath = source.FilePath;
			sheets.CopyFrom(source.Sheets);
		}
	}
	#endregion
	#region ExternalWorksheet
	public class ExternalWorksheet : SheetBase, IWorksheet, ISupportsCopyFrom<ExternalWorksheet> {
		#region Fields
		readonly ExternalDefinedNameCollection definedNames;
		readonly ExternalRowCollection rows;
		readonly ExternalTablesCollection tables;
		readonly ApiExternalWorksheet apiExternalWorksheet;
		#endregion
		public ExternalWorksheet(IModelWorkbook workbook, string name)
			: base(workbook) {
			this.Name = name;
			this.definedNames = new ExternalDefinedNameCollection(workbook.DataContext.Workbook);
			this.rows = new ExternalRowCollection(this);
			this.tables = new ExternalTablesCollection();
		}
		public ExternalWorksheet(IModelWorkbook workbook, string name, ApiExternalWorksheet apiExternalWorksheet)
			: this(workbook, name) {
			this.apiExternalWorksheet = apiExternalWorksheet;
		}
		#region Properties
		public ExternalDefinedNameCollection DefinedNames { get { return definedNames; } }
		public ExternalRowCollection Rows { get { return rows; } }
		public bool RefreshFailed { get; set; }
		public ApiExternalWorksheet ApiExternalWorksheet { get { return apiExternalWorksheet; } }
		public override SheetType SheetType { get { return SheetType.External; } }
		#endregion
		#region IWorksheet Members
		DefinedNameCollectionBase IWorksheet.DefinedNames { get { return definedNames; } }
		IRowCollectionBase ICellTable.Rows { get { return rows; } }
		ITableCollection IWorksheet.Tables { get { return tables; } }
		public bool ReadOnly { get { return false; } }
		public bool IsDataSheet { get { return false; } }
		public ICellBase TryGetCell(int column, int row) {
			if (apiExternalWorksheet != null)
				return TryCreateCellFromData(column, row);
			ExternalRow externalRow = Rows.TryGetRow(row);
			if (externalRow == null)
				return null;
			return externalRow.Cells.TryGetCell(column);
		}
		public ICellBase TryCreateCellFromData(int column, int row) {
			Spreadsheet.CellValue cellValue = apiExternalWorksheet.GetCellValue(column, row);
			VariantValue value = VariantValue.Empty;
			if (cellValue != null) {
				if (cellValue.ModelVariantValue.IsSharedString)
					value = cellValue.TextValue;
				else
					value = cellValue.ModelVariantValue;
			}
			if (!value.IsEmpty) {
				ICellBase cell = ((IWorksheet)this).GetCell(column, row);
				cell.Value = value;
				return cell;
			}
			return null;
		}
		ICellBase ICellTable.GetCell(int column, int row) {
			return Rows[row].Cells[column];
		}
		public VariantValue GetCalculatedCellValue(int column, int row) {
			ICellBase cell = null;
			if (apiExternalWorksheet != null) {
				cell = TryCreateCellFromData(column, row);
				if (cell != null)
					return cell.Value;
				return VariantValue.ErrorReference;
			}
			cell = TryGetCell(column, row);
			if (cell == null) {
				return VariantValue.ErrorReference;
			}
			else
				return cell.Value;
		}
		bool IWorksheet.IsColumnVisible(int columnIndex) {
			return true;
		}
		bool IWorksheet.IsRowVisible(int rowIndex) {
			return true;
		}
		public Table GetTableByCellPosition(int columnIndex, int rowIndex) {
			return null;
		}
		#endregion
		#region ICellTable Members
		public int MaxRowCount { get { return IndicesChecker.MaxRowCount; } }
		public int MaxColumnCount { get { return IndicesChecker.MaxColumnCount; } }
		#endregion
		#region ISupportsCopyFrom<IWorksheet> Members
		public void CopyFrom(ExternalWorksheet sourceWorksheet) {
			CopyExternalWorksheetOperation copyOperation = new CopyExternalWorksheetOperation(sourceWorksheet, this);
			copyOperation.Execute();
		}
		#endregion
	}
	#endregion
	#region ExternalWorksheetCollection
	public class ExternalWorksheetCollection : WorksheetCollectionBase<ExternalWorksheet>, ISupportsCopyFrom<ExternalWorksheetCollection> {
		public ExternalWorksheetCollection(ExternalWorkbook workbook)
			: base(workbook) {
		}
		#region Properties
		public new ExternalWorkbook Workbook { get { return (ExternalWorkbook)base.Workbook; } }
		#endregion
		public void CopyFrom(ExternalWorksheetCollection source) {
			List<ExternalWorksheet> sourceSheets = source.GetRange();
			foreach (ExternalWorksheet sourceExternalWorksheet in sourceSheets) {
				ExternalWorksheet targetWorksheet = new ExternalWorksheet(Workbook, sourceExternalWorksheet.Name);
				targetWorksheet.CopyFrom(sourceExternalWorksheet);
			}
		}
	}
	#endregion
	#region ExternalDefinedName
	public class ExternalDefinedName : DefinedNameBase {
		public ExternalDefinedName(string name, ExternalWorkbook workbook, string reference, int scopedSheetId)
			: base(name, workbook, reference, scopedSheetId) {
		}
		public ExternalDefinedName(string name, ExternalWorkbook workbook, ParsedExpression expression, int scopedSheetId)
			: base(name, workbook, expression, scopedSheetId) {
		}
		#region Properties
		protected internal new ExternalWorkbook Workbook { get { return (ExternalWorkbook)base.Workbook; } }
		#endregion
		protected internal override DefinedNameBase Clone() {
			return new ExternalDefinedName(Name, Workbook, Expression.Clone(), ScopedSheetId);
		}
	}
	#endregion
	#region ExternalDefinedNameCollection
	public class ExternalDefinedNameCollection : DefinedNameCollectionBase {
		public ExternalDefinedNameCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public override UndoableClonableCollection<DefinedNameBase> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new ExternalDefinedNameCollection(documentModelPart.DocumentModel);
		}
		public override DefinedNameBase GetCloneItem(DefinedNameBase item, IDocumentModelPart documentModelPart) {
			return item.Clone();
		}
	}
	#endregion
	#region ExternalTablesCollection
	public class ExternalTablesCollection : ITableCollection {
		#region ITablesCollection Members
		public Table this[string name] { get { return null; } }
		public int Add(Table table) {
			return -1;
		}
		#endregion
	}
	#endregion
}
