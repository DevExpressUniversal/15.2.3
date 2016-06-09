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
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid.Native {
	public class DetailScrollInfoCache {
		#region ExpandedDetailInfo
		public struct ExpandedDetailInfo {
			public ExpandedDetailInfo(int commonScrollIndex, int localScrollIndex, int detailRowsCount, int index, 
				int commonVisibleIndex, int localVisibleIndex, int visibleRowCount, int visibleRowCountBeforeRow) {
				this.CommonScrollIndex = commonScrollIndex;
				this.LocalScrollIndex = localScrollIndex;
				this.ScrollRowCount = detailRowsCount;
				this.Index = index;
				this.CommonVisibleIndex = commonVisibleIndex;
				this.LocalVisibleIndex = localVisibleIndex;
				this.VisibleRowCount = visibleRowCount;
				this.VisibleDetailRowCountBeforeRow = visibleRowCountBeforeRow;
			}
			public readonly int CommonScrollIndex;
			public readonly int LocalScrollIndex;
			public readonly int ScrollRowCount;
			public readonly int Index;
			public int ScrollRowCountBeforeRow { get { return CommonScrollIndex - LocalScrollIndex; } }
			public int ScrollRowCountBeforeNextRow { get { return ScrollRowCountBeforeRow + ScrollRowCount; } }
			public int LastDetailRowCommonScrollIndex { get { return CommonScrollIndex + ScrollRowCount; } }
			public readonly int LocalVisibleIndex;
			public readonly int CommonVisibleIndex;
			public readonly int VisibleRowCount;
			public int VisibleDetailRowCountBeforeNextRow { get { return VisibleDetailRowCountBeforeRow + VisibleRowCount; } }
			public readonly int VisibleDetailRowCountBeforeRow;
			public int LastDetailRowCommonVisibleIndex { get { return CommonVisibleIndex + VisibleRowCount - 1; } }
		}
		#endregion
		bool isCacheValid = false;
		int ScrollDetailRowsCount;
		int VisibleDetailRowCount;
#if DEBUGTEST
		public List<ExpandedDetailInfo> ExpandedDetailInfoList { get { return expandedDetailInfoList; } }
#endif
		readonly List<ExpandedDetailInfo> expandedDetailInfoList = new List<ExpandedDetailInfo>();
		readonly List<int> expandedDetailsVisibleIndices = new List<int>();
		readonly MasterDetailProvider owner;
		DataViewBase View { get { return owner.View; } }
		readonly CalcVisibleDetailRowsCountBeforeRowComparer calcScrollDetailRowsCountBeforeRowComparer;
		readonly CalcVisibleDetailRowsCountBeforeRowComparer calcVisibleDetailRowsCountBeforeRowComparer;
		readonly CalcMasterRowInfoComparer calcMasterRowScrollInfoComparer;
		readonly CalcMasterRowInfoComparer calcMasterRowNavigationInfoComparer;
		public DetailScrollInfoCache(MasterDetailProvider owner) {
			this.owner = owner;
			this.calcScrollDetailRowsCountBeforeRowComparer = new CalcVisibleDetailRowsCountBeforeRowComparer(expandedDetailInfoList, info => info.LocalScrollIndex);
			this.calcVisibleDetailRowsCountBeforeRowComparer = new CalcVisibleDetailRowsCountBeforeRowComparer(expandedDetailInfoList, info => info.LocalVisibleIndex);
			this.calcMasterRowScrollInfoComparer = new CalcMasterRowInfoComparer(expandedDetailInfoList, info => info.CommonScrollIndex, info => info.LastDetailRowCommonScrollIndex);
			this.calcMasterRowNavigationInfoComparer = new CalcMasterRowInfoComparer(expandedDetailInfoList, info => info.CommonVisibleIndex, info => info.LastDetailRowCommonVisibleIndex);
		}
		public void InvalidateCache() {
			isCacheValid = false;
		}
		public int CalcScrollDetailRowsCount() {
			ValidateCache();
			return ScrollDetailRowsCount;
		}
		public int CalcVisibleDetailDataRowCount() {
			ValidateCache();
			return VisibleDetailRowCount;
		}
		void ValidateCache() {
			if(isCacheValid)
				return;
#if DEBUGTEST
			owner.RecalсDetailScrollInfoCacheCount++;
#endif
			isCacheValid = true;
			expandedDetailsVisibleIndices.Clear();
			foreach(int listIndex in View.DataProviderBase.GetRowListIndicesWithExpandedDetails()) {
				int rowHandle = View.DataProviderBase.GetRowHandleByListIndex(listIndex);
				if(rowHandle == DataControlBase.InvalidRowHandle ) 
					continue;
				int visibleIndex = View.DataProviderBase.GetRowVisibleIndexByHandle(rowHandle);
				if(visibleIndex >= 0) 
					expandedDetailsVisibleIndices.Add(visibleIndex);
			}
			expandedDetailsVisibleIndices.Sort();
			int expandedDetailsCount = expandedDetailsVisibleIndices.Count;
			ScrollDetailRowsCount = 0;
			VisibleDetailRowCount = 0;
			expandedDetailInfoList.Clear();
			for(int i = 0; i < expandedDetailsCount; i++) {
				int localVisibleIndex = expandedDetailsVisibleIndices[i];
				int scrollIndex = View.DataProviderBase.ConvertVisibleIndexToScrollIndex(localVisibleIndex, View.AllowFixedGroupsCore);
				int rowHandle = View.DataControl.GetRowHandleByVisibleIndexCore(localVisibleIndex);
				int currentRowScrollDetailRowsCount = owner.CalcVisibleDetailRowsCountForRow(rowHandle);
				int currentRowVisibleDetailRowsCount = owner.CalcVisibleDataRowCountForRow(rowHandle);
				int commonVisibleIndex = localVisibleIndex + VisibleDetailRowCount + 1;
				int visibleRowCountBeforeRow = commonVisibleIndex - localVisibleIndex - 1;
				if(View.IsNewItemRowVisible)
					commonVisibleIndex++;
				expandedDetailInfoList.Add(new ExpandedDetailInfo(scrollIndex + ScrollDetailRowsCount, scrollIndex, currentRowScrollDetailRowsCount, i,
					commonVisibleIndex, localVisibleIndex, currentRowVisibleDetailRowsCount, visibleRowCountBeforeRow));
				ScrollDetailRowsCount += currentRowScrollDetailRowsCount;
				VisibleDetailRowCount += currentRowVisibleDetailRowsCount;
			}
		}
		class CalcVisibleDetailRowsCountBeforeRowComparer : IComparer<ExpandedDetailInfo> {
			readonly List<ExpandedDetailInfo> expandedDetailInfoList;
			internal int Index;
			readonly Func<ExpandedDetailInfo, int> getIndex;
			public CalcVisibleDetailRowsCountBeforeRowComparer(List<ExpandedDetailInfo> expandedDetailInfoList, Func<ExpandedDetailInfo, int> getIndex) {
				this.expandedDetailInfoList = expandedDetailInfoList;
				this.getIndex = getIndex;
			}
			int IComparer<ExpandedDetailInfo>.Compare(ExpandedDetailInfo item, ExpandedDetailInfo ignore) {
				if(Index < getIndex(item))
					return +1;
				int listIndex = item.Index;
				if(listIndex < expandedDetailInfoList.Count - 1 && Index >= getIndex(expandedDetailInfoList[listIndex + 1]))
					return -1;
				return 0;
			}
		}
		public int CalcScrollDetailRowsCountBeforeRow(int scrollIndex) {
			ValidateCache();
			calcScrollDetailRowsCountBeforeRowComparer.Index = scrollIndex;
			int listIndex = expandedDetailInfoList.BinarySearch(default(ExpandedDetailInfo), calcScrollDetailRowsCountBeforeRowComparer);
			if(listIndex < 0)
				return 0;
			ExpandedDetailInfo info = expandedDetailInfoList[listIndex];
			if(scrollIndex > info.LocalScrollIndex) {
				return info.ScrollRowCountBeforeRow + info.ScrollRowCount;
			}
			return info.ScrollRowCountBeforeRow;
		}
		public int CalcVisibleDetailRowsCountBeforeRow(int visibleIndex) {
			ValidateCache();
			calcVisibleDetailRowsCountBeforeRowComparer.Index = visibleIndex;
			int listIndex = expandedDetailInfoList.BinarySearch(default(ExpandedDetailInfo), calcVisibleDetailRowsCountBeforeRowComparer);
			if(listIndex < 0)
				return 0;
			ExpandedDetailInfo info = expandedDetailInfoList[listIndex];
			if(visibleIndex > info.LocalVisibleIndex) {
				return info.VisibleDetailRowCountBeforeRow + info.VisibleRowCount;
			}
			return info.VisibleDetailRowCountBeforeRow;
		}
		class CalcMasterRowInfoComparer : IComparer<ExpandedDetailInfo> {
			readonly List<ExpandedDetailInfo> expandedDetailInfoList;
			internal int CommonIndex;
			readonly Func<ExpandedDetailInfo, int> getStartIndex;
			readonly Func<ExpandedDetailInfo, int> getEndIndex;
			public CalcMasterRowInfoComparer(List<ExpandedDetailInfo> expandedDetailInfoList, Func<ExpandedDetailInfo, int> getStartIndex, Func<ExpandedDetailInfo, int> getEndIndex) {
				this.expandedDetailInfoList = expandedDetailInfoList;
				this.getStartIndex = getStartIndex;
				this.getEndIndex = getEndIndex;
			}
			int IComparer<ExpandedDetailInfo>.Compare(ExpandedDetailInfo item, ExpandedDetailInfo ignore) {
				int startIndex = getStartIndex(item);
				int endIndex = getEndIndex(item);
				if(startIndex <= CommonIndex && CommonIndex <= endIndex) {
					return 0;
				}
				if(CommonIndex < startIndex) {
					int listIndex = item.Index;
					if(listIndex == 0 || CommonIndex > getEndIndex(expandedDetailInfoList[listIndex - 1]))
						return 0;
				}
				if(CommonIndex > endIndex) {
					return -1;
				}
				return +1;
			}
		}
		public MasterRowScrollInfo CalcMasterRowScrollInfo(int commonScrollIndex) {
			ValidateCache();
			calcMasterRowScrollInfoComparer.CommonIndex = commonScrollIndex;
			int listIndex = expandedDetailInfoList.BinarySearch(default(ExpandedDetailInfo), calcMasterRowScrollInfoComparer);
			int startScrollIndex = commonScrollIndex;
			if(listIndex < 0) {
				startScrollIndex -= GetPrevInfoScrollRowCountBeforeNextRow(expandedDetailInfoList.Count);
				if(startScrollIndex < View.DataControl.VisibleRowCount + View.CalcGroupSummaryVisibleRowCount())
					return new MasterRowScrollInfo(startScrollIndex, 0, true);
				else
					return null;
			}
			ExpandedDetailInfo info = expandedDetailInfoList[listIndex];
			if(commonScrollIndex < info.CommonScrollIndex) {
				startScrollIndex -= GetPrevInfoScrollRowCountBeforeNextRow(listIndex);
				return new MasterRowScrollInfo(startScrollIndex, 0, true);
			}
			return new MasterRowScrollInfo(info.LocalScrollIndex, commonScrollIndex - info.CommonScrollIndex, info.LastDetailRowCommonScrollIndex == commonScrollIndex);
		}
		int GetPrevInfoScrollRowCountBeforeNextRow(int index) {
			if(index > 0)
				return expandedDetailInfoList[index - 1].ScrollRowCountBeforeNextRow;
			return 0;
		}
		public MasterRowNavigationInfo CalcMasterRowNavigationInfo(int commonVisibleIndex) {
			ValidateCache();
			calcMasterRowNavigationInfoComparer.CommonIndex = commonVisibleIndex;
			int listIndex = expandedDetailInfoList.BinarySearch(default(ExpandedDetailInfo), calcMasterRowNavigationInfoComparer);
			int startVisibleIndex = commonVisibleIndex;
			if(listIndex < 0) {
				startVisibleIndex -= GetPrevInfoVisibleRowCountBeforeNextRow(expandedDetailInfoList.Count);
				int visibleRowCount = View.DataControl.VisibleRowCount;
				if(View.IsNewItemRowVisible)
					visibleRowCount++;
				if(0 <= startVisibleIndex && startVisibleIndex < visibleRowCount)
					return new MasterRowNavigationInfo(startVisibleIndex, 0, false);
				return null;
			}
			ExpandedDetailInfo info = expandedDetailInfoList[listIndex];
			if(commonVisibleIndex < info.CommonVisibleIndex) {
				startVisibleIndex -= GetPrevInfoVisibleRowCountBeforeNextRow(listIndex);
				return new MasterRowNavigationInfo(startVisibleIndex, 0, false);
			}
			return new MasterRowNavigationInfo(info.LocalVisibleIndex, commonVisibleIndex - info.CommonVisibleIndex, true);
		}
		int GetPrevInfoVisibleRowCountBeforeNextRow(int index) {
			if(index > 0)
				return expandedDetailInfoList[index - 1].VisibleDetailRowCountBeforeNextRow;
			return 0;
		}
	}
}
