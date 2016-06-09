#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Reflection;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon {
	public enum FormatConditionAppearanceType {
		None,
		Custom,
		PaleRed,
		PaleYellow,
		PaleGreen,
		PaleBlue,
		PalePurple,
		PaleCyan,
		PaleOrange,
		PaleGray,
		Red,
		Yellow,
		Green,
		Blue,
		Purple,
		Cyan,
		Orange,
		Gray,
		GradientRed,
		GradientYellow,
		GradientGreen,
		GradientBlue,
		GradientPurple,
		GradientCyan,
		GradientOrange,
		GradientTransparent,
		FontBold,
		FontItalic,
		FontUnderline,
		FontGrayed,
		FontRed,
		FontYellow,
		FontGreen,
		FontBlue
	}
	public enum FormatConditionIconType {
		None,
		DirectionalGreenArrowUp,
		DirectionalYellowUpInclineArrow,
		DirectionalYellowSideArrow,
		DirectionalYellowDownInclineArrow,
		DirectionalRedDownArrow,
		DirectionalGrayArrowUp,
		DirectionalGrayUpInclineArrow,
		DirectionalGraySideArrow,
		DirectionalGrayDownInclineArrow,
		DirectionalGrayDownArrow,
		DirectionalYellowDash,
		DirectionalRedTriangleDown,
		DirectionalGreenTriangleUp,
		RatingFullGrayStar,
		RatingHalfGrayStar,
		RatingEmptyGrayStar,
		RatingFullGrayCircle,
		Rating1QuarterGrayCircle,
		Rating2QuartersGrayCircle,
		Rating3QuartersGrayCircle,
		RatingEmptyGrayCircle,
		Rating4Bars,
		Rating3Bars,
		Rating2Bars,
		Rating1Bar,
		Rating0Bars,
		Rating4FilledBoxes,
		Rating3FilledBoxes,
		Rating2FilledBoxes,
		Rating1FilledBox,
		Rating0FilledBoxes,
		ShapeGreenTrafficLight,
		ShapeYellowTrafficLight,
		ShapeRedTrafficLight,
		ShapeGreenCircle,
		ShapeYellowCircle,
		ShapeRedCircle,
		ShapeLightRedCircle,
		ShapeLightGrayCircle,
		ShapeYellowTriangle,
		ShapeRedDiamond,
		IndicatorGreenCheck,
		IndicatorYellowExclamation,
		IndicatorRedCross,
		IndicatorCircledGreenCheck,
		IndicatorCircledYellowExclamation,
		IndicatorCircledRedCross,
		IndicatorGreenFlag,
		IndicatorYellowFlag,
		IndicatorRedFlag
	}
	public enum FormatConditionRangeSetPredefinedType {
		None,
		Custom,
		Arrows2,
		Arrows3,
		Arrows4,
		Arrows5,
		ArrowsGray2,
		ArrowsGray3,
		ArrowsGray4,
		ArrowsGray5,
		PositiveNegative3,
		Stars3,
		Quarters5,
		Bars4,
		Bars5,
		Boxes5,
		TrafficLights3,
		Circles2,
		Circles3,
		Circles4,
		CirclesRedToBlack4,
		Signs3,
		Symbols2,
		Symbols3,
		SymbolsCircled2,
		SymbolsCircled3,
		Flags3,
		ColorsPaleRedGreen,
		ColorsPaleRedGreenBlue,
		ColorsPaleRedYellowGreenBlue,
		ColorsPaleRedOrangeYellowGreenBlue,
		ColorsRedGreen,
		ColorsRedGreenBlue,
		ColorsRedYellowGreenBlue,
		ColorsRedOrangeYellowGreenBlue
	}
	public enum FormatConditionRangeGradientPredefinedType {
		None,
		Custom,
		GreenWhite,
		WhiteGreen,
		RedWhite,
		WhiteRed,
		YellowGreen,
		GreenYellow,
		YellowRed,
		RedYellow,
		BlueWhite,
		WhiteBlue,
		BlueRed,
		RedBlue,
		YellowBlue,
		BlueYellow,
		GreenBlue,
		BlueGreen,
		GreenWhiteBlue,
		BlueWhiteGreen,
		BlueWhiteRed,
		RedWhiteBlue,
		GreenWhiteRed,
		RedWhiteGreen,
		GreenYellowRed,
		RedYellowGreen,
		BlueYellowRed,
		RedYellowBlue,
		GreenYellowBlue,
		BlueYellowGreen
	}
	enum FormatConditionColorScheme { Light, Dark }
	enum FormatConditionAppearanceTypeGroups {
		BackColors,
		BackColorsWithFont,
		GradientColors,
		Fonts
	}
	enum FormatConditionIconGroups {
		Directional,
		Indicators,
		Flags,
		Shapes,
		RatingsMonochrome,
		RatingsColor
	}
	enum FormatConditionRangeSetTypeGroups {
		Ranges2,
		Ranges3,
		Ranges4,
		Ranges5
	}
	enum FormatConditionRangeGradientTypeGroups {
		TwoColors,
		ThreeColors
	}
	static class FormatConditionEnumExtensions {
		static Color WhiteColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
		static Color GrayedTextColor = Color.FromArgb(0xD3, 0xD3, 0xD3);
		static Color LightGradientRedColor = Color.FromArgb(255, 166, 173);
		static Color LightGradientYellowColor = Color.FromArgb(255, 226, 81);
		static Color LightGradientGreenColor = Color.FromArgb(139, 210, 78);
		static Color LightGradientBlueColor = Color.FromArgb(149, 204, 255);
		static Color LightGradientPurpleColor = Color.FromArgb(223, 166, 232);
		static Color LightGradientCyanColor = Color.FromArgb(113, 223, 221);
		static Color LightGradientOrangeColor = Color.FromArgb(255, 182, 90);
		static Color LightGradientTransparentColor = Color.FromArgb(0, 0xFF, 0xFF, 0xFF);
		static Color DarkGradientRedColor = Color.FromArgb(0xAC, 0x20, 0x3D);
		static Color DarkGradientYellowColor = Color.FromArgb(0xFF, 0x8A, 0x01);
		static Color DarkGradientGreenColor = Color.FromArgb(0x53, 0x8A, 0x31);
		static Color DarkGradientBlueColor = Color.FromArgb(0x43, 0x71, 0xB0);
		static Color DarkGradientPurpleColor = Color.FromArgb(0x7E, 0x53, 0xA2);
		static Color DarkGradientCyanColor = Color.FromArgb(0x14, 0x9B, 0xA3);
		static Color DarkGradientOrangeColor = Color.FromArgb(0xD8, 0x3D, 0x00);
		static Color DarkGradientTransparentColor = Color.FromArgb(0, 0x00, 0x00, 0x00);
		static Color LightPaleRedColor = Color.FromArgb(255, 221, 224);
		static Color LightPaleYellowColor = Color.FromArgb(255, 245, 174);
		static Color LightPaleGreenColor = Color.FromArgb(208, 239, 172);
		static Color LightPaleBlueColor = Color.FromArgb(213, 237, 255);
		static Color LightPalePurpleColor = Color.FromArgb(244, 221, 247);
		static Color LightPaleCyanColor = Color.FromArgb(194, 244, 243);
		static Color LightPaleOrangeColor = Color.FromArgb(255, 228, 180);
		static Color LightPaleGrayColor = Color.FromArgb(234, 234, 234);
		static Color DarkPaleRedColor = Color.FromArgb(0x5B, 0x2D, 0x3D);
		static Color DarkPaleYellowColor = Color.FromArgb(0x51, 0x49, 0x2D);
		static Color DarkPaleGreenColor = Color.FromArgb(0x3B, 0x4D, 0x2D);
		static Color DarkPaleBlueColor = Color.FromArgb(0x2D, 0x3F, 0x5A);
		static Color DarkPalePurpleColor = Color.FromArgb(0x51, 0x2D, 0x55);
		static Color DarkPaleCyanColor = Color.FromArgb(0x2D, 0x4B, 0x4B);
		static Color DarkPaleOrangeColor = Color.FromArgb(0x59, 0x3E, 0x2D);
		static Color DarkPaleGrayColor = Color.FromArgb(0x44, 0x44, 0x44);
		static Color LightRedColor = Color.FromArgb(226, 60, 76);
		static Color LightYellowColor = Color.FromArgb(255, 166, 38);
		static Color LightGreenColor = Color.FromArgb(101, 172, 80);
		static Color LightBlueColor = Color.FromArgb(89, 143, 216);
		static Color LightPurpleColor = Color.FromArgb(148, 105, 184);
		static Color LightCyanColor = Color.FromArgb(39, 192, 187);
		static Color LightOrangeColor = Color.FromArgb(255, 92, 12);
		static Color LightGrayColor = Color.FromArgb(111, 111, 111);
		static Color DarkRedColor = Color.FromArgb(0xE2, 0x3C, 0x4C);
		static Color DarkYellowColor = Color.FromArgb(0xFF, 0xA6, 0x26);
		static Color DarkGreenColor = Color.FromArgb(0x65, 0xAC, 0x50);
		static Color DarkBlueColor = Color.FromArgb(0x59, 0x8F, 0xD8);
		static Color DarkPurpleColor = Color.FromArgb(0x94, 0x69, 0xB8);
		static Color DarkCyanColor = Color.FromArgb(0x27, 0xC0, 0xBB);
		static Color DarkOrangeColor = Color.FromArgb(0xFF, 0x5C, 0x0C);
		static Color DarkGrayColor = Color.FromArgb(0x6F, 0x6F, 0x6F);
#if !DXPORTABLE
		public static Image ToImage(this FormatConditionIconType iconType, FormatConditionColorScheme schema) {
			return ResourceImageHelper.CreateBitmapFromResources(string.Format("DevExpress.DashboardCommon.ConditionalFormatting.Images.{0}.{1}.png", schema == FormatConditionColorScheme.Light ? "Light" : "Dark", iconType), typeof(FormatConditionIconType).GetAssembly());
		}
#endif
		public static IList<FormatConditionIconType> ToIconTypes(this FormatConditionRangeSetPredefinedType iconRangeSetType) {
			switch(iconRangeSetType) {
				case FormatConditionRangeSetPredefinedType.Arrows2:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalRedDownArrow, FormatConditionIconType.DirectionalGreenArrowUp };
				case FormatConditionRangeSetPredefinedType.Arrows3:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalRedDownArrow, FormatConditionIconType.DirectionalYellowSideArrow, FormatConditionIconType.DirectionalGreenArrowUp };
				case FormatConditionRangeSetPredefinedType.Arrows4:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalRedDownArrow, FormatConditionIconType.DirectionalYellowDownInclineArrow, FormatConditionIconType.DirectionalYellowUpInclineArrow, FormatConditionIconType.DirectionalGreenArrowUp };
				case FormatConditionRangeSetPredefinedType.Arrows5:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalRedDownArrow, FormatConditionIconType.DirectionalYellowDownInclineArrow, FormatConditionIconType.DirectionalYellowSideArrow, FormatConditionIconType.DirectionalYellowUpInclineArrow, FormatConditionIconType.DirectionalGreenArrowUp };
				case FormatConditionRangeSetPredefinedType.ArrowsGray2:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalGrayDownArrow, FormatConditionIconType.DirectionalGrayArrowUp };
				case FormatConditionRangeSetPredefinedType.ArrowsGray3:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalGrayDownArrow, FormatConditionIconType.DirectionalGraySideArrow, FormatConditionIconType.DirectionalGrayArrowUp };
				case FormatConditionRangeSetPredefinedType.ArrowsGray4:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalGrayDownArrow, FormatConditionIconType.DirectionalGrayDownInclineArrow, FormatConditionIconType.DirectionalGrayUpInclineArrow, FormatConditionIconType.DirectionalGrayArrowUp };
				case FormatConditionRangeSetPredefinedType.ArrowsGray5:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalGrayDownArrow, FormatConditionIconType.DirectionalGrayDownInclineArrow, FormatConditionIconType.DirectionalGraySideArrow, FormatConditionIconType.DirectionalGrayUpInclineArrow, FormatConditionIconType.DirectionalGrayArrowUp };
				case FormatConditionRangeSetPredefinedType.PositiveNegative3:
					return new FormatConditionIconType[] { FormatConditionIconType.DirectionalRedTriangleDown, FormatConditionIconType.DirectionalYellowDash, FormatConditionIconType.DirectionalGreenTriangleUp };
				case FormatConditionRangeSetPredefinedType.Stars3:
					return new FormatConditionIconType[] { FormatConditionIconType.RatingEmptyGrayStar, FormatConditionIconType.RatingHalfGrayStar, FormatConditionIconType.RatingFullGrayStar };
				case FormatConditionRangeSetPredefinedType.Quarters5:
					return new FormatConditionIconType[] { FormatConditionIconType.RatingEmptyGrayCircle, FormatConditionIconType.Rating3QuartersGrayCircle, FormatConditionIconType.Rating2QuartersGrayCircle, FormatConditionIconType.Rating1QuarterGrayCircle, FormatConditionIconType.RatingFullGrayCircle };
				case FormatConditionRangeSetPredefinedType.Bars4:
					return new FormatConditionIconType[] { FormatConditionIconType.Rating1Bar, FormatConditionIconType.Rating2Bars, FormatConditionIconType.Rating3Bars, FormatConditionIconType.Rating4Bars };
				case FormatConditionRangeSetPredefinedType.Bars5:
					return new FormatConditionIconType[] { FormatConditionIconType.Rating0Bars, FormatConditionIconType.Rating1Bar, FormatConditionIconType.Rating2Bars, FormatConditionIconType.Rating3Bars, FormatConditionIconType.Rating4Bars };
				case FormatConditionRangeSetPredefinedType.Boxes5:
					return new FormatConditionIconType[] { FormatConditionIconType.Rating0FilledBoxes, FormatConditionIconType.Rating1FilledBox, FormatConditionIconType.Rating2FilledBoxes, FormatConditionIconType.Rating3FilledBoxes, FormatConditionIconType.Rating4FilledBoxes };
				case FormatConditionRangeSetPredefinedType.TrafficLights3:
					return new FormatConditionIconType[] { FormatConditionIconType.ShapeRedTrafficLight, FormatConditionIconType.ShapeYellowTrafficLight, FormatConditionIconType.ShapeGreenTrafficLight };
				case FormatConditionRangeSetPredefinedType.Circles2:
					return new FormatConditionIconType[] { FormatConditionIconType.ShapeRedCircle, FormatConditionIconType.ShapeGreenCircle };
				case FormatConditionRangeSetPredefinedType.Circles3:
					return new FormatConditionIconType[] { FormatConditionIconType.ShapeRedCircle, FormatConditionIconType.ShapeYellowCircle, FormatConditionIconType.ShapeGreenCircle };
				case FormatConditionRangeSetPredefinedType.Circles4:
					return new FormatConditionIconType[] { FormatConditionIconType.RatingFullGrayCircle, FormatConditionIconType.ShapeRedCircle, FormatConditionIconType.ShapeYellowCircle, FormatConditionIconType.ShapeGreenCircle };
				case FormatConditionRangeSetPredefinedType.CirclesRedToBlack4:
					return new FormatConditionIconType[] { FormatConditionIconType.RatingFullGrayCircle, FormatConditionIconType.ShapeLightGrayCircle, FormatConditionIconType.ShapeLightRedCircle, FormatConditionIconType.ShapeRedCircle };
				case FormatConditionRangeSetPredefinedType.Signs3:
					return new FormatConditionIconType[] { FormatConditionIconType.ShapeRedDiamond, FormatConditionIconType.ShapeYellowTriangle, FormatConditionIconType.ShapeGreenCircle };
				case FormatConditionRangeSetPredefinedType.Symbols2:
					return new FormatConditionIconType[] { FormatConditionIconType.IndicatorRedCross, FormatConditionIconType.IndicatorGreenCheck };
				case FormatConditionRangeSetPredefinedType.Symbols3:
					return new FormatConditionIconType[] { FormatConditionIconType.IndicatorRedCross, FormatConditionIconType.IndicatorYellowExclamation, FormatConditionIconType.IndicatorGreenCheck };
				case FormatConditionRangeSetPredefinedType.SymbolsCircled2:
					return new FormatConditionIconType[] { FormatConditionIconType.IndicatorCircledRedCross, FormatConditionIconType.IndicatorCircledGreenCheck };
				case FormatConditionRangeSetPredefinedType.SymbolsCircled3:
					return new FormatConditionIconType[] { FormatConditionIconType.IndicatorCircledRedCross, FormatConditionIconType.IndicatorCircledYellowExclamation, FormatConditionIconType.IndicatorCircledGreenCheck };
				case FormatConditionRangeSetPredefinedType.Flags3:
					return new FormatConditionIconType[] { FormatConditionIconType.IndicatorRedFlag, FormatConditionIconType.IndicatorYellowFlag, FormatConditionIconType.IndicatorGreenFlag };
				default:
					return null;
			}
		}
		public static IList<FormatConditionAppearanceType> ToColorTypes(this FormatConditionRangeSetPredefinedType colorRangeSetType) {
			switch(colorRangeSetType) {
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.PaleRed, FormatConditionAppearanceType.PaleGreen };
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.PaleRed, FormatConditionAppearanceType.PaleGreen, FormatConditionAppearanceType.PaleBlue };
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedYellowGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.PaleRed, FormatConditionAppearanceType.PaleYellow, FormatConditionAppearanceType.PaleGreen, FormatConditionAppearanceType.PaleBlue };
				case FormatConditionRangeSetPredefinedType.ColorsPaleRedOrangeYellowGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.PaleRed, FormatConditionAppearanceType.PaleOrange, FormatConditionAppearanceType.PaleYellow, FormatConditionAppearanceType.PaleGreen, FormatConditionAppearanceType.PaleBlue };
				case FormatConditionRangeSetPredefinedType.ColorsRedGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.Red, FormatConditionAppearanceType.Green };
				case FormatConditionRangeSetPredefinedType.ColorsRedGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.Red, FormatConditionAppearanceType.Green, FormatConditionAppearanceType.Blue };
				case FormatConditionRangeSetPredefinedType.ColorsRedYellowGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.Red, FormatConditionAppearanceType.Yellow, FormatConditionAppearanceType.Green, FormatConditionAppearanceType.Blue };
				case FormatConditionRangeSetPredefinedType.ColorsRedOrangeYellowGreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.Red, FormatConditionAppearanceType.Orange, FormatConditionAppearanceType.Yellow, FormatConditionAppearanceType.Green, FormatConditionAppearanceType.Blue };
				default:
					return null;
			}
		}
		public static IList<FormatConditionAppearanceType> ToAppearanceTypes(this FormatConditionRangeGradientPredefinedType gradientType) {
			switch(gradientType) {
				case FormatConditionRangeGradientPredefinedType.GreenWhite:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientTransparent };
				case FormatConditionRangeGradientPredefinedType.WhiteGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.RedWhite:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientTransparent };
				case FormatConditionRangeGradientPredefinedType.WhiteRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.GreenYellow:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientYellow };
				case FormatConditionRangeGradientPredefinedType.YellowGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.RedYellow:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientYellow };
				case FormatConditionRangeGradientPredefinedType.YellowRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.BlueWhite:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientTransparent };
				case FormatConditionRangeGradientPredefinedType.WhiteBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.RedBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueYellow:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientYellow };
				case FormatConditionRangeGradientPredefinedType.YellowBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.GreenBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.GreenWhiteBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueWhiteGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.RedWhiteBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueWhiteRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.GreenWhiteRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.RedWhiteGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientTransparent, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.GreenYellowRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.RedYellowGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientGreen };
				case FormatConditionRangeGradientPredefinedType.BlueYellowRed:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientRed };
				case FormatConditionRangeGradientPredefinedType.RedYellowBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientRed, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.GreenYellowBlue:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientGreen, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientBlue };
				case FormatConditionRangeGradientPredefinedType.BlueYellowGreen:
					return new FormatConditionAppearanceType[] { FormatConditionAppearanceType.GradientBlue, FormatConditionAppearanceType.GradientYellow, FormatConditionAppearanceType.GradientGreen };
				default:
					return null;
			}
		}
		public static IList<Color> ToColors(this FormatConditionRangeGradientPredefinedType gradientType, FormatConditionColorScheme scheme) {
			IList<FormatConditionAppearanceType> types = gradientType.ToAppearanceTypes();
			List<Color> colors = new List<Color>();
			foreach(FormatConditionAppearanceType type in types) {
				colors.Add(type.ToBackColor(scheme));
			}
			return colors;
		}
		public static Color ToBackColor(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			bool isDark = (scheme == FormatConditionColorScheme.Dark);
			Color color = type.BackAndGradientColorGroupsToBackColor(scheme);
			if(!color.IsEmpty)
				return color;
			return type.BackColorsWithFontGroupToBackColor(scheme);
		}
		public static IList<StyleSettingsBase> ToBarStyleSettings(this FormatConditionRangeGradientPredefinedType gradientType) {
			return gradientType.ToStyleSettings((type) => { return new BarStyleSettings(type); });
		}
		public static IList<StyleSettingsBase> ToAppearanceSettings(this FormatConditionRangeGradientPredefinedType gradientType) {
			return gradientType.ToStyleSettings((type) => { return new AppearanceSettings(type); });
		}
		public static AppearanceSettings ToAppearanceSettings(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			bool isDark = (scheme == FormatConditionColorScheme.Dark);
			switch(type) {
				case FormatConditionAppearanceType.FontBold:
					return new AppearanceSettings(FontStyle.Bold);
				case FormatConditionAppearanceType.FontItalic:
					return new AppearanceSettings(FontStyle.Italic);
				case FormatConditionAppearanceType.FontUnderline:
					return new AppearanceSettings(FontStyle.Underline);
				case FormatConditionAppearanceType.FontGrayed:
					return new AppearanceSettings(GrayedTextColor, FontStyle.Regular);
				case FormatConditionAppearanceType.FontRed:
					return new AppearanceSettings(isDark ? DarkRedColor : LightRedColor, FontStyle.Regular);
				case FormatConditionAppearanceType.FontYellow:
					return new AppearanceSettings(isDark ? DarkYellowColor : LightYellowColor, FontStyle.Regular);
				case FormatConditionAppearanceType.FontGreen:
					return new AppearanceSettings(isDark ? DarkGreenColor : LightGreenColor, FontStyle.Regular);
				case FormatConditionAppearanceType.FontBlue:
					return new AppearanceSettings(isDark ? DarkBlueColor : LightBlueColor, FontStyle.Regular);
				default:
					Color color = type.BackAndGradientColorGroupsToBackColor(scheme);
					if(!color.IsEmpty)
						return new AppearanceSettings(color);
					color = type.BackColorsWithFontGroupToBackColor(scheme);
					if(!color.IsEmpty)
						return new AppearanceSettings(color, WhiteColor);
					return null;
			}
		}
		internal static BarStyleSettings ToBarStyleSettings(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			return new BarStyleSettings(type.ToBackColor(scheme));
		}
		internal static StyleSettingsModel ToAppearanceSettingsModel(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			IStyleSettings styleSettings = type.ToAppearanceSettings(scheme);
			return styleSettings.CreateViewModel();
		}
		internal static StyleSettingsModel ToBarStyleSettingsModel(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			IStyleSettings styleSettings = type.ToBarStyleSettings(scheme);
			return styleSettings.CreateViewModel();
		}
		public static bool IsPredefined(this FormatConditionRangeSetPredefinedType type) {
			return type != FormatConditionRangeSetPredefinedType.None && type != FormatConditionRangeSetPredefinedType.Custom;
		}
		public static bool IsPredefined(this FormatConditionRangeGradientPredefinedType type) {
			return type != FormatConditionRangeGradientPredefinedType.None && type != FormatConditionRangeGradientPredefinedType.Custom;
		}
		static IList<StyleSettingsBase> ToStyleSettings(this FormatConditionRangeGradientPredefinedType gradientType, Func<FormatConditionAppearanceType, StyleSettingsBase> createStyleSettings) {
			IList<FormatConditionAppearanceType> types = gradientType.ToAppearanceTypes();
			if(types == null)
				return null;
			List<StyleSettingsBase> settings = new List<StyleSettingsBase>();
			foreach(FormatConditionAppearanceType type in types) {
				settings.Add(createStyleSettings(type));
			}
			return settings;
		}
		static Color BackAndGradientColorGroupsToBackColor(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			bool isDark = (scheme == FormatConditionColorScheme.Dark);
			switch(type) {
				case FormatConditionAppearanceType.PaleRed:
					return isDark ? DarkPaleRedColor : LightPaleRedColor;
				case FormatConditionAppearanceType.PaleYellow:
					return isDark ? DarkPaleYellowColor : LightPaleYellowColor;
				case FormatConditionAppearanceType.PaleGreen:
					return isDark ? DarkPaleGreenColor : LightPaleGreenColor;
				case FormatConditionAppearanceType.PaleBlue:
					return isDark ? DarkPaleBlueColor : LightPaleBlueColor;
				case FormatConditionAppearanceType.PalePurple:
					return isDark ? DarkPalePurpleColor : LightPalePurpleColor;
				case FormatConditionAppearanceType.PaleCyan:
					return isDark ? DarkPaleCyanColor : LightPaleCyanColor;
				case FormatConditionAppearanceType.PaleOrange:
					return isDark ? DarkPaleOrangeColor : LightPaleOrangeColor;
				case FormatConditionAppearanceType.PaleGray:
					return isDark ? DarkPaleGrayColor : LightPaleGrayColor;
				case FormatConditionAppearanceType.GradientRed:
					return isDark ? DarkGradientRedColor : LightGradientRedColor;
				case FormatConditionAppearanceType.GradientYellow:
					return isDark ? DarkGradientYellowColor : LightGradientYellowColor;
				case FormatConditionAppearanceType.GradientGreen:
					return isDark ? DarkGradientGreenColor : LightGradientGreenColor;
				case FormatConditionAppearanceType.GradientBlue:
					return isDark ? DarkGradientBlueColor : LightGradientBlueColor;
				case FormatConditionAppearanceType.GradientPurple:
					return isDark ? DarkGradientPurpleColor : LightGradientPurpleColor;
				case FormatConditionAppearanceType.GradientCyan:
					return isDark ? DarkGradientCyanColor : LightGradientCyanColor;
				case FormatConditionAppearanceType.GradientOrange:
					return isDark ? DarkGradientOrangeColor : LightGradientOrangeColor;
				case FormatConditionAppearanceType.GradientTransparent:
					return isDark ? DarkGradientTransparentColor : LightGradientTransparentColor;
				default:
					return Color.Empty;
			}
		}
		static Color BackColorsWithFontGroupToBackColor(this FormatConditionAppearanceType type, FormatConditionColorScheme scheme) {
			bool isDark = (scheme == FormatConditionColorScheme.Dark);
			switch(type) {
				case FormatConditionAppearanceType.Red:
					return isDark ? DarkRedColor : LightRedColor;
				case FormatConditionAppearanceType.Yellow:
					return isDark ? DarkYellowColor : LightYellowColor;
				case FormatConditionAppearanceType.Green:
					return isDark ? DarkGreenColor : LightGreenColor;
				case FormatConditionAppearanceType.Blue:
					return isDark ? DarkBlueColor : LightBlueColor;
				case FormatConditionAppearanceType.Purple:
					return isDark ? DarkPurpleColor : LightPurpleColor;
				case FormatConditionAppearanceType.Cyan:
					return isDark ? DarkCyanColor : LightCyanColor;
				case FormatConditionAppearanceType.Orange:
					return isDark ? DarkOrangeColor : LightOrangeColor;
				case FormatConditionAppearanceType.Gray:
					return isDark ? DarkGrayColor : LightGrayColor;
				default:
					return Color.Empty;
			}
		}
		internal static IList<FormatConditionAppearanceType> ToAppearanceTypes(this FormatConditionAppearanceTypeGroups appearanceGroup) {
			if(appearanceGroup == FormatConditionAppearanceTypeGroups.Fonts)
				return new FormatConditionAppearanceType[] {
						FormatConditionAppearanceType.FontBold,
						FormatConditionAppearanceType.FontItalic,
						FormatConditionAppearanceType.FontUnderline,
						FormatConditionAppearanceType.FontGrayed,
						FormatConditionAppearanceType.FontRed,
						FormatConditionAppearanceType.FontYellow,
						FormatConditionAppearanceType.FontGreen,
						FormatConditionAppearanceType.FontBlue
					};
			return appearanceGroup.ToBarStyleTypes();
		}
		internal static IList<FormatConditionAppearanceType> ToBarStyleTypes(this FormatConditionAppearanceTypeGroups appearanceGroup) {
			switch(appearanceGroup) {
				case FormatConditionAppearanceTypeGroups.BackColors:
					return new FormatConditionAppearanceType[] {
						FormatConditionAppearanceType.PaleRed,
						FormatConditionAppearanceType.PaleYellow,
						FormatConditionAppearanceType.PaleGreen,
						FormatConditionAppearanceType.PaleBlue,
						FormatConditionAppearanceType.PalePurple,
						FormatConditionAppearanceType.PaleCyan,
						FormatConditionAppearanceType.PaleOrange,
						FormatConditionAppearanceType.PaleGray
					};
				case FormatConditionAppearanceTypeGroups.BackColorsWithFont:
					return new FormatConditionAppearanceType[] {
						FormatConditionAppearanceType.Red,
						FormatConditionAppearanceType.Yellow,
						FormatConditionAppearanceType.Green,
						FormatConditionAppearanceType.Blue,
						FormatConditionAppearanceType.Purple,
						FormatConditionAppearanceType.Cyan,
						FormatConditionAppearanceType.Orange,
						FormatConditionAppearanceType.Gray
					};
				case FormatConditionAppearanceTypeGroups.GradientColors:
					return new FormatConditionAppearanceType[] {
						FormatConditionAppearanceType.GradientRed,
						FormatConditionAppearanceType.GradientYellow,
						FormatConditionAppearanceType.GradientGreen,
						FormatConditionAppearanceType.GradientBlue,
						FormatConditionAppearanceType.GradientPurple,
						FormatConditionAppearanceType.GradientCyan,
						FormatConditionAppearanceType.GradientOrange,
						FormatConditionAppearanceType.GradientTransparent
					};
				case FormatConditionAppearanceTypeGroups.Fonts:
					return null;
				default:
					throw new ArgumentException("Undefined group type");
			}
		}
		internal static IList<FormatConditionIconType> ToIconTypes(this FormatConditionIconGroups iconsGroup) {
			switch(iconsGroup) {
				case FormatConditionIconGroups.Directional:
					return new FormatConditionIconType[] { 
						FormatConditionIconType.DirectionalGreenArrowUp, 
						FormatConditionIconType.DirectionalYellowUpInclineArrow, 
						FormatConditionIconType.DirectionalYellowSideArrow, 
						FormatConditionIconType.DirectionalYellowDownInclineArrow, 
						FormatConditionIconType.DirectionalRedDownArrow,		
						FormatConditionIconType.DirectionalGrayArrowUp, 
						FormatConditionIconType.DirectionalGrayUpInclineArrow, 
						FormatConditionIconType.DirectionalGraySideArrow, 
						FormatConditionIconType.DirectionalGrayDownInclineArrow, 
						FormatConditionIconType.DirectionalGrayDownArrow,		
						FormatConditionIconType.DirectionalYellowDash, 
						FormatConditionIconType.DirectionalRedTriangleDown, 
						FormatConditionIconType.DirectionalGreenTriangleUp
					};
				case FormatConditionIconGroups.RatingsMonochrome:
					return new FormatConditionIconType[] { 
						FormatConditionIconType.RatingFullGrayCircle, 
						FormatConditionIconType.Rating1QuarterGrayCircle, 
						FormatConditionIconType.Rating2QuartersGrayCircle, 
						FormatConditionIconType.Rating3QuartersGrayCircle, 
						FormatConditionIconType.RatingEmptyGrayCircle,
						FormatConditionIconType.RatingFullGrayStar, 
						FormatConditionIconType.RatingHalfGrayStar, 
						FormatConditionIconType.RatingEmptyGrayStar
					};
				case FormatConditionIconGroups.RatingsColor:
					return new FormatConditionIconType[] { 
						FormatConditionIconType.Rating4Bars,
						FormatConditionIconType.Rating3Bars,
						FormatConditionIconType.Rating2Bars,
						FormatConditionIconType.Rating1Bar,
						FormatConditionIconType.Rating0Bars,
						FormatConditionIconType.Rating4FilledBoxes, 
						FormatConditionIconType.Rating3FilledBoxes, 
						FormatConditionIconType.Rating2FilledBoxes, 
						FormatConditionIconType.Rating1FilledBox, 
						FormatConditionIconType.Rating0FilledBoxes
					};
				case FormatConditionIconGroups.Indicators:
					return new FormatConditionIconType[] { 
						FormatConditionIconType.IndicatorGreenCheck, 
						FormatConditionIconType.IndicatorYellowExclamation, 
						FormatConditionIconType.IndicatorRedCross,
						FormatConditionIconType.IndicatorCircledGreenCheck, 
						FormatConditionIconType.IndicatorCircledYellowExclamation, 
						FormatConditionIconType.IndicatorCircledRedCross,
						FormatConditionIconType.ShapeYellowTriangle, 
						FormatConditionIconType.ShapeRedDiamond
					};
				case FormatConditionIconGroups.Flags:
					return new FormatConditionIconType[] {
						FormatConditionIconType.IndicatorGreenFlag, 
						FormatConditionIconType.IndicatorYellowFlag, 
						FormatConditionIconType.IndicatorRedFlag
					};
				case FormatConditionIconGroups.Shapes:
					return new FormatConditionIconType[] { 
						FormatConditionIconType.ShapeGreenTrafficLight, 
						FormatConditionIconType.ShapeYellowTrafficLight, 
						FormatConditionIconType.ShapeRedTrafficLight,
						FormatConditionIconType.ShapeGreenCircle, 
						FormatConditionIconType.ShapeYellowCircle, 
						FormatConditionIconType.ShapeRedCircle, 
						FormatConditionIconType.ShapeLightRedCircle, 
						FormatConditionIconType.ShapeLightGrayCircle
					};
				default:
					throw new ArgumentException("Undefined group type");
			}
		}
		internal static IList<FormatConditionRangeSetPredefinedType> ToRangeIconTypes(this FormatConditionRangeSetTypeGroups iconsGroup) {
			switch(iconsGroup) {
				case FormatConditionRangeSetTypeGroups.Ranges2:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.Arrows2,
						FormatConditionRangeSetPredefinedType.ArrowsGray2,
						FormatConditionRangeSetPredefinedType.Circles2,
						FormatConditionRangeSetPredefinedType.Symbols2,
						FormatConditionRangeSetPredefinedType.SymbolsCircled2
					};
				case FormatConditionRangeSetTypeGroups.Ranges3:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.Arrows3,
						FormatConditionRangeSetPredefinedType.ArrowsGray3,
						FormatConditionRangeSetPredefinedType.PositiveNegative3,
						FormatConditionRangeSetPredefinedType.Circles3,
						FormatConditionRangeSetPredefinedType.TrafficLights3,
						FormatConditionRangeSetPredefinedType.Signs3,
						FormatConditionRangeSetPredefinedType.Symbols3,
						FormatConditionRangeSetPredefinedType.SymbolsCircled3,
						FormatConditionRangeSetPredefinedType.Stars3,
						FormatConditionRangeSetPredefinedType.Flags3
					};
				case FormatConditionRangeSetTypeGroups.Ranges4:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.Arrows4,
						FormatConditionRangeSetPredefinedType.ArrowsGray4,
						FormatConditionRangeSetPredefinedType.Circles4,
						FormatConditionRangeSetPredefinedType.CirclesRedToBlack4,
						FormatConditionRangeSetPredefinedType.Bars4
					};
				case FormatConditionRangeSetTypeGroups.Ranges5:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.Arrows5,
						FormatConditionRangeSetPredefinedType.ArrowsGray5,
						FormatConditionRangeSetPredefinedType.Quarters5,
						FormatConditionRangeSetPredefinedType.Bars5,
						FormatConditionRangeSetPredefinedType.Boxes5
					};
				default:
					throw new ArgumentException("Undefined group type");
			}
		}
		internal static IList<FormatConditionRangeSetPredefinedType> ToRangeColorTypes(this FormatConditionRangeSetTypeGroups iconsGroup) {
			switch(iconsGroup) {
				case FormatConditionRangeSetTypeGroups.Ranges2:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.ColorsPaleRedGreen, FormatConditionRangeSetPredefinedType.ColorsRedGreen
					};
				case FormatConditionRangeSetTypeGroups.Ranges3:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.ColorsPaleRedGreenBlue, FormatConditionRangeSetPredefinedType.ColorsRedGreenBlue
					};
				case FormatConditionRangeSetTypeGroups.Ranges4:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.ColorsPaleRedYellowGreenBlue, FormatConditionRangeSetPredefinedType.ColorsRedYellowGreenBlue
					};
				case FormatConditionRangeSetTypeGroups.Ranges5:
					return new FormatConditionRangeSetPredefinedType[] {
						FormatConditionRangeSetPredefinedType.ColorsPaleRedOrangeYellowGreenBlue, FormatConditionRangeSetPredefinedType.ColorsRedOrangeYellowGreenBlue
					};
				default:
					throw new ArgumentException("Undefined group type");
			}
		}
		internal static IList<FormatConditionRangeGradientPredefinedType> ToRangeGradientTypes(this FormatConditionRangeGradientTypeGroups gradientGroup) {
			switch(gradientGroup) {
				case FormatConditionRangeGradientTypeGroups.TwoColors:
					return new FormatConditionRangeGradientPredefinedType[] {
						FormatConditionRangeGradientPredefinedType.GreenWhite,
						FormatConditionRangeGradientPredefinedType.WhiteGreen,
						FormatConditionRangeGradientPredefinedType.RedWhite,
						FormatConditionRangeGradientPredefinedType.WhiteRed,
						FormatConditionRangeGradientPredefinedType.GreenYellow,
						FormatConditionRangeGradientPredefinedType.YellowGreen,
						FormatConditionRangeGradientPredefinedType.RedYellow,
						FormatConditionRangeGradientPredefinedType.YellowRed,
						FormatConditionRangeGradientPredefinedType.BlueWhite,
						FormatConditionRangeGradientPredefinedType.WhiteBlue,
						FormatConditionRangeGradientPredefinedType.BlueRed,
						FormatConditionRangeGradientPredefinedType.RedBlue,
						FormatConditionRangeGradientPredefinedType.YellowBlue,
						FormatConditionRangeGradientPredefinedType.BlueYellow,
						FormatConditionRangeGradientPredefinedType.GreenBlue,
						FormatConditionRangeGradientPredefinedType.BlueGreen
					};
				case FormatConditionRangeGradientTypeGroups.ThreeColors:
					return new FormatConditionRangeGradientPredefinedType[] {
						FormatConditionRangeGradientPredefinedType.GreenWhiteBlue,
						FormatConditionRangeGradientPredefinedType.BlueWhiteGreen,
						FormatConditionRangeGradientPredefinedType.BlueWhiteRed,
						FormatConditionRangeGradientPredefinedType.RedWhiteBlue,
						FormatConditionRangeGradientPredefinedType.GreenWhiteRed,
						FormatConditionRangeGradientPredefinedType.RedWhiteGreen,
						FormatConditionRangeGradientPredefinedType.GreenYellowRed,
						FormatConditionRangeGradientPredefinedType.RedYellowGreen,
						FormatConditionRangeGradientPredefinedType.BlueYellowRed,
						FormatConditionRangeGradientPredefinedType.RedYellowBlue,
						FormatConditionRangeGradientPredefinedType.GreenYellowBlue,
						FormatConditionRangeGradientPredefinedType.BlueYellowGreen
					};
				default:
					throw new ArgumentException("Undefined group type");
			}
		}
	}
}
