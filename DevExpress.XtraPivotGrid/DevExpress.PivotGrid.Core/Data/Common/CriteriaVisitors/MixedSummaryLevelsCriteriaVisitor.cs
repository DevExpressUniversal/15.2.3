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
using DevExpress.Data.PivotGrid;
namespace DevExpress.PivotGrid.CriteriaVisitors {
	public interface IUniqueFieldNameGenerator {
		string Generate(string prefix);
	}
	public class MixedSummaryLevelCriteriaVisitor : CriteriaPatcherBase {
		public class SummaryCriteria {
			readonly PivotSummaryType summaryType;
			readonly IPivotCustomSummaryAggregate aggregate;
			readonly OperandProperty operand;
			public string SummaryLevelName {
				get { return Operand.PropertyName; }
			}
			public PivotSummaryType SummaryType { get { return summaryType; } }
			public IPivotCustomSummaryAggregate CustomAggregate { get { return aggregate; } }
			public OperandProperty Operand { get { return operand; } }
			public Type GetDataType(Type sourceType) {
					if(summaryType == PivotSummaryType.Custom)
						return aggregate.ResultType(new Type[] { sourceType });
					else
						return PivotSummaryValue.GetValueType(summaryType, sourceType);
			}
			public SummaryCriteria(PivotSummaryType summaryType, IPivotCustomSummaryAggregate aggregate, string name) {
				this.summaryType = summaryType;
				this.aggregate = aggregate;
				this.operand = new OperandProperty(name);
			}
		}
		public class DataSourceCriteria {
			readonly CriteriaOperator criteria;
			string dataSourceLevelName;
			readonly List<SummaryCriteria> summaryCriterias = new List<SummaryCriteria>();
			public CriteriaOperator Criteria { get { return criteria; } }
			public List<SummaryCriteria> SummaryCriterias { get { return summaryCriterias; } }
			public string DataSourceLevelName {
				get { return dataSourceLevelName; }
				set { dataSourceLevelName = value; }
			}
			public DataSourceCriteria(CriteriaOperator criteria, string dataSourceLevelName) {
				this.criteria = criteria;
				this.dataSourceLevelName = dataSourceLevelName;
			}
			public CriteriaOperator GetOrCreate(PivotSummaryType st, IPivotCustomSummaryAggregate aggregate, Func<string> generateName) {
				foreach(SummaryCriteria sc in summaryCriterias)
					if(sc.SummaryType == st && sc.CustomAggregate == aggregate)
						return sc.Operand;
				SummaryCriteria summaryCriteria = new SummaryCriteria(st, aggregate, generateName());
				summaryCriterias.Add(summaryCriteria);
				return summaryCriteria.Operand;
			}
		}
		public const string SummaryNamePrefix = "Temp_SumamryLevelColumn";
		public const string DataSourceNamePrefix = "Temp_DataSourceLevelColumn";
		IUniqueFieldNameGenerator nameGenerator;
		readonly Dictionary<CriteriaOperator, DataSourceCriteria> summaryLevel = new Dictionary<CriteriaOperator, DataSourceCriteria>();
		public Dictionary<CriteriaOperator, DataSourceCriteria> SummaryLevel {
			get {
				return summaryLevel;
			}
		}
		public MixedSummaryLevelCriteriaVisitor(IUniqueFieldNameGenerator generator) {
			this.nameGenerator = generator;
		}
		protected void SetUniqueFieldNameGenerator(IUniqueFieldNameGenerator nameGenerator) {
			this.nameGenerator = nameGenerator;
		}
		public override CriteriaOperator Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic) {
				IPivotCustomSummaryAggregate customAggregate = CriteriaOperator.GetCustomFunction(((OperandValue)theOperator.Operands[0]).Value as string) as IPivotCustomSummaryAggregate;
				if(customAggregate != null) {
					return CreatePivotAggregate(false, PivotSummaryType.Custom, customAggregate, theOperator.Operands[1], null);
				}
			}
			return base.Visit(theOperator);
		}
		public override CriteriaOperator Visit(AggregateOperand theOperand) {
			if(!ReferenceEquals(theOperand.CollectionProperty, null) && !string.IsNullOrEmpty(theOperand.CollectionProperty.PropertyName))
				return theOperand;
			bool notParametrizedOperand = theOperand.AggregateType == Aggregate.Count || theOperand.AggregateType == Aggregate.Exists;
			if(object.Equals(null, theOperand.AggregatedExpression) && !notParametrizedOperand)
				return new OperandValue(PivotSummaryValue.ErrorValue);
			PivotSummaryType st = PivotSummaryType.Sum;
			switch(theOperand.AggregateType) {
				case Aggregate.Avg:
					st = PivotSummaryType.Average;
					break;
				case Aggregate.Count:
					st = PivotSummaryType.Count;
					break;
				case Aggregate.Max:
					st = PivotSummaryType.Max;
					break;
				case Aggregate.Min:
					st = PivotSummaryType.Min;
					break;
				case Aggregate.Sum:
					st = PivotSummaryType.Sum;
					break;
				case Aggregate.Single:
					return new FunctionOperator(FunctionOperatorType.Iif,
						  new BinaryOperator(
							  Visit(new AggregateOperand(theOperand.CollectionProperty, theOperand.AggregatedExpression, Aggregate.Count, theOperand.Condition)),
							  new OperandValue(1),
							  BinaryOperatorType.Equal),
							 Visit(new AggregateOperand(theOperand.CollectionProperty, theOperand.AggregatedExpression, Aggregate.Max, theOperand.Condition)),
							 new OperandValue(PivotSummaryValue.ErrorValue));
				case Aggregate.Exists:
					return
							  new BinaryOperator(
							  Visit(new AggregateOperand(theOperand.CollectionProperty, theOperand.AggregatedExpression, Aggregate.Count, theOperand.Condition)),
							  new OperandValue(0),
							  BinaryOperatorType.Greater);
				default:
					throw new ArgumentException("Aggregate");
			}
			return CreatePivotAggregate(notParametrizedOperand, st, null, theOperand.AggregatedExpression, theOperand.Condition);
		}
		CriteriaOperator CreatePivotAggregate(bool notParametrizedOperand, PivotSummaryType st, IPivotCustomSummaryAggregate aggregate, CriteriaOperator expression, CriteriaOperator filter) {
			DataSourceCriteria datasourceCriteria;
			if(notParametrizedOperand)
				if(st == PivotSummaryType.Count) {
					expression = BaseCriteriaChecker.GetCheckingForNull(filter);
					if(ReferenceEquals(null, expression))
						expression = new OperandValue(1);
					else
						filter = null;
				} else
					expression = new OperandValue(1);
			if(!ReferenceEquals(null, filter))
				expression = new FunctionOperator(FunctionOperatorType.Iif, filter, expression, new OperandValue(null));
			if(!summaryLevel.TryGetValue(expression, out datasourceCriteria)) {
				datasourceCriteria = new DataSourceCriteria(expression, nameGenerator.Generate(DataSourceNamePrefix));
				summaryLevel[expression] = datasourceCriteria;
			}
			return datasourceCriteria.GetOrCreate(st, aggregate, () => nameGenerator.Generate(SummaryNamePrefix));
		}
	}
}
