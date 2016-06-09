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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation {
	public class ModelValidationRulesNodeGenerator : ModelNodesGeneratorBase {
		public static string GetRuleInfoString(IRuleBaseProperties ruleProperties) {
			string info = string.Format(@"ID=""{0}"" TargetType=""{1}""", ruleProperties.Id, ruleProperties.TargetType.FullName);
			if(ruleProperties is IRulePropertyValueProperties) {
				info += string.Format(@" TargetPropertyName=""{0}""", ((IRulePropertyValueProperties)ruleProperties).TargetPropertyName);
			}
			return info;
		}
		private void ThrowRuleAlreadyExists(IRule rule, IModelRuleBase existingRule) {
			string existingRuleTypeName = existingRule.GetType().Name;
			if(existingRuleTypeName.StartsWith("Model")) {
				existingRuleTypeName = existingRuleTypeName.Remove(0, "Model".Length);
			}
			throw new InvalidOperationException(string.Format(
				"Another rule with ID = '{0}' already exists.\r\n\r\n" +
				"Existing rule: \r\n\r\n'{1}'\r\n\r\n" +
				"New rule: \r\n\r\n'{2}'",
				rule.Id,
				string.Join(": ", new string[] { existingRuleTypeName, GetRuleInfoString(existingRule as IRuleBaseProperties) }),
				string.Join(": ", new string[] { rule.GetType().Name, GetRuleInfoString(rule.Properties) })));
		}
		private void AddRuleNodesIntoModel(ModelNode node, IList<IRule> rules) {
			IModelValidationRules rulesNode = (IModelValidationRules)node;
			System.Reflection.Assembly assembly = node.GetType().Assembly;
			foreach(IRule rule in rules) {
				if(rulesNode[rule.Id] != null) {
					ThrowRuleAlreadyExists(rule, rulesNode[rule.Id]);
				}
				Type ruleNodeType = assembly.GetType(string.Format("Model{0}", rule.GetType().Name));
				if(ruleNodeType == null) {
					throw new InvalidOperationException(string.Format("Unable to create a model node for the '{0}' rule. The rule type is unregistered. Use the ValidationRulesRegistrator class to register the rule on module setup.", rule.GetType().Name));
				}
				ModelNode ruleNode = ((ModelNode)rulesNode).AddNode(rule.Id, ruleNodeType);
				AssignPropertiesToNode((ModelNode)ruleNode, rule.Properties);
			}
		}
		private void AssignPropertiesToNode(ModelNode node, IRuleBaseProperties properties) {
			foreach(System.Reflection.PropertyInfo property in properties.GetType().GetProperties()) {
				object value = property.GetValue(properties, null);
				if(!object.Equals(node.GetValue(property.Name), value)) {
					node.SetValue(property.Name, value);
				}
			}
		}
		protected override void GenerateNodesCore(ModelNode node) {
			RuleSet registrationRuleSet = new RuleSet();
			foreach(IModelClass targetClass in node.Application.BOModel) {
				registrationRuleSet.RegisterRules(targetClass.TypeInfo);
			}
			ITypeInfo iRuleType = XafTypesInfo.Instance.FindTypeInfo(typeof(IRule));
			if(iRuleType != null) {
				foreach(ITypeInfo iRuleImplementor in iRuleType.Implementors) {
					registrationRuleSet.RegisterRules(iRuleImplementor);
				}
			}
#if DebugTest
			List<IRule> rulesToProcess = new List<IRule>(registrationRuleSet.RegisteredRules);
			CustomizeModelRulesEventArgs args = new CustomizeModelRulesEventArgs(rulesToProcess, new List<IRuleSource>());
			if (CustomizeModelRules != null) {
				CustomizeModelRules(this, args);
			}
			RuleSet resultRuleSet = new RuleSet(args.Rules, args.RuleSources);
#else
			RuleSet resultRuleSet = registrationRuleSet;
#endif
			AddRuleNodesIntoModel(node, resultRuleSet.GetRules());
		}
#if DebugTest
		public static event EventHandler<CustomizeModelRulesEventArgs> CustomizeModelRules;
#endif
	}
	public class ModelValidationContextsNodeGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			foreach(IModelRuleBase rule in ((IModelApplicationValidation)node.Application).Validation.Rules) {
				foreach(string context in (ContextIdentifiers)((IRuleBaseProperties)rule).TargetContextIDs) {
					if(node[context] != null) {
						continue;
					}
					IModelValidationContext addedContext = node.AddNode<IModelValidationContext>(context);
					addedContext.Caption = context;
					addedContext.AllowInplaceValidation = context == ContextIdentifier.Save;
				}
			}
		}
	}
	public interface IModelApplicationValidation : IModelNode {
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("IModelApplicationValidationValidation")]
#endif
		IModelValidation Validation { get; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ImageName("BO_Validation")]
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationIModelValidation")]
#endif
	public interface IModelValidation : IModelNode {
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("IModelValidationRules")]
#endif
		IModelValidationRules Rules { get; }
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("IModelValidationContexts")]
#endif
		IModelValidationContexts Contexts { get; }
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("IModelValidationErrorMessageTemplates")]
#endif
		IModelValidationdDefaultErrorMessageTemplates ErrorMessageTemplates { get; }
	}
	[ModelNodesGenerator(typeof(ModelValidationRulesNodeGenerator))]
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationIModelValidationRules")]
#endif
	public interface IModelValidationRules : IModelNode, IModelList<IModelRuleBase> {
	}
	[ModelAbstractClass]
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationIModelRuleBase")]
#endif
	public interface IModelRuleBase : IModelNode {
	}
	[ModelNodesGenerator(typeof(ModelValidationContextsNodeGenerator))]
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationIModelValidationContexts")]
#endif
	public interface IModelValidationContexts : IModelNode, IModelList<IModelValidationContext> { 
	}
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationIModelValidationContext")]
#endif
	public interface IModelValidationContext : IModelNode {
		[
#if !SL
	DevExpressExpressAppValidationLocalizedDescription("IModelValidationContextId"),
#endif
 Required]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppValidationLocalizedDescription("IModelValidationContextCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppValidationLocalizedDescription("IModelValidationContextAllowInplaceValidation"),
#endif
 DefaultValue(false)]
		bool AllowInplaceValidation { get; set; }
	}	
	public interface IModelLayoutManagerOptionsValidation {
		[ Category("Layout")]
		[DefaultValue(""), Localizable(true)]
		string RequiredFieldMark { get; set; }
	}
	public sealed class ValidationRulesRegistrator {
		public static void RegisterRule(ApplicationModulesManager modulesManager, Type ruleType, Type ruleProperties) {
			Guard.ArgumentNotNull(modulesManager, "modulesManager");
			Guard.ArgumentNotNull(modulesManager.Modules, "modulesManager.Modules");
			ValidationModule validationModule = (ValidationModule)modulesManager.Modules.FindModule(typeof(ValidationModule));
			Guard.ArgumentNotNull(validationModule, "validationModule");
			Guard.ArgumentNotNull(ruleType, "ruleType");
			Guard.ArgumentNotNull(ruleProperties, "ruleProperties");
			validationModule.RegisterRule(ruleType, ruleProperties);
		}
	}
	sealed class ValidationModelUpdater {
		static readonly string ErrorMessageTemplatesExtenderName = string.Format("{0}Extender", typeof(IModelValidationdDefaultErrorMessageTemplates).Name);
		static readonly Type[] RuleModelInterfaceInterfaces = new Type[] { typeof(IModelRuleBase) };
		internal const string RuleTypeValueName = "__RuleType";
		readonly ITypesInfo typesInfo;
		readonly ModelInterfaceExtenders extenders;
		readonly CustomTypeForValidation errorMessageTemplatesExtender;
		readonly Dictionary<string, CustomTypeForValidation> ruleModelInterfaces;
		readonly Dictionary<Type, CustomTypeForValidation> messageTemplatesModelInterfaces;
		readonly Dictionary<Type, GenerateMessageTemplatesModelAttribute> attributes;
		internal ValidationModelUpdater(ModelInterfaceExtenders extenders) {
			Guard.ArgumentNotNull(extenders, "extenders");
			typesInfo = XafTypesInfo.Instance;
			this.extenders = extenders;
			errorMessageTemplatesExtender = new CustomTypeForValidation(ErrorMessageTemplatesExtenderName, Type.EmptyTypes);
			extenders.Add(typeof(IModelValidationdDefaultErrorMessageTemplates), errorMessageTemplatesExtender);
			ruleModelInterfaces = new Dictionary<string, CustomTypeForValidation>();
			attributes = new Dictionary<Type, GenerateMessageTemplatesModelAttribute>();
			messageTemplatesModelInterfaces = new Dictionary<Type, CustomTypeForValidation>();
		}
		private bool IsMessageTemplateModelInterfaceProperty(PropertyInfo property) {
			return property.Name.StartsWith("MessageTemplate");
		}
		private GenerateMessageTemplatesModelAttribute GetAttribute(Type ruleProperties) {
			GenerateMessageTemplatesModelAttribute attribute = null;
			if(!attributes.TryGetValue(ruleProperties, out attribute)) {
				GenerateMessageTemplatesModelAttribute[] currentAttributes = AttributeHelper.GetAttributes<GenerateMessageTemplatesModelAttribute>(ruleProperties, false);
				if(currentAttributes.Length > 0) {
					attribute = currentAttributes[0];
				}
				attributes[ruleProperties] = attribute;
			}
			return attribute;
		}
		private CustomTypeForValidation CreateRuleModelInterface(Type ruleType, Type ruleProperties) {
			CustomTypeForValidation ruleModelInterface = new CustomTypeForValidation(GetRuleModelInterfaceName(ruleType), RuleModelInterfaceInterfaces);
			GenerateMessageTemplatesModelAttribute attribute = GetAttribute(ruleProperties);
			if(attribute != null) {
				foreach(Type baseInterface in Enumerator.Combine<Type>(new Type[] { ruleProperties }, ruleProperties.GetInterfaces())) {
					foreach(PropertyInfo property in baseInterface.GetProperties()) {
						if(IsMessageTemplateModelInterfaceProperty(property)) {
							string customCode = string.Format("(({1})(({0})Root).Validation.ErrorMessageTemplates).{2}.{3}", typeof(IModelApplicationValidation).FullName, ErrorMessageTemplatesExtenderName, attribute.MessageTemplatePropertyName, property.Name);
							ruleModelInterface.AddAttribute(new ModelDefaultValueCustomCodeAttribute(property.Name, customCode));
						}
					}
				}
			}
			ruleModelInterface.AddAttribute(new DefaultValueAttribute(ruleType.FullName));
			typesInfo.FindTypeInfo(ruleModelInterface); 
			return ruleModelInterface;
		}
		private CustomTypeForValidation GetMessageTemplateModelInterface(Type ruleProperties) {
			CustomTypeForValidation messageTemplateModelInterface;
			if(!messageTemplatesModelInterfaces.TryGetValue(ruleProperties, out messageTemplateModelInterface)) {
				messageTemplateModelInterface = CreateMessageTemplateModelInterface(ruleProperties);
				messageTemplatesModelInterfaces[ruleProperties] = messageTemplateModelInterface;
			}
			return messageTemplateModelInterface;
		}
		private CustomTypeForValidation CreateMessageTemplateModelInterface(Type ruleProperties) {
			GenerateMessageTemplatesModelAttribute attribute = GetAttribute(ruleProperties);
			if(attribute != null) {
				Type[] interfaces = CollectMessageTemplateModelInterfaceInterfaces(ruleProperties);
				string name = string.Format("IModelMessageTemplate{0}", attribute.MessageTemplatePropertyName);
				CustomTypeForValidation messageTemplateModelInterface = new CustomTypeForValidation(name, interfaces);
				foreach(PropertyInfo property in CollectMessageTemplateModelInterfaceProperties(ruleProperties)) {
					messageTemplateModelInterface.AddProperty(property);
				}
				foreach(Attribute attr in CollectMessageTemplateModelInterfaceAttributes(ruleProperties)) {
					messageTemplateModelInterface.AddAttribute(attr);
				}
				foreach(Type baseInterface in ruleProperties.GetInterfaces()) {
					GenerateMessageTemplatesModelAttribute attr = GetAttribute(baseInterface);
					if(attr != null) {
						foreach(PropertyInfo property in baseInterface.GetProperties()) {
							if(IsMessageTemplateModelInterfaceProperty(property)) {
								string customCode = string.Format("(({1})(({0})Root).Validation.ErrorMessageTemplates).{2}.{3}", typeof(IModelApplicationValidation).FullName, ErrorMessageTemplatesExtenderName, attr.MessageTemplatePropertyName, property.Name);
								messageTemplateModelInterface.AddAttribute(new ModelDefaultValueCustomCodeAttribute(property.Name, customCode));
							}
						}
					}
				}
				CustomPropertyInfoForValidation messageTemplateProperty = new CustomPropertyInfoForValidation(attribute.MessageTemplatePropertyName, messageTemplateModelInterface, errorMessageTemplatesExtender);
				errorMessageTemplatesExtender.AddProperty(messageTemplateProperty);
				return messageTemplateModelInterface;
			}
			return null;
		}
		private Type[] CollectMessageTemplateModelInterfaceInterfaces(Type ruleProperties) {
			List<Type> list = new List<Type>();
			list.Add(typeof(IModelNode));
			foreach(Type baseInterface in ruleProperties.GetInterfaces()) {
				CustomTypeForValidation item = GetMessageTemplateModelInterface(baseInterface);
				if(item != null) {
					list.Add(item);
				}
			}
			return list.ToArray();
		}
		private PropertyInfo[] CollectMessageTemplateModelInterfaceProperties(Type ruleProperties) {
			List<PropertyInfo> list = new List<PropertyInfo>();
			foreach(PropertyInfo property in ruleProperties.GetProperties()) {
				if(IsMessageTemplateModelInterfaceProperty(property)) {
					list.Add(property);
				}
			}
			return list.ToArray();
		}
		private Attribute[] CollectMessageTemplateModelInterfaceAttributes(Type ruleProperties) {
			List<Attribute> list = new List<Attribute>();
			foreach(Attribute attribute in AttributeHelper.GetAttributes<Attribute>(ruleProperties, false)) {
				Type attributeType = attribute.GetType();
				if(!typeof(GenerateMessageTemplatesModelAttribute).IsAssignableFrom(attributeType) && !typeof(DomainComponentAttribute).IsAssignableFrom(attributeType)) {
					list.Add(attribute);
				}
			}
			return list.ToArray();
		}
		internal void RegisterRule(Type ruleType, Type ruleProperties) {
			Guard.ArgumentNotNull(ruleType, "ruleType");
			Guard.ArgumentNotNull(ruleProperties, "ruleProperties");
			string ruleName = GetRuleName(ruleType);
			if(ruleModelInterfaces.ContainsKey(ruleName)) {
				throw new LightDictionary<string, Type>.DuplicatedKeyException(ruleName, ruleModelInterfaces[ruleName], ruleType);
			}
			CustomTypeForValidation ruleModelInterface = CreateRuleModelInterface(ruleType, ruleProperties);
			extenders.Add(ruleModelInterface, ruleProperties);
			GetMessageTemplateModelInterface(ruleProperties);
			ruleModelInterfaces[ruleName] = ruleModelInterface;
		}
		internal static string GetRuleName(Type ruleType) {
			Guard.ArgumentNotNull(ruleType, "ruleType");
			return ruleType.Name;
		}
		internal static string GetRuleModelInterfaceName(Type ruleType) {
			return string.Format("IModel{0}", GetRuleName(ruleType));
		}
	}
	sealed class CustomTypeForValidation : Type {
		const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
		static readonly ConstructorInfo[] EmptyConstructors = new ConstructorInfo[0];
		static readonly PropertyInfo[] EmptyProperties = new PropertyInfo[0];
		static readonly FieldInfo[] EmptyFields = new FieldInfo[0];
		static readonly MethodInfo[] EmptyMethods = new MethodInfo[0];
		static readonly EventInfo[] EmptyEvents = new EventInfo[0];
		readonly string name;
		readonly Type[] interfaces;
		readonly Guid guid;
		readonly List<PropertyInfo> properties;
		readonly List<Attribute> attributes;
		internal CustomTypeForValidation(string name, Type[] interfaces) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			this.name = name;
			this.interfaces = CollectInterfaces(interfaces);
			guid = Guid.NewGuid();
			properties = new List<PropertyInfo>();
			attributes = new List<Attribute>();
		}
		public override string Name { get { return name; } }
		public override string Namespace { get { return null; } }
		public override string FullName { get { return name; } }
		public override Type BaseType { get { return null; } }
		public override Type UnderlyingSystemType { get { return null; } }
		public override Assembly Assembly { get { return typeof(CustomTypeForValidation).Assembly; } }
		public override string AssemblyQualifiedName { get { return typeof(CustomTypeForValidation).AssemblyQualifiedName; } }
		public override Guid GUID { get { return guid; } }
		public override Module Module { get { return typeof(CustomTypeForValidation).Module; } }
		private Type[] CollectInterfaces(Type[] interfaces) {
			List<Type> list = new List<Type>();
			foreach(Type type in interfaces) {
				if(!list.Contains(type)) {
					list.Add(type);
				}
				foreach(Type item in type.GetInterfaces()) {
					if(!list.Contains(item)) {
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}
		internal void AddProperty(PropertyInfo property) {
			Guard.ArgumentNotNull(property, "property");
			if(properties.Contains(property)) {
				throw new ArgumentException("property");
			}
			properties.Add(property);
		}
		internal void AddAttribute(Attribute attribute) {
			Guard.ArgumentNotNull(attribute, "attribute");
			if(!attributes.Contains(attribute)) {
				attributes.Add(attribute);
			}
		}
		protected override TypeAttributes GetAttributeFlagsImpl() {
			return TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract;
		}
		protected override bool HasElementTypeImpl() {
			return false;
		}
		protected override bool IsArrayImpl() {
			return false;
		}
		protected override bool IsByRefImpl() {
			return false;
		}
		protected override bool IsCOMObjectImpl() {
			return false;
		}
		protected override bool IsPointerImpl() {
			return false;
		}
		protected override bool IsPrimitiveImpl() {
			return false;
		}
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
			return (bindingAttr & Flags) == Flags ? properties.Find(delegate(PropertyInfo p) { return p.Name == name; }) : null;
		}
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			return null;
		}
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			return null;
		}
		public override object[] GetCustomAttributes(bool inherit) {
			return attributes.ToArray();
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			List<Attribute> list = new List<Attribute>();
			foreach(Attribute attribute in attributes) {
				if(attributeType.IsAssignableFrom(attribute.GetType())) {
					list.Add(attribute);
				}
			}
			return list.ToArray();
		}
		public override bool IsDefined(Type attributeType, bool inherit) {
			return GetCustomAttributes(attributeType, inherit).Length > 0;
		}
		public override Type[] GetInterfaces() {
			return interfaces;
		}
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
			return EmptyConstructors;
		}
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
			return GetProperties(bindingAttr);
		}
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
			return (bindingAttr & Flags) == Flags ? properties.ToArray() : EmptyProperties;
		}
		public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
			return EmptyFields;
		}
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
			return EmptyMethods;
		}
		public override Type GetElementType() {
			return null;
		}
		public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
			return null;
		}
		public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
			return EmptyEvents;
		}
		public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
			return null;
		}
		public override Type GetInterface(string name, bool ignoreCase) {
			Guard.ArgumentNotNull(name, "name");
			string namePart = name;
			string namespacePart = string.Empty;
			int indexOfDot = name.LastIndexOf('.');
			if(indexOfDot >= 0) {
				namePart = name.Substring(indexOfDot + 1);
				namespacePart = name.Substring(0, indexOfDot);
			}
			Type[] list = Array.FindAll(interfaces, delegate(Type type) { return string.Compare(type.Name, namePart, ignoreCase) == 0; });
			if(list.Length == 0) {
				return null;
			}
			if(!string.IsNullOrEmpty(namespacePart)) {
				list = Array.FindAll(list, delegate(Type type) { return type.Namespace == namespacePart; });
				return list.Length > 0 ? list[0] : null;
			}
			else if(list.Length == 1) {
				return list[0];
			}
			else {
				throw new System.Reflection.AmbiguousMatchException();
			}
		}
		public override Type GetNestedType(string name, BindingFlags bindingAttr) {
			return null;
		}
		public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
			return Type.EmptyTypes;
		}
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) {
			throw new NotSupportedException();
		}
		public override bool Equals(object obj) {
			if(!ReferenceEquals(this, obj)) {
				CustomTypeForValidation type = obj as CustomTypeForValidation;
				return type != null && name == type.name;
			}
			return true;
		}
		public override int GetHashCode() {
			return name.GetHashCode();
		}
	}
	sealed class CustomPropertyInfoForValidation : PropertyInfo {
		static readonly ParameterInfo[] EmptyParameters = new ParameterInfo[0];
		static readonly Attribute[] EmptyAttributes = new Attribute[0];
		static readonly MethodInfo[] EmptyMethods = new MethodInfo[0];
		readonly string name;
		readonly Type propertyType;
		readonly CustomTypeForValidation declaringType;
		internal CustomPropertyInfoForValidation(string name, Type propertyType, CustomTypeForValidation declaringType) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(propertyType, "propertyType");
			Guard.ArgumentNotNull(declaringType, "declaringType");
			this.name = name;
			this.propertyType = propertyType;
			this.declaringType = declaringType;
		}
		public override string Name { get { return name; } }
		public override Type PropertyType { get { return propertyType; } }
		public override Type DeclaringType { get { return declaringType; } }
		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override PropertyAttributes Attributes { get { return PropertyAttributes.None; } }
		public override Type ReflectedType { get { return declaringType; } }
		public override ParameterInfo[] GetIndexParameters() {
			return EmptyParameters;
		}
		public override MethodInfo GetGetMethod(bool nonPublic) {
			return null;
		}
		public override MethodInfo GetSetMethod(bool nonPublic) {
			return null;
		}
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			return EmptyAttributes;
		}
		public override object[] GetCustomAttributes(bool inherit) {
			return EmptyAttributes;
		}
		public override bool IsDefined(Type attributeType, bool inherit) {
			return false;
		}
		public override MethodInfo[] GetAccessors(bool nonPublic) {
			return EmptyMethods;
		}
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
		public override bool Equals(object obj) {
			if(!ReferenceEquals(this, obj)) {
				CustomPropertyInfoForValidation other = obj as CustomPropertyInfoForValidation;
				return other != null && Name == other.Name && PropertyType == other.PropertyType;
			}
			return true;
		}
		public override int GetHashCode() {
			return Name.GetHashCode() ^ PropertyType.GetHashCode();
		}
		public override string ToString() {
			return string.Format("{0} {1}", propertyType.FullName, name);
		}
	}
}
