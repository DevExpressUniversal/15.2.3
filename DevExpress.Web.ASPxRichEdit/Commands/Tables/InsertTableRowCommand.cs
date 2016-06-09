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

using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class InsertTableRowCommand : WebRichEditUpdateModelCommandBase {
		public InsertTableRowCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.InsertTableRow; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override void PerformModifyModel() {
			var table = GetTableByPositionInfo((ArrayList)Parameters["tablePosition"]);
			var newRowIndex = (int)Parameters["newRowIndex"];
			ArrayList clientCells = (ArrayList)Parameters["newRowCells"];
			if (table.Rows.Count > newRowIndex) {
				var nextRow = table.Rows[newRowIndex];
				var lastClientCell = (Hashtable)clientCells[clientCells.Count - 1];
				var lastClientCellEndLogPosition = new DocumentLogPosition((int)lastClientCell["endPosition"]);
				PieceTable.ChangeCellStartParagraphIndex(nextRow.FirstCell, PieceTable.FindParagraphIndex(lastClientCellEndLogPosition));
			}
			var newRow = new TableRow(table);
			table.Rows.AddRowCore(newRowIndex, newRow);
			foreach (Hashtable clientCell in clientCells) {
				var cell = CreateTableCell(newRow, newRow.Cells.Count, (int)clientCell["startPosition"], (int)clientCell["endPosition"], false);
				ApplyTableCellFormatting(cell, clientCell);
			}
			ApplyTableRowFormatting(newRow, (Hashtable)Parameters["newRowInfo"]);
		}
	}
}
