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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.btnApply")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.lblGroups")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.listUsers")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.listUserGroups")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.RangeEditingPermissionsForm.lblUsers")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class RangeEditingPermissionsForm : XtraForm {
		readonly IRichEditControl control;
		readonly RangeEditingPermissionsFormController controller;
		RangeEditingPermissionsForm() {
			InitializeComponent();
		}
		public RangeEditingPermissionsForm(RangeEditingPermissionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			UpdateForm();
		}
		public IRichEditControl Control { get { return control; } }
		public RangeEditingPermissionsFormController Controller { get { return controller; } }
		protected virtual RangeEditingPermissionsFormController CreateController(RangeEditingPermissionsFormControllerParameters controllerParameters) {
			return new RangeEditingPermissionsFormController(controllerParameters);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
		}
		protected internal virtual void UnsubscribeControlsEvents() {
		}
		protected internal virtual void UpdateFormCore() {
			PopulateUsers();
			PopulateUserGroups();
		}
		protected internal virtual void PopulateUserGroups() {
			listUserGroups.DataSource = Controller.AvailableUserGroups;
			int count = listUserGroups.ItemCount;
			for (int i = 0; i < count; i++) {
				if (Controller.CheckedUserGroups.Contains((string)listUserGroups.GetItemValue(i)))
					listUserGroups.SetItemChecked(i, true);
			}
		}
		protected internal virtual void PopulateUsers() {
			listUsers.DataSource = Controller.AvailableUsers;
			int count = listUsers.ItemCount;
			for (int i = 0; i < count; i++) {
				if (Controller.CheckedUsers.Contains((string)listUsers.GetItemValue(i)))
					listUsers.SetItemChecked(i, true);
			}
		}
		protected internal virtual void OnOk(object sender, EventArgs e) {
			OnApply(sender, e);
		}
		protected internal virtual void OnCancel(object sender, EventArgs e) {
		}
		protected internal virtual void OnApply(object sender, EventArgs e) {
			ApplyCheckedUsers();
			ApplyCheckedUserGroups();
			Controller.ApplyChanges();
		}
		protected internal virtual void ApplyCheckedUserGroups() {
			Controller.CheckedUserGroups.Clear();
			int count = listUserGroups.ItemCount;
			for (int i = 0; i < count; i++) {
				if (listUserGroups.GetItemChecked(i))
					Controller.CheckedUserGroups.Add((string)listUserGroups.GetItemValue(i));
			}
		}
		protected internal virtual void ApplyCheckedUsers() {
			Controller.CheckedUsers.Clear();
			int count = listUsers.ItemCount;
			for (int i = 0; i < count; i++) {
				if (listUsers.GetItemChecked(i))
					Controller.CheckedUsers.Add((string)listUsers.GetItemValue(i));
			}
		}
	}
}
