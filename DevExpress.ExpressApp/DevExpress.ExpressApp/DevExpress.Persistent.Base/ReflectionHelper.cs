#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.Persistent.Base {
	[Serializable]
	public class TypeWasNotFoundException : ArgumentException {
		[Serializable]
		private struct TypeWasNotFoundExceptionState : ISafeSerializationData {
			public string TypeName { get; set; }
			void ISafeSerializationData.CompleteDeserialization(Object obj) {
				TypeWasNotFoundException exception = (TypeWasNotFoundException)obj;
				exception.state = this;
			}
		}
		[NonSerialized]
		private TypeWasNotFoundExceptionState state = new TypeWasNotFoundExceptionState();
		public TypeWasNotFoundException() : base() { }
		public TypeWasNotFoundException(string typeName)
			: base(String.Format("The '{0}' type was not found", typeName)) {
			state.TypeName = typeName;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public string TypeName {
			get { return state.TypeName; }
		}
	}
	[Serializable]
	public class MemberNotFoundException : ArgumentException {
		[Serializable]
		private struct MemberNotFoundExceptionState : ISafeSerializationData {
			public string TypeName { get; set; }
			public string MemberName { get; set; }
			void ISafeSerializationData.CompleteDeserialization(Object obj) {
				MemberNotFoundException exception = (MemberNotFoundException)obj;
				exception.state = this;
			}
		}
		[NonSerialized]
		private MemberNotFoundExceptionState state = new MemberNotFoundExceptionState();
		public MemberNotFoundException() : base() { }
		public MemberNotFoundException(Type type, string memberName)
			: base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotFindThePropertyWithinTheClass, memberName, type.FullName)) {
			state.TypeName = type.FullName;
			state.MemberName = memberName;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public MemberNotFoundException(Type type, string memberName, string message)
			: base(message) {
			state.TypeName = type.FullName;
			state.MemberName = memberName;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public string MemberName {
			get { return state.MemberName; }
		}
		public string TypeName {
			get { return state.TypeName; }
		}
	}
	public class AssemblyResolveEventArgs : EventArgs {
		AssemblyName assemblyName;
		public AssemblyName AssemblyName {
			get {
				return assemblyName;
			}
		}
		public AssemblyResolveEventArgs(AssemblyName assemblyName) {
			this.assemblyName = assemblyName;
		}
	}
	public delegate Assembly AssemblyResolveEventHandler(object sender, AssemblyResolveEventArgs args);
	public class TypeResolveEventArgs : EventArgs {
		private string typeName;
		public string TypeName {
			get {
				return typeName;
			}
		}
		public TypeResolveEventArgs(string typeName) {
			this.typeName = typeName;
		}
	}
	public delegate Type TypeResolveEventHandler(object sender, TypeResolveEventArgs args);
	public static class ReflectionHelper {
		private static bool? hasUnmanagedCodePermission = null;
		private static bool? hasControlAppDomainPermission = null;
		private static bool hasPermissionToGetAssemblyName = true;
		private static string xafVersionPostfix = null;
		private static HitCountCache<Assembly> assemblyCache = new HitCountCache<Assembly>();
		private static Dictionary<string, Type> nameToTypeMap = new Dictionary<string, Type>();
		private static List<string> resolvePaths = new List<string>();
		private static string lastLoadedAssemblyPath;
		private static int FindNextCloseBracketIndex(string str, int openBracketIndex) {
			int counter = 1;
			int i = openBracketIndex + 1;
			bool found = false;
			while(i < str.Length) {
				if(str[i] == '[') {
					counter++;
				}
				if(str[i] == ']') {
					counter--;
				}
				if(counter == 0) {
					found = true;
					break;
				}
				i++;
			}
			if(found) {
				return i;
			}
			else {
				return -1;
			}
		}
		private static Assembly GetAssembly(string assemblyName, List<string> assembliesPath) {
			if(assemblyName.IndexOf(',') >= 0) {
				assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
			}
			if(assemblyName.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)) {
				assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
			}
			Assembly assembly = null;
			try {
				foreach(string path in assembliesPath) {
					string assemblyLocation = Path.Combine(path, assemblyName.Split(',')[0] + ".dll");
					if(File.Exists(assemblyLocation)) {
						assembly = Assembly.LoadFrom(assemblyLocation);
						return assembly;
					}
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
			return null;
		}
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			Assembly result = ReflectionHelper.GetAssembly(args.Name, GetAssemblyPaths());
			if(result == null) {
				result = AlternativeAssemblyLoaderFromAppDomain(args.Name);
				string assemblyName = args.Name.Split(',')[0];
				if(result == null && !assemblyName.EndsWith(".resources")) { 
					Tracing.Tracer.LogText("Resolve the '{0}' assembly", args.Name);
				}
			}
			return result;
		}
		private static Assembly AlternativeAssemblyLoaderFromAppDomain(string assemblyName) {
			Assembly[] loadAssembly = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly ass in loadAssembly) {
				if(ass.FullName == assemblyName) {
					return ass;
				}
			}
			return null;
		}
		static ReflectionHelper() {
			if(HasControlAppDomainPermission) {
				HandleAppDomainAssemblyResolveEvent();
			}
		}
		[SecuritySafeCritical]
		private static void HandleAppDomainAssemblyResolveEvent() {
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		private static List<string> GetAssemblyPaths() {
			List<string> result = new List<string>(resolvePaths);
			lock(resolvePaths) {
				if(!string.IsNullOrEmpty(lastLoadedAssemblyPath) && !resolvePaths.Contains(lastLoadedAssemblyPath)) {
					result.Add(lastLoadedAssemblyPath);
				}
				return result;
			}
		}
		private static bool TryLoadAssemblyFromFile(string fileName, out Assembly result) {
			result = null;
			if(File.Exists(fileName)) {
				result = Assembly.LoadFrom(fileName);
				return true;
			}
			return false;
		}
		private static Assembly InternalLoadAssembly(string assemblyName, string assembliesPath) {
			lastLoadedAssemblyPath = assembliesPath;
			Assembly result = null;
			string fullPathWithoutExtension = Path.Combine(assembliesPath, assemblyName);
			if(TryLoadAssemblyFromFile(fullPathWithoutExtension + ".exe", out result))
				return result;
			if(TryLoadAssemblyFromFile(fullPathWithoutExtension + ".dll", out result))
				return result;
			AssemblyName fullAssemblyName = new AssemblyName(assemblyName);
			if((fullAssemblyName.Version != null)) {
				return Assembly.Load(fullAssemblyName); 
			}
			else {
#pragma warning disable 0618
				return Assembly.LoadWithPartialName(assemblyName, null); 
#pragma warning restore 0618
			}
		}
		private static Assembly LoadAssembly(AssemblyName assemblyName, string assembliesPath) {
			lastLoadedAssemblyPath = assembliesPath;
			return Assembly.Load(assemblyName);
		}
		private static Assembly FindLoadedAssembly(string assemblyName) { 
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if(string.Compare(assemblyName, GetAssemblyName(assembly), true) == 0) {
					return assembly;
				}
			}
			return null;
		}
		private static bool IsShortTypeName(string typeName) {
			return (!typeName.Contains(".")) && (!typeName.Contains(","));
		}
		private static Type FindTypeInAssemblies(string typeName, ICollection<Assembly> assemblies) {
			foreach(Assembly assembly in assemblies) {
				Type type = assembly.GetType(typeName);
				if(type != null) {
					if(IsAssemblyVersionCorrect(assembly)) {
						return type;
					}
				}
			}
			if(IsShortTypeName(typeName)) {
				List<ReflectionTypeLoadException> loaderExceptions = new List<ReflectionTypeLoadException>();
				foreach(Assembly assembly in assemblies) {
					try {
						foreach(Type type in assembly.GetTypes()) {
							if(type.Name == typeName) {
								if(IsAssemblyVersionCorrect(assembly)) {
									return type;
								}
							}
						}
					}
					catch(ReflectionTypeLoadException e) {
						loaderExceptions.Add(e);
					}
				}
				foreach(ReflectionTypeLoadException exception in loaderExceptions) {
					Tracing.Tracer.LogError(exception);
					if(exception.LoaderExceptions.Length > 0)
						Tracing.Tracer.LogError(exception.LoaderExceptions[0]);
				}
			}
			return null;
		}
		private static bool IsDevExpressAssembly(Assembly assembly) {
			return assembly.FullName.StartsWith("DevExpress.");
		}
		private static bool IsAssemblyVersionCorrect(Assembly assembly) {
			if(xafVersionPostfix != null && IsDevExpressAssembly(assembly)) {
				AssemblyName assemblyName = assembly.GetName();
				int versionIndex = assemblyName.Name.LastIndexOf(".v");
				if(versionIndex != -1 && assemblyName.Name.Substring(versionIndex) != xafVersionPostfix) {
					return false;
				}
			}
			return true;
		}
		public static Assembly LoadAssembly(string assemblyName, string assembliesPath) {
			Assembly assembly = null;
			if(AssemblyResolve != null) {
				assembly = AssemblyResolve(null, new AssemblyResolveEventArgs(new AssemblyName(assemblyName)));
			}
			if(assembly == null) {
				assembly = FindLoadedAssembly(assemblyName);
				if(assembly == null) {
					try {
						assembly = InternalLoadAssembly(assemblyName, assembliesPath);
					}
					catch(FileNotFoundException) { }
				}
			}
			return assembly;
		}
		public static Assembly GetAssembly(AssemblyName assemblyName, string assembliesPath) {
			Assembly assembly = null;
			if(AssemblyResolve != null) {
				assembly = AssemblyResolve(null, new AssemblyResolveEventArgs(assemblyName));
			}
			if(assembly == null) {
				assembly = FindLoadedAssembly(assemblyName.Name);
				if(assembly == null) {
					assembly = LoadAssembly(assemblyName, assembliesPath);
					if(assembly == null) {
						throw new FileNotFoundException(string.Format(
							"The '{0}' assembly is not found", assemblyName));
					}
				}
			}
			return assembly;
		}
		public static Assembly GetAssembly(string assemblyName, string assembliesPath) {
			Assembly assembly = LoadAssembly(assemblyName, assembliesPath);
			if(assembly == null) {
				throw new FileNotFoundException(string.Format(
					"The '{0}' assembly is not found", assemblyName));
			}
			return assembly;
		}
		public static void AddResolvePath(string path) {
			lock(resolvePaths) {
				if(!string.IsNullOrEmpty(path) && !resolvePaths.Contains(path)) {
					resolvePaths.Add(path);
				}
			}
		}
		public static void RemoveResolvePath(string path) {
			lock(resolvePaths) {
				resolvePaths.Remove(path);
			}
		}
		public static void CacheAssemblyTypes(Assembly assembly) {
			XafTypesInfo.Instance.LoadTypes(assembly);
			lock(assemblyCache) {
				if(!assemblyCache.Contains(assembly)) {
					assemblyCache.Add(assembly);
					assemblyCache.Hit(assembly);
				}
			}
		}
		public static Type FindType(string typeName) {
			ITypeInfo result = FindTypeInfoByName(typeName);
			return result != null ? result.Type : null;
		}
		private static ITypeInfo FindEntityByShortName(string shortName) {
			return XafTypesInfo.Instance.PersistentTypes.FirstOrDefault(info => info.Name == shortName);
		}
		public static ITypeInfo FindTypeInfoByName(string typeName) {
			if(String.IsNullOrEmpty(typeName))
				return null;
			ITypeInfo result = XafTypesInfo.Instance.FindTypeInfo(typeName);
			if(result == null) {
				result = FindEntityByShortName(typeName);
			}
			if(result != null)
				return result;
			Type typeToFind = null;
			Tracing.Tracer.LogLockedSectionEntering(typeof(ReflectionHelper), "FindType", nameToTypeMap);
			lock(nameToTypeMap) {
				Tracing.Tracer.LogLockedSectionEntered();
				if(!nameToTypeMap.TryGetValue(typeName, out typeToFind)) {
					Tracing.Tracer.LogLockedSectionEntering(typeof(ReflectionHelper), "FindType", assemblyCache);
					lock(assemblyCache) {
						if(!assemblyCache.Contains(Assembly.GetExecutingAssembly())) {
							assemblyCache.Add(Assembly.GetExecutingAssembly());
						}
						Tracing.Tracer.LogLockedSectionEntered();
						typeToFind = OnTypeResolve(typeName);
						if(typeToFind == null) {
							typeToFind = FindTypeInAssemblies(typeName, assemblyCache);
							if(typeToFind == null) {
								List<Assembly> nonCachedAssemblies = new List<Assembly>();
								foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
									if(!assemblyCache.Contains(assembly)) {
										assemblyCache.Add(assembly);
										nonCachedAssemblies.Add(assembly);
									}
								}
								typeToFind = FindTypeInAssemblies(typeName, nonCachedAssemblies);
							}
						}
						if(typeToFind != null && !typeToFind.IsGenericType) {
							nameToTypeMap.Add(typeName, typeToFind);
							assemblyCache.Hit(typeToFind.Assembly);
						}
					}
				}
			}
			result = XafTypesInfo.Instance.FindTypeInfo(typeToFind);
			return result;
		}
		private static Type OnTypeResolve(string typeName) {
			Type result = null;
			if(TypeResolve != null) {
				result = TypeResolve(null, new TypeResolveEventArgs(typeName));
			}
			lock(assemblyCache) {
				if(result != null && !assemblyCache.Contains(result.Assembly)) {
					assemblyCache.Add(result.Assembly);
				}
			}
			return result;
		}
		public static Type GetType(string typeName) {
			if(string.IsNullOrEmpty(typeName)) {
				return null;
			}
			Type objectType = Type.GetType(typeName);
			if(objectType == null) {
				objectType = FindType(typeName);
			}
			if(objectType == null) {
				throw new TypeWasNotFoundException(typeName);
			}
			return objectType;
		}
		public static IEnumerable<ITypeInfo> FindTypeDescendants(IAssemblyInfo assemblyInfo, ITypeInfo typeInfo) {
			return FindTypeDescendants(assemblyInfo, typeInfo, true);
		}
		public static IEnumerable<ITypeInfo> FindTypeDescendants(IAssemblyInfo assemblyInfo, ITypeInfo typeInfo, bool includeObsolete) {
			foreach(ITypeInfo info in FindTypeDescendants(typeInfo)) {
				if((includeObsolete || (info.FindAttribute<ObsoleteAttribute>() == null)) &&
					(info.AssemblyInfo == assemblyInfo)) {
					yield return info;
				}
			}
		}
		public static IEnumerable<ITypeInfo> FindTypeDescendants(ITypeInfo typeInfo, bool includeObsolete) {
			foreach(ITypeInfo info in FindTypeDescendants(typeInfo)) {
				if((includeObsolete || (info.FindAttribute<ObsoleteAttribute>() == null))) {
					yield return info;
				}
			}
		}
		public static IEnumerable<ITypeInfo> FindTypeDescendants(ITypeInfo typeInfo) {
			if(typeInfo.IsInterface) {
				return typeInfo.Implementors;
			}
			return typeInfo.Descendants;
		}
		public static Type GetInterfaceImplementation(Type classType, Type interfaceType) {
			if(classType != null) {
				if(interfaceType.IsAssignableFrom(classType) || AreEqualTypeDefinitions(classType, interfaceType))
					return classType;
				foreach(Type implemented in classType.GetInterfaces()) {
					if(AreEqualTypeDefinitions(implemented, interfaceType))
						return implemented;
				}
			}
			return null;
		}
		public static bool AreEqualTypeDefinitions(Type type1, Type type2) {
			if((type1 == null) && (type2 == null)) {
				return true;
			}
			else {
				if((type1 == null) || (type2 == null)) {
					return false;
				}
			}
			if(type1.IsGenericType && type2.IsGenericType) {
				return type1.GetGenericTypeDefinition() == type2.GetGenericTypeDefinition();
			}
			else {
				if(!type1.IsGenericType && !type2.IsGenericType) {
					return type1 == type2;
				}
				else {
					return false;
				}
			}
		}
		public static bool IsTypeAssignableFrom(ITypeInfo typeInfo, ITypeInfo checkingTypeInfo) {
			if(typeInfo.IsAssignableFrom(checkingTypeInfo)) {
				return true;
			}
			CheckAssemblesConflict(checkingTypeInfo.Type, typeInfo.Type, true);
			return false;
		}
		public static string GetShortClassName(string className) {
			string shortClassName = "";
			int firstOpenBracketIndex = className.IndexOf("[");
			if(firstOpenBracketIndex < 0) {
				shortClassName = StringHelper.GetLastPart('.', StringHelper.GetFirstPart(',', className));
			}
			else {
				shortClassName = StringHelper.GetLastPart('.', className.Substring(0, firstOpenBracketIndex - 2));
				int currentParameterOpenBracketIndex = className.IndexOf("[", firstOpenBracketIndex + 1);
				while(currentParameterOpenBracketIndex > 0) {
					int currentParameterCloseBracketIndex = FindNextCloseBracketIndex(className, currentParameterOpenBracketIndex);
					if(currentParameterCloseBracketIndex < 0) {
						Tracing.Tracer.LogWarning("Cannot find close bracket.\r\n" +
							" ClassName: '{0}'\r\n" +
							" currentOpenBracketIndex: '{1}'", className, currentParameterOpenBracketIndex);
						return className;
					}
					shortClassName += "_" + GetShortClassName(className.Substring(currentParameterOpenBracketIndex + 1, currentParameterCloseBracketIndex - currentParameterOpenBracketIndex));
					currentParameterOpenBracketIndex = className.IndexOf('[', currentParameterCloseBracketIndex);
				}
			}
			return shortClassName;
		}
		public static IMemberInfo FindDisplayableMemberDescriptor(ITypeInfo typeInfo, String memberName) {
			IMemberInfo member = typeInfo.FindMember(memberName);
			return FindDisplayableMemberDescriptor(member);
		}
		public static IMemberInfo FindDisplayableMemberDescriptor(IMemberInfo member) {
			if(member == null)
				return null;
			IMemberInfo displayableMember = null;
			if(member.MemberTypeInfo != null) {
				displayableMember = member.MemberTypeInfo.DefaultMember;
			}
			if(displayableMember == null) {
				return member;
			}
			else {
				if(!Enumerator.Exists<IMemberInfo>(member.GetPath(), displayableMember)) {
					return XafTypesInfo.Instance.CreatePath(member,
						FindDisplayableMemberDescriptor(displayableMember)
					);
				}
				else {
					return member;
				}
			}
		}
		public static object CreateObject(string typeName) {
			return CreateObject(GetType(typeName));
		}
		public static ObjectType CreateObject<ObjectType>(params object[] args) {
			return (ObjectType)CreateObject(typeof(ObjectType), args);
		}
		public static object CreateObject(Type objectType, params object[] args) {
			Guard.ArgumentNotNull(objectType, "objectType");
			return TypeHelper.CreateInstance(objectType, args);
		}
		public static object CreateObject(string objectTypeName, params object[] args) {
			return CreateObject(GetType(objectTypeName), args);
		}
		public static object GetObjectFromString(string stringObject, Type type) {
			object result = null;
			if(typeof(IConvertible).IsAssignableFrom(type)) {
				result = System.Convert.ChangeType(stringObject, type);
			}
			else {
				result = ReflectionHelper.CreateObject(type, new object[] { stringObject });
			}
			return result;
		}
		public static object Convert(object objectToConvert, Type conversionType) {
			Guard.ArgumentNotNull(objectToConvert, "objectToConvert");
			Guard.ArgumentNotNull(conversionType, "conversionType");
			Type underlyingNullableType = Nullable.GetUnderlyingType(conversionType);
			if(underlyingNullableType != null) {
				if(objectToConvert == null || (objectToConvert is string && string.IsNullOrEmpty((string)objectToConvert) && !typeof(string).IsAssignableFrom(conversionType))) {
					return null;
				}
				conversionType = underlyingNullableType;
			}
			MethodInfo methodInfo = conversionType.GetMethod("op_Implicit", new Type[] { objectToConvert.GetType() });
			if(methodInfo != null) {
				return methodInfo.Invoke(null, new object[] { objectToConvert });
			}
			if(conversionType.IsEnum && objectToConvert is string) {
				try {
					return new EnumDescriptor(conversionType).ParseCaption((string)objectToConvert);
				}
				catch(KeyNotFoundException) {
					return Enum.Parse(conversionType, (string)objectToConvert);
				}
			}
			if(conversionType == typeof(Guid) && objectToConvert is string) {
				return new Guid(((string)objectToConvert).Trim()); 
			}
			return System.Convert.ChangeType(objectToConvert, conversionType);
		}
		public static void SetMemberValue(object theObject, string propertyName, object propertyValue) {
			ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType());
			objType.FindMember(propertyName).SetValue(theObject, propertyValue);
		}
		public static object GetMemberValue(object theObject, string propertyName) {
			if(theObject == null)
				return null;
			return GetMemberValue(theObject, propertyName, theObject.GetType());
		}
		public static object GetMemberValue(object theObject, string propertyName, Type objectType) {
			if((theObject == null) || (theObject == DBNull.Value))
				return null;
			ITypeInfo objTypeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			IMemberInfo propertyInfo = objTypeInfo.FindMember(propertyName);
			if(propertyInfo == null) {
				throw new MemberNotFoundException(objectType, propertyName);
			}
			return propertyInfo.GetValue(theObject);
		}
		public static IList<Type> GetInterfaceHierarchy(Type interfaceType) {
			if(!interfaceType.IsInterface) {
				throw new ArgumentException("!interfaceType.IsInterface");
			}
			IList<Type> resultList = new List<Type>(new Type[] { interfaceType });
			foreach(Type currentInterfaceType in interfaceType.GetInterfaces()) {
				int indexToInsert = 0;
				foreach(Type typeToCompareWith in resultList) {
					if(typeToCompareWith.IsAssignableFrom(currentInterfaceType)) {
						indexToInsert = resultList.IndexOf(typeToCompareWith) + 1;
					}
				}
				resultList.Insert(indexToInsert, currentInterfaceType);
			}
			return resultList;
		}
		public static IList<Type> GetInterfaceHierarchyForType(Type targetType, Type interfaceType) {
			if(!interfaceType.IsInterface) {
				throw new ArgumentException("!interfaceType.IsInterface");
			}
			if(targetType == interfaceType) {
				return GetInterfaceHierarchy(interfaceType);
			}
			IList<Type> resultList = new List<Type>();
			IList<Type> typeInterfaces = targetType.GetInterfaces();
			if(typeInterfaces.Contains(interfaceType)) {
				foreach(Type currentInterfaceType in typeInterfaces) {
					if(!currentInterfaceType.IsAssignableFrom(interfaceType) && !interfaceType.IsAssignableFrom(currentInterfaceType)) {
						continue;
					}
					int indexToInsert = 0;
					foreach(Type typeToCompareWith in resultList) {
						if(typeToCompareWith.IsAssignableFrom(currentInterfaceType)) {
							indexToInsert = resultList.IndexOf(typeToCompareWith) + 1;
						}
					}
					resultList.Insert(indexToInsert, currentInterfaceType);
				}
			}
			return resultList;
		}
		public static IList<Type> GetInterfaceHierarchyForObject(object theObject, Type interfaceType) {
			return GetInterfaceHierarchyForType(theObject.GetType(), interfaceType);
		}
		public static string GetObjectDisplayText(object obj) {
			if(obj == null) {
				return null;
			}
			ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
			if(objType != null && objType.DefaultMember != null) {
				return GetObjectDisplayText(objType.DefaultMember.GetValue(obj));
			}
			if(objType.Type.IsEnum) {
				EnumDescriptor enumDescriptor = new EnumDescriptor(objType.Type);
				return enumDescriptor.GetCaption(obj);
			}
			return obj.ToString();
		}
		private static Dictionary<Type, string[]> enumHash = new Dictionary<Type, string[]>();
		public static TEnum ParseToEnum<TEnum>(string value, TEnum defaultValue) {
			Type enumType = typeof(TEnum);
			if(!enumHash.ContainsKey(enumType)) {
				enumHash.Add(enumType, Enum.GetNames(enumType));
			}
			string[] enumValues = enumHash[typeof(TEnum)];
			int i = 0;
			foreach(string enumValue in enumValues) {
				if(value == enumValue) {
					return (TEnum)Enum.ToObject(enumType, i);
				}
				i++;
			}
			return defaultValue;
		}
		public static void Reset() {
			lock(assemblyCache) {
				assemblyCache.Clear();
			}
			lock(nameToTypeMap) {
				nameToTypeMap.Clear();
			}
		}
		public static Boolean AreEqual(Object objA, Object objB) {
			if(Object.Equals(objA, objB)) {
				return true;
			}
			else {
				if(objA != null && objB != null && objA.GetType() == objB.GetType()) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objA.GetType());
					Boolean result = false;
					foreach(IMemberInfo keyMemberInfo in typeInfo.KeyMembers) {
						if(keyMemberInfo != null) {
							Object keyA = keyMemberInfo.GetValue(objA);
							Object keyB = keyMemberInfo.GetValue(objB);
							result = (!Object.Equals(keyA, null) && !Object.Equals(keyB, null) && Object.Equals(keyA, keyB));
							if(!result) {
								break;
							}
						}
					}
					return result;
				}
				return false;
			}
		}
		public static bool CompareAssemblyName(Assembly assembly, string assemblyName) {
			return (string.Compare(assemblyName, GetAssemblyName(assembly), true) == 0);
		}
		public static string GetAssemblyName(Assembly assembly) {
			return AssemblyHelper.GetName(assembly);
		}
		public static Version GetAssemblyVersion(Assembly assembly) {
			return AssemblyHelper.GetVersion(assembly);
		}
		public static void CheckAssemblesConflict(Type type1, Type type2) {
			if(type1 == type2) {
				return;
			}
			if(type1.FullName == type2.FullName && type1.Assembly.FullName == type2.Assembly.FullName) {
				throw new InvalidOperationException("Assemblies conflict:\r\n" +
					type1.Assembly.FullName + ", " + type1.Assembly.Location + "\r\n" +
					type2.Assembly.FullName + ", " + type2.Assembly.Location);
			}
		}
		public static void CheckAssemblesConflict(Type type1, Type type2, bool checkBaseClasses) {
			if(type1 == type2 || type1 == null || type2 == null) {
				return;
			}
			if(checkBaseClasses) {
				Type currentType = type1;
				while((currentType != typeof(Object)) && (currentType != null)) {
					if(currentType == type2) {
						break;
					}
					CheckAssemblesConflict(currentType, type2);
					currentType = currentType.BaseType;
				}
			}
			else {
				CheckAssemblesConflict(type1, type2);
			}
		}
		public static ItemType[] CombineArrays<ItemType>(IEnumerable<ItemType> items1, params ItemType[] items2) {
			List<ItemType> result = new List<ItemType>(items1);
			result.AddRange(items2);
			return result.ToArray();
		}
		public static ItemType[] CombineArrays<ItemType>(IEnumerable<ItemType> items1, IEnumerable<ItemType> items2) {
			List<ItemType> result = new List<ItemType>(items1);
			result.AddRange(items2);
			return result.ToArray();
		}
		public static string CreateStringFromMemoryStream(MemoryStream memStream) {
			int count;
			byte[] byteArray;
			char[] charArray;
			byteArray = new byte[memStream.Length];
			count = memStream.Read(byteArray, 0, (int)memStream.Length);
			UnicodeEncoding uniEncoding = new UnicodeEncoding();
			charArray = new char[uniEncoding.GetCharCount(
				byteArray, 0, count)];
			uniEncoding.GetDecoder().GetChars(
				byteArray, 0, count, charArray, 0);
			return new string(charArray);
		}
		public static MemoryStream CreateMemoryStreamFromString(string str) {
			UnicodeEncoding uniEncoding = new UnicodeEncoding();
			byte[] bytes = uniEncoding.GetBytes(str);
			MemoryStream memStream = new MemoryStream(bytes.Length);
			memStream.Write(bytes, 0, bytes.Length);
			return memStream;
		}
		public static void ThrowInvalidCastException(Type targetType, Type type) {
			throw new InvalidCastException(string.Format("Unable to cast object of type '{0}' to type '{1}'", type, targetType));
		}
		public static string GetCurrentCallStack() {
			StackTrace stackTrace = new StackTrace(1, true);
			try {
				stackTrace = new StackTrace(1, true);
			}
			catch {
			}
			if(stackTrace == null) {
				try {
					stackTrace = new StackTrace(1);
				}
				catch { }
			}
			string result = "";
			if(stackTrace != null) {
				try {
					result = stackTrace.ToString();
				}
				catch(Exception e) {
					result = "Cannot enumerate stack frames: " + e.Message;
				}
			}
			return result;
		}
		public static void SetXafVersionPostfix(string postfix) {
			xafVersionPostfix = postfix;
		}
		public static bool HasPermissionToGetAssemblyName {
			get {
				if(hasPermissionToGetAssemblyName) {
					try {
						typeof(ReflectionHelper).Assembly.GetName();
					}
					catch(SecurityException) {
						hasPermissionToGetAssemblyName = false;
					}
				}
				return hasPermissionToGetAssemblyName;
			}
		}
		[SecuritySafeCritical]
		private static bool GetDomainPermission(SecurityPermissionFlag flag) {
			try {
				var permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new SecurityPermission(flag));
				return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
			}
			catch {
				return false;
			}
		}
		public static bool HasUnmanagedCodePermission {
			get {
				if(hasUnmanagedCodePermission == null) {
					hasUnmanagedCodePermission = GetDomainPermission(SecurityPermissionFlag.UnmanagedCode);
				}
				return hasUnmanagedCodePermission.Value;
			}
		}
		public static bool HasControlAppDomainPermission {
			get {
				if(hasControlAppDomainPermission == null) {
					hasControlAppDomainPermission = GetDomainPermission(SecurityPermissionFlag.ControlAppDomain);
				}
				return hasControlAppDomainPermission.Value;
			}
		}
		public static event AssemblyResolveEventHandler AssemblyResolve;
		public static event TypeResolveEventHandler TypeResolve;
	}
}
