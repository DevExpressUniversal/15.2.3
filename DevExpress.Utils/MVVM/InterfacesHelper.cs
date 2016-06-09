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
	static class InterfacesProxy {
		static IDictionary<Type, Func<object, object>> getParentViewModelCache = new Dictionary<Type, Func<object, object>>();
		internal static object GetParentViewModel(Type supportParentViewModelType, object viewModel) {
			return Member<Func<object, object>>(getParentViewModelCache, supportParentViewModelType, "ParentViewModel", MakeAccessor)(viewModel);
		}
		static IDictionary<Type, Action<object, object>> setParentViewModelCache = new Dictionary<Type, Action<object, object>>();
		internal static void SetParentViewModel(Type supportParentViewModelType, object viewModel, object parentViewModel) {
			Member<Action<object, object>>(setParentViewModelCache, supportParentViewModelType, "ParentViewModel", MakeMutator)(viewModel, parentViewModel);
		}
		static IDictionary<Type, Func<object, object>> getServiceContainerCache = new Dictionary<Type, Func<object, object>>();
		internal static object GetServiceContainer(Type supportServicesType, object viewModel) {
			return Member<Func<object, object>>(getServiceContainerCache, supportServicesType, "ServiceContainer", MakeAccessor)(viewModel);
		}
		static IDictionary<Type, Func<object>> getDefaultServiceContainerCache = new Dictionary<Type, Func<object>>();
		internal static object GetDefaultServiceContainer(Type defaultServiceContainerType) {
			return Member<Func<object>>(getDefaultServiceContainerCache, defaultServiceContainerType, "Default", MakeStaticAccessor)();
		}
		static IDictionary<Type, Action<object, object>> setParameterCache = new Dictionary<Type, Action<object, object>>();
		internal static void SetParameter(Type supportParameterType, object viewModel, object parameter) {
			Member<Action<object, object>>(setParameterCache, supportParameterType, "Parameter", MakeMutator)(viewModel, parameter);
		}
		static IDictionary<Type, Action<object, object>> setDocumentOwnerCache = new Dictionary<Type, Action<object, object>>();
		internal static void SetDocumentOwner(Type documentContentType, Type documentOwnerType, object viewModel, object documentOwner) {
			Member<Action<object, object>>(setDocumentOwnerCache, documentContentType, documentOwnerType, "DocumentOwner", MakeMutator)(viewModel, documentOwner);
		}
		static IDictionary<Type, Func<object, object>> getTitleCache = new Dictionary<Type, Func<object, object>>();
		internal static object GetTitle(Type documentContentType, object documentContent) {
			return Member<Func<object, object>>(getTitleCache, documentContentType, "Title", MakeAccessor)(documentContent);
		}
		static IDictionary<Type, Func<object, object>> getTagCache = new Dictionary<Type, Func<object, object>>();
		internal static object GetUICommandTag(Type uiCommandType, object command) {
			return Member<Func<object, object>>(getTagCache, uiCommandType, "Tag", MakeAccessor)(command);
		}
		static IDictionary<Type, Action<bool>> setDefaultUseCommandManagerCache = new Dictionary<Type, Action<bool>>();
		internal static void SetDefaultUseCommandManager(Type commandBaseType, bool value) {
			Member<Action<bool>>(setDefaultUseCommandManagerCache, commandBaseType, "DefaultUseCommandManager", MakeStaticMutator<bool>)(value);
		}
		static IDictionary<Type, Func<object, object>> getCancelCommandCache = new Dictionary<Type, Func<object, object>>();
		internal static object GetCancelCommand(Type asyncCommandType, object command) {
			return Member<Func<object, object>>(getCancelCommandCache, asyncCommandType, "CancelCommand", MakeAccessor)(command);
		}
		#region Attribute Properties
		internal static string GetAttributeName(Type attributeType, object attribute) {
			return (string)GetAttributeProperty(attributeType, attribute, "Name");
		}
		internal static bool GetAttributeIsCommand(Type attributeType, object attribute) {
			return (bool)GetAttributeProperty(attributeType, attribute, "IsCommand");
		}
		internal static bool GetAttributeIsBindable(Type attributeType, object attribute) {
			return (bool)GetAttributeProperty(attributeType, attribute, "IsBindable");
		}
		internal static string GetAttributeCommandParameter(Type attributeType, object attribute) {
			return (string)GetAttributeProperty(attributeType, attribute, "CommandParameter");
		}
		static IDictionary<string, Func<object, object>> getAttributePropertyCache = new Dictionary<string, Func<object, object>>();
		internal static object GetAttributeProperty(Type attributeType, object attribute, string propertyName) {
			return Member<Func<object, object>>(getAttributePropertyCache, attributeType, propertyName, MakeAccessorChecked)(attribute);
		}
		#endregion Attribute Properties
		static IDictionary<string, Func<object, object>> getAccessorsCache = new Dictionary<string, Func<object, object>>();
		internal static Func<object, object> GetAccessor(Type sourceType, string propertyName) {
			return Member<Func<object, object>>(getAccessorsCache, sourceType, propertyName, MakeAccessorChecked);
		}
		static IDictionary<string, Func<MVVMContext, object, string, IPropertyBinding>> setBindingCache = new Dictionary<string, Func<MVVMContext, object, string, IPropertyBinding>>();
		internal static Func<MVVMContext, object, string, IPropertyBinding> GetSetBindingMethod(Type targetType, Type bindingMemberType, string bindingMember) {
			string key = targetType.Name + "." + bindingMember + ":" + bindingMemberType.Name;
			Func<MVVMContext, object, string, IPropertyBinding> setBinding;
			if(!setBindingCache.TryGetValue(key, out setBinding)) {
				var mInfo = GetSetBindingMethodInfo(targetType, bindingMemberType);
				setBinding = CreateSetBindingMethod(mInfo, targetType, bindingMemberType, bindingMember);
			}
			return setBinding;
		}
		static Func<MVVMContext, object, string, IPropertyBinding> CreateSetBindingMethod(MethodInfo mInfo, Type targetType, Type bindingMemberType, string bindingMember) {
			var dExp = Expression.Parameter(targetType, "d");
			var selectorExpression = Expression.Lambda(
				Expression.MakeMemberAccess(dExp, targetType.GetProperty(bindingMember)), dExp);
			var context = Expression.Parameter(typeof(MVVMContext), "context");
			var dest = Expression.Parameter(typeof(object), "dest");
			var propertyName = Expression.Parameter(typeof(string), "propertyName");
			var call = Expression.Call(context, mInfo,
				Expression.Convert(dest, targetType), selectorExpression, propertyName);
			return Expression.Lambda<Func<MVVMContext, object, string, IPropertyBinding>>(
				call, context, dest, propertyName).Compile();
		}
		static MethodInfo setBindingMethodInfo;
		static MethodInfo GetSetBindingMethodInfo(Type targetType, Type bindingMemberType) {
			if(setBindingMethodInfo == null)
				setBindingMethodInfo = typeof(MVVMContext).GetMember("SetBinding", MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance)[0] as MethodInfo;
			return setBindingMethodInfo.MakeGenericMethod(targetType, bindingMemberType);
		}
		static IDictionary<Type, Func<MVVMContext, object, object, object[], IDisposable>> attachBehaviorCache = new Dictionary<Type, Func<MVVMContext, object, object, object[], IDisposable>>();
		internal static Func<MVVMContext, object, object, object[], IDisposable> GetAttachBehaviorMethod(Type behaviorType) {
			Func<MVVMContext, object, object, object[], IDisposable> attachBehavior;
			if(!attachBehaviorCache.TryGetValue(behaviorType, out attachBehavior)) {
				var mInfo = GetAttachBehaviorMethodInfo(behaviorType);
				attachBehavior = CreateAttachBehaviorMethod(mInfo, behaviorType);
			}
			return attachBehavior;
		}
		static Func<MVVMContext, object, object, object[], IDisposable> CreateAttachBehaviorMethod(MethodInfo mInfo, Type behaviorType) {
			var context = Expression.Parameter(typeof(MVVMContext), "context");
			var source = Expression.Parameter(typeof(object), "source");
			var settings = Expression.Parameter(typeof(object), "settings");
			var parameters = Expression.Parameter(typeof(object[]), "parameters");
			var call = Expression.Call(context, mInfo,
				source, Expression.Convert(settings, typeof(Action<>).MakeGenericType(behaviorType)), parameters);
			return Expression.Lambda<Func<MVVMContext, object, object, object[], IDisposable>>(
				call, context, source, settings, parameters).Compile();
		}
		static MethodInfo attachBehaviorMethodInfo;
		static MethodInfo GetAttachBehaviorMethodInfo(Type behaviorType) {
			if(attachBehaviorMethodInfo == null)
				attachBehaviorMethodInfo = typeof(MVVMContext).GetMember("AttachBehavior", MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance)[1] as MethodInfo;
			return attachBehaviorMethodInfo.MakeGenericMethod(behaviorType);
		}
		#region Member(Accessor & Mutator)
		static TMember Member<TMember>(IDictionary<string, TMember> cache, Type type, string memberName, Func<Type, string, TMember> makeFunc) {
			string key = type.Name + "." + memberName;
			TMember member;
			if(!cache.TryGetValue(key, out member)) {
				member = makeFunc(type, memberName);
				cache.Add(key, member);
			}
			return member;
		}
		static TMember Member<TMember>(IDictionary<Type, TMember> cache, Type type, string memberName, Func<Type, string, TMember> makeFunc) {
			TMember member;
			if(!cache.TryGetValue(type, out member)) {
				member = makeFunc(type, memberName);
				cache.Add(type, member);
			}
			return member;
		}
		static TMember Member<TMember>(IDictionary<Type, TMember> cache, Type type, Type memberType, string memberName, Func<Type, Type, string, TMember> makeFunc) {
			TMember member;
			if(!cache.TryGetValue(type, out member)) {
				member = makeFunc(type, memberType, memberName);
				cache.Add(type, member);
			}
			return member;
		}
		static Type GetMemberType(MemberInfo mInfo) {
			PropertyInfo pInfo = mInfo as PropertyInfo;
			if(pInfo != null)
				return pInfo.PropertyType;
			FieldInfo fInfo = mInfo as FieldInfo;
			if(fInfo != null)
				return fInfo.FieldType;
			throw new NotSupportedException("MemberInfo");
		}
		static MemberInfo GetMemberInfo(Type type, string memberName) {
			return (MemberInfo)type.GetProperty(memberName) ??
				(MemberInfo)type.GetField(memberName);
		}
		static MemberInfo GetStaticMemberInfo(Type type, string memberName) {
			return (MemberInfo)type.GetProperty(memberName, BF.Static | BF.Public) ??
				(MemberInfo)type.GetField(memberName, BF.Static | BF.Public);
		}
		static Func<object> MakeStaticAccessor(Type type, string memberName) {
			var mInfo = GetStaticMemberInfo(type, memberName);
			return Expression.Lambda<Func<object>>(Expression.MakeMemberAccess(null, mInfo)).Compile();
		}
		static Action<TValue> MakeStaticMutator<TValue>(Type type, string memberName) {
			var mInfo = GetStaticMemberInfo(type, memberName);
			var value = Expression.Parameter(typeof(TValue), "value");
			var assign = Expression.Assign(Expression.MakeMemberAccess(null, mInfo), value);
			return Expression.Lambda<Action<TValue>>(assign, value).Compile();
		}
		static Func<object, object> MakeAccessorChecked(Type type, string memberName) {
			var mInfo = GetMemberInfo(type, memberName);
			ParameterExpression parameter;
			var accessor = CreateAccessor(type, mInfo, out parameter);
			return Expression.Lambda<Func<object, object>>(CheckAccessor(mInfo, accessor), parameter).Compile();
		}
		static Func<object, object> MakeAccessor(Type type, string memberName) {
			ParameterExpression parameter;
			var accessor = CreateAccessor(type, GetMemberInfo(type, memberName), out parameter);
			return Expression.Lambda<Func<object, object>>(accessor, parameter).Compile();
		}
		static Expression CreateAccessor(Type type, MemberInfo mInfo, out ParameterExpression parameter) {
			parameter = Expression.Parameter(typeof(object), "instance");
			var instance = Expression.Convert(parameter, type);
			return Expression.MakeMemberAccess(instance, mInfo);
		}
		static Expression CheckAccessor(MemberInfo mInfo, Expression accessor) {
			Type mType = GetMemberType(mInfo);
			if(mType.IsValueType)
				accessor = Expression.Convert(accessor, typeof(object));
			return accessor;
		}
		static Action<object, object> MakeMutator(Type type, string memberName) {
			var mInfo = GetMemberInfo(type, memberName);
			var parameter = Expression.Parameter(typeof(object), "instance");
			var instance = Expression.Convert(parameter, type);
			var value = Expression.Parameter(typeof(object), "value");
			var assign = Expression.Assign(
				Expression.MakeMemberAccess(instance, mInfo), value);
			return Expression.Lambda<Action<object, object>>(assign, parameter, value).Compile();
		}
		static Action<object, object> MakeMutator(Type type, Type valueType, string memberName) {
			var mInfo = GetMemberInfo(type, memberName);
			var parameter = Expression.Parameter(typeof(object), "instance");
			var instance = Expression.Convert(parameter, type);
			var value = Expression.Parameter(typeof(object), "value");
			var assign = Expression.Assign(
				Expression.MakeMemberAccess(instance, mInfo), Expression.Convert(value, valueType));
			return Expression.Lambda<Action<object, object>>(assign, parameter, value).Compile();
		}
		#endregion Member(Accessor & Mutator)
		static IDictionary<Type, IEnumerable<IGetServiceProxy>> getServiceCache = new Dictionary<Type, IEnumerable<IGetServiceProxy>>();
		internal static TService GetService<TService>(Type serviceContainerType, object serviceContainer, params object[] parameters) where TService : class {
			IEnumerable<IGetServiceProxy> proxies;
			if(!getServiceCache.TryGetValue(serviceContainerType, out proxies)) {
				var getServiceMethods = serviceContainerType.GetMember("GetService", System.Reflection.MemberTypes.Method, BF.Public | BF.Instance);
				List<IGetServiceProxy> proxiesList = new List<IGetServiceProxy>(getServiceMethods.Length);
				for(int i = 0; i < getServiceMethods.Length; i++) {
					MethodInfo mInfo = getServiceMethods[i] as MethodInfo;
					proxiesList.Add(new GetServiceProxy(mInfo, serviceContainerType, mInfo.GetParameters()));
				}
				proxies = proxiesList;
				getServiceCache.Add(serviceContainerType, proxies);
			}
			return TryGetService<TService>(serviceContainer, parameters, proxies);
		}
		static TService TryGetService<TService>(object serviceContainer, object[] parameters, IEnumerable<IGetServiceProxy> proxies) where TService : class {
			while(true) {
				var proxy = proxies.FirstOrDefault(p => p.Match(parameters));
				if(proxy != null)
					return proxy.GetService<TService>(serviceContainer, parameters);
				if(parameters.Length != 0)
					parameters = ProxyBase.Reduce(parameters);
				else return null;
			}
		}
		static IDictionary<Type, IEnumerable<IRegisterServiceProxy>> registerServiceCache = new Dictionary<Type, IEnumerable<IRegisterServiceProxy>>();
		internal static void RegisterService(Type serviceContainerType, object serviceContainer, params object[] parameters) {
			IEnumerable<IRegisterServiceProxy> proxies;
			if(!registerServiceCache.TryGetValue(serviceContainerType, out proxies)) {
				var registerServiceMethods = serviceContainerType.GetMember("RegisterService", System.Reflection.MemberTypes.Method, BF.Public | BF.Instance);
				List<IRegisterServiceProxy> proxiesList = new List<IRegisterServiceProxy>(registerServiceMethods.Length);
				for(int i = 0; i < registerServiceMethods.Length; i++) {
					MethodInfo mInfo = registerServiceMethods[i] as MethodInfo;
					proxiesList.Add(new RegisterServiceProxy(mInfo, serviceContainerType, mInfo.GetParameters()));
				}
				proxies = proxiesList;
				registerServiceCache.Add(serviceContainerType, proxies);
			}
			TryRegisterService(serviceContainer, parameters, proxies);
		}
		static void TryRegisterService(object serviceContainer, object[] parameters, IEnumerable<IRegisterServiceProxy> proxies) {
			while(true) {
				var proxy = proxies.FirstOrDefault(p => p.Match(parameters));
				if(proxy != null) {
					proxy.RegisterService(serviceContainer, parameters);
					break;
				}
				if(parameters.Length != 0)
					parameters = ProxyBase.Reduce(parameters);
				else break;
			}
		}
		static IDictionary<Type, IUnregisterServiceProxy> unregisterServiceCache = new Dictionary<Type, IUnregisterServiceProxy>();
		internal static void UnregisterService(Type serviceContainerType, object serviceContainer, object service) {
			IUnregisterServiceProxy proxy;
			if(!unregisterServiceCache.TryGetValue(serviceContainerType, out proxy)) {
				var unregisterServiceMethod = serviceContainerType.GetMethod("UnregisterService", new Type[] { typeof(object) });
				if(unregisterServiceMethod != null) {
					proxy = new UnregisterServiceProxy(unregisterServiceMethod, serviceContainerType);
					unregisterServiceCache.Add(serviceContainerType, proxy);
				}
			}
			if(proxy != null) proxy.UnregisterService(serviceContainer, service);
		}
		#region  GetService proxy
		interface IGetServiceProxy {
			TService GetService<TService>(object serviceContainer, params object[] parameters) where TService : class;
			bool Match(params object[] parameters);
		}
		sealed class GetServiceProxy : ProxyBase, IGetServiceProxy {
			MethodInfo mInfo;
			Type serviceContainerType;
			IDictionary<Type, Func<object, object[], object>> getServiceCache = new Dictionary<Type, Func<object, object[], object>>();
			public GetServiceProxy(MethodInfo mInfo, Type serviceContainerType, ParameterInfo[] mInfoParameters)
				: base(mInfoParameters) {
				this.mInfo = mInfo;
				this.serviceContainerType = serviceContainerType;
			}
			TService IGetServiceProxy.GetService<TService>(object serviceContainer, params object[] parameters) {
				Func<object, object[], object> getService;
				if(!getServiceCache.TryGetValue(typeof(TService), out getService)) {
					var pServiceContainer = Expression.Parameter(typeof(object), "serviceContainer");
					var instance = Expression.Convert(pServiceContainer, serviceContainerType);
					var paramsExpression = Expression.Parameter(typeof(object[]), "parameters");
					var call = Expression.Call(instance, mInfo.MakeGenericMethod(typeof(TService)),
						CreateCallParameters(paramsExpression));
					getService = Expression.Lambda<Func<object, object[], object>>(
									call, pServiceContainer, paramsExpression
								).Compile();
					getServiceCache.Add(typeof(TService), getService);
				}
				return getService(serviceContainer, parameters) as TService;
			}
			bool IGetServiceProxy.Match(params object[] parameters) {
				return MatchCore(parameters);
			}
		}
		#endregion  GetService proxy
		#region RegisterService proxy
		interface IRegisterServiceProxy {
			void RegisterService(object serviceContainer, params object[] parameters);
			bool Match(params object[] parameters);
		}
		sealed class RegisterServiceProxy : ProxyBase, IRegisterServiceProxy {
			Action<object, object[]> registerService;
			Expression<Action<object, object[]>> expression;
			public RegisterServiceProxy(MethodInfo mInfo, Type serviceContainerType, ParameterInfo[] mInfoParameters)
				: base(mInfoParameters) {
				var pServiceContainer = Expression.Parameter(typeof(object), "serviceContainer");
				var instance = Expression.Convert(pServiceContainer, serviceContainerType);
				var paramsExpression = Expression.Parameter(typeof(object[]), "parameters");
				var call = Expression.Call(instance, mInfo,
					CreateCallParameters(paramsExpression));
				expression = Expression.Lambda<Action<object, object[]>>(
								call, pServiceContainer, paramsExpression
							);
			}
			void IRegisterServiceProxy.RegisterService(object serviceContainer, params object[] parameters) {
				if(registerService == null)
					registerService = expression.Compile();
				registerService(serviceContainer, parameters);
			}
			bool IRegisterServiceProxy.Match(params object[] parameters) {
				return MatchCore(parameters);
			}
		}
		#endregion RegisterService proxy
		#region UnregisterService proxy
		interface IUnregisterServiceProxy {
			void UnregisterService(object serviceContainer, object service);
		}
		sealed class UnregisterServiceProxy : IUnregisterServiceProxy {
			Action<object, object> unregisterService;
			public UnregisterServiceProxy(MethodInfo mInfo, Type serviceContainerType) {
				var pServiceContainer = Expression.Parameter(typeof(object), "serviceContainer");
				var instance = Expression.Convert(pServiceContainer, serviceContainerType);
				var pService = Expression.Parameter(typeof(object), "service");
				unregisterService = Expression.Lambda<Action<object, object>>(
								Expression.Call(instance, mInfo, pService), pServiceContainer, pService).Compile();
			}
			void IUnregisterServiceProxy.UnregisterService(object serviceContainer, object service) {
				unregisterService(serviceContainer, service);
			}
		}
		#endregion
		static IDictionary<Type, IOnCloseProxy> onCloseCache = new Dictionary<Type, IOnCloseProxy>();
		internal static void OnClose(Type documentContentType, object documentContent, System.ComponentModel.CancelEventArgs e) {
			IOnCloseProxy proxy;
			if(!onCloseCache.TryGetValue(documentContentType, out proxy)) {
				var onCloseMethod = documentContentType.GetMethod("OnClose", new Type[] { typeof(System.ComponentModel.CancelEventArgs) });
				if(onCloseMethod != null) {
					proxy = new OnCloseProxy(onCloseMethod, documentContentType);
					onCloseCache.Add(documentContentType, proxy);
				}
			}
			if(proxy != null) proxy.OnClose(documentContent, e);
		}
		static IDictionary<Type, IOnDestroyProxy> onDestroyCache = new Dictionary<Type, IOnDestroyProxy>();
		internal static void OnDestroy(Type documentContentType, object documentContent) {
			IOnDestroyProxy proxy;
			if(!onDestroyCache.TryGetValue(documentContentType, out proxy)) {
				var onDestroyMethod = documentContentType.GetMethod("OnDestroy", Type.EmptyTypes);
				if(onDestroyMethod != null) {
					proxy = new OnDestroyProxy(onDestroyMethod, documentContentType);
					onDestroyCache.Add(documentContentType, proxy);
				}
			}
			if(proxy != null) proxy.OnDestroy(documentContent);
		}
		#region  OnClose proxy
		interface IOnCloseProxy {
			void OnClose(object documentContent, System.ComponentModel.CancelEventArgs e);
		}
		sealed class OnCloseProxy : IOnCloseProxy {
			Action<object, System.ComponentModel.CancelEventArgs> onClose;
			public OnCloseProxy(MethodInfo mInfo, Type documentContentType) {
				var pDocumentContent = Expression.Parameter(typeof(object), "documentContent");
				var instance = Expression.Convert(pDocumentContent, documentContentType);
				var argsExpression = Expression.Parameter(typeof(System.ComponentModel.CancelEventArgs), "e");
				onClose = Expression.Lambda<Action<object, System.ComponentModel.CancelEventArgs>>(
								Expression.Call(instance, mInfo, argsExpression), pDocumentContent, argsExpression).Compile();
			}
			void IOnCloseProxy.OnClose(object documentContent, System.ComponentModel.CancelEventArgs e) {
				onClose(documentContent, e);
			}
		}
		#endregion  OnClose proxy
		#region  OnDestroy proxy
		interface IOnDestroyProxy {
			void OnDestroy(object documentContent);
		}
		sealed class OnDestroyProxy : IOnDestroyProxy {
			Action<object> onDestroy;
			public OnDestroyProxy(MethodInfo mInfo, Type documentContentType) {
				var pDocumentContent = Expression.Parameter(typeof(object), "documentContent");
				var instance = Expression.Convert(pDocumentContent, documentContentType);
				onDestroy = Expression.Lambda<Action<object>>(
								Expression.Call(instance, mInfo), pDocumentContent).Compile();
			}
			void IOnDestroyProxy.OnDestroy(object documentContent) {
				onDestroy(documentContent);
			}
		}
		#endregion  OnDestroy proxy
		static IDictionary<Type, IGetExternalAndFluentAPIAttributesProxy> getExternalAndFluentAPIAttributesCache = new Dictionary<Type, IGetExternalAndFluentAPIAttributesProxy>();
		internal static IEnumerable<Attribute> GetExternalAndFluentAPIAttributes(Type metadataHelperType, Type componentType, string memberName) {
			IGetExternalAndFluentAPIAttributesProxy proxy;
			if(!getExternalAndFluentAPIAttributesCache.TryGetValue(metadataHelperType, out proxy)) {
				var getMethod = metadataHelperType.GetMethod("GetExternalAndFluentAPIAttrbutes", new Type[] { typeof(Type), typeof(string) }) ?? 
					metadataHelperType.GetMethod("GetExternalAndFluentAPIAttributes", new Type[] { typeof(Type), typeof(string) }) ??
					metadataHelperType.GetMethod("GetExtenalAndFluentAPIAttrbutes", new Type[] { typeof(Type), typeof(string) }) ??
					metadataHelperType.GetMethod("GetExtenalAndFluentAPIAttributes", new Type[] { typeof(Type), typeof(string) });
				if(getMethod != null) {
					proxy = new GetExternalAndFluentAPIAttributesProxy(getMethod, metadataHelperType);
					getExternalAndFluentAPIAttributesCache.Add(metadataHelperType, proxy);
				}
			}
			return (proxy != null) ? proxy.Get(componentType, memberName) : new Attribute[0];
		}
		#region  GetExtenalAndFluentAPIAttributes proxy
		interface IGetExternalAndFluentAPIAttributesProxy {
			IEnumerable<Attribute> Get(Type componentType, string memberName);
		}
		sealed class GetExternalAndFluentAPIAttributesProxy : IGetExternalAndFluentAPIAttributesProxy {
			Func<Type, string, IEnumerable<Attribute>> get;
			public GetExternalAndFluentAPIAttributesProxy(MethodInfo mInfo, Type metadataHelperType) {
				var componentType = Expression.Parameter(typeof(Type), "componentType");
				var memberName = Expression.Parameter(typeof(string), "memberName");
				get = Expression.Lambda<Func<Type, string, IEnumerable<Attribute>>>(
								Expression.Call(mInfo, componentType, memberName), componentType, memberName).Compile();
			}
			IEnumerable<Attribute> IGetExternalAndFluentAPIAttributesProxy.Get(Type componentType, string memberName) {
				return get(componentType, memberName);
			}
		}
		#endregion  GetExtenalAndFluentAPIAttributes proxy
		internal static void Reset() {
			getParentViewModelCache.Clear();
			setParentViewModelCache.Clear();
			getServiceContainerCache.Clear();
			getDefaultServiceContainerCache.Clear();
			setParameterCache.Clear();
			setDocumentOwnerCache.Clear();
			getServiceCache.Clear();
			registerServiceCache.Clear();
			unregisterServiceCache.Clear();
			onCloseCache.Clear();
			onDestroyCache.Clear();
			getTagCache.Clear();
			setDefaultUseCommandManagerCache.Clear();
			getCancelCommandCache.Clear();
			getAttributePropertyCache.Clear();
			getExternalAndFluentAPIAttributesCache.Clear();
			getAccessorsCache.Clear();
			setBindingCache.Clear();
			attachBehaviorCache.Clear();
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System.Collections.Generic;
	using NUnit.Framework;
	#region Test Classes
	interface ISupportParentViewModelForTests {
		object ParentViewModel { get; set; }
	}
	interface ISupportServicesForTests {
		object ServiceContainer { get; }
	}
	enum SearchModeForTests { Default, Local }
	interface IServiceContainerForTests {
		T GetService<T>(SearchModeForTests mode = SearchModeForTests.Default) where T : class;
		T GetService<T>(string key, SearchModeForTests mode = SearchModeForTests.Default) where T : class;
		void RegisterService(object service, bool yieldToParent = false);
		void RegisterService(string key, object service, bool yieldToParent = false);
		void UnregisterService(object service);
	}
	interface IFoo { }
	interface IBar { }
	class FooEx : Foo, IServiceContainerForTests, IFoo {
		object service;
		IDictionary<string, object> services = new Dictionary<string, object>();
		T IServiceContainerForTests.GetService<T>(SearchModeForTests mode) {
			return (service as T) ?? this as T;
		}
		T IServiceContainerForTests.GetService<T>(string key, SearchModeForTests mode) {
			object s;
			return services.TryGetValue(key, out s) ? (T)s : ((service as T) ?? this as T);
		}
		void IServiceContainerForTests.RegisterService(object service, bool yieldToParent) {
			this.service = service;
		}
		void IServiceContainerForTests.RegisterService(string key, object service, bool yieldToParent) {
			services[key] = service;
		}
		void IServiceContainerForTests.UnregisterService(object service) {
			if(this.service == service) this.service = null;
		}
	}
	class BarEx : Bar, ISupportParentViewModelForTests, ISupportServicesForTests, IBar {
		public BarEx(object parent) {
			this.parent = parent;
		}
		object parent;
		object ISupportParentViewModelForTests.ParentViewModel {
			get { return parent; }
			set { parent = value; }
		}
		object ISupportServicesForTests.ServiceContainer {
			get { return parent; }
		}
	}
	interface IDocumentOwnerForTests { }
	interface IDocumentContentForTests {
		IDocumentOwnerForTests DocumentOwner { get; set; }
		void OnClose(System.ComponentModel.CancelEventArgs e);
		void OnDestroy();
		object Title { get; }
	}
	class DocumentOwnerForTest : IDocumentOwnerForTests { }
	class DocumentContentForTests : IDocumentContentForTests {
		IDocumentOwnerForTests IDocumentContentForTests.DocumentOwner { get; set; }
		internal int closeCount;
		void IDocumentContentForTests.OnClose(System.ComponentModel.CancelEventArgs e) {
			if(!e.Cancel) closeCount++;
		}
		internal int destroyCount;
		void IDocumentContentForTests.OnDestroy() { destroyCount++; }
		object IDocumentContentForTests.Title { get { return titleCore; } }
		internal string titleCore;
	}
	class UICommandForTests {
		public object Tag { get; set; }
	}
	#endregion
	[TestFixture]
	public class InterfacesProxyTest_ViewModelInterfaces {
		Foo foo;
		Bar bar;
		[TestFixtureSetUp]
		public void FixtureSetUp() {
			foo = new FooEx();
			bar = new BarEx(foo);
		}
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			InterfacesProxy.Reset();
		}
		[Test]
		public void Test00_GetParentViewModel() {
			Assert.AreEqual(foo, InterfacesProxy.GetParentViewModel(typeof(ISupportParentViewModelForTests), bar));
		}
		[Test]
		public void Test00_SetParentViewModel() {
			Bar bar = new BarEx(null);
			Assert.IsNull(InterfacesProxy.GetParentViewModel(typeof(ISupportParentViewModelForTests), bar));
			Foo foo = new Foo();
			InterfacesProxy.SetParentViewModel(typeof(ISupportParentViewModelForTests), bar, foo);
			Assert.AreEqual(foo, InterfacesProxy.GetParentViewModel(typeof(ISupportParentViewModelForTests), bar));
		}
		[Test]
		public void Test01_GetServiceContainer() {
			Assert.AreEqual(foo, InterfacesProxy.GetServiceContainer(typeof(ISupportServicesForTests), bar));
		}
		[Test]
		public void Test02_GetService() {
			Assert.AreEqual(foo, InterfacesProxy.GetService<IFoo>(typeof(IServiceContainerForTests), foo));
		}
		[Test]
		public void Test02_GetService_Key() {
			Assert.AreEqual(foo, InterfacesProxy.GetService<IFoo>(typeof(IServiceContainerForTests), foo, "foo"));
		}
		[Test]
		public void Test03_RegisterService() {
			InterfacesProxy.RegisterService(typeof(IServiceContainerForTests), foo, bar);
			Assert.AreEqual(bar, InterfacesProxy.GetService<IBar>(typeof(IServiceContainerForTests), foo));
			Assert.AreEqual(foo, InterfacesProxy.GetService<IFoo>(typeof(IServiceContainerForTests), foo));
		}
		[Test]
		public void Test03_RegisterService_Key() {
			InterfacesProxy.RegisterService(typeof(IServiceContainerForTests), foo, "bar", bar);
			Assert.AreEqual(bar, InterfacesProxy.GetService<IBar>(typeof(IServiceContainerForTests), foo, "bar"));
			Assert.AreEqual(foo, InterfacesProxy.GetService<IFoo>(typeof(IServiceContainerForTests), foo, "foo"));
		}
		[Test]
		public void Test04_UnregisterService() {
			InterfacesProxy.RegisterService(typeof(IServiceContainerForTests), foo, bar);
			Assert.AreEqual(bar, InterfacesProxy.GetService<IBar>(typeof(IServiceContainerForTests), foo));
			InterfacesProxy.UnregisterService(typeof(IServiceContainerForTests), foo, bar);
			Assert.IsNull(InterfacesProxy.GetService<IBar>(typeof(IServiceContainerForTests), foo));
		}
	}
	[TestFixture]
	public class InterfacesProxyTests_DocumentInterfaces {
		DocumentContentForTests documentContent;
		DocumentOwnerForTest documentOwner;
		[TestFixtureSetUp]
		public void FixtureSetUp() {
			documentContent = new DocumentContentForTests();
			documentOwner = new DocumentOwnerForTest();
		}
		[TestFixtureTearDown]
		public void FixtureTearDown() {
			InterfacesProxy.Reset();
		}
		[Test]
		public void Test04_SetDocumentOwner() {
			IDocumentContentForTests dc = (IDocumentContentForTests)documentContent;
			Assert.IsNull(dc.DocumentOwner);
			InterfacesProxy.SetDocumentOwner(typeof(IDocumentContentForTests), typeof(IDocumentOwnerForTests), documentContent, documentOwner);
			Assert.AreEqual(documentOwner, dc.DocumentOwner);
		}
		[Test]
		public void Test04_OnClose() {
			System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
			InterfacesProxy.OnClose(typeof(IDocumentContentForTests), documentContent, e);
			Assert.AreEqual(1, documentContent.closeCount);
			e.Cancel = true;
			InterfacesProxy.OnClose(typeof(IDocumentContentForTests), documentContent, e);
			Assert.AreEqual(1, documentContent.closeCount);
		}
		[Test]
		public void Test04_OnDestroy() {
			InterfacesProxy.OnDestroy(typeof(IDocumentContentForTests), documentContent);
			Assert.AreEqual(1, documentContent.destroyCount);
		}
		[Test]
		public void Test05_GetUICommandTag() {
			UICommandForTests command = new UICommandForTests() { Tag = 5 };
			Assert.AreEqual(5, InterfacesProxy.GetUICommandTag(typeof(UICommandForTests), command));
		}
		[Test]
		public void Test06_GetTitle() {
			documentContent.titleCore = "Title";
			Assert.AreEqual("Title", InterfacesProxy.GetTitle(typeof(IDocumentContentForTests), documentContent));
		}
	}
}
#endif
