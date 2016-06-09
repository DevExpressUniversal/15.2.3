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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows;
#endif
namespace DevExpress.XtraRichEdit.Model {
	public enum UpdateFieldResult {
		FieldNotUpdated,
		FieldUpdated,
		FieldUpdatedAndCodeDeleted
	}
	public class UpdateFieldOperationResult {
		public static UpdateFieldOperationResult DisabledUpdate = new UpdateFieldOperationResult(UpdateFieldResult.FieldNotUpdated, true);
		readonly UpdateFieldResult updateFieldResult;
		readonly bool suppressUpdateInnerCodeFields;
		public UpdateFieldOperationResult(UpdateFieldResult updateFieldResult, bool suppressUpdateInnerCodeFields) {
			this.updateFieldResult = updateFieldResult;
			this.suppressUpdateInnerCodeFields = suppressUpdateInnerCodeFields;
		}
		public UpdateFieldResult UpdateFieldResult { get { return updateFieldResult; } }
		public bool SuppressUpdateInnerCodeFields { get { return suppressUpdateInnerCodeFields; } }
	}
	public enum UpdateFieldOperationType {
		Load = 1,
		Copy = 2,
		Normal = 4,
		PasteFromIE = 8,
		CreateModelForExport = 16
	}
	#region UpdateFieldOperation
	public class UpdateFieldOperation {
		#region Fields
		readonly PieceTable pieceTable;
		readonly MailMergeDataMode mailMergeDataMode;
		Field field;
		DocumentModelPosition startResultPos;
		DocumentLogPosition endResultLogPos = new DocumentLogPosition(-1);
		#endregion
		public UpdateFieldOperation(PieceTable pieceTable, MailMergeDataMode mailMergeDataMode) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.mailMergeDataMode = mailMergeDataMode;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public MailMergeDataMode MailMergeDataMode { get { return mailMergeDataMode; } }
		#endregion
		public virtual UpdateFieldOperationResult Execute(Field field, UpdateFieldOperationType updateType) {
			try {
				CalculateFieldResult result = PieceTable.CalculateFieldResult(field, mailMergeDataMode, updateType);
				return InsertResult(field, result, updateType);
			}
			catch {
				return new UpdateFieldOperationResult(UpdateFieldResult.FieldNotUpdated, false);
			}
		}
		public UpdateFieldOperationResult InsertResult(Field field, CalculateFieldResult result, UpdateFieldOperationType updateType) {
			if(result == null)
				return new UpdateFieldOperationResult(UpdateFieldResult.FieldNotUpdated, false);
			if((updateType & result.UpdateType) == 0 || result.KeepOldResult)
				return new UpdateFieldOperationResult(UpdateFieldResult.FieldNotUpdated, false);
			if((result.Options & FieldResultOptions.HyperlinkField) != 0) {
				if(field.IsCodeView)
					PieceTable.ToggleFieldCodes(field);
				return new UpdateFieldOperationResult(UpdateFieldResult.FieldUpdated, (result.Options & FieldResultOptions.SuppressUpdateInnerCodeFields) != 0);
			}
			else {
				if(PieceTable.IsHyperlinkField(field))
					PieceTable.RemoveHyperlinkInfo(field.Index);
			}
			try {
				BeginFieldUpdate(field);
				try {
					UpdateFieldOperationResult updateResult = ReplaceFieldResult(result, updateType);
					if(updateResult.UpdateFieldResult != UpdateFieldResult.FieldUpdatedAndCodeDeleted && field.IsCodeView)
						PieceTable.ToggleFieldCodes(field);
					return updateResult;
				}
				finally {
					EndFieldUpdate();
				}
			}
			finally {
				result.Dispose();
			}
		}
		protected internal virtual void BeginFieldUpdate(Field field) {
			this.field = field;
			this.startResultPos = DocumentModelPosition.FromRunStart(PieceTable, field.Result.Start);
			int resultLength = field.GetResultLength(PieceTable);
			this.endResultLogPos = this.startResultPos.LogPosition + resultLength - 1;
			DocumentModel.History.BeginTransaction();
		}
		protected internal virtual void EndFieldUpdate() {
			this.field = null;
			this.startResultPos = null;
			this.endResultLogPos = new DocumentLogPosition(-1);
			DocumentModel.History.EndTransaction();
		}
		protected internal virtual UpdateFieldOperationResult ReplaceFieldResult(CalculateFieldResult result, UpdateFieldOperationType updateType) {
			CalculatedFieldValue fieldValue = result.Value;			
			DocumentModel targetModel = fieldValue.IsDocumentModelValue() ? fieldValue.DocumentModel : null;
			Debug.Assert(fieldValue.IsDocumentModelValue() || fieldValue.ConvertedToString);
			if (result.MergeFormat) {
				if (!fieldValue.IsDocumentModelValue()) {					
					targetModel = PieceTable.DocumentModel.GetFieldResultModel();					
					if(!String.IsNullOrEmpty(fieldValue.Text))
						targetModel.MainPieceTable.InsertPlainText(new DocumentLogPosition(0), fieldValue.RawValue.ToString());										
				}				
				CopyCharacterFormatToDocumentModel(targetModel);
				CopyParagraphFormatToDocumentModel(targetModel);
			}
			else if (result.ApplyFieldCodeFormatting) {
				if(targetModel != null)
					CopyFormattingFromCodeFirstChar(new DocumentModelPosition(targetModel.MainPieceTable));			   
			}
			bool shouldDeleteFieldCode = MailMergeDataMode == MailMergeDataMode.FinalMerging && result.MailMergeField;
			Size pictureFieldSize = GetInlinePictureFieldSize();
			DocumentModelPosition oldResultStart = GetDeleteStart(shouldDeleteFieldCode);
			DocumentModelPosition oldResultEnd = GetDeleteEnd(shouldDeleteFieldCode);
			if (shouldDeleteFieldCode) {
				bool emptyParagraphInserted = false;
				if (targetModel == null) {
					DocumentLogPosition endPositionBeforeInsert = PieceTable.DocumentEndLogPosition;
					PieceTable.DocumentModel.ResetMerging();
					if (!string.IsNullOrEmpty(fieldValue.Text)) {
						PieceTable.InsertPlainText(oldResultEnd.LogPosition + 1, fieldValue.Text);
						DocumentLogPosition endPositionAfterInsert = PieceTable.DocumentEndLogPosition;
						if (!result.MergeFormat && result.ApplyFieldCodeFormatting) {
							TextRunBase sourceRun = GetSourceRun();
							int length = endPositionAfterInsert - endPositionBeforeInsert;
							CopyFormattingFromCodeFirstChar(PieceTable, sourceRun, oldResultEnd.LogPosition + 1, length);
						}
					}
				}
				else {
					emptyParagraphInserted = targetModel.MainPieceTable.Paragraphs.First.IsInCell() && !(PieceTable.Runs[oldResultEnd.RunIndex] is ParagraphRun); 
					DocumentLogPosition bookmarksEndPosition = oldResultEnd.LogPosition + 1;
					List<Bookmark> bookmarks = GetBookmarksByEndPosition(PieceTable, bookmarksEndPosition);
					DocumentLogPosition oldDocumentEnd = PieceTable.DocumentEndLogPosition;
					PieceTable.InsertDocumentModelContent(targetModel, oldResultEnd.LogPosition + 1, true, false, true);
					DocumentLogPosition newDocumentEnd = PieceTable.DocumentEndLogPosition;
					int delta = newDocumentEnd - oldDocumentEnd;
					ModifyBookmarks(PieceTable, bookmarks, bookmarksEndPosition + delta);
				}
				DeleteOldFieldResult(oldResultStart, oldResultEnd, result.SuppressMergeUseFirstParagraphStyle);
				if (emptyParagraphInserted)
					DeleteOldFieldResult(oldResultStart, oldResultStart, result.SuppressMergeUseFirstParagraphStyle);
			}
			else {
				TextRunBase sourceRun = GetSourceRun();
				bool isFieldSelectedBefore = IsFieldSelected(oldResultStart.LogPosition, oldResultEnd.LogPosition);
				DeleteOldFieldResult(oldResultStart, oldResultEnd, result.SuppressMergeUseFirstParagraphStyle);
				if (targetModel == null) {
					DocumentLogPosition endPositionBeforeInsert = PieceTable.DocumentEndLogPosition;
					PieceTable.DocumentModel.ResetMerging();
					if (!string.IsNullOrEmpty(fieldValue.Text)) {
						PieceTable.InsertPlainText(oldResultStart.LogPosition, fieldValue.Text);
						DocumentLogPosition endPositionAfterInsert = PieceTable.DocumentEndLogPosition;
						int length = endPositionAfterInsert - endPositionBeforeInsert;
						if(!result.MergeFormat && result.ApplyFieldCodeFormatting) {
							CopyFormattingFromCodeFirstChar(PieceTable, sourceRun, oldResultStart.LogPosition, length);
						}
						DocumentLogPosition newResultEnd = new DocumentLogPosition(((IConvertToInt<DocumentLogPosition>)oldResultStart.LogPosition).ToInt() + length);
						bool isFieldSelectedAfter = IsFieldSelected(oldResultStart.LogPosition, newResultEnd);
						if(isFieldSelectedBefore && !isFieldSelectedAfter) {
							PieceTable.DocumentModel.Selection.Start = MinLogPosition(PieceTable.DocumentModel.Selection.Start, oldResultStart.LogPosition);
							PieceTable.DocumentModel.Selection.End = MaxLogPosition(PieceTable.DocumentModel.Selection.End, newResultEnd);
						}
					}
				}
				else
					PieceTable.InsertDocumentModelContent(targetModel, oldResultStart.LogPosition, true, true, true);
			}
			if (updateType == UpdateFieldOperationType.Copy || updateType == UpdateFieldOperationType.Load || updateType == UpdateFieldOperationType.CreateModelForExport)
				SetInlinePictureFieldSize(pictureFieldSize);
			if (result.MergeFormat)
				FixLastParagraphFormatting(targetModel, oldResultStart);
			return new UpdateFieldOperationResult(shouldDeleteFieldCode ? UpdateFieldResult.FieldUpdatedAndCodeDeleted : UpdateFieldResult.FieldUpdated, (result.Options & FieldResultOptions.SuppressUpdateInnerCodeFields) != 0); ;
		}
		List<Bookmark> GetBookmarksByEndPosition(PieceTable pieceTable, DocumentLogPosition position) {
			List<Bookmark> result = new List<Bookmark>();
			BookmarkCollection source = pieceTable.Bookmarks;
			int count = source.Count;
			for (int i = 0; i < count; i++)
				if (source[i].End == position)
					result.Add(source[i]);
			return result;
		}
		void ModifyBookmarks(PieceTable pieceTable, List<Bookmark> bookmarks, DocumentLogPosition newEndLogPosition) {
			int count = bookmarks.Count;
			DocumentModelPosition newEndPosition = PositionConverter.ToDocumentModelPosition(pieceTable, newEndLogPosition);
			for (int i = 0; i < count; i++) {
				if(bookmarks[i].End == bookmarks[i].Start)
					bookmarks[i].SetStartCore(newEndPosition);
				bookmarks[i].SetEndCore(newEndPosition);
			}
		}
		bool IsFieldSelected(DocumentLogPosition startPosition, DocumentLogPosition endPosition) {
			return PieceTable.DocumentModel.Selection.Start <= startPosition
					&& endPosition <= PieceTable.DocumentModel.Selection.End
					&& PieceTable.DocumentModel.Selection.Start != PieceTable.DocumentModel.Selection.End;
		}
		DocumentLogPosition MinLogPosition(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			if(pos1 < pos2)
				return pos1;
			return pos2;
		}
		DocumentLogPosition MaxLogPosition(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			if(pos1 > pos2)
				return pos1;
			return pos2;
		}
		protected internal void SetInlinePictureFieldSize(Size size) {
			if (size == new Size()) 
				return;
			InlinePictureRun pictureRun = PieceTable.Runs[field.Result.Start] as InlinePictureRun;
			if ((field.Result.End - field.Result.Start) == 1 && pictureRun != null)
#if !SL
				pictureRun.ActualSize = size;
#else
				pictureRun.ActualSize = new System.Drawing.Size((int)size.Width, (int)size.Height);
#endif
		}
		protected internal virtual Size GetInlinePictureFieldSize() {
			InlinePictureRun pictureRun = PieceTable.Runs[field.Result.Start] as InlinePictureRun;
			if ((field.Result.End - field.Result.Start) == 1 && pictureRun != null)
#if !SL
				return pictureRun.ActualSize;
#else
				return new Size(pictureRun.ActualSize.Width, pictureRun.ActualSize.Height);
#endif
			return new Size();
		}
		protected internal virtual void FixLastParagraphFormatting(DocumentModel targetModel, DocumentModelPosition resultStart) {
			ParagraphCollection paragraphs = targetModel.MainPieceTable.Paragraphs;
			if (paragraphs.Count == 1)
				return;
			Paragraph lastParagraph = paragraphs.Last;
			ParagraphIndex lastInsertedParIndex = resultStart.ParagraphIndex + paragraphs.Count - 1;
			CopyParagraphFormatCore(lastParagraph, PieceTable.Paragraphs[lastInsertedParIndex]);
		}
		protected internal virtual DocumentModelPosition DeleteOldFieldResult(DocumentModelPosition startPosition, DocumentModelPosition endPosition, bool suppressMergeUseFirstParagraphStyle) {
			Debug.Assert(startPosition.RunStartLogPosition == startPosition.LogPosition);			
			int length = endPosition.LogPosition - startPosition.LogPosition + 1;
			DocumentModel.BeginUpdate();
			bool oldValue = DocumentModel.EditingOptions.MergeUseFirstParagraphStyle;
			if(suppressMergeUseFirstParagraphStyle)
				DocumentModel.EditingOptions.MergeUseFirstParagraphStyle = false;
			try {
				IList<Field> includedFields = PieceTable.GetEntireFieldsFromInterval(startPosition.RunIndex, endPosition.RunIndex);
				foreach (Field field in includedFields)
					PieceTable.RemoveField(field);
				DeleteContentOperation operation = PieceTable.CreateDeleteContentOperation();
				operation.SuppressFieldDelete = true;
				operation.Execute(startPosition.LogPosition, length, false);
				PieceTable.ApplyChanges(DocumentModelChangeType.DeleteContent, startPosition.RunIndex, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EditingOptions.MergeUseFirstParagraphStyle = oldValue;
				DocumentModel.EndUpdate();
			}
			return startPosition;
		}
		protected virtual void RemoveField() {
		}
		protected virtual DocumentModelPosition GetDeleteStart(bool deleteFieldCode) {
			if (!deleteFieldCode)
				return this.startResultPos;
			return DocumentModelPosition.FromRunStart(PieceTable, this.field.FirstRunIndex);
		}
		protected virtual DocumentModelPosition GetDeleteEnd(bool deleteFieldCode) {
			if (!deleteFieldCode)
				return PositionConverter.ToDocumentModelPosition(PieceTable, this.endResultLogPos);
			return DocumentModelPosition.FromRunStart(PieceTable, this.field.LastRunIndex);
		}
		protected internal virtual void CopyCharacterFormatToDocumentModel(DocumentModel targetModel) {
			DocumentModelPosition sourcePos = this.startResultPos.Clone();
			DocumentModelPosition targetPos = new DocumentModelPosition(targetModel.MainPieceTable);
			DocumentLogPosition targetEndLogPos = targetModel.MainPieceTable.DocumentEndLogPosition;
			while (sourcePos.LogPosition <= this.endResultLogPos && targetPos.LogPosition <= targetEndLogPos) {
				CopyCharacterFormatToRuns(GetTextRun(sourcePos), targetPos);
				targetPos = GetNextCharacterGroupPosition(targetPos);
				sourcePos = GetNextCharacterGroupPosition(sourcePos);
			}
			if (targetPos.LogPosition < targetEndLogPos)
				CopyFormattingFromCodeFirstChar(targetPos);
		}
		protected internal virtual void CopyFormattingFromCodeFirstChar(DocumentModelPosition targetPos) {
			TextRunBase sourceRun = GetSourceRun();
			if (sourceRun == null)
				return;
			TextRunCollection runs = targetPos.PieceTable.Runs;
			RunIndex endRunIndex = new RunIndex(runs.Count - 1);
			for (RunIndex i = targetPos.RunIndex; i < endRunIndex; i++) {
				CopyCharacterFormatCore(sourceRun, runs[i]);
			}
		}
		protected internal virtual void CopyFormattingFromCodeFirstChar(PieceTable pieceTable, TextRunBase sourceRun, DocumentLogPosition targetPos, int length) {
			if (sourceRun == null || length == 0)
				return;
			TextRunCollection runs = pieceTable.Runs;
			RunIndex startIndex = PositionConverter.ToDocumentModelPosition(pieceTable, targetPos).RunIndex;
			RunIndex endIndex = PositionConverter.ToDocumentModelPosition(pieceTable, targetPos + length).RunIndex;
			for (RunIndex i = startIndex; i < endIndex; i++) {
				CopyCharacterFormatCore(sourceRun, runs[i]);
			}
		}
		protected internal virtual TextRunBase GetSourceRun() {
			RunIndex endRunIndex = field.Code.End;
			DocumentModelPosition pos = DocumentModelPosition.FromRunEnd(PieceTable, field.Code.Start);
			while (Char.IsWhiteSpace(GetCharacter(pos)) && pos.RunIndex < endRunIndex) {
				DocumentModelPosition.MoveForwardCore(pos);
			}
			return pos.RunIndex != endRunIndex ? PieceTable.Runs[pos.RunIndex] : null;
		}
		char GetCharacter(DocumentModelPosition pos) {
			string runText = PieceTable.Runs[pos.RunIndex].GetNonEmptyText(PieceTable.TextBuffer);
			return runText[pos.RunOffset];
		}
		protected internal virtual void CopyParagraphFormatToDocumentModel(DocumentModel targetModel) {
			DocumentModelPosition sourcePos = this.startResultPos.Clone();
			DocumentModelPosition targetPos = new DocumentModelPosition(targetModel.MainPieceTable);
			DocumentLogPosition targetEndLogPos = targetModel.MainPieceTable.DocumentEndLogPosition;
			Paragraph sourceParagraph = null;
			while (sourcePos.LogPosition <= this.endResultLogPos && targetPos.LogPosition <= targetEndLogPos) {
				sourceParagraph = GetParagraph(sourcePos);
				CopyParagraphFormatCore(sourceParagraph, GetParagraph(targetPos));
				targetPos = GetNextParagraphPosition(targetPos);
				sourcePos = GetNextParagraphPosition(sourcePos);
			}
			if (targetPos.LogPosition < targetEndLogPos && sourceParagraph != null)
				CopyParagraphFormatCore(sourceParagraph, targetModel.MainPieceTable.Paragraphs.Last);
		}
		protected internal virtual void TrySplitRun(DocumentModelPosition to) {
			if (to.RunOffset == 0)
				return;
			DocumentModel documentModel = to.PieceTable.DocumentModel;
			documentModel.BeginUpdate();
			try {
				to.PieceTable.SplitTextRun(to.ParagraphIndex, to.RunIndex, to.RunOffset);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		void CopyCharacterFormatToRuns(TextRunBase sourceRun, DocumentModelPosition targetPos) {
			DocumentModelPosition targetEnd = GetNextCharacterGroupPosition(targetPos);
			TrySplitRun(targetEnd);
			TextRunCollection runs = targetPos.PieceTable.Runs;
			for (RunIndex i = targetPos.RunIndex; i <= targetEnd.RunIndex; i++) {
				CopyCharacterFormatCore(sourceRun, runs[i]);
			}
		}
		void CopyCharacterFormatCore(TextRunBase sourceRun, TextRunBase targetRun) {
			targetRun.CharacterProperties.CopyFrom(sourceRun.CharacterProperties.Info);
			targetRun.CharacterStyleIndex = sourceRun.CharacterStyle.Copy(targetRun.Paragraph.DocumentModel);
		}
		protected void CopyParagraphFormatCore(Paragraph sourceParagraph, Paragraph targetParagraph) {
			targetParagraph.ParagraphProperties.CopyFrom(sourceParagraph.ParagraphProperties.Info);
			targetParagraph.ParagraphStyleIndex = sourceParagraph.ParagraphStyle.Copy(targetParagraph.DocumentModel);
		}
		protected TextRunBase GetTextRun(DocumentModelPosition pos) {
			return pos.PieceTable.Runs[pos.RunIndex];
		}
		protected Paragraph GetParagraph(DocumentModelPosition pos) {
			return pos.PieceTable.Paragraphs[pos.ParagraphIndex];
		}
		protected DocumentModelPosition GetNextCharacterGroupPosition(DocumentModelPosition pos) {
			CharactersGroupIterator iterator = new CharactersGroupIterator(pos.PieceTable);
			return iterator.MoveNext(pos);
		}
		protected DocumentModelPosition GetNextParagraphPosition(DocumentModelPosition pos) {
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(pos.PieceTable);
			return iterator.MoveForward(pos);
		}
	}
	#endregion
}
