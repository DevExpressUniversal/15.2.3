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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker {
	#region SpellCheckMode
	public enum SpellCheckMode {
		OnDemand,
		AsYouType
	}
	#endregion
	#region SpellCheckOperation
	public enum SpellCheckOperation {
		AddToDictionary,
		Cancel,
		Change,
		SilentChange,
		ChangeAll,
		Delete,
		Ignore,
		SilentIgnore,
		IgnoreAll,
		Options,
		Recheck,
		Undo,
		Custom,
		None
	}
	#endregion
	#region CheckWordResult
	public enum CheckWordResult {
		Misspelled,
		Correct,
		Ignored
	}
	#endregion
	#region IIgnoredList
	public interface IIgnoreList : IEnumerable<IIgnoreItem> {
		void Add(string word);
		void Add(Position start, Position end, string word);
		bool Contains(Position start, Position end, string word);
		bool Contains(string word);
		void Remove(string word);
		void Remove(Position start, Position end, string word);
		void Clear();
	}
	#endregion
	#region IIgnoreItem
	public interface IIgnoreItem {
		Position Start { get; }
		Position End { get; }
		string Word { get; }
	} 
	#endregion
	public interface ICheckSpellingResult {
		string Text { get; }
		bool HasError { get; }
		int Index { get; }
		int Length { get; }
		string Value { get; }
		CheckSpellingResultType Result { get; }
	}
	public enum SpellingError {
		Unknown,
		Misspelling,
		Repeating,
		Syntax
	}
	public interface ISpellingErrorInfo {
		SpellingError Error { get; }
		Position WordStartPosition { get; }
		Position WordEndPosition { get; }
		string Word { get; }
	}
	#region ISpellChecker
	public interface ISpellChecker {
		bool IsChecking { get; }
		SpellCheckMode SpellCheckMode { get; set; }
		CultureInfo Culture { get; set; }
		void Check(object control);
		ISpellingErrorInfo Check(object control, ISpellCheckTextController controller, Position from, Position to);
		ICheckSpellingResult CheckText(object control, string text, int index, CultureInfo culture);
		void RegisterIgnoreList(object control, IIgnoreList ignoreList);
		void UnregisterIgnoreList(object control);
		bool CanAddToDictionary();
		bool CanAddToDictionary(CultureInfo culture);
		void AddToDictionary(string word);
		void AddToDictionary(string word, CultureInfo culture);
		void Ignore(object control, string word, Position start, Position end);
		void IgnoreAll(object control, string word);
		IOptionsSpellings GetOptions(object control);
		string[] GetSuggestions(string word, CultureInfo culture);
		event EventHandler CultureChanged;
		event EventHandler SpellCheckModeChanged;
		event AfterCheckWordEventHandler AfterCheckWord;
		event WordAddedEventHandler WordAdded;
		event EventHandler CustomDictionaryChanged;
	}
	#endregion
	public enum CheckSpellingResultType {
		Misspelling,
		Repeating,
		Success
	}
	public enum WordType {
		Misspelled,
		Repeated,
		Correct
	}
	public interface IOptionsSpellings {
		DefaultBoolean CheckFromCursorPos { get; set; }
		DefaultBoolean CheckSelectedTextFirst { get; set; }
		DefaultBoolean IgnoreEmails { get; set; }
		DefaultBoolean IgnoreMarkupTags { get; set; }
		DefaultBoolean IgnoreMixedCaseWords { get; set; }
		DefaultBoolean IgnoreRepeatedWords { get; set; }
		DefaultBoolean IgnoreUpperCaseWords { get; set; }
		DefaultBoolean IgnoreUrls { get; set; }
		DefaultBoolean IgnoreWordsWithNumbers { get; set; }
		event EventHandler OptionChanged;
		void BeginUpdate();
		void EndUpdate();
	}
	#region AfterCheckWordEventArgs
	public class AfterCheckWordEventArgs : EventArgs {
		object editControl;
		string originalWord;
		string changedWord;
		SpellCheckOperation operation;
		Position startPosition;
		public AfterCheckWordEventArgs(object editControl, string originalWord, string changedWord, SpellCheckOperation result, Position startPosition) {
			this.editControl = editControl;
			this.changedWord = changedWord;
			this.originalWord = originalWord;
			this.operation = result;
			this.startPosition = startPosition;
		}
		public object EditControl { get { return editControl; } }
		public string OriginalWord { get { return originalWord; } }
		public string ChangedWord { get { return changedWord; } }
		public SpellCheckOperation Operation { get { return operation; } }
		public Position StartPosition { get { return startPosition; } }
	}
	#endregion
	#region AfterCheckWordEventHandler
	public delegate void AfterCheckWordEventHandler(object sender, AfterCheckWordEventArgs e);
	#endregion
	#region WordAddedEventArgs
	public class WordAddedEventArgs : EventArgs {
		string word;
		public WordAddedEventArgs(string word) {
			this.word = word;
		}
		public string Word { get { return word; } }
	}
	#endregion
	#region WordAddedEventHandler
	public delegate void WordAddedEventHandler(object sender, WordAddedEventArgs e);
	#endregion
}
