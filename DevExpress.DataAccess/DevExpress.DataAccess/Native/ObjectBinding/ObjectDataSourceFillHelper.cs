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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Utils;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Data.Access;
namespace DevExpress.DataAccess.Native.ObjectBinding {
	public static class ObjectDataSourceFillHelper {
		public class FindPropertiesHelper {
			object instance;
			object data;
			PropertyDescriptorCollection pdc;
			public object Instance {
				get { return instance; }
			}
			public object Data {
				get { return data; }
			}
			public PropertyDescriptorCollection PDC {
				get { return pdc; }
			}
			public FindPropertiesHelper(object dataSource, string dataMember, ParameterList dataMemberParameters, bool ctorIsNull, ParameterList ctorParameters) {
				Type type = dataSource as Type;
				instance = null;
				data = null;
				if(type != null) {
					if(typeof(IListSource).IsAssignableFrom(type) || typeof(ITypedList).IsAssignableFrom(type))
						instance = CreateInstance(type, ctorIsNull, ctorParameters);
				}
				else {
					type = dataSource.GetType();
					instance = dataSource;
				}
				if(!IsSpecialityType(ref type, dataMember, dataMemberParameters))
					pdc = GetPropertiesFromDataMember(type, dataMember, dataMemberParameters, ctorIsNull, ctorParameters);
			}
			bool IsSpecialityType(ref Type type, string dataMember, ParameterList parameters) {
				if(typeof(IListSource).IsAssignableFrom(type)) {
					instance = ((IListSource)instance).GetList();
					type = instance.GetType();
				}
				if(typeof(ExpandoObject).IsAssignableFrom(type)) {
					pdc = InvokeDataMemberOfExpandoObject((ExpandoObject)instance, dataMember, parameters);
					return true;
				}
				Type enumerableType;
				if(IsEnumerable(type, out enumerableType)) {
					if(typeof(ExpandoObject).IsAssignableFrom(enumerableType)) {
						ExpandoObject sample = ((IEnumerable)instance).Cast<ExpandoObject>().FirstOrDefault();
						pdc = InvokeDataMemberOfExpandoObject(sample, dataMember, parameters);
						return true;
					}
					if(string.IsNullOrEmpty(dataMember))
						pdc = TypeDescriptor.GetProperties(enumerableType);
					else
						pdc = TypeDescriptor.GetProperties(FindMethod(type, dataMember, parameters).ReturnType);
					return true;
				}
				if(typeof(ITypedList).IsAssignableFrom(type)) {
					pdc = ((ITypedList)instance).GetItemProperties(null);
					if(!string.IsNullOrEmpty(dataMember))
						pdc = ((ITypedList)instance).GetItemProperties(new[] { pdc[dataMember] });
					return true;
				}
				return false;
			}
			PropertyDescriptorCollection GetPropertiesFromDataMember(Type type, string dataMember, ParameterList parameters, bool ctorIsNull, ParameterList ctorParameters) {
				if(string.IsNullOrEmpty(dataMember))
					return TypeDescriptor.GetProperties(type);
				else {
					MethodInfo method = FindMethod(type, dataMember, parameters);
					Type typeOfMethod = method.ReturnType;
					if(typeof(ExpandoObject).IsAssignableFrom(typeOfMethod)) {
						GetData(type, method, parameters, ctorIsNull, ctorParameters);
						return GetExpandoObjectProperties((ExpandoObject)data);
					}
					Type enumerableType;
					if(IsEnumerable(typeOfMethod, out enumerableType)) {
						if(typeof(ExpandoObject).IsAssignableFrom(enumerableType)) {
							GetData(type, method, parameters, ctorIsNull, ctorParameters);
							return GetExpandoObjectProperties(((IEnumerable)data).Cast<ExpandoObject>().FirstOrDefault());
						}
						return TypeDescriptor.GetProperties(enumerableType);
					}
					if(typeof(ITypedList).IsAssignableFrom(typeOfMethod)) {
						GetData(type, method, parameters, ctorIsNull, ctorParameters);
						return ((ITypedList)data).GetItemProperties(null);
					}
					return TypeDescriptor.GetProperties(typeOfMethod);
				}
			}
			void GetData(Type type, MethodInfo method, ParameterList parameters, bool ctorIsNull, ParameterList ctorParameters) {
				if(instance == null && !method.IsStatic)
					instance = CreateInstance(type, ctorIsNull, ctorParameters);
				else if(method.IsStatic)
					instance = type;
				data = method.Invoke(instance, GetValues(method, parameters));
			}
			PropertyDescriptorCollection GetExpandoObjectProperties(ExpandoObject instance) {
				if(instance == null)
					return PropertyDescriptorCollection.Empty;
				return new PropertyDescriptorCollection(GetItemExpandoObjectProperties(instance).ToArray(), true);
			}
			IEnumerable<ExpandoPropertyDescriptor> GetItemExpandoObjectProperties(ExpandoObject instance) {
				foreach(KeyValuePair<string, object> p in instance) {
					string memberName = p.Key;
					object memberValue = p.Value;
					if(!(memberValue is Delegate))
						yield return new ExpandoPropertyDescriptor(null, memberName, (memberValue == null) ? typeof(object) : memberValue.GetType());
				}
			}
			PropertyDescriptorCollection InvokeDataMemberOfExpandoObject(ExpandoObject instance, string dataMember, ParameterList parameters) {
				if(string.IsNullOrEmpty(dataMember))
					return GetExpandoObjectProperties(instance);
				data = ((IDictionary<string, object>)instance)[dataMember];
				Type typeOfMethod = data.GetType();
				Delegate deleg = data as Delegate;
				if(deleg != null) {
					if(parameters == null || parameters.Count == 0)
						data = deleg.GetMethodInfo().Invoke(instance, null);
					else {
						Delegate[] allMethods = GetExpandoObjectMethods(instance, dataMember).ToArray();
						Delegate method = FindMemberCore(allMethods, dataMember, parameters);
						data = method.DynamicInvoke(GetValues(method.GetMethodInfo(), parameters));
					}
					typeOfMethod = data.GetType();
				}
				if(typeof(ExpandoObject).IsAssignableFrom(typeOfMethod))
					return GetExpandoObjectProperties((ExpandoObject)data);
				Type enumerableType;
				if(IsEnumerable(typeOfMethod, out enumerableType)) {
					if(typeof(ExpandoObject).IsAssignableFrom(enumerableType))
						return GetExpandoObjectProperties(((IEnumerable)data).Cast<ExpandoObject>().FirstOrDefault());
					return TypeDescriptor.GetProperties(enumerableType);
				}
				if(typeof(ITypedList).IsAssignableFrom(typeOfMethod))
					return ((ITypedList)data).GetItemProperties(null);
				return TypeDescriptor.GetProperties(data);
			}
			IEnumerable<Delegate> GetExpandoObjectMethods(ExpandoObject expando, string methodName) {
				return expando.Where(pair => pair.Key == methodName).Select(pair => pair.Value).OfType<Delegate>();
			}
			object CreateInstance(Type type, bool ctorIsNull, ParameterList ctorParameters) {
				if(ctorIsNull) {
					ConstructorInfo ctor = FindConstructor(type, new ParameterList(0));
					return ctor.Invoke(null);
				}
				else {
					ConstructorInfo ctor = FindConstructor(type, ctorParameters);
					return ctor.Invoke(GetValues(ctor, ctorParameters));
				}
			}
			bool IsEnumerable(Type type, out Type enumerableType) {
				if(typeof(IEnumerable).IsAssignableFrom(type)) {
					enumerableType = GetItemType(type);
					return (enumerableType != typeof(object));
				}
				enumerableType = null;
				return false;
			}
		}
		#region Stage 1 (create instance)
		public static object CreateInstanceForSchema(object dataSource) {
			Type dsType = dataSource as Type;
			object instance = null;
			if(dsType == null) {
				dsType = dataSource.GetType();
				instance = dataSource;
			}
			if(typeof(IListSource).IsAssignableFrom(dsType)) {
				instance = instance ?? CreateInstance(dsType);
				return ((IListSource)instance).GetList();
			}
			if(typeof(ITypedList).IsAssignableFrom(dsType))
				return instance ?? CreateInstance(dsType);
			return dsType;
		}
		public static object CreateInstanceForResult(object dataSource) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Type dsType = dataSource as Type;
			if(dsType == null) {
				dsType = dataSource.GetType();
				object instance = dataSource;
				if(typeof(IListSource).IsAssignableFrom(dsType))
					return ((IListSource)instance).GetList();
				return instance;
			}
			return dsType;
		}
		public static object CreateInstanceForResult(object dataSource, ParameterList ctorParameters) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Type dsType = dataSource as Type;
			object instance;
			if(dsType == null) {
				dsType = dataSource.GetType();
				instance = dataSource;
			}
			else
				instance = CreateInstance(dsType, ctorParameters);
			if(typeof(IListSource).IsAssignableFrom(dsType))
				return ((IListSource)instance).GetList();
			return instance;
		}
		public static object CreateInstance(Type dsType) {
			ConstructorInfo ctor = dsType.GetConstructor(new Type[0]);
			if(ctor == null)
				throw new NoDefaultConstructorException(dsType);
			object instance = ctor.Invoke(new object[0]);
			return instance;
		}
		static object CreateInstance(Type dsType, ParameterList ctorParameters) {
			ConstructorInfo ctor = FindConstructor(dsType, ctorParameters);
			object instance = ctor.Invoke(GetValues(ctor, ctorParameters));
			return instance;
		}
		public static ConstructorInfo FindConstructor(Type dsType, ParameterList ctorParameters) {
			ConstructorInfo[] constructorInfos = dsType.GetConstructors();
			return FindMemberCore(constructorInfos, dsType, ctorParameters);
		}
		static object[] GetValues(ConstructorInfo ctor, IEnumerable<Parameter> parameterList) {
			return GetValues(ctor.GetParameters(), parameterList);
		}
		#endregion
		#region Stage 2 (browse to data member)
		public static object BrowseForResult(object instance, string dataMember, ParameterList parameters) {
			ITypedList typedList = instance as ITypedList;
			if(typedList != null)
				return BrowseTypedList(dataMember, typedList);
			MethodInfo method = FindMethod(instance, dataMember, parameters);
			if(!method.IsStatic && instance is Type && method.DeclaringType != typeof(Type))
				return method.ReturnType;
			object[] values = GetValues(method, parameters);
			IEnumerable enumerable = instance as IEnumerable;
			if(enumerable != null) {
				IEnumerable<object> objEnumerable = enumerable.Cast<object>();
				int count = objEnumerable.Count();
				Array array = Array.CreateInstance(method.ReturnType, count);
				int i = 0;
				foreach(object o in objEnumerable)
					array.SetValue(method.Invoke(o, values), i++);
				return array;
			}
			return method.Invoke(instance, values) ?? method.ReturnType;
		}
		public static object BrowseTypedList(string dataMember, ITypedList typedList) {
			IEnumerable enumerable = typedList as IEnumerable;
			PropertyDescriptor property = typedList.GetItemProperties(null)[dataMember];
			if(property == null)
				throw new InvalidOperationException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.ODSErrorMissingMember), dataMember));
			if(enumerable != null) {
				ArrayList data = new ArrayList();
				foreach(object item in enumerable) {
					IEnumerable subEnumerable = property.GetValue(item) as IEnumerable;
					if(subEnumerable != null) {
						foreach(object subItem in subEnumerable) {
							data.Add(subItem);
						}
					}
					else data.Add(property.GetValue(item));
				}
				return data;
			}
			return property.GetValue(typedList);
		}
		public static MethodInfo FindMethod(object instance, string dataMember, ParameterList parameters) {
			MemberInfo member = FindMemberCore(instance, dataMember, parameters);
			PropertyInfo property = member as PropertyInfo;
			if(property != null)
				return property.GetGetMethod();
			MethodInfo method = member as MethodInfo;
			if(method != null)
				return method;
			throw new InvalidOperationException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.ODSErrorMissingMember), dataMember));
		}
		static object[] GetValues(MethodInfo method, IEnumerable<Parameter> parameterList) {
			return GetValues(method.GetParameters(), parameterList);
		}
		#endregion
		#region Stage 3 (explore instance)
		public static ResultTypedList CreateTypedList(object data, string name) {
			PropertyDescriptorCollection pdc = GetItemProperties(data);
			{
				Type type = data as Type;
				if(type != null) {
					return new ResultTypedList(name, pdc);
				}
			}
			IEnumerable enumerable = data as IEnumerable ?? new[] { data };
			if(data is ITypedList)
				return new ResultTypedList(name, (ITypedList)data, enumerable);
			return new ResultTypedList(name, pdc, enumerable);
		}
		public static ResultTypedList CreateTypedList(object data, string name, PropertyDescriptorCollection pdc) {
			Type type = data as Type;
			if(type != null) {
				return new ResultTypedList(name, pdc);
			}
			type = data.GetType();
			if(typeof(ExpandoObject).IsAssignableFrom(type)) {
				return new ResultTypedList(name, pdc, new[] { data });
			}
			IEnumerable enumerable = data as IEnumerable ?? new[] { data };
			if(data is ITypedList)
				return new ResultTypedList(name, (ITypedList)data, enumerable);
			return new ResultTypedList(name, pdc, enumerable);
		}
		public static PropertyDescriptorCollection GetItemProperties(object data) {
			Type type = data as Type ?? data.GetType();
			if(typeof(IEnumerable).IsAssignableFrom(type))
				type = GetItemType(type);
			return TypeDescriptor.GetProperties(type);
		}
		#endregion
		#region Wizard support
		public static object BrowseForSchema(object instance, ObjectMember dataMember) {
			if(dataMember == null)
				return instance;
			ITypedList typedList = instance as ITypedList;
			if(typedList != null && dataMember.Pd != null) {
				IEnumerable enumerable = typedList as IEnumerable;
				if(enumerable != null)
					return dataMember.Pd.GetValue(enumerable.Cast<object>().First());
				return dataMember.Pd.GetValue(typedList);
			}
			if(instance is IEnumerable)
				return typeof(IEnumerable<>).MakeGenericType(dataMember.ReturnType);
			return dataMember.ReturnType;
		}
		public static ObjectMember FindMember(object instance, string dataMember, ParameterList parameters) {
			ITypedList typedList = instance as ITypedList;
			if(typedList == null)
				return new ObjectMember(FindMemberCore(instance, dataMember, parameters));
			PropertyDescriptor itemProperty = typedList.GetItemProperties(null)[dataMember];
			if(itemProperty == null)
				return null;
			return new ObjectMember(itemProperty);
		}
		public static IEnumerable<ObjectMember> GetItemMembers(object data) {
			ITypedList typedList = data as ITypedList;
			IEnumerable<ObjectMember> members = typedList != null
				? GetItemMembersCore(typedList)
				: GetItemMembersCore(data as Type ?? data.GetType());
			return members.Where(member => IsBrowsable(member) && member.ReturnType.FullName != null && !DataContext.IsStandardType(member.ReturnType));
		}
		public static ParameterList EvaluateParametersForSchema(IEnumerable<Parameter> odsParameters) {
			return new ParameterList(odsParameters.Select(
				parameter =>
					parameter.Type == typeof(Expression)
						? new Parameter(parameter.Name, ((Expression)parameter.Value).ResultType, null)
						: parameter));
		}
		public static ParameterList EvaluateParametersForResult(ParameterList odsParameters, IEnumerable<IParameter> envParameters) {
			return
				new ParameterList(
					odsParameters.Select(
						parameter => {
							if(parameter.Type == typeof(Expression)) {
								Expression expression = ((Expression)parameter.Value);
								return new Parameter(parameter.Name, expression.ResultType,
									expression.EvaluateExpression(envParameters));
							}
							return parameter;
						}));
		}
		public static string ValidateResultType(Type type) {
			if(typeof(IEnumerable).IsAssignableFrom(type))
				if(GetItemType(type) != typeof(object))
					return null;
			if(typeof(IListSource).IsAssignableFrom(type))
				if(type.GetConstructor(new Type[0]) == null)
					return DataAccessLocalizer.GetString(DataAccessStringId.ODSWizardErrorNoDefaultCtor);
			if(typeof(ITypedList).IsAssignableFrom(type)) {
				ConstructorInfo constructor = type.GetConstructor(new Type[0]);
				if(constructor == null)
					return DataAccessLocalizer.GetString(DataAccessStringId.ODSWizardErrorNoDefaultCtor);
				ITypedList typedList;
				try { typedList = (ITypedList)constructor.Invoke(new object[0]); }
				catch {
					return DataAccessLocalizer.GetString(DataAccessStringId.ODSWizardErrorExceptionInCtor);
				}
				try { typedList.GetItemProperties(null); }
				catch {
					return DataAccessLocalizer.GetString(DataAccessStringId.ODSWizardErrorExceptionInGetItemProperties);
				}
			}
			if(type.IsAbstract() && type.IsSealed() && !GetItemMembers(type).Any())
				return DataAccessLocalizer.GetString(DataAccessStringId.ODSWizardErrorStaticValue);
			return null;
		}
		static bool IsBrowsable(ObjectMember member) {
			foreach(Attribute attribute in member.Attributes) {
				BrowsableAttribute browsable = attribute as BrowsableAttribute;
				if(browsable == null)
					continue;
				return browsable.Browsable;
			}
			return true;
		}
		static IEnumerable<ObjectMember> GetItemMembersCore(Type type) {
			bool enumerable = false;
			if(typeof(IEnumerable).IsAssignableFrom(type)) {
				type = GetItemType(type);
				enumerable = true;
			}
			IEnumerable<PropertyInfo> properties =
				type.GetProperties().Where(info => info.GetIndexParameters().Length == 0);
			IEnumerable<MethodInfo> methods =
				type.GetMethods().Where(info => info.ReturnType != typeof(void) && !IsPropertyGetter(info, type) && info.DeclaringType != typeof(object));
			if(enumerable) {
				properties = properties.Where(info => !info.GetGetMethod().IsStatic);
				methods = methods.Where(info => !info.IsStatic);
			}
			properties = RemoveShadowed(properties);
			methods = RemoveShadowed(methods);
			IEnumerable<ObjectMember> result = ((IEnumerable<MemberInfo>)properties).Union(methods)
				.Select(info => new ObjectMember(info));
			if(type.IsAbstract() && type.IsSealed())
				result = result.Where(member => member.IsStatic);
			return result;
		}
		static IEnumerable<PropertyInfo> RemoveShadowed(IEnumerable<PropertyInfo> properties) {
			List<PropertyInfo> list = properties.OrderBy(info => info.Name).ToList();
			for(int i = 0; i < list.Count - 1; i++) {
				PropertyInfo current = list[i];
				PropertyInfo next = list[i + 1];
				if(string.Equals(current.Name, next.Name, StringComparison.Ordinal)) {
					list.RemoveAt(next.DeclaringType.IsAssignableFrom(current.DeclaringType) ? i + 1 : i);
					i--;
				}
			}
			return list;
		}
		static IEnumerable<MethodInfo> RemoveShadowed(IEnumerable<MethodInfo> methods) {
			List<MethodInfo> list =
				methods.OrderBy(
					info =>
						string.Format("{0}({1})", info.Name,
							string.Join(",",
								info.GetParameters().Select(parameterInfo => parameterInfo.ParameterType.FullName))))
					.ToList();
			for(int i = 0; i < list.Count - 1; i++) {
				MethodInfo current = list[i];
				MethodInfo next = list[i + 1];
				if(string.Equals(current.Name, next.Name, StringComparison.Ordinal) && HasSameParameters(current, next)) {
					list.RemoveAt(next.DeclaringType.IsAssignableFrom(current.DeclaringType) ? i + 1 : i);
					i--;
				}
			}
			return list;
		}
		static bool HasSameParameters(MethodInfo x, MethodInfo y) {
			ParameterInfo[] xp = x.GetParameters();
			ParameterInfo[] yp = y.GetParameters();
			int n = xp.Length;
			if(yp.Length != n)
				return false;
			for(int i = 0; i < n; i++)
				if(xp[i].ParameterType != yp[i].ParameterType)
					return false;
			return true;
		}
		static IEnumerable<ObjectMember> GetItemMembersCore(ITypedList typedList) {
			try {
				return typedList.GetItemProperties(null)
					.Cast<PropertyDescriptor>()
					.Select(descriptor => new ObjectMember(descriptor));
			}
			catch {
				return new ObjectMember[0];
			}
		}
		static bool IsPropertyGetter(MethodInfo method, Type objectType) {
			if(!method.Name.StartsWith("get_"))
				return false;
			string propName = method.Name.Substring(4);
			if(string.IsNullOrEmpty(propName))
				return false;
			PropertyInfo property = objectType.GetProperty(propName, method.ReturnType, method.GetParameters().Select(pi => pi.ParameterType).ToArray());
			if(property == null)
				return false;
			return property.GetGetMethod() == method;
		}
		#endregion
		#region Misc helper staff
		static object[] GetValues(IEnumerable<ParameterInfo> signature, IEnumerable<Parameter> parameterList) {
			return
				signature.Select(
					info => parameterList.First(parameter => string.Equals(parameter.Name, info.Name, StringComparison.Ordinal)).Value).ToArray();
		}
		public static Type GetItemType(Type enumerableType) {
			IEnumerable<Type> interfaces = enumerableType.GetInterfaces();
			if(enumerableType.IsInterface())
				interfaces = interfaces.Union(new[] { enumerableType });
			Type generic =
				interfaces.FirstOrDefault(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if(generic != null)
				return generic.GetGenericArguments()[0];
			Type[] candidates =
				enumerableType.GetProperties()
					.Where(info => info.GetIndexParameters().Length > 0)
					.Select(info => info.PropertyType)
					.Distinct()
					.ToArray();
			if(candidates.Length == 1)
				return candidates[0];
			return typeof(object);
		}
		static MemberInfo FindMemberCore(object instance, string dataMember, ParameterList parameters) {
			Type type = instance as Type ?? instance.GetType();
			if(typeof(IEnumerable).IsAssignableFrom(type))
				type = GetItemType(type);
			MemberInfo[] memberInfos = type.GetMember(dataMember);
			return FindMemberCore<MemberInfo>(memberInfos, dataMember, parameters);
		}
		static TMember FindMemberCore<TMember>(TMember[] allMembers, string memberName, ParameterList parameters) where TMember : MemberInfo {
			Guard.ArgumentNotNull(parameters, "parameters");
			List<TMember> matches = new List<TMember>(allMembers.Length);
			foreach(TMember member in allMembers) {
				PropertyInfo property = member as PropertyInfo;
				if(property != null) {
					if(parameters.Count == 0)
						matches.Add((TMember)((MemberInfo)property));
					continue;
				}
				if(CheckSignature((MethodBase)(MemberInfo)member, parameters))
					matches.Add(member);
			}
			return Unique(matches, memberName, parameters);
		}
		static ConstructorInfo FindMemberCore(ConstructorInfo[] allMembers, Type type, ParameterList parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			List<ConstructorInfo> matches = allMembers.Where(ctor => CheckSignature(ctor, parameters)).ToList();
			if(matches.Count == 1)
				return matches[0];
			if(matches.Count > 1)
				throw new AmbigousConstructorException(type, parameters);
			if(parameters.Count == 0)
				throw new NoDefaultConstructorException(type);
			throw new ConstructorNotFoundException(type, parameters);
		}
		static Delegate FindMemberCore(Delegate[] allMembers, string memberName, ParameterList parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			List<Delegate> matches = allMembers.Where(member => CheckSignature(member.GetMethodInfo(), parameters)).ToList();
			return Unique(matches, memberName, parameters);
		}
		static bool CheckSignature(MethodBase method, ParameterList parameters) {
			ParameterInfo[] signature = method.GetParameters();
			if(signature.Length != parameters.Count)
				return false;
			foreach(ParameterInfo expected in signature) {
				Parameter actual = parameters.FirstOrDefault(p => string.Equals(p.Name, expected.Name, StringComparison.Ordinal));
				if(actual == null || !expected.ParameterType.IsAssignableFrom(actual.Type))
					return false;
			}
			return true;
		}
		static T Unique<T>(List<T> matches, string memberName, ParameterList parameters) {
			if(matches.Count == 0)
				throw new DataMemberResolveException(string.Format("Cannot find the following DataMember: {0}({1}).", memberName, string.Join(", ", parameters)));
			if(matches.Count > 1)
				throw new DataMemberResolveException(string.Format("DataMember ambiguous match: {0}({1}).", memberName, string.Join(", ", parameters)));
			return matches[0];
		}
		#endregion
	}
}
