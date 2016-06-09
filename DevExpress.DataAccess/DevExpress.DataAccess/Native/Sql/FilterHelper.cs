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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.DataAccess.Native.Sql {
	public static class FilterHelper {
		public static CriteriaOperator GetFilter(string filterString, IEnumerable<IParameter> parameters) {
			CriteriaOperator criteriaOperator = GetCriteriaOperator(filterString, parameters);
			OperandPropertyReplacingCriteriaOperatorPatcher patcher = new OperandPropertyReplacingCriteriaOperatorPatcher(parameters);
			return patcher.Process(criteriaOperator);
		}
		public static CriteriaOperator GetCriteriaOperator(string filterString, IEnumerable<IParameter> parameters) {
			OperandValue[] values;
			CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filterString, out values);
			if(values != null) {
				IList<IParameter> parametersList = parameters as IList<IParameter> ?? parameters.ToList();
				foreach(OperandValue operand in values) {
					string parameterName = string.Empty;
					OperandParameter operandParameter = operand as OperandParameter;
					if(!ReferenceEquals(operandParameter, null))
						parameterName = operandParameter.ParameterName;
					foreach(IParameter param in parametersList) {
						if(parameterName == param.Name)
							operand.Value = param.Value;
					}
				}
			}
			return criteriaOperator;
		}
	}
}
