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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public class OperandPropertyReplacingCriteriaOperatorPatcher : ClientCriteriaVisitorBase {
		readonly Dictionary<string, object> values = new Dictionary<string, object>();
		public OperandPropertyReplacingCriteriaOperatorPatcher(IEnumerable<IParameter> parameters) {
			foreach (IParameter param in parameters) {
				if (!values.ContainsKey(param.Name))
					values.Add(param.Name, param.Value);
			}
		}
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			string propertyName = theOperand.PropertyName;
			int index = propertyName.LastIndexOf('.');
			if(index == -1)
				return new QueryOperand(propertyName, null);
			string aliasName = propertyName.Substring(0, index);
			return new QueryOperand(propertyName.Substring(index + 1), aliasName);
		}
		protected override CriteriaOperator Visit(FunctionOperator theOperator) {
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
					return Process(EvalHelpers.ExpandIsOutlookInterval(theOperator));
			}
			return base.Visit(theOperator);
		}
		protected override CriteriaOperator Visit(InOperator theOperator) {
			CriteriaOperator op = base.Visit(theOperator);
			if (op is InOperator) {
				theOperator = (InOperator)op;
				SubstituteMultivalueParameters(theOperator.Operands);
				return theOperator;
			}
			return op;
		}
		void SubstituteMultivalueParameters(CriteriaOperatorCollection operands) {
			List<CriteriaOperator> newOperands = new List<CriteriaOperator>(operands.Count);
			foreach (CriteriaOperator operand in operands) {
				OperandParameter operandParameter = operand as OperandParameter;
				if (!ReferenceEquals(operandParameter, null)) {
					object parameterValue;
					if (values.TryGetValue(operandParameter.ParameterName, out parameterValue)) {
						IEnumerable enumerable = parameterValue as IEnumerable;
						if (parameterValue != null && !(parameterValue is string) && enumerable != null) {
							newOperands.AddRange(enumerable.Cast<object>().Select(value => new OperandValue {Value = value}));
						}
						else
							newOperands.Add(new OperandParameter(operandParameter.ParameterName, parameterValue));
					}
				}
				else {
					newOperands.Add(operand);
				}
			}
			operands.Clear();
			operands.AddRange(newOperands);
		}
		public new CriteriaOperator Process(CriteriaOperator criteriaOperator) {
			return base.Process(criteriaOperator);
		}
	}
}
