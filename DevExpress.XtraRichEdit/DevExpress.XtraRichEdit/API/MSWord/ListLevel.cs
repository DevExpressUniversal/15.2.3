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
	#region ListLevel
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ListLevel : IWordObject {
		int Index { get; }
		string NumberFormat { get; set; }
		WdTrailingCharacter TrailingCharacter { get; set; }
		WdListNumberStyle NumberStyle { get; set; }
		float NumberPosition { get; set; }
		WdListLevelAlignment Alignment { get; set; }
		float TextPosition { get; set; }
		float TabPosition { get; set; }
		bool ResetOnHigherOld { get; set; }
		int StartAt { get; set; }
		string LinkedStyle { get; set; }
		Font Font { get; set; }
		int ResetOnHigher { get; set; }
		InlineShape PictureBullet { get; }
		InlineShape ApplyPictureBullet(string FileName);
	}
	#endregion
	#region ListLevels
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ListLevels : IWordObject, IEnumerable {
		int Count { get; }
		ListLevel this[int Index] { get; }
	}
	#endregion
	#region WdTrailingCharacter
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTrailingCharacter {
		wdTrailingTab,
		wdTrailingSpace,
		wdTrailingNone
	}
	#endregion
	#region WdListNumberStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdListNumberStyle {
		wdListNumberStyleAiueo = 20,
		wdListNumberStyleAiueoHalfWidth = 12,
		wdListNumberStyleArabic = 0,
		wdListNumberStyleArabic1 = 0x2e,
		wdListNumberStyleArabic2 = 0x30,
		wdListNumberStyleArabicFullWidth = 14,
		wdListNumberStyleArabicLZ = 0x16,
		wdListNumberStyleBullet = 0x17,
		wdListNumberStyleCardinalText = 6,
		wdListNumberStyleChosung = 0x19,
		wdListNumberStyleGanada = 0x18,
		wdListNumberStyleGBNum1 = 0x1a,
		wdListNumberStyleGBNum2 = 0x1b,
		wdListNumberStyleGBNum3 = 0x1c,
		wdListNumberStyleGBNum4 = 0x1d,
		wdListNumberStyleHangul = 0x2b,
		wdListNumberStyleHanja = 0x2c,
		wdListNumberStyleHanjaRead = 0x29,
		wdListNumberStyleHanjaReadDigit = 0x2a,
		wdListNumberStyleHebrew1 = 0x2d,
		wdListNumberStyleHebrew2 = 0x2f,
		wdListNumberStyleHindiArabic = 0x33,
		wdListNumberStyleHindiCardinalText = 0x34,
		wdListNumberStyleHindiLetter1 = 0x31,
		wdListNumberStyleHindiLetter2 = 50,
		wdListNumberStyleIroha = 0x15,
		wdListNumberStyleIrohaHalfWidth = 13,
		wdListNumberStyleKanji = 10,
		wdListNumberStyleKanjiDigit = 11,
		wdListNumberStyleKanjiTraditional = 0x10,
		wdListNumberStyleKanjiTraditional2 = 0x11,
		wdListNumberStyleLegal = 0xfd,
		wdListNumberStyleLegalLZ = 0xfe,
		wdListNumberStyleLowercaseLetter = 4,
		wdListNumberStyleLowercaseRoman = 2,
		wdListNumberStyleLowercaseRussian = 0x3a,
		wdListNumberStyleNone = 0xff,
		wdListNumberStyleNumberInCircle = 0x12,
		wdListNumberStyleOrdinal = 5,
		wdListNumberStyleOrdinalText = 7,
		wdListNumberStylePictureBullet = 0xf9,
		wdListNumberStyleSimpChinNum1 = 0x25,
		wdListNumberStyleSimpChinNum2 = 0x26,
		wdListNumberStyleSimpChinNum3 = 0x27,
		wdListNumberStyleSimpChinNum4 = 40,
		wdListNumberStyleThaiArabic = 0x36,
		wdListNumberStyleThaiCardinalText = 0x37,
		wdListNumberStyleThaiLetter = 0x35,
		wdListNumberStyleTradChinNum1 = 0x21,
		wdListNumberStyleTradChinNum2 = 0x22,
		wdListNumberStyleTradChinNum3 = 0x23,
		wdListNumberStyleTradChinNum4 = 0x24,
		wdListNumberStyleUppercaseLetter = 3,
		wdListNumberStyleUppercaseRoman = 1,
		wdListNumberStyleUppercaseRussian = 0x3b,
		wdListNumberStyleVietCardinalText = 0x38,
		wdListNumberStyleZodiac1 = 30,
		wdListNumberStyleZodiac2 = 0x1f,
		wdListNumberStyleZodiac3 = 0x20
	}
	#endregion
	#region WdListLevelAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdListLevelAlignment {
		wdListLevelAlignLeft,
		wdListLevelAlignCenter,
		wdListLevelAlignRight
	}
	#endregion
}
