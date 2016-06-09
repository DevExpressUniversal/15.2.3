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
using System.ComponentModel;
using DevExpress.Data.Helpers;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Data.IO;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
#if !SL
using System.Data;
using DevExpress.Data.Access;
#else
using Comparer = System.Collections.Generic.Comparer<object>;
#endif
namespace DevExpress.Data.Storage {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028")]
	public enum ValueType : byte { Regular, Null, Error };
	public class DataStorageObjectComparer {
		public const string StreamSign = "DSOC", NullStreamSign = "DSNL",
			CorruptedDataMessage = "Corrupted data!";
		public static DataStorageObjectComparer CreateComparer(Type type) {
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null)
				type = underlyingType;
			if(type.Equals(typeof(Int32))) return new DataStorageIntComparer(type);
			if(type.Equals(typeof(Int16))) return new DataStorageInt16Comparer(type);
			if(type.Equals(typeof(string))) return new DataStorageStrComparer(type);
			if(type.Equals(typeof(DateTime))) return new DataStorageDateTimeComparer(type);
			if(type.Equals(typeof(double))) return new DataStorageDoubleComparer(type);
			if(type.Equals(typeof(float))) return new DataStorageFloatComparer(type);
			return new DataStorageObjectComparer(type);
		}
		protected object[] records = null;
		protected ValueType[] nullRecords = null;
		int[] groupIndexes = null;
		protected TypeCode typeCode = TypeCode.Empty;		
		bool isAscending = true;
		protected Type type;
		bool treatErrorAsNull = true;
		ICustomObjectConverter customConverter;
		public DataStorageObjectComparer(Type type) {
			this.type = type;
		}
		public object ErrorObject = UnboundErrorObject.Value;
		public bool TreatErrorAsNull {
			get { return treatErrorAsNull; }
			set { treatErrorAsNull = value; }
		}
		public void CreateStorage(VisibleListSourceRowCollection rows, BaseDataControllerHelper dataHelper, int column) {
			int recordCount = dataHelper.Count; 
			CreateRecords(recordCount);
			this.nullRecords = new ValueType[recordCount];
			typeCode = DXTypeExtensions.GetTypeCode(dataHelper.Columns[column].Type);
			for(int n = rows.VisibleRowCount - 1; n >= 0; n--) {
				int listSourceRow = rows.GetListSourceRow(n);
				object val = dataHelper.GetRowValue(listSourceRow, column, null);
				SetNullableRecordValue(listSourceRow, val);
			}
		}
		internal void CreateStorage(Type type, params object[] data) {
			int recordCount = data.Length;
			CreateRecords(recordCount);
			this.nullRecords = new ValueType[recordCount];
			typeCode = DXTypeExtensions.GetTypeCode(type);
			for(int n = data.Length - 1; n >= 0; n--) {
				object val = data[n];
				SetNullableRecordValue(n, val);
			}
		}
		void SetNullableRecordValue(int row, object val) {
			if(val == null || val == DBNull.Value) {
				this.nullRecords[row] = ValueType.Null;
			} else {
				if(val == ErrorObject)
					this.nullRecords[row] = ValueType.Error;
				else {
					this.nullRecords[row] = ValueType.Regular;
					SetRecordValueCore(row, val);
				}
			}
		}
		protected virtual void CreateRecords(int rowCount) {
			this.records = new object[rowCount];
		}
		public virtual void SetRecordValue(int row, object val) {
			if(val == null) {
				this.nullRecords[row] = ValueType.Null;
			} else {
				if(val == ErrorObject)
					this.nullRecords[row] = ValueType.Error;
				else {
					this.nullRecords[row] = ValueType.Regular;
					SetRecordValueCore(row, val);
				}
			}
		}
		protected virtual void SetRecordValueCore(int row, object val) {
			this.records[row] = val;
		}
		public virtual void ClearStorage() {
			this.records = null;
			this.nullRecords = null;
		}
		public bool IsStorageEmpty { get { return this.nullRecords == null; } }
		public int RecordCount { get { return this.records.Length; } }
		public bool IsError(int listSourceRow) {
			return this.nullRecords[listSourceRow] == ValueType.Error;
		}
		public bool IsNull(int listSourceRow) {
			ValueType type = this.nullRecords[listSourceRow];
			return type == ValueType.Null || (TreatErrorAsNull && type == ValueType.Error);
		}
		public object GetNullableRecordValue(int listSourceRow) {
			if(IsNull(listSourceRow)) return null;
			if(!TreatErrorAsNull && IsError(listSourceRow)) return ErrorObject;
			return GetRecordValue(listSourceRow);
		}
		public virtual object GetRecordValue(int listSourceRow) {
			return this.records[listSourceRow];
		}
		public virtual int CompareRecords(int listSourceRow1, int listSourceRow2) {
			if(nullRecords == null || listSourceRow1 == listSourceRow2) return 0;
			int? res = CheckNull(listSourceRow1, listSourceRow2);
			if(res.HasValue)
				return res.Value;
			res = CheckError(listSourceRow1, listSourceRow2);
			if(res.HasValue)
				return res.Value;
			return CompareRecordsCore(listSourceRow1, listSourceRow2);
		}
		public bool HasNullValue() {
			if(nullRecords == null) return false;
			for(int i = 0; i < nullRecords.Length; i ++) {
				if(IsNull(i))
					return true;
			}
			return false;
		}
		public virtual bool SupportComparerCache { get { return false; } }
		public bool HasComparerCache { get { return this.groupIndexes != null; } }
		public void SetComparerCache(int[] cache, bool isAscending) {
			if(!SupportComparerCache) return;
			this.groupIndexes = cache;
			this.isAscending = isAscending;
		}
		IComparer customComparer;
		public IComparer CustomComparer {
			get { return customComparer; }
		}
		internal void SetCustomComparer(IComparer customComparer) {
			this.customComparer = customComparer;
		}
		public ICustomObjectConverter CustomObjectConverter {
			get { return customConverter; }
			set { customConverter = value; }
		}
		TypedBinaryWriter CreateTypedBinaryWriter(Stream stream) {
			return TypedBinaryWriter.CreateWriter(stream, CustomObjectConverter);
		}
		TypedBinaryReader CreateTypedBinaryReader(Stream stream) {
			return TypedBinaryReader.CreateReader(stream, CustomObjectConverter);
		}
		protected virtual int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {
			if(CustomComparer != null)
				return CustomComparer.Compare(records[listSourceRow1], records[listSourceRow2]);
			return Comparer.Default.Compare(records[listSourceRow1], records[listSourceRow2]);
		}
		protected int? CheckNull(int listSourceRow1, int listSourceRow2) {
			bool isNull1 = IsNull(listSourceRow1), isNull2 = IsNull(listSourceRow2);
			if(isNull1 && isNull2)
				return 0;
			else if(isNull1)
				return -1;
			else if(isNull2)
				return 1;
			else
				return null;
		}
		protected int? CheckError(int listSourceRow1, int listSourceRow2) {
			if(TreatErrorAsNull)
				return null;
			bool isError1 = IsError(listSourceRow1), isError2 = IsError(listSourceRow2);
			if(isError1 && isError2)
				return 0;
			else if(isError1)
				return 1;
			else if(isError2)
				return -1;
			else
				return null;
		} 
		protected int? CheckGroupIndexes(int listSourceRow1, int listSourceRow2) {
			if(!HasComparerCache)
				return null;
			int group1 = this.groupIndexes[listSourceRow1];
			int group2 = this.groupIndexes[listSourceRow2];
			if(group1 == group2) return 0;
			int res = group1 < group2 ? -1 : 1;
			return isAscending ? res : -res;
		}
		protected bool SupportsPreSorted { get { return false; } }
#if !SL
		public virtual void SaveToStream(Stream stream) {
			TypedBinaryWriter writer = CreateTypedBinaryWriter(stream);
			writer.Write(StreamSign);
			writer.Write(2);	
			writer.WriteType(type);
			writer.Write((int)typeCode);
			writer.Write(nullRecords.Length);			
			if(nullRecords.Length > 0 && typeCode != DXTypeExtensions.TypeCodeDBNull && typeCode != TypeCode.Empty && !IsObject) {
				SaveNullRecords(writer);
				if(typeCode == TypeCode.String) SaveStringDataToStream(writer);						
				else SaveDataToStream(writer);
			}
		}
#endif
		bool IsObject {
			get { return (type == typeof(System.Object) && typeCode == TypeCode.Object); }
		}
#if !SL
		private void SaveNullRecords(TypedBinaryWriter writer) {
			if(!TreatErrorAsNull)
				throw new InvalidOperationException("Can't save error values");
			byte c = 0;
			for(int i = 0; i < nullRecords.Length; i++) {
				c |= (byte)((IsNull(i) ? 1 : 0) << (i % 8));
				if(i % 8 == 7) {
					writer.Write(c);
					c = 0;
				}
			}
			if(nullRecords.Length % 8 != 0) writer.Write(c);
		}
		private void SaveStringDataToStream(TypedBinaryWriter writer) {
			if(!TreatErrorAsNull)
				throw new InvalidOperationException("Can't save error values");
			Hashtable hash = new Hashtable();
			for(int i = 0; i < nullRecords.Length; i++) {				
				if(IsNull(i)) continue;
				string value = (string)GetRecordValue(i);				
				if(!hash.ContainsKey(value)) {
					hash.Add(value, hash.Count);
					writer.Write(-1);
					writer.Write(value);
				} else writer.Write((int)hash[value]);
			}
		}
		private void SaveDataToStream(TypedBinaryWriter writer) {
			if(!TreatErrorAsNull)
				throw new InvalidOperationException("Can't save error values");
			for(int i = 0; i < nullRecords.Length; i++) {
				if(IsNull(i)) continue;
				switch(typeCode) {
					case TypeCode.Boolean:
						writer.Write((bool)GetRecordValue(i));
						break;
					case TypeCode.Byte:
						writer.Write((byte)GetRecordValue(i));
						break;
					case TypeCode.Char:
						writer.Write((char)GetRecordValue(i));
						break;
					case TypeCode.DateTime:
						writer.Write(((DateTime)GetRecordValue(i)).ToBinary());
						break;
					case TypeCode.Decimal:
						writer.Write((decimal)GetRecordValue(i));
						break;
					case TypeCode.Double:
						writer.Write((double)GetRecordValue(i));
						break;
					case TypeCode.Int16:
						writer.Write((short)GetRecordValue(i));
						break;
					case TypeCode.Int32:
						writer.Write((int)GetRecordValue(i));
						break;
					case TypeCode.Int64:
						writer.Write((Int64)GetRecordValue(i));
						break;
					case TypeCode.SByte:
						writer.Write((SByte)GetRecordValue(i));
						break;
					case TypeCode.Single:
						writer.Write((float)GetRecordValue(i));
						break;
					case TypeCode.String:
						writer.Write((string)GetRecordValue(i));
						break;
					case TypeCode.UInt16:
						writer.Write((UInt16)GetRecordValue(i));
						break;
					case TypeCode.UInt32:
						writer.Write((UInt32)GetRecordValue(i));
						break;
					case TypeCode.UInt64:
						writer.Write((UInt64)GetRecordValue(i));
						break;
					case TypeCode.Object:
						writer.WriteTypedObject(Convert.ChangeType(GetRecordValue(i), type));
						break;
				}
			}
		}
		public virtual void LoadFromStream(Stream stream) {
			LoadFromStream(stream, true);
		}
		public virtual void LoadFromStream(Stream stream, bool checkStreamSign) {
			TypedBinaryReader reader = CreateTypedBinaryReader(stream);
			if(checkStreamSign) {
				string sign = reader.ReadString();
				if(sign != StreamSign) throw new Exception(CorruptedDataMessage);
			}
			int version = reader.ReadInt32();
			switch(version) {
				case 2:
					LoadFromStreamV2(reader);
					break;
				case 1:
					LoadFromStreamV1(reader);
					break;
			}
		}
		void LoadFromStreamV2(TypedBinaryReader reader) {
			type = reader.ReadType();
			typeCode = (TypeCode)reader.ReadInt32();
			LoadFromStreamV1Core(reader);
		}		
		void LoadFromStreamV1(TypedBinaryReader reader) {
			long startPosition = reader.BaseStream.Position;
			typeCode = (TypeCode)reader.ReadInt32();
			if(!TypeCodeHelper.IsValidTypeCode(typeCode)) {
				reader.BaseStream.Position = startPosition;
				type = reader.ReadType();
				typeCode = (TypeCode)reader.ReadInt32();
				if(type != null && TypeCodeHelper.IsValidTypeCode(typeCode)) {
					LoadFromStreamV1Core(reader);
					return;
				} else
					throw new Exception(CorruptedDataMessage);
			}
			type = DevExpress.Data.Filtering.Helpers.CriteriaTypeResolverBase.GetTypeFromCode(typeCode);
			LoadFromStreamV1Core(reader);
		}
		void LoadFromStreamV1Core(TypedBinaryReader reader) {
			int recordCount = reader.ReadInt32();
			CreateRecords(recordCount);
			this.nullRecords = new ValueType[recordCount];
			if(recordCount > 0 && typeCode != DXTypeExtensions.TypeCodeDBNull && typeCode != TypeCode.Empty && !IsObject) {
				LoadNullRecordsFromStream(reader);
				if(typeCode == TypeCode.String) LoadStringDataFromStream(reader);
				else LoadDataFromStream(reader, recordCount);
			}
		}
		void LoadNullRecordsFromStream(TypedBinaryReader reader) {
			byte c = 0;
			for(int i = 0; i < nullRecords.Length; i ++) {
				if(i % 8 == 0) c = reader.ReadByte();
				this.nullRecords[i] = (c & 1) == 1 ? ValueType.Null : ValueType.Regular;
				c >>= 1;
			}
		}
		void LoadStringDataFromStream(TypedBinaryReader reader) {
			List<string> vals = new List<string>(); 
			for(int i = 0; i < nullRecords.Length; i++) {
				if(IsNull(i)) continue;
				int hashKey = reader.ReadInt32();
				if(hashKey == -1) {
					string value = reader.ReadString();
					vals.Add(value);
					SetRecordValue(i, value);
				} else {
					SetRecordValue(i, vals[hashKey]);
				}
			}
		}
		void LoadDataFromStream(TypedBinaryReader reader, int recordCount) {
			for(int i = 0; i < recordCount; i++) {
				if(IsNull(i)) continue;			
				switch(typeCode) {
					case TypeCode.Boolean:
						SetRecordValue(i, reader.ReadBoolean());
						break;
					case TypeCode.Byte:
						SetRecordValue(i, reader.ReadByte());
						break;
					case TypeCode.Char:
						SetRecordValue(i, reader.ReadChar());
						break;
					case TypeCode.DateTime:
						SetRecordValue(i, DateTime.FromBinary(reader.ReadInt64()));
						break;
					case TypeCode.Decimal:
						SetRecordValue(i, reader.ReadDecimal());
						break;
					case TypeCode.Double:
						SetRecordValue(i, reader.ReadDouble());
						break;
					case TypeCode.Int16:
						SetRecordValue(i, reader.ReadInt16());
						break;
					case TypeCode.Int32:
						SetRecordValue(i, reader.ReadInt32());
						break;
					case TypeCode.Int64:
						SetRecordValue(i, reader.ReadInt64());
						break;
					case TypeCode.SByte:
						SetRecordValue(i, reader.ReadSByte());
						break;
					case TypeCode.Single:
						SetRecordValue(i, reader.ReadSingle());
						break;
					case TypeCode.String:
						SetRecordValue(i, reader.ReadString());
						break;
					case TypeCode.UInt16:
						SetRecordValue(i, reader.ReadUInt16());
						break;
					case TypeCode.UInt32:
						SetRecordValue(i, reader.ReadUInt32());
						break;
					case TypeCode.UInt64:
						SetRecordValue(i, reader.ReadUInt64());
						break;
					case TypeCode.Object:
						SetRecordValue(i, Convert.ChangeType(reader.ReadTypedObject(), type));
						break;
				}				
			}
		}
#endif
	}
	public class DataStorageInt16Comparer : DataStorageObjectComparer {
		Int16[] intRecords = null;
		public DataStorageInt16Comparer(Type type) : base(type) { }
		public override void ClearStorage() {
			base.ClearStorage();
			this.intRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.intRecords = new Int16[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.intRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {
			this.intRecords[row] = (Int16)val;
		}
		protected sealed override int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {
			int v1 = intRecords[listSourceRow1], v2 = intRecords[listSourceRow2];
			return v1 == v2 ? 0 : (v1 < v2 ? -1 : 1);
		}
	}
	public class DataStorageIntComparer : DataStorageObjectComparer { 
		int[] intRecords = null;
		public DataStorageIntComparer(Type type) : base(type) { }
		public override void ClearStorage() {
			base.ClearStorage();
			this.intRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.intRecords = new int[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.intRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {
			this.intRecords[row] = (int)val;
		}
		protected sealed override int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {			
			int v1 = intRecords[listSourceRow1], v2 = intRecords[listSourceRow2];
			return v1 == v2 ? 0 : (v1 < v2 ? -1 : 1);
		}
	}
	public class DataStorageDoubleComparer : DataStorageObjectComparer {
		double[] doubleRecords = null;
		public DataStorageDoubleComparer(Type type) : base(type) { }
		public override void ClearStorage() {
			base.ClearStorage();
			this.doubleRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.doubleRecords = new double[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.doubleRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {
			this.doubleRecords[row] = (double)val;
		}
		protected override int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {
			double v1 = doubleRecords[listSourceRow1], v2 = doubleRecords[listSourceRow2];
			return Comparer<double>.Default.Compare(v1, v2);
		}
	}
	public class DataStorageFloatComparer : DataStorageObjectComparer {
		float[] floatRecords = null;
		public DataStorageFloatComparer(Type type) : base(type) { }
		public override void ClearStorage() {
			base.ClearStorage();
			this.floatRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.floatRecords = new float[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.floatRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {
			this.floatRecords[row] = (float)val;
		}
		protected override int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {
			float v1 = floatRecords[listSourceRow1], v2 = floatRecords[listSourceRow2];
			return v1.CompareTo(v2);
		}
	}
	public class DataStorageStrComparer : DataStorageObjectComparer {
		string[] strRecords;
		readonly CompareInfo ci;
		CompareOptions compareOptions = CompareOptions.None;
		public CompareOptions CompareOptions {
			get { return compareOptions; }
			set { compareOptions = value; }
		}
		public DataStorageStrComparer(Type type)
							: base(type) {
			ci = CultureInfo.CurrentCulture.CompareInfo;
		}
		public override void ClearStorage() {
			base.ClearStorage();
			this.strRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.strRecords = new string[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.strRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {			
			this.strRecords[row] = (string)val.ToString();
		}
		public override bool SupportComparerCache { get { return true; } }
		public override int CompareRecords(int listSourceRow1, int listSourceRow2) {
			if(listSourceRow1 == listSourceRow2) return 0;
			int? res = CheckGroupIndexes(listSourceRow1, listSourceRow2);
			if(res.HasValue)
				return res.Value;
			res = CheckNull(listSourceRow1, listSourceRow2);
			if(res.HasValue)
				return res.Value;
			res = CheckError(listSourceRow1, listSourceRow2);
			if(res.HasValue)
				return res.Value;
			return ci.Compare(strRecords[listSourceRow1], strRecords[listSourceRow2], compareOptions);
		}
	}
	public class DataStorageDateTimeComparer : DataStorageObjectComparer {
		DateTime[] dateRecords = null;
		public DataStorageDateTimeComparer(Type type) : base(type) { }
		public override void ClearStorage() {
			base.ClearStorage();
			this.dateRecords = null;
		}
		protected override void CreateRecords(int rowCount) {
			this.dateRecords = new DateTime[rowCount];
		}
		public override object GetRecordValue(int listSourceRow) {
			return this.dateRecords[listSourceRow];
		}
		protected override void SetRecordValueCore(int row, object val) {
			this.dateRecords[row] = (DateTime)val;
		}
		protected override int CompareRecordsCore(int listSourceRow1, int listSourceRow2) {
			return DateTime.Compare(dateRecords[listSourceRow1], dateRecords[listSourceRow2]);
		}
	}
#if !SL
	public class TypeCodeHelper {
		static Array typeCodeValues;
		public static Array TypeCodeValues {
			get {
				if(typeCodeValues == null)
					typeCodeValues = Enum.GetValues(typeof(TypeCode));
				return typeCodeValues;
			}
		}
		public static bool IsValidTypeCode(TypeCode typeCode) {
			return Array.IndexOf(TypeCodeValues, typeCode) >= 0;
		}
	}
#endif
	public static class DataStoragesExtenders {
		public static DataStorageObjectComparer GetStorageComparer(this DataColumnInfo dataColumnInfo) {
			return ((DevExpress.Data.PivotGrid.LightDataColumnInfo)dataColumnInfo).StorageComparer;
		}
		public static void CreateColumnStorages(this DataColumnSortInfoCollection sic, VisibleListSourceRowCollection visibleListSourceRows, BaseDataControllerHelper helper) {
			for(int i = 0; i < sic.Count; i++) {
				sic[i].ColumnInfo.GetStorageComparer().CreateStorage(visibleListSourceRows, helper, sic[i].ColumnInfo.Index);
			}
		}
		public static void ClearColumnStorages(this DataColumnSortInfoCollection sic) {
			for(int i = 0; i < sic.Count; i++) {
				sic[i].ColumnInfo.GetStorageComparer().ClearStorage();
			}
		}
	}
}
