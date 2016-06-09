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
	partial class SeriesViewAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesViewAppearanceControl));
			this.grpColor = new DevExpress.XtraEditors.GroupControl();
			this.colorControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ColorControl();
			this.sepFillStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpFillStyle = new DevExpress.XtraEditors.GroupControl();
			this.fillStylesControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FillStylesControl();
			this.sepLineStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpLineStyle = new DevExpress.XtraEditors.GroupControl();
			this.lineStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			((System.ComponentModel.ISupportInitialize)(this.grpColor)).BeginInit();
			this.grpColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).BeginInit();
			this.grpFillStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepLineStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLineStyle)).BeginInit();
			this.grpLineStyle.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.grpColor, "grpColor");
			this.grpColor.Controls.Add(this.colorControl);
			this.grpColor.Name = "grpColor";
			resources.ApplyResources(this.colorControl, "colorControl");
			this.colorControl.Name = "colorControl";
			this.sepFillStyle.BackColor = System.Drawing.Color.Transparent;
			this.sepFillStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepFillStyle, "sepFillStyle");
			this.sepFillStyle.Name = "sepFillStyle";
			resources.ApplyResources(this.grpFillStyle, "grpFillStyle");
			this.grpFillStyle.Controls.Add(this.fillStylesControl);
			this.grpFillStyle.Name = "grpFillStyle";
			resources.ApplyResources(this.fillStylesControl, "fillStylesControl");
			this.fillStylesControl.Name = "fillStylesControl";
			this.sepLineStyle.BackColor = System.Drawing.Color.Transparent;
			this.sepLineStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepLineStyle, "sepLineStyle");
			this.sepLineStyle.Name = "sepLineStyle";
			resources.ApplyResources(this.grpLineStyle, "grpLineStyle");
			this.grpLineStyle.Controls.Add(this.lineStyleControl);
			this.grpLineStyle.Name = "grpLineStyle";
			resources.ApplyResources(this.lineStyleControl, "lineStyleControl");
			this.lineStyleControl.Name = "lineStyleControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpLineStyle);
			this.Controls.Add(this.sepLineStyle);
			this.Controls.Add(this.grpFillStyle);
			this.Controls.Add(this.sepFillStyle);
			this.Controls.Add(this.grpColor);
			this.Name = "SeriesViewAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.grpColor)).EndInit();
			this.grpColor.ResumeLayout(false);
			this.grpColor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).EndInit();
			this.grpFillStyle.ResumeLayout(false);
			this.grpFillStyle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepLineStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLineStyle)).EndInit();
			this.grpLineStyle.ResumeLayout(false);
			this.grpLineStyle.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpColor;
		private ColorControl colorControl;
		private ChartPanelControl sepFillStyle;
		private DevExpress.XtraEditors.GroupControl grpFillStyle;
		private FillStylesControl fillStylesControl;
		private ChartPanelControl sepLineStyle;
		private DevExpress.XtraEditors.GroupControl grpLineStyle;
		private LineStyleControl lineStyleControl;
	}
}
