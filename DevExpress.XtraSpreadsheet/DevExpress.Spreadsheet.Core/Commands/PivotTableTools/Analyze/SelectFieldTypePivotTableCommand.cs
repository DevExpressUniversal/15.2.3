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
namespace DevExpress.XtraSpreadsheet.Commands {
	public class SelectFieldTypePivotTableCommand : PivotTableCommandBase {
		public SelectFieldTypePivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.SelectFieldTypePivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_SelectFieldTypePivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_SelectFieldTypePivotTableDescription; } }
		public override string ImageName { get { return "FieldSettingsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			int fieldIndex = GetPivotFieldIndex(table);
			PivotFieldZoneInfo zoneInfo = table.CalculationInfo.GetActiveFieldInfo(ActiveSheet.Selection.ActiveCell);
			if (zoneInfo.FieldIndex == -1)
				return;
			if (zoneInfo.Axis == PivotTableAxis.Value)
				new DataFieldSettingsPivotTableCommand(Control).ExecuteCore();
			else
				new FieldSettingsPivotTableCommand(Control).ExecuteCore();
		}
		protected int GetPivotFieldIndex(PivotTable table) {
			if (table == null)
				return -1;
			return table.CalculationInfo.GetActiveFieldIndex(ActiveSheet.Selection.ActiveCell);
		}
		protected override bool GetEnabled(PivotTable table) {
			if (table == null)
				return false;
			int index = GetPivotFieldIndex(table);
			if (index == -1 || InnerControl.IsAnyInplaceEditorActive || !table.EnableFieldProperties)
				return false;
			return true;
		}
	}
}
