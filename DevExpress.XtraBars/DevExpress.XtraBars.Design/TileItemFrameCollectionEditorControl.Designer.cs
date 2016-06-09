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

using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Design {
	partial class TileItemFrameEditorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.listControl = new DevExpress.XtraEditors.TileControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnAddInfo = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemoveInfo = new DevExpress.XtraEditors.SimpleButton();
			this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.previewControl = new DevExpress.XtraEditors.TileControl();
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
			this.splitContainerControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			this.SuspendLayout();
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(6, 6);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
			this.splitContainerControl1.Panel1.Controls.Add(this.panel1);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(763, 469);
			this.splitContainerControl1.SplitterPosition = 414;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.groupControl1.Controls.Add(this.listControl);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(414, 438);
			this.groupControl1.TabIndex = 1;
			this.groupControl1.Text = "Frame List";
			this.listControl.AllowItemHover = true;
			this.listControl.AllowSelectedItem = true;
			this.listControl.ColumnCount = 1;
			this.listControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listControl.Location = new System.Drawing.Point(2, 21);
			this.listControl.Name = "listControl";
			this.listControl.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.listControl.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollBar;
			this.listControl.Size = new System.Drawing.Size(410, 415);
			this.listControl.TabIndex = 0;
			this.listControl.Text = "tileControl1";
			this.listControl.EndItemDragging += new DevExpress.XtraEditors.TileItemDragEventHandler(this.listControl_EndItemDragging);
			this.listControl.SelectedItemChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.listControl_SelectedItemChanged);
			this.panel1.Controls.Add(this.btnAddInfo);
			this.panel1.Controls.Add(this.btnRemoveInfo);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 438);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(414, 31);
			this.panel1.TabIndex = 2;
			this.btnAddInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddInfo.Location = new System.Drawing.Point(255, 5);
			this.btnAddInfo.Name = "btnAddInfo";
			this.btnAddInfo.Size = new System.Drawing.Size(75, 23);
			this.btnAddInfo.TabIndex = 1;
			this.btnAddInfo.Text = "Add";
			this.btnAddInfo.Click += new System.EventHandler(this.btnAddInfo_Click);
			this.btnRemoveInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveInfo.Enabled = false;
			this.btnRemoveInfo.Location = new System.Drawing.Point(336, 5);
			this.btnRemoveInfo.Name = "btnRemoveInfo";
			this.btnRemoveInfo.Size = new System.Drawing.Size(75, 23);
			this.btnRemoveInfo.TabIndex = 0;
			this.btnRemoveInfo.Text = "Remove";
			this.btnRemoveInfo.Click += new System.EventHandler(this.btnRemoveInfo_Click);
			this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl2.Horizontal = false;
			this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl2.Name = "splitContainerControl2";
			this.splitContainerControl2.Panel1.Controls.Add(this.groupControl2);
			this.splitContainerControl2.Panel1.Text = "Panel1";
			this.splitContainerControl2.Panel2.Controls.Add(this.groupControl3);
			this.splitContainerControl2.Panel2.Text = "Panel2";
			this.splitContainerControl2.Size = new System.Drawing.Size(344, 469);
			this.splitContainerControl2.SplitterPosition = 238;
			this.splitContainerControl2.TabIndex = 0;
			this.splitContainerControl2.Text = "splitContainerControl2";
			this.groupControl2.Controls.Add(this.previewControl);
			this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl2.Location = new System.Drawing.Point(0, 0);
			this.groupControl2.Name = "groupControl2";
			this.groupControl2.Size = new System.Drawing.Size(344, 238);
			this.groupControl2.TabIndex = 1;
			this.groupControl2.Text = "Preview";
			this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewControl.Location = new System.Drawing.Point(2, 21);
			this.previewControl.Name = "previewControl";
			this.previewControl.Size = new System.Drawing.Size(340, 215);
			this.previewControl.TabIndex = 0;
			this.previewControl.Text = "tileControl2";
			this.previewControl.StartItemDragging += new DevExpress.XtraEditors.TileItemDragEventHandler(this.previewControl_StartItemDragging);
			this.groupControl3.Controls.Add(this.propertyGrid1);
			this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl3.Location = new System.Drawing.Point(0, 0);
			this.groupControl3.Name = "groupControl3";
			this.groupControl3.Size = new System.Drawing.Size(344, 226);
			this.groupControl3.TabIndex = 0;
			this.groupControl3.Text = "Properties";
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(2, 21);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(340, 203);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerControl1);
			this.Name = "TileItemFrameEditorControl";
			this.Padding = new System.Windows.Forms.Padding(6);
			this.Size = new System.Drawing.Size(775, 481);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
			this.splitContainerControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private SplitContainerControl splitContainerControl1;
		private DevExpress.XtraEditors.TileControl listControl;
		private SplitContainerControl splitContainerControl2;
		private DevExpress.XtraEditors.TileControl previewControl;
		private GroupControl groupControl1;
		private System.Windows.Forms.Panel panel1;
		private SimpleButton btnRemoveInfo;
		private SimpleButton btnAddInfo;
		private GroupControl groupControl2;
		private GroupControl groupControl3;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
	}
}
