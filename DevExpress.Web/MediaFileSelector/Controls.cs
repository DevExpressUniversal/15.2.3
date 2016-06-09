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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class MediaFileSelectorMainControl : ASPxInternalWebControl {
		protected internal const string PageControlID = "PageControl";
		protected internal const string GalleryFileManagerID = "FileManager";
		protected internal const string UploadControlID = "UploadControl";
		protected internal const string UploadFormLayoutID = "UploadFormLayout";
		protected internal const string UploadPreviewPanelID = "UploadPreviewPanel";
		protected internal const string UploadCancelButtonID = "UploadCancelButton";
		protected internal const string UrlFormLayoutID = "UrlFormLayout";
		protected internal const string UrlTextBoxID = "UrlTextBox";
		protected internal const string UrlCheckBoxID = "UrlCheckBox";
		protected internal const string UrlPreviewPanelID = "UrlPreviewPanel";
		protected internal const string FromUrlTabName = "FromURL";
		protected internal const string FromGalleryTabName = "FromGallery";
		protected internal const string UploadTabName = "UploadTab";
		protected MediaFileSelector Owner { get; private set; }
		protected ASPxPageControl PageControl { get; private set; }
		protected ASPxFileManager GalleryFileManager { get; private set; }
		protected ASPxUploadControl UploadControl { get; private set; }
		protected ASPxFormLayout UploadFormLayout { get; private set; }
		protected ASPxRoundPanel UploadPreviewPanel { get; private set; }
		protected ASPxHyperLink UploadCancelButton { get; private set; }
		protected ASPxFormLayout UrlFormLayout { get; private set; }
		protected ASPxTextBox UrlTextBox { get; private set; }
		protected ASPxCheckBox UrlCheckBox { get; private set; }
		protected ASPxRoundPanel UrlPreviewPanel { get; private set; }
		public MediaFileSelectorMainControl(MediaFileSelector owner)
			: base() {
			Owner = owner;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PageControl = new ASPxPageControl();
			PageControl.ID = PageControlID;
			Controls.Add(PageControl);
			CreateUploadSectionHierarchy();
			CreateGallerySectionHierarchy();
			CreateURLSectionHierarchy();
			EnableClientIdGeneration(PageControl, GalleryFileManager, UploadControl, UploadFormLayout,
				UploadPreviewPanel, UploadCancelButton, UrlFormLayout, UrlTextBox, UrlCheckBox, UrlPreviewPanel);
			AssignUploadSectionSettings();
			AssignGallerySectionSettings();
			AssignURLSectionSettings();
		}
		void EnableClientIdGeneration(params ASPxWebControlBase[] controls) {
			foreach(ASPxWebControlBase control in controls) {
				if(control != null)
					ClientIDHelper.EnableClientIDGeneration(control);
			}
		}
		void CreateGallerySectionHierarchy() {
			if(Owner.Settings.AllowGalleryTab) {
				TabPage tab = CreateTabPage(Owner.Settings.GalleryTabText, FromGalleryTabName);
				GalleryFileManager = Owner.CreateFileManager();
				GalleryFileManager.ID = GalleryFileManagerID;
				tab.Controls.Add(GalleryFileManager);
			}
		}
		void CreateURLSectionHierarchy() {
			if(Owner.Settings.AllowURLTab) {
				TabPage tab = CreateTabPage(Owner.Settings.URLTabText, FromUrlTabName);
				UrlFormLayout = new ASPxFormLayout();
				UrlTextBox = new ASPxTextBox();
				UrlPreviewPanel = new ASPxRoundPanel();
				UrlTextBox.ID = UrlTextBoxID;
				UrlFormLayout.ID = UrlFormLayoutID;
				UrlPreviewPanel.ID = UrlPreviewPanelID;
				AddFormLayoutItem(UrlFormLayout, UrlTextBox);
				if(Owner.Settings.ShowSaveToServerCheckBox) {
					UrlCheckBox = new ASPxCheckBox();
					UrlCheckBox.ID = UrlCheckBoxID;
					AddFormLayoutItem(UrlFormLayout, UrlCheckBox);
				}
				AddFormLayoutItem(UrlFormLayout, UrlPreviewPanel);
				tab.Controls.Add(UrlFormLayout);
			}
		}
		void CreateUploadSectionHierarchy() {
			if(Owner.Settings.AllowUploadTab) {
				TabPage tab = CreateTabPage(Owner.Settings.UploadTabText, UploadTabName);
				var table = RenderUtils.CreateTable();
				var row = RenderUtils.CreateTableRow();
				var uploadControlCell = RenderUtils.CreateTableCell();
				var buttonCell = RenderUtils.CreateTableCell();
				table.Rows.Add(row);
				row.Cells.Add(uploadControlCell);
				row.Cells.Add(buttonCell);
				table.Width = Unit.Percentage(100);
				uploadControlCell.Width = Unit.Percentage(100);
				UploadCancelButton = new ASPxHyperLink();
				UploadCancelButton.ID = UploadCancelButtonID;
				UploadControl = Owner.CreateUploadControl();
				UploadControl.ID = UploadControlID;
				UploadControl.FileUploadMode = UploadControlFileUploadMode.OnPageLoad;
				UploadControl.FileUploadComplete += UploadControl_FileUploadComplete;
				uploadControlCell.Controls.Add(UploadControl);
				buttonCell.Controls.Add(UploadCancelButton);
				UploadFormLayout = new ASPxFormLayout();
				UploadFormLayout.ID = UploadFormLayoutID;
				UploadPreviewPanel = new ASPxRoundPanel();
				UploadPreviewPanel.ID = UploadPreviewPanelID;
				AddFormLayoutItem(UploadFormLayout, table);
				AddFormLayoutItem(UploadFormLayout, UploadPreviewPanel);
				tab.Controls.Add(UploadFormLayout);
			}
		}
		void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs args) {
			OnFileUploadComplete(sender, args, (s, e) => { Owner.OnUploadControlFileUploadComplete(s, e); }, Owner.Settings.UploadFolder, "", Owner.Settings.UploadFolderUrlPath);
		}
		public static void OnFileUploadComplete(object sender, FileUploadCompleteEventArgs args, FileSavingEventHandler handler, string uploadFolder, string fileName, string uploadFolderUrlPath) {
			FileSavingEventArgs customArgs = new FileSavingEventArgs(args);
			if(handler != null)
				handler(sender, customArgs);
			if(!customArgs.Cancel)
				MediaFileSelector.SaveUploadedFile(sender as ASPxUploadControl, customArgs, uploadFolder, fileName, uploadFolderUrlPath);
			args.ErrorText = customArgs.ErrorText;
			args.CallbackData = customArgs.SavedFileUrl;
			args.IsValid = customArgs.IsValid;
		}
		TabPage CreateTabPage(string text, string name) {
			TabPage result = PageControl.TabPages.Add(text, name);
			result.ContentControl.CssClass = "dxic-tabPage";
			return result;
		}
		LayoutItem AddFormLayoutItem(ASPxFormLayout formLayout, Control control) {
			return AddFormLayoutItem(formLayout, null, control);
		}
		LayoutItem AddFormLayoutItem(ASPxFormLayout formLayout, string caption, params Control[] controls) {
			LayoutItem item = formLayout.Items.Add<LayoutItem>(caption);
			item.ShowCaption = Utils.DefaultBoolean.False;
			LayoutItemNestedControlContainer container = new LayoutItemNestedControlContainer();
			foreach(var control in controls)
				container.Controls.Add(control);
			item.LayoutItemNestedControlCollection.Add(container);
			return item;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			PageControl = null;
			GalleryFileManager = null;
			UploadControl = null;
			UploadFormLayout = null;
			UploadPreviewPanel = null;
			UrlTextBox = null;
			UrlCheckBox = null;
			UrlFormLayout = null;
			UrlPreviewPanel = null;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(Owner, this);
			Width = Owner.Width;
			Height = Owner.Height;
			CssClass = "dxic-control";
			PrepareRoundPanels(UploadPreviewPanel, UrlPreviewPanel);
			SetFullWidth(PageControl, UrlFormLayout, UrlTextBox, GalleryFileManager, UploadControl, UploadPreviewPanel, UploadFormLayout);
			SetFullHeight(PageControl, UrlFormLayout, GalleryFileManager, UploadFormLayout, UrlPreviewPanel, UploadPreviewPanel);
			AssignUploadContolClientSideEvents();
		}
		void AssignGallerySectionSettings() {
			if(Owner.Settings.AllowGalleryTab) {
				GalleryFileManager.ClientSideEvents.Assign(Owner.Settings.FileManagerClientSideEvents);
				GalleryFileManager.Styles.CopyFrom(Owner.StylesFileManager);
				GalleryFileManager.ControlStyle.CopyFrom(Owner.StylesFileManagerControl);
				GalleryFileManager.Images.CopyFrom(Owner.ImagesFileManager);
				GalleryFileManager.CustomFileSystemProvider = Owner.Settings.FileManagerCustomFileSystemProvider;
				GalleryFileManager.CustomFileSystemProviderTypeName = Owner.Settings.FileManagerCustomFileSystemProviderTypeName;
				GalleryFileManager.ProviderType = Owner.Settings.FileManagerProviderType;
				GalleryFileManager.SettingsAmazon.Assign(Owner.Settings.FileManagerSettingsAmazon);
				GalleryFileManager.SettingsAzure.Assign(Owner.Settings.FileManagerSettingsAzure);
				GalleryFileManager.SettingsDropbox.Assign(Owner.Settings.FileManagerSettingsDropbox);
				GalleryFileManager.SettingsDataSource.Assign(Owner.Settings.FileManagerSettingsDataSource);
				GalleryFileManager.Settings.Assign(Owner.Settings.FileManagerCommonSettings);
				GalleryFileManager.SettingsEditing.Assign(Owner.Settings.FileManagerEditingSettings);
				GalleryFileManager.SettingsFolders.Assign(Owner.Settings.FileManagerFoldersSettings);
				GalleryFileManager.SettingsToolbar.Assign(Owner.Settings.FileManagerToolbarSettings);
				GalleryFileManager.SettingsUpload.Assign(Owner.Settings.FileManagerUploadSettings);
				GalleryFileManager.SettingsUpload.ValidationSettings.Assign(Owner.Settings.UploadValidationSettings);
				GalleryFileManager.SettingsPermissions.Assign(Owner.Settings.FileManagerPermissionSettings);
				GalleryFileManager.SettingsFileList.Assign(Owner.Settings.FileManagerFileListSettings);
				GalleryFileManager.SettingsBreadcrumbs.Assign(Owner.Settings.FileManagerBreadcrumbsSettings);
				GalleryFileManager.JSProperties["cp_RootFolderRelativePath"] = GalleryFileManager.GetRootFolderRelativePath(Owner.Settings.FileManagerRootFolderUrlPath);
				if(string.IsNullOrEmpty(GalleryFileManager.Settings.RootFolder))
					GalleryFileManager.Settings.RootFolder = Owner.Settings.UploadFolder;
				if(Owner.Settings.FileManagerFolderCreating != null)
					GalleryFileManager.FolderCreating += Owner.Settings.FileManagerFolderCreating;
				if(Owner.Settings.FileManagerItemDeleting != null)
					GalleryFileManager.ItemDeleting += Owner.Settings.FileManagerItemDeleting;
				if(Owner.Settings.FileManagerItemMoving != null)
					GalleryFileManager.ItemMoving += Owner.Settings.FileManagerItemMoving;
				if(Owner.Settings.FileManagerItemRenaming != null)
					GalleryFileManager.ItemRenaming += Owner.Settings.FileManagerItemRenaming;
				if(Owner.Settings.FileManagerFileUploading != null)
					GalleryFileManager.FileUploading += Owner.Settings.FileManagerFileUploading;
				if(Owner.Settings.FileManagerItemCopying != null)
					GalleryFileManager.ItemCopying += Owner.Settings.FileManagerItemCopying;
				if(Owner.Settings.FileManagerCustomThumbnail != null)
					GalleryFileManager.CustomThumbnail += Owner.Settings.FileManagerCustomThumbnail;
				GalleryFileManager.CssClass = "dxic-fileManager";
				GalleryFileManager.JSProperties["cpRequiredErrorText"] = Owner.Settings.GalleryTabRequiredErrorText;
			}
		}
		void AssignURLSectionSettings() {
			if(Owner.Settings.AllowURLTab) {
				UrlTextBox.ValidationSettings.RequiredField.IsRequired = true;
				UrlTextBox.ValidationSettings.Display = Display.Dynamic;
				UrlTextBox.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
				UrlTextBox.NullText = Owner.Settings.URLTabNullText;
				if(!string.IsNullOrEmpty(Owner.Settings.URLTabRegularExpression)) {
					UrlTextBox.ValidationSettings.RegularExpression.ValidationExpression = Owner.Settings.URLTabRegularExpression;
					UrlTextBox.ValidationSettings.RegularExpression.ErrorText = Owner.Settings.URLTabRegularExpressionErrorText;
				}
				if(Owner.Settings.ShowSaveToServerCheckBox) {
					UrlCheckBox.Text = Owner.Settings.SaveToServerText;
					UrlCheckBox.AutoPostBack = false;
					UrlCheckBox.ClientEnabled = false;
				}
				RenderUtils.AppendDefaultDXClassName(UrlFormLayout, "dxic-formLayout");
				UrlFormLayout.RequiredMarkDisplayMode = RequiredMarkMode.None;
			}
		}
		void AssignUploadContolClientSideEvents() {
			if(UploadCancelButton != null) {
				UploadCancelButton.ClientSideEvents.Click = string.Format(@"function(s, e) {{
                    s.SetVisible(false);
                    {0}.Cancel();
                }}", UploadControl.ClientID);
			}
		}
		void AssignUploadSectionSettings() {
			if(Owner.Settings.AllowUploadTab) {
				RenderUtils.AppendDefaultDXClassName(UploadCancelButton, "dxic-uploadCancelButton");
				RenderUtils.AppendDefaultDXClassName(UploadFormLayout, "dxic-formLayout");
				UploadFormLayout.RequiredMarkDisplayMode = RequiredMarkMode.None;
				UploadCancelButton.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_CancelButton);
				UploadCancelButton.ClientVisible = false;
				UploadControl.ShowCancelButton = false;
				UploadControl.DialogTriggerID = UploadPreviewPanelID;
				UploadControl.AdvancedModeSettings.ExternalDropZoneID = UploadPreviewPanelID;
				UploadControl.ShowClearFileSelectionButton = false;
				UploadControl.ShowProgressPanel = true;
				UploadControl.AutoStartUpload = true;
				UploadControl.ValidationSettings.GeneralErrorText = Owner.Settings.UploadTabRequiredErrorText;
				UploadControl.AdvancedModeSettings.EnableDragAndDrop = true;
				UploadControl.AdvancedModeSettings.TemporaryFolder = Owner.Settings.AdvancedUploadModeTemporaryFolder;
				UploadControl.AdvancedModeSettings.PacketSize = Owner.Settings.AdvancedUploadModePacketSize;
				UploadControl.ValidationSettings.Assign(Owner.Settings.UploadValidationSettings);
				UploadControl.RightToLeft = Owner.RightToLeft;
				UploadControl.UploadMode = Owner.Settings.UploadMode;
				PopulateUploadPreviewPanel();
			}
		}
		void PopulateUploadPreviewPanel() {
			if(!string.IsNullOrEmpty(Owner.Settings.PreviewUploadTipText)) {
				Label label = RenderUtils.CreateLabel();
				label.Text = Owner.Settings.PreviewUploadTipText;
				label.CssClass = "dxic-previewUploadTip";
				UploadPreviewPanel.Controls.Add(label);
			}
			if(UploadControl.ValidationSettings.AllowedFileExtensions.Any())
				AddValidationTipLabel("{0}: {1}.", Owner.Settings.AllowedFileExtensionsText, string.Join(", ", UploadControl.ValidationSettings.AllowedFileExtensions.Select(s => "<b>" + s + "</b>")));
			if(UploadControl.ValidationSettings.MaxFileSize > 0)
				AddValidationTipLabel("{0}: <b>{1}</b>.", Owner.Settings.MaximumUploadFileSizeText, GetMaxFileSizeText(UploadControl.ValidationSettings.MaxFileSize));
		}
		string GetMaxFileSizeText(double maxFileSize) {
			string[] suffixes = new string[] {"B", "KB", "MB", "GB", "TB" };
			int currentIndex = 0;
			while(maxFileSize >= 1024 && currentIndex++ < suffixes.Length - 1)
				maxFileSize *= (float)1 / 1024;
			return string.Format("{0:N3} {1}", maxFileSize, suffixes[currentIndex]);
		}
		void AddValidationTipLabel(string mask, string labelText, string value) {
			Label label = RenderUtils.CreateLabel();
			label.CssClass = "dxic-validationTip";
			label.Text = string.Format(mask, labelText, value);
			UploadPreviewPanel.Controls.Add(label);
		}
		void SetFullWidth(params ASPxWebControl[] controls) {
			foreach(var control in controls) {
				if(control != null)
					control.Width = Unit.Percentage(100);
			}
		}
		void SetFullHeight(params ASPxWebControl[] controls) {
			foreach(var control in controls) {
				if(control != null)
					control.Height = Unit.Percentage(100);
			}
		}
		void PrepareRoundPanels(params ASPxRoundPanel[] panels) {
			foreach(var panel in panels) {
				if(panel == null)
					continue;
				panel.EnableClientSideAPI = true;
				panel.ShowHeader = false;
				RenderUtils.AppendDefaultDXClassName(panel, "dxic-previewPanel");
				Label label = RenderUtils.CreateLabel();
				label.Text = Owner.Settings.PreviewText;
				label.CssClass = "dxic-previewText";
				panel.Controls.AddAt(0, label);
			}
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
	}
}
