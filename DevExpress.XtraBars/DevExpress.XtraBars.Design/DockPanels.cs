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
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Docking.Design {
	[ToolboxItem(false)]
	public class DockPanels : DevExpress.XtraEditors.XtraUserControl {
		private DevExpress.XtraEditors.SimpleButton sbDelete;
		private DevExpress.XtraEditors.SimpleButton sbNew;
		private DevExpress.XtraEditors.CheckedListBoxControl clbList;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.Container components = null;
		DockManager manager = null;
		public DockPanels() : this(null) {}
		public DockPanels(DockManager manager) : this(manager, "", null) {}
		public DockPanels(DockManager manager, string caption, PropertyGrid properties) {
			InitializeComponent();
			Properties = properties;
			Manager = manager;
			if(caption != "") label1.Text = caption;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing && !IsDisposed) {
				if(components != null) {
					components.Dispose();
				}
				Manager = null;
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.sbDelete = new DevExpress.XtraEditors.SimpleButton();
			this.sbNew = new DevExpress.XtraEditors.SimpleButton();
			this.clbList = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.clbList)).BeginInit();
			this.SuspendLayout();
			this.sbDelete.Enabled = false;
			this.sbDelete.Location = new System.Drawing.Point(232, 64);
			this.sbDelete.Name = "sbDelete";
			this.sbDelete.Size = new System.Drawing.Size(112, 28);
			this.sbDelete.TabIndex = 7;
			this.sbDelete.Text = "Delete";
			this.sbDelete.Click += new System.EventHandler(this.sbDelete_Click);
			this.sbNew.Location = new System.Drawing.Point(232, 28);
			this.sbNew.Name = "sbNew";
			this.sbNew.Size = new System.Drawing.Size(112, 28);
			this.sbNew.TabIndex = 6;
			this.sbNew.Text = "New";
			this.sbNew.Click += new System.EventHandler(this.sbNew_Click);
			this.clbList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.clbList.ItemHeight = 16;
			this.clbList.Location = new System.Drawing.Point(8, 28);
			this.clbList.Name = "clbList";
			this.clbList.Size = new System.Drawing.Size(216, 199);
			this.clbList.TabIndex = 5;
			this.clbList.SelectedIndexChanged += new System.EventHandler(this.clbList_SelectedIndexChanged);
			this.clbList.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.clbList_ItemCheck);
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "DockWindows:";
			this.Controls.Add(this.sbDelete);
			this.Controls.Add(this.sbNew);
			this.Controls.Add(this.clbList);
			this.Controls.Add(this.label1);
			this.Name = "DockPanels";
			this.Size = new System.Drawing.Size(356, 240);
			((System.ComponentModel.ISupportInitialize)(this.clbList)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public DockManager Manager {
			get { return manager; }
			set {
				if(Manager == value) return;
				if(Manager != null)
					Manager.VisibilityChanged -= new DevExpress.XtraBars.Docking.VisibilityChangedEventHandler(Manager_VisibilityChanged);
				this.manager = value;
				if(Manager != null)
					Manager.VisibilityChanged += new DevExpress.XtraBars.Docking.VisibilityChangedEventHandler(Manager_VisibilityChanged);
				InitDockList();
			}
		}
		void Manager_VisibilityChanged(object sender, DevExpress.XtraBars.Docking.VisibilityChangedEventArgs e) {
			if(checkUpdate) return;
			for(int i = 0; i < clbList.ItemCount; i++)
				if(e.Panel.Name.Equals(((CheckedListBoxItem)clbList.Items[i]).Value)) {
					((CheckedListBoxItem)clbList.Items[i]).CheckState = (e.Panel.Visibility != DockVisibility.Hidden ? CheckState.Checked : CheckState.Unchecked);
				}
		}
		DockPanel SelectedPanel {
			get { 
				if(clbList.SelectedItem == null) return null;
				return Manager[clbList.SelectedValue.ToString()]; 
			}
		}
		public void InitDockList() {
			InitDockList(0);
		}
		public void InitDockList(int index) {
			if(Manager == null) return;
			clbList.Items.BeginUpdate();
			try {
				clbList.Items.Clear();
				for(int i = 0; i < Manager.Count; i++) {
					DockPanel panel = Manager[i];
					if(panel.Count > 0) continue;
					clbList.Items.Add(new CheckedListBoxItem(panel.Name, panel.Visibility != DockVisibility.Hidden));
				}
				if(clbList.ItemCount == 0)
					index = -1;
				clbList.SelectedIndex = Math.Min(index, clbList.ItemCount - 1);
			}
			finally{
				clbList.Items.EndUpdate();
			}
		}	
		private void sbNew_Click(object sender, System.EventArgs e) {
			DockPanel panel = Manager.AddPanel(DockingStyle.Float);
			if(panel == null) return;
			SelectComponent(panel);
			InitDockList(-1);
			clbList.SelectedIndex = clbList.FindStringExact(panel.Name);
		}
		private void sbDelete_Click(object sender, System.EventArgs e) {
			if(SelectedPanel != null) {
				Manager.RemovePanel(SelectedPanel);
				InitDockList(clbList.SelectedIndex);
			}
		}
		void SelectComponent(Component component) {
			if(Manager == null || Manager.Site == null) return;
			ISelectionService selServ = Manager.Site.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selServ != null) {
				selServ.SetSelectedComponents(new object[] {component} );
			}
		}
		PropertyGrid properties = null;
		public PropertyGrid Properties {
			get { return properties; }
			set { properties = value; }
		}
		public DevExpress.XtraEditors.CheckedListBoxControl ActiveListBox { get { return clbList; }}
		public virtual void RefreshProperties() {
			if(properties != null)
				properties.SelectedObject = (clbList.SelectedIndex == -1 ? null : SelectedPanel);
		}
		void clbList_SelectedIndexChanged(object sender, System.EventArgs e) {
			sbDelete.Enabled = clbList.ItemCount > 0;
			RefreshProperties();
		}
		public bool CheckUpdate { get { return checkUpdate; }}
		bool checkUpdate = false;
		private void clbList_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			checkUpdate = true;
			if(SelectedPanel != null)
				SelectedPanel.Visibility = ( e.State == CheckState.Checked ? DockVisibility.Visible : DockVisibility.Hidden);	
			checkUpdate = false;
		}
	}
}
