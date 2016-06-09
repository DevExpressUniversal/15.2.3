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
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region BinaryRPNReaderXls
	public class BinaryRPNReaderXls : BinaryRPNReaderBase {
		#region Fields
		List<IOffsetPositionReader> pendingOffcetReaders;
		BinaryReader extraReader;
		#endregion
		public BinaryRPNReaderXls(IRPNContext context)
			: base(context, context.WorkbookContext) {
			pendingOffcetReaders = new List<IOffsetPositionReader>();
		}
		#region Properties
		protected new IXlsRPNContext Context { get { return base.RpnContext as IXlsRPNContext; } }
		protected BinaryReader ExtraReader { get { return extraReader; } }
		#endregion
		public override ParsedExpression FromBinary(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			if (data.Length < 2)
				throw new ArgumentException("Data length less than 2");
			int bytesCount = BitConverter.ToUInt16(data, 0);
			if (bytesCount == 0)
				return new ParsedExpression();
			Expression = new ParsedExpression();
			Process(data, 2, bytesCount);
			return Expression;
		}
		public ParsedExpression FromBinary(byte[] data, int start, int count) {
			Guard.ArgumentNotNull(data, "data");
			Expression = new ParsedExpression();
			Process(data, start, count);
			return Expression;
		}
		public ParsedExpression FromBinary(byte[] data, BinaryReader extraReader) {
			Guard.ArgumentNotNull(data, "data");
			Guard.ArgumentNotNull(extraReader, "extraReader");
			Expression = new ParsedExpression();
			Process(data, extraReader);
			return Expression;
		}
		protected override void Process(byte[] data, int start, int count) {
			using (MemoryStream stream = new MemoryStream(data, start, count, false)) {
				using (Reader = new BinaryReader(stream)) {
					int extraSize = data.Length - count - start;
					using (MemoryStream streamExtra = new MemoryStream(data, start + count, extraSize, false)) {
						using (this.extraReader = new BinaryReader(streamExtra)) {
							ProcessCore();
						}
					}
				}
			}
		}
		protected void Process(byte[] data, BinaryReader extraReader) {
			using (MemoryStream stream = new MemoryStream(data, false)) {
				using (Reader = new BinaryReader(stream)) {
					this.extraReader = extraReader;
					ProcessCore();
				}
			}
		}
		protected override void ProcessCore() {
			IParsedThingFactory factory = (Context.WorkbookContext.DefinedNameProcessing) ? ParsedThingFactories.NameFactory : ParsedThingFactories.CommonFactory;
			while (Reader.BaseStream.Position < Reader.BaseStream.Length) {
				ProcessThing(factory);
				while (pendingOffcetReaders.Count > 0) {
					IOffsetPositionReader pendingOffcetReader = pendingOffcetReaders[0];
					if (Reader.BaseStream.Position < pendingOffcetReader.EndPosition)
						break;
					pendingOffcetReader.ReadData(Expression);
					pendingOffcetReaders.RemoveAt(0);
				}
			}
		}
		void AddOffsetReader(IOffsetPositionReader offcetReader) {
			int position = Algorithms.BinarySearch(pendingOffcetReaders, new OffsetReaderComparable(offcetReader.EndPosition));
			if (position < 0)
				position = ~position;
			pendingOffcetReaders.Insert(position, offcetReader);
		}
		public override void Visit(ParsedThingExp thing) {
			int rowIndex = Reader.ReadUInt16();
			int columnIndex = Reader.ReadUInt16();
			thing.Position = new CellPosition(columnIndex, rowIndex);
		}
		public override void Visit(ParsedThingDataTable thing) {
			int rowIndex = Reader.ReadUInt16();
			int columnIndex = Reader.ReadUInt16();
			thing.Position = new CellPosition(columnIndex, rowIndex);
		}
		#region Elf
		void VisitParsedThingPositionElfBase(ParsedThingPositionElfBase thing) {
			int rowIndex = Reader.ReadUInt16();
			ushort bitwiseField = Reader.ReadUInt16();
			int columnIndex = bitwiseField & 0x3fff;
			thing.Quoted = Convert.ToBoolean(bitwiseField & 0x4000);
			PositionType positionType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			thing.Position = new CellPosition(columnIndex, rowIndex, positionType, positionType);
		}
		void VisitParsedThingElfBase(ParsedThingElfBase thing) {
			Reader.ReadUInt32(); 
			uint bitwiseField = ExtraReader.ReadUInt32();
			int count = (int)(bitwiseField & 0x3fffffff);
			thing.IsRelative = Convert.ToBoolean(bitwiseField & 0x80000000);
			for (int i = 0; i < count; i++) {
				int row = ExtraReader.ReadUInt16();
				int column = ExtraReader.ReadUInt16() & 0x3fff;
				thing.Positions.Add(new CellPosition(column, row));
			}
		}
		void VisitParsedThingElfLelBase(ParsedThingElfLelBase thing) {
			thing.Index = Reader.ReadUInt16();
			thing.Quoted = Convert.ToBoolean(Reader.ReadUInt16());
		}
		public override void Visit(ParsedThingElfLel thing) {
			VisitParsedThingElfLelBase(thing);
		}
		public override void Visit(ParsedThingElfRw thing) {
			VisitParsedThingPositionElfBase(thing);
		}
		public override void Visit(ParsedThingElfCol thing) {
			VisitParsedThingPositionElfBase(thing);
		}
		public override void Visit(ParsedThingElfRwV thing) {
			VisitParsedThingPositionElfBase(thing);
		}
		public override void Visit(ParsedThingElfColV thing) {
			VisitParsedThingPositionElfBase(thing);
		}
		public override void Visit(ParsedThingElfRadical thing) {
			VisitParsedThingPositionElfBase(thing);
		}
		public override void Visit(ParsedThingElfRadicalS thing) {
			VisitParsedThingElfBase(thing);
		}
		public override void Visit(ParsedThingElfColS thing) {
			VisitParsedThingElfBase(thing);
		}
		public override void Visit(ParsedThingElfColSV thing) {
			VisitParsedThingElfBase(thing);
		}
		public override void Visit(ParsedThingElfRadicalLel thing) {
			VisitParsedThingElfLelBase(thing);
		}
		public override void Visit(ParsedThingSxName thing) {
			thing.Index = Reader.ReadInt32();
		}
		#endregion
		#region Attributes
		public override void Visit(ParsedThingAttrSemi thing) {
			Reader.ReadUInt16();
			thing.AddProperty(FormulaProperties.HasVolatileFunction);
		}
		public override void Visit(ParsedThingAttrSum thing) {
			Reader.ReadUInt16(); 
		}
		public override void Visit(ParsedThingAttrBaxcel thing) {
			Reader.ReadUInt16(); 
		}
		public override void Visit(ParsedThingAttrBaxcelVolatile thing) {
			Reader.ReadUInt16(); 
		}
		public override void Visit(ParsedThingAttrIf thing) {
			BinaryReader reader = Reader;
			long endPosition = reader.ReadUInt16() + reader.BaseStream.Position;
			Debug.Assert(endPosition <= reader.BaseStream.Length);
			AttrIfOffsetPositionReader offcetReader = new AttrIfOffsetPositionReader(thing, endPosition);
			AddOffsetReader(offcetReader);
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			int count = Reader.ReadUInt16();
			long startPosition = Reader.BaseStream.Position;
			Reader.ReadUInt16(); 
			for (int i = 0; i < count; i++) {
				long endPosition = Reader.ReadUInt16() + startPosition;
				AttrChooseOffsetPositionReader offcetReader = new AttrChooseOffsetPositionReader(thing, endPosition);
				AddOffsetReader(offcetReader);
			}
		}
		void ReadAttrGoto(ParsedThingAttrGoto thing) {
			BinaryReader reader = Reader;
			long endPosition = reader.ReadUInt16() + reader.BaseStream.Position;
			Debug.Assert(endPosition <= reader.BaseStream.Length);
			AttrGotoOffsetPositionReader offcetReader = new AttrGotoOffsetPositionReader(thing, endPosition);
			AddOffsetReader(offcetReader);
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			ReadAttrGoto(thing);
		}
		#endregion
		#region Operand
		protected override void ReadRefBase(ParsedThingRef thing) {
			int rowIndex = Reader.ReadUInt16();
			ushort bitwiseField = Reader.ReadUInt16();
			int columnIndex = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			thing.Position = new CellPosition(columnIndex, rowIndex, columnType, rowType);
		}
		protected override void ReadAreaBase(ParsedThingArea thing) {
			BinaryReader reader = Reader;
			int firstRow = reader.ReadUInt16();
			int lastRow = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			int column = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			CellPosition first = new CellPosition(column, firstRow, columnType, rowType);
			bitwiseField = reader.ReadUInt16();
			column = bitwiseField & 0x3fff;
			columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			CellPosition last = new CellPosition(column, lastRow, columnType, rowType);
			thing.CellRange = CellRange.PrepareCellRangeBaseValue(null, first, last);
		}
		protected override void ReadRefRel(ParsedThingRefRel thing) {
			BinaryReader reader = Reader;
			ushort rowRaw = reader.ReadUInt16();
			ushort columnRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int column = RawToInt(columnRaw, columnType);
			int row = (rowType == CellOffsetType.Position) ? (int)rowRaw : (int)((short)rowRaw);
			thing.Location = new CellOffset(column, row, columnType, rowType);
		}
		protected override void ReadAreaRel(ParsedThingAreaN thing) {
			BinaryReader reader = Reader;
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
		}
		public override void Visit(ParsedThingStringValue thing) {
			thing.Value = ShortXLUnicodeString.FromStream(Reader).Value;
		}
		public override void Visit(ParsedThingArray thing) {
			Reader.ReadByte(); 
			Reader.ReadUInt16(); 
			Reader.ReadUInt32(); 
			int width = (byte)((int)ExtraReader.ReadByte() + 1);
			int height = (ushort)((int)ExtraReader.ReadUInt16() + 1);
			int count = width * height;
			VariantArray array = VariantArray.Create(width, height);
			for (int i = 0; i < count; i++) {
				IPtgExtraArrayValue item = PtgExtraArrayFactory.CreateArrayValue(ExtraReader);
				item.Read(ExtraReader);
				array.Values[i] = item.Value;
			}
			thing.ArrayValue = array;
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			throw new System.NotSupportedException("External functions not supported");
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			throw new System.NotSupportedException("Custom functions not supported");
		}
		public override void Visit(ParsedThingName thing) {
			thing.DefinedName = Context.GetDefinedName(Reader.ReadInt32());
		}
		public override void Visit(ParsedThingArea thing) {
			ReadAreaBase(thing);
		}
		protected void RegisterMemThing(ParsedThingMemBase thing) {
			BinaryReader reader = Reader;
			long memThingEndPosition = reader.ReadUInt16() + reader.BaseStream.Position;
			Debug.Assert(memThingEndPosition <= reader.BaseStream.Length);
			MemThingOffsetPositionReader offcetReader = new MemThingOffsetPositionReader(thing, memThingEndPosition);
			AddOffsetReader(offcetReader);
		}
		public override void Visit(ParsedThingMemArea thing) {
			BinaryReader reader = Reader;
			reader.ReadInt32(); 
			RegisterMemThing(thing);
			int count = ExtraReader.ReadUInt16();
			for (int i = 0; i < count; i++) {
				int firstRow = ExtraReader.ReadUInt16();
				int lastRow = ExtraReader.ReadUInt16();
				int firstColumn = ExtraReader.ReadUInt16();
				int lastColumn = ExtraReader.ReadUInt16();
				CellRangeInfo range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
				if (range.IsValid())
					thing.Ranges.Add(range);
			}
		}
		public override void Visit(ParsedThingMemErr thing) {
			BinaryReader reader = Reader;
			thing.ErrorValue = reader.ReadByte();
			reader.ReadByte(); 
			reader.ReadInt16(); 
			RegisterMemThing(thing);
		}
		public override void Visit(ParsedThingMemNoMem thing) {
			BinaryReader reader = Reader;
			reader.ReadInt32(); 
			RegisterMemThing(thing);
		}
		public override void Visit(ParsedThingMemFunc thing) {
			RegisterMemThing(thing);
		}
		public override void Visit(ParsedThingRefErr thing) {
			Reader.ReadUInt16(); 
			Reader.ReadUInt16(); 
		}
		public override void Visit(ParsedThingAreaErr thing) {
			BinaryReader reader = Reader;
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
		}
		public override void Visit(ParsedThingRefRel thing) {
			ReadRefRel(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			ReadAreaRel(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			BinaryReader reader = Reader;
			int xtiIndex = reader.ReadUInt16();
			int nameIndex = reader.ReadInt32();
			SheetDefinition sheetDefinition = Context.GetSheetDefinitionNameX(xtiIndex, nameIndex);
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			thing.DefinedName = Context.GetDefinedName(xtiIndex, nameIndex);
		}
		public override void Visit(ParsedThingRef3d thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(Reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			ReadRefBase(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(Reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			ReadAreaBase(thing);
		}
		public override void Visit(ParsedThingErr3d thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(Reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			Reader.ReadInt16(); 
			Reader.ReadInt16(); 
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			BinaryReader reader = Reader;
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			reader.ReadInt16(); 
			reader.ReadInt16(); 
			reader.ReadInt16(); 
			reader.ReadInt16(); 
		}
		public override void Visit(ParsedThingTable thing) {
			throw new System.NotSupportedException("Tables not supported yet");
		}
		public override void Visit(ParsedThingTableExt thing) {
			throw new System.NotSupportedException("External tables not supported yet");
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(Reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			ReadRefRel(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(Reader.ReadUInt16());
			thing.SheetDefinitionIndex = Context.WorkbookContext.RegisterSheetDefinition(sheetDefinition);
			ReadAreaRel(thing);
		}
		#endregion
	}
	#endregion
}
