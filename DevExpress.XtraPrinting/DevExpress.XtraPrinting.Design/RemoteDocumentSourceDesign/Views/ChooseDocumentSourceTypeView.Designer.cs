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

namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	partial class ChooseDocumentSourceTypeView {
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
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem1 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseDocumentSourceTypeView));
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem2 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			DevExpress.Skins.SkinPaddingEdges skinPaddingEdges1 = new DevExpress.Skins.SkinPaddingEdges();
			this.galleryControl1 = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.galleryControl1)).BeginInit();
			this.galleryControl1.SuspendLayout();
			this.SuspendLayout();
			this.galleryControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.galleryControl1.Controls.Add(this.galleryControlClient1);
			this.galleryControl1.DesignGalleryGroupIndex = 0;
			this.galleryControl1.DesignGalleryItemIndex = 0;
			this.galleryControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryControl1.Gallery.Appearance.ItemCaptionAppearance.Pressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
			this.galleryControl1.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseBackColor = true;
			this.galleryControl1.Gallery.Appearance.ItemDescriptionAppearance.Pressed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
			this.galleryControl1.Gallery.Appearance.ItemDescriptionAppearance.Pressed.Options.UseBackColor = true;
			this.galleryControl1.Gallery.BackColor = System.Drawing.Color.Transparent;
			this.galleryControl1.Gallery.CheckDrawMode = DevExpress.XtraBars.Ribbon.Gallery.CheckDrawMode.ImageAndText;
			this.galleryControl1.Gallery.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.galleryControl1.Gallery.DistanceBetweenItems = 24;
			this.galleryControl1.Gallery.FirstItemVertAlignment = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAlignment.Center;
			this.galleryControl1.Gallery.FixedImageSize = false;
			galleryItemGroup1.Caption = "Group1";
			galleryItem1.Caption = "Report Service";
			galleryItem1.Image = ((System.Drawing.Image)(resources.GetObject("galleryItem1.Image")));
			galleryItem1.Value = 0;
			galleryItem2.Caption = "Report Server";
			galleryItem2.Image = ((System.Drawing.Image)(resources.GetObject("galleryItem2.Image")));
			galleryItem2.Value = 1;
			galleryItemGroup1.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
			galleryItem1,
			galleryItem2});
			this.galleryControl1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1});
			this.galleryControl1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
			this.galleryControl1.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.galleryControl1.Gallery.ItemSize = new System.Drawing.Size(160, 180);
			skinPaddingEdges1.All = 12;
			skinPaddingEdges1.Bottom = 12;
			skinPaddingEdges1.Left = 12;
			skinPaddingEdges1.Right = 12;
			skinPaddingEdges1.Top = 12;
			this.galleryControl1.Gallery.ItemTextPadding = skinPaddingEdges1;
			this.galleryControl1.Gallery.RowCount = 1;
			this.galleryControl1.Gallery.ShowGroupCaption = false;
			this.galleryControl1.Gallery.ShowItemText = true;
			this.galleryControl1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.galleryControl1.Location = new System.Drawing.Point(0, 0);
			this.galleryControl1.LookAndFeel.UseDefaultLookAndFeel = false;
			this.galleryControl1.Name = "galleryControl1";
			this.galleryControl1.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this.galleryControl1.Size = new System.Drawing.Size(513, 338);
			this.galleryControl1.TabIndex = 1;
			this.galleryControl1.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.galleryControl1;
			this.galleryControlClient1.Location = new System.Drawing.Point(24, 0);
			this.galleryControlClient1.Size = new System.Drawing.Size(489, 338);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.galleryControl1);
			this.Name = "ChooseDocumentSourceTypeView";
			this.Size = new System.Drawing.Size(513, 338);
			((System.ComponentModel.ISupportInitialize)(this.galleryControl1)).EndInit();
			this.galleryControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraBars.Ribbon.GalleryControl galleryControl1;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
	}
}
