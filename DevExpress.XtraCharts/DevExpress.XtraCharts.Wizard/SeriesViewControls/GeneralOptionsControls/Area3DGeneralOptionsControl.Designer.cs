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
	partial class Area3DGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Area3DGeneralOptionsControl));
			this.grpAreaOptions = new DevExpress.XtraEditors.GroupControl();
			this.pnlWidth = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblWidth = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpAreaOptions)).BeginInit();
			this.grpAreaOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).BeginInit();
			this.pnlWidth.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnWidth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpAreaOptions, "grpAreaOptions");
			this.grpAreaOptions.Controls.Add(this.pnlWidth);
			this.grpAreaOptions.Name = "grpAreaOptions";
			resources.ApplyResources(this.pnlWidth, "pnlWidth");
			this.pnlWidth.BackColor = System.Drawing.Color.Transparent;
			this.pnlWidth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlWidth.Controls.Add(this.spnWidth);
			this.pnlWidth.Controls.Add(this.lblWidth);
			this.pnlWidth.Name = "pnlWidth";
			resources.ApplyResources(this.spnWidth, "spnWidth");
			this.spnWidth.Name = "spnWidth";
			this.spnWidth.Properties.Appearance.Options.UseTextOptions = true;
			this.spnWidth.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.spnWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnWidth.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnWidth.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("spnWidth.Properties.Mask.IgnoreMaskBlank")));
			this.spnWidth.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("spnWidth.Properties.Mask.ShowPlaceHolders")));
			this.spnWidth.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.spnWidth.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			262144});
			this.spnWidth.EditValueChanged += new System.EventHandler(this.spnWidth_EditValueChanged);
			resources.ApplyResources(this.lblWidth, "lblWidth");
			this.lblWidth.Name = "lblWidth";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpAreaOptions);
			this.Name = "Area3DGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpAreaOptions)).EndInit();
			this.grpAreaOptions.ResumeLayout(false);
			this.grpAreaOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlWidth)).EndInit();
			this.pnlWidth.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnWidth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpAreaOptions;
		private ChartPanelControl pnlWidth;
		private DevExpress.XtraEditors.SpinEdit spnWidth;
		private ChartLabelControl lblWidth;
	}
}
