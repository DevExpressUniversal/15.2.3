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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BinaryRPNWriterBase
	public class BinaryRPNWriterBase : ParsedThingVisitor {
		#region Fields
		BinaryWriter writer;
		IRPNContext context;
		int currentExpression;
		List<IOffsetPositionWriter> pendingOffsetWriters;
		#endregion
		public BinaryRPNWriterBase(IRPNContext context) {
			this.context = context;
			pendingOffsetWriters = new List<IOffsetPositionWriter>();
		}
		#region Properties
		protected BinaryWriter Writer { get { return writer; } set { writer = value; } }
		protected IRPNContext Context { get { return context; } }
		public virtual int CurrentExpression { get { return currentExpression; } }
		public List<IOffsetPositionWriter> PendingOffsetWriters { get { return pendingOffsetWriters; } }
		#endregion
		public virtual byte[] GetBinary(ParsedExpression expression) {
			IParsedThingFactory factory = ParsedThingFactories.ModelCommonFactory;
			using (MemoryStream stream = new MemoryStream()) {
				using (writer = new BinaryWriter(stream)) {
					int expressionCount = expression.Count;
					for (currentExpression = 0; currentExpression < expressionCount; currentExpression++) {
						WriteBaseData((ParsedThingBase)expression[currentExpression], factory);
						expression[currentExpression].Visit(this);
						while (pendingOffsetWriters.Count > 0) {
							IOffsetPositionWriter currentOffcetWriter = pendingOffsetWriters[0];
							if (currentOffcetWriter.OffsetPositionThingsCounter != currentExpression)
								break;
							currentOffcetWriter.WriteData(Writer);
							pendingOffsetWriters.RemoveAt(0);
						}
					}
				}
				return stream.ToArray();
			}
		}
		protected void AddOffsetWriter(IOffsetPositionWriter offcetWriter) {
			int position = Algorithms.BinarySearch(pendingOffsetWriters, new OffcetWriterComparable(offcetWriter.OffsetPositionThingsCounter));
			if (position < 0)
				position = ~position;
			pendingOffsetWriters.Insert(position, offcetWriter);
		}
		protected void WriteBaseData(ParsedThingBase thing, IParsedThingFactory factory) {
			short typeCode = factory.GetTypeCodeByType(thing.GetType());
			if (thing.DataType != OperandDataType.None)
				typeCode = (short)((typeCode & ~0x0060) | (thing.GetDataType() << 5));
			if (typeCode <= byte.MaxValue && typeCode != 0x0019)
				writer.Write((byte)typeCode);
			else
				writer.Write(typeCode);
		}
		void WriteStringValue(string value) {
			if (string.IsNullOrEmpty(value))
				writer.Write(string.Empty);
			else
				writer.Write(value);
		}
		#region Attributes
		public override void Visit(ParsedThingAttrSemi thing) {
			writer.Write((byte)thing.FormulaProperties);
		}
		public override void Visit(ParsedThingAttrIf thing) {
			AttrIfOffsetPositionWriter positionWriter = new AttrIfOffsetPositionWriter(Writer.BaseStream.Position, thing.Offset + CurrentExpression);
			AddOffsetWriter(positionWriter);
			Writer.Write((ushort)0);
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			int count = thing.Offsets.Count;
			Writer.Write((ushort)count);
			Writer.Write((ushort)((count + 1) * 2));
			for (int i = 0; i < count; i++) {
				AttrChoosePartOffsetPositionWriter positionWriter = new AttrChoosePartOffsetPositionWriter(Writer.BaseStream.Position, thing.Offsets[i] + CurrentExpression, i);
				AddOffsetWriter(positionWriter);
				Writer.Write((ushort)0);
			}
		}
		void VisitAttrGoto(ParsedThingAttrGoto thing) {
			AttrGotoOffsetPositionWriter positionWriter = new AttrGotoOffsetPositionWriter(Writer.BaseStream.Position, thing.Offset + CurrentExpression);
			AddOffsetWriter(positionWriter);
			Writer.Write((ushort)0);
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			VisitAttrGoto(thing);
		}
		public override void Visit(ParsedThingAttrSpace thing) {
			writer.Write((byte)thing.SpaceType);
			writer.Write((byte)thing.CharCount);
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			writer.Write((byte)thing.SpaceType);
			writer.Write((byte)thing.CharCount);
		}
		#endregion
		#region Operand
		public static void WriteCellPostion(CellPosition position, byte[] data, int startIndex) {
			ushort bitwiseField = (ushort)(position.Column & 0x3fff);
			if (position.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (position.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			BitConverter.GetBytes(position.Row).CopyTo(data, startIndex);
			BitConverter.GetBytes(bitwiseField).CopyTo(data, startIndex + 4);
		}
		protected virtual void WriteRefBase(ParsedThingRef thing) {
			CellPosition position = thing.Position;
			writer.Write(position.Row);
			ushort bitwiseField = (ushort)(position.Column & 0x3fff);
			if (position.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (position.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
		}
		protected virtual void WriteAreaBase(ParsedThingArea thing) {
			CellPosition first = thing.TopLeft;
			CellPosition last = thing.BottomRight;
			writer.Write(first.Row);
			writer.Write(last.Row);
			ushort bitwiseField = (ushort)(first.Column & 0x3fff);
			if (first.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (first.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
			bitwiseField = (ushort)(last.Column & 0x3fff);
			if (last.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (last.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
		}
		public static void WriteCellRange(CellRange range, byte[] data, int startIndex) {
			CellPosition first = range.TopLeft;
			CellPosition last = range.BottomRight;
			BitConverter.GetBytes(((long)first.Row << 32) + last.Row).CopyTo(data, startIndex);
			ushort bitwiseField = (ushort)(first.Column & 0x3fff);
			if (first.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (first.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			BitConverter.GetBytes(bitwiseField).CopyTo(data, startIndex + 8);
			bitwiseField = (ushort)(last.Column & 0x3fff);
			if (last.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (last.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			BitConverter.GetBytes(bitwiseField).CopyTo(data, startIndex + 10);
		}
		protected virtual void WriteRefRel(ParsedThingRefRel thing) {
			CellOffset location = thing.Location;
			uint rowRaw = (uint)(location.Row & 0xfffff);
			ushort columnRaw = (ushort)((ushort)location.Column & 0x3fff);
			if (location.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (location.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(rowRaw);
			writer.Write(columnRaw);
		}
		protected virtual void WriteAreaRel(ParsedThingAreaN thing) {
			CellOffset first = thing.First;
			CellOffset last = thing.Last;
			uint rowRaw = (uint)(first.Row & 0xfffff);
			writer.Write(rowRaw);
			rowRaw = (uint)(last.Row & 0xfffff);
			writer.Write(rowRaw);
			ushort columnRaw = (ushort)((ushort)first.Column & 0x3fff);
			if (first.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (first.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
			columnRaw = (ushort)((ushort)last.Column & 0x3fff);
			if (last.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (last.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(columnRaw);
		}
		public override void Visit(ParsedThingStringValue thing) {
			WriteStringValue(thing.Value);
		}
		public override void Visit(ParsedThingError thing) {
			writer.Write((byte)ErrorConverter.ValueToErrorCode(thing.Value));
		}
		public override void Visit(ParsedThingBoolean thing) {
			writer.Write(thing.Value);
		}
		public override void Visit(ParsedThingInteger thing) {
			writer.Write((ushort)thing.Value);
		}
		public override void Visit(ParsedThingNumeric thing) {
			writer.Write(thing.Value);
		}
		public override void Visit(ParsedThingArray thing) {
			writer.Write((byte)(thing.Width - 1));
			writer.Write((ushort)(thing.Height - 1));
			int count = thing.Width * thing.Height;
			IVariantArray array = thing.ArrayValue;
			List<IPtgExtraArrayValue> values = new List<IPtgExtraArrayValue>();
			for (int i = 0; i < count; i++) {
				VariantValue item = array[i];
				IPtgExtraArrayValue extraValue = PtgExtraArrayFactory.CreateArrayValue(item);
				extraValue.Value = item;
				values.Add(extraValue);
			}
			for (int i = 0; i < count; i++)
				values[i].Write(writer);
		}
		public override void Visit(ParsedThingFunc thing) {
			writer.Write((ushort)thing.FuncCode);
		}
		public override void Visit(ParsedThingFuncVar thing) {
			writer.Write((byte)thing.ParamCount);
			writer.Write((ushort)thing.FuncCode);
		}
		void WriteUnknownFunc(ParsedThingUnknownFunc thing) {
			writer.Write((byte)thing.ParamCount);
			writer.Write(thing.Name);
		}
		public override void Visit(ParsedThingUnknownFunc thing) {
			WriteUnknownFunc(thing);
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			writer.Write((byte)thing.ParamCount);
			writer.Write(thing.Name);
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteUnknownFunc(thing);
		}
		public override void Visit(ParsedThingAddinFunc thing) {
			WriteUnknownFunc(thing);
		}
		public override void Visit(ParsedThingRef thing) {
			WriteRefBase(thing);
		}
		public override void Visit(ParsedThingArea thing) {
			WriteAreaBase(thing);
		}
		public override void Visit(ParsedThingName thing) {
			writer.Write(thing.DefinedName);
		}
		#region Mem
		public override void Visit(ParsedThingMemArea thing) {
			throw new NotSupportedException();
		}
		public override void Visit(ParsedThingMemErr thing) {
			throw new NotSupportedException();
		}
		public override void Visit(ParsedThingMemNoMem thing) {
			throw new NotSupportedException();
		}
		public override void Visit(ParsedThingMemFunc thing) {
			writer.Write((ushort)thing.InnerThingCount);
		}
		#endregion
		public override void Visit(ParsedThingRefRel thing) {
			WriteRefRel(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			WriteAreaRel(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			writer.Write(thing.DefinedName);
		}
		public override void Visit(ParsedThingRef3d thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteRefBase(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteAreaBase(thing);
		}
		public override void Visit(ParsedThingErr3d thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
		}
		void WriteParsedThingTable(ParsedThingTable thing) {
			WriteStringValue(thing.TableName);
			writer.Write((byte)thing.IncludedRows);
			WriteStringValue(thing.ColumnStart);
			WriteStringValue(thing.ColumnEnd);
		}
		public override void Visit(ParsedThingTable thing) {
			WriteParsedThingTable(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteParsedThingTable(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteRefRel(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			writer.Write((ushort)thing.SheetDefinitionIndex);
			WriteAreaRel(thing);
		}
		#endregion
	}
	#endregion
}
