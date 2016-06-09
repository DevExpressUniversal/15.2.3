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
	partial class ScrollingZooming2DControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScrollingZooming2DControl));
			this.gcAxisY = new DevExpress.XtraEditors.GroupControl();
			this.chEnableAxixYZooming = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl10 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEnableAxixYScrolling = new DevExpress.XtraEditors.CheckEdit();
			this.pnlSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.gcAxisX = new DevExpress.XtraEditors.GroupControl();
			this.chEnableAxixXZooming = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl9 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEnableAxixXScrolling = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.gcScrollingOptions = new DevExpress.XtraEditors.GroupControl();
			this.scrollingOptions = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollingOptionsControl();
			this.gcZoomingOptions = new DevExpress.XtraEditors.GroupControl();
			this.zoomingOptions = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ZoomingOptionsControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisY)).BeginInit();
			this.gcAxisY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixYZooming.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixYScrolling.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisX)).BeginInit();
			this.gcAxisX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixXZooming.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixXScrolling.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcScrollingOptions)).BeginInit();
			this.gcScrollingOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcZoomingOptions)).BeginInit();
			this.gcZoomingOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.gcAxisY, "gcAxisY");
			this.gcAxisY.Controls.Add(this.chEnableAxixYZooming);
			this.gcAxisY.Controls.Add(this.chartPanelControl6);
			this.gcAxisY.Controls.Add(this.chartPanelControl10);
			this.gcAxisY.Controls.Add(this.chEnableAxixYScrolling);
			this.gcAxisY.Name = "gcAxisY";
			resources.ApplyResources(this.chEnableAxixYZooming, "chEnableAxixYZooming");
			this.chEnableAxixYZooming.Name = "chEnableAxixYZooming";
			this.chEnableAxixYZooming.Properties.Caption = resources.GetString("chEnableAxixYZooming.Properties.Caption");
			this.chEnableAxixYZooming.CheckedChanged += new System.EventHandler(this.chEnableAxixYZooming_CheckedChanged);
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Name = "chartPanelControl6";
			this.chartPanelControl10.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl10, "chartPanelControl10");
			this.chartPanelControl10.Name = "chartPanelControl10";
			resources.ApplyResources(this.chEnableAxixYScrolling, "chEnableAxixYScrolling");
			this.chEnableAxixYScrolling.Name = "chEnableAxixYScrolling";
			this.chEnableAxixYScrolling.Properties.Caption = resources.GetString("chEnableAxixYScrolling.Properties.Caption");
			this.chEnableAxixYScrolling.CheckedChanged += new System.EventHandler(this.chEnableAxixYScrolling_CheckedChanged);
			this.pnlSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlSeparator, "pnlSeparator");
			this.pnlSeparator.Name = "pnlSeparator";
			resources.ApplyResources(this.gcAxisX, "gcAxisX");
			this.gcAxisX.Controls.Add(this.chEnableAxixXZooming);
			this.gcAxisX.Controls.Add(this.chartPanelControl9);
			this.gcAxisX.Controls.Add(this.chartPanelControl8);
			this.gcAxisX.Controls.Add(this.chEnableAxixXScrolling);
			this.gcAxisX.Name = "gcAxisX";
			resources.ApplyResources(this.chEnableAxixXZooming, "chEnableAxixXZooming");
			this.chEnableAxixXZooming.Name = "chEnableAxixXZooming";
			this.chEnableAxixXZooming.Properties.Caption = resources.GetString("chEnableAxixXZooming.Properties.Caption");
			this.chEnableAxixXZooming.CheckedChanged += new System.EventHandler(this.chEnableAxixXZooming_CheckedChanged);
			resources.ApplyResources(this.chartPanelControl9, "chartPanelControl9");
			this.chartPanelControl9.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl9.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl9.Name = "chartPanelControl9";
			this.chartPanelControl8.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl8, "chartPanelControl8");
			this.chartPanelControl8.Name = "chartPanelControl8";
			resources.ApplyResources(this.chEnableAxixXScrolling, "chEnableAxixXScrolling");
			this.chEnableAxixXScrolling.Name = "chEnableAxixXScrolling";
			this.chEnableAxixXScrolling.Properties.Caption = resources.GetString("chEnableAxixXScrolling.Properties.Caption");
			this.chEnableAxixXScrolling.CheckedChanged += new System.EventHandler(this.chEnableAxixXScrolling_CheckedChanged);
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.gcScrollingOptions, "gcScrollingOptions");
			this.gcScrollingOptions.Controls.Add(this.scrollingOptions);
			this.gcScrollingOptions.Name = "gcScrollingOptions";
			resources.ApplyResources(this.scrollingOptions, "scrollingOptions");
			this.scrollingOptions.Name = "scrollingOptions";
			resources.ApplyResources(this.gcZoomingOptions, "gcZoomingOptions");
			this.gcZoomingOptions.Controls.Add(this.zoomingOptions);
			this.gcZoomingOptions.Name = "gcZoomingOptions";
			resources.ApplyResources(this.zoomingOptions, "zoomingOptions");
			this.zoomingOptions.Name = "zoomingOptions";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.gcZoomingOptions);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.gcScrollingOptions);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.gcAxisY);
			this.Controls.Add(this.pnlSeparator);
			this.Controls.Add(this.gcAxisX);
			this.Name = "ScrollingZooming2DControl";
			((System.ComponentModel.ISupportInitialize)(this.gcAxisY)).EndInit();
			this.gcAxisY.ResumeLayout(false);
			this.gcAxisY.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixYZooming.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixYScrolling.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisX)).EndInit();
			this.gcAxisX.ResumeLayout(false);
			this.gcAxisX.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixXZooming.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxixXScrolling.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcScrollingOptions)).EndInit();
			this.gcScrollingOptions.ResumeLayout(false);
			this.gcScrollingOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcZoomingOptions)).EndInit();
			this.gcZoomingOptions.ResumeLayout(false);
			this.gcZoomingOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl gcAxisY;
		private DevExpress.XtraEditors.CheckEdit chEnableAxixYZooming;
		private ChartPanelControl chartPanelControl6;
		private ChartPanelControl chartPanelControl10;
		private DevExpress.XtraEditors.CheckEdit chEnableAxixYScrolling;
		private ChartPanelControl pnlSeparator;
		private DevExpress.XtraEditors.GroupControl gcAxisX;
		private DevExpress.XtraEditors.CheckEdit chEnableAxixXZooming;
		private ChartPanelControl chartPanelControl9;
		private ChartPanelControl chartPanelControl8;
		private DevExpress.XtraEditors.CheckEdit chEnableAxixXScrolling;
		private ChartPanelControl chartPanelControl1;
		private DevExpress.XtraEditors.GroupControl gcScrollingOptions;
		private ScrollingOptionsControl scrollingOptions;
		private DevExpress.XtraEditors.GroupControl gcZoomingOptions;
		private ZoomingOptionsControl zoomingOptions;
		private ChartPanelControl chartPanelControl2;
	}
}
