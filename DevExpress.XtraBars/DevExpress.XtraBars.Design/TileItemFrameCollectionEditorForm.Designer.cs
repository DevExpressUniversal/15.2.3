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
	partial class TileItemFrameEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.TileItemFrameEditorControl1 = new DevExpress.XtraBars.Design.TileItemFrameEditorControl();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButton1.Location = new System.Drawing.Point(763, 9);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(75, 23);
			this.simpleButton1.TabIndex = 0;
			this.simpleButton1.Text = "Cancel";
			this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButton2.Location = new System.Drawing.Point(682, 9);
			this.simpleButton2.Name = "simpleButton2";
			this.simpleButton2.Size = new System.Drawing.Size(75, 23);
			this.simpleButton2.TabIndex = 1;
			this.simpleButton2.Text = "OK";
			this.panel1.Controls.Add(this.simpleButton2);
			this.panel1.Controls.Add(this.simpleButton1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 609);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(844, 44);
			this.panel1.TabIndex = 2;
			this.TileItemFrameEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TileItemFrameEditorControl1.Location = new System.Drawing.Point(0, 0);
			this.TileItemFrameEditorControl1.Name = "TileItemFrameEditorControl1";
			this.TileItemFrameEditorControl1.Padding = new System.Windows.Forms.Padding(6);
			this.TileItemFrameEditorControl1.Size = new System.Drawing.Size(844, 609);
			this.TileItemFrameEditorControl1.TabIndex = 3;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(844, 653);
			this.Controls.Add(this.TileItemFrameEditorControl1);
			this.Controls.Add(this.panel1);
			this.Name = "TileItemFrameEditorForm";
			this.Text = "TileItemFrame Collection Editor";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private SimpleButton simpleButton1;
		private SimpleButton simpleButton2;
		private System.Windows.Forms.Panel panel1;
		private TileItemFrameEditorControl TileItemFrameEditorControl1;
	}
}
