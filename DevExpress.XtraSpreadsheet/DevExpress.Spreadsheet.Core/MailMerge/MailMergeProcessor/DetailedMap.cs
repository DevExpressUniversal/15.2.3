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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public class DetailedMap {
		#region fields
		MailMergeOptions options;
		CellRange parentRange;
		GrowthStrategy strategy;
		Dictionary<int, CellRange> detailsTopLeft;
		List<RangeInfo> ranges;
		#endregion
		public DetailedMap(CellRange parentRange, MailMergeOptions options, GrowthStrategy strategy) {
			this.options = options;
			this.parentRange = parentRange;
			this.strategy = strategy;
			Initialize();
		}
		#region Properties
		public List<RangeInfo> Ranges { get { return ranges; } }
		#endregion
		void Initialize() {
			if (options.IsGroupedRange(parentRange))
				parentRange = strategy.ExcludeGroupHeaderFooterRanges(options.GetGroupInfo(parentRange), parentRange);
			ranges = new List<RangeInfo>();
			List<int> grid = GetRangeGrid(options.GetChildRanges(parentRange, true));
			for (int i = 0; i < grid.Count - 1; i++) {
				int topLeft = grid[i];
				CellRange detail = null;
				if (detailsTopLeft.TryGetValue(topLeft, out detail)) {
					ranges.Add(new RangeInfo(detail, true));
					continue;
				}
				int bottomRight = grid[i + 1] - 1;
				ranges.Add(new RangeInfo(strategy.GetSubrangeByCriteria(parentRange, topLeft, bottomRight), false));
			}
		}
		List<int> GetRangeGrid(List<CellRangeBase> detailLevels) {
			detailsTopLeft = new Dictionary<int, CellRange>();
			DetailList<int> result = new DetailList<int>();
			result.AddInSortOrder(strategy.GetPositionCriterion(parentRange.TopLeft), strategy.GetPositionCriterion(parentRange.TopLeft));
			foreach (CellRange detailLevel in detailLevels) {
				int topleft = strategy.GetPositionCriterion(detailLevel.TopLeft);
				if(!detailsTopLeft.ContainsKey(topleft))
					detailsTopLeft.Add(topleft, detailLevel);
				int bottomRight = strategy.GetPositionCriterion(detailLevel.BottomRight) + 1;
				if (!result.Contains(topleft))
					result.AddInSortOrder(topleft, topleft);
				result.AddInSortOrder(bottomRight, bottomRight);
			}
			int parentBottomRight = strategy.GetPositionCriterion(parentRange.BottomRight) + 1;
			if (!result.Contains(parentBottomRight))
				result.AddInSortOrder(parentBottomRight, parentBottomRight);
			return result;
		}
	}
}
