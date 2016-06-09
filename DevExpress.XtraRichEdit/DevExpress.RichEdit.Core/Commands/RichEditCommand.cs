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
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System;
#if !SL
using System.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region RichEditCommandId
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct RichEditCommandId : IConvertToInt<RichEditCommandId>, IEquatable<RichEditCommandId> {
		public static readonly RichEditCommandId None = new RichEditCommandId(0);
		public static readonly RichEditCommandId ToggleShowWhitespace = new RichEditCommandId(1);
		public static readonly RichEditCommandId PreviousCharacter = new RichEditCommandId(2);
		public static readonly RichEditCommandId NextCharacter = new RichEditCommandId(3);
		public static readonly RichEditCommandId ExtendPreviousCharacter = new RichEditCommandId(4);
		public static readonly RichEditCommandId ExtendNextCharacter = new RichEditCommandId(5);
		public static readonly RichEditCommandId PreviousWord = new RichEditCommandId(6);
		public static readonly RichEditCommandId NextWord = new RichEditCommandId(7);
		public static readonly RichEditCommandId ExtendPreviousWord = new RichEditCommandId(8);
		public static readonly RichEditCommandId ExtendNextWord = new RichEditCommandId(9);
		public static readonly RichEditCommandId PreviousParagraph = new RichEditCommandId(10);
		public static readonly RichEditCommandId NextParagraph = new RichEditCommandId(11);
		public static readonly RichEditCommandId ExtendPreviousParagraph = new RichEditCommandId(12);
		public static readonly RichEditCommandId ExtendNextParagraph = new RichEditCommandId(13);
		public static readonly RichEditCommandId PreviousPage = new RichEditCommandId(14);
		public static readonly RichEditCommandId NextPage = new RichEditCommandId(15);
		public static readonly RichEditCommandId ExtendPreviousPage = new RichEditCommandId(16);
		public static readonly RichEditCommandId ExtendNextPage = new RichEditCommandId(17);
		public static readonly RichEditCommandId PreviousScreen = new RichEditCommandId(18);
		public static readonly RichEditCommandId NextScreen = new RichEditCommandId(19);
		public static readonly RichEditCommandId ExtendPreviousScreen = new RichEditCommandId(20);
		public static readonly RichEditCommandId ExtendNextScreen = new RichEditCommandId(21);
		public static readonly RichEditCommandId StartOfLine = new RichEditCommandId(22);
		public static readonly RichEditCommandId EndOfLine = new RichEditCommandId(23);
		public static readonly RichEditCommandId ExtendStartOfLine = new RichEditCommandId(24);
		public static readonly RichEditCommandId ExtendEndOfLine = new RichEditCommandId(25);
		public static readonly RichEditCommandId StartOfDocument = new RichEditCommandId(26);
		public static readonly RichEditCommandId EndOfDocument = new RichEditCommandId(27);
		public static readonly RichEditCommandId ExtendStartOfDocument = new RichEditCommandId(28);
		public static readonly RichEditCommandId ExtendEndOfDocument = new RichEditCommandId(29);
		public static readonly RichEditCommandId PreviousLine = new RichEditCommandId(30);
		public static readonly RichEditCommandId NextLine = new RichEditCommandId(31);
		public static readonly RichEditCommandId ExtendPreviousLine = new RichEditCommandId(32);
		public static readonly RichEditCommandId ExtendNextLine = new RichEditCommandId(33);
		public static readonly RichEditCommandId SelectAll = new RichEditCommandId(34);
		public static readonly RichEditCommandId Undo = new RichEditCommandId(35);
		public static readonly RichEditCommandId Redo = new RichEditCommandId(36);
		public static readonly RichEditCommandId InsertParagraph = new RichEditCommandId(37);
		public static readonly RichEditCommandId InsertLineBreak = new RichEditCommandId(38);
		public static readonly RichEditCommandId InsertPageBreak = new RichEditCommandId(39);
		public static readonly RichEditCommandId InsertColumnBreak = new RichEditCommandId(40);
		public static readonly RichEditCommandId InsertNonBreakingSpace = new RichEditCommandId(41);
		public static readonly RichEditCommandId InsertEnDash = new RichEditCommandId(42);
		public static readonly RichEditCommandId InsertEmDash = new RichEditCommandId(43);
		public static readonly RichEditCommandId InsertCopyrightSymbol = new RichEditCommandId(44);
		public static readonly RichEditCommandId InsertRegisteredTrademarkSymbol = new RichEditCommandId(45);
		public static readonly RichEditCommandId InsertTrademarkSymbol = new RichEditCommandId(46);
		public static readonly RichEditCommandId InsertEllipsis = new RichEditCommandId(47);
		public static readonly RichEditCommandId ToggleFontBold = new RichEditCommandId(48);
		public static readonly RichEditCommandId ToggleFontItalic = new RichEditCommandId(49);
		public static readonly RichEditCommandId ToggleFontUnderline = new RichEditCommandId(50);
		public static readonly RichEditCommandId ToggleFontDoubleUnderline = new RichEditCommandId(51);
		public static readonly RichEditCommandId IncreaseFontSize = new RichEditCommandId(52);
		public static readonly RichEditCommandId DecreaseFontSize = new RichEditCommandId(53);
		public static readonly RichEditCommandId IncrementFontSize = new RichEditCommandId(54);
		public static readonly RichEditCommandId DecrementFontSize = new RichEditCommandId(55);
		public static readonly RichEditCommandId ToggleFontSuperscript = new RichEditCommandId(56);
		public static readonly RichEditCommandId ToggleFontSubscript = new RichEditCommandId(57);
		public static readonly RichEditCommandId ShowFontForm = new RichEditCommandId(58);
		public static readonly RichEditCommandId ShowParagraphForm = new RichEditCommandId(59);
		public static readonly RichEditCommandId ToggleParagraphAlignmentLeft = new RichEditCommandId(60);
		public static readonly RichEditCommandId ToggleParagraphAlignmentCenter = new RichEditCommandId(61);
		public static readonly RichEditCommandId ToggleParagraphAlignmentRight = new RichEditCommandId(62);
		public static readonly RichEditCommandId ToggleParagraphAlignmentJustify = new RichEditCommandId(63);
		public static readonly RichEditCommandId SetSingleParagraphSpacing = new RichEditCommandId(64);
		public static readonly RichEditCommandId SetDoubleParagraphSpacing = new RichEditCommandId(65);
		public static readonly RichEditCommandId SetSesquialteralParagraphSpacing = new RichEditCommandId(66);
		public static readonly RichEditCommandId Delete = new RichEditCommandId(67);
		public static readonly RichEditCommandId BackSpaceKey = new RichEditCommandId(68);
		public static readonly RichEditCommandId DeleteWord = new RichEditCommandId(69);
		public static readonly RichEditCommandId DeleteWordBack = new RichEditCommandId(70);
		public static readonly RichEditCommandId CopySelection = new RichEditCommandId(71);
		public static readonly RichEditCommandId PasteSelection = new RichEditCommandId(72);
		public static readonly RichEditCommandId CutSelection = new RichEditCommandId(73);
		public static readonly RichEditCommandId IncrementNumerationFromParagraph = new RichEditCommandId(74);
		public static readonly RichEditCommandId DecrementNumerationFromParagraph = new RichEditCommandId(75);
		public static readonly RichEditCommandId FindNext = new RichEditCommandId(76);
		public static readonly RichEditCommandId FindPrev = new RichEditCommandId(77);
		public static readonly RichEditCommandId Find = new RichEditCommandId(78);
		public static readonly RichEditCommandId Replace = new RichEditCommandId(79);
		public static readonly RichEditCommandId TabKey = new RichEditCommandId(80);
		public static readonly RichEditCommandId ShiftTabKey = new RichEditCommandId(81);
		public static readonly RichEditCommandId FileNew = new RichEditCommandId(82);
		public static readonly RichEditCommandId FileOpen = new RichEditCommandId(83);
		public static readonly RichEditCommandId FileSave = new RichEditCommandId(84);
		public static readonly RichEditCommandId FileSaveAs = new RichEditCommandId(85);
		public static readonly RichEditCommandId IncreaseIndent = new RichEditCommandId(86);
		public static readonly RichEditCommandId DecreaseIndent = new RichEditCommandId(87);
		public static readonly RichEditCommandId ZoomIn = new RichEditCommandId(88);
		public static readonly RichEditCommandId ZoomOut = new RichEditCommandId(89);
		public static readonly RichEditCommandId InsertPicture = new RichEditCommandId(90);
		public static readonly RichEditCommandId ToggleFontStrikeout = new RichEditCommandId(91);
		public static readonly RichEditCommandId ToggleFontDoubleStrikeout = new RichEditCommandId(92);
		public static readonly RichEditCommandId ToggleNumberingListItem = new RichEditCommandId(93);
		public static readonly RichEditCommandId ToggleMultilevelListItem = new RichEditCommandId(94);
		public static readonly RichEditCommandId ToggleBulletedListItem = new RichEditCommandId(95);
		public static readonly RichEditCommandId Print = new RichEditCommandId(96);
		public static readonly RichEditCommandId QuickPrint = new RichEditCommandId(97);
		public static readonly RichEditCommandId PrintPreview = new RichEditCommandId(98);
		public static readonly RichEditCommandId ClearFormatting = new RichEditCommandId(99);
		public static readonly RichEditCommandId ToggleHiddenText = new RichEditCommandId(100);
		public static readonly RichEditCommandId CreateField = new RichEditCommandId(101);
		public static readonly RichEditCommandId UpdateField = new RichEditCommandId(102);
		public static readonly RichEditCommandId ToggleFieldCodes = new RichEditCommandId(103);
		public static readonly RichEditCommandId ShowInsertMergeFieldForm = new RichEditCommandId(104);
		public static readonly RichEditCommandId ToggleViewMergedData = new RichEditCommandId(105);
		public static readonly RichEditCommandId ShowAllFieldCodes = new RichEditCommandId(106);
		public static readonly RichEditCommandId ShowAllFieldResults = new RichEditCommandId(107);
		public static readonly RichEditCommandId ChangeFontName = new RichEditCommandId(108);
		public static readonly RichEditCommandId ChangeFontSize = new RichEditCommandId(109);
		public static readonly RichEditCommandId ChangeFontStyle = new RichEditCommandId(110);
		public static readonly RichEditCommandId ShowNumberingListForm = new RichEditCommandId(111);
		public static readonly RichEditCommandId InsertTab = new RichEditCommandId(112);
		public static readonly RichEditCommandId ShowSymbolForm = new RichEditCommandId(113);
		public static readonly RichEditCommandId SwitchToDraftView = new RichEditCommandId(114);
		public static readonly RichEditCommandId SwitchToPrintLayoutView = new RichEditCommandId(115);
		public static readonly RichEditCommandId SwitchToSimpleView = new RichEditCommandId(116);
		public static readonly RichEditCommandId ShowBookmarkForm = new RichEditCommandId(117);
		public static readonly RichEditCommandId ShowHyperlinkForm = new RichEditCommandId(118);
		public static readonly RichEditCommandId EnterKey = new RichEditCommandId(119);
		public static readonly RichEditCommandId EditPageHeader = new RichEditCommandId(120);
		public static readonly RichEditCommandId EditPageFooter = new RichEditCommandId(121);
		public static readonly RichEditCommandId ClosePageHeaderFooter = new RichEditCommandId(122);
		public static readonly RichEditCommandId GoToPageHeader = new RichEditCommandId(123);
		public static readonly RichEditCommandId GoToPageFooter = new RichEditCommandId(124);
		public static readonly RichEditCommandId ToggleHeaderFooterLinkToPrevious = new RichEditCommandId(125);
		public static readonly RichEditCommandId GoToPreviousHeaderFooter = new RichEditCommandId(126);
		public static readonly RichEditCommandId GoToNextHeaderFooter = new RichEditCommandId(127);
		public static readonly RichEditCommandId ToggleDifferentFirstPage = new RichEditCommandId(128);
		public static readonly RichEditCommandId ToggleDifferentOddAndEvenPages = new RichEditCommandId(129);
		public static readonly RichEditCommandId ChangeParagraphLineSpacing = new RichEditCommandId(130);
		public static readonly RichEditCommandId ShowLineSpacingForm = new RichEditCommandId(131);
		public static readonly RichEditCommandId AddSpacingBeforeParagraph = new RichEditCommandId(132);
		public static readonly RichEditCommandId AddSpacingAfterParagraph = new RichEditCommandId(133);
		public static readonly RichEditCommandId RemoveSpacingBeforeParagraph = new RichEditCommandId(134);
		public static readonly RichEditCommandId RemoveSpacingAfterParagraph = new RichEditCommandId(135);
		public static readonly RichEditCommandId SetPortraitPageOrientation = new RichEditCommandId(136);
		public static readonly RichEditCommandId SetLandscapePageOrientation = new RichEditCommandId(137);
		public static readonly RichEditCommandId ChangeSectionPageOrientation = new RichEditCommandId(138);
		public static readonly RichEditCommandId ChangeSectionPageMargins = new RichEditCommandId(139);
		public static readonly RichEditCommandId SetNormalSectionPageMargins = new RichEditCommandId(140);
		public static readonly RichEditCommandId SetNarrowSectionPageMargins = new RichEditCommandId(141);
		public static readonly RichEditCommandId SetModerateSectionPageMargins = new RichEditCommandId(142);
		public static readonly RichEditCommandId SetWideSectionPageMargins = new RichEditCommandId(143);
		public static readonly RichEditCommandId InsertTable = new RichEditCommandId(144);
		public static readonly RichEditCommandId InsertTableRowBelow = new RichEditCommandId(146);
		public static readonly RichEditCommandId InsertTableRowAbove = new RichEditCommandId(147);
		public static readonly RichEditCommandId MergeTableElement = new RichEditCommandId(148);
		public static readonly RichEditCommandId MergeTableCells = new RichEditCommandId(149);
		public static readonly RichEditCommandId InsertPageNumberField = new RichEditCommandId(150);
		public static readonly RichEditCommandId InsertPageCountField = new RichEditCommandId(151);
		public static readonly RichEditCommandId DeleteTableRowsMenuItem = new RichEditCommandId(152);
		public static readonly RichEditCommandId ToggleSpellCheckAsYouType = new RichEditCommandId(153);
		public static readonly RichEditCommandId CheckSpelling = new RichEditCommandId(154);
		public static readonly RichEditCommandId NextDataRecord = new RichEditCommandId(155);
		public static readonly RichEditCommandId PreviousDataRecord = new RichEditCommandId(156);
		public static readonly RichEditCommandId MailMergeSaveDocumentAs = new RichEditCommandId(157);
		public static readonly RichEditCommandId InsertMailMergeField = new RichEditCommandId(158);
		public static readonly RichEditCommandId InsertMailMergeFieldPlaceholder = new RichEditCommandId(159);
		public static readonly RichEditCommandId SplitTable = new RichEditCommandId(160);
		public static readonly RichEditCommandId ChangeTableBordersPlaceholder = new RichEditCommandId(161);
		public static readonly RichEditCommandId ToggleTableCellsAllBorders = new RichEditCommandId(162);
		public static readonly RichEditCommandId ResetTableCellsAllBorders = new RichEditCommandId(163);
		public static readonly RichEditCommandId ToggleTableCellsOutsideBorder = new RichEditCommandId(164);
		public static readonly RichEditCommandId ToggleTableCellsInsideBorder = new RichEditCommandId(165);
		public static readonly RichEditCommandId ToggleTableCellsLeftBorder = new RichEditCommandId(166);
		public static readonly RichEditCommandId ToggleTableCellsRightBorder = new RichEditCommandId(167);
		public static readonly RichEditCommandId ToggleTableCellsTopBorder = new RichEditCommandId(168);
		public static readonly RichEditCommandId ToggleTableCellsBottomBorder = new RichEditCommandId(169);
		public static readonly RichEditCommandId ToggleTableCellsInsideHorizontalBorder = new RichEditCommandId(170);
		public static readonly RichEditCommandId ToggleTableCellsInsideVerticalBorder = new RichEditCommandId(171);
		public static readonly RichEditCommandId InsertTableColumnToTheLeft = new RichEditCommandId(172);
		public static readonly RichEditCommandId InsertTableColumnToTheRight = new RichEditCommandId(173);
		public static readonly RichEditCommandId ChangeFontForeColor = new RichEditCommandId(174);
		public static readonly RichEditCommandId ChangeFontBackColor = new RichEditCommandId(175);
		public static readonly RichEditCommandId DeleteTable = new RichEditCommandId(176);
		public static readonly RichEditCommandId ChangeTableCellsBorderColor = new RichEditCommandId(177);
		public static readonly RichEditCommandId ChangeTableCellsShading = new RichEditCommandId(178);
		public static readonly RichEditCommandId ShowInsertTableCellsForm = new RichEditCommandId(179);
		public static readonly RichEditCommandId Zoom = new RichEditCommandId(180);
		public static readonly RichEditCommandId GoToPage = new RichEditCommandId(181);
		public static readonly RichEditCommandId ShowDeleteTableCellsForm = new RichEditCommandId(182);
		public static readonly RichEditCommandId DeleteTableColumns = new RichEditCommandId(183);
		public static readonly RichEditCommandId DeleteTableRows = new RichEditCommandId(184);
		public static readonly RichEditCommandId DeleteTablePlaceholder = new RichEditCommandId(185);
		public static readonly RichEditCommandId ChangeTableCellsContentAlignmentPlaceholder = new RichEditCommandId(186);
		public static readonly RichEditCommandId ToggleTableCellsTopLeftAlignment = new RichEditCommandId(187);
		public static readonly RichEditCommandId ToggleTableCellsTopCenterAlignment = new RichEditCommandId(188);
		public static readonly RichEditCommandId ToggleTableCellsTopRightAlignment = new RichEditCommandId(189);
		public static readonly RichEditCommandId ToggleTableCellsMiddleLeftAlignment = new RichEditCommandId(190);
		public static readonly RichEditCommandId ToggleTableCellsMiddleCenterAlignment = new RichEditCommandId(191);
		public static readonly RichEditCommandId ToggleTableCellsMiddleRightAlignment = new RichEditCommandId(192);
		public static readonly RichEditCommandId ToggleTableCellsBottomLeftAlignment = new RichEditCommandId(193);
		public static readonly RichEditCommandId ToggleTableCellsBottomCenterAlignment = new RichEditCommandId(194);
		public static readonly RichEditCommandId ToggleTableCellsBottomRightAlignment = new RichEditCommandId(195);
		public static readonly RichEditCommandId ZoomPercent = new RichEditCommandId(196);
		public static readonly RichEditCommandId ShowSplitTableCellsForm = new RichEditCommandId(197);
		public static readonly RichEditCommandId SetSectionOneColumn = new RichEditCommandId(198);
		public static readonly RichEditCommandId SetSectionTwoColumns = new RichEditCommandId(199);
		public static readonly RichEditCommandId SetSectionThreeColumns = new RichEditCommandId(200);
		public static readonly RichEditCommandId SetSectionColumnsPlaceholder = new RichEditCommandId(201);
		public static readonly RichEditCommandId ShowSplitTableCellsFormMenuItem = new RichEditCommandId(202);
		public static readonly RichEditCommandId SelectTableColumns = new RichEditCommandId(203);
		public static readonly RichEditCommandId SelectTableCell = new RichEditCommandId(204);
		public static readonly RichEditCommandId SelectTableRow = new RichEditCommandId(205);
		public static readonly RichEditCommandId LastDataRecord = new RichEditCommandId(206);
		public static readonly RichEditCommandId FirstDataRecord = new RichEditCommandId(207);
		public static readonly RichEditCommandId ShowRangeEditingPermissionsForm = new RichEditCommandId(208);
		public static readonly RichEditCommandId ChangeSectionPaperKind = new RichEditCommandId(209);
		public static readonly RichEditCommandId ChangeSectionPaperKindPlaceholder = new RichEditCommandId(210);
		public static readonly RichEditCommandId ProtectDocument = new RichEditCommandId(211);
		public static readonly RichEditCommandId UnprotectDocument = new RichEditCommandId(212);
		public static readonly RichEditCommandId MakeTextUpperCase = new RichEditCommandId(213);
		public static readonly RichEditCommandId MakeTextLowerCase = new RichEditCommandId(214);
		public static readonly RichEditCommandId ToggleTextCase = new RichEditCommandId(215);
		public static readonly RichEditCommandId ChangeTextCasePlaceholder = new RichEditCommandId(216);
		public static readonly RichEditCommandId SelectTable = new RichEditCommandId(217);
		public static readonly RichEditCommandId SelectTablePlaceholder = new RichEditCommandId(218);
		public static readonly RichEditCommandId ShowDeleteTableCellsFormMenuItem = new RichEditCommandId(219);
		public static readonly RichEditCommandId DeleteTableColumnsMenuItem = new RichEditCommandId(220);
		public static readonly RichEditCommandId ClearUndo = new RichEditCommandId(221);
		public static readonly RichEditCommandId DeselectAll = new RichEditCommandId(222);
		public static readonly RichEditCommandId ToggleShowTableGridLines = new RichEditCommandId(223);
		public static readonly RichEditCommandId ToggleShowHorizontalRuler = new RichEditCommandId(224);
		public static readonly RichEditCommandId ToggleShowVerticalRuler = new RichEditCommandId(225);
		public static readonly RichEditCommandId ChangeTableCellsBorderLineStyle = new RichEditCommandId(226);
		public static readonly RichEditCommandId InsertBreak = new RichEditCommandId(228);
		public static readonly RichEditCommandId InsertSectionBreakNextPage = new RichEditCommandId(229);
		public static readonly RichEditCommandId InsertSectionBreakOddPage = new RichEditCommandId(230);
		public static readonly RichEditCommandId InsertSectionBreakEvenPage = new RichEditCommandId(231);
		public static readonly RichEditCommandId InsertSectionBreakContinuous = new RichEditCommandId(232);
		public static readonly RichEditCommandId ChangeSectionLineNumbering = new RichEditCommandId(233);
		public static readonly RichEditCommandId SetSectionLineNumberingNone = new RichEditCommandId(234);
		public static readonly RichEditCommandId SetSectionLineNumberingContinuous = new RichEditCommandId(235);
		public static readonly RichEditCommandId SetSectionLineNumberingRestartNewPage = new RichEditCommandId(236);
		public static readonly RichEditCommandId SetSectionLineNumberingRestartNewSection = new RichEditCommandId(237);
		public static readonly RichEditCommandId ToggleParagraphSuppressLineNumbers = new RichEditCommandId(238);
		public static readonly RichEditCommandId ToggleParagraphSuppressHyphenation = new RichEditCommandId(239);
		public static readonly RichEditCommandId ShowLineNumberingForm = new RichEditCommandId(240);
		public static readonly RichEditCommandId ShowPageSetupForm = new RichEditCommandId(241);
		public static readonly RichEditCommandId ShowPasteSpecialForm = new RichEditCommandId(242);
		public static readonly RichEditCommandId CreateHyperlink = new RichEditCommandId(243);
		public static readonly RichEditCommandId EditHyperlink = new RichEditCommandId(244);
		public static readonly RichEditCommandId CreateBookmark = new RichEditCommandId(245);
		public static readonly RichEditCommandId ShowTablePropertiesForm = new RichEditCommandId(246);
		public static readonly RichEditCommandId ShowTablePropertiesFormMenuItem = new RichEditCommandId(247);
		public static readonly RichEditCommandId IncrementParagraphOutlineLevel = new RichEditCommandId(248);
		public static readonly RichEditCommandId DecrementParagraphOutlineLevel = new RichEditCommandId(249);
		public static readonly RichEditCommandId SetParagraphBodyTextLevel = new RichEditCommandId(250);
		public static readonly RichEditCommandId SetParagraphHeading1Level = new RichEditCommandId(251);
		public static readonly RichEditCommandId SetParagraphHeading2Level = new RichEditCommandId(252);
		public static readonly RichEditCommandId SetParagraphHeading3Level = new RichEditCommandId(253);
		public static readonly RichEditCommandId SetParagraphHeading4Level = new RichEditCommandId(254);
		public static readonly RichEditCommandId SetParagraphHeading5Level = new RichEditCommandId(255);
		public static readonly RichEditCommandId SetParagraphHeading6Level = new RichEditCommandId(256);
		public static readonly RichEditCommandId SetParagraphHeading7Level = new RichEditCommandId(257);
		public static readonly RichEditCommandId SetParagraphHeading8Level = new RichEditCommandId(258);
		public static readonly RichEditCommandId SetParagraphHeading9Level = new RichEditCommandId(259);
		public static readonly RichEditCommandId AddParagraphsToTableOfContents = new RichEditCommandId(260);
		public static readonly RichEditCommandId InsertTableOfContents = new RichEditCommandId(261);
		public static readonly RichEditCommandId InsertTableOfEquations = new RichEditCommandId(262);
		public static readonly RichEditCommandId InsertTableOfFigures = new RichEditCommandId(263);
		public static readonly RichEditCommandId InsertTableOfTables = new RichEditCommandId(264);
		public static readonly RichEditCommandId InsertTableOfFiguresPlaceholder = new RichEditCommandId(265);
		public static readonly RichEditCommandId InsertEquationsCaption = new RichEditCommandId(266);
		public static readonly RichEditCommandId InsertFiguresCaption = new RichEditCommandId(267);
		public static readonly RichEditCommandId InsertTablesCaption = new RichEditCommandId(268);
		public static readonly RichEditCommandId InsertCaptionPlaceholder = new RichEditCommandId(269);
		public static readonly RichEditCommandId UpdateTableOfContents = new RichEditCommandId(270);
		public static readonly RichEditCommandId FindForward = new RichEditCommandId(271);
		public static readonly RichEditCommandId FindBackward = new RichEditCommandId(272);
		public static readonly RichEditCommandId ReplaceForward = new RichEditCommandId(273);
		public static readonly RichEditCommandId ReplaceAllForward = new RichEditCommandId(274);
		public static readonly RichEditCommandId ShowColumnsSetupForm = new RichEditCommandId(275);
		public static readonly RichEditCommandId ToggleTableAutoFitPlaceholder = new RichEditCommandId(276);
		public static readonly RichEditCommandId ToggleTableAutoFitContents = new RichEditCommandId(277);
		public static readonly RichEditCommandId ToggleTableAutoFitWindow = new RichEditCommandId(278);
		public static readonly RichEditCommandId ToggleTableFixedColumnWidth = new RichEditCommandId(279);
		public static readonly RichEditCommandId UpdateFields = new RichEditCommandId(280);
		public static readonly RichEditCommandId ToggleTableAutoFitMenuPlaceholder = new RichEditCommandId(281);
		public static readonly RichEditCommandId SetFloatingObjectSquareTextWrapType = new RichEditCommandId(282);
		public static readonly RichEditCommandId SetFloatingObjectBehindTextWrapType = new RichEditCommandId(283);
		public static readonly RichEditCommandId SetFloatingObjectInFrontOfTextWrapType = new RichEditCommandId(284);
		public static readonly RichEditCommandId SetFloatingObjectThroughTextWrapType = new RichEditCommandId(285);
		public static readonly RichEditCommandId SetFloatingObjectTightTextWrapType = new RichEditCommandId(286);
		public static readonly RichEditCommandId SetFloatingObjectTopAndBottomTextWrapType = new RichEditCommandId(287);
		public static readonly RichEditCommandId SetFloatingObjectTopLeftAlignment = new RichEditCommandId(288);
		public static readonly RichEditCommandId SetFloatingObjectTopCenterAlignment = new RichEditCommandId(289);
		public static readonly RichEditCommandId SetFloatingObjectTopRightAlignment = new RichEditCommandId(290);
		public static readonly RichEditCommandId SetFloatingObjectMiddleLeftAlignment = new RichEditCommandId(291);
		public static readonly RichEditCommandId SetFloatingObjectMiddleCenterAlignment = new RichEditCommandId(292);
		public static readonly RichEditCommandId SetFloatingObjectMiddleRightAlignment = new RichEditCommandId(293);
		public static readonly RichEditCommandId SetFloatingObjectBottomLeftAlignment = new RichEditCommandId(294);
		public static readonly RichEditCommandId SetFloatingObjectBottomCenterAlignment = new RichEditCommandId(295);
		public static readonly RichEditCommandId SetFloatingObjectBottomRightAlignment = new RichEditCommandId(296);
		public static readonly RichEditCommandId ChangeFloatingObjectTextWrapType = new RichEditCommandId(297);
		public static readonly RichEditCommandId ChangeFloatingObjectAlignment = new RichEditCommandId(298);
		public static readonly RichEditCommandId FloatingObjectBringForwardPlaceholder = new RichEditCommandId(299);
		public static readonly RichEditCommandId FloatingObjectBringForward = new RichEditCommandId(300);
		public static readonly RichEditCommandId FloatingObjectBringToFront = new RichEditCommandId(301);
		public static readonly RichEditCommandId FloatingObjectBringInFrontOfText = new RichEditCommandId(302);
		public static readonly RichEditCommandId FloatingObjectSendBackward = new RichEditCommandId(303);
		public static readonly RichEditCommandId FloatingObjectSendToBack = new RichEditCommandId(304);
		public static readonly RichEditCommandId FloatingObjectSendBehindText = new RichEditCommandId(305);
		public static readonly RichEditCommandId FloatingObjectSendBackwardPlaceholder = new RichEditCommandId(306);
		public static readonly RichEditCommandId SelectUpperLevelObject = new RichEditCommandId(307);
		public static readonly RichEditCommandId ChangeFloatingObjectFillColor = new RichEditCommandId(308);
		public static readonly RichEditCommandId ChangeFloatingObjectOutlineColor = new RichEditCommandId(309);
		public static readonly RichEditCommandId ChangeFloatingObjectOutlineWeight = new RichEditCommandId(310);
		public static readonly RichEditCommandId ShowPageMarginsSetupForm = new RichEditCommandId(311);
		public static readonly RichEditCommandId ShowPagePaperSetupForm = new RichEditCommandId(312);
		public static readonly RichEditCommandId ShowEditStyleForm = new RichEditCommandId(313);
		public static readonly RichEditCommandId InsertTextBox = new RichEditCommandId(314);
		public static readonly RichEditCommandId ShowFloatingObjectLayoutOptionsForm = new RichEditCommandId(315);
		public static readonly RichEditCommandId InsertFloatingPicture = new RichEditCommandId(316);
		public static readonly RichEditCommandId ChangeParagraphBackColor = new RichEditCommandId(317);
		public static readonly RichEditCommandId ShowTableOptionsForm = new RichEditCommandId(318);
		public static readonly RichEditCommandId ChangePageColor = new RichEditCommandId(319);
		public static readonly RichEditCommandId ChangeTableCellsBorderLineWeight = new RichEditCommandId(320);
		public static readonly RichEditCommandId ChangeTableStyle = new RichEditCommandId(321);
		public static readonly RichEditCommandId UpdateTableOfFigures = new RichEditCommandId(322);
		public static readonly RichEditCommandId ShowTOCForm = new RichEditCommandId(323);
		public static readonly RichEditCommandId ChangeLanguage = new RichEditCommandId(324);
		public static readonly RichEditCommandId ShowLanguageForm = new RichEditCommandId(325);
		public static readonly RichEditCommandId ChangeNoProof = new RichEditCommandId(326);
		public static readonly RichEditCommandId CapitalizeEachWordTextCase = new RichEditCommandId(327);
		public static readonly RichEditCommandId ChangeDoubleFontSize = new RichEditCommandId(328);
		public static readonly RichEditCommandId ToggleOvertype = new RichEditCommandId(329);
		public static readonly RichEditCommandId InnerReplace = new RichEditCommandId(330);
		public static readonly RichEditCommandId ViewComments = new RichEditCommandId(331);
		public static readonly RichEditCommandId ShowReviewingPane = new RichEditCommandId(332);
		public static readonly RichEditCommandId ToggleReviewingPane = new RichEditCommandId(333);
		public static readonly RichEditCommandId Reviewers = new RichEditCommandId(334);
		public static readonly RichEditCommandId NewComment = new RichEditCommandId(335);
		public static readonly RichEditCommandId DeleteCommentsPlaceholder = new RichEditCommandId(336);
		public static readonly RichEditCommandId DeleteOneComment = new RichEditCommandId(337);
		public static readonly RichEditCommandId DeleteAllCommentsShown = new RichEditCommandId(338);
		public static readonly RichEditCommandId DeleteAllComments = new RichEditCommandId(339);
		public static readonly RichEditCommandId PreviousComment = new RichEditCommandId(340);
		public static readonly RichEditCommandId NextComment = new RichEditCommandId(341);
		public static readonly RichEditCommandId InsertText = new RichEditCommandId(342);
		public static readonly RichEditCommandId OvertypeText = new RichEditCommandId(343);
		public static readonly RichEditCommandId InsertPictureInner = new RichEditCommandId(344);
		public static readonly RichEditCommandId InsertTableInner = new RichEditCommandId(345);
		public static readonly RichEditCommandId InsertSymbol = new RichEditCommandId(346);
		public static readonly RichEditCommandId ToggleFirstRow = new RichEditCommandId(347);
		public static readonly RichEditCommandId ToggleLastRow = new RichEditCommandId(348);
		public static readonly RichEditCommandId ToggleBandedRows = new RichEditCommandId(349);
		public static readonly RichEditCommandId ToggleFirstColumn = new RichEditCommandId(350);
		public static readonly RichEditCommandId ToggleLastColumn = new RichEditCommandId(351);
		public static readonly RichEditCommandId ToggleBandedColumns = new RichEditCommandId(352);
		public static readonly RichEditCommandId ToolsFloatingPictureCommandGroup = new RichEditCommandId(353);
		public static readonly RichEditCommandId ToolsTableCommandGroup = new RichEditCommandId(354);
		public static readonly RichEditCommandId ToolsHeaderFooterCommandGroup = new RichEditCommandId(355);
		public static readonly RichEditCommandId DeleteNonEmptySelection = new RichEditCommandId(356);
		public static readonly RichEditCommandId ImeUndo = new RichEditCommandId(357);
		public static readonly RichEditCommandId ResetMerging = new RichEditCommandId(358);
		public static readonly RichEditCommandId CollapseOrExpandFormulaBar = new RichEditCommandId(400);
		public static readonly RichEditCommandId InsertPageBreak2 = new RichEditCommandId(401);
		public static readonly RichEditCommandId OpenHyperlink = new RichEditCommandId(402);
		public static readonly RichEditCommandId RemoveHyperlink = new RichEditCommandId(403);
		public static readonly RichEditCommandId FitToPage = new RichEditCommandId(404);
		public static readonly RichEditCommandId FitWidth = new RichEditCommandId(405);
		public static readonly RichEditCommandId FitHeight = new RichEditCommandId(406);
		public static readonly RichEditCommandId ReplaceMisspelling = new RichEditCommandId(407);
		public static readonly RichEditCommandId IgnoreMisspelling = new RichEditCommandId(408);
		public static readonly RichEditCommandId IgnoreAllMisspellings = new RichEditCommandId(409);
		public static readonly RichEditCommandId AddWordToDictionary = new RichEditCommandId(410);
		public static readonly RichEditCommandId DeleteRepeatedWord = new RichEditCommandId(411);
		public static readonly RichEditCommandId ChangeCase = new RichEditCommandId(412);
		public static readonly RichEditCommandId ToggleFieldLockedCommand = new RichEditCommandId(413);
		public static readonly RichEditCommandId LockFieldCommand = new RichEditCommandId(414);
		public static readonly RichEditCommandId UnlockFieldCommand = new RichEditCommandId(415);
		public static readonly RichEditCommandId ResizeInlinePicture = new RichEditCommandId(416);
		public static readonly RichEditCommandId RotateInlinePicture = new RichEditCommandId(417);
#if SL
		public static readonly RichEditCommandId BrowserPrint = new RichEditCommandId(800);
		public static readonly RichEditCommandId BrowserPrintPreview = new RichEditCommandId(801);
#endif
		readonly int m_value;
		public RichEditCommandId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is RichEditCommandId) && (this.m_value == ((RichEditCommandId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(RichEditCommandId id1, RichEditCommandId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(RichEditCommandId id1, RichEditCommandId id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<RichEditCommandId> Members
		int IConvertToInt<RichEditCommandId>.ToInt() {
			return m_value;
		}
		RichEditCommandId IConvertToInt<RichEditCommandId>.FromInt(int value) {
			return new RichEditCommandId(value);
		}
		#endregion
		#region IEquatable<RichEditCommandId> Members
		public bool Equals(RichEditCommandId other) {
			return this.m_value == other.m_value;
		}
		#endregion
	}
	#endregion
	#region RichEditCommandBase (abstract class)
	public abstract class RichEditCommandBase<TLocalizedStringId> : ControlCommand<IRichEditControl, RichEditCommandId, TLocalizedStringId> where TLocalizedStringId : struct {
		protected RichEditCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal DocumentModel DocumentModel { get { return DocumentServer.DocumentModel; } }
		protected internal RichEditView ActiveView { get { return InnerControl.ActiveView; } }
		protected internal RichEditViewType ActiveViewType { get { return InnerControl.ActiveViewType; } }
		protected internal RichEditControlOptionsBase Options { get { return DocumentServer.Options; } }
		protected internal PieceTable ActivePieceTable { get { return DocumentModel.ActivePieceTable; } }
		protected internal bool IsContentEditable { get { return DocumentServer.IsEditable; } }
		protected internal InnerRichEditControl InnerControl { get { return Control.InnerControl; } }
		protected internal InnerRichEditDocumentServer DocumentServer { get { return Control.InnerDocumentServer; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.None; } }
		#endregion
		protected internal virtual void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option, bool additionEnabledCondition) {
			ApplyCommandsRestriction(state, option);
			state.Enabled = state.Enabled && additionEnabledCondition;
		}
		protected internal virtual void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option) {
			state.Enabled = CheckDocumentCapability(option);
			state.Visible = option != DocumentCapability.Hidden;
		}
		protected internal virtual void ApplyCommandRestrictionOnEditableControl(ICommandUIState state, DocumentCapability option, bool additionEnabledCondition) {
			ApplyCommandRestrictionOnEditableControl(state, option);
			state.Enabled = state.Enabled && additionEnabledCondition;
		}
		protected internal virtual void ApplyCommandRestrictionOnEditableControl(ICommandUIState state, DocumentCapability option) {
			state.Enabled = CheckIsContentEditable() && CheckDocumentCapability(option);
			state.Visible = option != DocumentCapability.Hidden;
		}
		protected virtual bool CheckIsContentEditable() {
			return IsContentEditable;
		}
		bool CheckDocumentCapability(DocumentCapability option) {
			return option != DocumentCapability.Disabled && option != DocumentCapability.Hidden;
		}
		protected internal virtual bool CanEditSelection() {
			return ActivePieceTable.CanEditSelection();
		}
		protected internal virtual bool CanEditTable(Table table) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			PieceTable pieceTable = table.PieceTable;
			ParagraphIndex startParagraphIndex = table.FirstRow.FirstCell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = table.LastRow.LastCell.EndParagraphIndex;
			Paragraph startParagraph = pieceTable.Paragraphs[startParagraphIndex];
			Paragraph endParagraph = pieceTable.Paragraphs[endParagraphIndex];
			return table.PieceTable.CanEditRange(startParagraph.LogPosition, endParagraph.EndLogPosition);
		}
		protected internal virtual void ApplyDocumentProtectionToSelectedCharacters(ICommandUIState state) {
			if (state.Enabled)
				state.Enabled = CanEditSelection();
		}
		protected internal virtual void ApplyDocumentProtectionToSelectedParagraphs(ICommandUIState state) {
			if (state.Enabled) {
				if (DocumentModel.IsDocumentProtectionEnabled) {
					List<SelectionItem> items = ExtendSelectionToParagraphBoundary();
					state.Enabled = ActivePieceTable.CanEditSelectionItems(items);
				}
			}
		}
		protected internal virtual void ApplyDocumentProtectionToTable(ICommandUIState state, Table table) {
			if (state.Enabled)
				state.Enabled = CanEditTable(table);
		}
		protected internal virtual void ApplyDocumentProtectionToSelectedSections(ICommandUIState state) {
			if (state.Enabled) {
				if (DocumentModel.IsDocumentProtectionEnabled) {
					SelectionItem item = ExtendSelectionToSectionBoundary();
					state.Enabled = DocumentModel.MainPieceTable.CanEditRange(item.NormalizedStart, item.NormalizedEnd);
				}
			}
		}
		List<SelectionItem> ExtendSelectionToParagraphBoundary() {
			List<SelectionItem> result = new List<SelectionItem>();
			List<SelectionItem> items = this.DocumentModel.Selection.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				result.Add(ExtendToParagraphBoundary(items[i]));
			return result;
		}
		SelectionItem ExtendToParagraphBoundary(SelectionItem selectionItem) {
			PieceTable pieceTable = selectionItem.PieceTable;
			SelectionItem result = new SelectionItem(pieceTable);
			result.Start = pieceTable.Paragraphs[selectionItem.Interval.NormalizedStart.ParagraphIndex].LogPosition;
			Paragraph lastParagraph = pieceTable.Paragraphs[selectionItem.Interval.NormalizedEnd.ParagraphIndex];
			if (lastParagraph.Index >= new ParagraphIndex(pieceTable.Paragraphs.Count - 1))
				result.End = lastParagraph.LogPosition + lastParagraph.Length - 1;
			else
				result.End = lastParagraph.LogPosition + lastParagraph.Length;
			return result;
		}
		SelectionItem ExtendSelectionToSectionBoundary() {
			Selection selection = this.DocumentModel.Selection;
			SectionIndex firstSectionIndex = DocumentModel.FindSectionIndex(selection.NormalizedStart);
			if (firstSectionIndex < new SectionIndex(0))
				return selection.Items[0];
			SectionIndex lastSectionIndex = DocumentModel.FindSectionIndex(selection.NormalizedEnd);
			if (lastSectionIndex < new SectionIndex(0))
				return selection.Items[0];
			Section firstSection = DocumentModel.Sections[firstSectionIndex];
			Section lastSection = DocumentModel.Sections[lastSectionIndex];
			PieceTable mainPieceTable = DocumentModel.MainPieceTable;
			SelectionItem result = new SelectionItem(mainPieceTable);
			result.Start = mainPieceTable.Paragraphs[firstSection.FirstParagraphIndex].LogPosition;
			Paragraph lastParagraph = mainPieceTable.Paragraphs[lastSection.LastParagraphIndex];
			result.Start = lastParagraph.LogPosition + lastParagraph.Length;
			return result;
		}
		protected internal virtual void CheckExecutedAtUIThread() {
			ActiveView.CheckExecutedAtUIThread();
		}
	}
	#endregion
	#region RichEditCommand (abstract class)
	public abstract class RichEditCommand : RichEditCommandBase<XtraRichEditStringId> {
		static RichEditCommand() {
			XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InternalError);
		}
		protected RichEditCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected override XtraLocalizer<XtraRichEditStringId> Localizer { get { return XtraRichEditLocalizer.Active; } }
		protected override string ImageResourcePrefix { get { return "DevExpress.XtraRichEdit.Images"; } }
		protected override Assembly ImageResourceAssembly { get { return typeof(RichEditCommand).GetAssembly(); } }
		#endregion
	}
	#endregion
	public interface IPlaceholderCommand {
	}
}
