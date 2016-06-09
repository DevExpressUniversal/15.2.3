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
using DevExpress.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Model {
	#region DeletedFieldsInfo
	public class DeletedFieldsInfo {
		#region Fields
		Field leftField;
		Field rightField;
		readonly List<Field> deletedFields;
		#endregion
		public DeletedFieldsInfo() {
			this.deletedFields = new List<Field>();
		}
		#region Properties
		public Field RightField { get { return rightField; } set { rightField = value; } }
		public Field LeftField { get { return leftField; } set { leftField = value; } }
		public IList<Field> DeletedFields { get { return deletedFields; } }
		#endregion
		public bool IsAnyFieldAffected() {
			return RightField != null || LeftField != null || DeletedFields.Count > 0;
		}
	}
	#endregion
	#region Old DeleteContentOperation
	#endregion
	#region DeleteContentOperationBase (abstract class)
	public abstract class DeleteContentOperationBase {
		#region Fields
		bool deletingResult;
		readonly PieceTable pieceTable;
		bool allowPartiallyDeletingField;
		#endregion
		protected DeleteContentOperationBase(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public bool AllowPartiallyDeletingField { get { return allowPartiallyDeletingField; } set { allowPartiallyDeletingField = value; } }
		#endregion
		public bool Execute(DocumentLogPosition startLogPosition, int length, bool documentLastParagraphSelected) {
			if (length <= 0)
				return false;
			RunInfo runInfo = PieceTable.ObtainAffectedRunInfo(startLogPosition, length);
			this.deletingResult = false;
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				BeforeExecute(runInfo);
				ExecuteCore(runInfo, documentLastParagraphSelected);
				AfterExecute();
			}
			return this.deletingResult;
		}
		protected internal virtual void BeforeExecute(RunInfo runInfo) {
		}
		protected internal virtual void AfterExecute() {
		}
		protected internal abstract void ExecuteCore(RunInfo runInfo, bool documentLastParagraphSelected);
		protected internal virtual void DeleteContent(DocumentModelPosition start, DocumentModelPosition end, bool documentLastParagraphSelected) {
			DocumentLogPosition startPosition = start.LogPosition;
			int length = end.LogPosition - startPosition + 1;
			DeleteContent(startPosition, length, documentLastParagraphSelected);
		}
		protected internal virtual void DeleteContent(DocumentLogPosition startPosition, int length, bool documentLastParagraphSelected) {
			DeleteSectionOperation operation = CreateDeleteSectionOperation();
			this.deletingResult |= operation.Execute(startPosition, length, documentLastParagraphSelected);
		}
		protected virtual DeleteSectionOperation CreateDeleteSectionOperation() {
			return new DeleteSectionOperation(PieceTable);
		}
	}
	#endregion
	#region DeleteContentOperation
	public class DeleteContentOperation : DeleteContentOperationBase {
		bool leaveFieldIfResultIsRemoved;
		bool suppressFieldDelete;
		bool forceRemoveInnerFields;
		bool backspacePressed;
		public DeleteContentOperation(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal bool LeaveFieldIfResultIsRemoved { get { return leaveFieldIfResultIsRemoved; } set { leaveFieldIfResultIsRemoved = value; } }
		protected internal bool SuppressFieldDelete { get { return suppressFieldDelete; } set { suppressFieldDelete = value; } }
		protected internal bool BackspacePressed { get { return backspacePressed; } set { backspacePressed = value; } }
		public bool ForceRemoveInnerFields { get { return forceRemoveInnerFields; } set { forceRemoveInnerFields = value; } } 
		protected internal override void ExecuteCore(RunInfo runInfo, bool documentLastParagraphSelected) {
			if (!suppressFieldDelete)
				if (DeleteFields(runInfo, documentLastParagraphSelected))
					return;
			DeleteContent(runInfo.Start, runInfo.End, documentLastParagraphSelected);
		}
		protected virtual bool DeleteFields(RunInfo runInfo, bool documentLastParagraphSelected) {
			Field leftField = PieceTable.FindFieldByRunIndex(runInfo.Start.RunIndex);
			Field rightField = PieceTable.FindFieldByRunIndex(runInfo.End.RunIndex);
			DeleteContentWithFieldsOperation operation = new DeleteContentWithFieldsOperation(PieceTable);
			operation.AllowPartiallyDeletingField = AllowPartiallyDeletingField;
			operation.ForceRemoveInnerFields = ForceRemoveInnerFields;
			if (leftField != null || rightField != null) {
				if (Object.ReferenceEquals(leftField, rightField) && ShouldDeleteField(runInfo, leftField))
					ExtendDeletedInterval(runInfo, leftField);
				else {
					operation.ExecuteCore(runInfo, documentLastParagraphSelected);
					return true;
				}
			}
			operation.DeleteFieldsWithinInterval(runInfo);
			return false;
		}
		bool ShouldDeleteField(RunInfo runInfo, Field field) {
			return FieldsOperation.IsEntireFieldResultAffectd(runInfo, field) && !LeaveFieldIfResultIsRemoved;
		}
		void ExtendDeletedInterval(RunInfo runInfo, Field field) {
			FieldsOperation.EnsureIntervalContainsField(runInfo, field);
		}
		protected internal override void BeforeExecute(RunInfo runInfo) {
			base.BeforeExecute(runInfo);
			DeleteBookmarks(runInfo);
		}
		protected internal virtual void DeleteBookmarks(RunInfo runInfo) {
			DeleteBookmarksCore(PieceTable.Bookmarks, runInfo, ShouldDeleteBookmark);
			DeleteBookmarksCore(PieceTable.RangePermissions, runInfo, CanDeleteRangePermission);
			DeleteBookmarksCore(PieceTable.Comments, runInfo, ShouldDeleteBookmark);
		}
		protected internal delegate bool CanDeleteBookmarkDelegate(DocumentLogPosition start, DocumentLogPosition end, BookmarkBase bookmark);
		protected internal virtual void DeleteBookmarksCore<T>(BookmarkBaseCollection<T> bookmarks, RunInfo runInfo, CanDeleteBookmarkDelegate canDeleteBookmark)
			where T : BookmarkBase, IDocumentModelStructureChangedListener {
			DocumentLogPosition start = runInfo.Start.LogPosition;
			DocumentLogPosition end = runInfo.End.LogPosition + 1;
			for (int i = bookmarks.Count - 1; i >= 0; i--) {
				BookmarkBase bookmark = bookmarks[i];
				if (canDeleteBookmark(start, end, bookmark))
					bookmark.Delete(i);
			}
		}
		protected internal virtual bool ShouldDeleteBookmark(DocumentLogPosition start, DocumentLogPosition end, BookmarkBase bookmark) {
			if (bookmark.CanExpand)
				return bookmark.Start >= start && bookmark.End <= end;
			else
				return bookmark.Start > start && bookmark.End < end;
		}
		protected internal virtual bool CanDeleteRangePermission(DocumentLogPosition start, DocumentLogPosition end, BookmarkBase bookmark) {
			return bookmark.Start > start && bookmark.End < end;
		}
		protected override DeleteSectionOperation CreateDeleteSectionOperation() {
			DeleteSectionOperation result = base.CreateDeleteSectionOperation();
			result.BackspacePressed = BackspacePressed;
			return result;
		}
	}
	#endregion
	#region DeleteContentWithFieldsOperation
	public class DeleteContentWithFieldsOperation : DeleteContentOperationBase {
		bool forceRemoveInnerFields;
		public DeleteContentWithFieldsOperation(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public bool ForceRemoveInnerFields { get { return forceRemoveInnerFields; } set { forceRemoveInnerFields = value; } }
		protected internal override void ExecuteCore(RunInfo runInfo, bool documentLastParagraphSelected) {
			DeleteFieldsWithinInterval(runInfo);
			DeleteContentWithFieldsCore(runInfo.Start, runInfo.End, documentLastParagraphSelected);
		}
		protected internal virtual void DeleteFieldsWithinInterval(RunInfo runInfo) {
			IList<Field> deletedFields = CalculateDeletedFields(runInfo);
			int count = deletedFields.Count;
			for (int i = 0; i < count; i++)
				PieceTable.RemoveField(deletedFields[i]);
		}
		protected internal virtual IList<Field> CalculateDeletedFields(RunInfo runInfo) {
			List<Field> result = new List<Field>();
			for (int index = PieceTable.Fields.Count - 1; index >= 0; index--) {
				Field field = PieceTable.Fields[index];
				bool fieldInInterval = IsFieldWithinInterval(runInfo, field);
				bool fieldShouldBeRemove = forceRemoveInnerFields || !IsFieldHidByParent(field) || IsParentShouldBeRemoved(field, runInfo);
				if (fieldInInterval && fieldShouldBeRemove)
					result.Add(field);
			}
			return result;
		}
		protected internal virtual bool IsFieldWithinInterval(RunInfo runInfo, Field field) {
			RunIndex startIndex = runInfo.Start.RunIndex;
			RunIndex endIndex = runInfo.End.RunIndex;
			return field.FirstRunIndex >= startIndex && field.LastRunIndex <= endIndex;
		}
		protected internal virtual bool IsParentShouldBeRemoved(Field field, RunInfo runInfo) {
			return field.Parent != null && IsFieldWithinInterval(runInfo, field.Parent);
		}
		protected internal virtual bool IsFieldHidByParent(Field field) {
			Field parent = field.Parent;
			if (parent != null) {
				FieldRunInterval invisibleInterval = parent.IsCodeView ? parent.Result : parent.Code;
				return invisibleInterval.Contains(field);
			}
			return false;
		}
		protected internal virtual void DeleteContentWithFieldsCore(DocumentModelPosition start, DocumentModelPosition end, bool documentLastParagraphSelected) {
			if (AllowPartiallyDeletingField) { 
				DeleteContent(start, end, documentLastParagraphSelected);
				return;
			}
			RunIndex runIndex = end.RunIndex;
			DocumentLogPosition logPosition = end.LogPosition;
			while (runIndex >= start.RunIndex) {
				int length = 0;
				while (runIndex >= start.RunIndex && !ShouldSkipRun(runIndex)) {
					length += PieceTable.Runs[runIndex].Length;
					runIndex--;
				}
				if (length > 0) {
					DeleteContent(logPosition - length + 1, length, documentLastParagraphSelected);
					logPosition -= length;
				}
				else {
					logPosition -= PieceTable.Runs[runIndex].Length;
					runIndex--;
				}
			}
		}
		protected internal virtual bool ShouldSkipRun(RunIndex runIndex) {
			Field field = PieceTable.FindFieldByRunIndex(runIndex);
			if (field == null)
				return false;
			bool isFieldMarkRun = field.Code.Start == runIndex || field.Code.End == runIndex || field.Result.End == runIndex;
			if (isFieldMarkRun || IsFieldHidByParent(field))
				return true;
			FieldRunInterval invisibleInterval = field.IsCodeView ? field.Result : field.Code;
			return invisibleInterval.Contains(runIndex);
		}
	}
	#endregion
}
