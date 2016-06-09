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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Workflow.DC {
	[DomainComponent]
	[XafDisplayName("Workflow Definition")]
	public interface IDCWorkflowDefinition : IWorkflowDefinition, ISupportIsActive, IWorkflowDefinitionSettings {
		[RuleRequiredField(DCWorkflowDefinitionLogic.RuleId_Name_RequiredField, DefaultContexts.Save)]
		[RuleUniqueValue(DCWorkflowDefinitionLogic.RuleId_Name_UniqueValue, DefaultContexts.Save)] 
		[Size(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		new string Name { get; set; }
		[Size(-1)]
		[VisibleInListView(false)]
		new string Xaml { get; set; }
		[RuleFromBoolProperty(DCWorkflowDefinitionLogic.RuleId_Xaml_ValidActivity_WhenIsActive, DefaultContexts.Save,
			UsedProperties = "Xaml", SkipNullOrEmptyValues = false, TargetCriteria = "[IsActive] = true")]
		[Browsable(false)]
		bool IsValidActivity { get; }
		[ValueConverter(typeof(TypeToStringConverter))]
		[RuleRequiredField(DCWorkflowDefinitionLogic.RuleId_TargetType_RequiredField_WhenIsActive, DefaultContexts.Save, TargetCriteria = "[IsActive] = true && ([AutoStartWhenObjectIsCreated] = 'True' || [AutoStartWhenObjectFitsCriteria] = 'True')")]
		new Type TargetObjectType { get; set; }
		[CriteriaOptions("TargetObjectType")]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		[Size(SizeAttribute.Unlimited)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionCriteriaAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false)]
		new string Criteria { get; set; }
		[VisibleInDetailView(false)]
		new bool IsActive { get; set; }
		[VisibleInDetailView(false)]
		new bool AutoStartWhenObjectIsCreated { get; set; }
		[VisibleInDetailView(false)]
		[ImmediatePostData]
		new bool AutoStartWhenObjectFitsCriteria { get; set; }
		[VisibleInDetailView(false)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionAllowMultipleRunsAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false, Visibility = ViewItemVisibility.Hide)]
		new bool AllowMultipleRuns { get; set; }
		[Browsable(false)]
		new bool CanCompile { get; }
		[Browsable(false)]
		new bool CanCompileForDesigner { get; }
		[Browsable(false)]
		new bool CanOpenHost { get; }
	}
	[DomainLogic(typeof(IDCWorkflowDefinition))]
	public static class DCWorkflowDefinitionLogic {
		public const string RuleId_Name_RequiredField = "{D6D02583-C317-4B75-B7A0-2E6988B01C79}";
		public const string RuleId_Name_UniqueValue = "{12C07BD5-2E45-43BD-B616-D33B48967257}";
		public const string RuleId_TargetType_RequiredField_WhenIsActive = "{0FC8E8B2-0BD5-4EEC-A0A0-5593BB139F12}";
		public const string RuleId_Xaml_ValidActivity_WhenIsActive = "{3D2C3E9B-58BB-4788-A9BD-F7550FE147A8}";
		public static bool Get_CanCompile(IDCWorkflowDefinition definition) {
			return PersistentWorkflowDefinitionCore.Get_CanCompile(definition);
		}
		public static bool Get_CanCompileForDesigner(IDCWorkflowDefinition definition) {
			return PersistentWorkflowDefinitionCore.Get_CanCompileForDesigner(definition);
		}
		public static bool Get_CanOpenHost(IDCWorkflowDefinition definition) {
			return PersistentWorkflowDefinitionCore.Get_CanOpenHost(definition);
		}
		public static string GetUniqueId(IDCWorkflowDefinition definition, IObjectSpace os) {
			return PersistentWorkflowDefinitionCore.GetUniqueId(definition, os);
		}
		public static string GetActivityTypeName(IDCWorkflowDefinition definition, IObjectSpace os) {
			return GetUniqueId(definition, os);
		}
		public static IList<IStartWorkflowCondition> GetConditions(IDCWorkflowDefinition definition) {
			return PersistentWorkflowDefinitionCore.GetConditions(definition);
		}
		public static bool Get_IsValidActivity(IDCWorkflowDefinition definition) {
			return PersistentWorkflowDefinitionCore.Get_IsValidActivity(definition);
		}
		public static void AfterConstruction(IDCWorkflowDefinition definition) {			
			PersistentWorkflowDefinitionCore.AfterConstruction(definition);
		}
		[Obsolete("Use the 'PersistentWorkflowDefinitionCore.GetWorkflowDefinitionKeyByUniqueId' method instead.")]
		public static object GetWorkflowDefinitionKeyByUniqueId(string uniqueId) {
			if(!uniqueId.StartsWith(UniqueIdPrefix)) {
				throw new ArgumentException(string.Format("{0} is wrong workflow UniqueId", uniqueId));
			}
			string keyString = uniqueId.Replace(UniqueIdPrefix, "").Replace("_", "-");
			try {
				return new Guid(keyString);
			}
			catch(Exception) {
				throw new ArgumentException(string.Format("{0} is wrong workflow UniqueId", uniqueId));
			}
		}
		public static string UniqueIdPrefix { get { return PersistentWorkflowDefinitionCore.UniqueIdPrefix; } }
		public static int WorkflowDefinitionNameMaxLength { get { return PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength; } }
		public static string WorkflowDefinitionCriteriaAppearance { get { return PersistentWorkflowDefinitionCore.WorkflowDefinitionCriteriaAppearance; } }		
	}
}
