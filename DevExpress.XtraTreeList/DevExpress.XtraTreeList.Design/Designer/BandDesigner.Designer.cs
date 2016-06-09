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

namespace DevExpress.XtraTreeList.Frames {
	partial class BandDesigner {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.ceAutoWidth = new DevExpress.XtraEditors.CheckEdit();
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btReset = new DevExpress.XtraEditors.SimpleButton();
			this.btDel = new DevExpress.XtraEditors.SimpleButton();
			this.chColumns = new DevExpress.XtraEditors.SimpleButton();
			this.treeListPreview = new DevExpress.XtraTreeList.TreeList();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeListPreview)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(167, 216);
			this.splMain.Size = new System.Drawing.Size(5, 264);
			this.pgMain.Location = new System.Drawing.Point(172, 216);
			this.pgMain.Size = new System.Drawing.Size(528, 264);
			this.pnlControl.Controls.Add(this.treeListPreview);
			this.pnlControl.Size = new System.Drawing.Size(700, 170);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(700, 42);
			this.pnlMain.Controls.Add(this.ceAutoWidth);
			this.pnlMain.Controls.Add(this.btAdd);
			this.pnlMain.Controls.Add(this.btReset);
			this.pnlMain.Controls.Add(this.btDel);
			this.pnlMain.Controls.Add(this.chColumns);
			this.pnlMain.Location = new System.Drawing.Point(0, 216);
			this.pnlMain.Size = new System.Drawing.Size(167, 264);
			this.horzSplitter.Size = new System.Drawing.Size(700, 4);
			this.ceAutoWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.ceAutoWidth.Location = new System.Drawing.Point(8, 112);
			this.ceAutoWidth.Name = "ceAutoWidth";
			this.ceAutoWidth.Properties.Caption = "Auto Width";
			this.ceAutoWidth.Size = new System.Drawing.Size(139, 19);
			this.ceAutoWidth.TabIndex = 8;
			this.ceAutoWidth.CheckedChanged += new System.EventHandler(this.ceAutoWidth_CheckedChanged);
			this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btAdd.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
			this.btAdd.Appearance.Options.UseFont = true;
			this.btAdd.Location = new System.Drawing.Point(8, 6);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(147, 30);
			this.btAdd.TabIndex = 5;
			this.btAdd.Text = "Add New Band...";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.btAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btAdd_MouseDown);
			this.btAdd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btAdd_MouseMove);
			this.btReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btReset.Location = new System.Drawing.Point(8, 221);
			this.btReset.Name = "btReset";
			this.btReset.Size = new System.Drawing.Size(147, 30);
			this.btReset.TabIndex = 9;
			this.btReset.Text = "&Reset";
			this.btReset.Click += new System.EventHandler(this.btReset_Click);
			this.btDel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.btDel.Location = new System.Drawing.Point(8, 74);
			this.btDel.Name = "btDel";
			this.btDel.Size = new System.Drawing.Size(147, 30);
			this.btDel.TabIndex = 7;
			this.btDel.Text = "&Delete Selected Band";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			this.chColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.chColumns.Location = new System.Drawing.Point(8, 40);
			this.chColumns.Name = "chColumns";
			this.chColumns.Size = new System.Drawing.Size(147, 30);
			this.chColumns.TabIndex = 6;
			this.chColumns.Text = "&Show Column/Band Selector";
			this.chColumns.Click += new System.EventHandler(this.chColumns_Click);
			this.treeListPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeListPreview.Location = new System.Drawing.Point(4, 4);
			this.treeListPreview.Name = "treeListPreview";
			this.treeListPreview.Size = new System.Drawing.Size(692, 162);
			this.treeListPreview.TabIndex = 0;
			this.treeListPreview.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnPreviewTreeListKeyDown);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "BandDesigner";
			this.Size = new System.Drawing.Size(700, 480);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeListPreview)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.CheckEdit ceAutoWidth;
		private XtraEditors.SimpleButton btAdd;
		private XtraEditors.SimpleButton btReset;
		private XtraEditors.SimpleButton btDel;
		private XtraEditors.SimpleButton chColumns;
		private TreeList treeListPreview;
	}
}
