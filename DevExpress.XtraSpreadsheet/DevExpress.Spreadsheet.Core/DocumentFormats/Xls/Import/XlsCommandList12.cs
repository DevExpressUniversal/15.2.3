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
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandList12
	public class XlsCommandList12 : XlsCommandBase {
		internal const short FixedPartSize = 18;
		#region Properies
		public int IdList { get; set; }
		public IList12Data Data { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			List12DataType dataType = (List12DataType)reader.ReadInt16();
			IdList = reader.ReadInt32();
			Data = List12DataBuilder.CreateForImport(dataType, Size - FixedPartSize);
			Data.Read(reader);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Data.ApplyContent(contentBuilder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandList12));
			header.Write(writer);
			writer.Write((short)Data.DataType);
			writer.Write((uint)IdList);
			Data.Write(writer);
		}
		protected override short GetSize() {
			return (short)(FixedPartSize + Data.GetSize());
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandList12();
		}
	}
	#endregion 
	#region List12DataType
	public enum List12DataType {
		List12BlockLevel = 0,
		List12TableStyleClientInfo = 1,
		List12DisplayName = 2
	}
	#endregion
	#region IList12Data
	public interface IList12Data {
		List12DataType DataType { get; }
		void Read(XlsReader reader);
		void Write(BinaryWriter writer);
		void ApplyContent(XlsContentBuilder contentBuilder);
		void AssignProperties(Table table, ExportXlsStyleSheet styleSheet);
		short GetSize();
	}
	#endregion
	#region List12DataBuilder
	public static class List12DataBuilder {
		static IList12Data CreateCore(List12DataType dataType) {
			if (dataType == List12DataType.List12TableStyleClientInfo)
				return new List12TableStyleClientInfo();
			if (dataType == List12DataType.List12DisplayName)
				return new List12DisplayName();
			return new List12BlockLevel();
		}
		public static IList12Data CreateForImport(List12DataType dataType, int currentBlockSize) {
			IList12Data result = CreateCore(dataType);
			List12BlockLevel list12BlockLevel = result as List12BlockLevel;
			if (list12BlockLevel != null)
				list12BlockLevel.CurrentBlockSize = currentBlockSize;
			return result;
		}
		public static IList12Data CreateForExport(List12DataType dataType, Table table, ExportXlsStyleSheet styleSheet) {
			IList12Data result = CreateCore(dataType);
			result.AssignProperties(table, styleSheet);
			return result;
		}
	}
	#endregion
	#region List12BlockLevel
	public class List12BlockLevel : IList12Data {
		#region Fields
		const short basePartSize = 36;
		internal const int DefaultValue = -1;
		int headerRowCellStyleId = DefaultValue;
		int dataCellStyleId = DefaultValue;
		int totalsRowCellStyleId = DefaultValue;
		DxfN12ListInfo headerRowDxfInfo;
		DxfN12ListInfo dataDxfInfo;
		DxfN12ListInfo totalsRowDxfInfo;
		DxfN12ListInfo dataDxfBorderInfo;
		DxfN12ListInfo headerRowDxfBorderInfo;
		DxfN12ListInfo totalsRowDxfBorderInfo;
		XLUnicodeString headerRowCellStyleName = new XLUnicodeString();
		XLUnicodeString dataCellStyleName = new XLUnicodeString();
		XLUnicodeString totalsRowCellStyleName = new XLUnicodeString();
		#endregion
		public List12BlockLevel() {
		}
		#region Properties 
		public int CurrentBlockSize { get; set; }
		public int HeaderRowCellStyleId { get { return headerRowCellStyleId; } set { headerRowCellStyleId = value; } }
		public int DataCellStyleId { get { return dataCellStyleId; } set { dataCellStyleId = value; } }
		public int TotalsRowCellStyleId { get { return totalsRowCellStyleId; } set { totalsRowCellStyleId = value; } }
		public DxfN12ListInfo HeaderRowDxfInfo { get { return headerRowDxfInfo; } set { headerRowDxfInfo = value; } }
		public DxfN12ListInfo DataDxfInfo { get { return dataDxfInfo; } set { dataDxfInfo = value; } }
		public DxfN12ListInfo TotalsRowDxfInfo { get { return totalsRowDxfInfo; } set { totalsRowDxfInfo = value; } }
		public DxfN12ListInfo DataDxfBorderInfo { get { return dataDxfBorderInfo; } set { dataDxfBorderInfo = value; } }
		public DxfN12ListInfo HeaderRowDxfBorderInfo { get { return headerRowDxfBorderInfo; } set { headerRowDxfBorderInfo = value; } }
		public DxfN12ListInfo TotalsRowDxfBorderInfo { get { return totalsRowDxfBorderInfo; } set { totalsRowDxfBorderInfo = value; } }
		public XLUnicodeString HeaderRowCellStyleName { get { return headerRowCellStyleName; } set { headerRowCellStyleName = value; } }
		public XLUnicodeString DataCellStyleName { get { return dataCellStyleName; } set { dataCellStyleName = value; } }
		public XLUnicodeString TotalsRowCellStyleName { get { return totalsRowCellStyleName; } set { totalsRowCellStyleName = value; } }
		#endregion
		#region IList12Data Members
		public List12DataType DataType { get { return List12DataType.List12BlockLevel; }  }
		#region Read
		public void Read(XlsReader reader) {
			int headerRowDxfSize = (int)reader.ReadUInt32();
			HeaderRowCellStyleId = (int)reader.ReadUInt32();
			int dataDxfSize = (int)reader.ReadUInt32();
			DataCellStyleId = (int)reader.ReadUInt32();
			int totalRowDxfSize = (int)reader.ReadUInt32();
			TotalsRowCellStyleId = (int)reader.ReadUInt32();
			int dataDxfBorderSize = (int)reader.ReadUInt32();
			int headerRowDxfBorderSize = (int)reader.ReadUInt32();
			int totalRowDxfBorderSize = (int)reader.ReadUInt32();
			if (headerRowDxfSize > 0)
				HeaderRowDxfInfo = ReadDxfN12ListInfo(reader, headerRowDxfSize);
			if (dataDxfSize > 0)
				DataDxfInfo = ReadDxfN12ListInfo(reader, dataDxfSize);
			if (totalRowDxfSize > 0)
				TotalsRowDxfInfo = ReadDxfN12ListInfo(reader, totalRowDxfSize);
			if (dataDxfBorderSize > 0)
				DataDxfBorderInfo = ReadDxfN12ListInfo(reader, dataDxfBorderSize);
			if (headerRowDxfBorderSize > 0)
				HeaderRowDxfBorderInfo = ReadDxfN12ListInfo(reader, headerRowDxfBorderSize);
			if (totalRowDxfBorderSize > 0)
				TotalsRowDxfBorderInfo = ReadDxfN12ListInfo(reader, totalRowDxfBorderSize);
			if (HeaderRowCellStyleId != DefaultValue)
				HeaderRowCellStyleName = XLUnicodeString.FromStream(reader);
			if (DataCellStyleId != DefaultValue)
				DataCellStyleName = XLUnicodeString.FromStream(reader);
			if (TotalsRowCellStyleId != DefaultValue)
				TotalsRowCellStyleName = XLUnicodeString.FromStream(reader);
		}
		DxfN12ListInfo ReadDxfN12ListInfo(XlsReader reader, int infoSize) {
			DxfN12ListInfo result = DxfN12ListInfo.FromStream(reader, infoSize, CurrentBlockSize);
			CurrentBlockSize += result.GetSize();
			return result;
		}
		#endregion
		#region Write
		public void Write(BinaryWriter writer) {
			WriteDxfN12ListSize(writer, HeaderRowDxfInfo);
			writer.Write(HeaderRowCellStyleId);
			WriteDxfN12ListSize(writer, DataDxfInfo);
			writer.Write(DataCellStyleId);
			WriteDxfN12ListSize(writer, TotalsRowDxfInfo);
			writer.Write(TotalsRowCellStyleId);
			WriteDxfN12ListSize(writer, DataDxfBorderInfo);
			WriteDxfN12ListSize(writer, HeaderRowDxfBorderInfo);
			WriteDxfN12ListSize(writer, TotalsRowDxfBorderInfo);
			WriteDxfN12ListInfo(writer, HeaderRowDxfInfo);
			WriteDxfN12ListInfo(writer, DataDxfInfo);
			WriteDxfN12ListInfo(writer, TotalsRowDxfInfo);
			WriteDxfN12ListInfo(writer, DataDxfBorderInfo);
			WriteDxfN12ListInfo(writer, HeaderRowDxfBorderInfo);
			WriteDxfN12ListInfo(writer, TotalsRowDxfBorderInfo);
			WriteCellStyleName(writer, HeaderRowCellStyleId, HeaderRowCellStyleName);
			WriteCellStyleName(writer, DataCellStyleId, DataCellStyleName);
			WriteCellStyleName(writer, TotalsRowCellStyleId, TotalsRowCellStyleName);
		}
		void WriteDxfN12ListSize(BinaryWriter writer, DxfN12ListInfo info) {
			uint size = info != null ? (uint)info.GetSize() : 0;
			writer.Write((uint)size);
		}
		void WriteDxfN12ListInfo(BinaryWriter writer, DxfN12ListInfo info) {
			if (info != null)
				info.Write(writer);
		}
		void WriteCellStyleName(BinaryWriter writer, int cellStyleId, XLUnicodeString unicodeCellStyleName) {
			if (cellStyleId != DefaultValue && !String.IsNullOrEmpty(unicodeCellStyleName.Value))
				unicodeCellStyleName.Write(writer);
		}
		#endregion
		#region ApplyContent
		public void ApplyContent(XlsContentBuilder contentBuilder) {
			ApplyDxfInfoContent(contentBuilder, HeaderRowDxfInfo, Table.HeaderRowIndex);
			ApplyDxfInfoContent(contentBuilder, DataDxfInfo, Table.DataIndex);
			ApplyDxfInfoContent(contentBuilder, TotalsRowDxfInfo, Table.TotalsRowIndex);
			ApplyDxfBorderInfoContent(contentBuilder, HeaderRowDxfBorderInfo, Table.HeaderRowIndex); 
			ApplyDxfBorderInfoContent(contentBuilder, DataDxfBorderInfo, Table.DataIndex);
			ApplyDxfBorderInfoContent(contentBuilder, TotalsRowDxfBorderInfo, Table.TotalsRowIndex);
			ApplyCellStyleContent(contentBuilder, HeaderRowCellStyleId, HeaderRowCellStyleName, Table.HeaderRowIndex);
			ApplyCellStyleContent(contentBuilder, DataCellStyleId, DataCellStyleName, Table.DataIndex);
			ApplyCellStyleContent(contentBuilder, TotalsRowCellStyleId, TotalsRowCellStyleName, Table.TotalsRowIndex);
		}
		int GetDifferentialFormatIndex(XlsContentBuilder contentBuilder, DxfN12ListInfo info) {
			DifferentialFormat format = info.GetDifferentialFormat(contentBuilder);
			return contentBuilder.DocumentModel.Cache.CellFormatCache.AddItem(format);
		}
		void ApplyDxfInfoContent(XlsContentBuilder contentBuilder, DxfN12ListInfo info, int elementIndex) {
			if (info != null) 
				contentBuilder.ActiveTable.AssignDifferentialFormatIndex(elementIndex, GetDifferentialFormatIndex(contentBuilder, info));
		}
		void ApplyDxfBorderInfoContent(XlsContentBuilder contentBuilder, DxfN12ListInfo info, int elementIndex) {
			if (info != null)
				contentBuilder.ActiveTable.AssignBorderFormatIndex(elementIndex, GetDifferentialFormatIndex(contentBuilder, info));
		}
		int GetFormatIndex(CellStyleBase cellStyle) {
			DocumentModel documentModel = cellStyle.DocumentModel;
			CellFormat format = new CellFormat(documentModel);
			format.Style = cellStyle;
			return documentModel.Cache.CellFormatCache.AddItem(format);
		}
		void ApplyCellStyleContent(XlsContentBuilder contentBuilder, int cellStyleId, XLUnicodeString unicodeCellStyleName, int elementIndex) {
			if (cellStyleId != DefaultValue && !String.IsNullOrEmpty(unicodeCellStyleName.Value)) {
				XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
				int index = styleSheet.XFCellStyleIndexTable[cellStyleId];
				CellStyleBase cellStyle = styleSheet.CellStyles[index];
				int formatIndex = GetFormatIndex(cellStyle);
				Table table = contentBuilder.ActiveTable;
				table.AssignCellFormatIndex(elementIndex, formatIndex);
				TableCellStyleApplyFlagsInfo options = table.ApplyFlagsInfo.Clone();
				options.SetApplyCellStyle(elementIndex, true);
				table.AssignApplyFlagsIndex(options.PackedValues);
			}
		}
		#endregion 
		#region AssignProperties
		public void AssignProperties(Table table, ExportXlsStyleSheet styleSheet) {
			if (table.HeaderRow.ApplyDifferentialFormat)
				HeaderRowDxfInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetDifferentialFormat(Table.HeaderRowIndex));
			if (table.Data.ApplyDifferentialFormat)
				DataDxfInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetDifferentialFormat(Table.DataIndex));
			if (table.TotalsRow.ApplyDifferentialFormat)
				TotalsRowDxfInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetDifferentialFormat(Table.TotalsRowIndex));
			if (table.HeaderRow.ApplyBorderFormat)
				HeaderRowDxfBorderInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetBorderFormat(Table.HeaderRowIndex));
			if (table.Data.ApplyBorderFormat)
				DataDxfBorderInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetBorderFormat(Table.DataIndex));
			if (table.TotalsRow.ApplyBorderFormat)
				TotalsRowDxfBorderInfo = DxfN12ListInfo.FromDifferentialFormat(table.GetBorderFormat(Table.TotalsRowIndex));
			if (table.HeaderRow.ApplyCellStyle) {
				CellFormat cellFormat = table.GetCellFormat(Table.HeaderRowIndex);
				HeaderRowCellStyleId = styleSheet.CellStyleIndexTable[cellFormat.StyleIndex];
				HeaderRowCellStyleName = CreateUnicodeCellStyleName(cellFormat);
			}
			if (table.Data.ApplyCellStyle) {
				CellFormat cellFormat = table.GetCellFormat(Table.DataIndex);
				DataCellStyleId = styleSheet.CellStyleIndexTable[cellFormat.StyleIndex];
				DataCellStyleName = CreateUnicodeCellStyleName(cellFormat);
			}
			if (table.TotalsRow.ApplyCellStyle) {
				CellFormat cellFormat = table.GetCellFormat(Table.TotalsRowIndex);
				TotalsRowCellStyleId = styleSheet.CellStyleIndexTable[cellFormat.StyleIndex];
				TotalsRowCellStyleName = CreateUnicodeCellStyleName(cellFormat);
			}
		}
		XLUnicodeString CreateUnicodeCellStyleName(CellFormat format) {
			XLUnicodeString result = new XLUnicodeString();
			result.Value = format.Style.Name;
			return result;
		}
		#endregion
		#region GetSize
		public short GetSize() {
			int result = basePartSize;
			result += GetSize(HeaderRowDxfInfo);
			result += GetSize(DataDxfInfo);
			result += GetSize(TotalsRowDxfInfo);
			result += GetSize(HeaderRowDxfBorderInfo);
			result += GetSize(DataDxfBorderInfo);
			result += GetSize(TotalsRowDxfBorderInfo);
			result += GetSize(HeaderRowCellStyleId, HeaderRowCellStyleName);
			result += GetSize(DataCellStyleId, DataCellStyleName);
			result += GetSize(TotalsRowCellStyleId, TotalsRowCellStyleName);
			return (short)result;
		}
		int GetSize(DxfN12ListInfo info) {
			if (info != null)
				return info.GetSize();
			return 0;
		}
		int GetSize(int cellStyleId, XLUnicodeString unicodeCellStyleName) {
			if (cellStyleId != DefaultValue && !String.IsNullOrEmpty(unicodeCellStyleName.Value))
				return unicodeCellStyleName.Length;
			return 0;
		}
		#endregion 
		#endregion
	}
	#endregion
	#region List12TableStyleClientInfo
	public class List12TableStyleClientInfo : IList12Data {
		#region Fields
		const short fixedPartSize = 2;
		const ushort MaskShowFirstColumn = 0x0001;
		const ushort MaskShowLastColumn = 0x0002;
		const ushort MaskShowRowStripes = 0x0004;
		const ushort MaskShowColumnStripes = 0x0008;
		const ushort MaskDefaultStyle = 0x0040;
		ushort packedValues;
		XLUnicodeString listStyleName = new XLUnicodeString();
		#endregion
		#region Properties
		public bool ShowFirstColumn { get { return GetBooleanVal(MaskShowFirstColumn); } set { SetBooleanVal(MaskShowFirstColumn, value); } }
		public bool ShowLastColumn { get { return GetBooleanVal(MaskShowLastColumn); } set { SetBooleanVal(MaskShowLastColumn, value); } }
		public bool ShowRowStripes { get { return GetBooleanVal(MaskShowRowStripes); } set { SetBooleanVal(MaskShowRowStripes, value); } }
		public bool ShowColumnStripes { get { return GetBooleanVal(MaskShowColumnStripes); } set { SetBooleanVal(MaskShowColumnStripes, value); } }
		public bool DefaultStyle { get { return GetBooleanVal(MaskDefaultStyle); } set { SetBooleanVal(MaskDefaultStyle, value); } }
		public XLUnicodeString ListStyleName { get { return listStyleName; } set { listStyleName = value; } }
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(ushort mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= (ushort)(~mask);
		}
		bool GetBooleanVal(ushort mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region IList12Data Members
		public List12DataType DataType { get { return List12DataType.List12TableStyleClientInfo; }  }
		public void Read(XlsReader reader) {
			packedValues = reader.ReadUInt16();
			ListStyleName = XLUnicodeString.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(packedValues);
			ListStyleName.Write(writer);
		}
		public void ApplyContent(XlsContentBuilder contentBuilder) {
			Table table = contentBuilder.ActiveTable;
			TableInfo info = table.TableInfo.Clone();
			info.ShowFirstColumn = ShowFirstColumn;
			info.ShowLastColumn = ShowLastColumn;
			info.ShowRowStripes = ShowRowStripes;
			info.ShowColumnStripes = ShowColumnStripes;
			contentBuilder.StyleSheet.SetTableStyleName(info, ListStyleName.Value);
			table.AssignTableInfo(info);
		}
		public void AssignProperties(Table table, ExportXlsStyleSheet styleSheet) {
			ShowFirstColumn = table.ShowFirstColumn;
			ShowLastColumn = table.ShowLastColumn;
			ShowRowStripes = table.ShowRowStripes;
			ShowColumnStripes = table.ShowColumnStripes;
			string tableStyleName = table.TableInfo.StyleName;
			ListStyleName.Value = tableStyleName;
			DefaultStyle = table.DocumentModel.StyleSheet.TableStyles.DefaultTableStyle.Name.IsEquals(tableStyleName);
		}
		public short GetSize() {
			return (short)(fixedPartSize + ListStyleName.Length);
		}
		#endregion
	}
	#endregion
	#region List12DisplayName
	public class List12DisplayName : IList12Data {
		#region Fields
		XLUnicodeString displayName = new XLUnicodeString();
		XLUnicodeString comment = new XLUnicodeString();
		#endregion
		#region Properties
		public XLUnicodeString DisplayName { get { return displayName; } set { displayName = value; } }
		public XLUnicodeString Comment { get { return comment; } set { comment = value; } }
		#endregion
		#region IList12Data Members
		public List12DataType DataType { get { return List12DataType.List12DisplayName; } }
		public void Read(XlsReader reader) {
			DisplayName = XLUnicodeString.FromStream(reader);
			Comment = XLUnicodeString.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			DisplayName.Write(writer);
			Comment.Write(writer);
		}
		#region ApplyContent
		public void ApplyContent(XlsContentBuilder contentBuilder) {
			Table table = contentBuilder.ActiveTable;
			ApplyDisplayName(table, DisplayName.Value);
			ApplyComment(table, Comment.Value);
		}
		void ApplyDisplayName(Table table, string name) {
			if (!String.IsNullOrEmpty(name))
				table.SetDisplayNameCore(name);
		}
		void ApplyComment(Table table, string comment) {
			if (String.IsNullOrEmpty(comment))
				return;
			table.BeginUpdate();
			try {
				table.Comment = comment;
			} finally {
				table.EndUpdate();
			}
		}
		#endregion
		public void AssignProperties(Table table, ExportXlsStyleSheet styleSheet) {
			DisplayName.Value = table.Name;
			Comment.Value = table.Comment;
		}
		public short GetSize() {
			return (short)(DisplayName.Length + Comment.Length);
		}
		#endregion
	}
	#endregion
}
