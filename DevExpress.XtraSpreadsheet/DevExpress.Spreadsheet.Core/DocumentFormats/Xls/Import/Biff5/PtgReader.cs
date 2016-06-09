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
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsBIFF5PtgReader
	public class XlsBIFF5PtgReader : BinaryRPNReaderXls {
		readonly XlsContentBuilder contentBuilder;
		public XlsBIFF5PtgReader(XlsContentBuilder contentBuilder)
			: base(contentBuilder.RPNContext) {
			this.contentBuilder = contentBuilder;
		}
		public override void Visit(ParsedThingStringValue thing) {
			int length = Reader.ReadByte();
			byte[] buffer = Reader.ReadBytes(length);
			thing.Value = contentBuilder.Options.ActualEncoding.GetString(buffer, 0, length);
		}
		public override void Visit(ParsedThingName thing) {
			thing.DefinedName = Context.GetDefinedName(Reader.ReadUInt16());
			Reader.ReadBytes(12); 
		}
		protected override void ReadRefBase(ParsedThingRef thing) {
			ushort bitwiseField = Reader.ReadUInt16();
			int rowIndex = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			int columnIndex = Reader.ReadByte();
			thing.Position = new CellPosition(columnIndex, rowIndex, columnType, rowType);
		}
		protected override void ReadAreaBase(ParsedThingArea thing) {
			BinaryReader reader = Reader;
			ushort bitwiseField = reader.ReadUInt16();
			int firstRow = bitwiseField & 0x3fff;
			PositionType firstColumnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType firstRowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			bitwiseField = reader.ReadUInt16();
			int lastRow = bitwiseField & 0x3fff;
			PositionType lastColumnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType lastRowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			int column = reader.ReadByte();
			CellPosition first = new CellPosition(column, firstRow, firstColumnType, firstRowType);
			column = reader.ReadByte();
			CellPosition last = new CellPosition(column, lastRow, lastColumnType, lastRowType);
			thing.CellRange = CellRange.PrepareCellRangeBaseValue(null, first, last);
		}
		public override void Visit(ParsedThingRefErr thing) {
			Reader.ReadUInt16(); 
			Reader.ReadByte(); 
		}
		public override void Visit(ParsedThingAreaErr thing) {
			BinaryReader reader = Reader;
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
		}
		protected override void ReadRefRel(ParsedThingRefRel thing) {
			BinaryReader reader = Reader;
			ushort rowRaw = reader.ReadUInt16();
			byte columnRaw = reader.ReadByte();
			CellOffsetType columnType = Convert.ToBoolean(rowRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(rowRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int row = RawToInt(rowRaw, rowType);
			int column = (columnType == CellOffsetType.Position) ? (int)columnRaw : (int)((sbyte)columnRaw);
			thing.Location = new CellOffset(column, row, columnType, rowType);
		}
		protected override void ReadAreaRel(ParsedThingAreaN thing) {
			BinaryReader reader = Reader;
			ushort firstRowRaw = reader.ReadUInt16();
			ushort lastRowRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(firstRowRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(firstRowRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			byte columnRaw = reader.ReadByte();
			int row = RawToInt(firstRowRaw, rowType);
			int column = (columnType == CellOffsetType.Position) ? (int)columnRaw : (int)((sbyte)columnRaw);
			thing.First = new CellOffset(column, row, columnType, rowType);
			columnRaw = reader.ReadByte();
			columnType = Convert.ToBoolean(lastRowRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			rowType = Convert.ToBoolean(lastRowRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			row = RawToInt(lastRowRaw, rowType);
			column = (columnType == CellOffsetType.Position) ? (int)columnRaw : (int)((sbyte)columnRaw);
			thing.Last = new CellOffset(column, row, columnType, rowType);
		}
		public override void Visit(ParsedThingArray thing) {
			Reader.ReadByte(); 
			Reader.ReadUInt16(); 
			Reader.ReadUInt32(); 
			int width = ExtraReader.ReadByte();
			int height = ExtraReader.ReadUInt16();
			int count = width * height;
			VariantArray array = VariantArray.Create(width, height);
			for (int i = 0; i < count; i++) {
				IPtgBIFF5ExtraArrayValue item = PtgBIFF5ExtraArrayFactory.CreateArrayValue(ExtraReader);
				item.Read(ExtraReader, contentBuilder);
				array.Values[i] = item.Value;
			}
			thing.ArrayValue = array;
		}
		public override void Visit(ParsedThingMemArea thing) {
			BinaryReader reader = Reader;
			reader.ReadInt32(); 
			RegisterMemThing(thing);
			int count = ExtraReader.ReadUInt16();
			for (int i = 0; i < count; i++) {
				int firstRow = ExtraReader.ReadUInt16();
				int lastRow = ExtraReader.ReadUInt16();
				int firstColumn = ExtraReader.ReadByte();
				int lastColumn = ExtraReader.ReadByte();
				CellRangeInfo range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
				if (range.IsValid())
					thing.Ranges.Add(range);
			}
		}
	}
	#endregion
	#region IPtgBIFF5ExtraArrayValue
	public interface IPtgBIFF5ExtraArrayValue {
		VariantValue Value { get; }
		void Read(BinaryReader reader, XlsContentBuilder contentBuilder);
	}
	#endregion
	#region PtgBIFF5ExtraArrayValueBase
	public abstract class PtgBIFF5ExtraArrayValueBase : IPtgBIFF5ExtraArrayValue {
		#region IPtgBIFF5ExtraArrayValue Members
		public abstract VariantValue Value { get; }
		public abstract void Read(BinaryReader reader, XlsContentBuilder contentBuilder);
		#endregion
	}
	#endregion
	#region PtgBIFF5ExtraArrayValues
	public class PtgBIFF5ExtraArrayValueNil : PtgBIFF5ExtraArrayValueBase {
		public override VariantValue Value {
			get { return VariantValue.Empty; }
		}
		public override void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt32(); 
			reader.ReadUInt32(); 
		}
	}
	public class PtgBIFF5ExtraArrayValueNum : PtgBIFF5ExtraArrayValueBase {
		double value;
		public override VariantValue Value {
			get {
				VariantValue result = new VariantValue();
				result.NumericValue = this.value;
				return result;
			}
		}
		public override void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadDouble();
			if (XNumChecker.IsNegativeZero(this.value))
				this.value = 0.0;
		}
	}
	public class PtgBIFF5ExtraArrayValueStr : PtgBIFF5ExtraArrayValueBase {
		string stringValue = string.Empty;
		public override VariantValue Value {
			get {
				VariantValue result = new VariantValue();
				result.InlineTextValue = stringValue;
				return result;
			}
		}
		public override void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			int length = reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length);
			stringValue = contentBuilder.Options.ActualEncoding.GetString(buffer, 0, length);
		}
	}
	public class PtgBIFF5ExtraArrayValueBool : PtgBIFF5ExtraArrayValueBase {
		bool value;
		public override VariantValue Value {
			get {
				VariantValue result = new VariantValue();
				result.BooleanValue = this.value;
				return result;
			}
		}
		public override void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			this.value = Convert.ToBoolean(reader.ReadByte());
			reader.ReadByte(); 
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
		}
	}
	public class PtgBIFF5ExtraArrayValueErr : PtgBIFF5ExtraArrayValueBase {
		int errorCode;
		public override VariantValue Value {
			get {
				return ErrorConverter.ErrorCodeToValue(this.errorCode);
			}
		}
		public override void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			this.errorCode = reader.ReadByte();
			reader.ReadByte(); 
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
		}
	}
	#endregion
	#region PtgBIFF5ExtraArrayFactory
	public static class PtgBIFF5ExtraArrayFactory {
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
		static PtgBIFF5ExtraArrayFactory() {
			infos = new List<ItemInfo>();
			itemTypes = new Dictionary<short, ConstructorInfo>();
			typeCodes = new Dictionary<Type, short>();
			infos.Add(new ItemInfo(0x00, typeof(PtgBIFF5ExtraArrayValueNil)));
			infos.Add(new ItemInfo(0x01, typeof(PtgBIFF5ExtraArrayValueNum)));
			infos.Add(new ItemInfo(0x02, typeof(PtgBIFF5ExtraArrayValueStr)));
			infos.Add(new ItemInfo(0x04, typeof(PtgBIFF5ExtraArrayValueBool)));
			infos.Add(new ItemInfo(0x10, typeof(PtgBIFF5ExtraArrayValueErr)));
			for (int i = 0; i < infos.Count; i++) {
				itemTypes.Add(infos[i].TypeCode, infos[i].ItemType.GetConstructor(new Type[] { }));
				typeCodes.Add(infos[i].ItemType, infos[i].TypeCode);
			}
		}
		public static IPtgBIFF5ExtraArrayValue CreateArrayValue(short typeCode) {
			ConstructorInfo itemConstructor = itemTypes[typeCode];
			IPtgBIFF5ExtraArrayValue instance = itemConstructor.Invoke(new object[] { }) as IPtgBIFF5ExtraArrayValue;
			return instance;
		}
		public static IPtgBIFF5ExtraArrayValue CreateArrayValue(BinaryReader reader) {
			short typeCode = reader.ReadByte();
			return CreateArrayValue(typeCode);
		}
	}
	#endregion
}
