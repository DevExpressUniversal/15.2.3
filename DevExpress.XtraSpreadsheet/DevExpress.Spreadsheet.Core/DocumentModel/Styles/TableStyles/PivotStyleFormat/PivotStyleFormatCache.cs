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

using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotStyleFormatCache
	public class PivotStyleFormatCache : TableStyleFormatCacheBase<IPivotStyleOwner, IPivotRelativeColumnRowCache> {
		public PivotStyleFormatCache(IPivotStyleOwner owner)
			: base(owner) {
		}
		public PivotZoneFormattingCache PivotZoneCache { get { return Owner.PivotZoneCache; } }
		public PivotTableLocationCache LocationCache { get { return Owner.Location.Cache; } }
		protected override void PrepareCore(TableStyleElementInfoCache styleCache) {
			int columnFieldsCount = Owner.ColumnFieldsCount;
			int rowFieldsCount = Owner.RowFieldsCount;
			int dataFieldsCount = Owner.DataFieldsCount;
			Owner.PivotZoneCache = new PivotZoneFormattingCache(Owner.Options, columnFieldsCount, rowFieldsCount, styleCache);
			IPivotTableLocation location = Owner.Location;
			location.Cache.Prepare(location, rowFieldsCount > 0);
			base.PrepareCore(styleCache);
		}
		protected override void Clear() {
			base.Clear();
			Owner.PivotZoneCache = new PivotZoneFormattingCache();
			Owner.Location.Cache.SetInvalid();
		}
		#region TableStyleFormatCacheBase<IPivotStyleOwner, IPivotRelativeColumnRowCache> Members
		protected override bool CheckBuildFormatting { get { return Owner.ColumnFieldsCount > 0 || Owner.RowFieldsCount > 0 || Owner.DataFieldsCount > 0 || Owner.PageFieldsCount > 0; } }
		protected override ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> GetInstanceRelative() {
			return new PivotRelativeRangeItemsCacheCollection();
		}
		protected override TableStyleFormatRangeHelperCacheBase<IPivotStyleOwner> GetInstanceRangeHelpers() {
			return new PivotStyleFormatRangeHelperCache();
		}
		protected override bool CheckVisiblePosition(CellPosition absolutePosition) {
			int column = absolutePosition.Column;
			int row = absolutePosition.Row;
			if (Owner.Location.Cache.General.ContainsCell(column, row))
				return CheckVisibleGeneralPosition(column, row);
			else 
				return CheckVisiblePageFieldsPosition(column, row);
		}
		#endregion
		protected virtual bool CheckVisibleGeneralPosition(int column, int row) {
			return
				Relative.ColumnsCache.GeneralCache.HasVisibleColumnRowIndex(column) &&
				Relative.RowsCache.GeneralCache.HasVisibleColumnRowIndex(row);
		}
		protected virtual bool CheckVisiblePageFieldsPosition(int column, int row) {
			if (!Owner.Location.Cache.PageFieldsZonesCache.IsEmpty) {
				int index = Owner.Location.Cache.PageFieldsZonesCache.TryGetIndexByCellPosition(column, row);
				if (index != -1)
					return
						Relative.ColumnsCache.PageFieldsCaches.GetCache(index).HasVisibleColumnRowIndex(column) &&
						Relative.RowsCache.PageFieldsCaches.GetCache(index).HasVisibleColumnRowIndex(row);
			}
			return false;
		}
	}
	#endregion
	#region PivotStyleFormatViewInfoCache
	public class PivotStyleFormatViewInfoCache : PivotStyleFormatCache {
		public PivotStyleFormatViewInfoCache(IPivotStyleOwner owner)
			: base(owner) {
		}
		#region TableStyleFormatCacheBase<IPivotStyleOwner, IPivotRelativeColumnRowCache> Members
		protected override ITableRelativeRangeItemsCaches<IPivotStyleOwner, IPivotRelativeColumnRowCache> GetInstanceRelative() {
			return new PivotAbsoluteRangeItemsCacheCollectionForPivotStyleViewInfo();
		}
		protected override bool CheckVisiblePosition(CellPosition absolutePosition) {
			return true;
		}
		#endregion
	}
	#endregion
	#region PivotZoneFormattingCache (struct)
	public struct PivotZoneFormattingCache {
		#region Fields
		const uint hasGrandTotalRowFormattingMask = 0x01;
		const uint hasGrandTotalColumnFormattingMask = 0x02;
		const uint hasFirstRowSubheadingFormattingMask = 0x04;
		const uint hasSecondRowSubheadingFormattingMask = 0x08;
		const uint hasThirdRowSubheadingFormattingMask = 0x10;
		const uint hasFirstColumnSubheadingFormattingMask = 0x20;
		const uint hasSecondColumnSubheadingFormattingMask = 0x40;
		const uint hasThirdColumnSubheadingFormattingMask = 0x80;
		const uint hasFirstSubtotalRowFormattingMask = 0x100;
		const uint hasSecondSubtotalRowFormattingMask = 0x200;
		const uint hasThirdSubtotalRowFormattingMask = 0x400;
		const uint hasBlankRowFormattingMask = 0x800;
		const uint hasFirstSubtotalColumnFormattingMask = 0x1000;
		const uint hasSecondSubtotalColumnFormattingMask = 0x2000;
		const uint hasThirdSubtotalColumnFormattingMask = 0x4000;
		uint packedValues;
		#endregion
		public PivotZoneFormattingCache(IPivotStyleOptions options, int columnFieldsCount, int rowFieldsCount, TableStyleElementInfoCache styleCache) {
			packedValues = 0;
			InitProperties(options, columnFieldsCount, rowFieldsCount, styleCache);
		}
		void InitProperties(IPivotStyleOptions options, int columnFieldsCount, int rowFieldsCount, TableStyleElementInfoCache styleCache) {
			HasGrandTotalRowFormatting = styleCache.HasDifferentialFormatting(TableStyle.TotalRowIndex) && options.HasColumnGrandTotals; 
			HasGrandTotalColumnFormatting = styleCache.HasDifferentialFormatting(TableStyle.LastColumnIndex) && options.HasRowGrandTotals; 
			HasBlankRowFormatting = styleCache.HasDifferentialFormatting(TableStyle.BlankRowIndex);
			if (rowFieldsCount > 1) {
				HasFirstRowSubheadingFormatting = options.ShowRowHeaders && styleCache.HasDifferentialFormatting(TableStyle.FirstRowSubheadingIndex);
				HasFirstSubtotalRowFormatting = styleCache.HasDifferentialFormatting(TableStyle.FirstSubtotalRowIndex);
			}
			if (rowFieldsCount > 2) {
				HasSecondRowSubheadingFormatting = options.ShowRowHeaders && styleCache.HasDifferentialFormatting(TableStyle.SecondRowSubheadingIndex);
				HasSecondSubtotalRowFormatting = styleCache.HasDifferentialFormatting(TableStyle.SecondSubtotalRowIndex);
			}
			if (rowFieldsCount > 3) {
				HasThirdRowSubheadingFormatting = options.ShowRowHeaders && styleCache.HasDifferentialFormatting(TableStyle.ThirdRowSubheadingIndex);
				HasThirdSubtotalRowFormatting = styleCache.HasDifferentialFormatting(TableStyle.ThirdSubtotalRowIndex);
			}
			if (columnFieldsCount > 0) {
				HasFirstColumnSubheadingFormatting = options.ShowColumnHeaders && styleCache.HasDifferentialFormatting(TableStyle.FirstColumnSubheadingIndex);
				HasFirstSubtotalColumnFormatting = styleCache.HasDifferentialFormatting(TableStyle.FirstSubtotalColumnIndex);
			}
			if (columnFieldsCount > 1) {
				HasSecondColumnSubheadingFormatting = options.ShowColumnHeaders && styleCache.HasDifferentialFormatting(TableStyle.SecondColumnSubheadingIndex);
				HasSecondSubtotalColumnFormatting = styleCache.HasDifferentialFormatting(TableStyle.SecondSubtotalColumnIndex);
			}
			if (columnFieldsCount > 2) {
				HasThirdColumnSubheadingFormatting = options.ShowColumnHeaders && styleCache.HasDifferentialFormatting(TableStyle.ThirdColumnSubheadingIndex);
				HasThirdSubtotalColumnFormatting = styleCache.HasDifferentialFormatting(TableStyle.ThirdSubtotalColumnIndex);
			}
		}
		#region Properties
		public bool HasGrandTotalRowFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasGrandTotalRowFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasGrandTotalRowFormattingMask, value); }
		}
		public bool HasGrandTotalColumnFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasGrandTotalColumnFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasGrandTotalColumnFormattingMask, value); }
		}
		public bool HasFirstRowSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasFirstRowSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasFirstRowSubheadingFormattingMask, value); }
		}
		public bool HasSecondRowSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasSecondRowSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasSecondRowSubheadingFormattingMask, value); }
		}
		public bool HasThirdRowSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasThirdRowSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasThirdRowSubheadingFormattingMask, value); }
		}
		public bool HasFirstColumnSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasFirstColumnSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasFirstColumnSubheadingFormattingMask, value); }
		}
		public bool HasSecondColumnSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasSecondColumnSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasSecondColumnSubheadingFormattingMask, value); }
		}
		public bool HasThirdColumnSubheadingFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasThirdColumnSubheadingFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasThirdColumnSubheadingFormattingMask, value); }
		}
		public bool HasFirstSubtotalRowFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasFirstSubtotalRowFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasFirstSubtotalRowFormattingMask, value); }
		}
		public bool HasSecondSubtotalRowFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasSecondSubtotalRowFormattingMask); }
			set { PackedValues.SetBoolBitValue(ref packedValues, hasSecondSubtotalRowFormattingMask, value); }
		}
		public bool HasThirdSubtotalRowFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasThirdSubtotalRowFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasThirdSubtotalRowFormattingMask, value); }
		}
		public bool HasBlankRowFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasBlankRowFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasBlankRowFormattingMask, value); }
		}
		public bool HasFirstSubtotalColumnFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasFirstSubtotalColumnFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasFirstSubtotalColumnFormattingMask, value); }
		}
		public bool HasSecondSubtotalColumnFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasSecondSubtotalColumnFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasSecondSubtotalColumnFormattingMask, value); }
		}
		public bool HasThirdSubtotalColumnFormatting {
			get { return PackedValues.GetBoolBitValue(packedValues, hasThirdSubtotalColumnFormattingMask); }
			private set { PackedValues.SetBoolBitValue(ref packedValues, hasThirdSubtotalColumnFormattingMask, value); }
		}
		public bool IsDefault { get { return packedValues == 0; } }
		#endregion
	}
	#endregion
}   
