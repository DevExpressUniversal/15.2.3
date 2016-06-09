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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheRecord
	public interface IPivotCacheRecord : ICloneable<IPivotCacheRecord>, IEnumerable<IPivotCacheRecordValue> {
		byte[] ToArray();
		IEnumerable<VariantValue> GetValuesEnumerable(PivotCacheFieldsCollection cacheFields, WorkbookDataContext dataContext);
		int Count { get; }
		IPivotCacheRecordValue this[int index] { get; set; }
	}
	public class PivotCacheRecord : IPivotCacheRecord {
		readonly IPivotCacheRecordValue[] innerArray;
		public PivotCacheRecord(int capacity) {
			innerArray = new IPivotCacheRecordValue[capacity];
		}
		public PivotCacheRecord(IList<IPivotCacheRecordValue> list) {
			innerArray = new IPivotCacheRecordValue[list.Count];
			list.CopyTo(innerArray, 0);
		}
		public PivotCacheRecord(IPivotCacheRecordValue[] innerArray) {
			this.innerArray = innerArray;
		}
		public int Count { get { return innerArray.Length; } }
		public IPivotCacheRecordValue this[int index] { get { return innerArray[index]; } set { innerArray[index] = value; } }
		public IEnumerable<VariantValue> GetValuesEnumerable(PivotCacheFieldsCollection cacheFields, WorkbookDataContext dataContext) {
			return new Enumerable<VariantValue>(GetValuesEnumerator(cacheFields, dataContext));
		}
		public IEnumerator<VariantValue> GetValuesEnumerator(PivotCacheFieldsCollection cacheFields, WorkbookDataContext dataContext) {
			for (int i = 0; i < Count; i++)
				yield return this[i].ToVariantValue(cacheFields[i], dataContext);
		}
		#region IEnumerable<IPivotCacheRecordValue> Members
		public IEnumerator<IPivotCacheRecordValue> GetEnumerator() {
			foreach (IPivotCacheRecordValue value in innerArray)
				yield return value;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<IPivotCacheRecordValue>)this).GetEnumerator();
		}
		#endregion
		#region ToArray
		public byte[] ToArray() {
			int count = Count;
			if (count <= 0)
				return null;
			int size = count; 
			foreach (IPivotCacheRecordValue value in this) {
				size += value.Size;
			}
			byte[] result = new byte[size];
			int position = 0;
			foreach (IPivotCacheRecordValue value in this) {
				PivotCacheRecordValueInfo info = PivotCacheRecordValueInfo.Create(value);
				int infoSize = info.WriteTo(result, position);
				position += infoSize;
				position += value.WriteTo(result, position);
			}
			return result;
		}
		#endregion
		#region FromArray
		public static PivotCacheRecord FromArray(byte[] array) {
			throw new ArgumentException("FromArray is currently disabled");
		}
		#endregion
		public IPivotCacheRecord Clone() {
			IPivotCacheRecordValue[] innerArrayClone = new IPivotCacheRecordValue[innerArray.Length];
			for (int i = 0; i < innerArray.Length; i++)
				innerArrayClone[i] = innerArray[i].Clone();
			return new PivotCacheRecord(innerArrayClone);
		}
	}
	#endregion
	#region PivotCacheRecordCollection
	public interface IPivotCacheRecordCollection : IEnumerable<IPivotCacheRecord> {
		IPivotCacheRecord this[int i] { get; set; }
		int Count { get; }
		int Add(IPivotCacheRecord record);
		void Clear();
	}
	public class PivotCacheRecordValueCollection : List<IPivotCacheRecordValue>, ICloneable<PivotCacheRecordValueCollection> {
		public PivotCacheRecordValueCollection()
			: base() {
		}
		public PivotCacheRecordValueCollection(int capacity)
			: base(capacity) {
		}
		public PivotCacheRecordValueCollection(IEnumerable<IPivotCacheRecordValue> collection)
			: base(collection) {
		}
		public PivotCacheRecordValueCollection Clone() {
			PivotCacheRecordValueCollection clone = new PivotCacheRecordValueCollection(this.Count);
			foreach (IPivotCacheRecordValue value in this)
				clone.Add(value.Clone());
			return clone;
		}
		public IEnumerator<VariantValue> GetValuesEnumerator(PivotCacheFieldsCollection cacheFields, WorkbookDataContext dataContext) {
			for (int i = 0; i < Count; i++)
				yield return this[i].ToVariantValue(cacheFields[i], dataContext);
		}
	}
	public class PivotCacheRecordCollection : IPivotCacheRecordCollection {
		readonly ChunkedList<IPivotCacheRecord> records;
		public PivotCacheRecordCollection() {
			this.records = new ChunkedList<IPivotCacheRecord>();
		}
		public int Count { get { return records.Count; } }
		public IPivotCacheRecord this[int index] { get { return records[index]; } set { records[index] = value; } }
		#region IEnumerable<IPivotCacheRecord> members
		public IEnumerator<IPivotCacheRecord> GetEnumerator() {
			return records.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<IPivotCacheRecord>)this).GetEnumerator();
		}
		#endregion
		public int Add(IPivotCacheRecord record) {
			int result = Count;
			records.Add(record);
			return result;
		}
		public void AddRange(IEnumerable<IPivotCacheRecord> enumerable) {
			records.AddRange(enumerable);
		}
		public void Clear() {
			records.Clear();
		}
		public void CopyFrom(PivotCacheRecordCollection source) {
			this.records.AddRange(source.records);
		}
	}
	#endregion
	#region PivotCacheRecordValueType (3 bits)
	public enum PivotCacheRecordValueType {
		SharedItemIndex = 0,
		Numeric = 1,
		String = 2,
		Boolean = 3,
		Error = 4,
		DateTime = 5,
		NoValue = 6,
	}
	#endregion
	#region IBinaryConvartableData
	public interface IBinaryConvartableData {
		int Size { get; }
		int WriteTo(byte[] array, int position);
		int InitFromBinary(byte[] array, int position);
	}
	#endregion
	#region PivotCacheRecordValueInfo
	public struct PivotCacheRecordValueInfo : IBinaryConvartableData {
		public static PivotCacheRecordValueInfo Create(IPivotCacheRecordValue value) {
			PivotCacheRecordValueInfo result = new PivotCacheRecordValueInfo();
			result.ValueType = value.ValueType;
			result.HasAdditionalProperties = value.HasAdditionalProperties;
			result.HasOlapProperties = value.HasOlapProperties;
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		public static PivotCacheRecordValueInfo Create(byte[] array, int position, out int size) {
			PivotCacheRecordValueInfo result = new PivotCacheRecordValueInfo();
			result.InitFromBinary(array, position);
			size = 1;
			return result;
		}
		#region Fields
		const byte MaskValueType = 0x07;				 
		const byte MaskHasAdditionalProperties = 0x08;   
		const byte MaskHasOlapProperties = 0x10;		 
		PivotCacheRecordValueType valueType;
		bool hasAdditionalProperties;
		bool hasOlapProperties;
		#endregion
		#region Properties
		public PivotCacheRecordValueType ValueType { get { return valueType; } set { valueType = value; } }
		public bool HasAdditionalProperties { get { return hasAdditionalProperties; } set { hasAdditionalProperties = value; } }
		public bool HasOlapProperties { get { return hasOlapProperties; } set { hasOlapProperties = value; } }
		#endregion
		public IPivotCacheRecordValue GetInstanceValue() {
			switch (ValueType) {
				case PivotCacheRecordValueType.Numeric:
					return new PivotCacheRecordNumericValue();
				case PivotCacheRecordValueType.Boolean:
					return new PivotCacheRecordBooleanValue();
				case PivotCacheRecordValueType.NoValue:
					return new PivotCacheRecordEmptyValue();
				case PivotCacheRecordValueType.Error:
					return new PivotCacheRecordErrorValue();
				case PivotCacheRecordValueType.SharedItemIndex:
					return new PivotCacheRecordSharedItemsIndexValue();
				case PivotCacheRecordValueType.DateTime:
					return new PivotCacheRecordDateTimeValue();
				case PivotCacheRecordValueType.String:
					return new PivotCacheRecordCharacterValue();
				default:
					throw new ArgumentException("Invalid PivotCacheRecordValueType: " + ValueType.ToString());
			}
		}
		byte GetPackedValues() {
			byte result = (byte)((byte)ValueType & MaskValueType);
			if (HasAdditionalProperties)
				result |= MaskHasAdditionalProperties;
			if (HasOlapProperties)
				result |= MaskHasOlapProperties;
			return result;
		}
		void SetProperties(byte packedValues) {
			valueType = (PivotCacheRecordValueType)(packedValues & MaskValueType);
			hasAdditionalProperties = GetBooleanValue(packedValues, MaskHasAdditionalProperties);
			hasOlapProperties = GetBooleanValue(packedValues, MaskHasOlapProperties);
		}
		bool GetBooleanValue(byte packedValues, byte mask) {
			return (packedValues & mask) != 0;
		}
		#region IBinaryConvartableData Members
		public int Size { get { return 1; } }
		public int WriteTo(byte[] array, int position) {
			array[position] = GetPackedValues();
			return 1;
		}
		public int InitFromBinary(byte[] array, int position) {
			SetProperties(array[position]);
			return 1;
		}
		#endregion
	}
	#endregion
	#region PivotCacheRecordValue
	public interface IPivotCacheRecordValue : IBinaryConvartableData, IComparable<IPivotCacheRecordValue>, ICloneable<IPivotCacheRecordValue> {
		PivotCacheRecordValueType ValueType { get; }
		bool HasAdditionalProperties { get; }
		bool HasOlapProperties { get; }
		VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context);
		PivotCacheRecordSharedItemsIndexValue ToSharedItem(IPivotCacheField ownerField);
		PivotCacheRecordSharedItemsIndexValue ToSharedItemThreadSafe(IPivotCacheField ownerField);
		void Visit(IPivotCacheRecordValueVisitor visitor);
		bool AreBaseDataEqual(IPivotCacheRecordValue y);
		int GetHashCodeForBaseData();
		bool IsCalculatedItem { get; }
		bool IsUnusedItem { get; set; }
		bool HasCaption { get; }
		string Caption { get; }
	}
	#region PivotCacheRecordSharedItemsIndexValue
	public class PivotCacheRecordSharedItemsIndexValue : IPivotCacheRecordValue {
		int indexValue;
		public PivotCacheRecordSharedItemsIndexValue() {
		}
		public PivotCacheRecordSharedItemsIndexValue(int indexValue) {
			this.indexValue = indexValue;
		}
		public int IndexValue { get { return indexValue; } set { indexValue = value; } }
		#region IPivotCacheRecordValue members
		public PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.SharedItemIndex; } }
		public bool HasAdditionalProperties { get { return false; } }
		public bool HasOlapProperties { get { return false; } }
		public int Size { get { return sizeof(int); } }
		public bool IsUnusedItem { get { throw new InvalidOperationException("SharedItemsIndexValue can't be marked as unused"); } set { throw new InvalidOperationException("SharedItemsIndexValue can't be marked as unused"); } }
		public int WriteTo(byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(indexValue);
			convertedValue.CopyTo(array, position);
			return sizeof(int);
		}
		public int InitFromBinary(byte[] array, int position) {
			indexValue = BitConverter.ToInt32(array, position);
			return sizeof(int);
		}
		public void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return ownerField.SharedItems[indexValue].ToVariantValue(ownerField, context);
		}
		public PivotCacheRecordSharedItemsIndexValue ToSharedItem(IPivotCacheField ownerField) {
			return this;
		}
		public PivotCacheRecordSharedItemsIndexValue ToSharedItemThreadSafe(IPivotCacheField ownerField) {
			return this;
		}
		#endregion
		public bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordSharedItemsIndexValue castedObj = obj as PivotCacheRecordSharedItemsIndexValue;
			if (castedObj == null)
				return false;
			return indexValue == castedObj.indexValue;
		}
		public int GetHashCodeForBaseData() {
			return indexValue.GetHashCode();
		}
		public int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordSharedItemsIndexValue otherCasted = other as PivotCacheRecordSharedItemsIndexValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return indexValue.CompareTo(otherCasted.IndexValue);
		}
		public IPivotCacheRecordValue Clone() {
			return new PivotCacheRecordSharedItemsIndexValue(indexValue);
		}
		#region IPivotCacheRecordValue Members
		public bool IsCalculatedItem { get { throw new InvalidOperationException(); } }
		public bool HasCaption { get { throw new InvalidOperationException(); } }
		public string Caption { get { throw new InvalidOperationException(); } }
		#endregion
	}
	#endregion
	#region PivotCacheRecordOrdinalValueBase
	public abstract class PivotCacheRecordOrdinalValueBase : IPivotCacheRecordValue {
		#region Fields
		bool isCalculatedItem;
		bool isUnusedItem;
		#endregion
		#region Properties
		public bool IsCalculatedItem { get { return isCalculatedItem; } set { isCalculatedItem = value; } }
		public bool IsUnusedItem { get { return isUnusedItem; } set { isUnusedItem = value; } }
		public virtual string Caption { get { return string.Empty; } set { } }
		public virtual bool HasCaption { get { return false; } }
		public virtual bool HasMemberPropertyIndexes { get { return false; } }
		#endregion
		#region IPivotCacheRecordValue Members
		public bool HasAdditionalProperties { get { return isCalculatedItem || isUnusedItem || HasCaption || HasMemberPropertyIndexes; } }
		public virtual bool HasOlapProperties { get { return false; } }
		public abstract PivotCacheRecordValueType ValueType { get; }
		public virtual int Size { get { throw new InvalidOperationException(); } }
		#endregion
		public virtual int WriteTo(byte[] array, int position) {
			throw new InvalidOperationException();
		}
		public virtual int InitFromBinary(byte[] array, int position) {
			throw new InvalidOperationException();
		}
		public PivotCacheRecordSharedItemsIndexValue ToSharedItemThreadSafe(IPivotCacheField ownerField) {
			int sharedItemIndex = ownerField.SharedItems.AddIfNotAlreadyAddedThreadSafe(this);
			return new PivotCacheRecordSharedItemsIndexValue(sharedItemIndex);
		}
		public PivotCacheRecordSharedItemsIndexValue ToSharedItem(IPivotCacheField ownerField) {
			int sharedItemIndex = ownerField.SharedItems.AddIfNotAlreadyAdded(this);
			return new PivotCacheRecordSharedItemsIndexValue(sharedItemIndex);
		}
		public abstract VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context);
		public abstract void Visit(IPivotCacheRecordValueVisitor visitor);
		public abstract bool AreBaseDataEqual(IPivotCacheRecordValue y);
		public abstract int GetHashCodeForBaseData();
		public abstract int CompareTo(IPivotCacheRecordValue other);
		public abstract IPivotCacheRecordValue Clone();
		protected virtual void CopyFromCore(PivotCacheRecordOrdinalValueBase value) {
			this.isCalculatedItem = value.isCalculatedItem;
			this.isUnusedItem = value.isUnusedItem;
		}
	}
	#endregion
	#region PivotCacheRecordValueBase
	public abstract class PivotCacheRecordValueBase : PivotCacheRecordOrdinalValueBase {
		#region Fields
		const byte MaskIsCalculatedItem = 0x01;		 
		const byte MaskIsUnusedItem = 0x02;			 
		const byte MaskHasCaption = 0x04;			   
		const byte MaskHasMemberPropertyIndexes = 0x08; 
		IList<int> memberPropertyIndexes;
		string caption;
		#endregion
		#region Properties
		public IList<int> MemberPropertyIndexes { get { return memberPropertyIndexes; } set { memberPropertyIndexes = value; } }
		public override string Caption { get { return caption; } set { caption = value; } }
		public override bool HasCaption { get { return caption != null; } }
		public override bool HasMemberPropertyIndexes { get { return memberPropertyIndexes != null; } }
		protected int AdditionalPropertiesSize {
			get {
				int result = 1;
				if (HasMemberPropertyIndexes)
					result += sizeof(int) + sizeof(int) * memberPropertyIndexes.Count;
				if (HasCaption)
					result += GetStringSize(caption);
				return result;
			}
		}
		#endregion
		#region IPivotCacheRecordValue Members
		protected abstract int SizeCore { get; }
		public override int Size { get { return SizeCore + AdditionalPropertiesSize; } }
		#endregion
		protected int GetStringSize(string value) {
			return sizeof(ushort) + Encoding.Unicode.GetByteCount(value);
		}
		byte GetAdditionalPackedValues() {
			byte result = 0;
			if (IsCalculatedItem)
				result += MaskIsCalculatedItem;
			if (IsUnusedItem)
				result += MaskIsUnusedItem;
			if (HasCaption)
				result += MaskHasCaption;
			if (HasMemberPropertyIndexes)
				result += MaskHasMemberPropertyIndexes;
			return result;
		}
		void SetBooleanAdditionalProperties(byte packedValues) {
			IsCalculatedItem = GetBooleanValue(packedValues, MaskIsCalculatedItem);
			IsUnusedItem = GetBooleanValue(packedValues, MaskIsUnusedItem);
		}
		protected bool GetBooleanValue(byte packedValues, byte mask) {
			return (packedValues & mask) != 0;
		}
		public override int WriteTo(byte[] array, int position) {
			int coreSize = WriteToCore(array, position);
			position += coreSize;
			return coreSize + WriteAdditionalPropertiesTo(array, position);
		}
		protected int WriteAdditionalPropertiesTo(byte[] array, int position) {
			int oldPosition = position;
			array[position] = GetAdditionalPackedValues();
			position++;
			if (HasMemberPropertyIndexes) {
				int count = memberPropertyIndexes.Count;
				position += WriteIntTo(count, array, position);
				for (int i = 0; i < count; i++)
					position += WriteIntTo(memberPropertyIndexes[i], array, position);
			}
			if (HasCaption)
				position += WriteStringTo(caption, array, position);
			return position - oldPosition;
		}
		protected int WriteIntTo(int value, byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(value);
			convertedValue.CopyTo(array, position);
			return convertedValue.Length;
		}
		protected int WriteStringTo(string value, byte[] array, int position) {
			int bytesCount = Encoding.Unicode.GetBytes(value, 0, value.Length, array, position + 2);
			byte[] lengthBytes = BitConverter.GetBytes((ushort)bytesCount);
			Array.Copy(lengthBytes, 0, array, position, lengthBytes.Length);
			return sizeof(ushort) + bytesCount;
		}
		public override int InitFromBinary(byte[] array, int position) {
			int coreSize = InitFromBinaryCore(array, position);
			position += coreSize;
			return coreSize + InitAdditionalPropertiesFromBinary(array, position);
		}
		int InitAdditionalPropertiesFromBinary(byte[] array, int position) {
			int oldPosition = position;
			byte packedValues = array[position];
			SetBooleanAdditionalProperties(packedValues);
			bool hasMemberPropertyIndexes = GetBooleanValue(packedValues, MaskHasMemberPropertyIndexes);
			bool hasCaption = GetBooleanValue(packedValues, MaskHasCaption);
			position++;
			if (hasMemberPropertyIndexes) {
				int count = ReadInt(array, position);
				memberPropertyIndexes = new List<int>();
				position += sizeof(int);
				for (int i = 0; i < count; i++) {
					int index = ReadInt(array, position);
					position += sizeof(int);
					memberPropertyIndexes.Add(index);
				}
			}
			if (hasCaption) {
				int captionSize;
				caption = ReadString(array, position, out captionSize);
				position += captionSize;
			}
			return position - oldPosition;
		}
		protected int ReadInt(byte[] array, int position) {
			return BitConverter.ToInt32(array, position);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		protected string ReadString(byte[] array, int position, out int size) {
			int length = BitConverter.ToUInt16(array, position);
			size = length + 2;
			return Encoding.Unicode.GetString(array, position + 2, length);
		}
		protected abstract int WriteToCore(byte[] array, int position);
		protected abstract int InitFromBinaryCore(byte[] array, int position);
		protected override void CopyFromCore(PivotCacheRecordOrdinalValueBase value) {
			base.CopyFromCore(value);
			PivotCacheRecordValueBase valueBase = value as PivotCacheRecordValueBase;
			if (valueBase != null) {
				if (valueBase.HasMemberPropertyIndexes) {
					var newList = new List<int>(valueBase.memberPropertyIndexes.Count);
					newList.AddRange(valueBase.memberPropertyIndexes);
					this.memberPropertyIndexes = newList;
				}
				else
					this.memberPropertyIndexes = null;
				this.caption = valueBase.caption;
			}
		}
	}
	#endregion
	#region PivotCacheRecordDateTimeValue
	public class PivotCacheRecordDateTimeValue : PivotCacheRecordValueBase {
		DateTime value;
		public PivotCacheRecordDateTimeValue(DateTime value) {
			this.value = value;
		}
		public PivotCacheRecordDateTimeValue() {
		}
		#region  Properties
		public DateTime Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.DateTime; } }
		protected override int SizeCore { get { return sizeof(long); } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(value.ToBinary());
			convertedValue.CopyTo(array, position);
			return convertedValue.Length;
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			value = DateTime.FromBinary(BitConverter.ToInt64(array, position));
			return sizeof(long);
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			VariantValue result = new VariantValue();
			result.SetDateTime(value, context);
			return result;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordDateTimeValue castedObj = obj as PivotCacheRecordDateTimeValue;
			if (castedObj == null)
				return false;
			return value.Equals(castedObj.value);
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordDateTimeValue otherCasted = other as PivotCacheRecordDateTimeValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordDateTimeValue clone = new PivotCacheRecordDateTimeValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordDateTimeValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordBooleanValue
	public class PivotCacheRecordBooleanValue : PivotCacheRecordValueBase {
		bool value;
		public PivotCacheRecordBooleanValue(bool value) {
			this.value = value;
		}
		public PivotCacheRecordBooleanValue() {
		}
		#region Properties
		internal bool Value { get { return value; } set { this.value = value; } }
		protected override int SizeCore { get { return 1; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Boolean; } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			array[position] = value ? (byte)1 : (byte)0;
			return 1;
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			value = array[position] > 0;
			return 1;
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordBooleanValue castedObj = obj as PivotCacheRecordBooleanValue;
			if (castedObj == null)
				return false;
			return value == castedObj.value;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordBooleanValue otherCasted = other as PivotCacheRecordBooleanValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordBooleanValue clone = new PivotCacheRecordBooleanValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordBooleanValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordFormattedValue
	public abstract class PivotCacheRecordFormattedValue : PivotCacheRecordValueBase {
		#region Fields
		const byte MaskBold = 0x01;				 
		const byte MaskItalic = 0x02;			   
		const byte MaskUnderline = 0x04;			
		const byte MaskStrikethrough = 0x08;		
		const byte MaskHasBackgroundColor = 0x10;   
		const byte MaskHasForegroundColor = 0x20;   
		const byte MaskHasFormatIndex = 0x40;	   
		const byte MaskHasTuples = 0x80;			
		bool bold;
		bool italic;
		bool underline;
		bool strikethrough;
		Color backgroundColor = DXColor.Empty;
		Color foregroundColor = DXColor.Empty;
		int? formatIndex;
		IList<PivotTupleCollection> tuples = new List<PivotTupleCollection>();
		#endregion
		#region Properties
		public Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
		public bool Bold { get { return bold; } set { bold = value; } }
		public Color ForegroundColor { get { return foregroundColor; } set { foregroundColor = value; } }
		public int? FormatIndex { get { return formatIndex; } set { formatIndex = value; } }
		public bool Italic { get { return italic; } set { italic = value; } }
		public bool Strikethrough { get { return strikethrough; } set { strikethrough = value; } }
		public bool Underline { get { return underline; } set { underline = value; } }
		public IList<PivotTupleCollection> Tuples { get { return tuples; } set { tuples = value; } }
		public bool HasTuples { get { return tuples.Count > 0; } }
		public override bool HasOlapProperties {
			get {
				return
					formatIndex.HasValue || !backgroundColor.IsEmpty || !foregroundColor.IsEmpty ||
					Bold || Italic || Strikethrough || Underline || HasTuples;
			}
		}
		public override int Size { get { return base.Size + OlapPropertiesSize; } }
		int OlapPropertiesSize {
			get {
				int result = 1;
				if (!backgroundColor.IsEmpty)
					result += 4;
				if (!foregroundColor.IsEmpty)
					result += 4;
				if (formatIndex.HasValue)
					result += sizeof(int);
				result += TuplesSize;
				return result;
			}
		}
		int TuplesSize {
			get {
				if (!HasTuples)
					return 0;
				int result = sizeof(int);
				int count = tuples.Count;
				for (int i = 0; i < count; i++)
					result += tuples[i].Size;
				return result;
			}
		}
		#endregion
		byte GetBooleanPackedValues() {
			byte result = 0;
			if (bold)
				result += MaskBold;
			if (italic)
				result += MaskItalic;
			if (underline)
				result += MaskUnderline;
			if (strikethrough)
				result += MaskStrikethrough;
			if (!backgroundColor.IsEmpty)
				result += MaskHasBackgroundColor;
			if (!foregroundColor.IsEmpty)
				result += MaskHasForegroundColor;
			if (formatIndex.HasValue)
				result += MaskHasFormatIndex;
			if (HasTuples)
				result += MaskHasTuples;
			return result;
		}
		void SetBooleanOlapProperties(byte packedValues) {
			bold = GetBooleanValue(packedValues, MaskBold);
			italic = GetBooleanValue(packedValues, MaskItalic);
			underline = GetBooleanValue(packedValues, MaskUnderline);
			strikethrough = GetBooleanValue(packedValues, MaskStrikethrough);
		}
		public override int WriteTo(byte[] array, int position) {
			int baseSize = base.WriteTo(array, position);
			position += baseSize;
			return baseSize + WriteOlapPropertiesTo(array, position);
		}
		int WriteOlapPropertiesTo(byte[] array, int position) {
			int oldPosition = position;
			array[position] = GetBooleanPackedValues();
			position++;
			if (!backgroundColor.IsEmpty)
				position += WriteColorTo(backgroundColor, array, position);
			if (!foregroundColor.IsEmpty)
				position += WriteColorTo(foregroundColor, array, position);
			if (formatIndex.HasValue)
				position += WriteIntTo(formatIndex.Value, array, position);
			position += WriteTuplesTo(array, position);
			return position - oldPosition;
		}
		int WriteColorTo(Color color, byte[] array, int position) {
			array[position] = color.A;
			array[position + 1] = color.R;
			array[position + 2] = color.G;
			array[position + 3] = color.B;
			return 4;
		}
		int WriteTuplesTo(byte[] array, int position) {
			if (!HasTuples)
				return 0;
			int oldPosition = position;
			int count = tuples.Count;
			position += WriteIntTo(count, array, position);
			for (int i = 0; i < count; i++)
				position += tuples[i].WriteTo(array, position);
			return position - oldPosition;
		}
		Color ReadColor(byte[] array, int position) {
			return Color.FromArgb(array[position], array[position + 1], array[position + 2], array[position + 3]);
		}
		public override int InitFromBinary(byte[] array, int position) {
			int baseSize = base.InitFromBinary(array, position);
			position += baseSize;
			return baseSize + InitOlapPropertiesFromBinary(array, position);
		}
		int InitOlapPropertiesFromBinary(byte[] array, int position) {
			int oldPosition = position;
			byte packedValues = array[position];
			SetBooleanOlapProperties(packedValues);
			bool hasBackgroundColor = GetBooleanValue(packedValues, MaskHasBackgroundColor);
			bool hasForegroundColor = GetBooleanValue(packedValues, MaskHasForegroundColor);
			bool hasFormatIndex = GetBooleanValue(packedValues, MaskHasFormatIndex);
			bool hasTuples = GetBooleanValue(packedValues, MaskHasTuples);
			position++;
			if (hasBackgroundColor) {
				backgroundColor = ReadColor(array, position);
				position += 4;
			}
			if (hasForegroundColor) {
				foregroundColor = ReadColor(array, position);
				position += 4;
			}
			if (hasFormatIndex) {
				formatIndex = ReadInt(array, position);
				position += sizeof(int);
			}
			if (hasTuples)
				position += InitTuplesFromBinary(array, position);
			return position - oldPosition;
		}
		int InitTuplesFromBinary(byte[] array, int position) {
			int oldPosition = position;
			int count = BitConverter.ToInt32(array, position);
			position += sizeof(int);
			for (int i = 0; i < count; i++) {
				PivotTupleCollection tupleCollection = new PivotTupleCollection();
				position += tupleCollection.InitFromBinary(array, position);
				tuples.Add(tupleCollection);
			}
			return position - oldPosition;
		}
		protected override void CopyFromCore(PivotCacheRecordOrdinalValueBase value) {
			PivotCacheRecordFormattedValue castedValue = (PivotCacheRecordFormattedValue)value;
			base.CopyFromCore(value);
			this.bold = castedValue.bold;
			this.italic = castedValue.italic;
			this.underline = castedValue.underline;
			this.strikethrough = castedValue.strikethrough;
			this.backgroundColor = castedValue.backgroundColor;
			this.foregroundColor = castedValue.foregroundColor;
			this.formatIndex = castedValue.formatIndex;
			this.tuples = new List<PivotTupleCollection>(castedValue.tuples.Count);
			foreach (PivotTupleCollection tuple in castedValue.tuples)
				this.tuples.Add(tuple.Clone());
		}
	}
	#endregion
	#region PivotCacheRecordEmptyValue
	public class PivotCacheRecordEmptyValue : PivotCacheRecordFormattedValue {
		#region Properties
		protected override int SizeCore { get { return 0; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.NoValue; } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			return 0;
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			return 0;
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return VariantValue.Empty;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			return obj is PivotCacheRecordEmptyValue;
		}
		public override int GetHashCodeForBaseData() {
			return CombinedHashCode.Initial.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordEmptyValue otherCasted = other as PivotCacheRecordEmptyValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return 0;
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordEmptyValue clone = new PivotCacheRecordEmptyValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordEmptyValue value) {
			CopyFromCore(value);
		}
	}
	#endregion
	#region PivotCacheRecordNumericValue
	public class PivotCacheRecordNumericValue : PivotCacheRecordFormattedValue {
		double value;
		public PivotCacheRecordNumericValue(double value) {
			this.value = value;
		}
		public PivotCacheRecordNumericValue() {
		}
		#region Properties
		internal double Value { get { return value; } set { this.value = value; } }
		protected override int SizeCore { get { return sizeof(double); } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Numeric; } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(value);
			convertedValue.CopyTo(array, position);
			return convertedValue.Length;
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			value = BitConverter.ToDouble(array, position);
			return sizeof(double);
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordNumericValue castedObj = obj as PivotCacheRecordNumericValue;
			if (castedObj == null)
				return false;
			return value == castedObj.value;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordNumericValue otherCasted = other as PivotCacheRecordNumericValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordNumericValue clone = new PivotCacheRecordNumericValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordNumericValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordErrorValue
	public class PivotCacheRecordErrorValue : PivotCacheRecordFormattedValue {
		ICellError value;
		public PivotCacheRecordErrorValue(ICellError errorValue) {
			this.value = errorValue;
		}
		public PivotCacheRecordErrorValue() {
		}
		#region Properties
		public ICellError Value { get { return value; } set { this.value = value; } }
		protected override int SizeCore { get { return 1; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Error; } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			ModelCellErrorType errorType = Value.Type;
			array[position] = (byte)errorType;
			return 1;
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			value = CellErrorFactory.Errors[array[position]];
			return 1;
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value.Value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordErrorValue castedObj = obj as PivotCacheRecordErrorValue;
			if (castedObj == null)
				return false;
			return value.Type == castedObj.value.Type;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordErrorValue otherCasted = other as PivotCacheRecordErrorValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.Type.CompareTo(otherCasted.Value.Type);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordErrorValue clone = new PivotCacheRecordErrorValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordErrorValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordCharacterValue
	public class PivotCacheRecordCharacterValue : PivotCacheRecordFormattedValue {
		string value;
		public PivotCacheRecordCharacterValue(string value) {
			this.value = value;
		}
		public PivotCacheRecordCharacterValue() {
		}
		#region Properties
		public string Value { get { return value; } set { this.value = value; } }
		protected override int SizeCore { get { return GetStringSize(value); } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.String; } }
		#endregion
		protected override int WriteToCore(byte[] array, int position) {
			return WriteStringTo(value, array, position);
		}
		protected override int InitFromBinaryCore(byte[] array, int position) {
			int size;
			value = ReadString(array, position, out size);
			return size;
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordCharacterValue castedObj = obj as PivotCacheRecordCharacterValue;
			if (castedObj == null)
				return false;
			return StringExtensions.CompareInvariantCultureIgnoreCase(value, castedObj.value) == 0;
		}
		public override int GetHashCodeForBaseData() {
			return value.ToLowerInvariant().GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordCharacterValue otherCasted = other as PivotCacheRecordCharacterValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordCharacterValue clone = new PivotCacheRecordCharacterValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordCharacterValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#endregion
	#region IPivotCacheRecordValueVisitor
	public interface IPivotCacheRecordValueVisitor {
		void Visit(PivotCacheRecordSharedItemsIndexValue value);
		void Visit(PivotCacheRecordBooleanValue value);
		void Visit(PivotCacheRecordDateTimeValue value);
		void Visit(PivotCacheRecordEmptyValue value);
		void Visit(PivotCacheRecordNumericValue value);
		void Visit(PivotCacheRecordErrorValue value);
		void Visit(PivotCacheRecordCharacterValue value);
		void Visit(PivotCacheRecordOrdinalBooleanValue value);
		void Visit(PivotCacheRecordOrdinalDateTimeValue value);
		void Visit(PivotCacheRecordOrdinalEmptyValue value);
		void Visit(PivotCacheRecordOrdinalNumericValue value);
		void Visit(PivotCacheRecordOrdinalErrorValue value);
		void Visit(PivotCacheRecordOrdinalCharacterValue value);
	}
	#endregion
	#region PivotCacheRecordOrdinalDateTimeValue
	public class PivotCacheRecordOrdinalDateTimeValue : PivotCacheRecordOrdinalValueBase {
		DateTime value;
		public PivotCacheRecordOrdinalDateTimeValue(DateTime value) {
			this.value = value;
		}
		public PivotCacheRecordOrdinalDateTimeValue() {
		}
		#region  Properties
		public DateTime Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.DateTime; } }
		#endregion
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			VariantValue result = new VariantValue();
			result.SetDateTime(value, context);
			return result;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordOrdinalDateTimeValue castedObj = obj as PivotCacheRecordOrdinalDateTimeValue;
			if (castedObj == null)
				return false;
			return value.Equals(castedObj.value);
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalDateTimeValue otherCasted = other as PivotCacheRecordOrdinalDateTimeValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalDateTimeValue clone = new PivotCacheRecordOrdinalDateTimeValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalDateTimeValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordOrdinalBooleanValue
	public class PivotCacheRecordOrdinalBooleanValue : PivotCacheRecordOrdinalValueBase {
		bool value;
		public PivotCacheRecordOrdinalBooleanValue(bool value) {
			this.value = value;
		}
		public PivotCacheRecordOrdinalBooleanValue() {
		}
		#region Properties
		internal bool Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Boolean; } }
		#endregion
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordOrdinalBooleanValue castedObj = obj as PivotCacheRecordOrdinalBooleanValue;
			if (castedObj == null)
				return false;
			return value == castedObj.value;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalBooleanValue otherCasted = other as PivotCacheRecordOrdinalBooleanValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalBooleanValue clone = new PivotCacheRecordOrdinalBooleanValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalBooleanValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordOrdinalEmptyValue
	public class PivotCacheRecordOrdinalEmptyValue : PivotCacheRecordOrdinalValueBase {
		public static PivotCacheRecordOrdinalEmptyValue Instance = new PivotCacheRecordOrdinalEmptyValue();
		#region Properties
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.NoValue; } }
		#endregion
		PivotCacheRecordOrdinalEmptyValue() {
		}
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return VariantValue.Empty;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			return obj is PivotCacheRecordOrdinalEmptyValue;
		}
		public override int GetHashCodeForBaseData() {
			return CombinedHashCode.Initial.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalEmptyValue otherCasted = other as PivotCacheRecordOrdinalEmptyValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return 0;
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalEmptyValue clone = new PivotCacheRecordOrdinalEmptyValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalEmptyValue value) {
			CopyFromCore(value);
		}
	}
	#endregion
	#region PivotCacheRecordOrdinalNumericValue
	public class PivotCacheRecordOrdinalNumericValue : PivotCacheRecordOrdinalValueBase {
		double value;
		public PivotCacheRecordOrdinalNumericValue(double value) {
			this.value = value;
		}
		public PivotCacheRecordOrdinalNumericValue() {
		}
		#region Properties
		internal double Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Numeric; } }
		#endregion
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordOrdinalNumericValue castedObj = obj as PivotCacheRecordOrdinalNumericValue;
			if (castedObj == null)
				return false;
			return value == castedObj.value;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalNumericValue otherCasted = other as PivotCacheRecordOrdinalNumericValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalNumericValue clone = new PivotCacheRecordOrdinalNumericValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalNumericValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordOrdinalErrorValue
	public class PivotCacheRecordOrdinalErrorValue : PivotCacheRecordOrdinalValueBase {
		ICellError value;
		public PivotCacheRecordOrdinalErrorValue(ICellError errorValue) {
			this.value = errorValue;
		}
		public PivotCacheRecordOrdinalErrorValue() {
		}
		#region Properties
		public ICellError Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.Error; } }
		#endregion
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value.Value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordOrdinalErrorValue castedObj = obj as PivotCacheRecordOrdinalErrorValue;
			if (castedObj == null)
				return false;
			return value.Type == castedObj.value.Type;
		}
		public override int GetHashCodeForBaseData() {
			return value.GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalErrorValue otherCasted = other as PivotCacheRecordOrdinalErrorValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.Type.CompareTo(otherCasted.Value.Type);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalErrorValue clone = new PivotCacheRecordOrdinalErrorValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalErrorValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
	#region PivotCacheRecordOrdinalCharacterValue
	public class PivotCacheRecordOrdinalCharacterValue : PivotCacheRecordOrdinalValueBase {
		string value;
		public PivotCacheRecordOrdinalCharacterValue(string value) {
			this.value = value;
		}
		public PivotCacheRecordOrdinalCharacterValue() {
		}
		#region Properties
		public string Value { get { return value; } set { this.value = value; } }
		public override PivotCacheRecordValueType ValueType { get { return PivotCacheRecordValueType.String; } }
		#endregion
		public override void Visit(IPivotCacheRecordValueVisitor visitor) {
			visitor.Visit(this);
		}
		public override VariantValue ToVariantValue(IPivotCacheField ownerField, WorkbookDataContext context) {
			return value;
		}
		public override bool AreBaseDataEqual(IPivotCacheRecordValue obj) {
			PivotCacheRecordOrdinalCharacterValue castedObj = obj as PivotCacheRecordOrdinalCharacterValue;
			if (castedObj == null)
				return false;
			return StringExtensions.CompareInvariantCultureIgnoreCase(value, castedObj.value) == 0;
		}
		public override int GetHashCodeForBaseData() {
			return value.ToLowerInvariant().GetHashCode();
		}
		public override int CompareTo(IPivotCacheRecordValue other) {
			PivotCacheRecordOrdinalCharacterValue otherCasted = other as PivotCacheRecordOrdinalCharacterValue;
			if (otherCasted == null)
				return ValueType.CompareTo(other.ValueType);
			return value.CompareTo(otherCasted.value);
		}
		public override IPivotCacheRecordValue Clone() {
			PivotCacheRecordOrdinalCharacterValue clone = new PivotCacheRecordOrdinalCharacterValue();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(PivotCacheRecordOrdinalCharacterValue value) {
			CopyFromCore(value);
			this.value = value.Value;
		}
	}
	#endregion
}
