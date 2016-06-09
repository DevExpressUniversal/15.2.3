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
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office {
	public static class XmlTextHelper {
		public static string DeleteIllegalXmlCharacters(string value) {
			int count = value.Length;
			for (int i = count - 1; i >= 0; i--)
				if (!IsLegalXmlChar(value[i]))
					value = value.Remove(i, 1);
			return value;
		}
		public static bool IsLegalXmlChar(char character) {
			return character >= 0x20;
		}
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	public static class DocumentFormatsHelper {
		public static bool ShouldExportSectionColumns(SectionColumns columns, DocumentModel documentModel) {
			ColumnsInfo defaultColumns = documentModel.Cache.ColumnsInfoCache.DefaultItem;
			return
				columns.EqualWidthColumns != defaultColumns.EqualWidthColumns ||
				columns.ColumnCount != defaultColumns.ColumnCount ||
				columns.Space != defaultColumns.Space ||
				columns.DrawVerticalSeparator != defaultColumns.DrawVerticalSeparator;
		}
#region DocumentCapability Helpers
		public static bool ShouldInsertHyperlink(DocumentModel model) {
			return model.DocumentCapabilities.HyperlinksAllowed;
		}
		public static bool ShouldInsertPicture(DocumentModel model) {
			return model.DocumentCapabilities.InlinePicturesAllowed;
		}
		public static bool ShouldInsertHyperlinks(DocumentModel model) {
			return model.DocumentCapabilities.HyperlinksAllowed;
		}
		public static bool ShouldInsertBookmarks(DocumentModel model) {
			return model.DocumentCapabilities.BookmarksAllowed;
		}
		public static bool ShouldApplyCharacterStyle(DocumentModel model) {
			return model.DocumentCapabilities.CharacterStyleAllowed;
		}
		public static bool ShouldApplyParagraphStyle(DocumentModel model) {
			return model.DocumentCapabilities.ParagraphStyleAllowed;
		}
		public static bool ShouldInsertParagraph(DocumentModel model) {
			return model.DocumentCapabilities.ParagraphsAllowed;
		}
#endregion
#region Numbering Lists
		public static bool ShouldInsertNumbering(DocumentModel model) {
			NumberingOptions options = model.DocumentCapabilities.Numbering;
			return options.BulletedAllowed || options.SimpleAllowed || ShouldInsertMultiLevelNumbering(model);
		}
		public static bool ShouldInsertMultiLevelNumbering(DocumentModel model) {
			return model.DocumentCapabilities.Numbering.MultiLevelAllowed;
		}
		public static bool ShouldInsertBulletedNumbering(DocumentModel model) {
			return model.DocumentCapabilities.Numbering.BulletedAllowed;
		}
		public static bool ShouldInsertSimpleNumbering(DocumentModel model) {
			return model.DocumentCapabilities.Numbering.SimpleAllowed;
		}
		public static bool NeedReplaceSimpleToBulletNumbering(DocumentModel model) {
			return !ShouldInsertSimpleNumbering(model) && ShouldInsertBulletedNumbering(model);
		}
		public static bool NeedReplaceBulletedLevelsToDecimal(DocumentModel model) {
			return !ShouldInsertBulletedNumbering(model) && ShouldInsertSimpleNumbering(model);
		}
#endregion
	}
#region ImportBookmarkInfoCore
	public class ImportBookmarkInfoCore {
		DocumentLogPosition start = new DocumentLogPosition(-1);
		DocumentLogPosition end = new DocumentLogPosition(-1);
		public DocumentLogPosition Start { get { return start; } set { start = value; } }
		public DocumentLogPosition End { get { return end; } set { end = value; } }
		public virtual bool Validate(PieceTable pieceTable) {
			if (start < DocumentLogPosition.Zero || end < DocumentLogPosition.Zero)
				return false;
			if (start > end) {
				DocumentLogPosition temp = start;
				start = end;
				end = temp;
			}
			DocumentLogPosition maxPosition = pieceTable.DocumentEndLogPosition + 1;
			if (start > maxPosition)
				start = maxPosition;
			if (end > maxPosition)
				end = maxPosition;
			return true;
		}
	}
#endregion
#region ImportBookmarkInfo
	public class ImportBookmarkInfo : ImportBookmarkInfoCore {
		string name;
		public string Name { get { return name; } set { name = value; } }
		public override bool Validate(PieceTable pieceTable) {
			if (String.IsNullOrEmpty(name))
				return false;
			if (pieceTable.Bookmarks.FindByName(Name) != null)
				return false;
			return base.Validate(pieceTable);
		}
	}
#endregion
#region ImportRangePermissionInfo
	public class ImportRangePermissionInfo : ImportBookmarkInfoCore {
		RangePermissionInfo permissionInfo = new RangePermissionInfo();
		public RangePermissionInfo PermissionInfo { get { return permissionInfo; } }
		public override bool Validate(PieceTable pieceTable) {
			if (String.IsNullOrEmpty(PermissionInfo.UserName) && String.IsNullOrEmpty(PermissionInfo.Group))
				return false;
			if (End - Start <= 0)
				return false;
			return base.Validate(pieceTable);
		}
	}
#endregion
#region ImportCommentInfo
	public class ImportCommentInfo : ImportBookmarkInfoCore {
#region Fields
		Comment comment;
		DocumentLogPosition reference = new DocumentLogPosition(-1);
#endregion
		public ImportCommentInfo(PieceTable pieceTable) : base ()  {
			comment = new Comment(pieceTable, DocumentLogPosition.Zero, DocumentLogPosition.Zero, null);
			comment.Date = Comment.MinCommentDate;
		}		
#region Properties
		public string Name { get { return comment.Name; } set { comment.Name = value; } }
		public string Author { get { return comment.Author; } set { comment.Author = value; } }
		public DateTime Date { get { return comment.Date; } set { comment.Date = value; } }
		public CommentContentType Content { get { return comment.Content; } set { comment.Content = value; } }
		protected internal Comment Comment { get { return comment; } set { comment = value; } }
		public Int32 ParaId { get; set; }
		public bool HasReference { get; set; }
		public DocumentLogPosition Reference { get { return reference; } set { reference = value; } }
#endregion
		public virtual void CalculateCommentPosition() {
			if (Reference < DocumentLogPosition.Zero)
				return;
			if (Start < DocumentLogPosition.Zero)
				Start = CalculateCommentStart();
			if (End < DocumentLogPosition.Zero)
				End = CalculateCommentEnd();
		}
		DocumentLogPosition CalculateCommentStart() {
			if (End < DocumentLogPosition.Zero)
				return Reference;
			else
				return End;
		}
		DocumentLogPosition CalculateCommentEnd() {
			if (Start < DocumentLogPosition.Zero)
				return Reference;
			else
				return Start;
		}
		public override bool Validate(PieceTable pieceTable) {
			if(Name == null || Author == null ||  !HasReference ||  Content == null  )
				return false;
			return base.Validate(pieceTable);
		}
	}
#endregion
#region RTFImportCommentInfo
	public class RTFImportCommentInfo : ImportCommentInfo {
		public RTFImportCommentInfo(PieceTable pieceTable) : base(pieceTable) { }
		public string Id { get; set; }
		public override void CalculateCommentPosition() {
			if (String.IsNullOrEmpty(Id))
				base.CalculateCommentPosition();
		}
	}
#endregion
#region ImportFieldInfo
	public class ImportFieldInfo {
		Field field;
		RunIndex codeStartIndex;
		RunIndex codeEndIndex;
		RunIndex resultEndIndex;
		public ImportFieldInfo(PieceTable pieceTable) {
			this.field = CreateField(pieceTable);
		}
		public RunIndex CodeStartIndex { get { return codeStartIndex; } set { codeStartIndex = value; } }
		public RunIndex CodeEndIndex { get { return codeEndIndex; } set { codeEndIndex = value; } }
		public RunIndex ResultEndIndex { get { return resultEndIndex; } set { resultEndIndex = value; } }
		public bool DisableUpdate { get; set; }
		public bool Locked { get; set; }
		public Field Field { get { return field; } set { field = value; } }
		protected virtual Field CreateField(PieceTable pieceTable) {
			return new Field(pieceTable);
		}
	}
#endregion
#region ImportFieldHelper
	public class ImportFieldHelper {
		readonly PieceTable pieceTable;
		public ImportFieldHelper(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public void ProcessFieldBegin(ImportFieldInfo fieldInfo, InputPosition position) {
			fieldInfo.CodeStartIndex = PieceTable.InsertFieldCodeStartRunCore(position);
		}
		public void ProcessFieldSeparator(ImportFieldInfo fieldInfo, InputPosition position) {
			fieldInfo.CodeEndIndex = PieceTable.InsertFieldCodeEndRunCore(position);
		}
		public Field ProcessFieldEnd(ImportFieldInfo fieldInfo, InputPosition position) {
			if (fieldInfo.CodeEndIndex <= fieldInfo.CodeStartIndex) 
				fieldInfo.CodeEndIndex = PieceTable.InsertFieldCodeEndRunCore(position);
			fieldInfo.ResultEndIndex = PieceTable.InsertFieldResultEndRunCore(position);
			Field field = fieldInfo.Field;
			field.DisableUpdate = fieldInfo.DisableUpdate;
			field.Locked = fieldInfo.Locked;
			field.Code.SetInterval(fieldInfo.CodeStartIndex, fieldInfo.CodeEndIndex);
			field.Result.SetInterval(fieldInfo.CodeEndIndex + 1, fieldInfo.ResultEndIndex);
			FieldCollection fields = PieceTable.Fields;
			field.Index = fields.Count;
			fields.Add(field);
			return field;
		}
		public void InsertHyperlinkInstruction(HyperlinkInfo info, InputPosition position) {
			HyperlinkInstructionBuilder builder = new HyperlinkInstructionBuilder(info);
			PieceTable.InsertTextCore(position, builder.GetFieldInstruction());
		}
	}
#endregion
#region ImportInputPosition
	public class ImportInputPosition : InputPosition {
#region Fields
		readonly CharacterFormattingBase paragraphMarkCharacterFormatting;
		readonly ParagraphFormattingBase paragraphFormatting;
		readonly ParagraphFrameFormattingBase paragraphFrameFormatting;
		readonly TabFormattingInfo paragraphTabs;
		int paragraphStyleIndex;
		int paragraphMarkCharacterStyleIndex;
		#endregion
		public ImportInputPosition(PieceTable pieceTable)
			: base(pieceTable) {
			this.paragraphMarkCharacterFormatting = new CharacterFormattingBase(pieceTable, pieceTable.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.paragraphFormatting = new ParagraphFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.paragraphFrameFormatting = new ParagraphFrameFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFrameFormattingInfoCache.DefaultItemIndex, ParagraphFrameFormattingOptionsCache.EmptyParagraphFrameFormattingOptionIndex);
			this.paragraphTabs = new TabFormattingInfo();
		}
#region Properties
		public CharacterFormattingBase ParagraphMarkCharacterFormatting { get { return paragraphMarkCharacterFormatting; } }
		public ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		public ParagraphFrameFormattingBase ParagraphFrameFormatting { get { return paragraphFrameFormatting; } }
		public TabFormattingInfo ParagraphTabs { get { return paragraphTabs; } }
		public int ParagraphMarkCharacterStyleIndex { get { return paragraphMarkCharacterStyleIndex; } set { paragraphMarkCharacterStyleIndex = value; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } set { paragraphStyleIndex = value; } }
		public int TableCellStyleIndex { get; set; }
		#endregion
	}
#endregion
#region ImportPieceTableInfoBase (abstract class)
	public abstract class ImportPieceTableInfoBase<TImportCommentInfo> where TImportCommentInfo : ImportCommentInfo {
#region Fields
		readonly PieceTable pieceTable;
		readonly Dictionary<string, ImportBookmarkInfo> bookmarks;
		readonly Dictionary<string, ImportRangePermissionInfo> rangePermissions;
		readonly Dictionary<string, TImportCommentInfo> comments;
		ImportCommentInfo activeComment;
#endregion
		protected ImportPieceTableInfoBase(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.bookmarks = new Dictionary<string, ImportBookmarkInfo>();
			this.rangePermissions = new Dictionary<string, ImportRangePermissionInfo>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.comments = new Dictionary<string, TImportCommentInfo>();
		}
		protected ImportPieceTableInfoBase(PieceTable pieceTable, Dictionary<string, ImportBookmarkInfo> bookmarks, Dictionary<string, ImportRangePermissionInfo> rangePermissions, Dictionary<string, TImportCommentInfo> comments, ImportCommentInfo activeComment) {
			this.pieceTable = pieceTable;
			this.bookmarks = bookmarks;
			this.rangePermissions = rangePermissions;
			this.comments = comments;
			this.activeComment = activeComment; 
		}
#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public Dictionary<string, ImportBookmarkInfo> Bookmarks { get { return bookmarks; } }
		public Dictionary<string, ImportRangePermissionInfo> RangePermissions { get { return rangePermissions; } }
		public Dictionary<string, TImportCommentInfo> Comments { get { return comments; } }
		public ImportCommentInfo ActiveComment { get { return activeComment; } set { activeComment = value; } }
#endregion
	}
#endregion
#region ImportPieceTableInfo
	public class ImportPieceTableInfo : ImportPieceTableInfoBase<ImportCommentInfo> {
#region Fields
		readonly Stack<Table> tableStack;
		readonly Stack<ImportFieldInfo> fieldInfoStack;
		readonly ImportInputPosition position;
#endregion
		public ImportPieceTableInfo(PieceTable pieceTable)
			: base(pieceTable) {
			this.position = new ImportInputPosition(pieceTable);
			this.fieldInfoStack = new Stack<ImportFieldInfo>();
			this.tableStack = new Stack<Table>();
		}
#region Properties
		public ImportInputPosition Position { get { return position; } }
		public Stack<ImportFieldInfo> FieldInfoStack { get { return fieldInfoStack; } }
		public Stack<Table> TableStack { get { return tableStack; } }
#endregion
	}
#endregion
#region RichEditDocumentModelImporter (abstract class)
	public abstract class RichEditDocumentModelImporter : DocumentModelImporter {
		readonly DocumentImporterOptions options;
		protected RichEditDocumentModelImporter(DocumentModel documentModel, DocumentImporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected internal DocumentImporterOptions InnerOptions { get { return options; } }
	}
#endregion
	public abstract class RichEditDestinationAndXmlBasedImporter : DestinationAndXmlBasedImporter {
		readonly XmlBasedDocumentImporterOptions options;
		readonly Stack<ImportPieceTableInfo> pieceTableInfos;
		readonly CommentExListInfoCollection commentExListInfos;
		readonly CommentListInfoCollection commentListInfos;
		Section currentSection;
		protected RichEditDestinationAndXmlBasedImporter(DocumentModel documentModel, XmlBasedDocumentImporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.pieceTableInfos = new Stack<ImportPieceTableInfo>();
			this.commentExListInfos = new CommentExListInfoCollection();
			this.commentListInfos = new CommentListInfoCollection();
			PushCurrentPieceTable(documentModel.MainPieceTable);
			ResetPositionCharacterProperties();
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected internal XmlBasedDocumentImporterOptions InnerOptions { get { return options; } }
		protected Stack<ImportPieceTableInfo> PieceTableInfos { get { return pieceTableInfos; } }
		protected ImportPieceTableInfo PieceTableInfo { get { return pieceTableInfos.Peek(); } }
		public PieceTable PieceTable { get { return PieceTableInfo.PieceTable; } }
		public Section CurrentSection { get { return currentSection; } set { currentSection = value; } }
		public ImportInputPosition Position { get { return PieceTableInfo.Position; } }
		public Stack<ImportFieldInfo> FieldInfoStack { get { return PieceTableInfo.FieldInfoStack; } }
		public Dictionary<string, ImportBookmarkInfo> Bookmarks { get { return PieceTableInfo.Bookmarks; } }
		public Dictionary<string, ImportRangePermissionInfo> RangePermissions { get { return PieceTableInfo.RangePermissions; } }
		public Dictionary<string, ImportCommentInfo> Comments { get { return PieceTableInfo.Comments; } }
		public Stack<Table> TableStack { get { return PieceTableInfo.TableStack; } }
		protected override bool IgnoreParseErrors { get { return InnerOptions.IgnoreParseErrors; } }
		public CommentExListInfoCollection CommentExListInfos { get { return commentExListInfos; } }
		public CommentListInfoCollection CommentListInfos { get { return commentListInfos; } }
		protected internal virtual void ResetPositionCharacterProperties() {
			Position.CharacterFormatting.ReplaceInfo(DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			Position.CharacterStyleIndex = 0;
		}
		protected override void BeforeImportMainDocument() {
			base.BeforeImportMainDocument();
			this.CurrentSection = DocumentModel.Sections.First;
		}
		protected override void AfterImportMainDocument() {
			base.AfterImportMainDocument();
			PieceTable.FixLastParagraph();
			InsertBookmarks();
			InsertRangePermissions();
			InsertComments();
			PieceTable.FixTables();
		}
		protected internal virtual void PushCurrentPieceTable(PieceTable pieceTable) {
			pieceTableInfos.Push(new ImportPieceTableInfo(pieceTable));
		}
		protected internal virtual PieceTable PopCurrentPieceTable() {
			return pieceTableInfos.Pop().PieceTable;
		}
		public override void BeginSetMainDocumentContent() {
			DocumentModel.BeginSetContent();
		}
		public override void EndSetMainDocumentContent() {
			DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, true, options.UpdateField.GetNativeOptions());
		}
		public override void SetMainDocumentEmptyContent() {
			DocumentModel.ClearDocumentCore(true, true);
		}
#region Bookmarks
		protected internal virtual void InsertBookmarks() {
			if (!DocumentModel.DocumentCapabilities.BookmarksAllowed)
				return;
			foreach (string id in Bookmarks.Keys) {
				ImportBookmarkInfo bookmark = Bookmarks[id];
				if (bookmark.Validate(PieceTable))
					PieceTable.CreateBookmarkCore(bookmark.Start, bookmark.End - bookmark.Start, bookmark.Name);
			}
		}
#endregion
#region Range Permissions
		protected internal void InsertRangePermissions() {
			foreach (string id in RangePermissions.Keys) {
				ImportRangePermissionInfo rangePermission = RangePermissions[id];
				if (rangePermission.Validate(PieceTable))
					PieceTable.ApplyDocumentPermission(rangePermission.Start, rangePermission.End, rangePermission.PermissionInfo);
			}
		}
#endregion
#region Comments
		protected internal void InsertComments() {
			foreach (string id in Comments.Keys) {
				ImportCommentInfo comment = Comments[id];
				comment.CalculateCommentPosition();
				if (comment.Validate(PieceTable)) {
					Comment parentComment = FindParentComment(comment);
					comment.Comment = PieceTable.CreateCommentCore(comment.Start, comment.End - comment.Start, comment.Name, comment.Author,  comment.Date, parentComment, comment.Content);
				}
			}
		}
		Comment FindParentComment(ImportCommentInfo comment) {
			CommentListInfo listInfo = new CommentListInfo();
			if (comment.ParaId > 0) {
				CommentExListInfo commentExlistInfo = new CommentExListInfo();
				commentExlistInfo = CommentExListInfos.FindByParaId(comment.ParaId);
				if (commentExlistInfo != null && commentExlistInfo.ParaIdParent > 0) {
					listInfo = CommentListInfos.FindByParaId(commentExlistInfo.ParaIdParent);
					if (listInfo != null)
						return Comments[listInfo.Id].Comment;
				}
				return null;
			}
			return null;
		}
#endregion
	}
#region ExportedTableInfo
	public class ImportedTableInfo : TableInfo {
		readonly CellsRowSpanCollection cellsRowSpanCollection;
		int columnIndex;
		public ImportedTableInfo(Table table)
			: base(table) {
			this.cellsRowSpanCollection = new CellsRowSpanCollection();
		}
		public CellsRowSpanCollection CellsRowSpanCollection { get { return cellsRowSpanCollection; } }
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public void MoveNextRow() {
			columnIndex = 0;
		}
		public virtual void AddCellToSpanCollection(int rowSpan, int columnSpan) {			
			AddCellToSpanCollectionCore(columnIndex, rowSpan, columnSpan);
		}
		public virtual void ExpandSpanCollection(int rowSpan, int columnSpan) {
			int columnIndex = ColumnIndex;
			if (columnIndex >= CellsRowSpanCollection.Count)
				CellsRowSpanCollection.Add(rowSpan);
			int newCount = ColumnIndex + columnSpan - CellsRowSpanCollection.Count;
			for (int i = 0; i < newCount; i++)
				CellsRowSpanCollection.Add(0);
		}
		internal virtual int AddCellToSpanCollectionCore(int columnIndex, int rowSpan, int columnSpan) {
			CellsRowSpanCollection.Add(rowSpan);
			for (int i = columnSpan; i > 1; i--) {
				CellsRowSpanCollection.Add(0);
				columnIndex++;
			}
			return columnIndex;
		}
	}
#endregion
#region CellsRowSpanCollection
	public class CellsRowSpanCollection : List<int> {
#if DEBUGTEST
		public override string ToString() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < Count; i++) {
				sb.Append(String.Format("{0} ", this[i].ToString()));
			}
			return sb.ToString();
		}
#endif
	}
#endregion
#region TablesImportHelper (abstract class)
	public abstract class TablesImportHelper {
		readonly PieceTable pieceTable;
		readonly Stack<ImportedTableInfo> tableStack;
		ImportedTableInfo topLevelTableInfo;
		protected TablesImportHelper(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.tableStack = new Stack<ImportedTableInfo>();
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public Table Table { get { return (tableStack.Count == 0) ? null : tableStack.Peek().Table; } }
		public virtual ImportedTableInfo TableInfo { get { return (tableStack.Count == 0) ? null : tableStack.Peek(); } }
		public virtual ImportedTableInfo TopLevelTableInfo { get { return topLevelTableInfo; } }
		internal int TableStackCount { get { return tableStack.Count; } }
		public bool IsInTable { get { return TableStackCount > 0; } }
		public bool TablesAllowed {
			get {
				DocumentCapabilitiesOptions options = pieceTable.DocumentModel.DocumentCapabilities;
				return options.TablesAllowed && options.ParagraphsAllowed;
			}
		}
		protected abstract ImportedTableInfo CreateTableInfo(Table newTable);
#region Row
		public TableRow CreateNewRow() {
			TablesCapabilityCheck();
			TableRow row = new TableRow(Table);
			row.Table.Rows.AddInternal(row);
			MoveNextRow();
			InsertCoveredByVerticalMergingCellsInRow();
			return row;
		}
		public TableRow CreateNewRowOrGetLastEmpty() {
			TableRow result = null;
			if (IsEmptyLastRow())
				result = Table.LastRow;
			else
				result = CreateNewRow();
			return result;
		}
		bool IsEmptyLastRow() {
			bool noRows = Table.Rows.Count == 0;
			if (noRows)
				return false;
			bool noCells = Table.LastRow.Cells.Count == 0;
			return noCells;
		}
		public virtual void MoveNextRow() {
			TableInfo.ColumnIndex = 0;
		}
		public void RemoveEmptyRow() {
			TableRow row = Table.LastRow;
			if (row.Cells.Count == 0) {
				row.Table.Rows.RemoveInternal(row);
				SkipFakeRow();
			}
		}
		void SkipFakeRow() {
			this.TableInfo.ColumnIndex = 0;
		}
		void InsertCoveredByVerticalMergingCellsInRow() {
			if (Table.Rows.Count == 1 || TableInfo.CellsRowSpanCollection.Count == 0)
				return;
			TablesCapabilityCheck();
			int cellsInPreviousRowCount = TableInfo.CellsRowSpanCollection.Count;
			int columnIndex = TableInfo.ColumnIndex;
			while (columnIndex < cellsInPreviousRowCount && TableInfo.CellsRowSpanCollection[columnIndex] > 1)
				columnIndex = CreateCoveredInVerticalMergeCell(columnIndex);
			TableInfo.ColumnIndex = Math.Min(columnIndex, cellsInPreviousRowCount);
		}
		internal void TablesCapabilityCheck() {
			Debug.Assert(IsInTable && Table != null);
			DocumentCapabilitiesOptions options = Table.DocumentModel.DocumentCapabilities;
			Debug.Assert(options.TablesAllowed);
			Debug.Assert(options.ParagraphsAllowed);
		}
#endregion
#region Cell
		public virtual void InitializeTableCell(TableCell cell, ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
			ParagraphIndex invalid = ParagraphIndex.Zero - 1;
			Debug.Assert(startParagraphIndex != invalid && endParagraphIndex != invalid);
			if (cell != null && startParagraphIndex != invalid && endParagraphIndex != invalid)
				PieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, endParagraphIndex);
		}
		int CreateCoveredInVerticalMergeCells(int nextColumnIndex) {
			TablesCapabilityCheck();
			int result = nextColumnIndex;
			while (result < TableInfo.CellsRowSpanCollection.Count
				&& TableInfo.CellsRowSpanCollection[result] > 1)
				result = CreateCoveredInVerticalMergeCell(result);
			return result;
		}
		TableCell FindPatternCellWithMergeRestartInPreviousRows(TableRow row, int columnIndex) {
			TableCell findedCell = Table.GetCell(row, columnIndex);
			if (findedCell == null)
				return null;
			if (findedCell.VerticalMerging != MergingState.Restart)
				findedCell = FindPatternCellWithMergeRestartInPreviousRows(row.Previous, columnIndex);
			return findedCell;
		}
		protected TableRow GetRowBeforeLastRow(Table table) {
			TableRowCollection rows = table.Rows;
			int count = rows.Count;
			if (count > 1)
				return rows[count - 2];
			else
				return null;
		}
		protected internal virtual int CreateCoveredInVerticalMergeCell(int columnIndex) {
			TablesCapabilityCheck();
			TableCell covered = CreateCoveredCellWithEmptyParagraph(Table.LastRow);
			Debug.Assert(columnIndex == Table.GetCellColumnIndexConsiderRowGrid(covered));
			TableCell patternCell = FindPatternCellWithMergeRestartInPreviousRows(GetRowBeforeLastRow(Table), columnIndex);
			if (patternCell != null)
				covered.Properties.CopyFrom(patternCell.Properties);
			covered.Properties.VerticalMerging = MergingState.Continue;
			Debug.Assert(TableInfo.CellsRowSpanCollection.Count > columnIndex);
			if (TableInfo.CellsRowSpanCollection.Count <= columnIndex)
				return columnIndex;
			TableInfo.CellsRowSpanCollection[columnIndex]--;
			int nextColumnIndex = columnIndex + 1;
			while (nextColumnIndex < TableInfo.CellsRowSpanCollection.Count
					&& TableInfo.CellsRowSpanCollection[nextColumnIndex] == 0) {
				nextColumnIndex++;
			}
			if(nextColumnIndex < TableInfo.CellsRowSpanCollection.Count) {
				int expectedNextColumnIndex = columnIndex + covered.ColumnSpan;
				if (expectedNextColumnIndex < nextColumnIndex) {
					TableCell horizontalCoveredCell = CreateCoveredCellWithEmptyParagraph(Table.LastRow);
					horizontalCoveredCell.ColumnSpan = nextColumnIndex - expectedNextColumnIndex;
				}
			}
			return nextColumnIndex;
		}
		protected internal abstract TableCell CreateCoveredCellWithEmptyParagraph(TableRow lastRow);
		public virtual void UpdateFirstRowSpanCollection(int rowSpan, int columnSpan) {
			TablesCapabilityCheck();
			if (Table.Rows.Count > 1)
				UpdateFirstRowSpanCollectionCore(rowSpan, columnSpan);
		}
		void UpdateFirstRowSpanCollectionCore(int rowSpan, int columnSpan) {
			int columnIndex = TableInfo.ColumnIndex;
			int nextColumnIndex = columnIndex + 1;
			int cellCollectionCount = TableInfo.CellsRowSpanCollection.Count;
			if (cellCollectionCount == 0) {
				TableInfo.ColumnIndex = 0;
				return;
			}
			if (cellCollectionCount <= columnIndex)		  
				for (int i = 0; i < columnIndex - cellCollectionCount + 1; i++)
					TableInfo.CellsRowSpanCollection.Add(1);
			if (TableInfo.CellsRowSpanCollection[columnIndex] < 2)
				TableInfo.CellsRowSpanCollection[columnIndex] = rowSpan;
			if (nextColumnIndex == TableInfo.CellsRowSpanCollection.Count) {
				if (columnSpan > 1) 
					TableInfo.ColumnIndex = ProcessLastCellInRow(rowSpan, columnSpan, columnIndex);
				else
					TableInfo.ColumnIndex = nextColumnIndex;
			}
			else if (TableInfo.CellsRowSpanCollection[nextColumnIndex] > 1) {
				int indexOfLastCell = Table.GetCellColumnIndexConsiderRowGrid(Table.LastRow.LastCell) + Table.LastRow.LastCell.ColumnSpan;
				if (indexOfLastCell >= nextColumnIndex) {
					Table.LastRow.LastCell.ColumnSpan = 1;
				}
				TableInfo.ColumnIndex = CreateCoveredInVerticalMergeCells(nextColumnIndex);
			}
			else {
				TableInfo.ColumnIndex = ProcessUnCoveredCell(rowSpan, columnSpan, columnIndex);
				TableInfo.ColumnIndex = CreateCoveredInVerticalMergeCells(TableInfo.ColumnIndex);
			}
		}
		int ProcessUnCoveredCell(int rowSpan, int columnSpan, int columnIndex) {
			int localColumnIndex = columnIndex;
			int upperBounds = TableInfo.CellsRowSpanCollection.Count - 1;
			for (int i = columnSpan; i > 1 && localColumnIndex < upperBounds; i--) {
				localColumnIndex++;
				TableInfo.CellsRowSpanCollection[localColumnIndex] = 0;
			}
			return localColumnIndex + 1;
		}
		int ProcessLastCellInRow(int rowSpan, int columnSpan, int localColumnIndex) {
			for (int i = columnSpan; i > 1 && localColumnIndex < TableInfo.CellsRowSpanCollection.Count; i--) {
				TableInfo.CellsRowSpanCollection[localColumnIndex] = 0;
				localColumnIndex++;
			}
			return localColumnIndex;
		}
		public void AddCellToSpanCollection(int rowSpan, int columnSpan) {
			TablesCapabilityCheck();
			TableInfo.AddCellToSpanCollection(rowSpan, columnSpan);
		}
		public void ExpandSpanCollection(int rowSpan, int columnSpan) {
			TableInfo.ExpandSpanCollection(rowSpan, columnSpan);
		}
		protected internal virtual ParagraphIndex FindStartParagraphIndexForCell(ParagraphIndex inputPositionPragraphIndex) {
			Table table = Table;
			ParagraphIndex lastCellParagraphIndex = ParagraphIndex.Zero;
			if (table.LastRow.Cells.Count == 1) {				
				TableRow previous = GetRowBeforeLastRow(table);
				if (previous == null)
					return inputPositionPragraphIndex;
				lastCellParagraphIndex = previous.LastCell.EndParagraphIndex;
			}
			else
				lastCellParagraphIndex = table.LastRow.Cells[table.LastRow.Cells.Count - 2].EndParagraphIndex;
			if (inputPositionPragraphIndex > lastCellParagraphIndex + 1)
				return lastCellParagraphIndex + 1;
			return inputPositionPragraphIndex;
		}
#endregion
#region Table
		public Table CreateTable(TableCell parent) {
			Table table = new Table(PieceTable, parent, 0, 0);
			PushTable(table);
			PieceTable.Tables.Add(table);
			return table;
		}
		public virtual void FinalizeTableCreation() {
			TablesCapabilityCheck();
			if (Table.Rows.Count == 0 || Table.Rows.First.Cells.Count == 0)
				PieceTable.Tables.Remove(Table.Index);
			else {
				CloseUnclosedLastRowLastCell(Table, pieceTable.Paragraphs.Last.Index - 1);
				PostProcessTableCells();
				RemoveEmptyRows(Table);
				FixBrokenCells(Table);
			}
			PopTable();
		}
		protected internal virtual void CloseUnclosedLastRowLastCell(Table table, ParagraphIndex lastParagraphIndex) {
			ParagraphIndex invalid = ParagraphIndex.Zero - 1;
			TableCell lastCell = FindLastValidCellInTable(table);
			if (lastCell == null)
				return;
			if (lastCell.EndParagraphIndex == invalid) {
				if (lastParagraphIndex > ParagraphIndex.Zero) {
					lastCell.EndParagraphIndex = lastParagraphIndex;
					InitializeTableCell(lastCell, lastCell.StartParagraphIndex, lastCell.EndParagraphIndex);
				}
			}
		}
		TableCell FindLastValidCellInTable(Table table) {
			TableRowCollection rows = table.Rows;
			int count = rows.Count;
			int rowIndex = count - 1;			
			while (rowIndex >= 0 && rows[rowIndex].Cells.Count == 0) {
				rowIndex--;
			}
			return (rowIndex < 0 || rows[rowIndex].Cells.Count == 0) ?
				null :
				rows[rowIndex].LastCell;
		}
		protected internal virtual void RemoveEmptyRows(Table currentTable) {
			int rowsCount = currentTable.Rows.Count;
			for (int rowId = rowsCount - 1; rowId >= 0; rowId--) {
				TableRow row = currentTable.Rows[rowId];
				if (row.Cells.Count == 0) {
					currentTable.Rows.RemoveInternal(row);
				}
			}
			if (Table.Rows.Count == 0) {
			}
		}
		public virtual void PostProcessTableCells() {
			TableRowCollection rows = Table.Rows;
			int rowCount = rows.Count;
			ParagraphIndex invalidParagraphIndex = new ParagraphIndex(-1);
			for (int rowIndex = 0; rowIndex < rowCount - 1; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for (int cellIndex = cellCount - 1; cellIndex >= 0; cellIndex--) {
					TableCell cell = cells[cellIndex];
					if (cell.StartParagraphIndex == invalidParagraphIndex
						|| cell.EndParagraphIndex == invalidParagraphIndex)
						cells.DeleteInternal(cell);
				}
			}
		}
		public virtual void FixBrokenCells(Table currentTable) {
			int rowCount = currentTable.Rows.Count;
			if (rowCount < 1)
				return;
			SortedList<int> existingPositions = currentTable.GetExistingValidColumnsPositions();
			int expectedMaxColumnIndex = existingPositions[existingPositions.Count - 1];
			for (int i = 0; i < rowCount; i++) {
				TableRow row = currentTable.Rows[i];
				int maxColumnIndex = currentTable.GetTotalCellsInRowConsiderGrid(row);
				if (maxColumnIndex < expectedMaxColumnIndex) {
					int newEndColumnListIndex = existingPositions.BinarySearch(maxColumnIndex);
					if (newEndColumnListIndex < 0)
						newEndColumnListIndex = ~newEndColumnListIndex;
					int newEndColumnIndex = existingPositions[newEndColumnListIndex];
					if(newEndColumnIndex != maxColumnIndex)
						row.LastCell.ColumnSpan += newEndColumnIndex - maxColumnIndex;
					if (newEndColumnIndex != expectedMaxColumnIndex)
						row.GridAfter += expectedMaxColumnIndex - newEndColumnIndex;
				}
			}			
			Table.Normalize();
		}
		void PushTable(Table table) {
			ImportedTableInfo info = CreateTableInfo(table);
			tableStack.Push(info);
			if(tableStack.Count == 1)
				topLevelTableInfo = info;
		}
		internal void PopTable() {
			tableStack.Pop();
			if (tableStack.Count == 0)
				topLevelTableInfo = null;
		}
#endregion
	}
#endregion
	public static class RichEditDocumentFormatsDependecies {
		public static DocumentFormatsDependencies CreateDocumentFormatsDependecies() {
			IDocumentExportManagerService exportManagerService = new DocumentExportManagerService();
			IDocumentImportManagerService importManagerService = new DocumentImportManagerService();
			IDocumentExportersFactory exportersFactory = new DocumentExportersFactory();
			IDocumentImportersFactory importersFactory = new DocumentImportersFactory();
			return new DocumentFormatsDependencies(exportManagerService, importManagerService, exportersFactory, importersFactory);
		}
	}
	public class DocumentExportersFactory : IDocumentExportersFactory {
		public Export.Doc.IDocExporter CreateDocExporter(DocumentModel documentModel, Export.DocDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.Doc.DocExporter(documentModel, options);
		}
		public Export.Html.IHtmlExporter CreateHtmlExporter(DocumentModel documentModel, Export.HtmlDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.Html.HtmlExporter(documentModel, options);
		}
		public Export.Mht.IMhtExporter CreateMhtExporter(DocumentModel documentModel, Export.MhtDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.Mht.MhtExporter(documentModel, options);
		}
		public Export.OpenDocument.IOpenDocumentTextExporter CreateOpenDocumentTextExporter(DocumentModel documentModel, Export.OpenDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.OpenDocument.OpenDocumentTextExporter(documentModel, options);
		}
		public Export.OpenXml.IOpenXmlExporter CreateOpenXmlExporter(DocumentModel documentModel, Export.OpenXmlDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.OpenXml.OpenXmlExporter(documentModel, options);
		}
		public Export.PlainText.IPlainTextExporter CreatePlainTextExporter(DocumentModel documentModel, Export.PlainTextDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.PlainText.PlainTextExporter(documentModel, options);
		}
		public IRtfExporter CreateRtfExporter(DocumentModel documentModel, Export.RtfDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.Rtf.RtfExporter(documentModel, options); ;
		}
		public Export.WordML.IWordMLExporter CreateWordMLExporter(DocumentModel documentModel, Export.WordMLDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.WordML.WordMLExporter(documentModel, options); ;
		}
		public Export.Xaml.IXamlExporter CreateXamlExporter(DocumentModel documentModel, Export.XamlDocumentExporterOptions options) {
			return new DevExpress.XtraRichEdit.Export.Xaml.XamlExporter(documentModel, options); ;
		}
	}
	public class DocumentImportersFactory : IDocumentImportersFactory {
		public Import.Doc.IDocImporter CreateDocImporter(DocumentModel documentModel, Import.DocDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.Doc.DocImporter(documentModel, options);
		}
		public Import.Html.IHtmlImporter CreateHtmlImporter(DocumentModel documentModel, Import.HtmlDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.Html.HtmlImporter(documentModel, options);
		}
		public Import.Mht.IMhtImporter CreateMhtImporter(DocumentModel documentModel, Import.MhtDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.Mht.MhtImporter(documentModel, options);
		}
		public Import.OpenDocument.IOpenDocumentTextImporter CreateOpenDocumentTextImporter(DocumentModel documentModel, Import.OpenDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.OpenDocument.OpenDocumentTextImporter(documentModel, options);
		}
		public Import.OpenXml.IOpenXmlImporter CreateOpenXmlImporter(DocumentModel documentModel, Import.OpenXmlDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.OpenXml.OpenXmlImporter(documentModel, options);
		}
		public DevExpress.XtraRichEdit.Import.Rtf.IRtfImporter CreateRtfImporter(DocumentModel documentModel, Import.RtfDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.Rtf.RtfImporter(documentModel, options); ;
		}
		public Import.WordML.IWordMLImporter CreateWordMLImporter(DocumentModel documentModel, Import.WordMLDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.WordML.WordMLImporter(documentModel, options); ;
		}
		public Import.Xaml.IXamlImporter CreateXamlImporter(DocumentModel documentModel, Import.XamlDocumentImporterOptions options) {
			return new DevExpress.XtraRichEdit.Import.Xaml.XamlImporter(documentModel, options); ;
		}
	}
	public static class ShadingHelper {
		static Dictionary<ShadingPattern, int> patternMultipliers = CreatePatternMultiplierDictionary();
		static Dictionary<ShadingPattern, int> CreatePatternMultiplierDictionary() {
			Dictionary<ShadingPattern, int> result = new Dictionary<ShadingPattern, int>();
			result.Add(ShadingPattern.Clear, 0);
			result.Add(ShadingPattern.Pct5, 50);
			result.Add(ShadingPattern.Pct10, 100);
			result.Add(ShadingPattern.Pct12, 125);
			result.Add(ShadingPattern.Pct15, 150);
			result.Add(ShadingPattern.Pct20, 200);
			result.Add(ShadingPattern.Pct25, 250);
			result.Add(ShadingPattern.Pct30, 300);
			result.Add(ShadingPattern.Pct35, 350);
			result.Add(ShadingPattern.Pct37, 375);
			result.Add(ShadingPattern.Pct40, 400);
			result.Add(ShadingPattern.Pct45, 450);
			result.Add(ShadingPattern.Pct50, 500);
			result.Add(ShadingPattern.Pct55, 550);
			result.Add(ShadingPattern.Pct60, 600);
			result.Add(ShadingPattern.Pct62, 625);
			result.Add(ShadingPattern.Pct65, 650);
			result.Add(ShadingPattern.Pct70, 700);
			result.Add(ShadingPattern.Pct75, 750);
			result.Add(ShadingPattern.Pct80, 800);
			result.Add(ShadingPattern.Pct85, 850);
			result.Add(ShadingPattern.Pct87, 875);
			result.Add(ShadingPattern.Pct90, 900);
			result.Add(ShadingPattern.Pct95, 950);
			result.Add(ShadingPattern.Solid, 1000);
			return result;
		}
		internal static Color GetActualBackColor(Color fill, Color patternColor, ShadingPattern pattern) {
			if (pattern == ShadingPattern.Clear || pattern == ShadingPattern.Nil)
				return fill;
			if ((DXColor.IsTransparentOrEmpty(fill) || fill.ToArgb() == DXColor.White.ToArgb()) && DXColor.IsTransparentOrEmpty(patternColor)) {
				int multiplier;
				if (patternMultipliers.TryGetValue(pattern, out multiplier)) {
					int intensity = 255 * (1000 - multiplier) / 1000;
					return DXColor.FromArgb(intensity, intensity, intensity);
				}
			}
			return pattern != ShadingPattern.Solid ? fill : patternColor;
		}
	}
}
