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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	partial class ScaleBreaksAutoControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleBreaksAutoControl));
			this.chEnabled = new DevExpress.XtraEditors.CheckEdit();
			this.sepEnabled = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlMaxCount = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnMaxCount = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxCount = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.chEnabled.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepEnabled)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxCount)).BeginInit();
			this.pnlMaxCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnMaxCount.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chEnabled, "chEnabled");
			this.chEnabled.Name = "chEnabled";
			this.chEnabled.Properties.Caption = resources.GetString("chEnabled.Properties.Caption");
			this.chEnabled.CheckedChanged += new System.EventHandler(this.chEnabled_CheckedChanged);
			this.sepEnabled.BackColor = System.Drawing.Color.Transparent;
			this.sepEnabled.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepEnabled, "sepEnabled");
			this.sepEnabled.Name = "sepEnabled";
			resources.ApplyResources(this.pnlMaxCount, "pnlMaxCount");
			this.pnlMaxCount.BackColor = System.Drawing.Color.Transparent;
			this.pnlMaxCount.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxCount.Controls.Add(this.spnMaxCount);
			this.pnlMaxCount.Controls.Add(this.lblMaxCount);
			this.pnlMaxCount.Name = "pnlMaxCount";
			resources.ApplyResources(this.spnMaxCount, "spnMaxCount");
			this.spnMaxCount.Name = "spnMaxCount";
			this.spnMaxCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnMaxCount.Properties.IsFloatValue = false;
			this.spnMaxCount.Properties.Mask.EditMask = resources.GetString("spnMaxCount.Properties.Mask.EditMask");
			this.spnMaxCount.Properties.MaxValue = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.spnMaxCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spnMaxCount.Properties.ValidateOnEnterKey = true;
			this.spnMaxCount.EditValueChanged += new System.EventHandler(this.spnMaxCount_EditValueChanged);
			resources.ApplyResources(this.lblMaxCount, "lblMaxCount");
			this.lblMaxCount.Name = "lblMaxCount";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlMaxCount);
			this.Controls.Add(this.sepEnabled);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.chEnabled);
			this.Name = "ScaleBreaksAutoControl";
			((System.ComponentModel.ISupportInitialize)(this.chEnabled.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepEnabled)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxCount)).EndInit();
			this.pnlMaxCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnMaxCount.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chEnabled;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepEnabled;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlMaxCount;
		private DevExpress.XtraEditors.LabelControl lblMaxCount;
		private DevExpress.XtraEditors.SpinEdit spnMaxCount;
		private DevExpress.XtraEditors.LabelControl labelControl2;
	}
}
