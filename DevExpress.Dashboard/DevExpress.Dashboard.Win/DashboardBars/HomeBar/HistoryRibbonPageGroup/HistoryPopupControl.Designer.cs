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

namespace DevExpress.DashboardWin.Native
{
	partial class HistoryPopupControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.panel = new DevExpress.XtraEditors.PanelControl();
			this.label = new DevExpress.XtraEditors.LabelControl();
			this.listBox = new DevExpress.DashboardWin.Native.HistoryItemListBox();
			((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBox)).BeginInit();
			this.SuspendLayout();
			this.panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel.Controls.Add(this.label);
			this.panel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel.Location = new System.Drawing.Point(0, 120);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(235, 30);
			this.panel.TabIndex = 0;
			this.label.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.label.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.label.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(235, 30);
			this.label.TabIndex = 0;
			this.label.Text = "Undo 0 Actions";
			this.listBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.Location = new System.Drawing.Point(0, 0);
			this.listBox.Name = "listBox";
			this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.listBox.Size = new System.Drawing.Size(235, 120);
			this.listBox.TabIndex = 1;
			this.listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseMove);
			this.listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseClick);
			this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listBox);
			this.Controls.Add(this.panel);
			this.Name = "HistoryPopupControl";
			this.Size = new System.Drawing.Size(235, 150);
			((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
			this.panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl panel;
		private DevExpress.XtraEditors.LabelControl label;
		private DevExpress.DashboardWin.Native.HistoryItemListBox listBox;
	}
}
