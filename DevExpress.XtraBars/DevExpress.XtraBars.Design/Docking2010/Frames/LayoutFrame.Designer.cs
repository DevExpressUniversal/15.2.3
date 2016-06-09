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
	partial class LayoutFrame {
		#region Component Designer generated code
		private void InitializeComponent() {
			this.pnlTop = new DevExpress.XtraEditors.PanelControl();
			this.btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
			this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.pnlPreview = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).BeginInit();
			this.pnlTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
			this.pnlBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(784, 42);
			this.pnlMain.Controls.Add(this.pnlPreview);
			this.pnlMain.Controls.Add(this.pnlBottom);
			this.pnlMain.Controls.Add(this.pnlTop);
			this.pnlMain.Location = new System.Drawing.Point(0, 46);
			this.pnlMain.Size = new System.Drawing.Size(784, 427);
			this.horzSplitter.Location = new System.Drawing.Point(0, 42);
			this.horzSplitter.Size = new System.Drawing.Size(784, 4);
			this.pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTop.Controls.Add(this.btnSave);
			this.pnlTop.Controls.Add(this.btnLoad);
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(784, 54);
			this.pnlTop.TabIndex = 4;
			this.btnSave.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnSave.Location = new System.Drawing.Point(142, 12);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(132, 30);
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "Sa&ve Layout...";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnLoad.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnLoad.Location = new System.Drawing.Point(0, 12);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(132, 30);
			this.btnLoad.TabIndex = 2;
			this.btnLoad.Text = "&Load Layout...";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBottom.Controls.Add(this.btnApply);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 373);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(784, 54);
			this.pnlBottom.TabIndex = 3;
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Location = new System.Drawing.Point(669, 12);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(115, 30);
			this.btnApply.TabIndex = 1;
			this.btnApply.Text = "&Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.pnlPreview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlPreview.Location = new System.Drawing.Point(0, 54);
			this.pnlPreview.Name = "pnlPreview";
			this.pnlPreview.Size = new System.Drawing.Size(784, 319);
			this.pnlPreview.TabIndex = 2;
			this.Name = "LayoutFrame";
			this.Size = new System.Drawing.Size(784, 473);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).EndInit();
			this.pnlTop.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnLoad;
		private DevExpress.XtraEditors.PanelControl pnlBottom;
		private DevExpress.XtraEditors.SimpleButton btnSave;
		protected DevExpress.XtraEditors.PanelControl pnlPreview;
		protected DevExpress.XtraEditors.SimpleButton btnApply;
		protected internal XtraEditors.PanelControl pnlTop;
	}
}
