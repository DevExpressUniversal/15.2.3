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

namespace DevExpress.XtraBars.WinRTLiveTiles {
	partial class TileTemplateHelperForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
			this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.comboBoxEdit2 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.pictureBoxWide = new System.Windows.Forms.PictureBox();
			this.pictureBoxSquare = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.tcWide = new DevExpress.XtraEditors.TileControl();
			this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
			this.tcSquare = new DevExpress.XtraEditors.TileControl();
			this.tileGroup2 = new DevExpress.XtraEditors.TileGroup();
			((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSquare)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			this.memoEdit1.Location = new System.Drawing.Point(6, 20);
			this.memoEdit1.Name = "memoEdit1";
			this.memoEdit1.Size = new System.Drawing.Size(362, 184);
			this.memoEdit1.TabIndex = 0;
			this.memoEdit1.UseOptimizedRendering = true;
			this.comboBoxEdit1.Location = new System.Drawing.Point(12, 12);
			this.comboBoxEdit1.Name = "comboBoxEdit1";
			this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit1.Size = new System.Drawing.Size(248, 20);
			this.comboBoxEdit1.TabIndex = 1;
			this.comboBoxEdit2.Location = new System.Drawing.Point(266, 12);
			this.comboBoxEdit2.Name = "comboBoxEdit2";
			this.comboBoxEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit2.Size = new System.Drawing.Size(120, 20);
			this.comboBoxEdit2.TabIndex = 2;
			this.pictureBoxWide.Location = new System.Drawing.Point(12, 38);
			this.pictureBoxWide.Name = "pictureBoxWide";
			this.pictureBoxWide.Size = new System.Drawing.Size(248, 120);
			this.pictureBoxWide.TabIndex = 3;
			this.pictureBoxWide.TabStop = false;
			this.pictureBoxSquare.Location = new System.Drawing.Point(266, 38);
			this.pictureBoxSquare.Name = "pictureBoxSquare";
			this.pictureBoxSquare.Size = new System.Drawing.Size(120, 120);
			this.pictureBoxSquare.TabIndex = 4;
			this.pictureBoxSquare.TabStop = false;
			this.groupBox1.Controls.Add(this.memoEdit1);
			this.groupBox1.Location = new System.Drawing.Point(12, 308);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(374, 215);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Code snippet";
			this.simpleButton1.Location = new System.Drawing.Point(433, 562);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(150, 23);
			this.simpleButton1.TabIndex = 6;
			this.simpleButton1.Text = "Copy all and close";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.tcWide.Groups.Add(this.tileGroup1);
			this.tcWide.Location = new System.Drawing.Point(12, 163);
			this.tcWide.MaxId = 1;
			this.tcWide.Name = "tcWide";
			this.tcWide.Size = new System.Drawing.Size(248, 139);
			this.tcWide.TabIndex = 7;
			this.tcWide.Text = "tileControl1";
			this.tileGroup1.Name = "tileGroup1";
			this.tileGroup1.Text = null;
			this.tcSquare.Groups.Add(this.tileGroup2);
			this.tcSquare.Location = new System.Drawing.Point(266, 164);
			this.tcSquare.Name = "tcSquare";
			this.tcSquare.Size = new System.Drawing.Size(120, 138);
			this.tcSquare.TabIndex = 8;
			this.tcSquare.Text = "tileControl1";
			this.tileGroup2.Name = "tileGroup2";
			this.tileGroup2.Text = null;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(398, 526);
			this.Controls.Add(this.tcSquare);
			this.Controls.Add(this.tcWide);
			this.Controls.Add(this.simpleButton1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBoxSquare);
			this.Controls.Add(this.pictureBoxWide);
			this.Controls.Add(this.comboBoxEdit2);
			this.Controls.Add(this.comboBoxEdit1);
			this.Name = "TileTemplateHelperForm";
			this.Text = "TileTemplateHelper";
			this.Load += new System.EventHandler(this.TileTemplateHelperForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWide)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSquare)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.MemoEdit memoEdit1;
		private XtraEditors.ComboBoxEdit comboBoxEdit1;
		private XtraEditors.ComboBoxEdit comboBoxEdit2;
		private System.Windows.Forms.PictureBox pictureBoxWide;
		private System.Windows.Forms.PictureBox pictureBoxSquare;
		private System.Windows.Forms.GroupBox groupBox1;
		private XtraEditors.SimpleButton simpleButton1;
		private XtraEditors.TileControl tcWide;
		private XtraEditors.TileGroup tileGroup1;
		private XtraEditors.TileControl tcSquare;
		private XtraEditors.TileGroup tileGroup2;
	}
}
