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
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Border
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Border : IWordObject {
		bool Visible { get; set; }
		WdColorIndex ColorIndex { get; set; }
		bool Inside { get; }
		WdLineStyle LineStyle { get; set; }
		WdLineWidth LineWidth { get; set; }
		WdPageBorderArt ArtStyle { get; set; }
		int ArtWidth { get; set; }
		WdColor Color { get; set; }
	}
	#endregion
	#region Borders
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Borders : IWordObject, IEnumerable {
		int Count { get; }
		int Enable { get; set; }
		int DistanceFromTop { get; set; }
		bool Shadow { get; set; }
		WdLineStyle InsideLineStyle { get; set; }
		WdLineStyle OutsideLineStyle { get; set; }
		WdLineWidth InsideLineWidth { get; set; }
		WdLineWidth OutsideLineWidth { get; set; }
		WdColorIndex InsideColorIndex { get; set; }
		WdColorIndex OutsideColorIndex { get; set; }
		int DistanceFromLeft { get; set; }
		int DistanceFromBottom { get; set; }
		int DistanceFromRight { get; set; }
		bool AlwaysInFront { get; set; }
		bool SurroundHeader { get; set; }
		bool SurroundFooter { get; set; }
		bool JoinBorders { get; set; }
		bool HasHorizontal { get; }
		bool HasVertical { get; }
		WdBorderDistanceFrom DistanceFrom { get; set; }
		bool EnableFirstPageInSection { get; set; }
		bool EnableOtherPagesInSection { get; set; }
		Border this[WdBorderType Index] { get; }
		void ApplyPageBordersToAllSections();
		WdColor InsideColor { get; set; }
		WdColor OutsideColor { get; set; }
	}
	#endregion
	#region WdBorderType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdBorderType {
		wdBorderBottom = -3,
		wdBorderDiagonalDown = -7,
		wdBorderDiagonalUp = -8,
		wdBorderHorizontal = -5,
		wdBorderLeft = -2,
		wdBorderRight = -4,
		wdBorderTop = -1,
		wdBorderVertical = -6
	}
	#endregion
	#region WdLineStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLineStyle {
		wdLineStyleNone,
		wdLineStyleSingle,
		wdLineStyleDot,
		wdLineStyleDashSmallGap,
		wdLineStyleDashLargeGap,
		wdLineStyleDashDot,
		wdLineStyleDashDotDot,
		wdLineStyleDouble,
		wdLineStyleTriple,
		wdLineStyleThinThickSmallGap,
		wdLineStyleThickThinSmallGap,
		wdLineStyleThinThickThinSmallGap,
		wdLineStyleThinThickMedGap,
		wdLineStyleThickThinMedGap,
		wdLineStyleThinThickThinMedGap,
		wdLineStyleThinThickLargeGap,
		wdLineStyleThickThinLargeGap,
		wdLineStyleThinThickThinLargeGap,
		wdLineStyleSingleWavy,
		wdLineStyleDoubleWavy,
		wdLineStyleDashDotStroked,
		wdLineStyleEmboss3D,
		wdLineStyleEngrave3D,
		wdLineStyleOutset,
		wdLineStyleInset
	}
	#endregion
	#region WdLineWidth
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLineWidth {
		wdLineWidth025pt = 2,
		wdLineWidth050pt = 4,
		wdLineWidth075pt = 6,
		wdLineWidth100pt = 8,
		wdLineWidth150pt = 12,
		wdLineWidth225pt = 0x12,
		wdLineWidth300pt = 0x18,
		wdLineWidth450pt = 0x24,
		wdLineWidth600pt = 0x30
	}
	#endregion
	#region WdColorIndex
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdColorIndex {
		wdAuto = 0,
		wdBlack = 1,
		wdBlue = 2,
		wdBrightGreen = 4,
		wdByAuthor = -1,
		wdDarkBlue = 9,
		wdDarkRed = 13,
		wdDarkYellow = 14,
		wdGray25 = 0x10,
		wdGray50 = 15,
		wdGreen = 11,
		wdNoHighlight = 0,
		wdPink = 5,
		wdRed = 6,
		wdTeal = 10,
		wdTurquoise = 3,
		wdViolet = 12,
		wdWhite = 8,
		wdYellow = 7
	}
	#endregion
	#region WdColor
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdColor {
		wdColorAqua = 0xcccc33,
		wdColorAutomatic = -16777216,
		wdColorBlack = 0,
		wdColorBlue = 0xff0000,
		wdColorBlueGray = 0x996666,
		wdColorBrightGreen = 0xff00,
		wdColorBrown = 0x3399,
		wdColorDarkBlue = 0x800000,
		wdColorDarkGreen = 0x3300,
		wdColorDarkRed = 0x80,
		wdColorDarkTeal = 0x663300,
		wdColorDarkYellow = 0x8080,
		wdColorGold = 0xccff,
		wdColorGray05 = 0xf3f3f3,
		wdColorGray10 = 0xe6e6e6,
		wdColorGray125 = 0xe0e0e0,
		wdColorGray15 = 0xd9d9d9,
		wdColorGray20 = 0xcccccc,
		wdColorGray25 = 0xc0c0c0,
		wdColorGray30 = 0xb3b3b3,
		wdColorGray35 = 0xa6a6a6,
		wdColorGray375 = 0xa0a0a0,
		wdColorGray40 = 0x999999,
		wdColorGray45 = 0x8c8c8c,
		wdColorGray50 = 0x808080,
		wdColorGray55 = 0x737373,
		wdColorGray60 = 0x666666,
		wdColorGray625 = 0x606060,
		wdColorGray65 = 0x595959,
		wdColorGray70 = 0x4c4c4c,
		wdColorGray75 = 0x404040,
		wdColorGray80 = 0x333333,
		wdColorGray85 = 0x262626,
		wdColorGray875 = 0x202020,
		wdColorGray90 = 0x191919,
		wdColorGray95 = 0xc0c0c,
		wdColorGreen = 0x8000,
		wdColorIndigo = 0x993333,
		wdColorLavender = 0xff99cc,
		wdColorLightBlue = 0xff6633,
		wdColorLightGreen = 0xccffcc,
		wdColorLightOrange = 0x99ff,
		wdColorLightTurquoise = 0xffffcc,
		wdColorLightYellow = 0x99ffff,
		wdColorLime = 0xcc99,
		wdColorOliveGreen = 0x3333,
		wdColorOrange = 0x66ff,
		wdColorPaleBlue = 0xffcc99,
		wdColorPink = 0xff00ff,
		wdColorPlum = 0x663399,
		wdColorRed = 0xff,
		wdColorRose = 0xcc99ff,
		wdColorSeaGreen = 0x669933,
		wdColorSkyBlue = 0xffcc00,
		wdColorTan = 0x99ccff,
		wdColorTeal = 0x808000,
		wdColorTurquoise = 0xffff00,
		wdColorViolet = 0x800080,
		wdColorWhite = 0xffffff,
		wdColorYellow = 0xffff
	}
	#endregion
	#region WdBorderDistanceFrom
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdBorderDistanceFrom {
		wdBorderDistanceFromText,
		wdBorderDistanceFromPageEdge
	}
	#endregion
	#region WdPageBorderArt
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPageBorderArt {
		wdArtApples = 1,
		wdArtArchedScallops = 0x61,
		wdArtBabyPacifier = 70,
		wdArtBabyRattle = 0x47,
		wdArtBalloons3Colors = 11,
		wdArtBalloonsHotAir = 12,
		wdArtBasicBlackDashes = 0x9b,
		wdArtBasicBlackDots = 0x9c,
		wdArtBasicBlackSquares = 0x9a,
		wdArtBasicThinLines = 0x97,
		wdArtBasicWhiteDashes = 0x98,
		wdArtBasicWhiteDots = 0x93,
		wdArtBasicWhiteSquares = 0x99,
		wdArtBasicWideInline = 150,
		wdArtBasicWideMidline = 0x94,
		wdArtBasicWideOutline = 0x95,
		wdArtBats = 0x25,
		wdArtBirds = 0x66,
		wdArtBirdsFlight = 0x23,
		wdArtCabins = 0x48,
		wdArtCakeSlice = 3,
		wdArtCandyCorn = 4,
		wdArtCelticKnotwork = 0x63,
		wdArtCertificateBanner = 0x9e,
		wdArtChainLink = 0x80,
		wdArtChampagneBottle = 6,
		wdArtCheckedBarBlack = 0x91,
		wdArtCheckedBarColor = 0x3d,
		wdArtCheckered = 0x90,
		wdArtChristmasTree = 8,
		wdArtCirclesLines = 0x5b,
		wdArtCirclesRectangles = 140,
		wdArtClassicalWave = 0x38,
		wdArtClocks = 0x1b,
		wdArtCompass = 0x36,
		wdArtConfetti = 0x1f,
		wdArtConfettiGrays = 0x73,
		wdArtConfettiOutline = 0x74,
		wdArtConfettiStreamers = 14,
		wdArtConfettiWhite = 0x75,
		wdArtCornerTriangles = 0x8d,
		wdArtCouponCutoutDashes = 0xa3,
		wdArtCouponCutoutDots = 0xa4,
		wdArtCrazyMaze = 100,
		wdArtCreaturesButterfly = 0x20,
		wdArtCreaturesFish = 0x22,
		wdArtCreaturesInsects = 0x8e,
		wdArtCreaturesLadyBug = 0x21,
		wdArtCrossStitch = 0x8a,
		wdArtCup = 0x43,
		wdArtDecoArch = 0x59,
		wdArtDecoArchColor = 50,
		wdArtDecoBlocks = 90,
		wdArtDiamondsGray = 0x58,
		wdArtDoubleD = 0x37,
		wdArtDoubleDiamonds = 0x7f,
		wdArtEarth1 = 0x16,
		wdArtEarth2 = 0x15,
		wdArtEclipsingSquares1 = 0x65,
		wdArtEclipsingSquares2 = 0x56,
		wdArtEggsBlack = 0x42,
		wdArtFans = 0x33,
		wdArtFilm = 0x34,
		wdArtFirecrackers = 0x1c,
		wdArtFlowersBlockPrint = 0x31,
		wdArtFlowersDaisies = 0x30,
		wdArtFlowersModern1 = 0x2d,
		wdArtFlowersModern2 = 0x2c,
		wdArtFlowersPansy = 0x2b,
		wdArtFlowersRedRose = 0x27,
		wdArtFlowersRoses = 0x26,
		wdArtFlowersTeacup = 0x67,
		wdArtFlowersTiny = 0x2a,
		wdArtGems = 0x8b,
		wdArtGingerbreadMan = 0x45,
		wdArtGradient = 0x7a,
		wdArtHandmade1 = 0x9f,
		wdArtHandmade2 = 160,
		wdArtHeartBalloon = 0x10,
		wdArtHeartGray = 0x44,
		wdArtHearts = 15,
		wdArtHeebieJeebies = 120,
		wdArtHolly = 0x29,
		wdArtHouseFunky = 0x49,
		wdArtHypnotic = 0x57,
		wdArtIceCreamCones = 5,
		wdArtLightBulb = 0x79,
		wdArtLightning1 = 0x35,
		wdArtLightning2 = 0x77,
		wdArtMapleLeaf = 0x51,
		wdArtMapleMuffins = 2,
		wdArtMapPins = 30,
		wdArtMarquee = 0x92,
		wdArtMarqueeToothed = 0x83,
		wdArtMoons = 0x7d,
		wdArtMosaic = 0x76,
		wdArtMusicNotes = 0x4f,
		wdArtNorthwest = 0x68,
		wdArtOvals = 0x7e,
		wdArtPackages = 0x1a,
		wdArtPalmsBlack = 80,
		wdArtPalmsColor = 10,
		wdArtPaperClips = 0x52,
		wdArtPapyrus = 0x5c,
		wdArtPartyFavor = 13,
		wdArtPartyGlass = 7,
		wdArtPencils = 0x19,
		wdArtPeople = 0x54,
		wdArtPeopleHats = 0x17,
		wdArtPeopleWaving = 0x55,
		wdArtPoinsettias = 40,
		wdArtPostageStamp = 0x87,
		wdArtPumpkin1 = 0x41,
		wdArtPushPinNote1 = 0x3f,
		wdArtPushPinNote2 = 0x40,
		wdArtPyramids = 0x71,
		wdArtPyramidsAbove = 0x72,
		wdArtQuadrants = 60,
		wdArtRings = 0x1d,
		wdArtSafari = 0x62,
		wdArtSawtooth = 0x85,
		wdArtSawtoothGray = 0x86,
		wdArtScaredCat = 0x24,
		wdArtSeattle = 0x4e,
		wdArtShadowedSquares = 0x39,
		wdArtSharksTeeth = 0x84,
		wdArtShorebirdTracks = 0x53,
		wdArtSkyrocket = 0x4d,
		wdArtSnowflakeFancy = 0x4c,
		wdArtSnowflakes = 0x4b,
		wdArtSombrero = 0x18,
		wdArtSouthwest = 0x69,
		wdArtStars = 0x13,
		wdArtStars3D = 0x11,
		wdArtStarsBlack = 0x4a,
		wdArtStarsShadowed = 0x12,
		wdArtStarsTop = 0x9d,
		wdArtSun = 20,
		wdArtSwirligig = 0x3e,
		wdArtTornPaper = 0xa1,
		wdArtTornPaperBlack = 0xa2,
		wdArtTrees = 9,
		wdArtTriangleParty = 0x7b,
		wdArtTriangles = 0x81,
		wdArtTribal1 = 130,
		wdArtTribal2 = 0x6d,
		wdArtTribal3 = 0x6c,
		wdArtTribal4 = 0x6b,
		wdArtTribal5 = 110,
		wdArtTribal6 = 0x6a,
		wdArtTwistedLines1 = 0x3a,
		wdArtTwistedLines2 = 0x7c,
		wdArtVine = 0x2f,
		wdArtWaveline = 0x3b,
		wdArtWeavingAngles = 0x60,
		wdArtWeavingBraid = 0x5e,
		wdArtWeavingRibbon = 0x5f,
		wdArtWeavingStrips = 0x88,
		wdArtWhiteFlowers = 0x2e,
		wdArtWoodwork = 0x5d,
		wdArtXIllusions = 0x6f,
		wdArtZanyTriangles = 0x70,
		wdArtZigZag = 0x89,
		wdArtZigZagStitch = 0x8f
	}
	#endregion
}
