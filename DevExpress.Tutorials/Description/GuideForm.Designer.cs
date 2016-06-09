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

namespace DevExpress.Description.Controls {
	partial class GuideForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.btHide = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.lbDescription = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.btHide);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(0, 163);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(543, 10);
			this.panelControl1.TabIndex = 0;
			this.panelControl1.Visible = false;
			this.btHide.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btHide.Location = new System.Drawing.Point(440, 11);
			this.btHide.Name = "btHide";
			this.btHide.Size = new System.Drawing.Size(75, 23);
			this.btHide.TabIndex = 0;
			this.btHide.Text = "Hide";
			this.btHide.Click += new System.EventHandler(this.btHide_Click);
			this.groupControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.groupControl1.Controls.Add(this.lbDescription);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(3);
			this.groupControl1.ShowCaption = false;
			this.groupControl1.Size = new System.Drawing.Size(543, 153);
			this.groupControl1.TabIndex = 1;
			this.groupControl1.Text = "Description";
			this.lbDescription.AllowHtmlString = true;
			this.lbDescription.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lbDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lbDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbDescription.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lbDescription.Cursor = System.Windows.Forms.Cursors.Default;
			this.lbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbDescription.Location = new System.Drawing.Point(3, 3);
			this.lbDescription.Name = "lbDescription";
			this.lbDescription.Size = new System.Drawing.Size(537, 147);
			this.lbDescription.TabIndex = 0;
			this.lbDescription.Text = "Grid\r\n\r\nTo get more information visit <href=https://www.devexpress.com/Products/N" +
	"ET/Controls/WinForms/Grid/>Learn more</href>\r\n";
			this.lbDescription.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.lbDescription_HyperlinkClick);
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelControl1.Location = new System.Drawing.Point(0, 153);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(543, 10);
			this.labelControl1.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btHide;
			this.ClientSize = new System.Drawing.Size(543, 173);
			this.Controls.Add(this.groupControl1);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.panelControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.LookAndFeel.SkinName = "Office 2013";
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GuideForm";
			this.ShowInTaskbar = false;
			this.Text = "Description";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.PanelControl panelControl1;
		private XtraEditors.GroupControl groupControl1;
		private XtraEditors.SimpleButton btHide;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl lbDescription;
	}
}
