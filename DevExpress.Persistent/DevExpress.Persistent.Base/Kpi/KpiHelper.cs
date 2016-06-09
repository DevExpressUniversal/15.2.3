#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.Persistent.Base.Kpi {
	public static class KpiHelper {
		public const string RangeStartParameterName = "RangeStart";
		public const string RangeEndParameterName = "RangeEnd";
		[Obsolete("Use 'GetCriteriaOperator' instead.")]
		public static CriteriaWrapper GetCriteriaWrapper(IObjectSpace objectSpace, string criteriaString, Type targetObjectType, DateTime rangeStart, DateTime rangeEnd) {
			if(targetObjectType == null) {
				return null;
			}
			CriteriaWrapper criteriaWrapper;
			string patchedCriteriaString = criteriaString;
			if(!string.IsNullOrEmpty(patchedCriteriaString)) {
				patchedCriteriaString = patchedCriteriaString.
					Replace("'@RangeStart'", "?" + RangeStartParameterName).
					Replace("RangeStart()", "?" + RangeStartParameterName).
					Replace("'@RangeEnd'", "?" + RangeEndParameterName).
					Replace("RangeEnd()", "?" + RangeEndParameterName);
				criteriaWrapper = CriteriaEditorHelper.GetCriteriaWrapper(patchedCriteriaString, targetObjectType, objectSpace);
				if(criteriaWrapper.Parameters != null) {
					foreach(IParameter parameter in criteriaWrapper.Parameters) {
						if(parameter.Name == RangeStartParameterName) {
							parameter.CurrentValue = rangeStart;
						}
						else if(parameter.Name == RangeEndParameterName) {
							parameter.CurrentValue = rangeEnd;
						}
					}
				}
			}
			else {
				criteriaWrapper = CriteriaEditorHelper.GetCriteriaWrapper("", targetObjectType, objectSpace);
			}
			return criteriaWrapper;
		}
		public static CriteriaOperator GetCriteriaOperator(IObjectSpace objectSpace, string criteriaString, Type targetObjectType, DateTime rangeStart, DateTime rangeEnd) {
#pragma warning disable 0618
			return GetCriteriaWrapper(objectSpace, criteriaString, targetObjectType, rangeStart, rangeEnd).CriteriaOperator;
#pragma warning restore 0618
		}
		public static CriteriaOperator GetExpressionOperator(string expression, DateTime rangeStart, DateTime rangeEnd) {
			string pathed_expression = expression.
				Replace("'@RangeStart'", "?" + RangeStartParameterName).
				Replace("'@RangeEnd'", "?" + RangeEndParameterName);
			OperandValue[] operandValues;
			CriteriaOperator result = CriteriaOperator.Parse(pathed_expression, out operandValues);
			foreach(OperandValue operandValue in operandValues) {
				OperandParameter operandParameter = operandValue as OperandParameter;
				if(!CriteriaOperator.Equals(operandParameter, null) && operandParameter.ParameterName == RangeStartParameterName) {
					operandParameter.Value = rangeStart;
				}
				if(!CriteriaOperator.Equals(operandParameter, null) && operandParameter.ParameterName == RangeEndParameterName) {
					operandParameter.Value = rangeEnd;
				}
			}
			return result;
		}
	}
}
