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
using System;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class SubtotalPivotFieldCommand : PivotTablePopupMenuCommandBase {
		public SubtotalPivotFieldCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.SubtotalPivotField; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string MenuCaption { get { return GetFormattedCaption(base.MenuCaption); } }
		#endregion
		protected internal override void ExecuteCore() {
			if (PivotTable == null)
				return;
			PivotTable.BeginTransaction(ErrorHandler);
			try {
				PivotTable.Fields[FieldIndex].DefaultSubtotal = !IsChecked();
			}
			finally {
				PivotTable.EndTransaction();
			}
		}
		protected override bool IsEnabled() {
			return GetState();
		}
		protected override bool IsVisible() {
			return GetState();
		}
		protected override bool IsChecked() {
			if (FieldIndex == -1)
				return base.IsChecked();
			return PivotTable.Fields[FieldIndex].DefaultSubtotal;
		}
		bool GetState() {
			PivotTable table = ActiveSheet.TryGetPivotTable(ActiveCell);
			if (table == null)
				return false;
			PivotFieldZoneInfo zone = table.CalculationInfo.GetActiveFieldInfo(ActiveSheet.Selection.ActiveCell);
			return IsColumnField(zone) || IsRowField(zone);
		}
		bool IsRowField(PivotFieldZoneInfo zone) {
			return zone.FieldIndex != -1 && zone.Axis == PivotTableAxis.Row;
		}
		bool IsColumnField(PivotFieldZoneInfo zone) {
			return zone.FieldIndex != -1 && zone.Axis == PivotTableAxis.Column;
		}
		string GetFormattedCaption(string caption) {
			if (Axis == PivotTableAxis.None)
				return caption;
			return String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_SubtotalPivotField), FieldName);
		}
	}
}
