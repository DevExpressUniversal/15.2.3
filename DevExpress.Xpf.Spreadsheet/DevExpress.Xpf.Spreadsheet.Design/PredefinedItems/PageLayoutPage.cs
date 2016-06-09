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
namespace DevExpress.Xpf.Spreadsheet.Design {
	public static partial class BarInfos {
		#region PageSetup
		public static BarInfo PageSetup { get { return pageSetup; } }
		static readonly BarInfo pageSetup = new BarInfo(
			String.Empty,
			"Page Layout",
			"Page Setup",
			new BarInfoItems(
				new string[] { "PageSetupMarginsCommandGroup", "PageSetupOrientationCommandGroup", "PageSetupPaperKindCommandGroup", "PageSetupPrintAreaCommandGroup" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageSetupMarginsNormal", "PageSetupMarginsWide", "PageSetupMarginsNarrow", "PageSetupCustomMargins" },
						new BarItemInfo[] { BarItemInfos.PageMarginsCheck, BarItemInfos.PageMarginsCheck, BarItemInfos.PageMarginsCheck, BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageSetupOrientationPortrait", "PageSetupOrientationLandscape" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { String.Empty, "PageSetupMorePaperSizes" },
						new BarItemInfo[] { new PaperKindBarListItemInfo(), BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PageSetupSetPrintArea", "PageSetupClearPrintArea", "PageSetupAddPrintArea" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
				}
			),
			"PageSetupPage", 
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupPageSetup"
		);
		#endregion
		#region Show
		public static BarInfo PageLayoutShow { get { return pageLayoutShow; } }
		static readonly BarInfo pageLayoutShow = new BarInfo(
			String.Empty,
			"Page Layout",
			"Show",
			new BarInfoItems(
				new string[] { "ViewShowGridlines", "ViewShowHeadings" },
				new BarItemInfo[] { BarItemInfos.CheckEditItem, BarItemInfos.CheckEditItem }
			),
			String.Empty,
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupShow"
		);
		#endregion
		#region Print
		public static BarInfo PageLayoutPrint { get { return pageLayoutPrint; } }
		static readonly BarInfo pageLayoutPrint = new BarInfo(
			String.Empty,
			"Page Layout",
			"Print",
			new BarInfoItems(
				new string[] { "PageSetupPrintGridlines", "PageSetupPrintHeadings" },
				new BarItemInfo[] { BarItemInfos.CheckEditItem, BarItemInfos.CheckEditItem }
			),
			"PageSetupSheet",
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupPrint"
		);
		#endregion
		#region PageLayoutArrange
		public static BarInfo PageLayoutArrange { get { return pageLayoutArrange; } }
		static readonly BarInfo pageLayoutArrange = new BarInfo(
			String.Empty,
			"Page Layout",
			"Arrange",
			new BarInfoItems(
				new string[] { "ArrangeBringForwardCommandGroup", "ArrangeSendBackwardCommandGroup" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "ArrangeBringForward", "ArrangeBringToFront", },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }),
					"PageLayout"),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "ArrangeSendBackward", "ArrangeSendToBack" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }),
					"PageLayout"),
				}
			),
			String.Empty, 
			String.Empty,
			"Caption_PagePageLayout",
			"Caption_GroupArrange"
		);
		#endregion
	}
}
