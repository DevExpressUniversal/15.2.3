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
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class PersistentParameterVisitor : ClientCriteriaVisitorBase {
		IEnumerable<DevExpress.Data.IParameter> parameters;
		private PersistentParameterVisitor(IEnumerable<DevExpress.Data.IParameter> parameters) {
			this.parameters = parameters;
		}
		public override CriteriaOperator Visit(BinaryOperator theOperator) {
			SetFullPropertyName(theOperator.LeftOperand as OperandProperty, theOperator.RightOperand as OperandParameter);
			return base.Visit(theOperator);
		}
		public override CriteriaOperator Visit(InOperator theOperator) {
			if(theOperator.Operands.Count == 1) {
				SetFullPropertyName(theOperator.LeftOperand as OperandProperty, theOperator.Operands[0] as OperandParameter);
			}
			return base.Visit(theOperator);
		}
		private void SetFullPropertyName(OperandProperty operandProperty, OperandParameter operandParameter) {
			if(!ReferenceEquals(operandProperty, null) && !ReferenceEquals(operandParameter, null)) {
				Parameter param = parameters.GetByName(operandParameter.ParameterName) as Parameter;
				if(param != null && param.LookUpSettings is DynamicListLookUpSettings) {
					operandProperty.PropertyName += "." + ((DynamicListLookUpSettings)param.LookUpSettings).ValueMember;
				}
			}
		}
		public static string Process(CriteriaOperator criteriaOperator, IEnumerable<DevExpress.Data.IParameter> parameters) {
			if(!ReferenceEquals(criteriaOperator, null)) {
				PersistentParameterVisitor visitor = new PersistentParameterVisitor(parameters);
				criteriaOperator.Accept(visitor);
				return criteriaOperator.ToString();
			}
			return string.Empty;
		}
	}
}
