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
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region BinaryRPNWriterXls
	public class BinaryRPNWriterXls : BinaryRPNWriterBase {
		#region Fields
		BinaryWriter writerExtra;
		int currentExpression;
		#endregion
		public BinaryRPNWriterXls(IRPNContext context)
			: base(context) {
		}
		#region Properties
		protected new IXlsRPNContext Context { get { return base.Context as IXlsRPNContext; } }
		public override int CurrentExpression { get { return currentExpression; } }
		#endregion
		public override byte[] GetBinary(ParsedExpression expression) {
			IParsedThingFactory factory = (Context.WorkbookContext.DefinedNameProcessing) ? ParsedThingFactories.NameFactory : ParsedThingFactories.CommonFactory;
			using (MemoryStream stream = new MemoryStream()) {
				using (Writer = new BinaryWriter(stream)) {
					Writer.Write((ushort)0);
					using (MemoryStream streamExtra = new MemoryStream()) {
						using (writerExtra = new BinaryWriter(streamExtra)) {
							for (currentExpression = 0; currentExpression < expression.Count; currentExpression++) {
								WriteBaseData((ParsedThingBase)expression[currentExpression], factory);
								expression[currentExpression].Visit(this);
								while (PendingOffsetWriters.Count > 0) {
									IOffsetPositionWriter currentOffcetWriter = PendingOffsetWriters[0];
									if (currentOffcetWriter.OffsetPositionThingsCounter != currentExpression)
										break;
									currentOffcetWriter.WriteData(Writer);
									PendingOffsetWriters.RemoveAt(0);
								}
							}
							stream.Seek(0, SeekOrigin.Begin);
							Writer.Write((ushort)(stream.Length - 2));
							stream.Seek(0, SeekOrigin.End);
							Writer.Write(streamExtra.ToArray());
						}
					}
				}
				return stream.ToArray();
			}
		}
		public override void Visit(ParsedThingExp thing) {
			CellPosition position = thing.Position;
			Writer.Write((ushort)position.Row);
			Writer.Write((ushort)position.Column);
		}
		public override void Visit(ParsedThingDataTable thing) {
			CellPosition position = thing.Position;
			Writer.Write((ushort)position.Row);
			Writer.Write((ushort)position.Column);
		}
		#region Elf
		void VisitParsedThingPositionElfBase(ParsedThingPositionElfBase thing) {
			Writer.Write((ushort)thing.Position.Row);
			ushort bitwiseField = (ushort)(thing.Position.Column & 0x3fff);
			if (thing.Quoted)
				bitwiseField |= 0x4000;
			if (thing.Position.ColumnType == PositionType.Relative) 
				bitwiseField |= 0x8000;
			Writer.Write(bitwiseField);
		}
		void VisitParsedThingElfBase(ParsedThingElfBase thing) {
			Writer.Write((uint)0); 
			IList<CellPosition> positions = thing.Positions;
			int count = positions.Count;
			uint bitwiseField = (uint)(count & 0x3fffffff);
			if (thing.IsRelative)
				bitwiseField |= 0x80000000;
			writerExtra.Write(bitwiseField);
			for (int i = 0; i < count; i++) {
				writerExtra.Write((ushort)positions[i].Row);
				writerExtra.Write((ushort)(positions[i].Column & 0x3fff));
			}
		}
		void VisitParsedThingElfLelBase(ParsedThingElfLelBase thing) {
			Writer.Write((ushort)thing.Index);
			Writer.Write(Convert.ToUInt16(thing.Quoted));
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
			Writer.Write(thing.Index);
		}
		#endregion
		#region Attributes
		public override void Visit(ParsedThingAttrSemi thing) {
			Writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingAttrSum thing) {
			Writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingAttrBaxcel thing) {
			Writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingAttrBaxcelVolatile thing) {
			Writer.Write((ushort)0); 
		}
		#endregion
		#region Operand
		protected override void WriteRefBase(ParsedThingRef thing) {
			BinaryWriter writer = Writer;
			CellPosition position = thing.Position;
			writer.Write((ushort)position.Row);
			ushort bitwiseField = (ushort)(position.Column & 0x3fff);
			if (position.ColumnType == PositionType.Relative)
				bitwiseField |= 0x4000;
			if (position.RowType == PositionType.Relative)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
		}
		protected override void WriteAreaBase(ParsedThingArea thing) {
			BinaryWriter writer = Writer;
			CellPosition first = thing.TopLeft;
			CellPosition last = thing.BottomRight;
			writer.Write((ushort)first.Row);
			writer.Write((ushort)last.Row);
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
		protected override void WriteRefRel(ParsedThingRefRel thing) {
			BinaryWriter writer = Writer;
			CellOffset location = thing.Location;
			ushort rowRaw = (ushort)location.Row;
			ushort columnRaw = (ushort)((ushort)location.Column & 0x3fff);
			if (location.ColumnType == CellOffsetType.Offset)
				columnRaw |= 0x4000;
			if (location.RowType == CellOffsetType.Offset)
				columnRaw |= 0x8000;
			writer.Write(rowRaw);
			writer.Write(columnRaw);
		}
		protected override void WriteAreaRel(ParsedThingAreaN thing) {
			BinaryWriter writer = Writer;
			CellOffset first = thing.First;
			CellOffset last = thing.Last;
			writer.Write((ushort)first.Row);
			writer.Write((ushort)last.Row);
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
			ShortXLUnicodeString xlsUnicodeStringValue = new ShortXLUnicodeString();
			xlsUnicodeStringValue.Value = thing.Value;
			xlsUnicodeStringValue.Write(Writer);
		}
		public override void Visit(ParsedThingArray thing) {
			BinaryWriter writer = Writer;
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			writer.Write((uint)0); 
			writerExtra.Write((byte)(thing.Width - 1));
			writerExtra.Write((ushort)(thing.Height - 1));
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
				values[i].Write(writerExtra);
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			throw new System.NotSupportedException("External functions not supported");
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			throw new System.NotSupportedException("Custom functions not supported");
		}
		public override void Visit(ParsedThingName thing) {
			int nameIndex = Context.IndexOfDefinedName(thing.DefinedName);
			Writer.Write(nameIndex);
		}
		void WriteMemThing(ParsedThingMemBase thing) {
			MemThingOffsetPositionWriter positionWriter = new MemThingOffsetPositionWriter(Writer.BaseStream.Position, thing.InnerThingCount + CurrentExpression);
			AddOffsetWriter(positionWriter);
			Writer.Write((ushort)0);
		}
		public override void Visit(ParsedThingMemArea thing) {
			BinaryWriter writer = Writer;
			writer.Write((int)0); 
			WriteMemThing(thing);
			int count = thing.Ranges.Count;
			writerExtra.Write((ushort)count);
			for (int i = 0; i < count; i++) {
				CellRangeInfo range = thing.Ranges[i];
				writerExtra.Write((ushort)range.First.Row);
				writerExtra.Write((ushort)range.Last.Row);
				writerExtra.Write((ushort)range.First.Column);
				writerExtra.Write((ushort)range.Last.Column);
			}
		}
		public override void Visit(ParsedThingMemErr thing) {
			BinaryWriter writer = Writer;
			writer.Write(thing.ErrorValue);
			writer.Write((byte)0); 
			writer.Write((ushort)0); 
			WriteMemThing(thing);
		}
		public override void Visit(ParsedThingMemNoMem thing) {
			Writer.Write((int)0); 
			WriteMemThing(thing);
		}
		public override void Visit(ParsedThingMemFunc thing) {
			WriteMemThing(thing);
		}
		public override void Visit(ParsedThingRefErr thing) {
			BinaryWriter writer = Writer;
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingAreaErr thing) {
			BinaryWriter writer = Writer;
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingNameX thing) {
			BinaryWriter writer = Writer;
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			writer.Write((ushort)Context.IndexOfSheetDefinitionNameX(sheetDefinition, thing.DefinedName));
			int nameIndex = Context.IndexOfDefinedName(thing.DefinedName, sheetDefinition);
			writer.Write(nameIndex);
		}
		public override void Visit(ParsedThingRef3d thing) {
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			Writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			WriteRefBase(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			Writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			WriteAreaBase(thing);
		}
		public override void Visit(ParsedThingErr3d thing) {
			BinaryWriter writer = Writer;
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			BinaryWriter writer = Writer;
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override void Visit(ParsedThingTable thing) {
			throw new System.NotSupportedException("Tables not supported yet");
		}
		public override void Visit(ParsedThingTableExt thing) {
			throw new System.NotSupportedException("External tables not supported yet");
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			Writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			WriteRefRel(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			SheetDefinition sheetDefinition = Context.WorkbookContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			Writer.Write((ushort)Context.IndexOfSheetDefinition(sheetDefinition));
			WriteAreaRel(thing);
		}
		#endregion
	}
	#endregion
}
