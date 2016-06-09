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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections;
	using System.Linq;
	using System.Linq.Expressions;
	static class DynamicCastHelper {
		internal static Func<IEnumerable, object> GetEnumerableCast(ref Func<IEnumerable, object> enumerableCastConverter, Func<Type> getType) {
			if(enumerableCastConverter == null) {
				var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(getType());
				var source = Expression.Parameter(typeof(IEnumerable), "source");
				enumerableCastConverter = Expression.Lambda<Func<IEnumerable, object>>(
					Expression.Call(castMethod, source), source).Compile();
			}
			return enumerableCastConverter;
		}
		internal static Func<TSource, object> GetAsyncFunction<TSource, TSourceResult>(ref Func<TSource, object> asyncFunction, string asyncMethod, Type resultType) {
			if(asyncFunction == null) {
				Type sourceTaskType = typeof(System.Threading.Tasks.Task<TSourceResult>);
				Type resultTaskType = typeof(System.Threading.Tasks.Task<>).MakeGenericType(resultType);
				Type continueWithArgumentType = typeof(Func<,>).MakeGenericType(sourceTaskType, resultType);
				var genericContinueWithMethod = sourceTaskType.GetMethods()
					.Where(m => IsGenericContinueWith(m))
					.FirstOrDefault()
					.MakeGenericMethod(resultType);
				var t = Expression.Parameter(sourceTaskType, "t");
				var getResult = Expression.MakeMemberAccess(t,
					sourceTaskType.GetProperty("Result"));
				var underlyingType = Enum.GetUnderlyingType(typeof(TSourceResult));
				var convert = Expression.Lambda(
					Expression.Convert(Expression.Convert(getResult, underlyingType), resultType), t);
				var src = Expression.Parameter(typeof(TSource), "src");
				var asyncCall = Expression.Call(src, typeof(TSource).GetMethod(asyncMethod));
				asyncFunction = Expression.Lambda<Func<TSource, object>>(
					Expression.Call(asyncCall, genericContinueWithMethod, convert), src).Compile();
			}
			return asyncFunction;
		}
		static bool IsGenericContinueWith(System.Reflection.MethodInfo mInfo) {
			if(!mInfo.IsGenericMethod)
				return false;
			if(mInfo.Name != "ContinueWith")
				return false;
			var parameters = mInfo.GetParameters();
			if(parameters.Length == 1) {
				var pType = parameters[0].ParameterType;
				var pTypeGeneric = pType.GetGenericTypeDefinition();
				if(typeof(Func<,>) == pTypeGeneric) {
					var arguments = pType.GetGenericArguments();
					if(arguments[0].IsGenericType && arguments[0].GetGenericTypeDefinition() == typeof(System.Threading.Tasks.Task<>))
						return true;
				}
			}
			return false;
		}
	}
}
