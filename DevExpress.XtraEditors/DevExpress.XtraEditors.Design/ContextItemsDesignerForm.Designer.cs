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
using System.Windows.Forms;
namespace DevExpress.XtraEditors.Design {
	partial class ContextItemsDesignerForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContextItemsDesignerForm));
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.pePreview = new ContextButtonsPreviewControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.hyperlinkLabelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lbItems = new DevExpress.XtraEditors.ImageListBoxControl();
			this.lbObjectName = new DevExpress.XtraEditors.LabelControl();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.lbClassName = new DevExpress.XtraEditors.LabelControl();
			this.cbItems = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.chkShowItems = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbItems.Properties)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItems.Properties)).BeginInit();
			this.SuspendLayout();
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(358, 29);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(379, 275);
			this.propertyGrid1.TabIndex = 8;
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Horizontal = false;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.chkShowItems);
			this.splitContainerControl1.Panel1.Controls.Add(this.pePreview);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.labelControl1);
			this.splitContainerControl1.Panel2.Controls.Add(this.hyperlinkLabelControl1);
			this.splitContainerControl1.Panel2.Controls.Add(this.lbItems);
			this.splitContainerControl1.Panel2.Controls.Add(this.lbObjectName);
			this.splitContainerControl1.Panel2.Controls.Add(this.btnAdd);
			this.splitContainerControl1.Panel2.Controls.Add(this.lbClassName);
			this.splitContainerControl1.Panel2.Controls.Add(this.cbItems);
			this.splitContainerControl1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainerControl1.Panel2.Controls.Add(this.btnUp);
			this.splitContainerControl1.Panel2.Controls.Add(this.btnDelete);
			this.splitContainerControl1.Panel2.Controls.Add(this.btnDown);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(749, 551);
			this.splitContainerControl1.SplitterPosition = 236;
			this.splitContainerControl1.TabIndex = 13;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.pePreview.Location = new System.Drawing.Point(232, 36);
			this.pePreview.Name = "pePreview";
			this.pePreview.Size = new System.Drawing.Size(248, 120);
			this.pePreview.TabIndex = 0;
			this.labelControl1.Location = new System.Drawing.Point(12, 12);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(158, 13);
			this.labelControl1.TabIndex = 4;
			this.labelControl1.Text = "Select item type and click Add:";
			this.hyperlinkLabelControl1.Location = new System.Drawing.Point(12, 70);
			this.hyperlinkLabelControl1.Name = "hyperlinkLabelControl1";
			this.hyperlinkLabelControl1.Size = new System.Drawing.Size(47, 13);
			this.hyperlinkLabelControl1.TabIndex = 0;
			this.hyperlinkLabelControl1.Text = "Buttons:";
			this.lbItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			this.lbItems.Location = new System.Drawing.Point(12, 89);
			this.lbItems.Name = "lbItems";
			this.lbItems.Size = new System.Drawing.Size(306, 215);
			this.lbItems.TabIndex = 1;
			this.lbItems.SelectedValueChanged += new System.EventHandler(this.lbItems_SelectedValueChanged);
			this.lbObjectName.Location = new System.Drawing.Point(500, 10);
			this.lbObjectName.Name = "lbObjectName";
			this.lbObjectName.Size = new System.Drawing.Size(62, 13);
			this.lbObjectName.TabIndex = 10;
			this.lbObjectName.Text = "Button Name";
			this.btnAdd.Location = new System.Drawing.Point(232, 29);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(86, 23);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.lbClassName.AllowHtmlString = true;
			this.lbClassName.Location = new System.Drawing.Point(358, 10);
			this.lbClassName.Name = "lbClassName";
			this.lbClassName.Size = new System.Drawing.Size(64, 13);
			this.lbClassName.TabIndex = 9;
			this.lbClassName.Text = "<b>Class Name</b>";
			this.cbItems.Location = new System.Drawing.Point(12, 31);
			this.cbItems.Name = "cbItems";
			this.cbItems.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbItems.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbItems.Size = new System.Drawing.Size(214, 20);
			this.cbItems.TabIndex = 3;
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnUp.Location = new System.Drawing.Point(324, 174);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(28, 23);
			this.btnUp.TabIndex = 5;
			this.btnUp.Click += new EventHandler(btnUp_Click);
			this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
			this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDelete.Location = new System.Drawing.Point(324, 232);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(28, 23);
			this.btnDelete.TabIndex = 7;
			this.btnDelete.Click += new EventHandler(btnDelete_Click);
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDown.Location = new System.Drawing.Point(324, 203);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(28, 23);
			this.btnDown.TabIndex = 6;
			this.btnDown.Click += new EventHandler(btnDown_Click);
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOk);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 551);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(749, 52);
			this.panel1.TabIndex = 14;
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(651, 15);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(86, 23);
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(559, 15);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(86, 23);
			this.btnOk.TabIndex = 12;
			this.btnOk.Text = "&OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.chkShowItems.Location = new System.Drawing.Point(12, 12);
			this.chkShowItems.Name = "chkShowItems";
			this.chkShowItems.Properties.Caption = "Show Items";
			this.chkShowItems.Size = new System.Drawing.Size(75, 19);
			this.chkShowItems.TabIndex = 1;
			this.chkShowItems.CheckedChanged += new System.EventHandler(this.chkShowItems_CheckedChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(749, 603);
			this.Controls.Add(this.splitContainerControl1);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContextItemsDesignerForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Context Buttons";
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbItems.Properties)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkShowItems.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private LabelControl hyperlinkLabelControl1;
		private ImageListBoxControl lbItems;
		private SimpleButton btnAdd;
		private ComboBoxEdit cbItems;
		private LabelControl labelControl1;
		private SimpleButton btnUp;
		private SimpleButton btnDown;
		private SimpleButton btnDelete;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private LabelControl lbClassName;
		private LabelControl lbObjectName;
		private SimpleButton btnCancel;
		private SimpleButton btnOk;
		private SplitContainerControl splitContainerControl1;
		private System.Windows.Forms.Panel panel1;
		private ContextButtonsPreviewControl pePreview;
		private CheckEdit chkShowItems;
	}
}
