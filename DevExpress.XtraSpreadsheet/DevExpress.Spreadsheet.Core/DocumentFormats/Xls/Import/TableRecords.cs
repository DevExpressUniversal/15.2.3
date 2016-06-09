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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region TableFutureRecordHeader
	public class TableFutureRecordHeader : FutureRecordHeaderFlagsBase {
		#region Static Members
		public static TableFutureRecordHeader FromStream(XlsReader reader) {
			TableFutureRecordHeader result = new TableFutureRecordHeader();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		internal const short Feature11RecordTypeId = 0x0872;
		internal const short Feature12RecordTypeId = 0x0878;
		Ref8U ref8Info = new Ref8U();
		short recordTypeId = Feature12RecordTypeId;
		#endregion
		#region Fields
		public override short RecordTypeId { get { return recordTypeId; } set { recordTypeId = value; } }
		protected internal CellRangeInfo CellRangeInfo {
			get { return ref8Info.CellRangeInfo; }
			set {
				Guard.ArgumentNotNull(value, "cellRange");
				RangeOfCells = true;
				ref8Info.CellRangeInfo = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			ref8Info = Ref8U.FromStream(reader);
			RangeOfCells = true;
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			if (CellRangeInfo != null) {
				ref8Info.CellRangeInfo = CellRangeInfo;
				ref8Info.Write(writer);
			} else {
				writer.Write(new byte[Ref8U.Size]);
			}
		}
		public override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region SharedFeatureTable
	public class SharedFeatureTable {
		#region Static Members
		public static SharedFeatureTable FromTableFeatureType(TableFeatureType tableInfo, short recordTypeId) {
			SetupFieldIdentifiers(tableInfo);
			SharedFeatureTable result = new SharedFeatureTable(tableInfo.CurrentSheet, tableInfo.CellRangeInfo, recordTypeId);
			result.tableInfo = tableInfo;
			return result;
		}
		static void SetupFieldIdentifiers(TableFeatureType tableInfo) {
			HashSet<int> uniqueIds = new HashSet<int>();
			int count = tableInfo.Columns.Count;
			int maxId = 0;
			for (int i = 0; i < count; i++) {
				XlsTableColumnInfo column = tableInfo.Columns[i];
				if (column.IdField > 0 && !uniqueIds.Contains(column.IdField)) {
					uniqueIds.Add(column.IdField);
					maxId = Math.Max(maxId, column.IdField);
				}
			}
			for (int i = 0; i < count; i++) {
				XlsTableColumnInfo column = tableInfo.Columns[i];
				if (column.IdField == 0) {
					column.IdField = i + 1;
					if (uniqueIds.Contains(column.IdField))
						column.IdField = maxId + 1;
					uniqueIds.Add(column.IdField);
					maxId = Math.Max(maxId, column.IdField);
				}
			}
		}
		#endregion
		#region Fields
		const short fixedPartSize = 27;
		readonly Worksheet currentSheet; 
		readonly List<Ref8U> listRef8U = new List<Ref8U>();
		readonly CellRangeInfo cellRangeInfo;
		readonly short recordTypeId;
		TableFeatureType tableInfo;
		XlsSharedFeatureType sharedFeatureType = XlsSharedFeatureType.List;
		#endregion
		public SharedFeatureTable(Worksheet currentSheet, CellRangeInfo cellRangeInfo, short recordTypeId) {
			Guard.ArgumentNotNull(currentSheet, "currentSheet");
			this.currentSheet = currentSheet;
			Guard.ArgumentNotNull(cellRangeInfo, "cellRangeInfo");
			this.cellRangeInfo = cellRangeInfo;
			Ref8U info = new Ref8U();
			info.CellRangeInfo = cellRangeInfo;
			ListRef8U.Add(info);
			this.tableInfo = new TableFeatureType(currentSheet, cellRangeInfo);
			this.recordTypeId = recordTypeId;
		}
		#region Properties
		public Worksheet CurrentSheet { get { return currentSheet; } }
		public CellRangeInfo CellRangeInfo { get { return cellRangeInfo; } }
		public List<Ref8U> ListRef8U { get { return listRef8U; } }
		public short RecordTypeId { get { return recordTypeId; } }
		public TableFeatureType TableInfo { get { return tableInfo; } }
		public XlsSharedFeatureType SharedFeatureType { get { return sharedFeatureType; } set { sharedFeatureType = value; } }
		#endregion
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			SharedFeatureType = (XlsSharedFeatureType)reader.ReadUInt16();
			reader.ReadBytes(5);
			int listRef8UCount = reader.ReadInt16();
			reader.ReadInt32();
			reader.ReadInt16();
			ListRef8U.Clear();
			for (int i = 0; i < listRef8UCount; i++)
				ListRef8U.Add(Ref8U.FromStream(reader));
			TableInfo.Read(reader, contentBuilder);
		}
		public void Write(BinaryWriter writer) {
			TableFutureRecordHeader header = new TableFutureRecordHeader();
			header.RecordTypeId = RecordTypeId;
			header.CellRangeInfo = cellRangeInfo;
			header.Write(writer);
			writer.Write((ushort)SharedFeatureType);
			writer.Write(new byte[5]);
			int listRef8UCount = ListRef8U.Count;
			writer.Write((ushort)listRef8UCount);
			writer.Write((uint)0);
			writer.Write((ushort)0);
			for (int i = 0; i < listRef8UCount; i++)
				ListRef8U[i].Write(writer);
			TableInfo.Write(writer);
		}
		public short GetSize() {
			return (short)(fixedPartSize + ListRef8U.Count * Ref8U.Size + TableInfo.GetSize());
		}
		internal void CreateTable(XlsContentBuilder contentBuilder) {
			Table table = TableInfo.CreateTable(contentBuilder);
			if(IsSingleCellTable(table)) 
				return;
			if(table.TableType == TableType.Xml)
				ConvertXmlTable(table);
			if(table.TableType == TableType.QueryTable)
				ConvertQueryTable(table);
			contentBuilder.CurrentSheet.Tables.Add(table);
		}
		bool IsSingleCellTable(Table table) {
			return table.TableType == TableType.Xml && table.Columns.Count == 1 && table.RowCount == 1;
		}
		void ConvertXmlTable(Table table) {
			table.BeginUpdate();
			try {
				table.TableType = TableType.Worksheet;
			}
			finally {
				table.EndUpdate();
			}
			foreach(TableColumn column in table.Columns) {
				column.BeginUpdate();
				try {
					column.XmlProperties = null;
				}
				finally {
					column.EndUpdate();
				}
			}
		}
		void ConvertQueryTable(Table table) {
			table.BeginUpdate();
			try {
				table.TableType = TableType.Worksheet;
			}
			finally {
				table.EndUpdate();
			}
			foreach(TableColumn column in table.Columns) {
				column.BeginUpdate();
				try {
					column.QueryTableFieldId = TableColumn.DefaultQueryTableFieldId;
				}
				finally {
					column.EndUpdate();
				}
			}
		}
	}
	#endregion
	#region TableFeatureType
	public class TableFeatureType {
		#region Static Members
		public static TableFeatureType FromTable(Table table, ExportXlsStyleSheet styleSheet) {
			CellRange range = table.Range;
			CellRangeInfo rangeInfo = new CellRangeInfo(range.TopLeft, range.BottomRight);
			TableFeatureType result = new TableFeatureType(table.Worksheet, rangeInfo);
			result.AssignThisProperties(table, styleSheet);
			return result;
		}
		#endregion
		#region Fields
		const short fixedPartSize = 66;
		const int byteCountOfFixedSizeData = 64;
		internal const int HashParamentersCount = 16;
		readonly XlsTableFlagsInfo flagsInfo = new XlsTableFlagsInfo();
		readonly List<XlsTableColumnInfo> columns = new List<XlsTableColumnInfo>();
		readonly Worksheet currentSheet;
		readonly CellRangeInfo cellRangeInfo;
		readonly List<int> feature11RgSharepointIdDel = new List<int>();
		readonly List<int> feature11RgSharepointIdChange = new List<int>();
		readonly List<XlsFeature11CellStruct> feature11RgInvalidCells = new List<XlsFeature11CellStruct>();
		byte[] hashParameters = new byte[HashParamentersCount];
		XLUnicodeString tableName = new XLUnicodeString();
		XLUnicodeString cryptographicServiceProviderName = new XLUnicodeString();
		XLUnicodeString entryIdName = new XLUnicodeString();
		XlsLemMode lemMode = XlsLemMode.Normal;
		TableType tableType = TableType.Worksheet;
		bool hasHeadersRow = true;
		#endregion
		public TableFeatureType(Worksheet currentSheet, CellRangeInfo cellRangeInfo) {
			Guard.ArgumentNotNull(currentSheet, "currentSheet");
			this.currentSheet = currentSheet;
			Guard.ArgumentNotNull(cellRangeInfo, "cellRangeInfo");
			this.cellRangeInfo = cellRangeInfo;
		}
		#region Properties
		public Worksheet CurrentSheet { get { return currentSheet; } }
		public CellRangeInfo CellRangeInfo { get { return cellRangeInfo; } }
		public XlsTableFlagsInfo FlagsInfo { get { return flagsInfo; } }
		public byte[] HashParameters { get { return hashParameters; } }
		public List<int> Feature11RgSharepointIdDel { get { return feature11RgSharepointIdDel; } }
		public List<int> Feature11RgSharepointIdChange { get { return feature11RgSharepointIdChange; } }
		public List<XlsFeature11CellStruct> Feature11RgInvalidCells { get { return feature11RgInvalidCells; } }
		public List<XlsTableColumnInfo> Columns { get { return columns; } }
		public XlsLemMode LemMode { get { return lemMode; } set { lemMode = value; } }
		public TableType TableType { get { return tableType; } set { tableType = value; } }
		public string TableName { get { return tableName.Value; } set { tableName.Value = value; } }
		public string CryptographicServiceProviderName { get { return cryptographicServiceProviderName.Value; } set { cryptographicServiceProviderName.Value = value; } }
		public string EntryIdName { get { return entryIdName.Value; } set { entryIdName.Value = value; } }
		public bool HasHeadersRow { get { return hasHeadersRow; } set { hasHeadersRow = value; } }
		public bool HasTotalsRow { get; set; }
		public int IdList { get; set; }
		public int IdFieldNext { get; set; }
		public int DataStreamCachePosition { get; set; }
		public int DataStreamCacheSize { get; set; }
		public int DataStreamCacheCharacterCount { get; set; }
		#endregion
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			TableType = (TableType)reader.ReadUInt32();
			IdList = reader.ReadInt32();
			HasHeadersRow = reader.ReadInt32() > 0;
			HasTotalsRow = reader.ReadInt32() > 0;
			IdFieldNext = reader.ReadInt32();
			reader.ReadInt64();
			FlagsInfo.Read(reader);
			DataStreamCachePosition = reader.ReadInt32();
			DataStreamCacheSize = reader.ReadInt32();
			DataStreamCacheCharacterCount = reader.ReadInt32();
			LemMode = (XlsLemMode)reader.ReadUInt32();
			hashParameters = reader.ReadBytes(HashParamentersCount);
			this.tableName = XLUnicodeString.FromStream(reader);
			int tableColumnInfoCount = reader.ReadInt16();
			if (FlagsInfo.LoadCSPName)
				this.cryptographicServiceProviderName = XLUnicodeString.FromStream(reader);
			if (FlagsInfo.LoadEntryId)
				this.entryIdName = XLUnicodeString.FromStream(reader);
			for (int i = 0; i < tableColumnInfoCount; i++) {
				XlsTableColumnInfo columnInfo = new XlsTableColumnInfo(this);
				columnInfo.Read(reader, contentBuilder);
				Columns.Add(columnInfo);
			}
			if (FlagsInfo.LoadPresentIdDeleted)
				ReadListInt(reader, Feature11RgSharepointIdDel);
			if (FlagsInfo.LoadPresentIdChanged)
				ReadListInt(reader, Feature11RgSharepointIdChange);
			if (FlagsInfo.LoadPresentCellInvalid)
				ReadFeature11RgInvalidCells(reader);
		}
		void ReadListInt(BinaryReader reader, List<int> array) {
			short arrayCount = reader.ReadInt16();
			for (short i = 0; i < arrayCount; i++)
				array.Add(reader.ReadInt32());
		}
		void ReadFeature11RgInvalidCells(BinaryReader reader) {
			short arrayCount = reader.ReadInt16();
			for (short i = 0; i < arrayCount; i++) {
				XlsFeature11CellStruct element = XlsFeature11CellStruct.FromStream(reader);
				Feature11RgInvalidCells.Add(element);
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write((int)TableType);
			writer.Write(IdList);
			writer.Write(HasHeadersRow ? 1 : 0);
			writer.Write(HasTotalsRow ? 1 : 0);
			writer.Write((uint)IdFieldNext);
			writer.Write((uint)byteCountOfFixedSizeData);
			writer.Write(XlsDefs.DefaultBuildIdentifier);
			writer.Write((short)0);
			FlagsInfo.Write(writer);
			writer.Write((uint)DataStreamCachePosition);
			writer.Write((uint)DataStreamCacheSize);
			writer.Write((uint)DataStreamCacheCharacterCount);
			writer.Write((uint)LemMode);
			writer.Write(hashParameters);
			this.tableName.Write(writer);
			int tableColumnInfoCount = Columns.Count;
			writer.Write((ushort)tableColumnInfoCount);
			if (FlagsInfo.LoadCSPName)
				this.cryptographicServiceProviderName.Write(writer);
			if (FlagsInfo.LoadEntryId)
				this.entryIdName.Write(writer);
			for (int i = 0; i < tableColumnInfoCount; i++) {
				XlsTableColumnInfo columnInfo = Columns[i];
				columnInfo.Write(writer);
			}
			if (FlagsInfo.LoadPresentIdDeleted)
				WriteListInt(writer, Feature11RgSharepointIdDel);
			if (FlagsInfo.LoadPresentIdChanged)
				WriteListInt(writer, Feature11RgSharepointIdChange);
			if (FlagsInfo.LoadPresentCellInvalid)
				WriteFeature11RgInvalidCells(writer);
		}
		void WriteListInt(BinaryWriter writer, List<int> array) {
			short arrayCount = (short)array.Count;
			writer.Write(arrayCount);
			for (short i = 0; i < arrayCount; i++)
				writer.Write((uint)array[i]);
		}
		void WriteFeature11RgInvalidCells(BinaryWriter writer) {
			short arrayCount = (short)Feature11RgInvalidCells.Count;
			writer.Write(arrayCount);
			for (short i = 0; i < arrayCount; i++)
				Feature11RgInvalidCells[i].Write(writer);
		}
		public short GetSize() {
			int result = fixedPartSize + this.tableName.Length;
			if (FlagsInfo.LoadCSPName)
				result += this.cryptographicServiceProviderName.Length;
			if (FlagsInfo.LoadEntryId)
				result += this.entryIdName.Length;
			if (FlagsInfo.LoadPresentIdDeleted)
				result += 2 + Feature11RgSharepointIdDel.Count * 4;
			if (FlagsInfo.LoadPresentIdChanged)
				result += 2 + Feature11RgSharepointIdChange.Count * 4;
			if (FlagsInfo.LoadPresentCellInvalid)
				result += 2 + Feature11RgInvalidCells.Count * XlsFeature11CellStruct.Size;
			int count = Columns.Count;
			for(int i = 0; i < count; i++)
				result += Columns[i].GetSize(); 
			return (short)result;
		}
		#region CreateTable
		protected internal Table CreateTable(XlsContentBuilder contentBuilder) {
			CellRange cellRange = new CellRange(currentSheet, cellRangeInfo.First, cellRangeInfo.Last);
			Table result = new Table(currentSheet, TableName, cellRange);
			contentBuilder.ActiveTable = result;
			result.BeginUpdate();
			try {
				result.TableType = tableType;
				AssignColumns(contentBuilder);
				if (result.Columns.Count > 0) {
					result.SetHasHeadersRow(HasHeadersRow);
					result.SetHasTotalsRow(HasTotalsRow);
				}
				if (!FlagsInfo.HasAutoFilter)
					result.AutoFilter.SetRangeCore(null);
				result.InsertRowProperty = FlagsInfo.ShowInsertRow;
				result.InsertRowShift = FlagsInfo.InsertRowInsCells;
				result.Published = FlagsInfo.Published;
			} 
			finally {
				result.EndUpdate();
			}
			return result;
		}
		void AssignColumns(XlsContentBuilder contentBuilder) {
			if (Columns.Count > 0) {
				int count = Columns.Count;
				for (int i = 0; i < count; i++) {
					XlsTableColumnInfo xlsColumnInfo = Columns[i];
					TableColumn columnInfo = xlsColumnInfo.CreateTableColumnInfo(contentBuilder);
					contentBuilder.ActiveTable.Columns.Add(columnInfo);
					xlsColumnInfo.RestoreColumnFormula(columnInfo);
				}
				CheckColumnCaptions(contentBuilder.ActiveTable);
			}
		}
		void CheckColumnCaptions(Table table) {
			if(FlagsInfo.SingleCell)
				return;
			if(!table.HasHeadersRow)
				return;
			int count = table.Columns.Count;
			CellPosition topLeft = table.Range.TopLeft;
			Worksheet sheet = table.Worksheet;
			for(int i = 0; i < count; i++) {
				TableColumn columnInfo = table.Columns[i];
				if(string.IsNullOrEmpty(columnInfo.Name))
					continue;
				ICell cell = sheet.TryGetCell(topLeft.Column + i, topLeft.Row) as ICell;
				if(cell == null) 
					continue;
				VariantValue textValue = cell.Value.ToText(sheet.DataContext);
				if(textValue.InlineTextValue != columnInfo.Name)
					ChangeCellValue(cell, columnInfo.Name);
			}
		}
		void ChangeCellValue(ICell cell, string value) {
			DocumentModel workbook = cell.Worksheet.Workbook;
			bool suppressCellValueAssignment = workbook.SuppressCellValueAssignment;
			workbook.SuppressCellValueAssignment = false;
			cell.AssignValueCore(value);
			workbook.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		#endregion
		#region AssignThisProperties
		void AssignThisProperties(Table table, ExportXlsStyleSheet styleSheet) {
			IdList = styleSheet.TableCount + 1;
			TableType = table.TableType;
			HasHeadersRow = table.HasHeadersRow;
			HasTotalsRow = table.HasTotalsRow;
			bool hasAutoFilter = table.AutoFilter.Enabled;
			FlagsInfo.HasAutoFilter = hasAutoFilter;
			FlagsInfo.PersistAutoFilter = hasAutoFilter;
			FlagsInfo.ApplyAutoFilter = hasAutoFilter;
			FlagsInfo.ShowInsertRow = table.InsertRowProperty;
			FlagsInfo.InsertRowInsCells = table.InsertRowShift;
			FlagsInfo.Published = table.Published;
			FlagsInfo.ShowTotalRow = table.HasTotalsRow;
			TableName = table.Name;
			AssignColumns(table.Columns, styleSheet);
			styleSheet.TableCount++;
			IdFieldNext = table.Columns.Count + 1;
		}
		void AssignColumns(TableColumnInfoCollection columns, ExportXlsStyleSheet styleSheet) {
			if (columns.Count > 0) {
				int count = columns.Count;
				for (int i = 0; i < count; i++) {
					TableColumn columnInfo = columns[i];
					XlsTableColumnInfo xlsColumnInfo = XlsTableColumnInfo.FromTableColumnInfo(i, columnInfo, this, styleSheet);
					Columns.Add(xlsColumnInfo);
				}
			}
		}
		#endregion
	}
	#endregion
	#region XlsTableFlagsInfo // 32 bits
	public class XlsTableFlagsInfo {
		#region Fields
		const uint MaskHasAutoFilter = 0x00000002;
		const uint MaskPersistAutoFilter = 0x00000004;
		const uint MaskShowInsertRow = 0x00000008;
		const uint MaskInsertRowInsCells = 0x00000010;
		const uint MaskLoadPresentIdDeleted = 0x00000020;
		const uint MaskShowTotalRow = 0x00000040;
		const uint MaskNeedsCommit = 0x00000100;
		const uint MaskSingleCell = 0x00000200;
		const uint MaskApplyAutoFilter = 0x00000800;
		const uint MaskForceInsertToBeVisible = 0x00001000;
		const uint MaskCompressedXml = 0x00002000;
		const uint MaskLoadCSPName = 0x00004000;
		const uint MaskLoadPresentIdChanged = 0x00008000;
		const uint MaskVersionXL = 0x000F0000;  
		const uint MaskLoadEntryId = 0x00100000;
		const uint MaskLoadPresentCellInvalid = 0x00200000;
		const uint MaskGoodRupBuild = 0x00400000;
		const uint MaskPublished = 0x01000000;
		uint packedValues;
		#endregion
		public XlsTableFlagsInfo() {
			VersionXL = XlsDefs.DefaultVersionXL;
			GoodRupBuild = true;
		}
		#region Properties
		protected internal bool HasAutoFilter { get { return GetBooleanValue(MaskHasAutoFilter); } set { SetBooleanValue(MaskHasAutoFilter, value); } }
		protected internal bool PersistAutoFilter { get { return GetBooleanValue(MaskPersistAutoFilter); } set { SetBooleanValue(MaskPersistAutoFilter, value); } }
		protected internal bool ShowInsertRow { get { return GetBooleanValue(MaskShowInsertRow); } set { SetBooleanValue(MaskShowInsertRow, value); } }
		protected internal bool InsertRowInsCells { get { return GetBooleanValue(MaskInsertRowInsCells); } set { SetBooleanValue(MaskInsertRowInsCells, value); } }
		protected internal bool LoadPresentIdDeleted { get { return GetBooleanValue(MaskLoadPresentIdDeleted); } set { SetBooleanValue(MaskLoadPresentIdDeleted, value); } }
		protected internal bool ShowTotalRow { get { return GetBooleanValue(MaskShowTotalRow); } set { SetBooleanValue(MaskShowTotalRow, value); } }
		protected internal bool NeedsCommit { get { return GetBooleanValue(MaskNeedsCommit); } set { SetBooleanValue(MaskNeedsCommit, value); } }
		protected internal bool SingleCell { get { return GetBooleanValue(MaskSingleCell); } set { SetBooleanValue(MaskSingleCell, value); } }
		protected internal bool ApplyAutoFilter { get { return GetBooleanValue(MaskApplyAutoFilter); } set { SetBooleanValue(MaskApplyAutoFilter, value); } }
		protected internal bool ForceInsertToBeVisible { get { return GetBooleanValue(MaskForceInsertToBeVisible); } set { SetBooleanValue(MaskForceInsertToBeVisible, value); } }
		protected internal bool CompressedXml { get { return GetBooleanValue(MaskCompressedXml); } set { SetBooleanValue(MaskCompressedXml, value); } }
		protected internal bool LoadCSPName { get { return GetBooleanValue(MaskLoadCSPName); } set { SetBooleanValue(MaskLoadCSPName, value); } }
		protected internal bool LoadPresentIdChanged { get { return GetBooleanValue(MaskLoadPresentIdChanged); } set { SetBooleanValue(MaskLoadPresentIdChanged, value); } }
		public int VersionXL {
			get { return (int)((packedValues & MaskVersionXL) >> 16); }
			set {
				packedValues &= ~MaskVersionXL;
				packedValues |= ((uint)value << 16) & MaskVersionXL;
			}
		}
		protected internal bool LoadEntryId { get { return GetBooleanValue(MaskLoadEntryId); } set { SetBooleanValue(MaskLoadEntryId, value); } }
		protected internal bool LoadPresentCellInvalid { get { return GetBooleanValue(MaskLoadPresentCellInvalid); } set { SetBooleanValue(MaskLoadPresentCellInvalid, value); } }
		protected internal bool GoodRupBuild { get { return GetBooleanValue(MaskGoodRupBuild); } set { SetBooleanValue(MaskGoodRupBuild, value); } }
		protected internal bool Published { get { return GetBooleanValue(MaskPublished); } set { SetBooleanValue(MaskPublished, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		public void Read(BinaryReader reader) {
			packedValues = reader.ReadUInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(packedValues);
		}
	}
	#endregion
	#region XlsFeature11CellStruct
	public class XlsFeature11CellStruct {
		#region Static Members
		public static XlsFeature11CellStruct FromStream(BinaryReader reader) {
			XlsFeature11CellStruct result = new XlsFeature11CellStruct();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		internal const short Size = 8;
		int idRow;
		int idField;
		#endregion
		#region Properties
		protected internal int IdRow { get { return idRow; } set { idRow = value; } }
		protected internal int IdField { get { return idField; } set { idField = value; } }
		#endregion
		void Read(BinaryReader reader) {
			IdRow = reader.ReadInt32();
			IdField = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((uint)IdRow);
			writer.Write((uint)IdField);
		}
	}
	#endregion
	#region XlsLemMode
	public enum XlsLemMode {
		Normal = 0,
		RefreshCopy = 1,
		RefreshCache = 2,
		RefreshCacheUndo = 3,
		RefreshLoaded = 4,
		RefreshTemplate = 5,
		RefreshRefresh = 6,
		NoInsertRowsPrequired = 7,
		NoInsertRowsBasedDataProvider = 8,
		RefreshLoadDiscarded = 9,
		RefreshLoadHashValidation = 10,
		NoEditSPModView = 11
	}
	#endregion
}
