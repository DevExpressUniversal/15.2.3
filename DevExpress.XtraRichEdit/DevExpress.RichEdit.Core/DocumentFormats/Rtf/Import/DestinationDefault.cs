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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing.Printing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DefaultDestination
	public class DefaultDestination : DestinationPieceTable {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("rtf", OnRtfKeyword);
			table.Add("deff", OnDeffKeyword);
			table.Add("info", OnInfoKeyword);
			table.Add("stylesheet", OnStyleSheetKeyword);
			table.Add("defchp", OnDefaultCharacterPropertiesKeyword);
			table.Add("defpap", OnDefaultParagraphPropertiesKeyword);
			AddCommonCharacterKeywords(table);
			AddCommonParagraphKeywords(table);
			AddCommonSymbolsAndObjectsKeywords(table);
			table.Add("page", OnPageBreakKeyword);
			table.Add("column", OnColumnBreakKeyword);
			AddCommonTabKeywords(table);
			table.Add("deftab", OnDefaultTabKeyword);
			AddDocumentProtectionKeywords(table);
			AddCommonNumberingListsKeywords(table);
			table.Add("listtable", OnListTableKeyword);
			table.Add("listoverridetable", OnListOverrideTableKeyword);
			table.Add("background", OnPageBackground);
			AppendTableKeywords(table);
			AddDocumentPropertiesKeywords(table);
			AddSectionKeywords(table);
			AddFootNoteAndEndNoteKeywords(table);
			AddCommentKeywords(table);
			return table;
		}
		internal static void AddDocumentPropertiesKeywords(KeywordTranslatorTable table) {
			table.Add("paperw", OnPaperWidthKeyword);
			table.Add("paperh", OnPaperHeightKeyword);
			table.Add("psz", OnPaperSizeKeyword);
			table.Add("landscape", OnLandscapeKeyword);
			table.Add("gutter", OnGutterKeyword);
			table.Add("margl", OnLeftMarginKeyword);
			table.Add("margr", OnRightMarginKeyword);
			table.Add("margt", OnTopMarginKeyword);
			table.Add("margb", OnBottomMarginKeyword);
			table.Add("rtlgutter", OnGutterAtRight);
			table.Add("pgnstart", OnPageNumberingStart);
			table.Add("facingp", OnPageFacing);
			#region Document Level Footnotes
			table.Add("endnotes", OnFootNotePlacementEndOfSection);
			table.Add("enddoc", OnFootNotePlacementEndOfDocument);
			table.Add("ftntj", OnFootNotePlacementBelowText);
			table.Add("ftnbj", OnFootNotePlacementPageBottom);
			table.Add("ftnstart", OnFootNoteNumberingStart);
			table.Add("ftnrstpg", OnFootNoteNumberingRestartEachPage);
			table.Add("ftnrestart", OnFootNoteNumberingRestartEachSection);
			table.Add("ftnrstcont", OnFootNoteNumberingRestartContinuous);
			table.Add("ftnnar", OnFootNoteNumberingDecimal);
			table.Add("ftnnalc", OnFootNoteNumberingLowerCaseLetter);
			table.Add("ftnnauc", OnFootNoteNumberingUpperCaseLetter);
			table.Add("ftnnrlc", OnFootNoteNumberingLowerCaseRoman);
			table.Add("ftnnruc", OnFootNoteNumberingUpperCaseRoman);
			table.Add("ftnnchi", OnFootNoteNumberingChicago);
			table.Add("ftnnchosung", OnFootNoteNumberingChosung);
			table.Add("ftnncnum", OnFootNoteNumberingDecimalEnclosedCircle);
			table.Add("ftnndbar", OnFootNoteNumberingDecimalFullWidth);
			table.Add("ftnnganada", OnFootNoteNumberingGanada);
			#endregion
			#region Document Level Endnotes
			table.Add("aendnotes", OnEndNotePlacementEndOfSection);
			table.Add("aenddoc", OnEndNotePlacementEndOfDocument);
			table.Add("aftntj", OnEndNotePlacementBelowText);
			table.Add("aftnbj", OnEndNotePlacementPageBottom);
			table.Add("aftnstart", OnEndNoteNumberingStart);
			table.Add("aftnrestart", OnEndNoteNumberingRestartEachSection);
			table.Add("aftnrstcont", OnEndNoteNumberingRestartContinuous);
			table.Add("aftnnar", OnEndNoteNumberingDecimal);
			table.Add("aftnnalc", OnEndNoteNumberingLowerCaseLetter);
			table.Add("aftnnauc", OnEndNoteNumberingUpperCaseLetter);
			table.Add("aftnnrlc", OnEndNoteNumberingLowerCaseRoman);
			table.Add("aftnnruc", OnEndNoteNumberingUpperCaseRoman);
			table.Add("aftnnchi", OnEndNoteNumberingChicago);
			table.Add("aftnnchosung", OnEndNoteNumberingChosung);
			table.Add("aftnncnum", OnEndNoteNumberingDecimalEnclosedCircle);
			table.Add("aftnndbar", OnEndNoteNumberingDecimalFullWidth);
			table.Add("aftnnganada", OnEndNoteNumberingGanada);
			#endregion
			#region Document Level Views and Zoom
			table.Add("viewbksp", OnDisplayBackgroundShape);
			#endregion
		}
		internal static void AddDocumentProtectionKeywords(KeywordTranslatorTable table) {
			table.Add("enforceprot", OnEnforceDocumentProtection);
			table.Add("annotprot", OnAnnotationProtection);
			table.Add("readprot", OnReadOnlyProtection);
			table.Add("protlevel", OnDocumentProtectionLevel);
		}
		internal static void AddSectionKeywords(KeywordTranslatorTable table) {
			#region Section Page Numbering
			table.Add("pgndec", OnPageNumberingDecimal);
			table.Add("pgnucrm", OnPageNumberingUpperRoman);
			table.Add("pgnlcrm", OnPageNumberingLowerRoman);
			table.Add("pgnucltr", OnPageNumberingUpperLetter);
			table.Add("pgnlcltr", OnPageNumberingLowerLetter);
			table.Add("pgnbidia", OnPageNumberingArabicAbjad);
			table.Add("pgnbidib", OnPageNumberingArabicAlpha);
			table.Add("pgnchosung", OnPageNumberingChosung);
			table.Add("pgncnum", OnPageNumberingDecimalEnclosedCircle);
			table.Add("pgndecd", OnPageNumberingDecimalFullWidth);
			table.Add("pgnganada", OnPageNumberingGanada);
			table.Add("pgnhindia", OnPageNumberingHindiVowels);
			table.Add("pgnhindib", OnPageNumberingHindiConsonants);
			table.Add("pgnhindic", OnPageNumberingHindiNumbers);
			table.Add("pgnhindid", OnPageNumberingHindiDescriptive);
			table.Add("pgnthaia", OnPageNumberingThaiLetters);
			table.Add("pgnthaib", OnPageNumberingThaiNumbers);
			table.Add("pgnthaic", OnPageNumberingThaiDescriptive);
			table.Add("pgnvieta", OnPageNumberingVietnameseDescriptive);
			#endregion
			table.Add("pgnhn", OnPageNumberingChapterHeaderStyle);
			table.Add("pgnhnsh", OnPageNumberingChapterSeparatorHyphen);
			table.Add("pgnhnsp", OnPageNumberingChapterSeparatorPeriod);
			table.Add("pgnhnsc", OnPageNumberingChapterSeparatorColon);
			table.Add("pgnhnsm", OnPageNumberingChapterSeparatorEmDash);
			table.Add("pgnhnsn", OnPageNumberingChapterSeparatorEnDash);
			table.Add("vertal", OnVerticalTextAlignmentBottom);
			table.Add("vertalb", OnVerticalTextAlignmentBottom);
			table.Add("vertalt", OnVerticalTextAlignmentTop);
			table.Add("vertalc", OnVerticalTextAlignmentCenter);
			table.Add("vertalj", OnVerticalTextAlignmentJustify);
			table.Add("pgnstarts", OnSectionPageNumberingStart);
			table.Add("pgncont", OnSectionPageNumberingContinuous);
			table.Add("pgnrestart", OnSectionPageNumberingRestart);
			table.Add("stextflow", OnSectionTextFlow);
			table.Add("linemod", OnLineNumberingStep);
			table.Add("linex", OnLineNumberingDistance);
			table.Add("linestarts", OnLineNumberingStartingLineNumber);
			table.Add("linerestart", OnLineNumberingRestart);
			table.Add("lineppage", OnLineNumberingRestartOnEachPage);
			table.Add("linecont", OnLineNumberingContinuous);
			table.Add("pgwsxn", OnSectionPaperWidth);
			table.Add("pghsxn", OnSectionPaperHeight);
			table.Add("lndscpsxn", OnSectionLandscape);
			table.Add("marglsxn", OnSectionLeftMargin);
			table.Add("margrsxn", OnSectionRightMargin);
			table.Add("margtsxn", OnSectionTopMargin);
			table.Add("margbsxn", OnSectionBottomMargin);
			table.Add("guttersxn", OnSectionGutter);
			table.Add("headery", OnSectionHeaderOffset);
			table.Add("footery", OnSectionFooterOffset);
			table.Add("binfsxn", OnSectionFirstPagePaperSource);
			table.Add("binsxn", OnSectionOtherPagePaperSource);
			table.Add("sectunlocked", OnOnlyAllowEditingOfFormFields);
			table.Add("cols", OnSectionColumnCount);
			table.Add("colsx", OnSectionColumnSpace);
			table.Add("colno", OnSectionCurrentColumnIndex);
			table.Add("colsr", OnSectionCurrentColumnSpace);
			table.Add("colw", OnSectionCurrentColumnWidth);
			table.Add("linebetcol", OnSectionDrawVerticalSeparator);
			table.Add("sbknone", OnSectionBreakNone);
			table.Add("sbkcol", OnSectionBreakColumn);
			table.Add("sbkpage", OnSectionBreakPage);
			table.Add("sbkeven", OnSectionBreakEvenPage);
			table.Add("sbkodd", OnSectionBreakOddPage);
			table.Add("sectd", OnSectionDefault);
			table.Add("sect", OnNewSection);
			table.Add("titlepg", OnSectionDifferentFirstPage);
			table.Add("header", OnHeaderKeyword);
			table.Add("headerl", OnHeaderForLeftPagesKeyword);
			table.Add("headerr", OnHeaderForRightPagesKeyword);
			table.Add("headerf", OnHeaderForFirstPageKeyword);
			table.Add("footer", OnFooterKeyword);
			table.Add("footerl", OnFooterForLeftPagesKeyword);
			table.Add("footerr", OnFooterForRightPagesKeyword);
			table.Add("footerf", OnFooterForFirstPageKeyword);
			#region Section Level Footnotes
			table.Add("sftntj", OnSectionFootNotePlacementBelowText);
			table.Add("sftnbj", OnSectionFootNotePlacementPageBottom);
			table.Add("sftnstart", OnSectionFootNoteNumberingStart);
			table.Add("sftnrstpg", OnSectionFootNoteNumberingRestartEachPage);
			table.Add("sftnrestart", OnSectionFootNoteNumberingRestartEachSection);
			table.Add("sftnrstcont", OnSectionFootNoteNumberingRestartContinuous);
			table.Add("sftnnar", OnSectionFootNoteNumberingDecimal);
			table.Add("sftnnalc", OnSectionFootNoteNumberingLowerCaseLetter);
			table.Add("sftnnauc", OnSectionFootNoteNumberingUpperCaseLetter);
			table.Add("sftnnrlc", OnSectionFootNoteNumberingLowerCaseRoman);
			table.Add("sftnnruc", OnSectionFootNoteNumberingUpperCaseRoman);
			table.Add("sftnnchi", OnSectionFootNoteNumberingChicago);
			table.Add("sftnnchosung", OnSectionFootNoteNumberingChosung);
			table.Add("sftnncnum", OnSectionFootNoteNumberingDecimalEnclosedCircle);
			table.Add("sftnndbar", OnSectionFootNoteNumberingDecimalFullWidth);
			table.Add("sftnnganada", OnSectionFootNoteNumberingGanada);
			#endregion
			#region Section Level Endnotes
			table.Add("saftnstart", OnSectionEndNoteNumberingStart);
			table.Add("saftnrestart", OnSectionEndNoteNumberingRestartEachSection);
			table.Add("saftnrstcont", OnSectionEndNoteNumberingRestartContinuous);
			table.Add("saftnnar", OnSectionEndNoteNumberingDecimal);
			table.Add("saftnnalc", OnSectionEndNoteNumberingLowerCaseLetter);
			table.Add("saftnnauc", OnSectionEndNoteNumberingUpperCaseLetter);
			table.Add("saftnnrlc", OnSectionEndNoteNumberingLowerCaseRoman);
			table.Add("saftnnruc", OnSectionEndNoteNumberingUpperCaseRoman);
			table.Add("saftnnchi", OnSectionEndNoteNumberingChicago);
			table.Add("saftnnchosung", OnSectionEndNoteNumberingChosung);
			table.Add("saftnncnum", OnSectionEndNoteNumberingDecimalEnclosedCircle);
			table.Add("saftnndbar", OnSectionEndNoteNumberingDecimalFullWidth);
			table.Add("saftnnganada", OnSectionEndNoteNumberingGanada);
			#endregion
		}
		#endregion
		internal static void AddFootNoteAndEndNoteKeywords(KeywordTranslatorTable table) {
			table.Add("footnote", OnFootNoteKeyword);
		}
		internal static void AddCommentKeywords(KeywordTranslatorTable table) {
			table.Add("atrfstart", OnCommentStartPositionKeyword);
			table.Add("atrfend", OnCommentEndPositionKeyword);
			table.Add("atnid", OnCommentIdKeyword);
			table.Add("atnauthor", OnCommentAuthorKeyword);
			table.Add("annotation", OnCommentAnnotationKeyword);
		} 
		#region processing control char and keyword
		static void OnRtfKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnDeffKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultFontNumber = parameterValue;
		}
		static void OnInfoKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new InfoDestination(importer);
		}
		static void OnStyleSheetKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new StyleSheetDestination(importer);
		}
		static void OnDefaultCharacterPropertiesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DefaultCharacterPropertiesDestination(importer);
		}
		static void OnDefaultParagraphPropertiesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DefaultParagraphPropertiesDestination(importer);
		}
		static void OnPageBackground(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new PageBackgroundDestination(importer);
		}
		static void OnDefaultTabKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue <= 0)
				parameterValue = 720;
			importer.DocumentModel.DocumentProperties.DefaultTabWidth = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnEnforceDocumentProtection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentModel.ProtectionProperties.EnforceProtection = true;
		}
		static void OnAnnotationProtection(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnReadOnlyProtection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentModel.ProtectionProperties.ProtectionType = DocumentProtectionType.ReadOnly;
		}
		static void OnDocumentProtectionLevel(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && parameterValue == 3)
				importer.DocumentModel.ProtectionProperties.ProtectionType = DocumentProtectionType.ReadOnly;
		}
		static void OnPageBreakKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.PageBreak);
		}
		static void OnColumnBreakKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.ColumnBreak);
		}
		static void OnPaperWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 12240;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Page.Width = value;
			importer.Position.SectionFormattingInfo.Page.Width = value;
		}
		static void OnPaperHeightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 15840;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Page.Height = value;
			importer.Position.SectionFormattingInfo.Page.Height = value;
		}
		static void OnPaperSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter) {
				PaperKind value = (PaperKind)parameterValue;
				importer.DocumentProperties.DefaultSectionProperties.Page.PaperKind = value;
				importer.Position.SectionFormattingInfo.Page.PaperKind = value;
			}
		}
		static void OnLandscapeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.Page.Landscape = true;
			importer.Position.SectionFormattingInfo.Page.Landscape = true;
		}
		static void OnGutterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Margins.Gutter = value;
			importer.Position.SectionFormattingInfo.Margins.Gutter = value;
		}
		static void OnLeftMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1800;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Margins.Left = value;
			importer.Position.SectionFormattingInfo.Margins.Left = value;
		}
		static void OnRightMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1800;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Margins.Right = value;
			importer.Position.SectionFormattingInfo.Margins.Right = value;
		}
		static void OnTopMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1440;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Margins.Top = value;
			importer.Position.SectionFormattingInfo.Margins.Top = value;
		}
		static void OnBottomMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1440;
			int value = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.DocumentProperties.DefaultSectionProperties.Margins.Bottom = value;
			importer.Position.SectionFormattingInfo.Margins.Bottom = value;
		}
		static void OnGutterAtRight(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.Margins.GutterAlignment = SectionGutterAlignment.Right;
			importer.Position.SectionFormattingInfo.Margins.GutterAlignment = SectionGutterAlignment.Right;
		}
		static void OnPageNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			importer.DocumentProperties.DefaultSectionProperties.PageNumbering.StartingPageNumber = parameterValue;
			importer.Position.SectionFormattingInfo.PageNumbering.StartingPageNumber = parameterValue;
		}
		static void OnPageFacing(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentModel.DocumentProperties.DifferentOddAndEvenPages = true;
		}
		static void OnDisplayBackgroundShape(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentModel.DocumentProperties.DisplayBackgroundShape = parameterValue == 1 ? true : false;
		}
		static void OnPageNumberingDecimal(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.Decimal;
		}
		static void OnPageNumberingUpperRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.UpperRoman;
		}
		static void OnPageNumberingLowerRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.LowerRoman;
		}
		static void OnPageNumberingUpperLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.UpperLetter;
		}
		static void OnPageNumberingLowerLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.LowerLetter;
		}
		static void OnPageNumberingArabicAbjad(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.ArabicAbjad;
		}
		static void OnPageNumberingArabicAlpha(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.ArabicAlpha;
		}
		static void OnPageNumberingChosung(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.Chosung;
		}
		static void OnPageNumberingDecimalEnclosedCircle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
		}
		static void OnPageNumberingDecimalFullWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.DecimalFullWidth;
		}
		static void OnPageNumberingGanada(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.Ganada;
		}
		static void OnPageNumberingHindiVowels(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.HindiVowels;
		}
		static void OnPageNumberingHindiConsonants(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.HindiConsonants;
		}
		static void OnPageNumberingHindiNumbers(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.HindiNumbers;
		}
		static void OnPageNumberingHindiDescriptive(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.HindiDescriptive;
		}
		static void OnPageNumberingThaiLetters(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.ThaiLetters;
		}
		static void OnPageNumberingThaiNumbers(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.ThaiNumbers;
		}
		static void OnPageNumberingThaiDescriptive(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.ThaiDescriptive;
		}
		static void OnPageNumberingVietnameseDescriptive(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.NumberingFormat = NumberingFormat.VietnameseDescriptive;
		}
		static void OnPageNumberingChapterHeaderStyle(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterHeaderStyle = parameterValue;
		}
		static void OnPageNumberingChapterSeparatorHyphen(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterSeparator = Characters.Hyphen;
		}
		static void OnPageNumberingChapterSeparatorPeriod(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterSeparator = '.';
		}
		static void OnPageNumberingChapterSeparatorColon(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterSeparator = ':';
		}
		static void OnPageNumberingChapterSeparatorEmDash(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterSeparator = Characters.EmDash;
		}
		static void OnPageNumberingChapterSeparatorEnDash(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.PageNumbering.ChapterSeparator = Characters.EnDash;
		}
		static void OnVerticalTextAlignmentBottom(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.VerticalTextAlignment = VerticalAlignment.Bottom;
		}
		static void OnVerticalTextAlignmentTop(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.VerticalTextAlignment = VerticalAlignment.Top;
		}
		static void OnVerticalTextAlignmentCenter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.VerticalTextAlignment = VerticalAlignment.Center;
		}
		static void OnVerticalTextAlignmentJustify(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.VerticalTextAlignment = VerticalAlignment.Both;
		}
		static void OnSectionPageNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.SectionFormattingInfo.PageNumbering.StartingPageNumber = parameterValue;
		}
		static void OnSectionPageNumberingContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.RestartPageNumbering = false;
		}
		static void OnSectionPageNumberingRestart(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.RestartPageNumbering = true;
		}
		static void OnSectionTextFlow(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			TextDirection flow = (TextDirection)parameterValue;
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.TextDirection = flow;
		}
		static void OnLineNumberingStep(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			importer.Position.SectionFormattingInfo.LineNumbering.Step = parameterValue;
		}
		static void OnLineNumberingDistance(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 360;
			importer.Position.SectionFormattingInfo.LineNumbering.Distance = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnLineNumberingStartingLineNumber(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.SectionFormattingInfo.LineNumbering.StartingLineNumber = parameterValue;
		}
		static void OnLineNumberingRestart(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.LineNumbering.NumberingRestartType = LineNumberingRestart.NewSection;
		}
		static void OnLineNumberingRestartOnEachPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.LineNumbering.NumberingRestartType = LineNumberingRestart.NewPage;
		}
		static void OnLineNumberingContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.LineNumbering.NumberingRestartType = LineNumberingRestart.Continuous;
		}
		static void OnSectionPaperWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Page.Width = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionPaperHeight(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Page.Height = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionLandscape(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Page.Landscape = true;
		}
		static void OnSectionLeftMargin(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Margins.Left = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionRightMargin(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Margins.Right = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionTopMargin(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Margins.Top = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionBottomMargin(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Margins.Bottom = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionGutter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Margins.Gutter = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionHeaderOffset(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 720;
			importer.Position.SectionFormattingInfo.Margins.HeaderOffset = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionFooterOffset(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 720;
			importer.Position.SectionFormattingInfo.Margins.FooterOffset = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnSectionFirstPagePaperSource(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.FirstPagePaperSource = parameterValue;
		}
		static void OnSectionOtherPagePaperSource(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.OtherPagePaperSource = parameterValue;
		}
		static void OnOnlyAllowEditingOfFormFields(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.OnlyAllowEditingOfFormFields = true;
		}
		static void OnSectionColumnCount(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.SectionFormattingInfo.Columns.ColumnCount = parameterValue;
			importer.Position.SectionFormattingInfo.Columns.EqualWidthColumns = true;
		}
		static void OnSectionColumnSpace(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 720;
			importer.Position.SectionFormattingInfo.Columns.Space = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			importer.Position.SectionFormattingInfo.Columns.EqualWidthColumns = true;
		}
		static void OnSectionCurrentColumnIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.CurrentColumnIndex = parameterValue - 1;
			importer.Position.SectionFormattingInfo.Columns.EqualWidthColumns = false;
		}
		static void OnSectionCurrentColumnSpace(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.SetCurrentColumnSpace(importer.UnitConverter.TwipsToModelUnits(parameterValue));
			importer.Position.SectionFormattingInfo.Columns.EqualWidthColumns = false;
		}
		static void OnSectionCurrentColumnWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.SetCurrentColumnWidth(importer.UnitConverter.TwipsToModelUnits(parameterValue));
			importer.Position.SectionFormattingInfo.Columns.EqualWidthColumns = false;
		}
		static void OnSectionDrawVerticalSeparator(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.Columns.DrawVerticalSeparator = true;
		}
		static void OnSectionBreakNone(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.StartType = SectionStartType.Continuous;
		}
		static void OnSectionBreakColumn(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.StartType = SectionStartType.Column;
		}
		static void OnSectionBreakPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.StartType = SectionStartType.NextPage;
		}
		static void OnSectionBreakEvenPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.StartType = SectionStartType.EvenPage;
		}
		static void OnSectionBreakOddPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.StartType = SectionStartType.OddPage;
		}
		static void OnSectionDefault(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.CopyFrom(importer.DocumentProperties.DefaultSectionProperties);
		}
		static void OnNewSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ApplySectionFormatting();
			importer.InsertSection();
			importer.TableReader.ResetState();
		}
		static void OnHeaderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Headers.Create(HeaderFooterType.Odd);
			importer.Destination = new SectionPageHeaderDestination(importer, section, section.InnerOddPageHeader);
		}
		static void OnHeaderForLeftPagesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Headers.Create(HeaderFooterType.Even);
			importer.Destination = new SectionPageHeaderDestination(importer, section, section.InnerEvenPageHeader);
		}
		static void OnHeaderForRightPagesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Headers.Create(HeaderFooterType.Odd);
			importer.Destination = new SectionPageHeaderDestination(importer, section, section.InnerOddPageHeader);
		}
		static void OnHeaderForFirstPageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Headers.Create(HeaderFooterType.First);
			importer.Destination = new SectionPageHeaderDestination(importer, section, section.InnerFirstPageHeader);
		}
		static void OnFooterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Footers.Create(HeaderFooterType.Odd);
			importer.Destination = new SectionPageFooterDestination(importer, section, section.InnerOddPageFooter);
		}
		static void OnFooterForLeftPagesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Footers.Create(HeaderFooterType.Even);
			importer.Destination = new SectionPageFooterDestination(importer, section, section.InnerEvenPageFooter);
		}
		static void OnFooterForRightPagesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Footers.Create(HeaderFooterType.Odd);
			importer.Destination = new SectionPageFooterDestination(importer, section, section.InnerOddPageFooter);
		}
		static void OnFooterForFirstPageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			Section section = importer.DocumentModel.Sections.Last;
			section.Footers.Create(HeaderFooterType.First);
			importer.Destination = new SectionPageFooterDestination(importer, section, section.InnerFirstPageFooter);
		}
		static void OnSectionDifferentFirstPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.GeneralSectionInfo.DifferentFirstPage = true;
		}
		static void OnListTableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListTableDestination(importer);
		}
		static void OnListOverrideTableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListOverrideTableDestination(importer);
		}
		#region Section Level Footnotes
		static void OnSectionFootNotePlacementBelowText(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.BelowText;
		}
		static void OnSectionFootNotePlacementPageBottom(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.BottomOfPage;
		}
		static void OnSectionFootNoteNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue <= 0)
				parameterValue = 1;
			importer.Position.SectionFormattingInfo.FootNote.StartingNumber = parameterValue;
		}
		static void OnSectionFootNoteNumberingRestartEachPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.NewPage;
		}
		static void OnSectionFootNoteNumberingRestartEachSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.NewSection;
		}
		static void OnSectionFootNoteNumberingRestartContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.Continuous;
		}
		static void OnSectionFootNoteNumberingDecimal(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Decimal;
		}
		static void OnSectionFootNoteNumberingLowerCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.LowerLetter;
		}
		static void OnSectionFootNoteNumberingUpperCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.UpperLetter;
		}
		static void OnSectionFootNoteNumberingLowerCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.LowerRoman;
		}
		static void OnSectionFootNoteNumberingUpperCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.UpperRoman;
		}
		static void OnSectionFootNoteNumberingChicago(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Chicago;
		}
		static void OnSectionFootNoteNumberingChosung(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Chosung;
		}
		static void OnSectionFootNoteNumberingDecimalEnclosedCircle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
		}
		static void OnSectionFootNoteNumberingDecimalFullWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
		}
		static void OnSectionFootNoteNumberingGanada(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Ganada;
		}
		#endregion
		#region Section Level Endnotes
		static void OnSectionEndNoteNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue <= 0)
				parameterValue = 1;
			importer.Position.SectionFormattingInfo.EndNote.StartingNumber = parameterValue;
		}
		static void OnSectionEndNoteNumberingRestartEachSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingRestartType = LineNumberingRestart.NewSection;
		}
		static void OnSectionEndNoteNumberingRestartContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingRestartType = LineNumberingRestart.Continuous;
		}
		static void OnSectionEndNoteNumberingDecimal(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Decimal;
		}
		static void OnSectionEndNoteNumberingLowerCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.LowerLetter;
		}
		static void OnSectionEndNoteNumberingUpperCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.UpperLetter;
		}
		static void OnSectionEndNoteNumberingLowerCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.LowerRoman;
		}
		static void OnSectionEndNoteNumberingUpperCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.UpperRoman;
		}
		static void OnSectionEndNoteNumberingChicago(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Chicago;
		}
		static void OnSectionEndNoteNumberingChosung(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Chosung;
		}
		static void OnSectionEndNoteNumberingDecimalEnclosedCircle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
		}
		static void OnSectionEndNoteNumberingDecimalFullWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
		}
		static void OnSectionEndNoteNumberingGanada(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Ganada;
		}
		#endregion
		#region Document Level Footnotes
		static void OnFootNotePlacementEndOfSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.Position = FootNotePosition.EndOfSection;
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.EndOfSection;
		}
		static void OnFootNotePlacementEndOfDocument(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.Position = FootNotePosition.EndOfDocument;
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.EndOfDocument;
		}
		static void OnFootNotePlacementBelowText(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.Position = FootNotePosition.BelowText;
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.BelowText;
		}
		static void OnFootNotePlacementPageBottom(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.Position = FootNotePosition.BottomOfPage;
			importer.Position.SectionFormattingInfo.FootNote.Position = FootNotePosition.BottomOfPage;
		}
		static void OnFootNoteNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue <= 0)
				parameterValue = 1;
			importer.DocumentProperties.DefaultSectionProperties.FootNote.StartingNumber = parameterValue;
			importer.Position.SectionFormattingInfo.FootNote.StartingNumber = parameterValue;
		}
		static void OnFootNoteNumberingRestartEachPage(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingRestartType = LineNumberingRestart.NewPage;
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.NewPage;
		}
		static void OnFootNoteNumberingRestartEachSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingRestartType = LineNumberingRestart.NewSection;
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.NewSection;
		}
		static void OnFootNoteNumberingRestartContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingRestartType = LineNumberingRestart.Continuous;
			importer.Position.SectionFormattingInfo.FootNote.NumberingRestartType = LineNumberingRestart.Continuous;
		}
		static void OnFootNoteNumberingDecimal(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.Decimal;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Decimal;
		}
		static void OnFootNoteNumberingLowerCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.LowerLetter;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.LowerLetter;
		}
		static void OnFootNoteNumberingUpperCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.UpperLetter;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.UpperLetter;
		}
		static void OnFootNoteNumberingLowerCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.LowerRoman;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.LowerRoman;
		}
		static void OnFootNoteNumberingUpperCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.UpperRoman;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.UpperRoman;
		}
		static void OnFootNoteNumberingChicago(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.Chicago;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Chicago;
		}
		static void OnFootNoteNumberingChosung(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.Chosung;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Chosung;
		}
		static void OnFootNoteNumberingDecimalEnclosedCircle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
		}
		static void OnFootNoteNumberingDecimalFullWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
		}
		static void OnFootNoteNumberingGanada(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.FootNote.NumberingFormat = NumberingFormat.Ganada;
			importer.Position.SectionFormattingInfo.FootNote.NumberingFormat = NumberingFormat.Ganada;
		}
		#endregion
		#region Document Level Endnotes
		static void OnEndNotePlacementEndOfSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.Position = FootNotePosition.EndOfSection;
			importer.Position.SectionFormattingInfo.EndNote.Position = FootNotePosition.EndOfSection;
		}
		static void OnEndNotePlacementEndOfDocument(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.Position = FootNotePosition.EndOfDocument;
			importer.Position.SectionFormattingInfo.EndNote.Position = FootNotePosition.EndOfDocument;
		}
		static void OnEndNotePlacementBelowText(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.Position = FootNotePosition.BelowText;
			importer.Position.SectionFormattingInfo.EndNote.Position = FootNotePosition.BelowText;
		}
		static void OnEndNotePlacementPageBottom(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.Position = FootNotePosition.BottomOfPage;
			importer.Position.SectionFormattingInfo.EndNote.Position = FootNotePosition.BottomOfPage;
		}
		static void OnEndNoteNumberingStart(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue <= 0)
				parameterValue = 1;
			importer.DocumentProperties.DefaultSectionProperties.EndNote.StartingNumber = parameterValue;
			importer.Position.SectionFormattingInfo.EndNote.StartingNumber = parameterValue;
		}
		static void OnEndNoteNumberingRestartEachSection(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingRestartType = LineNumberingRestart.NewSection;
			importer.Position.SectionFormattingInfo.EndNote.NumberingRestartType = LineNumberingRestart.NewSection;
		}
		static void OnEndNoteNumberingRestartContinuous(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingRestartType = LineNumberingRestart.Continuous;
			importer.Position.SectionFormattingInfo.EndNote.NumberingRestartType = LineNumberingRestart.Continuous;
		}
		static void OnEndNoteNumberingDecimal(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.Decimal;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Decimal;
		}
		static void OnEndNoteNumberingLowerCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.LowerLetter;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.LowerLetter;
		}
		static void OnEndNoteNumberingUpperCaseLetter(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.UpperLetter;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.UpperLetter;
		}
		static void OnEndNoteNumberingLowerCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.LowerRoman;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.LowerRoman;
		}
		static void OnEndNoteNumberingUpperCaseRoman(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.UpperRoman;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.UpperRoman;
		}
		static void OnEndNoteNumberingChicago(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.Chicago;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Chicago;
		}
		static void OnEndNoteNumberingChosung(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.Chosung;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Chosung;
		}
		static void OnEndNoteNumberingDecimalEnclosedCircle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.DecimalEnclosedCircle;
		}
		static void OnEndNoteNumberingDecimalFullWidth(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.DecimalFullWidth;
		}
		static void OnEndNoteNumberingGanada(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.DocumentProperties.DefaultSectionProperties.EndNote.NumberingFormat = NumberingFormat.Ganada;
			importer.Position.SectionFormattingInfo.EndNote.NumberingFormat = NumberingFormat.Ganada;
		}
		#endregion
		static void OnFootNoteKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!importer.DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			if (!importer.DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			FootNote note = new FootNote(importer.DocumentModel);
			importer.DocumentModel.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
			importer.Destination = new FootNoteDestination(importer, note);
		}
		#region Common Comment Keywords
		static void OnCommentStartPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentStartPositionDestination(importer);
		}
		static void OnCommentEndPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentEndPositionDestination(importer);
		}
		static void OnCommentIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentNameDestination(importer);
		}
		static void OnCommentAuthorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentAuthorDestination(importer);
		}
		static void OnCommentAnnotationKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentAnnotationDestination(importer, new CommentContentType(importer.DocumentModel));
		}
		#endregion
		#endregion
		public DefaultDestination(RtfImporter importer, PieceTable targetPieceTable)
			: base(importer, targetPieceTable) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override void BeforeNestedGroupFinishedCore(DestinationBase nestedDestination) {
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			base.NestedGroupFinished(nestedDestination);
			ListTableDestination listTableDestination = nestedDestination as ListTableDestination;
			if (listTableDestination != null)
				Importer.DocumentProperties.ListTableComplete = true;
			ListOverrideTableDestination listOverrideTableDestination = nestedDestination as ListOverrideTableDestination;
			if (listOverrideTableDestination != null)
				Importer.DocumentProperties.ListOverrideTableComplete = true;
			if (Importer.DocumentProperties.ListOverrideTableComplete && Importer.DocumentProperties.ListTableComplete) {
				RtfListConverter converter = new RtfListConverter(Importer);
				converter.Convert(Importer.DocumentProperties.ListTable, Importer.DocumentProperties.ListOverrideTable);
				Importer.DocumentProperties.ListTableComplete = false;
				Importer.DocumentProperties.ListOverrideTableComplete = false;
			}
			StyleSheetDestination styleSheetDestination = nestedDestination as StyleSheetDestination;
			if (styleSheetDestination != null) {
				ApplyStyleLinks();
			}
			DestinationPieceTable pieceTableDestination = nestedDestination as DestinationPieceTable;
			if (pieceTableDestination != null) {
				if (!Object.ReferenceEquals(pieceTableDestination.PieceTable, PieceTable))
					pieceTableDestination.FinalizePieceTableCreation();
			}
		}
		public override void AfterNestedGroupFinished(DestinationBase nestedDestination) {
			FootNoteDestination footNoteDestination = nestedDestination as FootNoteDestination;
			if (footNoteDestination != null) {
				int index = DocumentModel.FootNotes.IndexOf(footNoteDestination.Note);
				Debug.Assert(index >= 0);
				PieceTable.InsertFootNoteRun(Importer.Position, index);
			}
			EndNoteDestination endNoteDestination = nestedDestination as EndNoteDestination;
			if (endNoteDestination != null) {
				int index = DocumentModel.EndNotes.IndexOf(endNoteDestination.Note);
				Debug.Assert(index >= 0);
				PieceTable.InsertEndNoteRun(Importer.Position, index);
			}
		}
		protected internal virtual void ApplyStyleLinks() {
			Dictionary<int, int> styleLinks = Importer.LinkParagraphStyleIndexToCharacterStyleIndex;
			DocumentModel documentModel = Importer.DocumentModel;
			CharacterStyleCollection characterStyles = documentModel.CharacterStyles;
			ParagraphStyleCollection paragraphStyles = documentModel.ParagraphStyles;
			foreach (int rtfParagraphStyleIndex in styleLinks.Keys) {
				int rtfCharacterStyleIndex = styleLinks[rtfParagraphStyleIndex];
				int paragraphStyleIndex;
				int characterStyleIndex;
				if (Importer.ParagraphStyleCollectionIndex.TryGetValue(rtfParagraphStyleIndex, out paragraphStyleIndex) && Importer.CharacterStyleCollectionIndex.TryGetValue(rtfCharacterStyleIndex, out characterStyleIndex)) {
					CharacterStyle characterStyle = characterStyles[characterStyleIndex];
					ParagraphStyle paragraphStyle = paragraphStyles[paragraphStyleIndex];
					documentModel.StyleLinkManager.CreateLinkCore(paragraphStyle, characterStyle);
				}
			}
			styleLinks = Importer.NextParagraphStyleIndexTable;
			foreach (int rtfParagraphStyleIndex in styleLinks.Keys) {
				int rtfNextStyleIndex = styleLinks[rtfParagraphStyleIndex];
				int paragraphStyleIndex;
				if (Importer.ParagraphStyleCollectionIndex.TryGetValue(rtfParagraphStyleIndex, out paragraphStyleIndex)) {
					int nextStyleIndex;
					if (Importer.ParagraphStyleCollectionIndex.TryGetValue(rtfNextStyleIndex, out nextStyleIndex))
						paragraphStyles[paragraphStyleIndex].NextParagraphStyle = paragraphStyles[nextStyleIndex];
				}
			}
		}
		protected override DestinationBase CreateClone() {
			return new DefaultDestination(Importer, PieceTable);
		}
		protected internal override void FixLastParagraph() {
			Importer.ApplySectionFormatting(true);
			if (!Importer.Options.SuppressLastParagraphDelete && PieceTable.ShouldFixLastParagraph())
				PieceTable.FixLastParagraphCore();
			else {
				Importer.ApplyFormattingToLastParagraph();
			}
			if (!PieceTable.DocumentModel.DocumentCapabilities.ParagraphsAllowed) {
				PieceTable.UnsafeRemoveLastSpaceSymbolRun();
			}
			PieceTable.FixTables();
		}
	}
	#endregion
	#region RtfCharacterPropertiesContainer
	public class RtfCharacterPropertiesContainer : ICharacterPropertiesContainer {
		readonly PieceTable pieceTable;
		public RtfCharacterPropertiesContainer(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		#region ICharacterPropertiesContainer Members
		PieceTable ICharacterPropertiesContainer.PieceTable {
			get { return pieceTable; }
		}
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			throw new Exception("The method or operation is not implemented.");
		}
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
		}
		#endregion
	}
	#endregion
}
