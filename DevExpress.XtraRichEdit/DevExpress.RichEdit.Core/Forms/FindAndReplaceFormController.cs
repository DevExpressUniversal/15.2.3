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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Menu;
using System.Text;
namespace DevExpress.XtraRichEdit.Forms {
	public class StringsCollection : List<string> { }
	public enum SearchFormActivePage {
		Find,
		Replace,
		Undefined
	}
	public enum TextSearchDirection {
		All = 0,
		Down = 1,
		Up = 2
	}
	#region SearchCompleteMessageHelper
	public static class SearchCompleteMessageHelper {
		public static string GetFindInSingleDirectionCompleteMessage(SearchCompleteEventArgs e) {
			string messageTemplate = GetCommonMessageTemplate(e.SearchScope, e.Direction);
			return CreateFindCompleteMessage(e.MatchCount, messageTemplate);
		}
		public static string GetFindCompleteMessage(SearchCompleteEventArgs e) {
			string messageTemplate = GetSearchCompleteMessage() + " {0}";
			return CreateFindCompleteMessage(e.MatchCount, messageTemplate);
		}
		static string CreateFindCompleteMessage(int matchCount, string messageTemplate) {
			string resultMessage = matchCount > 0 ? String.Empty : GetSearchItemNotFoundMessage();
			return String.Format(messageTemplate, resultMessage);
		}
		public static string GetReplaceAllInSingleDirectionCompleteMessage(SearchCompleteEventArgs e) {
			string messageTemplate = GetCommonMessageTemplate(e.SearchScope, e.Direction);
			return CreateReplaceAllCompleteMessage(e.MatchCount, messageTemplate);
		}
		public static string GetReplaceAllCompleteMessage(SearchCompleteEventArgs e) {
			string messageTemplate = GetSearchCompleteMessage() + "{0}";
			return CreateReplaceAllCompleteMessage(e.MatchCount, messageTemplate);
		}
		static string CreateReplaceAllCompleteMessage(int matchCount, string messageTemplate) {
			string resultMessage = matchCount > 0 ? GetReplacemetsCountMessage(matchCount) : GetSearchItemNotFoundMessage();
			return String.Format(messageTemplate, resultMessage);
		}
		static string GetCommonMessageTemplate(SearchScope searchScope, Direction direction) {
			string searchResultMessage = GetIntermediateResultMessage(searchScope, direction);
			string continueSearchQuestion = GetContinueSearchQuestion(searchScope, direction);
			return searchResultMessage + " {0} " + continueSearchQuestion;
		}
		static string GetIntermediateResultMessage(SearchScope searchScope, Direction direction) {
			if (searchScope == SearchScope.SelectedText)
				return GetSearchInSelectionCompleteMessage();
			if (searchScope == SearchScope.BelowSelectedText && direction == Direction.Forward)
				return GetSearchInForwardDirectionCompleteMessage();
			if (searchScope == SearchScope.AboveSelectedText && direction == Direction.Backward)
				return GetSearchInBackwardDirectionCompleteMessage();
			return String.Empty;
		}
		static string GetContinueSearchQuestion(SearchScope searchScope, Direction direction) {
			if (searchScope == SearchScope.SelectedText)
				return GetContinueSearchInRemainderQuestion();
			if (searchScope == SearchScope.BelowSelectedText && direction == Direction.Forward)
				return GetContinueSearchFromBeginningQuestion();
			if (searchScope == SearchScope.AboveSelectedText && direction == Direction.Backward)
				return GetContinueSearchFromEndQuestion();
			return String.Empty;
		}
		static string GetSearchInSelectionCompleteMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SearchInSelectionComplete);
		}
		static string GetSearchInForwardDirectionCompleteMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SearchInForwardDirectionComplete);
		}
		static string GetSearchInBackwardDirectionCompleteMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SearchInBackwardDirectionComplete);
		}
		static string GetSearchCompleteMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SearchComplete);
		}
		static string GetContinueSearchInRemainderQuestion() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ContinueSearchInRemainderQuestion);
		}
		static string GetContinueSearchFromBeginningQuestion() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ContinueSearchFromBeginningQuestion);
		}
		static string GetContinueSearchFromEndQuestion() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ContinueSearchFromEndQuestion);
		}
		static string GetSearchItemNotFoundMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SearchItemNotFound);
		}
		static string GetReplacemetsCountMessage(int replacemetsCount) {
			return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ReplacementsCount), replacemetsCount);
		}
	}
	#endregion
	#region SearchFormControllerParameters
	public class SearchFormControllerParameters : FormControllerParameters {
		SearchFormActivePage activePage;
		internal SearchFormControllerParameters(IRichEditControl control, SearchFormActivePage activePage)
			: base(control) {
			this.activePage = activePage;
		}
		internal SearchFormActivePage ActivePage { get { return activePage; } set { activePage = value; } }
	}
	#endregion
	#region SearchFormControllerBase (abstract class)
	public abstract class SearchFormControllerBase : IDisposable {
		protected const string ParagraphStartPattern = @"(?:(?<=^)|(?<=\n))";
		protected const string ParagraphEndPattern = @"(?=\n)";
		#region Fields
		bool caseSensitive;
		bool findWholeWord;
		bool regularExpression;
		string searchString;
		string replaceString;
		readonly IRichEditControl control;
		SearchHelperBase searchHelper;
		#endregion
		protected SearchFormControllerBase(SearchFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.searchHelper = CreateSearchHelper();
		}
		#region Properties
		public string SearchString { get { return searchString; } set { searchString = value; } }
		public string ReplaceString { get { return replaceString; } set { replaceString = value; } }
		public bool FindWholeWord { get { return findWholeWord; } set { findWholeWord = value; } }
		public bool CaseSensitive { get { return caseSensitive; } set { caseSensitive = value; } }
		public bool RegularExpression { get { return regularExpression; } set { regularExpression = value; } }
		public IRichEditControl Control { get { return control; } }
		public TextSearchDirection Direction { get { return SearchHelper.Direction; } set { SearchHelper.Direction = value; } }
		protected SearchHelperBase SearchHelper { get { return searchHelper; } }
		DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		#endregion
		protected abstract SearchHelperBase CreateSearchHelper();
		public bool TryGetSearchStringFromSelection(out string result) {
			result = String.Empty;
			if (!DocumentModel.IsTextSelectedOnly())
				return false;
			string selectedText = GetSelectedText();
			if (String.IsNullOrEmpty(selectedText))
				return false;
			if (!ShouldSearchInSelection(selectedText)) {
				result = selectedText;
				return true;
			}
			return false;
		}
		protected internal string GetSelectedText() {
			return DocumentModel.GetSelectionText().Trim();
		}
		protected virtual bool ShouldSearchInSelection(string selectedText) {
			int textLength = selectedText.Length;
			for (int i = 0; i < textLength; i++) {
				if (Char.IsWhiteSpace(selectedText[i]))
					return true;
			}
			return false;
		}
		protected virtual void ValidateSelectedTextOnSearchPossibility() {
			if (!SearchContext.StartOfSearch)
				return;
			if (ShouldSearchInSelection(GetSelectedText()))
				SearchParameters.FindInSelection = true;
			else
				SearchParameters.FindInSelection = false;
		}
		public void Find() {
			ValidateSelectedTextOnSearchPossibility();
			PopulateSearchParameters();
			SearchHelper.ExecuteFindCommand();
		}
		public void Replace() {
			ValidateSelectedTextOnSearchPossibility();
			PopulateSearchParameters();
			SearchHelper.ExecuteReplaceCommand();
		}
		public void ReplaceAll() {
			ValidateSelectedTextOnSearchPossibility();
			PopulateSearchParameters();
			SearchHelper.ExecuteReplaceAllCommand();
		}
		protected internal void PopulateSearchParameters() {
			if (RegularExpression)
				SearchParameters.SearchString = GetRegularExpression();
			else
				SearchParameters.SearchString = SearchString;
			SearchParameters.UseRegularExpression = RegularExpression;
			SearchParameters.CaseSensitive = CaseSensitive;
			SearchParameters.FindWholeWord = FindWholeWord;
			SearchParameters.ReplaceString = ReplaceString;
		}
		protected internal virtual string GetRegularExpression() {
			if (IsParagraphExpression(SearchString))
				return String.Format("{0}\n", ParagraphStartPattern);
			int count = SearchString.Length;
			char[] chars = SearchString.ToCharArray();
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < count; i++) {
				if (chars[i] == '$')
					result.Append(ParagraphEndPattern);
				else if (chars[i] == '^' && (i == 0 || chars[i - 1] != '['))
					result.Append(ParagraphStartPattern);
				else
					result.Append(chars[i]);
			}
			return result.ToString();
		}
		bool IsParagraphExpression(string pattern) {
			return pattern.Equals("^$");
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.searchHelper != null) {
					this.searchHelper.Dispose();
					this.searchHelper = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region SearchHelperBase (abstract class)
	public abstract class SearchHelperBase : IDisposable {
		#region Fields
		readonly IRichEditControl control;
		TextSearchDirection searchDirection;
		#endregion
		protected SearchHelperBase(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public TextSearchDirection Direction { get { return searchDirection; } set { searchDirection = value; } }
		public IRichEditControl Control { get { return control; } }
		protected DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		protected SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		#endregion
		public virtual void ExecuteFindCommand() {
			try {
				FindNextAndSelectCommandBase cmd = CreateFindCommand();
				cmd.Execute();
			}
			catch (ArgumentException e) {
				OnExceptionsOccurs(e);
			}
		}
		protected virtual void OnExceptionsOccurs(Exception e) {
			if (DocumentModel.SearchParameters.UseRegularExpression)
				OnInvalidRegExp();
			else
				throw e;
		}
		protected virtual FindNextAndSelectCommandBase CreateFindCommand() {
			if (Direction == TextSearchDirection.Up)
				return new FindAndSelectBackwardCommand(Control);
			else
				return new FindAndSelectForwardCommand(Control);
		}
		public virtual void ExecuteReplaceCommand() {
			try {
				ReplaceCommandBase cmd = CreateReplaceCommand();
				cmd.Execute();
			}
			catch (ArgumentException e) {
				OnExceptionsOccurs(e);
			}
		}
		protected virtual ReplaceCommandBase CreateReplaceCommand() {
			if (Direction == TextSearchDirection.Up)
				return new ReplaceBackwardCommand(Control);
			else
				return new ReplaceForwardCommand(Control);
		}
		public virtual void ExecuteReplaceAllCommand() {
			try {
				ReplaceAllCommandBase cmd = CreateReplaceAllCommand();
				cmd.Execute();
			}
			catch (ArgumentException e) {
				OnExceptionsOccurs(e);
			}
		}
		protected virtual ReplaceAllCommandBase CreateReplaceAllCommand() {
			if (Direction == TextSearchDirection.Up)
				return new ReplaceAllBackwardCommand(Control);
			else
				return new ReplaceAllForwardCommand(Control);
		}
		protected abstract void OnStopSearching(string message);
		protected abstract bool ShouldContinueSearch(string message);
		protected virtual void OnInvalidRegExp(string message) {
		}
		protected virtual void OnInvalidRegExp() {
			OnInvalidRegExp(GetIncorrectRegExpMessage());
		}
		protected virtual void OnSearchComplete(SearchCompleteEventArgs e) {
			if (e.Cancel)
				return;
			if (e.Action == SearchAction.ReplaceAll)
				OnReplaceAllComplete(e);
			else
				OnFindComplete(e);
		}
		void OnFindComplete(SearchCompleteEventArgs e) {
			string message = GetFindInSingleDirectionCompleteMessage(e);
			if (!e.EntireDocument && ShouldContinueSearch(e.SearchScope, message))
				e.Continue = true;
			else {
				OnStopSearching(GetFindCompleteMessage(e));
				SearchContext.StopSearching();
			}
		}
		void OnReplaceAllComplete(SearchCompleteEventArgs e) {
			string message = GetReplaceAllInSingleDirectionCompleteMessage(e);
			if (!e.EntireDocument && ShouldContinueSearch(e.SearchScope, message))
				e.Continue = true;
			else {
				OnStopSearching(GetReplaceAllCompleteMessage(e));
				SearchContext.StopSearching();
			}
		}
		bool ShouldContinueSearch(SearchScope searchScope, string message) {
			if (Direction == TextSearchDirection.All && searchScope != SearchScope.AboveSelectedText)
				return true;
			return ShouldContinueSearch(message);
		}
		string GetFindInSingleDirectionCompleteMessage(SearchCompleteEventArgs e) {
			return SearchCompleteMessageHelper.GetFindInSingleDirectionCompleteMessage(e);
		}
		string GetFindCompleteMessage(SearchCompleteEventArgs e) {
			return SearchCompleteMessageHelper.GetFindCompleteMessage(e);
		}
		string GetReplaceAllInSingleDirectionCompleteMessage(SearchCompleteEventArgs e) {
			return SearchCompleteMessageHelper.GetReplaceAllInSingleDirectionCompleteMessage(e);
		}
		string GetReplaceAllCompleteMessage(SearchCompleteEventArgs e) {
			return SearchCompleteMessageHelper.GetReplaceAllCompleteMessage(e);
		}
		string GetIncorrectRegExpMessage() {
			return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_IncorrectPattern);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region IButtonEditAdapter
	public interface IButtonEditAdapter {
		string Text { get; set; }
		void Select(int start, int length);
	}
	#endregion
	#region InsertRegexItemCommand
	public class InsertRegexItemCommand : RichEditCommand {
		#region Fields
		readonly IButtonEditAdapter edit;
		readonly string menuCaption;
		readonly string insertStr;
		#endregion
		public InsertRegexItemCommand(IRichEditControl control, IButtonEditAdapter edit, XtraRichEditStringId captionStringId, string insertStr)
			: base(control) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
			this.menuCaption = String.Format("{0}\t{1}", XtraRichEditLocalizer.GetString(captionStringId), insertStr);
			this.insertStr = insertStr;
		}
		public InsertRegexItemCommand(IRichEditControl control, IButtonEditAdapter edit, string caption, string insertStr)
			: base(control) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
			this.menuCaption = caption;
			this.insertStr = insertStr;
		}
		#region Properties
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override string MenuCaption { get { return menuCaption; } }
		public override string Description { get { return menuCaption; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			edit.Text += insertStr;
			edit.Select(edit.Text.Length, 0);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Menu {
	#region SearchRegexPopupMenuBuilder
	public class SearchRegexPopupMenuBuilder : RichEditMenuBuilder {
		readonly IButtonEditAdapter edit;
		public SearchRegexPopupMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory, IButtonEditAdapter edit)
			: base(control, uiFactory) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_AnySingleCharacter, ".");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_ZeroOrMore, "*");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_OneOrMore, "+");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_BeginningOfLine, "^").BeginGroup = true;
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_EndOfLine, "$");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_BeginningOfWord, "\\b");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_EndOfWord, "\\B");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_AnyOneCharacterInTheSet, "[ ]").BeginGroup = true;
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_AnyOneCharacterNotInTheSet, "[^]");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_Or, "|");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_EscapeSpecialCharacter, "\\");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_TagExpression, "( )");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_WordCharacter, "\\w").BeginGroup = true;
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_SpaceOrTab, "\\s");
			AddMenuItem(menu, XtraRichEditStringId.FindAndReplaceForm_Integer, "\\d");
		}
		protected internal virtual IDXMenuItem<RichEditCommandId> AddMenuItem(IDXPopupMenu<RichEditCommandId> menu, XtraRichEditStringId captionStringId, string insertStr) {
			InsertRegexItemCommand command = new InsertRegexItemCommand(Control, edit, captionStringId, insertStr);
			return AddMenuItem(menu, command);
		}
	}
	#endregion
	#region ReplaceRegexPopupMenuBuilder
	public class ReplaceRegexPopupMenuBuilder : RichEditMenuBuilder {
		readonly IButtonEditAdapter edit;
		public ReplaceRegexPopupMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory, IButtonEditAdapter edit)
			: base(control, uiFactory) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			for (int i = 1; i < 10; i++) {
				string caption = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.FindAndReplaceForm_TaggedExpression), i);
				AddMenuItem(menu, caption, String.Format("${0}", i));
			}
		}
		protected internal virtual IDXMenuItem<RichEditCommandId> AddMenuItem(IDXPopupMenu<RichEditCommandId> menu, string caption, string insertStr) {
			InsertRegexItemCommand command = new InsertRegexItemCommand(Control, edit, caption, insertStr);
			return AddMenuItem(menu, command);
		}
	}
	#endregion
}
