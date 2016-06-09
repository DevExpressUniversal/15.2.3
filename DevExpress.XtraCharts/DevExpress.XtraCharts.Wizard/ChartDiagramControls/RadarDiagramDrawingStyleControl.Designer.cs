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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class RadarDiagramDrawingStyleControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarDiagramDrawingStyleControl));
			this.pnlThickness = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtAngle = new DevExpress.XtraEditors.SpinEdit();
			this.panelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chRotate = new DevExpress.XtraEditors.CheckEdit();
			this.cbDrawingStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chRotate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDrawingStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			this.SuspendLayout();
			this.pnlThickness.BackColor = System.Drawing.Color.Transparent;
			this.pnlThickness.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlThickness, "pnlThickness");
			this.pnlThickness.Name = "pnlThickness";
			resources.ApplyResources(this.txtAngle, "txtAngle");
			this.txtAngle.Name = "txtAngle";
			this.txtAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtAngle.Properties.IsFloatValue = false;
			this.txtAngle.Properties.Mask.EditMask = resources.GetString("txtAngle.Properties.Mask.EditMask");
			this.txtAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.txtAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.txtAngle.Properties.ValidateOnEnterKey = true;
			this.txtAngle.EditValueChanged += new System.EventHandler(this.txtAngle_EditValueChanged);
			resources.ApplyResources(this.panelControl4, "panelControl4");
			this.panelControl4.BackColor = System.Drawing.Color.Transparent;
			this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl4.Name = "panelControl4";
			resources.ApplyResources(this.chRotate, "chRotate");
			this.chRotate.Name = "chRotate";
			this.chRotate.Properties.Caption = resources.GetString("chRotate.Properties.Caption");
			this.chRotate.CheckedChanged += new System.EventHandler(this.chRotate_CheckedChanged);
			resources.ApplyResources(this.cbDrawingStyle, "cbDrawingStyle");
			this.cbDrawingStyle.Name = "cbDrawingStyle";
			this.cbDrawingStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDrawingStyle.Properties.Buttons"))))});
			this.cbDrawingStyle.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDrawingStyle.Properties.Items"),
			resources.GetString("cbDrawingStyle.Properties.Items1")});
			this.cbDrawingStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDrawingStyle.SelectedIndexChanged += new System.EventHandler(this.cbDrawingStyle_SelectedIndexChanged);
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.txtAngle);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.cbDrawingStyle);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.panelControl4);
			this.Controls.Add(this.pnlThickness);
			this.Controls.Add(this.chRotate);
			this.Name = "RadarDiagramDrawingStyleControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chRotate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDrawingStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlThickness;
		private DevExpress.XtraEditors.SpinEdit txtAngle;
		private ChartPanelControl panelControl4;
		private DevExpress.XtraEditors.CheckEdit chRotate;
		private DevExpress.XtraEditors.ComboBoxEdit cbDrawingStyle;
		private ChartPanelControl chartPanelControl4;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl panelControl2;
		private ChartPanelControl chartPanelControl1;
		private ChartLabelControl chartLabelControl1;
	}
}
