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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Workflow.Xpo {
	[NonPersistent]
	public abstract class WFBaseObject : XPCustomObject {
		public WFBaseObject(Session session) : base(session) { }
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		protected override void OnSaving() {
			base.OnSaving();
			if(!(Session is NestedUnitOfWork) && Session.IsNewObject(this)) {
				if(oid.Equals(Guid.Empty)) {
					oid = XpoDefault.NewGuid();
				}
			}
		}
	}
	[ImageName("BO_WorkflowDefinition")]
	[System.ComponentModel.DisplayName("Workflow Definition")]
	public class XpoWorkflowDefinition : WFBaseObject, IWorkflowDefinitionSettings, ISupportIsActive {
		public const string RuleId_Name_RequiredField = "{E51ABF7D-8DAE-47F5-904E-F2113B687C72}";
		public const string RuleId_Name_UniqueValue = "{5F6B2B07-2A68-4BB6-8AD9-5059B3D23163}";
		public const string RuleId_TargetType_RequiredField_WhenIsActive = "{90FA7B3A-21C8-4472-9AA8-E5A3BA1253EF}";
		public const string RuleId_Xaml_ValidActivity_WhenIsActive = "{8972576C-6666-436C-B6F8-948E98346A9D}";
		public XpoWorkflowDefinition(Session session) : base(session) { }
		public override void AfterConstruction() {
			PersistentWorkflowDefinitionCore.AfterConstruction(this);
		}
		[RuleRequiredField(RuleId_Name_RequiredField, DefaultContexts.Save)]
		[Size(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		[RuleUniqueValue(RuleId_Name_UniqueValue, DefaultContexts.Save)] 
		public string Name {
			get { return GetPropertyValue<string>("Name"); }
			set { SetPropertyValue<string>("Name", value); }
		}
		[Size(-1)]
		[VisibleInListView(false)]
		public string Xaml {
			get { return GetPropertyValue<string>("Xaml"); }
			set { SetPropertyValue<string>("Xaml", value); }
		}
		[RuleFromBoolProperty(RuleId_Xaml_ValidActivity_WhenIsActive, DefaultContexts.Save,
			UsedProperties = "Xaml", SkipNullOrEmptyValues = false, TargetCriteria = "[IsActive] = true")]
		[Browsable(false)]
		public bool IsValidActivity {
			get { return PersistentWorkflowDefinitionCore.Get_IsValidActivity(this); }
		}
		[ValueConverter(typeof(TypeToStringConverter))]
		[RuleRequiredField(RuleId_TargetType_RequiredField_WhenIsActive, DefaultContexts.Save, 
			TargetCriteria = "[IsActive] = true && ([AutoStartWhenObjectIsCreated] = true || [AutoStartWhenObjectFitsCriteria] = true)")]
		public Type TargetObjectType {
			get { return GetPropertyValue<Type>("TargetObjectType"); }
			set { SetPropertyValue<Type>("TargetObjectType", value); }
		}
		[CriteriaOptions("TargetObjectType")]
		[EditorAlias(EditorAliases.ExtendedCriteriaPropertyEditor)]
		[Size(SizeAttribute.Unlimited)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionCriteriaAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false)]
		public string Criteria {
			get { return GetPropertyValue<string>("Criteria"); }
			set { SetPropertyValue<string>("Criteria", value); }
		}
		[VisibleInDetailView(false)]
		public bool IsActive {
			get { return GetPropertyValue<bool>("IsActive"); }
			set { SetPropertyValue<bool>("IsActive", value); }
		}
		[VisibleInDetailView(false)]
		public bool AutoStartWhenObjectIsCreated {
			get { return GetPropertyValue<bool>("AutoStartWhenObjectIsCreated"); }
			set { SetPropertyValue<bool>("AutoStartWhenObjectIsCreated", value); }
		}
		[VisibleInDetailView(false)]
		[ImmediatePostData]
		public bool AutoStartWhenObjectFitsCriteria {
			get { return GetPropertyValue<bool>("AutoStartWhenObjectFitsCriteria"); }
			set { SetPropertyValue<bool>("AutoStartWhenObjectFitsCriteria", value); }
		}
		[VisibleInDetailView(false)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionAllowMultipleRunsAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false, Visibility = ViewItemVisibility.Hide)]
		public bool AllowMultipleRuns {
			get { return GetPropertyValue<bool>("AllowMultipleRuns"); }
			set { SetPropertyValue<bool>("AllowMultipleRuns", value); }
		}
		public virtual string GetUniqueId() {
			IObjectSpace os = XPObjectSpace.FindObjectSpaceByObject(this);
			if(os == null) {
				os = new XPObjectSpace(XpoTypesInfoHelper.GetTypesInfo(), XpoTypesInfoHelper.GetXpoTypeInfoSource(), () => { return (UnitOfWork)Session; });
			}
			Guard.ArgumentNotNull(os, "ObjectSpace.FindObjectSpaceByObject(this)");
			return PersistentWorkflowDefinitionCore.GetUniqueId(this, os);
		}
		public virtual string GetActivityTypeName() {
			return GetUniqueId();
		}
		public virtual IList<IStartWorkflowCondition> GetConditions() {
			return PersistentWorkflowDefinitionCore.GetConditions(this);
		}
		[Browsable(false)]
		public virtual bool CanCompile {
			get { return PersistentWorkflowDefinitionCore.Get_CanCompile(this); }
		}
		[Browsable(false)]
		public virtual bool CanCompileForDesigner {
			get { return PersistentWorkflowDefinitionCore.Get_CanCompileForDesigner(this); }
		}
		[Browsable(false)]
		public virtual bool CanOpenHost {
			get { return PersistentWorkflowDefinitionCore.Get_CanOpenHost(this); }
		}
	}
}
