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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.Utils.Frames;
using DevExpress.XtraTab;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Docking.Design {
	public class DockCustomizationForm : DevExpress.XtraEditors.XtraForm {
		private DevExpress.XtraTab.XtraTabControl tabControl1;
		private DevExpress.XtraTab.XtraTabPage tabPage1;
		private DevExpress.XtraEditors.SimpleButton sbClose;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel splitter1;
		private System.Windows.Forms.Panel splitter2;
		private DevExpress.Utils.Frames.PropertyGridEx pgMain;
		DockManager manager;
		private DevExpress.XtraTab.XtraTabPage tabPage2;
		private DevExpress.XtraEditors.SimpleButton sbDelete;
		private DevExpress.XtraEditors.CheckedListBoxControl clbList;
		private System.Windows.Forms.Label label1;
		DockPanels panels;
		public DockCustomizationForm() : this(null) {}
		public DockCustomizationForm(DockManager manager) {
			this.LookAndFeel.SetSkinStyle(SkinRegistrator.DesignTimeSkinName);
			InitializeComponent();
			pgMain.SelectedObjectsChanged += new EventHandler(OnPropertyGridSelectedObjectChanged);
			pgMain.CommandsVisibleIfAvailable = false;
			pgMain.HelpVisible = true;
			pgMain.DrawFlat = true;
			Manager = manager;
			panels = new DockPanels(Manager, "DockPanels:", pgMain);
			panels.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			InitDockContainers();
			tabPage1.Controls.Add(panels);
			splitter1.BackColor = splitter2.BackColor = panel1.BackColor = this.BackColor;
		}
		void OnPropertyGridSelectedObjectChanged(object sender, EventArgs e) {
			UpdatePropertyGridSite();
			pgMain.ShowEvents(true);
		}
		IServiceProvider GetPropertyGridServiceProvider() {
			object selObject = null;
			if(pgMain.SelectedObjects != null && pgMain.SelectedObjects.Length > 0) 
				selObject = pgMain.SelectedObjects[0];
			else selObject = pgMain.SelectedObject;
			if(selObject is Component) {
				return (selObject as Component).Site;
			}
			return null;
		}
		void UpdatePropertyGridSite() {
			pgMain.Site = null;
			IServiceProvider provider = GetPropertyGridServiceProvider();
			if(provider != null) {
				pgMain.Site = new DevExpress.XtraEditors.Designer.Utils.XtraPGFrame.MySite(provider, pgMain as IComponent);
				pgMain.PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab));
			}
		}
		public DockManager Manager {
			get { return manager; }
			set {
				if(Manager == value) return;
				if(Manager != null) {
					Manager.RegisterDockPanel -= new DockPanelEventHandler(Manager_RegisterDockPanel);
					Manager.UnregisterDockPanel -= new DockPanelEventHandler(Manager_UnregisterDockPanel);
					Manager.VisibilityChanged -= new VisibilityChangedEventHandler(Manager_VisibilityChanged);
				}
				this.manager = value;
				if(Manager != null) {
					Manager.RegisterDockPanel += new DockPanelEventHandler(Manager_RegisterDockPanel);
					Manager.UnregisterDockPanel += new DockPanelEventHandler(Manager_UnregisterDockPanel);
					Manager.VisibilityChanged += new VisibilityChangedEventHandler(Manager_VisibilityChanged);
				}
				if(panels != null)
					panels.Manager = value;
			}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing  && !IsDisposed) {
				if(components != null) {
					components.Dispose();
				}
				Manager = null;
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.tabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.tabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.tabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.sbDelete = new DevExpress.XtraEditors.SimpleButton();
			this.clbList = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.label1 = new System.Windows.Forms.Label();
			this.sbClose = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Panel();
			this.pgMain = new DevExpress.Utils.Frames.PropertyGridEx();
			((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clbList)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabControl1.Location = new System.Drawing.Point(11, 11);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedTabPage = this.tabPage1;
			this.tabControl1.Size = new System.Drawing.Size(360, 267);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabPage1,
			this.tabPage2});
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(351, 236);
			this.tabPage1.Text = "Dock Panels";
			this.tabPage2.Controls.Add(this.sbDelete);
			this.tabPage2.Controls.Add(this.clbList);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(351, 236);
			this.tabPage2.Text = "Dock Containers";
			this.sbDelete.Enabled = false;
			this.sbDelete.Location = new System.Drawing.Point(232, 32);
			this.sbDelete.Name = "sbDelete";
			this.sbDelete.Size = new System.Drawing.Size(112, 28);
			this.sbDelete.TabIndex = 10;
			this.sbDelete.Text = "Delete";
			this.sbDelete.Click += new System.EventHandler(this.sbDelete_Click);
			this.clbList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.clbList.ItemHeight = 16;
			this.clbList.Location = new System.Drawing.Point(8, 28);
			this.clbList.Name = "clbList";
			this.clbList.Size = new System.Drawing.Size(216, 199);
			this.clbList.TabIndex = 9;
			this.clbList.SelectedIndexChanged += new System.EventHandler(this.clbList_SelectedIndexChanged);
			this.clbList.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.clbList_ItemCheck);
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 19);
			this.label1.TabIndex = 8;
			this.label1.Text = "DockContainers:";
			this.sbClose.Dock = System.Windows.Forms.DockStyle.Right;
			this.sbClose.Location = new System.Drawing.Point(578, 0);
			this.sbClose.Name = "sbClose";
			this.sbClose.Size = new System.Drawing.Size(96, 28);
			this.sbClose.TabIndex = 3;
			this.sbClose.Text = "Close";
			this.sbClose.Click += new System.EventHandler(this.sbClose_Click);
			this.panel1.Controls.Add(this.sbClose);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(11, 285);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(674, 28);
			this.panel1.TabIndex = 4;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Left;
			this.splitter1.Enabled = false;
			this.splitter1.Location = new System.Drawing.Point(371, 11);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(10, 267);
			this.splitter1.TabIndex = 5;
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Enabled = false;
			this.splitter2.Location = new System.Drawing.Point(11, 278);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(674, 7);
			this.splitter2.TabIndex = 6;
			this.pgMain.BackColor = System.Drawing.SystemColors.Control;
			this.pgMain.CommandsBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
			this.pgMain.CommandsForeColor = System.Drawing.Color.Black;
			this.pgMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgMain.DrawFlat = false;
			this.pgMain.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgMain.Location = new System.Drawing.Point(381, 11);
			this.pgMain.Name = "pgMain";
			this.pgMain.Size = new System.Drawing.Size(304, 267);
			this.pgMain.TabIndex = 7;
			this.pgMain.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgMain_PropertyValueChanged);
			this.ClientSize = new System.Drawing.Size(696, 324);
			this.Controls.Add(this.pgMain);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(692, 358);
			this.Name = "DockCustomizationForm";
			this.Padding = new System.Windows.Forms.Padding(11);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DockPanels Customization";
			((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clbList)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private void sbClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
		DockPanel SelectedPanel {
			get { 
				if(clbList.SelectedItem == null) return null;
				return Manager[clbList.SelectedValue.ToString()]; 
			}
		}
		void Manager_VisibilityChanged(object sender, DevExpress.XtraBars.Docking.VisibilityChangedEventArgs e) {
			if(checkUpdate) return;
			for(int i = 0; i < clbList.ItemCount; i++)
				if(e.Panel.Name.Equals(((CheckedListBoxItem)clbList.Items[i]).Value)) {
					((CheckedListBoxItem)clbList.Items[i]).CheckState = (e.Panel.Visibility != DockVisibility.Hidden ? CheckState.Checked : CheckState.Unchecked);
				}
		}
		void Manager_RegisterDockPanel(object sender, DockPanelEventArgs e) {
			InitDockContainers(e.Panel);
		}
		void Manager_UnregisterDockPanel(object sender, DockPanelEventArgs e) {
			InitDockContainers(e.Panel);
			if(!panels.CheckUpdate)
				panels.InitDockList(0);
		}
		void InitDockContainers(DockPanel panel) {
			clbList.Items.BeginUpdate();
			try {
				InitDockContainers();
				int index = clbList.FindStringExact(panel.Name);
				if(index == -1 && clbList.ItemCount > 0) index = 0;
				clbList.SelectedIndex = index;
			}
			finally {
				clbList.Items.EndUpdate();
			}
		}
		void InitDockContainers() {
			clbList.Items.BeginUpdate();
			try {
				clbList.Items.Clear();
				for(int i = 0; i < Manager.Count; i++) {
					DockPanel panel = Manager[i];
					if(panel.Count == 0) continue;
					clbList.Items.Add(new CheckedListBoxItem(panel.Name, panel.Visibility != DockVisibility.Hidden));
				}
				clbList.SelectedIndex = clbList.ItemCount > 0 ? 0 : -1;
			}
			finally {
				clbList.Items.EndUpdate();
			}
		}
		private void sbDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(string.Format(DockConsts.DeleteContainerTextDialogFormatString, SelectedPanel.Name), DockConsts.DeleteContainerCaptionDialogString, 
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
			Manager.RemovePanel(SelectedPanel);
			clbList_SelectedIndexChanged(clbList, EventArgs.Empty);
		}
		void RefreshProperties() {
			if(tabControl1.SelectedTabPageIndex != 0)
				pgMain.SelectedObject = (clbList.SelectedIndex == -1 ? null : SelectedPanel);
		}
		private void clbList_SelectedIndexChanged(object sender, System.EventArgs e) {
			sbDelete.Enabled = clbList.ItemCount > 0;
			RefreshProperties();
		}
		bool checkUpdate = false;
		private void clbList_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			checkUpdate = true;
			if(SelectedPanel != null)
				SelectedPanel.Visibility = ( e.State == CheckState.Checked ? DockVisibility.Visible : DockVisibility.Hidden);	
			checkUpdate = false;
		}
		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(tabControl1.SelectedTabPageIndex == 0)
				panels.RefreshProperties();
			else RefreshProperties();
		}
		DevExpress.XtraEditors.CheckedListBoxControl ActiveListBox {
			get {
				if(tabControl1.SelectedTabPageIndex == 0)
					return panels.ActiveListBox;
				return clbList;
			}
		}
		private void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			if(ActiveListBox.SelectedItem == null) return;
			DockPanel panel = Manager[ActiveListBox.SelectedItem.ToString()];
			if(panel == null) 
				((CheckedListBoxItem)ActiveListBox.SelectedItem).Value = ((DockPanel)pgMain.SelectedObject).Name;
		}
	}
}
