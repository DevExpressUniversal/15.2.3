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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region ExtNameParsedThingFactory
	public static class ExtNameParsedThingFactory {
		#region Factory instance class
		class ExtNameParsedThingFactoryImpl : ParsedThingFactoryBase {
			protected override void InitializeFactory() {
				AddProduct(0x001c, typeof(ParsedThingError), new ParsedThingErrorCreator());
				AddProduct(0x003a, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
				AddProduct(0x003b, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
				AddProduct(0x003c, typeof(ParsedThingErr3d), new ParsedThingErr3dCreator());
				AddProduct(0x003d, typeof(ParsedThingAreaErr3d), new ParsedThingAreaErr3dCreator());
			}
		}
		#endregion
		#region Fields
		static ExtNameParsedThingFactoryImpl instance;
		static readonly object syncRoot = new object();
		#endregion
		#region Properties
		public static IParsedThingFactory Instance { 
			get {
				if (instance == null) {
					lock (syncRoot) {
						if (instance == null)
							instance = new ExtNameParsedThingFactoryImpl();
					}
				}
				return instance; 
			} 
		}
		#endregion
	}
	#endregion
	#region ExtNameBinaryRPNReader
	public class ExtNameBinaryRPNReader : ParsedThingVisitor {
		#region Fields
		BinaryReader reader;
		IXlsRPNContext context;
		#endregion
		public ExtNameBinaryRPNReader(IXlsRPNContext context) {
			this.context = context;
		}
		public ParsedExpression FromBinary(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			ParsedExpression result = new ParsedExpression();
			using(MemoryStream stream = new MemoryStream(data, false)) {
				using(reader = new BinaryReader(stream)) {
					while(stream.Position < stream.Length) {
						IParsedThing item = ExtNameParsedThingFactory.Instance.CreateParsedThing(reader);
						item.Visit(this);
						result.Add(item);
					}
				}
			}
			return result;
		}
		public override void Visit(ParsedThingError thing) {
			byte errorValue = reader.ReadByte();
			thing.Value = ErrorConverter.ErrorCodeToValue(errorValue);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			int firstSheetIndex = reader.ReadInt16();
			int lastSheetIndex = reader.ReadInt16();
			ushort rowRaw = reader.ReadUInt16();
			ushort columnRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int column = RawToInt(columnRaw, columnType);
			int row = (rowType == CellOffsetType.Position) ? (int)rowRaw : (int)((short)rowRaw);
			thing.Location = new CellOffset(column, row, columnType, rowType);
			SheetDefinition sheetDefinition = this.context.GetSheetDefinition(firstSheetIndex, lastSheetIndex);
			thing.SheetDefinitionIndex = context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			int firstSheetIndex = reader.ReadInt16();
			int lastSheetIndex = reader.ReadInt16();
			ushort firstRowRaw = reader.ReadUInt16();
			ushort lastRowRaw = reader.ReadUInt16();
			ushort columnRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int column = RawToInt(columnRaw, columnType);
			int row = (rowType == CellOffsetType.Position) ? (int)firstRowRaw : (int)((short)firstRowRaw);
			thing.First = new CellOffset(column, row, columnType, rowType);
			columnRaw = reader.ReadUInt16();
			columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			column = RawToInt(columnRaw, columnType);
			row = (rowType == CellOffsetType.Position) ? (int)lastRowRaw : (int)((short)lastRowRaw);
			thing.Last = new CellOffset(column, row, columnType, rowType);
			SheetDefinition sheetDefinition = this.context.GetSheetDefinition(firstSheetIndex, lastSheetIndex);
			thing.SheetDefinitionIndex = this.context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
		}
		public override void Visit(ParsedThingErr3d thing) {
			int firstSheetIndex = reader.ReadInt16();
			int lastSheetIndex = reader.ReadInt16();
			reader.ReadInt32(); 
			SheetDefinition sheetDefinition = this.context.GetSheetDefinition(firstSheetIndex, lastSheetIndex);
			thing.SheetDefinitionIndex = this.context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			int firstSheetIndex = reader.ReadInt16();
			int lastSheetIndex = reader.ReadInt16();
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			SheetDefinition sheetDefinition = this.context.GetSheetDefinition(firstSheetIndex, lastSheetIndex);
			thing.SheetDefinitionIndex = this.context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
		}
		int RawToInt(int value, CellOffsetType locationType) {
			value &= 0x3fff;
			if(locationType == CellOffsetType.Offset) {
				if((value & 0x2000) != 0)
					return (int)((short)(value | 0xc000));
			}
			return value;
		}
	}
	#endregion
	#region ExtNameBinaryRPNWriter
	public class ExtNameBinaryRPNWriter : ParsedThingVisitor {
		#region Fields
		BinaryWriter writer;
		IXlsRPNContext context;
		#endregion
		public ExtNameBinaryRPNWriter(IXlsRPNContext context) {
			this.context = context;
		}
		public byte[] GetBinary(ParsedExpression expression) {
			using(MemoryStream stream = new MemoryStream()) {
				using(writer = new BinaryWriter(stream)) {
					for(int i = 0; i < expression.Count; i++) {
						IParsedThing thing = expression[i];
						short typeCode = ExtNameParsedThingFactory.Instance.GetTypeCodeByType(thing.GetType());
						writer.Write((byte)typeCode); 
						thing.Visit(this);
					}
				}
				return stream.ToArray();
			}
		}
		public override void Visit(ParsedThingError thing) {
			writer.Write((byte)ErrorConverter.ValueToErrorCode(thing.Value));
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			WriteSheetScope(thing.SheetDefinitionIndex);
			CellOffset location = thing.Location;
			ushort rowRaw = (ushort)location.Row;
			ushort columnRaw = (ushort)((ushort)location.Column & 0x3fff);
			if(location.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if(location.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(rowRaw);
			writer.Write(columnRaw);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			WriteSheetScope(thing.SheetDefinitionIndex);
			CellOffset first = thing.First;
			CellOffset last = thing.Last;
			writer.Write((ushort)first.Row);
			writer.Write((ushort)last.Row);
			ushort columnRaw = (ushort)((ushort)first.Column & 0x3fff);
			if(first.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if(first.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
			columnRaw = (ushort)((ushort)last.Column & 0x3fff);
			if(last.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if(last.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
		}
		public override void Visit(ParsedThingErr3d thing) {
			WriteSheetScope(thing.SheetDefinitionIndex);
			writer.Write((int)0); 
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			WriteSheetScope(thing.SheetDefinitionIndex);
			writer.Write((int)0); 
			writer.Write((int)0); 
		}
		void WriteSheetScope(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = context.WorkbookContext.GetSheetDefinition(sheetDefinitionIndex);
			SheetScope scope = this.context.GetSheetScope(sheetDefinition);
			writer.Write((short)scope.FirstSheetIndex);
			writer.Write((short)scope.LastSheetIndex);
		}
	}
	#endregion
}
