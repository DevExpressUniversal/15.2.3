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
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfExportSR
	public static class RtfExportSR {
		public static readonly string OpenGroup = @"{";
		public static readonly string CloseGroup = @"}";
		public static readonly string RtfSignature = @"\rtf1";
		public static readonly string DefaultFontIndex = @"\deff";
		public static readonly string StyleTable = @"\stylesheet";
		public static readonly string ColorTable = @"\colortbl";
		public static readonly string FontTable = @"\fonttbl";
		public static readonly string FontCharset = @"\fcharset";
		public static readonly string UserTable = @"\*\protusertbl";
		public static readonly string DocumentInformation = @"\info";
		public static readonly string Password = @"\*\password";
		public static readonly string PasswordHash = @"\*\passwordhash";
		public static readonly string EnforceProtection = @"\enforceprot";
		public static readonly string AnnotationProtection = @"\annotprot";
		public static readonly string ReadOnlyProtection = @"\readprot";
		public static readonly string ProtectionLevel = @"\protlevel";
		public static readonly string NoUICompatible = @"\nouicompat"; 
		public static readonly string DefaultTabWidth = @"\deftab";
		public static readonly string HyphenateDocument = @"\hyphauto";
		public static readonly string PageFacing = @"\facingp";
		public static readonly string DisplayBackgroundShape = @"\viewbksp";
		public static readonly string ColorRed = @"\red";
		public static readonly string ColorGreen = @"\green";
		public static readonly string ColorBlue = @"\blue";
		public static readonly string ResetParagraphProperties = @"\pard";
		public static readonly string FrameHorizontalPosition = @"\posx";
		public static readonly string FrameVerticalPosition = @"\posy";
		public static readonly string FrameWidth = @"\absw";
		public static readonly string FrameHeight = @"\absh";
		public static readonly string FrameNoWrap = @"\nowrap";
		public static readonly string FrameWrapOverlay = @"\overlay";
		public static readonly string FrameWrapDefault = @"\wrapdefault";
		public static readonly string FrameWrapAround = @"\wraparound";
		public static readonly string FrameWrapTight = @"\wraptight";
		public static readonly string FrameWrapThrough = @"\wrapthrough";
		public static readonly string TopParagraphBorder = @"\brdrt";
		public static readonly string BottomParagraphBorder = @"\brdrb";
		public static readonly string LeftParagraphBorder = @"\brdrl";
		public static readonly string RightParagraphBorder = @"\brdrr";
		public static readonly string ParagraphHorizontalPositionTypeMargin = @"\phmrg";
		public static readonly string ParagraphHorizontalPositionTypePage = @"\phpg";
		public static readonly string ParagraphHorizontalPositionTypeColumn = @"\phcol";
		public static readonly string ParagraphVerticalPositionTypeMargin = @"\pvmrg";
		public static readonly string ParagraphVerticalPositionTypePage = @"\pvpg";
		public static readonly string ParagraphVerticalPositionTypeLine = @"\pvpara";
		public static readonly string ResetCharacterFormatting = @"\plain";
		public static readonly string EndOfParagraph = @"\par";
		public static readonly string LeftAlignment = @"\ql";
		public static readonly string RightAlignment = @"\qr";
		public static readonly string CenterAlignment = @"\qc";
		public static readonly string JustifyAlignment = @"\qj";
		public static readonly string FirstLineIndentInTwips = @"\fi";
		public static readonly string LeftIndentInTwips = @"\li";
		public static readonly string LeftIndentInTwips_Lin = @"\lin";
		public static readonly string RightIndentInTwips = @"\ri";
		public static readonly string RightIndentInTwips_Rin = @"\rin";
		public static readonly string AutomaticParagraphHyphenation = @"\hyphpar";
		public static readonly string SuppressLineNumbering = @"\noline";
		public static readonly string ContextualSpacing = @"\contextualspace";
		public static readonly string PageBreakBefore = @"\pagebb";
		public static readonly string BeforeAutoSpacing = @"\sbauto";
		public static readonly string AfterAutoSpacing = @"\saauto";
		public static readonly string KeepWithNext = @"\keepn";
		public static readonly string KeepLinesTogether = @"\keep";
		public static readonly string WidowOrphanControlOn = @"\widctlpar";
		public static readonly string WidowOrphanControlOff = @"\nowidctlpar";
		public static readonly string OutlineLevel = @"\outlinelevel";
		public static readonly string ParagraphBackgroundColor = @"\cbpat";
		public static readonly string RtfLineSpacingValue = @"\sl";
		public static readonly string RtfLineSpacingMultiple = @"\slmult";
		public static readonly string SpaceBefore = @"\sb";
		public static readonly string SpaceAfter = @"\sa";
		public static readonly string ListIndex = @"\ls";
		public static readonly string LevelIndex = @"\ilvl";
		public static readonly string AlternativeText = @"\listtext";
		public static readonly string ParagraphNumerationText = @"\*\pntext";
		public static readonly string CenteredTab = @"\tqc";
		public static readonly string DecimalTab = @"\tqdec";
		public static readonly string FlushRightTab = @"\tqr";
		public static readonly string TabLeaderDots = @"\tldot";
		public static readonly string TabLeaderEqualSign = @"\tleq";
		public static readonly string TabLeaderHyphens = @"\tlhyph";
		public static readonly string TabLeaderMiddleDots = @"\tlmdot";
		public static readonly string TabLeaderThickLine = @"\tlth";
		public static readonly string TabLeaderUnderline = @"\tlul";
		public static readonly string TabPosition = @"\tx";
		public static readonly string AllCapitals = @"\caps";
		public static readonly string LangInfo = @"\lang";
		public static readonly string LangInfo1 = @"\langfe";
		public static readonly string LangInfo2 = @"\langnp";
		public static readonly string LangInfo3 = @"\langfenp";
		public static readonly string NoProof = @"\noproof";
		public static readonly string HiddenText = @"\v";
		public static readonly string FontBold = @"\b";
		public static readonly string FontItalic = @"\i";
		public static readonly string FontStrikeout = @"\strike";
		public static readonly string FontDoubleStrikeout = @"\striked1";
		public static readonly string FontUnderline = @"\ul";
		public static readonly string FontUnderlineDotted = @"\uld";
		public static readonly string FontUnderlineDashed = @"\uldash";
		public static readonly string FontUnderlineDashDotted = @"\uldashd";
		public static readonly string FontUnderlineDashDotDotted = @"\uldashdd";
		public static readonly string FontUnderlineDouble = @"\uldb";
		public static readonly string FontUnderlineHeavyWave = @"\ulhwave";
		public static readonly string FontUnderlineLongDashed = @"\ulldash";
		public static readonly string FontUnderlineThickSingle = @"\ulth";
		public static readonly string FontUnderlineThickDotted = @"\ulthd";
		public static readonly string FontUnderlineThickDashed = @"\ulthdash";
		public static readonly string FontUnderlineThickDashDotted = @"\ulthdashd";
		public static readonly string FontUnderlineThickDashDotDotted = @"\ulthdashdd";
		public static readonly string FontUnderlineThickLongDashed = @"\ulthldash";
		public static readonly string FontUnderlineDoubleWave = @"\ululdbwave";
		public static readonly string FontUnderlineWave = @"\ulwave";
		public static readonly string FontUnderlineWordsOnly = @"\ulw";
		public static readonly string FontNumber = @"\f";
		public static readonly string FontSize = @"\fs";
		public static readonly string RunBackgroundColor = @"\chcbpat";
		public static readonly string RunBackgroundColor2 = @"\highlight";
		public static readonly string RunForegroundColor = @"\cf";
		public static readonly string RunUnderlineColor = @"\ulc";
		public static readonly string RunSuperScript = @"\super";
		public static readonly string RunSubScript = @"\sub";
		public static readonly string Picture = @"\pict";
		public static readonly string PictureWidth = @"\picw";
		public static readonly string PictureHeight = @"\pich";
		public static readonly string PictureDesiredWidth = @"\picwgoal";
		public static readonly string PictureDesiredHeight = @"\pichgoal";
		public static readonly string PictureScaleX = @"\picscalex";
		public static readonly string PictureScaleY = @"\picscaley";
		public static readonly string ShapePicture = @"\*\shppict";
		public static readonly string NonShapePicture = @"\nonshppict";
		public static readonly string DxImageUri = @"\*\dximageuri";
		public static readonly string Space = " ";
		public static readonly string CLRF = "\r\n";
		public static readonly string ResetSectionProperties = @"\sectd";
		public static readonly string SectionEndMark = @"\sect";
		public static readonly string SectionMarginsLeft = @"\marglsxn";
		public static readonly string SectionMarginsRight = @"\margrsxn";
		public static readonly string SectionMarginsTop = @"\margtsxn";
		public static readonly string SectionMarginsBottom = @"\margbsxn";
		public static readonly string SectionMarginsHeaderOffset = @"\headery";
		public static readonly string SectionMarginsFooterOffset = @"\footery";
		public static readonly string SectionMarginsGutter = @"\guttersxn";
		public static readonly string SectionFirstPageHeader = @"\headerf";
		public static readonly string SectionOddPageHeader = @"\headerr";
		public static readonly string SectionEvenPageHeader = @"\headerl";
		public static readonly string SectionFirstPageFooter = @"\footerf";
		public static readonly string SectionOddPageFooter = @"\footerr";
		public static readonly string SectionEvenPageFooter = @"\footerl";
		public static readonly string SectionPageWidth = @"\pgwsxn";
		public static readonly string SectionPageHeight = @"\pghsxn";
		public static readonly string SectionPageLandscape = @"\lndscpsxn";
		public static readonly string PaperKind = @"\psz";
		public static readonly string SectionFirstPagePaperSource = @"\binfsxn";
		public static readonly string SectionOtherPagePaperSource = @"\binsxn";
		public static readonly string SectionOnlyAllowEditingOfFormFields = @"\sectunlocked";
		public static readonly string SectionTextFlow = @"\stextflow";
		public static readonly string SectionTitlePage = @"\titlepg";
		public static readonly string VerticalAlignmentBottom = @"\vertal\vertalb";
		public static readonly string VerticalAlignmentTop = @"\vertalt";
		public static readonly string VerticalAlignmentCenter = @"\vertalc";
		public static readonly string VerticalAlignmentJustify = @"\vertalj";
		public static readonly string SectionBreakTypeNextPage = @"\sbkpage";
		public static readonly string SectionBreakTypeOddPage = @"\sbkodd";
		public static readonly string SectionBreakTypeEvenPage = @"\sbkeven";
		public static readonly string SectionBreakTypeColumn = @"\sbkcol";
		public static readonly string SectionBreakTypeContinuous = @"\sbknone";
		public static readonly string SectionChapterSeparatorHyphen = @"\pgnhnsh";
		public static readonly string SectionChapterSeparatorPeriod = @"\pgnhnsp";
		public static readonly string SectionChapterSeparatorColon = @"\pgnhnsc";
		public static readonly string SectionChapterSeparatorEmDash = @"\pgnhnsm";
		public static readonly string SectionChapterSeparatorEnDash = @"\pgnhnsn";
		public static readonly string SectionChapterHeaderStyle = @"\pgnhn";
		public static readonly string SectionPageNumberingStart = @"\pgnstarts";
		public static readonly string SectionPageNumberingContinuous = @"\pgncont";
		public static readonly string SectionPageNumberingRestart = @"\pgnrestart";
		public static readonly string SectionPageNumberingDecimal = @"\pgndec";
		public static readonly string SectionPageNumberingUpperRoman = @"\pgnucrm";
		public static readonly string SectionPageNumberingLowerRoman = @"\pgnlcrm";
		public static readonly string SectionPageNumberingUpperLetter = @"\pgnucltr";
		public static readonly string SectionPageNumberingLowerLetter = @"\pgnlcltr";
		public static readonly string SectionPageNumberingArabicAbjad = @"\pgnbidia";
		public static readonly string SectionPageNumberingArabicAlpha = @"\pgnbidib";
		public static readonly string SectionPageNumberingChosung = @"\pgnchosung";
		public static readonly string SectionPageNumberingDecimalEnclosedCircle = @"\pgncnum";
		public static readonly string SectionPageNumberingDecimalFullWidth = @"\pgndecd";
		public static readonly string SectionPageNumberingGanada = @"\pgnganada";
		public static readonly string SectionPageNumberingHindiVowels = @"\pgnhindia";
		public static readonly string SectionPageNumberingHindiConsonants = @"\pgnhindib";
		public static readonly string SectionPageNumberingHindiNumbers = @"\pgnhindic";
		public static readonly string SectionPageNumberingHindiDescriptive = @"\pgnhindid";
		public static readonly string SectionPageNumberingThaiLetters = @"\pgnthaia";
		public static readonly string SectionPageNumberingThaiNumbers = @"\pgnthaib";
		public static readonly string SectionPageNumberingThaiDescriptive = @"\pgnthaic";
		public static readonly string SectionPageNumberingVietnameseDescriptive = @"\pgnvieta";
		public static readonly string SectionLineNumberingContinuous = @"\linecont";
		public static readonly string SectionLineNumberingStartingLineNumber = @"\linestarts";
		public static readonly string SectionLineNumberingRestartNewPage = @"\lineppage";
		public static readonly string SectionLineNumberingRestartNewSection = @"\linerestart";
		public static readonly string SectionLineNumberingStep = @"\linemod";
		public static readonly string SectionLineNumberingDistance = @"\linex";
		public static readonly string SectionColumnsCount = @"\cols";
		public static readonly string SectionSpaceBetweenColumns = @"\colsx";
		public static readonly string SectionColumnsDrawVerticalSeparator = @"\linebetcol";
		public static readonly string SectionColumnNumber = @"\colno";
		public static readonly string SectionColumnWidth = @"\colw";
		public static readonly string SectionColumnSpace = @"\colsr";
		public static readonly string SectionFootNotePlacementBelowText = @"\sftntj";
		public static readonly string SectionFootNotePlacementPageBottom = @"\sftnbj";
		public static readonly string SectionFootNoteNumberingStart = @"\sftnstart";
		public static readonly string SectionFootNoteNumberingRestartEachPage = @"\sftnrstpg";
		public static readonly string SectionFootNoteNumberingRestartEachSection = @"\sftnrestart";
		public static readonly string SectionFootNoteNumberingRestartContinuous = @"\sftnrstcont";
		public static readonly string SectionFootNoteNumberingFormatDecimal = @"\sftnnar";
		public static readonly string SectionFootNoteNumberingFormatUpperRoman = @"\sftnnruc";
		public static readonly string SectionFootNoteNumberingFormatLowerRoman = @"\sftnnrlc";
		public static readonly string SectionFootNoteNumberingFormatUpperLetter = @"\sftnnauc";
		public static readonly string SectionFootNoteNumberingFormatLowerLetter = @"\sftnnalc";
		public static readonly string SectionFootNoteNumberingFormatChicago = @"\sftnnchi";
		public static readonly string SectionFootNoteNumberingFormatChosung = @"\sftnnchosung";
		public static readonly string SectionFootNoteNumberingFormatDecimalEnclosedCircle = @"\sftnncnum";
		public static readonly string SectionFootNoteNumberingFormatDecimalFullWidth = @"\sftnndbar";
		public static readonly string SectionFootNoteNumberingFormatGanada = @"\sftnnganada";
		public static readonly string SectionEndNoteNumberingStart = @"\saftnstart";
		public static readonly string SectionEndNoteNumberingRestartEachSection = @"\saftnrestart";
		public static readonly string SectionEndNoteNumberingRestartContinuous = @"\saftnrstcont";
		public static readonly string SectionEndNoteNumberingFormatDecimal = @"\saftnnar";
		public static readonly string SectionEndNoteNumberingFormatUpperRoman = @"\saftnnruc";
		public static readonly string SectionEndNoteNumberingFormatLowerRoman = @"\saftnnrlc";
		public static readonly string SectionEndNoteNumberingFormatUpperLetter = @"\saftnnauc";
		public static readonly string SectionEndNoteNumberingFormatLowerLetter = @"\saftnnalc";
		public static readonly string SectionEndNoteNumberingFormatChicago = @"\saftnnchi";
		public static readonly string SectionEndNoteNumberingFormatChosung = @"\saftnnchosung";
		public static readonly string SectionEndNoteNumberingFormatDecimalEnclosedCircle = @"\saftnncnum";
		public static readonly string SectionEndNoteNumberingFormatDecimalFullWidth = @"\saftnndbar";
		public static readonly string SectionEndNoteNumberingFormatGanada = @"\saftnnganada";
		public static readonly string LegacyPaperWidth = @"\paperw";
		public static readonly string LegacyPaperHeight = @"\paperh";
		public static readonly string LegacyLandscape = @"\landscape";
		public static readonly string LegacyPageNumberingStart = @"\pgnstart";
		public static readonly string LegacyMarginsLeft = @"\margl";
		public static readonly string LegacyMarginsRight = @"\margr";
		public static readonly string LegacyMarginsTop = @"\margt";
		public static readonly string LegacyMarginsBottom = @"\margb";
		public static readonly string LegacyMarginsGutter = @"\gutter";
		public static readonly string LegacyMarginsGutterAtRight = @"\rtlgutter";
		public static readonly string FootNotePlacementBelowText = @"\ftntj";
		public static readonly string FootNotePlacementPageBottom = @"\ftnbj";
		public static readonly string FootNoteNumberingStart = @"\ftnstart";
		public static readonly string FootNoteNumberingRestartEachPage = @"\ftnrstpg";
		public static readonly string FootNoteNumberingRestartEachSection = @"\ftnrestart";
		public static readonly string FootNoteNumberingRestartContinuous = @"\ftnrstcont";
		public static readonly string FootNoteNumberingFormatDecimal = @"\ftnnar";
		public static readonly string FootNoteNumberingFormatUpperRoman = @"\ftnnruc";
		public static readonly string FootNoteNumberingFormatLowerRoman = @"\ftnnrlc";
		public static readonly string FootNoteNumberingFormatUpperLetter = @"\ftnnauc";
		public static readonly string FootNoteNumberingFormatLowerLetter = @"\ftnnalc";
		public static readonly string FootNoteNumberingFormatChicago = @"\ftnnchi";
		public static readonly string FootNoteNumberingFormatChosung = @"\ftnnchosung";
		public static readonly string FootNoteNumberingFormatDecimalEnclosedCircle = @"\ftnncnum";
		public static readonly string FootNoteNumberingFormatDecimalFullWidth = @"\ftnndbar";
		public static readonly string FootNoteNumberingFormatGanada = @"\ftnnganada";
		public static readonly string EndNotePlacementEndOfSection = @"\aendnotes";
		public static readonly string EndNotePlacementEndOfDocument = @"\aenddoc";
		public static readonly string EndNoteNumberingStart = @"\aftnstart";
		public static readonly string EndNoteNumberingRestartEachSection = @"\aftnrestart";
		public static readonly string EndNoteNumberingRestartContinuous = @"\aftnrstcont";
		public static readonly string EndNoteNumberingFormatDecimal = @"\aftnnar";
		public static readonly string EndNoteNumberingFormatUpperRoman = @"\aftnnruc";
		public static readonly string EndNoteNumberingFormatLowerRoman = @"\aftnnrlc";
		public static readonly string EndNoteNumberingFormatUpperLetter = @"\aftnnauc";
		public static readonly string EndNoteNumberingFormatLowerLetter = @"\aftnnalc";
		public static readonly string EndNoteNumberingFormatChicago = @"\aftnnchi";
		public static readonly string EndNoteNumberingFormatChosung = @"\aftnnchosung";
		public static readonly string EndNoteNumberingFormatDecimalEnclosedCircle = @"\aftnncnum";
		public static readonly string EndNoteNumberingFormatDecimalFullWidth = @"\aftnndbar";
		public static readonly string EndNoteNumberingFormatGanada = @"\aftnnganada";
		public static readonly string Field = @"\field";
		public static readonly string FieldLocked = @"\fldlock";
		public static readonly string FieldInstructions = @"\*\fldinst";
		public static readonly string FieldResult = @"\fldrslt";
		public static readonly string FieldCodeView = @"\dxfldcodeview";
		public static readonly string FieldMapData = @"\*\mmodsofldmpdata";
		public static readonly string FieldTypeNull = @"\mmfttypenull";
		public static readonly string FieldTypeColumn = @"\mmfttypedbcolumn";
		public static readonly string FieldTypeAddress = @"\mmfttypeaddress";
		public static readonly string FieldTypeSalutation = @"\mmfttypesalutation";
		public static readonly string FieldTypeMapped = @"\mmfttypemapped";
		public static readonly string FieldTypeBarcode = @"\mmfttypebarcode";
		public static readonly string MailMergeDataSourceObjectName = @"\mmodsoname";
		public static readonly string MailMergeDataSourceObjectMappedName = @"\mmodsomappedname";
		public static readonly string MailMergeDataSourceObjectColumnIndex = @"\mmodsofmcolumn";
		public static readonly string MailMergeDataSourceObjectDynamicAddress = @"\mmodsodynaddr";
		public static readonly string MailMergeDataSourceObjectLanguageId = @"\mmodsolid";
		public static readonly string BookmarkStart = @"\*\bkmkstart";
		public static readonly string BookmarkEnd = @"\*\bkmkend";
		public static readonly string RangePermissionStart = @"\*\protstart";
		public static readonly string RangePermissionEnd = @"\*\protend";
		public static readonly string CommentStart = @"\*\atrfstart";
		public static readonly string CommentEnd = @"\*\atrfend";
		public static readonly string CommentId = @"\*\atnid";
		public static readonly string CommentAuthor = @"\*\atnauthor";
		public static readonly string CommentTime = @"\*\atntime";
		public static readonly string CommentChatn = @"\chatn";
		public static readonly string CommentAnnotation = @"\*\annotation";
		public static readonly string CommentDate = @"\*\atndate";
		public static readonly string CommentRef = @"\*\atnref";
		public static readonly string CommentParent = @"\*\atnparent";
		public static readonly string HyperlinkFieldType = @"HYPERLINK";
		public static readonly string DocumentVariable = @"\*\docvar";
		public static readonly string FootNote = @"\footnote";
		public static readonly string FootNoteReference = @"\chftn";
		public static readonly string EndNote = @"\ftnalt";
		public static readonly string PageBackground = @"\*\background";
		public static readonly string Shape = @"\shp";
		public static readonly string ShapeInstance = @"\*\shpinst";
		public static readonly string ShapeText = @"\shptxt";
		public static readonly string ShapeLeft = @"\shpleft";
		public static readonly string ShapeRight = @"\shpright";
		public static readonly string ShapeTop = @"\shptop";
		public static readonly string ShapeBottom = @"\shpbottom";
		public static readonly string ShapeZOrder = @"\shpz";
		public static readonly string ShapeLegacyHorizontalPositionTypePage = @"\shpbxpage";
		public static readonly string ShapeLegacyHorizontalPositionTypeMargin = @"\shpbxmargin";
		public static readonly string ShapeLegacyHorizontalPositionTypeColumn = @"\shpbxcolumn";
		public static readonly string ShapeIgnoreLegacyHorizontalPositionType = @"\shpbxignore";
		public static readonly string ShapeLegacyVerticalPositionTypePage = @"\shpbypage";
		public static readonly string ShapeLegacyVerticalPositionTypeMargin = @"\shpbymargin";
		public static readonly string ShapeLegacyVerticalPositionTypeParagraph = @"\shpbypara";
		public static readonly string ShapeIgnoreLegacyVerticalPositionType = @"\shpbyignore";
		public static readonly string ShapeWrapTextType = @"\shpwr";
		public static readonly string ShapeWrapTextTypeZOrder = @"\shpfblwtxt";
		public static readonly string ShapeWrapTextSide = @"\shpwrk";
		public static readonly string ShapeLocked = @"\shplockanchor";
		public static readonly string ShapeProperty = @"\sp";
		public static readonly string ShapePropertyName = @"\sn";
		public static readonly string ShapePropertyValue = @"\sv";
		public static readonly string ShapeResult = @"\shprslt";
		public static readonly string ShapeDoNotLay = @"\splytwnine";
		public static readonly string HtmlAutoSpacing = @"\htmautsp";
		public static readonly string CustomRunData = @"\*\dxcustomrundata";
		public static readonly string ParagraphGroupPropertiesTable = @"\*\pgptbl";
		public static readonly string ParagraphGroupProperties = @"\pgp";
		public static readonly string ParagraphGroupPropertiesId = @"\ipgp";
		#region Tables
		public static readonly string ResetTableProperties = @"\trowd";
		public static readonly string InTableParagraph = @"\intbl";
		public static readonly string TableEndCell = @"\cell";
		public static readonly string NestedTableEndCell = @"\nestcell";
		public static readonly string TableEndRow = @"\row";
		public static readonly string NestedTableEndRow = @"\nestrow";
		public static readonly string NestedTableProperties = @"\*\nesttableprops";
		public static readonly string NoNestedTable = @"\nonesttables";
		public static readonly string ParagraphNestingLevel = @"\itap";
		public static readonly string TableCellRight = @"\cellx";
		public static readonly string TableCellPreferredWidth = @"\clwWidth";
		public static readonly string TableCellPreferredWidthType = @"\clftsWidth";
		public static readonly string TableCellBottomMargin = @"\clpadb";
		public static readonly string TableCellLeftMargin = @"\clpadl";
		public static readonly string TableCellRightMargin = @"\clpadr";
		public static readonly string TableCellTopMargin = @"\clpadt";
		public static readonly string TableCellBottomMarginType = @"\clpadfb";
		public static readonly string TableCellLeftMarginType = @"\clpadfl";
		public static readonly string TableCellRightMarginType = @"\clpadfr";
		public static readonly string TableCellTopMarginType = @"\clpadft";
		public static readonly string TableRowIndex = @"\irow";
		public static readonly string TableRowBandIndex = @"\irowband";
		public static readonly string TableRowLeftAlignment = @"\trql";
		public static readonly string TableRowRightAlignment = @"\trqr";
		public static readonly string TableRowCenterAlignment = @"\trqc";
		public static readonly string TableIndent = @"\tblind";
		public static readonly string TableIndentType = @"\tblindtype";
		public static readonly string TableCellBottomBorder = @"\clbrdrb";
		public static readonly string TableCellTopBorder = @"\clbrdrt";
		public static readonly string TableCellLeftBorder = @"\clbrdrl";
		public static readonly string TableCellRightBorder = @"\clbrdrr";
		public static readonly string TableCellUpperLeftToLowerRightBorder = @"\cldglu";
		public static readonly string TableCellUpperRightToLowerLeftBorder = @"\cldgll";
		public static readonly string TableCellStartHorizontalMerging = @"\clmgf";
		public static readonly string TableCellContinueHorizontalMerging = @"\clmrg";
		public static readonly string TableCellStartVerticalMerging = @"\clvmgf";
		public static readonly string TableCellContinueVerticalMerging = @"\clvmrg";
		public static readonly string TableCellTextTopAlignment = @"\clvertalt";
		public static readonly string TableCellTextCenterAlignment = @"\clvertalc";
		public static readonly string TableCellTextBottomAlignment = @"\clvertalb";
		public static readonly string TableCellLeftToRightTopToBottomTextDirection = @"\cltxlrtb";
		public static readonly string TableCellTopToBottomRightToLeftTextDirection = @"\cltxtbrl";
		public static readonly string TableCellBottomToTopLeftToRightTextDirection = @"\cltxbtlr";
		public static readonly string TableCellLeftToRightTopToBottomVerticalTextDirection = @"\cltxlrtbv";
		public static readonly string TableCellTopToBottomRightToLeftVerticalTextDirection = @"\cltxtbrlv";
		public static readonly string TableCellFitText = @"\clFitText";
		public static readonly string TableCellNoWrap = @"\clNoWrap";
		public static readonly string TableCellHideMark = @"\clhidemark";
		public static readonly string TableCellBackgroundColor = @"\clcbpat";
		public static readonly string TableCellForegroundColor = @"\clcfpat";
		public static readonly string TableCellShading = @"\clshdng";
		public static readonly string TableTopBorder = @"\trbrdrt";
		public static readonly string TableLeftBorder = @"\trbrdrl";
		public static readonly string TableBottomBorder = @"\trbrdrb";
		public static readonly string TableRightBorder = @"\trbrdrr";
		public static readonly string TableHorizontalBorder = @"\trbrdrh";
		public static readonly string TableVerticalBorder = @"\trbrdrv";
		public static readonly string TableRowHorizontalAnchorColumn = @"\tphcol";
		public static readonly string TableRowHorizontalAnchorMargin = @"\tphmrg";
		public static readonly string TableRowHorizontalAnchorPage = @"\tphpg";
		public static readonly string TableRowVerticalAnchorMargin = @"\tpvmrg";
		public static readonly string TableRowVerticalAnchorParagraph = @"\tpvpara";
		public static readonly string TableRowVerticalAnchorPage = @"\tpvpg";
		public static readonly string TableRowHorizontalAlignCenter = @"\tposxc";
		public static readonly string TableRowHorizontalAlignInside = @"\tposxi";
		public static readonly string TableRowHorizontalAlignLeft = @"\tposxl";
		public static readonly string TableRowHorizontalAlignOutside = @"\tposxo";
		public static readonly string TableRowHorizontalAlignRight = @"\tposxr";
		public static readonly string TableRowHorizontalPosition = @"\tposx";
		public static readonly string TableRowHorizontalPositionNeg = @"\tposnegx";
		public static readonly string TableRowVerticalAlignBottom = @"\tposyb";
		public static readonly string TableRowVerticalAlignCenter = @"\tposyc";
		public static readonly string TableRowVerticalAlignInline = @"\tposyil";
		public static readonly string TableRowVerticalAlignInside = @"\tposyin";
		public static readonly string TableRowVerticalAlignOutside = @"\tposyout";
		public static readonly string TableRowVerticalAlignTop = @"\tposyt";
		public static readonly string TableRowVerticalPosition = @"\tposy";
		public static readonly string TableRowVerticalPositionNeg = @"\tposnegy";
		public static readonly string TableRowLeftFromText = @"\tdfrmtxtLeft";
		public static readonly string TableRowBottomFromText = @"\tdfrmtxtBottom";
		public static readonly string TableRowRightFromText = @"\tdfrmtxtRight";
		public static readonly string TableRowTopFromText = @"\tdfrmtxtTop";
		public static readonly string TableNoOverlap = @"\tabsnoovrlp";
		public static readonly string TableHalfSpaceBetweenCells = @"\trgaph";
		public static readonly string TableRowLeft = @"\trleft";
		public static readonly string TableRowHeight = @"\trrh";
		public static readonly string TableRowHeader = @"\trhdr";
		public static readonly string TableRowCantSplit = @"\trkeep";
		public static readonly string TablePreferredWidth = @"\trwWidth";
		public static readonly string TablePreferredWidthType = @"\trftsWidth";
		public static readonly string TableRowWidthBefore = @"\trwWidthB";
		public static readonly string TableRowWidthBeforeType = @"\trftsWidthB";
		public static readonly string TableRowWidthAfter = @"\trwWidthA";
		public static readonly string TableRowWidthAfterType = @"\trftsWidthA";
		public static readonly string TableLayout = @"\trautofit";
		public static readonly string TableCellSpacingBottom = @"\trspdb";
		public static readonly string TableCellSpacingLeft = @"\trspdl";
		public static readonly string TableCellSpacingRight = @"\trspdr";
		public static readonly string TableCellSpacingTop = @"\trspdt";
		public static readonly string TableCellSpacingBottomType = @"\trspdfb";
		public static readonly string TableCellSpacingLeftType = @"\trspdfl";
		public static readonly string TableCellSpacingRightType = @"\trspdfr";
		public static readonly string TableCellSpacingTopType = @"\trspdft";
		public static readonly string TableCellMarginsBottom = @"\trpaddb";
		public static readonly string TableCellMarginsLeft = @"\trpaddl";
		public static readonly string TableCellMarginsRight = @"\trpaddr";
		public static readonly string TableCellMarginsTop = @"\trpaddt";
		public static readonly string TableCellMarginsBottomType = @"\trpaddfb";
		public static readonly string TableCellMarginsLeftType = @"\trpaddfl";
		public static readonly string TableCellMarginsRightType = @"\trpaddfr";
		public static readonly string TableCellMarginsTopType = @"\trpaddft";
		public static readonly string TableApplyFirstRow = @"\tbllkhdrrows";
		public static readonly string TableApplyLastRow = @"\tbllklastrow";
		public static readonly string TableApplyFirstColumn = @"\tbllkhdrcols";
		public static readonly string TableApplyLastColumn = @"\tbllklastcol";
		public static readonly string TableDoNotApplyRowBanding = @"\tbllknorowband";
		public static readonly string TableDoNotApplyColumnBanding = @"\tbllknocolband";
		public static readonly string TableLastRow = @"\lastrow";
		#endregion
		#region Table Style
		public static readonly string TableStyleResetTableProperties = @"\tsrowd";
		public static readonly string TableStyleCellVerticalAlignmentTop = @"\tsvertalt";
		public static readonly string TableStyleCellVerticalAlignmentCenter = @"\tsvertalc";
		public static readonly string TableStyleCellVerticalAlignmentBottom = @"\tsvertalb";
		public static readonly string TableStyleRowBandSize = @"\tscbandsh";
		public static readonly string TableStyleColumnBandSize = @"\tscbandsv";
		public static readonly string TableStyleCellBackgroundColor = @"\tscellcbpat";
		public static readonly string TableStyleTopCellBorder = @"\tsbrdrt";
		public static readonly string TableStyleLeftCellBorder = @"\tsbrdrl";
		public static readonly string TableStyleBottomCellBorder = @"\tsbrdrb";
		public static readonly string TableStyleRightCellBorder = @"\tsbrdrr";
		public static readonly string TableStyleHorizontalCellBorder = @"\tsbrdrh";
		public static readonly string TableStyleVerticalCellBorder = @"\tsbrdrv";
		public static readonly string TableStyleCellNoWrap = @"\tsnowrap";
		public static readonly string TableStyleTableBottomCellMargin = @"\tscellpaddb";
		public static readonly string TableStyleTableLeftCellMargin = @"\tscellpaddl";
		public static readonly string TableStyleTableRightCellMargin = @"\tscellpaddr";
		public static readonly string TableStyleTableTopCellMargin = @"\tscellpaddt";
		public static readonly string TableStyleTableBottomCellMarginUnitType = @"\tscellpaddfb";
		public static readonly string TableStyleTableLeftCellMarginUnitType = @"\tscellpaddfl";
		public static readonly string TableStyleTableRightCellMarginUnitType = @"\tscellpaddfr";
		public static readonly string TableStyleTableTopCellMarginUnitType = @"\tscellpaddft";
		public static readonly string TableStyleUpperLeftToLowerRightBorder = @"\tsbrdrdgl";
		public static readonly string TableStyleUpperRightToLowerLeftBorder = @"\tsbrdrdgr";
		#endregion
		#region Table Conditional Style
		public static readonly string TableConditionalStyleFirstRow = @"\tscfirstrow";
		public static readonly string TableConditionalStyleLastRow = @"\tsclastrow";
		public static readonly string TableConditionalStyleFirstColumn = @"\tscfirstcol";
		public static readonly string TableConditionalStyleLastColumn = @"\tsclastcol";
		public static readonly string TableConditionalStyleOddRowBanding = @"\tscbandhorzodd";
		public static readonly string TableConditionalStyleEvenRowBanding = @"\tscbandhorzeven";
		public static readonly string TableConditionalStyleOddColumnBanding = @"\tscbandvertodd";
		public static readonly string TableConditionalStyleEvenColumnBanding = @"\tscbandverteven";
		public static readonly string TableConditionalStyleTopLeftCell = @"\tscnwcell";
		public static readonly string TableConditionalStyleTopRightCell = @"\tscnecell";
		public static readonly string TableConditionalStyleBottomLeftCell = @"\tscswcell";
		public static readonly string TableConditionalStyleBottomRightCell = @"\tscsecell";
		#endregion
		#region Border
		public static readonly string NoTableBorder = @"\brdrtbl";
		public static readonly string NoBorder = @"\brdrnil";
		public static readonly string BorderWidth = @"\brdrw";
		public static readonly string BorderColor = @"\brdrcf";
		public static readonly string BorderFrame = @"\brdrframe";
		public static readonly string BorderSpace = @"\brsp";
		public static readonly string BorderArtIndex = @"\brdrart";
		public static readonly string BorderSingleWidth = @"\brdrs";
		public static readonly string BorderDoubleWidth = @"\brdrth";
		public static readonly string BorderShadow = @"\brdrsh";
		public static readonly string BorderDouble = @"\brdrdb";
		public static readonly string BorderDotted = @"\brdrdot";
		public static readonly string BorderDashed = @"\brdrdash";
		public static readonly string BorderSingle = @"\brdrhair";
		public static readonly string BorderDashedSmall = @"\brdrdashsm";
		public static readonly string BorderDotDashed = @"\brdrdashd";
		public static readonly string BorderDotDotDashed = @"\brdrdashdd";
		public static readonly string BorderInset = @"\brdrinset";
		public static readonly string BorderNone = @"\brdrnone";
		public static readonly string BorderOutset = @"\brdroutset";
		public static readonly string BorderTriple = @"\brdrtriple";
		public static readonly string BorderThickThinSmall = @"\brdrtnthsg";
		public static readonly string BorderThinThickSmall = @"\brdrthtnsg";
		public static readonly string BorderThinThickThinSmall = @"\brdrtnthtnsg";
		public static readonly string BorderThickThinMedium = @"\brdrtnthmg";
		public static readonly string BorderThinThickMedium = @"\brdrthtnmg";
		public static readonly string BorderThinThickThinMedium = @"\brdrtnthtnmg";
		public static readonly string BorderThickThinLarge = @"\brdrtnthlg";
		public static readonly string BorderThinThickLarge = @"\brdrthtnlg";
		public static readonly string BorderThinThickThinLarge = @"\brdrtnthtnlg";
		public static readonly string BorderWavy = @"\brdrwavy";
		public static readonly string BorderDoubleWavy = @"\brdrwavydb";
		public static readonly string BorderDashDotStroked = @"\brdrdashdotstr";
		public static readonly string BorderThreeDEmboss = @"\brdremboss";
		public static readonly string BorderThreeDEngrave = @"\brdrengrave";
		#endregion
		#region NumberingList
		public static readonly string NumberingListTable = @"\*\listtable";
		public static readonly string ListOverrideTable = @"\*\listoverridetable";
		public static readonly string NumberingList = @"\list";
		public static readonly string NumberingListId = @"\listid";
		public static readonly string NumberingListTemplateId = @"\listtemplateid";
		public static readonly string NumberingListStyleId = @"\liststyleid";
		public static readonly string NumberingListStyleName = @"\*\liststylename ";
		public static readonly string NumberingListName = @"\listname ;";
		public static readonly string NumberingListHybrid = @"\listhybrid";
		public static readonly string ListLevel = @"\listlevel";
		public static readonly string ListLevelStart = @"\levelstartat";
		public static readonly string ListLevelTentative = @"\lvltentative";
		public static readonly string ListLevelNumberingFormat = @"\levelnfc";
		public static readonly string ListLevelAlignment = @"\leveljc";
		public static readonly string ListLevelNumberingFormatN = @"\levelnfcn";
		public static readonly string ListLevelAlignmentN = @"\leveljcn";
		public static readonly string LisLeveltOld = @"\levelold";
		public static readonly string ListLevelPrev = @"\levelprev";
		public static readonly string ListLevelPrevSpase = @"\levelprevspace";
		public static readonly string ListLevelSpace = @"\levelspace";
		public static readonly string ListLevelIntdent = @"\levelindent";
		public static readonly string ListLevelNumbers = @"\levelnumbers";
		public static readonly string ListLevelText = @"\leveltext";
		public static readonly string LevelTemplateId = @"\leveltemplateid";
		public static readonly string ListLevelFollow = @"\levelfollow";
		public static readonly string ListLevelLegal = @"\levellegal";
		public static readonly string ListLevelNoRestart = @"\levelnorestart";
		public static readonly string ListLevelPicture = @"\levelpicture";
		public static readonly string ListLevelPictureNoSize = @"\levelpicturenosize";
		public static readonly string ListLevelLegacy = @"\levelold";
		public static readonly string ListLevelLegacySpace = @"\levelspace";
		public static readonly string ListLevelLegacyIndent = @"\levelindent";
		public static readonly string ListOverride = @"\listoverride";
		public static readonly string ListOverrideListId = @"\listid";
		public static readonly string ListOverrideCount = @"\listoverridecount";
		public static readonly string ListOverrideLevel = @"\lfolevel";
		public static readonly string ListOverrideFormat = @"\listoverrideformat";
		public static readonly string ListOverrideStart = @"\listoverridestartat";
		public static readonly string ListOverrideStartValue = @"\levelstartat";
		public static readonly string ListOverrideListLevel = @"\listlevel";
		#endregion
		#region DefaultProperties
		public static readonly string DefaultCharacterProperties = @"\*\defchp";
		public static readonly string DefaultParagraphProperties = @"\*\defpap";
		#endregion
		#region Style
		public static readonly string StyleSheet = @"\stylesheet";
		public static readonly string ParagraphStyle = @"\s";
		public static readonly string CharacterStyle = @"\*\cs";
		public static readonly string CharacterStyleIndex = @"\cs";
		public static readonly string TableStyle = @"\*\ts";
		public static readonly string TableStyleIndex = @"\ts";
		public static readonly string TableStyleCellIndex = @"\yts";
		public static readonly string ParentStyle = @"\sbasedon";
		public static readonly string LinkedStyle = @"\slink";
		public static readonly string NextStyle = @"\snext";
		public static readonly string QuickFormatStyle = @"\sqformat";
		#endregion
		internal static readonly Dictionary<FloatingObjectTextWrapType, int> FloatingObjectTextWrapTypeTable = CreateFloatingObjectTextWrapTypeTable();
		internal static readonly Dictionary<FloatingObjectTextWrapSide, int> FloatingObjectTextWrapSideTable = CreateFloatingObjectTextWrapSideTable();
		internal static readonly Dictionary<FloatingObjectHorizontalPositionType, int> FloatingObjectHorizontalPositionTypeTable = CreateFloatingObjectHorizontalPositionTypeTable();
		internal static readonly Dictionary<FloatingObjectHorizontalPositionAlignment, int> FloatingObjectHorizontalPositionAlignmentTable = CreateFloatingObjectHorizontalPositionAlignmentTable();
		internal static readonly Dictionary<FloatingObjectVerticalPositionType, int> FloatingObjectVerticalPositionTypeTable = CreateFloatingObjectVerticalPositionTypeTable();
		internal static readonly Dictionary<FloatingObjectVerticalPositionAlignment, int> FloatingObjectVerticalPositionAlignmentTable = CreateFloatingObjectVerticalPositionAlignmentTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromHorizontal, int> FloatingObjectRelativeFromHorizontalTable = CreateFloatingObjectRelativeFromHorizontalTable();
		internal static readonly Dictionary<FloatingObjectRelativeFromVertical, int> FloatingObjectRelativeFromVerticalTable = CreateFloatingObjectRelativeFromVerticalTable();
		static Dictionary<FloatingObjectRelativeFromHorizontal, int> CreateFloatingObjectRelativeFromHorizontalTable() {
			Dictionary<FloatingObjectRelativeFromHorizontal, int> result = new Dictionary<FloatingObjectRelativeFromHorizontal, int>();
			result.Add(FloatingObjectRelativeFromHorizontal.Margin, 0);
			result.Add(FloatingObjectRelativeFromHorizontal.Page, 1);
			result.Add(FloatingObjectRelativeFromHorizontal.LeftMargin, 2);
			result.Add(FloatingObjectRelativeFromHorizontal.RightMargin, 3);
			result.Add(FloatingObjectRelativeFromHorizontal.InsideMargin, 4);
			result.Add(FloatingObjectRelativeFromHorizontal.OutsideMargin, 5);
			return result;
		}
		static Dictionary<FloatingObjectRelativeFromVertical, int> CreateFloatingObjectRelativeFromVerticalTable() {
			Dictionary<FloatingObjectRelativeFromVertical, int> result = new Dictionary<FloatingObjectRelativeFromVertical, int>();
			result.Add(FloatingObjectRelativeFromVertical.Margin, 0);
			result.Add(FloatingObjectRelativeFromVertical.Page, 1);
			result.Add(FloatingObjectRelativeFromVertical.TopMargin, 2);
			result.Add(FloatingObjectRelativeFromVertical.BottomMargin, 3);
			result.Add(FloatingObjectRelativeFromVertical.InsideMargin, 4);
			result.Add(FloatingObjectRelativeFromVertical.OutsideMargin, 5);
			return result;
		}
		static Dictionary<FloatingObjectVerticalPositionAlignment, int> CreateFloatingObjectVerticalPositionAlignmentTable() {
			Dictionary<FloatingObjectVerticalPositionAlignment, int> result = new Dictionary<FloatingObjectVerticalPositionAlignment, int>();
			result.Add(FloatingObjectVerticalPositionAlignment.None, 0);
			result.Add(FloatingObjectVerticalPositionAlignment.Top, 1);
			result.Add(FloatingObjectVerticalPositionAlignment.Center, 2);
			result.Add(FloatingObjectVerticalPositionAlignment.Bottom, 3);
			result.Add(FloatingObjectVerticalPositionAlignment.Inside, 4);
			result.Add(FloatingObjectVerticalPositionAlignment.Outside, 5);
			return result;
		}
		static Dictionary<FloatingObjectVerticalPositionType, int> CreateFloatingObjectVerticalPositionTypeTable() {
			Dictionary<FloatingObjectVerticalPositionType, int> result = new Dictionary<FloatingObjectVerticalPositionType, int>();
			result.Add(FloatingObjectVerticalPositionType.Margin, 0);
			result.Add(FloatingObjectVerticalPositionType.Page, 1);
			result.Add(FloatingObjectVerticalPositionType.Paragraph, 2);
			result.Add(FloatingObjectVerticalPositionType.Line, 3);
			result.Add(FloatingObjectVerticalPositionType.TopMargin, 4);
			result.Add(FloatingObjectVerticalPositionType.BottomMargin, 5);
			result.Add(FloatingObjectVerticalPositionType.InsideMargin, 6);
			result.Add(FloatingObjectVerticalPositionType.OutsideMargin, 7);
			return result;
		}
		static Dictionary<FloatingObjectHorizontalPositionAlignment, int> CreateFloatingObjectHorizontalPositionAlignmentTable() {
			Dictionary<FloatingObjectHorizontalPositionAlignment, int> result = new Dictionary<FloatingObjectHorizontalPositionAlignment, int>();
			result.Add(FloatingObjectHorizontalPositionAlignment.None, 0);
			result.Add(FloatingObjectHorizontalPositionAlignment.Left, 1);
			result.Add(FloatingObjectHorizontalPositionAlignment.Center, 2);
			result.Add(FloatingObjectHorizontalPositionAlignment.Right, 3);
			result.Add(FloatingObjectHorizontalPositionAlignment.Inside, 4);
			result.Add(FloatingObjectHorizontalPositionAlignment.Outside, 5);
			return result;
		}
		static Dictionary<FloatingObjectHorizontalPositionType, int> CreateFloatingObjectHorizontalPositionTypeTable() {
			Dictionary<FloatingObjectHorizontalPositionType, int> result = new Dictionary<FloatingObjectHorizontalPositionType, int>();
			result.Add(FloatingObjectHorizontalPositionType.Margin, 0);
			result.Add(FloatingObjectHorizontalPositionType.Page, 1);
			result.Add(FloatingObjectHorizontalPositionType.Column, 2);
			result.Add(FloatingObjectHorizontalPositionType.Character, 3);
			result.Add(FloatingObjectHorizontalPositionType.LeftMargin, 4);
			result.Add(FloatingObjectHorizontalPositionType.RightMargin, 5);
			result.Add(FloatingObjectHorizontalPositionType.InsideMargin, 6);
			result.Add(FloatingObjectHorizontalPositionType.OutsideMargin, 7);
			return result;
		}
		static Dictionary<FloatingObjectTextWrapSide, int> CreateFloatingObjectTextWrapSideTable() {
			Dictionary<FloatingObjectTextWrapSide, int> result = new Dictionary<FloatingObjectTextWrapSide, int>();
			result.Add(FloatingObjectTextWrapSide.Both, 0);
			result.Add(FloatingObjectTextWrapSide.Left, 1);
			result.Add(FloatingObjectTextWrapSide.Right, 2);
			result.Add(FloatingObjectTextWrapSide.Largest, 3);
			return result;
		}
		static Dictionary<FloatingObjectTextWrapType, int> CreateFloatingObjectTextWrapTypeTable() {
			Dictionary<FloatingObjectTextWrapType, int> result = new Dictionary<FloatingObjectTextWrapType, int>();
			result.Add(FloatingObjectTextWrapType.TopAndBottom, 1);
			result.Add(FloatingObjectTextWrapType.Square, 2);
			result.Add(FloatingObjectTextWrapType.None, 3);
			result.Add(FloatingObjectTextWrapType.Tight, 4);
			result.Add(FloatingObjectTextWrapType.Through, 5);
			return result;
		}
	}
	#endregion
}
