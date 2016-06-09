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

namespace DevExpress.XtraReports.Native.Templates {
	partial class GalleryUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GalleryUserControl));
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			this.gcTreeListContainer = new DevExpress.XtraEditors.GroupControl();
			this.gcProgress = new DevExpress.XtraReports.Native.Templates.ProgressUserControl();
			this.galleryControl1 = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			this.gcNoData = new DevExpress.XtraReports.Native.Templates.NoDataUserControl();
			this.teSearch = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.gcTreeListContainer)).BeginInit();
			this.gcTreeListContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.galleryControl1)).BeginInit();
			this.galleryControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.teSearch.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.gcTreeListContainer, "gcTreeListContainer");
			this.gcTreeListContainer.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("gcTreeListContainer.Appearance.BackColor")));
			this.gcTreeListContainer.Appearance.Options.UseBackColor = true;
			this.gcTreeListContainer.Controls.Add(this.gcProgress);
			this.gcTreeListContainer.Controls.Add(this.galleryControl1);
			this.gcTreeListContainer.Controls.Add(this.gcNoData);
			this.gcTreeListContainer.Name = "gcTreeListContainer";
			this.gcTreeListContainer.ShowCaption = false;
			resources.ApplyResources(this.gcProgress, "gcProgress");
			this.gcProgress.Name = "gcProgress";
			this.gcProgress.TabStop = false;
			this.galleryControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.galleryControl1.Controls.Add(this.galleryControlClient1);
			this.galleryControl1.DesignGalleryGroupIndex = 0;
			this.galleryControl1.DesignGalleryItemIndex = 0;
			resources.ApplyResources(this.galleryControl1, "galleryControl1");
			this.galleryControl1.Gallery.BackColor = System.Drawing.Color.White;
			this.galleryControl1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1});
			this.galleryControl1.Gallery.ImageSize = new System.Drawing.Size(96, 96);
			this.galleryControl1.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.galleryControl1.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.ZoomInside;
			this.galleryControl1.Gallery.ShowGroupCaption = false;
			this.galleryControl1.Gallery.ShowItemText = true;
			this.galleryControl1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			this.galleryControl1.Name = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.galleryControl1;
			resources.ApplyResources(this.galleryControlClient1, "galleryControlClient1");
			resources.ApplyResources(this.gcNoData, "gcNoData");
			this.gcNoData.Name = "gcNoData";
			this.gcNoData.TabStop = false;
			resources.ApplyResources(this.teSearch, "teSearch");
			this.teSearch.Name = "teSearch";
			this.teSearch.Properties.NullValuePrompt = resources.GetString("teSearch.Properties.NullValuePrompt");
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.teSearch);
			this.Controls.Add(this.gcTreeListContainer);
			this.Name = "GalleryUserControl";
			((System.ComponentModel.ISupportInitialize)(this.gcTreeListContainer)).EndInit();
			this.gcTreeListContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.galleryControl1)).EndInit();
			this.galleryControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.teSearch.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.GroupControl gcTreeListContainer;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		public ProgressUserControl gcProgress;
		public NoDataUserControl gcNoData;
		public XtraBars.Ribbon.GalleryControl galleryControl1;
		public XtraEditors.TextEdit teSearch;
	}
}
