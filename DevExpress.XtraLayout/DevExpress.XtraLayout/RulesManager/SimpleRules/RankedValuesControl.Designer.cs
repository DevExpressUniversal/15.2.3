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

namespace DevExpress.XtraEditors.Frames {
	partial class RankedValuesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcRankedValues = new DevExpress.XtraLayout.LayoutControl();
			this.chePercent = new DevExpress.XtraEditors.CheckEdit();
			this.tedCount = new DevExpress.XtraEditors.TextEdit();
			this.cmbRankedValues = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgRankedValues = new DevExpress.XtraLayout.LayoutControlGroup();
			this.sliInfo = new DevExpress.XtraLayout.SimpleLabelItem();
			this.lciRankedValues = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCount = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPercent = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.lcRankedValues)).BeginInit();
			this.lcRankedValues.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chePercent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tedCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbRankedValues.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRankedValues)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciRankedValues)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPercent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.lcRankedValues.AllowCustomization = false;
			this.lcRankedValues.Controls.Add(this.chePercent);
			this.lcRankedValues.Controls.Add(this.tedCount);
			this.lcRankedValues.Controls.Add(this.cmbRankedValues);
			this.lcRankedValues.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcRankedValues.Location = new System.Drawing.Point(0, 0);
			this.lcRankedValues.Name = "lcRankedValues";
			this.lcRankedValues.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(623, 195, 885, 567);
			this.lcRankedValues.Root = this.lcgRankedValues;
			this.lcRankedValues.Size = new System.Drawing.Size(470, 41);
			this.lcRankedValues.TabIndex = 0;
			this.lcRankedValues.Text = "layoutControl1";
			this.chePercent.Location = new System.Drawing.Point(198, 19);
			this.chePercent.Name = "chePercent";
			this.chePercent.Properties.Caption = "% of the column\'s cell values";
			this.chePercent.Size = new System.Drawing.Size(270, 19);
			this.chePercent.StyleController = this.lcRankedValues;
			this.chePercent.TabIndex = 6;
			this.tedCount.EditValue = "";
			this.tedCount.Location = new System.Drawing.Point(118, 19);
			this.tedCount.Name = "tedCount";
			this.tedCount.Size = new System.Drawing.Size(66, 20);
			this.tedCount.StyleController = this.lcRankedValues;
			this.tedCount.TabIndex = 5;
			this.cmbRankedValues.EditValue = "";
			this.cmbRankedValues.Location = new System.Drawing.Point(8, 19);
			this.cmbRankedValues.Name = "cmbRankedValues";
			this.cmbRankedValues.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbRankedValues.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbRankedValues.Size = new System.Drawing.Size(96, 20);
			this.cmbRankedValues.StyleController = this.lcRankedValues;
			this.cmbRankedValues.TabIndex = 4;
			this.lcgRankedValues.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgRankedValues.GroupBordersVisible = false;
			this.lcgRankedValues.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.sliInfo,
			this.lciRankedValues,
			this.lciCount,
			this.lciPercent,
			this.emptySpaceItem1,
			this.emptySpaceItem2});
			this.lcgRankedValues.Location = new System.Drawing.Point(0, 0);
			this.lcgRankedValues.Name = "Root";
			this.lcgRankedValues.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
			this.lcgRankedValues.Size = new System.Drawing.Size(470, 41);
			this.lcgRankedValues.TextVisible = false;
			this.sliInfo.AllowHotTrack = false;
			this.sliInfo.Location = new System.Drawing.Point(0, 0);
			this.sliInfo.Name = "sliInfo";
			this.sliInfo.Size = new System.Drawing.Size(464, 17);
			this.sliInfo.Text = "Format values that rank in the:";
			this.sliInfo.TextSize = new System.Drawing.Size(149, 13);
			this.lciRankedValues.Control = this.cmbRankedValues;
			this.lciRankedValues.Location = new System.Drawing.Point(0, 17);
			this.lciRankedValues.Name = "lciRankedValues";
			this.lciRankedValues.Size = new System.Drawing.Size(100, 24);
			this.lciRankedValues.TextSize = new System.Drawing.Size(0, 0);
			this.lciRankedValues.TextVisible = false;
			this.lciCount.Control = this.tedCount;
			this.lciCount.Location = new System.Drawing.Point(110, 17);
			this.lciCount.Name = "lciCount";
			this.lciCount.Size = new System.Drawing.Size(70, 24);
			this.lciCount.TextSize = new System.Drawing.Size(0, 0);
			this.lciCount.TextVisible = false;
			this.lciPercent.Control = this.chePercent;
			this.lciPercent.Location = new System.Drawing.Point(190, 17);
			this.lciPercent.Name = "lciPercent";
			this.lciPercent.Size = new System.Drawing.Size(274, 24);
			this.lciPercent.TextSize = new System.Drawing.Size(0, 0);
			this.lciPercent.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(100, 17);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(180, 17);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcRankedValues);
			this.Name = "RankedValuesControl";
			this.Size = new System.Drawing.Size(470, 41);
			((System.ComponentModel.ISupportInitialize)(this.lcRankedValues)).EndInit();
			this.lcRankedValues.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chePercent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tedCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbRankedValues.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRankedValues)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciRankedValues)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPercent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcRankedValues;
		private DevExpress.XtraLayout.LayoutControlGroup lcgRankedValues;
		private DevExpress.XtraEditors.CheckEdit chePercent;
		private DevExpress.XtraEditors.TextEdit tedCount;
		private DevExpress.XtraEditors.ComboBoxEdit cmbRankedValues;
		private DevExpress.XtraLayout.SimpleLabelItem sliInfo;
		private DevExpress.XtraLayout.LayoutControlItem lciRankedValues;
		private DevExpress.XtraLayout.LayoutControlItem lciCount;
		private DevExpress.XtraLayout.LayoutControlItem lciPercent;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
	}
}
