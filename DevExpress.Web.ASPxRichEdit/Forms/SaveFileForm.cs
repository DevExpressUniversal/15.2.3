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

using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class SaveFileForm : RichEditDialogBase {
		protected RichEditSaveFileDialogSettings SettingsSaveFileDialog {
			get { return RichEdit.SettingsDialogs.SaveFileDialog; }
		}
		protected DialogFormLayoutBase ContentLayout { get; private set; }
		protected ASPxRoundPanel ContentPanel { get; private set; }
		protected ASPxPopupControl BrowsePopup { get; private set; }
		protected ASPxTextBox FileNameTextBox { get; private set; }
		protected ASPxComboBox FileTypeComboBox { get; private set; }
		protected ASPxComboBox DownloadFileTypeComboBox { get; private set; }
		protected ASPxRadioButtonList NavigationList { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			if(SettingsSaveFileDialog.DisplaySectionMode == SaveFileDialogDisplaySectionMode.ShowAllSections)
				NavigationList = group.Items.CreateEditor<ASPxRadioButtonList>("RblNavigation", showCaption: false, buffer: Editors);
			ContentPanel = new ASPxRoundPanel();
			ContentPanel.ShowHeader = false;
			var content = group.Items.CreateItem("generalContent", ContentPanel);
			content.ShowCaption = DefaultBoolean.False;
			ContentLayout = new RichEditDialogFormLayout();
			ContentLayout.ClientInstanceName = GetClientInstanceName("SaveFileContent");
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowDownloadSection) {
				var pathEdit = ContentLayout.Items.CreateEditor<ASPxButtonEdit>("TbxFolderPath", buffer: Editors);
				pathEdit.Buttons.Add(new EditButton());
				FileNameTextBox = ContentLayout.Items.CreateTextBox("TbxFileName", buffer: Editors);
				FileTypeComboBox = ContentLayout.Items.CreateComboBox("CbxFileType", buffer: Editors);
				BrowsePopup = CreatePopupControl();
				BrowsePopup.Visible = true;
				ContentPanel.Controls.Add(BrowsePopup);
			} 
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowServerSection)
				DownloadFileTypeComboBox = ContentLayout.Items.CreateComboBox("CbxDownloadFileType", buffer: Editors, location: LayoutItemCaptionLocation.Top);
			ContentLayout.ApplyCommonSettings();
			ContentPanel.Controls.Add(ContentLayout);
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			ASPxButton downloadButton = CreateDialogButton("BtnDownload", ASPxRichEditStringId.DownloadButton);
			ASPxButton submitButton = CreateDialogButton("BtnOk", ASPxRichEditStringId.SaveAsButton);
			submitButton.ID = SubmitButtonId;
			ASPxButton cancelButton = CreateDialogButton("BtnCancel", ASPxRichEditStringId.CancelButton);
			controls.Add(downloadButton);
			controls.Add(submitButton);
			controls.Add(cancelButton);
		}
		ASPxPopupControl CreatePopupControl() {
			ASPxPopupControl popupControl = new ASPxPopupControl();
			popupControl.CloseAction = CloseAction.CloseButton;
			popupControl.CloseOnEscape = true;
			popupControl.PopupAction = PopupAction.None;
			popupControl.ClientInstanceName = "BrowsePopup";
			popupControl.PopupElementID = "dxpMainPanel";
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.Center;
			popupControl.PopupVerticalAlign = PopupVerticalAlign.Middle;
			popupControl.AllowDragging = true;
			popupControl.Modal = true;
			popupControl.ContentStyle.Paddings.Padding = new Unit(0D, UnitType.Pixel);
			popupControl.ModalBackgroundStyle.Opacity = 1;
			var contentControl = new PopupControlContentControl();
			contentControl.ID = "popupControlContentControl";
			var selectFolderDialog = new SelectFolderForm();
			selectFolderDialog.InitializeAsUserControl(Page);
			selectFolderDialog.ID = "ucSelectFolderDialog";
			contentControl.Controls.Add(selectFolderDialog);
			popupControl.ContentCollection.Add(contentControl);
			return popupControl;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			FormLayoutWrapper.ClientInstanceName = GetClientInstanceName("LayoutWrapper");
			if(SettingsSaveFileDialog.DisplaySectionMode == SaveFileDialogDisplaySectionMode.ShowAllSections)
				PrepareNavigation(); 
			ListEditItemCollection allowedFileCollection = GetAllowedFileCollection();
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowDownloadSection) {
				FileTypeComboBox.Items.AddRange(allowedFileCollection);
				SetupValidationSettings();
			}
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowServerSection)
				DownloadFileTypeComboBox.Items.AddRange(allowedFileCollection);
			ContentPanel.Width = Unit.Percentage(100);
			ContentLayout.Width = Unit.Percentage(100);		   
		}
		void PrepareNavigation() {
			NavigationList.CssClass = "dxre-dialogRadioNavigation";
			NavigationList.RepeatColumns = 2;
			NavigationList.RepeatDirection = RepeatDirection.Horizontal;
			NavigationList.Border.BorderStyle = BorderStyle.None;
			NavigationList.EnableFocusedStyle = false;
			NavigationList.ValueType = typeof(int);
			NavigationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SaveToServer), 0);
			NavigationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DownloadCopy), 1);
			NavigationList.Value = 0;
		}
		protected override void Localize() {
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowDownloadSection) {
				ContentLayout.LocalizeField("TbxFolderPath", ASPxRichEditStringId.FolderPath);
				ContentLayout.LocalizeField("TbxFileName", ASPxRichEditStringId.FileName);
				ContentLayout.LocalizeField("CbxFileType", ASPxRichEditStringId.FileType);
				BrowsePopup.HeaderText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SaveFileAs);
			}
			if(SettingsSaveFileDialog.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowServerSection)
				ContentLayout.LocalizeField("CbxDownloadFileType", ASPxRichEditStringId.ChooseType);		   
		}
		void SetupValidationSettings() {
			FileNameTextBox.ValidationSettings.ValidationGroup = "_dxeTbxSaveFilePathGroup";
			FileNameTextBox.ValidationSettings.RegularExpression.ValidationExpression = "^[^/?*:|\"<>\\\\]+$";
			FileNameTextBox.ValidationSettings.RegularExpression.ErrorText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Invalid_FileName);
			FileNameTextBox.ValidationSettings.SetupDefaultSettings();
		}
		ListEditItemCollection GetAllowedFileCollection() {
			ListEditItemCollection fileTypes = new ListEditItemCollection();
			string[] allowedFiles = RichEdit.SettingsDocumentSelector.CommonSettings.AllowedFileExtensions;
			foreach(string fileExtension in allowedFiles) {
				switch(fileExtension) {
					case ".docx":
						fileTypes.Add(new ListEditItem("Microsoft Word 2007 Document (*.docx)", fileExtension));
						break;
					case ".doc":
						fileTypes.Add(new ListEditItem("Microsoft Word Document (*.doc)", fileExtension));
						break;
					case ".rtf":
						fileTypes.Add(new ListEditItem("Rich Text Format (*.rtf)", fileExtension));
						break;
					case ".txt":
						fileTypes.Add(new ListEditItem("Text Files (*.txt)", fileExtension));
						break;
					case ".html":
						fileTypes.Add(new ListEditItem("HyperText Markup Language Format (*.html)", fileExtension));
						break;
					case ".mht":
						fileTypes.Add(new ListEditItem("Web Archive, single file (*.mht)", fileExtension));
						break;
					case ".odt":
						fileTypes.Add(new ListEditItem("OpentDocument Text Document (*.odt)", fileExtension));
						break;
					case ".xml":
						fileTypes.Add(new ListEditItem("Word XML Document (*.xml)", fileExtension));
						break;
					case ".epub":
						fileTypes.Add(new ListEditItem("Electronic Publication (*.epub)", fileExtension));
						break;
				}
			}
			return fileTypes;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgSaveFileForm";
		}
	}
}
