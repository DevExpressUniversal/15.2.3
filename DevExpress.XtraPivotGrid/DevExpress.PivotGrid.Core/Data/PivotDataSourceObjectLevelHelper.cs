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
using System.Text;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotDataSourceObjectLevelHelper {
		IPivotGridDataSourceOwner dataSourceOwner;
		PivotGridOptionsData optionsData;
		public PivotDataSourceObjectLevelHelper(IPivotGridDataSourceOwner dataSourceOwner, PivotGridOptionsData optionsData) {
			this.dataSourceOwner = dataSourceOwner;
			this.optionsData = optionsData;
		}
		IPivotGridDataSource DataSource { get { return this.dataSourceOwner.DataSource; } }
		PivotGridOptionsData OptionsData { get { return optionsData; } }
		public int GetParentIndex(bool byColumn, int visibleIndex) {
			int level = DataSource.GetObjectLevel(byColumn, visibleIndex) - 1;
			if(level >= 0) {
				for(int i = visibleIndex - 1; i >= 0; i--) {
					if(DataSource.GetObjectLevel(byColumn, i) == level)
						return i;
				}
			}
			return -1;
		}
		public int GetPrevIndex(bool byColumn, int currentIndex) {
			return GetPrevIndex(byColumn, currentIndex, !OptionsData.AllowCrossGroupVariation);
		}
		public int GetPrevIndex(bool byColumn, int currentIndex, bool stopOnParent) {
			int i = currentIndex - 1, curLevel = DataSource.GetObjectLevel(byColumn, currentIndex);
			if(stopOnParent) {
				while(DataSource.GetObjectLevel(byColumn, i) > curLevel && i >= 0) i--;
			} else {
				while(DataSource.GetObjectLevel(byColumn, i) != curLevel && i >= 0) i--;
			}
			if(DataSource.GetObjectLevel(byColumn, i) != curLevel) return -1;
			return i;
		}
		public int GetGroupStartIndex(bool byColumn, int currentIndex) {
			return GetGroupStartIndex(byColumn, currentIndex, false);
		}
		public int GetGroupStartIndex(bool byColumn, int currentIndex, bool allowCrossGroupVariation) {
			int i = currentIndex - 1, startIndex = currentIndex, curLevel = DataSource.GetObjectLevel(byColumn, currentIndex);
			while((allowCrossGroupVariation || ((DataSource.GetObjectLevel(byColumn, i) >= curLevel))) && i >= 0) {
				if(DataSource.GetObjectLevel(byColumn, i) == curLevel)
					startIndex = i;
				i--;
			}
			return startIndex;
		}
		public int GetNextIndex(bool isColumn, int currentIndex) {
			return GetNextIndex(isColumn, currentIndex, false);
		}
		public int GetNextIndex(bool isColumn, int currentIndex, bool allowCrossGroupVariation) {
			int i = currentIndex + 1, curLevel = GetObjectLevel(isColumn, currentIndex), count = DataSource.GetCellCount(isColumn);
			if(!allowCrossGroupVariation) {
				while(DataSource.GetObjectLevel(isColumn, i) > curLevel && i < count) i++;
			} else {
				while(DataSource.GetObjectLevel(isColumn, i) != curLevel && i < count) i++;
			}
			if(DataSource.GetObjectLevel(isColumn, i) != curLevel) return -1;
			return i;
		}
		public int GetColumnLevel(int columnIndex) {
			return GetObjectLevel(true, columnIndex);
		}
		public int GetRowLevel(int rowIndex) {
			return GetObjectLevel(false, rowIndex);
		}
		public int GetObjectLevel(bool byColumn, int visibleIndex) {
			return DataSource.GetObjectLevel(byColumn, visibleIndex);
		}
		public int GetMaxObjectLevel(bool isColumn) {
			int count = DataSource.GetCellCount(isColumn),
				maxLevel = -1;
			for(int i = 0; i < count; i++)
				maxLevel = Math.Max(GetObjectLevel(isColumn, i), maxLevel);
			return maxLevel;
		}
	}
}
