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
	public class MaskValidator {
		public static bool IsValueValid(string value, string maskValue) {
			return IsValueValid(value, maskValue, MaskIncludeLiteralsMode.All);
		}
		public static bool IsValueValid(string value, MaskSettings settings) {
			return IsValueValid(value, settings.Mask, settings.IncludeLiterals);
		}
		public static bool IsValueValid(string value, string maskValue, MaskIncludeLiteralsMode includeLiterals) {
			Mask mask = new Mask(maskValue, includeLiterals);
			return mask.IsValid(value);
		}
	}
	public class MaskBase {
		public string MaskValue { get; protected set; }
		public MaskIncludeLiteralsMode IncludeLiterals { get; protected set; }
		public string MaskTimeSeparator { get; private set; }
		public string MaskDateSeparator { get; private set; }
		public string MaskGroupNumSeparator { get; private set; }
		public string MaskDecimalPoint { get; private set; }
		public string MaskCurrencySymbol { get; private set; }
		public string[] MonthNames { get; private set; }
		public string[] AbbrMonthNames { get; private set; }
		public string[] DayNames { get; private set; }
		public string[] AbbrDayNames { get; private set; }
		public string[] AMPM { get; private set; }
		public bool HasAMPM { get; private set; }
		public int[] NumberGroupSizes { get; private set; }
		public Calendar Calendar { get; private set; }
		public MaskBase() : this(CultureInfo.CurrentCulture) { }
		public MaskBase(CultureInfo cultureInfo) {
			InitializeCultureDependentProperties(cultureInfo);
		}
		private void InitializeCultureDependentProperties(CultureInfo cultureInfo) {
			DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;
			MonthNames = RemoveEmpty(dateTimeFormat.MonthGenitiveNames);
			AbbrMonthNames = RemoveEmpty(dateTimeFormat.AbbreviatedMonthNames);
			DayNames = RemoveEmpty(dateTimeFormat.DayNames);
			AbbrDayNames = RemoveEmpty(dateTimeFormat.AbbreviatedDayNames);
			MaskTimeSeparator = dateTimeFormat.TimeSeparator;
			MaskDateSeparator = dateTimeFormat.DateSeparator;
			AMPM = new string[] { dateTimeFormat.AMDesignator, dateTimeFormat.PMDesignator };
			HasAMPM = AMPM.All(d => !string.IsNullOrEmpty(d));
			Calendar = dateTimeFormat.Calendar;
			NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
			MaskGroupNumSeparator = numberFormat.NumberGroupSeparator;
			MaskDecimalPoint = numberFormat.NumberDecimalSeparator;
			NumberGroupSizes = numberFormat.NumberGroupSizes;
			MaskCurrencySymbol = numberFormat.CurrencySymbol;
		}
		private string[] RemoveEmpty(string[] array) {
			return array.Where(s => !string.IsNullOrEmpty(s)).ToArray();
		}
	}
	public class Mask : MaskBase {
		protected readonly MaskPartCollection MaskParts;
		public Mask(MaskSettings maskSettings)
			: this(maskSettings.Mask, maskSettings.IncludeLiterals) {
		}
		public Mask(string value)
			: this(value, MaskIncludeLiteralsMode.All) {
		}
		public Mask(string value, MaskIncludeLiteralsMode includeLiterals)
			: base() {
			MaskValue = (value ?? "").Trim(MaskPartConstants.ReplaceToken);
			IncludeLiterals = includeLiterals;
			MaskParts = MaskPartBuilder.GetMaskParts(this);
		}
		public bool IsValid(string inputValue) {
			if(MaskParts.Count() == 0)
				return true;
			return IsStringValid(inputValue ?? "");
		}
		public int GetCapacity() {
			return MaskParts.Where(p => !p.CanSkip()).Sum(p => p.GetPartLength());
		}
		private ModifyAction GetModifyAction(ModifyAction oldValue, ModifyAction newValue) {
			switch(newValue) {
				case ModifyAction.None:
					return oldValue;
				case ModifyAction.ToLowerCase:
				case ModifyAction.ToUpperCase:
					return newValue;
				case ModifyAction.StopModify:
					return ModifyAction.None;
				default:
					throw new NotImplementedException(string.Format("ModifyAction.{0} has not been handled", newValue));
			}
		}
		private bool IsStringValid(string input) {
			input = input.Trim(MaskPartConstants.ReplaceToken);
			int prevPosition = 0;
			ModifyAction currentAction = ModifyAction.None;
			int lenght = MaskParts.Count();
			for(int i = 0; i < lenght; i++) {
				IMaskPart part = MaskParts[i];
				if(part.CanSkip())
					continue;
				currentAction = GetModifyAction(currentAction, part.GetModifyAction());
				MaskValidationResult result = part.TryValidate(input, prevPosition, prevPosition + part.GetPartLength(), currentAction);
				if(!result.IsValid)
					return false;
				prevPosition = result.EndPosition;
			}
			return prevPosition >= input.Length && MaskParts.IsPresentedDateValid();
		}
	}
	public static class MaskPartBuilder {
		public static IMaskPart GetMaskPart(string pattern, string value, MaskBase mask) {
			switch(pattern) {
				case MaskPartConstants.MaskEnumPartRegexp:
					return new MaskEnumPart(value, mask);
				case MaskPartConstants.MaskRangePartRegexp:
					return new MaskRangePart(value, mask);
				case MaskPartConstants.MaskPromtPartRegexp:
					return new MaskPromtPart(value, mask);
				case MaskPartConstants.MaskModifierPartRegexp:
					return new MaskCharModifierPart(value, mask);
				case MaskPartConstants.MaskStaticCharacterRegexp:
					return new MaskStaticCharPart(value, mask);
				case MaskPartConstants.MaskStaticCultureIndependentCharacterRegexp:
					return new MaskStaticCultureIndependentCharPart(value, mask);
				case MaskPartConstants.MaskDatePartRegexp:
					return new MaskDateTimePart(value, GetDateTimePart(value, mask));
				case MaskPartConstants.MaskQuotePartRegexp:
					return new MaskQuotePart(value, mask);
				default:
					throw new NotImplementedException();
			}
		}
		public static MaskPartCollection GetMaskParts(string inputValue) {
			return GetMaskParts(inputValue, MaskIncludeLiteralsMode.All);
		}
		public static MaskPartCollection GetMaskParts(string inputValue, MaskIncludeLiteralsMode includeLiterals) {
			return CreateMaskParts(new Mask(inputValue, includeLiterals));
		}
		public static MaskPartCollection GetMaskParts(MaskBase mask) {
			return CreateMaskParts(mask);
		}
		private static MaskPartCollection CreateMaskParts(MaskBase mask) {
			string maskValue = mask.MaskValue;
			MaskPartCollection parts = new MaskPartCollection();
			foreach(ParseInfo info in MaskPartConstants.ParseInfoCollection) {
				maskValue = Regex.Replace(maskValue, info.RegexpString, match => {
					switch(info.ParseAction) {
						case ParseAction.BackslashDateTime:
							return Regex.Replace(match.Value, ".", m => (m.Index > 0 ? @"\" : "") + m.Value);
						case ParseAction.CreatePart:
							parts.Insert(match.Index, info.RegexpString, match.Value, mask);
							return new string(MaskPartConstants.ReplaceToken, match.Length);
					}
					return null;
				});
			}
			return ExtractQuotedParts(parts).SetIncludeLiterals(mask.IncludeLiterals);
		}
		private static MaskPartCollection ExtractQuotedParts(MaskPartCollection parts) {
			IEnumerable<MaskQuotePart> quoteCharacters = parts.OfType<MaskQuotePart>(p => p.IsQuotePart);
			if(!quoteCharacters.Any())
				return parts;
			List<IMaskPart> result = new List<IMaskPart>();
			bool isQuoting = false;
			for(int i = 0; i < parts.Count(); i++) {
				IMaskPart part = parts[i];
				if(part is MaskQuotePart) {
					isQuoting = !isQuoting;
					continue;
				}
				if(isQuoting)
					result.AddRange(MaskStaticCultureIndependentCharPart.Create(part));
				else
					result.Add(part);
			}
			return new MaskPartCollection(result);
		}
		private static IMaskPart GetDateTimePart(string value, MaskBase maskBase) {
			switch(value) {
				case "yyyy":
					return new MaskRangePart("<0100..9999>", maskBase);
				case "yyy":
					return new MaskRangePart("<100..9999>", maskBase);
				case "yy":
					return new MaskRangePart("<00..99>", maskBase);
				case "y":
					return new MaskRangePart("<0..99>", maskBase);
				case "MMMM":
					return new MaskEnumPart("<" + string.Join("|", maskBase.MonthNames) + ">", maskBase);
				case "MMM":
					return new MaskEnumPart("<" + string.Join("|", maskBase.AbbrMonthNames) + ">", maskBase);
				case "MM":
					return new MaskRangePart("<01..12>", maskBase);
				case "M":
					return new MaskRangePart("<1..12>", maskBase);
				case "dddd":
					return new MaskEnumPart("<" + string.Join("|", maskBase.DayNames) + ">", maskBase);
				case "ddd":
					return new MaskEnumPart("<" + string.Join("|", maskBase.AbbrDayNames) + ">", maskBase);
				case "dd":
					return new MaskRangePart("<01..31>", maskBase);
				case "d":
					return new MaskRangePart("<1..31>", maskBase);
				case "hh":
					return new MaskRangePart("<01..12..12>", maskBase);
				case "h":
					return new MaskRangePart("<1..12..12>", maskBase);
				case "HH":
					return new MaskRangePart("<00..23>", maskBase);
				case "H":
					return new MaskRangePart("<0..23>", maskBase);
				case "ss":
				case "mm":
					return new MaskRangePart("<00..59>", maskBase);
				case "m":
				case "s":
					return new MaskRangePart("<0..59>", maskBase);
				case "f":
				case "F":
					return new MaskRangePart("<0..9>", maskBase);
				case "ff":
				case "FF":
					return new MaskRangePart("<0..99>", maskBase);
				case "fff":
				case "FFF":
				case "ffff":
				case "FFFF":
				case "fffff":
				case "FFFFF":
				case "ffffff":
				case "FFFFFF":
					return new MaskRangePart("<0..999>", maskBase);
				case "t":
				case "tt":
					if(maskBase.HasAMPM)
						return new MaskEnumPart("<" + string.Join("|", maskBase.AMPM.Select(d => value.Length < 2 ? d.Substring(0, 1) : d)) + ">", maskBase);
					return new MaskStaticCharPart("", maskBase);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
