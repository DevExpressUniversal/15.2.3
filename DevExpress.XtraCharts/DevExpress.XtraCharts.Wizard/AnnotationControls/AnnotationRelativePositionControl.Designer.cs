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
	partial class AnnotationRelativePositionControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationRelativePositionControl));
			this.pnlAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnAngle = new DevExpress.XtraEditors.SpinEdit();
			this.lblAngle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlConnectorLength = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnConnectorLength = new DevExpress.XtraEditors.SpinEdit();
			this.lblConnectorLength = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlAngle)).BeginInit();
			this.pnlAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAngle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlConnectorLength)).BeginInit();
			this.pnlConnectorLength.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnConnectorLength.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlAngle, "pnlAngle");
			this.pnlAngle.BackColor = System.Drawing.Color.Transparent;
			this.pnlAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAngle.Controls.Add(this.spnAngle);
			this.pnlAngle.Controls.Add(this.lblAngle);
			this.pnlAngle.Name = "pnlAngle";
			resources.ApplyResources(this.spnAngle, "spnAngle");
			this.spnAngle.Name = "spnAngle";
			this.spnAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnAngle.Properties.DisplayFormat.FormatString = "F2";
			this.spnAngle.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spnAngle.Properties.EditFormat.FormatString = "F2";
			this.spnAngle.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spnAngle.Properties.Mask.EditMask = resources.GetString("spnAngle.Properties.Mask.EditMask");
			this.spnAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.spnAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.spnAngle.EditValueChanged += new System.EventHandler(this.spnAngle_EditValueChanged);
			resources.ApplyResources(this.lblAngle, "lblAngle");
			this.lblAngle.Name = "lblAngle";
			this.sepAngle.BackColor = System.Drawing.Color.Transparent;
			this.sepAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAngle, "sepAngle");
			this.sepAngle.Name = "sepAngle";
			resources.ApplyResources(this.pnlConnectorLength, "pnlConnectorLength");
			this.pnlConnectorLength.BackColor = System.Drawing.Color.Transparent;
			this.pnlConnectorLength.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlConnectorLength.Controls.Add(this.spnConnectorLength);
			this.pnlConnectorLength.Controls.Add(this.lblConnectorLength);
			this.pnlConnectorLength.Name = "pnlConnectorLength";
			resources.ApplyResources(this.spnConnectorLength, "spnConnectorLength");
			this.spnConnectorLength.Name = "spnConnectorLength";
			this.spnConnectorLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnConnectorLength.Properties.DisplayFormat.FormatString = "F2";
			this.spnConnectorLength.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spnConnectorLength.Properties.EditFormat.FormatString = "F2";
			this.spnConnectorLength.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spnConnectorLength.Properties.Mask.EditMask = resources.GetString("spnConnectorLength.Properties.Mask.EditMask");
			this.spnConnectorLength.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.spnConnectorLength.EditValueChanged += new System.EventHandler(this.spnConnectorLength_EditValueChanged);
			resources.ApplyResources(this.lblConnectorLength, "lblConnectorLength");
			this.lblConnectorLength.Name = "lblConnectorLength";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlConnectorLength);
			this.Controls.Add(this.sepAngle);
			this.Controls.Add(this.pnlAngle);
			this.Name = "AnnotationRelativePositionControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlAngle)).EndInit();
			this.pnlAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAngle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlConnectorLength)).EndInit();
			this.pnlConnectorLength.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnConnectorLength.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlAngle;
		private ChartLabelControl lblAngle;
		private DevExpress.XtraEditors.SpinEdit spnAngle;
		private ChartPanelControl sepAngle;
		private ChartPanelControl pnlConnectorLength;
		private DevExpress.XtraEditors.SpinEdit spnConnectorLength;
		private ChartLabelControl lblConnectorLength;
	}
}
