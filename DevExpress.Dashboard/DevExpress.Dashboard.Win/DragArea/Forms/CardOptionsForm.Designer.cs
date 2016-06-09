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
	partial class CardOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardOptionsForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.deltaOptionsControl = new DevExpress.DashboardWin.Native.DeltaOptionsControl();
			this.panelOkCancel = new DevExpress.XtraEditors.PanelControl();
			this.sparklineOptionsControl = new DevExpress.DashboardWin.Native.SparklineOptionsControl();
			this.deltaOptionsContainer = new DevExpress.XtraEditors.GroupControl();
			this.sparklineOptionsContainer = new DevExpress.XtraEditors.GroupControl();
			this.ceShowSparkline = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).BeginInit();
			this.panelOkCancel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.deltaOptionsContainer)).BeginInit();
			this.deltaOptionsContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sparklineOptionsContainer)).BeginInit();
			this.sparklineOptionsContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceShowSparkline.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.ButtonOKClick);
			resources.ApplyResources(this.deltaOptionsControl, "deltaOptionsControl");
			this.deltaOptionsControl.Name = "deltaOptionsControl";
			resources.ApplyResources(this.panelOkCancel, "panelOkCancel");
			this.panelOkCancel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelOkCancel.Controls.Add(this.btnOK);
			this.panelOkCancel.Controls.Add(this.btnCancel);
			this.panelOkCancel.Name = "panelOkCancel";
			resources.ApplyResources(this.sparklineOptionsControl, "sparklineOptionsControl");
			this.sparklineOptionsControl.Name = "sparklineOptionsControl";
			this.deltaOptionsContainer.Controls.Add(this.deltaOptionsControl);
			resources.ApplyResources(this.deltaOptionsContainer, "deltaOptionsContainer");
			this.deltaOptionsContainer.Name = "deltaOptionsContainer";
			this.sparklineOptionsContainer.Controls.Add(this.ceShowSparkline);
			this.sparklineOptionsContainer.Controls.Add(this.sparklineOptionsControl);
			resources.ApplyResources(this.sparklineOptionsContainer, "sparklineOptionsContainer");
			this.sparklineOptionsContainer.Name = "sparklineOptionsContainer";
			resources.ApplyResources(this.ceShowSparkline, "ceEnableSparkline");
			this.ceShowSparkline.Name = "ceEnableSparkline";
			this.ceShowSparkline.Properties.Caption = resources.GetString("ceEnableSparkline.Properties.Caption");
			this.ceShowSparkline.CheckedChanged += new System.EventHandler(this.ShowSparklineCheckedChanged);
			this.AcceptButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.sparklineOptionsContainer);
			this.Controls.Add(this.deltaOptionsContainer);
			this.Controls.Add(this.panelOkCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CardOptionsForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).EndInit();
			this.panelOkCancel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.deltaOptionsContainer)).EndInit();
			this.deltaOptionsContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sparklineOptionsContainer)).EndInit();
			this.sparklineOptionsContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceShowSparkline.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DeltaOptionsControl deltaOptionsControl;
		private DevExpress.XtraEditors.PanelControl panelOkCancel;
		private SparklineOptionsControl sparklineOptionsControl;
		private XtraEditors.GroupControl deltaOptionsContainer;
		private XtraEditors.GroupControl sparklineOptionsContainer;
		private XtraEditors.CheckEdit ceShowSparkline;
	}
}
