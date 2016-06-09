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
	partial class Line3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Line3DGeneralOptionsControl));
			this.grpLineOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlThickness = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtThickness = new DevExpress.XtraEditors.SpinEdit();
			this.lblThickness = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpLineOptions)).BeginInit();
			this.grpLineOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).BeginInit();
			this.pnlWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).BeginInit();
			this.pnlThickness.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpLineOptions, "grpLineOptions");
			this.grpLineOptions.Controls.Add(this.pnlWidth);
			this.grpLineOptions.Controls.Add(this.sepWidth);
			this.grpLineOptions.Controls.Add(this.pnlThickness);
			this.grpLineOptions.Name = "grpLineOptions";
			resources.ApplyResources(this.pnlWidth, "pnlWidth");
			this.pnlWidth.BackColor = System.Drawing.Color.Transparent;
			this.pnlWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlWidth.Controls.Add(this.txtWidth);
			this.pnlWidth.Controls.Add(this.lblWidth);
			this.pnlWidth.Name = "pnlWidth";
			resources.ApplyResources(this.txtWidth, "txtWidth");
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Properties.Appearance.Options.UseTextOptions = true;
			this.txtWidth.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtWidth.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtWidth.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtWidth.Properties.Mask.IgnoreMaskBlank")));
			this.txtWidth.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtWidth.Properties.Mask.ShowPlaceHolders")));
			this.txtWidth.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtWidth.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			262144});
			this.txtWidth.EditValueChanged += new System.EventHandler(this.txtWidth_EditValueChanged);
			resources.ApplyResources(this.lblWidth, "lblWidth");
			this.lblWidth.Name = "lblWidth";
			this.sepWidth.BackColor = System.Drawing.Color.Transparent;
			this.sepWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepWidth, "sepWidth");
			this.sepWidth.Name = "sepWidth";
			resources.ApplyResources(this.pnlThickness, "pnlThickness");
			this.pnlThickness.BackColor = System.Drawing.Color.Transparent;
			this.pnlThickness.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlThickness.Controls.Add(this.txtThickness);
			this.pnlThickness.Controls.Add(this.lblThickness);
			this.pnlThickness.Name = "pnlThickness";
			resources.ApplyResources(this.txtThickness, "txtThickness");
			this.txtThickness.Name = "txtThickness";
			this.txtThickness.Properties.Appearance.Options.UseTextOptions = true;
			this.txtThickness.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtThickness.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtThickness.Properties.IsFloatValue = false;
			this.txtThickness.Properties.Mask.EditMask = resources.GetString("txtThickness.Properties.Mask.EditMask");
			this.txtThickness.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtThickness.Properties.Mask.IgnoreMaskBlank")));
			this.txtThickness.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtThickness.Properties.Mask.ShowPlaceHolders")));
			this.txtThickness.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtThickness.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtThickness.EditValueChanged += new System.EventHandler(this.txtThickness_EditValueChanged);
			resources.ApplyResources(this.lblThickness, "lblThickness");
			this.lblThickness.Name = "lblThickness";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpLineOptions);
			this.Name = "Line3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpLineOptions)).EndInit();
			this.grpLineOptions.ResumeLayout(false);
			this.grpLineOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).EndInit();
			this.pnlWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlThickness)).EndInit();
			this.pnlThickness.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtThickness.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpLineOptions;
		private ChartPanelControl pnlWidth;
		private DevExpress.XtraEditors.SpinEdit txtWidth;
		private ChartLabelControl lblWidth;
		private ChartPanelControl sepWidth;
		private ChartPanelControl pnlThickness;
		private DevExpress.XtraEditors.SpinEdit txtThickness;
		private ChartLabelControl lblThickness;
	}
}
