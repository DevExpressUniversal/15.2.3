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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Model {
	#region EditingLogicalAction
	public enum SearchLogicalAction {
		None,
		FindReplaceForward,
		FindReplaceBackward,
		ReplaceAllForward,
		ReplaceAllBackward
	}
	#endregion
	#region SearchAction
	public enum SearchAction {
		Find = 0x00000001,
		Replace = 0x00000002,
		ReplaceAll = 0x00000004
	}
	#endregion
	#region Direction
	public enum Direction {
		Forward = 1,
		Backward = 2
	}
	#endregion
	#region SearchScope
	public enum SearchScope {
		None = 0,
		SelectedText = 1,
		AboveSelectedText = 2,
		BelowSelectedText = 3
	}
	#endregion
	#region TextSearchType
	public enum SearchState {
		FindStart,
		FindInSelection,
		FindAfterSelection,
		FindBeforeSelection,
		FindAfterCaret,
		FindBeforeCaret,
		FindFinish
	}
	#endregion
	#region TextSearchStateBase
	public abstract class TextSearchStateBase {
		readonly TextSearchProvider searchProvider;
		protected TextSearchStateBase(TextSearchProvider searchProvider) {
			Guard.ArgumentNotNull(searchProvider, "searchProvider");
			this.searchProvider = searchProvider;
		}
		public abstract SearchScope SearchScope { get; }
		public abstract SearchState Type { get; }
		public abstract bool ShouldSearch { get; }
		protected TextSearchProvider SearchProvider { get { return searchProvider; } }
		protected DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		protected SearchParameters SearchParameters { get { return SearchProvider.Parameters; } }
		protected SearchContext SearchContext { get { return SearchProvider.Context; } }
		protected PieceTable PieceTable { get { return SearchProvider.PieceTable; } }
		protected int SearchLimitOffset {
			get {
				if (!SearchParameters.UseRegularExpression)
					return SearchParameters.SearchString.Length - 1;
				return 0;
			}
		}
		public virtual SearchResult FindNext(DocumentLogPosition startPosition) {
			if (!(ShouldSearch && CanSearch(startPosition)))
				return new SearchResult();
			return DoSearch(startPosition);
		}
		protected internal SearchResult DoSearch(DocumentLogPosition start, DocumentLogPosition end) {
			return SearchProvider.FindRunInfo(start, end);
		}
		protected internal virtual bool CanSearch(DocumentLogPosition startPosition) {
			return SearchContext.StartOfIntervalSearch || CanSearchFromPosition(startPosition);
		}
		public abstract SearchState GetNextStateType();
		protected abstract SearchResult DoSearch(DocumentLogPosition startPosition);
		protected abstract bool CanSearchFromPosition(DocumentLogPosition startPosition);
	}
	#endregion
	#region FindStartState
	public class FindStartState : TextSearchStateBase {
		public FindStartState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { throw new Exception("Incorrect calling"); } }
		public override SearchState Type { get { return SearchState.FindStart; } }
		public override bool ShouldSearch { get { return true; } }
		#endregion
		public override SearchResult FindNext(DocumentLogPosition startPosition) {
			if (SearchParameters.FindInSelection)
				SearchProvider.ChangeState(SearchState.FindInSelection);
			else
				SearchProvider.ChangeState(SearchState.FindAfterCaret);
			return SearchProvider.State.FindNext(startPosition);
		}
		public override SearchState GetNextStateType() {
			throw new Exception("Incorrect calling");
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			throw new Exception("Incorrect calling");
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return true;
		}
	}
	#endregion
	#region FindInSelectionForwardState
	public class FindInSelectionForwardState : TextSearchStateBase {
		public FindInSelectionForwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.SelectedText; } }
		public override SearchState Type { get { return SearchState.FindInSelection; } }
		public override bool ShouldSearch { get { return (SearchContext.EndSelection > SearchContext.StartSelection); } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindAfterSelection;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(SearchContext.StartSelection, SearchContext.EndSelection);
			return DoSearch(startPosition, SearchContext.EndSelection);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition < SearchContext.EndSelection;
		}
	}
	#endregion
	#region FindAfterSelectionForwardState
	public class FindAfterSelectionForwardState : TextSearchStateBase {
		public FindAfterSelectionForwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.BelowSelectedText; } }
		public override SearchState Type { get { return SearchState.FindAfterSelection; } }
		public override bool ShouldSearch { get { return SearchContext.EndSelection < PieceTable.DocumentEndLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			if (SearchContext.StartSelection == PieceTable.DocumentStartLogPosition) {
				return SearchState.FindFinish;
			}
			return SearchState.FindBeforeSelection;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch) {
				DocumentLogPosition startPos = Algorithms.Max(PieceTable.DocumentStartLogPosition, SearchContext.EndSelection - SearchLimitOffset);
				return DoSearch(startPos, PieceTable.DocumentEndLogPosition);
			}
			return DoSearch(startPosition, PieceTable.DocumentEndLogPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition <= PieceTable.DocumentEndLogPosition;
		}
	}
	#endregion
	#region FindBeforeSelectionForwardState
	public class FindBeforeSelectionForwardState : TextSearchStateBase {
		public FindBeforeSelectionForwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.AboveSelectedText; } }
		public override SearchState Type { get { return SearchState.FindBeforeSelection; } }
		public override bool ShouldSearch { get { return SearchContext.StartSelection > PieceTable.DocumentStartLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindFinish;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			DocumentLogPosition endPosition = Algorithms.Min(SearchContext.StartSelection + SearchLimitOffset, PieceTable.DocumentEndLogPosition);
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(PieceTable.DocumentStartLogPosition, endPosition);
			return DoSearch(startPosition, endPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition < SearchContext.StartSelection;
		}
	}
	#endregion
	#region FindAfterCaretForwardState
	public class FindAfterCaretForwardState : TextSearchStateBase {
		public FindAfterCaretForwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.BelowSelectedText; } }
		public override SearchState Type { get { return SearchState.FindAfterCaret; } }
		public override bool ShouldSearch { get { return SearchContext.EndSelection < PieceTable.DocumentEndLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			if (SearchContext.EndSelection == PieceTable.DocumentStartLogPosition)
				return SearchState.FindFinish;
			return SearchState.FindBeforeCaret;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(SearchContext.EndSelection, PieceTable.DocumentEndLogPosition);
			return DoSearch(startPosition, PieceTable.DocumentEndLogPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition < PieceTable.DocumentEndLogPosition;
		}
	}
	#endregion
	#region FindBeforeCaretForwardState
	public class FindBeforeCaretForwardState : TextSearchStateBase {
		public FindBeforeCaretForwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.AboveSelectedText; } }
		public override SearchState Type { get { return SearchState.FindBeforeCaret; } }
		public override bool ShouldSearch { get { return SearchContext.EndSelection > PieceTable.DocumentStartLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindFinish;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			DocumentLogPosition endPosition = Algorithms.Min(SearchContext.EndSelection + SearchLimitOffset, PieceTable.DocumentEndLogPosition);
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(PieceTable.DocumentStartLogPosition, endPosition);
			return DoSearch(startPosition, endPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition < SearchContext.EndSelection;
		}
	}
	#endregion
	#region FindFinishState
	public class FindFinishState : TextSearchStateBase {
		public FindFinishState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { throw new Exception("Incorrect calling."); } }
		public override SearchState Type { get { return SearchState.FindFinish; } }
		public override bool ShouldSearch { get { return false; } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindStart;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			Exceptions.ThrowInternalException();
			return false;
		}
	}
	#endregion
	#region FindInSelectionBackwardState
	public class FindInSelectionBackwardState : TextSearchStateBase {
		public FindInSelectionBackwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.SelectedText; } }
		public override SearchState Type { get { return SearchState.FindInSelection; } }
		public override bool ShouldSearch { get { return (SearchContext.EndSelection > SearchContext.StartSelection); } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindAfterSelection;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(SearchContext.StartSelection, SearchContext.EndSelection);
			return DoSearch(SearchContext.StartSelection, startPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition > SearchContext.StartSelection;
		}
	}
	#endregion
	#region FindAfterSelectionBackwardState
	public class FindAfterSelectionBackwardState : TextSearchStateBase {
		public FindAfterSelectionBackwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.AboveSelectedText; } }
		public override SearchState Type { get { return SearchState.FindAfterSelection; } }
		public override bool ShouldSearch { get { return SearchContext.StartSelection > PieceTable.DocumentStartLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			if (SearchContext.EndSelection == PieceTable.DocumentEndLogPosition)
				return SearchState.FindFinish;
			return SearchState.FindBeforeSelection;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch) {
				DocumentLogPosition endPos = Algorithms.Min(SearchContext.StartSelection + SearchLimitOffset, PieceTable.DocumentEndLogPosition);
				return DoSearch(PieceTable.DocumentStartLogPosition, endPos);
			}
			return DoSearch(PieceTable.DocumentStartLogPosition, startPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition >= PieceTable.DocumentStartLogPosition;
		}
	}
	#endregion
	#region FindBeforeSelectionBackwardState
	public class FindBeforeSelectionBackwardState : TextSearchStateBase {
		public FindBeforeSelectionBackwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.BelowSelectedText; } }
		public override SearchState Type { get { return SearchState.FindBeforeSelection; } }
		public override bool ShouldSearch { get { return SearchContext.EndSelection < PieceTable.DocumentEndLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindFinish;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			DocumentLogPosition startPos = Algorithms.Max(PieceTable.DocumentStartLogPosition, SearchContext.EndSelection - SearchLimitOffset);
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(startPos, PieceTable.DocumentEndLogPosition);
			return DoSearch(startPos, startPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition >= SearchContext.EndSelection;
		}
	}
	#endregion
	#region FindAfterCaretBackwardState
	public class FindAfterCaretBackwardState : TextSearchStateBase {
		public FindAfterCaretBackwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.AboveSelectedText; } }
		public override SearchState Type { get { return SearchState.FindAfterCaret; } }
		public override bool ShouldSearch { get { return SearchContext.StartSelection > PieceTable.DocumentStartLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			if (SearchContext.StartSelection == PieceTable.DocumentEndLogPosition)
				return SearchState.FindFinish;
			return SearchState.FindBeforeCaret;
		}
		protected override SearchResult DoSearch(DocumentLogPosition startPosition) {
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(PieceTable.DocumentStartLogPosition, SearchContext.StartSelection);
			return DoSearch(PieceTable.DocumentStartLogPosition, startPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition >= PieceTable.DocumentStartLogPosition;
		}
	}
	#endregion
	#region FindBeforeCaretBackwardState
	public class FindBeforeCaretBackwardState : TextSearchStateBase {
		public FindBeforeCaretBackwardState(TextSearchProvider searchController)
			: base(searchController) {
		}
		#region Properties
		public override SearchScope SearchScope { get { return SearchScope.BelowSelectedText; } }
		public override SearchState Type { get { return SearchState.FindBeforeCaret; } }
		public override bool ShouldSearch { get { return SearchContext.StartSelection < PieceTable.DocumentEndLogPosition; } }
		#endregion
		public override SearchState GetNextStateType() {
			return SearchState.FindFinish;
		}
		protected override SearchResult DoSearch(DocumentLogPosition endPosition) {
			DocumentLogPosition startPos = Algorithms.Max(SearchContext.StartSelection - SearchLimitOffset, PieceTable.DocumentStartLogPosition);
			if (SearchContext.StartOfIntervalSearch)
				return DoSearch(startPos, PieceTable.DocumentEndLogPosition);
			return DoSearch(startPos, endPosition);
		}
		protected override bool CanSearchFromPosition(DocumentLogPosition startPosition) {
			return startPosition >= SearchContext.StartSelection;
		}
	}
	#endregion
	#region TextSearchProvider (abstract class)
	public abstract class TextSearchProvider {
		#region Fields
		readonly PieceTable pieceTable;
		TextSearchStateBase state;
		readonly SearchParameters parameters;
		readonly SearchContext context;
		#endregion
		protected TextSearchProvider(PieceTable pieceTable, SearchParameters parameters, SearchContext context) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(parameters, "parameters");
			Guard.ArgumentNotNull(context, "context");
			this.pieceTable = pieceTable;
			this.parameters = parameters;
			this.context = context;
			SetState(context.SearchState);
		}
		#region Properties
		public TextSearchStateBase State { get { return state; } internal set { state = value; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		protected internal SearchParameters Parameters { get { return parameters; } }
		protected internal SearchContext Context { get { return context; } }
		#endregion
		public virtual RunInfo FindNextPosition() {
			if (Context.SearchState == SearchState.FindFinish)
				return null;
			SearchResult result = State.FindNext(Context.StartAt);
			if (result.Success) {
				Context.StartOfIntervalSearch = false;
				Context.StartAt = GetStartAtPosition(result.Value);
				Context.MatchCount++;
				RegexSearchResult regexResult = result as RegexSearchResult;
				if (regexResult != null)
					Context.MatchInfo = regexResult.MatchInfo;
				else
					Context.MatchInfo = null;
			}
			else {
				Context.StartOfIntervalSearch = true;
				Context.SearchState = State.GetNextStateType();
			}
			Context.SearchScope = State.SearchScope;
			return result.Value;
		}
		protected internal SearchResult FindRunInfo(DocumentLogPosition start, DocumentLogPosition end) {
			ISearchStrategy searchStrategy = CreateSearchStrategy();
			return searchStrategy.Match(Parameters.SearchString, Parameters.SearchOptions, start, end);
		}
		protected internal virtual ISearchStrategy CreateSearchStrategy() {
			return Parameters.UseRegularExpression ? CreateRegexSearch() : CreateStringSearch();
		}
		public void ChangeState(SearchState searchType) {
			SetState(searchType);
			Context.SearchState = searchType;
		}
		public abstract void SetState(SearchState searchType);
		protected internal abstract DocumentLogPosition GetStartAtPosition(RunInfo runInfo);
		protected internal abstract ISearchStrategy CreateStringSearch();
		protected internal abstract ISearchStrategy CreateRegexSearch();
	}
	#endregion
	#region TextSearchForwardProvider
	public class TextSearchForwardProvider : TextSearchProvider {
		public TextSearchForwardProvider(PieceTable pieceTable, SearchParameters parameters, SearchContext context)
			: base(pieceTable, parameters, context) {
		}
		public override void SetState(SearchState searchType) {
			switch (searchType) {
				case SearchState.FindStart:
					State = new FindStartState(this);
					break;
				case SearchState.FindInSelection:
					State = new FindInSelectionForwardState(this);
					break;
				case SearchState.FindAfterCaret:
					State = new FindAfterCaretForwardState(this);
					break;
				case SearchState.FindBeforeCaret:
					State = new FindBeforeCaretForwardState(this);
					break;
				case SearchState.FindAfterSelection:
					State = new FindAfterSelectionForwardState(this);
					break;
				case SearchState.FindBeforeSelection:
					State = new FindBeforeSelectionForwardState(this);
					break;
				case SearchState.FindFinish:
					State = new FindFinishState(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal override ISearchStrategy CreateStringSearch() {
			return new TextSearchByStringForwardStrategy(PieceTable);
		}
		protected internal override ISearchStrategy CreateRegexSearch() {
			return new TextSearchByRegexForwardStrategy(PieceTable);
		}
		protected internal override DocumentLogPosition GetStartAtPosition(RunInfo runInfo) {
			if (Context.Action == SearchAction.Find) {
				if (Parameters.UseRegularExpression) {
					if (runInfo.Start < runInfo.End)
						return runInfo.End.LogPosition;
					else
						return GetNextVisiblePosition(runInfo.End);
				}
				else
					return GetNextVisiblePosition(runInfo.Start);
			}
			else {
				int offset = 0;
				if (Parameters.UseRegularExpression && Context.MatchInfo != null)
					offset = Context.MatchInfo.Match.Result(Parameters.ReplaceString).Length;
				else
					offset = Parameters.ReplaceString.Length;
				if (offset == 0 && runInfo.End == runInfo.Start)
					offset = 1;
				return runInfo.Start.LogPosition + offset;
			}
		}
		DocumentLogPosition GetNextVisiblePosition(DocumentModelPosition position) {
			return GetNextVisiblePosition(position.LogPosition);
		}
		DocumentLogPosition GetNextVisiblePosition(DocumentLogPosition position) {
			DocumentLogPosition nextPosition = position + 1;
			if (nextPosition > PieceTable.DocumentEndLogPosition)
				return nextPosition;
			DocumentModelPosition modelPosition = PositionConverter.ToDocumentModelPosition(PieceTable, nextPosition);
			if (PieceTable.VisibleTextFilter.IsRunVisible(modelPosition.RunIndex))
				return nextPosition;
			return PieceTable.VisibleTextFilter.GetNextVisibleLogPosition(modelPosition, false);
		}
	}
	#endregion
	#region TextSearchBackwardProvider
	public class TextSearchBackwardProvider : TextSearchProvider {
		public TextSearchBackwardProvider(PieceTable pieceTable, SearchParameters parameters, SearchContext context)
			: base(pieceTable, parameters, context) {
		}
		public override void SetState(SearchState searchType) {
			switch (searchType) {
				case SearchState.FindStart:
					State = new FindStartState(this);
					break;
				case SearchState.FindInSelection:
					State = new FindInSelectionBackwardState(this);
					break;
				case SearchState.FindAfterCaret:
					State = new FindAfterCaretBackwardState(this);
					break;
				case SearchState.FindBeforeCaret:
					State = new FindBeforeCaretBackwardState(this);
					break;
				case SearchState.FindAfterSelection:
					State = new FindAfterSelectionBackwardState(this);
					break;
				case SearchState.FindBeforeSelection:
					State = new FindBeforeSelectionBackwardState(this);
					break;
				case SearchState.FindFinish:
					State = new FindFinishState(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal override ISearchStrategy CreateStringSearch() {
			return new TextSearchByStringBackwardStrategy(PieceTable);
		}
		protected internal override ISearchStrategy CreateRegexSearch() {
			return new TextSearchByRegexBackwardStrategy(PieceTable);
		}
		protected internal override DocumentLogPosition GetStartAtPosition(RunInfo runInfo) {
			if (Context.Action == SearchAction.Find) {
				if (Parameters.UseRegularExpression) {
					if (runInfo.Start < runInfo.End)
						return runInfo.Start.LogPosition;
					else
						return GetPrevVisiblePosition(runInfo.Start);
				}
				else
					return GetPrevVisiblePosition(runInfo.End);
			}
			else
				return GetPrevVisiblePosition(runInfo.Start);
		}
		DocumentLogPosition GetPrevVisiblePosition(DocumentModelPosition position) {
			return GetPrevVisiblePosition(position.LogPosition);
		}
		DocumentLogPosition GetPrevVisiblePosition(DocumentLogPosition position) {
			DocumentLogPosition prevPosition = position - 1;
			if (prevPosition < PieceTable.DocumentStartLogPosition)
				return position;
			DocumentModelPosition modelPosition = PositionConverter.ToDocumentModelPosition(PieceTable, prevPosition);
			if (PieceTable.VisibleTextFilter.IsRunVisible(modelPosition.RunIndex))
				return prevPosition;
			return PieceTable.VisibleTextFilter.GetPrevVisibleLogPosition(modelPosition, true);
		}
	}
	#endregion
}
