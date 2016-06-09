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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraReports.Native.Parameters;
using FieldPathService = DevExpress.Snap.Core.Native.Data.Implementations.FieldPathService;
using InstructionController = DevExpress.XtraRichEdit.Fields.InstructionController;
using DevExpress.Data;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native.Templates {
	#region RowTemplateUpdaterBase
	public abstract class RowTemplateUpdaterBase {
		readonly Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> updateTemplateActions;
		readonly List<SnapTemplateInterval> updatedIntervals = new List<SnapTemplateInterval>();
		readonly List<TableRow> updatedRows = new List<TableRow>();
		readonly List<int> columnSpansTemplate;
		readonly TableCell cell;
		List<TableRow> rows;
		protected RowTemplateUpdaterBase(TableCell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
			this.columnSpansTemplate = TableRowHelper.GetColumnSpans(cell.Row);
			updateTemplateActions = CreateUpdateTemplateActions();
		}
		protected Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> UpdateTemplateActions { get { return updateTemplateActions; } }
		protected List<int> ColumnSpansTemplate { get { return columnSpansTemplate; } }
		protected List<SnapTemplateInterval> UpdatedIntervals { get { return updatedIntervals; } }
		protected List<TableRow> UpdatedRows { get { return updatedRows; } }
		protected List<TableRow> Rows {
			get {
				if (rows == null)
					InitTableRows(cell);
				return rows;
			}
			set { rows = value; }
		}
		protected internal virtual void InitTableRows(TableCell cell) {
			Rows = new List<TableRow>();
			cell.Table.Rows.ForEach(r => { Rows.Add(r); });
		}
		public void UpdateTemplates() {
			UpdateTemplates(true);
		}
		public virtual void UpdateTemplates(bool forceUpdateRows) {
			PrepareUpdateInfo();
			int count = Rows.Count;
			for (int i = 0; i < count; i++)
				UpdateTemplate(Rows[i]);
			if (forceUpdateRows)
				UpdateRows();
		}
		protected internal virtual void PrepareUpdateInfo() {
		}
		protected internal virtual void UpdateTemplate(TableRow row) {
			SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(row);
			if (bookmark == null || UpdatedIntervals.Contains(bookmark.TemplateInterval))
				return;
			UpdateTemplateCore(row, bookmark);
		}
		protected internal void UpdateTemplateCore(TableRow row, SnapBookmark bookmark) {
			List<TableRow> templateRows = new List<TableRow>();
			SnapBookmark currentRowBookmark = TableRowHelper.FindTemplateBookmark(row);
			while (Object.ReferenceEquals(currentRowBookmark, bookmark)) {
				templateRows.Add(row);
				row = row.Next;
				currentRowBookmark = TableRowHelper.FindTemplateBookmark(row);
			}
			for (int i = 0; i < templateRows.Count; i++)
				UpdateTableRowForTemplate(templateRows[i], bookmark);
			UpdatedIntervals.Add(bookmark.TemplateInterval);
		}
		protected internal virtual void UpdateTableRowForTemplate(TableRow row, SnapBookmark bookmark) {
			if (ShouldUpdateContent(row, bookmark))
				UpdateRowContent(bookmark, GetCell(row), row);
		}
		protected void UpdateRowContent(SnapBookmark bookmark, TableCell cell, TableRow row) {
			UpdateTemplateInterval(bookmark.TemplateInterval, cell);
			UpdatedRows.Add(row);
		}
		protected void UpdateTemplateInterval(SnapTemplateInterval interval, TableCell cell) {
			Action<TableCell, DocumentLogPosition> updater;
			if (UpdateTemplateActions.TryGetValue(interval.TemplateInfo.TemplateType, out updater))
				updater(cell, GetDocumentLogPosition(cell));
		}
		protected internal virtual void UpdateRows() {
			int count = UpdatedRows.Count;
			for (int i = 0; i < count; i++)
				((SnapPieceTable)UpdatedRows[i].PieceTable).UpdateTemplateByTableRow(UpdatedRows[i], false);
		}
		protected virtual DocumentLogPosition GetDocumentLogPosition(TableCell cell) {
			return new SnapObjectModelController((SnapPieceTable)cell.PieceTable).FindCellStartLogPosition(cell);
		}
		protected abstract Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions();
		protected abstract bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark);
		protected abstract TableCell GetCell(TableRow row);
		internal static void CopyModel(TableCell cell, DocumentLogPosition pos, DocumentModel source) {
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(cell.PieceTable, source, pos, DevExpress.XtraRichEdit.API.Native.InsertOptions.MatchDestinationFormatting, false) {
				SuppressFieldsUpdate = true,
				CopyBetweenInternalModels = true,
				PasteFromIE = false,
				RemoveLeadingPageBreak = true
			};
			command.Execute();
		}
		protected string GetSNFieldText(DataBrowser dataBrowser) {
			if (TemplatedFieldTypeQualifier.IsImageField(dataBrowser))
				return SNImageField.FieldType;
			if (TemplatedFieldTypeQualifier.IsCheckBoxField(dataBrowser))
				return SNCheckBoxField.FieldType;
			if (TemplatedFieldTypeQualifier.IsHyperlinkField(dataBrowser))
				return SNHyperlinkField.FieldType;
			return SNTextField.FieldType;
		}
	}
	#endregion
	#region RowTemplateDataUpdater
	public abstract class RowTemplateDataUpdater : RowTemplateUpdaterBase {
		SNDataInfo dataInfo;
		bool listHeaderUpdated = false;
		bool insertedToRight;
		List<SnapBookmark> processedDataRowBookmarks = new List<SnapBookmark>();
		protected RowTemplateDataUpdater(TableCell cell, SNDataInfo dataInfo, bool insertedToRight)
			: base(cell) {
			Guard.ArgumentNotNull(dataInfo, "dataInfo");
			this.dataInfo = dataInfo;
			this.insertedToRight = insertedToRight;
		}
		protected SNDataInfo DataInfo { get { return dataInfo; } }
		protected bool InsertedToRight { get { return insertedToRight; } }
		protected internal virtual void UpdateDataRowTemplate(TableCell cell, DocumentLogPosition pos) {
			PieceTable pieceTable = cell.PieceTable;
			SnapDocumentModel model = (SnapDocumentModel)pieceTable.DocumentModel;
			UpdateDataRowTemplate(pieceTable, model, pos);
		}
		protected internal virtual void UpdateDataRowTemplate(PieceTable pieceTable, SnapDocumentModel hostingModel, DocumentLogPosition pos) {
			object dataSource = DataInfo.Source;
			string dataMember = DataInfo.Member;
			if (dataSource is ParametersDataSource) {
				string fieldName = DataMemberInfo.Create(DataInfo.DataPaths).ColumnName;
				InsertParameterAsSnTextField(pieceTable, pos, fieldName);
			}
			else {
				string fieldName = DataMemberInfo.Create(DataInfo.EscapedDataPaths).ColumnName;
				IDataSourceDisplayNameProvider displayNameProvider = hostingModel.GetService<IDataSourceDisplayNameProvider>();
				using (DataContext context = new SnapDataContext(hostingModel.DataSourceDispatcher.GetCalculatedFields(dataSource), hostingModel.Parameters, displayNameProvider)) {
					DataBrowser dataBrowser = context.GetDataBrowser(dataSource, dataMember, true);
					if (dataBrowser == null)
						return;
					InsertSNField(pieceTable, pos, dataBrowser, fieldName);
				}
			}
		}
		void InsertParameterAsSnTextField(PieceTable pieceTable, DocumentLogPosition pos, string fieldName) {
			string fieldCode = string.Format("{0} {1} \\{2}", SNTextField.FieldType, InstructionController.GetEscapedArgument(fieldName), SNTextField.ParameterSwitch);
			pieceTable.InsertText(pos, fieldCode);
			pieceTable.CreateField(pos, fieldCode.Length);
		}
		protected internal override void UpdateTableRowForTemplate(TableRow row, SnapBookmark bookmark) {
			TableCell cell = GetCell(row);
			if (!processedDataRowBookmarks.Contains(bookmark) && ShouldUpdateContent(row, bookmark)) {
				UpdateRowContent(bookmark, cell, row);
				processedDataRowBookmarks.Add(bookmark);
			}
			else
				MergeInsertedCell(cell);
		}
		protected void MergeInsertedCell(TableCell cell) {
			TableCell mergedCell = InsertedToRight && !cell.IsFirstCellInRow ? cell.Previous : cell;
			int count = mergedCell.ColumnSpan + mergedCell.Next.ColumnSpan;
			PieceTableMergeTableCellsHorizontallyCommand command = new PieceTableMergeTableCellsHorizontallyCommand(mergedCell.PieceTable, mergedCell, count);
			command.Execute();
		}
		protected override Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions() {
			Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> result = new Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>>();
			result.Add(SnapTemplateIntervalType.DataRow, UpdateDataRowTemplate);
			result.Add(SnapTemplateIntervalType.ListHeader, UpdateListHeaderTemplate);
			result.Add(SnapTemplateIntervalType.GroupHeader, UpdateGroupHeaderTemplate);
			result.Add(SnapTemplateIntervalType.GroupFooter, UpdateGroupFooterTemplate);
			return result;
		}
		protected internal virtual void UpdateListHeaderTemplate(TableCell cell, DocumentLogPosition pos) {
			listHeaderUpdated = UpdateListHeaderTemplateCore(cell, pos);
		}
		protected internal virtual bool UpdateListHeaderTemplateCore(TableCell cell, DocumentLogPosition pos) {
			cell.PieceTable.InsertText(pos, FieldPathService.DecodePath(DataInfo.DisplayName));
			return true;
		}
		protected internal virtual void UpdateGroupHeaderTemplate(TableCell cell, DocumentLogPosition pos) {
			InsertDataInfoHeader(cell, pos);
		}
		protected internal virtual void UpdateGroupFooterTemplate(TableCell cell, DocumentLogPosition pos) {
			InsertDataInfoHeader(cell, pos);
		}
		void InsertDataInfoHeader(TableCell cell, DocumentLogPosition pos) {
			if (!listHeaderUpdated)
				cell.PieceTable.InsertText(pos, FieldPathService.DecodePath(dataInfo.DisplayName));
		}
		protected abstract void InsertSNField(PieceTable pieceTable, DocumentLogPosition logPosition, DataBrowser dataBrowser, string fieldName);
	}
	#endregion
	#region InsertColumnTemplateUpdater
	public class InsertColumnTemplateUpdater : RowTemplateDataUpdater {
		int columnSpanAfterInsertedCell;
		DocumentModel headerTemplate;
		DocumentModel bodyTemplate;
		public InsertColumnTemplateUpdater(TableCell cell, SNDataInfo dataInfo, bool insertedToRight)
			: base(cell, dataInfo, insertedToRight) {
			this.columnSpanAfterInsertedCell = CalculateColumnSpanAfterInsertedCell(cell);
			PrepareTemplates(cell, dataInfo);
		}
		protected internal virtual void PrepareTemplates(TableCell cell, SNDataInfo dataInfo) {
			SnapDocumentModel targetModel = (SnapDocumentModel)cell.DocumentModel;
			TableRow firstRow = cell.Table.Rows.First;
			SnapBookmark firstRowBookmark = TableRowHelper.FindTemplateBookmark(firstRow);
			while (!ShouldUpdateContent(firstRow, firstRowBookmark)) {
				firstRow = firstRow.Next;
				firstRowBookmark = TableRowHelper.FindTemplateBookmark(firstRow);
			}
			if (firstRowBookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.ListHeader) {
				headerTemplate = targetModel.CreateNew();
				headerTemplate.MainPieceTable.InsertText(headerTemplate.MainPieceTable.DocumentStartLogPosition, FieldPathService.DecodePath(dataInfo.DisplayName));
			}
			else
				headerTemplate = null;
			bodyTemplate = targetModel.CreateNew();
			UpdateDataRowTemplate(bodyTemplate.MainPieceTable, (SnapDocumentModel)cell.DocumentModel, DocumentLogPosition.Zero);
			targetModel.RaisePrepareSnListColumns(new PrepareSnListColumnsEventArgs(headerTemplate, bodyTemplate));
		}
		protected internal override bool UpdateListHeaderTemplateCore(TableCell cell, DocumentLogPosition pos) {
			if (headerTemplate == null)
				return false;
			CopyModel(cell, pos, headerTemplate);
			return true;
		}
		protected internal override void UpdateDataRowTemplate(TableCell cell, DocumentLogPosition pos) { CopyModel(cell, pos, bodyTemplate); }
		protected internal virtual int CalculateColumnSpanAfterInsertedCell(TableCell cell) {
			TableRow row = cell.Row;
			int columnIndex = cell.IndexInRow;
			int columnSpanAfterInsertedCell = row.GridAfter;
			for (int i = row.Cells.Count - 1; i > columnIndex; i--)
				columnSpanAfterInsertedCell += row.Cells[i].ColumnSpan;
			return columnSpanAfterInsertedCell;
		}
		protected override bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark) {
			return TableRowHelper.CheckColumnSpans(row, ColumnSpansTemplate);
		}
		protected override TableCell GetCell(TableRow row) {
			int currentSpan = row.GridAfter;
			TableCellCollection cells = row.Cells;
			int count = cells.Count - 1;
			for (int i = count; i > 0; i--) {
				TableCell cell = cells[i];
				if (currentSpan >= this.columnSpanAfterInsertedCell)
					return cell;
				currentSpan += cell.ColumnSpan;
			}
			return row.FirstCell;
		}
		protected override void InsertSNField(PieceTable pieceTable, DocumentLogPosition logPosition, DataBrowser dataBrowser, string fieldName) {
			string fieldText = GetSNFieldText(dataBrowser);
			string fieldCode = String.Format("{0} {1}", fieldText, InstructionController.GetEscapedArgument(fieldName));
			pieceTable.InsertText(logPosition, fieldCode);
			pieceTable.CreateField(logPosition, fieldCode.Length);
		}
	}
	#endregion
	#region InsertColumnCommandTemplateUpdater
	public class InsertColumnCommandTemplateUpdater : InsertColumnTemplateUpdater {
		public InsertColumnCommandTemplateUpdater(TableCell cell, bool insertedToRight)
			: base(cell, new SNDataInfo(null, string.Empty), insertedToRight) {
		}
		protected internal override void PrepareTemplates(TableCell cell, SNDataInfo dataInfo) { }
		protected internal override void UpdateDataRowTemplate(TableCell cell, DocumentLogPosition pos) { }
		protected internal override bool UpdateListHeaderTemplateCore(TableCell cell, DocumentLogPosition pos) {
			return true;
		}
		protected internal override void UpdateTableRowForTemplate(TableRow row, SnapBookmark bookmark) {
			TableCell cell = GetCell(row);
			if(cell == null)
				return;
			if(!ShouldMergeCell(cell, bookmark)) {
				UpdateRowContent(bookmark, cell, row);
				return;
			}
			MergeInsertedCell(cell);
		}
		TableCell CorrectCell(TableCell cell) {
			TableRow row = cell.Row;
			if(InsertedToRight) {
				if(Object.ReferenceEquals(cell, row.FirstCell) && row.Cells.Count > 1)
					return row.Cells[1];
				else
					return cell.Previous;
			}
			return cell.NextCellInRow;
		}
		bool ShouldMergeCell(TableCell resultCell, SnapBookmark bookmark) {
			TableCell cell = CorrectCell(resultCell);
			if(cell == null)
				return false;
			SnapPieceTable pieceTable = (SnapPieceTable)cell.PieceTable;
			Paragraph startParag = pieceTable.Paragraphs[cell.StartParagraphIndex];
			Paragraph endParag = pieceTable.Paragraphs[cell.EndParagraphIndex];
			Field field = pieceTable.FindFieldByRunIndex(startParag.FirstRunIndex);
			SnapFieldInfo snapField = new SnapFieldInfo((SnapPieceTable)field.PieceTable, field);
			if(!(snapField.ParsedInfo is SNListField))
				return false;
			var startLogPos = DocumentModelPosition.FromRunStart(pieceTable, snapField.Field.Result.Start);
			var endLogPos = DocumentModelPosition.FromRunEnd(pieceTable, snapField.Field.Result.End);
			if(!(startParag.LogPosition <= startLogPos.LogPosition && (endLogPos.LogPosition - 1) <= endParag.LogPosition))
				return false;
			return true;
		}
	}
	#endregion
	#region SummaryTemplateUpdater
	public class SummaryTemplateUpdater : RowTemplateDataUpdater {
		string running;
		SummaryItemType summaryItemType;
		string stringFormat;
		public SummaryTemplateUpdater(TableCell cell, SNDataInfo info, SummaryItemType summaryItemType, string running, string stringFormat)
			: base(cell, info, false) {
			this.summaryItemType = summaryItemType;
			this.running = running;
			this.stringFormat = stringFormat;
		}
		protected internal override void InitTableRows(TableCell cell) {
			Rows = new List<TableRow>();
			TableRow row = cell.Table.LastRow;
			if (running == "report") {
				Rows.Add(row);
				return;
			}
			while (row != null) {
				SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(row);
				if (bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.GroupFooter) {
					Rows.Add(row);
					return;
				}
				row = row.Previous;
			}
			Rows.Add(cell.Table.LastRow);
		}
		protected override bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark) {
			SnapTemplateIntervalType templateType = bookmark.TemplateInterval.TemplateInfo.TemplateType;
			return templateType == SnapTemplateIntervalType.ListFooter || templateType == SnapTemplateIntervalType.GroupFooter;
		}
		protected override Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions() {
			Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> result = new Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>>();
			result.Add(SnapTemplateIntervalType.ListFooter, UpdateDataRowTemplate);
			result.Add(SnapTemplateIntervalType.GroupFooter, UpdateDataRowTemplate);
			return result;
		}
		protected override void InsertSNField(PieceTable pieceTable, DocumentLogPosition logPosition, DataBrowser dataBrowser, string fieldName) {
			string fieldCode = SNTextField.GetSummaryFieldCode(fieldName, running, summaryItemType) + " ";
			if (summaryItemType != SummaryItemType.Count)
				fieldCode += String.Format("\\{0} {1} ", SNTextField.FrameworkStringFormatSwitch, stringFormat);
			pieceTable.InsertText(logPosition, fieldCode);
			pieceTable.CreateField(logPosition, fieldCode.Length - 1);
			pieceTable.InsertText(logPosition, SNTextField.GetDisplaySummaryString(DataInfo.DisplayName, summaryItemType));
		}
		protected override TableCell GetCell(TableRow row) {
			return row.FirstCell;
		}
		protected override DocumentLogPosition GetDocumentLogPosition(TableCell cell) {
			return cell.PieceTable.GetRunInfoByTableCell(cell).End.LogPosition;
		}
	}
	#endregion
	#region DeleteColumnTemplateUpdater
	public class DeleteColumnTemplateUpdater : RowTemplateUpdaterBase {
		IRichEditControl control;
		int indexInRow;
		public DeleteColumnTemplateUpdater(TableCell cell, IRichEditControl control)
			: base(cell) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.indexInRow = cell.IndexInRow;
			SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(cell.Row);
			if (bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.DataRow)
				InitTableRows(cell);
		}
		protected internal override void InitTableRows(TableCell cell) {
			Rows = new List<TableRow>();
			if (TableRowHelper.CheckColumnSpans(cell.Table.FirstRow, ColumnSpansTemplate))
				Rows.Add(cell.Table.FirstRow);
			if (TableRowHelper.CheckColumnSpans(cell.Table.LastRow, ColumnSpansTemplate))
				Rows.Add(cell.Table.LastRow);
		}
		protected override Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions() {
			Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> result = new Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>>();
			result.Add(SnapTemplateIntervalType.ListHeader, UpdateListHeaderFooter);
			result.Add(SnapTemplateIntervalType.ListFooter, UpdateListHeaderFooter);
			return result;
		}
		protected override TableCell GetCell(TableRow row) {
			return row.Cells[indexInRow];
		}
		protected override bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark) {
			SnapTemplateIntervalType type = bookmark.TemplateInterval.TemplateInfo.TemplateType;
			return type == SnapTemplateIntervalType.ListHeader || type == SnapTemplateIntervalType.ListFooter;
		}
		protected void UpdateListHeaderFooter(TableCell cell, DocumentLogPosition pos) {
			DeleteCell(cell);
		}
		void DeleteCell(TableCell cell) {
			TableCell modifiedCell = GetModifiedTableCell(cell);
			PieceTable pieceTable = cell.PieceTable;
			pieceTable.DeleteTableCellWithContent(cell, control.InnerDocumentServer.Owner);
			if (modifiedCell != null)
				TableCommandsHelper.InsertSeparators(modifiedCell.PieceTable, new List<TableCell>() { modifiedCell });
		}
		TableCell GetModifiedTableCell(TableCell cell) {
			DocumentModel model = cell.DocumentModel;
			model.BeginUpdate();
			try {
				SnapObjectModelController modelController = new SnapObjectModelController((SnapPieceTable)cell.PieceTable);
				if (!(modelController.GetFirstCellRun(cell) is SeparatorTextRun))
					return null;
				SnapBookmarkController bookmarkController = new SnapBookmarkController((SnapPieceTable)cell.PieceTable);
				return bookmarkController.IsTableCellLastInTemplateBookmark(cell) ? null : cell.Next;
			}
			finally {
				model.EndUpdate();
			}
		}
	}
	#endregion
	#region InsertDetailListTemplateUpdater
	public class InsertDetailListTemplateUpdater : RowTemplateUpdaterBase {
		DocumentModel templateModel;
		TableRow row;
		List<SnapBookmark> processedDataRowBookmarks = new List<SnapBookmark>();
		public InsertDetailListTemplateUpdater(TableCell cell, SNDataInfo[] dataInfo)
			: base(cell) {
			this.row = cell.Row;
			PrepareTemplate(cell, dataInfo);
		}
		protected TableRow Row { get { return row; } }
		protected override Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions() {
			Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> result = new Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>>();
			result.Add(SnapTemplateIntervalType.DataRow, InsertSnListDetailCore);
			return result;
		}
		protected internal virtual void PrepareTemplate(TableCell cell, SNDataInfo[] dataInfo) {
			this.templateModel = CreateDataRowTemplate(cell, dataInfo);
			((SnapDocumentModel)cell.DocumentModel).RaisePrepareSnListDetail(new PrepareSnListDetailEventArgs(templateModel));
		}
		protected override TableCell GetCell(TableRow row) {
#if DEBUG || DEBUGTEST
			Debug.Assert(row.Cells.Count == 1);
#endif
			return row.FirstCell;
		}
		public override void UpdateTemplates(bool forceUpdateRows) {
			UpdateTemplate(Row);
			if (forceUpdateRows)
				UpdateRows();
		}
		protected internal override void UpdateTableRowForTemplate(TableRow row, SnapBookmark bookmark) {
			if(!processedDataRowBookmarks.Contains(bookmark) && ShouldUpdateContent(row, bookmark)) {
				UpdateRowContent(bookmark, GetCell(row), row);
				processedDataRowBookmarks.Add(bookmark);
			}
		}
		protected override bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark) {
			return bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.DataRow;
		}
		protected internal virtual DocumentModel CreateDataRowTemplate(TableCell cell, SNDataInfo[] dataInfo) {
			TableCellHelper.PrepareMasterCell(cell);
			SnapPieceTable pieceTable = (SnapPieceTable)cell.PieceTable;
			SnapDocumentModel sourceModel = pieceTable.DocumentModel;
			DocumentLogPosition pos = pieceTable.GetRunInfoByTableCell(cell).Start.LogPosition;
			TemplateBuilder templateBuilder = sourceModel.CreateTemplateBuilder();
			DocumentModel template = templateBuilder.CreateTemplateFromDraggedDataInfo(pieceTable, pos, dataInfo);
			template.InheritDataServices(sourceModel);
			TableCommandsHelper.InsertHeader((SnapPieceTable)template.MainPieceTable, sourceModel, pos, dataInfo);
			return template;
		}
		protected void InsertSnListDetailCore(TableCell cell, DocumentLogPosition pos) {
			CopyModel(cell, pos, this.templateModel);
		}
	}
	#endregion
	#region ResizeGroupHeaderFooterTemplateUpdater
	public class ResizeGroupHeaderFooterTemplateUpdater : RowTemplateUpdaterBase {
		IRichEditControl control;
		readonly TableRow patternRow;
		WidthUnitInfo preferredWidthInfo;
		int startCellIndex;
		int endCellIndex;
		int cellCount;
		List<PreferredWidth> cellsWidths;
		public ResizeGroupHeaderFooterTemplateUpdater(SelectedCellsIntervalInRow interval, IRichEditControl control)
			: base(interval.StartCell) {
			this.control = control;
			this.patternRow = interval.StartCell.Row;
			this.startCellIndex = interval.StartCellIndex;
			this.endCellIndex = interval.EndCellIndex;
			this.cellCount = interval.Row.Cells.Count;
			this.cellsWidths = new List<PreferredWidth>();
			for (int i = 0; i < cellCount; i++)
				cellsWidths.Add(interval.Row.Cells[i].PreferredWidth);
		}
		protected TableRow PatternRow { get { return patternRow; } }
		protected override Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> CreateUpdateTemplateActions() {
			Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>> result = new Dictionary<SnapTemplateIntervalType, Action<TableCell, DocumentLogPosition>>();
			result.Add(SnapTemplateIntervalType.GroupHeader, UpdatePreferredWidth);
			result.Add(SnapTemplateIntervalType.GroupFooter, UpdatePreferredWidth);
			result.Add(SnapTemplateIntervalType.GroupSeparator, UpdatePreferredWidth);
			result.Add(SnapTemplateIntervalType.Separator, UpdatePreferredWidth);
			result.Add(SnapTemplateIntervalType.DataRow, UpdateDataRow);
			return result;
		}
		protected override TableCell GetCell(TableRow row) {
			return row.FirstCell;
		}
		protected override bool ShouldUpdateContent(TableRow row, SnapBookmark bookmark) {
			if (Object.ReferenceEquals(this.preferredWidthInfo, null))
				return false;
			SnapTemplateIntervalType type = bookmark.TemplateInterval.TemplateInfo.TemplateType;
			if (type == SnapTemplateIntervalType.DataRow && row.Cells.Count == cellCount) {
				for (int i = 0; i < cellCount; i++) {
					if (row.Cells[i].PreferredWidth.Value != cellsWidths[i].Value)
						return false;
				}
				return true;
			}
			return IsCorrespondsType(type) && row.Cells.Count == 1;
		}
		bool IsCorrespondsType(SnapTemplateIntervalType type) {
			return type == SnapTemplateIntervalType.GroupHeader ||
				type == SnapTemplateIntervalType.GroupFooter ||
				type == SnapTemplateIntervalType.GroupSeparator ||
				type == SnapTemplateIntervalType.Separator;
		}
		protected internal override void PrepareUpdateInfo() {
			if (PatternRow.FirstCell.PreferredWidth.Type != WidthUnitType.ModelUnits)
				return;
			int value = 0;
			for (int i = 0; i < PatternRow.Cells.Count; i++) {
				if (PatternRow.Cells[i].PreferredWidth.Type == WidthUnitType.ModelUnits)
					value += PatternRow.Cells[i].PreferredWidth.Value;
			}
			this.preferredWidthInfo = new WidthUnitInfo(WidthUnitType.ModelUnits, value);
		}
		protected void UpdatePreferredWidth(TableCell cell, DocumentLogPosition pos) {
			cell.Properties.PreferredWidth.CopyFrom(preferredWidthInfo);
		}
		protected void UpdateDataRow(TableCell cell, DocumentLogPosition pos) {
			for (int i = endCellIndex; i >= startCellIndex; i--)
				DeleteCell(cell.Row.Cells[i]);
		}
		void DeleteCell(TableCell cell) {
			TableCell modifiedCell = GetModifiedTableCell(cell);
			PieceTable pieceTable = cell.PieceTable;
			pieceTable.DeleteTableCellWithContent(cell, control.InnerDocumentServer.Owner);
			if (modifiedCell != null)
				TableCommandsHelper.InsertSeparators(modifiedCell.PieceTable, new List<TableCell>() { modifiedCell });
		}
		TableCell GetModifiedTableCell(TableCell cell) {
			DocumentModel model = cell.DocumentModel;
			model.BeginUpdate();
			try {
				SnapObjectModelController modelController = new SnapObjectModelController((SnapPieceTable)cell.PieceTable);
				if (!(modelController.GetFirstCellRun(cell) is SeparatorTextRun))
					return null;
				SnapBookmarkController bookmarkController = new SnapBookmarkController((SnapPieceTable)cell.PieceTable);
				return bookmarkController.IsTableCellLastInTemplateBookmark(cell) ? null : cell.Next;
			}
			finally {
				model.EndUpdate();
			}
		}
	}
	#endregion
}
