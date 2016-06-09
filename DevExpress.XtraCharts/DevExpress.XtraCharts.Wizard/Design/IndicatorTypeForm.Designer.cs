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

namespace DevExpress.XtraCharts.Design
{
	partial class IndicatorTypeForm
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndicatorTypeForm));
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup2 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup3 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup4 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup5 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			this.gcIndicators = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
			((System.ComponentModel.ISupportInitialize)(this.gcIndicators)).BeginInit();
			this.gcIndicators.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.galleryControlClient1.GalleryControl = this.gcIndicators;
			resources.ApplyResources(this.galleryControlClient1, "galleryControlClient1");
			resources.ApplyResources(this.gcIndicators, "gcIndicators");
			this.gcIndicators.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcIndicators.Controls.Add(this.galleryControlClient1);
			this.gcIndicators.DesignGalleryGroupIndex = 0;
			this.gcIndicators.DesignGalleryItemIndex = 0;
			this.gcIndicators.Gallery.AllowFilter = false;
			this.gcIndicators.Gallery.Appearance.ItemCaptionAppearance.Disabled.Options.UseFont = true;
			this.gcIndicators.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseFont = true;
			this.gcIndicators.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseFont = true;
			this.gcIndicators.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseFont = true;
			this.gcIndicators.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.None;
			this.gcIndicators.Gallery.ColumnCount = 1;
			this.gcIndicators.Gallery.GroupContentMargin = new System.Windows.Forms.Padding(10, 0, 0, 0);
			resources.ApplyResources(galleryItemGroup1, "galleryItemGroup1");
			galleryItemGroup1.Tag = "General";
			resources.ApplyResources(galleryItemGroup2, "galleryItemGroup2");
			resources.ApplyResources(galleryItemGroup3, "galleryItemGroup3");
			resources.ApplyResources(galleryItemGroup4, "galleryItemGroup4");
			resources.ApplyResources(galleryItemGroup5, "galleryItemGroup5");
			this.gcIndicators.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1,
			galleryItemGroup2,
			galleryItemGroup3,
			galleryItemGroup4,
			galleryItemGroup5});
			this.gcIndicators.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.gcIndicators.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.gcIndicators.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			this.gcIndicators.Gallery.RowCount = 8;
			this.gcIndicators.Gallery.ShowItemImage = false;
			this.gcIndicators.Gallery.ShowItemText = true;
			this.gcIndicators.Gallery.StretchItems = true;
			this.gcIndicators.Gallery.ItemDoubleClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.gcIndicators_Gallery_ItemDoubleClick);
			this.gcIndicators.Name = "gcIndicators";
			this.separatorControl1.LineAlignment = DevExpress.XtraEditors.Alignment.Center;
			resources.ApplyResources(this.separatorControl1, "separatorControl1");
			this.separatorControl1.Name = "separatorControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gcIndicators);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.separatorControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "IndicatorTypeForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.gcIndicators)).EndInit();
			this.gcIndicators.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraEditors.SimpleButton btnCancel;
		protected XtraEditors.SimpleButton btnOk;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		private XtraEditors.SeparatorControl separatorControl1;
		internal XtraBars.Ribbon.GalleryControl gcIndicators;
	}
}
