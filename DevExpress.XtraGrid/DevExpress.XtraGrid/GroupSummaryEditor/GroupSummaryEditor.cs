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
using DevExpress.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Summary;
namespace DevExpress.XtraGrid.GroupSummaryEditor {
	public class GridSummaryItemsEditorController : SummaryItemsEditorController {
		public static string GetNameBySummaryType(SummaryItemType summaryType) {
			switch (summaryType) {
				case SummaryItemType.Average: return GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorSummaryAverage);
				case SummaryItemType.Count: return GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorSummaryCount);
				case SummaryItemType.Max: return GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorSummaryMax);
				case SummaryItemType.Min: return GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorSummaryMin);
				case SummaryItemType.Sum: return GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorSummarySum);
			}
			return string.Empty;
		}
		public GridSummaryItemsEditorController(ISummaryItemsOwner itemsOwner) : base(itemsOwner) {
		}
		protected override string GetTextBySummaryType(SummaryItemType summaryType) {
			return GetNameBySummaryType(summaryType);
		}
		protected override bool IsGroupSummaryItem(ISummaryItem item) {
			GridGroupSummaryItem gItem = item as GridGroupSummaryItem;
			if(gItem == null) return base.IsGroupSummaryItem(item);
			return gItem.ShowInGroupColumnFooter == null;
		}
	}
}
