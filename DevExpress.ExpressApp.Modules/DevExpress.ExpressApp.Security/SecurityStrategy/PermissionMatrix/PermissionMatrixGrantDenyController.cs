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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System;
namespace DevExpress.ExpressApp.Security.Strategy.PermissionMatrix {
	public class PermissionMatrixGrantDenyController : ViewController<ListView> {
		SingleChoiceAction grant;
		SingleChoiceAction deny;
		string grantActionId = "Grant_TypePermissionMatrixItem";
		string denyActionId = "Deny_TypePermissionMatrixItem";
		internal static void SetTypeOperations(ITypePermissionOperations permissionMatrixItem, string operations, bool modifier) {
			string[] operationItems = operations.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(string operation in operationItems) {
				switch(operation.Trim()) {
					case SecurityOperations.Read:
						permissionMatrixItem.AllowRead = modifier;
						break;
					case SecurityOperations.Write:
						permissionMatrixItem.AllowWrite = modifier;
						break;
					case SecurityOperations.Create:
						permissionMatrixItem.AllowCreate = modifier;
						break;
					case SecurityOperations.Delete:
						permissionMatrixItem.AllowDelete = modifier;
						break;
					case SecurityOperations.Navigate:
						permissionMatrixItem.AllowNavigate = modifier;
						break;
					default:
						throw Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
		}
		public PermissionMatrixGrantDenyController() {
			TargetObjectType = typeof(TypePermissionMatrixItem);
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
		private void grant_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			foreach(object item in e.SelectedObjects) {
				GrantOperation((TypePermissionMatrixItem)item, (string)e.SelectedChoiceActionItem.Data);
			}
		}
		private void deny_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			foreach(object item in e.SelectedObjects) {
				DenyOperation((TypePermissionMatrixItem)item, (string)e.SelectedChoiceActionItem.Data);
			}
		}
		private void SetModifier(TypePermissionMatrixItem permissionMatrixItem, string operation, bool modifier) {
			if(string.IsNullOrEmpty(operation)) {
				return;
			}
			SetTypeOperations(permissionMatrixItem, operation, modifier);
		}
		protected void GrantOperation(TypePermissionMatrixItem permissionMatrixItem, string operation) {
			SetModifier(permissionMatrixItem, operation, true);
		}
		protected void DenyOperation(TypePermissionMatrixItem permissionMatrixItem, string operation) {
			if(operation == SecurityOperations.FullAccess) {
				permissionMatrixItem.ObjectPermissions.Clear();
				permissionMatrixItem.MemberPermissions.Clear();
			}
			SetModifier(permissionMatrixItem, operation, false);
		}
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			bool isGrantedBySecurity = DataManipulationRight.CanEdit(typeof(TypePermissionMatrixItem), null, null, null, null)
				&& DataManipulationRight.CanEdit(typeof(SecuritySystemTypePermissionObject), null, null, null, null);
			if(action.Id == denyActionId) {
				isGrantedBySecurity &= DataManipulationRight.CanDelete(typeof(SecuritySystemTypePermissionObject), null, null, null); 
			}
			action.Active.SetItemValue(DevExpress.ExpressApp.View.SecurityReadOnlyItemName, isGrantedBySecurity);
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
