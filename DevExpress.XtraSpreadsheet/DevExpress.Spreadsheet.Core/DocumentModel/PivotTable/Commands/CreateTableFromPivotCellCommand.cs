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
using System.Linq;
namespace DevExpress.XtraSpreadsheet.Model {
	public class CreateTableFromPivotCellCommand : PivotTableTransactedCommand {
		CellPosition activeCell;
		public CreateTableFromPivotCellCommand(PivotTable pivotTable, CellPosition activeCell, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
				this.activeCell = activeCell;
		}
		protected internal override void ExecuteCore() {
			Worksheet sheet = DocumentModel.CreateWorksheet();
			InitTableHeader(sheet);
			int rowCount = InitTableData(sheet, GetSharedIndices(), PivotTable.GetKeyFieldIndices());
			CellRange range = CreateRange(sheet, rowCount);
			InsertTableCommand command = new InsertTableCommand(ErrorHandler, sheet, String.Empty, true, true);
			command.Range = range;
			command.ExecuteCore();
			DocumentModel.Sheets.Add(sheet);
			DocumentModel.ActiveSheet = sheet;
			sheet.Selection.SetSelection(range);
		}
		#region SetUp
		PivotDataKey GetSharedIndices() { 
			List<int> result = new List<int>();
			result.Add(0);
			result.Add(-1);
			result.Add(1);
			return new PivotDataKey(result.ToArray());
		}
		CellRange CreateRange(Worksheet sheet, int row) {
			CellPosition topLeft = new CellPosition(0, 0);
			CellPosition bottomRight = new CellPosition(PivotTable.Fields.Count - 1, row);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		int InitTableData(Worksheet sheet, PivotDataKey key, List<int> keyFieldIndices) {
			PivotCache cache = PivotTable.Cache;
			int row = 0;
			foreach(IPivotCacheRecord record in cache.GetRowByKeys(key, keyFieldIndices)){
				int column = 0;
				row++;
				foreach (VariantValue value in record.GetValuesEnumerable(cache.CacheFields, this.DataContext))
					sheet[new CellPosition(column++, row)].Value = value;
			}
			return row;
		}
		void InitTableHeader(Worksheet sheet) {
			int column = 0;
			foreach (PivotCacheField cField in PivotTable.Cache.CacheFields)
				sheet[new CellPosition(column++, 0)].Value = cField.Name;
		}
		#endregion
	}
}
