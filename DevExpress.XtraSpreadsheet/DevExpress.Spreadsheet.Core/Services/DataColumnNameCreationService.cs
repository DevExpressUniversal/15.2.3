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

using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
#if !SL && !DXPORTABLE
using System.Data;
#endif
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Services {
	public interface IDataColumnNameCreationService {
#if !SL && !DXPORTABLE
		void ExtractDataTableColumnNamesFromHeaderRange(DataTable dataTable, Range headerRange);
#endif
		String ColumnPrefix { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	public class DataColumnNameCreationService : IDataColumnNameCreationService {
		string columnPrefix = "Column";
		public String ColumnPrefix { get { return columnPrefix; } set { columnPrefix = value; } }
#if !SL && !DXPORTABLE
		public void ExtractDataTableColumnNamesFromHeaderRange(DataTable dataTable, Range headerRange) {
			Model.Worksheet modelWorksheet = (headerRange.Worksheet as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet).ModelWorksheet;
			Model.Row headerRow = modelWorksheet.Rows[headerRange.TopRowIndex]; 
			IEnumerable<Model.ICell> existingCells = headerRow.Cells.GetExistingCells(headerRange.LeftColumnIndex, headerRange.RightColumnIndex, false);
			foreach (Model.ICell modelCell in existingCells) {
				int index = modelCell.Key.ColumnIndex - headerRange.LeftColumnIndex;
				Model.VariantValue value = modelCell.Value;
				string displayText = value.ToText(modelWorksheet.DataContext).GetTextValue(modelWorksheet.SharedStringTable);
				if (String.IsNullOrEmpty(displayText))
					continue;
				int postfix = 2;
				string fixedName = displayText;
				while (true) {
					try {
						dataTable.Columns[index].ColumnName = fixedName;
						break;
					}
					catch (System.Data.DuplicateNameException) {
						fixedName = String.Format("{0} ({1})", displayText, postfix.ToString());
						postfix++;
					}
				}
			}
		}
#endif
	}
}
