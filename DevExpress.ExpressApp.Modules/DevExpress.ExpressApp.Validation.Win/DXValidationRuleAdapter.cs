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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.ExpressApp.Validation.Win {
	public class XafConditionValidationRule : ConditionValidationRule {
		private bool invertResult;
		private bool skipNullOrEmptyValues;
		protected bool IsEmptyValue(object value) {
			if(value == null) {
				return true;
			}
			string stringValue = value as string;
			if(stringValue != null) {
				return stringValue.Length == 0;
			}
			if(value is DateTime) {
				return (DateTime)value == DateTime.MinValue;
			}
			return false;
		}
		public override bool Validate(System.Windows.Forms.Control control, object value) {
			if(IsEmptyValue(value) && skipNullOrEmptyValues) {
				return true;
			}
			else {
				bool ruleValid = base.Validate(control, value);
				return invertResult ? !ruleValid : ruleValid;
			}
		}
		public bool InvertResult {
			get { return invertResult; }
			set { invertResult = value; }
		}
		public bool SkipNullOrEmptyValues {
			get { return skipNullOrEmptyValues; }
			set { skipNullOrEmptyValues = value; }
		}
	}
	public class RegularExpressionValidationRule : ValidationRule {
		private string pattern;
		private bool invertResult;
		private bool skipNullOrEmptyValues;
		public override bool Validate(System.Windows.Forms.Control control, object value) {
			string valueString = value as string;
			if(string.IsNullOrEmpty(valueString)) {
				return skipNullOrEmptyValues;
			}
			else {
				bool isMatch = new Regex(pattern).IsMatch(valueString);
				return invertResult ? !isMatch : isMatch;
			}
		}
		public bool InvertResult {
			get { return invertResult; }
			set { invertResult = value; }
		}
		public bool SkipNullOrEmptyValues {
			get { return skipNullOrEmptyValues; }
			set { skipNullOrEmptyValues = value; }
		}
		public string Pattern {
			get { return pattern; }
			set { pattern = value; }
		}
	}
	public class ComplexValidationRule : ValidationRule {
		private List<ValidationRule> rules;
		public override bool Validate(System.Windows.Forms.Control control, object value) {
			bool isValid = true;
			ErrorText = string.Empty;
			ErrorType = ErrorType.None;
			foreach(ValidationRule rule in Rules) {
				bool currentRuleValid = rule.Validate(control, value);
				if(!currentRuleValid) {
					if(!string.IsNullOrEmpty(ErrorText)) {
						ErrorText += "\r\n";
					}
					ErrorText += rule.ErrorText;
					if(rule.ErrorType > ErrorType) {
						ErrorType = rule.ErrorType;
					}
					isValid = false;
				}
			}
			return isValid;
		}
		public List<ValidationRule> Rules {
			get {
				if(rules == null) {
					rules = new List<ValidationRule>();
				}
				return rules;
			}
			set { rules = value; }
		}
	}
	public abstract class DXValidationRuleAdapterBase {
		protected virtual XafConditionValidationRule CreateValidationRule(IRule xafValidationRule, object targetObject) {
			XafConditionValidationRule rule = new XafConditionValidationRule();
			rule.ErrorType = GetDXErrorType(xafValidationRule.Properties.ResultType);
			rule.InvertResult = xafValidationRule.Properties.InvertResult;
			rule.SkipNullOrEmptyValues = xafValidationRule.Properties.SkipNullOrEmptyValues;
			return rule;
		}
		public abstract ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject);
		public static ValidationResultType GetXafValidationResultByErrorType(ErrorType errorType) {
			switch(errorType) {
				case ErrorType.Critical: return ValidationResultType.Error;
				case ErrorType.Warning: return ValidationResultType.Warning;
				case ErrorType.Information: return ValidationResultType.Information;
			}
			return ValidationResultType.Error;
		}
		public static ErrorType GetDXErrorType(ValidationResultType xafValidationResultType) {
			switch(xafValidationResultType) {
				case ValidationResultType.Error: return ErrorType.Critical;
				case ValidationResultType.Information: return ErrorType.Information;
				case ValidationResultType.Warning: return ErrorType.Warning;
				default: return ErrorType.Critical;
			}
		}
	}
	public class DXValidationRuleRequiredAdapter : DXValidationRuleAdapterBase {		
		public override ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject) {
			RuleRequiredField ruleRequiredField = (RuleRequiredField)xafValidationRule;
			XafConditionValidationRule rule = CreateValidationRule(xafValidationRule, targetObject);
			rule.ErrorText = RuleBase.GetValidationResultMessage(ruleRequiredField.Properties.MessageTemplateMustNotBeEmpty, ruleRequiredField, targetObject);
			rule.ConditionOperator = ConditionOperator.IsNotBlank;
			return rule;
		}
	}
	public class DXValidationRuleRangeAdapter : DXValidationRuleAdapterBase {		
		public override ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject) {
			RuleRange ruleRange = (RuleRange)xafValidationRule;
			if(ruleRange.Properties.MinimumValueExpression != null || ruleRange.Properties.MaximumValueExpression != null || ruleRange.Properties.TargetCollectionOwnerType != null) {
				return null;
			}
			else {
				XafConditionValidationRule rule = CreateValidationRule(xafValidationRule, targetObject);
				rule.ErrorText = RuleBase.GetValidationResultMessage(ruleRange.Properties.MessageTemplateMustBeInRange, ruleRange, targetObject);
				rule.ConditionOperator = ConditionOperator.Between;
				rule.Value1 = ReflectionHelper.Convert(CriteriaWrapper.TryGetReadOnlyParameterValue(ruleRange.Properties.MinimumValue), ruleRange.TargetMember.MemberType);
				rule.Value2 = ReflectionHelper.Convert(CriteriaWrapper.TryGetReadOnlyParameterValue(ruleRange.Properties.MaximumValue), ruleRange.TargetMember.MemberType);
				return rule;
			}
		}
	}
	public class DXValidationRuleValueComparisonAdapter : DXValidationRuleAdapterBase {
		private string GetErrorMessageTemplate(RuleValueComparison rule) {
			switch(rule.Properties.OperatorType) {
				case ValueComparisonType.Equals: return rule.Properties.MessageTemplateMustBeEqualToOperand;
				case ValueComparisonType.GreaterThan: return rule.Properties.MessageTemplateMustBeGreaterThanOperand;
				case ValueComparisonType.GreaterThanOrEqual: return rule.Properties.MessageTemplateMustBeGreaterThanOrEqualToOperand;
				case ValueComparisonType.LessThan: return rule.Properties.MessageTemplateMustBeLessThanOperand;
				case ValueComparisonType.LessThanOrEqual: return rule.Properties.MessageTemplateMustBeLessThanOrEqualToOperand;
				case ValueComparisonType.NotEquals: return rule.Properties.MessageTemplateMustNotBeEqualToOperand;
			}
			return string.Empty;
		}
		private ConditionOperator GetConditionOperator(ValueComparisonType valueComparisonType) {
			switch(valueComparisonType) {
				case ValueComparisonType.Equals: return ConditionOperator.Equals;
				case ValueComparisonType.GreaterThan: return ConditionOperator.Greater;
				case ValueComparisonType.GreaterThanOrEqual: return ConditionOperator.GreaterOrEqual;
				case ValueComparisonType.LessThan: return ConditionOperator.Less;
				case ValueComparisonType.LessThanOrEqual: return ConditionOperator.LessOrEqual;
				case ValueComparisonType.NotEquals: return ConditionOperator.NotEquals;
			}
			return ConditionOperator.None;
		}
		public override ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject) {
			RuleValueComparison ruleValueComparison = (RuleValueComparison)xafValidationRule;
			if(ruleValueComparison.Properties.RightOperandExpression != null || ruleValueComparison.Properties.TargetCollectionOwnerType != null) {
				return null;
			}
			else {
				XafConditionValidationRule rule = CreateValidationRule(xafValidationRule, targetObject);
				rule.ErrorText = RuleBase.GetValidationResultMessage(GetErrorMessageTemplate(ruleValueComparison), ruleValueComparison, targetObject);
				rule.ConditionOperator = GetConditionOperator(ruleValueComparison.Properties.OperatorType);
				rule.Value1 = ReflectionHelper.Convert(CriteriaWrapper.TryGetReadOnlyParameterValue(ruleValueComparison.Properties.RightOperand), ruleValueComparison.TargetMember.MemberType);
				return rule;
			}		
		}
	}
	public class DXValidationRuleStringComparisonAdapter : DXValidationRuleAdapterBase {
		private string GetErrorMessageTemplate(RuleStringComparison ruleStringComparison) {
			switch(ruleStringComparison.Properties.OperatorType) {
				case StringComparisonType.Contains: return ruleStringComparison.Properties.MessageTemplateMustContain;
				case StringComparisonType.EndsWith: return ruleStringComparison.Properties.MessageTemplateMustEndWith;
				case StringComparisonType.Equals: return ruleStringComparison.Properties.MessageTemplateMustBeEqual;
				case StringComparisonType.NotEquals: return ruleStringComparison.Properties.MessageTemplateMustNotBeEqual;
				case StringComparisonType.StartsWith: return ruleStringComparison.Properties.MessageTemplateMustBeginWith;
			}
			return string.Empty;
		}
		private ConditionOperator GetConditionOperator(StringComparisonType stringComparisonType) {
			switch(stringComparisonType) {
				case StringComparisonType.Contains: return ConditionOperator.Contains;
				case StringComparisonType.EndsWith: return ConditionOperator.EndsWith;
				case StringComparisonType.Equals: return ConditionOperator.Equals;
				case StringComparisonType.NotEquals: return ConditionOperator.NotEquals;
				case StringComparisonType.StartsWith: return ConditionOperator.BeginsWith;
			}
			return ConditionOperator.None;
		}
		public override ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject) {
			RuleStringComparison ruleStringComparison = (RuleStringComparison)xafValidationRule;
			XafConditionValidationRule rule = CreateValidationRule(xafValidationRule, targetObject);
			rule.ErrorText = RuleBase.GetValidationResultMessage(GetErrorMessageTemplate(ruleStringComparison), ruleStringComparison, targetObject);
			rule.ConditionOperator = GetConditionOperator(ruleStringComparison.Properties.OperatorType);
			rule.CaseSensitive = !ruleStringComparison.Properties.IgnoreCase;
			rule.Value1 = ruleStringComparison.Properties.OperandValue;
			return rule;
		}
	}
	public class DXRuleRegularExpressionAdapter : DXValidationRuleAdapterBase {
		public override ValidationRule GetDXValidationRule(IRule xafValidationRule, object targetObject) {
			RuleRegularExpression ruleRegularExpression = (RuleRegularExpression)xafValidationRule;
			RegularExpressionValidationRule rule = new RegularExpressionValidationRule();
			rule.ErrorType = GetDXErrorType(ruleRegularExpression.Properties.ResultType);			
			rule.ErrorText = RuleBase.GetValidationResultMessage(ruleRegularExpression.Properties.MessageTemplateMustMatchPattern, ruleRegularExpression, targetObject);
			rule.InvertResult = ruleRegularExpression.Properties.InvertResult;
			rule.SkipNullOrEmptyValues = ruleRegularExpression.Properties.SkipNullOrEmptyValues;
			rule.Pattern = ruleRegularExpression.Properties.Pattern;
			return rule;
		}
	}
}
