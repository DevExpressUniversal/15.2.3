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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	public class RulePropertiesAdapter : IRuleNamedProperties {
		IRuleBaseProperties properties;
		public RulePropertiesAdapter(IRuleBaseProperties properties) {
			this.properties = properties;
		}
		public object GetPropertyValue(string propertyName, Type valueType, Type objectType) {
			return ReflectionHelper.GetMemberValue(properties, propertyName, objectType);
		}
		public void SetPropertyValue(string propertyName, object value) {
			if(string.IsNullOrEmpty(propertyName)) {
				throw new ArgumentNullException("propertyName");
			}
			ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(properties.GetType());
			IMemberInfo propertyInfo = targetTypeInfo.FindMember(propertyName);
			if(propertyInfo != null && propertyInfo.IsPublic) {
				bool isString = propertyInfo.MemberTypeInfo.Type == typeof(string);
				bool isEmptyValue = isString ? string.IsNullOrEmpty((string)value) : (value == null);
				if(isEmptyValue) {
					RulePropertiesRequiredAttribute requiredAttribute = propertyInfo.FindAttribute<RulePropertiesRequiredAttribute>(false);
					if(requiredAttribute != null && requiredAttribute.IsRequired) {
						throw new InvalidOperationException(string.Format("Unable to assign an empty value to the required property.\r\nRule Id: {0}, property: {1}", properties.Id, propertyInfo.Name));
					}
				}
				object convertedValue = value;
				if(value != null && !propertyInfo.MemberTypeInfo.Type.IsAssignableFrom(value.GetType())) {
					convertedValue = ReflectionHelper.Convert(value, propertyInfo.MemberTypeInfo.Type);
				}
				try {
					propertyInfo.SetValue(properties, convertedValue);
				}
				catch(TargetInvocationException e) {
					if(e.InnerException != null && e.InnerException is MemberNotFoundException) {
						throw e.InnerException;
					}
				}
			}
		}
		public string GetRealPropertyName(string propertyName) {
			return propertyName;
		}
	}
	public interface ISupportCheckRuleIntegrity {
		void CheckRuleIntegrity();
	}
	public class CustomFormatValidationMessageEventArgs : System.ComponentModel.HandledEventArgs {
		public CustomFormatValidationMessageEventArgs(string messageFormat, object validatedObject) {
			MessageFormat = messageFormat;
			Object = validatedObject;
		}
		public string MessageFormat { get; private set; }
		public object Object { get; private set; }
		public string ResultMessage { get; set; }
	}
	public abstract class RuleBase : IRule, ISupportCheckRuleIntegrity, IObjectSpaceLink {
		public const string PropertiesId = "Id";
		public const string PropertiesName = "Name";
		public const string PropertiesCustomMessageTemplate = "CustomMessageTemplate";
		public const string PropertiesTargetType = "TargetType";
		public const string PropertiesTargetContextIDs = "TargetContextIDs";
		public const string PropertiesInvertResult = "InvertResult";
		public const string PropertiesSkipNullOrEmptyValues = "SkipNullOrEmptyValues";
		public const string PropertiesMessageTemplateSkipNullOrEmptyValues = "MessageTemplateSkipNullOrEmptyValues";
		public const string PropertiesMessageTemplateTargetDoesNotSatisfyTargetCriteria = "MessageTemplateTargetDoesNotSatisfyTargetCriteria";
		public const string PropertiesMessageTemplateTargetDoesNotSatisfyCollectionCriteria = "MessageTemplateTargetDoesNotSatisfyCollectionCriteria";
		public const string PropertiesMessageTemplateCollectionValidationMessageSuffix = "MessageTemplateCollectionValidationMessageSuffix";
		public const string PropertiesTargetCriteria = "TargetCriteria"; 
		private static void ObjectFormatter_CustomGetValue(object sender, CustomGetValueEventArgs e) {
			IRule rule = e.Object as IRule;
			if(rule != null) {
				if(!string.IsNullOrEmpty(e.MemberPath) && !e.MemberPath.Contains(".")) {
					List<object> objects = new List<object>(new object[] { rule, rule.Properties });
					if(rule is RuleBase) {
						object target = ((RuleBase)rule).TargetObject;
						if(target != null) {
							if(e.MemberPath == RulePropertyValue.PropertiesTargetPropertyName && rule.Properties is IRulePropertyValueProperties) {
								string propertyName = ((IRulePropertyValueProperties)rule.Properties).TargetPropertyName;
								if(!string.IsNullOrEmpty(propertyName)) {
									ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(target.GetType());
									IMemberInfo propertyInfo = targetTypeInfo.FindMember(propertyName);
									if(propertyInfo != null) {
										e.Value = CaptionHelper.GetMemberCaption(propertyInfo);
										e.Handled = true;
										return;
									}
								}
							}
							else if(e.MemberPath == RulePropertyValue.PropertiesTargetCollectionPropertyName && rule.Properties is IRuleCollectionPropertyProperties) {
								string propertyName = ((IRuleCollectionPropertyProperties)rule.Properties).TargetCollectionPropertyName;
								if(!string.IsNullOrEmpty(propertyName)) {
									ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(((IRuleCollectionPropertyProperties)rule.Properties).TargetCollectionOwnerType);
									if(targetTypeInfo != null) {
										IMemberInfo propertyInfo = targetTypeInfo.FindMember(propertyName);
										if(propertyInfo != null) {
											e.Value = CaptionHelper.GetMemberCaption(propertyInfo);
											e.Handled = true;
											return;
										}
									}
								}
							}
							else if(e.MemberPath == RuleValueComparison.PropertiesRightOperand && rule.Properties is IRuleValueComparisonProperties) {
								string propertyName = ((IRuleValueComparisonProperties)rule.Properties).RightOperandExpression as string;
								if(!string.IsNullOrEmpty(propertyName)) {
									ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(target.GetType());
									IMemberInfo propertyInfo = targetTypeInfo.FindMember(propertyName);
									if(propertyInfo != null) {
										e.Value = CaptionHelper.GetMemberCaption(propertyInfo);
										e.Handled = true;
										return;
									}
								}
							}
							objects.Add(target);
						}
					}
					foreach(object currentObject in objects) {
						IMemberInfo propertyInfo = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType()).FindMember(e.MemberPath);
						if(propertyInfo != null) {
							e.Value = propertyInfo.GetValue(currentObject);
							e.Handled = true;
							return;
						}
					}
				}
			}
		}
		protected static readonly ReadOnlyCollection<string> EmptyStringCollection = new List<string>().AsReadOnly();
		private IRuleBaseProperties properties;
		private object currentTarget = null;
		protected IObjectSpace targetObjectSpace;
		protected RuleBase(string id, ContextIdentifiers targetContextIDs, Type targetType)
			: this() {
			if(targetType == null){
				throw new ArgumentException(string.Format("Unable to instatiate a Validation Rule. Cannot find the specified target class.\r\nRule Id: '{0}'\r\nRule Type: '{1}'", id, GetType().FullName));
			}
			this.Properties.Id = id;
			this.Properties.TargetContextIDs = targetContextIDs.ToString();
			this.Properties.TargetType = targetType;
		}
		protected abstract bool IsValidInternal(object target, out string errorMessageTemplate);
		protected virtual void CheckRuleIntegrityCore() { }
		protected IRuleBaseProperties CreateProperties() {
			return (IRuleBaseProperties)TypeHelper.CreateInstance(PropertiesType);
		}
		protected bool IsCollectionPropertyRule {
			get {
				if(!(Properties is IRuleCollectionPropertyProperties)) {
					return false;
				}
				return ((IRuleCollectionPropertyProperties)Properties).TargetCollectionOwnerType != null && !string.IsNullOrEmpty(((IRuleCollectionPropertyProperties)Properties).TargetCollectionPropertyName);
			}
		}
		protected ExpressionEvaluator CreateEvaluator(object target, CriteriaOperator criteria) {
			CriteriaWrapper aggregateCriteriaWrapper = new CriteriaWrapper(Properties.TargetType, criteria);
			aggregateCriteriaWrapper.UpdateParametersValues(target);
			if(targetObjectSpace == null) {
				throw new InvalidOperationException("Unable to create ExpressionEvaluator, targetObjectSpace is not set.");
			}
			ExpressionEvaluator evaluator = targetObjectSpace.GetExpressionEvaluator(Properties.TargetType, aggregateCriteriaWrapper.CriteriaOperator);
			return evaluator;
		}
		protected bool FitToCriteria(CriteriaOperator criteriaOperator, object target) {
			ExpressionEvaluator evaluator = CreateEvaluator(target, criteriaOperator);
			return evaluator.Fit(target);
		}
		protected void OnCustomFormatValidationMessage(CustomFormatValidationMessageEventArgs args) {
			if(CustomFormatValidationMessage != null) {
				CustomFormatValidationMessage(this, args);
			}
		}
		internal void SetTargetObject(object target) {
			currentTarget = target;
		}
		static RuleBase() {
			ObjectFormatter.CustomGetValue += new EventHandler<CustomGetValueEventArgs>(ObjectFormatter_CustomGetValue);
		}
		public RuleBase() {
			properties = CreateProperties();
			RulePropertiesHelper.AssignDefaultValues(this.properties);
		}
		public RuleBase(IRuleBaseProperties properties) {
			this.properties = properties;
			CheckRuleIntegrityCore();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetValidationResultMessage(string errorMessageTemplate, RuleBase rule, object target) {
			if(!string.IsNullOrEmpty(rule.Properties.CustomMessageTemplate)) {
				errorMessageTemplate = rule.Properties.CustomMessageTemplate;
			}
			if(rule.IsCollectionPropertyRule) {
				string messageSuffix = ((IRuleCollectionPropertyProperties)rule.Properties).MessageTemplateCollectionValidationMessageSuffix;
				if(!string.IsNullOrEmpty(messageSuffix)) {
					errorMessageTemplate += " " + messageSuffix;
				}
			}
			string format = errorMessageTemplate != null ? errorMessageTemplate.Replace(RuleSearchObject.OpeningBraceReplacement, "{").Replace(RuleSearchObject.ClosingBraceReplacement, "}") : null;
			CustomFormatValidationMessageEventArgs args = new CustomFormatValidationMessageEventArgs(format, target);
			rule.OnCustomFormatValidationMessage(args);
			bool needSetTargetObject = rule.TargetObject == null && target != null;
			if(needSetTargetObject) {
				rule.SetTargetObject(target);
			}
			string validationResultMessage = args.Handled ? args.ResultMessage : ObjectFormatter.Format(errorMessageTemplate, rule).Replace(RuleSearchObject.OpeningBraceReplacement, "{").Replace(RuleSearchObject.ClosingBraceReplacement, "}");
			if(needSetTargetObject) {
				rule.SetTargetObject(null);
			}
			return validationResultMessage;
		}
		public RuleValidationResult Validate(object target) {
			try {
				Guard.ArgumentNotNull(target, "target");
				currentTarget = target;
				lock(currentTarget) {
					string errorMessageTemplate = "";
					bool isValid = IsValidInternal(target, out errorMessageTemplate);
					if(!string.IsNullOrEmpty(Properties.CustomMessageTemplate)) {
						errorMessageTemplate = Properties.CustomMessageTemplate;
					}
					if(Properties.InvertResult) {
						isValid = !isValid;
					}
					ValidationState validationState = isValid ? ValidationState.Valid : ValidationState.Invalid;
					string validationResultMessage = GetValidationResultMessage(errorMessageTemplate, this, target);
					RuleValidationResult validationResult = new RuleValidationResult(this, target, validationState, validationResultMessage);
					return validationResult;
				}
			}
			catch(Exception e) {
				string targetObjectString = (target != null) ? "'" + ReflectionHelper.GetObjectDisplayText(target) + "'" : "'' (N/A)";
				throw new Exception(string.Format(
@"An exception occurred while validating an object:					
  Exception: '{0}'
  Rule Id: '{1}'
  Rule Type: '{2}'
  Target Object: {3}", e.Message, Id, GetType().FullName, targetObjectString), e);
			}
			finally {
				currentTarget = null;
			}
		}
		void ISupportCheckRuleIntegrity.CheckRuleIntegrity() {
			CheckRuleIntegrityCore();
		}
		public IRuleBaseProperties Properties {
			get { return properties; }
		}
		public static string DefaultMessageTemplateSkipNullOrEmptyValues {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateSkipNullOrEmptyValues");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.SkipNullOrEmptyValues;
				} return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateSkipNullOrEmptyValues").Value = value; }
		}
		public static string DefaultMessageTemplateTargetDoesNotSatisfyTargetCriteria {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateTargetDoesNotSatisfyTargetCriteria");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.TargetDoesNotSatisfyTargetCriteria;
				} 
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateTargetDoesNotSatisfyTargetCriteria").Value = value; }
		}
		public static string DefaultMessageTemplateTargetDoesNotSatisfyCollectionCriteria {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateTargetDoesNotSatisfyCollectionCriteria");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.TargetDoesNotSatisfyCollectionCriteria;
				} 
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateTargetDoesNotSatisfyCollectionCriteria").Value = value; }
		}
		public static string DefaultMessageTemplateCollectionValidationMessageSuffix {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateCollectionValidationMessageSuffix");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.CollectionValidationMessageSuffix;
				} 
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleBase_defaultMessageTemplateCollectionValidationMessageSuffix").Value = value; }
		}
		public virtual Type PropertiesType {
			get { return typeof(RuleBaseProperties); }
		}
		public virtual ReadOnlyCollection<string> UsedProperties {
			get { return EmptyStringCollection; }
		}
		public string Id {
			get { return Properties.Id; }
		}
		public object TargetObject {
			get { return currentTarget; }
		}
		public static event EventHandler<CustomFormatValidationMessageEventArgs> CustomFormatValidationMessage;
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return targetObjectSpace; }
			set { targetObjectSpace = value; }
		}
	}
	public abstract class RuleBase<T> : RuleBase {
		protected RuleBase(string id, ContextIdentifiers targetContextIDs) : this(id, targetContextIDs, typeof(T)) { }
		protected RuleBase(string id, ContextIdentifiers targetContextIDs, Type targetType)
			: base(id, targetContextIDs, targetType) {
			if(!typeof(T).IsAssignableFrom(targetType)) {
				throw new ArgumentException("targetType");
			}
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			return IsValidInternal((T)target, out errorMessageTemplate);
		}
		protected abstract bool IsValidInternal(T target, out string errorMessageTemplate);
		public RuleBase() { }
		public RuleBase(IRuleBaseProperties properties)
			: base(properties) {
		}
	}
}
