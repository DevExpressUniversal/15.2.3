#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class GridOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridOptionsForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.columnTypeLabel = new DevExpress.XtraEditors.LabelControl();
			this.dimensionCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.measureCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.deltaCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.columnTypePanel = new DevExpress.XtraEditors.PanelControl();
			this.autoCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.sparklineCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.buttonsPanel = new DevExpress.XtraEditors.PanelControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.dimensionColumnControl = new DevExpress.DashboardWin.Native.GridDimensionColumnControl();
			this.measureColumnControl = new DevExpress.DashboardWin.Native.GridMeasureColumnControl();
			this.deltaColumnControl = new DevExpress.DashboardWin.Native.GridDeltaColumnControl();
			this.sparklineColumnControl = new DevExpress.DashboardWin.Native.GridSparklineColumnControl();
			((System.ComponentModel.ISupportInitialize)(this.dimensionCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.measureCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.deltaCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.columnTypePanel)).BeginInit();
			this.columnTypePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.autoCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sparklineCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).BeginInit();
			this.buttonsPanel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			resources.ApplyResources(this.separator, "separator");
			this.separator.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.separator.LineVisible = true;
			this.separator.Name = "separator";
			resources.ApplyResources(this.columnTypeLabel, "columnTypeLabel");
			this.columnTypeLabel.Name = "columnTypeLabel";
			resources.ApplyResources(this.dimensionCheckEdit, "dimensionCheckEdit");
			this.dimensionCheckEdit.Name = "dimensionCheckEdit";
			this.dimensionCheckEdit.Properties.Caption = resources.GetString("dimensionCheckEdit.Properties.Caption");
			this.dimensionCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.dimensionCheckEdit.Properties.RadioGroupIndex = 1;
			this.dimensionCheckEdit.TabStop = false;
			resources.ApplyResources(this.measureCheckEdit, "measureCheckEdit");
			this.measureCheckEdit.Name = "measureCheckEdit";
			this.measureCheckEdit.Properties.Caption = resources.GetString("measureCheckEdit.Properties.Caption");
			this.measureCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.measureCheckEdit.Properties.RadioGroupIndex = 1;
			this.measureCheckEdit.TabStop = false;
			resources.ApplyResources(this.deltaCheckEdit, "deltaCheckEdit");
			this.deltaCheckEdit.Name = "deltaCheckEdit";
			this.deltaCheckEdit.Properties.Caption = resources.GetString("deltaCheckEdit.Properties.Caption");
			this.deltaCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.deltaCheckEdit.Properties.RadioGroupIndex = 1;
			this.deltaCheckEdit.TabStop = false;
			this.columnTypePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.columnTypePanel.Controls.Add(this.autoCheckEdit);
			this.columnTypePanel.Controls.Add(this.sparklineCheckEdit);
			this.columnTypePanel.Controls.Add(this.columnTypeLabel);
			this.columnTypePanel.Controls.Add(this.dimensionCheckEdit);
			this.columnTypePanel.Controls.Add(this.measureCheckEdit);
			this.columnTypePanel.Controls.Add(this.deltaCheckEdit);
			resources.ApplyResources(this.columnTypePanel, "columnTypePanel");
			this.columnTypePanel.Name = "columnTypePanel";
			resources.ApplyResources(this.autoCheckEdit, "autoCheckEdit");
			this.autoCheckEdit.Name = "autoCheckEdit";
			this.autoCheckEdit.Properties.Caption = resources.GetString("autoCheckEdit.Properties.Caption");
			this.autoCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.autoCheckEdit.Properties.RadioGroupIndex = 1;
			this.autoCheckEdit.TabStop = false;
			resources.ApplyResources(this.sparklineCheckEdit, "sparklineCheckEdit");
			this.sparklineCheckEdit.Name = "sparklineCheckEdit";
			this.sparklineCheckEdit.Properties.Caption = resources.GetString("sparklineCheckEdit.Properties.Caption");
			this.sparklineCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.sparklineCheckEdit.Properties.RadioGroupIndex = 1;
			this.sparklineCheckEdit.TabStop = false;
			this.buttonsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.buttonsPanel.Controls.Add(this.btnApply);
			this.buttonsPanel.Controls.Add(this.btnCancel);
			this.buttonsPanel.Controls.Add(this.btnOK);
			resources.ApplyResources(this.buttonsPanel, "buttonsPanel");
			this.buttonsPanel.Name = "buttonsPanel";
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			resources.ApplyResources(this.dimensionColumnControl, "dimensionColumnControl");
			this.dimensionColumnControl.Name = "dimensionColumnControl";
			resources.ApplyResources(this.measureColumnControl, "measureColumnControl");
			this.measureColumnControl.Name = "measureColumnControl";
			resources.ApplyResources(this.deltaColumnControl, "deltaColumnControl");
			this.deltaColumnControl.Name = "deltaColumnControl";
			resources.ApplyResources(this.sparklineColumnControl, "sparklineColumnControl");
			this.sparklineColumnControl.Name = "sparklineColumnControl";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.separator);
			this.Controls.Add(this.columnTypePanel);
			this.Controls.Add(this.buttonsPanel);
			this.Controls.Add(this.sparklineColumnControl);
			this.Controls.Add(this.deltaColumnControl);
			this.Controls.Add(this.measureColumnControl);
			this.Controls.Add(this.dimensionColumnControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GridOptionsForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.dimensionCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.measureCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.deltaCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.columnTypePanel)).EndInit();
			this.columnTypePanel.ResumeLayout(false);
			this.columnTypePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.autoCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sparklineCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.LabelControl separator;
		private DevExpress.XtraEditors.LabelControl columnTypeLabel;
		private GridDimensionColumnControl dimensionColumnControl;
		private DevExpress.XtraEditors.CheckEdit dimensionCheckEdit;
		private DevExpress.XtraEditors.CheckEdit measureCheckEdit;
		private DevExpress.XtraEditors.CheckEdit deltaCheckEdit;
		private DevExpress.XtraEditors.PanelControl columnTypePanel;
		private DevExpress.XtraEditors.PanelControl buttonsPanel;
		private XtraEditors.SimpleButton btnApply;
		private XtraEditors.CheckEdit sparklineCheckEdit;
		private GridMeasureColumnControl measureColumnControl;
		private GridDeltaColumnControl deltaColumnControl;
		private GridSparklineColumnControl sparklineColumnControl;
		private XtraEditors.CheckEdit autoCheckEdit;
	}
}
