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
using System.Linq;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	class PartialUpdater : PartialUpdaterBase<OLAPCubeColumn> {
		public PartialUpdater(PivotGridFieldReadOnlyCollection sortedFields, AreasState<OLAPCubeColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<OLAPCubeColumn> newAreas, IPartialUpdaterOwner<OLAPCubeColumn> owner, bool isDataSourceFullyExpanded, bool removeEmptyData, CollapsedState row, CollapsedState column) :
			base(sortedFields, oldAreas, oldMetas, newAreas, owner, isDataSourceFullyExpanded, removeEmptyData, row, column) {
		}
		protected override bool UpdateAllOnColumnVisibleChange(OLAPCubeColumn column) {
			if(!column.HasCustomDefaultMember)
				return false;
			return true;
		}
		protected override bool NeedRefreshOnColumnHide(List<OLAPCubeColumn> area, int level) {
			return false;
		}
		protected override bool CalculateDataAreaChange(List<OLAPCubeColumn> oldColumns, List<OLAPCubeColumn> newColumns) {
			bool update = base.CalculateDataAreaChange(oldColumns, newColumns);
			return update || newColumns.Count == 0 && oldColumns.Count > 0 && Areas.RowValues.Count <= 1 && Areas.ColumnValues.Count <= 1;
		}
		protected override bool SortOnClient(XtraPivotGrid.PivotSortMode sortMode, bool hasTopn, bool hasSortBy, bool hasResolvedSortBySummary) {
			return !OLAPSortedMembersWriter.SortOnServer(sortMode, hasTopn, hasSortBy, hasResolvedSortBySummary);
		}
		protected override bool SetColumnAdditionalProperties(PivotGridFieldBase field, OLAPCubeColumn column) {
			if(((IOLAPHelpersOwner)Owner).Options.DefaultMemberFields == PivotDefaultMemberFields.AllFilterFields && field.Visible) {
				if(column == null || column.IsMeasure || !column.HasCustomDefaultMember)
					return false;
				PivotGridFieldFilterValues filterValues = field.FilterValues;
				if(field.Area == PivotArea.FilterArea) {
					if(PivotGridFieldBase.IsFilterEmptyFast(field) && column.DefaultMember != null) {
						filterValues.SetValues(new object[] { column.DefaultMember.Value }, PivotFilterType.Included, false, false);
						return true;
					}
				} else {
					object[] valuesIncluded = filterValues.FilterType == PivotFilterType.Included ? filterValues.ValuesIncluded : null;
					if(valuesIncluded != null && valuesIncluded.Length == 1 && valuesIncluded[0] == column.DefaultMember.Value) {
						filterValues.SetValues(new object[0], PivotFilterType.Excluded, true, false);
						return true;
					}
				}
			}
			return false;
		}
		internal override SortModeUpdater<OLAPCubeColumn> CreateSortModeUpdater(PivotGridFieldBase field) {
			return new OLAPSortModeUpdater(Owner, field.IsColumn, field.AreaIndex);
		}
		public override bool UpdateColumnSortOrder(OLAPCubeColumn column, PivotGridFieldBase field, bool update) {
			bool result = column.AssignAutoPopulatedProperties(field);
			result = base.UpdateColumnSortOrder(column, field, update) || result;
			column.SortByAttribute = field.SortByAttribute;
			return result;
		}
	}
	class OLAPSortModeUpdater : SortModeUpdater<OLAPCubeColumn> {
		public OLAPSortModeUpdater(IPartialUpdaterOwner<OLAPCubeColumn> owner, bool isColumn, int level)
			: base(owner, isColumn, level) {
		}
		protected override System.Action<List<GroupInfo>> GetUpdateAction() {
			Dictionary<OLAPMember, object> dic = new Dictionary<OLAPMember, object>();
			if(Column.IsSortedByAttribute) {
				int start = Level == 0 ? 1 : 0;
				ForEachLevelGroupInfo((list) => {
					for(int i = start; i < list.Count; i++) {
						GroupInfo info = list[i];
						if(!info.Member.IsTotal)
							dic[(OLAPMember)info.Member] = null;
					}
				});
			}
			((OLAPDataSourceBase)Owner).QueryMemberAttributes(Column, dic.Keys.ToArray());
			return base.GetUpdateAction();
		}
	}
}
