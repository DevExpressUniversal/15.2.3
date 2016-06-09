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

namespace DevExpress.XtraRichEdit.Design.Forms {
	partial class ModifierEditControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.chkCtrl = new DevExpress.XtraEditors.CheckEdit();
			this.chkShift = new DevExpress.XtraEditors.CheckEdit();
			this.chkAlt = new DevExpress.XtraEditors.CheckEdit();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnReset = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.chkCtrl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShift.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlt.Properties)).BeginInit();
			this.SuspendLayout();
			this.chkCtrl.Location = new System.Drawing.Point(13, 13);
			this.chkCtrl.Name = "chkCtrl";
			this.chkCtrl.Properties.Caption = "Ctrl";
			this.chkCtrl.Size = new System.Drawing.Size(47, 19);
			this.chkCtrl.TabIndex = 0;
			this.chkShift.Location = new System.Drawing.Point(66, 13);
			this.chkShift.Name = "chkShift";
			this.chkShift.Properties.Caption = "Shift";
			this.chkShift.Size = new System.Drawing.Size(48, 19);
			this.chkShift.TabIndex = 1;
			this.chkAlt.Location = new System.Drawing.Point(120, 13);
			this.chkAlt.Name = "chkAlt";
			this.chkAlt.Properties.Caption = "Alt";
			this.chkAlt.Size = new System.Drawing.Size(36, 19);
			this.chkAlt.TabIndex = 2;
			this.btnOK.Location = new System.Drawing.Point(46, 50);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(52, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnReset.Location = new System.Drawing.Point(104, 50);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(52, 23);
			this.btnReset.TabIndex = 4;
			this.btnReset.Text = "Reset";
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.chkAlt);
			this.Controls.Add(this.chkShift);
			this.Controls.Add(this.chkCtrl);
			this.Name = "ModifierEditControl";
			this.Size = new System.Drawing.Size(166, 85);
			((System.ComponentModel.ISupportInitialize)(this.chkCtrl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShift.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlt.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chkCtrl;
		private DevExpress.XtraEditors.CheckEdit chkShift;
		private DevExpress.XtraEditors.CheckEdit chkAlt;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.SimpleButton btnReset;
	}
}
