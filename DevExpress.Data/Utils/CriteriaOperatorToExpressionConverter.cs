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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
namespace DevExpress.Data.Utils {
	public enum ExpressionKind { Simple, EF, LinqToObjects }
	public static class CriteriaOperatorToExpressionConverter {
		public static Expression<Func<T, bool>> GetWhere<T>(CriteriaOperator criteria, ICriteriaToExpressionConverter converter) {			
			if(ReferenceEquals(criteria, null))
				return x => true;
			return GetLambda<T, bool>(criteria, converter);			
		}
		public static Expression<Func<T, bool>> GetGenericWhere<T>(CriteriaOperator criteria) {
			return GetWhere<T>(criteria, GetGenericConverter());
		}
		public static Expression<Func<T, bool>> GetLinqToObjectsWhere<T>(CriteriaOperator criteria) {
			return GetWhere<T>(criteria, GetLinqToObjectsConverter());
		}
		public static Expression<Func<T, bool>> GetEntityFrameworkWhere<T>(CriteriaOperator criteria) {
			return GetWhere<T>(criteria, GetEntityFrameworkConverter());
		}
		public static Expression<Func<T, TResult>> GetLambda<T, TResult>(CriteriaOperator criteria, ICriteriaToExpressionConverter converter) {
			if(converter == null)
				throw new ArgumentNullException("converter");
			ParameterExpression parameter = Expression.Parameter(typeof(T));
			Expression body = converter.Convert(parameter, criteria);
			if(body == null)
				return null;
			return Expression.Lambda<Func<T, TResult>>(body, parameter);
		}
		public static Expression<Func<T, TResult>> GetGenericLambda<T, TResult>(CriteriaOperator criteria) {
			return GetLambda<T, TResult>(criteria, GetGenericConverter());
		}
		public static Expression<Func<T, TResult>> GetLinqToObjectsLambda<T, TResult>(CriteriaOperator criteria) {
			return GetLambda<T, TResult>(criteria, GetLinqToObjectsConverter());
		}
		public static Expression<Func<T, TResult>> GetEntityFrameworkLambda<T, TResult>(CriteriaOperator criteria) {
			return GetLambda<T, TResult>(criteria, GetEntityFrameworkConverter());
		}
		static ICriteriaToExpressionConverter GetLinqToObjectsConverter() {
			return new CriteriaToExpressionConverterForObjects();
		}
		static ICriteriaToExpressionConverter GetGenericConverter() {
			return new CriteriaToExpressionConverter();
		}
		static ICriteriaToExpressionConverter GetEntityFrameworkConverter() {
			return new CriteriaToEFExpressionConverter();
		}
	}
}
