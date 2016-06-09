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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableFieldSortCommandGroup
	public class PivotTableFieldSortCommandGroup : PivotTablePopupMenuCommandGroupBase {
		public PivotTableFieldSortCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableFieldSortCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableSortCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableSortCommandGroupDescription; } }
		#endregion
		protected override bool IsVisible() {
			PivotZone zone = PivotTable.CalculationInfo.GetPivotZoneByCellPosition(ActiveSheet.Selection.ActiveCell);
			return zone.GetActiveSortFieldIndex() != -1;
		}
	}
	#endregion
	#region PivotTableFieldSortCommandBase
	public abstract class PivotTableFieldSortCommandBase : PivotTablePopupMenuCommandBase {
		protected PivotTableFieldSortCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return GetMenuStringId(); } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return GetDescriptionStringId(); } }
		protected override int FieldIndex { get { return GetSortFieldIndex(); } }
		protected abstract PivotTableSortTypeField SortType { get; }
		public bool IsPivotTableContextMenu { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			ApplyPivotTableSortCommand command = new ApplyPivotTableSortCommand(PivotTable, FieldIndex, SortType, Control.InnerControl.ErrorHandler);
			command.Execute();
		}
		protected override bool IsChecked() {
			if (FieldIndex < 0)
				return false;
			return Field.SortType == SortType;
		}
		protected abstract XtraSpreadsheetStringId GetMenuStringId();
		protected abstract XtraSpreadsheetStringId GetDescriptionStringId();
		protected XtraSpreadsheetStringId GetStringId(XtraSpreadsheetStringId any, XtraSpreadsheetStringId onlyNumbers, XtraSpreadsheetStringId onlyDates) {
			if (Info == null || FieldIndex < 0)
				return any;
			PivotCacheSharedItemsCollection sharedItems = PivotTable.Cache.CacheFields[FieldIndex].SharedItems;
			if (sharedItems.ContainsNonDate)
				return sharedItems.ContainsOnlyNumbers ? onlyNumbers : any;
			return onlyDates;
		}
		int GetSortFieldIndex() {
			if (IsPivotTableContextMenu) {
				PivotZone zone = PivotTable.CalculationInfo.GetPivotZoneByCellPosition(ActiveCell);
				return zone.GetActiveSortFieldIndex();
			}
			return base.FieldIndex;
		}
	}
	#endregion
	#region PivotTableFieldSortAscendingCommand
	public class PivotTableFieldSortAscendingCommand : PivotTableFieldSortCommandBase {
		public PivotTableFieldSortAscendingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override string ImageName { get { return "SortAsc"; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableFieldSortAscending; } }
		protected override PivotTableSortTypeField SortType { get { return PivotTableSortTypeField.Ascending; } }
		#endregion
		protected override XtraSpreadsheetStringId GetMenuStringId() {
			return GetStringId(XtraSpreadsheetStringId.MenuCmd_SortAscending,
							   XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyNumbers,
							   XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyDates);
		}
		protected override XtraSpreadsheetStringId GetDescriptionStringId() {
			return GetStringId(XtraSpreadsheetStringId.MenuCmd_SortAscendingDescription,
							   XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyNumbersDescription,
							   XtraSpreadsheetStringId.MenuCmd_SortAscendingOnlyDatesDescription);
		}
	}
	#endregion
	#region PivotTableFieldSortDescendingCommand
	public class PivotTableFieldSortDescendingCommand : PivotTableFieldSortCommandBase {
		public PivotTableFieldSortDescendingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override string ImageName { get { return "SortDesc"; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableFieldSortDescending; } }
		protected override PivotTableSortTypeField SortType { get { return PivotTableSortTypeField.Descending; } }
		#endregion
		protected override XtraSpreadsheetStringId GetMenuStringId() {
			return GetStringId(XtraSpreadsheetStringId.MenuCmd_SortDescending,
							   XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyNumbers,
							   XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyDates);
		}
		protected override XtraSpreadsheetStringId GetDescriptionStringId() {
			return GetStringId(XtraSpreadsheetStringId.MenuCmd_SortDescendingDescription,
							   XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyNumbersDescription,
							   XtraSpreadsheetStringId.MenuCmd_SortDescendingOnlyDatesDescription);
		}
	}
	#endregion
}
