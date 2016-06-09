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
namespace DevExpress.DashboardCommon.DataProcessing {
	public interface ITypedDataReader {
		int FieldCount { get; }
		Type GetFieldType(int i);
		string GetFieldName(int i);
		bool Read();
		bool IsNull(int i);
		T GetValue<T>(int i);
	}
	public struct ColumnDescription {
		public String Name { get; set; }
		public Type Type { get; set; }
		public Int32 UniqueCount { get; set; }
		public Int32 DataInBytes { get; set; }
	}
	public class ColumnReadPosition {
		public int CurrentRecordIndex { get; set; }
		public int CurrentByteInBlockIndex { get; set; }
		public int ReadingOffsetBitInByte { get; set; }
		public int ReadValuesCount { get; set; }
		public ColumnReadPosition() {
			CurrentRecordIndex = 0;
			CurrentByteInBlockIndex = 0;
			ReadingOffsetBitInByte = 0;
			ReadValuesCount = 0;
		}
	}
	public interface IRequestBroker {
		void SendRequest(ITask task);
		void ClientCheck();
		TaskLocker CreateTaskLocker();
		void DestroyTaskLocker(TaskLocker locker);
	}
	public interface ITask {
		bool IsReady { get; set; }
		TaskValuesKind ValuesKind { get; }
		int StartIndex { get; }
		int RecordCount { get; }
		string ColumnName { get; }
		int MoveToBuffer<IN>(CircularBuffer<IN> input);
		int CopyToBuffer<IN>(CircularBuffer<IN> input);
	}
	public interface IFileStorage : IDisposable {
		IStorageTable CreateTable(string name, int rowCount, int columnsCount);
		IStorageTable GetTable();
		void Clear();
	}
	public interface IStorageTable {
		string Name { get; }
		IStorageColumn AddColumn(string name, int uniqueCount, Type type);
		IStorageColumn this[string colName] { get; }
		List<IStorageColumn> Columns { get; }
	}
	public interface IStorageColumn {
		string Name { get; }
		int CompressedBlocksCount { get; }
		int UniqueCount { get; }
		Type Type { get; }
		int Records { get; }
		void WriteUniqueValues<T>(List<T> list);
		T[] ReadColumnUniqueValues<T>(int start, int count);
		void WriteCompressedValues(int startIndex, int endIndex, Byte[] values);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		Byte[] ReadCompressedValues(int compressedBlockIndex, out int startIndex, out int endIndex);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		int[] ReadDecompressedValues(int compressedBlockIndex, out int length);
		int[] ReadDecompressedValues(int length, ColumnReadPosition position);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		bool GetCompressedBlockMetadata(int compressedBlockIndex, out int startIndex, out int endIndex, out int offset, out int length);
	}
	public interface IThreadObject {
		void Start();
		void Stop();
		bool Join(int millisecondsTimeout);
	}
}
