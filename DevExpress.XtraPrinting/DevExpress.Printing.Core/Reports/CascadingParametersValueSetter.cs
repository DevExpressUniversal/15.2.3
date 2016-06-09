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
using DevExpress.XtraReports.Native.Parameters;
using System.Linq;
using DevExpress.Data;
namespace DevExpress.XtraReports.Parameters.Native {
	sealed class CascadingParametersValueSetter : ParametersValueSetter {
		readonly IParameterEditorValueProvider valueProvider;
		public CascadingParametersValueSetter(IEnumerable<Parameter> parameters, IParameterEditorValueProvider valueProvider) : base(parameters) {
			this.valueProvider = valueProvider;
		}
		public static void Process(CriteriaOperator criteriaOperator, IEnumerable<Parameter> parameters, IParameterEditorValueProvider parameterValueProvider) {
			new CascadingParametersValueSetter(parameters, parameterValueProvider).Process(criteriaOperator);
		}
		public override CriteriaOperator Visit(OperandValue theOperand) {
			var operandParameter = theOperand as OperandParameter;
			if(ReferenceEquals(operandParameter, null) || valueProvider == null)
				return base.Visit(theOperand);
			var parameter = this.Parameters.FirstOrDefault(x => x.Name == operandParameter.ParameterName);
			if(parameter == null)
				return base.Visit(operandParameter);
			try {
				operandParameter.Value = valueProvider.GetValue(parameter);
			} catch {
				return base.Visit(operandParameter);
			}
			return operandParameter;
		}
	}
}
