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
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Native {
	public class FunctionNameValidator : DevExpress.Data.Filtering.Helpers.ClientCriteriaVisitorBase {
		public static void Validate(CriteriaOperator input) {
			new FunctionNameValidator().Process(input);
		}
		protected override CriteriaOperator Visit(FunctionOperator fn) {
			if(fn.OperatorType == FunctionOperatorType.Custom) {
				string customFnName = null;
				if(fn.Operands.Count >= 1) {
					OperandValue fnNameOv = fn.Operands[0] as OperandValue;
					if(!ReferenceEquals(fnNameOv, null)) {
						customFnName = fnNameOv.Value as string;
					}
				}
				if(string.IsNullOrEmpty(customFnName))
					throw new InvalidOperationException(ReportLocalizer.GetString(ReportStringId.Msg_InvalidExpressionEx));
				ICustomFunctionOperator customFunction = CriteriaOperator.GetCustomFunction(customFnName);
				if(customFunction == null)
					throw new InvalidOperationException(string.Format(ReportLocalizer.GetString(ReportStringId.Msg_NoCustomFunction), customFnName));
			}
			return base.Visit(fn);
		}
	}
}
