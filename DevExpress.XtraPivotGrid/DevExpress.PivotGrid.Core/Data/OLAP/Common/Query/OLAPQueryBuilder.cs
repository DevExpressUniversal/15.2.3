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
namespace DevExpress.PivotGrid.OLAP {
	public class QueryTempMember {
		bool isMember;
		string name, mdx;
		public bool IsMember { get { return isMember; } }
		public string Name { get { return name; } }
		public string MDX { get { return mdx; } protected set { mdx = value; } }
		public QueryTempMember(bool isMember, string name, string mdx) {
			this.isMember = isMember;
			this.name = name;
			this.mdx = mdx;
		}
		public override string ToString() {
			return Name;
		}
	}
	public class QueryTopTempSet : QueryTempMember {
		public QueryTopTempSet(string name, string mdx)
			: base(false, name, mdx) {
		}
		public static string GetColumnTopName(OLAPCubeColumn column) {
			return "XtraPivotGrid " + column.GetHashCode().ToString() + " Top";
		}
	}
	public class QueryBuilder {
		static string EscapeApostroph(string str) {
			StringBuilder escapeString = new StringBuilder(str);
			for(int i = 0; i < escapeString.Length; i++) {
				char symb = '\'';
				if(escapeString[i] == symb) {
					i++;
					escapeString.Insert(i,
#if SL
						Convert.ToString
#endif
 (symb));
				}
			}
			return escapeString.ToString();
		}
		Dictionary<string, QueryTempMember> withMembers;
		List<string> whereMembers;
		StringBuilder onColumns, onRows, cellProperties;
		string cubeName;
		QueryBuilder subSelect;
		bool isSubSelect;
		bool nonEmptyBehaviour;
		Dictionary<OLAPCubeColumn, SortByTempMember> sortByMembers;
		List<OLAPCubeColumn> measures;
		bool allowInternalNonEmpty;
		string onColumnsDimensionProperties, onRowsDimensionProperties;
		public QueryBuilder(string cubeName, bool allowInternalNonEmpty)
			: this(cubeName, false, allowInternalNonEmpty) {
		}
		public QueryBuilder(string cubeName, bool isSubSelect, bool allowInternalNonEmpty) {
			this.withMembers = new Dictionary<string, QueryTempMember>();
			this.onColumns = new StringBuilder();
			this.onRows = new StringBuilder();
			this.cellProperties = new StringBuilder();
			this.measures = new List<OLAPCubeColumn>();
			this.whereMembers = new List<string>();
			this.cubeName = cubeName;
			this.subSelect = null;
			this.isSubSelect = isSubSelect;
			this.nonEmptyBehaviour = !IsSubSelect;
			this.allowInternalNonEmpty = allowInternalNonEmpty;
			this.onColumnsDimensionProperties = string.Empty;
			this.onRowsDimensionProperties = string.Empty;
		}
		protected internal Dictionary<string, QueryTempMember> WithMembers { get { return withMembers; } }
		public Dictionary<OLAPCubeColumn, SortByTempMember> SortByMembers {
			get {
				if(sortByMembers == null)
					sortByMembers = new Dictionary<OLAPCubeColumn, SortByTempMember>();
				return sortByMembers;
			}
		}
		public bool AllowInternalNonEmpty { get { return allowInternalNonEmpty && GetAllowNonEmptyMeasures(); } }
		public StringBuilder OnColumns { get { return onColumns; } }
		public StringBuilder OnRows { get { return onRows; } }
		public StringBuilder CellProperties { get { return cellProperties; } }
		public string OnColumnsDimensionProperties { get { return onColumnsDimensionProperties; } set { onColumnsDimensionProperties = value; } }
		public string OnRowsDimensionProperties { get { return onRowsDimensionProperties; } set { onRowsDimensionProperties = value; } }
		public List<OLAPCubeColumn> Measures { get { return measures; } }
		public string CubeName { get { return cubeName; } set { cubeName = value; } }
		public List<string> WhereMembers { get { return whereMembers; } }
		public QueryBuilder SubSelect { get { return subSelect; } set { subSelect = value; } }
		public bool IsSubSelect { get { return isSubSelect; } }
		public bool NonEmptyBehaviour { get { return nonEmptyBehaviour; } set { nonEmptyBehaviour = value; } }
		protected string NonEmptyString {
			get { return NonEmptyBehaviour ? "non empty " : ""; }
		}
		bool GetAllowNonEmptyMeasures() {
			for(int i = 0; i < measures.Count; i++)
				if(!measures[i].OLAPUseNonEmpty)
					return false;
			return true;
		}
		public void AddWithMember(QueryTempMember member) {
			if(WithMembers.ContainsKey(member.Name))
				return;
			WithMembers.Add(member.Name, member);
			SortByTempMember sortBy = member as SortByTempMember;
			if(sortBy != null)
				SortByMembers.Add(sortBy.Column, sortBy);
		}
		public bool ContainsCalculatedMember(string memberName) {
			return WithMembers.ContainsKey(memberName);
		}
		public string GetWithMemberMdx(string memberName) {
			if(!WithMembers.ContainsKey(memberName))
				return string.Empty;
			return WithMembers[memberName].MDX;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			if(!IsSubSelect) {
				Dictionary<string, QueryTempMember> withMembers = GetAllWithMembers();
				if(withMembers.Count > 0) {
					result.AppendLine("with");
					foreach(QueryTempMember member in withMembers.Values) {
						if(member.IsMember)
							result.Append("member ");
						else
							result.Append("set ");
						result.Append(member.Name).Append(" as '").Append(EscapeApostroph(member.MDX)).AppendLine("' ");
					}
				}
			}
			result.AppendLine("select");
			WriteOnColumns(result);
			if(OnRows.Length > 0) {
				result.AppendLine(",").Append(NonEmptyString)
					.AppendLine(OnRows.ToString()).Append(OnRowsDimensionProperties).Append(" on rows");
			}
			result.AppendLine().Append("from ");
			if(SubSelect == null)
				result.Append("[").Append(CubeName).AppendLine("]");
			else
				result.AppendLine("(").Append(SubSelect.ToString()).Append(")");
			if(!IsSubSelect) {
				List<string> whereMembers = GetAllWhereMembers();
				if(whereMembers.Count > 0) {
					result.AppendLine("where").Append("( ");
					foreach(string member in whereMembers) {
						result.Append(member).Append(", ");
					}
					result.Length -= 2;
					result.Append(" )");
				}
			}
			if(cellProperties.Length > 0) {
				result.Append(" CELL PROPERTIES ");
				result.Append(cellProperties);
			}
			return result.ToString();
		}
		List<string> GetAllWhereMembers() {
			List<string> whereMembers = new List<string>();
			QueryBuilder builder = this;
			while(builder != null) {
				whereMembers.AddRange(builder.WhereMembers);
				builder = builder.SubSelect;
			}
			return whereMembers;
		}
		Dictionary<string, QueryTempMember> GetAllWithMembers() {
			Dictionary<string, QueryTempMember> withMembers = new Dictionary<string, QueryTempMember>();
			QueryBuilder builder = this;
			while(builder != null) {
				foreach(QueryTempMember member in builder.WithMembers.Values) {
					if(!withMembers.ContainsKey(member.Name))
						withMembers.Add(member.Name, member);
				}
				builder = builder.SubSelect;
			}
			return withMembers;
		}
		public void CreateNonAggregatables(List<OLAPMetadataColumn> nonAggregatables) {
			for(int i = 0; i < nonAggregatables.Count; i++) {
				OLAPMetadataColumn column = nonAggregatables[i];
				string name = column.Hierarchy.UniqueName + ".[XtraPivotGrid All]";
				QueryTempMember aggregate = new QueryTempMember(true, name,
					"aggregate({" + column.UniqueName + ".members})");
				AddWithMember(aggregate);
				WhereMembers.Add(name);
			}
		}
		protected void WriteOnColumns(StringBuilder result) {
			result.Append(NonEmptyString);
			if(OnColumns.Length > 0 && Measures.Count > 0) {
				result.Append("{ ").Append(OnColumns.ToString())
					.Append(" * { ");
				WriteMeasures(result);
				result.Append("}} ");
			} else {
				if(OnColumns.Length > 0)
					result.Append(OnColumns.ToString());
				else {
					result.Append("{ ");
					WriteMeasures(result);
					result.Append("} ");
				}
			}
			result.Append(OnColumnsDimensionProperties).Append(" on columns");
		}
		public void WriteMeasures(StringBuilder result) {
			if(Measures.Count == 0) {
				result.Append("[Measures].defaultmember");
				return;
			}
			result.Append(Measures[0].UniqueName);
			for(int i = 1; i < Measures.Count; i++)
				result.Append(", ").Append(Measures[i].UniqueName);
		}
		public QueryBuilder CreateSubSelect() {
			if(SubSelect != null)
				throw new Exception("The MDX query uses an incorrect subselect clause");
			SubSelect = new QueryBuilder(CubeName, true, AllowInternalNonEmpty);
			return SubSelect;
		}
		public QueryBuilder GetInnerSubSelect() {
			QueryBuilder res = this;
			while(res.SubSelect != null)
				res = res.SubSelect;
			return res;
		}
		public QueryBuilder CreateInnerSubSelect() {
			QueryBuilder innerSubSelect = GetInnerSubSelect();
			return innerSubSelect.CreateSubSelect();
		}
		internal void AddOneValueOnColumns() {
			QueryTempMember member = new QueryTempMember(true, "[Measures].[XtraPivotGridOne]", "(1)");
			AddWithMember(member);
			OnColumns.Append("{").Append(member.Name).Append("}");
		}
	}
}
