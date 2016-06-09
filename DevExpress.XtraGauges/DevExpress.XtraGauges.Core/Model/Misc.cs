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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Model {
	public enum DigitalGaugeDisplayMode { SevenSegment, FourteenSegment, Matrix5x8, Matrix8x14 }
	public enum ScaleOrientation { Horizontal, Vertical }
	public enum LabelOrientation { Default, Radial, Tangent, LeftToRight, BottomToTop, TopToBottom }
	public enum DefaultShapeLoaderType { XAMLLoader, SHPLoader }
	public enum DigitShapeType { Segment7, Segment14, Matrix5x8, Matrix8x14 }
	public enum TickmarkShapeType {
		Default, Line, Box, Diamond,
		Circular_Default1, Circular_Default2, Circular_Default3, Circular_Default4,
		Circular_Style1_1, Circular_Style1_2, Circular_Style1_3, Circular_Style1_4,
		Circular_Style2_1, Circular_Style2_2,
		Circular_Style3_1, Circular_Style3_2, Circular_Style3_3, Circular_Style3_4,
		Circular_Style4_1, Circular_Style4_2, Circular_Style4_3, Circular_Style4_4,
		Circular_Style5_1, Circular_Style5_2, Circular_Style5_3, Circular_Style5_4, Circular_Style5_5,
		Circular_Style6_1, Circular_Style6_2, Circular_Style6_3, Circular_Style6_4, Circular_Style6_5,
		Circular_Style7_1, Circular_Style7_2,
		Circular_Style8_1, Circular_Style8_2, Circular_Style8_3, Circular_Style8_4, Circular_Style8_5,
		Circular_Style9_1, Circular_Style9_2,
		Circular_Style10_1, Circular_Style10_2, Circular_Style10_3, Circular_Style10_4,
		Circular_Style11_1, Circular_Style11_2, Circular_Style11_3, Circular_Style11_4,
		Circular_Style12_1, Circular_Style12_2, Circular_Style12_3, Circular_Style12_4,
		Circular_Style13_1, Circular_Style13_2, Circular_Style13_3, Circular_Style13_4, Circular_Style13_5,
		Circular_Style14_1, Circular_Style14_2, Circular_Style14_3, Circular_Style14_4, Circular_Style14_5,
		Circular_Style15_1, Circular_Style15_2, Circular_Style15_3, Circular_Style15_4,
		Circular_Style16_1, Circular_Style16_2,
		Circular_Style17_1, Circular_Style17_2,
		Circular_Style18_1, Circular_Style18_2,
		Circular_Style19_1, Circular_Style19_2,
		Circular_Style20_1, Circular_Style20_2,
		Circular_Style21_1, Circular_Style21_2,
		Circular_Style22_1, Circular_Style22_2,
		Circular_Style23_1, Circular_Style23_2,
		Circular_Style24_1, Circular_Style24_2,
		Circular_Style25_1, Circular_Style25_2,
		Circular_Style26_1, Circular_Style26_2,
		Circular_Style27_1,
		Circular_Style28_1,
		Linear_Default1, Linear_Default2,
		Linear_Style1_1, Linear_Style1_2,
		Linear_Style2_1, Linear_Style2_2,
		Linear_Style3_1, Linear_Style3_2,
		Linear_Style4_1, Linear_Style4_2,
		Linear_Style5_1, Linear_Style5_2, Linear_Style5_3,
		Linear_Style6_1, Linear_Style6_2, Linear_Style6_3,
		Linear_Style7_1, Linear_Style7_2, Linear_Style7_3,
		Linear_Style8_1, Linear_Style8_2, Linear_Style8_3,
		Linear_Style9_1, Linear_Style9_2, Linear_Style9_3,
		Linear_Style10_1, Linear_Style10_2, Linear_Style10_3,
		Linear_Style11_1, Linear_Style11_2, Linear_Style11_3, Linear_Style11_4,
		Linear_Style12_1, Linear_Style12_2,
		Linear_Style13_1, Linear_Style13_2,
		Linear_Style14_1, Linear_Style14_2,
		Linear_Style15_1, Linear_Style15_2,
		Linear_Style16_1, Linear_Style16_2,
		Linear_Style17_1, Linear_Style17_2,
		Linear_Style18_1, Linear_Style18_2,
		Linear_Style19_1, Linear_Style19_2,
		Linear_Style20_1, Linear_Style20_2,
		Linear_Style21_1, Linear_Style21_2,
		Linear_Style22_1, Linear_Style22_2,
		Linear_Style23_1, Linear_Style23_2,
		Linear_Style24_1, Linear_Style24_2,
		Linear_Style25_1, Linear_Style25_2,
		Linear_Style26_1, Linear_Style26_2
	}
	public enum NeedleShapeType {
		Circular_Default,
		CircularFull_Style1,
		CircularFull_Style2,
		CircularFull_Style3,
		CircularFull_Style4,
		CircularFull_Style5,
		CircularFull_Style6,
		CircularFull_Style7,
		CircularFull_Style8,
		CircularFull_Style9,
		CircularFull_Style10,
		CircularFull_Style11,
		CircularFull_Style12,
		CircularFull_Style13,
		CircularFull_Style14,
		CircularFull_Style15,
		CircularFull_Style16,
		CircularFull_Style17,
		CircularFull_Style18,
		CircularFull_Style19,
		CircularFull_Style20,
		CircularFull_Style21,
		CircularFull_Style22,
		CircularFull_Style23,
		CircularFull_Style24,
		CircularFull_Style25,
		CircularFull_Style26,
		CircularFull_Style27,
		CircularFull_Style28,
		CircularFull_Clock,
		CircularFull_ClockHour,
		CircularFull_ClockMinute,
		CircularFull_ClockSecond,
		CircularHalf_Style23,
		CircularWide_Style1,
		CircularWide_Style2,
		CircularWide_Style3,
		CircularWide_Style4,
		CircularWide_Style5,
		CircularWide_Style6,
		CircularWide_Style7,
		CircularWide_Style8,
		CircularWide_Style9,
		CircularWide_Style10,
		CircularWide_Style11,
		CircularWide_Style12,
		CircularWide_Style13,
		CircularWide_Style14,
		CircularWide_Style15
	}
	public enum EffectLayerShapeType {
		Circular_Default,
		CircularFull_Style6,
		CircularFull_Style7,
		CircularFull_Style8,
		CircularFull_Style9,
		CircularFull_Clock,
		CircularQuarter_Style6Left,
		CircularQuarter_Style7Left,
		CircularQuarter_Style8Left,
		CircularQuarter_Style9Left,
		CircularQuarter_Style6Right,
		CircularQuarter_Style7Right,
		CircularQuarter_Style8Right,
		CircularQuarter_Style9Right,
		CircularThreeFourth_Style6,
		CircularThreeFourth_Style7,
		CircularThreeFourth_Style8,
		CircularThreeFourth_Style9,
		Linear_Style6,
		Linear_Style8,
		CircularWide_Style7,
		CircularWide_Style8,
		CircularWide_Style9,
		Empty
	}
	public enum BackgroundLayerShapeType {
		Linear_Default,
		Linear_Style1,
		Linear_Style2,
		Linear_Style3,
		Linear_Style4,
		Linear_Style5,
		Linear_Style6,
		Linear_Style7,
		Linear_Style8,
		Linear_Style9,
		Linear_Style10,
		Linear_Style11,
		Linear_Style12,
		Linear_Style13,
		Linear_Style14,
		Linear_Style15,
		Linear_Style16,
		Linear_Style17,
		Linear_Style18,
		Linear_Style19,
		Linear_Style20,
		Linear_Style21,
		Linear_Style22,
		Linear_Style23,
		Linear_Style24,
		Linear_Style25,
		Linear_Style26,
		CircularFull_Default,
		CircularFull_Style1,
		CircularFull_Style2,
		CircularFull_Style3,
		CircularFull_Style4, CircularFull_Style4_1,
		CircularFull_Style5, CircularFull_Style5_1,
		CircularFull_Style6,
		CircularFull_Style7,
		CircularFull_Style8,
		CircularFull_Style9,
		CircularFull_Style10,
		CircularFull_Style11,
		CircularFull_Style12,
		CircularFull_Style13,
		CircularFull_Style14,
		CircularFull_Style15,
		CircularFull_Style16,
		CircularFull_Style17,
		CircularFull_Style18,
		CircularFull_Style19,
		CircularFull_Style20,
		CircularFull_Style21,
		CircularFull_Style22,
		CircularFull_Style23,
		CircularFull_Style24,
		CircularFull_Style25,
		CircularFull_Style26,
		CircularFull_Style27,
		CircularFull_Style28,
		CircularFull_Clock,
		CircularFull_WorldTimeClock,
		CircularFull_SysInfoBack,
		CircularHalf_Default,
		CircularHalf_Style1,
		CircularHalf_Style2,
		CircularHalf_Style3,
		CircularHalf_Style4,
		CircularHalf_Style5,
		CircularHalf_Style6,
		CircularHalf_Style7,
		CircularHalf_Style8,
		CircularHalf_Style9,
		CircularHalf_Style10,
		CircularHalf_Style11,
		CircularHalf_Style12,
		CircularHalf_Style13,
		CircularHalf_Style14,
		CircularHalf_Style15,
		CircularHalf_Style16,
		CircularHalf_Style17,
		CircularHalf_Style18,
		CircularHalf_Style19,
		CircularHalf_Style20,
		CircularHalf_Style21,
		CircularHalf_Style22,
		CircularHalf_Style23,
		CircularHalf_Style24,
		CircularHalf_Style25,
		CircularHalf_Style26,
		CircularHalf_Style27,
		CircularHalf_Style28,
		CircularHalf_CarPanelBack,
		CircularQuarter_DefaultLeft,
		CircularQuarter_Style1Left,
		CircularQuarter_Style2Left,
		CircularQuarter_Style3Left,
		CircularQuarter_Style4Left,
		CircularQuarter_Style5Left,
		CircularQuarter_Style6Left,
		CircularQuarter_Style7Left,
		CircularQuarter_Style8Left,
		CircularQuarter_Style9Left,
		CircularQuarter_Style10Left,
		CircularQuarter_Style11Left,
		CircularQuarter_Style12Left,
		CircularQuarter_Style13Left,
		CircularQuarter_Style14Left,
		CircularQuarter_Style15Left,
		CircularQuarter_Style16Left,
		CircularQuarter_Style17Left,
		CircularQuarter_Style18Left,
		CircularQuarter_Style19Left,
		CircularQuarter_Style20Left,
		CircularQuarter_Style21Left,
		CircularQuarter_Style22Left,
		CircularQuarter_Style23Left,
		CircularQuarter_Style24Left,
		CircularQuarter_Style25Left,
		CircularQuarter_Style26Left,
		CircularQuarter_Style27Left,
		CircularQuarter_Style28Left,
		CircularQuarter_DefaultRight,
		CircularQuarter_Style1Right,
		CircularQuarter_Style2Right,
		CircularQuarter_Style3Right,
		CircularQuarter_Style4Right,
		CircularQuarter_Style5Right,
		CircularQuarter_Style6Right,
		CircularQuarter_Style7Right,
		CircularQuarter_Style8Right,
		CircularQuarter_Style9Right,
		CircularQuarter_Style10Right,
		CircularQuarter_Style11Right,
		CircularQuarter_Style12Right,
		CircularQuarter_Style13Right,
		CircularQuarter_Style14Right,
		CircularQuarter_Style15Right,
		CircularQuarter_Style16Right,
		CircularQuarter_Style17Right,
		CircularQuarter_Style18Right,
		CircularQuarter_Style19Right,
		CircularQuarter_Style20Right,
		CircularQuarter_Style21Right,
		CircularQuarter_Style22Right,
		CircularQuarter_Style23Right,
		CircularQuarter_Style24Right,
		CircularQuarter_Style25Right,
		CircularQuarter_Style26Right,
		CircularQuarter_Style27Right,
		CircularQuarter_Style28Right,
		CircularThreeFourth_Default,
		CircularThreeFourth_Style1,
		CircularThreeFourth_Style2,
		CircularThreeFourth_Style3,
		CircularThreeFourth_Style4,
		CircularThreeFourth_Style5,
		CircularThreeFourth_Style6,
		CircularThreeFourth_Style7,
		CircularThreeFourth_Style8,
		CircularThreeFourth_Style9,
		CircularThreeFourth_Style10,
		CircularThreeFourth_Style11,
		CircularThreeFourth_Style12,
		CircularThreeFourth_Style13,
		CircularThreeFourth_Style14,
		CircularThreeFourth_Style15,
		CircularThreeFourth_Style16,
		CircularThreeFourth_Style17,
		CircularThreeFourth_Style18,
		CircularThreeFourth_Style19,
		CircularThreeFourth_Style20,
		CircularThreeFourth_Style21,
		CircularThreeFourth_Style22,
		CircularThreeFourth_Style23,
		CircularThreeFourth_Style24,
		CircularThreeFourth_Style25,
		CircularThreeFourth_Style26,
		CircularThreeFourth_Style27,
		CircularThreeFourth_Style28,
		CircularWide_Style1, CircularWide_Style1_1,
		CircularWide_Style2, CircularWide_Style2_1,
		CircularWide_Style3, CircularWide_Style3_1,
		CircularWide_Style4, CircularWide_Style4_1,
		CircularWide_Style5,
		CircularWide_Style6, CircularWide_Style6_1,
		CircularWide_Style7, CircularWide_Style7_1,
		CircularWide_Style8, CircularWide_Style8_1,
		CircularWide_Style9, CircularWide_Style9_1,
		CircularWide_Style10,
		CircularWide_Style11,
		CircularWide_Style12, CircularWide_Style12_1,
		CircularWide_Style13, CircularWide_Style13_1,
		CircularWide_Style14, CircularWide_Style14_1,
		CircularWide_Style15
	}
	public enum LevelShapeSetType {
		Default,
		Style1,
		Style2,
		Style3,
		Style4,
		Style5,
		Style6,
		Style7,
		Style8,
		Style9,
		Style10,
		Style11,
		Style12,
		Style13,
		Style14,
		Style15,
		Style16,
		Style17,
		Style18,
		Style19,
		Style20,
		Style21,
		Style22,
		Style23,
		Style24,
		Style25,
		Style26
	}
	public enum DigitalBackgroundShapeSetType {
		Default,
		Style1,
		Style2,
		Style3,
		Style4,
		Style5,
		Style6,
		Style7,
		Style8,
		Style9,
		Style10,
		Style11,
		Style12,
		Style13,
		Style14,
		Style15,
		Style16,
		Style17,
		Style18,
		Style19,
		Style20,
		Style21,
		Style22,
		Style23,
		Style24,
		Style25,
		Style26
	}
	public enum DigitalEffectShapeType {
		Default,
		Style6,
		Style9,
		Empty
	}
	public enum ScaleLevelShapeType {
		BarStart = 0,
		BarPacked = 1,
		BarEmpty = 2,
		BarEnd = 3
	}
	public enum DigitalBackgroundShapeType {
		BgNear = 0,
		BgCenter = 1,
		BgFar = 2
	}
	public enum SpindleCapShapeType {
		CircularFull_Default,
		CircularFull_Style2,
		CircularFull_Style3,
		CircularFull_Style4,
		CircularFull_Style6,
		CircularFull_Style8,
		CircularFull_Style9,
		CircularFull_Style16,
		CircularFull_Style19,
		CircularFull_Style20,
		CircularFull_Style22,
		CircularFull_Style25,
		CircularFull_Style26,
		CircularFull_Clock,
		Empty
	}
	public enum MarkerPointerShapeType {
		Default, TriangleLeft, SliderLeft, Circle, WedgeLeft, Diamond, ArrowLeft, Box, Star, Button, SnowFlake, TriangleRight, SliderRight, WedgeRight, ArrowRight
	}
	public enum StateIndicatorShapeType {
		Default,
		ElectricLight1, ElectricLight2, ElectricLight3, ElectricLight4,
		Arrow1, Arrow2, Arrow3, Arrow4, Arrow5, Arrow6, Arrow7, Arrow8, Arrow9, Arrow10,
		Smile1, Smile2, Smile3, Smile4, Smile5,
		TrafficLight1, TrafficLight2, TrafficLight3, TrafficLight4,
		FlagUSA, FlagChina, FlagJapan, FlagIndia, FlagGermany, FlagUK, FlagRussia, FlagFrance, FlagBrazil, FlagItaly, FlagSpain, FlagCanada, FlagSouthKorea, FlagIran, FlagIndonesia, FlagAustralia, FlagTaiwan, FlagTurkey, FlagNetherlands,
		Currency, CurrencyUSD, CurrencyCent, CurrencyEUR, CurrencyUAH, CurrencyGBP, CurrencyJPY, CurrencyITL, CurrencyESP, CurrencyFRF,
		CurrencyNLG, CurrencyINR, CurrencyBRR, CurrencyILS, CurrencyMNT, CurrencyKRW, CurrencyCRC, CurrencyNGN, CurrencyLAK, CurrencyRUR,
		Equalizer0, Equalizer1, Equalizer2, Equalizer3, Equalizer4,
		CarBattery1, CarBattery2, CarBattery3, CarBattery4,
		CarABS1, CarABS2, CarABS3, CarABS4,
		CarEngine1, CarEngine2, CarEngine3, CarEngine4,
		CarParkingLights1, CarParkingLights2, CarParkingLights3, CarParkingLights4,
		CarHiBeam1, CarHiBeam2, CarHiBeam3, CarHiBeam4,
		CarLowBeam1, CarLowBeam2, CarLowBeam3, CarLowBeam4,
		CarAirBag1, CarAirBag2, CarAirBag3, CarAirBag4,
		CarWater1, CarWater2, CarWater3, CarWater4,
		CarGas1, CarGas2, CarGas3, CarGas4,
		CarMotorOil1, CarMotorOil2, CarMotorOil3, CarMotorOil4,
		DashboardTrendArrowUp, DashboardTrendArrowDown, DashboardWarning,
		ProgressItem0, ProgressItem1, ProgressItem2, ProgressItem3, ProgressItem4,
		ProgressItem5, ProgressItem6, ProgressItem7, ProgressItem8, ProgressItem9,
		ProgressItem10, ProgressItem11, ProgressItem12, ProgressItem13, ProgressItem14
	}
	public static class Currency {
		public static StateIndicatorShapeType General { get { return StateIndicatorShapeType.Currency; } }
		public static StateIndicatorShapeType USD { get { return StateIndicatorShapeType.CurrencyUSD; } }
		public static StateIndicatorShapeType Cent { get { return StateIndicatorShapeType.CurrencyCent; } }
		public static StateIndicatorShapeType EUR { get { return StateIndicatorShapeType.CurrencyEUR; } }
		public static StateIndicatorShapeType UAH { get { return StateIndicatorShapeType.CurrencyUAH; } }
		public static StateIndicatorShapeType GBP { get { return StateIndicatorShapeType.CurrencyGBP; } }
		public static StateIndicatorShapeType JPY { get { return StateIndicatorShapeType.CurrencyJPY; } }
		public static StateIndicatorShapeType ITL { get { return StateIndicatorShapeType.CurrencyITL; } }
		public static StateIndicatorShapeType ESP { get { return StateIndicatorShapeType.CurrencyESP; } }
		public static StateIndicatorShapeType FRF { get { return StateIndicatorShapeType.CurrencyFRF; } }
		public static StateIndicatorShapeType NLG { get { return StateIndicatorShapeType.CurrencyNLG; } }
		public static StateIndicatorShapeType INR { get { return StateIndicatorShapeType.CurrencyINR; } }
		public static StateIndicatorShapeType BRR { get { return StateIndicatorShapeType.CurrencyBRR; } }
		public static StateIndicatorShapeType ILS { get { return StateIndicatorShapeType.CurrencyILS; } }
		public static StateIndicatorShapeType MNT { get { return StateIndicatorShapeType.CurrencyMNT; } }
		public static StateIndicatorShapeType KRW { get { return StateIndicatorShapeType.CurrencyKRW; } }
		public static StateIndicatorShapeType CRC { get { return StateIndicatorShapeType.CurrencyCRC; } }
		public static StateIndicatorShapeType NGN { get { return StateIndicatorShapeType.CurrencyNGN; } }
		public static StateIndicatorShapeType LAK { get { return StateIndicatorShapeType.CurrencyLAK; } }
		public static StateIndicatorShapeType RUR { get { return StateIndicatorShapeType.CurrencyRUR; } }
	}
	public static class Flags {
		public static StateIndicatorShapeType USA { get { return StateIndicatorShapeType.FlagUSA; } }
		public static StateIndicatorShapeType China { get { return StateIndicatorShapeType.FlagChina; } }
		public static StateIndicatorShapeType Japan { get { return StateIndicatorShapeType.FlagJapan; } }
		public static StateIndicatorShapeType India { get { return StateIndicatorShapeType.FlagIndia; } }
		public static StateIndicatorShapeType Germany { get { return StateIndicatorShapeType.FlagGermany; } }
		public static StateIndicatorShapeType UK { get { return StateIndicatorShapeType.FlagUK; } }
		public static StateIndicatorShapeType Russia { get { return StateIndicatorShapeType.FlagRussia; } }
		public static StateIndicatorShapeType France { get { return StateIndicatorShapeType.FlagFrance; } }
		public static StateIndicatorShapeType Brazil { get { return StateIndicatorShapeType.FlagBrazil; } }
		public static StateIndicatorShapeType Italy { get { return StateIndicatorShapeType.FlagItaly; } }
		public static StateIndicatorShapeType Spain { get { return StateIndicatorShapeType.FlagSpain; } }
		public static StateIndicatorShapeType Canada { get { return StateIndicatorShapeType.FlagCanada; } }
		public static StateIndicatorShapeType SouthKorea { get { return StateIndicatorShapeType.FlagSouthKorea; } }
		public static StateIndicatorShapeType Iran { get { return StateIndicatorShapeType.FlagIran; } }
		public static StateIndicatorShapeType Indonesia { get { return StateIndicatorShapeType.FlagIndonesia; } }
		public static StateIndicatorShapeType Australia { get { return StateIndicatorShapeType.FlagAustralia; } }
		public static StateIndicatorShapeType Taiwan { get { return StateIndicatorShapeType.FlagTaiwan; } }
		public static StateIndicatorShapeType Turkey { get { return StateIndicatorShapeType.FlagTurkey; } }
		public static StateIndicatorShapeType Netherlands { get { return StateIndicatorShapeType.FlagNetherlands; } }
	}
	public static class EqualizerShapes {
		public static StateIndicatorShapeType EmptyBar { get { return StateIndicatorShapeType.Equalizer0; } }
		public static StateIndicatorShapeType GrayBar { get { return StateIndicatorShapeType.Equalizer1; } }
		public static StateIndicatorShapeType RedBar { get { return StateIndicatorShapeType.Equalizer2; } }
		public static StateIndicatorShapeType AmberBar { get { return StateIndicatorShapeType.Equalizer3; } }
		public static StateIndicatorShapeType GreenBar { get { return StateIndicatorShapeType.Equalizer4; } }
	}
	public static class ProgressItemShapes {
		public static StateIndicatorShapeType GrayBar { get { return StateIndicatorShapeType.ProgressItem0; } }
		public static StateIndicatorShapeType RedBar { get { return StateIndicatorShapeType.ProgressItem1; } }
		public static StateIndicatorShapeType OrangeBar { get { return StateIndicatorShapeType.ProgressItem2; } }
		public static StateIndicatorShapeType LimeBar { get { return StateIndicatorShapeType.ProgressItem3; } }
		public static StateIndicatorShapeType BlueBar { get { return StateIndicatorShapeType.ProgressItem4; } }
		public static StateIndicatorShapeType GrayCircle { get { return StateIndicatorShapeType.ProgressItem5; } }
		public static StateIndicatorShapeType RedCircle { get { return StateIndicatorShapeType.ProgressItem6; } }
		public static StateIndicatorShapeType OrangeCircle { get { return StateIndicatorShapeType.ProgressItem7; } }
		public static StateIndicatorShapeType LimeCircle { get { return StateIndicatorShapeType.ProgressItem8; } }
		public static StateIndicatorShapeType BlueCircle { get { return StateIndicatorShapeType.ProgressItem9; } }
		public static StateIndicatorShapeType GrayStar { get { return StateIndicatorShapeType.ProgressItem10; } }
		public static StateIndicatorShapeType RedStar { get { return StateIndicatorShapeType.ProgressItem11; } }
		public static StateIndicatorShapeType OrangeStar { get { return StateIndicatorShapeType.ProgressItem12; } }
		public static StateIndicatorShapeType LimeStar { get { return StateIndicatorShapeType.ProgressItem13; } }
		public static StateIndicatorShapeType BlueStar { get { return StateIndicatorShapeType.ProgressItem14; } }
	}
	public static class CarIcons {
		public static StateIndicatorShapeType BatteryRed { get { return StateIndicatorShapeType.CarBattery1; } }
		public static StateIndicatorShapeType BatteryOrange { get { return StateIndicatorShapeType.CarBattery2; } }
		public static StateIndicatorShapeType BatteryGreen { get { return StateIndicatorShapeType.CarBattery3; } }
		public static StateIndicatorShapeType BatteryGray { get { return StateIndicatorShapeType.CarBattery4; } }
		public static StateIndicatorShapeType ABSRed { get { return StateIndicatorShapeType.CarABS1; } }
		public static StateIndicatorShapeType ABSOrange { get { return StateIndicatorShapeType.CarABS2; } }
		public static StateIndicatorShapeType ABSGreen { get { return StateIndicatorShapeType.CarABS3; } }
		public static StateIndicatorShapeType ABSGray { get { return StateIndicatorShapeType.CarABS4; } }
		public static StateIndicatorShapeType EngineRed { get { return StateIndicatorShapeType.CarEngine1; } }
		public static StateIndicatorShapeType EngineOrange { get { return StateIndicatorShapeType.CarEngine2; } }
		public static StateIndicatorShapeType EngineGreen { get { return StateIndicatorShapeType.CarEngine3; } }
		public static StateIndicatorShapeType EngineGray { get { return StateIndicatorShapeType.CarEngine4; } }
		public static StateIndicatorShapeType ParkingLightsRed { get { return StateIndicatorShapeType.CarParkingLights1; } }
		public static StateIndicatorShapeType ParkingLightsOrange { get { return StateIndicatorShapeType.CarParkingLights2; } }
		public static StateIndicatorShapeType ParkingLightsGreen { get { return StateIndicatorShapeType.CarParkingLights3; } }
		public static StateIndicatorShapeType ParkingLightsGray { get { return StateIndicatorShapeType.CarParkingLights4; } }
		public static StateIndicatorShapeType LowBeamRed { get { return StateIndicatorShapeType.CarLowBeam1; } }
		public static StateIndicatorShapeType LowBeamOrange { get { return StateIndicatorShapeType.CarLowBeam2; } }
		public static StateIndicatorShapeType LowBeamGreen { get { return StateIndicatorShapeType.CarLowBeam3; } }
		public static StateIndicatorShapeType LowBeamGray { get { return StateIndicatorShapeType.CarLowBeam4; } }
		public static StateIndicatorShapeType HiBeamRed { get { return StateIndicatorShapeType.CarHiBeam1; } }
		public static StateIndicatorShapeType HiBeamOrange { get { return StateIndicatorShapeType.CarHiBeam2; } }
		public static StateIndicatorShapeType HiBeamGreen { get { return StateIndicatorShapeType.CarHiBeam3; } }
		public static StateIndicatorShapeType HiBeamGray { get { return StateIndicatorShapeType.CarHiBeam4; } }
		public static StateIndicatorShapeType AirBagRed { get { return StateIndicatorShapeType.CarAirBag1; } }
		public static StateIndicatorShapeType AirBagOrange { get { return StateIndicatorShapeType.CarAirBag2; } }
		public static StateIndicatorShapeType AirBagGreen { get { return StateIndicatorShapeType.CarAirBag3; } }
		public static StateIndicatorShapeType AirBagGray { get { return StateIndicatorShapeType.CarAirBag4; } }
		public static StateIndicatorShapeType WaterRed { get { return StateIndicatorShapeType.CarWater1; } }
		public static StateIndicatorShapeType WaterOrange { get { return StateIndicatorShapeType.CarWater2; } }
		public static StateIndicatorShapeType WaterGreen { get { return StateIndicatorShapeType.CarWater3; } }
		public static StateIndicatorShapeType WaterGray { get { return StateIndicatorShapeType.CarWater4; } }
		public static StateIndicatorShapeType GasRed { get { return StateIndicatorShapeType.CarGas1; } }
		public static StateIndicatorShapeType GasOrange { get { return StateIndicatorShapeType.CarGas2; } }
		public static StateIndicatorShapeType GasGreen { get { return StateIndicatorShapeType.CarGas3; } }
		public static StateIndicatorShapeType GasGray { get { return StateIndicatorShapeType.CarGas4; } }
		public static StateIndicatorShapeType MotorOilRed { get { return StateIndicatorShapeType.CarMotorOil1; } }
		public static StateIndicatorShapeType MotorOilOrange { get { return StateIndicatorShapeType.CarMotorOil2; } }
		public static StateIndicatorShapeType MotorOilGreen { get { return StateIndicatorShapeType.CarMotorOil3; } }
		public static StateIndicatorShapeType MotorOilGray { get { return StateIndicatorShapeType.CarMotorOil4; } }
	}
	public static class ElectricLights {
		public static StateIndicatorShapeType Off { get { return StateIndicatorShapeType.ElectricLight1; } }
		public static StateIndicatorShapeType Red { get { return StateIndicatorShapeType.ElectricLight2; } }
		public static StateIndicatorShapeType Amber { get { return StateIndicatorShapeType.ElectricLight3; } }
		public static StateIndicatorShapeType Green { get { return StateIndicatorShapeType.ElectricLight4; } }
	}
	public static class TrafficLights {
		public static StateIndicatorShapeType Off { get { return StateIndicatorShapeType.TrafficLight1; } }
		public static StateIndicatorShapeType Red { get { return StateIndicatorShapeType.TrafficLight2; } }
		public static StateIndicatorShapeType Amber { get { return StateIndicatorShapeType.TrafficLight3; } }
		public static StateIndicatorShapeType Green { get { return StateIndicatorShapeType.TrafficLight4; } }
	}
	public static class Smiles {
		public static StateIndicatorShapeType Laughing { get { return StateIndicatorShapeType.Smile1; } }
		public static StateIndicatorShapeType Happy { get { return StateIndicatorShapeType.Smile2; } }
		public static StateIndicatorShapeType Neutral { get { return StateIndicatorShapeType.Smile3; } }
		public static StateIndicatorShapeType Sad { get { return StateIndicatorShapeType.Smile4; } }
		public static StateIndicatorShapeType Crying { get { return StateIndicatorShapeType.Smile5; } }
	}
	public static class Arrows {
		public static StateIndicatorShapeType Up { get { return StateIndicatorShapeType.Arrow1; } }
		public static StateIndicatorShapeType Down { get { return StateIndicatorShapeType.Arrow2; } }
		public static StateIndicatorShapeType Left { get { return StateIndicatorShapeType.Arrow3; } }
		public static StateIndicatorShapeType Right { get { return StateIndicatorShapeType.Arrow4; } }
		public static StateIndicatorShapeType LeftUp { get { return StateIndicatorShapeType.Arrow5; } }
		public static StateIndicatorShapeType RightUp { get { return StateIndicatorShapeType.Arrow6; } }
		public static StateIndicatorShapeType LeftDown { get { return StateIndicatorShapeType.Arrow8; } }
		public static StateIndicatorShapeType RightDown { get { return StateIndicatorShapeType.Arrow7; } }
		public static StateIndicatorShapeType DoubleUp { get { return StateIndicatorShapeType.Arrow9; } }
		public static StateIndicatorShapeType DoubleDown { get { return StateIndicatorShapeType.Arrow10; } }
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public static class DashboardIndicators {
		public static StateIndicatorShapeType TrendUp { get { return StateIndicatorShapeType.DashboardTrendArrowUp; } }
		public static StateIndicatorShapeType TrendDown { get { return StateIndicatorShapeType.DashboardTrendArrowDown; } }
		public static StateIndicatorShapeType Warning { get { return StateIndicatorShapeType.DashboardWarning; } }
	}
	public delegate void CustomTickmarkTextEventHandler(CustomTickmarkTextEventArgs ea);
	public delegate void CustomRescalingEventHandler(CustomRescalingEventArgs ea);
	public class CustomRescalingEventArgs : EventArgs {
		float valueCore;
		float minValueCore;
		float maxValueCore;
		public CustomRescalingEventArgs(float value, float minValue, float maxValue) {
			this.valueCore = value;
			this.minValueCore = minValue;
			this.maxValueCore = maxValue;
		}
		public float Value {
			get { return valueCore; }
			set { valueCore = value; }
		}
		public float MinValue {
			get { return minValueCore; }
			set { minValueCore = value; }
		}
		public float MaxValue {
			get { return maxValueCore; }
			set { maxValueCore = value; }
		}
	}
	public class ValueChangedEventArgs : EventArgs {
		float prevCore;
		float valueCore;
		public ValueChangedEventArgs(float prev, float value)
			: base() {
			this.prevCore = prev;
			this.valueCore = value;
		}
		public float PrevValue {
			get { return prevCore; }
		}
		public float Value {
			get { return valueCore; }
		}
	}
	public class MinMaxValueChangedEventArgs : ValueChangedEventArgs {
		bool isMinCore;
		public MinMaxValueChangedEventArgs(bool isMin, float prev, float value)
			: base(prev, value) {
			isMinCore = isMin;
		}
		public bool IsMin { get { return isMinCore; } }
	}
	public class CustomTickmarkTextEventArgs : EventArgs {
		string textCore;
		float valueCore;
		string resultCore;
		public CustomTickmarkTextEventArgs(string text, float value) {
			this.resultCore = this.textCore = text;
			this.valueCore = value;
		}
		public string Text {
			get { return textCore; }
		}
		public float Value {
			get { return valueCore; }
		}
		public string Result {
			get { return resultCore; }
			set { resultCore = value; }
		}
	}
}
