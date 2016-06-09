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
using System.Linq;
using System.Reflection;
namespace DevExpress.Mvvm.Native {
	public static class DelegateCommandFactory {
		internal static MethodInfo GetGenericMethodWithoutResult(Type parameterType, bool withUseCommandManagerParameter) {
			var method = GetMethodByParameter("Create", withUseCommandManagerParameter ? new Type[] { typeof(Action<>), typeof(Func<,>), typeof(bool) } : new Type[] { typeof(Action<>), typeof(Func<,>) });
			return method.MakeGenericMethod(parameterType);
		}
		internal static MethodInfo GetGenericMethodWithResult(Type parameterType1, Type parameterType2, bool withUseCommandManagerParameter) {
			var method = GetMethodByParameter("CreateFromFunction", withUseCommandManagerParameter ? new Type[] { typeof(Func<,>), typeof(Func<,>), typeof(bool) } : new Type[] { typeof(Func<,>), typeof(Func<,>) });
			return method.MakeGenericMethod(parameterType1, parameterType2);
		}
		internal static MethodInfo GetSimpleMethodWithoutResult(bool withUseCommandManagerParameter) {
			var method = GetMethodByParameter("Create", withUseCommandManagerParameter ? new Type[] { typeof(Action), typeof(Func<bool>), typeof(bool) } : new Type[] { typeof(Action), typeof(Func<bool>) });
			return method;
		}
		internal static MethodInfo GetSimpleMethodWithResult(Type parameterType, bool withUseCommandManagerParameter) {
			var method = GetMethodByParameter("CreateFromFunction", withUseCommandManagerParameter ? new Type[] { typeof(Func<>), typeof(Func<bool>), typeof(bool) } : new Type[] { typeof(Func<>), typeof(Func<bool>) });
			return method.MakeGenericMethod(parameterType);
		}
		static MethodInfo GetMethodByParameter(string methodName, Type[] parameterTypes) {
			Type delegateCommandFactoryType = typeof(DelegateCommandFactory);
			MethodInfo[] methodInfos = delegateCommandFactoryType.GetMethods();
			var methods = methodInfos.Where(m => m.Name == methodName);
			foreach(MethodInfo methodInfo in methods) {
				ParameterInfo[] parameterInfos = methodInfo.GetParameters();
				if(parameterInfos.Length != parameterTypes.Length) 
					continue;
				bool isThisMatched = true;
				for(int i = 0; i < parameterInfos.Length; i++) {
					if(parameterInfos[i].ParameterType.Name != parameterTypes[i].Name) {
						isThisMatched = false;
						break;
					}
				}
				if(isThisMatched)
					return methodInfo;
			}
			return null;
		}
		public static DelegateCommand<T> CreateFromFunction<T, TResult>(Func<T, TResult> executeMethod, Func<T, bool> canExecuteMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand<T>(x => executeMethod(x), canExecuteMethod, useCommandManager);
#else
			return new DelegateCommand<T>(x => executeMethod(x), canExecuteMethod);
#endif
		}
		public static DelegateCommand<T> CreateFromFunction<T, TResult>(Func<T, TResult> executeMethod, Func<T, bool> canExecuteMethod) {
			return new DelegateCommand<T>(x => executeMethod(x), canExecuteMethod);
		}
		public static DelegateCommand<T> Create<T>(Action<T> executeMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand<T>(executeMethod, useCommandManager);
#else
			return new DelegateCommand<T>(executeMethod);
#endif
		}
		public static DelegateCommand<T> Create<T>(Action<T> executeMethod) {
			return new DelegateCommand<T>(executeMethod);
		}
		public static DelegateCommand<T> Create<T>(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand<T>(executeMethod, canExecuteMethod, useCommandManager);
#else
			return new DelegateCommand<T>(executeMethod, canExecuteMethod);
#endif
		}
		public static DelegateCommand<T> Create<T>(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			return new DelegateCommand<T>(executeMethod, canExecuteMethod);
		}
		public static DelegateCommand CreateFromFunction<TResult>(Func<TResult> executeMethod, Func<bool> canExecuteMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand(() => executeMethod(), canExecuteMethod, useCommandManager);
#else
			return new DelegateCommand(() => executeMethod(), canExecuteMethod);
#endif
		}
		public static DelegateCommand CreateFromFunction<TResult>(Func<TResult> executeMethod, Func<bool> canExecuteMethod) {
			return new DelegateCommand(() => executeMethod(), canExecuteMethod);
		}
		public static DelegateCommand Create(Action executeMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand(executeMethod, useCommandManager);
#else
			return new DelegateCommand(executeMethod);
#endif
		}
		public static DelegateCommand Create(Action executeMethod) {
			return new DelegateCommand(executeMethod);
		}
		public static DelegateCommand Create(Action executeMethod, Func<bool> canExecuteMethod, bool useCommandManager) {
#if !SILVERLIGHT
			return new DelegateCommand(executeMethod, canExecuteMethod, useCommandManager);
#else
			return new DelegateCommand(executeMethod, canExecuteMethod);
#endif
		}
		public static DelegateCommand Create(Action executeMethod, Func<bool> canExecuteMethod) {
			return new DelegateCommand(executeMethod, canExecuteMethod);
		}
	}
}
