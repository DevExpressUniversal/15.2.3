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
	#region Find
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Find : IWordObject {
		bool Forward { get; set; }
		Font Font { get; set; }
		bool Found { get; }
		bool MatchAllWordForms { get; set; }
		bool MatchCase { get; set; }
		bool MatchWildcards { get; set; }
		bool MatchSoundsLike { get; set; }
		bool MatchWholeWord { get; set; }
		bool MatchFuzzy { get; set; }
		bool MatchByte { get; set; }
		ParagraphFormat ParagraphFormat { get; set; }
		object Style { get; set; }
		string Text { get; set; }
		WdLanguageID LanguageID { get; set; }
		int Highlight { get; set; }
		Replacement Replacement { get; }
		Frame Frame { get; }
		WdFindWrap Wrap { get; set; }
		bool Format { get; set; }
		WdLanguageID LanguageIDFarEast { get; set; }
		WdLanguageID LanguageIDOther { get; set; }
		bool CorrectHangulEndings { get; set; }
		bool ExecuteOld(ref object FindText, ref object MatchCase, ref object MatchWholeWord, ref object MatchWildcards, ref object MatchSoundsLike, ref object MatchAllWordForms, ref object Forward, ref object Wrap, ref object Format, ref object ReplaceWith, ref object Replace);
		void ClearFormatting();
		void SetAllFuzzyOptions();
		void ClearAllFuzzyOptions();
		bool Execute(ref object FindText, ref object MatchCase, ref object MatchWholeWord, ref object MatchWildcards, ref object MatchSoundsLike, ref object MatchAllWordForms, ref object Forward, ref object Wrap, ref object Format, ref object ReplaceWith, ref object Replace, ref object MatchKashida, ref object MatchDiacritics, ref object MatchAlefHamza, ref object MatchControl);
		int NoProofing { get; set; }
		bool MatchKashida { get; set; }
		bool MatchDiacritics { get; set; }
		bool MatchAlefHamza { get; set; }
		bool MatchControl { get; set; }
		bool MatchPhrase { get; set; }
		bool MatchPrefix { get; set; }
		bool MatchSuffix { get; set; }
		bool IgnoreSpace { get; set; }
		bool IgnorePunct { get; set; }
		bool HitHighlight(ref object FindText, ref object HighlightColor, ref object TextColor, ref object MatchCase, ref object MatchWholeWord, ref object MatchPrefix, ref object MatchSuffix, ref object MatchPhrase, ref object MatchWildcards, ref object MatchSoundsLike, ref object MatchAllWordForms, ref object MatchByte, ref object MatchFuzzy, ref object MatchKashida, ref object MatchDiacritics, ref object MatchAlefHamza, ref object MatchControl, ref object IgnoreSpace, ref object IgnorePunct, ref object HanjaPhoneticHangul);
		bool ClearHitHighlight();
		bool Execute2007(ref object FindText, ref object MatchCase, ref object MatchWholeWord, ref object MatchWildcards, ref object MatchSoundsLike, ref object MatchAllWordForms, ref object Forward, ref object Wrap, ref object Format, ref object ReplaceWith, ref object Replace, ref object MatchKashida, ref object MatchDiacritics, ref object MatchAlefHamza, ref object MatchControl, ref object MatchPrefix, ref object MatchSuffix, ref object MatchPhrase, ref object IgnoreSpace, ref object IgnorePunct);
		bool HanjaPhoneticHangul { get; set; }
	}
	#endregion
	#region WdFindWrap
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFindWrap {
		wdFindStop,
		wdFindContinue,
		wdFindAsk
	}
	#endregion
}
