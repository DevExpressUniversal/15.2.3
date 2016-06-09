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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native {
	public class SnapObjectModelController {
		readonly SnapPieceTable pieceTable;
		public SnapObjectModelController(SnapPieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public SnapPieceTable PieceTable { get { return pieceTable; } }
		public SnapDocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public SnapBookmark GetSelectedBookmark() {
			Selection selection = PieceTable.DocumentModel.Selection;
			Debug.Assert(Object.Equals(selection.PieceTable, PieceTable));
			SnapBookmarkController controller = new SnapBookmarkController(PieceTable);
			return controller.FindInnermostTemplateBookmarkByPosition(selection.Start);
		}
		public static bool IsBookmarkCorrespondsToListHeaderFooter(SnapBookmark bookmark) {
			if (Object.ReferenceEquals(bookmark, null))
				return false;
			SnapTemplateIntervalType type = bookmark.TemplateInterval.TemplateInfo.TemplateType;
			return type == SnapTemplateIntervalType.ListHeader || type == SnapTemplateIntervalType.ListFooter;
		}
		public static bool IsBookmarkCorrespondsToGroup(SnapBookmark bookmark) {
			if (Object.ReferenceEquals(bookmark, null))
				return false;
			SnapTemplateIntervalType type = bookmark.TemplateInterval.TemplateInfo.TemplateType;
			return type == SnapTemplateIntervalType.GroupHeader || type == SnapTemplateIntervalType.GroupFooter || type == SnapTemplateIntervalType.GroupSeparator;
		}
		public static bool IsBookmarkCorrespondsToDataRow(SnapBookmark bookmark) {
			if (Object.ReferenceEquals(bookmark, null))
				return false;
			return bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.DataRow;
		}
		public TextRunBase FindRunByLogPosition(DocumentLogPosition pos) {
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(PieceTable, pos);
			return PieceTable.Runs[modelPos.RunIndex];
		}
		public string GetRunPlainText(DocumentLogPosition pos) {
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(PieceTable, pos);
			return PieceTable.GetRunPlainText(modelPos.RunIndex);
		}
		public DocumentLogPosition FindCellStartLogPosition(TableCell cell) {
			return PieceTable.GetRunInfoByTableCell(cell).Start.LogPosition;
		}
		public DocumentLogPosition FindCellEndLogPosition(TableCell cell) {
			return PieceTable.GetRunInfoByTableCell(cell).End.LogPosition;
		}
		public DocumentLogPosition FindCellNormalizedStartLogPosition(TableCell cell) {
			return PieceTable.GetRunInfoByTableCell(cell).NormalizedStart.LogPosition;
		}
		public DocumentLogPosition FindCellNormalizedEndLogPosition(TableCell cell) {
			return PieceTable.GetRunInfoByTableCell(cell).NormalizedEnd.LogPosition;
		}
		public SnapFieldInfo FindFieldInfoByLogPosition(DocumentLogPosition pos) {
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(PieceTable, pos);
			Field field = PieceTable.FindFieldByRunIndex(modelPos.RunIndex);
			if (field == null)
				return null;
			return new SnapFieldInfo(PieceTable, field);
		}
		public TableCell FindCellByLogPosition(DocumentLogPosition pos) {
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(PieceTable, pos);
			return PieceTable.Paragraphs[modelPos.ParagraphIndex].GetCell();
		}
		public SnapBookmark GetNextBookmark(SnapBookmark bookmark) {
			SnapDocumentModel model = (SnapDocumentModel)bookmark.DocumentModel;
			foreach (var item in model.MainPieceTable.SnapBookmarks) {
				if (bookmark.Parent == item.Parent && bookmark.End == item.Start)
					return item;
			}
			return null;
		}
		public TextRunBase GetFirstCellRun(TableCell cell) {
			RunInfo info = PieceTable.GetRunInfoByTableCell(cell);
			return PieceTable.Runs[info.Start.RunIndex];
		}
		public Section GetSectionByRun(TextRunBase run) {
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(run.Paragraph.Index);
			return PieceTable.DocumentModel.Sections[sectionIndex];
		}
		public TableCell FindTableCellBySeparator() {
			SnapBookmarkController bookmarkController = new SnapBookmarkController(PieceTable);
			SnapBookmark bookmark = bookmarkController.FindInnermostTemplateBookmarkByPosition(PieceTable.DocumentModel.Selection.Start);
			if (bookmark != null) {
#if DEBUGTEST || DEBUG
				SnapTemplateIntervalType type = bookmark.TemplateInterval.TemplateInfo.TemplateType;
				Debug.Assert(type == SnapTemplateIntervalType.GroupSeparator || type == SnapTemplateIntervalType.Separator);
#endif
				bookmark = bookmarkController.FindInnermostTemplateBookmarkByPosition(bookmark.Start - 1);
			}
			if (bookmark != null)
				return FindCellByLogPosition(bookmark.Start);
			return null;
		}
		public bool IsWholeContentSelected() {
			Selection selection = DocumentModel.Selection;
			PieceTable activePieceTable = DocumentModel.ActivePieceTable;
			DocumentLogPosition start = DocumentModelPosition.FromParagraphStart(activePieceTable, activePieceTable.Paragraphs.First.Index).LogPosition;
			DocumentLogPosition end = DocumentModelPosition.FromParagraphEnd(activePieceTable, activePieceTable.Paragraphs.Last.Index).LogPosition;
			return start == selection.NormalizedStart && end == selection.NormalizedEnd;
		}
		public Pair<string, int> GetTag(SnapTemplateInfo info, SnapListFieldInfo listFieldInfo) {
			if (info.TemplateType == SnapTemplateIntervalType.GroupHeader || info.TemplateType == SnapTemplateIntervalType.GroupFooter) {
				int groupIndex = listFieldInfo.ParsedInfo.GetGroupIndex(DocumentModel, info.FirstGroupIndex);
				return new Pair<string, int>(listFieldInfo.ParsedInfo.Name, groupIndex);
			}
			else {
				return new Pair<string, int>(listFieldInfo.ParsedInfo.Name, -1);
			}
		}
		public Field GetTopLevelListFieldBySelection() {
			SnapListFieldInfo fieldInfo = new ListFieldSelectionController(DocumentModel).FindListField();
			if (fieldInfo == null)
				return null;
			Field result = null;
			Field field = fieldInfo.Field;
			while (field != null) {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField parsedInfo = calculator.ParseField(PieceTable, field) as SNListField;
				if (parsedInfo != null)
					result = field;
				field = field.Parent;
			}
			return result;
		}
	}
}
