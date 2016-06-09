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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using System.Reflection;
using DevExpress.XtraMap.Native;
using DevExpress.Utils;
namespace DevExpress.XtraMap.Design {
	public abstract class CollectionEditorForm : XtraForm {
		Type typeToAdd;
		private DevExpress.XtraEditors.SimpleButton buttonCancel;
		private DevExpress.XtraEditors.SimpleButton buttonRemove;
		private DevExpress.XtraEditors.SimpleButton buttonUp;
		private DevExpress.XtraEditors.SimpleButton buttonDown;
		private LabelControl bottomLine;
		private DevExpress.Utils.Frames.PropertyGridEx propertyGrid;
		private IContainer components;
		private ListBoxControl listBox;
		private DropDownButton btnAdd;
		private DevExpress.XtraBars.PopupMenu popupMenu;
		private BarDockControl barDockControlTop;
		private BarDockControl barDockControlBottom;
		private BarDockControl barDockControlLeft;
		private BarDockControl barDockControlRight;
		private DevExpress.XtraBars.BarManager barManager;
		protected virtual int DefaultItemToAddIndex { get { return 0; } }
		protected virtual object[] EditValueArray { get { return new object[0]; } }
		public CollectionEditorForm() {
			InitializeComponent();
			InitItemsToAdd();
			SetSelectedIndex(0);
			UpdateUpDownButtons();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionEditorForm));
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonUp = new DevExpress.XtraEditors.SimpleButton();
			this.buttonDown = new DevExpress.XtraEditors.SimpleButton();
			this.listBox = new DevExpress.XtraEditors.ListBoxControl();
			this.bottomLine = new DevExpress.XtraEditors.LabelControl();
			this.propertyGrid = new DevExpress.Utils.Frames.PropertyGridEx();
			this.btnAdd = new DevExpress.XtraEditors.DropDownButton();
			this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.listBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonCancel.Name = "buttonCancel";
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonUp.Image")));
			this.buttonUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonUp, "buttonUp");
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			this.buttonDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonDown.Image")));
			this.buttonDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonDown, "buttonDown");
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			resources.ApplyResources(this.listBox, "listBox");
			this.listBox.Name = "listBox";
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
			this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
			resources.ApplyResources(this.bottomLine, "bottomLine");
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			this.bottomLine.Name = "bottomLine";
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.DrawFlat = false;
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.DropDownControl = this.popupMenu;
			this.btnAdd.ImageIndex = 0;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.popupMenu.Manager = this.barManager;
			this.popupMenu.Name = "popupMenu";
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 10;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.CancelButton = this.buttonCancel;
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.bottomLine);
			this.Controls.Add(this.listBox);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonUp);
			this.Controls.Add(this.buttonDown);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CollectionEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.listBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void btnAdd_Click(object sender, EventArgs e) {
			if (typeToAdd != null) {
				object newItem = Activator.CreateInstance(typeToAdd);
				if (newItem != null) {
					AddNewItemToCollection(newItem);
					UpdateListBox();
					listBox.SelectedItem = newItem;
					UpdatePropertyGrid();
					UpdateUpDownButtons();
					UpdateRemoveButton();
				}
			}
		}
		void buttonRemove_Click(object sender, System.EventArgs e) {
			RemoveItem();
		}
		void buttonUp_Click(object sender, System.EventArgs e) {
			int index = listBox.SelectedIndex;
			SwapItems(index - 1, index);
			UpdateListBox();
			SetSelectedIndex(index - 1);
		}
		void buttonDown_Click(object sender, System.EventArgs e) {
			int index = listBox.SelectedIndex;
			SwapItems(index, index + 1);
			UpdateListBox();
			SetSelectedIndex(index + 1);
		}
		void listBox_SelectedIndexChanged(object sender, EventArgs e) {
			UpdatePropertyGrid();
			UpdateUpDownButtons();
		}
		void listBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete)
				RemoveItem();
		}
		void barButtonItem_Click(object sender, ItemClickEventArgs e) {
			OnBarButtonItemClick(e);
			btnAdd.Text = GetButtonAddHeader(e.Item);
		}
		void UpdateRemoveButton() {
			buttonRemove.Enabled = listBox.ItemCount > 0;
		}
		void UpdatePropertyGrid() {
			try {
				propertyGrid.SelectedObject = listBox.SelectedItem;
			}
			catch {
				propertyGrid.SelectedObject = null;
			}
		}
		void InitDefaultItemToAdd() {
			int itemsCount = popupMenu.LinksPersistInfo.Count;
			if (itemsCount > 0 && DefaultItemToAddIndex >= 0 && DefaultItemToAddIndex < itemsCount) {
				BarItem defaultItem = popupMenu.LinksPersistInfo[DefaultItemToAddIndex].Item;
				btnAdd.Text = GetButtonAddHeader(defaultItem);
				typeToAdd = defaultItem.Tag as Type;
			}
		}
		void InitItemsToAdd() {
			IEnumerable<BarItem> items = GetBarItems();
			if (items != null) {
				foreach (BarItem barItem in items) {
					barManager.Items.Add(barItem);
					barItem.ItemClick += new ItemClickEventHandler(barButtonItem_Click);
					popupMenu.LinksPersistInfo.Add(new LinkPersistInfo(barItem));
				}
			}
			InitDefaultItemToAdd();
		}
		void RemoveItem() {
			object selectedItem = listBox.SelectedItem;
			if (selectedItem != null) {
				int index = listBox.SelectedIndex;
				RemoveItemFromCollection(selectedItem);
				UpdateListBox();
				SetSelectedIndex(index);
				UpdatePropertyGrid();
				UpdateRemoveButton();
				UpdateUpDownButtons();
			}
		}
		void UpdateUpDownButtons() {
			buttonUp.Enabled = listBox.SelectedIndex > 0;
			buttonDown.Enabled = listBox.SelectedIndex < listBox.ItemCount - 1 && listBox.ItemCount > 0;
		}
		void SetSelectedIndex(int index) {
			if (listBox.Items.Count > 0) {
				if (index < 0)
					index = 0;
				if (index >= listBox.Items.Count)
					index = listBox.Items.Count - 1;
				if (index >= 0 && index < listBox.Items.Count)
					listBox.SelectedIndex = index;
			}
		}
		void UpdateListBox() {
			listBox.BeginUpdate();
			listBox.Items.Clear();
			listBox.Items.AddRange(EditValueArray);
			listBox.EndUpdate();
		}
		void OnBarButtonItemClick(ItemClickEventArgs e) {
			Type newTypeToAdd = (Type)e.Item.Tag;
			if (newTypeToAdd != null)
				typeToAdd = newTypeToAdd;
		}
		protected void OnEditValueChanged() {
			UpdateListBox();
			SetSelectedIndex(0);
			UpdateRemoveButton();
		}
		protected void InitializePropertyGrid(IServiceProvider servProvider) {
			propertyGrid.Site = new CollectionSite(propertyGrid, servProvider);
		}
		protected string GetButtonAddHeader(BarItem selectedItem) {
			return "Add " + selectedItem.Caption;
		}
		protected BarItem CreateBarButtonItem(string caption, object tag) {
			BarItem barItem = new BarButtonItem();
			barItem.Caption = caption;
			barItem.Tag = tag;
			return barItem;
		}
		protected BarItem CreateBarButtonItem(string caption) {
			return CreateBarButtonItem(caption, null);
		}
		protected virtual void AddNewItemToCollection(object item) {
		}
		protected virtual void RemoveItemFromCollection(object item) {
		}
		protected IEnumerable<BarItem> GetBarItems() {
			Type[] itemTypes = GetItemTypes();
			foreach(Type item in itemTypes)
				yield return CreateBarButtonItem(item.Name, item);
		}
		protected virtual Type[] GetItemTypes() {
			return new Type[0];
		}
		protected virtual void SwapItems(int index1, int index2) {
		}
	}
}
