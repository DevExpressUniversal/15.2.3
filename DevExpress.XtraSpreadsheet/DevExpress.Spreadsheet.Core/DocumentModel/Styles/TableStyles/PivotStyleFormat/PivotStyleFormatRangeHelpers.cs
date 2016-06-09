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

namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotStyleFormatZoneHelper
	public struct PivotStyleFormatZoneHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return !owner.PivotZoneCache.IsDefault;
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			IPivotZone pivotZone = owner.GetPivotZoneByCellPosition(absolutePosition);
			if (pivotZone == null)
				return;
			PivotZoneFormattingCache pivotZoneCache = owner.PivotZoneCache;
			PivotZoneType type = pivotZone.Type;
			IPivotZoneAxisInfo rowAxisInfo = pivotZone.RowAxisInfo;
			IPivotZoneAxisInfo columnAxisInfo = pivotZone.ColumnAxisInfo;
			if ((type & PivotZoneType.GrandTotalRow) != 0)
				RegisterGrandTotalRowFormatting(formatting, pivotZoneCache, styleCache, rowAxisInfo);
			if ((type & PivotZoneType.GrandTotalColumn) != 0)
				RegisterGrandTotalColumnFormatting(formatting, pivotZoneCache, styleCache, columnAxisInfo);
			if ((type & PivotZoneType.RowHeader) != 0)   
				RegisterRowHeaderFormatting(formatting, pivotZoneCache, styleCache, rowAxisInfo);
			if ((type & PivotZoneType.ColumnHeader) != 0) 
				RegisterColumnHeaderFormatting(formatting, pivotZoneCache, styleCache, columnAxisInfo);
			if ((type & PivotZoneType.Data) != 0) {
				RegisterRowHeaderFormatting(formatting, pivotZoneCache, styleCache, rowAxisInfo);
				RegisterSubtotalColumnFormatting(formatting, pivotZoneCache, styleCache, columnAxisInfo);
			}
		}
		void RegisterGrandTotalRowFormatting(ActualTableStyleCellFormatting formatting, PivotZoneFormattingCache pivotZoneCache, TableStyleElementInfoCache styleCache, IPivotZoneAxisInfo rowAxisInfo) {
			if (pivotZoneCache.HasGrandTotalRowFormatting) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateRowInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.TotalRowIndex), bordersOutlineInfo);
			}
		}
		void RegisterGrandTotalColumnFormatting(ActualTableStyleCellFormatting formatting, PivotZoneFormattingCache pivotZoneCache, TableStyleElementInfoCache styleCache, IPivotZoneAxisInfo columnAxisInfo) {
			if (pivotZoneCache.HasGrandTotalColumnFormatting) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateColumnInfo(columnAxisInfo.HasFirstCell, columnAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.LastColumnIndex), bordersOutlineInfo);
			}
		}
		void RegisterColumnHeaderFormatting(ActualTableStyleCellFormatting formatting, PivotZoneFormattingCache pivotZoneCache, TableStyleElementInfoCache styleCache, IPivotZoneAxisInfo columnAxisInfo) {
			PivotFormattingType formattingType = columnAxisInfo.Formatting;
			if (formattingType == PivotFormattingType.White || formattingType == PivotFormattingType.Grand || formattingType == PivotFormattingType.None)
				return;
			if (pivotZoneCache.HasFirstColumnSubheadingFormatting && (formattingType == PivotFormattingType.Subheading1 || formattingType == PivotFormattingType.SubTotal1)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetColumnHeaderBorder(PivotFormattingType.SubTotal1, formattingType, columnAxisInfo);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstColumnSubheadingIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasSecondColumnSubheadingFormatting && (formattingType == PivotFormattingType.Subheading2 || formattingType == PivotFormattingType.SubTotal2)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetColumnHeaderBorder(PivotFormattingType.SubTotal2, formattingType, columnAxisInfo);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondColumnSubheadingIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasThirdColumnSubheadingFormatting && (formattingType == PivotFormattingType.Subheading3 || formattingType == PivotFormattingType.SubTotal3)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetColumnHeaderBorder(PivotFormattingType.SubTotal3, formattingType, columnAxisInfo);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.ThirdColumnSubheadingIndex), bordersOutlineInfo);
				return;
			}
			RegisterSubtotalColumnFormatting(formatting, pivotZoneCache, styleCache, columnAxisInfo);
		}
		TableStyleFormatBordersOutlineInfo GetColumnHeaderBorder(PivotFormattingType expectedType, PivotFormattingType actualType, IPivotZoneAxisInfo columnInfo) {
			if (expectedType == actualType)
				return TableStyleFormatBordersOutlineInfo.CreatePositionInfo();
			return TableStyleFormatBordersOutlineInfo.CreateRowInfo(columnInfo.HasFirstCell, columnInfo.HasLastCell);
		}
		void RegisterSubtotalColumnFormatting(ActualTableStyleCellFormatting formatting, PivotZoneFormattingCache pivotZoneCache, TableStyleElementInfoCache styleCache, IPivotZoneAxisInfo columnAxisInfo) {
			PivotFormattingType formattingType = columnAxisInfo.Formatting;
			if (pivotZoneCache.HasFirstSubtotalColumnFormatting && formattingType == PivotFormattingType.SubTotal1) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateColumnInfo(columnAxisInfo.HasFirstCell, columnAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstSubtotalColumnIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasSecondSubtotalColumnFormatting && formattingType == PivotFormattingType.SubTotal2) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateColumnInfo(columnAxisInfo.HasFirstCell, columnAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondSubtotalColumnIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasThirdSubtotalColumnFormatting && formattingType == PivotFormattingType.SubTotal3) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateColumnInfo(columnAxisInfo.HasFirstCell, columnAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.ThirdSubtotalColumnIndex), bordersOutlineInfo);
			}
		}
		void RegisterRowHeaderFormatting(ActualTableStyleCellFormatting formatting, PivotZoneFormattingCache pivotZoneCache, TableStyleElementInfoCache styleCache, IPivotZoneAxisInfo rowAxisInfo) {
			PivotFormattingType formattingType = rowAxisInfo.Formatting;
			if (formattingType == PivotFormattingType.White || formattingType == PivotFormattingType.Grand || formattingType == PivotFormattingType.None)
				return;
			if (pivotZoneCache.HasFirstRowSubheadingFormatting && formattingType == PivotFormattingType.Subheading1) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateVectorInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell, rowAxisInfo.IsVertical);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstRowSubheadingIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasSecondRowSubheadingFormatting && formattingType == PivotFormattingType.Subheading2) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateVectorInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell, rowAxisInfo.IsVertical);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondRowSubheadingIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasThirdRowSubheadingFormatting && formattingType == PivotFormattingType.Subheading3) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateVectorInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell, rowAxisInfo.IsVertical);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.ThirdRowSubheadingIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasFirstSubtotalRowFormatting && formattingType == PivotFormattingType.SubTotal1) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateRowInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstSubtotalRowIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasSecondSubtotalRowFormatting && formattingType == PivotFormattingType.SubTotal2) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateRowInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondSubtotalRowIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasThirdSubtotalRowFormatting && formattingType == PivotFormattingType.SubTotal3) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateRowInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.ThirdSubtotalRowIndex), bordersOutlineInfo);
				return;
			}
			if (pivotZoneCache.HasBlankRowFormatting && formattingType == PivotFormattingType.Blank) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateRowInfo(rowAxisInfo.HasFirstCell, rowAxisInfo.HasLastCell);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.BlankRowIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region PivotStyleFormatFirstHeaderCellRangeHelper
	public struct PivotStyleFormatFirstHeaderCellRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			IPivotStyleOptions options = owner.Options;
			return options.ShowColumnHeaders && options.ShowRowHeaders && styleCache.HasDifferentialFormatting(TableStyle.FirstHeaderCellIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			PivotTableLocationZone locationZone = owner.Location.Cache.FirstHeaderCell;
			if (locationZone.ContainsCell(column, row)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = locationZone.GetBordersOutlineInfo(column, row);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstHeaderCellIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region PivotStyleFormatHeaderRowRangeHelper
	public struct PivotStyleFormatHeaderRowRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return owner.Options.ShowColumnHeaders && styleCache.HasDifferentialFormatting(TableStyle.HeaderRowIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			PivotTableLocationZone locationZone = owner.Location.Cache.RowHeader;
			if (locationZone.ContainsCell(column, row)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = locationZone.GetBordersOutlineInfo(column, row);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.HeaderRowIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region PivotStyleFormatFirstColumnRangeHelper
	public struct PivotStyleFormatFirstColumnRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return owner.Options.ShowRowHeaders && styleCache.HasDifferentialFormatting(TableStyle.FirstColumnIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			PivotTableLocationZone locationZone = owner.Location.Cache.ColumnHeader;
			if (locationZone.ContainsCell(column, row)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = locationZone.GetBordersOutlineInfo(column, row);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstColumnIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region PivotStyleFormatRowStripeRangeHelper
	public struct PivotStyleFormatRowStripeRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				owner.Options.ShowRowStripes && !(owner.DataFieldsCount == 0 && owner.ColumnFieldsCount == 0) &&
				(styleCache.HasDifferentialFormatting(TableStyle.SecondRowStripeIndex) ||
				styleCache.HasDifferentialFormatting(TableStyle.FirstRowStripeIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> relative = owner.StyleFormatCache.Relative;
			ITableStyleFormatStripeInfoCache stripeCache = relative.RowStripesCache;
			if (stripeCache == null)
				return;
			int row = absolutePosition.Row;
			if (stripeCache.ContainsIndex(row)) {
				TableStyleStripeInfo info = stripeCache.GetStripeInfo(row);
				bool isFirstStripe = info.IsFirstStripe;
				if (isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.FirstRowStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, relative.ColumnsCache.GeneralCache, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstRowStripeIndex), bordersOutlineInfo);
				}
				if (!isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.SecondRowStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, relative.ColumnsCache.GeneralCache, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondRowStripeIndex), bordersOutlineInfo);
				}
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableRelativeColumnRowCache columnsCache, TableStyleStripeInfo stripeInfo) {
			int relativeColumn = columnsCache.GetRelativeIndex(absolutePosition.Column);
			return new TableStyleFormatBordersOutlineInfo(relativeColumn == columnsCache.FirstRelativeDataIndex, relativeColumn == columnsCache.LastRelativeDataIndex, stripeInfo.IsFirstOutline, stripeInfo.IsLastOutline);
		}
	}
	#endregion
	#region PivotStyleFormatColumnStripeRangeHelper
	public struct PivotStyleFormatColumnStripeRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				owner.Options.ShowColumnStripes &&
				(styleCache.HasDifferentialFormatting(TableStyle.SecondColumnStripeIndex) ||
				styleCache.HasDifferentialFormatting(TableStyle.FirstColumnStripeIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> relative = owner.StyleFormatCache.Relative;
			ITableRelativeColumnRowCache rowsCache = relative.RowsCache.GeneralCache;
			int row = absolutePosition.Row;
			if (!rowsCache.ContainsAbsoluteDataIndex(row))
				return;
			ITableStyleFormatStripeInfoCache stripeCache = relative.ColumnStripesCache;
			if (stripeCache == null)
				return;
			int column = absolutePosition.Column;
			if (stripeCache.ContainsIndex(column)) {
				TableStyleStripeInfo info = stripeCache.GetStripeInfo(column);
				bool isFirstStripe = info.IsFirstStripe;
				if (isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.FirstColumnStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(column, row, rowsCache, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstColumnStripeIndex), bordersOutlineInfo);
				}
				if (!isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.SecondColumnStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(column, row, rowsCache, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondColumnStripeIndex), bordersOutlineInfo);
				}
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(int column, int row, ITableRelativeColumnRowCache rowsCache, TableStyleStripeInfo stripeInfo) {
			int relativeRow = rowsCache.GetRelativeIndex(row);
			return new TableStyleFormatBordersOutlineInfo(stripeInfo.IsFirstOutline, stripeInfo.IsLastOutline, relativeRow == rowsCache.FirstRelativeDataIndex, relativeRow == rowsCache.LastRelativeDataIndex);
		} 
	}
	#endregion
	#region PivotStyleFormatPageFieldsRangeHelper
	public struct PivotStyleFormatPageFieldsRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				!owner.Location.Cache.PageFieldsZonesCache.IsEmpty &&
				(styleCache.HasDifferentialFormatting(TableStyle.PageFieldLabelsIndex) ||
				styleCache.HasDifferentialFormatting(TableStyle.PageFieldValuesIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			PivotTableLocationPageFieldsZonesCache pageFieldsZonesCache = owner.Location.Cache.PageFieldsZonesCache;
			int index = pageFieldsZonesCache.TryGetIndexByCellPosition(column, row);
			if (index != -1) {
				int labelsColumn = pageFieldsZonesCache[index].TopLeft.Column;
				byte elementIndex = (byte)(labelsColumn == column ? TableStyle.PageFieldLabelsIndex : TableStyle.PageFieldValuesIndex);
				if (styleCache.HasDifferentialFormatting(elementIndex)) {
					ITableRelativeColumnRowCache relativeCache = owner.StyleFormatCache.Relative.RowsCache.PageFieldsCaches.GetCache(index);
					int relativeRow = relativeCache.GetRelativeIndex(row);
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreateColumnInfo(relativeRow == 0, relativeRow == relativeCache.LastRelativeIndex);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(elementIndex), bordersOutlineInfo);
				}
			}
		}
	}
	#endregion
	#region PivotStyleFormatWholeTableRangeHelper
	public struct PivotStyleFormatWholeTableRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return styleCache.HasDifferentialFormatting(TableStyle.WholeTableIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
			formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.WholeTableIndex), bordersOutlineInfo);
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, IPivotStyleOwner owner) {
			ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> relative = owner.StyleFormatCache.Relative;
			ITableRelativeColumnRowCache columnsCache = relative.ColumnsCache.GeneralCache;
			ITableRelativeColumnRowCache rowsCache = relative.RowsCache.GeneralCache;
			int relativeColumn = columnsCache.GetRelativeIndex(absolutePosition.Column);
			int relativeRow = rowsCache.GetRelativeIndex(absolutePosition.Row);
			return new TableStyleFormatBordersOutlineInfo(relativeColumn == 0, relativeColumn == columnsCache.LastRelativeIndex, relativeRow == 0, relativeRow == rowsCache.LastRelativeIndex);
		}
	}
	#endregion
	#region PivotStyleFormatWholeTablePageFieldsRangeHelper
	public struct PivotStyleFormatWholeTablePageFieldsRangeHelper : ITableStyleFormatRangeHelper<IPivotStyleOwner> {
		public bool CheckPrepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				!owner.Location.Cache.PageFieldsZonesCache.IsEmpty && 
				styleCache.HasDifferentialFormatting(TableStyle.WholeTableIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			PivotTableLocationPageFieldsZonesCache pageFieldsZonesCache = owner.Location.Cache.PageFieldsZonesCache;
			int index = pageFieldsZonesCache.TryGetIndexByCellPosition(column, row);
			if (index != -1) {
				ITableRelativeColumnRowCache relativeRowCache = owner.StyleFormatCache.Relative.RowsCache.PageFieldsCaches.GetCache(index);
				int relativeRow = relativeRowCache.GetRelativeIndex(row);
				ITableRelativeColumnRowCache relativeColumnCache = owner.StyleFormatCache.Relative.ColumnsCache.PageFieldsCaches.GetCache(index);
				int relativeColumn = relativeColumnCache.GetRelativeIndex(column);
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = new TableStyleFormatBordersOutlineInfo(relativeColumn == 0, relativeColumn == relativeColumnCache.LastRelativeIndex, relativeRow == 0, relativeRow == relativeRowCache.LastRelativeIndex);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.WholeTableIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
} 
