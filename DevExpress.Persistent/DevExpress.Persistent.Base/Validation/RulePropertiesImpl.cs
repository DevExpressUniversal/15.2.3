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
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
namespace DevExpress.Persistent.Validation {
	public static class RuleDefaultMessageTemplates {
		public const string SkipNullOrEmptyValues = @"The ""{Id}"" rule was not checked because one of the target properties is empty.";
		public const string TargetDoesNotSatisfyTargetCriteria = @"The ""{Id}"" rule was not checked because the target object does not satisfy rule's target criteria.";
		public const string TargetDoesNotSatisfyCollectionCriteria = @"The ""{Id}"" rule was not checked because the target is not an element of the ""{TargetCollectionOwnerType}.{TargetCollectionPropertyName}"" collection.";
		public const string CollectionValidationMessageSuffix = @"(For the ""{TargetCollectionOwnerType}.{TargetCollectionPropertyName}"" collection elements).";
		public const string MustNotBeEmpty = @"""{TargetPropertyName}"" must not be empty.";
		public const string MustBeTrue = @"""{TargetPropertyName}"" must be set to true.";
		public const string MustBeInRange = @"""{TargetPropertyName}"" must be within the ""{MinimumValue}"" and ""{MaximumValue}"" range, boundaries are included.";
		public const string MustBeEqualToOperand = @"""{TargetPropertyName}"" must be equal to ""{RightOperand}"".";
		public const string MustBeGreaterThanOperand = @"""{TargetPropertyName}"" must be greater than ""{RightOperand}"".";
		public const string MustBeGreaterThanOrEqualToOperand = @"""{TargetPropertyName}"" must be greater than or equal to ""{RightOperand}"".";
		public const string MustBeLessThanOperand = @"""{TargetPropertyName}"" must be less than ""{RightOperand}"".";
		public const string MustBeLessThanOrEqualToOperand = @"""{TargetPropertyName}"" must be less than or equal to ""{RightOperand}"".";
		public const string MustNotBeEqualToOperand = @"""{TargetPropertyName}"" must not be equal to ""{RightOperand}"".";
		public const string MustContain = @"""{TargetPropertyName}"" must contain ""{OperandValue}"".";
		public const string MustBeginWith = @"""{TargetPropertyName}"" must begin with ""{OperandValue}"".";
		public const string MustEndWith = @"""{TargetPropertyName}"" must end with ""{OperandValue}"".";
		public const string MustBeEqual = @"""{TargetPropertyName}"" must be equal to ""{OperandValue}"".";
		public const string MustNotBeEqual = @"""{TargetPropertyName}"" must not be equal to ""{OperandValue}"".";
		public const string MustMatchPattern = @"""{TargetPropertyName}"" must match the following pattern: ""{Pattern}"".";
		public const string MustSatisfyCriteria = @"The ""{TargetObject}"" object must satisfy the following criteria: ""{Criteria}"".";
		public const string FoundObjects = @"The following objects are found: ";
		public const string MustExist = @"The objects that satisfy the ""{Criteria}"" criteria must exist.";
		public const string MustBeUnique = @"There is an object with the same value specified for ""{TargetPropertyName}"". ""{TargetPropertyName}"" values must be unique.";
		public const string MustBeReferenced = @"The ""{TargetObject}"" object must be referenced.";
		public const string CombinationOfPropertiesMustBeUnique = @"Combination of the following properties must be unique: {FormattedTargetProperties}.";
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleBaseProperties : IRuleCollectionPropertyProperties {
		private string description;
		private string id;
		private string name;
		private bool skipNullOrEmptyValues;
		private bool invertResult;
		private ContextIdentifiers contextIDs;
		private Type targetType;
		private Type targetCollectionOwnerType;
		private string targetCollectionPropertyName;
		private string messageTemplateSkipNullOrEmptyValues;
		private string messageTemplateTargetDoesNotSatisfyTargetCriteria;
		private string messageTemplateTargetDoesNotSatisfyCollectionCriteria;
		private string messageTemplateCollectionValidationMessageSuffix;
		private string targetCriteria;
		public RuleBaseProperties() { }
		public RuleBaseProperties Clone() {
			return (RuleBaseProperties)this.MemberwiseClone();
		}
		[RulePropertiesIndex(0)]
		public string Id {
			get { return id; }
			set { id = value; }
		}
		[RulePropertiesIndex(1)]
		public string Name {
			get {
				if(string.IsNullOrEmpty(name)) {
					return id;
				}
				return name;
			}
			set { name = value; }
		}
		[RulePropertiesIndex(2)]
		public string CustomMessageTemplate {
			get { return description; }
			set { description = value; }
		}
		[RulePropertiesIndex(3)]
		public Type TargetType {
			get { return targetType; }
			set {
				DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(value, "value");
				targetType = value;
			}
		}
		[RulePropertiesIndex(4)]
		public string TargetContextIDs {
			get { return contextIDs.ToString(); }
			set { contextIDs = value; }
		}
		[RulePropertiesIndex(5)]
		public bool InvertResult {
			get { return invertResult; }
			set { invertResult = value; }
		}
		[RulePropertiesIndex(6)]
		public bool SkipNullOrEmptyValues {
			get { return skipNullOrEmptyValues; }
			set { skipNullOrEmptyValues = value; }
		}
		[RulePropertiesIndex(7)]
		public string MessageTemplateSkipNullOrEmptyValues {
			get { return messageTemplateSkipNullOrEmptyValues; }
			set { messageTemplateSkipNullOrEmptyValues = value; }
		}
		[RulePropertiesIndex(8)]
		public string MessageTemplateTargetDoesNotSatisfyTargetCriteria {
			get { return messageTemplateTargetDoesNotSatisfyTargetCriteria; }
			set { messageTemplateTargetDoesNotSatisfyTargetCriteria = value; }
		}
		[RulePropertiesIndex(9)]
		public string TargetCriteria {
			get { return targetCriteria; }
			set { targetCriteria = value; }
		}
		[RulePropertiesIndex(10)]
		public Type TargetCollectionOwnerType {
			get { return targetCollectionOwnerType; }
			set { targetCollectionOwnerType = value; }
		}
		[RulePropertiesIndex(11)]
		[RulePropertiesMemberOf("TargetCollectionOwnerType")]
		public string TargetCollectionPropertyName {
			get { return targetCollectionPropertyName; }
			set {
				if(!string.IsNullOrEmpty(value)) {
					if(TargetCollectionOwnerType == null) {
						throw new InvalidOperationException("TargetCollectionOwnerType must be assigned prior to TargetCollectionPropertyName");
					}
					ITypeInfo targetCollectionOwnerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(TargetCollectionOwnerType);
					IMemberInfo collectionPropertyMemberInfo = targetCollectionOwnerTypeInfo.FindMember(value);
					if(collectionPropertyMemberInfo == null) {
						throw new MemberNotFoundException(TargetCollectionOwnerType, value);
					}
					if(!collectionPropertyMemberInfo.IsList) {
						throw new InvalidOperationException(string.Format("The {0}.{1} must be a collection property.", TargetCollectionOwnerType, value));
					}
				}
				targetCollectionPropertyName = value;
			}
		}
		[RulePropertiesIndex(13)]
		public string MessageTemplateTargetDoesNotSatisfyCollectionCriteria {
			get { return messageTemplateTargetDoesNotSatisfyCollectionCriteria; }
			set { messageTemplateTargetDoesNotSatisfyCollectionCriteria = value; }
		}
		[RulePropertiesIndex(14)]
		public string MessageTemplateCollectionValidationMessageSuffix {
			get { return messageTemplateCollectionValidationMessageSuffix; }
			set { messageTemplateCollectionValidationMessageSuffix = value; }
		}
		[RulePropertiesIndex(15)]
		public ValidationResultType ResultType { get; set; }
	}
	[DomainComponent]
	public class RulePropertyValueProperties : RuleBaseProperties, IRulePropertyValueProperties {
		private string propertyName;
		public RulePropertyValueProperties() { }
		[RulePropertiesMemberOf("TargetType")]
		[RulePropertiesIndex(0)]
		public string TargetPropertyName {
			get { return propertyName; }
			set {
				if(string.IsNullOrEmpty(value)) {
					if(!(this is IRuleSupportsCollectionAggregatesProperties)) {
						throw new InvalidOperationException(string.Format("TargetPropertyName must have a value"));
					}
				}
				else {
					if(TargetType != null) {
						ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(TargetType);
						IMemberInfo propertyInfo = targetTypeInfo.FindMember(value);
						if(propertyInfo == null) {
							throw new MemberNotFoundException(TargetType, value);
						}
					}
				}
				propertyName = value;
			}
		}
	}
	[DomainComponent]
	public class RuleRequiredFieldProperties : RulePropertyValueProperties, IRuleRequiredFieldProperties {
		private string messageTemplateMustNotBeEmpty;
		public RuleRequiredFieldProperties() { }
		public string MessageTemplateMustNotBeEmpty {
			get { return messageTemplateMustNotBeEmpty; }
			set { messageTemplateMustNotBeEmpty = value; }
		}
	}
	[DomainComponent]
	public class RuleFromBoolPropertyProperties : RulePropertyValueProperties, IRuleFromBoolPropertyProperties {
		private string messageTemplateMustBeTrue;
		private string usedProperties;
		public RuleFromBoolPropertyProperties() { }
		public string MessageTemplateMustBeTrue {
			get { return messageTemplateMustBeTrue; }
			set { messageTemplateMustBeTrue = value; }
		}
		public string UsedProperties {
			get { return usedProperties; }
			set { usedProperties = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleRangeProperties : RulePropertyValueProperties, IRuleRangeProperties {
		private string messageTemplateMustBeInRange;
		private object minimumValue;
		private object maximumValue;
		private string minimumValueExpression;
		private string maximumValueExpression;
		private Aggregate? targetCollectionAggregate;
		public RuleRangeProperties() { }
		[RulePropertiesIndex(0)]
		public object MinimumValue {
			get { return minimumValue; }
			set { minimumValue = value; }
		}
		[RulePropertiesIndex(1)]
		public object MaximumValue {
			get { return maximumValue; }
			set { maximumValue = value; }
		}
		[RulePropertiesIndex(2)]
		public string MessageTemplateMustBeInRange {
			get { return messageTemplateMustBeInRange; }
			set { messageTemplateMustBeInRange = value; }
		}
		[RulePropertiesIndex(3)]
		public Aggregate? TargetCollectionAggregate {
			get { return targetCollectionAggregate; }
			set { targetCollectionAggregate = value; }
		}
		[RulePropertiesIndex(4)]
		public string MinimumValueExpression {
			get { return minimumValueExpression; }
			set {
				minimumValueExpression = value;
				MinimumValue = value;
			}
		}
		[RulePropertiesIndex(5)]
		public string MaximumValueExpression {
			get { return maximumValueExpression; }
			set {
				maximumValueExpression = value;
				MaximumValue = value;
			}
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleValueComparisonProperties : RulePropertyValueProperties, IRuleValueComparisonProperties {
		private string messageTemplateMustBeEqualToOperand;
		private string messageTemplateMustBeGreaterThanOperand;
		private string messageTemplateMustBeGreaterThanOrEqualToOperand;
		private string messageTemplateMustBeLessThanOperand;
		private string messageTemplateMustBeLessThanOrEqualToOperand;
		private string messageTemplateMustNotBeEqualToOperand;
		private Aggregate? targetCollectionAggregate;
		private object rightOperand;
		private string rightOperandExpression;
		private ValueComparisonType operatorType;
		public RuleValueComparisonProperties() { }
		[RulePropertiesIndex(0)]
		public object RightOperand {
			get { return rightOperand; }
			set { rightOperand = value; }
		}
		[RulePropertiesIndex(1)]
		public ValueComparisonType OperatorType {
			get { return operatorType; }
			set { operatorType = value; }
		}
		[RulePropertiesIndex(2)]
		public Aggregate? TargetCollectionAggregate {
			get { return targetCollectionAggregate; }
			set { targetCollectionAggregate = value; }
		}
		[RulePropertiesIndex(3)]
		public string MessageTemplateMustBeEqualToOperand {
			get { return messageTemplateMustBeEqualToOperand; }
			set { messageTemplateMustBeEqualToOperand = value; }
		}
		[RulePropertiesIndex(4)]
		public string MessageTemplateMustBeGreaterThanOperand {
			get { return messageTemplateMustBeGreaterThanOperand; }
			set { messageTemplateMustBeGreaterThanOperand = value; }
		}
		[RulePropertiesIndex(5)]
		public string MessageTemplateMustBeGreaterThanOrEqualToOperand {
			get { return messageTemplateMustBeGreaterThanOrEqualToOperand; }
			set { messageTemplateMustBeGreaterThanOrEqualToOperand = value; }
		}
		[RulePropertiesIndex(6)]
		public string MessageTemplateMustBeLessThanOperand {
			get { return messageTemplateMustBeLessThanOperand; }
			set { messageTemplateMustBeLessThanOperand = value; }
		}
		[RulePropertiesIndex(7)]
		public string MessageTemplateMustBeLessThanOrEqualToOperand {
			get { return messageTemplateMustBeLessThanOrEqualToOperand; }
			set { messageTemplateMustBeLessThanOrEqualToOperand = value; }
		}
		[RulePropertiesIndex(8)]
		public string MessageTemplateMustNotBeEqualToOperand {
			get { return messageTemplateMustNotBeEqualToOperand; }
			set { messageTemplateMustNotBeEqualToOperand = value; }
		}
		[RulePropertiesIndex(9)]
		public string RightOperandExpression {
			get { return rightOperandExpression; }
			set {
				rightOperandExpression = value;
				RightOperand = value;
			}
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleStringComparisonProperties : RulePropertyValueProperties, IRuleStringComparisonProperties {
		private StringComparisonType operatorType;
		private string operandValue;
		private bool ignoreCase;
		private string messageTemplateMustContain;
		private string messageTemplateMustBeginWith;
		private string messageTemplateMustEndWith;
		private string messageTemplateMustBeEqual;
		private string messageTemplateMustNotBeEqual;
		public RuleStringComparisonProperties() { }
		[RulePropertiesIndex(0)]
		public StringComparisonType OperatorType {
			get { return operatorType; }
			set { operatorType = value; }
		}
		[RulePropertiesIndex(1)]
		public string OperandValue {
			get { return operandValue; }
			set { operandValue = value; }
		}
		[RulePropertiesIndex(2)]
		public bool IgnoreCase {
			get { return ignoreCase; }
			set { ignoreCase = value; }
		}
		[RulePropertiesIndex(3)]
		public string MessageTemplateMustContain {
			get { return messageTemplateMustContain; }
			set { messageTemplateMustContain = value; }
		}
		[RulePropertiesIndex(4)]
		public string MessageTemplateMustBeginWith {
			get { return messageTemplateMustBeginWith; }
			set { messageTemplateMustBeginWith = value; }
		}
		[RulePropertiesIndex(5)]
		public string MessageTemplateMustEndWith {
			get { return messageTemplateMustEndWith; }
			set { messageTemplateMustEndWith = value; }
		}
		[RulePropertiesIndex(6)]
		public string MessageTemplateMustBeEqual {
			get { return messageTemplateMustBeEqual; }
			set { messageTemplateMustBeEqual = value; }
		}
		[RulePropertiesIndex(7)]
		public string MessageTemplateMustNotBeEqual {
			get { return messageTemplateMustNotBeEqual; }
			set { messageTemplateMustNotBeEqual = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleRegularExpressionProperties : RulePropertyValueProperties, IRuleRegularExpressionProperties {
		private string pattern;
		private string messageTemplateMustMatchPattern;
		public RuleRegularExpressionProperties() { }
		[RulePropertiesIndex(0)]
		public string Pattern {
			get { return pattern; }
			set { pattern = value; }
		}
		[RulePropertiesIndex(1)]
		public string MessageTemplateMustMatchPattern {
			get { return messageTemplateMustMatchPattern; }
			set { messageTemplateMustMatchPattern = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleCriteriaProperties : RuleBaseProperties, IRuleCriteriaProperties {
		private string criteria;
		private string messageTemplateMustSatisfyCriteria;
		private string usedProperties;
		public RuleCriteriaProperties() { }
		[RulePropertiesIndex(0)]
		public string Criteria {
			get { return criteria; }
			set { criteria = value; }
		}
		[RulePropertiesIndex(1)]
		public string MessageTemplateMustSatisfyCriteria {
			get { return messageTemplateMustSatisfyCriteria; }
			set { messageTemplateMustSatisfyCriteria = value; }
		}
		[RulePropertiesIndex(2)]
		public string UsedProperties {
			get { return usedProperties; }
			set { usedProperties = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleSearchObjectProperties : RuleBaseProperties, IRuleSearchObjectProperties {
		private bool includeCurrentObject;
		private string foundObjectMessagesSeparator;
		private string foundObjectMessageFormat;
		private CriteriaEvaluationBehavior criteriaEvaluationBehavior;
		private string messageTemplateFoundObjects;
		public RuleSearchObjectProperties() { }
		[RulePropertiesIndex(0)]
		public bool IncludeCurrentObject {
			get { return includeCurrentObject; }
			set { includeCurrentObject = value; }
		}
		[RulePropertiesIndex(1)]
		public string FoundObjectMessagesSeparator {
			get { return foundObjectMessagesSeparator; }
			set { foundObjectMessagesSeparator = value; }
		}
		[RulePropertiesIndex(2)]
		public string FoundObjectMessageFormat {
			get { return foundObjectMessageFormat; }
			set { foundObjectMessageFormat = value; }
		}
		[RulePropertiesIndex(3)]
		public CriteriaEvaluationBehavior CriteriaEvaluationBehavior {
			get { return criteriaEvaluationBehavior; }
			set { criteriaEvaluationBehavior = value; }
		}
		[RulePropertiesIndex(4)]
		public string MessageTemplateFoundObjects {
			get { return messageTemplateFoundObjects; }
			set { messageTemplateFoundObjects = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleObjectExistsProperties : RuleSearchObjectProperties, IRuleObjectExistsProperties {
		private string criteria;
		private Type looksFor;
		private string messageTemplateMustExist;
		public RuleObjectExistsProperties() { }
		[RulePropertiesIndex(0)]
		public string Criteria {
			get { return criteria; }
			set { criteria = value; }
		}
		[RulePropertiesIndex(1)]
		public Type LooksFor {
			get {
				if(looksFor == null) {
					return TargetType;
				}
				return looksFor;
			}
			set { looksFor = value; }
		}
		[RulePropertiesIndex(2)]
		public string MessageTemplateMustExist {
			get { return messageTemplateMustExist; }
			set { messageTemplateMustExist = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleUniqueValueProperties : RuleSearchObjectProperties, IRuleUniqueValueProperties {
		private string propertyName;
		private string messageTemplateMustBeUnique;
		public RuleUniqueValueProperties() { }
		[RulePropertiesIndex(0)]
		public string TargetPropertyName {
			get { return propertyName; }
			set { propertyName = value; }
		}
		[RulePropertiesIndex(1)]
		public string MessageTemplateMustBeUnique {
			get { return messageTemplateMustBeUnique; }
			set { messageTemplateMustBeUnique = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleIsReferencedProperties : RuleSearchObjectProperties, IRuleIsReferencedProperties {
		private string referencePropertyName;
		private Type looksFor;
		private string messageTemplateMustBeReferenced;
		public RuleIsReferencedProperties() { }
		[RulePropertiesIndex(0)]
		public string ReferencePropertyName {
			get { return referencePropertyName; }
			set { referencePropertyName = value; }
		}
		[RulePropertiesIndex(1)]
		public Type LooksFor {
			get {
				if(looksFor == null) {
					return TargetType;
				}
				return looksFor;
			}
			set { looksFor = value; }
		}
		[RulePropertiesIndex(2)]
		public string MessageTemplateMustBeReferenced {
			get { return messageTemplateMustBeReferenced; }
			set { messageTemplateMustBeReferenced = value; }
		}
	}
	[DomainComponent, RulePropertiesIndexed]
	public class RuleCombinationOfPropertiesIsUniqueProperties : RuleSearchObjectProperties, IRuleCombinationOfPropertiesIsUniqueProperties {
		private string targetProperties;
		private string messageTemplateCombinationOfPropertiesMustBeUnique;
		[RulePropertiesIndex(0)]
		public string TargetProperties {
			get { return targetProperties; }
			set { targetProperties = value; }
		}
		[RulePropertiesIndex(1)]
		public string MessageTemplateCombinationOfPropertiesMustBeUnique {
			get { return messageTemplateCombinationOfPropertiesMustBeUnique; }
			set { messageTemplateCombinationOfPropertiesMustBeUnique = value; }
		}
	}
}
