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
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using DevExpress.Data.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.Security;
using DevExpress.ExpressApp.Security.Strategy;
namespace DevExpress.ExpressApp.Security {
	public class IsCurrentUserInRoleOperator : ICustomFunctionOperatorConvertibleToExpression {
		public const string OperatorName = "IsCurrentUserInRole";
		private static readonly IsCurrentUserInRoleOperator instance = new IsCurrentUserInRoleOperator();
		public static void Register() {
			CustomFunctionOperatorHelper.Register(instance);
		}
		public object Evaluate(params object[] operands) {
			if(operands == null || operands.Length != 1) {
				throw new ArgumentException();
			}
			if(SecuritySystem.CurrentUser == null) {
				return false;
			}
			if(SecuritySystem.CurrentUser is IUserWithRoles) {
				IUserWithRoles user = (IUserWithRoles)SecuritySystem.CurrentUser;
				string roleName = Convert.ToString(operands[0]);
				return user.IsUserInRole(roleName);
			}
			if(SecuritySystem.CurrentUser is ISecurityUserWithRoles) {
				ISecurityUserWithRoles user = (ISecurityUserWithRoles)SecuritySystem.CurrentUser;
				string roleName = Convert.ToString(operands[0]);
				return user.IsUserInRole(roleName);
			}
			return false;
		}
		public string Name {
			get { return OperatorName; }
		}
		public Type ResultType(params Type[] operands) {
			return typeof(bool);
		}
		Expression ICustomFunctionOperatorConvertibleToExpression.Convert(ICriteriaToExpressionConverter converter, params Expression[] operands) {
			if((operands != null) && (operands.Length == 1) && (operands[0] is ConstantExpression)) {
				Object operand = ((ConstantExpression)operands[0]).Value;
				return Expression.Constant(Evaluate(operand));
			}
			else {
				throw new ArgumentException();
			}
		}
	}
}
