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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Workflow.EF {	
	public abstract class EFWBaseObject : IXafEntityObject {
		public virtual void OnCreated() {
			Oid = Guid.NewGuid();
		}
		public void OnLoaded() { }
		public void OnSaving() { }
		[Browsable(false), Key]
		public Guid Oid { get; set; }
	}
	[ImageName("BO_WorkflowDefinition")]
	[System.ComponentModel.DisplayName("Workflow Definition")]
	public class EFWorkflowDefinition : EFWBaseObject, IWorkflowDefinitionSettings, ISupportIsActive, IObjectSpaceLink, INotifyPropertyChanged {
		public const string RuleId_Name_RequiredField = "{DC526573-8EBE-4C4F-B80A-E9AA80BD7E0B}";
		public const string RuleId_Name_UniqueValue = "{C4D4F0FE-1DC5-44CF-BFDA-C710CB33E169}";
		public const string RuleId_TargetType_RequiredField_WhenIsActive = "{D10F4DAA-7EF0-41B3-A8A9-12A649AE49AB}";
		public const string RuleId_Xaml_ValidActivity_WhenIsActive = "{B67D393F-8B47-4999-8D8C-10FB916F3BA8}";
		private bool autoStartWhenObjectFitsCriteria;
		private bool isActive;
		private IObjectSpace objectSpace;
		private void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}		
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public override void OnCreated() {
			base.OnCreated();
			PersistentWorkflowDefinitionCore.AfterConstruction(this);
		}
		[Browsable(false)]
		public string TypeStorage { get; set; }
		[Browsable(false)]
		public int Id { get; protected set;}
		[FieldSize(PersistentWorkflowDefinitionCore.WorkflowDefinitionNameMaxLength)]
		[RuleRequiredField(RuleId_Name_RequiredField, DefaultContexts.Save)]
		[RuleUniqueValue(RuleId_Name_UniqueValue, DefaultContexts.Save)]		 
		public string Name { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[VisibleInListView(false)]
		public string Xaml { get; set; }
		[RuleFromBoolProperty(RuleId_Xaml_ValidActivity_WhenIsActive, DefaultContexts.Save,
			UsedProperties = "Xaml", SkipNullOrEmptyValues = false, TargetCriteria = "[IsActive] = true")]
		[Browsable(false)]
		public bool IsValidActivity {
			get { return PersistentWorkflowDefinitionCore.Get_IsValidActivity(this); }
		}
		[RuleRequiredField(RuleId_TargetType_RequiredField_WhenIsActive, DefaultContexts.Save, 
			TargetCriteria = "[IsActive] = true && ([AutoStartWhenObjectIsCreated] = true || [AutoStartWhenObjectFitsCriteria] = true)")]
		[NotMapped]
		public Type TargetObjectType {
			get { return ReflectionHelper.FindType(TypeStorage); }
			set {
				if(value != null) {
					TypeStorage = value.FullName;
				}
				else {
					TypeStorage = string.Empty;
				}
			}
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[CriteriaOptions("TargetObjectType")]
		[EditorAlias(EditorAliases.ExtendedCriteriaPropertyEditor)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionCriteriaAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false)]
		public string Criteria { get; set; }
		[VisibleInDetailView(false)]
		public bool IsActive {
			get { return isActive; }
			set {
				if(isActive != value) {
					isActive = value;
					OnPropertyChanged("IsActive");
				};
			}
		}
		[VisibleInDetailView(false)]
		public bool AutoStartWhenObjectIsCreated { get; set; }
		[VisibleInDetailView(false)]
		[ImmediatePostData]
		public bool AutoStartWhenObjectFitsCriteria {
			get { return autoStartWhenObjectFitsCriteria; }
			set {
				if(autoStartWhenObjectFitsCriteria != value) {
					autoStartWhenObjectFitsCriteria = value;
					OnPropertyChanged("AutoStartWhenObjectFitsCriteria");
				};
			}
		}
		[VisibleInDetailView(false)]
		[Appearance(PersistentWorkflowDefinitionCore.WorkflowDefinitionAllowMultipleRunsAppearance, Context = "DetailView", Criteria = "AutoStartWhenObjectFitsCriteria = 'False'", Enabled = false, Visibility = ViewItemVisibility.Hide)]
		public bool AllowMultipleRuns { get; set; }
		public virtual string GetUniqueId() {
			return PersistentWorkflowDefinitionCore.GetUniqueId(this, objectSpace);			 
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
		public event PropertyChangedEventHandler PropertyChanged; 
	}
}
