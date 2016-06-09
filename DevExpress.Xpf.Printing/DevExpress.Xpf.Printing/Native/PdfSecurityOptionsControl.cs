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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class PdfSecurityOptionsControl : Control, INotifyPropertyChanged {
		#region Fields
		bool openPasswordEnabled;
		bool requireOpenPassword;
		bool restrictPermissions;
		bool enableScreenReaders;
		PrintingPermissions printingPermissions;
		ChangingPermissions changingPermissions;
		IEnumerable<KeyValuePair<ChangingPermissions, string>> changingPermissionsValues;
		IEnumerable<KeyValuePair<PrintingPermissions, string>> printingPermissionsValues;
		string openPassword;
		string permissionsPassword;
		bool isPermissionsEnabled;
		bool enableCopying;
		#endregion
		#region Constructors
		static PdfSecurityOptionsControl() {
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfSecurityOptionsControl), new FrameworkPropertyMetadata(typeof(PdfSecurityOptionsControl)));
#endif
		}
		public PdfSecurityOptionsControl() {
			DataContext = this;
		}
		#endregion
		#region Properties
		#region IPdfPasswordSecurityOptionsView Members
		public bool RequireOpenPassword {
			get {
				return requireOpenPassword == true;
			}
			set {
				if(requireOpenPassword == value)
					return;
				requireOpenPassword = value;
				if(RequireOpenPasswordChanged != null)
					RequireOpenPasswordChanged(this, EventArgs.Empty);
				RaisePropertyChanged(() => RequireOpenPassword);
			}
		}
		public string OpenPassword {
			get {
				return openPassword;
			}
			set {
				if(openPassword == value)
					return;
				openPassword = value;
				RaisePropertyChanged(() => OpenPassword);
			}
		}
		public bool RestrictPermissions {
			get {
				return restrictPermissions == true;
			}
			set {
				if(restrictPermissions == value)
					return;
				restrictPermissions = value;
				if(RestrictPermissionsChanged != null)
					RestrictPermissionsChanged(this, EventArgs.Empty);
				RaisePropertyChanged(() => RestrictPermissions);
			}
		}
		public string PermissionsPassword {
			get {
				return permissionsPassword;
			}
			set {
				if(permissionsPassword == value)
					return;
				permissionsPassword = value;
				RaisePropertyChanged(() => PermissionsPassword);
			}
		}
		public PrintingPermissions PrintingPermissions {
			get {
				return printingPermissions;
			}
			set {
				if(printingPermissions == value)
					return;
				printingPermissions = value;
				RaisePropertyChanged(() => PrintingPermissions);
			}
		}
		public ChangingPermissions ChangingPermissions {
			get {
				return changingPermissions;
			}
			set {
				if(changingPermissions == value)
					return;
				changingPermissions = value;
				RaisePropertyChanged(() => ChangingPermissions);
			}
		}
		public IEnumerable<KeyValuePair<ChangingPermissions, string>> ChangingPermissionsValues {
			get {
				return changingPermissionsValues;
			}
			set {
				if(changingPermissionsValues == value)
					return;
				changingPermissionsValues = value;
				RaisePropertyChanged(() => ChangingPermissionsValues);
			}
		}
		public IEnumerable<KeyValuePair<PrintingPermissions, string>> PrintingPermissionsValues {
			get {
				return printingPermissionsValues;
			}
			set {
				if(printingPermissionsValues == value)
					return;
				printingPermissionsValues = value;
				RaisePropertyChanged(() => PrintingPermissionsValues);
			}
		}
		public bool EnableCopying {
			get {
				return enableCopying == true;
			}
			set {
				if(enableCopying == value)
					return;
				enableCopying = value;
				RaisePropertyChanged(() => EnableCopying);
			}
		}
		public bool EnableScreenReaders {
			get {
				return enableScreenReaders == true;
			}
			set {
				if(enableScreenReaders == value)
					return;
				enableScreenReaders = value;
				RaisePropertyChanged(() => EnableScreenReaders);
			}
		}
		public bool OpenPasswordEnabled {
			get {
				return openPasswordEnabled;
			}
			set {
				if(openPasswordEnabled == value)
					return;
				openPasswordEnabled = value;
				RaisePropertyChanged(() => OpenPasswordEnabled);
			}
		}
		public bool IsPermissionsEnabled {
			get {
				return isPermissionsEnabled;
			}
			set {
				if(isPermissionsEnabled == value)
					return;
				isPermissionsEnabled = value;
				RaisePropertyChanged(() => IsPermissionsEnabled);
			}
		}
		#endregion
		public event EventHandler RequireOpenPasswordChanged;
		public event EventHandler RestrictPermissionsChanged;
		public event EventHandler<RepeatPasswordCompleteEventArgs> RepeatPasswordComplete;
		public void EnableControl_OpenPassword(bool enable) {
			OpenPasswordEnabled = enable;
		}
		public void EnableControlGroup_Permissions(bool enable) {
			IsPermissionsEnabled = enable;
		}
		public void InitializeChangingPermissions(IEnumerable<KeyValuePair<ChangingPermissions, string>> availablePermissions) {
			ChangingPermissionsValues = availablePermissions;
		}
		public void InitializePrintingPermissions(IEnumerable<KeyValuePair<PrintingPermissions, string>> availablePermissions) {
			PrintingPermissionsValues = availablePermissions;
		}
		public void RepeatOpenPassword() {
			RepeatPassword(
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_OpenPassword_Title),
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_OpenPassword_Note),
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_OpenPassword));
		}
		public void RepeatPermissionsPassword() {
			RepeatPassword(
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_PermissionsPassword_Title),
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_PermissionsPassword_Note),
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_PermissionsPassword));
		}
		public void PasswordDoesNotMatch() {
			MessageBox.Show(
				PrintingLocalizer.GetString(PrintingStringId.RepeatPassword_ConfirmationPasswordDoesNotMatch),
				PrintingLocalizer.GetString(PrintingStringId.PdfPasswordSecurityOptions_Title),
				MessageBoxButton.OK);
		}
		public void RepeatPassword(string caption, string note, string passwordName) {
			RepeatPasswordWindow window = new RepeatPasswordWindow(caption, note, passwordName);
			window.FlowDirection = FlowDirection;
			string repeatedPassword = null;
			if(window.ShowDialog() == true)
				repeatedPassword = window.password.Password;
			if(RepeatPasswordComplete != null)
				RepeatPasswordComplete(this, new RepeatPasswordCompleteEventArgs(repeatedPassword));
		}
		#endregion
		protected void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
