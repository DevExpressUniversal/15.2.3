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
	partial class GalleryWithFilterUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.gallery = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.searchControl = new DevExpress.XtraEditors.SearchControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleSeparator2 = new DevExpress.XtraLayout.SimpleSeparator();
			this.searchControlLayoutItem = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.gallery)).BeginInit();
			this.gallery.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlLayoutItem)).BeginInit();
			this.SuspendLayout();
			this.gallery.Controls.Add(this.galleryControlClient1);
			this.gallery.DesignGalleryGroupIndex = 0;
			this.gallery.DesignGalleryItemIndex = 0;
			this.gallery.Gallery.AllowFilter = false;
			this.gallery.Gallery.AutoFitColumns = false;
			this.gallery.Gallery.ColumnCount = 1;
			this.gallery.Gallery.FixedImageSize = false;
			this.gallery.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			this.gallery.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.gallery.Gallery.ShowItemText = true;
			this.gallery.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.gallery.Gallery.ItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.OnGalleryControlItemClick);
			this.gallery.Location = new System.Drawing.Point(0, 42);
			this.gallery.Margin = new System.Windows.Forms.Padding(0);
			this.gallery.Name = "gallery";
			this.gallery.Size = new System.Drawing.Size(259, 471);
			this.gallery.StyleController = this.layoutControl;
			this.gallery.TabIndex = 80;
			this.gallery.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.gallery;
			this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient1.Size = new System.Drawing.Size(255, 467);
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Controls.Add(this.searchControl);
			this.layoutControl.Controls.Add(this.gallery);
			this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl.Location = new System.Drawing.Point(0, 0);
			this.layoutControl.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1451, 278, 1200, 350);
			this.layoutControl.Root = this.layoutControlGroup1;
			this.layoutControl.Size = new System.Drawing.Size(259, 515);
			this.layoutControl.TabIndex = 2;
			this.layoutControl.Text = "layoutControl1";
			this.searchControl.Client = this.gallery.Gallery;
			this.searchControl.Location = new System.Drawing.Point(10, 10);
			this.searchControl.Name = "searchControl";
			this.searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.searchControl.Properties.Client = this.gallery.Gallery;
			this.searchControl.Properties.ShowNullValuePromptWhenFocused = true;
			this.searchControl.Size = new System.Drawing.Size(239, 20);
			this.searchControl.StyleController = this.layoutControl;
			this.searchControl.TabIndex = 83;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleSeparator1,
			this.layoutControlItem2,
			this.simpleSeparator2,
			this.searchControlLayoutItem});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(259, 515);
			this.layoutControlGroup1.TextVisible = false;
			this.simpleSeparator1.AllowHotTrack = false;
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 40);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.simpleSeparator1.Size = new System.Drawing.Size(259, 2);
			this.layoutControlItem2.Control = this.gallery;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 42);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(259, 471);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.simpleSeparator2.AllowHotTrack = false;
			this.simpleSeparator2.Location = new System.Drawing.Point(0, 513);
			this.simpleSeparator2.Name = "simpleSeparator2";
			this.simpleSeparator2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.simpleSeparator2.Size = new System.Drawing.Size(259, 2);
			this.searchControlLayoutItem.Control = this.searchControl;
			this.searchControlLayoutItem.Location = new System.Drawing.Point(0, 0);
			this.searchControlLayoutItem.Name = "searchControlLayoutItem";
			this.searchControlLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
			this.searchControlLayoutItem.Size = new System.Drawing.Size(259, 40);
			this.searchControlLayoutItem.Text = "Filter: ";
			this.searchControlLayoutItem.TextSize = new System.Drawing.Size(0, 0);
			this.searchControlLayoutItem.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "GalleryWithFilterUserControl";
			this.Size = new System.Drawing.Size(259, 515);
			((System.ComponentModel.ISupportInitialize)(this.gallery)).EndInit();
			this.gallery.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.searchControl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlLayoutItem)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraBars.Ribbon.GalleryControl gallery;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.SimpleSeparator simpleSeparator1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.SimpleSeparator simpleSeparator2;
		private XtraEditors.SearchControl searchControl;
		private XtraLayout.LayoutControlItem searchControlLayoutItem;
	}
}
