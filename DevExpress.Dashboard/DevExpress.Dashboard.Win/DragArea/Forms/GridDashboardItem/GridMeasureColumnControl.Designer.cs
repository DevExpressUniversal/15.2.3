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
	partial class GridMeasureColumnControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridMeasureColumnControl));
			this.showLabel = new DevExpress.XtraEditors.LabelControl();
			this.showZeroLevelCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.valueBarControl = new DevExpress.DashboardWin.Native.ValueBarControl();
			((System.ComponentModel.ISupportInitialize)(this.showZeroLevelCheckEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.showLabel, "showLabel");
			this.showLabel.Name = "showLabel";
			resources.ApplyResources(this.showZeroLevelCheckEdit, "showZeroLevelCheckEdit");
			this.showZeroLevelCheckEdit.Name = "showZeroLevelCheckEdit";
			this.showZeroLevelCheckEdit.Properties.Caption = resources.GetString("showZeroLevelCheckEdit.Properties.Caption");
			this.showZeroLevelCheckEdit.CheckedChanged += new System.EventHandler(this.ShowZeroLevelCheckEditCheckedChanged);
			resources.ApplyResources(this.valueBarControl, "valueBarControl");
			this.valueBarControl.Name = "valueBarControl";
			this.valueBarControl.DisplayModeChanged += new System.EventHandler<DevExpress.DashboardWin.Native.ValueBarDisplayModeEventArgs>(this.ValueBarControlDisplayModeChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.showZeroLevelCheckEdit);
			this.Controls.Add(this.valueBarControl);
			this.Controls.Add(this.showLabel);
			this.Name = "GridMeasureColumnControl";
			((System.ComponentModel.ISupportInitialize)(this.showZeroLevelCheckEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl showLabel;
		private XtraEditors.CheckEdit showZeroLevelCheckEdit;
		private DevExpress.DashboardWin.Native.ValueBarControl valueBarControl;
	}
}
