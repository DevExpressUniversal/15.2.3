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
	partial class AnnotationShapePositionControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationShapePositionControl));
			this.pnlPositionKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPositionKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPositionKind = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepPositionKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlShapePositonOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlPositionKind)).BeginInit();
			this.pnlPositionKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPositionKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepPositionKind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapePositonOptions)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlPositionKind, "pnlPositionKind");
			this.pnlPositionKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlPositionKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPositionKind.Controls.Add(this.cbPositionKind);
			this.pnlPositionKind.Controls.Add(this.lblPositionKind);
			this.pnlPositionKind.Name = "pnlPositionKind";
			resources.ApplyResources(this.cbPositionKind, "cbPositionKind");
			this.cbPositionKind.Name = "cbPositionKind";
			this.cbPositionKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPositionKind.Properties.Buttons"))))});
			this.cbPositionKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPositionKind.SelectedIndexChanged += new System.EventHandler(this.cbPositionKind_SelectedIndexChanged);
			resources.ApplyResources(this.lblPositionKind, "lblPositionKind");
			this.lblPositionKind.Name = "lblPositionKind";
			this.sepPositionKind.BackColor = System.Drawing.Color.Transparent;
			this.sepPositionKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepPositionKind, "sepPositionKind");
			this.sepPositionKind.Name = "sepPositionKind";
			resources.ApplyResources(this.pnlShapePositonOptions, "pnlShapePositonOptions");
			this.pnlShapePositonOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnlShapePositonOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlShapePositonOptions.Name = "pnlShapePositonOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlShapePositonOptions);
			this.Controls.Add(this.sepPositionKind);
			this.Controls.Add(this.pnlPositionKind);
			this.Name = "AnnotationShapePositionControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlPositionKind)).EndInit();
			this.pnlPositionKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPositionKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepPositionKind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlShapePositonOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlPositionKind;
		private DevExpress.XtraEditors.ComboBoxEdit cbPositionKind;
		private ChartLabelControl lblPositionKind;
		private ChartPanelControl sepPositionKind;
		private ChartPanelControl pnlShapePositonOptions;
	}
}
