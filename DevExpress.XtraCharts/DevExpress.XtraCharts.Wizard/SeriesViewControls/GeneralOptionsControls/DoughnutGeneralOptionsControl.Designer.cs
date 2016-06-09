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
	partial class DoughnutGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DoughnutGeneralOptionsControl));
			this.doughnutOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.DoughnutOptionsControl();
			this.sepSeparator = new DevExpress.XtraCharts.Wizard.ChartUserControl();
			this.pieOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.PieGeneralOptionsControl();
			this.SuspendLayout();
			resources.ApplyResources(this.doughnutOptionsControl, "doughnutOptionsControl");
			this.doughnutOptionsControl.Name = "doughnutOptionsControl";
			resources.ApplyResources(this.sepSeparator, "sepSeparator");
			this.sepSeparator.Name = "sepSeparator";
			resources.ApplyResources(this.pieOptionsControl, "pieOptionsControl");
			this.pieOptionsControl.Name = "pieOptionsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pieOptionsControl);
			this.Controls.Add(this.sepSeparator);
			this.Controls.Add(this.doughnutOptionsControl);
			this.Name = "DoughnutGeneralOptionsControl";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DoughnutOptionsControl doughnutOptionsControl;
		private ChartUserControl sepSeparator;
		private PieGeneralOptionsControl pieOptionsControl;
	}
}
