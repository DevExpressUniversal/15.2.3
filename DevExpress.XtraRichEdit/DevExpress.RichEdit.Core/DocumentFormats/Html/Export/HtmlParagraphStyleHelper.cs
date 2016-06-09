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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.Export.Web;
using System.Drawing;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlParagraphStyleHelper
	public class HtmlParagraphStyleHelper {
		static readonly Dictionary<ParagraphAlignment, string> alignHT = CreateAlignTable();
		readonly DocumentModelUnitConverter unitConverter;
		readonly HtmlStyleHelper styleHelper;
		static Dictionary<ParagraphAlignment, string> CreateAlignTable() {
			Dictionary<ParagraphAlignment, string> table = new Dictionary<ParagraphAlignment, string>();
			table.Add(ParagraphAlignment.Left, "left");
			table.Add(ParagraphAlignment.Center, "center");
			table.Add(ParagraphAlignment.Right, "right");
			table.Add(ParagraphAlignment.Justify, "justify");
			return table;
		}
		public HtmlParagraphStyleHelper(DocumentModelUnitConverter unitConverter, HtmlStyleHelper styleHelper) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			Guard.ArgumentNotNull(styleHelper, "styleHelper");
			this.unitConverter = unitConverter;
			this.styleHelper = styleHelper;
		}
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		public HtmlStyleHelper HtmlStyleHelper { get { return styleHelper; } }
		public string GetHtmlLineHeight(ParagraphLineSpacing spacingType, float spacing, float fontSizeInPoints) {
			switch (spacingType) {
				default:
				case ParagraphLineSpacing.Single:
					return (1.0).ToString(CultureInfo.InvariantCulture);
				case ParagraphLineSpacing.Sesquialteral:
					return (1.5).ToString(CultureInfo.InvariantCulture);
				case ParagraphLineSpacing.Double:
					return (2.0).ToString(CultureInfo.InvariantCulture);
				case ParagraphLineSpacing.Multiple:
					return spacing.ToString(CultureInfo.InvariantCulture);
				case ParagraphLineSpacing.Exactly:
					return HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(spacing));
				case ParagraphLineSpacing.AtLeast: {
					float value = UnitConverter.ModelUnitsToPointsF(spacing);
					if (value > fontSizeInPoints)
						return HtmlStyleHelper.GetHtmlSizeInPoints(value);
					else
						return (1.0).ToString(CultureInfo.InvariantCulture);
				}
			}
		}
		public string GetHtmlAlignment(ParagraphAlignment align) {
			return alignHT[align];
		}
		public string GetHtmlListLevelType(IListLevel listLevel) {
			NumberingFormat format = listLevel.ListLevelProperties.Format;
			string displayFormat = listLevel.ListLevelProperties.DisplayFormatString;
			if (IsNotChangedDisplayFormat(displayFormat) && format != NumberingFormat.Bullet)
				return "disc";
			switch (format) {
				case NumberingFormat.Bullet:
					return GetBulletType(listLevel.CharacterProperties.FontName, displayFormat);
				case NumberingFormat.Decimal:
					return "decimal";
				case NumberingFormat.DecimalZero:
					return "decimal-leading-zero";
				case NumberingFormat.UpperLetter:
					return "upper-latin";
				case NumberingFormat.LowerLetter:
					return "lower-latin ";
				case NumberingFormat.UpperRoman:
					return "upper-roman";
				case NumberingFormat.LowerRoman:
					return "lower-roman";
				case NumberingFormat.Hebrew1:
					return "hebrew";
				case NumberingFormat.Hebrew2:
					return "hebrew";
				case NumberingFormat.AIUEOHiragana:
					return "hiragana";
				case NumberingFormat.AIUEOFullWidthHiragana:
					return "katakana";
				case NumberingFormat.Iroha:
					return "hiragana-iroha";
				case NumberingFormat.IrohaFullWidth:
					return "katakana-iroha";
			}
			return String.Empty;
		}
		string GetBulletType(string fontName, string displayFormat) {
			if (fontName == "Courier New")
				return "circle";
			else if (fontName == "Wingdings")
				return "square";
			else {
				if (displayFormat.Length <= 0)
					return "disc";
				char firstChar = displayFormat[0];
				if (firstChar == '\u006F')
					return "circle";
				else
					return "disc";
			}
		}
		bool IsNotChangedDisplayFormat(string displayFormat) {
			if (String.Format(displayFormat, 1, 1, 1, 1, 1, 1, 1, 1, 1) == displayFormat)
				return true;
			return false;
		}
		public string GetHtmlParagraphStyle(Paragraph paragraph, int listControlNestingLevel, int parentLevelOffset) {
			DXCssStyleCollection style = new DXCssStyleCollection();
			GetHtmlParagraphStyle(paragraph, listControlNestingLevel, parentLevelOffset, style);
			return HtmlStyleHelper.CreateCssStyle(style, ';');
		}
		public void GetHtmlParagraphStyle(Paragraph paragraph, int listControlNestingLevel, int parentLevelOffset, DXCssStyleCollection style) {
			GetHtmlParagraphStyleCore(paragraph, listControlNestingLevel, parentLevelOffset, style);
		}
		void GetHtmlParagraphStyleCore(Paragraph paragraph, int listControlNestingLevel, int parentLevelOffset, DXCssStyleCollection style) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			style.Add("text-align", GetHtmlAlignment(paragraph.Alignment));
			if (!paragraph.IsInList() || ShouldWriteNumberingListTextIndent(paragraph))
				style.Add("text-indent", GetTextIndentValue(paragraph.FirstLineIndentType, paragraph.FirstLineIndent));
			int leftIndent = CalculateParagraphLeftIndent(paragraph, listControlNestingLevel, parentLevelOffset);
			float spacingBefore = paragraph.ContextualSpacing ? 0 : UnitConverter.ModelUnitsToPointsFRound(paragraph.SpacingBefore);
			float spacingAfter = paragraph.ContextualSpacing ? 0 : UnitConverter.ModelUnitsToPointsFRound(paragraph.SpacingAfter);
			style.Add("margin",
			   HtmlStyleHelper.GetHtmlSizeInPoints(spacingBefore) + " " +
			   HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsFRound(paragraph.RightIndent)) + " " +
			   HtmlStyleHelper.GetHtmlSizeInPoints(spacingAfter) + " " +
			   HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsFRound(GetLeftPaddingValue(leftIndent, paragraph.FirstLineIndentType, paragraph.FirstLineIndent)))
			   );
			if (paragraph.LineSpacingType != ParagraphLineSpacing.Single)
				style.Add("line-height", GetLineSpacingValue(paragraph.LineSpacingType, paragraph.LineSpacing, paragraph.GetMergedCharacterProperties().Info.DoubleFontSize / 2f));
			if (paragraph.PageBreakBefore)
				style.Add("page-break-before", "always");
			if (paragraph.BeforeAutoSpacing)
				style.Add("mso-margin-top-alt", "auto");
			if (paragraph.AfterAutoSpacing)
				style.Add("mso-margin-bottom-alt", "auto");
			if (paragraph.KeepWithNext)
				style.Add("page-break-after", "avoid");
			if (paragraph.KeepLinesTogether)
				style.Add("page-break-inside", "avoid");
			if (!DXColor.IsTransparentOrEmpty(paragraph.BackColor))
				style.Add("background", HtmlConvert.ToHtml(paragraph.BackColor));
		}
		protected bool ShouldWriteNumberingListTextIndent(Paragraph paragraph) {
			ParagraphFirstLineIndent firstLineIndentType = paragraph.FirstLineIndentType;
			if (firstLineIndentType == ParagraphFirstLineIndent.Hanging || firstLineIndentType == ParagraphFirstLineIndent.None)
				return false;
			else
				return paragraph.FirstLineIndent > 0;
		}
		int CalculateParagraphLeftIndent(Paragraph paragraph, int listControlNestingLevel, int parentLevelOffset) {
			if (paragraph.IsInList()) {
				if (listControlNestingLevel == 1)
					return Math.Max(0, paragraph.LeftIndent - UnitConverter.DocumentsToModelUnits(150));
				else
					return paragraph.LeftIndent - parentLevelOffset;
			}
			return paragraph.LeftIndent;
		}
		public string GetHtmlParagraphInListStyle(Paragraph paragraph, IListLevel listLevel, int listControlNestingLevel, int parentLevelOffset) {
			DXCssStyleCollection style = new DXCssStyleCollection();
			GetHtmlParagraphInListStyle(paragraph, listLevel, listControlNestingLevel, parentLevelOffset, style);
			return HtmlStyleHelper.CreateCssStyle(style, ';');
		}
		internal void GetHtmlParagraphInListStyle(Paragraph paragraph, IListLevel listLevel, int listControlNestingLevel, int parentLevelOffset, DXCssStyleCollection style, bool defaultCharacterPropertiesExportToCss) {
			GetHtmlParagraphStyleCore(paragraph, listControlNestingLevel, parentLevelOffset, style);
			style.Add("list-style-type", GetHtmlListLevelType(listLevel));
			CharacterFormattingInfo characterProperties = paragraph.GetNumerationCharacterProperties().Info;
			Color foreColor = characterProperties.ForeColor;
			Color backColor = characterProperties.BackColor;
			if (foreColor == DXColor.Empty)
				foreColor = DXColor.Black;
			if (backColor == DXColor.Empty)
				backColor = DXColor.Transparent;
			string fontName = characterProperties.FontName;
			if (fontName == "Symbol") 
				fontName = "Arial";
			HtmlStyleRender.GetHtmlStyle(fontName, characterProperties.DoubleFontSize / 2f, GraphicsUnit.Point, characterProperties.FontBold, characterProperties.FontItalic, false, false, foreColor, backColor, style);
			if (!defaultCharacterPropertiesExportToCss)
				HtmlExportHelper.RemoveDefaultProperties(characterProperties, characterProperties.FontName, characterProperties.FontBold, characterProperties.FontItalic, characterProperties.DoubleFontSize, backColor, foreColor, style);
		}
		public void GetHtmlParagraphInListStyle(Paragraph paragraph, IListLevel listLevel, int listControlNestingLevel, int parentLevelOffset, DXCssStyleCollection style) {
			GetHtmlParagraphInListStyle(paragraph, listLevel, listControlNestingLevel, parentLevelOffset, style, true);
		}
		internal string GetLineSpacingValue(ParagraphLineSpacing spacingType, float spacing, float fontSizeInPoints) {
			string lineSpacing = GetHtmlLineHeight(spacingType, spacing, fontSizeInPoints);
			if (spacingType == ParagraphLineSpacing.Exactly)
				lineSpacing += ";mso-line-height-rule:exactly";
			return lineSpacing;
		}
		internal int GetLeftPaddingValue(int leftIndent, ParagraphFirstLineIndent firstLineIndentType, int firstLineIndent) {
			return leftIndent;
		}
		internal string GetTextIndentValue(ParagraphFirstLineIndent firstLineIndentType, int firstLineIndent) {
			switch (firstLineIndentType) {
				case ParagraphFirstLineIndent.Hanging:
					return HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsFRound(-firstLineIndent));
				case ParagraphFirstLineIndent.Indented:
					return HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsFRound(firstLineIndent));
				default:
					return "0pt";
			}
		}
	}
	#endregion
}
