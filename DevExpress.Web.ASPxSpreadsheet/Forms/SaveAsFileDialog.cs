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
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class SaveAsFileDialog : SpreadsheetDialogWithChoice {
		protected const string FilePathTextBoxId = "txbFolderPath",
							   FolderManagerId = "BrowsePopup",
							   FileNameTextBoxId = "tbxFileName",
							   FileTypeComboBoxId = "cbxFileType",
							   DownloadFileTypeComboboxId = "cblDownloadFileType";
		protected ASPxHiddenField HiddenField { get; private set; }
		protected ASPxButton DownloadButton { get; private set; }
		protected ASPxLabel FilePathCaption { get; private set; }
		protected ASPxButtonEdit FilePath { get; private set; }
		protected ASPxPopupControl FolderManager { get; private set; }
		protected ASPxLabel FileNameCaption { get; private set; }
		protected ASPxTextBox FileName { get; private set; }
		protected ASPxLabel FileTypeCaption { get; private set; }
		protected ASPxComboBox FileType { get; private set; }
		protected ASPxLabel DownloadFileTypeCaption { get; private set; }
		protected ASPxComboBox DownloadFileType { get; private set; }
		#region DialogSettings
		protected SpreadsheetSaveFileDialogSettings DialogSettings {
			get { return Spreadsheet.SettingsDialogs.SaveFileDialog; }
		}
		protected override bool IsDialogContainsChoiceSection {
			get {
				return DialogSettings.DisplaySectionMode == SaveFileDialogDisplaySectionMode.ShowAllSections;
			}
		}
		protected bool ShowDownloadSection {
			get {
				return DialogSettings.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowServerSection;
			}
		}
		protected bool ShowSaveAsSection {
			get {
				return DialogSettings.DisplaySectionMode != SaveFileDialogDisplaySectionMode.ShowDownloadSection;
			}
		}
		#endregion
		protected override string GetDialogCssClassName() {
			return "dxssDlgSaveFileAsForm";
		}
		protected override string GetContentTableID() {
			return "dxSaveFileForm";
		}
		protected override string GetRoundPanelID() {
			return "rpSaveFile";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			HtmlTableRow tableRow = null;
			HtmlTableCell tableCell = null;
			if(ShowSaveAsSection) {
				#region FilePath
				tableRow = new HtmlTableRow();
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
				tableRow.Cells.Add(tableCell);
				FilePathCaption = new ASPxLabel() { ID = "lblFileUrl", AssociatedControlID = FilePathTextBoxId };
				tableCell.Controls.Add(FilePathCaption);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
				tableCell.Style.Add("text-align", "left");
				tableRow.Cells.Add(tableCell);
				FilePath = new ASPxButtonEdit();
				FilePath.ID = FilePathTextBoxId;
				FilePath.ClientInstanceName = GetControlClientInstanceName("_dxTbxFolderPath");
				FilePath.Buttons.Add(new EditButton());
				FilePath.ClientSideEvents.ButtonClick = "ASPx.SpreadsheetDialog.ShowSelector";
				FilePath.AutoCompleteType = AutoCompleteType.Disabled;
				FilePath.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
				FilePath.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
				FilePath.ValidationSettings.SetFocusOnError = true;
				FilePath.ValidationSettings.ValidateOnLeave = false;
				FilePath.ValidationSettings.ValidationGroup = "_dxeTbxSaveFilePathGroup";
				FilePath.ValidationSettings.Display = Display.Dynamic;
				FilePath.ValidationSettings.RequiredField.IsRequired = true;
				FilePath.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
				tableCell.Controls.Add(FilePath);
				FolderManager = new ASPxPopupControl() { ID = FolderManagerId };
				FolderManager.CloseAction = CloseAction.CloseButton;
				FolderManager.CloseOnEscape = true;
				FolderManager.PopupAction = PopupAction.None;
				FolderManager.PopupElementID = MailPanelID;
				FolderManager.ClientInstanceName = "BrowsePopup";
				FolderManager.PopupHorizontalAlign = PopupHorizontalAlign.Center;
				FolderManager.PopupVerticalAlign = PopupVerticalAlign.Middle;
				FolderManager.AllowDragging = true;
				FolderManager.Modal = true;
				FolderManager.ClientSideEvents.PopUp = "";
				FolderManager.ContentStyle.Paddings.Padding = Unit.Pixel(0);
				FolderManager.ModalBackgroundStyle.Opacity = 1;
				FolderManager.ClientSideEvents.PopUp = "ASPx.SpreadsheetDialog.SelectorBeforeShow";
				FolderManager.ClientSideEvents.CloseUp = "ASPx.SpreadsheetDialog.SelectorAfterClose";
				FolderManager.Controls.Add(new SelectFolderDialog());
				tableCell.Controls.Add(FolderManager);
				#endregion
				#region FileName
				tableRow = new HtmlTableRow();
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
				tableCell.ColSpan = 2;
				tableRow.Cells.Add(tableCell);
				tableRow = new HtmlTableRow();
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
				tableCell.Style.Add("text-align", "left");
				tableRow.Cells.Add(tableCell);
				FileNameCaption = new ASPxLabel() { ID = "lblFileName", AssociatedControlID = FileNameTextBoxId };
				tableCell.Controls.Add(FileNameCaption);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
				tableRow.Cells.Add(tableCell);
				FileName = new ASPxTextBox();
				FileName.ID = FileNameTextBoxId;
				FileName.ClientInstanceName = GetControlClientInstanceName("_dxTbxFileName");
				FileName.AutoCompleteType = AutoCompleteType.Disabled;
				FileName.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
				FileName.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
				FileName.ValidationSettings.SetFocusOnError = true;
				FileName.ValidationSettings.ValidateOnLeave = false;
				FileName.ValidationSettings.ValidationGroup = "_dxeTbxSaveFilePathGroup";
				FileName.ValidationSettings.Display = Display.Dynamic;
				FileName.ValidationSettings.RequiredField.IsRequired = true;
				FileName.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
				FileName.ValidationSettings.RegularExpression.ValidationExpression = "^[^/?*:|\"<>\\\\]+$";
				tableCell.Controls.Add(FileName);
				#endregion
				#region FileType
				tableRow = new HtmlTableRow();
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
				tableCell.ColSpan = 2;
				tableRow.Cells.Add(tableCell);
				tableRow = new HtmlTableRow();
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
				tableCell.Style.Add("text-align", "left");
				tableRow.Cells.Add(tableCell);
				FileTypeCaption = new ASPxLabel() { ID = "lblFileType", AssociatedControlID = FileTypeComboBoxId };
				tableCell.Controls.Add(FileTypeCaption);
				tableCell = new HtmlTableCell();
				tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
				tableRow.Cells.Add(tableCell);
				FileType = new ASPxComboBox();
				FileType.ID = FileTypeComboBoxId;
				FileType.ClientInstanceName = GetControlClientInstanceName("_dxCbxFileType");
				FileType.Items.AddRange(GetAllowedFileCollection());
				if(FileType.Items.Count > 0)
					FileType.Items[0].Selected = true;
				tableCell.Controls.Add(FileType);
				#endregion
				InitializeFileProperties();
			}
			if(ShowDownloadSection) {
				#region FileType
				tableRow = new HtmlTableRow();
				tableRow.Style.Add("display", "none");
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Style.Add("text-align", "left");
				tableRow.Cells.Add(tableCell);
				DownloadFileTypeCaption = new ASPxLabel() { ID = "lblBrowse", AssociatedControlID = DownloadFileTypeComboboxId };
				tableCell.Controls.Add(DownloadFileTypeCaption);
				WebControl webControl = RenderUtils.CreateDiv("dxssDlgCaptionIndent");
				tableCell.Controls.Add(webControl);
				DownloadFileType = new ASPxComboBox();
				DownloadFileType.ID = DownloadFileTypeComboboxId;
				DownloadFileType.ClientInstanceName = GetControlClientInstanceName("_dxCbxDownloadFileType");
				DownloadFileType.Items.AddRange(GetAllowedFileCollection());
				if(DownloadFileType.Items.Count > 0)
					DownloadFileType.Items[0].Selected = true;
				tableCell.Controls.Add(DownloadFileType);
				#endregion
			}
		}
		protected void InitializeFileProperties() {
			string openDocumentPath = Spreadsheet.GetCurrentDocumentPath();
			string openDocumentFolderPath = string.IsNullOrEmpty(openDocumentPath) ? "" : UrlUtils.NormalizeRelativePath(Path.GetDirectoryName(openDocumentPath));
			string workingFolderFullPath = UrlUtils.ResolvePhysicalPath(Spreadsheet.GetWorkDirectory());
			string rootFolderName = DialogUtils.GetRootFolder(Spreadsheet.GetWorkDirectory());
			if(string.IsNullOrEmpty(openDocumentPath)) {
				FilePath.Text = rootFolderName;
			} else if(IsCurrentFileLocatedBelowWorkingFolder(workingFolderFullPath.ToLower(), openDocumentFolderPath.ToLower())) {
				FilePath.Text = rootFolderName + GetFolderPath(openDocumentFolderPath, workingFolderFullPath);
			} else {
				FilePath.Text = rootFolderName;
			}
			FileName.Text = GetFileName(openDocumentPath);
			ListEditItem item = FileType.Items.FindByValue(GetFileExtension(openDocumentPath));
			if(item != null)
				item.Selected = true;
		}
		protected ListEditItemCollection GetAllowedFileCollection() {
			ListEditItemCollection fileTypes = new ListEditItemCollection();
			string[] allowedFiles = Spreadsheet.SettingsDocumentSelector.CommonSettings.AllowedFileExtensions;
			foreach(string fileExtension in allowedFiles) {
				switch(fileExtension) {
					case ".xlsx":
						fileTypes.Add(new ListEditItem("Excel Workbook (*.xlsx)", ".xlsx"));
						break;
					case ".xlsm":
						fileTypes.Add(new ListEditItem("Excel Macro-Enabled Workbook (*.xlsm)", ".xlsm"));
						break;
					case ".xls":
						fileTypes.Add(new ListEditItem("Excel 97-2003 Workbook (*.xls)", ".xls"));
						break;
					case ".xltx":
						fileTypes.Add(new ListEditItem("Excel Template (*.xltx)", ".xltx"));
						break;
					case ".xltm":
						fileTypes.Add(new ListEditItem("Excel Macro-Enabled Template (*.xltm)", ".xltm"));
						break;
					case ".xlt":
						fileTypes.Add(new ListEditItem("Excel 97-2003 Template (*.xlt)", ".xlt"));
						break;
					case ".txt":
						fileTypes.Add(new ListEditItem("Text (Tab delimited) (*.txt)", ".txt"));
						break;
					case ".csv":
						fileTypes.Add(new ListEditItem("CSV (Comma delimited) (*.csv)", ".csv"));
						break;
				}
			}
			return fileTypes;
		}
		#region PopulateChoiceArea
		protected override ASPxRadioButton CreateFirstChoiceElement() {
			ASPxRadioButton firstChoice = new ASPxRadioButton();
			firstChoice.ID = "rblFromTheWeb";
			firstChoice.GroupName = "SaveFileFormGroup";
			firstChoice.Checked = true;
			firstChoice.ClientInstanceName = GetControlClientInstanceName("_dxRblFileSavedToServer");
			firstChoice.ClientSideEvents.CheckedChanged = "ASPx.SpreadsheetDialog.SectionTypeChanged";
			return firstChoice;
		}
		protected override ASPxRadioButton CreateSecondChoiceElement() {
			ASPxRadioButton secondChoice = new ASPxRadioButton();
			secondChoice.ID = "rblFromThisComputer";
			secondChoice.GroupName = "SaveFileFormGroup";
			secondChoice.ClientInstanceName = GetControlClientInstanceName("_dxRblFileSavedToClient");
			secondChoice.ClientSideEvents.CheckedChanged = "ASPx.SpreadsheetDialog.SectionTypeChanged";
			return secondChoice;
		}
		#endregion
		#region PopulateFooteerArea
		protected override void InitializeMiddleButtons(Control container) {
			InitializeDownloadButton(container);
		}
		protected void InitializeDownloadButton(Control container) {
			DownloadButton = new ASPxButton();
			DownloadButton.ID = "btnDownload";
			DownloadButton.AutoPostBack = false;
			DownloadButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			DownloadButton.CausesValidation = false;
			DownloadButton.ClientVisible = false;
			DownloadButton.ClientInstanceName = GetControlClientInstanceName("_dxBtnDownload");
			DownloadButton.ClientSideEvents.Init = GetDefaultSubmitButtonInitEventHandler();
			container.Controls.Add(DownloadButton);
		}
		#endregion
		protected override void CreateChildControls() {
			base.CreateChildControls();
			if(ShowSaveAsSection) {
				HiddenField = new ASPxHiddenField();
				HiddenField.ID = "dxHiddenField";
				HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
				HiddenField.SyncWithServer = false;
				Controls.Add(HiddenField);
			}
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RoundPanel.Style.Add("width", "350px"); 
			if(ShowSaveAsSection) {
				FilePath.Width = Unit.Percentage(100);
				FileName.Width = Unit.Percentage(100);
				FileType.Width = Unit.Percentage(100);
			}
			if(ShowDownloadSection) {
				DownloadFileType.Width = Unit.Percentage(100);
			}
		}
		protected override string GetDefaultSubmitButtonCaption() {
			return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonSaveAs);
		}
		protected override string GetDefaultSubmitButtonID() {
			return "btnSave";
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			if(IsDialogContainsChoiceSection) {
				FirstChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_SaveFileToServer);
				SecondChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_DownloadCopy);
			}
			if(ShowSaveAsSection) {
				FilePathCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_FolderPath) + DialogUtils.LabelEndMark;
				FileTypeCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_FileType) + DialogUtils.LabelEndMark;
				FileNameCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_FileName) + DialogUtils.LabelEndMark;
				FolderManager.HeaderText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_FolderManager_Header);
				FilePath.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
				FilePath.ValidationSettings.RegularExpression.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.Invalid_FileName);
				HiddenField.Add("ConfirmMessage", ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_FileAlreadyExists));
			}
			if(ShowDownloadSection) {
				DownloadFileTypeCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile_DownloadInstruction) + DialogUtils.LabelEndMark;
				DownloadButton.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonDownload);
			}
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { SubmitButton, CancelButton, DownloadButton };
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			ASPxEditBase[] baseList = base.GetChildDxEdits();
			List<ASPxEditBase> baseCollection = new List<ASPxEditBase>(baseList);
			if(ShowSaveAsSection) {
				baseCollection.Add(FilePath);
				baseCollection.Add(FileType);
				baseCollection.Add(FileName);
			}
			if(ShowDownloadSection)
				baseCollection.Add(DownloadFileType);
			return baseCollection.ToArray();
		}
		protected string GetFileName(string filePath) {
			if(string.IsNullOrEmpty(filePath))
				return DialogUtils.DefaultSpreadsheetFileName;
			return Path.GetFileNameWithoutExtension(Path.GetFileName(filePath));
		}
		protected string GetFileExtension(string filePath) {
			if(string.IsNullOrEmpty(filePath))
				return DialogUtils.DefaultSpreadsheetFileType;
			return Path.GetExtension(filePath);
		}
		protected string GetFolderPath(string openDocumentFolderPath, string workingFolderFullPath) {
			int pathCharCount = openDocumentFolderPath.ToLower().Replace(workingFolderFullPath.ToLower(), "").Length;
			return openDocumentFolderPath.Substring(openDocumentFolderPath.Length - pathCharCount);
		}
		protected bool IsCurrentFileLocatedBelowWorkingFolder(string workingFolderPath, string fileLocatedFolderPath) {
			return fileLocatedFolderPath.StartsWith(workingFolderPath);
		}
	}
}
