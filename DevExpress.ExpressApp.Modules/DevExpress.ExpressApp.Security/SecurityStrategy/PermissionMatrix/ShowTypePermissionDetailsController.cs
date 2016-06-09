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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Security {
	[Obsolete("Use 'PermissionMatrixGrantDenyController' instead.", true)]
	public abstract class PermissionMatrixGrantDenyControllerBase<PermissionMatrixItemType, PermissionDataType> : ViewController<ListView>{
		string grantActionId = "Grant_" + typeof(PermissionMatrixItemType).Name;
		string denyActionId = "Deny_" + typeof(PermissionMatrixItemType).Name;
		SingleChoiceAction grant;
		SingleChoiceAction deny;
		private ISupportUpdate GetUpdatable() {
			PropertyCollectionSource propertyCollectionSource = View.CollectionSource as PropertyCollectionSource;
			if(propertyCollectionSource != null) {
				return propertyCollectionSource.MasterObject as ISupportUpdate;
			}
			return null;
		}
		private void BeginPermissionsUpdate() {
			ISupportUpdate updatable = GetUpdatable();
			if(updatable != null) {
				updatable.BeginUpdate();
			}
		}
		private void EndPermissionsUpdate() {
			ISupportUpdate updatable = GetUpdatable();
			if(updatable != null) {
				updatable.EndUpdate();
			}
			if(this.Frame is NestedFrame) {
				DetailView parentView = ((NestedFrame)this.Frame).ViewItem.View as DetailView;
				if(parentView != null && parentView.ViewEditMode == ViewEditMode.View) {
					View.ObjectSpace.CommitChanges();
				}
			}
		}
		private void grant_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			BeginPermissionsUpdate();
			foreach(object item in e.SelectedObjects) {
				GrantOperation((PermissionMatrixItemType)item, (string)e.SelectedChoiceActionItem.Data);
			}
			EndPermissionsUpdate();
		}
		private void deny_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			BeginPermissionsUpdate();
			foreach(object item in e.SelectedObjects) {
				DenyOperation((PermissionMatrixItemType)item, (string)e.SelectedChoiceActionItem.Data);
			}
			EndPermissionsUpdate();
		}
		protected abstract void GrantOperation(PermissionMatrixItemType permissionMatrixItem, string operation);
		protected abstract void DenyOperation(PermissionMatrixItemType permissionMatrixItem, string operation);
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			bool isGrantedBySecurity = DataManipulationRight.CanEdit(typeof(PermissionMatrixItemType), null, null, null, null)
				&& DataManipulationRight.CanEdit(typeof(PermissionDataType), null, null, null, null);
			if(action.Id == denyActionId) {
				isGrantedBySecurity &= DataManipulationRight.CanDelete(typeof(PermissionDataType), null, null, null); 
			}
			action.Active.SetItemValue(DevExpress.ExpressApp.View.SecurityReadOnlyItemName, isGrantedBySecurity);
		}
		public PermissionMatrixGrantDenyControllerBase() {
			TargetObjectType = typeof(PermissionMatrixItemType);
			grant = new SingleChoiceAction(this, grantActionId, PredefinedCategory.Edit);
			grant.Caption = "Grant";
			grant.ImageName = "Action_Grant";
			grant.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			grant.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			grant.Items.Add(new ChoiceActionItem("Full Access", SecurityOperations.FullAccess));
			grant.Items.Add(new ChoiceActionItem("Read Only Access", SecurityOperations.ReadOnlyAccess));
			grant.Items.Add(new ChoiceActionItem(SecurityOperations.Read, SecurityOperations.Read));
			grant.Items.Add(new ChoiceActionItem(SecurityOperations.Write, SecurityOperations.Write));
			grant.Items.Add(new ChoiceActionItem(SecurityOperations.Create, SecurityOperations.Create));
			grant.Items.Add(new ChoiceActionItem(SecurityOperations.Delete, SecurityOperations.Delete));
			grant.Items.Add(new ChoiceActionItem(SecurityOperations.Navigate, SecurityOperations.Navigate));
			grant.Execute += new SingleChoiceActionExecuteEventHandler(grant_Execute);
			deny = new SingleChoiceAction(this, denyActionId, PredefinedCategory.Edit);
			deny.Caption = "Deny";
			deny.ImageName = "Action_Deny";
			deny.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			deny.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			deny.Items.Add(new ChoiceActionItem("Deny All", SecurityOperations.FullAccess));
			deny.Items.Add(new ChoiceActionItem("Deny Modification", SecurityOperations.ReadWriteAccess + ";" + SecurityOperations.Create));
			deny.Items.Add(new ChoiceActionItem(SecurityOperations.Read, SecurityOperations.Read));
			deny.Items.Add(new ChoiceActionItem(SecurityOperations.Write, SecurityOperations.Write));
			deny.Items.Add(new ChoiceActionItem(SecurityOperations.Create, SecurityOperations.Create));
			deny.Items.Add(new ChoiceActionItem(SecurityOperations.Delete, SecurityOperations.Delete));
			deny.Items.Add(new ChoiceActionItem(SecurityOperations.Navigate, SecurityOperations.Navigate));
			deny.Execute += new SingleChoiceActionExecuteEventHandler(deny_Execute);
		}
		public SingleChoiceAction ActionGrant {
			get {
				return grant;
			}
		}
		public SingleChoiceAction ActionDeny {
			get {
				return deny;
			}
		}
	}
}
