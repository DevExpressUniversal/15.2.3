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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing;
using System.Text;
using System.Drawing.Printing;
#else
using System.Windows.Media;
using System.Text;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfArtBorderConverter
	public static class RtfArtBorderConverter {		
		static Dictionary<BorderLineStyle, int> mapBorderLineStyleToIndex;
		static Dictionary<int, BorderLineStyle> mapIndexToBorderLineStyle;
		#region static constructor
		static RtfArtBorderConverter() {
			BorderLineStyle[] lineStyles = new BorderLineStyle[] {
				BorderLineStyle.Apples,
				BorderLineStyle.ArchedScallops,
				BorderLineStyle.BabyPacifier,
				BorderLineStyle.BabyRattle,
				BorderLineStyle.Balloons3Colors,
				BorderLineStyle.BalloonsHotAir,
				BorderLineStyle.BasicBlackDashes,
				BorderLineStyle.BasicBlackDots,
				BorderLineStyle.BasicBlackSquares,
				BorderLineStyle.BasicThinLines,
				BorderLineStyle.BasicWhiteDashes,
				BorderLineStyle.BasicWhiteDots,
				BorderLineStyle.BasicWhiteSquares,
				BorderLineStyle.BasicWideInline,
				BorderLineStyle.BasicWideMidline,
				BorderLineStyle.BasicWideOutline,
				BorderLineStyle.Bats,
				BorderLineStyle.Birds,
				BorderLineStyle.BirdsFlight,
				BorderLineStyle.Cabins,
				BorderLineStyle.CakeSlice,
				BorderLineStyle.CandyCorn,
				BorderLineStyle.CelticKnotwork,
				BorderLineStyle.CertificateBanner,
				BorderLineStyle.ChainLink,
				BorderLineStyle.ChampagneBottle,
				BorderLineStyle.CheckedBarBlack,
				BorderLineStyle.CheckedBarColor,
				BorderLineStyle.Checkered,
				BorderLineStyle.ChristmasTree,
				BorderLineStyle.CirclesLines,
				BorderLineStyle.CirclesRectangles,
				BorderLineStyle.ClassicalWave,
				BorderLineStyle.Clocks,
				BorderLineStyle.Compass,
				BorderLineStyle.Confetti,
				BorderLineStyle.ConfettiGrays,
				BorderLineStyle.ConfettiOutline,
				BorderLineStyle.ConfettiStreamers,
				BorderLineStyle.ConfettiWhite,
				BorderLineStyle.CornerTriangles,
				BorderLineStyle.CouponCutoutDashes,
				BorderLineStyle.CouponCutoutDots,
				BorderLineStyle.CrazyMaze,
				BorderLineStyle.CreaturesButterfly,
				BorderLineStyle.CreaturesFish,
				BorderLineStyle.CreaturesInsects,
				BorderLineStyle.CreaturesLadyBug,
				BorderLineStyle.CrossStitch,
				BorderLineStyle.Cup,
				BorderLineStyle.DecoArch,
				BorderLineStyle.DecoArchColor,
				BorderLineStyle.DecoBlocks,
				BorderLineStyle.DiamondsGray,
				BorderLineStyle.DoubleD,
				BorderLineStyle.DoubleDiamonds,
				BorderLineStyle.Earth1,
				BorderLineStyle.Earth2,
				BorderLineStyle.EclipsingSquares1,
				BorderLineStyle.EclipsingSquares2,
				BorderLineStyle.EggsBlack,
				BorderLineStyle.Fans,
				BorderLineStyle.Film,
				BorderLineStyle.Firecrackers,
				BorderLineStyle.FlowersBlockPrint,
				BorderLineStyle.FlowersDaisies,
				BorderLineStyle.FlowersModern1,
				BorderLineStyle.FlowersModern2,
				BorderLineStyle.FlowersPansy,
				BorderLineStyle.FlowersRedRose,
				BorderLineStyle.FlowersRoses,
				BorderLineStyle.FlowersTeacup,
				BorderLineStyle.FlowersTiny,
				BorderLineStyle.Gems,
				BorderLineStyle.GingerbreadMan,
				BorderLineStyle.Gradient,
				BorderLineStyle.Handmade1,
				BorderLineStyle.Handmade2,
				BorderLineStyle.HeartBalloon,
				BorderLineStyle.HeartGray,
				BorderLineStyle.Hearts,
				BorderLineStyle.HeebieJeebies,
				BorderLineStyle.Holly,
				BorderLineStyle.HouseFunky,
				BorderLineStyle.Hypnotic,
				BorderLineStyle.IceCreamCones,
				BorderLineStyle.LightBulb,
				BorderLineStyle.Lightning1,
				BorderLineStyle.Lightning2,
				BorderLineStyle.MapPins,
				BorderLineStyle.MapleLeaf,
				BorderLineStyle.MapleMuffins,
				BorderLineStyle.Marquee,
				BorderLineStyle.MarqueeToothed,
				BorderLineStyle.Moons,
				BorderLineStyle.Mosaic,
				BorderLineStyle.MusicNotes,
				BorderLineStyle.Northwest,
				BorderLineStyle.Ovals,
				BorderLineStyle.Packages,
				BorderLineStyle.PalmsBlack,
				BorderLineStyle.PalmsColor,
				BorderLineStyle.PaperClips,
				BorderLineStyle.Papyrus,
				BorderLineStyle.PartyFavor,
				BorderLineStyle.PartyGlass,
				BorderLineStyle.Pencils,
				BorderLineStyle.People,
				BorderLineStyle.PeopleWaving,
				BorderLineStyle.PeopleHats,
				BorderLineStyle.Poinsettias,
				BorderLineStyle.PostageStamp,
				BorderLineStyle.Pumpkin1,
				BorderLineStyle.PushPinNote2,
				BorderLineStyle.PushPinNote1,
				BorderLineStyle.Pyramids,
				BorderLineStyle.PyramidsAbove,
				BorderLineStyle.Quadrants,
				BorderLineStyle.Rings,
				BorderLineStyle.Safari,
				BorderLineStyle.Sawtooth,
				BorderLineStyle.SawtoothGray,
				BorderLineStyle.ScaredCat,
				BorderLineStyle.Seattle,
				BorderLineStyle.ShadowedSquares,
				BorderLineStyle.SharksTeeth,
				BorderLineStyle.ShorebirdTracks,
				BorderLineStyle.Skyrocket,
				BorderLineStyle.SnowflakeFancy,
				BorderLineStyle.Snowflakes,
				BorderLineStyle.Sombrero,
				BorderLineStyle.Southwest,
				BorderLineStyle.Stars,
				BorderLineStyle.StarsTop,
				BorderLineStyle.Stars3d,
				BorderLineStyle.StarsBlack,
				BorderLineStyle.StarsShadowed,
				BorderLineStyle.Sun,
				BorderLineStyle.Swirligig,
				BorderLineStyle.TornPaper,
				BorderLineStyle.TornPaperBlack,
				BorderLineStyle.Trees,
				BorderLineStyle.TriangleParty,
				BorderLineStyle.Triangles,
				BorderLineStyle.Tribal1,
				BorderLineStyle.Tribal2,
				BorderLineStyle.Tribal3,
				BorderLineStyle.Tribal4,
				BorderLineStyle.Tribal5,
				BorderLineStyle.Tribal6,
				BorderLineStyle.TwistedLines1,
				BorderLineStyle.TwistedLines2,
				BorderLineStyle.Vine,
				BorderLineStyle.Waveline,
				BorderLineStyle.WeavingAngles,
				BorderLineStyle.WeavingBraid,
				BorderLineStyle.WeavingRibbon,
				BorderLineStyle.WeavingStrips,
				BorderLineStyle.WhiteFlowers,
				BorderLineStyle.Woodwork,
				BorderLineStyle.XIllusions,
				BorderLineStyle.ZanyTriangles,
				BorderLineStyle.ZigZag,
				BorderLineStyle.ZigZagStitch};
			int count = lineStyles.Length;
			mapBorderLineStyleToIndex = new Dictionary<BorderLineStyle, int>();
			mapIndexToBorderLineStyle = new Dictionary<int, BorderLineStyle>();
			for (int i = 0; i < count; i++) {
				mapBorderLineStyleToIndex.Add(lineStyles[i], i + 1);
				mapIndexToBorderLineStyle.Add(i + 1, lineStyles[i]);
			}
		}
		#endregion
		public static int GetBorderArtIndex(BorderLineStyle borderLineStyle) {
			int result;
			if (mapBorderLineStyleToIndex.TryGetValue(borderLineStyle, out result))
				return result;
			else
				return 0;
		}
		public static BorderLineStyle GetBorderLineStyle(int borderArtIndex) {
			BorderLineStyle result;
			if (mapIndexToBorderLineStyle.TryGetValue(borderArtIndex, out result))
				return result;
			else
				return BorderLineStyle.None;			
		}
	}
	#endregion
	#region RtfContentExporter
	public class RtfContentExporter : DocumentModelExporter {
		#region Fields
		static readonly Dictionary<VerticalAlignment, string> verticalAlignmentTypes = CreateVerticalAlignmentTypesTable();
		static readonly Dictionary<SectionStartType, string> sectionStartTypes = CreateSectionStartTypesTable();
		static readonly Dictionary<char, string> chapterSeparatorTypes = CreateChapterSeparatorTypesTable();
		static readonly Dictionary<NumberingFormat, string> pageNumberingTypes = CreatePageNumberingTypesTable();
		static readonly Dictionary<NumberingFormat, string> sectionFootNoteNumberingTypes = CreateSectionFootNoteNumberingTypesTable();
		static readonly Dictionary<NumberingFormat, string> sectionEndNoteNumberingTypes = CreateSectionEndNoteNumberingTypesTable();
		static readonly Dictionary<NumberingFormat, string> footNoteNumberingTypes = CreateFootNoteNumberingTypesTable();
		static readonly Dictionary<NumberingFormat, string> endNoteNumberingTypes = CreateEndNoteNumberingTypesTable();
		static readonly Dictionary<BorderLineStyle, string> borderLineStyles = CreateBorderLineStylesTable();
		static readonly Dictionary<ConditionalTableStyleFormattingTypes, string> conditionalStylesTypes = CreateConditionalStylesTable();
		static readonly Dictionary<int, string> predefinedUserGroups = CreatePredefinedUserGroups();
		readonly IRtfExportHelper rtfExportHelper;
		readonly RtfBuilder rtfBuilder;
		readonly RtfParagraphPropertiesExporter paragraphPropertiesExporter;
		readonly RtfCharacterPropertiesExporter characterPropertiesExporter;
		readonly RtfDocumentExporterOptions options;
		bool lastParagraphRunNotSelected;
		bool keepFieldCodeViewState;
		RunMergedCharacterPropertiesCachedResult getMergedCharacterPropertiesCachedResult;
		#endregion
		#region Static Tables Initialization
		#region CreateBorderLineStylesTable
		static Dictionary<BorderLineStyle, string> CreateBorderLineStylesTable() {
			Dictionary<BorderLineStyle, string> result = new Dictionary<BorderLineStyle, string>();
			result.Add(BorderLineStyle.DashDotStroked, RtfExportSR.BorderDashDotStroked);
			result.Add(BorderLineStyle.Dashed, RtfExportSR.BorderDashed);
			result.Add(BorderLineStyle.DashSmallGap, RtfExportSR.BorderDashedSmall);
			result.Add(BorderLineStyle.DotDash, RtfExportSR.BorderDotDashed);
			result.Add(BorderLineStyle.DotDotDash, RtfExportSR.BorderDotDotDashed);
			result.Add(BorderLineStyle.Dotted, RtfExportSR.BorderDotted);
			result.Add(BorderLineStyle.Double, RtfExportSR.BorderDouble);
			result.Add(BorderLineStyle.DoubleWave, RtfExportSR.BorderDoubleWavy);
			result.Add(BorderLineStyle.Inset, RtfExportSR.BorderInset);
			result.Add(BorderLineStyle.None, RtfExportSR.BorderNone); 
			result.Add(BorderLineStyle.Nil, RtfExportSR.NoBorder);
			result.Add(BorderLineStyle.Outset, RtfExportSR.BorderOutset);
			result.Add(BorderLineStyle.Single, RtfExportSR.BorderSingle);
			result.Add(BorderLineStyle.ThickThinLargeGap, RtfExportSR.BorderThickThinLarge);
			result.Add(BorderLineStyle.ThickThinMediumGap, RtfExportSR.BorderThickThinMedium);
			result.Add(BorderLineStyle.ThickThinSmallGap, RtfExportSR.BorderThickThinSmall);
			result.Add(BorderLineStyle.ThinThickLargeGap, RtfExportSR.BorderThinThickLarge);
			result.Add(BorderLineStyle.ThinThickMediumGap, RtfExportSR.BorderThinThickMedium);
			result.Add(BorderLineStyle.ThinThickSmallGap, RtfExportSR.BorderThinThickSmall);
			result.Add(BorderLineStyle.ThinThickThinLargeGap, RtfExportSR.BorderThinThickThinLarge);
			result.Add(BorderLineStyle.ThinThickThinMediumGap, RtfExportSR.BorderThinThickThinMedium);
			result.Add(BorderLineStyle.ThinThickThinSmallGap, RtfExportSR.BorderThinThickThinSmall);
			result.Add(BorderLineStyle.ThreeDEmboss, RtfExportSR.BorderThreeDEmboss);
			result.Add(BorderLineStyle.ThreeDEngrave, RtfExportSR.BorderThreeDEngrave);
			result.Add(BorderLineStyle.Triple, RtfExportSR.BorderTriple);
			result.Add(BorderLineStyle.Wave, RtfExportSR.BorderWavy);
			return result;
		}
		#endregion
		static Dictionary<ConditionalTableStyleFormattingTypes, string> CreateConditionalStylesTable() {
			Dictionary<ConditionalTableStyleFormattingTypes, string> result = new Dictionary<ConditionalTableStyleFormattingTypes, string>();
			result.Add(ConditionalTableStyleFormattingTypes.BottomLeftCell, RtfExportSR.TableConditionalStyleBottomLeftCell);
			result.Add(ConditionalTableStyleFormattingTypes.BottomRightCell, RtfExportSR.TableConditionalStyleBottomRightCell);
			result.Add(ConditionalTableStyleFormattingTypes.EvenColumnBanding, RtfExportSR.TableConditionalStyleEvenColumnBanding);
			result.Add(ConditionalTableStyleFormattingTypes.EvenRowBanding, RtfExportSR.TableConditionalStyleEvenRowBanding);
			result.Add(ConditionalTableStyleFormattingTypes.FirstColumn, RtfExportSR.TableConditionalStyleFirstColumn);
			result.Add(ConditionalTableStyleFormattingTypes.FirstRow, RtfExportSR.TableConditionalStyleFirstRow);
			result.Add(ConditionalTableStyleFormattingTypes.LastColumn, RtfExportSR.TableConditionalStyleLastColumn);
			result.Add(ConditionalTableStyleFormattingTypes.LastRow, RtfExportSR.TableConditionalStyleLastRow);
			result.Add(ConditionalTableStyleFormattingTypes.OddColumnBanding, RtfExportSR.TableConditionalStyleOddColumnBanding);
			result.Add(ConditionalTableStyleFormattingTypes.OddRowBanding, RtfExportSR.TableConditionalStyleOddRowBanding);
			result.Add(ConditionalTableStyleFormattingTypes.TopLeftCell, RtfExportSR.TableConditionalStyleTopLeftCell);
			result.Add(ConditionalTableStyleFormattingTypes.TopRightCell, RtfExportSR.TableConditionalStyleTopRightCell);
			return result;
		}
		#region CreateVerticalAlignmentTypesTable
		static Dictionary<VerticalAlignment, string> CreateVerticalAlignmentTypesTable() {
			Dictionary<VerticalAlignment, string> result = new Dictionary<VerticalAlignment, string>();
			result.Add(VerticalAlignment.Both, RtfExportSR.VerticalAlignmentJustify);
			result.Add(VerticalAlignment.Bottom, RtfExportSR.VerticalAlignmentBottom);
			result.Add(VerticalAlignment.Center, RtfExportSR.VerticalAlignmentCenter);
			result.Add(VerticalAlignment.Top, RtfExportSR.VerticalAlignmentTop);
			return result;
		}
		#endregion
		#region CreateSectionBreakTypesTable
		static Dictionary<SectionStartType, string> CreateSectionStartTypesTable() {
			Dictionary<SectionStartType, string> result = new Dictionary<SectionStartType, string>();
			result.Add(SectionStartType.Continuous, RtfExportSR.SectionBreakTypeContinuous);
			result.Add(SectionStartType.NextPage, RtfExportSR.SectionBreakTypeNextPage);
			result.Add(SectionStartType.OddPage, RtfExportSR.SectionBreakTypeOddPage);
			result.Add(SectionStartType.EvenPage, RtfExportSR.SectionBreakTypeEvenPage);
			result.Add(SectionStartType.Column, RtfExportSR.SectionBreakTypeColumn);
			return result;
		}
		#endregion
		#region CreateChapterSeparatorTypesTable
		static Dictionary<char, string> CreateChapterSeparatorTypesTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			result.Add(Characters.Hyphen, RtfExportSR.SectionChapterSeparatorHyphen);
			result.Add('.', RtfExportSR.SectionChapterSeparatorPeriod);
			result.Add(':', RtfExportSR.SectionChapterSeparatorColon);
			result.Add(Characters.EmDash, RtfExportSR.SectionChapterSeparatorEmDash);
			result.Add(Characters.EnDash, RtfExportSR.SectionChapterSeparatorEnDash);
			return result;
		}
		#endregion
		#region CreatePageNumberingTypesTable
		static Dictionary<NumberingFormat, string> CreatePageNumberingTypesTable() {
			Dictionary<NumberingFormat, string> result = new Dictionary<NumberingFormat, string>();
			result.Add(NumberingFormat.Decimal, RtfExportSR.SectionPageNumberingDecimal);
			result.Add(NumberingFormat.UpperRoman, RtfExportSR.SectionPageNumberingUpperRoman);
			result.Add(NumberingFormat.LowerRoman, RtfExportSR.SectionPageNumberingLowerRoman);
			result.Add(NumberingFormat.UpperLetter, RtfExportSR.SectionPageNumberingUpperLetter);
			result.Add(NumberingFormat.LowerLetter, RtfExportSR.SectionPageNumberingLowerLetter);
			result.Add(NumberingFormat.ArabicAbjad, RtfExportSR.SectionPageNumberingArabicAbjad);
			result.Add(NumberingFormat.ArabicAlpha, RtfExportSR.SectionPageNumberingArabicAlpha);
			result.Add(NumberingFormat.Chosung, RtfExportSR.SectionPageNumberingChosung);
			result.Add(NumberingFormat.DecimalEnclosedCircle, RtfExportSR.SectionPageNumberingDecimalEnclosedCircle);
			result.Add(NumberingFormat.DecimalFullWidth, RtfExportSR.SectionPageNumberingDecimalFullWidth);
			result.Add(NumberingFormat.Ganada, RtfExportSR.SectionPageNumberingGanada);
			result.Add(NumberingFormat.HindiVowels, RtfExportSR.SectionPageNumberingHindiVowels);
			result.Add(NumberingFormat.HindiConsonants, RtfExportSR.SectionPageNumberingHindiConsonants);
			result.Add(NumberingFormat.HindiNumbers, RtfExportSR.SectionPageNumberingHindiNumbers);
			result.Add(NumberingFormat.HindiDescriptive, RtfExportSR.SectionPageNumberingHindiDescriptive);
			result.Add(NumberingFormat.ThaiLetters, RtfExportSR.SectionPageNumberingThaiLetters);
			result.Add(NumberingFormat.ThaiNumbers, RtfExportSR.SectionPageNumberingThaiNumbers);
			result.Add(NumberingFormat.ThaiDescriptive, RtfExportSR.SectionPageNumberingThaiDescriptive);
			result.Add(NumberingFormat.VietnameseDescriptive, RtfExportSR.SectionPageNumberingVietnameseDescriptive);
			return result;
		}
		#endregion
		#region CreateSectionFootNoteNumberingTypesTable
		static Dictionary<NumberingFormat, string> CreateSectionFootNoteNumberingTypesTable() {
			Dictionary<NumberingFormat, string> result = new Dictionary<NumberingFormat, string>();
			result.Add(NumberingFormat.Decimal, RtfExportSR.SectionFootNoteNumberingFormatDecimal);
			result.Add(NumberingFormat.UpperRoman, RtfExportSR.SectionFootNoteNumberingFormatUpperRoman);
			result.Add(NumberingFormat.LowerRoman, RtfExportSR.SectionFootNoteNumberingFormatLowerRoman);
			result.Add(NumberingFormat.UpperLetter, RtfExportSR.SectionFootNoteNumberingFormatUpperLetter);
			result.Add(NumberingFormat.LowerLetter, RtfExportSR.SectionFootNoteNumberingFormatLowerLetter);
			result.Add(NumberingFormat.Chicago, RtfExportSR.SectionFootNoteNumberingFormatChicago);
			result.Add(NumberingFormat.Chosung, RtfExportSR.SectionFootNoteNumberingFormatChosung);
			result.Add(NumberingFormat.DecimalEnclosedCircle, RtfExportSR.SectionFootNoteNumberingFormatDecimalEnclosedCircle);
			result.Add(NumberingFormat.DecimalFullWidth, RtfExportSR.SectionFootNoteNumberingFormatDecimalFullWidth);
			result.Add(NumberingFormat.Ganada, RtfExportSR.SectionFootNoteNumberingFormatGanada);
			return result;
		}
		#endregion
		#region CreateSectionEndNoteNumberingTypesTable
		static Dictionary<NumberingFormat, string> CreateSectionEndNoteNumberingTypesTable() {
			Dictionary<NumberingFormat, string> result = new Dictionary<NumberingFormat, string>();
			result.Add(NumberingFormat.Decimal, RtfExportSR.SectionEndNoteNumberingFormatDecimal);
			result.Add(NumberingFormat.UpperRoman, RtfExportSR.SectionEndNoteNumberingFormatUpperRoman);
			result.Add(NumberingFormat.LowerRoman, RtfExportSR.SectionEndNoteNumberingFormatLowerRoman);
			result.Add(NumberingFormat.UpperLetter, RtfExportSR.SectionEndNoteNumberingFormatUpperLetter);
			result.Add(NumberingFormat.LowerLetter, RtfExportSR.SectionEndNoteNumberingFormatLowerLetter);
			result.Add(NumberingFormat.Chicago, RtfExportSR.SectionEndNoteNumberingFormatChicago);
			result.Add(NumberingFormat.Chosung, RtfExportSR.SectionEndNoteNumberingFormatChosung);
			result.Add(NumberingFormat.DecimalEnclosedCircle, RtfExportSR.SectionEndNoteNumberingFormatDecimalEnclosedCircle);
			result.Add(NumberingFormat.DecimalFullWidth, RtfExportSR.SectionEndNoteNumberingFormatDecimalFullWidth);
			result.Add(NumberingFormat.Ganada, RtfExportSR.SectionEndNoteNumberingFormatGanada);
			return result;
		}
		#endregion
		#region CreateFootNoteNumberingTypesTable
		static Dictionary<NumberingFormat, string> CreateFootNoteNumberingTypesTable() {
			Dictionary<NumberingFormat, string> result = new Dictionary<NumberingFormat, string>();
			result.Add(NumberingFormat.Decimal, RtfExportSR.FootNoteNumberingFormatDecimal);
			result.Add(NumberingFormat.UpperRoman, RtfExportSR.FootNoteNumberingFormatUpperRoman);
			result.Add(NumberingFormat.LowerRoman, RtfExportSR.FootNoteNumberingFormatLowerRoman);
			result.Add(NumberingFormat.UpperLetter, RtfExportSR.FootNoteNumberingFormatUpperLetter);
			result.Add(NumberingFormat.LowerLetter, RtfExportSR.FootNoteNumberingFormatLowerLetter);
			result.Add(NumberingFormat.Chicago, RtfExportSR.FootNoteNumberingFormatChicago);
			result.Add(NumberingFormat.Chosung, RtfExportSR.FootNoteNumberingFormatChosung);
			result.Add(NumberingFormat.DecimalEnclosedCircle, RtfExportSR.FootNoteNumberingFormatDecimalEnclosedCircle);
			result.Add(NumberingFormat.DecimalFullWidth, RtfExportSR.FootNoteNumberingFormatDecimalFullWidth);
			result.Add(NumberingFormat.Ganada, RtfExportSR.FootNoteNumberingFormatGanada);
			return result;
		}
		#endregion
		#region CreateEndNoteNumberingTypesTable
		static Dictionary<NumberingFormat, string> CreateEndNoteNumberingTypesTable() {
			Dictionary<NumberingFormat, string> result = new Dictionary<NumberingFormat, string>();
			result.Add(NumberingFormat.Decimal, RtfExportSR.EndNoteNumberingFormatDecimal);
			result.Add(NumberingFormat.UpperRoman, RtfExportSR.EndNoteNumberingFormatUpperRoman);
			result.Add(NumberingFormat.LowerRoman, RtfExportSR.EndNoteNumberingFormatLowerRoman);
			result.Add(NumberingFormat.UpperLetter, RtfExportSR.EndNoteNumberingFormatUpperLetter);
			result.Add(NumberingFormat.LowerLetter, RtfExportSR.EndNoteNumberingFormatLowerLetter);
			result.Add(NumberingFormat.Chicago, RtfExportSR.EndNoteNumberingFormatChicago);
			result.Add(NumberingFormat.Chosung, RtfExportSR.EndNoteNumberingFormatChosung);
			result.Add(NumberingFormat.DecimalEnclosedCircle, RtfExportSR.EndNoteNumberingFormatDecimalEnclosedCircle);
			result.Add(NumberingFormat.DecimalFullWidth, RtfExportSR.EndNoteNumberingFormatDecimalFullWidth);
			result.Add(NumberingFormat.Ganada, RtfExportSR.EndNoteNumberingFormatGanada);
			return result;
		}
		#endregion
		#region CreatePredefinedUserGroups
		static Dictionary<int, string> CreatePredefinedUserGroups() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			result.Add(0xFFFA, "Current User");
			result.Add(0xFFFB, "Editors");
			result.Add(0xFFFC, "Owners");
			result.Add(0xFFFD, "Contributors");
			result.Add(0xFFFE, "Administrators");
			result.Add(0xFFFF, "Everyone");
			return result;
		}
		#endregion
		#endregion
		public RtfContentExporter(DocumentModel documentModel, RtfDocumentExporterOptions options, IRtfExportHelper rtfExportHelper)
			: base(documentModel) {
			Guard.ArgumentNotNull(rtfExportHelper, "rtfExportHelper");
			Guard.ArgumentNotNull(options, "options");
			this.rtfExportHelper = rtfExportHelper;
			this.options = options;
			this.rtfBuilder = CreateRtfBuilder();
			this.paragraphPropertiesExporter = CreateParagraphPropertiesExporter();
			this.characterPropertiesExporter = new RtfCharacterPropertiesExporter(documentModel, rtfExportHelper, rtfBuilder, options);
			this.getMergedCharacterPropertiesCachedResult = new RunMergedCharacterPropertiesCachedResult();
		}
		#region Properties
		internal static Dictionary<int, string> PredefinedUserGroups { get { return predefinedUserGroups; } }
		protected internal IRtfExportHelper RtfExportHelper { get { return rtfExportHelper; } }
		public RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		protected internal RtfParagraphPropertiesExporter ParagraphPropertiesExporter { get { return paragraphPropertiesExporter; } }
		protected internal RtfCharacterPropertiesExporter CharacterPropertiesExporter { get { return characterPropertiesExporter; } }
		public static Dictionary<BorderLineStyle, string> BorderLineStyles { get { return borderLineStyles; } }
		public static Dictionary<ConditionalTableStyleFormattingTypes, string> ConditionalStylesTypes { get { return conditionalStylesTypes; } }
		protected internal override bool ShouldExportHiddenText { get { return true; } }
		public RtfDocumentExporterOptions Options { get { return options; } }
		internal DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		public bool LastParagraphRunNotSelected { get { return lastParagraphRunNotSelected; } set { lastParagraphRunNotSelected = value; } }
		public bool KeepFieldCodeViewState { get { return keepFieldCodeViewState; } set { keepFieldCodeViewState = value; } }
		#endregion
		protected internal virtual RtfBuilder CreateRtfBuilder() {
			Encoding encoding = options.ActualEncoding;
			bool isSingleByte =
#if !SL
				encoding.IsSingleByte;
#else
				encoding.GetMaxByteCount(1) == 1;
#endif
			if (isSingleByte)
				return new RtfBuilder(encoding);
			else
				return new DBCSRtfBuilder(encoding);
		}
		protected virtual RtfParagraphPropertiesExporter CreateParagraphPropertiesExporter() {
			return new RtfParagraphPropertiesExporter(DocumentModel, RtfExportHelper, RtfBuilder);
		}
		public override void Export() {
			ExportDefaultCharacterProperties();
			ExportDefaultParagraphProperties();
			ExportStyleTable();
			if (options.ListExportFormat == RtfNumberingListExportFormat.RtfFormat)
				ExportNumberingListTable();
			PopulateUserTable();
			rtfBuilder.Clear();
			ExportDocumentProperties();
			if(options.WrapContentInGroup)
				rtfBuilder.OpenGroup();
			base.Export();
			if(options.WrapContentInGroup)
				rtfBuilder.CloseGroup();
		}
		protected virtual void ExportDefaultCharacterProperties() {
			if (!String.IsNullOrEmpty(RtfExportHelper.DefaultCharacterProperties))
				return;
			RtfBuilder.Clear();
			RtfCharacterPropertiesExporter characterPropertiesExporter = new RtfCharacterPropertiesExporter(DocumentModel, RtfExportHelper, RtfBuilder, options);
			CharacterFormattingBase characterFormatting = DocumentModel.DefaultCharacterProperties.Info;
			if(characterFormatting.ForeColor != DXColor.Transparent && characterFormatting.ForeColor != DXColor.Empty && characterFormatting.ForeColor != DXColor.Black)
				RtfExportHelper.GetColorIndex(DXColor.Black);
			EnsureValidFirstColor(characterFormatting.ForeColor);
			characterPropertiesExporter.ExportCharacterProperties(new MergedCharacterProperties(characterFormatting.Info, characterFormatting.Options), true, false, false);
			RtfExportHelper.DefaultCharacterProperties = RtfBuilder.RtfContent.ToString();
		}
		void EnsureValidFirstColor(Color color) {
		}
		protected virtual void ExportDefaultParagraphProperties() {
			if (!String.IsNullOrEmpty(RtfExportHelper.DefaultParagraphProperties))
				return;
			RtfBuilder.Clear();
			RtfParagraphPropertiesExporter paragraphPropertiesExporter = new RtfParagraphPropertiesExporter(DocumentModel, RtfExportHelper, RtfBuilder);
			ParagraphFormattingBase paragraphFormatting = DocumentModel.DefaultParagraphProperties.Info;
			paragraphPropertiesExporter.ExportParagraphPropertiesCore(new MergedParagraphProperties(paragraphFormatting.Info, paragraphFormatting.Options), true);
			RtfExportHelper.DefaultParagraphProperties = RtfBuilder.RtfContent.ToString();
		}
		void ExportNumberingListTable() {
			RtfNumberingListExporter listExporter = CreateNumberingListExporter(this);
			NumberingListCollection numberingLists = DocumentModel.NumberingLists;
			NumberingListIndex startIndex = CalculateFirstExportedNumberingListsIndex(numberingLists);
			startIndex = Algorithms.Min(startIndex, CalculateFirstExportedNumberingListsIndexForStyles(DocumentModel.ParagraphStyles));
			startIndex = Algorithms.Min(startIndex, CalculateFirstExportedNumberingListsIndexForNumberingListStyles(DocumentModel.NumberingListStyles));
			int count = new NumberingListIndex(numberingLists.Count) - startIndex;
			listExporter.Export(numberingLists, startIndex, count);
		}
		NumberingListIndex CalculateFirstExportedNumberingListsIndexForNumberingListStyles(NumberingListStyleCollection numberingListStyleCollection) {
			NumberingListIndex result = NumberingListIndex.MaxValue;
			foreach (NumberingListStyle style in numberingListStyleCollection) {
				NumberingListIndex styleListIndex = style.NumberingListIndex;
				if (styleListIndex >= NumberingListIndex.MinValue)
					result = Algorithms.Min(result, styleListIndex);
			}
			return result;
		}
		NumberingListIndex CalculateFirstExportedNumberingListsIndexForStyles(ParagraphStyleCollection paragraphStyleCollection) {
			NumberingListIndex result = NumberingListIndex.MaxValue;
			foreach (ParagraphStyle style in paragraphStyleCollection) {
				NumberingListIndex styleListIndex = style.GetOwnNumberingListIndex();
				if (styleListIndex >= NumberingListIndex.MinValue)
					result = Algorithms.Min(result, styleListIndex);
			}
			return result;
		}
		NumberingListIndex CalculateFirstExportedNumberingListsIndex(NumberingListCollection numberingLists) {
			NumberingListIndex firstIndex = new NumberingListIndex(0);
			NumberingListIndex lastIndex = new NumberingListIndex(numberingLists.Count - 1);
			while (firstIndex <= lastIndex) {
				if (!numberingLists[firstIndex].CanRemove())
					break;
				firstIndex++;
			}
			return firstIndex;
		}
		protected internal virtual RtfNumberingListExporter CreateNumberingListExporter(RtfContentExporter exporter) {
			return new RtfNumberingListExporter(exporter);
		}
		protected internal virtual void ExportStyleTable() {
			DocumentModel documentModel = PieceTable.DocumentModel;
			RtfStyleExporter styleExporter = CreateStyleExporter();
			styleExporter.ExportStyleSheet(documentModel.ParagraphStyles, documentModel.CharacterStyles, documentModel.TableStyles);
		}
		protected virtual RtfStyleExporter CreateStyleExporter(){
			return new RtfStyleExporter(PieceTable.DocumentModel, CreateRtfBuilder(), RtfExportHelper, Options);
		}
		protected internal virtual void PopulateUserTable() {
			RtfExportHelper helper = RtfExportHelper as RtfExportHelper;
			if (helper == null)
				return;
			List<string> users = helper.UserCollection;
			users.Clear();
			List<PieceTable> pieceTables = DocumentModel.GetPieceTables(false);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++)
				PopulateUserList(pieceTables[i], users);
		}
		protected internal virtual void PopulateUserList(PieceTable pieceTable, List<string> users) {
			RangePermissionCollection rangePermissions = pieceTable.RangePermissions;
			int count = rangePermissions.Count;
			for (int i = 0; i < count; i++) {
				string userName = rangePermissions[i].UserName;
				if (!String.IsNullOrEmpty(userName) && !users.Contains(userName))
					users.Add(userName);
			}
		}
		protected internal virtual void ExportDocumentProperties() {
			ExportDocumentInformation();
			DocumentInfo defaultDocumentInfo = DocumentModel.Cache.DocumentInfoCache.DefaultItem;
			DocumentProperties documentProperties = DocumentModel.DocumentProperties;
			ExportFormattingFlags();			
			if (documentProperties.DefaultTabWidth != defaultDocumentInfo.DefaultTabWidth)
				RtfBuilder.WriteCommand(RtfExportSR.DefaultTabWidth, UnitConverter.ModelUnitsToTwips(documentProperties.DefaultTabWidth));
			if (documentProperties.HyphenateDocument != DocumentInfo.HyphenateDocumentDefaultValue)
				RtfBuilder.WriteCommand(RtfExportSR.HyphenateDocument, documentProperties.HyphenateDocument ? "1" : "0");
			if (documentProperties.DifferentOddAndEvenPages != defaultDocumentInfo.DifferentOddAndEvenPages)
				RtfBuilder.WriteCommand(RtfExportSR.PageFacing);
			if (documentProperties.DisplayBackgroundShape)
				RtfBuilder.WriteCommand(RtfExportSR.DisplayBackgroundShape, "1");
			if (!DXColor.IsTransparentOrEmpty(documentProperties.PageBackColor))
				ExportPageBackground(documentProperties);
			ExportDocumentProtectionProperties();
			ExportDocumentLevelFootNotePresenceProperties();
			ExportDocumentLevelFootNoteProperties();
			ExportDocumentLevelEndNoteProperties();
		}
		protected virtual void ExportFormattingFlags() {
			RtfBuilder.WriteCommand(RtfExportSR.NoUICompatible);
			RtfBuilder.WriteCommand(RtfExportSR.ShapeDoNotLay);
			RtfBuilder.WriteCommand(RtfExportSR.HtmlAutoSpacing);
		}
		protected internal virtual void ExportDocumentProtectionProperties() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			if (!properties.EnforceProtection)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.EnforceProtection, 1);
			if (properties.ProtectionType == DocumentProtectionType.ReadOnly) {
				RtfBuilder.WriteCommand(RtfExportSR.AnnotationProtection);
				RtfBuilder.WriteCommand(RtfExportSR.ReadOnlyProtection);
				RtfBuilder.WriteCommand(RtfExportSR.ProtectionLevel, 3);
			}
		}
		protected internal virtual void ExportPageBackground(DocumentProperties documentProperties) {
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.PageBackground);
				RtfBuilder.OpenGroup();
				try {
					RtfBuilder.WriteCommand(RtfExportSR.Shape);
					RtfBuilder.OpenGroup();
					try {
						RtfBuilder.WriteCommand(RtfExportSR.ShapeInstance);
						RtfBuilder.WriteShapeColorProperty("fillColor", documentProperties.PageBackColor);
					}
					finally {
						RtfBuilder.CloseGroup();
					}
				}
				finally {
					RtfBuilder.CloseGroup();
				}
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal virtual void ExportDocumentInformation() {
			if (!ShouldExportDocumentInformation())
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.DocumentInformation);
			ExportDocumentProtectionPasswordHash();
			RtfBuilder.CloseGroup();
		}
		protected internal virtual bool ShouldExportDocumentInformation() {
			return ShouldExportDocumentProtectionPasswordHash();
		}
		protected internal virtual void ExportDocumentProtectionPasswordHash() {
			if (!ShouldExportDocumentProtectionPasswordHash())
				return;
			if (ShouldExportDocumentProtectionPasswordHashInWord2007Format())
				ExportDocumentProtectionPasswordHashWord2007();
			else
				ExportDocumentProtectionPasswordHashWord2003();
		}
		protected internal virtual bool ShouldExportDocumentProtectionPasswordHashInWord2007Format() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			return properties.PasswordHash != null && properties.HashAlgorithmType != HashAlgorithmType.None;
		}
		protected internal virtual bool ShouldExportDocumentProtectionPasswordHash() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			if (!properties.EnforceProtection || (properties.PasswordHash == null && properties.Word2003PasswordHash == null))
				return false;
			else
				return true;
		}
		protected internal virtual void ExportDocumentProtectionPasswordHashWord2007() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.PasswordHash);
			byte[] bytes = GetDocumentProtectionPasswordHashWord2007Bytes();
			RtfBuilder.WriteByteArrayAsHex(bytes);
			RtfBuilder.CloseGroup();
		}
		byte[] GetDocumentProtectionPasswordHashWord2007Bytes() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			byte[] passwordHash = properties.PasswordHash;
			byte[] passwordPrefix = properties.PasswordPrefix;
			if (passwordPrefix == null)
				passwordPrefix = new byte[] { };
			using (MemoryStream stream = new MemoryStream()) {
				using (BinaryWriter writer = new BinaryWriter(stream)) {
					writer.Write(1);
					writer.Write(40 + passwordHash.Length + passwordPrefix.Length);
					writer.Write(1);
					writer.Write(0x8000 + (int)properties.HashAlgorithmType);
					writer.Write(properties.HashIterationCount);
					writer.Write(passwordHash.Length);
					writer.Write(passwordPrefix.Length);
					writer.Write(0);
					writer.Write(0);
					writer.Write(0);
					writer.Flush();
					stream.Write(passwordHash, 0, passwordHash.Length);
					stream.Write(passwordPrefix, 0, passwordPrefix.Length);
					stream.Flush();
					byte[] bytes = stream.GetBuffer();
					byte[] result = new byte[(int)stream.Length];
					Array.Copy(bytes, result, result.Length);
					return result;
				}
			}
		}
		protected internal virtual void ExportDocumentProtectionPasswordHashWord2003() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.Password);
			byte[] hash = properties.Word2003PasswordHash;
			string value;
			if (hash == null) {
				value = "00000000";
			}
			else {
				int hashValue = BitConverter.ToInt32(hash, 0);
				value = String.Format("{0:x8}", hashValue);
			}
			RtfBuilder.WriteTextDirect(value);
			RtfBuilder.CloseGroup();
		}
		void ExportDocumentLevelFootNotePresenceProperties() {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			bool footNotesPresent = AreFootNotesPresent();
			bool endNotesPresent = AreEndNotesPresent();
			if (!footNotesPresent && !endNotesPresent)
				return;
			if (endNotesPresent && !footNotesPresent)
				RtfBuilder.WriteCommand(@"\fet1");
			else
				RtfBuilder.WriteCommand(@"\fet2");
		}
		bool AreFootNotesPresent() {
			FootNoteCollection notes = DocumentModel.FootNotes;
			int count = notes.Count;
			for (int i = 0; i < count; i++)
				if (notes[i].IsReferenced)
					return true;
			return false;
		}
		bool AreEndNotesPresent() {
			EndNoteCollection notes = DocumentModel.EndNotes;
			int count = notes.Count;
			for (int i = 0; i < count; i++)
				if (notes[i].IsReferenced)
					return true;
			return false;
		}
		void ExportDocumentLevelFootNoteProperties() {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			SectionFootNote note = DocumentModel.Sections.First.FootNote;
			FootNoteInfo defaultInfo = DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex];
			if (note.Position != defaultInfo.Position) {
				if (note.Position == FootNotePosition.BelowText)
					RtfBuilder.WriteCommand(RtfExportSR.FootNotePlacementBelowText);
				else
					RtfBuilder.WriteCommand(RtfExportSR.FootNotePlacementPageBottom);
			}
			if (note.StartingNumber != defaultInfo.StartingNumber)
				rtfBuilder.WriteCommand(RtfExportSR.FootNoteNumberingStart, note.StartingNumber);
			if (note.NumberingFormat != defaultInfo.NumberingFormat)
				WriteEnumValueCommand(footNoteNumberingTypes, note.NumberingFormat, RtfExportSR.FootNoteNumberingFormatDecimal);
			if (note.NumberingRestartType != defaultInfo.NumberingRestartType) {
				if (note.NumberingRestartType == LineNumberingRestart.Continuous)
					RtfBuilder.WriteCommand(RtfExportSR.FootNoteNumberingRestartContinuous);
				else if (note.NumberingRestartType == LineNumberingRestart.NewSection)
					RtfBuilder.WriteCommand(RtfExportSR.FootNoteNumberingRestartEachSection);
				else
					RtfBuilder.WriteCommand(RtfExportSR.FootNoteNumberingRestartEachPage);
			}
		}
		void ExportDocumentLevelEndNoteProperties() {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			SectionFootNote note = DocumentModel.Sections.First.EndNote;
			FootNoteInfo defaultInfo = DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex];
			if (note.Position != defaultInfo.Position) {
				if (note.Position == FootNotePosition.EndOfSection)
					RtfBuilder.WriteCommand(RtfExportSR.EndNotePlacementEndOfSection);
				else
					RtfBuilder.WriteCommand(RtfExportSR.EndNotePlacementEndOfDocument);
			}
			if (note.StartingNumber != defaultInfo.StartingNumber)
				rtfBuilder.WriteCommand(RtfExportSR.EndNoteNumberingStart, note.StartingNumber);
			if (note.NumberingFormat != defaultInfo.NumberingFormat)
				WriteEnumValueCommand(endNoteNumberingTypes, note.NumberingFormat, RtfExportSR.EndNoteNumberingFormatLowerRoman);
			if (note.NumberingRestartType != defaultInfo.NumberingRestartType) {
				if (note.NumberingRestartType == LineNumberingRestart.Continuous)
					RtfBuilder.WriteCommand(RtfExportSR.EndNoteNumberingRestartContinuous);
				else
					RtfBuilder.WriteCommand(RtfExportSR.EndNoteNumberingRestartEachSection);
			}
		}
		protected internal override void ExportSection(Section section) {
			StartNewSection(section);
			base.ExportSection(section);
		}
		protected internal override void ExportCustomRun(CustomRun run) {
			RtfBuilder.OpenGroup();
			CharacterPropertiesExporter.ExportCharacterProperties(run.GetMergedCharacterProperties(getMergedCharacterPropertiesCachedResult));
			WriteRunCharacterStyle(run);
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.CustomRunData);
			ICustomRunBoxLayoutExporterService service = DocumentModel.GetService<ICustomRunBoxLayoutExporterService>();
			if(service != null) {
				RtfBuilder.WriteText(service.CustomRunObjectToSting(run.CustomRunObject));
			}
			RtfBuilder.CloseGroup();
			RtfBuilder.CloseGroup();
		}
		#region Section Export
		internal void StartNewSection(Section section) {
			if (!DocumentModel.DocumentCapabilities.SectionsAllowed)
				return;
			RtfBuilder.WriteCommand(RtfExportSR.ResetSectionProperties);
			ExportSectionProperties(section);
		}
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionFirstPageHeader);
				base.ExportFirstPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionOddPageHeader);
				base.ExportOddPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionEvenPageHeader);
				base.ExportEvenPageHeader(sectionHeader, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionFirstPageFooter);
				base.ExportFirstPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionOddPageFooter);
				base.ExportOddPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			if (!DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.SectionEvenPageFooter);
				base.ExportEvenPageFooter(sectionFooter, linkedToPrevious);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		internal void ExportLegacyPageProperties(Section section, int sectionCount) {
			PageInfo defaultPage = DocumentModel.Cache.PageInfoCache.DefaultItem;
			PageNumberingInfo defaultPageNumbering = DocumentModel.Cache.PageNumberingInfoCache.DefaultItem;
			MarginsInfo defaultMargins = DocumentModel.Cache.MarginsInfoCache.DefaultItem;
			if (section.Page.Width != defaultPage.Width)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyPaperWidth, UnitConverter.ModelUnitsToTwips(section.Page.Width));
			if (section.Page.Height != defaultPage.Height)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyPaperHeight, UnitConverter.ModelUnitsToTwips(section.Page.Height));
			if (section.Page.PaperKind != defaultPage.PaperKind && section.Page.PaperKind != PaperKind.Custom)
				rtfBuilder.WriteCommand(RtfExportSR.PaperKind, (int)section.Page.PaperKind);
			if (sectionCount <= 1 && section.Page.Landscape)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyLandscape);
			if (section.PageNumbering.StartingPageNumber != defaultPageNumbering.StartingPageNumber)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyPageNumberingStart, section.PageNumbering.StartingPageNumber);
			if (section.Margins.Left != defaultMargins.Left)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsLeft, UnitConverter.ModelUnitsToTwips(section.Margins.Left));
			if (section.Margins.Right != defaultMargins.Right)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsRight, UnitConverter.ModelUnitsToTwips(section.Margins.Right));
			if (section.Margins.Top != defaultMargins.Top)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsTop, UnitConverter.ModelUnitsToTwips(section.Margins.Top));
			if (section.Margins.Bottom != defaultMargins.Bottom)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsBottom, UnitConverter.ModelUnitsToTwips(section.Margins.Bottom));
			if (section.Margins.Gutter != defaultMargins.Gutter)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsGutter, UnitConverter.ModelUnitsToTwips(section.Margins.Gutter));
			if (section.Margins.GutterAlignment == SectionGutterAlignment.Right)
				rtfBuilder.WriteCommand(RtfExportSR.LegacyMarginsGutterAtRight);
		}
		void ExportSectionProperties(Section section) {
			ExportSectionMargins(section.Margins);
			ExportSectionPage(section.Page);
			ExportSectionGeneralSettings(section.GeneralSettings);
			ExportSectionPageNumbering(section.PageNumbering);
			ExportSectionLineNumbering(section.LineNumbering);
			ExportSectionColumns(section.Columns);
			ExportSectionFootNote(section.FootNote);
			ExportSectionEndNote(section.EndNote);
		}
		void ExportSectionMargins(SectionMargins margins) {
			MarginsInfo defaultMargins = this.PieceTable.DocumentModel.Cache.MarginsInfoCache.DefaultItem;
			if (margins.Left != defaultMargins.Left)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsLeft, UnitConverter.ModelUnitsToTwips(margins.Left));
			if (margins.Right != defaultMargins.Right)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsRight, UnitConverter.ModelUnitsToTwips(margins.Right));
			if (margins.Top != defaultMargins.Top)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsTop, UnitConverter.ModelUnitsToTwips(margins.Top));
			if (margins.Bottom != defaultMargins.Bottom)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsBottom, UnitConverter.ModelUnitsToTwips(margins.Bottom));
			if (margins.HeaderOffset != defaultMargins.HeaderOffset)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsHeaderOffset, UnitConverter.ModelUnitsToTwips(margins.HeaderOffset));
			if (margins.FooterOffset != defaultMargins.FooterOffset)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsFooterOffset, UnitConverter.ModelUnitsToTwips(margins.FooterOffset));
			if (margins.Gutter != defaultMargins.Gutter)
				rtfBuilder.WriteCommand(RtfExportSR.SectionMarginsGutter, UnitConverter.ModelUnitsToTwips(margins.Gutter));
		}
		void ExportSectionPage(SectionPage page) {
			PageInfo defaultPage = this.PieceTable.DocumentModel.Cache.PageInfoCache.DefaultItem;
			if (page.Width != defaultPage.Width)
				rtfBuilder.WriteCommand(RtfExportSR.SectionPageWidth, UnitConverter.ModelUnitsToTwips(page.Width));
			if (page.Height != defaultPage.Height)
				rtfBuilder.WriteCommand(RtfExportSR.SectionPageHeight, UnitConverter.ModelUnitsToTwips(page.Height));
			if (page.Landscape)
				rtfBuilder.WriteCommand(RtfExportSR.SectionPageLandscape);
			if (page.PaperKind != defaultPage.PaperKind && page.PaperKind != PaperKind.Custom)
				rtfBuilder.WriteCommand(RtfExportSR.PaperKind, (int)page.PaperKind);
		}
		void ExportSectionGeneralSettings(SectionGeneralSettings settings) {
			GeneralSectionInfo defaultSettings = this.PieceTable.DocumentModel.Cache.GeneralSectionInfoCache.DefaultItem;
			if (settings.StartType != defaultSettings.StartType)
				WriteSectionBreakType(settings.StartType);
			if (settings.FirstPagePaperSource != defaultSettings.FirstPagePaperSource)
				rtfBuilder.WriteCommand(RtfExportSR.SectionFirstPagePaperSource, settings.FirstPagePaperSource);
			if (settings.OtherPagePaperSource != defaultSettings.OtherPagePaperSource)
				rtfBuilder.WriteCommand(RtfExportSR.SectionOtherPagePaperSource, settings.OtherPagePaperSource);
			if (settings.OnlyAllowEditingOfFormFields)
				rtfBuilder.WriteCommand(RtfExportSR.SectionOnlyAllowEditingOfFormFields);
			if (settings.TextDirection != defaultSettings.TextDirection)
				rtfBuilder.WriteCommand(RtfExportSR.SectionTextFlow, (int)settings.TextDirection);
			if (settings.VerticalTextAlignment != defaultSettings.VerticalTextAlignment)
				WriteVerticalAlignment(settings.VerticalTextAlignment);
			if (settings.DifferentFirstPage != defaultSettings.DifferentFirstPage)
				rtfBuilder.WriteCommand(RtfExportSR.SectionTitlePage);
		}
		void ExportSectionPageNumbering(SectionPageNumbering pageNumbering) {
			PageNumberingInfo defaultPageNumbering = this.PieceTable.DocumentModel.Cache.PageNumberingInfoCache.DefaultItem;
			if (pageNumbering.ChapterSeparator != defaultPageNumbering.ChapterSeparator)
				WriteChapterSeparator(pageNumbering.ChapterSeparator);
			if (pageNumbering.ChapterHeaderStyle != defaultPageNumbering.ChapterHeaderStyle)
				rtfBuilder.WriteCommand(RtfExportSR.SectionChapterHeaderStyle, pageNumbering.ChapterHeaderStyle);
			if (pageNumbering.StartingPageNumber != defaultPageNumbering.StartingPageNumber) {
				rtfBuilder.WriteCommand(RtfExportSR.SectionPageNumberingStart, pageNumbering.StartingPageNumber);
				rtfBuilder.WriteCommand(RtfExportSR.SectionPageNumberingRestart);
			}
			if (pageNumbering.NumberingFormat != defaultPageNumbering.NumberingFormat)
				WritePageNumberingFormat(pageNumbering.NumberingFormat);
		}
		void ExportSectionLineNumbering(SectionLineNumbering lineNumbering) {
			LineNumberingInfo defaultLineNumbering = this.PieceTable.DocumentModel.Cache.LineNumberingInfoCache.DefaultItem;
			if (lineNumbering.Distance != defaultLineNumbering.Distance)
				rtfBuilder.WriteCommand(RtfExportSR.SectionLineNumberingDistance, UnitConverter.ModelUnitsToTwips(lineNumbering.Distance));
			if (lineNumbering.Step != defaultLineNumbering.Step)
				rtfBuilder.WriteCommand(RtfExportSR.SectionLineNumberingStep, lineNumbering.Step);
			if (lineNumbering.NumberingRestartType != LineNumberingRestart.NewPage) {
				rtfBuilder.WriteCommand(RtfExportSR.SectionLineNumberingStartingLineNumber, lineNumbering.StartingLineNumber);
				if (lineNumbering.NumberingRestartType == LineNumberingRestart.Continuous)
					rtfBuilder.WriteCommand(RtfExportSR.SectionLineNumberingContinuous);
				else
					rtfBuilder.WriteCommand(RtfExportSR.SectionLineNumberingRestartNewSection);
			}
		}
		void ExportSectionColumns(SectionColumns columns) {
			ColumnsInfo defaultColumns = this.PieceTable.DocumentModel.Cache.ColumnsInfoCache.DefaultItem;
			if (columns.EqualWidthColumns) {
				if (columns.ColumnCount != defaultColumns.ColumnCount)
					rtfBuilder.WriteCommand(RtfExportSR.SectionColumnsCount, columns.ColumnCount);
				if (columns.Space != defaultColumns.Space)
					rtfBuilder.WriteCommand(RtfExportSR.SectionSpaceBetweenColumns, UnitConverter.ModelUnitsToTwips(columns.Space));
			}
			else
				ExportSectionColumnsDetails(columns.GetColumns());
			if (columns.DrawVerticalSeparator)
				rtfBuilder.WriteCommand(RtfExportSR.SectionColumnsDrawVerticalSeparator);
		}
		void ExportSectionColumnsDetails(ColumnInfoCollection columns) {
			int count = columns.Count;
			if (count > 0)
				rtfBuilder.WriteCommand(RtfExportSR.SectionColumnsCount, count);
			for (int i = 0; i < count; i++)
				ExportSectionColumn(columns[i], i);
		}
		void ExportSectionColumn(ColumnInfo column, int columnIndex) {
			rtfBuilder.WriteCommand(RtfExportSR.SectionColumnNumber, columnIndex + 1);
			rtfBuilder.WriteCommand(RtfExportSR.SectionColumnWidth, UnitConverter.ModelUnitsToTwips(column.Width));
			rtfBuilder.WriteCommand(RtfExportSR.SectionColumnSpace, UnitConverter.ModelUnitsToTwips(column.Space));
		}
		void ExportSectionFootNote(SectionFootNote note) {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return;
			FootNoteInfo defaultInfo = DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex];
			if (note.Position != defaultInfo.Position) {
				if (note.Position == FootNotePosition.BelowText)
					RtfBuilder.WriteCommand(RtfExportSR.SectionFootNotePlacementBelowText);
				else
					RtfBuilder.WriteCommand(RtfExportSR.SectionFootNotePlacementPageBottom);
			}
			if (note.StartingNumber != defaultInfo.StartingNumber)
				rtfBuilder.WriteCommand(RtfExportSR.SectionFootNoteNumberingStart, note.StartingNumber);
			if (note.NumberingFormat != defaultInfo.NumberingFormat)
				WriteEnumValueCommand(sectionFootNoteNumberingTypes, note.NumberingFormat, RtfExportSR.SectionFootNoteNumberingFormatDecimal);
			if (note.NumberingRestartType != defaultInfo.NumberingRestartType) {
				if (note.NumberingRestartType == LineNumberingRestart.Continuous)
					RtfBuilder.WriteCommand(RtfExportSR.SectionFootNoteNumberingRestartContinuous);
				else if (note.NumberingRestartType == LineNumberingRestart.NewSection)
					RtfBuilder.WriteCommand(RtfExportSR.SectionFootNoteNumberingRestartEachSection);
				else
					RtfBuilder.WriteCommand(RtfExportSR.SectionFootNoteNumberingRestartEachPage);
			}
		}
		void ExportSectionEndNote(SectionFootNote note) {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return;
			FootNoteInfo defaultInfo = DocumentModel.Cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex];
			if (note.StartingNumber != defaultInfo.StartingNumber)
				rtfBuilder.WriteCommand(RtfExportSR.SectionEndNoteNumberingStart, note.StartingNumber);
			if (note.NumberingFormat != defaultInfo.NumberingFormat)
				WriteEnumValueCommand(sectionEndNoteNumberingTypes, note.NumberingFormat, RtfExportSR.SectionEndNoteNumberingFormatLowerRoman);
			if (note.NumberingRestartType != defaultInfo.NumberingRestartType) {
				if (note.NumberingRestartType == LineNumberingRestart.Continuous)
					RtfBuilder.WriteCommand(RtfExportSR.SectionEndNoteNumberingRestartContinuous);
				else
					RtfBuilder.WriteCommand(RtfExportSR.SectionEndNoteNumberingRestartEachSection);
			}
		}
		void WriteVerticalAlignment(VerticalAlignment alignment) {
			WriteEnumValueCommand(verticalAlignmentTypes, alignment, RtfExportSR.VerticalAlignmentBottom);
		}
		void WriteSectionBreakType(SectionStartType breakType) {
			WriteEnumValueCommand(sectionStartTypes, breakType, RtfExportSR.SectionBreakTypeNextPage);
		}
		void WriteChapterSeparator(char separator) {
			WriteEnumValueCommand(chapterSeparatorTypes, separator, RtfExportSR.SectionChapterSeparatorHyphen);
		}
		void WritePageNumberingFormat(NumberingFormat numberingFormat) {
			WriteEnumValueCommand(pageNumberingTypes, numberingFormat, RtfExportSR.SectionPageNumberingDecimal);
		}
		void WriteEnumValueCommand<T>(Dictionary<T, string> table, T value, string defaultCommand) {
			string command;
			if (!table.TryGetValue(value, out command))
				command = defaultCommand;
			rtfBuilder.WriteCommand(command);
		}
		#endregion
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			bool tablesAllowed = DocumentModel.DocumentCapabilities.TablesAllowed;
			TableCell paragraphCell = (tablesAllowed) ? paragraph.GetCell() : null ;
			if (paragraphCell == null) {
				ExportSingleParagraph(paragraph);
				return paragraph.Index;
			}
			else 
				return ExportRtfTable(paragraphCell.Row.Table); 
		}
		protected internal override bool ShouldUseCustomSaveTableMethod() {
			return true;
		}
		protected virtual ParagraphIndex ExportRtfTable(Table table) {
			RtfTableExporter exporter = new RtfTableExporter(this);
			return exporter.Export(table);
		}
		void ExportSingleParagraph(Paragraph paragraph) {
			ExportParagraphCore(paragraph, 0, 0, -1);
			if (!(IsLastParagraph(paragraph) && SuppressExportLastParagraph(paragraph)))
				FinishParagraph(paragraph);
		}
		protected virtual bool SuppressExportLastParagraph(Paragraph paragraph) {
			if (!paragraph.PieceTable.IsMain)
				return false;
			return Options.ExportFinalParagraphMark == ExportFinalParagraphMark.Never ||
				(Options.ExportFinalParagraphMark == ExportFinalParagraphMark.SelectedOnly && lastParagraphRunNotSelected);
		}
		internal void ExportParagraphCore(Paragraph paragraph, int tableNestingLevel, ConditionalTableStyleFormattingTypes condTypes, int tableStyleIndex) {
			StartNewParagraph(paragraph, tableNestingLevel);
			ExportParagraphTableStyleProperties(condTypes, tableStyleIndex);
			ExportParagraphRuns(paragraph);
			ExportParagraphCharacterProperties(paragraph);
		}
		bool IsLastParagraph(Paragraph paragraph) {
			return PieceTable.Paragraphs.Last == paragraph;
		}
		void FinishParagraph(Paragraph paragraph) {
			if (PieceTable.Runs[paragraph.LastRunIndex] is SectionRun && 
				DocumentModel.DocumentCapabilities.SectionsAllowed) {
				rtfBuilder.WriteCommand(RtfExportSR.SectionEndMark);
			}
			else {
				rtfBuilder.WriteCommand(RtfExportSR.EndOfParagraph);
			}
		}
		void ExportParagraphCharacterProperties(Paragraph paragraph) {
			TextRunBase run = PieceTable.Runs[paragraph.LastRunIndex];
			CharacterPropertiesExporter.ExportParagraphCharacterProperties(run.GetMergedCharacterProperties(getMergedCharacterPropertiesCachedResult));
		}
		protected internal virtual void StartNewParagraph(Paragraph paragraph, int tableNestingLevel) {
			if (paragraph.IsInList())
				WriteAlternativeAndPlainText(paragraph);
			rtfBuilder.WriteCommand(RtfExportSR.ResetParagraphProperties);
			StartNewInnerParagraph(paragraph, tableNestingLevel);
		}
		protected internal virtual void ExportParagraphTableStyleProperties(ConditionalTableStyleFormattingTypes condTypes, int tableStyleIndex) {
			if (tableStyleIndex < 0)
				return;
			Array condTypesArray = Enum.GetValues(typeof(ConditionalTableStyleFormattingTypes));
			string keyword;
			foreach (ConditionalTableStyleFormattingTypes condType in condTypesArray)
				if ((condTypes & condType) > 0 && conditionalStylesTypes.TryGetValue(condType, out keyword))
					rtfBuilder.WriteCommand(keyword);
			rtfBuilder.WriteCommand(RtfExportSR.TableStyleCellIndex, tableStyleIndex);
		}
		protected void WriteAlternativeAndPlainText(Paragraph paragraph) {
			WriteAlternativeText(paragraph);
		}
		protected void WriteAlternativeText(Paragraph paragraph) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.AlternativeText);
			rtfBuilder.WriteCommand(RtfExportSR.ResetParagraphProperties);
			WriteText(GetNumberingListText(paragraph));
			string separator = paragraph.GetListLevelSeparator();
			if(!String.IsNullOrEmpty(separator))
				WriteText(separator);
			rtfBuilder.CloseGroup();
		}
		protected void StartNewInnerParagraph(Paragraph paragraph, int tableNestingLevel) {
			rtfBuilder.WriteCommand(RtfExportSR.ResetCharacterFormatting);
			ParagraphPropertiesExporter.ExportParagraphProperties(paragraph, tableNestingLevel);
		}
		protected internal void WriteParagraphStyle(RtfBuilder rtfBuilder, ParagraphStyle paragraphStyle) {
			Dictionary<string, int> styleCollection = RtfExportHelper.ParagraphStylesCollectionIndex;
			string styleName = paragraphStyle.StyleName;
			if (styleCollection.ContainsKey(styleName))
				rtfBuilder.WriteCommand(RtfExportSR.ParagraphStyle, styleCollection[styleName]);
		}
		protected internal void WriteCharacterStyle(CharacterStyle characterStyle) {
			Dictionary<string, int> styleCollection = RtfExportHelper.CharacterStylesCollectionIndex;
			string styleName = characterStyle.StyleName;
			if (styleCollection.ContainsKey(styleName))
				RtfBuilder.WriteCommand(RtfExportSR.CharacterStyleIndex, styleCollection[styleName]);
		}
		void WriteText(string text) {
			rtfBuilder.WriteText(text);
		}
		protected internal override void ExportTextRun(TextRun run) {
			RtfBuilder.OpenGroup();
			CharacterPropertiesExporter.ExportCharacterProperties(run.GetMergedCharacterProperties(getMergedCharacterPropertiesCachedResult));
			WriteRunCharacterStyle(run);
			string text = run.GetPlainText(PieceTable.TextBuffer);
			WriteText(text);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void WriteRunCharacterStyle(TextRunBase run) {
			if (ShouldWriteRunCharacterStyle(run))
				WriteCharacterStyle(run.CharacterStyle);
		}
		protected internal virtual bool ShouldWriteRunCharacterStyle(TextRunBase run) {
			if (run.CharacterStyleIndex == CharacterStyleCollection.EmptyCharacterStyleIndex)
				return false;
			if (!RtfExportHelper.SupportStyle)
				return false;
			if (run.Paragraph.ParagraphStyleIndex == ParagraphStyleCollection.EmptyParagraphStyleIndex)
				return true;
			ParagraphStyle paragraphStyle = run.Paragraph.ParagraphStyle;
			if (paragraphStyle.HasLinkedStyle && paragraphStyle.LinkedStyle == run.CharacterStyle)
				return false;
			else
				return true;
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			WriteFieldInstructionStart(run);
		}
		void WriteFieldInstructionStart(FieldCodeStartRun run) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.Field);
			Field field = PieceTable.FindFieldByRunIndex(run.GetRunIndex());
			if (field.Locked)
				RtfBuilder.WriteCommand(RtfExportSR.FieldLocked);
			if (!DocumentModel.FieldOptions.UpdateFieldsOnPaste && KeepFieldCodeViewState) {				
				if (field.IsCodeView)
					RtfBuilder.WriteCommand(RtfExportSR.FieldCodeView);
			}
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.FieldInstructions);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			WriteFieldInstructionEnd();
			WriteFieldResultStart();
		}
		void WriteFieldInstructionEnd() {
			RtfBuilder.CloseGroup();
			RtfBuilder.OpenGroup();
		}
		void WriteFieldResultStart() {
			RtfBuilder.WriteCommand(RtfExportSR.FieldResult);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			WriteFieldResultEnd();
		}
		void WriteFieldResultEnd() {
			RtfBuilder.CloseGroup();
			RtfBuilder.CloseGroup();
		}
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			MergedCharacterProperties characterProperties = run.GetMergedCharacterProperties(getMergedCharacterPropertiesCachedResult);
			bool shouldExportCharacterProperties = InlinePictureRun.ShouldExportInlinePictureRunCharacterProperties(characterProperties.Info, characterProperties.Options);
			if (shouldExportCharacterProperties) {
				RtfBuilder.OpenGroup();
				CharacterPropertiesExporter.ExportCharacterProperties(characterProperties);
			}
			RtfInlinePictureExportStrategy exportStrategy = new RtfInlinePictureExportStrategy();
			exportStrategy.Export(rtfBuilder, run, options.Compatibility.DuplicateObjectAsMetafile);
			if(shouldExportCharacterProperties)
				RtfBuilder.CloseGroup();
		}
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.BookmarkStart);
			RtfBuilder.WriteText(bookmark.Name);
			RtfBuilder.CloseGroup();
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.BookmarkEnd);
			RtfBuilder.WriteText(bookmark.Name);
			RtfBuilder.CloseGroup();
		}
		protected internal override void ExportRangePermissionStart(RangePermission rangePermission) {
			string data = GenerateRangePermissionData(rangePermission);
			if (String.IsNullOrEmpty(data))
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.RangePermissionStart);
			RtfBuilder.WriteTextDirect(data);
			RtfBuilder.CloseGroup();
		}
		protected internal override void ExportRangePermissionEnd(RangePermission rangePermission) {
			string data = GenerateRangePermissionData(rangePermission);
			if (String.IsNullOrEmpty(data))
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.RangePermissionEnd);
			RtfBuilder.WriteTextDirect(data);
			RtfBuilder.CloseGroup();
		}
		protected internal string GenerateRangePermissionData(RangePermission rangePermission) {
			RtfExportHelper helper = RtfExportHelper as RtfExportHelper;
			if (helper == null)
				return String.Empty;
			int userIndex = helper.GetUserIndex(rangePermission);
			if (userIndex == 0)
				return String.Empty;
			int rangeIndex = PieceTable.RangePermissions.IndexOf(rangePermission);
			if (rangeIndex < 0)
				return String.Empty;
			return IntToShortString(userIndex) + "0100" + IntToShortString(rangeIndex) + "0000";
		}
		protected internal string IntToShortString(int value) {
			value &= 0x0000FFFF;
			int low = value & 0x000000FF;
			int high = value >> 8;
			return String.Format("{0:X2}{1:X2}", low, high);
		}
		protected internal override void ExportCommentStart(Comment comment) {
			if (comment.Start != comment.End) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentStart);
				RtfBuilder.WriteText(Convert.ToString(comment.Index));
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportCommentEnd(Comment comment) {
			if (comment.Start != comment.End) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentEnd);
				RtfBuilder.WriteText(Convert.ToString(comment.Index));
				RtfBuilder.CloseGroup();
			}
			ExportCommentId(comment);
			ExportCommentAuthor(comment);
			RtfBuilder.WriteCommand(RtfExportSR.CommentChatn);
			ExportCommentAnnotation(comment);
		}
		void ExportCommentId(Comment comment) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.CommentId);
			RtfBuilder.WriteText(comment.Name);
			RtfBuilder.CloseGroup();
		}
		void ExportCommentAuthor(Comment comment) {
			if (!String.IsNullOrEmpty(comment.Author)) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentAuthor);
				RtfBuilder.WriteText(comment.Author);
				RtfBuilder.CloseGroup();
			}
		}
		void ExportCommentAnnotation(Comment comment) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.CommentAnnotation);
			ExportCommentRef(comment);
			ExportCommentDate(comment);
			ExportCommentParent(comment);
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.CommentChatn);
			RtfBuilder.CloseGroup();
			ExportCommentContent(comment);
			RtfBuilder.CloseGroup();
		}
		void ExportCommentDate(Comment comment) {
			if (comment.Date > Comment.MinCommentDate) {
				string stringDTTM = DateTimeUtils.ToDateTimeDTTM(comment.Date).ToString();
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentDate);
				RtfBuilder.WriteText(stringDTTM);
				RtfBuilder.CloseGroup();
			}
		}
		void ExportCommentRef(Comment comment) {
			if (comment.Start != comment.End && !String.IsNullOrEmpty(comment.Name)) { 
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentRef);
				RtfBuilder.WriteText(Convert.ToString(comment.Index));
				RtfBuilder.CloseGroup();
			}
		}
		void ExportCommentParent(Comment comment) {
			if (comment.ParentComment!= null) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.CommentParent);
				RtfBuilder.WriteText(CalculateCommentParentValue(comment));
				RtfBuilder.CloseGroup();
			}
		}
		String CalculateCommentParentValue(Comment comment) {
			return Convert.ToString(comment.ParentComment.Index - comment.Index);
		}
		void ExportCommentContent(Comment comment) {
			if (!comment.Content.PieceTable.IsEmpty) {
				RtfBuilder.OpenGroup();
				ExportFinalParagraphMark oldExportFinalParagraphMark = Options.ExportFinalParagraphMark;
				Options.ExportFinalParagraphMark = ExportFinalParagraphMark.Never;
				PerformExportPieceTable(comment.Content.PieceTable, ExportPieceTable);
				Options.ExportFinalParagraphMark = oldExportFinalParagraphMark;
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			ExportFootNoteRunCore(run, String.Empty);
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			ExportFootNoteRunCore(run, RtfExportSR.EndNote);
		}
		protected internal virtual void ExportFootNoteRunCore<T>(FootNoteRunBase<T> run, string suffixKeyword) where T : FootNoteBase<T>{
			RtfBuilder.OpenGroup();
			try {
				CharacterPropertiesExporter.ExportCharacterProperties(run.GetMergedCharacterProperties(getMergedCharacterPropertiesCachedResult));
				WriteRunCharacterStyle(run);
				RtfBuilder.WriteCommand(RtfExportSR.FootNoteReference);
				if (!run.Paragraph.PieceTable.IsMain)
					return;
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.FootNote);
				if (!String.IsNullOrEmpty(suffixKeyword))
					RtfBuilder.WriteCommand(suffixKeyword);
				ExportFinalParagraphMark oldExportFinalParagraphMark = Options.ExportFinalParagraphMark;
				Options.ExportFinalParagraphMark = ExportFinalParagraphMark.Never;
				try {
					PerformExportPieceTable(run.Note.PieceTable, ExportPieceTable);
				}
				finally {
					Options.ExportFinalParagraphMark = oldExportFinalParagraphMark;
				}
				RtfBuilder.CloseGroup();
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.Shape);
				RtfBuilder.OpenGroup(); 
				try {
					RtfBuilder.WriteCommand(RtfExportSR.ShapeInstance);
					ExportShapeInstanceProperties(floatingObjectProperties, run.Shape.UseRotation ? run.Shape.Rotation : 0);
					ExportFloatingObjectShape(run.Shape);
					ExportFloatingObjectHorizontalPositionAlignment(floatingObjectProperties);
					ExportFloatingObjectHorizontalPositionTypeProperty(floatingObjectProperties);
					ExportFloatingObjectVerticalPositionAlignment(floatingObjectProperties);
					ExportFloatingObjectVerticalPositionTypeProperty(floatingObjectProperties);
					ExportFloatingObjectLayoutInTableCell(floatingObjectProperties);
					ExportFloatingObjectAllowOverlap(floatingObjectProperties);
					ExportFloatingObjectLockAspectRatio(floatingObjectProperties);
					ExportFloatingObjectHidden(floatingObjectProperties);
					ExportFloatingObjectBehindDocument(floatingObjectProperties);
					ExportFloatingObjectLeftDistance(floatingObjectProperties);
					ExportFloatingObjectRightDistance(floatingObjectProperties);
					ExportFloatingObjectTopDistance(floatingObjectProperties);
					ExportFloatingObjectBottomDistance(floatingObjectProperties);
					ExportFloatingObjectRelativeSize(floatingObjectProperties);
					if (!String.IsNullOrEmpty(run.Name))
						RtfBuilder.WriteShapeProperty("wzName", run.Name);
					PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
					if (pictureContent != null)
						ExportPicture(run, pictureContent);
					TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
					if (textBoxContent != null)
						ExportTextBoxContent(run, textBoxContent);
				}
				finally {
					RtfBuilder.CloseGroup();
				}
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		void ExportPicture(FloatingObjectAnchorRun run, PictureFloatingObjectContent content) {
			RtfFloatingObjectPictureExportStrategy exportStrategy = new RtfFloatingObjectPictureExportStrategy();
			exportStrategy.Export(rtfBuilder, run, options.Compatibility.DuplicateObjectAsMetafile);
		}
		void ExportTextBoxContent(FloatingObjectAnchorRun run, TextBoxFloatingObjectContent content) {
			ExportTextBoxProperties(content.TextBoxProperties);
			RtfBuilder.OpenGroup();
			try {
				RtfBuilder.WriteCommand(RtfExportSR.ShapeText);
				PerformExportPieceTable(content.TextBox.PieceTable, ExportPieceTable);
			}
			finally {
				RtfBuilder.CloseGroup();
			}
		}
		void ExportTextBoxProperties(TextBoxProperties properties) {
			if (properties.UseLeftMargin)
				RtfBuilder.WriteShapeIntegerProperty("dxTextLeft", UnitConverter.ModelUnitsToEmu(properties.LeftMargin));
			if (properties.UseRightMargin)
				RtfBuilder.WriteShapeIntegerProperty("dxTextRight", UnitConverter.ModelUnitsToEmu(properties.RightMargin));
			if (properties.UseTopMargin)
				RtfBuilder.WriteShapeIntegerProperty("dyTextTop", UnitConverter.ModelUnitsToEmu(properties.TopMargin));
			if (properties.UseBottomMargin)
				RtfBuilder.WriteShapeIntegerProperty("dyTextBottom", UnitConverter.ModelUnitsToEmu(properties.BottomMargin));
			if (properties.UseResizeShapeToFitText)
				RtfBuilder.WriteShapeBoolProperty("fFitShapeToText", properties.ResizeShapeToFitText);
			if (properties.UseWrapText && !properties.WrapText)
				RtfBuilder.WriteShapeIntegerProperty("WrapText", 2); 
		}
		void ExportFloatingObjectShape(Shape shape) {
			if (shape.UseRotation)
				ExportFloatingObjectRotation(shape);
			if (DXColor.IsTransparentOrEmpty(shape.OutlineColor))
				RtfBuilder.WriteShapeBoolProperty("fLine", false);
			if (shape.UseOutlineWidth && shape.OutlineWidth > 0) {
				RtfBuilder.WriteShapeIntegerProperty("lineWidth", UnitConverter.ModelUnitsToEmu(shape.OutlineWidth));
				if (shape.UseOutlineColor && !DXColor.IsTransparentOrEmpty(shape.OutlineColor))
					RtfBuilder.WriteShapeColorProperty("lineColor", shape.OutlineColor);
			}
			if (shape.UseFillColor && !DXColor.IsTransparentOrEmpty(shape.FillColor)) {
				RtfBuilder.WriteShapeBoolProperty("fFilled", true);
				RtfBuilder.WriteShapeColorProperty("fillColor", shape.FillColor);
			}
			else
				RtfBuilder.WriteShapeBoolProperty("fFilled", false);
		}
		void ExportFloatingObjectRightDistance(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseRightDistance)
				RtfBuilder.WriteShapeIntegerProperty("dxWrapDistRight", UnitConverter.ModelUnitsToEmu(floatingObjectProperties.RightDistance));
		}
		void ExportFloatingObjectTopDistance(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseTopDistance)
				RtfBuilder.WriteShapeIntegerProperty("dyWrapDistTop", UnitConverter.ModelUnitsToEmu(floatingObjectProperties.TopDistance));
		}
		void ExportFloatingObjectBottomDistance(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseBottomDistance)
				RtfBuilder.WriteShapeIntegerProperty("dyWrapDistBottom", UnitConverter.ModelUnitsToEmu(floatingObjectProperties.BottomDistance));
		}
		void ExportFloatingObjectLeftDistance(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseLeftDistance)
				RtfBuilder.WriteShapeIntegerProperty("dxWrapDistLeft", UnitConverter.ModelUnitsToEmu(floatingObjectProperties.LeftDistance));
		}
		void ExportFloatingObjectRotation(Shape shape) {
			if (shape.UseRotation)
				RtfBuilder.WriteShapeIntegerProperty("rotation", UnitConverter.ModelUnitsToFD(shape.Rotation));
		}
		void ExportFloatingObjectHidden(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseHidden)
				RtfBuilder.WriteShapeBoolProperty("fHidden", floatingObjectProperties.Hidden);
		}
		void ExportFloatingObjectLockAspectRatio(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseLockAspectRatio)
				RtfBuilder.WriteShapeBoolProperty("fLockAspectRatio", floatingObjectProperties.LockAspectRatio);		
		}
		void ExportFloatingObjectBehindDocument(FloatingObjectProperties floatingObjectProperties) {
			RtfBuilder.WriteShapeBoolProperty("fBehindDocument",  floatingObjectProperties.IsBehindDoc);
		}
		void ExportFloatingObjectAllowOverlap(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseAllowOverlap)
				RtfBuilder.WriteShapeBoolProperty("fAllowOverlap", floatingObjectProperties.AllowOverlap);
		}
		void ExportFloatingObjectLayoutInTableCell(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseLayoutInTableCell)
				RtfBuilder.WriteShapeBoolProperty("fLayoutInCell", floatingObjectProperties.LayoutInTableCell);
		}
		void ExportFloatingObjectVerticalPositionAlignment(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseVerticalPositionAlignment)
				RtfBuilder.WriteShapeIntegerProperty("posv", RtfExportSR.FloatingObjectVerticalPositionAlignmentTable[floatingObjectProperties.VerticalPositionAlignment]);
		}
		void ExportFloatingObjectVerticalPositionTypeProperty(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseVerticalPositionType)
				RtfBuilder.WriteShapeIntegerProperty("posrelv", RtfExportSR.FloatingObjectVerticalPositionTypeTable[floatingObjectProperties.VerticalPositionType]);
		}
		void ExportFloatingObjectHorizontalPositionAlignment(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseHorizontalPositionAlignment)
				RtfBuilder.WriteShapeIntegerProperty("posh", RtfExportSR.FloatingObjectHorizontalPositionAlignmentTable[floatingObjectProperties.HorizontalPositionAlignment]);
		}
		void ExportFloatingObjectHorizontalPositionTypeProperty(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseHorizontalPositionType)
				RtfBuilder.WriteShapeIntegerProperty("posrelh", RtfExportSR.FloatingObjectHorizontalPositionTypeTable[floatingObjectProperties.HorizontalPositionType]);
		}
		void ExportFloatingObjectRelativeSize(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseRelativeWidth) {
				FloatingObjectRelativeWidth width = floatingObjectProperties.RelativeWidth;
				RtfBuilder.WriteShapeIntegerProperty("sizerelh", RtfExportSR.FloatingObjectRelativeFromHorizontalTable[width.From]);
				RtfBuilder.WriteShapeIntegerProperty("pctHoriz", width.Width / 100);
			}
			if (floatingObjectProperties.UseRelativeHeight) {
				FloatingObjectRelativeHeight height = floatingObjectProperties.RelativeHeight;
				RtfBuilder.WriteShapeIntegerProperty("sizerelv", RtfExportSR.FloatingObjectRelativeFromVerticalTable[height.From]);
				RtfBuilder.WriteShapeIntegerProperty("pctVert", height.Height / 100);
			}
			if (floatingObjectProperties.UsePercentOffset) {
				if(floatingObjectProperties.PercentOffsetX != 0)
					RtfBuilder.WriteShapeIntegerProperty("pctHorizPos", floatingObjectProperties.PercentOffsetX / 100);
				if (floatingObjectProperties.PercentOffsetY != 0)
					RtfBuilder.WriteShapeIntegerProperty("pctVertPos", floatingObjectProperties.PercentOffsetY / 100);
			}
		}
		void ExportShapeInstanceProperties(FloatingObjectProperties floatingObjectProperties, int rotation) {
			int offsetX = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.X);
			int offsetY = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.Offset.Y);
			int rtfRoation = UnitConverter.ModelUnitsToFD(rotation);
			int width = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.ActualSize.Width);
			int height = UnitConverter.ModelUnitsToTwips(floatingObjectProperties.ActualSize.Height);
			if (DevExpress.XtraRichEdit.Import.Rtf.ShapeInstanceDestination.ShouldSwapSize(rtfRoation)) {
				int temp = width;
				width = height;
				height = temp;
				offsetX += width;
				offsetY -= width;
			}
			RtfBuilder.WriteCommand(RtfExportSR.ShapeLeft, offsetX);
			RtfBuilder.WriteCommand(RtfExportSR.ShapeTop, offsetY);
			RtfBuilder.WriteCommand(RtfExportSR.ShapeRight, offsetX + width);
			RtfBuilder.WriteCommand(RtfExportSR.ShapeBottom, offsetY + height);
			ExportFloatingObjectTextWrapType(floatingObjectProperties);
			ExportFloatingObjectHorizontalPositionType(floatingObjectProperties);
			ExportFloatingObjectVerticalPositionType(floatingObjectProperties);
			if(floatingObjectProperties.ZOrder != 0)
				RtfBuilder.WriteCommand(RtfExportSR.ShapeZOrder, floatingObjectProperties.ZOrder);
			if (floatingObjectProperties.UseTextWrapSide)
				RtfBuilder.WriteCommand(RtfExportSR.ShapeWrapTextSide, RtfExportSR.FloatingObjectTextWrapSideTable[floatingObjectProperties.TextWrapSide]);
			if (floatingObjectProperties.UseLocked && floatingObjectProperties.Locked == true)
				RtfBuilder.WriteCommand(RtfExportSR.ShapeLocked);
		}
		private void ExportFloatingObjectTextWrapType(FloatingObjectProperties floatingObjectProperties) {
			FloatingObjectTextWrapType textWrapType = floatingObjectProperties.TextWrapType;
			if (textWrapType == FloatingObjectTextWrapType.None) {
				RtfBuilder.WriteCommand(RtfExportSR.ShapeWrapTextType, "3");
				RtfBuilder.WriteCommand(RtfExportSR.ShapeWrapTextTypeZOrder, floatingObjectProperties.IsBehindDoc ? "1" : "0");
			}
			else
				RtfBuilder.WriteCommand(RtfExportSR.ShapeWrapTextType, RtfExportSR.FloatingObjectTextWrapTypeTable[textWrapType]);
		}
		void ExportFloatingObjectHorizontalPositionType(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseHorizontalPositionType) {
				FloatingObjectHorizontalPositionType horizontalPositionType = floatingObjectProperties.HorizontalPositionType;
				string shapeHorizontalPositionType;
				switch(horizontalPositionType){
					case FloatingObjectHorizontalPositionType.Page:
						shapeHorizontalPositionType = RtfExportSR.ShapeLegacyHorizontalPositionTypePage;
						break;
					case FloatingObjectHorizontalPositionType.Margin:
						shapeHorizontalPositionType = RtfExportSR.ShapeLegacyHorizontalPositionTypeMargin;
						break;
					case FloatingObjectHorizontalPositionType.Column:
						shapeHorizontalPositionType = RtfExportSR.ShapeLegacyHorizontalPositionTypeColumn;
						break;
					default:
						shapeHorizontalPositionType = RtfExportSR.ShapeLegacyHorizontalPositionTypeMargin;
						break;
				}
				RtfBuilder.WriteCommand(shapeHorizontalPositionType);
				RtfBuilder.WriteCommand(RtfExportSR.ShapeIgnoreLegacyHorizontalPositionType);
			}
		}
		void ExportFloatingObjectVerticalPositionType(FloatingObjectProperties floatingObjectProperties) {
			if (floatingObjectProperties.UseVerticalPositionType) {
				FloatingObjectVerticalPositionType VerticalPositionType = floatingObjectProperties.VerticalPositionType;
				string shapeVerticalPositionType;
				switch (VerticalPositionType) {
					case FloatingObjectVerticalPositionType.Page:
						shapeVerticalPositionType = RtfExportSR.ShapeLegacyVerticalPositionTypePage;
						break;
					case FloatingObjectVerticalPositionType.Margin:
						shapeVerticalPositionType = RtfExportSR.ShapeLegacyVerticalPositionTypeMargin;
						break;
					case FloatingObjectVerticalPositionType.Paragraph:
						shapeVerticalPositionType = RtfExportSR.ShapeLegacyVerticalPositionTypeParagraph;
						break;
					default:
						shapeVerticalPositionType = RtfExportSR.ShapeLegacyVerticalPositionTypeParagraph;
						break;
				}
				RtfBuilder.WriteCommand(shapeVerticalPositionType);
				RtfBuilder.WriteCommand(RtfExportSR.ShapeIgnoreLegacyVerticalPositionType);
			}
		}
	}
	#endregion
}
