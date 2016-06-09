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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls
{
	partial class AxisLabelLayoutControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisLabelLayoutControl));
			this.ceStaggered = new DevExpress.XtraEditors.CheckEdit();
			this.panelAnglePadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtAngle = new DevExpress.XtraEditors.SpinEdit();
			this.labelAngle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelPosition = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelDirection = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelDirection = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.ceStaggered.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAnglePadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAngle)).BeginInit();
			this.panelAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPosition)).BeginInit();
			this.panelPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDirection)).BeginInit();
			this.panelDirection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.ceStaggered, "ceStaggered");
			this.ceStaggered.Name = "ceStaggered";
			this.ceStaggered.Properties.Caption = resources.GetString("ceStaggered.Properties.Caption");
			this.ceStaggered.CheckedChanged += new System.EventHandler(this.ceStaggered_CheckedChanged);
			this.panelAnglePadding.BackColor = System.Drawing.Color.Transparent;
			this.panelAnglePadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelAnglePadding, "panelAnglePadding");
			this.panelAnglePadding.Name = "panelAnglePadding";
			resources.ApplyResources(this.panelAngle, "panelAngle");
			this.panelAngle.BackColor = System.Drawing.Color.Transparent;
			this.panelAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelAngle.Controls.Add(this.txtAngle);
			this.panelAngle.Controls.Add(this.labelAngle);
			this.panelAngle.Name = "panelAngle";
			resources.ApplyResources(this.txtAngle, "txtAngle");
			this.txtAngle.Name = "txtAngle";
			this.txtAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtAngle.Properties.IsFloatValue = false;
			this.txtAngle.Properties.Mask.EditMask = resources.GetString("txtAngle.Properties.Mask.EditMask");
			this.txtAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.txtAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.txtAngle.EditValueChanged += new System.EventHandler(this.txtAngle_EditValueChanged);
			resources.ApplyResources(this.labelAngle, "labelAngle");
			this.labelAngle.Name = "labelAngle";
			resources.ApplyResources(this.panelPosition, "panelPosition");
			this.panelPosition.BackColor = System.Drawing.Color.Transparent;
			this.panelPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelPosition.Controls.Add(this.cbPosition);
			this.panelPosition.Controls.Add(this.labelPosition);
			this.panelPosition.Name = "panelPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPosition.Properties.Items"),
			resources.GetString("cbPosition.Properties.Items1"),
			resources.GetString("cbPosition.Properties.Items2"),
			resources.GetString("cbPosition.Properties.Items3"),
			resources.GetString("cbPosition.Properties.Items4")});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.labelPosition, "labelPosition");
			this.labelPosition.Name = "labelPosition";
			resources.ApplyResources(this.panelDirection, "panelDirection");
			this.panelDirection.BackColor = System.Drawing.Color.Transparent;
			this.panelDirection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelDirection.Controls.Add(this.cbDirection);
			this.panelDirection.Controls.Add(this.labelDirection);
			this.panelDirection.Name = "panelDirection";
			resources.ApplyResources(this.cbDirection, "cbDirection");
			this.cbDirection.Name = "cbDirection";
			this.cbDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDirection.Properties.Buttons"))))});
			this.cbDirection.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDirection.Properties.Items"),
			resources.GetString("cbDirection.Properties.Items1"),
			resources.GetString("cbDirection.Properties.Items2"),
			resources.GetString("cbDirection.Properties.Items3"),
			resources.GetString("cbDirection.Properties.Items4")});
			this.cbDirection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDirection.SelectedIndexChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
			resources.ApplyResources(this.labelDirection, "labelDirection");
			this.labelDirection.Name = "labelDirection";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelDirection);
			this.Controls.Add(this.panelPosition);
			this.Controls.Add(this.panelAngle);
			this.Controls.Add(this.panelAnglePadding);
			this.Controls.Add(this.ceStaggered);
			this.Name = "AxisLabelLayoutControl";
			((System.ComponentModel.ISupportInitialize)(this.ceStaggered.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAnglePadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAngle)).EndInit();
			this.panelAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelPosition)).EndInit();
			this.panelPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDirection)).EndInit();
			this.panelDirection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDirection.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit ceStaggered;
		private ChartPanelControl panelAnglePadding;
		protected ChartPanelControl panelAngle;
		private ChartLabelControl labelAngle;
		private DevExpress.XtraEditors.SpinEdit txtAngle;
		private ChartPanelControl panelPosition;
		private DevExpress.XtraEditors.ComboBoxEdit cbPosition;
		private ChartLabelControl labelPosition;
		private ChartPanelControl panelDirection;
		private DevExpress.XtraEditors.ComboBoxEdit cbDirection;
		private ChartLabelControl labelDirection;
	}
}
