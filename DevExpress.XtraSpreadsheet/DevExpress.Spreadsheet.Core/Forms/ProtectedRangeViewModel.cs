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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ProtectedRangeViewModel
	public class ProtectedRangeViewModel : ViewModelBase, IReferenceEditViewModel {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly IList<string> existingProtectedRangeNames = new List<string>();
		ModelProtectedRange protectedRange;
		string originalTitle = String.Empty;
		string title = String.Empty;
		string reference = String.Empty;
		string securityDescriptor = String.Empty;
		string password = String.Empty;
		bool isNew;
		bool isModified;
		bool isDeleted;
		bool hasPassword;
		#endregion
		public ProtectedRangeViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public string OriginalTitle {
			get { return originalTitle; }
			set {
				if (OriginalTitle == value)
					return;
				this.originalTitle = value;
				OnPropertyChanged("OriginalTitle");
			}
		}
		public string Title {
			get { return title; }
			set {
				if (Title == value)
					return;
				this.title = value;
				OnPropertyChanged("Title");
			}
		}
		public string Reference {
			get { return reference; }
			set {
				if (Reference == value)
					return;
				this.reference = value;
				OnPropertyChanged("Reference");
			}
		}
		public string SecurityDescriptor {
			get { return securityDescriptor; }
			set {
				if (SecurityDescriptor == value)
					return;
				this.securityDescriptor = value;
				OnPropertyChanged("SecurityDescriptor");
			}
		}
		public string Password {
			get { return password; }
			set {
				if (Password == value)
					return;
				this.password = value;
				this.PasswordChanged = true;
				OnPropertyChanged("Password");
			}
		}
		public bool IsNew {
			get { return isNew; }
			set {
				if (IsNew == value)
					return;
				this.isNew = value;
				OnPropertyChanged("IsNew");
			}
		}
		public bool IsModified {
			get { return isModified; }
			set {
				if (IsModified == value)
					return;
				this.isModified = value;
				OnPropertyChanged("IsModified");
			}
		}
		public bool IsDeleted {
			get { return isDeleted; }
			set {
				if (IsDeleted == value)
					return;
				this.isDeleted = value;
				OnPropertyChanged("IsDeleted");
			}
		}
		public bool HasPassword {
			get { return hasPassword; }
			set {
				if (HasPassword == value)
					return;
				this.hasPassword = value;
				OnPropertyChanged("HasPassword");
				OnPropertyChanged("AllowToSetNewPassword");
			}
		}
		public bool PasswordChanged { get; set; }
		public IList<string> ExistingProtectedRangeNames { get { return existingProtectedRangeNames; } }
		public bool AllowToSetNewPassword { get { return !HasPassword; } }
		public string FormText { get { return IsModified ? XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_EditProtectedRangeFormTitle) : XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_NewProtectedRangeFormTitle); } }
		protected internal ModelProtectedRange ProtectedRange { get { return protectedRange; } set { protectedRange = value; } }
		#endregion
		public void UpdateHasPassword() {
			HasPassword = (ProtectedRange != null && !ProtectedRange.Credentials.IsEmpty) || !String.IsNullOrEmpty(Password);
		}
		public ProtectedRangeViewModel Clone() {
			ProtectedRangeViewModel result = new ProtectedRangeViewModel(Control);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ProtectedRangeViewModel other) {
			this.ProtectedRange = other.ProtectedRange;
			this.OriginalTitle = other.OriginalTitle;
			this.Title = other.Title;
			this.Reference = other.Reference;
			this.SecurityDescriptor = other.SecurityDescriptor;
			this.Password = other.Password;
			this.IsNew = other.IsNew;
			this.IsModified = other.IsModified;
			this.IsDeleted = other.IsDeleted;
			this.HasPassword = other.HasPassword;
			this.PasswordChanged = other.PasswordChanged;
		}
		public bool Validate() {
			if (string.IsNullOrEmpty(Title)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeEmptyName));
				return false;
			}
			if (!ModelProtectedRange.IsValidName(Title)) {
				Control.ShowWarningMessage(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeInvalidName), Title));
				return false;
			}
			int count = ExistingProtectedRangeNames.Count;
			for (int i = 0; i < count; i++) {
				if (ExistingProtectedRangeNames[i] == Title && ExistingProtectedRangeNames[i] != OriginalTitle) {
					Control.ShowWarningMessage(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeDuplicateName), Title));
					return false;
				}
			}
			return true;
		}
		public void EditPermissions() {
			ProtectedRangePermissionsViewModel permissionsViewModel = new ProtectedRangePermissionsViewModel(Control);
			permissionsViewModel.Name = Title;
			permissionsViewModel.SecurityDescriptor = SecurityDescriptor;
			Control.ShowProtectedRangePermissionsForm(permissionsViewModel);
			SecurityDescriptor = permissionsViewModel.SecurityDescriptor;
		}
		public void ApplyChanges() {
		}
	}
	#endregion
}
