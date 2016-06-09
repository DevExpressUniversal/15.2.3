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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	public enum DigitProcessResult {
		Success,
		Skip,
		End
	}
	#region NumericFieldFormatter
	public class NumericFieldFormatter : SpecificFieldFormatter<double> {
		#region Fields
		const int MaxFormatStringLength = 64;
		const int MaxFractionLength = 10;
		double number;
		NumberFieldFormatInfo formatInfo;
		NumberFormatterStateBase state;
		MailMergeCustomSeparators customSeparators = new MailMergeCustomSeparators();
		#endregion
		public NumericFieldFormatter() {
		}
		#region Properties
		internal double Number { get { return number; } set { number = value; } }
		internal NumberFieldFormatInfo NumberFormatInfo { get { return formatInfo; } set { formatInfo = value; } }
		internal char DecimalSeparator { get { return NumberFormatInfo.DecimalSeparator; } }
		internal string FormatString { get { return NumberFormatInfo.FormatString; } }
		internal MailMergeCustomSeparators CustomSeparators { get { return customSeparators; } }
		#endregion
		protected override string Format(double value, string format) {
			StringBuilder result = new StringBuilder();
			BeginFormat(value, format);
			Format(result);
			EndFormat();
			return result.ToString().TrimEnd(new char[] { ' ' });
		}
		protected override string FormatByDefault(double value) {
			return value.ToString(Culture);
		}
		protected internal void BeginFormat(double number, string format) {
			this.number = number;
			string formatString;
			if (format.Length > MaxFormatStringLength)
				formatString = format.Substring(0, MaxFormatStringLength);
			else
				formatString = format;
			this.formatInfo = new NumberFieldFormatInfo(formatString, Culture.NumberFormat);
			this.formatInfo.CustomSeparators.Assign(this.CustomSeparators);
			PopulateNumberFormatInfo(this.formatInfo);
		}
		protected internal void EndFormat() {
			this.number = 0;
			this.formatInfo = null;
		}
		protected internal virtual void PopulateNumberFormatInfo(NumberFieldFormatInfo formatInfo) {
			NumberFormatParser parser = new NumberFormatParser();
			parser.Parse(formatInfo);
		}
		protected internal virtual void Format(StringBuilder result) {
			NumberPattern pattern = NumberFormatInfo.GetPattern(Number);
			if (pattern.IsEmpty)
				return;
			int roundValue = pattern.GetFractionLength();
			double actualNumber = GetActualNumber(roundValue);
			FormatIntegerPart(actualNumber, pattern, result);
			FormatFractionalPart(actualNumber, pattern, result);
		}
		protected internal virtual double GetActualNumber(int roundValue) {
			int decimals = Math.Min(MaxFractionLength, roundValue);
			return Math.Round(Number, decimals);
		}
		protected virtual internal string GetNumberString(double number) {
			if(String.IsNullOrEmpty(NumberFormatInfo.CustomSeparators.MaskDecimalSeparator))
				return number.ToString(Culture);
			CultureInfo culture = (CultureInfo)Culture.Clone();
			culture.NumberFormat.NumberDecimalSeparator = NumberFormatInfo.CustomSeparators.MaskDecimalSeparator;
			return number.ToString(culture);
		}
		protected virtual void FormatIntegerPart(double number, NumberPattern pattern, StringBuilder result) {
			int separatorIndex = pattern.GetSeparatorIndex();
			int rangeIndex = (separatorIndex < 0) ? pattern.Ranges.Count - 1 : separatorIndex - 1;
			FormatRangeIteratorBackward iterator = new FormatRangeIteratorBackward(pattern, rangeIndex, FormatString);
			FormatIntegerPartCore(number, iterator, result);
		}
		void FormatIntegerPartCore(double number, FormatRangeIteratorBackward iterator, StringBuilder result) {
			state = new IntegerFormatterState(this);
#if !SL
			double integerPart = Math.Truncate(number);
#else
			double integerPart = ((number < 0) ? -1 : 1) * Math.Floor(Math.Abs(number));
#endif
			string numberString = (integerPart != 0.0) ? GetNumberString(Math.Abs(integerPart)) : String.Empty;
			FormatNumberPart(iterator, numberString, result);
		}
		protected virtual void FormatFractionalPart(double number, NumberPattern pattern, StringBuilder result) {
			int separatorIndex = pattern.GetSeparatorIndex();
			if (separatorIndex < 0)
				return;
			FormatRangeIteratorForward iterator = new FormatRangeIteratorForward(pattern, separatorIndex, FormatString);
			FormatFractionalPartCore(number, result, iterator);
		}
		void FormatFractionalPartCore(double number, StringBuilder result, FormatRangeIteratorForward iterator) {
			state = new FractionalFormatterState(this);
			string numberString = GetNumberString(number);
			string[] numberParts = numberString.Split(DecimalSeparator);
			numberString = (numberParts.Length > 1) ? numberParts[1] : String.Empty;
			FormatNumberPart(iterator, numberString, result);
		}
		void FormatNumberPart(FormatRangeIterator iterator, string numberString, StringBuilder result) {
			for (int digitIndex = 0; iterator.MoveNext(); ) {
				if (digitIndex >= numberString.Length && state.CanChangeState)
					state = state.GetNextState();
				DigitProcessResult processResult = state.FormatNextDigit(iterator, digitIndex, numberString, result);
				if (processResult == DigitProcessResult.End)
					state = state.GetNextState();
				else if (processResult == DigitProcessResult.Success)
					digitIndex++;
			}
		}
	}
	#endregion
	#region NumberFormatterStateBase (abstract class)
	public abstract class NumberFormatterStateBase {
		readonly NumericFieldFormatter formatter;
		protected NumberFormatterStateBase(NumericFieldFormatter formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.formatter = formatter;
		}
		public abstract bool CanChangeState { get; }
		protected NumericFieldFormatter Formatter { get { return formatter; } }
		protected double Number { get { return Formatter.Number; } }
		public abstract DigitProcessResult FormatNextDigit(FormatRangeIterator iterator, int index, string number, StringBuilder result);
		public abstract NumberFormatterStateBase GetNextState();
		protected abstract void AppendResult(string str, StringBuilder resultString);
		protected abstract void AppendResult(char ch, StringBuilder resultString);
		protected virtual void AppendResult(NumberFormatRange range, StringBuilder resultString) {
			AppendResult(range.GetText(Formatter.FormatString), resultString);
		}
		protected virtual char GetNumberSign(char patternChar) {
			if (Number > 0 && patternChar == '-' || Number == 0)
				return ' ';
			else if (Number > 0)
				return '+';
			return '-';
		}
		protected virtual char GetPlaceholder(char patternChar) {
			if (patternChar == '#' || patternChar == 'x')
				return ' ';
			return '0';
		}
	}
	#endregion
	#region IntegerFormatterStateBase (abstract class)
	public abstract class IntegerFormatterStateBase : NumberFormatterStateBase {
		protected IntegerFormatterStateBase(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		#region Properties
		protected int GroupSize { get { return Formatter.Culture.NumberFormat.NumberGroupSizes[0]; } }
		protected string GroupSeparator {
			get {
				if(!String.IsNullOrEmpty(Formatter.CustomSeparators.FieldResultGroupSeparator))
					return Formatter.CustomSeparators.FieldResultGroupSeparator;
				return Formatter.Culture.NumberFormat.NumberGroupSeparator;
			}
		}
		#endregion
		protected override void AppendResult(string str, StringBuilder resultString) {
			resultString.Insert(0, str);
		}
		protected override void AppendResult(char ch, StringBuilder resultString) {
			resultString.Insert(0, new char[] { ch });
		}
		protected virtual void AppendSignIfNeeded(NumberPattern pattern, StringBuilder resultString) {
			if (pattern.Type != NumberPatternType.Positive)
				return;
			if (!pattern.HasSign && Number < 0)
				AppendResult('-', resultString);
		}
		protected virtual void AppendGroupSeparator(int digitCount, StringBuilder resultString) {
			if (digitCount == 0 || (digitCount % GroupSize != 0))
				return;
			AppendResult(GroupSeparator, resultString);
		}
	}
	#endregion
	#region IntegerFormatterState
	public class IntegerFormatterState : IntegerFormatterStateBase {
		public IntegerFormatterState(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		public override bool CanChangeState { get { return true; } }
		public override DigitProcessResult FormatNextDigit(FormatRangeIterator iterator, int index, string number, StringBuilder result) {
			int actualIndex = number.Length - index - 1;
			DigitProcessResult processResult = DigitProcessResult.Skip;
			switch (iterator.CurrentRange.Type) {
				case NumberFormatRangeType.StartNumberMask:
					AppendRemainedDigits(actualIndex, number, iterator.Pattern.HasGroupSeparator, result);
					AppendSignIfNeeded(iterator.Pattern, result);
					processResult = DigitProcessResult.End;
					break;
				case NumberFormatRangeType.Terminator:
					AppendResult(number[actualIndex], result);
					processResult = DigitProcessResult.End;
					break;
				case NumberFormatRangeType.NumberMask:
					AppendCurrentDigit(actualIndex, number, iterator.Pattern.HasGroupSeparator, result);
					processResult = DigitProcessResult.Success;
					break;
				case NumberFormatRangeType.GroupSeparator:
					break;
				case NumberFormatRangeType.Text:
					AppendResult(iterator.CurrentRange, result);
					break;
				case NumberFormatRangeType.Sign:
					AppendResult(GetNumberSign(iterator.CurrentCharacter), result);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return processResult;
		}
		public override NumberFormatterStateBase GetNextState() {
			return new FinalIntegerFormatterState(Formatter);
		}
		protected internal virtual void AppendRemainedDigits(int index, string number, bool separateGroups, StringBuilder result) {
			for (int i = index; i >= 0; i--)
				AppendCurrentDigit(i, number, separateGroups, result);
		}
		protected internal virtual void AppendCurrentDigit(int index, string number, bool separateGroups, StringBuilder result) {
			AppendResult(number[index], result);
			if (separateGroups && index != 0) {
				int digitCount = number.Length - index;
				AppendGroupSeparator(digitCount, result);
			}
		}
	}
	#endregion
	#region FinalIntegerFormatterState
	public class FinalIntegerFormatterState : IntegerFormatterStateBase {
		public FinalIntegerFormatterState(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		public override bool CanChangeState { get { return false; } }
		public override DigitProcessResult FormatNextDigit(FormatRangeIterator iterator, int index, string number, StringBuilder result) {
			DigitProcessResult processResult = DigitProcessResult.Skip;
			char ch = iterator.CurrentCharacter;
			switch (iterator.CurrentRange.Type) {
				case NumberFormatRangeType.StartNumberMask:
					AppendResult(GetPlaceholder(ch), result);
					AppendSignIfNeeded(iterator.Pattern, result);
					break;
				case NumberFormatRangeType.NumberMask:
					if (ch == '0' && iterator.Pattern.HasGroupSeparator)
						AppendGroupSeparator(index, result);
					AppendResult(GetPlaceholder(ch), result);
					processResult = DigitProcessResult.Success;
					break;
				case NumberFormatRangeType.Terminator:
					AppendResult(' ', result);
					break;
				case NumberFormatRangeType.GroupSeparator:
					break;
				case NumberFormatRangeType.Text:
					AppendResult(iterator.CurrentRange, result);
					break;
				case NumberFormatRangeType.Sign:
					AppendResult(GetNumberSign(ch), result);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return processResult;
		}
		public override NumberFormatterStateBase GetNextState() {
			Debug.Assert(false);
			return null;
		}
	}
	#endregion
	#region FractionFormatterStateBase (abstract class)
	public abstract class FractionFormatterStateBase : NumberFormatterStateBase {
		protected FractionFormatterStateBase(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		protected char DecimalSeparator {
			get {
				if(!String.IsNullOrEmpty(Formatter.CustomSeparators.FieldResultDecimalSeparator))
					return Formatter.CustomSeparators.FieldResultDecimalSeparator[0];
				return Formatter.Culture.NumberFormat.NumberDecimalSeparator[0];
			}
		}
		protected override void AppendResult(char ch, StringBuilder resultString) {
			resultString.Append(ch);
		}
		protected override void AppendResult(string str, StringBuilder resultString) {
			resultString.Append(str);
		}
	}
	#endregion
	#region FractionalFormatterState
	public class FractionalFormatterState : FractionFormatterStateBase {
		public FractionalFormatterState(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		public override bool CanChangeState { get { return true; } }
		public override DigitProcessResult FormatNextDigit(FormatRangeIterator iterator, int index, string number, StringBuilder result) {
			DigitProcessResult processResult = DigitProcessResult.Skip;
			switch (iterator.CurrentRange.Type) {
				case NumberFormatRangeType.Terminator:
				case NumberFormatRangeType.NumberMask:
					AppendResult(number[index], result);
					processResult = DigitProcessResult.Success;
					break;
				case NumberFormatRangeType.DecimalSeparator:
					AppendResult(DecimalSeparator, result);
					break;
				case NumberFormatRangeType.GroupSeparator:
					break;
				case NumberFormatRangeType.Text:
					AppendResult(iterator.CurrentRange, result);
					break;
				case NumberFormatRangeType.Sign:
					AppendResult(GetNumberSign(iterator.CurrentCharacter), result);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return processResult;
		}
		public override NumberFormatterStateBase GetNextState() {
			return new FinalFractionalFormatterState(Formatter);
		}
	}
	#endregion
	#region FinalFractionalFormatterState
	public class FinalFractionalFormatterState : FractionFormatterStateBase {
		public FinalFractionalFormatterState(NumericFieldFormatter formatter)
			: base(formatter) {
		}
		public override bool CanChangeState { get { return false; } }
		public override DigitProcessResult FormatNextDigit(FormatRangeIterator iterator, int index, string number, StringBuilder result) {
			DigitProcessResult processResult = DigitProcessResult.Skip;
			char ch = iterator.CurrentCharacter;
			switch (iterator.CurrentRange.Type) {
				case NumberFormatRangeType.NumberMask:
					if (ch != 'x')
						AppendResult(GetPlaceholder(ch), result);
					break;
				case NumberFormatRangeType.DecimalSeparator:
					AppendResult(DecimalSeparator, result);
					break;
				case NumberFormatRangeType.GroupSeparator:
					break;
				case NumberFormatRangeType.Terminator:
					break;
				case NumberFormatRangeType.Text:
					AppendResult(iterator.CurrentRange, result);
					break;
				case NumberFormatRangeType.Sign:
					AppendResult(GetNumberSign(ch), result);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return processResult;
		}
		public override NumberFormatterStateBase GetNextState() {
			Debug.Assert(false);
			return null;
		}
	}
	#endregion
	#region FormatRangeIterator (abstract class)
	public abstract class FormatRangeIterator {
		#region Fields
		readonly NumberPattern pattern;
		readonly string format;
		int currentRangeIndex = -1;
		int currentCharacterIndex = -1;
		int previousRangesLength;
		int cachedRangeIndex = -1;
		string cachedRangeText = String.Empty;
		#endregion
		protected FormatRangeIterator(NumberPattern pattern, int rangeIndex, string format) {
			Guard.ArgumentNotNull(pattern, "pattern");
			this.pattern = pattern;
			Guard.ArgumentNonNegative(rangeIndex, "rangeIndex");
			this.currentRangeIndex = rangeIndex;
			if (String.IsNullOrEmpty(format))
				Exceptions.ThrowArgumentException("format", format);
			this.format = format;
		}
		#region Properties
		public NumberPattern Pattern { get { return pattern; } }
		public string FormatString { get { return format; } }
		public NumberFormatRange CurrentRange {
			get {
				if (currentRangeIndex >= 0 && currentRangeIndex < Ranges.Count)
					return Ranges[currentRangeIndex];
				return null;
			}
		}
		public char CurrentCharacter {
			get {
				if (this.cachedRangeIndex != this.currentRangeIndex) {
					this.cachedRangeIndex = this.currentRangeIndex;
					this.cachedRangeText = CurrentRange.GetText(FormatString);
				}
				return GetCurrentCharacter();
			}
		}
		protected NumberFormatRangeCollection Ranges { get { return Pattern.Ranges; } }
		protected internal int CurrentCharacterIndex { get { return currentCharacterIndex; } set { currentRangeIndex = value; } }
		protected internal int CurrentRangeIndex { get { return currentRangeIndex; } set { currentRangeIndex = value; } }
		#endregion
		public virtual bool MoveNext() {
			if (CurrentRange == null)
				return false;
			if (this.currentCharacterIndex >= 0 && IsTextRange(CurrentRange)) {
				this.currentCharacterIndex += CurrentRange.Length;
				return MoveRangeToNext();
			}
			this.currentCharacterIndex++;
			if (IsCharIndexOutOfCurrentRange(this.currentCharacterIndex))
				return MoveRangeToNext();
			else
				return true;
		}
		bool IsTextRange(NumberFormatRange range) {
			return range.Type == NumberFormatRangeType.Text;
		}
		bool IsCharIndexOutOfCurrentRange(int charIndex) {
			return charIndex >= this.previousRangesLength + CurrentRange.Length;
		}
		protected virtual bool MoveRangeToNext() {
			this.previousRangesLength += CurrentRange.Length;
			MoveRangeIndexToNext();
			return (CurrentRange != null) ? true : false;
		}
		protected virtual char GetCurrentCharacter() {
			int logicalCharIndex = GetLogicalCharIndex();
			return this.cachedRangeText[logicalCharIndex];
		}
		protected virtual int GetActualCharIndex() {
			return this.currentCharacterIndex - this.previousRangesLength;
		}
		protected abstract void MoveRangeIndexToNext();
		protected abstract int GetLogicalCharIndex();
	}
	#endregion
	#region FormatRangeIteratorBackward
	public class FormatRangeIteratorBackward : FormatRangeIterator {
		public FormatRangeIteratorBackward(NumberPattern pattern, int rangeIndex, string format)
			: base(pattern, rangeIndex, format) {
		}
		protected override int GetLogicalCharIndex() {
			int actualCharIndex = GetActualCharIndex();
			return (CurrentRange.Length - 1) - actualCharIndex;
		}
		protected override void MoveRangeIndexToNext() {
			CurrentRangeIndex--;
		}
	}
	#endregion
	#region FormatRangeIteratorForward
	public class FormatRangeIteratorForward : FormatRangeIterator {
		public FormatRangeIteratorForward(NumberPattern pattern, int rangeIndex, string format)
			: base(pattern, rangeIndex, format) {
		}
		protected override int GetLogicalCharIndex() {
			return GetActualCharIndex();
		}
		protected override void MoveRangeIndexToNext() {
			CurrentRangeIndex++;
		}
	}
	#endregion
	#region NumberFormatRangeCollection
	public class NumberFormatRangeCollection : List<NumberFormatRange> { }
	#endregion
	#region NumberPatternType
	public enum NumberPatternType {
		Positive,
		Negative,
		Zero
	}
	#endregion
	#region NumberPattern
	public class NumberPattern {
		readonly NumberFormatRangeCollection ranges;
		readonly NumberPatternType type;
		public NumberPattern(NumberPatternType type) {
			this.type = type;
			this.ranges = new NumberFormatRangeCollection();
		}
		public NumberPatternType Type { get { return type; } }
		public NumberFormatRangeCollection Ranges { get { return ranges; } }
		public bool IsEmpty { get { return Ranges.Count == 0; } }
		public bool HasSign {
			get {
				foreach (NumberFormatRange range in Ranges) {
					if (range.Type == NumberFormatRangeType.Sign)
						return true;
				}
				return false;
			}
		}
		public bool HasGroupSeparator {
			get {
				foreach (NumberFormatRange range in Ranges) {
					if (range.Type == NumberFormatRangeType.GroupSeparator)
						return true;
				}
				return false;
			}
		}
		public int GetFractionLength() {
			int result = 0;
			for (int i = this.ranges.Count - 1; i >= 0; i--) {
				NumberFormatRange range = this.ranges[i];
				if (range.Type == NumberFormatRangeType.DecimalSeparator)
					return result;
				if (range.Type == NumberFormatRangeType.NumberMask)
					result += range.Length;
			}
			return 0;
		}
		public int GetSeparatorIndex() {
			int count = Ranges.Count;
			for (int i = 0; i < count; i++) {
				NumberFormatRange range = Ranges[i];
				if (range.Type == NumberFormatRangeType.DecimalSeparator)
					return i;
			}
			return -1;
		}
	}
	#endregion
	#region NumberFieldFormatInfo
	public class NumberFieldFormatInfo {
		#region Fields
		readonly string formatString;
		readonly NumberFormatInfo formatInfo;
		readonly NumberPattern positivePattern;
		readonly NumberPattern negativePattern;
		readonly NumberPattern zeroPattern;
		MailMergeCustomSeparators customSeparators = new MailMergeCustomSeparators();
		NumberPattern currentPattern;
		NumberFormatRange currentRange;
		#endregion
		public NumberFieldFormatInfo(string formatString, NumberFormatInfo formatInfo) {
			if (String.IsNullOrEmpty(formatString))
				Exceptions.ThrowArgumentException("formatString", formatString);
			this.formatString = formatString;
			Guard.ArgumentNotNull(formatInfo, "formatInfo");
			this.formatInfo = formatInfo;
			this.positivePattern = new NumberPattern(NumberPatternType.Positive);
			this.negativePattern = new NumberPattern(NumberPatternType.Negative);
			this.zeroPattern = new NumberPattern(NumberPatternType.Zero);
			this.currentPattern = this.positivePattern;
		}
		#region Properties
		public string FormatString { get { return formatString; } }
		public char DecimalSeparator {
			get {
				if(!String.IsNullOrEmpty(CustomSeparators.MaskDecimalSeparator))
					return CustomSeparators.MaskDecimalSeparator[0];
				return NumberFormat.NumberDecimalSeparator[0];
			}
		}
		public char GroupSeparator {
			get {
				if(!String.IsNullOrEmpty(CustomSeparators.MaskGroupSeparator))
					return CustomSeparators.MaskGroupSeparator[0];
				return NumberFormat.NumberGroupSeparator[0];
			}
		}
		internal MailMergeCustomSeparators CustomSeparators { get { return customSeparators; } }
		public int GroupSize { get { return NumberFormat.NumberGroupSizes[0]; } }
		public NumberPattern PositivePattern { get { return positivePattern; } }
		public NumberPattern NegativePattern { get { return negativePattern; } }
		public NumberPattern ZeroPattern { get { return zeroPattern; } }
		protected internal virtual NumberFormatInfo NumberFormat { get { return formatInfo; } }
		protected internal NumberFormatRange CurrentRange { get { return currentRange; } }
		protected internal NumberPattern CurrentPattern { get { return currentPattern; } }
		#endregion
		public NumberPattern GetPattern(double value) {
			if (value < 0 && !NegativePattern.IsEmpty)
				return NegativePattern;
			if (value == 0 && !ZeroPattern.IsEmpty)
				return ZeroPattern;
			return PositivePattern;
		}
		protected internal void AddToRange(int charIndex, NumberFormatRangeType rangeType) {
			if (this.currentRange != null && this.currentRange.Type == rangeType)
				this.currentRange.EndIndex = charIndex;
			else {
				this.currentRange = new NumberFormatRange(charIndex, rangeType);
				this.currentPattern.Ranges.Add(this.currentRange);
			}
		}
		protected internal bool ChangeCurrentPatternToNext() {
			ResetRangeMerging();
			if (this.currentPattern.Type == NumberPatternType.Positive) {
				this.currentPattern = NegativePattern;
				return true;
			}
			if (this.currentPattern.Type == NumberPatternType.Negative) {
				this.currentPattern = ZeroPattern;
				return true;
			}
			return false;
		}
		protected internal void ResetRangeMerging() {
			this.currentRange = null;
		}
	}
	#endregion
	#region NumberFormatRangeType
	public enum NumberFormatRangeType {
		None,
		Text,
		Sign,
		StartNumberMask,
		NumberMask,
		DecimalSeparator,
		GroupSeparator,
		Terminator
	}
	#endregion
	#region NumberFormatRange
	public class NumberFormatRange {
		int startIndex = -1;
		int endIndex = -1;
		NumberFormatRangeType type = NumberFormatRangeType.None;
		public NumberFormatRange() {
		}
		public NumberFormatRange(NumberFormatRangeType type) {
			this.type = type;
		}
		public NumberFormatRange(int index, NumberFormatRangeType type)
			: this(index, index, type) {
		}
		public NumberFormatRange(int startIndex, int endIndex, NumberFormatRangeType type) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.type = type;
		}
		public int StartIndex { get { return startIndex; } set { endIndex = value; } }
		public int EndIndex {
			get { return endIndex; }
			set {
				if (endIndex < startIndex)
					Exceptions.ThrowInternalException();
				endIndex = value;
			}
		}
		public NumberFormatRangeType Type { get { return type; } set { type = value; } }
		public int Length { get { return endIndex - startIndex + 1; } }
		public string GetText(string format) {
			return format.Substring(StartIndex, Length);
		}
	}
	#endregion
	#region NumberFormatParserState
	public enum NumberFormatParserState {
		Start,
		IntegerPart,
		FractionPart,
		EmbedText,
		Finish
	}
	#endregion
	#region NumberFormatParser
	public class NumberFormatParser {
		NumberFormatParserStateBase state;
		public NumberFormatParser() {
			this.state = new StartPatternState(this);
		}
		protected internal NumberFormatParserStateBase State { get { return state; } }
		public void Parse(NumberFieldFormatInfo formatInfo) {
			string formatString = formatInfo.FormatString;
			if (String.IsNullOrEmpty(formatString))
				return;
			int length = formatString.Length;
			for (int i = 0; i < length; i++) {
				if (State.Type == NumberFormatParserState.Finish)
					break;
				State.ProcessChar(i, formatInfo);
			}
		}
		public void ChangeState(NumberFormatParserStateBase state) {
			this.state = state;
		}
		public void ChangeState(NumberFormatParserState type) {
			switch (type) {
				case NumberFormatParserState.IntegerPart:
					ChangeState(new IntegerPartState(this));
					break;
				case NumberFormatParserState.FractionPart:
					ChangeState(new FractionPartState(this));
					break;
				case NumberFormatParserState.Start:
					ChangeState(new StartPatternState(this));
					break;
				case NumberFormatParserState.Finish:
					ChangeState(new FinishState(this));
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
	}
	#endregion
	#region NumberFormatParserStateBase (abstract class)
	public abstract class NumberFormatParserStateBase {
		readonly NumberFormatParser calculator;
		protected NumberFormatParserStateBase(NumberFormatParser formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.calculator = formatter;
		}
		public virtual NumberFormatParser Calculator { get { return calculator; } }
		public abstract NumberFormatParserState Type { get; }
		public abstract void ProcessChar(int index, NumberFieldFormatInfo formatInfo);
		protected void ChangeState(NumberFormatParserStateBase state) {
			Calculator.ChangeState(state);
		}
		protected void ChangeState(NumberFormatParserState type) {
			Calculator.ChangeState(type);
		}
	}
	#endregion
	#region NumberPatternStateBase (abstract class)
	public abstract class NumberPatternStateBase : NumberFormatParserStateBase {
		protected NumberPatternStateBase(NumberFormatParser calculator)
			: base(calculator) {
		}
		public override void ProcessChar(int index, NumberFieldFormatInfo formatInfo) {
			if (!ProcessCharStateSpecific(index, formatInfo))
				ProcessCharStateCommon(index, formatInfo);
		}
		protected abstract bool ProcessCharStateSpecific(int index, NumberFieldFormatInfo formatInfo);
		protected void ProcessCharStateCommon(int index, NumberFieldFormatInfo formatInfo) {
			char ch = formatInfo.FormatString[index];
			if (IsGroupSeparator(ch, formatInfo)) {
				formatInfo.AddToRange(index, NumberFormatRangeType.GroupSeparator);
				return;
			}
			switch (ch) {
				case '+':
				case '-':
					formatInfo.ResetRangeMerging();
					formatInfo.AddToRange(index, NumberFormatRangeType.Sign);
					break;
				case '\'':
					ChangeState(new EmbedTextState(Calculator, this));
					break;
				case '\"':
					ProcessDoubleQuotesChar(index, formatInfo);
					break;
				case '\\':
					ProcessBackslashChar(index, formatInfo);
					break;
				default:
					formatInfo.AddToRange(index, NumberFormatRangeType.Text);
					break;
			}
		}
		protected void ProcessDoubleQuotesChar(int index, NumberFieldFormatInfo formatInfo) {
			int prevCharIndex = index - 1;
			if (prevCharIndex >= 0 && formatInfo.FormatString[prevCharIndex] == '\\') {
				formatInfo.ResetRangeMerging();
				formatInfo.AddToRange(index, NumberFormatRangeType.Text);
			}
			else
				NumericFieldFormatter.ThrowSyntaxError("\"");
		}
		protected void ProcessBackslashChar(int index, NumberFieldFormatInfo formatInfo) {
			int prevCharIndex = index;
			while (prevCharIndex >= 0 && formatInfo.FormatString[prevCharIndex] == '\\')
				prevCharIndex--;
			if ((index - prevCharIndex) % 2 == 0)
				formatInfo.AddToRange(index, NumberFormatRangeType.Text);
			else
				formatInfo.ResetRangeMerging();
		}
		protected bool IsDecimalSeparator(char ch, NumberFieldFormatInfo formatInfo) {
			return ch == formatInfo.DecimalSeparator;
		}
		protected bool IsGroupSeparator(char ch, NumberFieldFormatInfo formatInfo) {
			return ch == formatInfo.GroupSeparator;
		}
	}
	#endregion
	#region StartPatternState
	public class StartPatternState : NumberPatternStateBase {
		public StartPatternState(NumberFormatParser formatter)
			: base(formatter) {
		}
		public override NumberFormatParserState Type { get { return NumberFormatParserState.Start; } }
		protected override bool ProcessCharStateSpecific(int index, NumberFieldFormatInfo formatInfo) {
			char ch = formatInfo.FormatString[index];
			if (IsDecimalSeparator(ch, formatInfo)) {
				formatInfo.AddToRange(index, NumberFormatRangeType.DecimalSeparator);
				ChangeState(NumberFormatParserState.FractionPart);
				return true;
			}
			if (ch == 'x') {
				formatInfo.AddToRange(index, NumberFormatRangeType.Terminator);
				ChangeState(NumberFormatParserState.IntegerPart);
				return true;
			}
			if (ch == '#' || ch == '0') {
				formatInfo.AddToRange(index, NumberFormatRangeType.StartNumberMask);
				ChangeState(NumberFormatParserState.IntegerPart);
				return true;
			}
			if (ch == ';') {
				if (!formatInfo.ChangeCurrentPatternToNext())
					ChangeState(NumberFormatParserState.Finish);
				return true;
			}
			return false;
		}
	}
	#endregion
	#region NumberMaskStateBase (abstract class)
	public abstract class NumberMaskStateBase : NumberPatternStateBase {
		protected NumberMaskStateBase(NumberFormatParser calculator)
			: base(calculator) {
		}
		protected override bool ProcessCharStateSpecific(int index, NumberFieldFormatInfo formatInfo) {
			char ch = formatInfo.FormatString[index];
			if (ch == 'x' || ch == '#' || ch == '0') {
				formatInfo.AddToRange(index, NumberFormatRangeType.NumberMask);
				return true;
			}
			if (ch == ';') {
				if (formatInfo.ChangeCurrentPatternToNext())
					ChangeState(NumberFormatParserState.Start);
				else
					ChangeState(NumberFormatParserState.Finish);
				return true;
			}
			return false;
		}
	}
	#endregion
	#region IntegerPartState
	public class IntegerPartState : NumberMaskStateBase {
		public IntegerPartState(NumberFormatParser formatter)
			: base(formatter) {
		}
		public override NumberFormatParserState Type { get { return NumberFormatParserState.IntegerPart; } }
		protected override bool ProcessCharStateSpecific(int index, NumberFieldFormatInfo formatInfo) {
			if (IsDecimalSeparator(formatInfo.FormatString[index], formatInfo)) {
				formatInfo.AddToRange(index, NumberFormatRangeType.DecimalSeparator);
				ChangeState(NumberFormatParserState.FractionPart);
				return true;
			}
			return base.ProcessCharStateSpecific(index, formatInfo);
		}
	}
	#endregion
	#region FractionPartState
	public class FractionPartState : NumberMaskStateBase {
		public FractionPartState(NumberFormatParser formatter)
			: base(formatter) {
		}
		public override NumberFormatParserState Type { get { return NumberFormatParserState.FractionPart; } }
	}
	#endregion
	#region EmbedTextState
	public class EmbedTextState : NumberFormatParserStateBase {
		readonly NumberFormatParserStateBase prevState;
		public EmbedTextState(NumberFormatParser calculator, NumberFormatParserStateBase prevState)
			: base(calculator) {
			this.prevState = prevState;
		}
		public override NumberFormatParserState Type { get { return NumberFormatParserState.EmbedText; } }
		public override void ProcessChar(int index, NumberFieldFormatInfo formatInfo) {
			string formatString = formatInfo.FormatString;
			if (formatString[index] == '\'') {
				formatInfo.ResetRangeMerging();
				ChangeState(prevState);
				return;
			}
			if (index == formatString.Length - 1)
				FieldFormatter.ThrowUnmatchedQuotesError();
			formatInfo.AddToRange(index, NumberFormatRangeType.Text);
		}
	}
	#endregion
	#region FinishState
	public class FinishState : NumberFormatParserStateBase {
		public FinishState(NumberFormatParser calculator)
			: base(calculator) {
		}
		public override NumberFormatParserState Type { get { return NumberFormatParserState.Finish; } }
		public override void ProcessChar(int index, NumberFieldFormatInfo formatInfo) {
			Debug.Assert(false);
		}
	}
	#endregion
}
