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

using DevExpress.XtraGauges.Win.Design.Controls;
namespace DevExpress.XtraGauges.Design {
	partial class ItemKindChoosingForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.gallery = new DevExpress.XtraGauges.Win.Design.Controls.GaugesGalleryControl();
			((System.ComponentModel.ISupportInitialize)(this.gallery)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCancel.Location = new System.Drawing.Point(243, 8);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 25);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnOk.Location = new System.Drawing.Point(156, 8);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 25);
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.separator.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.separator.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.separator.LineVisible = true;
			this.separator.Location = new System.Drawing.Point(0, 326);
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(330, 3);
			this.separator.TabIndex = 10;
			this.panel2.Controls.Add(this.btnOk);
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Controls.Add(this.btnCancel);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 329);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
			this.panel2.Size = new System.Drawing.Size(330, 41);
			this.panel2.TabIndex = 12;
			this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel3.Location = new System.Drawing.Point(231, 8);
			this.panel3.MinimumSize = new System.Drawing.Size(12, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(12, 25);
			this.panel3.TabIndex = 10;
			this.gallery.BackColor = System.Drawing.SystemColors.Window;
			this.gallery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gallery.ItemImageScaleFactor = 1F;
			this.gallery.ItemSize = new System.Drawing.Size(174, 174);
			this.gallery.ItemTextVerticalOffset = 0.8F;
			this.gallery.Location = new System.Drawing.Point(0, 0);
			this.gallery.Name = "gallery";
			this.gallery.SelectedIndex = -1;
			this.gallery.Size = new System.Drawing.Size(330, 326);
			this.gallery.TabIndex = 13;
			this.AcceptButton = this.btnOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(330, 370);
			this.ControlBox = false;
			this.Controls.Add(this.gallery);
			this.Controls.Add(this.separator);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MinimumSize = new System.Drawing.Size(200, 128);
			this.Name = "ItemKindChoosingForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Choose Kind";
			((System.ComponentModel.ISupportInitialize)(this.gallery)).EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.LabelControl separator;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		protected GaugesGalleryControl gallery;
	}
}
