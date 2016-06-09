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
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation.DiagnosticViews;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation.AllContextsView {
	[DomainComponent]
	[SuppressToolBar]
	[DisplayName("Validation Details")]
	public class DisplayableValidationResultItem {
		private RuleSetValidationResultItem ruleSetValidationResultItem;
		private string targetCaption;
		private IModelApplication modelApplication;
		private string BuildTargetCaption() {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(Target.GetType());
			StringBuilder builder = new StringBuilder();
			builder.Append("'");
			IMemberInfo defaultMember = typeInfo.DefaultMember;
			if(defaultMember != null) {
				builder.Append("{" + defaultMember.Name + "}");
			}
			else {
				builder.Append(Target.ToString());
			}
			builder.Append("' (" + CaptionHelper.GetClassCaption(typeInfo.FullName));
			string keyPropertyName = FriendlyKeyPropertyAttribute.FindFriendlyKeyMemberName(typeInfo, true);
			if(string.IsNullOrEmpty(keyPropertyName) && (typeInfo.KeyMember != null)) {
				keyPropertyName = typeInfo.KeyMember.Name;
			}
			if(!string.IsNullOrEmpty(keyPropertyName)) {
				builder.Append(", {" + keyPropertyName + "}");
			}
			builder.Append(")");
			return ObjectFormatter.Format(builder.ToString(), Target);
		}
		public DisplayableValidationResultItem(RuleSetValidationResultItem ruleSetValidationResultItem, IModelApplication modelApplication) {
			this.modelApplication = modelApplication;
			this.ruleSetValidationResultItem = ruleSetValidationResultItem;
			this.targetCaption = BuildTargetCaption();
		}
		public string RuleName {
			get { return ruleSetValidationResultItem.RuleName; }
		}
		public string ContextCaption {
			get { return ValidationModule.GetContextCaption(ruleSetValidationResultItem.ContextId.ToString(), modelApplication); }
		}
		public ContextIdentifier ContextId {
			get { return ruleSetValidationResultItem.ContextId; }
		}
		public IRule Rule {
			get { return ruleSetValidationResultItem.Rule; }
		}
		[Browsable(false)] 
		public ValidationState State {
			get {
				if(Outcome == ValidationOutcome.Skipped) {
					return ValidationState.Skipped;
				}
				return Outcome > ValidationOutcome.Warning ? ValidationState.Invalid : ValidationState.Valid;
			}
		}
		[DisplayName("State")] 
		public ValidationOutcome Outcome {
			get { return ruleSetValidationResultItem.ValidationOutcome; }
		}
		public string ErrorMessage {
			get { return ruleSetValidationResultItem.ErrorMessage; }
		}
		[Browsable(false)]
		public object Target {
			get { return ruleSetValidationResultItem.Target; }
		}
		[DisplayName("Target")]
		public string TargetCaption {
			get {
				if(string.IsNullOrEmpty(targetCaption)) {
					targetCaption = BuildTargetCaption();
				}
				return targetCaption;
			}
		}
		[Browsable(false)]
		public RuleSetValidationResultItem RuleSetValidationResultItem {
			get { return ruleSetValidationResultItem; }
		}
		[ExpandObjectMembers(ExpandObjectMembers.InListView), EditorAlias(EditorAliases.DetailPropertyEditor)]
		public IRuleBaseProperties RuleProperties {
			get { return ruleSetValidationResultItem.Rule.Properties; }
		}
	}
	public enum IsValid {
		[ImageName("State_Validation_Valid")]
		Valid,
		[ImageName("State_Validation_Invalid")]
		Invalid
	}
	public class ContextValidationResultSet {
		public ContextValidationResultSet(string contextId, RuleSetValidationResult ruleSetValidationResult, IModelApplication modelApplication) {
			this.GroupedResults = new Dictionary<ValidationOutcome, ContextValidationResult>();
			foreach(ValidationOutcome outcome in Enum.GetValues(typeof(ValidationOutcome))) {
				this.GroupedResults[outcome] = new ContextValidationResult(contextId, ruleSetValidationResult, modelApplication, outcome);
			}
		}
		public ValidationOutcome OverallContextOutcome {
			get {
				ValidationOutcome result = ValidationOutcome.Skipped;
				foreach(ValidationOutcome outcome in GroupedResults.Keys) {
					if(GroupedResults[outcome].DisplayableValidationResultItems.Count > 0 && outcome > result) {
						result = outcome;
					}
				}
				return result;
			}
		}
		public IDictionary<ValidationOutcome, ContextValidationResult> GroupedResults { get; private set; }
	}
	[DomainComponent]
	[SuppressToolBar]
	public class ContextValidationResult {
		public const string BrokenRulesMemberName = "BrokenRules";
		private string contextId;
		private IModelApplication modelApplication;
		private ValidationOutcome validationOutcome;
		private ValidationOutcome? outcomeToCollect;
		private string brokenRules;
		private List<DisplayableValidationResultItem> displayableValidationResultItems;
		public ContextValidationResult(string contextId, RuleSetValidationResult ruleSetValidationResult, IModelApplication modelApplication)
			: this(contextId, ruleSetValidationResult, modelApplication, null) {
		}
		public ContextValidationResult(string contextId, RuleSetValidationResult ruleSetValidationResult, IModelApplication modelApplication, ValidationOutcome? outcomeToCollect) {
			this.contextId = contextId;
			this.modelApplication = modelApplication;
			this.outcomeToCollect = outcomeToCollect;
			validationOutcome = ValidationOutcome.Skipped;
			List<string> errorMessages = new List<string>();
			displayableValidationResultItems = new List<DisplayableValidationResultItem>();
			foreach(RuleSetValidationResultItem resultItem in ruleSetValidationResult.GetResultsForContext(contextId)) {
				if(outcomeToCollect != null && resultItem.ValidationOutcome != outcomeToCollect.Value) {
					continue;
				}
				displayableValidationResultItems.Add(new DisplayableValidationResultItem(resultItem, modelApplication));
				if(resultItem.ValidationOutcome > validationOutcome) {
					validationOutcome = resultItem.ValidationOutcome;
				}
				if(resultItem.ValidationOutcome > ValidationOutcome.Valid) {
					if(!errorMessages.Contains(resultItem.ErrorMessage)) {
						errorMessages.Add(resultItem.ErrorMessage);
					}
				}
			}
			errorMessages.Sort();
			brokenRules = string.Join("\r\n", errorMessages.ToArray());
		}
		public string Context {
			get { return ValidationModule.GetContextCaption(contextId, modelApplication); }
		}
		[Browsable(false)] 
		public IsValid State {
			get { return validationOutcome > ValidationOutcome.Warning ? IsValid.Invalid : IsValid.Valid; }
		}
		[DisplayName("State")]
		public ValidationOutcome ContextValidationOutcome {
			get { return outcomeToCollect.HasValue ? outcomeToCollect.Value : validationOutcome; }
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[EditorAlias("Memo")]
		public string BrokenRules {
			get { return brokenRules; }
		}
		public ReadOnlyCollection<DisplayableValidationResultItem> DisplayableValidationResultItems {
			get { return displayableValidationResultItems.AsReadOnly(); }
		}
	}
	[DomainComponent]
	public class ValidationResults {
		private List<ContextValidationResult> items;
		private bool highlightErrors;
		private RuleSetValidationResult ruleSetValidationResult;
		private IModelApplication modelApplication;
		private List<DisplayableValidationResultItem> displayableValidationResultItems;
		public ValidationResults(RuleSetValidationResult ruleSetValidationResult, IModelApplication modelApplication) {
			this.ruleSetValidationResult = ruleSetValidationResult;
			this.modelApplication = modelApplication;
			highlightErrors = true;
		}
		[EditorAlias(EditorAliases.ListPropertyEditor)]
		[CollectionSourceMode(CollectionSourceMode.Normal)]
		public IList<ContextValidationResult> Results {
			get {
				if(items == null) {
					items = new List<ContextValidationResult>(ruleSetValidationResult.ContextIDs.Count);
					foreach(string contextId in ruleSetValidationResult.ContextIDs) {
						ContextValidationResultSet contextResultSet = new ContextValidationResultSet(contextId, ruleSetValidationResult, modelApplication);
						if(contextResultSet.OverallContextOutcome < ValidationOutcome.Information) {
							items.Add(contextResultSet.GroupedResults[ValidationOutcome.Valid]);
						}
						else {
							foreach(ValidationOutcome outcome in contextResultSet.GroupedResults.Keys) {
								if(outcome > ValidationOutcome.Valid && contextResultSet.GroupedResults[outcome].DisplayableValidationResultItems.Count > 0) {
									items.Add(contextResultSet.GroupedResults[outcome]);
								}
							}
						}
					}
				}
				return items.AsReadOnly();
			}
		}
		[DisplayName("Highlight Problems")]
		public bool HighlightErrors {
			get { return highlightErrors; }
			set { highlightErrors = value; }
		}
		[Browsable(false)]
		public RuleSetValidationResult RuleSetValidationResult {
			get { return ruleSetValidationResult; }
		}
		public ReadOnlyCollection<DisplayableValidationResultItem> DisplayableValidationResultItems {
			get {
				if(displayableValidationResultItems == null) {
					displayableValidationResultItems = new List<DisplayableValidationResultItem>();
					foreach(RuleSetValidationResultItem resultItem in ruleSetValidationResult.Results) {
						if(resultItem.ValidationOutcome > ValidationOutcome.Valid) {
							displayableValidationResultItems.Add(new DisplayableValidationResultItem(resultItem, modelApplication));
						}
					}
				}
				return displayableValidationResultItems.AsReadOnly();
			}
		}
	}
}
