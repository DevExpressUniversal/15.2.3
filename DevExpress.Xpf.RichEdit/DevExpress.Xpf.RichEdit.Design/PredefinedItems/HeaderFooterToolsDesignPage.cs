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
		#region Navigation
		public static BarInfo HeaderFooterNavigation { get { return headerFooterNavigation; } }
		static readonly BarInfo headerFooterNavigation = new BarInfo(
			"Header & Footer Tools",
			"Design",
			"Navigation",
			new BarInfoItems(
				new string[] { "GoToHeader", "GoToFooter", "HeaderFooterGoToPrevious", "HeaderFooterGoToNext", "HeaderFooterLinkToPrevious" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Check }
			),
			String.Empty,
			"Caption_PageCategoryHeaderFooterTools",
			"Caption_PageHeaderFooterToolsDesign",
			"Caption_GroupHeaderFooterToolsDesignNavigation",
			"ToolsHeaderFooterCommandGroup"
		);
		#endregion
		#region Options
		public static BarInfo HeaderFooterOptions { get { return headerFooterOptions; } }
		static readonly BarInfo headerFooterOptions = new BarInfo(
			"Header & Footer Tools",
			"Design",
			"Options",
			new BarInfoItems(
				new string[] { "HeaderFooterDifferentFirstPage", "HeaderFooterDifferentOddEvenPages" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check }
			),
			String.Empty,
			"Caption_PageCategoryHeaderFooterTools",
			"Caption_PageHeaderFooterToolsDesign",
			"Caption_GroupHeaderFooterToolsDesignOptions",
			"ToolsHeaderFooterCommandGroup"
		);
		#endregion
		#region Close
		public static BarInfo HeaderFooterClose { get { return headerFooterClose; } }
		static readonly BarInfo headerFooterClose = new BarInfo(
			"Header & Footer Tools",
			"Design",
			"Close",
			new BarInfoItems(
				new string[] { "HeaderFooterClose" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			"Caption_PageCategoryHeaderFooterTools",
			"Caption_PageHeaderFooterToolsDesign",
			"Caption_GroupHeaderFooterToolsDesignClose",
			"ToolsHeaderFooterCommandGroup"
		);
		#endregion
	}
}
