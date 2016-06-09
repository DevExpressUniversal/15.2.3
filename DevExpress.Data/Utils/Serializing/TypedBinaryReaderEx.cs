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
using System.ComponentModel;
using System.IO;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils.Serializing {
	#region TypedBinaryReaderEx
	public class TypedBinaryReaderEx : IDisposable {
		BinaryReader input;
		ICustomObjectConverter customObjectConverter;
		public TypedBinaryReaderEx(BinaryReader input) {
			if(input == null)
				throw new ArgumentNullException("input");
			this.input = input;
		}
		protected internal ICustomObjectConverter CustomObjectConverter {
			get { return customObjectConverter; }
			set { customObjectConverter = value; }
		}
		protected bool HasCustomObjectConverter { get { return CustomObjectConverter != null; } }
		protected internal BinaryReader Input { get { return input; } }
		protected internal virtual void Dispose(bool disposing) {
			if(disposing)
				input.Dispose();
		}
		void IDisposable.Dispose() {
			Dispose(true);
		}
		~TypedBinaryReaderEx() {
			Dispose(false);
		}
		public virtual void Close() {
			Dispose(true);
		}
		public virtual object ReadObject() {
			byte typeCode = input.ReadByte();
			return ReadObjectCore(typeCode);
		}
		protected internal virtual object ReadObjectCore(byte typeCode) {
			switch((DXTypeCode)typeCode) {
				case DXTypeCode.Null:
					return null;
				case DXTypeCode.DBNull:
					return DBNull.Value;
				case DXTypeCode.Single:
					return input.ReadSingle();
				case DXTypeCode.Double:
					return input.ReadDouble();
				case DXTypeCode.Char:
					return input.ReadChar();
				case DXTypeCode.String:
					return ReadString();
				case DXTypeCode.Guid:
					return ReadGuid();
				case DXTypeCode.ByteArray:
					return ReadByteArray();
				case DXTypeCode.Enum:
					return ReadEnum();
				default:
					return ReadObjectCoreDecodeTypeCode(typeCode);
			}
		}
		protected internal virtual object ReadObjectCoreDecodeTypeCode(byte typeCode) {
			DXTypeCode targetTypeCode = (DXTypeCode)((int)typeCode >> 4);
			DXTypeCode sourceTypeCode = (DXTypeCode)((int)typeCode & 0x0F);
			if(targetTypeCode == DXTypeCode.Boolean)
				return ReadBoolean(sourceTypeCode);
			if(sourceTypeCode == DXTypeCode.Null)
				sourceTypeCode = targetTypeCode;
			switch(targetTypeCode) {
				case DXTypeCode.SByte:
					return input.ReadSByte();
				case DXTypeCode.Byte:
					return input.ReadByte();
				case DXTypeCode.Int16:
					return Convert.ToInt16(ReadInteger(sourceTypeCode));
				case DXTypeCode.UInt16:
					return Convert.ToUInt16(ReadInteger(sourceTypeCode));
				case DXTypeCode.Int32:
					return Convert.ToInt32(ReadInteger(sourceTypeCode));
				case DXTypeCode.UInt32:
					return Convert.ToUInt32(ReadInteger(sourceTypeCode));
				case DXTypeCode.Int64:
					return Convert.ToInt64(ReadInteger(sourceTypeCode));
				case DXTypeCode.UInt64:
					return Convert.ToUInt64(ReadInteger(sourceTypeCode));
				case DXTypeCode.Decimal:
					return ReadDecimal(sourceTypeCode);
				case DXTypeCode.DateTime:
					return ReadDateTime(sourceTypeCode);
				case DXTypeCode.TimeSpan:
					return ReadTimeSpan(sourceTypeCode);
				case DXTypeCode.Object:
				default:
					return ReadObjectCore();
			}
		}
		protected internal virtual bool ReadBoolean(DXTypeCode typeCode) {
			return (int)typeCode != 0;
		}
		protected internal virtual object ReadInteger(DXTypeCode typeCode) {
			switch(typeCode) {
				case DXTypeCode.SByte:
					return input.ReadSByte();
				case DXTypeCode.Byte:
					return input.ReadByte();
				case DXTypeCode.Int16:
					return input.ReadInt16();
				case DXTypeCode.UInt16:
					return input.ReadUInt16();
				case DXTypeCode.Int32:
					return input.ReadInt32();
				case DXTypeCode.UInt32:
					return input.ReadUInt32();
				case DXTypeCode.Int64:
					return input.ReadInt64();
				case DXTypeCode.UInt64:
					return input.ReadUInt64();
				default:
					throw new Exception();
			}
		}
		protected internal virtual Decimal ReadDecimal(DXTypeCode typeCode) {
			if(typeCode == DXTypeCode.Decimal)
#if SL
				return Convert.ToDecimal(input.ReadDouble());
#else
				return input.ReadDecimal();
#endif
			else
				return Convert.ToDecimal(ReadInteger(typeCode));
		}
		protected internal virtual DateTime ReadDateTime(DXTypeCode typeCode) {
			Int64 ticks = Convert.ToInt64(ReadInteger(typeCode));
			return new DateTime(ticks);
		}
		protected internal virtual TimeSpan ReadTimeSpan(DXTypeCode typeCode) {
			Int64 ticks = Convert.ToInt64(ReadInteger(typeCode));
			return TimeSpan.FromTicks(ticks);
		}
		protected internal virtual string ReadString() {
			return input.ReadString();
		}
		protected internal virtual Guid ReadGuid() {
			byte[] bytes = input.ReadBytes(16);
			return new Guid(bytes);
		}
		protected internal virtual byte[] ReadByteArray() {
			int count = (int)ReadObject();
			return input.ReadBytes(count);
		}
		protected internal virtual object ReadEnum() {
			string typeName = (string)ReadObject();
			string value = (string)ReadObject();
			Type type = Type.GetType(typeName);
			return Enum.Parse(type, value, false);
		}
		protected internal virtual object ReadObjectCore() {
			string typeName = (string)ReadObject();
			string serializedObject = input.ReadString();
			Type type = Type.GetType(typeName);
			if(!String.IsNullOrEmpty(typeName) && type == null && HasCustomObjectConverter) {
				type = CustomObjectConverter.GetType(typeName);
				if(type != null)
					return CustomObjectConverter.FromString(type, serializedObject);
			}
			return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(serializedObject);
		}
	}
	#endregion
}
