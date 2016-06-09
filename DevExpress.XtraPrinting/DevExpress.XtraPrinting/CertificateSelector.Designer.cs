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

namespace DevExpress.XtraPrinting.Native.WinControls {
	partial class CertificateSelector {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			this.list = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.list)).BeginInit();
			this.list.SuspendLayout();
			this.SuspendLayout();
			this.list.Controls.Add(this.galleryControlClient1);
			this.list.DesignGalleryGroupIndex = 0;
			this.list.DesignGalleryItemIndex = 0;
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.Gallery.ColumnCount = 1;
			this.list.Gallery.DrawImageBackground = false;
			galleryItemGroup1.Caption = "Group1";
			this.list.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1});
			this.list.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.list.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Right;
			this.list.Gallery.RowCount = 1;
			this.list.Gallery.ShowGroupCaption = false;
			this.list.Gallery.ShowItemText = true;
			this.list.Gallery.StretchItems = true;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(248, 320);
			this.list.TabIndex = 0;
			this.galleryControlClient1.GalleryControl = this.list;
			this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient1.Size = new System.Drawing.Size(227, 316);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.list);
			this.Name = "CertificateSelector";
			this.Size = new System.Drawing.Size(248, 320);
			((System.ComponentModel.ISupportInitialize)(this.list)).EndInit();
			this.list.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
