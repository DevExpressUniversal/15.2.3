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
using System.Drawing;
using System.Runtime.InteropServices;
namespace DevExpress.Office.PInvoke {
	static partial class PInvokeSafeNativeMethods {
		#region LOGFONT
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), ComVisible(false)]
		public class LOGFONT {
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public LogFontCharSet lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string lfFaceName = null;
		}
		#endregion
		public enum LogFontCharSet : byte {
			ANSI = 0,
			DEFAULT = 1,
			SYMBOL = 2,
			SHIFTJIS = 128,
			HANGEUL = 129,
			HANGUL = 129,
			GB2312 = 134,
			CHINESEBIG5 = 136,
			OEM = 255,
			JOHAB = 130,
			HEBREW = 177,
			ARABIC = 178,
			GREEK = 161,
			TURKISH = 162,
			VIETNAMESE = 163,
			THAI = 222,
			EASTEUROPE = 238,
			RUSSIAN = 204,
			MAC = 77,
			BALTIC = 186
		}
	}
	public static partial class Win32 {
		#region FontCharset
		public enum FontCharset {
			Ansi = 0,
			Default = 1,
			Symbol = 2,
			ShiftJis = 128,
			Hangeul = 129,
			GB2312 = 134,
			ChineseBig5 = 136,
			Oem = 255,
			Johab = 130,
			Hebrew = 177,
			Arabic = 178,
			Greek = 161,
			Turkish = 162,
			Vietnamese = 163,
			Thai = 222,
			EastEurope = 238,
			Russian = 204,
			Mac = 77,
			Baltic = 186
		}
		#endregion
	}
}
