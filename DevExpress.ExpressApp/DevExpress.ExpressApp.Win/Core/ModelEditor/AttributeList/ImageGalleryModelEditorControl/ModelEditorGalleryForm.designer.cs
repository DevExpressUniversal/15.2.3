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

using System.Windows.Forms;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	partial class ModelEditorGalleryForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.editorsPanel = new DevExpress.XtraEditors.PanelControl();
			this.editPanel = new DevExpress.XtraEditors.PanelControl();
			this.textEdit = new DevExpress.XtraEditors.TextEdit();
			this.zoomPanel = new DevExpress.XtraEditors.PanelControl();
			this.zoomBar = new DevExpress.XtraEditors.ZoomTrackBarControl();
			this.galleryPanel = new DevExpress.XtraEditors.PanelControl();
			this.gallery = new ModelEditorGalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.editorsPanel)).BeginInit();
			this.editorsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.editPanel)).BeginInit();
			this.editPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomPanel)).BeginInit();
			this.zoomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryPanel)).BeginInit();
			this.galleryPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gallery)).BeginInit();
			this.gallery.SuspendLayout();
			this.SuspendLayout();
			this.editorsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.editorsPanel.Controls.Add(this.editPanel);
			this.editorsPanel.Controls.Add(this.zoomPanel);
			this.editorsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.editorsPanel.Location = new System.Drawing.Point(0, 0);
			this.editorsPanel.Name = "editorsPanel";
			this.editorsPanel.Size = new System.Drawing.Size(689, 18);
			this.editorsPanel.TabIndex = 0;
			this.editPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.editPanel.Controls.Add(this.textEdit);
			this.editPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editPanel.Location = new System.Drawing.Point(0, 0);
			this.editPanel.MinimumSize = new System.Drawing.Size(100, 0);
			this.editPanel.Name = "editPanel";
			this.editPanel.Size = new System.Drawing.Size(416, 18);
			this.editPanel.TabIndex = 2;
			this.textEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEdit.Location = new System.Drawing.Point(0, 0);
			this.textEdit.Name = "textEdit";
			this.textEdit.Size = new System.Drawing.Size(416, 20);
			this.textEdit.TabIndex = 0;
			this.zoomPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.zoomPanel.Controls.Add(this.zoomBar);
			this.zoomPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this.zoomPanel.Location = new System.Drawing.Point(416, 0);
			this.zoomPanel.MinimumSize = new System.Drawing.Size(200, 0);
			this.zoomPanel.Name = "zoomPanel";
			this.zoomPanel.Size = new System.Drawing.Size(273, 18);
			this.zoomPanel.TabIndex = 1;
			this.zoomBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.zoomBar.EditValue = null;
			this.zoomBar.Location = new System.Drawing.Point(0, 0);
			this.zoomBar.Name = "zoomBar";
			this.zoomBar.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
			this.zoomBar.Size = new System.Drawing.Size(273, 18);
			this.zoomBar.TabIndex = 0;
			this.galleryPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.galleryPanel.Controls.Add(this.gallery);
			this.galleryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryPanel.Location = new System.Drawing.Point(0, 18);
			this.galleryPanel.MinimumSize = new System.Drawing.Size(250, 0);
			this.galleryPanel.Name = "galleryPanel";
			this.galleryPanel.Size = new System.Drawing.Size(689, 375);
			this.galleryPanel.TabIndex = 1;
			this.gallery.Controls.Add(this.galleryControlClient1);
			this.gallery.DesignGalleryGroupIndex = 0;
			this.gallery.DesignGalleryItemIndex = 0;
			this.gallery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gallery.Location = new System.Drawing.Point(0, 0);
			this.gallery.Name = "gallery";
			this.gallery.Size = new System.Drawing.Size(689, 375);
			this.gallery.TabIndex = 0;
			this.gallery.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.gallery;
			this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient1.Size = new System.Drawing.Size(668, 371);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this.galleryPanel);
			this.Controls.Add(this.editorsPanel);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 200);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Name = "XtraForm1";
			this.Text = "XtraForm1";
			((System.ComponentModel.ISupportInitialize)(this.editorsPanel)).EndInit();
			this.editorsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.editPanel)).EndInit();
			this.editPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomPanel)).EndInit();
			this.zoomPanel.ResumeLayout(false);
			this.zoomPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryPanel)).EndInit();
			this.galleryPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gallery)).EndInit();
			this.gallery.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl editorsPanel;
		private DevExpress.XtraEditors.PanelControl galleryPanel;
		private DevExpress.XtraEditors.PanelControl zoomPanel;
		private DevExpress.XtraEditors.PanelControl editPanel;
		private ModelEditorGalleryControl gallery;
		private DevExpress.XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		private DevExpress.XtraEditors.ZoomTrackBarControl zoomBar;
		private DevExpress.XtraEditors.TextEdit textEdit;
	}
}
