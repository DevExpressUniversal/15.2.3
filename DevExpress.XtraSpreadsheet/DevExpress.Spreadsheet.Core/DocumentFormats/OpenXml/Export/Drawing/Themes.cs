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
using System.IO;
using System.Xml;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office;
using DevExpress.Utils.Zip;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ExportThemes
		#region Translation tables
		static readonly Dictionary<ThemeColorIndex, string> themeColorIndexTranslationTable = CreateThemeColorIndexTranslationTable();
		static Dictionary<ThemeColorIndex, string> CreateThemeColorIndexTranslationTable() {
			Dictionary<ThemeColorIndex, string> result = new Dictionary<ThemeColorIndex, string>();
			result.Add(ThemeColorIndex.Dark1, "dk1");
			result.Add(ThemeColorIndex.Dark2, "dk2");
			result.Add(ThemeColorIndex.Light1, "lt1");
			result.Add(ThemeColorIndex.Light2, "lt2");
			result.Add(ThemeColorIndex.Accent1, "accent1");
			result.Add(ThemeColorIndex.Accent2, "accent2");
			result.Add(ThemeColorIndex.Accent3, "accent3");
			result.Add(ThemeColorIndex.Accent4, "accent4");
			result.Add(ThemeColorIndex.Accent5, "accent5");
			result.Add(ThemeColorIndex.Accent6, "accent6");
			result.Add(ThemeColorIndex.Hyperlink, "hlink");
			result.Add(ThemeColorIndex.FollowedHyperlink, "folHlink");
			return result;
		}
		#endregion
		protected internal virtual CompressedStream ExportThemes() {
			return CreateXmlContent(GenerateThemesXmlContent);
		}
		protected internal virtual void GenerateThemesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateThemesContent();
		}
		protected internal virtual void GenerateThemesContent() {
			WriteStartElement("a", "theme", DrawingMLNamespace);
			try {
				WriteStringAttr("xmlns", "a", null, DrawingMLNamespace);
				WriteStringValue("name", Workbook.OfficeTheme.Name);
				GenerateThemeElementsContent();
			} finally {
				WriteEndElement();
			}
		}
		void GenerateThemeElementsContent() {
			WriteStartElement("themeElements", DrawingMLNamespace);
			try {
				GenerateThemeColorSchemesContent();
				GenerateThemeFontSchemesContent();
				GenerateThemeFormatSchemesContent();
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateThemeColorSchemesContent() {
			WriteStartElement("clrScheme", DrawingMLNamespace);
			try {
				WriteStringValue("name", Workbook.OfficeTheme.Colors.Name);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Dark1, Workbook.OfficeTheme.Colors.Dark1);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Light1, Workbook.OfficeTheme.Colors.Light1);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Dark2, Workbook.OfficeTheme.Colors.Dark2);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Light2, Workbook.OfficeTheme.Colors.Light2);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent1, Workbook.OfficeTheme.Colors.Accent1);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent2, Workbook.OfficeTheme.Colors.Accent2);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent3, Workbook.OfficeTheme.Colors.Accent3);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent4, Workbook.OfficeTheme.Colors.Accent4);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent5, Workbook.OfficeTheme.Colors.Accent5);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Accent6, Workbook.OfficeTheme.Colors.Accent6);
				GenerateThemeColorSchemeContent(ThemeColorIndex.Hyperlink, Workbook.OfficeTheme.Colors.Hyperlink);
				GenerateThemeColorSchemeContent(ThemeColorIndex.FollowedHyperlink, Workbook.OfficeTheme.Colors.FollowedHyperlink);
			} finally {
				WriteEndElement();
			}
		}
		void GenerateThemeColorSchemeContent(ThemeColorIndex index, DrawingColor color) {
			WriteStartElement(themeColorIndexTranslationTable[index], DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(color);
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateThemeFontSchemesContent() {
			WriteStartElement("fontScheme", DrawingMLNamespace);
			try {
				WriteStringValue("name", Workbook.OfficeTheme.FontScheme.Name);
				GenerateThemeFontSchemePartContent(Workbook.OfficeTheme.FontScheme.MajorFont, "majorFont");
				GenerateThemeFontSchemePartContent(Workbook.OfficeTheme.FontScheme.MinorFont, "minorFont");
			} finally {
				WriteEndElement();
			}
		}
		void GenerateThemeFontSchemePartContent(ThemeFontSchemePart scheme, string tagName) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				GenerateDrawingTextFontContent(scheme.Latin, "latin");
				GenerateDrawingTextFontContent(scheme.EastAsian, "ea");
				GenerateDrawingTextFontContent(scheme.ComplexScript, "cs");
				GenerateSupplementalFontsContent(scheme);
			} finally {
				WriteEndElement();
			}
		}
		void GenerateSupplementalFontsContent(ThemeFontSchemePart scheme) {
			Dictionary<string, string> fonts = scheme.SupplementalFonts;
			foreach (KeyValuePair<string, string> info in fonts)
				GenerateThemeSupplementalFontContent(info.Key, info.Value);
		}
		void GenerateThemeSupplementalFontContent(string script, string typeface) {
			WriteStartElement("font", DrawingMLNamespace);
			try {
				WriteStringValue("script", script);
				WriteStringValue("typeface", typeface);
			} finally {
				WriteEndElement();
			}
		}
		void GenerateThemeFormatSchemesContent() {
			WriteStartElement("fmtScheme", DrawingMLNamespace);
			try {
				ThemeFormatScheme scheme = Workbook.OfficeTheme.FormatScheme;
				WriteStringValue("name", scheme.Name, !String.IsNullOrEmpty(scheme.Name));
				GenerateDrawingListContent("fillStyleLst", scheme.FillStyleList, GenerateDrawingFillContent);
				GenerateDrawingListContent("lnStyleLst", scheme.LineStyleList, GenerateOutlineContent);
				GenerateDrawingListContent("effectStyleLst", scheme.EffectStyleList, GenerateDrawingEffectStyleContent);
				GenerateDrawingListContent("bgFillStyleLst", scheme.BackgroundFillStyleList, GenerateDrawingFillContent);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingListContent<T>(string tagName, List<T> list, Action<T> action) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				int count = list.Count;
				for (int i = 0; i < count; i++)
					action(list[i]);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
