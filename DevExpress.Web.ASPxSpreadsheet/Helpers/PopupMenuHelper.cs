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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public static class SpreadsheetPopupMenuItemsProvider {
		public static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> RootPopupCommandIds =
			new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID>() 
				{
					{ SpreadsheetCommandId.CutSelection, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.CopySelection, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.PasteSelection, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.FormatClearContents, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.InsertHyperlinkContextMenuItem, WebSpreadsheetCommandID.InsertHyperlink },
					{ SpreadsheetCommandId.EditHyperlinkContextMenuItem, WebSpreadsheetCommandID.EditHyperlink },
					{ SpreadsheetCommandId.RemoveHyperlinkContextMenuItem, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.HideColumns, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.HideRows, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.UnhideColumns, WebSpreadsheetCommandID.UnhideColumns },
					{ SpreadsheetCommandId.UnhideRows, WebSpreadsheetCommandID.UnhideRows },
					{ SpreadsheetCommandId.ChartChangeType, WebSpreadsheetCommandID.ChartChangeType },
					{ SpreadsheetCommandId.ChartSelectData, WebSpreadsheetCommandID.ChartSelectData },	  
					{ SpreadsheetCommandId.ModifyChartLayout, WebSpreadsheetCommandID.ModifyChartLayout },
					{ SpreadsheetCommandId.ModifyChartStyle, WebSpreadsheetCommandID.ModifyChartStyle },
					{ SpreadsheetCommandId.ChartChangeTitleContextMenuItem, WebSpreadsheetCommandID.ChartChangeTitle },
					{ SpreadsheetCommandId.ChartChangeHorizontalAxisTitleContextMenuItem, WebSpreadsheetCommandID.ChartChangeHorizontalAxisTitle },
					{ SpreadsheetCommandId.ChartChangeVerticalAxisTitleContextMenuItem, WebSpreadsheetCommandID.ChartChangeVerticalAxisTitle },
					{ SpreadsheetCommandId.ArrangeBringForwardCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.ArrangeSendBackwardCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataSortAscending, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataSortDescending, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterColumnClear, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDateFiltersCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterTextFiltersCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterNumberFiltersCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterSimple, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.InsertSheet, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.RemoveSheet, WebSpreadsheetCommandID.RemoveSheet },
					{ SpreadsheetCommandId.RenameSheet, WebSpreadsheetCommandID.RenameSheet },
					{ SpreadsheetCommandId.MoveOrCopySheet, WebSpreadsheetCommandID.MoveOrCopySheetWebCommand },
					{ SpreadsheetCommandId.HideSheet, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.UnhideSheet, WebSpreadsheetCommandID.UnhideSheet }
				};
		private static List<SpreadsheetCommandId> NewGroupItems = new List<SpreadsheetCommandId> {
				SpreadsheetCommandId.FormatClearContents,
				SpreadsheetCommandId.ChartChangeType,
				SpreadsheetCommandId.ArrangeBringForwardCommandGroup,
				SpreadsheetCommandId.ChartChangeTitleContextMenuItem,
				SpreadsheetCommandId.DataFilterDateBefore,
				SpreadsheetCommandId.DataFilterTomorrow,
				SpreadsheetCommandId.DataFilterNextWeek,
				SpreadsheetCommandId.DataFilterNextMonth,
				SpreadsheetCommandId.DataFilterNextQuarter,
				SpreadsheetCommandId.DataFilterNextYear,
				SpreadsheetCommandId.DataFilterYearToDate,
				SpreadsheetCommandId.DataFilterBeginsWith,
				SpreadsheetCommandId.DataFilterContains,
				SpreadsheetCommandId.DataFilterCustom,
				SpreadsheetCommandId.DataFilterGreaterThan,
				SpreadsheetCommandId.DataFilterTop10,
				SpreadsheetCommandId.DataFilterSimple,
				SpreadsheetCommandId.DataFilterColumnClear, 
				SpreadsheetCommandId.DataFilterDateBefore,
				SpreadsheetCommandId.DataFilterTomorrow,
				SpreadsheetCommandId.DataFilterNextWeek,
				SpreadsheetCommandId.DataFilterNextMonth,
				SpreadsheetCommandId.DataFilterNextQuarter, 
				SpreadsheetCommandId.DataFilterNextYear,
				SpreadsheetCommandId.DataFilterYearToDate,
				SpreadsheetCommandId.DataFilterMonthJanuary,
				SpreadsheetCommandId.DataFilterDateCustom
			};
		public static bool IsBeginGroup(SpreadsheetCommandId commandId) {
			return NewGroupItems.Contains(commandId);
		}
		public static string GetMenuItemName(KeyValuePair<SpreadsheetCommandId, WebSpreadsheetCommandID> commandId) {
			if(commandId.Value != WebSpreadsheetCommandID.None)
				return WebSpreadsheetCommands.GetWebCommandName(commandId.Value);
			return SpreadsheetRibbonHelper.GetCommandName(commandId.Key);
		}
		#region CreateSubMenuItems
		public static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateSubMenuItems(SpreadsheetCommandId commandId) {
			if(commandId == SpreadsheetCommandId.ArrangeBringForwardCommandGroup)
				return CreateForwardSubMenu();
			if(commandId == SpreadsheetCommandId.ArrangeSendBackwardCommandGroup)
				return CreateBackwardSubMenu();
			if(commandId == SpreadsheetCommandId.DataFilterTextFiltersCommandGroup)
				return CreateTextFiltersSubMenu();
			if(commandId == SpreadsheetCommandId.DataFilterDateFiltersCommandGroup)
				return CreateDateFiltersSubMenu();
			if(commandId == SpreadsheetCommandId.DataFilterAllDatesInPeriodCommandGroup)
				return CreateAllDatesInPeriodSubMenu();
			if(commandId == SpreadsheetCommandId.DataFilterNumberFiltersCommandGroup)
				return CreateNumberFiltersSubMenu();
			return null;
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateForwardSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.ArrangeBringForward, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.ArrangeBringToFront, WebSpreadsheetCommandID.None }   
				};
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateBackwardSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.ArrangeSendBackward, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.ArrangeSendToBack, WebSpreadsheetCommandID.None }		   
				};
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateDateFiltersSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.DataFilterDateEquals, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDateBefore, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDateAfter, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDateBetween, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterTomorrow, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterToday, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterYesterday, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterNextWeek, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterThisWeek, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLastWeek, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterNextMonth, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterThisMonth, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLastMonth, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterNextQuarter, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterThisQuarter, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLastQuarter, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterNextYear, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterThisYear, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLastYear, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterYearToDate, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterAllDatesInPeriodCommandGroup, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDateCustom, WebSpreadsheetCommandID.None }
				};
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateAllDatesInPeriodSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.DataFilterQuarter1, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterQuarter2, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterQuarter3, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterQuarter4, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthJanuary, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthFebruary, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthMarch, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthMay, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthJune, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthJuly, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthAugust, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthSeptember, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthOctober, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthNovember, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterMonthDecember, WebSpreadsheetCommandID.None }
				};
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateTextFiltersSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.DataFilterEquals, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDoesNotEqual, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterBeginsWith, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterEndsWith, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterContains, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDoesNotContain, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterCustom, WebSpreadsheetCommandID.None }
				};
		}
		private static Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> CreateNumberFiltersSubMenu() {
			return new Dictionary<SpreadsheetCommandId, WebSpreadsheetCommandID> {
					{ SpreadsheetCommandId.DataFilterEquals, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterDoesNotEqual, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterGreaterThan, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterGreaterThanOrEqualTo, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLessThan, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterLessThanOrEqualTo, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterBetween, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterTop10, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterAboveAverage, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterBelowAverage, WebSpreadsheetCommandID.None },
					{ SpreadsheetCommandId.DataFilterCustom, WebSpreadsheetCommandID.None }
				};
		}
		#endregion
	}
}
