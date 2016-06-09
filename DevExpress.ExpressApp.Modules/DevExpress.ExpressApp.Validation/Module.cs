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
using System.Drawing;
using System.Security;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation {
	public class CustomizeModelRulesEventArgs : EventArgs { 
		private List<IRule> rules;
		private List<IRuleSource> ruleSources;
		public CustomizeModelRulesEventArgs(List<IRule> rules, List<IRuleSource> ruleSources) {
			this.rules = rules;
			this.ruleSources = ruleSources;
		}
		public List<IRule> Rules {
			get { return rules; }
		}
		public List<IRuleSource> RuleSources {
			get { return ruleSources; }
		}
	}
	public class CustomizeRulesEventArgs : EventArgs {
		private List<IRule> rules;
		private XafApplication application;
		private List<IRuleSource> ruleSources;
		public CustomizeRulesEventArgs(XafApplication application, List<IRule> rules, List<IRuleSource> ruleSources) {
			this.rules = rules;
			this.application = application;
			this.ruleSources = ruleSources;
		}
		public List<IRule> Rules {
			get { return rules; }
		}
		public XafApplication Application {
			get { return application; }
		}
		public List<IRuleSource> RuleSources {
			get { return ruleSources; }
		}
	}
	public class CustomizeApplicationRuntimeRulesEventArgs : CustomizeRulesEventArgs {
		private IObjectSpaceProvider objectSpaceProvider;
		private RuleSet applicationModelRuleSet;
		public CustomizeApplicationRuntimeRulesEventArgs(XafApplication application, List<IRule> rules, List<IRuleSource> ruleSources, IObjectSpaceProvider objectSpaceProvider, RuleSet applicationModelRuleSet)
			: base(application, rules, ruleSources) {
			this.objectSpaceProvider = objectSpaceProvider;
			this.applicationModelRuleSet = applicationModelRuleSet;
		}
		public IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProvider; }
		}
		public RuleSet ApplicationModelRuleSet {
			get { return applicationModelRuleSet; }
		}
	}
	public class RuleSetInitializedEventArgs : EventArgs {
		private RuleSet ruleSet;
		public RuleSetInitializedEventArgs(RuleSet ruleSet) {
			this.ruleSet = ruleSet;
		}
		public RuleSet RuleSet {
			get { return ruleSet; }
		}
	}
	public class CustomizeRegisteredRuleTypesEventArgs : EventArgs {
		IList<Type> ruleTypes;
		public CustomizeRegisteredRuleTypesEventArgs(IList<Type> ruleTypes) {
			this.ruleTypes = ruleTypes;
		}
		public IList<Type> RuleTypes {
			get { return ruleTypes; }
		}
	}
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes a powerful and flexible validation engine and a large number of ready-to-use validation rule types. Allows you to create rules both at runtime or design time and check them when required.")]
	[ToolboxBitmap(typeof(ValidationModule), "Resources.Toolbox_Module_Validation.ico")]
	public sealed class ValidationModule : ModuleBase {
		private bool allowValidationDetailsAccess = true;
		private bool ignoreWarningAndInformationRules = false;
		private readonly Dictionary<Type, Type> registeredRules = new Dictionary<Type, Type>();
		private void Application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
			if(EnableControllersOnLogonWindow) {
				foreach(Type controllerType in GetDeclaredControllerTypes()) {
					e.Controllers.Add((Controller)DevExpress.Persistent.Base.ReflectionHelper.CreateObject(controllerType));
				}
			}
		}
		private RuleSet CollectApplicationRuntimeRules(RuleSet applicationModelRuleSet) {
			RuleSet tempRuleSet = new RuleSet();
			foreach(IModelClass modelClass in Application.Model.BOModel) {
				if(modelClass.TypeInfo != null) {
					IRuleSource ruleSource = null;
					if(PersistentObjectRuleSource.TryCreateRuleSource(Application.ObjectSpaceProvider, modelClass.TypeInfo.Type, out ruleSource)) {
						tempRuleSet.RegisteredSources.Add(ruleSource);
					}
					ruleSource = null;
					if(PersistentContainerRuleSource.TryCreateRuleSource(Application.ObjectSpaceProvider, modelClass.TypeInfo.Type, out ruleSource)) {
						tempRuleSet.RegisteredSources.Add(ruleSource);
					}
				}
			}
			CustomizeApplicationRuntimeRulesEventArgs args = new CustomizeApplicationRuntimeRulesEventArgs(
				Application, new List<IRule>(tempRuleSet.RegisteredRules), new List<IRuleSource>(tempRuleSet.RegisteredSources), Application.ObjectSpaceProvider, applicationModelRuleSet);
			if(CustomizeApplicationRuntimeRules != null) {
				CustomizeApplicationRuntimeRules(this, args);
			}
			return new RuleSet(args.Rules, args.RuleSources);
		}
		private void RegisterRuleTypes() {			  
			RegisterRule(typeof(RuleRequiredField), typeof(IRuleRequiredFieldProperties));
			RegisterRule(typeof(RuleFromBoolProperty), typeof(IRuleFromBoolPropertyProperties));
			RegisterRule(typeof(RuleRange), typeof(IRuleRangeProperties));
			RegisterRule(typeof(RuleValueComparison), typeof(IRuleValueComparisonProperties));
			RegisterRule(typeof(RuleStringComparison), typeof(IRuleStringComparisonProperties));
			RegisterRule(typeof(RuleRegularExpression), typeof(IRuleRegularExpressionProperties));
			RegisterRule(typeof(RuleCriteria), typeof(IRuleCriteriaProperties));
			RegisterRule(typeof(RuleObjectExists), typeof(IRuleObjectExistsProperties));
			RegisterRule(typeof(RuleUniqueValue), typeof(IRuleUniqueValueProperties));
			RegisterRule(typeof(RuleIsReferenced), typeof(IRuleIsReferencedProperties));
			RegisterRule(typeof(RuleCombinationOfPropertiesIsUnique), typeof(IRuleCombinationOfPropertiesIsUniqueProperties));
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelActionValidationContexts),
			typeof(IModelApplicationValidation),
			typeof(IModelValidation),
			typeof(IModelValidationRules),
			typeof(IModelRuleBase),
			typeof(IModelValidationContexts),
			typeof(IModelValidationContext),
			typeof(RuleObjectExistsPropertiesLogic),
			typeof(RulePropertyValuePropertiesLogic),
			typeof(RuleCollectionPropertyPropertiesLogic),
			typeof(RuleValueComparisonPropertiesLogic),
			typeof(RuleRangePropertiesLogic),
			typeof(RuleBasePropertiesLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(RuleSetValidationResult),
				typeof(RuleSetValidationResultItem),
				typeof(RuleSetValidationResultItemAggregate),
				typeof(DisplayableValidationResultItem),
				typeof(ContextValidationResult),
				typeof(ValidationResults),
				typeof(RulePropertyValueProperties),
				typeof(RuleRequiredFieldProperties),
				typeof(RuleFromBoolPropertyProperties),
				typeof(RuleRangeProperties),
				typeof(RuleValueComparisonProperties),
				typeof(RuleStringComparisonProperties),
				typeof(RuleRegularExpressionProperties),
				typeof(RuleCriteriaProperties),
				typeof(RuleSearchObjectProperties),
				typeof(RuleObjectExistsProperties),
				typeof(RuleUniqueValueProperties),
				typeof(RuleIsReferencedProperties),
				typeof(RuleCombinationOfPropertiesIsUniqueProperties)
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(ActionValidationController),
				typeof(PersistenceValidationController),
				typeof(ResultsHighlightController),
				typeof(RuleSetInitializationController),
				typeof(CheckContextsAvailableController),
				typeof(PreventValidationDetailsAccessController),
				typeof(ShowAllContextsController),
				typeof(GridClientValidationController),
				typeof(DevExpress.ExpressApp.Validation.DiagnosticViews.ShowRulesController)
			};
		}
		static ValidationModule() {
			EnableControllersOnLogonWindow = true;
		}
		public ValidationModule() {
			ValidationExceptionResourceLocalizer.Register(typeof(ValidationExceptionResourceLocalizer));
			RegisterRuleTypes();
		}
		protected override void Dispose(bool disposing) {
			if(Application != null) {
				Application.CreateCustomLogonWindowControllers -= new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
			}
			base.Dispose(disposing);
		}
		private Type GetRuleTypeAssociatedWithRuleNode(IModelRuleBase ruleNode) {
			ITypesInfo typesInfo = XafTypesInfo.Instance;
			DefaultValueAttribute attribute = typesInfo.FindTypeInfo(((ModelNode)ruleNode).NodeInfo.BaseInterface).FindAttribute<DefaultValueAttribute>(true);
			return typesInfo.FindTypeInfo((string)attribute.Value).Type;
		}
		private void CollectRulesFromModel(IModelApplicationValidation validationModel) {
			if(validationModel != null && validationModel.Validation.Rules.Count > 0) {
				foreach(IModelRuleBase ruleModelNode in validationModel.Validation.Rules) {
					Type ruleType = GetRuleTypeAssociatedWithRuleNode(ruleModelNode);
					IRule rule = (IRule)TypeHelper.CreateInstance(ruleType, ruleModelNode);
					Validator.RuleSet.RegisteredRules.Add(rule);
				}
			}
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelApplication, IModelApplicationValidation>();
			extenders.Add<IModelLayoutManagerOptions, IModelLayoutManagerOptionsValidation>();
			try {
				ValidationModelUpdater validationModelUpdater = new ValidationModelUpdater(extenders);
				foreach(KeyValuePair<Type, Type> item in registeredRules) {
					validationModelUpdater.RegisterRule(item.Key, item.Value);
				}
			}
			catch(SecurityException) {
			}
		}
		public void InitializeRuleSet() {
			Guard.ArgumentNotNull(Validator.RuleSet, "Validator.RuleSet"); 
			Validator.RuleSet.Clear();
			Validator.RuleSet.IgnoreWarningAndInformationRules = IgnoreWarningAndInformationRules;
			Validator.RuleSet.EnableDelayedRuleRegistration = false;
			if(Application == null || Application.Model == null) {
				return;
			}
			CollectRulesFromModel(Application.Model as IModelApplicationValidation);		  
			RuleSet ruleSet = CollectApplicationRuntimeRules(Validator.RuleSet);
			Validator.RuleSet.RegisteredRules.AddRange(ruleSet.RegisteredRules);
			Validator.RuleSet.RegisteredSources.AddRange(ruleSet.RegisteredSources);
			if(RuleSetInitialized != null) {
				RuleSetInitialized(this, new RuleSetInitializedEventArgs(Validator.RuleSet));
			}
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.IgnoredExceptions.Add(typeof(ValidationException));
			application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		public static string GetContextCaption(string contextId, IModelApplication modelApplication) {
			string result = contextId;
			if(modelApplication is IModelApplicationValidation) {
				IModelValidationContext context = ((IModelApplicationValidation)modelApplication).Validation.Contexts[contextId];
				if(context != null) {
					result = context.Caption;
				}
			}
			else {
				IModelAction action = modelApplication.ActionDesign.Actions[contextId];
				if(action != null) {
					result = action.Caption;
				}
			}
			return result;
		}
		public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			ITypeInfo ruleBase = typesInfo.FindTypeInfo(typeof(RuleBaseProperties));
			foreach(ITypeInfo typeInfo in ReflectionHelper.FindTypeDescendants(ruleBase)) {
				if(typeInfo.FindAttribute<DomainComponentAttribute>() == null) {
					typeInfo.AddAttribute(new DomainComponentAttribute());
				}
			}
		}
		internal void RegisterRule(Type ruleType, Type ruleProperties) {
			Guard.ArgumentNotNull(ruleType, "ruleType");
			Guard.ArgumentNotNull(ruleProperties, "ruleProperties");
			XafTypesInfo.Instance.FindTypeInfo(ruleType); 
			registeredRules[ruleType] = ruleProperties;
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationModuleAllowValidationDetailsAccess")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Settings")]
		public bool AllowValidationDetailsAccess {
			get { return allowValidationDetailsAccess; }
			set { allowValidationDetailsAccess = value; }
		}
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationModuleIgnoreWarningAndInformationRules")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Settings")]
		public bool IgnoreWarningAndInformationRules {
			get { return ignoreWarningAndInformationRules; }
			set { ignoreWarningAndInformationRules = value; }
		}
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationModuleCustomizeApplicationRuntimeRules")]
#endif
		public event EventHandler<CustomizeApplicationRuntimeRulesEventArgs> CustomizeApplicationRuntimeRules;
#if !SL
	[DevExpressExpressAppValidationLocalizedDescription("ValidationModuleRuleSetInitialized")]
#endif
		public event EventHandler<RuleSetInitializedEventArgs> RuleSetInitialized;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public static bool EnableControllersOnLogonWindow { get; set; }
#if DebugTest
		public Type DebugTest_GetRuleTypeAssociatedWithRuleNode(IModelRuleBase ruleNode) {
			return GetRuleTypeAssociatedWithRuleNode(ruleNode);
		}
#endif
	}
	public class PersistentObjectRuleSource : IRuleSource {
		private static readonly Type ruleInterface = typeof(IRule);
		private IObjectSpaceProvider objectSpaceProvider;
		private Type classType;
		public static bool TryCreateRuleSource(IObjectSpaceProvider objectSpaceProvider, Type ruleClass, out IRuleSource ruleSource) {
			ruleSource = null;
			if(ruleInterface.IsAssignableFrom(ruleClass) && !ruleInterface.IsAssignableFrom(ruleClass.BaseType)) {
				ruleSource = new PersistentObjectRuleSource(objectSpaceProvider, ruleClass);
				return true;
			}
			return false;
		}
		protected PersistentObjectRuleSource(IObjectSpaceProvider objectSpaceProvider, Type classType) {
			this.objectSpaceProvider = objectSpaceProvider;
			this.classType = classType;
		}
		public ICollection<IRule> CreateRules() {
			using(IObjectSpace space = objectSpaceProvider.CreateObjectSpace()) {
				List<IRule> result = new List<IRule>();
				foreach(IRule rule in space.CreateCollection(classType)) {
					result.Add(rule);
				}
				return result;
			}
		}
		public string Name {
			get { return string.Format("Persistent class {0} (assembly {1})", classType.Name, classType.Assembly.GetName().Name); }
		}
	}
	public class PersistentContainerRuleSource : IRuleSource {
		private static readonly Type sourceInterface = typeof(IRuleSource);
		private IObjectSpaceProvider objectSpaceProvider;
		private Type classType;
		public static bool TryCreateRuleSource(IObjectSpaceProvider objectSpaceProvider, Type ruleSourceClass, out IRuleSource ruleSource) {
			ruleSource = null;
			if(sourceInterface.IsAssignableFrom(ruleSourceClass) && !sourceInterface.IsAssignableFrom(ruleSourceClass.BaseType)) {
				ruleSource = new PersistentContainerRuleSource(objectSpaceProvider, ruleSourceClass);
				return true;
			}
			return false;
		}
		protected PersistentContainerRuleSource(IObjectSpaceProvider objectSpaceProvider, Type classType) {
			this.objectSpaceProvider = objectSpaceProvider;
			this.classType = classType;
		}
		public ICollection<IRule> CreateRules() {
			using(IObjectSpace space = objectSpaceProvider.CreateObjectSpace()) {
				RuleSet result = new RuleSet();
				foreach(IRuleSource ruleSource in space.CreateCollection(classType)) {
					result.RegisteredSources.Add(ruleSource);
				}
				return result.GetRules();
			}
		}
		public string Name {
			get { return string.Format("Persistent class {0} (assembly {1})", classType.Name, classType.Assembly.GetName().Name); }
		}
	}
}
