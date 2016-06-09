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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;
#else
using System.Windows.Media;
using System.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	public abstract partial class DestinationPieceTable : DestinationBase {
		#region CreateControlCharTable
		static ControlCharTranslatorTable controlCharHT = CreateControlCharTable();
		static ControlCharTranslatorTable CreateControlCharTable() {
			ControlCharTranslatorTable table = new ControlCharTranslatorTable();
			table.Add('\r', OnParChar);
			table.Add('\n', OnParChar);
			table.Add('\\', OnEscapedChar);
			table.Add('{', OnEscapedChar);
			table.Add('}', OnEscapedChar);
			table.Add('~', OnNonBreakingSpaceChar);
			table.Add('_', OnNonBreakingHyphenChar);
			table.Add('-', OnOptionalHyphenChar);
			return table;
		}
		#endregion
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AddCommonCharacterKeywords(table);
			AddCommonParagraphKeywords(table);
			AddCommonSymbolsAndObjectsKeywords(table);
			AddCommonTabKeywords(table);
			AddCommonNumberingListsKeywords(table);
			AppendTableKeywords(table);
			return table;
		}
		internal static void AddCommonSymbolsAndObjectsKeywords(KeywordTranslatorTable table) {
			table.Add("pict", OnPictureKeyword);
			table.Add("shp", OnShapeKeyword);
			table.Add("shppict", OnShapePictureKeyword);
			table.Add("nonshppict", OnNonShapePictureKeyword);
			table.Add("line", OnLineBreakKeyword);
			table.Add("uc", OnUnicodeCountKeyword);
			table.Add("u", OnUnicodeKeyword);
			table.Add("tab", OnTabKeyword);
			table.Add("emdash", OnEmDashKeyword);
			table.Add("endash", OnEnDashKeyword);
			table.Add("lquote", OnLeftSingleQuoteKeyword);
			table.Add("rquote", OnRightSingleQuoteKeyword);
			table.Add("ldblquote", OnLeftDoubleQuoteKeyword);
			table.Add("rdblquote", OnRightDoubleQuoteKeyword);
			table.Add("bullet", OnBulletKeyword);
			table.Add("emspace", OnEmSpaceKeyword);
			table.Add("enspace", OnEnSpaceKeyword);
			table.Add("qmspace", OnQmSpaceKeyword);
			table.Add("field", OnFieldStartKeyword);
			table.Add("mailmerge", OnMailMergeKeyword);
			table.Add("bkmkstart", OnBookmarkStartKeyword);
			table.Add("bkmkend", OnBookmarkEndKeyword);
			table.Add("protstart", OnRangePermissionStartKeyword);
			table.Add("protend", OnRangePermissionEndKeyword);
			table.Add("docvar", OnDocumentVariableKeyword);
			table.Add("dxcustomrundata", OnDxCustomRunDataKeyword);
		}
		internal static void AddCommonNumberingListsKeywords(KeywordTranslatorTable table) {
			table.Add("ls", OnListOverride);
			table.Add("ilvl", OnListLevel);
			table.Add("listtext", OnListText);
			table.Add("pntext", OnListText);
			table.Add("pnseclvl", OnSectionLevelNumberingKeyword);
			table.Add("pn", OnOldParagraphNumberingKeyword);
			table.Add("pntxta", OnOldParagraphNumberingTextAfterKeyword);
			table.Add("pntxtb", OnOldParagraphNumberingTextBeforeKeyword);
		}
		internal static void AddCommonTabKeywords(KeywordTranslatorTable table) {
			table.Add("tqr", OnTabRightKeyword);
			table.Add("tqc", OnTabCenterKeyword);
			table.Add("tqdec", OnTabDecimalKeyword);
			table.Add("tldot", OnTabLeaderDotsKeyword);
			table.Add("tlmdot", OnTabLeaderMiddleDotsKeyword);
			table.Add("tlhyph", OnTabLeaderHyphensKeyword);
			table.Add("tlul", OnTabLeaderUnderlineKeyword);
			table.Add("tlth", OnTabLeaderThickLineKeyword);
			table.Add("tleq", OnTabLeaderEqualSignKeyword);
			table.Add("tx", OnTabPositionKeyword);
			table.Add("tb", OnBarTabKeyword);
		}
		internal static void AddCommonCharacterKeywords(KeywordTranslatorTable table) {
			table.Add("plain", OnPlainKeyword);
			table.Add("cs", OnCharacterStyleIndex);
			AddCharacterPropertiesKeywords(table);
		}
		internal static void AddCharacterPropertiesKeywords(KeywordTranslatorTable table) {
			table.Add("deleted", OnDeletedKeyword);
			table.Add("b", OnBoldKeyword);
			table.Add("i", OnItalicKeyword);
			table.Add("ul", OnUnderlineSingleKeyword);
			table.Add("uld", OnUnderlineDottedKeyword);
			table.Add("uldash", OnUnderlineDashedKeyword);
			table.Add("uldashd", OnUnderlineDashDottedKeyword);
			table.Add("uldashdd", OnUnderlineDashDotDottedKeyword);
			table.Add("uldb", OnUnderlineDoubleKeyword);
			table.Add("ulhwave", OnUnderlineHeavyWaveKeyword);
			table.Add("ulldash", OnUnderlineLongDashedKeyword);
			table.Add("ulth", OnUnderlineThickSingleKeyword);
			table.Add("ulthd", OnUnderlineThickDottedKeyword);
			table.Add("ulthdash", OnUnderlineThickDashedKeyword);
			table.Add("ulthdashd", OnUnderlineThickDashDottedKeyword);
			table.Add("ulthdashdd", OnUnderlineThickDashDotDottedKeyword);
			table.Add("ulthldash", OnUnderlineThickLongDashedKeyword);
			table.Add("ululdbwave", OnUnderlineDoubleWaveKeyword);
			table.Add("ulwave", OnUnderlineWaveKeyword);
			table.Add("ulw", OnUnderlineWordsOnlyKeyword);
			table.Add("ulnone", OnUnderlineNoneKeyword);
			table.Add("ulc", OnUnderlineColorKeyword);
			table.Add("strike", OnStrikeoutKeyword);
			table.Add("striked", OnDoubleStrikeoutKeyword);
			table.Add("sub", OnSubscriptKeyword);
			table.Add("super", OnSuperscriptKeyword);
			table.Add("nosupersub", OnNoSuperAndSubScriptKeyword);
			table.Add("lang", OnLanguageKeyword);
			table.Add("langfe", OnLanguageKeyword);
			table.Add("langfenp", OnLanguageNpKeyword);
			table.Add("langnp", OnLanguageNpKeyword);
			table.Add("noproof", OnNoProofKeyword);
			table.Add("caps", OnCapsKeyword);
			table.Add("v", OnHiddenTextKeyword);
			table.Add("fs", OnFontSizeKeyword);
			table.Add("f", OnFontNameKeyword);
			table.Add("af", OnAssociatedFontNameKeyword);
			table.Add("cf", OnForeColorKeyword);
			table.Add("cb", OnBackColorKeyword);
			table.Add("highlight", OnBackColorKeyword);
			table.Add("chcbpat", OnBackColorKeyword);
			table.Add("dbch", OnDoubleByteCharactersKeyword);
			table.Add("loch", OnLowAnsiFontNameKeyword);
			table.Add("hich", OnHighAnsiFontNameKeyword);
		}
		internal static void AddCommonParagraphKeywords(KeywordTranslatorTable table) {
			table.Add("par", OnParKeyword);
			table.Add("pard", OnResetParagraphPropertiesKeyword);
			table.Add("s", OnParagraphStyleIndex);
			table.Add("yts", OnTableStyleIndexForRowOrCell);
			AddParagraphPropertiesKeywords(table);
		}
		internal static void AddParagraphPropertiesKeywords(KeywordTranslatorTable table) {
			table.Add("brdrt", OnTopParagraphBorderKeyword);
			table.Add("brdrb", OnBottomParagraphBorderKeyword);
			table.Add("brdrl", OnLeftParagraphBorderKeyword);
			table.Add("brdrr", OnRightParagraphBorderKeyword);
			table.Add("posx", OnFrameHorizontalPositionKeyword);
			table.Add("posy", OnFrameVerticalPositionKeyword);
			table.Add("absw", OnFrameWidthKeyword);
			table.Add("absh", OnFrameHeightKeyword);
			table.Add("phmrg", OnParagraphHorizontalPositionTypeMarginKeyword);
			table.Add("phpg", OnParagraphHorizontalPositionTypePageKeyword);
			table.Add("phcol", OnParagraphHorizontalPositionTypeColumnKeyword);
			table.Add("pvmrg", OnParagraphVerticalPositionTypeMarginKeyword);
			table.Add("pvpg", OnParagraphVerticalPositionTypePageKeyword);
			table.Add("pvpara", OnParagraphVerticalPositionTypeLineKeyword);
			table.Add("nowrap", OnFrameNoWrapKeyword);
			table.Add("overlay", OnFrameWrapOverlayKeyword);
			table.Add("wrapdefault", OnFrameWrapDefaultKeyword);
			table.Add("wraparound", OnFrameWrapAroundKeyword);
			table.Add("wraptight", OnFrameWrapTightKeyword);
			table.Add("wrapthrough", OnFrameWrapThroughKeyword);
			table.Add("ql", OnAlignLeftKeyword);
			table.Add("qc", OnAlignCenterKeyword);
			table.Add("qr", OnAlignRightKeyword);
			table.Add("qd", OnAlignJustifyKeyword);
			table.Add("qj", OnAlignJustifyKeyword);
			table.Add("li", OnLeftIndentKeyword);
			table.Add("lin", OnLeftIndentKeyword);
			table.Add("ri", OnRightIndentKeyword);
			table.Add("fi", OnFirstLineIndentKeyword);
			table.Add("rin", OnRightIndentKeyword);
			table.Add("sb", OnSpacingBeforeKeyword);
			table.Add("sa", OnSpacingAfterKeyword);
			table.Add("sl", OnLineSpacingTypeKeyword);
			table.Add("slmult", OnLineSpacingMultiplierKeyword);
			table.Add("hyphpar", OnHyphenateParagraphKeyword);
			table.Add("noline", OnSuppressLineNumbersKeyword);
			table.Add("contextualspace", OnContextualSpacingKeyword);
			table.Add("pagebb", OnPageBreakBeforeKeyword);
			table.Add("sbauto", OnBeforeAutoSpacingKeyword);
			table.Add("saauto", OnAfterAutoSpacingKeyword);
			table.Add("keepn", OnKeepWithNextKeyword);
			table.Add("keep", OnKeepLinesTogetherKeyword);
			table.Add("widctlpar", OnWidowOrphanControlOnKeyword);
			table.Add("nowidctlpar", OnWidowOrphanControlOffKeyword);
			table.Add("outlinelevel", OnOutlineLevelKeyword);
			table.Add("cbpat", OnParagraphBackgroundKeyword);
		}
		#endregion
		const int highestCyrillic = 1279;
		const int lowLatinExtendedAdditional = 7680;
		const int highLatinExtendedAdditional = 7929;
		const int lowGeneralPunctuation = 8192;
		const int highGeneralPunctuation = 8303;
		const int lowCurrencySymbols = 8352;
		const int highCurrencySymbols = 8367;
		const int lowLetterlikeSymbols = 8448;
		const int highLetterlikeSymbols = 8506;
		const int lowNumberForms = 8531;
		const int highNumberForms = 8579;
		const int lowMathematicalOperations = 8704;
		const int highMathematicalOperations = 8945;
		const int lowAnsiCharactersStart = 0x00;
		const int lowAnsiCharactersEnd = 0x7F;
		const int highAnsiCharactersStart = 0x80;
		const int highAnsiCharactersEnd = 0xFF;
		readonly PieceTable pieceTable;
		protected DestinationPieceTable(RtfImporter importer, PieceTable targetPieceTable)
			: base(importer) {
			this.pieceTable = targetPieceTable;
		}
		#region Properties
		protected override ControlCharTranslatorTable ControlCharHT { get { return controlCharHT; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return true; } }
		protected internal override PieceTable PieceTable { get { return pieceTable; } }
		protected DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		#endregion
		#region Processing control chars and keywords
		#region ControlChars
		static void OnParChar(RtfImporter importer, char ch) {
			importer.FlushDecoder();
			importer.TableReader.OnEndParagraph();
			importer.InsertParagraph();
		}
		static void OnNonBreakingSpaceChar(RtfImporter importer, char ch) {
			importer.FlushDecoder();
			importer.ParseCharWithoutDecoding(Characters.NonBreakingSpace);
		}
		static void OnNonBreakingHyphenChar(RtfImporter importer, char ch) {
			importer.FlushDecoder();
			importer.ParseCharWithoutDecoding('-');
		}
		static void OnOptionalHyphenChar(RtfImporter importer, char ch) {
		}
		#endregion
		#region Common Symbols And Objects Keywords
		static void OnPictureKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new PictureDestination(importer);
		}
		static void OnNonShapePictureKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new SkipDestination(importer);
		}
		static void OnUnicodeCountKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.RtfFormattingInfo.UnicodeCharacterByteCount = parameterValue;
		}
		static void OnTabKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.DocumentModel.DocumentCapabilities.TabSymbolAllowed)
				importer.ParseCharWithoutDecoding(Characters.TabMark);
			else
				importer.ParseCharWithoutDecoding(' ');
		}
		static void OnEmDashKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.EmDash);
		}
		static void OnEnDashKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.EnDash);
		}
		static void OnBulletKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.Bullet);
		}
		static void OnLeftSingleQuoteKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.LeftSingleQuote);
		}
		static void OnRightSingleQuoteKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.RightSingleQuote);
		}
		static void OnLeftDoubleQuoteKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.LeftDoubleQuote);
		}
		static void OnRightDoubleQuoteKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.RightDoubleQuote);
		}
		static void OnEmSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.EmSpace);
		}
		static void OnEnSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.EnSpace);
		}
		static void OnQmSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.ParseCharWithoutDecoding(Characters.QmSpace);
		}
		static void OnShapePictureKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnShapeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ShapeDestination(importer);
		}
		static void OnLineBreakKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			switch (importer.Options.LineBreakSubstitute) {
				case LineBreakSubstitute.Space:
					importer.ParseCharWithoutDecoding(Characters.Space);
					break;
				case LineBreakSubstitute.Paragraph:
					OnParKeyword(importer, 0, false);
					break;
				default:
					importer.ParseCharWithoutDecoding(Characters.LineBreak);
					break;
			}
		}
		static void OnFieldStartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			DestinationPieceTable destination = (DestinationPieceTable)importer.Destination;
			destination.StartNewField();
		}
		static void OnMailMergeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new MailMergeDestination(importer);
		}
		static void OnBookmarkStartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new BookmarkStartDestination(importer);
		}
		static void OnBookmarkEndKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new BookmarkEndDestination(importer);
		}
		static void OnRangePermissionStartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new RangePermissionStartDestination(importer);
		}
		static void OnRangePermissionEndKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new RangePermissionEndDestination(importer);
		}
		static void OnDocumentVariableKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DocumentVariableDestination(importer);
		}
		static void OnDxCustomRunDataKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CustomRunDestination(importer);
		}
		#endregion
		#region Common Paragraph Keywords
		static void OnParKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnEndParagraph();
			if (importer.DocumentModel.DocumentCapabilities.ParagraphsAllowed)
				importer.InsertParagraph();
			else
				InsertSpace(importer);
		}
		static void OnParagraphStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.StyleIndex = importer.GetParagraphStyleIndex(parameterValue);
		}
		static void OnTableStyleIndexForRowOrCell(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.RtfTableStyleIndexForRowOrCell = parameterValue;
		}
		static void OnAlignLeftKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.Alignment = ParagraphAlignment.Left;
		}
		static void OnAlignCenterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.Alignment = ParagraphAlignment.Center;
		}
		static void OnAlignRightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.Alignment = ParagraphAlignment.Right;
		}
		static void OnAlignJustifyKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.Alignment = ParagraphAlignment.Justify;
		}
		static void OnLeftIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.LeftIndent = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnRightIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.RightIndent = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnFirstLineIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ParagraphFormattingInfo info = importer.Position.ParagraphFormattingInfo;
			if (!ShouldApplyParagraphStyle(importer)) {
				info.FirstLineIndentType = ParagraphFirstLineIndent.None;
				info.FirstLineIndent = 0;
				return;
			}
			int indent = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			if (indent > 0) {
				info.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				info.FirstLineIndent = indent;
			}
			else if (indent < 0) {
				info.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				info.FirstLineIndent = -indent;
			}
			else {
				info.FirstLineIndentType = ParagraphFirstLineIndent.None;
				info.FirstLineIndent = 0;
			}
		}
		static void OnSpacingBeforeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.SpacingBefore = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		internal static bool ShouldApplyParagraphStyle(RtfImporter importer) {
			return importer.DocumentModel.DocumentCapabilities.ParagraphStyleAllowed;
		}
		static void OnSpacingAfterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.SpacingAfter = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnLineSpacingTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.RtfLineSpacingType = parameterValue;
		}
		static void OnLineSpacingMultiplierKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.RtfLineSpacingMultiplier = parameterValue;
		}
		static void OnHyphenateParagraphKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			if (hasParameter && parameterValue == 0)
				importer.Position.ParagraphFormattingInfo.SuppressHyphenation = true;
			else
				importer.Position.ParagraphFormattingInfo.SuppressHyphenation = false;
		}
		static void OnSuppressLineNumbersKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.SuppressLineNumbers = true;
		}
		static void OnContextualSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.ContextualSpacing = true;
		}
		static void OnPageBreakBeforeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.ParagraphFormattingInfo.PageBreakBefore = parameterValue != 0;
		}
		static void OnBeforeAutoSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer)) {
				importer.Position.ParagraphFormattingInfo.BeforeAutoSpacing = parameterValue != 0;
				if (parameterValue != 0)
					importer.Position.ParagraphFormattingInfo.SpacingBefore = 0;
			}
		}
		static void OnAfterAutoSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer)) {
				importer.Position.ParagraphFormattingInfo.AfterAutoSpacing = parameterValue != 0;
				if (parameterValue != 0)
					importer.Position.ParagraphFormattingInfo.SpacingAfter = 0;
			}
		}
		static void OnKeepWithNextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.ParagraphFormattingInfo.KeepWithNext = parameterValue != 0;
		}
		static void OnKeepLinesTogetherKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			if (!hasParameter)
				parameterValue = 1;
			importer.Position.ParagraphFormattingInfo.KeepLinesTogether = parameterValue != 0;
		}
		static void OnWidowOrphanControlOnKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.WidowOrphanControl = true;
		}
		static void OnWidowOrphanControlOffKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.WidowOrphanControl = false;
		}
		static void OnParagraphBackgroundKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			RtfDocumentProperties props = importer.DocumentProperties;
			Color color = props.Colors.GetRtfColorById(parameterValue);
			importer.Position.ParagraphFormattingInfo.BackColor = color;
		}
		static void OnOutlineLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!ShouldApplyParagraphStyle(importer))
				return;
			int level = parameterValue;
			if (level < 0 || level > 8)
				level = 0;
			else
				level++;
			if (!hasParameter)
				level = 0;
			importer.Position.ParagraphFormattingInfo.OutlineLevel = level;
		}
		internal static void OnResetParagraphPropertiesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer)) {
				int oldTableStyleIndex = importer.Position.ParagraphFormattingInfo.RtfTableStyleIndexForRowOrCell;
				importer.Position.ParagraphFormattingInfo = new RtfParagraphFormattingInfo();
				importer.Position.ParagraphFormattingInfo.RtfTableStyleIndexForRowOrCell = oldTableStyleIndex;
			}
			importer.Position.ParagraphFrameFormattingInfo = null;
		}
		static void OnTopParagraphBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.TopBorder = new BorderInfo();
			importer.Position.ParagraphFormattingInfo.ProcessedBorder = importer.Position.ParagraphFormattingInfo.TopBorder;
		}
		static void OnBottomParagraphBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.BottomBorder = new BorderInfo();
			importer.Position.ParagraphFormattingInfo.ProcessedBorder = importer.Position.ParagraphFormattingInfo.BottomBorder;
		}
		static void OnLeftParagraphBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.LeftBorder = new BorderInfo();
			importer.Position.ParagraphFormattingInfo.ProcessedBorder = importer.Position.ParagraphFormattingInfo.LeftBorder;
		}
		static void OnRightParagraphBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.RightBorder = new BorderInfo();
			importer.Position.ParagraphFormattingInfo.ProcessedBorder = importer.Position.ParagraphFormattingInfo.RightBorder;
		}
		static bool EnsureFramePropertiesExists(RtfImporter importer) {
			if (importer.Position.PieceTable.IsTextBox)
				return false;
			if (importer.Position.ParagraphFrameFormattingInfo == null)
				importer.Position.ParagraphFrameFormattingInfo = new ParagraphFrameFormattingInfo();
			return true;
		}
		internal static void OnFrameHorizontalPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.HorizontalPosition = parameterValue;
				importer.Position.ParagraphFrameFormattingInfo.X = Math.Abs(importer.UnitConverter.TwipsToModelUnits(parameterValue));
			}
		}
		internal static void OnFrameVerticalPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.VerticalPosition = parameterValue;
				importer.Position.ParagraphFrameFormattingInfo.Y = Math.Abs(importer.UnitConverter.TwipsToModelUnits(parameterValue));
			}
		}
		internal static void OnFrameWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.Width = Math.Abs(importer.UnitConverter.TwipsToModelUnits(parameterValue));
			}
		}
		internal static void OnFrameHeightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.Height = Math.Abs(importer.UnitConverter.TwipsToModelUnits(parameterValue));
				if (parameterValue < 0)
					importer.Position.ParagraphFrameFormattingInfo.HorizontalRule = ParagraphFrameHorizontalRule.Exact;
				if (parameterValue > 0)
					importer.Position.ParagraphFrameFormattingInfo.HorizontalRule = ParagraphFrameHorizontalRule.AtLeast;
			}
		}
		private static void OnParagraphHorizontalPositionTypeMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Margin;
			}
		}
		private static void OnParagraphHorizontalPositionTypePageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Page;
			}
		}
		private static void OnParagraphHorizontalPositionTypeColumnKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Column;
			}
		}
		private static void OnParagraphVerticalPositionTypeMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.VerticalPositionType = ParagraphFrameVerticalPositionType.Margin;
			}
		}
		private static void OnParagraphVerticalPositionTypePageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.VerticalPositionType = ParagraphFrameVerticalPositionType.Page;
			}
		}
		private static void OnParagraphVerticalPositionTypeLineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.VerticalPositionType = ParagraphFrameVerticalPositionType.Paragraph;
			}
		}
		private static void OnFrameNoWrapKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.TextWrapType = ParagraphFrameTextWrapType.NotBeside;
			}
		}
		private static void OnFrameWrapOverlayKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.TextWrapType = ParagraphFrameTextWrapType.None;
			}
		}
		private static void OnFrameWrapDefaultKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		private static void OnFrameWrapAroundKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.TextWrapType = ParagraphFrameTextWrapType.Around;
			}
		}
		private static void OnFrameWrapTightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.TextWrapType = ParagraphFrameTextWrapType.Tight;
			}
		}
		private static void OnFrameWrapThroughKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer) && EnsureFramePropertiesExists(importer)) {
				importer.Position.ParagraphFrameFormattingInfo.TextWrapType = ParagraphFrameTextWrapType.Through;
			}
		}
		#endregion
		#region Common Character Keywords
		static void OnCharacterStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.DocumentModel.DocumentCapabilities.CharacterStyleAllowed)
				importer.Position.CharacterStyleIndex = importer.GetCharacterStyleIndex(parameterValue);
		}
		static void OnBoldKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontBold = val;
		}
		static void OnDeletedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.RtfFormattingInfo.Deleted = val;
		}
		static void OnItalicKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontItalic = val;
		}
		static void OnUnderlineKeywordCore(RtfImporter importer, int parameterValue, bool hasParameter, UnderlineType underlineType) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontUnderlineType = val ? underlineType : UnderlineType.None;
		}
		static void OnUnderlineSingleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.Single);
		}
		static void OnUnderlineDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.Dotted);
		}
		static void OnUnderlineDashedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.Dashed);
		}
		static void OnUnderlineDashDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.DashDotted);
		}
		static void OnUnderlineDashDotDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.DashDotDotted);
		}
		static void OnUnderlineDoubleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.Double);
		}
		static void OnUnderlineHeavyWaveKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.HeavyWave);
		}
		static void OnUnderlineLongDashedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.LongDashed);
		}
		static void OnUnderlineThickSingleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickSingle);
		}
		static void OnUnderlineThickDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickDotted);
		}
		static void OnUnderlineThickDashedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickDashed);
		}
		static void OnUnderlineThickDashDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickDashDotted);
		}
		static void OnUnderlineThickDashDotDottedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickDashDotDotted);
		}
		static void OnUnderlineThickLongDashedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.ThickLongDashed);
		}
		static void OnUnderlineDoubleWaveKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.DoubleWave);
		}
		static void OnUnderlineWaveKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnUnderlineKeywordCore(importer, parameterValue, hasParameter, UnderlineType.Wave);
		}
		static void OnUnderlineNoneKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.CharacterFormatting.FontUnderlineType = UnderlineType.None;
		}
		static void OnUnderlineWordsOnlyKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontUnderlineType = val ? UnderlineType.Single : UnderlineType.None;
			importer.Position.CharacterFormatting.UnderlineWordsOnly = val;
		}
		static void OnUnderlineColorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			RtfDocumentProperties props = importer.DocumentProperties;
			Color underlineColor = props.Colors.GetRtfColorById(parameterValue);
			importer.Position.CharacterFormatting.UnderlineColor = underlineColor;
		}
		static void OnStrikeoutKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontStrikeoutType = val ? StrikeoutType.Single : StrikeoutType.None;
		}
		static void OnDoubleStrikeoutKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontStrikeoutType = val ? StrikeoutType.Double : StrikeoutType.None;
		}
		static void OnSubscriptKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.Script = val ? CharacterFormattingScript.Subscript : CharacterFormattingScript.Normal;
		}
		static void OnSuperscriptKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.Script = val ? CharacterFormattingScript.Superscript : CharacterFormattingScript.Normal;
		}
		static void OnNoSuperAndSubScriptKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.CharacterFormatting.Script = CharacterFormattingScript.Normal;
		}
		static void OnLanguageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			OnLanguageNpKeyword(importer, parameterValue, hasParameter);
			importer.Position.CharacterFormatting.NoProof = false;
		}
		static void OnLanguageNpKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && (parameterValue != 0) && (parameterValue != 1024)) {
				CultureInfo latin = CultureInfoHelper.CreateCultureInfo(parameterValue);
				LangInfo val = new LangInfo(latin, latin, latin);
				importer.Position.CharacterFormatting.LangInfo = val;
			}
		}
		static void OnNoProofKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.NoProof = val;
		}
		static void OnCapsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.AllCaps = val;
		}
		static void OnHiddenTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.Hidden = val;
		}
		static void OnFontSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 24;
			importer.Position.CharacterFormatting.DoubleFontSize = Math.Max(PredefinedFontSizeCollection.MinFontSize, parameterValue);
		}
		static void OnDoubleByteCharactersKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.FontType = RtfFontType.DoubleByteCharactersFont;
		}
		static void OnLowAnsiFontNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.FontType = RtfFontType.LowAnsiCharactersFont;
		}
		static void OnHighAnsiFontNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.FontType = RtfFontType.HighAnsiCharactersFont;
		}
		static void OnAssociatedFontNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.Position.FontType == RtfFontType.Undefined)
				return;
			OnFontNameKeyword(importer, parameterValue, hasParameter);
		}
		static void OnFontNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = importer.DocumentProperties.DefaultFontNumber;
			RtfDocumentProperties props = importer.DocumentProperties;
			importer.SetFont(props.Fonts.GetRtfFontInfoById(parameterValue));
		}
		static void OnForeColorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			RtfDocumentProperties props = importer.DocumentProperties;
			Color foreColor = props.Colors.GetRtfColorById(parameterValue);
			importer.Position.CharacterFormatting.ForeColor = foreColor;
		}
		static void OnBackColorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			RtfDocumentProperties props = importer.DocumentProperties;
			Color backColor = props.Colors.GetRtfColorById(parameterValue);
			importer.Position.CharacterFormatting.BackColor = backColor;
		}
		static void OnPlainKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			CharacterFormattingInfo defaultFormatting = (CharacterFormattingInfo)importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem;
			CharacterFormattingBase currentPositionFormatting = importer.Position.CharacterFormatting;
			currentPositionFormatting.FontName = importer.DocumentProperties.Fonts.GetRtfFontInfoById(importer.DocumentProperties.DefaultFontNumber).Name;
			importer.Position.ResetUseAssociatedProperties();
			currentPositionFormatting.DoubleFontSize = 24;
			currentPositionFormatting.FontBold = defaultFormatting.FontBold;
			currentPositionFormatting.FontItalic = defaultFormatting.FontItalic;
			currentPositionFormatting.FontStrikeoutType = defaultFormatting.FontStrikeoutType;
			currentPositionFormatting.FontUnderlineType = defaultFormatting.FontUnderlineType;
			currentPositionFormatting.AllCaps = defaultFormatting.AllCaps;
			currentPositionFormatting.Hidden = defaultFormatting.Hidden;
			currentPositionFormatting.UnderlineWordsOnly = defaultFormatting.UnderlineWordsOnly;
			currentPositionFormatting.StrikeoutWordsOnly = defaultFormatting.StrikeoutWordsOnly;
			currentPositionFormatting.ForeColor = defaultFormatting.ForeColor;
			currentPositionFormatting.BackColor = defaultFormatting.BackColor;
			currentPositionFormatting.StrikeoutColor = defaultFormatting.StrikeoutColor;
			currentPositionFormatting.UnderlineColor = defaultFormatting.UnderlineColor;
			currentPositionFormatting.Script = defaultFormatting.Script;
			importer.SetCodePage(importer.DocumentProperties.DefaultCodePage);
		}
		#endregion
		#region Common Numbering Lists Keywords
		internal static void OnListOverride(RtfImporter importer, int parameterValue, bool hasParameter) {
			NumberingListIndex index;
			if (importer.ListOverrideIndexToNumberingListIndexMap.TryGetValue(parameterValue, out index))
				importer.Position.ParagraphFormattingInfo.NumberingListIndex = index;
		}
		internal static void OnListLevel(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.ListLevelIndex = parameterValue;
		}
		internal static void OnListText(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new SkipDestination(importer);
		}
		static void OnSectionLevelNumberingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DestinationOldSectionNumberingLevel(importer, parameterValue);
		}
		static void OnOldParagraphNumberingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DestinationOldParagraphNumbering(importer);
		}
		static void OnOldParagraphNumberingTextBeforeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new TextBeforeDestination(importer);
		}
		static void OnOldParagraphNumberingTextAfterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new TextAfterDestination(importer);
		}
		#endregion
		#region Common Tab Keywords
		static void OnTabRightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabAlignment = TabAlignmentType.Right;
		}
		static void OnTabCenterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabAlignment = TabAlignmentType.Center;
		}
		static void OnTabDecimalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabAlignment = TabAlignmentType.Decimal;
		}
		static void OnTabLeaderDotsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.Dots;
		}
		static void OnTabLeaderMiddleDotsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.MiddleDots;
		}
		static void OnTabLeaderHyphensKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.Hyphens;
		}
		static void OnTabLeaderUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.Underline;
		}
		static void OnTabLeaderThickLineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.ThickLine;
		}
		static void OnTabLeaderEqualSignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (ShouldApplyParagraphStyle(importer))
				importer.Position.ParagraphFormattingInfo.TabLeader = TabLeaderType.EqualSign;
		}
		static void OnTabPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			RtfParagraphFormattingInfo info = importer.Position.ParagraphFormattingInfo;
			if (hasParameter) {
				TabInfo tab = new TabInfo(importer.UnitConverter.TwipsToModelUnits(parameterValue), info.TabAlignment, info.TabLeader);
				info.Tabs.Add(tab);
			}
			info.TabAlignment = TabInfo.DefaultAlignment;
			info.TabLeader = TabInfo.DefaultLeader;
		}
		static void OnBarTabKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			RtfParagraphFormattingInfo info = importer.Position.ParagraphFormattingInfo;
			info.TabAlignment = TabInfo.DefaultAlignment;
			info.TabLeader = TabInfo.DefaultLeader;
		}
		#endregion
		#region Table Keywords
		static void OnTableRowDefaultsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnTableRowDefaults();
		}
		static void OnTableStyleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.DocumentModel.DocumentCapabilities.TableStyleAllowed)
				importer.TableReader.TableProperties.TableStyleIndex = importer.GetTableStyleIndex(parameterValue);
		}
		static void OnInTableParagraphKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.InTableParagraph = true;
		}
		static void OnRowKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnEndRow();
		}
		static void OnCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnEndCell();
			importer.InsertParagraph();
		}
		static void OnNestedCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnEndNestedCell();
			importer.InsertParagraph();
		}
		static void OnNestedRowKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnEndNestedRow();
		}
		static void OnNestedTablePropertiesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnStartNestedTableProperties();
		}
		static void OnNoNestedTablesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new SkipNestedTableDestination(importer);
		}
		static void OnItapKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue < 0)
				return;
			importer.Position.ParagraphFormattingInfo.NestingLevel = parameterValue;
		}
		#endregion
		#endregion
		protected virtual void StartNewField() {
			Importer.Fields.Push(new RtfFieldInfo());
			Importer.Destination = new FieldDestination(Importer);
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			PictureDestination pictDest = nestedDestination as PictureDestination;
			if (pictDest != null) {
				RtfImageInfo imageInfo = pictDest.GetImageInfo();
				InsertImage(Importer, imageInfo);
			}
			NumberingListIndex oldListIndex = Importer.Position.CurrentOldSimpleList ? Importer.Position.CurrentOldSimpleListIndex : Importer.Position.CurrentOldMultiLevelListIndex;
			if (oldListIndex >= NumberingListIndex.MinValue) {
				int levelNumber = Importer.Position.CurrentOldSimpleList ? 0 : Importer.Position.CurrentOldListLevelNumber;
				IListLevel listLevel = DocumentModel.NumberingLists[oldListIndex].Levels[levelNumber];
				TextBeforeDestination textBeforeDestination = nestedDestination as TextBeforeDestination;
				if (textBeforeDestination != null) {
					listLevel.ListLevelProperties.DisplayFormatString = textBeforeDestination.Value + listLevel.ListLevelProperties.DisplayFormatString;
					return;
				}
				TextAfterDestination textAfterDestination = nestedDestination as TextAfterDestination;
				if (textAfterDestination != null) {
					listLevel.ListLevelProperties.DisplayFormatString = listLevel.ListLevelProperties.DisplayFormatString + textAfterDestination.Value;
					return;
				}
			}
		}
		internal static void InsertImage(RtfImporter importer, RtfImageInfo imageInfo) {
			if (!DocumentFormatsHelper.ShouldInsertPicture(importer.DocumentModel) || imageInfo == null || imageInfo.RtfImage == null) {
				InsertSpace(importer);
				return;
			}
			PieceTable table = importer.PieceTable;
			int scaleX = Math.Max(1, imageInfo.ScaleX);
			int scaleY = Math.Max(1, imageInfo.ScaleY);
			table.AppendImage(importer.Position, imageInfo.RtfImage, scaleX, scaleY);
			TextRunCollection runs = table.Runs;
			InlinePictureRun run = table.LastInsertedInlinePictureRunInfo.Run;
			run.SetOriginalSize(imageInfo.SizeInModelUnits);
			run.Properties.PseudoInline = imageInfo.PseudoInline;
		}
		static void InsertSpace(RtfImporter importer) {
			if (importer.Position.RtfFormattingInfo.Deleted && importer.Options.IgnoreDeletedText)
				return;
			importer.PieceTable.InsertTextCore(importer.Position, " ");
		}
		protected override void ProcessCharCore(char ch) {
			RtfInputPosition pos = Importer.Position;
			if (pos.RtfFormattingInfo.Deleted && Importer.Options.IgnoreDeletedText)
				return;
			if (!pos.UseDoubleByteCharactersFontName && !pos.UseLowAnsiCharactersFontName && !pos.UseHighAnsiCharactersFontName)
				PieceTable.AppendText(pos, ch);
			else
				AppendChar(ch);				
			}
		protected override void ProcessTextCore(string text) {
			RtfInputPosition pos = Importer.Position;
			if (pos.RtfFormattingInfo.Deleted && Importer.Options.IgnoreDeletedText)
				return;
			if (!pos.UseDoubleByteCharactersFontName && !pos.UseLowAnsiCharactersFontName && !pos.UseHighAnsiCharactersFontName)
				PieceTable.AppendText(pos, text);
			else
				for (int i = 0; i < text.Length; i++)
					AppendChar(text[i]);
		}
		void AppendChar(char ch) {
			RtfInputPosition pos = Importer.Position;
			string oldFontName = pos.CharacterFormatting.FontName;
			if (pos.UseDoubleByteCharactersFontName && IsDoubleByteChar(ch))
				pos.CharacterFormatting.FontName = pos.DoubleByteCharactersFontName;
			else if (pos.UseLowAnsiCharactersFontName && IsLowAnsiCharacter(ch))
				pos.CharacterFormatting.FontName = pos.LowAnsiCharactersFontName;
			else if (pos.UseHighAnsiCharactersFontName && IsHighAnsiCharacter(ch))
				pos.CharacterFormatting.FontName = pos.HighAnsiCharactersFontName;
			PieceTable.AppendText(pos, ch);
			pos.CharacterFormatting.FontName = oldFontName;
		}
		bool IsLowAnsiCharacter(char ch) {
			int chVal = (int)ch;
			return chVal >= lowAnsiCharactersStart && chVal <= lowAnsiCharactersEnd;
		}
		bool IsHighAnsiCharacter(char ch) {
			int chVal = (int)ch;
			return chVal >= highAnsiCharactersStart && chVal <= highAnsiCharactersEnd;
		}
		bool IsDoubleByteChar(char ch) {
			int chVal = (int)ch;
			if (chVal <= highestCyrillic)
				return false;
			if (chVal >= lowLatinExtendedAdditional && chVal <= highLatinExtendedAdditional)
				return false;
			if (chVal >= lowGeneralPunctuation && chVal <= highGeneralPunctuation)
				return false;
			if (chVal >= lowCurrencySymbols && chVal <= highCurrencySymbols)
				return false;
			if (chVal >= lowLetterlikeSymbols && chVal <= highLetterlikeSymbols)
				return false;
			if (chVal >= lowNumberForms && chVal <= highNumberForms)
				return false;
			if (chVal >= lowMathematicalOperations && chVal <= highMathematicalOperations)
				return false;
			return true;
		}
		public virtual void FinalizePieceTableCreation() {
			Debug.Assert(Object.ReferenceEquals(PieceTable, Importer.PieceTable));
			Debug.Assert(Object.ReferenceEquals(PieceTable, Importer.PieceTableInfo.PieceTable));
			Importer.TableReader.InsertTables();
			FixLastParagraph();
			InsertBookmarks();
			InsertRangePermissions();
			InsertComments();
			Importer.LinkParagraphStylesWithNumberingLists();
			NormalizeProperties();
			FixFieldCodeRunsProperties();
		}
		protected virtual void FixFieldCodeRunsProperties() {
			FieldCollection fields = PieceTable.Fields;
			TextRunCollection runs = PieceTable.Runs;
			int count = fields.Count;
			for (int i = 0; i < count; i++) {
				TextRunBase sourceRun = GetFormattingSourceRun(fields[i]);
				if (sourceRun == null)
					continue;
				TextRunBase fieldCodeStart = runs[fields[i].Code.Start];
				TextRunBase fieldCodeEnd = runs[fields[i].Code.End];
				SetFieldRunFormatting(fieldCodeStart, sourceRun);
				SetFieldRunFormatting(fieldCodeEnd, sourceRun);
			}
		}
		TextRunBase GetFormattingSourceRun(Field field) {
			TextRunCollection runs = PieceTable.Runs;
			for (RunIndex runIndex = field.FirstRunIndex + 1; runIndex < field.Code.End; runIndex++) {
				TextRunBase run = runs[runIndex];
				if (!(run is FieldCodeRunBase) && !(run is FieldResultEndRun))
					return run;
			}
			return null;
		}
		protected virtual void SetFieldRunFormatting(TextRunBase fieldCodeStart, TextRunBase sourceRun) {
			fieldCodeStart.CharacterProperties.CopyFrom(sourceRun.CharacterProperties);
			fieldCodeStart.CharacterStyleIndex = sourceRun.CharacterStyleIndex;
		}
		protected internal virtual void FixLastParagraph() {
			if (PieceTable.ShouldFixLastParagraph())
				PieceTable.FixLastParagraphCore();
			else
				Importer.ApplyFormattingToLastParagraph();
			PieceTable.FixTables();
		}
		void NormalizeProperties() {
			NormalizeParagraphStyleNumbering();
			NormalizeParagraphsProperties();
			NormalizeTableProperties();
		}
		void NormalizeParagraphStyleNumbering() {
			ParagraphStyleCollection styles = DocumentModel.ParagraphStyles;
			int count = DocumentModel.ParagraphStyles.Count;
			for (int i = 0; i < count; i++) {
				NormalizeParagrapStyleNumbering(styles[i]);
			}
		}
		void NormalizeParagrapStyleNumbering(ParagraphStyle paragraphStyle) {
			NumberingListIndex ownNumberingListIndex = paragraphStyle.GetOwnNumberingListIndex();
			if (paragraphStyle.Parent == null) {
				if (ownNumberingListIndex == NumberingListIndex.NoNumberingList)
					ownNumberingListIndex = NumberingListIndex.ListIndexNotSetted;
				paragraphStyle.SetNumberingListIndex(ownNumberingListIndex);
				return;
			}
			NumberingListIndex parentNumberingListIndex = paragraphStyle.Parent.GetNumberingListIndex();
			paragraphStyle.SetNumberingListIndex(GetNormalizedNumberingListIndex(ownNumberingListIndex, parentNumberingListIndex));
		}
		NumberingListIndex GetNormalizedNumberingListIndex(NumberingListIndex ownNumberingListIndex, NumberingListIndex parentNumberingListIndex) {
			if (ownNumberingListIndex == parentNumberingListIndex)
				return NumberingListIndex.ListIndexNotSetted;
			if (ownNumberingListIndex == NumberingListIndex.NoNumberingList || ownNumberingListIndex == NumberingListIndex.ListIndexNotSetted)
				return parentNumberingListIndex >= NumberingListIndex.MinValue ? NumberingListIndex.NoNumberingList : NumberingListIndex.ListIndexNotSetted;
			return ownNumberingListIndex;
		}
		void NormalizeTableProperties() {
			PieceTable.Tables.ForEach(NormalizeTableProperties);
			pieceTable.FixTables();
		}
		void NormalizeTableProperties(Table table) {
			TableProperties tableProperties = table.TableProperties;
			MergedTableProperties parentProperties = table.GetParentMergedTableProperties();
			Importer.ApplyTableProperties(tableProperties, parentProperties);
			NormalizeTableRowProperties(table);
		}
		void NormalizeTableRowProperties(Table table) {
			table.Rows.ForEach(NormalizeTableRowProperties);
		}
		void NormalizeTableRowProperties(TableRow row) {
			row.UnsubscribeRowPropertiesEvents();
			try {
				TableRowProperties rowProperties = row.Properties;
				MergedTableRowProperties parentProperties = row.GetParentMergedTableRowProperties();
				Importer.ApplyTableRowProperties(rowProperties, parentProperties);
				NormalizeTableCellProperties(row);
			}
			finally {
				row.SubscribeRowPropertiesEvents();
			}
		}
		void NormalizeTableCellProperties(TableRow row) {
			row.Cells.ForEach(NormalizeTableCellProperties);
		}
		void NormalizeTableCellProperties(TableCell cell) {
			cell.UnsubscribeCellPropertiesEvents();
			try {
				TableCellProperties cellProperties = cell.Properties;
				MergedTableCellProperties parentProperties = cell.GetParentMergedTableCellProperties();
				Importer.ApplyTableCellProperties(cellProperties, parentProperties);
			}
			finally {
				cell.SubscribeCellPropertiesEvents();
			}
		}
		class CustomMergedParagraphPropertiesCachedResult : ParagraphMergedParagraphPropertiesCachedResult {
			int initialParagraphPropertiesIndex = -1;
			int finalParagraphPropertiesIndex = -1;
			public int InitialParagraphPropertiesIndex { get { return initialParagraphPropertiesIndex; } set { initialParagraphPropertiesIndex = value; } }
			public int FinalParagraphPropertiesIndex { get { return finalParagraphPropertiesIndex; } set { finalParagraphPropertiesIndex = value; } }
		}
		void NormalizeParagraphsProperties() {
			Dictionary<Type, CustomMergedCharacterPropertiesCachedResult> textRunsCachedResults = new Dictionary<Type, CustomMergedCharacterPropertiesCachedResult>();
			CustomMergedParagraphPropertiesCachedResult cachedResult = new CustomMergedParagraphPropertiesCachedResult();
			ParagraphToTableStyleIndexMap paragraphStyles = Importer.PieceTableInfo.ParagraphTableStyles;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			int nextIndexInParagraphStyleMap = 0;
			int paragraphTableStylesCount = paragraphStyles.Count;
			ParagraphIndex nextParagraphIndexInParagraphStyleMap = nextIndexInParagraphStyleMap < paragraphTableStylesCount ? paragraphStyles[nextIndexInParagraphStyleMap].ParagraphIndex : ParagraphIndex.MaxValue;
			RunIndex runIndex = new RunIndex(0);			
			for (ParagraphIndex i = new ParagraphIndex(0); i < count; i++) {
				Paragraph paragraph = paragraphs[i];
				if (i == nextParagraphIndexInParagraphStyleMap) {
					int styleIndex = paragraphStyles[nextIndexInParagraphStyleMap].RtfTableStyleIndex;
					nextIndexInParagraphStyleMap++;
					nextParagraphIndexInParagraphStyleMap = nextIndexInParagraphStyleMap < paragraphTableStylesCount ? paragraphStyles[nextIndexInParagraphStyleMap].ParagraphIndex : ParagraphIndex.MaxValue;
					int tableStyleIndex = Importer.GetTableStyleIndex(styleIndex, -1);
					NormalizeParagraphProperties(paragraph, cachedResult, tableStyleIndex);
					runIndex = NormalizeRunsProperties(paragraph, runIndex, tableStyleIndex, textRunsCachedResults);
				}
				else {
					NormalizeParagraphProperties(paragraph, cachedResult, -1);
					runIndex = NormalizeRunsProperties(paragraph, runIndex, -1, textRunsCachedResults);
				}
			}
		}
		void NormalizeParagraphProperties(Paragraph paragraph, CustomMergedParagraphPropertiesCachedResult cachedResult, int tableStyleIndex) {
			ParagraphProperties properties = paragraph.ParagraphProperties;
			if (cachedResult.InitialParagraphPropertiesIndex == properties.Index && paragraph.TryUseParentMergedCachedResult(cachedResult, tableStyleIndex)) {
				properties.SetIndexInitial(cachedResult.FinalParagraphPropertiesIndex);
			}
			else {
				cachedResult.InitialParagraphPropertiesIndex = properties.Index;
				TableStyle tableStyleToMerge = GetTableStyleToMerge(tableStyleIndex);
				MergedParagraphProperties parentProperties = paragraph.GetParentMergedWithTableStyleParagraphProperties(true, tableStyleToMerge);
				cachedResult.MergedParagraphProperties = parentProperties;
				Importer.ApplyParagraphProperties(properties, properties.Info.Info, parentProperties);
				cachedResult.FinalParagraphPropertiesIndex = properties.Index;
			}
			if (paragraph.GetListLevelIndex() == 0)
				paragraph.SetNumberingListIndex(GetNormalizedNumberingListIndex(paragraph.GetOwnNumberingListIndex(), paragraph.ParagraphStyle.GetNumberingListIndex()));
		}
		TableStyle GetTableStyleToMerge(int tableStyleIndex) {
			if (tableStyleIndex >= 0) {
				TableStyle tableStyle = DocumentModel.TableStyles[tableStyleIndex];
				if (tableStyle.StyleName != KnownStyleNames.NormalTable)
					return tableStyle;
			}
			return null;
		}
		class CustomMergedCharacterPropertiesCachedResult : RunMergedCharacterPropertiesCachedResult {
			int initialRunCharacterPropertiesIndex = -1;
			int finalRunCharacterPropertiesIndex = -1;
			int tableStyleIndex = -1;
			public int InitialRunCharacterPropertiesIndex { get { return initialRunCharacterPropertiesIndex; } set { initialRunCharacterPropertiesIndex = value; } }
			public int FinalRunCharacterPropertiesIndex { get { return finalRunCharacterPropertiesIndex; } set { finalRunCharacterPropertiesIndex = value; } }
			public int TableStyleIndex { get { return tableStyleIndex; } set { tableStyleIndex = value; } }
		}
		RunIndex NormalizeRunsProperties(Paragraph paragraph, RunIndex runIndex, int tableStyleIndex, Dictionary<Type, CustomMergedCharacterPropertiesCachedResult> cachedResults) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex count = new RunIndex(runs.Count);
			while (runIndex < count) {
				TextRunBase run = runs[runIndex];
				if (!Object.ReferenceEquals(paragraph, run.Paragraph))
					break;
				CustomMergedCharacterPropertiesCachedResult cachedResult;
				if (!cachedResults.TryGetValue(run.GetType(), out cachedResult)) {
					cachedResult = new CustomMergedCharacterPropertiesCachedResult();
					cachedResults.Add(run.GetType(), cachedResult);
				}
				NormalizeRunProperties(run, cachedResult, tableStyleIndex);
				runIndex++;
			}
			return runIndex;
		}
		void NormalizeRunProperties(TextRunBase run, CustomMergedCharacterPropertiesCachedResult cachedResult, int tableStyleIndex) {
			CharacterProperties properties = run.CharacterProperties;
			if (cachedResult.InitialRunCharacterPropertiesIndex == properties.Index && run.TryUseParentMergedCachedResult(cachedResult) && cachedResult.TableStyleIndex == tableStyleIndex) {
				properties.SetIndexInitial(cachedResult.FinalRunCharacterPropertiesIndex);				
				return;
			}
			cachedResult.InitialRunCharacterPropertiesIndex = properties.Index;
			cachedResult.TableStyleIndex = tableStyleIndex;
			TableStyle tableStyle = GetTableStyleToMerge(tableStyleIndex);
			MergedCharacterProperties parentProperties = run.GetParentMergedCharacterProperties(true, tableStyle);
			cachedResult.MergedCharacterProperties = parentProperties;
			Importer.ApplyCharacterProperties(properties, properties.Info.Info, parentProperties);
			cachedResult.FinalRunCharacterPropertiesIndex = properties.Index;
		}
		void InsertBookmarks() {
			if (!DocumentModel.DocumentCapabilities.BookmarksAllowed)
				return;
			Dictionary<string, ImportBookmarkInfo> bookmarks = Importer.Bookmarks;
			foreach (string name in bookmarks.Keys) {
				ImportBookmarkInfo bookmark = bookmarks[name];
				if (bookmark.Validate(PieceTable))
					PieceTable.CreateBookmarkCore(bookmark.Start, bookmark.End - bookmark.Start, name);
			}
		}
		void InsertRangePermissions() {
			Dictionary<string, ImportRangePermissionInfo> rangePermissions = Importer.RangePermissions;
			foreach (string name in rangePermissions.Keys) {
				ImportRangePermissionInfo rangePermission = rangePermissions[name];
				if (rangePermission.Validate(PieceTable))
					PieceTable.ApplyDocumentPermission(rangePermission.Start, rangePermission.End, rangePermission.PermissionInfo);
			}
		}
		protected virtual void InsertComments() {
			Dictionary<string, RTFImportCommentInfo> comments = Importer.Comments;
			foreach (string name in comments.Keys) {
				ImportCommentInfo comment = comments[name];
				comment.CalculateCommentPosition();
				if (comment.Validate(PieceTable)) {
					Comment parentComment = FindParentComment(comment, name);
					Comment currentComment = PieceTable.CreateCommentCore(comment.Start, comment.End - comment.Start, comment.Name, comment.Author, comment.Date, parentComment, comment.Content);
					comment.Comment = currentComment;
				}
			}
		}
		Comment FindParentComment( ImportCommentInfo comment, string name) {
			if (comment.ParaId < 0 ) {
				Dictionary<string, RTFImportCommentInfo> comments = Importer.Comments;
				int index;
				string parentName;
				if (Importer.CommentsRef.TryGetValue(name, out index) && Importer.CommentsIndexRef.TryGetValue(index + comment.ParaId, out parentName) && comments.ContainsKey(parentName)) 
					return comments[parentName].Comment;
				else 
					return null;
			}
			return null;
		}
	}
}
