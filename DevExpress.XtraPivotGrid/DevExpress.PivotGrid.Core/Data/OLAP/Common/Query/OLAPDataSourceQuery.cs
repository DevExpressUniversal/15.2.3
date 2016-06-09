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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public abstract class OLAPDataSourceQueryBase {
		public const string InvalidColumnsCountExceptionText = "Invalid columns count";
		public const string MeasuresStringName = "[Measures]";
		public const string TempMeasureNameFormat = "[Measures].[XtraPivotGrid Temp Measure {0}]";
		public const string CountMeasureStringTuple = "[Measures].[XtraPivotGrid Count Measure]";
		public const string FilterMemberString = " XtraPivotGrid Filter]";
		public const string OthersMemberString = " XtraPivotGrid Others]";
		public const string EmptyMeasureStringTuple = "[Measures].[XtraPivotGrid Empty]";
		public const string ValueMeasureName = "[Measures].[XtraPivotGrid Member Value]";
		public const string CaptionMeasureName = "[Measures].[XtraPivotGrid Member Caption]";
		public const string MemberPropertyMeasureFormat = "[Measures].[XtraPivotGrid Member {0}]";
		public const string IsCalculatedMeasureName = "[Measures].[XtraPivotGrid Member IsCalculated]";
		public const string MemberUniqueNameName = "[MEMBER_UNIQUE_NAME]";
		public const string MemberUniqueName = "MEMBER_UNIQUE_NAME";
		public const string MemberTypeName = "[MEMBER_TYPE]";
		public const string MemberType = "MEMBER_TYPE";
		public const string MemberValue = "MEMBER_VALUE";
		public static bool IsOthersMember(string uniqueName) { return uniqueName.EndsWith(OthersMemberString); }
		public static string GetVisualTotalsName(OLAPCubeColumn column) { return "[" + column.Name + " Visual Totals]"; }
		public static string GetFilterMember(string hierarchy, string name) { return hierarchy + ".[" + name + FilterMemberString; }
		public static string GetFilterMember(OLAPCubeColumn column) { return GetFilterMember(column.Metadata.Hierarchy.UniqueName, column.Name); }
		IOLAPDataSourceQueryOwner owner;
		protected OLAPDataSourceQueryBase(IOLAPDataSourceQueryOwner owner) {
			this.owner = owner;
		}
		protected abstract bool AllowInternalNonEmpty { get; }
		protected PivotGridOptionsOLAP Options { get { return owner.Options; } }
		protected IOLAPFilterHelper FilterHelper { get { return owner.FilterHelper; } }
		protected virtual bool DimensionPropertiesMemberValueSupported { get { return owner.DimensionPropertiesSupported; } }
		public virtual string GetDrillDownQueryString(string cubeName, List<string> filteredValues,
													List<string> returnColumns, int maxRowCount) {
			if(string.IsNullOrEmpty(cubeName))
				return null;
			StringBuilder result = new StringBuilder("drillthrough ");
			if(maxRowCount > 0)
				result.Append("maxrows " + maxRowCount.ToString() + " ");
			result.Append("select from [").Append(cubeName).Append("] ");
			if(filteredValues.Count > 0) {
				result.Append("where ( ");
				foreach(string member in filteredValues)
					result.Append(member).Append(", ");
				result.Length -= 2;
				result.Append(" )");
			}
			return result.ToString();
		}
		public string GetQueryString(string cubeName, List<OLAPCubeColumn> columns, List<OLAPCubeColumn> rows, List<OLAPCubeColumn> columnRowCustomDefaultMemberColumns,
			List<QueryTuple> columnTuples, List<QueryTuple> rowTuples, List<OLAPCubeColumn> measures, List<OLAPCubeColumn> columnRowFilters,
			List<OLAPCubeColumn> filters, bool columnExpand, bool rowExpand) {
			if(measures == null || measures.Count == 0 || string.IsNullOrEmpty(cubeName))
				return null;
			QueryBuilder result = new QueryBuilder(cubeName, AllowInternalNonEmpty);
			result.Measures.AddRange(measures);
			if(!columnExpand) {
				for(int i = 0; i < columns.Count; i++) {
					if(!columns[0].IsAggregatable && AllowInternalNonEmpty || (i > 0 && columns[i - 1].IsParent(columns[i]) || !columns[i].HasCustomTotal))
						continue;
					result.AddWithMember(CreateTotal(result, columns[i]));
				}
			}
			if(!rowExpand) {
				for(int i = 0; i < rows.Count; i++) {
					if(!rows[0].IsAggregatable && AllowInternalNonEmpty || (i > 0 && rows[i - 1].IsParent(rows[i]) || !rows[i].HasCustomTotal))
						continue;
					result.AddWithMember(CreateTotal(result, rows[i]));
				}
			}
			CreateWhereColumnRow(columnRowCustomDefaultMemberColumns, result);
			if(!CreateColumnRowFilters(result, columnRowFilters, false) || !CreateWhereFilters(result, filters))
				return null;
			if(columns.Count > 0) {
				result.OnColumns.Append(GetEverything(result, columns, columnTuples, columnExpand));
				result.OnColumnsDimensionProperties = AppendDimensionProperties(columns);
			}
			if(rows.Count > 0) {
				result.OnRows.Append(GetEverything(result, rows, rowTuples, rowExpand));
				result.OnRowsDimensionProperties = AppendDimensionProperties(rows);
			}
			result.CubeName = cubeName;
			result.CellProperties.Append(" VALUE, FORMAT_STRING, LANGUAGE ");
			return result.ToString();
		}
		protected string AppendDimensionProperties(IEnumerable<OLAPCubeColumn> columns) {
			StringBuilder builder = new StringBuilder().Append(" dimension properties ");
			if(DimensionPropertiesMemberValueSupported) {
				builder.Append(MemberValue).Append(" ");
				if(columns != null)
					foreach(OLAPCubeColumn column in columns) {
						foreach(OLAPPropertyDescriptor property in column.AutoProperties)
							builder.Append(", ").Append(column.UniqueName).Append(".[").Append(property.Name).Append("]");
					}
			} else {
				builder.Append(MemberUniqueName).Append(" ");
			}
			return builder.ToString();
		}
		protected virtual void CreateWhereColumnRow(List<OLAPCubeColumn> columnRowCustomDefaultMemberColumns, QueryBuilder result) { }
		internal string GetEverything(QueryBuilder queryBuilder, List<OLAPCubeColumn> columns, List<QueryTuple> tuples, bool isExpand) {
			StringBuilder result = new StringBuilder();
			if(isExpand) {
				CheckSingleColumn(columns);
				CreateExpandWriter().WriteExpand(queryBuilder, result, columns[0], tuples, AllowInternalNonEmpty);
			} else {
				if(tuples.Count > 0)
					new OLAPTuplesWriter().WriteAllTuples(tuples, result);
				else {
					CheckSingleColumn(columns);
					WriteFirstLevelMembers(queryBuilder, columns, result);
				}
			}
			return result.ToString();
		}
		void CheckSingleColumn(List<OLAPCubeColumn> columns) {
			if(columns.Count != 1)
				throw new Exception(InvalidColumnsCountExceptionText);
		}
		protected abstract bool CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters, bool deferUpdates);
		protected abstract void WriteFirstLevelMembers(QueryBuilder queryBuilder, List<OLAPCubeColumn> columns, StringBuilder result);
		protected QueryTempMember CreateTotal(QueryBuilder queryBuilder, OLAPCubeColumn column) {
			if(!column.HasCustomTotal)
				throw new ArgumentException("!column.HasCustomTotal");
			StringBuilder mdx = new StringBuilder();
			mdx.Append("aggregate(");
			if(column.TopValueCount > 0) {
				OLAPSortedMembersWriter writer = new OLAPSortedMembersWriter(FilterHelper, CreateSortByTempMember);
				writer.WriteAllColumnMembers(queryBuilder, mdx, column);
			} else {
				OLAPColumnMembersWriter writer = new OLAPColumnMembersWriter(FilterHelper);
				writer.WriteAllColumnMembers(queryBuilder, mdx, column);
			}
			mdx.Append(")");
			return new QueryTempMember(true, column.TotalMember.UniqueName, mdx.ToString());
		}
		protected QueryTempMember CreateVisualTotalsSet(List<string> members, int setId) {
			StringBuilder mdx = new StringBuilder();
			mdx.Append("visualtotals(hierarchize(");
			new OLAPMembersWriter().WriteMembers(mdx, members, true);
			mdx.Append("))");
			return new QueryTempMember(false, "XtraPivotGridVTSet" + setId.ToString(), mdx.ToString());
		}
		protected QueryTempMember CreateVisualTotalsSet(string content, int setId) {
			StringBuilder mdx = new StringBuilder();
			mdx.Append("visualtotals(hierarchize({");
			mdx.Append(content);
			mdx.Append("}))");
			return new QueryTempMember(false, "XtraPivotGridVTSet" + setId.ToString(), mdx.ToString());
		}
		protected bool CreateWhereFilters(QueryBuilder result, List<OLAPCubeColumn> filters) {
			return CreateWhereFilters(result, filters, false, false);
		}
		protected bool CreateWhereFilters(QueryBuilder result, List<OLAPCubeColumn> filters, bool forceWhere, bool deferUpdates) {
			for(int i = filters.Count - 1; i >= 0; i--) {
				if(i < filters.Count - 1 && filters[i].IsParent(filters[i + 1]))
					continue;
				List<OLAPCubeColumn> parentFilters = GetParentFilters(filters, i);
				if(!CreateWhereFilterCore(ref result, filters[i], parentFilters, forceWhere, deferUpdates))
					return false;
			}
			return true;
		}
		List<OLAPCubeColumn> GetParentFilters(List<OLAPCubeColumn> filters, int fieldIndex) {
			List<OLAPCubeColumn> res = null;
			OLAPCubeColumn filter = filters[fieldIndex];
			for(int i = fieldIndex - 1; i >= 0; i--) {
				OLAPCubeColumn column = filters[i];
				if(!column.IsParent(filter))
					break;
				if(res == null)
					res = new List<OLAPCubeColumn>();
				res.Add(column);
			}
			return res;
		}
		protected virtual bool CreateWhereFilterCore(ref QueryBuilder result, OLAPCubeColumn filter, List<OLAPCubeColumn> parentFilters, bool forceWhere, bool deferUpdates) {
			OLAPFilterValues filterValues;
			if(parentFilters == null)
				filterValues = FilterHelper.GetFilterValues(null, filter, true, deferUpdates);
			else
				if(FilterHelper.HasGroupFilter(parentFilters[parentFilters.Count - 1], deferUpdates))
					filterValues = FilterHelper.GetFilterValues(null, parentFilters[parentFilters.Count - 1], true, deferUpdates);
				else
					filterValues = null;
			if(filterValues != null && filterValues.IsIncluded && filterValues.GetMemberCount() == 1 &&
					!Options.UseAggregateForSingleFilterValue) {
				result.WhereMembers.Add(filterValues.GetSingleMember());
			} else {
				QueryTempMember tempMember = CreateFilterAggregate(filter, parentFilters, deferUpdates);
				result.AddWithMember(tempMember);
				result.WhereMembers.Add(tempMember.Name);
			}
			return true;
		}
		protected QueryTempMember CreateFilterAggregate(OLAPCubeColumn column, List<OLAPCubeColumn> parentFilters, bool deferUpdates) {
			StringBuilder mdx = new StringBuilder();
			InitFilterAggregate(mdx, column);
			mdx.Append("{");
			OLAPColumnMembersWriter writer = new OLAPColumnMembersWriter(FilterHelper);
			OLAPFilterValues filterValues = FilterHelper.GetFilterValues(null, column, true, deferUpdates);
			if(parentFilters == null)
				writer.WriteFilteredColumnMembers(mdx,
					new WriteFilteredMembersParams(null, column, filterValues, false, false));
			else {
				string expr = GetFilterValuesExpression(column, writer, filterValues, true);
				for(int i = parentFilters.Count - 1; i >= 0; i--) {
					OLAPCubeColumn parent = parentFilters[i];
					OLAPFilterValues parentFilterValues = FilterHelper.GetFilterValues(null, parent, true, deferUpdates);
					expr = MDX.Intersect(
								MDX.Descendants(
									GetFilterValuesExpression(parent, writer, parentFilterValues, false),
									column, parent.Hierarchy.Structure == 2 ? "LEAVES" : null),
									expr);
				}
				mdx.Append(expr);
			}
			mdx.Append("})");
			return new QueryTempMember(true, GetFilterMember(column), mdx.ToString());
		}
		protected virtual void InitFilterAggregate(StringBuilder mdx, OLAPCubeColumn column) {
			mdx.Append("aggregate(");
		}
		string GetFilterValuesExpression(OLAPCubeColumn column, OLAPColumnMembersWriter writer, OLAPFilterValues filterValues, bool isIntersection) {
			string expr = null;
			if(filterValues != null)
				expr = writer.GetColumnFilteredMembers(new WriteFilteredMembersParams(null, column, filterValues, false, false), isIntersection);
			else
				expr = writer.GetColumnUnfilteredMembers(new WriteMembersParams(null, column, false), isIntersection);
			return expr;
		}
		protected abstract string MemberValuePropertyName { get; }
		protected abstract string MemberCaptionPropertyName { get; }
		public string GetMembersQueryString(string cubeName, OLAPMetadataColumn meta, OLAPCubeColumn column, string[] members, List<OLAPMetadataColumn> nonaggregatables,
												int head, int tail, bool canQueryValueFast) {
			QueryBuilder result = CreateMemberValueRequest(cubeName, meta, column, nonaggregatables, canQueryValueFast);
			if(result != null) {
				string onRows = result.OnRows.ToString();
				result.OnRows.Clear();
				result.OnRows.Append("{ ");
				if(members == null || members.Length == 0) {
					if(tail > 0)
						result.OnRows.Append("tail(");
					if(head > 0)
						result.OnRows.Append("head(");
					result.OnRows.Append("addcalculatedmembers(").Append(meta.UniqueName).Append(".members)");
					if(head > 0) {
						result.OnRows.Append(", ");
						result.OnRows.Append(head);
						result.OnRows.Append(" )");
					}
					if(tail > 0) {
						result.OnRows.Append(", ");
						result.OnRows.Append(tail);
						result.OnRows.Append(" )");
					}
				} else {
					for(int i = 0; i < members.Length; i++) {
						result.OnRows.Append(members[i]);
						if(i != members.Length - 1)
							result.OnRows.Append(", ");
					}
				}
				result.OnRows.Append(onRows);
				return result.ToString();
			} else
				return null;
		}
		public string GetChildMembersQueryString(OLAPMember member, OLAPMetadataColumn meta, OLAPCubeColumn childColumn, string cubeName, List<OLAPMetadataColumn> nonaggregatables, bool canQueryValueFast) {
			QueryBuilder result = CreateMemberValueRequest(cubeName, meta, childColumn, nonaggregatables, canQueryValueFast);
			if(result != null) {
				result.OnRows.Insert(0, ".children)");
				result.OnRows.Insert(0, member.UniqueName);
				result.OnRows.Insert(0, "{addcalculatedmembers(");
				return result.ToString();
			} else
				return null;
		}
		QueryBuilder CreateMemberValueRequest(string cubeName, OLAPMetadataColumn meta, OLAPCubeColumn childColumn, List<OLAPMetadataColumn> nonaggregatables, bool canQueryValueFast) {
			if(string.IsNullOrEmpty(cubeName))
				return null;
			QueryBuilder result = new QueryBuilder(cubeName, AllowInternalNonEmpty);
			result.NonEmptyBehaviour = false;
			if(!canQueryValueFast) {
				List<string> measureNames = new List<string>();
				QueryTempMember valueMeasure = new QueryTempMember(true, ValueMeasureName, meta.Hierarchy.UniqueName + ".currentmember.Properties(" + MemberValuePropertyName + ")");
				result.AddWithMember(valueMeasure);
				measureNames.Add(valueMeasure.Name);
				QueryTempMember captionMeasure = new QueryTempMember(true, CaptionMeasureName, meta.Hierarchy.UniqueName + ".currentmember.Properties(" + MemberCaptionPropertyName + ")");
				result.AddWithMember(captionMeasure);
				measureNames.Add(captionMeasure.Name);
				if(childColumn != null) {
					foreach(OLAPPropertyDescriptor property in childColumn.AutoProperties) {
						QueryTempMember propertyMeasure =
							new QueryTempMember(true, string.Format(MemberPropertyMeasureFormat, property.Name), meta.Hierarchy.UniqueName + ".currentmember.Properties(\"" + property.Name + "\", TYPED)");
						result.AddWithMember(propertyMeasure);
						measureNames.Add(propertyMeasure.Name);
					}
				}
				result.OnColumns.Append("{ ").Append(string.Join(", ", measureNames)).Append(" }");
			} else
				result.AddOneValueOnColumns();
			AddNonAggregatables(meta, nonaggregatables, result);
			result.OnRows.Append(" }").Append(AppendDimensionProperties(childColumn != null ? Enumerable.Repeat(childColumn, 1) : Enumerable.Empty<OLAPCubeColumn>()));
			if(AllowInternalNonEmpty)
				result.CellProperties.Append(" VALUE ");
			return result;
		}
		protected void AddNonAggregatables(OLAPMetadataColumn column, List<OLAPMetadataColumn> nonaggregatables, QueryBuilder result) {
			List<OLAPMetadataColumn> filteredList = new List<OLAPMetadataColumn>(nonaggregatables.Count);
			for(int i = 0; i < nonaggregatables.Count; i++) {
				if(CanAddNonAggregatable(column, nonaggregatables[i]))
					filteredList.Add(nonaggregatables[i]);
			}
			result.CreateNonAggregatables(filteredList);
		}
		protected virtual bool CanAddNonAggregatable(OLAPMetadataColumn column, OLAPMetadataColumn nonaggregatable) {
			return nonaggregatable != column && !nonaggregatable.IsParent(column);
		}
		public abstract string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy);
		protected virtual string GetUniqueNameScript(OLAPCubeColumn column) {
			return column.Hierarchy.UniqueName + ".currentmember.properties(\"unique_name\")";
		}
		public string GetSortedMembersQueryString(string cubeName, OLAPMember[] members, OLAPCubeColumn column, List<OLAPMetadataColumn> nonaggregatables, bool canQueryValueFast) {
			QueryBuilder result = CreateMemberValueRequest(cubeName, column.Metadata, column, nonaggregatables, canQueryValueFast);
			OLAPSortedMembersWriter writer = new OLAPSortedMembersWriter(FilterHelper, CreateSortByTempMember);
			string onRows = result.OnRows.ToString();
			result.OnRows.Clear();
			result.OnRows.Append("{ ");
			if(members == null || members.Length == 0) {
				writer.WriteSortedMembers(result, result.OnRows, column, delegate() {
					writer.WriteUnfilteredColumnMembers(result.OnRows, new WriteMembersParams(result, column, true));
				}, true);
			} else {
				writer.WriteSortedMembers(result, result.OnRows, column, delegate() {
					writer.WriteMembers(result.OnRows, members, true);
				}, true);
			}
			result.OnRows.Append(onRows);
			return result.ToString();
		}
		protected abstract SortByTempMember CreateSortByTempMember(OLAPCubeColumn column);
		protected virtual OLAPExpandWriter CreateExpandWriter() {
			return new OLAPExpandWriter(FilterHelper, CreateSortByTempMember);
		}
		public string GetKPIValueQuery(string kpiName, string cubeName) {
			return string.Format("select {{ KPIValue(\"{0}\"), KPIGoal(\"{0}\"), KPIStatus(\"{0}\"), KPITrend(\"{0}\"), KPIWeight(\"{0}\") }} " +
				"on columns from [{1}] cell properties Value, {2}, {3} ", kpiName, cubeName, OlapProperty.CellFormatString, OlapProperty.LANGUAGE);
		}
		public virtual string GetAvailableValuesQuery(OLAPCubeColumn column, string cubeName,
							List<OLAPCubeColumn> subSelectFilters, List<OLAPCubeColumn> measures,
							List<OLAPMetadataColumn> nonaggregatables, bool deferUpdates) {
			List<OLAPCubeColumn> filters = new List<OLAPCubeColumn>(subSelectFilters);
			filters.Remove(column);
			return GetVisibleValuesQuery(column, cubeName, filters, measures, nonaggregatables, true, deferUpdates);
		}
		public string GetVisibleValuesQuery(OLAPCubeColumn column, string cubeName,
							List<OLAPCubeColumn> subSelectFilters, List<OLAPCubeColumn> measures,
							List<OLAPMetadataColumn> nonaggregatables, bool queryAvailableValues, bool deferUpdates) {
			if(string.IsNullOrEmpty(cubeName))
				return null;
			QueryBuilder result = new QueryBuilder(cubeName, AllowInternalNonEmpty);
			List<OLAPMetadataColumn> newNonaggregatables = new List<OLAPMetadataColumn>();
			newNonaggregatables.AddRange(nonaggregatables);
			List<OLAPCubeColumn> whereFilters = new List<OLAPCubeColumn>();
			if(queryAvailableValues) {
				for(int i = 0; i < subSelectFilters.Count; i++)
					newNonaggregatables.Remove(subSelectFilters[i].Metadata);
				AddNonAggregatables(column.Metadata, newNonaggregatables, result);
				whereFilters.AddRange(subSelectFilters);
				subSelectFilters = new List<OLAPCubeColumn>();
				foreach(OLAPMetadataColumn child in column.Metadata.GetColumnHierarchy()) {
					OLAPCubeColumn childColumn = whereFilters.Find((d) => d.Metadata == child);
					if(whereFilters.Contains(childColumn)) {
						whereFilters.Remove(childColumn);
						subSelectFilters.Add(childColumn);
					}
				}
			} else
				AddNonAggregatables(column.Metadata, nonaggregatables, result);
			bool nonEmpty = true;
			for(int i = 0; i < measures.Count; i++)
				if(!measures[i].OLAPUseNonEmpty) {
					nonEmpty = false;
					break;
				}
			nonEmpty = nonEmpty && measures.Count > 0 && AllowInternalNonEmpty;
			if(nonEmpty) {
				result.OnRows.Append("{ ").Append("NONEMPTY(")
				   .Append("addcalculatedmembers(").Append(column.UniqueName).Append(".members)").Append(",").Append("{")
					.Append(string.Join(",", measures.Select((column1) => column1.UniqueName).ToArray()))
					.Append("}").Append(")").Append(" } ");
				result.AddOneValueOnColumns();
			} else {
				result.OnRows.Append("{ ")
					.Append("addcalculatedmembers(").Append(column.UniqueName).Append(".members)")
					.Append(" } ");
				result.Measures.AddRange(measures);
			}
			if(!DimensionPropertiesMemberValueSupported)
				result.OnRows.Append(AppendDimensionProperties(Enumerable.Repeat(column, 1)));
			if(!queryAvailableValues) {
				bool isColumn = column.Owner.Areas.ColumnArea.Contains(column);
				bool isRow = column.Owner.Areas.RowArea.Contains(column);
				if(!column.TopValueShowOthers) {
					int index = -1;
					if(isColumn)
						index = column.Owner.Areas.ColumnArea.IndexOf(column);
					else if(isRow)
						index = column.Owner.Areas.RowArea.IndexOf(column);
					List<OLAPCubeColumn> area = isColumn ? column.Owner.Areas.ColumnArea : column.Owner.Areas.RowArea;
					if(index > 0 && area.Find((col) => col.TopValueCount > 0) != null) {
						List<OLAPCubeColumn> parentColumns = new List<OLAPCubeColumn>();
						for(int i = 0; i < index; i++)
							parentColumns.Add(area[i]);
						QueryBuilder queryBuilder = result.GetInnerSubSelect();
						queryBuilder = queryBuilder.CreateInnerSubSelect();
						StringBuilder builder = new StringBuilder();
						queryBuilder.Measures.AddRange(measures);
						new OLAPAllFieldsExpandWriter(FilterHelper, CreateSortByTempMember).WriteExpand(area, queryBuilder, builder, nonEmpty);
						queryBuilder.Measures.Clear();
						string str = builder.ToString();
						List<string> sets = new List<string>();
						foreach(KeyValuePair<string, QueryTempMember> pair1 in queryBuilder.WithMembers)
							foreach(KeyValuePair<string, QueryTempMember> pair in queryBuilder.WithMembers) {
								str = str.Replace(pair.Value.Name, pair.Value.MDX);
								if(!pair.Value.IsMember)
									sets.Add(pair.Key);
							}
						foreach(string set in sets)
							queryBuilder.WithMembers.Remove(set);
						queryBuilder.OnColumns.Append(str);
					}
				}
				if(isColumn || isRow) {
					IList<OLAPCubeColumn> crossArea = column.Owner.Areas.GetArea(!isColumn);
					if(crossArea.Count > 0 && subSelectFilters.Contains(crossArea[0])) {
						subSelectFilters.Remove(crossArea[0]);
						whereFilters.Add(crossArea[0]);
					}
				}
			}
			bool createFilterResult =
			   (whereFilters.Count > 0 ? CreateWhereFilters(result, whereFilters, true, deferUpdates) : true) &&
			   (subSelectFilters.Count > 0 ? CreateColumnRowFilters(result, subSelectFilters, deferUpdates) : true);
			return createFilterResult ? result.ToString() : null;
		}
		public string GetCustomSummaryString(OLAPAreas areas, string cubeName, IList<AggregationLevel> levels, List<AggregationItemValue> actions) {
			int num = 0;
			QueryBuilder queryBuilder = new QueryBuilder(cubeName, true);
			queryBuilder.NonEmptyBehaviour = false;
			queryBuilder.OnColumns.Append(" { ");
			List<OLAPCubeColumn> subSelectFilters = MDX.GetAllFilteredColumns(false, null, owner.FilterHelper);
			foreach(AggregationLevel level in levels) {
				int rowLevel = level.Row;
				int columnLevel = level.Column;
				List<OLAPCubeColumn> columns = new List<OLAPCubeColumn>();
				for(int i = 0; i <= columnLevel; i++)
					columns.Add(areas.ColumnArea[i]);
				for(int i = 0; i <= rowLevel; i++)
					columns.Add(areas.RowArea[i]);
				foreach(AggregationCalculation calc in level) {
					if(calc.Target != AggregationCalculatationTarget.Data)
						throw new NotImplementedException();
					int dataIndex = calc.Index;
					OLAPCubeColumn measure = areas.DataArea[dataIndex];
					StringBuilder expandWriterBuilder = new StringBuilder();
					queryBuilder.Measures.Add(measure);
					new OLAPAllFieldsExpandWriter(FilterHelper, CreateSortByTempMember).WriteExpand(columns, queryBuilder, expandWriterBuilder, true);
					queryBuilder.Measures.Clear();
					string expandMemberName = string.Format("[XtraPivotGridExpand{0}_{1}_{2}]", columnLevel, rowLevel, dataIndex);
					if(columns.Count > 0)
						queryBuilder.AddWithMember(new QueryTempMember(false, expandMemberName, expandWriterBuilder.ToString()));
					foreach(AggregationItemValue item in calc) {
						actions.Add(item);
						SummaryItemTypeEx summaryType = item.SummaryType;
						decimal argument = item.SummaryArgument;
						DevExpress.Data.PivotGrid.PivotSummaryType pivotST = summaryType.ToPivotSummaryType();
						StringBuilder withMemberBuilder = new StringBuilder();
						if(columns.Count > 0) {
							if(pivotST != Data.PivotGrid.PivotSummaryType.Custom) {
								withMemberBuilder.Append(string.Format(" {2} ( nonempty ( {0}, {1} ) , {1})", expandMemberName, measure.UniqueName, MDX.GetMDXSummaryExpression(pivotST)));
							} else {
								if(argument < 1)
									argument = argument * 100;
								withMemberBuilder.Append(IsTop(summaryType) ? " min( " : " max( ");
								withMemberBuilder.Append(MDX.WriteTopCount(
																			ToTopValueMode(summaryType),
																			(stb) => stb.Append(expandMemberName),
																			queryBuilder,
																			IsTop(summaryType),
																			Convert.ToInt32(argument),
																			new List<OLAPCubeColumn>() { measure },
																			measure.UniqueName));
								withMemberBuilder.Append(" , ");
								withMemberBuilder.Append(measure.UniqueName);
								withMemberBuilder.Append(" )");
							}
						} else {
							withMemberBuilder.Append(" ");
							withMemberBuilder.Append(measure.UniqueName);
							withMemberBuilder.Append(" ");
						}
						string memberName = string.Format("[XtraPivotGridCustomSummary{0}]", num);
						queryBuilder.AddWithMember(new QueryTempMember(true, memberName, withMemberBuilder.ToString()));
						if(num != 0)
							queryBuilder.OnColumns.Append(", ");
						queryBuilder.OnColumns.Append(memberName);
						num++;
					}
				}
			}
			queryBuilder.OnColumns.Append(" } ");
			bool createFilterResult = (subSelectFilters.Count > 0 ? CreateColumnRowFilters(queryBuilder, subSelectFilters, false) : true);
			if(!createFilterResult)
				return null;
			return queryBuilder.ToString();
		}
		bool IsTop(SummaryItemTypeEx summaryItemTypeEx) {
			switch(summaryItemTypeEx) {
				case SummaryItemTypeEx.Bottom:
				case SummaryItemTypeEx.BottomPercent:
					return false;
				case SummaryItemTypeEx.Top:
				case SummaryItemTypeEx.TopPercent:
					return true;
				default:
					throw new ArgumentException("SummaryItemTypeEx");
			}
		}
		PivotTopValueType ToTopValueMode(SummaryItemTypeEx summaryItemTypeEx) {
			switch(summaryItemTypeEx) {
				case SummaryItemTypeEx.Top:
				case SummaryItemTypeEx.Bottom:
					return PivotTopValueType.Absolute;
				case SummaryItemTypeEx.TopPercent:
				case SummaryItemTypeEx.BottomPercent:
					return PivotTopValueType.Percent;
				default:
					throw new ArgumentException("SummaryItemTypeEx");
			}
		}
	}
	public class OLAPDataSourceQuery2005 : OLAPDataSourceQueryBase {
		public OLAPDataSourceQuery2005(IOLAPDataSourceQueryOwner owner) : base(owner) { }
		protected override bool AllowInternalNonEmpty { get { return true; } }
		protected override SortByTempMember CreateSortByTempMember(OLAPCubeColumn column) {
			return new SortByTempMember2005(column);
		}
		public override string GetDrillDownQueryString(string cubeName, List<string> filteredValues,
									List<string> returnColumns, int maxRowCount) {
			string res = base.GetDrillDownQueryString(cubeName, filteredValues, returnColumns, maxRowCount);
			if(returnColumns != null && returnColumns.Count > 0) {
				StringBuilder result = new StringBuilder(res);
				result.AppendLine().Append("return ");
				for(int i = 0; i < returnColumns.Count; i++) {
					string column = returnColumns[i];
					if(column.StartsWith("[$"))
						result.Append("MemberValue(").Append(column).Append(")");
					else
						result.Append(column);
					if(i != returnColumns.Count - 1)
						result.Append(", ");
				}
				return result.ToString();
			}
			return res;
		}
		protected override string MemberValuePropertyName { get { return "\"MEMBER_VALUE\", TYPED"; } }
		protected override string MemberCaptionPropertyName { get { return "\"MEMBER_CAPTION\""; } }
		public override string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy) {
			if(string.IsNullOrEmpty(cubeName))
				return null;
			StringBuilder result = new StringBuilder();
			string memberMeasure = string.Format(TempMeasureNameFormat, 1);
			result.AppendLine("with")
				.Append("member ").Append(memberMeasure).Append(" as ").Append(hierarchy).AppendLine(".currentmember.member_value")
				.Append("member ").Append(CountMeasureStringTuple).Append(" as ").Append("count(filter({ ")
					.Append(levelUniqueName).Append(".members }, ").Append(memberMeasure).AppendLine(" = null))")
				.Append("select { ").Append(CountMeasureStringTuple).Append(" } on columns from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		protected override bool CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters, bool deferUpdates) {
			for(int i = 0; i < filters.Count; i++) {
				if(!CreateSubSelectFilterCore(ref result, filters[i], deferUpdates))
					return false;
			}
			return true;
		}
		protected override bool CreateWhereFilterCore(ref QueryBuilder result, OLAPCubeColumn filter, List<OLAPCubeColumn> parentFilters,
				bool forceWhere, bool deferUpdates) {
			if(forceWhere || FilterHelper.IsFilteredUsingWhereClause(filter, result.Measures))
				base.CreateWhereFilterCore(ref result, filter, parentFilters, forceWhere, deferUpdates);
			else {
				if(parentFilters != null) {
					foreach(OLAPCubeColumn parentFilter in parentFilters) {
						if(!CreateSubSelectFilterCore(ref result, parentFilter, deferUpdates))
							return false;
					}
				}
				if(!CreateSubSelectFilterCore(ref result, filter, deferUpdates))
					return false;
			}
			return true;
		}
		protected bool CreateSubSelectFilterCore(ref QueryBuilder result, OLAPCubeColumn filter) {
			return CreateSubSelectFilterCore(ref result, filter, false);
		}
		protected bool CreateSubSelectFilterCore(ref QueryBuilder result, OLAPCubeColumn filter, bool deferUpdates) {
			OLAPFilterValues filterValues = FilterHelper.GetFilterValues(null, filter, true, deferUpdates);
			if(filterValues == null || filterValues.IsEmpty)
				return true;
			result = result.GetInnerSubSelect();
			result.CreateSubSelect();
			StringBuilder onColumns = result.SubSelect.OnColumns;
			OLAPColumnMembersWriter writer = new OLAPColumnMembersWriter(FilterHelper);
			onColumns.Append("{");
			if(!writer.WriteFilteredColumnMembers(onColumns, new WriteFilteredMembersParams(result.SubSelect, filter, filterValues, false, false)))
				return false;
			onColumns.Append("}");
			result = result.SubSelect;
			return true;
		}
		protected override void WriteFirstLevelMembers(QueryBuilder queryBuilder, List<OLAPCubeColumn> columns, StringBuilder result) {
			new OLAPFirstLevelMembersWriter(FilterHelper, CreateSortByTempMember).
				WriteAllColumnMembers(queryBuilder, result, columns[0], !FilterHelper.HasGroupFilter(columns[0], false));
		}
		protected override void CreateWhereColumnRow(List<OLAPCubeColumn> columnRowCustomDefaultMemberColumns, QueryBuilder result) {
			for(int i = 0; i < columnRowCustomDefaultMemberColumns.Count; i++)
				if(columnRowCustomDefaultMemberColumns[i].AllMember != null)
					result.WhereMembers.Add(columnRowCustomDefaultMemberColumns[i].AllMember.UniqueName);
		}
	}
	public class OLAPDataSourceQuery2008 : OLAPDataSourceQuery2005 {
		public OLAPDataSourceQuery2008(IOLAPDataSourceQueryOwner owner) : base(owner) { }
		protected override OLAPExpandWriter CreateExpandWriter() {
			return new OLAPExpandWriter2008(FilterHelper, CreateSortByTempMember);
		}
	}
	public class OLAPDataSourceQuery2000 : OLAPDataSourceQueryBase {
		public OLAPDataSourceQuery2000(IOLAPDataSourceQueryOwner owner) : base(owner) { }
		protected override bool DimensionPropertiesMemberValueSupported { get { return false; } }
		protected override bool AllowInternalNonEmpty { get { return false; } }
		protected override string MemberValuePropertyName { get { return "\"CAPTION\""; } }
		protected override string MemberCaptionPropertyName { get { return "\"CAPTION\""; } }
		public override string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy) {
			if(string.IsNullOrEmpty(cubeName))
				return null;
			StringBuilder result = new StringBuilder();
			result.AppendLine("with")
				.Append("member ").Append(CountMeasureStringTuple).Append(" as '").Append("count(filter({ ")
					.Append(levelUniqueName).Append(".members }, ").Append(hierarchy).AppendLine(".currentmember.properties(\"CAPTION\") = \"\"))'")
				.Append("select { ").Append(CountMeasureStringTuple).Append(" } on columns from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		protected override SortByTempMember CreateSortByTempMember(OLAPCubeColumn column) {
			return new SortByTempMember2000(column);
		}
		protected override bool CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters, bool deferUpdates) {
			return CreateVisualTotals(result, filters, deferUpdates);
		}
		protected bool CreateVisualTotals(QueryBuilder result, List<OLAPCubeColumn> filters, bool deferUpdates) {
			int setId = 0;
			for(int i = 0; i < filters.Count; i++) {
				OLAPCubeColumn filter = filters[i],
					prevFilter = i != 0 ? filters[i - 1] : null;
				if(prevFilter != null && prevFilter.IsParent(filter))
					continue;
				bool includeAllMember = (prevFilter == null || !prevFilter.IsParent(filter)) && filter.AllMember != null;
				if(!FilterHelper.HasGroupFilter(filter, deferUpdates))
					CreateFieldVisualTotal(result, ref setId, filter, includeAllMember, deferUpdates);
				else
					CreateGroupVisualTotal(result, ref setId, filter, includeAllMember, deferUpdates);
			}
			return true;
		}
		void CreateGroupVisualTotal(QueryBuilder result, ref int setId, OLAPCubeColumn filter, bool includeAllMember, bool deferUpdates) {
			OLAPFilterValues filterValues = FilterHelper.GetCompleteGroupFilterValues_SQL2000(filter, FilterHelper.IsIncludedFilter_SQL2000(filter, deferUpdates), deferUpdates);
			if(filterValues == null)
				return;
			List<IQueryMetadataColumn> hierarchy = filter.Metadata.GetColumnHierarchy();
			OLAPLevelFilter[] membersByColumns = filterValues.GetMembersByLevels(hierarchy); 
			List<string> names = new List<string>(membersByColumns.Length + (includeAllMember ? 1 : 0));
			StringBuilder mdx = new StringBuilder();
			if(includeAllMember) {
				string name = MDX.UniqueNameToSetName(filter.AllMember.UniqueName);
				result.AddWithMember(new QueryTempMember(false, name, new StringBuilder().Append("{").Append(filter.AllMember.UniqueName).Append("}").ToString()));
				names.Add(name);
			}
			for(int columnIndex = 0; columnIndex < membersByColumns.Length; columnIndex++) {
				OLAPMetadataColumn column = (OLAPMetadataColumn)hierarchy[columnIndex];
				List<OLAPMember> members = membersByColumns[columnIndex] == null ? null : membersByColumns[columnIndex].GetProcessedMembers();
				OLAPFilterValues columnValues;
				if(members != null)
					columnValues = new OLAPFilterValues(filterValues.IsIncluded, members, filterValues.Column);
				else
					columnValues = new OLAPFilterValues(false, null, filterValues.Column);
				string name = MDX.UniqueNameToSetName(column.UniqueName);
				string superset = filterValues.IsIncluded || names.Count == 0 ? string.Empty : names[names.Count - 1];
				names.Add(name);
				result.AddWithMember(new QueryTempMember(false, name, MDX.FilterValuesToHierarchySet(superset, columnValues.GetLevelFilter())));
			}
			mdx.Append(MDX.GetSet<string>(names));
			result.AddWithMember(CreateVisualTotalsSet(mdx.ToString(), setId++));
		}
		void CreateFieldVisualTotal(QueryBuilder result, ref int setId, OLAPCubeColumn filter, bool includeAllMember, bool deferUpdates) {
			List<string> members = new List<string>();
			if(includeAllMember && filter.AllMember != null)
				members.Add(filter.AllMember.UniqueName);
			OLAPFilterValues filterValues = FilterHelper.GetIncludedFieldFilterValues(filter, deferUpdates);
			if(filterValues != null)
				members.AddRange(GetMembersUniqueNames(filterValues.GetProcessedMembers()));
			else
				members.Add(filter.UniqueName + ".members");
			result.AddWithMember(CreateVisualTotalsSet(members, setId++));
		}
		IEnumerable<string> GetMembersUniqueNames(ReadOnlyCollection<OLAPMember> members) {
			foreach(OLAPMember member in members)
				yield return member.UniqueName;
		}
		protected override bool CanAddNonAggregatable(OLAPMetadataColumn column, OLAPMetadataColumn nonaggregatable) {
			return base.CanAddNonAggregatable(column, nonaggregatable) && column.Dimension != nonaggregatable.Dimension;
		}
		protected override void WriteFirstLevelMembers(QueryBuilder queryBuilder, List<OLAPCubeColumn> columns, StringBuilder result) {
			new OLAPFirstLevelMembersWriter(FilterHelper, CreateSortByTempMember).
				WriteAllColumnMembers(queryBuilder, result, columns[0], true);
		}
		public override string GetAvailableValuesQuery(OLAPCubeColumn column, string cubeName, List<OLAPCubeColumn> subSelectFilters, List<OLAPCubeColumn> measures, List<OLAPMetadataColumn> nonaggregatables, bool deferUpdates) {
			if(string.IsNullOrEmpty(cubeName)) 
				return null;
			List<OLAPCubeColumn> filters = new List<OLAPCubeColumn>(subSelectFilters);
			filters.Remove(column);
			QueryBuilder result = new QueryBuilder(cubeName, AllowInternalNonEmpty);
			result.Measures.AddRange(measures);
			List<OLAPCubeColumn> dependent = new List<OLAPCubeColumn>(),
				independent = new List<OLAPCubeColumn>();
			foreach(OLAPCubeColumn item in filters) {
				if(item.Metadata.Dimension == column.Metadata.Dimension)
					dependent.Add(item);
				else
					independent.Add(item);
			}
			List<string> names = new List<string>();
			names.Add("DEPENDENT");
			result.AddWithMember(new QueryTempMember(false, names[0], GetDependent(column, dependent, deferUpdates)));
			int setId = 0;
			for(int i = 0; i < independent.Count; i++) {
				OLAPCubeColumn item = independent[i],
					prevItem = i == 0 ? null : independent[i - 1];
				if(prevItem != null && prevItem.Metadata.IsParent(item.Metadata))
					continue;
				if(!FilterHelper.HasGroupFilter(item, deferUpdates)) {
					string name = MDX.UniqueNameToSetName(item.UniqueName);
					result.AddWithMember(new QueryTempMember(false, name, GetIndependent(item, deferUpdates)));
					names.Add(name);
				} else {
					bool includeAllMember = (prevItem == null || !prevItem.Metadata.IsParent(item.Metadata)) && item.AllMember != null;
					CreateGroupVisualTotal(result, ref setId, item, includeAllMember, deferUpdates);
				}
			}
			result.OnRows.Append(MDX.Extract(MDX.NestedNonEmptyCrossJoin(names), column.Metadata.Dimension));
			if(!DimensionPropertiesMemberValueSupported)
				result.OnRows.Append(AppendDimensionProperties(Enumerable.Repeat(column, 1)));
			return result.ToString();
		}
		static string GetDependent(OLAPCubeColumn column, List<OLAPCubeColumn> dependent, bool deferUpdates) {
			if(dependent.Count == 0)
				return MDX.Members(column);
			else
				return MDX.Intersect(MDX.NestedIntersect(dependent, deferUpdates), MDX.Members(column));
		}
		static string GetIndependent(OLAPCubeColumn item, bool deferUpdates) {
			return MDX.GetSet(item.Owner.FilterHelper.GetFilterValues(null, item, true, deferUpdates).GetLevelFilter());
		}
	}
}
