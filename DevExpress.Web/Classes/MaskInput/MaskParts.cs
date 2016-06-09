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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	#region mask parts
	public interface IMaskPart {
		bool IsValid { get; }
		string DefaultValue { get; }
		string StringValue { get; }
		MaskBase Owner { get; }
		MaskValidationResult LastValidationResult { get; }
		bool CanSkip();
		ModifyAction GetModifyAction();
		int GetPartLength();
		MaskValidationResult TryValidate(string input, int startPosition, int endPosition, ModifyAction modifier);
		void SetIncludeLiteralsMode(MaskIncludeLiteralsMode includeLiterals);
	}
	public class MaskPartCollection {
		public IMaskPart this[int index] {
			get { return Items[index]; }
			set { Items[index] = value; }
		}
		protected Dictionary<int, IMaskPart> Dictionary { get; set; }
		private List<IMaskPart> items = null;
		protected List<IMaskPart> Items {
			get {
				if(items == null)
					items = Dictionary.OrderBy(x => x.Key).Select(x => x.Value).ToList();
				return items;
			}
		}
		public MaskPartCollection() {
			Dictionary = new Dictionary<int, IMaskPart>();
		}
		public MaskPartCollection(IEnumerable<IMaskPart> collection)
			: base() {
			int key = 0;
			Dictionary = collection.ToDictionary<IMaskPart, int>(part => key++);
		}
		public void Insert(int index, string pattern, string value, MaskBase maskBase) {
			items = null;
			Dictionary.Add(index, MaskPartBuilder.GetMaskPart(pattern, value, maskBase));
		}
		public int IndexOf(IMaskPart part) {
			return Items.IndexOf(part);
		}
		public int Count(Func<IMaskPart, bool> filter = null) {
			return Items.Count(i => filter == null || filter(i));
		}
		public IEnumerable<IMaskPart> Where(Func<IMaskPart, bool> filter) {
			return Items.Where(filter);
		}
		public IEnumerable<T> OfType<T>(Func<T, bool> additionalFilter = null) where T : IMaskPart {
			return Items.Where(i => i.GetType() == typeof(T) && (additionalFilter == null || additionalFilter((T)i))).Select(i => (T)i);
		}
		public MaskPartCollection SetIncludeLiterals(MaskIncludeLiteralsMode includeLiterals) {
			foreach(IMaskPart part in Items)
				part.SetIncludeLiteralsMode(includeLiterals);
			return this;
		}
	}
	public abstract class MaskPartBase : IMaskPart {
		public bool IsValid { get; protected set; }
		public string DefaultValue { get; protected set; }
		public string StringValue { get; protected set; }
		public MaskBase Owner { get; private set; }
		public MaskValidationResult LastValidationResult { get; protected set; }
		protected MaskIncludeLiteralsMode IncludeLiteralsMode { get; private set; }
		public MaskPartBase(string maskStringView, MaskBase mask) {
			Owner = mask;
			StringValue = maskStringView;
			IsValid = true;
			DefaultValue = StringValue;
		}
		public virtual ModifyAction GetModifyAction() {
			return ModifyAction.None;
		}
		public virtual bool CanSkip() {
			return false;
		}
		public virtual int GetPartLength() {
			return 0;
		}
		public MaskValidationResult TryValidate(string input, int start, int end, ModifyAction modifier) {
			LastValidationResult = Validate(input, start, end, modifier);
			LastValidationResult.CheckedValue = GetSubstring(input, start, LastValidationResult.EndPosition);
			LastValidationResult.CheckModifier = modifier;
			return LastValidationResult;
		}
		public void SetIncludeLiteralsMode(MaskIncludeLiteralsMode includeLiterals) {
			IncludeLiteralsMode = includeLiterals;
		}
		protected virtual MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			return MaskValidationResult.CreateSuccess(end, DefaultValue);
		}
		protected string GetSubstring(string s, int start, int end, Func<char, bool> takeWhileFilter = null) {
			IEnumerable<char> result = s.Skip(start).Take(end - start);
			if(takeWhileFilter != null)
				result = result.TakeWhile(takeWhileFilter);
			return string.Join(string.Empty, result);
		}
		public override string ToString() {
			return string.Format("{0} {1}", this.GetType().Name, StringValue);
		}
	}
	public class MaskEnumPart : MaskPartBase {
		public List<string> Options { get; protected set; }
		protected const string Delimeter = "|";
		public MaskEnumPart(string maskView, MaskBase mask)
			: base(maskView, mask) {
			DefaultValue = null;
			Options = new List<string>();
			IsValid = true;
			string clearStringView = StringValue.Remove(StringValue.Length - 1, 1).Remove(0, 1);
			Options = clearStringView.Split(new string[] { Delimeter }, StringSplitOptions.None).Select(value => {
				if(value[0] == '*') {
					if(!string.IsNullOrEmpty(DefaultValue))
						IsValid = false;
					DefaultValue = value.Remove(0, 1);
					return DefaultValue;
				}
				return value.Replace(@"\", "");
			}).ToList();
		}
		public override int GetPartLength() {
			return Options.Max(x => x.Length);
		}
		protected override MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			string valueToCheck = GetSubstring(input, start, end);
			string matchedOption = Options.OrderByDescending(o => o.Length).FirstOrDefault(o => valueToCheck.StartsWith(o));
			if(!string.IsNullOrEmpty(matchedOption))
				return MaskValidationResult.CreateSuccess(start + matchedOption.Length, matchedOption);
			return MaskValidationResult.CreateFailed(end, DefaultValue);
		}
	}
	public class MaskRangePart : MaskPartBase {
		private const string GroupParseRegexpPattern = @"[{0}]?(?<group>\d+)";
		public Int64 MinValue { get; private set; }
		public Int64 MaxValue { get; private set; }
		protected bool IsZeroFilling { get; private set; }
		protected bool HasGroupSeparator { get; private set; }
		protected bool CanContainGroupSeparator {
			get { return HasGroupSeparator && IncludeLiteralsMode == MaskIncludeLiteralsMode.All; }
		}
		public MaskRangePart(string maskView, MaskBase mask)
			: base(maskView, mask) {
			Match match = Regex.Match(StringValue, MaskPartConstants.MaskRangePartRegexp);
			IsValid = true;
			Int64 minValue;
			string minValStr = match.Groups["num1"].Value;
			if(!Int64.TryParse(minValStr, out minValue))
				IsValid = false;
			MinValue = minValue;
			IsZeroFilling = MinValue.ToString().Length < minValStr.Length;
			Int64 maxValue;
			string maxValStr = match.Groups["num3"].Success ? match.Groups["num3"].Value : match.Groups["num2"].Value;
			if(!Int64.TryParse(maxValStr, out maxValue))
				IsValid = false;
			MaxValue = maxValue;
			if(match.Groups["num3"].Success) {
				double defaulValue;
				if(!Double.TryParse(match.Groups["num2"].Value, out defaulValue) || (defaulValue > MaxValue || defaulValue < minValue))
					IsValid = false;
				DefaultValue = defaulValue.ToString();
			}
			HasGroupSeparator = match.Groups["separator"].Success;
		}
		public override int GetPartLength() {
			int result = Math.Max(MaxValue.ToString().Length, MinValue.ToString().Length);
			if(CanContainGroupSeparator) {
				int additionalLength = 0;
				int[] sizes = Owner.NumberGroupSizes;
				if(!(sizes.Length == 1 && sizes[0] == 0)) {
					int separatorSize = Owner.MaskGroupNumSeparator.Length;
					int maxLength = Math.Max(Math.Abs(MinValue), Math.Abs(MaxValue)).ToString().Length;
					int currentLength = 0;
					int index = 0;
					while((currentLength + sizes[index]) < maxLength) {
						additionalLength += separatorSize;
						currentLength += sizes[index];
						if(sizes.Length > (index + 1))
							index++;
					}
				}
				result += additionalLength;
			}
			return result;
		}
		protected override MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			List<char> allowedCharacters = new List<char>(new char[] { MaskPartConstants.MinusSymbol, MaskPartConstants.PlusSymbol });
			if(CanContainGroupSeparator)
				allowedCharacters.AddRange(Owner.MaskGroupNumSeparator.ToArray());
			string valueToCheck = GetSubstring(input, start, end, c => char.IsDigit(c) || allowedCharacters.Contains(c));
			if(IsValueValid(valueToCheck) && (!IsZeroFilling || valueToCheck.Length == GetPartLength()))
				return MaskValidationResult.CreateSuccess(start + valueToCheck.Length, valueToCheck);
			return MaskValidationResult.CreateFailed(end, DefaultValue);
		}
		private bool IsValueValid(string value) {
			if(!string.IsNullOrEmpty(Owner.MaskGroupNumSeparator)) {
				if(IncludeLiteralsMode == MaskIncludeLiteralsMode.All) {
					if(!IsSeparatedByGroupsCorrected(value))
						return false;
					value = value.Replace(Owner.MaskGroupNumSeparator, "");
				} else if(value.Contains(Owner.MaskGroupNumSeparator))
					return false;
			}
			Int64 parsedValue = 0;
			return (Int64.TryParse(value, out parsedValue) && parsedValue >= MinValue && parsedValue <= MaxValue);
		}
		private bool IsSeparatedByGroupsCorrected(string value) {
			if(!HasGroupSeparator)
				return true;
			int[] sizes = Owner.NumberGroupSizes;
			if(sizes.Length == 0 || sizes[0] == 0)
				return true;
			MatchCollection matches = Regex.Matches(value, string.Format(GroupParseRegexpPattern, Owner.MaskGroupNumSeparator));
			int sizeIndex = 0;
			for(int i = matches.Count - 1; i >= 0; i--) {
				string groupVal = matches[i].Groups["group"].Value;
				if(groupVal.Length != sizes[sizeIndex] && (i != 0 || groupVal.Length > sizes[sizeIndex]))
					return false;
				if(sizes.Length > sizeIndex + 1)
					sizeIndex++;
			}
			return true;
		}
	}
	public class MaskDateTimePart : MaskPartBase {
		public MaskDateTimeComponentType DateTimeComponentType {
			get {
				switch(StringValue) {
					case "y":
					case "yy":
					case "yyy":
					case "yyyy":
						return MaskDateTimeComponentType.Year;
					case "M":
					case "MM":
					case "MMM":
					case "MMMM":
						return MaskDateTimeComponentType.Month;
					case "d":
					case "dd":
						return MaskDateTimeComponentType.Day;
					case "ddd":
					case "dddd":
						return MaskDateTimeComponentType.DayOfWeek;
					case "H":
					case "HH":
					case "h":
					case "hh":
						return MaskDateTimeComponentType.Hour;
					case "mm":
					case "m":
						return MaskDateTimeComponentType.Minute;
					case "ss":
					case "s":
						return MaskDateTimeComponentType.Second;
					case "t":
					case "tt":
						return MaskDateTimeComponentType.AMPM;
					case "f":
					case "ff":
					case "fff":
					case "ffff":
					case "fffff":
					case "ffffff":
					case "F":
					case "FF":
					case "FFF":
					case "FFFF":
					case "FFFFF":
					case "FFFFFF":
						return MaskDateTimeComponentType.Millisecond;
					default:
						throw new NotSupportedException(string.Format("'{0}' datetime format is not supported", StringValue));
				}
			}
		}
		protected IMaskPart MaskPart { get; private set; }
		public MaskDateTimePart(string maskView, IMaskPart maskPart)
			: base(maskView, maskPart.Owner) {
			MaskPart = maskPart;
		}
		public override ModifyAction GetModifyAction() {
			return MaskPart.GetModifyAction();
		}
		public override int GetPartLength() {
			return MaskPart.GetPartLength();
		}
		public Calendar GetCalendar() {
			return Owner.Calendar;
		}
		public bool HasSameValue(MaskDateTimePart comparingPart) {
			if(LastValidationResult == null || comparingPart == null || LastValidationResult == null || comparingPart.LastValidationResult == null ||
				DateTimeComponentType != comparingPart.DateTimeComponentType)
				return false;
			string value = LastValidationResult.CheckedValue;
			string comparingValue = comparingPart.LastValidationResult.CheckedValue;
			if(MaskPart is MaskRangePart && comparingPart.MaskPart is MaskRangePart) {
				int intValue = Int32.Parse(value);
				int intComparingValue = Int32.Parse(comparingValue);
				if(DateTimeComponentType == MaskDateTimeComponentType.Year) {
					int minLength = Math.Min(comparingValue.Length, value.Length);
					intValue = (int)(intValue % Math.Pow(10, minLength));
					intComparingValue = (int)(intComparingValue % Math.Pow(10, minLength));
				} else if(DateTimeComponentType == MaskDateTimeComponentType.Millisecond) {
					if(comparingValue.Length > value.Length)
						intComparingValue = (int)Math.Round(intComparingValue / Math.Pow(10, comparingValue.Length - value.Length));
					else if(value.Length > comparingValue.Length)
						intValue = (int)Math.Round(intValue / Math.Pow(10, value.Length - comparingValue.Length));
				} else if(DateTimeComponentType == MaskDateTimeComponentType.Hour) {
					MaskRangePart mask = MaskPart as MaskRangePart;
					MaskRangePart comparingMask = comparingPart.MaskPart as MaskRangePart;
					if(mask.MaxValue > comparingMask.MaxValue && intValue > comparingMask.MaxValue)
						intValue -= (int)comparingMask.MaxValue;
					if(comparingMask.MaxValue > mask.MaxValue && intComparingValue > mask.MaxValue)
						intComparingValue -= (int)mask.MaxValue;
				}
				return intValue == intComparingValue;
			} else if(MaskPart is MaskEnumPart && comparingPart.MaskPart is MaskEnumPart) {
				return (MaskPart as MaskEnumPart).Options.IndexOf(value) == (comparingPart.MaskPart as MaskEnumPart).Options.IndexOf(comparingValue);
			} else if(!(MaskPart is MaskStaticCharPart || comparingPart.MaskPart is MaskStaticCharPart)) {
				MaskEnumPart enumPart = (MaskPart as MaskEnumPart) ?? (comparingPart.MaskPart as MaskEnumPart);
				MaskRangePart rangePart = (MaskPart as MaskRangePart) ?? (comparingPart.MaskPart as MaskRangePart);
				return enumPart.Options.IndexOf(enumPart.LastValidationResult.CheckedValue) == (Int32.Parse(rangePart.LastValidationResult.CheckedValue)) - 1;
			} else
				return StringValue == comparingPart.StringValue && value == comparingValue;
		}
		protected override MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			return MaskPart.TryValidate(input, start, end, modifier);
		}
	}
	public static class MaskDateTimePartExtensions {
		public static int GetYear(this MaskDateTimePart part) {
			int year = MaskPartConstants.FirstLeapYear;
			if(HasValue(part, MaskDateTimeComponentType.Year)) {
				year = Int32.Parse(part.LastValidationResult.CheckedValue);
				if(year < 100) {
					year += 1900;
					if(year + 99 < part.GetCalendar().TwoDigitYearMax)
						year += 100;
				}
			}
			return year;
		}
		public static int GetMonth(this MaskDateTimePart part) {
			int month = 1;
			if(!HasValue(part, MaskDateTimeComponentType.Month))
				return month;
			string value = part.LastValidationResult.CheckedValue;
			if(!Int32.TryParse(value, out month))
				month = GetIndexFromMultipleSources(value, part.Owner.MonthNames, part.Owner.AbbrMonthNames) + 1;
			return month;
		}
		public static int GetDay(this MaskDateTimePart part) {
			int day = 1;
			if(!HasValue(part, MaskDateTimeComponentType.Day))
				return day;
			Int32.TryParse(part.LastValidationResult.CheckedValue, out day);
			return day;
		}
		public static bool IsPresentedDateValid(this MaskPartCollection maskParts) {
			IEnumerable<MaskDateTimePart> dateTimeParts = maskParts.OfType<MaskDateTimePart>();
			if(!dateTimeParts.Any())
				return true;
			List<MaskDateTimePart> valuableParts = new List<MaskDateTimePart>();
			foreach(MaskDateTimeComponentType enumValue in (MaskDateTimeComponentType[])Enum.GetValues(typeof(MaskDateTimeComponentType))) {
				IEnumerable<MaskDateTimePart> tmpParts = dateTimeParts.Where(p => p.DateTimeComponentType == enumValue);
				if(tmpParts.Any()) {
					MaskDateTimePart lastPart = tmpParts.Last();
					if(tmpParts.Any(p => !p.HasSameValue(lastPart)))
						return false;
					valuableParts.Add(lastPart);
				}
			}
			MaskDateTimePart year = valuableParts.FirstOrDefault(p => p.DateTimeComponentType == MaskDateTimeComponentType.Year);
			MaskDateTimePart month = valuableParts.FirstOrDefault(p => p.DateTimeComponentType == MaskDateTimeComponentType.Month);
			MaskDateTimePart day = valuableParts.FirstOrDefault(p => p.DateTimeComponentType == MaskDateTimeComponentType.Day);
			MaskDateTimePart dayOfWeek = valuableParts.FirstOrDefault(p => p.DateTimeComponentType == MaskDateTimeComponentType.DayOfWeek);
			bool isValidDay = DateTime.DaysInMonth(year.GetYear(), month.GetMonth()) >= day.GetDay();
			if(isValidDay && dayOfWeek != null && year != null && day != null && month != null) {
				DateTime date = new DateTime(year.GetYear(), month.GetMonth(), day.GetDay(), day.GetCalendar());
				return date.ToString(dayOfWeek.StringValue) == dayOfWeek.LastValidationResult.CheckedValue;
			}
			return isValidDay;
		}
		private static bool HasValue(MaskDateTimePart part, MaskDateTimeComponentType type) {
			return part != null && part.LastValidationResult != null && part.DateTimeComponentType == type;
		}
		private static int GetIndexFromMultipleSources(string value, params string[][] sources) {
			foreach(string[] source in sources) {
				if(source.Contains(value))
					return Array.IndexOf(source, value);
			}
			return -1;
		}
	}
	public class MaskPromtPart : MaskPartBase {
		public MaskPromtPart(string maskView, MaskBase mask)
			: base(maskView, mask) {
		}
		protected MaskPromtCharType MaskPromtCharType {
			get {
				switch(StringValue.ToLower()) {
					case MaskPartConstants.SignumPattern:
						return MaskPromtCharType.Signum;
					case MaskPartConstants.RequiredDigitPattern:
						return MaskPromtCharType.RequiredDigit;
					case MaskPartConstants.OptionalDigitPattern:
						return MaskPromtCharType.OptionalDigit;
					case MaskPartConstants.AlphanumericPattern:
						return MaskPromtCharType.Alphanumeric;
					case MaskPartConstants.AlphabeticPattern:
						return MaskPromtCharType.Alphabetic;
					case MaskPartConstants.AnyCharPattern:
					default:
						return MaskPromtCharType.Anychar;
				}
			}
		}
		public override int GetPartLength() {
			return 1;
		}
		protected override MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			char? valueToCheck = GetSubstring(input, start, end).Select(c => (char?)c).FirstOrDefault();
			if(valueToCheck.HasValue) {
				int newPosition = GetCharacterPosition(valueToCheck.Value, end, modifier, MaskPromtCharType);
				if(newPosition > -1)
					return MaskValidationResult.CreateSuccess(newPosition, valueToCheck.Value.ToString());
			} else if(MaskPromtCharType != MaskPromtCharType.RequiredDigit)
				return MaskValidationResult.CreateSuccess(end, "");
			return MaskValidationResult.CreateFailed(end, valueToCheck.GetValueOrDefault().ToString());
		}
		private int GetCharacterPosition(char valueToCheck, int end, ModifyAction modifier, MaskPromtCharType type) {
			switch(type) {
				case MaskPromtCharType.Alphabetic:
				case MaskPromtCharType.Alphanumeric:
				case MaskPromtCharType.Anychar:
					return IsValidCharacter(valueToCheck, modifier) ? end : -1;
				case MaskPromtCharType.Signum:
					return (valueToCheck == MaskPartConstants.PlusSymbol || valueToCheck == MaskPartConstants.MinusSymbol) ?
						end : GetCharacterPosition(valueToCheck, end, modifier, MaskPromtCharType.OptionalDigit);
				case MaskPromtCharType.OptionalDigit:
					return char.IsDigit(valueToCheck) || char.IsWhiteSpace(valueToCheck) ? end : -1;
				case MaskPromtCharType.RequiredDigit:
					return char.IsDigit(valueToCheck) ? end : -1;
				default:
					return end;
			}
		}
		private bool IsValidCharacter(char c, ModifyAction modifier) {
			bool isCaseMatched = !char.IsLetter(c) ||
				((modifier != ModifyAction.ToUpperCase || char.IsUpper(c)) && (modifier != ModifyAction.ToLowerCase || char.IsLower(c)));
			if(MaskPromtCharType == MaskPromtCharType.Alphabetic)
				return (char.IsLetter(c) || char.IsWhiteSpace(c)) && isCaseMatched;
			else if(MaskPromtCharType == MaskPromtCharType.Alphanumeric)
				return (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)) && isCaseMatched;
			return isCaseMatched;
		}
	}
	public class MaskStaticCharPart : MaskPartBase {
		public MaskStaticCharPart(string maskView, MaskBase mask)
			: base(maskView, mask) {
			StringValue = GetCorrectedStringView(maskView);
		}
		public override bool CanSkip() {
			switch(IncludeLiteralsMode) {
				case MaskIncludeLiteralsMode.None:
					return true;
				case MaskIncludeLiteralsMode.DecimalSymbol:
					return StringValue != Owner.MaskDecimalPoint;
				default:
					return false;
			}
		}
		public override int GetPartLength() {
			return StringValue.Length;
		}
		protected override MaskValidationResult Validate(string input, int start, int end, ModifyAction modifier) {
			string valueToCheck = GetSubstring(input, start, end);
			if(StringValue == valueToCheck)
				return MaskValidationResult.CreateSuccess(end, valueToCheck);
			return MaskValidationResult.CreateFailed(end, valueToCheck);
		}
		protected virtual string GetCorrectedStringView(string stringView) {
			switch(stringView) {
				case MaskPartConstants.DecimalPointPattern:
					return Owner.MaskDecimalPoint;
				case MaskPartConstants.NumGroupSeparatorPattern:
					return Owner.MaskGroupNumSeparator;
				case MaskPartConstants.CurrencyPattern:
					return Owner.MaskCurrencySymbol;
				case MaskPartConstants.TimeSeparatorPattern:
					return Owner.MaskTimeSeparator;
				case MaskPartConstants.DateSeparatorPattern:
					return Owner.MaskDateSeparator;
				default:
					return stringView;
			}
		}
	}
	public class MaskStaticCultureIndependentCharPart : MaskStaticCharPart {
		public MaskStaticCultureIndependentCharPart(string maskView, MaskBase mask)
			: base(maskView, mask) {
		}
		protected override string GetCorrectedStringView(string stringView) {
			return stringView.StartsWith(@"\") ? stringView.Substring(1) : stringView;
		}
		public static IEnumerable<MaskStaticCultureIndependentCharPart> Create(IMaskPart maskPart) {
			foreach(char c in maskPart.StringValue)
				yield return new MaskStaticCultureIndependentCharPart(c.ToString(), maskPart.Owner);
		}
	}
	public class MaskQuotePart : MaskPartBase {
		public bool IsQuotePart { get { return StringValue == "'" || StringValue == "\""; } }
		public MaskQuotePart(string maskView, MaskBase mask)
			: base(maskView, mask) {
		}
		public override bool CanSkip() {
			return true;
		}
		public override int GetPartLength() {
			return 0;
		}
	}
	public class MaskCharModifierPart : MaskPartBase {
		public MaskCharModifierPart(string maskView, MaskBase mask)
			: base(maskView, mask) { }
		public override ModifyAction GetModifyAction() {
			switch(StringValue) {
				case MaskPartConstants.StopModifyPattern:
					return ModifyAction.StopModify;
				case MaskPartConstants.ToLowerCasePattern:
					return ModifyAction.ToLowerCase;
				case MaskPartConstants.ToUpperCasePattern:
					return ModifyAction.ToUpperCase;
				default:
					return ModifyAction.None;
			}
		}
	}
	#endregion
	#region additional entities
	public static class MaskPartConstants {
		public const string TokenRemoveRegexpPattern = @"[\u0000]";
		public const string DatePartPatchRegexpPattern = @"((?<=(([^\\]|^)([\\](\\\\)*)))(y{1,4}|M{1,4}|d{1,4}|hh?|HH?|mm?|ss?|F{1,6}|f{1,6}|tt?))";
		public const string MaskEnumPartRegexp = @"<[^><|]+(\|[^><|]+)+>";
		public const string MaskRangePartRegexp = @"<(?<num1>-?\d+)\.\.(?<num2>-?\d+)(?:\.\.(?<num3>-?\d+))?(?<separator>[g]{1})?>";
		public const string MaskPromtPartRegexp = @"(((?<=(^)|((^|[^\\])(\\\\)*))([aAlLcC#09]{1,1})))";
		public const string MaskModifierPartRegexp = @"((?<=(^)|((^|[^\\])(\\\\)*))([<>~]{1,1}))";
		public const string MaskDatePartRegexp = @"((?<=(^)|((^|[^\\])(\\\\)*))(y{1,4}|M{1,4}|d{1,4}|hh?|HH?|mm?|ss?|F{1,6}|f{1,6}|tt?))";
		public const string MaskQuotePartRegexp = @"(((?<=(^)|((^|[^\\])(\\\\)*))(['""]{1,1})))";
		public const string MaskStaticCultureIndependentCharacterRegexp = @"(\\[^\u0000])";
		public const string MaskStaticCharacterRegexp = @"([^\u0000])";
		public const char ReplaceToken = (char)0x0000;
		public const int FirstLeapYear = 1004;
		public const string SignumPattern = "#";
		public const string OptionalDigitPattern = "9";
		public const string RequiredDigitPattern = "0";
		public const string AlphanumericPattern = "a";
		public const string AlphabeticPattern = "l";
		public const string AnyCharPattern = "c";
		public const string TimeSeparatorPattern = ":";
		public const string DateSeparatorPattern = "/";
		public const char MinusSymbol = '-';
		public const char PlusSymbol = '+';
		public const string StopModifyPattern = "~";
		public const string ToLowerCasePattern = "<";
		public const string ToUpperCasePattern = ">";
		public const string DecimalPointPattern = ".";
		public const string NumGroupSeparatorPattern = ",";
		public const string CurrencyPattern = "$";
		public static readonly ParseInfo[] ParseInfoCollection;
		static MaskPartConstants() {
			ParseInfoCollection = new ParseInfo[]{
				new ParseInfo(TokenRemoveRegexpPattern, ParseAction.RemoveReplaceToken),
				new ParseInfo(DatePartPatchRegexpPattern, ParseAction.BackslashDateTime),
				new ParseInfo(MaskEnumPartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskRangePartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskQuotePartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskPromtPartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskModifierPartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskDatePartRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskStaticCultureIndependentCharacterRegexp, ParseAction.CreatePart),
				new ParseInfo(MaskStaticCharacterRegexp, ParseAction.CreatePart)
			};
		}
	}
	public class MaskValidationResult {
		public bool IsValid { get; set; }
		public int EndPosition { get; set; }
		public string CheckedValue { get; set; }
		public string PossibleValue { get; set; }
		public ModifyAction CheckModifier { get; set; }
		public static MaskValidationResult CreateSuccess(int position, string value) {
			return new MaskValidationResult() { IsValid = true, PossibleValue = value, EndPosition = position };
		}
		public static MaskValidationResult CreateFailed(int position, string value) {
			return new MaskValidationResult() { IsValid = false, PossibleValue = value, EndPosition = position };
		}
	}
	public class ParseInfo {
		public string RegexpString { get; set; }
		public ParseAction ParseAction { get; set; }
		public ParseInfo(string regexp, ParseAction action) {
			RegexpString = regexp;
			ParseAction = action;
		}
	}
	public enum ParseAction {
		CreatePart,
		BackslashDateTime,
		RemoveReplaceToken
	}
	public enum ModifyAction {
		ToUpperCase = 1,
		ToLowerCase = 2,
		StopModify = -1,
		None = 0
	}
	public enum MaskDateTimeComponentType {
		Year,
		Month,
		Day,
		DayOfWeek,
		Hour,
		Minute,
		Second,
		Millisecond,
		AMPM
	}
	public enum MaskPromtCharType {
		Signum = 0,
		RequiredDigit = 1,
		OptionalDigit = 2,
		Alphanumeric = 3,
		Alphabetic = 4,
		Anychar = 5,
	}
	#endregion
}
