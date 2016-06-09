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
	public class MemoryStorage : IFileStorage {
		MemoryTable table;
		public static IFileStorage CreateStorage() {
			return new MemoryStorage();
		}
		public IStorageTable CreateTable(string name, int rowCount, int columnsCount) {
			table = new MemoryTable(name);   
			return table;
		}
		public IStorageTable GetTable() {
			return table;
		}
		public void Clear() {
			table = null;
		}
		public void Dispose() {
		}
	}
	public class MemoryTable : IStorageTable {
		readonly List<IStorageColumn> columns = new List<IStorageColumn>();
		public string Name { get; private set; }
		public IStorageColumn this[string colName] {
			get {
				foreach (IStorageColumn column in columns)
					if (column.Name == colName)
						return column;
				return null;
			}
		}
		public List<IStorageColumn> Columns { get { return this.columns; } }
		public MemoryTable(string name) {
			this.Name = name;
		}
		public IStorageColumn AddColumn(string name, int uniqueCount, Type type) {
			IStorageColumn res = new MemoryColumn(name, type);
			columns.Add(res);
			return res;
		}
	}
	public class MemoryColumn : IStorageColumn {
		List<OffsetRecord> compressedValuesRecords = new List<OffsetRecord>();
		List<byte> compressedValues = new List<byte>();
		int compressedBlocksCount = 0;
		int uniqueCount = 0;
		int recordsCount = 0;
		int maxRecordCount = -1;
		Type type;
		UniqueValuesStorageBase uniqueValuesStorage;
		int[] decompressedData;
		public string Name { get; private set; }
		public MemoryColumn(string name, Type type) {
			this.type = type;
			Name = name;
			CreateUniqueValuesStorage();
		}
		void CreateUniqueValuesStorage() {
			uniqueValuesStorage = GenericActivator.New<UniqueValuesStorageBase>(typeof(UniqueValuesStorage<>), type);
		}
		#region IStorageColumn 
		int IStorageColumn.CompressedBlocksCount { get { return compressedBlocksCount; } }
		int IStorageColumn.UniqueCount { get { return uniqueCount; } }
		Type IStorageColumn.Type { get { return type; } }
		int IStorageColumn.Records { get { return recordsCount; } }
		void IStorageColumn.WriteUniqueValues<T>(List<T> list) {
			UniqueValuesStorage<T> typedStorage = (UniqueValuesStorage<T>)uniqueValuesStorage;			
			typedStorage.WriteValues(list);
			uniqueCount += list.Count;
		}
		T[] IStorageColumn.ReadColumnUniqueValues<T>(int start, int count) {
			UniqueValuesStorage<T> typedStorage = (UniqueValuesStorage<T>)uniqueValuesStorage;
			return typedStorage.ReadValues(start, count);
		}
		void IStorageColumn.WriteCompressedValues(int startIndex, int endIndex, byte[] values) {
			OffsetRecord record = new OffsetRecord();
			record.MinIndex = startIndex;
			record.MaxIndex = endIndex;
			maxRecordCount = Math.Max(maxRecordCount, endIndex - startIndex + 1);
			record.Length = values.Length;
			record.Offset = compressedValues.Count;
			compressedValuesRecords.Add(record);
			compressedBlocksCount++;			
			compressedValues.AddRange(values);
			recordsCount += endIndex - startIndex + 1;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		byte[] IStorageColumn.ReadCompressedValues(int compressedBlockIndex, out int startIndex, out int endIndex) {
			OffsetRecord record = compressedValuesRecords[compressedBlockIndex];
			startIndex = record.MinIndex;
			endIndex = record.MaxIndex;
			return compressedValues.GetRange(record.Offset, record.Length).ToArray();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		int[] IStorageColumn.ReadDecompressedValues(int compressedBlockIndex, out int length) {
			if (decompressedData == null)
				decompressedData = new int[maxRecordCount];
			OffsetRecord record = compressedValuesRecords[compressedBlockIndex];
			byte[] compressedData = compressedValues.GetRange(record.Offset, record.Length).ToArray();
			length = record.MaxIndex - record.MinIndex + 1;
			DataCompression.Decompression(compressedData, 0, decompressedData, uniqueCount, length);			
			return decompressedData;
		}
		int[] IStorageColumn.ReadDecompressedValues(int length, ColumnReadPosition position) {
			int bitsForElement = DataCompression.CalculateBitsForElement(uniqueCount);
			int valuesToRead = Math.Min(length, recordsCount - position.ReadValuesCount);
			int bitsToRead = bitsForElement * valuesToRead;
			int bitsToReadLeft = bitsToRead;
			int[] decompressedData = new int[valuesToRead];
			int decompressionIndex = 0;
			while (decompressionIndex < valuesToRead) {
				OffsetRecord record = compressedValuesRecords[position.CurrentRecordIndex];
				int compressedElementsLeft = ((record.Length - position.CurrentByteInBlockIndex) * 8 - position.ReadingOffsetBitInByte) / bitsForElement;
				int readingElementsCount = Math.Min(compressedElementsLeft, bitsToReadLeft / bitsForElement);
				int countBitsToRead = readingElementsCount * bitsForElement;
				int countBytesToRead = (int)Math.Ceiling((position.ReadingOffsetBitInByte + countBitsToRead) / 8.0);
				byte[] compressedData = compressedValues.GetRange(record.Offset + position.CurrentByteInBlockIndex, countBytesToRead).ToArray();
				bitsToReadLeft -= countBitsToRead;
				position.CurrentByteInBlockIndex += countBitsToRead / 8;
				int[] decompressedFullData = new int[readingElementsCount];
				DataCompression.Decompression(compressedData, position.ReadingOffsetBitInByte, decompressedFullData, uniqueCount, readingElementsCount);
				Array.Copy(decompressedFullData, 0, decompressedData, decompressionIndex, readingElementsCount);
				decompressionIndex += readingElementsCount;
				position.ReadingOffsetBitInByte = (position.ReadingOffsetBitInByte + bitsForElement * readingElementsCount) % 8;
				if (compressedElementsLeft - readingElementsCount == 0) {
					position.CurrentRecordIndex++;
					position.CurrentByteInBlockIndex = 0;
					position.ReadingOffsetBitInByte = 0;
				}
			}
			position.ReadValuesCount += bitsToRead / bitsForElement;
			return decompressedData;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		bool IStorageColumn.GetCompressedBlockMetadata(int compressedBlockIndex, out int startIndex, out int endIndex, out int offset, out int length) {
			OffsetRecord record = compressedValuesRecords[compressedBlockIndex];
			startIndex = record.MinIndex;
			endIndex = record.MaxIndex;
			offset = record.Offset;
			length = record.Length;
			return true;
		}	
		#endregion
	}
	public abstract class UniqueValuesStorageBase {
	}
	public class UniqueValuesStorage<T> : UniqueValuesStorageBase {
		List<T> storage = new List<T>();
		public void WriteValues(List<T> values) {
			storage.AddRange(values);
		}
		public T[] ReadValues(int start, int count) {
			return storage.GetRange(start, count).ToArray();
		}
	}
}
