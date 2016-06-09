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
	partial class BarDistanceControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BarDistanceControl));
			this.panelWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtWidth = new DevExpress.XtraEditors.SpinEdit();
			this.labelWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelEqualBarWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEqualBarWidth = new DevExpress.XtraEditors.CheckEdit();
			this.panelDistance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDistance = new DevExpress.XtraEditors.SpinEdit();
			this.labelDistance = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.txtDistanceFixed = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelWidth)).BeginInit();
			this.panelWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEqualBarWidth)).BeginInit();
			this.panelEqualBarWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEqualBarWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDistance)).BeginInit();
			this.panelDistance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDistanceFixed)).BeginInit();
			this.panelDistanceFixed.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDistanceFixed.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelWidth, "panelWidth");
			this.panelWidth.BackColor = System.Drawing.Color.Transparent;
			this.panelWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelWidth.Controls.Add(this.txtWidth);
			this.panelWidth.Controls.Add(this.labelWidth);
			this.panelWidth.Name = "panelWidth";
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
			100,
			0,
			0,
			0});
			this.txtWidth.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtWidth.EditValueChanged += new System.EventHandler(this.txtWidth_EditValueChanged);
			resources.ApplyResources(this.labelWidth, "labelWidth");
			this.labelWidth.Name = "labelWidth";
			resources.ApplyResources(this.panelEqualBarWidth, "panelEqualBarWidth");
			this.panelEqualBarWidth.BackColor = System.Drawing.Color.Transparent;
			this.panelEqualBarWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelEqualBarWidth.Controls.Add(this.chEqualBarWidth);
			this.panelEqualBarWidth.Name = "panelEqualBarWidth";
			resources.ApplyResources(this.chEqualBarWidth, "chEqualBarWidth");
			this.chEqualBarWidth.Name = "chEqualBarWidth";
			this.chEqualBarWidth.Properties.Caption = resources.GetString("chEqualBarWidth.Properties.Caption");
			this.chEqualBarWidth.CheckedChanged += new System.EventHandler(this.chEqualBarWidth_CheckedChanged);
			resources.ApplyResources(this.panelDistance, "panelDistance");
			this.panelDistance.BackColor = System.Drawing.Color.Transparent;
			this.panelDistance.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelDistance.Controls.Add(this.txtDistance);
			this.panelDistance.Controls.Add(this.labelDistance);
			this.panelDistance.Name = "panelDistance";
			resources.ApplyResources(this.txtDistance, "txtDistance");
			this.txtDistance.Name = "txtDistance";
			this.txtDistance.Properties.Appearance.Options.UseTextOptions = true;
			this.txtDistance.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDistance.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtDistance.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtDistance.Properties.Mask.IgnoreMaskBlank")));
			this.txtDistance.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtDistance.Properties.Mask.ShowPlaceHolders")));
			this.txtDistance.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtDistance.EditValueChanged += new System.EventHandler(this.txtDistance_EditValueChanged);
			resources.ApplyResources(this.labelDistance, "labelDistance");
			this.labelDistance.Name = "labelDistance";
			resources.ApplyResources(this.panelDistanceFixed, "panelDistanceFixed");
			this.panelDistanceFixed.BackColor = System.Drawing.Color.Transparent;
			this.panelDistanceFixed.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelDistanceFixed.Controls.Add(this.txtDistanceFixed);
			this.panelDistanceFixed.Controls.Add(this.labelDistanceFixed);
			this.panelDistanceFixed.Name = "panelDistanceFixed";
			resources.ApplyResources(this.labelDistanceFixed, "labelDistanceFixed");
			this.labelDistanceFixed.Name = "labelDistanceFixed";
			resources.ApplyResources(this.txtDistanceFixed, "txtDistanceFixed");
			this.txtDistanceFixed.Name = "txtDistanceFixed";
			this.txtDistanceFixed.Properties.Appearance.Options.UseTextOptions = true;
			this.txtDistanceFixed.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtDistanceFixed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDistanceFixed.Properties.Mask.EditMask = resources.GetString("txtDistanceFixed.Properties.Mask.EditMask");
			this.txtDistanceFixed.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtDistanceFixed.Properties.Mask.IgnoreMaskBlank")));
			this.txtDistanceFixed.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtDistanceFixed.Properties.Mask.ShowPlaceHolders")));
			this.txtDistanceFixed.EditValueChanged += new System.EventHandler(this.txtDistanceFixed_EditValueChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelDistanceFixed);
			this.Controls.Add(this.panelDistance);
			this.Controls.Add(this.panelWidth);
			this.Controls.Add(this.panelEqualBarWidth);
			this.Name = "BarDistanceControl";
			((System.ComponentModel.ISupportInitialize)(this.panelWidth)).EndInit();
			this.panelWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEqualBarWidth)).EndInit();
			this.panelEqualBarWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chEqualBarWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDistance)).EndInit();
			this.panelDistance.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDistanceFixed)).EndInit();
			this.panelDistanceFixed.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDistanceFixed.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl panelWidth;
		private ChartLabelControl labelWidth;
		private DevExpress.XtraEditors.SpinEdit txtWidth;
		private ChartPanelControl panelEqualBarWidth;
		private DevExpress.XtraEditors.CheckEdit chEqualBarWidth;
		private ChartPanelControl panelDistance;
		protected DevExpress.XtraEditors.SpinEdit txtDistance;
		private ChartLabelControl labelDistance;
		private ChartPanelControl panelDistanceFixed;
		private ChartLabelControl labelDistanceFixed;
		private XtraEditors.SpinEdit txtDistanceFixed;
	}
}
