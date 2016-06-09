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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IPtgExtraArrayValue
	public interface IPtgExtraArrayValue {
		VariantValue Value { get; set; }
		void Read(BinaryReader reader);
		void Write(BinaryWriter writer);
		int GetSize();
	}
	#endregion
	#region PtgExtraArrayValueBase
	public abstract class PtgExtraArrayValueBase : IPtgExtraArrayValue {
		#region IPtgExtraArrayValue Members
		public abstract VariantValue Value { get; set; }
		public abstract void Read(BinaryReader reader);
		public void Write(BinaryWriter writer) {
			short typeCode = PtgExtraArrayFactory.GetTypeCodeByType(GetType());
			writer.Write((byte)typeCode);
			WriteCore(writer);
		}
		public virtual int GetSize() {
			return 9;
		}
		#endregion
		protected abstract void WriteCore(BinaryWriter writer);
	}
	#endregion
	#region PtgExtraArrayValues
	public class PtgExtraArrayValueNil : PtgExtraArrayValueBase {
		public override VariantValue Value { 
			get { return VariantValue.Empty; } 
			set { } 
		}
		public override void Read(BinaryReader reader) {
			reader.ReadUInt32(); 
			reader.ReadUInt32(); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((uint)0); 
			writer.Write((uint)0); 
		}
	}
	public class PtgExtraArrayValueNum : PtgExtraArrayValueBase {
		double value;
		public override VariantValue Value { 
			get {
				VariantValue result = new VariantValue();
				result.NumericValue = this.value;
				return result;
			} 
			set {
				if(!value.IsNumeric)
					throw new ArgumentException("Non numeric value");
				this.value = value.NumericValue;
				FixNegativeZero();
			} 
		}
		public override void Read(BinaryReader reader) {
			this.value = reader.ReadDouble();
			FixNegativeZero();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.value);
		}
		void FixNegativeZero() {
			if(XNumChecker.IsNegativeZero(this.value))
				this.value = 0.0;
		}
	}
	public class PtgExtraArrayValueStr : PtgExtraArrayValueBase {
		XLUnicodeString stringValue = new XLUnicodeString();
		public override VariantValue Value {
			get {
				VariantValue result = new VariantValue();
				result.InlineTextValue = stringValue.Value;
				return result;
			}
			set {
				if(!value.IsInlineText)
					throw new ArgumentException("Non inline text value");
				if(value.InlineTextValue.Length > 255)
					stringValue.Value = value.InlineTextValue.Substring(0, 255);
				else
					stringValue.Value = value.InlineTextValue;
			}
		}
		public override void Read(BinaryReader reader) {
			stringValue = XLUnicodeString.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			stringValue.Write(writer);
		}
		public override int GetSize() {
			return 1 + stringValue.Length;
		}
	}
	public class PtgExtraArrayValueBool : PtgExtraArrayValueBase {
		bool value;
		public override VariantValue Value {
			get {
				VariantValue result = new VariantValue();
				result.BooleanValue = this.value;
				return result;
			}
			set {
				if(!value.IsBoolean)
					throw new ArgumentException("Non boolean value");
				this.value = value.BooleanValue;
			}
		}
		public override void Read(BinaryReader reader) {
			this.value = Convert.ToBoolean(reader.ReadByte());
			reader.ReadByte(); 
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)(this.value ? 1 : 0));
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			writer.Write((uint)0); 
		}
	}
	public class PtgExtraArrayValueErr : PtgExtraArrayValueBase {
		int errorCode;
		public override VariantValue Value {
			get {
				return ErrorConverter.ErrorCodeToValue(this.errorCode);
			}
			set {
				if(!value.IsError)
					throw new ArgumentException("Non error value");
				this.errorCode = ErrorConverter.ValueToErrorCode(value);
			}
		}
		public override void Read(BinaryReader reader) {
			this.errorCode = reader.ReadByte();
			reader.ReadByte(); 
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)this.errorCode);
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			writer.Write((uint)0); 
		}
	}
	#endregion
	#region PtgExtraArrayFactory
	public static class PtgExtraArrayFactory {
		#region ItemInfo
		internal class ItemInfo {
			short typeCode;
			Type itemType;
			public ItemInfo(short typeCode, Type itemType) {
				this.typeCode = typeCode;
				this.itemType = itemType;
			}
			public short TypeCode { get { return this.typeCode; } }
			public Type ItemType { get { return this.itemType; } }
		}
		#endregion
		static List<ItemInfo> infos;
		static Dictionary<short, ConstructorInfo> itemTypes;
		static Dictionary<Type, short> typeCodes;
		static PtgExtraArrayFactory() {
			infos = new List<ItemInfo>();
			itemTypes = new Dictionary<short, ConstructorInfo>();
			typeCodes = new Dictionary<Type, short>();
			infos.Add(new ItemInfo(0x00, typeof(PtgExtraArrayValueNil)));
			infos.Add(new ItemInfo(0x01, typeof(PtgExtraArrayValueNum)));
			infos.Add(new ItemInfo(0x02, typeof(PtgExtraArrayValueStr)));
			infos.Add(new ItemInfo(0x04, typeof(PtgExtraArrayValueBool)));
			infos.Add(new ItemInfo(0x10, typeof(PtgExtraArrayValueErr)));
			for(int i = 0; i < infos.Count; i++) {
				itemTypes.Add(infos[i].TypeCode, infos[i].ItemType.GetConstructor(new Type[] { }));
				typeCodes.Add(infos[i].ItemType, infos[i].TypeCode);
			}
		}
		public static short GetTypeCodeByType(Type itemType) {
			short typeCode;
			if(!typeCodes.TryGetValue(itemType, out typeCode))
				typeCode = -1;
			return typeCode;
		}
		public static IPtgExtraArrayValue CreateArrayValue(short typeCode) {
			ConstructorInfo itemConstructor = itemTypes[typeCode];
			IPtgExtraArrayValue instance = itemConstructor.Invoke(new object[] { }) as IPtgExtraArrayValue;
			return instance;
		}
		public static IPtgExtraArrayValue CreateArrayValue(BinaryReader reader) {
			short typeCode = reader.ReadByte();
			return CreateArrayValue(typeCode);
		}
		public static IPtgExtraArrayValue CreateArrayValue(VariantValue value) {
			if(value.IsBoolean)
				return new PtgExtraArrayValueBool();
			if(value.IsError)
				return new PtgExtraArrayValueErr();
			if(value.IsInlineText)
				return new PtgExtraArrayValueStr();
			if(value.IsNumeric)
				return new PtgExtraArrayValueNum();
			return new PtgExtraArrayValueNil();
		}
	}
	#endregion
}
