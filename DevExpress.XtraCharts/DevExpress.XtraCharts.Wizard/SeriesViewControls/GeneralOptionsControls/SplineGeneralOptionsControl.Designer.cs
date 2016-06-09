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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class SplineGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplineGeneralOptionsControl));
			this.pnlLineTension = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnLineTension = new DevExpress.XtraEditors.SpinEdit();
			this.lblLineTension = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.grpSplineOptions = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlLineTension)).BeginInit();
			this.pnlLineTension.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnLineTension.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpSplineOptions)).BeginInit();
			this.grpSplineOptions.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlLineTension, "pnlLineTension");
			this.pnlLineTension.BackColor = System.Drawing.Color.Transparent;
			this.pnlLineTension.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlLineTension.Controls.Add(this.spnLineTension);
			this.pnlLineTension.Controls.Add(this.lblLineTension);
			this.pnlLineTension.Name = "pnlLineTension";
			resources.ApplyResources(this.spnLineTension, "spnLineTension");
			this.spnLineTension.Name = "spnLineTension";
			this.spnLineTension.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnLineTension.Properties.IsFloatValue = false;
			this.spnLineTension.Properties.Mask.EditMask = resources.GetString("spnLineTension.Properties.Mask.EditMask");
			this.spnLineTension.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spnLineTension.EditValueChanged += new System.EventHandler(this.spnLineTension_EditValueChanged);
			resources.ApplyResources(this.lblLineTension, "lblLineTension");
			this.lblLineTension.Name = "lblLineTension";
			resources.ApplyResources(this.grpSplineOptions, "grpSplineOptions");
			this.grpSplineOptions.Controls.Add(this.pnlLineTension);
			this.grpSplineOptions.Name = "grpSplineOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpSplineOptions);
			this.Name = "SplineGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlLineTension)).EndInit();
			this.pnlLineTension.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnLineTension.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpSplineOptions)).EndInit();
			this.grpSplineOptions.ResumeLayout(false);
			this.grpSplineOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlLineTension;
		private ChartLabelControl lblLineTension;
		private DevExpress.XtraEditors.SpinEdit spnLineTension;
		private DevExpress.XtraEditors.GroupControl grpSplineOptions;
	}
}
