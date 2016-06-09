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
using System.ComponentModel;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker {
	public delegate void StrategyStateChangedEventHandler(object sender, StrategyStateChangedEventArgs e);
	public class StrategyStateChangedEventArgs : EventArgs {
		StrategyState state = StrategyState.Stop;
		public StrategyStateChangedEventArgs(StrategyState state) {
			this.state = state;
		}
		public StrategyState State { get { return state; } }
	}
	public delegate void ActiveErrorChangedEventHandler(object sender, ActiveErrorChangedEventArgs e);
	public class ActiveErrorChangedEventArgs  : EventArgs {
		SpellCheckErrorBase error = null;
		Position start = Position.Null;
		Position finish = Position.Null;
		public ActiveErrorChangedEventArgs(Position start, Position finish, SpellCheckErrorBase error) {
			this.start = start;
			this.finish = finish;
			this.error = error;
		}
		public SpellCheckErrorBase Error { get { return this.error; } }
		public Position Start { get { return this.start; } }
		public Position Finish { get { return this.finish; } }
	}
	public delegate void FinishCheckingMainPartEventHandler(object sender, FinishCheckingMainPartEventArgs e);
	public class FinishCheckingMainPartEventArgs :  EventArgs {
		bool needCheckRemainingPart = false;
		public FinishCheckingMainPartEventArgs(bool needCheckRemainingPart) {
			this.needCheckRemainingPart = needCheckRemainingPart;
		}
		public bool NeedCheckRemainingPart {
			get { return needCheckRemainingPart; }
			set { needCheckRemainingPart = value; }
		}
	}
	public delegate void BeforeCheckEventHandler(object sender, BeforeCheckEventArgs e);
	public class BeforeCheckEventArgs : CancelEventArgs {
		string text = string.Empty;
		object editControl = null;
		public BeforeCheckEventArgs(string text, object editControl, bool cancel) : base(cancel) {
			this.text = text;
			this.editControl = editControl;
		}
		public string Text { get { return text; } }
		public object EditControl { get { return editControl; } }
	}
	public delegate void RepeatedWordFoundEventHandler(object sender, RepeatedWordFoundEventArgs e);
	public class RepeatedWordFoundEventArgs : EventArgs { 
		SpellCheckOperation result;
		string suggestion;
		string word;
		bool handled;
		Position startPosition;
		Position length;
		public RepeatedWordFoundEventArgs(string word, SpellCheckOperation result, string suggestion, Position startPosition, Position length, bool handled)
			: base() {
			this.word = word;
			this.result = result;
			this.suggestion = suggestion;
			this.startPosition = startPosition;
			this.length = length;
			this.handled = handled;
		}
		public string Word { get { return word; } }
		public SpellCheckOperation Result { get { return result; } set { result = value; } }
		public virtual string Suggestion { get { return suggestion; } set { suggestion = value; } }
		public Position StartPosition { get { return startPosition; } }
		public Position Length { get { return length; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public delegate void NotInDictionaryWordFoundEventHandler(object sender, NotInDictionaryWordFoundEventArgs e);
	public class NotInDictionaryWordFoundEventArgs : RepeatedWordFoundEventArgs {
		#region Inner Classes
		abstract class SuggestionsAccessorBase {
			public abstract SuggestionCollection Suggestions { get;}
			public abstract string Suggestion { get; set; }
		}
		class InnerSuggestionsAccessor : SuggestionsAccessorBase {
			readonly NotInDictionaryWordFoundEventArgs args;
			public InnerSuggestionsAccessor(NotInDictionaryWordFoundEventArgs args) {
				this.args = args;
			}
			public override SuggestionCollection Suggestions { get { return args.InnerSuggestions; } }
			public override string Suggestion { get { return args.InnerSuggestion; } set { args.InnerSuggestion = value; } }
		}
		class ExternSuggestionsAccessor : SuggestionsAccessorBase {
			readonly NotInDictionaryWordError error;
			public ExternSuggestionsAccessor(NotInDictionaryWordError error) {
				this.error = error;
			}
			public override SuggestionCollection Suggestions { get { return error.Suggestions; } }
			public override string Suggestion { get { return error.Suggestion; } set { error.Suggestion = value; } }
		}
		#endregion
		readonly SuggestionCollection suggestions;
		readonly SuggestionsAccessorBase accessor;
		public NotInDictionaryWordFoundEventArgs(string word, SuggestionCollection suggestions, SpellCheckOperation result, string suggestion, Position startPosition, Position length, bool handled)
			: base(word, result, suggestion, startPosition, length, handled) {
			this.suggestions = suggestions;
			this.accessor = new InnerSuggestionsAccessor(this);
		}
		internal NotInDictionaryWordFoundEventArgs(NotInDictionaryWordError error, bool handled)
			: base(error.WrongWord, error.Result, null, error.StartPosition, new IntPosition(error.WrongWord.Length), handled) {
			this.accessor = new ExternSuggestionsAccessor(error);
		}
		string InnerSuggestion { get { return base.Suggestion; } set { base.Suggestion = value; } }
		SuggestionCollection InnerSuggestions { get { return suggestions; } }
		public SuggestionCollection Suggestions { get { return accessor.Suggestions; } }
		public override string Suggestion { get { return accessor.Suggestion; } set { accessor.Suggestion = value; } }
	}
	public delegate void PrepareSuggestionsEventHandler(object sender, PrepareSuggestionsEventArgs e);
	public class PrepareSuggestionsEventArgs : EventArgs {
		string word;
		SuggestionCollection suggestions;
		public PrepareSuggestionsEventArgs(string word, SuggestionCollection suggestions) {
			this.word = word;
			this.suggestions = suggestions;
		}
		public string Word { get { return word;} }
		public SuggestionCollection Suggestions { get { return suggestions; } }
	}
	public delegate void BeforeCheckWordEventHandler(object sender, BeforeCheckWordEventArgs e);
	public class BeforeCheckWordEventArgs : CancelEventArgs {
		object editControl;
		string word;
		public BeforeCheckWordEventArgs(object editControl, string word, bool cancel)
			: base(cancel) {
			this.editControl = editControl;
			this.word = word;
		}
		public object EditControl { get { return editControl; } }
		public string Word { get {return word;}}
	}
	public delegate void AfterCheckEventHandler(object sender, AfterCheckEventArgs e);
	public enum StopCheckingReason { User, Default }
	public class AfterCheckEventArgs : EventArgs {
		StopCheckingReason reason;
		public AfterCheckEventArgs(StopCheckingReason reason)
			: base() {
			this.reason = reason;
		}
		public StopCheckingReason Reason { get { return reason; } }
	}
	#region SpellCheckerUnhandledExceptionEventHandler
	public delegate void SpellCheckerUnhandledExceptionEventHandler(object sender, SpellCheckerUnhandledExceptionEventArgs e);
	#endregion
	#region SpellCheckerUnhandledExceptionEventArgs
	public class SpellCheckerUnhandledExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		bool handled;
		#endregion
		public SpellCheckerUnhandledExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	public delegate void SpellingFormShowingEventHandler(object sender, SpellingFormShowingEventArgs e);
	public class FormShowingEventArgs {
		bool handled;
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public delegate void FormShowingEventHandler(object sender, FormShowingEventArgs e);
	public class SpellingFormShowingEventArgs : FormShowingEventArgs {
		string word;
		StringCollection suggestions;
		public SpellingFormShowingEventArgs(string word, StringCollection suggestions) {
			this.word = word;
			this.suggestions = suggestions;
		}
		public string Word { get { return word; } }
		public StringCollection Suggestions { get { return suggestions; } }
	}
	public delegate void SpellingFormResultChangedEventHandler(object sender, SpellingFormResultChangedEventArgs e);
	public class SpellingFormResultChangedEventArgs : EventArgs {
		string suggestion;
		SpellCheckOperation operation;
		public SpellingFormResultChangedEventArgs(SpellCheckOperation operation, string suggestion) {
			this.operation = operation;
			this.suggestion = suggestion;
		}
		public SpellingFormResultChangedEventArgs(SpellCheckOperation result) : this(result, string.Empty) { }
		public string Suggestion { get { return suggestion; } }
		public SpellCheckOperation Operation { get { return operation; } }
	}
	public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
	public class TextChangedEventArgs : EventArgs {
		string text = string.Empty;
		public TextChangedEventArgs(string text) {
			this.text = text;
		}
		public string Text { get { return text; } }
	}
	internal delegate void DictionaryLoadingEventHandler(object sender, DictionaryLoadingEventAgrs e);
	internal class DictionaryLoadingEventAgrs : EventArgs {
		bool handled = false;
		public bool Handled { get { return handled; } set { handled = value; } }
	}
}
