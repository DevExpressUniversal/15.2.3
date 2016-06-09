﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
		#region SortAndFilter
		public static BarInfo SortAndFilter { get { return sortAndFilter; } }
		static readonly BarInfo sortAndFilter = new BarInfo(
			String.Empty,
			"Data",
			"Sort & Filter",
			new BarInfoItems(
				new string[] { "DataSortAscending", "DataSortDescending", "DataFilterToggle", "DataFilterClear", "DataFilterReApply" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageData",
			"Caption_GroupSortAndFilter"
		);
		#endregion
		#region DataTools
		public static BarInfo DataTools { get { return dataTools; } }
		static BarSubItemInfo dataValidationSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "DataValidation", "DataCircleValidationInvalidData", "DataClearValidationCircles" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static readonly BarInfo dataTools = new BarInfo(
			String.Empty,
			"Data",
			"Data Tools",
			new BarInfoItems(
				new string[] { "DataValidationCommandGroup" },
				new BarItemInfo[] { dataValidationSubItem }
			),
			String.Empty,
			String.Empty,
			"Caption_PageData",
			"Caption_GroupDataTools"
		);
		#endregion
		#region Outline
		public static BarInfo Outline { get { return outline; } }
		static BarSubItemInfo groupOutlineSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "DataGroupOutline", "DataAutoOutline" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static BarSubItemInfo ungroupOutlineSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "DataUngroupOutline", "DataClearOutline" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
		static readonly BarInfo outline = new BarInfo(
			String.Empty,
			"Data",
			"Outline",
			new BarInfoItems(
				new string[] { "DataOutlineGroupCommandGroup", "DataOutlineUngroupCommandGroup", "DataSubtotal", "DataShowDetail", "DataHideDetail" },
				new BarItemInfo[] { groupOutlineSubItem, ungroupOutlineSubItem, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			"DataOutlineSetting",
			String.Empty,
			"Caption_PageData",
			"Caption_GroupOutline"
		);
		#endregion
	}
}
