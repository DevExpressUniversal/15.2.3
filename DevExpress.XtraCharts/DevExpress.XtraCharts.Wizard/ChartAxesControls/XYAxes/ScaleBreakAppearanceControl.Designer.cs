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
	partial class ScaleBreakAppearanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleBreakAppearanceControl));
			this.pnlStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblStyle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnSize = new DevExpress.XtraEditors.SpinEdit();
			this.lblSize = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlStyle)).BeginInit();
			this.pnlStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			this.pnlSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).BeginInit();
			this.pnlColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlStyle, "pnlStyle");
			this.pnlStyle.BackColor = System.Drawing.Color.Transparent;
			this.pnlStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlStyle.Controls.Add(this.cbStyle);
			this.pnlStyle.Controls.Add(this.lblStyle);
			this.pnlStyle.Name = "pnlStyle";
			resources.ApplyResources(this.cbStyle, "cbStyle");
			this.cbStyle.Name = "cbStyle";
			this.cbStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbStyle.Properties.Buttons"))))});
			this.cbStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbStyle.SelectedIndexChanged += new System.EventHandler(this.cbStyle_SelectedIndexChanged);
			resources.ApplyResources(this.lblStyle, "lblStyle");
			this.lblStyle.Name = "lblStyle";
			this.sepStyle.BackColor = System.Drawing.Color.Transparent;
			this.sepStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepStyle, "sepStyle");
			this.sepStyle.Name = "sepStyle";
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Controls.Add(this.spnSize);
			this.pnlSize.Controls.Add(this.lblSize);
			this.pnlSize.Name = "pnlSize";
			resources.ApplyResources(this.spnSize, "spnSize");
			this.spnSize.Name = "spnSize";
			this.spnSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSize.Properties.IsFloatValue = false;
			this.spnSize.Properties.Mask.EditMask = resources.GetString("spnSize.Properties.Mask.EditMask");
			this.spnSize.Properties.MaxValue = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.spnSize.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.spnSize.Properties.ValidateOnEnterKey = true;
			this.spnSize.EditValueChanged += new System.EventHandler(this.spnSize_EditValueChanged);
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			this.sepSize.BackColor = System.Drawing.Color.Transparent;
			this.sepSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSize, "sepSize");
			this.sepSize.Name = "sepSize";
			resources.ApplyResources(this.pnlColor, "pnlColor");
			this.pnlColor.BackColor = System.Drawing.Color.Transparent;
			this.pnlColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlColor.Controls.Add(this.ceColor);
			this.pnlColor.Controls.Add(this.lblColor);
			this.pnlColor.Name = "pnlColor";
			resources.ApplyResources(this.ceColor, "ceColor");
			this.ceColor.Name = "ceColor";
			this.ceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceColor.Properties.Buttons"))))});
			this.ceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceColor.EditValueChanged += new System.EventHandler(this.ceColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlColor);
			this.Controls.Add(this.sepSize);
			this.Controls.Add(this.pnlSize);
			this.Controls.Add(this.sepStyle);
			this.Controls.Add(this.pnlStyle);
			this.Name = "ScaleBreakAppearanceControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlStyle)).EndInit();
			this.pnlStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			this.pnlSize.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlColor)).EndInit();
			this.pnlColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlStyle;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblStyle;
		private DevExpress.XtraEditors.ComboBoxEdit cbStyle;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepStyle;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlSize;
		private DevExpress.XtraEditors.SpinEdit spnSize;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblSize;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepSize;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlColor;
		private DevExpress.XtraEditors.ColorEdit ceColor;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblColor;
	}
}
