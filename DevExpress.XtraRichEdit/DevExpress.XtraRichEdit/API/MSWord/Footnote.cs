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
	#region Footnote
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Footnote : IWordObject {
		Range Range { get; }
		Range Reference { get; }
		int Index { get; }
		void Delete();
	}
	#endregion
	#region Footnotes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Footnotes : IWordObject, IEnumerable {
		int Count { get; }
		WdFootnoteLocation Location { get; set; }
		WdNoteNumberStyle NumberStyle { get; set; }
		int StartingNumber { get; set; }
		WdNumberingRule NumberingRule { get; set; }
		Range Separator { get; }
		Range ContinuationSeparator { get; }
		Range ContinuationNotice { get; }
		Footnote this[int Index] { get; }
		Footnote Add(Range Range, ref object Reference, ref object Text);
		void Convert();
		void SwapWithEndnotes();
		void ResetSeparator();
		void ResetContinuationSeparator();
		void ResetContinuationNotice();
	}
	#endregion
	#region FootnoteOptions
	[GeneratedCode("Suppress FxCop check", "")]
	public interface FootnoteOptions : IWordObject {
		WdFootnoteLocation Location { get; set; }
		WdNoteNumberStyle NumberStyle { get; set; }
		int StartingNumber { get; set; }
		WdNumberingRule NumberingRule { get; set; }
	}
	#endregion
	#region WdNoteNumberStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdNoteNumberStyle {
		wdNoteNumberStyleArabic = 0,
		wdNoteNumberStyleArabicFullWidth = 14,
		wdNoteNumberStyleArabicLetter1 = 0x2e,
		wdNoteNumberStyleArabicLetter2 = 0x30,
		wdNoteNumberStyleHanjaRead = 0x29,
		wdNoteNumberStyleHanjaReadDigit = 0x2a,
		wdNoteNumberStyleHebrewLetter1 = 0x2d,
		wdNoteNumberStyleHebrewLetter2 = 0x2f,
		wdNoteNumberStyleHindiArabic = 0x33,
		wdNoteNumberStyleHindiCardinalText = 0x34,
		wdNoteNumberStyleHindiLetter1 = 0x31,
		wdNoteNumberStyleHindiLetter2 = 50,
		wdNoteNumberStyleKanji = 10,
		wdNoteNumberStyleKanjiDigit = 11,
		wdNoteNumberStyleKanjiTraditional = 0x10,
		wdNoteNumberStyleLowercaseLetter = 4,
		wdNoteNumberStyleLowercaseRoman = 2,
		wdNoteNumberStyleNumberInCircle = 0x12,
		wdNoteNumberStyleSimpChinNum1 = 0x25,
		wdNoteNumberStyleSimpChinNum2 = 0x26,
		wdNoteNumberStyleSymbol = 9,
		wdNoteNumberStyleThaiArabic = 0x36,
		wdNoteNumberStyleThaiCardinalText = 0x37,
		wdNoteNumberStyleThaiLetter = 0x35,
		wdNoteNumberStyleTradChinNum1 = 0x21,
		wdNoteNumberStyleTradChinNum2 = 0x22,
		wdNoteNumberStyleUppercaseLetter = 3,
		wdNoteNumberStyleUppercaseRoman = 1,
		wdNoteNumberStyleVietCardinalText = 0x38
	}
	#endregion
	#region WdFootnoteLocation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFootnoteLocation {
		wdBottomOfPage,
		wdBeneathText
	}
	#endregion
}
