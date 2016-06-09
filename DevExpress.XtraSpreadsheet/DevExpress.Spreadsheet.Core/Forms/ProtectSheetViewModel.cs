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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ProtectSheetViewModel
	public class ProtectSheetViewModel : ViewModelBase {
		readonly ISpreadsheetControl control;
		readonly DocumentModel documentModel;
		readonly WorksheetProtectionOptions protection;
		readonly List<ProtectedSheetPermission> permissions;
		readonly Worksheet worksheet;
		bool isProtected = true;
		string password = String.Empty;
		public ProtectSheetViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.documentModel = control.InnerControl.DocumentModel;
			this.protection = documentModel.ActiveSheet.Properties.Protection;
			this.worksheet = documentModel.ActiveSheet;
			this.permissions = new List<ProtectedSheetPermission>();
			PopulatePermissions();
		}
		#region Properties
		#region IsProtected
		public bool IsProtected {
			get { return isProtected; }
			set {
				if (IsProtected == value)
					return;
				this.isProtected = value;
				OnPropertyChanged("IsProtected");
			}
		}
		#endregion
		#region Password
		public string Password {
			get { return password; }
			set {
				if (Password == value)
					return;
				this.password = value;
				OnPropertyChanged("Password");
			}
		}
		#endregion
		public List<ProtectedSheetPermission> Permissions { get { return permissions; } }
		WorksheetProtectionOptions Protection { get { return protection; } }
		DocumentModel DocumentModel { get { return documentModel; } }
		public ISpreadsheetControl Control { get { return control; } }
		#endregion
		void AppendPermission(XtraSpreadsheetStringId nameId, bool isAllowed) {
			permissions.Add(new ProtectedSheetPermission(XtraSpreadsheetLocalizer.GetString(nameId), isAllowed));
		}
		void PopulatePermissions() {
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionSelectLockedCells, !Protection.SelectLockedCellsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionSelectUnlockedCells, !Protection.SelectUnlockedCellsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionFormatCells, !Protection.FormatCellsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionFormatColumns, !Protection.FormatColumnsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionFormatRows, !Protection.FormatRowsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionInsertColumns, !Protection.InsertColumnsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionInsertRows, !Protection.InsertRowsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionInsertHyperlinks, !Protection.InsertHyperlinksLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionDeleteColumns, !Protection.DeleteColumnsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionDeleteRows, !Protection.DeleteRowsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionSort, !Protection.SortLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionAutoFilter, !Protection.AutoFiltersLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionPivotTable, !Protection.PivotTablesLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionEditObjects, !Protection.ObjectsLocked);
			AppendPermission(XtraSpreadsheetStringId.Caption_PermissionEditScenarios, !Protection.ScenariosLocked);
		}
		void ApplyPermissions() {
			Protection.SelectLockedCellsLocked = !Permissions[0].IsAllowed;
			Protection.SelectUnlockedCellsLocked = !Permissions[1].IsAllowed;
			Protection.FormatCellsLocked = !Permissions[2].IsAllowed;
			Protection.FormatColumnsLocked = !Permissions[3].IsAllowed;
			Protection.FormatRowsLocked = !Permissions[4].IsAllowed;
			Protection.InsertColumnsLocked = !Permissions[5].IsAllowed;
			Protection.InsertRowsLocked = !Permissions[6].IsAllowed;
			Protection.InsertHyperlinksLocked = !Permissions[7].IsAllowed;
			Protection.DeleteColumnsLocked = !Permissions[8].IsAllowed;
			Protection.DeleteRowsLocked = !Permissions[9].IsAllowed;
			Protection.SortLocked = !Permissions[10].IsAllowed;
			Protection.AutoFiltersLocked = !Permissions[11].IsAllowed;
			Protection.PivotTablesLocked = !Permissions[12].IsAllowed;
			Protection.ObjectsLocked = !Permissions[13].IsAllowed;
			Protection.ScenariosLocked = !Permissions[14].IsAllowed;
		}
		public void ApplyChanges() {
			DocumentModel.BeginUpdateFromUI();
			try {
				Protection.BeginUpdate();
				try {
					ApplyPermissions();
					worksheet.Protect(Password);
				}
				finally {
					Protection.EndUpdate();
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
			documentModel.History.Clear();
		}
	}
	#endregion
	#region ProtectedSheetPermission
	public class ProtectedSheetPermission : ViewModelBase {
		string name;
		bool isAllowed;
		public ProtectedSheetPermission(string name, bool isAllowed) {
			this.Name = name;
			this.IsAllowed = isAllowed;
		}
		#region Properties
		#region Name
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				this.name = value;
				OnPropertyChanged("Name");
			}
		}
		#endregion
		#region IsAllowed
		public bool IsAllowed {
			get { return isAllowed; }
			set {
				if (IsAllowed == value)
					return;
				this.isAllowed = value;
				OnPropertyChanged("IsAllowed");
			}
		}
		#endregion
		#endregion
	}
	#endregion
}
