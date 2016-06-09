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
	partial class AnnotationSeriesPointAnchorPointControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationSeriesPointAnchorPointControl));
			this.pnlPoints = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.btnPoints = new DevExpress.XtraEditors.ButtonEdit();
			this.lblPoints = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlPoints)).BeginInit();
			this.pnlPoints.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnPoints.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlPoints, "pnlPoints");
			this.pnlPoints.BackColor = System.Drawing.Color.Transparent;
			this.pnlPoints.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPoints.Controls.Add(this.btnPoints);
			this.pnlPoints.Controls.Add(this.lblPoints);
			this.pnlPoints.Name = "pnlPoints";
			resources.ApplyResources(this.btnPoints, "btnPoints");
			this.btnPoints.Name = "btnPoints";
			this.btnPoints.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnPoints.Properties.ReadOnly = true;
			this.btnPoints.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnPoints_ButtonClick);
			resources.ApplyResources(this.lblPoints, "lblPoints");
			this.lblPoints.Name = "lblPoints";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlPoints);
			this.Name = "AnnotationSeriesPointAnchorPointControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlPoints)).EndInit();
			this.pnlPoints.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.btnPoints.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlPoints;
		private DevExpress.XtraEditors.ButtonEdit btnPoints;
		private ChartLabelControl lblPoints;
	}
}
