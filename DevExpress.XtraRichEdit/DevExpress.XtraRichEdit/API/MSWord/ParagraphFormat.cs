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
namespace DevExpress.XtraRichEdit.API.Word {
	#region ParagraphFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ParagraphFormat : IWordObject {
		ParagraphFormat Duplicate { get; }
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
		int FarEastLineBreakControl { get; set; }
		int WordWrap { get; set; }
		int HangingPunctuation { get; set; }
		int HalfWidthPunctuationOnTopOfLine { get; set; }
		int AddSpaceBetweenFarEastAndAlpha { get; set; }
		int AddSpaceBetweenFarEastAndDigit { get; set; }
		WdBaselineAlignment BaseLineAlignment { get; set; }
		int AutoAdjustRightIndent { get; set; }
		int DisableLineHeightGrid { get; set; }
		TabStops TabStops { get; set; }
		Borders Borders { get; set; }
		Shading Shading { get; }
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
		float CharacterUnitRightIndent { get; set; }
		float CharacterUnitLeftIndent { get; set; }
		float CharacterUnitFirstLineIndent { get; set; }
		float LineUnitBefore { get; set; }
		float LineUnitAfter { get; set; }
		WdReadingOrder ReadingOrder { get; set; }
		int SpaceBeforeAuto { get; set; }
		int SpaceAfterAuto { get; set; }
		int MirrorIndents { get; set; }
		WdTextboxTightWrap TextboxTightWrap { get; set; }
	}
	#endregion
}
