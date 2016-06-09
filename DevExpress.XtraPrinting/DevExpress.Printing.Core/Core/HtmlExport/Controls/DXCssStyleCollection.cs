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
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public sealed class DXCssStyleCollection {
		const string StyleKey = "style";
		static readonly Regex styleAttribRegex = new Regex(@"\G(\s*(;\s*)*(?<stylename>[^:]+?)\s*:\s*(?<styleval>[^;]*))*\s*(;\s*)*$", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
		Dictionary<DXHtmlTextWriterStyle, string> htmlStyleTable;
		Dictionary<string, string> styleTable;
		DXStateBag state;
		string style;
		public int Count {
			get {
				if(styleTable == null)
					ParseString();
				return styleTable.Count + (htmlStyleTable != null ? htmlStyleTable.Count : 0);
			}
		}
		public string this[string key] {
			get {
				if(styleTable == null)
					ParseString();
				string str = null;
				if(!styleTable.TryGetValue(key, out str)) {
					DXHtmlTextWriterStyle styleKey = DXCssTextWriter.GetStyleKey(key);
					if(styleKey != (DXHtmlTextWriterStyle)(-1))
						str = this[styleKey];
				}
				return str;
			}
			set { Add(key, value); }
		}
		public string this[DXHtmlTextWriterStyle key] {
			get {
				if(htmlStyleTable == null)
					return null;
				string result = null;
				htmlStyleTable.TryGetValue(key, out result);
				return result;
			}
			set { Add(key, value); }
		}
		public ICollection Keys {
			get {
				if(styleTable == null)
					ParseString();
				if(htmlStyleTable == null)
					return styleTable.Keys;
				string[] strArray = new string[styleTable.Count + htmlStyleTable.Count];
				int index = 0;
				foreach(string str in styleTable.Keys)
					strArray[index++] = str;
				foreach(DXHtmlTextWriterStyle style in htmlStyleTable.Keys)
					strArray[index++] = DXCssTextWriter.GetStyleName(style);
				return strArray;
			}
		}
		public string Value {
			get {
				if(state != null)
					return (string)state[StyleKey];
				if(style == null)
					style = BuildString();
				return style;
			}
			set {
				if(state == null)
					style = value;
				else
					state[StyleKey] = value;
				styleTable = null;
			}
		}
		public DXCssStyleCollection()
			: this(null) {
		}
		internal DXCssStyleCollection(DXStateBag state) {
			this.state = state;
		}
		public void Add(string key, string value) {
			if(string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			if(styleTable == null)
				ParseString();
			styleTable[key] = value;
			if(htmlStyleTable != null) {
				DXHtmlTextWriterStyle styleKey = DXCssTextWriter.GetStyleKey(key);
				if(styleKey != (DXHtmlTextWriterStyle)(-1))
					htmlStyleTable.Remove(styleKey);
			}
			if(state != null)
				state[StyleKey] = BuildString();
			style = null;
		}
		public void Add(DXHtmlTextWriterStyle key, string value) {
			if(htmlStyleTable == null)
				htmlStyleTable = new Dictionary<DXHtmlTextWriterStyle, string>();
			htmlStyleTable[key] = value;
			string styleName = DXCssTextWriter.GetStyleName(key);
			if(styleName.Length != 0) {
				if(styleTable == null)
					ParseString();
				styleTable.Remove(styleName);
			}
			if(state != null)
				state[StyleKey] = BuildString();
			style = null;
		}
		public void Clear() {
			styleTable = null;
			htmlStyleTable = null;
			if(state != null)
				state.Remove(StyleKey);
			style = null;
		}
		private void ParseString() {
			styleTable = new Dictionary<string, string>();
			string input = state == null ? style : (string)state[StyleKey];
			if(input != null) {
				Match match = styleAttribRegex.Match(input, 0);
				if(match.Success) {
					CaptureCollection styleNames = match.Groups["stylename"].Captures;
					CaptureCollection styleValues = match.Groups["styleval"].Captures;
					for(int i = 0; i < styleNames.Count; i++) {
						string styleName = styleNames[i].ToString();
						string styleValue = styleValues[i].ToString();
						styleTable[styleName] = styleValue;
					}
				}
			}
		}
		public void Remove(string key) {
			if(styleTable == null)
				ParseString();
			if(styleTable[key] != null) {
				styleTable.Remove(key);
				if(state != null)
					state[StyleKey] = BuildString();
				style = null;
			}
		}
		public void Remove(DXHtmlTextWriterStyle key) {
			if(htmlStyleTable != null) {
				htmlStyleTable.Remove(key);
				if(state != null)
					state[StyleKey] = BuildString();
				style = null;
			}
		}
		internal void Render(DXCssTextWriter writer) {
			if(styleTable != null && styleTable.Count > 0)
				foreach(KeyValuePair<string, string> entry in styleTable)
					writer.WriteAttribute(entry.Key, entry.Value);
			if(htmlStyleTable != null && htmlStyleTable.Count > 0)
				foreach(KeyValuePair<DXHtmlTextWriterStyle, string> entry in htmlStyleTable)
					writer.WriteAttribute(entry.Key, entry.Value);
		}
		internal void Render(DXHtmlTextWriter writer) {
			if(styleTable != null && styleTable.Count > 0)
				foreach(KeyValuePair<string, string> entry in styleTable)
					writer.AddStyleAttribute(entry.Key, entry.Value);
			if(htmlStyleTable != null && htmlStyleTable.Count > 0)
				foreach(KeyValuePair<DXHtmlTextWriterStyle, string> entry in htmlStyleTable)
					writer.AddStyleAttribute(entry.Key, entry.Value);
		}
		string BuildString() {
			if((styleTable == null || styleTable.Count == 0) && (htmlStyleTable == null || htmlStyleTable.Count == 0))
				return null;
			using(StringWriter writer = new StringWriter())
			using(DXCssTextWriter dxWriter = new DXCssTextWriter(writer)) {
				Render(dxWriter);
				return writer.ToString();
			}
		}
	}
}
