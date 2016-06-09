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
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxHtmlEditor;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxHtmlEditor: ASPxHtmlEditor, IViewContext{
		string callbackDialogAction;
		ViewContext viewContext;
		public MVCxHtmlEditor()
			: this(null) {
		}
		protected internal MVCxHtmlEditor(ViewContext viewContext)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomDataActionRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public new MVCxHtmlEditorDefaultDialogSettings SettingsDialogs {
			get { return (MVCxHtmlEditorDefaultDialogSettings)base.SettingsDialogs; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageUpload property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorImageUploadSettings SettingsImageUpload {
			get { return (MVCxHtmlEditorImageUploadSettings)SettingsDialogs.InsertImageDialog.SettingsImageUpload; }
		}
		[Obsolete("Use the SettingsDialogs.InsertAudioDialog.SettingsAudioUpload property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorAudioUploadSettings SettingsAudioUpload {
			get { return (MVCxHtmlEditorAudioUploadSettings)SettingsDialogs.InsertAudioDialog.SettingsAudioUpload; }
		}
		[Obsolete("Use the SettingsDialogs.InsertVideoDialog.SettingsVideoUpload property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorVideoUploadSettings SettingsVideoUpload {
			get { return (MVCxHtmlEditorVideoUploadSettings)SettingsDialogs.InsertVideoDialog.SettingsVideoUpload; }
		}
		[Obsolete("Use the SettingsDialogs.InsertFlashDialog.SettingsFlashUpload property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorFlashUploadSettings SettingsFlashUpload {
			get { return (MVCxHtmlEditorFlashUploadSettings)SettingsDialogs.InsertFlashDialog.SettingsFlashUpload; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorImageSelectorSettings SettingsImageSelector {
			get { return (MVCxHtmlEditorImageSelectorSettings)SettingsDialogs.InsertImageDialog.SettingsImageSelector; }
		}
		[Obsolete("Use the SettingsDialogs.InsertAudioDialog.SettingsAudioSelector property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorAudioSelectorSettings SettingsAudioSelector {
			get { return (MVCxHtmlEditorAudioSelectorSettings)SettingsDialogs.InsertAudioDialog.SettingsAudioSelector; }
		}
		[Obsolete("Use the SettingsDialogs.InsertFlashDialog.SettingsFlashSelector property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorFlashSelectorSettings SettingsFlashSelector {
			get { return (MVCxHtmlEditorFlashSelectorSettings)SettingsDialogs.InsertFlashDialog.SettingsFlashSelector; }
		}
		[Obsolete("Use the SettingsDialogs.InsertVideoDialog.SettingsVideoSelector property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorVideoSelectorSettings SettingsVideoSelector {
			get { return (MVCxHtmlEditorVideoSelectorSettings)SettingsDialogs.InsertVideoDialog.SettingsVideoSelector; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorDocumentSelectorSettings SettingsDocumentSelector {
			get { return (MVCxHtmlEditorDocumentSelectorSettings)SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector; }
		}
		public new HtmlEditorImages Images {
			get { return base.Images; }
		}
		public new HtmlEditorStyles Styles {
			get { return base.Styles; }
		}
		protected string CallbackDialogAction {
			get { return callbackDialogAction; }
		}
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal new RoundPanelParts PartsRoundPanelInternal {
			get { return base.PartsRoundPanelInternal; }
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
		protected internal new bool IsRightToLeft() {
			return base.IsRightToLeft();
		}
		protected override bool BeforeSkinApplying() {
			return false;
		}
		protected internal new void OnWidthSet(bool beforeSkinApplying, Unit width) {
			base.OnWidthSet(beforeSkinApplying, width);
		}
		protected internal new void RestoreWidth() {
			base.RestoreWidth();
		}
		protected override void CreateDialogBlock() { }
		protected override Control CreateUserControl(string virtualPath) {
			DummyPage page = new DummyPage();
			return page.LoadControl(virtualPath);
		}
		protected override void PrepareUserControl(Control userControl, Control parent, string id, bool builtInControl) {
			DialogHelper.ForceOnInit(userControl);
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			DialogHelper.ForceOnLoad(userControl);
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected internal new string CurrentDialogName { get { return base.CurrentDialogName; } }
		protected override Control GetCurrentDialogControl(string dialogName) {
			Control dialogControl = null;
			if(DialogFormNameDictionary.ContainsKey(dialogName)) {  
				string dialogFormName = DialogFormNameDictionary[dialogName];
				string dialogAction = ((MVCxHtmlEditorFormsSettings)SettingsForms).GetFormAction(dialogFormName);
				if(string.IsNullOrEmpty(dialogAction)) {
					dialogControl = CreateDefaultForm(dialogFormName);
					PrepareUserControl(dialogControl, ContainerControl, dialogName, true);
				}
				else {
					this.callbackDialogAction = dialogAction;
					dialogControl = new LiteralControl(Utils.CallbackHtmlContentPlaceholder);
					PrepareUserControl(dialogControl, ContainerControl, dialogName, false);
				}
			}
			else { 
				string customDialogName = dialogName.Substring(CustomDialogNamePrefix.Length);
				MVCxHtmlEditorCustomDialog dialog = CustomDialogs[customDialogName] as MVCxHtmlEditorCustomDialog;
				if(dialog != null) {
					if(string.IsNullOrEmpty(dialog.FormAction))
						throw new Exception(StringResources.HtmlEditorExceptionText_CustomDialogFormNameNotSpecified);
					this.callbackDialogAction = dialog.FormAction;
					dialogControl = new CustomDialogsContainer(dialog, new LiteralControl(Utils.CallbackHtmlContentPlaceholder));
					PrepareUserControl(dialogControl, ContainerControl, dialog.Name, false);
				}
				else
					throw new Exception(StringResources.HtmlEditorExceptionText_CustomDialogNotFound);
			}
			RenderUtils.LoadPostDataRecursive(dialogControl, System.Web.HttpContext.Current.Request.Params);
			return dialogControl;
		}
		protected internal void RenderCallbackResultControl() {
			if(string.IsNullOrEmpty(CallbackDialogAction)) return;
			if(Writer != null) {
				CustomActionInvoker c = new CustomActionInvoker();
				c.InvokeAction(ControllerContext, CallbackDialogAction);
				Writer.Write(c.ActionResult);
			}
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CustomDataActionRouteValues != null)
				stb.Append(localVarName + ".customDataActionUrl=\"" + Utils.GetUrl(CustomDataActionRouteValues) + "\";\n");
			if(ExportRouteValues != null)
				stb.Append(localVarName + ".exportUrl=\"" + Utils.GetUrl(ExportRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientHtmlEditor";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxHtmlEditor), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxHtmlEditor), Utils.HtmlEditorScriptResourceName);
		}
		internal void UploadFileToFileManager(string dialogName) {
			(GetDialogFileManager(dialogName) as MVCxHtmlEditorFileManager).UploadFile();
		}
		public override bool IsLoading() {
			return false;
		}
		protected override HtmlEditorCustomDialogs CreateCustomDialogs() {
			return new MVCxHtmlEditorCustomDialogs(this);
		}
		protected override HtmlEditorSpellChecker CreateSpellCheckerInstance() {
			return new MVCxHtmlEditorSpellChecker(this);
		}
		protected override HtmlEditorDefaultDialogSettings CreateSettingsDialogs() {
			return new MVCxHtmlEditorDefaultDialogSettings(this);
		}
		protected override HtmlEditorDialogFormElementSettings CreateSettingsDialogFormElement() {
			return new MVCxHtmlEditorDialogFormElementSettings(this);
		}
		protected override HtmlEditorFormsSettings CreateSettingsForms() {
			return new MVCxHtmlEditorFormsSettings(this);
		}
		[Obsolete("Use the SettingsDocumentSelector.FolderCreating property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerFolderCreateEventHandler DocumentSelectorFolderCreating { add { } remove { } }
		[Obsolete("Use the SettingsDocumentSelector.ItemRenaming property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemRenameEventHandler DocumentSelectorItemRenaming { add { } remove { } }
		[Obsolete("Use the SettingsDocumentSelector.ItemDeleting  property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemDeleteEventHandler DocumentSelectorItemDeleting { add { } remove { } }
		[Obsolete("Use the SettingsDocumentSelector.ItemMoving property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemMoveEventHandler DocumentSelectorItemMoving { add { } remove { } }
		[Obsolete("Use the SettingsDocumentSelector.FileUploading property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerFileUploadEventHandler DocumentSelectorFileUploading { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FolderCreating != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FolderCreating(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemRenaming != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemRenaming(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemCopying != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemCopying(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemDeleting != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemDeleting(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemMoving != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemMoving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileUploading != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileUploading(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseDocumentSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			if(SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.CustomThumbnail != null)
				SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.CustomThumbnail(this, args);
		}
		[Obsolete("Use the SettingsImageSelector.FolderCreating property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerFolderCreateEventHandler ImageSelectorFolderCreating { add { } remove { } }
		[Obsolete("Use the SettingsImageSelector.ItemRenaming property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemRenameEventHandler ImageSelectorItemRenaming { add { } remove { } }
		[Obsolete("Use the SettingsImageSelector.ItemDeleting property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemDeleteEventHandler ImageSelectorItemDeleting { add { } remove { } }
		[Obsolete("Use the SettingsImageSelector.ItemMoving property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerItemMoveEventHandler ImageSelectorItemMoving { add { } remove { } }
		[Obsolete("Use the SettingsImageSelector.FileUploading property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event FileManagerFileUploadEventHandler ImageSelectorFileUploading { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.FolderCreating != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.FolderCreating(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemRenaming != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemRenaming(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemCopying != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemCopying(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemMoving != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemMoving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileUploading != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileUploading(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.CustomThumbnail != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.CustomThumbnail(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseImageFileSaving(FileSavingEventArgs args) {
			if(SettingsDialogs.InsertImageDialog.SettingsImageSelector.ImageFileSaving != null)
				SettingsDialogs.InsertImageDialog.SettingsImageSelector.ImageFileSaving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.FolderCreating != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.FolderCreating(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemRenaming != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemRenaming(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemCopying != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemCopying(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemDeleting != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemDeleting(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemMoving != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ItemMoving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.FileUploading != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.FileUploading(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.CustomThumbnail != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.CustomThumbnail(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseAudioFileSaving(FileSavingEventArgs args) {
			if(SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.AudioFileSaving != null)
				SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.AudioFileSaving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FolderCreating != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FolderCreating(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemRenaming != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemRenaming(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemCopying != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemCopying(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemDeleting != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemDeleting(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemMoving != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ItemMoving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FileUploading != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FileUploading(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.CustomThumbnail != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.CustomThumbnail(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseFlashFileSaving(FileSavingEventArgs args) {
			if(SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FlashFileSaving != null)
				SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FlashFileSaving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FolderCreating != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FolderCreating(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemRenaming != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemRenaming(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemCopying != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemCopying(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemDeleting != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemDeleting(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemMoving != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ItemMoving(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FileUploading != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FileUploading(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoSelectorCustomThumbnail(FileManagerThumbnailCreateEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.CustomThumbnail != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.CustomThumbnail(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RaiseVideoFileSaving(FileSavingEventArgs args) {
			if(SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.VideoFileSaving != null)
				SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.VideoFileSaving(this, args);
		}
		protected override MediaFileSelector CreateMediaFileSelector() {
			return new MVCxHtmlEditorMediaFileSelector();
		}
		protected override HtmlEditorFileManager CreateFileManager() {
			return new MVCxHtmlEditorFileManager();
		}
		protected internal void ExecuteLoadPostedHtml() {
			LoadPostedHtml();
		}
		#region IViewContext Members
		ViewContext IViewContext.ViewContext { get { return ViewContext; } }
		#endregion
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public class CustomActionInvoker : ControllerActionInvoker {
		StringWriter writer = new StringWriter();
		public string ActionResult {
			get { return writer.ToString(); }
		}
		protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue) {
			if(actionReturnValue is ActionResult) {
				ViewResultBase viewResult = actionReturnValue as ViewResultBase;
				if(viewResult != null) {
					if(actionReturnValue is PartialViewResult)
						return new CustomPartialViewResult(this.writer) {
							TempData = viewResult.TempData,
							View = viewResult.View,
							ViewData = viewResult.ViewData,
							ViewEngineCollection = viewResult.ViewEngineCollection,
							ViewName = viewResult.ViewName
						};
					else if(actionReturnValue is ViewResult)
						return new CustomViewResult(this.writer) {
							TempData = viewResult.TempData,
							View = viewResult.View,
							ViewData = viewResult.ViewData,
							ViewEngineCollection = viewResult.ViewEngineCollection,
							ViewName = viewResult.ViewName
						};
				}
				else if(actionReturnValue is ContentResult)
					this.writer.Write(((ContentResult)actionReturnValue).Content);
			}
			else if(actionReturnValue != null) { 
				this.writer.Write(Convert.ToString(actionReturnValue, CultureInfo.InvariantCulture));
			}
			return new EmptyResult();
		}
		public static void InitViewResult(ControllerContext context, ViewResultBase viewResult) {
			if(context == null)
				throw new ArgumentNullException("context");
			if(string.IsNullOrEmpty(viewResult.ViewName))
				viewResult.ViewName = context.RouteData.GetRequiredString("action");
		}
		public static void RenderViewResult(ControllerContext context, ViewResultBase viewResult, ViewEngineResult viewEngineResult, TextWriter writer) {
			ViewContext viewContext = new ViewContext(context, viewResult.View, viewResult.ViewData, viewResult.TempData, writer);
			viewResult.View.Render(viewContext, writer);
			if(viewEngineResult != null)
				viewEngineResult.ViewEngine.ReleaseView(context, viewResult.View);
		}
	}
	public class CustomPartialViewResult : PartialViewResult {
		TextWriter writer;
		public CustomPartialViewResult(TextWriter writer) {
			this.writer = writer;
		}
		public override void ExecuteResult(ControllerContext context) {
			CustomActionInvoker.InitViewResult(context, this);
			ViewEngineResult result = null;
			if(View == null) {
				result = FindView(context);
				View = result.View;
			}
			CustomActionInvoker.RenderViewResult(context, this, result, this.writer);
		}
	}
	public class CustomViewResult : ViewResult {
		TextWriter writer;
		public CustomViewResult(TextWriter writer) {
			this.writer = writer;
		}
		public override void ExecuteResult(ControllerContext context) {
			CustomActionInvoker.InitViewResult(context, this);
			ViewEngineResult result = null;
			if(View == null) {
				result = FindView(context);
				View = result.View;
			}
			CustomActionInvoker.RenderViewResult(context, this, result, this.writer);
		}
	}
}
