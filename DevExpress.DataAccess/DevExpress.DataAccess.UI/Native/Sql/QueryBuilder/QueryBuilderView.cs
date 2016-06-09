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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraWaitForm;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using Padding = System.Windows.Forms.Padding;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public partial class QueryBuilderView : XtraForm, IQueryBuilderView {
		struct SelectedTable {
			public SelectedTable(string tableName) : this() { TableName = tableName; }
			public string TableName { get; private set; }
		}
		struct AvailableTable {
			public AvailableTable(string tableName) : this() { TableName = tableName; }
			public string TableName { get; private set; }
		}
		class QueryExceptionHandler : ExceptionHandler {
			public QueryExceptionHandler(QueryBuilderView owner)
				: base(
					owner.LookAndFeel, owner,
					DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle)) { }
			#region Overrides of ExceptionHandler
			protected override string GetMessage(Exception exception) {
				if(exception is NoTablesValidationException || exception is NoColumnsValidationException)
					return DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderNothingSelected);
				return base.GetMessage(exception);
			}
			#endregion
		}
		class InitialStateTreeListOperation : TreeListOperation {
			public override void Execute(TreeListNode node) {
				SelectionItemData item = (SelectionItemData) node.TreeList.GetDataRecordByNode(node);
				if(item != null)
					node.CheckState = item.Selected.HasValue ? item.Selected.Value ? CheckState.Checked : CheckState.Unchecked : CheckState.Indeterminate;
			}
		}
		class SetBoundFieldTreeListOperation : TreeListOperation {
			public override void Execute(TreeListNode node) {
				SelectionItemData item = (SelectionItemData) node.TreeList.GetDataRecordByNode(node);
				switch(node.CheckState) {
					case CheckState.Checked:
						item.Selected = true;
						break;
					case CheckState.Unchecked:
						item.Selected = false;
						break;
					case CheckState.Indeterminate:
						item.Selected = null;
						break;
				}
			}
		}
		[UserRepositoryItem("Register")]
		class NullRepositoryItem : RepositoryItem {
			static NullRepositoryItem() { Register(); }
			internal const string EditorName = "NullObjectEdit";
			public static void Register() {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(NullEdit), typeof(NullRepositoryItem), typeof(BaseEditViewInfo), new BaseEditPainter(), false));
			}
			BaseAccessible accessible;
			public override string EditorTypeName { get { return EditorName; } }
			protected override BaseAccessible CreateAccessibleInstance() { return accessible ?? (accessible = new BaseEditAccessible(this)); }
		}
		class NullEdit : BaseEdit {
			static NullEdit() { NullRepositoryItem.Register(); }
			public override string EditorTypeName { get { return NullRepositoryItem.EditorName; } }
		}
		protected readonly IParameterService parameterService;
		readonly IQueryBuilderViewModel viewModel;
		readonly IWin32Window owner;
		readonly WaitFormActivator waitFormActivator;
		readonly IServiceProvider propertyGridServices;
		readonly IExceptionHandler exceptionHandler;
		readonly RepositoryItem nullRepositoryItem = new NullRepositoryItem();
		readonly SqlRichEditControl reSql;
		readonly List<ColumnLookUpDataItem> columnsLookUpDataItems;
		Point lastMouseDownLocation;
		string lastMouseDownAvailableTable;
		public QueryBuilderView(IQueryBuilderViewModel viewModel, IWin32Window owner, UserLookAndFeel lookAndFeel, IParameterService parameterService)
			: this(viewModel, owner, lookAndFeel, parameterService, null) {}
		public QueryBuilderView(IQueryBuilderViewModel viewModel, IWin32Window owner, UserLookAndFeel lookAndFeel, IParameterService parameterService, IServiceProvider propertyGridServices)
			: this() {
			this.propertyGridServices = propertyGridServices;
			this.viewModel = viewModel;
			this.owner = owner;
			this.waitFormActivator = new WaitFormActivator(this, typeof(DemoWaitForm));
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			this.parameterService = parameterService;
			this.exceptionHandler = new LoaderExceptionHandler(owner, lookAndFeel);
			repositoryItemImageComboBoxNodeType.SmallImages = new ImageCollection(this.components) {
				Images = { ImageHelper.GetImage("table"), ImageHelper.GetImage("view") }
			};
			this.gridControlAvailable.DataSource = viewModel.Available;
			this.tlSelection.DataSource = viewModel.Selection;
			viewModel.Selection.ListChanged += Selection_ListChanged;
			this.gridControlQuery.DataSource = viewModel.QueryGrid;
			#region sqlRichEditInitialize
			SqlSyntaxColors syntaxColors = new SqlSyntaxColors(LookAndFeel);
			this.reSql = new SqlRichEditControl(syntaxColors);
			InitializeSqlRichEditControl();
			#endregion
			this.reSql.Text = viewModel.SqlText;
			UpdateAllowEditSql();
			SetSqlSupported(viewModel.IsSqlSupported);
			viewModel.BeforeUpdate += OnBeforeUpdate;
			viewModel.AfterUpdate += OnAfterUpdate;
			viewModel.ShowWaitForm += OnShowWaitForm;
			viewModel.HideWaitForm += OnHideWaitForm;
			viewModel.Error += OnError;
			viewModel.SqlTextChanged += OnSqlTextChanged;
			viewModel.AllowEditSqlChanged += (sender, args) => UpdateAllowEditSql();
			viewModel.IsSqlSupportedChanged += OnIsSqlSupportedChanged;
			viewModel.AliasFormatterChanged += OnAliasFormatterChanged;
			this.tlSelection.NodeChanged += tlSelection_NodeChanged;
			this.reSql.Leave += reSql_Leave;
		}
		QueryBuilderView() {
			InitializeComponent();
			LocalizeComponent();
			this.barAndDockingController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.repositoryItemLookUpEditSortingType.Columns.AddRange(new[] {
				new LookUpColumnInfo("Name", "Name"),
				new LookUpColumnInfo("Value", "Value", 20, Utils.FormatType.None, "", false, Utils.HorzAlignment.Default)
			});
			this.repositoryItemLookUpEditSortingType.DataSource = new[] {
				new {
					Name = DataAccessUILocalizer.GetString(DataAccessUIStringId.SortingTypeNone),
					Value = (SortingInfo.SortingDirection?)null
				},
				new {
					Name = DataAccessUILocalizer.GetString(DataAccessUIStringId.SortingTypeAscending),
					Value = (SortingInfo.SortingDirection?)SortingInfo.SortingDirection.Ascending
				},
				new {
					Name = DataAccessUILocalizer.GetString(DataAccessUIStringId.SortingTypeDescending),
					Value = (SortingInfo.SortingDirection?)SortingInfo.SortingDirection.Descending
				}
			};
			this.repositoryItemButtonEditAddJoin.Buttons[0].Image = ImageHelper.GetImage("JoinTable");
			this.repositoryItemButtonEditJoined.Buttons[0].Image = ImageHelper.GetImage("TableJoined");
			this.repositoryItemComboBoxAggregateEdit.Items.AddRange(Enum.GetValues(typeof(AggregationType)));
			this.columnsLookUpDataItems = new List<ColumnLookUpDataItem>();
			this.repositoryItemLookUpEditNewItemColumn.Columns.AddRange(new[] {
				new LookUpColumnInfo("TableDisplayName", DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderTable)),
				new LookUpColumnInfo("ColumnDisplayName", DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumn))
			});
			repositoryItemLookUpEditNewItemColumn.DisplayMember = "ColumnDisplayName";
			this.repositoryItemLookUpEditNewItemColumn.DataSource = this.columnsLookUpDataItems;
			this.repositoryItemImageComboBoxNodeType.Items.AddRange(new[] {
				new ImageComboBoxItem("None", AvailableItemData.NodeType.None, -1),
				new ImageComboBoxItem("Table", AvailableItemData.NodeType.Table, 0),
				new ImageComboBoxItem("View", AvailableItemData.NodeType.View, 1),
				new ImageComboBoxItem("Column", AvailableItemData.NodeType.Column, -1)
			});
		}
		#region Implementation of IQueryBuilderView
		public virtual void Start() { ShowDialog(this.owner); }
		public void ShowError(string message) {
			XtraMessageBox.Show(LookAndFeel, this, message,
				DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}
		public void Stop() { Close(); }
		public event EventHandler Ok;
		#endregion
		protected override bool ProcessDialogKey(Keys keyData) {
			switch(keyData) {
				case Keys.Delete:
					if(this.tlSelection.HasFocus) {
						TreeListNode node = this.tlSelection.FocusedNode;
						if(node == null || node.Level != 0)
							return true;
						this.tlSelection.CloseEditor();
						DeleteNodeFromSelection(node);
						return true;
					}
					break;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected virtual JoinEditorView CreateJoinEditorView() { return new JoinEditorView(this, LookAndFeel); }
		protected virtual FiltersView CreateFilterView() { return new FiltersView(this, LookAndFeel, null, null, null, parameterService, propertyGridServices); }
		protected virtual IEnumerable<ColumnLookUpDataItem> ColumnLookUpData() {
			return this.tlSelection.Nodes.SelectMany(
				tableNode =>
					tableNode.Nodes.Select(
						columnNode =>
							new ColumnLookUpDataItem((string)tableNode.GetValue(this.colName1),
								(string)columnNode.GetValue(this.colName1))));
		}
		protected virtual string GetColumnsListCaption(string name) {
			return string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumnsOf), name);
		}
		protected virtual string GetTableDisplayName(string table) { return table; }
		protected virtual string GetColumnDisplayName(string table, string column) { return column; }
		static Rectangle DrawIcon(Graphics graphics, Rectangle contentArea, string imageName) {
			const int imgWidth = 16;
			const int imgHeight = 16;
			const int ingOffset = 1;
			int x = contentArea.X + contentArea.Width - imgWidth;
			int y = contentArea.Y + (contentArea.Height - imgHeight) / 2;
			Image image = ImageHelper.GetImage(imageName);
			graphics.DrawImage(image, x, y, imgWidth, imgHeight);
			return new Rectangle(contentArea.X, contentArea.Y, contentArea.Width - imgWidth - ingOffset, contentArea.Height);
		}
		void LocalizeComponent() {
			barButtonItemRename.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilder_Rename);
			barButtonItemDelete.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilder_Delete);
			colName1.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Name);
			colCondition.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinInformation);
			chbAllowEditSql.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilder_AllowEdit);
			btnFilter.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderButtons_Filter);
			btnPreview.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderButtons_PreviewResults);
			btnCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			btnOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			colColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumn);
			colTable.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderTable);
			colAlias.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_Alias);
			colOutput.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_Output);
			colSortingType.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_SortingType);
			colSortOrder.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_SortOrder);
			colGroupBy.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_GroupBy);
			colAggregate.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumns_Aggregate);
			colType.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Type);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilder);
		}
		void InitializeSqlRichEditControl() {
			this.reSql.ActiveViewType = RichEditViewType.Draft;
			this.reSql.Name = "sqlEditor";
			this.reSql.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			this.reSql.Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Auto;
			this.reSql.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Auto;
			this.reSql.SyntaxHelper = null;
			this.reSql.Views.DraftView.AllowDisplayLineNumbers = true;
			this.reSql.Views.DraftView.Padding = new Padding(0, 0, 0, 0);
			this.reSql.Views.DraftView.AllowDisplayLineNumbers = false;
			this.reSql.InitializeDocument += reSql_InitializeDocument;
			this.reSql.SyntaxColors.SqlEnabled = true;
			this.reSql.ActiveView.BackColor = this.reSql.SyntaxColors.BackgroundColor;
			this.reSql.ReadOnly = false;
			this.reSql.Dock = DockStyle.Fill;
			this.reSql.AllowDrop = true;
			this.reSqlPanel.Controls.Add(this.reSql);
			this.reSql.SyntaxHelper = new SyntaxHelper(this.reSql);
			this.reSql.CreateNewDocument();
		}
		bool ConfirmTablesRemoving(IEnumerable<string> tables) {
			return
				WarningAsk(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlRemoveTables), 
						string.Join(",\r\n", tables.Select(t => string.Format("\t\u2022 {0}", GetTableDisplayName(t)))) + ".\r\n")
				);
		}
		bool WarningAsk(string question) {
			return Ask(question, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxIcon.Warning);
		}
		bool Ask(string question, string caption, MessageBoxIcon icon) {
			return XtraMessageBox.Show(LookAndFeel, this, question, caption, MessageBoxButtons.OKCancel, icon) == DialogResult.OK;
		}
		void UpdateAllowEditSql() {
			chbAllowEditSql.Checked = viewModel.AllowEditSql;
			reSql.SyntaxColors.SqlEnabled = viewModel.AllowEditSql;
			reSql.ActiveView.BackColor = reSql.SyntaxColors.BackgroundColor;
			bool en = !viewModel.AllowEditSql;
			tlSelection.Enabled = en;
			gridControlQuery.Enabled = en;
			reSql.ReadOnly = en;
			btnFilter.Enabled = en;
			LayoutVisibility vis = en ? LayoutVisibility.Always : LayoutVisibility.Never;
			layoutItemSelectionPane.Visibility = vis;
			layoutItemGridPane.Visibility = vis;
			splitterItem3.Visibility = vis;
			splitterItem1.Visibility = vis;
		}
		void SetSqlSupported(bool value) {
			var layoutVisibility = value ? LayoutVisibility.Always : LayoutVisibility.Never;
			this.layoutItemReSqlPanel.Visibility = layoutVisibility;
			this.layoutItemAllowEdit.Visibility = layoutVisibility;
			this.emptySpaceTop.Visibility = layoutVisibility;
			this.splitterItem1.Visibility = layoutVisibility;
		}
		void DeleteNodeFromSelection(TreeListNode node) {
			int index = this.tlSelection.GetVisibleIndexByNode(node);
			TreeListNode nodeToFocus = this.tlSelection.GetNodeByVisibleIndex(index - 1);
			bool isExpanded = false;
			if(nodeToFocus != null) {
				if(nodeToFocus.ParentNode != null)
					nodeToFocus = nodeToFocus.ParentNode;
				isExpanded = nodeToFocus.Expanded;
			}
			string table = node.GetValue(this.colName1).ToString();
			this.viewModel.OnTableRemoveFromSelection(table, ConfirmTablesRemoving);
			if(this.viewModel.Selection.All(item => item.Name != table) && nodeToFocus != null) {
				this.tlSelection.FocusedNode = nodeToFocus;
				this.tlSelection.FocusedNode.Expanded = isExpanded;
			}
		}
		void RefreshColumnLookUp() {
			this.columnsLookUpDataItems.Clear();
			this.columnsLookUpDataItems.AddRange(ColumnLookUpData());
		}
		void RenameFocusedTableInSelection() {
			if(this.tlSelection.FocusedColumn == null)
				return;
			this.tlSelection.FocusedColumn = this.colName1;
			this.colName1.OptionsColumn.AllowEdit = true;
			this.tlSelection.ShowEditor();
			this.tlSelection.ActiveEditor.Leave += (o, args) => this.colName1.OptionsColumn.AllowEdit = false;
		}
		string GetAvailableTable(Point location) {
			GridHitInfo hitInfo = this.gridViewTables.CalcHitInfo(location);
			return !hitInfo.InRow ? null : this.gridViewTables.GetRowCellValue(hitInfo.RowHandle, this.gridColName).ToString();
		}
		void UpdateColumnsList(int focusedRowHandle) {
			object columns = null;
			int sourceRowIndex = this.gridViewTables.GetDataSourceRowIndex(focusedRowHandle);
			if(sourceRowIndex >= 0) {
				AvailableItemData data = this.viewModel.Available[sourceRowIndex];
				if(data != null) {
					if(data.Children != null && data.Children.Count == 1 && data.Children[0].Name == null)
						this.viewModel.OnAvailableExpand(data.Name);
					columns = data.Children;
					this.layoutControlGroupColumns.Text = GetColumnsListCaption(data.Name);
				}
			}
			this.gridControlColumns.DataSource = columns;
			this.gridControlColumns.RefreshDataSource();
			this.gridControlColumns.Invalidate();
			this.gridControlColumns.Update();
		}
		#region ViewModel Event handlers
		void OnBeforeUpdate(object sender, EventArgs eventArgs) { SuspendLayout(); }
		void OnAfterUpdate(object sender, EventArgs eventArgs) {
			ResumeLayout();
			RefreshColumnLookUp();
		}
		void OnShowWaitForm(object sender, EventArgs eventArgs) { this.waitFormActivator.ShowWaitForm(true, true); }
		void OnHideWaitForm(object sender, EventArgs eventArgs) { this.waitFormActivator.CloseWaitForm(); }
		void OnError(object sender, ErrorEventArgs errorEventArgs) {
			this.exceptionHandler.HandleException(errorEventArgs.Error);
		}
		void OnSqlTextChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
			this.reSql.Text = this.viewModel.SqlText;
		}
		void OnIsSqlSupportedChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) { SetSqlSupported(this.viewModel.IsSqlSupported); }
		void OnAliasFormatterChanged(object sender, PropertyChangedEventArgs e) {
			this.reSql.SetAliasFormatter(this.viewModel.AliasFormatter);
		}
		void Selection_ListChanged(object sender, ListChangedEventArgs e) {
			if(!this.viewModel.IsUpdating)
				return;
			this.tlSelection.NodesIterator.DoOperation(new InitialStateTreeListOperation());
		}
		#endregion
		#region UI Event handlers
		#region gridControlAvailable
		void gridControlAvailable_DragDrop(object sender, DragEventArgs e) {
			var treeListNode = e.Data.GetData(typeof(SelectedTable)) as SelectedTable?;
			if(treeListNode != null)
				this.viewModel.OnTableRemoveFromSelection(treeListNode.Value.TableName, ConfirmTablesRemoving);
		}
		void gridControlAvailable_DragOver(object sender, DragEventArgs e) {
			e.Effect = e.Data.GetDataPresent(typeof(SelectedTable)) ? DragDropEffects.Move : DragDropEffects.None;
		}
		#endregion
		#region gridViewTables
		void gridViewTables_DoubleClick(object sender, EventArgs e) {
			int dataSourceRowIndex = this.gridViewTables.GetDataSourceRowIndex(this.gridViewTables.FocusedRowHandle);
			if(dataSourceRowIndex < 0)
				return;
			var data = this.viewModel.Available[dataSourceRowIndex];
			this.viewModel.OnTableAddToSelection(data.Name, CreateJoinEditorView);
		}
		void gridViewTables_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			if(e.FocusedRowHandle == e.PrevFocusedRowHandle)
				return;
			if(this.layoutControlGroupColumns.Expanded)
				UpdateColumnsList(e.FocusedRowHandle);
			else {
				int sourceRowIndex = this.gridViewTables.GetDataSourceRowIndex(e.FocusedRowHandle);
				if(sourceRowIndex >= 0) {
					AvailableItemData data = this.viewModel.Available[sourceRowIndex];
					if(data != null)
						this.layoutControlGroupColumns.Text = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryBuilderColumnsOf), data.Name);
				}
			}
		}
		void gridViewTables_MouseDown(object sender, MouseEventArgs e) {
			this.lastMouseDownLocation = e.Location;
			this.lastMouseDownAvailableTable = GetAvailableTable(e.Location);
		}
		void gridViewTables_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && ModifierKeys == Keys.None && this.lastMouseDownLocation != e.Location) {
				string table = GetAvailableTable(e.Location);
				if(table != null && table == this.lastMouseDownAvailableTable)
					DoDragDrop(new AvailableTable(table), DragDropEffects.Copy);
			}
		}
		void gridViewTables_RowStyle(object sender, RowStyleEventArgs e) {
			int sourceRowIndex = this.gridViewTables.GetDataSourceRowIndex(e.RowHandle);
			if(sourceRowIndex >= 0 && !this.viewModel.Available[sourceRowIndex].Shadowed)
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
		}
		#endregion
		#region layoutControlItemGridColumns
		void layoutControlItemGridColumns_Hidden(object sender, EventArgs e) {
			this.splitterItemAvailableAndColumns.Visibility = LayoutVisibility.Never;
		}
		void layoutControlItemGridColumns_Shown(object sender, EventArgs e) {
			UpdateColumnsList(this.gridViewTables.FocusedRowHandle);
			this.splitterItemAvailableAndColumns.Visibility = LayoutVisibility.Always;
		}
		#endregion
		void gridViewColumns_RowStyle(object sender, RowStyleEventArgs e) {
			GridView view = (GridView)sender;
			int sourceRowIndex = this.gridViewTables.GetDataSourceRowIndex(view.SourceRowHandle);
			if(sourceRowIndex >= 0 && this.viewModel.Available[sourceRowIndex].Shadowed)
				e.Appearance.ForeColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("DisabledText");
		}
		#region tlSelection
		void tlSelection_AfterCheckNode(object sender, NodeEventArgs e) {
			SelectionItemData item = (SelectionItemData)e.Node.TreeList.GetDataRecordByNode(e.Node);
			item.Selected = e.Node.Checked;
			e.Node.TreeList.NodesIterator.DoLocalOperation(new SetBoundFieldTreeListOperation(), e.Node.Nodes);
		}
		void tlSelection_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e) { e.ImageIndex = -1; }
		void tlSelection_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			if(e.Column == this.colName1) {
				Rectangle contentArea = e.EditViewInfo.ContentRect;
				if((bool)e.Node.GetValue(this.colGroupedBy))
					contentArea = DrawIcon(e.Graphics, contentArea, "GroupBy");
				if((bool)e.Node.GetValue(this.colAggregated))
					contentArea = DrawIcon(e.Graphics, contentArea, "Summary_16x16");
				if((bool)e.Node.GetValue(this.colSortedAsc))
					contentArea = DrawIcon(e.Graphics, contentArea, "SortAsc_16x16");
				if((bool)e.Node.GetValue(this.colSortedDesc))
					contentArea = DrawIcon(e.Graphics, contentArea, "SortDesc_16x16");
				e.Graphics.DrawString(e.CellText, e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), contentArea, e.Appearance.GetStringFormat());
				e.Handled = true;
			}
		}
		void tlSelection_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e) {
			if(e.Column == this.colCondition) {
				if(e.Node.ParentNode == null) {
					if(e.Node.GetValue(this.colCondition) != null)
						e.RepositoryItem = this.repositoryItemButtonEditEditJoin;
				}
				else {
					switch((SelectionItemData.FKState)e.Node.GetValue(this.colForeignKey)) {
						case SelectionItemData.FKState.CanBeJoined:
							e.RepositoryItem = this.repositoryItemButtonEditAddJoin;
							break;
						case SelectionItemData.FKState.AlreadyJoined:
							e.RepositoryItem = this.repositoryItemButtonEditJoined;
							break;
					}
				}
			}
		}
		void tlSelection_DragDrop(object sender, DragEventArgs e) {
			if(e.Data.GetDataPresent(typeof(AvailableTable)))
				this.viewModel.OnTableAddToSelection(((AvailableTable)e.Data.GetData(typeof(AvailableTable))).TableName,
					CreateJoinEditorView);
		}
		void tlSelection_DragOver(object sender, DragEventArgs e) {
			e.Effect = e.Data.GetDataPresent(typeof(AvailableTable)) ? DragDropEffects.Copy : DragDropEffects.None;
		}
		void tlSelection_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			if((e.Effect & DragDropEffects.Move) == DragDropEffects.Move) {
				e.UseDefaultCursors = false;
				Cursor.Current = DragManager.DragRemoveCursor;
			}
			else
				e.UseDefaultCursors = true;
		}
		void tlSelection_KeyUp(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.F2)
				RenameFocusedTableInSelection();
		}
		void tlSelection_MouseClick(object sender, MouseEventArgs e) {
			this.lastMouseDownLocation = e.Location;
			if(e.Button == MouseButtons.Right) {
				TreeListHitInfo hitInfo = this.tlSelection.CalcHitInfo(e.Location);
				if(hitInfo.Node != null && hitInfo.Node.Level == 0 && hitInfo.Column == this.colName1) {
					this.tlSelection.FocusedNode = hitInfo.Node;
					this.popupMenu1.ShowPopup(this.tlSelection.PointToScreen(e.Location));
				}
			}
		}
		void tlSelection_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && ModifierKeys == Keys.None && this.lastMouseDownLocation != e.Location) {
				TreeListHitInfo hitInfo = this.tlSelection.CalcHitInfo(e.Location);
				if(hitInfo.Node == null && hitInfo.HitInfoType == HitInfoType.Column)
					this.tlSelection.DoDragDrop(new object(), DragDropEffects.None);
				if(hitInfo.Node != null && hitInfo.Node.ParentNode == null && hitInfo.Column == this.colName1) {
					string tableName = hitInfo.Node.GetValue(this.colName1).ToString();
					if(!string.IsNullOrEmpty(tableName))
						this.tlSelection.DoDragDrop(new SelectedTable(tableName), DragDropEffects.Move);
				}
			}
		}
		void tlSelection_NodeChanged(object sender, NodeChangedEventArgs e) {
			if(e.ChangeType == NodeChangeTypeEnum.Add && e.Node.ParentNode != null && !e.Node.ParentNode.Expanded)
				e.Node.ParentNode.Expanded = true;
		}
		#endregion
		void barButtonItemRename_ItemClick(object sender, ItemClickEventArgs e) {
			RenameFocusedTableInSelection();
		}
		void barButtonItemDelete_ItemClick(object sender, ItemClickEventArgs e) {
			TreeListNode node = this.tlSelection.FocusedNode;
			if(node == null)
				return;
			DeleteNodeFromSelection(node.ParentNode ?? node);
		}
		#region gridViewQuery
		void gridViewQuery_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column == this.colAggregate && e.Value != null && e.Value as AggregationType? == AggregationType.None)
				e.DisplayText = string.Empty;
		}
		void gridViewQuery_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
			int dataSourceRowIndex = this.gridViewQuery.GetDataSourceRowIndex(e.RowHandle);
			if(dataSourceRowIndex < 0 || this.viewModel.QueryGrid[dataSourceRowIndex].Column == null) {
				e.RepositoryItem = e.Column == this.colColumn ? this.repositoryItemLookUpEditNewItemColumn : this.nullRepositoryItem;
			}
			else {
				if(e.Column == this.colColumn)
					e.RepositoryItem = this.repositoryItemLookUpEditNewItemColumn;
				else if(e.Column == this.colSortOrder) {
					if(e.CellValue == null)
						e.RepositoryItem = this.nullRepositoryItem;
				}
			}
		}
		void gridViewQuery_KeyUp(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				int dataSourceRowIndex = this.gridViewQuery.GetDataSourceRowIndex(this.gridViewQuery.FocusedRowHandle);
				if(dataSourceRowIndex < 0)
					return;
				this.viewModel.QueryGrid.RemoveAt(dataSourceRowIndex);
			}
		}
		#endregion
		void chbAllowEditSql_CheckedChanged(object sender, EventArgs e) {
			if(!this.chbAllowEditSql.Checked && this.viewModel.CustomSqlModified &&
			   XtraMessageBox.Show(LookAndFeel, this,
				   DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlExpressionChanged),
				   DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.YesNo,
				   MessageBoxIcon.Warning) == DialogResult.No) {
				this.chbAllowEditSql.Checked = true;
				return;
			}
			this.viewModel.AllowEditSql = this.chbAllowEditSql.Checked;
		}
		#region reSql
		void reSql_InitializeDocument(object sender, EventArgs e) {
			Document document = this.reSql.Document;
			DocumentRange range = document.Range;
			CharacterProperties cp = document.BeginUpdateCharacters(range);
			cp.FontName = "Courier New";
			cp.FontSize = 10.0f;
			document.EndUpdateCharacters(cp);
		}
		void reSql_Leave(object sender, EventArgs e) {
			if(this.viewModel.IsUpdating)
				return;
			this.viewModel.SqlText = this.reSql.Text;
		}
		#endregion
		#region Repository items
		void repositoryItemButtonEditAddJoin_ButtonClick(object sender, ButtonPressedEventArgs e) {
			TreeListNode node = this.tlSelection.FocusedNode;
			string column = (string)node.GetValue(this.colName1);
			string table = (string)node.ParentNode.GetValue(this.colName1);
			this.viewModel.OnJoinWithForeignKey(table, column);
		}
		void repositoryItemButtonEditEditJoin_ButtonClick(object sender, ButtonPressedEventArgs e) {
			TreeListNode node = this.tlSelection.FocusedNode;
			this.viewModel.OnEditJoin((string)node.GetValue(this.colName1), CreateJoinEditorView);
		}
		void repositoryItemCheckEditSelection_CheckedChanged(object sender, EventArgs e) { this.tlSelection.PostEditor(); }
		void repositoryItemCheckEditQuery_CheckedChanged(object sender, EventArgs e) { this.gridViewQuery.PostEditor(); }
		void repositoryItemComboBoxAggregateEdit_EditValueChanged(object sender, EventArgs e) {
			this.gridViewQuery.PostEditor();
		}
		void repositoryItemLookUpEditNewItemColumn_EditValueChanged(object sender, EventArgs e) {
			int index;
			try { index = ((LookUpEdit)this.gridViewQuery.ActiveEditor).ItemIndex; }
			catch {
				return;
			}
			this.gridViewQuery.SetRowCellValue(this.gridViewQuery.FocusedRowHandle, this.colTable,
				this.repositoryItemLookUpEditNewItemColumn.GetDataSourceValue("Table", index));
			this.gridViewQuery.CloseEditor();
		}
		void repositoryItemLookUpEditSortingType_Closed(object sender, ClosedEventArgs e) {
			this.gridViewQuery.PostEditor();
		}
		void repositoryItemSpinEditSortOrder_EditValueChanged(object sender, EventArgs e) {
			this.gridViewQuery.PostEditor();
		}
		void repositoryItemSpinEditSortOrder_Enter(object sender, EventArgs e) {
			this.repositoryItemSpinEditSortOrder.MaxValue = this.viewModel.QueryGrid.Count(data => data.SortOrder != null);
		}
		void repositoryItemTextEditConditionDefault_KeyUp(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Delete) {
				this.tlSelection.CloseEditor();
				TreeListNode node = this.tlSelection.FocusedNode;
				DeleteNodeFromSelection(node.ParentNode ?? node);
			}
		}
		#endregion
		#region Buttons
		void btnPreview_Click(object sender, EventArgs e) {
			SelectedDataEx data = null;
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationTokenHook hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			this.waitFormActivator.ShowWaitForm(true, true);
			this.waitFormActivator.SetWaitFormObject(hook);
			Exception exception = null;
			try {
				Task<SelectedDataEx> task = this.viewModel.GetPreviewDataAsync(token);
				task.Wait(token);
				if(task.Status == TaskStatus.RanToCompletion)
					data = task.Result;
			}
			catch(Exception ex) {
				exception = ex;
			}
			finally { this.waitFormActivator.CloseWaitForm(); }
			if(exception != null) {
				AggregateException aex = exception as AggregateException;
				if(aex != null) {
					aex.Flatten();
					if(aex.InnerExceptions.Count == 1)
						exception = aex.GetBaseException();
				}
				new QueryExceptionHandler(this).HandleException(exception);
			}
			if(data == null)
				return;
			using(DataPreviewForm previewForm = new DataPreviewForm(!this.viewModel.IsTopThousandApplicable()) {
				DataSource = new ResultTable(string.Empty, data)
			}) {
				previewForm.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				previewForm.ShowDialog(this);
			}
		}
		void btnFilter_Click(object sender, EventArgs e) { this.viewModel.OnEditFilter(CreateFilterView); }
		void btnOk_Click(object sender, EventArgs e) {
			if(Ok != null)
				Ok(this, EventArgs.Empty);
		}
		void btnCancel_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }
		#endregion
		#endregion
	}
	public class ColumnLookUpDataItem {
		public ColumnLookUpDataItem(string table, string column) {
			TableDisplayName = Table = table;
			ColumnDisplayName = Column = column;
		}
		public string Table { get; private set; }
		public string TableDisplayName { get; protected set; }
		public string Column { get; private set; }
		public string ColumnDisplayName { get; protected set; }
		public ColumnDataItem ColumnData { get { return new ColumnDataItem(Table, Column); } }
	}
}
