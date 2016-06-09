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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BinaryRPNThingByThingReader
	public abstract class BinaryRPNThingByThingReader : ParsedThingVisitor {
		#region Fields
		readonly WorkbookDataContext context;
		BinaryReader reader;
		IRPNContext rpnContext;
		#endregion
		protected BinaryRPNThingByThingReader(IRPNContext rpnContext, WorkbookDataContext context) {
			this.context = context;
			this.rpnContext = rpnContext;
		}
		#region Properties
		protected BinaryReader Reader { get { return reader; } set { reader = value; } }
		protected IRPNContext RpnContext { get { return rpnContext; } }
		protected WorkbookDataContext Context { get { return context; } }
		#endregion
		protected virtual void Process(byte[] data, int start, int count) {
			using (MemoryStream stream = new MemoryStream(data, start, count, false)) {
				using (this.reader = new BinaryReader(stream)) {
					ProcessCore();
				}
			}
		}
		protected virtual void ProcessCore() {
			IParsedThingFactory factory = ParsedThingFactories.ModelCommonFactory;
			long length = Reader.BaseStream.Length;
			while (Reader.BaseStream.Position < length) {
				ProcessThing(factory);
			}
		}
		protected abstract void ProcessThing(IParsedThingFactory factory);
		#region Attributes
		public override void Visit(ParsedThingAttrSemi thing) {
			thing.AddProperty((FormulaProperties)reader.ReadByte());
		}
		public override void Visit(ParsedThingAttrIf thing) {
			thing.Offset = reader.ReadUInt16();
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			int count = reader.ReadUInt16() + 1;
			for (int i = 0; i < count; i++) {
				thing.Offsets.Add(reader.ReadUInt16());
			}
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			thing.Offset = reader.ReadUInt16();
		}
		public override void Visit(ParsedThingAttrSpace thing) {
			thing.SpaceType = (ParsedThingAttrSpaceType)reader.ReadByte();
			thing.CharCount = reader.ReadByte();
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			thing.SpaceType = (ParsedThingAttrSpaceType)reader.ReadByte();
			thing.CharCount = reader.ReadByte();
		}
		#endregion
		#region Operand
		public static CellPosition ReadCellPosition(byte[] data, int startIndex) {
			int rowIndex = BitConverter.ToInt32(data, startIndex);
			ushort bitwiseField = BitConverter.ToUInt16(data, startIndex + 4);
			int columnIndex = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			return new CellPosition(columnIndex, rowIndex, columnType, rowType);
		}
		protected virtual void ReadRefBase(ParsedThingRef thing) {
			int rowIndex = reader.ReadInt32();
			ushort bitwiseField = reader.ReadUInt16();
			int columnIndex = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			thing.Position = new CellPosition(columnIndex, rowIndex, columnType, rowType);
		}
		protected virtual void ReadAreaBase(ParsedThingArea thing) {
			int firstRow = reader.ReadInt32();
			int lastRow = reader.ReadInt32();
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
			thing.CellRange = CellRange.PrepareCellRangeBaseValue(context.CurrentWorksheet, first, last);
		}
		public static CellRange ReadCellRange(byte[] data, int startIndex, IWorksheet sheet) {
			int firstRow = BitConverter.ToInt32(data, startIndex); 
			int lastRow = BitConverter.ToInt32(data, startIndex + 4); 
			ushort bitwiseField = BitConverter.ToUInt16(data, startIndex + 8);
			int column = bitwiseField & 0x3fff;
			PositionType columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			PositionType rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			CellPosition first = new CellPosition(column, firstRow, columnType, rowType);
			bitwiseField = BitConverter.ToUInt16(data, startIndex + 10);
			column = bitwiseField & 0x3fff;
			columnType = Convert.ToBoolean(bitwiseField & 0x4000) ? PositionType.Relative : PositionType.Absolute;
			rowType = Convert.ToBoolean(bitwiseField & 0x8000) ? PositionType.Relative : PositionType.Absolute;
			CellPosition last = new CellPosition(column, lastRow, columnType, rowType);
			return CellRange.PrepareCellRangeBaseValue(sheet, first, last);
		}
		protected virtual void ReadRefRel(ParsedThingRefRel thing) {
			int rowRaw = (int)reader.ReadUInt32();
			ushort columnRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int column = RawToInt(columnRaw, columnType);
			int row = RowRawToInt(rowRaw, rowType); 
			thing.Location = new CellOffset(column, row, columnType, rowType);
		}
		protected virtual void ReadAreaRel(ParsedThingAreaN thing) {
			int firstRowRaw = (int)reader.ReadUInt32();
			int lastRowRaw = (int)reader.ReadUInt32();
			ushort columnRaw = reader.ReadUInt16();
			CellOffsetType columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			CellOffsetType rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			int column = RawToInt(columnRaw, columnType);
			int row = RowRawToInt(firstRowRaw, rowType);
			thing.First = new CellOffset(column, row, columnType, rowType);
			columnRaw = reader.ReadUInt16();
			columnType = Convert.ToBoolean(columnRaw & 0x4000) ? CellOffsetType.Offset : CellOffsetType.Position;
			rowType = Convert.ToBoolean(columnRaw & 0x8000) ? CellOffsetType.Offset : CellOffsetType.Position;
			column = RawToInt(columnRaw, columnType);
			row = RowRawToInt(lastRowRaw, rowType);
			thing.Last = new CellOffset(column, row, columnType, rowType);
		}
		protected virtual int RowRawToInt(int value, CellOffsetType locationType) {
			if (locationType == CellOffsetType.Offset) {
				if (value >= 0x7FFFF)
					value = (int)((uint)value | 0xFFF00000);
			}
			return value;
		}
		protected virtual int RawToInt(int value, CellOffsetType locationType) {
			value &= 0x3fff;
			if (locationType == CellOffsetType.Offset) {
				if ((value & 0x2000) != 0)
					return (int)((short)(value | 0xc000));
			}
			return value;
		}
		public override void Visit(ParsedThingStringValue thing) {
			thing.Value = reader.ReadString();
		}
		public override void Visit(ParsedThingError thing) {
			byte errorValue = reader.ReadByte();
			thing.Value = ErrorConverter.ErrorCodeToValue(errorValue);
		}
		public override void Visit(ParsedThingBoolean thing) {
			thing.Value = reader.ReadBoolean();
		}
		public override void Visit(ParsedThingInteger thing) {
			thing.Value = reader.ReadUInt16();
		}
		public override void Visit(ParsedThingNumeric thing) {
			thing.Value = reader.ReadDouble();
		}
		public override void Visit(ParsedThingArray thing) {
			int width = (byte)((int)reader.ReadByte() + 1);
			int height = (ushort)((int)reader.ReadUInt16() + 1);
			int count = width * height;
			VariantArray array = VariantArray.Create(width, height);
			for (int i = 0; i < count; i++) {
				IPtgExtraArrayValue item = PtgExtraArrayFactory.CreateArrayValue(reader);
				item.Read(reader);
				array.Values[i] = item.Value;
			}
			thing.ArrayValue = array;
		}
		public override void Visit(ParsedThingFunc thing) {
			thing.FuncCode = reader.ReadUInt16();
		}
		void ReadFuncVar(ParsedThingFuncVar thing) {
			thing.ParamCount = reader.ReadByte();
			thing.FuncCode = reader.ReadUInt16();
		}
		void ReadUnknownFunc(ParsedThingUnknownFunc thing) {
			thing.ParamCount = reader.ReadByte();
			thing.Name = reader.ReadString();
		}
		public override void Visit(ParsedThingFuncVar thing) {
			ReadFuncVar(thing);
		}
		public override void Visit(ParsedThingUnknownFunc thing) {
			ReadUnknownFunc(thing);
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			thing.ParamCount = reader.ReadByte();
			thing.SetFunction(FormulaCalculator.GetFunctionByInvariantName(reader.ReadString(), Context));
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadUnknownFunc(thing);
		}
		public override void Visit(ParsedThingAddinFunc thing) {
			ReadUnknownFunc(thing);
		}
		public override void Visit(ParsedThingRef thing) {
			ReadRefBase(thing);
		}
		public override void Visit(ParsedThingName thing) {
			thing.DefinedName = reader.ReadString();
		}
		public override void Visit(ParsedThingArea thing) {
			ReadAreaBase(thing);
		}
		#region Mem
		public override void Visit(ParsedThingMemArea thing) {
			throw new System.NotSupportedException();
		}
		public override void Visit(ParsedThingMemErr thing) {
			throw new System.NotSupportedException();
		}
		public override void Visit(ParsedThingMemNoMem thing) {
			throw new System.NotSupportedException();
		}
		public override void Visit(ParsedThingMemFunc thing) {
			thing.InnerThingCount = reader.ReadUInt16();
		}
		#endregion
		public override void Visit(ParsedThingRefRel thing) {
			ReadRefRel(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			ReadAreaRel(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			thing.DefinedName = reader.ReadString();
		}
		public override void Visit(ParsedThingRef3d thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadRefBase(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadAreaBase(thing);
		}
		public override void Visit(ParsedThingErr3d thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
		}
		void ReadParsedThingTable(ParsedThingTable thing) {
			thing.TableName = reader.ReadString();
			thing.IncludedRows = (TableRowEnum)reader.ReadByte();
			thing.ColumnStart = reader.ReadString();
			thing.ColumnEnd = reader.ReadString();
		}
		public override void Visit(ParsedThingTable thing) {
			ReadParsedThingTable(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadParsedThingTable(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadRefRel(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			thing.SheetDefinitionIndex = reader.ReadUInt16();
			ReadAreaRel(thing);
		}
		#endregion
	}
	#endregion
	#region IOffsetPositionReader
	interface IOffsetPositionReader {
		long EndPosition { get; }
		void ReadData(ParsedExpression result);
	}
	#endregion
	#region MemThingOffsetPositionReader
	class MemThingOffsetPositionReader : IOffsetPositionReader {
		ParsedThingMemBase memThing;
		long endPosition;
		public MemThingOffsetPositionReader(ParsedThingMemBase memThing, long endPosition) {
			this.memThing = memThing;
			this.endPosition = endPosition;
		}
		#region IOffsetPositionReader Members
		public long EndPosition { get { return endPosition; } }
		public void ReadData(ParsedExpression result) {
			memThing.InnerThingCount = result.Count - result.IndexOf(memThing) - 1;
		}
		#endregion
	}
	#endregion
	#region AttrGotoOffsetPositionReader
	class AttrGotoOffsetPositionReader : IOffsetPositionReader {
		ParsedThingAttrGoto attrGoto;
		long endPosition;
		public AttrGotoOffsetPositionReader(ParsedThingAttrGoto attrGoto, long endPosition) {
			this.attrGoto = attrGoto;
			this.endPosition = endPosition;
		}
		#region IOffsetPositionReader Members
		public long EndPosition { get { return endPosition + 1; } }
		public void ReadData(ParsedExpression result) {
			attrGoto.Offset = result.Count - result.IndexOf(attrGoto) - 1;
		}
		#endregion
	}
	#endregion
	#region AttrIfOffsetPositionReader
	class AttrIfOffsetPositionReader : IOffsetPositionReader {
		ParsedThingAttrIf attrIf;
		long endPosition;
		public AttrIfOffsetPositionReader(ParsedThingAttrIf attrIf, long endPosition) {
			this.attrIf = attrIf;
			this.endPosition = endPosition;
		}
		#region IOffsetPositionReader Members
		public long EndPosition { get { return endPosition; } }
		public void ReadData(ParsedExpression result) {
			attrIf.Offset = result.Count - result.IndexOf(attrIf) - 1;
		}
		#endregion
	}
	#endregion
	#region AttrChooseOffsetPositionReader
	class AttrChooseOffsetPositionReader : IOffsetPositionReader {
		ParsedThingAttrChoose attrChoose;
		long endPosition;
		public AttrChooseOffsetPositionReader(ParsedThingAttrChoose attrChoose, long endPosition) {
			this.attrChoose = attrChoose;
			this.endPosition = endPosition;
		}
		#region IOffsetPositionReader Members
		public long EndPosition { get { return endPosition; } }
		public void ReadData(ParsedExpression result) {
			attrChoose.Offsets.Add(result.Count - result.IndexOf(attrChoose) - 1);
		}
		#endregion
	}
	#endregion
	#region OffsetReaderComparable<T>
	internal class OffsetReaderComparable : IComparable<IOffsetPositionReader> {
		readonly long endPosition;
		public OffsetReaderComparable(long endPosition) {
			this.endPosition = endPosition;
		}
		public int CompareTo(IOffsetPositionReader other) {
			return (int)(other.EndPosition - endPosition);
		}
	}
	#endregion
	#region BinaryRPNReaderBase
	public class BinaryRPNReaderBase : BinaryRPNThingByThingReader {
		#region Fields
		ParsedExpression expression;
		List<IOffsetPositionReader> pendingOffsetReaders;
		#endregion
		public BinaryRPNReaderBase(IRPNContext rpnContext, WorkbookDataContext context)
			: base(rpnContext, context) {
			pendingOffsetReaders = new List<IOffsetPositionReader>();
		}
		#region Properties
		protected ParsedExpression Expression { get { return expression; } set { expression = value; } }
		#endregion
		public virtual ParsedExpression FromBinary(byte[] data) {
			return FromBinary(data, 0);
		}
		public virtual ParsedExpression FromBinary(byte[] data, int startIndex) {
			Guard.ArgumentNotNull(data, "data");
			int dataLength = data.Length;
			if (dataLength < startIndex)
				return new ParsedExpression();
			expression = new ParsedExpression();
			Process(data, startIndex, dataLength - startIndex);
			return expression;
		}
		protected override void ProcessCore() {
			IParsedThingFactory factory = ParsedThingFactories.ModelCommonFactory;
			long length = Reader.BaseStream.Length;
			while (Reader.BaseStream.Position < length) {
				ProcessThing(factory);
				while (pendingOffsetReaders.Count > 0) {
					IOffsetPositionReader pendingOffsetReader = pendingOffsetReaders[0];
					if (Reader.BaseStream.Position < pendingOffsetReader.EndPosition)
						break;
					pendingOffsetReader.ReadData(Expression);
					pendingOffsetReaders.RemoveAt(0);
				}
			}
		}
		protected override void ProcessThing(IParsedThingFactory factory) {
			IParsedThing item = factory.CreateParsedThing(Reader);
			item.Visit(this);
			expression.Add(item);
		}
		void AddOffsetReader(IOffsetPositionReader offsetReader) {
			int position = Algorithms.BinarySearch(pendingOffsetReaders, new OffsetReaderComparable(offsetReader.EndPosition));
			if (position < 0)
				position = ~position;
			pendingOffsetReaders.Insert(position, offsetReader);
		}
		public override void Visit(ParsedThingAttrIf thing) {
			BinaryReader reader = Reader;
			long endPosition = reader.ReadUInt16() + reader.BaseStream.Position;
			System.Diagnostics.Debug.Assert(endPosition <= reader.BaseStream.Length);
			AttrIfOffsetPositionReader offsetReader = new AttrIfOffsetPositionReader(thing, endPosition);
			AddOffsetReader(offsetReader);
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			int count = Reader.ReadUInt16();
			long startPosition = Reader.BaseStream.Position;
			Reader.ReadUInt16(); 
			for (int i = 0; i < count; i++) {
				long endPosition = Reader.ReadUInt16() + startPosition;
				AttrChooseOffsetPositionReader offsetReader = new AttrChooseOffsetPositionReader(thing, endPosition);
				AddOffsetReader(offsetReader);
			}
		}
		void ReadAttrGoto(ParsedThingAttrGoto thing) {
			BinaryReader reader = Reader;
			long endPosition = reader.ReadUInt16() + reader.BaseStream.Position;
			System.Diagnostics.Debug.Assert(endPosition <= reader.BaseStream.Length);
			AttrGotoOffsetPositionReader offsetReader = new AttrGotoOffsetPositionReader(thing, endPosition);
			AddOffsetReader(offsetReader);
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			ReadAttrGoto(thing);
		}
	}
	#endregion
	#region BinaryRPNStringBuilder
	public class BinaryRPNStringBuilder : BinaryRPNThingByThingReader {
		#region Fields
		Stack<int> stack;
		StringBuilder builder;
		StringBuilder spacesBuilder;
		#endregion
		public BinaryRPNStringBuilder(IRPNContext rpnContext, WorkbookDataContext context)
			: base(rpnContext, context) {
		}
		public string BuildExpressionString(byte[] data, int startIndex) {
			stack = new Stack<int>();
			builder = new StringBuilder();
			spacesBuilder = new StringBuilder();
			Process(data, startIndex, data.Length - startIndex);
			if (stack.Count != 1)
				Exceptions.ThrowInternalException();
			return builder.ToString();
		}
		protected override void ProcessThing(IParsedThingFactory factory) {
			IParsedThing item = factory.CreateParsedThing(Reader);
			item.Visit(this);
			item.BuildExpressionString(stack, builder, spacesBuilder, Context);
		}
	}
	#endregion
	#region BinaryRPNCalculator
	public class BinaryRPNCalculator : BinaryRPNThingByThingReader {
		#region Fields
		Stack<VariantValue> stack;
		Stack<bool> gotoStack;
		long memThingCounter = -1;
		ParsedThingMemBase currentMemThing;
		#endregion
		public BinaryRPNCalculator(IRPNContext rpnContext, WorkbookDataContext context)
			: base(rpnContext, context) {
		}
		public VariantValue Evaluate(byte[] data, int startIndex) {
			stack = new Stack<VariantValue>();
			gotoStack = new Stack<bool>();
			DocumentModel documentModel = Context.Workbook;
			documentModel.SuppressCellCreation = true;
			documentModel.RealTimeDataManager.OnStartExpressionEvaluation();
			try {
				Process(data, startIndex, data.Length - startIndex);
			}
			finally {
				documentModel.SuppressCellCreation = false;
					documentModel.RealTimeDataManager.OnEndExpressionEvaluation();
			}
			if (stack.Count != 1)
				Exceptions.ThrowInternalException();
			VariantValue result = stack.Pop();
			if (result.IsEmpty)
				result = 0;
			return result;
		}
		protected override void ProcessCore() {
			IParsedThingFactory factory = ParsedThingFactories.ModelCommonFactory;
			long length = Reader.BaseStream.Length;
			while (Reader.BaseStream.Position < length) {
				ProcessThing(factory);
				if (stack.Count > 0 && stack.Peek() == VariantValue.ErrorGettingData) {
					stack.Clear();
					stack.Push(VariantValue.ErrorGettingData);
					return;
				}
			}
		}
		protected override void ProcessThing(IParsedThingFactory factory) {
			IParsedThing thing = factory.CreateParsedThing(Reader);
			thing.Visit(this);
			ParsedThingMemBase memThing = thing as ParsedThingMemBase;
			if (memThing != null) {
				System.Diagnostics.Debug.Assert(currentMemThing == null);
				currentMemThing = memThing;
				memThingCounter = currentMemThing.InnerThingCount;
				return;
			}
			else {
				ParsedThingAttrIf attrIf = thing as ParsedThingAttrIf;
				if (attrIf != null)
					Reader.BaseStream.Seek(EvaluateAttrIf(Context, attrIf), SeekOrigin.Current);
				else {
					ParsedThingAttrChoose attrChoose = thing as ParsedThingAttrChoose;
					if (attrChoose != null)
						Reader.BaseStream.Seek(EvaluateAttrChoose(Context, attrChoose), SeekOrigin.Current);
					else {
						ParsedThingAttrGoto attrGoto = thing as ParsedThingAttrGoto;
						if (attrGoto != null)
							Reader.BaseStream.Seek(EvaluateAttrGoto(attrGoto), SeekOrigin.Current);
						else {
							if (gotoStack.Count > 0) {
								ParsedThingFunc funcThing = thing as ParsedThingFunc;
								if (funcThing != null) {
									int code = funcThing.FuncCode;
									if (code == 0x0001  || code == 0x0064)
										gotoStack.Pop();
								}
							}
							thing.Evaluate(stack, Context);
						}
					}
				}
			}
			memThingCounter--;
			if (memThingCounter == 0) {
				stack.Push(currentMemThing.ConvertResultToDataType(stack.Pop(), Context));
				currentMemThing = null;
			}
		}
		int EvaluateAttrIf(WorkbookDataContext context, ParsedThingAttrIf attrIf) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			int result = 0;
			VariantValue condition = stack.Peek();
			bool canUseOptimization = !condition.IsArray && !condition.IsCellRange && !condition.IsError;
			if (canUseOptimization) {
				condition = condition.ToBoolean(context);
				canUseOptimization = !condition.IsError;
			}
			if (canUseOptimization) {
				stack.Pop();
				stack.Push(condition.BooleanValue);
				if (!condition.BooleanValue) {
					result = attrIf.Offset;
					stack.Push(VariantValue.Empty);
				}
				gotoStack.Push(true);
			}
			else
				gotoStack.Push(false);
			return result;
		}
		int EvaluateAttrChoose(WorkbookDataContext context, ParsedThingAttrChoose attrChoose) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			int result = 0;
			VariantValue condition = stack.Peek();
			if (condition.IsArray) {
				gotoStack.Push(false);
				return 0;
			}
			int paramsCount = attrChoose.Offsets.Count;
			int numericIndex = 0;
			int fakeValuesCount = paramsCount;
			condition = condition.ToNumeric(context);
			stack.Pop();
			if (condition.IsError) {
				stack.Push(condition);
				result = attrChoose.Offsets[paramsCount - 1];
			}
			else {
				numericIndex = (int)condition.NumericValue;
				if (numericIndex < 1 || numericIndex > 254 || numericIndex >= paramsCount + 1) {
					stack.Push(VariantValue.ErrorInvalidValueInFunction);
					result = attrChoose.Offsets[paramsCount - 1];
				}
				else {
					if (numericIndex == paramsCount) {
						stack.Push(paramsCount);
						fakeValuesCount = paramsCount - 1; 
					}
					else {
						stack.Push(paramsCount - 1);
						fakeValuesCount = paramsCount - 2; 
					}
					if (numericIndex > 1)
						result = attrChoose.Offsets[numericIndex - 2];
				}
			}
			for (int i = 0; i < fakeValuesCount; i++)
				stack.Push(VariantValue.Empty);
			gotoStack.Push(true);
			return result;
		}
		int EvaluateAttrGoto(ParsedThingAttrGoto attrGoto) {
			int result = 0;
			if (gotoStack.Count > 0 && gotoStack.Peek()) {
				if (attrGoto.Offset != 1)
					stack.Push(VariantValue.Empty);
				result = attrGoto.Offset - 1;
			}
			return result;
		}
		public override void Visit(ParsedThingAttrIf thing) {
			thing.Offset = Reader.ReadUInt16();
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			int count = Reader.ReadUInt16();
			int rgSize = Reader.ReadUInt16(); 
			for (int i = 0; i < count; i++) {
				thing.Offsets.Add(Reader.ReadUInt16() - rgSize);
			}
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			thing.Offset = Reader.ReadUInt16() - 2;
		}
	}
	#endregion
}
