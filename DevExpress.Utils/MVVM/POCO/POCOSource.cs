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

namespace DevExpress.Utils.MVVM.Internal {
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Reflection.Emit;
	using DevExpress.Utils.MVVM.Services;
	using BF = System.Reflection.BindingFlags;
	using MA = System.Reflection.MethodAttributes;
	public static class POCOSource {
		static TypesActivator activator = new TypesActivator();
		public static T Create<T>() {
			ValidateType(typeof(T));
			return activator.Create<T>(CreateType);
		}
		public static T Create<T>(Expression<Func<T>> constructorExpression) where T : class {
			ValidateCtorExpression(constructorExpression, typeof(T));
			return Expression.Lambda<Func<T>>(GetCtorExpression(constructorExpression, typeof(T))).Compile()();
		}
		#region Factory
		public static Func<T> Factory<T>() {
			ValidateType(typeof(T));
			return () => activator.Create<T>(CreateType);
		}
		public static Func<P1, T> Factory<T, P1>() {
			ValidateType(typeof(T));
			return (P1 p1) => activator.Create<T, P1>(CreateType, p1);
		}
		public static Func<P1, P2, T> Factory<T, P1, P2>() {
			ValidateType(typeof(T));
			return (P1 p1, P2 p2) => activator.Create<T, P1, P2>(CreateType, p1, p2);
		}
		public static Func<P1, P2, P3, T> Factory<T, P1, P2, P3>() {
			ValidateType(typeof(T));
			return (P1 p1, P2 p2, P3 p3) => activator.Create<T, P1, P2, P3>(CreateType, p1, p2, p3);
		}
		#endregion Factory
		static Expression GetCtorExpression(LambdaExpression constructorExpression, Type sourceType) {
			Type pocoType = activator.CreateType(sourceType, CreateType);
			NewExpression newExpression = constructorExpression.Body as NewExpression;
			if(newExpression != null)
				return GetNewExpression(pocoType, newExpression);
			MemberInitExpression memberInitExpression = constructorExpression.Body as MemberInitExpression;
			if(memberInitExpression == null)
				Throw(Error_TypeHasNoCtors, sourceType);
			return Expression.MemberInit(GetNewExpression(pocoType, memberInitExpression.NewExpression), memberInitExpression.Bindings);
		}
		static NewExpression GetNewExpression(Type type, NewExpression newExpression) {
			var parameterTypes = newExpression.Constructor.GetParameters()
				.Select(x => x.ParameterType).ToArray();
			return Expression.New(GetConstructor(type, parameterTypes), newExpression.Arguments);
		}
		static ConstructorInfo GetConstructor(Type proxyType, Type[] argsTypes) {
			var ctor = proxyType.GetConstructor(argsTypes ?? Type.EmptyTypes);
			if(ctor == null)
				Throw(Error_TypeHasNoCtors, proxyType);
			return ctor;
		}
		internal static bool ValidateType(Type type) {
			if(!type.IsPublic && !type.IsNestedPublic)
				Throw(Error_InternalClass, type);
			if(type.IsSealed)
				Throw(Error_SealedClass, type);
			return true;
		}
		static void ValidateCtorExpression(LambdaExpression constructorExpression, Type sourceType) {
			NewExpression newExpression = constructorExpression.Body as NewExpression;
			if(newExpression != null)
				return;
			MemberInitExpression memberInitExpression = constructorExpression.Body as MemberInitExpression;
			if(memberInitExpression == null)
				Throw(Error_ConstructorExpressionCanOnlyBeOfNewExpressionType, sourceType);
		}
		internal static void Reset() {
			activator.Reset();
		}
		static Type CreateType(Type sourceType) {
			return CreateType(sourceType, null, null, null);
		}
		internal static Type CreateType(Type sourceType, string typeNameModifier, Func<PropertyInfo, bool> forceBindableProperty, Action<PropertyInfo, PropertyBuilder> buildPropertyAttributes) {
			var moduleBuilder = DynamicTypesHelper.GetModuleBuilder(sourceType.Assembly);
			var typeBuilder = moduleBuilder.DefineType(
				DynamicTypesHelper.GetDynamicTypeName(sourceType, typeNameModifier), TypeAttributes.NotPublic, sourceType,
				new Type[] { typeof(INotifyPropertyChanged) });
			BuildConstructors(sourceType, typeBuilder);
			var raisePropertyChangedMethod = ImplementINotifyPropertyChanged(sourceType, typeBuilder);
			BuildBindableProperties(sourceType, typeBuilder, raisePropertyChangedMethod, forceBindableProperty, buildPropertyAttributes);
			BuildCommands(sourceType, typeBuilder);
			BuildParentViewModelProperty(sourceType, typeBuilder, raisePropertyChangedMethod);
			return typeBuilder.CreateType();
		}
		static IMVVMTypesResolver GetTypesResolver() {
			return POCOTypesResolver.Instance;
		}
		#region Constructors
		static void BuildConstructors(Type sourceType, TypeBuilder typeBuilder) {
			var cInfos = sourceType.GetConstructors(BF.Instance | BF.NonPublic | BF.Public);
			if(cInfos.Length == 0)
				Throw(Error_TypeHasNoCtors, sourceType);
			for(int i = 0; i < cInfos.Length; i++)
				DynamicServiceSource.CreateConstructor(cInfos[i], typeBuilder, sourceType);
		}
		#endregion constructors
		#region INotifyPropertyChanged implementation
		static MethodBuilder BuildGetPropertyChangedHelperMethod(TypeBuilder type, Type helperType) {
			MethodAttributes methodAttributes = MA.Private | MA.HideBySig;
			MethodBuilder method = type.DefineMethod("GetHelper", methodAttributes);
			method.SetReturnType(helperType);
			ILGenerator gen = method.GetILGenerator();
			gen.DeclareLocal(helperType);
			FieldBuilder propertyChangedHelperField = type.DefineField("propertyChangedHelper", helperType, FieldAttributes.Private);
			Label returnLabel = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, propertyChangedHelperField);
			gen.Emit(OpCodes.Dup);
			gen.Emit(OpCodes.Brtrue_S, returnLabel);
			gen.Emit(OpCodes.Pop);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Newobj, helperType.GetConstructor(Type.EmptyTypes));
			gen.Emit(OpCodes.Dup);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Stfld, propertyChangedHelperField);
			gen.Emit(OpCodes.Ldloc_0);
			gen.MarkLabel(returnLabel);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static MethodBuilder BuildRaisePropertyChangedMethod(TypeBuilder type, MethodInfo getHelperMethod, MethodInfo mInfo) {
			MethodAttributes methodAttributes = MA.Family | MA.HideBySig;
			MethodBuilder method = type.DefineMethod("RaisePropertyChanged", methodAttributes);
			method.SetReturnType(typeof(void));
			method.SetParameters(typeof(string));
			method.DefineParameter(1, ParameterAttributes.None, "propertyName");
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, getHelperMethod);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Callvirt, mInfo);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static MethodInfo ImplementINotifyPropertyChanged(Type baseType, TypeBuilder typeBuilder) {
			MethodInfo raisePropertyChangedMethod;
			if(typeof(INotifyPropertyChanged).IsAssignableFrom(baseType)) {
				raisePropertyChangedMethod = baseType.GetMethods(BF.NonPublic | BF.Instance | BF.Public)
					.FirstOrDefault(x =>
					{
						var parameters = x.GetParameters();
						return (x.Name == "RaisePropertyChanged") && MemberInfoHelper.CanAccessFromDescendant(x)
							&& parameters.Length == 1
							&& parameters[0].ParameterType == typeof(string)
							&& !parameters[0].IsOut
							&& !parameters[0].ParameterType.IsByRef;
					});
				if(raisePropertyChangedMethod == null)
					Throw(Error_RaisePropertyChangedMethodNotFound, baseType);
			}
			else {
				var helperType = typeof(PropertyChangedHelper);
				var getHelperMethod = BuildGetPropertyChangedHelperMethod(typeBuilder, helperType);
				raisePropertyChangedMethod = BuildRaisePropertyChangedMethod(typeBuilder, getHelperMethod, helperType.GetMethod("OnPropertyChanged"));
				var addHandler = BuildAddRemovePropertyChangedHandler(typeBuilder, getHelperMethod, "add", helperType.GetMethod("AddHandler"));
				typeBuilder.DefineMethodOverride(addHandler, typeof(INotifyPropertyChanged).GetMethod("add_PropertyChanged"));
				var removeHandler = BuildAddRemovePropertyChangedHandler(typeBuilder, getHelperMethod, "remove", helperType.GetMethod("RemoveHandler"));
				typeBuilder.DefineMethodOverride(removeHandler, typeof(INotifyPropertyChanged).GetMethod("remove_PropertyChanged"));
			}
			return raisePropertyChangedMethod;
		}
		static MethodBuilder BuildAddRemovePropertyChangedHandler(TypeBuilder type, MethodInfo getHelperMethod, string methodName, MethodInfo mInfo) {
			MethodAttributes methodAttributes = MA.Private | MA.Virtual | MA.Final | MA.HideBySig | MA.NewSlot;
			MethodBuilder method = type.DefineMethod(typeof(INotifyPropertyChanged).FullName + "." + methodName + "_PropertyChanged", methodAttributes);
			method.SetReturnType(typeof(void));
			method.SetParameters(typeof(PropertyChangedEventHandler));
			method.DefineParameter(1, ParameterAttributes.None, "value");
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, getHelperMethod);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Callvirt, mInfo);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		#endregion
		#region Bindable properties
		static void BuildBindableProperties(Type type, TypeBuilder typeBuilder, MethodInfo raisePropertyChangedMethod, Func<PropertyInfo, bool> forceBindableProperty, Action<PropertyInfo, PropertyBuilder> buildPropertyAttributes) {
			foreach(var propertyInfo in MemberInfoHelper.GetBindableProperties(GetTypesResolver(), type, forceBindableProperty)) {
				var getter = BuildBindablePropertyGetter(typeBuilder,
					propertyInfo.GetGetMethod());
				typeBuilder.DefineMethodOverride(getter, propertyInfo.GetGetMethod());
				bool generateRaisePropertyChanged = GenerateRaisePropertyChanged(forceBindableProperty, propertyInfo);
				var setter = BuildBindablePropertySetter(typeBuilder,
					generateRaisePropertyChanged ? raisePropertyChangedMethod : null, propertyInfo,
					GetPropertyChangedMethod(type, propertyInfo, "Changed"),
					GetPropertyChangedMethod(type, propertyInfo, "Changing"));
				typeBuilder.DefineMethodOverride(setter, propertyInfo.GetSetMethod(true));
				var newProperty = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new Type[0]);
				newProperty.SetGetMethod(getter);
				newProperty.SetSetMethod(setter);
				if(buildPropertyAttributes != null)
					buildPropertyAttributes(propertyInfo, newProperty);
			}
		}
		static bool GenerateRaisePropertyChanged(Func<PropertyInfo, bool> forceBindableProperty, PropertyInfo propertyInfo) {
			if(forceBindableProperty != null && forceBindableProperty(propertyInfo))
				return MemberInfoHelper.IsAutoImplemented(propertyInfo);
			return true;
		}
		static MethodInfo GetPropertyChangedMethod(Type type, PropertyInfo propertyInfo, string methodNameSuffix) {
			if(!MemberInfoHelper.IsAutoImplemented(propertyInfo))
				return null;
			return GetPropertyChangedMethodCore(type, propertyInfo.Name, propertyInfo.PropertyType, methodNameSuffix);
		}
		static MethodInfo GetPropertyChangedMethodCore(Type type, string propertyName, Type propertyType, string methodNameSuffix) {
			string onChangedMethodName = "On" + propertyName + methodNameSuffix;
			MethodInfo[] changedMethods = type.GetMethods(BF.NonPublic | BF.Public | BF.Instance)
				.Where(x => x.Name == onChangedMethodName).ToArray();
			if(changedMethods.Length > 1)
				Throw(Error_MoreThanOnePropertyChangedMethod, propertyName);
			var changedMethod = changedMethods.FirstOrDefault();
			if(changedMethod != null)
				CheckOnChangedMethod(changedMethod, propertyType);
			return changedMethod;
		}
		static void CheckOnChangedMethod(MethodInfo method, Type propertyType) {
			if(!MemberInfoHelper.CanAccessFromDescendant(method))
				Throw(Error_PropertyChangedMethodShouldBePublicOrProtected, method);
			var parameters = method.GetParameters();
			if(parameters.Length >= 2)
				Throw(Error_PropertyChangedCantHaveMoreThanOneParameter, method);
			if(parameters.Length == 1 && parameters[0].ParameterType != propertyType)
				Throw(Error_PropertyChangedMethodArgumentTypeShouldMatchPropertyType, method);
			if(method.ReturnType != typeof(void))
				Throw(Error_PropertyChangedCantHaveReturnType, method);
		}
		static MethodBuilder BuildBindablePropertyGetter(TypeBuilder type, MethodInfo originalGetter) {
			MethodAttributes methodAttributes = MA.Public | MA.Virtual | MA.HideBySig;
			MethodBuilder method = type.DefineMethod(originalGetter.Name, methodAttributes);
			method.SetReturnType(originalGetter.ReturnType);
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, originalGetter);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static MethodInfo mInfoEquals;
		static MethodBuilder BuildBindablePropertySetter(TypeBuilder type, MethodInfo raisePropertyChangedMethod, PropertyInfo property, MethodInfo propertyChangedMethod, MethodInfo propertyChangingMethod) {
			var setMethod = property.GetSetMethod(true);
			MethodAttributes methodAttributes = (setMethod.IsPublic ? MA.Public : MA.Family) | MA.Virtual | MA.HideBySig;
			MethodBuilder method = type.DefineMethod(setMethod.Name, methodAttributes);
			method.SetReturnType(typeof(void));
			method.SetParameters(property.PropertyType);
			method.DefineParameter(1, ParameterAttributes.None, "value");
			ILGenerator gen = method.GetILGenerator();
			gen.DeclareLocal(property.PropertyType);
			Label returnLabel = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, property.GetGetMethod());
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			bool shouldBoxValues = property.PropertyType.IsValueType;
			if(shouldBoxValues)
				gen.Emit(OpCodes.Box, property.PropertyType);
			gen.Emit(OpCodes.Ldarg_1);
			if(shouldBoxValues)
				gen.Emit(OpCodes.Box, property.PropertyType);
			if(mInfoEquals == null)
				mInfoEquals = typeof(object).GetMethod("Equals", BF.Static | BF.Public);
			gen.Emit(OpCodes.Call, mInfoEquals);
			gen.Emit(OpCodes.Brtrue_S, returnLabel);
			if(propertyChangingMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				if(propertyChangingMethod.GetParameters().Length == 1)
					gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Call, propertyChangingMethod);
			}
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Call, setMethod);
			if(propertyChangedMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				if(propertyChangedMethod.GetParameters().Length == 1)
					gen.Emit(OpCodes.Ldloc_0);
				gen.Emit(OpCodes.Call, propertyChangedMethod);
			}
			if(raisePropertyChangedMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldstr, property.Name);
				gen.Emit(OpCodes.Call, raisePropertyChangedMethod);
			}
			gen.MarkLabel(returnLabel);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		#endregion
		#region Commands
		static void BuildCommands(Type type, TypeBuilder typeBuilder) {
			MethodInfo[] methods = MemberInfoHelper.GetCommandMethods(GetTypesResolver(), type);
			foreach(var commandMethod in methods) {
				bool isAsyncCommand = commandMethod.ReturnType == typeof(System.Threading.Tasks.Task);
				string commandName = GetCommandName(commandMethod);
				if(type.GetMember(commandName, BF.Instance | BF.Static | BF.NonPublic | BF.Public)
					.Any() || methods.Any(x => x != commandMethod && GetCommandName(x) == commandName))
					Throw(Error_MemberWithSameCommandNameAlreadyExists, commandMethod);
				MethodInfo canExecuteMethod = GetCanExecuteMethod(type, commandMethod);
				var getMethod = BuildGetCommandMethod(typeBuilder, commandMethod, canExecuteMethod, commandName, isAsyncCommand);
				PropertyBuilder commandProperty = typeBuilder.DefineProperty(
					commandName, PropertyAttributes.None, getMethod.ReturnType, null);
				commandProperty.SetGetMethod(getMethod);
			}
		}
		static MethodInfo GetCanExecuteMethod(Type type, MethodInfo method) {
			return GetCanExecuteMethod(type, method, x => new POCOException(x), MemberInfoHelper.CanAccessFromDescendant);
		}
		internal static MethodInfo GetCanExecuteMethod(Type type, MethodInfo methodInfo, Func<string, Exception> createException, Func<MethodInfo, bool> canAccessMethod) {
			string canExecuteMethodName = GetCanExecuteMethodName(methodInfo);
			MethodInfo canExecuteMethod = type.GetMethod(canExecuteMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(canExecuteMethod != null)
				CheckCanExecuteMethod(methodInfo, createException, canExecuteMethod, canAccessMethod);
			return canExecuteMethod;
		}
		static string GetCanExecuteMethodName(MethodInfo commandMethod) {
			return CanExecutePrefix + commandMethod.Name;
		}
		static string GetCommandName(MethodInfo method) {
			return method.Name + CommandSuffix;
		}
		static void CheckCanExecuteMethod(MethodInfo methodInfo, Func<string, Exception> createException, MethodInfo canExecuteMethod, Func<MethodInfo, bool> canAccessMethod) {
			ParameterInfo[] parameters = methodInfo.GetParameters();
			ParameterInfo[] canExecuteParameters = canExecuteMethod.GetParameters();
			if(parameters.Length != canExecuteParameters.Length)
				Throw(Error_CanExecuteMethodHasIncorrectParameters, canExecuteMethod);
			if(parameters.Length == 1 && (parameters[0].ParameterType != canExecuteParameters[0].ParameterType || parameters[0].IsOut != canExecuteParameters[0].IsOut))
				Throw(Error_CanExecuteMethodHasIncorrectParameters, canExecuteMethod);
			if(!canAccessMethod(canExecuteMethod))
				Throw(Error_MethodShouldBePublic, canExecuteMethod);
		}
		static Type[] SimpleDelegateTypes = new Type[] { typeof(object), typeof(IntPtr) };
		static MethodBuilder BuildGetCommandMethod(TypeBuilder typeBuilder, MethodInfo executeMethod, MethodInfo canExecuteMethod, string commandName, bool isAsyncCommand) {
			bool hasParameter = executeMethod.GetParameters().Length == 1;
			bool isCommandMethodReturnTypeVoid = executeMethod.ReturnType == typeof(void);
			Type commandMethodReturnType = executeMethod.ReturnType;
			Type parameterType = hasParameter ? executeMethod.GetParameters()[0].ParameterType : null;
			Type commandPropertyType = isAsyncCommand ?
				hasParameter ? typeof(AsyncCommand<>).MakeGenericType(parameterType) : typeof(AsyncCommand)
				: hasParameter ? typeof(DelegateCommand<>).MakeGenericType(parameterType) : typeof(DelegateCommand);
			var commandField = typeBuilder.DefineField("_" + commandName, commandPropertyType, FieldAttributes.Private);
			MethodAttributes methodAttributes = MA.Public | MA.SpecialName | MA.HideBySig;
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("get_" + commandName, methodAttributes);
			Type executeType = (hasParameter ?
					(isCommandMethodReturnTypeVoid ? typeof(Action<>).MakeGenericType(parameterType) : typeof(Func<,>).MakeGenericType(parameterType, commandMethodReturnType)) :
					(isCommandMethodReturnTypeVoid ? typeof(Action) : typeof(Func<>).MakeGenericType(commandMethodReturnType)));
			Type canExecuteType = (hasParameter ?
					typeof(Func<,>).MakeGenericType(parameterType, typeof(bool)) :
					typeof(Func<bool>));
			Type factoryType = (hasParameter ?
				typeof(CommandFactory<>).MakeGenericType(parameterType) : typeof(CommandFactory));
			MethodInfo factoryMethod = factoryType
				.GetMethod("Create", BF.Static | BF.Public, null, new Type[] { executeType, canExecuteType }, null);
			ConstructorInfo executeDelegateConstructor = executeType
				.GetConstructor(BF.Instance | BF.Public, null, SimpleDelegateTypes, null);
			ConstructorInfo canExecuteDelegateConstructor = canExecuteType
				.GetConstructor(BF.Instance | BF.Public, null, SimpleDelegateTypes, null);
			methodBuilder.SetReturnType(commandPropertyType);
			ILGenerator gen = methodBuilder.GetILGenerator();
			gen.DeclareLocal(commandPropertyType);
			Label returnLabel = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, commandField);
			gen.Emit(OpCodes.Brtrue_S, returnLabel);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldftn, executeMethod);
			gen.Emit(OpCodes.Newobj, executeDelegateConstructor);
			if(canExecuteMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldftn, canExecuteMethod);
				gen.Emit(OpCodes.Newobj, canExecuteDelegateConstructor);
			}
			else gen.Emit(OpCodes.Ldnull);
			gen.Emit(OpCodes.Call, factoryMethod);
			gen.Emit(OpCodes.Stfld, commandField);
			gen.MarkLabel(returnLabel);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, commandField);
			gen.Emit(OpCodes.Ret);
			return methodBuilder;
		}
		#endregion
		#region Parent ViewModel
		internal const string ParentViewModelPropertyName = "ParentViewModel";
		static void BuildParentViewModelProperty(Type type, TypeBuilder typeBuilder, MethodInfo raisePropertyChangedMethod) {
			var propertyInfo = type.GetProperty(ParentViewModelPropertyName, BF.Instance | BF.Public | BF.NonPublic);
			if(propertyInfo != null)
				return;
			FieldBuilder parentViewModelField = typeBuilder.DefineField("parentViewModel", typeof(object), FieldAttributes.Private);
			var getMethod = BuildGetParentViewModelMethod(typeBuilder, parentViewModelField);
			var setMethod = BuildSetParentViewModelMethod(typeBuilder, parentViewModelField,
				raisePropertyChangedMethod,
				GetPropertyChangedMethodCore(type, ParentViewModelPropertyName, typeof(object), "Changed"),
				GetPropertyChangedMethodCore(type, ParentViewModelPropertyName, typeof(object), "Changing"));
			var newProperty = typeBuilder.DefineProperty(ParentViewModelPropertyName, PropertyAttributes.None, typeof(object), new Type[0]);
			newProperty.SetGetMethod(getMethod);
			newProperty.SetSetMethod(setMethod);
		}
		static MethodBuilder BuildSetParentViewModelMethod(TypeBuilder type, FieldBuilder parentViewModelField, MethodInfo raisePropertyChangedMethod, MethodInfo propertyChangedMethod, MethodInfo propertyChangingMethod) {
			MethodBuilder method = type.DefineMethod("set_" + ParentViewModelPropertyName, MA.Family | MA.HideBySig);
			method.SetReturnType(typeof(void));
			method.SetParameters(typeof(object));
			ParameterBuilder value = method.DefineParameter(1, ParameterAttributes.None, "value");
			ILGenerator gen = method.GetILGenerator();
			bool useLocal = (propertyChangedMethod != null) && (propertyChangedMethod.GetParameters().Length == 1);
			if(useLocal)
				gen.DeclareLocal(typeof(object));
			Label returnLabel = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, parentViewModelField);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Beq_S, returnLabel);
			if(propertyChangingMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				if(propertyChangingMethod.GetParameters().Length == 1)
					gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Call, propertyChangingMethod);
			}
			if(useLocal) {
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, parentViewModelField);
				gen.Emit(OpCodes.Stloc_0);
			}
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Stfld, parentViewModelField);
			if(propertyChangedMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				if(propertyChangedMethod.GetParameters().Length == 1)
					gen.Emit(OpCodes.Ldloc_0);
				gen.Emit(OpCodes.Call, propertyChangedMethod);
			}
			if(raisePropertyChangedMethod != null) {
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldstr, ParentViewModelPropertyName);
				gen.Emit(OpCodes.Call, raisePropertyChangedMethod);
			}
			gen.MarkLabel(returnLabel);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		static MethodBuilder BuildGetParentViewModelMethod(TypeBuilder type, FieldBuilder parentViewModelField) {
			MethodBuilder method = type.DefineMethod("get_" + ParentViewModelPropertyName, MA.Family | MA.HideBySig);
			method.SetReturnType(typeof(object));
			ILGenerator gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, parentViewModelField);
			gen.Emit(OpCodes.Ret);
			return method;
		}
		#endregion
		#region Exceptions
		static bool Throw(string text, string memberName) {
			throw new POCOException(string.Format(text, memberName));
		}
		static bool Throw(string text, MemberInfo member) {
			throw new POCOException(string.Format(text, member.Name));
		}
		static bool Throw(string text, Type type) {
			throw new POCOException(string.Format(text, type.Name));
		}
		class POCOException : Exception {
			public POCOException(string message)
				: base(message) {
			}
		}
		const string CommandSuffix = "Command";
		const string CanExecutePrefix = "Can";
		const string Error_ConstructorExpressionCanOnlyBeOfNewExpressionType = "Constructor expression can only be of NewExpression type.";
		const string Error_TypeHasNoCtors = "Type has no accessible constructors: {0}.";
		const string Error_SealedClass = "Cannot create dynamic class for the sealed class: {0}.";
		const string Error_InternalClass = "Cannot create dynamic class for the internal class: {0}.";
		const string Error_RaisePropertyChangedMethodNotFound = "Class already supports INotifyPropertyChanged, but RaisePropertyChanged(string) method not found: {0}.";
		const string Error_MoreThanOnePropertyChangedMethod = "More than one property changed method: {0}.";
		const string Error_PropertyChangedMethodShouldBePublicOrProtected = "Property changed method should be public or protected: {0}.";
		const string Error_PropertyChangedCantHaveMoreThanOneParameter = "Property changed method cannot have more than one parameter: {0}.";
		const string Error_PropertyChangedCantHaveReturnType = "Property changed method cannot have return type: {0}.";
		const string Error_PropertyChangedMethodArgumentTypeShouldMatchPropertyType = "Property changed method argument type should match property type: {0}.";
		const string Error_MemberWithSameCommandNameAlreadyExists = "Member with the same command name already exists: {0}.";
		const string Error_CanExecuteMethodHasIncorrectParameters = "Can execute method has incorrect parameters: {0}.";
		const string Error_MethodShouldBePublic = "Method should be public: {0}.";
		const string Error_MethodNotFound = "Method not found: {0}.";
		#endregion Exceptions
	}
	public class PropertyChangedHelper {
		event PropertyChangedEventHandler PropertyChanged;
		public void AddHandler(PropertyChangedEventHandler handler) {
			PropertyChanged += handler;
		}
		public void RemoveHandler(PropertyChangedEventHandler handler) {
			PropertyChanged -= handler;
		}
		public void OnPropertyChanged(INotifyPropertyChanged obj, string propertyName) {
			var handler = PropertyChanged;
			if(handler != null) handler(obj, new PropertyChangedEventArgs(propertyName));
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using System.ComponentModel;
	using DevExpress.Utils.MVVM.Internal;
	using NUnit.Framework;
	#region Test Classes
	public class Obj1 {
		public string Name { get; set; }
	}
	public class Obj2 {
		public virtual string Name { get; set; }
	}
	public class Obj3 : Obj2 {
		internal int count;
		protected void OnNameChanged() { count++; }
	}
	public class Obj4 {
		public Obj4(int a, int b, int c) {
			Result = a + b + c;
		}
		public Obj4(float a, float b, float c) {
			Result = (int)((a + b + c) + 0.5f);
		}
		public int Result { get; private set; }
	}
	public class Obj5 {
		internal int executing;
		public void DoSomething() {
			executing++;
		}
	}
	public class Obj6 : Obj5 {
		internal bool canDo;
		public bool CanDoSomething() {
			return canDo;
		}
	}
	public class Obj5_Generic {
		internal int executing;
		public void DoSomething(int param) {
			executing += param;
		}
	}
	public class Obj6_Generic : Obj5_Generic {
		public bool CanDoSomething(int param) {
			return param != 0;
		}
	}
	public class Obj5_Async {
		internal int executing;
		public System.Threading.Tasks.Task DoSomething() {
			return System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(10); 
				executing++;
			});
		}
	}
	public class Obj6_Async {
		internal int executing;
		public System.Threading.Tasks.Task DoSomething(int n) {
			return System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				var typesResolver = POCOTypesResolver.Instance;
				object @this = this;
				Type commandType;
				string methodPath;
				var asyncCommand = CommandHelper.GetCommandCore<Obj6_Async>(typesResolver, x => x.DoSomething(0), ref @this, out commandType, out methodPath) as IAsyncCommand;
				for(int i = 0; i < n; i++) {
					if(asyncCommand.IsCancellationRequested)
						break;
					System.Threading.Thread.Sleep(10); 
					executing++;
				}
			});
		}
		public bool CanDoSomething(int param) {
			return param != 0;
		}
	}
	public class Obj7<T> {
		public virtual T Value { get; set; }
		internal int count;
		protected void OnValueChanged() { count++; }
	}
	public class Obj8 {
		protected object ParentViewModel { get; set; }
	}
	public class Obj9 {
		internal int changing, changed;
		protected void OnParentViewModelChanging() { changing++; }
		protected void OnParentViewModelChanged() { changed++; }
	}
	public class Obj10 {
		internal object value, prev;
		protected void OnParentViewModelChanging(object value) { this.value = value; }
		protected void OnParentViewModelChanged(object prev) { this.prev = prev; }
	}
	#endregion
	[TestFixture]
	public class POCOSourceTests {
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			POCOSource.Reset();
		}
		[Test]
		public void Test00_SimpleObject() {
			var obj = POCOSource.Create<Obj1>();
			Assert.IsNotNull(obj);
			Assert.AreEqual(obj.GetType().FullName, DynamicTypesHelper.GetDynamicTypeName(typeof(Obj1)));
			Assert.IsTrue(obj is INotifyPropertyChanged);
		}
		[Test]
		public void Test01_SimpleNotify() {
			Obj2 obj = POCOSource.Create<Obj2>();
			int count = 0;
			var handler = new PropertyChangedEventHandler((s, e) => count++);
			((INotifyPropertyChanged)obj).PropertyChanged += handler;
			Assert.AreEqual(0, count);
			obj.Name = "Test";
			Assert.AreEqual(1, count);
			obj.Name = "Test";
			Assert.AreEqual(1, count);
			((INotifyPropertyChanged)obj).PropertyChanged -= handler;
		}
		[Test]
		public void Test02_Changed() {
			Obj3 obj = POCOSource.Create<Obj3>();
			Assert.AreEqual(0, obj.count);
			obj.Name = "Test";
			Assert.AreEqual(1, obj.count);
			obj.Name = "Test";
			Assert.AreEqual(1, obj.count);
		}
		[Test]
		public void Test03_Factory() {
			Func<int, int, int, Obj4> f1 = POCOSource.Factory<Obj4, int, int, int>();
			Assert.AreEqual(0, f1(0, 0, 0).Result);
			Assert.AreEqual(6, f1(1, 2, 3).Result);
			Func<float, float, float, Obj4> f2 = POCOSource.Factory<Obj4, float, float, float>();
			Assert.AreEqual(0, f2(0, 0, 0).Result);
			Assert.AreEqual(1, f2(0.1f, 0.2f, 0.3f).Result);
		}
		[Test]
		public void Test04_GenericType() {
			Obj7<int> obj = POCOSource.Create<Obj7<int>>();
			Assert.IsNotNull(obj);
			Assert.AreEqual(obj.GetType().FullName, DynamicTypesHelper.GetDynamicTypeName(typeof(Obj7<int>)));
			obj.Value = 5;
			Assert.AreEqual(1, obj.count);
			obj.Value = 5;
			Assert.AreEqual(1, obj.count);
		}
		[Test]
		public void Test05_ParentViewModel() {
			var obj = POCOSource.Create<Obj1>();
			Assert.IsNotNull(obj);
			var pInfo = obj.GetType().GetProperty(POCOSource.ParentViewModelPropertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Assert.IsNotNull(pInfo);
			Assert.AreEqual(obj.GetType(), pInfo.DeclaringType);
		}
		[Test]
		public void Test05_ParentViewModel_Exist() {
			var obj = POCOSource.Create<Obj8>();
			Assert.IsNotNull(obj);
			var pInfo = obj.GetType().GetProperty(POCOSource.ParentViewModelPropertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Assert.IsNotNull(pInfo);
			Assert.AreEqual(typeof(Obj8), pInfo.DeclaringType);
		}
		[Test]
		public void Test05_ParentViewModel_Callbacks() {
			var obj = POCOSource.Create<Obj9>();
			Assert.IsNotNull(obj);
			var pInfo = obj.GetType().GetProperty(POCOSource.ParentViewModelPropertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Assert.IsNotNull(pInfo);
			Assert.AreEqual(obj.GetType(), pInfo.DeclaringType);
			pInfo.SetValue(obj, null, new object[] { });
			Assert.AreEqual(0, obj.changing);
			Assert.AreEqual(0, obj.changed);
			pInfo.SetValue(obj, "Test", new object[] { });
			Assert.AreEqual(1, obj.changing);
			Assert.AreEqual(1, obj.changed);
		}
		[Test]
		public void Test05_ParentViewModel_CallbacksParametrized() {
			var obj = POCOSource.Create<Obj10>();
			Assert.IsNotNull(obj);
			var pInfo = obj.GetType().GetProperty(POCOSource.ParentViewModelPropertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Assert.AreEqual(null, obj.value);
			Assert.AreEqual(null, obj.prev);
			pInfo.SetValue(obj, "Test", new object[] { });
			Assert.AreEqual("Test", obj.value);
			Assert.AreEqual(null, obj.prev);
			pInfo.SetValue(obj, null, new object[] { });
			Assert.AreEqual(null, obj.value);
			Assert.AreEqual("Test", obj.prev);
		}
	}
	[TestFixture]
	public class POCOSourceTests_Commands {
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			POCOSource.Reset();
		}
		[Test]
		public void Test00_SimpleCommand() {
			Obj5 obj = POCOSource.Create<Obj5>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(DelegateCommand), prop.PropertyType);
			var command = prop.GetValue(obj) as DelegateCommand;
			Assert.IsNotNull(command);
			Assert.IsTrue(command.CanExecute(null));
			command.Execute(null);
			Assert.AreEqual(1, obj.executing);
		}
		[Test]
		public void Test00_SimpleCommand_CanDo() {
			Obj6 obj = POCOSource.Create<Obj6>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(DelegateCommand), prop.PropertyType);
			var command = prop.GetValue(obj) as DelegateCommand;
			Assert.IsNotNull(command);
			Assert.IsFalse(command.CanExecute(null));
			obj.canDo = true;
			Assert.IsTrue(command.CanExecute(null));
			command.Execute(null);
			Assert.AreEqual(1, obj.executing);
		}
		[Test]
		public void Test01_GenericCommand() {
			Obj5_Generic obj = POCOSource.Create<Obj5_Generic>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(DelegateCommand<int>), prop.PropertyType);
			var command = prop.GetValue(obj) as DelegateCommand<int>;
			Assert.IsNotNull(command);
			Assert.IsTrue(command.CanExecute(0));
			command.Execute(5);
			Assert.AreEqual(5, obj.executing);
		}
		[Test]
		public void Test01_GenericCommand_CanDo() {
			Obj6_Generic obj = POCOSource.Create<Obj6_Generic>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(DelegateCommand<int>), prop.PropertyType);
			var command = prop.GetValue(obj) as DelegateCommand<int>;
			Assert.IsNotNull(command);
			Assert.IsFalse(command.CanExecute(0));
			Assert.IsTrue(command.CanExecute(5));
			command.Execute(5);
			Assert.AreEqual(5, obj.executing);
		}
		[Test, Explicit]
		public void Test02_AsyncCommand() {
			Obj5_Async obj = POCOSource.Create<Obj5_Async>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(AsyncCommand), prop.PropertyType);
			var command = prop.GetValue(obj) as AsyncCommand;
			Assert.IsNotNull(command);
			Assert.IsTrue(command.CanExecute(null));
			Assert.IsFalse(command.IsExecuting);
			command.Execute(null);
			Assert.AreEqual(0, obj.executing);
			Assert.IsTrue(command.IsExecuting);
			Assert.IsFalse(command.CanExecute(null));
			command.Wait(TimeSpan.FromMilliseconds(-1));
			Assert.AreEqual(1, obj.executing);
			Assert.IsFalse(command.IsExecuting);
		}
		[Test, Explicit]
		public void Test02_AsyncCommand_Cancelation() {
			Obj6_Async obj = POCOSource.Create<Obj6_Async>();
			Assert.AreEqual(0, obj.executing);
			var prop = TypeDescriptor.GetProperties(obj)["DoSomethingCommand"];
			Assert.IsNotNull(prop);
			Assert.AreEqual(typeof(AsyncCommand<int>), prop.PropertyType);
			var command = prop.GetValue(obj) as AsyncCommand<int>;
			Assert.IsNotNull(command);
			Assert.IsFalse(command.CanExecute(0));
			Assert.IsTrue(command.CanExecute(10));
			Assert.IsFalse(command.IsExecuting);
			command.Execute(10);
			Assert.AreEqual(0, obj.executing);
			Assert.IsTrue(command.IsExecuting);
			Assert.IsFalse(command.CanExecute(10));
			command.Wait(TimeSpan.FromMilliseconds(50));
			command.Cancel();
			Assert.IsTrue(obj.executing > 0 && obj.executing < 10);
			command.Wait(TimeSpan.FromMilliseconds(-1));
			Assert.IsFalse(command.IsExecuting);
		}
	}
	[TestFixture]
	public class ViewModelSourceProxyTests_POCOSource : DevExpress.Utils.MVVM.Tests.ViewModelSourceProxyTests {
		protected override Type GetViewModelSourceType() {
			return typeof(POCOSource);
		}
		protected override void ResetViewModelSourceType() {
			POCOSource.Reset();
		}
	}
}
#endif
