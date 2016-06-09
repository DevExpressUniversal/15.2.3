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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Xpo.DB;
using DevExpress.PivotGrid.DataCalculation;
using System.Threading;
namespace DevExpress.PivotGrid.ServerMode {
	public abstract class ServerModeQueryCriteriaBase : IEnumerable<CriteriaOperator> {
		protected ServerModeQueryCriteriaBase() {
			Grouping = new List<CriteriaOperator>();
		}
		protected ServerModeQueryCriteriaBase(IEnumerable<CriteriaOperator> grouping, CriteriaOperator filter) {
			Grouping = new List<CriteriaOperator>(grouping);
			Filter = filter;
		}
		public CriteriaOperator Filter { get; set; }
		public List<CriteriaOperator> Grouping { get; private set; }
		IEnumerator<CriteriaOperator> IEnumerable<CriteriaOperator>.GetEnumerator() {
			return GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		protected abstract IEnumerator<CriteriaOperator> GetEnumerator();
	}
	public class ServerModeQueryCriteria : ServerModeQueryCriteriaBase {
		public ServerModeQueryCriteria() : this(-1) { }
		public ServerModeQueryCriteria(int aggregatesStart) {
			this.AggregatesStart = aggregatesStart;
			SkipSelectedRecords = 0;
			TopSelectedRecords = 0;
			TopValuePercent = false;
			Sort = new List<SortingColumn>();
			Operands = new List<CriteriaOperator>();
		}
		public List<SortingColumn> Sort { get; private set; }
		public List<CriteriaOperator> Operands { get; private set; }
		public int AggregatesStart { get; set; }
		public int SkipSelectedRecords { get; set; }
		public int TopSelectedRecords { get; set; }
		public bool TopValuePercent { get; set; }
		protected override IEnumerator<CriteriaOperator> GetEnumerator() {
			foreach(CriteriaOperator op in Operands)
				yield return op;
			if(!ReferenceEquals(null, Filter))
				yield return Filter;
		}
	}
	public class NestedSummaryServerModeQueryCriteria : ServerModeQueryCriteriaBase {
		public NestedSummaryServerModeQueryCriteria(IEnumerable<CriteriaOperator> grouping, CriteriaOperator filter, CriteriaOperator operand, IEnumerable<AggregationItem> summaries) : base(grouping, filter) {
			Operand = operand;
			SummaryTypes = new List<AggregationItem>(summaries);
		}
		public CriteriaOperator Operand { get; set; }
		public IList<AggregationItem> SummaryTypes { get; private set; }
		protected override IEnumerator<CriteriaOperator> GetEnumerator() {
			foreach(CriteriaOperator op in Grouping)
				yield return op;
			yield return Operand;
			if(!ReferenceEquals(null, Filter))
				yield return Filter;
		}
	}
	public class ServerModeColumnModel : ServerModeColumnModelBase {
		public string Alias { get; set; }
		public string TableName { get; set; }
		public string TableAlias { get; set; }
		public string DisplayFolder { get; set; }
		public string Caption { get; set; }
	}
	public class ServerModeColumnModelBase {
		public string Name { get; set; }
		public Type DataType { get; set; }
	}
	interface IServerModeHelpersOwner : IDataSourceHelpersOwner<ServerModeColumn> {
		Areas Areas { get; }
		IPivotQueryExecutor Executor { get; }
		List<object> QueryValues(QueryColumn column, Dictionary<QueryColumn, object> values);
		bool CaseSensitiveDataBinding { get; set; }
		new FilterHelper FilterHelper { get; }
	}
	[Flags]
	public enum CriteriaSyntax {
		ServerCriteria = 1,
		ClientCriteria = 2,
		SupportsNestedSummaries = 4,
		ForceInt16SummaryAsInt32 = 8,
		SupportsTopNPercentage = 16,
	}
	public interface IPivotQueryResult {
		IEnumerable<object[]> Data { get; }
		Type[] AggregatesSchema { get; }
	}
	class PivotQueryResult : IPivotQueryResult {
		public static PivotQueryResult Empty = new PivotQueryResult(null, null);
		readonly IEnumerable<object[]> data;
		readonly Type[] aggregatesSchema;
		public PivotQueryResult(IEnumerable<object[]> data, Type[] aggregatesSchema) {
			this.data = data;
			this.aggregatesSchema = aggregatesSchema;
		}
		IEnumerable<object[]> IPivotQueryResult.Data { get { return data; } }
		Type[] IPivotQueryResult.AggregatesSchema { get { return aggregatesSchema; } }
	}
	public interface IPivotQueryExecutor {
		IPivotQueryResult GetQueryResult(ServerModeQueryCriteria query, CancellationToken cancellationToken);
		List<object> GetQueryResult(NestedSummaryServerModeQueryCriteria query);
		Type ValidateCriteria(CriteriaOperator criteria, bool? exactType);
		IEnumerable<ServerModeColumnModel> GetColumns();
		bool Connected { get; }
		CriteriaSyntax CriteriaSyntax { get; }
	}
	interface IServerQueryContext : IQueryContext<ServerModeColumn> {
		bool IsFullExpand { get; }
	}
	interface IRawCriteriaConvertible {
		CriteriaOperator GetRawCriteria();
	}
	interface IGroupCriteriaConvertible : IRawCriteriaConvertible {
		CriteriaOperator GetGroupCriteria();
	}
	interface ISelectCriteriaConvertible : IGroupCriteriaConvertible {
		CriteriaOperator GetSelectCriteria();
	}
	interface IDrillDownProvider {
		string DrillDownName { get; }
		string Name { get; }
		Type DataType { get; }
	}
	interface IServerModeQueryExecutor : IQueryExecutor<ServerModeColumn> {
		List<object> QueryValues(IServerModeHelpersOwner owner, QueryColumn column, Dictionary<QueryColumn, object> values);
		bool ValidateExpression(IServerModeHelpersOwner owner, CriteriaOperator criteria, UnboundColumnType columnType);
		Type GetUnboundExpressionType(IServerModeHelpersOwner owner, CriteriaOperator criteria, bool makeQuery);
	}
}
