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
	partial class LayoutSettingsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutSettingsControl));
			this.groupLocation = new DevExpress.XtraEditors.GroupControl();
			this.pnTextDirection = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbTextDirection = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelTextOrientation = new DevExpress.XtraEditors.LabelControl();
			this.separator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnOptions = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.groupLocation)).BeginInit();
			this.groupLocation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnTextDirection)).BeginInit();
			this.pnTextDirection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbTextDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnOptions)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.groupLocation, "groupLocation");
			this.groupLocation.Controls.Add(this.pnTextDirection);
			this.groupLocation.Controls.Add(this.separator);
			this.groupLocation.Controls.Add(this.pnOptions);
			this.groupLocation.Name = "groupLocation";
			this.pnTextDirection.BackColor = System.Drawing.Color.Transparent;
			this.pnTextDirection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnTextDirection.Controls.Add(this.cbTextDirection);
			this.pnTextDirection.Controls.Add(this.labelTextOrientation);
			resources.ApplyResources(this.pnTextDirection, "pnTextDirection");
			this.pnTextDirection.Name = "pnTextDirection";
			resources.ApplyResources(this.cbTextDirection, "cbTextDirection");
			this.cbTextDirection.Name = "cbTextDirection";
			this.cbTextDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTextDirection.Properties.Buttons"))))});
			this.cbTextDirection.SelectedIndexChanged += new System.EventHandler(this.cbTextDirection_SelectedIndexChanged);
			resources.ApplyResources(this.labelTextOrientation, "labelTextOrientation");
			this.labelTextOrientation.Name = "labelTextOrientation";
			this.separator.BackColor = System.Drawing.Color.Transparent;
			this.separator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separator, "separator");
			this.separator.Name = "separator";
			resources.ApplyResources(this.pnOptions, "pnOptions");
			this.pnOptions.BackColor = System.Drawing.Color.Transparent;
			this.pnOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnOptions.Name = "pnOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupLocation);
			this.Name = "LayoutSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.groupLocation)).EndInit();
			this.groupLocation.ResumeLayout(false);
			this.groupLocation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnTextDirection)).EndInit();
			this.pnTextDirection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbTextDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl groupLocation;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnTextDirection;
		private DevExpress.XtraEditors.LabelControl labelTextOrientation;
		private DevExpress.XtraEditors.ComboBoxEdit cbTextDirection;
		private ChartPanelControl pnOptions;
		private ChartPanelControl separator;
	}
}
