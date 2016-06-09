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
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
#else
using System.Data;
#endif
namespace DevExpress.Data.IO {
	internal class StreamValueType {
		public const byte Null = 0;
		public const byte DBNull = 1;
		public const byte NotNull = 2;
	}
	internal class StreamNumericType {
		public const byte Decimal = StreamValueType.NotNull + 1;
		public const byte Int64 = Decimal + 1;
		public const byte Int32 = Int64 + 1;
		public const byte Int16 = Int32 + 1;
		public const byte Byte = Int16 + 1;
	}	
	internal enum StringType { Null = 0, Empty = 1, NonEmpty = 2 };
	internal static class DecimalBinaryWriterHelper {
		public const decimal MaxInt64Value = Int64.MaxValue;
		public const decimal MinInt64Value = Int64.MinValue;
		public static bool CanConvertToInt64(decimal value) {
			if(value > MaxInt64Value || value < MinInt64Value)
				return false;
			return Decimal.GetBits(Math.Abs(value))[3] == 0;
		}
	}
	public class TypedBinaryReader : BinaryReader {
		public static TypedBinaryReader CreateReader(Stream input, Encoding encoding, ICustomObjectConverter customConverter) {
			if(customConverter != null)
				return new TypedBinaryReaderEx2(input, encoding, customConverter);
			return new TypedBinaryReader(input, encoding);
		}
		public static TypedBinaryReader CreateReader(Stream input, ICustomObjectConverter customConverter) {
			if(customConverter != null)
				return new TypedBinaryReaderEx2(input, customConverter);
			return new TypedBinaryReader(input);
		}
		public TypedBinaryReader(Stream input) : base(input) {}
		public TypedBinaryReader(Stream input, Encoding encoding) : base(input, encoding) {}
		public object ReadTypedObject() {
			Type type = ReadType();
			if(type == null) return null;
			if(type == typeof(DBNull)) return DBNull.Value;
			return ReadObject(type);
		}
		public T ReadObject<T>() {
			return (T)ReadObject(typeof(T));
		}
		public virtual object ReadObject(Type type) {
			byte emptyValue = ReadByte();
			if(emptyValue == StreamValueType.Null) return null;
			if(emptyValue == StreamValueType.DBNull) return DBNull.Value;
			Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
			if(nullableUnderlyingType != null) type = nullableUnderlyingType;
			if(type == typeof(string)) 
				return ReadString();
			if(type == typeof(long))
				return ReadInt64Optimized(emptyValue);
			if(type == typeof(int))
				return ReadInt32Optimized(emptyValue);
			if(type == typeof(short))
				return ReadInt16Optimized(emptyValue);
			if(type == typeof(decimal))
				return ReadDecimalOptimized(emptyValue);
			if(type == typeof(double)) 
				return ReadDouble();
			if(type == typeof(char)) 
				return ReadChar();
			if(type == typeof(byte))
				return ReadByte();
			if (type == typeof(bool))
				return ReadBoolean();
			if(type == typeof(DateTime))
				return ReadDateTime();
			if(type == typeof(Guid))
				return ReadGuid();
			if(type.IsEnum())
				return ReadEnum(type, emptyValue);
#if !SL && !DXPORTABLE
			if(TypedBinaryWriter.IsRequireSerializableWriter(type)) {
				return ReadSerializableObject();
			}
#endif
			return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(ReadString());
		}
		object ReadEnum(Type type, byte emptyValue) {
			long res = ReadInt64Optimized(emptyValue);
			return Enum.ToObject(type, res);
		}
#if !SL && !DXPORTABLE
		object ReadSerializableObject() {
			return DevExpress.Utils.Serializing.Helpers.ObjectConverter.BinaryFormatter.Deserialize(this.BaseStream);
		}
#endif
		public Type ReadType() {
			byte typeValue = ReadByte();
			if(typeValue == 0) {
				string fullName = ReadString();
				return Type.GetType(fullName);
			}
			else return SmartReadWriteConverter.GetTypeByByte(typeValue);
		}
		decimal ReadDecimalOptimized(byte decimalType) {
			if(decimalType == StreamNumericType.Decimal) {
				int[] bits = new int[4];
				for(int i = 0; i < 4; i++)
					bits[i] = ReadInt32();
				return new Decimal(bits);
			}
			return (decimal)ReadInt64Optimized(decimalType);
		}
		long ReadInt64Optimized(byte intType) {
			if (intType == StreamNumericType.Int64)
				return ReadInt64();
			return ReadInt32Optimized(intType);
		}
		int ReadInt32Optimized(byte intType) {
			if (intType == StreamNumericType.Int32)
				return ReadInt32();
			return ReadInt16Optimized(intType);
		}
		short ReadInt16Optimized(byte intType) {
			if (intType == StreamNumericType.Int16)
				return ReadInt16();
			return ReadByte();
		}
		DateTime ReadDateTime() {
			long date = (long)ReadObject(typeof(long));
			return new DateTime(date);
		}
		Guid ReadGuid() {
			byte[] bytes = new byte[16];
			Read(bytes, 0, 16);
			return new Guid(bytes);
		}
		public string ReadNullableString() {
			StringType type = (StringType)ReadByte();
			switch(type) {
				case StringType.Null:
					return null;
				case StringType.Empty:
					return string.Empty;
				case StringType.NonEmpty:
					return ReadString();
				default:
					throw new ArgumentOutOfRangeException("Invalid record");
			}
		}
	}
	public class TypedBinaryWriter : BinaryWriter {
		public static TypedBinaryWriter CreateWriter(Stream input, Encoding encoding, ICustomObjectConverter customConverter) {
			if(customConverter != null)
				return new TypedBinaryWriterEx2(input, encoding, customConverter);
			return new TypedBinaryWriter(input, encoding);
		}
		public static TypedBinaryWriter CreateWriter(Stream input, ICustomObjectConverter customConverter) {
			if(customConverter != null)
				return new TypedBinaryWriterEx2(input, customConverter);
			return new TypedBinaryWriter(input);
		}
		public TypedBinaryWriter(Stream input) : base(input) {}
		public TypedBinaryWriter(Stream input, Encoding encoding) : base(input, encoding) {}
		public void WriteTypedObject(object value) {
			WriteType(value == null ? null : value.GetType());
			if(value == null || value.GetType() == typeof(DBNull)) return;
			WriteObject(value);
		}
		public virtual void WriteObject(object value) {
			if(value == null) {
				Write(StreamValueType.Null);
				return;
			}
			if(value == DBNull.Value) {
				Write(StreamValueType.DBNull);
				return;
			}
			Type type = value.GetType();
			if(!IsSmartWrittingType(type))
				Write(StreamValueType.NotNull);
			if(type == typeof(string)) {
				Write((string)value);
				return;
			}
			if(type == typeof(long)) {
				WriteInt64((long)value);
				return;
			}
			if(type == typeof(int)) {
				WriteInt32((int)value);
				return;
			}
			if(type == typeof(short)) {
				WriteInt16((short)value);
				return;
			}
			if(type == typeof(decimal)) {
				WriteDecimal((decimal)value);
				return;
			}
			if(type == typeof(double)) {
				Write((double)value);
				return;
			}
			if(type == typeof(char)) {
				Write((char)value);
				return;
			}
			if(type == typeof(byte)) {
				Write((byte)value);
				return;
			}
			if(type == typeof(bool)) {
				Write((bool)value);
				return;
			}
			if(type == typeof(DateTime)) {
				WriteDateTime((DateTime)value);
				return;
			}
			if(type == typeof(Guid)) {
				WriteGuid((Guid)value);
				return;
			}
			if(type.IsEnum()) {
				WriteEnum(type, value);
				return;
			}
#if !SL && !DXPORTABLE
			if(IsRequireSerializableWriter(type)) {
				WriteSerializable(value);
				return;
			}
#endif
			Write(TypeDescriptor.GetConverter(type).ConvertToInvariantString(value));
		}
		public void WriteNullableString(string str) {
			StringType type = StringType.NonEmpty;
			if(str == null)
				type = StringType.Null;
			else if (str.Length == 0) 
				type = StringType.Empty;
			Write((byte)type);
			if(type == StringType.NonEmpty)
				Write(str);
		}
		void WriteEnum(Type type, object value) {
			WriteInt64(Convert.ToInt64(value));
		}
#if !SL && !DXPORTABLE
		internal static bool IsRequireSerializableWriter(Type type) {
			if(DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust) return false;
			return type.IsSerializable; 
		}
		void WriteSerializable(object value) {
			DevExpress.Utils.Serializing.Helpers.ObjectConverter.BinaryFormatter.Serialize(this.BaseStream, value);
		}
#endif
		public void WriteType(Type type) {
			byte typeValue = SmartReadWriteConverter.GetByteByType(type);
			Write(typeValue);
			if(typeValue == 0) {
				Write(type.AssemblyQualifiedName);
			}
		}
		public void WriteType(object value) {
			WriteType(value.GetType());
		}
		bool IsSmartWrittingType(Type type) {
			return type == typeof(decimal) || type == typeof(int) || type == typeof(long) || type == typeof(short) || type.IsEnum();
		}
		void WriteInt16(Int16 value) {
			if(value != Int16.MinValue && value >= 0 && Math.Abs(value) < Byte.MaxValue) {
				Write(StreamNumericType.Byte);
				Write(Convert.ToByte(value));
				return;
			}
			Write(StreamNumericType.Int16);
			Write(Convert.ToInt16(value));
		}
		void WriteInt32(int value) {
			if(value != int.MinValue && Math.Abs(value) < Int16.MaxValue) {
				WriteInt16(Convert.ToInt16(value));
				return;
			}
			Write(StreamNumericType.Int32);
			Write(value);
		}
		void WriteInt64(long value) {
			if(value != long.MinValue && Math.Abs(value) < Int32.MaxValue) {
				WriteInt32(Convert.ToInt32(value));
				return;
			}
			Write(StreamNumericType.Int64);
			Write(value);
		}
		void WriteDecimal(decimal value) {
			if(DecimalBinaryWriterHelper.CanConvertToInt64(value)) {
				WriteInt64(decimal.ToInt64(value));
			} else {
				Write(StreamNumericType.Decimal);
				foreach(int i in Decimal.GetBits(value))
					Write(i);
			}
		}
		void WriteGuid(Guid guid) {
			Write(guid.ToByteArray(), 0, 16);
		}
		void WriteDateTime(DateTime dateTime) {
			WriteObject(dateTime.Ticks);
		}
	}
	internal class SmartReadWriteConverter {
		public static Type GetTypeByByte(byte value) {
			switch(value) {
				case 1: return typeof(byte);
				case 2: return typeof(short);
				case 3: return typeof(int);
				case 4: return typeof(long);
				case 5: return typeof(decimal);
				case 6: return typeof(double);
				case 7: return typeof(string);
				case 8: return typeof(DateTime);
				case 9: return null;
				case 10: return typeof(bool);
				case 11: return typeof(Guid);
				case 12: return typeof(DBNull);
			}
			return null;
		}
		public static byte GetByteByType(Type type) {
			if(type == typeof(byte)) return 1;
			if(type == typeof(short)) return 2;
			if(type == typeof(int)) return 3;
			if(type == typeof(long)) return 4;
			if(type == typeof(decimal)) return 5;
			if(type == typeof(double)) return 6;
			if(type == typeof(string)) return 7;
			if(type == typeof(DateTime)) return 8;
			if(type == null) return 9;
			if(type == typeof(bool)) return 10;
			if(type == typeof(Guid)) return 11;
			if(type == typeof(DBNull)) return 12;
			return 0;
		}
	}
	class TypedBinaryReaderEx2 : TypedBinaryReader {
		ICustomObjectConverter customConverter;
		public TypedBinaryReaderEx2(Stream input, Encoding encoding, ICustomObjectConverter customConverter)
			: base(input, encoding) {
			this.customConverter = customConverter;
		}
		public TypedBinaryReaderEx2(Stream input, ICustomObjectConverter customConverter)
			: base(input) {
			this.customConverter = customConverter;
		}
		public override object ReadObject(Type type) {
			bool useCustomConverter = customConverter.CanConvert(type);
			object value = base.ReadObject(useCustomConverter ? typeof(string) : type);
			if(value != null && useCustomConverter)
				value = customConverter.FromString(type, value.ToString());
			return value;
		}
	}
	class TypedBinaryWriterEx2 : TypedBinaryWriter {
		ICustomObjectConverter customConverter;
		public TypedBinaryWriterEx2(Stream input, Encoding encoding, ICustomObjectConverter customConverter)
			: base(input, encoding) {
			this.customConverter = customConverter;
		}
		public TypedBinaryWriterEx2(Stream input, ICustomObjectConverter customConverter)
			: base(input) {
			this.customConverter = customConverter;
		}
		public override void WriteObject(object value) {
			if(value == null) {
				base.WriteObject(value);
			} else {
				Type type = value.GetType();
				if(customConverter.CanConvert(type))
					value = customConverter.ToString(type, value);
				base.WriteObject(value);
			}
		}
	}
}
