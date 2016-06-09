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

namespace DevExpress.XtraBars.Design {
	partial class TileItemElementsCollectionEditorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileItemElementsCollectionEditorControl));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.tileControl = new DevExpress.XtraEditors.TileControl();
			this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.listBox = new DevExpress.XtraEditors.ImageListBoxControl();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			this.btnDownCmd = new DevExpress.XtraEditors.SimpleButton();
			this.imageCollection = new DevExpress.Utils.ImageCollection(this.components);
			this.btnUpCmd = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.btnRemoveElement = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddElement = new DevExpress.XtraEditors.SimpleButton();
			this.propertyGrid = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			this.listBoxImageCollection = new DevExpress.Utils.ImageCollection(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
			this.splitContainerControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxImageCollection)).BeginInit();
			this.SuspendLayout();
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Horizontal = false;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.labelControl2);
			this.splitContainerControl1.Panel1.Controls.Add(this.tileControl);
			this.splitContainerControl1.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(566, 466);
			this.splitContainerControl1.SplitterPosition = 242;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelControl2.LineVisible = true;
			this.labelControl2.Location = new System.Drawing.Point(0, 236);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(566, 3);
			this.labelControl2.TabIndex = 10;
			this.tileControl.BackColor = System.Drawing.Color.Green;
			this.tileControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileControl.DragSize = new System.Drawing.Size(0, 0);
			this.tileControl.Location = new System.Drawing.Point(0, 0);
			this.tileControl.Name = "tileControl";
			this.tileControl.Size = new System.Drawing.Size(566, 239);
			this.tileControl.TabIndex = 1;
			this.tileControl.Text = "tileControl1";
			this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl2.Name = "splitContainerControl2";
			this.splitContainerControl2.Panel1.Controls.Add(this.panelControl1);
			this.splitContainerControl2.Panel1.Controls.Add(this.panelControl2);
			this.splitContainerControl2.Panel1.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
			this.splitContainerControl2.Panel1.Text = "Panel1";
			this.splitContainerControl2.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainerControl2.Panel2.Padding = new System.Windows.Forms.Padding(0, 4, 12, 0);
			this.splitContainerControl2.Panel2.Text = "Panel2";
			this.splitContainerControl2.Size = new System.Drawing.Size(566, 219);
			this.splitContainerControl2.SplitterPosition = 237;
			this.splitContainerControl2.TabIndex = 0;
			this.splitContainerControl2.Text = "splitContainerControl2";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.listBox);
			this.panelControl1.Controls.Add(this.panelControl3);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(4, 4);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Padding = new System.Windows.Forms.Padding(8, 26, 4, 4);
			this.panelControl1.Size = new System.Drawing.Size(233, 190);
			this.panelControl1.TabIndex = 8;
			this.labelControl1.Location = new System.Drawing.Point(8, 7);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(66, 13);
			this.labelControl1.TabIndex = 8;
			this.labelControl1.Text = "Tile Elements:";
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.Location = new System.Drawing.Point(8, 26);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(195, 160);
			this.listBox.TabIndex = 6;
			this.listBox.SelectedValueChanged += new System.EventHandler(this.listBox_SelectedValueChanged);
			this.listBox.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.ListBox_DrawItem);
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.Add(this.btnDownCmd);
			this.panelControl3.Controls.Add(this.btnUpCmd);
			this.panelControl3.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelControl3.Location = new System.Drawing.Point(203, 26);
			this.panelControl3.Name = "panelControl3";
			this.panelControl3.Size = new System.Drawing.Size(26, 160);
			this.panelControl3.TabIndex = 7;
			this.btnDownCmd.ImageIndex = 0;
			this.btnDownCmd.ImageList = this.imageCollection;
			this.btnDownCmd.Location = new System.Drawing.Point(2, 57);
			this.btnDownCmd.Name = "btnDownCmd";
			this.btnDownCmd.Size = new System.Drawing.Size(24, 34);
			this.btnDownCmd.TabIndex = 6;
			this.btnDownCmd.Click += new System.EventHandler(this.btnDownCmd_Click);
			this.imageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection.ImageStream")));
			this.imageCollection.Images.SetKeyName(0, "Next_16x16.png");
			this.imageCollection.Images.SetKeyName(1, "Prev_16x16.png");
			this.imageCollection.Images.SetKeyName(2, "Delete_16x16.png");
			this.btnUpCmd.ImageIndex = 1;
			this.btnUpCmd.ImageList = this.imageCollection;
			this.btnUpCmd.Location = new System.Drawing.Point(2, 0);
			this.btnUpCmd.Name = "btnUpCmd";
			this.btnUpCmd.Size = new System.Drawing.Size(24, 34);
			this.btnUpCmd.TabIndex = 5;
			this.btnUpCmd.Click += new System.EventHandler(this.btnUpCmd_Click);
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.btnRemoveElement);
			this.panelControl2.Controls.Add(this.btnAddElement);
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl2.Location = new System.Drawing.Point(4, 194);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Padding = new System.Windows.Forms.Padding(0, 0, 32, 0);
			this.panelControl2.Size = new System.Drawing.Size(233, 25);
			this.panelControl2.TabIndex = 7;
			this.btnRemoveElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveElement.ImageIndex = 2;
			this.btnRemoveElement.ImageList = this.imageCollection;
			this.btnRemoveElement.Location = new System.Drawing.Point(108, 2);
			this.btnRemoveElement.Name = "btnRemoveElement";
			this.btnRemoveElement.Size = new System.Drawing.Size(95, 23);
			this.btnRemoveElement.TabIndex = 6;
			this.btnRemoveElement.Text = "Remove";
			this.btnRemoveElement.Click += new System.EventHandler(this.btnRemoveElement_Click);
			this.btnAddElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddElement.Location = new System.Drawing.Point(8, 2);
			this.btnAddElement.Name = "btnAddElement";
			this.btnAddElement.Size = new System.Drawing.Size(95, 23);
			this.btnAddElement.TabIndex = 5;
			this.btnAddElement.Text = "New Element";
			this.btnAddElement.Click += new System.EventHandler(this.btnAddElement_Click);
			this.propertyGrid.CommandsVisibleIfAvailable = false;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.DrawFlat = false;
			this.propertyGrid.HelpVisible = false;
			this.propertyGrid.Location = new System.Drawing.Point(0, 4);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ShowSearchPanel = false;
			this.propertyGrid.Size = new System.Drawing.Size(312, 215);
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			this.listBoxImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("listBoxImageCollection.ImageStream")));
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerControl1);
			this.Name = "TileItemElementsCollectionEditorControl";
			this.Size = new System.Drawing.Size(566, 466);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
			this.splitContainerControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imageCollection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxImageCollection)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx propertyGrid;
		private DevExpress.XtraEditors.TileControl tileControl;
		private DevExpress.Utils.ImageCollection imageCollection;
		private DevExpress.Utils.ImageCollection listBoxImageCollection;
		private DevExpress.XtraEditors.SimpleButton btnDownCmd;
		private DevExpress.XtraEditors.SimpleButton btnUpCmd;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		protected XtraEditors.SplitContainerControl splitContainerControl1;
		protected XtraEditors.SplitContainerControl splitContainerControl2;
		protected XtraEditors.ImageListBoxControl listBox;
		protected XtraEditors.PanelControl panelControl2;
		protected XtraEditors.PanelControl panelControl1;
		protected XtraEditors.PanelControl panelControl3;
		protected XtraEditors.SimpleButton btnRemoveElement;
		protected XtraEditors.SimpleButton btnAddElement;
	}
}
