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

using DevExpress.Utils;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotZoneType (enum)
	public enum PivotZoneType {
		TableHeader = 0x1,
		ColumnHeader = 0x2,
		RowHeader = 0x4,
		Data = 0x8,
		GrandTotalColumn = 0x10,
		GrandTotalRow = 0x20,
		TableGrandTotal = 0x30,
	}
	#endregion
	#region PivotFormattingType (enum)
	public enum PivotFormattingType {
		None,
		Grand,
		Subheading1,
		Subheading2,
		Subheading3,
		SubTotal1,
		SubTotal2,
		SubTotal3,
		Blank,
		White,
	}
	#endregion
	#region IPivotZone
	public interface IPivotZone {
		PivotZoneType Type { get; }
		IPivotZoneAxisInfo ColumnAxisInfo { get; }
		IPivotZoneAxisInfo RowAxisInfo { get; }
	}
	#endregion
	#region IPivotZoneAxisInfo
	public interface IPivotZoneAxisInfo {
		PivotFormattingType Formatting { get; }
		bool HasFirstCell { get; }
		bool HasLastCell { get; }
		bool IsVertical { get; }
	}
	#endregion
	#region PivotZone
	public class PivotZone : IPivotZone {
		readonly PivotTable table;
		readonly PivotZoneAxisInfo columnAxisInfo;
		readonly PivotZoneAxisInfo rowAxisInfo;
		PivotZoneType type;
		public PivotZone(PivotTable table, PivotZoneAxisInfo columnAxisInfo, PivotZoneAxisInfo rowAxisInfo) {
			this.table = table;
			this.columnAxisInfo = columnAxisInfo;
			this.rowAxisInfo = rowAxisInfo;
			type = DetectType();
		}
		#region Properties
		public PivotZoneType Type { get { return type; } }
		public PivotZoneAxisInfo ColumnAxisInfo { get { return columnAxisInfo; } }
		public PivotZoneAxisInfo RowAxisInfo { get { return rowAxisInfo; } }
		#endregion
		PivotZoneType DetectType() {
			PivotZoneType result = PivotZoneType.TableHeader;
			if (columnAxisInfo != null) {
				if (rowAxisInfo != null) {
					result = PivotZoneType.Data;
					if (columnAxisInfo.LayoutItem.Type == PivotFieldItemType.Grand)
						result |= PivotZoneType.GrandTotalColumn;
					if (rowAxisInfo.LayoutItem.Type == PivotFieldItemType.Grand)
						result |= PivotZoneType.GrandTotalRow;
				}
				else {
					result = PivotZoneType.ColumnHeader;
					if (columnAxisInfo.LayoutItem.Type == PivotFieldItemType.Grand)
						result |= PivotZoneType.GrandTotalColumn;
				}
			}
			else {
				if (rowAxisInfo != null) {
					result = PivotZoneType.RowHeader;
					if (rowAxisInfo.LayoutItem.Type == PivotFieldItemType.Grand)
						result |= PivotZoneType.GrandTotalRow;
				}
			}
			return result;
		}
		public int GetActiveFieldIndex() {
			return GetActiveFieldInfo().FieldIndex;
		}
		public int GetActiveSortFieldIndex() {
			int fieldIndex = -1;
			if ((this.Type & PivotZoneType.Data) > 0) {
				if (this.RowAxisInfo != null) {
					fieldIndex = this.RowAxisInfo.FieldIndex;
					fieldIndex = LookupNonDataFieldInAxis(fieldIndex, table.CalculationInfo.RowLayoutItemAccessor);
				}
				if (fieldIndex < 0 && this.ColumnAxisInfo != null) {
					fieldIndex = this.ColumnAxisInfo.FieldIndex;
					fieldIndex = LookupNonDataFieldInAxis(fieldIndex, table.CalculationInfo.ColumnLayoutItemAccessor);
				}
			}
			else
				fieldIndex = this.GetActiveFieldIndex();
			return fieldIndex;
		}
		int LookupNonDataFieldInAxis(int fieldIndex, IPivotLayoutItemAccessor layoutAccessor) {
			if (fieldIndex >= 0) {
				int currentIndex = fieldIndex;
				do {
					fieldIndex = layoutAccessor.GetFieldIndex(currentIndex);
					currentIndex--;
				}
				while (fieldIndex < 0 && currentIndex >= 0);
			}
			return fieldIndex;
		}
		public PivotFieldZoneInfo GetActiveFieldInfo() {
			if (Type == PivotZoneType.TableHeader) {
				return PivotFieldZoneInfo.Invalid;
			}
			IPivotLayoutItemAccessor rowLayoutItemAccessor = table.CalculationInfo.RowLayoutItemAccessor;
			IPivotLayoutItemAccessor columnLayoutItemAccessor = table.CalculationInfo.ColumnLayoutItemAccessor;
			if ((Type & PivotZoneType.Data) > 0) {
				IPivotLayoutItem rowLayoutItem = rowAxisInfo.LayoutItem;
				IPivotLayoutItem columnLayoutItem = columnAxisInfo.LayoutItem;
				if (rowLayoutItem.Type == PivotFieldItemType.Blank || columnLayoutItem.Type == PivotFieldItemType.Blank)
					return PivotFieldZoneInfo.Invalid;
				int rowLastFieldindex = rowLayoutItem.PivotFieldItemIndices.Length + rowLayoutItem.RepeatedItemsCount;
				int columnLastFieldindex = columnLayoutItem.PivotFieldItemIndices.Length + columnLayoutItem.RepeatedItemsCount;
				int columnDataPosition = -1;
				int rowDataPosition = -1;
				if (table.DataOnRows)
					rowDataPosition = table.DataPosition;
				else
					columnDataPosition = table.DataPosition;
				bool rowIsFull = rowLastFieldindex > rowDataPosition;
				bool columnIsFull = columnLastFieldindex > columnDataPosition;
				if (rowIsFull && columnIsFull) {
					int dataFieldIndex = System.Math.Max(columnLayoutItem.DataFieldIndex, rowLayoutItem.DataFieldIndex);
					if (table.DataFields.Count > dataFieldIndex)
						return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[dataFieldIndex].FieldIndex, dataFieldIndex);
				}
				if (table.DataFields.Count == 1)
					return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[0].FieldIndex, 0);
				return PivotFieldZoneInfo.Invalid;
			}
			if (Type == PivotZoneType.RowHeader) {
				IPivotLayoutItem rowLayoutItem = rowAxisInfo.LayoutItem;
				if (rowLayoutItem.Type != PivotFieldItemType.Blank)
					if (rowLayoutItemAccessor.HasFields)
						return rowLayoutItemAccessor.GetFieldZoneInfo(rowAxisInfo.FieldIndex, rowLayoutItem.DataFieldIndex);
					else
						return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[rowLayoutItem.DataFieldIndex].FieldIndex, rowLayoutItem.DataFieldIndex);
			}
			if (Type == PivotZoneType.ColumnHeader) {
				IPivotLayoutItem columnLayoutItem = columnAxisInfo.LayoutItem;
				if (columnLayoutItemAccessor.HasFields)
					return columnLayoutItemAccessor.GetFieldZoneInfo(columnAxisInfo.FieldIndex, columnLayoutItem.DataFieldIndex);
				else
					return new PivotFieldZoneInfo(PivotTableAxis.Value, table.DataFields[columnLayoutItem.DataFieldIndex].FieldIndex, columnLayoutItem.DataFieldIndex);
			}
			return PivotFieldZoneInfo.Invalid;
		}
		#region IPivotZone members
		IPivotZoneAxisInfo IPivotZone.ColumnAxisInfo { get { return ColumnAxisInfo; } }
		IPivotZoneAxisInfo IPivotZone.RowAxisInfo { get { return RowAxisInfo; } }
		#endregion
	}
	#endregion
	public struct PivotFieldZoneInfo {
		static readonly PivotFieldZoneInfo invalid = new PivotFieldZoneInfo(PivotTableAxis.None, -1, -1);
		PivotTableAxis axis;
		int fieldIndex;
		int fieldReferenceIndex;
		public static PivotFieldZoneInfo Invalid { get { return invalid; } }
		public PivotFieldZoneInfo(PivotTableAxis axis, int fieldIndex, int fieldReferenceIndex) {
			this.axis = axis;
			this.fieldIndex = fieldIndex;
			this.fieldReferenceIndex = fieldReferenceIndex;
		}
		public PivotTableAxis Axis { get { return axis; } }
		public int FieldIndex { get { return fieldIndex; } }
		public int FieldReferenceIndex { get { return fieldReferenceIndex; } }
	}
	#region PivotZoneViewInfo
	public class PivotZoneViewInfo : IPivotZone {
		public static PivotZoneViewInfo TryCreate(CellPosition position, PivotStyleViewInfo info) {
			int column = position.Column;
			int row = position.Row;
			bool hasFirstSubheadingRow = row == 1 || row == 4;
			CellPosition bottomRight = info.WholeRange.BottomRight;
			bool hasLastColumn = column == bottomRight.Column;
			bool hasLastRow = row == bottomRight.Row;
			bool hasFirstColumn = column == 0;
			bool hasFirstRow = row == 0;
			if (hasFirstColumn) {
				if (hasFirstSubheadingRow) {
					PivotZoneType type = PivotZoneType.RowHeader;
					PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo();
					PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Subheading1, true, false, false);
					return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
				}
				if (hasLastRow) {
					PivotZoneType type = PivotZoneType.GrandTotalRow;
					PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo();
					PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, true, false, false);
					return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
				}
			}
			if (hasLastColumn) {
				if (hasFirstSubheadingRow) {
					PivotZoneType type = PivotZoneType.GrandTotalColumn | PivotZoneType.Data;
					PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, false, false, false);
					PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Subheading1, false, true, false);
					return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
				}
				else if (hasLastRow) {
					PivotZoneType type = PivotZoneType.GrandTotalColumn | PivotZoneType.GrandTotalRow;
					PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, false, true, false);
					PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, false, true, false);
					return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
				}
				else {
					PivotZoneType type = PivotZoneType.GrandTotalColumn;
					PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, false, false, false);
					PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo();
					return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
				}
			}
			if (hasFirstSubheadingRow) {
				PivotZoneType type = PivotZoneType.Data;
				PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo();
				PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Subheading1, false, false, false);
				return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
			}
			else if (hasLastRow) {
				PivotZoneType type = PivotZoneType.GrandTotalRow;
				PivotZoneAxisViewInfo columnAxisInfo = new PivotZoneAxisViewInfo();
				PivotZoneAxisViewInfo rowAxisInfo = new PivotZoneAxisViewInfo(PivotFormattingType.Grand, false, false, false);
				return new PivotZoneViewInfo(type, columnAxisInfo, rowAxisInfo);
			}
			return null;
		}
		#region Fields
		PivotZoneType type;
		PivotZoneAxisViewInfo columnAxisInfo;
		PivotZoneAxisViewInfo rowAxisInfo;
		#endregion
		public PivotZoneViewInfo(PivotZoneType type, PivotZoneAxisViewInfo columnAxisInfo, PivotZoneAxisViewInfo rowAxisInfo) {
			this.type = type;
			this.columnAxisInfo = columnAxisInfo;
			this.rowAxisInfo = rowAxisInfo;
		}
		#region Properties
		public PivotZoneType Type { get { return type; } }
		public IPivotZoneAxisInfo ColumnAxisInfo { get { return columnAxisInfo; } }
		public IPivotZoneAxisInfo RowAxisInfo { get { return rowAxisInfo; } }
		#endregion
	}
	#endregion
	#region PivotZoneAxisInfo
	public class PivotZoneAxisInfo : IPivotZoneAxisInfo {
		readonly IPivotLayoutItem layoutItem;
		public PivotZoneAxisInfo(IPivotLayoutItem layoutItem) {
			Guard.ArgumentNotNull(layoutItem, "layoutItem");
			this.layoutItem = layoutItem;
			FieldIndex = -1;
			Formatting = PivotFormattingType.None;
		}
		public IPivotLayoutItem LayoutItem { get { return layoutItem; } }
		public int FieldIndex { get; set; }
		public PivotFormattingType Formatting { get; set; }
		public bool HasFirstCell { get; set; }
		public bool HasLastCell { get; set; }
		public bool IsVertical { get; set; }
		int GetFormattingIndex(int fieldCount, int lastFieldIndex) {
			int result = 0; 
			if (lastFieldIndex < fieldCount)
				result = lastFieldIndex == 1 ? 1 : lastFieldIndex % 2 + 2;
			return result;
		}
		public void SetSubHeadingFormatting(int fieldCount, int lastFieldIndex) {
			SetSubHeadingFormatting(GetFormattingIndex(fieldCount, lastFieldIndex));
		}
		public void SetSubHeadingFormatting(int index) {
			if (index <= 0)
				Formatting = PivotFormattingType.White;
			else {
				switch (index) {
					case 1:
						Formatting = PivotFormattingType.Subheading1;
						break;
					case 2:
						Formatting = PivotFormattingType.Subheading2;
						break;
					case 3:
						Formatting = PivotFormattingType.Subheading3;
						break;
					default:
						throw new ArgumentException();
				}
			}
		}
		public void SetSubTotalFormatting(int fieldCount, int lastFieldIndex) {
			SetSubTotalFormatting(GetFormattingIndex(fieldCount, lastFieldIndex));
		}
		public void SetSubTotalFormatting(int index) {
			if (index <= 0)
				Formatting = PivotFormattingType.White;
			else {
				switch (index) {
					case 1:
						Formatting = PivotFormattingType.SubTotal1;
						break;
					case 2:
						Formatting = PivotFormattingType.SubTotal2;
						break;
					case 3:
						Formatting = PivotFormattingType.SubTotal3;
						break;
					default:
						throw new ArgumentException();
				}
			}
		}
	}
	#endregion
	#region PivotZoneAxisViewInfo
	public class PivotZoneAxisViewInfo : IPivotZoneAxisInfo {
		#region Fields
		PivotFormattingType formatting;
		bool hasFirstCell;
		bool hasLastCell;
		bool isVertical;
		#endregion
		public PivotZoneAxisViewInfo() {
		}
		public PivotZoneAxisViewInfo(PivotFormattingType formatting, bool hasFirstCell, bool hasLastCell, bool isVertical) {
			this.formatting = formatting;
			this.hasFirstCell = hasFirstCell;
			this.hasLastCell = hasLastCell;
			this.isVertical = isVertical;
		}
		#region Properties
		public PivotFormattingType Formatting { get { return formatting; } }
		public bool HasFirstCell { get { return hasFirstCell; } }
		public bool HasLastCell { get { return hasLastCell; } }
		public bool IsVertical { get { return isVertical; } }
		#endregion
	}
	#endregion
}
