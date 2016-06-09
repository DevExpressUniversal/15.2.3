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

using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class RemoveGrandTotalPivotTableCommand : PivotTablePopupMenuCommandBase {
		public RemoveGrandTotalPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RemoveGrandTotalPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveGrandTotalPivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveGrandTotalPivotTableDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = ActiveSheet.TryGetPivotTable(ActiveCell);
			if (table == null)
				return;
			PivotZone zone = table.CalculationInfo.GetPivotZoneByCellPosition(ActiveCell);
			if (IsColumnGrandTotal(zone)) 
				ModifyGrandTotal(table, true, false);
			if (IsRowGrandTotal(zone)) 
				ModifyGrandTotal(table, false, true);
		}
		void ModifyGrandTotal(PivotTable table, bool firstValue, bool secondValue) {
			table.BeginTransaction(ErrorHandler);
			try {
				table.ColumnGrandTotals = firstValue;
				table.RowGrandTotals = secondValue;
			}
			finally {
				table.EndTransaction();
			}
		}
		protected override bool IsVisible() {
			return GetState();
		}
		protected override bool IsEnabled() {
			return GetState();
		}
		bool GetState() {
			PivotTable table = ActiveSheet.TryGetPivotTable(ActiveCell);
			if (table == null)
				return false;
			PivotZone zone = table.CalculationInfo.GetPivotZoneByCellPosition(ActiveCell);
			return IsColumnGrandTotal(zone) || IsRowGrandTotal(zone);
		}
		bool IsColumnGrandTotal(PivotZone zone) {
			return zone.Type.HasFlag(PivotZoneType.GrandTotalColumn) && zone.Type.HasFlag(PivotZoneType.ColumnHeader);
		}
		bool IsRowGrandTotal(PivotZone zone) {
			return zone.Type.HasFlag(PivotZoneType.GrandTotalRow) && zone.Type.HasFlag(PivotZoneType.RowHeader);
		}
	}
}
