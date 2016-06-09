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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IWorkBookCellsWalker
	public interface IWorkBookCellsWalker {
		void Walk(DocumentModel workBook);
		void Walk(Worksheet sheet);
		void Walk(Row row);
		void Walk(ICell cell);
	}
	#endregion
	#region WorkBookCellsWalker
	public abstract class WorkBookCellsWalker : IWorkBookCellsWalker {
		public virtual void Walk(DocumentModel workBook) {
			workBook.Sheets.ForEach(Walk);
		}
		public virtual void Walk(Worksheet sheet) {
			sheet.Workbook.DataContext.PushCurrentWorksheet(sheet);
			try {
				sheet.Rows.ForEach(Walk);
			}
			finally {
				sheet.Workbook.DataContext.PopCurrentWorksheet();
			}
		}
		public virtual void Walk(Row row) {
			if (row.CellsCount > 0)
				row.Cells.ForEach(Walk);
		}
		public abstract void Walk(ICell cell);
	}
	#endregion
	#region ResetCellContentVersionCellsWalker
	public class ResetCellContentVersionCellsWalker : WorkBookCellsWalker {
		readonly int initialContentVersion;
		public ResetCellContentVersionCellsWalker(int initialContentVersion) {
			this.initialContentVersion = initialContentVersion;
		}
		public override void Walk(ICell cell) {
			if(cell.ContentVersion != 0x3FFF)
				cell.ContentVersion = initialContentVersion;
		}
	}
	#endregion
	#region ForceRecalculationCellsWalker
	public class ForceRecalculationCellsWalker : WorkBookCellsWalker {
		public override void Walk(ICell cell) {
			cell.ContentVersion = 0x3FFF;
		}
	}
	#endregion
}
