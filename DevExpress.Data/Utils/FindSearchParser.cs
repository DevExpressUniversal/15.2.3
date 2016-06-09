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
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using DevExpress.Data.Filtering;
#if SL
using DevExpress.Utils;
#endif
namespace DevExpress.Data.Helpers {
	public class FindColumnInfo {
		public IDataColumnInfo Column { get; set; }
		public string PropertyName { get; set; }
		public override string ToString() {
			return PropertyName;
		}
	}
	public class FindSearchParser {
		static Regex parser, queryParser;
		protected static Regex SParser {
			get {
				if(parser == null) parser = 
												new Regex(@"(?<find>(?<field>(?:(?:[+-]*""[^""]+"")|(?:(?<!""\w*)[\w#$%&*+-]+)):)+[ ]{0,1}(?<value>(?:(?:[+-]*[\w,.]+)|(?:[+-]*""[^""]+?""))){1})");
				return parser;
			}
		}
		protected static Regex SQueryParser {
			get {
				if(queryParser == null) queryParser = new Regex(@"(?<Text>(?:[+-]*""[^""]+?"")|(?:\S+))");
				return queryParser;
			}
		}
		protected Regex Parser { get { return SParser; } }
		protected Regex QueryParser { get { return SQueryParser; } }
		public FindSearchParserResults Parse(string sourceText) {
			return Parse(sourceText, (FindSearchFieldResolveDelegate)null);
		}
		public virtual FindSearchParserResults Parse(string sourceText, ICollection columns) {
			if(columns == null) return Parse(sourceText);
			FindSearchParserResults res = Parse(sourceText, new FindSearchFieldResolveDelegate(delegate(FindSearchField field) {
				return DefaultOnFindResolveColumnName(field, columns);
			}));
			List<FindColumnInfo> columnNames = new List<FindColumnInfo>();
			foreach(IDataColumnInfo dc in columns) {
				if(!res.ContainsField(dc.FieldName)) {
					columnNames.Add(new FindColumnInfo() { PropertyName = dc.FieldName, Column = dc });
				}
			}
			var cs = columns.Cast<IDataColumnInfo>();
			for(int n = res.FieldCount - 1; n >= 0; n--) {
				FindSearchField field = res.Fields[n];
				IDataColumnInfo column = cs.First(q => q.FieldName == field.Name);
				if(column != null)
					field.ColumnInfo = column;
				else
					res.Fields.RemoveAt(n);
			}
			res.ColumnNames = columnNames.ToArray();
			return res;
		}
		public virtual FindSearchParserResults Parse(string sourceText, FindSearchFieldResolveDelegate fieldResolver) {
			if(sourceText == null) return new FindSearchParserResults("", new string[0], new List<FindSearchField>());
			MatchCollection matches = Parser.Matches(sourceText);
			string searchText = Parser.Replace(sourceText, "").Trim();
			List<FindSearchField> fields = ParseFields(matches, fieldResolver);
			return new FindSearchParserResults(sourceText, ParseSearchText(searchText), fields);
		}
		string[] ParseSearchText(string searchText) {
			if(string.IsNullOrEmpty(searchText)) return new string[0];
			List<string> res = new List<string>();
			MatchCollection matches = QueryParser.Matches(searchText);
			foreach(Match match in matches) {
				if(!match.Success) continue;
				var m = RemoveQuotes(match.Value);
				if(m == "+" || m == "-") continue;
				res.Add(m);
			}
			return res.ToArray();
		}
		protected virtual List<FindSearchField> ParseFields(MatchCollection matches, FindSearchFieldResolveDelegate fieldResolver) {
			List<FindSearchField> res = new List<FindSearchField>();
			Dictionary<string, FindSearchField> hash = new Dictionary<string, FindSearchField>();
			foreach(Match match in matches) {
				Group field = match.Groups["field"];
				Group value = match.Groups["value"];
				if(field == null || field.Length == 0 || value == null || value.Length == 0) continue;
				FindSearchField f = new FindSearchField(ExtractField(field.Value.Trim()), ExtractValue(value.Value.Trim()));
				if(f.Name.Length > 0 && (f.Name[0] == '+' || f.Name[0] == '-')) {
					f.Value = f.Name[0].ToString() + f.Value;
					f.Name = f.Name.Remove(0, 1);
				}
				if(fieldResolver != null) f.Name = fieldResolver(f);
				if(string.IsNullOrEmpty(f.Name)) continue;
				FindSearchField fh;
				hash.TryGetValue(f.Name, out fh);
				if(fh != null) {
					fh.AddValue(f.Value);
					continue;
				}
				hash[f.Name] = f;
				res.Add(f);
			}
			return res;
		}
		string ExtractField(string field) {
			if(field.EndsWith(":")) field = field.Substring(0, field.Length - 1);
			return RemoveQuotes(field);
		}
		string ExtractValue(string text) { return RemoveQuotes(text); }
		string RemoveQuotes(string text) {
			if(string.IsNullOrEmpty(text) || text.Length < 2) return text;
			int qIndex = text[0] == '-' || text[0] == '+' ? 1 : 0;
			if(qIndex > 0 && text.Length < 3) return text;
			if(text[qIndex] == '"' && text[text.Length - 1] == '"') {
				return string.Concat(qIndex == 0 ? "" : text[0].ToString(), text.Substring(qIndex + 1, text.Length - 2 - qIndex));
			}
			return text;
		}
		public static string GetWordFirstLetters(string text) {
			if(string.IsNullOrEmpty(text) || text.Length < 2) return text;
			StringBuilder sb = new StringBuilder();
			bool shouldAdd = true;
			for(int n = 0; n < text.Length; n++) {
				char ch = text[n];
				if(shouldAdd && !char.IsWhiteSpace(ch)) sb.Append(ch);
				shouldAdd = char.IsWhiteSpace(ch) || !char.IsLetterOrDigit(ch);
				if(shouldAdd && n == text.Length - 1 && !char.IsWhiteSpace(ch)) sb.Append(ch);
			}
			return sb.ToString();
		}
		public static void AppendColumnPrefixes(string[] columns) {
			if(columns == null) return;
			for(int n = 0; n < columns.Length; n++) {
				columns[n] = AppendColumnPrefix(columns[n]);
			}
		}
		public static string AppendColumnPrefix(string field) {
			return DxFtsContainsHelper.DxFtsPropertyPrefix + field;
		}
		string DefaultOnFindResolveColumnName(FindSearchField field, ICollection columns) {
			IDataColumnInfo col = ColumnByCaption(columns, field.Name);
			if(col != null) return col.FieldName;
			return string.Empty;
		}
		internal IDataColumnInfo ColumnByCaption(ICollection columns, string caption) {
			if(string.IsNullOrEmpty(caption)) return null;
			IDataColumnInfo res = ColumnByCaption(columns, caption, false, false, false);
			caption = caption.ToLower();
			if(res == null) res = ColumnByCaption(columns, caption, true, false, false);
			if(res == null) res = ColumnByCaption(columns, caption, true, true, false);
			if(res == null) res = ColumnByCaption(columns, caption, true, false, true);
			return res;
		}
		internal IDataColumnInfo ColumnByCaption(ICollection columns, string caption, bool toLower, bool firstLetters, bool useStartWith) {
			if(string.IsNullOrEmpty(caption)) return null;
			foreach(IDataColumnInfo column in columns) {
				string cc = column.Caption;
				if(toLower) cc = cc.ToLower();
				if(firstLetters) cc = FindSearchParser.GetWordFirstLetters(cc);
				if(useStartWith && cc.StartsWith(caption)) return column;
				if(caption == cc) return column;
			}
			return null;
		}
	}
	public delegate string FindSearchFieldResolveDelegate(FindSearchField field);
	public class FindSearchField {
		FindColumnInfo column;
		string[] _values;
		public FindSearchField(string name, string value) : this(name, new string[] { value == null ? "" : value }) { }
		public FindSearchField(string name, string[] values) {
			if(!string.IsNullOrEmpty(name)) this.column = new FindColumnInfo() { Column = null, PropertyName = name};
			this._values = values;
			UpdateCase();
		}
		void UpdateCase() {
			for(int n = 0; n < _values.Length; n++) {
				_values[n] = _values[n].ToLower();
			}
		}
		public FindColumnInfo Column {
			get { return column; }
			set {
				column = value; 
			}
		}
		public IDataColumnInfo ColumnInfo {
			get { return column == null ? null : column.Column; }
			set {
				column = new FindColumnInfo() { Column = value, PropertyName = Name };
			}
		}
		public string Name { 
			get { return column == null ? "" : column.PropertyName; }
			set {
				if(string.IsNullOrEmpty(value)) {
					column = null;
					return;
				}
				column = new FindColumnInfo() { PropertyName = value };
			}
		}
		public string Value { 
			get { return Values.Length == 0 ? string.Empty : Values[0]; }
			set {
				if(value == null) value = string.Empty;
				if(Values.Length == 0) Values = new string[1];
				Values[0] = value;
			}
		}
		public string[] Values {
			get { return _values; }
			set {
				if(value == null) value = new string[0];
				_values = value;
			}
		}
		internal void AddValue(string value) {
			if(string.IsNullOrEmpty(value)) return;
			value = value.ToLower();
			string[] newValues = new string[Values.Length + 1];
			Array.Copy(Values, 0, newValues, 0, Values.Length);
			newValues[newValues.Length - 1] = value;
			this._values = newValues;
		}
	}
	public class FindSearchParserResults {
		FindColumnInfo[] columnNames;
		string sourceText;
		string[] searchText;
		List<FindSearchField> fields;
		public FindSearchParserResults(string sourceText, string[] searchText, List<FindSearchField> fields) {
			this.sourceText = sourceText;
			for(int n = 0; n < searchText.Length; n++) {
				searchText[n] = searchText[n].ToLower();
			}
			this.searchText = searchText;
			this.fields = fields;
			this.columnNames = new FindColumnInfo[0];
		}
		public FindColumnInfo[] ColumnNames { get { return columnNames; } set { columnNames = value; } }
		public void AppendColumnFieldPrefixes() {
			for(int n = 0; n < Fields.Count; n++) {
				Fields[n].Name = FindSearchParser.AppendColumnPrefix(Fields[n].Name);
			}
			for(int n = 0; n < ColumnNames.Length; n++) {
				ColumnNames[n].PropertyName = FindSearchParser.AppendColumnPrefix(ColumnNames[n].PropertyName);
			}
		}
		public FindSearchField this[int index] { get { return Fields[index]; } }
		public List<FindSearchField> Fields { get { return fields; } }
		public string[] SearchTexts { get { return searchText; } }
		public string SearchText { get { return string.Join(" ", searchText); } }
		public string SourceText { get { return sourceText; } }
		public int FieldCount { get { return fields == null ? 0 : fields.Count; } }
		public string GetMatchedText(string field, string text) {
			if(string.IsNullOrEmpty(text)) return string.Empty;
			if(FieldCount == 0) return GetMatchedSearchText(text);
			string[] cachedInfo = GetFieldCache(field);
			if(cachedInfo != null) return GetFieldMatchedText(field, text, cachedInfo);
			return GetMatchedSearchText(text);
		}
		string GetMatchedSearchText(string text) {
			if(SearchTexts.Length == 0) return string.Empty;
			foreach(string s in SearchTexts) {
				if(s.Length == 0) continue;
				if(s[0] == '-') continue;
				string vt = s[0] == '+' ? s.Substring(1) : s;
				if(text.IndexOf(vt, StringComparison.CurrentCultureIgnoreCase) > -1) return vt;
			}
			return string.Empty;
		}
		string[] GetFieldCache(string field) {
			EnsureFieldsCache();
			string[] result;
			cache.TryGetValue(field, out result);
			return result;
		}
		public bool ContainsField(string field) { 
			return GetFieldCache(field) != null;
		}
		public string[] RemoveFieldColumns(string[] columns) {
			if(columns == null || FieldCount == 0) return columns;
			List<string> res = new List<string>();
			for(int n = 0; n < columns.Length; n++) {
				if(!ContainsField(columns[n])) res.Add(columns[n]);
			}
			return res.ToArray();
		}
		public bool IsAllowHighlight(string field, string text) {
			string match = GetMatchedText(field, text);
			if(string.IsNullOrEmpty(match)) return false;
			return true;
		}
		string GetFieldMatchedText(string field, string text, string[] cachedInfo) {
			if(cachedInfo == null) return string.Empty;
			string[] values = cachedInfo;
			if(values.Length == 1) {
				if(values[0][0] == '-') return string.Empty;
				string vt = values[0][0] == '+' ? values[0].Substring(1) : values[0];
				if(text.IndexOf(vt, StringComparison.CurrentCultureIgnoreCase)  > -1) return vt;
			}
			else {
				foreach(string fieldSearch in values) {
					if(fieldSearch[0] == '-') continue;
					string vt = fieldSearch[0] == '+' ? fieldSearch.Substring(1) : fieldSearch;
					if(text.IndexOf(vt, StringComparison.CurrentCultureIgnoreCase) > -1) return vt;
				}
			}
			return string.Empty;
		}
		Dictionary<string, string[]> cache;
		void EnsureFieldsCache() {
			if(cache != null) return;
			cache = new Dictionary<string, string[]>();
			foreach(FindSearchField field in Fields) {
				cache[field.Name] = field.Values;
			}
		}
	}
}
