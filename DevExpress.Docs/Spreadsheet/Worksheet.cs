#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
#if !SL && !DXPORTABLE
using System.Data;
using DevExpress.Spreadsheet.Export;
#endif
namespace DevExpress.Spreadsheet {
	public static class WorksheetExtensions {
		public static void Import(this Worksheet sheet, object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex, converter);
		}
		public static void Import(this Worksheet sheet, object[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex, options);
		}
		public static void Import(this Worksheet sheet, int[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, short[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, byte[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, long[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, bool[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, float[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, double[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, decimal[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, DateTime[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, string[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, string[,] twoDimensionalArray, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(twoDimensionalArray, firstRowIndex, firstColumnIndex, options);
		}
		public static void Import(this Worksheet sheet, object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, IDataValueConverter converter) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical, converter);
		}
		public static void Import(this Worksheet sheet, object[] array, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical, options);
		}
		public static void Import(this Worksheet sheet, int[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, short[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, byte[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, long[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, bool[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, float[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, double[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, decimal[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, DateTime[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, string[] array, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(array, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, firstRowIndex, firstColumnIndex, isVertical);
		}
		public static void Import(this Worksheet sheet, IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical, IDataValueConverter converter) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, firstRowIndex, firstColumnIndex, isVertical, converter);
		}
		public static void Import(this Worksheet sheet, IEnumerable source, int firstRowIndex, int firstColumnIndex, bool isVertical, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, firstRowIndex, firstColumnIndex, isVertical, options);
		}
		public static void Import(this Worksheet sheet, object dataSource, int firstRowIndex, int firstColumnIndex, DataSourceImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(dataSource, firstRowIndex, firstColumnIndex, options);
		}
		public static void Import(this Worksheet sheet, object dataSource, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(dataSource, firstRowIndex, firstColumnIndex);
		}
#if !SL && !DXPORTABLE
		public static void Import(this Worksheet sheet, DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex, converter);
		}
		public static void Import(this Worksheet sheet, DataTable source, bool addHeader, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex, options);
		}
		public static void Import(this Worksheet sheet, IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex);
		}
		public static void Import(this Worksheet sheet, IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex, IDataValueConverter converter) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex, converter);
		}
		public static void Import(this Worksheet sheet, IDataReader source, bool addHeader, int firstRowIndex, int firstColumnIndex, DataImportOptions options) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			worksheet.Import(source, addHeader, firstRowIndex, firstColumnIndex, options);
		}
		public static DataTableExporter CreateDataTableExporter(this Worksheet sheet, Range range, DataTable dataTable, bool rangeHasHeaders) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			return worksheet.CreateDataTableExporter(range, dataTable, rangeHasHeaders);
		}
		public static DataTable CreateDataTable(this Worksheet sheet, Range range, bool rangeHasHeaders) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			return worksheet.CreateDataTable(range, rangeHasHeaders);
		}
		public static DataTable CreateDataTable(this Worksheet sheet, Range range, bool rangeHasHeaders, bool stringColumnType) {
			NativeWorksheet worksheet = (NativeWorksheet)sheet;
			return worksheet.CreateDataTable(range, rangeHasHeaders, stringColumnType);
		}
#endif
	}
}
#if !SL && !DXPORTABLE
namespace DevExpress.Spreadsheet.Export {
	public static class DataTableExporterExtensions {
		public static void Export(this DataTableExporter exporter) {
			exporter.Export();
		}
	}
}
#endif 
