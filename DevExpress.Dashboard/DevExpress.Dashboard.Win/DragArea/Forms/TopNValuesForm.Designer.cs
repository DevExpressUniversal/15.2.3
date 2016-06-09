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
	partial class TopNValuesForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopNValuesForm));
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.cbMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.ceShowOthersValue = new DevExpress.XtraEditors.CheckEdit();
			this.ceEnabledTopN = new DevExpress.XtraEditors.CheckEdit();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.cbMeasure = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblMeasure = new DevExpress.XtraEditors.LabelControl();
			this.panelOkCancelApply = new DevExpress.XtraEditors.PanelControl();
			this.panelTopN = new DevExpress.XtraEditors.PanelControl();
			this.seTopValuesCount = new DevExpress.XtraEditors.SpinEdit();
			this.lblCount = new DevExpress.XtraEditors.LabelControl();
			this.lblMode = new DevExpress.XtraEditors.LabelControl();
			this.lblShowOthers = new DevExpress.XtraEditors.LabelControl();
			this.panelShowOthersValue = new DevExpress.XtraEditors.PanelControl();
			this.panelEnabled = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowOthersValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEnabledTopN.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMeasure.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancelApply)).BeginInit();
			this.panelOkCancelApply.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelTopN)).BeginInit();
			this.panelTopN.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seTopValuesCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelShowOthersValue)).BeginInit();
			this.panelShowOthersValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelEnabled)).BeginInit();
			this.panelEnabled.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.separator, "separator");
			this.separator.LineVisible = true;
			this.separator.Name = "separator";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			resources.ApplyResources(this.cbMode, "cbMode");
			this.cbMode.Name = "cbMode";
			this.cbMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMode.Properties.Buttons"))))});
			this.cbMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbMode.Properties.Items"),
			resources.GetString("cbMode.Properties.Items1")});
			this.cbMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.ceShowOthersValue, "ceShowOthersValue");
			this.ceShowOthersValue.Name = "ceShowOthersValue";
			this.ceShowOthersValue.Properties.Caption = resources.GetString("ceShowOthersValue.Properties.Caption");
			this.ceShowOthersValue.Properties.FullFocusRect = true;
			resources.ApplyResources(this.ceEnabledTopN, "ceEnabledTopN");
			this.ceEnabledTopN.Name = "ceEnabledTopN";
			this.ceEnabledTopN.Properties.Caption = resources.GetString("ceEnabledTopN.Properties.Caption");
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			resources.ApplyResources(this.cbMeasure, "cbMeasure");
			this.cbMeasure.Name = "cbMeasure";
			this.cbMeasure.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMeasure.Properties.Buttons"))))});
			this.cbMeasure.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblMeasure, "lblMeasure");
			this.lblMeasure.Name = "lblMeasure";
			resources.ApplyResources(this.panelOkCancelApply, "panelOkCancelApply");
			this.panelOkCancelApply.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelOkCancelApply.Controls.Add(this.btnApply);
			this.panelOkCancelApply.Controls.Add(this.btnOK);
			this.panelOkCancelApply.Controls.Add(this.btnCancel);
			this.panelOkCancelApply.Name = "panelOkCancelApply";
			resources.ApplyResources(this.panelTopN, "panelTopN");
			this.panelTopN.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelTopN.Controls.Add(this.cbMeasure);
			this.panelTopN.Controls.Add(this.lblMeasure);
			this.panelTopN.Controls.Add(this.seTopValuesCount);
			this.panelTopN.Controls.Add(this.lblCount);
			this.panelTopN.Controls.Add(this.cbMode);
			this.panelTopN.Controls.Add(this.lblMode);
			this.panelTopN.Name = "panelTopN";
			resources.ApplyResources(this.seTopValuesCount, "seTopValuesCount");
			this.seTopValuesCount.Name = "seTopValuesCount";
			this.seTopValuesCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seTopValuesCount.Properties.IsFloatValue = false;
			this.seTopValuesCount.Properties.Mask.EditMask = resources.GetString("seTopValuesCount.Properties.Mask.EditMask");
			this.seTopValuesCount.Properties.MaxValue = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.seTopValuesCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.seTopValuesCount.Properties.NullText = resources.GetString("seTopValuesCount.Properties.NullText");
			resources.ApplyResources(this.lblCount, "lblCount");
			this.lblCount.Name = "lblCount";
			resources.ApplyResources(this.lblMode, "lblMode");
			this.lblMode.Name = "lblMode";
			resources.ApplyResources(this.lblShowOthers, "lblShowOthers");
			this.lblShowOthers.Name = "lblShowOthers";
			resources.ApplyResources(this.panelShowOthersValue, "panelShowOthersValue");
			this.panelShowOthersValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelShowOthersValue.Controls.Add(this.lblShowOthers);
			this.panelShowOthersValue.Controls.Add(this.ceShowOthersValue);
			this.panelShowOthersValue.Name = "panelShowOthersValue";
			resources.ApplyResources(this.panelEnabled, "panelEnabled");
			this.panelEnabled.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelEnabled.Controls.Add(this.ceEnabledTopN);
			this.panelEnabled.Name = "panelEnabled";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.panelEnabled);
			this.Controls.Add(this.panelShowOthersValue);
			this.Controls.Add(this.panelTopN);
			this.Controls.Add(this.separator);
			this.Controls.Add(this.panelOkCancelApply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TopNValuesForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.cbMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowOthersValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEnabledTopN.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMeasure.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancelApply)).EndInit();
			this.panelOkCancelApply.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelTopN)).EndInit();
			this.panelTopN.ResumeLayout(false);
			this.panelTopN.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.seTopValuesCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelShowOthersValue)).EndInit();
			this.panelShowOthersValue.ResumeLayout(false);
			this.panelShowOthersValue.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelEnabled)).EndInit();
			this.panelEnabled.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit ceShowOthersValue;
		private DevExpress.XtraEditors.LabelControl separator;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.CheckEdit ceEnabledTopN;
		private DevExpress.XtraEditors.SimpleButton btnApply;
		private DevExpress.XtraEditors.LabelControl lblMeasure;
		private DevExpress.XtraEditors.ComboBoxEdit cbMode;
		private DevExpress.XtraEditors.ComboBoxEdit cbMeasure;
		private DevExpress.XtraEditors.PanelControl panelOkCancelApply;
		private DevExpress.XtraEditors.PanelControl panelTopN;
		private DevExpress.XtraEditors.LabelControl lblMode;
		private DevExpress.XtraEditors.SpinEdit seTopValuesCount;
		private DevExpress.XtraEditors.LabelControl lblCount;
		private DevExpress.XtraEditors.LabelControl lblShowOthers;
		private DevExpress.XtraEditors.PanelControl panelShowOthersValue;
		private DevExpress.XtraEditors.PanelControl panelEnabled;
	}
}
