#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.Forms {
	#region RangeEditingPermissionsFormControllerParameters
	public class RangeEditingPermissionsFormControllerParameters : FormControllerParameters {
		internal RangeEditingPermissionsFormControllerParameters(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
	#region RangeEditingPermissionsFormController
	public class RangeEditingPermissionsFormController : FormController {
		#region Fields
		readonly IRichEditControl control;
		IList<string> checkedUsers;
		IList<string> checkedUserGroups;
		IList<string> availableUsers;
		IList<string> availableUserGroups;
		RangePermissionCollection rangePermissions;
		#endregion
		public RangeEditingPermissionsFormController(RangeEditingPermissionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			Initialize();
		}
		#region Properties
		protected internal IRichEditControl Control { get { return control; } }
		public IList<string> CheckedUsers { get { return checkedUsers; } }
		public IList<string> CheckedUserGroups { get { return checkedUserGroups; } }
		public IList<string> AvailableUsers { get { return availableUsers; } }
		public IList<string> AvailableUserGroups { get { return availableUserGroups; } }
		protected internal RangePermissionCollection RangePermissions { get { return rangePermissions; } }
		#endregion
		protected internal virtual void Initialize() {
			this.rangePermissions = ObtainRangePermissionsMatchSelection();
			RangePermissionCollection permissions;
			if (rangePermissions.Count > 0)
				permissions = rangePermissions;
			else
				permissions = ObtainRangePermissionsWithSelectionInside();
			this.availableUsers = ObtainAvailableUsers();
			this.availableUserGroups = ObtainAvailableUserGroups();
			this.checkedUsers = ObtainCheckedUsers(permissions);
			this.checkedUserGroups = ObtainCheckedGroups(permissions);
			MergeStringLists(availableUsers, checkedUsers);
			MergeStringLists(availableUserGroups, checkedUserGroups);
		}
		public void CheckUser(string userName) {
			if (String.IsNullOrEmpty(userName))
				return;
			if (!CheckedUsers.Contains(userName))
				CheckedUsers.Add(userName);
		}
		public void UncheckUser(string userName) {
			if (String.IsNullOrEmpty(userName))
				return;
			int index = CheckedUsers.IndexOf(userName);
			if (index >= 0)
				CheckedUsers.RemoveAt(index);
		}
		public void CheckUserGroup(string userGroupName) {
			if (!CheckedUserGroups.Contains(userGroupName))
				CheckedUserGroups.Add(userGroupName);
		}
		public void UncheckUserGroup(string userGroupName) {
			int index = CheckedUserGroups.IndexOf(userGroupName);
			if (index >= 0)
				CheckedUserGroups.RemoveAt(index);
		}
		protected internal virtual IList<string> ObtainAvailableUsers() {
			IUserListService service = control.InnerControl.GetService<IUserListService>();
			if (service != null)
				return service.GetUsers();
			else
				return new List<string>();
		}
		protected internal virtual IList<string> ObtainAvailableUserGroups() {
			IUserGroupListService service = control.InnerControl.GetService<IUserGroupListService>();
			if (service != null)
				return service.GetUserGroups();
			else
				return new List<string>();
		}
		protected internal virtual IList<string> ObtainCheckedUsers(RangePermissionCollection rangePermissions) {
			List<string> result = new List<string>();
			int count = rangePermissions.Count;
			for (int i = 0; i < count; i++) {
				string value = rangePermissions[i].UserName;
				if (!String.IsNullOrEmpty(value) && !result.Contains(value))
					result.Add(value);
			}
			return result;
		}
		protected internal virtual IList<string> ObtainCheckedGroups(RangePermissionCollection rangePermissions) {
			List<string> result = new List<string>();
			int count = rangePermissions.Count;
			for (int i = 0; i < count; i++) {
				string value = rangePermissions[i].Group;
				if (!String.IsNullOrEmpty(value) && !result.Contains(value))
					result.Add(value);
			}
			return result;
		}
		protected internal void MergeStringLists(IList<string> target, IList<string> listToAppend) {
			int count = listToAppend.Count;
			for (int i = 0; i < count; i++) {
				string value = listToAppend[i];
				if (!target.Contains(value))
					target.Add(value);
			}
		}
		protected internal virtual RangePermissionCollection ObtainRangePermissionsMatchSelection() {
			InnerRichEditControl innerControl = control.InnerControl;
			Selection selection = innerControl.DocumentModel.Selection;
			return selection.PieceTable.ObtainRangePermissionsMatchSelection();
		}
		protected internal virtual RangePermissionCollection ObtainRangePermissionsWithSelectionInside() {
			InnerRichEditControl innerControl = control.InnerControl;
			Selection selection = innerControl.DocumentModel.Selection;
			RangePermissionCollection permissions = selection.PieceTable.ObtainRangePermissionsWithSelectionInside();
			int count = permissions.Count;
			if (count <= 0)
				return permissions;
			if (selection.Items.Count > 1) {
				int index = permissions[0].Properties.Index;
				for (int i = 1; i < count; i++) {
					if (permissions[i].Properties.Index != index) {
						permissions.Clear();
						break;
					}
				}
			}
			return permissions;
		}
		public override void ApplyChanges() {
			DocumentModel documentModel = control.InnerControl.DocumentModel;
			control.BeginUpdate();
			try {
				documentModel.BeginUpdate();
				try {
					PieceTable pieceTable = documentModel.ActivePieceTable;
					RemoveOldRangePermissions(pieceTable);
					ApplyNewRangePermissions(pieceTable);
				}
				finally {
					documentModel.EndUpdate();
				}
			}
			finally {
				control.EndUpdate();
			}
		}
		protected internal void RemoveOldRangePermissions(PieceTable pieceTable) {
			int count = rangePermissions.Count;
			for (int i = 0; i < count; i++) {
				pieceTable.DeleteRangePermission(rangePermissions[i]);
			}
		}
		protected internal void ApplyNewRangePermissions(PieceTable pieceTable) {
			List<RangePermissionInfo> permissions = new List<RangePermissionInfo>();
			AppendUserNamePermissions(permissions);
			AppendUserGroupNamePermissions(permissions);
			if (permissions.Count > 0)
				ApplyNewRangePermissions(pieceTable.DocumentModel.Selection.Items, permissions);
			else
				RemoveRangePermissions(pieceTable.DocumentModel.Selection.Items);
		}
		protected internal void ApplyNewRangePermissions(List<SelectionItem> selectionItems, List<RangePermissionInfo> permissions) {
			int count = selectionItems.Count;
			for (int i = 0; i < count; i++)
				ApplyNewRangePermissions(selectionItems[i], permissions);
		}
		protected internal void ApplyNewRangePermissions(SelectionItem item, List<RangePermissionInfo> permissions) {
			int count = permissions.Count;
			for (int i = 0; i < count; i++)
				item.PieceTable.ApplyDocumentPermission(item.NormalizedStart, item.NormalizedEnd, permissions[i]);
		}
		protected internal void RemoveRangePermissions(List<SelectionItem> selectionItems) {
			int count = selectionItems.Count;
			for (int i = 0; i < count; i++)
				RemoveRangePermissions(selectionItems[i]);
		}
		protected internal void RemoveRangePermissions(SelectionItem item) {
			RangePermissionCollection permissions = item.PieceTable.RangePermissions;
			for (int i = 0; i < permissions.Count; i++) {
				RangePermission permission = permissions[i];
				if (permission.IntersectsWithExcludingBounds(item)) {
					DocumentLogPosition start = item.Length != 0 ? Algorithms.Max(item.NormalizedStart, permission.NormalizedStart) : permission.NormalizedStart;
					DocumentLogPosition end = item.Length != 0 ?  Algorithms.Min(item.NormalizedEnd, permission.NormalizedEnd) : permission.NormalizedEnd;
					item.PieceTable.RemoveDocumentPermission(start, end, permission.Properties.Info);
					i = -1; 
				}
			}
		}
		protected internal void AppendUserGroupNamePermissions(List<RangePermissionInfo> permissions) {
			int count = CheckedUsers.Count;
			for (int i = 0; i < count; i++) {
				RangePermissionInfo info = new RangePermissionInfo();
				info.UserName = CheckedUsers[i];
				permissions.Add(info);
			}
		}
		protected internal void AppendUserNamePermissions(List<RangePermissionInfo> permissions) {
			int count = CheckedUserGroups.Count;
			for (int i = 0; i < count; i++) {
				RangePermissionInfo info = new RangePermissionInfo();
				info.Group = CheckedUserGroups[i];
				permissions.Add(info);
			}
		}
	}
	#endregion
	#region DocumentProtectionQueryNewPasswordFormControllerParameters
	public class DocumentProtectionQueryNewPasswordFormControllerParameters : FormControllerParameters {
		readonly PasswordInfo passwordInfo;
		internal DocumentProtectionQueryNewPasswordFormControllerParameters(IRichEditControl control, PasswordInfo passwordInfo)
			: base(control) {
			Guard.ArgumentNotNull(passwordInfo, "passwordInfo");
			this.passwordInfo = passwordInfo;
		}
		internal PasswordInfo PasswordInfo { get { return passwordInfo; } }
	}
	#endregion
	#region DocumentProtectionQueryNewPasswordFormController
	public class DocumentProtectionQueryNewPasswordFormController : FormController {
		readonly PasswordInfo passwordInfo;
		public DocumentProtectionQueryNewPasswordFormController(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.passwordInfo = controllerParameters.PasswordInfo;
		}
		public string Password { get { return passwordInfo.Password; } set { passwordInfo.Password = value; } }
		public override void ApplyChanges() {
		}
	}
	#endregion
	#region DocumentProtectionQueryPasswordFormControllerParameters
	public class DocumentProtectionQueryPasswordFormControllerParameters : FormControllerParameters {
		readonly PasswordInfo passwordInfo;
		internal DocumentProtectionQueryPasswordFormControllerParameters(IRichEditControl control, PasswordInfo passwordInfo)
			: base(control) {
			Guard.ArgumentNotNull(passwordInfo, "passwordInfo");
			this.passwordInfo = passwordInfo;
		}
		internal PasswordInfo PasswordInfo { get { return passwordInfo; } }
	}
	#endregion
	#region DocumentProtectionQueryPasswordFormController
	public class DocumentProtectionQueryPasswordFormController : FormController {
		readonly PasswordInfo passwordInfo;
		public DocumentProtectionQueryPasswordFormController(DocumentProtectionQueryPasswordFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.passwordInfo = controllerParameters.PasswordInfo;
		}
		public string Password { get { return passwordInfo.Password; } set { passwordInfo.Password = value; } }
		public override void ApplyChanges() {
		}
	}
	#endregion
	#region InsertMergeFieldFormControllerParameters
	public class InsertMergeFieldFormControllerParameters : FormControllerParameters {
		internal InsertMergeFieldFormControllerParameters(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
}
