#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.ComponentModel.Design;
using System;
namespace DevExpress.ExpressApp.Design {
	partial class XafModuleRootDesignerGUI {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(designer != null) {
					try {
						designer.SelectionService.SelectionChanged -= new EventHandler(SelectionService_SelectionChanged);
					}
					catch { }
				}
				FocusChanged = null;
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Module", 0);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XafModuleRootDesignerGUI));
			this.controllersTreeView = new DevExpress.ExpressApp.Design.ControllersTreeViewTray(this.components);
			this.itemsImageList = new System.Windows.Forms.ImageList(this.components);
			this.businessClassesTreeView = new DevExpress.ExpressApp.Design.BusinessClassesTreeViewTray(this.components);
			this.modulesTreeView = new DevExpress.ExpressApp.Design.ModuleTypesTreeViewTray(this.components);
			this.moduleListView = new System.Windows.Forms.ListView();
			this.moduleImageList = new System.Windows.Forms.ImageList(this.components);
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.requiredModulesHeaderPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox4.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.requiredModulesHeaderPanel.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			this.controllersTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controllersTreeView.ImageIndex = 0;
			this.controllersTreeView.ImageList = this.itemsImageList;
			this.controllersTreeView.Location = new System.Drawing.Point(3, 24);
			this.controllersTreeView.Name = "controllersTreeView";
			this.controllersTreeView.SelectedImageIndex = 0;
			this.controllersTreeView.SelectedObject = null;
			this.controllersTreeView.Size = new System.Drawing.Size(222, 411);
			this.controllersTreeView.TabIndex = 2;
			this.itemsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.itemsImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.itemsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.businessClassesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.businessClassesTreeView.ImageIndex = 0;
			this.businessClassesTreeView.ImageList = this.itemsImageList;
			this.businessClassesTreeView.Location = new System.Drawing.Point(3, 24);
			this.businessClassesTreeView.Name = "objectsTreeView";
			this.businessClassesTreeView.SelectedImageIndex = 0;
			this.businessClassesTreeView.SelectedObject = null;
			this.businessClassesTreeView.Size = new System.Drawing.Size(231, 411);
			this.businessClassesTreeView.TabIndex = 1;
			this.modulesTreeView.AllowDrop = true;
			this.modulesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modulesTreeView.ImageIndex = 0;
			this.modulesTreeView.ImageList = this.itemsImageList;
			this.modulesTreeView.Location = new System.Drawing.Point(0, 24);
			this.modulesTreeView.Name = "modulesTreeView";
			this.modulesTreeView.SelectedImageIndex = 0;
			this.modulesTreeView.SelectedObject = null;
			this.modulesTreeView.ShowNodeToolTips = true;
			this.modulesTreeView.Size = new System.Drawing.Size(220, 411);
			this.modulesTreeView.TabIndex = 1;
			this.modulesTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.modulesTreeView_DragDrop);
			this.moduleListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.moduleListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.moduleListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem1});
			this.moduleListView.LabelEdit = true;
			this.moduleListView.LabelWrap = false;
			this.moduleListView.LargeImageList = this.moduleImageList;
			this.moduleListView.Location = new System.Drawing.Point(3, 13);
			this.moduleListView.Name = "moduleListView";
			this.moduleListView.Size = new System.Drawing.Size(687, 56);
			this.moduleListView.TabIndex = 1;
			this.moduleListView.UseCompatibleStateImageBehavior = false;
			this.moduleListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.moduleListView_MouseClick);
			this.moduleListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.moduleListView_AfterLabelEdit);
			this.moduleListView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.moduleListView_BeforeLabelEdit);
			this.moduleImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("moduleImageList.ImageStream")));
			this.moduleImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.moduleImageList.Images.SetKeyName(0, "Module.ico");
			this.groupBox4.Controls.Add(this.moduleListView);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(8, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 0, 3, 5);
			this.groupBox4.Size = new System.Drawing.Size(693, 74);
			this.groupBox4.TabIndex = 2;
			this.groupBox4.TabStop = false;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(8, 6, 8, 3);
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
			this.splitContainer1.Size = new System.Drawing.Size(709, 530);
			this.splitContainer1.SplitterDistance = 83;
			this.splitContainer1.TabIndex = 5;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(8, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.Controls.Add(this.modulesTreeView);
			this.splitContainer2.Panel1.Controls.Add(this.requiredModulesHeaderPanel);
			this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.splitContainer2.Panel1MinSize = 154;
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Panel2MinSize = 100;
			this.splitContainer2.Size = new System.Drawing.Size(693, 435);
			this.splitContainer2.SplitterDistance = 223;
			this.splitContainer2.TabIndex = 0;
			this.requiredModulesHeaderPanel.Controls.Add(this.label1);
			this.requiredModulesHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.requiredModulesHeaderPanel.Location = new System.Drawing.Point(0, 0);
			this.requiredModulesHeaderPanel.Name = "requiredModulesHeaderPanel";
			this.requiredModulesHeaderPanel.Size = new System.Drawing.Size(220, 24);
			this.requiredModulesHeaderPanel.TabIndex = 2;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Required Modules:";
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Panel1.Controls.Add(this.controllersTreeView);
			this.splitContainer3.Panel1.Controls.Add(this.panel1);
			this.splitContainer3.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.splitContainer3.Panel1MinSize = 50;
			this.splitContainer3.Panel2.Controls.Add(this.businessClassesTreeView);
			this.splitContainer3.Panel2.Controls.Add(this.panel2);
			this.splitContainer3.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.splitContainer3.Panel2MinSize = 50;
			this.splitContainer3.Size = new System.Drawing.Size(466, 435);
			this.splitContainer3.SplitterDistance = 228;
			this.splitContainer3.TabIndex = 0;
			this.panel1.Controls.Add(this.label2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(222, 24);
			this.panel1.TabIndex = 3;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Controllers:";
			this.panel2.Controls.Add(this.label3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(231, 24);
			this.panel2.TabIndex = 3;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Exported Types:";
			this.Controls.Add(this.splitContainer1);
			this.Name = "XafModuleDesignerGUI";
			this.Size = new System.Drawing.Size(709, 530);
			this.groupBox4.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.requiredModulesHeaderPanel.ResumeLayout(false);
			this.requiredModulesHeaderPanel.PerformLayout();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private BusinessClassesTreeViewTray businessClassesTreeView;
		private ModuleTypesTreeViewTray modulesTreeView;
		private System.Windows.Forms.ListView moduleListView;
		private System.Windows.Forms.ImageList moduleImageList;
		private ControllersTreeViewTray controllersTreeView;
		private System.Windows.Forms.ImageList itemsImageList;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Panel requiredModulesHeaderPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label3;
	}
}
