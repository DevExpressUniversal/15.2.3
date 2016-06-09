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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using BF = System.Reflection.BindingFlags;
	static class ViewModelSourceProxy {
		static IDictionary<Type, IEnumerable<ICreateProxy>> proxiesCache = new Dictionary<Type, IEnumerable<ICreateProxy>>();
		internal static void Reset() {
			proxiesCache.Clear();
		}
		internal static object Create(Type viewModelSourceType, Type type, params object[] parameters) {
			IEnumerable<ICreateProxy> proxies;
			if(!proxiesCache.TryGetValue(type, out proxies)) {
				var createMethods = viewModelSourceType.GetMember("Create", System.Reflection.MemberTypes.Method, BF.Static | BF.Public);
				List<ICreateProxy> proxiesList = new List<ICreateProxy>(createMethods.Length);
				var constructors = type.GetConstructors(BF.Instance | BF.Public | BF.NonPublic);
				for(int i = 0; i < constructors.Length; i++) {
					var ctorParameters = constructors[i].GetParameters();
					if(ctorParameters.Length == 0 && !HasDefaultConstructorConstraint(createMethods[0] as MethodInfo))
						proxiesList.Add(new CreateProxy(createMethods[0] as MethodInfo, type));
					else
						proxiesList.Add(new CreateProxyParametrized(createMethods[1] as MethodInfo, type, constructors[i], ctorParameters));
				}
				proxies = proxiesList;
				proxiesCache.Add(type, proxies);
			}
			return TryCreate(parameters, proxies);
		}
		static bool HasDefaultConstructorConstraint(MethodInfo mInfo) {
			Type[] typeArgs = mInfo.GetGenericArguments();
			if(typeArgs.Length == 1 && typeArgs[0].GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
				return true;
			return false;
		}
		static object TryCreate(object[] parameters, IEnumerable<ICreateProxy> proxies) {
			while(true) {
				var proxy = proxies.FirstOrDefault(p => p.Match(parameters));
				if(proxy != null)
					return proxy.Create(parameters);
				if(parameters.Length != 0)
					parameters = Reduce(parameters);
				else return null;
			}
		}
		static object[] Reduce(object[] parameters) {
			object[] result = new object[parameters.Length - 1];
			Array.Copy(parameters, result, result.Length);
			return result;
		}
		#region CreateProxy
		interface ICreateProxy {
			object Create(params object[] parameters);
			bool Match(params object[] parameters);
		}
		sealed class CreateProxy : ICreateProxy {
			Func<object> create;
			public CreateProxy(MethodInfo mInfo, Type type) {
				var call = Expression.Call(mInfo.MakeGenericMethod(type));
				create = Expression.Lambda<Func<object>>(call).Compile();
			}
			object ICreateProxy.Create(params object[] parameters) {
				return create();
			}
			bool ICreateProxy.Match(params object[] parameters) {
				return parameters.Length == 0;
			}
		}
		sealed class CreateProxyParametrized : ICreateProxy {
			Func<object[], object> create;
			ParameterInfo[] ctorParameters;
			public CreateProxyParametrized(MethodInfo mInfo, Type type, ConstructorInfo ctorInfo, ParameterInfo[] ctorParameters) {
				this.ctorParameters = ctorParameters;
				var pp = Expression.Parameter(typeof(object[]), "parameters");
				Expression[] parameters = new Expression[ctorParameters.Length];
				for(int i = 0; i < ctorParameters.Length; i++) {
					var ppi = Expression.ArrayIndex(pp, Expression.Constant(i));
					parameters[i] = Expression.Convert(ppi, ctorParameters[i].ParameterType);
				}
				var ctorExpression = Expression.New(ctorInfo, parameters);
				var call = Expression.Call(mInfo.MakeGenericMethod(type),
						Expression.Lambda(ctorExpression)
					);
				create = Expression.Lambda<Func<object[], object>>(call, pp).Compile();
			}
			object ICreateProxy.Create(params object[] parameters) {
				object[] callParameters = new object[ctorParameters.Length];
				for(int i = 0; i < callParameters.Length; i++)
					callParameters[i] = (i < parameters.Length) ? parameters[i] : ctorParameters[i].DefaultValue;
				return create(callParameters);
			}
			bool ICreateProxy.Match(params object[] parameters) {
				int length = ctorParameters.Length;
				var ctorParameterTypes = GetCtorParameterTypes(length);
				while(true) {
					if(parameters.SequenceEqual(ctorParameterTypes, ProxyBase.ParameterTypesComparer))
						return true;
					if(length > 0)
						ctorParameterTypes = GetCtorParameterTypes(--length);
					else break;
				}
				return false;
			}
			IEnumerable<Type> GetCtorParameterTypes(int length) {
				return ctorParameters.Where((p, index) => !p.IsOptional || index < length).Select(p => p.ParameterType);
			}
		}
		#endregion CreateProxy
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using System.Linq.Expressions;
	using NUnit.Framework;
	#region Test Classes
	public class Foo { }
	public class Bar {
		public Bar() : this(null) { }
		public Bar(Foo foo) { this.Foo = foo; }
		public Foo Foo { get; private set; }
	}
	public class FooBar {
		protected FooBar(Bar bar = null) {
			this.Bar = bar;
		}
		public Bar Bar { get; private set; }
	}
	public class Zoo {
		protected Zoo() { }
	}
	public class TestViewModelSource {
		public static T Create<T>() where T : class, new() {
			return new T();
		}
		public static T Create<T>(Expression<Func<T>> constructorExpression) where T : class {
			return constructorExpression.Compile()();
		}
	}
	#endregion
	[TestFixture]
	public class ViewModelSourceProxyTests : IViewModelSource {
		IViewModelSource ViewModelSource;
		[TestFixtureSetUp]
		public void FixtureSetUp() {
			ViewModelSource = this;
		}
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			ViewModelSource = null;
			ResetViewModelSourceType();
			ViewModelSourceProxy.Reset();
		}
		[Test]
		public void Test00() {
			var foo = ViewModelSource.Create(typeof(Foo));
			Assert.IsNotNull(foo);
			Assert.IsTrue(foo is Foo);
			var bar1 = ViewModelSource.Create(typeof(Bar));
			Assert.IsNotNull(bar1);
			Assert.IsTrue(bar1 is Bar);
			Assert.IsNull(((Bar)bar1).Foo);
			var bar2 = ViewModelSource.Create(typeof(Bar), foo);
			Assert.IsNotNull(bar2);
			Assert.IsTrue(bar2 is Bar);
			Assert.AreEqual(foo, ((Bar)bar2).Foo);
		}
		[Test]
		public void Test01() {
			var foo = ViewModelSource.Create(typeof(Foo), 5);
			Assert.IsNotNull(foo);
			Assert.IsTrue(foo is Foo);
			var bar1 = ViewModelSource.Create(typeof(Bar), 5);
			Assert.IsNotNull(bar1);
			Assert.IsTrue(bar1 is Bar);
			Assert.IsNull(((Bar)bar1).Foo);
			var bar2 = ViewModelSource.Create(typeof(Bar), foo, 5);
			Assert.IsNotNull(bar2);
			Assert.IsTrue(bar2 is Bar);
			Assert.AreEqual(foo, ((Bar)bar2).Foo);
		}
		[Test]
		public void Test02_ProtectedCtor() {
			Bar b = new Bar();
			var fooBar = ViewModelSource.Create(typeof(FooBar), b);
			Assert.IsNotNull(fooBar);
			Assert.IsTrue(fooBar is FooBar);
			Assert.AreEqual(b, ((FooBar)fooBar).Bar);
		}
		[Test]
		public void Test02_CtorDefaultParameters() {
			var fooBar = ViewModelSource.Create(typeof(FooBar));
			Assert.IsNotNull(fooBar);
			Assert.IsTrue(fooBar is FooBar);
			Assert.IsNull(((FooBar)fooBar).Bar);
		}
		[Test]
		public void Test03_ProtectedCtorNewConstraint() {
			var zoo = ViewModelSource.Create(typeof(Zoo));
			Assert.IsNotNull(zoo);
			Assert.IsTrue(zoo is Zoo);
		}
		object IViewModelSource.Create(Type viewModelType, params object[] parameters) {
			return ViewModelSourceProxy.Create(GetViewModelSourceType(), viewModelType, parameters);
		}
		protected virtual Type GetViewModelSourceType() {
			return typeof(TestViewModelSource);
		}
		protected virtual void ResetViewModelSourceType() { 
		}
	}
}
#endif
