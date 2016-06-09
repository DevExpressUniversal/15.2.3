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
	partial class BubbleViewControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BubbleViewControl));
			this.pnlValue = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbValue = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblValue = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue)).BeginInit();
			this.pnlValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbValue.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlValue, "pnlValue");
			this.pnlValue.BackColor = System.Drawing.Color.Transparent;
			this.pnlValue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValue.Controls.Add(this.cbValue);
			this.pnlValue.Controls.Add(this.lblValue);
			this.pnlValue.Name = "pnlValue";
			resources.ApplyResources(this.cbValue, "cbValue");
			this.cbValue.Name = "cbValue";
			this.cbValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbValue.Properties.Buttons"))))});
			this.cbValue.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbValue.SelectedIndexChanged += new System.EventHandler(this.cbValue_SelectedIndexChanged);
			resources.ApplyResources(this.lblValue, "lblValue");
			this.lblValue.Name = "lblValue";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlValue);
			this.Name = "BubbleViewControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlValue)).EndInit();
			this.pnlValue.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlValue;
		private DevExpress.XtraEditors.ComboBoxEdit cbValue;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblValue;
	}
}
