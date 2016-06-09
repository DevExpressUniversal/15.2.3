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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class ExplodedPointsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplodedPointsControl));
			this.panelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlFilters = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.beFilters = new DevExpress.XtraEditors.ButtonEdit();
			this.chartLabelControl3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.beExplodedPoints = new DevExpress.XtraEditors.ButtonEdit();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbExplodeMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDistance = new DevExpress.XtraEditors.SpinEdit();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlFilters)).BeginInit();
			this.pnlFilters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.beExplodedPoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			this.chartPanelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbExplodeMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.BackColor = System.Drawing.Color.Transparent;
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.Add(this.pnlFilters);
			this.panelControl3.Controls.Add(this.chartPanelControl3);
			this.panelControl3.Controls.Add(this.chartPanelControl2);
			this.panelControl3.Controls.Add(this.chartPanelControl1);
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.pnlFilters, "pnlFilters");
			this.pnlFilters.BackColor = System.Drawing.Color.Transparent;
			this.pnlFilters.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFilters.Controls.Add(this.beFilters);
			this.pnlFilters.Controls.Add(this.chartLabelControl3);
			this.pnlFilters.Controls.Add(this.chartPanelControl4);
			this.pnlFilters.Name = "pnlFilters";
			resources.ApplyResources(this.beFilters, "beFilters");
			this.beFilters.Name = "beFilters";
			this.beFilters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beFilters.Properties.ReadOnly = true;
			this.beFilters.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beFilters_ButtonClick);
			resources.ApplyResources(this.chartLabelControl3, "chartLabelControl3");
			this.chartLabelControl3.Name = "chartLabelControl3";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.beExplodedPoints);
			this.chartPanelControl3.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.beExplodedPoints, "beExplodedPoints");
			this.beExplodedPoints.Name = "beExplodedPoints";
			this.beExplodedPoints.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beExplodedPoints.Properties.ReadOnly = true;
			this.beExplodedPoints.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beExplodedPoints_ButtonClick);
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Controls.Add(this.cbExplodeMode);
			this.chartPanelControl1.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.cbExplodeMode, "cbExplodeMode");
			this.cbExplodeMode.Name = "cbExplodeMode";
			this.cbExplodeMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbExplodeMode.Properties.Buttons"))))});
			this.cbExplodeMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbExplodeMode.Properties.Items"),
			resources.GetString("cbExplodeMode.Properties.Items1"),
			resources.GetString("cbExplodeMode.Properties.Items2"),
			resources.GetString("cbExplodeMode.Properties.Items3"),
			resources.GetString("cbExplodeMode.Properties.Items4"),
			resources.GetString("cbExplodeMode.Properties.Items5")});
			this.cbExplodeMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbExplodeMode.SelectedIndexChanged += new System.EventHandler(this.cbExplodeMode_SelectedIndexChanged);
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.panelControl6.BackColor = System.Drawing.Color.Transparent;
			this.panelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl6, "panelControl6");
			this.panelControl6.Name = "panelControl6";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.txtDistance);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.txtDistance, "txtDistance");
			this.txtDistance.Name = "txtDistance";
			this.txtDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDistance.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtDistance.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtDistance.EditValueChanged += new System.EventHandler(this.txtDistance_EditValueChanged);
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chartPanelControl5);
			this.Controls.Add(this.panelControl6);
			this.Controls.Add(this.panelControl3);
			this.Name = "ExplodedPointsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			this.panelControl3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlFilters)).EndInit();
			this.pnlFilters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.beExplodedPoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			this.chartPanelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbExplodeMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelControl3;
		private ChartPanelControl panelControl6;
		private ChartPanelControl pnlFilters;
		private DevExpress.XtraEditors.ButtonEdit beFilters;
		private ChartLabelControl chartLabelControl3;
		private ChartPanelControl chartPanelControl3;
		private DevExpress.XtraEditors.ButtonEdit beExplodedPoints;
		private ChartLabelControl chartLabelControl2;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.ComboBoxEdit cbExplodeMode;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraEditors.SpinEdit txtDistance;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl4;
	}
}
