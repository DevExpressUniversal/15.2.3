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
	partial class GaugeOptionsForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GaugeOptionsForm));
			this.panelOkCancel = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.panelMinimum = new DevExpress.XtraEditors.PanelControl();
			this.panelAutomaticMinimum = new System.Windows.Forms.Panel();
			this.ceAutomaticMinimum = new DevExpress.XtraEditors.CheckEdit();
			this.labelMinimum = new DevExpress.XtraEditors.LabelControl();
			this.seMinimum = new DevExpress.XtraEditors.SpinEdit();
			this.panelMaximum = new DevExpress.XtraEditors.PanelControl();
			this.panelAutomaticMaximum = new System.Windows.Forms.Panel();
			this.ceAutomaticMaximum = new DevExpress.XtraEditors.CheckEdit();
			this.labelMaximum = new DevExpress.XtraEditors.LabelControl();
			this.seMaximum = new DevExpress.XtraEditors.SpinEdit();
			this.deltaOptionsControl = new DevExpress.DashboardWin.Native.DeltaOptionsControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).BeginInit();
			this.panelOkCancel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelMinimum)).BeginInit();
			this.panelMinimum.SuspendLayout();
			this.panelAutomaticMinimum.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticMinimum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seMinimum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelMaximum)).BeginInit();
			this.panelMaximum.SuspendLayout();
			this.panelAutomaticMaximum.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticMaximum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seMaximum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			this.SuspendLayout();
			this.panelOkCancel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelOkCancel.Controls.Add(this.btnCancel);
			this.panelOkCancel.Controls.Add(this.btnOK);
			resources.ApplyResources(this.panelOkCancel, "panelOkCancel");
			this.panelOkCancel.Name = "panelOkCancel";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.OnOkClick);
			this.panelMinimum.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelMinimum.Controls.Add(this.panelAutomaticMinimum);
			this.panelMinimum.Controls.Add(this.labelMinimum);
			this.panelMinimum.Controls.Add(this.seMinimum);
			resources.ApplyResources(this.panelMinimum, "panelMinimum");
			this.panelMinimum.Name = "panelMinimum";
			this.panelAutomaticMinimum.Controls.Add(this.ceAutomaticMinimum);
			resources.ApplyResources(this.panelAutomaticMinimum, "panelAutomaticMinimum");
			this.panelAutomaticMinimum.Name = "panelAutomaticMinimum";
			resources.ApplyResources(this.ceAutomaticMinimum, "ceAutomaticMinimum");
			this.ceAutomaticMinimum.Name = "ceAutomaticMinimum";
			this.ceAutomaticMinimum.Properties.Caption = resources.GetString("ceAutomaticMinimum.Properties.Caption");
			this.ceAutomaticMinimum.CheckedChanged += new System.EventHandler(this.OnAutomaticMinimumCheckedChanged);
			resources.ApplyResources(this.labelMinimum, "labelMinimum");
			this.labelMinimum.Name = "labelMinimum";
			resources.ApplyResources(this.seMinimum, "seMinimum");
			this.seMinimum.Name = "seMinimum";
			this.seMinimum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.panelMaximum.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelMaximum.Controls.Add(this.panelAutomaticMaximum);
			this.panelMaximum.Controls.Add(this.labelMaximum);
			this.panelMaximum.Controls.Add(this.seMaximum);
			resources.ApplyResources(this.panelMaximum, "panelMaximum");
			this.panelMaximum.Name = "panelMaximum";
			this.panelAutomaticMaximum.Controls.Add(this.ceAutomaticMaximum);
			resources.ApplyResources(this.panelAutomaticMaximum, "panelAutomaticMaximum");
			this.panelAutomaticMaximum.Name = "panelAutomaticMaximum";
			resources.ApplyResources(this.ceAutomaticMaximum, "ceAutomaticMaximum");
			this.ceAutomaticMaximum.Name = "ceAutomaticMaximum";
			this.ceAutomaticMaximum.Properties.Caption = resources.GetString("ceAutomaticMaximum.Properties.Caption");
			this.ceAutomaticMaximum.CheckedChanged += new System.EventHandler(this.OnAutomaticMaximumCheckedChanged);
			resources.ApplyResources(this.labelMaximum, "labelMaximum");
			this.labelMaximum.Name = "labelMaximum";
			resources.ApplyResources(this.seMaximum, "seMaximum");
			this.seMaximum.Name = "seMaximum";
			this.seMaximum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.deltaOptionsControl, "deltaOptionsControl");
			this.deltaOptionsControl.Name = "deltaOptionsControl";
			this.groupControl1.CaptionImageUri.Uri = "";
			this.groupControl1.Controls.Add(this.panelMinimum);
			this.groupControl1.Controls.Add(this.panelMaximum);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.groupControl2.CaptionImageUri.Uri = "";
			this.groupControl2.Controls.Add(this.deltaOptionsControl);
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Name = "groupControl2";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.groupControl2);
			this.Controls.Add(this.groupControl1);
			this.Controls.Add(this.panelOkCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GaugeOptionsForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).EndInit();
			this.panelOkCancel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelMinimum)).EndInit();
			this.panelMinimum.ResumeLayout(false);
			this.panelMinimum.PerformLayout();
			this.panelAutomaticMinimum.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticMinimum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seMinimum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelMaximum)).EndInit();
			this.panelMaximum.ResumeLayout(false);
			this.panelMaximum.PerformLayout();
			this.panelAutomaticMaximum.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticMaximum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seMaximum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl panelOkCancel;
		private DevExpress.XtraEditors.PanelControl panelMinimum;
		private DevExpress.XtraEditors.PanelControl panelMaximum;
		private DevExpress.XtraEditors.LabelControl labelMinimum;
		private DevExpress.XtraEditors.SpinEdit seMinimum;
		private DevExpress.XtraEditors.CheckEdit ceAutomaticMinimum;
		private DevExpress.XtraEditors.CheckEdit ceAutomaticMaximum;
		private DevExpress.XtraEditors.LabelControl labelMaximum;
		private DevExpress.XtraEditors.SpinEdit seMaximum;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DeltaOptionsControl deltaOptionsControl;
		private XtraEditors.GroupControl groupControl1;
		private XtraEditors.GroupControl groupControl2;
		private System.Windows.Forms.Panel panelAutomaticMinimum;
		private System.Windows.Forms.Panel panelAutomaticMaximum;
	}
}
