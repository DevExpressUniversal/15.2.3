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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using System.IO.MemoryMappedFiles;
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class DataStorageProcessor {
		ITypedDataReader dataReader;
		bool shouldCompress;
		int rowProcessingLimit;
		public DataStorageProcessor(ITypedDataReader dataReader)
			: this(dataReader, DataProcessingOptions.DefaultRowProcessingLimit, true) { }
		public DataStorageProcessor(ITypedDataReader dataReader, int rowProcessingLimit)
			: this(dataReader, rowProcessingLimit, true) { }
		public DataStorageProcessor(ITypedDataReader dataReader, int rowProcessingLimit, bool shouldCompress) {
			this.dataReader = dataReader;
			this.rowProcessingLimit = rowProcessingLimit;
			this.shouldCompress = shouldCompress;
		}
		ColumnProcessorBase GetProcessor(Type columnType, string name) {
			return GenericActivator.New<ColumnProcessorBase>(typeof(ColumnProcessor<>), columnType, name, rowProcessingLimit);
		}
		List<ColumnProcessorBase> CreateProcessors() {
			List<ColumnProcessorBase> columnProcessors = new List<ColumnProcessorBase>();
			for (int j = 0; j < dataReader.FieldCount; j++) {
				ColumnProcessorBase processor = GetProcessor(dataReader.GetFieldType(j), dataReader.GetFieldName(j));
				columnProcessors.Add(processor);
			}
			return columnProcessors;
		}
		IStorageTable PrepareTempFileStorage(IFileStorage storage) {
			IStorageTable fileTable = storage.CreateTable("UniqueData", 0, dataReader.FieldCount);
			for (int i = 0; i < dataReader.FieldCount; i++)
				fileTable.AddColumn(dataReader.GetFieldName(i), 0, typeof(int));
			return fileTable;
		}
		void Process(IFileStorage fileStorage, IFileStorage tempFileStorage, CancellationToken cancellationToken) {
			IStorageTable fileTable = fileStorage.CreateTable("DataTable", 0, dataReader.FieldCount);
			IStorageTable tempFileTable = PrepareTempFileStorage(tempFileStorage);
			List<ColumnProcessorBase> columnProcessors = CreateProcessors();
			List<ColumnReaderBase> columnReaders = columnProcessors.Select((processor, i) => ColumnReaderBase.New(processor, dataReader, i)).ToList();
			int rowProcessing = 0;
			while (dataReader.Read()) {
				if (cancellationToken.IsCancellationRequested)
					return;
				foreach (ColumnReaderBase reader in columnReaders)
					reader.Read();
				rowProcessing++;
				if (rowProcessing >= rowProcessingLimit) {
					rowProcessing = 0;
					for (int i = 0; i < columnProcessors.Count; i++)
						columnProcessors[i].SaveProcessingData(tempFileTable.Columns[i]);
				}
			}
			for (int i = 0; i < dataReader.FieldCount; i++) {
				if (cancellationToken.IsCancellationRequested)
					return;
				columnProcessors[i].FinishProcessing(tempFileTable.Columns[i]);
				fileTable.AddColumn(columnProcessors[i].ColumnName, columnProcessors[i].UniqueCount, columnProcessors[i].DataType);
				columnProcessors[i].WriteUniqueData(fileTable.Columns[i]);
				columnProcessors[i].SaveIndexesSortedByValues(tempFileTable.Columns[i], fileTable.Columns[i], shouldCompress);
			}
		}
		public void ProcessTables(string path) {
			if (!string.IsNullOrEmpty(path)) {
				IFileStorage fileStorage = FileStorage.CreateFileStorage(path);
				IFileStorage tempFileStorage = FileStorage.CreateFileStorage(path + ".tmp");
				Process(fileStorage, tempFileStorage, CancellationToken.None);
				tempFileStorage.Dispose();
				fileStorage.Dispose();
			}
		}
		public IFileStorage ProcessTablesInMemory(string path) {
			IFileStorage fileStorage = null;
			if (!string.IsNullOrEmpty(path)) {
				fileStorage = FileStorage.CreateMemoryMappedFileStorage(path);
				IFileStorage tempFileStorage = FileStorage.CreateMemoryMappedFileStorage(path + ".tmp");
				Process(fileStorage, tempFileStorage, CancellationToken.None);
				tempFileStorage.Dispose();
			}
			return fileStorage;
		}
		public IFileStorage ProcessTablesInNonPersistentMemory(MemoryMappedFile file) {
			IFileStorage fileStorage = null;
			if (file != null) {
				fileStorage = FileStorage.CreateNonPersistentMemoryMappedFileStorage(file);
				MemoryMappedFile tempFile = MemoryMappedFile.CreateNew("tempNonPersistentMemory", NonPersistentMMFileWorker.MMFLimit);
				IFileStorage tempFileStorage = FileStorage.CreateNonPersistentMemoryMappedFileStorage(tempFile);
				Process(fileStorage, tempFileStorage, CancellationToken.None);
				tempFileStorage.Dispose();
				if (tempFile != null)
					tempFile.Dispose();
			}
			return fileStorage;
		}
		public IStorage ProcessTables() {
			return ProcessTables(CancellationToken.None);
		}
		public IStorage ProcessTables(CancellationToken cancellationToken) {
			IFileStorage fileStorage = MemoryStorage.CreateStorage();
			IFileStorage tempFileStorage = MemoryStorage.CreateStorage();
			Process(fileStorage, tempFileStorage, cancellationToken);
			tempFileStorage.Dispose();
			if (!cancellationToken.IsCancellationRequested)				
				return new MemorySource((MemoryStorage)fileStorage);
			else {
				fileStorage.Dispose();
				return null;
			}
		}
	}
	public abstract class ColumnReaderBase {
		protected ColumnProcessorBase ProcessorBase { get; private set; }
		protected ITypedDataReader Reader { get; private set; }
		protected int ColumnIndex { get; private set; }
		protected ColumnReaderBase(ColumnProcessorBase processor, ITypedDataReader reader, int columnIndex) {
			this.ProcessorBase = processor;
			this.Reader = reader;
			this.ColumnIndex = columnIndex;
		}
		public abstract void Read();
		public static ColumnReaderBase New(ColumnProcessorBase processor, ITypedDataReader reader, int columnIndex) {
			return GenericActivator.New<ColumnReaderBase>(typeof(ColumnReader<>), processor.DataType, processor, reader, columnIndex);
		}
	}
	public class ColumnReader<T> : ColumnReaderBase {
		ColumnProcessor<T> Processor { get { return (ColumnProcessor<T>)ProcessorBase; } }
		public ColumnReader(ColumnProcessorBase processor, ITypedDataReader reader, int columnIndex) : base(processor, reader, columnIndex) { }
		public override void Read() {
			T value = default(T);
			bool isNullValue = Reader.IsNull(ColumnIndex);
			if(!isNullValue)
				value = Reader.GetValue<T>(ColumnIndex);
			Processor.Process(value, isNullValue);
		}
	}
}
