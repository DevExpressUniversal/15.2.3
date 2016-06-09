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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.Tutorials {
	public class FrmWhatsThisTextOnly : FrmWhatsThisBase {
		private System.Windows.Forms.Panel pnlButtonContainer;
		private DevExpress.XtraEditors.SimpleButton btnClose;
		private System.Windows.Forms.Panel pnlDescContainer;
		private System.Windows.Forms.Panel pnlLeft;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblDescriptionHeader;
		private System.Windows.Forms.Panel pnlSeparator1;
		private System.Windows.Forms.Panel panel1;
		public FrmWhatsThisTextOnly() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.pnlButtonContainer = new System.Windows.Forms.Panel();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.pnlDescContainer = new System.Windows.Forms.Panel();
			this.lblDescription = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblDescriptionHeader = new System.Windows.Forms.Label();
			this.pnlSeparator1 = new System.Windows.Forms.Panel();
			this.pnlLeft = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.pnlButtonContainer.SuspendLayout();
			this.pnlDescContainer.SuspendLayout();
			this.SuspendLayout();
			this.pnlButtonContainer.BackColor = System.Drawing.SystemColors.Info;
			this.pnlButtonContainer.Controls.Add(this.btnClose);
			this.pnlButtonContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtonContainer.Location = new System.Drawing.Point(0, 86);
			this.pnlButtonContainer.Name = "pnlButtonContainer";
			this.pnlButtonContainer.Size = new System.Drawing.Size(374, 40);
			this.pnlButtonContainer.TabIndex = 10;
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(284, 8);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(80, 24);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			this.pnlDescContainer.BackColor = System.Drawing.SystemColors.Info;
			this.pnlDescContainer.Controls.Add(this.lblDescription);
			this.pnlDescContainer.Controls.Add(this.panel1);
			this.pnlDescContainer.Controls.Add(this.lblDescriptionHeader);
			this.pnlDescContainer.Controls.Add(this.pnlSeparator1);
			this.pnlDescContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlDescContainer.Location = new System.Drawing.Point(8, 0);
			this.pnlDescContainer.Name = "pnlDescContainer";
			this.pnlDescContainer.Size = new System.Drawing.Size(358, 86);
			this.pnlDescContainer.TabIndex = 11;
			this.lblDescription.BackColor = System.Drawing.SystemColors.Info;
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(1)))));
			this.lblDescription.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblDescription.Location = new System.Drawing.Point(0, 32);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(358, 40);
			this.lblDescription.TabIndex = 3;
			this.lblDescription.Text = "Description content";
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(358, 8);
			this.panel1.TabIndex = 13;
			this.lblDescriptionHeader.BackColor = System.Drawing.SystemColors.Info;
			this.lblDescriptionHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDescriptionHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblDescriptionHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(1)))));
			this.lblDescriptionHeader.Location = new System.Drawing.Point(0, 8);
			this.lblDescriptionHeader.Name = "lblDescriptionHeader";
			this.lblDescriptionHeader.Size = new System.Drawing.Size(358, 16);
			this.lblDescriptionHeader.TabIndex = 11;
			this.lblDescriptionHeader.Text = "Description:";
			this.pnlSeparator1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlSeparator1.Location = new System.Drawing.Point(0, 0);
			this.pnlSeparator1.Name = "pnlSeparator1";
			this.pnlSeparator1.Size = new System.Drawing.Size(358, 8);
			this.pnlSeparator1.TabIndex = 12;
			this.pnlLeft.BackColor = System.Drawing.SystemColors.Info;
			this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLeft.Location = new System.Drawing.Point(0, 0);
			this.pnlLeft.Name = "pnlLeft";
			this.pnlLeft.Size = new System.Drawing.Size(8, 86);
			this.pnlLeft.TabIndex = 12;
			this.pnlRight.BackColor = System.Drawing.SystemColors.Info;
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlRight.Location = new System.Drawing.Point(366, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(8, 86);
			this.pnlRight.TabIndex = 13;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(374, 126);
			this.Controls.Add(this.pnlDescContainer);
			this.Controls.Add(this.pnlLeft);
			this.Controls.Add(this.pnlRight);
			this.Controls.Add(this.pnlButtonContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimumSize = new System.Drawing.Size(300, 150);
			this.Name = "FrmWhatsThisTextOnly";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.pnlButtonContainer.ResumeLayout(false);
			this.pnlDescContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void UpdateControls(WhatsThisParams whatsThisParams) {
			base.UpdateControls(whatsThisParams);
			lblDescription.Text = whatsThisParams.Description;
		}
		protected override void UpdateDescriptionPanel() {
			ControlUtils.UpdateLabelHeight(lblDescription);
			int totalHeight = 0;
			foreach(Control control in pnlDescContainer.Controls)
				totalHeight += control.Height;
			this.ClientSize = new Size(this.ClientSize.Width, totalHeight + pnlButtonContainer.Height);
		}
		private void btnClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
}
