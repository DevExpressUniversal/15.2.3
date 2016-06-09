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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public class FileManagerControl : ASPxInternalWebControl {
		const int DefaultHeight = 500;
		internal const string SplitterID = "Splitter";
		const string RenameInputID = "RFI";
		const string FolderBrowserPopupID = "FolderBrowserPopup";
		const string UploadProgressPopupID = "UploadProgressPopup";
		const string SelectedItemsPopupID = "SelectedItemsPopup";
		const string BreadCrumbsPopupID = "BreadCrumbsPopup";
		const string FolderBrowserPopupShownEventHandler = "ASPxClientFileManager.OnFolderBrowserPopupShown";
		const string FolderBrowserPopupClosingEventHandler = "ASPxClientFileManager.OnFolderBrowserPopupClosing";
		ASPxFileManager fileManager;
		FileManagerContainer container;
		TextBox tbRename;
		FileManagerFolderBrowserPopup folderBrowserPopup;
		FileManagerUploadProgressPopup uploadProgressPopup;
		public FileManagerControl(ASPxFileManager fileManager)
			: base() {
			this.fileManager = fileManager;
		}
		public ASPxFileManager FileManager { get { return fileManager; } }
		public FileManagerContainer Container { get { return container; } }
		public TextBox TbRename { get { return tbRename; } }
		public FileManagerFolderBrowserPopup FolderBrowserPopup { get { return folderBrowserPopup; } }
		public FileManagerUploadProgressPopup UploadProgressPopup { get { return uploadProgressPopup; } }
		public ASPxPopupControl BreadCrumbsPopup { get; private set; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.container = CreateFileManagerContainer();
			Controls.Add(Container);
			if(FileManager.SettingsEditing.AllowRename || (FileManager.SettingsFileList.ShowFolders && FileManager.SettingsEditing.AllowCreate)) {
				this.tbRename = new InternalTextBox();
				TbRename.ID = RenameInputID;
				Controls.Add(TbRename);
			}
			if(!FileManager.DesignMode) {
				CreateFolderBrowserPopup();
				if(!FileManager.SettingsUpload.ShowUploadPanel)
					CreateUploadProgressPopup();
				if(FileManager.SettingsBreadcrumbs.Visible)
					CreateBreadCrumbsPopup();
			}
		}
		protected virtual FileManagerContainer CreateFileManagerContainer() {
			return new FileManagerContainer(this);
		}
		void CreateFolderBrowserPopup() {
			this.folderBrowserPopup = new FileManagerFolderBrowserPopup(FileManager);
			FolderBrowserPopup.ID = FolderBrowserPopupID;
			FolderBrowserPopup.EnableViewState = false;
			FolderBrowserPopup.ClientSideEvents.Closing = FolderBrowserPopupClosingEventHandler;
			FolderBrowserPopup.ClientSideEvents.Shown = FolderBrowserPopupShownEventHandler;
			FolderBrowserPopup.Visible = (FileManager.SettingsEditing.AllowMove || FileManager.SettingsEditing.AllowCopy) && (FileManager.SettingsFolders.Visible || FileManager.SettingsFileList.ShowFolders);
			Controls.Add(FolderBrowserPopup);
		}
		void CreateUploadProgressPopup() {
			this.uploadProgressPopup = new FileManagerUploadProgressPopup(FileManager);
			UploadProgressPopup.ID = UploadProgressPopupID;
			UploadProgressPopup.EnableViewState = false;
			Controls.Add(UploadProgressPopup);
		}
		void CreateBreadCrumbsPopup() {
			BreadCrumbsPopup = new ASPxPopupControl();
			BreadCrumbsPopup.ID = BreadCrumbsPopupID;
			BreadCrumbsPopup.PopupAnimationType = AnimationType.Fade;
			BreadCrumbsPopup.ShowCloseButton = false;
			BreadCrumbsPopup.PopupVerticalAlign = PopupVerticalAlign.Below;
			BreadCrumbsPopup.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			BreadCrumbsPopup.ShowHeader = false;
			BreadCrumbsPopup.ShowShadow = false;
			BreadCrumbsPopup.CloseAction = CloseAction.OuterMouseClick;
			Controls.Add(BreadCrumbsPopup);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(FileManager, this);
			Width = GetWidth();
			Height = GetHeight();
			if(FolderBrowserPopup != null && Height.Type != UnitType.Percentage)
				FolderBrowserPopup.Height = new Unit(Height.Value / 2, Height.Type);
			FileManager.Helper.GetRootControlStyle().AssignToControl(this);
			if(TbRename != null) {
				TbRename.CssClass = FileManagerStyles.RenameFileInputCssClass;
				TbRename.Font.CopyFrom(FileManager.Font);
				RenderUtils.SetStyleStringAttribute(TbRename, "display", "none");
			}
			if(BreadCrumbsPopup != null)
				BreadCrumbsPopup.CssClass = FileManagerStyles.BreadcrumbsPopupCssClass;
		}
		protected override void ClearControlFields() {
			this.container = null;
			this.tbRename = null;
			this.folderBrowserPopup = null;
			this.uploadProgressPopup = null;
		}
		protected internal Unit GetHeight() {
			return Height.IsEmpty ? Unit.Pixel(DefaultHeight) : Height;
		}
		protected internal Unit GetWidth() {
			return Width.IsEmpty ? Unit.Percentage(100) : Width;
		}
	}
	[ToolboxItem(false)]
	public class FileManagerContainer : ASPxInternalWebControl {
		const string ToolbarID = "Toolbar";
		const string ContextMenuID = "ContextMenu";
		const string FilterID = "Filter";
		const string FoldersID = "Folders";
		const string ItemsID = "Items";
		const string GridViewID = "FilesGridView";
		const string UploadControlID = "Upload";
		const string UploadButtonID = "UploadButton";
		const string BreadCrumbsPaneName = "BreadCrumbsPane";
		const string ItemsPaneName = "ItemsPane";
		const string UploadControlPaneName = "UploadPanelPane";
		const string FoldersNodeClickEventHandler = "ASPxClientFileManager.OnFoldersTreeViewNodeClick";
		const string FoldersNodeExpandingEventHandler = "ASPxClientFileManager.OnFoldersTreeViewNodeExpanding";
		const string UploadControlFileUploadStartEventHandler = "ASPxClientFileManager.OnUploadControlFileUploadStartEventHandler";
		const string UploadControlUploadingProgressChangedEventHandler = "ASPxClientFileManager.OnUploadControlUploadingProgressChanged";
		const string UploadControlFilesUploadCompleteEventHandler = "ASPxClientFileManager.OnUploadControlFilesUploadComplete";
		const string UploadTextChangedEventHandler = "ASPxClientFileManager.OnUploadControlTextChanged";
		const string ToolbarMenuItemClickEventHandler = "ASPxClientFileManager.OnToolbarMenuItemClick";
		const string ContextMenuItemClickEventHandler = "ASPxClientFileManager.OnContextMenuItemClick";
		const string PaneResizeCompletedEventHandler = "ASPxClientFileManager.OnPaneResizeCompleted";
		const string DefaultCustomColumnFieldName = "CustomColumnDefaultFieldName";
		protected readonly Unit SeparatorSize = Unit.Pixel(3);
		FileManagerControl owner;
		FileManagerToolbar toolbar;
		FileManagerContextMenu contextMenu;
		FileManagerFolders folders;
		FileManagerItems itemsControl;
		FileManagerGridView filesGridView;
		ASPxUploadControl uploadControl;
		Table uploadPanelTable;
		TableCell uploadPanelTableButtonCell;
		Table toolbarTable;
		InternalHyperLink uploadButton;
		InternalTableCell filterCell;
		TextBox tbFilter;
		public FileManagerContainer(FileManagerControl owner)
			: base() {
			this.owner = owner;
		}
		public FileManagerControl Owner { get { return owner; } }
		public ASPxFileManager FileManager { get { return Owner.FileManager; } }
		public FileManagerToolbar Toolbar { get { return toolbar; } }
		public FileManagerContextMenu ContextMenu { get { return contextMenu; } }
		public FileManagerFolders Folders { get { return folders; } }
		public FileManagerItems ItemsControl { get { return itemsControl; }  }
		public FileManagerGridView FilesGridView { get { return filesGridView; } }
		public ASPxUploadControl UploadControl { get { return uploadControl; } }
		public Table UploadPanelTable { get { return uploadPanelTable; } }
		public TableCell UploadPanelTableButtonCell { get { return uploadPanelTableButtonCell; } }
		public Table ToolbarTable { get { return toolbarTable; } }
		public InternalHyperLink UploadButton { get { return uploadButton; } }
		public InternalTableCell FilterCell { get { return filterCell; } }
		public TextBox TbFilter { get { return tbFilter; } }
		ASPxSplitter splitter;
		ASPxSplitter Splitter { get { return splitter; } }
		SplitterPane ContentPane1 { get { return Splitter.Panes[1]; } }
		SplitterPane ContentPane2 { get { return ContentPane1.Panes[1]; } }
		SplitterPane ToolbarPane { get { return Splitter.Panes[0]; } }
		SplitterPane FoldersPane { get { return ContentPane1.Panes[0]; } }
		SplitterPane BreadCrumbsPane { get { return ContentPane2.Panes[BreadCrumbsPaneName]; } }
		SplitterPane ItemsPane { get { return ContentPane2.Panes[ItemsPaneName]; } }
		SplitterPane UploadPanelPane { get { return ContentPane2.Panes[UploadControlPaneName]; } }
		InternalTable table;
		InternalTableCell toolbarCell;
		InternalTableCell foldersCell;
		InternalTableCell filesCell;
		InternalTableCell breadCrumbsCell;
		InternalTableCell uploadCell;
		InternalTableCell uploadPanelCell;
		InternalTableCell separatorCell;
		FileManagerUploadCommand uploadCommand;
		InternalTable Table { get { return table; } }
		InternalTableCell ToolbarCell { get { return toolbarCell; } }
		InternalTableCell FoldersCell { get { return foldersCell; } }
		InternalTableCell FilesCell { get { return filesCell; } }
		InternalTableCell BreadCrumbsCell { get { return breadCrumbsCell; } }
		InternalTableCell UploadCell { get { return uploadCell; } }
		InternalTableCell UploadPanelCell { get { return uploadPanelCell; } }
		InternalTableCell SeparatorCell { get { return separatorCell; } }
		FileManagerUploadCommand UploadCommand {
			get {
				if(this.uploadCommand == null)
					this.uploadCommand = new FileManagerUploadCommand(FileManager);
				return this.uploadCommand;
			}
			set {
				this.uploadCommand = value;
			}
		}
		protected override void ClearControlFields() {
			this.folders = null;
			this.itemsControl = null;
			this.toolbar = null;
			this.uploadControl = null;
			this.tbFilter = null;
			this.table = null;
			this.splitter = null;
		}
		protected override void CreateControlHierarchy() {
			CreateSplitter();
			if(DesignMode)
				CreateDesignModeTable();
			CreateUploadPanel();
			CreateToolbar();
			if(!DesignMode || FileManager.SettingsFolders.Visible)
				CreateFolders();
			if(FileManager.SettingsFileList.View == FileListView.Thumbnails)
				CreateFilesControl();
			else 
				CreateFilesGridView();			
			base.CreateControlHierarchy();
		}
		protected void CreateSplitter() {
			this.splitter = new ASPxSplitter();
			Controls.Add(Splitter);
			Splitter.Orientation = Orientation.Vertical;
			Splitter.Height = Unit.Percentage(100);
			Splitter.Width = Unit.Percentage(100);
			Splitter.SeparatorSize = Unit.Pixel(1);
			Splitter.ShowSeparatorImage = false;
			Splitter.PaneMinSize = Unit.Pixel(10);
			Splitter.ResizingMode = ResizingMode.Postponed;
			Splitter.ID = "Splitter";
			Splitter.ClientSideEvents.PaneResizeCompleted = PaneResizeCompletedEventHandler;
			Splitter.Panes.Add("ToolbarPane");
			ToolbarPane.AllowResize = DefaultBoolean.False;
			ToolbarPane.Visible = ShowToolbar();
			Splitter.Panes.Add("ContentPane1");
			ContentPane1.Panes.Add("FoldersPane");
			FoldersPane.Size = FileManager.Helper.GetFoldersContainerWidth();
			FoldersPane.ScrollBars = ScrollBars.Auto;
			FoldersPane.Visible = FileManager.SettingsFolders.Visible;
			ContentPane1.Panes.Add("ContentPane2");
			ContentPane2.Separator.Size = SeparatorSize;
			if(FileManager.SettingsBreadcrumbs.Position == BreadcrumbsPosition.Top)
				CreateBreadCrumbsPane();
			ContentPane2.Panes.Add(ItemsPaneName);
			ItemsPane.ScrollBars = ScrollBars.Auto;
			if(FileManager.SettingsBreadcrumbs.Position == BreadcrumbsPosition.Bottom)
				CreateBreadCrumbsPane();
			ContentPane2.Panes.Add(UploadControlPaneName);
			UploadPanelPane.AllowResize = DefaultBoolean.False;
			UploadPanelPane.Collapsed = true; 
			if(Browser.Platform.IsWebKitTouchUI){
				ContentPane2.Separator.Size = 20;
				ContentPane2.ShowSeparatorImage = DefaultBoolean.True;
			}
			Splitter.RecreateControlHierarchy();
		}
		void CreateBreadCrumbsPane() {
			ContentPane2.Panes.Add(BreadCrumbsPaneName);
			BreadCrumbsPane.AllowResize = DefaultBoolean.False;
			BreadCrumbsPane.Visible = FileManager.Enabled && FileManager.SettingsBreadcrumbs.Visible;
		}
		protected void CreateDesignModeTable() {
			Splitter.Visible = false;
			this.table = new InternalTable();
			Controls.Add(Table);
			Table.Height = Unit.Percentage(100);
			Table.Width = Owner.GetWidth();
			Table.BorderWidth = Unit.Pixel(1);
			InternalTableRow tbRow = new InternalTableRow();
			Table.Controls.Add(tbRow);
			this.toolbarCell = new InternalTableCell();
			if(FileManager.SettingsFolders.Visible)
				ToolbarCell.ColumnSpan = 3;
			tbRow.Controls.Add(ToolbarCell);
			InternalTableRow bodyRow = new InternalTableRow();
			Table.Controls.Add(bodyRow);
			if(FileManager.SettingsFolders.Visible) {
				this.foldersCell = new InternalTableCell();
				FoldersCell.RowSpan = 2;
				FoldersCell.Height = Unit.Percentage(100);
				bodyRow.Controls.Add(FoldersCell);
				this.separatorCell = new InternalTableCell();
				SeparatorCell.RowSpan = 2;
				bodyRow.Controls.Add(SeparatorCell);
			}
			this.filesCell = new InternalTableCell();
			bodyRow.Controls.Add(FilesCell);
			InternalTableRow uploadRow = new InternalTableRow();
			Table.Controls.Add(uploadRow);
			uploadCell = new InternalTableCell();
			UploadCell.VerticalAlign = VerticalAlign.Bottom;
			uploadRow.Controls.Add(UploadCell);
			if(FileManager.SettingsBreadcrumbs.Visible)
				CreateDesignModeBreadCrumbs();
			InternalTable uptable = new InternalTable();
			uptable.Width = Unit.Percentage(100);
			UploadCell.Controls.Add(uptable);
			InternalTableRow uploadPanelRow = new InternalTableRow();
			uptable.Controls.Add(uploadPanelRow);
			this.uploadPanelCell = new InternalTableCell();
			uploadPanelRow.Controls.Add(UploadPanelCell);
		}
		protected void CreateToolbar() {
			this.toolbarTable = new InternalTable();
			ToolbarTable.CssClass = FileManagerStyles.SpecialCssClass;
			ToolbarTable.CellPadding = 0;
			ToolbarTable.CellSpacing = 0;
			TableRow toolbarTableRow = new InternalTableRow();
			ToolbarTable.Rows.Add(toolbarTableRow);
			CreateToolbarMenu(toolbarTableRow);
			if(this.FileManager.SettingsContextMenu.Enabled)
				CreateContextMenu();
			CreateToolbarFilterCell(toolbarTableRow);
			if(!DesignMode)
				ToolbarPane.Controls.Add(ToolbarTable);
			else
				ToolbarCell.Controls.Add(ToolbarTable);
		}
		protected void CreateDesignModeBreadCrumbs() {
			InternalTable bcTable = new InternalTable();
			bcTable.Width = Unit.Percentage(100);
			InternalTableCell parent = FileManager.SettingsBreadcrumbs.Position == BreadcrumbsPosition.Top ? FilesCell : UploadCell;
			parent.Controls.Add(bcTable);
			InternalTableRow bcRow = new InternalTableRow();
			bcTable.Controls.Add(bcRow);
			this.breadCrumbsCell = new InternalTableCell();
			bcRow.Controls.Add(BreadCrumbsCell);
			BreadCrumbsCell.VerticalAlign = VerticalAlign.Middle;
			if(FileManager.SettingsBreadcrumbs.ShowParentFolderButton) {
				WebControl imageContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				FileManager.Helper.GetBreadCrumbsItemStyle().DisabledStyle.AssignToControl(imageContainer);
				Image img = RenderUtils.CreateImage();
				FileManager.Helper.GetBreadCrumbsUpButtonImage().AssignToControl(img, DesignMode);
				imageContainer.Controls.Add(img);
				BreadCrumbsCell.Controls.Add(imageContainer);
			}
			WebControl item = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			item.Controls.Add(RenderUtils.CreateLiteralControl(FileManagerDesignProvider.RootFolderName));
			FileManager.Helper.GetBreadCrumbsItemStyle().AssignToControl(item);
			BreadCrumbsCell.Controls.Add(item);
		}
		protected void CreateToolbarMenu(TableRow parentRow) {
			TableCell cell = new InternalTableCell();
			this.toolbar = new FileManagerToolbar(this, ToolbarID);
			Toolbar.ClientSideEvents.ItemClick = ToolbarMenuItemClickEventHandler;
			cell.Controls.Add(Toolbar);
			cell.HorizontalAlign = FileManager.RightToLeft == DefaultBoolean.True ? HorizontalAlign.Right : HorizontalAlign.Left;
			parentRow.Cells.Add(cell);
		}
		protected void CreateContextMenu() {
			this.contextMenu = new FileManagerContextMenu(this, ContextMenuID);
			ContextMenu.ClientSideEvents.ItemClick = ContextMenuItemClickEventHandler;
			Controls.Add(ContextMenu);
		}
		protected void CreateToolbarFilterCell(TableRow parentRow) {
			this.filterCell = new InternalTableCell();
			parentRow.Controls.Add(FilterCell);
			Label label = new Label();
			label.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Filter) + " ";
			label.AssociatedControlID = FilterID;
			label.Visible = FileManager.IsFilterAvailable();
			FilterCell.Controls.Add(label);
			this.tbFilter = new InternalTextBox();
			TbFilter.ID = FilterID;
			TbFilter.Visible = FileManager.IsFilterAvailable();
			TbFilter.Width = FileManager.Helper.GetToolbarFilterTextBoxWidth();
			FilterCell.Controls.Add(TbFilter);
		}
		protected void CreateFolders() {
			this.folders = InitializeFolders();
			Folders.ID = FoldersID;
			Folders.ClientSideEvents.NodeClick = FoldersNodeClickEventHandler;
			Folders.ClientSideEvents.ExpandedChanging = FoldersNodeExpandingEventHandler;
			Folders.Images.ExpandButton.Assign(FileManager.Helper.GetImage(FileManagerImages.FolderExpandButtonImageName));
			Folders.Images.CollapseButton.Assign(FileManager.Helper.GetImage(FileManagerImages.FolderCollapseButtonImageName));
			Folders.Images.NodeLoadingPanel.Assign(FileManager.Helper.GetImage(FileManagerImages.FolderNodeLoadingPanelImageName));
			if(!DesignMode)
				FoldersPane.Controls.Add(Folders);
			else {
				Folders.Width = FileManager.Helper.GetFoldersContainerWidth();
				FoldersCell.Controls.Add(Folders);
			}
			if(!Folders.EnableCallBacks)
				Folders.RepopulateTree(false);
			else
				FileManager.Helper.Data.SyncFolders(Folders);
		}
		protected void CreateFilesControl() {
			this.itemsControl = new FileManagerItems(this);
			ItemsControl.ID = ItemsID;
			if(!DesignMode)
				ItemsPane.Controls.Add(ItemsControl);
			else
				FilesCell.Controls.Add(ItemsControl);
		}
		protected void CreateFilesGridView() {
			this.filesGridView = new FileManagerGridView(FileManager);
			FilesGridView.ID = GridViewID;
			FilesGridView.AutoGenerateColumns = DesignMode;
			if(FileManager.Settings.EnableMultiSelect) {
				FilesGridView.Columns.Add(new GridViewCommandColumn() {
					VisibleIndex = 0,
					ShowSelectCheckbox = true,
					Width = FileManager.Helper.GridViewCheckboxColumnWidth
				});
			}
			foreach(FileManagerDetailsColumn column in FileManager.SettingsFileList.DetailsViewSettings.ColumnsInternal) {
				var gridColumn = column is FileManagerDetailsCustomColumn ? CreateFilesGridViewCustomColumn((FileManagerDetailsCustomColumn)column) : CreateFilesGridViewDefaultColumn(column);
				if(column.ItemTemplate != null)
					gridColumn.DataItemTemplate = column.ItemTemplate;
				gridColumn.Settings.SortMode = XtraGrid.ColumnSortMode.Custom;
				FilesGridView.Columns.Add(gridColumn);
			}
			FilesGridView.CustomUnboundColumnData += FilesGridView_CustomUnboundColumnData;
			FilesGridView.CustomColumnDisplayText += FilesGridView_CustomColumnDisplayText;
			FilesGridView.HtmlDataCellPrepared += FilesGridView_HtmlDataCellPrepared;
			FilesGridView.CommandButtonInitialize += FilesGridView_CommandButtonInitialize;
			if(FileManager.SettingsFileList.ShowFolders || FileManager.SettingsFileList.ShowParentFolder)
				FilesGridView.CustomColumnSort += FilesGridView_CustomColumnSort;
			FilesGridView.EnableRowsCache = false;
			if(!DesignMode) {
				FilesGridView.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
				ItemsPane.Controls.Add(FilesGridView);
				FilesGridView.DataSource = FileManager.Helper.Data.GetItemsList(true);
				if(FileManager.SettingsFileList.DetailsViewSettings.ShowHeaderFilterButton || FileManager.SettingsFileList.DetailsViewSettings.Columns.Count(c => c is FileManagerDetailsCustomColumn) > 0)
					FilesGridView.HeaderFilterFillItems += FilesGridView_HeaderFilterFillItems;
				if(!FileManager.IsCallback)
					FilesGridView.DataBind();
				FilesGridView.FocusedRowIndex = -1;
				if(FileManager.IsVirtualScrollingEnabled()) {
					FilesGridView.Settings.VerticalScrollBarStyle = GridViewVerticalScrollBarStyle.Virtual;
					FilesGridView.SettingsPager.PageSize = FileManager.SettingsFileList.PageSize;
					FilesGridView.SettingsPager.Visible = false;
				}
			}
			else { 
				FileManagerDesignProvider provider = new FileManagerDesignProvider("", FileManager);
				FilesGridView.DataSource = provider.GetItems(FileManager.SettingsFileList.ShowFolders, FileManager.SettingsFileList.ShowParentFolder);
				FilesGridView.DataBind();
				FilesCell.Controls.Add(FilesGridView);
			}
		}
		protected virtual FileManagerFolders InitializeFolders() {
			return new FileManagerFolders(FileManager, false);
		}
		void FilesGridView_HeaderFilterFillItems(object sender, ASPxGridViewHeaderFilterEventArgs e) {
			if(FileManager.SettingsFileList.DetailsViewSettings.Columns.IsEmpty || !(FileManager.SettingsFileList.DetailsViewSettings.Columns[e.Column.Index] is FileManagerDetailsCustomColumn))
				FileManager.Helper.Data.GridViewHeaderFilterFillItems(e);
			else
				FileManager.RaiseDetailsViewCustomColumnHeaderFilterFillItems(new FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs(GetDetailsCustomColumn(FileManager.Settings.EnableMultiSelect ? e.Column.Index - 1 : e.Column.Index), e.Values));
		}
		void FilesGridView_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e) {
			if(DesignMode)
				return;
			var grid = (FileManagerGridView)sender;
			var item = grid.DataBoundProxy.GetRowByListSourceIndex(e.ListSourceRowIndex) as FileManagerItem;
			if(!(item is FileManagerFile))
				return;
			var column = GetDetailsCustomColumn(FileManager.Settings.EnableMultiSelect ? e.Column.Index - 1 : e.Column.Index);
			var eventResult = FileManager.RaiseDetailsViewColumnDisplayText(new FileManagerDetailsViewCustomColumnDisplayTextEventArgs(this.FileManager.FileSystemProvider.GetDetailsCustomColumnDisplayText(column), column, item as FileManagerFile));
			if(!eventResult.EncodeHtml)
				grid.CustomColumnsEncodeHtmlPropertiesCache[column.Index + item.FullName] = eventResult.EncodeHtml;
			e.Value = eventResult.DisplayText;
		}
		void FilesGridView_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			if(DesignMode)
				return;
			var grid = sender as FileManagerGridView;
			var item = grid.GetRow(e.VisibleRowIndex) as FileManagerItem;
			if(e.Column.UnboundType != UnboundColumnType.Bound) {
				var encodeHtmlPropertyKey = e.Column.Index + item.FullName;
				if(grid.CustomColumnsEncodeHtmlPropertiesCache.ContainsKey(encodeHtmlPropertyKey))
					e.EncodeHtml = grid.CustomColumnsEncodeHtmlPropertiesCache[encodeHtmlPropertyKey];
				return;
			}
			var viewColumn = FileManager.Helper.Data.GetFileInfoType(e.Column.FieldName);
			if(viewColumn != FileInfoType.Thumbnail) {
				bool encodeHtml;
				e.DisplayText = FileManager.Helper.Data.GetItemInfoDisplayText(item, viewColumn, out encodeHtml);
				e.EncodeHtml = encodeHtml;
			}
		}
		void FilesGridView_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e) {
			if(e.DataColumn.FieldName == FileManager.Helper.Data.GetFilePropertyName(FileInfoType.Thumbnail)) {
				foreach(var control in e.Cell.Controls) {
					var image = control as ImageDisplayControl;
					if(image == null || !FileManager.Helper.Data.CustomThumbnailsProperties.ContainsKey(e.KeyValue.ToString()))
						continue;
					image.ImageProperties.CopyFrom(FileManager.Helper.Data.CustomThumbnailsProperties[e.KeyValue.ToString()]);
				}
			}
			if(e.DataColumn.FieldName == FileManager.Helper.Data.GetFilePropertyName(FileInfoType.FileName))
				e.Cell.CssClass += " " + FileManagerStyles.FileColumnTitleCellCssClass;
		}
		void FilesGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e) {
			if(DesignMode)
				return;
			var items = FilesGridView.DataSource as IEnumerable<FileManagerItem>;
			var folder1 = items.ElementAt(e.ListSourceRowIndex1) as FileManagerFolder;
			var folder2 = items.ElementAt(e.ListSourceRowIndex2) as FileManagerFolder;
			bool isFolder1Exists = folder1 != null;
			bool isFolder2Exists = folder2 != null;
			if(!isFolder1Exists && !isFolder2Exists)
				return;			
			if(isFolder1Exists && !isFolder2Exists)
				e.Result = e.SortOrder == ColumnSortOrder.Ascending ? -1 : 1;
			else if(!isFolder1Exists && isFolder2Exists)
				e.Result = e.SortOrder == ColumnSortOrder.Ascending ? 1 : -1;
			else {
				if(folder1.IsParentFolderItem || folder2.IsCreateHelperItem)
					e.Result = e.SortOrder == ColumnSortOrder.Ascending ? -1 : 1;
				if(folder2.IsParentFolderItem || folder1.IsCreateHelperItem)
					e.Result = e.SortOrder == ColumnSortOrder.Ascending ? 1 : -1;
			}
			if(e.Result != 0)
				e.Handled = true;
		}
		void FilesGridView_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e) {
			var folder = ((ASPxGridView)sender).DataBoundProxy.GetRowByListSourceIndex(e.VisibleIndex) as FileManagerFolder;
			e.Visible = !(folder != null && folder.IsParentFolderItem);
		}
		protected GridViewDataColumn CreateFilesGridViewDefaultColumn(FileManagerDetailsColumn fileColumn) {
			GridViewDataColumn column;
			if(fileColumn.FileInfoType == FileInfoType.Thumbnail) {
				column = new GridViewDataImageColumn();
				var imageColumn = (GridViewDataImageColumn)column;
				imageColumn.PropertiesImage.ImageWidth = Unit.Pixel(FileManager.Helper.ThumbnailWidth);
				imageColumn.PropertiesImage.ImageHeight = Unit.Pixel(FileManager.Helper.ThumbnailHeight);
				column.Width = fileColumn.Width.IsEmpty ? FileManager.Helper.GetThumbnailColumnWidth(fileColumn.CellStyle) : fileColumn.Width;
			}
			else if(fileColumn.FileInfoType == FileInfoType.Size) {
				column = new GridViewDataTextColumn();
				column.Width = fileColumn.Width.IsEmpty ? FileManager.Helper.GetFileSizeColumnWidth() : fileColumn.Width;
			}
			else {
				column = new GridViewDataTextColumn();
				column.Width = fileColumn.Width;
			}
			column.Caption = fileColumn.Caption;
			PrepareFilesGridViewColumn(column, fileColumn);
			column.FieldName = FileManager.Helper.Data.GetFilePropertyName(fileColumn.FileInfoType);
			if(!DesignMode && fileColumn.FileInfoType == FileInfoType.FileName) {
				column.Settings.AllowHeaderFilter = DefaultBoolean.False;
				column.DataItemTemplate = new NameCellTemplate();
			}
			if(!DesignMode && fileColumn.FileInfoType == FileInfoType.Thumbnail) {
				column.Settings.AllowHeaderFilter = DefaultBoolean.False;
				column.CellStyle.CssClass = RenderUtils.CombineCssClasses(column.CellStyle.CssClass, FileManagerStyles.FileColumnThumbnailCssClass);
			}
			return column;
		}
		protected GridViewDataColumn CreateFilesGridViewCustomColumn(FileManagerDetailsCustomColumn fileColumn) {
			CheckCustomColumnName(fileColumn.Name);
			GridViewDataColumn column;
			column = new GridViewDataColumn();
			column.Width = fileColumn.Width;
			column.FieldName = string.IsNullOrEmpty(fileColumn.Name) ? DefaultCustomColumnFieldName : fileColumn.Name;
			column.UnboundType = UnboundColumnType.Object;
			column.Settings.AllowHeaderFilter = fileColumn.ShowHeaderFilterButton;
			column.Caption = string.IsNullOrEmpty(fileColumn.Caption) ? " " : fileColumn.Caption;
			PrepareFilesGridViewColumn(column, fileColumn);
			return column;
		}
		protected void PrepareFilesGridViewColumn(GridViewDataColumn column, FileManagerDetailsColumn fileColumn) {
			column.Settings.AllowDragDrop = fileColumn.AllowDragDrop;
			column.Settings.AllowSort = fileColumn.AllowSort;
			column.SortOrder = fileColumn.SortOrder;
			column.VisibleIndex = fileColumn.VisibleIndex;
			column.CellStyle.Assign(fileColumn.CellStyle);
			column.HeaderStyle.Assign(fileColumn.HeaderStyle);
			column.FixedStyle = fileColumn.FixedStyle;
		}
		protected FileManagerDetailsCustomColumn GetDetailsCustomColumn(int index) {
			return FileManager.SettingsFileList.DetailsViewSettings.Columns[index] as FileManagerDetailsCustomColumn;
		}
		protected void CreateUploadPanel() {
			this.uploadControl = CreateUploadControl();
			if(FileManager.SettingsUpload.ShowUploadPanel)
				CreateUploadPanelTable();
			else
				Controls.Add(UploadControl);
		}
		protected void CreateUploadPanelTable() {
			this.uploadPanelTable = RenderUtils.CreateTable(false);
			if(!DesignMode) {
				UploadPanelPane.Controls.Add(UploadPanelTable);
				UploadPanelPane.Visible = FileManager.Enabled && FileManager.SettingsUpload.Enabled;
			}
			else {
				UploadPanelCell.Controls.Add(UploadPanelTable);
				UploadPanelCell.Visible = FileManager.Enabled && FileManager.SettingsUpload.Enabled;
			}
			TableRow tableRow = new InternalTableRow();
			UploadPanelTable.Controls.Add(tableRow);
			TableCell uploadPanelTableCell = new InternalTableCell();
			tableRow.Controls.Add(uploadPanelTableCell);
			this.uploadPanelTableButtonCell = new InternalTableCell();
			tableRow.Controls.Add(UploadPanelTableButtonCell);
			uploadPanelTableCell.Controls.Add(UploadControl);
			this.uploadButton = CreateUploadButton();
			uploadPanelTableButtonCell.Controls.Add(UploadButton);
		}
		protected ASPxUploadControl CreateUploadControl() {
			var uploadControl = FileManager.CreateUploadControl(DesignMode ? (ASPxWebControl)FileManager : (ASPxWebControl)Splitter);
			uploadControl.ID = UploadControlID;
			uploadControl.FileUploadMode = UploadControlFileUploadMode.OnPageLoad;
			uploadControl.ShowProgressPanel = !DesignMode;
			uploadControl.ShowCancelButton = false;
			uploadControl.FileUploadComplete += UploadControl_FileUploadComplete;
			uploadControl.FilesUploadComplete += UploadControl_FilesUploadComplete;
			if(!FileManager.SettingsUpload.ShowUploadPanel) {
				uploadControl.ClientSideEvents.FilesUploadStart = UploadControlFileUploadStartEventHandler;
				uploadControl.ClientSideEvents.UploadingProgressChanged = UploadControlUploadingProgressChangedEventHandler;
				uploadControl.ShowUI = false;
			}
			uploadControl.ClientSideEvents.FilesUploadComplete = UploadControlFilesUploadCompleteEventHandler;
			uploadControl.ClientSideEvents.TextChanged = UploadTextChangedEventHandler;
			uploadControl.NullText = FileManager.SettingsUpload.NullText;
			uploadControl.DialogTriggerID = FileManager.SettingsUpload.DialogTriggerID;
			uploadControl.ValidationSettings.Assign(FileManager.SettingsUpload.ValidationSettings);
			uploadControl.UploadMode = FileManager.SettingsUpload.UseAdvancedUploadMode ? UploadControlUploadMode.Auto : UploadControlUploadMode.Standard;
			uploadControl.AdvancedModeSettings.Assign(FileManager.SettingsUpload.AdvancedModeSettings);
			uploadControl.AutoStartUpload = !FileManager.SettingsUpload.ShowUploadPanel || FileManager.SettingsUpload.AutoStartUpload;
			uploadControl.RightToLeft = FileManager.RightToLeft;
			uploadControl.Width = Unit.Pixel(FileManagerHelper.UploadControlWidth);
			uploadControl.ClientVisible = false; 
			uploadControl.Visible = FileManager.SettingsUpload.Enabled;
			return uploadControl;
		}
		protected InternalHyperLink CreateUploadButton() {
			var uploadButton = RenderUtils.CreateHyperLink(true, true);
			uploadButton.ID = UploadButtonID;
			uploadButton.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadButton);
			uploadButton.NavigateUrl = "javascript:void(0)";
			return uploadButton;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!DesignMode)
				FileManager.Helper.GetSplitterStyle().AssignToControl(Splitter);
			else
				FileManager.Helper.GetSplitterStyle().AssignToControl(Table);
			BreadCrumbsPane.PaneStyle.CopyFrom(FileManager.Helper.GetBreadCrumbsStyle());
			PrepareToolbar();
			PrepareUploadPanel();
			UploadControl.Height = FileManager.Helper.GetUploadControlHeight();
			FoldersPane.PaneStyle.Assign(FileManager.Helper.GetFolderContainerStyle());
			ItemsPane.PaneStyle.Assign(FileManager.Helper.GetFileContainerStyle());
			if(DesignMode) {
				if(FileManager.SettingsFolders.Visible) {
					Splitter.RenderHelper.GetSeparatorStyle(ContentPane2).AssignToControl(SeparatorCell, AttributesRange.All);
					Splitter.RenderHelper.GetPaneStyle(FoldersPane).AssignToControl(FoldersCell, AttributesRange.All);
					FoldersCell.Width = FileManager.Helper.GetFoldersContainerWidth();
					FoldersCell.VerticalAlign = VerticalAlign.Top;
				}
				if(FileManager.SettingsBreadcrumbs.Visible)
					FileManager.Helper.GetBreadCrumbsStyle().AssignToControl(BreadCrumbsCell);
				Splitter.RenderHelper.GetPaneStyle(ItemsPane).AssignToControl(FilesCell, AttributesRange.All);
				FilesCell.VerticalAlign = VerticalAlign.Top;
			}
		}
		protected void PrepareToolbar() {
			ToolbarPane.MinSize = FileManager.Styles.Toolbar.Height;
			ToolbarPane.PaneStyle.CopyFrom(FileManager.Helper.GetToolbarStyle());
			if(DesignMode) {
				ToolbarCell.Height = FileManager.Helper.GetToolbarHeight();
				Splitter.RenderHelper.GetPaneStyle(ToolbarPane).AssignToControl(ToolbarCell, AttributesRange.All);
			}
			FilterCell.CssClass = FileManagerStyles.ToolbarFilterCssClass;
			FileManager.Helper.GetToolbarFilterBoxStyle().AssignToControl(FilterCell);
			TbFilter.Font.CopyFrom(FileManager.Font);
		}
		protected void PrepareUploadPanel() {
			if(!FileManager.SettingsUpload.ShowUploadPanel) return;
			UploadPanelPane.MinSize = FileManager.Styles.UploadPanel.Height;
			UploadPanelPane.PaneStyle.CopyFrom(FileManager.Helper.GetUploadPanelStyle());
			if(DesignMode) {
				Splitter.RenderHelper.GetPaneStyle(UploadPanelPane).AssignToControl(UploadPanelCell, AttributesRange.All);
				UploadPanelCell.Height = FileManager.Helper.GetUploadPanelHeight();
			}
			UploadPanelTable.CssClass = FileManagerStyles.UploadPanelTableCssClass;
			UploadButton.CssClass = FileManagerStyles.UploadPanelDisableButton;
			UploadPanelTableButtonCell.CssClass = FileManagerStyles.UploadPanelTableButtonCellCssClass;
			FileManager.GetUploadPanelElementStyle().AssignToHyperLink(UploadButton);
			FileManager.GetUploadPanelElementStyle().AssignToControl(UploadControl);
		}
		internal bool ShowToolbar() {
			return FileManager.SettingsToolbar.Visible && !(
#pragma warning disable 618
				(!FileManager.SettingsToolbar.ShowCreateButton || !FileManager.SettingsEditing.AllowCreate) &&
				(!FileManager.SettingsToolbar.ShowRenameButton || !FileManager.SettingsEditing.AllowRename) &&
				(!FileManager.SettingsToolbar.ShowMoveButton || !FileManager.SettingsEditing.AllowMove) &&
				(!FileManager.SettingsToolbar.ShowDeleteButton || !FileManager.SettingsEditing.AllowDelete) &&
				!FileManager.SettingsToolbar.ShowFilterBox &&
				!FileManager.SettingsToolbar.ShowPath &&
				!FileManager.SettingsToolbar.ShowRefreshButton &&
				(!FileManager.SettingsToolbar.ShowDownloadButton || !FileManager.SettingsEditing.AllowDownload) &&
				(!FileManager.SettingsToolbar.ShowCopyButton || !FileManager.SettingsEditing.AllowCopy));
#pragma warning restore 618
		}
		void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			UploadCommand.UploadFile(e);
		}
		void UploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
			e.CallbackData = UploadCommand.GetCallbackResult(UploadControl);
			UploadCommand = null;
		}
		internal string SaveClientState() {
			return Splitter != null ? Splitter.SaveClientState() : string.Empty;
		}
		internal void LoadClientState(string state) {
			if(Splitter != null)
				Splitter.LoadClientState(state);
		}
		void CheckCustomColumnName(string name) {
			foreach(PropertyInfo p in FileManager.Helper.Data.ItemProperties) {
				if(name == p.Name)
					throw new Exception("Exception text: A custom column's name cannot duplicate the file manager item's property names. To learn more, see http://help.devexpress.com/#AspNet/CustomDocument17537.");
			}
		}
	}
	public class FileManagerMenuHelper {
		const string StandartItemPrefix = "fmtsi-";
		Regex hotkeyTextRegExp = new Regex("\\s?\\(\\w*\\)");
		public FileManagerMenuHelper(ASPxFileManager fileManager, ASPxMenuBase menu, bool isContextMenu) {
			FileManager = fileManager;
			Menu = menu;
			IsContextMenu = isContextMenu;
			var customItems = IsContextMenu ? FileManager.SettingsContextMenu.Items : FileManager.SettingsToolbar.Items;
			IsDefaultItems = customItems.Count == 0;
			if(IsDefaultItems)
				CreateDefaultButtons();
			else
				CreateMenuItems(customItems, Items);
		}
		public bool IsDefaultItems { get; private set; }
		public bool IsContextMenu { get; private set; }
		public ASPxFileManager FileManager { get; private set; }
		public ASPxMenuBase Menu { get; private set; }
		public MenuItemCollection Items { get { return Menu.Items; } }
		public FileManagerSettingsToolbar SettingsToolbar { get { return FileManager.SettingsToolbar; } }
		void CreateDefaultButtons() {
			CreateMenuItem(new FileManagerToolbarCreateButton(), Items);
			CreateMenuItem(new FileManagerToolbarRenameButton(), Items);
			CreateMenuItem(new FileManagerToolbarMoveButton(), Items);
			CreateMenuItem(new FileManagerToolbarCopyButton(), Items);
			CreateMenuItem(new FileManagerToolbarDeleteButton(), Items);
			CreateMenuItem(new FileManagerToolbarRefreshButton(), Items);
			CreateMenuItem(new FileManagerToolbarDownloadButton(), Items);
			CreateMenuItem(new FileManagerToolbarUploadButton(), Items);
		}
		void CreateMenuItems(FileManagerToolbarItemCollection items, MenuItemCollection itemCollection) {
			foreach(FileManagerToolbarItemBase toolbarItem in items) {
				var menuItem = CreateMenuItem(toolbarItem, itemCollection);
				if(toolbarItem is FileManagerToolbarCustomDropDownButton)
					CreateMenuItems(((FileManagerToolbarCustomDropDownButton)toolbarItem).Items, menuItem.Items);
			}
		}
		MenuItem CreateMenuItem(FileManagerToolbarItemBase toolbarItem, MenuItemCollection itemCollection) {
			var customToolbarButton = toolbarItem as FileManagerToolbarCustomButton;
			bool isCustomButton = customToolbarButton != null;
			var item = new MenuItem();
			itemCollection.Add(item);
			item.BeginGroup = toolbarItem.BeginGroup;
			item.ToolTip = toolbarItem.ToolTip;
			item.Text = GetText(toolbarItem);
			item.Name = (isCustomButton ? string.Empty : StandartItemPrefix) + toolbarItem.CommandName;
			item.ClientEnabled = toolbarItem.GetClientEnabled();
			item.Image.Assign(GetImageProperties(toolbarItem));
			item.ItemStyle.CopyFrom(IsContextMenu ? FileManager.Helper.GetContextMenuItemStyle() : FileManager.Helper.GetToolbarItemStyle());
			if(toolbarItem is FileManagerToolbarUploadButton && FileManager.SettingsUpload.Enabled)
				FileManager.Control.Container.UploadControl.DialogTriggerID += ";" + GetItemClientID(item);
			if(isCustomButton) {
				item.Checked = customToolbarButton.Checked;
				item.GroupName = customToolbarButton.GroupName;
				item.ItemStyle.CopyFrom(customToolbarButton.ItemStyle);
				item.Enabled = customToolbarButton.Enabled;
				item.Visible = customToolbarButton.Visible;
				item.ClientVisible = customToolbarButton.ClientVisible;
				var dropDownButton = toolbarItem as FileManagerToolbarCustomDropDownButton;
				if(dropDownButton != null) {
					item.DropDownMode = dropDownButton.DropDownMode;
					item.PopOutImage.CopyFrom(dropDownButton.PopOutImage);
					item.SubMenuStyle.CopyFrom(dropDownButton.SubMenuStyle);
				}
			} else
				PrepareToolbarStandardItem(item);
			return item;
		}
		void PrepareToolbarStandardItem(MenuItem item) {
			switch(item.Name.Substring(StandartItemPrefix.Length)) {
#pragma warning disable 618
				case "Create":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowCreateButton : FileManager.IsItemCreatingAvailable();
					break;
				case "Rename":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowRenameButton : FileManager.IsItemRenamingAvailable();
					break;
				case "Move":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowMoveButton : FileManager.IsItemMovingAvailable();
					break;
				case "Copy":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowCopyButton : FileManager.IsItemCopyAvailable();
					break;
				case "Delete":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowDeleteButton : FileManager.IsItemDeletingAvailable();
					break;
				case "Refresh":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowRefreshButton : FileManager.IsRefreshAvailable();
					break;
				case "Download":
					item.Visible = !IsDefaultItems ? SettingsToolbar.ShowDownloadButton : FileManager.IsItemDownloadAvailable();
					break;
#pragma warning restore 618
				case "Upload":
					item.Visible = !IsDefaultItems || (FileManager.SettingsUpload.Enabled && !FileManager.SettingsUpload.ShowUploadPanel);
					break;
			}
		}
		string GetText(FileManagerToolbarItemBase item) {
			if(!IsContextMenu || item is FileManagerToolbarCustomButton)
				return item.Text;
			return string.IsNullOrEmpty(item.Text) ? hotkeyTextRegExp.Replace(item.ToolTip, string.Empty) : item.Text;
		}
		string GetItemClientID(MenuItem item) {
			return FileManager.ClientID + "_" + (IsContextMenu ? string.Empty : FileManagerControl.SplitterID + "_") + Menu.ClientID + "_" + Menu.GetItemElementID(item);
		}
		ImagePropertiesBase GetImageProperties(FileManagerToolbarItemBase toolbarItem) {
			string resourceName = toolbarItem.GetResourceImageName();
			if(string.IsNullOrEmpty(resourceName))
				return toolbarItem.Image;
			else {
				ImagePropertiesBase image = FileManager.Helper.GetImage(resourceName);
				image.CopyFrom(toolbarItem.Image);
				return image;
			}
		}
	}
	[ToolboxItem(false)]
	public class FileManagerToolbar : ASPxMenu {
		const string PathControlID = "Path";
		FileManagerContainer owner;
		public FileManagerToolbar(FileManagerContainer owner, string id)
			: base(owner.FileManager) {
			this.owner = owner;
			ID = id;
			ClientIDHelper.EnableClientIDGeneration(this);
			Initialize();
			EnableClientSideAPI = true;
			ApplyItemStyleToTemplates = true;
		}
		public FileManagerContainer Owner { get { return owner; } }
		public ASPxFileManager FileManager { get { return Owner.FileManager; } }
		public FileManagerMenuHelper MenuHelper { get; private set; }
		public DevExpress.Web.MenuItem PathItem { get { return Items.FindByName("Path"); } }
		protected void Initialize() {
			Orientation = System.Web.UI.WebControls.Orientation.Horizontal;
			ShowAsToolbar = true;
			Items.Add("", "Path");
			PathItem.Template = new ToolbarPathItem(this, PathControlID);
			MenuHelper = new FileManagerMenuHelper(FileManager, this, false);
		}
		protected override void CreateControlHierarchy() {
			PathItem.Visible = FileManager.SettingsToolbar.ShowPath;	   
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			FileManager.Helper.GetToolbarMenuStyle().AssignToControl(this);
			PathItem.ItemStyle.CssClass = FileManagerStyles.ToolbarPathCssClass;
			PathItem.ItemStyle.CopyFrom(FileManager.Helper.GetToolbarPathBoxStyle());
			base.PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class FileManagerContextMenu : ASPxPopupMenu {
		FileManagerContainer owner;
		public FileManagerContextMenu(FileManagerContainer owner, string id)
			: base(owner.FileManager) {
			this.owner = owner;
			ID = id;
			ClientIDHelper.EnableClientIDGeneration(this);
			Initialize();
		}
		public FileManagerContainer Owner { get { return owner; } }
		public ASPxFileManager FileManager { get { return Owner.FileManager; } }
		protected FileManagerMenuHelper MenuHelper { get; private set; }
		public DevExpress.Web.MenuItem PathItem { get { return Items.FindByName("Path"); } }
		protected void Initialize() {
			MenuHelper = new FileManagerMenuHelper(FileManager, this, true);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			Styles.CopyFrom(FileManager.StylesContextMenu);
			base.PrepareControlHierarchy();
		}
	}
	[ToolboxItem(false)]
	public class FileManagerGridView : ASPxGridView {
		public const string CustomKeyboardHelper = "FileManagerGridKbdHelper";
		ASPxFileManager fileManager;
		Dictionary<string, bool> customColumnsEncodeHtmlPropertiesCache;
		public FileManagerGridView(ASPxFileManager fileManager)
			: base() {
			this.fileManager = fileManager;
			ClientIDHelper.EnableClientIDGeneration(this);
			Initialize();
		}
		public ASPxFileManager FileManager { get { return fileManager; } }
		protected internal override bool IsRenderPostponeScriptAfterMainTable {
			get {
				if(FileManager.CurrentCommand != null && FileManager.CurrentCommand.CommandId == FileManagerCommandId.CustomCallback)
					return true;
				if(FileManager.IsCallback || IsCallback)
					return ClientObjectState == null;
				return true;
			}
		}
		internal Dictionary<string, bool> CustomColumnsEncodeHtmlPropertiesCache {
			get {
				if(customColumnsEncodeHtmlPropertiesCache == null)
					customColumnsEncodeHtmlPropertiesCache = new Dictionary<string, bool>();
				return customColumnsEncodeHtmlPropertiesCache;
			}
		}
		public void UpdatePageIndex(int pageIndex) {
			if(PageIndex != pageIndex)
				PageIndex = pageIndex;
		}
		public IEnumerable<FileManagerItem> GetAllVisibleItems() {
			return GetVisibleItems(0, VisibleRowCount);
		}
		public IEnumerable<FileManagerItem> GetCurrentPageVisibleItems() {
			int pageSize = FileManager.SettingsFileList.PageSize;
			if(VisibleStartIndex + pageSize >= VisibleRowCount)
				pageSize = VisibleRowCount - VisibleStartIndex;
			FileManager.Helper.Data.VirtScrollItemIndex = VisibleStartIndex;
			FileManager.Helper.Data.VirtScrollPageItemsCount = pageSize;
			FileManager.Helper.Data.ItemsCount = VisibleRowCount;
			return GetVisibleItems(VisibleStartIndex, pageSize);
		}
		IEnumerable<FileManagerItem> GetVisibleItems(int startIndex, int count) {
			int endIndex = startIndex + count;
			for(int i = startIndex; i < endIndex; i++)
				yield return GetRow(i) as FileManagerItem;
		}
		protected void Initialize() {
			ParentSkinOwner = FileManager;
			RenderHelper.CustomKbdHelperName = CustomKeyboardHelper;
			if(!FileManager.IsVirtualScrollingEnabled())
				SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
			SettingsBehavior.AllowSelectByRowClick = true;
			SettingsBehavior.AllowSelectSingleRowOnly = !FileManager.Settings.EnableMultiSelect;
			SettingsBehavior.EnableRowHotTrack = true;
			ClientSideEvents.RowDblClick = "function(){}";
			SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
			KeyFieldName = "Id";
			KeyboardSupport = true;
			Settings.ShowHeaderFilterButton = FileManager.SettingsFileList.DetailsViewSettings.ShowHeaderFilterButton;
			SettingsBehavior.AllowSort = FileManager.SettingsFileList.DetailsViewSettings.AllowColumnSort;
			SettingsBehavior.AllowDragDrop = FileManager.SettingsFileList.DetailsViewSettings.AllowColumnDragDrop;
			if(FileManager.SettingsFileList.DetailsViewSettings.AllowColumnResize)
				SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
			ImagesEditors.CheckBoxChecked.CopyFrom(FileManager.Images.DetailsCheckBoxChecked);
			ImagesEditors.CheckBoxUnchecked.CopyFrom(FileManager.Images.DetailsCheckBoxUnchecked);
			Images.CopyFrom(FileManager.ImagesDetailsView);
			EnableViewState = false;
		}
		protected internal void PrepareControlHierarchyCore() {
			Width = Unit.Percentage(100);
			Styles.Row.CopyFrom(FileManager.Helper.GetFileStyle());
			if(FileManager.Settings.EnableMultiSelect) {
				Styles.FocusedRow.CopyFrom(FileManager.Helper.GetFileFocusStyle());
				Styles.SelectedRow.CopyFrom(FileManager.Helper.GetFileSelectionActiveStyle());
			}
			else
				Styles.FocusedRow.CopyFrom(FileManager.Helper.GetFileSelectionActiveStyle());
			Styles.RowHotTrack.CopyFrom(FileManager.Helper.GetFileHoverStyle());
			Styles.CopyFrom(FileManager.StylesDetailsView);
			Styles.Header.CssClass += " " + FileManagerDetailsViewStyles.GridViewHeaderPostfix;
			Border.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
		}
		protected override void PrepareControlHierarchy() {
			PrepareControlHierarchyCore();
			base.PrepareControlHierarchy();
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.FileManagerGridView";
		}
		protected internal void RaiseCallbackEventCore(string callbackArgs) {
			RaiseCallbackEvent(callbackArgs);
		}
		protected internal object GetCallbackResultCore() {
			PrepareControlHierarchyCore();
			return GetCallbackResult();
		}
		protected override GridRenderHelper CreateRenderHelper() {
			return new FilesGridViewRenderHelper(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsCallback {
			get { return base.IsCallback || (FileManager.IsCallback && FileManager.IsVirtualScrollingEnabled()); }
		}
	}
	[ToolboxItem(false)]
	public class FileManagerThumbnailCheckBoxOwner : ASPxWebControl, IInternalCheckBoxOwner {
		const string CheckBoxIDWithPlaceHolder = "{{itemId}}_CHK";
		ASPxFileManager owner;
		public FileManagerThumbnailCheckBoxOwner(ASPxFileManager owner)
			: base() {
			this.owner = owner;
		}
		public ASPxFileManager FileManager { get { return owner; } }
		public bool ClientEnabled {
			get { return true; }
		}
		public CheckState CheckState {
			get { return CheckState.Unchecked; }
		}
		public string GetCheckBoxInputID() {
			return CheckBoxIDWithPlaceHolder;
		}
		public bool IsInputElementRequired {
			get { return true; }
		}
		public AppearanceStyleBase InternalCheckBoxStyle {
			get {
				var style = FileManager.Styles.CreateStyleByName(string.Empty, InternalCheckboxControl.CheckBoxClassName);
				return style;
			}
		}
		public InternalCheckBoxImageProperties GetCurrentCheckableImage() {
			return FileManager.GetCheckImage(((IInternalCheckBoxOwner)this).CheckState);
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return null; } }
	}
   [ToolboxItem(false)]
	public class FileManagerItems : ASPxWebControl {
		const string ItemID = "I";
		const string ItemTemplateContainerID = "ITC";
		FileManagerContainer owner;
		public FileManagerItems(FileManagerContainer owner)
			: base() {
			this.owner = owner;
		}
		public FileManagerContainer Owner { get { return owner; } }
		public ASPxFileManager FileManager { get { return Owner.FileManager; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(DesignMode)
				CreateDesignModeItems();
			else {
				if(FileManager.IsThumbnailsViewFileAreaItemTemplate) {
					CreateItemTemplates(FileManager.Helper.Data.GetMinimizedItemsList(true));
				}
			}
		}
		internal void CreateItemTemplates(IEnumerable<FileManagerItem> items) {
			int i = 0;
			foreach(FileManagerItem item in items) {
				if(item.ClientInvisible)
					continue;
				var itemContainer = RenderUtils.CreateDiv();
				if(DesignMode)
					GetDesignViewItemStyle(item is FileManagerFile ? FileManagerItemType.File : FileManagerItemType.Folder).AssignToControl(itemContainer);
				int itemIndex = FileManager.IsVirtualScrollingEnabled() ? i + FileManager.Helper.Data.VirtScrollItemIndex : i;
				itemContainer.ID = GetItemID(itemIndex);
				Controls.Add(itemContainer);
				var itemContentContainer = RenderUtils.CreateDiv();
				itemContainer.Controls.Add(itemContentContainer);
				FileManagerThumbnailsViewItemTemplateContainer container = new FileManagerThumbnailsViewItemTemplateContainer(item, i);
				container.AddToHierarchy(itemContentContainer, ItemTemplateContainerID + i.ToString());
				FileManager.SettingsFileList.ThumbnailsViewSettings.ItemTemplate.InstantiateIn(container);
				i++;
			}
		}
		protected void CreateDesignModeItems() {
			if(FileManager.IsThumbnailsViewFileAreaItemTemplate) {
				FileManagerDesignProvider provider = new FileManagerDesignProvider("", FileManager);
				CreateItemTemplates(provider.GetItems(FileManager.SettingsFileList.ShowFolders, FileManager.SettingsFileList.ShowParentFolder));
				return;
			}
			if(FileManager.SettingsFileList.ShowParentFolder)
				CreateDesignViewItem(FileManagerItemType.ParentFolder, 0);
			if(FileManager.SettingsFileList.ShowFolders) {
				for(int i = 1; i < 4; i++)
					CreateDesignViewItem(FileManagerItemType.Folder, i);
			}
			for(int i = 0; i < 4; i++)
				CreateDesignViewItem(FileManagerItemType.File, i);
		}
		void CreateDesignViewItem(FileManagerItemType type, int index) {
			WebControl file = RenderUtils.CreateDiv();
			GetDesignViewItemStyle(type).AssignToControl(file);
			Controls.Add(file);
			WebControl content = RenderUtils.CreateDiv();
			FileManager.Helper.GetFileContentStyle().AssignToControl(content);
			file.Controls.Add(content);
			Image img = RenderUtils.CreateImage();
			GetDesignViewItemImage(type).AssignToControl(img, DesignMode);
			content.Controls.Add(img);
			content.Controls.Add(RenderUtils.CreateBr());
			content.Controls.Add(new LiteralControl(GetDesignViewItemText(type, index)));
		}
		string GetDesignViewItemText(FileManagerItemType type, int index) {
			if(type == FileManagerItemType.File)
				return "File_" + index.ToString() + ".ext";
			if(type == FileManagerItemType.Folder)
				return "Folder " + index.ToString();
			return "..";
		}
		string GetItemID(int index) {
			return ItemID + index;
		}
		FileManagerFileStyle GetDesignViewItemStyle(FileManagerItemType type) {
			if(type == FileManagerItemType.File)
				return FileManager.Helper.GetFileStyle();
			return FileManager.Helper.GetFileAreaFolderStyle();
		}
		ImagePropertiesBase GetDesignViewItemImage(FileManagerItemType type) {
			if(type == FileManagerItemType.File)
				return FileManager.Helper.GetNoThumbnailImage();
			if(type == FileManagerItemType.Folder)
				return FileManager.Helper.GetFolderImage();
			return FileManager.Helper.GetFolderUpImage();
		}
		protected override object GetCallbackResult() {
			string result = string.Empty;
			foreach(WebControl control in Controls)
				result += RenderUtils.GetRenderResult(control);				
			return result;
		}
		internal string GetCallbackResultCore() {
			return GetCallbackResult().ToString();
		}
		protected override bool HasRootTag() {
			return FileManager.HasThumbnailsViewFileAreaItemTemplate;
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
	}
	[ToolboxItem(false)]
	public class FileManagerUploadControl : ASPxUploadControl {
		public ASPxFileManager FileManager { get; private set; }
		public FileManagerUploadControl(ASPxWebControl owner, ASPxFileManager fileManager) : base(owner) {
			FileManager = fileManager;
			ClientIDHelper.EnableClientIDGeneration(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.FileManagerUploadControl";
		}
		protected override Hashtable CreateValidationSettingsClientHashtable() {
			Hashtable result = base.CreateValidationSettingsClientHashtable();
			if(!result.ContainsKey("notAllowedFileExtensionErrorText") && FileManager.IsClientUploadAccessRulesValidationEnabled())
				result["notAllowedFileExtensionErrorText"] = ValidationSettings.NotAllowedFileExtensionErrorText;
			return result;
		}
	}
	[ToolboxItem(false)]
	public class ToolbarPathItem : ASPxWebControl, ITemplate {
		const string InputID = "I";
		TextBox tbPath;
		FileManagerToolbar toolbar;
		public ToolbarPathItem(ASPxWebControl owner, string id)
			:base(owner) {
			this.toolbar = (FileManagerToolbar)owner;
			ID = id;
			ClientIDHelper.EnableClientIDGeneration(this);
		}
		protected TextBox TbPath { get { return tbPath; } }
		protected FileManagerToolbar Toolbar { get { return toolbar; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Label label = new Label();
			label.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Path) + " ";
			label.AssociatedControlID = InputID;
			Controls.Add(label);
			this.tbPath = new InternalTextBox();
			TbPath.ID = InputID;
			TbPath.ReadOnly = true;
			TbPath.Width = Toolbar.FileManager.Helper.GetToolbarPathTextBoxWidth();
			Controls.Add(TbPath);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			string pathText = string.Empty;
			if(DesignMode)
				pathText = "Folder path will be displayed here";
			else if(Toolbar.FileManager.Settings.UseAppRelativePath)
				pathText = Toolbar.FileManager.FileSystemProvider.GetRelativeFolderPath(Toolbar.FileManager.SelectedFolder, Toolbar.FileManager);
			TbPath.Text = pathText;
			TbPath.Font.CopyFrom(Toolbar.FileManager.Font);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.tbPath = null;
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class FileManagerFolderBrowserPopup : DevExpress.Web.ASPxPopupControl {
		const string FoldersID = "FolderBrowserFolders";
		const string FoldersContainerID = "FC";
		const string OkButtonID = "OkB";
		const string CancelButtonID = "CaB";
		FileManagerFolders folderBrowserFolders;
		ASPxFileManager fileManager;
		WebControl foldersContainer;
		public FileManagerFolderBrowserPopup(ASPxFileManager owner)
			: base(owner) {
			AllowDragging = true;
			EnableClientSideAPI = true;
			PopupAnimationType = AnimationType.Fade;
			Modal = true;
			CloseAction = CloseAction.CloseButton;
			PopupElementID = owner.ClientID;
			PopupAction = PopupAction.None;
			PopupHorizontalAlign = PopupHorizontalAlign.Center;
			PopupVerticalAlign = PopupVerticalAlign.Middle;
			this.fileManager = owner;
			HeaderText = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_FolderBrowserPopupHeader);
			Width = Unit.Pixel(400);
			InitializeControls();
		}
		public FileManagerFolders FolderBrowserFolders { get { return folderBrowserFolders; } }
		public ASPxFileManager FileManager { get { return fileManager; } }
		public WebControl FoldersContainer { get { return foldersContainer; } }
		void InitializeControls() {
			this.foldersContainer = RenderUtils.CreateDiv();
			Controls.Add(FoldersContainer);
			FoldersContainer.ID = FoldersContainerID;
			FoldersContainer.CssClass = FileManagerStyles.FolderBrowserPopupFoldersContainer;
			FoldersContainer.Height = DefaultWindow.Height;
			this.folderBrowserFolders = new FileManagerFolders(FileManager, true);
			FolderBrowserFolders.ID = FoldersID;
			FolderBrowserFolders.Visible = false;
			FolderBrowserFolders.Images.NodeLoadingPanel.Assign(FileManager.Helper.GetImage(FileManagerImages.FolderNodeLoadingPanelImageName));
			FoldersContainer.Controls.Add(FolderBrowserFolders);
			WebControl buttonsContainer = RenderUtils.CreateDiv();
			Controls.Add(buttonsContainer);
			buttonsContainer.CssClass = FileManagerStyles.FolderBrowserPopupButtonContainer;
			HyperLink okButton = RenderUtils.CreateHyperLink(true, true);
			okButton.ID = OkButtonID;
			okButton.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Ok);
			okButton.NavigateUrl = "javascript:void(0)";
			buttonsContainer.Controls.Add(okButton);
			buttonsContainer.Controls.Add(new LiteralControl(" "));
			HyperLink cancelButton = RenderUtils.CreateHyperLink(true, true);
			cancelButton.ID = CancelButtonID;
			cancelButton.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_Cancel);
			cancelButton.NavigateUrl = "javascript:void(0)";
			buttonsContainer.Controls.Add(cancelButton);
		}
		public override Unit Height {
			get { return base.Height; }
			set {
				base.Height = value;
				FoldersContainer.Height = base.Height;
			}
		}
		protected override bool HideBodyScrollWhenModal() {
			return !Browser.Family.IsNetscape;
		}
		protected override bool LoadWindowsState(string state) {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class FileManagerItemsAreaPopup : DevExpress.Web.ASPxPopupControl {
		public FileManagerItemsAreaPopup(ASPxFileManager owner)
			: base(owner) {
			AllowDragging = false;
			EnableClientSideAPI = true;
			PopupAnimationType = AnimationType.Fade;
			CloseAction = CloseAction.None;
			PopupAction = PopupAction.None;
			PopupElementID = owner.ClientID;
			PopupHorizontalAlign = PopupHorizontalAlign.RightSides;
			PopupVerticalAlign = PopupVerticalAlign.BottomSides;
			ShowHeader = false;
			ShowFooter = false;
			ShowShadow = false;
			EnableTheming = false;
		}
	}
	[ToolboxItem(false)]
	public class FileManagerUploadProgressPopup : FileManagerItemsAreaPopup {
		const string ProgressBarID = "PB";
		const string CancelButtonID = "CB";
		const int popupHeight = 66;
		const int popupWidth = 170;
		const int progressBarHeight = 5;
		const int progressBarWidth = 130;
		const int HorizontalOffset = -12;
		const int VerticalOffset = -11;
		ASPxProgressBar progressBar;
		WebControl cancelButton;
		public FileManagerUploadProgressPopup(ASPxFileManager owner)
			: base(owner) {
			PopupHorizontalOffset = HorizontalOffset;
			PopupVerticalOffset = VerticalOffset;
			Width = Unit.Pixel(popupWidth);
			Height = Unit.Pixel(popupHeight);
			CssClass = FileManagerStyles.UploadProgressPopupCssClass;
			InitializeControls();
		}
		public ASPxProgressBar ProgressBar { get { return progressBar; } }
		public WebControl CancelButton { get { return cancelButton; } }
		void InitializeControls() {
			var uploadingTextContainer = RenderUtils.CreateLabel();
			Controls.Add(uploadingTextContainer);
			uploadingTextContainer.Text = ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_UploadProgressPopupText);
			progressBar = new ASPxProgressBar();
			Controls.Add(ProgressBar);
			ProgressBar.ID = ProgressBarID;
			ProgressBar.ShowPosition = false;
			ProgressBar.EnableTheming = false;
			ProgressBar.Height = Unit.Pixel(progressBarHeight);
			ProgressBar.Width = Unit.Pixel(progressBarWidth);
			this.cancelButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.A);
			CancelButton.Controls.Add(RenderUtils.CreateLiteralControl(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_CancelButton)));
			CancelButton.ID = CancelButtonID;
			Controls.Add(CancelButton);
		}
	}
	[ToolboxItem(false)]
	public class CreateFolderNode : ASPxWebControl, ITemplate {
		TextBox tbNode;
		public CreateFolderNode(ASPxWebControl ownerControl)
			: base(ownerControl) {
				ClientIDHelper.EnableClientIDGeneration(this);
		}
		public TextBox TbNode { get { return tbNode; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.tbNode = new TextBox();
			Controls.Add(TbNode);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.tbNode = null;
		}
		protected override void PrepareControlHierarchy() {
			TbNode.CssClass = FileManagerStyles.CreateInputCssClass;
			TbNode.Font.CopyFrom(OwnerControl.Font);
		}
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
	}
	[ToolboxItem(false)]
	public class NameCellTemplate : ITemplate {
		public void InstantiateIn(Control container) {
			var gridContainer = (GridViewDataItemTemplateContainer)container;
			WebControl wrapper = RenderUtils.CreateDiv();
			wrapper.CssClass = FileManagerStyles.FileColumnTitleCssClass;
			container.Controls.Add(wrapper);
			var nameValue = gridContainer.Grid.GetRowValues(gridContainer.VisibleIndex, gridContainer.Column.FieldName);
			var textControl = new LiteralControl();
			textControl.Text = nameValue != null ? nameValue.ToString() : string.Empty;
			wrapper.Controls.Add(textControl);
		}
	}
	[PredefinedFileSystemProvider]
	public class FileManagerDesignProvider : FileSystemProviderBase {
		internal const string RootFolderName = "Root Folder";
		ASPxFileManager fileManager;
		public FileManagerDesignProvider(string rootFolder, ASPxFileManager fileManager)
			: base(rootFolder) {
				this.fileManager = fileManager;
		}
		public List<FileManagerItem> GetItems(bool includeFolders, bool includeParentFolder) {
			var items = new List<FileManagerItem>();
			if(includeParentFolder)
				items.Add(GetParentFolder());
			if(includeFolders)
				items.AddRange(GetFolders());
			items.AddRange(GetFiles());
			return items;
		}
		FileManagerItem GetParentFolder() {
			var folder = new FileManagerFolder(this, RootFolderName);
			folder.IsParentFolderItem = true;
			folder.ThumbnailUrl = fileManager.Helper.GetFolderUpImage().Url;
			return folder;
		}
		IEnumerable<FileManagerItem> GetFolders() {
			for(int i = 1; i < 4; i++) {
				var folder = new FileManagerFolder(this, "Folder " + i.ToString());
				folder.ThumbnailUrl = fileManager.Helper.GetFolderImage().Url;
				yield return folder;
			}
		}
		IEnumerable<FileManagerItem> GetFiles() {
			for(int i = 0; i < 4; i++) {
				var file = new FileManagerFile(this, "File_" + i.ToString() + ".ext");
				file.ThumbnailUrl = fileManager.Helper.GetNoThumbnailImage().Url;
				yield return file;
			}
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			return DateTime.Now;
		}
		public override DateTime GetLastWriteTime(FileManagerFolder folder) {
			return DateTime.Now;
		}
		public override long GetLength(FileManagerFile file) {
			return 0;
		}
	}
}
