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
	partial class ScrollingZooming3DControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingZooming3DControl));
			this.txtZoom = new DevExpress.XtraEditors.SpinEdit();
			this.txtVScroll = new DevExpress.XtraEditors.SpinEdit();
			this.txtHScroll = new DevExpress.XtraEditors.SpinEdit();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartLabelControl2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.gcZoomingOptions = new DevExpress.XtraEditors.GroupControl();
			this.zoomingOptions = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ZoomingOptionsControl();
			this.pnlSeparator2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.gcScrollingOptions = new DevExpress.XtraEditors.GroupControl();
			this.scrollingOptions = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollingOptionsControl();
			this.pnlSeparator1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.txtZoom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtVScroll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHScroll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.chartPanelControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcZoomingOptions)).BeginInit();
			this.gcZoomingOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcScrollingOptions)).BeginInit();
			this.gcScrollingOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.txtZoom, "txtZoom");
			this.txtZoom.Name = "txtZoom";
			this.txtZoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtZoom.Properties.IsFloatValue = false;
			this.txtZoom.Properties.Mask.EditMask = resources.GetString("txtZoom.Properties.Mask.EditMask");
			this.txtZoom.Properties.MaxValue = new decimal(new int[] {
			500,
			0,
			0,
			0});
			this.txtZoom.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtZoom.EditValueChanged += new System.EventHandler(this.txtZoom_EditValueChanged);
			resources.ApplyResources(this.txtVScroll, "txtVScroll");
			this.txtVScroll.Name = "txtVScroll";
			this.txtVScroll.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtVScroll.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtVScroll.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			-2147483648});
			this.txtVScroll.EditValueChanged += new System.EventHandler(this.txtVScroll_EditValueChanged);
			resources.ApplyResources(this.txtHScroll, "txtHScroll");
			this.txtHScroll.Name = "txtHScroll";
			this.txtHScroll.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtHScroll.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtHScroll.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			-2147483648});
			this.txtHScroll.EditValueChanged += new System.EventHandler(this.txtHScroll_EditValueChanged);
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl4.Controls.Add(this.txtHScroll);
			this.chartPanelControl4.Controls.Add(this.chartLabelControl4);
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.chartLabelControl4, "chartLabelControl4");
			this.chartLabelControl4.Name = "chartLabelControl4";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.txtVScroll);
			this.chartPanelControl2.Controls.Add(this.chartLabelControl1);
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartLabelControl1, "chartLabelControl1");
			this.chartLabelControl1.Name = "chartLabelControl1";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.txtZoom);
			this.chartPanelControl5.Controls.Add(this.chartLabelControl2);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.chartLabelControl2, "chartLabelControl2");
			this.chartLabelControl2.Name = "chartLabelControl2";
			resources.ApplyResources(this.gcZoomingOptions, "gcZoomingOptions");
			this.gcZoomingOptions.Controls.Add(this.zoomingOptions);
			this.gcZoomingOptions.Name = "gcZoomingOptions";
			resources.ApplyResources(this.zoomingOptions, "zoomingOptions");
			this.zoomingOptions.Name = "zoomingOptions";
			this.pnlSeparator2.BackColor = System.Drawing.Color.Transparent;
			this.pnlSeparator2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlSeparator2, "pnlSeparator2");
			this.pnlSeparator2.Name = "pnlSeparator2";
			resources.ApplyResources(this.gcScrollingOptions, "gcScrollingOptions");
			this.gcScrollingOptions.Controls.Add(this.scrollingOptions);
			this.gcScrollingOptions.Name = "gcScrollingOptions";
			resources.ApplyResources(this.scrollingOptions, "scrollingOptions");
			this.scrollingOptions.Name = "scrollingOptions";
			this.pnlSeparator1.BackColor = System.Drawing.Color.Transparent;
			this.pnlSeparator1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlSeparator1, "pnlSeparator1");
			this.pnlSeparator1.Name = "pnlSeparator1";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.gcZoomingOptions);
			this.Controls.Add(this.pnlSeparator2);
			this.Controls.Add(this.gcScrollingOptions);
			this.Controls.Add(this.pnlSeparator1);
			this.Controls.Add(this.chartPanelControl5);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.chartPanelControl4);
			this.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "ScrollingZooming3DControl";
			((System.ComponentModel.ISupportInitialize)(this.txtZoom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtVScroll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtHScroll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.chartPanelControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcZoomingOptions)).EndInit();
			this.gcZoomingOptions.ResumeLayout(false);
			this.gcZoomingOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcScrollingOptions)).EndInit();
			this.gcScrollingOptions.ResumeLayout(false);
			this.gcScrollingOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SpinEdit txtZoom;
		private DevExpress.XtraEditors.SpinEdit txtVScroll;
		private DevExpress.XtraEditors.SpinEdit txtHScroll;
		private ChartPanelControl chartPanelControl4;
		private ChartLabelControl chartLabelControl4;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl chartPanelControl2;
		private ChartLabelControl chartLabelControl1;
		private ChartPanelControl chartPanelControl3;
		private ChartPanelControl chartPanelControl5;
		private ChartLabelControl chartLabelControl2;
		private DevExpress.XtraEditors.GroupControl gcZoomingOptions;
		private ZoomingOptionsControl zoomingOptions;
		private ChartPanelControl pnlSeparator2;
		private DevExpress.XtraEditors.GroupControl gcScrollingOptions;
		private ScrollingOptionsControl scrollingOptions;
		private ChartPanelControl pnlSeparator1;
	}
}
