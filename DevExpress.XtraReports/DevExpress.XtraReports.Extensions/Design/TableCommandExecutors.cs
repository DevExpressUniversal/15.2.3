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
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design.Commands
{
	internal abstract class TableCommandExecutor : CommandExecutorBase {
		static ArrayList deleteCommands = new ArrayList(new object[] { TableCommands.DeleteCell, TableCommands.DeleteColumn, TableCommands.DeleteRow });
		protected XRTableCell fCell;
		protected XRTableRow fRow;
		protected XRTable fTable;
		protected CommandID cmdID;
		public TableCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			fRow = GetSelectedRow();
			fCell = GetSelectedCell();
			if(fCell != null) fRow = fCell.Row;
			if(fRow == null) return;
			fTable = fRow.Table;
			this.cmdID = cmdID;
			if((cmdID == TableCommands.DeleteCell || (cmdID == TableCommands.DeleteColumn && fRow.IsSingleChild)) && fCell.IsSingleChild) {
				new TableRowCommandExecutor(designerHost).ExecCommand(TableCommands.DeleteRow);
				return;
			}
			string descr = deleteCommands.Contains(cmdID) ? DesignSR.Trans_Delete : DesignSR.Trans_CreateComponents;
			DesignerTransaction trans = designerHost.CreateTransaction(descr);
			try {
				ModifyControl();
			} catch {
				trans.Cancel();
			} finally {
				trans.Commit();
			}
		}
		protected abstract void ModifyControl();
		XRTableRow GetSelectedRow() {
			ArrayList comps = GetSelectedComponents();
			return (comps.Count == 1) ? comps[0] as XRTableRow : null;
		}
		XRTableCell GetSelectedCell() {
			ArrayList comps = GetSelectedComponents();
			return (comps.Count == 1) ? comps[0] as XRTableCell : null;
		}
	}
	internal class TableCellCommandExecutor : TableCommandExecutor {
		public TableCellCommandExecutor(IDesignerHost host) : base(host) {
		} 
		public override void ExecCommand(CommandID cmdID) { 
			base.ExecCommand(cmdID);
		}
		protected override void ModifyControl() {
			if(cmdID == TableCommands.DeleteCell) {
				new CellDisposer(changeServ).DisposeJustCell(fCell);
			} else if(cmdID == TableCommands.InsertCell)
				fRow.InsertCell(new XRTableCell(), fCell.Index, true);
		}
	}
	internal class TableRowCommandExecutor : TableCommandExecutor {
		public TableRowCommandExecutor(IDesignerHost host) : base(host) {
		} 
		protected override void ModifyControl() {
			if(cmdID == TableCommands.DeleteRow)
				fTable.DeleteRow(fRow);
			else if(cmdID == TableCommands.InsertRowAbove)
				fTable.InsertRowAbove(fRow);
			else if(cmdID == TableCommands.InsertRowBelow)
				fTable.InsertRowBelow(fRow);
		}
	}
	internal class TableColumnCommandExecutor : TableCommandExecutor {
		public TableColumnCommandExecutor(IDesignerHost host) : base(host) {
		} 
		protected override void ModifyControl() {
			if(cmdID == TableCommands.DeleteColumn)
				fTable.DeleteColumn(fCell, true);
			else if(cmdID == TableCommands.InsertColumnToLeft)
				fTable.InsertColumnToLeft(fCell, true, true);
			else if(cmdID == TableCommands.InsertColumnToRight)
				fTable.InsertColumnToRight(fCell, true, true);
		}
	}
	internal class TableCommonCommandExecutor : CommandExecutorBase {
		public TableCommonCommandExecutor(IDesignerHost host)
			: base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			string descr = DesignSR.Trans_ConvertToLabels;
			DesignerTransaction trans = designerHost.CreateTransaction(descr);
			try {
				ArrayList comps = GetSelectedComponents();
				List<XRTable> tables = new List<XRTable>(); 
				foreach(object o in comps) { 
					XRTable table = GetTable(o);
					if(table != null && !tables.Contains(table)) {
						table.ConvertToControls();
						tables.Add(table);
					}
				}
			} finally {
				trans.Commit();
			}
		}
		static XRTable GetTable(object o) {
			XRTable table = o as XRTable;
			if(table != null)
				return table;
			XRTableRow row = o as XRTableRow;
			if(row != null)
				return row.Table;
			XRTableCell cell = o as XRTableCell;
			if(cell != null) {
				row = cell.Row;
				if(row != null)
					return row.Table;
			}
			return null;
		}
	}
}
