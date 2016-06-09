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
using DevExpress.ExpressApp.Actions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Workflow.Controllers {
	public class ActivationController : ObjectViewController {
		private const string WorkflowActive_ItemName = "WorkflowActive";
		private const string BySecurity = "By Security";
		private static CriteriaOperator isActiveCriteriaOperator = new BinaryOperator("IsActive", true);
		private static CriteriaOperator isInactiveCriteriaOperator = new BinaryOperator("IsActive", false);
		SimpleAction actionActivate;
		SimpleAction actionDeactivate;
		public ActivationController() {
			TargetObjectType = typeof(ISupportIsActive);
			actionActivate = new SimpleAction(this, "Activate", PredefinedCategory.RecordEdit);
			actionActivate.TargetObjectsCriteria = isInactiveCriteriaOperator.ToString();
			actionActivate.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			actionActivate.ImageName = "Action_Workflow_Activate";
			actionActivate.Execute += new SimpleActionExecuteEventHandler(actionActivate_Execute);
			actionDeactivate = new SimpleAction(this, "Deactivate", PredefinedCategory.RecordEdit);
			actionDeactivate.TargetObjectsCriteria = isActiveCriteriaOperator.ToString();
			actionDeactivate.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne;
			actionDeactivate.ImageName = "Action_Workflow_Deactivate";
			actionDeactivate.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			actionDeactivate.Execute += new SimpleActionExecuteEventHandler(actionDeactivate_Execute);
		}
		void actionDeactivate_Execute(object sender, SimpleActionExecuteEventArgs e) {
			foreach(object obj in e.SelectedObjects) {
				((ISupportIsActive)obj).IsActive = false;
				ObjectSpace.SetModified(obj);
			}
		}
		void actionActivate_Execute(object sender, SimpleActionExecuteEventArgs e) {
			foreach(object obj in e.SelectedObjects) {
				((ISupportIsActive)obj).IsActive = true;
				ObjectSpace.SetModified(obj);
			}
		}
		private void CollectionSource_CollectionChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			DoUpdateActionState();
		}
		private void DoUpdateActionState() {
			if (View != null) {
				actionActivate.BeginUpdate();
				actionDeactivate.BeginUpdate();
				try {
					UpdateActionActivityByCurrentObject();
					UpdateActionActivityBySecurity();
				} finally {
					actionActivate.EndUpdate();
					actionDeactivate.EndUpdate();
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is DetailView) {
				actionDeactivate.Enabled.Changed += delegate(object sender, EventArgs e) {
					UpdateActionActivityByCurrentObject();
				};
				actionActivate.Enabled.Changed += delegate(object sender, EventArgs e) {
					UpdateActionActivityByCurrentObject();
				};
			}
			ListView listView = View as ListView;
			if (listView != null && listView.CollectionSource != null) {
				listView.CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionChanged);
			}
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			DoUpdateActionState();
		}
		protected override void OnDeactivated() {
			View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			ListView listView = View as ListView;
			if (listView != null && listView.CollectionSource != null) {
				listView.CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionChanged);
			}
			base.OnDeactivated();
		}
		protected virtual void UpdateActionActivityByCurrentObject() {
			if(View is DetailView) {
				actionDeactivate.Active[WorkflowActive_ItemName] = !actionDeactivate.Enabled.Contains(ActionsCriteriaViewController.EnabledByCriteriaKey)
					|| actionDeactivate.Enabled[ActionsCriteriaViewController.EnabledByCriteriaKey];
				actionActivate.Active[WorkflowActive_ItemName] = !actionActivate.Enabled.Contains(ActionsCriteriaViewController.EnabledByCriteriaKey)
					|| actionActivate.Enabled[ActionsCriteriaViewController.EnabledByCriteriaKey];
			}
		}
		protected virtual void UpdateActionActivityBySecurity() {
			bool isModificationGrantedForAllSelectedObjects = false;
			if (View.SelectedObjects.Count == 1) {
				isModificationGrantedForAllSelectedObjects = DataManipulationRight.CanEdit(View.ObjectTypeInfo.Type, null, View.SelectedObjects[0], null, View.ObjectSpace);
			} else {
				isModificationGrantedForAllSelectedObjects = DataManipulationRight.CanEdit(View.ObjectTypeInfo.Type, null, null, null, View.ObjectSpace);
				if (isModificationGrantedForAllSelectedObjects) {
					foreach (object selectedObject in View.SelectedObjects) {
						if (!DataManipulationRight.CanEdit(View.ObjectTypeInfo.Type, null, selectedObject, null, View.ObjectSpace)) {
							isModificationGrantedForAllSelectedObjects = false;
							break;
						}
					}
				}
			}
			actionDeactivate.Active[BySecurity] = isModificationGrantedForAllSelectedObjects;
			actionActivate.Active[BySecurity] = isModificationGrantedForAllSelectedObjects;
		}
		public SimpleAction ActionActivate {
			get { return actionActivate; }
		}
		public SimpleAction ActionDeactivate {
			get { return actionDeactivate; }
		}
	}
}
