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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	[DomainComponent]
	[DisplayName("Validaton Results")]
	public class RuleSetValidationResult {
		private List<RuleSetValidationResultItem> validationResults = new List<RuleSetValidationResultItem>();
		private ValidationOutcome validationOutcome = ValidationOutcome.Skipped;
		private const string defaultErrorMessageHeaderFormat = "Problems were detected during data validation. Please read the information below to understand what the issues are and how you can correct them.";
		private const string defaultErrorMessageItemsSeparator = "\r\n - ";
		public RuleSetValidationResult() { }
		internal static RuleSetValidationResult GroupCollectionRuleAggregatedResults(RuleSetValidationResult validationResult) {
			RuleSetValidationResult result = new RuleSetValidationResult();
			Dictionary<IRule, Dictionary<object, IList<RuleSetValidationResultItem>>> aggregatedResults = new Dictionary<IRule, Dictionary<object, IList<RuleSetValidationResultItem>>>();
			foreach(RuleSetValidationResultItem item in validationResult.Results) {
				IRuleSupportsCollectionAggregatesProperties aggregateRuleProperties = item.Rule.Properties as IRuleSupportsCollectionAggregatesProperties;
				if(aggregateRuleProperties == null || !aggregateRuleProperties.TargetCollectionAggregate.HasValue) {
					result.AddResult(item);
				}
				else {
					IMemberInfo associatedMemberInfo = RuleCollectionPropertyTargetCriteriaHelper.GetAssociatedMember(aggregateRuleProperties);
					object ownerObject = associatedMemberInfo.GetValue(item.Target);
					if(ownerObject == null) {
						result.AddResult(item);
					}
					else {
						if(!aggregatedResults.ContainsKey(item.Rule)) {
							aggregatedResults.Add(item.Rule, new Dictionary<object, IList<RuleSetValidationResultItem>>());
						}
						if(!aggregatedResults[item.Rule].ContainsKey(ownerObject)) {
							aggregatedResults[item.Rule].Add(ownerObject, new List<RuleSetValidationResultItem>());
						}
						aggregatedResults[item.Rule][ownerObject].Add(item);
					}
				}
			}
			foreach(IRule keyRule in aggregatedResults.Keys) {
				foreach(object keyTarget in aggregatedResults[keyRule].Keys) {
					if(aggregatedResults[keyRule][keyTarget].Count == 1) {
						result.AddResult(aggregatedResults[keyRule][keyTarget][0]);
					}
					else {
						ContextIdentifier context = aggregatedResults[keyRule][keyTarget][0].ContextId;
						RuleSetValidationResultItemAggregate aggregatedItem = new RuleSetValidationResultItemAggregate(keyTarget, context, keyRule, aggregatedResults[keyRule][keyTarget]);
						result.AddResult(aggregatedItem);
					}
				}
			}
			return result;
		}
		public void Join(RuleSetValidationResult ruleSetResult) {
			if(ruleSetResult == null) {
				throw new ArgumentNullException("ruleResult");
			}
			AddResults(ruleSetResult.Results);
		}
		public void AddResults(IEnumerable<RuleSetValidationResultItem> ruleResults) {
			if(ruleResults == null) {
				throw new ArgumentNullException("ruleResults");
			}
			foreach(RuleSetValidationResultItem currentRuleResult in ruleResults) {
				AddResult(currentRuleResult);
			}
		}
		public void AddResult(RuleSetValidationResultItem resultItem) {
			if(resultItem == null) {
				throw new ArgumentNullException("ruleResult");
			}
			validationResults.Add(resultItem);
			if(validationOutcome < resultItem.ValidationOutcome) {
				validationOutcome = resultItem.ValidationOutcome;
			}
		}
		public IList<RuleSetValidationResultItem> GetResultsForTarget(object target) {
			List<RuleSetValidationResultItem> results = new List<RuleSetValidationResultItem>();
			foreach(RuleSetValidationResultItem validationResult in validationResults) {
				if(validationResult.Target == target) {
					results.Add(validationResult);
				}
			}
			return results.ToArray();
		}
		public IList<RuleSetValidationResultItem> GetResultsForContext(string contextId) {
			List<RuleSetValidationResultItem> results = new List<RuleSetValidationResultItem>();
			foreach(RuleSetValidationResultItem validationResult in validationResults) {
				if(validationResult.ContextId == contextId) {
					results.Add(validationResult);
				}
			}
			return results.ToArray();
		}
		public Dictionary<string, IList<RuleSetValidationResultItem>> GetResultsByContexts() {
			Dictionary<string, IList<RuleSetValidationResultItem>> dictionary = new Dictionary<string, IList<RuleSetValidationResultItem>>();
			foreach(RuleSetValidationResultItem currentResult in validationResults) {
				string currentContextId = currentResult.ContextId.Id;
				if(!dictionary.ContainsKey(currentContextId)) {
					dictionary.Add(currentContextId, GetResultsForContext(currentContextId));
				}
			}
			return dictionary;
		}
		public Dictionary<object, IList<RuleSetValidationResultItem>> GetResultsByTargets() {
			Dictionary<object, IList<RuleSetValidationResultItem>> dictionary = new Dictionary<object, IList<RuleSetValidationResultItem>>();
			foreach(RuleSetValidationResultItem currentResult in validationResults) {
				object currentTarget = currentResult.Target;
				if(!dictionary.ContainsKey(currentTarget)) {
					dictionary.Add(currentTarget, GetResultsForTarget(currentTarget));
				}
			}
			return dictionary;
		}
		public RuleSetValidationResultItem GetResultItem(string ruleId) {
			RuleSetValidationResultItem result = null;
			for(int i = 0; i < Results.Count; ++i) {
				if(Results[i].Rule.Id == ruleId) {
					result = Results[i];
					break;
				}
			}
			return result;
		}
		public string GetFormattedErrorMessage() {
			return GetFormattedErrorMessage(defaultErrorMessageHeaderFormat);
		}
		public string GetFormattedErrorMessage(string objectHeaderFormat, ValidationResultType[] resultTypes = null) {
			return GetFormattedErrorMessage(objectHeaderFormat, defaultErrorMessageItemsSeparator, resultTypes);
		}
		public string GetFormattedErrorMessage(string objectHeaderFormat, string itemsSeparator, ValidationResultType[] resultTypes = null) {
			return string.Join("\r\n", Results
				 .Where(x => x.ValidationOutcome > ValidationOutcome.Valid && (resultTypes == null || resultTypes.Contains(x.Rule.Properties.ResultType)))
				 .GroupBy(x => x.DisplayObjectName)
				 .Select(x => new {
					 ObjectDisplayName = string.Format(objectHeaderFormat, x.Key),
					 ErrorMessages = x.Select(r => r.ErrorMessage).OrderBy(s => s)
				 })
				 .OrderBy(x => x.ObjectDisplayName)
				 .Select(x => (x.ObjectDisplayName + itemsSeparator + string.Join(itemsSeparator, x.ErrorMessages)).TrimStart('\r', '\n')));
		}
		[Browsable(false)]
		public ContextIdentifiers ContextIDs {
			get {
				ContextIdentifiers contextIdentifiers = new ContextIdentifiers();
				foreach(RuleSetValidationResultItem ruleResult in validationResults) {
					contextIdentifiers += ruleResult.ContextId;
				}
				return contextIdentifiers;
			}
		}
		[ReadOnly(true)]
		public ReadOnlyCollection<RuleSetValidationResultItem> Results {
			get { return validationResults.AsReadOnly(); }
		}
		[Browsable(false)]
		public ValidationState State {
			get {
				if(ValidationOutcome == ValidationOutcome.Error) {
					return ValidationState.Invalid;
				}
				return ValidationOutcome == ValidationOutcome.Skipped ? ValidationState.Skipped : ValidationState.Valid;
			}
		}
		[Browsable(false)]
		public ValidationOutcome ValidationOutcome {
			get {
				return validationOutcome;
			}
		}
		public bool CompareResults(RuleSetValidationResult comparedResult) {
			var invalidResults = Results
				.Where(x => x.ValidationOutcome > ValidationOutcome.Valid);
			var comparedInvalidResults = comparedResult.Results
				.Where(x => x.ValidationOutcome > ValidationOutcome.Valid);
			return
				(invalidResults.Count() == comparedInvalidResults.Count()) &&
				(invalidResults
					.Select(x => x.Rule)
					.Except(comparedInvalidResults.Select(x => x.Rule))
					.Count() == 0);
		}
	}
	[DomainComponent]
	[DisplayName("Validation Details")]
	public class RuleSetValidationResultItem {
		private object target;
		private ContextIdentifier contextId;
		private IRule rule;
		private RuleValidationResult ruleResult;
		private string displayObjectName;
		internal RuleValidationResult GetRuleResult() {
			return ruleResult;
		}
		public RuleSetValidationResultItem(object target, ContextIdentifier contextId, IRule rule, RuleValidationResult validationResult) {
			this.target = target;
			this.contextId = contextId;
			this.rule = rule;
			this.ruleResult = validationResult;
		}
		public string DisplayObjectName {
			get {
				if(displayObjectName == null) {
					displayObjectName = ReflectionHelper.GetObjectDisplayText(target);
				}
				return displayObjectName;
			}
		}
		public string RuleName {
			get { return Rule.Properties.Name; }
		}
		public object Target {
			get { return target; }
		}
		public ContextIdentifier ContextId {
			get { return contextId; }
		}
		public IRule Rule {
			get { return rule; }
		}
		[Browsable(false)] 
		public ValidationState State {
			get { return ruleResult.State; }
		}
		[DisplayName("State")] 
		public ValidationOutcome ValidationOutcome {
			get { return ruleResult.ValidationOutcome; }
		}
		public string ErrorMessage {
			get { return ruleResult.ErrorMessage; }
		}
	}
	[DomainComponent]
	public class RuleSetValidationResultItemAggregate : RuleSetValidationResultItem {
		List<RuleSetValidationResultItem> innerItems;
		public RuleSetValidationResultItemAggregate(object target, ContextIdentifier contextId, IRule rule, IList<RuleSetValidationResultItem> innerItems)
			: base(target, contextId, rule, new RuleValidationResult(rule, target, innerItems[0].State, innerItems[0].ErrorMessage)) {
			this.innerItems = new List<RuleSetValidationResultItem>(innerItems);
		}
		[ReadOnly(true)]
		public ReadOnlyCollection<RuleSetValidationResultItem> AggregatedResults {
			get { return innerItems.AsReadOnly(); }
		}
	}
	public delegate void ValidationFailedDelegate(ValidationCompletedEventArgs args);
	[Serializable]
	public class ValidationException : Exception {
		[Serializable]
		private struct ValidationExceptionState : ISafeSerializationData {
			public string MessageHeader { get; set; }
			public string ObjectHeaderFormat { get; set; }
			public string Message { get; set; }
			void ISafeSerializationData.CompleteDeserialization(Object obj) {
				ValidationException exception = (ValidationException)obj;
				exception.state = this;
			}
		}
		private const string defaultMessageHeader = "Problems were detected during data validation. Please read the information below to understand what the issues are and how you can correct them.";
		private const string defaultObjectHeaderFormat = "";
		[NonSerialized]
		private ValidationExceptionState state = new ValidationExceptionState();
		private RuleSetValidationResult ruleSetResult;
		private static string GetValidationMessage(RuleSetValidationResult ruleSetResult) {
			List<string> targetsResults = new List<string>();
			targetsResults.Add(ruleSetResult.GetFormattedErrorMessage());
			return string.Join("\r\n", targetsResults.ToArray());
		}
		public static ValidationErrorMessageHandler DefaultErrorMessageHandler {
			get { return GetValidationMessage; }
		}
		public ValidationException(RuleSetValidationResult result) : this("", result) { }
		public ValidationException(string message, RuleSetValidationResult result)
			: base(message) {
			this.ruleSetResult = result;
			state.Message = message;
			state.MessageHeader = CaptionHelper.GetLocalizedText(@"Exceptions\UserVisibleExceptions\Validation", "AllContextsErrorMessageHeader", defaultMessageHeader);
			state.ObjectHeaderFormat = defaultObjectHeaderFormat;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public ValidationException(ValidationErrorMessageHandler messageHandler, RuleSetValidationResult result) : this(messageHandler(result), result) { }
		public ValidationException(RuleSetValidationResult result, string messageHeader, string objectHeaderFormat) {
			this.ruleSetResult = result;
			state.MessageHeader = messageHeader;
			state.ObjectHeaderFormat = objectHeaderFormat;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public string GetMessages(ValidationResultType resultType) {
			return Result.GetFormattedErrorMessage(ObjectHeaderFormat, new ValidationResultType[] { resultType });
		}
		public RuleSetValidationResult Result {
			get { return ruleSetResult; }
		}
		public string MessageHeader {
			get { return state.MessageHeader; }
			set { state.MessageHeader = value; }
		}
		public string ObjectHeaderFormat {
			get { return state.ObjectHeaderFormat; }
			set { state.ObjectHeaderFormat = value; }
		}
		public override string Message {
			get {
				if(string.IsNullOrEmpty(state.Message)) {
					return state.MessageHeader + "\r\n" + ruleSetResult.GetFormattedErrorMessage(state.ObjectHeaderFormat);
				}
				return state.Message;
			}
		}
	}
	public class RuleValidatingEventArgs : EventArgs {
		private IRule rule;
		object target;
		public RuleValidatingEventArgs(IRule rule, object target) {
			this.rule = rule;
			this.target = target;
		}
		public IRule Rule {
			get { return rule; }
		}
		public object Target {
			get { return target; }
		}
	}
	public class CustomNeedToValidateRuleEventArgs : HandledEventArgs {
		public CustomNeedToValidateRuleEventArgs(IRule rule, object target, string contextId, IObjectSpace objectSpace) {
			NeedToValidateRule = true;
			this.Rule = rule;
			this.Target = target;
			this.ContextId = contextId;
			this.ObjectSpace = objectSpace;
			Reason = string.Empty;
		}
		public IRule Rule { get; private set; }
		public object Target { get; private set; }
		public string ContextId { get; private set; }
		[DefaultValue(true)]
		public bool NeedToValidateRule { get; set; }
		public string Reason { get; set; }
		public IObjectSpace ObjectSpace { get; private set; }
	}
	public class CustomValidateRuleEventArgs : EventArgs {
		public CustomValidateRuleEventArgs(IRule rule, object target, IObjectSpace objectSpace) {
			this.Rule = rule;
			this.Target = target;
			this.ObjectSpace = objectSpace;
		}
		public IRule Rule { get; private set; }
		public object Target { get; private set; }
		public RuleValidationResult RuleValidationResult { get; set; }
		public IObjectSpace ObjectSpace { get; private set; }
	}
	public class RuleValidatedEventArgs : HandledEventArgs {
		public RuleValidatedEventArgs(RuleSetValidationResultItem ruleSetValidationResultItem, IObjectSpace objectSpace) {
			this.RuleSetValidationResultItem = ruleSetValidationResultItem;
			this.ObjectSpace = objectSpace;
		}
		public RuleSetValidationResultItem RuleSetValidationResultItem { get; private set; }
		public IObjectSpace ObjectSpace { get; private set; }
	}
	public class ValidationCompletedEventArgs : HandledEventArgs {
		private RuleSetValidationResult validationResult;
		public ValidationCompletedEventArgs(RuleSetValidationResult validationResult, IObjectSpace objectSpace) {
			this.validationResult = validationResult;
			this.ObjectSpace = objectSpace;
			if(validationResult.ValidationOutcome > ValidationOutcome.Information) {
				this.Exception = new ValidationException(validationResult);
			}
		}
		public ValidationCompletedEventArgs(ValidationException exception, IObjectSpace objectSpace) {
			this.Exception = exception;
			this.validationResult = exception != null ? exception.Result : null;
			this.ObjectSpace = objectSpace;
		}
		public ValidationException Exception { get; set; }
		public bool Successful {
			get { return Result.ValidationOutcome < ValidationOutcome.Error; }
		}
		public IObjectSpace ObjectSpace { get; private set; }
		public RuleSetValidationResult Result {
			get {
				if(validationResult == null) {
					return Exception != null ? Exception.Result : null;
				}
				return validationResult;
			}
		}
	}
	public class CustomIsEmptyValueEventArgs : HandledEventArgs {
		private object targetObject;
		private string propertyName;
		private object propertyValue;
		private bool isEmpty;
		public CustomIsEmptyValueEventArgs(object targetObject, string propertyName, object propertyValue) {
			this.targetObject = targetObject;
			this.propertyName = propertyName;
			this.propertyValue = propertyValue;
		}
		public object TargetObject {
			get { return targetObject; }
		}
		public string PropertyName {
			get { return propertyName; }
		}
		public object PropertyValue {
			get { return propertyValue; }
		}
		public bool IsEmpty {
			get { return isEmpty; }
			set { isEmpty = value; }
		}
	}
	public class CustomIsEmptyValueTypeEventArgs : HandledEventArgs {
		public CustomIsEmptyValueTypeEventArgs(Type type) {
			this.Type = type;
		}
		public Type Type { get; private set; }
		public bool IsEmpty { get; set; }
	}
	internal class RuleSetSnapShot {
		private ReadOnlyCollection<IRule> rules;
		public RuleSetSnapShot(ReadOnlyCollection<IRule> rules) {
			if(rules == null) {
				rules = new ReadOnlyCollection<IRule>(new IRule[0]);
			}
			this.rules = rules;
		}
		public RuleSetValidationResult ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets) {
			return ValidateAll(targetObjectSpace, targets, "", false);
		}
		public RuleSetValidationResult ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets, bool groupAggregatedRuleResults) {
			return ValidateAll(targetObjectSpace, targets, "", groupAggregatedRuleResults);
		}
		public RuleSetValidationResult ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets, ContextIdentifiers contextIDs, bool groupAggregatedRuleResults) {
			RuleSetValidationResult ruleSetValidationResult = new RuleSetValidationResult();
			foreach(object target in targets) {
				ruleSetValidationResult.Join(Validate(targetObjectSpace, target, contextIDs));
			}
			if(groupAggregatedRuleResults) {
				return RuleSetValidationResult.GroupCollectionRuleAggregatedResults(ruleSetValidationResult);
			}
			return ruleSetValidationResult;
		}
		public RuleSetValidationResult ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets, ContextIdentifiers contextIDs) {
			return ValidateAll(targetObjectSpace, targets, contextIDs, false);
		}
		private static bool IsRuleContext(IRule rule, Type objectType, string contextId) {
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentNotNull(rule, "rule");
			ContextIdentifiers ruleContexts = new ContextIdentifiers(rule.Properties.TargetContextIDs); 
			return (ruleContexts.Contains(contextId) || string.IsNullOrEmpty(contextId)) && rule.Properties.TargetType.IsAssignableFrom(objectType);
		}
		private static bool NeedToValidateRule(IObjectSpace targetObjectSpace, IRule rule, object target, string contextId, out string reason) {
			return RuleSet.NeedToValidateRule(targetObjectSpace, rule, target, contextId, out reason);
		}
		private static void ValidateContext(IObjectSpace targetObjectSpace, RuleSetValidationResult ruleSetValidationResult, IRule rule, object target, string contextId) {
			string needToSkipValidationReason = string.Empty;
			if(!NeedToValidateRule(targetObjectSpace, rule, target, contextId, out needToSkipValidationReason)) {
				if(!string.IsNullOrEmpty(needToSkipValidationReason)) {
					RuleValidationResult result = new RuleValidationResult(rule, target, ValidationState.Skipped, needToSkipValidationReason);
					RuleSetValidationResultItem ruleSetValidationResultItem = new RuleSetValidationResultItem(target, contextId, rule, result);
					ruleSetValidationResult.AddResult(ruleSetValidationResultItem);
				}
			}
			else {
				CustomValidateRuleEventArgs customValidateArgs = new CustomValidateRuleEventArgs(rule, target, targetObjectSpace);
				RuleSet.RaiseCustomValidateRule(customValidateArgs);
				if(customValidateArgs.RuleValidationResult == null) {
					customValidateArgs.RuleValidationResult = rule.Validate(target);
				}
				RuleSetValidationResultItem ruleSetValidationResultItem = new RuleSetValidationResultItem(target, contextId, rule, customValidateArgs.RuleValidationResult);
				ruleSetValidationResult.AddResult(ruleSetValidationResultItem);
				RuleValidatedEventArgs validatedArgs = new RuleValidatedEventArgs(ruleSetValidationResultItem, targetObjectSpace);
				RuleSet.RaiseRuleValidated(validatedArgs);
			}
		}
		public RuleSetValidationResult Validate(IObjectSpace targetObjectSpace, object target, ContextIdentifiers contextIDs, bool groupAggregatedRuleResults) {
			Guard.ArgumentNotNull(target, "target");
			RuleSetValidationResult ruleSetValidationResult = new RuleSetValidationResult();
			foreach(IRule rule in rules) {
				try {
					if(rule is IObjectSpaceLink) {
						((IObjectSpaceLink)rule).ObjectSpace = targetObjectSpace;
					}
					if(contextIDs == "") {
						foreach(string contextId in (ContextIdentifiers)rule.Properties.TargetContextIDs) {
							ValidateContext(targetObjectSpace, ruleSetValidationResult, rule, target, contextId);
						}
					}
					else {
						foreach(string contextId in contextIDs) {
							ValidateContext(targetObjectSpace, ruleSetValidationResult, rule, target, contextId);
						}
					}
				}
				finally {
					if(rule is IObjectSpaceLink) {
						((IObjectSpaceLink)rule).ObjectSpace = null;
					}
				}
			}
			if(groupAggregatedRuleResults) {
				return RuleSetValidationResult.GroupCollectionRuleAggregatedResults(ruleSetValidationResult);
			}
			return ruleSetValidationResult;
		}
		public RuleSetValidationResult Validate(IObjectSpace targetObjectSpace, object target, ContextIdentifiers contextIDs) {
			return Validate(targetObjectSpace, target, contextIDs, false);
		}
		public ReadOnlyCollection<IRule> GetTargetRules(Type targetType, ContextIdentifiers contextIDs) {
			Guard.ArgumentNotNull(targetType, "targetType");
			List<IRule> result = new List<IRule>();
			foreach(IRule rule in rules) {
				foreach(string contextId in contextIDs) {
					if(IsRuleContext(rule, targetType, contextId)) {
						result.Add(rule);
					}
				}
			}
			return result.AsReadOnly();
		}
		public IList<IRule> Rules {
			get { return rules; }
		}
	}
	public delegate string ValidationErrorMessageHandler(RuleSetValidationResult result);
	public class RuleSet : IEnumerable<IRule> {
		private void NullDelegate(ValidationCompletedEventArgs args) {
		}
		private static readonly AssemblyName validationCoreAssembly = new AssemblyName(typeof(RuleBase).Assembly.ToString()); 
		private List<IRule> registeredRules = new List<IRule>();
		private List<IRuleSource> registeredSources = new List<IRuleSource>();
		private Dictionary<ITypeInfo, Object> queriedTypes = new Dictionary<ITypeInfo, Object>();
		private List<Assembly> queriedAssemblies = new List<Assembly>();
		private bool isDelayedRuleRegistrationMode = false;
		private bool enableDelayedRuleRegistration = true;
		private static bool IsAcceptableTargetByRuleTargetCriteria(IObjectSpace targetObjectSpace, object target, string criteria) {
			if(string.IsNullOrEmpty(criteria)) {
				return true;
			}
			Guard.ArgumentNotNull(target, "target");
			Type targetType = target.GetType();
			CriteriaWrapper criteriaWrapper = new CriteriaWrapper(targetType, criteria);
			criteriaWrapper.UpdateParametersValues(target);
			ExpressionEvaluator evaluator = targetObjectSpace.GetExpressionEvaluator(targetType, criteriaWrapper.CriteriaOperator);
			return evaluator.Fit(target);
		}
		private static bool IsAcceptableTargetBySkipNullOrEmptyValues(object target, bool skipNullOrEmptyValues, IList<string> usedProperties, string ruleId) {
			if(!skipNullOrEmptyValues) {
				return true;
			}
			Guard.ArgumentNotNull(target, "target");
			if(usedProperties.Count > 0) {
				ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(target.GetType());
				foreach(string propertyName in usedProperties) {
					IMemberInfo memberInfo = targetTypeInfo.FindMember(propertyName);
					if(memberInfo == null) {
						string message = SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ValidationCannotFindThePropertyWithinTheClass, ruleId, propertyName, targetTypeInfo.Type.FullName);
						throw new MemberNotFoundException(targetTypeInfo.Type, propertyName, message);
					}
					if(IsEmptyValueType(memberInfo.MemberType) || (memberInfo.GetPath().Count > 1)) {
						if(RuleSet.IsEmptyValue(target, propertyName, memberInfo.GetValue(target))) {
							return false;
						}
					}
				}
			}
			return true;
		}
		internal static bool IsRuleContext(IRule rule, Type objectType, string contextId) {
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentNotNull(rule, "rule");
			Guard.ArgumentNotNull(rule.Properties, "rule.Properties");
			Guard.ArgumentNotNull(rule.Properties.TargetType, "rule.Properties.TargetType");
			ContextIdentifiers ruleContexts = new ContextIdentifiers(rule.Properties.TargetContextIDs); 
			return (ruleContexts.Contains(contextId) || string.IsNullOrEmpty(contextId)) && rule.Properties.TargetType.IsAssignableFrom(objectType);
		}
		private List<Type> GetObjectTypes(IEnumerable objects) {
			List<Type> targetTypes = new List<Type>();
			foreach(object target in objects) {
				if(target != null && !targetTypes.Contains(target.GetType())) {
					targetTypes.Add(target.GetType());
				}
			}
			return targetTypes;
		}
		private void OnValidationFailed(ValidationCompletedEventArgs args) {
			OnValidationCompleted(this, args);
			if(!args.Handled && args.Exception != null) {
				throw args.Exception;
			}
		}
		private void OnValidationPassed(RuleSetValidationResult result, IObjectSpace objectSpace) {
			OnValidationCompleted(this, new ValidationCompletedEventArgs(result, objectSpace));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnValidationCompleted(object sender, ValidationCompletedEventArgs args) {
			if(ValidationCompleted != null) {
				ValidationCompleted(sender, args);
			}
		}
		private RuleSetSnapShot CreateSnapShot() {
			return CreateSnapShot(new object[0]);
		}
		private RuleSetSnapShot CreateSnapShot(Type targetTypesForDelayedCollectRules) {
			return CreateSnapShot(new Type[] { targetTypesForDelayedCollectRules });
		}
		private RuleSetSnapShot CreateSnapShot(object targetForDelayedCollectRules) {
			Guard.ArgumentNotNull(targetForDelayedCollectRules, "targetForDelayedCollectRules");
			return CreateSnapShot(targetForDelayedCollectRules.GetType());
		}
		private RuleSetSnapShot CreateSnapShot(IEnumerable<Type> targetTypesForDelayedCollectRules) {
			return new RuleSetSnapShot(GetRules(targetTypesForDelayedCollectRules));
		}
		private RuleSetSnapShot CreateSnapShot(IEnumerable targetsForDelayedCollectRules) {
			return CreateSnapShot(GetObjectTypes(targetsForDelayedCollectRules));
		}
		public RuleSet(IEnumerable<IRule> rules, IEnumerable<IRuleSource> sources)
			: this() {
			RegisteredRules.AddRange(rules);
			RegisteredSources.AddRange(sources);
		}
		public RuleSet() { }
		public RuleSet(RuleSet source) {
			RegisteredRules.AddRange(source.registeredRules);
			registeredSources.AddRange(source.registeredSources);
		}
		protected static bool IsEmptyValueCore(object value) {
			if(value == null)
				return true;
			string stringValue = value as string;
			if(stringValue != null) {
				return stringValue.Length == 0;
			}
			if(value is DateTime) {
				return (DateTime)value == DateTime.MinValue;
			}
			IEmptyCheckable emptyCheckable = value as IEmptyCheckable;
			if(emptyCheckable != null) {
				return emptyCheckable.IsEmpty;
			}
			return false;
		}
		protected static void RaiseCustomIsEmptyValue(CustomIsEmptyValueEventArgs args) {
			if(CustomIsEmptyValue != null) {
				CustomIsEmptyValue(null, args);
			}
		}
		public static List<Type> NonEmptyValueTypes = new List<Type>() { typeof(bool) }; 
		public static bool IsEmptyValueType(Type type) {
			CustomIsEmptyValueTypeEventArgs args = new CustomIsEmptyValueTypeEventArgs(type);
			if(CustomIsEmptyValueType != null) {
				CustomIsEmptyValueType(null, args);
			}
			if(args.Handled) {
				return args.IsEmpty;
			}
			else {
				return !NonEmptyValueTypes.Contains(type);
			}
		}
		public static bool IsEmptyValue(object targetObject, string propertyName, object propertyValue) {
			CustomIsEmptyValueEventArgs args = new CustomIsEmptyValueEventArgs(targetObject, propertyName, propertyValue);
			RaiseCustomIsEmptyValue(args);
			if(args.Handled) {
				return args.IsEmpty;
			}
			return IsEmptyValueCore(propertyValue);
		}
		[DefaultValue(false)]
		public static bool NeedToValidateAggregatedRulesInNestedObjectSpace { get; set; }
		public static bool NeedToValidateRule(IObjectSpace targetObjectSpace, IRule rule, object target, out string reason) {
			return NeedToValidateRule(targetObjectSpace, rule, target, null, out reason);
		}
		public static bool NeedToValidateRule(IObjectSpace targetObjectSpace, IRule rule, object target, string contextId, out string reason) {
			Guard.ArgumentNotNull(rule, "rule");
			Guard.ArgumentNotNull(target, "target");
			CustomNeedToValidateRuleEventArgs customNeedToValidateRuleEventArgs = new CustomNeedToValidateRuleEventArgs(rule, target, contextId, targetObjectSpace);
			RuleSet.RaiseCustomNeedToValidateRule(customNeedToValidateRuleEventArgs);
			if(customNeedToValidateRuleEventArgs.Handled) {
				reason = customNeedToValidateRuleEventArgs.Reason;
				return customNeedToValidateRuleEventArgs.NeedToValidateRule;
			}
			reason = string.Empty;
			if(!IsRuleContext(rule, target.GetType(), contextId)) {
				reason = string.Empty;
				return false;
			}
			if(!NeedToValidateAggregatedRulesInNestedObjectSpace && (targetObjectSpace is INestedObjectSpace)) {
				IRuleSupportsCollectionAggregatesProperties aggregateProperties = rule.Properties as IRuleSupportsCollectionAggregatesProperties;
				if(aggregateProperties != null && aggregateProperties.TargetCollectionAggregate.HasValue && targetObjectSpace.Contains(target)) {
					reason = "Rules with TargetCollectionAggregate is not checked in NestedObjectSpace";
					return false;
				}
			}
			if(rule.Properties is IRuleCollectionPropertyProperties) {
				IRuleCollectionPropertyProperties collectionPropertyProperties = (IRuleCollectionPropertyProperties)rule.Properties;
				if(collectionPropertyProperties.TargetCollectionOwnerType != null && !string.IsNullOrEmpty(collectionPropertyProperties.TargetCollectionPropertyName)) {
					string collectionCriteriaReason = ObjectFormatter.Format(collectionPropertyProperties.MessageTemplateTargetDoesNotSatisfyCollectionCriteria, rule);
					ITypeInfo ownerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(collectionPropertyProperties.TargetCollectionOwnerType);
					IMemberInfo collectionPropertyInfo = ownerTypeInfo.FindMember(collectionPropertyProperties.TargetCollectionPropertyName);
					object membeValue = RuleCollectionPropertyTargetCriteriaHelper.GetCollectionMemberValue(collectionPropertyInfo, target);
					if(membeValue == null || (membeValue is IList && ((IList)membeValue).Count == 0)) {
						reason = collectionCriteriaReason;
						return false;
					}
				}
			}
			if(!RuleSet.IsAcceptableTargetByRuleTargetCriteria(targetObjectSpace, target, rule.Properties.TargetCriteria)) {
				reason = ObjectFormatter.Format(rule.Properties.MessageTemplateTargetDoesNotSatisfyTargetCriteria, rule);
				return false;
			}
			else if(!RuleSet.IsAcceptableTargetBySkipNullOrEmptyValues(target, rule.Properties.SkipNullOrEmptyValues, rule.UsedProperties, rule.Id)) {
				reason = ObjectFormatter.Format(rule.Properties.MessageTemplateSkipNullOrEmptyValues, rule);
				return false;
			}
			return true;
		}
		public static RuleValidationResult ValidateRule(IObjectSpace targetObjectSpace, IRule rule, object target) {
			string message = string.Empty;
			if(!NeedToValidateRule(targetObjectSpace, rule, target, out message)) {
				return new RuleValidationResult(rule, target, ValidationState.Skipped, message);
			}
			return rule.Validate(target);
		}
		public void RegisterRules(ITypeInfo targetType) {
			if(targetType == null || queriedTypes.ContainsKey(targetType)) {
				return;
			}
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "EnsureRules(Type)", queriedTypes);
			lock(queriedTypes) {
				Tracing.Tracer.LogLockedSectionEntered();
				ITypeInfo currentType = targetType;
				while(currentType != null) {
					if(queriedTypes.ContainsKey(currentType))
						break;
					RegisterRulesFromType(currentType);
					queriedTypes.Add(currentType, null);
					currentType = currentType.Base;
				}
				foreach(ITypeInfo implementedInterface in targetType.ImplementedInterfaces) {
					if(!queriedTypes.ContainsKey(implementedInterface)) {
						RegisterRulesFromType(implementedInterface);
						queriedTypes.Add(implementedInterface, null);
					}
				}
			}
		}
		private void RegisterRulesFromType(ITypeInfo targetType) {
			foreach(CodeRuleAttribute attribute in targetType.FindAttributes<CodeRuleAttribute>(false)) {
				attribute.SetDeclaringClass(targetType.Type);
				RegisteredRules.Add(attribute.CreateRule());
			}
			foreach(RuleBaseAttribute attribute in targetType.FindAttributes<RuleBaseAttribute>(false)) {
				RuleBaseAttribute clonedAttribute = attribute.Clone();
				((IRuleBaseAttribute)clonedAttribute).DeclaringClass = targetType.Type;
				RegisteredRules.Add(((IRuleBaseAttribute)clonedAttribute).CreateRule());
			}
			foreach(IMemberInfo property in targetType.OwnMembers) {
				foreach(RuleBaseAttribute attribute in property.FindAttributes<RuleBaseAttribute>(false)) {
					RuleBaseAttribute clonedAttribute = attribute.Clone();
					((IRuleBaseAttribute)clonedAttribute).DeclaringClass = targetType.Type;
					((IRuleBaseAttribute)clonedAttribute).DeclaringProperty = property.Name;
					RegisteredRules.Add(((IRuleBaseAttribute)clonedAttribute).CreateRule());
				}
			}
		}
		public void Clear() {
			lock(registeredRules) {
				registeredRules.Clear();
			}
			lock(registeredSources) {
				registeredSources.Clear();
			}
			lock(queriedAssemblies) {
				queriedAssemblies.Clear();
			}
			lock(queriedTypes) {
				queriedTypes.Clear();
			}
#if DebugTest
			ValidationCompleted = null;
#endif
		}
		public RuleSetValidationResult ValidateTarget(IObjectSpace targetObjectSpace, object target, ContextIdentifiers contextIDs) {
			return CreateSnapShot(target).Validate(targetObjectSpace, target, contextIDs, true);
		}
		public RuleSetValidationResult ValidateAllTargets(IObjectSpace targetObjectSpace, IEnumerable targets, ContextIdentifiers contextIDs) {
			return CreateSnapShot(targets).ValidateAll(targetObjectSpace, targets, contextIDs, true);
		}
		public RuleSetValidationResult ValidateAllTargets(IObjectSpace targetObjectSpace, IEnumerable targets) {
			return CreateSnapShot(targets).ValidateAll(targetObjectSpace, targets, true);
		}
		public void Validate(IObjectSpace targetObjectSpace, object target, ContextIdentifiers contextIDs, ValidationFailedDelegate validationFailedDelegate) {
			RuleSetValidationResult result = ValidateTarget(targetObjectSpace, target, contextIDs);
			if(result.ValidationOutcome > ValidationOutcome.Information) {
				ValidationCompletedEventArgs args = new ValidationCompletedEventArgs(result, targetObjectSpace);
				if(validationFailedDelegate != null) {
					validationFailedDelegate(args);
				}
				OnValidationFailed(args);
			}
			else {
				OnValidationPassed(result, targetObjectSpace);
			}
		}
		public bool ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets, ContextIdentifiers contextIDs, ValidationFailedDelegate validationFailedDelegate) {
			RuleSetValidationResult result = ValidateAllTargets(targetObjectSpace, targets, contextIDs);
			if(result.ValidationOutcome > ValidationOutcome.Information) {
				ValidationCompletedEventArgs args = new ValidationCompletedEventArgs(result, targetObjectSpace);
				if(validationFailedDelegate != null) {
					validationFailedDelegate(args);
				}
				OnValidationFailed(args);
				return false;
			}
			else {
				OnValidationPassed(result, targetObjectSpace);
				return true;
			}
		}
		public void Validate(IObjectSpace targetObjectSpace, object target, ContextIdentifiers contextIDs) {
			Validate(targetObjectSpace, target, contextIDs, NullDelegate);
		}
		public bool ValidateAll(IObjectSpace targetObjectSpace, IEnumerable targets, ContextIdentifiers contextIDs) {
			return ValidateAll(targetObjectSpace, targets, contextIDs, NullDelegate);
		}
		public ReadOnlyCollection<IRule> GetRules() {
			return GetRules(new Type[0]);
		}
		public ReadOnlyCollection<IRule> GetRules(object target, ContextIdentifiers contextIDs) {
			Guard.ArgumentNotNull(target, "target");
			return GetRules(target.GetType(), contextIDs);
		}
		public ReadOnlyCollection<IRule> GetRules(Type targetType, ContextIdentifiers contextIDs) {
			Guard.ArgumentNotNull(targetType, "targetType");
			return CreateSnapShot(targetType).GetTargetRules(targetType, contextIDs);
		}
		public ReadOnlyCollection<IRule> GetRules(object targetForDelayedCollectRules) {
			Guard.ArgumentNotNull(targetForDelayedCollectRules, "targetForDelayedCollectRules");
			return GetRules(new Type[] { targetForDelayedCollectRules.GetType() });
		}
		public ReadOnlyCollection<IRule> GetRules(IEnumerable<Type> targetTypesForDelayedCollectRules) {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "GetRules", registeredRules);
			lock(registeredRules) {
				lock(registeredSources) {
					Tracing.Tracer.LogLockedSectionEntered();
					if(EnableDelayedRuleRegistration && (isDelayedRuleRegistrationMode || (registeredRules.Count == 0) && (registeredSources.Count == 0))) {
						isDelayedRuleRegistrationMode = true;
						foreach(Type type in targetTypesForDelayedCollectRules) {
							if(type != null) {
								RegisterRules(XafTypesInfo.Instance.FindTypeInfo(type));
							}
						}
					}
					List<IRule> result = new List<IRule>(registeredRules);
					foreach(IRuleSource source in registeredSources) {
						foreach(IRule rule in source.CreateRules()) {
							result.Add(rule);
						}
					}
					return IgnoreWarningAndInformationRules ? new ReadOnlyCollection<IRule>(new List<IRule>(result.Where<IRule>(x => x.Properties.ResultType == ValidationResultType.Error))) : new ReadOnlyCollection<IRule>(result);
				}
			}
		}
		public ReadOnlyCollection<IRule> GetRules(IEnumerable targetsForDelayedCollectRules) {
			return GetRules(GetObjectTypes(targetsForDelayedCollectRules));
		}
		public IRule FindRule(string id) {
			foreach(IRule rule in GetRules()) {
				if(rule.Id == id) {
					return rule;
				}
			}
			return null;
		}
		public IEnumerator<IRule> GetEnumerator() {
			return GetRules().GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetRules().GetEnumerator();
		}
		public bool EnableDelayedRuleRegistration {
			get { return enableDelayedRuleRegistration; }
			set { enableDelayedRuleRegistration = value; }
		}
		public List<IRule> RegisteredRules {
			get { return registeredRules; }
		}
		public List<IRuleSource> RegisteredSources {
			get { return registeredSources; }
		}
		public bool IgnoreWarningAndInformationRules { get; set; }
		internal static void RaiseCustomValidateRule(CustomValidateRuleEventArgs args) {
			if(CustomValidateRule != null) {
				CustomValidateRule(null, args);
			}
		}
		internal static void RaiseCustomNeedToValidateRule(CustomNeedToValidateRuleEventArgs args) {
			if(CustomNeedToValidateRule != null) {
				CustomNeedToValidateRule(null, args);
			}
		}
		internal static void RaiseRuleValidated(RuleValidatedEventArgs args) {
			if(RuleValidated != null) {
				RuleValidated(null, args);
			}
		}
		public event EventHandler<ValidationCompletedEventArgs> ValidationCompleted;
		public static event EventHandler<CustomValidateRuleEventArgs> CustomValidateRule;
		public static event EventHandler<RuleValidatedEventArgs> RuleValidated;
		public static event EventHandler<CustomNeedToValidateRuleEventArgs> CustomNeedToValidateRule;
		public static event EventHandler<CustomIsEmptyValueEventArgs> CustomIsEmptyValue;
		public static event EventHandler<CustomIsEmptyValueTypeEventArgs> CustomIsEmptyValueType;
	}
}
