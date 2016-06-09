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
	using System.Linq.Expressions;
	using System.Reflection;
	static class @Notify {
		internal static void RaiseCanExecuteChanged(this object @this, Expression<Action> expression) {
			RaiseCanExecuteChanged(@this, ((MethodCallExpression)expression.Body).Method);
		}
		internal static void RaiseCanExecuteChanged<T>(this object @this, Expression<Action<T>> expression) {
			RaiseCanExecuteChanged(@this, ((MethodCallExpression)expression.Body).Method);
		}
		static void RaiseCanExecuteChanged(this object @this, MethodInfo methodInfo) {
			@this
				.@Get(x => GetGetPropertyValue(methodInfo, x.GetType()))
				.@Get(getProp => getProp(@this))
				.@Do(command =>
				{
					Action<object> raise = GetRaiseCanExecuteChanged(command);
					if(raise != null)
						raise(command);
				});
		}
		internal static void RaisePropertyChanged<T>(this object @this, Expression<Func<T>> expression) {
			RaisePropertyChanged(@this, ExpressionHelper.GetPropertyName(expression));
		}
		internal static void RaisePropertyChanged(this object @this, LambdaExpression propertySelector) {
			RaisePropertyChanged(@this, ExpressionHelper.GetPropertyName(propertySelector));
		}
		internal static void RaisePropertyChanged(this object @this, string propertyName) {
			@this.@Do(x =>
			{
				Action<object, string> raise = GetRaisePropertyChanged(@this);
				if(raise != null)
					raise(@this, propertyName);
			});
		}
		static IDictionary<string, Func<object, object>> getPropCache = new Dictionary<string, Func<object, object>>();
		static Func<object, object> GetGetPropertyValue(MethodInfo methodInfo, Type type) {
			string key = type.FullName + "." + methodInfo.Name;
			Func<object, object> getProp;
			if(!getPropCache.TryGetValue(key, out getProp)) {
				var getMethod = MethodInfoHelper.GetMethodInfo(type, "get_" + methodInfo.Name + "Command");
				if(getMethod != null) {
					var source = Expression.Parameter(typeof(object), "source");
					getProp = Expression.Lambda<Func<object, object>>(
								Expression.Call(Expression.Convert(source, getMethod.DeclaringType), getMethod),
								source
						).Compile();
					getPropCache.Add(key, getProp);
				}
				else getPropCache.Add(key, null);
			}
			return getProp;
		}
		static IDictionary<Type, Action<object, string>> raisePropertyCache = new Dictionary<Type, Action<object, string>>();
		static Action<object, string> GetRaisePropertyChanged(object @this) {
			Type type = @this.GetType();
			Action<object, string> raise;
			if(!raisePropertyCache.TryGetValue(type, out raise)) {
				var method = MethodInfoHelper.GetMethodInfo(type, "RaisePropertyChanged", new Type[] { typeof(string) });
				if(method != null) {
					var source = Expression.Parameter(typeof(object), "source");
					var parameter = Expression.Parameter(typeof(string), "parameter");
					raise = Expression.Lambda<Action<object, string>>(
								Expression.Call(Expression.Convert(source, method.DeclaringType), method, parameter),
								source, parameter
						).Compile();
					raisePropertyCache.Add(type, raise);
				}
				else raisePropertyCache.Add(type, null);
			}
			return raise;
		}
		static IDictionary<Type, Action<object>> raiseCanExecuteCache = new Dictionary<Type, Action<object>>();
		static Action<object> GetRaiseCanExecuteChanged(object @this) {
			Type type = @this.GetType();
			Action<object> raise;
			if(!raiseCanExecuteCache.TryGetValue(type, out raise)) {
				var method = MethodInfoHelper.GetMethodInfo(type, "RaiseCanExecuteChanged");
				if(method != null) {
					var source = Expression.Parameter(typeof(object), "source");
					raise = Expression.Lambda<Action<object>>(
								Expression.Call(Expression.Convert(source, method.DeclaringType), method),
								source
						).Compile();
					raiseCanExecuteCache.Add(type, raise);
				}
				else raiseCanExecuteCache.Add(type, null);
			}
			return raise;
		}
		internal static void Reset() {
			getPropCache.Clear();
			raisePropertyCache.Clear();
			raiseCanExecuteCache.Clear();
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.Filtering.Tests {
	using Internal;
	using NUnit.Framework;
	#region Test Classes
	class NPC_Obj {
		internal string changedProperty;
		public string Name { get; set; }
		protected void RaisePropertyChanged(string propertyName) {
			this.changedProperty = propertyName;
		}
		internal void OnNameChanged() {
			this.RaisePropertyChanged(() => Name);
		}
	}
	class Command {
		internal int counter = 0;
		protected void RaiseCanExecuteChanged() {
			counter++;
		}
	}
	class Obj_WithCommand {
		public Obj_WithCommand() {
			HelloCommand = new Command();
			SayCommand = new Command();
		}
		public object HelloCommand { get; private set; }
		public object SayCommand { get; private set; }
		public void Hello() { }
		public void Say(string text) { }
		internal void OnHelloChanged() {
			this.RaiseCanExecuteChanged(() => Hello());
		}
		internal void OnSayChanged() {
			this.RaiseCanExecuteChanged(() => Say(null));
		}
	}
	#endregion Test Classes
	[TestFixture]
	public class NotifyTests {
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			@Notify.Reset();
		}
		[Test]
		public void Test_00_NPC() {
			NPC_Obj obj = new NPC_Obj();
			Assert.IsNull(obj.changedProperty);
			obj.OnNameChanged();
			Assert.AreEqual("Name", obj.changedProperty);
		}
		[Test]
		public void Test_01_Command() {
			Obj_WithCommand obj = new Obj_WithCommand();
			Assert.AreEqual(0, ((Command)obj.HelloCommand).counter);
			obj.OnHelloChanged();
			Assert.AreEqual(1, ((Command)obj.HelloCommand).counter);
			Assert.AreEqual(0, ((Command)obj.SayCommand).counter);
			obj.OnSayChanged();
			Assert.AreEqual(1, ((Command)obj.SayCommand).counter);
			Assert.AreEqual(1, ((Command)obj.HelloCommand).counter);
		}
	}
}
#endif
