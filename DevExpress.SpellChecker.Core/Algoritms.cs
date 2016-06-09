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
using System.Text;
using System.Globalization;
using System.Collections;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Native;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Utils;
using DevExpress.XtraSpellChecker.Parser;
namespace DevExpress.XtraSpellChecker.Algorithms {
	#region Old operations
	#endregion
	public delegate bool GetStopStatus();
	public abstract class AlgorithmBase {
		StringBuilder input;
		ISpellCheckerDictionary dictionary;
		protected AlgorithmBase(string input, ISpellCheckerDictionary dictionary) {
			this.input = new StringBuilder(input);
			this.dictionary = dictionary;
		}
		public StringBuilder Input {
			get { return input; }
		}
		public ISpellCheckerDictionary Dictionary {
			get { return dictionary; }
		}
		public abstract SuggestionCollection GetSuggestions(int distance, GetStopStatus handler);
	}
	public class OperationCollection : List<CreateOperation> {
	}
	public interface IOperationResult : IDisposable {
		SuggestionCollection GetSuggestions();
	}
	#region OperationResult
	public class OperationResult : IOperationResult {
		readonly SuggestionCollection suggestions;
		public OperationResult() {
			this.suggestions = new SuggestionCollection();
		}
		internal void AddSuggestion(SuggestionBase suggestion) {
			this.suggestions.Add(suggestion);
		}
		public SuggestionCollection GetSuggestions() {
			return suggestions;
		}
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#endregion
	#region AsyncOperationResult
	public class AsyncOperationResult : IOperationResult {
		class SuggestionComparer : IComparer<SuggestionBase> {
			readonly CultureInfo culture;
			public SuggestionComparer(CultureInfo culture) {
				this.culture = culture;
			}
			#region IComparer<SuggestionBase> Members
			public int Compare(SuggestionBase x, SuggestionBase y) {
#if !SL
				return String.Compare(x.Suggestion, y.Suggestion, false, culture);
#else
				return String.Compare(x.Suggestion, y.Suggestion, culture, CompareOptions.None);
#endif
			}
			#endregion
		}
		#region Filelds
		static readonly object locker = new object();
		readonly List<SuggestionBase> suggestions;
		readonly SuggestionComparer comparer;
		ManualResetEvent resetEvent;
		long outstanding;
		int timeOut;
		bool disposed;
		#endregion
		internal AsyncOperationResult(int timeOut, CultureInfo culture) {
			this.timeOut = timeOut;
			this.comparer = new SuggestionComparer(culture);
			this.suggestions = new List<SuggestionBase>();
			this.resetEvent = new ManualResetEvent(true);
		}
		public bool Disposed { get { return disposed; } }
		public virtual SuggestionCollection GetSuggestions() {
			this.resetEvent.WaitOne(timeOut);
			SuggestionCollection result = new SuggestionCollection();
			lock (locker) {
				result.AddRange(suggestions);
			}
			return result;
		}
		internal void AddSuggestion(SuggestionBase suggestion) {
			lock (locker) {
				AddSuggestionCore(suggestion);
			}
		}
		void AddSuggestionCore(SuggestionBase suggestion) {
			int index = suggestions.BinarySearch(suggestion, comparer);
			if (index < 0) {
				index = ~index;
				suggestions.Insert(index, suggestion);
			}
		}
		internal void OnCalculationStart() {
			if (Interlocked.Increment(ref outstanding) == 1) {
				if (!Disposed)
					this.resetEvent.Reset();
			}
		}
		internal void OnCalculationEnd() {
			if (Interlocked.Decrement(ref outstanding) == 0) {
				if (!Disposed)
					this.resetEvent.Set();
			}
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (this.disposed)
				return;
			if (disposing)
				this.resetEvent.Close();
			this.disposed = true;
		}
		~AsyncOperationResult() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
#if OLDOPERATIONS
	public delegate OperationBase CreateOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut);
	public class NearMissStrategy {
		static OperationCollection availableOperations = CreateAvailableOperations();
		static OperationCollection CreateAvailableOperations() {
			OperationCollection result = new OperationCollection();
			result.Add(CreateCapitalizeFirstLetterOperation);
			result.Add(CreateCapitalizeAllLettersOperation);
			result.Add(CreateReplaceTypicalFaultOperation);
			result.Add(CreateInterchangeTwoAdjacentLetters);
			result.Add(CreateInterchangeTwoNotAdjacentLetters);
			result.Add(CreateDeleteLetterOperation);
			result.Add(CreateInsertLetterOperation);
			result.Add(CreateChangeOneLetterOperation);
			result.Add(CreateInsertSpaceOperation);
			return result;
		}
		static OperationBase CreateReplaceTypicalFaultOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateReplaceTypicalFaultOperation(dictionary, timeOut);
		}
		static OperationBase CreateCapitalizeFirstLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateCapitalizeFirstLetterOperation(dictionary, timeOut);
		}
		static OperationBase CreateCapitalizeAllLettersOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateCapitalizeAllLettersOperation(dictionary, timeOut);
		}
		static OperationBase CreateInterchangeTwoAdjacentLetters(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateInterchangeTwoAdjacentLetters(dictionary, timeOut);
		}
		static OperationBase CreateInterchangeTwoNotAdjacentLetters(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateInterchangeTwoNotAdjacentLetters(dictionary, timeOut);
		}
		static OperationBase CreateDeleteLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateDeleteLetterOperation(dictionary, timeOut);
		}
		static OperationBase CreateInsertLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateInsertLetterOperation(dictionary, timeOut);
		}
		static OperationBase CreateChangeOneLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateChangeOneLetterOperation(dictionary, timeOut);
		}
		static OperationBase CreateInsertSpaceOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary, int timeOut) {
			return strategy.CreateInsertSpaceOperation(dictionary, timeOut);
		}
		readonly ISpellCheckerDictionary dictionary;
		public NearMissStrategy(ISpellCheckerDictionary dictionary) {
			Guard.ArgumentNotNull(dictionary, "dictionary");
			this.dictionary = dictionary;
		}
		public virtual OperationCollection AvailableOperations { get { return availableOperations; } }
		public ISpellCheckerDictionary Dictionary { get { return dictionary; } }
		public virtual SuggestionCollection GetSuggestions(string word, int maxDistance, int timeOut) {
			if (!Dictionary.CaseSensitive)
				word = word.ToLower(Dictionary.Culture);
			List<IOperationResult> results = BeginCalculation(word, maxDistance, timeOut);
			SuggestionCollection suggestions = new SuggestionCollection();
			PopulateSuggestions(suggestions, results, word);
			return suggestions;
		}
		List<IOperationResult> BeginCalculation(string word, int maxDistance, int timeOut) {
			List<IOperationResult> results = new List<IOperationResult>();
			int count = AvailableOperations.Count;
			for (int i = 0; i < count; i++) {
				CreateOperation createOperation = AvailableOperations[i];
				OperationBase operation = createOperation(this, Dictionary, timeOut);
				results.Add(operation.GenerateSuggestions(word, maxDistance));
			}
			return results;
		}
		void PopulateSuggestions(SuggestionCollection suggestions, List<IOperationResult> results, string word) {
			int count = results.Count;
			for (int i = 0; i < count; i++) {
				IOperationResult result = results[i];
				SuggestionCollection operationSuggestions = result.GetSuggestions();
				SuggestionsComparer comparer = new SuggestionsComparer(word);
				operationSuggestions.Sort((x, y) => x.Distance - y.Distance);
				MergeSuggestions(suggestions, operationSuggestions, comparer);
				((IDisposable)result).Dispose();
			}
		}
		void MergeSuggestions(SuggestionCollection target, SuggestionCollection source, SuggestionsComparer comparer) {
			int targetIndex = 0;
			int sourceIndex = 0;
			while (targetIndex < target.Count && sourceIndex < source.Count) {
				if (target[targetIndex].Distance <= source[sourceIndex].Distance)
					targetIndex++;
				else {
					target.Insert(targetIndex, source[sourceIndex]);
					sourceIndex++;
					targetIndex++;
				}
			}
			if (sourceIndex < source.Count)
				target.AddRange(source.GetRange(sourceIndex));
		}
		protected virtual OperationBase CreateReplaceTypicalFaultOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new ReplaceTypicalFaultOperation(dictionary, timeOut);
		}
		protected virtual OperationBase CreateCapitalizeFirstLetterOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new CapitalizeFirstLetterOperation(dictionary);
		}
		protected virtual OperationBase CreateCapitalizeAllLettersOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new CapitalizeAllLettersOperation(dictionary);
		}
		protected virtual OperationBase CreateInterchangeTwoAdjacentLetters(ISpellCheckerDictionary dictionary, int timeOut) {
			return new InterchangeTwoAdjacentLetters(dictionary, timeOut);
		}
		protected virtual OperationBase CreateInterchangeTwoNotAdjacentLetters(ISpellCheckerDictionary dictionary, int timeOut) {
			return new InterchangeTwoNotAdjacentLetters(dictionary, timeOut);
		}
		protected virtual OperationBase CreateDeleteLetterOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new DeleteLetterOperation(dictionary, timeOut);
		}
		protected virtual OperationBase CreateInsertLetterOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new InsertLetterOperation(dictionary, timeOut);
		}
		protected virtual OperationBase CreateChangeOneLetterOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new ChangeOneLetterOperation(dictionary, timeOut);
		}
		protected virtual OperationBase CreateInsertSpaceOperation(ISpellCheckerDictionary dictionary, int timeOut) {
			return new InsertSpaceOperation(dictionary, timeOut);
		}
	}
	#region Operations
	#region OperationBase (abstract class)
	public abstract class OperationBase {
		readonly ISpellCheckerDictionary dictionary;
		protected OperationBase(ISpellCheckerDictionary dictionary) {
			this.dictionary = dictionary;
		}
		public ISpellCheckerDictionary Dictionary { get { return dictionary; } }
		protected CultureInfo Culture { get { return Dictionary.Culture; } }
		public abstract IOperationResult GenerateSuggestions(string input, int maxDistance);
		protected virtual SuggestionBase CreateSuggestion(string word, string candidat, int maxDistance) {
			if (String.IsNullOrEmpty(candidat))
				return null;
			int distance = LevenshteinAlgorithm.CalcDistance(word, candidat, Dictionary.Culture);
			if (distance > maxDistance)
				return null;
			if (IsWordCorrect(candidat))
				return new SuggestionBase(candidat, distance);
			return null;
		}
		protected virtual bool IsWordCorrect(string suggestion) {
			char[] separators = new char[] { ' ' };
			string[] words = suggestion.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			int count = words.Length;
			for (int i = 0; i < count; i++) {
				if (!Dictionary.Contains(words[i]))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region CapitalizeFirstLetterOperation
	public class CapitalizeFirstLetterOperation : OperationBase {
		public CapitalizeFirstLetterOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override IOperationResult GenerateSuggestions(string input, int maxDistance) {
			OperationResult result = new OperationResult();
			if (!Dictionary.CaseSensitive || !IsLowercase(input))
				return result;
			char[] chars = input.ToCharArray();
			chars[0] = Char.ToUpper(chars[0]);
			string candidate = new string(chars);
			SuggestionBase suggestion = CreateSuggestion(input, candidate, maxDistance);
			if (suggestion != null)
				result.AddSuggestion(suggestion);
			return result;
		}
		bool IsLowercase(string word) {
			string lowercase = word.ToLower(Dictionary.Culture);
			return String.Equals(word, lowercase);
		}
	}
	#endregion
	#region CapitalizeAllLettersOperation
	public class CapitalizeAllLettersOperation : OperationBase {
		public CapitalizeAllLettersOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override IOperationResult GenerateSuggestions(string input, int maxDistance) {
			OperationResult result = new OperationResult();
			if (!Dictionary.CaseSensitive || IsUpperrcase(input))
				return result;
			string candidate = input.ToUpper(Dictionary.Culture);
			SuggestionBase suggestion = CreateSuggestion(input, candidate, maxDistance);
			if (suggestion != null)
				result.AddSuggestion(suggestion);
			return result;
		}
		bool IsUpperrcase(string word) {
			string uppercase = word.ToUpper(Dictionary.Culture);
			return String.Equals(word, uppercase);
		}
	}
	#endregion
	#region AsyncOperationBase (abstract class)
	public abstract class AsyncOperationBase : OperationBase {
		const int chunkLength = 10;
		int timeOut;
		protected AsyncOperationBase(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary) {
			this.timeOut = timeOut;
		}
		public override IOperationResult GenerateSuggestions(string input, int maxDistance) {
			AsyncOperationResult result = new AsyncOperationResult(timeOut, Dictionary.Culture);
			List<string> candidates = GenerateCandidats(input);
			if (candidates.Count == 0)
				return result;
			List<WaitCallback> workItems = DivideIntoWorkItems(candidates, input, maxDistance, result);
			foreach (WaitCallback workItem in workItems)
				ThreadPool.QueueUserWorkItem(workItem);
			return result;
		}
		List<WaitCallback> DivideIntoWorkItems(List<string> candidates, string input, int maxDistance, AsyncOperationResult result) {
			List<WaitCallback> workItems = new List<WaitCallback>();
			int count = candidates.Count;
			for (int index = 0; index < count; index += chunkLength) {
				int startIndex = index;
				int endIndex = Math.Min((index + chunkLength - 1), count - 1);
				result.OnCalculationStart();
				WaitCallback callback = delegate(object state) {
					for (int i = startIndex; i <= endIndex; i++) {
						string candidate = candidates[i];
						SuggestionBase suggestion = CreateSuggestion(input, candidate, maxDistance);
						if (suggestion != null)
							result.AddSuggestion(suggestion);
					}
					result.OnCalculationEnd();
				};
				workItems.Add(callback);
			}
			return workItems;
		}
		public abstract List<string> GenerateCandidats(string input);
	}
	#endregion
	#region ReplaceTypicalFaultOperation
	public class ReplaceTypicalFaultOperation : AsyncOperationBase {
		public ReplaceTypicalFaultOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		public override List<string> GenerateCandidats(string input) {
			List<string> result = new List<string>();
			if (!(Dictionary is HunspellDictionary))
				return result;
			ConversionTable table = ((HunspellDictionary)Dictionary).ReplacementTable;
			int count = input.Length;
			int charIndex = 0;
			while (charIndex < count) {
				int index = table.Match(input.Substring(charIndex));
				if (index >= 0) {
					KeyValuePair<string, string> pattern = table[index];
					string candidate = input.Remove(charIndex, pattern.Key.Length).Insert(charIndex, pattern.Value);
					result.Add(candidate);
					charIndex += pattern.Key.Length;
				}
				else
					charIndex++;
			}
			return result;
		}
	}
	#endregion
	#region SimpleAsyncOperation (abstract class)
	public abstract class SimpleAsyncOperation : AsyncOperationBase {
		protected SimpleAsyncOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		public override List<string> GenerateCandidats(string input) {
			List<string> result = new List<string>();
			int count = input.Length;
			for (int i = 0; i <= count; i++) {
				string candidate = GenerateCandidate(input, i);
				if (!String.IsNullOrEmpty(candidate))
					result.Add(candidate);
			}
			return result;
		}
		protected abstract string GenerateCandidate(string input, int i);
	}
	#endregion
	#region InsertSpaceOperation
	public class InsertSpaceOperation : SimpleAsyncOperation {
		public InsertSpaceOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position == 0 || position == input.Length)
				return null;
			return input.Insert(position, " ");
		}
	}
	#endregion
	#region DeleteLetterOperation
	public class DeleteLetterOperation : SimpleAsyncOperation {
		public DeleteLetterOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position == input.Length)
				return null;
			return input.Remove(position, 1);
		}
	}
	#endregion
	#region InsertLetterOperation
	public class InsertLetterOperation : AsyncOperationBase {
		public InsertLetterOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		public char[] Alphabet { get { return Dictionary.AlphabetChars; } }
		public override List<string> GenerateCandidats(string input) {
			List<string> result = new List<string>();
			if (Alphabet == null || Alphabet.Length == 0)
				return result;
			int count = input.Length;
			for (int i = 0; i <= count; i++) {
				string[] suggestions = GenerateCandidats(input, i);
				if (suggestions != null)
					result.AddRange(suggestions);
			}
			return result;
		}
		protected virtual string[] GenerateCandidats(string input, int position) {
			List<string> result = new List<string>();
			int count = Alphabet.Length;
			for (int i = 0; i < count; i++) {
				char ch = Dictionary.CaseSensitive ? Alphabet[i] : Char.ToLower(Alphabet[i]);
				string candidate = input.Insert(position, ch.ToString());
				int index = result.BinarySearch(candidate);
				if (index < 0) {
					index = ~index;
					result.Insert(index, candidate);
				}
			}
			return result.ToArray();
		}
	}
	#endregion
	#region InterchangeTwoAdjacentLetters
	public class InterchangeTwoAdjacentLetters : SimpleAsyncOperation {
		public InterchangeTwoAdjacentLetters(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position >= input.Length - 1)
				return null;
			char[] inputChars = input.ToCharArray();
			char ch = inputChars[position];
			inputChars[position] = inputChars[position + 1];
			inputChars[position + 1] = ch;
			return new string(inputChars);
		}
	}
	#endregion
	#region InterchangeTwoAdjacentLetters
	public class InterchangeTwoNotAdjacentLetters : AsyncOperationBase {
		public InterchangeTwoNotAdjacentLetters(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		public override List<string> GenerateCandidats(string input) {
			List<string> result = new List<string>();
			char[] chars = input.ToCharArray();
			for (int i = 0; i < input.Length - 2; i++) {
				for (int j = i + 2; j < input.Length; j++) {
					char ch = chars[i];
					chars[i] = chars[j];
					chars[j] = ch;
					result.Add(new string(chars));
					ch = chars[j];
					chars[j] = chars[i];
					chars[i] = ch;
				}
			}
			return result;
		}
	}
	#endregion
	#region ChangeOneLetterOperation
	public class ChangeOneLetterOperation : InsertLetterOperation {
		public ChangeOneLetterOperation(ISpellCheckerDictionary dictionary, int timeOut)
			: base(dictionary, timeOut) {
		}
		protected override string[] GenerateCandidats(string input, int position) {
			if (position == input.Length)
				return null;
			bool isUpper = Char.IsUpper(input[position]);
			List<string> result = new List<string>();
			int count = Alphabet.Length;
			for (int i = 0; i < count; i++) {
				char ch = Alphabet[i];
				if (Dictionary.CaseSensitive) {
					if (Char.IsUpper(ch) != isUpper)
						continue;
				}
				else
					ch = Char.ToLowerInvariant(ch);
				char[] inputChars = input.ToCharArray();
				inputChars[position] = ch;
				string candidate = new string(inputChars);
				int index = result.BinarySearch(candidate);
				if (index < 0) {
					index = ~index;
					result.Insert(index, candidate);
				}
			}
			return result.ToArray();
		}
	}
	#endregion
	#endregion
#else
	public delegate OperationBase CreateOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary);
	public class NearMissStrategy {
		static OperationCollection availableOperations = CreateAvailableOperations();
		static OperationCollection CreateAvailableOperations() {
			OperationCollection result = new OperationCollection();
			result.Add(CreateCapitalizeFirstLetterOperation);
			result.Add(CreateCapitalizeAllLettersOperation);
			result.Add(CreateReplaceTypicalFaultOperation);
			result.Add(CreateInterchangeTwoAdjacentLetters);
			result.Add(CreateInterchangeTwoNotAdjacentLetters);
			result.Add(CreateReplaceBadCharacterKeyOperation);
			result.Add(CreateDeleteLetterOperation);
			result.Add(CreateInsertLetterOperation);
			result.Add(CreateChangeOneLetterOperation);
			result.Add(CreateInsertSpaceOperation);
			return result;
		}
		static OperationBase CreateReplaceTypicalFaultOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateReplaceTypicalFaultOperation(dictionary);
		}
		static OperationBase CreateReplaceBadCharacterKeyOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateReplaceBadCharacterKeyOperation(dictionary);
		}
		static OperationBase CreateCapitalizeFirstLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateCapitalizeFirstLetterOperation(dictionary);
		}
		static OperationBase CreateCapitalizeAllLettersOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateCapitalizeAllLettersOperation(dictionary);
		}
		static OperationBase CreateInterchangeTwoAdjacentLetters(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateInterchangeTwoAdjacentLetters(dictionary);
		}
		static OperationBase CreateInterchangeTwoNotAdjacentLetters(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateInterchangeTwoNotAdjacentLetters(dictionary);
		}
		static OperationBase CreateDeleteLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateDeleteLetterOperation(dictionary);
		}
		static OperationBase CreateInsertLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateInsertLetterOperation(dictionary);
		}
		static OperationBase CreateChangeOneLetterOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateChangeOneLetterOperation(dictionary);
		}
		static OperationBase CreateInsertSpaceOperation(NearMissStrategy strategy, ISpellCheckerDictionary dictionary) {
			return strategy.CreateInsertSpaceOperation(dictionary);
		}
		readonly ISpellCheckerDictionary dictionary;
		public NearMissStrategy(ISpellCheckerDictionary dictionary) {
			Guard.ArgumentNotNull(dictionary, "dictionary");
			this.dictionary = dictionary;
		}
		public virtual OperationCollection AvailableOperations { get { return availableOperations; } }
		public ISpellCheckerDictionary Dictionary { get { return dictionary; } }
		public virtual SuggestionCollection GetSuggestions(string word, int maxDistance, int timeOut) {
			if (!Dictionary.CaseSensitive)
				word = word.ToLower(Dictionary.Culture);
			SuggestionCollection suggestions = new SuggestionCollection();
			using (TaskManager<SuggestionBase> manager = new TaskManager<SuggestionBase>()) {
				int count = AvailableOperations.Count;
				for (int i = 0; i < count; i++) {
					CreateOperation createOperation = AvailableOperations[i];
					OperationBase operation = createOperation(this, Dictionary);
					operation.GenerateSuggestions(manager, word, maxDistance);
				}
				SuggestionBase[] result = manager.GetResults(timeOut);
				if (result.Length > 0)
					suggestions.AddRange(result);
			}
			return suggestions;
		}	
		protected virtual OperationBase CreateReplaceTypicalFaultOperation(ISpellCheckerDictionary dictionary) {
			return new ReplaceTypicalFaultOperation(dictionary);
		}
		protected virtual OperationBase CreateReplaceBadCharacterKeyOperation(ISpellCheckerDictionary dictionary) {
			return new ReplaceBadCharacterKeyOperation(dictionary);
		}
		protected virtual OperationBase CreateCapitalizeFirstLetterOperation(ISpellCheckerDictionary dictionary) {
			return new CapitalizeFirstLetterOperation(dictionary);
		}
		protected virtual OperationBase CreateCapitalizeAllLettersOperation(ISpellCheckerDictionary dictionary) {
			return new CapitalizeAllLettersOperation(dictionary);
		}
		protected virtual OperationBase CreateInterchangeTwoAdjacentLetters(ISpellCheckerDictionary dictionary) {
			return new InterchangeTwoAdjacentLetters(dictionary);
		}
		protected virtual OperationBase CreateInterchangeTwoNotAdjacentLetters(ISpellCheckerDictionary dictionary) {
			return new InterchangeTwoNotAdjacentLetters(dictionary);
		}
		protected virtual OperationBase CreateDeleteLetterOperation(ISpellCheckerDictionary dictionary) {
			return new DeleteLetterOperation(dictionary);
		}
		protected virtual OperationBase CreateInsertLetterOperation(ISpellCheckerDictionary dictionary) {
			return new InsertLetterOperation(dictionary);
		}
		protected virtual OperationBase CreateChangeOneLetterOperation(ISpellCheckerDictionary dictionary) {
			return new ChangeOneLetterOperation(dictionary);
		}
		protected virtual OperationBase CreateInsertSpaceOperation(ISpellCheckerDictionary dictionary) {
			return new InsertSpaceOperation(dictionary);
		}
	}
	#region TaskManager<TResult>
	public class TaskManager<TResult> : IDisposable where TResult : IComparable<TResult> {
		#region Fields
		long completedTasksCount;
		long tasksCount;
		readonly Queue<Action> tasks = new Queue<Action>();
		ManualResetEvent completeEvent = new ManualResetEvent(true);
		Dictionary<TResult, long> results = new Dictionary<TResult, long>();
		#endregion
		internal Queue<Action> Tasks { get { return tasks; } }
		protected internal long TasksCount { get { return tasksCount; } }
		protected internal long CompletedTasksCount { get { return completedTasksCount; } }
		public virtual void RegisterTask(Func<TResult> task) {
			lock (this.tasks) {
				long order = this.tasksCount;
				Action action = () => {
					OnTaskCompleted(task(), order);
				};
				this.tasks.Enqueue(action);
				this.tasksCount++;
			}
		}
		public virtual TResult[] GetResults(int timeOut) {
			lock (this.tasks) {
				if (this.tasks.Count > 0) {
					this.completeEvent.Reset();
					while (this.tasks.Count > 0)
						RunTask(this.tasks.Dequeue());
				}
			}
			this.completeEvent.WaitOne(timeOut);
			Interlocked.Exchange(ref this.tasksCount, 0);
			lock (this.results) {
				List<TResult> result = new List<TResult>(this.results.Keys);
				result.Sort((x, y) => {
					return (int)(this.results[x] - this.results[y]);
				});
				return result.ToArray();
			}
		}
		protected virtual void RunTask(Action task) {
			ThreadPool.QueueUserWorkItem((state) => task());
		}
		protected virtual void OnTaskCompleted(TResult result, long order) {
			if (Interlocked.Read(ref this.tasksCount) <= Interlocked.Read(ref this.completedTasksCount))
				return;
			if (result != null) {
				lock (this.results) {
					long prevOrder;
					if (this.results.TryGetValue(result, out prevOrder))
						order = Math.Min(prevOrder, order);
					this.results[result] = order;
				}
			}
			OnTaskCompleted();
		}
		void OnTaskCompleted() {
			if (Interlocked.Read(ref this.tasksCount) <= Interlocked.Increment(ref this.completedTasksCount))
				this.completeEvent.Set();
		}
		public virtual void Reset() {
			this.tasks.Clear();
			this.results.Clear();
			this.tasksCount = 0;
			this.completedTasksCount = 0;
		}
		public void Dispose() {
			if (this.completeEvent != null) {
				this.completeEvent.Dispose();
				this.completeEvent = null;
			}
		}
	}
	#endregion
	#region Operations
	#region OperationBase (abstract class)
	public abstract class OperationBase {
		readonly ISpellCheckerDictionary dictionary;
		protected OperationBase(ISpellCheckerDictionary dictionary) {
			this.dictionary = dictionary;
		}
		public ISpellCheckerDictionary Dictionary { get { return dictionary; } }
		protected CultureInfo Culture { get { return Dictionary.Culture; } }
		public abstract void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance);
		protected virtual void ProcessCandidate(TaskManager<SuggestionBase> manager, string word, string candidate, int maxDistance) {
			manager.RegisterTask(() => CalculateSuggestion(word, candidate, maxDistance));
		}
		protected virtual SuggestionBase CalculateSuggestion(string word, string candidat, int maxDistance) {
			if (String.IsNullOrEmpty(candidat))
				return null;
			int distance = LevenshteinAlgorithm.CalcDistance(word, candidat, Dictionary.Culture);
			if (distance > maxDistance)
				return null;
			if (IsWordCorrect(candidat))
				return new SuggestionBase(candidat, distance);
			return null;
		}
		protected virtual bool IsWordCorrect(string suggestion) {
			char[] separators = new char[] { ' ' };
			string[] words = suggestion.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			int count = words.Length;
			for (int i = 0; i < count; i++) {
				if (!Dictionary.Contains(words[i]))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region CapitalizeFirstLetterOperation
	public class CapitalizeFirstLetterOperation : OperationBase {
		public CapitalizeFirstLetterOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			if (!Dictionary.CaseSensitive || !IsLowercase(input))
				return;
			char[] chars = input.ToCharArray();
			chars[0] = Char.ToUpper(chars[0]);
			string candidate = new string(chars);
			ProcessCandidate(manager, input, candidate, maxDistance);
		}
		bool IsLowercase(string word) {
			string lowercase = word.ToLower(Dictionary.Culture);
			return String.Equals(word, lowercase);
		}
	}
	#endregion
	#region CapitalizeAllLettersOperation
	public class CapitalizeAllLettersOperation : OperationBase {
		public CapitalizeAllLettersOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			if (!Dictionary.CaseSensitive || IsUpperrcase(input))
				return;
			string candidate = input.ToUpper(Dictionary.Culture);
			ProcessCandidate(manager, input, candidate, maxDistance);
		}
		bool IsUpperrcase(string word) {
			string uppercase = word.ToUpper(Dictionary.Culture);
			return String.Equals(word, uppercase);
		}
	}
	#endregion
	#region ReplaceTypicalFaultOperation
	public class ReplaceTypicalFaultOperation : OperationBase {
		public ReplaceTypicalFaultOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			if (!(Dictionary is HunspellDictionary))
				return;
			ConversionTable table = ((HunspellDictionary)Dictionary).ReplacementTable;
			int count = input.Length;
			int charIndex = 0;
			while (charIndex < count) {
				int index = table.Match(input.Substring(charIndex));
				if (index >= 0) {
					KeyValuePair<string, string> pattern = table[index];
					string candidate = input.Remove(charIndex, pattern.Key.Length).Insert(charIndex, pattern.Value);
					ProcessCandidate(manager, input, candidate, maxDistance);
					charIndex += pattern.Key.Length;
				}
				else
					charIndex++;
			}
		}
	}
	#endregion
	public class ReplaceBadCharacterKeyOperation : OperationBase {
		public ReplaceBadCharacterKeyOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			HunspellDictionary hunspellDictionary = Dictionary as HunspellDictionary;
			string keys = hunspellDictionary != null ? hunspellDictionary.Keys : HunspellDictionary.DefaultKeys;
			int count = input.Length;
			for (int i = 0; i < count; i++) {
				int startIndex = 0;
				while (startIndex < keys.Length) {
					int charIndex = keys.IndexOf(input[i], startIndex);
					if (charIndex < 0)
						break;
					if (charIndex > 0 && keys[charIndex - 1] != '|') {
						string candidate = ReplaceChar(input, i, keys[charIndex - 1]);
						ProcessCandidate(manager, input, candidate, maxDistance);
					}
					if (charIndex < keys.Length - 1 && keys[charIndex + 1] != '|') {
						string candidate = ReplaceChar(input, i, keys[charIndex + 1]);
						ProcessCandidate(manager, input, candidate, maxDistance);
					}
					startIndex = charIndex + 1;
				}
			}
		}
		static string ReplaceChar(string str, int index, char newChar) {
			char[] chars = str.ToCharArray();
			chars[index] = newChar;
			return new string(chars);
		}
	}
	#region SimpleOperation (abstract class)
	public abstract class SimpleOperation : OperationBase {
		protected SimpleOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			int count = input.Length;
			for (int i = 0; i <= count; i++) {
				string candidate = GenerateCandidate(input, i);
				ProcessCandidate(manager, input, candidate, maxDistance);
			}
		}
		protected abstract string GenerateCandidate(string input, int i);
	}
	#endregion
	#region InsertSpaceOperation
	public class InsertSpaceOperation : SimpleOperation {
		public InsertSpaceOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position == 0 || position == input.Length)
				return null;
			return input.Insert(position, " ");
		}
	}
	#endregion
	#region DeleteLetterOperation
	public class DeleteLetterOperation : SimpleOperation {
		public DeleteLetterOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position == input.Length)
				return null;
			return input.Remove(position, 1);
		}
	}
	#endregion
	#region InsertLetterOperation
	public class InsertLetterOperation : OperationBase {
		public InsertLetterOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public char[] Alphabet { get { return Dictionary.AlphabetChars; } }
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			if (Alphabet == null || Alphabet.Length == 0)
				return;
			int count = input.Length;
			for (int i = 0; i <= count; i++)
				GenerateCandidats(manager, input, i, maxDistance);
		}
		protected virtual void GenerateCandidats(TaskManager<SuggestionBase> manager, string input, int position, int maxDistance) {
			int count = Alphabet.Length;
			for (int i = 0; i < count; i++) {
				char ch = Dictionary.CaseSensitive ? Alphabet[i] : Char.ToLower(Alphabet[i]);
				string candidate = input.Insert(position, ch.ToString());
				ProcessCandidate(manager, input, candidate, maxDistance);
			}
		}
	}
	#endregion
	#region InterchangeTwoAdjacentLetters
	public class InterchangeTwoAdjacentLetters : SimpleOperation {
		public InterchangeTwoAdjacentLetters(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		protected override string GenerateCandidate(string input, int position) {
			if (position >= input.Length - 1)
				return null;
			char[] inputChars = input.ToCharArray();
			char ch = inputChars[position];
			inputChars[position] = inputChars[position + 1];
			inputChars[position + 1] = ch;
			return new string(inputChars);
		}
	}
	#endregion
	#region InterchangeTwoAdjacentLetters
	public class InterchangeTwoNotAdjacentLetters : OperationBase {
		public InterchangeTwoNotAdjacentLetters(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		public override void GenerateSuggestions(TaskManager<SuggestionBase> manager, string input, int maxDistance) {
			char[] chars = input.ToCharArray();
			for (int i = 0; i < input.Length - 2; i++) {
				for (int j = i + 2; j < input.Length; j++) {
					char ch = chars[i];
					chars[i] = chars[j];
					chars[j] = ch;
					ProcessCandidate(manager, input, new string(chars), maxDistance);
					ch = chars[j];
					chars[j] = chars[i];
					chars[i] = ch;
				}
			}
		}
	}
	#endregion
	#region ChangeOneLetterOperation
	public class ChangeOneLetterOperation : InsertLetterOperation {
		public ChangeOneLetterOperation(ISpellCheckerDictionary dictionary)
			: base(dictionary) {
		}
		protected override void GenerateCandidats(TaskManager<SuggestionBase> manager, string input, int position, int maxDistance) {
			if (position == input.Length)
				return;
			bool isUpper = Char.IsUpper(input[position]);
			List<string> result = new List<string>();
			int count = Alphabet.Length;
			for (int i = 0; i < count; i++) {
				char ch = Alphabet[i];
				if (Dictionary.CaseSensitive) {
					if (Char.IsUpper(ch) != isUpper)
						continue;
				}
				else
					ch = Char.ToLowerInvariant(ch);
				char[] inputChars = input.ToCharArray();
				inputChars[position] = ch;
				string candidate = new string(inputChars);
				ProcessCandidate(manager, input, candidate, maxDistance);
			}
		}
	}
	#endregion
	#endregion
#endif
	public static class LevenshteinAlgorithm {
		static int DeleteCost(string x, int xi) {
			return 1;
		}
		static int InsertCost(string x, int xi) {
			return 1;
		}
		static int SubstitutionCost(string x, int pos1, string y, int pos2, CultureInfo culture) {
			int result = char.ToLower(x[pos1], culture) == char.ToLower(y[pos2], culture) ? 0 : 1;
			return result;
		}
		public static int Min(int x1, int x2, int x3) {
			return Math.Min(Math.Min(x1, x2), x3);
		}
		public static int CalcDistance(string x, string y, CultureInfo culture) {
			int result = CalcAlignmentMatrix(x, y, culture)[x.Length, y.Length];
			return result;
		}
		public static int[,] CalcAlignmentMatrix(string x, string y, CultureInfo culture) {
			int m = x.Length;
			int n = y.Length;
			int[,] result = new int[m + 1, n + 1];
			result[0, 0] = 0;
			for (int j = 0; j < n; j++)
				result[0, j + 1] = result[0, j] + InsertCost(y, j);
			for (int i = 0; i < m; i++) {
				result[i + 1, 0] = result[i, 0] + DeleteCost(x, i);
				for (int j = 0; j < n; j++) {
					result[i + 1, j + 1] = Min(
						result[i, j] + SubstitutionCost(x, i, y, j, culture),
						result[i, j + 1] + DeleteCost(x, i),
						result[i + 1, j] + InsertCost(y, j));
				}
			}
			return result;
		}
	}
	[Serializable]
	class MetaphoneComparer : IEqualityComparer<Metaphone> {
		public bool Equals(Metaphone x, Metaphone y) {
			return x.Equals(y);
		}
		public int GetHashCode(Metaphone obj) {
			return obj.GetHashCode();
		}
	}
	[Serializable]
	public struct Metaphone : IEquatable<Metaphone> {
		char c1;
		char c2;
		char c3;
		char c4;
		public int Length {
			get {
				if (c1 == '\0')
					return 0;
				if (c2 == '\0')
					return 1;
				if (c3 == '\0')
					return 2;
				if (c4 == '\0')
					return 3;
				return 4;
			}
		}
		public override int GetHashCode() {
			return ((int)c1) | ((int)c2 << 8) | ((int)c3 << 16) | ((int)c4 << 24);
		}
		public bool Equals(Metaphone other) {
			return c1 == other.c1 && c2 == other.c2 && c3 == other.c3 && c4 == other.c4;
		}
		void Append(char main) {
			if (c1 == '\0') {
				c1 = main;
			}
			else
				if (c2 == '\0')
					c2 = main;
				else
					if (c3 == '\0')
						c3 = main;
					else
						c4 = main;
		}
		public void Append(string main) {
			int len = Math.Min(main.Length, 4 - Length);
			for (int i = 0; i < len; i++)
				Append(main[i]);
		}
	}
	public class MetaphonePair {
		Metaphone first;
		Metaphone second;
		public MetaphonePair(Metaphone first, Metaphone second) {
			this.first = first;
			this.second = second;
		}
		public Metaphone First { get { return first; } }
		public Metaphone Second { get { return second; } }
	}
	public class DoubleMetaphoneAlgorithm {
		const char separator = ';';
		char[] input = new char[20];
		int inputLen;
		bool alternate;
		Metaphone primaryBuilder;
		Metaphone secondaryBuilder;
		public char[] Input {
			get { return input; }
		}
		protected virtual void Reset() {
			primaryBuilder = new Metaphone();
			secondaryBuilder = new Metaphone();
			alternate = false;
		}
		public int Length {
			get { return inputLen; }
		}
		public int Last {
			get { return Length - 1; }
		}
		protected int Find(string test) {
			return new string(Input, 0, Length).IndexOf(test);
		}
		protected int Find(char test) {
			for (int i = 0; i < Length; i++)
				if (test == Input[i])
					return i;
			return -1;
		}
		bool SlavoGermanic() {
			if ((Find('W') > -1) || (Find('K') > -1) || (Find("CZ") > -1) || (Find("WITZ") > -1))
				return true;
			return false;
		}
		protected virtual void MetaphAdd(string main) {
			if (main != null) {
				primaryBuilder.Append(main);
				secondaryBuilder.Append(main);
			}
		}
		protected virtual void MetaphAdd(string main, string alt) {
			if (main != null)
				primaryBuilder.Append(main);
			if (alt != null) {
				alternate = true;
				if (alt[0] != ' ')
					secondaryBuilder.Append(alt);
			}
			else
				if (main != null && (main[0] != ' '))
					secondaryBuilder.Append(main);
		}
		bool IsVowel(int at) {
			if ((at < 0) || (at >= Length))
				return false;
			char it = Input[at];
			if ((it == 'A') || (it == 'E') || (it == 'I') || (it == 'O') || (it == 'U') || (it == 'Y'))
				return true;
			return false;
		}
		protected virtual bool StringAt(int start, string strings) {
			if (start >= Length && strings[strings.Length - 1] == separator)
				return true;
			if (start < 0)
				start = 0;
			bool result = true;
			int index = start;
			for (int i = 0; i < strings.Length; i++) {
				char c = strings[i];
				if (!result) {
					if (c == separator) {
						result = true;
						index = start;
					}
				}
				else {
					if (c == separator)
						return true;
					if (index >= Length || c != Input[index])
						result = false;
					index++;
				}
			}
			return false;
		}
		protected virtual Char GetAt(int at) {
			if (at < 0 || at >= Length) return char.MinValue;
			return Input[at];
		}
		public MetaphonePair DoubleMetaphone(string inputString, CultureInfo culture) {
			Reset();
			if (input.Length < inputString.Length)
				input = new char[inputString.Length];
			inputLen = inputString.Length;
			for (int i = 0; i < inputString.Length; i++)
				Input[i] = char.ToUpper(inputString[i], culture);
			int current = 0;
			if (StringAt(0, "GN;KN;PN;WR;PS;"))
				current++;
			if (GetAt(0) == 'X') {
				MetaphAdd("S"); 
				current += 1;
			}
			while ((primaryBuilder.Length < 4) || (secondaryBuilder.Length < 4)) {
				if (current >= Length)
					break;
				switch (GetAt(current)) {
					case 'A':
					case 'E':
					case 'I':
					case 'O':
					case 'U':
					case 'Y':
						current = ProcessA(current);
						break;
					case 'B':
						current = ProcessB(current);
						break;
					case 'C':
						current = ProcessC(current);
						break;
					case 'D':
						current = ProcessD(current);
						break;
					case 'F':
						current = ProcessF(current);
						break;
					case 'G':
						current = ProcessG(current);
						break;
					case 'H':
						current = ProcessH(current);
						break;
					case 'J':
						current = ProcessJ(current);
						break;
					case 'K':
						current = ProcessK(current);
						break;
					case 'L':
						current = ProcessL(current);
						break;
					case 'M':
						current = ProcessM(current);
						break;
					case 'N':
						current = ProcessN(current);
						break;
					case 'P':
						current = ProcessP(current);
						break;
					case 'Q':
						current = ProcessQ(current);
						break;
					case 'R':
						current = ProcessR(current);
						break;
					case 'S':
						current = ProcessS(current);
						break;
					case 'T':
						current = ProcessT(current);
						break;
					case 'V':
						current = ProcessV(current);
						break;
					case 'W':
						current = ProcessW(current);
						break;
					case 'X':
						current = ProcessX(current);
						break;
					case 'Z':
						current = ProcessZ(current);
						break;
					default:
						current++;
						break;
				}
			}
			return new MetaphonePair(primaryBuilder, alternate ? secondaryBuilder : new Metaphone());
		}
		#region ProcessLetters
		protected virtual int ProcessA(int current) {
			if (current == 0)
				MetaphAdd("A");
			return current + 1;
		}
		protected virtual int ProcessB(int current) {
			MetaphAdd("P");
			if (GetAt(current + 1) == 'B')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessC(int current) {
			if ((current > 1) && !IsVowel(current - 2) && StringAt((current - 1), "ACH;")
				&& ((GetAt(current + 2) != 'I') && ((GetAt(current + 2) != 'E')
				|| StringAt((current - 2), "BACHER;MACHER;")))) {
				MetaphAdd("K");
				return current + 2;
			}
			if ((current == 0) && StringAt(current, "CAESAR;")) {
				MetaphAdd("S");
				return current + 2;
			}
			if (StringAt(current, "CHIA;")) {
				MetaphAdd("K");
				return current + 2;
			}
			if (StringAt(current, "CH;")) {
				if ((current > 0) && StringAt(current, "CHAE;")) {
					MetaphAdd("K", "X");
					return current + 2;
				}
				if ((current == 0)
					&& (StringAt((current + 1), "HARAC;HARIS;")
					|| StringAt((current + 1), "HOR;HYM;HIA;HEM;"))
					&& !StringAt(0, "CHORE;")) {
					MetaphAdd("K");
					return current + 2;
				}
				if ((StringAt(0, "VAN ;VON ;") || StringAt(0, "SCH;"))
					|| StringAt((current - 2), "ORCHES;ARCHIT;ORCHID;")
					|| StringAt((current + 2), "T;S;")
					|| ((StringAt((current - 1), "A;O;U;E;") || (current == 0))
					&& StringAt((current + 2), "L;R;N;M;B;H;F;V;W; ;"))) {
					MetaphAdd("K");
				}
				else {
					if (current > 0) {
						if (StringAt(0, "MC;"))
							MetaphAdd("K");
						else
							MetaphAdd("X", "K");
					}
					else
						MetaphAdd("X");
				}
				return current + 2;
			}
			if (StringAt(current, "CZ;") && !StringAt((current - 2), "WICZ;")) {
				MetaphAdd("S", "X");
				return current + 2;
			}
			if (StringAt((current + 1), "CIA;")) {
				MetaphAdd("X");
				return current + 3;
			}
			if (StringAt(current, "CC;") && !((current == 1) && (GetAt(0) == 'M')))
				if (StringAt((current + 2), "I;E;H;") && !StringAt((current + 2), "HU;")) {
					if (((current == 1) && (GetAt(current - 1) == 'A')) || StringAt((current - 1), "UCCEE;UCCES;"))
						MetaphAdd("KS");
					else
						MetaphAdd("X");
					return current + 3;
				}
				else {
					MetaphAdd("K");
					return current + 2;
				}
			if (StringAt(current, "CK;CG;CQ;")) {
				MetaphAdd("K");
				return current + 2;
			}
			if (StringAt(current, "CI;CE;CY;")) {
				if (StringAt(current, "CIO;CIE;CIA;"))
					MetaphAdd("S", "X");
				else
					MetaphAdd("S");
				return current + 2;
			}
			MetaphAdd("K");
			if (StringAt((current + 1), " C; Q; G;")) {
				return current + 3;
			}
			else
				if (StringAt((current + 1), "C;K;Q;")
				&& !StringAt((current + 1), "CE;CI;"))
					return current + 2;
				else
					return current + 1;
		}
		protected virtual int ProcessD(int current) {
			if (StringAt(current, "DG;"))
				if (StringAt((current + 2), "I;E;Y;")) {
					MetaphAdd("J");
					return current + 3;
				}
				else {
					MetaphAdd("TK");
					return current + 2;
				}
			if (StringAt(current, "DT;DD;")) {
				MetaphAdd("T");
				return current + 2;
			}
			MetaphAdd("T");
			return current + 1;
		}
		protected virtual int ProcessF(int current) {
			MetaphAdd("F");
			if (GetAt(current + 1) == 'F')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessG(int current) {
			if (GetAt(current + 1) == 'H') {
				if ((current > 0) && !IsVowel(current - 1)) {
					MetaphAdd("K");
					return current + 2;
				}
				if (current < 3) {
					if (current == 0) {
						if (GetAt(current + 2) == 'I')
							MetaphAdd("J");
						else
							MetaphAdd("K");
						return current + 2;
					}
				}
				if (((current > 1) && StringAt((current - 2), "B;H;D;"))
					|| ((current > 2) && StringAt((current - 3), "B;H;D;"))
					|| ((current > 3) && StringAt((current - 4), "B;H;"))) {
					return current + 2;
				}
				else {
					if ((current > 2) && (GetAt(current - 1) == 'U') && StringAt((current - 3), "C;G;L;R;T;")) {
						MetaphAdd("F");
					}
					else
						if ((current > 0) && GetAt(current - 1) != 'I')
							MetaphAdd("K");
					return current + 2;
				}
			}
			if (GetAt(current + 1) == 'N') {
				if ((current == 1) && IsVowel(0) && !SlavoGermanic()) {
					MetaphAdd("KN", "N");
				}
				else
					if (!StringAt((current + 2), "EY;") && (GetAt(current + 1) != 'Y') && !SlavoGermanic()) {
						MetaphAdd("N", "KN");
					}
					else
						MetaphAdd("KN");
				return current + 2;
			}
			if (StringAt((current + 1), "LI;") && !SlavoGermanic()) {
				MetaphAdd("KL", "L");
				return current + 2;
			}
			if ((current == 0) && ((GetAt(current + 1) == 'Y')
				|| StringAt((current + 1), "ES;EP;EB;EL;EY;IB;IL;IN;IE;EI;ER;"))) {
				MetaphAdd("K", "J");
				return current + 2;
			}
			if ((StringAt((current + 1), "ER;") || (GetAt(current + 1) == 'Y'))
				&& !StringAt(0, "DANGER;RANGER;MANGER;")
				&& !StringAt((current - 1), "E;I;")
				&& !StringAt((current - 1), "RGY;OGY;")) {
				MetaphAdd("K", "J");
				return current + 2;
			}
			if (StringAt((current + 1), "E;I;Y;") || StringAt((current - 1), "AGGI;OGGI;")) {
				if ((StringAt(0, "VAN ;VON ;") || StringAt(0, "SCH;"))
					|| StringAt((current + 1), "ET;"))
					MetaphAdd("K");
				else
					if (StringAt((current + 1), "IER ;"))
						MetaphAdd("J");
					else
						MetaphAdd("J", "K");
				return current + 2;
			}
			MetaphAdd("K");
			if (GetAt(current + 1) == 'G')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessH(int current) {
			if (((current == 0) || IsVowel(current - 1)) && IsVowel(current + 1)) {
				MetaphAdd("H");
				return current + 2;
			}
			else
				return current + 1;
		}
		protected virtual int ProcessJ(int current) {
			if (StringAt(current, "JOSE;") || StringAt(0, "SAN ;")) {
				if (((current == 0) && (GetAt(current + 4) == ' ')) || StringAt(0, "SAN ;"))
					MetaphAdd("H");
				else {
					MetaphAdd("J", "H");
				}
				return current + 1;
			}
			if ((current == 0) && !StringAt(current, "JOSE;"))
				MetaphAdd("J", "A");
			else
				if (IsVowel(current - 1) && !SlavoGermanic()
				&& ((GetAt(current + 1) == 'A') || (GetAt(current + 1) == 'O')))
					MetaphAdd("J", "H");
				else
					if (current == Last)
						MetaphAdd("J", " ");
					else
						if (!StringAt((current + 1), "L;T;K;S;N;M;B;Z;")
						&& !StringAt((current - 1), "S;K;L;"))
							MetaphAdd("J");
			if (GetAt(current + 1) == 'J')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessK(int current) {
			MetaphAdd("K");
			if (GetAt(current + 1) == 'K')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessL(int current) {
			if (GetAt(current + 1) == 'L') {
				if (((current == (Length - 3)) && StringAt((current - 1), "ILLO;ILLA;ALLE;"))
					|| ((StringAt((Last - 1), "AS;OS;") || StringAt(Last, "A;O;"))
					&& StringAt((current - 1), "ALLE;"))) {
					MetaphAdd("L", " ");
					return current + 2;
				}
				MetaphAdd("L");
				return current + 2;
			}
			else {
				MetaphAdd("L");
				return current + 1;
			}
		}
		protected virtual int ProcessM(int current) {
			MetaphAdd("M");
			if ((StringAt((current - 1), "UMB;")
				&& (((current + 1) == Last) || StringAt((current + 2), "ER;")))
			|| (GetAt(current + 1) == 'M'))
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessN(int current) {
			MetaphAdd("N");
			if (GetAt(current + 1) == 'N')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessCCCCCC(int current) {
			MetaphAdd("N");
			return current + 1;
		}
		protected virtual int ProcessP(int current) {
			if (GetAt(current + 1) == 'H') {
				MetaphAdd("F");
				return current + 2;
			}
			MetaphAdd("P");
			if (StringAt((current + 1), "P;B;"))
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessQ(int current) {
			MetaphAdd("K");
			if (GetAt(current + 1) == 'Q')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessR(int current) {
			if ((current == Last) && !SlavoGermanic() && StringAt((current - 2), "IE;")
				&& !StringAt((current - 4), "ME;MA;"))
				MetaphAdd("", "R");
			else
				MetaphAdd("R");
			if (GetAt(current + 1) == 'R')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessS(int current) {
			if (StringAt((current - 1), "ISL;YSL;")) {
				return current + 1;
			}
			if ((current == 0) && StringAt(current, "SUGAR;")) {
				MetaphAdd("X", "S");
				return current + 1;
			}
			if (StringAt(current, "SH;")) {
				if (StringAt((current + 1), "HEIM;HOEK;HOLM;HOLZ;"))
					MetaphAdd("S");
				else
					MetaphAdd("X");
				return current + 2;
			}
			if (StringAt(current, "SIO;SIA;") || StringAt(current, "SIAN;")) {
				if (!SlavoGermanic())
					MetaphAdd("S", "X");
				else
					MetaphAdd("S");
				return current + 3;
			}
			if (((current == 0) && StringAt((current + 1), "M;N;L;W;"))
				|| StringAt((current + 1), "Z;")) {
				MetaphAdd("S", "X");
				if (StringAt((current + 1), "Z;")) {
					return current + 2;
				}
				else {
					return current + 1;
				}
			}
			if (StringAt(current, "SC;")) {
				if (GetAt(current + 2) == 'H')
					if (StringAt((current + 3), "OO;ER;EN;UY;ED;EM;")) {
						if (StringAt((current + 3), "ER;EN;")) {
							MetaphAdd("X", "SK");
						}
						else
							MetaphAdd("SK");
						return current + 3;
					}
					else {
						if ((current == 0) && !IsVowel(3) && (GetAt(3) != 'W'))
							MetaphAdd("X", "S");
						else
							MetaphAdd("X");
						return current + 3;
					}
				if (StringAt((current + 2), "I;E;Y;")) {
					MetaphAdd("S");
					return current + 3;
				}
				MetaphAdd("SK");
				return current + 3;
			}
			if ((current == Last) && StringAt((current - 2), "AI;OI;"))
				MetaphAdd("", "S");
			else
				MetaphAdd("S");
			if (StringAt((current + 1), "S;Z;"))
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessT(int current) {
			if (StringAt(current, "TION;")) {
				MetaphAdd("X");
				return current + 3;
			}
			if (StringAt(current, "TIA;TCH;")) {
				MetaphAdd("X");
				return current + 3;
			}
			if (StringAt(current, "TH;") || StringAt(current, "TTH;")) {
				if (StringAt((current + 2), "OM;AM;")
					|| StringAt(0, "VAN ;VON ;")
					|| StringAt(0, "SCH;")) {
					MetaphAdd("T");
				}
				else {
					MetaphAdd("0", "T");
				}
				return current + 2;
			}
			MetaphAdd("T");
			if (StringAt((current + 1), "T;D;"))
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessV(int current) {
			MetaphAdd("F");
			if (GetAt(current + 1) == 'V')
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessW(int current) {
			if (StringAt(current, "WR;")) {
				MetaphAdd("R");
				return current + 2;
			}
			if ((current == 0) && (IsVowel(current + 1) || StringAt(current, "WH;"))) {
				if (IsVowel(current + 1))
					MetaphAdd("A", "F");
				else
					MetaphAdd("A");
			}
			if (((current == Last) && IsVowel(current - 1)) || StringAt((current - 1), "EWSKI;EWSKY;OWSKI;OWSKY;")
				|| StringAt(0, "SCH;")) {
				MetaphAdd("", "F");
				return current + 1;
			}
			if (StringAt(current, "WICZ;WITZ;")) {
				MetaphAdd("TS", "FX");
				return current + 4;
			}
			return current + 1;
		}
		protected virtual int ProcessX(int current) {
			if (!((current == Last) && (StringAt((current - 3), "IAU;EAU;")
				|| StringAt((current - 2), "AU;OU;"))))
				MetaphAdd("KS");
			if (StringAt((current + 1), "C;X;"))
				return current + 2;
			else
				return current + 1;
		}
		protected virtual int ProcessZ(int current) {
			if (GetAt(current + 1) == 'H') {
				MetaphAdd("J");
				return current + 2;
			}
			else
				if (StringAt((current + 1), "ZO;ZI;ZA;")
					|| (SlavoGermanic() && ((current > 0) && GetAt(current - 1) != 'T'))) {
					MetaphAdd("S", "TS");
				}
				else
					MetaphAdd("S");
			if (GetAt(current + 1) == 'Z')
				return current + 2;
			else
				return current + 1;
		}
		#endregion
	}
#if SL
	public static class SortAlgorithms {
		class FunctorComparer<T> : IComparer<T> {
			readonly Comparison<T> comparison;
			public FunctorComparer(Comparison<T> comparison) {
				this.comparison = comparison;
			}
			public int Compare(T x, T y) {
				return comparison(x, y);
			}
		}
		public static void QuickSort<T>(List<T> items, int left, int right, IComparer<T> comparer) {
			Guard.ArgumentNotNull(items, "items");
			Guard.ArgumentNotNull(comparer, "comparer");
			Guard.ArgumentNonNegative(left, "left");
			Guard.ArgumentNonNegative(right, "right");
			if (right > left)
				QuickSortInternal<T>(items, ref left, ref right, comparer);
		}
		static void QuickSortInternal<T>(List<T> items, ref int left, ref int right, IComparer<T> comparer) {
			do {
				int a = left;
				int b = right;
				int num3 = a + ((b - a) >> 1);
				SwapIfGreaterWithItems(items, comparer, a, num3);
				SwapIfGreaterWithItems(items, comparer, a, b);
				SwapIfGreaterWithItems(items, comparer, num3, b);
				T y = items[num3];
				do {
					while (comparer.Compare(items[a], y) < 0) {
						a++;
					}
					while (comparer.Compare(y, items[b]) < 0) {
						b--;
					}
					if (a > b) {
						break;
					}
					if (a < b) {
						T local2 = items[a];
						items[a] = items[b];
						items[b] = local2;
					}
					a++;
					b--;
				}
				while (a <= b);
				if ((b - left) <= (right - a)) {
					if (left < b) {
						QuickSort(items, left, b, comparer);
					}
					left = a;
				}
				else {
					if (a < right) {
						QuickSort(items, a, right, comparer);
					}
					right = b;
				}
			}
			while (left < right);
		}
		static void SwapIfGreaterWithItems<T>(List<T> items, IComparer<T> comparer, int a, int b) {
			if ((a != b) && (comparer.Compare(items[a], items[b]) > 0)) {
				T local = items[a];
				items[a] = items[b];
				items[b] = local;
			}
		}
		public static void QuickSort<T>(List<T> items, IComparer<T> comparer) {
			Guard.ArgumentNotNull(items, "items");
			if (items.Count > 0)
				QuickSort<T>(items, 0, items.Count - 1, comparer);
		}
		public static void QuickSort<T>(List<T> items, int left, int right, Comparison<T> comparison) {
			Guard.ArgumentNotNull(comparison, "comparison");
			IComparer<T> comparer = new FunctorComparer<T>(comparison);
			QuickSort<T>(items, comparer);
		}
		public static void QuickSort<T>(List<T> items, Comparison<T> comparison) {
			QuickSort<T>(items, 0, items.Count - 1, comparison);
		}
	}
#endif
	public static class IndistinctComparer {
		struct StringComparerInfo {
			long subRows;
			long countLike;
			public long SubRows { get { return subRows; } set { subRows = value; } }
			public long CountLike { get { return countLike; } set { countLike = value; } }
		}
		public static float Compare(string source, string target) {
			return Compare(source, target, 2);
		}
		public static float Compare(string source, string target, int maxComparedGroupLength) {
			StringComparerInfo result = new StringComparerInfo();
			if (maxComparedGroupLength == 0 || String.IsNullOrEmpty(target) || String.IsNullOrEmpty(source))
				return 0;
			for (int groupLength = 1; groupLength <= maxComparedGroupLength; groupLength++) {
				StringComparerInfo info = Match(target, source, groupLength);
				result.CountLike += info.CountLike;
				result.SubRows += info.SubRows;
				info = Match(source, target, groupLength);
				result.CountLike += info.CountLike;
				result.SubRows += info.SubRows;
			}
			if (result.SubRows == 0)
				return 0;
			return (float)result.CountLike / (float)result.SubRows;
		}
		static StringComparerInfo Match(string first, string second, int length) {
			StringComparerInfo result = new StringComparerInfo();
			for (int i = 0; i <= first.Length - length; i++) {
				string firstSubstring = first.Substring(i, length);
				for (int j = 0; j <= second.Length - length; j++) {
					string secondSubstring = second.Substring(j, length);
					if (String.Equals(firstSubstring, secondSubstring)) {
						result.CountLike++;
						break;
					}
				}
				result.SubRows++;
			}
			return result;
		}
	}
}
