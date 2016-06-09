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
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	class OLAPCriteriaFilterHelper : OLAPFilterHelperBase {
		string lastCriteria;
		Dictionary<QueryColumn, CriteriaOperator> prefilters = new Dictionary<QueryColumn, CriteriaOperator>();
		Dictionary<QueryColumn, OLAPCriteriaFilter> cachedFilters = new Dictionary<QueryColumn, OLAPCriteriaFilter>();
		public OLAPCriteriaFilterHelper(IOLAPHelpersOwner owner)
			: base(owner) {
		}
		public override void ClearCache() {
			prefilters.Clear();
			cachedFilters.Clear();
		}
		public override bool BeforeSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields) {
			CriteriaOperator criteria = Owner.Owner.PrefilterCriteria;
			bool changed = lastCriteria != (new CriteriaParameterReplacer().Process(criteria) ?? string.Empty).ToString();
			ClearCache();
			lastCriteria = (new CriteriaParameterReplacer().Process(criteria) ?? string.Empty).ToString();
			if(ReferenceEquals(criteria, null))
				return changed;
			Dictionary<string, Dictionary<string, string>> allNames = ((IOLAPHelpersOwner)Owner).GetExpressionNamesByHierarchies(criteria);
			foreach(KeyValuePair<string, Dictionary<string, string>> columns in allNames) {
				prefilters.Add(Owner.CubeColumns[columns.Key], new QueryCriteriaOperatorVisitor(columns.Value).Process(criteria));
			}
			return changed;
		}
		public override IEnumerable<QueryColumn> GetAdditionalFilteredColumns() {
			foreach(KeyValuePair<QueryColumn, CriteriaOperator> pair in prefilters)
				yield return pair.Key;
		}
		public override bool HasGroupFilter(PivotGridFieldBase field, QueryColumn column, bool deferUpdates) {
			return prefilters.ContainsKey(column) && column.Metadata.ParentColumn == null && column.Metadata.ChildColumn != null;
		}
		protected override bool IsFiltered(OLAPCubeColumn column, bool checkGroupFilter, bool checkDefaultMembers, bool deferUpdates) {
			return prefilters.ContainsKey(column);
		}
		protected override IFieldFilter GetFieldFilter(PivotGridFieldBase field, OLAPCubeColumn column, bool deferUpdates) {
			return GetFieldFilter(column, false);
		}
		IFieldFilter GetFieldFilter(OLAPCubeColumn column, bool includeMiddleValues) {
			return GetPrefilter(column).GetFieldFilter(includeMiddleValues, null);
		}
		IGroupFilter GetGroupFilter(OLAPCubeColumn column) {
			return GetPrefilter(column).GetGroupFilter();
		}
		OLAPCriteriaFilter GetPrefilter(OLAPCubeColumn column) {
			if(!cachedFilters.ContainsKey(column))
				cachedFilters.Add(column, new OLAPCriteriaFilter(prefilters[column], column));
			return cachedFilters[column];
		}
		protected override OLAPFilterValues GetGroupFilterValues(PivotGridGroup group, QueryTuple parentTuple, PivotGridFieldBase field, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates) {
			return GetGroupFilterValuesCore(parentTuple, column, includeChildValues, null);
		}
		OLAPFilterValues GetGroupFilterValuesCore(QueryTuple parentTuple, OLAPCubeColumn column, bool includeChildValues, bool? isIncluded) {
			if(parentTuple != null)
				throw new ArgumentException("not implemented"); 
			column.EnsureColumnMembersLoaded();
			return GetPrefilter(column).GetFilterValues(includeChildValues, isIncluded);
		}
		protected override IGroupFilter GetGroupFilterValues(OLAPCubeColumn column, bool deferUpdates) {
			return GetGroupFilter(column);
		}
		public override bool IsIncludedFilter_SQL2000(OLAPCubeColumn column, bool deferUpdates) {
			return GetPrefilter(column).GetPreferableFilterType() == PivotFilterType.Included;
		}
		protected override OLAPFilterValues GetCompleteGroupFilterValues(bool includeMiddleValues, IGroupFilter filter, bool? isIncluded) {
			OLAPCubeColumn column = GetColumn(filter.GetFieldName(0));
			return GetGroupFilterValuesCore(null, column, includeMiddleValues, isIncluded);
		}
	}
}
