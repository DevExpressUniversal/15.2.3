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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableToggleRowHeadersCommand
	class PivotTableToggleRowHeadersCommand : ModifyPivotTableCommandBase {
		public PivotTableToggleRowHeadersCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableToggleRowHeaders; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleRowHeaders; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleRowHeadersDescription; } }
		#endregion
		protected override void ModifyPivotTable(Model.PivotTable table) {
			table.StyleInfo.ShowRowHeaders = !IsChecked(table);
		}
		protected override bool IsChecked(DevExpress.XtraSpreadsheet.Model.PivotTable table) {
			return table.StyleInfo.ShowRowHeaders ? true : false;
		}
	}
	#endregion
	#region PivotTableToggleColumnHeadersCommand
	class PivotTableToggleColumnHeadersCommand : ModifyPivotTableCommandBase {
		public PivotTableToggleColumnHeadersCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableToggleColumnHeaders; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleColumnHeaders; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleColumnHeadersDescription; } }
		#endregion
		protected override void ModifyPivotTable(Model.PivotTable table) {
			table.StyleInfo.ShowColumnHeaders = !IsChecked(table);
		}
		protected override bool IsChecked(DevExpress.XtraSpreadsheet.Model.PivotTable table) {
			return table.StyleInfo.ShowColumnHeaders ? true : false;
		}
	}
	#endregion
	#region PivotTableToggleBandedRowsCommand
	class PivotTableToggleBandedRowsCommand : ModifyPivotTableCommandBase {
		public PivotTableToggleBandedRowsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableToggleBandedRows; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedRows; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedRowsDescription; } }
		#endregion
		protected override void ModifyPivotTable(Model.PivotTable table) {
			table.StyleInfo.ShowRowStripes = !IsChecked(table);
		}
		protected override bool IsChecked(DevExpress.XtraSpreadsheet.Model.PivotTable table) {
			return table.StyleInfo.ShowRowStripes ? true : false;
		}
	}
	#endregion
	#region PivotTableToggleBandedColumnsCommand
	class PivotTableToggleBandedColumnsCommand : ModifyPivotTableCommandBase {
		public PivotTableToggleBandedColumnsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableToggleBandedColumns; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedColumns; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableToggleBandedColumnsDescription; } }
		#endregion
		protected override void ModifyPivotTable(Model.PivotTable table) {
			table.StyleInfo.ShowColumnStripes = !IsChecked(table);
		}
		protected override bool IsChecked(DevExpress.XtraSpreadsheet.Model.PivotTable table) {
			return table.StyleInfo.ShowColumnStripes ? true : false;
		}
	}
	#endregion
}
