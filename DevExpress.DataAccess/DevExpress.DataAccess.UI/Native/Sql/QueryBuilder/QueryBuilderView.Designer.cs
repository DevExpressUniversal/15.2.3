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

using DevExpress.DataAccess.Native.Sql.QueryBuilder;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	partial class QueryBuilderView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			this.gridViewColumns = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colIsKey = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colType1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colNullable = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridControlColumns = new DevExpress.XtraGrid.GridControl();
			this.childrenBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.availableDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.barListItem1 = new DevExpress.XtraBars.BarListItem();
			this.barButtonItemRename = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItemDelete = new DevExpress.XtraBars.BarButtonItem();
			this.gridControlAvailable = new DevExpress.XtraGrid.GridControl();
			this.gridViewTables = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColType = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemImageComboBoxNodeType = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
			this.gridColName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.reSqlPanel = new DevExpress.XtraEditors.PanelControl();
			this.tlSelection = new DevExpress.XtraTreeList.TreeList();
			this.colSelected = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemCheckEditSelection = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.colName1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colCondition = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemButtonEditConditionDefault = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.colAggregated = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colSortedAsc = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colSortedDesc = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colGroupedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.colForeignKey = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.selectionDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.repositoryItemButtonEditEditJoin = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.repositoryItemButtonEditAddJoin = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.repositoryItemButtonEditJoined = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.chbAllowEditSql = new DevExpress.XtraEditors.CheckEdit();
			this.btnFilter = new DevExpress.XtraEditors.SimpleButton();
			this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.gridControlQuery = new DevExpress.XtraGrid.GridControl();
			this.queryGridDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.gridViewQuery = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colTable = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colAlias = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colOutput = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemCheckEditQuery = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.colSortingType = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemLookUpEditSortingType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
			this.colSortOrder = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemSpinEditSortOrder = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
			this.colGroupBy = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colAggregate = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemComboBoxAggregateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repositoryItemLookUpEditNewItemColumn = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.splitterItem3 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutItemGridPane = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemTablesPane = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroupColumns = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGridColumns = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemPreviewButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemFilterButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceBottom = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutItemCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemOkButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItemAvailableAndColumns = new DevExpress.XtraLayout.SplitterItem();
			this.layoutItemAllowEdit = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceTop = new DevExpress.XtraLayout.EmptySpaceItem();
			this.splitterItem4 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutItemSelectionPane = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemReSqlPanel = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.colType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
			this.layoutGroupTop = new DevExpress.XtraLayout.LayoutControlGroup();
			((System.ComponentModel.ISupportInitialize)(this.gridViewColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.childrenBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.availableDataBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlAvailable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewTables)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBoxNodeType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.reSqlPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tlSelection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditSelection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditConditionDefault)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.selectionDataBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditEditJoin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditAddJoin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditJoined)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllowEditSql.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlQuery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.queryGridDataBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewQuery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditQuery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEditSortingType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditSortOrder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBoxAggregateEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEditNewItemColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridPane)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTablesPane)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilterButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancelButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOkButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItemAvailableAndColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectionPane)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemReSqlPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupTop)).BeginInit();
			this.SuspendLayout();
			this.gridViewColumns.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridViewColumns.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colIsKey,
			this.colName,
			this.colType1,
			this.colNullable});
			this.gridViewColumns.GridControl = this.gridControlColumns;
			this.gridViewColumns.Name = "gridViewColumns";
			this.gridViewColumns.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsBehavior.Editable = false;
			this.gridViewColumns.OptionsBehavior.ReadOnly = true;
			this.gridViewColumns.OptionsCustomization.AllowColumnMoving = false;
			this.gridViewColumns.OptionsCustomization.AllowColumnResizing = false;
			this.gridViewColumns.OptionsCustomization.AllowFilter = false;
			this.gridViewColumns.OptionsCustomization.AllowGroup = false;
			this.gridViewColumns.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridViewColumns.OptionsCustomization.AllowSort = false;
			this.gridViewColumns.OptionsDetail.AllowZoomDetail = false;
			this.gridViewColumns.OptionsDetail.ShowDetailTabs = false;
			this.gridViewColumns.OptionsDetail.SmartDetailExpand = false;
			this.gridViewColumns.OptionsFilter.AllowColumnMRUFilterList = false;
			this.gridViewColumns.OptionsFilter.AllowFilterEditor = false;
			this.gridViewColumns.OptionsFind.AllowFindPanel = false;
			this.gridViewColumns.OptionsHint.ShowColumnHeaderHints = false;
			this.gridViewColumns.OptionsHint.ShowFooterHints = false;
			this.gridViewColumns.OptionsLayout.StoreDataSettings = false;
			this.gridViewColumns.OptionsLayout.StoreVisualOptions = false;
			this.gridViewColumns.OptionsMenu.EnableColumnMenu = false;
			this.gridViewColumns.OptionsMenu.EnableFooterMenu = false;
			this.gridViewColumns.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridViewColumns.OptionsMenu.ShowAddNewSummaryItem = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsMenu.ShowAutoFilterRowItem = false;
			this.gridViewColumns.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
			this.gridViewColumns.OptionsMenu.ShowGroupSortSummaryItems = false;
			this.gridViewColumns.OptionsMenu.ShowSplitItem = false;
			this.gridViewColumns.OptionsNavigation.UseTabKey = false;
			this.gridViewColumns.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridViewColumns.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridViewColumns.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
			this.gridViewColumns.OptionsView.ShowColumnHeaders = false;
			this.gridViewColumns.OptionsView.ShowDetailButtons = false;
			this.gridViewColumns.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridViewColumns.OptionsView.ShowGroupExpandCollapseButtons = false;
			this.gridViewColumns.OptionsView.ShowGroupPanel = false;
			this.gridViewColumns.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.OptionsView.ShowIndicator = false;
			this.gridViewColumns.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewColumns.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
			this.gridViewColumns.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewColumns_RowStyle);
			this.colIsKey.FieldName = "IsKey";
			this.colIsKey.Name = "colIsKey";
			this.colIsKey.OptionsColumn.ReadOnly = true;
			this.colName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.colName.AppearanceCell.Options.UseFont = true;
			this.colName.FieldName = "Name";
			this.colName.Name = "colName";
			this.colName.OptionsColumn.ReadOnly = true;
			this.colName.Visible = true;
			this.colName.VisibleIndex = 0;
			this.colType1.AppearanceCell.Options.UseTextOptions = true;
			this.colType1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.colType1.FieldName = "Type";
			this.colType1.Name = "colType1";
			this.colType1.OptionsColumn.ReadOnly = true;
			this.colType1.Visible = true;
			this.colType1.VisibleIndex = 1;
			this.colNullable.FieldName = "Nullable";
			this.colNullable.Name = "colNullable";
			this.colNullable.OptionsColumn.ReadOnly = true;
			this.gridControlColumns.DataSource = this.childrenBindingSource;
			this.gridControlColumns.Location = new System.Drawing.Point(15, 442);
			this.gridControlColumns.MainView = this.gridViewColumns;
			this.gridControlColumns.MenuManager = this.barManager1;
			this.gridControlColumns.Name = "gridControlColumns";
			this.gridControlColumns.Size = new System.Drawing.Size(275, 190);
			this.gridControlColumns.TabIndex = 16;
			this.gridControlColumns.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewColumns});
			this.childrenBindingSource.DataMember = "Children";
			this.childrenBindingSource.DataSource = this.availableDataBindingSource;
			this.availableDataBindingSource.DataSource = typeof(DevExpress.DataAccess.Native.Sql.QueryBuilder.AvailableItemData.List);
			this.barManager1.Controller = this.barAndDockingController;
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barListItem1,
			this.barButtonItemRename,
			this.barButtonItemDelete});
			this.barManager1.MaxItemId = 3;
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(1264, 0);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 681);
			this.barDockControlBottom.Size = new System.Drawing.Size(1264, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 681);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(1264, 0);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 681);
			this.barListItem1.Id = 0;
			this.barListItem1.Name = "barListItem1";
			this.barButtonItemRename.Caption = "Rename";
			this.barButtonItemRename.Id = 1;
			this.barButtonItemRename.Name = "barButtonItemRename";
			this.barButtonItemRename.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemRename_ItemClick);
			this.barButtonItemDelete.Caption = "Delete";
			this.barButtonItemDelete.Id = 2;
			this.barButtonItemDelete.Name = "barButtonItemDelete";
			this.barButtonItemDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemDelete_ItemClick);
			this.gridControlAvailable.AllowDrop = true;
			this.gridControlAvailable.DataSource = this.availableDataBindingSource;
			this.gridControlAvailable.Location = new System.Drawing.Point(12, 41);
			this.gridControlAvailable.MainView = this.gridViewTables;
			this.gridControlAvailable.MenuManager = this.barManager1;
			this.gridControlAvailable.Name = "gridControlAvailable";
			this.gridControlAvailable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemImageComboBoxNodeType});
			this.gridControlAvailable.Size = new System.Drawing.Size(281, 371);
			this.gridControlAvailable.TabIndex = 15;
			this.gridControlAvailable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewTables});
			this.gridControlAvailable.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControlAvailable_DragDrop);
			this.gridControlAvailable.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControlAvailable_DragOver);
			this.gridViewTables.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColType,
			this.gridColName});
			this.gridViewTables.GridControl = this.gridControlAvailable;
			this.gridViewTables.Name = "gridViewTables";
			this.gridViewTables.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.OptionsBehavior.AllowIncrementalSearch = true;
			this.gridViewTables.OptionsBehavior.AutoPopulateColumns = false;
			this.gridViewTables.OptionsBehavior.AutoUpdateTotalSummary = false;
			this.gridViewTables.OptionsBehavior.Editable = false;
			this.gridViewTables.OptionsBehavior.KeepGroupExpandedOnSorting = false;
			this.gridViewTables.OptionsBehavior.ReadOnly = true;
			this.gridViewTables.OptionsCustomization.AllowColumnMoving = false;
			this.gridViewTables.OptionsCustomization.AllowColumnResizing = false;
			this.gridViewTables.OptionsCustomization.AllowFilter = false;
			this.gridViewTables.OptionsCustomization.AllowGroup = false;
			this.gridViewTables.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridViewTables.OptionsCustomization.AllowSort = false;
			this.gridViewTables.OptionsDetail.EnableMasterViewMode = false;
			this.gridViewTables.OptionsFilter.AllowColumnMRUFilterList = false;
			this.gridViewTables.OptionsFilter.AllowFilterEditor = false;
			this.gridViewTables.OptionsFind.AllowFindPanel = false;
			this.gridViewTables.OptionsHint.ShowCellHints = false;
			this.gridViewTables.OptionsHint.ShowColumnHeaderHints = false;
			this.gridViewTables.OptionsHint.ShowFooterHints = false;
			this.gridViewTables.OptionsLayout.Columns.AddNewColumns = false;
			this.gridViewTables.OptionsLayout.Columns.RemoveOldColumns = false;
			this.gridViewTables.OptionsLayout.Columns.StoreLayout = false;
			this.gridViewTables.OptionsMenu.EnableColumnMenu = false;
			this.gridViewTables.OptionsMenu.EnableFooterMenu = false;
			this.gridViewTables.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridViewTables.OptionsMenu.ShowAddNewSummaryItem = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.OptionsMenu.ShowAutoFilterRowItem = false;
			this.gridViewTables.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
			this.gridViewTables.OptionsMenu.ShowGroupSortSummaryItems = false;
			this.gridViewTables.OptionsMenu.ShowSplitItem = false;
			this.gridViewTables.OptionsNavigation.AutoMoveRowFocus = false;
			this.gridViewTables.OptionsNavigation.UseTabKey = false;
			this.gridViewTables.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridViewTables.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridViewTables.OptionsView.AllowHtmlDrawGroups = false;
			this.gridViewTables.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
			this.gridViewTables.OptionsView.ShowColumnHeaders = false;
			this.gridViewTables.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridViewTables.OptionsView.ShowGroupPanel = false;
			this.gridViewTables.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.OptionsView.ShowIndicator = false;
			this.gridViewTables.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewTables.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewTables_RowStyle);
			this.gridViewTables.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewTables_FocusedRowChanged);
			this.gridViewTables.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewTables_MouseDown);
			this.gridViewTables.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridViewTables_MouseMove);
			this.gridViewTables.DoubleClick += new System.EventHandler(this.gridViewTables_DoubleClick);
			this.gridColType.ColumnEdit = this.repositoryItemImageComboBoxNodeType;
			this.gridColType.FieldName = "Type";
			this.gridColType.MaxWidth = 20;
			this.gridColType.Name = "gridColType";
			this.gridColType.Visible = true;
			this.gridColType.VisibleIndex = 0;
			this.gridColType.Width = 20;
			this.repositoryItemImageComboBoxNodeType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemImageComboBoxNodeType.Name = "repositoryItemImageComboBoxNodeType";
			this.repositoryItemImageComboBoxNodeType.ReadOnly = true;
			this.gridColName.FieldName = "Name";
			this.gridColName.Name = "gridColName";
			this.gridColName.Visible = true;
			this.gridColName.VisibleIndex = 1;
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.reSqlPanel);
			this.layoutControl1.Controls.Add(this.tlSelection);
			this.layoutControl1.Controls.Add(this.chbAllowEditSql);
			this.layoutControl1.Controls.Add(this.gridControlAvailable);
			this.layoutControl1.Controls.Add(this.btnFilter);
			this.layoutControl1.Controls.Add(this.btnPreview);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.gridControlQuery);
			this.layoutControl1.Controls.Add(this.gridControlColumns);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(0, 344, 1119, 621);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(1264, 681);
			this.layoutControl1.TabIndex = 0;
			this.reSqlPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.reSqlPanel.Location = new System.Drawing.Point(800, 41);
			this.reSqlPanel.Name = "reSqlPanel";
			this.reSqlPanel.Size = new System.Drawing.Size(452, 409);
			this.reSqlPanel.TabIndex = 19;
			this.tlSelection.AllowDrop = true;
			this.tlSelection.Appearance.BandPanel.BackColor = System.Drawing.SystemColors.Control;
			this.tlSelection.Appearance.BandPanel.Options.UseBackColor = true;
			this.tlSelection.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.colSelected,
			this.colName1,
			this.colCondition,
			this.colAggregated,
			this.colSortedAsc,
			this.colSortedDesc,
			this.colGroupedBy,
			this.colForeignKey});
			this.tlSelection.DataSource = this.selectionDataBindingSource;
			this.tlSelection.KeyFieldName = "Id";
			this.tlSelection.Location = new System.Drawing.Point(302, 41);
			this.tlSelection.Name = "tlSelection";
			this.tlSelection.OptionsBehavior.AllowRecursiveNodeChecking = true;
			this.tlSelection.OptionsCustomization.AllowBandMoving = false;
			this.tlSelection.OptionsCustomization.AllowColumnMoving = false;
			this.tlSelection.OptionsDragAndDrop.ExpandNodeOnDrag = false;
			this.tlSelection.OptionsFilter.AllowFilterEditor = false;
			this.tlSelection.OptionsMenu.EnableColumnMenu = false;
			this.tlSelection.OptionsView.ShowCheckBoxes = true;
			this.tlSelection.OptionsView.ShowIndicator = false;
			this.tlSelection.ParentFieldName = "Parent";
			this.tlSelection.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemCheckEditSelection,
			this.repositoryItemButtonEditEditJoin,
			this.repositoryItemButtonEditAddJoin,
			this.repositoryItemButtonEditJoined,
			this.repositoryItemButtonEditConditionDefault});
			this.tlSelection.RootValue = -1;
			this.tlSelection.Size = new System.Drawing.Size(489, 409);
			this.tlSelection.TabIndex = 18;
			this.tlSelection.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.tlSelection_CustomNodeCellEdit);
			this.tlSelection.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tlSelection_AfterCheckNode);
			this.tlSelection.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(this.tlSelection_CalcNodeDragImageIndex);
			this.tlSelection.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.tlSelection_CustomDrawNodeCell);
			this.tlSelection.DragDrop += new System.Windows.Forms.DragEventHandler(this.tlSelection_DragDrop);
			this.tlSelection.DragOver += new System.Windows.Forms.DragEventHandler(this.tlSelection_DragOver);
			this.tlSelection.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.tlSelection_GiveFeedback);
			this.tlSelection.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tlSelection_KeyUp);
			this.tlSelection.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tlSelection_MouseClick);
			this.tlSelection.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlSelection_MouseMove);
			this.colSelected.ColumnEdit = this.repositoryItemCheckEditSelection;
			this.colSelected.FieldName = "Selected";
			this.colSelected.Name = "colSelected";
			this.colSelected.OptionsColumn.AllowMove = false;
			this.colSelected.OptionsColumn.AllowSort = false;
			this.colSelected.Width = 63;
			this.repositoryItemCheckEditSelection.AutoHeight = false;
			this.repositoryItemCheckEditSelection.Name = "repositoryItemCheckEditSelection";
			this.repositoryItemCheckEditSelection.CheckedChanged += new System.EventHandler(this.repositoryItemCheckEditSelection_CheckedChanged);
			this.colName1.Caption = "Name";
			this.colName1.FieldName = "Name";
			this.colName1.MinWidth = 54;
			this.colName1.Name = "colName1";
			this.colName1.OptionsColumn.AllowEdit = false;
			this.colName1.OptionsColumn.AllowMove = false;
			this.colName1.OptionsColumn.AllowSort = false;
			this.colName1.Visible = true;
			this.colName1.VisibleIndex = 0;
			this.colName1.Width = 188;
			this.colCondition.Caption = "Join Information";
			this.colCondition.ColumnEdit = this.repositoryItemButtonEditConditionDefault;
			this.colCondition.FieldName = "Condition";
			this.colCondition.Name = "colCondition";
			this.colCondition.OptionsColumn.AllowMove = false;
			this.colCondition.OptionsColumn.ReadOnly = true;
			this.colCondition.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
			this.colCondition.Visible = true;
			this.colCondition.VisibleIndex = 1;
			this.colCondition.Width = 233;
			this.repositoryItemButtonEditConditionDefault.AutoHeight = false;
			this.repositoryItemButtonEditConditionDefault.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
			this.repositoryItemButtonEditConditionDefault.Name = "repositoryItemButtonEditConditionDefault";
			this.repositoryItemButtonEditConditionDefault.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.repositoryItemButtonEditConditionDefault.KeyUp += new System.Windows.Forms.KeyEventHandler(this.repositoryItemTextEditConditionDefault_KeyUp);
			this.colAggregated.FieldName = "Aggregated";
			this.colAggregated.Name = "colAggregated";
			this.colAggregated.OptionsColumn.AllowEdit = false;
			this.colAggregated.OptionsColumn.ReadOnly = true;
			this.colAggregated.Width = 70;
			this.colSortedAsc.FieldName = "SortedAsc";
			this.colSortedAsc.Name = "colSortedAsc";
			this.colSortedAsc.OptionsColumn.AllowEdit = false;
			this.colSortedAsc.OptionsColumn.ReadOnly = true;
			this.colSortedAsc.Width = 70;
			this.colSortedDesc.FieldName = "SortedDesc";
			this.colSortedDesc.Name = "colSortedDesc";
			this.colGroupedBy.FieldName = "GroupedBy";
			this.colGroupedBy.Name = "colGroupedBy";
			this.colGroupedBy.OptionsColumn.AllowEdit = false;
			this.colGroupedBy.OptionsColumn.ReadOnly = true;
			this.colGroupedBy.Width = 70;
			this.colForeignKey.FieldName = "ForeignKey";
			this.colForeignKey.Name = "colForeignKey";
			this.selectionDataBindingSource.DataSource = typeof(DevExpress.DataAccess.Native.Sql.QueryBuilder.SelectionItemData.List);
			this.repositoryItemButtonEditEditJoin.AutoHeight = false;
			this.repositoryItemButtonEditEditJoin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.repositoryItemButtonEditEditJoin.Name = "repositoryItemButtonEditEditJoin";
			this.repositoryItemButtonEditEditJoin.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.repositoryItemButtonEditEditJoin.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditEditJoin_ButtonClick);
			this.repositoryItemButtonEditAddJoin.AutoHeight = false;
			serializableAppearanceObject2.Options.UseImage = true;
			this.repositoryItemButtonEditAddJoin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, true)});
			this.repositoryItemButtonEditAddJoin.Name = "repositoryItemButtonEditAddJoin";
			this.repositoryItemButtonEditAddJoin.ReadOnly = true;
			this.repositoryItemButtonEditAddJoin.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.repositoryItemButtonEditAddJoin.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditAddJoin_ButtonClick);
			this.repositoryItemButtonEditJoined.AutoHeight = false;
			this.repositoryItemButtonEditJoined.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true)});
			this.repositoryItemButtonEditJoined.Name = "repositoryItemButtonEditJoined";
			this.repositoryItemButtonEditJoined.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.chbAllowEditSql.AutoSizeInLayoutControl = true;
			this.chbAllowEditSql.Location = new System.Drawing.Point(1162, 12);
			this.chbAllowEditSql.Name = "chbAllowEditSql";
			this.chbAllowEditSql.Properties.Caption = "Allow Edit SQL";
			this.chbAllowEditSql.Size = new System.Drawing.Size(90, 19);
			this.chbAllowEditSql.StyleController = this.layoutControl1;
			this.chbAllowEditSql.TabIndex = 15;
			this.chbAllowEditSql.CheckedChanged += new System.EventHandler(this.chbAllowEditSql_CheckedChanged);
			this.btnFilter.Location = new System.Drawing.Point(142, 647);
			this.btnFilter.Name = "btnFilter";
			this.btnFilter.Size = new System.Drawing.Size(86, 22);
			this.btnFilter.StyleController = this.layoutControl1;
			this.btnFilter.TabIndex = 12;
			this.btnFilter.Text = "Filter...";
			this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
			this.btnPreview.Location = new System.Drawing.Point(12, 647);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(126, 22);
			this.btnPreview.StyleController = this.layoutControl1;
			this.btnPreview.TabIndex = 11;
			this.btnPreview.Text = "Preview Results...";
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			this.btnCancel.Location = new System.Drawing.Point(1175, 647);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(77, 22);
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOk.Location = new System.Drawing.Point(1095, 647);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(76, 22);
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.TabIndex = 9;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.gridControlQuery.DataSource = this.queryGridDataBindingSource;
			this.gridControlQuery.Location = new System.Drawing.Point(302, 459);
			this.gridControlQuery.MainView = this.gridViewQuery;
			this.gridControlQuery.Name = "gridControlQuery";
			this.gridControlQuery.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemCheckEditQuery,
			this.repositoryItemLookUpEditSortingType,
			this.repositoryItemSpinEditSortOrder,
			this.repositoryItemLookUpEditNewItemColumn,
			this.repositoryItemComboBoxAggregateEdit});
			this.gridControlQuery.Size = new System.Drawing.Size(950, 176);
			this.gridControlQuery.TabIndex = 6;
			this.gridControlQuery.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewQuery});
			this.queryGridDataBindingSource.DataSource = typeof(DevExpress.DataAccess.Native.Sql.QueryBuilder.QueryGridItemData.List);
			this.gridViewQuery.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colColumn,
			this.colTable,
			this.colAlias,
			this.colOutput,
			this.colSortingType,
			this.colSortOrder,
			this.colGroupBy,
			this.colAggregate});
			this.gridViewQuery.GridControl = this.gridControlQuery;
			this.gridViewQuery.Name = "gridViewQuery";
			this.gridViewQuery.OptionsCustomization.AllowColumnMoving = false;
			this.gridViewQuery.OptionsCustomization.AllowSort = false;
			this.gridViewQuery.OptionsMenu.EnableColumnMenu = false;
			this.gridViewQuery.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
			this.gridViewQuery.OptionsView.ShowGroupPanel = false;
			this.gridViewQuery.OptionsView.ShowIndicator = false;
			this.gridViewQuery.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewQuery_CustomRowCellEdit);
			this.gridViewQuery.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewQuery_CustomColumnDisplayText);
			this.gridViewQuery.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridViewQuery_KeyUp);
			this.colColumn.Caption = "Column";
			this.colColumn.FieldName = "ColumnData";
			this.colColumn.Name = "colColumn";
			this.colColumn.Visible = true;
			this.colColumn.VisibleIndex = 0;
			this.colColumn.Width = 190;
			this.colTable.Caption = "Table";
			this.colTable.FieldName = "Table";
			this.colTable.Name = "colTable";
			this.colTable.OptionsColumn.AllowEdit = false;
			this.colTable.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
			this.colTable.Visible = true;
			this.colTable.VisibleIndex = 1;
			this.colTable.Width = 190;
			this.colAlias.Caption = "Alias";
			this.colAlias.FieldName = "Alias";
			this.colAlias.Name = "colAlias";
			this.colAlias.Visible = true;
			this.colAlias.VisibleIndex = 2;
			this.colAlias.Width = 190;
			this.colOutput.Caption = "Output";
			this.colOutput.ColumnEdit = this.repositoryItemCheckEditQuery;
			this.colOutput.FieldName = "Output";
			this.colOutput.Name = "colOutput";
			this.colOutput.Visible = true;
			this.colOutput.VisibleIndex = 3;
			this.colOutput.Width = 70;
			this.repositoryItemCheckEditQuery.AutoHeight = false;
			this.repositoryItemCheckEditQuery.Name = "repositoryItemCheckEditQuery";
			this.repositoryItemCheckEditQuery.CheckedChanged += new System.EventHandler(this.repositoryItemCheckEditQuery_CheckedChanged);
			this.colSortingType.Caption = "Sorting Type";
			this.colSortingType.ColumnEdit = this.repositoryItemLookUpEditSortingType;
			this.colSortingType.FieldName = "SortingType";
			this.colSortingType.Name = "colSortingType";
			this.colSortingType.Visible = true;
			this.colSortingType.VisibleIndex = 4;
			this.colSortingType.Width = 95;
			this.repositoryItemLookUpEditSortingType.AutoHeight = false;
			this.repositoryItemLookUpEditSortingType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemLookUpEditSortingType.DisplayMember = "Name";
			this.repositoryItemLookUpEditSortingType.Name = "repositoryItemLookUpEditSortingType";
			this.repositoryItemLookUpEditSortingType.NullText = "";
			this.repositoryItemLookUpEditSortingType.ShowFooter = false;
			this.repositoryItemLookUpEditSortingType.ShowHeader = false;
			this.repositoryItemLookUpEditSortingType.ShowLines = false;
			this.repositoryItemLookUpEditSortingType.ValueMember = "Value";
			this.repositoryItemLookUpEditSortingType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.repositoryItemLookUpEditSortingType_Closed);
			this.colSortOrder.Caption = "Sort Order";
			this.colSortOrder.ColumnEdit = this.repositoryItemSpinEditSortOrder;
			this.colSortOrder.FieldName = "SortOrder";
			this.colSortOrder.Name = "colSortOrder";
			this.colSortOrder.Visible = true;
			this.colSortOrder.VisibleIndex = 5;
			this.colSortOrder.Width = 70;
			this.repositoryItemSpinEditSortOrder.AutoHeight = false;
			this.repositoryItemSpinEditSortOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemSpinEditSortOrder.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.repositoryItemSpinEditSortOrder.IsFloatValue = false;
			this.repositoryItemSpinEditSortOrder.Mask.EditMask = "N00";
			this.repositoryItemSpinEditSortOrder.MaxValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.repositoryItemSpinEditSortOrder.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.repositoryItemSpinEditSortOrder.Name = "repositoryItemSpinEditSortOrder";
			this.repositoryItemSpinEditSortOrder.EditValueChanged += new System.EventHandler(this.repositoryItemSpinEditSortOrder_EditValueChanged);
			this.repositoryItemSpinEditSortOrder.Enter += new System.EventHandler(this.repositoryItemSpinEditSortOrder_Enter);
			this.colGroupBy.Caption = "Group By";
			this.colGroupBy.ColumnEdit = this.repositoryItemCheckEditQuery;
			this.colGroupBy.FieldName = "GroupBy";
			this.colGroupBy.Name = "colGroupBy";
			this.colGroupBy.Visible = true;
			this.colGroupBy.VisibleIndex = 6;
			this.colGroupBy.Width = 70;
			this.colAggregate.AppearanceCell.Options.UseTextOptions = true;
			this.colAggregate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.colAggregate.Caption = "Aggregate";
			this.colAggregate.ColumnEdit = this.repositoryItemComboBoxAggregateEdit;
			this.colAggregate.FieldName = "Aggregate";
			this.colAggregate.Name = "colAggregate";
			this.colAggregate.Visible = true;
			this.colAggregate.VisibleIndex = 7;
			this.colAggregate.Width = 95;
			this.repositoryItemComboBoxAggregateEdit.AutoHeight = false;
			this.repositoryItemComboBoxAggregateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemComboBoxAggregateEdit.Name = "repositoryItemComboBoxAggregateEdit";
			this.repositoryItemComboBoxAggregateEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.repositoryItemComboBoxAggregateEdit.EditValueChanged += new System.EventHandler(this.repositoryItemComboBoxAggregateEdit_EditValueChanged);
			this.repositoryItemLookUpEditNewItemColumn.AutoHeight = false;
			this.repositoryItemLookUpEditNewItemColumn.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemLookUpEditNewItemColumn.DisplayMember = "Column";
			this.repositoryItemLookUpEditNewItemColumn.Name = "repositoryItemLookUpEditNewItemColumn";
			this.repositoryItemLookUpEditNewItemColumn.NullText = "";
			this.repositoryItemLookUpEditNewItemColumn.ValueMember = "ColumnData";
			this.repositoryItemLookUpEditNewItemColumn.EditValueChanged += new System.EventHandler(this.repositoryItemLookUpEditNewItemColumn_EditValueChanged);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.splitterItem3,
			this.layoutItemGridPane,
			this.layoutItemTablesPane,
			this.layoutControlGroupColumns,
			this.layoutControlGroup3,
			this.splitterItemAvailableAndColumns,
			this.splitterItem4,
			this.layoutItemSelectionPane,
			this.layoutItemReSqlPanel,
			this.splitterItem1,
			this.emptySpaceItem3,
			this.layoutGroupTop});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 8);
			this.layoutControlGroup1.Size = new System.Drawing.Size(1264, 681);
			this.layoutControlGroup1.TextVisible = false;
			this.splitterItem3.AllowHotTrack = true;
			this.splitterItem3.Location = new System.Drawing.Point(290, 442);
			this.splitterItem3.Name = "splitterItem3";
			this.splitterItem3.Size = new System.Drawing.Size(954, 5);
			this.layoutItemGridPane.Control = this.gridControlQuery;
			this.layoutItemGridPane.Location = new System.Drawing.Point(290, 447);
			this.layoutItemGridPane.Name = "layoutItemGridPane";
			this.layoutItemGridPane.Size = new System.Drawing.Size(954, 180);
			this.layoutItemGridPane.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGridPane.TextVisible = false;
			this.layoutItemTablesPane.Control = this.gridControlAvailable;
			this.layoutItemTablesPane.Location = new System.Drawing.Point(0, 31);
			this.layoutItemTablesPane.Name = "layoutItemTablesPane";
			this.layoutItemTablesPane.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
			this.layoutItemTablesPane.Size = new System.Drawing.Size(285, 373);
			this.layoutItemTablesPane.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemTablesPane.TextVisible = false;
			this.layoutControlGroupColumns.ExpandButtonMode = DevExpress.Utils.Controls.ExpandButtonMode.Inverted;
			this.layoutControlGroupColumns.ExpandButtonVisible = true;
			this.layoutControlGroupColumns.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGridColumns});
			this.layoutControlGroupColumns.Location = new System.Drawing.Point(0, 409);
			this.layoutControlGroupColumns.Name = "layoutControlGroupColumns";
			this.layoutControlGroupColumns.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroupColumns.Size = new System.Drawing.Size(285, 218);
			this.layoutControlGroupColumns.Text = "Columns of ...";
			this.layoutItemGridColumns.Control = this.gridControlColumns;
			this.layoutItemGridColumns.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGridColumns.Name = "layoutItemGridColumns";
			this.layoutItemGridColumns.Size = new System.Drawing.Size(279, 194);
			this.layoutItemGridColumns.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGridColumns.TextVisible = false;
			this.layoutItemGridColumns.Shown += new System.EventHandler(this.layoutControlItemGridColumns_Shown);
			this.layoutItemGridColumns.Hidden += new System.EventHandler(this.layoutControlItemGridColumns_Hidden);
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemPreviewButton,
			this.layoutItemFilterButton,
			this.emptySpaceBottom,
			this.layoutItemCancelButton,
			this.layoutItemOkButton});
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 637);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Size = new System.Drawing.Size(1244, 26);
			this.layoutControlGroup3.TextVisible = false;
			this.layoutItemPreviewButton.Control = this.btnPreview;
			this.layoutItemPreviewButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemPreviewButton.Name = "layoutItemPreviewButton";
			this.layoutItemPreviewButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 4);
			this.layoutItemPreviewButton.Size = new System.Drawing.Size(130, 26);
			this.layoutItemPreviewButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreviewButton.TextVisible = false;
			this.layoutItemFilterButton.Control = this.btnFilter;
			this.layoutItemFilterButton.Location = new System.Drawing.Point(130, 0);
			this.layoutItemFilterButton.Name = "layoutItemFilterButton";
			this.layoutItemFilterButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 4);
			this.layoutItemFilterButton.Size = new System.Drawing.Size(90, 26);
			this.layoutItemFilterButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemFilterButton.TextVisible = false;
			this.emptySpaceBottom.AllowHotTrack = false;
			this.emptySpaceBottom.Location = new System.Drawing.Point(220, 0);
			this.emptySpaceBottom.Name = "emptySpaceBottom";
			this.emptySpaceBottom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 4);
			this.emptySpaceBottom.Size = new System.Drawing.Size(863, 26);
			this.emptySpaceBottom.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCancelButton.Control = this.btnCancel;
			this.layoutItemCancelButton.Location = new System.Drawing.Point(1163, 0);
			this.layoutItemCancelButton.Name = "layoutItemCancelButton";
			this.layoutItemCancelButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 4);
			this.layoutItemCancelButton.Size = new System.Drawing.Size(81, 26);
			this.layoutItemCancelButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCancelButton.TextVisible = false;
			this.layoutItemOkButton.Control = this.btnOk;
			this.layoutItemOkButton.Location = new System.Drawing.Point(1083, 0);
			this.layoutItemOkButton.Name = "layoutItemOkButton";
			this.layoutItemOkButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 4);
			this.layoutItemOkButton.Size = new System.Drawing.Size(80, 26);
			this.layoutItemOkButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemOkButton.TextVisible = false;
			this.splitterItemAvailableAndColumns.AllowHotTrack = true;
			this.splitterItemAvailableAndColumns.Location = new System.Drawing.Point(0, 404);
			this.splitterItemAvailableAndColumns.Name = "splitterItemAvailableAndColumns";
			this.splitterItemAvailableAndColumns.Size = new System.Drawing.Size(285, 5);
			this.layoutItemAllowEdit.Control = this.chbAllowEditSql;
			this.layoutItemAllowEdit.Location = new System.Drawing.Point(1150, 0);
			this.layoutItemAllowEdit.Name = "layoutItemAllowEdit";
			this.layoutItemAllowEdit.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 0);
			this.layoutItemAllowEdit.Size = new System.Drawing.Size(94, 21);
			this.layoutItemAllowEdit.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAllowEdit.TextVisible = false;
			this.emptySpaceTop.AllowHotTrack = false;
			this.emptySpaceTop.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceTop.Name = "emptySpaceTop";
			this.emptySpaceTop.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 0);
			this.emptySpaceTop.Size = new System.Drawing.Size(1150, 21);
			this.emptySpaceTop.TextSize = new System.Drawing.Size(0, 0);
			this.splitterItem4.AllowHotTrack = true;
			this.splitterItem4.Location = new System.Drawing.Point(285, 31);
			this.splitterItem4.Name = "splitterItem4";
			this.splitterItem4.Size = new System.Drawing.Size(5, 596);
			this.layoutItemSelectionPane.Control = this.tlSelection;
			this.layoutItemSelectionPane.Location = new System.Drawing.Point(290, 31);
			this.layoutItemSelectionPane.Name = "layoutItemSelectionPane";
			this.layoutItemSelectionPane.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
			this.layoutItemSelectionPane.Size = new System.Drawing.Size(493, 411);
			this.layoutItemSelectionPane.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSelectionPane.TextVisible = false;
			this.layoutItemReSqlPanel.Control = this.reSqlPanel;
			this.layoutItemReSqlPanel.Location = new System.Drawing.Point(788, 31);
			this.layoutItemReSqlPanel.Name = "layoutItemReSqlPanel";
			this.layoutItemReSqlPanel.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 2);
			this.layoutItemReSqlPanel.Size = new System.Drawing.Size(456, 411);
			this.layoutItemReSqlPanel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemReSqlPanel.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.Location = new System.Drawing.Point(783, 31);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(5, 411);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 627);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem3.Size = new System.Drawing.Size(1244, 10);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(0, 21);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(1244, 10);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.colType.Caption = "Type";
			this.colType.FieldName = "Type";
			this.colType.Name = "colType";
			this.popupMenu1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemRename),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemDelete)});
			this.popupMenu1.Manager = this.barManager1;
			this.popupMenu1.Name = "popupMenu1";
			this.layoutGroupTop.GroupBordersVisible = false;
			this.layoutGroupTop.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem4,
			this.layoutItemAllowEdit,
			this.emptySpaceTop});
			this.layoutGroupTop.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupTop.Name = "layoutGroupTop";
			this.layoutGroupTop.Size = new System.Drawing.Size(1244, 31);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "QueryBuilderView";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Query Builder";
			((System.ComponentModel.ISupportInitialize)(this.gridViewColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.childrenBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.availableDataBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlAvailable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewTables)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBoxNodeType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.reSqlPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tlSelection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditSelection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditConditionDefault)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.selectionDataBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditEditJoin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditAddJoin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditJoined)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllowEditSql.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControlQuery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.queryGridDataBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewQuery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditQuery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEditSortingType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditSortOrder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBoxAggregateEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEditNewItemColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridPane)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTablesPane)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilterButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancelButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOkButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItemAvailableAndColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectionPane)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemReSqlPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupTop)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControl1;
		protected XtraLayout.LayoutControlGroup layoutControlGroup1;
		protected XtraGrid.GridControl gridControlQuery;
		protected XtraGrid.Views.Grid.GridView gridViewQuery;
		protected XtraLayout.SplitterItem splitterItem3;
		protected XtraLayout.LayoutControlItem layoutItemGridPane;
		protected System.Windows.Forms.BindingSource queryGridDataBindingSource;
		protected XtraGrid.Columns.GridColumn colColumn;
		protected XtraGrid.Columns.GridColumn colTable;
		protected XtraGrid.Columns.GridColumn colAlias;
		protected XtraGrid.Columns.GridColumn colOutput;
		protected XtraGrid.Columns.GridColumn colSortingType;
		protected XtraGrid.Columns.GridColumn colSortOrder;
		protected XtraGrid.Columns.GridColumn colGroupBy;
		protected XtraGrid.Columns.GridColumn colAggregate;
		protected System.Windows.Forms.BindingSource selectionDataBindingSource;
		protected System.Windows.Forms.BindingSource availableDataBindingSource;
		protected XtraTreeList.Columns.TreeListColumn colType;
		protected XtraEditors.SimpleButton btnOk;
		protected XtraLayout.EmptySpaceItem emptySpaceBottom;
		protected XtraLayout.LayoutControlItem layoutItemOkButton;
		protected XtraEditors.SimpleButton btnCancel;
		protected XtraLayout.LayoutControlItem layoutItemCancelButton;
		protected XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditQuery;
		protected XtraEditors.SimpleButton btnPreview;
		protected XtraLayout.LayoutControlItem layoutItemPreviewButton;
		protected XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEditSortingType;
		protected XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEditSortOrder;
		protected XtraEditors.SimpleButton btnFilter;
		protected XtraLayout.LayoutControlItem layoutItemFilterButton;
		protected XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEditNewItemColumn;
		protected XtraBars.PopupMenu popupMenu1;
		protected XtraBars.BarListItem barListItem1;
		protected XtraBars.BarManager barManager1;
		protected XtraBars.BarDockControl barDockControlTop;
		protected XtraBars.BarDockControl barDockControlBottom;
		protected XtraBars.BarDockControl barDockControlLeft;
		protected XtraBars.BarDockControl barDockControlRight;
		protected XtraBars.BarButtonItem barButtonItemRename;
		protected XtraBars.BarButtonItem barButtonItemDelete;
		protected XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBoxAggregateEdit;
		protected XtraGrid.GridControl gridControlAvailable;
		protected XtraGrid.Views.Grid.GridView gridViewTables;
		protected XtraLayout.LayoutControlItem layoutItemTablesPane;
		protected XtraGrid.Columns.GridColumn gridColName;
		protected XtraGrid.Columns.GridColumn gridColType;
		protected XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBoxNodeType;
		protected XtraGrid.Views.Grid.GridView gridViewColumns;
		protected XtraGrid.GridControl gridControlColumns;
		protected XtraLayout.LayoutControlItem layoutItemGridColumns;
		protected XtraLayout.SplitterItem splitterItem4;
		protected XtraGrid.Columns.GridColumn colIsKey;
		protected XtraGrid.Columns.GridColumn colName;
		protected XtraGrid.Columns.GridColumn colType1;
		protected XtraGrid.Columns.GridColumn colNullable;
		protected System.Windows.Forms.BindingSource childrenBindingSource;
		protected XtraLayout.LayoutControlGroup layoutControlGroupColumns;
		protected XtraEditors.CheckEdit chbAllowEditSql;
		protected XtraLayout.LayoutControlGroup layoutControlGroup3;
		protected XtraLayout.SplitterItem splitterItemAvailableAndColumns;
		protected XtraLayout.LayoutControlItem layoutItemAllowEdit;
		protected XtraLayout.EmptySpaceItem emptySpaceTop;
		protected XtraEditors.PanelControl reSqlPanel;
		protected XtraTreeList.TreeList tlSelection;
		protected XtraTreeList.Columns.TreeListColumn colSelected;
		protected XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditSelection;
		protected XtraTreeList.Columns.TreeListColumn colName1;
		protected XtraTreeList.Columns.TreeListColumn colCondition;
		protected XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditConditionDefault;
		protected XtraTreeList.Columns.TreeListColumn colAggregated;
		protected XtraTreeList.Columns.TreeListColumn colSortedAsc;
		protected XtraTreeList.Columns.TreeListColumn colSortedDesc;
		protected XtraTreeList.Columns.TreeListColumn colGroupedBy;
		protected XtraTreeList.Columns.TreeListColumn colForeignKey;
		protected XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditEditJoin;
		protected XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditAddJoin;
		protected XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditJoined;
		protected XtraLayout.LayoutControlItem layoutItemSelectionPane;
		protected XtraLayout.LayoutControlItem layoutItemReSqlPanel;
		protected XtraLayout.SplitterItem splitterItem1;
		protected XtraLayout.EmptySpaceItem emptySpaceItem3;
		protected XtraLayout.EmptySpaceItem emptySpaceItem4;
		protected XtraBars.BarAndDockingController barAndDockingController;
		protected XtraLayout.LayoutControlGroup layoutGroupTop;
	}
}
