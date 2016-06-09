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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraEditors.Design {
	[ToolboxItem(false)]
	public abstract class BaseRepositoryEditor : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		protected DropDownButton btAdd;
		protected SimpleButton btRemove;
		protected IMultiColumnListBox listView1;
		private IContainer components;
		protected System.Collections.Generic.List<object> itemsCore;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private System.Windows.Forms.ImageList imageList1;
		protected CheckButton btnSearch;
		private SearchControl searchControl;
		public BaseRepositoryEditor(int index)
			: base(index) {
			InitializeComponent();
			pgMain.BringToFront();
			groupControl1.BringToFront();
			InititalizeSearch();
			itemsCore = new System.Collections.Generic.List<object>();
			addItem.Image = new Bitmap(16, 16);
		}
		void InititalizeSearch() {
			this.searchControl.Client = this.listView1.MultiColumnListBox as ISearchControlClient;
			this.searchControl.Visible = this.btnSearch.Checked;
			this.listView1.MultiColumnListBox.BringToFront();
			this.btnSearch.Image = FindImage;
		}
		protected override IServiceProvider GetPropertyGridServiceProvider() {
			object selObject = null;
			if(pgMain.SelectedObjects != null && pgMain.SelectedObjects.Length > 0)
				selObject = pgMain.SelectedObjects[0];
			else selObject = pgMain.SelectedObject;
			Component component = selObject as Component;
			if(component != null) return component.Site;
			return null;
		}
		public override void DoInitFrame() {
			InitMenu();
			UpdateButtonText();
			RefreshListBox();
			if(itemsCore.Count == 0) DisabledRemove();
		}
		protected virtual void UpdateButtonText() {
			var element = GetDefaultElement();
			btAdd.Enabled = (element != null);
			if(element != null)
				btAdd.Text = string.Format(Properties.Resources.AddNameCaption, GetElementText(element));
			else
				btAdd.Text = string.Format(Properties.Resources.AddNameCaption, string.Empty);
		}
		protected object lastElement = null;
		protected virtual int GetElementImageIndex(string name) {
			return GetSortElementList().IndexOf(name);
		}
		protected abstract object GetDefaultElement();
		protected abstract bool GetElementVisible(object element);
		protected abstract object GetElement(int index);
		protected abstract object GetElementByName(string name);
		protected abstract int FindElement(object component);
		protected abstract string GetElementText(object element);
		protected abstract int GetElementCount();
		protected abstract ArrayList GetSortElementList();
		protected abstract Image GetElementImage(object element);
		protected abstract void AddNewItem(object item);
		protected abstract bool CanRemoveComponent(object component);
		protected abstract IList GetComponentCollection();
		DXPopupMenu popupMenu = new DXPopupMenu(), addRemoveMenu = new DXPopupMenu();
		DXMenuItem removeItem = new DXMenuItem(Properties.Resources.RemoveCaption);
		DXSubMenuItem addItem = new DXSubMenuItem(Properties.Resources.AddCaption);
		bool menuInitialized = false;
		protected virtual bool CanUseGlyphSkinning { get { return false; } }
		protected virtual DXMenuItem CreateMenuItem(string name) {
			object element = GetElementByName(name);
			DXMenuItem dxItem = new DXMenuItem(GetElementText(element), new EventHandler(OnClickMenuItem));
			dxItem.Image = GetElementMenuImage(element);
			dxItem.AllowGlyphSkinning = CanUseGlyphSkinning;
			dxItem.Tag = element;
			return dxItem;
		}
		protected virtual Image GetElementMenuImage(object element) {
			return GetElementImage(element) ?? GetDefaultElementImage(element);
		}
		protected virtual Image GetDefaultElementImage(object element) {
			return null;
		}
		protected virtual void InitializeAddRemoveMenu() {
			this.removeItem.Click += new EventHandler(OnMenuRemove);
			this.addRemoveMenu.Items.Add(this.addItem);
			this.addRemoveMenu.Items.Add(this.removeItem);
			this.addRemoveMenu.Items.Add(new DXMenuItem(Properties.Resources.RemoveUnusedCaption, new EventHandler(OnMenuRemoveAll)));
		}
		protected virtual void OnMenuRemove(object sender, EventArgs e) {
			btRemove_Click(this, EventArgs.Empty);
		}
		protected virtual void OnMenuRemoveAll(object sender, EventArgs e) {
			RemoveAllItems();
		}
		protected virtual void InitMenu() {
			if(this.menuInitialized) return;
			this.menuInitialized = true;
			InitializeAddRemoveMenu();
			InitializePopupMenu();
		}
		protected virtual void InitializePopupMenu() {
			ArrayList list = GetSortElementList();
			foreach(string name in list) {
				object element = GetElementByName(name);
				Image image = GetElementImage(element);
				if(image == null) image = new Bitmap(16, 16);
				imageList1.Images.Add(image);
				if(!GetElementVisible(GetElementByName(name))) continue;
				this.popupMenu.Items.Add(CreateMenuItem(name));
				this.addItem.Items.Add(CreateMenuItem(name));
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && components != null)
				components.Dispose();
			this.popupMenu.Dispose();
			this.addRemoveMenu.Dispose();
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseRepositoryEditor));
			this.btAdd = new DevExpress.XtraEditors.DropDownButton();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.searchControl = new DevExpress.XtraEditors.SearchControl();
			this.btnSearch = new DevExpress.XtraEditors.CheckButton();
			this.listView1 = MultiColumnListBoxCreator.CreateMultiColumnListBox();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pgMain.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgMainChanged);
			this.pnlControl.Controls.Add(this.btnSearch);
			this.pnlControl.Controls.Add(this.btAdd);
			this.pnlControl.Controls.Add(this.btRemove);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.btAdd, "btAdd");
			this.btAdd.Name = "btAdd";
			this.btAdd.ArrowButtonClick += new System.EventHandler(this.btAddM_Click);
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			resources.ApplyResources(this.btRemove, "btRemove");
			this.btRemove.Name = "btRemove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			resources.ApplyResources(this.imageList1, "imageList1");
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.groupControl1.CaptionImageUri.Uri = "";
			this.groupControl1.Controls.Add(this.searchControl);
			this.groupControl1.Controls.Add(this.listView1.MultiColumnListBox);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.searchControl, "searchControl");
			this.searchControl.Name = "searchControl";
			this.searchControl.Properties.ShowClearButton = false;
			this.searchControl.Properties.ShowSearchButton = false;
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.CheckedChanged += new System.EventHandler(this.btnSearch_CheckedChanged);
			this.btnSearch.ImageLocation = ImageLocation.MiddleCenter;
			this.listView1.Properties.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.listView1.MultiColumnListBox.Dock = DockStyle.Fill;
			this.listView1.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name",  "Name") {Width = 30},
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Type", "Type") {Width = 30}
			});
			resources.ApplyResources(this.listView1, "listView1");
			this.listView1.MultiColumnListBox.Name = "listView1";
			this.listView1.Properties.DisplayMember = "Type";
			this.listView1.ImageList = imageList1;
			this.listView1.SelectedItemChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.MultiColumnListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
			this.listView1.MultiColumnListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
			this.Name = "BaseRepositoryEditor";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void pgMainChanged(object sender, PropertyValueChangedEventArgs e) {
			if(e.ChangedItem.Label == "(Name)" && listView1.SelectedItem != null) {
				RefreshListBox();
			}
		}
		void OnClickMenuItem(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			AddNewItem(item.Tag);
			MenuItem mi = sender as MenuItem;
		}
		protected virtual void OnRemovingItem(object component) { }
		protected virtual void OnRemoveItem(object component) { }
		protected virtual bool RemoveItem(object component) {
			OnRemovingItem(component);
			if(!CanRemoveComponent(component)) return false;
			IDisposable disposable = component as IDisposable;
			if(disposable != null)
				disposable.Dispose();
			OnRemoveItem(component);
			RefreshListBox();
			if(itemsCore.Count == 0) DisabledRemove();
			return true;
		}
		protected virtual string WarningText {
			get {
				return Properties.Resources.RemoveUnusedWarning;
			}
		}
		bool removeAll = false;
		protected virtual void RemoveAllItems() {
			if(XtraMessageBox.Show(this.FindForm(), WarningText, Properties.Resources.RemoveUnusedCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Cursor.Current = Cursors.WaitCursor;
			removeAll = true;
			for(int i = GetComponentCollection().Count - 1; i >= 0; i--)
				RemoveItem(GetComponentCollection()[i]);
			removeAll = false;
			Cursor.Current = Cursors.Default;
			RefreshListBox();
			this.removeItem.Enabled = btRemove.Enabled = false;
			UpdateSelectedItems();
		}
		protected virtual void RefreshListBox() {
			if(removeAll) return;
			itemsCore.Clear();
			foreach(object component in GetComponentCollection()) {
				IDataRepositoryItem lvi = CreateDataRepositoryItem(component);
				lvi.Tag = component;
				itemsCore.Add(lvi);
			}
			this.removeItem.Enabled = btRemove.Enabled;
			listView1.MultiColumnListBox.SuspendLayout();
			listView1.Properties.DataSource = itemsCore;
			listView1.Update();
			listView1.MultiColumnListBox.ResumeLayout();
			RefreshPropertyGrid();
		}
		IDataRepositoryItem CreateDataRepositoryItem(object element) {
			string name = GetDataRepositoryItemName(element);
			string type = GetDataRepositoryItemType(element);
			int imageIndex = GetDataRepositoryItemImageIndex(element, type);
			return new DataRepositoryItem(name, type, imageIndex);
		}
		protected override void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			object element = GetElement(((PropertyGrid)sender).SelectedObject);
			if(ShouldUpdateRepositoryItem(element))
				UpdateDataRepositoryItem(element);
		}
		object GetElement(object selectedObject) {
			var wrapper = selectedObject as DevExpress.Utils.Design.IDXObjectWrapper;
			return (wrapper != null) ? wrapper.SourceObject : selectedObject;
		}
		protected virtual bool ShouldUpdateRepositoryItem(object element) {
			return false;
		}
		void UpdateDataRepositoryItem(object element) {
			var rItem = itemsCore.Find(item => ((IDataRepositoryItem)item).Tag == element) as DataRepositoryItem;
			if(rItem != null) {
				listView1.Properties.BeginUpdate();
				rItem.Name = GetDataRepositoryItemName(element);
				rItem.Type = GetDataRepositoryItemType(element);
				rItem.ImageIndex = GetDataRepositoryItemImageIndex(element, rItem.Type);
				listView1.MultiColumnListBox.Invalidate();
				listView1.Properties.EndUpdate();
			}
		}
		protected virtual int GetDataRepositoryItemImageIndex(object element, string type) {
			return NormalizeImageIndex(GetElementImageIndex(type));
		}
		protected virtual string GetDataRepositoryItemType(object element) {
			int index = FindElement(element);
			return GetElementText(GetElement(index));
		}
		protected virtual string GetDataRepositoryItemName(object element) {
			IComponent component = element as IComponent;
			string name = element.ToString();
			if(component != null && component.Site != null)
				name = component.Site.Name;
			return name;
		}
		int NormalizeImageIndex(int index) {
			if(imageList1.Images.Count > 0 && index > -1) {
				if(index >= imageList1.Images.Count)
					return index - imageList1.Images.Count;
				return index;
			}
			return -1;
		}
		protected override object SelectedObject {
			get {
				if(itemsCore.Count > 0 && listView1.SelectedItem != null)
					return (listView1.SelectedItem as IDataRepositoryItem).Tag;
				else return 0;
			}
		}
		private void btAddM_Click(object sender, System.EventArgs e) {
			MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(popupMenu, btAdd, new Point(0, btAdd.Size.Height));
		}
		void DisabledRemove() {
			btRemove.Enabled = this.removeItem.Enabled = false;
		}
		void btRemove_Click(object sender, System.EventArgs e) {
			if(itemsCore == null || itemsCore.Count <= 0 || listView1.SelectedItem == null) return;
			RemoveItem((listView1.SelectedItem as IDataRepositoryItem).Tag);
		}
		void listView1_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				this.BeginInvoke(new MethodInvoker(delegate() {
					MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(addRemoveMenu, listView1.MultiColumnListBox, new Point(e.X, e.Y));
				}));
			}
		}
		void listView1_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateSelectedItems();
		}
		void UpdateSelectedItems() {
			RefreshPropertyGrid();
			btRemove.Enabled = (listView1.SelectedItem != null && CanRemoveComponent((listView1.SelectedItem as IDataRepositoryItem).Tag));
			this.removeItem.Enabled = btRemove.Enabled;
			if(listView1.SelectedItem != null)
				OnSelectedItemChanged((listView1.SelectedItem as IDataRepositoryItem).Tag);
		}
		protected virtual void OnSelectedItemChanged(object item) { }
		void listView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Delete:
					btRemove_Click(sender, new EventArgs());
					e.Handled = true;
					break;
				case Keys.Insert:
					btAdd_Click(this, new EventArgs());
					break;
			}
		}
		void btAdd_Click(object sender, System.EventArgs e) { AddNewItem(GetDefaultElement()); }
		void btnSearch_CheckedChanged(object sender, EventArgs e) {
			this.searchControl.Visible = btnSearch.Checked;
			if(!btnSearch.Checked)
				this.searchControl.SetFilter(null);
		}
		interface IDataRepositoryItem {
			object Tag { get; set; }
		}
		class DataRepositoryItem : IDataRepositoryItem {
			public DataRepositoryItem()
				: this(null, null, -1) { }
			public DataRepositoryItem(string name)
				: this(name, null, -1) { }
			public DataRepositoryItem(string name, int imageIndex)
				: this(name, null, imageIndex) { }
			public DataRepositoryItem(string name, string type, int imageIndex) {
				Name = name;
				Type = type;
				ImageIndex = imageIndex;
			}
			public int ImageIndex { get; set; }
			public string Name { get; set; }
			public string Type { get; set; }
			public object Tag { get; set; }
		}
	}
}
