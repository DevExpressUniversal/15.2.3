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

using System.ComponentModel;
namespace DevExpress.XtraBars.Design.Frames {
	partial class TableGroupLayoutFrame {
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableGroupLayoutFrame));
			this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.pnlTab = new System.Windows.Forms.Panel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.pnlTab.SuspendLayout();
			this.SuspendLayout();
			this.pnlPreview.Size = new System.Drawing.Size(429, 319);
			this.pnlTop.Size = new System.Drawing.Size(429, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.pnlMain.Controls.Add(this.splitterControl1);
			this.pnlMain.Controls.Add(this.pnlTab);
			this.pnlMain.Controls.SetChildIndex(this.pnlTab, 0);
			this.pnlMain.Controls.SetChildIndex(this.splitterControl1, 0);
			this.pnlMain.Controls.SetChildIndex(this.pnlTop, 0);
			this.pnlMain.Controls.SetChildIndex(this.pnlPreview, 0);
			this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitterControl1.Location = new System.Drawing.Point(429, 0);
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.Size = new System.Drawing.Size(5, 373);
			this.splitterControl1.TabIndex = 1;
			this.splitterControl1.TabStop = false;
			this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.xtraTabControl1.Location = new System.Drawing.Point(0, 12);
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.Size = new System.Drawing.Size(350, 361);
			this.xtraTabControl1.TabIndex = 0;
			this.pnlTab.Controls.Add(this.xtraTabControl1);
			this.pnlTab.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlTab.Location = new System.Drawing.Point(434, 0);
			this.pnlTab.MinimumSize = new System.Drawing.Size(250, 0);
			this.pnlTab.Name = "pnlTab";
			this.pnlTab.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this.pnlTab.Size = new System.Drawing.Size(350, 373);
			this.pnlTab.TabIndex = 0;
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Document_16x16.png");
			this.imageList1.Images.SetKeyName(1, "Tile_16x16.png");
			this.Name = "TableGroupLayoutFrame";
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.pnlTab.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraTab.XtraTabControl xtraTabControl1;
		private XtraEditors.SplitterControl splitterControl1;
		private System.Windows.Forms.Panel pnlTab;
		private System.Windows.Forms.ImageList imageList1;
		private IContainer components;
	}
}
