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
using System.Linq;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Native {
	public static class DisplayFormatHelper {
		public static bool GetDisplayFormatParts(string sourceDisplayFormat, out string prefix, out string displayFormat, out string suffix, string nullValueDisplayFormat) {
			prefix = String.Empty;
			displayFormat = String.Empty;
			suffix = String.Empty;
			if(String.IsNullOrEmpty(sourceDisplayFormat)) {
				if(String.IsNullOrEmpty(nullValueDisplayFormat))
					return true;
				if(!GetDisplayFormatParts(nullValueDisplayFormat, out prefix, out displayFormat, out suffix, nullValueDisplayFormat))
					return false;
				return true;
			}
			string dirtyFormat;
			if(!GetSplittedDifsplayFormat(sourceDisplayFormat, out prefix, out dirtyFormat, out suffix))
				return false;
			if(!GetCleanFormat(dirtyFormat, out displayFormat))
				return false;
			return true;
		}
		public static string GetDisplayTextFromDisplayFormat(IFormatProvider language, string displayFormat, params object[] displayFormatArgs) {
			string prefix = String.Empty;
			string cleanDisplayFormat = String.Empty;
			string suffix = String.Empty;
			if(!GetDisplayFormatParts(displayFormat, out prefix, out cleanDisplayFormat, out suffix, String.Empty))
				return String.Empty;
			try {
				return String.Format(language, displayFormat, displayFormatArgs);
			}
			catch(FormatException) {				
				return String.Format(language, GetDisplayFormatFromParts(prefix, String.Empty, suffix), displayFormatArgs);
			}
		}
#if DEBUG
		public
#endif
 static bool GetSplittedDifsplayFormat(string sourceDisplayFormat, out string prefix, out string displayFormat, out string suffix) {
			prefix = String.Empty;
			displayFormat = String.Empty;
			suffix = String.Empty;
			if(!sourceDisplayFormat.Replace("{{", "{").Replace("}}", "}").Contains("{0")) {
				displayFormat = sourceDisplayFormat;
				return true;
			}
			int indexOfZeroOpen = sourceDisplayFormat.IndexOf("{0");
			prefix = sourceDisplayFormat.Remove(indexOfZeroOpen);
			prefix = GetVisualStringFromSuffix(prefix);
			string subDisplayFormat = sourceDisplayFormat.Remove(0, indexOfZeroOpen);
			int indexOfZeroClose;
			if(!GetIndexOfZeroClose(subDisplayFormat, indexOfZeroOpen, out indexOfZeroClose))
				return false;
			displayFormat = indexOfZeroClose == subDisplayFormat.Length - 1 ? subDisplayFormat : subDisplayFormat.Remove(indexOfZeroClose + 1);
			suffix = indexOfZeroClose == subDisplayFormat.Length - 1 ? String.Empty : subDisplayFormat.Remove(0, indexOfZeroClose + 1);
			suffix = GetVisualStringFromSuffix(suffix);
			return true;
		}
		static bool GetIndexOfZeroClose(string subDisplayFormat, int indexOfZeroOpen, out int indexOfZeroClose) {
			indexOfZeroClose = -1;
			bool isParamOpen = false;
			for(int i = 0; i < subDisplayFormat.Length; i++) {
				if(subDisplayFormat[i] != '}') {
					switch(subDisplayFormat[i]) {
						case ':':
						case ',':
							isParamOpen = true;
							break;
					}
					continue;
				}
				if(!isParamOpen) {
					indexOfZeroClose = i;
					return true;
				}
				if(i == subDisplayFormat.Length - 1) {
					indexOfZeroClose = i;
					return true;
				}
				if(subDisplayFormat[i + 1] != '}') {
					indexOfZeroClose = i;
					return true;
				}
				else {
					i++;
				}
			}
			return false;
		}
		static bool GetCleanFormat(string dirtyFormat, out string displayFormat) {
			displayFormat = dirtyFormat;
			if(!dirtyFormat.StartsWith("{0"))
				return true;
			if(!dirtyFormat.EndsWith("}"))
				return true;
			displayFormat = dirtyFormat.Remove(dirtyFormat.Length - 1).Remove(0, 2);
			if(displayFormat.StartsWith(":"))
				displayFormat = displayFormat.Remove(0, 1);
			return true;
		}
		public static string GetDisplayFormatFromParts(string prefix, string currentDisplayFormat, string suffix) {
			prefix = DisplayFormatHelper.GetSuffixFromVisualString(prefix);
			suffix = DisplayFormatHelper.GetSuffixFromVisualString(suffix);
			if(String.IsNullOrEmpty(currentDisplayFormat)) {
				if(String.IsNullOrEmpty(prefix) && String.IsNullOrEmpty(suffix))
					return "";
				return String.Concat(prefix, "{0}", suffix);
			}
			if(currentDisplayFormat.StartsWith("{0"))
				return String.Concat(prefix, currentDisplayFormat, suffix);
			if(currentDisplayFormat.StartsWith(","))
				return String.Concat(prefix, "{0", currentDisplayFormat, "}", suffix);
			if(String.IsNullOrEmpty(prefix) && String.IsNullOrEmpty(suffix))
				return currentDisplayFormat;
			return String.Concat(prefix, "{0:", currentDisplayFormat, "}", suffix);
		}
		public static string GetVisualStringFromSuffix(string suffix) {
			return suffix.Replace("{1}", "{column}");
		}
		public static string GetSuffixFromVisualString(string str) {
			if(str == null)
				return String.Empty;
			if(!str.Contains("{{column}}"))
				return str.Replace("{column}", "{1}");
			string result = String.Empty;
			for(int i = 0; i < str.Length; i++) {
				if(str.Remove(0, i).StartsWith("{{column}}")) {
					result += "{{column}}";
					i += 9;
					continue;
				}
				if(str.Remove(0, i).StartsWith("{column}")) {
					result += "{1}";
					i += 7;
					continue;
				}
				result += str[i];
			}
			return result;
		}
	}
	public static class DisplayFormatValidationHelper {
		static DevExpress.Utils.Localization.XtraLocalizer<EditorStringId> localizer = EditorLocalizer.Active;
		public static ErrorParameters IsDisplayFormatStringValid(ref string text) {
			if(String.IsNullOrEmpty(text))
				return ErrorParameters.None;
			if(text.ToLower() == "a" || text.ToLower() == ":a")
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			if(text.StartsWith("{0") && text.EndsWith("}")) {
				text = text.Remove(text.Length - 1).Remove(0, 2);
				if(text.StartsWith(":"))
					text.Remove(0, 1);
				return null;
			}
			if(!ValidateBakets(text))
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			if(!ValidateComma(text))
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			if(!ValidateAfterColonValue(text))
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			return ErrorParameters.None;
		}
		public static ErrorParameters GetNullErrorParameters() {
			return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
		}
		static bool ValidateBakets(string text) {
			return ValidateBakets(text, '{') && ValidateBakets(text, '}');
		}
		static bool ValidateBakets(string text, char backet) {
			for(int i = 0; i < text.Length; i++) {
				if(text[i] != backet)
					continue;
				if(i == text.Length - 1)
					return false;
				if(text[i + 1] != backet)
					return false;
				i++;
			}
			return true;
		}
		static bool ValidateComma(string text) {
			if(!text.Contains(","))
				return true;
			if(text == ",")
				return false;
			if(!text.StartsWith(","))
				return true;
			string splitText = text.Contains(":") && (text.IndexOf(":") < text.Length - 1) ? text.Remove(text.IndexOf(':')) : text;
			splitText = splitText.Remove(0, 1);
			if(String.IsNullOrEmpty(splitText))
				return true;
			if(splitText.StartsWith(":"))
				return false;
			splitText = splitText.Contains(":") ? splitText.Remove(text.IndexOf(':') - 1) : splitText;
			if(String.IsNullOrEmpty(splitText))
				return true;
			if(!Char.IsNumber(splitText[0])) {
				if(splitText[0] != '-')
					return false;
				splitText = splitText.Remove(0, 1);
			}
			if(String.IsNullOrEmpty(splitText))
				return true;
			if(String.IsNullOrEmpty(splitText))
				return true;
			foreach(char c in splitText) {
				if(!Char.IsNumber(c) && c != '{' && c != '}')
					return false;
			}
			return true;
		}
		static List<char> notValidAfterColonValues = new List<char>() { 'a', 'b', 'h', 'i', 'j', 'k', 'l', 'm', 'o', 'q', 'r', 's', 't', 'u', 'v', 'w', 'y', 'z', '{', '}' };
		static bool ValidateAfterColonValue(string text) {
			string splitText = text.Contains(":") ? text.Remove(0, text.IndexOf(':') + 1) : text;
			if(String.IsNullOrEmpty(splitText))
				return true;
			if(splitText.Length == 1 && notValidAfterColonValues.Contains(splitText.ToLower()[0]))
				return false;
			return true;
		}
		public static ErrorParameters IsSuffixValid(string suffix) {
			if(String.IsNullOrEmpty(suffix))
				return ErrorParameters.None;
			string splitSuffix = suffix;
			RemoveDubleBakets(ref splitSuffix);
			if(!IsBaketsSuffixValid(splitSuffix))
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			splitSuffix = splitSuffix.Replace("{column}", String.Empty);
			if(splitSuffix.Contains("{") || splitSuffix.Contains("}"))
				return new ErrorParameters(ErrorType.Default, localizer.GetLocalizedString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue));
			return ErrorParameters.None;
		}
		static bool IsBaketsSuffixValid(string splitSuffix) {
			int openBaketsCount = 0;
			int closeBaketsCount = 0;
			foreach(char c in splitSuffix) {
				if(c == '{')
					openBaketsCount++;
				if(c == '}')
					closeBaketsCount++;
			}
			return openBaketsCount == closeBaketsCount;
		}
		static void RemoveDubleBakets(ref string splitSuffix) {
			RemoveDubleBaketsCore(ref splitSuffix, "{{");
			RemoveDubleBaketsCore(ref splitSuffix, "}}");
		}
		static void RemoveDubleBaketsCore(ref string splitSuffix, string bakets) {
			while(true) {
				if(!splitSuffix.Contains(bakets))
					break;
				splitSuffix = splitSuffix.Remove(splitSuffix.IndexOf(bakets), 2);
			}
		}
	}
	public class ErrorParameters {
		ErrorType errorType = ErrorType.None;
		string errorContent = "";
		public ErrorParameters() { }
		public ErrorParameters(ErrorType errorType, string errorContent) {
			SetParameters(errorType, errorContent);
		}
		public void SetParameters(ErrorType errorType, string errorContent) {
			ErrorType = errorType;
			ErrorContent = errorContent;
		}
		public static ErrorParameters None {
			get { return new ErrorParameters(ErrorType.None, ""); }
		}
		public ErrorType ErrorType { get { return errorType; } private set { errorType = value; } }
		public string ErrorContent { get { return errorContent; } private set { errorContent = value; } }
	}
	public enum DisplayFormatGroupType {
		Default, Number, Percent, Currency, Special, Datetime, Custom
	}
	public enum TypeCategory {
		Numeric,
		DateTime,
		Custom,
		String
	}
	public static class DisplayFormatExamleHelper {
		static DevExpress.Utils.Localization.XtraLocalizer<EditorStringId> localizer = EditorLocalizer.Active;
		public static object GetExample(Type type) {
			switch(Type.GetTypeCode(type)) {
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return 100;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return 100.5;
				case TypeCode.String:
					return localizer.GetLocalizedString(EditorStringId.DataTypeStringExample);
				case TypeCode.DateTime:
					return DateTime.Now;
				case TypeCode.Char:
					return localizer.GetLocalizedString(EditorStringId.DataTypeCharExample);
				case TypeCode.Boolean:
					return true;
				default:
					return String.Empty;
			}
		}
		public static TypeCategory GetTypeCategory(Type type) {
			switch(Type.GetTypeCode(type)) {
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return TypeCategory.Numeric;
				case TypeCode.DateTime:
					return TypeCategory.DateTime;
				case TypeCode.String:
					return TypeCategory.String;
				default:
					return TypeCategory.Custom;
			}
		}
	}
}
