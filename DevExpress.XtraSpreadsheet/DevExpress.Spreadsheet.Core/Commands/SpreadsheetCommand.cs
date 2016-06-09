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
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Compatibility.System;
#if !SL
using System.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SpreadsheetCommandId
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct SpreadsheetCommandId : IConvertToInt<SpreadsheetCommandId>, IEquatable<SpreadsheetCommandId> {
		public static readonly SpreadsheetCommandId None = new SpreadsheetCommandId(0);
		public static readonly SpreadsheetCommandId FileOpen = new SpreadsheetCommandId(1);
		public static readonly SpreadsheetCommandId FileSave = new SpreadsheetCommandId(2);
		public static readonly SpreadsheetCommandId FileSaveAs = new SpreadsheetCommandId(3);
		public static readonly SpreadsheetCommandId FileNew = new SpreadsheetCommandId(4);
		public static readonly SpreadsheetCommandId FileQuickPrint = new SpreadsheetCommandId(5);
		public static readonly SpreadsheetCommandId FilePrint = new SpreadsheetCommandId(6);
		public static readonly SpreadsheetCommandId FilePrintPreview = new SpreadsheetCommandId(7);
		public static readonly SpreadsheetCommandId FileUndo = new SpreadsheetCommandId(8);
		public static readonly SpreadsheetCommandId FileRedo = new SpreadsheetCommandId(9);
		public static readonly SpreadsheetCommandId FileOpenSilently = new SpreadsheetCommandId(10);
		public static readonly SpreadsheetCommandId FileSaveAsSilently = new SpreadsheetCommandId(11);
		public static readonly SpreadsheetCommandId FileShowDocumentProperties = new SpreadsheetCommandId(12);
		public static readonly SpreadsheetCommandId FormatFontBold = new SpreadsheetCommandId(100);
		public static readonly SpreadsheetCommandId FormatFontItalic = new SpreadsheetCommandId(101);
		public static readonly SpreadsheetCommandId FormatFontUnderline = new SpreadsheetCommandId(102);
		public static readonly SpreadsheetCommandId FormatFontStrikeout = new SpreadsheetCommandId(103);
		public static readonly SpreadsheetCommandId FormatFontName = new SpreadsheetCommandId(104);
		public static readonly SpreadsheetCommandId FormatFontSize = new SpreadsheetCommandId(105);
		public static readonly SpreadsheetCommandId FormatIncreaseFontSize = new SpreadsheetCommandId(106);
		public static readonly SpreadsheetCommandId FormatDecreaseFontSize = new SpreadsheetCommandId(107);
		public static readonly SpreadsheetCommandId FormatFontColor = new SpreadsheetCommandId(108);
		public static readonly SpreadsheetCommandId FormatFillColor = new SpreadsheetCommandId(109);
		public static readonly SpreadsheetCommandId FormatAlignmentLeft = new SpreadsheetCommandId(110);
		public static readonly SpreadsheetCommandId FormatAlignmentCenter = new SpreadsheetCommandId(111);
		public static readonly SpreadsheetCommandId FormatAlignmentRight = new SpreadsheetCommandId(112);
		public static readonly SpreadsheetCommandId FormatAlignmentTop = new SpreadsheetCommandId(113);
		public static readonly SpreadsheetCommandId FormatAlignmentMiddle = new SpreadsheetCommandId(114);
		public static readonly SpreadsheetCommandId FormatAlignmentBottom = new SpreadsheetCommandId(115);
		public static readonly SpreadsheetCommandId FormatWrapText = new SpreadsheetCommandId(116);
		public static readonly SpreadsheetCommandId FormatIncreaseIndent = new SpreadsheetCommandId(117);
		public static readonly SpreadsheetCommandId FormatDecreaseIndent = new SpreadsheetCommandId(118);
		public static readonly SpreadsheetCommandId FormatBorderLineColor = new SpreadsheetCommandId(119);
		public static readonly SpreadsheetCommandId FormatBorderLineStyle = new SpreadsheetCommandId(120);
		public static readonly SpreadsheetCommandId FormatThickBorder = new SpreadsheetCommandId(121);
		public static readonly SpreadsheetCommandId FormatOutsideBorders = new SpreadsheetCommandId(122);
		public static readonly SpreadsheetCommandId FormatLeftBorder = new SpreadsheetCommandId(123);
		public static readonly SpreadsheetCommandId FormatRightBorder = new SpreadsheetCommandId(124);
		public static readonly SpreadsheetCommandId FormatTopBorder = new SpreadsheetCommandId(125);
		public static readonly SpreadsheetCommandId FormatBottomBorder = new SpreadsheetCommandId(126);
		public static readonly SpreadsheetCommandId FormatBottomDoubleBorder = new SpreadsheetCommandId(127);
		public static readonly SpreadsheetCommandId FormatBottomThickBorder = new SpreadsheetCommandId(128);
		public static readonly SpreadsheetCommandId FormatTopAndBottomBorder = new SpreadsheetCommandId(129);
		public static readonly SpreadsheetCommandId FormatTopAndThickBottomBorder = new SpreadsheetCommandId(130);
		public static readonly SpreadsheetCommandId FormatTopAndDoubleBottomBorder = new SpreadsheetCommandId(131);
		public static readonly SpreadsheetCommandId FormatAllBorders = new SpreadsheetCommandId(132);
		public static readonly SpreadsheetCommandId FormatNoBorders = new SpreadsheetCommandId(133);
		public static readonly SpreadsheetCommandId FormatBordersCommandGroup = new SpreadsheetCommandId(134);
		public static readonly SpreadsheetCommandId FormatCellLocked = new SpreadsheetCommandId(135);
		public static readonly SpreadsheetCommandId FormatNumber = new SpreadsheetCommandId(150);
		public static readonly SpreadsheetCommandId FormatNumberGeneral = new SpreadsheetCommandId(151);
		public static readonly SpreadsheetCommandId FormatNumberDecimal = new SpreadsheetCommandId(152);
		public static readonly SpreadsheetCommandId FormatNumberPercent = new SpreadsheetCommandId(153);
		public static readonly SpreadsheetCommandId FormatNumberPercentage = new SpreadsheetCommandId(154);
		public static readonly SpreadsheetCommandId FormatNumberScientific = new SpreadsheetCommandId(155);
		public static readonly SpreadsheetCommandId FormatNumberFraction = new SpreadsheetCommandId(156);
		public static readonly SpreadsheetCommandId FormatNumberAccounting = new SpreadsheetCommandId(157);
		public static readonly SpreadsheetCommandId FormatNumberAccountingRegular = new SpreadsheetCommandId(158);
		public static readonly SpreadsheetCommandId FormatNumberAccountingCurrency = new SpreadsheetCommandId(159);
		public static readonly SpreadsheetCommandId FormatNumberShortDate = new SpreadsheetCommandId(160);
		public static readonly SpreadsheetCommandId FormatNumberLongDate = new SpreadsheetCommandId(161);
		public static readonly SpreadsheetCommandId FormatNumberText = new SpreadsheetCommandId(162);
		public static readonly SpreadsheetCommandId FormatNumberTime = new SpreadsheetCommandId(163);
		public static readonly SpreadsheetCommandId FormatNumberPredefined4 = new SpreadsheetCommandId(164);
		public static readonly SpreadsheetCommandId FormatNumberPredefined8 = new SpreadsheetCommandId(165);
		public static readonly SpreadsheetCommandId FormatNumberPredefined15 = new SpreadsheetCommandId(166);
		public static readonly SpreadsheetCommandId FormatNumberPredefined18 = new SpreadsheetCommandId(167);
		public static readonly SpreadsheetCommandId FormatNumberAccountingUS = new SpreadsheetCommandId(168);
		public static readonly SpreadsheetCommandId FormatNumberAccountingUK = new SpreadsheetCommandId(169);
		public static readonly SpreadsheetCommandId FormatNumberAccountingPRC = new SpreadsheetCommandId(170);
		public static readonly SpreadsheetCommandId FormatNumberAccountingEuro = new SpreadsheetCommandId(171);
		public static readonly SpreadsheetCommandId FormatNumberAccountingSwiss = new SpreadsheetCommandId(172);
		public static readonly SpreadsheetCommandId FormatNumberAccountingCommandGroup = new SpreadsheetCommandId(173);
		public static readonly SpreadsheetCommandId FormatNumberIncreaseDecimal = new SpreadsheetCommandId(174);
		public static readonly SpreadsheetCommandId FormatNumberDecreaseDecimal = new SpreadsheetCommandId(175);
		public static readonly SpreadsheetCommandId FormatCellStyle = new SpreadsheetCommandId(176);
		public static readonly SpreadsheetCommandId FormatAsTable = new SpreadsheetCommandId(177);
		public static readonly SpreadsheetCommandId FormatClearCommandGroup = new SpreadsheetCommandId(200);
		public static readonly SpreadsheetCommandId FormatClearAll = new SpreadsheetCommandId(201);
		public static readonly SpreadsheetCommandId FormatClearFormats = new SpreadsheetCommandId(202);
		public static readonly SpreadsheetCommandId FormatClearContents = new SpreadsheetCommandId(203);
		public static readonly SpreadsheetCommandId FormatClearContentsContextMenuItem = new SpreadsheetCommandId(204);
		public static readonly SpreadsheetCommandId FormatClearComments = new SpreadsheetCommandId(205);
		public static readonly SpreadsheetCommandId FormatClearHyperlinks = new SpreadsheetCommandId(206);
		public static readonly SpreadsheetCommandId FormatRemoveHyperlinks = new SpreadsheetCommandId(207);
		public static readonly SpreadsheetCommandId EditingAutoSumCommandGroup = new SpreadsheetCommandId(208);
		public static readonly SpreadsheetCommandId EditingMergeAndCenterCells = new SpreadsheetCommandId(209);
		public static readonly SpreadsheetCommandId EditingMergeCellsAcross = new SpreadsheetCommandId(210);
		public static readonly SpreadsheetCommandId EditingMergeCells = new SpreadsheetCommandId(211);
		public static readonly SpreadsheetCommandId EditingUnmergeCells = new SpreadsheetCommandId(212);
		public static readonly SpreadsheetCommandId EditingMergeCellsCommandGroup = new SpreadsheetCommandId(213);
		public static readonly SpreadsheetCommandId EditingFillDown = new SpreadsheetCommandId(214);
		public static readonly SpreadsheetCommandId EditingFillUp = new SpreadsheetCommandId(215);
		public static readonly SpreadsheetCommandId EditingFillLeft = new SpreadsheetCommandId(216);
		public static readonly SpreadsheetCommandId EditingFillRight = new SpreadsheetCommandId(217);
		public static readonly SpreadsheetCommandId EditingFillCommandGroup = new SpreadsheetCommandId(218);
		public static readonly SpreadsheetCommandId EditingSortAndFilterCommandGroup = new SpreadsheetCommandId(219);
		public static readonly SpreadsheetCommandId EditingFindAndSelectCommandGroup = new SpreadsheetCommandId(220);
		public static readonly SpreadsheetCommandId EditingFind = new SpreadsheetCommandId(221);
		public static readonly SpreadsheetCommandId EditingReplace = new SpreadsheetCommandId(222);
		public static readonly SpreadsheetCommandId EditingSelectFormulas = new SpreadsheetCommandId(223);
		public static readonly SpreadsheetCommandId EditingSelectComments = new SpreadsheetCommandId(224);
		public static readonly SpreadsheetCommandId EditingSelectConditionalFormatting = new SpreadsheetCommandId(225);
		public static readonly SpreadsheetCommandId EditingSelectConstants = new SpreadsheetCommandId(226);
		public static readonly SpreadsheetCommandId EditingSelectDataValidation = new SpreadsheetCommandId(227);
		public static readonly SpreadsheetCommandId FunctionsInsertSpecificFunction = new SpreadsheetCommandId(400);
		public static readonly SpreadsheetCommandId FunctionsFinancialCommandGroup = new SpreadsheetCommandId(401);
		public static readonly SpreadsheetCommandId FunctionsLogicalCommandGroup = new SpreadsheetCommandId(402);
		public static readonly SpreadsheetCommandId FunctionsTextCommandGroup = new SpreadsheetCommandId(403);
		public static readonly SpreadsheetCommandId FunctionsDateAndTimeCommandGroup = new SpreadsheetCommandId(404);
		public static readonly SpreadsheetCommandId FunctionsLookupAndReferenceCommandGroup = new SpreadsheetCommandId(405);
		public static readonly SpreadsheetCommandId FunctionsMathAndTrigonometryCommandGroup = new SpreadsheetCommandId(406);
		public static readonly SpreadsheetCommandId FunctionsMoreCommandGroup = new SpreadsheetCommandId(407);
		public static readonly SpreadsheetCommandId FunctionsStatisticalCommandGroup = new SpreadsheetCommandId(408);
		public static readonly SpreadsheetCommandId FunctionsEngineeringCommandGroup = new SpreadsheetCommandId(409);
		public static readonly SpreadsheetCommandId FunctionsCubeCommandGroup = new SpreadsheetCommandId(410);
		public static readonly SpreadsheetCommandId FunctionsInformationCommandGroup = new SpreadsheetCommandId(411);
		public static readonly SpreadsheetCommandId FunctionsCompatibilityCommandGroup = new SpreadsheetCommandId(412);
		public static readonly SpreadsheetCommandId FunctionsAutoSumCommandGroup = new SpreadsheetCommandId(413);
		public static readonly SpreadsheetCommandId FunctionsInsertSum = new SpreadsheetCommandId(414);
		public static readonly SpreadsheetCommandId FunctionsInsertAverage = new SpreadsheetCommandId(415);
		public static readonly SpreadsheetCommandId FunctionsInsertCountNumbers = new SpreadsheetCommandId(416);
		public static readonly SpreadsheetCommandId FunctionsInsertMax = new SpreadsheetCommandId(417);
		public static readonly SpreadsheetCommandId FunctionsInsertMin = new SpreadsheetCommandId(418);
		public static readonly SpreadsheetCommandId FunctionsWebCommandGroup = new SpreadsheetCommandId(419);
		public static readonly SpreadsheetCommandId FormulasCalculateFull = new SpreadsheetCommandId(420);
		public static readonly SpreadsheetCommandId FormulasCalculateFullRebuild = new SpreadsheetCommandId(421);
		public static readonly SpreadsheetCommandId FormulasCalculateNow = new SpreadsheetCommandId(422);
		public static readonly SpreadsheetCommandId FormulasCalculateSheet = new SpreadsheetCommandId(423);
		public static readonly SpreadsheetCommandId FormulasCalculationOptionsCommandGroup = new SpreadsheetCommandId(424);
		public static readonly SpreadsheetCommandId FormulasCalculationModeAutomatic = new SpreadsheetCommandId(425);
		public static readonly SpreadsheetCommandId FormulasCalculationModeManual = new SpreadsheetCommandId(426);
		public static readonly SpreadsheetCommandId FormulasDefineNameCommandGroup = new SpreadsheetCommandId(427);
		public static readonly SpreadsheetCommandId FormulasDefineNameCommand = new SpreadsheetCommandId(428);
		public static readonly SpreadsheetCommandId FormulasShowNameManager = new SpreadsheetCommandId(429);
		public static readonly SpreadsheetCommandId FormulasCreateDefinedNamesFromSelection = new SpreadsheetCommandId(430);
		public static readonly SpreadsheetCommandId FormulasInsertDefinedNameCommandGroup = new SpreadsheetCommandId(431);
		public static readonly SpreadsheetCommandId FormulasInsertDefinedName = new SpreadsheetCommandId(432);
		public static readonly SpreadsheetCommandId InsertFunction = new SpreadsheetCommandId(433);
		public static readonly SpreadsheetCommandId InplaceBeginEdit = new SpreadsheetCommandId(450);
		public static readonly SpreadsheetCommandId InplaceEndEdit = new SpreadsheetCommandId(451);
		public static readonly SpreadsheetCommandId InplaceCancelEdit = new SpreadsheetCommandId(452);
		public static readonly SpreadsheetCommandId InplaceToggleEditMode = new SpreadsheetCommandId(453);
		public static readonly SpreadsheetCommandId InplaceEndEditEnterToMultipleCells = new SpreadsheetCommandId(454);
		public static readonly SpreadsheetCommandId InplaceEndEditEnterArrayFormula = new SpreadsheetCommandId(455);
		public static readonly SpreadsheetCommandId SelectionMoveLeft = new SpreadsheetCommandId(500);
		public static readonly SpreadsheetCommandId SelectionMoveRight = new SpreadsheetCommandId(501);
		public static readonly SpreadsheetCommandId SelectionMoveUp = new SpreadsheetCommandId(502);
		public static readonly SpreadsheetCommandId SelectionMoveDown = new SpreadsheetCommandId(503);
		public static readonly SpreadsheetCommandId SelectionMoveToLeftColumn = new SpreadsheetCommandId(504);
		public static readonly SpreadsheetCommandId SelectionMoveLeftToDataEdge = new SpreadsheetCommandId(505);
		public static readonly SpreadsheetCommandId SelectionMoveRightToDataEdge = new SpreadsheetCommandId(506);
		public static readonly SpreadsheetCommandId SelectionMoveUpToDataEdge = new SpreadsheetCommandId(507);
		public static readonly SpreadsheetCommandId SelectionMoveDownToDataEdge = new SpreadsheetCommandId(508);
		public static readonly SpreadsheetCommandId SelectionMoveToTopLeftCell = new SpreadsheetCommandId(509);
		public static readonly SpreadsheetCommandId SelectionMoveToLastUsedCell = new SpreadsheetCommandId(510);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellRight = new SpreadsheetCommandId(511);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellLeft = new SpreadsheetCommandId(512);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellDown = new SpreadsheetCommandId(513);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellUp = new SpreadsheetCommandId(514);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellToNextCorner = new SpreadsheetCommandId(515);
		public static readonly SpreadsheetCommandId SelectionNextRange = new SpreadsheetCommandId(516);
		public static readonly SpreadsheetCommandId SelectionPreviousRange = new SpreadsheetCommandId(517);
		public static readonly SpreadsheetCommandId SelectActiveCell = new SpreadsheetCommandId(518);
		public static readonly SpreadsheetCommandId SelectActiveColumn = new SpreadsheetCommandId(519);
		public static readonly SpreadsheetCommandId SelectActiveRow = new SpreadsheetCommandId(520);
		public static readonly SpreadsheetCommandId SelectAll = new SpreadsheetCommandId(521);
		public static readonly SpreadsheetCommandId SelectionExpandLeft = new SpreadsheetCommandId(522);
		public static readonly SpreadsheetCommandId SelectionExpandRight = new SpreadsheetCommandId(523);
		public static readonly SpreadsheetCommandId SelectionExpandUp = new SpreadsheetCommandId(524);
		public static readonly SpreadsheetCommandId SelectionExpandDown = new SpreadsheetCommandId(525);
		public static readonly SpreadsheetCommandId SelectionExpandToLeftColumn = new SpreadsheetCommandId(526);
		public static readonly SpreadsheetCommandId SelectionExpandLeftToDataEdge = new SpreadsheetCommandId(527);
		public static readonly SpreadsheetCommandId SelectionExpandRightToDataEdge = new SpreadsheetCommandId(528);
		public static readonly SpreadsheetCommandId SelectionExpandUpToDataEdge = new SpreadsheetCommandId(529);
		public static readonly SpreadsheetCommandId SelectionExpandDownToDataEdge = new SpreadsheetCommandId(530);
		public static readonly SpreadsheetCommandId SelectionExpandToTopLeftCell = new SpreadsheetCommandId(531);
		public static readonly SpreadsheetCommandId SelectionExpandToLastUsedCell = new SpreadsheetCommandId(532);
		public static readonly SpreadsheetCommandId SelectionMovePageDown = new SpreadsheetCommandId(533);
		public static readonly SpreadsheetCommandId SelectionMovePageUp = new SpreadsheetCommandId(534);
		public static readonly SpreadsheetCommandId SelectionExpandPageDown = new SpreadsheetCommandId(535);
		public static readonly SpreadsheetCommandId SelectionExpandPageUp = new SpreadsheetCommandId(536);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellOnEnterPress = new SpreadsheetCommandId(537);
		public static readonly SpreadsheetCommandId SelectionMoveActiveCellOnEnterPressReverse = new SpreadsheetCommandId(538);
		public static readonly SpreadsheetCommandId ViewZoomIn = new SpreadsheetCommandId(600);
		public static readonly SpreadsheetCommandId ViewZoomOut = new SpreadsheetCommandId(601);
		public static readonly SpreadsheetCommandId ViewZoom100Percent = new SpreadsheetCommandId(602);
		public static readonly SpreadsheetCommandId ViewShowGridlines = new SpreadsheetCommandId(603);
		public static readonly SpreadsheetCommandId ViewShowHeadings = new SpreadsheetCommandId(604);
		public static readonly SpreadsheetCommandId ViewFreezePanesCommandGroup = new SpreadsheetCommandId(605);
		public static readonly SpreadsheetCommandId ViewFreezePanes = new SpreadsheetCommandId(606);
		public static readonly SpreadsheetCommandId ViewUnfreezePanes = new SpreadsheetCommandId(607);
		public static readonly SpreadsheetCommandId ViewFreezeTopRow = new SpreadsheetCommandId(608);
		public static readonly SpreadsheetCommandId ViewFreezeFirstColumn = new SpreadsheetCommandId(609);
		public static readonly SpreadsheetCommandId ViewShowFormulas = new SpreadsheetCommandId(610);
		public static readonly SpreadsheetCommandId PageSetupMarginsNormal = new SpreadsheetCommandId(650);
		public static readonly SpreadsheetCommandId PageSetupMarginsNarrow = new SpreadsheetCommandId(651);
		public static readonly SpreadsheetCommandId PageSetupMarginsWide = new SpreadsheetCommandId(652);
		public static readonly SpreadsheetCommandId PageSetupMarginsCommandGroup = new SpreadsheetCommandId(653);
		public static readonly SpreadsheetCommandId PageSetupOrientationPortrait = new SpreadsheetCommandId(654);
		public static readonly SpreadsheetCommandId PageSetupOrientationLandscape = new SpreadsheetCommandId(655);
		public static readonly SpreadsheetCommandId PageSetupOrientationCommandGroup = new SpreadsheetCommandId(656);
		public static readonly SpreadsheetCommandId PageSetupSetPaperKind = new SpreadsheetCommandId(657);
		public static readonly SpreadsheetCommandId PageSetupPaperKindCommandGroup = new SpreadsheetCommandId(658);
		public static readonly SpreadsheetCommandId PageSetupPrintAreaCommandGroup = new SpreadsheetCommandId(659);
		public static readonly SpreadsheetCommandId PageSetupSetPrintArea = new SpreadsheetCommandId(660);
		public static readonly SpreadsheetCommandId PageSetupClearPrintArea = new SpreadsheetCommandId(661);
		public static readonly SpreadsheetCommandId PageSetupAddPrintArea = new SpreadsheetCommandId(662);
		public static readonly SpreadsheetCommandId PageSetupPrintGridlines = new SpreadsheetCommandId(663);
		public static readonly SpreadsheetCommandId PageSetupPrintHeadings = new SpreadsheetCommandId(664);
		public static readonly SpreadsheetCommandId InsertCellsCommandGroup = new SpreadsheetCommandId(700);
		public static readonly SpreadsheetCommandId InsertSheet = new SpreadsheetCommandId(701);
		public static readonly SpreadsheetCommandId InsertSheetContextMenuItem = new SpreadsheetCommandId(702);
		public static readonly SpreadsheetCommandId InsertSheetRows = new SpreadsheetCommandId(703);
		public static readonly SpreadsheetCommandId InsertSheetRowsContextMenuItem = new SpreadsheetCommandId(704);
		public static readonly SpreadsheetCommandId InsertSheetColumns = new SpreadsheetCommandId(705);
		public static readonly SpreadsheetCommandId InsertSheetColumnsContextMenuItem = new SpreadsheetCommandId(706);
		public static readonly SpreadsheetCommandId InsertHyperlinkContextMenuItem = new SpreadsheetCommandId(712);
		public static readonly SpreadsheetCommandId EditHyperlinkContextMenuItem = new SpreadsheetCommandId(713);
		public static readonly SpreadsheetCommandId OpenHyperlinkContextMenuItem = new SpreadsheetCommandId(714);
		public static readonly SpreadsheetCommandId RemoveHyperlinkContextMenuItem = new SpreadsheetCommandId(715);
		public static readonly SpreadsheetCommandId RemoveHyperlinksContextMenuItem = new SpreadsheetCommandId(716);
		public static readonly SpreadsheetCommandId OpenPictureHyperlinkContextMenuItem = new SpreadsheetCommandId(717);
		public static readonly SpreadsheetCommandId ShapeMoveLeft = new SpreadsheetCommandId(800);
		public static readonly SpreadsheetCommandId ShapeMoveRight = new SpreadsheetCommandId(801);
		public static readonly SpreadsheetCommandId ShapeMoveUp = new SpreadsheetCommandId(802);
		public static readonly SpreadsheetCommandId ShapeMoveDown = new SpreadsheetCommandId(803);
		public static readonly SpreadsheetCommandId ShapeRotateLeft = new SpreadsheetCommandId(804);
		public static readonly SpreadsheetCommandId ShapeRotateLeftByDegree = new SpreadsheetCommandId(805);
		public static readonly SpreadsheetCommandId ShapeRotateRight = new SpreadsheetCommandId(806);
		public static readonly SpreadsheetCommandId ShapeRotateRightByDegree = new SpreadsheetCommandId(807);
		public static readonly SpreadsheetCommandId ShapeEnlargeHeight = new SpreadsheetCommandId(808);
		public static readonly SpreadsheetCommandId ShapeEnlargeWidth = new SpreadsheetCommandId(809);
		public static readonly SpreadsheetCommandId ShapeBitEnlargeWidth = new SpreadsheetCommandId(810);
		public static readonly SpreadsheetCommandId ShapeBitEnlargeHeight = new SpreadsheetCommandId(811);
		public static readonly SpreadsheetCommandId ShapeReduceHeight = new SpreadsheetCommandId(812);
		public static readonly SpreadsheetCommandId ShapeReduceWidth = new SpreadsheetCommandId(813);
		public static readonly SpreadsheetCommandId ShapeBitReduceWidth = new SpreadsheetCommandId(814);
		public static readonly SpreadsheetCommandId ShapeBitReduceHeight = new SpreadsheetCommandId(815);
		public static readonly SpreadsheetCommandId ShapeSelectAll = new SpreadsheetCommandId(816);
		public static readonly SpreadsheetCommandId ShapeSelectNext = new SpreadsheetCommandId(817);
		public static readonly SpreadsheetCommandId ShapeSelectPrevious = new SpreadsheetCommandId(818);
		public static readonly SpreadsheetCommandId ShapeChangeBounds = new SpreadsheetCommandId(820);
		public static readonly SpreadsheetCommandId ShapeMoveAndResize = new SpreadsheetCommandId(821);
		public static readonly SpreadsheetCommandId InsertHyperlink = new SpreadsheetCommandId(900);
		public static readonly SpreadsheetCommandId InsertPicture = new SpreadsheetCommandId(901);
		public static readonly SpreadsheetCommandId InsertSymbol = new SpreadsheetCommandId(902);
		public static readonly SpreadsheetCommandId RemoveSheet = new SpreadsheetCommandId(1000);
		public static readonly SpreadsheetCommandId RemoveSheetContextMenuItem = new SpreadsheetCommandId(1001);
		public static readonly SpreadsheetCommandId RemoveSheetRows = new SpreadsheetCommandId(1002);
		public static readonly SpreadsheetCommandId RemoveSheetRowsContextMenuItem = new SpreadsheetCommandId(1003);
		public static readonly SpreadsheetCommandId RemoveSheetColumns = new SpreadsheetCommandId(1004);
		public static readonly SpreadsheetCommandId RemoveSheetColumnsContextMenuItem = new SpreadsheetCommandId(1005);
		public static readonly SpreadsheetCommandId RemoveCellsCommandGroup = new SpreadsheetCommandId(1006);
		public static readonly SpreadsheetCommandId CopySelection = new SpreadsheetCommandId(1100);
		public static readonly SpreadsheetCommandId PasteSelection = new SpreadsheetCommandId(1101);
		public static readonly SpreadsheetCommandId CutSelection = new SpreadsheetCommandId(1102);
		public static readonly SpreadsheetCommandId ClearCopiedRange = new SpreadsheetCommandId(1103);
		public static readonly SpreadsheetCommandId FormatCommandGroup = new SpreadsheetCommandId(1200);
		public static readonly SpreadsheetCommandId FormatAutoFitRowHeight = new SpreadsheetCommandId(1201);
		public static readonly SpreadsheetCommandId FormatAutoFitColumnWidth = new SpreadsheetCommandId(1202);
		public static readonly SpreadsheetCommandId HideRows = new SpreadsheetCommandId(1203);
		public static readonly SpreadsheetCommandId HideRowsContextMenuItem = new SpreadsheetCommandId(1204);
		public static readonly SpreadsheetCommandId HideColumns = new SpreadsheetCommandId(1205);
		public static readonly SpreadsheetCommandId HideColumnsContextMenuItem = new SpreadsheetCommandId(1206);
		public static readonly SpreadsheetCommandId HideSheet = new SpreadsheetCommandId(1207);
		public static readonly SpreadsheetCommandId HideSheetContextMenuItem = new SpreadsheetCommandId(1208);
		public static readonly SpreadsheetCommandId RenameSheet = new SpreadsheetCommandId(1209);
		public static readonly SpreadsheetCommandId RenameSheetContextMenuItem = new SpreadsheetCommandId(1210);
		public static readonly SpreadsheetCommandId UnhideRows = new SpreadsheetCommandId(1211);
		public static readonly SpreadsheetCommandId UnhideRowsContextMenuItem = new SpreadsheetCommandId(1212);
		public static readonly SpreadsheetCommandId UnhideColumns = new SpreadsheetCommandId(1213);
		public static readonly SpreadsheetCommandId UnhideColumnsContextMenuItem = new SpreadsheetCommandId(1214);
		public static readonly SpreadsheetCommandId UnhideSheet = new SpreadsheetCommandId(1215);
		public static readonly SpreadsheetCommandId UnhideSheetContextMenuItem = new SpreadsheetCommandId(1216);
		public static readonly SpreadsheetCommandId HideAndUnhideCommandGroup = new SpreadsheetCommandId(1217);
		public static readonly SpreadsheetCommandId MoveOrCopySheet = new SpreadsheetCommandId(1218);
		public static readonly SpreadsheetCommandId MoveOrCopySheetContextMenuItem = new SpreadsheetCommandId(1219);
		public static readonly SpreadsheetCommandId FormatTabColor = new SpreadsheetCommandId(1220);
		public static readonly SpreadsheetCommandId FormatRowHeight = new SpreadsheetCommandId(1221);
		public static readonly SpreadsheetCommandId FormatColumnWidth = new SpreadsheetCommandId(1222);
		public static readonly SpreadsheetCommandId FormatDefaultColumnWidth = new SpreadsheetCommandId(1223);
		public static readonly SpreadsheetCommandId FormatRowHeightContextMenuItem = new SpreadsheetCommandId(1224);
		public static readonly SpreadsheetCommandId FormatColumnWidthContextMenuItem = new SpreadsheetCommandId(1225);
		public static readonly SpreadsheetCommandId FormatAutoFitColumnWidthUsingMouse = new SpreadsheetCommandId(1226);
		public static readonly SpreadsheetCommandId FormatAutoFitRowHeightUsingMouse = new SpreadsheetCommandId(1227);
		public static readonly SpreadsheetCommandId ShowPasteSpecialForm = new SpreadsheetCommandId(1300);
		public static readonly SpreadsheetCommandId AddNewWorksheet = new SpreadsheetCommandId(1301);
		public static readonly SpreadsheetCommandId FormatCellsNumber = new SpreadsheetCommandId(1400);
		public static readonly SpreadsheetCommandId FormatCellsAlignment = new SpreadsheetCommandId(1401);
		public static readonly SpreadsheetCommandId FormatCellsFont = new SpreadsheetCommandId(1402);
		public static readonly SpreadsheetCommandId FormatCellsBorder = new SpreadsheetCommandId(1403);
		public static readonly SpreadsheetCommandId FormatCellsFill = new SpreadsheetCommandId(1404);
		public static readonly SpreadsheetCommandId FormatCellsProtection = new SpreadsheetCommandId(1405);
		public static readonly SpreadsheetCommandId FormatCellsContextMenuItem = new SpreadsheetCommandId(1406);
		public static readonly SpreadsheetCommandId InsertTable = new SpreadsheetCommandId(1449);
		public static readonly SpreadsheetCommandId DataSortAscending = new SpreadsheetCommandId(1450);
		public static readonly SpreadsheetCommandId DataSortDescending = new SpreadsheetCommandId(1451);
		public static readonly SpreadsheetCommandId DataFilterEquals = new SpreadsheetCommandId(1452);
		public static readonly SpreadsheetCommandId DataFilterDoesNotEqual = new SpreadsheetCommandId(1453);
		public static readonly SpreadsheetCommandId DataFilterGreaterThan = new SpreadsheetCommandId(1454);
		public static readonly SpreadsheetCommandId DataFilterGreaterThanOrEqualTo = new SpreadsheetCommandId(1455);
		public static readonly SpreadsheetCommandId DataFilterLessThan = new SpreadsheetCommandId(1456);
		public static readonly SpreadsheetCommandId DataFilterLessThanOrEqualTo = new SpreadsheetCommandId(1457);
		public static readonly SpreadsheetCommandId DataFilterBetween = new SpreadsheetCommandId(1458);
		public static readonly SpreadsheetCommandId DataFilterTop10 = new SpreadsheetCommandId(1459);
		public static readonly SpreadsheetCommandId DataFilterAboveAverage = new SpreadsheetCommandId(1460);
		public static readonly SpreadsheetCommandId DataFilterBelowAverage = new SpreadsheetCommandId(1461);
		public static readonly SpreadsheetCommandId DataFilterBeginsWith = new SpreadsheetCommandId(1462);
		public static readonly SpreadsheetCommandId DataFilterEndsWith = new SpreadsheetCommandId(1463);
		public static readonly SpreadsheetCommandId DataFilterContains = new SpreadsheetCommandId(1464);
		public static readonly SpreadsheetCommandId DataFilterDoesNotContain = new SpreadsheetCommandId(1465);
		public static readonly SpreadsheetCommandId DataFilterCustom = new SpreadsheetCommandId(1466);
		public static readonly SpreadsheetCommandId DataFilterClear = new SpreadsheetCommandId(1467);
		public static readonly SpreadsheetCommandId DataFilterColumnClear = new SpreadsheetCommandId(1468);
		public static readonly SpreadsheetCommandId DataFilterReApply = new SpreadsheetCommandId(1469);
		public static readonly SpreadsheetCommandId DataFilterToggle = new SpreadsheetCommandId(1470);
		public static readonly SpreadsheetCommandId DataFilterSimple = new SpreadsheetCommandId(1471);
		public static readonly SpreadsheetCommandId DataFilterNumberFiltersCommandGroup = new SpreadsheetCommandId(1473);
		public static readonly SpreadsheetCommandId DataFilterTextFiltersCommandGroup = new SpreadsheetCommandId(1474);
		public static readonly SpreadsheetCommandId DataFilterDateFiltersCommandGroup = new SpreadsheetCommandId(1475);
		public static readonly SpreadsheetCommandId DataFilterAllDatesInPeriodCommandGroup = new SpreadsheetCommandId(1476);
		public static readonly SpreadsheetCommandId MoveToNextSheet = new SpreadsheetCommandId(1500);
		public static readonly SpreadsheetCommandId MoveToPreviousSheet = new SpreadsheetCommandId(1501);
		public static readonly SpreadsheetCommandId CollapseOrExpandFormulaBar = new SpreadsheetCommandId(1550);
		public static readonly SpreadsheetCommandId DataFilterToday = new SpreadsheetCommandId(1600);
		public static readonly SpreadsheetCommandId DataFilterYesterday = new SpreadsheetCommandId(1601);
		public static readonly SpreadsheetCommandId DataFilterTomorrow = new SpreadsheetCommandId(1602);
		public static readonly SpreadsheetCommandId DataFilterThisWeek = new SpreadsheetCommandId(1603);
		public static readonly SpreadsheetCommandId DataFilterLastWeek = new SpreadsheetCommandId(1604);
		public static readonly SpreadsheetCommandId DataFilterNextWeek = new SpreadsheetCommandId(1605);
		public static readonly SpreadsheetCommandId DataFilterThisMonth = new SpreadsheetCommandId(1606);
		public static readonly SpreadsheetCommandId DataFilterLastMonth = new SpreadsheetCommandId(1607);
		public static readonly SpreadsheetCommandId DataFilterNextMonth = new SpreadsheetCommandId(1608);
		public static readonly SpreadsheetCommandId DataFilterThisQuarter = new SpreadsheetCommandId(1609);
		public static readonly SpreadsheetCommandId DataFilterLastQuarter = new SpreadsheetCommandId(1610);
		public static readonly SpreadsheetCommandId DataFilterNextQuarter = new SpreadsheetCommandId(1611);
		public static readonly SpreadsheetCommandId DataFilterThisYear = new SpreadsheetCommandId(1612);
		public static readonly SpreadsheetCommandId DataFilterLastYear = new SpreadsheetCommandId(1613);
		public static readonly SpreadsheetCommandId DataFilterNextYear = new SpreadsheetCommandId(1614);
		public static readonly SpreadsheetCommandId DataFilterYearToDate = new SpreadsheetCommandId(1615);
		public static readonly SpreadsheetCommandId DataFilterQuarter1 = new SpreadsheetCommandId(1616);
		public static readonly SpreadsheetCommandId DataFilterQuarter2 = new SpreadsheetCommandId(1617);
		public static readonly SpreadsheetCommandId DataFilterQuarter3 = new SpreadsheetCommandId(1618);
		public static readonly SpreadsheetCommandId DataFilterQuarter4 = new SpreadsheetCommandId(1619);
		public static readonly SpreadsheetCommandId DataFilterMonthJanuary = new SpreadsheetCommandId(1620);
		public static readonly SpreadsheetCommandId DataFilterMonthFebruary = new SpreadsheetCommandId(1621);
		public static readonly SpreadsheetCommandId DataFilterMonthMarch = new SpreadsheetCommandId(1622);
		public static readonly SpreadsheetCommandId DataFilterMonthApril = new SpreadsheetCommandId(1623);
		public static readonly SpreadsheetCommandId DataFilterMonthMay = new SpreadsheetCommandId(1624);
		public static readonly SpreadsheetCommandId DataFilterMonthJune = new SpreadsheetCommandId(1625);
		public static readonly SpreadsheetCommandId DataFilterMonthJuly = new SpreadsheetCommandId(1626);
		public static readonly SpreadsheetCommandId DataFilterMonthAugust = new SpreadsheetCommandId(1627);
		public static readonly SpreadsheetCommandId DataFilterMonthSeptember = new SpreadsheetCommandId(1628);
		public static readonly SpreadsheetCommandId DataFilterMonthOctober = new SpreadsheetCommandId(1630);
		public static readonly SpreadsheetCommandId DataFilterMonthNovember = new SpreadsheetCommandId(1631);
		public static readonly SpreadsheetCommandId DataFilterMonthDecember = new SpreadsheetCommandId(1632);
		public static readonly SpreadsheetCommandId DataFilterDateEquals = new SpreadsheetCommandId(1633);
		public static readonly SpreadsheetCommandId DataFilterDateBefore = new SpreadsheetCommandId(1634);
		public static readonly SpreadsheetCommandId DataFilterDateAfter = new SpreadsheetCommandId(1635);
		public static readonly SpreadsheetCommandId DataFilterDateBetween = new SpreadsheetCommandId(1636);
		public static readonly SpreadsheetCommandId DataFilterDateCustom = new SpreadsheetCommandId(1637);
		public static readonly SpreadsheetCommandId DataToolsDataValidationCommandGroup = new SpreadsheetCommandId(1700);
		public static readonly SpreadsheetCommandId DataToolsDataValidation = new SpreadsheetCommandId(1701);
		public static readonly SpreadsheetCommandId DataToolsCircleInvalidData = new SpreadsheetCommandId(1702);
		public static readonly SpreadsheetCommandId DataToolsClearValidationCircles = new SpreadsheetCommandId(1703);
		const int conditionalFormattingCommandIdBase = 2000;
		public static readonly SpreadsheetCommandId ConditionalFormattingRemoveCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase);
		public static readonly SpreadsheetCommandId ConditionalFormattingRemoveFromSheet = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 1);
		public static readonly SpreadsheetCommandId ConditionalFormattingRemove = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 2);
		public static readonly SpreadsheetCommandId ConditionalFormattingCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 3);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScalesCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 4);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleGreenYellowRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 5);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleRedYellowGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 6);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleGreenWhiteRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 7);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleRedWhiteGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 8);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleBlueWhiteRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 9);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleRedWhiteBlue = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 10);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleWhiteRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 11);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleRedWhite = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 12);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleWhiteGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 13);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleGreenWhite = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 14);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleYellowGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 15);
		public static readonly SpreadsheetCommandId ConditionalFormattingColorScaleGreenYellow = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 16);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarsCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 17);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarsGradientFillCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 18);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarsSolidFillCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 19);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetsCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 20);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetsDirectionalCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 21);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetsShapesCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 22);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetsIndicatorsCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 23);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetsRatingsCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 24);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows3Colored = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 25);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows3Grayed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 26);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows4Colored = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 27);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows4Grayed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 28);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows5Colored = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 29);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetArrows5Grayed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 30);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetTriangles3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 31);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetTrafficLights3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 32);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetTrafficLights3Rimmed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 33);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetTrafficLights4 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 34);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetSigns3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 35);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetRedToBlack = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 36);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetSymbols3Circled = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 37);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetSymbols3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 38);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetFlags3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 39);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetStars3 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 40);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetRatings4 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 41);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetRatings5 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 42);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetQuarters5 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 43);
		public static readonly SpreadsheetCommandId ConditionalFormattingIconSetBoxes5 = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 44);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientBlue = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 45);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidBlue = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 46);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 47);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidGreen = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 48);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 49);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidRed = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 50);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientOrange = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 51);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidOrange = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 52);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientLightBlue = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 53);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidLightBlue = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 54);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarGradientPurple = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 55);
		public static readonly SpreadsheetCommandId ConditionalFormattingDataBarSolidPurple = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 56);
		public static readonly SpreadsheetCommandId ConditionalFormattingTopBottomRuleCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 57);
		public static readonly SpreadsheetCommandId ConditionalFormattingTop10RuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 58);
		public static readonly SpreadsheetCommandId ConditionalFormattingBottom10RuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 59);
		public static readonly SpreadsheetCommandId ConditionalFormattingTop10PercentRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 60);
		public static readonly SpreadsheetCommandId ConditionalFormattingBottom10PercentRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 61);
		public static readonly SpreadsheetCommandId ConditionalFormattingAboveAverageRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 62);
		public static readonly SpreadsheetCommandId ConditionalFormattingBelowAverageRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 63);
		public static readonly SpreadsheetCommandId ConditionalFormattingHighlightCellsRuleCommandGroup = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 64);
		public static readonly SpreadsheetCommandId ConditionalFormattingGreaterThanRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 65);
		public static readonly SpreadsheetCommandId ConditionalFormattingLessThanRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 66);
		public static readonly SpreadsheetCommandId ConditionalFormattingEqualToRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 67);
		public static readonly SpreadsheetCommandId ConditionalFormattingTextContainsRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 68);
		public static readonly SpreadsheetCommandId ConditionalFormattingDateOccurringRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 69);
		public static readonly SpreadsheetCommandId ConditionalFormattingDuplicateValuesRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 70);
		public static readonly SpreadsheetCommandId ConditionalFormattingBetweenRuleCommand = new SpreadsheetCommandId(conditionalFormattingCommandIdBase + 71);
		public static readonly SpreadsheetCommandId InsertChartColumnCommandGroup = new SpreadsheetCommandId(2170);
		public static readonly SpreadsheetCommandId InsertChartBarCommandGroup = new SpreadsheetCommandId(2171);
		public static readonly SpreadsheetCommandId InsertChartColumn2DCommandGroup = new SpreadsheetCommandId(2172);
		public static readonly SpreadsheetCommandId InsertChartColumn3DCommandGroup = new SpreadsheetCommandId(2173);
		public static readonly SpreadsheetCommandId InsertChartCylinderCommandGroup = new SpreadsheetCommandId(2174);
		public static readonly SpreadsheetCommandId InsertChartConeCommandGroup = new SpreadsheetCommandId(2175);
		public static readonly SpreadsheetCommandId InsertChartPyramidCommandGroup = new SpreadsheetCommandId(2176);
		public static readonly SpreadsheetCommandId InsertChartBar2DCommandGroup = new SpreadsheetCommandId(2177);
		public static readonly SpreadsheetCommandId InsertChartBar3DCommandGroup = new SpreadsheetCommandId(2178);
		public static readonly SpreadsheetCommandId InsertChartHorizontalCylinderCommandGroup = new SpreadsheetCommandId(2179);
		public static readonly SpreadsheetCommandId InsertChartHorizontalConeCommandGroup = new SpreadsheetCommandId(2180);
		public static readonly SpreadsheetCommandId InsertChartHorizontalPyramidCommandGroup = new SpreadsheetCommandId(2181);
		public static readonly SpreadsheetCommandId InsertChartPieCommandGroup = new SpreadsheetCommandId(2182);
		public static readonly SpreadsheetCommandId InsertChartPie2DCommandGroup = new SpreadsheetCommandId(2183);
		public static readonly SpreadsheetCommandId InsertChartPie3DCommandGroup = new SpreadsheetCommandId(2184);
		public static readonly SpreadsheetCommandId InsertChartLineCommandGroup = new SpreadsheetCommandId(2185);
		public static readonly SpreadsheetCommandId InsertChartLine2DCommandGroup = new SpreadsheetCommandId(2186);
		public static readonly SpreadsheetCommandId InsertChartLine3DCommandGroup = new SpreadsheetCommandId(2187);
		public static readonly SpreadsheetCommandId InsertChartAreaCommandGroup = new SpreadsheetCommandId(2188);
		public static readonly SpreadsheetCommandId InsertChartArea2DCommandGroup = new SpreadsheetCommandId(2189);
		public static readonly SpreadsheetCommandId InsertChartArea3DCommandGroup = new SpreadsheetCommandId(2190);
		public static readonly SpreadsheetCommandId InsertChartScatterCommandGroup = new SpreadsheetCommandId(2191);
		public static readonly SpreadsheetCommandId InsertChartBubbleCommandGroup = new SpreadsheetCommandId(2192);
		public static readonly SpreadsheetCommandId InsertChartDoughnut2DCommandGroup = new SpreadsheetCommandId(2193);
		public static readonly SpreadsheetCommandId InsertChartOtherCommandGroup = new SpreadsheetCommandId(2194);
		public static readonly SpreadsheetCommandId InsertChartStockCommandGroup = new SpreadsheetCommandId(2195);
		public static readonly SpreadsheetCommandId InsertChartRadarCommandGroup = new SpreadsheetCommandId(2196);
		public static readonly SpreadsheetCommandId InsertChartColumnClustered2D = new SpreadsheetCommandId(2200);
		public static readonly SpreadsheetCommandId InsertChartColumnStacked2D = new SpreadsheetCommandId(2201);
		public static readonly SpreadsheetCommandId InsertChartColumnPercentStacked2D = new SpreadsheetCommandId(2202);
		public static readonly SpreadsheetCommandId InsertChartColumnClustered3D = new SpreadsheetCommandId(2203);
		public static readonly SpreadsheetCommandId InsertChartColumnStacked3D = new SpreadsheetCommandId(2204);
		public static readonly SpreadsheetCommandId InsertChartColumnPercentStacked3D = new SpreadsheetCommandId(2205);
		public static readonly SpreadsheetCommandId InsertChartCylinderClustered = new SpreadsheetCommandId(2206);
		public static readonly SpreadsheetCommandId InsertChartCylinderStacked = new SpreadsheetCommandId(2207);
		public static readonly SpreadsheetCommandId InsertChartCylinderPercentStacked = new SpreadsheetCommandId(2208);
		public static readonly SpreadsheetCommandId InsertChartConeClustered = new SpreadsheetCommandId(2209);
		public static readonly SpreadsheetCommandId InsertChartConeStacked = new SpreadsheetCommandId(2210);
		public static readonly SpreadsheetCommandId InsertChartConePercentStacked = new SpreadsheetCommandId(2211);
		public static readonly SpreadsheetCommandId InsertChartPyramidClustered = new SpreadsheetCommandId(2212);
		public static readonly SpreadsheetCommandId InsertChartPyramidStacked = new SpreadsheetCommandId(2213);
		public static readonly SpreadsheetCommandId InsertChartPyramidPercentStacked = new SpreadsheetCommandId(2214);
		public static readonly SpreadsheetCommandId InsertChartBarClustered2D = new SpreadsheetCommandId(2215);
		public static readonly SpreadsheetCommandId InsertChartBarStacked2D = new SpreadsheetCommandId(2216);
		public static readonly SpreadsheetCommandId InsertChartBarPercentStacked2D = new SpreadsheetCommandId(2217);
		public static readonly SpreadsheetCommandId InsertChartBarClustered3D = new SpreadsheetCommandId(2218);
		public static readonly SpreadsheetCommandId InsertChartBarStacked3D = new SpreadsheetCommandId(2219);
		public static readonly SpreadsheetCommandId InsertChartBarPercentStacked3D = new SpreadsheetCommandId(2220);
		public static readonly SpreadsheetCommandId InsertChartHorizontalCylinderClustered = new SpreadsheetCommandId(2221);
		public static readonly SpreadsheetCommandId InsertChartHorizontalCylinderStacked = new SpreadsheetCommandId(2222);
		public static readonly SpreadsheetCommandId InsertChartHorizontalCylinderPercentStacked = new SpreadsheetCommandId(2223);
		public static readonly SpreadsheetCommandId InsertChartHorizontalConeClustered = new SpreadsheetCommandId(2224);
		public static readonly SpreadsheetCommandId InsertChartHorizontalConeStacked = new SpreadsheetCommandId(2225);
		public static readonly SpreadsheetCommandId InsertChartHorizontalConePercentStacked = new SpreadsheetCommandId(2226);
		public static readonly SpreadsheetCommandId InsertChartHorizontalPyramidClustered = new SpreadsheetCommandId(2227);
		public static readonly SpreadsheetCommandId InsertChartHorizontalPyramidStacked = new SpreadsheetCommandId(2228);
		public static readonly SpreadsheetCommandId InsertChartHorizontalPyramidPercentStacked = new SpreadsheetCommandId(2229);
		public static readonly SpreadsheetCommandId InsertChartColumn3D = new SpreadsheetCommandId(2230);
		public static readonly SpreadsheetCommandId InsertChartCylinder = new SpreadsheetCommandId(2231);
		public static readonly SpreadsheetCommandId InsertChartCone = new SpreadsheetCommandId(2232);
		public static readonly SpreadsheetCommandId InsertChartPyramid = new SpreadsheetCommandId(2233);
		public static readonly SpreadsheetCommandId InsertChartPie2D = new SpreadsheetCommandId(2234);
		public static readonly SpreadsheetCommandId InsertChartPie3D = new SpreadsheetCommandId(2235);
		public static readonly SpreadsheetCommandId InsertChartPieExploded2D = new SpreadsheetCommandId(2236);
		public static readonly SpreadsheetCommandId InsertChartPieExploded3D = new SpreadsheetCommandId(2237);
		public static readonly SpreadsheetCommandId InsertChartLine = new SpreadsheetCommandId(2238);
		public static readonly SpreadsheetCommandId InsertChartStackedLine = new SpreadsheetCommandId(2239);
		public static readonly SpreadsheetCommandId InsertChartPercentStackedLine = new SpreadsheetCommandId(2240);
		public static readonly SpreadsheetCommandId InsertChartLineWithMarkers = new SpreadsheetCommandId(2241);
		public static readonly SpreadsheetCommandId InsertChartStackedLineWithMarkers = new SpreadsheetCommandId(2242);
		public static readonly SpreadsheetCommandId InsertChartPercentStackedLineWithMarkers = new SpreadsheetCommandId(2243);
		public static readonly SpreadsheetCommandId InsertChartLine3D = new SpreadsheetCommandId(2244);
		public static readonly SpreadsheetCommandId InsertChartArea = new SpreadsheetCommandId(2245);
		public static readonly SpreadsheetCommandId InsertChartStackedArea = new SpreadsheetCommandId(2246);
		public static readonly SpreadsheetCommandId InsertChartPercentStackedArea = new SpreadsheetCommandId(2247);
		public static readonly SpreadsheetCommandId InsertChartArea3D = new SpreadsheetCommandId(2248);
		public static readonly SpreadsheetCommandId InsertChartStackedArea3D = new SpreadsheetCommandId(2249);
		public static readonly SpreadsheetCommandId InsertChartPercentStackedArea3D = new SpreadsheetCommandId(2250);
		public static readonly SpreadsheetCommandId InsertChartScatterMarkers = new SpreadsheetCommandId(2251);
		public static readonly SpreadsheetCommandId InsertChartScatterLines = new SpreadsheetCommandId(2252);
		public static readonly SpreadsheetCommandId InsertChartScatterSmoothLines = new SpreadsheetCommandId(2253);
		public static readonly SpreadsheetCommandId InsertChartScatterLinesAndMarkers = new SpreadsheetCommandId(2254);
		public static readonly SpreadsheetCommandId InsertChartScatterSmoothLinesAndMarkers = new SpreadsheetCommandId(2255);
		public static readonly SpreadsheetCommandId InsertChartBubble = new SpreadsheetCommandId(2256);
		public static readonly SpreadsheetCommandId InsertChartBubble3D = new SpreadsheetCommandId(2257);
		public static readonly SpreadsheetCommandId InsertChartDoughnut2D = new SpreadsheetCommandId(2258);
		public static readonly SpreadsheetCommandId InsertChartDoughnutExploded2D = new SpreadsheetCommandId(2259);
		public static readonly SpreadsheetCommandId InsertChartRadar = new SpreadsheetCommandId(2260);
		public static readonly SpreadsheetCommandId InsertChartRadarWithMarkers = new SpreadsheetCommandId(2261);
		public static readonly SpreadsheetCommandId InsertChartRadarFilled = new SpreadsheetCommandId(2262);
		public static readonly SpreadsheetCommandId InsertChartStockHighLowClose = new SpreadsheetCommandId(2263);
		public static readonly SpreadsheetCommandId InsertChartStockOpenHighLowClose = new SpreadsheetCommandId(2264);
		public static readonly SpreadsheetCommandId InsertChartStockVolumeHighLowClose = new SpreadsheetCommandId(2265);
		public static readonly SpreadsheetCommandId InsertChartStockVolumeOpenHighLowClose = new SpreadsheetCommandId(2266);
		public static readonly SpreadsheetCommandId ModifyChartLayout = new SpreadsheetCommandId(2290);
		public static readonly SpreadsheetCommandId ModifyChartStyle = new SpreadsheetCommandId(2291);
		public static readonly SpreadsheetCommandId ChartAxesCommandGroup = new SpreadsheetCommandId(2300);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisCommandGroup = new SpreadsheetCommandId(2301);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisCommandGroup = new SpreadsheetCommandId(2302);
		public static readonly SpreadsheetCommandId ChartHidePrimaryHorizontalAxis = new SpreadsheetCommandId(2303);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisLeftToRight = new SpreadsheetCommandId(2304);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisRightToLeft = new SpreadsheetCommandId(2305);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisHideLabels = new SpreadsheetCommandId(2306);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisDefault = new SpreadsheetCommandId(2307);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisScaleLogarithm = new SpreadsheetCommandId(2308);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisScaleThousands = new SpreadsheetCommandId(2309);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisScaleMillions = new SpreadsheetCommandId(2310);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisScaleBillions = new SpreadsheetCommandId(2311);
		public static readonly SpreadsheetCommandId ChartHidePrimaryVerticalAxis = new SpreadsheetCommandId(2312);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisLeftToRight = new SpreadsheetCommandId(2313);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisRightToLeft = new SpreadsheetCommandId(2314);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisHideLabels = new SpreadsheetCommandId(2315);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisDefault = new SpreadsheetCommandId(2316);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisScaleLogarithm = new SpreadsheetCommandId(2317);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisScaleThousands = new SpreadsheetCommandId(2318);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisScaleMillions = new SpreadsheetCommandId(2319);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisScaleBillions = new SpreadsheetCommandId(2320);
		public static readonly SpreadsheetCommandId ChartGridlinesCommandGroup = new SpreadsheetCommandId(2321);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalGridlinesCommandGroup = new SpreadsheetCommandId(2322);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalGridlinesCommandGroup = new SpreadsheetCommandId(2323);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalGridlinesNone = new SpreadsheetCommandId(2324);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalGridlinesMajor = new SpreadsheetCommandId(2325);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalGridlinesMinor = new SpreadsheetCommandId(2326);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalGridlinesMajorAndMinor = new SpreadsheetCommandId(2327);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalGridlinesNone = new SpreadsheetCommandId(2328);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalGridlinesMajor = new SpreadsheetCommandId(2329);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalGridlinesMinor = new SpreadsheetCommandId(2330);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalGridlinesMajorAndMinor = new SpreadsheetCommandId(2331);
		public static readonly SpreadsheetCommandId ChartAxisTitlesCommandGroup = new SpreadsheetCommandId(2332);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisTitleCommandGroup = new SpreadsheetCommandId(2333);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisTitleCommandGroup = new SpreadsheetCommandId(2334);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisTitleNone = new SpreadsheetCommandId(2335);
		public static readonly SpreadsheetCommandId ChartPrimaryHorizontalAxisTitleBelow = new SpreadsheetCommandId(2336);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisTitleNone = new SpreadsheetCommandId(2337);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisTitleRotated = new SpreadsheetCommandId(2338);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisTitleVertical = new SpreadsheetCommandId(2339);
		public static readonly SpreadsheetCommandId ChartPrimaryVerticalAxisTitleHorizontal = new SpreadsheetCommandId(2340);
		public static readonly SpreadsheetCommandId ChartTitleCommandGroup = new SpreadsheetCommandId(2341);
		public static readonly SpreadsheetCommandId ChartTitleNone = new SpreadsheetCommandId(2342);
		public static readonly SpreadsheetCommandId ChartTitleCenteredOverlay = new SpreadsheetCommandId(2343);
		public static readonly SpreadsheetCommandId ChartTitleAbove = new SpreadsheetCommandId(2344);
		public static readonly SpreadsheetCommandId ChartLegendCommandGroup = new SpreadsheetCommandId(2345);
		public static readonly SpreadsheetCommandId ChartLegendNone = new SpreadsheetCommandId(2346);
		public static readonly SpreadsheetCommandId ChartLegendAtRight = new SpreadsheetCommandId(2347);
		public static readonly SpreadsheetCommandId ChartLegendAtTop = new SpreadsheetCommandId(2348);
		public static readonly SpreadsheetCommandId ChartLegendAtLeft = new SpreadsheetCommandId(2349);
		public static readonly SpreadsheetCommandId ChartLegendAtBottom = new SpreadsheetCommandId(2350);
		public static readonly SpreadsheetCommandId ChartLegendOverlayAtRight = new SpreadsheetCommandId(2351);
		public static readonly SpreadsheetCommandId ChartLegendOverlayAtLeft = new SpreadsheetCommandId(2352);
		public static readonly SpreadsheetCommandId ChartDataLabelsCommandGroup = new SpreadsheetCommandId(2353);
		public static readonly SpreadsheetCommandId ChartDataLabelsNone = new SpreadsheetCommandId(2354);
		public static readonly SpreadsheetCommandId ChartDataLabelsDefault = new SpreadsheetCommandId(2355);
		public static readonly SpreadsheetCommandId ChartDataLabelsCenter = new SpreadsheetCommandId(2356);
		public static readonly SpreadsheetCommandId ChartDataLabelsInsideEnd = new SpreadsheetCommandId(2357);
		public static readonly SpreadsheetCommandId ChartDataLabelsInsideBase = new SpreadsheetCommandId(2358);
		public static readonly SpreadsheetCommandId ChartDataLabelsOutsideEnd = new SpreadsheetCommandId(2359);
		public static readonly SpreadsheetCommandId ChartDataLabelsBestFit = new SpreadsheetCommandId(2360);
		public static readonly SpreadsheetCommandId ChartDataLabelsLeft = new SpreadsheetCommandId(2361);
		public static readonly SpreadsheetCommandId ChartDataLabelsRight = new SpreadsheetCommandId(2362);
		public static readonly SpreadsheetCommandId ChartDataLabelsAbove = new SpreadsheetCommandId(2363);
		public static readonly SpreadsheetCommandId ChartDataLabelsBelow = new SpreadsheetCommandId(2364);
		public static readonly SpreadsheetCommandId ChartLinesCommandGroup = new SpreadsheetCommandId(2365);
		public static readonly SpreadsheetCommandId ChartLinesNone = new SpreadsheetCommandId(2366);
		public static readonly SpreadsheetCommandId ChartShowDropLines = new SpreadsheetCommandId(2367);
		public static readonly SpreadsheetCommandId ChartShowHighLowLines = new SpreadsheetCommandId(2368);
		public static readonly SpreadsheetCommandId ChartShowDropLinesAndHighLowLines = new SpreadsheetCommandId(2369);
		public static readonly SpreadsheetCommandId ChartShowSeriesLines = new SpreadsheetCommandId(2370);
		public static readonly SpreadsheetCommandId ChartUpDownBarsCommandGroup = new SpreadsheetCommandId(2371);
		public static readonly SpreadsheetCommandId ChartHideUpDownBars = new SpreadsheetCommandId(2372);
		public static readonly SpreadsheetCommandId ChartShowUpDownBars = new SpreadsheetCommandId(2373);
		public static readonly SpreadsheetCommandId ChartErrorBarsCommandGroup = new SpreadsheetCommandId(2374);
		public static readonly SpreadsheetCommandId ChartErrorBarsNone = new SpreadsheetCommandId(2375);
		public static readonly SpreadsheetCommandId ChartErrorBarsPercentage = new SpreadsheetCommandId(2376);
		public static readonly SpreadsheetCommandId ChartErrorBarsStandardError = new SpreadsheetCommandId(2377);
		public static readonly SpreadsheetCommandId ChartErrorBarsStandardDeviation = new SpreadsheetCommandId(2378);
		public static readonly SpreadsheetCommandId ChartSwitchRowColumn = new SpreadsheetCommandId(2379);
		public static readonly SpreadsheetCommandId ChartChangeType = new SpreadsheetCommandId(2380);
		public static readonly SpreadsheetCommandId ChartChangeTypeContextMenuItem = new SpreadsheetCommandId(2381);
		public static readonly SpreadsheetCommandId ChartChangeTitleContextMenuItem = new SpreadsheetCommandId(2382);
		public static readonly SpreadsheetCommandId ChartChangeHorizontalAxisTitleContextMenuItem = new SpreadsheetCommandId(2383);
		public static readonly SpreadsheetCommandId ChartChangeVerticalAxisTitleContextMenuItem = new SpreadsheetCommandId(2384);
		public static readonly SpreadsheetCommandId ChartSelectData = new SpreadsheetCommandId(2385);
		public static readonly SpreadsheetCommandId ChartSelectDataContextMenuItem = new SpreadsheetCommandId(2386);
		public static readonly SpreadsheetCommandId TableToolsToggleHeaderRow = new SpreadsheetCommandId(2400);
		public static readonly SpreadsheetCommandId TableToolsToggleTotalRow = new SpreadsheetCommandId(2401);
		public static readonly SpreadsheetCommandId TableToolsToggleBandedColumns = new SpreadsheetCommandId(2402);
		public static readonly SpreadsheetCommandId TableToolsToggleBandedRows = new SpreadsheetCommandId(2403);
		public static readonly SpreadsheetCommandId TableToolsToggleFirstColumn = new SpreadsheetCommandId(2404);
		public static readonly SpreadsheetCommandId TableToolsToggleLastColumn = new SpreadsheetCommandId(2405);
		public static readonly SpreadsheetCommandId ModifyTableStyles = new SpreadsheetCommandId(2406);
		public static readonly SpreadsheetCommandId TableToolsRenameTable = new SpreadsheetCommandId(2407);
		public static readonly SpreadsheetCommandId TableToolsConvertToRange = new SpreadsheetCommandId(2408);
		public static readonly SpreadsheetCommandId TableToolsCommandGroup = new SpreadsheetCommandId(2409);
		public static readonly SpreadsheetCommandId MailMergeSelectDataMember = new SpreadsheetCommandId(2454);
		public static readonly SpreadsheetCommandId MailMergeAddDataSource = new SpreadsheetCommandId(2455);
		public static readonly SpreadsheetCommandId MailMergeSelectDataSource = new SpreadsheetCommandId(2456);
		public static readonly SpreadsheetCommandId MailMergeDocumentsMode = new SpreadsheetCommandId(2457);
		public static readonly SpreadsheetCommandId MailMergeOneDocumentMode = new SpreadsheetCommandId(2458);
		public static readonly SpreadsheetCommandId MailMergeOneSheetMode = new SpreadsheetCommandId(2459);
		public static readonly SpreadsheetCommandId MailMergeSetDetailRange = new SpreadsheetCommandId(2460);
		public static readonly SpreadsheetCommandId MailMergeResetRange = new SpreadsheetCommandId(2461);
		public static readonly SpreadsheetCommandId MailMergeSetFooterRange = new SpreadsheetCommandId(2462);
		public static readonly SpreadsheetCommandId MailMergeShowRanges = new SpreadsheetCommandId(2463);
		public static readonly SpreadsheetCommandId MailMergeSetHeaderRange = new SpreadsheetCommandId(2464);
		public static readonly SpreadsheetCommandId EditingMailMergeMasterDetailCommandGroup = new SpreadsheetCommandId(2465);
		public static readonly SpreadsheetCommandId MailMergeSetDetailLevel = new SpreadsheetCommandId(2466);
		public static readonly SpreadsheetCommandId MailMergeHorizontalMode = new SpreadsheetCommandId(2467);
		public static readonly SpreadsheetCommandId MailMergeSetDetailDataMember = new SpreadsheetCommandId(2468);
		public static readonly SpreadsheetCommandId MailMergeVerticalMode = new SpreadsheetCommandId(2469);
		public static readonly SpreadsheetCommandId MailMergeOrientationCommandGroup = new SpreadsheetCommandId(2470);
		public static readonly SpreadsheetCommandId MailMergeSetGroup = new SpreadsheetCommandId(2471);
		public static readonly SpreadsheetCommandId MailMergeSetGroupHeader = new SpreadsheetCommandId(2472);
		public static readonly SpreadsheetCommandId MailMergeSetGroupFooter = new SpreadsheetCommandId(2473);
		public static readonly SpreadsheetCommandId MailMergeSetFilter = new SpreadsheetCommandId(2474);
		public static readonly SpreadsheetCommandId MailMergeResetFilter = new SpreadsheetCommandId(2475);
		public static readonly SpreadsheetCommandId MailMergePreview = new SpreadsheetCommandId(2476);
		public static readonly SpreadsheetCommandId MailMergeManageRelationsCommandGroup = new SpreadsheetCommandId(2477);
		public static readonly SpreadsheetCommandId MailMergeManageRelationsCommand = new SpreadsheetCommandId(2478);
		public static readonly SpreadsheetCommandId MailMergeManageQueriesCommand = new SpreadsheetCommandId(2479);
		public static readonly SpreadsheetCommandId MailMergeManageDataSourceCommandGroup = new SpreadsheetCommandId(2480);
		public static readonly SpreadsheetCommandId MailMergeManageDataSourcesCommand = new SpreadsheetCommandId(2481);
		public static readonly SpreadsheetCommandId ArrangeBringForwardCommandGroup = new SpreadsheetCommandId(2600);
		public static readonly SpreadsheetCommandId ArrangeBringForwardCommandGroupContextMenuItem = new SpreadsheetCommandId(2601);
		public static readonly SpreadsheetCommandId ArrangeBringForward = new SpreadsheetCommandId(2602);
		public static readonly SpreadsheetCommandId ArrangeBringToFront = new SpreadsheetCommandId(2603);
		public static readonly SpreadsheetCommandId ArrangeSendBackwardCommandGroup = new SpreadsheetCommandId(2604);
		public static readonly SpreadsheetCommandId ArrangeSendBackwardCommandGroupContextMenuItem = new SpreadsheetCommandId(2605);
		public static readonly SpreadsheetCommandId ArrangeSendBackward = new SpreadsheetCommandId(2606);
		public static readonly SpreadsheetCommandId ArrangeSendToBack = new SpreadsheetCommandId(2607);
		public static readonly SpreadsheetCommandId ToolsPictureCommandGroup = new SpreadsheetCommandId(2800);
		public static readonly SpreadsheetCommandId ToolsDrawingCommandGroup = new SpreadsheetCommandId(2801);
		public static readonly SpreadsheetCommandId ToolsChartCommandGroup = new SpreadsheetCommandId(2802);
		public static readonly SpreadsheetCommandId ToolsPivotTableCommandGroup = new SpreadsheetCommandId(2803);
		public static readonly SpreadsheetCommandId ReviewProtectSheet = new SpreadsheetCommandId(3000);
		public static readonly SpreadsheetCommandId ReviewUnprotectSheet = new SpreadsheetCommandId(3001);
		public static readonly SpreadsheetCommandId ReviewProtectWorkbook = new SpreadsheetCommandId(3002);
		public static readonly SpreadsheetCommandId ReviewUnprotectWorkbook = new SpreadsheetCommandId(3003);
		public static readonly SpreadsheetCommandId ReviewShowProtectedRangeManager = new SpreadsheetCommandId(3004);
		public static readonly SpreadsheetCommandId ReviewProtectSheetContextMenuItem = new SpreadsheetCommandId(3005);
		public static readonly SpreadsheetCommandId ReviewUnprotectSheetContextMenuItem = new SpreadsheetCommandId(3006);
		public static readonly SpreadsheetCommandId AutoOutline = new SpreadsheetCommandId(3200);
		public static readonly SpreadsheetCommandId ClearOutline = new SpreadsheetCommandId(3201);
		public static readonly SpreadsheetCommandId GroupOutline = new SpreadsheetCommandId(3202);
		public static readonly SpreadsheetCommandId HideDetail = new SpreadsheetCommandId(3203);
		public static readonly SpreadsheetCommandId ShowDetail = new SpreadsheetCommandId(3204);
		public static readonly SpreadsheetCommandId Subtotal = new SpreadsheetCommandId(3205);
		public static readonly SpreadsheetCommandId UngroupOutline = new SpreadsheetCommandId(3206);
		public static readonly SpreadsheetCommandId OutlineGroupCommandGroup = new SpreadsheetCommandId(3207);
		public static readonly SpreadsheetCommandId OutlineUngroupCommandGroup = new SpreadsheetCommandId(3208);
		public static readonly SpreadsheetCommandId OutlineSettingsCommand = new SpreadsheetCommandId(3209);
		public static readonly SpreadsheetCommandId ReviewInsertComment = new SpreadsheetCommandId(3500);
		public static readonly SpreadsheetCommandId ReviewInsertCommentContextMenuItem = new SpreadsheetCommandId(3501);
		public static readonly SpreadsheetCommandId ReviewEditComment = new SpreadsheetCommandId(3502);
		public static readonly SpreadsheetCommandId ReviewEditCommentContextMenuItem = new SpreadsheetCommandId(3503);
		public static readonly SpreadsheetCommandId ReviewDeleteComment = new SpreadsheetCommandId(3504);
		public static readonly SpreadsheetCommandId ReviewDeleteCommentContextMenuItem = new SpreadsheetCommandId(3505);
		public static readonly SpreadsheetCommandId ReviewShowHideComment = new SpreadsheetCommandId(3506);
		public static readonly SpreadsheetCommandId ReviewShowHideCommentContextMenuItem = new SpreadsheetCommandId(3507);
		public static readonly SpreadsheetCommandId ReviewShowAllComment = new SpreadsheetCommandId(3508);
		public static readonly SpreadsheetCommandId CommentCloseEditor = new SpreadsheetCommandId(3600);
		public static readonly SpreadsheetCommandId DataValidationCloseEditor = new SpreadsheetCommandId(3601);
		public static readonly SpreadsheetCommandId DataValidationInplaceEndEdit = new SpreadsheetCommandId(3602);
		public static readonly SpreadsheetCommandId PageSetup = new SpreadsheetCommandId(3700);
		public static readonly SpreadsheetCommandId PageSetupMargins = new SpreadsheetCommandId(3701);
		public static readonly SpreadsheetCommandId PageSetupHeaderFooter = new SpreadsheetCommandId(3702);
		public static readonly SpreadsheetCommandId PageSetupSheet = new SpreadsheetCommandId(3703);
		public static readonly SpreadsheetCommandId PageSetupCustomMargins = new SpreadsheetCommandId(3704);
		public static readonly SpreadsheetCommandId PageSetupMorePaperSizes = new SpreadsheetCommandId(3705);
		public static readonly SpreadsheetCommandId InsertPivotTable = new SpreadsheetCommandId(3800);
		public static readonly SpreadsheetCommandId OptionsPivotTable = new SpreadsheetCommandId(3801);
		public static readonly SpreadsheetCommandId OptionsPivotTableContextMenuItem = new SpreadsheetCommandId(3802);
		public static readonly SpreadsheetCommandId MovePivotTable = new SpreadsheetCommandId(3803);
		public static readonly SpreadsheetCommandId ChangeDataSourcePivotTable = new SpreadsheetCommandId(3804);
		public static readonly SpreadsheetCommandId SelectFieldTypePivotTable = new SpreadsheetCommandId(3805);
		public static readonly SpreadsheetCommandId FieldSettingsPivotTable = new SpreadsheetCommandId(3806);
		public static readonly SpreadsheetCommandId FieldSettingsPivotTableContextMenuItem = new SpreadsheetCommandId(3807);
		public static readonly SpreadsheetCommandId DataFieldSettingsPivotTable = new SpreadsheetCommandId(3808);
		public static readonly SpreadsheetCommandId DataFieldSettingsPivotTableContextMenuItem = new SpreadsheetCommandId(3809);
		public static readonly SpreadsheetCommandId FieldListPanelPivotTable = new SpreadsheetCommandId(3810);
		public static readonly SpreadsheetCommandId PivotTableActionsClearGroup = new SpreadsheetCommandId(3811);
		public static readonly SpreadsheetCommandId ClearAllPivotTable = new SpreadsheetCommandId(3812);
		public static readonly SpreadsheetCommandId ClearFiltersPivotTable = new SpreadsheetCommandId(3813);
		public static readonly SpreadsheetCommandId PivotTableActionsSelectGroup = new SpreadsheetCommandId(3814);
		public static readonly SpreadsheetCommandId SelectValuesPivotTable = new SpreadsheetCommandId(3815);
		public static readonly SpreadsheetCommandId SelectLabelsPivotTable = new SpreadsheetCommandId(3816);
		public static readonly SpreadsheetCommandId SelectEntirePivotTable = new SpreadsheetCommandId(3817);
		public static readonly SpreadsheetCommandId PivotTableDataRefreshGroup = new SpreadsheetCommandId(3818);
		public static readonly SpreadsheetCommandId RefreshPivotTable = new SpreadsheetCommandId(3819);
		public static readonly SpreadsheetCommandId RefreshAllPivotTable = new SpreadsheetCommandId(3820);
		public static readonly SpreadsheetCommandId ShowPivotTableFieldHeaders = new SpreadsheetCommandId(3821);
		public static readonly SpreadsheetCommandId ShowPivotTableExpandCollapseButtons = new SpreadsheetCommandId(3822);
		public static readonly SpreadsheetCommandId PivotTableLayoutSubtotalsGroup = new SpreadsheetCommandId(3823);
		public static readonly SpreadsheetCommandId PivotTableLayoutGrandTotalsGroup = new SpreadsheetCommandId(3824);
		public static readonly SpreadsheetCommandId PivotTableLayoutReportLayoutGroup = new SpreadsheetCommandId(3825);
		public static readonly SpreadsheetCommandId PivotTableLayoutBlankRowsGroup = new SpreadsheetCommandId(3826);
		public static readonly SpreadsheetCommandId PivotTableDoNotShowSubtotals = new SpreadsheetCommandId(3827);
		public static readonly SpreadsheetCommandId PivotTableShowAllSubtotalsAtBottom = new SpreadsheetCommandId(3828);
		public static readonly SpreadsheetCommandId PivotTableShowAllSubtotalsAtTop = new SpreadsheetCommandId(3829);
		public static readonly SpreadsheetCommandId PivotTableGrandTotalsOffRowsColumns = new SpreadsheetCommandId(3830);
		public static readonly SpreadsheetCommandId PivotTableGrandTotalsOnRowsColumns = new SpreadsheetCommandId(3831);
		public static readonly SpreadsheetCommandId PivotTableGrandTotalsOnRowsOnly = new SpreadsheetCommandId(3832);
		public static readonly SpreadsheetCommandId PivotTableGrandTotalsOnColumnsOnly = new SpreadsheetCommandId(3833);
		public static readonly SpreadsheetCommandId PivotTableShowCompactForm = new SpreadsheetCommandId(3834);
		public static readonly SpreadsheetCommandId PivotTableShowOutlineForm = new SpreadsheetCommandId(3835);
		public static readonly SpreadsheetCommandId PivotTableShowTabularForm = new SpreadsheetCommandId(3836);
		public static readonly SpreadsheetCommandId PivotTableRepeatAllItemLabels = new SpreadsheetCommandId(3837);
		public static readonly SpreadsheetCommandId PivotTableDoNotRepeatItemLabels = new SpreadsheetCommandId(3838);
		public static readonly SpreadsheetCommandId PivotTableInsertBlankLineEachItem = new SpreadsheetCommandId(3839);
		public static readonly SpreadsheetCommandId PivotTableRemoveBlankLineEachItem = new SpreadsheetCommandId(3840);
		public static readonly SpreadsheetCommandId PivotTableToggleRowHeaders = new SpreadsheetCommandId(3841);
		public static readonly SpreadsheetCommandId PivotTableToggleColumnHeaders = new SpreadsheetCommandId(3842);
		public static readonly SpreadsheetCommandId PivotTableToggleBandedRows = new SpreadsheetCommandId(3843);
		public static readonly SpreadsheetCommandId PivotTableToggleBandedColumns = new SpreadsheetCommandId(3844);
		public static readonly SpreadsheetCommandId ModifyPivotTableStyles = new SpreadsheetCommandId(3845);
		public static readonly SpreadsheetCommandId PivotTableExpandField = new SpreadsheetCommandId(3846);
		public static readonly SpreadsheetCommandId PivotTableCollapseField = new SpreadsheetCommandId(3847);
		public static readonly SpreadsheetCommandId PivotTablePageFieldsFilterItems = new SpreadsheetCommandId(3850);
		public static readonly SpreadsheetCommandId PivotTableRowFieldsFilterItems = new SpreadsheetCommandId(3851);
		public static readonly SpreadsheetCommandId PivotTableColumnFieldsFilterItems = new SpreadsheetCommandId(3852);
		public static readonly SpreadsheetCommandId FieldListPanelPivotTableContextMenuItem = new SpreadsheetCommandId(3855);
		public static readonly SpreadsheetCommandId PivotTableExpandCollapseFieldCommandGroup = new SpreadsheetCommandId(3860);
		public static readonly SpreadsheetCommandId PivotTableExpandFieldContextMenuItem = new SpreadsheetCommandId(3861);
		public static readonly SpreadsheetCommandId PivotTableCollapseFieldContextMenuItem = new SpreadsheetCommandId(3862);
		public static readonly SpreadsheetCommandId PivotTableFieldSortCommandGroup = new SpreadsheetCommandId(3868);
		public static readonly SpreadsheetCommandId PivotTableFieldsFiltersCommandGroup = new SpreadsheetCommandId(3869);
		public static readonly SpreadsheetCommandId PivotTableLabelFiltersCommandGroup = new SpreadsheetCommandId(3870);
		public static readonly SpreadsheetCommandId PivotTableDateFiltersCommandGroup = new SpreadsheetCommandId(3871);
		public static readonly SpreadsheetCommandId PivotTableValueFiltersCommandGroup = new SpreadsheetCommandId(3872);
		public static readonly SpreadsheetCommandId PivotTableFilterAllDatesInPeriodCommandGroup = new SpreadsheetCommandId(3873);
		public static readonly SpreadsheetCommandId PivotTableDateFilterToday = new SpreadsheetCommandId(3874);
		public static readonly SpreadsheetCommandId PivotTableDateFilterYesterday = new SpreadsheetCommandId(3875);
		public static readonly SpreadsheetCommandId PivotTableDateFilterTomorrow = new SpreadsheetCommandId(3876);
		public static readonly SpreadsheetCommandId PivotTableDateFilterThisWeek = new SpreadsheetCommandId(3877);
		public static readonly SpreadsheetCommandId PivotTableDateFilterNextWeek = new SpreadsheetCommandId(3878);
		public static readonly SpreadsheetCommandId PivotTableDateFilterLastWeek = new SpreadsheetCommandId(3879);
		public static readonly SpreadsheetCommandId PivotTableDateFilterThisMonth = new SpreadsheetCommandId(3880);
		public static readonly SpreadsheetCommandId PivotTableDateFilterNextMonth = new SpreadsheetCommandId(3881);
		public static readonly SpreadsheetCommandId PivotTableDateFilterLastMonth = new SpreadsheetCommandId(3882);
		public static readonly SpreadsheetCommandId PivotTableDateFilterThisQuarter = new SpreadsheetCommandId(3883);
		public static readonly SpreadsheetCommandId PivotTableDateFilterNextQuarter = new SpreadsheetCommandId(3884);
		public static readonly SpreadsheetCommandId PivotTableDateFilterLastQuarter = new SpreadsheetCommandId(3885);
		public static readonly SpreadsheetCommandId PivotTableDateFilterThisYear = new SpreadsheetCommandId(3886);
		public static readonly SpreadsheetCommandId PivotTableDateFilterNextYear = new SpreadsheetCommandId(3887);
		public static readonly SpreadsheetCommandId PivotTableDateFilterLastYear = new SpreadsheetCommandId(3888);
		public static readonly SpreadsheetCommandId PivotTableDateFilterYearToDate = new SpreadsheetCommandId(3889);
		public static readonly SpreadsheetCommandId PivotTableDateFilterFirstQuarter = new SpreadsheetCommandId(3890);
		public static readonly SpreadsheetCommandId PivotTableDateFilterSecondQuarter = new SpreadsheetCommandId(3891);
		public static readonly SpreadsheetCommandId PivotTableDateFilterThirdQuarter = new SpreadsheetCommandId(3892);
		public static readonly SpreadsheetCommandId PivotTableDateFilterFourthQuarter = new SpreadsheetCommandId(3893);
		public static readonly SpreadsheetCommandId PivotTableDateFilterJanuary = new SpreadsheetCommandId(3894);
		public static readonly SpreadsheetCommandId PivotTableDateFilterFebruary = new SpreadsheetCommandId(3895);
		public static readonly SpreadsheetCommandId PivotTableDateFilterMarch = new SpreadsheetCommandId(3896);
		public static readonly SpreadsheetCommandId PivotTableDateFilterApril = new SpreadsheetCommandId(3897);
		public static readonly SpreadsheetCommandId PivotTableDateFilterMay = new SpreadsheetCommandId(3898);
		public static readonly SpreadsheetCommandId PivotTableDateFilterJune = new SpreadsheetCommandId(3899);
		public static readonly SpreadsheetCommandId PivotTableDateFilterJuly = new SpreadsheetCommandId(3900);
		public static readonly SpreadsheetCommandId PivotTableDateFilterAugust = new SpreadsheetCommandId(3901);
		public static readonly SpreadsheetCommandId PivotTableDateFilterSeptember = new SpreadsheetCommandId(3902);
		public static readonly SpreadsheetCommandId PivotTableDateFilterOctober = new SpreadsheetCommandId(3903);
		public static readonly SpreadsheetCommandId PivotTableDateFilterNovember = new SpreadsheetCommandId(3904);
		public static readonly SpreadsheetCommandId PivotTableDateFilterDecember = new SpreadsheetCommandId(3905);
		public static readonly SpreadsheetCommandId PivotTableDateFilterEquals = new SpreadsheetCommandId(3906);
		public static readonly SpreadsheetCommandId PivotTableDateFilterBefore = new SpreadsheetCommandId(3907);
		public static readonly SpreadsheetCommandId PivotTableDateFilterAfter = new SpreadsheetCommandId(3908);
		public static readonly SpreadsheetCommandId PivotTableDateFilterBetween = new SpreadsheetCommandId(3909);
		public static readonly SpreadsheetCommandId PivotTableDateFilterCustom = new SpreadsheetCommandId(3910);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterEquals = new SpreadsheetCommandId(3911);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterDoesNotEqual = new SpreadsheetCommandId(3912);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterBeginsWith = new SpreadsheetCommandId(3913);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterDoesNotBeginWith = new SpreadsheetCommandId(3914);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterEndsWith = new SpreadsheetCommandId(3915);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterDoesNotEndWith = new SpreadsheetCommandId(3916);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterContains = new SpreadsheetCommandId(3917);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterDoesNotContain = new SpreadsheetCommandId(3918);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterGreaterThan = new SpreadsheetCommandId(3919);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterGreaterThanOrEqual = new SpreadsheetCommandId(3920);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterLessThan = new SpreadsheetCommandId(3921);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterLessThanOrEqual = new SpreadsheetCommandId(3922);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterBetween = new SpreadsheetCommandId(3923);
		public static readonly SpreadsheetCommandId PivotTableLabelFilterNotBetween = new SpreadsheetCommandId(3924);
		public static readonly SpreadsheetCommandId PivotTableValueFilterEquals = new SpreadsheetCommandId(3925);
		public static readonly SpreadsheetCommandId PivotTableValueFilterDoesNotEqual = new SpreadsheetCommandId(3926);
		public static readonly SpreadsheetCommandId PivotTableValueFilterGreaterThan = new SpreadsheetCommandId(3927);
		public static readonly SpreadsheetCommandId PivotTableValueFilterGreaterThanOrEqual = new SpreadsheetCommandId(3928);
		public static readonly SpreadsheetCommandId PivotTableValueFilterLessThan = new SpreadsheetCommandId(3929);
		public static readonly SpreadsheetCommandId PivotTableValueFilterLessThanOrEqual = new SpreadsheetCommandId(3930);
		public static readonly SpreadsheetCommandId PivotTableValueFilterBetween = new SpreadsheetCommandId(3931);
		public static readonly SpreadsheetCommandId PivotTableValueFilterNotBetween = new SpreadsheetCommandId(3932);
		public static readonly SpreadsheetCommandId PivotTableValueFilterTop10 = new SpreadsheetCommandId(3933);
		public static readonly SpreadsheetCommandId PivotTableClearFieldFilters = new SpreadsheetCommandId(3934);
		public static readonly SpreadsheetCommandId PivotTableClearFieldLabelFilter = new SpreadsheetCommandId(3935);
		public static readonly SpreadsheetCommandId PivotTableClearFieldValueFilter = new SpreadsheetCommandId(3936);
		public static readonly SpreadsheetCommandId PivotTableFieldSortAscending = new SpreadsheetCommandId(3937);
		public static readonly SpreadsheetCommandId PivotTableFieldSortDescending = new SpreadsheetCommandId(3938);
		public static readonly SpreadsheetCommandId RemovePivotFieldCommand = new SpreadsheetCommandId(3939);
		public static readonly SpreadsheetCommandId RemoveGrandTotalPivotTable = new SpreadsheetCommandId(3940);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByCommandGroup = new SpreadsheetCommandId(3941);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesBySum = new SpreadsheetCommandId(3942);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByCount = new SpreadsheetCommandId(3943);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByMin = new SpreadsheetCommandId(3944);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByMax = new SpreadsheetCommandId(3945);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByAverage = new SpreadsheetCommandId(3946);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByProduct = new SpreadsheetCommandId(3947);
		public static readonly SpreadsheetCommandId PivotTableSummarizeValuesByMoreOptions = new SpreadsheetCommandId(3948);
		public static readonly SpreadsheetCommandId SubtotalPivotField = new SpreadsheetCommandId(3949);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceCommandGroup = new SpreadsheetCommandId(3950);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceUp = new SpreadsheetCommandId(3951);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceDown = new SpreadsheetCommandId(3952);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceToBeginning = new SpreadsheetCommandId(3953);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceToEnd = new SpreadsheetCommandId(3954);
		public static readonly SpreadsheetCommandId MovePivotFieldReferenceToDifferentAxis = new SpreadsheetCommandId(3955);
		public static readonly SpreadsheetCommandId MovePivotFieldItemUp = new SpreadsheetCommandId(3956);
		public static readonly SpreadsheetCommandId MovePivotFieldItemDown = new SpreadsheetCommandId(3957);
		public static readonly SpreadsheetCommandId MovePivotFieldItemToBeginning = new SpreadsheetCommandId(3958);
		public static readonly SpreadsheetCommandId MovePivotFieldItemToEnd = new SpreadsheetCommandId(3959);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsCommandGroup = new SpreadsheetCommandId(3960);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsNormal = new SpreadsheetCommandId(3961);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfTotal = new SpreadsheetCommandId(3962);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfColumn = new SpreadsheetCommandId(3963);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfParentColumn = new SpreadsheetCommandId(3964);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfRow = new SpreadsheetCommandId(3965);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfParentRow = new SpreadsheetCommandId(3966);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsIndex = new SpreadsheetCommandId(3967);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsMoreOptions = new SpreadsheetCommandId(3968);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfParent = new SpreadsheetCommandId(3969);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsRunningTotal = new SpreadsheetCommandId(3970);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentOfRunningTotal = new SpreadsheetCommandId(3971);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsRankAscending = new SpreadsheetCommandId(3972);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsRankDescending = new SpreadsheetCommandId(3973);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercent = new SpreadsheetCommandId(3974);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsDifference = new SpreadsheetCommandId(3975);
		public static readonly SpreadsheetCommandId PivotTableShowValuesAsPercentDifference = new SpreadsheetCommandId(3976);
		readonly int m_value;
		public SpreadsheetCommandId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is SpreadsheetCommandId) && (this.m_value == ((SpreadsheetCommandId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(SpreadsheetCommandId id1, SpreadsheetCommandId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(SpreadsheetCommandId id1, SpreadsheetCommandId id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<SpreadsheetCommandId> Members
		int IConvertToInt<SpreadsheetCommandId>.ToInt() {
			return m_value;
		}
		SpreadsheetCommandId IConvertToInt<SpreadsheetCommandId>.FromInt(int value) {
			return new SpreadsheetCommandId(value);
		}
		#endregion
		#region IEquatable<SpreadsheetCommandId> Members
		public bool Equals(SpreadsheetCommandId other) {
			return this.m_value == other.m_value;
		}
		#endregion
		public static SpreadsheetCommandId GetCommandId(string commandName) {
			SpreadsheetCommandId result;
			if (!commandMap.TryGetValue(commandName, out result))
				throw new Exception("Command '" + commandName + "' doesn't exist.");
			else
				return result;
		}
		public static string GetCommandName(SpreadsheetCommandId id) {
			foreach (string name in commandMap.Keys)
				if (commandMap[name] == id)
					return name;
			return String.Empty;
		}
		static readonly Dictionary<string, SpreadsheetCommandId> commandMap = CreateCommandMap();
		static Dictionary<string, SpreadsheetCommandId> CreateCommandMap() {
			Dictionary<string, SpreadsheetCommandId> result = new Dictionary<string, SpreadsheetCommandId>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			FieldInfo[] properties = typeof(SpreadsheetCommandId).GetFields(BindingFlags.Static | BindingFlags.Public);
			int count = properties.Length;
			for (int i = 0; i < count; i++) {
				FieldInfo info = properties[i];
				if (info.FieldType == typeof(SpreadsheetCommandId))
					result.Add(info.Name, (SpreadsheetCommandId)info.GetValue(null));
			}
			return result;
		}
	}
	#endregion
	#region SpreadsheetCommandBase (abstract class)
	public abstract class SpreadsheetCommandBase<TLocalizedStringId> : ControlCommand<ISpreadsheetControl, SpreadsheetCommandId, TLocalizedStringId> where TLocalizedStringId : struct {
		protected SpreadsheetCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected internal DocumentModel DocumentModel { get { return DocumentServer.DocumentModel; } }
		protected internal SpreadsheetView ActiveView { get { return InnerControl.ActiveView; } }
		protected internal SpreadsheetViewType ActiveViewType { get { return InnerControl.ActiveViewType; } }
		protected internal DocumentOptions Options { get { return DocumentServer.Options; } }
		protected internal DevExpress.XtraSpreadsheet.Model.Worksheet ActiveSheet { get { return DocumentModel.ActiveSheet; } }
		protected internal bool IsContentEditable {
			get {
				if (!DocumentServer.IsEditable)
					return false;
				return !InnerControl.DocumentModel.ReferenceEditMode;
			}
		}
		protected internal InnerSpreadsheetControl InnerControl { get { return Control.InnerControl; } }
		protected internal InnerSpreadsheetDocumentServer DocumentServer { get { return Control.InnerDocumentServer; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.None; } }
		protected WorksheetProtectionOptions Protection { get { return ActiveSheet.Properties.Protection; } }
		protected WorkbookProtectionOptions WorkbookProtection { get { return DocumentModel.Properties.Protection; } }
		#endregion
		protected internal virtual void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option) {
			state.Enabled = option != DocumentCapability.Disabled && option != DocumentCapability.Hidden;
			state.Visible = option != DocumentCapability.Hidden;
		}
		protected internal virtual void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option, bool additionEnabledCondition) {
			ApplyCommandsRestriction(state, option);
			state.Enabled = state.Enabled && additionEnabledCondition;
		}
		protected internal virtual void ApplyCommandRestrictionOnEditableControl(ICommandUIState state, DocumentCapability option) {
			state.Enabled = IsContentEditable && option != DocumentCapability.Disabled && option != DocumentCapability.Hidden;
			state.Visible = option != DocumentCapability.Hidden;
		}
		protected internal virtual void ApplyCommandRestrictionOnEditableControl(ICommandUIState state, DocumentCapability option, bool additionEnabledCondition) {
			ApplyCommandRestrictionOnEditableControl(state, option);
			state.Enabled = state.Enabled && additionEnabledCondition;
		}
		protected internal virtual void ApplyActiveSheetProtection(ICommandUIState state) {
			ApplyActiveSheetProtection(state, !Protection.SheetLocked);
		}
		protected internal virtual void ApplyActiveSheetProtection(ICommandUIState state, bool option) {
			if (ActiveSheet.ReadOnly) {
				state.Enabled = false;
				return;
			}
			WorksheetProtectionOptions options = Protection;
			if (!options.SheetLocked)
				return;
			state.Enabled = state.Enabled && option;
		}
		protected internal virtual void ApplyWorkbookProtection(ICommandUIState state, bool option) {
			WorkbookProtectionOptions options = WorkbookProtection;
			if (!options.IsLocked)
				return;
			state.Enabled = state.Enabled && !option;
		}
		protected internal virtual void CheckExecutedAtUIThread() {
		}
	}
	#endregion
	#region SpreadsheetCommand (abstract class)
	public abstract class SpreadsheetCommand : SpreadsheetCommandBase<XtraSpreadsheetStringId> {
		static SpreadsheetCommand() {
			OfficeLocalizer.GetString(OfficeStringId.Msg_InternalError);
			XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorBlankSheetName);
		}
		protected SpreadsheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override XtraLocalizer<XtraSpreadsheetStringId> Localizer { get { return XtraSpreadsheetLocalizer.Active; } }
		protected virtual bool UseOfficeTextsAndImage { get { return false; } }
		protected virtual bool UseOfficeImage { get { return UseOfficeTextsAndImage; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.Msg_ErrorInternalError; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.Msg_ErrorInternalError; } }
		public virtual OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.Msg_InternalError; } }
		public virtual OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.Msg_InternalError; } }
		XtraLocalizer<OfficeStringId> LocalizerOffice { get { return OfficeLocalizer.Active; } }
		string OfficeMenuCaption { get { return LocalizerOffice.GetLocalizedString(OfficeMenuCaptionStringId); } }
		string OfficeDescription { get { return LocalizerOffice.GetLocalizedString(OfficeDescriptionStringId); } }
		public override string MenuCaption {
			get {
				if (UseOfficeTextsAndImage)
					return OfficeMenuCaption;
				else
					return base.MenuCaption;
			}
		}
		public override string Description {
			get {
				if (UseOfficeTextsAndImage)
					return OfficeDescription;
				else
					return base.Description;
			}
		}
		protected override string ImageResourcePrefix {
			get {
				if (UseOfficeImage)
					return "DevExpress.Office.Images";
				else
					return "DevExpress.XtraSpreadsheet.Images";
			}
		}
		protected override Assembly ImageResourceAssembly {
			get {
				if (UseOfficeImage)
					return typeof(DevExpress.Office.DocumentUnit).GetAssembly();
				else
					return typeof(SpreadsheetCommand).GetAssembly();
			}
		}
		protected internal IErrorHandler ErrorHandler { get { return Control.InnerControl.ErrorHandler; } }
		#endregion
	}
	#endregion
}
