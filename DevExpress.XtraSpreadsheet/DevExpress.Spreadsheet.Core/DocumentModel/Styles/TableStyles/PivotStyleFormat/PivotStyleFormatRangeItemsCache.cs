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
	#region PivotAbsoluteRangeItemsCacheCollection
	public class PivotAbsoluteRangeItemsCacheCollection : ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> {
		#region Fields
		IPivotRelativeColumnRowCache columnsCache;
		IPivotRelativeColumnRowCache rowsCache;
		ITableStyleFormatStripeInfoCache columnStripesCache;
		ITableStyleFormatStripeInfoCache rowStripesCache;
		#endregion
		protected internal bool IsEmpty { get { return columnsCache == null && rowsCache == null && columnStripesCache == null && rowStripesCache == null; } }
		#region ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> Members
		public IPivotRelativeColumnRowCache ColumnsCache { get { return columnsCache; } protected set { columnsCache = value; } }
		public IPivotRelativeColumnRowCache RowsCache { get { return rowsCache; } protected set { rowsCache = value; } }
		public ITableStyleFormatStripeInfoCache ColumnStripesCache { get { return columnStripesCache; } protected set { columnStripesCache = value; } }
		public ITableStyleFormatStripeInfoCache RowStripesCache { get { return rowStripesCache; } protected set { rowStripesCache = value; } }
		public void Prepare(IPivotStyleOwner styleOwner, TableStyleElementInfoCache styleCache) {
			CreateGeneralColumnsCaches(styleCache.FirstColumnStripeSize, styleCache.SecondColumnStripeSize, styleOwner);
			CreateGeneralRowsCaches(styleCache.FirstRowStripeSize, styleCache.SecondRowStripeSize, styleOwner);
			CreatePageFieldsCaches(styleOwner);
		}
		#region CreateCaches (Virtual methods)
		protected virtual void CreateGeneralColumnsCaches(int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			CreateAbsoluteGeneralColumnsCaches(firstStripeSize, secondStripeSize, styleOwner.Location, styleOwner.Options.ShowColumnStripes);
		}
		protected virtual void CreateGeneralRowsCaches(int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			CreateAbsoluteGeneralRowsCaches(firstStripeSize, secondStripeSize, styleOwner.Location, styleOwner.Options.ShowRowStripes && styleOwner.ColumnFieldsCount > 0);
		}
		protected virtual void CreatePageFieldsCaches(IPivotStyleOwner styleOwner) {
			PivotTableLocationPageFieldsZonesCache cache = styleOwner.Location.Cache.PageFieldsZonesCache;
			if (!cache.IsEmpty) {
				int count = cache.Count;
				for (int i = 0; i < count; i++) {
					PivotTableLocationZone zone = cache[i];
					CreateAbsolutePageFieldsCache(columnsCache.PageFieldsCaches, zone.TopLeft.Column, zone.BottomRight.Column);
					CreateAbsolutePageFieldsCache(rowsCache.PageFieldsCaches, zone.TopLeft.Row, zone.BottomRight.Row);
				}
			}
		}
		#endregion
		#region CreateAbsoluteCaches
		protected void CreateAbsoluteGeneralColumnsCaches(int firstStripeSize, int secondStripeSize, IPivotTableLocation location, bool hasStripe) {
			CellPosition generalTopLeft = location.Cache.General.TopLeft;
			PivotTableLocationZone stripeZone = location.Cache.ColumnStripe;
			CellPosition stripeTopLeft = stripeZone.TopLeft;
			CellPosition stripeBottomRight = stripeZone.BottomRight;
			columnsCache = new PivotRelativeColumnRowCache();
			int firstAbsoluteIndex = generalTopLeft.Column;
			TableAbsoluteColumnsRowsCache generalColumnsCache = new TableAbsoluteColumnsRowsCache(firstAbsoluteIndex);
			columnsCache.GeneralCache = generalColumnsCache;
			int firstAbsoluteDataIndex = stripeTopLeft.Column;
			int lastAbsoluteDataIndex = stripeBottomRight.Column;
			int firstRelativeDataIndex = firstAbsoluteDataIndex - firstAbsoluteIndex;
			int lastRelativeDataIndex = lastAbsoluteDataIndex - firstAbsoluteIndex;
			generalColumnsCache.FirstRelativeDataIndex = firstRelativeDataIndex;
			generalColumnsCache.LastRelativeDataIndex = lastRelativeDataIndex;
			generalColumnsCache.LastRelativeIndex = lastRelativeDataIndex;
			if (hasStripe) {
				int stripeCount = firstStripeSize + secondStripeSize;
				columnStripesCache = new TableStyleFormatAbsoluteStripeInfoCache(firstAbsoluteDataIndex, lastAbsoluteDataIndex, firstStripeSize, stripeCount);
			}
		}
		protected void CreateAbsoluteGeneralRowsCaches(int firstStripeSize, int secondStripeSize, IPivotTableLocation location, bool hasStripe) {
			CellPosition generalTopLeft = location.Cache.General.TopLeft;
			PivotTableLocationZone stripeZone = location.Cache.RowStripe;
			CellPosition stripeTopLeft = stripeZone.TopLeft;
			CellPosition stripeBottomRight = stripeZone.BottomRight;
			rowsCache = new PivotRelativeColumnRowCache();
			int firstAbsoluteIndex = generalTopLeft.Row;
			TableAbsoluteColumnsRowsCache generalRowsCache = new TableAbsoluteColumnsRowsCache(firstAbsoluteIndex);
			rowsCache.GeneralCache = generalRowsCache;
			int firstAbsoluteDataIndex = stripeTopLeft.Row;
			int lastAbsoluteDataIndex = stripeBottomRight.Row;
			int firstRelativeDataIndex = firstAbsoluteDataIndex - firstAbsoluteIndex;
			int lastRelativeDataIndex = lastAbsoluteDataIndex - firstAbsoluteIndex;
			generalRowsCache.FirstRelativeDataIndex = firstRelativeDataIndex;
			generalRowsCache.LastRelativeDataIndex = lastRelativeDataIndex;
			generalRowsCache.LastRelativeIndex = lastRelativeDataIndex;
			if (hasStripe) {
				int stripeCount = firstStripeSize + secondStripeSize;
				rowStripesCache = new TableStyleFormatAbsoluteStripeInfoCache(firstAbsoluteDataIndex, lastAbsoluteDataIndex, firstStripeSize, stripeCount);
			}
		}
		protected void CreateAbsolutePageFieldsCache(PivotPageFieldsCacheCollection collection, int firstIndex, int lastIndex) {
			TableAbsoluteColumnsRowsCache pageFiltersCache = new TableAbsoluteColumnsRowsCache(firstIndex);
			collection.Add(pageFiltersCache);
			pageFiltersCache.FirstRelativeDataIndex = 0;
			int lastRelativeDataIndex = lastIndex - firstIndex;
			pageFiltersCache.LastRelativeDataIndex = lastRelativeDataIndex;
			pageFiltersCache.LastRelativeIndex = lastRelativeDataIndex;
		}
		#endregion
		public void Clear() {
			columnsCache = null;
			rowsCache = null;
			columnStripesCache = null;
			rowStripesCache = null;
		}
		#endregion
	}
	#endregion
	#region PivotAbsoluteRangeItemsCacheCollectionForPivotStyleViewInfo
	public class PivotAbsoluteRangeItemsCacheCollectionForPivotStyleViewInfo : PivotAbsoluteRangeItemsCacheCollection {
		protected override void CreatePageFieldsCaches(IPivotStyleOwner styleOwner) {
		}
	}
	#endregion
	#region PivotRelativeRangeItemsCacheCollection
	public class PivotRelativeRangeItemsCacheCollection : PivotAbsoluteRangeItemsCacheCollection {
		protected override void CreateGeneralColumnsCaches(int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			IPivotTableLocation location = styleOwner.Location;
			PivotTableLocationZone generalZone = location.Cache.General;
			int leftAbsoluteIndex = generalZone.TopLeft.Column;
			int rightAbsoluteIndex = generalZone.BottomRight.Column;
			IEnumerator<Column> notVisibleColumns = ((Worksheet)location.WholeRange.Worksheet).Columns.GetExistingNotVisibleColumnsEnumerator(leftAbsoluteIndex, rightAbsoluteIndex, false);
			if (notVisibleColumns.MoveNext()) {
				notVisibleColumns.Reset();
				CreateRelativeGeneralColumnsCaches(notVisibleColumns, leftAbsoluteIndex, rightAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner);
			} else
				CreateAbsoluteGeneralColumnsCaches(firstStripeSize, secondStripeSize, location, styleOwner.Options.ShowColumnStripes);
		}
		protected override void CreateGeneralRowsCaches(int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			IPivotTableLocation location = styleOwner.Location;
			PivotTableLocationZone generalZone = location.Cache.General;
			int topAbsoluteIndex = generalZone.TopLeft.Row;
			int bottomAbsoluteIndex = generalZone.BottomRight.Row;
			IEnumerator<Row> notVisibleRows = ((Worksheet)location.WholeRange.Worksheet).Rows.GetExistingNotVisibleRowsEnumerator(topAbsoluteIndex, bottomAbsoluteIndex, false);
			if (notVisibleRows.MoveNext()) {
				notVisibleRows.Reset();
				CreateRelativeGeneralRowsCaches(notVisibleRows, topAbsoluteIndex, bottomAbsoluteIndex, firstStripeSize, secondStripeSize, styleOwner);
			} else
				CreateAbsoluteGeneralRowsCaches(firstStripeSize, secondStripeSize, location, styleOwner.Options.ShowRowStripes && styleOwner.ColumnFieldsCount > 0);
		}
		protected override void CreatePageFieldsCaches(IPivotStyleOwner styleOwner) {
			IPivotTableLocation location = styleOwner.Location;
			Worksheet sheet = location.WholeRange.Worksheet as Worksheet;
			PivotTableLocationPageFieldsZonesCache cache = location.Cache.PageFieldsZonesCache;
			if (!cache.IsEmpty) {
				ColumnsCache.PageFieldsCaches = new PivotPageFieldsCacheCollection();
				RowsCache.PageFieldsCaches = new PivotPageFieldsCacheCollection();
				int count = cache.Count;
				for (int i = 0; i < count; i++) {
					PivotTableLocationZone zone = cache[i];
					CreateColumnsPageFieldsCache(sheet, zone.TopLeft.Column, zone.BottomRight.Column);
					CreateRowsPageFieldsCache(sheet, zone.TopLeft.Row, zone.BottomRight.Row);
				}
			}
		}
		#region Internal methods
		void CreateRelativeGeneralColumnsCaches(IEnumerator<Column> notVisibleColumns, int leftAbsoluteIndex, int rightAbsoluteIndex, int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			TableRelativeColumnsCacheBuilder builder = new TableRelativeColumnsCacheBuilder();
			ColumnsCache = new PivotRelativeColumnRowCache();
			TableRelativeColumnsRowsCache generalColumnsCache = new TableRelativeColumnsRowsCache();
			ColumnsCache.GeneralCache = generalColumnsCache;
			PivotTableLocationZone stripeZone = styleOwner.Location.Cache.ColumnStripe;
			int firstAbsoluteDataIndex = stripeZone.TopLeft.Column;
			int lastAbsoluteDataIndex = stripeZone.BottomRight.Column;
			if (styleOwner.Options.ShowColumnStripes) {
				ColumnStripesCache = new PivotStyleFormatRelativeColumnStripeInfoCache(styleOwner, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
				int stripeCount = firstStripeSize + secondStripeSize;
				builder.Build(leftAbsoluteIndex, rightAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, generalColumnsCache, notVisibleColumns, ColumnStripesCache, firstStripeSize, stripeCount);
			} else
				builder.Build(leftAbsoluteIndex, rightAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, generalColumnsCache, notVisibleColumns);
		}
		void CreateRelativeGeneralRowsCaches(IEnumerator<Row> notVisibleRows, int topAbsoluteIndex, int bottomAbsoluteIndex, int firstStripeSize, int secondStripeSize, IPivotStyleOwner styleOwner) {
			TableRelativeRowsCacheBuilder builder = new TableRelativeRowsCacheBuilder();
			RowsCache = new PivotRelativeColumnRowCache();
			TableRelativeColumnsRowsCache generalRowsCache = new TableRelativeColumnsRowsCache();
			RowsCache.GeneralCache = generalRowsCache;
			PivotTableLocationZone stripeZone = styleOwner.Location.Cache.RowStripe;
			int firstAbsoluteDataIndex = stripeZone.TopLeft.Row;
			int lastAbsoluteDataIndex = stripeZone.BottomRight.Row;
			if (styleOwner.Options.ShowRowStripes && styleOwner.RowFieldsCount > 0) {
				RowStripesCache = new PivotStyleFormatRelativeRowStripeInfoCache(styleOwner, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
				int stripeCount = firstStripeSize + secondStripeSize;
				builder.Build(topAbsoluteIndex, bottomAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, generalRowsCache, notVisibleRows, RowStripesCache, firstStripeSize, stripeCount);
			} else 
				builder.Build(topAbsoluteIndex, bottomAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex, generalRowsCache, notVisibleRows);
		}
		void CreateColumnsPageFieldsCache(Worksheet sheet, int leftAbsoluteIndex, int rightAbsoluteIndex) {
			IEnumerator<Column> notVisibleColumns = sheet.Columns.GetExistingNotVisibleColumnsEnumerator(leftAbsoluteIndex, rightAbsoluteIndex, false);
			if (notVisibleColumns.MoveNext()) {
				notVisibleColumns.Reset();
				CreateRelativePageFieldsCaches(new TableRelativeColumnsCacheBuilder(), ColumnsCache.PageFieldsCaches, notVisibleColumns, leftAbsoluteIndex, rightAbsoluteIndex);
			} else
				CreateAbsolutePageFieldsCache(ColumnsCache.PageFieldsCaches, leftAbsoluteIndex, rightAbsoluteIndex);
		}
		void CreateRowsPageFieldsCache(Worksheet sheet, int topAbsoluteIndex, int bottomAbsoluteIndex) {
			IEnumerator<Row> notVisibleRows = sheet.Rows.GetExistingNotVisibleRowsEnumerator(topAbsoluteIndex, bottomAbsoluteIndex, false);
			if (notVisibleRows.MoveNext()) {
				notVisibleRows.Reset();
				CreateRelativePageFieldsCaches(new TableRelativeRowsCacheBuilder(), RowsCache.PageFieldsCaches, notVisibleRows, topAbsoluteIndex, bottomAbsoluteIndex);
			} else
				CreateAbsolutePageFieldsCache(RowsCache.PageFieldsCaches, topAbsoluteIndex, bottomAbsoluteIndex);
		}
		void CreateRelativePageFieldsCaches<T>(TableRelativeColumnRowItemsCacheBuilderBase<T> builder, PivotPageFieldsCacheCollection pageFieldsCaches, IEnumerator<T> notVisibleItems, int startIndex, int lastIndex) {
			TableRelativeColumnsRowsCache pageFieldsCache = new TableRelativeColumnsRowsCache();
			pageFieldsCaches.Add(pageFieldsCache);
			builder.Build(startIndex, lastIndex, startIndex, lastIndex, pageFieldsCache, notVisibleItems);
		}
		#endregion
	}
	#endregion
} 
