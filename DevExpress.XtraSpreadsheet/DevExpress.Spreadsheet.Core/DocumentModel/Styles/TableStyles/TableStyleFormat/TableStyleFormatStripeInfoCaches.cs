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
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ITableStyleFormatStripeInfoCache
	public interface ITableStyleFormatStripeInfoCache {
		int FirstAbsoluteIndex { get; }
		int LastAbsoluteIndex { get; }
		void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo);
		void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo);
		TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex);
		bool ContainsIndex(int absoluteIndex);
	}
	#endregion
	#region TableStyleFormatRelativeColumnStripeInfoCacheBase<TStyleOwner> (abstract class)
	public abstract class TableStyleFormatRelativeColumnStripeInfoCacheBase<TStyleOwner> : ITableStyleFormatStripeInfoCache {
		#region Fields
		readonly TStyleOwner owner;
		readonly int firstAbsoluteIndex;
		readonly int lastAbsoluteIndex;
		#endregion
		protected TableStyleFormatRelativeColumnStripeInfoCacheBase(TStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.firstAbsoluteIndex = firstAbsoluteIndex;
			this.lastAbsoluteIndex = lastAbsoluteIndex;
		}
		#region ITableStyleFormatStripeInfoCache
		protected TStyleOwner Owner { get { return owner; } }
		public int FirstAbsoluteIndex { get { return firstAbsoluteIndex; } }
		public int LastAbsoluteIndex { get { return lastAbsoluteIndex; } }
		public abstract void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo);
		public abstract void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo);
		public abstract TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex);
		public bool ContainsIndex(int absoluteIndex) {
			return firstAbsoluteIndex <= absoluteIndex && absoluteIndex <= lastAbsoluteIndex;
		}
		#endregion
	}
	#endregion
	#region TableStyleFormatRelativeColumnStripeInfoCache
	public class TableStyleFormatRelativeColumnStripeInfoCache : TableStyleFormatRelativeColumnStripeInfoCacheBase<ITableStyleOwner> {
		public TableStyleFormatRelativeColumnStripeInfoCache(ITableStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex)
			: base(owner, firstAbsoluteIndex, lastAbsoluteIndex) {
		}
		int GetTableColumnIndex(int absoluteIndex) {
			return absoluteIndex - Owner.Range.LeftColumnIndex;
		}
		#region TableStyleFormatRelativeColumnStripeInfoCacheBase<TableStyleFormatCache> Memrbers
		public override void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			int tableColumnIndex = GetTableColumnIndex(absoluteDataIndex);
			Owner.CacheColumnStripeInfo(tableColumnIndex, stripeInfo);
		}
		public override void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			RegisterStripeInfo(absoluteDataIndex, stripeInfo);
		}
		public override TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex) {
			int tableColumnIndex = GetTableColumnIndex(absoluteDataIndex);
			return Owner.GetColumnStripeInfo(tableColumnIndex);
		}
		#endregion
	}
	#endregion
	#region TableStyleFormatRelativeRowStripeInfoCache
	public class TableStyleFormatRelativeRowStripeInfoCache : TableStyleFormatRelativeColumnStripeInfoCacheBase<ITableStyleOwner> {
		readonly Dictionary<int, TableStyleStripeInfo> absoluteRowIndexToStripeTranslationTable;
		public TableStyleFormatRelativeRowStripeInfoCache(ITableStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex)
			: base(owner, firstAbsoluteIndex, lastAbsoluteIndex) {
			absoluteRowIndexToStripeTranslationTable = new Dictionary<int, TableStyleStripeInfo>();
		}
		protected internal Dictionary<int, TableStyleStripeInfo> InnerTable { get { return absoluteRowIndexToStripeTranslationTable; } }
		#region TableStyleFormatRelativeColumnStripeInfoCacheBase<TableStyleFormatCache> Members
		public override void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			absoluteRowIndexToStripeTranslationTable.Add(absoluteDataIndex, stripeInfo);
		}
		public override void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			absoluteRowIndexToStripeTranslationTable[absoluteDataIndex] = stripeInfo;
		}
		public override TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex) {
			return absoluteRowIndexToStripeTranslationTable[absoluteDataIndex];
		}
		#endregion
	}
	#endregion
	#region TableStyleFormatAbsoluteStripeInfoCache
	public class TableStyleFormatAbsoluteStripeInfoCache : ITableStyleFormatStripeInfoCache {
		readonly int firstAbsoluteIndex;
		readonly int lastAbsoluteIndex;
		readonly int firstStripeSize;
		readonly int stripeCount;
		public TableStyleFormatAbsoluteStripeInfoCache(int firstAbsoluteIndex, int lastAbsoluteIndex, int firstStripeSize, int stripeCount) {
			this.firstAbsoluteIndex = firstAbsoluteIndex;
			this.lastAbsoluteIndex = lastAbsoluteIndex;
			this.firstStripeSize = firstStripeSize;
			this.stripeCount = stripeCount;
		}
		#region ITableStyleFormatStripeInfoCache
		public int FirstAbsoluteIndex { get { return firstAbsoluteIndex; } }
		public int LastAbsoluteIndex { get { return lastAbsoluteIndex; } }
		public void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
		}
		public void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
		}
		public TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex) {
			int relativeIndex = absoluteDataIndex - firstAbsoluteIndex;
			bool isLastIndex = absoluteDataIndex == lastAbsoluteIndex;
			return new TableStyleStripeInfo(relativeIndex, firstStripeSize, stripeCount, isLastIndex);
		}
		public bool ContainsIndex(int absoluteIndex) {
			return firstAbsoluteIndex <= absoluteIndex && absoluteIndex <= lastAbsoluteIndex;
		}
		#endregion
	}
	#endregion
} 
