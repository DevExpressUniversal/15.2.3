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

using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotChangeGrandTotalsCommandBase (abstract class)
	public abstract class PivotChangeGrandTotalsCommandBase : PivotTableTransactedCommand {
		readonly bool newGrandTotals;
		protected PivotChangeGrandTotalsCommandBase(IErrorHandler errorHandler, PivotTable table, bool newGrandTotals)
			: base(table, errorHandler) {
			this.newGrandTotals = newGrandTotals;
		}
		protected bool NewGrandTotals { get { return newGrandTotals; } }
		protected abstract bool OldGrandTotals { get; }
		protected abstract int ColumnRowItemCount { get; }
		protected internal override bool Validate() {
			if (OldGrandTotals == newGrandTotals)
				return false;
			return true;
		}
	}
	#endregion
	#region PivotChangeColumnGrandTotalsCommand
	public class PivotChangeColumnGrandTotalsCommand : PivotChangeGrandTotalsCommandBase {
		public PivotChangeColumnGrandTotalsCommand(IErrorHandler errorHandler, PivotTable table, bool newGrandTotals)
			: base(errorHandler, table, newGrandTotals) {
		}
		protected override bool OldGrandTotals { get { return PivotTable.ColumnGrandTotals; } }
		protected override int ColumnRowItemCount { get { return PivotTable.CalculationInfo.ColumnItems.Count; } }
		protected internal override void ExecuteCore() {
			PivotTable.SetColumnGrandTotals(NewGrandTotals);
		}
	}
	#endregion
	#region PivotChangeRowGrandTotalsCommand
	public class PivotChangeRowGrandTotalsCommand : PivotChangeGrandTotalsCommandBase {
		public PivotChangeRowGrandTotalsCommand(IErrorHandler errorHandler, PivotTable table, bool newGrandTotals)
			: base(errorHandler, table, newGrandTotals) {
		}
		protected override bool OldGrandTotals { get { return PivotTable.RowGrandTotals; } }
		protected override int ColumnRowItemCount { get { return PivotTable.CalculationInfo.RowItems.Count; } }
		protected internal override void ExecuteCore() {
			PivotTable.SetRowGrandTotals(NewGrandTotals);
		}
	}
	#endregion
}
