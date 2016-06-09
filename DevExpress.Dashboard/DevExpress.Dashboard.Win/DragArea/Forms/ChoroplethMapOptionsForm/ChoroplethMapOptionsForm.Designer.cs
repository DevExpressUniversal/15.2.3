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

namespace DevExpress.DashboardWin.Native{
	partial class ChoroplethMapOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoroplethMapOptionsForm));
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.columnTypeLabel = new DevExpress.XtraEditors.LabelControl();
			this.valueMapCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.deltaCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.columnTypePanel = new DevExpress.XtraEditors.PanelControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.buttonsPanel = new DevExpress.XtraEditors.PanelControl();
			this.deltaMapControl = new DevExpress.DashboardWin.Native.DeltaMapOptionsControl();
			this.valueMapControl = new DevExpress.DashboardWin.Native.ValueMapOptionsControl();
			((System.ComponentModel.ISupportInitialize)(this.valueMapCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.deltaCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.columnTypePanel)).BeginInit();
			this.columnTypePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).BeginInit();
			this.buttonsPanel.SuspendLayout();
			this.SuspendLayout();
			this.separator.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.separator.LineVisible = true;
			resources.ApplyResources(this.separator, "separator");
			this.separator.Name = "separator";
			resources.ApplyResources(this.columnTypeLabel, "columnTypeLabel");
			this.columnTypeLabel.Name = "columnTypeLabel";
			resources.ApplyResources(this.valueMapCheckEdit, "valueMapCheckEdit");
			this.valueMapCheckEdit.Name = "valueMapCheckEdit";
			this.valueMapCheckEdit.Properties.Caption = resources.GetString("valueMapCheckEdit.Properties.Caption");
			this.valueMapCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.valueMapCheckEdit.Properties.RadioGroupIndex = 1;
			this.valueMapCheckEdit.CheckedChanged += new System.EventHandler(this.colorizerCheckEdit_CheckedChanged);
			resources.ApplyResources(this.deltaCheckEdit, "deltaCheckEdit");
			this.deltaCheckEdit.Name = "deltaCheckEdit";
			this.deltaCheckEdit.Properties.Caption = resources.GetString("deltaCheckEdit.Properties.Caption");
			this.deltaCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.deltaCheckEdit.Properties.RadioGroupIndex = 1;
			this.deltaCheckEdit.TabStop = false;
			this.deltaCheckEdit.CheckedChanged += new System.EventHandler(this.deltaCheckEdit_CheckedChanged);
			this.columnTypePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.columnTypePanel.Controls.Add(this.columnTypeLabel);
			this.columnTypePanel.Controls.Add(this.valueMapCheckEdit);
			this.columnTypePanel.Controls.Add(this.deltaCheckEdit);
			resources.ApplyResources(this.columnTypePanel, "columnTypePanel");
			this.columnTypePanel.Name = "columnTypePanel";
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.buttonsPanel, "buttonsPanel");
			this.buttonsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.buttonsPanel.Controls.Add(this.btnApply);
			this.buttonsPanel.Controls.Add(this.btnCancel);
			this.buttonsPanel.Controls.Add(this.btnOK);
			this.buttonsPanel.Name = "buttonsPanel";
			resources.ApplyResources(this.deltaMapControl, "deltaMapControl");
			this.deltaMapControl.MapOptionsForm = null;
			this.deltaMapControl.Name = "deltaMapControl";
			resources.ApplyResources(this.valueMapControl, "valueMapControl");
			this.valueMapControl.Name = "valueMapControl";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.valueMapControl);
			this.Controls.Add(this.deltaMapControl);
			this.Controls.Add(this.separator);
			this.Controls.Add(this.columnTypePanel);
			this.Controls.Add(this.buttonsPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChoroplethMapOptionsForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.valueMapCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.deltaCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.columnTypePanel)).EndInit();
			this.columnTypePanel.ResumeLayout(false);
			this.columnTypePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl separator;
		private XtraEditors.LabelControl columnTypeLabel;
		private XtraEditors.CheckEdit valueMapCheckEdit;
		private XtraEditors.CheckEdit deltaCheckEdit;
		private XtraEditors.PanelControl columnTypePanel;
		private XtraEditors.SimpleButton btnApply;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.PanelControl buttonsPanel;
		private DeltaMapOptionsControl deltaMapControl;
		private ValueMapOptionsControl valueMapControl;
	}
}
