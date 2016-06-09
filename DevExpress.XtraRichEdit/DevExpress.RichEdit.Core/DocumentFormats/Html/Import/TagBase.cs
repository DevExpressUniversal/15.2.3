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
using DevExpress.XtraRichEdit.Model;
using System.IO;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using System.Globalization;
using System.Text;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region TagBaseHelper
	public class TagBaseHelper {
		readonly HtmlImporter importer;
		int currentTagIndex;
		TagBase tagBase;
		public TagBaseHelper(HtmlImporter importer, TagBase tagBase) {
			this.importer = importer;
			this.tagBase = tagBase;
		}
		#region Properties
		HtmlInputPosition OldPosition {
			get {
				int count = Importer.TagsStack.Count;
				return count > 0 ? Importer.TagsStack[count - 1].OldPosition : null;
			}
		}
		protected internal HtmlImporter Importer { get { return importer; } }
		#endregion
		protected internal ParagraphFormattingOptions ApplyCssProperties(CssProperties styleAttributeProperties) {
			ParagraphFormattingOptions options = ApplyCssPropertiesCore(styleAttributeProperties);
			if (IsFontSizeChanged()) {
				options.Value |= ApplyCssPropertiesCore(styleAttributeProperties).Value;
			}
			return options;
		}
		bool IsFontSizeChanged() {
			if (OldPosition == null)
				return false;
			return OldPosition.CharacterFormatting.DoubleFontSize != importer.Position.CharacterFormatting.DoubleFontSize;
		}
		ParagraphFormattingOptions ApplyCssPropertiesCore(CssProperties styleAttributeProperties) {
			CssElementCollection styleTagCollection = Importer.StyleTagCollection;
			int count = styleTagCollection.Count;
			ParagraphFormattingOptions paragraphOptions = new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone);
			for (int i = 0; i < count; i++) {
				if (CheckSelector(styleTagCollection[i].Selector)) {
					ParagraphFormattingOptions overridenOptions = OverrideProperties(GetRelativeProperties(styleTagCollection[i].Properties));
					paragraphOptions.Value |= overridenOptions.Value;
				}
			}
			if (styleAttributeProperties != null) {
				ParagraphFormattingOptions overridenOptions = OverrideProperties(GetRelativeProperties(styleAttributeProperties));
				paragraphOptions.Value |= overridenOptions.Value;
			}
			return paragraphOptions;
		}
		protected internal bool CheckSelector(Selector selector) {
			Combinator combinator = Combinator.None;
			IList<SelectorElement> elements = selector.Elements;
			for (int i = elements.Count - 1; i >= 0; i--) {
				if (CheckSelectorCore(elements[i], combinator)) {
					if (elements[i].Combinator == Combinator.None)
						return true;
					combinator = elements[i].Combinator;
				}
				else
					return false;
			}
			return false;
		}
		protected internal bool CheckSelectorCore(SelectorElement element, Combinator combinator) {
			switch (combinator) {
				case Combinator.None:
					currentTagIndex = GetCurrentTagIndex();
					return IsTrueSelector(element.SimpleSelector);
				case Combinator.WhiteSpace:
					return CheckAncestorTag(element);
				case Combinator.PlusSign:
					break;
				case Combinator.RightAngle:
					return CheckParentTag(element);
			}
			return true;
		}
		int GetCurrentTagIndex() {
			List<OpenHtmlTag> tags = Importer.TagsStack;			
			for (int i = Importer.TagsStack.Count - 1; i >= 0; i--)
				if (tagBase == tags[i].Tag)
					return i;
			return Importer.TagsStack.Count - 1;
		}
		protected internal bool CheckParentTag(SelectorElement element) {
			int index = currentTagIndex - 1;
			if (index >= 0) {
				currentTagIndex = index;
				return IsTrueSelector(element.SimpleSelector);
			}
			return false;
		}
		protected internal bool CheckAncestorTag(SelectorElement element) {
			int index = currentTagIndex;
			for (int i = 0; i < index; i++) {
				currentTagIndex = i;
				if (IsTrueSelector(element.SimpleSelector))
					return true;
			}
			return false;
		}
		protected internal bool IsTrueSelector(SimpleSelector simpleSelector) {
			if (currentTagIndex >= Importer.TagsStack.Count || currentTagIndex < 0)
				return false;
			TagBase tagBase = Importer.TagsStack[currentTagIndex].Tag;
			return CheckSelectorName(simpleSelector, tagBase) &&
				   CheckSelectorId(simpleSelector, tagBase) &&
				   CheckSelectorClasses(simpleSelector, tagBase) &&
				   CheckSelectorPseudoClasses(simpleSelector, tagBase) &&
				   CheckSelectorAttributes(simpleSelector, tagBase);
		}
		protected internal bool CheckSelectorName(SimpleSelector simpleSelector, TagBase tagBase) {
			HtmlTagNameID simpleSelectorName = this.Importer.GetTagNameID(simpleSelector.Name);
			return tagBase.Name == simpleSelectorName || String.IsNullOrEmpty(simpleSelector.Name) || simpleSelector.Name == "*";
		}
		protected internal bool CheckSelectorId(SimpleSelector simpleSelector, TagBase tagBase) {
			return tagBase.Id == simpleSelector.Id || String.IsNullOrEmpty(simpleSelector.Id);
		}
		protected internal bool CheckSelectorClasses(SimpleSelector simpleSelector, TagBase tagBase) {
			for (int i = 0; i < simpleSelector.Classes.Count; i++) {
				if (!tagBase.Classes.Contains(simpleSelector.Classes[i]))
					return false;
			}
			return true;
		}
		protected internal bool CheckSelectorPseudoClasses(SimpleSelector simpleSelector, TagBase tagBase) {
			if (simpleSelector.PseudoClasses.Count > 0)
				return false;
			return true;
		}
		protected internal bool CheckSelectorAttributes(SimpleSelector simpleSelector, TagBase tagBase) {
			for (int i = 0; i < simpleSelector.SelectorAttributes.Count; i++) {
				SelectorAttribute selectorAttributes = simpleSelector.SelectorAttributes[i];
				if (!CheckSelectorAttribute(selectorAttributes, tagBase))
					return false;
			}
			return true;
		}
		protected internal bool CheckSelectorAttribute(SelectorAttribute selectorAttribute, TagBase tagBase) {
			for (int i = 0; i < tagBase.Tag.Attributes.Count; i++) {
				Attribute attr = tagBase.Tag.Attributes[i];
				if (attr.Name == selectorAttribute.AttributeName) {
					if (CheckSelectorAttributeCore(selectorAttribute, attr))
						return true;
				}
			}
			return false;
		}
		protected internal bool CheckSelectorAttributeCore(SelectorAttribute selectorAttributes, Attribute attr) {
			string selectorAttrValue = selectorAttributes.AttributeValue;
			switch (selectorAttributes.AttributeConnector) {
				case "=":
					return attr.Value == selectorAttrValue;
				case "^=":
					return attr.Value.StartsWith(selectorAttrValue);
				case "$=":
					return attr.Value.EndsWith(selectorAttrValue);
				case "*=":
					return attr.Value.Contains(selectorAttrValue);
				case "~=":
					return attr.Value == selectorAttrValue;
				case "|=":
					return attr.Value == selectorAttrValue || attr.Value.StartsWith(selectorAttrValue + "-");
				default:
					return true;
			}
		}
		protected internal CssProperties GetRelativeProperties(CssProperties properties) {
			RelativeProperties relativeProperties = properties.RelativeProperties;
			if (!relativeProperties.HasRelativeProperties)
				return properties;
			CssProperties result = new CssProperties(Importer.DocumentModel.MainPieceTable);
			result.CopyFrom(properties);
			ParagraphFormattingBase cssParagraphProperties = result.CssParagraphProperties;
			RelativeProperties resultRelativeProperties = result.RelativeProperties;
			if (IsRelative(relativeProperties.UnitRelativeFirstLineIndent))
				cssParagraphProperties.FirstLineIndent = GetRelativeIndentProperties(resultRelativeProperties.UnitRelativeFirstLineIndent, cssParagraphProperties.FirstLineIndent);
			if (IsRelative(relativeProperties.UnitRelativeLeftIndent))
				cssParagraphProperties.LeftIndent = GetRelativeIndentProperties(resultRelativeProperties.UnitRelativeLeftIndent, cssParagraphProperties.LeftIndent);
			if (IsRelative(relativeProperties.UnitRelativeRightIndent))
				cssParagraphProperties.RightIndent = GetRelativeIndentProperties(resultRelativeProperties.UnitRelativeRightIndent, cssParagraphProperties.RightIndent);
			if (IsRelative(relativeProperties.UnitRelativeLineSpacing))
				cssParagraphProperties.LineSpacing = GetRelativeLineSpacingProperties(resultRelativeProperties.UnitRelativeLineSpacing, cssParagraphProperties);
			if (IsRelative(relativeProperties.UnitRelativeSpacingAfter))
				cssParagraphProperties.SpacingAfter = GetRelativeIndentProperties(resultRelativeProperties.UnitRelativeSpacingAfter, cssParagraphProperties.SpacingAfter);
			if (IsRelative(relativeProperties.UnitRelativeLineSpacing))
				cssParagraphProperties.SpacingBefore = GetRelativeIndentProperties(resultRelativeProperties.UnitRelativeSpacingBefore, cssParagraphProperties.SpacingBefore);
			if (IsRelative(relativeProperties.UnitRelativeFontSize)) {
				if(relativeProperties.UnitRelativeFontSize == "rem")
					result.CssCharacterProperties.DoubleFontSize = Math.Max(1, Convert.ToInt32(result.CssCharacterProperties.DoubleFontSize * Importer.RootDoubleFontSize / 2.0 / 100.0));
				else
					result.CssCharacterProperties.DoubleFontSize = Math.Max(1, Convert.ToInt32(result.CssCharacterProperties.DoubleFontSize * OldPosition.CharacterFormatting.DoubleFontSize / 2.0 / 100.0));
			}
			int fontSize = importer.Position.CharacterFormatting.DoubleFontSize / 2;
			if (IsRelative(relativeProperties.UnitRelativeWidth)) {
				WidthUnitInfo width = GetRelativeWidthProperties(relativeProperties.UnitRelativeWidth, relativeProperties.RelativeWidth, fontSize);
				WidthUnitInfo height = GetRelativeWidthProperties(relativeProperties.UnitRelativeHeight, relativeProperties.RelativeHeight, fontSize);
				result.CellProperties.PreferredWidth.CopyFrom(width);
				result.TableProperties.Width = width;
				result.ImageProperties.Width = width;
				result.ImageProperties.Height = height;
			}
			return result;
		}
		protected internal bool IsRelative(string relativeUnit) {
			return RelativeProperties.IsRelative(relativeUnit);
		}
		protected internal int GetRelativeIndentProperties(string relativeUnit, int cssProperties) {
			int pageWidth = Importer.DocumentModel.Sections.Last.Page.Width;
			if (relativeUnit == "%")
				return Convert.ToInt32(pageWidth * cssProperties / 100.0);
			int value = Convert.ToInt32(importer.Position.CharacterFormatting.DoubleFontSize / 2.0 * cssProperties / 100.0);
			return importer.UnitConverter.PointsToModelUnits(value);
		}
		protected internal float GetRelativeLineSpacingProperties(string relativeUnit, ParagraphFormattingBase cssProperties) {
			if (relativeUnit == "%") {
				float lineSpacing = cssProperties.LineSpacing / 100.0f;
				cssProperties.SetMultipleLineSpacing(lineSpacing);
				return lineSpacing;
			}
			float value = importer.Position.CharacterFormatting.DoubleFontSize * cssProperties.LineSpacing / 100.0f / 2f;
			return importer.UnitConverter.PointsToModelUnitsF(value);
		}
		protected internal WidthUnitInfo GetRelativeWidthProperties(string relativeUnit, float relativeValue, int fontSize) {
			return GetRelativeWidth(importer.UnitConverter, relativeUnit, relativeValue, fontSize, importer.RootDoubleFontSize);
		}
		internal static WidthUnitInfo GetRelativeWidth(IDocumentModelUnitConverter unitConverter, string relativeUnit, float relativeValue, int fontSize, int rootDoubleFontSize) {
			WidthUnitInfo info = new WidthUnitInfo();
			if (relativeUnit == "em") {
				info.Value = unitConverter.PointsToModelUnits((int)(relativeValue * fontSize));
				info.Type = WidthUnitType.ModelUnits;
				return info;
			}
			else if (relativeUnit == "ex") {
				info.Value = unitConverter.PointsToModelUnits((int)(relativeValue * fontSize * 2));
				info.Type = WidthUnitType.ModelUnits;
				return info;
			}
			else if (relativeUnit == "%") {
				info.Value = (int)(relativeValue * 50);
				info.Type = WidthUnitType.FiftiethsOfPercent;
				return info;
			}
			else if (relativeUnit == "rem") {
				info.Value = unitConverter.PointsToModelUnits((int)(relativeValue * rootDoubleFontSize / 100.0));
				info.Type = WidthUnitType.ModelUnits;
				return info;
			}
			return info;
		}
		protected internal ParagraphFormattingOptions OverrideProperties(CssProperties cssProperties) {
			HtmlInputPosition importerPosition = importer.Position;
			MergedCharacterProperties overriddenCharacterProperties = GetOverriddenCharacterProperties(cssProperties.CssCharacterProperties);
			importerPosition.CharacterFormatting.CopyFromCore(overriddenCharacterProperties.Info, overriddenCharacterProperties.Options);
			MergedParagraphProperties overriddenParagraphProperties = GetOverriddenParagraphProperties(cssProperties.CssParagraphProperties);
			ParagraphFormattingOptions result = new ParagraphFormattingOptions(cssProperties.CssParagraphProperties.Options.Value);
			importerPosition.ParagraphFormatting.CopyFromCore(overriddenParagraphProperties.Info, overriddenParagraphProperties.Options);
			importerPosition.ListLevelProperties.CopyFrom(cssProperties.ListLevelProperties.MergeWith(importerPosition.ListLevelProperties));
			importerPosition.TableProperties.CopyFrom(cssProperties.TableProperties.MergeWith(importerPosition.TableProperties));
			importerPosition.TableProperties.BordersProperties.CopyFrom(cssProperties.BordersProperties.MergeWith(importerPosition.TableProperties.BordersProperties));
			importerPosition.RowProperties.CopyFrom(cssProperties.RowProperties.MergeWith(importerPosition.RowProperties));
			importerPosition.ImageProperties.CopyFrom(cssProperties.ImageProperties.MergeWith(importerPosition.ImageProperties));
			MergedTableCellProperties overriddenCellProperties = GetOverriddenCellProperties(cssProperties.CellProperties);
			importerPosition.CellProperties.CopyFrom(overriddenCellProperties);
			TableCellBorders importerPositionCellPropertiesBorders = importerPosition.CellProperties.Borders;
			HtmlBordersProperties cssBordersProperties = cssProperties.BordersProperties;
			cssBordersProperties.TopBorder.Apply(importerPositionCellPropertiesBorders.TopBorder);
			cssBordersProperties.LeftBorder.Apply(importerPositionCellPropertiesBorders.LeftBorder);
			cssBordersProperties.RightBorder.Apply(importerPositionCellPropertiesBorders.RightBorder);
			cssBordersProperties.BottomBorder.Apply(importerPositionCellPropertiesBorders.BottomBorder);
			if (cssProperties.CssFloat != HtmlCssFloat.NotSet)
				importerPosition.CssFloat = cssProperties.CssFloat;
			return result;
		}
		protected internal MergedCharacterProperties GetOverriddenCharacterProperties(CharacterFormattingBase cssCharacterProperties) {
			MergedCharacterProperties newMergedCharacterProperties = new MergedCharacterProperties(cssCharacterProperties.Info, cssCharacterProperties.Options);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(newMergedCharacterProperties);
			merger.Merge(new MergedCharacterProperties(Importer.Position.CharacterFormatting.Info, Importer.Position.CharacterFormatting.Options));
			return merger.MergedProperties;
		}
		protected internal MergedParagraphProperties GetOverriddenParagraphProperties(ParagraphFormattingBase cssParagraphProperties) {
			MergedParagraphProperties newMergedParagraphProperties = new MergedParagraphProperties(cssParagraphProperties.Info, cssParagraphProperties.Options);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(newMergedParagraphProperties);
			merger.Merge(new MergedParagraphProperties(Importer.Position.ParagraphFormatting.Info, Importer.Position.ParagraphFormatting.Options));
			if (newMergedParagraphProperties.Options.UseSpacingBefore && !newMergedParagraphProperties.Options.UseBeforeAutoSpacing && merger.MergedProperties.Options.UseBeforeAutoSpacing)
					merger.MergedProperties.Info.BeforeAutoSpacing = false;
			if (newMergedParagraphProperties.Options.UseSpacingAfter && !newMergedParagraphProperties.Options.UseAfterAutoSpacing && merger.MergedProperties.Options.UseAfterAutoSpacing)
				merger.MergedProperties.Info.AfterAutoSpacing = false;
			return merger.MergedProperties;
		}
		protected internal MergedTableCellProperties GetOverriddenCellProperties(TableCellProperties tableCellProperties) {
			MergedTableCellProperties newMergedTableCellProperties = new MergedTableCellProperties(new CombinedCellPropertiesInfo(tableCellProperties), tableCellProperties.Info);
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(newMergedTableCellProperties);
			merger.Merge(Importer.Position.CellProperties);
			if (importer.Position.CellProperties.GeneralSettings.ColumnSpan > 0) {
				 merger.MergedProperties.Info.GeneralSettings.ColumnSpan = importer.Position.CellProperties.GeneralSettings.ColumnSpan;
			}
			return merger.MergedProperties;
		}
	}
	#endregion
	#region TagBase (abstract class)
	public abstract class TagBase {
		#region Field
		readonly HtmlImporter importer;
		readonly HtmlTagNameID name;
		string id;
		readonly StyleClasses classes;
		CssProperties styleAttributeProperties;
		readonly CharacterFormattingBase characterFormatting;
		#endregion
		#region Translation Table
		static TranslationTable<BorderLineStyle> borderLineStyleTable = CreateBorderLineStyleTable();
		static TranslationTable<BorderLineStyle> CreateBorderLineStyleTable() {
			TranslationTable<BorderLineStyle> result = new TranslationTable<BorderLineStyle>();
			result.Add(BorderLineStyle.Nil, "none");
			result.Add(BorderLineStyle.Dotted, "dotted");
			result.Add(BorderLineStyle.Dashed, "dashed");
			result.Add(BorderLineStyle.Double, "double");
			result.Add(BorderLineStyle.Inset, "inset");
			result.Add(BorderLineStyle.Outset, "outset");
			result.Add(BorderLineStyle.Single, "solid");
			return result;
		}
		public static TranslationTable<BorderLineStyle> BorderLineStyleTable { get { return borderLineStyleTable; } }
		#endregion
		static internal string ConvertKeyToUpper(string key) {
			return key.ToUpper(CultureInfo.InvariantCulture);
		}
		static AttributeKeywordTranslatorTable attributeTable = CreateAttributeTable();
		static internal AttributeKeywordTranslatorTable CreateAttributeTable() {
			AttributeKeywordTranslatorTable attributeTable = new AttributeKeywordTranslatorTable();
			attributeTable.Add(ConvertKeyToUpper("style"), StyleAttributeKeyword);
			attributeTable.Add(ConvertKeyToUpper("class"), ClassAttributeKeyword);
			attributeTable.Add(ConvertKeyToUpper("id"), IdAttributeKeyword);
			return attributeTable;
		}
		static void StyleAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			if (String.IsNullOrEmpty(value))
				return;
			CssParser cssParser = new CssParser(importer.DocumentModel);
			CssElementCollection cssElementCollection;
			using (StringReader reader = new StringReader(value)) {
				cssElementCollection = cssParser.ParseAttribute(reader);
			}
			if (cssElementCollection.Count > 0)
				tag.styleAttributeProperties = cssElementCollection[0].Properties;
		}
		static void ClassAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			StringBuilder val = new StringBuilder();
			for (int i = 0; i < value.Length; i++) {
				if (!Char.IsWhiteSpace(value[i]))
					val.Append(value[i]);
				else {
					AddClass(tag, val.ToString());
					val.Length = 0;
				}
			}
			AddClass(tag, val.ToString());
		}
		static void AddClass(TagBase tag, string val) {
			if (!String.IsNullOrEmpty(val))
				tag.Classes.Add(val);
		}
		static void IdAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			tag.id = value.ToUpper(CultureInfo.InvariantCulture);
		}
		static internal Color GetColorValue(string value) {
			StringBuilder colorName = new StringBuilder();
			if (value[0] == '#') {
				colorName.Append(value.Remove(0, 1));
				if (colorName.Length == 6)
					return GetColorByRgb(colorName.ToString());
				if (colorName.Length == 3) {
					colorName.Insert(0, colorName[0].ToString(), 1);
					colorName.Insert(2, colorName[2].ToString(), 1);
					colorName.Insert(4, colorName[4].ToString(), 1);
					return GetColorByRgb(colorName.ToString());
				}
				return GetColorByName(value);
			}
			if (value.Length == 6) {
				Color color = GetColorByRgb(value);
				if (color != DXColor.Empty)
					return color;
			}
			if (value.StartsWith("rgb(", StringComparison.CurrentCultureIgnoreCase)) {
				Color result = ParseRGB(value.Substring(4));
				if (result != DXColor.Empty)
					return result;
			}
			return GetColorByName(value);
		}
		static Color GetColorByName(string value) {
			Color color = DXColor.FromName(value);
#if !SL
			if (color.IsKnownColor)
				return color;
			return DXColor.Empty;
#else
			return color;
#endif
		}
		static int GetColor(string colorName, int startIndex) {
			int color;
			string sr = colorName.Substring(startIndex, 2);
			Int32.TryParse(sr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color);
			if (color == 0 && sr != "00")
				color = -1;
			return color;
		}
		static Color GetColorByRgb(string colorName) {
			int r = GetColor(colorName, 0);
			int g = GetColor(colorName, 2);
			int b = GetColor(colorName, 4);
			if (r != -1 && g != -1 && b != -1)
				return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
			return DXColor.Empty;
		}
		static Color ParseRGB(string value) {
			string rgb = String.Empty;
			int color;
			List<int> colors = new List<int>();
			for (int i = 0; i < value.Length; i++) {
				if (value[i] != ',' && value[i] != ')') {
					if (!Char.IsWhiteSpace(value[i]))
						rgb += value[i];
				}
				else {
					bool isDigit = Int32.TryParse(rgb, out color);
					if (isDigit)
						colors.Add(color);
					rgb = String.Empty;
				}
			}
			if (colors.Count == 3) {
				return Color.FromArgb(255, (byte)colors[0], (byte)colors[1], (byte)colors[2]);
			}
			return DXColor.Empty;
		}
		static internal List<string> ParseMediaAttribute(string value) {
			List<string> mediaDescriptors = new List<string>();
			StringBuilder descriptor = new StringBuilder();
			for (int i = 0; i < value.Length; i++) {
				if (value[i] != ',') {
					if (Char.IsLetter(value[i]))
						descriptor.Append(value[i]);
				}
				else {
					if (descriptor.Length > 0)
						mediaDescriptors.Add(descriptor.ToString());
					descriptor.Length = 0;
				}
			}
			if (descriptor.Length > 0)
				mediaDescriptors.Add(descriptor.ToString());
			return mediaDescriptors;
		}
		public static WidthUnitInfo ConvertPixelsValueToWidthUnitInfo(DocumentModelUnitConverter unitConverter, string value) {
			WidthUnitInfo result = new WidthUnitInfo();
			int valueInPixels;
			LengthValueParser parsedValue;
			if (Int32.TryParse(value, out valueInPixels))
				parsedValue = new LengthValueParser(value + "px", unitConverter.ScreenDpi);
			else
				parsedValue = new LengthValueParser(value, unitConverter.ScreenDpi);
			if (parsedValue.IsRelativeUnit) {
				result.Value = Math.Max(0, (int)Math.Round(parsedValue.PointsValue) * 50);
				result.Type = WidthUnitType.FiftiethsOfPercent;
			}
			else if (parsedValue.IsDigit) {
				DocumentModelUnitConverter converter = unitConverter;
				result.Value = Math.Max(0,(int)Math.Round(converter.PointsToModelUnitsF(parsedValue.PointsValue)));
				result.Type = WidthUnitType.ModelUnits;
			}
			return result;
		}
		static internal bool ReadParagraphAlignment(string value, ref ParagraphAlignment targetAlignment) {
			switch (value.ToLower(CultureInfo.InvariantCulture)) {
				case "right":
					targetAlignment = ParagraphAlignment.Right;
					return true;
				case "center":
					targetAlignment = ParagraphAlignment.Center;
					return true;
				case "justify":
					targetAlignment = ParagraphAlignment.Justify;
					return true;
				case "left":
					targetAlignment = ParagraphAlignment.Left;
					return true;
			}
			return false;
		}
		static internal VerticalAlignment ReadVerticalAlignment(string value) {
			VerticalAlignment result = VerticalAlignment.Top;
			switch (value.ToLower(CultureInfo.InvariantCulture)) {
				case "top":
					result = VerticalAlignment.Top;
					break;
				case "middle":
					result = VerticalAlignment.Center;
					break;
				case "bottom":
					result = VerticalAlignment.Bottom;
					break;
			}
			return result;
		}
		protected TagBase(HtmlImporter importer) {
			this.importer = importer;
			this.name = Tag.NameID;
			this.id = String.Empty;
			this.classes = new StyleClasses();
			this.characterFormatting = new CharacterFormattingBase(importer.PieceTable, importer.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.characterFormatting.BeginUpdate();
			this.characterFormatting.FontName = "Times New Roman";
			this.characterFormatting.DoubleFontSize = 24;
			this.characterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.characterFormatting.EndUpdate();
		}
		#region Properties
		protected internal HtmlImporter Importer { get { return importer; } }
		protected internal Tag Tag { get { return (Tag)Importer.Element; } }
		public HtmlTagNameID Name { get { return name; } }
		public string Id { get { return id; } }
		public StyleClasses Classes { get { return classes; } }
		protected internal virtual AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		protected internal virtual bool CanAppendToTagStack { get { return true; } }
		protected internal CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } }
		protected internal virtual bool ShouldBeIgnored { get { return false; } }
		protected internal virtual bool ApplyStylesToInnerHtml { get { return true; } }
		#endregion
		protected internal virtual ParagraphFormattingOptions ApplyCssProperties() {
			TagBaseHelper tagBaseHelper = new TagBaseHelper(Importer, this);
			return tagBaseHelper.ApplyCssProperties(styleAttributeProperties);
		}
		protected internal void OpenTagProcess() {
			RemoveDuplicateAttributes();
			FindKeywordInAttributeTable();
			CopyActualCharacterProperties();
			ApplyProperties();
			FunctionalTagProcess();
			CopyActualCharacterProperties();
			OpenTagProcessCore();
		}
		void CopyActualCharacterProperties() {
			this.characterFormatting.CopyFrom(Importer.Position.CharacterFormatting);
		}
		void RemoveDuplicateAttributes() {
			List<Attribute> attributes = Tag.Attributes;
			int count = attributes.Count;
			for (int i = 1; i < count; i++) {
				string attrName = attributes[i].Name;
				for (int j = 0; j < i; j++) {
					if (String.CompareOrdinal(attrName, attributes[j].Name) == 0) {
						attributes[i].Name = String.Empty;
						break;
					}
				}
			}
		}
		protected internal virtual void EmptyTagProcess() {
			Importer.OpenProcess(this);
		}
		protected internal virtual void FunctionalTagProcess() {
		}
		protected internal virtual void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			TagBase deletedTag = Importer.TagsStack[indexOfDeletedTag].Tag;
			this.characterFormatting.CopyFrom(deletedTag.characterFormatting);
		}
		protected internal virtual void FindKeywordInAttributeTable() {
			if (Tag.NameID == HtmlTagNameID.Unknown)
				return;			
			List<Attribute> attributes = Tag.Attributes;			
			for (int i = 0; i < attributes.Count; i++) {
				AttributeTranslateKeywordHandler translator = null;
				if (!String.IsNullOrEmpty(attributes[i].Value) || attributes[i].Name == "NOWRAP")
					AttributeTable.TryGetValue(attributes[i].Name, out translator);				
				if (translator != null)
					translator(importer, Importer.DecodeStringContent(attributes[i].Value), this);
			}
		}
		protected internal abstract void ApplyTagProperties();
		protected internal virtual void ApplyProperties() {
			HtmlInputPosition position = Importer.Position;
			position.CharacterFormatting.BeginUpdate();
			position.ParagraphFormatting.BeginUpdate();
			position.CellProperties.BeginUpdate();
			ApplyTagProperties();
			ApplyCssProperties();
			if (position.CharacterFormatting.Options.UseForeColor)
				position.CharacterFormatting.Options.UseUnderlineColor = false;
			position.CellProperties.EndUpdate();
			position.ParagraphFormatting.EndUpdate();
			position.CharacterFormatting.EndUpdate();
		}
		protected internal virtual void OpenTagProcessCore() {
		}
		protected internal virtual void DeleteOldOpenTag() {
		}
		protected internal virtual void ParagraphFunctionalProcess() {
			ProcessIsEmptyLine();
			ProcessIsEmptyParagraph();
			CopyProperties();
		}
		protected internal void ProcessIsEmptyLine() {
			bool paragraphsAllowed = Importer.DocumentModel.DocumentCapabilities.ParagraphsAllowed;
			if (Importer.IsEmptyLine && !Importer.IsEmptyParagraph && paragraphsAllowed) {
				Importer.Position.LogPosition--;
				bool oldValue = DocumentModel.ForceNotifyStructureChanged;
				DocumentModel.ForceNotifyStructureChanged = true;
				Importer.PieceTable.DeleteContent(Importer.Position.LogPosition, 1, false);
				Importer.IsEmptyLine = false;
				DocumentModel.ForceNotifyStructureChanged = oldValue;
			}
		}
		protected internal void ProcessIsEmptyParagraph() {
			PieceTable pieceTable = Importer.PieceTable;
			if (ShouldAddParagraph()) {
				if (!Importer.DocumentModel.DocumentCapabilities.ParagraphsAllowed)
					Importer.AppendText(" ");
				else {
					ApplyCharacterProperties(new RunIndex(pieceTable.Runs.Count - 2));
					Importer.Position.LogPosition++;
					Importer.Position.ParagraphIndex++;
					Importer.AppendParagraph();
					Importer.Position.LogPosition--;
					Importer.Position.ParagraphIndex--;
				}
				Importer.IsEmptyParagraph = true;
				Importer.IsEmptyListItem = false;
			}
		}
		void ApplyCharacterProperties(RunIndex runIndex) {
			TextRunBase run = Importer.PieceTable.Runs[runIndex];
			run.CharacterProperties.CopyFrom(this.characterFormatting);
		}
		protected virtual bool ShouldAddParagraph() {
			if (Importer.IsEmptyListItem)
				return false;
			else
				return !Importer.IsEmptyParagraph;
		}
		protected internal void CopyProperties() {
			PieceTable pieceTable = Importer.PieceTable;
			ParagraphIndex paragraphIndex = pieceTable.Paragraphs.Last.Index - 1;
			RunIndex runIndex = new RunIndex(pieceTable.Runs.Count - 2);
			ParagraphProperties paragraphProperties = new ParagraphProperties(Importer.DocumentModel);
			paragraphProperties.BeginInit();
			paragraphProperties.CopyFrom(Importer.Position.ParagraphFormatting);
			paragraphProperties.LeftIndent += Importer.Position.AdditionalIndent;
			paragraphProperties.RightIndent += Importer.Position.AdditionalIndent;
			paragraphProperties.EndInit();
			pieceTable.Paragraphs[paragraphIndex].ParagraphProperties.CopyFrom(paragraphProperties);
			pieceTable.Runs[runIndex].CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
			if (Importer.Position.ParagraphTabs.Count > 0)
				pieceTable.Paragraphs[paragraphIndex].Tabs.SetTabs(Importer.Position.ParagraphTabs);
		}
		protected internal virtual string GetAbsoluteUri(string baseUriString, string value) {
			try {
				Uri uri = CreateUri(baseUriString, value);
				if (!uri.IsAbsoluteUri)
					return value;
				if (uri.Scheme == "file") {
					string path = Path.GetFullPath(uri.LocalPath);
					uri = new Uri(path, UriKind.Absolute);
				}
				return uri.AbsoluteUri;
			}
			catch {
				return value;
			}
		}
		Uri CreateUri(string baseUriString, string value) {
			Uri baseUri;
			if (Uri.TryCreate(baseUriString, UriKind.Absolute, out baseUri))
				return new Uri(baseUri, value);
			return new Uri(value, UriKind.RelativeOrAbsolute);
		}
		public static WidthUnitInfo ImportBorderWidthCore(DocumentModelUnitConverter unitConverter, string value) {
			string namedValue = String.Empty;
			if (value == "thin")
				namedValue = "2px";
			else if (value == "medium")
				namedValue = "4px";
			else if (value == "thick")
				namedValue = "6px";
			if (String.IsNullOrEmpty(namedValue))
				namedValue = value;
			return ConvertPixelsValueToWidthUnitInfo(unitConverter, namedValue);
		}
		public static Color ImportColor(DocumentModelUnitConverter unitConverter, string value) {
			return DevExpress.Data.Utils.MarkupLanguageColorParser.ParseColor(value);
		}
		static internal void ImportBorderLineStyle(DocumentModelUnitConverter unitConverter, string value, HtmlBorderProperty border) {
			BorderLineStyle result = TableTag.BorderLineStyleTable.GetEnumValue(value, BorderLineStyle.ThreeDEmboss);
			if (result != BorderLineStyle.ThreeDEmboss)
				border.LineStyle = result;
		}
		static internal void ImportBordersLineStyles(DocumentModelUnitConverter unitConverter, string value, HtmlBordersProperties bordersProperties) {
			if (String.IsNullOrEmpty(value))
				return;
			string valueNormalizedSpaces = value.Replace("  ", " ");
			string[] lineStyles = valueNormalizedSpaces.Split(' ');
			if (lineStyles.Length == 0 || lineStyles.Length  > 4)
				return;
			BorderLineStyle top;
			BorderLineStyle bottom;
			BorderLineStyle left;
			BorderLineStyle right;
			if (lineStyles.Length == 4) {
				top = BorderLineStyleTable.GetEnumValue(lineStyles[0], BorderLineStyle.ThreeDEmboss);
				right = BorderLineStyleTable.GetEnumValue(lineStyles[1], BorderLineStyle.ThreeDEmboss);
				bottom = BorderLineStyleTable.GetEnumValue(lineStyles[2], BorderLineStyle.ThreeDEmboss);
				left = BorderLineStyleTable.GetEnumValue(lineStyles[3], BorderLineStyle.ThreeDEmboss);
			}
			else if (lineStyles.Length == 3) {
				top = BorderLineStyleTable.GetEnumValue(lineStyles[0], BorderLineStyle.ThreeDEmboss);
				left = BorderLineStyleTable.GetEnumValue(lineStyles[1], BorderLineStyle.ThreeDEmboss);
				right = left;
				bottom = BorderLineStyleTable.GetEnumValue(lineStyles[2], BorderLineStyle.ThreeDEmboss);
			}
			else if (lineStyles.Length == 2) {
				top = BorderLineStyleTable.GetEnumValue(lineStyles[0], BorderLineStyle.ThreeDEmboss);
				bottom = top;
				left = BorderLineStyleTable.GetEnumValue(lineStyles[1], BorderLineStyle.ThreeDEmboss);
				right = left;
			}
			else {
				top = BorderLineStyleTable.GetEnumValue(lineStyles[0], BorderLineStyle.ThreeDEmboss);
				if (top == BorderLineStyle.ThreeDEmboss)
					return;
				left = top;
				right = top;
				bottom = top;
			}
			if (top != BorderLineStyle.ThreeDEmboss)
				bordersProperties.TopBorder.LineStyle = top;
			if (left != BorderLineStyle.ThreeDEmboss)
				bordersProperties.LeftBorder.LineStyle = left;
			if (right != BorderLineStyle.ThreeDEmboss)
				bordersProperties.RightBorder.LineStyle = right;
			if (bottom != BorderLineStyle.ThreeDEmboss)
				bordersProperties.BottomBorder.LineStyle = bottom;
		}
		static internal void ImportBordersColors(DocumentModelUnitConverter unitConverter, string value, HtmlBordersProperties bordersProperties) {
			if (String.IsNullOrEmpty(value))
				return;
			string valueNormalizedSpaces = value.Replace("  ", " ");
			string[] differentColors = valueNormalizedSpaces.Split(' ');
			if (differentColors.Length == 0 || differentColors.Length > 4)
				return;
			Color topColor;
			Color bottomColor;
			Color leftColor;
			Color rightColor;
			if (differentColors.Length == 4) {
				topColor = ImportColor(unitConverter, differentColors[0]);
				rightColor = ImportColor(unitConverter, differentColors[1]);
				bottomColor = ImportColor(unitConverter, differentColors[2]);
				leftColor = ImportColor(unitConverter, differentColors[3]);
			}
			else if (differentColors.Length == 3) {
				topColor = ImportColor(unitConverter, differentColors[0]);
				leftColor = ImportColor(unitConverter, differentColors[1]);
				rightColor = leftColor;
				bottomColor = ImportColor(unitConverter, differentColors[2]);
			}
			else if (differentColors.Length == 2) {
				topColor = ImportColor(unitConverter, differentColors[0]);
				rightColor = ImportColor(unitConverter, differentColors[1]);
				bottomColor = topColor;
				leftColor = rightColor;
			}
			else {
				topColor = ImportColor(unitConverter, value);
				if (topColor == DXColor.Empty)
					return;
				rightColor = topColor;
				bottomColor = topColor;
				leftColor = topColor;
			}
			if (topColor != DXColor.Empty)
				bordersProperties.TopBorder.Color = topColor;
			if (leftColor != DXColor.Empty)
				bordersProperties.LeftBorder.Color = leftColor;
			if (rightColor != DXColor.Empty)
				bordersProperties.RightBorder.Color = rightColor;
			if (bottomColor != DXColor.Empty)
				bordersProperties.BottomBorder.Color = bottomColor;
		}
		static internal void ImportBordersWidths(DocumentModelUnitConverter unitConverter, string value, HtmlBordersProperties bordersProperties) {
			if (String.IsNullOrEmpty(value))
				return;
			string valueNormalizedSpaces = value.Replace("  ", " ");
			string[] differentBorders = valueNormalizedSpaces.Split(' ');
			if (differentBorders.Length == 0 || differentBorders.Length > 4)
				return;
			WidthUnitInfo topWidth;
			WidthUnitInfo rightWidth;
			WidthUnitInfo bottomWidth;
			WidthUnitInfo leftWidth;
			if (differentBorders.Length == 4) {
				topWidth = ImportBorderWidthCore(unitConverter, differentBorders[0]);
				rightWidth = ImportBorderWidthCore(unitConverter, differentBorders[1]);
				bottomWidth = ImportBorderWidthCore(unitConverter, differentBorders[2]);
				leftWidth = ImportBorderWidthCore(unitConverter, differentBorders[3]);
			}
			else if (differentBorders.Length == 3) {
				topWidth = ImportBorderWidthCore(unitConverter, differentBorders[0]);
				leftWidth = ImportBorderWidthCore(unitConverter, differentBorders[1]);
				rightWidth = leftWidth;
				bottomWidth = ImportBorderWidthCore(unitConverter, differentBorders[2]);
			}
			else if (differentBorders.Length == 2) {
				topWidth = ImportBorderWidthCore(unitConverter, differentBorders[0]);
				rightWidth = ImportBorderWidthCore(unitConverter, differentBorders[1]);
				bottomWidth = topWidth;
				leftWidth = rightWidth;
			}
			else {
				topWidth = ImportBorderWidthCore(unitConverter, differentBorders[0]);
				if (topWidth.Type == WidthUnitType.Nil)
					return;
				rightWidth = topWidth;
				bottomWidth = topWidth;
				leftWidth = topWidth;
			}
			if (topWidth.Type != WidthUnitType.Nil)
				bordersProperties.TopBorder.Width = topWidth.Value;
			if (leftWidth.Type != WidthUnitType.Nil)
				bordersProperties.LeftBorder.Width = leftWidth.Value;
			if (rightWidth.Type != WidthUnitType.Nil)
				bordersProperties.RightBorder.Width = rightWidth.Value;
			if (bottomWidth.Type != WidthUnitType.Nil)
				bordersProperties.BottomBorder.Width = bottomWidth.Value;
		}
		protected internal virtual int GetStartIndexAllowedSearchScope() {
			return 0;
		}
		protected internal void IgnoredMarginPropertiesFromBlockTags() {
			List<OpenHtmlTag> tagsStack = Importer.TagsStack;
			ParagraphFormattingBase paragraphFormatting = tagsStack[tagsStack.Count - 1].OldPosition.ParagraphFormatting;
			if (tagsStack.Count > 0) {
				Importer.Position.ParagraphFormatting.SpacingAfter = paragraphFormatting.SpacingAfter;
				Importer.Position.ParagraphFormatting.SpacingBefore = paragraphFormatting.SpacingBefore;
				Importer.Position.ParagraphFormatting.FirstLineIndent = paragraphFormatting.FirstLineIndent;
				Importer.Position.ParagraphFormatting.LeftIndent = paragraphFormatting.LeftIndent;
				Importer.Position.ParagraphFormatting.RightIndent = paragraphFormatting.RightIndent;
			}
		}
		public static bool ImportAlignment(string value, ref TableRowAlignment targetAlignment) {
			switch (value.ToLower(CultureInfo.InvariantCulture)) {
				case "right":
					targetAlignment = TableRowAlignment.Right;
					return true;
				case "center":
					targetAlignment = TableRowAlignment.Center;
					return true;
				case "justify":
					targetAlignment = TableRowAlignment.Distribute;
					return true;
				case "left":
					targetAlignment = TableRowAlignment.Left;
					return true;
			}
			return false;
		}
	}
#endregion
}
