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
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using System.Collections;
using DevExpress.Data;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.Native {
	public static class SearchControlHelper {
		public static List<string> ParseColumnsString(string columnsString) {
			return columnsString.Split(';').Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s)).Distinct().ToList();
		}
		static List<string> RemoveDublicates(List<string> inputList) {
			return inputList.Distinct().ToList();
		}
		public static CriteriaOperator GetCriteriaOperator(ISearchPanelColumnProviderBase columnProvider, FilterCondition filterCondition, string searchText, CriteriaOperatorType operatorType) {
			if(string.IsNullOrEmpty(searchText))
				return null;
			RemovePlusMinusIfNeed(ref searchText);
			if(operatorType == CriteriaOperatorType.And)
				AddPlusIfNeed(ref searchText);
			if(columnProvider is ISearchPanelColumnProvider)
				return GetCriteriaOperator(columnProvider as ISearchPanelColumnProvider, filterCondition, searchText, operatorType);
			if(columnProvider is ISearchPanelColumnProviderEx)
				return GetCriteriaOperator(columnProvider as ISearchPanelColumnProviderEx, filterCondition, searchText, operatorType);
			return null;
		}
		static CriteriaOperator GetCriteriaOperator(ISearchPanelColumnProvider columnProvider, FilterCondition filterCondition, string searchText, CriteriaOperatorType operatorType) {
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText);
			parserResults.AppendColumnFieldPrefixes();
			return DxFtsContainsHelper.Create(columnProvider.Columns.ToArray(), parserResults, filterCondition);
		}
		static CriteriaOperator GetCriteriaOperator(ISearchPanelColumnProviderEx columnProviderEx, FilterCondition filterCondition, string searchText, CriteriaOperatorType operatorType) {
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText, columnProviderEx.Columns);
			if(!columnProviderEx.IsServerMode)
				parserResults.AppendColumnFieldPrefixes();
			CriteriaOperator result = DxFtsContainsHelperAlt.Create(parserResults, filterCondition, columnProviderEx.IsServerMode);
			AddColumnsForceWithoutPrefix(columnProviderEx, filterCondition, searchText, operatorType, ref result);
			if(columnProviderEx.CustomFilterColumns == null || columnProviderEx.CustomFilterColumns.Count == 0)
				return result;
			foreach(FilterCondition condition in Enum.GetValues(typeof(FilterCondition)).Cast<FilterCondition>())
				ApplyCriteriaOperatorCustomFormat(columnProviderEx, condition, searchText, operatorType, ref result);
			return result;
		}
		static void AddColumnsForceWithoutPrefix(ISearchPanelColumnProviderEx columnProviderEx, FilterCondition filterCondition, string searchText, CriteriaOperatorType operatorType, ref CriteriaOperator op) {
			if(columnProviderEx.ColumnsForceWithoutPrefix.Count == 0)
				return;
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText, columnProviderEx.ColumnsForceWithoutPrefix);
			CriteriaOperator op2 = DxFtsContainsHelperAlt.Create(parserResults, filterCondition, columnProviderEx.IsServerMode);
			op = ConcatCriteriaOperators(operatorType, op, op2);
		}
		static CriteriaOperator ConcatCriteriaOperators(CriteriaOperatorType operatorType, CriteriaOperator op1, CriteriaOperator op2) {
			return operatorType == CriteriaOperatorType.And ? CriteriaOperator.And(op1, op2) : CriteriaOperator.Or(op1, op2);
		}
		static void ApplyCriteriaOperatorCustomFormat(ISearchPanelColumnProviderEx columnProviderEx, FilterCondition customFilterCondition, string searchText, CriteriaOperatorType operatorType, ref CriteriaOperator op) {
			ICollection columns = (from col in columnProviderEx.CustomFilterColumns
										   where col.FilterCondition == customFilterCondition
										   select col.Column).ToList();
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText, columns);
			if(!columnProviderEx.IsServerMode)
				parserResults.AppendColumnFieldPrefixes();
			CriteriaOperator op2 = DxFtsContainsHelperAlt.Create(parserResults, customFilterCondition, columnProviderEx.IsServerMode);
			op = ConcatCriteriaOperators(operatorType, op, op2);
		}
		public static CriteriaOperator GetSimpleCriteriaOperator(IEnumerable<string> columns, FilterCondition filterCondition, string searchText, CriteriaOperatorType operatorType) {
			if (string.IsNullOrEmpty(searchText))
				return null;
			RemovePlusMinusIfNeed(ref searchText);
			if (operatorType == CriteriaOperatorType.And)
				AddPlusIfNeed(ref searchText);
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText);
			return DxFtsContainsHelper.Create(columns.ToArray(), parserResults, filterCondition);
		}
#if DEBUGTEST
		public
#endif
		static void RemovePlusMinusIfNeed(ref string searchText) {
			if(searchText.Contains("\""))
				return;
			string result = String.Empty;
			foreach(string str in searchText.Split(' ')){
				if(String.IsNullOrWhiteSpace(str))
					continue;
				if(str == "-" || str == "+")
					continue;
				string separator = String.IsNullOrEmpty(result) ? String.Empty : " ";
				result += separator + str;
			}
			searchText = result;
		}
		static void AddPlusIfNeed(ref string searchText) {
			if(string.IsNullOrEmpty(searchText))
				return;
			if(searchText[0] != '-' && searchText[0] != '+')
				searchText = '+' + searchText;
			string updatedText = "";
			bool backets = false;
			for(int i = 0; i < searchText.Length; i++) {
				updatedText += searchText[i];
				if(searchText[i] == '"')
					backets = !backets;
				if(backets)
					continue;
				if(searchText[i] == ' ' && searchText[i + 1] != '-' && searchText[i + 1] != '+')
					updatedText += '+';
			}
			searchText = updatedText;
		}
		public static List<FieldAndHighlightingString> GetTextHighlightingString(string searchText, ICollection columns, FilterCondition filterCondition) {
			FindSearchParserResults parserResults = new FindSearchParser().Parse(searchText, columns);
			List<FieldAndHighlightingString> result = new List<FieldAndHighlightingString>();
			foreach(IDataColumnInfo column in columns)
				result.Add(new FieldAndHighlightingString(column.FieldName, GetTextHighlightingString(parserResults.SearchTexts, filterCondition)));
			foreach(FindSearchField field in parserResults.Fields) {
				List<FieldAndHighlightingString> fhrl = result.Where(fhr => fhr.Field.ToLower() == field.Name.ToLower()).ToList();
				if(fhrl.Count == 0)
					continue;
				fhrl[0].AddHighlightingString(GetTextHighlightingString(field.Values, filterCondition));
			}
			return result;
		}
		static string GetTextHighlightingString(string[] searchTexts, FilterCondition filterCondition) {
			if(searchTexts.Length == 0)
				return String.Empty;
			string resultText = String.Empty;
			for(int i = 0; i < searchTexts.Length; i++) {
				if(searchTexts[i].StartsWith("+") || searchTexts[i].StartsWith("-"))
					searchTexts[i] = searchTexts[i].Remove(0, 1);
				if(filterCondition == FilterCondition.Default || filterCondition == FilterCondition.Like)
					searchTexts[i] = searchTexts[i].Replace("%", String.Empty);
				resultText += resultText == String.Empty ? searchTexts[i] : "\n" + searchTexts[i];
			}
			return resultText;
		}
		public static void UpdateTextHighlighting(BaseEditSettings settings, TextHighlightingProperties highlightingProperties) {
			if(settings == null || !(settings is TextEditSettings))
				return;
			if(highlightingProperties == null)
				highlightingProperties = GetDefaultTextHighlightingProperties(); 
			LookUpEditHelper.SetHighlightedText((TextEditSettings)settings, highlightingProperties.Text, highlightingProperties.FilterCondition);
		}
		internal static TextHighlightingProperties GetDefaultTextHighlightingProperties() {
			return new TextHighlightingProperties(null, FilterCondition.Default);
		}
	}
	public class FieldAndHighlightingString {
		public FieldAndHighlightingString(string field, string highlightingString = "") {
			Field = field;
			HighlightingString = highlightingString;
		}
		public void AddHighlightingString(string stringToAdd) {
			HighlightingString += String.IsNullOrEmpty(HighlightingString) ? stringToAdd : "\n" + stringToAdd;
		}
		public string Field { get; private set; }
		public string HighlightingString { get; private set; }
	}
}
