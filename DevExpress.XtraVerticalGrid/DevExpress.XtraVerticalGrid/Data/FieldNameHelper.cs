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

#define OBSOLETE_CODE
using System.Collections.Generic;
using System.Text;
#if !Win
using System.Linq;
using System.Text.RegularExpressions;
#endif
namespace DevExpress.XtraVerticalGrid.Data {
	static class FieldNameHelper {
#if !Win
		static readonly string anyCharExceptSpecial = @"[^\<\>\(\)\.]";  
		static readonly string groupFormat = @"(?<g>{0})";
		static readonly string simpleFormat = string.Format(@"(?:{0})",
			string.Format(groupFormat, @"\<[^\<\>]*\>") + "|" + 
			string.Format(@"\({0}\)", string.Format(groupFormat, @"[^\(\)]*")) + "|" + 
			string.Format(groupFormat, anyCharExceptSpecial + "+")	   
			);
		static readonly string leftExtended = string.Format(groupFormat, string.Format(@"{0}+{1}{0}*", anyCharExceptSpecial, simpleFormat));
		static readonly string rightExtended = string.Format(groupFormat, string.Format(@"{0}*{1}{0}+", anyCharExceptSpecial, simpleFormat));
		static readonly Regex pathExpression = new Regex(
			@"(?<=^|\.)" + 
			leftExtended + "|" + rightExtended + "|" + simpleFormat
			, RegexOptions.Compiled);
#endif
		public const string FieldNameDelimiter = ".";
		public static string GetParentFieldName(string fieldName) {
			string[] path = GetPath(fieldName);
			StringBuilder parentNameStringBuilder = new StringBuilder();
			for(int i = 0; i < path.Length - 1; i++) {
				parentNameStringBuilder.Append(GetEscapeString(path[i])).Append(FieldNameDelimiter);
			}
			if(parentNameStringBuilder.Length >= FieldNameDelimiter.Length) {
				parentNameStringBuilder.Length -= FieldNameDelimiter.Length;
			}
			return parentNameStringBuilder.ToString();
		}
		public static string[] GetPath(string fieldName) {
			if (string.IsNullOrEmpty(fieldName))
				return new string[0];
#if Win
			return fieldName.Split('.');
#elif OBSOLETE_CODE
			var result = pathExpression.Matches(fieldName).OfType<Match>().Where(x => x.Success).Select(x => x.Groups["g"].Value).ToArray();
			return result;
#else
			var result = new string[fieldName.Length];
			int resultIndex = 0;
			int attb = 0;
			int catb = 0;
			StringBuilder currentBuilder = new StringBuilder();			
			for (int i = 0; i < fieldName.Length; i++) {
				var current = fieldName[i];				
				if (current == '.' && (attb == 0 && catb == 0)) {
					result[resultIndex] = currentBuilder.ToString();
					currentBuilder.Clear();
					resultIndex++;
					continue;
				}
				currentBuilder.Append(current);
				if (current == '(') {
					attb++;	
					continue;
				}
				if (current == '<') {
					catb++;
					continue;
				}
				if (current == ')') {
					attb--;
					continue;
				}
				if (current == '>') {
					catb--;
					continue;
				}
			}
			result[resultIndex] = currentBuilder.ToString();
			resultIndex++;
			return result.Take(resultIndex).ToArray();
#endif
		}
		public static string GetFieldName(string parentFieldName, string propertyName) {
			if(string.IsNullOrEmpty(propertyName))
				return parentFieldName;
#if !Win
			propertyName = GetEscapeString(propertyName);
#endif
			if(parentFieldName == string.Empty)
				return propertyName;
			return parentFieldName + FieldNameDelimiter + propertyName;
		}
		public static string GetPropertyName(string fieldName) {
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			string[] path = GetPath(fieldName);
			return path[path.Length - 1];
		}
#if !Win
		public static string GetCategoryFieldName(string fieldName, string categoryName) {
			return fieldName + "<" + categoryName + ">";
		}
#endif
		static string GetEscapeString(string propertyName) {
#if !Win
#if OBSOLETE_CODE
			if (propertyName.Contains(FieldNameDelimiter) && !(propertyName.StartsWith("<") && propertyName.EndsWith(">")))
				return '(' + propertyName + ')';
#endif
#endif
			return propertyName;
		}
	}
}
