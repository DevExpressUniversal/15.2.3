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
using DevExpress.Web.ASPxRichEdit.Forms;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxRichEditFileManager : ASPxRichEdit.RichEditFileManager {
		public override bool IsLoading() {
			return false;
		}
		public MVCxUploadControl GetUploadControl() {
			return (MVCxUploadControl)Helper.UploadControl;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxRichEditFileManager), Utils.FileManagerScriptResourceName);
		}
		protected internal override ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new MVCxRichEditFileManagerUploadControl();
		}
	}
	public class MVCxRichEditFileManagerUploadControl : MVCxRichEditUploadControl {
		protected override string GetClientObjectClassName() {
			return "MVCx.FileManagerUploadControl";
		}
	}
	public class MVCxRichEditUploadControl : MVCxUploadControl, IDialogFormElementRequiresLoad {
		public MVCxRichEditUploadControl()
			: base() {
			this.OwnerControl = FindParentRichEditControl();
		}
		void IDialogFormElementRequiresLoad.ForceInit() { }
		void IDialogFormElementRequiresLoad.ForceLoad() {
			MVCxRichEdit richEdit = FindParentRichEditControl();
			if(richEdit != null)
				CallbackRouteValues = richEdit.CallbackRouteValues;
		}
		protected MVCxRichEdit FindParentRichEditControl() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is MVCxRichEdit)
					return curControl as MVCxRichEdit;
				curControl = curControl.Parent;
			}
			return null;
		}
	}
}
namespace DevExpress.Web.Mvc.RichEdit.Forms {
	public interface IMvcRichEditDialogRequiresUpload {
		MVCxUploadControl GetChildUploadControl();
	}
	public class MVCxInsertImageForm : InsertImageForm, IMvcRichEditDialogRequiresUpload {
		protected override ASPxUploadControl CreateUploadControl() {
			return new MVCxRichEditUploadControl();
		}
		MVCxUploadControl IMvcRichEditDialogRequiresUpload.GetChildUploadControl() {
			return (MVCxUploadControl)UploadControl;
		}
	}
	public class MVCxOpenFileForm : OpenFileForm, IMvcRichEditDialogRequiresUpload {
		protected override ASPxFileManager CreateFileManager() {
			return new MVCxRichEditFileManager();
		}
		MVCxUploadControl IMvcRichEditDialogRequiresUpload.GetChildUploadControl() {
			return ((MVCxRichEditFileManager)FileManager).GetUploadControl();
		}
	}
}
