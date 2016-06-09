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

namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	partial class LegendInteriorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegendInteriorControl));
			this.pnlEditors = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.groupIndents = new DevExpress.XtraEditors.GroupControl();
			this.speTextOffset = new DevExpress.XtraEditors.SpinEdit();
			this.lblTextOffset = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speHIndent = new DevExpress.XtraEditors.SpinEdit();
			this.lblHorizontal = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.speVIndent = new DevExpress.XtraEditors.SpinEdit();
			this.lblVertical = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl10 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grPadding = new DevExpress.XtraEditors.GroupControl();
			this.paddingControl = new DevExpress.XtraCharts.Wizard.RectangleIndentsControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupIndents)).BeginInit();
			this.groupIndents.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speTextOffset.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			this.chartPanelControl6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speHIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.speVIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grPadding)).BeginInit();
			this.grPadding.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlEditors, "pnlEditors");
			this.pnlEditors.BackColor = System.Drawing.Color.Transparent;
			this.pnlEditors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlEditors.Name = "pnlEditors";
			resources.ApplyResources(this.groupIndents, "groupIndents");
			this.groupIndents.Controls.Add(this.speTextOffset);
			this.groupIndents.Controls.Add(this.lblTextOffset);
			this.groupIndents.Controls.Add(this.chartPanelControl7);
			this.groupIndents.Controls.Add(this.chartPanelControl6);
			this.groupIndents.Controls.Add(this.chartPanelControl5);
			this.groupIndents.Controls.Add(this.chartPanelControl3);
			this.groupIndents.Name = "groupIndents";
			resources.ApplyResources(this.speTextOffset, "speTextOffset");
			this.speTextOffset.Name = "speTextOffset";
			this.speTextOffset.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.speTextOffset.Properties.IsFloatValue = false;
			this.speTextOffset.Properties.Mask.EditMask = resources.GetString("speTextOffset.Properties.Mask.EditMask");
			this.speTextOffset.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.speTextOffset.Properties.ValidateOnEnterKey = true;
			this.speTextOffset.EditValueChanged += new System.EventHandler(this.speTextOffset_EditValueChanged);
			resources.ApplyResources(this.lblTextOffset, "lblTextOffset");
			this.lblTextOffset.Name = "lblTextOffset";
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Controls.Add(this.speHIndent);
			this.chartPanelControl6.Controls.Add(this.lblHorizontal);
			this.chartPanelControl6.Name = "chartPanelControl6";
			resources.ApplyResources(this.speHIndent, "speHIndent");
			this.speHIndent.Name = "speHIndent";
			this.speHIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.speHIndent.Properties.IsFloatValue = false;
			this.speHIndent.Properties.Mask.EditMask = resources.GetString("speHIndent.Properties.Mask.EditMask");
			this.speHIndent.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.speHIndent.Properties.ValidateOnEnterKey = true;
			this.speHIndent.EditValueChanged += new System.EventHandler(this.speHIndent_EditValueChanged);
			resources.ApplyResources(this.lblHorizontal, "lblHorizontal");
			this.lblHorizontal.Name = "lblHorizontal";
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.speVIndent);
			this.chartPanelControl3.Controls.Add(this.lblVertical);
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.speVIndent, "speVIndent");
			this.speVIndent.Name = "speVIndent";
			this.speVIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.speVIndent.Properties.IsFloatValue = false;
			this.speVIndent.Properties.Mask.EditMask = resources.GetString("speVIndent.Properties.Mask.EditMask");
			this.speVIndent.Properties.MaxValue = new decimal(new int[] {
			999,
			0,
			0,
			0});
			this.speVIndent.Properties.ValidateOnEnterKey = true;
			this.speVIndent.EditValueChanged += new System.EventHandler(this.speVIndent_EditValueChanged);
			resources.ApplyResources(this.lblVertical, "lblVertical");
			this.lblVertical.Name = "lblVertical";
			this.panelControl10.BackColor = System.Drawing.Color.Transparent;
			this.panelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl10, "panelControl10");
			this.panelControl10.Name = "panelControl10";
			resources.ApplyResources(this.grPadding, "grPadding");
			this.grPadding.Controls.Add(this.paddingControl);
			this.grPadding.Name = "grPadding";
			resources.ApplyResources(this.paddingControl, "paddingControl");
			this.paddingControl.Name = "paddingControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grPadding);
			this.Controls.Add(this.panelControl10);
			this.Controls.Add(this.groupIndents);
			this.Controls.Add(this.pnlEditors);
			this.Name = "LegendInteriorControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupIndents)).EndInit();
			this.groupIndents.ResumeLayout(false);
			this.groupIndents.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.speTextOffset.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			this.chartPanelControl6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speHIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.speVIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grPadding)).EndInit();
			this.grPadding.ResumeLayout(false);
			this.grPadding.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlEditors;
		private DevExpress.XtraEditors.GroupControl groupIndents;
		private DevExpress.XtraEditors.SpinEdit speHIndent;
		private DevExpress.XtraEditors.SpinEdit speVIndent;
		private ChartPanelControl panelControl10;
		private ChartPanelControl chartPanelControl3;
		private ChartLabelControl lblVertical;
		private ChartPanelControl chartPanelControl6;
		private ChartLabelControl lblHorizontal;
		private ChartPanelControl chartPanelControl5;
		private DevExpress.XtraEditors.SpinEdit speTextOffset;
		private ChartLabelControl lblTextOffset;
		private ChartPanelControl chartPanelControl7;
		private DevExpress.XtraEditors.GroupControl grPadding;
		private DevExpress.XtraCharts.Wizard.RectangleIndentsControl paddingControl;
	}
}
