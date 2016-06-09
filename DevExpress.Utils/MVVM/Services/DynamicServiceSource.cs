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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using BF = System.Reflection.BindingFlags;
	using MA = System.Reflection.MethodAttributes;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class DynamicServiceSource {
		static readonly object[] EmptyParameters = new object[] { };
		public static T Create<T>(Type serviceType) {
			return activator.Create<T>(sourceType => CreateType(sourceType, new Type[] { serviceType }));
		}
		public static T Create<T, P1>(Type serviceType, P1 parameter1) {
			return activator.Create<T, P1>(sourceType => CreateType(sourceType, new Type[] { serviceType }), parameter1);
		}
		public static T Create<T, P1, P2>(Type serviceType, P1 parameter1, P2 parameter2) {
			return activator.Create<T, P1, P2>(sourceType => CreateType(sourceType, new Type[] { serviceType }), parameter1, parameter2);
		}
		public static T Create<T, P1, P2, P3>(Type serviceType, P1 parameter1, P2 parameter2, P3 parameter3) {
			return activator.Create<T, P1, P2, P3>(sourceType => CreateType(sourceType, new Type[] { serviceType }), parameter1, parameter2, parameter3);
		}
		public static T Create<T>(Type[] serviceTypes) {
			return activator.Create<T>(sourceType => CreateType(sourceType, serviceTypes));
		}
		public static T Create<T, P1>(Type[] serviceTypes, P1 parameter1) {
			return activator.Create<T, P1>(sourceType => CreateType(sourceType, serviceTypes), parameter1);
		}
		public static T Create<T, P1, P2>(Type[] serviceTypes, P1 parameter1, P2 parameter2) {
			return activator.Create<T, P1, P2>(sourceType => CreateType(sourceType, serviceTypes), parameter1, parameter2);
		}
		public static T Create<T, P1, P2, P3>(Type[] serviceTypes, P1 parameter1, P2 parameter2, P3 parameter3) {
			return activator.Create<T, P1, P2, P3>(sourceType => CreateType(sourceType, serviceTypes), parameter1, parameter2, parameter3);
		}
		static TypesActivator activator = new TypesActivator();
		static Type CreateType(Type sourceType, Type[] interfaces) {
			var typeBuilder = DynamicTypesHelper.GetTypeBuilder(interfaces[0], sourceType);
			var cInfos = sourceType.GetConstructors(BF.Instance | BF.NonPublic | BF.Public);
			for(int i = 0; i < cInfos.Length; i++)
				CreateConstructor(cInfos[i], typeBuilder, sourceType);
			for(int i = 0; i < interfaces.Length; i++)
				CreateInterfaceImplementation(sourceType, interfaces[i], typeBuilder);
			return typeBuilder.CreateType();
		}
		static void CreateInterfaceImplementation(Type sourceType, Type interfaceType, TypeBuilder typeBuilder) {
			IDictionary<MethodInfo, MethodBuilder> methodsCache = new Dictionary<MethodInfo, MethodBuilder>();
			var methods = interfaceType.GetMethods();
			for(int i = 0; i < methods.Length; i++) {
				var builder = CreateMethod(methods[i], typeBuilder, sourceType);
				if(builder != null) {
					methodsCache.Add(methods[i], builder);
					typeBuilder.DefineMethodOverride(builder, methods[i]);
				}
			}
			var properties = interfaceType.GetProperties();
			for(int i = 0; i < properties.Length; i++)
				CreateProperty(properties[i], typeBuilder, methodsCache);
			var events = interfaceType.GetEvents();
			for(int i = 0; i < events.Length; i++)
				CreateEvent(events[i], typeBuilder, methodsCache);
			typeBuilder.AddInterfaceImplementation(interfaceType);
		}
		internal static void CreateConstructor(ConstructorInfo cInfo, TypeBuilder typeBuilder, Type sourceType) {
			var parameterTypes = GetParameterTypes(cInfo);
			var ctorBuilder = typeBuilder.DefineConstructor(MA.Public, CallingConventions.Standard, parameterTypes);
			var ctorGenerator = ctorBuilder.GetILGenerator();
			ctorGenerator.Emit(OpCodes.Ldarg_0);
			EmitLdargs(parameterTypes, ctorGenerator);
			ctorGenerator.Emit(OpCodes.Call, cInfo);
			ctorGenerator.Emit(OpCodes.Ret);
		}
		const MethodAttributes methodAttributes = MA.Private | MA.HideBySig | MA.NewSlot | MA.Virtual | MA.Final;
		static MethodBuilder CreateMethod(MethodInfo mInfo, TypeBuilder typeBuilder, Type sourceType) {
			var parameterTypes = GetParameterTypes(mInfo);
			var customAttributes = (mInfo.IsSpecialName) ? MA.SpecialName : MA.Private;
			var methodBuilder = typeBuilder.DefineMethod(DefineName(mInfo), customAttributes | methodAttributes, mInfo.ReturnType, parameterTypes);
			var methodGenerator = methodBuilder.GetILGenerator();
			var sourceMethod = sourceType.GetMethod(mInfo.Name, CheckParameterTypes(parameterTypes, mInfo.DeclaringType));
			if(sourceMethod != null) {
				methodGenerator.Emit(OpCodes.Ldarg_0); 
				EmitLdargs(parameterTypes, methodGenerator);
				methodGenerator.Emit(OpCodes.Call, sourceMethod);
				if(sourceMethod.ReturnType != CheckEnumType(mInfo.ReturnType, mInfo.DeclaringType))
					methodGenerator.Emit(OpCodes.Castclass, mInfo.ReturnType);
			}
			else {
				if(mInfo.ReturnType != typeof(void)) {
					if(!mInfo.ReturnType.IsValueType)
						methodGenerator.Emit(OpCodes.Ldnull);
					else {
						Action<ILGenerator> generate;
						if(defaultValuesGenerator.TryGetValue(CheckEnumType(mInfo.ReturnType), out generate))
							generate(methodGenerator);
					}
				}
			}
			methodGenerator.Emit(OpCodes.Ret);
			return methodBuilder;
		}
		static PropertyBuilder CreateProperty(PropertyInfo pInfo, TypeBuilder typeBuilder, IDictionary<MethodInfo, MethodBuilder> methodsCache) {
			var propertyBuilder = typeBuilder.DefineProperty(DefineName(pInfo), PropertyAttributes.None, CallingConventions.HasThis, pInfo.PropertyType, null);
			if(pInfo.CanRead)
				propertyBuilder.SetGetMethod(methodsCache[pInfo.GetGetMethod()]);
			if(pInfo.CanWrite)
				propertyBuilder.SetSetMethod(methodsCache[pInfo.GetSetMethod()]);
			return propertyBuilder;
		}
		static EventBuilder CreateEvent(EventInfo eInfo, TypeBuilder typeBuilder, IDictionary<MethodInfo, MethodBuilder> methodsCache) {
			var eventBuilder = typeBuilder.DefineEvent(DefineName(eInfo), EventAttributes.None, eInfo.EventHandlerType);
			eventBuilder.SetAddOnMethod(methodsCache[eInfo.GetAddMethod()]);
			eventBuilder.SetRemoveOnMethod(methodsCache[eInfo.GetRemoveMethod()]);
			return eventBuilder;
		}
		static string DefineName(MemberInfo memberInfo) {
			return memberInfo.DeclaringType.FullName + "." + memberInfo.Name;
		}
		static Type CheckEnumType(Type type) {
			return type.IsEnum ? type.GetEnumUnderlyingType() : type;
		}
		static Type CheckEnumType(Type type, Type interfaceType) {
			return (interfaceType.Assembly == type.Assembly) ? CheckEnumType(type) : type;
		}
		static IDictionary<Type, Action<ILGenerator>> defaultValuesGenerator = new Dictionary<Type, Action<ILGenerator>> { 
			{ typeof(int), (g) => g.Emit(OpCodes.Ldc_I4_0) },
			{ typeof(byte), (g) => g.Emit(OpCodes.Ldc_I4_0) },
			{ typeof(short), (g) => g.Emit(OpCodes.Ldc_I4_0) },
			{ typeof(long), (g) => g.Emit(OpCodes.Ldc_I8, (long)0) },
			{ typeof(float), (g) => g.Emit(OpCodes.Ldc_R4, 0f) },
			{ typeof(double), (g) => g.Emit(OpCodes.Ldc_R8, 0.0) },
		};
		static Type[] GetParameterTypes(MethodBase method) {
			return method.GetParameters().Select(p => p.ParameterType).ToArray();
		}
		static Type[] CheckParameterTypes(Type[] parameterTypes, Type interfaceType) {
			return parameterTypes.Select(t => CheckEnumType(t, interfaceType)).ToArray();
		}
		static OpCode[] args = new OpCode[] { OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3 };
		static void EmitLdargs(Array parameters, ILGenerator generator) {
			for(int i = 0; i < parameters.Length; i++) {
				if(i < 3)
					generator.Emit(args[i]);
				else
					generator.Emit(OpCodes.Ldarg_S, i + 1);
			}
		}
		public static void Reset() {
			activator.Reset();
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using DevExpress.Utils.MVVM.Services;
	using NUnit.Framework;
	#region TestClasses
	public interface ITestService {
		string Name { get; set; }
		void ReverseName();
	}
	public class TestService {
		public string Name { get; set; }
		public void ReverseName() {
			char[] charArray = Name.ToCharArray();
			System.Array.Reverse(charArray);
			Name = new string(charArray);
		}
	}
	public interface ITestService2 {
		ITestService Owner { get; set; }
		bool CheckOwner(ITestService owner);
	}
	public class TestService2 {
		public object Owner { get; set; }
		public bool CheckOwner(object owner) {
			return owner != null && owner is ITestService;
		}
	}
	public interface ITestService3 {
		ITestService Owner { get; set; }
		ITestService GetParent(ITestService owner);
	}
	public class TestService3 { }
	public class TestService4 {
		public string Name { get; set; }
		public object Owner { get; set; }
		public bool CheckOwner(object owner) {
			return owner != null && owner is ITestService;
		}
	}
	public interface ITestService5 {
		int Sum { get; }
	}
	public class TestService5 {
		protected TestService5(int a) {
			Sum = a;
		}
		protected TestService5(int a, int b) {
			Sum = a + b;
		}
		protected TestService5(int a, int b, int c) {
			Sum = a + b + c;
		}
		public int Sum { get; private set; }
	}
	public enum TestEnum1 { One = 1, Two = 2, Three = 3 }
	public interface ITestService6 {
		TestEnum1 Calc(TestEnum1 a, TestEnum1 b);
	}
	public class TestService6 {
		public int Calc(int a, int b) { return a + b; }
	}
	#endregion
	[TestFixture]
	public class DynamicServiceSourceTests {
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			DynamicServiceSource.Reset();
		}
		[Test]
		public void Test00_SimplePropertiesAndMethods() {
			TestService service = DynamicServiceSource.Create<TestService>(typeof(ITestService));
			Assert.IsNotNull(service);
			ITestService ts = service as ITestService;
			Assert.IsNotNull(ts);
			ts.Name = "Test";
			Assert.AreEqual("Test", service.Name);
			ts.ReverseName();
			Assert.AreEqual("tseT", service.Name);
			service.Name = "AAA";
			Assert.AreEqual("AAA", ts.Name);
		}
		[Test]
		public void Test02_BoxedPropertiesAndMethodParameters() {
			ITestService serviceOwner = DynamicServiceSource.Create<TestService>(typeof(ITestService)) as ITestService;
			TestService2 service = DynamicServiceSource.Create<TestService2>(typeof(ITestService2));
			Assert.IsNotNull(service);
			ITestService2 ts2 = service as ITestService2;
			Assert.IsNotNull(ts2);
			ts2.Owner = serviceOwner;
			Assert.AreEqual(serviceOwner, service.Owner);
			Assert.IsTrue(ts2.CheckOwner(serviceOwner));
			Assert.IsTrue(service.CheckOwner(serviceOwner));
			Assert.IsFalse(service.CheckOwner(new object()));
		}
		[Test]
		public void Test03_OptionalPropertiesAndMethods() {
			TestService3 service = DynamicServiceSource.Create<TestService3>(typeof(ITestService3));
			Assert.IsNotNull(service);
			ITestService3 ts3 = service as ITestService3;
			Assert.IsNotNull(ts3);
			Assert.IsNull(ts3.Owner);
			Assert.IsNull(ts3.GetParent(null));
		}
		[Test]
		public void Test04_MultipleInterfaces() {
			TestService4 service = DynamicServiceSource.Create<TestService4>(
				new System.Type[] { typeof(ITestService), typeof(ITestService2), typeof(ITestService3) });
			Assert.IsNotNull(service);
			Assert.IsNotNull(service as ITestService);
			Assert.IsNotNull(service as ITestService2);
			Assert.IsNotNull(service as ITestService3);
		}
		[Test]
		public void Test05_MultipleCtorParameters() {
			TestService5 service1 = DynamicServiceSource.Create<TestService5, int>(typeof(ITestService5), 1);
			TestService5 service2 = DynamicServiceSource.Create<TestService5, int, int>(typeof(ITestService5), 2, 2);
			TestService5 service3 = DynamicServiceSource.Create<TestService5, int, int, int>(typeof(ITestService5), 3, 3, 3);
			Assert.AreEqual(1, (service1 as ITestService5).Sum);
			Assert.AreEqual(4, (service2 as ITestService5).Sum);
			Assert.AreEqual(9, (service3 as ITestService5).Sum);
		}
		[Test]
		public void Test06_EnumConversionForMethodParameters() {
			TestService6 service = DynamicServiceSource.Create<TestService6>(typeof(ITestService6));
			Assert.IsNotNull(service);
			ITestService6 ts6 = service as ITestService6;
			Assert.AreEqual(TestEnum1.Three, ts6.Calc(TestEnum1.One, TestEnum1.Two));
		}
	}
}
#endif
