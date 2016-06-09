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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxRichEdit;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.RichEdit.Forms;
	[ToolboxItem(false)]
	public class MVCxRichEdit : ASPxRichEdit {
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			DialogHelper.ForceOnLoad(userControl);
		}
		protected override void CreateDialogs() {}
		protected override Control CreateInsertImageDialog() {
			return new MVCxInsertImageForm();
		}
		protected override Control CreateOpenFileDialog() {
			return new MVCxOpenFileForm();
		}
		protected internal MVCxUploadControl GetCurrentDialogUploadControl() {
			IMvcRichEditDialogRequiresUpload currentDialog = GetCurrentDialogControl(CurrentDialogName) as IMvcRichEditDialogRequiresUpload;
			return currentDialog != null ? currentDialog.GetChildUploadControl() : null;
		}
		protected override Control GetCurrentDialogControl(string dialogName) {
			Control dialogControl = CreateDialogFromControl(CurrentDialogName, this);
			RenderUtils.LoadPostDataRecursive(dialogControl, PostDataCollection);
			return dialogControl;
		}
		public override bool IsCallback { get { return MvcUtils.CallbackName == ID; } }
		protected override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null && base.IsCallBacksEnabled();
		}
		protected internal new object GetCustomCallbackResult() { return base.GetCustomCallbackResult(); }
		protected internal new void LoadWorkSessionIdFromRequest() { base.LoadWorkSessionIdFromRequest(); }
		protected internal new void CheckWorkDirectoryAccess() { base.CheckWorkDirectoryAccess(); }
		protected internal new void ApplyRequestCommands() { base.ApplyRequestCommands(); }
		protected internal void AssignWorkDirectoryFromWorkSession() {
			if(CurrentSession == null || !CurrentSession.HasClient(ClientGuid))
				return;
			var client = CurrentSession.GetClient(ClientGuid);
			WorkDirectory = client.WorkDirectory;
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientRichEdit";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxRichEdit), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxRichEdit), Utils.RichEditScriptResourceName);
		}
	}
}
