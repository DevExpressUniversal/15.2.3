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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Persistent.Validation {
	public enum CriteriaEvaluationBehavior {
		BeforeTransaction = 0,
		InTransaction = 1,
	}
	public abstract class RuleSearchObject : RuleBase {
		public const string PropertiesFoundObjectMessagesSeparator = "FoundObjectMessagesSeparator";
		public const string PropertiesFoundObjectMessageFormat = "FoundObjectMessageFormat";
		public const string PropertiesCriteriaEvaluationBehavior = "CriteriaEvaluationBehavior";
		public const string PropertiesIncludeCurrentObject = "IncludeCurrentObject";
		public const string PropertiesMessageTemplateFoundObjects = "MessageTemplateFoundObjects";
		internal const string OpeningBraceReplacement = "OPEN_BRACE";
		internal const string ClosingBraceReplacement = "CLOSE_BRACE";
		public static IMemberInfo GetCaptionMemberDescriptor(Type type) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
			return GetCaptionMemberDescriptor(typeInfo);
		}
		public static IMemberInfo GetCaptionMemberDescriptor(ITypeInfo typeInfo) {
			return ReflectionHelper.FindDisplayableMemberDescriptor(typeInfo.DefaultMember);
		}
		private CriteriaOperator GetCurrentObjectCriteriaOperator(ITypeInfo targetTypeInfo) {
			string identificationMember = "This";
			if(targetTypeInfo.FindMember(identificationMember) == null) {
				identificationMember = targetTypeInfo.KeyMember.Name;
			}
			return new BinaryOperator(identificationMember, "@" + identificationMember);
		}
		private CriteriaOperator CreateOperator(object target, Type looksForType, CriteriaOperator criteria) {
			return CreateOperator(target, looksForType, criteria, true);
		}
		private CriteriaOperator CreateOperator(object target, Type looksForType, CriteriaOperator criteria, bool generateExcludeCurrentObjectCriteria) {
			CriteriaOperator criteriaToCheck = criteria;
			if(!Properties.IncludeCurrentObject && looksForType.IsAssignableFrom(target.GetType()) && generateExcludeCurrentObjectCriteria) {
				CriteriaOperator currentObjectOperator = GetCurrentObjectCriteriaOperator(XafTypesInfo.Instance.FindTypeInfo(target.GetType()));
				if(!ReferenceEquals(currentObjectOperator, null)) {
					criteriaToCheck = new GroupOperator(criteriaToCheck, new NotOperator(currentObjectOperator));
				}
			}
			CriteriaWrapper criteriaWrapper = new CriteriaWrapper(looksForType, criteriaToCheck);
			criteriaWrapper.UpdateParametersValues(target);
			return criteriaWrapper.CriteriaOperator;
		}
		private void FillCollection(IMemberInfo collectionPropertyInfo, object associationMemberValue, ExpressionEvaluator evaluator, IList collection) {
			IList ownerMemberValue = collectionPropertyInfo.GetValue(associationMemberValue) as IList;
			if(ownerMemberValue != null) {
				for(int j = 0; j < ownerMemberValue.Count; j++) {
					if(evaluator.Fit(ownerMemberValue[j]) && !collection.Contains(ownerMemberValue[j])) {
						collection.Add(ownerMemberValue[j]);
					}
				}
			}
		}
		protected bool IsSearchedObjectsExist(object target, Type looksForType, out string lastSearchResults) {
			return IsSearchedObjectsExist(target, looksForType, out lastSearchResults, GetSearchCriteriaCore(target));
		}
		protected bool IsSearchedObjectsExist(object target, Type looksForType, out string lastSearchResults, CriteriaOperator criteria) {
			int count = 0;
			lastSearchResults = "";
			if(looksForType.IsInterface || Properties.CriteriaEvaluationBehavior == CriteriaEvaluationBehavior.InTransaction || !string.IsNullOrEmpty(Properties.FoundObjectMessageFormat)) {
				IList collection = new List<Object>();
				ExpressionEvaluator evaluator = targetObjectSpace.GetExpressionEvaluator(target.GetType(), CreateOperator(target, looksForType, criteria, false));
				if(Properties.TargetCollectionOwnerType != null && !string.IsNullOrEmpty(Properties.TargetCollectionPropertyName)) {
					ITypeInfo ownerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(Properties.TargetCollectionOwnerType);
					IMemberInfo collectionPropertyInfo = ownerTypeInfo.FindMember(Properties.TargetCollectionPropertyName);
					if(collectionPropertyInfo.IsManyToMany) {
						IList associationMemberValue = RuleCollectionPropertyTargetCriteriaHelper.GetCollectionMemberValue(collectionPropertyInfo, target) as IList;
						if(associationMemberValue != null) {
							for(int i = 0; i < associationMemberValue.Count; i++) {
								FillCollection(collectionPropertyInfo, associationMemberValue[i], evaluator, collection);
							}
						}
					}
					else {
						object associationMemberValue = RuleCollectionPropertyTargetCriteriaHelper.GetCollectionMemberValue(collectionPropertyInfo, target);
						if(associationMemberValue != null) {
							FillCollection(collectionPropertyInfo, associationMemberValue, evaluator, collection);
						}
					}
				}
				else {
					collection = targetObjectSpace.GetObjects(looksForType, CreateOperator(target, looksForType, criteria, false), Properties.CriteriaEvaluationBehavior == CriteriaEvaluationBehavior.InTransaction);
				}
				if(!Properties.IncludeCurrentObject && looksForType.IsAssignableFrom(target.GetType())) {
					collection.Remove(target);
				}
				count = collection.Count;
				if(string.IsNullOrEmpty(Properties.FoundObjectMessageFormat)) {
					lastSearchResults = string.Empty;
				}
				else {
					List<string> captions = new List<string>();
					IMemberInfo captionMemberDescriptor = GetCaptionMemberDescriptor(looksForType);
					foreach(object obj in collection) {
						object value = (captionMemberDescriptor != null) ? captionMemberDescriptor.GetValue(obj) : obj;
						string preprocessedFormat = Properties.FoundObjectMessageFormat;
						if(Properties.FoundObjectMessageFormat.Contains("{0}")) {
							string displayText = value != null ? value.ToString() : string.Empty;
							displayText = displayText.Replace("{", OpeningBraceReplacement).Replace("}", ClosingBraceReplacement);
							preprocessedFormat = Properties.FoundObjectMessageFormat.Replace("{0}", displayText);
						}
						CustomFormatValidationMessageEventArgs args = new CustomFormatValidationMessageEventArgs(Properties.FoundObjectMessageFormat, obj);
						if(CustomFormatFoundObjectMessage != null) {
							CustomFormatFoundObjectMessage(this, args);
						}
						if(args.Handled) {
							captions.Add(args.ResultMessage);
						}
						else {
							captions.Add(string.Format(new ObjectFormatter(), preprocessedFormat, obj));
						}
					}
					lastSearchResults = string.Join(Properties.FoundObjectMessagesSeparator, captions.ToArray()).TrimEnd();
				}
			}
			else {
				count = targetObjectSpace.GetObjectsCount(looksForType, CreateOperator(target, looksForType, criteria));
			}
			return count > 0;
		}
		protected string GetFoundObjectsString(string lastSearchResults) {
			if(!string.IsNullOrEmpty(Properties.FoundObjectMessageFormat) && Properties.FoundObjectMessageFormat.Contains("{0")) {
				return " " + Properties.MessageTemplateFoundObjects + lastSearchResults;
			}
			return string.Empty;
		}
		protected RuleSearchObject(string id, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, targetContextIDs, objectType) {
		}
		public RuleSearchObject() { }
		public RuleSearchObject(IRuleSearchObjectProperties properties) : base(properties) { }
		protected static ReadOnlyCollection<string> GetUsedPropertiesFromCriteria(Type looksForType, string criteria) {
			CriteriaWrapper tempWrapper = new CriteriaWrapper(looksForType, criteria);
			return new List<string>(tempWrapper.EditableParameters.Keys).AsReadOnly();
		}
		protected virtual CriteriaOperator GetSearchCriteriaCore(object target) {
			return null;
		}
		public new IRuleSearchObjectProperties Properties {
			get { return (IRuleSearchObjectProperties)base.Properties; }
		}
		public static string DefaultMessageTemplateFoundObjects {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleSearchObject_defaultMessageTemplateFoundObjects");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.FoundObjects;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleSearchObject_defaultMessageTemplateFoundObjects").Value = value; }
		}
		public static event EventHandler<CustomFormatValidationMessageEventArgs> CustomFormatFoundObjectMessage;
	}
	public class RuleObjectExists : RuleSearchObject {
		public const string PropertiesLooksFor = "LooksFor";
		public const string PropertiesCriteria = "Criteria";
		public const string PropertiesMessageTemplateMustExist = "MessageTemplateMustExist";
		public static string DefaultMessageTemplateMustExist {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleObjectExists_defaultMessageTemplateMustExist");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustExist;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleObjectExists_defaultMessageTemplateMustExist").Value = value; }
		}
		protected override CriteriaOperator GetSearchCriteriaCore(object target) {
			return CriteriaOperator.Parse(Properties.Criteria);
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			string lastSearchResults = "";
			bool result = IsSearchedObjectsExist(target, Properties.LooksFor, out lastSearchResults);
			errorMessageTemplate = Properties.MessageTemplateMustExist;
			if(result) {
				errorMessageTemplate += GetFoundObjectsString(lastSearchResults);
			}
			return result;
		}
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				return GetUsedPropertiesFromCriteria(Properties.LooksFor, Properties.Criteria);
			}
		}
		public RuleObjectExists(string id, ContextIdentifiers targetContextIDs, Type objectType) : base(id, targetContextIDs, objectType) { }
		public RuleObjectExists() { }
		public RuleObjectExists(IRuleObjectExistsProperties properties) : base(properties) { }
		public new IRuleObjectExistsProperties Properties {
			get { return (IRuleObjectExistsProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleObjectExistsProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class RuleObjectExistsAttribute : RuleBaseAttribute, IRuleObjectExistsProperties {
		public RuleObjectExistsAttribute(string criteria)
			: this(null, DefaultContexts.Save, criteria) { }
		public RuleObjectExistsAttribute(string id, string targetContextIDs, string criteria)
			: this(id, targetContextIDs, criteria, string.Empty) { }
		public RuleObjectExistsAttribute(string id, DefaultContexts targetContexts, string criteria)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), criteria) { }
		public RuleObjectExistsAttribute(DefaultContexts targetContexts, string criteria)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), criteria) { }
		public RuleObjectExistsAttribute(string id, string targetContextIDs, string criteria, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["Criteria"] = criteria;
		}
		public RuleObjectExistsAttribute(string id, DefaultContexts targetContexts, string criteria, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), criteria, messageTemplate) { }
		protected new IRuleObjectExistsProperties Properties {
			get { return (IRuleObjectExistsProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleObjectExists); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleObjectExistsProperties); }
		}
		public string FoundObjectMessagesSeparator {
			get { return Properties.FoundObjectMessagesSeparator; }
			set { 
				if(properties == null) {
					propertiesDictionary["FoundObjectMessagesSeparator"] = value;
				}
				else {
					Properties.FoundObjectMessagesSeparator = value;
				}
			}
		}
		public string FoundObjectMessageFormat {
			get { return Properties.FoundObjectMessageFormat; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessageFormat"] = value;
				}
				else {
					Properties.FoundObjectMessageFormat = value;
				}
			}
		}
		public CriteriaEvaluationBehavior CriteriaEvaluationBehavior {
			get { return Properties.CriteriaEvaluationBehavior; }
			set {
				if(properties == null) {
					propertiesDictionary["CriteriaEvaluationBehavior"] = value;
				}
				else {
					Properties.CriteriaEvaluationBehavior = value;
				}
			}
		}
		public Type LooksFor {
			get { return Properties.LooksFor; }
			set {
				if(properties == null) {
					propertiesDictionary["LooksFor"] = value;
				}
				else {
					Properties.LooksFor = value;
				}
			}
		}
		public bool IncludeCurrentObject {
			get { return Properties.IncludeCurrentObject; }
			set {
				if(properties == null) {
					propertiesDictionary["IncludeCurrentObject"] = value;
				}
				else {
					Properties.IncludeCurrentObject = value;
				}
			}
		}
		string IRuleObjectExistsProperties.Criteria {
			get { return Properties.Criteria; }
			set { Properties.Criteria = value; }
		}
		public string MessageTemplateMustExist {
			get { return Properties.MessageTemplateMustExist; }
			set {
				if(properties == null) {
					propertiesDictionary["MessageTemplateMustExist"] = value;
				}
				else {
					Properties.MessageTemplateMustExist = value;
				}
			}
		}
		string IRuleSearchObjectProperties.MessageTemplateFoundObjects {
			get { return Properties.MessageTemplateFoundObjects; }
			set { Properties.MessageTemplateFoundObjects = value; }
		}
	}
	public class RuleUniqueValue : RuleSearchObject {
		public const string PropertiesTargetPropertyName = "TargetPropertyName";
		public const string PropertiesMessageTemplateMustBeUnique = "MessageTemplateMustBeUnique";
		internal static string GetUniqueCriteriaTemplate(bool skipNullValues) {
			string criteriaTemplate = "([{0}] == '@{0}')";
			if(!skipNullValues) {
				criteriaTemplate = "(" + criteriaTemplate + " OR (IsNullOrEmpty([{0}]) AND IsNullOrEmpty('@{0}')))";
			}
			return criteriaTemplate;
		}
		public static string DefaultMessageTemplateMustBeUnique {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleUniqueValue_defaultMessageTemplateMustBeUnique");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeUnique;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleUniqueValue_defaultMessageTemplateMustBeUnique").Value = value; }
		}
		protected override CriteriaOperator GetSearchCriteriaCore(object target) {
			return GetCriteria();
		}
		protected virtual CriteriaOperator GetCriteria() {
			string criteriaTemplate = GetUniqueCriteriaTemplate(Properties.SkipNullOrEmptyValues);
			return CriteriaOperator.Parse(string.Format(criteriaTemplate, Properties.TargetPropertyName));
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			string lastSearchResults = "";
			bool result = !IsSearchedObjectsExist(target, Properties.TargetType, out lastSearchResults);
			errorMessageTemplate = Properties.MessageTemplateMustBeUnique;
			if(!result) {
				errorMessageTemplate += GetFoundObjectsString(lastSearchResults);
			}
			return result;
		}
		public RuleUniqueValue(string id, ContextIdentifiers targetContextIDs, Type objectType)
			: base(id, targetContextIDs, objectType) {
		}
		public RuleUniqueValue() { }
		public RuleUniqueValue(IRuleUniqueValueProperties properties) : base(properties) { }
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				return new ReadOnlyCollection<string>(new string[] { Properties.TargetPropertyName });
			}
		}
		public new IRuleUniqueValueProperties Properties {
			get { return (IRuleUniqueValueProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleUniqueValueProperties); }
		}
		public object TargetValue {
			get {
				if(TargetObject != null) {
					ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(TargetObject.GetType());
					IMemberInfo targetProperty = targetTypeInfo.FindMember(Properties.TargetPropertyName);
					if(targetProperty != null) {
						return targetProperty.GetValue(TargetObject);
					}
				}
				return null;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class RuleUniqueValueAttribute : RuleBaseAttribute, IRuleUniqueValueProperties {
		public RuleUniqueValueAttribute()
			: this(null, DefaultContexts.Save) { }
		public RuleUniqueValueAttribute(string id, string targetContextIDs)
			: base(id, targetContextIDs) { }
		public RuleUniqueValueAttribute(DefaultContexts targetContexts)
			: base(null, targetContexts) { }
		public RuleUniqueValueAttribute(string id, DefaultContexts targetContexts)
			: base(id, targetContexts) { }
		public RuleUniqueValueAttribute(string id, string targetContextIDs, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) { }
		public RuleUniqueValueAttribute(string id, DefaultContexts targetContexts, string messageTemplate)
			: base(id, targetContexts, messageTemplate) { }
		protected new IRuleUniqueValueProperties Properties {
			get { return (IRuleUniqueValueProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleUniqueValue); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleUniqueValueProperties); }
		}
		public string FoundObjectMessagesSeparator {
			get { return Properties.FoundObjectMessagesSeparator; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessagesSeparator"] = value;
				}
				else {
					Properties.FoundObjectMessagesSeparator = value;
				}
			}
		}
		public string FoundObjectMessageFormat {
			get { return Properties.FoundObjectMessageFormat; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessageFormat"] = value;
				}
				else {
					Properties.FoundObjectMessageFormat = value;
				}
			}
		}
		public CriteriaEvaluationBehavior CriteriaEvaluationBehavior {
			get { return Properties.CriteriaEvaluationBehavior; }
			set {
				if(properties == null) {
					propertiesDictionary["CriteriaEvaluationBehavior"] = value;
				}
				else {
					Properties.CriteriaEvaluationBehavior = value;
				}
			}
		}
		bool IRuleSearchObjectProperties.IncludeCurrentObject {
			get { return Properties.IncludeCurrentObject; }
			set { Properties.IncludeCurrentObject = value; }
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
		string IRuleUniqueValueProperties.MessageTemplateMustBeUnique {
			get { return Properties.MessageTemplateMustBeUnique; }
			set { Properties.MessageTemplateMustBeUnique = value; }
		}
		string IRuleSearchObjectProperties.MessageTemplateFoundObjects {
			get { return Properties.MessageTemplateFoundObjects; }
			set { Properties.MessageTemplateFoundObjects = value; }
		}
	}
	public class RuleIsReferenced : RuleSearchObject {
		public const string PropertiesReferencePropertyName = "ReferencePropertyName";
		public const string PropertiesMessageTemplateMustBeReferenced = "MessageTemplateMustBeReferenced";
		public const string PropertiesLooksFor = "LooksFor";
		public static string DefaultMessageTemplateMustBeReferenced {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleIsReferenced_defaultMessageTemplateMustBeReferenced");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.MustBeReferenced;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleIsReferenced_defaultMessageTemplateMustBeReferenced").Value = value; }
		}
		protected override CriteriaOperator GetSearchCriteriaCore(object target) {
			return new BinaryOperator(Properties.ReferencePropertyName, target);
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			string lastSearchResults = "";
			bool result = IsSearchedObjectsExist(target, Properties.LooksFor, out lastSearchResults);
			errorMessageTemplate = Properties.MessageTemplateMustBeReferenced;
			if(result) {
				errorMessageTemplate += GetFoundObjectsString(lastSearchResults);
			}
			return result;
		}
		public RuleIsReferenced(string id, ContextIdentifiers targetContextIDs, Type objectType) : base(id, targetContextIDs, objectType) { }
		public RuleIsReferenced() { }
		public RuleIsReferenced(IRuleIsReferencedProperties properties) : base(properties) { }
		public new IRuleIsReferencedProperties Properties {
			get { return (IRuleIsReferencedProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleIsReferencedProperties); }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class RuleIsReferencedAttribute : RuleBaseAttribute, IRuleIsReferencedProperties {
		public RuleIsReferencedAttribute(Type looksFor, string referencePropertyName)
			: this(null, DefaultContexts.Save, looksFor, referencePropertyName) { }
		public RuleIsReferencedAttribute(string id, string targetContextIDs, Type looksFor, string referencePropertyName)
			: this(id, targetContextIDs, looksFor, referencePropertyName, string.Empty) { }
		public RuleIsReferencedAttribute(DefaultContexts targetContexts, Type looksFor, string referencePropertyName)
			: this(null, ((ContextIdentifiers)targetContexts).ToString(), looksFor, referencePropertyName) { }
		public RuleIsReferencedAttribute(string id, DefaultContexts targetContexts, Type looksFor, string referencePropertyName)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), looksFor, referencePropertyName) { }
		public RuleIsReferencedAttribute(string id, string targetContextIDs, Type looksFor, string referencePropertyName, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["LooksFor"] = looksFor;
			propertiesDictionary["ReferencePropertyName"] = referencePropertyName;
		}
		public RuleIsReferencedAttribute(string id, DefaultContexts targetContexts, Type looksFor, string referencePropertyName, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), looksFor, referencePropertyName, messageTemplate) {
		}
		protected new IRuleIsReferencedProperties Properties {
			get { return (IRuleIsReferencedProperties)base.Properties; }
		}
		protected override Type RuleType {
			get { return typeof(RuleIsReferenced); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleIsReferencedProperties); }
		}
		public string FoundObjectMessagesSeparator {
			get { return Properties.FoundObjectMessagesSeparator; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessagesSeparator"] = value;
				}
				else {
					Properties.FoundObjectMessagesSeparator = value;
				}
			}
		}
		public string FoundObjectMessageFormat {
			get { return Properties.FoundObjectMessageFormat; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessageFormat"] = value;
				}
				else {
					Properties.FoundObjectMessageFormat = value;
				}
			}
		}
		public Type LooksFor {
			get { return Properties.LooksFor; }
			set {
				if(properties == null) {
					propertiesDictionary["LooksFor"] = value;
				}
				else {
					Properties.LooksFor = value;
				}
			}
		}
		public CriteriaEvaluationBehavior CriteriaEvaluationBehavior {
			get { return Properties.CriteriaEvaluationBehavior; }
			set {
				if(properties == null) {
					propertiesDictionary["CriteriaEvaluationBehavior"] = value;
				}
				else {
					Properties.CriteriaEvaluationBehavior = value;
				}
			}
		}
		string IRuleIsReferencedProperties.ReferencePropertyName {
			get { return Properties.ReferencePropertyName; }
			set { Properties.ReferencePropertyName = value; }
		}
		bool IRuleSearchObjectProperties.IncludeCurrentObject {
			get { return Properties.IncludeCurrentObject; }
			set { Properties.IncludeCurrentObject = value; }
		}
		public string MessageTemplateMustBeReferenced {
			get { return Properties.MessageTemplateMustBeReferenced; }
			set {
				if(properties == null) {
					propertiesDictionary["MessageTemplateMustBeReferenced"] = value;
				}
				else {
					Properties.MessageTemplateMustBeReferenced = value;
				}
			}
		}
		string IRuleSearchObjectProperties.MessageTemplateFoundObjects {
			get { return Properties.MessageTemplateFoundObjects; }
			set { Properties.MessageTemplateFoundObjects = value; }
		}
	}
	public class RuleCombinationOfPropertiesIsUnique : RuleSearchObject {
		public const string PropertiesTargetProperties = "TargetProperties";
		public const string Separators = ";, ";
		public const string PropertiesMessageTemplateCombinationOfPropertiesMustBeUnique = "MessageTemplateCombinationOfPropertiesMustBeUnique";
		public static string DefaultMessageTemplateCombinationOfPropertiesMustBeUnique {
			get {
				IValueManager<string> manager = ValueManager.GetValueManager<string>("RuleCombinationOfPropertiesIsUnique_defaultMessageTemplateCombinationOfPropertiesMustBeUnique");
				if(manager.Value == null) {
					manager.Value = RuleDefaultMessageTemplates.CombinationOfPropertiesMustBeUnique;
				}
				return manager.Value;
			}
			set { ValueManager.GetValueManager<string>("RuleCombinationOfPropertiesIsUnique_defaultMessageTemplateCombinationOfPropertiesMustBeUnique").Value = value; }
		}
		private string criteria = string.Empty;
		public RuleCombinationOfPropertiesIsUnique() {
		}
		public RuleCombinationOfPropertiesIsUnique(IRuleCombinationOfPropertiesIsUniqueProperties properties)
			: base(properties) {
		}
		private IList<string> ParseTargetProperties() {
			List<string> uniqueProperties = new List<string>();
			foreach(string item in Properties.TargetProperties.Split(Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
				uniqueProperties.Add(item.Trim());
			}
			return uniqueProperties;
		}
		private void BuildCriteriaString() {
			criteria = string.Empty;
			string criteriaTemplate = RuleUniqueValue.GetUniqueCriteriaTemplate(Properties.SkipNullOrEmptyValues);
			foreach(string currentProperty in ParseTargetProperties()) {
				if(!string.IsNullOrEmpty(criteria)) {
					criteria += " AND ";
				}
				criteria += string.Format(criteriaTemplate, currentProperty.Trim());
			}
		}
		protected override CriteriaOperator GetSearchCriteriaCore(object target) {
			if(string.IsNullOrEmpty(criteria)) {
				BuildCriteriaString();
			}
			return CriteriaOperator.Parse(criteria);
		}
		protected override bool IsValidInternal(object target, out string errorMessageTemplate) {
			string lastSearchResults = "";
			bool result = !IsSearchedObjectsExist(target, Properties.TargetType, out lastSearchResults);
			errorMessageTemplate = Properties.MessageTemplateCombinationOfPropertiesMustBeUnique;
			if(!result) {
				errorMessageTemplate += GetFoundObjectsString(lastSearchResults);
			}
			return result;
		}
		public override ReadOnlyCollection<string> UsedProperties {
			get {
				return new ReadOnlyCollection<string>(ParseTargetProperties());
			}
		}
		public new IRuleCombinationOfPropertiesIsUniqueProperties Properties {
			get { return (IRuleCombinationOfPropertiesIsUniqueProperties)base.Properties; }
		}
		public override Type PropertiesType {
			get { return typeof(RuleCombinationOfPropertiesIsUniqueProperties); }
		}
		public string FormattedTargetProperties {
			get {
				List<string> uniquePropertiesCaptions = new List<string>();
				foreach(string propertyName in ParseTargetProperties()) {
					uniquePropertiesCaptions.Add(CaptionHelper.GetFullMemberCaption(XafTypesInfo.Instance.FindTypeInfo(Properties.TargetType), propertyName));
				}
				return string.Join(", ", uniquePropertiesCaptions.ToArray());
			}
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class RuleCombinationOfPropertiesIsUniqueAttribute : RuleBaseAttribute, IRuleCombinationOfPropertiesIsUniqueProperties {
		protected override void EnsurePropertiesAreCorrectlyAssignedCore() {
			base.EnsurePropertiesAreCorrectlyAssignedCore();
			if(string.IsNullOrEmpty(Properties.TargetProperties)) {
				throw new InvalidOperationException("TargetProperties must be set.");
			}
			foreach(string propertyName in Properties.TargetProperties.Split(RuleCombinationOfPropertiesIsUnique.Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
				ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(Properties.TargetType);
				if(targetTypeInfo.FindMember(propertyName) == null) {
					throw new MemberNotFoundException(Properties.TargetType, propertyName);
				}
			}
		}
		public RuleCombinationOfPropertiesIsUniqueAttribute(string targetProperties)
			: this(null, DefaultContexts.Save, targetProperties) { }
		public RuleCombinationOfPropertiesIsUniqueAttribute(string id, string targetContextIDs, string targetProperties)
			: this(id, targetContextIDs, targetProperties, string.Empty) {
		}
		public RuleCombinationOfPropertiesIsUniqueAttribute(DefaultContexts targetContexts, string targetProperties)
			: this(null, targetContexts, targetProperties, string.Empty) {
		}
		public RuleCombinationOfPropertiesIsUniqueAttribute(string id, DefaultContexts targetContexts, string targetProperties)
			: this(id, targetContexts, targetProperties, string.Empty) {
		}
		public RuleCombinationOfPropertiesIsUniqueAttribute(string id, string targetContextIDs, string targetProperties, string messageTemplate)
			: base(id, targetContextIDs, messageTemplate) {
			propertiesDictionary["TargetProperties"] = targetProperties;
		}
		public RuleCombinationOfPropertiesIsUniqueAttribute(string id, DefaultContexts targetContexts, string targetProperties, string messageTemplate)
			: this(id, ((ContextIdentifiers)targetContexts).ToString(), targetProperties, messageTemplate) {
		}
		protected override Type RuleType {
			get { return typeof(RuleCombinationOfPropertiesIsUnique); }
		}
		protected override Type PropertiesType {
			get { return typeof(RuleCombinationOfPropertiesIsUniqueProperties); }
		}
		protected new IRuleCombinationOfPropertiesIsUniqueProperties Properties {
			get { return (IRuleCombinationOfPropertiesIsUniqueProperties)base.Properties; }
		}
		string IRuleCombinationOfPropertiesIsUniqueProperties.TargetProperties {
			get { return Properties.TargetProperties; }
			set { Properties.TargetProperties = value; }
		}
		public string MessageTemplateCombinationOfPropertiesMustBeUnique {
			get { return Properties.MessageTemplateCombinationOfPropertiesMustBeUnique; }
			set {
				if(properties == null) {
					propertiesDictionary["MessageTemplateCombinationOfPropertiesMustBeUnique"] = value;
				}
				else {
					Properties.MessageTemplateCombinationOfPropertiesMustBeUnique = value;
				}
			}
		}
		string IRuleSearchObjectProperties.MessageTemplateFoundObjects {
			get { return Properties.MessageTemplateFoundObjects; }
			set { Properties.MessageTemplateFoundObjects = value; }
		}
		public string FoundObjectMessagesSeparator {
			get { return Properties.FoundObjectMessagesSeparator; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessagesSeparator"] = value;
				}
				else {
					Properties.FoundObjectMessagesSeparator = value;
				}
			}
		}
		public string FoundObjectMessageFormat {
			get { return Properties.FoundObjectMessageFormat; }
			set {
				if(properties == null) {
					propertiesDictionary["FoundObjectMessageFormat"] = value;
				}
				else {
					Properties.FoundObjectMessageFormat = value;
				}
			}
		}
		public CriteriaEvaluationBehavior CriteriaEvaluationBehavior {
			get { return Properties.CriteriaEvaluationBehavior; }
			set {
				if(properties == null) {
					propertiesDictionary["CriteriaEvaluationBehavior"] = value;
				}
				else {
					Properties.CriteriaEvaluationBehavior = value;
				}
			}
		}
		bool IRuleSearchObjectProperties.IncludeCurrentObject {
			get { return Properties.IncludeCurrentObject; }
			set { Properties.IncludeCurrentObject = value; }
		}
	}
}
