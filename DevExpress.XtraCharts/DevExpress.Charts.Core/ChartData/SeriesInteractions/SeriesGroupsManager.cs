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
using System.Linq;
using System.Text;
namespace DevExpress.Charts.Native {
	public class SeriesGroupsManager {
		readonly Dictionary<RefinedSeries, int> groups = new Dictionary<RefinedSeries, int>();
		readonly List<RefinedSeries> series;
		int groupsCount;
		public int GroupsCount { 
			get { return groupsCount; } 
		}
		public SeriesGroupsManager(List<RefinedSeries> series) {
			this.series = series;
		}
		int GetGroupIndex(object groupKey) {
			foreach (RefinedSeries series in groups.Keys) {
				ISupportSeriesGroups groupView = series.SeriesView as ISupportSeriesGroups;
				if (groupView != null && Object.Equals(groupView.SeriesGroup, groupKey))
					return groups[series];
			}
			return -1;
		}
		public void UpdateGroups() {
			groups.Clear();
			groupsCount = 0;
			for (int i = 0; i < series.Count; i++) {
				ISupportSeriesGroups view = series[i].SeriesView as ISupportSeriesGroups;
				if (view != null) {
					int index = GetGroupIndex(view.SeriesGroup);
					if (index >= 0) {
						groups.Add(series[i], index);
						continue;
					}
				}
				groups.Add(series[i], groupsCount);
				groupsCount++;
			}
		}
		public int GetGroupIndexBySeries(RefinedSeries series) {
			return groups[series];
		}
		public bool IsSeriesInGroups(RefinedSeries series) {
			return groups.ContainsKey(series);
		}
	}
}
