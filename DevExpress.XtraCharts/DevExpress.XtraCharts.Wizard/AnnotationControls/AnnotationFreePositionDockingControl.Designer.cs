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

namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	partial class AnnotationFreePositionDockingControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationFreePositionDockingControl));
			this.pnlCorner = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbCorner = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblCorner = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepTarget = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlTarget = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbTarget = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblTarget = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlCorner)).BeginInit();
			this.pnlCorner.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbCorner.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTarget)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTarget)).BeginInit();
			this.pnlTarget.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbTarget.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlCorner, "pnlCorner");
			this.pnlCorner.BackColor = System.Drawing.Color.Transparent;
			this.pnlCorner.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCorner.Controls.Add(this.cbCorner);
			this.pnlCorner.Controls.Add(this.lblCorner);
			this.pnlCorner.Name = "pnlCorner";
			resources.ApplyResources(this.cbCorner, "cbCorner");
			this.cbCorner.Name = "cbCorner";
			this.cbCorner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbCorner.Properties.Buttons"))))});
			this.cbCorner.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbCorner.SelectedIndexChanged += new System.EventHandler(this.cbCorner_SelectedIndexChanged);
			resources.ApplyResources(this.lblCorner, "lblCorner");
			this.lblCorner.Name = "lblCorner";
			this.sepTarget.BackColor = System.Drawing.Color.Transparent;
			this.sepTarget.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepTarget, "sepTarget");
			this.sepTarget.Name = "sepTarget";
			resources.ApplyResources(this.pnlTarget, "pnlTarget");
			this.pnlTarget.BackColor = System.Drawing.Color.Transparent;
			this.pnlTarget.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTarget.Controls.Add(this.cbTarget);
			this.pnlTarget.Controls.Add(this.lblTarget);
			this.pnlTarget.Name = "pnlTarget";
			resources.ApplyResources(this.cbTarget, "cbTarget");
			this.cbTarget.Name = "cbTarget";
			this.cbTarget.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTarget.Properties.Buttons"))))});
			this.cbTarget.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbTarget.SelectedIndexChanged += new System.EventHandler(this.cbTarget_SelectedIndexChanged);
			resources.ApplyResources(this.lblTarget, "lblTarget");
			this.lblTarget.Name = "lblTarget";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlCorner);
			this.Controls.Add(this.sepTarget);
			this.Controls.Add(this.pnlTarget);
			this.Name = "AnnotationFreePositionDockingControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlCorner)).EndInit();
			this.pnlCorner.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbCorner.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTarget)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTarget)).EndInit();
			this.pnlTarget.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbTarget.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlCorner;
		private DevExpress.XtraEditors.ComboBoxEdit cbCorner;
		private ChartLabelControl lblCorner;
		private ChartPanelControl sepTarget;
		private ChartPanelControl pnlTarget;
		private DevExpress.XtraEditors.ComboBoxEdit cbTarget;
		private ChartLabelControl lblTarget;
	}
}
