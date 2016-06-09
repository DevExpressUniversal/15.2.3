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

namespace DevExpress.XtraEditors.Frames {
	partial class SimpleRuleBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcPreviewFormat = new DevExpress.XtraLayout.LayoutControl();
			this.pnlFormatSetting = new DevExpress.XtraEditors.PanelControl();
			this.btnFormat = new DevExpress.XtraEditors.SimpleButton();
			this.pctPreview = new DevExpress.XtraEditors.PictureEdit();
			this.lcgPreviewFormat = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciFormat = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciPreview = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.lcPreviewFormat)).BeginInit();
			this.lcPreviewFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatSetting)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgPreviewFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFormat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
			this.SuspendLayout();
			this.lcPreviewFormat.AllowCustomization = false;
			this.lcPreviewFormat.Controls.Add(this.pnlFormatSetting);
			this.lcPreviewFormat.Controls.Add(this.btnFormat);
			this.lcPreviewFormat.Controls.Add(this.pctPreview);
			this.lcPreviewFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcPreviewFormat.Location = new System.Drawing.Point(0, 0);
			this.lcPreviewFormat.Name = "lcPreviewFormat";
			this.lcPreviewFormat.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(850, 137, 861, 612);
			this.lcPreviewFormat.Root = this.lcgPreviewFormat;
			this.lcPreviewFormat.Size = new System.Drawing.Size(470, 141);
			this.lcPreviewFormat.TabIndex = 0;
			this.lcPreviewFormat.Text = "layoutControl1";
			this.pnlFormatSetting.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFormatSetting.Location = new System.Drawing.Point(4, 4);
			this.pnlFormatSetting.Name = "pnlFormatSetting";
			this.pnlFormatSetting.Size = new System.Drawing.Size(462, 63);
			this.pnlFormatSetting.TabIndex = 1;
			this.btnFormat.Location = new System.Drawing.Point(317, 94);
			this.btnFormat.Name = "btnFormat";
			this.btnFormat.Size = new System.Drawing.Size(70, 22);
			this.btnFormat.StyleController = this.lcPreviewFormat;
			this.btnFormat.TabIndex = 5;
			this.btnFormat.Text = "Format...";
			this.btnFormat.Click += new System.EventHandler(this.btnFormatForm_Click);
			this.pctPreview.Location = new System.Drawing.Point(59, 81);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Properties.ShowMenu = false;
			this.pctPreview.Size = new System.Drawing.Size(203, 46);
			this.pctPreview.StyleController = this.lcPreviewFormat;
			this.pctPreview.TabIndex = 4;
			this.pctPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pctPreview_Paint);
			this.lcgPreviewFormat.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgPreviewFormat.GroupBordersVisible = false;
			this.lcgPreviewFormat.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciFormat,
			this.emptySpaceItem1,
			this.emptySpaceItem2,
			this.lciPreview,
			this.emptySpaceItem4,
			this.emptySpaceItem3,
			this.layoutControlItem1,
			this.emptySpaceItem5,
			this.emptySpaceItem6});
			this.lcgPreviewFormat.Location = new System.Drawing.Point(0, 0);
			this.lcgPreviewFormat.Name = "Root";
			this.lcgPreviewFormat.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lcgPreviewFormat.Size = new System.Drawing.Size(470, 141);
			this.lcgPreviewFormat.TextVisible = false;
			this.lciFormat.Control = this.btnFormat;
			this.lciFormat.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.lciFormat.Location = new System.Drawing.Point(315, 92);
			this.lciFormat.Name = "lciFormat";
			this.lciFormat.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lciFormat.Size = new System.Drawing.Size(70, 22);
			this.lciFormat.TextSize = new System.Drawing.Size(0, 0);
			this.lciFormat.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(385, 77);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(81, 50);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(315, 77);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem2.Size = new System.Drawing.Size(70, 15);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.lciPreview.Control = this.pctPreview;
			this.lciPreview.Location = new System.Drawing.Point(0, 77);
			this.lciPreview.Name = "lciPreview";
			this.lciPreview.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 2, 2);
			this.lciPreview.Size = new System.Drawing.Size(262, 50);
			this.lciPreview.Text = "Preview:";
			this.lciPreview.TextSize = new System.Drawing.Size(42, 13);
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(315, 114);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem4.Size = new System.Drawing.Size(70, 13);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(262, 77);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(53, 50);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.Control = this.pnlFormatSetting;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(466, 67);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.emptySpaceItem5.AllowHotTrack = false;
			this.emptySpaceItem5.Location = new System.Drawing.Point(0, 67);
			this.emptySpaceItem5.Name = "emptySpaceItem5";
			this.emptySpaceItem5.Size = new System.Drawing.Size(466, 10);
			this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem6.AllowHotTrack = false;
			this.emptySpaceItem6.Location = new System.Drawing.Point(0, 127);
			this.emptySpaceItem6.Name = "emptySpaceItem6";
			this.emptySpaceItem6.Size = new System.Drawing.Size(466, 10);
			this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcPreviewFormat);
			this.Name = "SimpleRuleBase";
			this.Size = new System.Drawing.Size(470, 141);
			((System.ComponentModel.ISupportInitialize)(this.lcPreviewFormat)).EndInit();
			this.lcPreviewFormat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatSetting)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgPreviewFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFormat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcPreviewFormat;
		private DevExpress.XtraLayout.LayoutControlGroup lcgPreviewFormat;
		private DevExpress.XtraEditors.PanelControl pnlFormatSetting;
		private DevExpress.XtraEditors.SimpleButton btnFormat;
		private DevExpress.XtraEditors.PictureEdit pctPreview;
		private DevExpress.XtraLayout.LayoutControlItem lciPreview;
		private DevExpress.XtraLayout.LayoutControlItem lciFormat;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem5;
		private XtraLayout.EmptySpaceItem emptySpaceItem6;
	}
}
