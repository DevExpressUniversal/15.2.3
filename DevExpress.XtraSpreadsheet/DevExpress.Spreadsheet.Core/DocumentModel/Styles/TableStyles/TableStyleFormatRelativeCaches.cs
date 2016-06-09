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
	#region ITableRelativeRangeItemsCaches<TStyleOwner, TRelativeColumnRowCache>
	public interface ITableRelativeRangeItemsCaches<TStyleOwner, TRelativeColumnRowCache> {
		TRelativeColumnRowCache ColumnsCache { get; }
		TRelativeColumnRowCache RowsCache { get; }
		ITableStyleFormatStripeInfoCache ColumnStripesCache { get; }
		ITableStyleFormatStripeInfoCache RowStripesCache { get; }
		void Prepare(TStyleOwner styleOwner, TableStyleElementInfoCache styleCache);
		void Clear();
	} 
	#endregion
	#region TableRelativeColumnRowItemsCacheBuilderBase<T> (abstract class)
	public abstract class TableRelativeColumnRowItemsCacheBuilderBase<T> {
		#region Fields
		int currentRelativeIndex;
		int currentAbsoluteDifference;
		int currentAbsoluteIndex;
		bool shouldMoveNext;
		int currentRelativeStripeIndex;
		int lastCachedAbsoluteStripeIndex;
		int currentAbsoluteDataCount;
		int currentHeaderCount;
		bool hasFirstAbsoluteDataIndex;
		bool hasLastAbsoluteDataIndex;
		#endregion
		void BeginProcess(int firstAbsoluteIndex, int lastAbsoluteIndex, int firstAbsoluteDataIndex, int lastAbsoluteDataIndex) {
			currentAbsoluteDifference = lastAbsoluteIndex - firstAbsoluteIndex + 1;
			currentAbsoluteIndex = firstAbsoluteIndex;
			currentRelativeIndex = -1;
			currentRelativeStripeIndex = -1;
			lastCachedAbsoluteStripeIndex = -1;
			currentAbsoluteDataCount = lastAbsoluteDataIndex - firstAbsoluteDataIndex + 1;
			currentHeaderCount = firstAbsoluteDataIndex - firstAbsoluteIndex;
			hasFirstAbsoluteDataIndex = false;
			hasLastAbsoluteDataIndex = false;
			shouldMoveNext = true;
		}
		#region ProcessEnumerator
		void ProcessEnumerator(ITableRelativeColumnRowCache cache, IEnumerator<T> notVisibleItems) {
			T item = notVisibleItems.Current;
			int visibleItemsCount = GetVisibleIndexesCountBeforeItem(currentAbsoluteIndex, item);
			for (int i = 0; i < visibleItemsCount; i++) {
				ProcessEnumeratorCore(cache);
				currentAbsoluteIndex++;
			}
			int notVisibleItemsCount = GetNotVisibleIndexesCount(item);
			currentAbsoluteIndex += notVisibleItemsCount;
			DecreaseAbsoluteParamaters(notVisibleItemsCount);
		}
		void ProcessEnumerator(ITableRelativeColumnRowCache cache, IEnumerator<T> notVisibleItems, ITableStyleFormatStripeInfoCache stripeCache, int firstStripeSize, int stripeCount) {
			T item = notVisibleItems.Current;
			int visibleItemsCount = GetVisibleIndexesCountBeforeItem(currentAbsoluteIndex, item);
			for (int i = 0; i < visibleItemsCount; i++) {
				ProcessEnumeratorCore(cache);
				RegisterStripeInfo(stripeCache, firstStripeSize, stripeCount);
				currentAbsoluteIndex++;
			}
			int notVisibleItemsCount = GetNotVisibleIndexesCount(item);
			currentAbsoluteIndex += notVisibleItemsCount;
			DecreaseAbsoluteParamaters(notVisibleItemsCount);
		}
		void ProcessEnumeratorCore(ITableRelativeColumnRowCache cache) {
			currentRelativeIndex++;
			DecreaseAbsoluteParamaters(1);
			RegisterDataIndexes(cache);
			cache.RegisterRelativeIndex(currentAbsoluteIndex, currentRelativeIndex);
		}
		void DecreaseAbsoluteParamaters(int value) {
			currentAbsoluteDifference -= value;
			currentAbsoluteDataCount -= value;
			currentHeaderCount -= value;
		}
		#endregion
		#region ProcessWithoutEnumerator
		void ProcessWithoutEnumerator(int firstAbsoluteIndex, ITableRelativeColumnRowCache cache) {
			ProcessWithoutEnumeratorCore(firstAbsoluteIndex, cache);
			currentAbsoluteIndex++;
		}
		void ProcessWithoutEnumerator(int firstAbsoluteIndex, ITableRelativeColumnRowCache cache, ITableStyleFormatStripeInfoCache stripeCache, int firstStripeSize, int stripeCount) {
			ProcessWithoutEnumeratorCore(firstAbsoluteIndex, cache);
			RegisterStripeInfo(stripeCache, firstStripeSize, stripeCount);
			currentAbsoluteIndex++;
		}
		void ProcessWithoutEnumeratorCore(int firstAbsoluteIndex, ITableRelativeColumnRowCache cache) {
			shouldMoveNext = false;
			ProcessEnumeratorCore(cache);
		}
		#endregion
		#region EndProcess
		void EndProcess(ITableRelativeColumnRowCache cache) {
			cache.LastRelativeIndex = currentRelativeIndex;
			if (!hasLastAbsoluteDataIndex)
				cache.LastRelativeDataIndex = currentRelativeIndex;
		}
		void EndProcess(ITableRelativeColumnRowCache cache, ITableStyleFormatStripeInfoCache stripeCache, int firstStripeSize, int stripeCount) {
			cache.LastRelativeIndex = currentRelativeIndex;
			if (!hasLastAbsoluteDataIndex) {
				cache.LastRelativeDataIndex = currentRelativeIndex;
			}
			if (lastCachedAbsoluteStripeIndex != -1) {
				TableStyleStripeInfo stripeInfo = new TableStyleStripeInfo(currentRelativeStripeIndex, firstStripeSize, stripeCount, true);
				stripeCache.ReplaceStripeInfo(lastCachedAbsoluteStripeIndex, stripeInfo);
			}
		}
		#endregion
		#region Register methods
		void RegisterDataIndexes(ITableRelativeColumnRowCache cache) {
			if (!hasFirstAbsoluteDataIndex && currentHeaderCount < 0) {
				cache.FirstRelativeDataIndex = currentRelativeIndex;
				hasFirstAbsoluteDataIndex = true;
			}
			if (!hasLastAbsoluteDataIndex && currentAbsoluteDataCount < 0 && currentAbsoluteDifference == 0) {
				cache.LastRelativeDataIndex = currentRelativeIndex;
				hasLastAbsoluteDataIndex = true;
			}
		}
		void RegisterStripeInfo(ITableStyleFormatStripeInfoCache stripeCache, int firstStripeSize, int stripeCount) {
			if (stripeCache.ContainsIndex(currentAbsoluteIndex)) {
				currentRelativeStripeIndex++;
				TableStyleStripeInfo stripeInfo = new TableStyleStripeInfo(currentRelativeStripeIndex, firstStripeSize, stripeCount, hasLastAbsoluteDataIndex);
				stripeCache.RegisterStripeInfo(currentAbsoluteIndex, stripeInfo);
				lastCachedAbsoluteStripeIndex = currentAbsoluteIndex;
			}
		}
		#endregion
		#region Build
		public void Build(int firstAbsoluteIndex, int lastAbsoluteIndex, int firstAbsoluteDataIndex, int lastAbsoluteDataIndex, ITableRelativeColumnRowCache cache, IEnumerator<T> notVisibleItems) {
			BeginProcess(firstAbsoluteIndex, lastAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
			while (currentAbsoluteDifference > 0) {
				if (shouldMoveNext && notVisibleItems.MoveNext())
					ProcessEnumerator(cache, notVisibleItems);
				else
					ProcessWithoutEnumerator(firstAbsoluteIndex, cache);
			}
			EndProcess(cache);
		}
		public void Build(int firstAbsoluteIndex, int lastAbsoluteIndex, int firstAbsoluteDataIndex, int lastAbsoluteDataIndex, ITableRelativeColumnRowCache cache, IEnumerator<T> notVisibleItems, ITableStyleFormatStripeInfoCache stripeCache, int firstStripeSize, int stripeCount) {
			BeginProcess(firstAbsoluteIndex, lastAbsoluteIndex, firstAbsoluteDataIndex, lastAbsoluteDataIndex);
			currentRelativeStripeIndex = -1;
			lastCachedAbsoluteStripeIndex = -1;
			while (currentAbsoluteDifference > 0) {
				if (shouldMoveNext && notVisibleItems.MoveNext())
					ProcessEnumerator(cache, notVisibleItems, stripeCache, firstStripeSize, stripeCount);
				else
					ProcessWithoutEnumerator(firstAbsoluteIndex, cache, stripeCache, firstStripeSize, stripeCount);
			}
			EndProcess(cache, stripeCache, firstStripeSize, stripeCount);
		}
		#endregion
		protected abstract int GetVisibleIndexesCountBeforeItem(int currentAbsoluteIndex, T notVisibleItem);
		protected abstract int GetNotVisibleIndexesCount(T notVisibleItem);
	}
	#endregion
	#region TableRelativeColumnsCacheBuilder
	public class TableRelativeColumnsCacheBuilder : TableRelativeColumnRowItemsCacheBuilderBase<Column> {
		#region TableRelativeColumnRowItemsCacheBuilderBase<Column> Members
		protected override int GetVisibleIndexesCountBeforeItem(int currentAbsoluteIndex, Column notVisibleItem) {
			return notVisibleItem.StartIndex - currentAbsoluteIndex;
		}
		protected override int GetNotVisibleIndexesCount(Column notVisibleItem) {
			return notVisibleItem.EndIndex - notVisibleItem.StartIndex + 1;
		}
		#endregion
	}
	#endregion
	#region TableRelativeRowsCacheBuilder
	public class TableRelativeRowsCacheBuilder : TableRelativeColumnRowItemsCacheBuilderBase<Row> {
		#region TableRelativeColumnRowItemsCacheBuilderBase<Row> Members
		protected override int GetVisibleIndexesCountBeforeItem(int currentAbsoluteIndex, Row notVisibleItem) {
			return notVisibleItem.Index - currentAbsoluteIndex;
		}
		protected override int GetNotVisibleIndexesCount(Row notVisibleItem) {
			return 1;
		}
		#endregion
	}
	#endregion
} 
