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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Native.Parameters {
	public class ParametersValueSetter : ClientCriteriaVisitorBase {
		public static void Process(CriteriaOperator criteriaOperator, IEnumerable<IParameter> parameters) {
			(new ParametersValueSetter(parameters)).Process(criteriaOperator);
		}
		IEnumerable<IParameter> parameters;
		protected IEnumerable<IParameter> Parameters { get { return parameters; } }
		public ParametersValueSetter(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		public override CriteriaOperator Visit(OperandValue theOperand) {
			OperandParameter operandParameter = theOperand as OperandParameter;
			if(!ReferenceEquals(operandParameter, null)) {
				IParameter param = parameters.GetByName(operandParameter.ParameterName);
				if(param != null)
					operandParameter.Value = param.Value;
			}
			return theOperand;
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
				if(IsMultivalueParameter(operand)) {
					OperandParameter operandParameter = operand as OperandParameter;
					if(operandParameter.Value != null && operandParameter.Value is IEnumerable) {
						hasMultiValueParameters = true;
						foreach(object value in (IEnumerable)operandParameter.Value) {
							newOperands.Add(new OperandValue() { Value = value });
						}
					}
				} else {
					newOperands.Add(operand);
				}
			}
			if(hasMultiValueParameters) {
				operands.Clear();
				operands.AddRange(newOperands);
			}
		}
		bool IsMultivalueParameter(CriteriaOperator operand) {
			OperandParameter operandParameter = operand as OperandParameter;
			if(ReferenceEquals(operandParameter, null))
				return false;
			IMultiValueParameter param = parameters.GetByName(operandParameter.ParameterName) as IMultiValueParameter;
			return param != null && param.MultiValue;
		}
	}
}
