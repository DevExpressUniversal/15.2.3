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
	partial class ColorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorControl));
			this.chColorEach = new DevExpress.XtraEditors.CheckEdit();
			this.sepColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.clrColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepTransparency = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlTransparency = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnTransparency = new DevExpress.XtraEditors.SpinEdit();
			this.lblTransparency = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.chColorEach.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).BeginInit();
			this.pnlColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTransparency)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTransparency)).BeginInit();
			this.pnlTransparency.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnTransparency.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chColorEach, "chColorEach");
			this.chColorEach.Name = "chColorEach";
			this.chColorEach.Properties.Caption = resources.GetString("chColorEach.Properties.Caption");
			this.chColorEach.CheckedChanged += new System.EventHandler(this.chColorEach_CheckedChanged);
			this.sepColor.BackColor = System.Drawing.Color.Transparent;
			this.sepColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepColor, "sepColor");
			this.sepColor.Name = "sepColor";
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
			this.sepTransparency.BackColor = System.Drawing.Color.Transparent;
			this.sepTransparency.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepTransparency, "sepTransparency");
			this.sepTransparency.Name = "sepTransparency";
			resources.ApplyResources(this.pnlTransparency, "pnlTransparency");
			this.pnlTransparency.BackColor = System.Drawing.Color.Transparent;
			this.pnlTransparency.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTransparency.Controls.Add(this.spnTransparency);
			this.pnlTransparency.Controls.Add(this.lblTransparency);
			this.pnlTransparency.Name = "pnlTransparency";
			resources.ApplyResources(this.spnTransparency, "spnTransparency");
			this.spnTransparency.Name = "spnTransparency";
			this.spnTransparency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnTransparency.Properties.IsFloatValue = false;
			this.spnTransparency.Properties.Mask.EditMask = resources.GetString("spnTransparency.Properties.Mask.EditMask");
			this.spnTransparency.Properties.MaxValue = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.spnTransparency.EditValueChanged += new System.EventHandler(this.spnTransparency_EditValueChanged);
			resources.ApplyResources(this.lblTransparency, "lblTransparency");
			this.lblTransparency.Name = "lblTransparency";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlTransparency);
			this.Controls.Add(this.sepTransparency);
			this.Controls.Add(this.pnlColor);
			this.Controls.Add(this.sepColor);
			this.Controls.Add(this.chColorEach);
			this.Name = "ColorControl";
			((System.ComponentModel.ISupportInitialize)(this.chColorEach.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).EndInit();
			this.pnlColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepTransparency)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTransparency)).EndInit();
			this.pnlTransparency.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnTransparency.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chColorEach;
		private ChartPanelControl sepColor;
		private ChartPanelControl pnlColor;
		private DevExpress.XtraEditors.ColorEdit clrColor;
		private ChartLabelControl lblColor;
		private ChartPanelControl sepTransparency;
		private ChartPanelControl pnlTransparency;
		private ChartLabelControl lblTransparency;
		private DevExpress.XtraEditors.SpinEdit spnTransparency;
	}
}
