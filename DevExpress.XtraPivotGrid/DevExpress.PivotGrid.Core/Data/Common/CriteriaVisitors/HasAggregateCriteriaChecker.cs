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

using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.ServerMode.Queryable;
namespace DevExpress.PivotGrid.CriteriaVisitors {
	public class HasAggregateCriteriaChecker : BaseCriteriaChecker, IQueryCriteriaVisitor<bool> {
		public static bool Check(CriteriaOperator criteria) {
			return Check<HasAggregateCriteriaChecker>(criteria);
		}
		public override bool Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Custom) {
				ICustomFunctionOperator op = CriteriaOperator.GetCustomFunction(((OperandValue)theOperator.Operands[0]).Value as string);
				IQueryableConvertible conv = op as IQueryableConvertible;
				if(conv != null && conv.IsAggregate)
					return true;
				IPivotCustomSummaryAggregate pc = op as IPivotCustomSummaryAggregate;
				if(pc != null)
					return true;
			}
			return base.Visit(theOperator);
		}
		public override bool Visit(AggregateOperand theOperand) {
			if(ReferenceEquals(theOperand.CollectionProperty, null) || string.IsNullOrEmpty(theOperand.CollectionProperty.PropertyName))
				return true;
			return base.Visit(theOperand);
		}
		public static bool Check(IEnumerable<CriteriaOperator> criterias) {
			foreach(CriteriaOperator criteria in criterias)
				if(Check(criteria))
					return true;
			return false;
		}
		bool IQueryCriteriaVisitor<bool>.Visit(Xpo.DB.QuerySubQueryContainer theOperand) {
			return true;
		}
		bool IQueryCriteriaVisitor<bool>.Visit(Xpo.DB.QueryOperand theOperand) {
			return false;
		}   
	}
}
