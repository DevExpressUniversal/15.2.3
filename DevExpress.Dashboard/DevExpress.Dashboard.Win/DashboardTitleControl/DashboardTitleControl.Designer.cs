#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	partial class DashboardTitleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				filterToolTip.RequestScreenHeight -= OnRequestScreenHeight;
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
			this.pnlFilterImage = new DevExpress.XtraEditors.PanelControl();
			this.pbDashboardTitleImage = new System.Windows.Forms.PictureBox();
			this.lblDashboardTitle = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFilterImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbDashboardTitleImage)).BeginInit();
			this.SuspendLayout();
			this.pnlButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlButtons.Location = new System.Drawing.Point(420, 0);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(62, 76);
			this.pnlButtons.TabIndex = 3;
			this.pnlFilterImage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pnlFilterImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFilterImage.Location = new System.Drawing.Point(301, 0);
			this.pnlFilterImage.Name = "pnlFilterImage";
			this.pnlFilterImage.Size = new System.Drawing.Size(119, 76);
			this.pnlFilterImage.TabIndex = 4;
			this.pnlFilterImage.Visible = false;
			this.pbDashboardTitleImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pbDashboardTitleImage.Location = new System.Drawing.Point(60, 0);
			this.pbDashboardTitleImage.Name = "pbDashboardTitleImage";
			this.pbDashboardTitleImage.Size = new System.Drawing.Size(100, 76);
			this.pbDashboardTitleImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pbDashboardTitleImage.TabIndex = 6;
			this.pbDashboardTitleImage.TabStop = false;
			this.pbDashboardTitleImage.Visible = false;
			this.lblDashboardTitle.AllowHtmlString = true;
			this.lblDashboardTitle.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
			this.lblDashboardTitle.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lblDashboardTitle.AutoEllipsis = true;
			this.lblDashboardTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblDashboardTitle.Location = new System.Drawing.Point(177, 104);
			this.lblDashboardTitle.Name = "lblDashboardTitle";
			this.lblDashboardTitle.Size = new System.Drawing.Size(63, 13);
			this.lblDashboardTitle.TabIndex = 5;
			this.lblDashboardTitle.UseMnemonic = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlFilterImage);
			this.Controls.Add(this.pnlButtons);
			this.Controls.Add(this.lblDashboardTitle);
			this.Controls.Add(this.pbDashboardTitleImage);
			this.Name = "DashboardTitleControl";
			this.Size = new System.Drawing.Size(482, 76);
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFilterImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbDashboardTitleImage)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private PanelControl pnlButtons;
		private PanelControl pnlFilterImage;
		private System.Windows.Forms.PictureBox pbDashboardTitleImage;
		private LabelControl lblDashboardTitle;
	}
}
