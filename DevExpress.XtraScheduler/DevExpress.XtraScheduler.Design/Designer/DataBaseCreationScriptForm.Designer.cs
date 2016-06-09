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

namespace DevExpress.XtraScheduler.Design {
	partial class DataBaseCreationScriptForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.rtbCreateScript = new System.Windows.Forms.RichTextBox();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.hyperLinkEdit1 = new DevExpress.XtraEditors.HyperLinkEdit();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.hyperLinkEdit1.Properties)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			this.rtbCreateScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbCreateScript.Location = new System.Drawing.Point(10, 10);
			this.rtbCreateScript.Margin = new System.Windows.Forms.Padding(10);
			this.rtbCreateScript.Name = "rtbCreateScript";
			this.rtbCreateScript.ReadOnly = true;
			this.rtbCreateScript.Size = new System.Drawing.Size(856, 254);
			this.rtbCreateScript.TabIndex = 2;
			this.rtbCreateScript.Text = "";
			this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButton1.Location = new System.Drawing.Point(705, 13);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(161, 23);
			this.simpleButton1.TabIndex = 3;
			this.simpleButton1.Text = "Copy to Clipboard and Close";
			this.simpleButton1.Click += new System.EventHandler(this.OnSimpleButton1Click);
			this.hyperLinkEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.hyperLinkEdit1.EditValue = "http://documentation.devexpress.com/#WindowsForms/CustomDocument3289";
			this.hyperLinkEdit1.Location = new System.Drawing.Point(12, 18);
			this.hyperLinkEdit1.Name = "hyperLinkEdit1";
			this.hyperLinkEdit1.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
			this.hyperLinkEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.Appearance.BackColor2 = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.Appearance.BorderColor = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.Appearance.Options.UseBackColor = true;
			this.hyperLinkEdit1.Properties.Appearance.Options.UseBorderColor = true;
			this.hyperLinkEdit1.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.AppearanceReadOnly.Options.UseBackColor = true;
			this.hyperLinkEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.hyperLinkEdit1.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.hyperLinkEdit1.Properties.Caption = "Read more...";
			this.hyperLinkEdit1.Size = new System.Drawing.Size(76, 18);
			this.hyperLinkEdit1.TabIndex = 5;
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.rtbCreateScript);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
			this.panel1.Size = new System.Drawing.Size(876, 264);
			this.panel1.TabIndex = 6;
			this.panel2.BackColor = System.Drawing.Color.Transparent;
			this.panel2.Controls.Add(this.hyperLinkEdit1);
			this.panel2.Controls.Add(this.simpleButton1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 264);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(876, 48);
			this.panel2.TabIndex = 7;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(876, 312);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "DataBaseCreationScriptForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create Sample Database  ";
			((System.ComponentModel.ISupportInitialize)(this.hyperLinkEdit1.Properties)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.RichTextBox rtbCreateScript;
		private XtraEditors.SimpleButton simpleButton1;
		private XtraEditors.HyperLinkEdit hyperLinkEdit1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
	}
}
