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

namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	partial class FunnelSeriesLabelOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunnelSeriesLabelOptionsControl));
			this.pnlPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPosition = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).BeginInit();
			this.pnlPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlPosition, "pnlPosition");
			this.pnlPosition.BackColor = System.Drawing.Color.Transparent;
			this.pnlPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPosition.Controls.Add(this.cbPosition);
			this.pnlPosition.Controls.Add(this.lblPosition);
			this.pnlPosition.Name = "pnlPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.lblPosition, "lblPosition");
			this.lblPosition.Name = "lblPosition";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlPosition);
			this.Name = "FunnelSeriesLabelOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).EndInit();
			this.pnlPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlPosition;
		private ChartLabelControl lblPosition;
		private DevExpress.XtraEditors.ComboBoxEdit cbPosition;
	}
}
