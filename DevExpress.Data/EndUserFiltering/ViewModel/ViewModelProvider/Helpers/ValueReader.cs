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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	static class MemberReader {
		internal static object Read(object viewModel, string memberName, IDictionary<string, object> valuesHash) {
			object value;
			if(!valuesHash.TryGetValue(memberName, out value)) {
				value = Read(viewModel, memberName);
				valuesHash.Add(memberName, value);
			}
			return value;
		}
		static IDictionary<string, Func<object, object>> accessors = new Dictionary<string, Func<object, object>>();
		internal static Func<object, object> defaultAccessor = (s) => null;
		internal static object Read(object viewModel, string memberName) {
			var viewModelType = viewModel.GetType();
			string path = viewModelType.FullName + "." + memberName;
			Func<object, object> accessor;
			if(!accessors.TryGetValue(path, out accessor)) {
				var memberInfo = viewModelType.GetMember(memberName).FirstOrDefault();
				if(memberInfo == null) {
					accessor = (vm) => vm.@GetParentViewModel()
							.@Get(pvm => Read(pvm, memberName));
				}
				else {
					accessor = GetAccessor(viewModelType, memberInfo)
							.@Get(e => e.Compile(), defaultAccessor);
				}
				accessors.Add(path, accessor);
			}
			return accessor(viewModel);
		}
		internal static void ResetAccessors(object viewModel) {
			if(viewModel != null) {
				string viewModelTypeName = viewModel.GetType().FullName;
				var pairsToRemove = accessors
					.Where(p => p.Key.StartsWith(viewModelTypeName)).ToArray();
				foreach(KeyValuePair<string, Func<object, object>> pair in pairsToRemove)
					accessors.Remove(pair);
			}
			else accessors.Clear();
		}
		#region Expresssions
		internal static Expression<Func<object, object>> GetAccessor(Type viewModelType, MemberInfo m) {
			if(m is PropertyInfo || m is FieldInfo)
				return Accessor(viewModelType, m);
			if(m is MethodInfo)
				return Call(viewModelType, (MethodInfo)m);
			return null;
		}
		static Expression<Func<object, object>> Call(Type sourceType, MethodInfo sourceMethod) {
			var s = Expression.Parameter(typeof(object), "s");
			var call = Expression.Call(Expression.Convert(s, sourceType), sourceMethod);
			return Expression.Lambda<Func<object, object>>(CheckReturnType(call), s);
		}
		static Expression<Func<object, object>> Accessor(Type sourceType, MemberInfo sourceProperty) {
			var s = Expression.Parameter(typeof(object), "s");
			var accessor = Expression.MakeMemberAccess(Expression.Convert(s, sourceType), sourceProperty);
			return Expression.Lambda<Func<object, object>>(CheckMemberType(accessor), s);
		}
		static Expression CheckReturnType(MethodCallExpression call) {
			return call.Method.ReturnType.IsValueType ?
				(Expression)Expression.Convert(call, typeof(object)) : call;
		}
		static Expression CheckMemberType(MemberExpression accessor) {
			return accessor.Type.IsValueType ?
				(Expression)Expression.Convert(accessor, typeof(object)) : accessor;
		}
		#endregion Expresssions
	}
}
