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
	partial class Bar3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bar3DGeneralOptionsControl));
			this.grpBarOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepDistanceFixed = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDistance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDistance = new DevExpress.XtraEditors.SpinEdit();
			this.lblDistance = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepDistance = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDepth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtDepth = new DevExpress.XtraEditors.SpinEdit();
			this.lblDepth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepDepth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEqualBarWidth = new DevExpress.XtraEditors.CheckEdit();
			this.chBarDepthAuto = new DevExpress.XtraEditors.CheckEdit();
			this.sepModelOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpModelOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlModel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbModel = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblModel = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepModel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chShowFacet = new DevExpress.XtraEditors.CheckEdit();
			this.txtDistanceFixed = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpBarOptions)).BeginInit();
			this.grpBarOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistanceFixed)).BeginInit();
			this.pnlDistanceFixed.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepDistanceFixed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistance)).BeginInit();
			this.pnlDistance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).BeginInit();
			this.pnlWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDepth)).BeginInit();
			this.pnlDepth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDepth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDepth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEqualBarWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chBarDepthAuto.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepModelOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpModelOptions)).BeginInit();
			this.grpModelOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlModel)).BeginInit();
			this.pnlModel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepModel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowFacet.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDistanceFixed.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpBarOptions, "grpBarOptions");
			this.grpBarOptions.Controls.Add(this.pnlDistanceFixed);
			this.grpBarOptions.Controls.Add(this.sepDistanceFixed);
			this.grpBarOptions.Controls.Add(this.pnlDistance);
			this.grpBarOptions.Controls.Add(this.sepDistance);
			this.grpBarOptions.Controls.Add(this.pnlWidth);
			this.grpBarOptions.Controls.Add(this.sepWidth);
			this.grpBarOptions.Controls.Add(this.pnlDepth);
			this.grpBarOptions.Controls.Add(this.sepDepth);
			this.grpBarOptions.Controls.Add(this.chEqualBarWidth);
			this.grpBarOptions.Controls.Add(this.chBarDepthAuto);
			this.grpBarOptions.Name = "grpBarOptions";
			resources.ApplyResources(this.pnlDistanceFixed, "pnlDistanceFixed");
			this.pnlDistanceFixed.BackColor = System.Drawing.Color.Transparent;
			this.pnlDistanceFixed.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDistanceFixed.Controls.Add(this.txtDistanceFixed);
			this.pnlDistanceFixed.Controls.Add(this.lblDistanceFixed);
			this.pnlDistanceFixed.Name = "pnlDistanceFixed";
			resources.ApplyResources(this.lblDistanceFixed, "lblDistanceFixed");
			this.lblDistanceFixed.Name = "lblDistanceFixed";
			this.sepDistanceFixed.BackColor = System.Drawing.Color.Transparent;
			this.sepDistanceFixed.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepDistanceFixed, "sepDistanceFixed");
			this.sepDistanceFixed.Name = "sepDistanceFixed";
			resources.ApplyResources(this.pnlDistance, "pnlDistance");
			this.pnlDistance.BackColor = System.Drawing.Color.Transparent;
			this.pnlDistance.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDistance.Controls.Add(this.txtDistance);
			this.pnlDistance.Controls.Add(this.lblDistance);
			this.pnlDistance.Name = "pnlDistance";
			resources.ApplyResources(this.txtDistance, "txtDistance");
			this.txtDistance.Name = "txtDistance";
			this.txtDistance.Properties.Appearance.Options.UseTextOptions = true;
			this.txtDistance.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtDistance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDistance.Properties.Increment = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.txtDistance.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtDistance.Properties.Mask.IgnoreMaskBlank")));
			this.txtDistance.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtDistance.Properties.Mask.ShowPlaceHolders")));
			this.txtDistance.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtDistance.EditValueChanged += new System.EventHandler(this.txtDistance_EditValueChanged);
			resources.ApplyResources(this.lblDistance, "lblDistance");
			this.lblDistance.Name = "lblDistance";
			this.sepDistance.BackColor = System.Drawing.Color.Transparent;
			this.sepDistance.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepDistance, "sepDistance");
			this.sepDistance.Name = "sepDistance";
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
			resources.ApplyResources(this.pnlDepth, "pnlDepth");
			this.pnlDepth.BackColor = System.Drawing.Color.Transparent;
			this.pnlDepth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDepth.Controls.Add(this.txtDepth);
			this.pnlDepth.Controls.Add(this.lblDepth);
			this.pnlDepth.Name = "pnlDepth";
			resources.ApplyResources(this.txtDepth, "txtDepth");
			this.txtDepth.Name = "txtDepth";
			this.txtDepth.Properties.Appearance.Options.UseTextOptions = true;
			this.txtDepth.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtDepth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtDepth.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtDepth.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtDepth.Properties.Mask.IgnoreMaskBlank")));
			this.txtDepth.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtDepth.Properties.Mask.ShowPlaceHolders")));
			this.txtDepth.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtDepth.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			262144});
			this.txtDepth.EditValueChanged += new System.EventHandler(this.txtDepth_EditValueChanged);
			resources.ApplyResources(this.lblDepth, "lblDepth");
			this.lblDepth.Name = "lblDepth";
			this.sepDepth.BackColor = System.Drawing.Color.Transparent;
			this.sepDepth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepDepth, "sepDepth");
			this.sepDepth.Name = "sepDepth";
			resources.ApplyResources(this.chEqualBarWidth, "chEqualBarWidth");
			this.chEqualBarWidth.Name = "chEqualBarWidth";
			this.chEqualBarWidth.Properties.Caption = resources.GetString("chEqualBarWidth.Properties.Caption");
			this.chEqualBarWidth.CheckedChanged += new System.EventHandler(this.chEqualBarWidth_CheckedChanged);
			resources.ApplyResources(this.chBarDepthAuto, "chBarDepthAuto");
			this.chBarDepthAuto.Name = "chBarDepthAuto";
			this.chBarDepthAuto.Properties.Caption = resources.GetString("chBarDepthAuto.Properties.Caption");
			this.chBarDepthAuto.CheckedChanged += new System.EventHandler(this.chBarDepthAuto_CheckedChanged);
			this.sepModelOptions.BackColor = System.Drawing.Color.Transparent;
			this.sepModelOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepModelOptions, "sepModelOptions");
			this.sepModelOptions.Name = "sepModelOptions";
			resources.ApplyResources(this.grpModelOptions, "grpModelOptions");
			this.grpModelOptions.Controls.Add(this.pnlModel);
			this.grpModelOptions.Controls.Add(this.sepModel);
			this.grpModelOptions.Controls.Add(this.chShowFacet);
			this.grpModelOptions.Name = "grpModelOptions";
			resources.ApplyResources(this.pnlModel, "pnlModel");
			this.pnlModel.BackColor = System.Drawing.Color.Transparent;
			this.pnlModel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlModel.Controls.Add(this.cbModel);
			this.pnlModel.Controls.Add(this.lblModel);
			this.pnlModel.Name = "pnlModel";
			resources.ApplyResources(this.cbModel, "cbModel");
			this.cbModel.Name = "cbModel";
			this.cbModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbModel.Properties.Buttons"))))});
			this.cbModel.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbModel.SelectedIndexChanged += new System.EventHandler(this.cbModel_SelectedIndexChanged);
			resources.ApplyResources(this.lblModel, "lblModel");
			this.lblModel.Name = "lblModel";
			this.sepModel.BackColor = System.Drawing.Color.Transparent;
			this.sepModel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepModel, "sepModel");
			this.sepModel.Name = "sepModel";
			resources.ApplyResources(this.chShowFacet, "chShowFacet");
			this.chShowFacet.Name = "chShowFacet";
			this.chShowFacet.Properties.Caption = resources.GetString("chShowFacet.Properties.Caption");
			this.chShowFacet.CheckedChanged += new System.EventHandler(this.chShowFacet_CheckedChanged);
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
			this.Controls.Add(this.grpModelOptions);
			this.Controls.Add(this.sepModelOptions);
			this.Controls.Add(this.grpBarOptions);
			this.Name = "Bar3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpBarOptions)).EndInit();
			this.grpBarOptions.ResumeLayout(false);
			this.grpBarOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistanceFixed)).EndInit();
			this.pnlDistanceFixed.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sepDistanceFixed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDistance)).EndInit();
			this.pnlDistance.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDistance.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).EndInit();
			this.pnlWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDepth)).EndInit();
			this.pnlDepth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtDepth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepDepth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEqualBarWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chBarDepthAuto.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepModelOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpModelOptions)).EndInit();
			this.grpModelOptions.ResumeLayout(false);
			this.grpModelOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlModel)).EndInit();
			this.pnlModel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepModel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chShowFacet.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDistanceFixed.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpBarOptions;
		private ChartPanelControl pnlDistanceFixed;
		private ChartLabelControl lblDistanceFixed;
		private ChartPanelControl sepDistanceFixed;
		private ChartPanelControl pnlDistance;
		protected DevExpress.XtraEditors.SpinEdit txtDistance;
		private ChartLabelControl lblDistance;
		private ChartPanelControl sepDistance;
		private ChartPanelControl pnlWidth;
		private DevExpress.XtraEditors.SpinEdit txtWidth;
		private ChartLabelControl lblWidth;
		private ChartPanelControl sepWidth;
		private ChartPanelControl pnlDepth;
		private DevExpress.XtraEditors.SpinEdit txtDepth;
		private ChartLabelControl lblDepth;
		private ChartPanelControl sepDepth;
		private DevExpress.XtraEditors.CheckEdit chEqualBarWidth;
		private DevExpress.XtraEditors.CheckEdit chBarDepthAuto;
		private ChartPanelControl sepModelOptions;
		private DevExpress.XtraEditors.GroupControl grpModelOptions;
		private ChartPanelControl sepModel;
		private DevExpress.XtraEditors.CheckEdit chShowFacet;
		private ChartPanelControl pnlModel;
		private DevExpress.XtraEditors.ComboBoxEdit cbModel;
		private ChartLabelControl lblModel;
		private XtraEditors.SpinEdit txtDistanceFixed;
	}
}
