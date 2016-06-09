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
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class TableToolsToggleTotalRowCommand : ModifyTableStyleOptionsCommandBase {
		public TableToolsToggleTotalRowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_TableToolsToggleTotalRowCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_TableToolsToggleTotalRowCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.TableToolsToggleTotalRow; } }
		protected override bool ProtectionOption { get { return true; } }
		#endregion
		protected override bool IsChecked(Table table) {
			return table.HasTotalsRow;
		}
		protected override void ModifyTable(Table table) {
			ChangeTotalsRowSelection(table);
			table.ChangeTotals(!IsChecked(table), ErrorHandler);
		}
		void ChangeTotalsRowSelection(Table table) {
			IList<CellRange> activeRanges = ActiveSheet.Selection.SelectedRanges;
			if (table.HasTotalsRow && activeRanges.Count == 1) {
				CellRange activeRange = activeRanges[0];
				CellRange totalsRowRange = table.TryGetTotalsRowRange();
				if (table.Range.ContainsRange(activeRange) && totalsRowRange.Intersects(activeRange)) {
					if (totalsRowRange.ContainsRange(activeRange))
						ActiveSheet.Selection.SetSelection(activeRange.GetResized(0, -1, 0, -1));
					else
						ActiveSheet.Selection.SetSelection(activeRange.GetResized(0, 0, 0, -1));
				}
			}
		}
	}
}
