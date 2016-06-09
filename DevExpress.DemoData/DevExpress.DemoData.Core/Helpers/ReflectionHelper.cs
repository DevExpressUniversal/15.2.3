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
using System.Reflection;
using System.Linq.Expressions;
namespace DevExpress.DemoData.Helpers {
	public static class ReflectionHelper {
		public static PropertyInfo GetPublicProperty(Type type, string propertyName) {
			return GetPublicProperty(type, propertyName, null);
		}
		public static PropertyInfo GetPublicProperty(Type type, string propertyName, Type propertyType) {
			return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public, null, propertyType, new Type[] { }, null);
		}
		public static MethodInfo GetPublicMethod(Type type, string methodName) {
			return GetPublicMethod(type, methodName, null);
		}
		public static MethodInfo GetPublicMethod(Type type, string methodName, Type[] argTypes) {
			if(argTypes == null)
				return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);
		}
		public static MethodInfo GetPublicStaticMethod(Type type, string methodName) {
			return GetPublicStaticMethod(type, methodName, null);
		}
		public static MethodInfo GetPublicStaticMethod(Type type, string methodName, Type[] argTypes) {
			if(argTypes == null)
				return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
			return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, argTypes, null);
		}
		public static Action<object, object, TEventArgs> ConvertEventHandlerToLambda<TEventArgs>(MethodInfo method) {
			ParameterInfo eParameter = method.GetParameters()[1];
			ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
			ParameterExpression sender = Expression.Parameter(typeof(object), "sender");
			ParameterExpression e = Expression.Parameter(typeof(EventArgs), "e");
			MethodCallExpression methodCall = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, sender, Expression.Convert(e, eParameter.ParameterType));
			return CompileLambda<Action<object, object, TEventArgs>>(methodCall, instance, sender, e);
		}
		public static Action<object, object> ConvertPropertySetterToLambda(PropertyInfo property) {
			ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
			ParameterExpression val = Expression.Parameter(typeof(object), "val");
			Expression setExpr = AssignmentExpression.Create(Expression.Property(Expression.Convert(instance, property.DeclaringType), property), Expression.Convert(val, property.PropertyType));
			return CompileLambda<Action<object, object>>(setExpr, instance, val);
		}
		public static Func<object, object> ConvertPropertyGetterToLambda(PropertyInfo property) {
			ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
			Expression getExpr = Expression.Convert(Expression.Property(Expression.Convert(instance, property.DeclaringType), property), typeof(object));
			return CompileLambda<Func<object, object>>(getExpr, instance);
		}
		public static Type GetDependencyPropertyType(Type ownerType, System.Windows.DependencyProperty property) {
			return property.PropertyType;
		}
		public static T CompileLambda<T>(Expression body, params ParameterExpression[] parameters) {
			try {
				return Expression.Lambda<T>(body, parameters).Compile();
			} catch {
				return default(T); 
			}
		}
	}
	public static class AssignmentExpression {
		public static Expression Create(Expression left, Expression right) {
			return Expression.Call(null, ReflectionHelper.GetPublicStaticMethod(typeof(AssignmentExpression), "Assign").MakeGenericMethod(left.Type), left, right);
		}
		public static void Assign<T>(ref T left, T right) {
			left = right;
		}
	}
}
