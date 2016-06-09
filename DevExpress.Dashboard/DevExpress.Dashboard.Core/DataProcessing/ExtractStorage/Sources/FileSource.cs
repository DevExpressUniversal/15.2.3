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

using System;
using System.Collections.Generic;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class FileSource : IStorage, IDisposable {
		FileStorage storage;
		IStorageTable dataTable;
		bool loadAll;
		public FileSource(string fileName, bool loadAll) {
			this.loadAll = loadAll;
			storage = FileStorage.CreateFileStorage(fileName);
			dataTable = ((IFileStorage)storage).GetTable();
		}
		IDataFlow<T> IStorage.OpenDataStream<T>(Query query) {
			IStorageColumn column = dataTable[query.ColumnName];
			if (column == null)
				return null;
			return new DataFlow<T>(column, loadAll);
		}
		Type IStorage.GetColumnType(string columnName) {
			return dataTable[columnName].Type;
		}
		List<string> IStorage.Columns {
			get {
				List<String> columns = new List<string>();
				foreach (IStorageColumn column in dataTable.Columns)
					columns.Add(column.Name);
				return columns;
			}
		}
		public void Dispose() {
			if (storage != null)
				((IDisposable)storage).Dispose();
		}
	}
	public class DataFlow<T> : IDataFlow<T> {
		int BufferLength = 2000000;
		IStorageColumn column;
		int blockIndex = 0;
		CircularBuffer<Int32> buffer;
		T[] dictionary;
		public DataFlow(IStorageColumn column, bool loadAll) {
			this.column = column;
			BufferLength = (int)((this.column.Records / this.column.CompressedBlocksCount) * 1.5);
			buffer = new CircularBuffer<Int32>(BufferLength);
			dictionary = this.column.ReadColumnUniqueValues<T>(0, this.column.UniqueCount);
			if (loadAll) {
				buffer = new CircularBuffer<Int32>(BufferLength * 100);
				while (buffer.FreeSpace > 0 && blockIndex < column.CompressedBlocksCount)
					FillBuffer();
			}
			else {
				buffer = new CircularBuffer<Int32>(BufferLength);
				FillBuffer();
			}
		}
		void FillBuffer() {
			int length;
			int[] temp = this.column.ReadDecompressedValues(blockIndex, out length);
			buffer.Push(temp, 0, length);
		}
		T GetMaterializedValue(int index, out bool nullFlag) {
			T value = default(T);
			nullFlag = true;
			if (index != 0) {
				value = dictionary[index];
				nullFlag = false;
			}
			return value;
		}
		#region IDataFlow
		string IDataFlow<T>.ColumnName { get { return column.Name; } }
		bool IDataFlow<T>.IsEnded {
			get { return blockIndex == column.CompressedBlocksCount && buffer.Count == 0; }
		}
		int IDataFlow<T>.NextMaterialized(IDataVector<T> result) {
			if (buffer.Count == 0 && blockIndex == column.CompressedBlocksCount)
				return -1;
			while (buffer.Count < result.Data.Length && blockIndex < column.CompressedBlocksCount)
				FillBuffer();
			int i = 0;
			while (i < result.Data.Length && buffer.Count > 0) {
				int index = buffer.PullOne();
				if (index < dictionary.Length) {
					bool isNull;
					result.Data[i] = GetMaterializedValue(index, out isNull);
					result.SpecialData[i] = isNull ? SpecialDataValue.Null : SpecialDataValue.None;
				}
				i++;
			}
			result.Count = i;
			return i;
		}
		int IDataFlow<T>.NextSubstitutes(IDataVector<int> result) {
			if (buffer.Count == 0 && blockIndex == column.CompressedBlocksCount)
				return -1;
			while (buffer.Count < result.Data.Length && blockIndex < column.CompressedBlocksCount)
				FillBuffer();
			int dataCount = buffer.Pull(result.Data, result.Data.Length);
			result.Count = dataCount;
			for (int i = 0; i < result.Count; i++)
				result.SpecialData[i] = result.Data[i] == 0 ? SpecialDataValue.Null : SpecialDataValue.None;
			return dataCount;
		}
		public int Materialize(IDataVector<int> substitutes, IDataVector<T> result) {
			for (int i = 0; i < substitutes.Count; i++) {
				result.Data[i] = dictionary[substitutes.Data[i]];
				result.SpecialData[i] = SpecialDataValue.None;
			}
			result.Count = substitutes.Count;
			return result.Count;
		}
		#endregion IDataFlow
	}
}
