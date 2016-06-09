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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public class MDISiteSize : System.Windows.Forms.UserControl {
		private System.Windows.Forms.Label lbName;
		private System.Windows.Forms.Label lbSize;
		private DevExpress.XtraEditors.SpinEdit seSize;
		private DevExpress.XtraEditors.CheckEdit ceAutoSize;
		private System.ComponentModel.Container components = null;
		public MDISiteSize() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lbName = new System.Windows.Forms.Label();
			this.lbSize = new System.Windows.Forms.Label();
			this.seSize = new DevExpress.XtraEditors.SpinEdit();
			this.ceAutoSize = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.seSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoSize.Properties)).BeginInit();
			this.SuspendLayout();
			this.lbName.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lbName.BackColor = System.Drawing.SystemColors.ControlDark;
			this.lbName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lbName.Location = new System.Drawing.Point(4, 4);
			this.lbName.Name = "lbName";
			this.lbName.Size = new System.Drawing.Size(188, 20);
			this.lbName.TabIndex = 0;
			this.lbName.Text = "Name";
			this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbSize.Enabled = false;
			this.lbSize.Location = new System.Drawing.Point(8, 28);
			this.lbSize.Name = "lbSize";
			this.lbSize.Size = new System.Drawing.Size(56, 20);
			this.lbSize.TabIndex = 1;
			this.lbSize.Text = "Size";
			this.lbSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.seSize.EditValue = new System.Decimal(new int[] {
																	 150,
																	 0,
																	 0,
																	 0});
			this.seSize.Location = new System.Drawing.Point(52, 28);
			this.seSize.Name = "seSize";
			this.seSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
																										   new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seSize.Properties.Enabled = false;
			this.seSize.Properties.Increment = new System.Decimal(new int[] {
																				10,
																				0,
																				0,
																				0});
			this.seSize.Properties.IsFloatValue = false;
			this.seSize.Properties.MaxValue = new System.Decimal(new int[] {
																			   1000,
																			   0,
																			   0,
																			   0});
			this.seSize.Size = new System.Drawing.Size(52, 19);
			this.seSize.TabIndex = 2;
			this.ceAutoSize.EditValue = true;
			this.ceAutoSize.Location = new System.Drawing.Point(112, 28);
			this.ceAutoSize.Name = "ceAutoSize";
			this.ceAutoSize.Properties.Caption = "Auto Size";
			this.ceAutoSize.Size = new System.Drawing.Size(80, 19);
			this.ceAutoSize.TabIndex = 3;
			this.ceAutoSize.CheckedChanged += new System.EventHandler(this.ceAutoSize_CheckedChanged);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.ceAutoSize,
																		  this.seSize,
																		  this.lbSize,
																		  this.lbName});
			this.Name = "MDISiteSize";
			this.Size = new System.Drawing.Size(196, 52);
			((System.ComponentModel.ISupportInitialize)(this.seSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoSize.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void ceAutoSize_CheckedChanged(object sender, System.EventArgs e) {
			seSize.Properties.Enabled = lbSize.Enabled = !ceAutoSize.Checked;
		}
		public string Caption {
			get { return lbName.Text; }
			set { lbName.Text = value; }
		}
		public string SizeName {
			get { return lbSize.Text; }
			set { lbSize.Text = value; }
		}
		public new bool AutoSize {
			get { return ceAutoSize.Checked; }
			set { ceAutoSize.Checked = value; }
		}
		public int SiteSize {
			get { return Convert.ToInt32(seSize.Value); }
			set { seSize.Value = value; }
		}
	}
}
