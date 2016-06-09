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
	#region ProtectWorkbookViewModel
	public class ProtectWorkbookViewModel : ViewModelBase {
		readonly ISpreadsheetControl control;
		readonly DocumentModel documentModel;
		readonly WorkbookProtectionOptions protection;
		bool isStructureLocked;
		bool isWindowsLocked;
		string password = String.Empty;
		public ProtectWorkbookViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.documentModel = control.InnerControl.DocumentModel;
			this.protection = documentModel.Properties.Protection;
			IsStructureLocked = Protection.LockStructure;
			IsWindowsLocked = Protection.LockWindows;
			if (!IsStructureLocked && !IsWindowsLocked)
				IsStructureLocked = true;
		}
		#region Properties
		#region IsStructureLocked
		public bool IsStructureLocked {
			get { return isStructureLocked; }
			set {
				if (IsStructureLocked == value)
					return;
				this.isStructureLocked = value;
				OnPropertyChanged("IsStructureLocked");
			}
		}
		#endregion
		#region IsWindowsLocked
		public bool IsWindowsLocked {
			get { return isWindowsLocked; }
			set {
				if (IsWindowsLocked == value)
					return;
				this.isWindowsLocked = value;
				OnPropertyChanged("IsWindowsLocked");
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
		DocumentModel DocumentModel { get { return documentModel; } }
		WorkbookProtectionOptions Protection { get { return protection; } }
		public ISpreadsheetControl Control { get { return control; } }
		#endregion
		public void ApplyChanges() {
			DocumentModel.BeginUpdateFromUI();
			try {
				DocumentModel.Protect(Password, IsStructureLocked, IsWindowsLocked);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
	}
	#endregion
}
