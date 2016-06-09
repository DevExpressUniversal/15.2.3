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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region BorderLineStyle
	public enum BorderLineStyle {
		Nil = -1,
		None = 0,
		Single = 1,
		Thick = 2,
		Double = 3,
		Dotted = 4,
		Dashed = 5,
		DotDash = 6,
		DotDotDash = 7,
		Triple = 8,
		ThinThickSmallGap = 9,
		ThickThinSmallGap = 10,
		ThinThickThinSmallGap = 11,
		ThinThickMediumGap = 12,
		ThickThinMediumGap = 13,
		ThinThickThinMediumGap = 14,
		ThinThickLargeGap = 15,
		ThickThinLargeGap = 16,
		ThinThickThinLargeGap = 17,
		Wave = 18,
		DoubleWave = 19,
		DashSmallGap = 20,
		DashDotStroked = 21,
		ThreeDEmboss = 22,
		ThreeDEngrave = 23,
		Outset = 24,
		Inset = 25,		
		Apples,
		ArchedScallops,
		BabyPacifier,
		BabyRattle,
		Balloons3Colors,
		BalloonsHotAir,
		BasicBlackDashes,
		BasicBlackDots,
		BasicBlackSquares,
		BasicThinLines,
		BasicWhiteDashes,
		BasicWhiteDots,
		BasicWhiteSquares,
		BasicWideInline,
		BasicWideMidline,
		BasicWideOutline,
		Bats,
		Birds,
		BirdsFlight,
		Cabins,
		CakeSlice,
		CandyCorn,
		CelticKnotwork,
		CertificateBanner,
		ChainLink,
		ChampagneBottle,
		CheckedBarBlack,
		CheckedBarColor,
		Checkered,
		ChristmasTree,
		CirclesLines,
		CirclesRectangles,
		ClassicalWave,
		Clocks,
		Compass,
		Confetti,
		ConfettiGrays,
		ConfettiOutline,
		ConfettiStreamers,
		ConfettiWhite,
		CornerTriangles,
		CouponCutoutDashes,
		CouponCutoutDots,
		CrazyMaze,
		CreaturesButterfly,
		CreaturesFish,
		CreaturesInsects,
		CreaturesLadyBug,
		CrossStitch,
		Cup,
		DecoArch,
		DecoArchColor,
		DecoBlocks,
		DiamondsGray,
		DoubleD,
		DoubleDiamonds,
		Earth1,
		Earth2,
		EclipsingSquares1,
		EclipsingSquares2,
		EggsBlack,
		Fans,
		Film,
		Firecrackers,
		FlowersBlockPrint,
		FlowersDaisies,
		FlowersModern1,
		FlowersModern2,
		FlowersPansy,
		FlowersRedRose,
		FlowersRoses,
		FlowersTeacup,
		FlowersTiny,
		Gems,
		GingerbreadMan,
		Gradient,
		Handmade1,
		Handmade2,
		HeartBalloon,
		HeartGray,
		Hearts,
		HeebieJeebies,
		Holly,
		HouseFunky,
		Hypnotic,
		IceCreamCones,
		LightBulb,
		Lightning1,
		Lightning2,
		MapleLeaf,
		MapleMuffins,
		MapPins,
		Marquee,
		MarqueeToothed,
		Moons,
		Mosaic,
		MusicNotes,
		Northwest,		
		Ovals,
		Packages,
		PalmsBlack,
		PalmsColor,
		PaperClips,
		Papyrus,
		PartyFavor,
		PartyGlass,
		Pencils,
		People,
		PeopleHats,
		PeopleWaving,
		Poinsettias,
		PostageStamp,
		Pumpkin1,
		PushPinNote1,
		PushPinNote2,
		Pyramids,
		PyramidsAbove,
		Quadrants,
		Rings,
		Safari,
		Sawtooth,
		SawtoothGray,
		ScaredCat,
		Seattle,
		ShadowedSquares,
		SharksTeeth,
		ShorebirdTracks,
		Skyrocket,
		SnowflakeFancy,
		Snowflakes,
		Sombrero,
		Southwest,
		Stars,
		Stars3d,
		StarsBlack,
		StarsShadowed,
		StarsTop,
		Sun,
		Swirligig,
		TornPaper,
		TornPaperBlack,
		Trees,
		TriangleParty,
		Triangles,
		Tribal1,
		Tribal2,
		Tribal3,
		Tribal4,
		Tribal5,
		Tribal6,
		TwistedLines1,
		TwistedLines2,
		Vine,
		Waveline,
		WeavingAngles,
		WeavingBraid,
		WeavingRibbon,
		WeavingStrips,
		WhiteFlowers,
		Woodwork,
		XIllusions,
		ZanyTriangles,
		ZigZag,
		ZigZagStitch,
		Disabled = 0x7FFFFFFF,
	}
	#endregion
	#region BorderCollection
	public class BorderCollection : List<Underline> {
	}
	#endregion
	#region BorderLineRepository
	public class BorderLineRepository : PatternLineRepository<UnderlineType, Underline, BorderCollection> {
		readonly static Dictionary<BorderLineStyle, UnderlineType> mapBorderStyleToUnderling;
		static BorderLineRepository() {
			mapBorderStyleToUnderling = new Dictionary<BorderLineStyle, UnderlineType>();
			mapBorderStyleToUnderling.Add(BorderLineStyle.DashSmallGap, UnderlineType.DashSmallGap);
			mapBorderStyleToUnderling.Add(BorderLineStyle.DotDotDash, UnderlineType.DashDotDotted);
			mapBorderStyleToUnderling.Add(BorderLineStyle.DotDash, UnderlineType.DashDotted);
			mapBorderStyleToUnderling.Add(BorderLineStyle.Dashed, UnderlineType.Dashed);
			mapBorderStyleToUnderling.Add(BorderLineStyle.Dotted, UnderlineType.Dotted);
			mapBorderStyleToUnderling.Add(BorderLineStyle.Double, UnderlineType.Double);
			mapBorderStyleToUnderling.Add(BorderLineStyle.DoubleWave, UnderlineType.DoubleWave);			
			mapBorderStyleToUnderling.Add(BorderLineStyle.Wave, UnderlineType.Wave);			
		}
		protected override void PopulateRepository() {
			RegisterPatternLine(Underline.UnderlineNone);
			RegisterPatternLine(new UnderlineSingle());
			RegisterPatternLine(new UnderlineDotted());
			RegisterPatternLine(new UnderlineDashed());
			RegisterPatternLine(new UnderlineDashSmallGap());
			RegisterPatternLine(new UnderlineDashDotted());
			RegisterPatternLine(new UnderlineDashDotDotted());
			RegisterPatternLine(new UnderlineDouble());
			RegisterPatternLine(new UnderlineHeavyWave());
			RegisterPatternLine(new UnderlineLongDashed());
			RegisterPatternLine(new UnderlineThickSingle());
			RegisterPatternLine(new UnderlineThickDotted());
			RegisterPatternLine(new UnderlineThickDashed());
			RegisterPatternLine(new UnderlineThickDashDotted());
			RegisterPatternLine(new UnderlineThickDashDotDotted());
			RegisterPatternLine(new UnderlineThickLongDashed());
			RegisterPatternLine(new UnderlineDoubleWave());
			RegisterPatternLine(new UnderlineWave());
		}
		protected internal Underline GetCharacterLineByType(BorderLineStyle lineStyle) {
			UnderlineType underlineType = GetUnderlineType(lineStyle);
			if (underlineType != UnderlineType.None)
				return GetPatternLineByType(underlineType);
			else
				return null;
		}
		protected UnderlineType GetUnderlineType(BorderLineStyle lineStyle) {
			UnderlineType result;
			if (mapBorderStyleToUnderling.TryGetValue(lineStyle, out result))
				return result;
			else
				return UnderlineType.None;
		}
	}
	#endregion
}
