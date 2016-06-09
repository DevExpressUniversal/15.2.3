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

using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableGrandTotalsOffRowsColumnsCommand
	public class PivotTableGrandTotalsOffRowsColumnsCommand : LayoutPivotTableCommandBase {
		public PivotTableGrandTotalsOffRowsColumnsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableGrandTotalsOffRowsColumns; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOffRowsColumns; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "GrandTotalsOffRowsColumnsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.ColumnGrandTotals = false;
				table.RowGrandTotals = false;
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableGrandTotalsOnRowsColumnsCommand
	public class PivotTableGrandTotalsOnRowsColumnsCommand : LayoutPivotTableCommandBase {
		public PivotTableGrandTotalsOnRowsColumnsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableGrandTotalsOnRowsColumns; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnRowsColumns; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "GrandTotalsOnRowsColumnsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.ColumnGrandTotals = true;
				table.RowGrandTotals = true;
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableGrandTotalsOnRowsOnlyCommand
	public class PivotTableGrandTotalsOnRowsOnlyCommand : LayoutPivotTableCommandBase {
		public PivotTableGrandTotalsOnRowsOnlyCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableGrandTotalsOnRowsOnly; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnRowsOnly; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "GrandTotalsOnRowsOnlyPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.ColumnGrandTotals = false;
				table.RowGrandTotals = true;
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableGrandTotalsOnColumnsOnlyCommand
	public class PivotTableGrandTotalsOnColumnsOnlyCommand : LayoutPivotTableCommandBase {
		public PivotTableGrandTotalsOnColumnsOnlyCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableGrandTotalsOnColumnsOnly; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableGrandTotalsOnColumnsOnly; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string ImageName { get { return "GrandTotalsOnColumnsOnlyPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				table.ColumnGrandTotals = true;
				table.RowGrandTotals = false;
			}
			finally {
				table.EndTransaction();
			}
		}
	}
	#endregion
}
