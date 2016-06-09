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

namespace DevExpress.Utils.Design.Taskbar {
	partial class JumpListEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JumpListEditorForm));
			this.treeView = new System.Windows.Forms.TreeView();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.btnAddTask = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.btnDownCmd = new DevExpress.XtraEditors.SimpleButton();
			this.btnUpCmd = new DevExpress.XtraEditors.SimpleButton();
			this.lblTasks = new DevExpress.XtraEditors.LabelControl();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
			this.splitContainerControl.SuspendLayout();
			this.SuspendLayout();
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.HideSelection = false;
			this.treeView.Location = new System.Drawing.Point(0, 26);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(195, 263);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeViewAfterSelect);
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.HelpVisible = false;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(279, 348);
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnPropertyGridValueChanged);
			this.propertyGrid.SelectedObjectsChanged += new System.EventHandler(this.OnSelectedObjectsChanged);
			this.btnAddTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddTask.Location = new System.Drawing.Point(0, 295);
			this.btnAddTask.Name = "btnAddTask";
			this.btnAddTask.Size = new System.Drawing.Size(95, 23);
			this.btnAddTask.TabIndex = 2;
			this.btnAddTask.Text = "Add Task";
			this.btnAddTask.Click += new System.EventHandler(this.OnBtnAddTaskClick);
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
			this.btnRemove.Location = new System.Drawing.Point(101, 324);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(95, 23);
			this.btnRemove.TabIndex = 5;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.OnBtnRemoveClick);
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(101, 295);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(95, 23);
			this.btnAdd.TabIndex = 3;
			this.btnAdd.Text = "Add";
			this.splitContainerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerControl.Location = new System.Drawing.Point(12, 12);
			this.splitContainerControl.Name = "splitContainerControl";
			this.splitContainerControl.Panel1.Controls.Add(this.btnDownCmd);
			this.splitContainerControl.Panel1.Controls.Add(this.btnUpCmd);
			this.splitContainerControl.Panel1.Controls.Add(this.lblTasks);
			this.splitContainerControl.Panel1.Controls.Add(this.treeView);
			this.splitContainerControl.Panel1.Controls.Add(this.btnAddTask);
			this.splitContainerControl.Panel1.Controls.Add(this.btnRemove);
			this.splitContainerControl.Panel1.Controls.Add(this.btnAdd);
			this.splitContainerControl.Panel1.MinSize = 226;
			this.splitContainerControl.Panel1.Text = "PanelTreeList";
			this.splitContainerControl.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainerControl.Panel2.MinSize = 100;
			this.splitContainerControl.Panel2.Text = "PanelPropertyGrid";
			this.splitContainerControl.Size = new System.Drawing.Size(510, 348);
			this.splitContainerControl.SplitterPosition = 226;
			this.splitContainerControl.TabIndex = 11;
			this.splitContainerControl.Text = "splitContainerControl1";
			this.btnDownCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDownCmd.Image = ((System.Drawing.Image)(resources.GetObject("btnDownCmd.Image")));
			this.btnDownCmd.ImageIndex = 0;
			this.btnDownCmd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDownCmd.Location = new System.Drawing.Point(197, 83);
			this.btnDownCmd.Name = "btnDownCmd";
			this.btnDownCmd.Size = new System.Drawing.Size(24, 34);
			this.btnDownCmd.TabIndex = 8;
			this.btnDownCmd.Click += new System.EventHandler(this.OnBtnDownCmdClick);
			this.btnUpCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpCmd.Image = ((System.Drawing.Image)(resources.GetObject("btnUpCmd.Image")));
			this.btnUpCmd.ImageIndex = 1;
			this.btnUpCmd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnUpCmd.Location = new System.Drawing.Point(197, 26);
			this.btnUpCmd.Name = "btnUpCmd";
			this.btnUpCmd.Size = new System.Drawing.Size(24, 34);
			this.btnUpCmd.TabIndex = 7;
			this.btnUpCmd.Click += new System.EventHandler(this.OnBtnUpCmdClick);
			this.lblTasks.Location = new System.Drawing.Point(0, 7);
			this.lblTasks.Name = "lblTasks";
			this.lblTasks.Size = new System.Drawing.Size(27, 13);
			this.lblTasks.TabIndex = 1;
			this.lblTasks.Text = "Tasks";
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(366, 372);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(447, 372);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.OnBtnCancelClick);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 407);
			this.Controls.Add(this.splitContainerControl);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(550, 440);
			this.Name = "JumpListEditorForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Jump List Editor";
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
			this.splitContainerControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private XtraEditors.SimpleButton btnAddTask;
		private XtraEditors.SimpleButton btnRemove;
		private XtraEditors.SimpleButton btnAdd;
		private XtraEditors.SplitContainerControl splitContainerControl;
		private XtraEditors.SimpleButton btnDownCmd;
		private XtraEditors.SimpleButton btnUpCmd;
		private XtraEditors.LabelControl lblTasks;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
	}
}
