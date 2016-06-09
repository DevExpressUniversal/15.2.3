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
using DevExpress.Xpo.DB;
namespace DevExpress.PivotGrid.ServerMode {
	class PropertyToQueryOperandPatcher : CriteriaPatcherBase, IQueryCriteriaVisitor<CriteriaOperator> {
		internal static CriteriaOperator Patch(CriteriaOperator criteria, IDataSourceHelpersOwner<ServerModeColumn> currentOwner) {
			return new PropertyToQueryOperandPatcher(currentOwner).Process(criteria);
		}
		IDataSourceHelpersOwner<ServerModeColumn> currentOwner;
		protected PropertyToQueryOperandPatcher(IDataSourceHelpersOwner<ServerModeColumn> currentOwner) {
			this.currentOwner = currentOwner;
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			ServerModeColumn column = null;
			if(currentOwner.CubeColumns.TryGetValue(theOperand.PropertyName, out column))
				return ((IGroupCriteriaConvertible)column).GetGroupCriteria();
			MetadataColumnBase metadataColumn = null;
			if(!currentOwner.Metadata.Columns.TryGetValue(theOperand.PropertyName, out metadataColumn)) {
				IServerModeHelpersOwner serverModeHelpersOwner = (IServerModeHelpersOwner)currentOwner;
				foreach(KeyValuePair<ServerModeColumn, DevExpress.XtraPivotGrid.PivotGridFieldBase> pair in serverModeHelpersOwner.Areas.FieldsByColumns)
					if(
						pair.Value.UnboundType != Data.UnboundColumnType.Bound && (
							pair.Value.Name == theOperand.PropertyName ||
							string.Equals(pair.Value.FieldName, theOperand.PropertyName, serverModeHelpersOwner.CaseSensitiveDataBinding ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ||
							pair.Value.UnboundFieldName == theOperand.PropertyName
						)
					)
						return ((IGroupCriteriaConvertible)pair.Key).GetGroupCriteria();
				throw new Exception(string.Format("Property {0} not found", theOperand.PropertyName));
			}
			return ((IGroupCriteriaConvertible)metadataColumn).GetGroupCriteria();
		}
		public override CriteriaOperator Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					return Process(DevExpress.Data.Filtering.Helpers.EvalHelpers.ExpandIsOutlookInterval(theOperator));
			}
			return base.Visit(theOperator);
		}
		CriteriaOperator IQueryCriteriaVisitor<CriteriaOperator>.Visit(Xpo.DB.QuerySubQueryContainer theOperand) {
			return new Xpo.DB.QuerySubQueryContainer(theOperand.Node, Process(theOperand.AggregateProperty), theOperand.AggregateType);
		}
		CriteriaOperator IQueryCriteriaVisitor<CriteriaOperator>.Visit(Xpo.DB.QueryOperand theOperand) {
			return theOperand;
		}
		public override CriteriaOperator Visit(AggregateOperand theOperand) {
			CriteriaOperator expression = Process(theOperand.AggregatedExpression);
			if(!ReferenceEquals(null, theOperand.Condition)) {
				switch(theOperand.AggregateType) {
					case Aggregate.Count: {
							CriteriaOperator nullCheking = BaseCriteriaChecker.GetCheckingForNull(theOperand.Condition);
							if(!ReferenceEquals(nullCheking, null))
								return new QuerySubQueryContainer(null, Process(nullCheking), theOperand.AggregateType);
							return new QuerySubQueryContainer(null,
												new FunctionOperator(FunctionOperatorType.Iif, Process(theOperand.Condition), new OperandValue(1), new OperandValue(0)),
													Aggregate.Sum);
						}
					case Aggregate.Sum:
						return new QuerySubQueryContainer(null,
											new FunctionOperator(FunctionOperatorType.Iif, Process(theOperand.Condition), expression, new OperandValue(0)),
												Aggregate.Sum);
					case Aggregate.Avg:
						return new BinaryOperator(
							Process(new AggregateOperand(theOperand.CollectionProperty, theOperand.AggregatedExpression, Aggregate.Sum, theOperand.Condition)),
							Process(new AggregateOperand(theOperand.CollectionProperty, theOperand.AggregatedExpression, Aggregate.Count, theOperand.Condition)),
							BinaryOperatorType.Divide);
					case Aggregate.Exists:
						return new FunctionOperator(FunctionOperatorType.Iif, new BinaryOperator(new QuerySubQueryContainer(
																  null,
																  new FunctionOperator(FunctionOperatorType.Iif, Process(theOperand.Condition), new OperandValue(1), new OperandValue(0)),
																  Aggregate.Sum),
										 new OperandValue(0), BinaryOperatorType.Greater), new OperandValue(true), new OperandValue(false));
					case Aggregate.Max:
					case Aggregate.Min:
					case Aggregate.Single:
					default:
						break;
				}
			}
			return new QuerySubQueryContainer(null, expression, theOperand.AggregateType);
		}
	}
}
