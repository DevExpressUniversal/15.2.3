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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public enum ClientValueType {
		Unknown = 0,
		Number,
		String,
		Date,
		Boolean,
		RegExp,
		List,
		Dictionary = 7
	}
	public struct KnownTypeCode {
		private readonly int code;
		public KnownTypeCode(int typeCode) {
			this.code = typeCode;
		}
		public KnownTypeCode(int index, ClientValueType clientValueType)
			: this(index, clientValueType, false) {
		}
		public KnownTypeCode(int index, ClientValueType clientValueType, bool isNullable) {
			this.code = (index << 4) + ((int)clientValueType << 1);
			if(isNullable)
				this.code += 1;
			AssertCodeIsValid(this.code);
		}
		public ClientValueType ClientValueType {
			get { return GetClientValueType(code); }
		}
		public int Index {
			get { return code >> 4; }
		}
		public bool IsNullable {
			get { return (code & 1) == 1; }
		}
		public int Value {
			get { return code; }
		}
		public override bool Equals(object obj) {
			if(obj == null || !(obj is KnownTypeCode))
				return false;
			KnownTypeCode knownTypeCode = (KnownTypeCode)obj;
			return knownTypeCode.code == this.code;
		}
		public static ClientValueType GetClientValueType(int typeCode) {
			return (ClientValueType)((typeCode >> 1) & 7);
		}
		public override int GetHashCode() {
			return this.code.GetHashCode();
		}
		public override string ToString() {
			return string.Format(IsNullable ? "[{0}]{1} {2}?" : "[{0}]{1} {2}", Index, Value, ClientValueType);
		}
		[Conditional("DEBUG")]
		private static void AssertCodeIsValid(int code) {
			if(code <= 0 || code >= KnownTypesRepository.MinUnknownTypeCode)
				throw new ArgumentOutOfRangeException("code");
		}
	}
	public class ArrayProxy : IList {
		private Type itemType;
		private ArrayList items;
		public ArrayProxy(Type itemType) {
			this.itemType = itemType;
		}
		private bool IsEmpty {
			get { return items == null || items.Count == 0; }
		}
		private ArrayList Items {
			get {
				if(items == null)
					items = new ArrayList();
				return items;
			}
		}
		private Type ItemType {
			get { return itemType; }
		}
		public int Add(object item) {
			return Items.Add(item);
		}
		public IList ExtractArray() {
			if(IsEmpty)
				return Array.CreateInstance(ItemType, 0);
			else
				return Items.ToArray(ItemType);
		}
		#region IList Members
		void IList.Clear() { throw new NotSupportedException(); }
		bool IList.Contains(object value) { throw new NotSupportedException(); }
		int IList.IndexOf(object value) { throw new NotSupportedException(); }
		void IList.Insert(int index, object value) { throw new NotSupportedException(); }
		bool IList.IsFixedSize { get { throw new NotSupportedException(); } }
		bool IList.IsReadOnly { get { throw new NotSupportedException(); } }
		void IList.Remove(object value) { throw new NotSupportedException(); }
		void IList.RemoveAt(int index) { throw new NotSupportedException(); }
		object IList.this[int index] { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
		void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException(); }
		int ICollection.Count { get { throw new NotSupportedException(); } }
		bool ICollection.IsSynchronized { get { throw new NotSupportedException(); } }
		object ICollection.SyncRoot { get { throw new NotSupportedException(); } }
		IEnumerator IEnumerable.GetEnumerator() { throw new NotSupportedException(); }
		#endregion
	}
	public static class KnownTypesRepository {
		public const int MinUnknownTypeCode = 1024;
		private const bool IsNullable = true;
		private static Dictionary<Type, KnownTypeCode> KnownTypeCodes = new Dictionary<Type, KnownTypeCode>();
		private static Dictionary<KnownTypeCode, Type> KnownTypes = new Dictionary<KnownTypeCode, Type>();
		private static Dictionary<ClientValueType, Type> RecognizableOnlyByClientTypeCodeTypes = new Dictionary<ClientValueType, Type>();
		private static List<Type> TypeInfoNonRequiredTypes = new List<Type>();
		static KnownTypesRepository() {
			int i = 0;
			KnownTypeCodes.Add(typeof(String), new KnownTypeCode(i++, ClientValueType.String));
			KnownTypeCodes.Add(typeof(Int32), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Double), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Boolean), new KnownTypeCode(i++, ClientValueType.Boolean));
			KnownTypeCodes.Add(typeof(Byte), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Single), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Decimal), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Char), new KnownTypeCode(i++, ClientValueType.String));
			KnownTypeCodes.Add(typeof(DateTime), new KnownTypeCode(i++, ClientValueType.Date));
			KnownTypeCodes.Add(typeof(Int64), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Regex), new KnownTypeCode(i++, ClientValueType.RegExp));
			KnownTypeCodes.Add(typeof(Int16), new KnownTypeCode(i++, ClientValueType.Number));
			KnownTypeCodes.Add(typeof(Guid), new KnownTypeCode(i++, ClientValueType.String));
			KnownTypeCodes.Add(typeof(DBNull), new KnownTypeCode(i++, ClientValueType.String));
			KnownTypeCodes.Add(typeof(Nullable<Int32>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Double>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Boolean>), new KnownTypeCode(i++, ClientValueType.Boolean, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Byte>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Single>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Decimal>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Char>), new KnownTypeCode(i++, ClientValueType.String, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<DateTime>), new KnownTypeCode(i++, ClientValueType.Date, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Int64>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Int16>), new KnownTypeCode(i++, ClientValueType.Number, IsNullable));
			KnownTypeCodes.Add(typeof(Nullable<Guid>), new KnownTypeCode(i++, ClientValueType.String, IsNullable));
			KnownTypeCodes.Add(typeof(ArrayList), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Object[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(String[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Int32[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Double[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Byte[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Boolean[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Decimal[]), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<String>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<Int32>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<Double>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<Byte>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<Boolean>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(List<Decimal>), new KnownTypeCode(i++, ClientValueType.List));
			KnownTypeCodes.Add(typeof(Hashtable), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(HybridDictionary), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Object>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, String>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Int32>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Double>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Byte>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Single>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Boolean>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Int64>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, DateTime>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(Dictionary<String, Regex>), new KnownTypeCode(i++, ClientValueType.Dictionary));
			KnownTypeCodes.Add(typeof(DateTime[]), new KnownTypeCode(i++, ClientValueType.List));
			AssertKnownTypeListCapacityIsValid();
			foreach(KeyValuePair<Type, KnownTypeCode> pair in KnownTypeCodes)
				KnownTypes.Add(pair.Value, pair.Key);
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.Unknown, null);
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.String, typeof(String));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.Number, typeof(Double));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.Boolean, typeof(Boolean));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.Date, typeof(DateTime));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.Dictionary, typeof(Dictionary<string, object>));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.List, typeof(object[]));
			RecognizableOnlyByClientTypeCodeTypes.Add(ClientValueType.RegExp, typeof(Regex));
			foreach(Type type in RecognizableOnlyByClientTypeCodeTypes.Values) {
				if(type != null)
					TypeInfoNonRequiredTypes.Add(type);
			}
		}
		public static IDictionary CreateKnownDictionaryTypeInstance(int typeCode) {
			Type type = GetKnownType(typeCode);
			if(type == typeof(Hashtable))
				return new Hashtable();
			if(type == typeof(HybridDictionary))
				return new HybridDictionary();
			if(type == typeof(Dictionary<String, Object>))
				return new Dictionary<String, Object>();
			if(type == typeof(Dictionary<String, String>))
				return new Dictionary<String, String>();
			if(type == typeof(Dictionary<String, Int32>))
				return new Dictionary<String, Int32>();
			if(type == typeof(Dictionary<String, Double>))
				return new Dictionary<String, Double>();
			if(type == typeof(Dictionary<String, Byte>))
				return new Dictionary<String, Byte>();
			if(type == typeof(Dictionary<String, Single>))
				return new Dictionary<String, Single>();
			if(type == typeof(Dictionary<String, Boolean>))
				return new Dictionary<String, Boolean>();
			if(type == typeof(Dictionary<String, Int64>))
				return new Dictionary<String, Int64>();
			if(type == typeof(Dictionary<String, DateTime>))
				return new Dictionary<String, DateTime>();
			if(type == typeof(Dictionary<String, Regex>))
				return new Dictionary<String, Regex>();
			ExceptionHelper.ThrowUnableToInstantiateDictionary(typeCode);
			return null;
		}
		public static IList CreateKnownListTypeInstance(int typeCode) {
			Type type = GetKnownType(typeCode);
			if(type == typeof(ArrayList))
				return new ArrayList();
			if(type == typeof(Object[]))
				return new ArrayProxy(typeof(Object));
			if(type == typeof(String[]))
				return new ArrayProxy(typeof(String));
			if(type == typeof(Int32[]))
				return new ArrayProxy(typeof(Int32));
			if(type == typeof(Double[]))
				return new ArrayProxy(typeof(Double));
			if(type == typeof(Byte[]))
				return new ArrayProxy(typeof(Byte));
			if(type == typeof(Boolean[]))
				return new ArrayProxy(typeof(Boolean));
			if(type == typeof(Decimal[]))
				return new ArrayProxy(typeof(Decimal));
			if (type == typeof(DateTime[]))
				return new ArrayProxy(typeof(DateTime));
			if(type == typeof(List<String>))
				return new List<String>();
			if(type == typeof(List<Int32>))
				return new List<Int32>();
			if(type == typeof(List<Double>))
				return new List<Double>();
			if(type == typeof(List<Byte>))
				return new List<Byte>();
			if(type == typeof(List<Boolean>))
				return new List<Boolean>();
			if(type == typeof(List<Decimal>))
				return new List<Decimal>();
			ExceptionHelper.ThrowUnableToInstantiateList(typeCode);
			return null;
		}
		public static bool IsValuesOfSameType(IEnumerable enumerable, out object lastItem) {
			lastItem = null;
			if(enumerable == null)
				return false;
			IEnumerator enumerator = enumerable.GetEnumerator();
			if(!enumerator.MoveNext()) 
				return false;
			object previous = enumerator.Current;
			foreach(object obj in enumerable) {
				if(obj == null && previous == null) {
					previous = obj;
					continue;
				}
				if(obj == null || previous == null || obj.GetType() != previous.GetType())
					return false;
				previous = obj;
			}
			lastItem = previous;
			return true;
		}
		public static Type GetKnownType(int typeCode) {
			if(!IsTypeRecognizableOnlyByClientTypeCode(typeCode))
				return KnownTypes[new KnownTypeCode(typeCode)];
			else {
				return RecognizableOnlyByClientTypeCodeTypes[KnownTypeCode.GetClientValueType(typeCode)];
			}
		}
		public static bool IsListTypeCode(int typeCode) {
			return KnownTypeCode.GetClientValueType(typeCode) == ClientValueType.List;
		}
		public static bool IsDictionaryTypeCode(int typeCode) {
			return KnownTypeCode.GetClientValueType(typeCode) == ClientValueType.Dictionary;
		}
		public static bool IsKnownTypeCode(int typeCode) {
			return typeCode < MinUnknownTypeCode;
		}
		public static bool IsTypeRecognizableOnlyByClientTypeCode(int typeCode) {
			return (typeCode >> 4) == 0;
		}
		public static bool IsTypeInfoRequiredFor(Type type) {
			return !TypeInfoNonRequiredTypes.Contains(type);
		}
		public static bool TryGetKnownTypeCode(Type type, out int code) {
			KnownTypeCode knownTypeCode;
			if(KnownTypeCodes.TryGetValue(type, out knownTypeCode)) {
				code = knownTypeCode.Value;
				return true;
			} else {
				code = -1;
				return false;
			}
		}
		[Conditional("DEBUG")]
		private static void AssertKnownTypeListCapacityIsValid() {
			if(KnownTypeCodes.Count > (MinUnknownTypeCode >> 4) && KnownTypes.Count == KnownTypeCodes.Count)
				ExceptionHelper.ThrowKnownTypesCollectionHasInvalidSize();
		}
	}
}
