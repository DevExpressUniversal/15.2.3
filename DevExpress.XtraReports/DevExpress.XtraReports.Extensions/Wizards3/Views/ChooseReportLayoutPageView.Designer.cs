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

namespace DevExpress.XtraReports.Wizards3.Views {
	partial class ChooseReportLayoutPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseReportLayoutPageView));
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup1 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.Skins.SkinPaddingEdges skinPaddingEdges1 = new DevExpress.Skins.SkinPaddingEdges();
			this.orientationPanel = new DevExpress.XtraEditors.RadioGroup();
			this.adjustFieldWidthButton = new DevExpress.XtraEditors.CheckEdit();
			this.reportLayoutGallery = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
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
			((System.ComponentModel.ISupportInitialize)(this.orientationPanel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.adjustFieldWidthButton.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.reportLayoutGallery)).BeginInit();
			this.reportLayoutGallery.SuspendLayout();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font = ((System.Drawing.Font)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			resources.ApplyResources(this.labelHeader, "labelHeader");
			this.panelBaseContent.Controls.Add(this.orientationPanel);
			this.panelBaseContent.Controls.Add(this.adjustFieldWidthButton);
			this.panelBaseContent.Controls.Add(this.reportLayoutGallery);
			resources.ApplyResources(this.panelBaseContent, "panelBaseContent");
			resources.ApplyResources(this.orientationPanel, "orientationPanel");
			this.orientationPanel.Name = "orientationPanel";
			this.orientationPanel.Properties.AllowFocused = false;
			this.orientationPanel.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("orientationPanel.Properties.Appearance.BackColor")));
			this.orientationPanel.Properties.Appearance.Options.UseBackColor = true;
			this.orientationPanel.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.orientationPanel.Properties.Columns = 2;
			this.orientationPanel.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("orientationPanel.Properties.Items"))), resources.GetString("orientationPanel.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("orientationPanel.Properties.Items2"))), resources.GetString("orientationPanel.Properties.Items3"))});
			resources.ApplyResources(this.adjustFieldWidthButton, "adjustFieldWidthButton");
			this.adjustFieldWidthButton.Name = "adjustFieldWidthButton";
			this.adjustFieldWidthButton.Properties.AllowFocused = false;
			this.adjustFieldWidthButton.Properties.Caption = resources.GetString("adjustFieldWidthButton.Properties.Caption");
			this.reportLayoutGallery.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.reportLayoutGallery.Controls.Add(this.galleryControlClient1);
			this.reportLayoutGallery.DesignGalleryGroupIndex = 0;
			this.reportLayoutGallery.DesignGalleryItemIndex = 0;
			resources.ApplyResources(this.reportLayoutGallery, "reportLayoutGallery");
			this.reportLayoutGallery.Gallery.BackColor = System.Drawing.Color.Transparent;
			this.reportLayoutGallery.Gallery.CheckDrawMode = DevExpress.XtraBars.Ribbon.Gallery.CheckDrawMode.ImageAndText;
			this.reportLayoutGallery.Gallery.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.reportLayoutGallery.Gallery.DistanceBetweenItems = 36;
			this.reportLayoutGallery.Gallery.DrawImageBackground = false;
			this.reportLayoutGallery.Gallery.FirstItemVertAlignment = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAlignment.Center;
			resources.ApplyResources(galleryItemGroup1, "galleryItemGroup1");
			this.reportLayoutGallery.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
			galleryItemGroup1});
			this.reportLayoutGallery.Gallery.ImageSize = new System.Drawing.Size(83, 98);
			this.reportLayoutGallery.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			skinPaddingEdges1.Bottom = 6;
			skinPaddingEdges1.Left = 6;
			skinPaddingEdges1.Right = 6;
			skinPaddingEdges1.Top = 3;
			this.reportLayoutGallery.Gallery.ItemImagePadding = skinPaddingEdges1;
			this.reportLayoutGallery.Gallery.RowCount = 1;
			this.reportLayoutGallery.Gallery.ShowGroupCaption = false;
			this.reportLayoutGallery.Gallery.ShowItemText = true;
			this.reportLayoutGallery.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.reportLayoutGallery.Gallery.UseMaxImageSize = true;
			this.reportLayoutGallery.Name = "reportLayoutGallery";
			this.galleryControlClient1.GalleryControl = this.reportLayoutGallery;
			resources.ApplyResources(this.galleryControlClient1, "galleryControlClient1");
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ChooseReportLayoutPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.orientationPanel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.adjustFieldWidthButton.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.reportLayoutGallery)).EndInit();
			this.reportLayoutGallery.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.RadioGroup orientationPanel;
		private XtraEditors.CheckEdit adjustFieldWidthButton;
		private XtraBars.Ribbon.GalleryControl reportLayoutGallery;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
	}
}
