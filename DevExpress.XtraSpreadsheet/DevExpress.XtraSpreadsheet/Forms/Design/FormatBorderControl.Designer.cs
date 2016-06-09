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

namespace DevExpress.XtraSpreadsheet.Forms.Design {
	partial class FormatBorderControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatBorderControl));
			this.chkDiagonalDownBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkRightBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkInsideVerticalBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkLeftBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkDiagonalUpBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkBottomBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkInsideHorizontalBorder = new DevExpress.XtraEditors.CheckButton();
			this.chkTopBorder = new DevExpress.XtraEditors.CheckButton();
			this.drawPanel = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.drawPanel)).BeginInit();
			this.SuspendLayout();
			this.chkDiagonalDownBorder.AccessibleName = "Diagonal Down Border";
			this.chkDiagonalDownBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkDiagonalDownBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkDiagonalDownBorder.Image")));
			this.chkDiagonalDownBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkDiagonalDownBorder.Location = new System.Drawing.Point(215, 113);
			this.chkDiagonalDownBorder.Name = "chkDiagonalDownBorder";
			this.chkDiagonalDownBorder.Size = new System.Drawing.Size(26, 22);
			this.chkDiagonalDownBorder.TabIndex = 6;
			this.chkRightBorder.AccessibleName = "Right Border";
			this.chkRightBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkRightBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkRightBorder.Image")));
			this.chkRightBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkRightBorder.Location = new System.Drawing.Point(181, 113);
			this.chkRightBorder.Name = "chkRightBorder";
			this.chkRightBorder.Size = new System.Drawing.Size(26, 22);
			this.chkRightBorder.TabIndex = 3;
			this.chkInsideVerticalBorder.AccessibleName = "Inside Vertical Border";
			this.chkInsideVerticalBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkInsideVerticalBorder.Enabled = false;
			this.chkInsideVerticalBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkInsideVerticalBorder.Image")));
			this.chkInsideVerticalBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkInsideVerticalBorder.Location = new System.Drawing.Point(109, 113);
			this.chkInsideVerticalBorder.Name = "chkInsideVerticalBorder";
			this.chkInsideVerticalBorder.Size = new System.Drawing.Size(26, 22);
			this.chkInsideVerticalBorder.TabIndex = 4;
			this.chkLeftBorder.AccessibleName = "Left Border";
			this.chkLeftBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkLeftBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkLeftBorder.Image")));
			this.chkLeftBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkLeftBorder.Location = new System.Drawing.Point(37, 113);
			this.chkLeftBorder.Name = "chkLeftBorder";
			this.chkLeftBorder.Size = new System.Drawing.Size(26, 22);
			this.chkLeftBorder.TabIndex = 0;
			this.chkDiagonalUpBorder.AccessibleName = "Diagonal Up Border";
			this.chkDiagonalUpBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkDiagonalUpBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkDiagonalUpBorder.Image")));
			this.chkDiagonalUpBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkDiagonalUpBorder.Location = new System.Drawing.Point(3, 113);
			this.chkDiagonalUpBorder.Name = "chkDiagonalUpBorder";
			this.chkDiagonalUpBorder.Size = new System.Drawing.Size(26, 22);
			this.chkDiagonalUpBorder.TabIndex = 7;
			this.chkBottomBorder.AccessibleName = "Bottom Border";
			this.chkBottomBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkBottomBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkBottomBorder.Image")));
			this.chkBottomBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkBottomBorder.Location = new System.Drawing.Point(3, 85);
			this.chkBottomBorder.Name = "chkBottomBorder";
			this.chkBottomBorder.Size = new System.Drawing.Size(26, 22);
			this.chkBottomBorder.TabIndex = 2;
			this.chkInsideHorizontalBorder.AccessibleName = "Horizontal Vertical Border";
			this.chkInsideHorizontalBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkInsideHorizontalBorder.Enabled = false;
			this.chkInsideHorizontalBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkInsideHorizontalBorder.Image")));
			this.chkInsideHorizontalBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkInsideHorizontalBorder.Location = new System.Drawing.Point(3, 45);
			this.chkInsideHorizontalBorder.Name = "chkInsideHorizontalBorder";
			this.chkInsideHorizontalBorder.Size = new System.Drawing.Size(26, 22);
			this.chkInsideHorizontalBorder.TabIndex = 5;
			this.chkTopBorder.AccessibleName = "Top Border";
			this.chkTopBorder.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkTopBorder.Image = ((System.Drawing.Image)(resources.GetObject("chkTopBorder.Image")));
			this.chkTopBorder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.chkTopBorder.Location = new System.Drawing.Point(3, 5);
			this.chkTopBorder.Name = "chkTopBorder";
			this.chkTopBorder.Size = new System.Drawing.Size(26, 22);
			this.chkTopBorder.TabIndex = 1;
			this.drawPanel.Appearance.BackColor = System.Drawing.SystemColors.Control;
			this.drawPanel.Appearance.Options.UseBackColor = true;
			this.drawPanel.Location = new System.Drawing.Point(37, 5);
			this.drawPanel.Name = "drawPanel";
			this.drawPanel.Size = new System.Drawing.Size(170, 102);
			this.drawPanel.TabIndex = 32;
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.drawPanel);
			this.Controls.Add(this.chkDiagonalDownBorder);
			this.Controls.Add(this.chkRightBorder);
			this.Controls.Add(this.chkInsideVerticalBorder);
			this.Controls.Add(this.chkLeftBorder);
			this.Controls.Add(this.chkDiagonalUpBorder);
			this.Controls.Add(this.chkBottomBorder);
			this.Controls.Add(this.chkInsideHorizontalBorder);
			this.Controls.Add(this.chkTopBorder);
			this.Name = "FormatBorderControl";
			this.Size = new System.Drawing.Size(246, 142);
			((System.ComponentModel.ISupportInitialize)(this.drawPanel)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.CheckButton chkDiagonalDownBorder;
		private XtraEditors.CheckButton chkRightBorder;
		private XtraEditors.CheckButton chkInsideVerticalBorder;
		private XtraEditors.CheckButton chkLeftBorder;
		private XtraEditors.CheckButton chkDiagonalUpBorder;
		private XtraEditors.CheckButton chkBottomBorder;
		private XtraEditors.CheckButton chkInsideHorizontalBorder;
		private XtraEditors.CheckButton chkTopBorder;
		private XtraEditors.PanelControl drawPanel;
	}
}
