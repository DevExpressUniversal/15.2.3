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

using System.ComponentModel.Design;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	public class TableRowModifier {
		protected XRTableRow row;
		public TableRowModifier(XRTableRow row) {
			this.row = row;
		}
		public virtual void DeleteCell(XRTableCell cell) {
			if(row.Cells.Contains(cell))
				row.Cells.Remove(cell);
		}
		public void InsertCellByIndex(XRTableCell baseCell, XRTableCell newCell, int index, bool autoExpandTable, bool inheritBaseCellAppearance) {
			bool isLoading = row.IsLoading;
			if(!isLoading)
				row.Table.BeginInit();
			PreProcessInsertCell(newCell);
			try {
				if(baseCell == null) {
					if(newCell.Weight == 0.0)
						AssignCellWeight(newCell, WeightHelper.DefaultWeight);
				} else {
					if(!autoExpandTable) AssignCellWeight(baseCell, baseCell.Weight / 2.0);
					if(inheritBaseCellAppearance) newCell.AssignStyle(baseCell.InnerStyle);
					AssignCellWeight(newCell, baseCell.Weight);
				}
				InsertCell(newCell, index);
			} finally {
				if(!isLoading)
					row.Table.EndInit();
			}
		}
		protected virtual void AssignCellWeight(XRTableCell newCell, double weight) {
			newCell.Weight = weight;
		}
		protected virtual void PreProcessInsertCell(XRTableCell newCell) {
		}
		protected virtual void InsertCell(XRTableCell newCell, int index) {
			row.Cells.Insert(index, newCell);
		}
		public virtual void SetCellRange(XRTableCell[] tableCells) {
			row.Cells.Clear();
			if(tableCells.Length > 0)
				foreach(XRTableCell cell in tableCells)
					row.Cells.Add(cell);
		}
	}
}
