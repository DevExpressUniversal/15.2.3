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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	#region XlExpression extensions
	static class XlExpressionExtensions {
		public static void Write(this XlExpression expression, BinaryWriter writer, XlsDataAwareExporter exporter) {
			XlsPtgBinaryWriter ptgWriter = new XlsPtgBinaryWriter(writer, exporter);
			ptgWriter.Write(expression);
		}
		public static byte[] GetBytes(this XlExpression expression, XlsDataAwareExporter exporter) {
			using (MemoryStream ms = new MemoryStream()) {
				using (BinaryWriter writer = new BinaryWriter(ms)) {
					expression.Write(writer, exporter);
					return ms.ToArray();
				}
			}
		}
		public static XlExpression ToXlsExpression(this XlExpression expression) {
			XlsPtgConverter converter = new XlsPtgConverter();
			return converter.Convert(expression);
		}
	}
	#endregion
	#region XlsPtgBinaryWriter
	class XlsPtgBinaryWriter : IXlPtgVisitor {
		readonly BinaryWriter writer;
		BinaryWriter writerExtra;
		readonly XlsDataAwareExporter exporter;
		int currentExpression;
		List<IOffsetPositionWriter> pendingOffsetWriters;
		public XlsPtgBinaryWriter(BinaryWriter writer, XlsDataAwareExporter exporter) {
			this.writer = writer;
			this.exporter = exporter;
			this.pendingOffsetWriters = new List<IOffsetPositionWriter>();
		}
		public void Write(XlExpression expression) {
			long initialPosition = writer.BaseStream.Position;
			writer.Write((ushort)0);
			using (MemoryStream streamExtra = new MemoryStream()) {
				using (writerExtra = new BinaryWriter(streamExtra)) {
					int expressionCount = expression.Count;
					for (currentExpression = 0; currentExpression < expressionCount; currentExpression++) {
						expression[currentExpression].Visit(this);
						while (pendingOffsetWriters.Count > 0) {
							IOffsetPositionWriter currentOffcetWriter = pendingOffsetWriters[0];
							if (currentOffcetWriter.OffsetPositionThingsCounter != currentExpression)
								break;
							currentOffcetWriter.WriteData(writer);
							pendingOffsetWriters.RemoveAt(0);
						}
					}
					long finalPosition = writer.BaseStream.Position;
					writer.BaseStream.Position = initialPosition;
					writer.Write((ushort)(finalPosition - initialPosition - 2));
					writer.BaseStream.Position = finalPosition;
					writer.Write(streamExtra.ToArray());
				}
			}
		}
		#region IXlsPtgVisitor Members
		public void Visit(XlPtgBinaryOperator ptg) {
			WritePtgCode(ptg);
		}
		public void Visit(XlPtgUnaryOperator ptg) {
			WritePtgCode(ptg);
		}
		public void Visit(XlPtgParen ptg) {
			WritePtgCode(ptg);
		}
		public void Visit(XlPtgMissArg ptg) {
			WritePtgCode(ptg);
		}
		public void Visit(XlPtgStr ptg) {
			WritePtgCode(ptg);
			ShortXLUnicodeString xlString = new ShortXLUnicodeString();
			xlString.Value = ptg.Value;
			xlString.Write(writer);
		}
		public void Visit(XlPtgErr ptg) {
			WritePtgCode(ptg);
			writer.Write((byte)ptg.Value);
		}
		public void Visit(XlPtgBool ptg) {
			WritePtgCode(ptg);
			writer.Write((byte)(ptg.Value ? 1 : 0));
		}
		public void Visit(XlPtgInt ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)ptg.Value);
		}
		public void Visit(XlPtgNum ptg) {
			WritePtgCode(ptg);
			writer.Write(ptg.Value);
		}
		public void Visit(XlPtgFunc ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)ptg.FuncCode);
		}
		public void Visit(XlPtgFuncVar ptg) {
			WritePtgCode(ptg);
			writer.Write((byte)ptg.ParamCount);
			writer.Write((ushort)ptg.FuncCode);
		}
		public void Visit(XlPtgRef ptg) {
			WritePtgCode(ptg);
			WriteCellPosition(ptg.CellPosition);
		}
		public void Visit(XlPtgRef3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			WriteCellPosition(ptg.CellPosition);
		}
		public void Visit(XlPtgArea ptg) {
			WritePtgCode(ptg);
			WriteArea(ptg.TopLeft, ptg.BottomRight);
		}
		public void Visit(XlPtgArea3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			WriteArea(ptg.TopLeft, ptg.BottomRight);
		}
		public void Visit(XlPtgAttrSemi ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)0); 
		}
		public void Visit(XlPtgAttrSpace ptg) {
			WritePtgCode(ptg);
			writer.Write((byte)ptg.SpaceType);
			writer.Write((byte)ptg.CharCount);
		}
		public void Visit(XlPtgAttrIf ptg) {
			WritePtgCode(ptg);
			AttrIfOffsetPositionWriter positionWriter = new AttrIfOffsetPositionWriter(writer.BaseStream.Position, ptg.Offset + currentExpression);
			AddOffsetWriter(positionWriter);
			writer.Write((ushort)0);
		}
		public void Visit(XlPtgAttrGoto ptg) {
			WritePtgCode(ptg);
			AttrGotoOffsetPositionWriter positionWriter = new AttrGotoOffsetPositionWriter(writer.BaseStream.Position, ptg.Offset + currentExpression);
			AddOffsetWriter(positionWriter);
			writer.Write((ushort)0);
		}
		public void Visit(XlPtgAttrChoose ptg) {
			WritePtgCode(ptg);
			int count = ptg.Offsets.Count;
			writer.Write((ushort)count);
			writer.Write((ushort)((count + 1) * 2));
			for (int i = 0; i < count; i++) {
				AttrChoosePartOffsetPositionWriter positionWriter = new AttrChoosePartOffsetPositionWriter(writer.BaseStream.Position, ptg.Offsets[i] + currentExpression, i);
				AddOffsetWriter(positionWriter);
				writer.Write((ushort)0);
			}
		}
		public void Visit(XlPtgRefN ptg) {
			WritePtgCode(ptg);
			WriteCellOffcet(ptg.CellOffset);
		}
		public void Visit(XlPtgRefN3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			WriteCellOffcet(ptg.CellOffset);
		}
		public void Visit(XlPtgAreaN ptg) {
			WritePtgCode(ptg);
			WriteAreaRelative(ptg.TopLeft, ptg.BottomRight);
		}
		public void Visit(XlPtgAreaN3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			WriteAreaRelative(ptg.TopLeft, ptg.BottomRight);
		}
		public void Visit(XlPtgRefErr ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public void Visit(XlPtgAreaErr ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public void Visit(XlPtgRefErr3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public void Visit(XlPtgAreaErr3d ptg) {
			WritePtgCode(ptg);
			int sheetDefinitionIndex = exporter.RegisterSheetDefinition(ptg.SheetName);
			writer.Write((ushort)sheetDefinitionIndex);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		#region Array
		public void Visit(XlPtgArray ptg) {
			WritePtgCode(ptg);
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			writer.Write((uint)0); 
			writerExtra.Write((byte)(ptg.Width - 1));
			writerExtra.Write((ushort)(ptg.Height - 1));
			int count = ptg.Width * ptg.Height;
			IList<XlVariantValue> array = ptg.Values;
			for (int i = 0; i < count; i++) {
				XlVariantValue item = array[i];
				WriteArrayItem(item);
			}
		}
		void WriteArrayItem(XlVariantValue item) {
			if (item.IsBoolean)
				WriteArrayBooleanItem(item.BooleanValue);
			else if (item.IsError)
				WriteArrayErrorItem(item.ErrorValue.Type);
			else if (item.IsText)
				WriteArrayTextItem(item.TextValue);
			else if (item.IsNumeric)
				WriteArrayNumericItem(item.NumericValue);
			else
				WriteArrayNilItem();
		}
		void WriteArrayNilItem() {
			writerExtra.Write((byte)0x00);
			writerExtra.Write((uint)0); 
			writerExtra.Write((uint)0); 
		}
		void WriteArrayNumericItem(double value) {
			writerExtra.Write((byte)0x01);
			if (XNumChecker.IsNegativeZero(value))
				value = 0.0;
			writerExtra.Write(value);
		}
		void WriteArrayTextItem(string value) {
			writerExtra.Write((byte)0x02);
			XLUnicodeString stringValue = new XLUnicodeString();
			if (value.Length > 255)
				stringValue.Value = value.Substring(0, 255);
			else
				stringValue.Value = value;
			stringValue.Write(writerExtra);
		}
		void WriteArrayErrorItem(XlCellErrorType xlCellErrorType) {
			writerExtra.Write((byte)0x10);
			writerExtra.Write((byte)xlCellErrorType);
			writerExtra.Write((byte)0); 
			writerExtra.Write((ushort)0); 
			writerExtra.Write((uint)0); 
		}
		void WriteArrayBooleanItem(bool value) {
			writerExtra.Write((byte)0x04);
			writerExtra.Write((byte)(value ? 1 : 0));
			writerExtra.Write((byte)0); 
			writerExtra.Write((ushort)0); 
			writerExtra.Write((uint)0); 
		}
		#endregion
		public void Visit(XlPtgName ptg) {
			WritePtgCode(ptg);
			writer.Write((uint)ptg.NameIndex);
		}
		public void Visit(XlPtgExp ptg) {
			WritePtgCode(ptg);
			writer.Write((ushort)ptg.CellPosition.Row);
			writer.Write((ushort)(byte)ptg.CellPosition.Column);
		}
		#region Mem
		void WriteMemThing(XlPtgMemBase ptg) {
			MemThingOffsetPositionWriter positionWriter = new MemThingOffsetPositionWriter(writer.BaseStream.Position, ptg.InnerThingCount + currentExpression);
			AddOffsetWriter(positionWriter);
			writer.Write((ushort)0);
		}
		public void Visit(XlPtgMemFunc ptg) {
			WritePtgCode(ptg);
			WriteMemThing(ptg);
		}
		#endregion
		#endregion
		void WritePtgCode(XlPtgBase ptg) {
			short ptgCode = ptg.GetPtgCode();
			if (ptgCode > byte.MaxValue)
				writer.Write(ptgCode);
			else
				writer.Write((byte)ptgCode);
		}
		void WriteCellPosition(XlCellPosition cellPosition) {
			writer.Write((ushort)cellPosition.Row);
			ushort colRelU = (byte)cellPosition.Column;
			if (cellPosition.ColumnType == XlPositionType.Relative)
				colRelU |= 0x4000;
			if (cellPosition.RowType == XlPositionType.Relative)
				colRelU |= 0x8000;
			writer.Write(colRelU);
		}
		void WriteCellOffcet(XlCellOffset cellOffset) {
			ushort rowRaw = (ushort)cellOffset.Row;
			ushort columnRaw = (ushort)((ushort)cellOffset.Column & 0x3fff);
			if (cellOffset.ColumnType == XlCellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (cellOffset.RowType == XlCellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(rowRaw);
			writer.Write(columnRaw);
		}
		void WriteArea(XlCellPosition topLeft, XlCellPosition bottomRight) {
			writer.Write((ushort)(topLeft.Row == XlCellPosition.InvalidValue.Row ? 0 : topLeft.Row));
			writer.Write((ushort)(bottomRight.Row == XlCellPosition.InvalidValue.Row ? XlsDefs.MaxRowCount - 1 : bottomRight.Row));
			ushort colRelU = (byte)(topLeft.Column == XlCellPosition.InvalidValue.Column ? 0 : topLeft.Column);
			if (topLeft.ColumnType == XlPositionType.Relative)
				colRelU |= 0x4000;
			if (topLeft.RowType == XlPositionType.Relative)
				colRelU |= 0x8000;
			writer.Write(colRelU);
			colRelU = (byte)(bottomRight.Column == XlCellPosition.InvalidValue.Column ? XlsDefs.MaxColumnCount - 1 : bottomRight.Column);
			if (bottomRight.ColumnType == XlPositionType.Relative)
				colRelU |= 0x4000;
			if (bottomRight.RowType == XlPositionType.Relative)
				colRelU |= 0x8000;
			writer.Write(colRelU);
		}
		void WriteAreaRelative(XlCellOffset first, XlCellOffset last) {
			writer.Write((ushort)first.Row);
			writer.Write((ushort)last.Row);
			ushort columnRaw = (ushort)((ushort)first.Column & 0x3fff);
			if (first.ColumnType == XlCellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (first.RowType == XlCellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
			columnRaw = (ushort)((ushort)last.Column & 0x3fff);
			if (last.ColumnType == XlCellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (last.RowType == XlCellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
		}
		void AddOffsetWriter(IOffsetPositionWriter offcetWriter) {
			int position = Algorithms.BinarySearch(pendingOffsetWriters, new OffcetWriterComparable(offcetWriter.OffsetPositionThingsCounter));
			if (position < 0)
				position = ~position;
			pendingOffsetWriters.Insert(position, offcetWriter);
		}
	}
	#endregion
	#region XlsPtgConverter
	class XlsPtgConverter : IXlPtgVisitor {
		XlExpression result;
		public XlExpression Convert(XlExpression expression) {
			result = new XlExpression();
			foreach (XlPtgBase ptg in expression)
				ptg.Visit(this);
			return result;
		}
		#region IXlPtgVisitor Members
		public void Visit(XlPtgBinaryOperator ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgUnaryOperator ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgParen ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgMissArg ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgStr ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgErr ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgBool ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgInt ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgNum ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgFunc ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgFuncVar ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgRef ptg) {
			if (!OutOfXlsRange(ptg.CellPosition))
				result.Add(ptg);
			else {
				result.Add(new XlPtgRefErr(ptg.DataType));
			}
		}
		public void Visit(XlPtgRef3d ptg) {
			if (!OutOfXlsRange(ptg.CellPosition))
				result.Add(ptg);
			else {
				XlPtgRefErr3d ptgErr = new XlPtgRefErr3d(ptg.SheetName);
				ptgErr.DataType = ptg.DataType;
				result.Add(ptgErr);
			}
		}
		public void Visit(XlPtgArea ptg) {
			if (OutOfXlsRange(ptg.TopLeft)) {
				result.Add(new XlPtgAreaErr(ptg.DataType));
			}
			else if (OutOfXlsRange(ptg.BottomRight)) {
				XlCellPosition bottomRight = new XlCellPosition(
					Math.Min(ptg.BottomRight.Column, XlsDefs.MaxColumnCount - 1),
					Math.Min(ptg.BottomRight.Row, XlsDefs.MaxRowCount - 1),
					ptg.BottomRight.ColumnType, ptg.BottomRight.RowType);
				result.Add(new XlPtgArea(ptg.TopLeft, bottomRight) { DataType = ptg.DataType });
			}
			else
				result.Add(ptg);
		}
		public void Visit(XlPtgArea3d ptg) {
			if (OutOfXlsRange(ptg.TopLeft)) {
				XlPtgAreaErr3d ptgErr = new XlPtgAreaErr3d(ptg.SheetName);
				ptgErr.DataType = ptg.DataType;
				result.Add(ptgErr);
			}
			else if (OutOfXlsRange(ptg.BottomRight)) {
				XlCellPosition bottomRight = new XlCellPosition(
					Math.Min(ptg.BottomRight.Column, XlsDefs.MaxColumnCount - 1),
					Math.Min(ptg.BottomRight.Row, XlsDefs.MaxRowCount - 1),
					ptg.BottomRight.ColumnType, ptg.BottomRight.RowType);
				result.Add(new XlPtgArea3d(ptg.TopLeft, bottomRight, ptg.SheetName) { DataType = ptg.DataType });
			}
			else
				result.Add(ptg);
		}
		public void Visit(XlPtgAttrSemi ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAttrSpace ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAttrIf ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAttrGoto ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAttrChoose ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgRefN ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAreaN ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgRefN3d ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAreaN3d ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgRefErr ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAreaErr ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgRefErr3d ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgAreaErr3d ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgArray ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgName ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgMemFunc ptg) {
			result.Add(ptg);
		}
		public void Visit(XlPtgExp ptg) {
			result.Add(ptg);
		}
		#endregion
		bool OutOfXlsRange(XlCellPosition position) {
			return position.Column >= XlsDefs.MaxColumnCount || position.Row >= XlsDefs.MaxRowCount;
		}
	}
	#endregion
	public interface IOffsetPositionWriter {
		long StartPosition { get; set; }
		int OffsetPositionThingsCounter { get; set; }
		void WriteData(BinaryWriter writer);
	}
	#region MemThingOffsetPositionWriter
	public class MemThingOffsetPositionWriter : IOffsetPositionWriter {
		long startPosition;
		int offcetPositionThingsCounter;
		public MemThingOffsetPositionWriter(long startPosition, int offcetPositionThingsCounter) {
			this.startPosition = startPosition;
			this.offcetPositionThingsCounter = offcetPositionThingsCounter;
		}
		#region IOffcetPositionWriter Members
		public long StartPosition { get { return startPosition; } set { startPosition = value; } }
		public int OffsetPositionThingsCounter { get { return offcetPositionThingsCounter; } set { offcetPositionThingsCounter = value; } }
		public void WriteData(BinaryWriter writer) {
			writer.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			writer.Write((ushort)(writer.BaseStream.Length - startPosition - 2));
			writer.Seek(0, SeekOrigin.End);
		}
		#endregion
	}
	#endregion
	#region AttrGotoOffsetPositionWriter
	public class AttrGotoOffsetPositionWriter : IOffsetPositionWriter {
		long startPosition;
		int offcetPositionThingsCounter;
		public AttrGotoOffsetPositionWriter(long startPosition, int offcetPositionThingsCounter) {
			this.startPosition = startPosition;
			this.offcetPositionThingsCounter = offcetPositionThingsCounter;
		}
		#region IOffcetPositionWriter Members
		public long StartPosition { get { return startPosition; } set { startPosition = value; } }
		public int OffsetPositionThingsCounter { get { return offcetPositionThingsCounter; } set { offcetPositionThingsCounter = value; } }
		public void WriteData(BinaryWriter writer) {
			writer.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			writer.Write((ushort)(writer.BaseStream.Length - startPosition - 2 - 1));
			writer.BaseStream.Seek(0, SeekOrigin.End);
		}
		#endregion
	}
	#endregion
	#region AttrIfOffsetPositionWriter
	public class AttrIfOffsetPositionWriter : IOffsetPositionWriter {
		long startPosition;
		int offcetPositionThingsCounter;
		public AttrIfOffsetPositionWriter(long startPosition, int offcetPositionThingsCounter) {
			this.startPosition = startPosition;
			this.offcetPositionThingsCounter = offcetPositionThingsCounter;
		}
		#region IOffcetPositionWriter Members
		public long StartPosition { get { return startPosition; } set { startPosition = value; } }
		public int OffsetPositionThingsCounter { get { return offcetPositionThingsCounter; } set { offcetPositionThingsCounter = value; } }
		public void WriteData(BinaryWriter writer) {
			writer.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			writer.Write((ushort)(writer.BaseStream.Length - startPosition - 2));
			writer.BaseStream.Seek(0, SeekOrigin.End);
		}
		#endregion
	}
	#endregion
	#region AttrChoosePartOffsetPositionWriter
	public class AttrChoosePartOffsetPositionWriter : IOffsetPositionWriter {
		long startPosition;
		int offcetPositionThingsCounter;
		int partIndex;
		public AttrChoosePartOffsetPositionWriter(long startPosition, int offcetPositionThingsCounter, int partIndex) {
			this.startPosition = startPosition;
			this.offcetPositionThingsCounter = offcetPositionThingsCounter;
			this.partIndex = partIndex;
		}
		#region IOffcetPositionWriter Members
		public long StartPosition { get { return startPosition; } set { startPosition = value; } }
		public int OffsetPositionThingsCounter { get { return offcetPositionThingsCounter; } set { offcetPositionThingsCounter = value; } }
		public void WriteData(BinaryWriter writer) {
			writer.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			writer.Write((ushort)(writer.BaseStream.Length - startPosition + (partIndex + 1) * 2));
			writer.BaseStream.Seek(0, SeekOrigin.End);
		}
		#endregion
	}
	#endregion
	#region OffcetReaderComparable<T>
	public class OffcetWriterComparable : IComparable<IOffsetPositionWriter> {
		readonly long thingsCounter;
		public OffcetWriterComparable(long thingsCounter) {
			this.thingsCounter = thingsCounter;
		}
		public int CompareTo(IOffsetPositionWriter other) {
			return (int)(other.OffsetPositionThingsCounter - thingsCounter);
		}
	}
	#endregion
}
