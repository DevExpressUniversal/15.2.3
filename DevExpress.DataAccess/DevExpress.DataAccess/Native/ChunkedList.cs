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
using System.Data;
using System.Linq;
using System.Threading;
using DevExpress.Xpo.DB;
using DevExpress.Utils;
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.DataAccess.Native {
	public class ChunkedList<T> : IList, IList<T> {
		class Enumerator : IEnumerator<T> {
			readonly ChunkedList<T> owner;
			int chunkIndex;
			IEnumerator<T> chunkEnumerator;
			bool disposed;
			public Enumerator(ChunkedList<T> owner) {
				this.owner = owner;
				Reset();
			}
			#region Implementation of IEnumerator
			public void Reset() {
				if(disposed)
					throw new ObjectDisposedException(null);
				chunkIndex = 0;
				CreateChunkEnumerator();
			}
			public bool MoveNext() {
				if(disposed)
					throw new ObjectDisposedException(null);
				if(chunkEnumerator == null)
					return false;
				if(chunkEnumerator.MoveNext())
					return true;
				chunkIndex++;
				CreateChunkEnumerator();
				return chunkEnumerator != null;
			}
			object IEnumerator.Current { get { return Current; } }
			#endregion
			#region Implementation of IEnumerator<out T>
			public T Current {
				get {
					if(disposed)
						throw new ObjectDisposedException(null);
					return chunkEnumerator.Current;
				}
			}
			#endregion
			#region Implementation of IDisposable
			public void Dispose() {
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			#endregion
			void Dispose(bool disposing) {
				if(disposed)
					return;
				if(disposing) {
					if(chunkEnumerator != null)
						chunkEnumerator.Dispose();
					chunkEnumerator = null;
				}
				disposed = true;
			}
			void CreateChunkEnumerator() {
				if(chunkEnumerator != null)
					chunkEnumerator.Dispose();
				chunkEnumerator = chunkIndex < owner.arrayList.Count
					? owner.arrayList[chunkIndex].Cast<T>().GetEnumerator()
					: null;
			}
			~Enumerator() { Dispose(false); }
		}
		readonly List<T[]> arrayList = new List<T[]>();
		int position;
		public ChunkedList(int chunkSize) {
			ChunkSize = chunkSize;
			this.position = chunkSize;
		}
		int ChunkSize { get; set; }
		public T this[int index] {
			get {
				int indexOfChunk = index / ChunkSize;
				int indexOfItemWithinChunk = index % ChunkSize;
				return (T)((IList)this.arrayList[indexOfChunk])[indexOfItemWithinChunk];
			}
			set {
				throw new NotSupportedException();
			}
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public int Count {
			get { return (this.arrayList.Count - 1) * ChunkSize + this.position; }
		}
		public bool IsReadOnly {
			get { throw new NotSupportedException(); }
		}
		#region IList Members
		bool IList.Contains(object value) {
			throw new NotSupportedException();
		}
		#endregion
		#region IList<T> Members
		public int IndexOf(T item) {
			throw new NotSupportedException();
		}
		public void Insert(int index, T item) {
			throw new NotSupportedException();
		}
		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				throw new NotSupportedException();
			}
		}
		#endregion
		#region ICollection<T> Members
		public void Add(T item) {
			if(this.position == ChunkSize) {
				var array = new T[ChunkSize];
				this.arrayList.Add(array);
				this.position = 0;
			}
			this.arrayList[this.arrayList.Count - 1][this.position] = item;
			this.position++;
		}
		public void CopyTo(T[] array, int arrayIndex) {
			throw new NotSupportedException();
		}
		public bool Remove(T item) {
			throw new NotSupportedException();
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new Enumerator(this);
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			Add((T)value);
			return Count - 1;
		}
		void IList.Clear() {
			throw new NotSupportedException();
		}
		int IList.IndexOf(object value) {
			throw new NotSupportedException();
		}
		void IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		bool IList.IsFixedSize {
			get {
				throw new NotSupportedException();
			}
		}
		bool IList.IsReadOnly {
			get {
				return true;
			}
		}
		void IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		bool ICollection.IsSynchronized {
			get {
				throw new NotSupportedException();
			}
		}
		object ICollection.SyncRoot {
			get {
				throw new NotSupportedException();
			}
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}
		#endregion
		#region IList<T> Members
		void IList<T>.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		#endregion
		#region ICollection<T> Members
		public bool Contains(T item) {
			throw new NotSupportedException();
		}
		#endregion
	}
	public static class DataStoreExHelper {
		const int chunkSize = 4096;
		static readonly Dictionary<Type, Action<int, IDataReader, IList>> readers = new Dictionary<Type, Action<int, IDataReader, IList>> { 
		{ typeof(decimal), (i, reader, list) => ReadStruct(reader, (ChunkedList<decimal?>)list, i, (r, index) => r.GetDecimal(index)) }, 
		{ typeof(float), (i, reader, list) => ReadStruct(reader, (ChunkedList<float?>)list, i, (r, index) => r.GetFloat(index)) },
		{ typeof(bool), (i, reader, list) => ReadStruct(reader, (ChunkedList<bool?>)list, i, (r, index) => r.GetBoolean(index))},
		{ typeof(string), (i, reader, list) => ReadClass(reader, (ChunkedList<string>)list, i, (r, index) => r.GetString(index))},
		{ typeof(double), (i, reader, list) => ReadStruct(reader, (ChunkedList<double?>)list, i, (r, index) => r.GetDouble(index))}, 
		{ typeof(byte), (i, reader, list) => ReadStruct(reader, (ChunkedList<byte?>)list, i, (r, index) => r.GetByte(index))}, 
		{ typeof(sbyte), (i, reader, list) => ReadStruct(reader, (ChunkedList<sbyte?>)list, i, (r, index) => (sbyte)r.GetValue(index))}, 
		{ typeof(short), (i, reader, list) => ReadStruct(reader, (ChunkedList<short?>)list, i, (r, index) => r.GetInt16(index))},
		{ typeof(ushort), (i, reader, list) => ReadStruct(reader, (ChunkedList<ushort?>)list, i, (r, index) => (ushort)r.GetValue((index)))},
		{ typeof(ulong), (i, reader, list) => ReadStruct(reader, (ChunkedList<ulong?>)list, i, (r, index) => (ulong)r.GetValue((index)))},
		{ typeof(uint), (i, reader, list) => ReadStruct(reader, (ChunkedList<uint?>)list, i, (r, index) => (uint)r.GetValue((index)))},
		{ typeof(int), (i, reader, list) => ReadStruct(reader, (ChunkedList<int?>)list, i, (r, index) => r.GetInt32(index))} ,
		{ typeof(long), (i, reader, list) => ReadStruct(reader, (ChunkedList<long?>)list, i, (r, index) => r.GetInt64(index))} ,
		{ typeof(DateTime), (i, reader, list) => ReadStruct(reader, (ChunkedList<DateTime?>)list, i, (r, index) => r.GetDateTime(index))} ,
		{ typeof(byte[]), (i, reader, list) => ReadClass(reader, (ChunkedList<byte[]>)list, i, (r, index) => (byte[])r.GetValue(index))}, 
		{ typeof(object), (i, reader, list) => ReadClass(reader, (ChunkedList<object>)list, i, (r, index) => r.GetValue(index))}, 
		{ typeof(Guid), (i, reader, list) => ReadStruct(reader, (ChunkedList<Guid?>)list, i, GetGuid)}, 
		{ typeof(TimeSpan), (i, reader, list) => ReadStruct(reader, (ChunkedList<TimeSpan?>)list, i, GetTimeSpan)}, 
		{ typeof(char), (i, reader, list) => ReadStruct(reader, (ChunkedList<char?>)list, i, GetChar)}, 
		};
		static Char GetChar(IDataReader dataReader, int index) {
			try {
				return dataReader.GetChar(index);
			}
			catch (NotSupportedException) {
			}
			string str = (string)dataReader.GetValue(index);
			return str.Length == 0 ? ' ' : Convert.ToChar(str);
		}
		static TimeSpan GetTimeSpan(IDataReader dataReader, int index) {
			object value = dataReader.GetValue(index);
			if(value is TimeSpan)
				return (TimeSpan)value;
			double seconds = Convert.ToDouble(value);
			if(seconds > TimeSpan.MaxValue.TotalSeconds - 0.0005 && seconds < TimeSpan.MaxValue.TotalSeconds + 0.0005)
				return TimeSpan.MaxValue;
			if(seconds < TimeSpan.MinValue.TotalSeconds + 0.0005 && seconds > TimeSpan.MinValue.TotalSeconds - 0.0005)
				return TimeSpan.MinValue;
			return TimeSpan.FromSeconds(seconds);
		}
		static Guid GetGuid(IDataReader dataReader, int index) {
			object value = dataReader.GetValue(index);
			return value is byte[] ? new Guid((byte[])value) : new Guid(value.ToString());
		}
		static void ReadStruct<T>(IDataReader dataReader, ChunkedList<T?> list, int index, Func<IDataReader, int, T> getValue) where T : struct {
			if(dataReader.IsDBNull(index))
				list.Add(null);
			else {
				list.Add(getValue(dataReader, index));
			}
		}
		static void ReadClass<T>(IDataReader dataReader, ChunkedList<T> list, int index, Func<IDataReader, int, T> getValue) where T : class {
			if(dataReader.IsDBNull(index))
				list.Add(null);
			else {
				list.Add(getValue(dataReader, index));
			}
		}
		public static SelectedDataEx GetData(IDbCommand command, CancellationToken cancellationToken) {
			return GetData(command, cancellationToken, null, true, 0);
		}
		public static SelectedDataEx GetData(IDbCommand command, CancellationToken cancellationToken, string[] columns) {
			return GetData(command, cancellationToken, columns, true, 0);
		}
		public static SelectedDataEx GetData(IDbCommand command, CancellationToken cancellationToken, bool nativeSkipSupport, int skipCount) {
			return GetData(command, cancellationToken, null, nativeSkipSupport, skipCount);
		}
		public static SelectedDataEx GetData(IDbCommand command, CancellationToken cancellationToken, string[] columns, bool nativeSkipSupport, int skipCount) {
			using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
				ColumnInfoEx[] schema;
				if(columns == null) {
					schema = new ColumnInfoEx[reader.FieldCount];
					for(int fld = 0; fld < reader.FieldCount; fld++)
						schema[fld].Name = reader.GetName(fld);
				} else {
					schema = new ColumnInfoEx[columns.Length];
					for(int col = 0; col < columns.Length; col++)
						schema[col].Name = columns[col];
				}
				for(int i = 0; i < reader.FieldCount; i++)
					schema[i].Type = reader.GetFieldType(i);
				List<IList> result = CreateChunkedLists(schema);
				if(!nativeSkipSupport)
					for(int s = 0; s < skipCount; s++)
						reader.Read();
				while(reader.Read()) {
					for(int i = 0; i < schema.Length; i++) {
						if(readers.ContainsKey(schema[i].Type))
							readers[schema[i].Type](i, reader, result[i]);
						else
							readers[typeof(object)](i, reader, result[i]);
					}
					cancellationToken.ThrowIfCancellationRequested();
				}
				return new SelectedDataEx(result.ToArray(), schema);
			}
		}
		public static SelectedDataEx GetData(SelectedData selectedData, ColumnInfoEx[] schema) {
			List<IList> result = CreateChunkedLists(schema);
			for(int i = 0; i < selectedData.ResultSet[0].Rows.Length; i++)
				for(int j = 0; j < selectedData.ResultSet[0].Rows[i].Values.Length; j++)
					result[j].Add(selectedData.ResultSet[0].Rows[i].Values[j]);
			return new SelectedDataEx(result.ToArray(), schema);
		}
		public static ColumnInfoEx[] GetSchema(IDbCommand command) {
			using(IDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly)) {
				ColumnInfoEx[] schema = new ColumnInfoEx[reader.FieldCount];
				for(int fld = 0; fld < reader.FieldCount; fld++)
					schema[fld].Name = reader.GetName(fld);
				for(int i = 0; i < reader.FieldCount; i++)
					schema[i].Type = reader.GetFieldType(i);
				return schema;
			}
		}
		public static List<IList> CreateChunkedLists(ColumnInfoEx[] schema) {
			List<IList> result = new List<IList>();
			for(int i = 0; i < schema.Length; i++) {
				Type chunkedListType;
				if(readers.ContainsKey(schema[i].Type)) {
					if(schema[i].Type.IsValueType())
						chunkedListType = typeof(ChunkedList<>).MakeGenericType(typeof(Nullable<>).MakeGenericType(schema[i].Type));
					else
						chunkedListType = typeof(ChunkedList<>).MakeGenericType(schema[i].Type);
				} else {
#if DEBUGTEST
					System.Diagnostics.Debug.Assert(false, string.Format("Unknown type {0}", schema[i].Type.FullName));
#endif
					chunkedListType = typeof(ChunkedList<>).MakeGenericType(typeof(object));
				}
				IList list = (IList)Activator.CreateInstance(chunkedListType, chunkSize);
				result.Add(list);
			}
			return result;
		}
	}
}
