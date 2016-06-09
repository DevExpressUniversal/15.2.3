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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	public interface IRuleBaseAttribute {
		IRule CreateRule();
		Type DeclaringClass { get; set; }
		string DeclaringProperty { get; set; }
	}
	public abstract class RuleBaseAttribute : Attribute, IRuleBaseAttribute, IRuleCollectionPropertyProperties {
		internal RuleBaseProperties properties;
		internal protected Dictionary<string, object> propertiesDictionary = new Dictionary<string, object>(); 
		private Type declaringClass;
		private string declaringProperty;
		private bool isDeclaringClassInitialized = false;
		private bool isDeclaringPropertyInitialized = false;
		internal RuleBaseAttribute(string id, ContextIdentifiers contextIDs)
			: this() {
			propertiesDictionary["Id"] = id;
			propertiesDictionary["Name"] = id;
			propertiesDictionary["TargetContextIDs"] = contextIDs.ToString();
		}
		private RuleBaseAttribute(string id, ContextIdentifiers contextIDs, string messageTemplate)
			: this(id, contextIDs) {
			propertiesDictionary["CustomMessageTemplate"] = messageTemplate;
		}
		private void UpdateId() {
			if(!string.IsNullOrEmpty(Properties.Id)) {
				return;
			}
			string ruleId = RuleType.FullName;
			IRuleBaseAttribute thisInterface = (IRuleBaseAttribute)this;
			if(thisInterface.DeclaringClass != null) {
				ruleId += string.Format("_{0}", thisInterface.DeclaringClass.FullName);
			}
			if(!string.IsNullOrEmpty(thisInterface.DeclaringProperty)) {
				ruleId += string.Format("_{0}", thisInterface.DeclaringProperty);
			}
			string postfixedRuleIdTemplate = ruleId + "_{0}";
			int ruleIdPostfix = 0;
			while(Validator.RuleSet.RegisteredRules.Find(new Predicate<IRule>(delegate(IRule item) { return item.Id == ruleId; })) != null) {
				ruleId = string.Format(postfixedRuleIdTemplate, ++ruleIdPostfix);
			}
			Properties.Id = ruleId;
		}
		private void EnsurePropertiesAreCorrectlyAssigned() {
			if(Properties.TargetType == null) {
				throw new InvalidOperationException("TargetType must be set.");
			}
			IRulePropertyValueProperties propertyValueProperties = Properties as IRulePropertyValueProperties;
			if(propertyValueProperties != null) {
				if(string.IsNullOrEmpty(propertyValueProperties.TargetPropertyName)) {
					IRuleSupportsCollectionAggregatesProperties aggregatedProperties = Properties as IRuleSupportsCollectionAggregatesProperties;
					if(aggregatedProperties == null || aggregatedProperties.TargetCollectionAggregate.Value != Aggregate.Count) {
						throw new InvalidOperationException("TargetPropertyName must be set.");
					}
				}
				else {
					ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(propertyValueProperties.TargetType);
					if(targetTypeInfo.FindMember(propertyValueProperties.TargetPropertyName) == null) {
						throw new MemberNotFoundException(propertyValueProperties.TargetType, propertyValueProperties.TargetPropertyName);
					}
				}
			}
			if(Properties.TargetCollectionOwnerType != null) {
				if(string.IsNullOrEmpty(Properties.TargetCollectionPropertyName)) {
					throw new InvalidOperationException("TargetCollectionPropertyName must be set.");
				}
				ITypeInfo targetCollectionOwnerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(Properties.TargetCollectionOwnerType);
				IMemberInfo collectionPropertyMemberInfo = targetCollectionOwnerTypeInfo.FindMember(Properties.TargetCollectionPropertyName);
				if(collectionPropertyMemberInfo == null) {
					throw new MemberNotFoundException(Properties.TargetCollectionOwnerType, Properties.TargetCollectionPropertyName);
				}
				if(!collectionPropertyMemberInfo.IsList) {
					throw new InvalidOperationException(string.Format("The {0}.{1} must be a collection property.", Properties.TargetCollectionOwnerType, Properties.TargetCollectionPropertyName));
				}
			}
			EnsurePropertiesAreCorrectlyAssignedCore();
		}
		private bool CheckIfCollectionPropertyRuleAttribute(IMemberInfo declaringMemberInfo) {
			if(!declaringMemberInfo.IsList) {
				return false;
			}
			if(!(Properties is IRulePropertyValueProperties)) {
				return true;
			}
			return CheckIfCollectionPropertyRuleAttributeCore();
		}
		protected internal RuleBaseAttribute Clone() {
			RuleBaseAttribute clonedAttribute = (RuleBaseAttribute)this.MemberwiseClone();
			if(properties != null) {
				clonedAttribute.SetProperties(Properties.Clone());
			}
			else {
				clonedAttribute.propertiesDictionary = propertiesDictionary;
			}
			return clonedAttribute;
		}
		protected virtual void EnsurePropertiesAreCorrectlyAssignedCore() { }
		protected virtual bool CheckIfCollectionPropertyRuleAttributeCore() {
			return true;
		}
		protected void SetProperties(RuleBaseProperties newProperties) {
			if(newProperties == null) {
				throw new ArgumentNullException();
			}
			if(newProperties.GetType() != PropertiesType) {
				ReflectionHelper.ThrowInvalidCastException(PropertiesType, newProperties.GetType());
			}
			properties = newProperties;
		}
		protected RuleBaseProperties CreateProperties() {
			RuleBaseProperties properties = (RuleBaseProperties)TypeHelper.CreateInstance(PropertiesType);
			RulePropertiesHelper.AssignDefaultValues(properties, RuleType);
			foreach(string key in propertiesDictionary.Keys) {
				TypeHelper.SetPropertyValue(properties, key, propertiesDictionary[key]);
			}
			return properties;
		}
		protected IRule CreateRuleCore() {
			if(properties != null) {
				return (IRule)TypeHelper.CreateInstance(RuleType, Properties);
			}
			else {
				return (IRule)TypeHelper.CreateInstance(RuleType);
			}
		}
		protected virtual void OnSetDeclaringClass(Type classType) { }
		protected virtual void OnSetDeclaringProperty(string propertyName) { }
		protected RuleBaseAttribute(string id, string targetContextIDs)
			: this(id, (ContextIdentifiers)targetContextIDs) { }
		protected RuleBaseAttribute(DefaultContexts targetContexts)
			: this(null, (ContextIdentifiers)targetContexts) { }
		protected RuleBaseAttribute(string id, DefaultContexts targetContexts)
			: this(id, (ContextIdentifiers)targetContexts) { }
		protected RuleBaseAttribute(string id, string targetContextIDs, string messageTemplate)
			: this(id, (ContextIdentifiers)targetContextIDs, messageTemplate) { }
		protected RuleBaseAttribute(string id, DefaultContexts targetContexts, string messageTemplate)
			: this(id, (ContextIdentifiers)targetContexts, messageTemplate) { }
		public RuleBaseAttribute() {
		}
		protected abstract Type RuleType { get; }
		protected virtual Type PropertiesType {
			get {
				return typeof(RuleBaseProperties);
			}
		}
		protected RuleBaseProperties Properties {
			get {
				if(properties == null) {
					properties = CreateProperties();
				}
				return properties;
			}
		}
		#region IRuleBaseAttribute members
		IRule IRuleBaseAttribute.CreateRule() {
			UpdateId();
			EnsurePropertiesAreCorrectlyAssigned();
			IRule result = CreateRuleCore();
			if(result is ISupportCheckRuleIntegrity) {
				((ISupportCheckRuleIntegrity)result).CheckRuleIntegrity();
			}
			return result;
		}
		Type IRuleBaseAttribute.DeclaringClass {
			get { return declaringClass; }
			set {
				if(isDeclaringClassInitialized) {
					return;
				}
				declaringClass = value;
				Properties.TargetType = declaringClass;
				OnSetDeclaringClass(declaringClass);
				isDeclaringClassInitialized = true;
			}
		}
		string IRuleBaseAttribute.DeclaringProperty {
			get { return declaringProperty; }
			set {
				if(declaringClass == null) {
					throw new InvalidOperationException("DeclaringClass must be assigned prior to DeclaringProperty.");
				}
				if(isDeclaringPropertyInitialized) {
					return;
				}
				declaringProperty = value;
				IRulePropertyValueProperties propertyValueProperties = Properties as IRulePropertyValueProperties;
				bool isTargetPropertyNameAssigned = (propertyValueProperties != null && !string.IsNullOrEmpty(propertyValueProperties.TargetPropertyName));
				ITypeInfo declaringTypeInfo = XafTypesInfo.Instance.FindTypeInfo(declaringClass);
				IMemberInfo declaringMemberInfo = declaringTypeInfo.FindMember(declaringProperty);
				if(declaringMemberInfo == null) {
					throw new MemberNotFoundException(declaringClass, declaringProperty);
				}
				if(CheckIfCollectionPropertyRuleAttribute(declaringMemberInfo)) {
					Properties.TargetCollectionOwnerType = declaringMemberInfo.Owner.Type;
					Properties.TargetCollectionPropertyName = declaringMemberInfo.Name;
					Properties.TargetType = declaringMemberInfo.ListElementType;
				}
				else {
					if(propertyValueProperties == null) {
						throw new InvalidOperationException(string.Format("The '{0}' rule can be applied only to class or collection property.", RuleType.Name));
					}
					else {
						if(isTargetPropertyNameAssigned) {
							throw new InvalidOperationException(string.Format("Direct TargetPropertyName assignment is allowed only for rule attributes applied to a collection property.\r\n Rule: {0}, TargetPropertyName: {1}", propertyValueProperties.Id, propertyValueProperties.TargetPropertyName));
						}
						propertyValueProperties.TargetPropertyName = declaringProperty;
					}
				}
				OnSetDeclaringProperty(declaringProperty);
				isDeclaringPropertyInitialized = true;
			}
		}
		#endregion
		public string CustomMessageTemplate {
			get { return Properties.CustomMessageTemplate; }
			set {
				if(properties == null) {
					propertiesDictionary["CustomMessageTemplate"] = value;
				}
				else {
					Properties.CustomMessageTemplate = value;
				}
			}
		}
		public string TargetContextIDs {
			get { return Properties.TargetContextIDs; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetContextIDs"] = value;
				}
				else {
					Properties.TargetContextIDs = value;
				}				
			}
		}
		public bool InvertResult {
			get { return Properties.InvertResult; }
			set {
				if(properties == null) {
					propertiesDictionary["InvertResult"] = value;
				}
				else {
					Properties.InvertResult = value;
				}				
			}
		}
		public bool SkipNullOrEmptyValues {
			get { return Properties.SkipNullOrEmptyValues; }
			set {
				if(properties == null) {
					propertiesDictionary["SkipNullOrEmptyValues"] = value;
				}
				else {
					Properties.SkipNullOrEmptyValues = value;
				}				
			}
		}
		public ValidationResultType ResultType {
			get { return Properties.ResultType; }
			set {
				if(properties == null) {
					propertiesDictionary["ResultType"] = value;
				}
				else {
					Properties.ResultType = value;
				}				
			}
		}
		public string TargetCriteria {
			get { return Properties.TargetCriteria; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetCriteria"] = value;
				}
				else {
					Properties.TargetCriteria = value;
				}
			}
		}
		string IRuleBaseProperties.CustomMessageTemplate {
			get { return Properties.CustomMessageTemplate; }
			set { Properties.CustomMessageTemplate = value; }
		}
		string IRuleBaseProperties.Id {
			get {
				return Properties.Id;
			}
			set {
				Properties.Id = value;
			}
		}
		public string Name {
			get {
				return Properties.Name;
			}
			set {
				if(properties == null) {
					propertiesDictionary["Name"] = value;
				}
				else {
					Properties.Name = value;
				}
			}
		}
		Type IRuleBaseProperties.TargetType {
			get {
				return Properties.TargetType;
			}
			set {
				Properties.TargetType = value;
			}
		}
		ValidationResultType IRuleBaseProperties.ResultType {
			get { return Properties.ResultType; }
			set { Properties.ResultType = value; }
		}
		string IRuleBaseProperties.MessageTemplateSkipNullOrEmptyValues {
			get { return Properties.MessageTemplateSkipNullOrEmptyValues; }
			set { Properties.MessageTemplateSkipNullOrEmptyValues = value; }
		}
		string IRuleBaseProperties.MessageTemplateTargetDoesNotSatisfyTargetCriteria {
			get { return Properties.MessageTemplateTargetDoesNotSatisfyTargetCriteria; }
			set { Properties.MessageTemplateTargetDoesNotSatisfyTargetCriteria = value; }
		}
		string IRuleCollectionPropertyProperties.MessageTemplateTargetDoesNotSatisfyCollectionCriteria {
			get { return Properties.MessageTemplateTargetDoesNotSatisfyCollectionCriteria; }
			set { Properties.MessageTemplateTargetDoesNotSatisfyCollectionCriteria = value; }
		}
		string IRuleCollectionPropertyProperties.MessageTemplateCollectionValidationMessageSuffix {
			get { return Properties.MessageTemplateCollectionValidationMessageSuffix; }
			set { Properties.MessageTemplateCollectionValidationMessageSuffix = value; }
		}
		Type IRuleCollectionPropertyProperties.TargetCollectionOwnerType {
			get { return Properties.TargetCollectionOwnerType; }
			set { Properties.TargetCollectionOwnerType = value; }
		}
		string IRuleCollectionPropertyProperties.TargetCollectionPropertyName {
			get { return Properties.TargetCollectionPropertyName; }
			set { Properties.TargetCollectionPropertyName = value; }
		}
	}
}
