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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region PageSetup
		public static BarInfo PageSetup { get { return pageSetup; } }
		static readonly BarInfo pageSetup = new BarInfo(
			String.Empty,
			"Page Layout",
			"Page Setup",
			new BarInfoItems(
				new string[] { "PageLayoutMargins", "PageLayoutOrientation", "PageLayoutSize", "PageLayoutColumns", "InsertBreak", "PageLayoutLineNumbering" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageLayoutNormalMargins", "PageLayoutNarrowMargins", "PageLayoutModerateMargins", "PageLayoutWideMargins", "PageLayoutPageMarginsOptions" },
						new BarItemInfo[] { BarItemInfos.PageMarginsCheck, BarItemInfos.PageMarginsCheck, BarItemInfos.PageMarginsCheck, BarItemInfos.PageMarginsCheck, BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageLayoutPortraitOrientation", "PageLayoutLandscapeOrientation" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { String.Empty, "PageLayoutPagePaperOptions" },
						new BarItemInfo[] { new PaperKindBarListItemInfo(), BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageLayoutOneColumn", "PageLayoutTwoColumns", "PageLayoutThreeColumns", "PageLayoutColumnsOptions" },
						new BarItemInfo[] { BarItemInfos.CheckLargeRibbonGlyph, BarItemInfos.CheckLargeRibbonGlyph, BarItemInfos.CheckLargeRibbonGlyph, BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "InsertPageBreak", "InsertColumnBreak", "InsertSectionBreakNextPage", "InsertSectionBreakContinuous", "InsertSectionBreakEvenPage", "InsertSectionBreakOddPage" },
						new BarItemInfo[] { BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageLayoutLineNumberingNone", "PageLayoutLineNumberingContinuous", "PageLayoutLineNumberingRestartNewPage", "PageLayoutLineNumberingRestartNewSection", "FormatParagraphSuppressLineNumbers", "PageLayoutLineNumberingOptions" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Button })
					)
				}
			),
			"PageLayoutPageOptions",
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupPageSetup"
		);
		#endregion
		#region PageBackground
		public static BarInfo PageBackground { get { return pageBackground; } }
		static readonly BarInfo pageBackground = new BarInfo(
			String.Empty,
			"Page Layout",
			"Page Background",
			new BarInfoItems(
				new string[] { "PageLayoutPageColor" },
				new BarItemInfo[] { new BarPageColorButtonItemInfo() }
			),
			String.Empty,
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupPageBackground"
		);
		#endregion
	}
}
