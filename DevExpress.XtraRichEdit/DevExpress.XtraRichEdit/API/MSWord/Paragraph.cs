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
	#region Paragraph
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Paragraph : IWordObject {
		Range Range { get; }
		ParagraphFormat Format { get; set; }
		TabStops TabStops { get; set; }
		Borders Borders { get; set; }
		DropCap DropCap { get; }
		object Style { get; set; }
		WdParagraphAlignment Alignment { get; set; }
		int KeepTogether { get; set; }
		int KeepWithNext { get; set; }
		int PageBreakBefore { get; set; }
		int NoLineNumber { get; set; }
		float RightIndent { get; set; }
		float LeftIndent { get; set; }
		float FirstLineIndent { get; set; }
		float LineSpacing { get; set; }
		WdLineSpacing LineSpacingRule { get; set; }
		float SpaceBefore { get; set; }
		float SpaceAfter { get; set; }
		int Hyphenation { get; set; }
		int WidowControl { get; set; }
		Shading Shading { get; }
		int FarEastLineBreakControl { get; set; }
		int WordWrap { get; set; }
		int HangingPunctuation { get; set; }
		int HalfWidthPunctuationOnTopOfLine { get; set; }
		int AddSpaceBetweenFarEastAndAlpha { get; set; }
		int AddSpaceBetweenFarEastAndDigit { get; set; }
		WdBaselineAlignment BaseLineAlignment { get; set; }
		int AutoAdjustRightIndent { get; set; }
		int DisableLineHeightGrid { get; set; }
		WdOutlineLevel OutlineLevel { get; set; }
		void CloseUp();
		void OpenUp();
		void OpenOrCloseUp();
		void TabHangingIndent(short Count);
		void TabIndent(short Count);
		void Reset();
		void Space1();
		void Space15();
		void Space2();
		void IndentCharWidth(short Count);
		void IndentFirstLineCharWidth(short Count);
		Paragraph Next(ref object Count);
		Paragraph Previous(ref object Count);
		void OutlinePromote();
		void OutlineDemote();
		void OutlineDemoteToBody();
		void Indent();
		void Outdent();
		float CharacterUnitRightIndent { get; set; }
		float CharacterUnitLeftIndent { get; set; }
		float CharacterUnitFirstLineIndent { get; set; }
		float LineUnitBefore { get; set; }
		float LineUnitAfter { get; set; }
		WdReadingOrder ReadingOrder { get; set; }
		string ID { get; set; }
		int SpaceBeforeAuto { get; set; }
		int SpaceAfterAuto { get; set; }
		bool IsStyleSeparator { get; }
		void SelectNumber();
		void ListAdvanceTo(short Level1, short Level2, short Level3, short Level4, short Level5, short Level6, short Level7, short Level8, short Level9);
		void ResetAdvanceTo();
		void SeparateList();
		void JoinList();
		int MirrorIndents { get; set; }
		WdTextboxTightWrap TextboxTightWrap { get; set; }
		short this[short Level] { get; }
	}
	#endregion
	#region Paragraphs
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Paragraphs : IWordObject, IEnumerable {
		int Count { get; }
		Paragraph First { get; }
		Paragraph Last { get; }
		ParagraphFormat Format { get; set; }
		TabStops TabStops { get; set; }
		Borders Borders { get; set; }
		object Style { get; set; }
		WdParagraphAlignment Alignment { get; set; }
		int KeepTogether { get; set; }
		int KeepWithNext { get; set; }
		int PageBreakBefore { get; set; }
		int NoLineNumber { get; set; }
		float RightIndent { get; set; }
		float LeftIndent { get; set; }
		float FirstLineIndent { get; set; }
		float LineSpacing { get; set; }
		WdLineSpacing LineSpacingRule { get; set; }
		float SpaceBefore { get; set; }
		float SpaceAfter { get; set; }
		int Hyphenation { get; set; }
		int WidowControl { get; set; }
		Shading Shading { get; }
		int FarEastLineBreakControl { get; set; }
		int WordWrap { get; set; }
		int HangingPunctuation { get; set; }
		int HalfWidthPunctuationOnTopOfLine { get; set; }
		int AddSpaceBetweenFarEastAndAlpha { get; set; }
		int AddSpaceBetweenFarEastAndDigit { get; set; }
		WdBaselineAlignment BaseLineAlignment { get; set; }
		int AutoAdjustRightIndent { get; set; }
		int DisableLineHeightGrid { get; set; }
		WdOutlineLevel OutlineLevel { get; set; }
		Paragraph this[int Index] { get; }
		Paragraph Add(ref object Range);
		void CloseUp();
		void OpenUp();
		void OpenOrCloseUp();
		void TabHangingIndent(short Count);
		void TabIndent(short Count);
		void Reset();
		void Space1();
		void Space15();
		void Space2();
		void IndentCharWidth(short Count);
		void IndentFirstLineCharWidth(short Count);
		void OutlinePromote();
		void OutlineDemote();
		void OutlineDemoteToBody();
		void Indent();
		void Outdent();
		float CharacterUnitRightIndent { get; set; }
		float CharacterUnitLeftIndent { get; set; }
		float CharacterUnitFirstLineIndent { get; set; }
		float LineUnitBefore { get; set; }
		float LineUnitAfter { get; set; }
		WdReadingOrder ReadingOrder { get; set; }
		int SpaceBeforeAuto { get; set; }
		int SpaceAfterAuto { get; set; }
		void IncreaseSpacing();
		void DecreaseSpacing();
	}
	#endregion
	#region WdParagraphAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdParagraphAlignment {
		wdAlignParagraphCenter = 1,
		wdAlignParagraphDistribute = 4,
		wdAlignParagraphJustify = 3,
		wdAlignParagraphJustifyHi = 7,
		wdAlignParagraphJustifyLow = 8,
		wdAlignParagraphJustifyMed = 5,
		wdAlignParagraphLeft = 0,
		wdAlignParagraphRight = 2,
		wdAlignParagraphThaiJustify = 9
	}
	#endregion
	#region WdBaselineAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdBaselineAlignment {
		wdBaselineAlignTop,
		wdBaselineAlignCenter,
		wdBaselineAlignBaseline,
		wdBaselineAlignFarEast50,
		wdBaselineAlignAuto
	}
	#endregion
	#region WdLineSpacing
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLineSpacing {
		wdLineSpaceSingle,
		wdLineSpace1pt5,
		wdLineSpaceDouble,
		wdLineSpaceAtLeast,
		wdLineSpaceExactly,
		wdLineSpaceMultiple
	}
	#endregion
	#region WdReadingOrder
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdReadingOrder {
		wdReadingOrderRtl,
		wdReadingOrderLtr
	}
	#endregion
	#region WdTextboxTightWrap
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTextboxTightWrap {
		wdTightNone,
		wdTightAll,
		wdTightFirstAndLastLines,
		wdTightFirstLineOnly,
		wdTightLastLineOnly
	}
	#endregion
	#region WdOutlineLevel
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOutlineLevel {
		wdOutlineLevel1 = 1,
		wdOutlineLevel2 = 2,
		wdOutlineLevel3 = 3,
		wdOutlineLevel4 = 4,
		wdOutlineLevel5 = 5,
		wdOutlineLevel6 = 6,
		wdOutlineLevel7 = 7,
		wdOutlineLevel8 = 8,
		wdOutlineLevel9 = 9,
		wdOutlineLevelBodyText = 10
	}
	#endregion
}
