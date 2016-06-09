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
	#region WdAnimation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdAnimation {
		wdAnimationNone,
		wdAnimationLasVegasLights,
		wdAnimationBlinkingBackground,
		wdAnimationSparkleText,
		wdAnimationMarchingBlackAnts,
		wdAnimationMarchingRedAnts,
		wdAnimationShimmer
	}
	#endregion
	#region WdUnderline
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdUnderline {
		wdUnderlineDash = 7,
		wdUnderlineDashHeavy = 0x17,
		wdUnderlineDashLong = 0x27,
		wdUnderlineDashLongHeavy = 0x37,
		wdUnderlineDotDash = 9,
		wdUnderlineDotDashHeavy = 0x19,
		wdUnderlineDotDotDash = 10,
		wdUnderlineDotDotDashHeavy = 0x1a,
		wdUnderlineDotted = 4,
		wdUnderlineDottedHeavy = 20,
		wdUnderlineDouble = 3,
		wdUnderlineNone = 0,
		wdUnderlineSingle = 1,
		wdUnderlineThick = 6,
		wdUnderlineWavy = 11,
		wdUnderlineWavyDouble = 0x2b,
		wdUnderlineWavyHeavy = 0x1b,
		wdUnderlineWords = 2
	}
	#endregion
	#region WdEmphasisMark
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdEmphasisMark {
		wdEmphasisMarkNone,
		wdEmphasisMarkOverSolidCircle,
		wdEmphasisMarkOverComma,
		wdEmphasisMarkOverWhiteCircle,
		wdEmphasisMarkUnderSolidCircle
	}
	#endregion
	#region Font
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Font : IWordObject {
		Font Duplicate { get; }
		int Bold { get; set; }
		int Italic { get; set; }
		int Hidden { get; set; }
		int SmallCaps { get; set; }
		int AllCaps { get; set; }
		int StrikeThrough { get; set; }
		int DoubleStrikeThrough { get; set; }
		WdColorIndex ColorIndex { get; set; }
		int Subscript { get; set; }
		int Superscript { get; set; }
		WdUnderline Underline { get; set; }
		float Size { get; set; }
		string Name { get; set; }
		int Position { get; set; }
		float Spacing { get; set; }
		int Scaling { get; set; }
		int Shadow { get; set; }
		int Outline { get; set; }
		int Emboss { get; set; }
		float Kerning { get; set; }
		int Engrave { get; set; }
		WdAnimation Animation { get; set; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		WdEmphasisMark EmphasisMark { get; set; }
		bool DisableCharacterSpaceGrid { get; set; }
		string NameFarEast { get; set; }
		string NameAscii { get; set; }
		string NameOther { get; set; }
		void Grow();
		void Shrink();
		void Reset();
		void SetAsTemplateDefault();
		WdColor Color { get; set; }
		int BoldBi { get; set; }
		int ItalicBi { get; set; }
		float SizeBi { get; set; }
		string NameBi { get; set; }
		WdColorIndex ColorIndexBi { get; set; }
		WdColor DiacriticColor { get; set; }
		WdColor UnderlineColor { get; set; }
	}
	#endregion
}
