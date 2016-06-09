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
	#region TableStyleFormatWholeTableRangeHelper
	public struct TableStyleFormatWholeTableRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return styleCache.HasDifferentialFormatting(TableStyle.WholeTableIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
			formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.WholeTableIndex), bordersOutlineInfo);
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableStyleOwner owner) {
			ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> relative = owner.Cache.Relative;
			ITableRelativeColumnRowCache columnsCache = relative.ColumnsCache;
			ITableRelativeColumnRowCache rowsCache = relative.RowsCache;
			int relativeColumn = columnsCache.GetRelativeIndex(absolutePosition.Column);
			int relativeRow = rowsCache.GetRelativeIndex(absolutePosition.Row);
			return new TableStyleFormatBordersOutlineInfo(relativeColumn == 0, relativeColumn == columnsCache.LastRelativeIndex, relativeRow == 0, relativeRow == rowsCache.LastRelativeIndex);
		}
	}
	#endregion
	#region TableStyleFormatHeaderTotalRowRangeHelper
	public struct TableStyleFormatHeaderTotalRowRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableStyleOptions options = owner.Options;
			return
				(options.HasHeadersRow && styleCache.HasDifferentialFormatting(TableStyle.HeaderRowIndex)) ||
				(options.HasTotalsRow && styleCache.HasDifferentialFormatting(TableStyle.TotalRowIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			int row = absolutePosition.Row;
			CellRange range = owner.Range;
			ITableStyleOptions options = owner.Options;
			if (options.HasHeadersRow && row == range.TopRowIndex && styleCache.HasDifferentialFormatting(TableStyle.HeaderRowIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.HeaderRowIndex), bordersOutlineInfo);
			}
			if (options.HasTotalsRow && row == range.BottomRowIndex && styleCache.HasDifferentialFormatting(TableStyle.TotalRowIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.TotalRowIndex), bordersOutlineInfo);
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableStyleOwner owner) {
			ITableRelativeColumnRowCache columnsCache = owner.Cache.Relative.ColumnsCache;
			int relativeColumn = columnsCache.GetRelativeIndex(absolutePosition.Column);
			return TableStyleFormatBordersOutlineInfo.CreateRowInfo(relativeColumn == 0, relativeColumn == columnsCache.LastRelativeIndex);
		}
	}
	#endregion
	#region TableStyleFormatFirstColumnRangeHelper
	public struct TableStyleFormatFirstColumnRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return owner.Options.ShowFirstColumn && styleCache.HasDifferentialFormatting(TableStyle.FirstColumnIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			if (absolutePosition.Column == owner.Range.LeftColumnIndex) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstColumnIndex), bordersOutlineInfo);
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableStyleOwner owner) {
			ITableRelativeColumnRowCache rowsCache = owner.Cache.Relative.RowsCache;
			int relativeRow = rowsCache.GetRelativeIndex(absolutePosition.Row);
			return TableStyleFormatBordersOutlineInfo.CreateColumnInfo(relativeRow == 0, relativeRow == rowsCache.LastRelativeIndex);
		} 
	}
	#endregion
	#region TableStyleFormatLastColumnRangeHelper
	public struct TableStyleFormatLastColumnRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return owner.Options.ShowLastColumn && styleCache.HasDifferentialFormatting(TableStyle.LastColumnIndex);
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			if (absolutePosition.Column == owner.Range.RightColumnIndex) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, owner);
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.LastColumnIndex), bordersOutlineInfo);
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableStyleOwner owner) {
			ITableRelativeColumnRowCache rowsCache = owner.Cache.Relative.RowsCache;
			int relativeRow = rowsCache.GetRelativeIndex(absolutePosition.Row);
			return TableStyleFormatBordersOutlineInfo.CreateColumnInfo(relativeRow == 0, relativeRow == rowsCache.LastRelativeIndex);
		} 
	}
	#endregion
	#region TableStyleFormatFirstColumnCellsRangeHelper
	public struct TableStyleFormatFirstColumnCellsRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableStyleOptions options = owner.Options;
			return
				options.ShowFirstColumn &&
				((options.HasHeadersRow && styleCache.HasDifferentialFormatting(TableStyle.FirstHeaderCellIndex)) ||
				(options.HasTotalsRow && styleCache.HasDifferentialFormatting(TableStyle.FirstTotalCellIndex)));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			CellRange range = owner.Range;
			ITableStyleOptions options = owner.Options;
			if (options.HasHeadersRow && absolutePosition.EqualsPosition(range.TopLeft) && styleCache.HasDifferentialFormatting(TableStyle.FirstHeaderCellIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreatePositionInfo();
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstHeaderCellIndex), bordersOutlineInfo);
			}
			if (options.HasTotalsRow && absolutePosition.EqualsPosition(range.BottomLeft) && styleCache.HasDifferentialFormatting(TableStyle.FirstTotalCellIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreatePositionInfo();
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstTotalCellIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region TableStyleFormatLastColumnCellsRangeHelper
	public struct TableStyleFormatLastColumnCellsRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableStyleOptions options = owner.Options;
			return
				options.ShowLastColumn &&
				((options.HasHeadersRow && styleCache.HasDifferentialFormatting(TableStyle.LastHeaderCellIndex)) ||
				(options.HasTotalsRow && styleCache.HasDifferentialFormatting(TableStyle.LastTotalCellIndex)));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			CellRange range = owner.Range;
			ITableStyleOptions options = owner.Options;
			if (options.HasHeadersRow && absolutePosition.EqualsPosition(range.TopRight) && styleCache.HasDifferentialFormatting(TableStyle.LastHeaderCellIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreatePositionInfo();
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.LastHeaderCellIndex), bordersOutlineInfo);
			}
			if (options.HasTotalsRow && absolutePosition.EqualsPosition(range.BottomRight) && styleCache.HasDifferentialFormatting(TableStyle.LastTotalCellIndex)) {
				TableStyleFormatBordersOutlineInfo bordersOutlineInfo = TableStyleFormatBordersOutlineInfo.CreatePositionInfo();
				formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.LastTotalCellIndex), bordersOutlineInfo);
			}
		}
	}
	#endregion
	#region TableStyleFormatRowStripeRangeHelper
	public struct TableStyleFormatRowStripeRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				owner.Options.ShowRowStripes &&
				(styleCache.HasDifferentialFormatting(TableStyle.SecondRowStripeIndex) ||
				styleCache.HasDifferentialFormatting(TableStyle.FirstRowStripeIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> relative = owner.Cache.Relative;
			ITableStyleFormatStripeInfoCache stripeCache = relative.RowStripesCache;
			if (stripeCache == null)
				return;
			int row = absolutePosition.Row;
			if (stripeCache.ContainsIndex(row)) {
				TableStyleStripeInfo info = stripeCache.GetStripeInfo(row);
				bool isFirstStripe = info.IsFirstStripe;
				if (isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.FirstRowStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, relative, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.FirstRowStripeIndex), bordersOutlineInfo);
				}
				if (!isFirstStripe && styleCache.HasDifferentialFormatting(TableStyle.SecondRowStripeIndex)) {
					TableStyleFormatBordersOutlineInfo bordersOutlineInfo = GetBordersOutlineInfo(absolutePosition, relative, info);
					formatting.RegisterCellFormatting(styleCache.GetDifferentialFormatIndex(TableStyle.SecondRowStripeIndex), bordersOutlineInfo);
				}
			}
		}
		TableStyleFormatBordersOutlineInfo GetBordersOutlineInfo(CellPosition absolutePosition, ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> relative, TableStyleStripeInfo stripeInfo) {
			ITableRelativeColumnRowCache columnsCache = relative.ColumnsCache;
			int relativeColumn = columnsCache.GetRelativeIndex(absolutePosition.Column);
			return new TableStyleFormatBordersOutlineInfo(relativeColumn == columnsCache.FirstRelativeDataIndex, relativeColumn == columnsCache.LastRelativeDataIndex, stripeInfo.IsFirstOutline, stripeInfo.IsLastOutline);
		} 
	}
	#endregion
	#region TableStyleFormatColumnStripeRangeHelper
	public struct TableStyleFormatColumnStripeRangeHelper : ITableStyleFormatRangeHelper<ITableStyleOwner> {
		public bool CheckPrepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			return
				owner.Options.ShowColumnStripes &&
				(styleCache.HasDifferentialFormatting(TableStyle.SecondColumnStripeIndex) ||
				styleCache.HasDifferentialFormatting(TableStyle.FirstColumnStripeIndex));
		}
		public void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> relative = owner.Cache.Relative;
			ITableRelativeColumnRowCache rowsCache = relative.RowsCache;
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
} 
