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
	partial class MarkerControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkerControl));
			this.sepSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.markerBaseControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerBaseControl();
			this.pnlColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.clrColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepMarkerBase = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).BeginInit();
			this.pnlColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMarkerBase)).BeginInit();
			this.SuspendLayout();
			this.sepSeparator.BackColor = System.Drawing.Color.Transparent;
			this.sepSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSeparator, "sepSeparator");
			this.sepSeparator.Name = "sepSeparator";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			resources.ApplyResources(this.markerBaseControl, "markerBaseControl");
			this.markerBaseControl.Name = "markerBaseControl";
			resources.ApplyResources(this.pnlColor, "pnlColor");
			this.pnlColor.BackColor = System.Drawing.Color.Transparent;
			this.pnlColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlColor.Controls.Add(this.clrColor);
			this.pnlColor.Controls.Add(this.lblColor);
			this.pnlColor.Name = "pnlColor";
			resources.ApplyResources(this.clrColor, "clrColor");
			this.clrColor.Name = "clrColor";
			this.clrColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("clrColor.Properties.Buttons"))))});
			this.clrColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.clrColor.EditValueChanged += new System.EventHandler(this.clrColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.sepMarkerBase.BackColor = System.Drawing.Color.Transparent;
			this.sepMarkerBase.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepMarkerBase, "sepMarkerBase");
			this.sepMarkerBase.Name = "sepMarkerBase";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.markerBaseControl);
			this.Controls.Add(this.sepMarkerBase);
			this.Controls.Add(this.pnlColor);
			this.Controls.Add(this.sepSeparator);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.chVisible);
			this.Name = "MarkerControl";
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).EndInit();
			this.pnlColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepMarkerBase)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl sepSeparator;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private MarkerBaseControl markerBaseControl;
		private ChartPanelControl pnlColor;
		private DevExpress.XtraEditors.ColorEdit clrColor;
		private ChartLabelControl lblColor;
		private ChartPanelControl sepMarkerBase;
	}
}
