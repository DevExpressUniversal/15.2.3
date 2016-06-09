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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.ServerMode {
	class PartialUpdater : PartialUpdaterBase<ServerModeColumn> {
		public PartialUpdater(PivotGridFieldReadOnlyCollection sortedFields, AreasState<ServerModeColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<ServerModeColumn> newAreas, IPartialUpdaterOwner<ServerModeColumn> owner, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column) :
			base(sortedFields, oldAreas, oldMetas, newAreas, owner, isDataSourceFullyExpanded, row, column) {
		}
		protected override bool NeedRefreshOnColumnHide(List<ServerModeColumn> area, int level) {
			return ExpandHelper.CanQueryFullLevel(area, level) && area[level + 1].CalculateTotals == false;
		}
		protected override bool SortOnClient(XtraPivotGrid.PivotSortMode sortMode, bool hasTopn, bool hasSortBy, bool hasResolvedSortBySummary) {
			return !hasTopn;
		}
		protected override bool SetColumnAdditionalProperties(PivotGridFieldBase field, ServerModeColumn column) {
			bool res = false;
			if(field.Area == PivotArea.DataArea && column.SummaryType != field.SummaryType) {
				column.SummaryType = field.SummaryType;
				res = true;
			}
			return res;
		}
		internal override SortModeUpdater<ServerModeColumn> CreateSortModeUpdater(PivotGridFieldBase field) {
			return new ServerModeSortModeUpdater(Owner, field.IsColumn, field.AreaIndex);
		}
	}
	class ServerModeSortModeUpdater : SortModeUpdater<ServerModeColumn> {
		bool consistencyChecked = false;
		public ServerModeSortModeUpdater(IPartialUpdaterOwner<ServerModeColumn> owner, bool isColumn, int level)
			: base(owner, isColumn, level) {
		}
		protected override Action<List<GroupInfo>> GetUpdateAction() {
			Action<List<GroupInfo>> basic = base.GetUpdateAction();
			return (gl) => {
				if(Column.SortBySummary != null && !consistencyChecked) {
					consistencyChecked = true;
					bool hasData = false;
					GroupInfo rowGroup = null, columnGroup = null;
					GroupInfo sortByInfo = ((ISortContext<ServerModeColumn, GroupInfo, IQueryMetadataColumn>)Areas).GetSortByObject(Column.SortBySummaryMembers, IsColumn);
					if(IsColumn)
						rowGroup = sortByInfo;
					else
						columnGroup = sortByInfo;
					foreach(GroupInfo info in Owner.Areas.GetFieldValues(IsColumn).EnumerateLevel(Level)) {
						if(IsColumn) {
							columnGroup = info;
						} else {
							rowGroup = info;
						}
						GroupInfoColumn c;
						MeasuresStorage st;
						if(Owner.Areas.Cells.TryGetValue(columnGroup, out c) && c.TryGetValue(rowGroup, out st)) {
							hasData = true;
							break;
						}
					}
					if(!hasData) {
						Action<List<GroupInfo>> query;
						if(IsColumn)
							query = (q) => Owner.QueryData(q.ToArray(), new GroupInfo[] { sortByInfo }, false, false);
						else
							query = (q) => Owner.QueryData(new GroupInfo[] { sortByInfo }, q.ToArray(), false, false);
						int rCount = 100;
						List<GroupInfo> infos = new List<GroupInfo>(rCount);
						foreach(GroupInfo info in Owner.Areas.GetFieldValues(IsColumn).EnumerateLevel(Level)) {
							infos.Add(info);
							if(infos.Count == rCount) {
								query(infos);
								infos.Clear();
							}
						}
						if(infos.Count > 0)
							query(infos);
					}
				}
				basic(gl);
			};
		}
	}
}
