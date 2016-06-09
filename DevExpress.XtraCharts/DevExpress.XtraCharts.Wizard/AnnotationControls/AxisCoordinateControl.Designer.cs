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
	partial class AxisCoordinateControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisCoordinateControl));
			this.pnlAxis = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAxis = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAxis = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepAxis = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAxisValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtAxisValue = new DevExpress.XtraEditors.TextEdit();
			this.lblAxisValue = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlAxis)).BeginInit();
			this.pnlAxis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbAxis.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAxis)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAxisValue)).BeginInit();
			this.pnlAxisValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtAxisValue.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlAxis, "pnlAxis");
			this.pnlAxis.BackColor = System.Drawing.Color.Transparent;
			this.pnlAxis.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAxis.Controls.Add(this.cbAxis);
			this.pnlAxis.Controls.Add(this.lblAxis);
			this.pnlAxis.Name = "pnlAxis";
			resources.ApplyResources(this.cbAxis, "cbAxis");
			this.cbAxis.Name = "cbAxis";
			this.cbAxis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAxis.Properties.Buttons"))))});
			this.cbAxis.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAxis.SelectedIndexChanged += new System.EventHandler(this.cbAxis_SelectedIndexChanged);
			resources.ApplyResources(this.lblAxis, "lblAxis");
			this.lblAxis.Name = "lblAxis";
			this.sepAxis.BackColor = System.Drawing.Color.Transparent;
			this.sepAxis.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAxis, "sepAxis");
			this.sepAxis.Name = "sepAxis";
			resources.ApplyResources(this.pnlAxisValue, "pnlAxisValue");
			this.pnlAxisValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlAxisValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAxisValue.Controls.Add(this.txtAxisValue);
			this.pnlAxisValue.Controls.Add(this.lblAxisValue);
			this.pnlAxisValue.Name = "pnlAxisValue";
			resources.ApplyResources(this.txtAxisValue, "txtAxisValue");
			this.txtAxisValue.EnterMoveNextControl = true;
			this.txtAxisValue.Name = "txtAxisValue";
			this.txtAxisValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtAxisValue_Validating);
			this.txtAxisValue.Validated += new System.EventHandler(this.txtAxisValue_Validated);
			resources.ApplyResources(this.lblAxisValue, "lblAxisValue");
			this.lblAxisValue.Name = "lblAxisValue";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlAxisValue);
			this.Controls.Add(this.sepAxis);
			this.Controls.Add(this.pnlAxis);
			this.Name = "AxisCoordinateControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlAxis)).EndInit();
			this.pnlAxis.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbAxis.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAxis)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAxisValue)).EndInit();
			this.pnlAxisValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtAxisValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlAxis;
		private DevExpress.XtraEditors.ComboBoxEdit cbAxis;
		private ChartLabelControl lblAxis;
		private ChartPanelControl sepAxis;
		private ChartPanelControl pnlAxisValue;
		private DevExpress.XtraEditors.TextEdit txtAxisValue;
		private ChartLabelControl lblAxisValue;
	}
}
