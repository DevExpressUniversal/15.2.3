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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.RichEdit;
using System.Collections;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class RangeEditingPermissionsControl : UserControl, IDialogContent {
		readonly IRichEditControl control;
		readonly RangeEditingPermissionsFormController controller;
		public RangeEditingPermissionsControl() {
			InitializeComponent();
		}
		public RangeEditingPermissionsControl(RangeEditingPermissionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			this.Loaded += OnRangeEditingPermissionsControlLoaded;
		}
		void OnRangeEditingPermissionsControlLoaded(object sender, RoutedEventArgs e) {
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
			listUserGroups.ItemsSource = Controller.AvailableUserGroups;
			listUserGroups.EditValue = Controller.CheckedUserGroups;
		}
		protected internal virtual void PopulateUsers() {
			listUsers.ItemsSource = Controller.AvailableUsers;
			listUsers.EditValue = Controller.CheckedUsers;
		}
		protected internal virtual void ApplyChanges() {
			ApplyCheckedUsers();
			ApplyCheckedUserGroups();
			Controller.ApplyChanges();
		}
		protected internal virtual void ApplyCheckedUserGroups() {
			Controller.CheckedUserGroups.Clear();
			IEnumerable checkedValues = listUserGroups.EditValue as IEnumerable;
			if (checkedValues == null)
				return;
			foreach (string value in checkedValues)
				Controller.CheckedUserGroups.Add(value);
		}
		protected internal virtual void ApplyCheckedUsers() {
			Controller.CheckedUsers.Clear();
			IEnumerable checkedValues = listUsers.EditValue as IEnumerable;
			if (checkedValues == null)
				return;
			foreach (string value in checkedValues)
				Controller.CheckedUsers.Add(value);
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
