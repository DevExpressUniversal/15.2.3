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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Model.Core {
	public static class HelperTypeGenerator {
		public static string IncorrectParametersMessageFormat { get { return "The {0}.{1} logic method has incorrect parameters."; } }
		public static void ThrowInvalidOperationException(string message) {
			throw new InvalidOperationException(message);
		}
		public static bool AreParametersEqual(ParameterInfo[] params1, ParameterInfo[] params2) {
			if(params1.Length == params2.Length) {
				for(int k = 0;k < params1.Length;k++) {
					if(!params1[k].ParameterType.Equals(params2[k].ParameterType)) {
						return false;
					}
				}
			} else {
				return false;
			}
			return true;
		}
		public static string GetCorrectFieldName(string fieldName) {
			return fieldName.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "").Replace(".", "_").Replace("?", "");
		}
		public static string GetTypeForMemberName(Type type, ICollection<Type> usedTypes, bool isFullName) {
			return GetCorrectFieldName(TypeToString(type, usedTypes, isFullName).Replace("global::", ""));
		}
		public static string TypeToString(Type type, ICollection<Type> usedTypes, bool isFullName) {
			if(type == null) {
				return string.Empty;
			}
			AddUsedType(type, usedTypes);
			if(Nullable.GetUnderlyingType(type) != null) {
				return TypeToString(Nullable.GetUnderlyingType(type), usedTypes, isFullName) + "?";
			}
			if(!type.IsGenericType) {
				if(type.Name == "Void") {
					return "void";
				}
				string result = isFullName && type.FullName != null ? "global::" + type.FullName.Replace('+', '.') : type.Name;
				return result.Replace("&", "");
			}
			string genericArgsStr = string.Empty;
			string typeName = type.Name;
			string[] splits = typeName.Split('`');
			if(splits.Length > 0) {
				typeName = splits[0];
				Type[] genericArgs = type.GetGenericArguments();
				if(genericArgs.Length > 0) {
					foreach(Type currentArg in genericArgs) {
						genericArgsStr += TypeToString(currentArg, usedTypes, isFullName) + ",";
					}
					genericArgsStr = genericArgsStr.TrimEnd(',');
				}
			}
			return string.Format("{0}{1}<{2}>", isFullName ? type.Namespace + "." : string.Empty, typeName, genericArgsStr);
		}
		public static string GetLogicInstanceName(Type logic) {
			List<Type> tmp = new List<Type>();
			return GetLogicInstanceName(logic, tmp);
		}
		public static string GetLogicInstanceName(Type logic, ICollection<Type> usedTypes) {
			return string.Format("{0}{1}", SpecificWords.FieldLogicInstancePrefix, HelperTypeGenerator.GetTypeForMemberName(logic, usedTypes, true));
		}
		public static Type GetForInterface(Type type, CustomLogics customLogics) {
			object[] tmpAtt = type.GetCustomAttributes(typeof(DomainLogicAttribute), true);
			if(tmpAtt.Length == 0) {
				return customLogics.GetInterfaceByLogic(type);
			} else {
				return ((DomainLogicAttribute)tmpAtt[0]).InterfaceType;
			}
		}
		public static void AddUsedType(Type type, ICollection<Type> usedTypes) {
			if(type == null) {
				return;
			}
			if(!usedTypes.Contains(type)) {
				usedTypes.Add(type);
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null && !usedTypes.Contains(underlyingType)) {
				usedTypes.Add(underlyingType);
			}
		}
	}
	public class SpecificWords {
		public const string FieldLogicInstancePrefix = "logic_";
		public const string FieldPropertyPrefix = "_";
		public const string LogicPrefixGetStatic = "Get_";
		public const string LogicPrefixGetNonStatic = "get_";
		public const string LogicPrefixSetStatic = "Set_";
		public const string LogicPrefixSetNonStatic = "set_";
		public const string LogicAfterConstruction = "AfterConstruction";
		public const string LogicBeforeGet = "BeforeGet";
		public const string LogicBeforeSet = "BeforeSet";
		public const string LogicUniversalGet = "UniversalGet";
	}
	public interface ICodeGeneratorLogicClassesFinder {
		Type[] FindLogics(Type forInterface);
		CustomLogics CustomLogics { get; }
	}
	public class CodeGeneratorLogicClassesFinder : ICodeGeneratorLogicClassesFinder {
		private readonly CustomLogics customLogics;
		private readonly Dictionary<Type, List<Type>> logicsByInterface;
		private readonly Dictionary<Assembly, Object> processedAssemblies;
		public CodeGeneratorLogicClassesFinder(CustomLogics customLogics) {
			Guard.ArgumentNotNull(customLogics, "customLogics");
			this.customLogics = customLogics;
			logicsByInterface = new Dictionary<Type, List<Type>>();
			processedAssemblies = new Dictionary<Assembly, Object>();
		}
		private Type[] FindLogics(Type forInterface) {
			CollectLogics(forInterface);
			List<Type> logics;
			if(logicsByInterface.TryGetValue(forInterface, out logics)) {
				return logics.ToArray();
			}
			return Type.EmptyTypes;
		}
		private void CollectLogics(Type type) {
			if(!processedAssemblies.ContainsKey(type.Assembly)) {
				foreach(Type logicCandidate in ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(type.Assembly)) {
					if(logicCandidate.IsPublic) {
						foreach(DomainLogicAttribute domainLogicAttribute in AttributeHelper.GetAttributes<DomainLogicAttribute>(logicCandidate, true)) {
							List<Type> logics;
							if(!logicsByInterface.TryGetValue(domainLogicAttribute.InterfaceType, out logics)) {
								logics = new List<Type>();
								logicsByInterface.Add(domainLogicAttribute.InterfaceType, logics);
							}
							logics.Add(logicCandidate);
						}
					}
				}
				processedAssemblies.Add(type.Assembly, null);
			}
		}
		#region ICodeGeneratorLogicClassesFinder Members
		Type[] ICodeGeneratorLogicClassesFinder.FindLogics(Type type) { return FindLogics(type); }
		public CustomLogics CustomLogics { get { return customLogics; } }
		#endregion
	}
	[DebuggerDisplay("Name = {Name}")]
	public sealed class ClassDescription {
		private readonly static List<ClassDescription> classDescriptions = new List<ClassDescription>();
		private readonly String name;
		private readonly Type primaryInterface;
		private readonly Type baseClass;
		private readonly Type[] interfaces;
		public static ClassDescription GetClassDescription(String name, Type primaryInterface, Type baseClass, ICollection<Type> interfaces) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(primaryInterface, "primaryInterface");
			if(!primaryInterface.IsInterface) {
				throw new ArgumentException(string.Format("The primaryInterface argument is not interface.", baseClass.FullName), "primaryInterface");
			}
			if(baseClass != null && !baseClass.IsClass) {
				throw new ArgumentException(string.Format("The {0} baseClass argument isn't a class.", baseClass.FullName), "baseClass");
			}
			Guard.ArgumentNotNull(interfaces, "interfaces");
			if(interfaces.Count == 0) {
				throw new ArgumentException(string.Format("The interfaces argument is empty.", baseClass.FullName), "interfaces");
			}
			foreach(Type interfaceType in interfaces) {
				if(!interfaceType.IsInterface) {
					throw new ArgumentException(string.Format("The interfaces argument contains not interface element.", baseClass.FullName), "interfaces");
				}
			}
			lock(classDescriptions) {
				ClassDescription result = FindClassDescription(name, primaryInterface, baseClass, interfaces);
				if(result == null) {
					Type[] interfaceArray = new Type[interfaces.Count];
					interfaces.CopyTo(interfaceArray, 0);
					result = new ClassDescription(name, primaryInterface, baseClass, interfaceArray);
					classDescriptions.Add(result);
				}
				return result;
			}
		}
		private static ClassDescription FindClassDescription(String name, Type primaryInterface, Type baseClass, ICollection<Type> interfaces) {
			ClassDescription result = null;
			foreach(ClassDescription classDescription in classDescriptions) {
				if(classDescription.Name == name 
					&& classDescription.PrimaryInterface == primaryInterface
					&& classDescription.BaseClass == baseClass 
					&& classDescription.Interfaces.Length == interfaces.Count) {
					bool areInterfacesEqual = true;
					foreach(Type interfaceType in classDescription.Interfaces) {
						if(!interfaces.Contains(interfaceType)) {
							areInterfacesEqual = false;
							break;
						}
					}
					if(areInterfacesEqual) {
						result = classDescription;
						break;
					}
				}
			}
			return result;
		}
		private ClassDescription(String name, Type primaryInterface, Type baseClass, Type[] interfaces) {
			this.name = name;
			this.primaryInterface = primaryInterface;
			this.baseClass = baseClass;
			this.interfaces = interfaces;
		}
		public String Name {
			get { return name; }
		}
		public Type PrimaryInterface {
			get { return primaryInterface; }
		}
		public Type BaseClass {
			get { return baseClass; }
		}
		public Type[] Interfaces {
			get { return interfaces; }
		}
	}
	public sealed class ClassDescriptionCollection : IEnumerable<ClassDescription> {
		private readonly List<ClassDescription> innerCollection;
		public ClassDescriptionCollection() {
			innerCollection = new List<ClassDescription>();
		}
		public void Add(String name, Type primaryInterface, Type baseClass, Type[] interfaces) {
			ClassDescription newItem = ClassDescription.GetClassDescription(name, primaryInterface, baseClass, interfaces);
			innerCollection.Add(newItem);
		}
		public ClassDescriptionCollection Clone() {
			ClassDescriptionCollection result = new ClassDescriptionCollection();
			result.innerCollection.AddRange(innerCollection);
			return result;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(this != obj) {
				ClassDescriptionCollection other = obj as ClassDescriptionCollection;
				return other != null && AreCollectionsEqual(innerCollection, other.innerCollection);
			}
			return true;
		}
		private static bool AreCollectionsEqual(ICollection<ClassDescription> collection1, ICollection<ClassDescription> collection2) {
			if(collection1.Count != collection2.Count) {
				return false;
			}
			foreach(ClassDescription item in collection1) {
				if(!collection2.Contains(item)) {
					return false;
				}
			}
			return true;
		}
		IEnumerator<ClassDescription> IEnumerable<ClassDescription>.GetEnumerator() {
			return innerCollection.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerCollection.GetEnumerator();
		}
#if DebugTest
		public void DebugTest_Add(String name, Type baseClass, Type[] interfaces) {
			Add(name, interfaces[0], baseClass, interfaces);
		}
#endif
	}
	public class EasyTypeGenerator : CodeGeneratorLogicClassesFinder {
		private readonly ClassDescriptionCollection classDescriptions;
		public EasyTypeGenerator(ClassDescriptionCollection classDescriptions, CustomLogics customLogics)
			: base(customLogics) {
			this.classDescriptions = classDescriptions;
		}
		public String GenerateCode(out String[] references) {
			EasyClassInfo[] classInfos = GetClassesInfos(classDescriptions);
			return GetCode(classInfos, out references);
		}
		#region class EasyCodeGenerator
		private EasyClassInfo[] GetClassesInfos(ClassDescriptionCollection classDescriptions) {
			List<EasyClassInfo> result = new List<EasyClassInfo>();
			foreach(ClassDescription modelClassDeclaration in classDescriptions) {
				EasyClassInfo classInfo = CreateClassInfo(modelClassDeclaration);
				result.Add(classInfo);
			}
			return result.ToArray();
		}
		private EasyClassInfo CreateClassInfo(ClassDescription modelClassDeclaration) {
			Type baseClass = GetCorrectGenericBaseClass(modelClassDeclaration.BaseClass, modelClassDeclaration.Interfaces);
			EasyClassInfo result = new EasyClassInfo(this);
			result.Create(modelClassDeclaration.Name, baseClass, modelClassDeclaration.Interfaces);
			return result;
		}
		private static Type GetCorrectGenericBaseClass(Type baseClass, Type[] interfaces) {
			if(baseClass == null || !baseClass.IsGenericType) {
				return baseClass;
			}
			Type[] gAgrsBase = baseClass.GetGenericArguments();
			if(gAgrsBase.Length != 1 || !gAgrsBase[0].Equals(typeof(object))) {
				return baseClass;
			}
			foreach(Type rootType in interfaces) {
				List<Type> allTypes = new List<Type>(rootType.GetInterfaces());
				allTypes.Add(rootType);
				foreach(Type tp in allTypes)
					if(tp.IsGenericType) {
						Type[] gAgrs = tp.GetGenericArguments();
						if(gAgrs.Length == 1) {
							return baseClass.GetGenericTypeDefinition().MakeGenericType(gAgrs[0]);
						}
					}
			}
			return null;
		}
		private string GetCode(EasyClassInfo[] classInfos, out string[] references) {
			StringBuilder stringBuilder = new StringBuilder();
			BeforeGetCode(stringBuilder);
			ReferencesCollector referencesCollector = new ReferencesCollector();
			foreach(EasyClassInfo classInfo in classInfos) {
				string currentClassCode = GetClassCode(classInfo);
				referencesCollector.Add(UsedTypes);
				foreach(Type implementedInterface in classInfo.ImplementedInterfaces) {
					referencesCollector.Add(implementedInterface.GetInterfaces());
				}
				stringBuilder.Append(currentClassCode);
				stringBuilder.AppendLine();
			}
			stringBuilder.Insert(0, GetArrangingUsings(referencesCollector.Usings));
			references = referencesCollector.References;
			AfterGetCode(stringBuilder);
			return stringBuilder.ToString();
		}
		private string GetArrangingUsings(string[] usings) {
			StringBuilder result = new StringBuilder();
			foreach(string curUsing in usings) {
				if(!string.IsNullOrEmpty(curUsing)) {
					result.AppendFormat("using {0};", curUsing).AppendLine();
				}
			}
			result.AppendLine();
			return result.ToString();
		}
		protected virtual void BeforeGetCode(StringBuilder sb) {
		}
		protected virtual void AfterGetCode(StringBuilder sb) {
		}
		#endregion
		#region class EasyClassCode
		private readonly ICollection<Type> usedTypes = new HashSet<Type>();
		private string className = string.Empty;
		private string classDeclare = string.Empty;
		private readonly Dictionary<string, string> fields = new Dictionary<string, string>();
		private readonly Dictionary<string, string> properties = new Dictionary<string, string>();
		private readonly Dictionary<string, string> methods = new Dictionary<string, string>();
		private readonly Dictionary<string, string> constructors = new Dictionary<string, string>();
		private readonly Dictionary<string, string> subsidiaryFields = new Dictionary<string, string>();
		private readonly Dictionary<string, string> subsidiaryFieldsInitializerInConstructor = new Dictionary<string, string>();
		private readonly List<string> subsidiaryMethods = new List<string>();
		public string GetClassCode(EasyClassInfo classInfo) {
			ClearAll();
			CreateAll(classInfo);
			return GetArrangingClassCode();
		}
		private void ClearAll() {
			properties.Clear();
			methods.Clear();
			fields.Clear();
			constructors.Clear();
			subsidiaryFields.Clear();
			subsidiaryFieldsInitializerInConstructor.Clear();
			subsidiaryMethods.Clear();
			usedTypes.Clear();
		}
		private string GetArrangingClassCode() {
			StringBuilder result = new StringBuilder();
			result.Append(classDeclare);
			result.Append(" {");
			result.AppendLine();
			result.Append(GetArrangingItems(fields.Values));
			result.Append(GetArrangingItems(subsidiaryFields.Values));
			result.Append(GetArrangingItems(properties.Values));
			result.Append(GetArrangingItems(constructors.Values));
			result.Append(GetArrangingItems(methods.Values));
			result.Append(GetArrangingItems(subsidiaryMethods));
			result.AppendLine();
			result.Append("}");
			return result.ToString();
		}
		private string GetArrangingItems(ICollection<string> items) {
			StringBuilder result = new StringBuilder();
			foreach(string item in items) {
				result.Append(item);
				result.AppendLine();
			}
			return result.ToString();
		}
		protected virtual void CreateAll(EasyClassInfo classInfo) {
			className = classInfo.Name;
			classDeclare = GetClassDeclare(classInfo);
			CreateMembersCode(classInfo.properties.ToArray(), classInfo);
			foreach(FieldInfo fi in classInfo.Fields) {
				if(!subsidiaryFields.ContainsKey(fi.Name)) {
					subsidiaryFields.Add(fi.Name, GetFieldCodeInit(fi));
				}
			}
			CreateMethodsOrConstructorsCode(classInfo.constructors.ToArray(), classInfo);
			CreateMethodsOrConstructorsCode(classInfo.Methods, classInfo);
		}
		private string GetClassDeclare(EasyClassInfo classInfo) {
			string result = string.Empty;
			if(AttributeHelper.GetAttributes<DomainComponentAttribute>(classInfo.ImplementedInterfaces[0], true).Length > 0) {
				AddUsedType(typeof(DomainComponentAttribute));
				result = string.Format("[{0}]\r\n", TypeToString(typeof(DomainComponentAttribute)));
			}
			result += string.Format("public class {0} : {1}", className, GetClassParents(classInfo));
			return result;
		}
		private string GetClassParents(EasyClassInfo classInfo) {
			List<string> result = new List<string>();
			if(classInfo.BaseClass != null) {
				result.Add(TypeToString(classInfo.BaseClass));
			}
			foreach(Type parent in classInfo.ImplementedInterfaces) {
				result.Add(TypeToString(parent));
			}
			return string.Join(", ", result.ToArray());
		}
		protected virtual void CreateMembersCode(MemberInfo[] membersInfo, EasyClassInfo classInfo) {
			IDictionary<string, int> allMembers = new Dictionary<string, int>();
			foreach(MemberInfo member in GetAllMembers(membersInfo, classInfo)) {
				if(!allMembers.ContainsKey(member.Name)) {
					allMembers[member.Name] = 1;
				}
				else {
					allMembers[member.Name]++;
				}
			}
			foreach(MemberInfo member in membersInfo) {
				if(member is PropertyInfo) {
					string currentPropertyCode = GetPropertyCode((PropertyInfo)member, classInfo, IsDuplicatedMember(allMembers, member));
					if(!string.IsNullOrEmpty(currentPropertyCode)) {
						properties.Add(member.DeclaringType.Name + "." + member.Name, currentPropertyCode);
					}
					else {
						allMembers[member.Name]--;
					}
				}
				if(member is FieldInfo) {
					fields.Add(member.Name, GetFieldCode((FieldInfo)member));
				}
			}
		}
		private IEnumerable<MemberInfo> GetAllMembers(MemberInfo[] membersInfo, EasyClassInfo classInfo) {
			List<MemberInfo> allMembers = new List<MemberInfo>(membersInfo);
			if(classInfo.BaseClass != null) {
				allMembers.AddRange(classInfo.BaseClass.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
			}
			return allMembers;
		}
		private bool IsDuplicatedMember(IDictionary<string, int> allMembers, MemberInfo member) {
			return allMembers[member.Name] > 1;
		}
		private string GetPropertyCode(PropertyInfo property, EasyClassInfo classInfo, bool isExplicitly) {
			AddUsedType(property.PropertyType);
			string attributes = string.Empty;
			foreach(object curAttribute in property.GetCustomAttributes(true)) {
				AddUsedType(curAttribute.GetType());
				string attributeDeclaration = GetAttributeDeclaration(curAttribute);
				if(!string.IsNullOrEmpty(attributeDeclaration)) {
					attributes += string.Format("\t[{0}]\r\n", attributeDeclaration);
				}
			}
			string propertyAccessType = isExplicitly ? string.Empty : "public";
			string propertyType = TypeToString(property.PropertyType);
			string propertyName = GetPropertyNameDeclaring(property, isExplicitly);
			if(string.IsNullOrEmpty(propertyName)) {
				return string.Empty;
			}
			string getter = property.CanRead ? 
				string.Format("\r\n\t\tget {{ {0} }}", GetPropertyGetterRoot(property, classInfo)) : string.Empty;
			string setter = property.CanWrite ? 
				string.Format("\r\n\t\tset {{ {0} }}", GetPropertySetterRoot(property, classInfo)) : string.Empty;
			string result = string.Format("\t{0} {1} {2} {{{3}{4}\r\n\t}}", propertyAccessType, propertyType, propertyName, getter, setter);
			return attributes + result;
		}
		protected virtual string GetAttributeDeclaration(object attribute) {
			if(attribute == null) {
				return string.Empty;
			}
			Type attributeType = attribute.GetType();
			if(attributeType.Equals(typeof(DomainComponentAttribute))) {
				return TypeToString(attributeType);
			}
			if(attributeType.Equals(typeof(LocalizableAttribute))) {
				bool isLocalizable = ((LocalizableAttribute)attribute).IsLocalizable;
				return string.Format("{0}({1})", TypeToString(attributeType), BooleanToString(isLocalizable));
			}
			if(attributeType.Equals(typeof(CategoryAttribute))) {
				string category = ((CategoryAttribute)attribute).Category;
				return string.Format("{0}({1})", TypeToString(attributeType), GetEscapedString(category));
			}
			if(attributeType.Equals(typeof(EditorAttribute))) {
				EditorAttribute editorAttribute = (EditorAttribute)attribute;
				return string.Format(@"{0}(""{1}"", ""{2}"")", TypeToString(attributeType), editorAttribute.EditorTypeName, editorAttribute.EditorBaseTypeName);
			}
			if(attributeType.Equals(typeof(RefreshPropertiesAttribute))) {
				string arg = ((RefreshPropertiesAttribute)attribute).RefreshProperties.ToString();
				return string.Format(@"{0}({1}.{2})", TypeToString(attributeType), TypeToString(typeof(RefreshProperties)), arg);
			}
			if(attributeType.Equals(typeof(TypeConverterAttribute))) {
				string arg = ((TypeConverterAttribute)attribute).ConverterTypeName;
				return string.Format(@"{0}(""{1}"")", TypeToString(attributeType), arg);
			}
			if(attributeType.Equals(typeof(DisplayNameAttribute))) {
				string arg = ((DisplayNameAttribute)attribute).DisplayName;
				return string.Format("{0}({1})", TypeToString(attributeType), GetEscapedString(arg));
			}
			return string.Empty;
		}
		protected virtual string GetPropertyNameDeclaring(PropertyInfo property, bool isExplicitly) {
			string fullPathPrefix = isExplicitly ? TypeToString(property.DeclaringType) + "." : string.Empty;
			ParameterInfo[] parameters = property.GetIndexParameters();
			if(parameters.Length > 0) {
				string paramDeclaring = GetParametersDeclaring(parameters);
				return string.Format("{0}this[{1}]", fullPathPrefix, paramDeclaring);
			}
			return string.Format("{0}{1}", fullPathPrefix, property.Name);
		}
		private string GetPropertyGetterRoot(PropertyInfo property, EasyClassInfo classInfo) {
			string result = string.Empty;
			MethodInfo logicMethodBeforeGet = classInfo.GetLogicMethod(string.Format("{0}_{1}", SpecificWords.LogicBeforeGet, property.Name), property.DeclaringType);
			if(logicMethodBeforeGet == null) {
				logicMethodBeforeGet = classInfo.GetLogicMethod(SpecificWords.LogicBeforeGet, property.DeclaringType);
			}
			if(logicMethodBeforeGet != null) {
				string beforeGetCall = GetBeforeGetMethodCall(logicMethodBeforeGet, property);
				if(!string.IsNullOrEmpty(beforeGetCall)) {
					result = beforeGetCall + "\r\n\t\t\t";
				}
			}
			MethodInfo logicMethod = classInfo.GetLogicMethod(new string[] { SpecificWords.LogicPrefixGetStatic + property.Name, SpecificWords.LogicPrefixGetNonStatic + property.Name }, property.DeclaringType);
			if(logicMethod == null) {
				logicMethod = classInfo.GetLogicMethod(SpecificWords.LogicUniversalGet, property.DeclaringType);
			}
			if(logicMethod != null) {
				result += GetPropertyGetterWithLogic(property, classInfo, logicMethod);
			}
			else {
				result += GetPropertyGetter(property, classInfo);
			}
			return result;
		}
		private string GetBeforeGetMethodCall(MethodInfo method, PropertyInfo property) {
			return GetBeforeGetOrSetMethodCall(method, property, true);
		}
		private string GetPropertyGetterWithLogic(PropertyInfo property, EasyClassInfo classInfo, MethodInfo logicMethod) {
			ParameterInfo[] logicMethodParameters = logicMethod.GetParameters();
			if(logicMethodParameters.Length > 2) {
				throw new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, logicMethod.ReflectedType.Name, logicMethod.Name));
			}
			string typeCast = string.Empty;
			string logicParameter = logicMethodParameters.Length > 0 ? "this" : string.Empty;
			if(logicMethodParameters.Length == 2) {
				if(logicMethod.ReturnType != typeof(object) || !logicMethodParameters[0].ParameterType.IsAssignableFrom(property.DeclaringType) || logicMethodParameters[1].ParameterType != typeof(string)) {
					throw new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, logicMethod.DeclaringType.Name, logicMethod.Name));
				}
				typeCast = string.Format("({0})", TypeToString(property.PropertyType));
				logicParameter += string.Format(" ,\"{0}\"", property.Name);
			}
			string result;
			if(logicMethod.IsStatic) {
				result = string.Format("return {0}{1}.{2}({3});", typeCast, TypeToString(logicMethod.ReflectedType), logicMethod.Name, logicParameter);
				result = GetCustomPropertyGetterWithLogic(property, classInfo, result, true);
			}
			else if(!logicMethod.IsSpecialName) {
				string logicInstanceName = GetLogicInstanceName(logicMethod.ReflectedType);
				result = string.Format("return {0}{1}.{2}({3});", typeCast, logicInstanceName, logicMethod.Name, logicParameter);
				result = GetCustomPropertyGetterWithLogic(property, classInfo, result, false);
			}
			else {
				string logicInstanceName = GetLogicInstanceName(logicMethod.ReflectedType);
				result = string.Format("return {0}{1}.{2};", typeCast, logicInstanceName, property.Name);
				result = GetCustomPropertyGetterWithLogic(property, classInfo, result, false);
			}
			return result;
		}
		protected virtual string GetCustomPropertyGetterWithLogic(PropertyInfo property, EasyClassInfo classInfo, string generatedBody, bool isStatic) {
			return generatedBody;
		}
		protected virtual string GetPropertyGetter(PropertyInfo property, EasyClassInfo classInfo) {
			return string.Format("return Node.GetAttribute(\"{0}\").Value;", property.Name);
		}
		private string GetPropertySetterRoot(PropertyInfo property, EasyClassInfo classInfo) {
			string result = string.Empty;
			MethodInfo logicMethodBeforeSet = classInfo.GetLogicMethod(string.Format("{0}_{1}", SpecificWords.LogicBeforeSet, property.Name), property.DeclaringType);
			if(logicMethodBeforeSet == null) {
				logicMethodBeforeSet = classInfo.GetLogicMethod(SpecificWords.LogicBeforeSet, property.DeclaringType);
			}
			string beforeSetCall = string.Empty;
			if(logicMethodBeforeSet != null) {
				beforeSetCall = GetBeforeSetMethodCall(logicMethodBeforeSet, property);
			}
			if(!string.IsNullOrEmpty(beforeSetCall)) {
				result = beforeSetCall + "\r\n\t\t\t";
			}
			MethodInfo logicMethod = classInfo.GetLogicMethod(new string[] { SpecificWords.LogicPrefixSetStatic + property.Name, SpecificWords.LogicPrefixSetNonStatic + property.Name }, property.DeclaringType);
			if(logicMethod != null) {
				result += GetPropertySetterWithLogic(property, logicMethod);
			}
			else {
				result += GetPropertySetter(property);
			}
			return result;
		}
		private string GetBeforeSetMethodCall(MethodInfo method, PropertyInfo property) {
			return GetBeforeGetOrSetMethodCall(method, property, false);
		}
		private string GetBeforeGetOrSetMethodCall(MethodInfo method, PropertyInfo property, bool isGet) {
			string beforeDot = method.IsStatic ? TypeToString(method.DeclaringType) : GetLogicInstanceName(method.ReflectedType);
			InvalidOperationException exception = new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, method.ReflectedType.Name, method.Name));
			string parameterValue = isGet ? string.Empty : ", value";
			ParameterInfo[] parameters = method.GetParameters();
			string parameterPropertyName = string.Format(", \"{0}\"", property.Name);
			if(parameters.Length < 1) {
				throw exception;
			}
			if(isGet) {
				if(method.Name == SpecificWords.LogicBeforeGet) {
					if(parameters.Length != 2 || parameters[1].ParameterType != typeof(string)) {
						throw new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, method.ReflectedType.Name, method.Name));
					}
				}
				else {
					if(parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(property.DeclaringType)) {
						throw new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, method.ReflectedType.Name, method.Name));
					}
					parameterPropertyName = string.Empty;
				}
			}
			else {
				if(method.Name == SpecificWords.LogicBeforeSet) {
					if(parameters.Length != 3 || parameters[1].ParameterType != typeof(string) || parameters[2].ParameterType != typeof(object)) {
						throw exception;
					}
				}
				else {
					parameterPropertyName = string.Empty;
					if(parameters.Length != 2 || !parameters[0].ParameterType.IsAssignableFrom(property.DeclaringType) || parameters[1].ParameterType != typeof(object)) {
						throw exception;
					}
				}
			}
			return string.Format("{0}.{1}(this{2}{3});", beforeDot, method.Name, parameterPropertyName, parameterValue);
		}
		private string GetPropertySetterWithLogic(PropertyInfo property, MethodInfo logicMethod) {
			ParameterInfo[] logicMethodParameters = logicMethod.GetParameters();
			if(logicMethodParameters.Length < 1 || logicMethodParameters.Length > 2) {
				throw new InvalidOperationException(string.Format(HelperTypeGenerator.IncorrectParametersMessageFormat, logicMethod.DeclaringType.Name, logicMethod.Name));
			}
			string result;
			if(logicMethod.IsStatic) {
				string firstParameter = logicMethodParameters.Length == 2 ? "this, " : string.Empty;
				result = string.Format("{0}.{1}({2}value);", TypeToString(logicMethod.ReflectedType), logicMethod.Name, firstParameter);
			}
			else {
				string logicInstanceName = GetLogicInstanceName(logicMethod.ReflectedType);
				result = string.Format("{0}.{1} = value;", logicInstanceName, property.Name);
			}
			return result;
		}
		protected virtual string GetPropertySetter(PropertyInfo property) {
			return string.Format("Node.SetAttribute(\"{0}\", value);", property.Name);
		}
		private string GetFieldCode(FieldInfo field) {
			AddUsedType(field.FieldType);
			return string.Format("\t{0} {1};", TypeToString(field.FieldType), field.Name);
		}
		private string GetFieldCodeInit(FieldInfo field) {
			ConstructorInfo[] logicConstructors = field.FieldType.GetConstructors();
			foreach(ConstructorInfo currentlogicConstructor in logicConstructors) {
				ParameterInfo[] currentParameters = currentlogicConstructor.GetParameters();
				string fieldTypeFullName = TypeToString(field.FieldType);
				if(currentParameters.Length == 0) {
					return string.Format("\t{0} {1} = new {0}();", fieldTypeFullName, field.Name);
				}
				if(currentParameters.Length == 1) {
					CreateInitializerSubsidiaryFieldInConstructor(field.Name, string.Format("{0} = new {1}(this);", field.Name, fieldTypeFullName));
					return string.Format("\t{0} {1};", fieldTypeFullName, field.Name);
				}
			}
			throw new InvalidOperationException(string.Format("The {0} logic class doesn't expose a constructor with the required parameters.", field.FieldType.Name));
		}
		private void CreateInitializerSubsidiaryFieldInConstructor(string fieldName, string initializer) {
			if(!subsidiaryFieldsInitializerInConstructor.ContainsKey(fieldName)) {
				subsidiaryFieldsInitializerInConstructor.Add(fieldName, initializer);
			}
		}
		private void CreateMethodsOrConstructorsCode(MethodBase[] methodsOrConstructorsInfo, EasyClassInfo classInfo) {
			List<MethodBase> allMethods = new List<MethodBase>();
			allMethods.AddRange(methodsOrConstructorsInfo);
			if(classInfo.BaseClass != null) {
				allMethods.AddRange(classInfo.BaseClass.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static));
			}
			int counter = 0;
			foreach(MethodBase methodOrConstructor in methodsOrConstructorsInfo) {
				if(methodOrConstructor is MethodInfo) {
					methods.Add(methodOrConstructor.Name + counter++, GetMethodCode((MethodInfo)methodOrConstructor, classInfo, IsDuplicatedMethod(allMethods.ToArray(), methodOrConstructor)));
				}
				if(methodOrConstructor is ConstructorInfo) {
					constructors.Add(methodOrConstructor.DeclaringType.Name + counter++, GetConstructorCode((ConstructorInfo)methodOrConstructor, classInfo));
				}
			}
		}
		private bool IsDuplicatedMethod(MethodBase[] methods, MethodBase method) {
			for(int i = 0; i < methods.Length; i++) {
				if(methods[i].Name == method.Name && HelperTypeGenerator.AreParametersEqual(methods[i].GetParameters(), method.GetParameters())) {
					for(int j = i + 1; j < methods.Length; j++) {
						if(method.Name == methods[j].Name && HelperTypeGenerator.AreParametersEqual(method.GetParameters(), methods[j].GetParameters())) {
							return true;
						}
					}
				}
			}
			return false;
		}
		private string GetMethodCode(MethodInfo method, EasyClassInfo classInfo, bool isDuplicatedMethod) {
			AddUsedType(method.ReturnType);
			string methodAccessType = isDuplicatedMethod ? string.Empty : "public ";
			string staticDeclare = ((method is MethodInfoLite) && ((MethodInfoLite)method).IsStatic) ? "static " : string.Empty;
			string methodTypeFullName = TypeToString(method.ReturnType);
			string methodName = GetMethodName(method, isDuplicatedMethod);
			return string.Format("\t{0}{1}{2} {3}({4}){{\r\n\t\t{5}\r\n\t}}", methodAccessType, staticDeclare, methodTypeFullName, methodName, GetMethodParametersDeclaring(method), GetMethodBody(method, classInfo));
		}
		private string GetMethodName(MethodInfo method, bool isDuplicatedMethod) {
			string result = string.Empty;
			if(isDuplicatedMethod) {
				result = string.Format("{0}.{1}", TypeToString(method.DeclaringType), method.Name);
			}
			else {
				result = method.Name;
			}
			string genericPostfix = method.IsGenericMethod ? "<T>" : string.Empty;
			return string.Format("{0}{1}", result, genericPostfix);
		}
		private string GetMethodParametersDeclaring(MethodBase method) {
			return GetParametersDeclaring(method.GetParameters());
		}
		private string GetParametersDeclaring(ParameterInfo[] parameters) {
			if(parameters == null) {
				return string.Empty;
			}
			List<string> result = new List<string>();
			foreach(ParameterInfo curParameter in parameters) {
				AddUsedType(curParameter.ParameterType);
				result.Add(string.Format("{0} {1}", TypeToString(curParameter.ParameterType), curParameter.Name));
			}
			return string.Join(", ", result.ToArray());
		}
		private string GetMethodBody(MethodInfo method, EasyClassInfo classInfo) {
			if(method is MethodInfoLite && !string.IsNullOrEmpty(((MethodInfoLite)method).MethodBody)) {
				return ((MethodInfoLite)method).MethodBody;
			}
			ParameterInfo[] pInfos = method.GetParameters();
			Type[] parametersTypes = new Type[pInfos.Length];
			for(int i = 0; i < pInfos.Length; i++) {
				parametersTypes[i] = pInfos[i].ParameterType;
			}
			MethodInfo logicMethod = classInfo.GetLogicMethodForMethod(method.Name, parametersTypes, method.DeclaringType);
			if(logicMethod != null) {
				List<string> logicParameters = new List<string>();
				foreach(ParameterInfo parameter in method.GetParameters()) {
					logicParameters.Add(parameter.Name);
				}
				if(logicMethod.IsStatic) {
					logicParameters.Insert(0, "this");
					return string.Format("return {0}.{1}({2});", TypeToString(logicMethod.ReflectedType), GetMethodName(logicMethod, false), string.Join(", ", logicParameters.ToArray()));
				}
				else {
					string logicInstanceName = GetLogicInstanceName(logicMethod.ReflectedType);
					return string.Format("return {0}.{1}({2});", logicInstanceName, GetMethodName(logicMethod, false), string.Join(", ", logicParameters.ToArray()));
				}
			}
			return "throw new NotImplementedException();";
		}
		private string GetConstructorCode(ConstructorInfo constructor, EasyClassInfo classInfo) {
			string className = classInfo.Name;
			return string.Format("\tpublic {0}({1}) : base({2}){{ {3} }}", className, GetMethodParametersDeclaring(constructor), GetConstructorParametersToBase(constructor), GetConstructorBody(classInfo));
		}
		private string GetConstructorParametersToBase(ConstructorInfo constructor) {
			ParameterInfo[] parameters = constructor.GetParameters();
			string[] result = new string[parameters.Length];
			for(int i = 0; i < parameters.Length; i++) {
				result[i] = parameters[i].Name;
			}
			return string.Join(", ", result);
		}
		private string GetConstructorBody(EasyClassInfo classInfo) {
			string result = string.Empty;
			string subsidiaryFieldsInitializers = GetInitializersCodeForSubsidiaryFields();
			if(!string.IsNullOrEmpty(subsidiaryFieldsInitializers)) {
				result += subsidiaryFieldsInitializers + "\r\n\t";
			}
			string afterConstructionCallCode = GetAfterConstructionCode(classInfo);
			if(!string.IsNullOrEmpty(afterConstructionCallCode)) {
				result += afterConstructionCallCode + "\r\n\t";
			}
			string constructorInitBody = GetConstructorInitBody(classInfo);
			if(!string.IsNullOrEmpty(constructorInitBody)) {
				result += constructorInitBody + "\r\n\t";
			}
			return result;
		}
		private string GetInitializersCodeForSubsidiaryFields() {
			string subsidiaryFieldsInitializers = string.Empty;
			foreach(KeyValuePair<string, string> curSFI in subsidiaryFieldsInitializerInConstructor) {
				subsidiaryFieldsInitializers += "\r\n\t\t" + curSFI.Value;
			}
			return subsidiaryFieldsInitializers;
		}
		private string GetAfterConstructionCode(EasyClassInfo classInfo) {
			MethodInfo afterConstructionMethod = null;
			foreach(Type curType in classInfo.ImplementedInterfaces) {
				MethodInfo tmp = classInfo.GetLogicMethod(SpecificWords.LogicAfterConstruction, curType);
				if(afterConstructionMethod != null && tmp != null && !afterConstructionMethod.Equals(tmp)) {
					HelperTypeGenerator.ThrowInvalidOperationException(string.Format("Duplicate {0} logic methods have been found: {1}, {2}.", tmp.Name, tmp.ReflectedType.Name, afterConstructionMethod.ReflectedType.Name));
				}
				afterConstructionMethod = tmp;
			}
			string afterConstructionCallCode = string.Empty;
			if(afterConstructionMethod != null) {
				string parameter = afterConstructionMethod.GetParameters().Length == 1 ? "this" : string.Empty;
				if(afterConstructionMethod.IsStatic) {
					string afterConstructionTypeFullName = TypeToString(afterConstructionMethod.ReflectedType);
					afterConstructionCallCode = string.Format("\r\n\t\t{0}.{1}({2});", afterConstructionTypeFullName, afterConstructionMethod.Name, parameter);
				}
				else {
					string logicInstanceName = GetLogicInstanceName(afterConstructionMethod.ReflectedType);
					afterConstructionCallCode = string.Format("\r\n\t\t{0}.{1}({2});", logicInstanceName, afterConstructionMethod.Name, parameter);
				}
			}
			return afterConstructionCallCode;
		}
		protected virtual string GetConstructorInitBody(EasyClassInfo classInfo) {
			return string.Empty;
		}
		protected void CreateSubsidiaryMethod(MethodInfoLite method, EasyClassInfo classInfo) {
			CreateSubsidiaryMethod(method, classInfo, false);
		}
		protected void CreateSubsidiaryMethod(string name, EasyClassInfo classInfo, PropertyInfo property, string methodBody) {
			CreateSubsidiaryMethod(new MethodInfoLite(name, property.DeclaringType, property.PropertyType, null, methodBody), classInfo, false);
		}
		protected void CreateSubsidiaryMethod(MethodInfoLite method, EasyClassInfo classInfo, bool isExplicitly) {
			subsidiaryMethods.Add(GetMethodCode(method, classInfo, isExplicitly));
		}
		protected void CreateSubsidiaryField(Type type, string fieldName) {
			if(!subsidiaryFields.ContainsKey(fieldName)) {
				subsidiaryFields.Add(fieldName, GetFieldCode(new FieldInfoLite(fieldName, type, null)));
			}
		}
		protected void AddUsedType(Type type) {
			HelperTypeGenerator.AddUsedType(type, usedTypes);
		}
		protected string TypeToString(Type type) {
			return HelperTypeGenerator.TypeToString(type, usedTypes, true);
		}
		protected string GetEscapedString(string sourceString) {
			return string.Format(@"@""{0}""", sourceString.Replace("\"", "\"\""));
		}
		protected string GetLogicInstanceName(Type logicType) {
			return HelperTypeGenerator.GetLogicInstanceName(logicType, usedTypes);
		}
		protected string BooleanToString(bool value) {
			return value ? "true" : "false";
		}
		public ICollection<Type> UsedTypes {
			get { return usedTypes; }
		}
		#endregion
	}
	public class ReferencesCollector {
		private readonly Dictionary<String, Object> usings;
		private readonly Dictionary<String, Object> references;
		private readonly Dictionary<Type, Object> proccesedTypes;
		private readonly Dictionary<String, Object> proccesedAssemblies;
		private void AddUsingAndReference(Type type) {
			if(type != null && !proccesedTypes.ContainsKey(type)) {
				proccesedTypes.Add(type, null);
				AddUsing(type.Namespace);
				AddReference(type.Assembly);
			}
		}
		private void AddUsing(String namespaceString) {
			if(!string.IsNullOrEmpty(namespaceString) && !usings.ContainsKey(namespaceString)) {
				usings.Add(namespaceString, null);
			}
		}
		private void AddReference(Assembly assembly) {
			if(!proccesedAssemblies.ContainsKey(assembly.FullName)) {
				proccesedAssemblies.Add(assembly.FullName, null);
				if(AssemblyHelper.IsDynamic(assembly)) {
					throw new InvalidOperationException("Cannot reference Domain Components from a dynamic assembly.");
				}
				String location = assembly.Location;
				if(!references.ContainsKey(location)) {
					references.Add(location, null);
				}
			}
		}
		public ReferencesCollector() {
			usings = new Dictionary<String, Object>();
			references = new Dictionary<String, Object>();
			proccesedTypes = new Dictionary<Type, Object>();
			proccesedAssemblies = new Dictionary<String, Object>();
		}
		public void Add(Type type) {
			AddUsingAndReference(type);
		}
		public void Add(IEnumerable<Type> types) {
			if(types != null) {
				foreach(Type type in types) {
					Add(type);
				}
			}
		}
		public String[] Usings {
			get {
				String[] result = new String[usings.Keys.Count];
				usings.Keys.CopyTo(result, 0);
				return result;
			}
		}
		public String[] References {
			get {
				String[] result = new String[references.Keys.Count];
				references.Keys.CopyTo(result, 0);
				return result;
			}
		}
	}
	public abstract class ClassInfoBase {
		private string name;
		private Type baseClass;
		private Type[] implementedInterfaces;
		private Type[] logics;
		private readonly List<FieldInfo> fields = new List<FieldInfo>();
		internal List<PropertyInfo> properties = new List<PropertyInfo>();
		private readonly List<MethodInfo> methods = new List<MethodInfo>();
		internal List<ConstructorInfo> constructors = new List<ConstructorInfo>();
		private readonly ICodeGeneratorLogicClassesFinder logicFinder;
		protected ClassInfoBase(ICodeGeneratorLogicClassesFinder logicFinder) {
			this.logicFinder = logicFinder;
		}
		public string Name { get { return name; } }
		public Type BaseClass { get { return baseClass; } }
		public Type[] ImplementedInterfaces { get { return implementedInterfaces; } }
		public FieldInfo[] Fields { get { return fields.ToArray(); } }
		public MethodInfo[] Methods { get { return methods.ToArray(); } }
		public void Create(string name, Type baseClass, Type[] implementedInterfaces) {
			this.name = name;
			this.baseClass = baseClass;
			this.implementedInterfaces = implementedInterfaces;
			Initialize();
		}
		private void Initialize() {
			List<Type> processedInterfaces = new List<Type>();
			Dictionary<Type, Type[]> multiLogics = new Dictionary<Type, Type[]>();
			foreach(Type implementedInterface in ImplementedInterfaces) {
				if(!processedInterfaces.Contains(implementedInterface)) {
					List<Type> interfacesToProcess = new List<Type>();
					interfacesToProcess.Add(implementedInterface);
					interfacesToProcess.AddRange(implementedInterface.GetInterfaces());
					CollectLogics(multiLogics, interfacesToProcess);
					AddMembers(implementedInterface, multiLogics, processedInterfaces);
					processedInterfaces.AddRange(interfacesToProcess);
				}
			}
			List<Type> logicTypes = new List<Type>();
			foreach(Type[] logicType in multiLogics.Values) {
				logicTypes.AddRange(logicType);
			}
			logics = logicTypes.ToArray();
			if(BaseClass != null) {
				ConstructorInfo[] baseConstructors = BaseClass.GetConstructors();
				if(baseConstructors.Length > 0) {
					constructors.AddRange(baseConstructors);
				}
			}
			CheckPropertiesAggregated();
			CorrectMembers();
		}
		private void CollectLogics(Dictionary<Type, Type[]> multiLogics, List<Type> interfaces) {
			foreach(Type interfaceType in interfaces) {
				if(!multiLogics.ContainsKey(interfaceType)) {
					Type[] newLogics = logicFinder.FindLogics(interfaceType);
					if(newLogics != null && newLogics.Length > 0) {
						multiLogics.Add(interfaceType, newLogics);
					}
				}
			}
		}
		private void AddMembers(Type interfaceType, Dictionary<Type, Type[]> multiLogics, List<Type> processedInterfaces) {
			List<Type> interfacesToProcess = new List<Type>();
			interfacesToProcess.Add(interfaceType);
			interfacesToProcess.AddRange(interfaceType.GetInterfaces());
			if(BaseClass != null) {
				foreach(Type interfaceFromBaseClass in BaseClass.GetInterfaces()) {
					interfacesToProcess.Remove(interfaceFromBaseClass);
				}
			}
			foreach(Type interfaceToProcess in interfacesToProcess) {
				if(!processedInterfaces.Contains(interfaceToProcess)) {
					AddMembersForLogics(multiLogics, interfaceToProcess);
					AddMethods(interfaceToProcess);
					AddProperties(interfaceToProcess);
				}
			}
		}
		private void AddMembersForLogics(Dictionary<Type, Type[]> multiLogics, Type interfaceType) {
			List<Type> logicTypes = new List<Type>();
			Type[] logicsForInterface;
			if(multiLogics.TryGetValue(interfaceType, out logicsForInterface)) {
				logicTypes.AddRange(logicsForInterface);
			}
			foreach(Type customLogic in logicFinder.CustomLogics.GetRegisteredLogics(interfaceType)) {
				if(!logicTypes.Contains(customLogic)) {
					logicTypes.Add(customLogic);
				}
			}
			foreach(Type logicType in logicTypes) {
				AddIncludingMembers(logicType);
				if(!logicType.IsAbstract) {
					string logicFieldName = HelperTypeGenerator.GetLogicInstanceName(logicType);
					if(!IsFieldExists(fields, logicFieldName)) {
						FieldInfo logicField = new FieldInfoLite(logicFieldName, logicType, null);
						fields.Add(logicField);
					}
				}
			}
		}
		private void AddIncludingMembers(Type type) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
			foreach(MethodInfo mi in type.GetMethods(flags)) {
				if(!mi.IsSpecialName && mi.GetCustomAttributes(typeof(IncludeItemAttribute), true).Length != 0) {
					methods.Add(mi);
				}
			}
			foreach(PropertyInfo pi in type.GetProperties(flags)) {
				if(pi.GetCustomAttributes(typeof(IncludeItemAttribute), true).Length != 0) {
					properties.Add(pi);
				}
			}
		}
		private bool IsFieldExists(IEnumerable<FieldInfo> inFields, string fieldName) {
			foreach(FieldInfo fi in inFields) {
				if(fi.Name == fieldName) {
					return true;
				}
			}
			return false;
		}
		private void AddMethods(Type interfaceType) {
			List<MethodInfo> allMethods = new List<MethodInfo>();
			allMethods.AddRange(interfaceType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly));
			foreach(MethodInfo method in allMethods) {
				if(!method.IsSpecialName) {
					methods.Add(method);
				}
			}
		}
		private void AddProperties(Type interfaceType) {
			properties.AddRange(interfaceType.GetProperties());
		}
		private void CheckPropertiesAggregated() {
			foreach(PropertyInfo pi in properties) {
				if(pi.PropertyType.IsGenericType && pi.PropertyType.Name.StartsWith("IList`")) {
					continue;
				}
				if(pi.GetCustomAttributes(typeof(AggregatedAttribute), true).Length > 0 && !pi.CanWrite) {
					throw new InvalidOperationException(string.Format("The {0}.{1} property doesn't expose a setter. You cannot apply the Aggregated attribute to read-only properties.", pi.DeclaringType.Name, pi.Name));
				}
			}
		}
		protected virtual void CorrectMembers() { }
		public MethodInfo GetLogicMethod(string[] logicMethodNames, Type forInterface) {
			return GetLogicMethod(logicMethodNames, null, forInterface, false);
		}
		public MethodInfo GetLogicMethod(string logicMethodName, Type forInterface) {
			return GetLogicMethod(new string[] { logicMethodName }, null, forInterface, false);
		}
		public MethodInfo GetLogicMethodForMethod(string logicMethodName, Type[] parameterTypes, Type forInterface) {
			return GetLogicMethod(new string[] { logicMethodName }, parameterTypes, forInterface, true);
		}
		private MethodInfo GetLogicMethod(string[] logicMethodNames, Type[] parameterTypes, Type forInterface, bool isForMethod) {
			List<MethodInfo> foundMethods = GetAllLogicMethods(logicMethodNames, parameterTypes, forInterface, isForMethod);
			if(foundMethods.Count > 0) {
				List<MethodInfo> methodsToDelete = new List<MethodInfo>();
				Type maxInterface = FindInheritor(forInterface);
				foreach(MethodInfo mi in foundMethods) {
					Type curMethodForInterface = HelperTypeGenerator.GetForInterface(mi.ReflectedType, logicFinder.CustomLogics);
					if(!curMethodForInterface.Equals(maxInterface) && !curMethodForInterface.IsAssignableFrom(maxInterface)) {
						methodsToDelete.Add(mi);
					}
				}
				foreach(MethodInfo mi in methodsToDelete) {
					foundMethods.Remove(mi);
				}
			}
			if(foundMethods.Count == 0) {
				return null;
			}
			MethodInfoComparer comparer = new MethodInfoComparer(this);
			return comparer.GetMaxItem(foundMethods);
		}
		private List<MethodInfo> GetAllLogicMethods(string[] logicMethodNames, Type[] parameterTypes, Type forInterface, bool isForMethod) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			List<MethodInfo> foundMethods = new List<MethodInfo>();
			Dictionary<int, MethodInfo> allCustomMethods = new Dictionary<int, MethodInfo>();
			bool isCustomMethodFound = false;
			foreach(string logicMethodName in logicMethodNames) {
				Type[] useLogics = logicFinder.CustomLogics.GetRegisteredLogics(forInterface);
				if(useLogics.Length > 0) {
					for(int logicLevel = useLogics.Length - 1; logicLevel >= 0; logicLevel--) {
						MethodInfo method;
						if(isForMethod) {
							method = GetLogicMethodForMethod(logicMethodName, useLogics[logicLevel], parameterTypes, forInterface);
						}
						else {
							method = GetLogicMethod(logicMethodName, useLogics[logicLevel], parameterTypes);
						}
						if(method != null) {
							if(allCustomMethods.ContainsKey(logicLevel)) {
								string message = string.Format("Duplicate {0} logic methods have been found for {1} interface: {2}, {3}.", logicMethodName, forInterface.Name, allCustomMethods[logicLevel].ReflectedType.Name, method.ReflectedType.Name);
								HelperTypeGenerator.ThrowInvalidOperationException(message);
							}
							allCustomMethods.Add(logicLevel, method);
							isCustomMethodFound = true;
							break;
						}
					}
				}
				foreach(Type curLogic in logics) {
					if(logicFinder.CustomLogics.IsUnregisteredLogic(forInterface, curLogic)) {
						continue;
					}
					MethodInfo tmpLogicMethod;
					if(isForMethod) {
						tmpLogicMethod = GetLogicMethodForMethod(logicMethodName, curLogic, parameterTypes, forInterface);
					}
					else {
						tmpLogicMethod = GetLogicMethod(logicMethodName, curLogic, parameterTypes);
					}
					if(tmpLogicMethod != null && !foundMethods.Contains(tmpLogicMethod)) {
						bool isDuplicateInCustom = false;
						foreach(MethodInfo mi in allCustomMethods.Values) {
							Type tmpLogicMethodForInterface = ((DomainLogicAttribute)tmpLogicMethod.DeclaringType.GetCustomAttributes(typeof(DomainLogicAttribute), true)[0]).InterfaceType;
							if(logicFinder.CustomLogics.GetInterfaceByLogic(mi.DeclaringType).Equals(tmpLogicMethodForInterface)) {
								isDuplicateInCustom = true;
								break;
							}
						}
						if(!isDuplicateInCustom) {
							foundMethods.Add(tmpLogicMethod);
						}
					}
				}
			}
			if(isCustomMethodFound) {
				MethodInfo result = null;
				int maxLogicLevel = -1;
				foreach(int curLogicLevel in allCustomMethods.Keys) {
					if(curLogicLevel > maxLogicLevel) {
						result = allCustomMethods[curLogicLevel];
					}
				}
				foundMethods.Add(result);
			}
			List<MethodInfo> methodsToDelete = new List<MethodInfo>();
			if(foundMethods.Count != 0) {
				foreach(MethodInfo mi in foundMethods) {
					Type curMethodForInterface = HelperTypeGenerator.GetForInterface(mi.ReflectedType, logicFinder.CustomLogics);
					if(!forInterface.IsAssignableFrom(curMethodForInterface) && !curMethodForInterface.IsAssignableFrom(forInterface)) {
						methodsToDelete.Add(mi);
					}
				}
				foreach(MethodInfo mi in methodsToDelete) {
					foundMethods.Remove(mi);
				}
			}
			return foundMethods;
		}
		private MethodInfo GetLogicMethodForMethod(string logicMethodName, Type logic, Type[] parametersTypes, Type forInterface) {
			List<MethodInfo> foundMethods = new List<MethodInfo>();
			foreach(MethodInfo method in logic.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
				if(!method.Name.Equals(logicMethodName)) {
					continue;
				}
				List<Type> curMethodParametersTypesList = GetParametersTypes(method.GetParameters());
				int supplementaryParamsCount = curMethodParametersTypesList.Count - parametersTypes.Length;
				if(supplementaryParamsCount > 1 || supplementaryParamsCount < 0) {
					continue;
				}
				bool mainParametersEquals = true;
				for(int i = 0; i < parametersTypes.Length; i++) {
					if(!parametersTypes[i].Equals(curMethodParametersTypesList[i + supplementaryParamsCount])) {
						mainParametersEquals = false;
						break;
					}
				}
				if(!mainParametersEquals) {
					continue;
				}
				if(supplementaryParamsCount == 0) {
					foundMethods.Add(method);
					continue;
				}
				if(!forInterface.Equals(curMethodParametersTypesList[0])) {
					continue;
				}
				foundMethods.Add(method);
			}
			if(foundMethods.Count > 1) {
				HelperTypeGenerator.ThrowInvalidOperationException("");
			}
			if(foundMethods.Count == 0) {
				return null;
			}
			return foundMethods[0];
		}
		private List<Type> GetParametersTypes(ParameterInfo[] parameters) {
			List<Type> result = new List<Type>();
			foreach(ParameterInfo pi in parameters) {
				result.Add(pi.ParameterType);
			}
			return result;
		}
		private MethodInfo GetLogicMethod(string logicMethodName, Type logic, Type[] parametersTypes) {
			MethodInfo logicMethod = null;
			try {
				logicMethod = parametersTypes == null ? logic.GetMethod(logicMethodName) : logic.GetMethod(logicMethodName, parametersTypes);
			}
			catch(AmbiguousMatchException) {
				string message = string.Format("Several methods with the {0} name have been found in the {1} logic class.", logicMethodName, logic.Name);
				HelperTypeGenerator.ThrowInvalidOperationException(message);
			}
			return logicMethod;
		}
		private Type FindInheritor(Type type) {
			foreach(Type tp in ImplementedInterfaces) {
				if(type.IsAssignableFrom(tp)) {
					return tp;
				}
			}
			return null;
		}
		public Type GetForInterfaceToLogic(Type logic) {
			object[] tmpAtt = logic.GetCustomAttributes(typeof(DomainLogicAttribute), true);
			if(tmpAtt.Length == 0) {
				return logicFinder.CustomLogics.GetInterfaceByLogic(logic);
			}
			else {
				return ((DomainLogicAttribute)tmpAtt[0]).InterfaceType;
			}
		}
		public override string ToString() {
			return string.Format("{0} - {1}", base.ToString(), Name);
		}
	}
	public class EasyClassInfo : ClassInfoBase {
		public EasyClassInfo(ICodeGeneratorLogicClassesFinder logicFinder) : base(logicFinder) { }
		protected override void CorrectMembers() {
			List<PropertyInfo> propertiesToCollect = new List<PropertyInfo>();
			bool[] isCheckedIndexes = new bool[properties.Count];
			TypeComparer typeComparer = new TypeComparer();
			PropertyInfoComparer propertyInfoComparer = new PropertyInfoComparer();
			for(int i = 0; i < properties.Count; i++) {
				if(isCheckedIndexes[i]) {
					continue;
				}
				isCheckedIndexes[i] = true;
				List<PropertyInfo> foundHierarchyDuplicates = new List<PropertyInfo>();
				foundHierarchyDuplicates.Add(properties[i]);
				for(int j = i + 1; j < properties.Count; j++) {
					if(properties[j].GetIndexParameters().Length > 0 || isCheckedIndexes[j]) {
						continue;
					}
					if(properties[i].Name == properties[j].Name && typeComparer.IsInHierarchy(properties[i].DeclaringType, properties[j].DeclaringType)) {
						isCheckedIndexes[j] = true;
						foundHierarchyDuplicates.Add(properties[j]);
					}
				}
				foundHierarchyDuplicates.Sort(propertyInfoComparer);
				propertiesToCollect.Add(foundHierarchyDuplicates[0]);
			}
			properties = propertiesToCollect;
		}
	}
	class FieldInfoLite : FieldInfo {
		private readonly string name;
		private readonly Type fieldType;
		private readonly object[] customAttributes;
		public FieldInfoLite(string name, Type fieldType, object[] customAttributes) {
			this.name = name;
			this.fieldType = fieldType;
			this.customAttributes = customAttributes ?? new object[0];
		}
		public override string Name {
			get { return name; }
		}
		public override Type FieldType {
			get { return fieldType; }
		}
		public override object[] GetCustomAttributes(bool inherit) {
			return customAttributes;
		}
		public override Type DeclaringType {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override FieldAttributes Attributes {
			get { return FieldAttributes.Public; }
		}
		public override RuntimeFieldHandle FieldHandle {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public override object GetValue(object obj) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override bool IsDefined(Type attributeType, bool inherit) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override Type ReflectedType {
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
	public class ParameterInfoLite : ParameterInfo {
		private readonly string name;
		private readonly Type parameterType;
		public ParameterInfoLite(string name, Type parameterType) {
			this.name = name;
			this.parameterType = parameterType;
		}
		public override string Name {
			get { return name; }
		}
		public override Type ParameterType {
			get { return parameterType; }
		}
	}
	public class MethodInfoLite : MethodInfo {
		private readonly string name;
		private readonly ParameterInfo[] parameters;
		private readonly Type declaringType;
		private readonly string methodBody;
		private readonly Type returnType;
		private bool isStatic;
		public MethodInfoLite(string name, Type declaringType, Type returnType, ParameterInfo[] parameters, string methodBody) {
			this.name = name;
			this.declaringType = declaringType;
			this.parameters = parameters ?? new ParameterInfo[0];
			this.methodBody = methodBody;
			this.returnType = returnType;
		}
		public MethodInfoLite(string name, Type declaringType, Type returnType, ParameterInfo[] parameters, string methodBody, bool isStatic)
			: this(name, declaringType, returnType, parameters, methodBody) {
			this.isStatic = isStatic;
		}
		public override string Name {
			get { return name; }
		}
		public override ParameterInfo[] GetParameters() {
			return parameters;
		}
		public override Type DeclaringType {
			get { return declaringType; }
		}
		public string MethodBody {
			get { return methodBody; }
		}
		public override Type ReturnType {
			get { return returnType; }
		}
		new public bool IsStatic { 
			get { return isStatic; } 
			set { isStatic = value; } 
		}
		public override MethodInfo GetBaseDefinition() {
			throw new NotImplementedException();
		}
		public override ICustomAttributeProvider ReturnTypeCustomAttributes {
			get { throw new NotImplementedException(); }
		}
		public override MethodAttributes Attributes {
			get { throw new NotImplementedException(); }
		}
		public override MethodImplAttributes GetMethodImplementationFlags() {
			throw new NotImplementedException();
		}
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override RuntimeMethodHandle MethodHandle {
			get { throw new NotImplementedException(); }
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}
		public override object[] GetCustomAttributes(bool inherit) {
			throw new NotImplementedException();
		}
		public override bool IsDefined(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}
		public override Type ReflectedType {
			get { throw new NotImplementedException(); }
		}
	}
	class PropertyInfoLite : PropertyInfo {
		private readonly PropertyInfo propertyInfo;
		private readonly string name;
		private readonly Type propertyType;
		private readonly Type declaringType;
		private readonly Type reflectedType;
		private readonly bool canRead;
		private readonly bool canWrite;
		public PropertyInfoLite(PropertyInfo propertyInfo)
			: this(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType, propertyInfo.CanRead, propertyInfo.CanWrite) {
			this.propertyInfo = propertyInfo;
			this.reflectedType = propertyInfo.ReflectedType;
		}
		public PropertyInfoLite(string name, Type propertyType, Type declaringType, bool canRead, bool canWrite) {
			this.propertyInfo = null;
			this.name = name;
			this.propertyType = propertyType;
			this.declaringType = declaringType;
			reflectedType = declaringType;
			this.canRead = canRead;
			this.canWrite = canWrite;
		}
		public override PropertyAttributes Attributes {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public override bool CanRead {
			get { return canRead; }
		}
		public override bool CanWrite {
			get { return canWrite; }
		}
		public override Type PropertyType {
			get { return propertyType; }
		}
		public override Type DeclaringType {
			get { return declaringType; }
		}
		public override string Name {
			get { return name; }
		}
		public override object[] GetCustomAttributes(bool inherit) {
			return new object[0];
		}
		public override MethodInfo[] GetAccessors(bool nonPublic) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override MethodInfo GetGetMethod(bool nonPublic) {
			return null;
		}
		public override ParameterInfo[] GetIndexParameters() {
			return new ParameterInfo[0];
		}
		public override MethodInfo GetSetMethod(bool nonPublic) {
			return null;
		}
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			return new object[0];
		}
		public override bool IsDefined(Type attributeType, bool inherit) {
			throw new Exception("The method or operation is not implemented.");
		}
		public override Type ReflectedType {
			get { return reflectedType; }
		}
	}
	public class TypeComparer : IComparer<Type> {
		public int Compare(Type x, Type y) {
			int result = CompareMayNotHierarchy(x, y);
			return result == -10 ? 0 : result;
		}
		public int CompareMayNotHierarchy(Type x, Type y) {
			if(x.Equals(y)) {
				return 0;
			}
			if(x.IsAssignableFrom(y)) {
				return 1;
			}
			if(y.IsAssignableFrom(x)) {
				return -1;
			}
			return -10;
		}
		public bool IsInHierarchy(Type x, Type y) {
			return CompareMayNotHierarchy(x, y) != -10;
		}
	}
	public class MethodInfoComparer : IComparer<MethodInfo> {
		private readonly ClassInfoBase classInfo;
		public MethodInfoComparer() : this(null) { }
		public MethodInfoComparer(ClassInfoBase classInfo) {
			this.classInfo = classInfo;
		}
		public int Compare(MethodInfo x, MethodInfo y) {
			Type typeX = x.ReflectedType;
			Type typeY = y.ReflectedType;
			Type interfaceY = classInfo.GetForInterfaceToLogic(typeY);
			Type interfaceX = classInfo.GetForInterfaceToLogic(typeX);
			TypeComparer typeComparer = new TypeComparer();
			int result = typeComparer.Compare(interfaceX, interfaceY);
			if(result == 0) {
				result = typeComparer.Compare(typeX, typeY);
			}
			return result;
		}
		public MethodInfo GetMaxItem(List<MethodInfo> methods) {
			if(methods.Count == 1) {
				return methods[0];
			}
			MethodInfo maxMethod = methods[0];
			MethodInfo secondMaxMethod = methods[1];
			for(int i = 1;i < methods.Count;i++) {
				if(Compare(maxMethod, secondMaxMethod) == 1) {
					MethodInfo tmp = maxMethod;
					maxMethod = secondMaxMethod;
					secondMaxMethod = tmp;
				}
				if(i == methods.Count - 1) {
					break;
				}
				int compareResult = Compare(secondMaxMethod, methods[i + 1]);
				if(compareResult == 1) {
					secondMaxMethod = methods[i + 1];
				}
			}
			if(Compare(maxMethod, secondMaxMethod) == 0) {
				string message = string.Format("Duplicate {0} logic methods have been found: {1}, {2}.", maxMethod.Name, maxMethod.ReflectedType.Name, secondMaxMethod.ReflectedType.Name);
				HelperTypeGenerator.ThrowInvalidOperationException(message);
			}
			return maxMethod;
		}
		public List<MethodInfo> SortList(List<MethodInfo> methods) {
			List<MethodInfo> result = new List<MethodInfo>();
			MethodInfo[] tmpMethods = methods.ToArray();
			for(int i = 0;i < tmpMethods.Length - 1;i++) {
				int maxPosition = i;
				for(int j = i + 1;j < tmpMethods.Length;j++) {
					int compareResult = Compare(tmpMethods[maxPosition], tmpMethods[j]);
					if(compareResult == 0) {
						string message = string.Format("Duplicate {0} logic methods have been found: {1}, {2}.", tmpMethods[i].Name, tmpMethods[i].ReflectedType.Name, tmpMethods[j].ReflectedType.Name);
						HelperTypeGenerator.ThrowInvalidOperationException(message);
					}
					if(compareResult == 1) {
						maxPosition = j;
					}
				}
				if(maxPosition != i) {
					MethodInfo tmp = tmpMethods[i];
					tmpMethods[i] = tmpMethods[maxPosition];
					tmpMethods[maxPosition] = tmp;
				}
			}
			result.AddRange(tmpMethods);
			return result;
		}
	}
	public class PropertyInfoComparer : IComparer<PropertyInfo> {
		public int Compare(PropertyInfo x, PropertyInfo y) {
			TypeComparer typeComparer = new TypeComparer();
			return typeComparer.Compare(x.DeclaringType, y.DeclaringType);
		}
	}
}
