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
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlStyleHelper
	public class HtmlStyleHelper {
		#region SpecialCharactersReplaceTable
		class SpecialCharactersReplaceTable : Dictionary<char, string> {
		}
		#endregion
		static readonly SpecialCharactersReplaceTable specialCharactersReplaceTable = CreateSpecialCharacterReplaceTable();
		static SpecialCharactersReplaceTable CreateSpecialCharacterReplaceTable() {
			SpecialCharactersReplaceTable replaceTable = new SpecialCharactersReplaceTable();
			replaceTable.Add('&', "&amp;");
			replaceTable.Add(Characters.Hyphen, "&shy;");
			HtmlSpecialSymbolTable table = HtmlImporter.CreateHtmlSpecialSymbolTable();
			foreach (string key in table.Keys) {
				if (key == "amp")
					continue;
				if (key == "apos")
					replaceTable.Add(table[key], "&#39;"); 
				else if (key == "thetasy")
					replaceTable.Add(table[key], "&#977;"); 
				else
					replaceTable.Add(table[key], "&" + key + ";");
			}
			replaceTable.Add(Characters.LineBreak, "<br/>");
			replaceTable.Add(Characters.PageBreak, " ");
			replaceTable.Add(Characters.ColumnBreak, " ");
			return replaceTable;
		}
		readonly StringBuilder preProcessHtmlContentTextStringBuilder;
		readonly HtmlFastCharacterMultiReplacement replacement;
		public HtmlStyleHelper() {
			this.preProcessHtmlContentTextStringBuilder = new StringBuilder();
			this.replacement = new HtmlFastCharacterMultiReplacement(preProcessHtmlContentTextStringBuilder);
		}
		public string GetHtmlSizeInPoints(float size) {
			return size.ToString(CultureInfo.InvariantCulture) + "pt";
		}
		public string GetHtmlSizeInPercents(int size) {
			return size.ToString(CultureInfo.InvariantCulture) + '%';
		}
		public string PreprocessHtmlContentText(string text, string tabMarker) {
			return replacement.PerformReplacements(text, specialCharactersReplaceTable, tabMarker);
		}
		readonly ChunkedStringBuilder createCssStyleStringCollectionStringBuilder = new ChunkedStringBuilder();
		internal string CreateCssStyle(StringCollection attributes, char separator) {
			int count = attributes.Count;
			int attributesWritten = 0;
			for (int i = 0; i < count; i++) {
				string attribute = attributes[i].Trim();
				if (!String.IsNullOrEmpty(attribute)) {
					if (attributesWritten > 0)
						createCssStyleStringCollectionStringBuilder.Append(separator);
					createCssStyleStringCollectionStringBuilder.Append(attribute);
					attributesWritten++;
				}
			}
			string result = createCssStyleStringCollectionStringBuilder.ToString();
			createCssStyleStringCollectionStringBuilder.Clear();
			return result;
		}
		readonly ChunkedStringBuilder createCssStyleStyleCollectionStringBuilder = new ChunkedStringBuilder();
		internal string CreateCssStyle(DXCssStyleCollection attributes, char separator) {
			int attributesWritten = 0;
			foreach (string key in attributes.Keys) {
				string attribute = attributes[key].Trim();
				if (!String.IsNullOrEmpty(attribute)) {
					if (attributesWritten > 0)
						createCssStyleStyleCollectionStringBuilder.Append(separator);
					createCssStyleStyleCollectionStringBuilder.Append(key);
					createCssStyleStyleCollectionStringBuilder.Append(':');
					createCssStyleStyleCollectionStringBuilder.Append(attribute);
					attributesWritten++;
				}
			}
			string result = createCssStyleStyleCollectionStringBuilder.ToString();
			createCssStyleStyleCollectionStringBuilder.Clear();
			return result;
		}
	}
	#endregion
	#region HtmlFastCharacterMultiReplacement
	public class HtmlFastCharacterMultiReplacement : FastCharacterMultiReplacement {
		public HtmlFastCharacterMultiReplacement(StringBuilder stringBuilder)
			: base(stringBuilder) {
		}
		public ReplacementInfo CreateReplacementInfo(string text, Dictionary<char, string> replaceTable, string tabMarker) {
			if (tabMarker == "\t")
				return CreateReplacementInfo(text, replaceTable);
			ReplacementInfo result = null;
			for (int i = text.Length - 1; i >= 0; i--) {
				string replaceWith;
				if (replaceTable.TryGetValue(text[i], out replaceWith)) {
					if (result == null)
						result = new ReplacementInfo();
					result.Add(i, replaceWith);
				}
				else if (text[i] == '\t') {
					if (result == null)
						result = new ReplacementInfo();
					result.Add(i, tabMarker);
				}
			}
			return result;
		}
		public string PerformReplacements(string text, Dictionary<char, string> replaceTable, string tabMarker) {
			return PerformReplacements(text, CreateReplacementInfo(text, replaceTable, tabMarker));
		}
	}
	#endregion
}
