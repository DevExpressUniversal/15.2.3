#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using System;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class MemorySource : IStorage, IDisposable {
		MemoryStorage storage;
		IStorageTable dataTable;
		public MemorySource(MemoryStorage storage) {
			this.storage = storage;
			dataTable = storage.GetTable();
		}
		#region IStorage
		public IDataFlow<T> OpenDataStream<T>(Query query) {
			IStorageColumn column = dataTable[query.ColumnName];
			if (column == null)
				return null;
			return new MemoryDataFlow<T>(column);
		}
		public Type GetColumnType(string columnName) {
			IStorageColumn column = dataTable[columnName];
			if (column == null)
				return typeof(object);
			return column.Type;
		}
		List<string> IStorage.Columns {
			get {
				List<String> columns = new List<string>();
				foreach (IStorageColumn column in dataTable.Columns)
					columns.Add(column.Name);
				return columns;
			}
		}
		#endregion
		public void Dispose() {
			if (storage != null)
				storage.Dispose();
		}
	}
	public class MemoryDataFlow<T> : IDataFlow<T> {
		IStorageColumn column;
		ColumnReadPosition position;
		T[] uniqueValues = null;
		internal T[] UniqueValues {
			get {
				if (uniqueValues == null)
					uniqueValues = column.ReadColumnUniqueValues<T>(0, column.UniqueCount);
				return uniqueValues;
			}
		}
		public MemoryDataFlow(IStorageColumn column) {
			this.column = column;
			position = new ColumnReadPosition();
		}
		T GetMaterializedValue(int[] indexes, int i, out bool nullFlag) {
			T value = default(T);
			nullFlag = true;
			int index = indexes[i];
			if (index != 0) {
				value = UniqueValues[index];
				nullFlag = false;
			}
			return value;
		}
		int GetSubstitutedValue(int[] indexes, int i, out bool nullFlag) {
			nullFlag = true;
			int value = indexes[i];
			if (value != 0)
				nullFlag = false;
			return value;
		}
		int GetResultData(IDataVector<T> result, bool getMaterialized) {
			if (IsEnded)
				return -1;
			int[] indexes = column.ReadDecompressedValues(result.Data.Length, position);
			bool isNull;
			for (int i = 0; i < indexes.Length; i++) {
				if (getMaterialized)
					result.Data[i] = GetMaterializedValue(indexes, i, out isNull);
				else
					((IDataVector<int>)result).Data[i] = GetSubstitutedValue(indexes, i, out isNull);
				result.SpecialData[i] = isNull ? SpecialDataValue.Null : SpecialDataValue.None;
			}
			result.Count = indexes.Length;
			return result.Count;
		}
		#region IDataFlow
		public string ColumnName { get { return column.Name; } }
		public bool IsEnded { get { return position.ReadValuesCount >= column.Records; } }
		public int NextMaterialized(IDataVector<T> result) {
			return GetResultData(result, true);
		}
		public int NextSubstitutes(IDataVector<int> result) {
			return GetResultData((IDataVector<T>)result, false);
		}
		public int Materialize(IDataVector<int> substitutes, IDataVector<T> result) {
			T[] uniqueValues = UniqueValues;
			for (int i = 0; i < substitutes.Count; i++) {
				if (substitutes.SpecialData[i] == SpecialDataValue.None)
					result.Data[i] = uniqueValues[substitutes.Data[i]];
				result.SpecialData[i] = substitutes.SpecialData[i];
			}
			result.Count = substitutes.Count;
			return result.Count;
		}
		#endregion
	}
}
