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
using System.Globalization;
using System.Text;
using DevExpress.XtraSpellChecker.Parser;
namespace DevExpress.XtraSpellChecker.Native {
	public static class SRCategryNames { 
		 public const string SpellChecker = "SpellChecker";
	}
	#region Exceptions
	public sealed class Exceptions {
		Exceptions() {
		}
		public static void ThrowArgumentException(string propName, object val) {
			string s = propName;
			throw new ArgumentException(s);
		}
		public static void ThrowInternalException() {
			throw new Exception();
		}
	}
	#endregion
	public struct ExceptionPosition {
		public Position StartPosition { get; set; }
		public Position FinishPosition { get; set; }
		public string Word { get; set; }
	}
	public enum WordCaseType {
		Lower,
		Upper,
		InitUpper,
		Mixed,
		InitUpperMixed
	}
	public static class StringUtils {
		public static bool IsAllCaps(string word, CultureInfo culture) {
			return String.Equals(word.ToUpper(culture), word, StringComparison.Ordinal);
		}
		public static bool IsInitCaps(string word, CultureInfo culture) {
			return String.Equals(MakeInitialCharCaps(word, culture), word, StringComparison.Ordinal);
		}
		public static string MakeInitialCharCaps(string word, CultureInfo culture) {
			StringBuilder lowercase = new StringBuilder(word.ToLower(culture));
			if (lowercase.Length > 0)
				lowercase[0] = Char.ToUpper(lowercase[0], culture);
			return lowercase.ToString();
		}
		public static string MakeAllCharsSmall(string word, CultureInfo culture) {
			return word.ToLower(culture);
		}
		public static string MakeInitCharSmall(string word, CultureInfo culture) {
			char[] chars = word.ToCharArray();
			if (chars.Length > 0)
				chars[0] = Char.ToLower(chars[0], culture);
			return new string(chars);
		}
		public static string MakeAllCharsCaps(string word, CultureInfo culture) {
			return word.ToUpper(culture);
		}
		public static WordCaseType GetCaseType(string word) {
			bool isInitUpper = false;
			bool isUpper = false;
			bool isMixed = false;
			int length = word.Length;
			for (int i = 0; i < length; i++) {
				bool charIsUpper = Char.IsUpper(word, i);
				if (i == 0) {
					isInitUpper = charIsUpper;
					isUpper = charIsUpper;
				}
				else {
					if (i == 1)
						isMixed = !isInitUpper && charIsUpper;
					else
						isMixed = isMixed || isUpper != charIsUpper;
					isUpper = isUpper && charIsUpper;
				}
			}
			WordCaseType result;
			if (isUpper)
				result = WordCaseType.Upper;
			else if (isMixed && isInitUpper)
				result = WordCaseType.InitUpperMixed;
			else if (isMixed)
				result = WordCaseType.Mixed;
			else if (isInitUpper)
				result = WordCaseType.InitUpper;
			else
				result = WordCaseType.Lower;
			return result;
		}
	}
}
#if SL
namespace DevExpress.Utils {
   public class InvalidEnumArgumentException : ArgumentException {
	   public InvalidEnumArgumentException(string argumentName, int invalidValue, Type enumClass) : base(string.Format("InvalidEnumArgument : {0} {1} {2}", new object[] { argumentName, invalidValue.ToString(CultureInfo.CurrentCulture), enumClass.Name }), argumentName) {}
   }
}
#endif
