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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class ChooseDataSourceTypePageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.Skins.SkinPaddingEdges skinPaddingEdges1 = new DevExpress.Skins.SkinPaddingEdges();
			DevExpress.Skins.SkinPaddingEdges skinPaddingEdges2 = new DevExpress.Skins.SkinPaddingEdges();
			this.galleryDataSourceType = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryDataSourceType)).BeginInit();
			this.galleryDataSourceType.SuspendLayout();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.panelBaseContent.Controls.Add(this.galleryDataSourceType);
			this.galleryDataSourceType.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.galleryDataSourceType.Controls.Add(this.galleryControlClient);
			this.galleryDataSourceType.DesignGalleryGroupIndex = 0;
			this.galleryDataSourceType.DesignGalleryItemIndex = 0;
			this.galleryDataSourceType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryDataSourceType.Gallery.AllowFilter = false;
			this.galleryDataSourceType.Gallery.AllowMarqueeSelection = true;
			this.galleryDataSourceType.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.None;
			this.galleryDataSourceType.Gallery.BackColor = System.Drawing.Color.Transparent;
			this.galleryDataSourceType.Gallery.BackgroundImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.Default;
			this.galleryDataSourceType.Gallery.CheckDrawMode = DevExpress.XtraBars.Ribbon.Gallery.CheckDrawMode.ImageAndText;
			this.galleryDataSourceType.Gallery.CheckSelectedItemViaKeyboard = true;
			this.galleryDataSourceType.Gallery.ColumnCount = 2;
			this.galleryDataSourceType.Gallery.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.galleryDataSourceType.Gallery.DistanceBetweenItems = 12;
			this.galleryDataSourceType.Gallery.DistanceItemImageToText = 6;
			this.galleryDataSourceType.Gallery.DrawImageBackground = false;
			this.galleryDataSourceType.Gallery.FirstItemVertAlignment = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAlignment.Center;
			this.galleryDataSourceType.Gallery.ImageSize = new System.Drawing.Size(96, 96);
			this.galleryDataSourceType.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.galleryDataSourceType.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Top;
			skinPaddingEdges1.Bottom = 12;
			skinPaddingEdges1.Left = 12;
			skinPaddingEdges1.Right = 12;
			skinPaddingEdges1.Top = 6;
			this.galleryDataSourceType.Gallery.ItemImagePadding = skinPaddingEdges1;
			skinPaddingEdges2.Bottom = 6;
			this.galleryDataSourceType.Gallery.ItemTextPadding = skinPaddingEdges2;
			this.galleryDataSourceType.Gallery.LastItemVertAlignment = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAlignment.Center;
			this.galleryDataSourceType.Gallery.RowCount = 1;
			this.galleryDataSourceType.Gallery.ShowGroupCaption = false;
			this.galleryDataSourceType.Gallery.ShowItemText = true;
			this.galleryDataSourceType.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.galleryDataSourceType.Gallery.UseMaxImageSize = true;
			this.galleryDataSourceType.Gallery.ItemDoubleClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.galleryControlGallery1_ItemDoubleClick);
			this.galleryDataSourceType.Location = new System.Drawing.Point(0, 0);
			this.galleryDataSourceType.Name = "galleryDataSourceType";
			this.galleryDataSourceType.Size = new System.Drawing.Size(606, 324);
			this.galleryDataSourceType.TabIndex = 0;
			this.galleryControlClient.GalleryControl = this.galleryDataSourceType;
			this.galleryControlClient.Location = new System.Drawing.Point(1, 1);
			this.galleryControlClient.Size = new System.Drawing.Size(604, 322);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ChooseDataSourceTypePageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryDataSourceType)).EndInit();
			this.galleryDataSourceType.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraBars.Ribbon.GalleryControl galleryDataSourceType;
		protected XtraBars.Ribbon.GalleryControlClient galleryControlClient;
	}
}
