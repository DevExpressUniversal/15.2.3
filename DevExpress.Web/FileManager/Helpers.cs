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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Zip;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.Rendering;
namespace DevExpress.Web.Internal {
	public class FileManagerHelper {
		public const string CreateNodeID = "CreateNode";
		public const int DefaultThumbnailSize = 70;
		public const int DefaultFileSizeColumnWidth = 80;
		public const int DefaultThumbnailGridSize = 16;
		public const int UploadControlWidth = 270;
		public const int DefaultGridCellLeftRightPadding = 6;
		public const int DefaultGridViewCheckboxColumnWidth = 28;
		const int DefaultThumbnailsViewFileAreaItemTemplateHeight = 100;
		ASPxFileManager fileManager;
		FileManagerDataHelper dataHelper;
		FileManagerThumbnailHelper thumbnailHelper;
		FileManagerClientStateHelper clientStateHelper;
		FileManagerEditHelper editHelper;
		public FileManagerHelper(ASPxFileManager fileManager) {
			this.fileManager = fileManager;
			this.dataHelper = new FileManagerDataHelper(fileManager);
			this.thumbnailHelper = new FileManagerThumbnailHelper(fileManager);
			this.clientStateHelper = new FileManagerClientStateHelper(fileManager);
			this.editHelper = new FileManagerEditHelper(fileManager);
		}
		protected ASPxFileManager FileManager { get { return fileManager; } }
		public ASPxUploadControl UploadControl { get { return FileManager.Control.Container.UploadControl; } }
		public FileManagerDataHelper Data { get { return dataHelper; } }
		public FileManagerThumbnailHelper Thumbnails { get { return thumbnailHelper; } }
		public FileManagerClientStateHelper ClientState { get { return clientStateHelper; } }
		public FileManagerEditHelper Edit { get { return editHelper; } }
		public Unit GridViewCheckboxColumnWidth {
			get {
				if(!FileManager.StylesDetailsView.CommandColumn.Width.IsEmpty)
					return FileManager.StylesDetailsView.CommandColumn.Width;
				return Unit.Pixel(DefaultGridViewCheckboxColumnWidth);
			}
		}
		public int ThumbnailSize {
			get {
				bool isDetailsView = FileManager.SettingsFileList.View == FileListView.Details;
#pragma warning disable 618
				if(isDetailsView && !FileManager.SettingsFileList.DetailsViewSettings.ThumbnailSize.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.DetailsViewSettings.ThumbnailSize);
				if(!isDetailsView && !FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailSize.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailSize);
				if(!FileManager.Settings.ThumbnailSize.IsEmpty)
					return GetThumbSize(FileManager.Settings.ThumbnailSize);
#pragma warning restore 618
				return isDetailsView ? DefaultThumbnailGridSize : DefaultThumbnailSize;
			}
		}
		public int ThumbnailWidth {
			get {
				bool isDetailsView = FileManager.SettingsFileList.View == FileListView.Details;
				if(isDetailsView && !FileManager.SettingsFileList.DetailsViewSettings.ThumbnailWidth.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.DetailsViewSettings.ThumbnailWidth);
				if(!isDetailsView && !FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailWidth.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailWidth);
				return ThumbnailSize;
			}
		}
		public int ThumbnailHeight {
			get {
				bool isDetailsView = FileManager.SettingsFileList.View == FileListView.Details;
				if(isDetailsView && !FileManager.SettingsFileList.DetailsViewSettings.ThumbnailHeight.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.DetailsViewSettings.ThumbnailHeight);
				if(!isDetailsView && !FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailHeight.IsEmpty)
					return GetThumbSize(FileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailHeight);
				return ThumbnailSize;
			}
		}
		int GetThumbSize(Unit value) {
			return (int)UnitUtils.ConvertToPixels(value).Value;
		}
		public Unit GetThumbnailColumnWidth(GridViewCellStyle cellStyle) {
			var paddings = DefaultGridCellLeftRightPadding * 2;
			if(!cellStyle.Paddings.IsEmpty)
				paddings = (int)UnitUtils.ConvertToPixels(cellStyle.Paddings.PaddingLeft).Value + (int)UnitUtils.ConvertToPixels(cellStyle.Paddings.PaddingRight).Value;
			return Unit.Pixel(ThumbnailWidth + paddings);
		}
		public Unit GetFileSizeColumnWidth() {
			return Unit.Pixel(DefaultFileSizeColumnWidth);
		}
		public Hashtable GetClientScriptStylesObject() {
			Hashtable result = new Hashtable();
			result["iw"] = GetCorrectedSize(GetFileStyle().Width);
			result["ih"] = GetCorrectedSize(GetFileStyle().Height);
			result["tw"] = ThumbnailWidth;
			result["th"] = ThumbnailHeight;
			result["fc"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileCssClass() ?? "";
			result["fs"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileStyleString() ?? "";
			result["fcc"] = GetFileContentCssClass();
			result["fcs"] = GetFileContentStyleString() ?? "";
			result["fhc"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileHoverCssClass() ?? "";
			result["fhs"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileHoverStyleString() ?? "";
			result["fsac"] = GetFileSelectionActiveCssClass() ?? "";
			result["fsas"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileSelectionActiveStyleString() ?? "";
			result["fsic"] = GetFileSelectionInactiveCssClass() ?? "";
			result["fsis"] = GetFileSelectionInactiveStyleString() ?? "";
			result["ffc"] = !FileManager.Settings.EnableMultiSelect ? "" : GetFileFocusCssClass();
			result["ffs"] = !FileManager.Settings.EnableMultiSelect || FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileFocusStyleString() ?? "";
			result["fafc"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderCssClass() ?? "";
			result["fafs"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderStyleString() ?? "";
			result["fafcc"] = GetFileAreaFolderContentCssClass();
			result["fafcs"] = GetFileAreaFolderContentStyleString() ?? "";
			result["fafhc"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderHoverCssClass() ?? "";
			result["fafhs"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderHoverStyleString() ?? "";
			result["fafsac"] = GetFileAreaFolderSelectionActiveCssClass() ?? "";
			result["fafsas"] = FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderSelectionActiveStyleString() ?? "";
			result["fafsic"] = GetFileAreaFolderSelectionInactiveCssClass() ?? "";
			result["fafsis"] = GetFileAreaFolderSelectionInactiveStyleString() ?? "";
			result["faffc"] = !FileManager.Settings.EnableMultiSelect ? "" : GetFileAreaFolderFocusCssClass();
			result["faffs"] = !FileManager.Settings.EnableMultiSelect || FileManager.SettingsFileList.View == FileListView.Details ? "" : GetFileAreaFolderFocusStyleString() ?? "";
			if(FileManager.SettingsFolders.Visible) {
				result["fosac"] = GetFolderSelectionActiveCssClass() ?? "";
				result["fosic"] = GetFolderSelectionInactiveCssClass() ?? "";
				result["fosis"] = GetFolderSelectionInactiveStyleString() ?? "";
			}
			if(FileManager.SettingsBreadcrumbs.Visible) {
				AppearanceStyle style = GetBreadCrumbsItemStyle();
				AppearanceSelectedStyle hoverStyle = GetBreadCrumbsItemHoverStyle();
				result["bc"] = style.CssClass;
				result["bs"] = GetStyleString(style) ?? "";
				result["bhc"] = hoverStyle.CssClass;
				result["bhs"] = GetStyleString(hoverStyle) ?? "";
				result["bids"] = FileManager.Helper.GetBreadCrumbsUpButtonImage().GetDisabledScriptObject(FileManager.Page);
				result["bihs"] = FileManager.Helper.GetBreadCrumbsUpButtonImage().GetHottrackedScriptObject(FileManager.Page);
			}
			if(FileManager.SettingsFileList.View == FileListView.Details && FileManager.SettingsFileList.ShowFolders && FileManager.SettingsEditing.AllowCreate) {
				result["gvrc"] = GetFilesGridViewDataRowCssClass();
				result["gvrs"] = GetFilesGridViewDataRowStyleString();
			}
			result["hc"] = GetHighlightCssClass() ?? "";
			result["hs"] = GetHighlightStyleString() ?? "";
			return result;
		}
		public string GetCurrentRelativePath() {
			return FileManager.FileSystemProvider.GetRelativeFolderPath(FileManager.SelectedFolder, FileManager);
		}
		int GetCorrectedSize(Unit size) {
			Unit correctedSize = UnitUtils.ConvertToPixels(size);
			return correctedSize.IsEmpty ? 0 : (int)correctedSize.Value;
		}
		protected internal bool GetUploadIsValid() {
			return UploadControl.IsValidInternal;
		}
		protected internal string GetUploadErrorText() {
			return UploadControl.ErrorText;
		}
		protected internal AppearanceStyleBase GetRootControlStyle() {
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.CssClass = FileManager.GetControlStyle().CssClass;
			if((FileManager as ISkinOwner).IsRightToLeft())
				result.CssClass = RenderUtils.CombineCssClasses(result.CssClass, FileManagerStyles.RightToLeftCssClass);
			string platformMarker = RenderUtils.Browser.Platform.IsWebKitTouchUI ? FileManagerStyles.TouchCssClass : FileManagerStyles.DesctopCssClass;
			result.CssClass = RenderUtils.CombineCssClasses(result.CssClass, platformMarker);
			return result;
		}
		protected internal AppearanceStyleBase GetSplitterStyle() {
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.CopyFrom(FileManager.GetControlStyle());
			result.CssClass = "";
			return result;
		}
		protected internal AppearanceStyleBase GetToolbarMenuStyle() {
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.ForeColor = FileManager.Styles.Toolbar.ForeColor;
			if(result.ForeColor.IsEmpty)
				result.ForeColor = FileManager.ForeColor;
			result.Font.CopyFrom(FileManager.Font);
			result.Font.CopyFrom(FileManager.Styles.Toolbar.Font);
			return result;
		}
		protected internal AppearanceStyleBase GetFoldersTreeViewStyle() {
			AppearanceStyleBase result = new AppearanceStyleBase();
			result.ForeColor = FileManager.ForeColor;
			return result;
		}
		protected internal FileManagerFileStyle GetFileStyle() {
			FileManagerFileStyle style = FileManager.Styles.GetDefaultFileStyle();
			if(FileManager.IsThumbnailsViewFileAreaItemTemplate)
				style.Height = Unit.Pixel(DefaultThumbnailsViewFileAreaItemTemplateHeight);
			style.CopyFrom(FileManager.Styles.Item);
			style.CopyFrom(FileManager.Styles.File);
			return style;
		}
		protected internal FileManagerFileStyle GetFolderStyle() {
			FileManagerFileStyle style = FileManager.Styles.GetDefaultFolderStyle();
			style.Font.CopyFrom(FileManager.Font);
			style.CopyFrom(FileManager.Styles.Item);
			style.CopyFrom(FileManager.Styles.Folder);
			return style;
		}
		protected internal FileManagerFileStyle GetFileAreaFolderStyle() {
			FileManagerFileStyle style = FileManager.Styles.GetDefaultFileAreaFolderStyle();
			if(FileManager.IsThumbnailsViewFileAreaItemTemplate)
				style.Height = Unit.Pixel(DefaultThumbnailsViewFileAreaItemTemplateHeight);
			style.CopyFrom(FileManager.Styles.Item);
			style.CopyFrom(FileManager.Styles.FileAreaFolder);
			return style;
		}
		protected internal FileManagerFolderContainerStyle GetFolderContainerStyle() {
			FileManagerFolderContainerStyle style = new FileManagerFolderContainerStyle();
			style.CopyFrom(FileManager.Styles.FolderContainer);
			return style;
		}
		protected internal AppearanceStyleBase GetFileContainerStyle() {
			AppearanceStyleBase style = FileManager.Styles.GetDefaultFileContainerStyle();
			style.CopyFrom(FileManager.Styles.FileContainer);
			return style;
		}
		protected internal AppearanceStyleBase GetClearFileStyle() {
			AppearanceStyleBase style = GetFileStyle();
			style.HorizontalAlign = HorizontalAlign.NotSet;
			style.Width = Unit.Empty;
			style.Height = Unit.Empty;
			return style;
		}
		protected internal AppearanceStyleBase GetFileFocusStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileFocusStyle();
			result.CopyFrom(FileManager.Styles.File.FocusedStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileContentStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileContentStyle();
			result.HorizontalAlign = GetFileStyle().HorizontalAlign;
			return result;
		}
		protected internal AppearanceStyleBase GetFileHoverStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileHoverStyle();
			result.MergeWith(GetFileStyle().HoverStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileSelectionActiveStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileSelectionActiveStyle();
			result.MergeWith(GetFileStyle().SelectionActiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileSelectionInactiveStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileSelectionInactiveStyle();
			result.MergeWith(GetFileStyle().SelectionInactiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFolderSelectionInactiveStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFolderSelectionInactiveStyle();
			result.MergeWith(GetFolderStyle().SelectionInactiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFolderSelectionActiveStyle() {
			AppearanceStyleBase result = FileManager.Folders.Styles.GetDefaultNodeSelectedStyle();
			result.MergeWith(GetFolderStyle().SelectionActiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetClearFileAreaFolderStyle() {
			AppearanceStyleBase style = GetFileAreaFolderStyle();
			style.HorizontalAlign = HorizontalAlign.NotSet;
			style.Width = Unit.Empty;
			style.Height = Unit.Empty;
			return style;
		}		
		protected internal AppearanceStyleBase GetFileAreaFolderFocusStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileAreaFolderFocusStyle();
			result.CopyFrom(FileManager.Styles.FileAreaFolder.FocusedStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileAreaFolderContentStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileAreaFolderContentStyle();
			result.HorizontalAlign = GetFileAreaFolderStyle().HorizontalAlign;
			return result;
		}
		protected internal AppearanceStyleBase GetFileAreaFolderHoverStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileAreaFolderHoverStyle();
			result.MergeWith(GetFileAreaFolderStyle().HoverStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileAreaFolderSelectionActiveStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileAreaFolderSelectionActiveStyle();
			result.MergeWith(GetFileAreaFolderStyle().SelectionActiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetFileAreaFolderSelectionInactiveStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultFileAreaFolderSelectionInactiveStyle();
			result.MergeWith(GetFileAreaFolderStyle().SelectionInactiveStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetBreadCrumbsStyle() {
			AppearanceStyleBase style = FileManager.Styles.GetDefaultBreadcrumbsStyle();
			style.CopyFrom(FileManager.Styles.Breadcrumbs);
			return style;
		}
		protected internal AppearanceStyle GetBreadCrumbsItemStyle() {
			AppearanceStyle style = FileManager.Styles.GetDefaultBreadcrumbsItemStyle();
			style.CopyFrom(FileManager.Styles.BreadcrumbsItem);
			return style;
		}
		protected internal AppearanceSelectedStyle GetBreadCrumbsItemHoverStyle() {
			AppearanceSelectedStyle result = FileManager.Styles.GetDefaultBreadcrumbsItemHoverStyle();
			result.MergeWith(GetBreadCrumbsItemStyle().HoverStyle);
			return result;
		}
		protected internal AppearanceStyleBase GetHighlightStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultHighlightStyle();
			result.MergeWith(FileManager.Styles.Highlight);
			return result;
		}
		Unit GetSize(Unit specified, Unit _default) {
			return specified.IsEmpty
				? _default
				: specified;
		}
		protected internal Unit GetToolbarHeight() {
			return GetSize(FileManager.Styles.Toolbar.Height, FileManager.Styles.GetDefaultToolbarHeight());
		}
		protected internal Unit GetFoldersContainerWidth() {
			return GetSize(FileManager.Styles.FolderContainer.Width, FileManager.Styles.GetDefaultFoldersContainerWidth());
		}
		protected internal AppearanceStyleBase GetToolbarStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultToolbarStyle();
			result.MergeWith(FileManager.Styles.Toolbar);
			return result;
		}
		protected internal FileManagerToolbarItemStyle GetToolbarItemStyle() {
			FileManagerToolbarItemStyle result = new FileManagerToolbarItemStyle();
			result.MergeWith(FileManager.Styles.ToolbarItem);
			return result;
		}
		protected internal FileManagerToolbarItemStyle GetContextMenuItemStyle() {
			FileManagerToolbarItemStyle result = new FileManagerToolbarItemStyle();
			result.MergeWith(FileManager.Styles.ContextMenuItem);
			return result;
		}
		protected internal FileManagerToolbarItemStyle GetToolbarPathBoxStyle() {
			FileManagerToolbarItemStyle result = new FileManagerToolbarItemStyle();
			result.MergeWith(FileManager.Styles.PathBox);
			return result;
		}
		protected internal FileManagerToolbarItemStyle GetToolbarFilterBoxStyle() {
			FileManagerToolbarItemStyle result = new FileManagerToolbarItemStyle();
			result.MergeWith(FileManager.Styles.FilterBox);
			return result;
		}
		protected internal Unit GetToolbarPathTextBoxWidth() {
			return FileManager.Styles.Toolbar.PathTextBoxWidth;
		}
		protected internal Unit GetToolbarFilterTextBoxWidth() {
			return FileManager.Styles.Toolbar.FilterTextBoxWidth;
		}
		protected internal Unit GetUploadPanelHeight() {
			Unit specifiedHeight = FileManager.Styles.UploadPanel.Height;
			return specifiedHeight.IsEmpty
				? FileManager.Styles.GetDefaultUploadPanelHeight()
				: specifiedHeight;
		}
		protected internal AppearanceStyleBase GetUploadPanelStyle() {
			AppearanceStyleBase result = FileManager.Styles.GetDefaultUploadPanelStyle();
			result.MergeWith(FileManager.Styles.UploadPanel);
			return result;
		}
		protected internal Unit GetUploadControlHeight() {
			return Unit.Pixel(23);
		}
		string GetStyleString(AppearanceStyleBase style) {
			string result = style.GetStyleAttributes(FileManager).Value;
			if(style.HorizontalAlign != HorizontalAlign.NotSet)
				result += "text-align:" + style.HorizontalAlign.ToString().ToLower() + ";";
			return result;
		}
		string GetFileCssClass() {
			return GetFileStyle().CssClass;
		}
		string GetFileStyleString() {
			return GetStyleString(GetClearFileStyle());
		}
		string GetFileContentCssClass() {
			return GetFileContentStyle().CssClass;
		}
		string GetFileContentStyleString() {
			return GetStyleString(GetFileContentStyle());
		}
		string GetFileHoverCssClass() {
			return GetFileHoverStyle().CssClass;
		}
		string GetFileHoverStyleString() {
			return GetStyleString(GetFileHoverStyle());
		}
		string GetFileSelectionActiveCssClass() {
			return GetFileSelectionActiveStyle().CssClass;
		}
		string GetFileSelectionActiveStyleString() {
			return GetStyleString(GetFileSelectionActiveStyle());
		}
		string GetFileSelectionInactiveCssClass() {
			return GetFileSelectionInactiveStyle().CssClass;
		}
		string GetFileSelectionInactiveStyleString() {
			return GetStyleString(GetFileSelectionInactiveStyle());
		}
		string GetFileFocusCssClass() {
			return GetFileFocusStyle().CssClass;
		}
		string GetFileFocusStyleString() {
			return GetStyleString(GetFileFocusStyle());
		}
		string GetFolderSelectionActiveCssClass() {
			return GetFolderSelectionActiveStyle().CssClass;
		}
		string GetFolderSelectionActiveStyleString() {
			return GetStyleString(GetFolderSelectionActiveStyle());
		}
		string GetFolderSelectionInactiveCssClass() {
			return GetFolderSelectionInactiveStyle().CssClass;
		}
		string GetFolderSelectionInactiveStyleString() {
			return GetStyleString(GetFolderSelectionInactiveStyle());
		}
		string GetFileAreaFolderCssClass() {
			return GetFileAreaFolderStyle().CssClass;
		}
		string GetFileAreaFolderStyleString() {
			return GetStyleString(GetClearFileAreaFolderStyle());
		}
		string GetFileAreaFolderContentCssClass() {
			return GetFileAreaFolderContentStyle().CssClass;
		}
		string GetFileAreaFolderContentStyleString() {
			return GetStyleString(GetFileAreaFolderContentStyle());
		}
		string GetFileAreaFolderHoverCssClass() {
			return GetFileAreaFolderHoverStyle().CssClass;
		}
		string GetFileAreaFolderHoverStyleString() {
			return GetStyleString(GetFileAreaFolderHoverStyle());
		}
		string GetFileAreaFolderSelectionActiveCssClass() {
			return GetFileAreaFolderSelectionActiveStyle().CssClass;
		}
		string GetFileAreaFolderSelectionActiveStyleString() {
			return GetStyleString(GetFileAreaFolderSelectionActiveStyle());
		}
		string GetFileAreaFolderSelectionInactiveCssClass() {
			return GetFileAreaFolderSelectionInactiveStyle().CssClass;
		}
		string GetFileAreaFolderSelectionInactiveStyleString() {
			return GetStyleString(GetFileAreaFolderSelectionInactiveStyle());
		}
		string GetFileAreaFolderFocusCssClass() {
			return GetFileAreaFolderFocusStyle().CssClass;
		}
		string GetFileAreaFolderFocusStyleString() {
			return GetStyleString(GetFileAreaFolderFocusStyle());
		}
		string GetHighlightCssClass() {
			return GetHighlightStyle().CssClass;
		}
		string GetHighlightStyleString() {
			return GetStyleString(GetHighlightStyle());
		}
		string GetFilesGridViewDataRowCssClass() {
			return FileManager.FilesGridView.RenderHelper.GetDataRowStyle(0).CssClass;
		}
		string GetFilesGridViewDataRowStyleString() {
			return GetStyleString(FileManager.FilesGridView.RenderHelper.GetDataRowStyle(0));
		}
		public ImagePropertiesBase GetImage(string name) {
			ImagePropertiesBase result = new ImagePropertiesBase();
			result.MergeWith(FileManager.Images.GetImageBase(name));
			result.MergeWith(FileManager.Images.GetImageProperties(FileManager.Page, name));
			return result;
		}
		public ImagePropertiesBase GetNoThumbnailImage() {
			return GetPredefinedImage(FileManagerImages.FileImageName);
		}
		public ImagePropertiesBase GetPdfFileImage() {
			return GetPredefinedImage(FileManagerImages.PdfFileImageName);
		}
		public ImagePropertiesBase GetPlainTextFileImage() {
			return GetPredefinedImage(FileManagerImages.PlainTextFileImageName);
		}
		public ImagePropertiesBase GetPresentationFileImage() {
			return GetPredefinedImage(FileManagerImages.PresentationFileImageName);
		}
		public ImagePropertiesBase GetRichTextFileImage() {
			return GetPredefinedImage(FileManagerImages.RichTextFileImageName);
		}
		public ImagePropertiesBase GetSpreadsheetFileImage() {
			return GetPredefinedImage(FileManagerImages.SpreadsheetFileImageName);
		}
		public ImagePropertiesBase GetFolderImage() {
			return GetPredefinedImage(FileManagerImages.FolderBigImageName);
		}
		public ImagePropertiesBase GetLockedFolderImage() {
			return GetPredefinedImage(FileManagerImages.FolderLockedBigImageName);
		}
		public ImagePropertiesBase GetFolderUpImage() {
			return GetPredefinedImage(FileManagerImages.FolderUpImageName);
		}
		public ImagePropertiesBase GetBreadCrumbsUpButtonImage() {
			return GetImage(FileManagerImages.BreadcrumbsParentFolderButtonImageName);
		}
		public ImagePropertiesBase GetBreadCrumbsSeparatorImage() {
			return GetImage(FileManagerImages.BreadcrumbsSeparatorImageName);
		}
		ImagePropertiesBase GetPredefinedImage(string name) {
			ImagePropertiesBase result = GetImage(name);
			result.Width = Unit.Pixel(ThumbnailWidth);
			result.Height = Unit.Pixel(ThumbnailHeight);
			return result;
		}
		public TreeViewNode CreateCreateFolderNode() {
			TreeViewNode node = null;
			if(FileManager.Folders.EnableCallBacks)
				node = new TreeViewVirtualNode(FileManagerDataHelper.NewFolderNodeName);
			else
				node = new TreeViewNode();
			CreateFolderNode nodeTemplate = new CreateFolderNode(FileManager.Folders);
			nodeTemplate.ID = CreateNodeID;
			node.Template = nodeTemplate;
			return node;
		}
	}
	public class FileManagerDataHelper {
		const string NameFieldName = "n";
		const string IDFieldName = "id";
		const string CustomThumbnailIndexFieldName = "ci";
		const string ThumbnailUrlFieldName = "i";
		const string RightsFieldName = "r";
		const string RightsMoveFieldValue = "m";
		const string RightsRenameFieldValue = "r";
		const string RightsDeleteFieldValue = "d";
		const string RightsCreateFieldValue = "c";
		const string RightsUploadFieldValue = "u";
		const string RightsDownloadFieldValue = "l";
		const string RightsCopyFieldValue = "o";
		const string RightsDefaultValue = "-";
		const string RightsAllowValue = "a";
		const string RightsDenyValue = "d";
		const string TooltipInfoFieldName = "t";
		const string ItemTypeFieldName = "it";
		const string ClientInvisibleFieldName = "cn";
		internal const string NewFolderNodeName = "[fmNewFolderNode]";
		ASPxFileManager fileManager;
		Dictionary<string, string> filePathIndex;
		List<string> customThumbnails;
		Dictionary<string, ImageProperties> customThumbnailsProperties;
		string selectedFolderPath;
		List<PropertyInfo> itemProperties;
		Dictionary<string, FileManagerFolder> FolderIdMap { get; set; }
		IEnumerable<string> selectedItems;
		int virtScrollPageItemsCount;
		public FileManagerDataHelper(ASPxFileManager fileManager) {
			this.fileManager = fileManager;
			this.filePathIndex = new Dictionary<string, string>();
			FolderIdMap = new Dictionary<string, FileManagerFolder>();
			this.customThumbnails = new List<string>();
			this.customThumbnailsProperties = new Dictionary<string, ImageProperties>();
			ResetVirtScrollState();
		}
		protected internal Dictionary<string, string> FilePathIndex { get { return filePathIndex; } }
		protected bool DesignMode { get { return FileManager.DesignMode; } }
		protected ASPxFileManager FileManager { get { return this.fileManager; } }
		protected internal List<string> CustomThumbnails { get { return customThumbnails; } }
		protected internal Dictionary<string, ImageProperties> CustomThumbnailsProperties { get { return customThumbnailsProperties; } }
		protected FileManagerFolders Folders { get { return FileManager.Folders; } }
		protected FileManagerItems Files { get { return FileManager.Items; } }
		protected RestrictedAccessFileSystemProvider FileSystemProvider { get { return FileManager.FileSystemProvider; } }
		protected internal bool NeedResetToInitialFolder { get; set; }
		protected internal string ItemFilter {
			get { return FileManager.Helper.ClientState.GetItemFilter(); }
		}
		protected internal bool IsItemFilterApplied {
			get { return !string.IsNullOrEmpty(ItemFilter); }
		}
		protected internal FileManagerFile[] ForcedlySelectedFiles { get; set; }
		protected internal IEnumerable<string> SelectedItemsIds {
			get {
				if(IsItemFilterApplied)
					return Enumerable.Empty<string>();
				if(ForcedlySelectedFiles != null)
					return GetVisibleForcedlySelectedFilesIds();
				if(this.selectedItems == null)
					return Enumerable.Empty<string>();
				return this.selectedItems; }
			set { 
				this.selectedItems = value;
			}
		}
		protected internal int ItemsCount { get; set; }
		protected internal int VirtScrollItemIndex { get; set; }
		protected internal int VirtScrollPageItemsCount {
			get {
				return this.virtScrollPageItemsCount == -1 ? FileManager.SettingsFileList.PageSize : this.virtScrollPageItemsCount;
			}
			set {
				this.virtScrollPageItemsCount = value;
			}
		}
		protected internal int VirtualScrollingPageSize { get { return FileManager.SettingsFileList.PageSize; } }
		public List<FileManagerItem> ItemListCache { get; set; }
		public string SelectedFolderPath { 
			get {
				return DetermineExistsFolder(this.selectedFolderPath, FileManager.Settings.InitialFolder);
			}
			private set {
				if(this.selectedFolderPath == value)
					return;
				this.selectedFolderPath = value;
				ItemListCache = null;
				if(!FileManager.IsCallback)
					FileManager.LayoutChanged();
			}
		}
		public string DetermineExistsFolder(params string[] suggestions) {
			foreach(var folderName in suggestions.Where(f => f != null)) {
				if(Folders == null) {
					if(FileSystemProvider.Exists(new FileManagerFolder(FileSystemProvider, folderName, GetFolderIdPath(folderName))))
						return folderName;
				}
				else if(FindDirectoryNode(Folders, folderName) != null)
					return folderName;
			}
			return string.Empty;
		}
		public List<PropertyInfo> ItemProperties {
			get {
				if(itemProperties == null) {
					itemProperties = typeof(FileManagerFile).GetProperties().ToList();
					var folerProperties = typeof(FileManagerFolder).GetProperties();
					foreach(PropertyInfo p in folerProperties) {
						if(!itemProperties.Contains(p))
							itemProperties.Add(p);
					}
				}
				return itemProperties;
			}
		}
		public void ResetFilePathIndexes() {
			FilePathIndex.Clear();
			FolderIdMap.Clear();
		}
		public void SyncFolders(FileManagerFolders folders) {
			FileManagerFolder initFolder = new FileManagerFolder(FileSystemProvider, FileManager.Settings.InitialFolder);
			if(string.IsNullOrEmpty(initFolder.RelativeName) || (FileSystemProvider.Exists(initFolder) && FileSystemProvider.CanBrowse(initFolder)))
				SelectFolder(folders, initFolder, false);
			else
				SelectFolder(folders, new FileManagerFolder(FileSystemProvider, string.Empty), false);
			if(!NeedResetToInitialFolder)
				FileManager.Helper.ClientState.SyncCurrentPath();
		}
		public IEnumerable<string> GetVisibleForcedlySelectedFilesIds() {
			FileManagerFile[] files = GetVisibleForcedlySelectedFiles().ToArray();
			FileManagerFile[] noIdFiles = files.Where(f => !f.HasId).ToArray();
			Dictionary<string, string> idMap = null;
			if(noIdFiles.Length > 0) {
				idMap = GetItemsList(false).
					Where(itm => itm is FileManagerFile && itm.HasId).
					Join(noIdFiles, itm => itm.RelativeName, nif => nif.RelativeName, (itm, nif) => new {
						RelativeName = itm.RelativeName,
						itm.Id
					}).
					ToDictionary(entry => entry.RelativeName, entry => entry.Id);
			}
			List<string> idList = new List<string>();
			foreach(FileManagerFile file in files) {
				string id = file.Id;
				if(!file.HasId && idMap != null) {
					string idValue;
					if(idMap.TryGetValue(file.RelativeName, out idValue))
						id = idValue;
				}
				idList.Add(id);
			}
			return idList;
		}
		IEnumerable<FileManagerFile> GetVisibleForcedlySelectedFiles() {
			if(ForcedlySelectedFiles != null) {
				foreach(var file in ForcedlySelectedFiles) {
					if(file.Folder.Equals(FileManager.SelectedFolder) && FileManager.FileSystemProvider.Exists(file) && FileManager.FileSystemProvider.CanBrowse(file))
						yield return file;
				}
			}
		}
		public void CreateFolders(FileManagerFolders folders, bool moveMode) {
			if(DesignMode) {
				CreateDesignModeFolders(folders);
			}
			else {
				folders.Nodes.Clear();
				ResetFilePathIndexes();
				FileManagerFolder rootFolder = new FileManagerFolder(FileSystemProvider, string.Empty, GetRootFolderId());
				TreeViewNode rootNode = CreateNode(folders.Nodes, rootFolder, moveMode);
				CreateFolders(rootNode, rootFolder, moveMode);
				SyncFolders(folders);
			}
		}
		void CreateFolders(TreeViewNode node, FileManagerFolder parent, bool moveMode) {
			try {
				foreach(FileManagerFolder folder in FileSystemProvider.GetFolders(parent)) {
					if(IsThumbnailFolder(folder)) continue;
					CreateFolders(CreateNode(node.Nodes, folder, moveMode), folder, moveMode);
				}
			}
			catch { }
		}
		TreeViewNode CreateNode(TreeViewNodeCollection nodes, FileManagerFolder folder, bool moveMode) {
			TreeViewNode result = nodes.Add(folder.Name, folder.Id);
			RegisterFolderIdByPath(folder);
			if(FileManager.SettingsFolders.ShowFolderIcons) {
				string folderIconName = IsLockedFolderIcon(folder) ? FileManagerImages.FolderLockedImageName : FileManagerImages.FolderImageName;
				result.Image.Assign(FileManager.Helper.GetImage(folderIconName));
			}
			if(moveMode)
				result.Enabled = FileSystemProvider.CanAddChild(folder);
			return result;
		}
		void CreateDesignModeFolders(FileManagerFolders folders) {
			folders.Nodes.Clear();
			TreeViewNode rootNode = new TreeViewNode("Root Folder");
			folders.Nodes.Add(rootNode);
			rootNode.Nodes.Add(new TreeViewNode("Folder 1"));
			rootNode.Nodes.Add(new TreeViewNode("Folder 2"));
			rootNode.Nodes.Add(new TreeViewNode("Folder 3"));
			rootNode.Expanded = true;
		}
		public void PopulateFolders(FileManagerFolders folders, TreeViewVirtualModeCreateChildrenEventArgs e) {
			List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
			if(e.NodeName == null) {
				FileManagerFolder rootFolder = new FileManagerFolder(FileSystemProvider, string.Empty, GetRootFolderId());
				children.Add(CreateVirtualNode(rootFolder, folders.IsMoving));
			}
			else {
				if(!string.IsNullOrEmpty(folders.CreateNodeParentName)) {
					if(e.NodeName == folders.CreateNodeParentName) {
						var crNode = FileManager.Helper.CreateCreateFolderNode();
						crNode.Text = FileManagerDataHelper.NewFolderNodeName;
						crNode.IsLeaf = true;
						children.Add((TreeViewVirtualNode)crNode);
					}
				}
				FileManagerFolder parentFolder = GetTargetFolderForPopulation(folders, e.NodeName);
				if(FileSystemProvider.Exists(parentFolder)) {
					foreach(var folder in FileSystemProvider.GetFolders(parentFolder)) {
						if(IsThumbnailFolder(folder)) continue;
						children.Add(CreateVirtualNode(folder, folders.IsMoving));
					}
				}
			}
			e.Children = children;
		}
		FileManagerFolder GetTargetFolderForPopulation(FileManagerFolders folders, string folderId) {
			FileManagerFolder folder = GetFolderById(folderId);
			if(folder == null) {
				if(!string.IsNullOrEmpty(folders.CallbackTargetFolderPath)) {
					folder = new FileManagerFolder(FileSystemProvider, folders.CallbackTargetFolderPath, folderId);
					RegisterFolderId(folder);
				}
				else
					throw new Exception("Invalid callback argument");
			}
			return folder;
		}
		bool IsThumbnailFolder(FileManagerFolder folder) {
			if(FileManager.FileSystemProvider.Provider is PhysicalFileSystemProvider) {
				var folderPath = UrlUtils.ResolvePhysicalPath(folder.FullName);
				return FileManager.ThumbnailsFolderPath.TrimEnd('\\', '/').EndsWith(folderPath.TrimEnd('\\', '/'), StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}
		TreeViewVirtualNode CreateVirtualNode(FileManagerFolder folder, bool moveMode) {
			TreeViewVirtualNode node = new TreeViewVirtualNode(folder.Id, folder.Name);
			node.IsLeaf = !FileSystemProvider.GetFolders(folder).Any();
			if(FileManager.SettingsFolders.ShowFolderIcons) {
				string folderIconName = IsLockedFolderIcon(folder) ? FileManagerImages.FolderLockedImageName : FileManagerImages.FolderImageName;
				node.Image.Assign(FileManager.Helper.GetImage(folderIconName));
			}
			if(!moveMode)
				RegisterFolderIdByPath(folder);
			RegisterFolderId(folder);
			return node;
		}
		public IEnumerable<FileManagerItem> GetItemsList(bool immediately) {
			FileManagerFolder folder = FileManager.SelectedFolder;
			if(!immediately && FileManager.Page != null && FileManager.Page.IsPostBack && FileManager.HasClientState() && !FileManager.Helper.ClientState.PathSynchronized)
				yield break;
			if(folder == null)
				yield break;
			if(immediately)
				ItemListCache = null;
			if(ItemListCache != null) {
				foreach(var dataItem in ItemListCache)
					yield return dataItem;
			}
			else {
				CustomThumbnails.Clear();
				ItemListCache = new List<FileManagerItem>();
				if(FileManager.SettingsFileList.ShowParentFolder && folder.Parent != null) {
					FileManagerFolder parentFolder = folder.Parent;
					parentFolder.IsParentFolderItem = true;
					yield return CreateItemData(parentFolder);
				}
				IEnumerable<FileManagerItem> items = GetItemsListCore(folder);
				if(FileManager.IsThumbnailsViewMode && IsItemFilterApplied) {
					string itemFilter = ItemFilter.ToLower();
					items = items.Where(item => item.Name.ToLower().IndexOf(itemFilter) > -1);
				}
				foreach(FileManagerItem item in items)
					yield return CreateItemData(item);
			}
		}
		IEnumerable<FileManagerItem> GetItemsListCore(FileManagerFolder folder) {
			if(FileManager.SettingsFileList.ShowFolders) {
				foreach(FileManagerFolder subFolder in FileSystemProvider.GetFolders(folder)) {
					if(IsThumbnailFolder(subFolder))
						continue;
					yield return subFolder;
				}
			}
			foreach(FileManagerFile file in FileSystemProvider.GetFiles(folder))
				yield return file;
		}
		public IEnumerable<FileManagerItem> GetMinimizedItemsList(bool immediately) {
			if(!FileManager.IsVirtualScrollingEnabled())
				return GetItemsList(immediately);
			if(FileManager.IsThumbnailsViewMode)
				return GetThumbnailsModeItemsList(immediately);
			return GetGridModeItemsList(immediately);
		}
		IEnumerable<FileManagerItem> GetThumbnailsModeItemsList(bool immediately) {
			ItemsCount = GetItemsList(immediately).Count();
			int selItemIndex = FindSelectedItemIndex(ItemListCache);
			if(selItemIndex > -1)
				VirtScrollItemIndex = Math.Max(selItemIndex - (VirtualScrollingPageSize / 2), 0);
			ValidateItemIndex();
			return ItemListCache.Skip(VirtScrollItemIndex).Take(VirtScrollPageItemsCount);
		}
		IEnumerable<FileManagerItem> GetGridModeItemsList(bool immediately) {
			int selItemIndex = FindSelectedItemIndex(FileManager.FilesGridView.GetAllVisibleItems());
			if(selItemIndex > -1) {
				int pageIndex = selItemIndex / FileManager.SettingsFileList.PageSize;
				FileManager.FilesGridView.UpdatePageIndex(pageIndex);
			}
			return FileManager.FilesGridView.GetCurrentPageVisibleItems();
		}
		int FindSelectedItemIndex(IEnumerable<FileManagerItem> itemList) {
			string selItemId = SelectedItemsIds.FirstOrDefault();
			if(selItemId != null) {
				int i = 0;
				foreach(FileManagerItem item in itemList) {
					if(item.Id == selItemId)
						return i;
					i++;
				}
			}
			return -1;
		}
		public List<Hashtable> GetItemsClientHashtable(bool immediately) {
			List<Hashtable> result = FileManager.IsVirtualScrollingEnabled()
				? GetMinimizedItemsClientHashtable(immediately)
				: GetAllItemsClientHashtable(immediately);
			if(FileManager.SettingsFileList.ShowFolders && FileManager.SettingsEditing.AllowCreate &&
			   (!FileManager.IsVirtualScrollingEnabled() || fileManager.Helper.Data.VirtScrollItemIndex == 0))
				result.Add(GetCreateHelperFolderScript());
			return result;
		}
		public List<Hashtable> GetApiCallAllItemsClientHashtable() {
			return GetAllItemsClientHashtableCore(true).
				Select(script => { script[ClientInvisibleFieldName] = true; return script; }).
				ToList();
		}
		List<Hashtable> GetAllItemsClientHashtable(bool immediately) {
			return GetAllItemsClientHashtableCore(immediately).ToList();
		}
		IEnumerable<Hashtable> GetAllItemsClientHashtableCore(bool immediately) {
			return GetItemsList(immediately).Select(item => item.Script);
		}
		List<Hashtable> GetMinimizedItemsClientHashtable(bool immediately) {
			List<Hashtable> result = new List<Hashtable>();
			IEnumerable<FileManagerItem> items = GetMinimizedItemsList(immediately);
			Dictionary<string, string> selItemsMap = SelectedItemsIds.ToDictionary(id => id);
			foreach(FileManagerItem item in items) {
				result.Add(item.Script);
				if(selItemsMap.ContainsKey(item.Id))
					selItemsMap.Remove(item.Id);
			}
			if(selItemsMap.Count > 0) {
				items = GetItemsList(false);
				foreach(FileManagerItem item in items) {
					if(selItemsMap.ContainsKey(item.Id)) {
						selItemsMap.Remove(item.Id);
						Hashtable script = CreateClientInvisibleItemData(item).Script;
						result.Add(script);
					}
				}
			}
			return result;
		}
		void ValidateItemIndex() {
			if(VirtScrollItemIndex + VirtScrollPageItemsCount > ItemsCount) {
				VirtScrollItemIndex = Math.Max(ItemsCount - VirtScrollPageItemsCount, 0);
				VirtScrollPageItemsCount = ItemsCount - VirtScrollItemIndex;
			}
		}
		public void ResetVirtScrollState() {
			ItemsCount = 0;
			VirtScrollItemIndex = 0;
			this.virtScrollPageItemsCount = -1;
		}
		Hashtable GetCreateHelperFolderScript() {
			var createHelperFolder = new FileManagerFolder(FileManager.FileSystemProvider, FileManager.SelectedFolder, FileManagerFolder.CreateHelperFolderTypeName);
			createHelperFolder.IsCreateHelperItem = true;
			return CreateItemData(createHelperFolder).Script;
		}
		FileManagerItem CreateClientInvisibleItemData(FileManagerItem item) {
			item.ClientInvisible = true;
			return CreateItemData(item);
		}
		FileManagerItem CreateItemData(FileManagerItem item) {
			var isFile = item is FileManagerFile;
			item.Script = isFile ? GenerateFileDataScriptInfo((FileManagerFile)item) : GenerateFolderDataScriptInfo((FileManagerFolder)item);
			if(!item.ClientInvisible && (isFile || !((FileManagerFolder)item).IsCreateHelperItem))
				ItemListCache.Add(item);
			return item;
		}
		Hashtable GenerateFileDataScriptInfo(FileManagerFile file) {
			Hashtable result = new Hashtable();
			FillItemDataScriptInfo(file, result);
			result[ItemTypeFieldName] = FileManagerItemType.File.ToString();
			if(!TryCreateItemCustomThumbnail(file, result)) {
				var src = FileManager.FileSystemProvider.Provider.GetThumbnailUrl(file) ?? FileManager.Helper.Thumbnails.GetThumbnailUrl(file);
				if(string.IsNullOrEmpty(src))
					src = FileManager.Helper.GetNoThumbnailImage().Url;
				result[ThumbnailUrlFieldName] = src;
				file.ThumbnailUrl = src;
			}
			result[RightsFieldName] = GetClientFileRightsScript(file);			
			return result;
		}
		Hashtable GenerateFolderDataScriptInfo(FileManagerFolder folder) {
			Hashtable result = new Hashtable();
			FillItemDataScriptInfo(folder, result);
			result[ItemTypeFieldName] = GetFolderType(folder);
			if(!TryCreateItemCustomThumbnail(folder, result)) {
				if(folder.IsParentFolderItem)
					result[ThumbnailUrlFieldName] = FileManager.Helper.GetFolderUpImage().Url;
				else
					result[ThumbnailUrlFieldName] = IsLockedFolderIcon(folder) ? FileManager.Helper.GetLockedFolderImage().Url : FileManager.Helper.GetFolderImage().Url;
				folder.ThumbnailUrl = result[ThumbnailUrlFieldName].ToString();
			}
			result[RightsFieldName] = GetClientFolderRightsScript(folder);
			return result;
		}
		void FillItemDataScriptInfo(FileManagerItem item, Hashtable hashtable) {
			hashtable[NameFieldName] = item.Name;
			hashtable[IDFieldName] = item.Id;
			hashtable[TooltipInfoFieldName] = GetItemTooltipInfo(item);
			hashtable[ClientInvisibleFieldName] = item.ClientInvisible;
		}
		bool TryCreateItemCustomThumbnail(FileManagerItem item, Hashtable dataScriptInfo) {
			bool isThumbnailViewWithoutItemTemplate = FileManager.SettingsFileList.View == FileListView.Thumbnails && !FileManager.HasThumbnailsViewFileAreaItemTemplate;
			if(isThumbnailViewWithoutItemTemplate) {
				string customThumb = FileManager.Helper.Thumbnails.GetCustomThumbnailRender(item);
				if(string.IsNullOrEmpty(customThumb))
					return false;
				int thumbIndex = CustomThumbnails.IndexOf(customThumb);
				if(thumbIndex == -1) {
					CustomThumbnails.Add(customThumb);
					thumbIndex = CustomThumbnails.Count - 1;
				}
				dataScriptInfo[CustomThumbnailIndexFieldName] = thumbIndex;
				return true;
			}
			ImageProperties properties = FileManager.Helper.Thumbnails.GetCustomThumbnailProperties(item);
			if(properties.IsEmpty)
				return false;
			CustomThumbnailsProperties[item.Id] = properties;
			item.ThumbnailUrl = properties.Url;
			return true;			
		}
		string GetFolderType(FileManagerFolder folder) {
			if(folder.IsParentFolderItem)
				return FileManagerItemType.ParentFolder.ToString();
			if(folder.IsCreateHelperItem)
				return FileManagerFolder.CreateHelperFolderTypeName;
			return FileManagerItemType.Folder.ToString();
		}
		string GetItemTooltipInfo(FileManagerItem item) {
			var infoTypes = new List<FileInfoType> { FileInfoType.LastWriteTime };
			if(item is FileManagerFile)
				infoTypes.Insert(0, FileInfoType.Size);
			StringBuilder sb = new StringBuilder();
			foreach(var fi in infoTypes)
				sb.AppendFormat("{0}: {1}||", GetFileInfoTypeCaption(fi), GetItemInfoDisplayText(item, fi));
			return sb.ToString();
		}
		string GetClientFileRightsScript(FileManagerFile file) {
			StringBuilder sb = new StringBuilder();
			if(FileSystemProvider.CanDelete(file))
				sb.Append(RightsDeleteFieldValue);
			if(FileSystemProvider.CanMove(file))
				sb.Append(RightsMoveFieldValue);
			if(FileSystemProvider.CanRename(file))
				sb.Append(RightsRenameFieldValue);
			if(FileSystemProvider.CanDownload(file))
				sb.Append(RightsDownloadFieldValue);
			if(FileSystemProvider.CanCopy(file))
				sb.Append(RightsCopyFieldValue);
			return sb.ToString();
		}
		internal string GetClientFolderRightsScript(FileManagerFolder folder) {
			StringBuilder sb = new StringBuilder();
			if(folder.IsParentFolderItem)
				return string.Empty;
			if(FileSystemProvider.CanDelete(folder))
				sb.Append(RightsDeleteFieldValue);
			if(FileSystemProvider.CanMove(folder))
				sb.Append(RightsMoveFieldValue);
			if(FileSystemProvider.CanRename(folder))
				sb.Append(RightsRenameFieldValue);
			if(FileSystemProvider.CanCreate(folder))
				sb.Append(RightsCreateFieldValue);
			if(FileSystemProvider.CanUpload(folder))
				sb.Append(RightsUploadFieldValue);
			if(FileSystemProvider.CanCopy(folder))
				sb.Append(RightsCopyFieldValue);
			return sb.ToString();
		}
		internal object[] GetFilesAccessRulesClientArray(FileManagerFolder folder) {
			object[] result = FileSystemProvider.GetChildFilesRules(folder).Select(r => GetClientAccessRuleParts(r)).ToArray();
			return result.Length > 0 ? result : null;
		}
		string[] GetClientAccessRuleParts(FileManagerAccessRuleBase rule) {
			return new[] {
				rule.Path,
				GetClientPermissionValue(rule.Edit) + GetClientPermissionValue(rule.Browse)
			};
		}
		string GetClientPermissionValue(Rights permission) {
			return permission == Rights.Default 
				? RightsDefaultValue
				: permission == Rights.Allow ? RightsAllowValue : RightsDenyValue;
		}
		string GetRootFolderId() {
			DataSourceFileSystemProvider dsProvider = FileSystemProvider.Provider as DataSourceFileSystemProvider;
			return dsProvider != null ? dsProvider.RootEntity.ID.ToString() : string.Empty;
		}
		protected internal string[] GetFolderIdPath(string path) {
			int pathItemCount = FileManagerItem.GetPathItemCount(path);
			string[] idPath = new string[pathItemCount];
			string folderId = null;
			for(int i = pathItemCount - 1; i >= 0; i--) {
				folderId = GetFolderIdByPath(path);
				idPath[i] = folderId ?? string.Empty;
				path = FileManagerItem.GetParentName(path);
			}
			return idPath;
		}
		protected internal string[] GetItemIdPath(string parentPath, string itemId) {
			string[] parentIdPath = GetFolderIdPath(parentPath);
			List<string> idList = new List<string>(parentIdPath);
			idList.Add(itemId);
			return idList.ToArray();
		}
		protected internal void RegisterFolderIdByPath(string path, string id) {
			FilePathIndex[path] = id;
		}
		protected internal void RegisterFolderIdByPath(FileManagerFolder folder) {
			RegisterFolderIdByPath(folder.RelativeName, folder.Id);
		}
		protected internal void RegisterFolderIdPath(string path, string[] idPath) {
			int pathItemCount = FileManagerItem.GetPathItemCount(path);
			if(pathItemCount != idPath.Length)
				FileManagerItem.ThrowWrongIdPathLengthException();
			for(int i = pathItemCount - 1; i >= 0; i--) {
				if(!FilePathIndex.ContainsKey(path))
					RegisterFolderIdByPath(path, idPath[i]);
				path = FileManagerItem.GetParentName(path);
			}
		}
		string GetFolderIdByPath(string path) {
			string folderId;
			if(FilePathIndex.TryGetValue(path, out folderId))
				return folderId;
			return null;
		}
		FileManagerFolder GetFolderById(string folderId) {
			FileManagerFolder folder;
			if(FolderIdMap.TryGetValue(folderId, out folder))
				return folder;
			return null;
		}
		void RegisterFolderId(FileManagerFolder folder) {
			FolderIdMap[folder.Id] = folder;
		}
		public bool SelectFolder(FileManagerFolders treeView, FileManagerFolder folder, bool resetState, bool expandNode = true) {
			if(treeView == null) {
				selectedFolderPath = folder.RelativeName;
				if(resetState)
					FileManager.Helper.ClientState.ResetPathState();
				return true;
			}
			return SelectFolder(treeView, folder.RelativeName, resetState, expandNode);
		}
		public bool SelectFolder(FileManagerFolders treeView, string relativeName, bool resetState, bool expandNode) {
			if(SelectNode(treeView, relativeName, expandNode)) {
				SelectedFolderPath = relativeName;
				if(resetState)
					FileManager.Helper.ClientState.ResetPathState();
				return true;
			}
			else {
				SelectedFolderPath = DetermineExistsFolder(FileManager.Settings.InitialFolder);
				SelectNode(treeView, SelectedFolderPath, expandNode);
				if(resetState)
					FileManager.Helper.ClientState.ResetPathState();
				return false;
			}
		}
		public void DisableNode(FileManagerFolders folders, FileManagerFolder folder, bool enable) {
			TreeViewNode node = FindDirectoryNode(folders, folder);
			if(node != null) {
				node.Enabled = enable;
			}
		}
		public void SetVirtualNodesEnabled(TreeViewNodeCollection nodes) {
			foreach(TreeViewNode node in nodes) {
				FileManagerFolder folder = GetFolderById(node.Name);
				if(folder != null)
					node.Enabled = FileSystemProvider.CanBrowse(folder);
				SetVirtualNodesEnabled(node.Nodes);
			}
		}
		public void ExpandNode(FileManagerFolders folders, FileManagerFolder folder, bool expand) {
			TreeViewNode node = FindDirectoryNode(folders, folder);
			if(node != null) {
				node.Expanded = expand;
				folders.ExpandToNode(node);
			}
		}
		bool SelectNode(FileManagerFolders folders, string path, bool expand) {
			TreeViewNode node = FindDirectoryNode(folders, path);
			folders.SelectedNode = node;
			if(node != null) {
				folders.ExpandToNode(node);
				folders.SelectedNode.Expanded = expand;
			}
			return node != null;
		}
		protected internal TreeViewNode FindDirectoryNode(FileManagerFolders foldersTree, FileManagerFolder folder) {
			return FindDirectoryNode(foldersTree, folder.RelativeName);
		}
		protected internal TreeViewNode FindDirectoryNode(FileManagerFolders foldersTree, string folderPath) {
			string[] folderPathParts = folderPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			TreeViewNode curNode = foldersTree.RootNode.Nodes.Count > 0 ? foldersTree.RootNode.Nodes[0] : null;
			for(int i = 0; i < folderPathParts.Length && curNode != null; i++) {
				if(curNode is TreeViewVirtualNode && (!curNode.IsLeaf && !curNode.Nodes.Any()))
					((TreeViewVirtualNode)curNode).ForceNodesPopulation();
				curNode = curNode.Nodes.Find(delegate(TreeViewNode node) { return folderPathParts[i] == node.Text; });
			}
			return curNode;
		}
		internal bool IsLockedFolderIcon(FileManagerFolder folder) {
			if(!FileManager.SettingsFolders.ShowLockedFolderIcons)
				return false;
			if(FileManager.SettingsPermissions.AccessRules.Count == 0) {
				if(!FileManager.SettingsUpload.Enabled || FileManager.SettingsUpload.AllowedFolderInternal == FileManagerAllowedFolder.Any)
					return false;
				return !FileSystemProvider.CanUpload(folder);
			}
			return !(
				FileSystemProvider.CanUpload(folder) ||
				FileSystemProvider.CanCreate(folder) ||
				FileSystemProvider.CanDelete(folder) ||
				FileSystemProvider.CanRename(folder));
		}
		public static string GetRootFolderName(string path) {
			path = path.TrimEnd('\\', '/');
			int sepIndex = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));
			return sepIndex > -1 ? path.Remove(0, sepIndex + 1) : path;
		}
		public string GetFilePropertyName(FileInfoType fileInfoType) {
			switch(fileInfoType) {
				case FileInfoType.FileName:
					return "Name";
				case FileInfoType.LastWriteTime:
					return "LastWriteTime";
				case FileInfoType.Size:
					return "Length";
				case FileInfoType.Thumbnail:
					return "ThumbnailUrl";
				default:
					throw new Exception("Wrong view column type");
			}
		}
		public FileInfoType GetFileInfoType(string filePropertyName) {
			switch(filePropertyName) {
				case "Name":
					return FileInfoType.FileName;
				case "LastWriteTime":
					return FileInfoType.LastWriteTime;
				case "Length":
					return FileInfoType.Size;
				case "ThumbnailUrl":
					return FileInfoType.Thumbnail;
				default:
					throw new Exception(string.Format("File Property '{0}' is not found"));
			}
		}
		public string GetItemInfoDisplayText(FileManagerItem item, FileInfoType fileInfoType) {
			bool encodeHtml;
			return GetItemInfoDisplayText(item, fileInfoType, out encodeHtml);
		}
		public string GetItemInfoDisplayText(FileManagerItem item, FileInfoType fileInfoType, out bool encodeHtml) {
			string displayText = string.Empty;
			var file = item as FileManagerFile;
			bool isFile = file != null;
			switch(fileInfoType) {
				case FileInfoType.LastWriteTime:
					displayText = item.LastWriteTime.ToString();
					break;
				case FileInfoType.Size:
					displayText = isFile ? GetItemSizeDisplayText(item.Length) : ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Folder);
					break;
				default:
					throw new Exception("Internal Error");
			}
			if(isFile)
				displayText = FileManager.RaiseCustomFileInfoDisplayText(file, fileInfoType, displayText, out encodeHtml);
			else
				encodeHtml = true;
			return displayText;
		}
		public void GridViewHeaderFilterFillItems(ASPxGridViewHeaderFilterEventArgs e) {
			var values = e.Values.Where(v => v.IsFilterByValue).Select(v => v.Value).ToList();
			var column = GetFileInfoType(e.Column.FieldName);
			e.Values.Clear();
			e.AddShowAll();
			switch(column) {
				case FileInfoType.LastWriteTime:
					FillDateFilterColumn(e);
					break;
				case FileInfoType.Size:
					FillSizeFilterColumn(e);
					break;
				default:
					throw new Exception("Internal error");
			}
		}
		public static string GetFileInfoTypeCaption(FileInfoType fileInfoType) {
			switch(fileInfoType) {
				case FileInfoType.FileName:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_FileName);
				case FileInfoType.LastWriteTime:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_LastWriteTime);
				case FileInfoType.Size:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_FileInfoTypeCaption_Size);
				case FileInfoType.Thumbnail:
					return " ";
				default:
					throw new Exception("Internal Error");
			}
		}
		void FillDateFilterColumn(ASPxGridViewHeaderFilterEventArgs e) {
			var prop = new OperandProperty(e.Column.FieldName);
			var date = DateTime.Now.Date;
			var today = prop > date & prop < date.AddDays(1);
			var lastWeek = prop > date.AddDays(-7);
			var lastMonth = prop > date.AddDays(-30);
			var lastYear = prop > date.AddDays(-365);
			e.AddValue(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_GridViewFilter_DateToday), string.Empty, today.ToString());
			e.AddValue(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_GridViewFilter_DateWeek), string.Empty, lastWeek.ToString());
			e.AddValue(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_GridViewFilter_DateMonth), string.Empty, lastMonth.ToString());
			e.AddValue(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_GridViewFilter_DateYear), string.Empty, lastYear.ToString());
		}
		void FillSizeFilterColumn(ASPxGridViewHeaderFilterEventArgs e) {
			var prop = new OperandProperty(e.Column.FieldName);
			var kb = 1024;
			var mb = kb * kb;
			Func<ASPxperienceStringId, string, string> formatDisplayText = (stringId, value) => {
				return string.Format(ASPxperienceLocalizer.GetString(stringId), value);
			};
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeEmpty, "0 " + GetInformationUnitSymbol("K")), string.Empty,
				(prop == 0).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeTiny, "0 - 10 " + GetInformationUnitSymbol("K")), string.Empty,
				(prop > 0 & prop <= 10 * kb).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeSmall, "10 - 100 " + GetInformationUnitSymbol("K")), string.Empty,
				(prop > 10 * kb & prop <= 100 * kb).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeMedium, "100 " + GetInformationUnitSymbol("K") + " - 1 " + GetInformationUnitSymbol("M")), string.Empty,
				(prop > 100 * kb & prop <= 100 * kb).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeLarge, "1 " + GetInformationUnitSymbol("M") + " - 16 " + GetInformationUnitSymbol("M")), string.Empty,
				(prop > 1 * mb & prop <= 16 * mb).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeHuge, "16 " + GetInformationUnitSymbol("M") + " - 128 " + GetInformationUnitSymbol("M")), string.Empty,
				(prop > 16 * mb & prop <= 128 * mb).ToString());
			e.AddValue(formatDisplayText(ASPxperienceStringId.FileManager_GridViewFilter_SizeGigantic, ">128 " + GetInformationUnitSymbol("M")), string.Empty,
				(prop > 128 * mb).ToString());
		}
		string GetItemSizeDisplayText(long size) {
			string[] indicator = { GetInformationUnitSymbol(""), GetInformationUnitSymbol("K"), GetInformationUnitSymbol("M"),
								   GetInformationUnitSymbol("G"), GetInformationUnitSymbol("T"), GetInformationUnitSymbol("P") };
			if(size == 0)
				return string.Format("0 {0}", indicator[0]);
			int place = Convert.ToInt32(Math.Floor(Math.Log(size, 1024)));
			double num = Math.Round(size / Math.Pow(1024, place), 2);
			return string.Format("{0} {1}", num, indicator[place]);
		}
		string GetInformationUnitSymbol(string metricPrefix) {
			return metricPrefix + ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_InformationUnitSymbol);
		}
	}
	public class FileManagerThumbnailHelper {
		const string FileNameFormat = "{0}/{1}.{2}";
		const string FileExtension = "png";
		Hashtable thumbNamesCache;
		ASPxFileManager fileManager;
		public FileManagerThumbnailHelper(ASPxFileManager fileManager) {
			this.fileManager = fileManager;
		}
		public ASPxFileManager FileManager { get { return fileManager; } }
		Hashtable ThumbNamesCache {
			get {
				if(thumbNamesCache == null)
					thumbNamesCache = new Hashtable();
				return thumbNamesCache;
			}
		}
		public string GetThumbnailUrl(FileManagerFile file) {
			return GetGeneratedThumbnailUrl(file) ??
				   GetPredefinedThumbnailUrl(file) ??
				   GetDefaultThumbnailUrl();
		}
		public ImageProperties GetCustomThumbnailProperties(FileManagerItem item) {
			return FileManager.RaiseThumbnailCreate(item, GetFileManagerItemType(item) == FileManagerItemType.ParentFolder);
		}
		public string GetCustomThumbnailRender(FileManagerItem item) {
			ImageProperties imageProp = FileManager.RaiseThumbnailCreate(item, GetFileManagerItemType(item) == FileManagerItemType.ParentFolder);
			if(!imageProp.IsEmpty) {
				Image img = new Image();
				imageProp.AssignToControl(img, FileManager.DesignMode);
				return RenderUtils.GetRenderResult(img);
			}
			return null;
		}
		FileManagerItemType GetFileManagerItemType(FileManagerItem item) {
			if(item is FileManagerFile)
				return FileManagerItemType.File;
			if(((FileManagerFolder)item).IsParentFolderItem)
				return FileManagerItemType.ParentFolder;
			return FileManagerItemType.Folder;
		}
		string GetPredefinedThumbnailUrl(FileManagerFile file) {
			switch(file.Extension) {
				case ".txt":
					return FileManager.Helper.GetPlainTextFileImage().Url;
				case ".rtf":
				case ".doc":
				case ".docx":
				case ".odt":
					return FileManager.Helper.GetRichTextFileImage().Url;
				case ".xls":
				case ".xlsx":
				case ".ods":
					return FileManager.Helper.GetSpreadsheetFileImage().Url;
				case ".ppt":
				case ".pptx":
				case ".odp":
					return FileManager.Helper.GetPresentationFileImage().Url;
				case ".pdf":
					return FileManager.Helper.GetPdfFileImage().Url;
				default:
					return null;
			}
		}
		string GetGeneratedThumbnailUrl(FileManagerFile file) {
			if(CanGenerateThumbnail(file.Extension)) {
				FileInfo thumbnailFile = new FileInfo(GetThumbnailFilePath(file));
				if(!HasActualThumbnail(file, thumbnailFile)) {
					using(var thumbnailStream = FileManager.FileSystemProvider.Provider.GetThumbnail(file) ?? FileManager.FileSystemProvider.ReadFile(file)) {
						if(!GenerateThumbnail(thumbnailStream, thumbnailFile))
							return null;
					}
				}
				if(FileManager.Request == null)
					return null;
				var thumbUrl = UrlUtils.Combine(FileManager.Request.ApplicationPath, "~/", Path.Combine(FileManager.Settings.ThumbnailFolder, thumbnailFile.Directory.Name, thumbnailFile.Name));
				return thumbUrl + "?" + FileManager.FileSystemProvider.GetLastWriteTime(file).ToFileTime().ToString();
			}
			return null;
		}
		string GetDefaultThumbnailUrl() {
			return "";
		}
		static string[] CanGenerateThumbnailList = new string[] {
			".png",
			".gif",
			".jpg",
			".jpeg",
			".ico",
			".bmp"
		};
		bool CanGenerateThumbnail(string extension) {
			return !string.IsNullOrEmpty(FileManager.ThumbnailsFolderPath) && Array.Exists(CanGenerateThumbnailList, delegate(string s) {
				return s.Equals(extension, StringComparison.OrdinalIgnoreCase);
			});
		}
		bool HasActualThumbnail(FileManagerFile file, FileInfo thumbnailFile) {
			if(!thumbnailFile.Exists)
				return false;
			if(FileManager.FileSystemProvider.GetLastWriteTime(file) > thumbnailFile.LastWriteTime) {
				try {
					thumbnailFile.Delete();
				}
				catch {
					throw new Exception(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorThumbnail));
				}
				return false;
			}
			return true;
		}
		bool GenerateThumbnail(Stream file, FileInfo thumbnailFile) {
			try {
				if(!Directory.Exists(thumbnailFile.DirectoryName))
					Directory.CreateDirectory(thumbnailFile.DirectoryName);
			}
			catch {
				throw new Exception(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorThumbnail));
			}
			try {
				GenerateThumbnail(file, thumbnailFile, FileManager.Helper.ThumbnailWidth, FileManager.Helper.ThumbnailHeight);
				return true;
			}
			catch {
				return false;
			}
		}
		string GetThumbnailFilePath(FileManagerFile file) {
			return FileManagerItem.PathCombine(
				FileManager.ThumbnailsFolderPath,
				string.Format(FileNameFormat, GetThumbnailFolderName(file.Folder.RelativeName), GetUriSafeFileName(GetThumbnailFileName(file)), FileExtension)
			);
		}
		string GetThumbnailFileName(FileManagerFile file) {
			string name = FileManagerEditHelper.GetPhysicalSafeUniqFileName(file.Name, ThumbNamesCache);
			DataSourceFileSystemProvider dataSourceProvider = this.FileManager.FileSystemProvider.Provider as DataSourceFileSystemProvider;
			if(dataSourceProvider == null)
				return name;
			string id = dataSourceProvider.RootEntity.Find(file.RelativeName, false, false).ID.ToString();
			return name + "_" + FileManagerEditHelper.GetPhysicalSafeFileName(name);
		}
		static string GetUriSafeFileName(string name) {
			return name.Replace("&", "[amp];").Replace("#", "[sharp]").Replace("+", "[plus]");
		}
		string GetThumbnailFolderName(string directoryName) {
			return CommonUtils.GetMD5Hash(directoryName + "_fmSize_" + FileManager.Helper.ThumbnailWidth.ToString() + FileManager.Helper.ThumbnailHeight.ToString());
		}
		System.Drawing.Bitmap ChangeImageSize(System.Drawing.Image original, int width, int height) {
			System.Drawing.Bitmap thumbnail = new System.Drawing.Bitmap(width, height);
			int newHeight = original.Height;
			int newWidth = original.Width;
			if(original.Height > height || original.Width > width) {
				newHeight = (original.Height > original.Width) ? height : (int)(height * original.Height / original.Width);
				newWidth = (original.Width > original.Height) ? width : (int)(width * original.Width / original.Height);
			}
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumbnail);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			int top = (int)(height - newHeight) / 2;
			int left = (int)(width - newWidth) / 2;
			g.DrawImage(original, left, top, newWidth, newHeight);
			return thumbnail;
		}
		void GenerateThumbnail(Stream file, FileInfo thumbnailFile, int width, int height) {
			System.Drawing.Image original = System.Drawing.Image.FromStream(file);
			System.Drawing.Bitmap thumbnail = ChangeImageSize(
				original,
				width,
				height
			);
			try {
				thumbnail.Save(thumbnailFile.FullName);
			}
			catch {
				throw new Exception(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorThumbnail));
			}
			finally {
				thumbnail.Dispose();
				original.Dispose();
			}
		}
		public void TryRemoveFileThumbnail(FileManagerFile file) {
			try {
				File.Delete(GetThumbnailFilePath(file));
			}
			catch { }
		}
		public void TryRemoveFolderThumbnail(FileManagerFolder folder) {
			try {
				Directory.Delete(FileManagerItem.PathCombine(FileManager.ThumbnailsFolderPath, GetThumbnailFolderName(folder.RelativeName)), true);
			}
			catch { }
		}
	}
	public class FileManagerClientStateHelper {
		internal const string
			CurrentPathFieldName = "currentPath",
			Item = "item",
			ItemSelected = "selected",
			ItemFocused = "focused",
			ItemFilter = "filter",
			VirtScrollItemIndexFieldName = "virtScrollItemIndex",
			VirtScrollPageItemsCountFieldName = "virtScrollPageItemsCount";
		bool pathSynchronized;
		ASPxFileManager fileManager;
		Hashtable state;
		public FileManagerClientStateHelper(ASPxFileManager fileManager) {
			this.fileManager = fileManager;
		}
		public ASPxFileManager FileManager { get { return fileManager; } }
		public Hashtable State { get { return state; } }
		public bool PathSynchronized { 
			get { 
				return pathSynchronized || State == null || !State.ContainsKey(CurrentPathFieldName); 
			} 
			set { pathSynchronized = value; } }
		public void SyncClientState(Hashtable state) {
			this.state = state;
			SyncCurrentPath();
			SyncVirtScrollState();
		}
		public void ResetClientState() {
			SyncClientState(null);
		}
		void SyncVirtScrollState() {
			if(State != null && State.ContainsKey(VirtScrollItemIndexFieldName))
				FileManager.Helper.Data.VirtScrollItemIndex = (int)State[VirtScrollItemIndexFieldName];
		}
		protected internal void SyncCurrentPath() {
			if(State != null && State.ContainsKey(CurrentPathFieldName)) {
				var currentPathArgs = ((string)State[CurrentPathFieldName]).Split(new string[] { FileManagerCommandsHelper.ArgumentSeparator }, StringSplitOptions.None);
				var folder = new FileManagerFolder(FileManager.FileSystemProvider, currentPathArgs[0]);
				var expandNode = currentPathArgs.Length > 1 ? bool.Parse(currentPathArgs[1]) : true;
				PathSynchronized = FileManager.Helper.Data.SelectFolder(FileManager.Folders, folder, false, expandNode);
			}
		}
		protected internal void ResetPathState() {
			if(State != null && State.ContainsKey(CurrentPathFieldName))
				State.Remove(CurrentPathFieldName);
		}
		protected internal void ResetItemFilter() {
			if(State != null) {
				Hashtable itemState = GetFileState();
				if(itemState != null && itemState.ContainsKey(ItemFilter))
					itemState.Remove(ItemFilter);
			}
		}
		protected internal string GetItemFilter() {
			if(State != null) {
				Hashtable itemState = GetFileState();
				if(itemState != null && itemState.ContainsKey(ItemFilter))
					return (string)itemState[ItemFilter];
			}
			return null;
		}
		protected internal ClientItemInfo[] GetSelectedItemsInfo(bool folders) {
			var fileState = GetFileState();
			var result = new List<ClientItemInfo>();
			if(fileState != null && fileState.ContainsKey(ItemSelected)) {
				ArrayList itemsInfo = fileState[ItemSelected] as ArrayList;
				if(itemsInfo != null) {
					foreach(string itemInfo in itemsInfo) {
						var itemProperties = itemInfo.Split(new string[] { FileManagerCommandsHelper.ItemPropertySeparator }, StringSplitOptions.None);
						if(bool.Parse(itemProperties[2]) == folders)
							result.Add(new ClientItemInfo(itemProperties[0], itemProperties[1]));
					}
				}
			}
			return result.ToArray();
		}
		protected internal string[] GetSelectedItemsIds() {
			return GetSelectedFilesInfo().
				Concat(GetSelectedItemsInfo(true)).
				Select(i => i.Id).
				ToArray();
		}
		protected internal ClientItemInfo[] GetSelectedFilesInfo() {
			return GetSelectedItemsInfo(false);
		}
		protected internal string GetFocusedItemName(bool folder) {
			var fileState = GetFileState();
			if(fileState != null && fileState.ContainsKey(ItemFocused)) {
				var itemProperties = fileState[ItemFocused].ToString().Split(new string[] { FileManagerCommandsHelper.ItemPropertySeparator }, StringSplitOptions.None);
				if(bool.Parse(itemProperties[1]) == folder)
					return itemProperties[0];
			}
			return string.Empty;
		}
		Hashtable GetFileState() { 
			return state.ContainsKey(Item) ? state[Item] as Hashtable : null;
		}
	}
	public class FileManagerEditHelper {
		delegate Stream GetFileStreamDelegate(FileManagerFile file);
		readonly string[] ReservedNames = new string[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", 
			"COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
		ASPxFileManager fileManager;
		string customErrorText;
		string downloadTempDirPath;
		public FileManagerEditHelper(ASPxFileManager fileManager) {
			this.fileManager = fileManager;
		}
		ASPxFileManager FileManager { get { return this.fileManager; } }
		FileSystemProviderBase FileSystemProvider { get { return FileManager.FileSystemProvider; } }
		string CustomErrorText { get { return customErrorText; } set { customErrorText = value; } }
		internal string DownloadError { get; set; }
		public void DeleteFile(FileManagerFile file) {
			ValidateEventArgs(FileManager.RaiseItemDeleting(file));
			ValidateFoldersChain(file.Folder);
			ValidateExists(file);
			FileSystemProvider.DeleteFile(file);
			CleanUpThumbnails(file);
		}
		public void DeleteFolder(FileManagerFolder folder) {
			ValidateEventArgs(FileManager.RaiseItemDeleting(folder));
			ValidateFoldersChain(folder);
			ValidateExists(folder);
			FileSystemProvider.DeleteFolder(folder);
			CleanUpThumbnails(folder);
		}
		public void RenameFile(string newName, FileManagerFile file) {
			ValidateEventArgs(FileManager.RaiseItemRenaming(newName, file));
			ValidateExtension(newName);
			ValidateName(newName);
			FileSystemProvider.RenameFile(file, newName);
			CleanUpThumbnails(file);
		}
		public void RenameFolder(FileManagerFolder Folder, string newName) {
			ValidateEventArgs(FileManager.RaiseItemRenaming(newName, Folder));
			ValidateName(newName);
			FileManagerFolder parentFolder = Folder.Parent;
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(parentFolder.RelativeName, null);
			FileManagerFolder newFolder = FileManagerFolder.Create(FileSystemProvider, parentFolder, newName, newIdPath);
			ValidateAlreadyExists(newFolder);
			FileSystemProvider.RenameFolder(Folder, newName);
			CleanUpThumbnails(Folder);
		}
		public void MoveFile(FileManagerFolder target, FileManagerFile file) {
			ValidateName(file.Name);
			ValidateFoldersChain(target);
			ValidateEventArgs(FileManager.RaiseItemMoving(target, file));
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(target.RelativeName, null);
			FileManagerFile newFile = FileManagerFile.Create(FileSystemProvider, target, file.Name, newIdPath);
			ValidateAlreadyExists(newFile);
			FileSystemProvider.MoveFile(file,target);
			CleanUpThumbnails(file);
		}
		public void CopyFile(FileManagerFolder target, FileManagerFile file) {
			ValidateName(file.Name);
			ValidateFoldersChain(target);
			ValidateEventArgs(FileManager.RaiseItemCopying(target, file));
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(target.RelativeName, null);
			FileManagerFile newFile = FileManagerFile.Create(FileSystemProvider, target, file.Name, newIdPath);
			ValidateAlreadyExists(newFile);
			FileSystemProvider.CopyFile(file,target);
			CleanUpThumbnails(file);
		}
		public void MoveFolder(FileManagerFolder targetFolder, FileManagerFolder folderToMove) {
			ValidateFoldersChain(targetFolder);
			ValidateEventArgs(FileManager.RaiseItemMoving(targetFolder, folderToMove));
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(targetFolder.RelativeName, null);
			FileManagerFolder newFolder = FileManagerFolder.Create(FileSystemProvider, targetFolder, folderToMove.Name, newIdPath);
			ValidateAlreadyExists(newFolder);
			FileSystemProvider.MoveFolder(folderToMove, targetFolder);
			CleanUpThumbnails(folderToMove);
		}
		public void CopyFolder(FileManagerFolder targetFolder, FileManagerFolder folderToCopy) {
			ValidateFoldersChain(targetFolder);
			ValidateEventArgs(FileManager.RaiseItemCopying(targetFolder, folderToCopy));
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(targetFolder.RelativeName, null);
			FileManagerFolder newFolder = FileManagerFolder.Create(FileSystemProvider, targetFolder, folderToCopy.Name, newIdPath);
			ValidateAlreadyExists(newFolder);
			FileSystemProvider.CopyFolder(folderToCopy, targetFolder);
			CleanUpThumbnails(folderToCopy);
		}
		public void CreateFolder(string name) {
			FileManagerFolder selectedFolder = FileManager.SelectedFolder;
			ValidateEventArgs(FileManager.RaiseFolderCreating(name, selectedFolder));
			ValidateName(name);
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(selectedFolder.RelativeName, null);
			FileManagerFolder newFolder = FileManagerFolder.Create(FileSystemProvider, selectedFolder, name, newIdPath);
			ValidateAlreadyExists(newFolder);
			FileSystemProvider.CreateFolder(selectedFolder, name);
		}
		public string UploadFile(UploadedFile file) {
			ValidateStateSynchronizer();
			FileManagerFolder folder = FileManager.SelectedFolder;
			string[] newIdPath = FileManager.Helper.Data.GetItemIdPath(folder.RelativeName, null);
			FileManagerFile newFile = FileManagerFile.Create(FileSystemProvider, folder, file.FileName, newIdPath);
			FileManagerFileUploadEventArgs args = FileManager.RaiseFileUploading(newFile, file.FileContent);
			try {
				ValidateEventArgs(args);
				FileManagerFile newFileForUpload = FileManagerFile.Create(FileSystemProvider, folder, args.FileName, newIdPath);
				ValidateAlreadyExists(newFileForUpload);
				args.InputStream.Position = 0;
				if(args.OutputStream != null)
					args.OutputStream.Position = 0;
				ProcessContentType(file);
				string fileName = args.FileName.Trim(' ', '.');
				Stream fileStream = args.OutputStream == null ? args.InputStream : args.OutputStream;
				FileSystemProvider.UploadFile(folder, fileName, fileStream);
				return fileName;
			}
			finally {
				if(!file.PostedFileInternal.IsStandardMode())
					args.InputStream.Dispose(); 
			}
		}
		public Stream DownloadFile(string[] fileNames, string[] fileIDs, out string name, out string fileFormat) {
			Stream resultStream = null;
			name = string.Empty;
			fileFormat = string.Empty;
			string zipName = FileManager.SettingsEditing.DownloadedArchiveName + ".zip";
			List<FileManagerFileDownloadingEventArgs> downloadArgs = new List<FileManagerFileDownloadingEventArgs>();
			try {
				downloadTempDirPath = GetTempDirectory();
				var zipPath = FileManagerItem.PathCombine(downloadTempDirPath, zipName);
				bool hasContent = false;
				FileManagerFolder folder = FileManager.SelectedFolder;
				using(var zip = new InternalZipArchive(zipPath)) {
					Hashtable fileNamesCache = new Hashtable();
					for(int i = 0; i < fileNames.Length; i++) {
						var fileName = fileNames[i];
						ValidateName(fileName);
						string[] fileIdPath = FileManager.Helper.Data.GetItemIdPath(folder.RelativeName, fileIDs[i]);
						FileManagerFile file = FileManagerFile.Create(FileManager.FileSystemProvider, folder, fileName, fileIdPath);
						ValidateDownload(file);
						using(Stream stream = GetFileStream(file)) {
							FileManagerFileDownloadingEventArgs args = FileManager.RaiseDownloading(file, stream);
							ValidateEventArgs(args);
							var fileStream = args.OutputStream != null ? args.OutputStream : stream;
							fileStream.Position = 0;
							zip.Add(GetPhysicalSafeUniqFileName(file.Name, fileNamesCache), file.LastWriteTime, fileStream);
							fileStream.Dispose();
							hasContent = true;
						}
					}
				}
				if(hasContent) {
					resultStream = File.OpenRead(zipPath);
					name = Path.GetFileNameWithoutExtension(zipName);
					fileFormat = "zip";
				}
			}
			catch {
				ClenUpDownloadTempDirectory();
			}
			return resultStream;
		}
		public Stream DownloadFile(string fileName, string id, out string name, out string fileFormat) {
			name = string.Empty;
			fileFormat = string.Empty;
			FileManagerFile file;
			Stream resultStream = PrepareFileDownload(fileName, id, GetFileStream, out file);
			name = Path.GetFileNameWithoutExtension(file.Name);
			fileFormat = file.Extension.TrimStart('.');
			return resultStream;
		}
		public string GetCloudDownloadUrl(string fileName, string fileId) {
			FileManagerFile file;
			PrepareFileDownload(fileName, fileId, GetFileEmptyStream, out file);
			CloudFileSystemProviderBase cloudProvider = FileManager.FileSystemProvider.Provider as CloudFileSystemProviderBase;
			return cloudProvider.GetDownloadUrl(new FileManagerFile[] { file });
		}
		Stream PrepareFileDownload(string fileName, string id, GetFileStreamDelegate fileStreamGetter, out FileManagerFile file) {
			Stream resultStream;
			ValidateName(fileName);
			FileManagerFolder folder = FileManager.SelectedFolder;
			string[] fileIdPath = FileManager.Helper.Data.GetItemIdPath(folder.RelativeName, id);
			file = FileManagerFile.Create(FileManager.FileSystemProvider, folder, fileName, fileIdPath);
			ValidateDownload(file);
			Stream stream = fileStreamGetter(file);
			FileManagerFileDownloadingEventArgs args = FileManager.RaiseDownloading(file, stream);
			if(args.Cancel && stream != null)
				stream.Dispose();
			ValidateEventArgs(args);
			resultStream = args.OutputStream != null ? args.OutputStream : stream;
			return resultStream;
		}
		Stream GetFileEmptyStream(FileManagerFile file) {
			return null;
		}
		Stream GetFileStream(FileManagerFile file) {
			var cloudProvider = FileManager.FileSystemProvider.Provider as CloudFileSystemProviderBase;
			return cloudProvider == null ? FileManager.FileSystemProvider.ReadFile(file) : cloudProvider.GetFileStream(file);
		}
		void ProcessContentType(UploadedFile file) {
			CloudFileSystemProviderBase provider = FileManager.FileSystemProvider.Provider as CloudFileSystemProviderBase;
			if(provider != null)
				provider.RegisterContentType(Path.GetExtension(file.FileName), file.ContentType);
		}
		protected void ValidateExtension(string name) {
			List<string> extensions = new List<string>(FileManager.Settings.AllowedFileExtensions);
			string extension = FileManagerFile.GetExtension(name);
			if(extensions.Count != 0 && string.IsNullOrEmpty(extensions.Find(delegate(string ext) { return ext.Equals(extension, StringComparison.OrdinalIgnoreCase); })))
				throw new FileManagerException(FileManagerErrors.WrongExtension);
		}
		protected internal void ValidateName(string name) {
			if(string.IsNullOrEmpty(name))
				throw new FileManagerException(FileManagerErrors.EmptyName);
			if(name.IndexOfAny(GetInvalidFileNameChars()) > -1)
				throw new FileManagerException(FileManagerErrors.InvalidSymbols);
			if(FileManagerItem.DotsPathRegex.IsMatch(name))
				throw new FileManagerException(FileManagerErrors.InvalidSymbols);
			if(ReservedNames.Contains(name, StringComparer.InvariantCultureIgnoreCase))
				throw new FileManagerException(FileManagerErrors.InvalidSymbols);
		}
		protected void ValidateEventArgs(FileManagerActionEventArgsBase eventArgs) {
			if(eventArgs.Cancel)
				throw new FileManagerCancelException(eventArgs.ErrorText);
		}
		protected void ValidateExists(FileManagerFile file) {
			if(!FileSystemProvider.Exists(file))
				throw new FileManagerIOException(FileManagerErrors.FileNotFound);
		}
		protected void ValidateExists(FileManagerFolder folder) {
			if(!FileSystemProvider.Exists(folder))
				throw new FileManagerIOException(FileManagerErrors.FolderNotFound);
		}
		protected void ValidateAlreadyExists(FileManagerFile file) {
			if(FileSystemProvider.Exists(file))
				throw new FileManagerException(FileManagerErrors.AlreadyExists, new Exception(), "File \"" + file.Name + "\" already exists.");
		}
		protected void ValidateAlreadyExists(FileManagerFolder folder) {
			if(FileSystemProvider.Exists(folder))
				throw new FileManagerException(FileManagerErrors.AlreadyExists, new Exception(), "Folder \"" + folder.Name + "\" already exists.");
		}
		protected internal void ValidateStateSynchronizer() {
			if(!FileManager.Helper.ClientState.PathSynchronized)
				throw new FileManagerIOException(FileManagerErrors.FolderNotFound);
		}
		protected void ValidateRootFolder() {
			if(string.IsNullOrEmpty(FileManager.SelectedFolder.RelativeName))
				throw new FileManagerIOException(FileManagerErrors.FolderNotFound);
		}
		protected void ValidateDownload(FileManagerFile file) {
			if(!FileManager.FileSystemProvider.CanDownload(file))
				throw new FileManagerAccessException();
			if(!FileManager.FileSystemProvider.Exists(file))
				throw new FileManagerIOException(FileManagerErrors.FileNotFound);
		}
		protected internal void ValidateFoldersChain(FileManagerFolder folder) {
			string[] pathParts = folder.RelativeName.Split(FileManagerItem.Separators, StringSplitOptions.RemoveEmptyEntries);
			if(pathParts.Length > 0 && pathParts.Last() == FileManagerDataHelper.NewFolderNodeName)
				pathParts = pathParts.Take(pathParts.Length - 1).ToArray();
			var currentFolder = new FileManagerFolder(FileManager.FileSystemProvider, "");
			for(int i = 0; i < pathParts.Length; i++) {
				var pathPart = pathParts[i];
				var folders = FileManager.FileSystemProvider.GetFolders(currentFolder);
				if(!folders.Any(f => string.Equals(f.Name, pathParts[i], StringComparison.InvariantCultureIgnoreCase)))
					throw new FileManagerIOException(FileManagerErrors.FolderNotFound);
				else
					currentFolder = folders.Where(f => string.Equals(f.Name, pathParts[i], StringComparison.InvariantCultureIgnoreCase)).First();
			}
		}
		internal void ClenUpDownloadTempDirectory() {
			if(!string.IsNullOrEmpty(downloadTempDirPath))
				CleanUpTempDirectory(downloadTempDirPath);
			downloadTempDirPath = null;
		}
		protected void CleanUpThumbnails(FileManagerFolder folder) {
			FileManager.Helper.Thumbnails.TryRemoveFolderThumbnail(folder);
		}
		protected void CleanUpThumbnails(FileManagerFile file) {
			FileManager.Helper.Thumbnails.TryRemoveFileThumbnail(file);
		}
		string GetTempDirectory() {
			string tmpDirPath = FileManager.SettingsEditing.TemporaryFolder;
			var rootDirectory = new DirectoryInfo(tmpDirPath[0] == '~' ? FileManager.MapPath(tmpDirPath) : tmpDirPath);
			if(!rootDirectory.Exists) {
				rootDirectory.Create();
				rootDirectory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
			}
			var dirPath = FileManagerItem.PathCombine(rootDirectory.FullName, Guid.NewGuid().ToString());
			Directory.CreateDirectory(FileManagerItem.PathCombine(rootDirectory.FullName, dirPath));
			return dirPath;
		}
		char[] GetInvalidFileNameChars() {
			if(FileManager.FileSystemProvider.Provider is CloudFileSystemProviderBase)
				return new char[] { Convert.ToChar(0) };
			return Path.GetInvalidFileNameChars();
		}
		void CleanUpTempDirectory(string lastTempDir) {
			DoSafeOperation(() => {
				if(Directory.Exists(lastTempDir))
					Directory.Delete(lastTempDir, true);
			});
			string rootDirectory = Path.GetDirectoryName(lastTempDir);
			if(Directory.Exists(rootDirectory)) {
				foreach(var tempDir in Directory.GetDirectories(rootDirectory)) {
					DoSafeOperation(() => {
						if(DateTime.Now - Directory.GetLastWriteTime(tempDir) > TimeSpan.FromDays(1))
							Directory.Delete(tempDir, true);
					});
				}
				DoSafeOperation(() => {
					if(Directory.GetDirectories(rootDirectory).Length == 0)
						Directory.Delete(rootDirectory, true);
				});
			}
		}
		static void DoSafeOperation(Action action) {
			try {
				action();
			}
			catch { }
		}
		static internal string GetPhysicalSafeFileName(string name) {
			return Regex.Replace(name, @"[" + new string(Path.GetInvalidFileNameChars()) + "]", "_");
		}
		static internal string GetPhysicalSafeUniqFileName(string name, Hashtable namesCache) {
			if(name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
				return name;
			name = GetPhysicalSafeFileName(name);
			int i = 2;
			var uniqName = name;
			while(namesCache.ContainsKey(uniqName)) {
				uniqName = name + "(" + i + ")";
				i++;
			}
			namesCache.Add(uniqName, null);
			return uniqName;
		}
	}
	public class FilesGridViewRenderHelper : GridViewRenderHelper {
		public FilesGridViewRenderHelper(ASPxGridView grid)
			: base(grid) {
		}
		protected override GridViewDataItemTemplateContainer CreateDataItemTemplateContainer(ASPxGridView grid, object row, int visibleIndex, GridViewDataColumn column) {
			return new FileManagerDetailsViewItemTemplateContainer(Grid, row, visibleIndex, column, grid.GetRow(visibleIndex) as FileManagerItem);
		}
	}
	public class ClientItemInfo {
		public ClientItemInfo(string id, string name) {
			Id = id;
			Name = name;
		}
		public string Id { get; set; }
		public string Name { get; set; }		
	}
	public enum FileManagerItemType {
		File = 0,
		Folder = 1,
		ParentFolder = 2
	}
}
