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
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Native;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OracleClient;
using DevExpress.XtraReports.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraReports.Design {
	public class WizPageTables : DevExpress.Utils.InteriorWizardPage {
		const string integratedSecurityIsTrue = "Integrated Security=True";
		#region static
		protected static string GetCompatibleOleDBConnectionString(string connetionString) {
			if(!(ConnectionStringHelper.GetConnectionType(connetionString) == ConnectionType.OleDB))
				return connetionString;
			if(connetionString.IndexOf(integratedSecurityIsTrue) >= 0)
				return connetionString.IndexOf("Provider=SQLOLEDB") >= 0 ? connetionString.Replace(integratedSecurityIsTrue, "Integrated Security=SSPI") : connetionString;
			else
				return connetionString.IndexOf("Prompt=") < 0 ? connetionString + ";Prompt=CompleteRequired" : connetionString;
		}
		public static DbDataAdapter CreateDataAdapter(string connectionString, string selectQuery) {
			if(ConnectionStringHelper.GetConnectionType(connectionString) == ConnectionType.Sql) {
				connectionString = ConnectionStringHelper.RemoveProviderFromConnectionString(connectionString);
				SqlCommand sqlSelectCommand = new SqlCommand(selectQuery, new SqlConnection(connectionString));
				return new SqlDataAdapter(sqlSelectCommand);
			}
			if(ConnectionStringHelper.GetConnectionType(connectionString) == ConnectionType.ODBC) {
				OdbcConnection connection = (OdbcConnection)ConnectionStringHelper.CreateDBConnection(connectionString);
				OdbcCommand odbcSelectCommand = new OdbcCommand(selectQuery, connection);
				return new OdbcDataAdapter(odbcSelectCommand);
			}
			OleDbCommand selectCommand = new OleDbCommand(selectQuery, new OleDbConnection(connectionString));
			return new OleDbDataAdapter(selectCommand);
		}
		public static DbDataAdapter CreateDataAdapter(DbConnection connection, string selectQuery) {
			if(connection is SqlConnection) {
				SqlCommand sqlSelectCommand = new SqlCommand(selectQuery, (SqlConnection)connection);
				return new SqlDataAdapter(sqlSelectCommand);
			}
			if(connection is OdbcConnection) {
				OdbcCommand odbcSelectCommand = new OdbcCommand(selectQuery, (OdbcConnection)connection);
				return new OdbcDataAdapter(odbcSelectCommand);
			}
#pragma warning disable 0618
			if(connection is OracleConnection) {
				OracleCommand oracleCommand = new OracleCommand(selectQuery, (OracleConnection)connection);
				return new OracleDataAdapter(oracleCommand);
			}
#pragma warning restore 0618
			OleDbCommand selectCommand = new OleDbCommand(selectQuery, (OleDbConnection)connection);
			return new OleDbDataAdapter(selectCommand);
		}
		static protected void AddItemsToNode(TreeNode node, string[] items, string[] tableSchemaNames, int imageIndex) {
			for(int i = 0; i < items.Length; i++) {
				bool showTableSchemaName = FindNode(node.Nodes, dbnode => { return items[i] == dbnode.TableName; }) != null;
				string tableSchemaName = !string.IsNullOrEmpty(tableSchemaNames[i]) ? tableSchemaNames[i] : null;
				DBTreeNode newNode = new DBTreeNode(items[i], imageIndex, tableSchemaName);
				if(showTableSchemaName)
					newNode.Text += string.Concat("(", tableSchemaName, ")");
				node.Nodes.Add(newNode);
			}
		}
		static DBTreeNode FindNode(TreeNodeCollection nodes, Predicate<DBTreeNode> predicate) {
			foreach(DBTreeNode node in nodes) {
				if(predicate(node))
					return node;
			}
			return null;
		}
		static void UpdateListViewColumnWidth(ListView lv) {
			lv.Columns[0].Width = lv.ClientRectangle.Width;
		}
		public static string GetRestrictionValue(DbConnection connection) {
			if(connection is SqlConnection)
				return "BASE TABLE";
#pragma warning disable 0618
			if(connection is OracleConnection)
				return "User";
#pragma warning restore 0618
			return "TABLE";
		}
		static string GetParamName(DbConnection connection) {
#pragma warning disable 0618
			if(connection is OracleConnection)
				return "TYPE";
#pragma warning restore 0618
			return "TABLE_TYPE";
		}
		static string GetViewColumnName(DbConnection connection) {
#pragma warning disable 0618
			return connection is OracleConnection ? "VIEW_NAME" : "TABLE_NAME";
#pragma warning restore 0618
		}
		static string[] GetColumnValues(DataRow[] dataRows, string columnName) {
			string[] columnValues = new string[dataRows.Length];
			if(dataRows.Length > 0 && dataRows[0].Table.Columns.Contains(columnName))
				for(int i = 0; i < dataRows.Length; i++)
					columnValues[i] = GetStringValue(dataRows[i][columnName]);
			return columnValues;
		}
		static string GetStringValue(object obj) {
			return obj != null && obj != System.DBNull.Value ? obj.ToString() : string.Empty;
		}
		#endregion
		public class DBListViewItem : ListViewItem {
			public string DataSetTable { get; private set; }
			public string TableName { get; private set; }
			public string TableSchemaName { get; private set; }
			public TreeNode TreeNodeParent { get; private set; }
			public DBListViewItem(string dataSetTable, int imageIndex, TreeNode parent, string tableName, string tableSchemaName)
				: base(tableName, imageIndex) {
				DataSetTable = dataSetTable;
				TreeNodeParent = parent;
				TableName = tableName;
				TableSchemaName = tableSchemaName;
			}
		}
		public class DBTreeNode : TreeNode {
			public string TableSchemaName { get; private set; }
			public string TableName { get; private set; }
			public DBTreeNode(string tableName, int imageIndex, string tableSchemaName)
				: base(tableName, imageIndex, imageIndex) {
				TableSchemaName = tableSchemaName;
				TableName = tableName;
			}
		}
		const int SB_HORZ = 0;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label lblItems;
		private System.Windows.Forms.Label lblAvailableItems;
		private System.Windows.Forms.Label lblSelectedItems;
		protected System.Windows.Forms.TreeView tvAvailableItems;
		private DevExpress.XtraEditors.SimpleButton btnAddItem;
		private DevExpress.XtraEditors.SimpleButton btnRemoveItem;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		protected System.Windows.Forms.ListView lvSelectedItems;
		protected TreeNode tablesNode;
		protected TreeNode viewsNode;
		private System.Windows.Forms.ImageList imageList;
		protected NewStandardReportWizard fWizard;
		DbConnection connection;
		readonly IDbSchemaHelper dbSchemaHelper = new DbSchemaHelperEx();
		protected IDbSchemaHelper DbSchemaHelper { get { return dbSchemaHelper; } }
		protected void CreateRootNodes() {
			tablesNode = new TreeNode("Tables", 0, 0);
			tvAvailableItems.Nodes.Add(tablesNode);
			viewsNode = new TreeNode("Views", 1, 1);
			tvAvailableItems.Nodes.Add(viewsNode);
		}
		protected void ExpandRootNodes() {
			tablesNode.Expand();
			viewsNode.Expand();
		}
		public WizPageTables(XRWizardRunnerBase runner)
			: this() {
			fWizard = (NewStandardReportWizard)runner.Wizard;
		}
		WizPageTables() {
			InitializeComponent();
			tvAvailableItems.Sort();
			ResourceImageHelper.FillImageListFromResources(imageList, "Images.DBItems.bmp", typeof(LocalResFinder));
			DevExpress.XtraPrinting.Native.Win32.ShowScrollBar(tvAvailableItems.Handle, SB_HORZ, false);
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopDataTable.gif", typeof(LocalResFinder));
			btnAddItem.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveRight.gif", typeof(LocalResFinder));
			btnRemoveItem.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveLeft.gif", typeof(LocalResFinder));
		}
		void Wizard_FormClosed(object sender, FormClosedEventArgs e) {
			connection.Close();
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageTables));
			this.lblItems = new System.Windows.Forms.Label();
			this.lblAvailableItems = new System.Windows.Forms.Label();
			this.lblSelectedItems = new System.Windows.Forms.Label();
			this.tvAvailableItems = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnAddItem = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemoveItem = new DevExpress.XtraEditors.SimpleButton();
			this.lvSelectedItems = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.lblItems, "lblItems");
			this.lblItems.Name = "lblItems";
			resources.ApplyResources(this.lblAvailableItems, "lblAvailableItems");
			this.lblAvailableItems.Name = "lblAvailableItems";
			resources.ApplyResources(this.lblSelectedItems, "lblSelectedItems");
			this.lblSelectedItems.Name = "lblSelectedItems";
			this.tvAvailableItems.AllowDrop = true;
			this.tvAvailableItems.HideSelection = false;
			resources.ApplyResources(this.tvAvailableItems, "tvAvailableItems");
			this.tvAvailableItems.ImageList = this.imageList;
			this.tvAvailableItems.ItemHeight = 16;
			this.tvAvailableItems.Name = "tvAvailableItems";
			this.tvAvailableItems.DoubleClick += new System.EventHandler(this.tvAvailableItems_DoubleClick);
			this.tvAvailableItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvAvailableItems_DragDrop);
			this.tvAvailableItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAvailableItems_AfterSelect);
			this.tvAvailableItems.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvAvailableItems_DragEnter);
			this.tvAvailableItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvAvailableItems_ItemDrag);
			this.tvAvailableItems.DragOver += new System.Windows.Forms.DragEventHandler(this.tvAvailableItems_DragOver);
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.TransparentColor = System.Drawing.Color.Magenta;
			this.btnAddItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAddItem, "btnAddItem");
			this.btnAddItem.Name = "btnAddItem";
			this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
			this.btnRemoveItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnRemoveItem, "btnRemoveItem");
			this.btnRemoveItem.Name = "btnRemoveItem";
			this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
			this.lvSelectedItems.AllowDrop = true;
			this.lvSelectedItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1});
			this.lvSelectedItems.FullRowSelect = true;
			this.lvSelectedItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvSelectedItems.HideSelection = false;
			resources.ApplyResources(this.lvSelectedItems, "lvSelectedItems");
			this.lvSelectedItems.MultiSelect = false;
			this.lvSelectedItems.Name = "lvSelectedItems";
			this.lvSelectedItems.SmallImageList = this.imageList;
			this.lvSelectedItems.UseCompatibleStateImageBehavior = false;
			this.lvSelectedItems.View = System.Windows.Forms.View.Details;
			this.lvSelectedItems.SelectedIndexChanged += new System.EventHandler(this.lvSelectedItems_SelectedIndexChanged);
			this.lvSelectedItems.DoubleClick += new System.EventHandler(this.lvSelectedItems_DoubleClick);
			this.lvSelectedItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvSelectedItems_DragDrop);
			this.lvSelectedItems.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvSelectedItems_DragEnter);
			this.lvSelectedItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvSelectedItems_ItemDrag);
			this.lvSelectedItems.DragOver += new System.Windows.Forms.DragEventHandler(this.lvSelectedItems_DragOver);
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			this.Controls.Add(this.lvSelectedItems);
			this.Controls.Add(this.btnAddItem);
			this.Controls.Add(this.tvAvailableItems);
			this.Controls.Add(this.lblSelectedItems);
			this.Controls.Add(this.lblAvailableItems);
			this.Controls.Add(this.lblItems);
			this.Controls.Add(this.btnRemoveItem);
			this.Name = "WizPageTables";
			this.Controls.SetChildIndex(this.btnRemoveItem, 0);
			this.Controls.SetChildIndex(this.lblItems, 0);
			this.Controls.SetChildIndex(this.lblAvailableItems, 0);
			this.Controls.SetChildIndex(this.lblSelectedItems, 0);
			this.Controls.SetChildIndex(this.tvAvailableItems, 0);
			this.Controls.SetChildIndex(this.btnAddItem, 0);
			this.Controls.SetChildIndex(this.lvSelectedItems, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected virtual bool CanAddItem() {
			return lvSelectedItems.Items.Count == 0 &&
				tvAvailableItems.SelectedNode != null &&
				tvAvailableItems.SelectedNode != tablesNode &&
				tvAvailableItems.SelectedNode != viewsNode;
		}
		bool CanRemoveItem() {
			return lvSelectedItems.SelectedItems.Count == 1;
		}
		bool IsInputFinished() {
			return lvSelectedItems.Items.Count >= 1;
		}
		protected virtual void UpdateButtons() {
			btnRemoveItem.Enabled = CanRemoveItem();
			btnAddItem.Enabled = CanAddItem();
			if(IsInputFinished())
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish | WizardButton.Next;
			else
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish;
		}
		protected virtual void FillAvailableItems() {
			tvAvailableItems.Nodes.Clear();
			lvSelectedItems.Items.Clear();
			CreateRootNodes();
			connection = ConnectionStringHelper.CreateDBConnection(GetCompatibleOleDBConnectionString(fWizard.ConnectionString));
			if(!TryOpenConnection(connection))
				return;
			try {
				DataRow[] schemaRows = dbSchemaHelper.GetSchemaTableRows(connection, "Tables", GetRestrictionValue(connection), GetParamName(connection));
				string[] tableNames = GetColumnValues(schemaRows, "TABLE_NAME");
				string[] tableSchemaNames = GetColumnValues(schemaRows, DbSchemaHelperEx.GetSchemaColumnName(connection));
				AddItemsToNode(tablesNode, tableNames, tableSchemaNames, 2);
				schemaRows = dbSchemaHelper.GetSchemaTableRows(connection, "Views", null, null);
				string[] viewNames = GetColumnValues(schemaRows, GetViewColumnName(connection));
				string[] viewSchemaNames = GetColumnValues(schemaRows, DbSchemaHelperEx.GetSchemaColumnName(connection));
				AddItemsToNode(viewsNode, viewNames, viewSchemaNames, 3);
				ExpandRootNodes();
			} catch {
			}
		}
		protected bool TryOpenConnection(DbConnection connection) {
			try {
				connection.Open();
			} catch(Exception e) {			 
				connection.Close();
				Form form = FindForm();
				UserLookAndFeel lookAndFeel = form is ISupportLookAndFeel ? ((ISupportLookAndFeel)form).LookAndFeel : null; 
				NotificationService.ShowException<XtraReport>(lookAndFeel, form, e);
				return false;
			}
			return true;
		}
		const string arrow = " -> ";
		void MoveSelectedItem(TreeView tv, ListView lv) {
			if(!CanAddItem())
				return;
			DBTreeNode node = tv.SelectedNode as DBTreeNode;
			if(node != null) {
				string dataSetTable = GetValidName(GetNames(lv.Items), node.TableName, 0);
				ListViewItem item = new DBListViewItem(dataSetTable, node.ImageIndex, node.Parent, node.TableName, node.TableSchemaName);
				item.Text = dataSetTable != node.TableName ? string.Concat(node.Text, arrow, dataSetTable) : node.Text;
				lv.Items.Add(item);
				item.Selected = true;
				node.Parent.Nodes.Remove(node);
			}
			UpdateButtons();
		}
		static string GetValidName(IList<string> names, string baseName, int baseIndex) {
			string name = baseIndex != 0 ? baseName + baseIndex : baseName;
			foreach(string item in names) {
				if(item == name)
					return GetValidName(names, baseName, baseIndex + 1);
			}
			return name;
		}
		static string[] GetNames(ListView.ListViewItemCollection items) {
			string[] names = new string[items.Count];
			for(int i = 0; i < items.Count; i++)
				names[i] = ((DBListViewItem)items[i]).TableName;
			return names;
		}
		void MoveSelectedItem(ListView lv, TreeView tv) {
			if(!CanRemoveItem())
				return;
			if(lv.SelectedItems.Count == 1) {
				DBListViewItem item = (DBListViewItem)lv.SelectedItems[0];
				TreeNode node = new DBTreeNode(item.TableName, item.ImageIndex, item.TableSchemaName);
				string[] entries = item.Text.Split(new string[] {arrow}, StringSplitOptions.RemoveEmptyEntries);
				if(entries.Length > 0)
					node.Text = entries[0];
				item.TreeNodeParent.Nodes.Add(node);
				lv.Items.Remove(item);
			}
			UpdateButtons();
		}
		protected override bool OnKillActive() {
			Wizard.FormClosed -= new FormClosedEventHandler(Wizard_FormClosed);
			return base.OnKillActive();
		}
		protected override bool OnSetActive() {
			Wizard.FormClosed += new FormClosedEventHandler(Wizard_FormClosed);
			UpdateListViewColumnWidth(lvSelectedItems);
			FillAvailableItems();
			UpdateButtons();
			tvAvailableItems.SelectedNode = tablesNode;
			return true;
		}
		protected override void UpdateWizardButtons() {
		}
		protected virtual DataSet CreateDataSet() {
			DBListViewItem item = (DBListViewItem)lvSelectedItems.Items[0];
			DataSet dataSet = new DataSet(fWizard.DatasetName);
			fWizard.DataAdapters.Clear();
			DbDataAdapter dataAdapter = CreateDataAdapter(dataSet, connection, null, item);
			fWizard.DataAdapters.Add(dataAdapter);
			connection.Close();
			return dataSet;
		}
		protected DbDataAdapter CreateDataAdapter(DataSet dataSet, DbConnection connection, string connectionString, DBListViewItem item) {
			string selectQuery = DbSchemaHelper.GetSelectColumnsQuery(connection, item.TableName, item.TableSchemaName);
			DbDataAdapter dataAdapter = !string.IsNullOrEmpty(connectionString) ?
				CreateDataAdapter(connectionString, selectQuery) :
				CreateDataAdapter(connection, selectQuery);
			dataAdapter.TableMappings.Add("Table", item.DataSetTable);
			DbSchemaHelper.FillSchema(connection, dataAdapter, dataSet, item.DataSetTable);
			return dataAdapter;
		}
		private void btnAddItem_Click(object sender, System.EventArgs e) {
			MoveSelectedItem(tvAvailableItems, lvSelectedItems);
		}
		private void btnRemoveItem_Click(object sender, System.EventArgs e) {
			MoveSelectedItem(lvSelectedItems, tvAvailableItems);
		}
		private void lvSelectedItems_DoubleClick(object sender, System.EventArgs e) {
			MoveSelectedItem(lvSelectedItems, tvAvailableItems);
		}
		private void tvAvailableItems_DoubleClick(object sender, System.EventArgs e) {
			MoveSelectedItem(tvAvailableItems, lvSelectedItems);
		}
		private void lvSelectedItems_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateButtons();
		}
		private void tvAvailableItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			UpdateButtons();
		}
		private void lvSelectedItems_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			MoveSelectedItem(tvAvailableItems, lvSelectedItems);
		}
		private void HandleSelectedDragOver(System.Windows.Forms.DragEventArgs e) {
			TreeView tv = (TreeView)e.Data.GetData(typeof(TreeView));
			if(tv == null || !CanAddItem())
				e.Effect = DragDropEffects.None;
			else
				e.Effect = e.AllowedEffect;
		}
		private void HandleAvailableDragOver(System.Windows.Forms.DragEventArgs e) {
			ListView lv = (ListView)e.Data.GetData(typeof(ListView));
			if(lv == null || !CanRemoveItem())
				e.Effect = DragDropEffects.None;
			else
				e.Effect = e.AllowedEffect;
		}
		private void lvSelectedItems_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleSelectedDragOver(e);
		}
		private void lvSelectedItems_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleSelectedDragOver(e);
		}
		private void tvAvailableItems_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e) {
			if(!e.Button.IsLeft())
				return;
			DoDragDrop(new DataObject(tvAvailableItems), DragDropEffects.Move);
		}
		private void lvSelectedItems_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e) {
			if(!e.Button.IsLeft())
				return;
			DoDragDrop(new DataObject(lvSelectedItems), DragDropEffects.Move);
		}
		private void tvAvailableItems_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			MoveSelectedItem(lvSelectedItems, tvAvailableItems);
		}
		private void tvAvailableItems_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleAvailableDragOver(e);
		}
		private void tvAvailableItems_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleAvailableDragOver(e);
		}
		protected override string OnWizardNext() {
			DataSet dataset = CreateDataSet();
			if(dataset == null)
				return DevExpress.Utils.WizardForm.NoPageChange;
			fWizard.Dataset = dataset;
			fWizard.TableSchemaName = ((DBListViewItem)lvSelectedItems.Items[0]).TableSchemaName;
			return DevExpress.Utils.WizardForm.NextPage;
		}
		protected override bool OnWizardFinish() {
			DataSet dataset = CreateDataSet();
			if(dataset == null)
				return false;
			fWizard.Dataset = dataset;
			return true;
		}
	}
	public static class DBObjectsHelper {
		static readonly IDbSchemaHelper dbSchemaHelper = new DbSchemaHelperEx();
		public static void MakeDataRelations(OleDbConnection connection, DataSet dataSet) {
			DataTable schemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, null);
			foreach(DataRow row in schemaTable.Rows) {
				string primariTableName = row["PK_TABLE_NAME"].ToString();
				string primaryColumnName = row["PK_COLUMN_NAME"].ToString();
				string childDataTable = row["FK_TABLE_NAME"].ToString();
				string childColumnName = row["FK_COLUMN_NAME"].ToString();
				string relationName = string.Format("{0}{1}", primariTableName, childDataTable);
				if(dataSet.Tables.Contains(childDataTable) && dataSet.Tables.Contains(primariTableName))
					CreateDataRelation(dataSet, relationName, primariTableName, primaryColumnName, childDataTable, childColumnName);
			}
		}
		static DataRelation CreateDataRelation(DataSet dataSet, string relationName, string primaryTableName, string primaryColumnName, string childTableName, string childColumnName) {
			if(dataSet.Relations.Contains(relationName))
				return null;
			DataColumn masterTableDataColumn = dataSet.Tables[primaryTableName].Columns[primaryColumnName];
			DataColumn childTableDataColumn = dataSet.Tables[childTableName].Columns[childColumnName];
			DataRelation dataRelation = dataSet.Relations.Add(relationName, masterTableDataColumn, childTableDataColumn);
			return dataRelation;
		}
	}
}
