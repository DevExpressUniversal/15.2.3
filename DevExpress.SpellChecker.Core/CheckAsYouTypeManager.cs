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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
namespace DevExpress.XtraSpellChecker.Native {
	public delegate void PaintErrorsDelegate(ArrayList errors);
	#region AsyncSearchStrategyBase
	public abstract class AsyncSearchStrategyBase : SimpleTextSearchStrategy {
		OptionsSpellingBase optionsSpelling;
		ArrayList errorList = new ArrayList();
		ErrorByWordCache errorByWordCache = new ErrorByWordCache();
		protected AsyncSearchStrategyBase(SpellCheckerBase spellChecker)
			: base(spellChecker, new SimpleTextController()) {
		}
		protected internal OptionsSpellingBase OptionsSpelling { get { return optionsSpelling; } }
		public ArrayList ErrorList { get { return errorList; } }
		protected ErrorByWordCache ErrorByWordCache { get { return this.errorByWordCache; } }
		protected override void Initialize() {
			this.optionsSpelling = CreateOptionsSpelling();
			base.Initialize();
		}
		protected override SpellCheckerStateBase CreateSpellCheckerState(StrategyState fState) {
			if (fState == StrategyState.Checking)
				return CreateAsyncCheckingState(fState);
			return base.CreateSpellCheckerState(fState);
		}
		protected abstract SpellCheckerStateBase CreateAsyncCheckingState(StrategyState state);
		protected abstract SpellCheckerRulesController CreateAsyncRulesController();
		protected abstract OptionsSpellingBase CreateOptionsSpelling();
		public virtual void ClearErrorList() {
			lock (ErrorList.SyncRoot)
				ErrorList.Clear();
		}
		protected override void OnClientHandlingError() {
			base.OnClientHandlingError();
			DoSpellCheckOperation(SpellCheckOperation.Ignore, String.Empty);
			SetState(StrategyState.Checking);
		}
		protected override void OnActiveErrorChanged(SpellCheckErrorBase oldError) {
			base.OnActiveErrorChanged(oldError);
			if (ActiveError == null)
				return;
			lock (ErrorList.SyncRoot) {
				if (ActiveError is NotInDictionaryWordError)
					ErrorByWordCache.Add(ActiveError);
				ErrorList.Add(ActiveError);
			}
		}
		protected override DevExpress.XtraSpellChecker.Rules.SpellCheckerRulesController CreateRulesController() {
			return CreateAsyncRulesController();
		}
		protected internal virtual SpellCheckErrorBase GetErrorByWord(string word) {
			return ErrorByWordCache[word];
		}
		public override void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion) {
			UpdateActiveError(operation, suggestion);
			AfterSpellCheckOperationComplete(operation);
		}
		protected override void Reset() {
			CurrentPosition = Position.Null;
			WordStartPosition = Position.Null;
			ActiveError = null;
			StateCache.Clear();
		}
	}
	#endregion
	#region AsyncSpellCheckerRulesController
	public class AsyncSpellCheckerErrorClassManager : SpellCheckerErrorClassManager {
		public override SpellCheckErrorBase CreateNotInDictionaryWordError(SearchStrategy strategy) {
			return new AsyncNotInDictionaryWordError(strategy);
		}
	}
	#endregion
	#region AsyncNotInDictionaryWordError
	public class AsyncNotInDictionaryWordError : NotInDictionaryWordError {
		public AsyncNotInDictionaryWordError(SearchStrategy strategy) : base(strategy) { }
		protected internal new AsyncSearchStrategyBase SearchStrategy { get { return base.SearchStrategy as AsyncSearchStrategyBase; } }
		protected override bool NotInDictionaryWordFound() {
			NotInDictionaryWordError error = SearchStrategy.GetErrorByWord(WrongWord) as NotInDictionaryWordError;
			if (error == null || error.InnerSuggestions == null)
				return base.NotInDictionaryWordFound();
			else {
				SuggestionCollection suggestions = new SuggestionCollection();
				suggestions.AddRange(error.InnerSuggestions);
				InnerSuggestions = suggestions;
				return OnNotInDictionaryWordFound();
			}
		}
	}
	#endregion
}
