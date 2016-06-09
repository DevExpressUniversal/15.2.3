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

using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Services {
	#region WorksheetNameError
	public enum WorksheetNameError {
		None = 0,
		Blank = 1,
		Duplicate = 2,
		ContainsProhibitedCharacters = 3,
		StartOrEndWithSingleQuote = 4,
		ExceedAllowedLength = 5
	}
	#endregion
	public interface IWorksheetNameCreationService {
		char[] RestrictedCharacters { get; }
		string GetNormalizedName(string sheetName, string[] existingSheetNames, bool existingSheetNamesContainsSheetName);
		WorksheetNameError VerifyName(string sheetName, string[] existingSheetNames);
	}
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region WorksheetNameCreationService
	public class WorksheetNameCreationService : IWorksheetNameCreationService {
		#region Fields
		const int maximumAllowedNameLenght = 31;
		static readonly char[] restrictedChars = { '\\', '/', '?', '*', '[', ']', ':' };
		readonly string defaultSheetName;
		#endregion
		public WorksheetNameCreationService() {
			this.defaultSheetName = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DefaultWorksheetName);
		}
		#region IWoorksheetNameCreationService Members
		public char[] RestrictedCharacters { get { return restrictedChars; } }
		public string GetNormalizedName(string sheetName, string[] existingSheetNames, bool existingSheetNamesContainsSheetName) {
			if (string.IsNullOrEmpty(sheetName))
				return GetPredefinedName(defaultSheetName, existingSheetNames);
			string normalizedName = ProcessRestrictedCharacters(sheetName);
			if (string.IsNullOrEmpty(normalizedName))
				return GetPredefinedName(defaultSheetName, existingSheetNames);
			int repeatsCount;
			if (IsUniqueName(normalizedName, existingSheetNames, out repeatsCount))
				return normalizedName;
			if (existingSheetNamesContainsSheetName)
				if (repeatsCount < 2 && string.Compare(sheetName, normalizedName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return sheetName;
			return GetPredefinedName(defaultSheetName, existingSheetNames);
		}
		public WorksheetNameError VerifyName(string sheetName, string[] existingSheetNames) {
			if (string.IsNullOrEmpty(sheetName))
				return WorksheetNameError.Blank;
			int repeatsCount;
			if (!IsUniqueName(sheetName, existingSheetNames, out repeatsCount))
				return WorksheetNameError.Duplicate;
			if (sheetName.StartsWith("'", StringComparison.Ordinal) || sheetName.EndsWith("'", StringComparison.Ordinal))
				return WorksheetNameError.StartOrEndWithSingleQuote;
			if (sheetName.IndexOfAny(restrictedChars) >= 0)
				return WorksheetNameError.ContainsProhibitedCharacters;
			if (sheetName.Length > maximumAllowedNameLenght)
				return WorksheetNameError.ExceedAllowedLength;
			return WorksheetNameError.None;
		}
		#endregion
		bool IsUniqueName(string sheetName, string[] existingSheetNames, out int repeatsCount) {
			repeatsCount = 0;
			int index = -1;
			int count = existingSheetNames.Length;
			for (int i = 0; i < count; i++)
				if (String.Compare(existingSheetNames[i], sheetName, StringComparison.CurrentCultureIgnoreCase) == 0) {
					index = i;
					++repeatsCount;
				}
			if (index < 0)
				return true;
			return false;
		}
		string ProcessRestrictedCharacters(string sheetName) {
			StringBuilder sb = new StringBuilder(maximumAllowedNameLenght + 16);
			foreach (char ch in sheetName) {
				if (sb.Length >= maximumAllowedNameLenght)
					break;
				switch (ch) {
					case '[': 
						sb.Append('(');
						break;
					case ']': 
						sb.Append(')');
						break;
					case '\\':
					case '/':
					case '*':
					case ':':
					case '?': 
						break;
					case '\'':
						if (sb.Length > 0) 
							sb.Append(ch);
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
			for (int i = sb.Length; (i > 0) && (sb[--i] == '\''); ) 
				sb.Length = i;
			return sb.ToString();
		}
		string GetPredefinedName(string sheetIdentificator, string[] existingSheetNames) {
			int repeatsCount;
			for (int i = existingSheetNames.Length; i < Int32.MaxValue; ) {
				string name = string.Format("{0}{1}", sheetIdentificator, ++i);
				if (IsUniqueName(name, existingSheetNames, out repeatsCount))
					return name;
			}
			Exceptions.ThrowInternalException();
			return string.Empty;
		}
	}
	#endregion
	#region WorksheetNotValidatedNameCreationService
	public class WorksheetNotValidatedNameCreationService : IWorksheetNameCreationService {
		static readonly char[] restrictedCharacters = { };
		public char[] RestrictedCharacters { get { return restrictedCharacters; } }
		public string GetNormalizedName(string sheetName, string[] existingSheetNames, bool existingSheetNamesContainsSheetName) {
			return sheetName;
		}
		public WorksheetNameError VerifyName(string sheetName, string[] existingSheetNames) {
			return WorksheetNameError.None;
		}
	}
	#endregion
}
