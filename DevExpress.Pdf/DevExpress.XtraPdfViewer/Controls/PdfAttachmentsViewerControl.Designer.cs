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

namespace DevExpress.XtraPdfViewer.Controls {
	partial class PdfAttachmentsViewerControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfAttachmentsViewerControl));
			this.attachmentsViewerBar = new DevExpress.XtraBars.Bar();
			this.attachmentsViewerPreviewBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
			this.attachmentsViewerSaveBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
			this.attachmentsViewerBarAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.attachmentsViewerManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.attachmentsImageCollection = new DevExpress.Utils.ImageCollection(this.components);
			this.attachmentsListBoxControl = new DevExpress.XtraEditors.ImageListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsViewerBarAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsViewerManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsImageCollection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsListBoxControl)).BeginInit();
			this.SuspendLayout();
			this.attachmentsViewerBar.BarName = "Tools";
			this.attachmentsViewerBar.DockCol = 0;
			this.attachmentsViewerBar.DockRow = 0;
			this.attachmentsViewerBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.attachmentsViewerBar.FloatLocation = new System.Drawing.Point(326, 134);
			this.attachmentsViewerBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.attachmentsViewerPreviewBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.attachmentsViewerSaveBarButtonItem)});
			this.attachmentsViewerBar.OptionsBar.DrawBorder = false;
			this.attachmentsViewerBar.OptionsBar.UseWholeRow = true;
			this.attachmentsViewerBar.Text = "Tools";
			this.attachmentsViewerPreviewBarButtonItem.Hint = "Open file in its native application";
			this.attachmentsViewerPreviewBarButtonItem.Id = 0;
			this.attachmentsViewerPreviewBarButtonItem.ImageIndex = 0;
			this.attachmentsViewerPreviewBarButtonItem.Name = "attachmentsViewerPreviewBarButtonItem";
			this.attachmentsViewerPreviewBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.AttachmentsViewerPreviewBarButtonItemItemClick);
			this.attachmentsViewerSaveBarButtonItem.Hint = "Save attachment";
			this.attachmentsViewerSaveBarButtonItem.Id = 1;
			this.attachmentsViewerSaveBarButtonItem.ImageIndex = 1;
			this.attachmentsViewerSaveBarButtonItem.Name = "attachmentsViewerSaveBarButtonItem";
			this.attachmentsViewerSaveBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.AttachmentsViewerSaveBarButtonItemItemClick);
			this.attachmentsViewerBarAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.attachmentsViewerBarAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.attachmentsViewerManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.attachmentsViewerBar});
			this.attachmentsViewerManager.Controller = this.attachmentsViewerBarAndDockingController;
			this.attachmentsViewerManager.DockControls.Add(this.barDockControlTop);
			this.attachmentsViewerManager.DockControls.Add(this.barDockControlBottom);
			this.attachmentsViewerManager.DockControls.Add(this.barDockControlLeft);
			this.attachmentsViewerManager.DockControls.Add(this.barDockControlRight);
			this.attachmentsViewerManager.Form = this;
			this.attachmentsViewerManager.Images = this.attachmentsImageCollection;
			this.attachmentsViewerManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.attachmentsViewerPreviewBarButtonItem,
			this.attachmentsViewerSaveBarButtonItem});
			this.attachmentsViewerManager.MaxItemId = 2;
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(150, 31);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 150);
			this.barDockControlBottom.Size = new System.Drawing.Size(150, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 119);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(150, 31);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 119);
			this.attachmentsImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("attachmentsImageCollection.ImageStream")));
			this.attachmentsImageCollection.Images.SetKeyName(0, "Preview_16x16.png");
			this.attachmentsImageCollection.Images.SetKeyName(1, "Save_16x16.png");
			this.attachmentsListBoxControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.attachmentsListBoxControl.DisplayMember = "FileName";
			this.attachmentsListBoxControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.attachmentsListBoxControl.ImageIndexMember = "ImageIndex";
			this.attachmentsListBoxControl.ItemAutoHeight = true;
			this.attachmentsListBoxControl.Location = new System.Drawing.Point(0, 31);
			this.attachmentsListBoxControl.Name = "attachmentsListBoxControl";
			this.attachmentsListBoxControl.Size = new System.Drawing.Size(150, 119);
			this.attachmentsListBoxControl.TabIndex = 9;
			this.attachmentsListBoxControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AttachmentsListBoxControlKeyDown);
			this.attachmentsListBoxControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.AttachmentsListBoxControlMouseDoubleClick);
			this.attachmentsListBoxControl.MouseLeave += new System.EventHandler(this.AttachmentsListBoxControlMouseLeave);
			this.attachmentsListBoxControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AttachmentsListBoxControlMouseMove);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.attachmentsListBoxControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "PdfAttachmentsViewerControl";
			((System.ComponentModel.ISupportInitialize)(this.attachmentsViewerBarAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsViewerManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsImageCollection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.attachmentsListBoxControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraBars.BarAndDockingController attachmentsViewerBarAndDockingController;
		private XtraBars.BarManager attachmentsViewerManager;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraBars.BarButtonItem attachmentsViewerPreviewBarButtonItem;
		private XtraBars.BarButtonItem attachmentsViewerSaveBarButtonItem;
		private XtraEditors.ImageListBoxControl attachmentsListBoxControl;
		private XtraBars.Bar attachmentsViewerBar;
		private Utils.ImageCollection attachmentsImageCollection;
	}
}
