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

namespace DevExpress.Utils.Design {
	partial class DXImageGalleryControl {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DXImageGalleryControl));
			this.pnlGalleryHeader = new DevExpress.XtraEditors.PanelControl();
			this.beSearchBox = new DevExpress.XtraEditors.ButtonEdit();
			this.lblCategories = new DevExpress.XtraEditors.LabelControl();
			this.pnlGalleryContent = new DevExpress.XtraEditors.PanelControl();
			this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lbCollections = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.lblCollection = new DevExpress.XtraEditors.LabelControl();
			this.lbSizes = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.lblSize = new DevExpress.XtraEditors.LabelControl();
			this.lbCategories = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.galleryControl = new DevExpress.XtraBars.Ribbon.GalleryControl();
			this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
			this.imgCol = new DevExpress.Utils.ImageCollection(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pnlGalleryHeader)).BeginInit();
			this.pnlGalleryHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.beSearchBox.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGalleryContent)).BeginInit();
			this.pnlGalleryContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
			this.splitContainerControl.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbCollections)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbSizes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryControl)).BeginInit();
			this.galleryControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgCol)).BeginInit();
			this.SuspendLayout();
			this.pnlGalleryHeader.Appearance.BackColor = System.Drawing.Color.White;
			this.pnlGalleryHeader.Appearance.Options.UseBackColor = true;
			this.pnlGalleryHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGalleryHeader.Controls.Add(this.beSearchBox);
			this.pnlGalleryHeader.Controls.Add(this.lblCategories);
			this.pnlGalleryHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlGalleryHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlGalleryHeader.Name = "pnlGalleryHeader";
			this.pnlGalleryHeader.Size = new System.Drawing.Size(671, 43);
			this.pnlGalleryHeader.TabIndex = 1;
			this.beSearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.beSearchBox.Location = new System.Drawing.Point(507, 10);
			this.beSearchBox.Name = "beSearchBox";
			this.beSearchBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "test", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
			this.beSearchBox.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnSearchBoxButtonClick);
			this.beSearchBox.Size = new System.Drawing.Size(164, 20);
			this.beSearchBox.TabIndex = 1;
			this.beSearchBox.EditValueChanged += new System.EventHandler(this.OnSearchBoxEditValueChanged);
			this.lblCategories.Location = new System.Drawing.Point(0, 24);
			this.lblCategories.Name = "lblCategories";
			this.lblCategories.Size = new System.Drawing.Size(52, 13);
			this.lblCategories.TabIndex = 0;
			this.lblCategories.Text = "Categories";
			this.pnlGalleryContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGalleryContent.Controls.Add(this.splitContainerControl);
			this.pnlGalleryContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlGalleryContent.Location = new System.Drawing.Point(0, 43);
			this.pnlGalleryContent.Name = "pnlGalleryContent";
			this.pnlGalleryContent.Size = new System.Drawing.Size(671, 358);
			this.pnlGalleryContent.TabIndex = 2;
			this.splitContainerControl.Appearance.BackColor = System.Drawing.Color.White;
			this.splitContainerControl.Appearance.Options.UseBackColor = true;
			this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl.Name = "splitContainerControl";
			this.splitContainerControl.Panel1.Controls.Add(this.tableLayoutPanel1);
			this.splitContainerControl.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.splitContainerControl.Panel1.Text = "Panel1";
			this.splitContainerControl.Panel2.Controls.Add(this.galleryControl);
			this.splitContainerControl.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.splitContainerControl.Panel2.Text = "Panel2";
			this.splitContainerControl.Size = new System.Drawing.Size(671, 358);
			this.splitContainerControl.SplitterPosition = 205;
			this.splitContainerControl.TabIndex = 0;
			this.splitContainerControl.Text = "splitContainerControl";
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lbCollections, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.lblCollection, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.lbSizes, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblSize, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lbCategories, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.58055F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.389918F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.71485F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.454903F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.85978F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(202, 358);
			this.tableLayoutPanel1.TabIndex = 2;
			this.lbCollections.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbCollections.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(0, "Colored"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(1, "Grayscale"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(3, "DevAV"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(2, "Office 2013")});
			this.lbCollections.Location = new System.Drawing.Point(0, 285);
			this.lbCollections.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.lbCollections.Name = "lbCollections";
			this.lbCollections.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbCollections.Size = new System.Drawing.Size(202, 73);
			this.lbCollections.TabIndex = 11;
			this.lbCollections.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.OnCollectionListItemCheck);
			this.lblCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCollection.Location = new System.Drawing.Point(0, 267);
			this.lblCollection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.lblCollection.Name = "lblCollection";
			this.lblCollection.Size = new System.Drawing.Size(46, 13);
			this.lblCollection.TabIndex = 7;
			this.lblCollection.Text = "Collection";
			this.lbSizes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbSizes.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(0, "16x16"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(1, "32x32")});
			this.lbSizes.Location = new System.Drawing.Point(0, 221);
			this.lbSizes.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.lbSizes.Name = "lbSizes";
			this.lbSizes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbSizes.Size = new System.Drawing.Size(202, 37);
			this.lbSizes.TabIndex = 10;
			this.lbSizes.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.OnSizeListItemCheck);
			this.lblSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSize.Location = new System.Drawing.Point(0, 203);
			this.lblSize.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.lblSize.Name = "lblSize";
			this.lblSize.Size = new System.Drawing.Size(19, 13);
			this.lblSize.TabIndex = 8;
			this.lblSize.Text = "Size";
			this.lbCategories.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbCategories.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "SomeCategory", System.Windows.Forms.CheckState.Checked)});
			this.lbCategories.Location = new System.Drawing.Point(0, 0);
			this.lbCategories.Margin = new System.Windows.Forms.Padding(0);
			this.lbCategories.Name = "lbCategories";
			this.lbCategories.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbCategories.Size = new System.Drawing.Size(202, 195);
			this.lbCategories.TabIndex = 9;
			this.lbCategories.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.OnCategoryItemCheck);
			this.galleryControl.Controls.Add(this.galleryControlClient1);
			this.galleryControl.DesignGalleryGroupIndex = 0;
			this.galleryControl.DesignGalleryItemIndex = 0;
			this.galleryControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.galleryControl.Gallery.AllowFilter = false;
			this.galleryControl.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
			this.galleryControl.Gallery.ItemDoubleClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(this.OnGalleryControlItemDoubleClick);
			this.galleryControl.Gallery.ItemCheckedChanged += new DevExpress.XtraBars.Ribbon.GalleryItemEventHandler(this.OnGalleryControlGalleryItemCheckedChanged);
			this.galleryControl.Location = new System.Drawing.Point(3, 0);
			this.galleryControl.LookAndFeel.UseDefaultLookAndFeel = false;
			this.galleryControl.Name = "galleryControl";
			this.galleryControl.Size = new System.Drawing.Size(458, 358);
			this.galleryControl.TabIndex = 0;
			this.galleryControl.Text = "galleryControl1";
			this.galleryControlClient1.GalleryControl = this.galleryControl;
			this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
			this.galleryControlClient1.Size = new System.Drawing.Size(437, 354);
			this.imgCol.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgCol.ImageStream")));
			this.imgCol.Images.SetKeyName(0, "SearchIcon.png");
			this.imgCol.Images.SetKeyName(1, "SearchCancelIcon.png");
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlGalleryContent);
			this.Controls.Add(this.pnlGalleryHeader);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "DXImageGalleryControl";
			this.Size = new System.Drawing.Size(671, 401);
			((System.ComponentModel.ISupportInitialize)(this.pnlGalleryHeader)).EndInit();
			this.pnlGalleryHeader.ResumeLayout(false);
			this.pnlGalleryHeader.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.beSearchBox.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGalleryContent)).EndInit();
			this.pnlGalleryContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
			this.splitContainerControl.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbCollections)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbSizes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCategories)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.galleryControl)).EndInit();
			this.galleryControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imgCol)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.PanelControl pnlGalleryHeader;
		private XtraEditors.ButtonEdit beSearchBox;
		private XtraEditors.PanelControl pnlGalleryContent;
		private XtraEditors.SplitContainerControl splitContainerControl;
		private XtraBars.Ribbon.GalleryControl galleryControl;
		private XtraBars.Ribbon.GalleryControlClient galleryControlClient1;
		private XtraEditors.LabelControl lblCategories;
		private XtraEditors.CheckedListBoxControl lbCategories;
		private XtraEditors.LabelControl lblSize;
		private XtraEditors.CheckedListBoxControl lbSizes;
		private XtraEditors.LabelControl lblCollection;
		private XtraEditors.CheckedListBoxControl lbCollections;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ImageCollection imgCol;
	}
}
