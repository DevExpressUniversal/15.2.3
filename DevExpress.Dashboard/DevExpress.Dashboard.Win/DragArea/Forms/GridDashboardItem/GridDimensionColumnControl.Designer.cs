#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class GridDimensionColumnControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridDimensionColumnControl));
			this.showLabel = new DevExpress.XtraEditors.LabelControl();
			this.noOptionsLabel = new DevExpress.XtraEditors.LabelControl();
			this.dimensionControlsPanel = new DevExpress.XtraEditors.PanelControl();
			this.textCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.imageCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.dimensionControlsPanel)).BeginInit();
			this.dimensionControlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.textCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCheckEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.showLabel, "showLabel");
			this.showLabel.Name = "showLabel";
			resources.ApplyResources(this.noOptionsLabel, "noOptionsLabel");
			this.noOptionsLabel.Name = "noOptionsLabel";
			this.dimensionControlsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.dimensionControlsPanel.Controls.Add(this.textCheckEdit);
			this.dimensionControlsPanel.Controls.Add(this.imageCheckEdit);
			resources.ApplyResources(this.dimensionControlsPanel, "dimensionControlsPanel");
			this.dimensionControlsPanel.Name = "dimensionControlsPanel";
			resources.ApplyResources(this.textCheckEdit, "textCheckEdit");
			this.textCheckEdit.Name = "textCheckEdit";
			this.textCheckEdit.Properties.Caption = resources.GetString("textCheckEdit.Properties.Caption");
			this.textCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.textCheckEdit.Properties.RadioGroupIndex = 3;
			this.textCheckEdit.CheckedChanged += new System.EventHandler(this.TextCheckEditCheckedChanged);
			resources.ApplyResources(this.imageCheckEdit, "imageCheckEdit");
			this.imageCheckEdit.Name = "imageCheckEdit";
			this.imageCheckEdit.Properties.Caption = resources.GetString("imageCheckEdit.Properties.Caption");
			this.imageCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.imageCheckEdit.Properties.RadioGroupIndex = 3;
			this.imageCheckEdit.TabStop = false;
			this.imageCheckEdit.CheckedChanged += new System.EventHandler(this.ImageCheckEditCheckedChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.showLabel);
			this.Controls.Add(this.noOptionsLabel);
			this.Controls.Add(this.dimensionControlsPanel);
			this.Name = "GridDimensionColumnControl";
			((System.ComponentModel.ISupportInitialize)(this.dimensionControlsPanel)).EndInit();
			this.dimensionControlsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.textCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCheckEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl showLabel;
		private XtraEditors.LabelControl noOptionsLabel;
		private XtraEditors.PanelControl dimensionControlsPanel;
		private XtraEditors.CheckEdit textCheckEdit;
		private XtraEditors.CheckEdit imageCheckEdit;
	}
}
