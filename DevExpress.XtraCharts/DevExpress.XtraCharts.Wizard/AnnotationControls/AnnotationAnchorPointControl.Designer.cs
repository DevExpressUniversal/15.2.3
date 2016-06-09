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

namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	partial class AnnotationAnchorPointControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationAnchorPointControl));
			this.pnlAnchorKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbAnchorKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAnchorKind = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepAnchorKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlAnchorOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlAnchorKind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAnchorKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAnchorKind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAnchorOptions)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlAnchorKind, "pnlAnchorKind");
			this.pnlAnchorKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlAnchorKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAnchorKind.Controls.Add(this.cbAnchorKind);
			this.pnlAnchorKind.Controls.Add(this.lblAnchorKind);
			this.pnlAnchorKind.Name = "pnlAnchorKind";
			resources.ApplyResources(this.cbAnchorKind, "cbAnchorKind");
			this.cbAnchorKind.Name = "cbAnchorKind";
			this.cbAnchorKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbAnchorKind.Properties.Buttons"))))});
			this.cbAnchorKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbAnchorKind.SelectedIndexChanged += new System.EventHandler(this.cbAnchorKind_SelectedIndexChanged);
			resources.ApplyResources(this.lblAnchorKind, "lblAnchorKind");
			this.lblAnchorKind.Name = "lblAnchorKind";
			this.sepAnchorKind.BackColor = System.Drawing.Color.Transparent;
			this.sepAnchorKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepAnchorKind, "sepAnchorKind");
			this.sepAnchorKind.Name = "sepAnchorKind";
			resources.ApplyResources(this.pnlAnchorOptions, "pnlAnchorOptions");
			this.pnlAnchorOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnlAnchorOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAnchorOptions.Name = "pnlAnchorOptions";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlAnchorOptions);
			this.Controls.Add(this.sepAnchorKind);
			this.Controls.Add(this.pnlAnchorKind);
			this.Name = "AnnotationAnchorPointControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlAnchorKind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAnchorKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepAnchorKind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAnchorOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlAnchorKind;
		private DevExpress.XtraEditors.ComboBoxEdit cbAnchorKind;
		private ChartLabelControl lblAnchorKind;
		private ChartPanelControl sepAnchorKind;
		private ChartPanelControl pnlAnchorOptions;
	}
}
