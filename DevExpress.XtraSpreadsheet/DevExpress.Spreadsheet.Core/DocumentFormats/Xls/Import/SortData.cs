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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Globalization;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsSortParent
	public enum XlsSortParent {
		Worksheet = 0,
		Table = 1,
		AutoFilter = 2,
		QueryTable = 3
	}
	#endregion
	#region XlsCommandSortData
	public class XlsCommandSortData : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x0895, 
			0x087f  
		};
		#endregion
		XlsSortDataInfo sortDataInfo;
		public XlsSortDataInfo SortDataInfo { get { return sortDataInfo; } set { sortDataInfo = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Sheet) {
				FutureRecordHeader header = FutureRecordHeader.FromStream(reader);
				int recordSize = Size - header.GetSize();
				using (XlsCommandStream stream = new XlsCommandStream(contentBuilder, reader, typeCodes, recordSize)) {
					using (BinaryReader sortDataReader = new BinaryReader(stream))
						SortDataInfo = XlsSortDataInfo.FromStream(sortDataReader);
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			sortDataInfo.ApplyContent(contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSortData();
		}
	}
	#endregion
	#region XlsSortDataInfo
	public class XlsSortDataInfo {
		#region Static Members
		public static XlsSortDataInfo FromStream(BinaryReader reader) {
			XlsSortDataInfo result = new XlsSortDataInfo();
			result.Read(reader);
			return result;
		}
		public static XlsSortDataInfo CreateForSheetExport(SortState sortState) {
			XlsSortDataInfo result = new XlsSortDataInfo();
			result.AssignThisProperties(sortState);
			return result;
		}
		public static XlsSortDataInfo CreateForTableExport(SortState sortState, bool hasHeaderRow, int tableId) {
			XlsSortDataInfo result = new XlsSortDataInfo();
			result.AssignThisProperties(sortState, hasHeaderRow, tableId);
			return result;
		}
		#endregion
		#region Fields
		const short fixedSize = 26;
		const short typeCode = 0x0895;
		const ushort parentOffset = 3;
		const uint parentMask = 0x38;
		List<XlsSortCondition12> conditionsList = new List<XlsSortCondition12>();
		RFX rfx = new RFX();
		#endregion
		#region Properties
		public List<XlsSortCondition12> ConditionsList { get { return conditionsList; } }
		public bool SortByColumns { get; set; }
		public bool CaseSensetive { get; set; }
		public bool SortMethod { get; set; }
		public XlsSortParent Parent { get; set; }
		public int ParentId { get; set; }
		public CellRangeInfo CellRangeInfo { get { return rfx.CellRangeInfo; } 
			set {
				Guard.ArgumentNotNull(value, "cellRangeInfo");
				rfx.CellRangeInfo = value;
			} 
		}
		#endregion
		#region Read & Write
		public void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			SortByColumns = (bitwiseField & 0x0001) != 0;
			CaseSensetive = (bitwiseField & 0x0002) != 0;
			SortMethod = (bitwiseField & 0x0004) != 0;
			Parent = (XlsSortParent)((bitwiseField & parentMask) >> parentOffset);
			rfx = RFX.FromStream(reader);
			int conditionsCount = reader.ReadInt32();
			ParentId = reader.ReadInt32();
			for (int i = 0; i < conditionsCount; i++)
				conditionsList.Add(XlsSortCondition12.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = typeCode;
			header.Write(writer);
			uint bitwiseField = 0;
			if (SortByColumns)
				bitwiseField |= 0x0001;
			if (CaseSensetive)
				bitwiseField |= 0x0002;
			if (SortMethod)
				bitwiseField |= 0x0004;
			bitwiseField &= ~parentMask;
			bitwiseField |= ((uint)Parent << parentOffset) & parentMask;
			writer.Write((ushort)bitwiseField);
			rfx.Write(writer);
			int conditionsCount = conditionsList.Count;
			writer.Write(conditionsCount);
			writer.Write(ParentId);
			for (int i = 0; i < conditionsCount; i++) {
				writer.Flush();
				conditionsList[i].Write(writer);
			}
		}
		#endregion
		#region ApplyContent
		internal void ApplyContent(XlsContentBuilder contentBuilder) {
			if (ConditionRangesInvalid())
				return;
			if (Parent == XlsSortParent.AutoFilter)
				ApplyContent(contentBuilder.CurrentSheet.AutoFilter.SortState, contentBuilder, true);
			else if (Parent == XlsSortParent.Table || Parent == XlsSortParent.QueryTable) {
				Table table = contentBuilder.ActiveTable;
				ApplyContent(table.AutoFilter.SortState, contentBuilder, table.HasHeadersRow);
			}
		}
		void ApplyContent(SortState sortState, XlsContentBuilder contentBuilder, bool hasHeaderRow) {
			Guard.ArgumentNotNull(sortState, "sortState");
			sortState.BeginUpdate();
			try {
				sortState.CaseSensitive = CaseSensetive;
				sortState.SortByColumns = SortByColumns;
				sortState.SortMethod = SortMethod ? Model.SortMethod.Stroke : Model.SortMethod.None; 
				sortState.SortRange = GetCellRange(sortState.Sheet, hasHeaderRow);
			}
			finally {
				sortState.EndUpdate();
			}
			SetupSortConditions(sortState, contentBuilder);
		}
		bool ConditionRangesInvalid() {
			int sortFirstColumn = CellRangeInfo.First.Column;
			int sortLastColumn = CellRangeInfo.Last.Column;
			int count = ConditionsList.Count;
			for (int i = 0; i < count; i++) {
				CellRangeInfo conditionRange = ConditionsList[i].CellRangeInfo;
				int conditionFirstColumn = conditionRange.First.Column;
				int conditionLastColumn = conditionRange.Last.Column;
				if (conditionFirstColumn != conditionLastColumn || conditionFirstColumn < sortFirstColumn || conditionLastColumn > sortLastColumn)
					return true;
			}
			return false;
		}
		CellRange GetCellRange(Worksheet sheet, bool hasHeaderRow) {
			int firstRow = CellRangeInfo.First.Row;
			int firstRowWithHeader = hasHeaderRow ? firstRow + 1 : firstRow;
			CellPosition topLeft = new CellPosition(CellRangeInfo.First.Column, firstRowWithHeader);
			return new CellRange(sheet, topLeft, CellRangeInfo.Last);
		}
		void SetupSortConditions(SortState sortState, XlsContentBuilder contentBuilder) {
			int count = ConditionsList.Count;
			for (int i = 0; i < count; i++) {
				XlsSortCondition12 xlsCondition = ConditionsList[i];
				CellRangeInfo cellRangeInfo = xlsCondition.CellRangeInfo;
				CellRange range = new CellRange(sortState.Sheet, cellRangeInfo.First, cellRangeInfo.Last);
				SortCondition condition = new SortCondition(sortState.Sheet, range);
				xlsCondition.SetupSortCondition(condition, contentBuilder);
				sortState.SortConditions.Add(condition);
			}
		}
		#endregion
		#region AssignThisProperties
		void AssignThisProperties(SortState sortState) {
			Parent = XlsSortParent.AutoFilter;
			AssignThisPropertiesCore(sortState, true);
		}
		void AssignThisProperties(SortState sortState, bool hasHeaderRow, int tableId) {
			Parent = XlsSortParent.Table;
			ParentId = tableId;
			AssignThisPropertiesCore(sortState, hasHeaderRow);
		}
		void AssignThisPropertiesCore(SortState sortState, bool hasHeaderRow) {
			SortByColumns = sortState.SortByColumns;
			CaseSensetive = sortState.CaseSensitive;
			SortMethod = sortState.SortMethod != Model.SortMethod.None;
			CellRangeInfo = GetCellRangeInfo(sortState.SortRange, hasHeaderRow);
			AssignSortConditions(sortState.SortConditions);
		}
		CellRangeInfo GetCellRangeInfo(CellRange sortRange, bool hasHeaderRow) {
			int firstRow = sortRange.TopLeft.Row;
			int firstRowWithHeader = hasHeaderRow ? firstRow - 1 : firstRow;
			CellPosition topLeft = new CellPosition(sortRange.TopLeft.Column, firstRowWithHeader);
			return new CellRangeInfo(topLeft, sortRange.BottomRight);
		}
		void AssignSortConditions(SortConditionCollection conditions) {
			int count = conditions.Count;
			for (int i = 0; i < count; i++) {
				XlsSortCondition12 xlsCondition = new XlsSortCondition12();
				xlsCondition.AssignFromModelCondition(conditions[i]);
				ConditionsList.Add(xlsCondition);
			}
		}
		#endregion
		public int GetSize() {
			int variableSize = 0;
			int count = conditionsList.Count;
			for (int i = 0; i < count; i++)
				variableSize += conditionsList[i].GetSize();
			return fixedSize + variableSize;
		}
	}
	#endregion
	#region XlsSortCondition12
	public class XlsSortCondition12 {
		#region Static Members
		public static XlsSortCondition12 FromStream(BinaryReader reader) {
			XlsSortCondition12 result = new XlsSortCondition12();
			result.Read(reader);
			return result;
		}
		static Dictionary<XlsIconSetType, IconSetType> IconSetTypeTable = XlsAutoFilter12Info.IconSetTypeTable;
		#endregion
		#region Fields
		const short fixedSize = 30;
		const uint sortOnMask = 0x1e;
		int iconId = -1;
		XlsIconSetType iconSetType = XlsIconSetType.None;
		XLUnicodeStringNoCch customListString = new XLUnicodeStringNoCch();
		RFX rfx = new RFX();
		#endregion
		#region Properties
		public bool DescendingOrder { get; set; }
		public XlsFilterFormatType SortOn { get; set; }
		public CellRangeInfo CellRangeInfo {
			get { return rfx.CellRangeInfo; }
			set {
				Guard.ArgumentNotNull(value, "cellRangeInfo");
				rfx.CellRangeInfo = value;
			}
		}
		public int FormatIndex { get; set; }
		public int IconId { get { return iconId; } set { iconId = value; } }
		public XlsIconSetType IconType { get { return iconSetType; } set { iconSetType = value; } }
		public string CustomList { get { return customListString.Value; } set { customListString.Value = value; } }
		#endregion
		#region Read & Write
		public void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			DescendingOrder = (bitwiseField & 0x0001) != 0;
			SortOn = (XlsFilterFormatType)((bitwiseField & sortOnMask) >> 1);
			rfx = RFX.FromStream(reader);
			ReadConditionDataValue(reader);
			int charCount = reader.ReadInt32();
			if (charCount > 0)
				customListString = XLUnicodeStringNoCch.FromStream(reader, charCount);
		}
		void ReadConditionDataValue(BinaryReader reader) {
			if (SortOn == XlsFilterFormatType.CellIcon) {
				IconType = (XlsIconSetType)reader.ReadInt32();
				IconId = reader.ReadInt32();
			}
			else if (SortOn == XlsFilterFormatType.CellFont || SortOn == XlsFilterFormatType.CellColor) {
				FormatIndex = reader.ReadInt32();
				reader.ReadInt32();
			}
			else
				reader.ReadBytes(8);
		}
		public void Write(BinaryWriter writer) {
			uint bitwiseField = 0;
			if (DescendingOrder)
				bitwiseField |= 0x0001;
			bitwiseField &= ~sortOnMask;
			bitwiseField |= ((uint)SortOn << 1) & sortOnMask;
			writer.Write((ushort)bitwiseField);
			rfx.Write(writer);
			WriteConditionDataValue(writer);
			int charCount = CustomList.Length;
			writer.Write(charCount);
			if (charCount > 0)
				customListString.Write(writer);
		}
		void WriteConditionDataValue(BinaryWriter writer) {
			if (SortOn == XlsFilterFormatType.CellIcon) {
				writer.Write((int)IconType);
				writer.Write(IconId);
			}
			else if (SortOn == XlsFilterFormatType.CellFont || SortOn == XlsFilterFormatType.CellColor) {
				writer.Write(FormatIndex);
				writer.Write(0);
			}
			else {
				writer.Write(0);
				writer.Write(0);
			}
		}
		#endregion
		internal void SetupSortCondition(SortCondition condition, XlsContentBuilder contentBuilder) {
			condition.BeginUpdate();
			try {
				condition.Descending = DescendingOrder;
				condition.CustomList = CustomList;
				condition.SortBy = (SortBy)SortOn;
				if (SortOn == XlsFilterFormatType.CellIcon) {
					condition.IconSet = IconSetTypeTable[IconType];
					condition.IconId = IconId;
				}
			}
			finally {
				condition.EndUpdate();
			}
		}
		internal void AssignFromModelCondition(SortCondition condition) {
			DescendingOrder = condition.Descending;
			CustomList = condition.CustomList;
			CellRange range = condition.SortReference;
			CellRangeInfo = new Model.CellRangeInfo(range.TopLeft, range.BottomRight);
			SortBy sortBy = condition.SortBy;
			if (sortBy == SortBy.Icon) {
				IconType = GetIconSetType(condition.IconSet);
				IconId = condition.IconId;
				SortOn = XlsFilterFormatType.CellIcon;
			}
		}
		XlsIconSetType GetIconSetType(IconSetType iconType) {
			foreach (XlsIconSetType key in IconSetTypeTable.Keys) 
				if (IconSetTypeTable[key] == iconType)
					return key;
			return XlsIconSetType.None;
		}
		public int GetSize() {
			int variableSize = String.IsNullOrEmpty(CustomList) ? 0 : customListString.Length;
			return fixedSize + variableSize;
		}
	}
	#endregion
	#region RFX
	public class RFX {
		#region Static Members
		public static RFX FromStream(BinaryReader reader) {
			RFX result = new RFX();
			result.Read(reader);
			return result;
		}
		#endregion
		internal const short Size = 16;
		public CellRangeInfo CellRangeInfo { get; set; }
		public void Read(BinaryReader reader) {
			int rowFirst = reader.ReadInt32();
			int rowLast = reader.ReadInt32();
			int columnFirst = reader.ReadInt32();
			int columnLast = reader.ReadInt32();
			CreateCellRangeInfo(rowFirst, rowLast, columnFirst, columnLast);
		}
		void CreateCellRangeInfo(int rowFirst, int rowLast, int columnFirst, int columnLast) {
			if (rowLast < rowFirst || columnLast < columnFirst)
				throw new ArgumentException("Invalid RFX structure");
			CellPosition topLeft = new CellPosition(columnFirst, rowFirst);
			CellPosition bottomRight = new CellPosition(columnLast, rowLast);
			CellRangeInfo = new CellRangeInfo(topLeft, bottomRight);
		}
		public void Write(BinaryWriter writer) {
			if (CellRangeInfo != null) {
				writer.Write(CellRangeInfo.First.Row);
				writer.Write(CellRangeInfo.Last.Row);
				writer.Write(CellRangeInfo.First.Column);
				writer.Write(CellRangeInfo.Last.Column);
			}
			else
				writer.Write(new byte[Size]);
		}
	}
	#endregion
}
