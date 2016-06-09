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
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ICellTable
	public interface ICellTable {
		string Name { get; }
		int SheetId { get; }
		IModelWorkbook Workbook { get; }
		IRowCollectionBase Rows { get; }
		ICellBase TryGetCell(int column, int row);
		ICellBase GetCell(int column, int row);
		VariantValue GetCalculatedCellValue(int column, int row);
		int MaxRowCount { get; }
		int MaxColumnCount { get; }
		bool ReadOnly { get; }
	}
	#endregion
	#region IWorksheet
	public interface IWorksheet : ICellTable { 
		DefinedNameCollectionBase DefinedNames { get; }
		bool IsColumnVisible(int columnIndex);
		bool IsRowVisible(int rowIndex);
		bool IsDataSheet { get; }
		Table GetTableByCellPosition(int columnIndex, int rowIndex);
		ITableCollection Tables { get; }
	}
	#endregion
}
