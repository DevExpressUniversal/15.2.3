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

using System.Web.UI;
using DevExpress.Web.ASPxSpreadsheet;
using DevExpress.Web.ASPxSpreadsheet.Internal.Forms;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxSpreadsheetFileManager : SpreadsheetFileManager {
		public override bool IsLoading() {
			return false;
		}
		public MVCxUploadControl GetUploadControl() {
			return Helper.UploadControl as MVCxUploadControl;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxSpreadsheetFileManager), Utils.FileManagerScriptResourceName);
		}
		protected override ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new MVCxSpreadsheetFileManagerUploadControl();
		}
	}
	public class MVCxSpreadsheetFileManagerUploadControl : MVCxSpreadsheetUploadControl {
		protected override void PrepareControl() {
			base.PrepareControl();
			ValidationSettings.AllowedFileExtensions = new string[] { ".xlsx", ".xlsm", ".xls", ".csv", ".ods" };
		}
		protected override string GetClientObjectClassName() {
			return "MVCx.FileManagerUploadControl";
		}
	}
	public class MVCxSpreadsheetUploadControl : MVCxUploadControl, IDialogFormElementRequiresLoad {
		public MVCxSpreadsheetUploadControl()
			: base() {
			this.OwnerControl = FindParentSpreadsheetControl();
		}
		protected MVCxSpreadsheet FindParentSpreadsheetControl() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is MVCxSpreadsheet)
					return curControl as MVCxSpreadsheet;
				curControl = curControl.Parent;
			}
			return null;
		}
		void IDialogFormElementRequiresLoad.ForceInit() { }
		void IDialogFormElementRequiresLoad.ForceLoad() {
			PrepareControl();
		}
		protected virtual void PrepareControl() {
			MVCxSpreadsheet spreadsheet = FindParentSpreadsheetControl();
			if(spreadsheet != null) {
				CallbackRouteValues = spreadsheet.CallbackRouteValues;
				ValidationSettings.AllowedFileExtensions = new string[] { ".bmp", ".dib", ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
			}
		}
	}
}
namespace DevExpress.Web.Mvc.Spreadsheet.Forms {
	public interface IMvcSpreadsheetDialogRequiresUpload {
		MVCxUploadControl GetChildUploadControl();
	}
	public class MVCxInsertImageDialog : InsertImageDialog, IMvcSpreadsheetDialogRequiresUpload {
		MVCxUploadControl IMvcSpreadsheetDialogRequiresUpload.GetChildUploadControl() {
			return ImageUpload as MVCxUploadControl;
		}
	}
	public class MVCxOpenFileDialog : OpenFileDialog, IMvcSpreadsheetDialogRequiresUpload {
		MVCxUploadControl IMvcSpreadsheetDialogRequiresUpload.GetChildUploadControl() {
			return (SSFileManager as MVCxSpreadsheetFileManager).GetUploadControl();
		}
	}
}
