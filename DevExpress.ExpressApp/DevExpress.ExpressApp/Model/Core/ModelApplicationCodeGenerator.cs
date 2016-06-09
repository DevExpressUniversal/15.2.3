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
using System.Drawing;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.Core {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ModelApplicationCodeGenerator : EasyTypeGenerator {
		sealed class ModelDelegate {
			readonly string methodName;
			readonly string sourceNodePath;
			public ModelDelegate(string methodName, string sourceNodePath) {
				Guard.ArgumentNotNull(methodName, "methodName");
				this.methodName = methodName;
				this.sourceNodePath = sourceNodePath;
			}
			public string MethodName { get { return methodName; } }
			public string SourceNodePath { get { return sourceNodePath; } }
		}
		sealed class ModelCalculator {
			readonly Type type;
			readonly ModelDelegate modelDelegate;
			public ModelCalculator(Type type, ModelDelegate modelDelegate) {
				Guard.ArgumentNotNull(type, "className");
				this.type = type;
				this.modelDelegate = modelDelegate;
			}
			public Type Type { get { return type; } }
			public ModelDelegate ModelDelegate { get { return modelDelegate; } }
		}
		sealed class ModelDefaultValue {
			readonly Type valueType;
			readonly object value;
			public ModelDefaultValue(Type valueType, object value) {
				Guard.ArgumentNotNull(valueType, "valueType");
				Guard.ArgumentNotNull(value, "value");
				this.valueType = valueType;
				this.value = value;
			}
			public Type ValueType { get { return valueType; } }
			public object Value { get { return value; } }
		}
		readonly IDictionary<string, ModelNodeCodeGenerationInfo> generatorInfo;
		readonly ClassDescriptionCollection classDescriptions;
		readonly HashSet<Type> customInterfacesToGenerate;
		readonly IList<Type> genericModelNodeInterfaces;
		readonly ModelListNodesCodeGenerationHelper interfacesInfo;
		readonly ModelInterfaceExtenders interfaceExtenders;
		ModelNodeCodeGenerationInfo currentInfo;
		readonly IList<string> generatedMethods;
		internal ModelApplicationCodeGenerator(IEnumerable<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders)
			: base(classDescriptions, customLogics) {
			Guard.ArgumentNotNull(customInterfacesToGenerate, "customInterfacesToGenerate");
			Guard.ArgumentNotNull(interfaceExtenders, "interfaceExtenders");
			this.customInterfacesToGenerate = new HashSet<Type>(customInterfacesToGenerate);
			this.interfaceExtenders = interfaceExtenders;
			this.classDescriptions = classDescriptions;
			genericModelNodeInterfaces = new List<Type>();
			generatorInfo = new Dictionary<string, ModelNodeCodeGenerationInfo>();
			interfacesInfo = new ModelListNodesCodeGenerationHelper();
			generatedMethods = new List<string>();
		}
		private IDictionary<string, ModelNodeCodeGenerationInfo> GeneratorInfo { get { return generatorInfo; } }
		private IList<Type> GenericModelNodeInterfaces { get { return genericModelNodeInterfaces; } }
		private ModelListNodesCodeGenerationHelper InterfacesInfo { get { return interfacesInfo; } }
		private ModelNodeCodeGenerationInfo CurrentInfo { get { return currentInfo; } }
		private void CreateCurrentInfo(EasyClassInfo classInfo) {
			if(IsCreatorInfoInProccessing(classInfo)) {
				AddModelApplicationCreatorInfoBaseProtectedConstructor(classInfo);
			}
			else {
				RemoveObsoleteModelNodeConstructors(classInfo);
				currentInfo = new ModelNodeCodeGenerationInfo(classInfo.ImplementedInterfaces[0]);
				CreateNodeCreatorStaticMethod(currentInfo, classInfo);
				if(GeneratorInfo.ContainsKey(classInfo.Name)) {
					string newSourceInterface = currentInfo.ModelInterface.FullName;
					string existingSourceInterface = GeneratorInfo[classInfo.Name].ModelInterface.FullName;
					throw new ArgumentException(string.Format("An item with the '{0}' key has already been added ('{1}', '{2}')", classInfo.Name, newSourceInterface, existingSourceInterface));
				}
				GeneratorInfo.Add(classInfo.Name, currentInfo);
			}
		}
		private void AddModelApplicationCreatorInfoBaseProtectedConstructor(EasyClassInfo classInfo) {
			classInfo.constructors.AddRange(typeof(ModelApplicationCreatorInfoBase).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance));
		}
		private void RemoveObsoleteModelNodeConstructors(EasyClassInfo classInfo) {
			foreach(ConstructorInfo constructor in classInfo.constructors.ToArray()) {
				if(constructor.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0) {
					classInfo.constructors.Remove(constructor);
				}
			}
		}
		private void CreateNodeCreatorStaticMethod(ModelNodeCodeGenerationInfo info, EasyClassInfo classInfo) {
			string methodName = string.Format("{0}_CreateNode", classInfo.Name);
			StringBuilder methodBody = new StringBuilder().Append("return new ").Append(classInfo.Name).Append("(");
			ConstructorInfo constructor = classInfo.constructors[0];
			ParameterInfo[] parameters = constructor.GetParameters();
			ParameterInfoLite[] liteParameters = new ParameterInfoLite[parameters.Length];
			for(int i = 0; i < liteParameters.Length; ++i) {
				liteParameters[i] = new ParameterInfoLite(parameters[i].Name, parameters[i].ParameterType);
				methodBody.Append(parameters[i].Name).Append(", ");
			}
			methodBody.Remove(methodBody.Length - 2, 2).Append(");");
			MethodInfoLite method = new MethodInfoLite(methodName, info.ModelInterface, typeof(ModelNode), liteParameters, methodBody.ToString(), true);
			CreateSubsidiaryMethod(method, classInfo);
			info.CreatorMethodName = string.Format("{0}.{1}", classInfo.Name, methodName);
		}
		private bool IsCreatorInfoInProccessing(EasyClassInfo classInfo) {
			return classInfo.BaseClass == typeof(ModelApplicationCreatorInfoBase);
		}
		protected override void BeforeGetCode(StringBuilder sb) {
			ModelCustomInterfacesCodeGeneratorHelper.GenerateCode(customInterfacesToGenerate, sb, TypeToString, GetAttributeDeclaration);
		}
		private const string ModelNodelListCreatorClassName = "ModelNodeListCreator";
		protected override void AfterGetCode(StringBuilder sb) {
			sb.AppendFormat("public class {0} {{", ModelNodelListCreatorClassName).AppendLine();
			foreach(Type type in GenericModelNodeInterfaces) {
				sb.AppendFormat("\tpublic static {2} {0}({1} nodeInfo, string id) {{", GetGenericModelNodeCreatorName(type), typeof(ModelNodeInfo).FullName, typeof(ModelNode).FullName).AppendLine();
				sb.AppendFormat("\t\treturn new {0}(nodeInfo, id);", ModelApplicationCreator.GetModelClassName(type)).AppendLine();
				sb.AppendLine("\t}");
			}
			sb.AppendLine("}");
		}
		string GetGenericModelNodeCreatorName(Type type) {
			return type.GetGenericArguments()[0].Name + "_Creator";
		}
		protected override void CreateAll(EasyClassInfo classInfo) {
			CreateCurrentInfo(classInfo);
			base.CreateAll(classInfo);
		}
		protected override void CreateMembersCode(MemberInfo[] membersInfo, EasyClassInfo classInfo) {
			base.CreateMembersCode(membersInfo, classInfo);
				AddDomainLogicForIndexProperty(classInfo);
		}
		protected override string GetConstructorInitBody(EasyClassInfo classInfo) {
			generatedMethods.Clear();
			return IsCreatorInfoInProccessing(classInfo) ? GetCreatorInfo_ConstructorBody() : base.GetConstructorInitBody(classInfo);
		}
		protected override string GetPropertyNameDeclaring(PropertyInfo property, bool isDuplicated) {
			if(!isDuplicated) {
				return base.GetPropertyNameDeclaring(property, isDuplicated);
			}
			return string.Empty;
		}
		protected override string GetPropertyGetter(PropertyInfo property, EasyClassInfo classInfo) {
			FindDefaultValueForProperty(property, classInfo);
			AddXmlName(property);
			if(IsModelNodeProperty(property)) {
				if(IsGenericModelNodeType(property.PropertyType) && !GenericModelNodeInterfaces.Contains(property.PropertyType)) {
					GenericModelNodeInterfaces.Add(property.PropertyType);
				}
				CheckNodeProperty(property);
				AddChildNodeInfo(property);
				return string.Format(@"return ({0})this[""{1}""];", TypeToString(property.PropertyType), property.Name);
			}
			AddValueInfo(property,classInfo);
			return string.Format(@"return GetValue<{0}>(""{1}"");", TypeToString(property.PropertyType), property.Name);
		}
		protected override string GetCustomPropertyGetterWithLogic(PropertyInfo property,EasyClassInfo classInfo, string generatedBody, bool isStatic) {
			AddDefaultValue(property, classInfo, generatedBody, null);
			AddXmlName(property);
			AddValueInfo(property,classInfo);
			return string.Format(@"return GetValue<{0}>(""{1}"");", TypeToString(property.PropertyType), property.Name);
		}
		protected override string GetPropertySetter(PropertyInfo property) {
			return string.Format(@"SetValue<{0}>(""{1}"", value);", TypeToString(property.PropertyType), property.Name);
		}
		void AddValueInfo(PropertyInfo property, EasyClassInfo classInfo) {
			LocalizableAttribute localizableAttribute = GetAttribute<LocalizableAttribute>(property);
			bool isLocalizable = localizableAttribute != null ? localizableAttribute.IsLocalizable : false;
			bool isReadOnly = !property.CanWrite;
			DataSourcePropertyAttribute persistentAttribute = null;
			if(property.PropertyType.IsInterface || property.PropertyType.IsClass) {
				persistentAttribute = GetAttribute<DataSourcePropertyAttribute>(property);
			}
			string persistentPath = persistentAttribute != null ? persistentAttribute.DataSourceProperty : string.Empty;
			TypeConverterAttribute typeConverterAttribute = GetAttribute<TypeConverterAttribute>(property);
			string converterTypeName = null;
			if(typeConverterAttribute != null) {
				converterTypeName = typeConverterAttribute.ConverterTypeName;
			}
			ModelValueInfo valueInfo = new ModelValueInfo(property.Name, property.PropertyType, isLocalizable, isReadOnly, persistentPath, converterTypeName);
			if(CurrentInfo.ValueInfosByName.ContainsKey(property.Name)) {
				throw new Exception(string.Format("The class {0} has already property {1}. It is declared in the interface: {2}", classInfo.Name, property.Name, property.DeclaringType.FullName));
			}
			CurrentInfo.ValueInfosByName.Add(property.Name, valueInfo);
		}
		void AddChildNodeInfo(PropertyInfo property) {
			CurrentInfo.NodeInfos.Add(property);
		}
		string GetCreatorInfo_ConstructorBody() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendFormat("\t\t{0} nodesGenerator = null;", typeof(ModelNodesGeneratorBase).FullName).AppendLine();
			sb.AppendFormat("\t\t{0} nodeInfo = null;", typeof(ModelNodeInfo).FullName).AppendLine();
			sb.AppendLine();
			foreach(KeyValuePair<string, ModelNodeCodeGenerationInfo> pair in GeneratorInfo) {
				ModelNodeCodeGenerationInfo generatorInfo = pair.Value;
				sb.AppendFormat("\t\t//  {0}", pair.Key).AppendLine();
				CreatorInfo_ConstructorBody_Generators(sb, generatorInfo);
				KeyPropertyAttribute keyPropertyAttribute = GetAttribute<KeyPropertyAttribute>(generatorInfo.ModelInterface);
				string keyProperty = keyPropertyAttribute != null ? keyPropertyAttribute.KeyPropertyName : string.Empty;
				sb.AppendFormat("\t\tnodeInfo = AddNodeInfo({0}, typeof({1}), typeof({2}), nodesGenerator, \"{3}\");", generatorInfo.CreatorMethodName, pair.Key, TypeToString(generatorInfo.ModelInterface), keyProperty).AppendLine();
				CreatorInfo_ConstructorBody_DefaultValues(sb, pair.Key, generatorInfo);
				CreatorInfo_ConstructorBody_ValuesInfo(sb, generatorInfo);
				CreatorInfo_ConstructorBody_ChildrenInfo(sb, generatorInfo);
				CreatorInfo_ConstructorBody_XmlNames(sb, generatorInfo);
				sb.AppendLine();
			}
			foreach(Type genericType in GenericModelNodeInterfaces) {
				string nodeCreatorName = ModelNodelListCreatorClassName + '.' + GetGenericModelNodeCreatorName(genericType);
				sb.AppendFormat("\t\tnodeInfo = AddNodeInfo({0}, typeof({1}), typeof({2}), nodesGenerator, \"{3}\");", nodeCreatorName, ModelApplicationCreator.GetModelClassName(genericType), TypeToString(genericType), string.Empty).AppendLine();
				CreatorInfo_ConstructorBody_ListChildrenInfo(sb, genericType);
			}
			return sb.ToString();
		}
		Dictionary<string, string> GetListChildrenInfo(Type baseInterface) {
			Dictionary<string, string> result = new Dictionary<string, string>();
			Type listChildClass = InterfacesInfo.GetListNodesType(baseInterface);
			if(listChildClass != null) {
				ClassDescription imlementor = null;
				foreach(ClassDescription classDescription in GetImplementorDescriptions(listChildClass)) {
					Type primaryInterface = classDescription.PrimaryInterface;
					if(primaryInterface == listChildClass) {
						imlementor = classDescription;
					}
					ModelPersistentNameAttribute persistentNameAttribute = GetAttribute<ModelPersistentNameAttribute>(primaryInterface);
					string name = persistentNameAttribute != null && !result.ContainsKey(persistentNameAttribute.Name) ? persistentNameAttribute.Name : ModelApplicationCreator.GetDefaultXmlName(primaryInterface);
					result[name] = classDescription.Name;
				}
				if(imlementor != null && GetAttribute<ModelPersistentNameAttribute>(listChildClass) == null) {
					result["Item"] = imlementor.Name; 
				}
			}
			return result;
		}
		List<ClassDescription> GetImplementorDescriptions(Type type) {
			List<ClassDescription> result = new List<ClassDescription>();
			foreach(ClassDescription classDescription in classDescriptions) {
				if(IsAssignableFrom(type, classDescription.PrimaryInterface)) {
					result.Add(classDescription);
				}
			}
			return result;
		}
		bool IsAssignableFrom(Type baseType, Type type) {
			bool result = false;
			if(baseType.IsAssignableFrom(type) || type == baseType) {
				result = true;
			}
			else if(customInterfacesToGenerate.Contains(type)) {
				foreach(Type interfaceType in type.GetInterfaces()) {
					if(IsAssignableFrom(baseType, interfaceType)) {
						result = true;
						break;
					}
				}
			}
			return result;
		}
		void CreatorInfo_ConstructorBody_Generators(StringBuilder sb, ModelNodeCodeGenerationInfo generatorInfo) {
			ModelNodesGeneratorAttribute nodesGeneratorAttribute = GetAttribute<ModelNodesGeneratorAttribute>(generatorInfo.ModelInterface);
			string nodesGeneratorName = nodesGeneratorAttribute != null ? nodesGeneratorAttribute.TypeName : string.Empty;
			if(!string.IsNullOrEmpty(nodesGeneratorName)) {
				sb.AppendFormat("\t\tnodesGenerator = new {0}();", nodesGeneratorName).AppendLine();
				sb.AppendLine("\t\tAddNodeGenerator(nodesGenerator);");
			}
			else {
				if(GetAttribute<OmitDefaultGenerationAttribute>(generatorInfo.ModelInterface) != null) {
					sb.AppendLine("\t\tnodesGenerator = null;");
				}
				else {
					sb.AppendFormat("\t\tnodesGenerator = {0}.Instance;", typeof(ModelNodesDefaultInterfaceGenerator).FullName).AppendLine();
				}
			}
		}
		void CreatorInfo_ConstructorBody_DefaultValues(StringBuilder sb, string generatedClass, ModelNodeCodeGenerationInfo generatorInfo) {
			foreach(KeyValuePair<string, object> item in generatorInfo.DefaultValuesByName) {
				if(item.Value is ModelDelegate) {
					ModelDelegate modelDelegate = (ModelDelegate)item.Value;
					sb.AppendFormat("\t\tnodeInfo.AddDefaultValue(\"{0}\", {1}.{2}, {3});", item.Key, generatedClass, modelDelegate.MethodName, GetStringValue(typeof(string), modelDelegate.SourceNodePath)).AppendLine();
				}
				else if(item.Value is ModelCalculator) {
					ModelCalculator calculator = (ModelCalculator)item.Value;
					string methodInfoVar = string.Empty;
					if(calculator.ModelDelegate != null) {
						methodInfoVar = string.Format("{0}.{1}", generatedClass, calculator.ModelDelegate.MethodName);
					}
					sb.AppendFormat("\t\tnodeInfo.AddDefaultValue(\"{0}\", new {1}({2}));", item.Key, TypeToString(calculator.Type), methodInfoVar).AppendLine();
				}
				else if(item.Value is ModelDefaultValue) {
					ModelDefaultValue defaultValue = (ModelDefaultValue)item.Value;
					sb.AppendFormat("\t\tnodeInfo.AddDefaultValue(\"{0}\", {1});", item.Key, GetStringValue(defaultValue.ValueType, defaultValue.Value)).AppendLine();
				}
			}
		}
		void CreatorInfo_ConstructorBody_ValuesInfo(StringBuilder sb, ModelNodeCodeGenerationInfo generatorInfo) {
			foreach(KeyValuePair<string, ModelValueInfo> item in generatorInfo.ValueInfosByName) {
				ModelValueInfo value = item.Value;
				sb.AppendFormat("\t\tnodeInfo.AddValueInfo(\"{0}\", typeof({1}), {2}, {3}, \"{4}\", \"{5}\");",
					item.Key, TypeToString(value.PropertyType), GetStringValue(value.IsLocalizable),
					GetStringValue(value.IsReadOnly), value.PersistentPath, value.ConverterTypeName).AppendLine();
			}
		}
		void CreatorInfo_ConstructorBody_ChildrenInfo(StringBuilder sb, ModelNodeCodeGenerationInfo generatorInfo) {
			CreatorInfo_ConstructorBody_ListChildrenInfo(sb, generatorInfo.ModelInterface);
			foreach(PropertyInfo property in generatorInfo.NodeInfos) {
				string generatedClass = ModelApplicationCreator.GetModelClassName(property.PropertyType);
				bool isAutoGenerated = GetAttribute<OmitDefaultGenerationAttribute>(property) == null;
				sb.AppendFormat("\t\tnodeInfo.AddChildInfo(\"{0}\", typeof({1}), typeof({2}), {3});", property.Name, generatedClass, TypeToString(property.DeclaringType), GetStringValue(isAutoGenerated)).AppendLine();
			}
		}
		void CreatorInfo_ConstructorBody_ListChildrenInfo(StringBuilder sb, Type generatorBaseInterface) {
			foreach(KeyValuePair<string, string> item in GetListChildrenInfo(generatorBaseInterface)) {
				CreatorInfo_ConstructorBody_ListChildrenInfo(sb, item.Key, item.Value, TypeToString(generatorBaseInterface));
			}
		}
		void CreatorInfo_ConstructorBody_ListChildrenInfo(StringBuilder sb, string childName, string className, string ownerClassName) {
			sb.AppendFormat("\t\tnodeInfo.AddListChildInfo(\"{0}\", typeof({1}), typeof({2}));", childName, className, ownerClassName).AppendLine();
		}
		void CreatorInfo_ConstructorBody_XmlNames(StringBuilder sb, ModelNodeCodeGenerationInfo generatorInfo) {
			foreach(KeyValuePair<string, string> item in generatorInfo.ValueSerializableNamesByName) {
				sb.AppendFormat("\t\tnodeInfo.AddXmlName(\"{0}\", \"{1}\");", item.Key, item.Value).AppendLine();
			}
		}
		private T GetAttribute<T>(PropertyInfo property) where T : Attribute {
			T[] attributes = AttributeHelper.GetAttributes<T>(property, true);
			return attributes.Length > 0 ? attributes[0] : null;
		}
		private T GetAttribute<T>(Type baseInterface) where T : Attribute {
			T[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<T>(baseInterface, true);
			return attributes.Length > 0 ? attributes[0] : null;
		}
		private IEnumerable<T> GetAttributes<T>(IEnumerable<Type> baseTypes) where T : Attribute {
			List<T> result = new List<T>();
			foreach(Type baseType in baseTypes) {
				result.AddRange(AttributeHelper.GetAttributesConsideringInterfaces<T>(baseType, true));
			}
			return result.AsReadOnly();
		}
		private void AddXmlName(PropertyInfo property) {
			ModelPersistentNameAttribute attribute = GetAttribute<ModelPersistentNameAttribute>(property);
			if(attribute == null) return;
			CurrentInfo.ValueSerializableNamesByName[property.Name] = ((ModelPersistentNameAttribute)attribute).Name;
		}
		private void FindDefaultValueForProperty(PropertyInfo property, EasyClassInfo classInfo) {
			Type calculatorType;
			string linkValue, sourceNode;
			GetCalculatedValues(property, classInfo, out linkValue, out calculatorType, out sourceNode);
			if(calculatorType != null) {
				AddDefaultValue(property, calculatorType);
			}
			else {
				AddDefaultValue(property, classInfo, linkValue, sourceNode);
			}
			if(!CurrentInfo.DefaultValuesByName.ContainsKey(property.Name)) {
				object defaultValue = GetDefaultValue(property);
				AddDefaultValue(property, defaultValue);
			}
		}
		private void AddDefaultValue(PropertyInfo property, EasyClassInfo classInfo, string linkValue, string sourceNode) {
			ModelDelegate modelDelegate = null;
			if(!string.IsNullOrEmpty(linkValue)) {
				modelDelegate = CreateModelDelegate(property, classInfo, linkValue, sourceNode);
			}
			DataSourcePropertyAttribute persistentAttribute = GetAttribute<DataSourcePropertyAttribute>(property);
			IDictionary<string, object> defaultValues = CurrentInfo.DefaultValuesByName;
			if(persistentAttribute != null && !string.IsNullOrEmpty(persistentAttribute.DataSourceProperty)) {
				defaultValues.Add(property.Name, new ModelCalculator(typeof(ModelValuePersistentPathCalculator), modelDelegate));
			}
			else if(modelDelegate != null) {
				defaultValues.Add(property.Name, modelDelegate);
			}
		}
		private void AddDefaultValue(PropertyInfo property, Type calculatorType) {
			CurrentInfo.DefaultValuesByName.Add(property.Name, new ModelCalculator(calculatorType, null));
		}
		private void AddDefaultValue(PropertyInfo property, object defaultValue) {
			if(defaultValue != null) {
				CurrentInfo.DefaultValuesByName.Add(property.Name, new ModelDefaultValue(property.PropertyType, defaultValue));
			}
		}
		private ModelDelegate CreateModelDelegate(PropertyInfo property, EasyClassInfo classInfo, string linkValue, string sourceNode) {
			string methodName = GetMethodName(string.Format("Get{0}Calculator", property.Name));
			string staticMethodName = GetMethodName(string.Format("GetStatic{0}Calculator", property.Name));
			linkValue = linkValue.Trim();
			if(!linkValue.StartsWith("return")) {
				linkValue = "return " + linkValue;
			}
			if(!linkValue.EndsWith(";")) {
				linkValue += ';';
			}
			CreateSubsidiaryMethod(methodName, classInfo, property, linkValue);
			MethodInfoLite methodInfo = new MethodInfoLite(staticMethodName, property.DeclaringType,
				typeof(object), new ParameterInfo[] { new ParameterInfoLite("node", typeof(ModelNode)) },
				string.Format("return (({0})node).{1}();", classInfo.Name, methodName), true);
			CreateSubsidiaryMethod(methodInfo, classInfo);
			ModelDelegate modelDelegate = new ModelDelegate(staticMethodName, sourceNode);
			return modelDelegate;
		}
		string GetStringValue(object value) {
			return GetStringValue(value != null ? value.GetType() : null, value);
		}
		string GetStringValue(Type type, object value) {
			if(type == null || value == null) {
				return "null";
			}
			Type nullableType = Nullable.GetUnderlyingType(type);
			if(nullableType != null) {
				type = nullableType;
			}
			if(type == typeof(string)) {
				return string.Format(@"@""{0}""", ((string)value).Replace("\"", "\"\""));
			}
			if(type == typeof(Boolean)) {
				return (bool)value ? "true" : "false";
			}
			if(type.IsEnum) {
				if(value.GetType() == typeof(int)) {
					value = Enum.ToObject(type, (int)value);
				}
				return string.Format("{0}.{1}", TypeToString(type), value.ToString());
			}
			if(type.IsClass) {
				return string.Format("typeof({0})", TypeToString(value.GetType()));
			}
			if(type == typeof(Color) && value is Color) {
				Color color = (Color)value;
				if(color.IsNamedColor) {
					return string.Format("{0}.FromName(\"{1}\")", TypeToString(typeof(Color)), color.Name);
				}
				return string.Format("{0}.FromArgb({1}, {2}, {3}, {4})", TypeToString(typeof(Color)), color.A, color.R, color.G, color.B);
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "({0})({1})", TypeToString(type), value);
		}
		string GetDefaultStringValue(Type type) {
			return string.Format("default({0})", TypeToString(type));
		}
		string GetDefaultStringValue(PropertyInfo property) {
			return GetDefaultStringValue(property.PropertyType);
		}
		bool IsModelNodeProperty(PropertyInfo property) {
			return (ModelInterfacesHelper.IsTypeSuitableToGenerateClass(property.PropertyType) || IsGenericModelNodeType(property.PropertyType))
				&& !property.CanWrite
				&& GetAttribute<ModelValueCalculatorAttribute>(property) == null
				&& GetAttribute<DataSourcePropertyAttribute>(property) == null
				&& GetAttribute<DefaultValueAttribute>(property) == null;
		}
		bool IsGenericModelNodeType(Type type) {
			return ModelInterfacesHelper.IsGenericListType(type) && ModelInterfacesHelper.IsTypeSuitableToGenerateClass(type.GetGenericArguments()[0]);
		}
		void CheckNodeProperty(PropertyInfo property) {
			Type listChildClass = InterfacesInfo.GetListNodesType(CurrentInfo.ModelInterface);
			if(listChildClass == null) return;
			if(!listChildClass.IsAssignableFrom(property.PropertyType)) {
				throw new Exception(string.Format("The property '{0}'  in the interface '{1}' is incorrect type: '{2}'. It should be  type '{3}' or inherited from it.",
					property.Name, property.DeclaringType.Name, property.PropertyType.Name, listChildClass.Name));
			}
		}
		void GetCalculatedValues(PropertyInfo property, EasyClassInfo classInfo, out string linkValue, out Type calculatorType, out string sourceNode) {
			if(GetCalculatedValuesFromValueCalculatorAttribute(property, out linkValue, out calculatorType, out sourceNode))
				return;
			if(GetCalculatedValuesFromModelNodeValueSourceAttribute(property, classInfo, out linkValue, out sourceNode))
				return;
			if(GetCalculatedValuesFromModelDefaultValueCustomCodeAttribute(property, classInfo, out linkValue))
				return;
			IEnumerable<ModelInterfaceImplementorAttribute> attributes = GetAttributes<ModelInterfaceImplementorAttribute>(classInfo.ImplementedInterfaces);
			Type implementedInterface = null;   
			String sourceProperty = null;
			foreach(ModelInterfaceImplementorAttribute implAttribute in attributes) {
				Type implementedInterfaceCandidate = null;
				if(property.DeclaringType.IsAssignableFrom(implAttribute.ImplementedInterface)) {
					implementedInterfaceCandidate = implAttribute.ImplementedInterface;
				}
				else {
					foreach(Type interfaceExtender in interfaceExtenders.GetInterfaceExtenders(implAttribute.ImplementedInterface)) {
						if(property.DeclaringType.IsAssignableFrom(interfaceExtender)) {
							implementedInterfaceCandidate = interfaceExtender;
							break;
						}
					}
				}
				if(implementedInterfaceCandidate != null && (implementedInterface == null || implementedInterface.IsAssignableFrom(implementedInterfaceCandidate))) {
					implementedInterface = implementedInterfaceCandidate;
					sourceProperty = implAttribute.PropertyName;
				}
			}
			if(implementedInterface != null) {
				linkValue = string.Format("{0} is {1} ? (({1}){0}).{2} : {3}",
					sourceProperty,
					TypeToString(implementedInterface),
					property.Name,
					GetDefaultStringValue(property));
				sourceNode = sourceProperty;
			}
		}
		bool GetCalculatedValuesFromModelNodeValueSourceAttribute(PropertyInfo property, EasyClassInfo classInfo, out string linkValue, out string sourceNode) {
			linkValue = null;
			sourceNode = null;
			ModelNodeValueSourceAttribute attribute = null;
			foreach(ModelNodeValueSourceAttribute item in GetAttributes<ModelNodeValueSourceAttribute>(classInfo.ImplementedInterfaces)) {
				if(item.TargetValueName == property.Name) {
					attribute = item;
					break;
				}
			}
			if(attribute != null) {
				sourceNode = attribute.SourceNodePath;
				linkValue = GetLinkValueFromValueCalculatorAttribute(property, attribute.SourceNodePath, attribute.SourceValueName, null, null);
				return true;
			}
			return false;
		}
		bool GetCalculatedValuesFromModelDefaultValueCustomCodeAttribute(PropertyInfo property, EasyClassInfo classInfo, out string linkValue) {
			linkValue = null;
			ModelDefaultValueCustomCodeAttribute attribute = null;
			foreach(ModelDefaultValueCustomCodeAttribute item in GetAttributes<ModelDefaultValueCustomCodeAttribute>(classInfo.ImplementedInterfaces)) {
				if(item.TargetValueName == property.Name) {
					attribute = item;
					break;
				}
			}
			if(attribute != null) {
				linkValue = attribute.CustomCode;
				return true;
			}
			return false;
		}
		bool GetCalculatedValuesFromValueCalculatorAttribute(PropertyInfo property, out string linkValue, out Type calculatorType, out string sourceNode) {
			linkValue = null;
			calculatorType = null;
			sourceNode = null;
			ModelValueCalculatorAttribute attr = GetAttribute<ModelValueCalculatorAttribute>(property);
			if(attr == null) return false;
			linkValue = attr.LinkValue;
			calculatorType = attr.Type;
			if(!string.IsNullOrEmpty(attr.NodeName)) {
				sourceNode = attr.NodeName;
				linkValue = GetLinkValueFromValueCalculatorAttribute(property, attr.NodeName, attr.PropertyName, attr.NodeType, null);
			}
			return true;
		}
		string GetLinkValueFromValueCalculatorAttribute(PropertyInfo property, string nodeName, string propertyName, Type nodeType, string[] nodeTypes) {
			string notNullCheckSt = string.Empty;
			string notNullObject = string.Empty;
			int index = 0;
			foreach(string nodeSt in nodeName.Split('.')) {
				if(!string.IsNullOrEmpty(notNullObject)) {
					notNullObject += '.';
					notNullCheckSt += " && ";
				}
				if(nodeTypes == null || string.IsNullOrEmpty(nodeTypes[index])) {
					notNullObject += nodeSt;
				}
				else {
					notNullObject = string.Format("(({0}){1}{2})", nodeTypes[index], notNullObject, nodeSt);
				}
				notNullCheckSt += string.Format("{0} != null", notNullObject);
				index++;
			}
			if(nodeType != null) {
				notNullObject = string.Format("(({0}){1})", TypeToString(nodeType), notNullObject);
			}
			return string.Format("{0} ? {1}.{2} : {3}", notNullCheckSt, notNullObject, propertyName, GetDefaultStringValue(property));
		}
		object GetDefaultValue(PropertyInfo property) {
			DefaultValueAttribute attr = GetAttribute<DefaultValueAttribute>(property);
			return attr != null ? attr.Value : null;
		}
		string GetMethodName(string name) {
			string origionalName = name;
			int counter = 1;
			while(generatedMethods.Contains(name)) {
				name = origionalName + counter.ToString();
				counter++;
			}
			generatedMethods.Add(name);
			return name;
		}
		protected override string GetAttributeDeclaration(object attribute) {
			Type attributeType = attribute.GetType();
			if(attributeType == typeof(DXDescriptionAttribute)) {
				string description = ((DXDescriptionAttribute)attribute).Description;
				return string.Format("{0}({1})", TypeToString(attributeType), GetEscapedString(description));
			}
			if(attributeType == typeof(DescriptionAttribute)) {
				string description = ((DescriptionAttribute)attribute).Description;
				return string.Format("{0}({1})", TypeToString(attributeType), GetEscapedString(description));
			}
			if(attributeType == typeof(DefaultValueAttribute)) {
				object value = ((DefaultValueAttribute)attribute).Value;
				if(value is Color) {
					Color color = (Color)value;
					if(color.IsNamedColor) {
						return string.Format("{0}(typeof({1}), \"{2}\")", TypeToString(attributeType), TypeToString(typeof(Color)), color.Name);
					}
					return string.Format("{0}(typeof({1}), \"{2}, {3}, {4}, {5}\")", TypeToString(attributeType), TypeToString(typeof(Color)), color.A, color.R, color.G, color.B);
				}
				return string.Format("{0}({1})", TypeToString(attributeType), GetStringValue(value));
			}
			return base.GetAttributeDeclaration(attribute);
		}
		private void AddDomainLogicForIndexProperty(EasyClassInfo classInfo) {
			MethodInfo logicMethod = classInfo.GetLogicMethod(SpecificWords.LogicPrefixGetStatic + ModelValueNames.Index, classInfo.ImplementedInterfaces[0]);
			if(logicMethod != null && !CurrentInfo.DefaultValuesByName.ContainsKey(ModelValueNames.Index)) {
				string methodBody = string.Format("return {0}.{1}(this);", TypeToString(logicMethod.DeclaringType), logicMethod.Name);
				AddDefaultValue(typeof(IModelNode).GetProperty(ModelValueNames.Index), classInfo, methodBody, null);
			}
		}
	}
	sealed class ModelNodeCodeGenerationInfo {
		readonly Type modelInterface;
		readonly List<PropertyInfo> nodeInfos;
		readonly Dictionary<string, ModelValueInfo> valueInfosByName;
		readonly Dictionary<string, object> defaultValuesByName;
		readonly Dictionary<string, string> valueSerializableNamesByName;
		string creatorMethodName;
		public ModelNodeCodeGenerationInfo(Type modelInterface) {
			Guard.ArgumentNotNull(modelInterface, "modelInterface");
			this.modelInterface = modelInterface;
			nodeInfos = new List<PropertyInfo>();
			valueInfosByName = new Dictionary<string, ModelValueInfo>();
			defaultValuesByName = new Dictionary<string, object>();
			valueSerializableNamesByName = new Dictionary<string, string>();
		}
		public Type ModelInterface { get { return modelInterface; } }
		public List<PropertyInfo> NodeInfos { get { return nodeInfos; } }
		public Dictionary<string, ModelValueInfo> ValueInfosByName { get { return valueInfosByName; } }
		public Dictionary<string, object> DefaultValuesByName { get { return defaultValuesByName; } }
		public Dictionary<string, string> ValueSerializableNamesByName { get { return valueSerializableNamesByName; } }
		public string CreatorMethodName {
			get { return creatorMethodName; }
			set {
				Guard.ArgumentNotNullOrEmpty(value, "value");
				creatorMethodName = value;
			}
		}
	}
	sealed class ModelListNodesCodeGenerationHelper {
		readonly Dictionary<Type, Type> cache;
		public ModelListNodesCodeGenerationHelper() {
			cache = new Dictionary<Type, Type>();
		}
		public Type GetListNodesType(Type modelInterface) {
			Guard.ArgumentNotNull(modelInterface, "modelInterface");
			if(modelInterface.IsGenericType) {
				return modelInterface.GetGenericArguments()[0];
			}
			else {
				return GetListType(modelInterface);
			}
		}
		private Type GetListType(Type modelInterface) {
			Type result;
			if(!cache.TryGetValue(modelInterface, out result)) {
				result = FindListType(modelInterface);
				cache[modelInterface] = result;
			}
			return result;
		}
		private Type FindListType(Type modelInterface) {
			Type result = null;
			foreach(Type type in modelInterface.GetInterfaces()) {
				result = GetGenericListTypeCore(type);
				if(result == null) {
					result = GetListType(type);
				}
				if(result != null) {
					return result;
				}
			}
			return result;
		}
		private Type GetGenericListTypeCore(Type type) {
			if(!ModelInterfacesHelper.IsGenericListType(type)) {
				return null;
			}
			Type result = type.GetGenericArguments()[0];
			return result.IsInterface ? result : null;
		}
	}
	static class ModelCustomInterfacesCodeGeneratorHelper {
		public static void GenerateCode(IEnumerable<Type> interfaces, StringBuilder buffer, Func<Type, String> typeNameProvider, Func<Object, String> attributeDeclarationProvider) {
			Guard.ArgumentNotNull(buffer, "buffer");
			Guard.ArgumentNotNull(interfaces, "interfaces");
			Guard.ArgumentNotNull(typeNameProvider, "typeNameProvider");
			Guard.ArgumentNotNull(attributeDeclarationProvider, "attributeDeclarationProvider");
			foreach(Type interfaceType in interfaces) {
				GenerateCodeForCustomInterface(interfaceType, buffer, typeNameProvider, attributeDeclarationProvider);
			}
		}
		private static void GenerateCodeForCustomInterface(Type interfaceType, StringBuilder buffer, Func<Type, String> typeNameProvider, Func<Object, String> attributeDeclarationProvider) {
			GenerateAttributeDeclarations(interfaceType, buffer, null, attributeDeclarationProvider);
			buffer.AppendFormat("public interface {0} :", interfaceType.Name);
			foreach(Type baseInterface in interfaceType.GetInterfaces()) {
				buffer.AppendFormat("{0}, ", baseInterface.FullName);
			}
			buffer.Remove(buffer.Length - 2, 2);
			buffer.AppendLine(" {");
			foreach(PropertyInfo property in interfaceType.GetProperties()) {
				GenerateCodeForCustomInterfaceProperty(property, buffer, typeNameProvider, attributeDeclarationProvider);
			}
			buffer.AppendLine("}");
		}
		private static void GenerateCodeForCustomInterfaceProperty(PropertyInfo property, StringBuilder buffer, Func<Type, String> typeNameProvider, Func<Object, String> attributeDeclarationProvider) {
			GenerateAttributeDeclarations(property, buffer, "\t", attributeDeclarationProvider);
			buffer.AppendFormat("\t{0} {1} {{", typeNameProvider(property.PropertyType), property.Name);
			if(property.CanRead) {
				buffer.Append(" get;");
			}
			if(property.CanWrite) {
				buffer.Append(" set;");
			}
			buffer.AppendLine(" }");
		}
		private static void GenerateAttributeDeclarations(MemberInfo memberInfo, StringBuilder buffer, String indent, Func<Object, String> attributeDeclarationProvider) {
			foreach(Attribute attribute in memberInfo.GetCustomAttributes(true)) {
				String declaration = attributeDeclarationProvider(attribute);
				if(!String.IsNullOrEmpty(declaration)) {
					if(String.IsNullOrEmpty(indent)) {
						buffer.Append(indent);
					}
					buffer.AppendFormat("[{0}]", declaration).AppendLine();
				}
			}
		}
	}
}
