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
using System.Diagnostics;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellsChain
	public class CellsChain {
		ICell header;
		ICell footer;
		public ICell Header { get { return header; } }
		public ICell Footer { get { return footer; } }
		public void RegisterCell(ICell cell) {
			FormulaInfo formulaInfo = cell.FormulaInfo;
			Debug.Assert(formulaInfo != null);
			formulaInfo.NextCell = header;
			formulaInfo.PreviousCell = null;
			if (header != null)
				header.FormulaInfo.PreviousCell = cell;
			else
				footer = cell;
			header = cell;
#if DEBUGTEST
#endif
		}
		public void UnRegisterCell(ICell cell) {
			if (!cell.HasFormula)
				return;
			FormulaInfo chainCell = cell.FormulaInfo;
			if (chainCell.NextCell != null)
				chainCell.NextCell.FormulaInfo.PreviousCell = chainCell.PreviousCell;
			else
				footer = chainCell.PreviousCell;
			if (chainCell.PreviousCell != null)
				chainCell.PreviousCell.FormulaInfo.NextCell = chainCell.NextCell;
			else
				header = chainCell.NextCell;
		}
		internal void InsertAfter(ICell positionCell, ICell insertingCell) {
			if (positionCell == null) {
				header = insertingCell;
				footer = insertingCell;
				insertingCell.FormulaInfo.NextCell = null;
				insertingCell.FormulaInfo.PreviousCell = null;
			}
			else {
				insertingCell.FormulaInfo.PreviousCell = positionCell;
				insertingCell.FormulaInfo.NextCell = positionCell.FormulaInfo.NextCell;
				if (positionCell.FormulaInfo.NextCell != null)
					positionCell.FormulaInfo.NextCell.FormulaInfo.PreviousCell = insertingCell;
				else {
					footer = insertingCell;
					insertingCell.FormulaInfo.NextCell = null;
				}
				positionCell.FormulaInfo.NextCell = insertingCell;
			}
		}
		internal void Add(ICell cell) {
			if (footer == null)
				header = cell;
			else
				footer.FormulaInfo.NextCell = cell;
			cell.FormulaInfo.PreviousCell = footer;
			cell.FormulaInfo.NextCell = null;
			footer = cell;
		}
		internal void Reset() {
			header = null;
			footer = null;
		}
		public string Trace() {
			if (header == null)
				return string.Empty;
			StringBuilder builder = new StringBuilder();
			HashSet<ICell> processedCells = new HashSet<ICell>();
			ICell currentCell = header;
			while (currentCell != null) {
				builder.Append(currentCell.Position.ToString());
				builder.Append("->");
				if (processedCells.Contains(currentCell)) {
					builder.Append("CYCLE DETECTED!");
					break;
				}
				else
					processedCells.Add(currentCell);
				currentCell = currentCell.FormulaInfo.NextCell;
			}
			return builder.ToString();
		}
		internal void CheckIntegrity() {
			ICell previousCell = null;
			ICell currentCell = header;
			HashSet<ICell> processedCells = new HashSet<ICell>();
			while (currentCell != null) {
				if (!object.ReferenceEquals(currentCell.FormulaInfo.PreviousCell, previousCell)) {
					string actualCellPosition = currentCell.FormulaInfo.PreviousCell == null ? "null" : "[" + currentCell.FormulaInfo.PreviousCell.Worksheet.Name + "," + currentCell.FormulaInfo.PreviousCell.Position.ToString() + "]";
					string prevCellPosition = previousCell == null ? "null" : "[" + previousCell.Worksheet.Name + "," + previousCell.Position.ToString() + "]";
					throw new ArgumentException("Cell " + currentCell.Position.ToString() + " previous cell is invalid. Actual: " + actualCellPosition + ", expected: " + prevCellPosition + ".");
				}
				if (processedCells.Contains(currentCell))
					throw new ArgumentException("Cell " + currentCell.Position.ToString() + " exists in chain more than once.");
				else
					processedCells.Add(currentCell);
				previousCell = currentCell;
				currentCell = currentCell.FormulaInfo.NextCell;
			}
		}
	}
	public class FormulaInfo {
		byte[] binaryFormula;
		public ICell PreviousCell { get; set; }
		public ICell NextCell { get; set; }
		public byte[] BinaryFormula { get { return binaryFormula; } set { binaryFormula = value; } }
	}
	#endregion
}
