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

namespace DevExpress.Utils.Design.ProjectImagePicker {
	partial class ProjectImageSelectionForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.cbResourceFile = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbImage = new XtraEditors.CheckedListBoxControl();
			this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.pePreview = new DevExpress.XtraEditors.PictureEdit();
			this.labelNotResFound = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbResourceFile.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pePreview.Properties)).BeginInit();
			this.SuspendLayout();
			this.cbResourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbResourceFile.Location = new System.Drawing.Point(12, 12);
			this.cbResourceFile.Name = "cbResourceFile";
			this.cbResourceFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbResourceFile.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbResourceFile.Size = new System.Drawing.Size(488, 20);
			this.cbResourceFile.TabIndex = 2;
			this.cbResourceFile.SelectedIndexChanged += new System.EventHandler(this.ResourceFile_SelectedIndexChanged);
			this.lbImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lbImage.Location = new System.Drawing.Point(12, 38);
			this.lbImage.Name = "lbImage";
			this.lbImage.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbImage.Size = new System.Drawing.Size(174, 304);
			this.lbImage.TabIndex = 3;
			this.lbImage.SelectedIndexChanged += new System.EventHandler(this.ImageList_SelectedIndexChanged);
			this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelect.Location = new System.Drawing.Point(344, 354);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(75, 23);
			this.btnSelect.TabIndex = 4;
			this.btnSelect.Text = "Select";
			this.btnSelect.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(425, 354);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.pePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pePreview.Location = new System.Drawing.Point(192, 38);
			this.pePreview.Name = "pePreview";
			this.pePreview.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
			this.pePreview.Size = new System.Drawing.Size(308, 304);
			this.pePreview.TabIndex = 6;
			this.labelNotResFound.Location = new System.Drawing.Point(9, 358);
			this.labelNotResFound.Name = "labelNotResFound";
			this.labelNotResFound.Size = new System.Drawing.Size(121, 13);
			this.labelNotResFound.TabIndex = 7;
			this.labelNotResFound.Text = "No resources were found";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(512, 389);
			this.Controls.Add(this.labelNotResFound);
			this.Controls.Add(this.pePreview);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSelect);
			this.Controls.Add(this.lbImage);
			this.Controls.Add(this.cbResourceFile);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProjectImageSelectionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Resources";
			((System.ComponentModel.ISupportInitialize)(this.cbResourceFile.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pePreview.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.ComboBoxEdit cbResourceFile;
		private DevExpress.XtraEditors.CheckedListBoxControl lbImage;
		private DevExpress.XtraEditors.SimpleButton btnSelect;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.PictureEdit pePreview;
		private DevExpress.XtraEditors.LabelControl labelNotResFound;
	}
}
