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
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public partial class PdfPasswordSecurityOptionsView : DXDialog, IPdfPasswordSecurityOptionsView {
		public PdfPasswordSecurityOptionsView()
		{  InitializeComponent();
			Loaded += PdfPasswordSecurityOptionsView_Loaded;
		}
		void PdfPasswordSecurityOptionsView_Loaded(object sender, RoutedEventArgs e) {
			SubscribeSecurityOptionsControl();
		}
		void SubscribeSecurityOptionsControl() {
			SecurityOptionsControl.RequireOpenPasswordChanged += SecurityOptionsControl_RequireOpenPasswordChanged;
			SecurityOptionsControl.RestrictPermissionsChanged += SecurityOptionsControl_RestrictPermissionsChanged;
			SecurityOptionsControl.RepeatPasswordComplete += SecurityOptionsControl_RepeatPasswordComplete;
			SubscribeOkCancelButtons();
		}
		void SecurityOptionsControl_RepeatPasswordComplete(object sender, RepeatPasswordCompleteEventArgs e) {
			if(RepeatPasswordComplete != null)
				RepeatPasswordComplete(this, e);
		}
		void SecurityOptionsControl_RestrictPermissionsChanged(object sender, EventArgs e) {
			if(RestrictPermissionsChanged != null)
				RestrictPermissionsChanged(this, EventArgs.Empty);
		}
		void SecurityOptionsControl_RequireOpenPasswordChanged(object sender, EventArgs e) {
			if(RequireOpenPasswordChanged != null)
				RequireOpenPasswordChanged(this, EventArgs.Empty);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SubscribeOkCancelButtons();
		}
		void SubscribeOkCancelButtons() {
			OkButton.Click += okButton_Click;
			CancelButton.Click += cancelButton_Click;
		}
		#region IPdfPasswordSecurityOptionsView Members
		public bool RequireOpenPassword {
			get {
				return SecurityOptionsControl.RequireOpenPassword;
			}
			set {
				SecurityOptionsControl.RequireOpenPassword = value;
			}
		}
		public string OpenPassword {
			get {
				return SecurityOptionsControl.OpenPassword;
			}
			set {
				SecurityOptionsControl.OpenPassword = value;
			}
		}
		public bool RestrictPermissions {
			get {
				return SecurityOptionsControl.RestrictPermissions;
			}
			set {
				SecurityOptionsControl.RestrictPermissions = value;
			}
		}
		public string PermissionsPassword {
			get {
				return SecurityOptionsControl.PermissionsPassword;
			}
			set {
				SecurityOptionsControl.PermissionsPassword = value;
			}
		}
		public PrintingPermissions PrintingPermissions {
			get {
				return (PrintingPermissions)SecurityOptionsControl.PrintingPermissions;
			}
			set {
				SecurityOptionsControl.PrintingPermissions = value;
			}
		}
		public ChangingPermissions ChangingPermissions {
			get {
				return (ChangingPermissions)SecurityOptionsControl.ChangingPermissions;
			}
			set {
				SecurityOptionsControl.ChangingPermissions = value;
			}
		}
		public bool EnableCopying {
			get {
				return SecurityOptionsControl.EnableCopying;
			}
			set {
				SecurityOptionsControl.EnableCopying = value;
			}
		}
		public bool EnableScreenReaders {
			get {
				return SecurityOptionsControl.EnableScreenReaders;
			}
			set {
				SecurityOptionsControl.EnableScreenReaders = value;
			}
		}
		public event EventHandler RequireOpenPasswordChanged;
		public event EventHandler RestrictPermissionsChanged;
		public event EventHandler<RepeatPasswordCompleteEventArgs> RepeatPasswordComplete;
		public event EventHandler Submit;
		public event EventHandler Dismiss;
		public void EnableControl_OpenPassword(bool enable) {
			SecurityOptionsControl.EnableControl_OpenPassword(enable);
		}
		public void EnableControlGroup_Permissions(bool enable) {
			SecurityOptionsControl.EnableControlGroup_Permissions(enable);
		}
		public void InitializeChangingPermissions(IEnumerable<KeyValuePair<ChangingPermissions, string>> availablePermissions) {
			SecurityOptionsControl.InitializeChangingPermissions(availablePermissions);
		}
		public void InitializePrintingPermissions(IEnumerable<KeyValuePair<PrintingPermissions, string>> availablePermissions) {
			SecurityOptionsControl.InitializePrintingPermissions(availablePermissions);
		}
		public void RepeatOpenPassword() {
			SecurityOptionsControl.RepeatOpenPassword();
		}
		public void RepeatPermissionsPassword() {
			SecurityOptionsControl.RepeatPermissionsPassword();
		}
		public void PasswordDoesNotMatch() {
			SecurityOptionsControl.PasswordDoesNotMatch();
		}
		#endregion
		public void RepeatPassword(string caption, string note, string passwordName) {
			SecurityOptionsControl.RepeatPassword(caption, note, passwordName);
		}
		private void okButton_Click(object sender, RoutedEventArgs e) {
			if(Submit != null)
				Submit(this, EventArgs.Empty);
		}
		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			if(Dismiss != null)
				Dismiss(this, EventArgs.Empty);
		}
	}
}
