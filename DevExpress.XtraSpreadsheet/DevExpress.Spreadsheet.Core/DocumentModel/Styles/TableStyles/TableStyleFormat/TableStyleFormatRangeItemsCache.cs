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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableAbsoluteRangeItemsCacheCollection
	public class TableAbsoluteRangeItemsCacheCollection : ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> {
		#region Fields
		ITableRelativeColumnRowCache columnsCache;
		ITableRelativeColumnRowCache rowsCache;
		ITableStyleFormatStripeInfoCache columnStripesCache;
		ITableStyleFormatStripeInfoCache rowStripesCache;
		#endregion
		protected internal bool IsEmpty { get { return columnsCache == null && rowsCache == null && columnStripesCache == null && rowStripesCache == null; } }
		#region ITableRelativeRangeItemsCaches<ITableStyleOwner, ITableRelativeColumnRowCache> Members
		public ITableRelativeColumnRowCache ColumnsCache { get { return columnsCache; } protected set { columnsCache = value; } }
		public ITableRelativeColumnRowCache RowsCache { get { return rowsCache; } protected set { rowsCache = value; } }
		public ITableStyleFormatStripeInfoCache ColumnStripesCache { get { return columnStripesCache; } protected set { columnStripesCache = value; } }
		public ITableStyleFormatStripeInfoCache RowStripesCache { get { return rowStripesCache; } protected set { rowStripesCache = value; } }
		public void Prepare(ITableStyleOwner styleOwner, TableStyleElementInfoCache styleCache) {
			CellRange tableRange = styleOwner.Range;
			Worksheet sheet = tableRange.Worksheet as Worksheet;
			if (sheet == null)
				return;
			CreateColumnsCaches(sheet, tableRange, styleCache.FirstColumnStripeSize, styleCache.SecondColumnStripeSize, styleOwner);
			CreateRowsCaches(sheet, tableRange, styleCache.FirstRowStripeSize, styleCache.SecondRowStripeSize, styleOwner);
		}
		public void Clear() {
			columnsCache = null;
			rowsCache = null;
			columnStripesCache = null;
			rowStripesCache = null;
		}
		#endregion
		protected virtual void CreateColumnsCaches(Worksheet sheet, CellRange tableRange, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			int leftAbsoluteIndex = tableRange.LeftColumnIndex;
			int rightAbsoluteIndex = tableRange.RightColumnIndex;
			CreateAbsoluteColumnsCaches(leftAbsoluteIndex, rightAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner.Options);
		}
		protected virtual void CreateRowsCaches(Worksheet sheet, CellRange tableRange, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			int topAbsoluteIndex = tableRange.TopRowIndex;
			int bottomAbsoluteIndex = tableRange.BottomRowIndex;
			CreateAbsoluteRowsCaches(topAbsoluteIndex, bottomAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner.Options);
		}
		protected void CreateAbsoluteColumnsCaches(int leftAbsoluteIndex, int rightAbsoluteIndex, int firstStripeSize, int secondStripeSize, ITableStyleOptions styleOptions) {
			columnsCache = new TableAbsoluteColumnsRowsCache(leftAbsoluteIndex);
			columnsCache.LastRelativeIndex = rightAbsoluteIndex - leftAbsoluteIndex;
			columnsCache.FirstRelativeDataIndex = 0;
			columnsCache.LastRelativeDataIndex = rightAbsoluteIndex - leftAbsoluteIndex;
			if (styleOptions.ShowColumnStripes) {
				int stripeCount = firstStripeSize + secondStripeSize;
				columnStripesCache = new TableStyleFormatAbsoluteStripeInfoCache(leftAbsoluteIndex, rightAbsoluteIndex, firstStripeSize, stripeCount);
			}
		}
		protected void CreateAbsoluteRowsCaches(int topAbsoluteIndex, int bottomAbsoluteIndex, int firstStripeSize, int secondStripeSize, ITableStyleOptions styleOptions) {
			rowsCache = new TableAbsoluteColumnsRowsCache(topAbsoluteIndex);
			rowsCache.LastRelativeIndex = bottomAbsoluteIndex - topAbsoluteIndex;
			int firstRelativeDataIndex = styleOptions.HasHeadersRow ? 1 : 0;
			int firstAbsoluteDataIndex = topAbsoluteIndex + firstRelativeDataIndex;
			int lastAbsoluteDataIndex = bottomAbsoluteIndex - (styleOptions.HasTotalsRow ? 1 : 0);
			int lastRelativeDataIndex = lastAbsoluteDataIndex - topAbsoluteIndex;
			rowsCache.FirstRelativeDataIndex = firstRelativeDataIndex;
			rowsCache.LastRelativeDataIndex = lastRelativeDataIndex;
			if (styleOptions.ShowRowStripes) {
				int stripeCount = firstStripeSize + secondStripeSize;
				rowStripesCache = new TableStyleFormatAbsoluteStripeInfoCache(firstAbsoluteDataIndex, lastAbsoluteDataIndex, firstStripeSize, stripeCount);
			}
		}
	}
	#endregion
	#region TableRelativeRangeItemsCacheCollection
	public class TableRelativeRangeItemsCacheCollection : TableAbsoluteRangeItemsCacheCollection {
		protected override void CreateColumnsCaches(Worksheet sheet, CellRange tableRange, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			int leftAbsoluteIndex = tableRange.LeftColumnIndex;
			int rightAbsoluteIndex = tableRange.RightColumnIndex;
			IEnumerator<Column> notVisibleColumns = sheet.Columns.GetExistingNotVisibleColumnsEnumerator(leftAbsoluteIndex, rightAbsoluteIndex, false);
			if (notVisibleColumns.MoveNext()) {
				notVisibleColumns.Reset();
				CreateRelativeColumnsCaches(notVisibleColumns, leftAbsoluteIndex, rightAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner);
			} else
				CreateAbsoluteColumnsCaches(leftAbsoluteIndex, rightAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner.Options);
		}
		protected override void CreateRowsCaches(Worksheet sheet, CellRange tableRange, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			int topAbsoluteIndex = tableRange.TopRowIndex;
			int bottomAbsoluteIndex = tableRange.BottomRowIndex;
			IEnumerator<Row> notVisibleRows = sheet.Rows.GetExistingNotVisibleRowsEnumerator(topAbsoluteIndex, bottomAbsoluteIndex, false);
			if (notVisibleRows.MoveNext()) {
				notVisibleRows.Reset();
				CreateRelativeRowsCaches(notVisibleRows, topAbsoluteIndex, bottomAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner);
			} else
				CreateAbsoluteRowsCaches(topAbsoluteIndex, bottomAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner.Options);
		}
		void CreateRelativeColumnsCaches(IEnumerator<Column> notVisibleColumns, int leftAbsoluteIndex, int rightAbsoluteIndex, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			TableRelativeColumnsCacheBuilder builder = new TableRelativeColumnsCacheBuilder();
			ColumnsCache = new TableRelativeColumnsRowsCache();
			int firstAbsoluteDataIndex = leftAbsoluteIndex;
			int lastAbsoluteDataIndex = rightAbsoluteIndex;
			if (styleOwner.Options.ShowColumnStripes) {
				ColumnStripesCache = new TableStyleFormatRelativeColumnStripeInfoCache(styleOwner, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
				int stripeCount = firstStripeSize + secondStripeSize;
				builder.Build(leftAbsoluteIndex, rightAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, ColumnsCache, notVisibleColumns, ColumnStripesCache, firstStripeSize, stripeCount);
			} else
				builder.Build(leftAbsoluteIndex, rightAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, ColumnsCache, notVisibleColumns);
		}
		void CreateRelativeRowsCaches(IEnumerator<Row> notVisibleRows, int topAbsoluteIndex, int bottomAbsoluteIndex, int firstStripeSize, int secondStripeSize, ITableStyleOwner styleOwner) {
			TableRelativeRowsCacheBuilder builder = new TableRelativeRowsCacheBuilder();
			RowsCache = new TableRelativeColumnsRowsCache();
			ITableStyleOptions options = styleOwner.Options;
			bool hasTotalsRow = options.HasTotalsRow;
			int firstAbsoluteDataIndex = topAbsoluteIndex + (options.HasHeadersRow ? 1 : 0);
			int lastAbsoluteDataIndex = bottomAbsoluteIndex - (hasTotalsRow ? 1 : 0);
			if (options.ShowRowStripes) {
				RowStripesCache = new TableStyleFormatRelativeRowStripeInfoCache(styleOwner, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
				int stripeCount = firstStripeSize + secondStripeSize;
				builder.Build(topAbsoluteIndex, bottomAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, RowsCache, notVisibleRows, RowStripesCache, firstStripeSize, stripeCount);
			} else
				builder.Build(topAbsoluteIndex, bottomAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, RowsCache, notVisibleRows);
			if (hasTotalsRow && RowsCache.HasVisibleColumnRowIndex(bottomAbsoluteIndex) && RowsCache.GetRelativeIndex(bottomAbsoluteIndex) == RowsCache.LastRelativeDataIndex)
				RowsCache.LastRelativeDataIndex--;
		}
	}
	#endregion
} 
