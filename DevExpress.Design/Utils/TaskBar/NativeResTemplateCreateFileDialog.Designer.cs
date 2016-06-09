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

namespace DevExpress.Utils.Design.Taskbar {
	partial class NativeResTemplateCreateFileDialog {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.listBox = new DevExpress.XtraEditors.ListBoxControl();
			this.lblProjects = new DevExpress.XtraEditors.LabelControl();
			this.lblFileName = new DevExpress.XtraEditors.LabelControl();
			this.textEditFileName = new DevExpress.XtraEditors.TextEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.listBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEditFileName.Properties)).BeginInit();
			this.SuspendLayout();
			this.listBox.Location = new System.Drawing.Point(12, 31);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(338, 191);
			this.listBox.TabIndex = 0;
			this.lblProjects.Location = new System.Drawing.Point(12, 12);
			this.lblProjects.Name = "lblProjects";
			this.lblProjects.Size = new System.Drawing.Size(39, 13);
			this.lblProjects.TabIndex = 1;
			this.lblProjects.Text = "Projects";
			this.lblFileName.Location = new System.Drawing.Point(12, 228);
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new System.Drawing.Size(175, 13);
			this.lblFileName.TabIndex = 2;
			this.lblFileName.Text = "Icon Resources File Name:";
			this.textEditFileName.EditValue = "Icons.res";
			this.textEditFileName.Location = new System.Drawing.Point(12, 247);
			this.textEditFileName.Name = "textEditFileName";
			this.textEditFileName.Size = new System.Drawing.Size(338, 20);
			this.textEditFileName.TabIndex = 3;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(276, 279);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnCreate.Location = new System.Drawing.Point(195, 279);
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Size = new System.Drawing.Size(75, 23);
			this.btnCreate.TabIndex = 5;
			this.btnCreate.Text = "Create";
			this.btnCreate.Click += new System.EventHandler(this.OnBtnCreateClick);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 317);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.textEditFileName);
			this.Controls.Add(this.lblFileName);
			this.Controls.Add(this.lblProjects);
			this.Controls.Add(this.listBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NativeResTemplateCreateFileDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create Icon Resources File";
			((System.ComponentModel.ISupportInitialize)(this.listBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEditFileName.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.ListBoxControl listBox;
		private XtraEditors.LabelControl lblProjects;
		private XtraEditors.LabelControl lblFileName;
		private XtraEditors.TextEdit textEditFileName;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnCreate;
	}
}
