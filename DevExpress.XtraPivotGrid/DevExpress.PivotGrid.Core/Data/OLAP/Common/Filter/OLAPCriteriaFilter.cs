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
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPCriteriaFilter {
		delegate void QueryMemberNodeAction(OLAPMemberNode node, bool? included);
		static void CheckSubNodes(OLAPMemberNode node, QueryMemberNodeAction action, bool? included, bool checkSelf) {
			if(checkSelf)
				action(node, included);
			foreach(OLAPMemberNode includedNode in node.IncludedChildren)
				CheckSubNodes(includedNode, action, true, true);
			foreach(OLAPMemberNode excludedNode in node.ExcludedChildren)
				CheckSubNodes(excludedNode, action, false, true);
			foreach(OLAPMemberNode undefinedNode in node.UndefinedChildren)
				CheckSubNodes(undefinedNode, action, null, true);
		}
		OLAPMemberNode filterRoot;
		OLAPCubeColumn column;
		CriteriaOperator criteria;
		PivotFilterType preferredFilterType;
		QueryMemberNodeToIFieldFiler fieldFilterIncludeMiddleValues;
		QueryMemberNodeToIFieldFiler fieldFilterExcludeMiddleValues;
		IGroupFilter groupFilter;
		public OLAPCriteriaFilter(CriteriaOperator criteria, OLAPCubeColumn column) {
			this.criteria = CriteriaOperator.Clone(criteria);
			this.column = column;
		}
		public IFieldFilter GetFieldFilter(bool includeMiddleValues, bool? isIncluded) {
			return GetFieldFilterCore(includeMiddleValues).GetIFieldFilter(isIncluded, GetPreferableFilterType());
		}
		QueryMemberNodeToIFieldFiler GetFieldFilterCore(bool includeMiddleValues) {
			if(includeMiddleValues) {
				if(fieldFilterIncludeMiddleValues == null) {
					EnsureValues();
					fieldFilterIncludeMiddleValues = new QueryMemberNodeToIFieldFiler(filterRoot, preferredFilterType == PivotFilterType.Included, true);
				}
				return fieldFilterIncludeMiddleValues;
			}
			if(fieldFilterExcludeMiddleValues == null) {
				EnsureValues();
				fieldFilterExcludeMiddleValues = new QueryMemberNodeToIFieldFiler(filterRoot, preferredFilterType == PivotFilterType.Included, false);
			}
			return fieldFilterExcludeMiddleValues;
		}
		public IGroupFilter GetGroupFilter() {
			if(groupFilter == null) {
				EnsureValues();
				groupFilter = new OLAPMemberNodeToIGroupFiler(filterRoot, preferredFilterType == PivotFilterType.Included);
			}
			return groupFilter;
		}
		public OLAPFilterValues GetFilterValues(bool includeChildValues, bool? isIncluded) {
			return GetFieldFilterCore(includeChildValues).CreateFilterValues(column, isIncluded, GetPreferableFilterType());
		}
		public PivotFilterType GetPreferableFilterType() {
			return preferredFilterType;
		}
		void EnsureValues() {
			if(filterRoot != null)
				return;
			filterRoot = new OLAPMemberNode(null);
			QueryFilterVisitor evaluator = new QueryFilterVisitor(new QueryGroupFilterEvaluator(), criteria, column.Metadata);
			foreach(OLAPMember member in column.Metadata) {
				if(member.IsTotal)
					continue;
				CheckMember(filterRoot, evaluator, new QueryContextCache(), member);
			}
			preferredFilterType = CalculatePreferredFilterType();
		}
		PivotFilterType CalculatePreferredFilterType() {
			int includedCount = 0;
			int excludedCount = 0;
			CheckSubNodes(filterRoot, (node, included) => {
				if(included == true)
					includedCount++;
				else if(included == false)
					excludedCount++;
			}, true, false);
			return includedCount > excludedCount ? PivotFilterType.Excluded : PivotFilterType.Included;
		}
		void CheckMember(OLAPMemberNode parentMemberNode, QueryFilterVisitor evaluator, QueryContextCache contextCache, OLAPMember lastMember) {
			OLAPMemberNode memberNode = new OLAPMemberNode(lastMember);
			contextCache.Add(lastMember.Column.UniqueName, lastMember);
			bool? fit = evaluator.Fit(contextCache);
			if(fit == true) {
				parentMemberNode.IncludedChildren.Add(memberNode);
			} else {
				if(fit == false) {
					parentMemberNode.ExcludedChildren.Add(memberNode);
				} else {
					parentMemberNode.UndefinedChildren.Add(memberNode);
					if(lastMember.ChildMembers.Count == 0) {
						IOLAPMetadata olapMetadata = column.Metadata.Owner;
						if(olapMetadata != null)
							olapMetadata.QueryChildMembers(null, lastMember);
					}
					foreach(OLAPMember childMember in lastMember.ChildMembers)
						CheckMember(memberNode, evaluator, contextCache, childMember);
				}
			}
			contextCache.Remove(lastMember.Column.UniqueName);
		}
		#region inner classes
		class OLAPMemberNode {
			readonly OLAPMember member;
			List<OLAPMemberNode> includedChildren;
			List<OLAPMemberNode> excludedChildren;
			List<OLAPMemberNode> undefinedChildren;
			public List<OLAPMemberNode> IncludedChildren {
				get {
					if(includedChildren == null)
						includedChildren = new List<OLAPMemberNode>();
					return includedChildren;
				}
			}
			public List<OLAPMemberNode> ExcludedChildren {
				get {
					if(excludedChildren == null)
						excludedChildren = new List<OLAPMemberNode>();
					return excludedChildren;
				}
			}
			public List<OLAPMemberNode> UndefinedChildren {
				get {
					if(undefinedChildren == null)
						undefinedChildren = new List<OLAPMemberNode>();
					return undefinedChildren;
				}
			}
			public OLAPMember Member { get { return member; } }
			public OLAPMemberNode(OLAPMember member) {
				this.member = member;
			}
		}
		class QueryMemberNodeToIFieldFiler {
			readonly OLAPMemberNode rootNode;
			readonly bool includeChildValues;
			readonly List<OLAPMember> includedMembers = new List<OLAPMember>();
			readonly List<OLAPMember> excludedMembers = new List<OLAPMember>();
			IFieldFilter included;
			IFieldFilter exluded;
			IFieldFilter Included {
				get {
					if(included == null)
						included = new QueryMemberNodeToIFieldFilter(this, true);
					return included;
				}
			}
			IFieldFilter Excluded {
				get {
					if(exluded == null)
						exluded = new QueryMemberNodeToIFieldFilter(this, false);
					return exluded;
				}
			}
			class QueryMemberNodeToIFieldFilter : IFieldFilter {
				QueryMemberNodeToIFieldFiler owner;
				bool isIncluded;
				public QueryMemberNodeToIFieldFilter(QueryMemberNodeToIFieldFiler owner, bool isIncluded) {
					this.owner = owner;
					this.isIncluded = isIncluded;
				}
				PivotFilterType IFieldFilter.FilterType {
					get { return isIncluded ? PivotFilterType.Included : PivotFilterType.Excluded; }
				}
				bool IFieldFilter.HasFilter {
					get {
						owner.EnsureValues();
						return owner.GetMembers(isIncluded).Count != 0;
					}
				}
				object[] IFieldFilter.Values {
					get {
						owner.EnsureValues();
						return owner.GetMembers(isIncluded).Select((d) => (string)d.UniqueLevelValue).ToArray();
					}
				}
				bool IFieldFilter.ShowBlanks {
					get { return true; }
				}
				NullableHashtable IFieldFilter.HashTable {
					get { throw new NotImplementedException(); }
				}
				string IFieldFilter.PersistentString {
					get { throw new NotImplementedException(); }
				}
			}
			public QueryMemberNodeToIFieldFiler(OLAPMemberNode rootNode, bool isIncluded, bool includeChildValues) {
				this.rootNode = rootNode;
				this.includeChildValues = includeChildValues;
			}
			public IFieldFilter GetIFieldFilter(bool? isIncluded, PivotFilterType preferable) {
				if(GetIsIncludedCore(isIncluded, preferable))
					return Included;
				else
					return Excluded;
			}
			internal OLAPFilterValues CreateFilterValues(OLAPCubeColumn rootColumn, bool? isIncluded, PivotFilterType preferable) {
				EnsureValues();
				bool isIncludedCore = GetIsIncludedCore(isIncluded, preferable);
				return new OLAPFilterValues(isIncludedCore, GetMembers(isIncludedCore), rootColumn.Owner);
			}
			List<OLAPMember> GetMembers(bool isIncluded) {
				return isIncluded ? includedMembers : excludedMembers;
			}
			static bool GetIsIncludedCore(bool? isIncluded, PivotFilterType preferable) {
				bool isIncludedCore = isIncluded == null ? preferable == PivotFilterType.Included : (bool)isIncluded;
				return isIncludedCore;
			}
			bool ensured;
			void EnsureValues() {
				if(ensured)
					return;
				ensured = true;
				CheckSubNodes(rootNode, (node, isNodeIncluded) => {
					if(isNodeIncluded == true)
						includedMembers.Add(node.Member);
					else
						if(isNodeIncluded == false)
							excludedMembers.Add(node.Member);
						else
							if(!includeChildValues)
								includedMembers.Add(node.Member);
				}, true, false);
				if(includeChildValues)
					includedMembers.Sort((a, b) => Comparer<int>.Default.Compare(a.Column.Level, b.Column.Level));
			}
		}
		class OLAPMemberNodeToIGroupFiler : IGroupFilter {
			readonly OLAPMemberNode rootNode;
			readonly bool isIncluded;
			PivotGroupFilterValuesCollection values;
			List<OLAPMetadataColumn> columns;
			public OLAPMemberNodeToIGroupFiler(OLAPMemberNode rootNode, bool isIncluded) {
				this.rootNode = rootNode;
				this.isIncluded = isIncluded;
			}
			bool IGroupFilter.HasFilter {
				get { return values.Count > 0; }
			}
			PivotGroupFilterValuesCollection IGroupFilter.Values {
				get {
					EnsureValues();
					return values;
				}
			}
			PivotFilterType IGroupFilter.FilterType {
				get { return isIncluded ? PivotFilterType.Included : PivotFilterType.Excluded; }
			}
			int IGroupFilter.GetOLAPLevel(int index) {
				EnsureColumns();
				return columns[index].Level;
			}
			string IGroupFilter.GetFieldName(int index) {
				EnsureColumns();
				return columns[index].UniqueName;
			}
			int IGroupFilter.LevelCount {
				get {
					EnsureColumns();
					return columns.Count;
				}
			}
			string IGroupFilter.PersistentString {
				get { throw new NotImplementedException(); }
			}
			void EnsureColumns() {
				if(columns != null)
					return;
				columns = new List<OLAPMetadataColumn>();
				CheckSubNodes(rootNode, (node, isIncluded) => {
					if(!columns.Contains(node.Member.Column))
						columns.Add(node.Member.Column);
				}, true, false);
				columns.Sort((a, b) => Comparer<int>.Default.Compare(a.Level, b.Level));
			}
			void EnsureValues() {
				if(values != null)
					return;
				values = new PivotGroupFilterValuesCollection(null);
				CreateFilterNodes(values, rootNode);
			}
			void CreateFilterNodes(PivotGroupFilterValuesCollection parentCollecition, OLAPMemberNode node) {
				if(isIncluded)
					foreach(OLAPMemberNode includedNode in node.IncludedChildren)
						parentCollecition.Add(new PivotGroupFilterValue(includedNode.Member.UniqueLevelValue));
				else
					foreach(OLAPMemberNode excludedNode in node.ExcludedChildren)
						parentCollecition.Add(new PivotGroupFilterValue(excludedNode.Member.UniqueLevelValue));
				foreach(OLAPMemberNode undefinedNode in node.UndefinedChildren) {
					PivotGroupFilterValue value = new PivotGroupFilterValue(undefinedNode.Member.UniqueLevelValue);
					parentCollecition.Add(value);
					CreateFilterNodes(value.ChildValues, undefinedNode);
				}
			}
		}
		#endregion
	}
}
