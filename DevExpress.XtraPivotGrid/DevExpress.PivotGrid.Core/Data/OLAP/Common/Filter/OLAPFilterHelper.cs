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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	class OLAPFilterHelper : OLAPFilterHelperBase {
		readonly CurrentFilterInfos currentFilterInfos = new CurrentFilterInfos();
		public OLAPFilterHelper(IOLAPHelpersOwner owner) : base(owner) { }
		public override bool HasGroupFilter(PivotGridFieldBase field, QueryColumn column, bool deferUpdates) {
			return field.Group != null && field.Group.FilterValues.GetActual(deferUpdates).HasFilter;
		}
		public override bool IsIncludedFilter_SQL2000(OLAPCubeColumn column, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			if(field == null)
				return false;
			if(field.Group != null && field.Group.FilterValues.GetActual(deferUpdates).HasFilter)
				return field.Group.FilterValues.GetActual(deferUpdates).FilterType == PivotFilterType.Included;
			return field.FilterValues.GetActual(deferUpdates).FilterType == PivotFilterType.Included;
		}
		protected override IGroupFilter GetGroupFilterValues(OLAPCubeColumn column, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			if(field.Group == null || field.Group[0] != field)
				return null;
			return field.Group.FilterValues.GetActual(deferUpdates);
		}
		protected bool? IsFieldFiltered(PivotGridFieldBase field, bool checkGroupFilter, bool deferUpdates) {
			if(field == null || !field.CanApplyFilter)
				return false;
			if(checkGroupFilter && field.Group != null && field.Group.FilterValues.GetActual(deferUpdates).HasFilter)
				return true;
			if(field.FilterValues.GetActual(deferUpdates).HasFilter)
				return true;
			return null;
		}
		protected override bool IsFiltered(OLAPCubeColumn column, bool checkGroupFilter, bool checkDefaultMembers, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			bool? filtered = IsFieldFiltered(field, checkGroupFilter, deferUpdates);
			if(filtered.HasValue)
				return filtered.Value;
			if(checkDefaultMembers && field.Visible && !column.Metadata.IsMeasure && !field.IsColumnOrRow && column.HasCustomDefaultMember
					&& field.Options.OLAPFilterUsingWhereClause != PivotOLAPFilterUsingWhereClause.Never) {
				if(field.Group == null)
					return true;
				foreach(PivotGridFieldBase groupField in field.Group) {
					if(groupField == field)
						continue;
					OLAPCubeColumn parent = Owner.CubeColumns[groupField.FieldName];
					if(parent == null)
						continue;
					if(IsFiltered(parent, false, false, deferUpdates))
						return false;
				}
				return field == field.Group[0];
			}
			return false;
		}
		protected override IFieldFilter GetFieldFilter(PivotGridFieldBase field, OLAPCubeColumn column, bool deferUpdates) {
			return field.FilterValues.GetActual(deferUpdates);
		}
		protected override OLAPFilterValues GetGroupFilterValues(PivotGridGroup group, QueryTuple parentTuple, PivotGridFieldBase field, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates) {
			IGroupFilter filter = group.FilterValues.GetActual(deferUpdates);
			if(parentTuple == null) {
				if(includeChildValues) {
					if(field.FieldName == filter.GetFieldName(0))
						return GetCompleteGroupFilterValues(false, filter, null);
					else
						return IsFiltered(column, false, true, deferUpdates) ? GetFieldFilterValues(field, column, deferUpdates) : null;
				} else {
					if(field.FieldName == filter.GetFieldName(0)) {
						List<object> values;
						if(filter.FilterType == PivotFilterType.Excluded) {
							if(filter.LevelCount == 0)
								values = new List<object>();
							else if(filter.LevelCount > 1)
								values = filter.Values.GetLastLevelValues();
							else
								values = filter.Values.ToList();
						} else {
							values = filter.Values.ToList();
						}
						return CreateFilterValues(filter.FilterType, GetColumnMembersByValues(column, values));
					} else
						throw new NotSupportedException();
				}
			}
			if(!parentTuple.Last.Column.IsParent(column.Metadata))
				throw new QueryException("Invalid parentMember");
			PivotGroupFilterValue parentValue = FindFilterValue(filter, parentTuple);
			if(parentValue == null || parentValue.ChildValues == null || parentValue.ChildValues.Count == 0)
				return null;
			if(((OLAPMember)parentTuple.Last).ChildMembers.Count == 0)
				Metadata.QueryChildMembers(Owner.CubeColumns[parentTuple.Last.Column.ChildColumn.UniqueName], parentTuple.Last);
			if(filter.FilterType == PivotFilterType.Included)
				return CreateFilterValues(filter.FilterType, GetColumnMembersByValues(column, parentValue.ChildValues.ToList()));
			else
				return CreateFilterValues(filter.FilterType, GetColumnMembersByValues(column, parentValue.ChildValues.GetLastLevelValues()));
		}
		PivotGroupFilterValue FindFilterValue(IGroupFilter filter, QueryTuple parentTuple) {
			PivotGroupFilterValue filterValue = null;
			PivotGroupFilterValuesCollection filterValues = filter.Values;
			foreach(QueryMember member in parentTuple.AllMembers) {
				filterValue = filterValues[member.Value];
				if(filterValue == null)
					return null;
				filterValues = filterValue.ChildValues;
			}
			return filterValue;
		}
		protected override OLAPFilterValues GetCompleteGroupFilterValues(bool includeMiddleValues, IGroupFilter filter, bool? isIncluded) {
			if(filter == null)
				return null;
			if(isIncluded == null) {
				List<OLAPMember> members = new List<OLAPMember>();
				OLAPCubeColumn column = GetColumn(filter.GetFieldName(0));
				column.EnsureColumnMembersLoaded();
				List<int> valueLevels = new List<int>();
				for(int i = 0; i < filter.LevelCount; i++)
					valueLevels.Add(filter.GetOLAPLevel(i));
				GetLastLevelFilterValuesCore(members, valueLevels, filter.Values, column.Metadata, includeMiddleValues);
				members.Sort((x, y) => {
					if(x.Column != y.Column)
						return x.Column.IsParent(y.Column) ? -1 : 1;
					return Comparer<string>.Default.Compare((string)x.UniqueLevelValue, (string)y.UniqueLevelValue);
				});
				return CreateFilterValues(filter.FilterType, members);
			} else
				if(isIncluded == true) {
					OLAPCubeColumn column = GetColumn(filter.GetFieldName(0));
					PivotGridFieldBase field = GetField(column);
					if(field.Group == null || field.Group[0] != field)
						return null;
					OLAPFilterValues filterValues = HasGroupFilter(field, column, false) ? GetCompleteGroupFilterValues(includeMiddleValues, filter, null) : null;
					if(filterValues == null)
						return null;
					if(filterValues.IsIncluded)
						return filterValues;
					IList<OLAPMember> parentMembers = new List<OLAPMember>();
					foreach(OLAPChildMember member in filterValues.GetProcessedMembers())
						if(!parentMembers.Contains(member.ParentMember))
							parentMembers.Add(member.ParentMember);
					column.EnsureColumnMembersLoaded();
					if(filterValues == null)
						return new OLAPFilterValues(true, column.Metadata.GetMembers(false), Owner);
					Dictionary<OLAPMember, object> membersHash = new Dictionary<OLAPMember, object>();
					for(int i = 0; i < parentMembers.Count; i++) {
						OLAPMember member = parentMembers[i];
						IEnumerable<QueryMember> members;
						if(member == null) {
							members = column.Metadata.GetMembers(false);
						} else {
							if(member.ChildMembers == null)
								Metadata.QueryChildMembers(Owner.CubeColumns[member.Column.ChildColumn.UniqueName], member);
							members = member.ChildMembers;
						}
						foreach(OLAPMember curMember in members) {
							membersHash.Add(curMember, null);
						}
					}
					foreach(OLAPMember member in filterValues.GetProcessedMembers()) {
						membersHash.Remove(member);
					}
					foreach(OLAPMember member in parentMembers) {
						if(member != null)
							membersHash.Remove(member);
					}
					return new OLAPFilterValues(true, new List<OLAPMember>(membersHash.Keys), Owner);
				} else {
					throw new NotImplementedException();
				}
		}
		void GetLastLevelFilterValuesCore(List<OLAPMember> res, List<int> valueLevels, PivotGroupFilterValuesCollection values, IOLAPEditableMemberCollection parent, bool includeMiddleValues) {
			foreach(PivotGroupFilterValue value in values) {
				IList<OLAPMember> childMembers = GetFilterMembersByValue(true, valueLevels[value.Level], value.Value, parent);
				if(childMembers == null || childMembers.Count == 0)
					continue;
				if(value.IsLastLevel || value.ChildValues.Count == 0) {
					res.AddRange(childMembers);
				} else {
					if(includeMiddleValues)
						res.AddRange(childMembers);
					foreach(OLAPMember childMember in childMembers) {
						if(childMember.ChildMembers.Count == 0)
							Metadata.QueryChildMembers(Owner.CubeColumns[childMember.Column.ChildColumn.UniqueName], childMember);
						GetLastLevelFilterValuesCore(res, valueLevels, value.ChildValues, childMember.ChildMembers, includeMiddleValues);
					}
				}
			}
		}
		public override bool BeforeSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields) {
			return false;
		}
		protected override bool SetColumnFilters(PivotGridFieldBase field, OLAPCubeColumn column) {
			bool res = base.SetColumnFilters(field, column);
			if(currentFilterInfos.IsFieldChanged(column, field))
				res = true;
			return res;
		}
		public override void ClearCache() {
			currentFilterInfos.Clear();
		}
	}
}
