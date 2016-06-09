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
	partial class SeriesListBoxControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (boldListBoxFont != null) {
					boldListBoxFont.Dispose();
					boldListBoxFont = null;
				}
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraEditors.PanelControl panelControl1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesListBoxControl));
			this.lvSeries = new DevExpress.XtraEditors.ImageListBoxControl();
			this.seriesImages = new System.Windows.Forms.ImageList(this.components);
			this.ttController = new DevExpress.Utils.ToolTipController(this.components);
			this.pnlChangeCount = new DevExpress.XtraEditors.PanelControl();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
			this.btnCopy = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.pnlChangePosition = new DevExpress.XtraEditors.PanelControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			panelControl1 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(panelControl1)).BeginInit();
			panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlChangeCount)).BeginInit();
			this.pnlChangeCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlChangePosition)).BeginInit();
			this.pnlChangePosition.SuspendLayout();
			this.SuspendLayout();
			panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			panelControl1.Controls.Add(this.lvSeries);
			panelControl1.Controls.Add(this.pnlChangeCount);
			panelControl1.Controls.Add(this.pnlChangePosition);
			resources.ApplyResources(panelControl1, "panelControl1");
			panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.lvSeries, "lvSeries");
			this.lvSeries.ImageList = this.seriesImages;
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.ToolTipController = this.ttController;
			this.lvSeries.SelectedIndexChanged += new System.EventHandler(this.lvSeries_SelectedIndexChanged);
			this.lvSeries.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lvSeries_DrawItem);
			this.lvSeries.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvSeries_MouseMove);
			this.seriesImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			resources.ApplyResources(this.seriesImages, "seriesImages");
			this.seriesImages.TransparentColor = System.Drawing.Color.Magenta;
			this.pnlChangeCount.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlChangeCount.Controls.Add(this.btnRemove);
			this.pnlChangeCount.Controls.Add(this.panelControl3);
			this.pnlChangeCount.Controls.Add(this.btnCopy);
			this.pnlChangeCount.Controls.Add(this.panelControl2);
			this.pnlChangeCount.Controls.Add(this.btnAdd);
			resources.ApplyResources(this.pnlChangeCount, "pnlChangeCount");
			this.pnlChangeCount.Name = "pnlChangeCount";
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			resources.ApplyResources(this.btnCopy, "btnCopy");
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.pnlChangePosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlChangePosition.Controls.Add(this.btnUp);
			this.pnlChangePosition.Controls.Add(this.btnDown);
			resources.ApplyResources(this.pnlChangePosition, "pnlChangePosition");
			this.pnlChangePosition.Name = "pnlChangePosition";
			this.btnUp.ImageIndex = 0;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDown.ImageIndex = 1;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(panelControl1);
			this.MinimumSize = new System.Drawing.Size(0, 100);
			this.Name = "SeriesListBoxControl";
			((System.ComponentModel.ISupportInitialize)(panelControl1)).EndInit();
			panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlChangeCount)).EndInit();
			this.pnlChangeCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlChangePosition)).EndInit();
			this.pnlChangePosition.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnRemove;
		private DevExpress.XtraEditors.SimpleButton btnUp;
		private DevExpress.XtraEditors.SimpleButton btnDown;
		private DevExpress.XtraEditors.PanelControl pnlChangePosition;
		private System.Windows.Forms.ImageList seriesImages;
		private DevExpress.XtraEditors.ImageListBoxControl lvSeries;
		private DevExpress.XtraEditors.PanelControl pnlChangeCount;
		private DevExpress.Utils.ToolTipController ttController;
		private DevExpress.XtraEditors.PanelControl panelControl3;
		private DevExpress.XtraEditors.SimpleButton btnCopy;
		private DevExpress.XtraEditors.PanelControl panelControl2;
	}
}
