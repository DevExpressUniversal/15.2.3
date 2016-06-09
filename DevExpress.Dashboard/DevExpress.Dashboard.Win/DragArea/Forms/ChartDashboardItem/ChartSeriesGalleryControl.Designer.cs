#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class ChartSeriesGalleryControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.galleryControl = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.galleryControl)).BeginInit();
			this.galleryControl.SuspendLayout();
			this.SuspendLayout();
			this.galleryControl.Controls.Add(this.galleryControlClient);
			this.galleryControl.DesignGalleryGroupIndex = 0;
			this.galleryControl.DesignGalleryItemIndex = 0;
			this.galleryControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryControl.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.galleryControl.Gallery.AllowFilter = false;
			this.galleryControl.Gallery.ImageSize = new System.Drawing.Size(32, 32);
			this.galleryControl.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.galleryControl.Gallery.ScrollMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryScrollMode.Smooth;
			this.galleryControl.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.galleryControl.Gallery.ItemCheckedChanged += new DevExpress.XtraBars.Ribbon.GalleryItemEventHandler(this.OnItemCheckedChanged);
			this.galleryControl.Location = new System.Drawing.Point(0, 0);
			this.galleryControl.Name = "galleryControl";
			this.galleryControl.Size = new System.Drawing.Size(389, 555);
			this.galleryControl.TabIndex = 10;
			this.galleryControlClient.GalleryControl = this.galleryControl;
			this.galleryControlClient.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient.Size = new System.Drawing.Size(385, 551);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.galleryControl);
			this.Name = "ChartSeriesGalleryControl";
			this.Size = new System.Drawing.Size(389, 555);
			((System.ComponentModel.ISupportInitialize)(this.galleryControl)).EndInit();
			this.galleryControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraBars.Ribbon.GalleryControl galleryControl;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient;
	}
}
