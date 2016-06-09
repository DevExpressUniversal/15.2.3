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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Localization;
namespace DevExpress.ExpressApp.Win.Editors {
	public class FindForm : DevExpress.XtraEditors.XtraForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			edtFindText.Focus();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.findLabel = new System.Windows.Forms.Label();
			this.edtFindText = new DevExpress.XtraEditors.TextEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnFind = new DevExpress.XtraEditors.SimpleButton();
			this.findPanel = new DevExpress.XtraEditors.PanelControl();
			this.labelPanel = new DevExpress.XtraEditors.PanelControl();
			this.editorPanel = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.findPanel)).BeginInit();
			this.findPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.labelPanel)).BeginInit();
			this.labelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.editorPanel)).BeginInit();
			this.editorPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFindText.Properties)).BeginInit();
			this.SuspendLayout();
			this.findPanel.AutoSize = true;
			this.findPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.findPanel.Controls.Add(this.editorPanel);
			this.findPanel.Controls.Add(this.labelPanel);
			this.findPanel.Location = new System.Drawing.Point(13, 13);
			this.findPanel.Name = "findPanel";
			this.findPanel.Size = new System.Drawing.Size(250, 30);
			this.findPanel.TabIndex = 4;
			this.labelPanel.AutoSize = true;
			this.labelPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.labelPanel.Controls.Add(this.findLabel);
			this.labelPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelPanel.Location = new System.Drawing.Point(0, 0);
			this.labelPanel.Name = "labelPanel";
			this.labelPanel.Size = new System.Drawing.Size(7, 26);
			this.labelPanel.TabIndex = 0;
			this.editorPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.editorPanel.Controls.Add(this.edtFindText);
			this.editorPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.editorPanel.Location = new System.Drawing.Point(46, 2);
			this.editorPanel.Name = "editorPanel";
			this.editorPanel.Size = new System.Drawing.Size(202, 26);
			this.editorPanel.TabIndex = 1;
			this.findLabel.AutoSize = true;
			this.findLabel.Location = new System.Drawing.Point(0, 3);
			this.findLabel.Name = "label1";
			this.findLabel.Size = new System.Drawing.Size(0, 13);
			this.findLabel.TabIndex = 0;
			this.edtFindText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.edtFindText.Location = new System.Drawing.Point(2, 0);
			this.edtFindText.Name = "edtFindText";
			this.edtFindText.Size = new System.Drawing.Size(187, 20);
			this.edtFindText.TabIndex = 1;
			this.edtFindText.EditValueChanged += new System.EventHandler(this.edtFindText_EditValueChanged);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(180, 54);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = DialogButtonsLocalizer.GetCaption(DialogButton.Cancel);
			this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFind.Location = new System.Drawing.Point(99, 54);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(75, 23);
			this.btnFind.TabIndex = 2;
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			this.AcceptButton = this.btnFind;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(271, 89);
			this.Controls.Add(this.findPanel);
			this.Controls.Add(this.btnFind);
			this.Controls.Add(this.btnCancel);
			this.Name = "FindForm";
			((System.ComponentModel.ISupportInitialize)(this.findPanel)).EndInit();
			this.findPanel.ResumeLayout(false);
			this.findPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.labelPanel)).EndInit();
			this.labelPanel.ResumeLayout(false);
			this.labelPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.editorPanel)).EndInit();
			this.editorPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.edtFindText.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl findPanel;
		private DevExpress.XtraEditors.PanelControl editorPanel;
		private DevExpress.XtraEditors.PanelControl labelPanel;
		private System.Windows.Forms.Label findLabel;
		private DevExpress.XtraEditors.TextEdit edtFindText;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnFind;
		private void edtFindText_EditValueChanged(object sender, EventArgs e) {
			btnFind.Enabled = !string.IsNullOrEmpty(edtFindText.Text);
		}
		private void btnFind_Click(object sender, EventArgs e) {
			if(DoFind != null) {
				DoFind(this, EventArgs.Empty);
			}
		}
		private void Localize() {
			this.Text = LargeStringEditFindFormLocalizer.Active.GetLocalizedString("FormText");
			this.btnFind.Text = LargeStringEditFindFormLocalizer.Active.GetLocalizedString("FindNextButtonText");
			this.findLabel.Text = LargeStringEditFindFormLocalizer.Active.GetLocalizedString("FindLabelText");
		}
		public FindForm() {
			InitializeComponent();
			Localize();
		}
		public string TextToFind {
			get { return edtFindText.Text; }
			set { edtFindText.Text = value; }
		}
		public event EventHandler DoFind;
	}
}
