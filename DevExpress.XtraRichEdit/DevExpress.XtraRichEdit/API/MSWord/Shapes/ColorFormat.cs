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
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region ColorFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ColorFormat : IWordObject {
		int RGB { get; set; }
		int SchemeColor { get; set; }
		MsoColorType Type { get; }
		string Name { get; set; }
		float TintAndShade { get; set; }
		MsoTriState OverPrint { get; set; }
		float this[int Index] { get; set; }
		int Cyan { get; set; }
		int Magenta { get; set; }
		int Yellow { get; set; }
		int Black { get; set; }
		void SetCMYK(int Cyan, int Magenta, int Yellow, int Black);
		WdThemeColorIndex ObjectThemeColor { get; set; }
	}
	#endregion
	#region WdThemeColorIndex
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdThemeColorIndex {
		wdNotThemeColor = -1,
		wdThemeColorAccent1 = 4,
		wdThemeColorAccent2 = 5,
		wdThemeColorAccent3 = 6,
		wdThemeColorAccent4 = 7,
		wdThemeColorAccent5 = 8,
		wdThemeColorAccent6 = 9,
		wdThemeColorBackground1 = 12,
		wdThemeColorBackground2 = 14,
		wdThemeColorHyperlink = 10,
		wdThemeColorHyperlinkFollowed = 11,
		wdThemeColorMainDark1 = 0,
		wdThemeColorMainDark2 = 2,
		wdThemeColorMainLight1 = 1,
		wdThemeColorMainLight2 = 3,
		wdThemeColorText1 = 13,
		wdThemeColorText2 = 15
	}
	#endregion
}
