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

using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPMembersWriter {
		public OLAPMembersWriter() { }
		protected string GetSortOrder(OLAPCubeColumn column, bool breakHierarchy) {
			if(breakHierarchy)
				return column.SortOrder == PivotSortOrder.Ascending ? "basc" : "bdesc";
			else
				return column.SortOrder == PivotSortOrder.Ascending ? "asc" : "desc";
		}
		public void WriteMembers(StringBuilder result, IList<string> members, bool includeBrackets) {
			if(includeBrackets) 
				result.Append("{ ");
			for(int i = 0; i < members.Count; i++) {
				result.Append(members[i]);
				if(i != members.Count - 1)
					result.Append(", ");
				else
					result.Append(" ");
			}
			if(includeBrackets) 
				result.Append("} ");
		}
		public void WriteMembers(StringBuilder result, OLAPLevelFilter members, bool includeBrackets) {
			if(includeBrackets)
				result.Append("{ ");
			result.Append(members.GetMDX(includeBrackets));
			if(includeBrackets)
				result.Append("} ");
		}
		public void WriteMembers(StringBuilder result, IList<OLAPMember> members, bool includeBrackets) {
			if(includeBrackets)
				result.Append("{ ");
			for(int i = 0; i < members.Count; i++) {
				result.Append(members[i].UniqueName);
				if(i != members.Count - 1)
					result.Append(", ");
				else
					result.Append(" ");
			}
			if(includeBrackets)
				result.Append("} ");
		}
		public string GetMembers(IList<string> members, bool includeBrackets) {
			StringBuilder stb = new StringBuilder();
			WriteMembers(stb, members, includeBrackets);
			return stb.ToString();
		}
		public string GetMembers(IList<OLAPMember> members, bool includeBrackets) {
			StringBuilder stb = new StringBuilder();
			WriteMembers(stb, members, includeBrackets);
			return stb.ToString();
		}
		protected void WriteTopMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, Action<StringBuilder> writeContent) {
			string setName = QueryTopTempSet.GetColumnTopName(column);
			string mdx = WriteTopMembersCore(column, writeContent, queryBuilder);
			int i = 0;
			string nSetName = "[" + setName + "]";
			while(i < int.MaxValue) {
				if(!queryBuilder.ContainsCalculatedMember(nSetName)) {
					queryBuilder.AddWithMember(new QueryTopTempSet(nSetName, mdx));
					result.Append(nSetName);
					return;
				}
				if(mdx == queryBuilder.GetWithMemberMdx(nSetName)) {
					result.Append(nSetName);
					return;
				} else {
					nSetName = "[" + setName + i.ToString() + "]";
					i++;
				}
			}
			throw new InvalidOperationException();
		}
		protected internal static string WriteTopMembersCore(OLAPCubeColumn column, Action<StringBuilder> writeContent, QueryBuilder queryBuilder) {
			bool topMembers = column.SortBySummary != null ^ column.SortOrder == PivotSortOrder.Ascending;
			if(!topMembers && column.SortBySummary == null)
				topMembers = true;
			string topMeasure = string.Empty;
			if(column.SortBySummary != null)
				topMeasure = column.GetSortBySummaryMDX();
			else
				if(column.TopValueType != PivotTopValueType.Absolute && queryBuilder.Measures.Count > 0)
					topMeasure = queryBuilder.Measures[0].UniqueName;
			return MDX.WriteTopCount(column.TopValueType, writeContent, queryBuilder, topMembers, column.TopValueCount, GetColumnTopCountMeasures(queryBuilder, column), topMeasure);
		}
		static List<OLAPCubeColumn> GetColumnTopCountMeasures(QueryBuilder queryBuilder, OLAPCubeColumn column) {
			if(column.SortBySummary == null)
				return queryBuilder.Measures;
			List<OLAPCubeColumn> columns = new List<OLAPCubeColumn>();
			columns.Add(column.SortBySummary);
			return columns;
		}
		protected void WriteTuple(StringBuilder result, QueryTuple tuple) {
			WriteTuple(result, tuple, true);
		}
		protected void WriteTuple(StringBuilder result, QueryTuple tuple, bool includeLastMember) {
			result.Append("( ");
			int count = includeLastMember ? tuple.MemberCount : tuple.MemberCount - 1;
			for(int j = 0; j < count; j++) {
				if(j != count - 1 && tuple[j].Column.IsParent(tuple[j + 1].Column))
					continue;
				result.Append((string)tuple[j].UniqueLevelValue);
				if(j != count - 1)
					result.Append(", ");
			}
			result.Append(" )");
		}
	}
	public class OLAPTuplesWriter : OLAPMembersWriter {
		public OLAPTuplesWriter()
			: base() {
		}
		public void WriteAllTuples(List<QueryTuple> tuples, StringBuilder result) {
			CheckTuples(tuples);
			result.Append("{ ");
			for(int i = 0; i < tuples.Count; i++) {
				WriteTuple(result, tuples[i]);
				if(i != tuples.Count - 1)
					result.Append(", ");
			}
			result.Append(" } ");
		}
		void CheckTuples(List<QueryTuple> tuples) {
			if(tuples.Count == 0) throw new Exception("Invalid tuple count (0)");
			int memberCount = tuples[0].MemberCount;
			for(int i = 0; i < tuples.Count; i++)
				if(tuples[i].MemberCount != memberCount) throw new Exception("Tuples have different count of members");
		}
	}
	public class OLAPColumnMembersWriter : OLAPMembersWriter {
		readonly IOLAPFilterHelper filterHelper;
		public OLAPColumnMembersWriter(IOLAPFilterHelper filterHelper)
			: base() {
				this.filterHelper = filterHelper;
		}
		protected IOLAPFilterHelper FilterHelper { get { return filterHelper; } }
		public void WriteAllColumnMembers(QueryBuilder queryBuilder, StringBuilder result,
			   OLAPCubeColumn column) {
			WriteAllColumnMembers(queryBuilder, result, column, delegate(StringBuilder result2) {
				WriteAllColumnMembersCore(queryBuilder, result2, column);
			});
		}
		public virtual void WriteAllColumnMembers(QueryBuilder queryBuilder, StringBuilder result,
			   OLAPCubeColumn column, Action<StringBuilder> writeContent) {
			result.Append("{ ");
			if(column.TopValueCount == 0)
				writeContent(result);
			else
				WriteTopMembers(queryBuilder, result, column, writeContent);
			result.Append("} ");
		}
		protected void WriteAllColumnMembersCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column) {
			if(column.Filtered) {
				OLAPFilterValues filterValues = FilterHelper.GetFilterValues(null, column, false, false);
				WriteFilteredColumnMembers(result, new WriteFilteredMembersParams(queryBuilder, column, filterValues, true, true));
			} else
				WriteUnfilteredColumnMembers(result, new WriteMembersParams(queryBuilder, column, true));
		}
		void WriteExcludedFilterValues(StringBuilder result, WriteFilteredMembersParams param) {
			OLAPFilterValues filterValues = param.FilterValues;
			if(filterValues.GetMemberCount() == 0)
				throw new ArgumentException("no filter items");
			if(filterValues.Depth == 1) {
				WriteMembersParams unfilteredParam = new WriteMembersParams(param);
				unfilteredParam.Column = filterValues.Column;
				WriteUnfilteredColumnMembers(result, unfilteredParam);
				result.Append(" - ");
				WriteMembers(result, filterValues.GetLevelFilter(), true);
			} else {
				if(param.Builder != null && !param.Builder.IsSubSelect)
					result.Append(WriteHierarchicalFilter(param.Builder, filterValues, param.AddCalculatedMembers, param.Column));
				else
					result.Append(GetMultiLevelExcludedFilterValuesString(filterValues, param.AddCalculatedMembers));
			}
		}
		string WriteHierarchicalFilter(QueryBuilder queryBuilder, OLAPFilterValues filterValues, bool addCalculatedMembers, OLAPCubeColumn column) {
			OLAPLevelFilter[] membersByLevels = filterValues.GetMembersByLevels();
			List<string> names = new List<string>();
			for(int i = 0; i < membersByLevels.Length; i++) {
				string name = MDX.Members(membersByLevels[i].Column, addCalculatedMembers);
				names.Add(name);
				if(column == membersByLevels[i].Column)
					break;
			}
			return new StringBuilder().Append("visualtotals(hierarchize(").Append(MDX.GetSet<string>(names)).Append("))").ToString();
		}
		string GetMultiLevelExcludedFilterValuesString(OLAPFilterValues filterValues, bool addCalculatedMembers) {
			if(filterValues.Column.Hierarchy.Structure == 2) {
				return GetRaggedExcludedMultilevelFilter(filterValues, addCalculatedMembers);
			}
			StringBuilder res = new StringBuilder();
			OLAPLevelFilter[] membersByLevels = filterValues.GetMembersByLevels();
			for(int i = 0; i < membersByLevels.Length; i++) {
				OLAPLevelFilter members = membersByLevels[i];
				if(i == 0) {
					WriteUnfilteredColumnMembers(res, new WriteMembersParams(null, members.Column, addCalculatedMembers));
					res.Append(" - ");
					WriteMembers(res, members, true);
				} else {
					res.Insert(0, "descendants(");
					res.Append(", ").Append(members.Column.UniqueName).Append(", LEAVES) - ");
					WriteMembers(res, members, true);
				}
			}
			return res.ToString();
		}
		string GetRaggedExcludedMultilevelFilter(OLAPFilterValues filterValues, bool addCalculatedMembers) {
			StringBuilder res = new StringBuilder();
			bool isgt2005 = filterValues.Column.Owner.Metadata.IsGT2005;
			List<IQueryMetadataColumn> columns = filterValues.Column.Metadata.GetColumnHierarchy();
			OLAPLevelFilter[] membersByLevels = filterValues.GetMembersByHierarchy(columns);
			for(int i = 0; i < membersByLevels.Length; i++) {
				OLAPLevelFilter members = membersByLevels[i];
				if(members != null && members.GetCount() != 0) {
					if(i != 0) {
						res.Insert(0, " DrillDownLevel(");
						res.Insert(0, " Except({ ");
					} else {
						res.Insert(0, " Except({ ");
						res.Append(MDX.Members(filterValues.Column, addCalculatedMembers));
					}
					if(i != 0) {
						res.Append(" , ").Append(columns[i - 1].UniqueName);
						if(addCalculatedMembers && isgt2005)
							res.Append(" , INCLUDE_CALC_MEMBERS ");
						res.Append(" )");
					}
					res.Append(" } ,");
					WriteMembers(res, members, true);
					res.Append(" )");
				} else {
					if(i == 0)
						res.Append(MDX.Members(filterValues.Column, addCalculatedMembers));
					else {
						res.Insert(0, " DrillDownLevel(");
						res.Append(" , ").Append(columns[i - 1].UniqueName);
						if(addCalculatedMembers && isgt2005)
							res.Append(" , INCLUDE_CALC_MEMBERS ");
						res.Append(" )");
					}
				}
			}
			for(int i = membersByLevels.Length; i < columns.Count; i++) {
				res.Insert(0, " DrillDownLevel(");
				res.Append(" , ").Append(columns[i - 1].UniqueName);
				if(addCalculatedMembers && isgt2005)
					res.Append(" , INCLUDE_CALC_MEMBERS ");
				res.Append(" )");
			}
			res.Insert(0, " Intersect( ");
			res.Append(" , ");
			res.Append(MDX.Descendants(filterValues.Column.Hierarchy.UniqueName, columns.Count, "LEAVES"));
			res.Append(")");
			return res.ToString();
		}
		static void WriteDescendantMembers(StringBuilder res, OLAPCubeColumn column, bool isIntersection) {
			if(column.Metadata.Hierarchy.Structure == 2 && isIntersection) {
				res.Append("{").Append(MDX.Descendants(column.Metadata.Hierarchy.UniqueName, column.Metadata.GetColumnHierarchy().Count, "LEAVES")).Append("}");
			} else
				res.Append(column.UniqueName).Append(".members");
		}
		bool WriteIncludedFilterValues(StringBuilder result, WriteFilteredMembersParams param) {
			if(param.FilterValues.Depth == 0)
				return false;
			OLAPLevelFilter filter = param.FilterValues.GetLevelFilter();
			if(!param.AddCalculatedMembers)
				filter.ExcludeCalculatedMembers();
			WriteMembers(result, filter, false);
			return true;
		}
		public string GetColumnFilteredMembers(WriteFilteredMembersParams param, bool isIntersection) {
			StringBuilder stb = new StringBuilder();
			WriteFilteredColumnMembers(stb, param, isIntersection);
			return stb.ToString();
		}
		public string GetColumnUnfilteredMembers(WriteMembersParams param, bool isIntersection) {
			StringBuilder stb = new StringBuilder();
			WriteUnfilteredColumnMembers(stb, param, isIntersection);
			return stb.ToString();
		}
		public bool WriteFilteredColumnMembers(StringBuilder result, WriteFilteredMembersParams param) {
			return WriteFilteredColumnMembers(result, param, false);
		}
		public bool WriteFilteredColumnMembers(StringBuilder result, WriteFilteredMembersParams param, bool isIntersection) {
			OLAPFilterValues filterValues = param.FilterValues;
			if(filterValues.IsIncluded) {
				if(param.Column.SortMode == PivotSortMode.None && param.KeepSortOrder) {
					result.Append("intersect ({ ");
					WriteDescendantMembers(result, param.Column, isIntersection);
					result.Append(" },{ ");
					WriteMembers(result, filterValues.GetLevelFilter(), false);
					result.Append(" }) ");
				} else
					return WriteIncludedFilterValues(result, param);
			} else {
				if(filterValues.GetMemberCount() == 0)
					WriteUnfilteredColumnMembers(result, param, false);   
				else
					WriteExcludedFilterValues(result, param);
			}
			return true;
		}
		public void WriteUnfilteredColumnMembers(StringBuilder result, WriteMembersParams param) {
			WriteUnfilteredColumnMembers(result, param, false);
		}
		public void WriteUnfilteredColumnMembers(StringBuilder result, WriteMembersParams param, bool isIntersection) {
			OLAPCubeColumn column = param.Column;
			if(column.HasCalculatedMembers && param.AddCalculatedMembers)
				result.Append("addcalculatedmembers({");
			else
				result.Append("{");
			WriteDescendantMembers(result, column, isIntersection);
			if(column.HasCalculatedMembers && param.AddCalculatedMembers)
				result.Append("})");
			else
				result.Append("}");
			if(param.Builder != null && column.HasCalculatedMembers 
					&& param.AddCalculatedMembers) {
				if(column.TotalMember != column.AllMember && 
						param.Builder.ContainsCalculatedMember(column.TotalMember.UniqueName))
					result.Append(" - {").Append(column.TotalMember.UniqueName).Append("}");
			}
		}
	}
	public class WriteMembersParams {
		bool addCalculatedMembers;
		QueryBuilder builder;
		OLAPCubeColumn column;		
		public WriteMembersParams() {
		}
		public WriteMembersParams(QueryBuilder builder, OLAPCubeColumn column, bool addCalculatedMembers) {
			this.builder = builder;
			this.column = column;
			this.addCalculatedMembers = addCalculatedMembers;
		}
		public WriteMembersParams(WriteMembersParams b) {
			Assign(b);
		}
		public QueryBuilder Builder {
			get { return builder; }
			set { builder = value; }
		}
		public OLAPCubeColumn Column {
			get { return column; }
			set { column = value; }
		}
		public bool AddCalculatedMembers {
			get { return addCalculatedMembers; }
			set { addCalculatedMembers = value; }
		}
		public virtual void Assign(WriteMembersParams b) {
			this.Builder = b.builder;
			this.Column = b.Column;
			this.AddCalculatedMembers = b.AddCalculatedMembers;
		}
	}
	public class WriteFilteredMembersParams : WriteMembersParams {
		bool keepSortOrder;
		OLAPFilterValues filterValues;
		public WriteFilteredMembersParams()
			: base() {
		}
		public WriteFilteredMembersParams(QueryBuilder builder, OLAPCubeColumn column, OLAPFilterValues filterValues,
				bool addCalculatedMembers, bool keepSortOrder)
			: base(builder, column, addCalculatedMembers) {
				this.filterValues = filterValues;
				this.keepSortOrder = keepSortOrder;
		}
		public WriteFilteredMembersParams(WriteFilteredMembersParams b) {
			Assign(b);
		}
		public OLAPFilterValues FilterValues {
			get { return filterValues; }
			set { filterValues = value; }
		}
		public bool KeepSortOrder {
			get { return keepSortOrder; }
			set { keepSortOrder = value; }
		}
		public override void Assign(WriteMembersParams b) {
			base.Assign(b);
			WriteFilteredMembersParams fb = b as WriteFilteredMembersParams;
			if(fb != null) {
				FilterValues = fb.FilterValues;
				KeepSortOrder = fb.KeepSortOrder;
			}
		}
	}
	public class OLAPSortedMembersWriter : OLAPColumnMembersWriter {
		Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember;
		public OLAPSortedMembersWriter(IOLAPFilterHelper filterHelper, Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember)
			: base(filterHelper) {
			if(createSortByTempMember == null)
				throw new ArgumentNullException("createSortByTempMember");
			this.createSortByTempMember = createSortByTempMember;
		}
		protected Func<OLAPCubeColumn, SortByTempMember> CreateSortByTempMember { get { return createSortByTempMember; } }
		public override void WriteAllColumnMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, 
				Action<StringBuilder> writeContent) {
			result.Append("{ ");
			if(column.TopValueCount == 0)
				WriteSortedMembers(queryBuilder, result, column, delegate() {
					writeContent(result);
				}, false);
			else {
				if(column.SortBySummary == null && column.SortMode != PivotSortMode.None)
					WriteTopMembers(queryBuilder, result, column, delegate(StringBuilder result2) {
						WriteSortedMembers(queryBuilder, result2, column, delegate() {
							writeContent(result2);
						}, false);
					});
					else
					WriteTopMembers(queryBuilder, result, column, writeContent);
			}
			result.Append("} ");
		}
		public void WriteSortedMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, Action writeContent, bool breakHierarchy) {
			WriteSortedMembers(queryBuilder, result, column, writeContent, CreateSortByTempMember(column), breakHierarchy);
		}
		public void WriteSortedMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, Action writeContent, SortByTempMember sortBy, bool breakHierarchy) {
			if(SortOnServer(column.SortMode, column.TopValueCount > 0, column.SortBySummary != null, column.SortBySummaryMembersExpanded) && column.SortMode != PivotSortMode.None) {
				queryBuilder.AddWithMember(sortBy);
				result.Append("order({");
				writeContent();
				result.Append("}, ").Append(sortBy.Name).Append(", ").Append(GetSortOrder(column, breakHierarchy)).Append(")");
			} else
				writeContent();
		}
		public static bool SortOnServer(PivotSortMode sortMode, bool hasTopN, bool hasSortBySummary, bool hasResolvedSortBySummary) {
			if(!hasSortBySummary && (sortMode == PivotSortMode.None))
				return true;
			if(hasResolvedSortBySummary) {
				return hasTopN;
			} else {
				return hasSortBySummary;
			}
		}
	}
	public class OLAPFirstLevelMembersWriter {
		readonly IOLAPFilterHelper filterHelper;
		readonly Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember;
		public OLAPFirstLevelMembersWriter(IOLAPFilterHelper filterHelper, Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember) {
			this.filterHelper = filterHelper;
			this.createSortByTempMember = createSortByTempMember;
		}
		public void WriteAllColumnMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, 
				bool applyFilter) {
			OLAPSortedMembersWriter sortedWriter = new OLAPSortedMembersWriter(this.filterHelper, this.createSortByTempMember);
			result.Append("{ ");
			if(column.IsAggregatable)
				result.Append(column.TotalMember.UniqueName).Append(", ");
			sortedWriter.WriteAllColumnMembers(queryBuilder, result, column, delegate(StringBuilder result2) {
				WriteAllColumnMembersCore(sortedWriter, queryBuilder, result2, column, applyFilter);
			});
			result.Append("} ");
		}
		void WriteAllColumnMembersCore(OLAPSortedMembersWriter sortedWriter, QueryBuilder queryBuilder, 
				StringBuilder result, OLAPCubeColumn column, bool applyFilter) {			
			if(column.Filtered) {				
				if(applyFilter || column.HasCalculatedMembers) {
					OLAPFilterValues filterValues = this.filterHelper.GetFilterValues(null, column, false, false);
					if(filterValues != null) {
						sortedWriter.WriteFilteredColumnMembers(result,
							new WriteFilteredMembersParams(queryBuilder, column, filterValues, true, applyFilter));
						return;
					}
				}
			} 
			sortedWriter.WriteUnfilteredColumnMembers(result, new WriteMembersParams(queryBuilder, column, true));
		}
	}
	public class OLAPAllFieldsExpandWriter : OLAPExpandWriter {
		public OLAPAllFieldsExpandWriter(IOLAPFilterHelper filterHelper, Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember) 
			: base(filterHelper, createSortByTempMember) { }
		bool nonEmpty;
		QueryBuilder builder;
		List<OLAPCubeColumn> expandColumns;
		public void WriteExpand(List<OLAPCubeColumn> columns, QueryBuilder builder, StringBuilder result, bool nonEmpty) {
			if(columns.Count == 0)
				return;
			expandColumns = columns;
			this.builder = builder;
			this.nonEmpty = nonEmpty;
			if(columns.Count > 1)
				WriteExpand(builder, result, columns[1], new List<QueryTuple>() { new QueryTuple((IEnumerable<QueryMember>)new OLAPVirtualMember(columns[0].Metadata, string.Format("[XtraPivotASet {0}].current", 0))) }, nonEmpty);
			else
				WriteAllColumnMembers(builder, result, columns[0]);
		}
		protected override string GetHierarchyExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, QueryTuple tuple) {
			StringBuilder result = new StringBuilder();
			result.Append(" generate( ");
			OLAPCubeColumn pColumn = expandColumns.Find((d) => d.Metadata == tuple[0].Column);
			new OLAPSortedMembersWriter(FilterHelper, CreateSortByTempMember).WriteAllColumnMembers(builder, result, pColumn
			,
				(sb) => {
					if(!pColumn.Filtered || !FilterHelper.HasGroupFilter(pColumn, false))
						WriteAllColumnMembersCore(queryBuilder, sb, pColumn);
					else
						WriteUnfilteredColumnMembers(sb, new WriteMembersParams(queryBuilder, pColumn, true));
				}
				);
			result.Append(" as [XtraPivotASet ");
			result.Append(expandColumns.IndexOf(expandColumns.Find((d) => d.Metadata == tuple[0].Column)));
			result.Append("] , ");
			if(expandColumns.IndexOf(column) == expandColumns.Count - 1) {
				result.Append(" { ");
				GetHierarchyExpandCore(queryBuilder, column, tuple, result);
			} else {
				WriteExpand(builder, result, expandColumns[expandColumns.IndexOf(column) + 1],
					new List<QueryTuple>() { new QueryTuple(new QueryMember[] { new OLAPVirtualMember(column.Metadata, string.Format("[XtraPivotASet {0}].current", expandColumns.IndexOf(column))) }) },
					nonEmpty);
			}
			result.Append(")");
			return result.ToString();
		}
		protected override string GetCrossJoinExpandCore(QueryBuilder queryBuilder, OLAPCubeColumn column, List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			result.Append(" generate( ");
			new OLAPSortedMembersWriter(FilterHelper, CreateSortByTempMember).WriteAllColumnMembers(builder, result, GetCubeColumn(tuples));
			result.Append(" as [XtraPivotASet ");
			result.Append(expandColumns.IndexOf(GetCubeColumn(tuples)));
			result.Append("] , ");
			WriteCrossJoinExpandCore(queryBuilder, result, column, tuples, allowInternalNonEmpty,
				() => {
					WriteSortedMembers(queryBuilder, result, column, delegate() {
						if(expandColumns.IndexOf(column) == expandColumns.Count - 1) {
							WriteAllColumnMembersCore(queryBuilder, result, column);
						} else {
							WriteExpand(builder, result, expandColumns[expandColumns.IndexOf(column) + 1],
						   new List<QueryTuple>() { new QueryTuple((IEnumerable<QueryMember>)new OLAPVirtualMember(column.Metadata, string.Format("[XtraPivotASet {0}].current", expandColumns.IndexOf(column)))) },
															  nonEmpty);
						}
					}, CreateSortByTempMember(column), false);
				});
			result.Append(")");
			return result.ToString();
		}
		OLAPCubeColumn GetCubeColumn(List<QueryTuple> tuples) {
			return FilterHelper.GetColumn((OLAPMetadataColumn)tuples[0].AllMembers[0].Column);
		}
		protected override string GetCrossJoinExpandTopCore(QueryBuilder queryBuilder, OLAPCubeColumn column, List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			result.Append(" generate( ");
			new OLAPSortedMembersWriter(FilterHelper, CreateSortByTempMember).WriteAllColumnMembers(builder, result, GetCubeColumn(tuples));
			result.Append(" as [XtraPivotASet ");
			result.Append(expandColumns.IndexOf(GetCubeColumn(tuples)));
			result.Append("] , ");
			result.Append(WriteTopMembersCore(column, delegate(StringBuilder result2) {
				if(column.SortBySummary == null && column.SortMode != PivotSortMode.None)
					WriteCrossJoinExpandCore(queryBuilder, result2, column, tuples, false, () => {
						WriteSortedMembers(queryBuilder, result2, column, delegate() {
							if(expandColumns.IndexOf(column) == expandColumns.Count - 1) {
								WriteAllColumnMembersCore(queryBuilder, result2, column);
							} else {
								WriteExpand(builder, result, expandColumns[expandColumns.IndexOf(column) + 1],
						new List<QueryTuple>() { new QueryTuple((IEnumerable<QueryMember>)new OLAPVirtualMember(column.Metadata, string.Format("[XtraPivotASet {0}].current", expandColumns.IndexOf(column)))) },
														   nonEmpty);
							}
						}, CreateSortByTempMember(column), allowInternalNonEmpty);
					});
				else
					WriteCrossJoinExpandCore(queryBuilder, result2, column, tuples, allowInternalNonEmpty);
			}, queryBuilder));
			result.Append(")");
			return result.ToString();
		}
	}
	public class OLAPExpandWriter : OLAPSortedMembersWriter {
		public OLAPExpandWriter(IOLAPFilterHelper filterHelper, Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember)
			: base(filterHelper, createSortByTempMember) {
		}
		public void WriteExpand(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, List<QueryTuple> tuples,
				bool allowInternalNonEmpty) {
			if(tuples[0].Last.IsTotal || tuples[0].Last.IsOthers)
				throw new Exception("Cannot expand total or others member");
			result.Append("{ ");
			if(tuples[0].Last.Column.IsParent(column.Metadata)) {
				foreach(QueryTuple tuple in tuples)
					result.Append(GetHierarchyExpand(queryBuilder, column, tuple)).Append(", ");
			} else
				result.Append(GetCrossJoinExpand(queryBuilder, column, tuples, allowInternalNonEmpty))
					.Append(", ");
			result.Length -= 2;
			result.Append(" } ");
		}
		protected virtual string GetHierarchyExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, QueryTuple tuple) {
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			WriteTuple(result, tuple);
			result.Append(", ");
			return GetHierarchyExpandCore(queryBuilder, column, tuple, result);
		}
		protected string GetHierarchyExpandCore(QueryBuilder queryBuilder, OLAPCubeColumn column, QueryTuple tuple, StringBuilder result) {
			Action<StringBuilder> writeDescendants = delegate(StringBuilder result2) {
				if(tuple.MemberCount > 1) {
					result2.Append("{");
					WriteTuple(result2, tuple, false);
					result2.Append("} * ");
				}
				if(column.SortBySummary != null)
					WriteDescendantsCore(result2, column, tuple);
				else
					WriteSortedMembers(queryBuilder, result2, column, delegate() {  WriteDescendantsCore(result2, column, tuple); }, false);
			};
			if(column.TopValueCount == 0)
				if(column.SortBySummary != null)
					WriteSortedMembers(queryBuilder, result, column, delegate() { writeDescendants(result); }, false);
				else
					writeDescendants(result); 
			else
				WriteTopMembers(queryBuilder, result, column, delegate(StringBuilder result2) {
					if(column.SortBySummary == null && column.SortMode != PivotSortMode.None)
						WriteSortedMembers(queryBuilder, result2, column, delegate() { writeDescendants(result2); }, false);
					else
						writeDescendants(result2);
				});
			result.Append(" } ");
			return result.ToString();
		}
		void WriteDescendantsCore(StringBuilder result, OLAPCubeColumn column, QueryTuple tuple) {
			if(!column.Filtered)
				result.Append(MDX.Descendants((OLAPMember)tuple.Last, column));
			else {
				OLAPFilterValues filterValues = FilterHelper.GetFilterValues(tuple, column, true, false);
				if(filterValues == null)
					result.Append(MDX.Descendants((OLAPMember)tuple.Last, column));
				else {
					if(filterValues.IsIncluded && filterValues.GetMemberCount() == 0)
						result.Append("{}");
					else {
						result.Append(
							MDX.Intersect(GetColumnFilteredMembers(new WriteFilteredMembersParams(null, column, filterValues, true, true), false),
								MDX.Descendants((OLAPMember)tuple.Last, column)));
					}
				}
			}
		}
		string GetCrossJoinExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, List<QueryTuple> tuples,
				bool allowInternalNonEmpty) {
			if(column.TopValueCount == 0)
				return GetCrossJoinExpandCore(queryBuilder, column, tuples, allowInternalNonEmpty);
			else
				return GetCrossJoinExpandTopCore(queryBuilder, column, tuples, allowInternalNonEmpty);
		}
		protected virtual string GetCrossJoinExpandCore(QueryBuilder queryBuilder, OLAPCubeColumn column,
				List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			WriteSortedMembers(queryBuilder, result, column, delegate() {
				WriteCrossJoinExpandCore(queryBuilder, result, column, tuples, allowInternalNonEmpty);
			}, CreateSortByTempMember(column), false);
			return result.ToString();
		}
		protected virtual string GetCrossJoinExpandTopCore(QueryBuilder queryBuilder, OLAPCubeColumn column,
				List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			foreach(QueryTuple tuple in tuples) {
				WriteTopMembers(queryBuilder, result, column, delegate(StringBuilder result2) {
					if(column.SortBySummary == null && column.SortMode != PivotSortMode.None)
						WriteSortedMembers(queryBuilder, result2, column, delegate() {
							WriteCrossJoinExpandCore(queryBuilder, result2, column, tuple, allowInternalNonEmpty);
						}, CreateSortByTempMember(column), false);
					else
						WriteCrossJoinExpandCore(queryBuilder, result2, column, tuple, allowInternalNonEmpty);
				});
				result.Append(", ");
			}
			result.Length -= 2;
			return result.ToString();
		}
		protected void WriteCrossJoinExpandCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column,
					 QueryTuple tuple, bool allowInternalNonEmpty) {
			WriteCrossJoinExpandCore(queryBuilder, result, column,
				new List<QueryTuple>(new QueryTuple[] { tuple }), allowInternalNonEmpty);
		}
		protected void WriteCrossJoinExpandCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column,
					QueryTuple tuple, bool allowInternalNonEmpty, Action writeContent) {
			WriteCrossJoinExpandCore(queryBuilder, result, column,
				new List<QueryTuple>(new QueryTuple[] { tuple }), allowInternalNonEmpty, writeContent);
		}
		protected void WriteCrossJoinExpandCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column,
					List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			WriteCrossJoinExpandCore(queryBuilder, result, column, tuples, allowInternalNonEmpty,
				() => {
					WriteAllColumnMembersCore(queryBuilder, result, column);
				});
		}
		protected void WriteCrossJoinExpandCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column,
					List<QueryTuple> tuples, bool allowInternalNonEmpty, Action writeContent) {
			bool allowNonEmpty = allowInternalNonEmpty && !column.HasCalculatedMembers && queryBuilder.AllowInternalNonEmpty;
			if(allowNonEmpty)
				result.Append("nonempty(");
			result.Append("crossjoin(");
			new OLAPTuplesWriter().WriteAllTuples(tuples, result);
			result.Append(", { ");
			writeContent();
			result.Append("} ");
			if(allowNonEmpty) {
				result.Append("), {");
				queryBuilder.WriteMeasures(result);
				result.Append("}) ");
			} else
				result.Append(") ");
		}
	}
	public class OLAPExpandWriter2008 : OLAPExpandWriter {
		public OLAPExpandWriter2008(IOLAPFilterHelper filterHelper, Func<OLAPCubeColumn, SortByTempMember> createSortByTempMember)
			: base(filterHelper, createSortByTempMember) {
		}
		protected override string GetCrossJoinExpandCore(QueryBuilder queryBuilder, OLAPCubeColumn column,
				List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			bool isSortingBySummary = column.SortBySummary != null;
			if(isSortingBySummary) {
				result.Append("generate(");
				new OLAPTuplesWriter().WriteAllTuples(tuples, result);
				result.Append(", ");
				QueryTuple currentMemberTuple = GetCurrentMembersTuple(tuples);
				tuples = new List<QueryTuple>();
				tuples.Add(currentMemberTuple);
			}
			WriteCrossJoinExpandCore(queryBuilder, result, column, tuples, allowInternalNonEmpty,
				() => {
					WriteSortedMembers(queryBuilder, result, column, delegate() {
						WriteAllColumnMembersCore(queryBuilder, result, column);
					}, CreateSortByTempMember(column), false);
				});
			if(isSortingBySummary)
				result.Append(")");
			return result.ToString();
		}
		protected override string GetCrossJoinExpandTopCore(QueryBuilder queryBuilder, OLAPCubeColumn column, List<QueryTuple> tuples, bool allowInternalNonEmpty) {
			StringBuilder result = new StringBuilder();
			result.Append("generate(");
			new OLAPTuplesWriter().WriteAllTuples(tuples, result);
			result.Append(", ");
			QueryTuple currentMemberTuple = GetCurrentMembersTuple(tuples);
			tuples = new List<QueryTuple>();
			tuples.Add(currentMemberTuple);
			result.Append(WriteTopMembersCore(column, delegate(StringBuilder result2) {
				if(column.SortBySummary == null && column.SortMode != PivotSortMode.None)
					WriteCrossJoinExpandCore(queryBuilder, result2, column, tuples, false, () => {
						WriteSortedMembers(queryBuilder, result2, column, delegate() {
							WriteAllColumnMembersCore(queryBuilder, result2, column);
						}, CreateSortByTempMember(column), allowInternalNonEmpty);
					});
				else
					WriteCrossJoinExpandCore(queryBuilder, result2, column, tuples, allowInternalNonEmpty);
			}, queryBuilder));
			result.Append(")");
			return result.ToString();
		}
		protected QueryTuple GetCurrentMembersTuple(List<QueryTuple> tuples) {
			QueryTuple tuple0 = tuples[0];
			OLAPMember[] currentMembers = new OLAPMember[tuple0.MemberCount];
			for(int i = 0; i < tuple0.MemberCount; i++) {
				OLAPMetadataColumn column = (OLAPMetadataColumn)tuple0[i].Column;
				currentMembers[i] = new OLAPVirtualMember(column, column.Hierarchy + ".currentmember");
			}
			return new QueryTuple(currentMembers);
		}
	}
}
