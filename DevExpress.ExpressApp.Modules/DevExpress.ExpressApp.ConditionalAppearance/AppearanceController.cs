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
using System.Diagnostics.CodeAnalysis;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public interface ISupportRefreshItemsAppearance {
		void RefreshViewItemsAppearance();
	}
	public class CollectAppearanceEventArgs : EventArgs {
		private string name;
		private AppearanceObject appearanceObject;
		public CollectAppearanceEventArgs(string name, AppearanceObject appearanceObject) {
			this.name = name;
			this.appearanceObject = appearanceObject;
		}
		public string Name {
			get { return name; }
		}
		public AppearanceObject AppearanceObject {
			get {
				return appearanceObject;
			}
		}
	}
	public class CollectAppearanceRulesEventArgs : EventArgs {
		private string name;
		public IViewInfo ViewInfo { get; private set; }
		private List<IAppearanceRuleProperties> appearanceRules;
		public CollectAppearanceRulesEventArgs(string name, IViewInfo context, List<IAppearanceRuleProperties> appearanceRules) {
			this.name = name;
			this.ViewInfo = context;
			this.appearanceRules = appearanceRules;
		}
		public string Name {
			get { return name; }
		}
		public List<IAppearanceRuleProperties> AppearanceRules {
			get { return appearanceRules; }
		}
		#region obsolete 14.2
		[Obsolete("Use the ViewInfo property instead", true)]
		public View ContextView {
			get { return null; }
		}
		#endregion
	}
	public class CustomCollectAllAppearanceRulePropertiesEventArgs : EventArgs {
		public CustomCollectAllAppearanceRulePropertiesEventArgs() {
		}
		public IEnumerable<IAppearanceRuleProperties> AppearanceRules { get; set; }
	}
	public class CustomCreateAppearanceRuleEventArgs : EventArgs {
		public CustomCreateAppearanceRuleEventArgs(IAppearanceRuleProperties ruleProperties, IObjectSpace objectSpace) {
			this.RuleProperties = ruleProperties;
			this.ObjectSpace = objectSpace;
		}
		public IObjectSpace ObjectSpace { get; private set; }
		public IAppearanceRuleProperties RuleProperties { get; private set; }
		public AppearanceRule Rule { get; set; }
	}
	public class CustomGetIsRulePropertiesEmptyEventArgs : EventArgs {
		public CustomGetIsRulePropertiesEmptyEventArgs(IAppearanceRuleProperties ruleProperties, bool isEmpty) {
			this.RuleProperties = ruleProperties;
			this.IsEmpty = isEmpty;
		}
		public bool IsEmpty { get; set; }
		public IAppearanceRuleProperties RuleProperties { get; private set; }
	}
	public class ApplyAppearanceEventArgs : HandledEventArgs {
		public ApplyAppearanceEventArgs(AppearanceObject appearanceObject, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor, List<IConditionalAppearanceItem> appearanceItems, IViewInfo view)
			: base(false) {
			this.AppearanceObject = appearanceObject;
			this.ItemType = itemType;
			this.ItemName = itemName;
			this.Item = item;
			this.ContextObjects = contextObjects;
			this.EvaluatorContextDescriptor = evaluatorContextDescriptor;
			this.AppearanceItems = appearanceItems;
			this.ViewInfo = view;
		}
		public AppearanceObject AppearanceObject { get; private set; }
		public string ItemType { get; private set; }
		public string ItemName { get; private set; }
		public object Item { get; private set; }
		public object[] ContextObjects { get; private set; }
		public EvaluatorContextDescriptor EvaluatorContextDescriptor { get; private set; }
		public List<IConditionalAppearanceItem> AppearanceItems { get; private set; }
		public IViewInfo ViewInfo { get; private set; }
		#region obsolete 14.2
		[Obsolete("Use the ViewInfo property instead", true)]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public View View { get; private set; }
		[Obsolete("Use the 'ApplyAppearanceEventArgs(AppearanceObject appearanceObject, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor, List<IConditionalAppearanceItem> appearanceItems, ViewInfo view)' method instead", true)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public ApplyAppearanceEventArgs(AppearanceObject appearanceObject, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor, List<IConditionalAppearanceItem> appearanceItems)
			: base(false) {
		}
		[Obsolete("Use the 'ApplyAppearanceEventArgs(AppearanceObject appearanceObject, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor, List<IConditionalAppearanceItem> appearanceItems, ViewInfo view)' method instead", true)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public ApplyAppearanceEventArgs(AppearanceObject appearanceObject, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor, List<IConditionalAppearanceItem> appearanceItems, View view)
			: base(false) {
		}
		#endregion
	}
	public class AppearanceController : ViewController<ObjectView> {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceViewItemType = AppearanceItemType.ViewItem.ToString();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceActionType = AppearanceItemType.Action.ToString();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceLayoutItemType = AppearanceItemType.LayoutItem.ToString();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceContextAny = AppearanceContext.Any.ToString();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceContextDetailView = AppearanceContext.DetailView.ToString();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public readonly static string AppearanceContextListView = AppearanceContext.ListView.ToString();
		private List<ISupportRefreshItemsAppearance> controllers = new List<ISupportRefreshItemsAppearance>();
		private Dictionary<string, List<AppearanceRule>> appearanceRules = new Dictionary<string, List<AppearanceRule>>();
		private Dictionary<IAppearanceRuleProperties, bool> isRuleEmptyCash = new Dictionary<IAppearanceRuleProperties, bool>();
		private IEnumerable<IAppearanceRuleProperties> allRulesCash;
		private Dictionary<IAppearanceRuleProperties, AppearanceRule> appearanceRuleCash = new Dictionary<IAppearanceRuleProperties, AppearanceRule>();
		private void RefreshItemAppearanceCore(IViewInfo view, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			if(LockCount == 0) {
				if(contextObjects != null && View != null) {
					foreach(object contextObject in contextObjects) {
						if(View.ObjectSpace.IsDisposedObject(contextObject)) {
							return;
						}
					}
				}
				List<AppearanceRule> itemAppearanceRules = GetAppearanceRules(view, itemType, itemName, item);
				List<IConditionalAppearanceItem> appearanceItems = new List<IConditionalAppearanceItem>();
				foreach(AppearanceRule appearanceRule in itemAppearanceRules) {
					appearanceItems.AddRange(appearanceRule.Validate(contextObjects, evaluatorContextDescriptor));
				}
				AppearanceObject appearanceObject = CombineAppearanceResults(appearanceItems);
				ApplyAppearanceEventArgs applyAppearanceEventArgs = new ApplyAppearanceEventArgs(appearanceObject, itemType, itemName, item, contextObjects, evaluatorContextDescriptor, appearanceItems, view);
				OnCustomApplyAppearance(applyAppearanceEventArgs);
				if(!applyAppearanceEventArgs.Handled) {
					ApplyAppearance(item, applyAppearanceEventArgs.AppearanceObject);
				}
				OnAppearanceApplied(applyAppearanceEventArgs);
			}
		}
		private bool IsRulePropertiesEmpty(IAppearanceRuleProperties ruleProperties) {
			bool result = true;
			if(!isRuleEmptyCash.TryGetValue(ruleProperties, out result)) {
				result = !ruleProperties.BackColor.HasValue && !ruleProperties.FontColor.HasValue &&
					!ruleProperties.Enabled.HasValue && !ruleProperties.FontStyle.HasValue &&
					!ruleProperties.Visibility.HasValue;
				CustomGetIsRulePropertiesEmptyEventArgs args = new CustomGetIsRulePropertiesEmptyEventArgs(ruleProperties, result);
				if(CustomGetIsRulePropertiesEmpty != null) {
					CustomGetIsRulePropertiesEmpty(this, args);
				}
				isRuleEmptyCash[ruleProperties] = args.IsEmpty;
				result = args.IsEmpty;
			}
			return result;
		}
		private bool IsRuleFitToItem(AppearanceRule ruleProperties, string itemName, string itemType) {
			if(string.IsNullOrEmpty(itemName)) return true;
			bool ruleFitToTargetItem = ruleProperties.TargetItems.Contains(itemName);
			if(ruleProperties.TargetItems.Contains(AppearanceRule.SelectAllString) && itemType != AppearanceController.AppearanceActionType) {
				ruleFitToTargetItem = !ruleFitToTargetItem;
			}
			return ruleFitToTargetItem;
		}
#if DebugTest
		public bool DebugTest_IsRulePropertiesEmpty(IAppearanceRuleProperties ruleProperties) {
			return IsRulePropertiesEmpty(ruleProperties);
		}
		public bool DebugTest_IsRuleFitToItem(AppearanceRule ruleProperties, string itemName, string itemType) {
			return IsRuleFitToItem(ruleProperties, itemName, itemType);
		}
		public bool DebugTest_IsRuleFitToContext(ObjectView view, IAppearanceRuleProperties ruleProperties) {
			AppearanceRule rule = new AppearanceRule(ruleProperties, null);
			return IsRuleFitToContext(rule, GetCurrentAppearanceContexts(ViewInfo.FromView(view)));
		}
		public bool DebugTest_IsRuleFitToItem(IAppearanceRuleProperties ruleProperties, string itemName, string itemType) {
			AppearanceRule rule = new AppearanceRule(ruleProperties, null);
			return IsRuleFitToItem(rule, itemName, itemType);
		}
#endif
		private bool IsRuleFitToContext(AppearanceRule ruleProperties, IEnumerable<string> contexts) {
			if(ruleProperties.TargetContexts.Count == 0) {
				return true;
			}
			bool ruleFitToCurrentContext = false;
			foreach(string contextItem in contexts) {
				if(ruleProperties.TargetContexts.Contains(contextItem)) {
					ruleFitToCurrentContext = true;
					break;
				}
			}
			return ruleProperties.TargetContexts.Contains(AppearanceController.AppearanceContextAny) ? !ruleFitToCurrentContext : ruleFitToCurrentContext;
		}
		private List<string> GetCurrentAppearanceContexts(IViewInfo view) {
			List<string> result = new List<string>();
			result.Add(view.ViewId);
			if(typeof(DetailView).IsAssignableFrom(view.ViewType)) {
				result.Add(AppearanceController.AppearanceContextDetailView);
			}
			else {
				result.Add(AppearanceController.AppearanceContextListView);
			}
			return result;
		}
		[Browsable(false)]
		public static List<IAppearanceRuleProperties> GetRulesFromModel(IModelClass classModel) {
			List<IAppearanceRuleProperties> allRules = new List<IAppearanceRuleProperties>();
			foreach(IModelAppearanceRule appearanceRule in ((IModelConditionalAppearance)(classModel)).AppearanceRules) {
				allRules.Add(appearanceRule);
			}
			if(classModel.BaseClass != null) {
				allRules.AddRange(GetRulesFromModel(classModel.BaseClass));
			}
			else if(classModel.TypeInfo != null && classModel.TypeInfo.IsDomainComponent && classModel.TypeInfo.IsInterface) {
				List<string> usedRulesIds = new List<string>();
				foreach(IAppearanceRuleProperties appearanceRule in allRules) {
					usedRulesIds.Add(((DevExpress.ExpressApp.Model.Core.ModelNode)appearanceRule).Id);
				}
				foreach(IModelMember member in classModel.AllMembers) {
					IModelClass memberClass = classModel.Application.BOModel[member.MemberInfo.Owner.FullName];
					if(memberClass != null) {
						foreach(IModelAppearanceRule appearanceRule in ((IModelConditionalAppearance)(memberClass)).AppearanceRules) {
							string id = ((DevExpress.ExpressApp.Model.Core.ModelNode)appearanceRule).Id;
							if(!usedRulesIds.Contains(id)) {
								allRules.Add(appearanceRule);
								usedRulesIds.Add(id);
							}
						}
					}
				}
			}
			return allRules;
		}
		internal IEnumerable<string> GetTargetedActions(ObjectView view) {
			List<string> actionsMentionedInRules = new List<string>();
			foreach(AppearanceRule appearanceRule in GetAppearanceRules(ViewInfo.FromView(view), AppearanceController.AppearanceActionType, null, null)) {
				actionsMentionedInRules.AddRange(appearanceRule.TargetItems);
			}
			return actionsMentionedInRules.ToArray();
		}
		private void EnsureAllRulesCash(IViewInfo view) {
			if(allRulesCash == null) {
				CustomCollectAllAppearanceRulePropertiesEventArgs customCollectAllAppearanceRulesEventArgs = new CustomCollectAllAppearanceRulePropertiesEventArgs();
				if(CustomCollectAllAppearanceRuleProperties != null) {
					CustomCollectAllAppearanceRuleProperties(this, customCollectAllAppearanceRulesEventArgs);
				}
				if(customCollectAllAppearanceRulesEventArgs.AppearanceRules != null) {
					allRulesCash = customCollectAllAppearanceRulesEventArgs.AppearanceRules;
				}
				else {
					allRulesCash = GetRulesFromModel(view.ModelClass);
				}
			}
		}
		private List<IAppearanceRuleProperties> GetAllAppearanceRules(IViewInfo view, string itemName) {
			EnsureAllRulesCash(view);
			return CustomCollectAppearanceRules(view, itemName);
		}
		private List<AppearanceRule> GetAppearanceRules(View view, string itemType, string itemName, object item) {
			return GetAppearanceRules(ViewInfo.FromView(view as ObjectView), itemType, itemName, item);
		}
#if DebugTest
		public List<AppearanceRule> DebugTest_GetAppearanceRules(View view, string itemType, string itemName, object item) {
			return GetAppearanceRules(view, itemType, itemName, item);
		}
		public List<AppearanceRule> DebugTest_GetAppearanceRules(IViewInfo view, string itemType, string itemName, object item) { 
			return GetAppearanceRules(view, itemType, itemName, item) ;
		}
		public AppearanceObject DebugTest_CombineAppearanceResults(IList<IConditionalAppearanceItem> appearance) {
			return CombineAppearanceResults(appearance);
		}
		public void DebugTest_ApplyAppearance(object targetItem, AppearanceObject appearance) {
			ApplyAppearance(targetItem, appearance);
		}
#endif
		private List<AppearanceRule> GetAppearanceRules(IViewInfo view, string itemType, string itemName, object item) {
			List<AppearanceRule> rulesForProperty = null;
			string appearanceItemId = null;
			if(!string.IsNullOrEmpty(itemName)) {
				appearanceItemId = itemType.ToString() + "." + itemName;
				appearanceRules.TryGetValue(appearanceItemId, out rulesForProperty);
			}
			if(rulesForProperty == null) {
				rulesForProperty = new List<AppearanceRule>();
				List<IAppearanceRuleProperties> collectedAppearanceRules = GetAllAppearanceRules(view, itemName);
				if(collectedAppearanceRules.Count > 0) {
					IEnumerable<string> contexts = GetCurrentAppearanceContexts(view);
					foreach(IAppearanceRuleProperties modelRule in collectedAppearanceRules) {
						if(itemType == modelRule.AppearanceItemType) {
							AppearanceRule rule = null;
							if(!appearanceRuleCash.TryGetValue(modelRule, out rule)) {
								CustomCreateAppearanceRuleEventArgs customCreateAppearanceRuleEventArgs = new CustomCreateAppearanceRuleEventArgs(modelRule, view.ObjectSpace);
								if(CustomCreateAppearanceRule != null) {
									CustomCreateAppearanceRule(this, customCreateAppearanceRuleEventArgs);
								}
								if(customCreateAppearanceRuleEventArgs.Rule != null) {
									rule = customCreateAppearanceRuleEventArgs.Rule;
								}
								else {
									rule = new AppearanceRule(new CachedAppearanceRuleProperties(modelRule), view.ObjectSpace);
								}
								appearanceRuleCash[modelRule] = rule;
							}
							if(!IsRulePropertiesEmpty(rule.Properties) && IsRuleFitToItem(rule, itemName, itemType) && IsRuleFitToContext(rule, contexts)) {
								rulesForProperty.Add(rule);
							}
						}
					}
				}
				if(!string.IsNullOrEmpty(appearanceItemId)) {
					appearanceRules[appearanceItemId] = rulesForProperty;
				}
			}
			if(string.Equals(itemType, AppearanceController.AppearanceActionType)) {  
				return rulesForProperty;
			}
			else {
				return AppearanceRulesFilterHelper.FilterRules(view, item, rulesForProperty);
			}
		}
		private AppearanceObject CombineAppearanceResults(IList<IConditionalAppearanceItem> appearance) {
			Dictionary<Type, IConditionalAppearanceItem> appearanceResults = new Dictionary<Type, IConditionalAppearanceItem>();
			foreach(IConditionalAppearanceItem item in appearance) {
				IConditionalAppearanceItem collectedItem = null;
				if(appearanceResults.TryGetValue(item.GetType(), out collectedItem)) {
					if(item.State == AppearanceState.CustomValue) {
						if(item.IsCombineValue && collectedItem.State != AppearanceState.ResetValue) {
							collectedItem.CombineValue(item);
						}
						else {
							if(collectedItem.State == AppearanceState.ResetValue || item.Priority > collectedItem.Priority) {
								appearanceResults[item.GetType()] = item;
							}
						}
					}
				}
				else {
					appearanceResults[item.GetType()] = item;
				}
			}
			AppearanceObject result = new AppearanceObject(new List<IConditionalAppearanceItem>(appearanceResults.Values));
			return result;
		}
		private void ApplyAppearance(object targetItem, AppearanceObject appearance) {
			foreach(AppearanceItemBase appearanceItem in appearance.Items) {
				appearanceItem.Apply(targetItem);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ResetRulesCache(); 
		}
		protected override void OnDeactivated() {
			ResetRulesCache();
			base.OnDeactivated();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual List<IAppearanceRuleProperties> CustomCollectAppearanceRules(IViewInfo view, string itemName) {
			List<IAppearanceRuleProperties> allRules = new List<IAppearanceRuleProperties>(allRulesCash);
			if(CollectAppearanceRules != null) {
				CollectAppearanceRulesEventArgs collectAppearanceRulesEventArgs = new CollectAppearanceRulesEventArgs(itemName, view, allRules);
				OnCollectAppearanceRules(collectAppearanceRulesEventArgs);
				return collectAppearanceRulesEventArgs.AppearanceRules;
			}
			return allRules;
		}
		protected virtual void OnCollectAppearanceRules(CollectAppearanceRulesEventArgs ea) {
			if(CollectAppearanceRules != null)
				CollectAppearanceRules(this, ea);
		}
		protected virtual void OnCustomApplyAppearance(ApplyAppearanceEventArgs ea) {
			if(CustomApplyAppearance != null) {
				CustomApplyAppearance(this, ea);
			}
		}
		protected virtual void OnAppearanceApplied(ApplyAppearanceEventArgs ea) {
			if(AppearanceApplied != null) {
				AppearanceApplied(this, ea);
			}
		}
		public AppearanceController() {
			IsInRefresh = false;
			LockCount = 0;
		}
		public bool Refresh() {
			if(!Active || (LockCount > 0) || IsInRefresh) {
				return false;
			}
			ISupportUpdate supportUpdate = null;
			if(View.LayoutManager != null) {
				supportUpdate = View.LayoutManager.Container as ISupportUpdate;
			}
			try {
				IsInRefresh = true;
				if(supportUpdate != null) {
					supportUpdate.BeginUpdate();
				}
				foreach(ISupportRefreshItemsAppearance controller in controllers) {
					controller.RefreshViewItemsAppearance();
				}
			}
			finally {
				if(supportUpdate != null) {
					supportUpdate.EndUpdate();
				}
				IsInRefresh = false;
			}
			return true;
		}
		public void RefreshItemAppearance(View view, string itemType, string itemName, object item, object contextObject) {
#pragma warning disable 0618
			RefreshItemAppearance(view, itemType, itemName, item, contextObject, null);
#pragma warning restore 0618
		}
		public void RefreshItemAppearance(IViewInfo view, string itemType, string itemName, object item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			object[] contextObjects = contextObject != null ? new object[] { contextObject } : new object[] { };
			RefreshItemAppearance(view, itemType, itemName, item, contextObjects, evaluatorContextDescriptor);
		}
		[Obsolete("Use the RefreshItemAppearance(IViewInfo view, string itemType, string itemName, object item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) method instead")]
		public void RefreshItemAppearance(View view, string itemType, string itemName, object item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			RefreshItemAppearance(ViewInfo.FromView(view as ObjectView), itemType, itemName, item, contextObject, evaluatorContextDescriptor);
		}
		public virtual void RefreshItemAppearance(IViewInfo view, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			RefreshItemAppearanceCore(view, itemType, itemName, item, contextObjects, evaluatorContextDescriptor);
		}
		[Obsolete("Use the RefreshItemAppearance(IViewInfo view, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) method instead")]
		public virtual void RefreshItemAppearance(View view, string itemType, string itemName, object item, object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			RefreshItemAppearance(ViewInfo.FromView(view as ObjectView), itemType, itemName, item, contextObjects, evaluatorContextDescriptor);
		}
		public void ResetRulesCache() {
			foreach(List<AppearanceRule> rulesList in appearanceRules.Values) {
				rulesList.Clear();
			}
			appearanceRules.Clear();
			appearanceRuleCash.Clear();
			allRulesCash = null;
			foreach(ISupportRefreshItemsAppearance controller in controllers) {
				if(controller is ISupportResetCache) {
					((ISupportResetCache)controller).ResetCache();
				}
			}
		}
		public void RegisterController(ISupportRefreshItemsAppearance controller) {
			controllers.Add(controller);
			if(Active) {
				controller.RefreshViewItemsAppearance();
			}
		}
		public void UnRegisterController(ISupportRefreshItemsAppearance controller) {
			controllers.Remove(controller);
		}
		public void AppearanceBeginUpdate() {
			LockCount++;
		}
		public void AppearanceEndUpdate() {
			if(LockCount > 0) {
				LockCount--;
				if(LockCount == 0 && Active) {
					ResetRulesCache();
					Refresh();
				}
			}
		}
		[DefaultValue(false)]
		public bool IsInRefresh { get; private set; }
		[DefaultValue(0)]
		public int LockCount { get; private set; }
		public event EventHandler<CustomCollectAllAppearanceRulePropertiesEventArgs> CustomCollectAllAppearanceRuleProperties;
		public event EventHandler<CustomGetIsRulePropertiesEmptyEventArgs> CustomGetIsRulePropertiesEmpty;
		public event EventHandler<CustomCreateAppearanceRuleEventArgs> CustomCreateAppearanceRule;
		public event EventHandler<CollectAppearanceRulesEventArgs> CollectAppearanceRules;
		public event EventHandler<ApplyAppearanceEventArgs> CustomApplyAppearance;
		public event EventHandler<ApplyAppearanceEventArgs> AppearanceApplied;
#pragma warning disable 0067
		[Obsolete("Use 'CustomApplyAppearance' or 'AppearanceApplied' instead.", true)]
		public event EventHandler<CollectAppearanceEventArgs> CollectAppearance;
#pragma warning restore 0067
	}
}
