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
using System.Text;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public static class MDX {
		public static string Descendants(string set, OLAPCubeColumn level, string flag) {
			if(level.ParentColumn == null)
				throw new ArgumentException("Invalid level");
			StringBuilder stb = new StringBuilder();
			stb.Append("descendants(");
			WriteSetWithBrackets(stb, set);
			stb.Append(", ").Append(level.UniqueName);
			if(!string.IsNullOrEmpty(flag))
				stb.Append(", ").Append(flag);
			stb.Append(")");
			return stb.ToString();
		}
		public static string Descendants(string set, OLAPCubeColumn level) {
			return Descendants(set, level, null);
		}
		public static string Descendants(string set, int level) {
			StringBuilder stb = new StringBuilder();
			stb.Append("descendants(").Append(set).Append(", ").Append(level).Append(")");
			return stb.ToString();
		}
		public static string Descendants(OLAPCubeColumn parent, OLAPCubeColumn level) {
			if(!parent.IsParent(level))
				throw new ArgumentException("Invalid parent and level values");
			return Descendants(Members(parent), level);
		}
		public static string Descendants(OLAPMember member, OLAPCubeColumn level) {
			if(!member.Column.IsParent(level))
				throw new ArgumentException("Invalid parent and level values");
			return Descendants(member.UniqueName, level);
		}
		public static string Descendants(string set, int level, string flag) {
			StringBuilder stb = new StringBuilder();
			stb.Append("descendants(").Append(set).Append(".Levels(0)").Append(", ").Append(level).Append(", ").Append(flag).Append(")");
			return stb.ToString();
		}
		public static string Members(OLAPCubeColumn column) {
			return column.UniqueName + ".members";
		}
		public static string Members(OLAPCubeColumn column, bool addCalculatedMembers) {
			return column.UniqueName + (addCalculatedMembers ? ".allmembers" : ".members");
		}
		public static string Intersect(string set1, string set2) {
			StringBuilder stb = new StringBuilder();
			stb.Append("intersect(");
			WriteSetWithBrackets(stb, set1);
			stb.Append(", ");
			WriteSetWithBrackets(stb, set2);
			stb.Append(")");
			return stb.ToString();
		}
		public static string NestedIntersect(List<OLAPCubeColumn> columns, bool deferUpdates) {
			StringBuilder stb = new StringBuilder();
			for(int i = 0; i < columns.Count - 1; i++)
				stb.Append("intersect(");
			WriteDescendants(stb, columns[0], deferUpdates);
			for(int i = 1; i < columns.Count; i++) {
				stb.Append(", ");
				WriteDescendants(stb, columns[i], deferUpdates);
				stb.Append(")");
			}
			return stb.ToString();
		}
		public static string NestedNonEmptyCrossJoin(List<string> sets) {
			StringBuilder stb = new StringBuilder();
			for(int i = 0; i < sets.Count - 1; i++)
				stb.Append("NonEmptyCrossJoin(");
			stb.Append(sets[0]);
			for(int i = 1; i < sets.Count; i++) {
				stb.Append(", ");
				stb.Append(sets[i]);
				stb.Append(")");
			}
			return stb.ToString();
		}
		static void WriteDescendants(StringBuilder stb, OLAPCubeColumn column, bool deferUpdates) {
			stb.Append("descendants(");
			stb.Append(GetSet(column.Owner.FilterHelper.GetFilterValues(null, column, true, deferUpdates).GetLevelFilter()));
			stb.Append(")");
		}
		public static string GetSet(OLAPLevelFilter members) {
			return members.GetMDX(true);
		}
		public static string GetSet<T>(IList<T> members) {
			return GetSet(members, true);
		}
		public static string GetSet<T>(IList<T> members, bool writeBrackets) {
			StringBuilder stb = new StringBuilder();
			if(writeBrackets)
				stb.Append("{");
			int count = members.Count;
			for(int i = 0; i < count - 1; i++) {
				stb.Append(members[i]);
				stb.Append(", ");
			}
			if(members.Count > 0)
				stb.Append(members[count - 1]);
			if(writeBrackets)
				stb.Append("}");
			return stb.ToString();
		}
		static void WriteSetWithBrackets(StringBuilder stb, string set) {
			bool hasBrackets = set.StartsWith("{");
			if(!hasBrackets)
				stb.Append("{");
			stb.Append(set);
			if(!hasBrackets)
				stb.Append("}");
		}
		public static string Extract(string set, string dimension) {
			return new StringBuilder().Append("Extract(").Append(set).Append(", ").Append(dimension).Append(")").ToString();
		}
		public static string UniqueNameToSetName(string uniqueName) {
			StringBuilder stb = new StringBuilder(uniqueName);
			foreach(char ch in new char[] { '[', ']', '.', ' ', '-', '+', '*' }) {
				stb.Replace(ch, '_');
			}
			return stb.ToString().ToUpper();
		}
		public static string FilterValuesToHierarchySet(string superset, OLAPLevelFilter members) {
			StringBuilder stb = new StringBuilder();
			if(!string.IsNullOrEmpty(superset)) {
				stb.Append(Descendants(superset, 1));
				if(members.GetCount() != 0)
					stb.Append(" - ").Append(GetSet(members));
			} else
				stb.Append(GetSet(members));
			return stb.ToString();
		}
		public static string GetMDXSummaryExpression(DevExpress.Data.PivotGrid.PivotSummaryType pivotST) {
			switch(pivotST) {
				case Data.PivotGrid.PivotSummaryType.Average:
					return "Avg";
				case Data.PivotGrid.PivotSummaryType.Count:
					return "Count";
				case Data.PivotGrid.PivotSummaryType.Max:
					return "Max";
				case Data.PivotGrid.PivotSummaryType.Min:
					return "Min";
				case Data.PivotGrid.PivotSummaryType.StdDev:
					return "StdDev";
				case Data.PivotGrid.PivotSummaryType.StdDevp:
					return "StdDevp";
				case Data.PivotGrid.PivotSummaryType.Sum:
					return "Sum";
				case Data.PivotGrid.PivotSummaryType.Var:
					return "Var";
				case Data.PivotGrid.PivotSummaryType.Varp:
					return "Varp";
				default:
					throw new ArgumentException("PivotSummaryType");
			}
		}
		public static string WriteTopCount(PivotTopValueType topValueType, Action<StringBuilder> writeContent, QueryBuilder queryBuilder, bool topMembers, int topCount, List<OLAPCubeColumn> measures, string topMeasure) {
			StringBuilder topResult = new StringBuilder();
			switch(topValueType) {
				case PivotTopValueType.Absolute:
					topResult.Append(topMembers ? "topcount(" : "bottomcount(");
					break;
				case PivotTopValueType.Percent:
					topResult.Append(topMembers ? "toppercent(" : "bottompercent(");
					break;
				case PivotTopValueType.Sum:
					topResult.Append(topMembers ? "topsum(" : "bottomsum(");
					break;
				default:
					throw new InvalidOperationException();
			}
			WriteNonEmpty(writeContent, topResult, measures, queryBuilder.AllowInternalNonEmpty);
			topResult.Append(", ").Append(topCount);
			if(!string.IsNullOrEmpty(topMeasure))
				topResult.Append(", ").Append(topMeasure);
			topResult.Append(")");
			return topResult.ToString();
		}
		public static void WriteNonEmpty(Action<StringBuilder> writeContent, StringBuilder result, List<OLAPCubeColumn> measures, bool allowInternalNonEmpty) {
			if(measures.Count == 0 || !measures[0].OLAPUseNonEmpty) {
				writeContent(result);
				return;
			}
			if(allowInternalNonEmpty)
				WriteNonEmptyCore(writeContent, result, measures);
			else
				WriteFilterCore(writeContent, result, measures);
		}
		public static void WriteFilterCore(Action<StringBuilder> writeContent, StringBuilder result, List<OLAPCubeColumn> measures) {
			result.Append("filter({");
			writeContent(result);
			result.Append("}, (");
			for(int i = 0; i < measures.Count; i++) {
				result.Append("not isEmpty(").Append(measures[i].UniqueName).Append(") or ");
			}
			result.Length -= 3;
			result.Append("))");
		}
		public static void WriteNonEmptyCore(Action<StringBuilder> writeContent, StringBuilder result, List<OLAPCubeColumn> measures) {
			result.Append("nonempty({");
			writeContent(result);
			result.Append("},{");
			for(int i = 0; i < measures.Count; i++) {
				result.Append(measures[i].UniqueName).Append(", ");
			}
			result.Length -= 2;
			result.Append("})");
		}
		public static List<OLAPCubeColumn> GetAllFilteredColumns(bool deferUpdates, List<OLAPCubeColumn> customFilters, IOLAPFilterHelper filterHelper) {
			if(!deferUpdates) {
				List<OLAPCubeColumn> whereFilters = filterHelper.GetFilteredColumns(PivotArea.FilterArea),
					columnRowFilters = filterHelper.GetFilteredColumns(PivotArea.RowArea, PivotArea.ColumnArea);
				columnRowFilters.AddRange(whereFilters);
				return columnRowFilters;
			} else {
				if(customFilters == null)
					throw new ArgumentException();
				return customFilters;
			}
		}
	}
}
