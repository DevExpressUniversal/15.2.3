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

using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode {
	abstract class ServerMeasureStorage : MeasuresStorage {
		readonly Dictionary<IQueryMetadataColumn, int> indexes;
		internal Dictionary<IQueryMetadataColumn, int> Indexes { get { return indexes; } }
		public override int Count {
			get { return indexes.Count; }
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes) {
			this.indexes = indexes;
		}
		public override PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure) {
			int index;
			if(indexes.TryGetValue(measure, out index))
				return new PivotCellValue(Get(index));
			return null;
		}
		public override object GetValue(IQueryMetadataColumn measure) {
			int index;
			if(indexes.TryGetValue(measure, out index))
				return Get(index);
			return null;
		}
		public override bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale) {
			return false;
		}
		internal override void Remove(IQueryMetadataColumn column) {
			indexes.Remove(column);
		}
		protected internal abstract object Get(int dataIndex);
		public override void SaveToStream(MeasureStorageKeepHelperBase helper, Data.IO.TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			((MeasureStorageKeepHelper)helper).SaveStorageFlag(this, writer);
			Write(writer);
		}
		internal abstract void Write(TypedBinaryWriter writer);
		protected static class Helper<TType> where TType : MeasuresStorage {
			static Action<TType, TypedBinaryWriter> write;
			static Action<TType, TypedBinaryReader> read;
			public static Action<TType, TypedBinaryWriter> GetWriteDelegate() {
				if(write == null)
					write = DevExpress.PivotGrid.ServerMode.ServerMeasureStorage.LambdaHelper.CreateWriter<TType>(typeof(TType).GetGenericArguments());
				return write;
			}
			public static Action<TType, TypedBinaryReader> GetReadDelegate() {
				if(read == null)
					read = DevExpress.PivotGrid.ServerMode.ServerMeasureStorage.LambdaHelper.CreateReader<TType>(typeof(TType).GetGenericArguments());
				return read;
			}
		}
		static class LambdaHelper {
			public static Action<TType, TypedBinaryWriter> CreateWriter<TType>(Type[] types) {
				ParameterExpression storage = Expression.Parameter(typeof(TType), "");
				ParameterExpression writer = Expression.Parameter(typeof(TypedBinaryWriter), "");
				List<Expression> ops = new List<Expression>(types.Length);
				ParameterExpression nullableTypeParam = Expression.Parameter(typeof(int), "isNullFlag");
				List<Type> nullableTypes = new List<Type>(types.Length);
				List<bool> startsBoolNullable = new List<bool>();
				int freeCount = 0;
				for(int i = 0; i < types.Length; i++) {
					Type nullableType = Nullable.GetUnderlyingType(types[i]);
					nullableTypes.Add(nullableType);
					if(nullableType != null) {
						if(freeCount == 0) {
							freeCount = 7;
							startsBoolNullable.Add(true);
						} else {
							startsBoolNullable.Add(false);
							freeCount--;
						}
					} else
						startsBoolNullable.Add(false);
				}
				for(int i = 0; i < types.Length; i++) {
					Expression expr;
					if(nullableTypes[i] == null)
						expr = GetWriteExpression(writer, Expression.Property(storage, "Item" + i.ToString()));
					else {
						if(startsBoolNullable[i]) {
							ops.Add(Expression.Assign(nullableTypeParam, Expression.Constant(0)));
							int counter = 0;
							int bitCounter = 1;
							for(int j = i; j < types.Length; j++) {
								if(counter == 8)
									break;
								if(nullableTypes[j] == null)
									continue;
								ops.Add(Expression.IfThen(
									Expression.Equal(Expression.Constant(null), Expression.Property(storage, "Item" + j.ToString())),
									Expression.Assign(nullableTypeParam, Expression.Or(nullableTypeParam, Expression.Constant(bitCounter)))));
								counter++;
								bitCounter = bitCounter * 2;
							}
							ops.Add(Expression.Call(writer, "Write", null, Expression.Convert(nullableTypeParam, typeof(byte))));
						}
						expr = Expression.IfThen(
								Expression.NotEqual(Expression.Constant(null), Expression.Property(storage, "Item" + i.ToString())),
								GetWriteExpression(writer, Expression.Property(Expression.Property(storage, "Item" + i.ToString()), "Value"))
							);
					}
					ops.Add(expr);
				}
				Expression<Action<TType, TypedBinaryWriter>> func = Expression.Lambda<Action<TType, TypedBinaryWriter>>(
																										Expression.Block(new ParameterExpression[] { nullableTypeParam }, ops),
																										storage,
																										writer
																									);
				return func.Compile();
			}
			public static Action<TType, TypedBinaryReader> CreateReader<TType>(Type[] types) {
				ParameterExpression storage = Expression.Parameter(typeof(TType), "");
				ParameterExpression reader = Expression.Parameter(typeof(TypedBinaryReader), "");
				ParameterExpression nullableTypeParam = Expression.Parameter(typeof(int), "isNullFlag");
				List<Type> nullableTypes = new List<Type>(types.Length);
				List<bool> startsBoolNullable = new List<bool>();
				List<int> isNullBits = new List<int>();
				int bitCounter = 1;
				int freeCount = 0;
				for(int i = 0; i < types.Length; i++) {
					Type nullableType = Nullable.GetUnderlyingType(types[i]);
					nullableTypes.Add(nullableType);
					if(nullableType != null) {
						if(freeCount == 0) {
							freeCount = 7;
							startsBoolNullable.Add(true);
							bitCounter = 1;
						} else {
							startsBoolNullable.Add(false);
							freeCount--;
							bitCounter = bitCounter * 2;
						}
						isNullBits.Add(bitCounter);
					} else {
						startsBoolNullable.Add(false);
						isNullBits.Add(0);
					}
				}
				List<Expression> ops = new List<Expression>(types.Length);
				for(int i = 0; i < types.Length; i++) {
					Expression expr;
					Type nullableUnderlying = nullableTypes[i];
					if(nullableUnderlying == null) {
						expr = Expression.Assign(
								   Expression.Property(storage, "Item" + i.ToString()),
								   GetReadExpression(reader, types[i])
									   );
					} else {
						if(startsBoolNullable[i])
							ops.Add(Expression.Assign(nullableTypeParam, Expression.Convert(Expression.Call(reader, "ReadByte", null), typeof(int))));
						expr = Expression.IfThenElse(
											Expression.NotEqual(Expression.Constant(0), Expression.And(nullableTypeParam, Expression.Constant(isNullBits[i]))),
											Expression.Assign(
													Expression.Property(storage, "Item" + i.ToString()),
													Expression.Constant(null, types[i])
													),
											Expression.Assign(
													Expression.Property(storage, "Item" + i.ToString()),
													Expression.Convert(GetReadExpression(reader, nullableUnderlying), types[i])
													)
							);
					}
					ops.Add(expr);
				}
				Expression<Action<TType, TypedBinaryReader>> func = Expression.Lambda<Action<TType, TypedBinaryReader>>(
																										Expression.Block(new ParameterExpression[] { nullableTypeParam }, ops),
																										storage,
																										reader
																									);
				return func.Compile();
			}
			static Expression GetWriteExpression(ParameterExpression writer, Expression property) {
				string simpleTypeMethod = GetReadMethodByType(property.Type);
				if(!string.IsNullOrEmpty(simpleTypeMethod))
					return Expression.Call(writer, "Write", null, new Expression[] { property });
				else
					return Expression.Call(writer, "WriteObject", null, new Expression[] { Expression.Convert(property, typeof(object)) });
			}
			static Expression GetReadExpression(ParameterExpression reader, Type type) {
				string simpleTypeMethod = GetReadMethodByType(type);
				if(!string.IsNullOrEmpty(simpleTypeMethod))
				return Expression.Call(reader, simpleTypeMethod, null);
				else
					return Expression.Convert(Expression.Call(reader, "ReadObject", null, new Expression[] { Expression.Constant(type) }), type);
			}
			static string GetReadMethodByType(Type type) {
				if(type == typeof(bool))
					return "ReadBoolean";
				if(type == typeof(byte))
					return "ReadByte";
				if(type == typeof(char))
					return "ReadChar";
				if(type == typeof(decimal))
					return "ReadDecimal";
				if(type == typeof(double))
					return "ReadDouble";
				if(type == typeof(short))
					return "ReadInt16";
				if(type == typeof(int))
					return "ReadInt32";
				if(type == typeof(long))
					return "ReadInt64";
				if(type == typeof(sbyte))
					return "ReadSByte";
				if(type == typeof(float))
					return "ReadSingle";
				if(type == typeof(string))
					return "ReadString";
				if(type == typeof(ushort))
					return "ReadUInt16";
				if(type == typeof(uint))
					return "ReadUInt32";
				if(type == typeof(ulong))
					return "ReadUInt64";
				return null;
			}
		}
	}
	public class MeasureStorageKeepHelper : MeasureStorageKeepHelperBase {
		public class IndexesRecord {
			public Type[] Types { get; set; }
			public int Index { get; set; }
		}
		Dictionary<Dictionary<IQueryMetadataColumn, int>, IndexesRecord> cache = new Dictionary<Dictionary<IQueryMetadataColumn, int>, IndexesRecord>();
		List<Func<TypedBinaryReader, MeasuresStorage>> readedCache;
		int ucount;
		internal MeasureStorageKeepHelper(ServerModeCellTable table, bool save) {
			if(save)
				Index(table);
		}
		void Index(ServerModeCellTable table) {
			int count = 0;
			foreach(KeyValuePair<GroupInfo, GroupInfoColumn> pair0 in table) {
				foreach(KeyValuePair<GroupInfo, MeasuresStorage> pair1 in pair0.Value) {
					Dictionary<IQueryMetadataColumn, int> indexes = ((ServerMeasureStorage)pair1.Value).Indexes;
					if(!cache.ContainsKey(indexes)) {
						cache.Add(indexes, new IndexesRecord() { Types = indexes.Count > 5 ? ((ArrayMeasureStorage)pair1.Value).Types : pair1.Value.GetType().GetGenericArguments(), Index = count });
						count++;
					}
				}
			}
			List<KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord>> unique = new List<KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord>>();
			List<KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord>> ununique = new List<KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord>>();
			int counter = 0;
			foreach(KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord> pair1 in cache) {
				int index = unique.FindIndex((t) => {
					if(pair1.Key.Count != t.Key.Count || pair1.Value.Types.Length != t.Value.Types.Length)
						return false;
					for(int i = 0; i < pair1.Value.Types.Length; i++)
						if(pair1.Value.Types[i] != t.Value.Types[i])
							return false;
					return true;
				}
				);
				if(index >= 0) {
					ununique.Add(pair1);
					pair1.Value.Index = index;
				} else {
					unique.Add(pair1);
					pair1.Value.Index = counter;
					counter++;
				}
				ucount = counter;
			}
			cache.Clear();
			foreach(KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord> pair in unique)
				cache.Add(pair.Key, pair.Value);
			foreach(KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord> pair in ununique)
				cache.Add(pair.Key, pair.Value);
		}
		public override void SaveToStream(TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			writer.Write(ucount);
			int counter = 0;
			foreach(KeyValuePair<Dictionary<IQueryMetadataColumn, int>, IndexesRecord> pair2 in cache) {
				if(counter == ucount)
					break;
				writer.Write(pair2.Value.Types.Length);
				for(int i = 0; i < pair2.Value.Types.Length; i++)
					writer.WriteType(pair2.Value.Types[i]);
				writer.Write(pair2.Key.Count);
				foreach(KeyValuePair<IQueryMetadataColumn, int> pair3 in pair2.Key) {
					writer.Write(pair3.Value);
					writer.Write(columnIndexes[pair3.Key]);
				}
				counter++;
			}
		}
		public override void ReadFromStream(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes) {
			int cacheCount = reader.ReadInt32();
			readedCache = new List<Func<TypedBinaryReader,MeasuresStorage>>();
			for(int i = 0; i < cacheCount; i++) {
				int typeCount = reader.ReadInt32();
				Type[] types = new Type[typeCount];
				for(int j = 0; j < typeCount; j++)
					types[j] = reader.ReadType();
				int indexCount = reader.ReadInt32();
				Dictionary<IQueryMetadataColumn, int> indexes = new Dictionary<IQueryMetadataColumn, int>(indexCount);
				for(int j = 0; j < indexCount; j++) {
					int val = reader.ReadInt32();
					indexes.Add(columnIndexes[reader.ReadInt32()], val);
				}
				readedCache.Add(MeasureStorageMaterializer.Create(types, indexes));
			}
		}
		internal void SaveStorageFlag(ServerMeasureStorage serverMeasureStorage, TypedBinaryWriter writer) {
			if(ucount > 1)
				writer.Write((ushort)cache[serverMeasureStorage.Indexes].Index);
		}
		internal override MeasuresStorage Load(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes) {
			if(readedCache.Count > 1)
				return readedCache[reader.ReadUInt16()](reader);
			else
				return readedCache[0](reader);
		}
	}
	abstract class MeasureStorageMaterializer {
		public static Func<object[], MeasuresStorage> Create(Type[] types, int start, Dictionary<IQueryMetadataColumn, int> indexes) {
			object materializer;
			Type type = MakeStorageTypeByTypes(types);
			if(types.Length == 0)
				return null;
			if(types.Length <= 5)
				materializer = Activator.CreateInstance(type, new object[] { indexes, start });
			else
				materializer = Activator.CreateInstance(type, new object[] { indexes, start, types });
			return ((MeasureStorageMaterializer)materializer).MaterializeMethod;
		}
		public static Func<TypedBinaryReader, MeasuresStorage> Create(Type[] types, Dictionary<IQueryMetadataColumn, int> indexes) {
			object materializer;
			Type type = MakeStorageTypeByTypes(types);
			if(types.Length <= 5)
				materializer = Activator.CreateInstance(type, new object[] { indexes });
			else
				materializer = Activator.CreateInstance(type, new object[] { indexes, types });
			return ((MeasureStorageMaterializer)materializer).MaterializeMethod;
		}
		public static Type MakeStorageTypeByTypes(Type[] types) {
			if(types.Length == 1)
				return typeof(MeasureStorageMaterializer<>).MakeGenericType(types);
			else
				if(types.Length == 2)
					return typeof(MeasureStorageMaterializer<,>).MakeGenericType(types);
				else
					if(types.Length == 3)
						return typeof(MeasureStorageMaterializer<,,>).MakeGenericType(types);
					else
						if(types.Length == 4)
							return typeof(MeasureStorageMaterializer<,,,>).MakeGenericType(types);
						else
							if(types.Length == 5)
								return typeof(MeasureStorageMaterializer<,,,,>).MakeGenericType(types);
							else
								return typeof(ArrayMeasureMaterializer);
		}
		protected readonly Dictionary<IQueryMetadataColumn, int> indexes;
		protected readonly int start;
		protected MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start) {
			this.indexes = indexes;
			this.start = start;
		}
		protected MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes) {
			this.indexes = indexes;
		}
		public abstract MeasuresStorage MaterializeMethod(object[] data);
		public abstract MeasuresStorage MaterializeMethod(TypedBinaryReader reader);
	}
	class MeasureStorageMaterializer<TType> : MeasureStorageMaterializer {
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes, start) {
		}
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes)
			: base(indexes) {
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ServerMeasureStorage<TType>(data, indexes, start);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ServerMeasureStorage<TType>(indexes, reader);
		}
	}
	class MeasureStorageMaterializer<TType0, TType1> : MeasureStorageMaterializer {
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes, start) {
		}
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes)
			: base(indexes) {
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ServerMeasureStorage<TType0, TType1>(data, indexes, start);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ServerMeasureStorage<TType0, TType1>(indexes, reader);
		}
	}
	class MeasureStorageMaterializer<TType0, TType1, TType2> : MeasureStorageMaterializer {
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes, start) {
		}
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes)
			: base(indexes) {
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ServerMeasureStorage<TType0, TType1, TType2>(data, indexes, start);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ServerMeasureStorage<TType0, TType1, TType2>(indexes, reader);
		}
	}
	class MeasureStorageMaterializer<TType0, TType1, TType2, TType3> : MeasureStorageMaterializer {
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes, start) {
		}
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes)
			: base(indexes) {
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ServerMeasureStorage<TType0, TType1, TType2, TType3>(data, indexes, start);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ServerMeasureStorage<TType0, TType1, TType2, TType3>(indexes, reader);
		}
	}
	class MeasureStorageMaterializer<TType0, TType1, TType2, TType3, TType4> : MeasureStorageMaterializer {
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes, start) {
		}
		public MeasureStorageMaterializer(Dictionary<IQueryMetadataColumn, int> indexes)
			: base(indexes) {
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ServerMeasureStorage<TType0, TType1, TType2, TType3, TType4>(data, indexes, start);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ServerMeasureStorage<TType0, TType1, TType2, TType3, TType4>(indexes, reader);
		}
	}
	class ArrayMeasureMaterializer : MeasureStorageMaterializer {
		readonly int count;
		readonly Type[] types;
		public ArrayMeasureMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, int start, Type[] types)
			: base(indexes, start) {
			this.count = indexes.Count;
			this.types = types;
		}
		public ArrayMeasureMaterializer(Dictionary<IQueryMetadataColumn, int> indexes, Type[] types)
			: base(indexes) {
			this.count = indexes.Count;
			this.types = types;
		}
		public override MeasuresStorage MaterializeMethod(object[] data) {
			return new ArrayMeasureStorage(data, count, start, indexes, types);
		}
		public override MeasuresStorage MaterializeMethod(TypedBinaryReader reader) {
			return new ArrayMeasureStorage(indexes, reader, types);
		}
	}
	class ServerMeasureStorage<TType0> : ServerMeasureStorage {
		TType0 item0;
		public TType0 Item0 {
			get { return item0; }
			set { item0 = value; }
		}
		public ServerMeasureStorage(object[] data, Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes) {
			item0 = (TType0)data[start];
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader)
			: base(indexes) {
			Helper<ServerMeasureStorage<TType0>>.GetReadDelegate()(this, reader);
		}
		internal override void Write(TypedBinaryWriter writer) {
			Helper<ServerMeasureStorage<TType0>>.GetWriteDelegate()(this, writer);
		}
		protected internal override object Get(int dataIndex) {
			if(dataIndex == 0)
				return item0;
			return null;
		}
	}
	class ServerMeasureStorage<TType0, TType1> : ServerMeasureStorage {
		TType0 item0;
		TType1 item1;
		public TType0 Item0 {
			get { return item0; }
			set { item0 = value; }
		}
		public TType1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}
		public ServerMeasureStorage(object[] data, Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes) {
			item0 = (TType0)data[start];
			item1 = (TType1)data[start + 1];
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader)
			: base(indexes) {
			Helper<ServerMeasureStorage<TType0, TType1>>.GetReadDelegate()(this, reader);
		}
		internal override void Write(TypedBinaryWriter writer) {
			Helper<ServerMeasureStorage<TType0, TType1>>.GetWriteDelegate()(this, writer);
		}
		protected internal override object Get(int dataIndex) {
			if(dataIndex == 0)
				return item0;
			if(dataIndex == 1)
				return item1;
			return null;
		}
	}
	class ServerMeasureStorage<TType0, TType1, TType2> : ServerMeasureStorage {
		TType0 item0;
		TType1 item1;
		TType2 item2;
		public TType0 Item0 {
			get { return item0; }
			set { item0 = value; }
		}
		public TType1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}
		public TType2 Item2 {
			get { return item2; }
			set { item2 = value; }
		}
		public ServerMeasureStorage(object[] data, Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes) {
			item0 = (TType0)data[start];
			item1 = (TType1)data[start + 1];
			item2 = (TType2)data[start + 2];
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader)
			: base(indexes) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2>>.GetReadDelegate()(this, reader);
		}
		internal override void Write(TypedBinaryWriter writer) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2>>.GetWriteDelegate()(this, writer);
		}
		protected internal override object Get(int dataIndex) {
			if(dataIndex == 0)
				return item0;
			if(dataIndex == 1)
				return item1;
			if(dataIndex == 2)
				return item2;
			return null;
		}
	}
	class ServerMeasureStorage<TType0, TType1, TType2, TType3> : ServerMeasureStorage {
		TType0 item0;
		TType1 item1;
		TType2 item2;
		TType3 item3;
		public TType0 Item0 {
			get { return item0; }
			set { item0 = value; }
		}
		public TType1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}
		public TType2 Item2 {
			get { return item2; }
			set { item2 = value; }
		}
		public TType3 Item3 {
			get { return item3; }
			set { item3 = value; }
		}
		public ServerMeasureStorage(object[] data, Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes) {
			item0 = (TType0)data[start];
			item1 = (TType1)data[start + 1];
			item2 = (TType2)data[start + 2];
			item3 = (TType3)data[start + 3];
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader)
			: base(indexes) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2, TType3>>.GetReadDelegate()(this, reader);
		}
		internal override void Write(TypedBinaryWriter writer) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2, TType3>>.GetWriteDelegate()(this, writer);
		}
		protected internal override object Get(int dataIndex) {
			if(dataIndex == 0)
				return item0;
			if(dataIndex == 1)
				return item1;
			if(dataIndex == 2)
				return item2;
			if(dataIndex == 3)
				return item3;
			return null;
		}
	}
	class ServerMeasureStorage<TType0, TType1, TType2, TType3, TType4> : ServerMeasureStorage {
		TType0 item0;
		TType1 item1;
		TType2 item2;
		TType3 item3;
		TType4 item4;
		public TType0 Item0 {
			get { return item0; }
			set { item0 = value; }
		}
		public TType1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}
		public TType2 Item2 {
			get { return item2; }
			set { item2 = value; }
		}
		public TType3 Item3 {
			get { return item3; }
			set { item3 = value; }
		}
		public TType4 Item4 {
			get { return item4; }
			set { item4 = value; }
		}
		public ServerMeasureStorage(object[] data, Dictionary<IQueryMetadataColumn, int> indexes, int start)
			: base(indexes) {
			item0 = (TType0)data[start];
			item1 = (TType1)data[start + 1];
			item2 = (TType2)data[start + 2];
			item3 = (TType3)data[start + 3];
			item4 = (TType4)data[start + 4];
		}
		public ServerMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader)
			: base(indexes) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2, TType3, TType4>>.GetReadDelegate()(this, reader);
		}
		internal override void Write(TypedBinaryWriter writer) {
			Helper<ServerMeasureStorage<TType0, TType1, TType2, TType3, TType4>>.GetWriteDelegate()(this, writer);
		}
		protected internal override object Get(int dataIndex) {
			if(dataIndex == 0)
				return item0;
			if(dataIndex == 1)
				return item1;
			if(dataIndex == 2)
				return item2;
			if(dataIndex == 3)
				return item3;
			if(dataIndex == 4)
				return item4;
			return null;
		}
	}
	class ArrayMeasureStorage : ServerMeasureStorage {
		object[] data;
		Type[] types;
		public Type[] Types { get { return types; } }
		public ArrayMeasureStorage(object[] data, int count, int start, Dictionary<IQueryMetadataColumn, int> indexes, Type[] types)
			: base(indexes) {
			this.types = types;
			this.data = new object[count];
			Array.Copy(data, start, this.data, 0, count);
		}
		public ArrayMeasureStorage(Dictionary<IQueryMetadataColumn, int> indexes, TypedBinaryReader reader, Type[] types)
			: base(indexes) {
			this.types = types;
   			this.data = new object[Indexes.Count];
			for(int i = 0; i < Indexes.Count; i++)
				data[i] = reader.ReadObject(types[i]);
		}
		protected internal override object Get(int dataIndex) {
			return dataIndex >= data.Length ? null : data[dataIndex];
		}
		internal override void Write(TypedBinaryWriter writer) {
			for(int i = 0; i < data.Length; i++)
				writer.WriteObject(data[i]);
		}
	}
}
