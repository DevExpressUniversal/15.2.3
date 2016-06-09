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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class InsertLinkForm : HtmlEditorDialogWithTemplates {
		const bool MoreOptionsCheckedDefaultValue = false;
		const string EmailRegExp = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"; 
		protected bool HasDocumentSelector { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.Enabled; } }
		protected bool HasOpenInNewWindowButton { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.ShowOpenInNewWindowButton; } }
		protected bool HasEmailAddressSection { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.ShowEmailAddressSection; } }
		protected bool HasDisplayPropertiesSection { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.ShowDisplayPropertiesSection; } }
		protected ASPxCheckBox OpenInNewWindowCheckbox { get; private set; }
		protected ASPxRadioButtonList RadioButtonList { get; private set; }
		protected ASPxPopupControl SelectDocumentPopup { get; private set; }
		protected HtmlEditorFileManager FileManager { get; private set; }
		protected ASPxButtonEdit URLButtonEdit { get; private set; }
		protected ASPxTextBox EmailTextBox { get; private set; }
		protected ASPxButton PopupSelectButton { get; private set; }
		protected ASPxButton PopupCancelButton { get; private set; }
		protected DialogFormLayoutBase PopupFormLayout { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertLinkDialog";
		}
		protected override void PopulateContentGroup(LayoutGroup contentGroup) {
			if(HasEmailAddressSection) {
				RadioButtonList = contentGroup.Items.CreateEditor<ASPxRadioButtonList>(buffer: Editors, showCaption: false);
			}
			LayoutGroup editorsGroup = contentGroup.Items.CreateGroup();
			editorsGroup.CssClass = "dxhe-insertLinkEditorsGroup";
			URLButtonEdit = editorsGroup.Items.CreateEditor<ASPxButtonEdit>("URLSection", buffer: Editors, cssClassName: DialogsHelper.FullWidthCssClass + " dxhe-hideNullText");
			if(HasEmailAddressSection) {
				EmailTextBox = editorsGroup.Items.CreateTextBox(name: "EmailTextBox", buffer: Editors, clientVisible: false);
				editorsGroup.Items.CreateTextBox(name: "SubjectTextBox", buffer: Editors, clientVisible: false);
			}
			if(HasDisplayPropertiesSection) {
				editorsGroup.Items.CreateTextBox(name: "TextItem", buffer: Editors);
				editorsGroup.Items.CreateTextBox(name: "ToolTipItem", buffer: Editors);
			}
			if(HasDocumentSelector) {
				URLButtonEdit.Buttons.Add("...");
				PopulateSelectDocumentPopup();
			}
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			if(!HasOpenInNewWindowButton)
				return;
			OpenInNewWindowCheckbox = new ASPxCheckBox();
			controls.Insert(0, OpenInNewWindowCheckbox);
			Editors.Add(OpenInNewWindowCheckbox);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareMainFormLayoutItems();
			if(HasDocumentSelector)
				PrepareSelectDocumentPopup();
			PrepareRadioButtonList();
		}
		void PopulateSelectDocumentPopup() {
			PopupFormLayout = new HtmlEditorDialogFormLayout();
			SelectDocumentPopup = new ASPxPopupControl();
			SelectDocumentPopup.Controls.Add(PopupFormLayout);
			Controls.Add(SelectDocumentPopup);
			PopupSelectButton = new ASPxButton();
			PopupCancelButton = new ASPxButton();
			var fileManagerDiv = RenderUtils.CreateDiv();
			fileManagerDiv.CssClass = "dxhe-selectDocFileManagerDiv";
			FileManager = HtmlEditor.CreateFileManager();
			FileManager.ID = "FileManager";
			FileManager.Border.BorderWidth = Unit.Pixel(0);
			fileManagerDiv.Controls.Add(FileManager);
			PopupFormLayout.Items.CreateItem("", fileManagerDiv);
			FileManager.FolderCreating += new FileManagerFolderCreateEventHandler(FileManager_FolderCreating);
			FileManager.ItemDeleting += new FileManagerItemDeleteEventHandler(FileManager_ItemDeleting);
			FileManager.ItemMoving += new FileManagerItemMoveEventHandler(FileManager_ItemMoving);
			FileManager.ItemRenaming += new FileManagerItemRenameEventHandler(FileManager_ItemRenaming);
			FileManager.ItemCopying += new FileManagerItemCopyEventHandler(FileManager_ItemCopying);
			FileManager.FileUploading += new FileManagerFileUploadEventHandler(FileManager_FileUploading);
			FileManager.CustomThumbnail += new FileManagerThumbnailCreateEventHandler(FileManager_CustomThumbnail);
			AssignFileManagerSettings();
			var PopupButtonsWrapper = RenderUtils.CreateDiv();
			PopupButtonsWrapper.Controls.Add(PopupSelectButton);
			PopupButtonsWrapper.Controls.Add(PopupCancelButton);
			PopupFormLayout.Items.CreateItem("", PopupButtonsWrapper);
			PopupButtonsWrapper.CssClass = "dxhe-right-align dxheDlgFooter";
		}
		void PrepareSelectDocumentPopup() {
			PopupFormLayout.ClientInstanceName = "PopupFormLayout";
			PopupSelectButton.CssClass = "dxheDlgFooterBtn";
			PopupSelectButton.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonSelect);
			PopupSelectButton.AutoPostBack = false;
			PopupSelectButton.ClientEnabled = false;
			PopupSelectButton.ClientInstanceName = GetClientInstanceName("PopupSelectButton");
			PopupCancelButton.CssClass = "dxheDlgFooterBtn";
			PopupCancelButton.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonCancel);
			PopupCancelButton.AutoPostBack = false;
			PopupCancelButton.ClientInstanceName = GetClientInstanceName("PopupCancelButton");
			PopupFormLayout.CssClass = "dxhe-selectDocFileManager";
			SelectDocumentPopup.AllowDragging = true;
			SelectDocumentPopup.ClientInstanceName = GetClientInstanceName("SelectDocumentPopup");
			SelectDocumentPopup.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			SelectDocumentPopup.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			SelectDocumentPopup.HeaderText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SelectDocument);
			SelectDocumentPopup.CloseOnEscape = true;
			SelectDocumentPopup.ContentStyle.Paddings.Padding = Unit.Pixel(0);
		}
		void PrepareMainFormLayoutItems() {
			if(HasEmailAddressSection) {
				MainFormLayout.LocalizeField("SubjectTextBox", ASPxHtmlEditorStringId.InsertLink_Subject);
				MainFormLayout.LocalizeField("EmailTextBox", ASPxHtmlEditorStringId.InsertLink_EmailTo);
				EmailTextBox.ValidationSettings.RegularExpression.ValidationExpression = EmailRegExp;
				EmailTextBox.ValidationSettings.RegularExpression.ErrorText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink_EmailErrorText);
				EmailTextBox.ValidationSettings.SetupRequiredSettings();
			}
			MainFormLayout.LocalizeField("URLSection", ASPxHtmlEditorStringId.InsertLink_Url);
			URLButtonEdit.NullText = "http://";
			URLButtonEdit.NullTextStyle.CssClass = "dxhe-nullText";
			URLButtonEdit.ClientInstanceName = GetClientInstanceName("ShowingPopupButtonEdit");
			URLButtonEdit.ValidationSettings.SetupRequiredSettings();
			if(HasDisplayPropertiesSection) {
				MainFormLayout.LocalizeField("TextItem", ASPxHtmlEditorStringId.InsertLink_Text);
				MainFormLayout.LocalizeField("ToolTipItem", ASPxHtmlEditorStringId.InsertLink_ToolTip);
			}
			if(HasOpenInNewWindowButton) {
				OpenInNewWindowCheckbox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink_OpenInNewWindow);
				OpenInNewWindowCheckbox.ClientInstanceName = GetClientInstanceName("OpenInNewWindowCheckbox");
			}
		}
		void PrepareRadioButtonList() {
			if(HasEmailAddressSection) {
				RadioButtonList.Items.Add(new ListEditItem(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink_Url), "URLRadioButton"));
				RadioButtonList.Items.Add(new ListEditItem(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink_Email), "EmailRadioButton"));
				RadioButtonList.RepeatDirection = RepeatDirection.Horizontal;
				RadioButtonList.ItemSpacing = Unit.Pixel(22); 
				RadioButtonList.SelectedIndex = 0;
				RadioButtonList.ClientInstanceName = GetClientInstanceName("RdBttnListSectionsSwitcher");
				RadioButtonList.CssClass = "dxhe-emptyBorder";
			}
		}
		protected override HtmlEditorDialogSettingsBase GetDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertLinkDialog;
		}
		protected void AssignFileManagerSettings() {
			FileManager.Styles.CopyFrom(HtmlEditor.StylesFileManager);
			FileManager.CssClass = DialogsHelper.FullWidthCssClass;
			FileManager.ClientInstanceName = GetClientInstanceName("FileManager");
			FileManager.ControlStyle.CopyFrom(HtmlEditor.StylesFileManager.Control);
			FileManager.Images.CopyFrom(HtmlEditor.ImagesFileManager);
			FileManager.Settings.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.CommonSettings);
			FileManager.SettingsEditing.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.EditingSettings);
			FileManager.SettingsFolders.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FoldersSettings);
			FileManager.SettingsToolbar.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ToolbarSettings);
			FileManager.SettingsUpload.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.UploadSettings);
			FileManager.SettingsPermissions.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.PermissionSettings);
			FileManager.SettingsFileList.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileListSettings);
			FileManager.SettingsBreadcrumbs.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.BreadcrumbsSettings);
			FileManager.SetRootFolderRelativePathJSProp(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.RootFolderUrlPath);
			FileManager.ClientSideEvents.Assign(HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ClientSideEvents);
		}
		protected void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorFolderCreating(e);
		}
		protected void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorItemDeleting(e);
		}
		protected void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorItemMoving(e);
		}
		protected void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorItemRenaming(e);
		}
		protected void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorFileUploading(e);
		}
		protected void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorItemCopying(e);
		}
		protected void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) {
			HtmlEditor.RaiseDocumentSelectorCustomThumbnail(e);
		}
		protected override void PopulateButtonList(List<ASPxButton> list) {
			if(HasDocumentSelector) {
				list.Add(PopupSelectButton);
				list.Add(PopupCancelButton);
			}
		}
	}
}
