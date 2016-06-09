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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Linq.Expressions;
	static class ExpressionHelper {
		internal static string GetPropertyName<T>(Expression<Func<T>> expression) {
			return GetPropertyName((LambdaExpression)expression);
		}
		internal static string GetPropertyName(LambdaExpression expression) {
			MemberExpression memberExpression = GetMemberExpression(expression);
			if(IsPropertyExpression(memberExpression.Expression as MemberExpression))
				throw new ArgumentException("Expression: " + expression.ToString());
			return memberExpression.Member.Name;
		}
		static bool IsPropertyExpression(MemberExpression expression) {
			return (expression != null) && (expression.Member.MemberType == System.Reflection.MemberTypes.Property);
		}
		static MemberExpression GetMemberExpression(LambdaExpression expression) {
			if(expression == null)
				throw new ArgumentNullException("expression");
			Expression body = expression.Body;
			if(body is UnaryExpression)
				body = ((UnaryExpression)body).Operand;
			MemberExpression memberExpression = body as MemberExpression;
			if(memberExpression == null)
				throw new ArgumentException("Expression: " + expression.ToString());
			return memberExpression;
		}
	}
}
