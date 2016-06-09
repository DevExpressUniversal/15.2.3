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
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	public static class RuleDefaultMessageTemplatesHelper {
		public static string DefaultMessageTemplatePrefix = "Default";
		public static string InstanceMessageTemplatePrefix = "MessageTemplate";
		private static bool IsCorrectDefaultMessageTemplatePropertyName(string propertyName) {
			return propertyName.StartsWith(DefaultMessageTemplatePrefix + InstanceMessageTemplatePrefix);
		}
		private static bool IsCorrectInstanceMessageTemplatePropertyName(string propertyName) {
			return propertyName.StartsWith(InstanceMessageTemplatePrefix) && propertyName != InstanceMessageTemplatePrefix;
		}
		private static PropertyInfo FindInstanceMessageTemplateProperty(string defaultMessageTemplatePropertyName, IRuleBaseProperties properties) {
			if(IsCorrectDefaultMessageTemplatePropertyName(defaultMessageTemplatePropertyName)) {
				string instancePropertyName = defaultMessageTemplatePropertyName.Substring(DefaultMessageTemplatePrefix.Length);
				return properties.GetType().GetProperty(instancePropertyName, BindingFlags.Public | BindingFlags.Instance);
			}
			return null;
		}
		public static PropertyInfo FindDefaultMessageTemplateProperty(Type ruleType, string instancePropertyName) {
			if(IsCorrectInstanceMessageTemplatePropertyName(instancePropertyName)) {
				string defaultTemplatePropertyName = DefaultMessageTemplatePrefix + instancePropertyName;
				return ruleType.GetProperty(defaultTemplatePropertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			}
			return null;
		}
		public static IList<PropertyInfo> FindDefaultMessageTemplateProperties(Type ruleType) {
			if(ruleType == null) {
				throw new ArgumentNullException("ruleType");
			}
			List<PropertyInfo> resultProperties = new List<PropertyInfo>();
			foreach(PropertyInfo propertyInfo in ruleType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)) {
				if(IsCorrectDefaultMessageTemplatePropertyName(propertyInfo.Name)) {
					resultProperties.Add(propertyInfo);
				}
			}
			return resultProperties;
		}
		public static string FindDefaultMessageTemplate(Type ruleType, string propertyName) {
			PropertyInfo defaultMessageTemplatePropertyInfo = FindDefaultMessageTemplateProperty(ruleType, propertyName);
			if(defaultMessageTemplatePropertyInfo != null) {
				return defaultMessageTemplatePropertyInfo.GetValue(null, null) as string;
			}
			return null;
		}
		public static bool IsDefaultMessageTemplateExists(Type ruleType, string propertyName) {
			return FindDefaultMessageTemplateProperty(ruleType, propertyName) != null;
		}
		public static void AssignDefaultMessageTemplates(IRuleBaseProperties properties, Type ruleType) {
			foreach(PropertyInfo propertyInfo in FindDefaultMessageTemplateProperties(ruleType)) {
				string defaultTemplate = propertyInfo.GetValue(null, null) as string;
				PropertyInfo instancePropertyInfo = FindInstanceMessageTemplateProperty(propertyInfo.Name, properties);
				if(instancePropertyInfo != null) {
					instancePropertyInfo.SetValue(properties, defaultTemplate, null);
				}
			}
		}
	}
	public static class RulePropertiesHelper {
		private static string mustImplementIRuleMessage = "The ruleType parameter must implement the IRule interface";
		private static IList<Type> GetRuleTypesHierarchy(Type ruleType) {
			if(ruleType == null) {
				throw new ArgumentNullException("ruleType");
			}
			if(!typeof(IRule).IsAssignableFrom(ruleType)) {
				throw new ArgumentOutOfRangeException("ruleType", ruleType, "The type that is passed as the ruleType parameter must implement the IRule interface");
			}
			Type currentType = ruleType;
			List<Type> typesHierarchy = new List<Type>();
			while(typeof(IRule).IsAssignableFrom(currentType)) {
				typesHierarchy.Insert(0, currentType);
				currentType = currentType.BaseType;
			}
			return typesHierarchy;
		}
		public static IList<PropertyInfo> GetRulePropertiesMembers(Type rulePropertiesType) {
			if(rulePropertiesType == null) {
				throw new ArgumentNullException("rulePropertiesType");
			}
			if(!typeof(IRuleBaseProperties).IsAssignableFrom(rulePropertiesType)) {
				throw new ArgumentOutOfRangeException("rulePropertiesType", rulePropertiesType, "The rulePropertiesType parameter must implement the IRuleBaseProperties interface");
			}
			Type currentType = rulePropertiesType;
			List<Type> typesHierarchy = new List<Type>();
			while(typeof(IRuleBaseProperties).IsAssignableFrom(currentType)) {
				typesHierarchy.Insert(0, currentType);
				currentType = currentType.BaseType;
			}
			List<PropertyInfo> result = new List<PropertyInfo>();
			foreach(Type ruleCurrentType in typesHierarchy) {
				ITypeInfo ruleInfo = XafTypesInfo.Instance.FindTypeInfo(ruleCurrentType);
				List<PropertyInfo> currentTypeProperties = new List<PropertyInfo>();
				currentTypeProperties.AddRange(ruleCurrentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
				if(ruleInfo.FindAttribute<RulePropertiesIndexedAttribute>() != null) {
					currentTypeProperties.Sort(SortByIndex);
				}
				result.AddRange(currentTypeProperties);
			}
			return result;
		}
		private static int SortByIndex(object x, object y) {
			if(x == null && y == null) {
				return 0;
			}
			else if(x == null && y != null) {
				return 1;
			}
			else if(x != null && y == null) {
				return -1;
			}
			else {
				return RulePropertiesIndexAttribute.RetrieveIndex((PropertyInfo)x).CompareTo(RulePropertiesIndexAttribute.RetrieveIndex((PropertyInfo)y));
			}
		}
		public static Type GetRulePropertiesType(Type ruleType) {
			Guard.ArgumentNotNull(ruleType, "ruleType");
			if(!typeof(IRule).IsAssignableFrom(ruleType)) {
				throw new InvalidCastException(mustImplementIRuleMessage);
			}
			IRule rule = (IRule)TypeHelper.CreateInstance(ruleType);
			Guard.ArgumentNotNull(rule.Properties, "rule.Properties");
			return rule.Properties.GetType();
		}
		public static IDictionary<string, object> GetRulePropertiesDefaultValues(Type ruleType) {
			LightDictionary<string, object> defaultValuesDictionary = new LightDictionary<string, object>();
			Dictionary<string, DefaultValueEntry> defaultValues = GetDefaultValues(GetRulePropertiesType(ruleType));
			foreach(string key in defaultValues.Keys) {
				defaultValuesDictionary[key] = defaultValues[key].Value;
			}
			return defaultValuesDictionary;
		}
		private class DefaultValueEntry {
			private PropertyInfo propertyInfo;
			private Type interfaceType;
			private object value;
			public DefaultValueEntry(PropertyInfo propertyInfo, object value, Type interfaceType) {
				this.propertyInfo = propertyInfo;
				this.interfaceType = interfaceType;
				object convertedValue = value;
				if(value != null && !propertyInfo.PropertyType.IsAssignableFrom(value.GetType())) {
					convertedValue = ReflectionHelper.Convert(value, propertyInfo.PropertyType);
				}
				this.value = convertedValue;
			}
			public PropertyInfo PropertyInfo {
				get { return propertyInfo; }
				set { propertyInfo = value; }
			}
			public Type InterfaceType {
				get { return interfaceType; }
				set { interfaceType = value; }
			}
			public object Value {
				get { return value; }
				set { this.value = value; }
			}
		}
		private static Dictionary<string, DefaultValueEntry> GetDefaultValues(Type propertiesType) {
			Guard.ArgumentNotNull(propertiesType, "propertiesType");
			Guard.TypeArgumentIs(typeof(IRuleBaseProperties), propertiesType, "propertiesType");
			Dictionary<string, DefaultValueEntry> defaultValues = new Dictionary<string, DefaultValueEntry>();
			IList<Type> interfaces = propertiesType.GetInterfaces();
			foreach(Type interfaceType in interfaces) {
				if(!typeof(IRuleBaseProperties).IsAssignableFrom(interfaceType)) {
					continue;
				}
				foreach(PropertyInfo info in interfaceType.GetProperties()) {
					IList attributes = info.GetCustomAttributes(typeof(DefaultValueAttribute), false);
					if(attributes.Count > 0) {
						DefaultValueEntry entry = new DefaultValueEntry(info, ((DefaultValueAttribute)attributes[0]).Value, interfaceType);
						if(!defaultValues.ContainsKey(info.Name) || defaultValues[info.Name].InterfaceType.IsAssignableFrom(interfaceType)) {
							defaultValues[info.Name] = entry;
						}
					}
				}
			}
			return defaultValues;
		}
		public static void AssignDefaultValues(IRuleBaseProperties properties) {
			Guard.ArgumentNotNull(properties, "properties");
			Dictionary<string, DefaultValueEntry> defaultValues = GetDefaultValues(properties.GetType());
			foreach(string key in defaultValues.Keys) {
				try {
					defaultValues[key].PropertyInfo.SetValue(properties, defaultValues[key].Value, null);
				}
				catch(TargetInvocationException e) {
					if(e.InnerException != null && e.InnerException is MemberNotFoundException) {
						throw e.InnerException;
					}
				}
			}
		}
		public static void AssignDefaultValues(IRuleBaseProperties properties, Type ruleType) {
			Guard.ArgumentNotNull(properties, "properties");
			AssignDefaultValues(properties);
		}
		public static void AssignDefaultValues(IRule rule) {
			Guard.ArgumentNotNull(rule, "rule");
			AssignDefaultValues(rule.Properties);
		}
		public static void Assign(IRuleNamedProperties target, IRuleNamedProperties source, Type propertiesType) {
			Guard.ArgumentNotNull(target, "target");
			Guard.ArgumentNotNull(source, "source");
			Guard.ArgumentNotNull(propertiesType, "propertiesType");
			foreach(PropertyInfo propertyInfo in GetRulePropertiesMembers(propertiesType)) {
				object value = source.GetPropertyValue(propertyInfo.Name, propertyInfo.PropertyType, propertiesType);
				target.SetPropertyValue(propertyInfo.Name, value);
			}
		}
		public static void Assign(IRuleNamedProperties target, IRuleBaseProperties source) {
			Assign(target, new RulePropertiesAdapter(source), source.GetType());
		}
		public static void Assign(IRuleBaseProperties target, IRuleNamedProperties source) {
			Assign(new RulePropertiesAdapter(target), source, target.GetType());
		}
		public static void Assign(IRuleBaseProperties target, IRuleBaseProperties source) {
			Assign(new RulePropertiesAdapter(target), new RulePropertiesAdapter(source), source.GetType());
		}
	}
	public interface IRuleNamedProperties {
		object GetPropertyValue(string propertyName, Type valueType, Type objectType);
		void SetPropertyValue(string propertyName, object value);
		string GetRealPropertyName(string propertyName);
	}
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("ValidationIModelValidationdDefaultErrorMessageTemplates")]
#endif
	public interface IModelValidationdDefaultErrorMessageTemplates : IModelNode {
	}
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class GenerateMessageTemplatesModelAttribute : Attribute {
		readonly string messageTemplatePropertyName;
		public GenerateMessageTemplatesModelAttribute(string messageTemplatePropertyName) {
			Guard.ArgumentNotNullOrEmpty(messageTemplatePropertyName, "messageTemplatePropertyName");
			this.messageTemplatePropertyName = messageTemplatePropertyName;
		}
		public string MessageTemplatePropertyName { get { return messageTemplatePropertyName; } }
	}
	public enum ValidationResultType { Error, Warning, Information };
	[GenerateMessageTemplatesModel("RuleBase")]
	[DomainComponent]
	[Description("Defines common templates for the messages displayed when validation rules are broken.")]
	public interface IRuleBaseProperties {
		[Required]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesId")]
#endif
		string Id { get; set; }
		[Localizable(true)]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesName")]
#endif
		string Name { get; set; }
		[Localizable(true)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesCustomMessageTemplate")]
#endif
		string CustomMessageTemplate { get; set; }
		[Required]
		[TypeConverter(typeof(StringToTypeConverter))]
		[Category("Data")]
		[RefreshPropertiesAttribute(RefreshProperties.All)]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesTargetType")]
#endif
		Type TargetType { get; set; }
		[Required]
		[Category("Behavior")]
		[DefaultValue(typeof(ValidationResultType), "Error")]
		ValidationResultType ResultType { get; set; }
		[Required]
		[Category("Behavior")]
		[TypeConverter(typeof(DefaultValidationContextsConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesTargetContextIDs")]
#endif
		string TargetContextIDs { get; set; }
		[DefaultValue(false)]
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesInvertResult")]
#endif
		bool InvertResult { get; set; }
		[DefaultValue(true)]
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesSkipNullOrEmptyValues")]
#endif
		bool SkipNullOrEmptyValues { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.SkipNullOrEmptyValues)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesMessageTemplateSkipNullOrEmptyValues")]
#endif
		string MessageTemplateSkipNullOrEmptyValues { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.TargetDoesNotSatisfyTargetCriteria)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesMessageTemplateTargetDoesNotSatisfyTargetCriteria")]
#endif
		string MessageTemplateTargetDoesNotSatisfyTargetCriteria { get; set; }
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesTargetCriteria")]
#endif
		[CriteriaOptions("TargetType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string TargetCriteria { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleCollectionProperty")]
	[DomainComponent]
	[Description("Defines common templates for the messages displayed when validation rules applied to collection properties are broken.")]
	public interface IRuleCollectionPropertyProperties : IRuleBaseProperties {
		[TypeConverter(typeof(StringToTypeConverter))]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("IRuleCollectionPropertyPropertiesTargetCollectionOwnerType")]
#endif
		Type TargetCollectionOwnerType { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("IRuleCollectionPropertyPropertiesTargetCollectionPropertyName")]
#endif
		string TargetCollectionPropertyName { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.TargetDoesNotSatisfyCollectionCriteria)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("IRuleCollectionPropertyPropertiesMessageTemplateTargetDoesNotSatisfyCollectionCriteria")]
#endif
		string MessageTemplateTargetDoesNotSatisfyCollectionCriteria { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.CollectionValidationMessageSuffix)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("IRuleCollectionPropertyPropertiesMessageTemplateCollectionValidationMessageSuffix")]
#endif
		string MessageTemplateCollectionValidationMessageSuffix { get; set; }
	}
	[DomainComponent]
	public interface IRuleSupportsCollectionAggregatesProperties : IRulePropertyValueProperties {
		[
#if !SL
	DevExpressPersistentBaseLocalizedDescription("IRuleSupportsCollectionAggregatesPropertiesTargetCollectionAggregate"),
#endif
 Category("Behavior")]
		Aggregate? TargetCollectionAggregate { get; set; }
	}
	[DomainComponent]
	[Description("Defines common templates for the messages displayed when property value checking validation rules are broken.")]
	public interface IRulePropertyValueProperties : IRuleCollectionPropertyProperties {
		[RulePropertiesMemberOf("TargetType")]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RulePropertyValuePropertiesTargetPropertyName")]
#endif
		[Required]
		string TargetPropertyName { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleRequiredField")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleRequiredField validation rules are broken.")]
	public interface IRuleRequiredFieldProperties : IRulePropertyValueProperties {
		[DefaultValue(false)]
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleBasePropertiesSkipNullOrEmptyValues")]
#endif
		new bool SkipNullOrEmptyValues { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustNotBeEmpty)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRequiredFieldPropertiesMessageTemplateMustNotBeEmpty")]
#endif
		string MessageTemplateMustNotBeEmpty { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleFromBoolProperty")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleFromBoolProperty validation rules are broken.")]
	public interface IRuleFromBoolPropertyProperties : IRulePropertyValueProperties {
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeTrue)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleFromBoolPropertyPropertiesMessageTemplateMustBeTrue")]
#endif
		string MessageTemplateMustBeTrue { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleFromBoolPropertyPropertiesUsedProperties")]
#endif
		string UsedProperties { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleRange")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleRange validation rules are broken.")]
	public interface IRuleRangeProperties : IRulePropertyValueProperties, IRuleSupportsCollectionAggregatesProperties {
		[Category("Data")]
		[TypeConverter(typeof(StringConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRangePropertiesMinimumValue")]
#endif
		object MinimumValue { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRangePropertiesMinimumValueExpression")]
#endif
		string MinimumValueExpression { get; set; }
		[Category("Data")]
		[TypeConverter(typeof(StringConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRangePropertiesMaximumValue")]
#endif
		object MaximumValue { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRangePropertiesMaximumValueExpression")]
#endif
		string MaximumValueExpression { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeInRange)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRangePropertiesMessageTemplateMustBeInRange")]
#endif
		string MessageTemplateMustBeInRange { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleValueComparison")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleValueComparison validation rules are broken.")]
	public interface IRuleValueComparisonProperties : IRulePropertyValueProperties, IRuleSupportsCollectionAggregatesProperties {
		[Category("Data")]
		[TypeConverter(typeof(StringConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesRightOperand")]
#endif
		object RightOperand { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesRightOperand")]
#endif
		string RightOperandExpression { get; set; }
		[Required]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesOperatorType")]
#endif
		ValueComparisonType OperatorType { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeEqualToOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustBeEqualToOperand")]
#endif
		string MessageTemplateMustBeEqualToOperand { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeGreaterThanOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustBeGreaterThanOperand")]
#endif
		string MessageTemplateMustBeGreaterThanOperand { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeGreaterThanOrEqualToOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustBeGreaterThanOrEqualToOperand")]
#endif
		string MessageTemplateMustBeGreaterThanOrEqualToOperand { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeLessThanOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustBeLessThanOperand")]
#endif
		string MessageTemplateMustBeLessThanOperand { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeLessThanOrEqualToOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustBeLessThanOrEqualToOperand")]
#endif
		string MessageTemplateMustBeLessThanOrEqualToOperand { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustNotBeEqualToOperand)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleValueComparisonPropertiesMessageTemplateMustNotBeEqualToOperand")]
#endif
		string MessageTemplateMustNotBeEqualToOperand { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleStringComparison")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleStringComparison validation rules are broken.")]
	public interface IRuleStringComparisonProperties : IRulePropertyValueProperties {
		[Required]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesOperatorType")]
#endif
		StringComparisonType OperatorType { get; set; }
		[Localizable(true)]
		[DefaultValue("")]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesOperandValue")]
#endif
		string OperandValue { get; set; }
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesIgnoreCase")]
#endif
		bool IgnoreCase { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustContain)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesMessageTemplateMustContain")]
#endif
		string MessageTemplateMustContain { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeginWith)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesMessageTemplateMustBeginWith")]
#endif
		string MessageTemplateMustBeginWith { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustEndWith)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesMessageTemplateMustEndWith")]
#endif
		string MessageTemplateMustEndWith { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeEqual)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesMessageTemplateMustBeEqual")]
#endif
		string MessageTemplateMustBeEqual { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustNotBeEqual)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleStringComparisonPropertiesMessageTemplateMustNotBeEqual")]
#endif
		string MessageTemplateMustNotBeEqual { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleRegularExpression")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleRegularExpression validation rules are broken.")]
	public interface IRuleRegularExpressionProperties : IRulePropertyValueProperties {
		[Required]
		[Localizable(true)]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRegularExpressionPropertiesPattern")]
#endif
		string Pattern { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustMatchPattern)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleRegularExpressionPropertiesMessageTemplateMustMatchPattern")]
#endif
		string MessageTemplateMustMatchPattern { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleCriteria")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleCriteria validation rules are broken.")]
	public interface IRuleCriteriaProperties : IRuleCollectionPropertyProperties {
		[Required]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleCriteriaPropertiesCriteria")]
#endif
		[CriteriaOptions("TargetType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string Criteria { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustSatisfyCriteria)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleCriteriaPropertiesMessageTemplateMustSatisfyCriteria")]
#endif
		string MessageTemplateMustSatisfyCriteria { get; set; }
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleCriteriaPropertiesUsedProperties")]
#endif
		string UsedProperties { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleSearchObject")]
	[DomainComponent]
	[Description("Defines common templates for the messages displayed when object searching validation rules are broken.")]
	public interface IRuleSearchObjectProperties : IRuleCollectionPropertyProperties {
		[Localizable(true)]
		[DefaultValue(", ")]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesFoundObjectMessagesSeparator")]
#endif
		string FoundObjectMessagesSeparator { get; set; }
		[Localizable(true)]
		[DefaultValue("{0}")]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesFoundObjectMessageFormat")]
#endif
		string FoundObjectMessageFormat { get; set; }
		[DefaultValue(CriteriaEvaluationBehavior.InTransaction)]
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesCriteriaEvaluationBehavior")]
#endif
		CriteriaEvaluationBehavior CriteriaEvaluationBehavior { get; set; }
		[Category("Behavior")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesIncludeCurrentObject")]
#endif
		bool IncludeCurrentObject { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.FoundObjects)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesMessageTemplateFoundObjects")]
#endif
		string MessageTemplateFoundObjects { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleObjectExists")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleObjectExists validation rules are broken.")]
	public interface IRuleObjectExistsProperties : IRuleSearchObjectProperties {
		[Required]
		[Localizable(true)]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleObjectExistsPropertiesCriteria")]
#endif
		[CriteriaOptions("TargetType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string Criteria { get; set; }
		[Category("Data")]
		[TypeConverter(typeof(StringToTypeConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleObjectExistsPropertiesLooksFor")]
#endif
		Type LooksFor { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustExist)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleObjectExistsPropertiesMessageTemplateMustExist")]
#endif
		string MessageTemplateMustExist { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleUniqueValue")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleUniqueValue validation rules are broken.")]
	public interface IRuleUniqueValueProperties : IRuleSearchObjectProperties, IRulePropertyValueProperties {
		[Localizable(true)]
		[DefaultValue("")]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleSearchObjectPropertiesFoundObjectMessageFormat")]
#endif
		new string FoundObjectMessageFormat { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeUnique)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleUniqueValuePropertiesMessageTemplateMustBeUnique")]
#endif
		string MessageTemplateMustBeUnique { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleIsReferenced")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleIsReferenced validation rules are broken.")]
	public interface IRuleIsReferencedProperties : IRuleSearchObjectProperties {
		[Required]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleIsReferencedPropertiesReferencePropertyName")]
#endif
		string ReferencePropertyName { get; set; }
		[Category("Data")]
		[TypeConverter(typeof(StringToTypeConverter))]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleIsReferencedPropertiesLooksFor")]
#endif
		Type LooksFor { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.MustBeReferenced)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleIsReferencedPropertiesMessageTemplateMustBeReferenced")]
#endif
		string MessageTemplateMustBeReferenced { get; set; }
	}
	[GenerateMessageTemplatesModel("RuleCombinationOfPropertiesIsUnique")]
	[DomainComponent]
	[Description("Defines templates for the messages displayed when RuleCombinationOfPropertiesIsUnique validation rules are broken.")]
	public interface IRuleCombinationOfPropertiesIsUniqueProperties : IRuleSearchObjectProperties {
		[Required]
		[Category("Data")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleCombinationOfPropertiesIsUniquePropertiesTargetProperties")]
#endif
		string TargetProperties { get; set; }
		[Localizable(true)]
		[DefaultValue(RuleDefaultMessageTemplates.CombinationOfPropertiesMustBeUnique)]
		[Category("Format")]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("RuleCombinationOfPropertiesIsUniquePropertiesMessageTemplateCombinationOfPropertiesMustBeUnique")]
#endif
		string MessageTemplateCombinationOfPropertiesMustBeUnique { get; set; }
	}
	public class DefaultValidationContextsConverter : StringConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(Enum.GetNames(typeof(DefaultContexts)));
		}
	}
}
