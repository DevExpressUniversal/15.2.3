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
	#region PageNumber
	[GeneratedCode("Suppress FxCop check", "")]
	public interface PageNumber : IWordObject {
		int Index { get; }
		WdPageNumberAlignment Alignment { get; set; }
		void Select();
		void Copy();
		void Cut();
		void Delete();
	}
	#endregion
	#region PageNumbers
	[GeneratedCode("Suppress FxCop check", "")]
	public interface PageNumbers : IWordObject, IEnumerable {
		int Count { get; }
		WdPageNumberStyle NumberStyle { get; set; }
		bool IncludeChapterNumber { get; set; }
		int HeadingLevelForChapter { get; set; }
		WdSeparatorType ChapterPageSeparator { get; set; }
		bool RestartNumberingAtSection { get; set; }
		int StartingNumber { get; set; }
		bool ShowFirstPageNumber { get; set; }
		PageNumber this[int Index] { get; }
		PageNumber Add(ref object PageNumberAlignment, ref object FirstPage);
		bool DoubleQuote { get; set; }
	}
	#endregion
	#region WdPageNumberAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPageNumberAlignment {
		wdAlignPageNumberLeft,
		wdAlignPageNumberCenter,
		wdAlignPageNumberRight,
		wdAlignPageNumberInside,
		wdAlignPageNumberOutside
	}
	#endregion
	#region WdPageNumberStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPageNumberStyle {
		wdPageNumberStyleArabic = 0,
		wdPageNumberStyleArabicFullWidth = 14,
		wdPageNumberStyleArabicLetter1 = 0x2e,
		wdPageNumberStyleArabicLetter2 = 0x30,
		wdPageNumberStyleHanjaRead = 0x29,
		wdPageNumberStyleHanjaReadDigit = 0x2a,
		wdPageNumberStyleHebrewLetter1 = 0x2d,
		wdPageNumberStyleHebrewLetter2 = 0x2f,
		wdPageNumberStyleHindiArabic = 0x33,
		wdPageNumberStyleHindiCardinalText = 0x34,
		wdPageNumberStyleHindiLetter1 = 0x31,
		wdPageNumberStyleHindiLetter2 = 50,
		wdPageNumberStyleKanji = 10,
		wdPageNumberStyleKanjiDigit = 11,
		wdPageNumberStyleKanjiTraditional = 0x10,
		wdPageNumberStyleLowercaseLetter = 4,
		wdPageNumberStyleLowercaseRoman = 2,
		wdPageNumberStyleNumberInCircle = 0x12,
		wdPageNumberStyleNumberInDash = 0x39,
		wdPageNumberStyleSimpChinNum1 = 0x25,
		wdPageNumberStyleSimpChinNum2 = 0x26,
		wdPageNumberStyleThaiArabic = 0x36,
		wdPageNumberStyleThaiCardinalText = 0x37,
		wdPageNumberStyleThaiLetter = 0x35,
		wdPageNumberStyleTradChinNum1 = 0x21,
		wdPageNumberStyleTradChinNum2 = 0x22,
		wdPageNumberStyleUppercaseLetter = 3,
		wdPageNumberStyleUppercaseRoman = 1,
		wdPageNumberStyleVietCardinalText = 0x38
	}
	#endregion
	#region WdSeparatorType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSeparatorType {
		wdSeparatorHyphen,
		wdSeparatorPeriod,
		wdSeparatorColon,
		wdSeparatorEmDash,
		wdSeparatorEnDash
	}
	#endregion
}
