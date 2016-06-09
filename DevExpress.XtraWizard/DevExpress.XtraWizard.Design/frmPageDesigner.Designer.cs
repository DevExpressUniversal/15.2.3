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

namespace DevExpress.XtraWizard.Design {
	partial class frmPageDesigner {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lbcPages = new DevExpress.XtraEditors.ListBoxControl();
			this.sbUp = new DevExpress.XtraEditors.SimpleButton();
			this.sbDown = new DevExpress.XtraEditors.SimpleButton();
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.lbcPages)).BeginInit();
			this.SuspendLayout();
			this.labelControl1.Location = new System.Drawing.Point(12, 12);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(107, 13);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "Select a page to view:";
			this.lbcPages.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
			this.lbcPages.Location = new System.Drawing.Point(12, 31);
			this.lbcPages.Name = "lbcPages";
			this.lbcPages.Size = new System.Drawing.Size(374, 236);
			this.lbcPages.TabIndex = 1;
			this.lbcPages.SelectedIndexChanged += new System.EventHandler(this.lbcPages_SelectedIndexChanged);
			this.sbUp.AllowFocus = false;
			this.sbUp.Enabled = false;
			this.sbUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.sbUp.Location = new System.Drawing.Point(401, 31);
			this.sbUp.Name = "sbUp";
			this.sbUp.Size = new System.Drawing.Size(32, 24);
			this.sbUp.TabIndex = 2;
			this.sbUp.Click += new System.EventHandler(this.sbUp_Click);
			this.sbDown.AllowFocus = false;
			this.sbDown.Enabled = false;
			this.sbDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.sbDown.Location = new System.Drawing.Point(401, 61);
			this.sbDown.Name = "sbDown";
			this.sbDown.Size = new System.Drawing.Size(32, 24);
			this.sbDown.TabIndex = 3;
			this.sbDown.Click += new System.EventHandler(this.sbDown_Click);
			this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.sbOK.Location = new System.Drawing.Point(401, 213);
			this.sbOK.Name = "sbOK";
			this.sbOK.Size = new System.Drawing.Size(84, 24);
			this.sbOK.TabIndex = 4;
			this.sbOK.Text = "&OK";
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.sbCancel.Location = new System.Drawing.Point(401, 243);
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.Size = new System.Drawing.Size(84, 24);
			this.sbCancel.TabIndex = 5;
			this.sbCancel.Text = "&Cancel";
			this.AcceptButton = this.sbOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.sbCancel;
			this.ClientSize = new System.Drawing.Size(497, 280);
			this.Controls.Add(this.sbCancel);
			this.Controls.Add(this.sbOK);
			this.Controls.Add(this.sbDown);
			this.Controls.Add(this.sbUp);
			this.Controls.Add(this.lbcPages);
			this.Controls.Add(this.labelControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPageDesigner";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Pages Designer";
			this.Load += new System.EventHandler(this.frmPageDesigner_Load);
			((System.ComponentModel.ISupportInitialize)(this.lbcPages)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.ListBoxControl lbcPages;
		private DevExpress.XtraEditors.SimpleButton sbUp;
		private DevExpress.XtraEditors.SimpleButton sbDown;
		private DevExpress.XtraEditors.SimpleButton sbOK;
		private DevExpress.XtraEditors.SimpleButton sbCancel;
	}
}
