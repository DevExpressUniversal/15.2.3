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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	public enum ParametersMode{
		Value,
		Expression
	}
	public static class UsedPropertiesStringHelper {
		private const string usedPropertiesDelimiters = ",:; ";
		public static ReadOnlyCollection<string> ParseUsedPropertiesString(string usedPropertiesString, Type targetType) {
			List<string> result = new List<string>();
			if(!string.IsNullOrEmpty(usedPropertiesString)) {
				foreach(string propertyName in usedPropertiesString.Split(usedPropertiesDelimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
					string trimmedPropertyName = propertyName.Trim();
					if(XafTypesInfo.Instance.FindTypeInfo(targetType).FindMember(trimmedPropertyName) == null) {
						throw new Exception(string.Format("TargetType doesn't contain the '{0}' property", trimmedPropertyName));
					}
					if(!result.Contains(trimmedPropertyName)) {
						result.Add(trimmedPropertyName);
					}
				}
			}
			return new ReadOnlyCollection<string>(result);
		}
	}
	public abstract class RulePropertyValue : RuleBase {
		public const string PropertiesTargetCollectionPropertyName = "TargetCollectionPropertyName"; 
		public const string PropertiesTargetPropertyName = "TargetPropertyName"; 
		private ReadOnlyCollection<string> usedProperties;
		protected const string targetPropertyValueTemplate = "{TargetValue}";
		[Obsolete("This constructor is obsolete. Use the constructor which takes an IMemberInfo argument instead.", true)]
		protected RulePropertyValue(string id, PropertyInfo property, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, targetContextIDs, objectType) {
			if(property == null) {
				throw new ArgumentException(string.Format("Unable to instatiate a Validation Rule. Cannot find the specified target property within the {0} class.\r\nRule Id: '{1}'\r\nRule Type: '{2}'", objectType, id, GetType().FullName));
			}
			if(!property.DeclaringType.IsAssignableFrom(objectType)) {
				throw new ArgumentException(string.Format(
					"Can't instantiate RulePropertyValue for property of class {0}. Property {1} declared in incompatible class ({2})",
					objectType, property.Name, property.DeclaringType));
			}
			this.Properties.TargetPropertyName = property.Name;
		}
		protected RulePropertyValue(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, targetContextIDs, objectType) {
			if(property == null) {
				throw new ArgumentException(string.Format("Unable to instatiate a Validation Rule. Cannot find the specified target property within the {0} class.\r\nRule Id: '{1}'\r\nRule Type: '{2}'", objectType, id, GetType().FullName));
			}
			if(!property.Owner.Type.IsAssignableFrom(objectType)) {
				throw new ArgumentException(string.Format(
					"Can't instantiate RulePropertyValue for property of class {0}. Property {1} declared in incompatible class ({2})",
					objectType, property.Name, property.Owner.Type));
			}
			this.Properties.TargetPropertyName = property.Name;
		}
		public RulePropertyValue() { }
		public RulePropertyValue(IRulePropertyValueProperties properties) : base(properties) { }
		public new IRulePropertyValueProperties Properties {
			get { return (IRulePropertyValueProperties)base.Properties; }
		}
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				if(usedProperties == null) {
					if(string.IsNullOrEmpty(Properties.TargetPropertyName)) {
						usedProperties = base.UsedProperties;
					}
					else {
						usedProperties = new ReadOnlyCollection<string>(new string[] { Properties.TargetPropertyName });
					}
				}
				return usedProperties;
			}
		}
		public IMemberInfo TargetMember {
			get {
				Guard.ArgumentNotNull(Properties.TargetType, "Properties.TargetType");
				ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(Properties.TargetType);
				if(!string.IsNullOrEmpty(Properties.TargetPropertyName)) {
					return targetTypeInfo.FindMember(Properties.TargetPropertyName);
				}
				return null;
			}
		}
		public object TargetValue {
			get {
				if(TargetMember != null && TargetObject != null) {
					return TargetMember.GetValue(TargetObject);
				}
				return null;
			}
		}
	}
	public abstract class RulePropertyValue<TPropertyValue> : RulePropertyValue {
		private object aggregatedTargetValue;
		protected RulePropertyValue(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, property, targetContextIDs, objectType) {
			((ISupportCheckRuleIntegrity)this).CheckRuleIntegrity();
		}
		protected RulePropertyValue(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : this(id, property, targetContextIDs, property.Owner.Type) { }
		protected abstract bool IsValueValid(TPropertyValue value, out string errorMessageTemplate);
		protected override void CheckRuleIntegrityCore() {
			if(TargetMember != null) {
				if(!typeof(TPropertyValue).IsAssignableFrom(TargetMember.MemberType)) {
					bool checkPassed = 
						AreNullableTargetsAllowed 
						&& typeof(TPropertyValue).IsAssignableFrom(Nullable.GetUnderlyingType(TargetMember.MemberType));
					if(!checkPassed) {
				throw new ArgumentException(string.Format(
					"Can't instantiate {0} for property {1} of class {2}. Property must return {3} or any successor of it.",
							GetType().Name, Properties.TargetPropertyName, Properties.TargetType, typeof(TPropertyValue)));
			}
		}
			}
			base.CheckRuleIntegrityCore();
		}
		protected string GetCollectionCriteria() {
			string collectionCriteria = string.Format(
				"{0}.{1}",
				RuleCollectionPropertyTargetCriteriaHelper.GetAssociatedMember(Properties).Name,
				Properties.TargetCollectionPropertyName);
			if(!string.IsNullOrEmpty(Properties.TargetCriteria)) {
				collectionCriteria += string.Format("[{0}]", Properties.TargetCriteria);
			}
			return collectionCriteria;
		}
		protected object GetTargetValue(object target) {
			aggregatedTargetValue = null;
			if(IsCollectionPropertyRule && Properties is IRuleSupportsCollectionAggregatesProperties) {
				Aggregate? aggregate = ((IRuleSupportsCollectionAggregatesProperties)Properties).TargetCollectionAggregate;
				if(aggregate.HasValue && aggregate.Value != Aggregate.Exists) {
					string aggregateCriteria = GetCollectionCriteria();
					aggregateCriteria += string.Format(".{0}", aggregate.Value.ToString());
					if(!string.IsNullOrEmpty(Properties.TargetPropertyName) && aggregate.Value != Aggregate.Count) {
						aggregateCriteria += string.Format("({0})", Properties.TargetPropertyName);
					}
					ExpressionEvaluator evaluator = CreateEvaluator(target, CriteriaOperator.Parse(aggregateCriteria));
					aggregatedTargetValue = evaluator.Evaluate(target);
					return aggregatedTargetValue;
				}
			}
			Guard.ArgumentNotNull(TargetMember, "TargetMember");
			return TargetMember.GetValue(target);
		}
		protected bool? PreValidateAggregates(object target, out string errorMessageTemplate) {
			if(IsCollectionPropertyRule && Properties is IRuleSupportsCollectionAggregatesProperties) {
				Aggregate? aggregate = ((IRuleSupportsCollectionAggregatesProperties)Properties).TargetCollectionAggregate;
				if(aggregate.HasValue && aggregate.Value == Aggregate.Exists) {
					object owner = RuleCollectionPropertyTargetCriteriaHelper.GetAssociatedMember(Properties).GetValue(target);
					IEnumerable collection = RuleCollectionPropertyTargetCriteriaHelper.GetCollectionProperty(Properties).GetValue(owner) as IEnumerable;
					ArrayList passedObjects = new ArrayList();
					string message = string.Empty;
					ExpressionEvaluator evaluator = CreateEvaluator(target, CriteriaOperator.Parse(Properties.TargetCriteria));
					foreach(object item in collection) {
						if(evaluator.Fit(item)) {
							if(IsValueValid((TPropertyValue)GetTargetValue(item), out message)) {
								passedObjects.Add(item);
							}
						}
					}
					if(string.IsNullOrEmpty(message)) {
						errorMessageTemplate = Properties.CustomMessageTemplate;
					}
					else {
						errorMessageTemplate = message;
					}
					return passedObjects.Count > 0;
				}
			}
			errorMessageTemplate = string.Empty;
			return null;
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			if(IsCollectionPropertyRule && Properties is IRuleSupportsCollectionAggregatesProperties) {
				bool? validationResult = PreValidateAggregates(target, out errorMessageTemplate);
				if(validationResult.HasValue) {
					return validationResult.Value;
				}
			}
			return IsValueValid((TPropertyValue)GetTargetValue(target), out errorMessageTemplate);
		}
		public RulePropertyValue() { }
		public RulePropertyValue(IRulePropertyValueProperties properties) : base(properties) { }
		protected virtual bool AreNullableTargetsAllowed {
			get { return true; }
		}
		public object AggregatedTargetValue {
			get { return aggregatedTargetValue; }
		}
	}
	public class RuleRequiredField : RulePropertyValue<object> {
		public const string PropertiesMessageTemplateMustNotBeEmpty = "MessageTemplateMustNotBeEmpty";
		public static string DefaultMessageTemplateMustNotBeEmpty {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleRequiredField_defaultMessageTemplateMustNotBeEmpty");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustNotBeEmpty;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleRequiredField_defaultMessageTemplateMustNotBeEmpty").Value = value; }
		}
		protected override bool IsValueValid(object value, out string errorMessageTemplate) {
			bool result = true;
			if(RuleSet.IsEmptyValue(TargetObject, TargetMember.Name, value)) {
				result = false;
			}
			else {
				IEnumerable enumerable = value as IEnumerable;
				if(enumerable != null && !(value is string)) {
					ICollection collection = enumerable as ICollection;
					if(collection != null) {
						result = collection.Count > 0;
					}
					result = enumerable.GetEnumerator().MoveNext();
				}
			}
			errorMessageTemplate = Properties.MessageTemplateMustNotBeEmpty;
			return result;
		}
		public RuleRequiredField(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
		public RuleRequiredField(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
		public RuleRequiredField() {}
		public RuleRequiredField(IRuleRequiredFieldProperties properties) : base(properties) { }
		public new IRuleRequiredFieldProperties Properties {
			get {
				return (IRuleRequiredFieldProperties)base.Properties;
			}
		}
		public override Type PropertiesType {
			get { return typeof(RuleRequiredFieldProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleRequiredFieldAttribute : RuleBaseAttribute, IRuleRequiredFieldProperties {
		protected override bool CheckIfCollectionPropertyRuleAttributeCore() {
			return !string.IsNullOrEmpty(Properties.TargetPropertyName);
		}
		public RuleRequiredFieldAttribute(string id, string targetContextIDs)
			: base(id, targetContextIDs) {
		}
		public RuleRequiredFieldAttribute(DefaultContexts targetContexts)
			: base(null, targetContexts) {
		}
		public RuleRequiredFieldAttribute(string id, DefaultContexts targetContexts)
			: base(id, targetContexts) {
		}
		public RuleRequiredFieldAttribute(string id, string targetContextIDs, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
		}
		public RuleRequiredFieldAttribute(string id, DefaultContexts targetContexts, string messageTemplate)
			: base(id, targetContexts, messageTemplate) {
		}
		public RuleRequiredFieldAttribute()
			: base(null, DefaultContexts.Save) {
		}
		protected new IRuleRequiredFieldProperties Properties {
			get { return (IRuleRequiredFieldProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleRequiredField); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleRequiredFieldProperties); }
		}
		public string TargetPropertyName {
			get {
				return Properties.TargetPropertyName;
			}
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		string IRuleRequiredFieldProperties.MessageTemplateMustNotBeEmpty {
			get { return Properties.MessageTemplateMustNotBeEmpty;	}
			set { Properties.MessageTemplateMustNotBeEmpty = value; }
		}
	}
	public class RuleFromBoolProperty : RulePropertyValue<bool> {
		private const string usedPropertiesDelimiters = ",:; ";
		public const string PropertiesMessageTemplateMustBeTrue = "MessageTemplateMustBeTrue";
		public static string DefaultMessageTemplateMustBeTrue {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleFromBoolProperty_defaultMessageTemplateMustBeTrue");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeTrue;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleFromBoolProperty_defaultMessageTemplateMustBeTrue").Value = value; }
		}
		protected override bool AreNullableTargetsAllowed {
			get { return false; }
		}
		protected override bool IsValueValid(bool value, out string errorMessageTemplate) {
			errorMessageTemplate = Properties.MessageTemplateMustBeTrue;
			return value;
		}
		public RuleFromBoolProperty(string id, IMemberInfo property, ContextIdentifiers targetContextIDs)
			: base(id, property, targetContextIDs, property.Owner.Type) { }
		public RuleFromBoolProperty() { }
		public RuleFromBoolProperty(IRuleFromBoolPropertyProperties properties) : base(properties) { }
		public new IRuleFromBoolPropertyProperties Properties {
			get {
				return (IRuleFromBoolPropertyProperties)base.Properties;
			}
		}
		public override Type PropertiesType {
			get { return typeof(RuleFromBoolPropertyProperties); }
		}
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				if(!string.IsNullOrEmpty(Properties.UsedProperties)) {
					return UsedPropertiesStringHelper.ParseUsedPropertiesString(Properties.UsedProperties, Properties.TargetType);
					}
				return base.UsedProperties;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleFromBoolPropertyAttribute : RuleBaseAttribute, IRuleFromBoolPropertyProperties {
		public RuleFromBoolPropertyAttribute()
			: this(null, DefaultContexts.Save) { }
		public RuleFromBoolPropertyAttribute(string id, string targetContextIDs)
			: base(id, targetContextIDs) { }
		public RuleFromBoolPropertyAttribute(DefaultContexts targetContexts)
			: base(null, targetContexts) { }
		public RuleFromBoolPropertyAttribute(string id, DefaultContexts targetContexts)
			: base(id, targetContexts) { }
		public RuleFromBoolPropertyAttribute(string id, string targetContextIDs, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) { }
		public RuleFromBoolPropertyAttribute(string id, DefaultContexts targetContexts, string messageTemplate)
			: base(id, targetContexts, messageTemplate) { }
		protected new IRuleFromBoolPropertyProperties Properties {
			get { return (IRuleFromBoolPropertyProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleFromBoolProperty); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleFromBoolPropertyProperties); }
		}
		public string TargetPropertyName {
			get {
				return Properties.TargetPropertyName;
			}
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		string IRuleFromBoolPropertyProperties.MessageTemplateMustBeTrue {
			get { return Properties.MessageTemplateMustBeTrue; }
			set { Properties.MessageTemplateMustBeTrue = value; }
		}
		public string UsedProperties {
			get {
				return Properties.UsedProperties;
			}
			set {
				if(properties == null) {
					propertiesDictionary["UsedProperties"] = value;
				}
				else {
					Properties.UsedProperties = value;
				}
			}
		}
	}
	public enum ValueComparisonType { Equals, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, NotEquals };
	public class RuleValueComparison : RulePropertyValue<IComparable> {
		public const string PropertiesRightOperand = "RightOperand";
		public const string PropertiesRightOperandExpression = "RightOperandExpression";
		public const string PropertiesOperatorType = "OperatorType";
		public const string PropertiesMessageTemplateMustBeEqualToOperand = "MessageTemplateMustBeEqualToOperand";
		public const string PropertiesMessageTemplateMustBeGreaterThanOperand = "MessageTemplateMustBeGreaterThanOperand";
		public const string PropertiesMessageTemplateMustBeGreaterThanOrEqualToOperand = "MessageTemplateMustBeGreaterThanOrEqualToOperand";
		public const string PropertiesMessageTemplateMustBeLessThanOperand = "MessageTemplateMustBeLessThanOperand";
		public const string PropertiesMessageTemplateMustBeLessThanOrEqualToOperand = "MessageTemplateMustBeLessThanOrEqualToOperand";
		public const string PropertiesMessageTemplateMustNotBeEqualToOperand = "MessageTemplateMustNotBeEqualToOperand"; 
		public static string DefaultMessageTemplateMustBeEqualToOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeEqualToOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeEqualToOperand;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeEqualToOperand").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeGreaterThanOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeGreaterThanOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeGreaterThanOperand;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeGreaterThanOperand").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeGreaterThanOrEqualToOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeGreaterThanOrEqualToOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeGreaterThanOrEqualToOperand;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeGreaterThanOrEqualToOperand").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeLessThanOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeLessThanOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeLessThanOperand;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeLessThanOperand").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeLessThanOrEqualToOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeLessThanOrEqualToOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeLessThanOrEqualToOperand;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustBeLessThanOrEqualToOperand").Value = value; }
		}
		public static string DefaultMessageTemplateMustNotBeEqualToOperand {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustNotBeEqualToOperand");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustNotBeEqualToOperand;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleValueComparison_defaultMessageTemplateMustNotBeEqualToOperand").Value = value; }
		}
		private string GetMessageTemplate() {
			switch(Properties.OperatorType) {
				case ValueComparisonType.Equals:
					return Properties.MessageTemplateMustBeEqualToOperand;
				case ValueComparisonType.GreaterThan:
					return Properties.MessageTemplateMustBeGreaterThanOperand;
				case ValueComparisonType.GreaterThanOrEqual:
					return Properties.MessageTemplateMustBeGreaterThanOrEqualToOperand;
				case ValueComparisonType.LessThan:
					return Properties.MessageTemplateMustBeLessThanOperand;
				case ValueComparisonType.LessThanOrEqual:
					return Properties.MessageTemplateMustBeLessThanOrEqualToOperand;
				case ValueComparisonType.NotEquals:
					return Properties.MessageTemplateMustNotBeEqualToOperand;
			}
			return string.Empty;
		}
		protected override bool IsValueValid(IComparable value, out string errorMessageTemplate) {
			errorMessageTemplate = "";
			bool result = false;
			if(value != null) {
				CriteriaOperator rightOperand;
				if(Properties.RightOperandExpression != null) {
					rightOperand = CriteriaOperator.Parse(Properties.RightOperandExpression);
				}
				else {
					object rightOperandConverted = CriteriaWrapper.TryGetReadOnlyParameterValue(Properties.RightOperand);
					if(rightOperandConverted is IConvertible) {
						rightOperandConverted = ReflectionHelper.Convert(rightOperandConverted, value.GetType());
					}
					rightOperand = new OperandValue(rightOperandConverted);
				}
				OperandValue targetOperandValue = new OperandValue(value);
				BinaryOperatorType operatorType;
				switch(Properties.OperatorType) {
					case ValueComparisonType.Equals:
						operatorType = BinaryOperatorType.Equal;
						break;
					case ValueComparisonType.GreaterThan:
						operatorType = BinaryOperatorType.Greater;
						break;
					case ValueComparisonType.GreaterThanOrEqual:
						operatorType = BinaryOperatorType.GreaterOrEqual;
						break;
					case ValueComparisonType.LessThan:
						operatorType = BinaryOperatorType.Less;
						break;
					case ValueComparisonType.LessThanOrEqual:
						operatorType = BinaryOperatorType.LessOrEqual;
						break;
					case ValueComparisonType.NotEquals:
						operatorType = BinaryOperatorType.NotEqual;
						break;
					default:
						throw new ArgumentOutOfRangeException("OperatorType");
				}
				CriteriaOperator criteriaOperator = new BinaryOperator(targetOperandValue, rightOperand, operatorType);
				result = FitToCriteria(criteriaOperator, TargetObject); 
			}
			errorMessageTemplate = GetMessageTemplate();
			return result; 
		}
		public RuleValueComparison(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
		public RuleValueComparison(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
		public RuleValueComparison() { }
		public RuleValueComparison(IRuleValueComparisonProperties properties) : base(properties) { }
		public new IRuleValueComparisonProperties Properties {
			get { return (IRuleValueComparisonProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleValueComparisonProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleValueComparisonAttribute : RuleBaseAttribute, IRuleValueComparisonProperties {
		public RuleValueComparisonAttribute(ValueComparisonType operatorType, object rightOperand) 
			: this(null, DefaultContexts.Save, operatorType, rightOperand) { }
		public RuleValueComparisonAttribute(string id, string targetContextIDs, ValueComparisonType operatorType, object rightOperand)
			: this(id, targetContextIDs, operatorType, rightOperand, string.Empty) {
		}
		public RuleValueComparisonAttribute(DefaultContexts targetContexts, ValueComparisonType operatorType, object rightOperand)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), operatorType, rightOperand) { }
		public RuleValueComparisonAttribute(string id, DefaultContexts targetContexts, ValueComparisonType operatorType, object rightOperand)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, rightOperand) { }
		public RuleValueComparisonAttribute(string id, string targetContextIDs, ValueComparisonType operatorType, object rightOperand, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary[RuleValueComparison.PropertiesOperatorType] = operatorType;
			propertiesDictionary[RuleValueComparison.PropertiesRightOperand] = rightOperand;
		}
		public RuleValueComparisonAttribute(string id, DefaultContexts targetContexts, ValueComparisonType operatorType, object rightOperand, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, rightOperand, messageTemplate) {
		}
		#region Expressions
		public RuleValueComparisonAttribute(string id, string targetContextIDs, ValueComparisonType operatorType, string rightOperand, ParametersMode mode)
			: this(id, targetContextIDs, operatorType, rightOperand, string.Empty, mode) {
		}
		public RuleValueComparisonAttribute(string id, DefaultContexts targetContexts, ValueComparisonType operatorType, string rightOperand, ParametersMode mode)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, rightOperand, mode) { }
		public RuleValueComparisonAttribute(string id, string targetContextIDs, ValueComparisonType operatorType, string rightOperand, string messageTemplate, ParametersMode mode)
			: base(id, targetContextIDs, messageTemplate) {
			if(mode != ParametersMode.Expression) {
				propertiesDictionary[RuleValueComparison.PropertiesRightOperand] = rightOperand;
			}
			else {
				propertiesDictionary[RuleValueComparison.PropertiesRightOperandExpression] = rightOperand;
			}
			propertiesDictionary[RuleValueComparison.PropertiesOperatorType] = operatorType;
		}
		public RuleValueComparisonAttribute(string id, DefaultContexts targetContexts, ValueComparisonType operatorType, string rightOperand, string messageTemplate, ParametersMode mode)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, rightOperand, messageTemplate, mode) {
		}
		#endregion
		protected new IRuleValueComparisonProperties Properties {
			get { return (IRuleValueComparisonProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleValueComparison); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleValueComparisonProperties); }
		}
		object IRuleValueComparisonProperties.RightOperand {
			get {
				return Properties.RightOperand;
			}
			set {
				Properties.RightOperand = value;
			}
		}
		string IRuleValueComparisonProperties.RightOperandExpression {
			get {
				return Properties.RightOperandExpression;
			}
			set {
				Properties.RightOperandExpression = value;
			}
		}
		ValueComparisonType IRuleValueComparisonProperties.OperatorType {
			get {
				return Properties.OperatorType;
			}
			set {
				Properties.OperatorType = value;
			}
		}
		public string TargetPropertyName {
			get { return Properties.TargetPropertyName; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		public Aggregate TargetCollectionAggregate {
			get {
				if(!Properties.TargetCollectionAggregate.HasValue) {
					return Aggregate.Exists;
				}
				return Properties.TargetCollectionAggregate.Value;
			}
			set {
				if(properties == null) {
					propertiesDictionary["TargetCollectionAggregate"] = value;
				}
				else {
					Properties.TargetCollectionAggregate = value;
				}
			}
		}
		public bool IsCollectionAggregateSet {
			get {
				return Properties.TargetCollectionAggregate.HasValue;
			}
		}
		Aggregate? IRuleSupportsCollectionAggregatesProperties.TargetCollectionAggregate {
			get { return Properties.TargetCollectionAggregate; }
			set { Properties.TargetCollectionAggregate = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustBeEqualToOperand {
			get { return Properties.MessageTemplateMustBeEqualToOperand; }
			set { Properties.MessageTemplateMustBeEqualToOperand = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustBeGreaterThanOperand {
			get { return Properties.MessageTemplateMustBeGreaterThanOperand; }
			set { Properties.MessageTemplateMustBeGreaterThanOperand = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustBeGreaterThanOrEqualToOperand {
			get { return Properties.MessageTemplateMustBeGreaterThanOrEqualToOperand; }
			set { Properties.MessageTemplateMustBeGreaterThanOrEqualToOperand = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustBeLessThanOperand {
			get { return Properties.MessageTemplateMustBeLessThanOperand; }
			set { Properties.MessageTemplateMustBeLessThanOperand = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustBeLessThanOrEqualToOperand {
			get { return Properties.MessageTemplateMustBeLessThanOrEqualToOperand; }
			set { Properties.MessageTemplateMustBeLessThanOrEqualToOperand = value; }
		}
		string IRuleValueComparisonProperties.MessageTemplateMustNotBeEqualToOperand {
			get { return Properties.MessageTemplateMustNotBeEqualToOperand; }
			set { Properties.MessageTemplateMustNotBeEqualToOperand = value; }
		}
	}
	public class RuleRange : RulePropertyValue<IComparable> {
		public const string PropertiesMinimumValue = "MinimumValue";
		public const string PropertiesMaximumValue = "MaximumValue";
		public const string PropertiesMinimumValueExpression = "MinimumValueExpression";
		public const string PropertiesMaximumValueExpression = "MaximumValueExpression";
		public const string PropertiesMessageTemplateMustBeInRange = "MessageTemplateMustBeInRange"; 
		public static string DefaultMessageTemplateMustBeInRange {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleRange_defaultMessageTemplateMustBeInRange");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeInRange;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleRange_defaultMessageTemplateMustBeInRange").Value = value; }
		}
		protected override bool IsValueValid(IComparable value, out string errorMessageTemplate) {
			Type conversionType = (TargetMember != null) ? TargetMember.MemberType : value.GetType();
			CriteriaOperator minimumValue;
			CriteriaOperator maximumValue;
			if(Properties.MinimumValueExpression != null) {
				minimumValue = CriteriaOperator.Parse(Properties.MinimumValueExpression);
			}
			else {
				object minValue = ReflectionHelper.Convert(CriteriaWrapper.TryGetReadOnlyParameterValue(Properties.MinimumValue), conversionType);
				minimumValue = new OperandValue(minValue);
			}
			if(Properties.MaximumValueExpression != null) {
				maximumValue = CriteriaOperator.Parse(Properties.MaximumValueExpression);
			}
			else {
				object maxValue = ReflectionHelper.Convert(CriteriaWrapper.TryGetReadOnlyParameterValue(Properties.MaximumValue), conversionType);
				maximumValue = new OperandValue(maxValue);
			}
			OperandValue targetOperandValue= new OperandValue(value);
			CriteriaOperator criteriaOperator = new GroupOperator(
				new BinaryOperator(targetOperandValue, minimumValue, BinaryOperatorType.GreaterOrEqual),
				new BinaryOperator(targetOperandValue, maximumValue, BinaryOperatorType.LessOrEqual)
				);
			bool result = FitToCriteria(criteriaOperator, TargetObject);
			errorMessageTemplate = Properties.MessageTemplateMustBeInRange;
			return result;
		}
		public RuleRange(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
		public RuleRange(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
		public RuleRange(IRuleRangeProperties properties) : base(properties) { }
		public RuleRange() { }
		public new IRuleRangeProperties Properties {
			get { return (IRuleRangeProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleRangeProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleRangeAttribute : RuleBaseAttribute, IRuleRangeProperties {
		public RuleRangeAttribute(object minimumValue, object maximumValue) 
			: this(null, DefaultContexts.Save, minimumValue, maximumValue) { }
		public RuleRangeAttribute(string id, string targetContextIDs, object minimumValue, object maximumValue)
			: this(id, targetContextIDs, minimumValue, maximumValue, string.Empty) { }
		public RuleRangeAttribute(DefaultContexts targetContexts, object minimumValue, object maximumValue)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), minimumValue, maximumValue) { }
		public RuleRangeAttribute(string id, DefaultContexts targetContexts, object minimumValue, object maximumValue)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), minimumValue, maximumValue) { }
		public RuleRangeAttribute(string id, string targetContextIDs, object minimumValue, object maximumValue, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary[RuleRange.PropertiesMinimumValue] = minimumValue;
			propertiesDictionary[RuleRange.PropertiesMaximumValue] = maximumValue;
		}
		public RuleRangeAttribute(string id, DefaultContexts targetContexts, object minimumValue, object maximumValue, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), minimumValue, maximumValue, messageTemplate) { }
		#region Expressions ctors
		public RuleRangeAttribute(string id, string targetContextIDs, string minimumValue, string maximumValue, ParametersMode mode)
			: this(id, targetContextIDs, minimumValue, maximumValue, string.Empty, mode) { }
		public RuleRangeAttribute(string id, DefaultContexts targetContexts, string minimumValue, string maximumValue, ParametersMode mode)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), minimumValue, maximumValue, mode) { }
		public RuleRangeAttribute(string id, string targetContextIDs, string minimumValue, string maximumValue, string messageTemplate, ParametersMode mode)
			: base(id, targetContextIDs, messageTemplate) {
			if(mode != ParametersMode.Expression) {
				propertiesDictionary[RuleRange.PropertiesMinimumValue] = minimumValue;
				propertiesDictionary[RuleRange.PropertiesMaximumValue] = maximumValue;
			}
			else {
				propertiesDictionary[RuleRange.PropertiesMinimumValueExpression] = minimumValue;
				propertiesDictionary[RuleRange.PropertiesMaximumValueExpression] = maximumValue;
			}
		}
		public RuleRangeAttribute(string id, DefaultContexts targetContexts, string minimumValue, string maximumValue, string messageTemplate, ParametersMode mode)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), minimumValue, maximumValue, messageTemplate, mode) { }
		#endregion
		protected new IRuleRangeProperties Properties {
			get { return (IRuleRangeProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleRange); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleRangeProperties); }
		}
		object IRuleRangeProperties.MinimumValue {
			get { return Properties.MinimumValue; }
			set { Properties.MinimumValue = value; }
		}
		object IRuleRangeProperties.MaximumValue {
			get { return Properties.MaximumValue; }
			set { Properties.MaximumValue = value; }
		}
		string IRuleRangeProperties.MinimumValueExpression {
			get { return Properties.MinimumValueExpression; }
			set { Properties.MinimumValueExpression = value; }
		}
		string IRuleRangeProperties.MaximumValueExpression {
			get { return Properties.MaximumValueExpression; }
			set { Properties.MaximumValueExpression = value; }
		}
		public string TargetPropertyName {
			get { return Properties.TargetPropertyName; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		public Aggregate TargetCollectionAggregate {
			get {
				if(!Properties.TargetCollectionAggregate.HasValue) {
					return Aggregate.Exists;
				}
				return Properties.TargetCollectionAggregate.Value;
			}
			set {
				if(properties == null) {
					propertiesDictionary["TargetCollectionAggregate"] = value;
				}
				else {
					Properties.TargetCollectionAggregate = value;
				}
			}
		}
		public bool IsCollectionAggregateSet {
			get {
				return Properties.TargetCollectionAggregate.HasValue;
			}
		}
		Aggregate? IRuleSupportsCollectionAggregatesProperties.TargetCollectionAggregate {
			get { return Properties.TargetCollectionAggregate; }
			set { Properties.TargetCollectionAggregate = value; }
		}
		string IRuleRangeProperties.MessageTemplateMustBeInRange {
			get { return Properties.MessageTemplateMustBeInRange; }
			set { Properties.MessageTemplateMustBeInRange = value; }
		}
	}
	public enum StringComparisonType { Contains, EndsWith, Equals, NotEquals, StartsWith };
	public class RuleStringComparison : RulePropertyValue<string> {
		public const string PropertiesOperatorType = "OperatorType";
		public const string PropertiesOperandValue = "OperandValue";
		public const string PropertiesIgnoreCase = "IgnoreCase";
		public const string PropertiesMessageTemplateMustContain = "MessageTemplateMustContain";
		public const string PropertiesMessageTemplateMustBeginWith = "MessageTemplateMustBeginWith";
		public const string PropertiesMessageTemplateMustEndWith = "MessageTemplateMustEndWith";
		public const string PropertiesMessageTemplateMustBeEqual = "MessageTemplateMustBeEqual";
		public const string PropertiesMessageTemplateMustNotBeEqual = "MessageTemplateMustNotBeEqual"; 
		public static string DefaultMessageTemplateMustContain {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustContain");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustContain;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustContain").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeginWith {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustBeginWith");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeginWith;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustBeginWith").Value = value; }
		}
		public static string DefaultMessageTemplateMustEndWith {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustEndWith");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustEndWith;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustEndWith").Value = value; }
		}
		public static string DefaultMessageTemplateMustBeEqual {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustBeEqual");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeEqual;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustBeEqual").Value = value; }
		}
		public static string DefaultMessageTemplateMustNotBeEqual {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustNotBeEqual");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustNotBeEqual;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleStringComparison_defaultMessageTemplateMustNotBeEqual").Value = value; }
		}
		private string GetCased(string source) {
			if(source == null)
				return string.Empty;
			return Properties.IgnoreCase ? source.ToLower() : source;
		}
		private string GetMessageTemplate() {
			switch(Properties.OperatorType) {
				case StringComparisonType.Contains:
					return Properties.MessageTemplateMustContain;
				case StringComparisonType.EndsWith:
					return Properties.MessageTemplateMustEndWith;
				case StringComparisonType.Equals:
					return Properties.MessageTemplateMustBeEqual;
				case StringComparisonType.NotEquals:
					return Properties.MessageTemplateMustNotBeEqual;
				case StringComparisonType.StartsWith:
					return Properties.MessageTemplateMustBeginWith;
			}
			return string.Empty;
		}
		protected override bool IsValueValid(string value, out string errorMessageTemplate) {
			string casedOperand = GetCased(Properties.OperandValue);
			string casedValue = GetCased(value);
			bool result = false;
			errorMessageTemplate = "";
			switch(Properties.OperatorType) {
				case StringComparisonType.Contains:
					result = casedValue.Contains(casedOperand);
					break;
				case StringComparisonType.EndsWith:
					result = casedValue.EndsWith(casedOperand);
					break;
				case StringComparisonType.Equals:
					result = casedValue.Equals(casedOperand);
					break;
				case StringComparisonType.NotEquals:
					result = !casedValue.Equals(casedOperand);
					break;
				case StringComparisonType.StartsWith:
					result = casedValue.StartsWith(casedOperand);
					break;
			}
			errorMessageTemplate = GetMessageTemplate();
			return result;
		}
		public RuleStringComparison(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
		public RuleStringComparison(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
		public RuleStringComparison() { }
		public RuleStringComparison(IRuleStringComparisonProperties properties) : base(properties) { }
		public new IRuleStringComparisonProperties Properties {
			get { return (IRuleStringComparisonProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleStringComparisonProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleStringComparisonAttribute : RuleBaseAttribute, IRuleStringComparisonProperties {
		public RuleStringComparisonAttribute(StringComparisonType operatorType, string operandValue) 
			: this(null, DefaultContexts.Save, operatorType, operandValue) { }
		public RuleStringComparisonAttribute(string id, string targetContextIDs, StringComparisonType operatorType, string operandValue)
			: this(id, targetContextIDs, operatorType, operandValue, string.Empty) {
		}
		public RuleStringComparisonAttribute(DefaultContexts targetContexts, StringComparisonType operatorType, string operandValue)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), operatorType, operandValue) { }
		public RuleStringComparisonAttribute(string id, DefaultContexts targetContexts, StringComparisonType operatorType, string operandValue)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, operandValue) { }
		public RuleStringComparisonAttribute(string id, string targetContextIDs, StringComparisonType operatorType, string operandValue, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["OperatorType"] = operatorType;
			propertiesDictionary["OperandValue"] = operandValue;
		}
		public RuleStringComparisonAttribute(string id, DefaultContexts targetContexts, StringComparisonType operatorType, string operandValue, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), operatorType, operandValue, messageTemplate) { }
		protected new IRuleStringComparisonProperties Properties {
			get { return (IRuleStringComparisonProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleStringComparison); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleStringComparisonProperties); }
		}
		public bool IgnoreCase {
			get { return Properties.IgnoreCase; }
			set {
				if(properties == null) {
					propertiesDictionary["IgnoreCase"] = value;
				}
				else {
					Properties.IgnoreCase = value;
				}
			}
		}
		StringComparisonType IRuleStringComparisonProperties.OperatorType {
			get { return Properties.OperatorType; }
			set { Properties.OperatorType = value; }
		}
		string IRuleStringComparisonProperties.OperandValue {
			get { return Properties.OperandValue; }
			set { Properties.OperandValue = value; }
		}
		public string TargetPropertyName {
			get { return Properties.TargetPropertyName; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		string IRuleStringComparisonProperties.MessageTemplateMustContain {
			get { return Properties.MessageTemplateMustContain; }
			set { Properties.MessageTemplateMustContain = value; }
		}
		string IRuleStringComparisonProperties.MessageTemplateMustBeginWith {
			get { return Properties.MessageTemplateMustBeginWith; }
			set { Properties.MessageTemplateMustBeginWith = value; }
		}
		string IRuleStringComparisonProperties.MessageTemplateMustEndWith {
			get { return Properties.MessageTemplateMustEndWith; }
			set { Properties.MessageTemplateMustEndWith = value; }
		}
		string IRuleStringComparisonProperties.MessageTemplateMustBeEqual {
			get { return Properties.MessageTemplateMustBeEqual; }
			set { Properties.MessageTemplateMustBeEqual = value; }
		}
		string IRuleStringComparisonProperties.MessageTemplateMustNotBeEqual {
			get { return Properties.MessageTemplateMustNotBeEqual; }
			set { Properties.MessageTemplateMustNotBeEqual = value; }
		}
	}
	public class RuleRegularExpression : RulePropertyValue<string> {
		public const string PropertiesPattern = "Pattern";
		public const string PropertiesMessageTemplateMustMatchPattern = "MessageTemplateMustMatchPattern"; 
		public static string DefaultMessageTemplateMustMatchPattern {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleRegularExpression_defaultMessageTemplateMustMatchPattern");				
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustMatchPattern;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleRegularExpression_defaultMessageTemplateMustMatchPattern").Value = value; }
		}
		protected override bool IsValueValid(string value, out string errorMessageTemplate) {
			if(value == null) {
				value = string.Empty;
			}
			bool result = new Regex(Properties.Pattern).IsMatch(value);
			errorMessageTemplate = Properties.MessageTemplateMustMatchPattern;
			return result;
		}
		public RuleRegularExpression(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
		public RuleRegularExpression(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
		public RuleRegularExpression() { }
		public RuleRegularExpression(IRuleRegularExpressionProperties properties) : base(properties) { }
		public new IRuleRegularExpressionProperties Properties {
			get { return (IRuleRegularExpressionProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleRegularExpressionProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleRegularExpressionAttribute : RuleBaseAttribute, IRuleRegularExpressionProperties {
		public RuleRegularExpressionAttribute(string pattern)
			: this(null, DefaultContexts.Save, pattern) { }
		public RuleRegularExpressionAttribute(string id, string targetContextIDs, string pattern)
			: this(id, targetContextIDs, pattern, string.Empty) {	}
		public RuleRegularExpressionAttribute(DefaultContexts targetContexts, string pattern)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), pattern) { }
		public RuleRegularExpressionAttribute(string id, DefaultContexts targetContexts, string pattern)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), pattern) { }
		public RuleRegularExpressionAttribute(string id, string targetContextIDs, string pattern, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["Pattern"] = pattern;
		}
		public RuleRegularExpressionAttribute(string id, DefaultContexts targetContexts, string pattern, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), pattern, messageTemplate) { }
		protected new IRuleRegularExpressionProperties Properties {
			get { return (IRuleRegularExpressionProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleRegularExpression); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleRegularExpressionProperties); }
		}
		string IRuleRegularExpressionProperties.Pattern {
			get { return Properties.Pattern; }
			set { Properties.Pattern = value; }
		}
		public string TargetPropertyName {
			get { return Properties.TargetPropertyName; }
			set {
				if(properties == null) {
					propertiesDictionary["TargetPropertyName"] = value;
				}
				else {
					Properties.TargetPropertyName = value;
				}
			}
		}
		string IRuleRegularExpressionProperties.MessageTemplateMustMatchPattern {
			get { return Properties.MessageTemplateMustMatchPattern; }
			set { Properties.MessageTemplateMustMatchPattern = value; }
		}
	}
	public class RuleCriteria : RuleBase {
		public const string PropertiesCriteria = "Criteria";
		public const string PropertiesMessageTemplateMustSatisfyCriteria = "MessageTemplateMustSatisfyCriteria"; 
		public static string DefaultMessageTemplateMustSatisfyCriteria {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleCriteria_defaultMessageTemplateMustSatisfyCriteria");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustSatisfyCriteria;
				}
				return manager.Value; 
			}
			set { ValueManager.GetValueManager<string>("RuleCriteria_defaultMessageTemplateMustSatisfyCriteria").Value = value; }
		}
		private class PropertiesVisitor : IClientCriteriaVisitor<IEnumerable<string>> {
			public IEnumerable<string> Visit(OperandProperty theOperand) {
				if(theOperand.PropertyName.Contains("^")) {
					return new string[0];
				}
				return new string[] { theOperand.PropertyName };
			}
			public IEnumerable<string> Visit(AggregateOperand theOperand) {
				if(AggregateOperand.ReferenceEquals(theOperand.CollectionProperty, null))
					return new string[0];
				else {
					return new string[] { theOperand.CollectionProperty.PropertyName };
				}
			}
			public IEnumerable<string> Visit(FunctionOperator theOperator) {
				List<string> result = new List<string>();
				foreach(CriteriaOperator operand in theOperator.Operands) {
					result.AddRange(operand.Accept(this));
				}
				return result;
			}
			public IEnumerable<string> Visit(OperandValue theOperand) {
				return new string[0];
			}
			public IEnumerable<string> Visit(GroupOperator theOperator) {
				List<string> result = new List<string>();
				foreach(CriteriaOperator operand in theOperator.Operands) {
					result.AddRange(operand.Accept(this));
				}
				return result;
			}
			public IEnumerable<string> Visit(InOperator theOperator) {
				List<string> result = new List<string>();
				if(!CriteriaOperator.ReferenceEquals(theOperator.LeftOperand, null)) {
					result.AddRange(theOperator.LeftOperand.Accept(this));
				}
				foreach(CriteriaOperator operand in theOperator.Operands) {
					result.AddRange(operand.Accept(this));
				}
				return result;
			}
			public IEnumerable<string> Visit(UnaryOperator theOperator) {
				if(CriteriaOperator.ReferenceEquals(theOperator.Operand, null)) {
					return new string[0];
				}
				else {
					return theOperator.Operand.Accept(this);
				}
			}
			public IEnumerable<string> Visit(BinaryOperator theOperator) {
				List<string> result = new List<string>();
				if(!CriteriaOperator.ReferenceEquals(theOperator.LeftOperand, null)) {
					result.AddRange(theOperator.LeftOperand.Accept(this));
				}
				if(!CriteriaOperator.ReferenceEquals(theOperator.RightOperand, null)) {
					result.AddRange(theOperator.RightOperand.Accept(this));
				}
				return result;
			}
			public IEnumerable<string> Visit(BetweenOperator theOperator) {
				List<string> result = new List<string>();
				if(!CriteriaOperator.ReferenceEquals(theOperator.BeginExpression, null)) {
					result.AddRange(theOperator.BeginExpression.Accept(this));
				}
				if(!CriteriaOperator.ReferenceEquals(theOperator.EndExpression, null)) {
					result.AddRange(theOperator.EndExpression.Accept(this));
				}
				if(!CriteriaOperator.ReferenceEquals(theOperator.TestExpression, null)) {
					result.AddRange(theOperator.TestExpression.Accept(this));
				}
				return result;
			}
			public IEnumerable<string> Visit(JoinOperand theOperand) {
				return new string[0];
			}
		}
		private CriteriaOperator criteriaOperator;
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			errorMessageTemplate = "";
			bool result = FitToCriteria(CriteriaOperator, target);
			errorMessageTemplate = Properties.MessageTemplateMustSatisfyCriteria;
			return result;
		}
		public RuleCriteria(string id, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, targetContextIDs, objectType) {
		}
		public RuleCriteria() {
		}
		public RuleCriteria(IRuleCriteriaProperties properties) : base(properties) { }
		public CriteriaOperator CriteriaOperator {
			get {
				if(CriteriaOperator.ReferenceEquals(criteriaOperator, null)) {
					CriteriaWrapper criteriaWrapper = new CriteriaWrapper(Properties.TargetType, Properties.Criteria);
					criteriaOperator = criteriaWrapper.CriteriaOperator;
				}
				return criteriaOperator;
			}
		}
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				if(!string.IsNullOrEmpty(Properties.UsedProperties)) {
					return UsedPropertiesStringHelper.ParseUsedPropertiesString(Properties.UsedProperties, Properties.TargetType);
				}
				List<string> propertiesList = new List<string>();
				foreach(string propertyName in (IList<string>)CriteriaOperator.Accept(new PropertiesVisitor())) {
					if(!propertiesList.Contains(propertyName)) {
						propertiesList.Add(propertyName);
					}
				}
				return propertiesList.AsReadOnly();
			}
		}
		public new IRuleCriteriaProperties Properties {
			get { return (IRuleCriteriaProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleCriteriaProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class RuleCriteriaAttribute : RuleBaseAttribute, IRuleCriteriaProperties {
		public RuleCriteriaAttribute(string criteria)
			: this(null, DefaultContexts.Save, criteria) { }
		public RuleCriteriaAttribute(string id, string targetContextIDs, string criteria)
			: this(id, targetContextIDs, criteria, string.Empty) { }
		public RuleCriteriaAttribute(DefaultContexts targetContexts, string criteria)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), criteria) { }
		public RuleCriteriaAttribute(string id, DefaultContexts targetContexts, string criteria)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), criteria) { }
		public RuleCriteriaAttribute(string id, string targetContextIDs, string criteria, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["Criteria"] = criteria;
		}
		public RuleCriteriaAttribute(string id, DefaultContexts targetContexts, string criteria, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), criteria, messageTemplate) { }
		protected new IRuleCriteriaProperties Properties {
			get { return (IRuleCriteriaProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleCriteria); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleCriteriaProperties); }
		}
		string IRuleCriteriaProperties.Criteria {
			get { return Properties.Criteria; }
			set { Properties.Criteria = value; }
		}
		string IRuleCriteriaProperties.MessageTemplateMustSatisfyCriteria {
			get { return Properties.MessageTemplateMustSatisfyCriteria; }
			set { Properties.MessageTemplateMustSatisfyCriteria = value; }
		}
		public string UsedProperties {
			get { return Properties.UsedProperties; }
			set {
				if(properties == null) {
					propertiesDictionary["UsedProperties"] = value;
				}
				else {
					Properties.UsedProperties = value;
				}
			}
		}
	}
}
