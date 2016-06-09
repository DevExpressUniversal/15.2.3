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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils.Serializing {
	#region TypedBinaryWriterEx
	public class TypedBinaryWriterEx : IDisposable {
		static bool IsMscorlib(Assembly assembly) {
#if SL
			return assembly.FullName.Split(',').First() == "mscorlib";
#else
			return assembly.GetName().Name == "mscorlib";
#endif
		}
		BinaryWriter output;
		ICustomObjectConverter customObjectConverter;
		public TypedBinaryWriterEx(BinaryWriter output) {
			if(output == null)
				throw new ArgumentNullException("output");
			this.output = output;
		}
		protected internal ICustomObjectConverter CustomObjectConverter {
			get { return customObjectConverter; }
			set { customObjectConverter = value; }
		}
		protected bool HasCustomObjectConverter { get { return CustomObjectConverter != null; } }
		protected internal BinaryWriter Output { get { return output; } }
		protected internal virtual void Dispose(bool disposing) {
			if(disposing)
				output.Dispose();
		}
		void IDisposable.Dispose() {
			Dispose(true);
		}
		~TypedBinaryWriterEx() {
			Dispose(false);
		}
		public virtual void Flush() {
			output.Flush();
		}
		public virtual void Close() {
			Dispose(true);
		}
		public virtual void WriteObject(object obj) {
			DXTypeCode typeCode = GetTypeCode(obj);
			WriteTypedObject(typeCode, obj);
		}
		protected internal virtual void WriteTypedObject(DXTypeCode typeCode, object obj) {
			switch(typeCode) {
				case DXTypeCode.Null:
					WriteNull();
					break;
				case DXTypeCode.DBNull:
					WriteDBNull();
					break;
				case DXTypeCode.Boolean:
					WriteBoolean((bool)obj);
					break;
				case DXTypeCode.Char:
					WriteChar((char)obj);
					break;
				case DXTypeCode.SByte:
					WriteSByte((sbyte)obj);
					break;
				case DXTypeCode.Byte:
					WriteByte((byte)obj);
					break;
				case DXTypeCode.Int16:
					WriteInt16((Int16)obj);
					break;
				case DXTypeCode.UInt16:
					WriteUInt16((UInt16)obj);
					break;
				case DXTypeCode.Int32:
					WriteInt32((Int32)obj);
					break;
				case DXTypeCode.UInt32:
					WriteUInt32((UInt32)obj);
					break;
				case DXTypeCode.Int64:
					WriteInt64((Int64)obj);
					break;
				case DXTypeCode.UInt64:
					WriteUInt64((UInt64)obj);
					break;
				case DXTypeCode.Single:
					WriteSingle((Single)obj);
					break;
				case DXTypeCode.Double:
					WriteDouble((Double)obj);
					break;
				case DXTypeCode.Decimal:
					WriteDecimal((Decimal)obj);
					break;
				case DXTypeCode.DateTime:
					WriteDateTime((DateTime)obj);
					break;
				case DXTypeCode.TimeSpan:
					WriteTimeSpan((TimeSpan)obj);
					break;
				case DXTypeCode.String:
					WriteString((string)obj);
					break;
				case DXTypeCode.Guid:
					WriteGuid((Guid)obj);
					break;
				case DXTypeCode.ByteArray:
					WriteByteArray((byte[])obj);
					break;
				case DXTypeCode.Enum:
					WriteEnum(obj);
					break;
				case DXTypeCode.Object:
				default:
					WriteObjectCore(obj);
					break;
			}
		}
		protected internal virtual void WriteNull() {
			output.Write((byte)DXTypeCode.Null);
		}
		protected internal virtual void WriteDBNull() {
			output.Write((byte)DXTypeCode.DBNull);
		}
		protected internal virtual void WriteBoolean(bool val) {
			if(val)
				output.Write(CreateTypeCodeByte(DXTypeCode.Boolean, (DXTypeCode)1));
			else
				output.Write(CreateTypeCodeByte(DXTypeCode.Boolean, (DXTypeCode)0));
		}
		protected internal virtual void WriteChar(char val) {
			output.Write((byte)DXTypeCode.Char);
			output.Write(val);
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteSByte(sbyte val) {
			output.Write(CreateTypeCodeByte(DXTypeCode.SByte, DXTypeCode.SByte));
			output.Write(val);
		}
		protected internal virtual void WriteByte(byte val) {
			output.Write(CreateTypeCodeByte(DXTypeCode.Byte, DXTypeCode.Byte));
			output.Write(val);
		}
		protected internal virtual void WriteInt16(Int16 val) {
			WriteInt16Core(val, DXTypeCode.Int16);
		}
		protected internal virtual void WriteInt16Core(Int16 val, DXTypeCode sourceTypeCode) {
			if(val >= 0) {
				if(val <= Byte.MaxValue) {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Byte));
					output.Write((byte)val);
				} else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int16));
					output.Write(val);
				}
			} else {
				if(val >= SByte.MinValue) {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.SByte));
					output.Write((SByte)val);
				} else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int16));
					output.Write(val);
				}
			}
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt16(UInt16 val) {
			WriteUInt16Core(val, DXTypeCode.UInt16);
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt16Core(UInt16 val, DXTypeCode sourceTypeCode) {
			if(val <= Byte.MaxValue) {
				output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Byte));
				output.Write((byte)val);
			} else {
				output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.UInt16));
				output.Write(val);
			}
		}
		protected internal virtual void WriteInt32(Int32 val) {
			WriteInt32Core(val, DXTypeCode.Int32);
		}
		protected internal virtual void WriteInt32Core(Int32 val, DXTypeCode sourceTypeCode) {
			if(val >= 0) {
				if(val <= UInt16.MaxValue)
					WriteUInt16Core((UInt16)val, sourceTypeCode);
				else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int32));
					output.Write(val);
				}
			} else {
				if(val >= Int16.MinValue)
					WriteInt16Core((Int16)val, sourceTypeCode);
				else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int32));
					output.Write(val);
				}
			}
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt32(UInt32 val) {
			WriteUInt32Core(val, DXTypeCode.UInt32);
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt32Core(UInt32 val, DXTypeCode sourceTypeCode) {
			if(val <= UInt16.MaxValue) {
				WriteUInt16Core((UInt16)val, sourceTypeCode);
			} else {
				output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.UInt32));
				output.Write(val);
			}
		}
		protected internal virtual void WriteInt64(Int64 val) {
			WriteInt64Core(val, DXTypeCode.Int64);
		}
		protected internal virtual void WriteInt64Core(Int64 val, DXTypeCode sourceTypeCode) {
			if(val >= 0) {
				if(val <= UInt32.MaxValue)
					WriteUInt32Core((UInt32)val, sourceTypeCode);
				else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int64));
					output.Write(val);
				}
			} else {
				if(val >= Int32.MinValue)
					WriteInt32Core((Int32)val, sourceTypeCode);
				else {
					output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.Int64));
					output.Write(val);
				}
			}
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt64(UInt64 val) {
			WriteUInt64Core(val, DXTypeCode.UInt64);
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteUInt64Core(UInt64 val, DXTypeCode sourceTypeCode) {
			if(val <= UInt32.MaxValue) {
				WriteUInt32Core((UInt32)val, sourceTypeCode);
			} else {
				output.Write(CreateTypeCodeByte(sourceTypeCode, DXTypeCode.UInt64));
				output.Write(val);
			}
		}
		protected internal virtual void WriteSingle(Single val) {
			output.Write((byte)DXTypeCode.Single);
			output.Write(val);
		}
		protected internal virtual void WriteDouble(Double val) {
			output.Write((byte)DXTypeCode.Double);
			output.Write(val);
		}
		protected internal virtual void WriteDecimal(Decimal val) {
			if(Decimal.Floor(val) != val) {
				output.Write(CreateTypeCodeByte(DXTypeCode.Decimal, DXTypeCode.Decimal));
#if SL
				output.Write(Convert.ToDouble(val));
#else
				output.Write(val);
#endif
			} else {
				if(val >= 0 && val <= UInt64.MaxValue)
					WriteUInt64Core(Decimal.ToUInt64(val), DXTypeCode.Decimal);
				else if(val < 0 && val >= Int64.MinValue)
					WriteInt64Core(Decimal.ToInt64(val), DXTypeCode.Decimal);
				else {
					output.Write(CreateTypeCodeByte(DXTypeCode.Decimal, DXTypeCode.Decimal));
#if SL
					output.Write(Convert.ToDouble(val));
#else
					output.Write(val);
#endif
				}
			}
		}
		protected internal virtual void WriteDateTime(DateTime val) {
			WriteInt64Core(val.Ticks, DXTypeCode.DateTime);
		}
		protected internal virtual void WriteTimeSpan(TimeSpan val) {
			WriteInt64Core(val.Ticks, DXTypeCode.TimeSpan);
		}
		protected internal virtual void WriteString(string val) {
			output.Write((byte)DXTypeCode.String);
			output.Write(val);
		}
		protected internal virtual void WriteGuid(Guid val) {
			output.Write((byte)DXTypeCode.Guid);
			output.Write(val.ToByteArray());
		}
		protected internal virtual void WriteByteArray(byte[] val) {
			output.Write((byte)DXTypeCode.ByteArray);
			WriteInt32(val.Length);
			output.Write(val);
		}
		protected internal virtual void WriteTypeName(Type type) {
			if(type.GetAssembly() == GetType().GetAssembly() || IsMscorlib(type.GetAssembly()))
				WriteString(type.FullName);
			else
				WriteString(type.AssemblyQualifiedName);
		}
		protected internal virtual void WriteEnum(object val) {
			output.Write((byte)DXTypeCode.Enum);
			WriteTypeName(val.GetType());
			WriteString(val.ToString());
		}
		protected internal virtual void WriteObjectCore(object val) {
			string serializedObject = TypeDescriptor.GetConverter(val.GetType()).ConvertToInvariantString(val);
			WriteObjectCore(val.GetType(), serializedObject);
		}
		void WriteObjectCore(Type type, string serializedObject) {
			WriteObjectCore(type, serializedObject, false);
		}
		protected internal virtual void WriteObjectCore(Type type, string serializedObject, bool forceWriteTypeFullName) {
			output.Write((byte)DXTypeCode.Object);
			if(forceWriteTypeFullName)
				WriteString(type.FullName);
			else
				WriteTypeName(type);
			output.Write(serializedObject);
		}
		protected internal virtual byte CreateTypeCodeByte(DXTypeCode sourceTypeCode, DXTypeCode destinationTypeCode) {
			if(sourceTypeCode == destinationTypeCode)
				return (byte)(((int)sourceTypeCode) << 4);
			else
				return (byte)((((int)sourceTypeCode) << 4) | (int)destinationTypeCode);
		}
		Dictionary<TypeCode, DXTypeCode> typeCodeTable = CreateTypeCodeTable();
		static Dictionary<TypeCode, DXTypeCode> CreateTypeCodeTable() {
			Dictionary<TypeCode, DXTypeCode> result = new Dictionary<TypeCode, DXTypeCode>();
			result[TypeCode.Empty] = DXTypeCode.Null;
			result[TypeCode.Object] = DXTypeCode.Object;
			result[DXTypeExtensions.TypeCodeDBNull] = DXTypeCode.DBNull;
			result[TypeCode.Boolean] = DXTypeCode.Boolean;
			result[TypeCode.Char] = DXTypeCode.Char;
			result[TypeCode.SByte] = DXTypeCode.SByte;
			result[TypeCode.Byte] = DXTypeCode.Byte;
			result[TypeCode.Int16] = DXTypeCode.Int16;
			result[TypeCode.UInt16] = DXTypeCode.UInt16;
			result[TypeCode.Int32] = DXTypeCode.Int32;
			result[TypeCode.UInt32] = DXTypeCode.UInt32;
			result[TypeCode.Int64] = DXTypeCode.Int64;
			result[TypeCode.UInt64] = DXTypeCode.UInt64;
			result[TypeCode.Single] = DXTypeCode.Single;
			result[TypeCode.Double] = DXTypeCode.Double;
			result[TypeCode.Decimal] = DXTypeCode.Decimal;
			result[TypeCode.DateTime] = DXTypeCode.DateTime;
			result[TypeCode.String] = DXTypeCode.String;
			return result;
		}
		protected internal virtual DXTypeCode GetTypeCode(object obj) {
			if(obj == null)
				return DXTypeCode.Null;
			Type type = obj.GetType();
			if(typeof(Enum).IsAssignableFrom(type))
				return DXTypeCode.Enum;
			DXTypeCode result = typeCodeTable[DXTypeExtensions.GetTypeCode(type)];
			if(result == DXTypeCode.Object) {
				if(type == typeof(Guid))
					result = DXTypeCode.Guid;
				else if(type == typeof(TimeSpan))
					result = DXTypeCode.TimeSpan;
				else if(type == typeof(byte[]))
					result = DXTypeCode.ByteArray;
			}
			return result;
		}
	}
	#endregion
}
