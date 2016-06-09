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
using System.Drawing;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport {
	public enum XlsExportOptimization {
		BySize,
		BySpeed
	}
	public enum XlsCellErrType {
		NULL = 0, 
		DIV_0 = 0x07, 
		VALUE = 0x0F, 
		REF = 0x17, 
		NAME = 0x1D, 
		NUM = 0x24,
		N_A = 0x2A	
	}
	[CLSCompliant(false)]
	public class XlsConsts {
		XlsConsts() {
		}
		public static XlsExportOptimization Optimization = XlsExportOptimization.BySpeed;
		public const ushort MaxColumn = 0xFF;
		public const ushort MaxRow = 0xFFFF;
		public const int MaxBlockSize = 8192;
		public const int BlankCellSize = 10;
		public const int CountOfXFStyles = 21;
		public const ushort MaxLenShortStringA = 0xFF;
		public const ushort MaxLenShortStringW = MaxLenShortStringA >> 1;
		public const ushort Font = 0x0031; 
		public const ushort Format = 0x041E; 
		public const ushort XF = 0x00E0; 
		public const ushort XFCRC = 0x087C; 
		public const ushort XFEXT = 0x087D; 
		public const ushort COLINFO = 0x007D; 
		public const ushort Row = 0x0208; 
		public const ushort Palette = 0x0092; 
		public const ushort BoundSheet = 0x0085; 
		public const ushort MergeCells = 0x00E5; 
		public const ushort Currency = 0x1003; 
		public const ushort DateTime = 0x1000; 
		public const ushort Date = 0x1001; 
		public const ushort Time = 0x1002; 
		public const ushort MergeState = 0x2000; 
		public const ushort BoolErr = 0x0205; 
		public const ushort Blank = 0x0201; 
		public const ushort Number = 0x0203; 
		public const ushort Label = 0x0204; 
		public const ushort LabelSST = 0x00FD; 
		public const ushort ExtSST = 0x00FF; 
		public const ushort SST = 0x00FC; 
		public const ushort Continue = 0x003C; 
		public const ushort MaxRecSize97 = MaxBlockSize;
		public const ushort SpId_Pict = 0x4B;
		#region Format Strings
		public const ushort GeneralFormat = 0x00;				
		public const ushort NoneDecimalFormat = 0x01;			
		public const ushort DecimalFormat = 0x02;				
		public const ushort DigitNoneDecimalFormat = 0x03;		
		public const ushort DigitDecimalFormat = 0x04;			
		public const ushort CurrencyNoneDecimalFormat = 0x05;	
		public const ushort CurrencyDecimalFormat = 0x07;   	
		public const ushort PercentNoneDecimalFormat = 0x09;	
		public const ushort PercentDecimalFormat = 0x0a;		
		public const ushort ExponentialDecimalFormat = 0x0b;		
		public const ushort ExponentialDecimalOneFormat = 0x30;	
		public const ushort DateFormat = 0x0e;					
		public const ushort DayMonthYearFormat = 0x0f;			
		public const ushort DayMonthFormat = 0x10;				
		public const ushort MonthYearFormat = 0x11;				
		public const ushort HourMinuteAMPMFormat = 0x12;		
		public const ushort HourMinuteSecondAMPMFormat = 0x13;	
		public const ushort HourMinuteFormat = 0x14;			
		public const ushort HourMinuteSecondFormat = 0x15;		
		public const ushort DateTimeFormat = 0x16;				
		public const ushort AccountFormat = 0x25;			   
		public const ushort AccountDecimalFormat = 0x27;		
		public const ushort MinuteSecondFormat = 0x2d;			
		public const ushort AbsoluteHourTimeFormat = 0x2e;		
		public const ushort MinuteSecondMilFormat = 0x2f;		
		public const ushort RealFormat = 0x31;					
		#endregion
		#region style consts
		public const byte WrapText = 0x08;
		public const byte ShrinkToFit = 0x10;
		public const byte ReadingOrder_LeftToRight = 0x40;
		public const byte ReadingOrder_RightToLeft = 0x80;
		#endregion
		#region BLIP
		public const ushort BLIP_Extra = 17;
		public const ushort BLIP_Png = 0x06E0;
		public const ushort BLIP_Jpeg = 0x046A;
		#endregion
		#region ObjRec
		public const ushort ObjRec_End = 0x00;
		public const ushort ObjRec_CF = 0x07;
		public const ushort ObjRec_PioGrbit = 0x08;
		public const ushort ObjRec_Cmo = 0x15;
		#endregion
		#region Mso
		public const ushort MsoSpId = 0x0401;
		public const ushort MsoBLIP_Start = 0xF018;
		public const ushort MsoDggContainer = 0xF000;
		public const ushort MsoBStoreContainer = 0xF001;
		public const ushort MsoDgContainer = 0xF002;
		public const ushort MsoSpGrContainer = 0xF003;
		public const ushort MsoSpContainer = 0xF004;
		public const ushort MsoDgg = 0xF006;
		public const ushort MsoBse = 0xF007;
		public const ushort MsoDg = 0xF008;
		public const ushort MsoSpGr = 0xF009;
		public const ushort MsoSp = 0xF00A;
		public const ushort MsoOpt = 0xF00B;
		public const ushort MsoClientAnchor = 0xF010;
		public const ushort MsoClientData = 0xF011;
		public const ushort MsoSplitMenuColors = 0xF11E;
		#endregion
		#region BIFF
		public const ushort BIFFRecId_MsoDrawingGroup = 0x00EB;
		public const ushort BIFFRecId_MsoDrawing = 0x00EC;
		public const ushort BIFFRecId_Continue = 0x003C;
		public const ushort BIFFRecId_Obj = 0x005D;
		public const ushort BIFFRecId_Hlink = 0x01B8;
		#endregion
		#region sizes
		public const int SizeOfBOF = 20;
		public const int SizeOfEOF = 4;
		public const int SizeOfWINDOW1 = 22;
		public const int SizeOfWINDOW2 = 22;
		public const int SizeOfWSBOOL = 6;
		public const int SizeOfMargin = 12;
		public const int SizeOfSetup = 38;
		public const int SizeOfFont_ = 30;
		public const int SizeOfTabID = 6;
		public const int SizeOfSupBook = 8;
		public const int SizeOfExternSheet = 12;
		public const int SizeOfDimension = 18;
		public const int SizeOfSTYLE = 48;
		public const int SizeOfXF_ = CountOfXFStyles * 24;
		public const int SizeOfPalette_ = 56 * 4;
		public const int SizeOfHyperlinkData = 16;
		public const int SizeOfXFChecksum = 20;
		public const int DefaultDataSize = SizeOfBOF * 2 + SizeOfEOF * 2 +
			SizeOfWINDOW1 + SizeOfWINDOW2 + SizeOfFont_ * 5 + SizeOfTabID +
			SizeOfSupBook + SizeOfExternSheet + SizeOfWSBOOL + SizeOfMargin * 4 + 
			SizeOfSetup + SizeOfSTYLE + SizeOfDimension + 12 + SizeOfXF_;
		#endregion
		#region byte arrays consts
		public static byte[] BOF = new byte[SizeOfBOF] { 
			 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00, 0xBB, 0x0D,
			 0xCC, 0x07, 0x00, 0x00, 0x00, 0x00, 0x06, 0x06, 0x00, 0x00};
		public static readonly byte[] EOF = new byte[SizeOfEOF] { 
			 0x0A, 0x00, 0x00, 0x00};
		public static readonly byte[] WINDOW1 = new byte[SizeOfWINDOW1] {
			 0x3D, 0x00, 0x12, 0x00, 0xE0, 0x01, 0x69, 0x00, 0xCC, 0x42, 0x7F,
			 0x26, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x58, 0x02};
		public static readonly byte[] WINDOW2 = new byte[SizeOfWINDOW2] {
			 0x3E, 0x02, 0x12, 0x00, 0xB6, 0x06, 0x00, 0x00, 0x00, 0x00, 0x40,
			 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly byte[] WSBOOL = new byte[SizeOfWSBOOL] {
			 0x81, 0x00, 0x02, 0x00, 0xC1, 0x04};
		public static readonly byte[] Margin = new byte[SizeOfMargin] {
			 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly byte[] SETUP = new byte[SizeOfSetup] {  
			 0xA1, 0x00, 0x22, 0x00, 0x00, 0x00, 0x64, 0x00, 
			 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
			 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			 0x00, 0x00, 0x00, 0x00, 0x01, 0x00};
		public static readonly byte[] Font_ = new byte[SizeOfFont_] {
			 0x31, 0x00, 0x1A, 0x00, 0xC8, 0x00, 0x00, 0x00, 0xFF, 0x7F, 0x90, 0x01, 0x00, 0x00, 0x00,
			 0x00, 0x00, 0x00, 0x05, 0x01, 0x41, 0x00, 0x72, 0x00, 0x69, 0x00, 0x61, 0x00, 0x6C, 0x00};
		public static readonly byte[] TabID = new byte[SizeOfTabID] {
			 0x3D, 0x01, 0x02, 0x00, 0x00, 0x00};
		public static readonly byte[] SupBook = new byte[SizeOfSupBook] {
			0xAE, 0x01, 0x04, 0x00, 0x01, 0x00, 0x01, 0x04};
		public static readonly byte[] ExternSheet = new byte[SizeOfExternSheet] {
			0x17, 0x00, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly byte[] Dimension = new byte[SizeOfDimension] {
			0x00, 0x02, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00};
		public static readonly byte[] STYLE = new byte[SizeOfSTYLE] {
			0x93, 0x02, 0x04, 0x00, 0x10, 0x80, 0x03, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x11, 0x80, 0x06, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x12, 0x80, 0x04, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x13, 0x80, 0x07, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x00, 0x80, 0x00, 0xFF,
			0x93, 0x02, 0x04, 0x00, 0x14, 0x80, 0x05, 0xFF};
		public static readonly byte[] XF_ = new byte[SizeOfXF_] {
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x20, 0x00,
			0x00, 0xF4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x2B, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x29, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x2C, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x01, 0x00, 0x2A, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20,
			0xE0, 0x00, 0x14, 0x00, 0x00, 0x00, 0x09, 0x00, 0xF8, 0xFF, 0x20, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x20};
		public static readonly int[] Palette_ = new int[SizeOfPalette_ / 4] {
			 0x000000, 0xFFFFFF, 0x0000FF, 0x00FF00, 0xFF0000, 0x00FFFF, 0xFF00FF, 0xFFFF00,
			 0x000080, 0x008000, 0x800000, 0x008080, 0x800080, 0x808000, 0xC0C0C0, 0x808080,
			 0xFF9999, 0x663399, 0xCCFFFF, 0xFFFFCC, 0x660066, 0x8080FF, 0xCC6600, 0xFFCCCC,
			 0x800000, 0xFF00FF, 0x00FFFF, 0xFFFF00, 0x800080, 0x000080, 0x808000, 0xFF0000,
			 0xFFCC00, 0xFFFFCC, 0xCCFFCC, 0x99FFFF, 0xFFCC99, 0xCC99FF, 0xFF99CC, 0x99CCFF,
			 0xFF6633, 0xCCCC33, 0x00CC99, 0x00CCFF, 0x0099FF, 0x0066FF, 0x996666, 0x969696,
			 0x663300, 0x669933, 0x003300, 0x003333, 0x003399, 0x663399, 0x993333, 0x333333};
		public static readonly byte[] HyperlinkData1 = new byte[SizeOfHyperlinkData] {
			0xD0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B};
		public static readonly byte[] HyperlinkData2 = new byte[SizeOfHyperlinkData] {
			0xE0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B};
		public static readonly byte[] XFExt_ = new byte[] {
			0x28, 0x00, 0x7D, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 
			0x00, 0x00, 0x00, 0x01, 0x00, 0x04, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly byte[] XFChecksum = new byte[] {
			0x14, 0x00, 0x7C, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF};
		public static readonly byte[] PictureHyperlinkPrefix = new byte[] {
			0xD0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B, 
			0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 
			0xE0, 0xC9, 0xEA, 0x79, 0xF9, 0xBA, 0xCE, 0x11, 
			0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B,
			0x00, 0x00, 0x00, 0x00};  
		public static readonly byte[] PictureHyperlinkPostfix = new byte[] {
			0x00, 0x00, 0x79, 0x58, 0x81, 0xF4, 0x3B, 0x1D, 
			0x7F, 0x48, 0xAF, 0x2C, 0x82, 0x5D, 0xC4, 0x85, 
			0x27, 0x63, 0x00, 0x00, 0x00, 0x00, 0xA5, 0xAB, 
			0x00, 0x00};  
		#endregion
		#region default palette
		public static Dictionary<ushort, Color> CreateDefaultColorTable() {
			Dictionary<ushort, Color> colorTable = new Dictionary<ushort, Color>();
			colorTable.Add(8, Color.FromArgb(255, 0, 0, 0));
			colorTable.Add(9, Color.FromArgb(255, 255, 255, 255));
			colorTable.Add(10, Color.FromArgb(255, 255, 0, 0));
			colorTable.Add(11, Color.FromArgb(255, 0, 255, 0));
			colorTable.Add(12, Color.FromArgb(255, 0, 0, 255));
			colorTable.Add(13, Color.FromArgb(255, 255, 255, 0));
			colorTable.Add(14, Color.FromArgb(255, 255, 0, 255));
			colorTable.Add(15, Color.FromArgb(255, 0, 255, 255));
			colorTable.Add(16, Color.FromArgb(255, 128, 0, 0));
			colorTable.Add(17, Color.FromArgb(255, 0, 128, 0));
			colorTable.Add(18, Color.FromArgb(255, 0, 0, 128));
			colorTable.Add(19, Color.FromArgb(255, 128, 128, 0));
			colorTable.Add(20, Color.FromArgb(255, 128, 0, 128));
			colorTable.Add(21, Color.FromArgb(255, 0, 128, 128));
			colorTable.Add(22, Color.FromArgb(255, 192, 192, 192));
			colorTable.Add(23, Color.FromArgb(255, 128, 128, 128));
			colorTable.Add(24, Color.FromArgb(255, 153, 153, 255));
			colorTable.Add(25, Color.FromArgb(255, 153, 51, 102));
			colorTable.Add(26, Color.FromArgb(255, 255, 255, 204));
			colorTable.Add(27, Color.FromArgb(255, 204, 255, 255));
			colorTable.Add(28, Color.FromArgb(255, 102, 0, 102));
			colorTable.Add(29, Color.FromArgb(255, 255, 128, 128));
			colorTable.Add(30, Color.FromArgb(255, 0, 102, 204));
			colorTable.Add(31, Color.FromArgb(255, 204, 204, 255));
			colorTable.Add(32, Color.FromArgb(255, 0, 0, 128));
			colorTable.Add(33, Color.FromArgb(255, 255, 0, 255));
			colorTable.Add(34, Color.FromArgb(255, 255, 255, 0));
			colorTable.Add(35, Color.FromArgb(255, 0, 255, 255));
			colorTable.Add(36, Color.FromArgb(255, 128, 0, 128));
			colorTable.Add(37, Color.FromArgb(255, 128, 0, 0));
			colorTable.Add(38, Color.FromArgb(255, 0, 128, 128));
			colorTable.Add(39, Color.FromArgb(255, 0, 0, 255));
			colorTable.Add(40, Color.FromArgb(255, 0, 204, 255));
			colorTable.Add(41, Color.FromArgb(255, 204, 255, 255));
			colorTable.Add(42, Color.FromArgb(255, 204, 255, 204));
			colorTable.Add(43, Color.FromArgb(255, 255, 255, 153));
			colorTable.Add(44, Color.FromArgb(255, 153, 204, 255));
			colorTable.Add(45, Color.FromArgb(255, 255, 153, 204));
			colorTable.Add(46, Color.FromArgb(255, 204, 153, 255));
			colorTable.Add(47, Color.FromArgb(255, 255, 204, 153));
			colorTable.Add(48, Color.FromArgb(255, 51, 102, 255));
			colorTable.Add(49, Color.FromArgb(255, 51, 204, 204));
			colorTable.Add(50, Color.FromArgb(255, 153, 204, 0));
			colorTable.Add(51, Color.FromArgb(255, 255, 204, 0));
			colorTable.Add(52, Color.FromArgb(255, 255, 153, 0));
			colorTable.Add(53, Color.FromArgb(255, 255, 102, 0));
			colorTable.Add(54, Color.FromArgb(255, 102, 102, 153));
			colorTable.Add(55, Color.FromArgb(255, 150, 150, 150));
			colorTable.Add(56, Color.FromArgb(255, 0, 51, 102));
			colorTable.Add(57, Color.FromArgb(255, 51, 153, 102));
			colorTable.Add(58, Color.FromArgb(255, 0, 51, 0));
			colorTable.Add(59, Color.FromArgb(255, 51, 51, 0));
			colorTable.Add(60, Color.FromArgb(255, 153, 51, 0));
			colorTable.Add(61, Color.FromArgb(255, 153, 51, 102));
			colorTable.Add(62, Color.FromArgb(255, 51, 51, 153));
			colorTable.Add(63, Color.FromArgb(255, 51, 51, 51));
			return colorTable;
		}
		#endregion
	}
}
