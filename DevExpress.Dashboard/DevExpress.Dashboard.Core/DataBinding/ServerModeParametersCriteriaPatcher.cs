#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using System.Collections;
namespace DevExpress.DashboardCommon.Native {
	class ParametersToValuesCriteriaPatcher : CriteriaPatcherBase {
		readonly NullableDictionary<string, object> expValues = new NullableDictionary<string, object>();
		readonly NullableDictionary<string, object> values = new NullableDictionary<string, object>();
		readonly bool replace;
		public ParametersToValuesCriteriaPatcher(IEnumerable<IParameter> parameters) : this(parameters, true) {
		}
		public ParametersToValuesCriteriaPatcher(IEnumerable<IParameter> parameters, bool replace) {
			this.replace = replace;
			foreach(IParameter param in parameters) {
				string propName = string.Format(CalculatedFieldsController.ParameterFormatString, param.Name);
				if(!expValues.ContainsKey(propName))
					expValues.Add(propName, param.Value);
				if(!values.ContainsKey(param.Name))
					values.Add(param.Name, param.Value);
			}
		}
		public override CriteriaOperator Visit(OperandValue theOperand) {
			OperandParameter param = theOperand as OperandParameter;
			object pValue;
			if(!object.ReferenceEquals(param, null) && values.TryGetValue(param.ParameterName, out pValue)) {
				if(replace)
					return new OperandValue(pValue);
				else
					return new OperandParameter(param.ParameterName, pValue);
			}
			return base.Visit(theOperand);
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			object pValue;
			if(expValues.TryGetValue(theOperand.PropertyName, out pValue))
				return new OperandValue(pValue);
			return base.Visit(theOperand);
		}
		public override CriteriaOperator Visit(InOperator theOperator) {
			CriteriaOperator op = base.Visit(theOperator);
			if(op is InOperator) {
				theOperator = (InOperator)op;
				SubstituteMultivalueParameters(theOperator.Operands);
				return theOperator;
			}
			return op;
		}
		  void SubstituteMultivalueParameters(CriteriaOperatorCollection operands) {
				List<CriteriaOperator> newOperands = new List<CriteriaOperator>(operands.Count);
				bool hasMultiValueParameters = false;
				foreach(CriteriaOperator operand in operands) {
					OperandParameter operandParameter = operand as OperandParameter;
					OperandValue operandValue = operand as OperandValue;
					if(!ReferenceEquals(operandParameter, null)) {
						object parameterValue = null;
						if(values.TryGetValue(operandParameter.ParameterName, out parameterValue))
							if(SubstituteMultivalueParametersFromEnumerable(newOperands, parameterValue))
								hasMultiValueParameters = true;
							else
								newOperands.Add(new OperandParameter(operandParameter.ParameterName, parameterValue));
					}
					else if(!ReferenceEquals(operandValue, null) && SubstituteMultivalueParametersFromEnumerable(newOperands, operandValue.Value))
							hasMultiValueParameters = true;						
					else {
						newOperands.Add(operand);
					}
				}
				if(hasMultiValueParameters) {
					operands.Clear();
					operands.AddRange(newOperands);
				}
			}
		  static bool SubstituteMultivalueParametersFromEnumerable(List<CriteriaOperator> newOperands, object parameterValue) {
			  IEnumerable enumerable = parameterValue as IEnumerable;			  
			  if(parameterValue != null && parameterValue.GetType() != typeof(string) && enumerable != null) {				  
				  foreach(object value in enumerable)
					  newOperands.Add(new OperandValue() { Value = value });
				  return true;
			  }
			  return false;
		  }
	}
	class CalculatedFieldsExpressionExpander : CriteriaPatcherBase {
		public static CriteriaOperator WrapToType(CriteriaOperator criteria, CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Boolean:
					return new FunctionOperator(FunctionOperatorType.Iif, criteria, new ConstantValue(true), new ConstantValue(false));
				case CalculatedFieldType.DateTime:
					return new FunctionOperator(FunctionOperatorType.AddDays, criteria, 0);
				case CalculatedFieldType.Decimal:
					return new FunctionOperator(FunctionOperatorType.ToDecimal, criteria);
				case CalculatedFieldType.Integer:
					return new FunctionOperator(FunctionOperatorType.ToInt, criteria);
				case CalculatedFieldType.String:
					return new FunctionOperator(FunctionOperatorType.ToStr, criteria);
				case CalculatedFieldType.Object:
				default:
					return criteria;
			}
		}
		readonly IEnumerable<CalculatedField> fields;
		readonly bool throwOnError;
		Stack<string> fieldsOnStack = new Stack<string>();
		public CalculatedFieldsExpressionExpander(IEnumerable<CalculatedField> fields, string calculatedFieldName, bool throwOnError) {
			this.fields = fields;
			this.throwOnError = throwOnError;
			this.fieldsOnStack.Push(calculatedFieldName);
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			foreach(CalculatedField field in fields)
				if(field.Name == theOperand.PropertyName) {
					if(fieldsOnStack.Contains(field.Name))
						if(throwOnError)
							throw new ArgumentException(string.Format("Recursive: ", string.Join(",", fieldsOnStack)));
						else
							return null;
					fieldsOnStack.Push(field.Name);
					CriteriaOperator val = null;
					CriteriaOperator op = string.IsNullOrEmpty(field.Expression) ? null : CriteriaOperator.Parse(field.Expression);
					if(!object.ReferenceEquals(null, op))
						val = op.Accept(this);
					fieldsOnStack.Pop();
					if(!ReferenceEquals(null, val))
						val = WrapToType(val, field.DataType);
					return val;
				}
			return base.Visit(theOperand);
		}
	}
}
