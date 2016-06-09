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

using System.ComponentModel;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting.BarCode {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum CodabarStartStopPair {
		None = 0,
		AT = 1,
		BN = 2,
		CStar = 3,
		DE = 4
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum Code128Charset {
		CharsetA = 103,
		CharsetB = 104,
		CharsetC = 105,
		CharsetAuto = 0
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum MSICheckSum {
		None = 0,
		Modulo10 = 1,
		DoubleModulo10 = 2,
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum BarCodeOrientation {
		Normal,
		UpsideDown,
		RotateLeft,
		RotateRight
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum ErrorCorrectionLevel {
		Level0,
		Level1,
		Level2,
		Level3,
		Level4,
		Level5,
		Level6,
		Level7,
		Level8
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum PDF417CompactionMode {
		Binary,
		Text
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum DataMatrixCompactionMode {
		ASCII,
		C40,
		Text,
		X12,
		Edifact,
		Binary
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum DataMatrixSize {
		MatrixAuto,
		Matrix10x10,
		Matrix12x12,
		Matrix14x14,
		Matrix16x16,
		Matrix18x18,
		Matrix20x20,
		Matrix22x22,
		Matrix24x24,
		Matrix26x26,
		Matrix32x32,
		Matrix36x36,
		Matrix40x40,
		Matrix44x44,
		Matrix48x48,
		Matrix52x52,
		Matrix64x64,
		Matrix72x72,
		Matrix80x80,
		Matrix88x88,
		Matrix96x96,
		Matrix104x104,
		Matrix120x120,
		Matrix132x132,
		Matrix144x144,
		Matrix8x18,
		Matrix8x32,
		Matrix12x26,
		Matrix12x36,
		Matrix16x36,
		Matrix16x48
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum QRCodeCompactionMode {
		Numeric,
		AlphaNumeric,
		Byte
	}
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum QRCodeErrorCorrectionLevel {
		M = 0,
		L = 1,
		H = 2,
		Q = 3,
	};
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum QRCodeVersion {
		AutoVersion = 0,
		Version1 = 1,
		Version2 = 2,
		Version3 = 3,
		Version4 = 4,
		Version5 = 5,
		Version6 = 6,
		Version7 = 7,
		Version8 = 8,
		Version9 = 9,
		Version10 = 10,
		Version11 = 11,
		Version12 = 12,
		Version13 = 13,
		Version14 = 14,
		Version15 = 15,
		Version16 = 16,
		Version17 = 17,
		Version18 = 18,
		Version19 = 19,
		Version20 = 20,
		Version21 = 21,
		Version22 = 22,
		Version23 = 23,
		Version24 = 24,
		Version25 = 25,
		Version26 = 26,
		Version27 = 27,
		Version28 = 28,
		Version29 = 29,
		Version30 = 30,
		Version31 = 31,
		Version32 = 32,
		Version33 = 33,
		Version34 = 34,
		Version35 = 35,
		Version36 = 36,
		Version37 = 37,
		Version38 = 38,
		Version39 = 39,
		Version40 = 40,
	};
	[TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder))]
	public enum DataBarType {
		Omnidirectional,
		Truncated,
		Stacked,
		StackedOmnidirectional,
		Limited,
		Expanded,
		ExpandedStacked,
	};
}
