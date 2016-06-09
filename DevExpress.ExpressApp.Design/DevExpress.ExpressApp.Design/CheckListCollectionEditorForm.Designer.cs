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

namespace DevExpress.ExpressApp.Design {
	partial class CheckListCollectionEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.itemList = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.selectAllCheckBox = new DevExpress.XtraEditors.CheckEdit();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.itemList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.selectAllCheckBox.Properties)).BeginInit();
			this.SuspendLayout();
			this.itemList.CheckOnClick = true;
			this.itemList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemList.Location = new System.Drawing.Point(14, 15);
			this.itemList.Margin = new System.Windows.Forms.Padding(0);
			this.itemList.Name = "itemList";
			this.itemList.Size = new System.Drawing.Size(429, 352);
			this.itemList.TabIndex = 4;
			this.panelControl1.AutoSize = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.selectAllCheckBox);
			this.panelControl1.Controls.Add(this.cancelButton);
			this.panelControl1.Controls.Add(this.okButton);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(14, 367);
			this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(429, 43);
			this.panelControl1.TabIndex = 5;
			this.selectAllCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.selectAllCheckBox.Location = new System.Drawing.Point(3, 17);
			this.selectAllCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.selectAllCheckBox.Name = "selectAllCheckBox";
			this.selectAllCheckBox.Properties.Caption = "Select all";
			this.selectAllCheckBox.Size = new System.Drawing.Size(87, 21);
			this.selectAllCheckBox.TabIndex = 6;
			this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(342, 15);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(87, 28);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(250, 15);
			this.okButton.Margin = new System.Windows.Forms.Padding(0);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(87, 28);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "OK";
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 425);
			this.Controls.Add(this.itemList);
			this.Controls.Add(this.panelControl1);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(276, 383);
			this.Name = "CheckListCollectionEditorForm";
			this.Padding = new System.Windows.Forms.Padding(14, 15, 14, 15);
			((System.ComponentModel.ISupportInitialize)(this.itemList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.selectAllCheckBox.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		internal DevExpress.XtraEditors.CheckedListBoxControl itemList;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.CheckEdit selectAllCheckBox;
		private DevExpress.XtraEditors.SimpleButton cancelButton;
		private DevExpress.XtraEditors.SimpleButton okButton;
	}
}
