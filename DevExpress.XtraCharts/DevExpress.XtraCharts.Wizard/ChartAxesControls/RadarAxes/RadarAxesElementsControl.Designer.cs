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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	partial class RadarAxesElementsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadarAxesElementsControl));
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.pageTickmarks = new DevExpress.XtraTab.XtraTabPage();
			this.tickmarksControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.TickmarksControl();
			this.pageGridLines = new DevExpress.XtraTab.XtraTabPage();
			this.gridLinesControl = new DevExpress.XtraCharts.Wizard.ChartAxesControls.GridLinesControl();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.pageTickmarks.SuspendLayout();
			this.pageGridLines.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.pageTickmarks;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageTickmarks,
			this.pageGridLines});
			this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
			this.pageTickmarks.Controls.Add(this.tickmarksControl);
			this.pageTickmarks.Name = "pageTickmarks";
			resources.ApplyResources(this.pageTickmarks, "pageTickmarks");
			resources.ApplyResources(this.tickmarksControl, "tickmarksControl");
			this.tickmarksControl.Name = "tickmarksControl";
			this.pageGridLines.Controls.Add(this.gridLinesControl);
			this.pageGridLines.Name = "pageGridLines";
			resources.ApplyResources(this.pageGridLines, "pageGridLines");
			resources.ApplyResources(this.gridLinesControl, "gridLinesControl");
			this.gridLinesControl.Name = "gridLinesControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.xtraTabControl1);
			this.Name = "RadarAxesElementsControl";
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.pageTickmarks.ResumeLayout(false);
			this.pageTickmarks.PerformLayout();
			this.pageGridLines.ResumeLayout(false);
			this.pageGridLines.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage pageTickmarks;
		private DevExpress.XtraTab.XtraTabPage pageGridLines;
		private GridLinesControl gridLinesControl;
		private TickmarksControl tickmarksControl;
	}
}
