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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.DataAccess.Excel;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native.Excel {
	public static class ExcelDataLoaderHelper {
		public static SelectedDataEx LoadData(ISpreadsheetSource source, FieldInfo[] schema, ExcelSourceOptionsBase sourceOptions, bool topThousand) {
			List<IList> lists = DataStoreExHelper.CreateChunkedLists(schema.Select(s => new ColumnInfoEx { Name = s.Name, Type = s.Type }).ToArray());
			ISpreadsheetDataReader dataReader = source.CreateReader(sourceOptions);
			if(sourceOptions.UseFirstRowAsHeader)
				dataReader.Read();
			int count = 1000;
			while(dataReader.Read() && (!topThousand || count-- > 0)) {
				for(int i = 0; i < schema.Length; i++) {
					if(!schema[i].Selected)
						continue;
					if(i < dataReader.FieldsCount)
						readers[schema[i].Type](i, dataReader, lists[i]);
					else
						lists[i].Add(null);
				}
			}
			List<IList> result = new List<IList>(schema.Count(f => f.Selected));
			for(int i = 0; i < schema.Length; i++) {
				if(schema[i].Selected)
					result.Add(lists[i]);
			}
			return new SelectedDataEx(result.ToArray(), schema.Where(s => s.Selected).Select(s => new ColumnInfoEx { Name = s.Name, Type = s.Type }).ToArray());
		}
		public static ISpreadsheetDataReader CreateReader(this ISpreadsheetSource source, ExcelSourceOptionsBase sourceOptions) {
			if(sourceOptions != null) {
				return sourceOptions.CreateReader(source);
			}
			return source.GetDataReader(source.Worksheets[0]);
		}
		public static ISpreadsheetSource CreateSource(Stream stream, ExcelDocumentFormat format, string fileName, ExcelSourceOptionsBase optionsBase) {
			if(stream != null)
				return SpreadsheetSourceFactory.CreateSource(stream, (SpreadsheetDocumentFormat)format, optionsBase.GetSpreadsheetSourceOptions());
			if(!string.IsNullOrEmpty(fileName)) {
				return SpreadsheetSourceFactory.CreateSource(fileName, optionsBase.GetSpreadsheetSourceOptions());
			}
			return null;
		}
		public static Type GetType(XlVariantValueType value) {
			if(value == XlVariantValueType.DateTime)
				return typeof(DateTime);
			if(value == XlVariantValueType.Boolean)
				return typeof(bool);
			if(value == XlVariantValueType.Numeric)
				return typeof(double);
			if(value == XlVariantValueType.Text)
				return typeof(string);
			return null;
		}
		public static string GetFieldName(Predicate<string> exist, string testedName) {
			return GetFieldName(exist, testedName, testedName);
		}
		public static string GetFieldName(Predicate<string> exist, string testedName, string generateFromHere) {
			if(!exist(testedName))
				return testedName;
			while(exist(generateFromHere)) {
				generateFromHere += 1;
			}
			return generateFromHere;
		}
		public static int GetColumnCount(ISpreadsheetSource spreadsheetSource, int testRowCount, ExcelSourceOptionsBase sourceSettings) {
			ISpreadsheetDataReader dataReader = spreadsheetSource.CreateReader(sourceSettings);
			int result = 0;
			while(dataReader.Read() && testRowCount > 0) {
				result = Math.Max(result, dataReader.FieldsCount);
				--testRowCount;
			}
			return result;
		}
		public static ExcelDocumentFormat DetectFormat(string fileName) {
			string extension = Path.GetExtension(fileName);
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xls") == 0)
				return ExcelDocumentFormat.Xls;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xlsx") == 0)
				return ExcelDocumentFormat.Xlsx;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".xlsm") == 0)
				return ExcelDocumentFormat.Xlsm;
			if(StringExtensions.CompareInvariantCultureIgnoreCase(extension, ".csv") == 0)
				return ExcelDocumentFormat.Csv;
			throw new ArgumentException("Unknown extension. Can't detect document format.");
		}
		static readonly Dictionary<Type, Action<int, ISpreadsheetDataReader, IList>> readers = new Dictionary<Type, Action<int, ISpreadsheetDataReader, IList>> { 
		{ typeof(byte), (i, reader, list) => ReadStruct(reader, (ChunkedList<byte?>)list, i, (r, index) => Convert.ToByte(r.GetDouble(index)))},
		{ typeof(sbyte), (i, reader, list) => ReadStruct(reader, (ChunkedList<sbyte?>)list, i, (r, index) => Convert.ToSByte(r.GetDouble(index)))},
		{ typeof(int), (i, reader, list) => ReadStruct(reader, (ChunkedList<int?>)list, i, (r, index) => Convert.ToInt32(r.GetDouble(index)))},
		{ typeof(uint), (i, reader, list) => ReadStruct(reader, (ChunkedList<uint?>)list, i, (r, index) => Convert.ToUInt32(r.GetDouble(index)))},
		{ typeof(short), (i, reader, list) => ReadStruct(reader, (ChunkedList<short?>)list, i, (r, index) => Convert.ToInt16(r.GetDouble(index)))},
		{ typeof(ushort), (i, reader, list) => ReadStruct(reader, (ChunkedList<ushort?>)list, i, (r, index) => Convert.ToUInt16(r.GetDouble(index)))},
		{ typeof(long), (i, reader, list) => ReadStruct(reader, (ChunkedList<long?>)list, i, (r, index) => Convert.ToInt64(r.GetDouble(index)))},
		{ typeof(ulong), (i, reader, list) => ReadStruct(reader, (ChunkedList<ulong?>)list, i, (r, index) => Convert.ToUInt64(r.GetDouble(index)))},
		{ typeof(bool), (i, reader, list) => ReadStruct(reader, (ChunkedList<bool?>)list, i, (r, index) => r.GetBoolean(index))},
		{ typeof(double), (i, reader, list) => ReadStruct(reader, (ChunkedList<double?>)list, i, (r, index) => r.GetDouble(index))}, 
		{ typeof(float), (i, reader, list) => ReadStruct(reader, (ChunkedList<float?>)list, i, (r, index) => Convert.ToSingle(r.GetDouble(index)))}, 
		{ typeof(decimal), (i, reader, list) => ReadStruct(reader, (ChunkedList<decimal?>)list, i, (r, index) => Convert.ToDecimal(r.GetDouble(index)))}, 
		{ typeof(DateTime), (i, reader, list) => ReadStruct(reader, (ChunkedList<DateTime?>)list, i, (r, index) => r.GetDateTime(index))} ,
		{ typeof(char), (i, reader, list) => ReadStruct(reader, (ChunkedList<char?>)list, i, (r, index) => Convert.ToChar(r.GetString(index)))},
		{ typeof(string), (i, reader, list) => ReadClass(reader, (ChunkedList<string>)list, i, (r, index) => r.GetString(index))},
		{ typeof(object), (i, reader, list) => ReadClass(reader, (ChunkedList<object>)list, i, (r, index) => r.GetString(index))},
		};
		static void ReadStruct<T>(ISpreadsheetDataReader dataReader, ChunkedList<T?> list, int index, Func<ISpreadsheetDataReader, int, T> getValue) where T : struct {
			if(dataReader.GetFieldType(index) == XlVariantValueType.None)
				list.Add(null);
			else {
				list.Add(getValue(dataReader, index));
			}
		}
		static void ReadClass<T>(ISpreadsheetDataReader dataReader, ChunkedList<T> list, int index, Func<ISpreadsheetDataReader, int, T> getValue) where T : class {
			if(dataReader.GetFieldType(index) == XlVariantValueType.None)
				list.Add(null);
			else {
				list.Add(getValue(dataReader, index));
			}
		}
	}
}
