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

namespace DevExpress.XtraRichEdit.Design {
	partial class ColumnsEdit {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnsEdit));
			this.panel = new DevExpress.XtraEditors.XtraScrollableControl();
			this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
			this.lblSpacing = new DevExpress.XtraEditors.LabelControl();
			this.lblWidth = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnNumber = new DevExpress.XtraEditors.LabelControl();
			this.xtraScrollableControl1.SuspendLayout();
			this.SuspendLayout();
			this.panel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("panel.Appearance.BackColor")));
			this.panel.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			this.xtraScrollableControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("xtraScrollableControl1.Appearance.BackColor")));
			this.xtraScrollableControl1.Appearance.Options.UseBackColor = true;
			this.xtraScrollableControl1.Controls.Add(this.lblSpacing);
			this.xtraScrollableControl1.Controls.Add(this.lblWidth);
			this.xtraScrollableControl1.Controls.Add(this.lblColumnNumber);
			resources.ApplyResources(this.xtraScrollableControl1, "xtraScrollableControl1");
			this.xtraScrollableControl1.Name = "xtraScrollableControl1";
			resources.ApplyResources(this.lblSpacing, "lblSpacing");
			this.lblSpacing.Name = "lblSpacing";
			resources.ApplyResources(this.lblWidth, "lblWidth");
			this.lblWidth.Name = "lblWidth";
			resources.ApplyResources(this.lblColumnNumber, "lblColumnNumber");
			this.lblColumnNumber.Name = "lblColumnNumber";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel);
			this.Controls.Add(this.xtraScrollableControl1);
			this.Name = "ColumnsEdit";
			this.xtraScrollableControl1.ResumeLayout(false);
			this.xtraScrollableControl1.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.XtraScrollableControl panel;
		private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
		private DevExpress.XtraEditors.LabelControl lblSpacing;
		private DevExpress.XtraEditors.LabelControl lblWidth;
		private DevExpress.XtraEditors.LabelControl lblColumnNumber;
	}
}
