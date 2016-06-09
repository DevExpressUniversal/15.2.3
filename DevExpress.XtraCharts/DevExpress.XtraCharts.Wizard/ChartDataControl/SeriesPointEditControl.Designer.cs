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

namespace DevExpress.XtraCharts.Wizard {
	partial class SeriesPointEditControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraCharts.Wizard.ChartPanelControl panelControl;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesPointEditControl));
			DevExpress.LookAndFeel.Design.UserLookAndFeelDefault userLookAndFeelDefault1 = new DevExpress.LookAndFeel.Design.UserLookAndFeelDefault();
			this.pointsGrid = new DevExpress.XtraCharts.Design.PointsGrid();
			this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.lvSeries = new DevExpress.XtraCharts.Wizard.SeriesListBoxControl();
			panelControl = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(panelControl)).BeginInit();
			panelControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
			this.splitContainerControl.SuspendLayout();
			this.SuspendLayout();
			panelControl.BackColor = System.Drawing.Color.Transparent;
			panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			panelControl.Controls.Add(this.pointsGrid);
			resources.ApplyResources(panelControl, "panelControl");
			panelControl.Name = "panelControl";
			this.pointsGrid.AllowSorting = false;
			this.pointsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.pointsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.pointsGrid.CaptionVisible = false;
			this.pointsGrid.DataMember = "";
			resources.ApplyResources(this.pointsGrid, "pointsGrid");
			this.pointsGrid.FlatMode = true;
			this.pointsGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.pointsGrid.LookAndFeel = userLookAndFeelDefault1;
			this.pointsGrid.Name = "pointsGrid";
			this.pointsGrid.ParentRowsVisible = false;
			resources.ApplyResources(this.splitContainerControl, "splitContainerControl");
			this.splitContainerControl.Name = "splitContainerControl";
			this.splitContainerControl.Panel1.Controls.Add(this.lvSeries);
			this.splitContainerControl.Panel2.Controls.Add(panelControl);
			this.splitContainerControl.SplitterPosition = 250;
			resources.ApplyResources(this.lvSeries, "lvSeries");
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.SelectedSeriesChanged += new System.EventHandler(this.lvSeries_SelectedIndexChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splitContainerControl);
			this.Name = "SeriesPointEditControl";
			((System.ComponentModel.ISupportInitialize)(panelControl)).EndInit();
			panelControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
			this.splitContainerControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private SeriesListBoxControl lvSeries;
		private DevExpress.XtraCharts.Design.PointsGrid pointsGrid;
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
	}
}
