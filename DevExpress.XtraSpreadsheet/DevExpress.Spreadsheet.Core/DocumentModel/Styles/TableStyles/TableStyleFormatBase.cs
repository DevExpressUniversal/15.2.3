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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ITableStyleFormatCache
	public interface ITableStyleFormatCache {
		bool IsValid { get; }
		void SetInvalid();
		void Prepare(TableStyleElementInfoCache styleCache);
		ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition absoluteVisiblePosition, TableStyleElementInfoCache styleCache);
	}
	#endregion
	#region TableStyleFormatCacheBase<TStyleOwner, TRelativeColumnRowCache> (abstract class)
	public abstract class TableStyleFormatCacheBase<TStyleOwner, TRelativeColumnRowCache> : ITableStyleFormatCache {
		#region Fields
		readonly TStyleOwner owner;
		readonly ITableRelativeRangeItemsCaches<TStyleOwner, TRelativeColumnRowCache> relative;
		readonly TableStyleFormatRangeHelperCacheBase<TStyleOwner> rangeHelpers;
		bool isValid = false;
		#endregion
		protected TableStyleFormatCacheBase(TStyleOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			relative = GetInstanceRelative();
			rangeHelpers = GetInstanceRangeHelpers();
		}
		#region Properties
		public bool IsValid { get { return isValid; } }
		public ITableRelativeRangeItemsCaches<TStyleOwner, TRelativeColumnRowCache> Relative { get { return relative; } }
		public TableStyleFormatRangeHelperCacheBase<TStyleOwner> RangeHelpers { get { return rangeHelpers; } }
		protected TStyleOwner Owner { get { return owner; } }
		protected abstract bool CheckBuildFormatting { get; }
		#endregion
		public void SetInvalid() {
			if (isValid) {
				Clear();
				isValid = false;
			}
		}
		public void Prepare(TableStyleElementInfoCache styleCache) {
			Guard.ArgumentNotNull(styleCache, "styleCache");
			SetInvalid();
			PrepareCore(styleCache);
			isValid = true;
		}
		protected virtual void PrepareCore(TableStyleElementInfoCache styleCache) {
			relative.Prepare(owner, styleCache);
			rangeHelpers.Prepare(owner, styleCache);
		}
		protected virtual void Clear() {
			relative.Clear();
			rangeHelpers.Clear();
		}
		public ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition absoluteVisiblePosition, TableStyleElementInfoCache styleCache) {
			ActualTableStyleCellFormatting result = new ActualTableStyleCellFormatting();
			if (!CheckBuildFormatting)
				return result;
			if (!IsValid)
				Prepare(styleCache);
			if (CheckVisiblePosition(absoluteVisiblePosition))
				rangeHelpers.ProcessFormatting(result, absoluteVisiblePosition, owner, styleCache);
			return result;
		}
		protected abstract ITableRelativeRangeItemsCaches<TStyleOwner, TRelativeColumnRowCache> GetInstanceRelative();
		protected abstract TableStyleFormatRangeHelperCacheBase<TStyleOwner> GetInstanceRangeHelpers();
		protected abstract bool CheckVisiblePosition(CellPosition absolutePosition);
	}
	#endregion
	#region ITableStyleFormatRangeHelper<TStyleOwner>
	public interface ITableStyleFormatRangeHelper<TStyleOwner> {
		bool CheckPrepare(TStyleOwner owner, TableStyleElementInfoCache styleCache);
		void RegisterPositionInfo(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, TStyleOwner owner, TableStyleElementInfoCache styleCache);
	}
	#endregion 
	#region TableStyleFormatRangeHelperCacheBase (abstract class)
	public abstract class TableStyleFormatRangeHelperCacheBase<TStyleOwner> {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected void PrepareCore(IList<ITableStyleFormatRangeHelper<TStyleOwner>> targetCache, IEnumerable<ITableStyleFormatRangeHelper<TStyleOwner>> orderedHelpers, TStyleOwner owner, TableStyleElementInfoCache styleCache) {
			foreach (ITableStyleFormatRangeHelper<TStyleOwner> helper in orderedHelpers)
				if (helper.CheckPrepare(owner, styleCache))
					targetCache.Add(helper);
		}
		public void ProcessFormatting(ActualTableStyleCellFormatting formatting, CellPosition absolutePosition, TStyleOwner owner, TableStyleElementInfoCache styleCache) {
			IEnumerable helpers = GetInnerEnumerable(absolutePosition, owner);
			foreach (ITableStyleFormatRangeHelper<TStyleOwner> helper in helpers) 
				helper.RegisterPositionInfo(formatting, absolutePosition, owner, styleCache);
		}
		public abstract void Prepare(TStyleOwner owner, TableStyleElementInfoCache styleCache);
		public abstract void Clear();
		protected abstract IEnumerable GetInnerEnumerable(CellPosition absolutePosition, TStyleOwner owner);
	} 
	#endregion
	#region TableStyleFormatRangeHelperCache
	public class TableStyleFormatRangeHelperCache : TableStyleFormatRangeHelperCacheBase<ITableStyleOwner> {
		#region Static Members
		static IEnumerable<ITableStyleFormatRangeHelper<ITableStyleOwner>> OrderedRangeHelpers = GetOrderedRangeHelpers();
		static IEnumerable<ITableStyleFormatRangeHelper<ITableStyleOwner>> GetOrderedRangeHelpers() {
			List<ITableStyleFormatRangeHelper<ITableStyleOwner>> result = new List<ITableStyleFormatRangeHelper<ITableStyleOwner>>();
			result.Add(new TableStyleFormatFirstColumnCellsRangeHelper());
			result.Add(new TableStyleFormatLastColumnCellsRangeHelper());
			result.Add(new TableStyleFormatHeaderTotalRowRangeHelper());
			result.Add(new TableStyleFormatFirstColumnRangeHelper());
			result.Add(new TableStyleFormatLastColumnRangeHelper());
			result.Add(new TableStyleFormatRowStripeRangeHelper());
			result.Add(new TableStyleFormatColumnStripeRangeHelper());
			result.Add(new TableStyleFormatWholeTableRangeHelper());
			return result;
		}
		#endregion
		IList<ITableStyleFormatRangeHelper<ITableStyleOwner>> innerTable = new List<ITableStyleFormatRangeHelper<ITableStyleOwner>>();
		protected internal bool IsEmpty { get { return innerTable.Count == 0; } }
		#region TableStyleFormatRangeHelperCacheBase<ITableStyleOwner> Members
		public override void Prepare(ITableStyleOwner owner, TableStyleElementInfoCache styleCache) {
			PrepareCore(innerTable, OrderedRangeHelpers, owner, styleCache);
		}
		protected override IEnumerable GetInnerEnumerable(CellPosition absolutePosition, ITableStyleOwner owner) {
			return innerTable;
		}
		public override void Clear() {
			innerTable.Clear();
		}
		#endregion
	}
	#endregion
	#region PivotStyleFormatRangeHelperCache
	public class PivotStyleFormatRangeHelperCache : TableStyleFormatRangeHelperCacheBase<IPivotStyleOwner> {
		#region Static Members
		static IEnumerable<ITableStyleFormatRangeHelper<IPivotStyleOwner>> GeneralOrderedRangeHelpers = GetGeneralOrderedRangeHelpers();
		static IEnumerable<ITableStyleFormatRangeHelper<IPivotStyleOwner>> GetGeneralOrderedRangeHelpers() {
			List<ITableStyleFormatRangeHelper<IPivotStyleOwner>> result = new List<ITableStyleFormatRangeHelper<IPivotStyleOwner>>();
			result.Add(new PivotStyleFormatZoneHelper());
			result.Add(new PivotStyleFormatFirstHeaderCellRangeHelper());
			result.Add(new PivotStyleFormatHeaderRowRangeHelper());
			result.Add(new PivotStyleFormatFirstColumnRangeHelper());
			result.Add(new PivotStyleFormatRowStripeRangeHelper());
			result.Add(new PivotStyleFormatColumnStripeRangeHelper());
			result.Add(new PivotStyleFormatWholeTableRangeHelper());
			return result;
		}
		static IEnumerable<ITableStyleFormatRangeHelper<IPivotStyleOwner>> PageFieldsOrderedRangeHelpers = GetPageFieldsOrderedRangeHelpers();
		static IEnumerable<ITableStyleFormatRangeHelper<IPivotStyleOwner>> GetPageFieldsOrderedRangeHelpers() {
			List<ITableStyleFormatRangeHelper<IPivotStyleOwner>> result = new List<ITableStyleFormatRangeHelper<IPivotStyleOwner>>();
			result.Add(new PivotStyleFormatPageFieldsRangeHelper());
			result.Add(new PivotStyleFormatWholeTablePageFieldsRangeHelper());
			return result;
		}
		#endregion
		#region Fields
		IList<ITableStyleFormatRangeHelper<IPivotStyleOwner>> generalTable = new List<ITableStyleFormatRangeHelper<IPivotStyleOwner>>();
		IList<ITableStyleFormatRangeHelper<IPivotStyleOwner>> pageFiltersTable = new List<ITableStyleFormatRangeHelper<IPivotStyleOwner>>();
		#endregion
		protected internal bool IsEmpty { get { return generalTable.Count == 0 && pageFiltersTable.Count == 0; } }
		#region TableStyleFormatRangeHelperCacheBase<IPivotStyleOwner, PivotStyleFormatRangeCache> Members
		protected override IEnumerable GetInnerEnumerable(CellPosition absolutePosition, IPivotStyleOwner owner) {
			if (owner.Location.Cache.General.ContainsCell(absolutePosition.Column, absolutePosition.Row))
				return generalTable;
			return pageFiltersTable;
		}
		public override void Prepare(IPivotStyleOwner owner, TableStyleElementInfoCache styleCache) {
			PrepareCore(generalTable, GeneralOrderedRangeHelpers, owner, styleCache);
			PrepareCore(pageFiltersTable, PageFieldsOrderedRangeHelpers, owner, styleCache);
		}
		public override void Clear() {
			generalTable.Clear();
			pageFiltersTable.Clear();
		}
		#endregion
	}
	#endregion
} 
