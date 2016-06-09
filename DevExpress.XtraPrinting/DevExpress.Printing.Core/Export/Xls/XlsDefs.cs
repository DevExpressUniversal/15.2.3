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
using System.Text;
namespace DevExpress.XtraExport.Xls {
	#region XlsDefs
	public static class XlsDefs {
		public const int MaxRecordDataSize = 8224;
		public const int MaxFontNameLength = 31;
		public const int MaxSheetNameLength = 31;
		public const int MaxFontRecordsCount = 510;
		public const int MaxFormatRecordsCount = 218;
		public const int MinStringsInBucket = 8;
		public const int MaxHeaderFooterLength = 255;
		public const int MaxColumnCount = 256;
		public const int MaxRowCount = 65536;
		public const int MaxLelIndex = 2048;
		public const int MaxMergeCellCount = 1026;
		public const int MaxOutlineLevel = 7;
		public const int MaxXFCount = 4050;
		public const int MaxStyleXFCount = 2048;
		public const int MaxCellXFCount = 2048;
		public const int MaxDefaultRowHeight = 8179;
		public const double MinMarginInInches = 0.0;
		public const double MaxMarginInInches = 49.0;
		public const int MaxCFRefCount = 513;
		public const int MaxFormulaStringLength = 1024;
		public const int MaxFormulaBytesSize = 1800;
		public const int MaxDataValidationRecordCount = 65534;
		public const int MaxDataValidationSqRefCount = 432;
		public const int MaxDataValidationTitleLength = 32;
		public const int MaxDataValidationPromptLength = 255;
		public const int MaxDataValidationErrorLength = 225;
		public const int UnusedFontIndex = 4;
		public const short DefaultColumnWidth = 8;
		public const int DefaultRowHeightInTwips = 0xff;
		public const int FullRangeColumnIndex = 0x100;
		public const int NoScope = -2;
		public const short BIFFVersion = 0x0600;
		public const short BIFF5Version = 0x0500;
		public const short DefaultBuildYear = 0x07cd;
		public const short DefaultBuildIdentifier = 0x3267;
		public const int DefaultVersionXL = 0x0e;
		public const double DefaultHeaderFooterMargin = 0.3;
		public const double DefaultLeftRightMargin = 0.7;
		public const double DefaultTopBottomMargin = 0.75;
		public const int DefaultStyleXFIndex = 0;
		public const int DefaultCellXFIndex = 15;
	}
	#endregion
	#region XlsRecordType
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
	public static class XlsRecordType {
		public const short BOF = 0x0809;
		public const short EOF = 0x000a;
		public const short Continue = 0x003c;
		public const short InterfaceHdr = 0x00e1;
		public const short Mms = 0x00c1;
		public const short InterfaceEnd = 0x00e2;
		public const short WriteAccess = 0x005c;
		public const short CodePage = 0x0042;
		public const short DSF = 0x0161;
		public const short RRTabId = 0x013d;
		public const short WinProtect = 0x0019;
		public const short Protect = 0x0012;
		public const short Password = 0x0013;
		public const short Prot4Rev = 0x01af;
		public const short Prot4RevPass = 0x01bc;
		public const short Window1 = 0x003d;
		public const short Backup = 0x0040;
		public const short HideObj = 0x008d;
		public const short Date1904 = 0x0022;
		public const short CalcPrecision = 0x000e;
		public const short RefreshAll = 0x01b7;
		public const short BookBool = 0x00da;
		public const short Font = 0x0031;
		public const short Format = 0x041e;
		public const short XF = 0x00e0;
		public const short XFCrc = 0x087c;
		public const short XFExt = 0x087d;
		public const short Style = 0x0293;
		public const short StyleExt = 0x0892;
		public const short UseELFs = 0x0160;
		public const short BoundSheet8 = 0x0085;
		public const short Country = 0x008c;
		public const short SupBook = 0x01ae;
		public const short ExternSheet = 0x0017;
		public const short Lbl = 0x0018;
		public const short MsoDrawingGroup = 0x00eb;
		public const short SST = 0x00fc;
		public const short ExtSST = 0x00ff;
		public const short Theme = 0x0896;
		public const short Index = 0x020b;
		public const short CalcMode = 0x000d;
		public const short CalcCount = 0x000c;
		public const short CalcRefMode = 0x000f;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Spelling", "DF1000")]
		public const short CalcIter = 0x0011;
		public const short CalcDelta = 0x0010;
		public const short CalcSaveRecalc = 0x005f;
		public const short PrintRowCol = 0x002a;
		public const short PrintGrid = 0x002b;
		public const short GridSet = 0x0082;
		public const short Guts = 0x0080;
		public const short DefaultRowHeight = 0x0225;
		public const short WsBool = 0x0081;
		public const short HorizontalPageBreaks = 0x001b;
		public const short VerticalPageBreaks = 0x001a;
		public const short Header = 0x0014;
		public const short Footer = 0x0015;
		public const short HCenter = 0x0083;
		public const short VCenter = 0x0084;
		public const short LeftMargin = 0x0026;
		public const short RightMargin = 0x0027;
		public const short TopMargin = 0x0028;
		public const short BottomMargin = 0x0029;
		public const short Setup = 0x00a1;
		public const short HeaderFooter = 0x089c;
		public const short DefColumnWidth = 0x0055;
		public const short ColInfo = 0x007d;
		public const short AutoFilterInfo = 0x009d;
		public const short Dimensions = 0x0200;
		public const short Window2 = 0x023e;
		public const short Pane = 0x0041;
		public const short Row = 0x0208;
		public const short SharedFormula = 0x04bc;
		public const short Formula = 0x0006;
		public const short String = 0x0207;
		public const short Blank = 0x0201;
		public const short MulBlank = 0x00be;
		public const short Rk = 0x027e;
		public const short MulRk = 0x000bd;
		public const short BoolErr = 0x0205;
		public const short Number = 0x0203;
		public const short LabelSst = 0x00fd;
		public const short DbCell = 0x00d7;
		public const short MergeCells = 0x00e5;
		public const short DVal = 0x01b2;
		public const short Dv = 0x01be;
		public const short HLink = 0x01b8;
		public const short HLinkTooltip = 0x0800;
		public const short CondFmt = 0x01b0;
		public const short CondFmt12 = 0x0879;
		public const short CF = 0x01b1;
		public const short CF12 = 0x087a;
		public const short CFEx = 0x087b;
		public const short FeatHdr = 0x0867;
		public const short Feat = 0x0868;
		public const short MsoDrawing = 0x00ec;
		public const short Obj = 0x005d;
	}
#endregion
}
