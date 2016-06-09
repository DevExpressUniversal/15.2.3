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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Shared strings
		protected internal virtual bool ShouldExportSharedStrings() {
			return Workbook.SharedStringTable.Count != 0;
		}
		protected internal virtual void AddSharedStringPackageContent() {
			if (!ShouldExportSharedStrings())
				return;
			AddPackageContent(@"xl\sharedStrings.xml", ExportSharedStrings());
		}
		protected internal virtual CompressedStream ExportSharedStrings() {
			return CreateXmlContent(GenerateSharedStringsXmlContent);
		}
		protected internal virtual void GenerateSharedStringsXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateSharedStringsContent();
		}
		protected internal virtual void GenerateSharedStringsContent() {
			WriteShStartElement("sst");
			try {
				int count = ExportStyleSheet.SharedStringsIndicies.Count;
				for (int i = 0; i < count; i++) {
					int sstIndex = ExportStyleSheet.SharedStringsIndicies[i];
					ExportSharedStringItem(Workbook.SharedStringTable[sstIndex]);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportSharedStringItem(ISharedStringItem item) {
			WriteShStartElement("si");
			try {
				PlainTextStringItem plainTextStringItem = item as PlainTextStringItem;
				if (plainTextStringItem != null) {
					ExportSharedStringPlainTextItem(plainTextStringItem);
					return;
				}
				FormattedStringItem formattedStringItem = item as FormattedStringItem;
				if (formattedStringItem == null)
					return;
				ExportSharedStringFormattedStringItem(formattedStringItem);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportSharedStringPlainTextItem(PlainTextStringItem plainTextStringItem) {
			WriteShString("t", EncodeXmlCharsXML1_0(plainTextStringItem.Content), true);
		}
		void ExportSharedStringFormattedStringItem(FormattedStringItem formattedStringItem) {
			List<FormattedStringItemPart> innerItems = formattedStringItem.Items;
			int count = innerItems.Count;
			for (int i = 0; i < count; i++) {
				WriteShStartElement("r");
				try {
					FormattedStringItemPart formattedStringItemPart = innerItems[i];
					GenerateRunFontPropertiesContent(formattedStringItemPart.Font, i);
					WriteShString("t", EncodeXmlCharsXML1_0(formattedStringItemPart.Content), true);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal virtual void GenerateRunFontPropertiesContent(RunFontInfo font, int runIndex) {
			RunFontInfo defaultItem = Workbook.GetDefaultRunFontInfoZeroItemInCache();
			if (Object.Equals(defaultItem, font) && runIndex == 0)
				return;
			WriteShStartElement("rPr");
			try {
				WriteElementWithValAttr("rFont", font.Name);
				if (font.Charset != defaultItem.Charset)
					WriteElementWithValAttr("charset", font.Charset);
				if (font.FontFamily != defaultItem.FontFamily)
					WriteElementWithValAttr("family", font.FontFamily);
				if (font.Bold != defaultItem.Bold)
					WriteElementWithValAttr("b", font.Bold);
				if (font.Italic != defaultItem.Italic)
					WriteElementWithValAttr("i", font.Italic);
				if (font.StrikeThrough != defaultItem.StrikeThrough)
					WriteElementWithValAttr("strike", font.StrikeThrough);
				if (font.Outline != defaultItem.Outline)
					WriteElementWithValAttr("outline", font.Outline);
				if (font.Shadow != defaultItem.Shadow)
					WriteElementWithValAttr("shadow", font.Shadow);
				if (font.Condense != defaultItem.Condense)
					WriteElementWithValAttr("condense", font.Condense);
				if (font.Extend != defaultItem.Extend)
					WriteElementWithValAttr("extend", font.Extend);
				ColorModelInfo colorInfo = this.Workbook.Cache.ColorModelInfoCache[font.ColorIndex];
				WriteColor(colorInfo, "color");
				if (font.Size != defaultItem.Size) {
					double sz = Math.Round(font.Size, 2);
					WriteElementWithValAttr("sz", sz.ToString(CultureInfo.InvariantCulture));
				}
				if (font.Underline != defaultItem.Underline)
					WriteElementWithValAttr("u", ConvertUnderlineType(font.Underline));
				if (font.Script != defaultItem.Script)
					WriteElementWithValAttr("vertAlign", ConvertVerticalAlignment(font.Script));
				if (font.SchemeStyle != defaultItem.SchemeStyle)
					WriteElementWithValAttr("scheme", ConvertFontScheme(font.SchemeStyle));
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteColor(ColorModelInfo info, string shStartElement, string ns) {
			ColorModelInfo defaultItem = Workbook.Cache.ColorModelInfoCache.DefaultItem;
			if (Object.ReferenceEquals(defaultItem, info))
				return;
			WriteStartElement(shStartElement, ns);
			try {
				if (info.Auto != defaultItem.Auto)
					WriteBoolValue("auto", info.Auto);
				if (info.ColorIndex != defaultItem.ColorIndex)
					WriteIntValue("indexed", info.ColorIndex);
				if (info.Rgb != defaultItem.Rgb)
					WriteStringValue("rgb", ConvertARGBColorToString(info.Rgb));
				if (info.Theme != defaultItem.Theme)
					WriteIntValue("theme", info.Theme.ToInt());
				if (info.Tint != defaultItem.Tint)
					WriteStringValue("tint", info.Tint.ToString(Workbook.DataContext.Culture.NumberFormat));
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteColor(ColorModelInfo info, string shStartElement) {
			WriteColor(info, shStartElement, SpreadsheetNamespace);
		}
		#endregion
		protected string ConvertUnderlineType(XlUnderlineType value) {
			return underlineTypeTable[value];
		}
		protected string ConvertVerticalAlignment(XlScriptType value) {
			return verticalAlignmentRunTypeTable[value];
		}
		protected string ConvertFontScheme(XlFontSchemeStyles value) {
			return fontSchemeStyleTable[value];
		}
		protected internal virtual string ConvertARGBColorToString(Color value) {
			return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)value.A, (int)value.R, (int)value.G, (int)value.B);
		}
		#region Translation tables
		internal static readonly Dictionary<XlUnderlineType, string> underlineTypeTable = CreateUnderlineTypeTable();
		static Dictionary<XlScriptType, string> verticalAlignmentRunTypeTable = CreateVerticalAlignmentRunTypeTable();
		static Dictionary<XlFontSchemeStyles, string> fontSchemeStyleTable = CreateFontSchemeStyleTable();
		static Dictionary<XlUnderlineType, string> CreateUnderlineTypeTable() {
			Dictionary<XlUnderlineType, string> result = new Dictionary<XlUnderlineType, string>();
			result.Add(XlUnderlineType.Single, "single");
			result.Add(XlUnderlineType.Double, "double");
			result.Add(XlUnderlineType.SingleAccounting, "singleAccounting");
			result.Add(XlUnderlineType.DoubleAccounting, "doubleAccounting");
			result.Add(XlUnderlineType.None, "none");
			return result;
		}
		static Dictionary<XlScriptType, string> CreateVerticalAlignmentRunTypeTable() {
			Dictionary<XlScriptType, string> result = new Dictionary<XlScriptType, string>();
			result.Add(XlScriptType.Baseline, "baseline");
			result.Add(XlScriptType.Subscript, "subscript");
			result.Add(XlScriptType.Superscript, "superscript");
			return result;
		}
		static Dictionary<XlFontSchemeStyles, string> CreateFontSchemeStyleTable() {
			Dictionary<XlFontSchemeStyles, string> result = new Dictionary<XlFontSchemeStyles, string>();
			result.Add(XlFontSchemeStyles.None, "none");
			result.Add(XlFontSchemeStyles.Minor, "minor");
			result.Add(XlFontSchemeStyles.Major, "major");
			return result;
		}
		#endregion
	}
}
