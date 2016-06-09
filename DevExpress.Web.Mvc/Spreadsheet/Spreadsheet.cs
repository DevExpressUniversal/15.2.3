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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using System.IO;
	using System.Web.Mvc;
	using DevExpress.Web.ASPxSpreadsheet;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.Spreadsheet.Forms;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxSpreadsheet : ASPxSpreadsheet {
		string callbackDialogAction;
		ViewContext viewContext;
		public MVCxSpreadsheet()
			: this(null) {
		}
		protected internal MVCxSpreadsheet(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public object CallbackRouteValues { get; set; }
		[Obsolete("This property is now obsolete."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object DownloadRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected string CallbackDialogAction {
			get { return callbackDialogAction; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ControllerBase Controller {
			get { return (ViewContext != null) ? ViewContext.Controller : null; }
		}
		protected internal ControllerContext ControllerContext {
			get { return (Controller != null) ? Controller.ControllerContext : null; }
		}
		protected internal TextWriter Writer {
			get { return (ViewContext != null) ? ViewContext.Writer : Utils.Writer; }
		}
		protected internal void LoadWorkSessionFromRequest() {
			LoadWorkSessionIdFromRequest();
		}
		protected internal void PerformOnLoad() {
			LoadDocumentClientState();
			CheckWorkDirectoryAccess();
		}
		protected internal new string GetCustomeCallbackResult(string callbackArgs) {
			return base.GetCustomeCallbackResult(callbackArgs);
		}
		protected override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null && base.IsCallBacksEnabled();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientSpreadsheet";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxSpreadsheet), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxSpreadsheet), Utils.SpreadsheetScriptResourceName);
		}
		protected override SpreadsheetDocumentSelectorSettings CreateSettingsDocumentSelector() {
			return new SpreadsheetDocumentSelectorSettings(null);
		}
		protected override SpreadsheetFormsSettings CreateSettingsForms() {
			return new MVCxSpreadsheetFormsSettings(this);
		}
		protected override Control CreateUserControl(string virtualPath) {
			DummyPage page = new DummyPage();
			return page.LoadControl(virtualPath);
		}
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			DialogHelper.ForceOnLoad(userControl);
		}
		protected override void CreateDialogs() { }
		protected override Control CreateOpenFileDialog() {
			return new MVCxOpenFileDialog();
		}
		protected override SpreadsheetFileManager CreateFileManager() {
			return new MVCxSpreadsheetFileManager();
		}
		protected override Control CreateInsertPictureDialog() {
			return new MVCxInsertImageDialog();
		}
		protected override ASPxUploadControl CreateUploadControl() {
			return new MVCxSpreadsheetUploadControl();
		}
		protected internal MVCxUploadControl GetCurrentDialogUploadControl() {
			IMvcSpreadsheetDialogRequiresUpload currentDialog = GetCurrentDialogControl(CurrentDialogName) as IMvcSpreadsheetDialogRequiresUpload;
			return currentDialog != null ? currentDialog.GetChildUploadControl() : null;
		}
		protected internal void RenderCallbackResultControl() {
			if(string.IsNullOrEmpty(CallbackDialogAction)) return;
			if(Writer != null) {
				CustomActionInvoker c = new CustomActionInvoker();
				c.InvokeAction(ControllerContext, CallbackDialogAction);
				Writer.Write(c.ActionResult);
			}
		}
		protected override Control GetCurrentDialogControl(string dialogName) {
			Control dialogControl;
			string dialogFormName = DialogFormNamesDictionary[CurrentDialogName];
			string dialogAction = ((MVCxSpreadsheetFormsSettings)SettingsForms).GetFormAction(dialogFormName);
			if(string.IsNullOrEmpty(dialogAction)) {
				dialogControl = CreateDefaultForm(dialogFormName);
				PrepareUserControl(dialogControl, ContainerControl, CurrentDialogName, true);
			} else {
				this.callbackDialogAction = dialogAction;
				dialogControl = new LiteralControl(Utils.CallbackHtmlContentPlaceholder);
				PrepareUserControl(dialogControl, ContainerControl, dialogName, false);
			}
			RenderUtils.LoadPostDataRecursive(dialogControl, PostDataCollection);
			return dialogControl;
		}
		public override bool IsInternalServiceCallback() {
			string callbackParams = MvcUtils.CallbackArgument;
			return (MvcUtils.CallbackName == ID && !string.IsNullOrEmpty(callbackParams) && callbackParams.EndsWith(InternalCallbackPostfix)) || IsInternalUploadControlPostback();
		}
		protected override bool IsInternalUploadControlPostback() {
			string uploadControlID = PostDataCollection[RenderUtils.UploadingCallbackQueryParamName] ??
				PostDataCollection[RenderUtils.HelperUploadingCallbackQueryParamName];
			if(!string.IsNullOrEmpty(uploadControlID))
				return GetCurrentDialogUploadControl() != null;
			return false;
		}
	}
}
