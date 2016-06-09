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
	partial class EnumGalleryControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.dropDownButton = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.popupContainer = new DevExpress.XtraBars.PopupControlContainer(this.components);
			this.glrViewTypes = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.popupContainer)).BeginInit();
			this.popupContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.glrViewTypes)).BeginInit();
			this.glrViewTypes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			this.SuspendLayout();
			this.dropDownButton.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.dropDownButton.DropDownControl = this.popupContainer;
			this.dropDownButton.ImageIndex = 0;
			this.dropDownButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.dropDownButton.Location = new System.Drawing.Point(0, 0);
			this.dropDownButton.Margin = new System.Windows.Forms.Padding(0);
			this.dropDownButton.Name = "dropDownButton";
			this.dropDownButton.NotDrawButtonOnNormalState = false;
			this.dropDownButton.NotDrawButtonState = DevExpress.Utils.Drawing.ObjectState.Disabled;
			this.dropDownButton.Size = new System.Drawing.Size(34, 34);
			this.dropDownButton.TabIndex = 1;
			this.dropDownButton.Click += new System.EventHandler(this.dropDownButton_Click);
			this.popupContainer.AutoSize = true;
			this.popupContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.popupContainer.Controls.Add(this.glrViewTypes);
			this.popupContainer.Location = new System.Drawing.Point(3, 3);
			this.popupContainer.Manager = this.barManager1;
			this.popupContainer.Name = "popupContainer";
			this.popupContainer.Size = new System.Drawing.Size(20, 20);
			this.popupContainer.TabIndex = 2;
			this.popupContainer.Visible = false;
			this.glrViewTypes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.glrViewTypes.Controls.Add(this.galleryControlClient1);
			this.glrViewTypes.DesignGalleryGroupIndex = 0;
			this.glrViewTypes.DesignGalleryItemIndex = 0;
			this.glrViewTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.glrViewTypes.Gallery.AllowFilter = false;
			this.glrViewTypes.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
			this.glrViewTypes.Gallery.FixedImageSize = false;
			this.glrViewTypes.Gallery.RowCount = 5;
			this.glrViewTypes.Gallery.ShowGroupCaption = false;
			this.glrViewTypes.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			this.glrViewTypes.Gallery.ItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.galleryControlGallery1_ItemClick);
			this.glrViewTypes.Location = new System.Drawing.Point(0, 0);
			this.glrViewTypes.Margin = new System.Windows.Forms.Padding(0);
			this.glrViewTypes.Name = "glrViewTypes";
			this.glrViewTypes.Size = new System.Drawing.Size(20, 20);
			this.glrViewTypes.TabIndex = 80;
			this.glrViewTypes.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.glrViewTypes;
			this.galleryControlClient1.Location = new System.Drawing.Point(1, 1);
			this.galleryControlClient1.Size = new System.Drawing.Size(100, 100);
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(34, 0);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 34);
			this.barDockControlBottom.Size = new System.Drawing.Size(34, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 34);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(34, 0);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 34);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.popupContainer);
			this.Controls.Add(this.dropDownButton);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "EnumGalleryControl";
			this.Size = new System.Drawing.Size(34, 34);
			((System.ComponentModel.ISupportInitialize)(this.popupContainer)).EndInit();
			this.popupContainer.ResumeLayout(false);
			this.popupContainer.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.glrViewTypes)).EndInit();
			this.glrViewTypes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DropDownButtonFlat dropDownButton;
		private XtraBars.PopupControlContainer popupContainer;
		private XtraBars.BarManager barManager1;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraBars.Ribbon.GalleryControl glrViewTypes;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
	}
}
