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
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xlsx {
	using DevExpress.Export.Xl;
	class XlString : IXlString {
		public XlString(string text) {
			Text = text;
		}
		public string Text { get; private set; }
		public bool IsPlainText { get { return true; } }
	}
	public class SimpleSharedStringTable {
		const int initialCapacity = 16384;
		readonly Dictionary<string, int> textIndexTable;
		readonly Dictionary<XlRichTextString, int> richTextIndexTable;
		readonly List<IXlString> items;
		public SimpleSharedStringTable() {
			this.textIndexTable = new Dictionary<string, int>(initialCapacity);
			this.richTextIndexTable = new Dictionary<XlRichTextString, int>(initialCapacity);
			this.items = new List<IXlString>(initialCapacity);
		}
		public int Count { get { return items.Count; } }
		public IList<IXlString> StringList { get { return items; } }
		public int RegisterString(string text) {
			if (text == null)
				text = String.Empty;
			int index;
			if(!textIndexTable.TryGetValue(text, out index)) {
				index = items.Count;
				textIndexTable.Add(text, index);
				items.Add(new XlString(text));
			}
			return index;
		}
		public int RegisterString(XlRichTextString text) {
			if(text == null)
				return RegisterString(string.Empty);
			int index;
			if(!richTextIndexTable.TryGetValue(text, out index)) {
				index = items.Count;
				richTextIndexTable.Add(text, index);
				items.Add(text);
			}
			return index;
		}
		public void Clear() {
			textIndexTable.Clear();
			richTextIndexTable.Clear();
			items.Clear();
		}
	}
	partial class XlsxDataAwareExporter {
		protected internal virtual void AddSharedStringContent() {
			if (!ShouldExportSharedStrings())
				return;
			BeginWriteXmlContent();
			WriteShStartElement("sst");
			try {
				int count = sharedStringTable.Count;
				IList<IXlString> stringList = sharedStringTable.StringList;
				for (int i = 0; i < count; i++)
					ExportSharedStringItem(stringList[i]);
			}
			finally {
				WriteShEndElement();
			}
			AddPackageContent(@"xl\sharedStrings.xml", EndWriteXmlContent());
			Builder.OverriddenContentTypes.Add("/xl/sharedStrings.xml", XlsxPackageBuilder.SharedStringsContentType);
			Builder.WorkbookRelations.Add(new OpenXmlRelation(Builder.WorkbookRelations.GenerateId(), "sharedStrings.xml", XlsxPackageBuilder.RelsSharedStringsNamespace));
		}
		protected internal virtual bool ShouldExportSharedStrings() {
			return sharedStringTable.Count != 0;
		}
		void ExportSharedStringItem(IXlString item) {
			WriteShStartElement("si");
			try {
				if(item.IsPlainText)
					WriteShString("t", XmlBasedExporterUtils.Instance.EncodeXmlCharsXML1_0(item.Text), true);
				else
					ExportRichTextString(item as XlRichTextString);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportRichTextString(XlRichTextString richText) {
			IList<XlRichTextRun> runs = richText.Runs;
			int count = runs.Count;
			for(int i = 0; i < count; i++) {
				WriteShStartElement("r");
				try {
					XlRichTextRun textRun = runs[i];
					GenerateTextRunPropertiesContent(textRun.Font);
					WriteShString("t", XmlBasedExporterUtils.Instance.EncodeXmlCharsXML1_0(textRun.Text), true);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		void GenerateTextRunPropertiesContent(XlFont font) {
			if(font == null)
				return;
			WriteShStartElement("rPr");
			try {
				WriteElementWithValAttr("rFont", font.Name);
				if(font.Charset != defaultFont.Charset)
					WriteElementWithValAttr("charset", font.Charset);
				if(font.FontFamily != defaultFont.FontFamily)
					WriteElementWithValAttr("family", (int)font.FontFamily);
				if(font.Bold != defaultFont.Bold)
					WriteElementWithValAttr("b", font.Bold);
				if(font.Italic != defaultFont.Italic)
					WriteElementWithValAttr("i", font.Italic);
				if(font.StrikeThrough != defaultFont.StrikeThrough)
					WriteElementWithValAttr("strike", font.StrikeThrough);
				if(font.Outline != defaultFont.Outline)
					WriteElementWithValAttr("outline", font.Outline);
				if(font.Shadow != defaultFont.Shadow)
					WriteElementWithValAttr("shadow", font.Shadow);
				if(font.Condense != defaultFont.Condense)
					WriteElementWithValAttr("condense", font.Condense);
				if(font.Extend != defaultFont.Extend)
					WriteElementWithValAttr("extend", font.Extend);
				if(!font.Color.IsEmpty)
					WriteColor(font.Color, "color");
				if(font.Size != defaultFont.Size) {
					double sz = Math.Round(font.Size, 2);
					WriteElementWithValAttr("sz", sz.ToString(CultureInfo.InvariantCulture));
				}
				if(font.Underline != defaultFont.Underline)
					WriteElementWithValAttr("u", underlineTypeTable[font.Underline]);
				if(font.Script != defaultFont.Script)
					WriteElementWithValAttr("vertAlign", verticalAlignmentRunTypeTable[font.Script]);
				if(font.SchemeStyle != defaultFont.SchemeStyle)
					WriteElementWithValAttr("scheme", fontSchemeStyleTable[font.SchemeStyle]);
			}
			finally {
				WriteShEndElement();
			}
		}
		#region WriteElementWithValAttr
		void WriteElementWithValAttr(string tag, bool val) {
			WriteShStartElement(tag);
			try {
				WriteBoolValue("val", val);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteElementWithValAttr(string tag, int val) {
			WriteShStartElement(tag);
			try {
				WriteIntValue("val", val);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteElementWithValAttr(string tag, string val) {
			WriteShStartElement(tag);
			try {
				WriteStringAttr(null, "val", null, val);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
	}
}
