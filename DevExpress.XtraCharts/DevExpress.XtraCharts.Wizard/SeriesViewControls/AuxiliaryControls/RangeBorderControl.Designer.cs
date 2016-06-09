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
	partial class RangeBorderControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RangeBorderControl));
			this.grpBorder1 = new DevExpress.XtraEditors.GroupControl();
			this.border1Control = new DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl();
			this.sepBorder = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpBorder2 = new DevExpress.XtraEditors.GroupControl();
			this.border2Control = new DevExpress.XtraCharts.Wizard.SeriesViewControls.BorderControl();
			((System.ComponentModel.ISupportInitialize)(this.grpBorder1)).BeginInit();
			this.grpBorder1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepBorder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBorder2)).BeginInit();
			this.grpBorder2.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.grpBorder1, "grpBorder1");
			this.grpBorder1.Controls.Add(this.border1Control);
			this.grpBorder1.Name = "grpBorder1";
			resources.ApplyResources(this.border1Control, "border1Control");
			this.border1Control.Name = "border1Control";
			this.sepBorder.BackColor = System.Drawing.Color.Transparent;
			this.sepBorder.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepBorder, "sepBorder");
			this.sepBorder.Name = "sepBorder";
			resources.ApplyResources(this.grpBorder2, "grpBorder2");
			this.grpBorder2.Controls.Add(this.border2Control);
			this.grpBorder2.Name = "grpBorder2";
			resources.ApplyResources(this.border2Control, "border2Control");
			this.border2Control.Name = "border2Control";
			resources.ApplyResources(this, "$this");
			this.CausesValidation = false;
			this.Controls.Add(this.grpBorder2);
			this.Controls.Add(this.sepBorder);
			this.Controls.Add(this.grpBorder1);
			this.Name = "RangeBorderControl";
			((System.ComponentModel.ISupportInitialize)(this.grpBorder1)).EndInit();
			this.grpBorder1.ResumeLayout(false);
			this.grpBorder1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepBorder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBorder2)).EndInit();
			this.grpBorder2.ResumeLayout(false);
			this.grpBorder2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.GroupControl grpBorder1;
		private BorderControl border1Control;
		private ChartPanelControl sepBorder;
		protected DevExpress.XtraEditors.GroupControl grpBorder2;
		private BorderControl border2Control;
	}
}
