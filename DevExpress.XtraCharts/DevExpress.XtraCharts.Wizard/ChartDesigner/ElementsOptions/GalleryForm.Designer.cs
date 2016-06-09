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

namespace DevExpress.XtraCharts.Designer.Native {
	partial class GalleryForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.glrViewTypes = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.glrViewTypes)).BeginInit();
			this.glrViewTypes.SuspendLayout();
			this.SuspendLayout();
			this.glrViewTypes.Controls.Add(this.galleryControlClient1);
			this.glrViewTypes.DesignGalleryGroupIndex = 0;
			this.glrViewTypes.DesignGalleryItemIndex = 0;
			this.glrViewTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glrViewTypes.Gallery.ItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.OnGalleryControlItemClick);
			this.glrViewTypes.Location = new System.Drawing.Point(0, 0);
			this.glrViewTypes.Name = "glrViewTypes";
			this.glrViewTypes.Size = new System.Drawing.Size(284, 261);
			this.glrViewTypes.TabIndex = 79;
			this.glrViewTypes.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.glrViewTypes;
			this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient1.Size = new System.Drawing.Size(263, 257);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.glrViewTypes);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "GalleryForm";
			this.Deactivate += new System.EventHandler(this.OnFormDeactivate);
			((System.ComponentModel.ISupportInitialize)(this.glrViewTypes)).EndInit();
			this.glrViewTypes.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		private XtraBars.Ribbon.GalleryControl glrViewTypes;
	}
}
