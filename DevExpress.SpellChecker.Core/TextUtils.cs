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
using System.Collections;
using System.Globalization;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Algorithms;
using DevExpress.XtraSpellChecker.Strategies;
using System.Collections.Generic;
#if !WPF
using System.Drawing;
#endif
#if SL
using DevExpress.Data;
using DevExpress.XtraSpellChecker.Native;
#else
using DevExpress.XtraSpellChecker.Native;
#endif
#if WPF
using System.Windows;
#endif
namespace DevExpress.XtraSpellChecker.Parser {
	public class UndefinedPosition : Position {
		public UndefinedPosition() : base() { }
		protected override Position InternalAdd(Position position) {
			throw new Exception("Cannot Add to Undefined");
		}
		protected override Position InternalSubtract(Position position) {
			throw new Exception("Cannot Subtract from Undefined");
		}
		protected override int InternalCompare(Position position) {
			throw new Exception("Cannot Compare the Undefined");
		}
		protected override Position MoveForward() {
			throw new Exception("Cannot MoveForward the Undefined");
		}
		protected override Position MoveBackward() {
			throw new Exception("Cannot MoveBackward the Undefined");
		}
		protected override object ActualPosition {
			get {
				throw new Exception("Cannot get the ActualPosition of Undefined");
			}
			set {
				throw new Exception("Cannot set the ActualPosition of Undefined.");
			}
		}
		protected override Position InternalSubtractFromNull() {
			throw new Exception("Cannot Subtract from Undefined");
		}
		public override Position Clone() {
			return new UndefinedPosition();
		}
		public override int ToInt() {
			throw new Exception("Cannot ToInt the Undefined");
		}
	}
	public class PositionRange {
		Position start;
		Position finish;
		public PositionRange(Position start, Position finish) {
			this.start = start;
			this.finish = finish;
		}
		public Position Start { get { return start; } set { start = value; } }
		public Position Finish { get { return finish; } set { finish = value; } }
	}
	public abstract class TextControllerBase : ISpellCheckTextController {
		string text = string.Empty;
		Position finishPosition = null;
		char[] separators;
		static HashSet<char> wordSeparators = CreateWordSeparators();
		static HashSet<char> uriCharacters = CreateUriCharacters();
		static HashSet<char> CreateWordSeparators() {
			HashSet<char> result = new HashSet<char>();
			result.Add(' ');
			result.Add('\t');
			result.Add('\n');
			result.Add('\r');
			result.Add('(');
			result.Add(')');
			result.Add('[');
			result.Add(']');
			result.Add('*');
			result.Add('{');
			result.Add('}');
			result.Add('<');
			result.Add('>');
			result.Add('/');
			result.Add('\\');
			result.Add('=');
			result.Add('\uFFFC');
			result.Add('\u00A0');
			result.Add('\u2002');
			result.Add('\u2003');
			result.Add('\u2005');
			result.Add('\u000B');
			result.Add('\u000E');
			result.Add((char)0x1D);
			result.Add('\u000C');
			return result;
		}
		static HashSet<char> CreateUriCharacters() {
			HashSet<char> result = new HashSet<char>();
			result.Add('\\');
			result.Add('/');
			result.Add('=');
			result.Add('?');
			result.Add('#');
			result.Add('%');
			result.Add('$');
			result.Add('-');
			result.Add('_');
			result.Add('.');
			result.Add('+');
			result.Add('!');
			result.Add('*');
			result.Add('\'');
			result.Add('(');
			result.Add(')');
			result.Add(':');
			return result;
		}
		protected TextControllerBase() {
		}
		public char[] Separators { get { return separators; } set { separators = value; } }
		public Position FinishPosition {
			get {
				if(finishPosition == null)
					finishPosition = GetTextLength(Text);
				return finishPosition;
			}
			set { finishPosition = value; }
		}
		public string Text { get { return GetText(); } set { SetText(value); } }
		public char this[Position position] {
			get {
				if (Position.IsLess(position, FinishPosition))
					return Text[position.ToInt()];
				return Char.MinValue;
			}
		}
		protected abstract char GetCharAtPos(Position pos);
		protected bool IsWordSeparator(char ch) {
			return wordSeparators.Contains(ch);
		}
		protected virtual bool IsUriCharacter(char ch) {
			return uriCharacters.Contains(ch);
		}
		protected bool IsSeparator(string text, int index) {
			bool isDoublePunctuation = (index > 0) && Char.IsPunctuation(text, index - 1) && Char.IsPunctuation(text, index);
			return isDoublePunctuation || wordSeparators.Contains(text[index]);
		}
		#region TextProcessing
		protected abstract bool DeleteWordCore(PositionRange range);
		protected abstract bool ReplaceWordCore(Position start, Position finish, string word);
		protected abstract string GetWordCore(Position start, Position finish);
		protected abstract Position GetNextPositionCore(Position pos);
		protected abstract Position GetPrevPositionCore(Position pos);
		protected abstract Position GetWordStartPositionCore(Position pos);
		protected abstract Position GetTextLengthCore(string text);
		protected abstract bool HasLettersCore(Position start, Position finish);
		protected abstract string GetPreviouseWordCore(Position pos);
		protected abstract Position GetSentenceStartPositionCore(Position pos);
		protected abstract Position GetSentenceEndPositionCore(Position pos);
		#endregion
		#region ISpellCheckTextController Members
		public Position GetSentenceStartPosition(Position pos) {
			return GetSentenceStartPositionCore(pos);
		}
		public Position GetSentenceEndPosition(Position pos) {
			return GetSentenceEndPositionCore(pos);
		}
		public bool CanDoNextStep(Position pos) {
			return CanDoNextStepCore(pos);
		}
		protected virtual bool CanDoNextStepCore(Position pos) {
			return Position.IsLess(pos, FinishPosition);
		}
		public bool DeleteWord(ref Position start, ref Position finish) {
			PositionRange range = new PositionRange(start, finish);
			bool result = DeleteWordCore(range);
			start = range.Start; 
			finish = range.Finish; 
			return result;
		}
		public string GetPreviousWord(Position pos) {
			return GetPreviouseWordCore(pos);
		}
		public bool ReplaceWord(Position start, Position finish, string word) {
			return ReplaceWordCore(start, finish, word);
		}
		public string GetWord(Position start, Position finish) {
			return GetWordCore(start, finish);
		}
		public Position GetNextPosition(Position pos) {
			return GetNextPositionCore(pos);
		}
		public Position GetPrevPosition(Position pos) {
			return GetPrevPositionCore(pos);
		}
		public Position GetWordStartPosition(Position pos) {
			return GetWordStartPositionCore(pos);
		}
		public Position GetTextLength(string text) {
			return GetTextLengthCore(text);
		}
		public bool HasLetters(Position start, Position finish) {
			return HasLettersCore(start, finish);
		}
		#endregion
		protected virtual string GetText() {
			return text;
		}
		protected virtual void SetText(string value) {
			if (this.text == value)
				return;
			this.text = value;
			OnTextChanged();
		}
		protected virtual void OnTextChanged() {
			UpdateFinishPosition();
		}
		protected virtual void UpdateFinishPosition() {
			this.finishPosition = null;
		}
	}
	public class IntPosition : Position {
		int actualIntPosition = -1;
		static IntPosition nullPosition = CreateNullPosition();
		private static IntPosition CreateNullPosition() {
			return new IntPosition(0);
		}
		public IntPosition(int actualValue)
			: base(actualValue) {
			this.actualIntPosition = actualValue;
		}
		protected virtual IntPosition GetIntPositionFromPosition(Position position) {
			if (position == null)
				return IntPosition.Null;
			return new IntPosition(position.ToInt());
		}
		protected override Position InternalAdd(Position position) {
			IntPosition pos = GetIntPositionFromPosition(position);
			return new IntPosition(ActualIntPosition + pos.ActualIntPosition);
		}
		protected override Position InternalSubtract(Position position) {
			IntPosition pos = GetIntPositionFromPosition(position);
			return new IntPosition(ActualIntPosition - pos.ActualIntPosition);
		}
		protected override int InternalCompare(Position position) {
			IntPosition pos = GetIntPositionFromPosition(position);
			return Comparer<int>.Default.Compare(ActualIntPosition, pos.ActualIntPosition);
		}
		protected override Position MoveForward() {
			return new IntPosition(ActualIntPosition + 1);
		}
		protected override Position MoveBackward() {
			return new IntPosition(ActualIntPosition - 1);
		}
		protected override object ActualPosition {
			get {
				return ActualIntPosition;
			}
			set {
				this.ActualIntPosition = Convert.ToInt32(value);
			}
		}
		public int ActualIntPosition {
			get { return this.actualIntPosition; }
			set { actualIntPosition = value; }
		}
		public new static IntPosition Null {
			get { return nullPosition; }
		}
		protected override Position InternalSubtractFromNull() {
			return new IntPosition(-ActualIntPosition);
		}
		public override Position Clone() {
			return new IntPosition(ActualIntPosition);
		}
		public override string ToString() {
			return ActualIntPosition.ToString();
		}
		public override int ToInt() {
			return ActualIntPosition;
		}
	}
	public class SimpleTextController : TextControllerBase, IUriRecognitionSupport {
		StringBuilder builder = null;
		bool ignoreUri;
		public SimpleTextController()
			: base() {
		}
		public bool IgnoreUri { get { return ignoreUri; } set { ignoreUri = value; } }
		protected virtual void OnTextModified() {
			base.Text = builder.ToString();
		}
		protected virtual UndoManagerBase CreateUndoManager() {
			return new UndoManagerBase(null);
		}
		protected virtual IntPosition GetIntPositionInstance(Position pos) {
			if(pos == null)
				return IntPosition.Null;
			return new IntPosition(pos.ToInt());
		}
		protected override char GetCharAtPos(Position pos) {
			IntPosition position = GetIntPositionInstance(pos);
			return Text[position.ActualIntPosition];
		}
		protected override Position GetTextLengthCore(string text) {
			if(text == null)
				return Position.Null;
			return new IntPosition(text.Length);
		}
		protected override Position GetSentenceStartPositionCore(Position pos) {
			return new IntPosition(0);
		}
		protected override Position GetSentenceEndPositionCore(Position pos) {
			return new IntPosition(Text.Length);
		}
		protected override string GetWordCore(Position start, Position finish) {
			IntPosition startPosition = GetIntPositionInstance(start);
			IntPosition finishPosition = GetIntPositionInstance(finish);
#if !SL
			char[] destination = new char[finishPosition.ActualIntPosition - startPosition.ActualIntPosition];
			builder.CopyTo(startPosition.ActualIntPosition, destination, 0, destination.Length);
			return new string(destination);
#else
			string text = builder.ToString();
			return text.Substring(startPosition.ActualIntPosition, finishPosition.ActualIntPosition - startPosition.ActualIntPosition);
#endif
		}
		protected virtual void CalcPositionsForDelete(PositionRange range) {
			if (Position.IsGreater(range.Start, Position.Null)) {
				IntPosition startPosition = GetIntPositionInstance(range.Start);
				range.Start = new IntPosition(startPosition.ActualIntPosition - 1);
				return;
			}
			if (Position.IsLess(range.Finish, FinishPosition)) {
				IntPosition finishPosition = GetIntPositionInstance(range.Finish);
				range.Finish = new IntPosition(finishPosition.ActualIntPosition + 1);
				return;
			}
		}
		protected virtual void InternalDeleteWord(Position start, Position finish) {
			IntPosition startPosition = GetIntPositionInstance(start);
			IntPosition finishPosition = GetIntPositionInstance(finish);
			builder = builder.Remove(startPosition.ActualIntPosition, finishPosition.ActualIntPosition - startPosition.ActualIntPosition);
		}
		protected override bool DeleteWordCore(PositionRange range) {
			CalcPositionsForDelete(range);
			InternalDeleteWord(range.Start, range.Finish);
			OnTextModified();
			return true; 
		}
		protected override Position GetWordStartPositionCore(Position pos) {
			IntPosition position = GetIntPositionInstance(pos);
			int startIndex = position.ActualIntPosition;
			if (startIndex <= 0)
				return pos;
			int finishIndex = GetIntPositionInstance(FinishPosition).ActualIntPosition;
			if (startIndex == finishIndex)
				startIndex--;
			startIndex = SkipNotWordsCharactersBackward(startIndex);
			if (startIndex == 0)
				return Char.IsLetterOrDigit(Text[startIndex]) ? new IntPosition(0) : pos;
			int result = GetWordStartIndex(startIndex, 0);
			return new IntPosition(result);
		}
		int SkipNotWordsCharactersBackward(int startIndex) {
			if (startIndex > 0) {
				for (int index = startIndex; index > 0; index--) {
					if (index < Text.Length && Char.IsLetterOrDigit(Text[index]))
						return index;
				}
			}
			return 0;
		}
		protected override bool ReplaceWordCore(Position start, Position finish, string word) {
			int startIndex = start.ToInt();
			finish = CorrectFinishPositionsForReplace(finish, word);
			InternalDeleteWord(start, finish);
			builder = builder.Insert(startIndex, word);
			OnTextModified();
			return true; 
		}
		protected Position CorrectFinishPositionsForReplace(Position finish, string word) {
			int finishIndex = finish.ToInt();
			if (finishIndex < Text.Length) {
				char ch = Text[finishIndex];
				int nextCharIndex = finishIndex + 1;
				if (word.EndsWith(ch.ToString(), StringComparison.Ordinal))
					return new IntPosition(nextCharIndex);
			}
			return finish;
		}
		protected override Position GetNextPositionCore(Position pos) {
			IntPosition position = GetIntPositionInstance(pos);
			if (position.ActualIntPosition >= Text.Length)
				return new IntPosition(Text.Length);
			if (position.ActualIntPosition < 0)
				return IntPosition.Null;
			position = GetIntPositionInstance(pos);
			IntPosition finish = GetIntPositionInstance(FinishPosition);
			int endIndex = finish.ActualIntPosition;
			int wordStartIndex = SkipNotLettersOrDigitsForward(position.ActualIntPosition, endIndex);
			if (wordStartIndex >= endIndex)
				return pos;
			int startIndex = SkipUriCharactersBackward(wordStartIndex, 0);
			int result = GetWordEndIndex(startIndex, endIndex);
			if (result == startIndex)
				return pos;
			return new IntPosition(result);
		}
		int SkipNotLettersOrDigitsForward(int startIndex, int endIndex) {
			for (int index = startIndex; index < endIndex; index++) {
				if (Char.IsLetterOrDigit(Text[index]))
					return index;
			}
			return endIndex;
		}
		int SkipUriCharactersBackward(int startIndex, int endIndex) {
			int result = startIndex;
			while (result > endIndex && IsUriCharacter(Text[result - 1]))
				result--;
			return result;
		}
		int GetWordEndIndex(int startIndex, int endIndex) {
			int index = startIndex;
			int result = startIndex;
			bool hasUriCharacters = false;
			while (index < endIndex) {
				if (IsUriCharacter(Text[index])) {
					hasUriCharacters = true;
					index++;
				}
				else if (Char.IsLetterOrDigit(Text[index])) {
					result = SkipWordCharactersForward(index, endIndex);
					index = result;
				}
				else
					break;
			}
			if (hasUriCharacters) {
				string word = Text.Substring(startIndex, result - startIndex);
				if (!IsUriString(word)) {
					int start = SkipNotLettersOrDigitsForward(startIndex, endIndex);
					if (start >= endIndex)
						return startIndex;
					word = Text.Substring(start, result - start);
					if (!IsUriString(word))
						return SkipWordCharactersForward(start, endIndex);
				}
			}
			return result;
		}
		int GetWordStartIndex(int startIndex, int endIndex) {
			Stack<int> separators = new Stack<int>();
			int start = startIndex;
			for (; start >= endIndex; start--) {
				if (Char.IsLetterOrDigit(Text[start])) {
					start = SkipWordCharactersBackward(start, endIndex);
					if (!IsUriCharacter(Text[start]))
						break;
					separators.Push(start);
				}
				else if (!IsUriCharacter(Text[start]))
					break;
			}
			while (true) {
				if (start < endIndex || IsNotLetterOrDigit(Text[start]))
					start++;
				if (separators.Count > 0) {
					string word = Text.Substring(start, startIndex - start + 1);
					if (IsUriString(word))
						return start;
					start = separators.Pop();
				}
				else
					break;
			}
			return start;
		}
		protected override bool IsUriCharacter(char ch) {
			return !IgnoreUri && base.IsUriCharacter(ch);
		}
		bool IsUriString(string uriString) {
			return UriHelper.IsAbsoluteFileUri(uriString) || UriHelper.IsWebUri(uriString);
		}
		int SkipWordCharactersForward(int startIndex, int endIndex) {
			bool canBeAbbreviation = true;
			int index = startIndex;
			for (; index < endIndex; index++) {
				char ch = Text[index];
				int charPosition = index - startIndex;
				canBeAbbreviation = canBeAbbreviation && CanBeAbbreviation(ch, charPosition);
				if (IsNotLetterOrDigit(ch)) {
					if (index == endIndex - 1 || IsNotLetterOrDigit(Text[index + 1])) {
						if (canBeAbbreviation && charPosition >= 3)
							index++;
						break;
					}
				}
				if (IsWordSeparator(ch))
					break;
			}
			return index;
		}
		int SkipWordCharactersBackward(int startIndex, int endIndex) {
			int index = startIndex;
			for (; index >= endIndex; index--) {
				char ch = Text[index];
				if (IsNotLetterOrDigit(ch)) {
					if (index == endIndex || IsNotLetterOrDigit(Text[index - 1]))
						break;
				}
				else if (index == endIndex)
					break;
				if (IsWordSeparator(ch))
					break;
			}
			return index;
		}
		bool CanBeAbbreviation(char ch, int position) {
			switch (position) {
				case 0:
				case 2:
					return Char.IsLetter(ch);
				case 1:
				case 3:
					return ch == '.';
				case 4:
					return IsWordSeparator(ch);
				default:
					return false;
			}
		}
		bool IsNotLetterOrDigit(char ch) {
			return !Char.IsLetterOrDigit(ch);
		}
		protected override Position GetPrevPositionCore(Position pos) {
			IntPosition position = pos as IntPosition;
			if (position.ActualIntPosition > Text.Length)
				return new IntPosition(Text.Length);
			if (position.ActualIntPosition < 0)
				return IntPosition.Null;
			int startIndex = Math.Min(position.ActualIntPosition, Text.Length - 1);
			int result = SkipNotWordsCharactersBackward(startIndex);
			result = GetWordStartIndex(result, 0);
			if (result <= 0)
				return new IntPosition(0);
			result--;
			result = SkipNotWordsCharactersBackward(result);
			if (result <= 0)
				return new IntPosition(0);
			result = GetWordStartIndex(result, 0);
			IntPosition finish = GetIntPositionInstance(FinishPosition);
			int endIndex = Math.Min(finish.ActualIntPosition, Text.Length - 1);
			result = GetWordEndIndex(result, endIndex);
			return new IntPosition(result);
		}
		protected override bool HasLettersCore(Position start, Position finish) {
			string text = GetWord(start, finish);
			text = text.Trim();
			if(String.IsNullOrEmpty(text))
				return false;
			for(int i = 0; i < text.Length; i++)
				if(char.IsLetterOrDigit(text[i]))
					return true;
			return false;
		}
		protected override string GetPreviouseWordCore(Position pos) {
			Position prevPosition = GetPrevPosition(pos);
			if (prevPosition.IsZero)
				return String.Empty;
			Position prevStartPosition = GetWordStartPosition(prevPosition);
			return GetWord(prevStartPosition, prevPosition);
		}
		protected virtual void RecreateTextBuilder() {
			builder = null;
			builder = new StringBuilder(base.Text);
		}
		protected override void OnTextChanged() {
			RecreateTextBuilder();
			base.OnTextChanged();
		}
	}
	internal interface IUriRecognitionSupport {
		bool IgnoreUri { get; set; }
	}
	internal static class UriHelper {
		static readonly HashSet<string> knownExtensions = CreateKnownExtensions();
		static readonly HashSet<string> subdomainPrefixes = CreateSubdomainPrefixes();
		static readonly HashSet<string> topLevelDomains = CreateTopLevelDomains();
		static readonly HashSet<string> schemes = CreateSchemas();
		#region CreateKnownExtensions
		static HashSet<string> CreateKnownExtensions() {
			HashSet<string> result = new HashSet<string>();
			result.Add("doc");
			result.Add("docx");
			result.Add("log");
			result.Add("msg");
			result.Add("rtf");
			result.Add("txt");
			result.Add("csv");
			result.Add("dat");
			result.Add("pps");
			result.Add("ppt");
			result.Add("pptx");
			result.Add("tar");
			result.Add("vcf");
			result.Add("xml");
			result.Add("aif");
			result.Add("mid");
			result.Add("mp3");
			result.Add("mpa");
			result.Add("wav");
			result.Add("wma");
			result.Add("avi");
			result.Add("mov");
			result.Add("mp4");
			result.Add("mpg");
			result.Add("rm");
			result.Add("swf");
			result.Add("wmv");
			result.Add("obj");
			result.Add("bmp");
			result.Add("gif");
			result.Add("jpg");
			result.Add("png");
			result.Add("tiff");
			result.Add("ai");
			result.Add("ps");
			result.Add("pct");
			result.Add("pdf");
			result.Add("xls");
			result.Add("xlsx");
			result.Add("accdb");
			result.Add("mdb");
			result.Add("pdb");
			result.Add("bat");
			result.Add("com");
			result.Add("exe");
			result.Add("jar");
			result.Add("wsf");
			result.Add("asp");
			result.Add("aspx");
			result.Add("cer");
			result.Add("css");
			result.Add("htm");
			result.Add("html");
			result.Add("js");
			result.Add("fon");
			result.Add("otf");
			result.Add("ttf");
			result.Add("cab");
			result.Add("dll");
			result.Add("drv");
			result.Add("ico");
			result.Add("sys");
			result.Add("ini");
			result.Add("hqx");
			result.Add("gz");
			result.Add("tar.gz");
			result.Add("zip");
			result.Add("cpp");
			result.Add("dtd");
			result.Add("java");
			result.Add("pl");
			result.Add("py");
			result.Add("sh");
			result.Add("sln");
			result.Add("ics");
			result.Add("msi");
			return result;
		}
		#endregion
		#region CreateSubdomainPrefixes
		static HashSet<string> CreateSubdomainPrefixes() {
			HashSet<string> result = new HashSet<string>();
			result.Add("www");
			result.Add("ftp");
			return result;
		}
		#endregion
		#region CreateTopLevelDomains
		static HashSet<string> CreateTopLevelDomains() {
			HashSet<string> result = new HashSet<string>();
			result.Add("com");
			result.Add("org");
			result.Add("net");
			result.Add("int");
			result.Add("edu");
			result.Add("gov");
			result.Add("mil");
			result.Add("ac");
			result.Add("ad");
			result.Add("ae");
			result.Add("af");
			result.Add("ag");
			result.Add("ai");
			result.Add("al");
			result.Add("am");
			result.Add("an");
			result.Add("ao");
			result.Add("aq");
			result.Add("ar");
			result.Add("as");
			result.Add("at");
			result.Add("au");
			result.Add("aw");
			result.Add("ax");
			result.Add("az");
			result.Add("ba");
			result.Add("bb");
			result.Add("bd");
			result.Add("be");
			result.Add("bf");
			result.Add("bg");
			result.Add("bh");
			result.Add("bi");
			result.Add("bj");
			result.Add("bm");
			result.Add("bn");
			result.Add("bo");
			result.Add("bq");
			result.Add("br");
			result.Add("bs");
			result.Add("bt");
			result.Add("bv");
			result.Add("bw");
			result.Add("by");
			result.Add("bz");
			result.Add("ca");
			result.Add("cc");
			result.Add("cd");
			result.Add("cf");
			result.Add("cg");
			result.Add("ch");
			result.Add("ci");
			result.Add("ck");
			result.Add("cl");
			result.Add("cm");
			result.Add("cn");
			result.Add("co");
			result.Add("cr");
			result.Add("cs");
			result.Add("cu");
			result.Add("cv");
			result.Add("cw");
			result.Add("cx");
			result.Add("cy");
			result.Add("cz");
			result.Add("dd");
			result.Add("de");
			result.Add("dj");
			result.Add("dk");
			result.Add("dm");
			result.Add("do");
			result.Add("dz");
			result.Add("ec");
			result.Add("ee");
			result.Add("eg");
			result.Add("eh");
			result.Add("er");
			result.Add("es");
			result.Add("et");
			result.Add("eu");
			result.Add("fi");
			result.Add("fj");
			result.Add("fk");
			result.Add("fm");
			result.Add("fo");
			result.Add("fr");
			result.Add("ga");
			result.Add("gb");
			result.Add("gd");
			result.Add("ge");
			result.Add("gf");
			result.Add("gg");
			result.Add("gh");
			result.Add("gi");
			result.Add("gl");
			result.Add("gm");
			result.Add("gn");
			result.Add("gp");
			result.Add("gq");
			result.Add("gr");
			result.Add("gs");
			result.Add("gt");
			result.Add("gu");
			result.Add("gw");
			result.Add("gy");
			result.Add("hk");
			result.Add("hm");
			result.Add("hn");
			result.Add("hr");
			result.Add("ht");
			result.Add("hu");
			result.Add("id");
			result.Add("ie");
			result.Add("il");
			result.Add("im");
			result.Add("in");
			result.Add("io");
			result.Add("iq");
			result.Add("ir");
			result.Add("is");
			result.Add("it");
			result.Add("je");
			result.Add("jm");
			result.Add("jo");
			result.Add("jp");
			result.Add("ke");
			result.Add("kg");
			result.Add("kh");
			result.Add("ki");
			result.Add("km");
			result.Add("kn");
			result.Add("kp");
			result.Add("kr");
			result.Add("kw");
			result.Add("ky");
			result.Add("kz");
			result.Add("la");
			result.Add("lb");
			result.Add("lc");
			result.Add("li");
			result.Add("lk");
			result.Add("lr");
			result.Add("ls");
			result.Add("lt");
			result.Add("lu");
			result.Add("lv");
			result.Add("ly");
			result.Add("ma");
			result.Add("mc");
			result.Add("md");
			result.Add("me");
			result.Add("mg");
			result.Add("mh");
			result.Add("mk");
			result.Add("ml");
			result.Add("mm");
			result.Add("mn");
			result.Add("mo");
			result.Add("mp");
			result.Add("mq");
			result.Add("mr");
			result.Add("ms");
			result.Add("mt");
			result.Add("mu");
			result.Add("mv");
			result.Add("mw");
			result.Add("mx");
			result.Add("my");
			result.Add("mz");
			result.Add("na");
			result.Add("nc");
			result.Add("ne");
			result.Add("nf");
			result.Add("ng");
			result.Add("ni");
			result.Add("nl");
			result.Add("no");
			result.Add("np");
			result.Add("nr");
			result.Add("nu");
			result.Add("nz");
			result.Add("om");
			result.Add("pa");
			result.Add("pe");
			result.Add("pf");
			result.Add("pg");
			result.Add("ph");
			result.Add("pk");
			result.Add("pl");
			result.Add("pm");
			result.Add("pn");
			result.Add("pr");
			result.Add("ps");
			result.Add("pt");
			result.Add("pw");
			result.Add("py");
			result.Add("qa");
			result.Add("re");
			result.Add("ro");
			result.Add("rs");
			result.Add("ru");
			result.Add("rw");
			result.Add("sa");
			result.Add("sb");
			result.Add("sc");
			result.Add("sd");
			result.Add("se");
			result.Add("sg");
			result.Add("sh");
			result.Add("si");
			result.Add("sj");
			result.Add("sk");
			result.Add("sl");
			result.Add("sm");
			result.Add("sn");
			result.Add("so");
			result.Add("sr");
			result.Add("ss");
			result.Add("st");
			result.Add("su");
			result.Add("sv");
			result.Add("sx");
			result.Add("sy");
			result.Add("sz");
			result.Add("tc");
			result.Add("td");
			result.Add("tf");
			result.Add("tg");
			result.Add("th");
			result.Add("tj");
			result.Add("tk");
			result.Add("tl");
			result.Add("tm");
			result.Add("tn");
			result.Add("to");
			result.Add("tp");
			result.Add("tr");
			result.Add("tt");
			result.Add("tv");
			result.Add("tw");
			result.Add("tz");
			result.Add("ua");
			result.Add("ug");
			result.Add("uk");
			result.Add("us");
			result.Add("uy");
			result.Add("uz");
			result.Add("va");
			result.Add("vc");
			result.Add("ve");
			result.Add("vg");
			result.Add("vi");
			result.Add("vn");
			result.Add("vu");
			result.Add("wf");
			result.Add("ws");
			result.Add("ye");
			result.Add("yt");
			result.Add("yu");
			result.Add("za");
			result.Add("zm");
			result.Add("zr");
			result.Add("zw");
			return result;
		}
		#endregion
		#region CreateSchemas
		static HashSet<string> CreateSchemas() {
			HashSet<string> result = new HashSet<string>();
			result.Add("file");
			result.Add("ftp");
			result.Add("gopher");
			result.Add("http");
			result.Add("https");
			result.Add("mailto");
			result.Add("telnet");
			result.Add("news");
			result.Add("nntp");
			return result;
		}
		#endregion
		static bool IsFileExtension(string value) {
			return knownExtensions.Contains(value);
		}
		static bool IsSubdomainPrefix(string value) {
			return subdomainPrefixes.Contains(value);
		}
		static bool IsTopLevelDomain(string value) {
			return topLevelDomains.Contains(value);
		}
		static bool IsKnownScheme(string value) {
			return schemes.Contains(value);
		}
		public static bool IsUri(string uriString) {
			return IsFileUri(uriString) || IsWebUri(uriString);
		}
		public static bool IsAbsoluteFileUri(string uriString) {
			Uri uri;
			return Uri.TryCreate(uriString, UriKind.Absolute, out uri) && uri.IsFile;
		}
		public static bool IsFileUri(string uriString) {
			if (IsAbsoluteFileUri(uriString))
				return true;
			int dotIndex = uriString.LastIndexOf('.');
			if (dotIndex <= 0 || dotIndex == uriString.Length - 1)
				return false;
			string extension = uriString.Substring(dotIndex + 1);
			if (IsFileExtension(extension))
				return true;
			return false;
		}
		public static bool IsWebUri(string urlString) {
			bool isSchemeKnown = false;
			bool isSchemeCorrect = false;
			int tokenStart = 0;
			int index = 0;
			while (tokenStart + index < urlString.Length) {
				char ch = urlString[tokenStart + index];
				if (isSchemeCorrect) {
					if (index == 0 || index == 1) {
						bool isValid = false;
						if (isSchemeKnown)
							isValid = ch == '/' || Char.IsLetterOrDigit(ch);
						else
							isValid = ch == '/';
						if (!isValid)
							return false;
						index++;
					}
					else
						return true;
				}
				else if (ch == ':') {
					string scheme = urlString.Substring(tokenStart, index);
					isSchemeKnown = IsKnownScheme(scheme);
					if (isSchemeKnown)
						isSchemeCorrect = true;
					else
						isSchemeCorrect = Uri.CheckSchemeName(scheme);
					if (!isSchemeCorrect)
						return false;
					tokenStart += index + 1;
					index = 0;
				}
				else if (ch == '.') {
					if (tokenStart == 0) {
						string prefix = urlString.Substring(tokenStart, index);
						if (IsSubdomainPrefix(prefix))
							return true;
					}
					tokenStart += index + 1;
					index = 0;
					string suffix = urlString.Substring(tokenStart);
					if (IsTopLevelDomain(suffix))
						return true;
				}
				else
					index++;
			}
			return false;
		}
	}
	public class EmptyEditControlTextControlController : SimpleTextController, ISpellCheckTextControlController {
		Position selectionStart;
		Position selectionFinish;
		public EmptyEditControlTextControlController() {
			ResetSelection();
		}
		public string EditControlText { get { return Text; } set { Text = value; } }
		public bool HasSelection { get { return SelectionStart != Position.Undefined && SelectionFinish != Position.Undefined; } }
		public bool IsReadOnly { get { return false; } }
		public Position SelectionFinish { get { return selectionStart; } }
		public Position SelectionStart { get { return selectionFinish; } }
		protected override void OnTextModified() {
			ResetSelection();
			base.OnTextModified();
		}
		protected override void OnTextChanged() {
			ResetSelection();
			base.OnTextChanged();
		}
		void ResetSelection() {
			this.selectionStart = Position.Undefined;
			this.selectionFinish = Position.Undefined;
		}
		public void Select(Position start, Position finish) {
			this.selectionStart = start;
			this.selectionFinish = finish;
		}
		public void HideSelection() {
		}
		public bool IsSelectionVisible() {
			return false;
		}
		public void ScrollToCaretPos() {
		}
		public void ShowSelection() {
		}
		public void UpdateText() {
		}
		public void Dispose() {
		}
	}
	public abstract class UndoItemBase : IUndoItem {
		SearchStrategy searchStrategy;
		Position startPosition = Position.Null;
		Position finishPosition = Position.Null;
		string oldText = string.Empty;
		protected UndoItemBase(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		public Position StartPosition { get { return startPosition; } set { startPosition = value; } }
		public Position FinishPosition { get { return finishPosition; } set { finishPosition = value; } }
		public string OldText { get { return oldText; } set { this.oldText = value; } }
		public SearchStrategy SearchStrategy { get { return this.searchStrategy; } }
		public virtual bool NeedRecheckWord { get { return false; } }
		public bool ShouldUpdateItemPosition { get { return false; } }
		protected abstract void DoAtomarUndoOperationCore();
		protected void AfterDoDoAtomarUndoOperationCore() {
			this.searchStrategy = null;
		}
		public void DoUndo() {
			DoAtomarOperation();
		}
		protected void DoAtomarOperation() {
			try {
				DoAtomarUndoOperationCore();
			}
			finally {
				AfterDoDoAtomarUndoOperationCore();
			}
		}
	}
	public class UndoReplaceItem : UndoItemBase {
		public UndoReplaceItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override void DoAtomarUndoOperationCore() {
			SearchStrategy.CurrentPosition = FinishPosition;
			SearchStrategy.WordStartPosition = StartPosition;
			SearchStrategy.DoSpellCheckOperationCore(SpellCheckOperation.Change, OldText);
		}
		public override bool NeedRecheckWord { get { return true; } }
	}
	public class UndoDeleteItem : UndoReplaceItem {
		public UndoDeleteItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override void DoAtomarUndoOperationCore() {
			SearchStrategy.CurrentPosition = StartPosition;
			SearchStrategy.WordStartPosition = StartPosition;
			SearchStrategy.DoSpellCheckOperationCore(SpellCheckOperation.Change, OldText);
		}
	}
	public class UndoIgnoreItem : UndoItemBase {
		public UndoIgnoreItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override void DoAtomarUndoOperationCore() {
			SearchStrategy.IgnoreList.Remove(this.StartPosition, this.FinishPosition, this.OldText);
		}
		public override bool NeedRecheckWord { get { return true; } }
	}
	public class UndoSilentReplaceItem : UndoReplaceItem {
		public UndoSilentReplaceItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override bool NeedRecheckWord { get { return false; } }
	}
	public class UndoAddWordItem : UndoItemBase {
		public UndoAddWordItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override bool NeedRecheckWord { get { return true; } }
		protected override void DoAtomarUndoOperationCore() {
			SpellCheckerCustomDictionary customDictionary = SearchStrategy.DictionaryHelper.GetCustomDictionary(SearchStrategy.ActualCulture);
			int wordIndex = customDictionary.WordList.BinarySearch(OldText.ToLower(customDictionary.Culture));
			if (wordIndex < 0)
				return;
			customDictionary.WordList.RemoveAt(wordIndex);
			customDictionary.Save();
		}
	}
	public class UndoChangeAllItem : UndoItemBase {
		public UndoChangeAllItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override bool NeedRecheckWord { get { return false; } }
		protected override void DoAtomarUndoOperationCore() {
			SearchStrategy.ChangeAllList.Remove(this.OldText);
		}
	}
	public class UndoIgnoreAllItem : UndoItemBase {
		public UndoIgnoreAllItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override bool NeedRecheckWord { get { return false; } }
		protected override void DoAtomarUndoOperationCore() {
			SearchStrategy.IgnoreList.Remove(OldText);
		}
	}
	public class UndoIgnoreAllItemWrapper : IUndoItem {
		readonly IUndoItem ignoreItem;
		readonly SearchStrategy searchStrategy;
		public UndoIgnoreAllItemWrapper(IUndoItem ignoreItem, SearchStrategy searchStrategy) {
			this.ignoreItem = ignoreItem;
			this.searchStrategy = searchStrategy;
		}
		protected internal IUndoItem IgnoreItem { get { return ignoreItem; } }
		public Position FinishPosition { get { return IgnoreItem.FinishPosition; } set { IgnoreItem.FinishPosition = value; } }
		public bool NeedRecheckWord { get { return IgnoreItem.NeedRecheckWord; } }
		public string OldText { get { return IgnoreItem.OldText; } set { IgnoreItem.OldText = value; } }
		public bool ShouldUpdateItemPosition { get { return IgnoreItem.ShouldUpdateItemPosition; } }
		public Position StartPosition { get { return IgnoreItem.StartPosition; } set { IgnoreItem.StartPosition = value; } }
		public void DoUndo() {
			IgnoreItem.DoUndo();
			IIgnoreList ignoreList = searchStrategy.SpellChecker.IgnoreList;
			if (ignoreList.Contains(OldText))
				ignoreList.Remove(OldText);
		}
	}
	public class UndoSilentIgnoreItem : UndoIgnoreItem {
		public UndoSilentIgnoreItem(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override bool NeedRecheckWord { get { return false; } }
	}
	public class CompositeUndoItem {
		List<IUndoItem> undoItems = new List<IUndoItem>();
		bool needRecheckWord = false;
		Position position = Position.Undefined;
		SearchStrategy searchStrategy = null;
		public CompositeUndoItem(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		protected virtual SearchStrategy SearchStrategy {
			get { return this.searchStrategy; }
		}
		protected List<IUndoItem> UndoItems { get { return undoItems; } }
		public virtual void AddUndoItem(IUndoItem item) {
			undoItems.Add(item);
		}
		public virtual void RemoveUndoItem(UndoItemBase item) {
			undoItems.Remove(item);
		}
		public virtual bool NeedRecheckWord {
			get { return needRecheckWord; }
			set {
				if(NeedRecheckWord != value)
					needRecheckWord = value;
			}
		}
		public Position Position {
			get { return position; }
			set {
				if(value == Position.Undefined)
					return;
				if(Position.IsLess(Position, value))
					position = value;
			}
		}
		protected virtual void StartTransaction() {
			needRecheckWord = false;
			position = Position.Undefined;
		}
		protected virtual void EndTransaction() {
			if(NeedRecheckWord) {
				SearchStrategy.CurrentPosition = Position;
			}
		}
		protected virtual void DoCompositeUndoOperationCore() {
			while(UndoItems.Count > 0) {
				IUndoItem undoItem = UndoItems[UndoItems.Count - 1] as IUndoItem;
				UndoItems.RemoveAt(UndoItems.Count - 1);
				undoItem.DoUndo();
				if (undoItem.NeedRecheckWord) {
					Position = undoItem.StartPosition;
					NeedRecheckWord = undoItem.NeedRecheckWord;
				}
				SearchStrategy.OperationsManager.OnUndo(undoItem);
			}
		}
		public virtual void DoCompositeUndoOperation() {
			StartTransaction();
			try {
				DoCompositeUndoOperationCore();
			}
			finally {
				EndTransaction();
			}
		}
		public virtual bool HasUndoItems() {
			return UndoItems.Count > 0;
		}
		protected virtual bool ShouldUpdateItemPosition(IUndoItem item) {
			return item.NeedRecheckWord || item.ShouldUpdateItemPosition;
		}
		public virtual void UpdateItemPositionsByChangeAll() {
			foreach(IUndoItem undoItem in UndoItems) {
				if(!ShouldUpdateItemPosition(undoItem)) continue;
				if(Position.IsLess(SearchStrategy.WordStartPosition, undoItem.StartPosition)) {
					undoItem.StartPosition = Position.Add(undoItem.StartPosition, SearchStrategy.DeltaPosition);
					undoItem.FinishPosition = Position.Add(undoItem.FinishPosition, SearchStrategy.DeltaPosition);
				}
			}
		}
	}
	public class UndoController : IUndoController {
		UndoManagerBase undoManager = null;
		public UndoController(object undoManager) {
			this.undoManager = undoManager as UndoManagerBase;
		}
		public IUndoItem GetUndoItemForDelete() {
			return new UndoDeleteItem(undoManager.SearchStrategy);
		}
		public IUndoItem GetUndoItemForReplace() {
			return new UndoReplaceItem(undoManager.SearchStrategy);
		}
		public IUndoItem GetUndoItemForSilentReplace() {
			return new UndoSilentReplaceItem(undoManager.SearchStrategy);
		}
		public IUndoItem GetUndoItemForIgnore() {
			return new UndoIgnoreItem(undoManager.SearchStrategy);
		}
		public IUndoItem GetUndoItemForIgnoreAll() {
			return new UndoIgnoreAllItem(undoManager.SearchStrategy);
		}
	}
	public class UndoManagerBase : IDisposable {
		Stack<CompositeUndoItem> undoList;
		SearchStrategy searchStrategy = null;
		CompositeUndoItem compositeItem = null;
		IUndoController undoController = null;
		public UndoManagerBase(SearchStrategy searchStrategy) {
			this.undoList = CreateUndoList();
			this.searchStrategy = searchStrategy;
			this.undoController = GetUndoController();
		}
		protected virtual IUndoController GetUndoController() {
			IUndoController result = UndoControllerRepository.GetUndoController(SearchStrategy.SpellChecker.EditControl);
			if(result != null)
				return result;
			return GetDefaultUndoController();
		}
		private IUndoController GetDefaultUndoController() {
			return new UndoController(this);
		}
		#region UndoItems
		public const int ReplaceActionID = 0;
		public const int DeleteActionID = 1;
		public const int IgnoreActionID = 2;
		public const int SilentChangeActionID = 3;
		public const int AddWordActionID = 4;
		public const int ChangeAllActionID = 5;
		public const int IgnoreAllActionID = 6;
		public const int SilentIgnoreActionID = 7;
		protected virtual IUndoItem CreateUndoReplaceItem() {
			if(undoController == null)
				throw new NullReferenceException("UndoController is null");
			return undoController.GetUndoItemForReplace();
		}
		protected virtual IUndoItem CreateUndoDeleteItem() {
			if(undoController == null)
				throw new NullReferenceException("UndoController is null");
			return undoController.GetUndoItemForDelete();
		}
		protected virtual IUndoItem CreateUndoSilentReplaceItem() {
			if(undoController == null)
				throw new NullReferenceException("UndoController is null");
			return undoController.GetUndoItemForSilentReplace();
		}
		protected virtual IUndoItem CreateUndoIgnoreItem() {
			if (undoController == null)
				throw new NullReferenceException("UndoController is null");
			return undoController.GetUndoItemForIgnore();
		}
		protected virtual IUndoItem CreateUndoAddWordItem() {
			return new UndoAddWordItem(SearchStrategy);
		}
		protected virtual IUndoItem CreateUndoChangeAllItem() {
			return new UndoChangeAllItem(SearchStrategy);
		}
		protected virtual IUndoItem CreateUndoIgnoreAllItem() {
			if (undoController == null)
				throw new NullReferenceException("UndoController is null");
			return new UndoIgnoreAllItemWrapper(undoController.GetUndoItemForIgnoreAll(), searchStrategy);
		}
		protected virtual IUndoItem CreateUndoSilentIgnoreItem() {
			return new UndoSilentIgnoreItem(SearchStrategy);
		}
		protected internal virtual IUndoItem GetUndoItemInstanceByActionID(int actionID) {
			switch (actionID) {
				case ReplaceActionID:
					return CreateUndoReplaceItem();
				case DeleteActionID:
					return CreateUndoDeleteItem();
				case IgnoreActionID:
					return CreateUndoIgnoreItem();
				case SilentChangeActionID:
					return CreateUndoSilentReplaceItem();
				case AddWordActionID:
					return CreateUndoAddWordItem();
				case ChangeAllActionID:
					return CreateUndoChangeAllItem();
				case IgnoreAllActionID:
					return CreateUndoIgnoreAllItem();
				case SilentIgnoreActionID:
					return CreateUndoSilentIgnoreItem();
				default:
					throw new InvalidOperationException("Cannot create UndoItem for operation with the ActionID = " + actionID.ToString());
			}
		}
		protected CompositeUndoItem CreateCompositeUndoItem() {
			return new CompositeUndoItem(SearchStrategy);
		}
		#endregion
		protected internal virtual SearchStrategy SearchStrategy { get { return searchStrategy; } }
		protected internal virtual CompositeUndoItem CompositeItem { get { return compositeItem; } }
		protected Stack<CompositeUndoItem> UndoList { get { return undoList; } }
		protected ISpellCheckTextController TextController { get { return SearchStrategy.TextController; } }
		protected virtual Stack<CompositeUndoItem> CreateUndoList() {
			return new Stack<CompositeUndoItem>();
		}
		public virtual void StartWriteComplexAction() {
			compositeItem = CreateCompositeUndoItem();
		}
		public virtual void FinishWriteComplexAction() {
			if(CompositeItem.HasUndoItems())
				UndoList.Push(compositeItem);
			compositeItem = null;
		}
		protected virtual CompositeUndoItem GetCompositeItem(int actionID, string oldText) {
			return CompositeItem;
		}
		public void Add(int actionID, Position startPosition, Position finishPosition, string oldText) {
			CompositeUndoItem cItem = GetCompositeItem(actionID, oldText);
			if(cItem != null) {  
				IUndoItem undoItem = GetUndoItemInstanceByActionID(actionID);
				undoItem.StartPosition = startPosition;
				undoItem.FinishPosition = finishPosition;
				undoItem.OldText = oldText;
				cItem.AddUndoItem(undoItem);
			}
		}
		protected virtual CompositeUndoItem GetUndoItem() {
			return UndoList.Pop();
		}
		public virtual void DoUndo() {
			while (IsUndoAvailable()) {
				compositeItem = GetUndoItem();
				CompositeItem.DoCompositeUndoOperation();
				if (CompositeItem.NeedRecheckWord)
					break;
			}
		}
		public virtual void Reset() {
			undoList.Clear();
		}
		public virtual bool IsUndoAvailable() {
			return UndoList.Count > 0;
		}
		public void Dispose() {
			searchStrategy = null;
		}
		public void UpdateCompositeItemPositionsByChangeAll() {
			IEnumerator enumerator = UndoList.GetEnumerator();
			enumerator.Reset();
			while(enumerator.MoveNext())
				((CompositeUndoItem)enumerator.Current).UpdateItemPositionsByChangeAll();
			if(CompositeItem != null && CompositeItem.HasUndoItems())
				CompositeItem.UpdateItemPositionsByChangeAll();
		}
	}
	public class SuggestionsComparer : IComparer<SuggestionBase> {
		readonly string word;
		public SuggestionsComparer(string word) {
			this.word = word;
		}
		public int Compare(SuggestionBase x, SuggestionBase y) {
			int result = CompareDistances(x, y);
			if (result != 0)
				return result;
			return x.Suggestion.CompareTo(y.Suggestion);
		}
		internal int CompareDistances(SuggestionBase x, SuggestionBase y) {
			int result = x.Distance.CompareTo(y.Distance);
			if (result != 0)
				return result;
			return CalculateMatchFactor(x.Suggestion) - CalculateMatchFactor(y.Suggestion);
		}
		int CalculateMatchFactor(string sugestion) {
			int result = 0;
			result += ((sugestion[0] != word[0]) ? 1 : 0);
			result += ((sugestion[sugestion.Length - 1] != word[word.Length - 1]) ? 1 : 0);
			return result;
		}
	}
	public enum FormatOperations { DeleteOperation, InsertOperation, SubstituteOperation };
#if !WPF
	public class SuggestionFormatter {
		string result;
		bool needTerminate = false;
		const int InvalidOperationCost = 1000;
		int[,] matrix = null;
		int maxX = -1, maxY = -1;
		CultureInfo culture;
		protected virtual string Result {
			get { return result; }
			set {
				if(result != value)
					result = value;
			}
		}
		public string CalcSuggestion(string suggestion, string wrongWord, CultureInfo culture) {
			this.culture = culture;
			Reset();
			CalcAlignmentMatrix(suggestion, wrongWord);
			return ProcessCell(new Point(0, 0), new Point(0, 0), Matrix[MaxX, MaxY], wrongWord, suggestion, 0, wrongWord);
		}
		public CultureInfo Culture { get { return culture; } }
		protected virtual void CalcAlignmentMatrix(string suggestion, string wrongWord) {
			matrix = LevenshteinAlgorithm.CalcAlignmentMatrix(suggestion, wrongWord, Culture);
		}
		public int[,] Matrix {
			get { return matrix; }
		}
		protected int MaxX {
			get {
				if(matrix != null) {
					if(maxX == -1)
						maxX = matrix.GetUpperBound(0);
					return maxX;
				}
				return -1;
			}
		}
		protected int MaxY {
			get {
				if(matrix != null) {
					if(maxY == -1)
						maxY = matrix.GetUpperBound(1);
					return maxY;
				}
				return -1;
			}
		}
		protected virtual void Reset() {
			needTerminate = false;
			Result = string.Empty;
			matrix = null;
			maxX = -1;
			maxY = -1;
		}
		protected virtual bool CheckStep(int index, string tempWord, string aim) {
			while(aim.Length < index)
				index--;
			while(tempWord.Length < index)
				index--;
			string word1 = tempWord.Substring(0, index);
			string word2 = aim.Substring(0, index);
#if SL
			return string.Compare(word1, word2, Culture, CompareOptions.IgnoreCase) == 0;
#else
			return string.Compare(word1, word2, true, Culture) == 0;
#endif
		}
		bool NeedTerminate {
			get { return needTerminate; }
		}
		protected virtual void Terminate() {
			needTerminate = true;
		}
		bool IsDeletion(Point currentPoint, Point prevPoint) {
			return currentPoint.X == prevPoint.X && currentPoint.Y == prevPoint.Y + 1 &&
				GetCurrentPrice(currentPoint) == GetCurrentPrice(prevPoint) + 1;
		}
		bool IsInsertion(Point currentPoint, Point prevPoint) {
			return currentPoint.Y == prevPoint.Y && currentPoint.X == prevPoint.X + 1 &&
				GetCurrentPrice(currentPoint) == GetCurrentPrice(prevPoint) + 1;
		}
		bool IsSubstitution(Point currentPoint, Point prevPoint) {
			return currentPoint.Y == prevPoint.Y + 1 && currentPoint.X == prevPoint.X + 1 &&
				GetCurrentPrice(currentPoint) == GetCurrentPrice(prevPoint) ||
				GetCurrentPrice(currentPoint) == GetCurrentPrice(prevPoint) + 1;
		}
		bool CanDoSubstitute(Point currentPoint) {
			return currentPoint.X < MaxX && currentPoint.Y < MaxY;
		}
		bool CanDoSubstitute(Point currentPoint, int localMax) {
			return CanDoSubstitute(currentPoint) && Matrix[currentPoint.X + 1, currentPoint.Y + 1] <= localMax;
		}
		bool CanDoInsert(Point currentPoint) {
			return currentPoint.X < MaxX && currentPoint.Y <= MaxY &&
				Matrix[currentPoint.X + 1, currentPoint.Y] == GetCurrentPrice(currentPoint) + 1;  
		}
		bool CanDoInsert(Point currentPoint, int localMax) {
			return CanDoInsert(currentPoint) && Matrix[currentPoint.X + 1, currentPoint.Y] <= localMax;
		}
		bool CanDoDelete(Point currentPoint) {
			return currentPoint.X <= MaxX && currentPoint.Y < MaxY &&
				Matrix[currentPoint.X, currentPoint.Y + 1] == GetCurrentPrice(currentPoint) + 1; 
		}
		bool CanDoDelete(Point currentPoint, int localMax) {
			return CanDoDelete(currentPoint) && Matrix[currentPoint.X, currentPoint.Y + 1] <= localMax;
		}
		int GetCurrentPrice(Point currentPoint) {
			if(currentPoint.X <= MaxX && currentPoint.Y <= MaxY)
				return Matrix[currentPoint.X, currentPoint.Y];
			else
				return SuggestionFormatter.InvalidOperationCost;
		}
		int GetDeletePrice(Point currentPoint) {
			if(CanDoDelete(currentPoint))
				return Matrix[currentPoint.X, currentPoint.Y + 1];
			else
				return SuggestionFormatter.InvalidOperationCost;
		}
		int GetInsertPrice(Point currentPoint) {
			if(CanDoInsert(currentPoint))
				return Matrix[currentPoint.X + 1, currentPoint.Y];
			else
				return SuggestionFormatter.InvalidOperationCost;
		}
		int GetSubstitutionPrice(Point currentPoint) {
			if(CanDoSubstitute(currentPoint))
				return Matrix[currentPoint.X + 1, currentPoint.Y + 1];
			else
				return SuggestionFormatter.InvalidOperationCost;
		}
		protected virtual bool IsValidStep(Point currentPoint, Point prevPoint) {
			if((CanDoInsert(prevPoint) && IsInsertion(currentPoint, prevPoint)) ||
				(CanDoDelete(prevPoint) && IsDeletion(currentPoint, prevPoint)))
				return GetCurrentPrice(currentPoint) == GetCurrentPrice(prevPoint) + 1;
			else
				if(CanDoSubstitute(prevPoint))
					return (Matrix[currentPoint.X, currentPoint.Y] == Matrix[prevPoint.X, prevPoint.Y] + 1) ||
						(Matrix[currentPoint.X, currentPoint.Y] == Matrix[prevPoint.X, prevPoint.Y]);
			return false;
		}
		int CalcLocalMax(Point currentPoint, int maxValue) {
			int tempValue = LevenshteinAlgorithm.Min(GetSubstitutionPrice(currentPoint),
				GetInsertPrice(currentPoint), GetDeletePrice(currentPoint));
			return tempValue < maxValue ? tempValue : maxValue;
		}
		protected virtual string ProcessCell(Point currentPoint, Point prevPoint, int maxValue, string tempWord, string aim, int letterIndex, string originalBadWord) {
			if(NeedTerminate) return Result;
			if(!IsValidStep(currentPoint, prevPoint))
				return Result;
			if(Matrix[currentPoint.X, currentPoint.Y] <= maxValue) {
				if(IsDeletion(currentPoint, prevPoint))
					tempWord = FormatSuggestion(tempWord, FormatOperations.DeleteOperation, letterIndex, char.MinValue);
				else {
					if(IsInsertion(currentPoint, prevPoint))
						tempWord = FormatSuggestion(tempWord, FormatOperations.InsertOperation, letterIndex - 1, aim[prevPoint.X]);
					else
						if(IsSubstitution(currentPoint, prevPoint)) 
							tempWord = FormatSuggestion(tempWord, FormatOperations.SubstituteOperation, letterIndex - 1, aim[prevPoint.X]);
				}
				if(currentPoint.X == MaxX && currentPoint.Y == MaxY) {
					if(CheckResult(tempWord, aim)) {
						Terminate();
						return Result;
					}
				}
				int localMax = CalcLocalMax(currentPoint, maxValue);
				string storedTempWord = tempWord;
				if(CanDoSubstitute(currentPoint, localMax))
					ProcessCell(new Point(currentPoint.X + 1, currentPoint.Y + 1), currentPoint, maxValue, tempWord, aim, letterIndex + 1, originalBadWord);
				if(NeedTerminate) return Result;
				tempWord = storedTempWord;
				if(CanDoInsert(currentPoint, localMax))
					ProcessCell(new Point(currentPoint.X + 1, currentPoint.Y), currentPoint, maxValue, tempWord, aim, letterIndex + 1, originalBadWord);
				if(NeedTerminate) return Result;
				tempWord = storedTempWord;
				if(CanDoDelete(currentPoint, localMax))
					ProcessCell(new Point(currentPoint.X, currentPoint.Y + 1), currentPoint, maxValue, tempWord, aim, letterIndex, originalBadWord);
				if(NeedTerminate) return Result;
			}
			return Result;
		}
		protected virtual bool CheckResult(string word, string aim) {
			if(aim.Length == word.Length && CheckStep(aim.Length, word, aim)) {
				Result = word;
				return true;
			}
			return false;
		}
		public virtual string FormatSuggestion(string tempWord, FormatOperations operation, int position, char letter) {
			string suggestion = "";
			switch(operation) {
				case FormatOperations.DeleteOperation: {
					suggestion = DoDelete(tempWord, position);
					break;
				}
				case FormatOperations.InsertOperation: {
					suggestion = DoInsert(tempWord, position, letter);
					break;
				}
				case FormatOperations.SubstituteOperation: {
					suggestion = DoSubstitute(tempWord, position, letter);
					break;
				}
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return suggestion;
		}
		protected virtual string DoDelete(string tempWord, int position) {
			if(tempWord.Length > position)
				return tempWord.Remove(position, 1);
			else
				return tempWord;
		}
		protected virtual string DoInsert(string tempWord, int position, char letter) {
			letter = FormatCharacter(tempWord, position, letter, FormatOperations.InsertOperation);
			return tempWord.Insert(position, letter.ToString());
		}
		protected virtual string DoSubstitute(string tempWord, int position, char letter) {
			letter = FormatCharacter(tempWord, position, letter, FormatOperations.SubstituteOperation);
			return tempWord.Remove(position, 1).Insert(position, letter.ToString());
		}
		protected virtual char FormatCharacter(string tempWord, int position, char letter, FormatOperations operation) {
			if (operation == FormatOperations.SubstituteOperation)
				return CopyFormat(tempWord, position, letter);
			else
				if (operation == FormatOperations.InsertOperation)
					return CalcFormat(tempWord, position, letter);
				else
					throw new Exception("FormatOperations." + operation.ToString() + " is not implemented!");
		}
		protected virtual char CopyFormat(string tempWord, int position, char letter) {
			return (char.IsUpper(tempWord[position]) ? char.ToUpper(letter, Culture) : char.ToLower(letter, Culture));
		}
		protected virtual char CalcFormat(string tempWord, int position, char letter) {
			if(position == 0) {
				if(tempWord.Length > 1)
					position++;
			}
			else
				position--;
			return CopyFormat(tempWord, position, letter);
		}
	}
#endif
	public static class SuggestionsHelper {
		const int MaxDistance = 100;
		const int MaxWordLength = 63; 
		public static SuggestionCollection CalcSuggestions(string word, DictionaryHelper helper, int timeOut, int distance, CultureInfo culture) {
			SuggestionCollection suggestions = new SuggestionCollection();
			if (word.Length <= MaxWordLength) {
				suggestions.AddRange(helper.GetNearMissSuggestions(word, distance, timeOut, culture));
				SuggestionCollection doubleMetaphoneSuggestions = helper.GetDoubleMetaphoneSuggs(word, distance, culture);
				suggestions.Merge(doubleMetaphoneSuggestions);
				if (StringUtils.IsAllCaps(word, culture))
					suggestions.ForEach(suggestion => ToAllUpper(suggestion, culture));
				else if (StringUtils.IsInitCaps(word, culture))
					suggestions.ForEach(suggestion => ToFirstUpper(suggestion, culture));
			}
			return suggestions;
		}
		static void ToFirstUpper(SuggestionBase suggestion, CultureInfo culture) {
			if (suggestion == null || String.IsNullOrEmpty(suggestion.Suggestion))
				return;
			suggestion.Suggestion = StringUtils.MakeInitialCharCaps(suggestion.Suggestion, culture);
		}
		static void ToAllUpper(SuggestionBase suggestion, CultureInfo culture) {
			if (suggestion == null || String.IsNullOrEmpty(suggestion.Suggestion))
				return;
			suggestion.Suggestion = StringUtils.MakeAllCharsCaps(suggestion.Suggestion, culture);
		}
	}
}
