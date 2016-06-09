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
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class FileStorage : IFileStorage {
		public static string GetName(char[] p) {
			int i = 0;
			while (p[i] != '\0' && i < p.Length)
				i++;
			return new string(p, 0, i);
		}
		public static char[] GetNameArray(string p) {
			char[] name = new char[256];
			for (int i = 0; i < p.Length; i++) {
				name[i] = p[i];
			}
			name[p.Length] = '\0';
			return name;
		}
		public static int GetDescriptorByType(Type type) {
			if (typeof(Int32) == type)
				return 1;
			if (typeof(Decimal) == type)
				return 2;
			if (typeof(float) == type)
				return 3;
			if (typeof(String) == type)
				return 4;
			return -1;
		}
		public static Type GetTypeByDescriptor(int descriptor) {
			if (descriptor == 1)
				return typeof(Int32);
			if (descriptor == 2)
				return typeof(Decimal);
			if (descriptor == 3)
				return typeof(float);
			if (descriptor == 4)
				return typeof(String);
			return typeof(Int32);
		}
		public static FileStorage CreateFileStorage(string path) {
			return new FileStorage(new FileWorker(path));
		}
		public static FileStorage CreateStorage(DataWorker reader) {
			return new FileStorage(reader);
		}
		public static FileStorage CreateMemoryMappedFileStorage(string path) {
			return new FileStorage(new MMFileWorker(path));
		}
		public static FileStorage CreateNonPersistentMemoryMappedFileStorage(string path) {
			return new FileStorage(new NonPersistentMMFileWorker());
		}
		public static FileStorage CreateNonPersistentMemoryMappedFileStorage(MemoryMappedFile file) {
			return new FileStorage(new NonPersistentMMFileWorker(file));
		}
		class OffsetTable {
			public static int MaxCountOfRecords = 64;
			public static OffsetTable Load(int offset, DataWorker dataWorker) {
				dataWorker.SetPositionInFile(offset);
				byte[] buff = dataWorker.ReadBytes<OffsetTableStruct>();
				OffsetTableStruct offsetTable = new OffsetTableStruct();
				int index = 0;
				offsetTable.NextTableOffset = BitConverter.ToInt32(buff, index);
				index += Marshal.SizeOf(offsetTable.NextTableOffset);
				offsetTable.TypeOfRecord = (OffsetTableType)buff[index];
				index += 1;
				offsetTable.RecordIndex = BitConverter.ToInt32(buff, index);
				index += Marshal.SizeOf(offsetTable.RecordIndex);
				offsetTable.Records = new OffsetRecord[OffsetTable.MaxCountOfRecords];
				for (int i = 0; i < OffsetTable.MaxCountOfRecords; i++) {
					int orIndex = 0;
					OffsetRecord or = new OffsetRecord();
					or.MinIndex = BitConverter.ToInt32(buff, index + i * 16 + orIndex);
					orIndex += Marshal.SizeOf(or.MinIndex);
					or.MaxIndex = BitConverter.ToInt32(buff, index + i * 16 + orIndex);
					orIndex += Marshal.SizeOf(or.MaxIndex);
					or.Offset = BitConverter.ToInt32(buff, index + i * 16 + orIndex);
					orIndex += Marshal.SizeOf(or.Offset);
					or.Length = BitConverter.ToInt32(buff, index + i * 16 + orIndex);
					orIndex += Marshal.SizeOf(or.Length);
					offsetTable.Records[i] = or;
				}
				return new OffsetTable(offsetTable);
			}
			int nextTableOffset;
			OffsetTableType typeOfRecord;
			List<OffsetRecord> records;
			public int NextTableOffset { get { return nextTableOffset; } set { nextTableOffset = value; } }
			public OffsetTableType TypeOfRecord { get { return typeOfRecord; } }
			public int Count { get { return records.Count; } }
			public bool Full { get { return records.Count == MaxCountOfRecords; } }
			public OffsetRecord this[int i] { get { return records[i]; } }
			internal OffsetTable AddRecord(OffsetRecord offsetRecord) {
				if (MaxCountOfRecords == records.Count)
					return new OffsetTable(typeOfRecord, -1);
				records.Add(offsetRecord);
				return null;
			}
			public OffsetTable(OffsetTableType typeOfRecord, int nextTableOffset) {
				this.typeOfRecord = typeOfRecord;
				this.records = new List<OffsetRecord>();
				this.nextTableOffset = nextTableOffset;
			}
			public OffsetTable(OffsetTableStruct offsetTable) {
				this.nextTableOffset = offsetTable.NextTableOffset;
				this.typeOfRecord = offsetTable.TypeOfRecord;
				this.records = new List<OffsetRecord>();
				for (int i = 0; i < offsetTable.RecordIndex; i++)
					this.records.Add(offsetTable.Records[i]);
			}
			public OffsetTableStruct GetStruct() {
				OffsetTableStruct table = new OffsetTableStruct();
				table.NextTableOffset = nextTableOffset;
				table.TypeOfRecord = typeOfRecord;
				table.RecordIndex = this.records.Count;
				table.Records = new OffsetRecord[MaxCountOfRecords];
				for (int i = 0; i < table.RecordIndex; i++)
					table.Records[i] = records[i];
				return table;
			}
		}
		class OffsetTableChain {
			public static OffsetTableChain Load(int startOffset, DataWorker dataWorker, string name) {
				OffsetTableChain chain = new OffsetTableChain(name);
				chain.Load(startOffset, dataWorker);
				return chain;
			}
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
			public static OffsetTableChain Create(string name, int endOfFile, out int newEndOfFile) {
				OffsetTableChain chain = new OffsetTableChain(name);
				newEndOfFile = chain.Init(endOfFile);
				return chain;
			}
			List<OffsetTable> total;
			List<OffsetTable> typedValuesTable;
			List<OffsetTable> BLOBTable;
			string parentName;
			OffsetTableChain(string name) {
				parentName = name;
				total = new List<OffsetTable>();
				BLOBTable = new List<OffsetTable>();
				typedValuesTable = new List<OffsetTable>();
			}
			void Load(int startOffset, DataWorker dataWorker) {
				int offset = startOffset;
				OffsetTable offsetTable;
				while (offset > 0) {
					offsetTable = OffsetTable.Load(offset, dataWorker);
					total.Add(offsetTable);
					if (offsetTable.TypeOfRecord == OffsetTableType.TypedValues)
						typedValuesTable.Add(offsetTable);
					else
						BLOBTable.Add(offsetTable);
					offset = offsetTable.NextTableOffset;
				}
			}
			int Init(int endOfFile) {
				endOfFile += DXMarshal.SizeOf<OffsetTableStruct>();
				typedValuesTable.Add(new OffsetTable(OffsetTableType.TypedValues, endOfFile));
				BLOBTable.Add(new OffsetTable(OffsetTableType.BLOB, -1));
				endOfFile += DXMarshal.SizeOf<OffsetTableStruct>();
				total.Add(typedValuesTable[0]);
				total.Add(BLOBTable[0]);
				return endOfFile;
			}
			public OffsetTable GetLast(OffsetTableType type) {
				List<OffsetTable> tables;
				if (type == OffsetTableType.TypedValues)
					tables = typedValuesTable;
				else
					tables = BLOBTable;
				foreach (OffsetTable table in tables)
					if (!table.Full)
						return table;
				return null;
			}
			public List<OffsetTable> GetTables(OffsetTableType type) {
				if (type == OffsetTableType.TypedValues)
					return typedValuesTable;
				else
					return BLOBTable;
			}
			public OffsetRecord this[int i, OffsetTableType type] {
				get {
					List<OffsetTable> tables;
					if (type == OffsetTableType.TypedValues)
						tables = typedValuesTable;
					else
						tables = BLOBTable;
					int tableIndex = i / OffsetTable.MaxCountOfRecords;
					int recordIndex = i % OffsetTable.MaxCountOfRecords;
					return tables[tableIndex][recordIndex];
				}
			}
			public int Count(OffsetTableType type) {
				if (type == OffsetTableType.TypedValues)
					return typedValuesTable.Count;
				return BLOBTable.Count;
			}
			public int Add(OffsetTable newTable, int endOfFile) {
				if (newTable == null)
					return endOfFile;
				OffsetTable lastTable = null;
				foreach (OffsetTable table in total)
					if (table.NextTableOffset < 0) {
						lastTable = table;
						break;
					}
				if (lastTable == null)
					throw new Exception("can not find last offset table for column: " + parentName);
				lastTable.NextTableOffset = endOfFile;
				endOfFile += DXMarshal.SizeOf<OffsetTableStruct>();
				total.Add(newTable);
				if (newTable.TypeOfRecord == OffsetTableType.TypedValues)
					typedValuesTable.Add(newTable);
				else
					BLOBTable.Add(newTable);
				return endOfFile;
			}
			public void Save(int startOffset, DataWorker dataWorker) {
				int offset = startOffset;
				dataWorker.SetPositionInFile(offset);
				foreach (OffsetTable currentTable in total) {
					OffsetTableStruct str = currentTable.GetStruct();
					dataWorker.WriteData(str.NextTableOffset);
					dataWorker.WriteData((Byte)str.TypeOfRecord);
					dataWorker.WriteData(str.RecordIndex);
					foreach (OffsetRecord record in str.Records) {
						dataWorker.WriteData(record.MinIndex);
						dataWorker.WriteData(record.MaxIndex);
						dataWorker.WriteData(record.Offset);
						dataWorker.WriteData(record.Length);
					}
					offset = str.NextTableOffset;
					if (offset == -1)
						break;
					dataWorker.SetPositionInFile(offset);
				}
			}
		}
		DataWorker dataWorker;
		Table table;
		List<Column> columns = new List<Column>();
		Dictionary<Column, OffsetTableChain> columnsOffsetTables = new Dictionary<Column, OffsetTableChain>();
		int endPositionForData = 0;
		int endPositionForMetadata = 0;
		bool metadataWrited = false;
		int maxRecordCount = -1;
		int[] decompressedData;
		FileStorage(DataWorker dataWorker) {
			this.dataWorker = dataWorker;
		}
		#region IFileStorage
		IStorageTable IFileStorage.CreateTable(string name, int rowCount, int columns) {
			endPositionForData = 0;
			endPositionForMetadata += DXMarshal.SizeOf<TableHeader>();
			endPositionForMetadata += DXMarshal.SizeOf<ColumnHeader>() * columns;
			endPositionForData = endPositionForMetadata + DXMarshal.SizeOf<OffsetTableStruct>() * columns * 2;
			table = new Table(name, rowCount, columns, this);
			return table;
		}
		IStorageTable IFileStorage.GetTable() {
			dataWorker.OpenFileForRead();
			try {
				byte[] buff = dataWorker.ReadBytes<TableHeader>();
				TableHeader tableHeader = new TableHeader();
				tableHeader.Version = BitConverter.ToInt32(buff, 0);
				tableHeader.ColumnCount = BitConverter.ToInt32(buff, Marshal.SizeOf(tableHeader.Version));
				tableHeader.RowCount = BitConverter.ToInt32(buff, Marshal.SizeOf(tableHeader.Version) * 2);
				tableHeader.Name = ByteConverter.Convert<char>(buff, Marshal.SizeOf(tableHeader.Version) * 3, 256);
				this.table = new Table(tableHeader, this);
				for (int i = 0; i < tableHeader.ColumnCount; i++) {
					buff = dataWorker.ReadBytes<ColumnHeader>();
					ColumnHeader columnHeader = new ColumnHeader();
					int index = 0;
					columnHeader.Name = ConvertToChars(buff, index, 256);
					index += 256;
					columnHeader.UniqueCount = BitConverter.ToInt32(buff, index);
					index += Marshal.SizeOf(columnHeader.UniqueCount);
					columnHeader.Type = BitConverter.ToInt32(buff, index);
					index += Marshal.SizeOf(columnHeader.Type);
					columnHeader.CompressionType = (CompressionType)BitConverter.ToInt32(buff, index);
					index += DXMarshal.SizeOf<Int32>();
					columnHeader.CompressedBlocksCount = BitConverter.ToInt32(buff, index);
					index += Marshal.SizeOf(columnHeader.CompressedBlocksCount);
					columnHeader.DataOffset = BitConverter.ToInt32(buff, index);
					index += Marshal.SizeOf(columnHeader.DataOffset);
					columnHeader.RecordsCount = BitConverter.ToInt32(buff, index);
					index += Marshal.SizeOf(columnHeader.RecordsCount);
					columns.Add(new Column(columnHeader, this));
				}
				table.AddColumn(columns);
				for (int i = 0; i < columns.Count; i++) {
					Column column = columns[i];
					OffsetTableChain chain = OffsetTableChain.Load(column.DataOffset, dataWorker, column.Name);
					columnsOffsetTables.Add(column, chain);
				}
			}
			finally {
				dataWorker.CloseFile();
			}
			return table;
		}
		void IFileStorage.Clear() {
			throw new NotSupportedException();
		}
		#endregion
		#region IDisposable
		void IDisposable.Dispose() {
			if (!metadataWrited) {
				WriteMetadata();
				metadataWrited = true;
			}
		}
		#endregion IDisposable
		~FileStorage() {
			if (!metadataWrited) {
				WriteMetadata();
				metadataWrited = true;
			}
		}
		void WriteMetadata() {
			dataWorker.OpenFileForWrite();
			try {
				TableHeader tableHeader = table.GetHeader();
				dataWorker.WriteData(tableHeader.Version);
				dataWorker.WriteData(tableHeader.ColumnCount);
				dataWorker.WriteData(tableHeader.RowCount);
				dataWorker.WriteData(tableHeader.Name);
				for (int i = 0; i < columns.Count; i++) {
					ColumnHeader column = columns[i].GetHeader();
					dataWorker.WriteData(column.Name);
					dataWorker.WriteData(column.UniqueCount);
					dataWorker.WriteData(column.Type);
					dataWorker.WriteData((Int32)column.CompressionType);
					dataWorker.WriteData(column.CompressedBlocksCount);
					dataWorker.WriteData(column.DataOffset);
					dataWorker.WriteData(column.RecordsCount);
				}
				for (int i = 0; i < columns.Count; i++) {
					Column column = columns[i];
					OffsetTableChain chain = columnsOffsetTables[column];
					chain.Save(column.DataOffset, dataWorker);
				}
			}
			finally {
				dataWorker.CloseFile();
			}
		}
		string[] ReadStrings(Column column, int start, int count) {
			List<OffsetTable> tables = columnsOffsetTables[column].GetTables(OffsetTableType.TypedValues);
			int realCount = column.UniqueCount - start < count ? column.UniqueCount - start : count;
			List<string> strings = new List<string>();
			foreach (OffsetTable table in tables) {
				for (int i = 0; i < table.Count; i++) {
					OffsetRecord record = table[i];
					if (record.MinIndex <= start && start <= record.MaxIndex) {
						byte[] block = new byte[record.Length];
						dataWorker.ReadFromFile(record.Offset, 0, record.Length, block);
						int startInBlock = start - record.MinIndex;
						int countInBlock = record.MaxIndex - start + 1;
						if (countInBlock > count - strings.Count)
							countInBlock = count - strings.Count;
						List<string> strBuff = ByteConverter.ConvertToString(block, startInBlock, countInBlock);
						strings.AddRange(strBuff);
						if (strings.Count == count)
							break;
						start = record.MaxIndex + 1;
					}
				}
				if (strings.Count == count)
					break;
			}
			return strings.ToArray();
		}
		byte[][] ReadByteArray(Column column, int start, int count) {
			List<OffsetTable> tables = columnsOffsetTables[column].GetTables(OffsetTableType.TypedValues);
			int realCount = column.UniqueCount - start < count ? column.UniqueCount - start : count;
			List<byte[]> blobs = new List<byte[]>();
			foreach (OffsetTable table in tables) {
				for (int i = 0; i < table.Count; i++) {
					OffsetRecord record = table[i];
					if (record.MinIndex <= start && start <= record.MaxIndex) {
						byte[] block = new byte[record.Length];
						dataWorker.ReadFromFile(record.Offset, 0, record.Length, block);
						int startInBlock = start - record.MinIndex;
						int countInBlock = record.MaxIndex - start + 1;
						if (countInBlock > count - blobs.Count)
							countInBlock = count - blobs.Count;
						List<byte[]> strBuff = ByteConverter.ConvertToByteArray(block, startInBlock, countInBlock);
						blobs.AddRange(strBuff);
						if (blobs.Count == count)
							break;
						start = record.MaxIndex + 1;
					}
				}
				if (blobs.Count == count)
					break;
			}
			return blobs.ToArray();
		}
		int GetOffsetOfNewBlock(Column column) {
			return endPositionForData;
		}
		int WriteBlock(int offset, byte[] buff) {
			dataWorker.WriteToFile(offset, 0, buff.Length, buff);
			return buff.Length;
		}
		int WriteBlock<T>(Column column, int offset, List<T> values) {
			byte[] buff = ByteConverter.Convert<T>(values);
			dataWorker.WriteToFile(offset, 0, buff.Length, buff);
			return buff.Length;
		}
		int GetByteCountForType(int typeCode) {
			if (typeCode == 1)
				return DXMarshal.SizeOf<Int32>();
			if (typeCode == 2)
				return DXMarshal.SizeOf<Decimal>();
			if (typeCode == 3)
				return DXMarshal.SizeOf<float>();
			return 4;
		}
		int GetByteCountForType(Type type) {
			return GetByteCountForType(GetDescriptorByType(type));
		}
		char[] ConvertToChars(byte[] buffer, int start, int length) {
			return System.Text.Encoding.UTF8.GetString(buffer).ToCharArray();
		}
		void AddTableToChain(Column column, OffsetTable newTable) {
			this.endPositionForData = columnsOffsetTables[column].Add(newTable, this.endPositionForData);
		}
		internal void WriteTypedValues<T>(Column column, List<T> values) {
			if (values.Count + column.IndexForUniqueValues > column.UniqueCount)
				throw new Exception("to many records for column " + column.Name);
			OffsetTable tableForUniquevalues = columnsOffsetTables[column].GetLast(OffsetTableType.TypedValues);
			OffsetRecord offsetRecord = new OffsetRecord();
			offsetRecord.MinIndex = column.IndexForUniqueValues;
			offsetRecord.MaxIndex = column.IndexForUniqueValues + values.Count - 1;
			offsetRecord.Offset = GetOffsetOfNewBlock(column);
			offsetRecord.Length = WriteBlock(column, offsetRecord.Offset, values);
			OffsetTable newTable = null;
			if (tableForUniquevalues == null) {
				newTable = new OffsetTable(OffsetTableType.TypedValues, -1);
				newTable.AddRecord(offsetRecord);
			}
			else
				tableForUniquevalues.AddRecord(offsetRecord);
			column.IndexForUniqueValues += values.Count;
			this.endPositionForData += offsetRecord.Length; 
			AddTableToChain(column, newTable);
		}
		internal T[] ReadTypedValues<T>(Column column, int start, int count) {
			if (column.UniqueCount - start < 0 || count > column.UniqueCount)
				throw new Exception("not enough records to read. column: " + column.Name);
			if (typeof(T) == typeof(string) && column.Type == typeof(string))
				return ReadStrings(column, start, count).Cast<T>().ToArray();
			if (typeof(T) == typeof(byte[]) && column.Type == typeof(byte[]))
				return ReadByteArray(column, start, count).Cast<T>().ToArray();
			int realCount = column.UniqueCount - start < count ? column.UniqueCount - start : count;
			int countByBytes = realCount * GetByteCountForType(column.Type);
			int indexInBuffer = 0;
			byte[] buff = new byte[countByBytes];
			OffsetTableChain chain = columnsOffsetTables[column];
			for (int i = 0; i < chain.Count(OffsetTableType.TypedValues); i++) {
				OffsetRecord record = chain[i, OffsetTableType.TypedValues];
				if (record.MinIndex <= start && start <= record.MaxIndex) {
					int offsetInFile = record.Offset + (start - record.MinIndex) * GetByteCountForType(column.Type);
					int bytesCount = (record.MaxIndex - start + 1) * GetByteCountForType(column.Type);
					if (countByBytes - indexInBuffer < bytesCount)
						bytesCount = countByBytes - indexInBuffer;
					if (bytesCount == 0)
						break;
					dataWorker.ReadFromFile(offsetInFile, indexInBuffer, bytesCount, buff);
					indexInBuffer += bytesCount;
					if (indexInBuffer + 1 == countByBytes)
						break;
					start = record.MaxIndex + 1;
				}
			}
			if (typeof(T) == typeof(decimal) && column.Type == typeof(decimal))
				return ByteConverter.ConvertToDecimal(buff, 0, countByBytes).Cast<T>().ToArray();
			if (typeof(T) == typeof(DateTime) && column.Type == typeof(DateTime))
				return ByteConverter.ConvertToDateTime(buff, 0, countByBytes).Cast<T>().ToArray();
			if (typeof(T) == typeof(TimeSpan) && column.Type == typeof(TimeSpan))
				return ByteConverter.ConvertToTimeSpan(buff, 0, countByBytes).Cast<T>().ToArray();
			if (typeof(T) == typeof(Guid) && column.Type == typeof(Guid))
				return ByteConverter.ConvertToGuid(buff, 0, countByBytes).Cast<T>().ToArray();
			return ByteConverter.Convert<T>(buff, 0, countByBytes);
		}
		internal void WriteCompressedValues(Column column, int startIndex, int endIndex, byte[] values) {
			OffsetTable tableForValues = columnsOffsetTables[column].GetLast(OffsetTableType.BLOB); 
			int length = 0;
			OffsetRecord offsetRecord = new OffsetRecord();
			offsetRecord.MinIndex = startIndex;
			offsetRecord.MaxIndex = endIndex;
			maxRecordCount = Math.Max(maxRecordCount, endIndex - startIndex + 1);
			offsetRecord.Offset = GetOffsetOfNewBlock(column);
			length = WriteBlock(offsetRecord.Offset, values);
			offsetRecord.Length = length;
			OffsetTable newTable = null;
			if (tableForValues == null) {
				newTable = new OffsetTable(OffsetTableType.BLOB, -1);
				newTable.AddRecord(offsetRecord);
			}
			else
				tableForValues.AddRecord(offsetRecord);
			column.CompressedBlocksCount++;
			column.Records += endIndex - startIndex + 1;
			this.endPositionForData += length; 
			AddTableToChain(column, newTable);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		internal byte[] ReadCompressedValues(Column column, int compressedBlockIndex, out int startIndex, out int endIndex) {
			startIndex = -1;
			endIndex = -1;
			OffsetRecord record = columnsOffsetTables[column][compressedBlockIndex, OffsetTableType.BLOB];
			if (record.Length == 0)
				return null;
			byte[] buff = new byte[record.Length];
			dataWorker.ReadFromFile(record.Offset, 0, record.Length, buff);
			startIndex = record.MinIndex;
			endIndex = record.MaxIndex;
			return buff;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		internal int[] ReadDecompressedValues(Column column, int compressedBlockIndex, out int length) {
			if (decompressedData == null)
				decompressedData = new int[maxRecordCount];
			OffsetRecord record = columnsOffsetTables[column][compressedBlockIndex, OffsetTableType.BLOB];
			length = record.MaxIndex - record.MinIndex + 1;
			if (record.Length == 0)
				return null;
			byte[] compressedData = new byte[record.Length];
			dataWorker.ReadFromFile(record.Offset, 0, record.Length, compressedData);						
			DataCompression.Decompression(compressedData, 0, decompressedData, column.UniqueCount, length);
			return decompressedData;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		internal bool GetCompressedBlockMetadata(Column column, int compressedBlockIndex, out int startIndex, out int endIndex, out int offset, out int length) {
			startIndex = -1;
			endIndex = -1;
			offset = -1;
			length = -1;
			OffsetRecord record = columnsOffsetTables[column][compressedBlockIndex, OffsetTableType.BLOB];
			startIndex = record.MinIndex;
			endIndex = record.MaxIndex;
			offset = record.Offset;
			length = record.Length;
			return true;
		}
		internal void AddColumn(Column column) {
			this.columns.Add(column);
			column.DataOffset = endPositionForMetadata;
			OffsetTableChain chain = OffsetTableChain.Create(column.Name, endPositionForMetadata, out endPositionForMetadata);
			columnsOffsetTables.Add(column, chain);
		}
	}
	public static class ByteConverter {
		public static byte[] Convert<T>(List<T> values) {
			Type type = typeof(T);
			if (type == typeof(string))
				return ByteConverter.Convert(values.ConvertAll(x => { return x as string; }));
			else if (type == typeof(byte[]))
				return ByteConverter.Convert(values.ConvertAll(x => { return x as byte[]; }));
			else if (type == typeof(decimal))
				return ByteConverter.Convert(values.ConvertAll(x => { return System.Convert.ToDecimal(x); }));
			else if (type == typeof(Guid))
				return ByteConverter.Convert(values.Cast<Guid>().ToList());
			else if (type == typeof(DateTime))
				return ByteConverter.Convert(values.ConvertAll(x => { return System.Convert.ToDateTime(x); }));
			else if (type == typeof(TimeSpan))
				return ByteConverter.Convert(values.Cast<TimeSpan>().ToList());
			else
				return ByteConverter.ConvertPrimitive<T>(values);
		}
		public static List<byte[]> ConvertToByteArray(byte[] block, int start, int count) {
			int blobsCount = 0;
			blobsCount = BitConverter.ToInt32(block, 0);
			int intLength = DXMarshal.SizeOf<Int32>();
			int offset = DXMarshal.SizeOf<Int32>() + DXMarshal.SizeOf<Int32>() * blobsCount;
			int[] lengths = new int[blobsCount];
			for (int i = 0; i < blobsCount; i++)
				lengths[i] = BitConverter.ToInt32(block, intLength + i * intLength);
			for (int i = 0; i < blobsCount; i++) {
				if (i < start)
					offset += lengths[i];
			}
			List<byte[]> list = new List<byte[]>();
			for (int i = start; i < start + count; i++) {
				byte[] buffer = new byte[lengths[i]];
				Buffer.BlockCopy(block, offset, buffer, 0, lengths[i]);
				offset += lengths[i];
				list.Add(buffer);
			}
			return list;
		}
		public static List<string> ConvertToString(byte[] block, int start, int count) {
			int stringsCount = 0;
			stringsCount = BitConverter.ToInt32(block, 0);
			int intLength = DXMarshal.SizeOf<Int32>();
			int offset = DXMarshal.SizeOf<Int32>() + DXMarshal.SizeOf<Int32>() * stringsCount;
			int[] lengths = new int[stringsCount];
			for (int i = 0; i < stringsCount; i++)
				lengths[i] = BitConverter.ToInt32(block, intLength + i * intLength);
			int stringsLength = 0;
			for (int i = 0; i < stringsCount; i++) {
				if (i < start)
					offset += lengths[i];
				else
					stringsLength += lengths[i];
			}
			List<string> list = new List<string>();
			for (int i = start; i < start + count; i++) {
				list.Add(System.Text.Encoding.Unicode.GetString(block, offset, lengths[i]));
				offset += lengths[i];
			}
			return list;
		}
		public static DateTime[] ConvertToDateTime(byte[] buffer, int start, int length) {
			int typeLength = DXMarshal.SizeOf<Int64>();
			DateTime[] buff = new DateTime[length / typeLength];
			int index = 0;
			byte[] mass = new byte[typeLength];
			for (int i = 0; i < buff.Length; i++) {
				Buffer.BlockCopy(buffer, index, mass, 0, typeLength);
				index += typeLength;
				buff[i] = DateTime.FromBinary(BitConverter.ToInt64(mass, 0));
			}
			return buff;
		}
		public static TimeSpan[] ConvertToTimeSpan(byte[] buffer, int start, int length) {
			int typeLength = DXMarshal.SizeOf<Int64>();
			TimeSpan[] buff = new TimeSpan[length / typeLength];
			int index = 0;
			byte[] mass = new byte[typeLength];
			for (int i = 0; i < buff.Length; i++) {
				Buffer.BlockCopy(buffer, index, mass, 0, typeLength);
				index += typeLength;
				buff[i] = TimeSpan.FromTicks(BitConverter.ToInt64(mass, 0));
			}
			return buff;
		}
		public static decimal[] ConvertToDecimal(byte[] buffer, int start, int length) {
			int typeLength = 4 * DXMarshal.SizeOf<int>();
			decimal[] buff = new decimal[length / typeLength];
			int index = 0;
			int[] mass = new int[4];
			for (int i = 0; i < buff.Length; i++) {
				Buffer.BlockCopy(buffer, index, mass, 0, typeLength);
				index += typeLength;
				buff[i] = new decimal(mass);
			}
			return buff;
		}
		public static Guid[] ConvertToGuid(byte[] buffer, int start, int length) {
			int typeLength = TypeLength<Guid>();
			Guid[] buff = new Guid[length / typeLength];
			int index = 0;
			byte[] mass = new byte[typeLength];
			for (int i = 0; i < buff.Length; i++) {
				Buffer.BlockCopy(buffer, index, mass, 0, typeLength);
				index += typeLength;
				buff[i] = new Guid(mass);
			}
			return buff;
		}
		public static T[] Convert<T>(byte[] buffer, int start, int lengthInByte) {
			int typeLength = TypeLength<T>();
			T[] array = new T[lengthInByte / typeLength];
			Buffer.BlockCopy(buffer, 0, array, 0, lengthInByte);
			return array;
		}
		static byte[] Convert(List<Guid> guids) {
			int typeLength = TypeLength<Guid>();
			byte[] buff = new byte[guids.Count * typeLength];
			int index = 0;
			foreach (Guid guid in guids) {
				byte[] mass = guid.ToByteArray();
				Buffer.BlockCopy(mass, 0, buff, index, typeLength);
				index += typeLength;
			}
			return buff;
		}
		static byte[] Convert(List<decimal> decimals) {
			int typeLength = 4 * DXMarshal.SizeOf<int>();
			byte[] buff = new byte[decimals.Count * typeLength];
			int index = 0;
			foreach (decimal d in decimals) {
				int[] mass = Decimal.GetBits(d);
				Buffer.BlockCopy(mass, 0, buff, index, typeLength);
				index += typeLength;
			}
			return buff;
		}
		static byte[] Convert(List<TimeSpan> spans) {
			int typeLength = DXMarshal.SizeOf<Int64>();
			byte[] buff = new byte[spans.Count * typeLength];
			int index = 0;
			foreach (TimeSpan d in spans) {
				byte[] mass = BitConverter.GetBytes(d.Ticks);
				Buffer.BlockCopy(mass, 0, buff, index, typeLength);
				index += typeLength;
			}
			return buff;
		}
		static byte[] Convert(List<DateTime> dateTime) {
			int typeLength = DXMarshal.SizeOf<Int64>();
			byte[] buff = new byte[dateTime.Count * typeLength];
			int index = 0;
			foreach (DateTime d in dateTime) {
				byte[] mass = BitConverter.GetBytes(d.Ticks);
				Buffer.BlockCopy(mass, 0, buff, index, typeLength);
				index += typeLength;
			}
			return buff;
		}
		static byte[] Convert(List<string> strings) {
			int count = strings.Count;
			int[] lengths = new int[count];
			int stringsLength = 0;
			for (int i = 0; i < count; i++) {
				lengths[i] = strings[i].Length * 2;
				stringsLength += lengths[i];
			}
			int blockLength = 4 + 4 * count + stringsLength;
			byte[] buff = new byte[blockLength];
			int index = 0;
			index = CopyInt(buff, index, count);
			for (int i = 0; i < count; i++)
				index = CopyInt(buff, index, lengths[i]);
			for (int i = 0; i < count; i++) {
				int length = lengths[i];
				byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(strings[i]);
				for (int j = 0; j < stringBytes.Length; j++)
					buff[j + index] = stringBytes[j];
				index += length;
			}
			return buff;
		}
		static byte[] Convert(List<byte[]> blobs) {
			int count = blobs.Count;
			int[] lengths = new int[count];
			int blobsLength = 0;
			for (int i = 0; i < count; i++) {
				lengths[i] = blobs[i].Length;
				blobsLength += lengths[i];
			}
			int blockLength = 4 + 4 * count + blobsLength;
			byte[] buff = new byte[blockLength];
			int index = 0;
			index = CopyInt(buff, index, count);
			for (int i = 0; i < count; i++)
				index = CopyInt(buff, index, lengths[i]);
			for (int i = 0; i < count; i++) {
				int length = lengths[i];
				for (int j = 0; j < blobs[i].Length; j++)
					buff[j + index] = blobs[i][j];
				index += length;
			}
			return buff;
		}
		static byte[] ConvertPrimitive<T>(List<T> values) {
			int typeLength = TypeLength<T>();
			T[] array = values.ToArray();
			byte[] buff = new byte[values.Count * typeLength];
			Buffer.BlockCopy(array, 0, buff, 0, values.Count * typeLength);
			return buff;
		}
		static int CopyInt(byte[] buff, int offset, int value) {
			byte[] intBytes = BitConverter.GetBytes(value);
			for (int j = 0; j < intBytes.Length; j++)
				buff[j + offset] = intBytes[j];
			return offset + DXMarshal.SizeOf<Int32>();
		}
		static int TypeLength<T>() {
			if (typeof(T) == typeof(byte[]))
				return 1;
			if (typeof(T) == typeof(char))
				return 2;
			else if (typeof(T) == typeof(DateTime))
				return 8;
			else if (typeof(T) == typeof(bool))
				return 1;
			else if (typeof(T) == typeof(Guid))
				return 16;
			return DXMarshal.SizeOf<T>();
		}
	}
	public class Table : IStorageTable {
		string name;
		int rowCount;
		int columnCount;
		List<IStorageColumn> columns;
		FileStorage storage;
		public Table(string name, int rowCount, int columnCount, FileStorage storage) {
			this.name = name;
			this.rowCount = rowCount;
			this.columns = new List<IStorageColumn>();
			this.storage = storage;
			this.columns = new List<IStorageColumn>();
			this.columnCount = columnCount;
		}
		public Table(TableHeader header, FileStorage storage) {
			this.name = FileStorage.GetName(header.Name);
			this.storage = storage;
			this.rowCount = header.RowCount;
			this.columns = new List<IStorageColumn>();
		}
		#region IStorageTable
		string IStorageTable.Name { get { return name; } }
		IStorageColumn IStorageTable.this[string colName] {
			get {
				foreach (IStorageColumn col in columns)
					if (col.Name == colName)
						return col;
				return null;
			}
		}
		List<IStorageColumn> IStorageTable.Columns { get { return columns; } }
		IStorageColumn IStorageTable.AddColumn(string name, int uniqueCount, Type type) {
			if (this.columns.Count == columnCount)
				throw new Exception("to many columns");
			Column column = new Column(name, uniqueCount, type, CompressionType.Dictionary, 0, storage);
			this.columns.Add(column);
			storage.AddColumn(column);
			return column;
		}
		#endregion IStorageTable
		internal void AddColumn(List<Column> columns) { this.columns.AddRange(columns); }
		public TableHeader GetHeader() {
			TableHeader header = new TableHeader();
			header.ColumnCount = columns.Count;
			header.Name = FileStorage.GetNameArray(this.name);
			header.RowCount = rowCount;
			header.Version = 1;
			return header;
		}
	}
	public class Column : IStorageColumn {
		string name;
		Type type;
		CompressionType compressionType;
		FileStorage storage;
		int dataOffset;
		Int32 uniqueCount;
		Int32 indexForUniqueValues;
		Int32 compressedBlocksCount;
		Int32 recordsCount;
		public int UniqueCount { get { return uniqueCount; } }
		public int IndexForUniqueValues { get { return indexForUniqueValues; } set { indexForUniqueValues = value; } }
		public int CompressedBlocksCount { get { return compressedBlocksCount; } set { compressedBlocksCount = value; } }
		public int Records { get { return recordsCount; } set { recordsCount = value; } }
		public int DataOffset {
			get { return dataOffset; }
			set { dataOffset = value; }
		}
		public string Name { get { return name; } }
		public Type Type { get { return type; } }
		public Column(string name, int uniqueCount, Type type, CompressionType compressionType, int compressedBlocksCount, FileStorage storage) {
			this.storage = storage;
			this.name = name;
			this.uniqueCount = uniqueCount;
			this.type = type;
			this.compressionType = compressionType;
			this.compressedBlocksCount = compressedBlocksCount;
		}
		public Column(FileStorage storage) {
			this.storage = storage;
		}
		public Column(ColumnHeader column, FileStorage storage) {
			this.storage = storage;
			this.name = FileStorage.GetName(column.Name);
			this.type = FileStorage.GetTypeByDescriptor(column.Type);
			this.compressionType = column.CompressionType;
			this.dataOffset = column.DataOffset;
			this.uniqueCount = column.UniqueCount;
			this.compressedBlocksCount = column.CompressedBlocksCount;
			this.recordsCount = column.RecordsCount;
		}
		#region IStorageTable
		string IStorageColumn.Name { get { return name; } }
		int IStorageColumn.Records { get { return recordsCount; } }
		int IStorageColumn.UniqueCount { get { return uniqueCount; } }
		Type IStorageColumn.Type { get { return type; } }
		int IStorageColumn.CompressedBlocksCount { get { return compressedBlocksCount; } }
		void IStorageColumn.WriteUniqueValues<T>(List<T> list) {
			storage.WriteTypedValues<T>(this, list);
		}
		T[] IStorageColumn.ReadColumnUniqueValues<T>(int start, int count) {
			return storage.ReadTypedValues<T>(this, start, count);
		}
		int[] IStorageColumn.ReadDecompressedValues(int length, ColumnReadPosition readPposition) {
			throw new NotSupportedException();
		}
		void IStorageColumn.WriteCompressedValues(int startIndex, int endIndex, Byte[] values) {
			storage.WriteCompressedValues(this, startIndex, endIndex, values);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		Byte[] IStorageColumn.ReadCompressedValues(int compressedBlockIndex, out int startIndex, out int endIndex) {
			return storage.ReadCompressedValues(this, compressedBlockIndex, out startIndex, out endIndex);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		int[] IStorageColumn.ReadDecompressedValues(int compressedBlockIndex, out int length) {
			return storage.ReadDecompressedValues(this, compressedBlockIndex, out length);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		bool IStorageColumn.GetCompressedBlockMetadata(int compressedBlockIndex, out int startIndex, out int endIndex, out int offset, out int length) {
			return storage.GetCompressedBlockMetadata(this, compressedBlockIndex, out  startIndex, out  endIndex, out  offset, out  length);
		}
		#endregion IStorageTable
		public ColumnHeader GetHeader() {
			ColumnHeader header = new ColumnHeader();
			header.DataOffset = dataOffset;
			header.Name = FileStorage.GetNameArray(this.name);
			header.Type = FileStorage.GetDescriptorByType(type);
			header.UniqueCount = uniqueCount;
			header.CompressionType = compressionType;
			header.CompressedBlocksCount = compressedBlocksCount;
			header.RecordsCount = recordsCount;
			return header;
		}
	}
}
