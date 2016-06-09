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
using System.CodeDom.Compiler;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Run
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Range : IWordObject {
		string Text { get; set; }
		Range FormattedText { get; set; }
		int Start { get; set; }
		int End { get; set; }
		Font Font { get; set; }
		Range Duplicate { get; }
		WdStoryType StoryType { get; }
		Tables Tables { get; }
		Words Words { get; }
		Sentences Sentences { get; }
		Characters Characters { get; }
		Footnotes Footnotes { get; }
		Endnotes Endnotes { get; }
		Comments Comments { get; }
		Cells Cells { get; }
		Sections Sections { get; }
		Paragraphs Paragraphs { get; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		TextRetrievalMode TextRetrievalMode { get; set; }
		Fields Fields { get; }
		FormFields FormFields { get; }
		Frames Frames { get; }
		ParagraphFormat ParagraphFormat { get; set; }
		ListFormat ListFormat { get; }
		Bookmarks Bookmarks { get; }
		int Bold { get; set; }
		int Italic { get; set; }
		WdUnderline Underline { get; set; }
		WdEmphasisMark EmphasisMark { get; set; }
		bool DisableCharacterSpaceGrid { get; set; }
		Revisions Revisions { get; }
		object Style { get; set; }
		int StoryLength { get; }
		WdLanguageID LanguageID { get; set; }
		SynonymInfo SynonymInfo { get; }
		Hyperlinks Hyperlinks { get; }
		ListParagraphs ListParagraphs { get; }
		Subdocuments Subdocuments { get; }
		bool GrammarChecked { get; set; }
		bool SpellingChecked { get; set; }
		WdColorIndex HighlightColorIndex { get; set; }
		Columns Columns { get; }
		Rows Rows { get; }
		int CanEdit { get; }
		int CanPaste { get; }
		bool IsEndOfRowMark { get; }
		int BookmarkID { get; }
		int PreviousBookmarkID { get; }
		Find Find { get; }
		PageSetup PageSetup { get; set; }
		ShapeRange ShapeRange { get; }
		WdCharacterCase Case { get; set; }
		object this[WdInformation Type] { get; }
		ProofreadingErrors GrammaticalErrors { get; }
		ProofreadingErrors SpellingErrors { get; }
		WdTextOrientation Orientation { get; set; }
		InlineShapes InlineShapes { get; }
		Range NextStoryRange { get; }
		WdLanguageID LanguageIDFarEast { get; set; }
		WdLanguageID LanguageIDOther { get; set; }
		void Select();
		void SetRange(int Start, int End);
		void Collapse(ref object Direction);
		void InsertBefore(string Text);
		void InsertAfter(string Text);
		Range Next(ref object Unit, ref object Count);
		Range Previous(ref object Unit, ref object Count);
		int StartOf(ref object Unit, ref object Extend);
		int EndOf(ref object Unit, ref object Extend);
		int Move(ref object Unit, ref object Count);
		int MoveStart(ref object Unit, ref object Count);
		int MoveEnd(ref object Unit, ref object Count);
		int MoveWhile(ref object Cset, ref object Count);
		int MoveStartWhile(ref object Cset, ref object Count);
		int MoveEndWhile(ref object Cset, ref object Count);
		int MoveUntil(ref object Cset, ref object Count);
		int MoveStartUntil(ref object Cset, ref object Count);
		int MoveEndUntil(ref object Cset, ref object Count);
		void Cut();
		void Copy();
		void Paste();
		void InsertBreak(ref object Type);
		void InsertFile(string FileName, ref object Range, ref object ConfirmConversions, ref object Link, ref object Attachment);
		bool InStory(Range Range);
		bool InRange(Range Range);
		int Delete(ref object Unit, ref object Count);
		void WholeStory();
		int Expand(ref object Unit);
		void InsertParagraph();
		void InsertParagraphAfter();
		Table ConvertToTableOld(ref object Separator, ref object NumRows, ref object NumColumns, ref object InitialColumnWidth, ref object Format, ref object ApplyBorders, ref object ApplyShading, ref object ApplyFont, ref object ApplyColor, ref object ApplyHeadingRows, ref object ApplyLastRow, ref object ApplyFirstColumn, ref object ApplyLastColumn, ref object AutoFit);
		void InsertDateTimeOld(ref object DateTimeFormat, ref object InsertAsField, ref object InsertAsFullWidth);
		void InsertSymbol(int CharacterNumber, ref object Font, ref object Unicode, ref object Bias);
		void InsertCrossReference_2002(ref object ReferenceType, WdReferenceKind ReferenceKind, ref object ReferenceItem, ref object InsertAsHyperlink, ref object IncludePosition);
		void InsertCaptionXP(ref object Label, ref object Title, ref object TitleAutoText, ref object Position);
		void CopyAsPicture();
		void SortOld(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object SortColumn, ref object Separator, ref object CaseSensitive, ref object LanguageID);
		void SortAscending();
		void SortDescending();
		bool IsEqual(Range Range);
		float Calculate();
		Range GoTo(ref object What, ref object Which, ref object Count, ref object Name);
		Range GoToNext(WdGoToItem What);
		Range GoToPrevious(WdGoToItem What);
		void PasteSpecial(ref object IconIndex, ref object Link, ref object Placement, ref object DisplayAsIcon, ref object DataType, ref object IconFileName, ref object IconLabel);
		void LookupNameProperties();
		int ComputeStatistics(WdStatistic Statistic);
		void Relocate(int Direction);
		void CheckSynonyms();
		void SubscribeTo(string Edition, ref object Format);
		void CreatePublisher(ref object Edition, ref object ContainsPICT, ref object ContainsRTF, ref object ContainsText);
		void InsertAutoText();
		void InsertDatabase(ref object Format, ref object Style, ref object LinkToSource, ref object Connection, ref object SQLStatement, ref object SQLStatement1, ref object PasswordDocument, ref object PasswordTemplate, ref object WritePasswordDocument, ref object WritePasswordTemplate, ref object DataSource, ref object From, ref object To, ref object IncludeFields);
		void AutoFormat();
		void CheckGrammar();
		void CheckSpelling(ref object CustomDictionary, ref object IgnoreUppercase, ref object AlwaysSuggest, ref object CustomDictionary2, ref object CustomDictionary3, ref object CustomDictionary4, ref object CustomDictionary5, ref object CustomDictionary6, ref object CustomDictionary7, ref object CustomDictionary8, ref object CustomDictionary9, ref object CustomDictionary10);
		void InsertParagraphBefore();
		void NextSubdocument();
		void PreviousSubdocument();
		void ConvertHangulAndHanja(ref object ConversionsMode, ref object FastConversion, ref object CheckHangulEnding, ref object EnableRecentOrdering, ref object CustomDictionary);
		void PasteAsNestedTable();
		void ModifyEnclosure(ref object Style, ref object Symbol, ref object EnclosedText);
		void PhoneticGuide(string Text, WdPhoneticGuideAlignmentType Alignment, int Raise, int FontSize, string FontName);
		void InsertDateTime(ref object DateTimeFormat, ref object InsertAsField, ref object InsertAsFullWidth, ref object DateLanguage, ref object CalendarType);
		void Sort(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object SortColumn, ref object Separator, ref object CaseSensitive, ref object BidiSort, ref object IgnoreThe, ref object IgnoreKashida, ref object IgnoreDiacritics, ref object IgnoreHe, ref object LanguageID);
		void DetectLanguage();
		Table ConvertToTable(ref object Separator, ref object NumRows, ref object NumColumns, ref object InitialColumnWidth, ref object Format, ref object ApplyBorders, ref object ApplyShading, ref object ApplyFont, ref object ApplyColor, ref object ApplyHeadingRows, ref object ApplyLastRow, ref object ApplyFirstColumn, ref object ApplyLastColumn, ref object AutoFit, ref object AutoFitBehavior, ref object DefaultTableBehavior);
		void TCSCConverter(WdTCSCConverterDirection WdTCSCConverterDirection, bool CommonTerms, bool UseVariants);
		bool LanguageDetected { get; set; }
		float FitTextWidth { get; set; }
		WdHorizontalInVerticalType HorizontalInVertical { get; set; }
		WdTwoLinesInOneType TwoLinesInOne { get; set; }
		bool CombineCharacters { get; set; }
		int NoProofing { get; set; }
		Tables TopLevelTables { get; }
		Scripts Scripts { get; }
		WdCharacterWidth CharacterWidth { get; set; }
		WdKana Kana { get; set; }
		int BoldBi { get; set; }
		int ItalicBi { get; set; }
		string ID { get; set; }
		SmartTags SmartTags { get; }
		bool ShowAll { get; set; }
		Document Document { get; }
		FootnoteOptions FootnoteOptions { get; }
		EndnoteOptions EndnoteOptions { get; }
		void PasteAndFormat(WdRecoveryType Type);
		void PasteExcelTable(bool LinkedToExcel, bool WordFormatting, bool RTF);
		void PasteAppendTable();
		XMLNodes XMLNodes { get; }
		XMLNode XMLParentNode { get; }
		string this[bool DataOnly] { get; }
		object EnhMetaFileBits { get; }
		Range GoToEditableRange(ref object EditorID);
		void InsertXML(string XML, ref object Transform);
		void InsertCaption(ref object Label, ref object Title, ref object TitleAutoText, ref object Position, ref object ExcludeLabel);
		void InsertCrossReference(ref object ReferenceType, WdReferenceKind ReferenceKind, ref object ReferenceItem, ref object InsertAsHyperlink, ref object IncludePosition, ref object SeparateNumbers, ref object SeparatorString);
		object CharacterStyle { get; }
		object ParagraphStyle { get; }
		object ListStyle { get; }
		object TableStyle { get; }
		ContentControls ContentControls { get; }
		void ExportFragment(string FileName, WdSaveFormat Format);
		string WordOpenXML { get; }
		void SetListLevel(short Level);
		void InsertAlignmentTab(int Alignment, int RelativeTo);
		ContentControl ParentContentControl { get; }
		void ImportFragment(string FileName, bool MatchDestination);
		void ExportAsFixedFormat(string OutputFileName, WdExportFormat ExportFormat, bool OpenAfterExport, WdExportOptimizeFor OptimizeFor, bool ExportCurrentPage, WdExportItem Item, bool IncludeDocProps, bool KeepIRM, WdExportCreateBookmarks CreateBookmarks, bool DocStructureTags, bool BitmapMissingFonts, bool UseISO19005_1, ref object FixedFormatExtClassPtr);
	}
	#endregion
	#region WdLanguageID
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLanguageID {
		wdAfrikaans = 0x436,
		wdAlbanian = 0x41c,
		wdAmharic = 0x45e,
		wdArabic = 0x401,
		wdArabicAlgeria = 0x1401,
		wdArabicBahrain = 0x3c01,
		wdArabicEgypt = 0xc01,
		wdArabicIraq = 0x801,
		wdArabicJordan = 0x2c01,
		wdArabicKuwait = 0x3401,
		wdArabicLebanon = 0x3001,
		wdArabicLibya = 0x1001,
		wdArabicMorocco = 0x1801,
		wdArabicOman = 0x2001,
		wdArabicQatar = 0x4001,
		wdArabicSyria = 0x2801,
		wdArabicTunisia = 0x1c01,
		wdArabicUAE = 0x3801,
		wdArabicYemen = 0x2401,
		wdArmenian = 0x42b,
		wdAssamese = 0x44d,
		wdAzeriCyrillic = 0x82c,
		wdAzeriLatin = 0x42c,
		wdBasque = 0x42d,
		wdBelgianDutch = 0x813,
		wdBelgianFrench = 0x80c,
		wdBengali = 0x445,
		wdBulgarian = 0x402,
		wdBurmese = 0x455,
		wdByelorussian = 0x423,
		wdCatalan = 0x403,
		wdCherokee = 0x45c,
		wdChineseHongKongSAR = 0xc04,
		wdChineseMacaoSAR = 0x1404,
		wdChineseSingapore = 0x1004,
		wdCroatian = 0x41a,
		wdCzech = 0x405,
		wdDanish = 0x406,
		wdDivehi = 0x465,
		wdDutch = 0x413,
		wdEdo = 0x466,
		wdEnglishAUS = 0xc09,
		wdEnglishBelize = 0x2809,
		wdEnglishCanadian = 0x1009,
		wdEnglishCaribbean = 0x2409,
		wdEnglishIndonesia = 0x3809,
		wdEnglishIreland = 0x1809,
		wdEnglishJamaica = 0x2009,
		wdEnglishNewZealand = 0x1409,
		wdEnglishPhilippines = 0x3409,
		wdEnglishSouthAfrica = 0x1c09,
		wdEnglishTrinidadTobago = 0x2c09,
		wdEnglishUK = 0x809,
		wdEnglishUS = 0x409,
		wdEnglishZimbabwe = 0x3009,
		wdEstonian = 0x425,
		wdFaeroese = 0x438,
		wdFilipino = 0x464,
		wdFinnish = 0x40b,
		wdFrench = 0x40c,
		wdFrenchCameroon = 0x2c0c,
		wdFrenchCanadian = 0xc0c,
		wdFrenchCongoDRC = 0x240c,
		wdFrenchCotedIvoire = 0x300c,
		wdFrenchHaiti = 0x3c0c,
		wdFrenchLuxembourg = 0x140c,
		wdFrenchMali = 0x340c,
		wdFrenchMonaco = 0x180c,
		wdFrenchMorocco = 0x380c,
		wdFrenchReunion = 0x200c,
		wdFrenchSenegal = 0x280c,
		wdFrenchWestIndies = 0x1c0c,
		wdFrisianNetherlands = 0x462,
		wdFulfulde = 0x467,
		wdGaelicIreland = 0x83c,
		wdGaelicScotland = 0x43c,
		wdGalician = 0x456,
		wdGeorgian = 0x437,
		wdGerman = 0x407,
		wdGermanAustria = 0xc07,
		wdGermanLiechtenstein = 0x1407,
		wdGermanLuxembourg = 0x1007,
		wdGreek = 0x408,
		wdGuarani = 0x474,
		wdGujarati = 0x447,
		wdHausa = 0x468,
		wdHawaiian = 0x475,
		wdHebrew = 0x40d,
		wdHindi = 0x439,
		wdHungarian = 0x40e,
		wdIbibio = 0x469,
		wdIcelandic = 0x40f,
		wdIgbo = 0x470,
		wdIndonesian = 0x421,
		wdInuktitut = 0x45d,
		wdItalian = 0x410,
		wdJapanese = 0x411,
		wdKannada = 0x44b,
		wdKanuri = 0x471,
		wdKashmiri = 0x460,
		wdKazakh = 0x43f,
		wdKhmer = 0x453,
		wdKirghiz = 0x440,
		wdKonkani = 0x457,
		wdKorean = 0x412,
		wdKyrgyz = 0x440,
		wdLanguageNone = 0,
		wdLao = 0x454,
		wdLatin = 0x476,
		wdLatvian = 0x426,
		wdLithuanian = 0x427,
		wdMacedonianFYROM = 0x42f,
		wdMalayalam = 0x44c,
		wdMalayBruneiDarussalam = 0x83e,
		wdMalaysian = 0x43e,
		wdMaltese = 0x43a,
		wdManipuri = 0x458,
		wdMarathi = 0x44e,
		wdMexicanSpanish = 0x80a,
		wdMongolian = 0x450,
		wdNepali = 0x461,
		wdNoProofing = 0x400,
		wdNorwegianBokmol = 0x414,
		wdNorwegianNynorsk = 0x814,
		wdOriya = 0x448,
		wdOromo = 0x472,
		wdPashto = 0x463,
		wdPersian = 0x429,
		wdPolish = 0x415,
		wdPortuguese = 0x816,
		wdPortugueseBrazil = 0x416,
		wdPunjabi = 0x446,
		wdRhaetoRomanic = 0x417,
		wdRomanian = 0x418,
		wdRomanianMoldova = 0x818,
		wdRussian = 0x419,
		wdRussianMoldova = 0x819,
		wdSamiLappish = 0x43b,
		wdSanskrit = 0x44f,
		wdSerbianCyrillic = 0xc1a,
		wdSerbianLatin = 0x81a,
		wdSesotho = 0x430,
		wdSimplifiedChinese = 0x804,
		wdSindhi = 0x459,
		wdSindhiPakistan = 0x859,
		wdSinhalese = 0x45b,
		wdSlovak = 0x41b,
		wdSlovenian = 0x424,
		wdSomali = 0x477,
		wdSorbian = 0x42e,
		wdSpanish = 0x40a,
		wdSpanishArgentina = 0x2c0a,
		wdSpanishBolivia = 0x400a,
		wdSpanishChile = 0x340a,
		wdSpanishColombia = 0x240a,
		wdSpanishCostaRica = 0x140a,
		wdSpanishDominicanRepublic = 0x1c0a,
		wdSpanishEcuador = 0x300a,
		wdSpanishElSalvador = 0x440a,
		wdSpanishGuatemala = 0x100a,
		wdSpanishHonduras = 0x480a,
		wdSpanishModernSort = 0xc0a,
		wdSpanishNicaragua = 0x4c0a,
		wdSpanishPanama = 0x180a,
		wdSpanishParaguay = 0x3c0a,
		wdSpanishPeru = 0x280a,
		wdSpanishPuertoRico = 0x500a,
		wdSpanishUruguay = 0x380a,
		wdSpanishVenezuela = 0x200a,
		wdSutu = 0x430,
		wdSwahili = 0x441,
		wdSwedish = 0x41d,
		wdSwedishFinland = 0x81d,
		wdSwissFrench = 0x100c,
		wdSwissGerman = 0x807,
		wdSwissItalian = 0x810,
		wdSyriac = 0x45a,
		wdTajik = 0x428,
		wdTamazight = 0x45f,
		wdTamazightLatin = 0x85f,
		wdTamil = 0x449,
		wdTatar = 0x444,
		wdTelugu = 0x44a,
		wdThai = 0x41e,
		wdTibetan = 0x451,
		wdTigrignaEritrea = 0x873,
		wdTigrignaEthiopic = 0x473,
		wdTraditionalChinese = 0x404,
		wdTsonga = 0x431,
		wdTswana = 0x432,
		wdTurkish = 0x41f,
		wdTurkmen = 0x442,
		wdUkrainian = 0x422,
		wdUrdu = 0x420,
		wdUzbekCyrillic = 0x843,
		wdUzbekLatin = 0x443,
		wdVenda = 0x433,
		wdVietnamese = 0x42a,
		wdWelsh = 0x452,
		wdXhosa = 0x434,
		wdYi = 0x478,
		wdYiddish = 0x43d,
		wdYoruba = 0x46a,
		wdZulu = 0x435
	}
	#endregion
	#region WdTextOrientation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTextOrientation {
		wdTextOrientationHorizontal,
		wdTextOrientationVerticalFarEast,
		wdTextOrientationUpward,
		wdTextOrientationDownward,
		wdTextOrientationHorizontalRotatedFarEast,
		wdTextOrientationVertical
	}
	#endregion
	#region WdCharacterCase
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCharacterCase {
		wdFullWidth = 7,
		wdHalfWidth = 6,
		wdHiragana = 9,
		wdKatakana = 8,
		wdLowerCase = 0,
		wdNextCase = -1,
		wdTitleSentence = 4,
		wdTitleWord = 2,
		wdToggleCase = 5,
		wdUpperCase = 1
	}
	#endregion
	#region WdStoryType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdStoryType {
		wdCommentsStory = 4,
		wdEndnoteContinuationNoticeStory = 0x11,
		wdEndnoteContinuationSeparatorStory = 0x10,
		wdEndnoteSeparatorStory = 15,
		wdEndnotesStory = 3,
		wdEvenPagesFooterStory = 8,
		wdEvenPagesHeaderStory = 6,
		wdFirstPageFooterStory = 11,
		wdFirstPageHeaderStory = 10,
		wdFootnoteContinuationNoticeStory = 14,
		wdFootnoteContinuationSeparatorStory = 13,
		wdFootnoteSeparatorStory = 12,
		wdFootnotesStory = 2,
		wdMainTextStory = 1,
		wdPrimaryFooterStory = 9,
		wdPrimaryHeaderStory = 7,
		wdTextFrameStory = 5
	}
	#endregion
	#region WdInformation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdInformation {
		wdActiveEndAdjustedPageNumber = 1,
		wdActiveEndPageNumber = 3,
		wdActiveEndSectionNumber = 2,
		wdAtEndOfRowMarker = 0x1f,
		wdCapsLock = 0x15,
		wdEndOfRangeColumnNumber = 0x11,
		wdEndOfRangeRowNumber = 14,
		wdFirstCharacterColumnNumber = 9,
		wdFirstCharacterLineNumber = 10,
		wdFrameIsSelected = 11,
		wdHeaderFooterType = 0x21,
		wdHorizontalPositionRelativeToPage = 5,
		wdHorizontalPositionRelativeToTextBoundary = 7,
		wdInClipboard = 0x26,
		wdInCommentPane = 0x1a,
		wdInEndnote = 0x24,
		wdInFootnote = 0x23,
		wdInFootnoteEndnotePane = 0x19,
		wdInHeaderFooter = 0x1c,
		wdInMasterDocument = 0x22,
		wdInWordMail = 0x25,
		wdMaximumNumberOfColumns = 0x12,
		wdMaximumNumberOfRows = 15,
		wdNumberOfPagesInDocument = 4,
		wdNumLock = 0x16,
		wdOverType = 0x17,
		wdReferenceOfType = 0x20,
		wdRevisionMarking = 0x18,
		wdSelectionMode = 20,
		wdStartOfRangeColumnNumber = 0x10,
		wdStartOfRangeRowNumber = 13,
		wdVerticalPositionRelativeToPage = 6,
		wdVerticalPositionRelativeToTextBoundary = 8,
		wdWithInTable = 12,
		wdZoomPercentage = 0x13
	}
	#endregion
	#region WdReferenceKind
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdReferenceKind {
		wdContentText = -1,
		wdEndnoteNumber = 6,
		wdEndnoteNumberFormatted = 0x11,
		wdEntireCaption = 2,
		wdFootnoteNumber = 5,
		wdFootnoteNumberFormatted = 0x10,
		wdNumberFullContext = -4,
		wdNumberNoContext = -3,
		wdNumberRelativeContext = -2,
		wdOnlyCaptionText = 4,
		wdOnlyLabelAndNumber = 3,
		wdPageNumber = 7,
		wdPosition = 15
	}
	#endregion
	#region WdGoToItem
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdGoToItem {
		wdGoToBookmark = -1,
		wdGoToComment = 6,
		wdGoToEndnote = 5,
		wdGoToEquation = 10,
		wdGoToField = 7,
		wdGoToFootnote = 4,
		wdGoToGrammaticalError = 14,
		wdGoToGraphic = 8,
		wdGoToHeading = 11,
		wdGoToLine = 3,
		wdGoToObject = 9,
		wdGoToPage = 1,
		wdGoToPercent = 12,
		wdGoToProofreadingError = 15,
		wdGoToSection = 0,
		wdGoToSpellingError = 13,
		wdGoToTable = 2
	}
	#endregion
	#region WdPhoneticGuideAlignmentType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPhoneticGuideAlignmentType {
		wdPhoneticGuideAlignmentCenter,
		wdPhoneticGuideAlignmentZeroOneZero,
		wdPhoneticGuideAlignmentOneTwoOne,
		wdPhoneticGuideAlignmentLeft,
		wdPhoneticGuideAlignmentRight,
		wdPhoneticGuideAlignmentRightVertical
	}
	#endregion
	#region WdTwoLinesInOneType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTwoLinesInOneType {
		wdTwoLinesInOneNone,
		wdTwoLinesInOneNoBrackets,
		wdTwoLinesInOneParentheses,
		wdTwoLinesInOneSquareBrackets,
		wdTwoLinesInOneAngleBrackets,
		wdTwoLinesInOneCurlyBrackets
	}
	#endregion
	#region WdTCSCConverterDirection
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTCSCConverterDirection {
		wdTCSCConverterDirectionSCTC,
		wdTCSCConverterDirectionTCSC,
		wdTCSCConverterDirectionAuto
	}
	#endregion
	#region WdHorizontalInVerticalType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdHorizontalInVerticalType {
		wdHorizontalInVerticalNone,
		wdHorizontalInVerticalFitInLine,
		wdHorizontalInVerticalResizeLine
	}
	#endregion
	#region WdCharacterWidth
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCharacterWidth {
		wdWidthFullWidth = 7,
		wdWidthHalfWidth = 6
	}
	#endregion
	#region WdKana
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdKana {
		wdKanaHiragana = 9,
		wdKanaKatakana = 8
	}
	#endregion
	#region WdRecoveryType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRecoveryType {
		wdChart = 14,
		wdChartLinked = 15,
		wdChartPicture = 13,
		wdFormatOriginalFormatting = 0x10,
		wdFormatPlainText = 0x16,
		wdFormatSurroundingFormattingWithEmphasis = 20,
		wdListCombineWithExistingList = 0x18,
		wdListContinueNumbering = 7,
		wdListDontMerge = 0x19,
		wdListRestartNumbering = 8,
		wdPasteDefault = 0,
		wdSingleCellTable = 6,
		wdSingleCellText = 5,
		wdTableAppendTable = 10,
		wdTableInsertAsRows = 11,
		wdTableOriginalFormatting = 12,
		wdTableOverwriteCells = 0x17,
		wdUseDestinationStylesRecovery = 0x13
	}
	#endregion
	#region WdSaveFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSaveFormat {
		wdFormatDocument = 0,
		wdFormatDocument97 = 0,
		wdFormatDocumentDefault = 0x10,
		wdFormatDOSText = 4,
		wdFormatDOSTextLineBreaks = 5,
		wdFormatEncodedText = 7,
		wdFormatFilteredHTML = 10,
		wdFormatFlatXML = 0x13,
		wdFormatFlatXMLMacroEnabled = 20,
		wdFormatFlatXMLTemplate = 0x15,
		wdFormatFlatXMLTemplateMacroEnabled = 0x16,
		wdFormatHTML = 8,
		wdFormatPDF = 0x11,
		wdFormatRTF = 6,
		wdFormatTemplate = 1,
		wdFormatTemplate97 = 1,
		wdFormatText = 2,
		wdFormatTextLineBreaks = 3,
		wdFormatUnicodeText = 7,
		wdFormatWebArchive = 9,
		wdFormatXML = 11,
		wdFormatXMLDocument = 12,
		wdFormatXMLDocumentMacroEnabled = 13,
		wdFormatXMLTemplate = 14,
		wdFormatXMLTemplateMacroEnabled = 15,
		wdFormatXPS = 0x12
	}
	#endregion
	#region WdExportFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdExportFormat {
		wdExportFormatPDF = 0x11,
		wdExportFormatXPS = 0x12
	}
	#endregion
	#region WdExportOptimizeFor
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdExportOptimizeFor {
		wdExportOptimizeForPrint,
		wdExportOptimizeForOnScreen
	}
	#endregion
	#region WdExportCreateBookmarks
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdExportCreateBookmarks {
		wdExportCreateNoBookmarks,
		wdExportCreateHeadingBookmarks,
		wdExportCreateWordBookmarks
	}
	#endregion
	#region WdExportItem
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdExportItem {
		wdExportDocumentContent = 0,
		wdExportDocumentWithMarkup = 7
	}
	#endregion
	#region WdExportRange
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdExportRange {
		wdExportAllDocument,
		wdExportSelection,
		wdExportCurrentPage,
		wdExportFromTo
	}
	#endregion
}
