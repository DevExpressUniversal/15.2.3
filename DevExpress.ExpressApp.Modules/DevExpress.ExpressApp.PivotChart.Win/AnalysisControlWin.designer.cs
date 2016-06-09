#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.ExpressApp.PivotChart.Win {
	partial class AnalysisControlWin {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			chartControlShowWizardMenuItem.Click -= new EventHandler(chartControlContextMenuItem_Click);
			pivotGrid.GridLayout -= new EventHandler(pivotGrid_GridLayout);
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraCharts.SideBySideBarSeriesView sideBySideBarSeriesView1 = new DevExpress.XtraCharts.SideBySideBarSeriesView();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pgPivot = new DevExpress.XtraTab.XtraTabPage();
			this.pivotGrid = new DevExpress.XtraPivotGrid.PivotGridControl();
			this.pgChart = new DevExpress.XtraTab.XtraTabPage();
			this.showRowGrandTotalCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.showColumnGrandTotalCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.chartControl = new DevExpress.XtraCharts.ChartControl();
			this.chartControlContextMenu = new System.Windows.Forms.ContextMenu();
			this.chartControlShowWizardMenuItem = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.pgPivot.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).BeginInit();
			this.pgChart.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.showRowGrandTotalCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.showColumnGrandTotalCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesView1)).BeginInit();
			this.SuspendLayout();
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.pgPivot;
			this.tabControl.Size = new System.Drawing.Size(732, 423);
			this.tabControl.TabIndex = 0;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pgPivot,
			this.pgChart});
			this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_SelectedPageChanged);
			this.pgPivot.Controls.Add(this.pivotGrid);
			this.pgPivot.Name = "pgPivot";
			this.pgPivot.Size = new System.Drawing.Size(723, 392);
			this.pgPivot.Text = "Pivot";
			this.pivotGrid.Cursor = System.Windows.Forms.Cursors.Default;			
			this.pivotGrid.Location = new System.Drawing.Point(3, 3);
			this.pivotGrid.Name = "pivotGrid";
			this.pivotGrid.Size = new System.Drawing.Size(746, 521);
			this.pivotGrid.TabIndex = 0;
			this.pgChart.Controls.Add(this.showRowGrandTotalCheckEdit);
			this.pgChart.Controls.Add(this.showColumnGrandTotalCheckEdit);
			this.pgChart.Controls.Add(this.chartControl);
			this.pgChart.Name = "pgChart";
			this.pgChart.Size = new System.Drawing.Size(723, 392);
			this.pgChart.Text = "Chart";
			this.showRowGrandTotalCheckEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.showRowGrandTotalCheckEdit.Location = new System.Drawing.Point(0, 373);
			this.showRowGrandTotalCheckEdit.Name = "showRowGrandTotalCheckEdit";
			this.showRowGrandTotalCheckEdit.Properties.Caption = "Show Row Grand Total";
			this.showRowGrandTotalCheckEdit.Size = new System.Drawing.Size(253, 19);
			this.showRowGrandTotalCheckEdit.TabIndex = 2;
			this.showRowGrandTotalCheckEdit.CheckedChanged += new System.EventHandler(this.ShowRowGrandTotal_CheckedChanged);			
			this.showColumnGrandTotalCheckEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.showColumnGrandTotalCheckEdit.Location = new System.Drawing.Point(0, 352);
			this.showColumnGrandTotalCheckEdit.Name = "showColumnGrandTotalCheckEdit";
			this.showColumnGrandTotalCheckEdit.Properties.Caption = "Show Column Grand Total";
			this.showColumnGrandTotalCheckEdit.Size = new System.Drawing.Size(253, 19);
			this.showColumnGrandTotalCheckEdit.TabIndex = 1;
			this.showColumnGrandTotalCheckEdit.CheckedChanged += new System.EventHandler(this.showColumnGrandTotalCheckEdit_CheckedChanged);			
			this.chartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.chartControl.ContextMenu = this.chartControlContextMenu;
			this.chartControl.Location = new System.Drawing.Point(0, 3);
			this.chartControl.Name = "chartControl";
			this.chartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
			this.chartControl.SeriesTemplate.View = sideBySideBarSeriesView1;
			this.chartControl.Size = new System.Drawing.Size(721, 343);
			this.chartControl.TabIndex = 0;
			this.chartControlContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.chartControlShowWizardMenuItem});
			this.chartControlShowWizardMenuItem.BarBreak = true;
			this.chartControlShowWizardMenuItem.Index = 0;
			this.chartControlShowWizardMenuItem.Text = "Chart Wizard";
			this.chartControlShowWizardMenuItem.Click += new System.EventHandler(this.chartControlContextMenuItem_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.Name = "AnalysisControlWin";
			this.Size = new System.Drawing.Size(732, 423);
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.pgPivot.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).EndInit();
			this.pgChart.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.showRowGrandTotalCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.showColumnGrandTotalCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(sideBySideBarSeriesView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartControl)).EndInit();
			this.ResumeLayout(false);
		}
#endregion
		private DevExpress.XtraTab.XtraTabControl tabControl;
		private DevExpress.XtraTab.XtraTabPage pgPivot;
		private DevExpress.XtraTab.XtraTabPage pgChart;
		private DevExpress.XtraPivotGrid.PivotGridControl pivotGrid;
		private DevExpress.XtraCharts.ChartControl chartControl;
		private System.Windows.Forms.ContextMenu chartControlContextMenu;
		private System.Windows.Forms.MenuItem chartControlShowWizardMenuItem;
		private DevExpress.XtraEditors.CheckEdit showColumnGrandTotalCheckEdit;
		private DevExpress.XtraEditors.CheckEdit showRowGrandTotalCheckEdit;
	}
}
