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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region BitwiseContainer
	public class BitwiseContainer {
		#region Fields
		private enum TypeContainer {
			TypeByte = 1,
			TypeShort = 2,
			TypeInteger = 4,
			TypeLong = 8
		}
		byte[] container;
		int[] arrayIntSize;
		int lenIntIndex;
		#endregion
		#region Constructors
		public BitwiseContainer(int size) {
			InitContainer(size);
		}
		public BitwiseContainer(int size, int[] arrayIntSize)
			: this(size) {
			this.arrayIntSize = arrayIntSize;
			foreach (int len in arrayIntSize)
				lenIntIndex += len;
		}
		public BitwiseContainer() : this((int)TypeContainer.TypeLong * 8) { }
		public BitwiseContainer(int[] indexSize) : this((int)TypeContainer.TypeLong * 8, indexSize) { }
		#endregion
		#region Properties
		public int Size { get { return container.Length; } }
		public byte[] Container { get { return container; } set { container = value; } }
		public byte ByteContainer { get { return (byte)GetType((int)TypeContainer.TypeByte); } set { SetType((long)value); } }
		public short ShortContainer { get { return (short)GetType((int)TypeContainer.TypeShort); } set { SetType((long)value); } }
		public int IntContainer { get { return (int)GetType((int)TypeContainer.TypeInteger); } set { SetType((long)value); } }
		public long LongContainer { get { return (long)GetType((int)TypeContainer.TypeLong); } set { SetType(value); } }
		#endregion
		#region Methods private
		void SetType(long result) {
			for (int i = 0; i < (int)TypeContainer.TypeLong && i < container.Length; i++) {
				container[i] = (byte)result;
				result >>= 8;
			}
		}
		long GetType(int len) {
			long result = 0;
			for (int i = len - 1; i >= 0; i--) {
				if (i < container.Length) {
					result <<= 8;
					result |= container[i];
				}
			}
			return result;
		}
		void InitContainer(int size) {
			int len = size / 8;
			len += (size % 8) > 0 ? 1 : 0;
			container = new byte[len];
			arrayIntSize = new int[0];
		}
		int CalculateIndexArray(ref int index) {
			int aIndex = index / 8;
			index %= 8;
			return aIndex;
		}
		void SetBoolByIndex(int iArray, int iByte, bool value) {
			byte mask = 1;
			mask <<= iByte;
			container[iArray] &= (byte)~mask;
			if (value)
				container[iArray] |= mask;
		}
		int CalculateLenBit(int index) {
			int len = 0;
			for (int i = 0; i < index; i++)
				len += arrayIntSize[i];
			return len;
		}
		long GetIntMask(int len) {
			long mask = 1;
			for (int i = len - 1; i > 0; i--) {
				mask <<= 1;
				mask++;
			}
			return mask;
		}
		int GetIndexBool(ref int index) {
			if (index >= arrayIntSize.Length) {
				index += lenIntIndex - arrayIntSize.Length;
				int indexByArray = CalculateIndexArray(ref index);
				if (indexByArray >= container.Length)
					throw new IndexOutOfRangeException();
				return indexByArray;
			}
			else {
				if (arrayIntSize[index] > 1)
					throw new InvalidCastException("Cannot implicitly convert type 'bool' to 'int'");
				else {
					index = CalculateLenBit(index);
					int indexByArray = CalculateIndexArray(ref index);
					if (indexByArray >= container.Length)
						throw new IndexOutOfRangeException();
					return indexByArray;
				}
			}
		}
		#endregion
		#region Methods public
		public bool GetBoolValue(int index) {
			int indexByArray = GetIndexBool(ref index);
			byte mask = 1;
			mask <<= index;
			if ((container[indexByArray] & (byte)mask) > 0)
				return true;
			return false;
		}
		public void SetBoolValue(int index, bool value) {
			int indexByArray = GetIndexBool(ref index);
			SetBoolByIndex(indexByArray, index, value);
		}
		public int GetIntValue(int index) {
			if (index < arrayIntSize.Length) {
				long result = 0;
				int len = CalculateLenBit(index);
				int startIndex = CalculateIndexArray(ref len);
				int endIndex = startIndex;
				int second = len + arrayIntSize[index];
				long mask = GetIntMask(arrayIntSize[index]);
				endIndex += CalculateIndexArray(ref second);
				for (int end = endIndex; startIndex <= end; end--) {
					if (end < container.Length) {
						result <<= 8;
						result |= container[end];
					}
				}
				result >>= len;
				return (int)(result & mask);
			}
			else
				throw new InvalidCastException("Cannot implicitly convert type 'int' to 'bool'");
		}
		public void SetIntValue(int index, int value) {
			if (index < arrayIntSize.Length) {
				long result = 0;
				int len = CalculateLenBit(index);
				int startIndex = CalculateIndexArray(ref len);
				int endIndex = startIndex;
				int second = len + arrayIntSize[index];
				long mask = GetIntMask(arrayIntSize[index]);
				endIndex += CalculateIndexArray(ref second);
				for (int end = endIndex; startIndex <= end; end--) {
					if (end < container.Length) {
						result <<= 8;
						result |= container[end];
					}
				}
				value &= (int)mask;
				value <<= len;
				mask <<= len;
				result &= ~mask;
				result |= (long)((long)value);
				for (; startIndex <= endIndex; startIndex++) {
					if (startIndex < container.Length) {
						container[startIndex] = (byte)result;
						result >>= 8;
					}
				}
			}
			else
				throw new InvalidCastException("Cannot implicitly convert type 'int' to 'bool'");
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(Object obj) {
			if (typeof(BitwiseContainer) == obj.GetType()) {
				BitwiseContainer other = (BitwiseContainer)obj;
				if (this.container.Length == other.container.Length)
					if (this.arrayIntSize.Length == other.arrayIntSize.Length) {
						for (int i = 0; i < this.container.Length; i++)
							if (this.container[i] != other.container[i])
								return false;
						for (int i = 0; i < this.arrayIntSize.Length; i++)
							if (this.arrayIntSize[i] != other.arrayIntSize[i])
								return false;
						return true;
					}
			}
			return false;
		}
		#endregion
	}
	#endregion
	#region XlsBuildPivotView
	public enum XlsPivotLinesType {
		None,
		Rows,
		Columns
	}
	public class XlsBuildPivotView {
		#region Fields
		PivotField field;
		int countItem;
		#endregion
		#region Properties
		public XlsPivotLinesType PivotLinesType { get; set; }
		public XlsPivotLinesType PivotAxisType { get; set; }
		public int PivotFieldCount { get; set; }
		public int PivotItemRowCount { get; set; }
		public int PivotItemColumnCount { get; set; }
		public int PivotLinesRowCount { get; set; }
		public int PivotLinesColumnCount { get; set; }
		public int PivotPageCount { get; set; }
		public int FilterCountItem { get; set; }
		public int DifferentialFormatLength { get; set; }
		public int CurrentIndexPageAxis { get; set; }
		public int CountPivotFieldItems { get { return countItem; } }
		public int PivotHierarchyCount { get; set; }
		public int PivotPageAxisExtCount { get; set; }
		public int PivotFieldOLAPExtCount { get; set; }
		public int FilterId { get; set; }
		public bool IsPivotSelection { get; set; }
		public bool IsPivotFormat { get; set; }
		public PivotTable PivotTable { get; set; }
		public PivotField PivotField { get { return field; } }
		public PivotAreaReference Reference { get; set; }
		public PivotFormat Format { get; set; }
		public PivotFilter Filter { get; set; }
		public AutoFilterColumn AutoFilterColumn { get; set; }
		public CustomFilter FirstValue { get; set; }
		public CustomFilter SecondValue { get; set; }
		public void SetPivotField(PivotField field, int countItem) {
			if (this.field != null) {
				if (this.field.Items.Count != this.countItem)
					throw new ArgumentException("SXVI number of items does not match the value.");
			}
			this.field = field;
			this.countItem = countItem;
		}
		public void SetPivotFieldByName(string name) {
			this.field = null;
			foreach (PivotField pivotField in PivotTable.Fields)
				if (!String.IsNullOrEmpty(pivotField.Name) && pivotField.Name.Equals(name))
					this.field = pivotField;
		}
		#endregion
	}
	#endregion
	#region XlsCommandPivotView  -- SxView --
	public class XlsCommandPivotView : XlsCommandBase {
		#region Fields
		private enum PivotTableFlags {
			RowsGrandTotal = 0,
			ColumnsGrandTotal = 1,
			AutoFormat = 3,
			NumberAutoFormat =4,
			FontAutoFormat = 5,
			AlignmentAutoFormat = 6,
			BorderAutoFormat = 7,
			PatternAutoFormat = 8,
			WidthHeightAutoFormat = 9
		}
		Ref8U ref8U = new Ref8U();
		int firstDataColumnIndex;
		int cacheIndex;
		int dataFieldPosition = -1;
		int numberOfFields;
		int numberOfRowAxisFields;
		int numberOfColumnAxisFields;
		int numberOfPageFields;
		int numberOfDataFields;
		int rowCount;
		int columnCount;
		XLUnicodeStringNoCch tableName = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch dataName = new XLUnicodeStringNoCch();
		BitwiseContainer pivotFieldFlags = new BitwiseContainer(16);
		#endregion
		#region Properties
		public CellRangeInfo Range {
			get { return ref8U.CellRangeInfo; }
			set {
				Guard.ArgumentNotNull(value, "Range");
				ref8U.CellRangeInfo = value;
			}
		}
		public int FirstRowIndex { get; set; }
		public int FirstDataRowIndex { get; set; }
		public int FirstDataColumnIndex {
			get { return firstDataColumnIndex; }
			set {
				ValueChecker.CheckValue(value, 0, XlsDefs.MaxColumnCount - 1, "FirstDataColumnIndex");
				firstDataColumnIndex = value;
			}
		}
		public int CacheIndex {
			get { return cacheIndex; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "CacheIndex");
				cacheIndex = value;
			}
		}
		public PivotTableAxis DefaultDataFieldAxis { get; set; }
		public int DataFieldPosition {
			get { return dataFieldPosition; }
			set {
				ValueChecker.CheckValue(value, -1, 0x7fff, "DataFieldPosition");
				dataFieldPosition = value;
			}
		}
		public int NumberOfFields {
			get { return numberOfFields; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "NumberOfFields");
				numberOfFields = value;
			}
		}
		public int NumberOfRowAxisFields {
			get { return numberOfRowAxisFields; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "NumberOfRowAxisFields");
				numberOfRowAxisFields = value;
			}
		}
		public int NumberOfColumnAxisFields {
			get { return numberOfColumnAxisFields; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "NumberOfColumnAxisFields");
				numberOfColumnAxisFields = value;
			}
		}
		public int NumberOfPageFields {
			get { return numberOfPageFields; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "NumberOfPageFields");
				numberOfPageFields = value;
			}
		}
		public int NumberOfDataFields {
			get { return numberOfDataFields; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "NumberOfDataFields");
				numberOfDataFields = value;
			}
		}
		public int RowCount {
			get { return rowCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "RowCount");
				rowCount = value;
			}
		}
		public int ColumnCount {
			get { return columnCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "ColumnCount");
				columnCount = value;
			}
		}
		public bool HasRowsGrandTotal {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.RowsGrandTotal);}
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.RowsGrandTotal, value); } 
		}
		public bool HasColumnsGrandTotal {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.ColumnsGrandTotal); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.ColumnsGrandTotal, value); }
		}
		public bool HasAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.AutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.AutoFormat, value); }
		}
		public bool HasNumberAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.NumberAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.NumberAutoFormat, value); }
		}
		public bool HasFontAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.FontAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.FontAutoFormat, value); }
		}
		public bool HasAlignmentAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.AlignmentAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.AlignmentAutoFormat, value); }
		}
		public bool HasBorderAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.BorderAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.BorderAutoFormat, value); }
		}
		public bool HasPatternAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.PatternAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.PatternAutoFormat, value); }
		}
		public bool HasWidthHeightAutoFormat {
			get { return pivotFieldFlags.GetBoolValue((int)PivotTableFlags.WidthHeightAutoFormat); }
			set { pivotFieldFlags.SetBoolValue((int)PivotTableFlags.WidthHeightAutoFormat, value); }
		}
		public int AutoFormat { get; set; }
		public string TableName {
			get { return tableName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "TableName");
				tableName.Value = value;
			}
		}
		public string DataName {
			get { return dataName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "DataName");
				dataName.Value = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ref8U = Ref8U.FromStream(reader);
			FirstRowIndex = reader.ReadUInt16();
			FirstDataRowIndex = reader.ReadUInt16();
			FirstDataColumnIndex = reader.ReadUInt16();
			CacheIndex = reader.ReadInt16();
			reader.ReadInt16(); 
			DefaultDataFieldAxis = (PivotTableAxis)(reader.ReadUInt16() & 0x000f);
			DataFieldPosition = reader.ReadInt16();
			NumberOfFields = reader.ReadInt16();
			NumberOfRowAxisFields = reader.ReadUInt16();
			NumberOfColumnAxisFields = reader.ReadUInt16();
			NumberOfPageFields = reader.ReadUInt16();
			NumberOfDataFields = reader.ReadInt16();
			RowCount = reader.ReadUInt16();
			ColumnCount = reader.ReadUInt16();
			pivotFieldFlags.ShortContainer = reader.ReadInt16();
			AutoFormat = reader.ReadUInt16();
			int cchTableName = reader.ReadUInt16();
			int cchDataName = reader.ReadUInt16();
			tableName = XLUnicodeStringNoCch.FromStream(reader, cchTableName);
			dataName = XLUnicodeStringNoCch.FromStream(reader, cchDataName);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = new XlsBuildPivotView();
			contentBuilder.CurrentBuilderPivotView = builder;
			SetupXlsBuildPivotView(contentBuilder);
			PivotTableLocation location = new PivotTableLocation(
				(CellRange)XlsCellRangeFactory.CreateCellRange(contentBuilder.CurrentSheet, Range.First, Range.Last));
			location.FirstDataColumn = FirstDataColumnIndex - Range.First.Column;
			location.FirstDataRow = FirstDataRowIndex - Range.First.Row;
			location.FirstHeaderRow = FirstRowIndex - Range.First.Row;
			PivotTable pivotTable = new PivotTable(contentBuilder.DocumentModel, TableName, location);
			contentBuilder.CurrentSheet.PivotTables.AddCore(pivotTable);
			builder.PivotTable = pivotTable;
			pivotTable.DataOnRows = DefaultDataFieldAxis == PivotTableAxis.Row;
			contentBuilder.BindPivotTableId.Add(pivotTable, CacheIndex);
			if (DataFieldPosition != -1)
				pivotTable.DataPosition = DataFieldPosition;
			pivotTable.SetRowGrandTotals(HasColumnsGrandTotal);
			pivotTable.SetColumnGrandTotals(HasRowsGrandTotal);
			pivotTable.SetUseAutoFormatting(HasAutoFormat);
			pivotTable.ApplyNumberFormats = HasNumberAutoFormat;
			pivotTable.ApplyFontFormats = HasFontAutoFormat;
			pivotTable.ApplyAlignmentFormats = HasAlignmentAutoFormat;
			pivotTable.ApplyBorderFormats = HasBorderAutoFormat;
			pivotTable.ApplyPatternFormats = HasPatternAutoFormat;
			pivotTable.ApplyWidthHeightFormats = HasWidthHeightAutoFormat;
			pivotTable.AutoFormatId = AutoFormat;
			pivotTable.SetDataCaptionCore(DataName);
		}
		void SetupXlsBuildPivotView(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderView = contentBuilder.CurrentBuilderPivotView;
			builderView.PivotFieldCount = NumberOfFields;
			builderView.PivotItemRowCount = NumberOfRowAxisFields;
			builderView.PivotItemColumnCount = NumberOfColumnAxisFields;
			builderView.PivotLinesRowCount = RowCount;
			builderView.PivotLinesColumnCount = ColumnCount;
			builderView.PivotPageCount = NumberOfPageFields;
			builderView.PivotAxisType = XlsPivotLinesType.None;
			if (NumberOfRowAxisFields > 0)
				builderView.PivotAxisType = XlsPivotLinesType.Rows;
			else if (NumberOfColumnAxisFields > 0)
				builderView.PivotAxisType = XlsPivotLinesType.Columns;
			if (RowCount > 0 || ColumnCount > 0) {
				builderView.PivotLinesType = XlsPivotLinesType.Rows;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			ref8U.Write(writer);
			writer.Write((ushort)FirstRowIndex);
			writer.Write((ushort)FirstDataRowIndex);
			writer.Write((ushort)FirstDataColumnIndex);
			writer.Write((short)CacheIndex);
			writer.Write((short)0); 
			writer.Write((ushort)DefaultDataFieldAxis);
			writer.Write((short)DataFieldPosition);
			writer.Write((short)NumberOfFields);
			writer.Write((short)NumberOfRowAxisFields);
			writer.Write((short)NumberOfColumnAxisFields);
			writer.Write((short)NumberOfPageFields);
			writer.Write((short)NumberOfDataFields);
			writer.Write((ushort)RowCount);
			writer.Write((ushort)ColumnCount);
			writer.Write((ushort)pivotFieldFlags.ShortContainer);
			writer.Write((ushort)AutoFormat);
			writer.Write((ushort)TableName.Length);
			writer.Write((ushort)DataName.Length);
			tableName.Write(writer);
			dataName.Write(writer);
		}
		protected override short GetSize() {
			return (short)(44 + tableName.Length + dataName.Length);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
