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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraBars.Design;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Ribbon.Design {
	public class RibbonPopupItemsEditor : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private System.ComponentModel.IContainer components = null;
		public RibbonPopupItemsEditor() {
			InitializeComponent();
		}
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraBars.Customization.BarItemsListBox lbItems;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private CategoriesComboBox cbCategories;
		private PopupItemLinksControl lbMenuDesigner;
		private DevExpress.XtraEditors.ImageComboBoxEdit cbMenus;
		private DevExpress.XtraEditors.PanelControl panelControl3;
		private DevExpress.Utils.ImageCollection imageCollection1;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			if(lbItems.Manager != null) lbItems.Manager.Items.CollectionChanged -= OnManagerItemsChanged;
			if(SelectionService != null) SelectionService.SelectionChanged -= OnSelectionObjectChanged;
			base.Dispose(disposing);
		}
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		public BarManager Manager { get { return Ribbon == null ? null : Ribbon.Manager; } }
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonPopupItemsEditor));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			this.lbMenuDesigner = new DevExpress.XtraBars.Ribbon.Design.PopupItemLinksControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.cbMenus = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.lbItems = new DevExpress.XtraBars.Customization.BarItemsListBox();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.cbCategories = new DevExpress.XtraBars.Ribbon.Design.CategoriesComboBox();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbMenus.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbCategories.Properties)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(490, 56);
			this.splMain.Size = new System.Drawing.Size(6, 419);
			this.pgMain.Location = new System.Drawing.Point(496, 56);
			this.pgMain.Size = new System.Drawing.Size(244, 419);
			this.pnlControl.Size = new System.Drawing.Size(740, 28);
			this.lbCaption.Size = new System.Drawing.Size(740, 42);
			this.pnlMain.Controls.Add(this.splitContainerControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 56);
			this.pnlMain.Size = new System.Drawing.Size(490, 419);
			this.horzSplitter.Size = new System.Drawing.Size(740, 4);
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.panelControl3);
			this.splitContainerControl1.Panel1.Controls.Add(this.panelControl2);
			this.splitContainerControl1.Panel1.Text = "splitContainerControl1_Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.lbItems);
			this.splitContainerControl1.Panel2.Controls.Add(this.panelControl1);
			this.splitContainerControl1.Panel2.Text = "splitContainerControl1_Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(490, 419);
			this.splitContainerControl1.SplitterPosition = 229;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.panelControl3.Controls.Add(this.lbMenuDesigner);
			this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl3.Location = new System.Drawing.Point(0, 41);
			this.panelControl3.Name = "panelControl3";
			this.panelControl3.Size = new System.Drawing.Size(225, 374);
			this.panelControl3.TabIndex = 3;
			this.lbMenuDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbMenuDesigner.Editor = null;
			this.lbMenuDesigner.Enabled = false;
			this.lbMenuDesigner.Form = null;
			this.lbMenuDesigner.IsVertical = false;
			this.lbMenuDesigner.Location = new System.Drawing.Point(2, 2);
			this.lbMenuDesigner.LockLayout = 0;
			this.lbMenuDesigner.Name = "lbMenuDesigner";
			this.lbMenuDesigner.Size = new System.Drawing.Size(221, 370);
			this.lbMenuDesigner.TabIndex = 2;
			this.lbMenuDesigner.TabStop = false;
			this.panelControl2.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.Appearance.Options.UseBackColor = true;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.cbMenus);
			this.panelControl2.Controls.Add(this.labelControl2);
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl2.Location = new System.Drawing.Point(0, 0);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Size = new System.Drawing.Size(225, 41);
			this.panelControl2.TabIndex = 1;
			this.cbMenus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbMenus.Location = new System.Drawing.Point(68, 9);
			this.cbMenus.Name = "cbMenus";
			this.cbMenus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
			this.cbMenus.Properties.SmallImages = this.imageCollection1;
			this.cbMenus.Size = new System.Drawing.Size(154, 20);
			this.cbMenus.TabIndex = 2;
			this.cbMenus.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbMenus_ButtonClick);
			this.cbMenus.SelectedIndexChanged += new System.EventHandler(this.cbMenus_SelectedIndexChanged);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.imageCollection1.TransparentColor = System.Drawing.Color.Magenta;
			this.labelControl2.Location = new System.Drawing.Point(3, 13);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(30, 13);
			this.labelControl2.TabIndex = 1;
			this.labelControl2.Text = "Menu:";
			this.lbItems.Appearance.BackColor = System.Drawing.Color.White;
			this.lbItems.Appearance.Options.UseBackColor = true;
			this.lbItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbItems.ItemHeight = 16;
			this.lbItems.Location = new System.Drawing.Point(0, 41);
			this.lbItems.Name = "lbItems";
			this.lbItems.Size = new System.Drawing.Size(251, 374);
			this.lbItems.TabIndex = 1;
			this.lbItems.SelectedIndexChanged += new System.EventHandler(this.lbItems_SelectedIndexChanged);
			this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.cbCategories);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelControl1.Location = new System.Drawing.Point(0, 0);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(251, 41);
			this.panelControl1.TabIndex = 0;
			this.labelControl1.Location = new System.Drawing.Point(3, 13);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(49, 13);
			this.labelControl1.TabIndex = 1;
			this.labelControl1.Text = "Category:";
			this.cbCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbCategories.Location = new System.Drawing.Point(77, 9);
			this.cbCategories.Manager = null;
			this.cbCategories.Name = "cbCategories";
			this.cbCategories.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbCategories.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbCategories.Size = new System.Drawing.Size(170, 20);
			this.cbCategories.TabIndex = 0;
			this.cbCategories.SelectedIndexChanged += new System.EventHandler(this.cbCategories_SelectedIndexChanged);
			this.Name = "RibbonPopupItemsEditor";
			this.Size = new System.Drawing.Size(740, 475);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			this.panelControl2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbMenus.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbCategories.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public override void InitComponent() {
			base.InitComponent();
			InitializeDesigner();
		}
		protected virtual void InitializeDesigner() {
			lbMenuDesigner.Editor = this;
			lbItems.Manager = Manager;
			lbItems.Manager.Items.CollectionChanged += OnManagerItemsChanged;
			if(SelectionService != null)
				SelectionService.SelectionChanged += OnSelectionObjectChanged;
			lbItems.DestinationDropControl = lbMenuDesigner;
			InitMenus();
			cbCategories.Manager = Manager;
			cbCategories.InitCategories(-1);
			lbItems.FillListBox(cbCategories.SelCategory, 0);
		}
		void OnSelectionObjectChanged(object sender, EventArgs e) {
			object selectedObject = SelectionService.PrimarySelection;
			if(!IsPopupBarSubItemElement(selectedObject))
				return;
			pgMain.SelectedObject = selectedObject;
		}
		bool IsPopupBarSubItemElement(object selectedObject) {
			BarItem item = selectedObject as BarItem;
			if(item == null)
				return false;
			BarLinkInfoProvider provider = item.GetBarLinkInfoProvider();
			if(provider == null)
				return false;
			BarItemLink link = provider.Link;
			return link != null && link.LinkedObject is BarSubItem;		 
		}
		void OnManagerItemsChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Refresh && e.Element != null) {
				lbItems.Invalidate();
				return;
			}
			lbItems.FillListBox(cbCategories.SelCategory, lbItems.SelectedIndex);
		}
		protected virtual void InitMenus() {
			IContainer container = GetDesignerContainer();
			if(container == null) return;
			cbMenus.Properties.Items.BeginUpdate();
			try {
				cbMenus.Properties.Items.Clear();
				cbMenus.Properties.Items.Add(new ImageComboBoxItem("[Add new PopupMenu]", 1, -1));
				lbMenuDesigner.Visible = false;
				foreach(object obj in container.Components) {
					if(obj is BarButtonGroup) continue;
					BarLinksHolder holder = obj as BarLinksHolder;
					if(holder != null && holder.Manager == Manager) {
						IComponent component = holder as IComponent;
						int imageIndex = holder is PopupMenu ? 1 : 0;
						cbMenus.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(component.Site == null ? component.ToString() : component.Site.Name, component, imageIndex));
					}
				}			
			}
			finally {
				cbMenus.Properties.Items.EndUpdate();
			}
			if(cbMenus.Properties.Items.Count == 1)
				cbMenus.EditValue = null;
			else {
				SelectMenu();
			}
			UpdateDeleteMenuButton();
		}
		protected virtual void SelectMenu() {
			int menuIndex = cbMenus.Properties.Items.Count;
			if(Parent == null || Parent.Parent == null || Parent.Parent.Parent == null) return;
			RibbonEditorForm rf = Parent.Parent.Parent as RibbonEditorForm;
			if(rf.IsPopupMenu()) {
				for(menuIndex = 0; menuIndex < cbMenus.Properties.Items.Count; menuIndex++) {
					if(cbMenus.Properties.Items[menuIndex].Value == rf.ComponentObj) {
						cbMenus.SelectedIndex = menuIndex;
						break;
					}
				}
			}
			if(menuIndex == cbMenus.Properties.Items.Count) cbMenus.SelectedIndex = 1;
		}
		IDesignerHost GetHost() { return GetDesignerService(typeof(IDesignerHost)) as IDesignerHost; }
		object GetDesignerService(Type serviceType) {
			if(Ribbon == null || Ribbon.Site == null) return null;
			return Ribbon.Site.GetService(serviceType);
		}
		ISelectionService selectionServiceCore = null;
		protected ISelectionService SelectionService {
			get {
				if(selectionServiceCore == null)
					selectionServiceCore = GetDesignerService(typeof(ISelectionService)) as ISelectionService;
				return selectionServiceCore;
			}
		}
		IContainer GetDesignerContainer() {
			if(GetHost() == null) return null;
			return GetHost().Container;
		}
		protected override void OnParentChanged(EventArgs e) {
			SelectMenu();
		}
		private void cbCategories_SelectedIndexChanged(object sender, EventArgs e) {
			cbCategories.InitCategories(cbCategories.SelectedIndex);
			lbItems.FillListBox(cbCategories.SelCategory, cbCategories.SelectedIndex);
		}
		private void cbMenus_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateDeleteMenuButton();
			BarLinksHolder holder  = null;
			ImageComboBoxItem item = cbMenus.SelectedItem as ImageComboBoxItem;
			if(item == null)
				return;
			if(item.Value is int) {
				CreateNewPopupMenu();
				return;
			}
			holder = item.Value as BarLinksHolder;
			lbMenuDesigner.Visible = true;
			lbMenuDesigner.Setup(Manager, holder);
		}
		void CreateNewPopupMenu() {
			IDesignerHost host = GetHost();
			if(host != null && host.Container != null) {
				PopupMenu menu = new PopupMenu();
				menu.Ribbon = Ribbon;
				host.Container.Add(menu);
				InitMenus();
				cbMenus.SelectedIndex = FindItem(menu);
			} 
			if(cbMenus.SelectedIndex == 0) cbMenus.EditValue = null;
		}
		PopupMenu SelectedMenu {
			get {
				if(cbMenus.SelectedItem == null) return null;
				ImageComboBoxItem item = cbMenus.SelectedItem as ImageComboBoxItem;
				if(item == null) return null;
				return item.Value as PopupMenu;
			}
		}
		void UpdateDeleteMenuButton() {
			if(SelectedMenu == null)
				cbMenus.Properties.Buttons[2].Enabled = false;
			else
				cbMenus.Properties.Buttons[2].Enabled = true;
		}
		void DeletePopupMenu() {
			if(MessageBox.Show(this.FindForm(), "Are you sure you want to delete the popup menu?", "Ribbon Control Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) 
				return;
			IDesignerHost host = GetHost();
			if(host != null && host.Container != null && SelectedMenu != null) {
				host.Container.Remove(SelectedMenu);
				InitMenus();
			}
		}
		int FindItem(object val) {
			for(int n = 0; n < cbMenus.Properties.Items.Count; n++) {
				if(cbMenus.Properties.Items[n].Value == val) return n;
			}
			return -1;
		}
		private void lbItems_SelectedIndexChanged(object sender, EventArgs e) {
			if(!(lbItems.SelectedItem is IComponent)) return;
			pgMain.SelectedObject = lbItems.SelectedItem;
		}
		private void cbMenus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			if(e.Button.Kind == ButtonPredefines.Plus) CreateNewPopupMenu();
			if(e.Button.Kind == ButtonPredefines.Delete) DeletePopupMenu();
		}
	}
	[ToolboxItem(false)]
	internal class PopupItemLinksControl : DevExpress.XtraBars.Customization.BarItemLinksControl {
		RibbonPopupItemsEditor editor;
		public RibbonPopupItemsEditor Editor {
			get { return editor; }
			set { editor = value; }
		}
		protected override void OnSelectLink(BarItemLink link) {
			base.OnSelectLink(link);
			if(Editor != null)
				Editor.pgMain.SelectedObject = link.Item;
		}
	}
}
