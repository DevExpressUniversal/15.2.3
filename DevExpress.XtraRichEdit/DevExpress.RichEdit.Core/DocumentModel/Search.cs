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
	#region SearchParameters
	public class SearchParameters {
		#region Fields
		SearchOptions searchOptions;
		bool findInSelection;
		bool useRegularExpression;
		string searchString = String.Empty;
		string replaceString = String.Empty;
		#endregion
		#region Properties
		public bool CaseSensitive { get { return GetOption(SearchOptions.CaseSensitive); } set { SetOption(SearchOptions.CaseSensitive, value); } }
		public bool FindWholeWord { get { return GetOption(SearchOptions.WholeWord); } set { SetOption(SearchOptions.WholeWord, value); } }
		public bool UseRegularExpression { get { return useRegularExpression; } set { useRegularExpression = value; } }
		public bool FindInSelection { get { return findInSelection; } set { findInSelection = value; } }
		public string SearchString { get { return searchString; } set { searchString = value; } }
		public string ReplaceString { get { return replaceString; } set { replaceString = value; } }
		internal SearchOptions SearchOptions { get { return searchOptions; } }
		#endregion
		bool GetOption(SearchOptions option) {
			return (SearchOptions & option) != 0;
		}
		void SetOption(SearchOptions option, bool value) {
			if (value)
				this.searchOptions |= option;
			else
				this.searchOptions &= ~option;
		}
	}
	#endregion
	#region SearchContextInfo
	public class SearchContextInfo {
		#region Fields
		int matchCount;
		int replaceCount;
		bool startOfIntervalSearch = true;
		DocumentLogPosition lastResult = DocumentLogPosition.Zero;
		SearchState searchState = SearchState.FindStart;
		SearchScope searchScope = SearchScope.None;
		#endregion
		#region Properties
		public int MatchCount { get { return matchCount; } set { matchCount = value; } }
		public int ReplaceCount { get { return replaceCount; } set { replaceCount = value; } }
		public SearchState SearchState { get { return searchState; } set { searchState = value; } }
		public SearchScope SearchScope { get { return searchScope; } set { searchScope = value; } }
		public DocumentLogPosition LastResult { get { return lastResult; } set { lastResult = value; } }
		public bool StartOfIntervalSearch { get { return startOfIntervalSearch; } set { startOfIntervalSearch = value; } }
		#endregion
	}
	#endregion
	#region SearchContext
	public class SearchContext : IDocumentModelStructureChangedListener {
		static readonly Dictionary<int, SearchLogicalAction> LogicalActionsTable = CreateLogicalActionsTable();
		static Dictionary<int, SearchLogicalAction> CreateLogicalActionsTable() {
			Dictionary<int, SearchLogicalAction> result = new Dictionary<int, SearchLogicalAction>();
			result.Add(GetCommandHash(SearchAction.Find, Direction.Forward), SearchLogicalAction.FindReplaceForward);
			result.Add(GetCommandHash(SearchAction.Find, Direction.Backward), SearchLogicalAction.FindReplaceBackward);
			result.Add(GetCommandHash(SearchAction.Replace, Direction.Forward), SearchLogicalAction.FindReplaceForward);
			result.Add(GetCommandHash(SearchAction.Replace, Direction.Backward), SearchLogicalAction.FindReplaceBackward);
			result.Add(GetCommandHash(SearchAction.ReplaceAll, Direction.Forward), SearchLogicalAction.ReplaceAllForward);
			result.Add(GetCommandHash(SearchAction.ReplaceAll, Direction.Backward), SearchLogicalAction.ReplaceAllBackward);
			return result;
		}
		static int GetCommandHash(SearchAction action, Direction direction) {
			return ((int)action) << 3 | (int)direction;
		}
		#region Fields
		SearchContextInfo searchInfo;
		BufferedRegexSearchResult matchInfo;
		DocumentModelPositionAnchor startSelectionPosition;
		DocumentModelPositionAnchor endSelectionPosition;
		int searchOffset;
		readonly PieceTable pieceTable;
		SearchLogicalAction logicalAction = SearchLogicalAction.None;
		SearchAction action;
		Direction direction;
		int suspendCount;
		#endregion
		public SearchContext(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.searchInfo = new SearchContextInfo();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public SearchAction Action { get { return action; } }
		public Direction Direction { get { return direction; } }
		public SearchLogicalAction LogicalAction { get { return logicalAction; } }
		public bool IsExecuteLocked { get { return suspendCount > 0; } }
		public bool StartOfSearch { get { return SearchState == SearchState.FindStart; } }
		public bool EndOfSearch { get { return SearchState == SearchState.FindFinish; } }
		public int MatchCount { get { return SearchInfo.MatchCount; } set { SearchInfo.MatchCount = value; } }
		public int ReplaceCount { get { return SearchInfo.ReplaceCount; } set { SearchInfo.ReplaceCount = value; } }
		public bool StartOfIntervalSearch { get { return SearchInfo.StartOfIntervalSearch; } set { SearchInfo.StartOfIntervalSearch = value; } }
		public SearchState SearchState { get { return SearchInfo.SearchState; } set { SearchInfo.SearchState = value; } }
		public SearchScope SearchScope { get { return SearchInfo.SearchScope; } set { SearchInfo.SearchScope = value; } }
		public DocumentLogPosition StartAt { get { return SearchInfo.LastResult; } set { SearchInfo.LastResult = value; } }
		public DocumentLogPosition StartSelection { get { return StartSelectionAnchor.Position.LogPosition; } }
		public DocumentLogPosition EndSelection { get { return EndSelectionAnchor.Position.LogPosition; } }
		public BufferedRegexSearchResult MatchInfo { get { return matchInfo; } set { matchInfo = value; } }
		protected internal int SearchOffset { get { return searchOffset; } set { searchOffset = value; } }
		protected internal DocumentModelPositionAnchor StartSelectionAnchor { get { return startSelectionPosition; } set { startSelectionPosition = value; } }
		protected internal DocumentModelPositionAnchor EndSelectionAnchor { get { return endSelectionPosition; } set { endSelectionPosition = value; } }
		protected internal virtual SearchContextInfo SearchInfo { get { return searchInfo; } }
		#endregion
		public virtual void BeginSearch(SearchAction action, Direction direction) {
			if (!IsExecuteLocked) {
				this.action = action;
				this.direction = direction;
				SearchLogicalAction currentLogicalAction = LogicalActionsTable[GetCommandHash(action, direction)];
				if (this.logicalAction != currentLogicalAction) {
					Initialize();
					this.logicalAction = currentLogicalAction;
				}
				DisableHandling();
			}
			suspendCount++;
		}
		public virtual void EndSearch() {
			suspendCount--;
			if (suspendCount == 0) {
				if (EndOfSearch)
					Clear();
				else
					EnableHandling();
			}
		}
		protected internal void EnableHandling() {
			if (!IsExecuteLocked)
				DocumentModel.InnerSelectionChanged += OnSelectionChanged;
		}
		protected internal void DisableHandling() {
			DocumentModel.InnerSelectionChanged -= OnSelectionChanged;
		}
		protected internal void OnSelectionChanged(object sender, EventArgs e) {
			Clear();
			DisableHandling();
		}
		public virtual void StopSearching() {
			SearchState = SearchState.FindFinish;
		}
		protected internal void ClearCore() {
			ResetSearchInfo();
			StartSelectionAnchor = null;
			EndSelectionAnchor = null;
			MatchInfo = null;
		}
		public virtual void Clear() {
			ClearCore();
			this.logicalAction = SearchLogicalAction.None;
			Debug.WriteLine("SearchContext->Clear");
		}
		protected internal void ResetSearchInfo() {
			this.searchInfo = new SearchContextInfo();
		}
		protected internal void Initialize() {
			ClearCore();
			DocumentModelPosition startPos = DocumentModel.Selection.Interval.NormalizedStart.Clone();
			StartSelectionAnchor = new DocumentModelPositionAnchor(startPos);
			DocumentModelPosition endPos = DocumentModel.Selection.Interval.NormalizedEnd.Clone();
			EndSelectionAnchor = new DocumentModelPositionAnchor(endPos);
			Debug.WriteLine("SearchContext->Initialize");
		}
		internal SearchCompleteEventArgs CreateEventArgs(string searchString, string replaceString) {
			SearchCompleteEventArgs args = new SearchCompleteEventArgs(Action, Direction, SearchScope);
			args.SetMatchCount(MatchCount);
			args.SetReplaceCount(ReplaceCount);
			args.SetEntireDocument(SearchState == SearchState.FindFinish);
			args.SetSearchString(searchString);
			args.SetReplaceString(replaceString);
			return args;
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnFieldRemoved(pieceTable, fieldIndex);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnFieldRemoved(pieceTable, fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			if (StartSelectionAnchor != null)
				StartSelectionAnchor.OnFieldInserted(pieceTable, fieldIndex);
			if (EndSelectionAnchor != null)
				EndSelectionAnchor.OnFieldInserted(pieceTable, fieldIndex);
		}
		#endregion
	}
	#endregion
}
