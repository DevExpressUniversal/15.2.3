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
	partial class NestedDoughnutOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NestedDoughnutOptionsControl));
			this.grpNestedDoughnutOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlWeight = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtWeight = new DevExpress.XtraEditors.SpinEdit();
			this.lblWeight = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepWeight = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlInnerIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtInnerIndent = new DevExpress.XtraEditors.SpinEdit();
			this.lblInnerIndent = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepInnerIndent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlGroup = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.seriesGroupsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SeriesGroupsControl();
			this.lblGroup = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpNestedDoughnutOptions)).BeginInit();
			this.grpNestedDoughnutOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWeight)).BeginInit();
			this.pnlWeight.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerIndent)).BeginInit();
			this.pnlInnerIndent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtInnerIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepInnerIndent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGroup)).BeginInit();
			this.pnlGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpNestedDoughnutOptions, "grpNestedDoughnutOptions");
			this.grpNestedDoughnutOptions.Controls.Add(this.pnlWeight);
			this.grpNestedDoughnutOptions.Controls.Add(this.sepWeight);
			this.grpNestedDoughnutOptions.Controls.Add(this.pnlInnerIndent);
			this.grpNestedDoughnutOptions.Controls.Add(this.sepInnerIndent);
			this.grpNestedDoughnutOptions.Controls.Add(this.pnlGroup);
			this.grpNestedDoughnutOptions.Controls.Add(this.pnlSize);
			this.grpNestedDoughnutOptions.Name = "grpNestedDoughnutOptions";
			resources.ApplyResources(this.pnlWeight, "pnlWeight");
			this.pnlWeight.BackColor = System.Drawing.Color.Transparent;
			this.pnlWeight.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlWeight.Controls.Add(this.txtWeight);
			this.pnlWeight.Controls.Add(this.lblWeight);
			this.pnlWeight.Name = "pnlWeight";
			resources.ApplyResources(this.txtWeight, "txtWeight");
			this.txtWeight.Name = "txtWeight";
			this.txtWeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtWeight.Properties.MaxValue = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.txtWeight.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtWeight.Properties.ValidateOnEnterKey = true;
			this.txtWeight.EditValueChanged += new System.EventHandler(this.txtWeight_EditValueChanged);
			resources.ApplyResources(this.lblWeight, "lblWeight");
			this.lblWeight.Name = "lblWeight";
			this.sepWeight.BackColor = System.Drawing.Color.Transparent;
			this.sepWeight.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepWeight, "sepWeight");
			this.sepWeight.Name = "sepWeight";
			resources.ApplyResources(this.pnlInnerIndent, "pnlInnerIndent");
			this.pnlInnerIndent.BackColor = System.Drawing.Color.Transparent;
			this.pnlInnerIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlInnerIndent.Controls.Add(this.txtInnerIndent);
			this.pnlInnerIndent.Controls.Add(this.lblInnerIndent);
			this.pnlInnerIndent.Name = "pnlInnerIndent";
			resources.ApplyResources(this.txtInnerIndent, "txtInnerIndent");
			this.txtInnerIndent.Name = "txtInnerIndent";
			this.txtInnerIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtInnerIndent.Properties.MaxValue = new decimal(new int[] {
			2000,
			0,
			0,
			0});
			this.txtInnerIndent.Properties.ValidateOnEnterKey = true;
			this.txtInnerIndent.EditValueChanged += new System.EventHandler(this.txtInnerIndent_EditValueChanged);
			resources.ApplyResources(this.lblInnerIndent, "lblInnerIndent");
			this.lblInnerIndent.Name = "lblInnerIndent";
			this.sepInnerIndent.BackColor = System.Drawing.Color.Transparent;
			this.sepInnerIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepInnerIndent, "sepInnerIndent");
			this.sepInnerIndent.Name = "sepInnerIndent";
			resources.ApplyResources(this.pnlGroup, "pnlGroup");
			this.pnlGroup.BackColor = System.Drawing.Color.Transparent;
			this.pnlGroup.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGroup.Controls.Add(this.seriesGroupsControl);
			this.pnlGroup.Controls.Add(this.lblGroup);
			this.pnlGroup.Name = "pnlGroup";
			resources.ApplyResources(this.seriesGroupsControl, "seriesGroupsControl");
			this.seriesGroupsControl.Name = "seriesGroupsControl";
			this.seriesGroupsControl.SeriesGroupChanged += new System.EventHandler(this.seriesGroupsControl_SeriesGroupChanged);
			resources.ApplyResources(this.lblGroup, "lblGroup");
			this.lblGroup.Name = "lblGroup";
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Name = "pnlSize";
			this.Controls.Add(this.grpNestedDoughnutOptions);
			this.Name = "NestedDoughnutOptionsControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.grpNestedDoughnutOptions)).EndInit();
			this.grpNestedDoughnutOptions.ResumeLayout(false);
			this.grpNestedDoughnutOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWeight)).EndInit();
			this.pnlWeight.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepWeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlInnerIndent)).EndInit();
			this.pnlInnerIndent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtInnerIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepInnerIndent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlGroup)).EndInit();
			this.pnlGroup.ResumeLayout(false);
			this.pnlGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.GroupControl grpNestedDoughnutOptions;
		private ChartPanelControl pnlWeight;
		private XtraEditors.SpinEdit txtWeight;
		private ChartLabelControl lblWeight;
		private ChartPanelControl sepWeight;
		private ChartPanelControl pnlInnerIndent;
		private XtraEditors.SpinEdit txtInnerIndent;
		private ChartLabelControl lblInnerIndent;
		private ChartPanelControl sepInnerIndent;
		private ChartPanelControl pnlGroup;
		private ChartLabelControl lblGroup;
		private ChartPanelControl pnlSize;
		private SeriesGroupsControl seriesGroupsControl;
	}
}
