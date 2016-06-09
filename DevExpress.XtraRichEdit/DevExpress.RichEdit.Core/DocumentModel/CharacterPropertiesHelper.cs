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
namespace DevExpress.XtraRichEdit.Native {
	public static class CharacterPropertiesHelper {
		public static void ForEach(ICharacterPropertiesActions actions) {
			actions.AllCapsAction();
			actions.BackColorAction();
#if THEMES_EDIT
			actions.BackColorInfoAction();
			actions.FontInfoAction();
			actions.ForeColorInfoAction();
			actions.StrikeoutColorInfoAction();
			actions.UnderlineColorInfoAction(); 
			actions.ShadingAction();
#endif
			actions.FontBoldAction();
			actions.FontItalicAction();
			actions.FontNameAction();
			actions.FontSizeAction();
			actions.DoubleFontSizeAction();
			actions.FontStrikeoutTypeAction();
			actions.FontUnderlineTypeAction();
			actions.ForeColorAction();
			actions.HiddenAction();
			actions.ScriptAction();
			actions.StrikeoutColorAction();
			actions.StrikeoutWordsOnlyAction();
			actions.UnderlineColorAction();		  
			actions.UnderlineWordsOnlyAction();
			actions.LangInfoAction();
			actions.NoProofAction();
		}
	}
	public interface ICharacterPropertiesActions {
		void AllCapsAction();
		void BackColorAction();
#if THEMES_EDIT
		void BackColorInfoAction();
		void FontInfoAction();
		void ForeColorInfoAction();
		void StrikeoutColorInfoAction();
		void UnderlineColorInfoAction();
		void ShadingAction();
#endif
		void FontBoldAction();
		void FontItalicAction();
		void FontNameAction();
		void FontSizeAction();
		void DoubleFontSizeAction();
		void FontStrikeoutTypeAction();
		void FontUnderlineTypeAction();
		void ForeColorAction();
		void HiddenAction();
		void ScriptAction();
		void StrikeoutColorAction();
		void StrikeoutWordsOnlyAction();
		void UnderlineColorAction();
		void UnderlineWordsOnlyAction();
		void LangInfoAction();
		void NoProofAction();
	}
}
