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
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentModelDeferredChanges
	public class DocumentModelDeferredChanges : IDocumentModelStructureChangedListener {
		#region Fields
		bool isSetContentMode;
		readonly DocumentModel documentModel;
		DocumentModelChangeActions changeActions;
		readonly DocumentModelPosition changeStart;
		readonly DocumentModelPosition changeEnd;
		readonly DocumentModelPositionAnchor startAnchor;
		readonly DocumentModelPositionAnchor endAnchor;
		readonly Dictionary<PieceTable, SortedRunIndexCollection> runIndicesForSplit;
		bool originalSelectionUsePreviousBoxBounds;
		int originalSelectionLength;
		bool selectionChanged;
		bool suppressSyntaxHighlight;
		bool suppressClearOutdatedSelectionItems;
		List<PieceTable> additionalChangedPieceTables;
		#endregion
		public DocumentModelDeferredChanges(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.runIndicesForSplit = new Dictionary<PieceTable, SortedRunIndexCollection>();
			this.changeStart = DocumentModelPosition.FromParagraphEnd(documentModel.MainPieceTable, documentModel.MainPieceTable.Paragraphs.Last.Index);
			this.changeEnd = DocumentModelPosition.FromParagraphStart(documentModel.MainPieceTable, ParagraphIndex.Zero);
			this.startAnchor = new DocumentModelPositionAnchor(changeStart);
			this.endAnchor = new DocumentModelPositionAnchor(changeEnd);
			ResetSelectionChanged();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public DocumentModelChangeActions ChangeActions { get { return changeActions; } set { changeActions = value; } }
		public DocumentModelPosition ChangeStart { get { return changeStart; } }
		public DocumentModelPosition ChangeEnd { get { return changeEnd; } }
		protected internal DocumentModelPositionAnchor StartAnchor { get { return startAnchor; } }
		protected internal DocumentModelPositionAnchor EndAnchor { get { return endAnchor; } }
		public Dictionary<PieceTable, SortedRunIndexCollection> RunIndicesForSplit { get { return runIndicesForSplit; } }
		public bool IsSetContentMode { get { return isSetContentMode; } set { isSetContentMode = value; } }
		public List<PieceTable> AdditionalChangedPieceTables { get { return additionalChangedPieceTables; } }
		public bool SelectionChanged {
			get {
				return selectionChanged || (originalSelectionUsePreviousBoxBounds != documentModel.Selection.UsePreviousBoxBounds);
			}
		}
		public bool SelectionLengthChanged { get { return originalSelectionLength != documentModel.Selection.Length; } }
		public bool SuppressSyntaxHighlight { get { return suppressSyntaxHighlight; } set { suppressSyntaxHighlight = value; } }
		public bool SuppressClearOutdatedSelectionItems { get { return suppressClearOutdatedSelectionItems; } set { suppressClearOutdatedSelectionItems = value; } }
		public bool EnsureCaretVisible { get; set; }
		#endregion
		protected internal virtual void ResetSelectionChanged() {
			this.originalSelectionUsePreviousBoxBounds = documentModel.Selection.UsePreviousBoxBounds;
			this.originalSelectionLength = documentModel.Selection.Length;
			this.selectionChanged = false;
		}
		public virtual void RegisterSelectionChanged() {
			this.selectionChanged = true;
		}
		protected internal SortedRunIndexCollection GetRunIndicesForSplit(PieceTable pieceTable) {
			SortedRunIndexCollection result;
			if (!runIndicesForSplit.TryGetValue(pieceTable, out result)) {
				result = new SortedRunIndexCollection(pieceTable);
				runIndicesForSplit.Add(pieceTable, result);
			}
			return result;
		}
		public virtual void ApplyChanges(PieceTable pieceTable, DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (pieceTable.DocumentModel.SuppressPerformLayout > 0) {
				if ((actions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0)
					actions ^= DocumentModelChangeActions.ResetAllPrimaryLayout;
				if ((actions & DocumentModelChangeActions.ResetPrimaryLayout) != 0)
					actions ^= DocumentModelChangeActions.ResetPrimaryLayout;
				if ((actions & DocumentModelChangeActions.ResetSecondaryLayout) != 0)
					actions ^= DocumentModelChangeActions.ResetSecondaryLayout;
			}
			this.changeActions |= actions;
			if ((actions & DocumentModelChangeActions.SplitRunByCharset) != 0 && !IsSetContentMode)
				GetRunIndicesForSplit(pieceTable).Add(startRunIndex);
			if (ShouldUpdatePositions(actions)) {
				Debug.Assert(startRunIndex > RunIndex.DontCare);
				Debug.Assert(endRunIndex > RunIndex.DontCare);
				if (startRunIndex < ChangeStart.RunIndex || (startRunIndex == ChangeStart.RunIndex && ChangeStart.RunStartLogPosition != ChangeStart.LogPosition))
					DocumentModelPosition.SetRunStart(ChangeStart, startRunIndex);
				if (endRunIndex >= ChangeEnd.RunIndex) {
					endRunIndex = Algorithms.Min(endRunIndex, new RunIndex(ChangeEnd.PieceTable.Runs.Count - 1));
					DocumentModelPosition.SetRunEnd(ChangeEnd, endRunIndex);
				}
			}
			if (!pieceTable.IsMain)
				AddAdditionalChangedPieceTable(pieceTable);
		}
		protected virtual void AddAdditionalChangedPieceTable(PieceTable additionalChangedPieceTable) {
			if (additionalChangedPieceTables == null) {
				additionalChangedPieceTables = new List<PieceTable>();
				additionalChangedPieceTables.Add(additionalChangedPieceTable);
				return;
			}
			if (additionalChangedPieceTables.Contains(additionalChangedPieceTable))
				return;
			additionalChangedPieceTables.Add(additionalChangedPieceTable);
		}
		protected internal virtual bool ShouldUpdatePositions(DocumentModelChangeActions actions) {
			return ((actions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0) ||
				((actions & DocumentModelChangeActions.ResetPrimaryLayout) != 0) ||
				((actions & DocumentModelChangeActions.ResetSecondaryLayout) != 0);
		}
		#region IDocumentModelStructureChangedListener Members
		public void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(GetRunIndicesForSplit(pieceTable), pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(StartAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(EndAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			}
		}
		public void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(GetRunIndicesForSplit(pieceTable), pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(StartAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(EndAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
		}
		public void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(GetRunIndicesForSplit(pieceTable), pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(StartAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(EndAnchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
		}
		public void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyRunInserted(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunInserted(StartAnchor, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunInserted(EndAnchor, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			}
		}
		public void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(StartAnchor, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(EndAnchor, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			}
		}
		public void OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		public void OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		public void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			DocumentModelStructureChangedNotifier.NotifyRunSplit(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, runIndex, splitOffset);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunSplit(StartAnchor, pieceTable, paragraphIndex, runIndex, splitOffset);
				DocumentModelStructureChangedNotifier.NotifyRunSplit(EndAnchor, pieceTable, paragraphIndex, runIndex, splitOffset);
			}
		}
		public void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunJoined(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunJoined(StartAnchor, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunJoined(EndAnchor, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			}
		}
		public void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunMerged(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, runIndex, deltaRunLength);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunMerged(StartAnchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunMerged(EndAnchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			}
		}
		public void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(GetRunIndicesForSplit(pieceTable), pieceTable, paragraphIndex, runIndex, deltaRunLength);
			if (pieceTable.IsMain) {
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(StartAnchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(EndAnchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			}
		}
		public void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
		}
		public void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
		}
		#endregion
	}
	#endregion
	#region SortedRunIndexCollection
	public class SortedRunIndexCollection : IDocumentModelStructureChangedListener {
		readonly PieceTable pieceTable;
		readonly List<RunIndex> indices;
		public SortedRunIndexCollection(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.indices = new List<RunIndex>();
		}
		public int Count { get { return indices.Count; } }
		public RunIndex this[int index] { get { return indices[index]; } }
		public void Add(RunIndex runIndex) {
			int index = indices.BinarySearch(runIndex);
			if (index >= 0)
				return;
			index = ~index;
			indices.Insert(index, runIndex);
		}
		#region IDocumentModelStructureChangedListener Members
		public void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
		}
		public void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		public void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		public void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.pieceTable, pieceTable));
			int index = indices.BinarySearch(newRunIndex);
			if (index < 0)
				index = ~index;
			ShiftRunIndex(index, 1);
		}
		public void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Debug.Assert(Object.ReferenceEquals(this.pieceTable, pieceTable));
			int index = indices.BinarySearch(runIndex);
			if (index >= 0) {
				indices.RemoveAt(index);
			}
			else
				index = ~index;
			ShiftRunIndex(index, -1);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		public void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Debug.Assert(Object.ReferenceEquals(this.pieceTable, pieceTable));
			int index = indices.BinarySearch(runIndex);
			if (index >= 0) {
				indices.Insert(index + 1, runIndex + 1);
				index += 2;
			}
			else
				index = ~index;
			ShiftRunIndex(index, 1);
		}
		public void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Debug.Assert(Object.ReferenceEquals(this.pieceTable, pieceTable));
			int index = indices.BinarySearch(joinedRunIndex);
			if (index >= 0) {
				index++;
				if (index < indices.Count && indices[index] == joinedRunIndex + 1)
					indices.RemoveAt(index);
			}
			else
				index = ~index;
			ShiftRunIndex(index, -1);
		}
		public void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		public void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		public void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
		}
		public void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
		}
		#endregion
		protected virtual void ShiftRunIndex(int startIndex, int delta) {
			int count = indices.Count;
			for (int i = startIndex; i < count; i++)
				indices[i] += delta;
		}
	}
	#endregion
}
