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

using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.History;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Operations;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native.Templates;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native {
	public class SnapPieceTable : PieceTable {
		readonly SnapBookmarkCollection snapBookmarks;
		readonly DocumentModelPositionManager documentPositionManager;
		public SnapPieceTable(SnapDocumentModel documentModel, ContentTypeBase contentType) : base(documentModel, contentType) {
			this.snapBookmarks = new SnapBookmarkCollection(this);
			this.documentPositionManager = new DocumentModelPositionManager(this);
		}
		protected internal override bool SupportFieldCommonStringFormat { get { return true;}
		}
		protected override FieldUpdater CreateFieldUpdater() {
			return new SnapFieldUpdater(this);
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public DocumentModelPositionManager DocumentPositionManager { get { return documentPositionManager; } }
		internal override bool IsTableCellStyleAvailable { get { return true; } }
		#region Bookmarks
		public SnapBookmarkCollection SnapBookmarks {
			[System.Diagnostics.DebuggerStepThrough]
			get { return snapBookmarks; }
		}
		#endregion
		public LastInsertedChartRunInfo LastInsertedChartRunInfo {
			get { return DocumentModel.GetLastInsertedChartRunInfo(this); }
		}
		public override void Clear() {
			base.Clear();
			if (snapBookmarks != null)
				snapBookmarks.Clear();
			if (documentPositionManager != null)
				documentPositionManager.Clear();
		}
		protected override void BookmarksClear() {
			SnapBookmarks.Clear();
		}
		protected internal override void OnEndSetContent() {
			base.OnEndSetContent();
			SnapBookmarks.UpdateIntervals();
		}
		public Field FindFieldNearestToSelection(string name, bool allowEmptyResult) { 
			return FindFieldNearestToLogPosition(name, allowEmptyResult, DocumentModel.Selection.Start); 
		}
		public Field FindFieldNearestToLogPosition(string name, bool allowEmptyResult, DocumentLogPosition position) {
			Field fieldWithResult = null;
			DocumentModelPosition selectionStartlPosition = PositionConverter.ToDocumentModelPosition(this, position);
			int delta = int.MaxValue;
			Fields.ForEach(f => {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField list = calculator.ParseField(this, f) as SNListField;
				bool empty = string.IsNullOrEmpty(name);
				if(list != null && (empty ? string.IsNullOrEmpty(list.Name) : (string.Compare(list.Name, name) == 0)) && (allowEmptyResult || f.Result.Start != f.Result.End) && delta > Math.Abs(f.Result.Start - selectionStartlPosition.RunIndex)) {
					fieldWithResult = f;
					if(f.Result.Start != f.Result.End)
						delta = Math.Abs(f.Result.Start - selectionStartlPosition.RunIndex);
				}
			});
			return fieldWithResult;
		}
		#region SnapBookmarks
		protected internal SnapBookmark CreateSnapBookmarkCore(DocumentLogPosition position, int length, IFieldContext dataContext, DocumentLogInterval templateInterval, SnapPieceTable templatePieceTable, SnapTemplateInfo templateInfo) {
			InsertSnapBookmarkHistoryItem item = CreateInsertSnapBookmarkHistoryItem(position, length, templateInfo);
			item.DataContext = dataContext;
			item.TemplateLogInterval = templateInterval;
			item.TemplatePieceTable = templatePieceTable;
			DocumentModel.History.Add(item);
			item.Execute();
			return SnapBookmarks[item.IndexToInsert];
		}
		protected virtual InsertSnapBookmarkHistoryItem CreateInsertSnapBookmarkHistoryItem(DocumentLogPosition position, int length, SnapTemplateInfo templateInfo) {
			bool startBeforeContent = position < DocumentLogPosition.Zero;
			bool endAfterContent = position + length > DocumentEndLogPosition;
			if (startBeforeContent || endAfterContent) {
				Exceptions.ThrowInternalException();
				return null;
			}
			else {
				InsertSnapBookmarkHistoryItem result = new InsertSnapBookmarkHistoryItem(this);
				result.Position = position;
				result.Length = length;
				result.TemplateInfo = templateInfo;
				return result;
			}
		}
		public SnapBookmark CreateSnapBookmark(DocumentLogPosition position, int length, IFieldContext dataContext, DocumentLogInterval templateInterval, SnapPieceTable templatePieceTable, SnapTemplateInfo templateInfo) {
			DocumentModel.BeginUpdate();
			try {
				SnapBookmark bookmark = CreateSnapBookmarkCore(position, length, dataContext, templateInterval, templatePieceTable, templateInfo);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout;
				RunInfo runInfo = bookmark.Interval;
				ApplyChangesCore(changeActions, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
				return bookmark;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteSnapBookmark(int bookmarkIndex) {
			DocumentModel.BeginUpdate();
			try {
				SnapBookmark bookmark = SnapBookmarks[bookmarkIndex];
				DeleteSnapBookmarkCore(bookmarkIndex);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout;
				RunInfo runInfo = bookmark.Interval;
				ApplyChangesCore(changeActions, runInfo.NormalizedStart.RunIndex, runInfo.NormalizedEnd.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void DeleteSnapBookmarkCore(int bookmarkIndex) {
			SnapBookmarkDeletedHistoryItem item = new SnapBookmarkDeletedHistoryItem(this);
			item.DeletedBookmarkIndex = bookmarkIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal List<SnapBookmark> GetEntireSnapBookmarks(DocumentLogPosition start, int length) {
			return GetEntireBookmarksCore(SnapBookmarks, start, length);
		}
		#endregion
		protected override PieceTableDeleteTextCommand CreatePieceTableDeleteTextCommand(DocumentLogPosition logPosition, int length) {
			return new SnapPieceTableDeleteTextCommand(this, logPosition, length);
		}
		#region IDocumentModelStructureChangedListener overrides
		protected override void OnParagraphInsertedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			base.OnParagraphInsertedCore(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(SnapBookmarks, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(DocumentPositionManager, this, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		protected override void OnParagraphRemovedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			base.OnParagraphRemovedCore(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(SnapBookmarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(DocumentPositionManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected override void OnParagraphMergedCore(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			base.OnParagraphMergedCore(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(SnapBookmarks, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(DocumentPositionManager, this, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		protected override void OnRunInsertedCore(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			base.OnRunInsertedCore(paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(SnapBookmarks, this, paragraphIndex, newRunIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunInserted(DocumentPositionManager, this, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected override void OnRunRemovedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			base.OnRunRemovedCore(paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(SnapBookmarks, this, paragraphIndex, runIndex, length, historyNotificationId);
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(DocumentPositionManager, this, paragraphIndex, runIndex, length, historyNotificationId);
		}
		protected override void OnRunMergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			base.OnRunMergedCore(paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(SnapBookmarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunMerged(DocumentPositionManager, this, paragraphIndex, runIndex, deltaRunLength);
		}
		protected override void OnRunUnmergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			base.OnRunUnmergedCore(paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(SnapBookmarks, this, paragraphIndex, runIndex, deltaRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(DocumentPositionManager, this, paragraphIndex, runIndex, deltaRunLength);
		}
		protected override void OnRunSplitCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			base.OnRunSplitCore(paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(SnapBookmarks, this, paragraphIndex, runIndex, splitOffset);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(DocumentPositionManager, this, paragraphIndex, runIndex, splitOffset);
		}
		protected override void OnRunJoinedCore(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			base.OnRunJoinedCore(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(SnapBookmarks, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(DocumentPositionManager, this, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		#endregion
		public Field FindNonTemplateFieldByRunIndex(RunIndex runIndex) {
			int index = FindFieldIndexByRunIndex(runIndex);
			if (index >= 0) {
				Field result = Fields[index];
				if (result.DisableUpdate)
					result = result.Parent;
				return result;
			}
			return null;
		}
		public void UpdateTemplate(bool clearUnusedSwitches) {
			UpdateTemplateCore(DocumentModel.Selection.NormalizedStart, clearUnusedSwitches);
		}
		public void UpdateTemplateByField(Field field, bool clearUnusedSwitches) {
			DocumentLogPosition logPosition = GetRunLogPosition(field.Code.Start);
			UpdateTemplateCore(logPosition, clearUnusedSwitches);
		}
		public void UpdateTemplateByTableRow(TableRow row, bool clearUnusedSwitches) {
			DocumentLogPosition start = GetRunInfoByTableCell(row.FirstCell).NormalizedStart.LogPosition;
			SnapBookmark bookmark = FindBookmarkByPosition(start);
			UpdateTemplateCore(start, clearUnusedSwitches);
			Field field = null;
			while (bookmark != null) {
				field = FindFieldByBookmarkTemplateInterval(bookmark);
				bookmark = FindBookmarkByPosition(bookmark.TemplateInterval.Start);
			}
			if (field != null)
				FieldUpdater.UpdateFieldAndNestedFields(field);
		}
		protected internal virtual void UpdateTemplateCore(DocumentLogPosition start, bool clearUnusedSwitches) {
			SnapBookmark bookmark = FindBookmarkByPosition(start);
			if (bookmark == null)
				return;
			UpdateTemplates(bookmark, clearUnusedSwitches);
			UpdateTemplateCore(bookmark.TemplateInterval.Start, clearUnusedSwitches);
		}
		protected virtual void UpdateTemplates(SnapBookmark changedBookmark, bool clearUnusedSwitches) {
			Field field = FindFieldByBookmarkTemplateInterval(changedBookmark);
			SnapFieldCalculatorService parser = new SnapFieldCalculatorService();
			SnapPieceTable pieceTable = (SnapPieceTable)changedBookmark.PieceTable;
			CalculatedFieldBase parsedField = parser.ParseField(pieceTable, field);
			TemplatedFieldBase templatedField = parsedField as TemplatedFieldBase;
			if (templatedField == null) {
				field = field.Parent;
				parsedField = parser.ParseField(pieceTable, field);
				templatedField = parsedField as TemplatedFieldBase;
				if (templatedField == null)
					return;
			}
			List<TemplateFieldInterval> templateIntervals = templatedField.GetTemplateIntervals(this);
			int count = templateIntervals.Count;
			List<SnapBookmark> bookmarks = new List<SnapBookmark>(count);
			List<TemplateFieldInterval> removedTemplateInterval = new List<TemplateFieldInterval>();
			SnapBookmarkController controller = new SnapBookmarkController(this);
			DocumentLogInterval changedBookmarkTemplateInterval = new DocumentLogInterval(changedBookmark.TemplateInterval.NormalizedStart, changedBookmark.TemplateInterval.Length);
			for (int i = 0; i < count; i++) {
				DocumentLogInterval templateInterval = templateIntervals[i].Interval;
				SnapBookmark bookmark = (changedBookmarkTemplateInterval.Start == templateInterval.Start && changedBookmarkTemplateInterval.Length == templateInterval.Length) ? changedBookmark : controller.FindBookmarkByTemplateInterval(pieceTable.SnapBookmarks, templateInterval);
				if (bookmark != null)
					bookmarks.Add(bookmark);
				else {
					if(ShouldRemoveTemplate(templateIntervals[i]))
						removedTemplateInterval.Add(templateIntervals[i]);
				}
			}
			count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				UpdateContentTemplate(bookmarks[i]);
			}
			count = removedTemplateInterval.Count;
			if (!clearUnusedSwitches || count == 0)
				return;
			templatedField = parser.ParseField(pieceTable, field) as TemplatedFieldBase;
			if (templatedField == null)
				return;
			using (InstructionController instructionController = new InstructionController(this, templatedField, field)) {
				instructionController.SuppressFieldsUpdateAfterUpdateInstruction = true;
				List<string> removedSwitches = new List<string>();
				for (int i = 0; i < count; i++) {
					string templateSwitch = removedTemplateInterval[i].TemplateSwitch;
					instructionController.RemoveSwitch(templateSwitch);
					removedSwitches.Add(templateSwitch);
				}
				templatedField.OnRemoveTemplateSwitches(this, instructionController, removedSwitches);
			}
		}
		bool ShouldRemoveTemplate(TemplateFieldInterval templateFieldInterval) {
			TemplateController templateController = new TemplateController(this);
			DocumentLogInterval interval = templateController.GetActualTemplateInterval(templateFieldInterval.Interval);
			SeparatorTemplateComparer comparer = new SeparatorTemplateComparer(this);
			return !comparer.IsPageBreak(interval);
		}
		public SnapBookmark FindBookmarkByPosition(DocumentLogPosition position) {
			SnapBookmarkController controller = new SnapBookmarkController(this);
			return controller.FindInnermostTemplateBookmarkByPosition(position);
		}
		protected internal Field FindFieldByBookmarkTemplateInterval(SnapBookmark bookmark) {
			SnapPieceTable pieceTable = (SnapPieceTable)bookmark.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.TemplateInterval.Start - 1);
			return bookmark.PieceTable.FindNonTemplateFieldByRunIndex(start.RunIndex);
		}
		protected virtual void UpdateContentTemplate(SnapBookmark bookmark) {
			DocumentLogInterval sourceInterval = new DocumentLogInterval(bookmark.Start, bookmark.Length);
			ReplaceContent(sourceInterval, bookmark.TemplateInterval);
		}
		protected virtual void ReplaceContent(DocumentLogInterval sourceInterval, SnapTemplateInterval targetInterval) {
			DocumentModel intermediateModel = GetNewTemplate(sourceInterval);
			DocumentLogPosition targetIntervalStart = targetInterval.Start;
			int intervalLength = targetInterval.Length;
			InsertParagraph(targetIntervalStart + intervalLength);
			DeleteContent(targetIntervalStart, intervalLength, false, true, true);
			DocumentModelPosition startPos = PositionConverter.ToDocumentModelPosition(this, targetInterval.Start - 1);
			TextRunBase prevPositionRun = Runs[startPos.RunIndex];
			bool shouldInsertField = true;
			FieldCodeStartRun fieldRun = prevPositionRun as FieldCodeStartRun;
			if (fieldRun != null) {
				int fieldIndex = FindFieldIndexByRunIndex(startPos.RunIndex);
				if (fieldIndex >= 0 && Fields[fieldIndex].DisableUpdate)
					shouldInsertField = false;
			}
			if (shouldInsertField) {
				Field field = CreateField(targetIntervalStart, 1);
				field.DisableUpdate = true;
				targetIntervalStart++;
			}
			if (intermediateModel != null)
				InsertToPosition(targetIntervalStart + 1, intermediateModel);
		}
		protected virtual void InsertToPosition(DocumentLogPosition position, DocumentModel sourceModel) {
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(sourceModel.MainPieceTable, this, ParagraphNumerationCopyOptions.CopyAlways);
			DocumentModelPosition targetPosition = PositionConverter.ToDocumentModelPosition(this, position);
			copyManager.TargetPosition.CopyFrom(targetPosition);
			CopySectionOperation operation = this.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.FixLastParagraph = false;
			operation.SuppressFieldsUpdate = true;
			operation.Execute(DocumentLogPosition.Zero, sourceModel.MainPieceTable.DocumentEndLogPosition - DocumentLogPosition.Zero, false);
		}
		protected virtual DocumentModel GetNewTemplate(DocumentLogInterval sourceInterval) {
			if (sourceInterval == null)
				return null;
			DocumentModel result = DocumentModel.CreateNew();			
			result.IntermediateModel = true;
			result.BeginUpdate();
			try {
				SnapDocumentModelCopyOptions options = new SnapDocumentModelCopyOptions(sourceInterval.Start, sourceInterval.Length);
				options.CopyDocumentVariables = false;
				options.CopySnapBookmark = false;
				DocumentModelCopyCommand copyCommand = new DocumentModelCopyCommand(this, result, options) { 
					AllowCopyWholeFieldResult = true,
					SuppressFieldsUpdate = true,	
					RemoveLeadingPageBreak = true
				};
				copyCommand.Execute();
				SnapPieceTable templatePieceTable = ((SnapPieceTable)result.MainPieceTable);
				templatePieceTable.RemoveAllSeparators();
				templatePieceTable.RemoveFieldResults();
				return result;
			}
			finally {
				result.EndUpdate();
			}
		}
		protected virtual void RemoveFieldResults() {
			int index = 0;
			int count = Fields.Count;
			while (index < count) {
				Field field = Fields[index];
				if (field.Result.End - field.Result.Start == 0) {
					index++;
					continue;
				}
				SetSizeInfo(field);
				DocumentLogPosition resultStart = GetRunLogPosition(field.Result.Start);
				DocumentLogPosition resultEnd = GetRunLogPosition(field.Result.End);
				DeleteContent(resultStart, resultEnd - resultStart, false, true, true, true);
				index = 0;
				count = Fields.Count;
			}
		}
		internal void SetSizeInfo(Field field) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			CalculatedFieldBase parsedInfo = calculator.ParseField(this, field);
			if (parsedInfo == null)
				return;
			SizeAndScaleFieldController controller = SizeAndScaleFieldController.Create(parsedInfo, new InstructionController(this, parsedInfo, field));
			if (controller != null && controller.IsReady)
				controller.SetImageSizeInfo();
		}
		public void RemoveAllSeparators() {
			DocumentModel.BeginUpdate();
			try {
				DocumentLogPosition position = DocumentLogPosition.Zero;
				RunIndex runIndex = RunIndex.Zero;
				while (runIndex < new RunIndex(Runs.Count)) {
					TextRunBase run = Runs[runIndex];
					SeparatorTextRun separatorRun = run as SeparatorTextRun;
					if (separatorRun != null)
						DeleteContent(position, run.Length, false, true, false);
					else {
						position += run.Length;
						runIndex++;
					}
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override DeleteContentOperation CreateDeleteContentOperation() {
			return new SnapDeleteContentOperation(this);
		}
		#region SnapBookmarks
		internal int[] GetChildSnapBookmarkIndexes(SnapBookmark parent) {
			Guard.ArgumentNotNull(parent, "parent");
			List<int> result = new List<int>();
			int count = SnapBookmarks.Count;
			SnapBookmarkController controller = new SnapBookmarkController(this);
			int index = controller.FindInnermostTemplateBookmarkIndexByStartPosition(parent.NormalizedStart);
			if (index < 0)
				index = ~index;
			for (; index < count; index++) {
				SnapBookmark bookmark = SnapBookmarks[index];
				if (bookmark.NormalizedStart > parent.NormalizedEnd)
					break;
				if (bookmark.NormalizedStart == parent.NormalizedEnd && bookmark.NormalizedEnd > parent.NormalizedEnd)
					break;
				Debug.Assert(bookmark.NormalizedEnd <= parent.NormalizedEnd);
				SnapBookmark currentBookmarkParent = bookmark.Parent;
				if (currentBookmarkParent != null && currentBookmarkParent.NormalizedStart >= parent.NormalizedStart && currentBookmarkParent.NormalizedEnd <= parent.NormalizedEnd)
					continue;
				result.Add(index);
			}
			return result.ToArray();
		}
		#endregion
		public int GetIndexRelativeToParent(SnapBookmark bookmark) {
			int count = SnapBookmarks.Count;
			int result = 0;
			SnapBookmark parent = bookmark.Parent;
			for (int i = 0; i < count; i++) {
				SnapBookmark currentBookmark = SnapBookmarks[i];
				if (currentBookmark == bookmark)
					return result;
				if (currentBookmark.Parent == parent)
					result++;
			}
			return -1;
		}
		public SnapBookmark GetBookmarkByRelativeIndex(SnapBookmark parent, int relativeIndex) {
			int count = SnapBookmarks.Count;
			int result = -1;
			for (int i = 0; i < count; i++) {
				SnapBookmark currentBookmark = SnapBookmarks[i];
				if (currentBookmark.Parent == parent)
					result++;
				if (result == relativeIndex)
					return currentBookmark;
			}
			return null;
		}
		internal IFieldContext GetRootDataContext() {
			return DocumentModel.GetRootDataContext();
		}
		protected internal override void PreprocessContentBeforeExport(DocumentFormat format) {
			if (format != SnapDocumentFormat.Snap) {
				ProcessFieldsRecursive(null, IsSupportedField);
				PreprocessRunsBeforeExport();
				DocumentModel.BeginUpdate();
				try {
					ConvertTableCellStyle();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		protected internal virtual void ConvertTableCellStyle() {
			Paragraphs.ForEach(MergeProperties);
		}
		protected internal virtual void MergeProperties(Paragraph paragraph) {
			TableCell cell = paragraph.GetCell();
			if (cell != null && cell.StyleIndex > 0) {
				paragraph.ParagraphProperties.Merge(cell.TableCellStyle.ParagraphProperties);
				cell.Properties.Merge(cell.TableCellStyle.TableCellProperties);
				RunIndex firstRunIndex = paragraph.FirstRunIndex;
				RunIndex lastRunIndex = paragraph.LastRunIndex;
				for (RunIndex runIndex = firstRunIndex; runIndex <= lastRunIndex; runIndex++) {
					TextRunBase run = Runs[runIndex];
					run.CharacterProperties.Merge(cell.TableCellStyle.CharacterProperties);
				}
			}
		}
		protected internal virtual void PreprocessRunsBeforeExport() {
			RunIndex lastIndex = new RunIndex(Runs.Count - 1);
			for (RunIndex i = lastIndex; i >= new RunIndex(0); i--) {
				if (Runs[i] is CustomRun)
					ReplaceCustomRunWithInlinePicture(i);
			}
		}
		void ReplaceCustomRunWithInlinePicture(RunIndex i) {
			DocumentModel.BeginUpdate();
			DocumentLogPosition startPositon = GetRunLogPosition(i);
			CustomRun customRun = (CustomRun)Runs[i];
			DeleteContent(startPositon, customRun.Length, false);
			IRectangularObject rectangularObject = customRun.GetRectangularObject();
			if (rectangularObject != null)
				InsertInlinePicture(startPositon, customRun.CustomRunObject, rectangularObject.ActualSize);
			DocumentModel.EndUpdate();
		}
		void InsertInlinePicture(DocumentLogPosition startPositon, ICustomRunObject customRunObject, Size actualSize) {
			Rectangle imageBounds = new Rectangle(new Point(0, 0), actualSize);
			OfficeImage image = customRunObject.ExportToImage(this, imageBounds);
			InsertInlinePicture(startPositon, image);
		}
		protected internal bool IsSupportedField(Field field) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(this, field);
			FieldScanner scanner = new FieldScanner(iterator, this);
			Token firstToken = scanner.Scan();
			return scanner.IsValidFirstToken(firstToken);
		}
		protected internal override FieldCalculatorService CreateFieldCalculatorService() {
			return new SnapFieldCalculatorService();
		}
		protected internal override void UpdateTableOfContents(UpdateFieldOperationType operationType) {
			List<Field> fieldsToUpdate = GetTocFields();
			int count = fieldsToUpdate.Count;
			for (int i = 0; i < count; i++) {
				FieldUpdater.UpdateFieldAndNestedFields(fieldsToUpdate[i]);
				DocumentModel.ResetTemporaryLayout();
				FieldUpdater.UpdateFieldAndNestedFields(fieldsToUpdate[i]);
			}
		}
		[Conditional("DEBUGTEST")]
		protected internal void CheckSnapBookmarksIntegrity() {
			int count = SnapBookmarks.Count;
			for (int i = 0; i < count - 1; i++) {
				SnapBookmark bookmark = SnapBookmarks[i];
				SnapBookmark nextBookmark = SnapBookmarks[i + 1];
				Debug.Assert(nextBookmark.Start >= bookmark.Start);
			}
		}
		public RunInfo GetRunInfo(DocumentLogPosition start, DocumentLogPosition end) {
			return FindRunInfo(start, end - start + 1);
		}
		protected internal bool CanEditRange(RunIndex start, RunIndex end) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			RangePermissionCollection permissions = RangePermissions;
			int count = permissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission rangePermission = permissions[i];
				RunInfo runInfo = rangePermission.Interval;
				if ((start > runInfo.Start.RunIndex || (start == runInfo.Start.RunIndex && runInfo.Start.RunOffset == 0)) && (end < runInfo.End.RunIndex) && IsPermissionGranted(rangePermission))
					return true;
			}
			return false;
		}
		protected internal override DocumentModelCopyManager GetCopyManagerCore(PieceTable sourcePieceTable, ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions) {
			return new SnapCopyManager(sourcePieceTable, this, paragraphNumerationCopyOptions, formattingCopyOptions);
		}
		public InstructionController CreateFieldInstructionController(Field field) {
			if (field == null)
				return null;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			CalculatedFieldBase parsedInfo = calculator.ParseField(this, field);
			if (parsedInfo == null)
				return null;
			return new InstructionController(this, parsedInfo, field);
		}
		protected internal override void ToggleFieldCodesFromCommandOrApi(Field field) {
			DocumentModel.BeginUpdate();
			try {
				if (DocumentModel.Selection.PieceTable == this) {
					SetSelectionAfterField(field);
					DocumentModel.SelectionInfo.CheckCurrentSnapBookmark(false, true);
				}
				base.ToggleFieldCodesFromCommandOrApi(field);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void SetSelectionAfterTopLevelField() {
			Selection selection = DocumentModel.Selection;
			System.Diagnostics.Debug.Assert(selection.PieceTable == this);
			DocumentModelPosition startPos = PositionConverter.ToDocumentModelPosition(this, selection.NormalizedStart);
			DocumentLogPosition endLogPosition = Algorithms.Min<DocumentLogPosition>(Paragraphs[startPos.ParagraphIndex].EndLogPosition, selection.NormalizedEnd);
			DocumentModelPosition endPos = selection.Length > 0 ? PositionConverter.ToDocumentModelPosition(this, endLogPosition) : startPos;
			Field field = FindNonTemplateFieldByRunIndex(endPos.RunIndex);
			if (field == null && endPos != startPos)
				field = FindNonTemplateFieldByRunIndex(startPos.RunIndex);
			if (field == null)
				return;
			while (field.Parent != null)
				field = field.Parent;
			SetSelectionAfterField(field);
		}
		protected void SetSelectionAfterField(Field field) {
			Selection selection = DocumentModel.Selection;
			System.Diagnostics.Debug.Assert(selection.PieceTable == this);
			RunIndex targetRunIndex = field.LastRunIndex + 1;
			DocumentLogPosition targetPosition = GetRunLogPosition(targetRunIndex);
			selection.Start = targetPosition;
			selection.End = targetPosition;
			selection.SetStartCell(targetPosition);
		}
		public ChartRun InsertChart(DocumentLogPosition position, OfficeImage image) {
			const int Scale = 100;
			return InsertChart(position, image, Scale, Scale);
		}
		public ChartRun InsertChart(DocumentLogPosition position, OfficeImage image, int scaleX, int scaleY) {
			PieceTableInsertChartAtLogPositionCommand command = new PieceTableInsertChartAtLogPositionCommand(this, position, scaleX, scaleY, image);
			command.Execute();
			return (ChartRun)command.Result;
		}
		public ChartRun InsertChartCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, OfficeImage image, float scaleX, float scaleY, bool forceVisible) {
			TextBuffer.Append(Characters.ObjectMark);
			ChartInserter inserter = new ChartInserter(this, image, scaleX, scaleY);
			RunIndex newRunIndex = InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
			return (ChartRun)Runs[newRunIndex];
		}
		public void InsertChartImage(ImportInputPosition position, OfficeImage image, float scaleX, float scaleY) {
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(position.PieceTable, this));
			InsertChartCore(position.ParagraphIndex, position.LogPosition, image, scaleX, scaleY, false);
			LastInsertedChartRunInfo lastInsertedChartRunInfo = LastInsertedChartRunInfo;
			ChartRun run = lastInsertedChartRunInfo.Run;
			run.ApplyFormatting(position);
			position.LogPosition++;
		}
		public SparklineRun InsertSparkline(DocumentLogPosition position, OfficeImage image) {
			const int Scale = 100;
			return InsertSparkline(position, image, Scale, Scale);
		}
		public SparklineRun InsertSparkline(DocumentLogPosition position, OfficeImage image, int scaleX, int scaleY) {
			PieceTableInsertSparklineAtLogPositionCommand command = new PieceTableInsertSparklineAtLogPositionCommand(this, position, scaleX, scaleY, image);
			command.Execute();
			return (SparklineRun)command.Result;
		}
		public SparklineRun InsertSparklineCore(ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, OfficeImage image, float scaleX, float scaleY, bool forceVisible) {
			TextBuffer.Append(Characters.ObjectMark);
			SparklineInserter inserter = new SparklineInserter(this, image, scaleX, scaleY);
			RunIndex newRunIndex = InsertObjectCore(inserter, paragraphIndex, logPosition, forceVisible);
			return (SparklineRun)Runs[newRunIndex];
		}
		public void InsertSparklineImage(ImportInputPosition position, OfficeImage image, float scaleX, float scaleY) {
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(position.PieceTable, this));
			SparklineRun run = InsertSparklineCore(position.ParagraphIndex, position.LogPosition, image, scaleX, scaleY, false);
			run.ApplyFormatting(position);
			position.LogPosition++;
		}
		public void DeleteFieldWithResult(Field field) {
			DocumentModelPosition startPosition = DocumentModelPosition.FromRunStart(this, field.Code.Start);
			DocumentModelPosition endPosition = DocumentModelPosition.FromRunEnd(this, field.Result.End);
			this.DeleteContent(startPosition.LogPosition, endPosition.LogPosition - startPosition.LogPosition, false, true, true, false);
		}
	}
}
