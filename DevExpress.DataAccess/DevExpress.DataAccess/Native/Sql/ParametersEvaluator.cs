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
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DataAccess.Native.Sql {
	public static class ParametersEvaluator {
		public static IEnumerable<IParameter> EvaluateParameters(IEnumerable<IParameter> sourceParameters, IEnumerable<DataSourceParameterBase> parametersToEvaluate) {
			List<IParameter> result = new List<IParameter>();
			XREvaluatorContextDescriptor context = new XREvaluatorContextDescriptor(sourceParameters, new DataContext(), null, string.Empty);
			foreach(DataSourceParameterBase parameter in parametersToEvaluate) {
				if(parameter.Type != typeof(Expression) || parameter.Value == null) {
					result.Add(new QueryParameter(parameter.Name, parameter.Type, parameter.Value));
					continue;
				}
				Expression expression = parameter.Value as Expression;
				if(expression == null)
					throw new InvalidOperationException("If the Type property is set to Expression, the Value property must return an Expression instance.");
				object value = expression.EvaluateExpression(context);
				result.Add(new QueryParameter(parameter.Name, value != null ? value.GetType() : typeof(object), value));
			}
			return result;
		}
		public static object EvaluateExpression(this Expression expression, IEnumerable<IParameter> sourceParameters) {
			XREvaluatorContextDescriptor context = new XREvaluatorContextDescriptor(sourceParameters, new DataContext(), null, string.Empty);
			return expression.EvaluateExpression(context);
		}
		public static object EvaluateExpression(this Expression expression, XREvaluatorContextDescriptor context) {
			CriteriaOperator criteriaOperator;
			try {
				criteriaOperator = CriteriaOperator.Parse(expression.ExpressionString, null);
			}
			catch(CriteriaParserException) {
				criteriaOperator = CriteriaOperator.Parse(string.Empty, null);
			}
			ExpressionEvaluator evaluator = new ExpressionEvaluator(context, criteriaOperator, true);
			object value = evaluator.Evaluate(null);
			if(expression.ResultType != null && !expression.ResultType.IsInstanceOfType(value)) {
				try {
					return  Convert.ChangeType(value, expression.ResultType); 
				}
				catch(InvalidCastException e) {
					System.Diagnostics.Debug.Fail(e.Message, e.ToString());
				}
			}
			return value;
		}
	}  
}
