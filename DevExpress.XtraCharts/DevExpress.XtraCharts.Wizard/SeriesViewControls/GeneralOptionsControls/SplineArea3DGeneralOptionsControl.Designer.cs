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
	partial class SplineArea3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplineArea3DGeneralOptionsControl));
			this.splineOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.SplineGeneralOptionsControl();
			this.sepSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.area3DOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.Area3DGeneralOptionsControl();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splineOptionsControl, "splineOptionsControl");
			this.splineOptionsControl.Name = "splineOptionsControl";
			this.sepSeparator.BackColor = System.Drawing.Color.Transparent;
			this.sepSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSeparator, "sepSeparator");
			this.sepSeparator.Name = "sepSeparator";
			resources.ApplyResources(this.area3DOptionsControl, "area3DOptionsControl");
			this.area3DOptionsControl.Name = "area3DOptionsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.area3DOptionsControl);
			this.Controls.Add(this.sepSeparator);
			this.Controls.Add(this.splineOptionsControl);
			this.Name = "SplineArea3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private SplineGeneralOptionsControl splineOptionsControl;
		private ChartPanelControl sepSeparator;
		private Area3DGeneralOptionsControl area3DOptionsControl;
	}
}
