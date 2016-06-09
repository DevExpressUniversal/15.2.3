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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Import.OpenDocument;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !DXPORTABLE
using System.Runtime.Serialization;
using System.Diagnostics;
#endif
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
#region OpenDocumentHelper
	public class OpenDocumentHelper : DevExpress.Office.OfficeOpenDocumentHelper {
		public OpenDocumentHelper(bool val) : base(val) {
		}
#region Translation Tables
		static readonly TranslationTable<NumberingFormat> numberingFormatTable = CreateNumberFormatsTable();
		static readonly TranslationTable<TabLeaderType> tabLeaderTable = CreateTabLeaderTable();
		static readonly TranslationTable<TabAlignmentType> tabAlignmentTypeTable = CreateTabAlignmentTypeTable();
		static readonly TranslationTable<ListNumberAlignment> listNumberAlignmentTable = CreateListNumberAlignmentTable();
		static readonly TranslationTable<Char> listNumberSeparatorTable = CreateListNumberSeparatorTable();
		static readonly TranslationTable<BorderLineStyle> lineTypeTable = CreateLineTypeTable();
		public static TranslationTable<NumberingFormat> NumberingFormatTable { get { return numberingFormatTable; } }
		public static TranslationTable<TabLeaderType> TabLeaderTable { get { return tabLeaderTable; } }
		public static TranslationTable<TabAlignmentType> TabAlignmentTypeTable { get { return tabAlignmentTypeTable; } }
		public static TranslationTable<ListNumberAlignment> ListNumberAlignmentTable { get { return listNumberAlignmentTable; } }
		public static TranslationTable<BorderLineStyle> LineTypeTable { get { return lineTypeTable; } }
		static TranslationTable<NumberingFormat> CreateNumberFormatsTable() {
			TranslationTable<NumberingFormat> result = new TranslationTable<NumberingFormat>();
			result.Add(NumberingFormat.Decimal, "1");
			result.Add(NumberingFormat.LowerLetter, "a");
			result.Add(NumberingFormat.UpperLetter, "A");
			result.Add(NumberingFormat.LowerRoman, "i");
			result.Add(NumberingFormat.UpperRoman, "I");
			return result;
		}
		static TranslationTable<TabLeaderType> CreateTabLeaderTable() {
			TranslationTable<TabLeaderType> result = new TranslationTable<TabLeaderType>();
			result.Add(TabLeaderType.Dots, String.Empty + Characters.Dot);
			result.Add(TabLeaderType.EqualSign, "=");
			result.Add(TabLeaderType.Hyphens, "-");
			result.Add(TabLeaderType.MiddleDots, String.Empty + Characters.MiddleDot);
			result.Add(TabLeaderType.ThickLine, "" + Characters.EmDash);
			result.Add(TabLeaderType.Underline, "" + Characters.Underscore);
			result.Add(TabLeaderType.None, "");
			return result;
		}
		static TranslationTable<TabAlignmentType> CreateTabAlignmentTypeTable() {
			TranslationTable<TabAlignmentType> result = new TranslationTable<TabAlignmentType>();
			result.Add(TabAlignmentType.Left, "left");
			result.Add(TabAlignmentType.Center, "center");
			result.Add(TabAlignmentType.Right, "right");
			result.Add(TabAlignmentType.Decimal, "char");
			return result;
		}
		private static TranslationTable<ListNumberAlignment> CreateListNumberAlignmentTable() {
			TranslationTable<ListNumberAlignment> result = new TranslationTable<ListNumberAlignment>();
			result.Add(ListNumberAlignment.Left, "left");
			result.Add(ListNumberAlignment.Right, "right");
			result.Add(ListNumberAlignment.Center, "center");
			return result;
		}
		private static TranslationTable<Char> CreateListNumberSeparatorTable() {
			TranslationTable<Char> result = new TranslationTable<Char>();
			result.Add(Characters.TabMark, "listtab");
			result.Add(' ', "space");
			result.Add(Char.MinValue, "nothing");
			return result;
		}
		static TranslationTable<BorderLineStyle> CreateLineTypeTable() {
			TranslationTable<BorderLineStyle> result = new TranslationTable<BorderLineStyle>();
			result.Add(BorderLineStyle.Double, "double");
			result.Add(BorderLineStyle.None, "none");
			result.Add(BorderLineStyle.Single, "solid");
			result.Add(BorderLineStyle.ThinThickSmallGap, "double");
			result.Add(BorderLineStyle.ThickThinSmallGap, "double");
			result.Add(BorderLineStyle.ThinThickMediumGap, "double");
			result.Add(BorderLineStyle.ThickThinMediumGap, "double");
			result.Add(BorderLineStyle.ThinThickLargeGap, "double");
			result.Add(BorderLineStyle.ThickThinLargeGap, "double");
			result.Add(BorderLineStyle.DoubleWave, "double");
			return result;
		}
#endregion
	}
#endregion
#region ParagraphBreakType
	public enum ParagraphBreakType { None, Page, Column }
#endregion
#region ExportHelper
	public partial class ExportHelper {
		readonly OpenDocumentTextExporter exporter;
		XmlWriter writer;
		OpenDocumentHelper openDocumentHelper;
		public ExportHelper(OpenDocumentTextExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
			this.openDocumentHelper = new OpenDocumentHelper(false);
		}
		public OpenDocumentTextExporter Exporter { get { return exporter; } }
		public XmlWriter Writer { get { return writer; } }
		public float ColumnWeightBase { get { return 1000.0f; } }
		public OpenDocumentHelper OpenDocumentHelper { get { return openDocumentHelper; } }
#region TranslationTables
		static readonly TranslationTable<UnderlineType> underlineTypeTable = CreateUnderlineTypeTable();
		public static TranslationTable<UnderlineType> UnderlineTypeTable { get { return underlineTypeTable; } }
		static TranslationTable<UnderlineType> CreateUnderlineTypeTable() {
			TranslationTable<UnderlineType> result = new TranslationTable<UnderlineType>();
			result.Add(UnderlineType.None, "none");
			result.Add(UnderlineType.Single, "solid");
			result.Add(UnderlineType.Double, "solid");
			result.Add(UnderlineType.Dotted, "dotted");
			result.Add(UnderlineType.Dashed, "dash");
			result.Add(UnderlineType.LongDashed, "long-dash");
			result.Add(UnderlineType.DashDotted, "dot-dash");
			result.Add(UnderlineType.DashDotDotted, "dot-dot-dash");
			result.Add(UnderlineType.DoubleWave, "wave");
			result.Add(UnderlineType.HeavyWave, "wave");
			result.Add(UnderlineType.ThickDashDotDotted, "dot-dot-dash");
			result.Add(UnderlineType.ThickDashDotted, "dot-dash");
			result.Add(UnderlineType.ThickDashed, "dash");
			result.Add(UnderlineType.ThickDotted, "dotted");
			result.Add(UnderlineType.ThickLongDashed, "long-dash");
			result.Add(UnderlineType.ThickSingle, "solid");
			result.Add(UnderlineType.Wave, "wave");
			return result;
		}
		internal static readonly Dictionary<char, string> breaksTable = CreateBreaksTable();
		static Dictionary<char, string> CreateBreaksTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			result.Add(Characters.LineBreak, "%0b");
			return result;
		}
#endregion
#region Write Helpers
#region StartElements
		protected internal virtual void WriteOfficeStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.OfficeNsPrefix, tag, OpenDocumentHelper.OfficeNamespace);
		}
		protected internal virtual void WriteTextStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.TextNsPrefix, tag, OpenDocumentHelper.TextNamespace);
		}
		protected internal virtual void WriteTableStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.TableNsPrefix, tag, OpenDocumentHelper.TableNamespace);
		}
		protected internal virtual void WriteDrawStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.DrawNsPrefix, tag, OpenDocumentHelper.DrawNamespace);
		}
		protected internal virtual void WriteStyleStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.StyleNsPrefix, tag, OpenDocumentHelper.StyleNamespace);
		}
		protected internal virtual void WriteManifestStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.ManifestNsPrefix, tag, OpenDocumentHelper.ManifestNamespace);
		}
		protected internal virtual void WriteDcStartElement(string tag) {
			WriteStartElementCore(OpenDocumentHelper.DcNsPrefix, tag, OpenDocumentHelper.DcNamespace);
		}
		protected internal virtual void WriteStartElementCore(string prefix, string tag, string ns) {
			Writer.WriteStartElement(prefix, tag, ns);
		}
#endregion
#region String Attributes
		protected internal virtual void WriteManifestStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.ManifestNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteStyleStringAttribute(string tag, string value, bool skipEmptyString) {
			if (skipEmptyString && string.IsNullOrEmpty(value))
				return;
			WriteStyleStringAttribute(tag, value);
		}
		protected internal virtual void WriteStyleStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.StyleNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteSvgStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.SvgNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteFoStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.FoNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteDrawStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.DrawNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteTextIntAttribute(string tag, int value) {
			WriteTextStringAttribute(tag, value.ToString());
		}
		protected internal virtual void WriteTextStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.TextNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteTableStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.TableNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteOfficeStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.OfficeNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteXlinkStringAttribute(string tag, string value) {
			WriteStringAttributeCore(OpenDocumentHelper.XlinkNsPrefix, tag, null, value);
		}
		protected internal virtual void WriteStringAttributeCore(string prefix, string tag, string ns, string value) {
			Writer.WriteAttributeString(prefix, tag, ns, value);
		}
#endregion
		protected internal virtual void WriteEndElement() {
			Writer.WriteEndElement();
		}
		protected internal virtual void WriteValue(string val) {
			val = XmlTextHelper.DeleteIllegalXmlCharacters(val);
			if(!String.IsNullOrEmpty(val))
				Writer.WriteValue(val);
		}
#endregion
#region helper  methods
		protected internal void UpdateWriter(XmlWriter writer) {
			this.writer = writer;
		}
		protected internal string ConvertColorToString(Color color) {
			return DXColor.ToHtml(color);
		}
		protected internal string ConvertToPercentString(float multiplierValue) {
			int per = (int)Math.Round(multiplierValue * 100);
			return ConvertPercentToString(per);
		}
		protected internal string ConvertPercentToString(int percent) {
			return String.Format(CultureInfo.InvariantCulture, "{0}%", percent.ToString());
		}
		protected internal string ConvertToCmString(float documentModelValue) {
			double value = exporter.DocumentModel.UnitConverter.ModelUnitsToCentimetersF(documentModelValue);
			return ConvertToStringValue(value, "cm");
		}
		protected internal string ConvertToInchString(float documentModelValue) {
			double value = exporter.DocumentModel.UnitConverter.ModelUnitsToInchesF(documentModelValue);
			return ConvertToStringValue(value, "in");
		}
		protected internal string ConvertToStringValue(double value, string unit) {
			return String.Format(CultureInfo.InvariantCulture, "{0:0.####}{1}", value, unit);
		}
		internal int CalculateDisplayLevelsCount(string displayFormat) {
			MatchCollection matches = Regex.Matches(displayFormat, @"(\{[0-9]\})");
			return matches.Count;
		}
		protected internal string ConvertToNumberingFormatSuffix(string displayFormat) {
			int index = displayFormat.LastIndexOf("}");
			return (index >= 0) ? displayFormat.Substring(index + 1, displayFormat.Length - 1 - index) : string.Empty;
		}
		protected internal string ConvertToNumberingFormatPrefix(string displayFormat) {
			int index = displayFormat.IndexOf("{");
			return (index >= 0) ? displayFormat.Substring(0, index) : string.Empty;
		}
		internal virtual string GetPictureWidth(InlinePictureRun run) {
			return ConvertToInchString(run.ActualSizeF.Width);
		}
		internal virtual string GetPictureHeight(InlinePictureRun run) {
			return ConvertToInchString(run.ActualSizeF.Height);
		}
		internal virtual string ConvertLineSpacingToString(ParagraphLineSpacing type, float lineSpacing) {
			string val = ConvertToInchString(lineSpacing);
			switch (type) {
				case ParagraphLineSpacing.AtLeast:
				case ParagraphLineSpacing.Exactly:
					return val;
				case ParagraphLineSpacing.Double:
					return "200%";
				case ParagraphLineSpacing.Multiple:
					return ConvertToPercentString(lineSpacing);
				case ParagraphLineSpacing.Sesquialteral:
					return "150%";
				case ParagraphLineSpacing.Single:
				default:
					return "100%";
			}
		}
#endregion
#region manifest
		public virtual void WriteDocumentManifest(XmlWriter writer) {
			WriteManifestStartElement("manifest");
			try {
				WriteManifestFileEntry("/", OpenDocumentHelper.ManifestMediaType);
				WriteManifestFileEntry("META-INF/manifest.xml", "text/xml");
				WriteManifestFileEntry("content.xml", "text/xml");
				WriteManifestFileEntry("styles.xml", "text/xml");
				WriteManifestFileEntry("meta.xml", "text/xml");
			}
			finally {
				WriteEndElement();
			}
		}
		public virtual void WriteManifestFileEntry(string fullPath, string mediaType) {
			WriteManifestStartElement("file-entry");
			try {
				WriteManifestStringAttribute("full-path", fullPath);
				WriteManifestStringAttribute("media-type", mediaType);
			}
			finally {
				writer.WriteEndElement();
			}
		}
#endregion
#region document-content
		public virtual void WriteDocumentContentStart() {
			WriteOfficeStartElement(OpenDocumentHelper.ElementDocumentContent);
			WriteDocumentContentNameSpaces();
		}
		protected internal virtual void WriteDocumentContentNameSpaces() {
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.StyleNsPrefix, null, OpenDocumentHelper.StyleNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.TextNsPrefix, null, OpenDocumentHelper.TextNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.FoNsPrefix, null, OpenDocumentHelper.FoNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.SvgNsPrefix, null, OpenDocumentHelper.SvgNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.DrawNsPrefix, null, OpenDocumentHelper.DrawNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.XlinkNsPrefix, null, OpenDocumentHelper.XlinkNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.OOWNsPrefix, null, OpenDocumentHelper.OOWNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.TableNsPrefix, null, OpenDocumentHelper.TableNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.OfficeNsPrefix, null, OpenDocumentHelper.OfficeNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.DcNsPrefix, null, OpenDocumentHelper.DcNamespace);
		}
		public virtual void WriteFontDeclarationStart() {
			WriteOfficeStartElement("font-face-decls");
		}
		public virtual void WriteFontDeclaration(string fontName) {
			WriteStyleStartElement("font-face");
			try {
				WriteStyleStringAttribute("name", fontName);
				WriteSvgStringAttribute("font-family", fontName);
			}
			finally {
				WriteEndElement();
			}
		}
#endregion
		public virtual void WriteDocumentStylesStart() {
			WriteOfficeStartElement(OpenDocumentHelper.ElementDocumentStyles);
		}
		public virtual void WriteDocumentStylesAttributes() {
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.StyleNsPrefix, null, OpenDocumentHelper.StyleNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.TextNsPrefix, null, OpenDocumentHelper.TextNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.FoNsPrefix, null, OpenDocumentHelper.FoNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.XlinkNsPrefix, null, OpenDocumentHelper.XlinkNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.SvgNsPrefix, null, OpenDocumentHelper.SvgNamespace);
			Writer.WriteAttributeString(OpenDocumentHelper.ElementNs, OpenDocumentHelper.TableNsPrefix, null, OpenDocumentHelper.TableNamespace);
		}
		public virtual void WriteDocumentCommonStylesStart() {
			WriteOfficeStartElement("styles");
		}
		public virtual void WriteBodyStart() {
			WriteOfficeStartElement("body");
		}
		public virtual void WriteTextStart() {
			WriteOfficeStartElement("text");
		}
		public virtual void WriteTableStart() {
			WriteTableStartElement("table");
		}
		public virtual void WriteTableRowStart() {
			WriteTableStartElement("table-row");
		}
		public virtual void WriteTableColumnStart() {
			WriteTableStartElement("table-column");
		}
		public virtual void WriteTableCellStart() {
			WriteTableStartElement("table-cell");
		}
		public virtual void WriteCoveredTableCellStart() {
			WriteTableStartElement("covered-table-cell");
		}
		public void WritePageFieldStart() {
			WriteTextStartElement("page-number");
		}
		public void WritePageRefFieldStart() {
			WriteTextStartElement("bookmark-ref");
		}
		public void WritePageCountFieldStart() {
			WriteTextStartElement("page-count");
		}
#region styles
		public virtual void WriteAutoStylesStart() {
			WriteOfficeStartElement("automatic-styles");
		}
		public virtual void WriteStyleStart() {
			WriteStyleStartElement("style");
		}
		internal void WriteDefaultParagraphStyleStart() {
			WriteStyleStartElement("default-style");
			WriteStyleStringAttribute("family", "paragraph");
		}
		internal void WriteDefaultTableStyleStart() {
			WriteStyleStartElement("default-style");
			WriteStyleStringAttribute("family", "table");
		}
		public virtual void WriteDocumentCharacterStyleAttributes(CharacterStyle style) {
			string parentStyleName = style.Parent != null ? style.Parent.StyleName : String.Empty;
			WriteCharacterAutoStyleAttributes(style.StyleName, parentStyleName);
		}
		public virtual void WriteCharacterAutoStyleAttributes(string styleName, string parentStyleName) {
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "text");
			if (!String.IsNullOrEmpty(parentStyleName))
				WriteStyleStringAttribute("parent-style-name", parentStyleName);
		}
		public virtual void WriteParagraphAutoStyleAttributes(Paragraph paragraph, ParagraphStyleInfo info) {
			WriteStyleStringAttribute("name", info.Name);
			WriteStyleStringAttribute("family", "paragraph");
			WriteStyleStringAttribute("parent-style-name", paragraph.ParagraphStyle.StyleName);
			if (!String.IsNullOrEmpty(info.MasterPageName))
				WriteStyleStringAttribute("master-page-name", info.MasterPageName);
			if (!String.IsNullOrEmpty(info.StyleListName))
				WriteStyleStringAttribute("list-style-name", info.StyleListName);
		}
		public virtual void WriteGraphicStyleContent(string styleName) {
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "graphic");
			WriteStyleStringAttribute("parent-style-name", "Graphics");
		}
		public virtual void WriteDocumentParagraphStyleAttributes(ParagraphStyle style) {
			WriteStyleStringAttribute("name", style.StyleName);
			WriteStyleStringAttribute("family", "paragraph");
			style.Tabs.GetTabs();
			if (style.Parent != null)
				WriteStyleStringAttribute("parent-style-name", style.Parent.StyleName);
			if (style.NextParagraphStyle != null)
				WriteStyleStringAttribute("next-style-name", style.NextParagraphStyle.StyleName);
		}
		public virtual void WriteStyleTextPropertiesStart() {
			WriteStyleStartElement("text-properties");
		}
		public virtual void WriteStyleParagraphPropertiesStart() {
			WriteStyleStartElement("paragraph-properties");
		}
		public virtual void WriteStyleTableCellPropertiesStart() {
			WriteStyleStartElement("table-cell-properties");
		}
		public virtual void WriteStyleTableRowPropertiesStart() {
			WriteStyleStartElement("table-row-properties");
		}
		public virtual void WriteStyleTableColumnPropertiesStart() {
			WriteStyleStartElement("table-column-properties");
		}
		public virtual void WriteStyleTablePropertiesStart() {
			WriteStyleStartElement("table-properties");
		}
		public virtual void WriteStyleParagraphPropertiesAttributes(ParagraphProperties properties, ParagraphBreakType breakAfterType) {
			WriteStyleParagraphPropertiesAlignment(properties);
			WriteStyleParagraphPropertiesIndention(properties);
			WriteStyleParagraphLineSpacing(properties);
			WriteParagraphBreak(properties, breakAfterType);
			if (properties.UseOutlineLevel)
				WriteStyleStringAttribute("default-outline-level", Math.Max(0, Math.Min(9, properties.OutlineLevel)).ToString());
			if (properties.UseKeepWithNext)
				WriteFoStringAttribute("keep-with-next", properties.KeepWithNext ? "always" : "auto");
			if (properties.UseKeepLinesTogether)
				WriteFoStringAttribute("keep-together", properties.KeepLinesTogether ? "always" : "auto");
			if (properties.UseWidowOrphanControl && properties.WidowOrphanControl) {
				WriteFoStringAttribute("widows", "2");
				WriteFoStringAttribute("orphans", "2");
			}
			if (properties.UseBackColor && !DXColor.IsTransparentOrEmpty(properties.BackColor)) {
				string backColorValue = ConvertColorToString(properties.BackColor);
				if (!String.IsNullOrEmpty(backColorValue))
					WriteFoStringAttribute("background-color", backColorValue);
			}
		}
		public virtual void WriteStyleTablePropertiesAttributes(TableProperties tableProperties, int parentWidth) {
			if (tableProperties.UseCellSpacing) {
				WriteTableStringAttribute("border-model", tableProperties.CellSpacing.Value > 0 ? "separating" : "collapsing");
			}
			if (tableProperties.PreferredWidth.Type == WidthUnitType.ModelUnits)
				WriteStyleStringAttribute("width", ConvertToCmString(tableProperties.PreferredWidth.Value));
			else if (parentWidth != Int32.MinValue) {
				WriteStyleStringAttribute("width", ConvertToCmString(parentWidth));
			}
			if (tableProperties.PreferredWidth.Type == WidthUnitType.FiftiethsOfPercent) {
				const int fiftieths = 50;
				int percent = tableProperties.PreferredWidth.Value / fiftieths;
				WriteStyleStringAttribute("rel-width", ConvertPercentToString(percent));
			}
			if (tableProperties.UseTableIndent && tableProperties.TableIndent.Value > 0) {
				WriteFoStringAttribute("margin-left", ConvertToCmString(tableProperties.TableIndent.Value));
			}
			WriteTableStringAttribute("align", "margins"); 
			if (tableProperties.UseFloatingPosition && tableProperties.FloatingPosition.TopFromText > 0)
				WriteFoStringAttribute("margin-top", ConvertToCmString(tableProperties.FloatingPosition.TopFromText));
			if (tableProperties.UseFloatingPosition && tableProperties.FloatingPosition.BottomFromText > 0)
				WriteFoStringAttribute("margin-bottom", ConvertToCmString(tableProperties.FloatingPosition.BottomFromText));
		}
		public void WriteStyleTableRowPropertiesAttributes(TableRowProperties properties) {
			if (properties.UseHeight) {
				if (properties.Height.Type == HeightUnitType.Minimum)
					WriteStyleStringAttribute("min-row-height", ConvertToCmString(properties.Height.Value));
				else if (properties.Height.Type == HeightUnitType.Exact)
					WriteStyleStringAttribute("row-height", ConvertToCmString(properties.Height.Value));
			}
			if (properties.UseCantSplit)
				WriteFoStringAttribute("keep-together", "always");
		}
		public virtual void WriteStyleParagraphPropertiesContent(TabFormattingInfo tabs) {
			WriteStyleParagraphPropertiesTabs(tabs);
		}
		public void WriteStyleTableColumnPropertiesContent(int width, bool useOtimalColumnWidth) {
			WriteStyleStringAttribute("column-width", ConvertToCmString(width));
			WriteStyleStringAttribute("rel-column-width", width.ToString()+"*");
			WriteStyleStringAttribute("use-optimal-column-width", useOtimalColumnWidth ? "true" : "false");
		}
		protected internal virtual void WriteParagraphBreak(ParagraphProperties properties, ParagraphBreakType breakAfterType) {
			if (properties.UsePageBreakBefore)
				if(properties.PageBreakBefore)
					WriteFoStringAttribute("break-before", "page");
				else
					WriteFoStringAttribute("break-before", "auto");
			if (breakAfterType == ParagraphBreakType.Page)
				WriteFoStringAttribute("break-after", "page");
			else if (breakAfterType == ParagraphBreakType.Column)
				WriteFoStringAttribute("break-after", "column");
		}
		protected internal void WriteStyleParagraphPropertiesTabs(TabFormattingInfo tabs) {
			if (!DocumentDataRegistrator.ShouldExportTabProperties(tabs))
				return;
			WriteStyleStartElement("tab-stops");
			try {
				int count = tabs.Count;
				for (int i = 0; i < count; i++)
					if (!tabs[i].IsDefault)
						WriteParagraphTab(tabs[i]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void WriteParagraphTab(TabInfo tabInfo) {
			WriteStyleStartElement("tab-stop");
			try {
				string position = ConvertToCmString(tabInfo.Position);
				WriteStyleStringAttribute("position", position);
				WriteTabLeaderType(tabInfo);
				WriteTabAlignmentType(tabInfo.Alignment);
			}
			finally {
				WriteEndElement();
			}
		}
		private void WriteTabLeaderType(TabInfo tabInfo) {
			if (tabInfo.Leader != TabLeaderType.None) {
				string leader = OpenDocumentHelper.TabLeaderTable.GetStringValue(tabInfo.Leader, TabLeaderType.None);
				WriteStyleStringAttribute("leader-style", "solid");
				WriteStyleStringAttribute("leader-text", leader);
			}
		}
		protected internal void WriteTabAlignmentType(TabAlignmentType alignment) {
			string type = OpenDocumentHelper.TabAlignmentTypeTable.GetStringValue(alignment, TabAlignmentType.Left);
			WriteStyleStringAttribute("type", type);
			if (alignment == TabAlignmentType.Decimal)
				WriteStyleStringAttribute("char", ",");
		}
		private void WriteStyleParagraphPropertiesAlignment(ParagraphProperties properties) {
			if (properties.UseAlignment) {
				string alignment = OpenDocumentHelper.ParagraphAlignmentTable.GetStringValue((OfficeParagraphAlignment)properties.Alignment, OfficeParagraphAlignment.Left);
				WriteFoStringAttribute("text-align", alignment);
			}
		}
		private void WriteStyleParagraphPropertiesIndention(ParagraphProperties properties) {
			if (properties.UseFirstLineIndent) {
				int indent = CalculateFirstLineIndent(properties.FirstLineIndentType, properties.FirstLineIndent);
				WriteFoStringAttribute("text-indent", ConvertToInchString(indent));
			}
			if (properties.UseLeftIndent || properties.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				WriteFoStringAttribute("margin-left", ConvertToInchString(properties.LeftIndent));
			if (properties.UseRightIndent)
				WriteFoStringAttribute("margin-right", ConvertToInchString(properties.RightIndent));
			if (properties.UseSpacingBefore)
				WriteFoStringAttribute("margin-top", ConvertToInchString(properties.SpacingBefore));
			if (properties.UseSpacingAfter)
				WriteFoStringAttribute("margin-bottom", ConvertToInchString(properties.SpacingAfter));
		}
		protected internal void WriteStyleParagraphLineSpacing(ParagraphProperties properties) {
			if (!properties.UseLineSpacingType)
				return;
			WriteParagraphLineSpacingCore(properties.LineSpacingType, properties.LineSpacing);
		}
		public bool ShouldExportParagraphOutlineLevel(int outlineLevel) {
			return (outlineLevel > 0 && outlineLevel < 10);
		}
		protected internal void WriteParagraphLineSpacingCore(ParagraphLineSpacing type, float lineSpacing) {
			string value = ConvertLineSpacingToString(type, lineSpacing);
			if (type == ParagraphLineSpacing.AtLeast)
				WriteStyleStringAttribute("line-height-at-least", value);
			else
				WriteFoStringAttribute("line-height", value);
		}
		protected internal virtual int CalculateFirstLineIndent(ParagraphFirstLineIndent firstLineIndentType, int firstLineIndent) {
			switch (firstLineIndentType) {
				case ParagraphFirstLineIndent.Hanging:
					return -firstLineIndent;
				case ParagraphFirstLineIndent.Indented:
					return firstLineIndent;
				case ParagraphFirstLineIndent.None:
				default:
					return 0;
			}
		}
		public virtual void WriteTableCellBorder(BorderBase border) {
			WriteCellBorder(border, "border", "border-line-width");
		}
		void WriteBorderDoubleLineConfiguration(BorderBase border, string attributeName) {
			if (border.Style == BorderLineStyle.None || border.Style == BorderLineStyle.Nil || border.Style == BorderLineStyle.Disabled)
				return;
			TableBorderCalculator.TableBorderInfo borderInfo = TableBorderCalculator.LineStyleInfos[border.Style];
			if (borderInfo.LineCount == 2) {
				string borderLineWidths = GenerateBorderLineWidth(border.Style, border.Width);
				WriteStyleStringAttribute(attributeName, borderLineWidths);
			}
		}
		public virtual void WriteCellBordersDifferent(TableCell cell) {
			WriteCellBorder(cell.GetActualTopCellBorder(), "border-top", "border-line-width-top");
			WriteCellBorder(cell.GetActualLeftCellBorder(), "border-left", "border-line-width-left");
			WriteCellBorder(cell.GetActualRightCellBorder(), "border-right", "border-line-width-right");
			WriteCellBorder(cell.GetActualBottomCellBorder(), "border-bottom", "border-line-width-bottom");
		}
		void WriteCellBorder(BorderBase border, string borderName, string borderLineWidth) {
			string borderContentString = GenerateBorderContentPropertyString(border);
			WriteFoStringAttribute(borderName, borderContentString);
			WriteBorderDoubleLineConfiguration(border, borderLineWidth);
		}
		public void WriteTableCellBackground(Color color) {
			WriteFoStringAttribute("background-color", ConvertColorToString(color));
		}
		public void WriteTableCellVerticalAlign(VerticalAlignment alignment) {
			switch(alignment) {
				case VerticalAlignment.Top:
					WriteStyleStringAttribute("vertical-align", String.Empty);
					return;
				case VerticalAlignment.Bottom:
					WriteStyleStringAttribute("vertical-align", "bottom");
					return;
				case VerticalAlignment.Center:
					WriteStyleStringAttribute("vertical-align", "middle");
					return;
			}
		}
		public virtual void WriteTableCellPadding(WidthUnit commonMargin) {
			WidthUnitInfo defaultWidth = commonMargin.DocumentModel.Cache.UnitInfoCache.DefaultItem;
			if (defaultWidth == commonMargin.Info)
				return;
			string padding = ConvertToCmString(commonMargin.Value);
			WriteFoStringAttribute("padding", padding);
		}
		public virtual void WriteTableCellPaddingsDifferent(TableCell cell) {
			WidthUnitInfo defaultWidth = cell.DocumentModel.Cache.UnitInfoCache.DefaultItem;
			MarginUnitBase leftMargin = cell.GetActualLeftMargin();
			if (defaultWidth != leftMargin.Info)
				WriteFoStringAttribute("padding-left", ConvertToCmString(leftMargin.Value));
			MarginUnitBase rightMargin = cell.GetActualRightMargin();
			if (defaultWidth != rightMargin.Info)
				WriteFoStringAttribute("padding-right", ConvertToCmString(rightMargin.Value));
			MarginUnitBase topMargin = cell.GetActualTopMargin();
			if (defaultWidth != topMargin.Info)
				WriteFoStringAttribute("padding-top", ConvertToCmString(topMargin.Value));
			MarginUnitBase bottomMargin = cell.GetActualBottomMargin();
			if (defaultWidth != bottomMargin.Info)
				WriteFoStringAttribute("padding-bottom", ConvertToCmString(bottomMargin.Value));
		}
#region GenerateBorderPropertyString
		internal string GenerateBorderContentPropertyString(BorderBase border) {
			if (border.Style == BorderLineStyle.None)
				return "none";
			string width = ConvertToCmString(border.Width);
			string borderLineStyle = OpenDocumentHelper.LineTypeTable.GetStringValue((BorderLineStyle)border.Style, BorderLineStyle.Single);
			string color = String.Format("#{0:X2}{1:X2}{2:X2}", border.Color.R, border.Color.G, border.Color.B);
			return String.Format("{0} {1} {2}", width, borderLineStyle, color);
		}
#endregion
		protected internal virtual OpenDocumentPageUsageType CalculatePageUsageTypeCore(PieceTable oddPage, PieceTable evenPage) {
			if (oddPage == null & evenPage != null)
				return OpenDocumentPageUsageType.Left;
			if (evenPage == null & oddPage != null)
				return OpenDocumentPageUsageType.Right;
			return OpenDocumentPageUsageType.All;
		}
#region CharacterPropertiesAttributes
		public virtual void WriteStyleTextPropertiesAttributes(CharacterProperties styleProps) {
			if (styleProps.UseDoubleFontSize)
				WriteFoStringAttribute("font-size", String.Format(CultureInfo.InvariantCulture, "{0}pt", styleProps.DoubleFontSize / 2f));
			if (styleProps.UseFontItalic)
				WriteFoStringAttribute("font-style", OpenDocumentHelper.FontStyleTable.GetStringValue(styleProps.FontItalic, false));
			if (styleProps.UseFontBold) {
				WriteFoStringAttribute("font-weight", OpenDocumentHelper.FontBoldTable.GetStringValue(styleProps.FontBold, false));
			}
			if (styleProps.UseFontName)
				WriteStyleStringAttribute("font-name", styleProps.FontName);
			if (styleProps.UseAllCaps) {
				WriteFoStringAttribute("text-transform", OpenDocumentHelper.FontCaseTable.GetStringValue(styleProps.AllCaps, false));
			}
			if (styleProps.UseForeColor)
				WriteFoStringAttribute("color", ConvertColorToString(styleProps.ForeColor));
			if (styleProps.UseBackColor)
				WriteFoStringAttribute("background-color", ConvertColorToString(styleProps.BackColor));
			if (styleProps.UseHidden) {
				string value = styleProps.Hidden ? "none" : "true";
				WriteTextStringAttribute("display", value);
			}
			WriteStyleTextUnderlineProperties(styleProps);
			WriteStyleTextStrikeOutProperties(styleProps);
			WriteStyleTextScriptProperties(styleProps);
		}
		internal void WriteStyleTextUnderlineProperties(CharacterProperties styleProps) {
			if (!styleProps.UseFontUnderlineType)
				return;
			WriteStyleTextUnderlineType(styleProps.FontUnderlineType);
			if (styleProps.UseUnderlineColor)
				WriteStyleStringAttribute("text-underline-color", ConvertColorToString(styleProps.UnderlineColor));
			if (styleProps.UseUnderlineWordsOnly) {
				string value = styleProps.UnderlineWordsOnly ? "skip-white-space" : "continuous";
				WriteStyleStringAttribute("text-underline-mode", value);
			}
		}
		internal void WriteStyleTextUnderlineType(UnderlineType type) {
			WriteTextUnderlineStyleAttribute(type);
			WriteTextUnderlineTypeAttribute(type);
			WriteTextUnderlineWidthAttribute(type);
		}
		internal void WriteTextUnderlineStyleAttribute(UnderlineType type) {
			string underlineType = UnderlineTypeTable.GetStringValue(type, UnderlineType.Single);
			WriteStyleStringAttribute("text-underline-style", underlineType);
		}
		internal void WriteTextUnderlineTypeAttribute(UnderlineType type) {
			if (type == UnderlineType.Double || type == UnderlineType.DoubleWave)
				WriteStyleStringAttribute("text-underline-type", "double");
		}
		internal void WriteTextUnderlineWidthAttribute(UnderlineType type) {
			if (type == UnderlineType.None)
				return;
			bool isBold = type == UnderlineType.HeavyWave || type == UnderlineType.ThickDashDotted ||
			type == UnderlineType.ThickDashDotDotted || type == UnderlineType.ThickDashed ||
			type == UnderlineType.ThickDotted || type == UnderlineType.ThickLongDashed || type == UnderlineType.ThickSingle;
			string width = OpenDocumentHelper.FontUnderLineWidthTable.GetStringValue(isBold, false);
			WriteStyleStringAttribute("text-underline-width", width);
		}
		protected internal void WriteStyleTextStrikeOutProperties(CharacterProperties styleProps) {
			if (!styleProps.UseFontStrikeoutType)
				return;
			WriteStyleTextStrikeOutType(styleProps.FontStrikeoutType);
			if (styleProps.UseStrikeoutColor)
				WriteStyleStringAttribute("text-line-through-color", ConvertColorToString(styleProps.StrikeoutColor));
			if (styleProps.UseStrikeoutWordsOnly) {
				string value = styleProps.StrikeoutWordsOnly ? "skip-white-space" : "continuous";
				WriteStyleStringAttribute("text-line-through-mode", value);
			}
		}
		protected internal void WriteStyleTextStrikeOutType(StrikeoutType type) {
			if (type == StrikeoutType.Double)
				WriteStyleStringAttribute("text-line-through-type", "double");
			if (type == StrikeoutType.Single)
				WriteStyleStringAttribute("text-line-through-style", "solid");
			if (type == StrikeoutType.None)
				WriteStyleStringAttribute("text-line-through-style", "none");
		}
		protected internal void WriteStyleTextScriptProperties(CharacterProperties styleProps) {
			if (!styleProps.UseScript)
				return;
			string textPosition = OpenDocumentHelper.CharacterScriptTable.GetStringValue(styleProps.Script, CharacterFormattingScript.Normal);
			WriteStyleStringAttribute("text-position", textPosition);
		}
#endregion
		public virtual void WriteGraphicStylePropertiesStart() {
			WriteStyleStartElement("graphic-properties");
		}
		public virtual void WriteInlinePictureFrameStyleContent(InlinePictureProperties properties) {
			WriteStyleStringAttribute("horizontal-pos", "center");
			WriteStyleStringAttribute("horizontal-rel", "paragraph");
			WriteStyleStringAttribute("vertical-pos", "top");
		}
		public virtual void WriteFloatingObjectFrameStyleContent(FloatingObjectProperties floatingObjectProperties) {
			string hPositionAlignment;
			if(ImportHelper.InverseHorizontalPositionAlignmentTable.TryGetValue(floatingObjectProperties.HorizontalPositionAlignment, out hPositionAlignment))
				WriteStyleStringAttribute("horizontal-pos", hPositionAlignment);
			string vPositionAlignment;
			if(ImportHelper.InverseVerticalPositionAlignmentTable.TryGetValue(floatingObjectProperties.VerticalPositionAlignment, out vPositionAlignment))
				WriteStyleStringAttribute("vertical-pos", vPositionAlignment);
			string hPositionType;
			if(ImportHelper.InverseHorizontalPositionTypeTable.TryGetValue(floatingObjectProperties.HorizontalPositionType, out hPositionType))
				WriteStyleStringAttribute("horizontal-rel", hPositionType);
			string vPositionType;
			if(ImportHelper.InverseVerticalPositionTypeTable.TryGetValue(floatingObjectProperties.VerticalPositionType, out vPositionType))
				WriteStyleStringAttribute("vertical-rel", vPositionType);
			ExportWrap(floatingObjectProperties);
			if (floatingObjectProperties.UseTopDistance)
				WriteFoStringAttribute("margin-top", ConvertToCmString(floatingObjectProperties.TopDistance));
			if (floatingObjectProperties.UseLeftDistance)
				WriteFoStringAttribute("margin-left", ConvertToCmString(floatingObjectProperties.LeftDistance));
			if (floatingObjectProperties.UseBottomDistance)
				WriteFoStringAttribute("margin-bottom", ConvertToCmString(floatingObjectProperties.BottomDistance));
			if (floatingObjectProperties.UseRightDistance)
				WriteFoStringAttribute("margin-right", ConvertToCmString(floatingObjectProperties.RightDistance));
		}
		string IsBehindDocConvertToString(bool isBehindDoc) {
			return (isBehindDoc ? "background" : "foreground");
		}
		public virtual void WriteShapeFrameStyleContent(Shape shape, bool isTextBox) {
			if (shape.UseFillColor && !DXColor.IsTransparentOrEmpty(shape.FillColor))
				WriteDrawStringAttribute("fill-color", ConvertColorToString(shape.FillColor));
			if (isTextBox) {
				if (shape.UseOutlineWidth && shape.OutlineWidth > 0)
					WriteSvgStringAttribute("stroke-width", ConvertToCmString(shape.OutlineWidth));
				if (shape.UseOutlineColor && !DXColor.IsTransparentOrEmpty(shape.OutlineColor))
					WriteSvgStringAttribute("stroke-color", ConvertColorToString(shape.OutlineColor));
			}
			else {
				if (shape.UseOutlineWidth && shape.OutlineWidth > 0) {
					string outlineWidth = ConvertToCmString(shape.OutlineWidth);
					bool isColorDefined = shape.UseOutlineColor && shape.OutlineColor != DXColor.Empty;
					string value = isColorDefined ? String.Format("{0} solid {1}", outlineWidth, ConvertColorToString(shape.OutlineColor)) : String.Format("{0} none", outlineWidth);
					WriteFoStringAttribute("border", value);
				}
			}
		}
		public virtual void WriteTextBoxPropertiesFrameStyleContent(TextBoxProperties textBoxProperties) {
			if (textBoxProperties.UseTopMargin)
				WriteFoStringAttribute("padding-top", ConvertToCmString(textBoxProperties.TopMargin));
			if (textBoxProperties.UseLeftMargin)
				WriteFoStringAttribute("padding-left", ConvertToCmString(textBoxProperties.LeftMargin));
			if (textBoxProperties.UseBottomMargin)
				WriteFoStringAttribute("padding-bottom", ConvertToCmString(textBoxProperties.BottomMargin));
			if (textBoxProperties.UseRightMargin)
				WriteFoStringAttribute("padding-right", ConvertToCmString(textBoxProperties.RightMargin));
			if (textBoxProperties.UseResizeShapeToFitText)
				WriteDrawStringAttribute("auto-grow-height", textBoxProperties.ResizeShapeToFitText ? "true" : "false");
		}
		private void ExportWrap(FloatingObjectProperties floatingObjectProperties) {
			string wrap = "";
			string runTrough = null;
			FloatingObjectTextWrapSide TextWrapSide = floatingObjectProperties.TextWrapSide;
			FloatingObjectTextWrapType TextWrapType = floatingObjectProperties.TextWrapType;
			if (TextWrapSide == FloatingObjectTextWrapSide.Left && TextWrapType == FloatingObjectTextWrapType.Square)
				wrap = "left";
			if (TextWrapSide == FloatingObjectTextWrapSide.Right && TextWrapType == FloatingObjectTextWrapType.Square)
				wrap = "right";
			if (TextWrapSide == FloatingObjectTextWrapSide.Both && TextWrapType == FloatingObjectTextWrapType.Square)
				wrap = "parallel";
			if (TextWrapSide == FloatingObjectTextWrapSide.Both && TextWrapType == FloatingObjectTextWrapType.None)
				wrap = "none";
			if (TextWrapSide == FloatingObjectTextWrapSide.Largest && TextWrapType == FloatingObjectTextWrapType.Square)
				wrap = "dynamic";
			if((TextWrapSide == FloatingObjectTextWrapSide.Largest && TextWrapType == FloatingObjectTextWrapType.Through) ||
				(TextWrapType == FloatingObjectTextWrapType.None && (floatingObjectProperties.ZOrder != 0 || floatingObjectProperties.IsBehindDoc))) {
				wrap = "run-through";
				runTrough = IsBehindDocConvertToString(floatingObjectProperties.IsBehindDoc);
			}
			if(!String.IsNullOrEmpty(wrap))
				WriteStyleStringAttribute("wrap", wrap);
			if(!String.IsNullOrEmpty(runTrough))
				WriteStyleStringAttribute("run-through", runTrough);
		}
		protected internal virtual void WriteInlineImageAttributes(OfficeImage image, string imageId) {
			OfficeImage rootImage = image.RootImage;
			string fullName;
			if (Exporter.ExportedImageTable.TryGetValue(rootImage, out fullName)) {
				WriteImageAttributesCore(fullName);
				return;
			}
			fullName = GenerateImageFullName(image, imageId);
			WriteImageAttributesCore(fullName);
			Exporter.AddPackageImage(fullName, rootImage);
			Exporter.ExportedImageTable.Add(rootImage, fullName);
		}
		protected internal virtual void WriteImageAttributesCore(string fullName) {
			WriteXlinkStringAttribute("href", fullName);
			WriteXlinkStringAttribute("type", "simple");
			WriteXlinkStringAttribute("show", "embed");
			WriteXlinkStringAttribute("actuate", "onLoad");
		}
		protected internal virtual string GenerateImageFullName(OfficeImage image, string imageId) {
			string extenstion = Exporter.GetImageExtension(image);
			return String.Format("Pictures/{0}.{1}", imageId, extenstion);
		}
		public virtual void WriteListStyleStart() {
			WriteTextStartElement("list-style");
		}
		public virtual void WriteListStyleAttributes(string styleName) {
			WriteStyleStringAttribute("name", styleName);
		}
		public virtual void WriteListLevelStyleNumberStart() {
			WriteTextStartElement("list-level-style-number");
		}
		internal void WriteListLevelStyleBulletStart() {
			WriteTextStartElement("list-level-style-bullet");
		}
		public virtual void WriteListLevelStyleCommonAttributes(int index, string styleName) {
			int num = index + 1;
			WriteTextStringAttribute("level", num.ToString());
			WriteStyleStringAttribute("style-name", styleName, true); 
		}
		public virtual void WriteListLevelStyleNumberAttributes(IListLevel level, int index, string styleName) {
			string displayFormat = level.ListLevelProperties.DisplayFormatString;
			WriteStyleStringAttribute("num-prefix", ConvertToNumberingFormatPrefix(displayFormat), true);
			WriteStyleStringAttribute("num-suffix", ConvertToNumberingFormatSuffix(displayFormat), true);
			int levelCount = CalculateDisplayLevelsCount(displayFormat);
			if (levelCount > 1)
				WriteTextStringAttribute("display-levels", levelCount.ToString());
			string numberingFormat = OpenDocumentHelper.NumberingFormatTable.GetStringValue(level.ListLevelProperties.Format, NumberingFormat.Decimal);
			WriteStyleStringAttribute("num-format", numberingFormat);
			if (level.ListLevelProperties.Start > 1)
				WriteTextIntAttribute("start-value", level.ListLevelProperties.Start);
		}
		public virtual void WriteListLevelStyleBulletAttributes(IListLevel level, int index, string styleName) {
			WriteTextStringAttribute("bullet-char", Convert.ToString(Characters.Bullet));
		}
		public virtual void WriteListLevelStylePropertiesStart() {
			WriteStyleStartElement("list-level-properties");
		}
		internal void WriteListLevelTextPropertiesAttributes(string fontName) {
		}
		public virtual void WriteListLevelStylePropertiesAttributes(IListLevel level, int index) {
			WriteTextStringAttribute("list-level-position-and-space-mode", "label-alignment");
		}
		protected internal void WriteStyleListLevelPropertiesAlignment(ListLevelProperties properties) {
			string alignment = OpenDocumentHelper.ListNumberAlignmentTable.GetStringValue(properties.Alignment, ListNumberAlignment.Left);
			WriteFoStringAttribute("text-align", alignment);
		}
		protected internal void WriteStyleListLevelPropertiesIndention(IListLevel level) {
			int indent = CalculateFirstLineIndent(level.FirstLineIndentType, level.FirstLineIndent);
			WriteFoStringAttribute("text-indent", ConvertToInchString(indent));
			if (level.LeftIndent != 0)
				WriteFoStringAttribute("margin-left", ConvertToInchString(level.LeftIndent));
			if (level.RightIndent != 0)
				WriteFoStringAttribute("margin-right", ConvertToInchString(level.RightIndent));
			if (level.SpacingBefore != 0)
				WriteFoStringAttribute("margin-top", ConvertToInchString(level.SpacingBefore));
			if (level.SpacingAfter != 0)
				WriteFoStringAttribute("margin-bottom", ConvertToInchString(level.SpacingAfter));
		}
		protected internal void WriteStyleListLevelPropertiesLineSpacing(IListLevel level) {
			WriteParagraphLineSpacingCore(level.LineSpacingType, level.LineSpacing);
		}
		protected internal void WriteListLevelLabelAlignmentProperties(IListLevel level) {
			WriteStyleStartElement("list-level-label-alignment");
			try {
				WriteStyleListLevelPropertiesAlignment(level.ListLevelProperties);
				WriteStyleListLevelPropertiesIndention(level);
				WriteStyleListLevelPropertiesLineSpacing(level);
				WriteTextStringAttribute("label-followed-by", OpenDocumentHelper.ListNumberSeparatorTable.GetStringValue(level.ListLevelProperties.Separator, Characters.TabMark));
			}
			finally {
				WriteEndElement();
			}
		}
#endregion
#region paragraphs
		public virtual void WriteParagraphStart(bool isHeading) {
			if (isHeading)
				WriteTextStartElement("h");
			else
				WriteTextStartElement("p");
		}
		public virtual void WriteParagraphAttributes(string styleName, int outlineLevel) {
			if (!String.IsNullOrEmpty(styleName))
				WriteTextStringAttribute("style-name", styleName);
			if (ShouldExportParagraphOutlineLevel(outlineLevel))
				WriteTextIntAttribute("outline-level", outlineLevel);
		}
		public virtual void WriteTextRunStart() {
			WriteTextStartElement("span");
		}
		public virtual void WriteInlinePictureFrameStart() {
			WriteDrawStartElement("frame");
		}
		public virtual void WriteFloatingObjectAnchorFrameStart() {
			WriteDrawStartElement("frame");
		}
		public virtual void WriteFloatingObjectAnchorTextBoxStart() {
			WriteDrawStartElement("text-box");
		}
		public virtual void WriteInlineImageStart() {
			WriteDrawStartElement("image");
		}
		public virtual void WriteTextRunAttributes(string styleName) {
			Debug.Assert(!String.IsNullOrEmpty(styleName));
			WriteTextStringAttribute("style-name", styleName);
		}
		public virtual void WriteTableAttributes(string styleName) {
			if (!String.IsNullOrEmpty(styleName))
				WriteTableStringAttribute("name", styleName);
			WriteTableStringAttribute("style-name", styleName);
		}
		public virtual void WriteStyleTableAttributes(TableStyleInfo info) {
			string tableStyleName = info.Name;
			WriteStyleStringAttribute("name", tableStyleName);
			WriteStyleStringAttribute("family", "table");
			if (!String.IsNullOrEmpty(info.MasterPageName))
				WriteStyleStringAttribute("master-page-name", info.MasterPageName);
		}
		public virtual void WriteStyleTableRowAttributes(string styleName){
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "table-row");
		}
		public virtual void WriteStyleTableColumnAttributes(string styleName) {
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "table-column");
		}
		public virtual void WriteStyleTableCellAttributes(string styleName) {
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "table-cell");
		}
		public virtual void WriteTableRowAttributes(string rowStyleName) {
			WriteTableStringAttribute("style-name", rowStyleName);
		}
		public virtual void WriteTableCellAttributes(TableCell cell, string cellStyleName, int rowsSpanned) {
			if (!String.IsNullOrEmpty(cellStyleName))
				WriteTableStringAttribute("style-name", cellStyleName);
			if (cell.ColumnSpan > 1)
				WriteTableStringAttribute("number-columns-spanned", cell.ColumnSpan.ToString());
			if (cell.VerticalMerging == MergingState.Restart && rowsSpanned > 0) {
				WriteTableStringAttribute("number-rows-spanned", rowsSpanned.ToString());
			}
			WriteOfficeStringAttribute("value-type", "string");
		}
		public virtual void WriteTableColumns(OpenDocumentTableColumnsInfo columns) {
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				WriteTableColumnStart();
				try {
					WriteTableStringAttribute("style-name", columns[i].Name);
				}
				finally {
					WriteEndElement();
				}
			}
		}
		protected internal virtual void WriteFieldStyle(FieldCodeStartRun run) {
			string styleName = Exporter.GetFieldStyleName(run);
			if (!string.IsNullOrEmpty(styleName))
				WriteTextStringAttribute("style-name", styleName);
		}
		protected internal virtual void WriteFieldExpressionStart() {
			WriteTextStartElement("expression");
		}
		protected internal virtual void WriteFieldAttributes() {
			WriteOfficeStringAttribute("value-type", "string");
		}
		protected internal virtual void WriteFieldFormula(string formula) {
			WriteTextStringAttribute("formula", OpenDocumentHelper.OOWNsPrefix + ":" + formula);
		}
		public void WritePageFieldAttributes() {
			WriteTextStringAttribute("fixed", "false");
		}
		public void WritePageCountFieldAttributes() {
		}
		public void WritePageRefFieldAttributes(string bookmarkName) {
			if (!String.IsNullOrEmpty(bookmarkName)) {
				WriteTextStringAttribute("ref-name", bookmarkName);
				WriteTextStringAttribute("reference-format", "page");
			}
		}
		internal virtual void WritePictureFrameAttributes(InlinePictureRun run, string imageID) {
			string styleName = Exporter.GetPictureRunStyleName(run);
			if (!string.IsNullOrEmpty(styleName))
				WriteDrawStringAttribute("style-name", styleName);
			string frameName = NameResolver.CalculatePictureFrameName(imageID);
			WriteDrawStringAttribute("name", frameName);
			WriteTextStringAttribute("anchor-type", "as-char");
			WriteSvgStringAttribute("width", GetPictureWidth(run));
			WriteSvgStringAttribute("height", GetPictureHeight(run));
			WriteDrawStringAttribute("z-index", "0");
		}
		internal virtual void WriteFloatingObjectFrameAttributes(FloatingObjectProperties floatingObjectProperties, Shape shape, TextBoxProperties textBoxProperties, bool isTextBox, string frameName) {
			string styleName = Exporter.GetFloatingObjectAnchorRunStyleName(floatingObjectProperties);
			if (!String.IsNullOrEmpty(styleName))
				WriteDrawStringAttribute("style-name", styleName);
			WriteDrawStringAttribute("name", frameName);
			WriteTextStringAttribute("anchor-type", OpenDocumentTextExporter.WriteAnchorType(floatingObjectProperties));
			OpenDocumentExportBoundsAdjuster adjuster = new OpenDocumentExportBoundsAdjuster(floatingObjectProperties, shape, isTextBox);
			Size actualSize = adjuster.AdjustActualSize();
			WriteDrawStringAttribute("z-index", floatingObjectProperties.ZOrder.ToString());
			if (floatingObjectProperties.UseOffset) {
				Point offset = adjuster.AdjustOffset();
				WriteSvgStringAttribute("x", ConvertToCmString(offset.X));
				WriteSvgStringAttribute("y", ConvertToCmString(offset.Y));
			}
			if (!Object.ReferenceEquals(textBoxProperties, null) && textBoxProperties.UseResizeShapeToFitText)
				WriteDrawStringAttribute("auto-grow-height", textBoxProperties.ResizeShapeToFitText ? "true" : "false");
			else
				WriteSvgStringAttribute("height", ConvertToCmString(actualSize.Height));
			WriteSvgStringAttribute("width", ConvertToCmString(actualSize.Width));
		}
#endregion
#region Section
		public void WriteSectionStart() {
			WriteTextStartElement("section");
		}
		public virtual void WriteSectionAttributes(string styleName, string sectionName, bool isProtected) {
			WriteTextStringAttribute("style-name", styleName);
			WriteTextStringAttribute("name", sectionName);
			if (isProtected) {
				WriteTextStringAttribute("protected", "true");
				byte[] passwordHash = Exporter.DocumentModel.ProtectionProperties.OpenOfficePasswordHash;
				if (passwordHash != null && passwordHash.Length > 0) {
					WriteTextStringAttribute("protection-key", Convert.ToBase64String(passwordHash));
				}
			}
		}
#endregion
#region Bookmark
		protected internal virtual void WriteBookmarkName(Bookmark bookmark) {
			WriteTextStringAttribute("name", bookmark.Name);
		}
		protected internal virtual void WriteBookmarkStart() {
			WriteTextStartElement("bookmark-start");
		}
		protected internal virtual void WriteBookmarkEnd() {
			WriteTextStartElement("bookmark-end");
		}
#endregion
#region Section Style
		public virtual void WriteSectionAutoStyleAttributes(string styleName) {
			WriteStyleStringAttribute("name", styleName);
			WriteStyleStringAttribute("family", "section");
		}
		public virtual void WriteSectionPropertiesStart() {
			WriteStyleStartElement("section-properties");
		}
		public virtual void WriteColumnsStyleStart() {
			WriteStyleStartElement("columns");
		}
		public virtual void WriteColumnStyleStart() {
			WriteStyleStartElement("column");
		}
		public virtual void WriteColumnSeparatorStyleStart() {
			WriteStyleStartElement("column-sep");
		}
		public virtual void WriteColumnSeparatorAttributes() {
			string width = ConvertToStringValue(0.05, "pt");
			WriteStyleStringAttribute("style", "solid");
			WriteStyleStringAttribute("width", width);
			WriteStyleStringAttribute("color", "#000000");
			WriteStyleStringAttribute("height", "100%");
		}
		public virtual void WriteColumnsStyleAttributes(SectionColumns columns) {
			WriteFoStringAttribute("column-count", columns.ColumnCount.ToString());
			if (columns.EqualWidthColumns)
				WriteFoStringAttribute("column-gap", ConvertToInchString(columns.Space));
		}
		public virtual void WriteColumnStyleAttributes(float relColumnWidth, float spaceBefore, float spaceAfter) {
			WriteStyleStringAttribute("rel-width", ConvertToStringValue(relColumnWidth, "*"));
			WriteFoStringAttribute("start-indent", ConvertToInchString(spaceBefore));
			WriteFoStringAttribute("end-indent", ConvertToInchString(spaceAfter));
		}
#endregion
#region Page Layout Style
		public virtual void WritePageLayoutStart() {
			WriteStyleStartElement("page-layout");
		}
		public virtual void WritePageLayoutAttributes(string styleName) {
			WriteStyleStringAttribute("name", styleName);
		}
		public virtual void WritePageLayoutPropertiesStart() {
			WriteStyleStartElement("page-layout-properties");
		}
		public virtual void WritePageLayoutPropertiesAttributes(Section section) {
			SectionPage page = section.Page;
			WriteFoStringAttribute("page-width", ConvertToInchString(page.Width));
			WriteFoStringAttribute("page-height", ConvertToInchString(page.Height));
			WriteStyleStringAttribute("print-orientation", page.Landscape ? "landscape" : "portrait");
			WritePageLayoutMargins(section.Margins);
			DocumentProperties documentProperties = page.DocumentModel.DocumentProperties;
			Color backColor = documentProperties.PageBackColor;
			if (documentProperties.DisplayBackgroundShape && !DXColor.IsTransparentOrEmpty(backColor))
				WriteFoStringAttribute("background-color", ConvertColorToString(backColor));
		}
		internal virtual void WritePageLayoutMargins(SectionMargins margins) {
			WriteFoStringAttribute("margin-top", ConvertToInchString(margins.Top));
			WriteFoStringAttribute("margin-bottom", ConvertToInchString(margins.Bottom));
			WriteFoStringAttribute("margin-left", ConvertToInchString(margins.Left));
			WriteFoStringAttribute("margin-right", ConvertToInchString(margins.Right));
		}
		public virtual void WriteHeaderStyleStart() {
			WriteStyleStartElement("header-style");
		}
		public virtual void WriteFooterStyleStart() {
			WriteStyleStartElement("footer-style");
		}
		public virtual void WriteHeaderFooterPropertiesStart() {
			WriteStyleStartElement("header-footer-properties");
		}
		public virtual void WriteHeaderStart() {
			WriteStyleStartElement("header");
		}
		public virtual void WriteFooterStart() {
			WriteStyleStartElement("footer");
		}
		public virtual void WriteHeaderPropertiesAttributes(Section section) {
			int minHeight = Math.Max(0, section.Margins.Top - section.Margins.HeaderOffset);
			WriteFoStringAttribute("min-height", ConvertToInchString(minHeight));
			WriteFoStringAttribute("margin-left", ConvertToInchString(0));
			WriteFoStringAttribute("margin-right", ConvertToInchString(0));
			WriteFoStringAttribute("margin-bottom", ConvertToInchString(section.Margins.HeaderOffset));
			WriteStyleStringAttribute("dynamic-spacing", "false");
		}
		public virtual void WriteFooterPropertiesAttributes(Section section) {
			WriteFoStringAttribute("margin-left", ConvertToInchString(0));
			WriteFoStringAttribute("margin-right", ConvertToInchString(0));
			WriteFoStringAttribute("margin-top", ConvertToInchString(section.Margins.FooterOffset));
			WriteStyleStringAttribute("dynamic-spacing", "false");
		}
#endregion
#region master-styles
		public virtual void WriteMasterStylesStart() {
			WriteOfficeStartElement("master-styles");
		}
		public virtual void WriteMasterPageStart() {
			WriteStyleStartElement("master-page");
		}
		public virtual void WriteMasterPageAttributes(string masterPageName, string pageLayoutName) {
			WriteMasterPageAttributes(masterPageName, pageLayoutName, String.Empty);
		}
		public virtual void WriteMasterPageAttributes(string masterPageName, string pageLayoutName, string nextMasterPageName) {
			WriteStyleStringAttribute("name", masterPageName);
			WriteStyleStringAttribute("page-layout-name", pageLayoutName);
			if (!String.IsNullOrEmpty(nextMasterPageName))
				WriteStyleStringAttribute("next-style-name", nextMasterPageName);
		}
#endregion
#region Lists
		public virtual void WriteListStart() {
			WriteTextStartElement("list");
		}
		public virtual void WriteListItemStart() {
			WriteTextStartElement("list-item");
		}
		public virtual void WriteListAttributes(string styleName, string id, string parentId) {
			if (!String.IsNullOrEmpty(id))
				WriteStringAttributeCore("xml", "id", null, id);
			if (!String.IsNullOrEmpty(parentId))
				WriteTextStringAttribute("continue-list", parentId);
			WriteTextStringAttribute("style-name", styleName);
		}
#endregion
#region Hyperlink
		public virtual void WriteHyperlinkStart() {
			WriteTextStartElement("a");
		}
		public void WriteHyperlinkAttributes(HyperlinkInfo hyperlinkInfo) {
			WriteXlinkStringAttribute("type", "simple");
			string link = string.Empty;
			if (!string.IsNullOrEmpty(hyperlinkInfo.NavigateUri)) {
				string url = HyperlinkUriHelper.EnsureUriIsValid(hyperlinkInfo.NavigateUri);
				link = ReplaceLineBreaks(url);
			}
			if (!string.IsNullOrEmpty(hyperlinkInfo.Anchor))
				link = string.Format("{0}#{1}", link, hyperlinkInfo.Anchor);
			if (link.Length != 0)
				WriteXlinkStringAttribute("href", link);
			if (!String.IsNullOrEmpty(hyperlinkInfo.ToolTip))
				WriteOfficeStringAttribute("name", ReplaceLineBreaks(hyperlinkInfo.ToolTip));
			WriteXlinkStringAttribute("show", "replace");
		}	   
		protected internal string ReplaceLineBreaks(string url) {
			string result = String.Empty;
			int count = url.Length;
			int from = 0;
			for (int i = 0; i < count; i++) {
				char currentChar = url[i];
				string value;
				if (breaksTable.TryGetValue(currentChar, out value)) {
					result += url.Substring(from, i);
					result += value;
					from = i + 1;
				}
			}
			result += url.Substring(from);
			return result;
		}
#endregion
#region Export Text Runs
		public virtual void WriteTextRunContent(TextRun run) {
			string runText = run.GetPlainText(Exporter.PieceTable.TextBuffer);
			StringBuilder sb = new StringBuilder();
			int count = runText.Length;
			bool firstSpace = true;
			int spaces = 0;
			for (int i = 0; i < count; i++) {
				char character = runText[i];
				if (character == ' ') {
					if (firstSpace) {
						firstSpace = false;
						spaces = 0;
					}
					spaces++;
					continue;
				}
				if (!firstSpace) {
					if ((sb.Length == 0) && (spaces > 0))
						WriteTextAndAppendSpaces(sb, spaces);
					else if (spaces > 0 && i > 1 && i <= count - 1) {
						sb.Append(' ');
						WriteTextAndAppendSpaces(sb, spaces - 1);
					}
					firstSpace = true; spaces = 0;
				}
				ProcessNonSpaceSymbol(sb, character);
			}
			if (spaces != 0)
				WriteTextAndAppendSpaces(sb, spaces);
			else				  
				WriteValue(sb.ToString());
		}
		void ProcessNonSpaceSymbol(StringBuilder sb, char character) {
			if (IsLineBreak(character)) {
				WriteTextAndBreakingSymbol(sb, "line-break");
			}
			else if (IsTabBreak(character)) {
				WriteTextAndBreakingSymbol(sb, "tab");
			}
			else if (!ShouldSkipCharacter(character))
				sb.Append(character);
		}
		void WriteTextAndBreakingSymbol(StringBuilder sb, string element) {
			WriteValue(sb.ToString());
			WriteStartElementCore(OpenDocumentHelper.TextNsPrefix, element, OpenDocumentHelper.TextNamespace);
			WriteEndElement();
			sb.Length = 0;
		}
		void WriteTextAndAppendSpaces(StringBuilder sb, int count) {
			if (count == 0)
				return;
			WriteValue(sb.ToString());
			WriteStartElementCore(OpenDocumentHelper.TextNsPrefix, "s", OpenDocumentHelper.TextNamespace);
			if (count > 1)
				WriteTextStringAttribute("c", count.ToString());
			WriteEndElement();
			sb.Length = 0;
		}
		static bool IsLineBreak(char character) {
			return character == Characters.LineBreak;
		}
		static bool IsTabBreak(char character) {
			return character == Characters.TabMark;
		}
		internal virtual bool ShouldSkipCharacter(char ch) {
			return ch == Characters.PageBreak || ch == Characters.ColumnBreak || ch == Characters.TabMark;
		}
#endregion
#region Line Numbering
		public virtual void WriteLineNumberingStart() {
			WriteTextStartElement("linenumbering-configuration");
		}
		public void WriteLineNumberingAttributes() {
			if (Exporter.DataRegistrator.LineNumbersIncrement == 0)
				WriteTextStringAttribute("number-lines", "false");
			WriteTextStringAttribute("offset", "0.5cm");
			WriteTextStringAttribute("num-format", "1");
			WriteTextStringAttribute("number-position", "left");
			WriteTextStringAttribute("increment", Exporter.DataRegistrator.LineNumbersIncrement.ToString());
		}
#endregion
#region Document variables
		public void WriteUserFieldDecls() {
			WriteTextStartElement("user-field-decls");
		}
		public void WriteUserFieldDecl() {
			WriteTextStartElement("user-field-decl");
		}
#endregion
		public void WriteStyleParagraphPropertiesLineNumber(ParagraphProperties properties, ParagraphIndex paragraphIndex, bool useParagraphIndex) {
			if (useParagraphIndex && Exporter.DataRegistrator.ParagraphsLineNumber.ContainsKey(paragraphIndex)) {
				OpenDocumentLineNumberingInfo info = Exporter.DataRegistrator.ParagraphsLineNumber[paragraphIndex];
				if (info.NumberingRestartType == LineNumberingRestart.NewSection) 
					WriteTextStringAttribute("line-number", info.StartingLineNumber.ToString());
				if (info.SectionExclude) 
					WriteTextStringAttribute("number-lines", "false");
			}
			else {
				if (properties.UseSuppressLineNumbers)
					WriteTextStringAttribute("number-lines", "false");
			}
		}
		public string GenerateBorderLineWidth(BorderLineStyle style, int width) {
			TableBorderCalculator.TableBorderInfo borderInfo = TableBorderCalculator.LineStyleInfos[style];
			float inner = borderInfo.DrawingCompoundArray[1] * width;
			float gap = (borderInfo.DrawingCompoundArray[2] - borderInfo.DrawingCompoundArray[1])*width;
			float outer = (borderInfo.DrawingCompoundArray[3] - borderInfo.DrawingCompoundArray[2])*width;
			return String.Format("{0} {1} {2}", ConvertToCmString(inner), ConvertToCmString(gap), ConvertToCmString(outer));
		}
	}
#endregion
#region UnderlineTypeTable
	public class UnderlineTypeTable : Dictionary<string, UnderlineType> {
		public UnderlineType GetEnumValue(string key) {
			UnderlineType result;
			if (!TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out result))
				return UnderlineType.None;
			return result;
		}
	}
#endregion
#region TextWrapTypeTable
	public class TextWrapTypeTable : Dictionary<string, FloatingObjectTextWrapType> { }
#endregion
#region FloatingObjectTextWrapSideTable
	public class TextWrapSideTable : Dictionary<string, FloatingObjectTextWrapSide> { }
#endregion
#region InverseTextWrapTypeTable
	public class InverseTextWrapTypeTable : Dictionary<FloatingObjectTextWrapType, string> { }
#endregion
#region InverseFloatingObjectTextWrapSideTable
	public class InverseTextWrapSideTable : Dictionary<FloatingObjectTextWrapSide, string> { }
#endregion
#region HorizontalPositionAlignmentTable
	public class HorizontalPositionAlignmentTable : Dictionary<string, FloatingObjectHorizontalPositionAlignment> {
	}
#endregion
#region InverseHorizontalPositionAlignmentTable
	public class InverseHorizontalPositionAlignmentTable : Dictionary<FloatingObjectHorizontalPositionAlignment, string> {
	}
#endregion
#region VerticalPositionAlignmentTable
	public class VerticalPositionAlignmentTable : Dictionary<string, FloatingObjectVerticalPositionAlignment> {
	}
#endregion
#region InverseVerticalPositionAlignmentTable
	public class InverseVerticalPositionAlignmentTable : Dictionary<FloatingObjectVerticalPositionAlignment, string> {
	}
#endregion
#region InverseHorizontalPositionTypeTable
	public class InverseHorizontalPositionTypeTable : Dictionary<FloatingObjectHorizontalPositionType, string> {
	}
#endregion
#region InverseVerticalPositionTypeTable
	public class InverseVerticalPositionTypeTable : Dictionary<FloatingObjectVerticalPositionType, string> {
	}
#endregion
#region HorizontalPositionTypeTable
	public class HorizontalPositionTypeTable : Dictionary<string, FloatingObjectHorizontalPositionType> {
	}
#endregion
#region VerticalPositionTypeTable
	public class VerticalPositionTypeTable : Dictionary<string, FloatingObjectVerticalPositionType> {
	}
#endregion
#region StrikeoutTypeTable
	public class StrikeoutTypeTable : Dictionary<string, StrikeoutType> {
		public StrikeoutType GetEnumValue(string key) {
			StrikeoutType result;
			if (!TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out result))
				return StrikeoutType.None;
			return result;
		}
	}
#endregion
#region OpenDocumentPageUsageTypeTable
	public class OpenDocumentPageUsageTypeTable : Dictionary<string, OpenDocumentPageUsageType> {
		public OpenDocumentPageUsageType GetEnumValue(string key) {
			OpenDocumentPageUsageType result;
			if (!TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out result))
				return OpenDocumentPageUsageType.All;
			return result;
		}
	}
#endregion
	public class OpenDocumentLineType : Dictionary<string, BorderLineStyle> {
		public BorderLineStyle GetEnumValue(string key) {
			BorderLineStyle result;
			if (!TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out result))
				return BorderLineStyle.None;
			return result;
		}
	}
#region OpenDocumentTableColumnInfo
	public class OpenDocumentTableColumnInfo {
		int width;
		string name;
		bool useOtimalColumnWidth;
		public OpenDocumentTableColumnInfo(int width, string name, bool useOtimalColumnWidth) {
			this.width = width;
			this.name = name;
			this.useOtimalColumnWidth = useOtimalColumnWidth;
		}
		public int Width { get { return width; } set { width = value; } }
		public string Name { get { return name; } set { name = value; } }
		public bool UseOtimalColumnWidth { get { return useOtimalColumnWidth; } set { useOtimalColumnWidth = value; } }
	}
#endregion
#region OpenDocumentTableColumnsInfo
	public class OpenDocumentTableColumnsInfo : List<OpenDocumentTableColumnInfo> {
		readonly TableGrid grid;
		readonly bool useOptimalColumnWidth;
		public OpenDocumentTableColumnsInfo(TableGrid grid, bool useOptimalColumnWidth) {
			Guard.ArgumentNotNull(grid, "grid");
			this.grid = grid;
			this.useOptimalColumnWidth = useOptimalColumnWidth;
		}
		public OpenDocumentTableColumnsInfo(int capacity, TableGrid grid)
			: base(capacity) {
			Guard.ArgumentNotNull(grid, "grid");
			this.grid = grid;
		}
		public void GenerateColumnsInfo(DocumentModel documentModel, string tableStyleName) {
			for (int i = 0; i < grid.Columns.Count; i++) {
				string name = NameResolver.CalculateTableColumnsStyleName(tableStyleName, i + 1);
				OpenDocumentTableColumnInfo info = new OpenDocumentTableColumnInfo(grid[i].Width, name, useOptimalColumnWidth);
				info.Width = documentModel.ToDocumentLayoutUnitConverter.ToModelUnits(grid.Columns[i].Width);
				Add(info);
			}
		}
	}
#endregion
#region ImportHelper
	public static class ImportHelper {
#region TranslationTables
		static readonly UnderlineTypeTable underlineThinTypeTable = CreateUnderlineThinTypeTable();
		static readonly UnderlineTypeTable underlineThickTypeTable = CreateUnderlineThickTypeTable(); 
		static readonly UnderlineTypeTable underlineDoubleTypeTable = CreateUnderlineDoubleTypeTable();
		static readonly HorizontalPositionAlignmentTable horizontalPositionAlignmentTable = CreateHorizontalPositionAlignmentTable();
		static readonly VerticalPositionAlignmentTable verticalPositionAlignmentTable = CreateVerticalPositionAlignmentTable();
		static readonly VerticalPositionTypeTable verticalPositionTypeTable = CreateVerticalPositionTypeTable();
		static readonly HorizontalPositionTypeTable horizontalPositionTypeTable = CreateHorizontalPositionTypeTable();
		static readonly InverseHorizontalPositionAlignmentTable inverseHorizontalPositionAlignmentTable = CreateInverseHorizontalPositionAlignmentTable();
		static readonly InverseVerticalPositionAlignmentTable inverseVerticalPositionAlignmentTable = CreateInverseVerticalPositionAlignmentTable();
		static readonly InverseVerticalPositionTypeTable inverseVerticalPositionTypeTable = CreateInverseVerticalPositionTypeTable();
		static readonly InverseHorizontalPositionTypeTable inverseHorizontalPositionTypeTable = CreateInverseHorizontalPositionTypeTable();
		static readonly TextWrapTypeTable textWrapTypeTable = CreateTextWrapTypeTable();
		static readonly TextWrapSideTable textWrapSideTable = CreateTextWrapSideTable();
		static readonly StrikeoutTypeTable fontStrikeoutThinTypeTable = CreateFontStrikeoutThinTypeTable();
		static readonly StrikeoutTypeTable fontStrikeoutThickTypeTable = CreateFontStrikeoutThickTypeTable();
		static readonly OpenDocumentPageUsageTypeTable pageUsageTypeTable = CreatePageUsageTypeTable(); 
		public static UnderlineTypeTable UnderlineThinTypeTable { get { return underlineThinTypeTable; } }			   
		public static UnderlineTypeTable UnderlineThickTypeTable { get { return underlineThickTypeTable; } }
		public static UnderlineTypeTable UnderlineDoubleTypeTable { get { return underlineDoubleTypeTable; } }
		public static StrikeoutTypeTable FontStrikeoutThinTypeTable { get { return fontStrikeoutThinTypeTable; } }
		public static HorizontalPositionAlignmentTable HorizontalPositionAlignmentTable { get { return horizontalPositionAlignmentTable; } }
		public static VerticalPositionAlignmentTable VerticalPositionAlignmentTable { get { return verticalPositionAlignmentTable; } }
		public static HorizontalPositionTypeTable HorizontalPositionTypeTable { get { return horizontalPositionTypeTable; } }
		public static VerticalPositionTypeTable VerticalPositionTypeTable { get { return verticalPositionTypeTable; } }
		public static InverseHorizontalPositionAlignmentTable InverseHorizontalPositionAlignmentTable { get { return inverseHorizontalPositionAlignmentTable; } }
		public static InverseVerticalPositionAlignmentTable InverseVerticalPositionAlignmentTable { get { return inverseVerticalPositionAlignmentTable; } }
		public static InverseHorizontalPositionTypeTable InverseHorizontalPositionTypeTable { get { return inverseHorizontalPositionTypeTable; } }
		public static InverseVerticalPositionTypeTable InverseVerticalPositionTypeTable { get { return inverseVerticalPositionTypeTable; } }
		public static TextWrapTypeTable TextWrapTypeTable { get { return textWrapTypeTable; } }
		public static TextWrapSideTable TextWrapSideTable { get { return textWrapSideTable; } }
		public static StrikeoutTypeTable FontStrikeoutThickTypeTable { get { return fontStrikeoutThickTypeTable; } }
		public static OpenDocumentPageUsageTypeTable PageUsageTypeTable { get { return pageUsageTypeTable; } }
		static UnderlineTypeTable CreateUnderlineThinTypeTable() {
			UnderlineTypeTable result = new UnderlineTypeTable();
			result.Add(String.Empty, UnderlineType.None);
			result.Add("none", UnderlineType.None);
			result.Add("solid", UnderlineType.Single);
			result.Add("dotted", UnderlineType.Dotted);
			result.Add("dash", UnderlineType.Dashed);
			result.Add("long-dash", UnderlineType.LongDashed);
			result.Add("dot-dash", UnderlineType.DashDotted);
			result.Add("dot-dot-dash", UnderlineType.DashDotDotted);
			result.Add("wave", UnderlineType.Wave);
			return result;
		}
		private static TextWrapSideTable CreateTextWrapSideTable() {
			TextWrapSideTable result = new TextWrapSideTable();
			result.Add("left", FloatingObjectTextWrapSide.Left);
			result.Add("right", FloatingObjectTextWrapSide.Right);
			result.Add("parallel", FloatingObjectTextWrapSide.Both);
			result.Add("none", FloatingObjectTextWrapSide.Both);
			result.Add("dynamic", FloatingObjectTextWrapSide.Largest);
			result.Add("run-through", FloatingObjectTextWrapSide.Largest);
			result.Add("biggest", FloatingObjectTextWrapSide.Largest);
			return result;
		}
		private static TextWrapTypeTable CreateTextWrapTypeTable() {
			TextWrapTypeTable result = new TextWrapTypeTable();
			result.Add("none", FloatingObjectTextWrapType.TopAndBottom);
			result.Add("left", FloatingObjectTextWrapType.Square);
			result.Add("right", FloatingObjectTextWrapType.Square);
			result.Add("parallel", FloatingObjectTextWrapType.Square);
			result.Add("dynamic", FloatingObjectTextWrapType.Square);
			result.Add("run-through", FloatingObjectTextWrapType.Through);
			result.Add("biggest", FloatingObjectTextWrapType.Square);
			return result;
		}
		private static InverseHorizontalPositionAlignmentTable CreateInverseHorizontalPositionAlignmentTable() {
			InverseHorizontalPositionAlignmentTable result = new InverseHorizontalPositionAlignmentTable();
			result.Add(FloatingObjectHorizontalPositionAlignment.Center, "center");
			result.Add(FloatingObjectHorizontalPositionAlignment.Inside, "inside");
			result.Add(FloatingObjectHorizontalPositionAlignment.Left, "left");
			result.Add(FloatingObjectHorizontalPositionAlignment.Outside, "outside");
			result.Add(FloatingObjectHorizontalPositionAlignment.Right, "right");
			result.Add(FloatingObjectHorizontalPositionAlignment.None, "from-left");
			return result;
		}
		private static InverseVerticalPositionAlignmentTable CreateInverseVerticalPositionAlignmentTable() {
			InverseVerticalPositionAlignmentTable result = new InverseVerticalPositionAlignmentTable();
			result.Add(FloatingObjectVerticalPositionAlignment.Bottom, "bottom");
			result.Add(FloatingObjectVerticalPositionAlignment.Center, "middle");
			result.Add(FloatingObjectVerticalPositionAlignment.Top, "top");
			result.Add(FloatingObjectVerticalPositionAlignment.None, "from-top");
			return result;
		}
		private static InverseVerticalPositionTypeTable CreateInverseVerticalPositionTypeTable() {
			InverseVerticalPositionTypeTable result = new InverseVerticalPositionTypeTable();
			result.Add(FloatingObjectVerticalPositionType.Page, "page");
			result.Add(FloatingObjectVerticalPositionType.Paragraph, "paragraph");
			result.Add(FloatingObjectVerticalPositionType.Line, "line");
			return result;
		}
		private static InverseHorizontalPositionTypeTable CreateInverseHorizontalPositionTypeTable() {
			InverseHorizontalPositionTypeTable result = new InverseHorizontalPositionTypeTable();
			result.Add(FloatingObjectHorizontalPositionType.Character, "char");
			result.Add(FloatingObjectHorizontalPositionType.Column, "paragraph");
			result.Add(FloatingObjectHorizontalPositionType.LeftMargin, "frame-start-margin");
			result.Add(FloatingObjectHorizontalPositionType.Page, "page");
			result.Add(FloatingObjectHorizontalPositionType.RightMargin, "frame-end-margin");
			return result;
		}
		private static VerticalPositionTypeTable CreateVerticalPositionTypeTable() {
			VerticalPositionTypeTable result = new VerticalPositionTypeTable();
			result.Add("page", FloatingObjectVerticalPositionType.Page);
			result.Add("page-content", FloatingObjectVerticalPositionType.Page);
			result.Add("paragraph", FloatingObjectVerticalPositionType.Paragraph);
			result.Add("paragraph-content", FloatingObjectVerticalPositionType.Paragraph);
			result.Add("line", FloatingObjectVerticalPositionType.Line);
			result.Add("baseline", FloatingObjectVerticalPositionType.Line);
			result.Add("frame", FloatingObjectVerticalPositionType.Paragraph);
			result.Add("frame-content", FloatingObjectVerticalPositionType.Paragraph);
			result.Add("char", FloatingObjectVerticalPositionType.Line);
			result.Add("Text", FloatingObjectVerticalPositionType.Line);
			return result;
		}
		private static HorizontalPositionTypeTable CreateHorizontalPositionTypeTable() {
			HorizontalPositionTypeTable result = new HorizontalPositionTypeTable();
			result.Add("char", FloatingObjectHorizontalPositionType.Character);
			result.Add("frame", FloatingObjectHorizontalPositionType.Column);
			result.Add("frame-content", FloatingObjectHorizontalPositionType.Column);
			result.Add("paragraph", FloatingObjectHorizontalPositionType.Column);
			result.Add("paragraph-content", FloatingObjectHorizontalPositionType.Column);
			result.Add("page-start-margin", FloatingObjectHorizontalPositionType.Column);
			result.Add("frame-start-margin", FloatingObjectHorizontalPositionType.LeftMargin);
			result.Add("paragraph-start-margin", FloatingObjectHorizontalPositionType.Column);
			result.Add("page-content", FloatingObjectHorizontalPositionType.Page);
			result.Add("page", FloatingObjectHorizontalPositionType.Page);
			result.Add("page-end-margin", FloatingObjectHorizontalPositionType.RightMargin);
			result.Add("frame-end-margin", FloatingObjectHorizontalPositionType.RightMargin);
			result.Add("paragraph-end-margin", FloatingObjectHorizontalPositionType.RightMargin);
			return result;
		}
		static HorizontalPositionAlignmentTable CreateHorizontalPositionAlignmentTable() {
			HorizontalPositionAlignmentTable result = new HorizontalPositionAlignmentTable();
			result.Add("left", FloatingObjectHorizontalPositionAlignment.Left);
			result.Add("from-left", FloatingObjectHorizontalPositionAlignment.None);
			result.Add("inside", FloatingObjectHorizontalPositionAlignment.Inside);
			result.Add("from-inside", FloatingObjectHorizontalPositionAlignment.Inside);
			result.Add("center", FloatingObjectHorizontalPositionAlignment.Center);
			result.Add("outside", FloatingObjectHorizontalPositionAlignment.Outside);
			result.Add("right", FloatingObjectHorizontalPositionAlignment.Right);
			return result;
		}
		static VerticalPositionAlignmentTable CreateVerticalPositionAlignmentTable() {
			VerticalPositionAlignmentTable result = new VerticalPositionAlignmentTable();
			result.Add("top", FloatingObjectVerticalPositionAlignment.Top);
			result.Add("from-top", FloatingObjectVerticalPositionAlignment.None);
			result.Add("bottom", FloatingObjectVerticalPositionAlignment.Bottom);
			result.Add("below", FloatingObjectVerticalPositionAlignment.Bottom);
			result.Add("middle", FloatingObjectVerticalPositionAlignment.Center); 
			return result;
		}
		static UnderlineTypeTable CreateUnderlineThickTypeTable() {
			UnderlineTypeTable result = new UnderlineTypeTable();
			result.Add(String.Empty, UnderlineType.None);
			result.Add("none", UnderlineType.None);
			result.Add("solid", UnderlineType.ThickSingle);
			result.Add("dotted", UnderlineType.ThickDotted);
			result.Add("dash", UnderlineType.ThickDashed);
			result.Add("long-dash", UnderlineType.ThickLongDashed);
			result.Add("dot-dash", UnderlineType.ThickDashDotted);
			result.Add("dot-dot-dash", UnderlineType.ThickDashDotDotted);
			result.Add("wave", UnderlineType.HeavyWave);
			return result;
		}
		static UnderlineTypeTable CreateUnderlineDoubleTypeTable() {
			UnderlineTypeTable result = new UnderlineTypeTable();
			result.Add(String.Empty, UnderlineType.Double);
			result.Add("none", UnderlineType.Double);
			result.Add("solid", UnderlineType.Double);
			result.Add("dotted", UnderlineType.Dotted);
			result.Add("dash", UnderlineType.Dashed);
			result.Add("long-dash", UnderlineType.LongDashed);
			result.Add("dot-dash", UnderlineType.DashDotted);
			result.Add("dot-dot-dash", UnderlineType.DashDotDotted);
			result.Add("wave", UnderlineType.DoubleWave);
			return result;
		}
		static StrikeoutTypeTable CreateFontStrikeoutThinTypeTable() {
			StrikeoutTypeTable result = new StrikeoutTypeTable();
			result.Add("none", StrikeoutType.None);
			result.Add("solid", StrikeoutType.Single);
			result.Add("double", StrikeoutType.Double);
			return result;
		}
		static StrikeoutTypeTable CreateFontStrikeoutThickTypeTable() {
			StrikeoutTypeTable result = new StrikeoutTypeTable();
			result.Add("none", StrikeoutType.None);
			result.Add("solid", StrikeoutType.Double);
			return result;
		}
		static OpenDocumentPageUsageTypeTable CreatePageUsageTypeTable() {
			OpenDocumentPageUsageTypeTable result = new OpenDocumentPageUsageTypeTable();
			result.Add("all", OpenDocumentPageUsageType.All);
			result.Add("left", OpenDocumentPageUsageType.Left);
			result.Add("right", OpenDocumentPageUsageType.Right);
			result.Add("Mirrored", OpenDocumentPageUsageType.Mirrored);
			return result;
		}
#endregion
		public const string WhitespaceString = " ";
		public static void ConvertLineSpacingFromString(string value, UnitsConverter unitsConverter, out ParagraphLineSpacing type, out float lineSpacing) {
			ValueInfo valueUnit = StringValueParser.TryParse(value);
			type = ParagraphLineSpacing.Single;
			lineSpacing = valueUnit.Value;
			if (valueUnit.Unit == "%") {
				if (valueUnit.Value == 200) {
					type = ParagraphLineSpacing.Double;
				}
				else if (valueUnit.Value == 150)
					type = ParagraphLineSpacing.Sesquialteral;
				else if (valueUnit.Value == 100)
					type = ParagraphLineSpacing.Single;
				else {
					type = ParagraphLineSpacing.Multiple;
					lineSpacing = Math.Max(1, unitsConverter.ValueUnitToModelUnitsF(valueUnit));
				}
			}
			else {
				type = ParagraphLineSpacing.AtLeast;
				lineSpacing = Math.Max(1, unitsConverter.ValueUnitToModelUnitsF(valueUnit));
			}
		}
		internal static ValueInfo GetStringValueInfo(XmlReader reader, string attributeName, string ns) {
			string valueText = GetStringAttribute(reader, attributeName, ns);
			return StringValueParser.TryParse(valueText);
		}
		public static string GetFoStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.FoNamespace);
		}
		public static ValueInfo GetFoAttributeInfo(XmlReader reader, string attributeName) {
			return GetStringValueInfo(reader, attributeName, OpenDocumentHelper.FoNamespace);
		}
		public static ValueInfo GetDrawAttributeInfo(XmlReader reader, string attributeName) {
			return GetStringValueInfo(reader, attributeName, OpenDocumentHelper.DrawNamespace);
		}
		public static ValueInfo GetTextAttributeInfo(XmlReader reader, string attributeName) {
			return GetStringValueInfo(reader, attributeName, OpenDocumentHelper.TextNamespace);
		}
		public static ValueInfo GetStyleAttributeInfo(XmlReader reader, string attributeName) {
			return GetStringValueInfo(reader, attributeName, OpenDocumentHelper.StyleNamespace);
		}
		public static ValueInfo GetSvgAttributeInfo(XmlReader reader, string attributeName) {
			return GetStringValueInfo(reader, attributeName, OpenDocumentHelper.SvgNamespace);
		}
		public static string GetTextStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.TextNamespace);
		}
		public static int GetTextIntegerAttribute(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerAttribute(reader, attributeName, OpenDocumentHelper.TextNamespace, defaultValue);
		}
		public static int GetDrawIntegerAttribute(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerAttribute(reader, attributeName, OpenDocumentHelper.DrawNamespace, defaultValue);
		}
		public static string GetDrawStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.DrawNamespace);
		}
		public static int GetStyleIntegerAttribute(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerAttribute(reader, attributeName, OpenDocumentHelper.StyleNamespace, defaultValue);
		}
		public static int GetTableIntegerAttribute(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerAttribute(reader, attributeName, OpenDocumentHelper.TableNamespace, defaultValue);
		}
		public static int GetFoIntegerAttribute(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerAttribute(reader, attributeName, OpenDocumentHelper.FoNamespace, defaultValue);
		}
		static int GetIntegerAttribute(XmlReader reader, string attributeName, string ns, int defaultValue) {
			string val = GetStringAttribute(reader, attributeName, ns);
			int result = Int32.MinValue;
			if (Int32.TryParse(val, out result))
				return result;
			return defaultValue;
		}
		public static bool GetTextBoolAttribute(XmlReader reader, string attributeName, bool defaultValue) {
			return GetBoolAttribute(reader, attributeName, OpenDocumentHelper.TextNamespace, defaultValue);
		}
		public static bool GetBoolAttribute(XmlReader reader, string attributeName, string ns, bool defaultValue) {
			string val = GetStringAttribute(reader, attributeName, ns);
			bool result = false;
			if (Boolean.TryParse(val, out result))
				return result;
			return defaultValue;
		}
		public static string GetStyleStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.StyleNamespace);
		}
		public static string GetSvgStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.SvgNamespace);
		}
		public static string GetTableStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.TableNamespace);
		}
		public static bool GetStyleBoolAttribute(XmlReader reader, string attributeName, bool defaultValue) {
			return GetBoolAttribute(reader, attributeName, OpenDocumentHelper.StyleNamespace, defaultValue);
		}
		public static string GetXlinkStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.XlinkNamespace);
		}
		public static string GetOfficeStringAttribute(XmlReader reader, string attributeName) {
			return GetStringAttribute(reader, attributeName, OpenDocumentHelper.OfficeNamespace);
		}
		internal static string GetStringAttribute(XmlReader reader, string attributeName, string ns) {
			string value = reader.GetAttribute(attributeName, ns);
			return String.IsNullOrEmpty(value) ? String.Empty : value;
		}
		public static int GetIntegerDocumentValue(ValueInfo valueInfo, UnitsConverter unitConverter) {
			return (int)Math.Round((double)unitConverter.ValueUnitToModelUnitsF(valueInfo));
		}
		public static float GetFloatDocumentValue(ValueInfo valueInfo, UnitsConverter unitConverter) {
			return unitConverter.ValueUnitToModelUnitsF(valueInfo);
		}
		public static StrikeoutType GetFontStrikeoutType(string strikeoutType, string textLineThroughStyle) {
			if (strikeoutType == "double")
				return FontStrikeoutThickTypeTable.GetEnumValue(textLineThroughStyle);
			else
				return FontStrikeoutThinTypeTable.GetEnumValue(textLineThroughStyle);
		}
#region GetUnderlineType
		public static UnderlineType GetUnderlineType(string style, bool bold, string type) {
			string actualType = type.ToLower(CultureInfo.InvariantCulture);
			if (actualType == "double")
				return UnderlineDoubleTypeTable.GetEnumValue(style);
			UnderlineType result = GetUnderlineSingleType(style, bold);
			if (result == UnderlineType.None && actualType == "single")
				result = GetUnderlineSingleType("solid", bold);
			return result;
		}
		static UnderlineType GetUnderlineSingleType(string style, bool bold) {
			return bold ? UnderlineThickTypeTable.GetEnumValue(style) : UnderlineThinTypeTable.GetEnumValue(style);
		}
#endregion
		public static Color ConvertStringToColor(string color) {
			if (color == "transparent")
				return DXColor.Transparent;
			return MarkupLanguageColorParser.ParseColor(color);
		}
		public static string RemoveRedundantSpaces(string text, bool keepBeginingSpaces) {
			if (String.IsNullOrEmpty(text))
				return text;
			char lastCharacter = text[text.Length - 1];
			bool lastSpace = lastCharacter == ' ' || lastCharacter == Characters.NonBreakingSpace;
			char firstCharacter = text[0];
			bool startSpace = firstCharacter == ' ' || firstCharacter == Characters.NonBreakingSpace;
			text = (lastSpace) ? text.Trim() + " " : text.Trim();
			if (keepBeginingSpaces && startSpace)
				text = " " + text;
			text = text.Replace('\n', ' ');
			int length = text.Length;
			for (; ; ) {
				text = text.Replace("  ", " ");
				if (text.Length == length)
					break;
				length = text.Length;
			}
			return text;
		}
		public static void ImportBreakType(XmlReader reader, ParagraphBreaksInfo breaksInfo, ParagraphFormattingBase paragraphFormatting) {
			string breakBefore = ImportHelper.GetFoStringAttribute(reader, "break-before");
			string breakAfter = ImportHelper.GetFoStringAttribute(reader, "break-after");
			if (breakBefore == "page")
				paragraphFormatting.PageBreakBefore = true;
			else if (breakBefore == "column")
				breaksInfo.BreakBefore = ParagraphBreakType.Column;
			if (breakAfter == "page")
				breaksInfo.BreakAfter = ParagraphBreakType.Page;
			else if (breakAfter == "column")
				breaksInfo.BreakAfter = ParagraphBreakType.Column;
		}
		public static bool ReadToRootElement(XmlReader reader, string name) {
			return ReadToRootElement(reader, name, OpenDocumentHelper.OfficeNamespace);
		}
		public static bool ReadToRootElement(XmlReader reader, string name, string ns) {
			try {
				return reader.ReadToFollowing(name, ns);
			}
			catch {
				return false;
			}
		}
		public static BorderInfo ParseBordersContent(string content, UnitsConverter converter){
			BorderInfo info = new BorderInfo();
			if (content == "none") {
				info.Style = BorderLineStyle.None;
				info.Width = 0;
				return info;
			}
			string[] substrings = content.Split(' ');
			if (substrings.Length != 3)
				return null;
			ValueInfo width = StringValueParser.TryParse(substrings[0]);
			info.Width = ImportHelper.GetIntegerDocumentValue(width, converter);
			info.Style = OpenDocumentHelper.LineTypeTable.GetEnumValue(substrings[1], BorderLineStyle.Single);
			info.Color = MarkupLanguageColorParser.ParseColor(substrings[2]);
			return info;
		}
		public static BorderLineStyle GetBorderLineStyle(string borderLineWidth, UnitsConverter unitConverter) {
			BorderLineStyle result = BorderLineStyle.None;
			string[] lineWidths = borderLineWidth.Split(' ');
			if (lineWidths.Length != 3)
				return result;
			ValueInfo inner = StringValueParser.TryParse(lineWidths[0]);
			ValueInfo gap = StringValueParser.TryParse(lineWidths[1]);
			ValueInfo outer = StringValueParser.TryParse(lineWidths[2]);
			if (!(inner.IsValidNumber && gap.IsValidNumber && outer.IsValidNumber))
				return result;
			float innerWidth = GetFloatDocumentValue(inner, unitConverter);
			float gapWidth = GetFloatDocumentValue(gap, unitConverter);
			float outerWidth = GetFloatDocumentValue(outer, unitConverter);
			float totalWidth = innerWidth + gapWidth + outerWidth;
			float[] compoundArray = new float[] { innerWidth, gapWidth, outerWidth };
			compoundArray[0] /= totalWidth;
			compoundArray[1] /= totalWidth;
			compoundArray[2] /= totalWidth;
			int lineCount = compoundArray.Length - 1; 
			double minDiff = float.MaxValue;
			foreach (KeyValuePair<BorderLineStyle, DevExpress.XtraRichEdit.Layout.TableBorderCalculator.TableBorderInfo> item in 
				DevExpress.XtraRichEdit.Layout.TableBorderCalculator.LineStyleInfos) {
				DevExpress.XtraRichEdit.Layout.TableBorderCalculator.TableBorderInfo borderInfo = item.Value;
				if (borderInfo.LineCount == lineCount) {
					float outerBorderWidth = borderInfo.DrawingCompoundArray[1] - borderInfo.DrawingCompoundArray[0];
					float gapBorderWidth = borderInfo.DrawingCompoundArray[2] - borderInfo.DrawingCompoundArray[1];
					float innerBorderWidth = borderInfo.DrawingCompoundArray[3] - borderInfo.DrawingCompoundArray[2];
					float diff0 = compoundArray[0] - outerBorderWidth;
					float diff1 = compoundArray[1] - gapBorderWidth;
					float diff2 = compoundArray[2] - innerBorderWidth;
					double standardDeviation = Math.Sqrt((diff0 * diff0 + diff1 * diff1 + diff2 * diff2) / 3);
					if (standardDeviation < minDiff) {
						minDiff = standardDeviation;
						result = item.Key;
					}
				}
			}
			return result;
		}
	}
#endregion
#region OpenDocumentPageUsageType
	public enum OpenDocumentPageUsageType {
		All,
		Left,
		Right,
		Mirrored
	}
#endregion
#region UnitsConverter
	public class UnitsConverter {
		readonly DocumentModelUnitConverter unitConverter;
		public UnitsConverter(DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public float ValueUnitToModelUnitsF(ValueInfo info) {
			if (String.IsNullOrEmpty(info.Unit))
				return info.Value;
			switch (info.Unit.ToLower(CultureInfo.InvariantCulture)) {
				case "km":
					return unitConverter.CentimetersToModelUnitsF(info.Value * 100000);
				case "m":
					return unitConverter.CentimetersToModelUnitsF(info.Value * 100);
				case "cm":
					return unitConverter.CentimetersToModelUnitsF(info.Value);
				case "mm":
					return unitConverter.MillimetersToModelUnitsF(info.Value);
				case "in":
					return unitConverter.InchesToModelUnitsF(info.Value);
				case "pt":
					return unitConverter.PointsToModelUnitsF(info.Value);
				case "pc":
					return unitConverter.PicasToModelUnitsF(info.Value);
				case "inch":
					return unitConverter.InchesToModelUnitsF(info.Value);
				case "ft":
					return unitConverter.InchesToModelUnitsF(info.Value * 12); 
				case "mi":
				case "%":
					return info.Value / 100;
			}
			return info.Value;
		}
	}
#endregion
#region ApplyHeaderToSectionHelper
	public class ApplyHeaderToSectionHelper : ApplyHeaderFooterToSectionHelper {
		public ApplyHeaderToSectionHelper(Section section, MasterPageStyleInfo masterPage)
			: base(section, masterPage) {
		}
		protected internal override bool SourceAvailable() {
			return MasterPage.IsHeadersAvailable;
		}
		protected internal override void ProcessFirstPage() {
			if (MasterPage.HeaderIndex == HeaderIndex.Invalid) {
				Copy(MasterPage.Header, CreateFirstPage());
				MasterPage.HeaderIndex = Section.InnerFirstPageHeaderIndex;
			}
			else {
				Section.InnerFirstPageHeaderIndex = MasterPage.HeaderIndex;
				if (Section.InnerFirstPageHeader.Type != HeaderFooterType.First)
					Copy(MasterPage.Header, CreateFirstPage());
			}
		}
		protected internal override void ProcessOddPage() {
			if (MasterPage.HeaderIndex == HeaderIndex.Invalid) {
				Copy(MasterPage.Header, CreateOdd());
				MasterPage.HeaderIndex = Section.InnerOddPageHeaderIndex;
			}
			else {
				Section.InnerOddPageHeaderIndex = MasterPage.HeaderIndex;
				if (Section.InnerOddPageHeader.Type != HeaderFooterType.Odd)
					Copy(MasterPage.Header, CreateOdd());
			}
		}
		protected internal override void ProcessEvenPage() {
			if (MasterPage.HeaderLeftIndex == HeaderIndex.Invalid) {
				Copy(MasterPage.HeaderLeft, CreateEven());
				MasterPage.HeaderLeftIndex = Section.InnerEvenPageHeaderIndex;
			}
			else {
				Section.InnerEvenPageHeaderIndex = MasterPage.HeaderLeftIndex;
				if (Section.InnerEvenPageHeader.Type != HeaderFooterType.Even)
					Copy(MasterPage.HeaderLeft, CreateEven());
			}
		}
		protected internal override SectionHeaderFooterBase CreateOdd() {
			Section.Headers.Create(HeaderFooterType.Odd);
			return Section.InnerOddPageHeader;
		}
		protected internal override SectionHeaderFooterBase CreateEven() {
			Section.Headers.Create(HeaderFooterType.Even);
			return Section.InnerEvenPageHeader;
		}
		protected internal override SectionHeaderFooterBase CreateFirstPage() {
			Section.Headers.Create(HeaderFooterType.First);
			return Section.InnerFirstPageHeader;
		}
		protected internal override void RemovePreviousHeaderFooter() {
			if (MasterPage.Header == null) {
				if (Section.InnerOddPageHeader != null)
					Section.Headers.Remove(HeaderFooterType.Odd);
				if (Section.InnerEvenPageHeader != null) {
					Section.Headers.Remove(HeaderFooterType.Even);
					return;
				}
			}
			if (MasterPage.HeaderLeft == null) {
				if (Section.InnerEvenPageHeader != null)
					Section.Headers.Remove(HeaderFooterType.Even);
			}
		}
	}
#endregion
#region ApplyFooterToSectionHelper
	public class ApplyFooterToSectionHelper : ApplyHeaderFooterToSectionHelper {
		public ApplyFooterToSectionHelper(Section section, MasterPageStyleInfo masterPage)
			: base(section, masterPage) {
		}
		protected internal override void ProcessOddPage() {
			if (MasterPage.FooterIndex == FooterIndex.Invalid) {
				Copy(MasterPage.Footer, CreateOdd());
				MasterPage.FooterIndex = Section.InnerOddPageFooterIndex;
			}
			else {
				Section.InnerOddPageFooterIndex = MasterPage.FooterIndex;
				if (Section.InnerOddPageFooter.Type != HeaderFooterType.Odd)
					Copy(MasterPage.Footer, CreateOdd());
			}
		}
		protected internal override void ProcessEvenPage() {
			if (MasterPage.FooterLeftIndex == FooterIndex.Invalid) {
				Copy(MasterPage.FooterLeft, CreateEven());
				MasterPage.FooterLeftIndex = Section.InnerEvenPageFooterIndex;
			}
			else {
				Section.InnerEvenPageFooterIndex = MasterPage.FooterLeftIndex;
				if (Section.InnerEvenPageFooter.Type != HeaderFooterType.Even)
					Copy(MasterPage.FooterLeft, CreateEven());
			}
		}
		protected internal override void ProcessFirstPage() {
			if (MasterPage.FooterIndex == FooterIndex.Invalid) {
				Copy(MasterPage.Footer, CreateFirstPage());
				MasterPage.FooterIndex = Section.InnerFirstPageFooterIndex;
			}
			else {
				Section.InnerFirstPageFooterIndex = MasterPage.FooterIndex;
				if (Section.InnerFirstPageFooter.Type != HeaderFooterType.First)
					Copy(MasterPage.Footer, CreateFirstPage());
			}
		}
		protected internal override bool SourceAvailable() {
			return MasterPage.IsFootersAvailable;
		}
		protected internal override SectionHeaderFooterBase CreateOdd() {
			Section.Footers.Create(HeaderFooterType.Odd);
			return Section.InnerOddPageFooter;
		}
		protected internal override SectionHeaderFooterBase CreateEven() {
			Section.Footers.Create(HeaderFooterType.Even);
			return Section.InnerEvenPageFooter;
		}
		protected internal override SectionHeaderFooterBase CreateFirstPage() {
			Section.Footers.Create(HeaderFooterType.First);
			return Section.InnerFirstPageFooter;
		}
		protected internal override void RemovePreviousHeaderFooter() {
			if (MasterPage.Footer == null) {
				if (Section.InnerOddPageFooter != null)
					Section.Footers.Remove(HeaderFooterType.Odd);
				if (Section.InnerEvenPageFooter != null) {
					Section.Footers.Remove(HeaderFooterType.Even);
					return;
				}
			}
			if (MasterPage.FooterLeft == null) {
				if (Section.InnerEvenPageFooter != null)
					Section.Footers.Remove(HeaderFooterType.Even);
			}
		}
	}
#endregion
#region ApplyHeaderFooterToSectionHelper ( abstract class)
	public abstract class ApplyHeaderFooterToSectionHelper {
#region Field
		readonly Section section;
		readonly MasterPageStyleInfo mpStyleInfo;
#endregion
		protected ApplyHeaderFooterToSectionHelper(Section section, MasterPageStyleInfo masterPageStyleInfo) {
			Guard.ArgumentNotNull(section, "section");
			Guard.ArgumentNotNull(masterPageStyleInfo, "MasterPageStyleInfo");
			this.section = section;
			this.mpStyleInfo = masterPageStyleInfo;
		}
#region Properties
		public Section Section { get { return section; } }
		public MasterPageStyleInfo MasterPage { get { return mpStyleInfo; } }
#endregion
		public virtual void Process(bool isFirstPage) {
			if (!SourceAvailable()) {
				if (!isFirstPage)
					RemovePreviousHeaderFooter();
				return;
			}
			if (isFirstPage)
				ProcessFirstPage();
			else {
				if (Section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages) {
					ProcessOddPage();
					ProcessEvenPage();
				}
				else
					ProcessOddPage();
			}
		}
		public virtual void Copy(SectionHeaderFooterBase source, SectionHeaderFooterBase destination) {
			CopyCore(source, destination);
			destination.PieceTable.FixLastParagraph();
		}
		protected internal virtual void CopyCore(SectionHeaderFooterBase source, SectionHeaderFooterBase destination) {
			if (source == null)
				return;
			PieceTable sourcePieceTable = source.PieceTable;
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(sourcePieceTable, destination.PieceTable, ParagraphNumerationCopyOptions.CopyIfWholeSelected);
			DocumentLogPosition startLogPosition = sourcePieceTable.DocumentStartLogPosition;
			DocumentLogPosition endLogPosition = sourcePieceTable.DocumentEndLogPosition;
			CopySectionOperation operation = sourcePieceTable.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.Execute(startLogPosition, endLogPosition - startLogPosition, false);
		}
		protected internal abstract bool SourceAvailable();
		protected internal abstract void RemovePreviousHeaderFooter();
		protected internal abstract void ProcessOddPage();
		protected internal abstract void ProcessEvenPage();
		protected internal abstract void ProcessFirstPage();
		protected internal abstract SectionHeaderFooterBase CreateOdd();
		protected internal abstract SectionHeaderFooterBase CreateEven();
		protected internal abstract SectionHeaderFooterBase CreateFirstPage();
	}
#endregion
}
