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
	partial class AddAnnotationUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.grlAnnotationTypes = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.grlAnnotationTypes)).BeginInit();
			this.grlAnnotationTypes.SuspendLayout();
			this.SuspendLayout();
			this.grlAnnotationTypes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.grlAnnotationTypes.Controls.Add(this.galleryControlClient1);
			this.grlAnnotationTypes.DesignGalleryGroupIndex = 0;
			this.grlAnnotationTypes.DesignGalleryItemIndex = 0;
			this.grlAnnotationTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grlAnnotationTypes.Gallery.AutoFitColumns = false;
			this.grlAnnotationTypes.Gallery.ColumnCount = 1;
			this.grlAnnotationTypes.Gallery.FixedImageSize = false;
			this.grlAnnotationTypes.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.grlAnnotationTypes.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.grlAnnotationTypes.Gallery.ShowItemText = true;
			this.grlAnnotationTypes.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.grlAnnotationTypes.Gallery.ItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.galleryControl_ItemClick);
			this.grlAnnotationTypes.Location = new System.Drawing.Point(0, 0);
			this.grlAnnotationTypes.Margin = new System.Windows.Forms.Padding(0);
			this.grlAnnotationTypes.Name = "grlAnnotationTypes";
			this.grlAnnotationTypes.Size = new System.Drawing.Size(187, 127);
			this.grlAnnotationTypes.TabIndex = 81;
			this.grlAnnotationTypes.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.grlAnnotationTypes;
			this.galleryControlClient1.Location = new System.Drawing.Point(1, 1);
			this.galleryControlClient1.Size = new System.Drawing.Size(185, 125);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.grlAnnotationTypes);
			this.Name = "AddAnnotationUserControl";
			this.Size = new System.Drawing.Size(187, 127);
			((System.ComponentModel.ISupportInitialize)(this.grlAnnotationTypes)).EndInit();
			this.grlAnnotationTypes.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraBars.Ribbon.GalleryControl grlAnnotationTypes;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
	}
}
