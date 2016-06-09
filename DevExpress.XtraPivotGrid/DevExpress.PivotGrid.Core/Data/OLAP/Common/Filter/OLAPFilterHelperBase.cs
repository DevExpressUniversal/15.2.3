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
using System.Text.RegularExpressions;
namespace DevExpress.PivotGrid.OLAP {
	abstract class OLAPFilterHelperBase : QueryFilterHelper, IOLAPFilterHelper {
		readonly IOLAPHelpersOwner owner;
		protected IOLAPHelpersOwner Owner { get { return owner; } }
		protected IOLAPMetadata Metadata { get { return owner.Metadata; } }
		public OLAPFilterHelperBase(IOLAPHelpersOwner owner)
			: base() {
			this.owner = owner;
		}
		#region IOLAPFilterHelper
		public abstract bool HasGroupFilter(PivotGridFieldBase field, QueryColumn column, bool deferUpdates);
		public abstract bool IsIncludedFilter_SQL2000(OLAPCubeColumn column, bool deferUpdates);
		public override bool AfterSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields) {
			bool res = false;
			OLAPCubeColumn column;
			foreach(PivotGridFieldBase field in sortedFields)
				if(Owner.CubeColumns.TryGetValue(field, out column))
					res |= SetColumnFilters(field, column);
			return res;
		}
		public OLAPFilterValues GetCompleteGroupFilterValues_SQL2000(OLAPCubeColumn column, bool includeMiddleValues, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			if(field == null)
				throw new ArgumentException("Can't find corresponding field");
			if(field.Group == null)
				throw new ArgumentException("Field has no group");
			return GetCompleteGroupFilterValues(includeMiddleValues, GetGroupFilterValues(column, deferUpdates), null);
		}
		public OLAPFilterValues GetFilterValues(QueryTuple parentTuple, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates) {
			if(!IsFiltered(column, deferUpdates))
				return null;
			return GetFilterValuesCore(parentTuple, column, includeChildValues, deferUpdates);
		}
		protected OLAPCubeColumn GetColumn(string fieldName) {
			OLAPCubeColumn res;
			return Owner.CubeColumns.TryGetValue(fieldName, out res) ? res : null;
		}
		public bool IsFiltered(OLAPCubeColumn column, bool deferUpdates) {
			return IsFiltered(column, true, true, deferUpdates);
		}
		public bool HasGroupFilter(OLAPCubeColumn column, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			if(field == null)
				return false;
			return HasGroupFilter(field, column, deferUpdates);
		}
		public OLAPFilterValues GetIncludedDrillDownFilterValues(OLAPCubeColumn column) {
			if(HasGroupFilter(GetField(column), column, false)) {
				return GetCompleteGroupFilterValues(false, GetGroupFilterValues(column, false), true);
			} else
				return GetIncludedFieldFilterValues(column, false);
		}
		public OLAPFilterValues GetIncludedFieldFilterValues(OLAPCubeColumn column, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			OLAPFilterValues filterValues = !HasGroupFilter(field, column, false) ? GetFieldFilterValues(field, column, deferUpdates) : null;
			if(filterValues == null)
				return null;
			if(filterValues.IsIncluded)
				return filterValues;
			return GetIncludedFieldFilterValuesFromExcludedCore(column, filterValues);
		}
		#endregion
		protected PivotGridFieldBase GetField(OLAPCubeColumn column) {
			PivotGridFieldBase res;
			return Owner.FieldsByColumns.TryGetValue(column, out res) ? res : null;
		}
		protected virtual bool SetColumnFilters(PivotGridFieldBase field, OLAPCubeColumn column) {
			bool res = false;
			if(column.OLAPFilterUsingWhereClause != field.Options.OLAPFilterUsingWhereClause) {
				column.OLAPFilterUsingWhereClause = field.Options.OLAPFilterUsingWhereClause;
				res = true;
			}
			bool isFiltered = IsFiltered(column, false);
			if(column.Filtered != isFiltered) {
				column.Filtered = isFiltered;
				res = true;
			}
			return res;
		}
		OLAPFilterValues GetFilterValuesCore(QueryTuple parentTuple, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates) {
			PivotGridFieldBase field = GetField(column);
			if(!HasGroupFilter(field, column, deferUpdates))
				return GetFieldFilterValues(field, column, deferUpdates);
			else
				return GetGroupFilterValues(field != null ? field.Group : null, parentTuple, field, column, includeChildValues, deferUpdates);
		}
		protected OLAPFilterValues GetFieldFilterValues(PivotGridFieldBase field, OLAPCubeColumn column, bool deferUpdates) {
			column.EnsureColumnMembersLoaded(); 
			IFieldFilter filter = GetFieldFilter(field, column, deferUpdates);
			return CreateFilterValues(filter.FilterType, GetColumnMembersByValues(column, filter.Values));
		}
		protected List<OLAPMember> GetColumnMembersByValues(OLAPCubeColumn column, IList<object> values) {
			List<OLAPMember> members = new List<OLAPMember>(values.Count);
			for(int i = 0; i < values.Count; i++) {
				members.AddRange(GetFilterMembersByValue(false, -1, values[i], column.Metadata));
			}
			return members;
		}
		protected IList<OLAPMember> GetFilterMembersByValue(bool recursive, int level, object value, IOLAPEditableMemberCollection parent) {
			IList<OLAPMember> childMembers = null;
			if(!Owner.CubeColumns[parent.Column.UniqueName].OLAPFilterByUniqueName) {
				childMembers = parent.GetMembersByValue(recursive, level, value);
			} else {
				childMembers = new List<OLAPMember>();
				OLAPMember member = null;
				string uniqueName = value as string;
				if(uniqueName != null)
					member = parent.GetMemberByUniqueLevelValue(uniqueName);
				if(member != null)
					childMembers.Add(member);
			}
			return childMembers;
		}
		protected OLAPFilterValues CreateFilterValues(PivotFilterType type, IList<OLAPMember> members) {
			return new OLAPFilterValues(type == PivotFilterType.Included, members, Owner);
		}
		OLAPFilterValues GetIncludedFieldFilterValuesFromExcludedCore(OLAPCubeColumn column, OLAPFilterValues filterValues) { 
			column.EnsureColumnMembersLoaded();
			IList<OLAPMember> members = column.GetMembers();
			if(filterValues == null)
				return CreateFilterValues(PivotFilterType.Included, members);
			Dictionary<OLAPMember, object> membersHash = new Dictionary<OLAPMember, object>(members.Count);
			foreach(OLAPMember member in column.GetMembers()) {
				membersHash.Add(member, null);
			}
			foreach(OLAPMember member in filterValues.GetProcessedMembers()) {
				membersHash.Remove(member);
			}
			return CreateFilterValues(PivotFilterType.Included, new List<OLAPMember>(membersHash.Keys));
		}
		protected abstract IGroupFilter GetGroupFilterValues(OLAPCubeColumn column, bool deferUpdates);
		protected abstract OLAPFilterValues GetCompleteGroupFilterValues(bool includeMiddleValues, IGroupFilter filter, bool? isInclued);
		protected abstract IFieldFilter GetFieldFilter(PivotGridFieldBase field, OLAPCubeColumn column, bool deferUpdates);
		protected abstract bool IsFiltered(OLAPCubeColumn column, bool checkGroupFilter, bool checkDefaultMembers, bool deferUpdates);
		protected abstract OLAPFilterValues GetGroupFilterValues(PivotGridGroup group, QueryTuple parentTuple, PivotGridFieldBase field, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates);
		#region IOLAPFilterHelper
		bool IOLAPFilterHelper.IsFilteredUsingWhereClause(OLAPCubeColumn column, List<OLAPCubeColumn> meausures) {
			if(column.OLAPFilterUsingWhereClause == PivotOLAPFilterUsingWhereClause.Auto) {
				if(IsFilterByAnyTimePeriod(column, meausures))
					return true;
			}
			bool usingWhereClause = column.OLAPFilterUsingWhereClause == PivotOLAPFilterUsingWhereClause.Always;
			if(column.OLAPFilterUsingWhereClause == PivotOLAPFilterUsingWhereClause.SingleValuesOnly || column.OLAPFilterUsingWhereClause == PivotOLAPFilterUsingWhereClause.Auto) {
				OLAPFilterValues filterValues = GetFilterValues(null, column, true, false);
				if(filterValues == null)
					return false;
				if(filterValues.IsCustomDefaultMemberFilter)
					return true;
				return filterValues.IsSingleValueFilter;
			}
			return usingWhereClause;
		}
		static Regex measuresRegEx = new Regex("(" + Regex.Escape("[Measures].[") + "[^\\]]*\\])");
		bool IsFilterByAnyTimePeriod(OLAPCubeColumn dimension, List<OLAPCubeColumn> meausures) {
			Regex dimensionRegex = new Regex(string.Format("(PERIODSTODATE|PARALLELPERIOD)\\(\\s*{0}", Regex.Escape(dimension.Hierarchy.Dimension.ToUpper())));
			foreach(OLAPCubeColumn measure in meausures) {
				if(IsFilterByAnyTimePeriod(dimension, measure, dimensionRegex))
					return true;
			}
			return false;
		}
		bool IsFilterByAnyTimePeriod(OLAPCubeColumn column, OLAPCubeColumn measure, Regex dimensionRegex) {
			if(string.IsNullOrEmpty(measure.Metadata.Expression))
				return false;
			string upper = measure.Metadata.Expression.ToUpper();
			if(dimensionRegex.IsMatch(upper))
				return true;
			Match match = measuresRegEx.Match(measure.Metadata.Expression);
			if(match != null)
				for(int ctr = 1; ctr <= match.Groups.Count - 1; ctr++)
					foreach(Capture capture in match.Groups[ctr].Captures) {
						OLAPCubeColumn dependantMeasure = null;
						if(capture.Value != null && column.Owner.CubeColumns.TryGetValue(capture.Value, out dependantMeasure) && IsFilterByAnyTimePeriod(column, dependantMeasure, dimensionRegex))
							return true;
					}
			return false;
		}
		OLAPCubeColumn IOLAPFilterHelper.GetColumn(OLAPMetadataColumn metadata) {
			return GetColumn(metadata.UniqueName);
		}
		List<OLAPCubeColumn> IOLAPFilterHelper.GetFilteredColumns(params PivotArea[] areas) {
			if(areas.Length == 0)
				throw new Exception("areas.Length");
			List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
			foreach(PivotArea area in areas)
				foreach(OLAPCubeColumn column in owner.Areas.GetArea(area))
					if(column.Filtered)
						result.Add(column);
			return result;
		}
		#endregion
	}
}
