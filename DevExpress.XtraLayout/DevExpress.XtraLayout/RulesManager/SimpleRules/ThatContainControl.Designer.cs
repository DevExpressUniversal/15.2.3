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
	partial class ThatContainControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcThatContain = new DevExpress.XtraLayout.LayoutControl();
			this.lbcConjunction = new DevExpress.XtraEditors.LabelControl();
			this.tedEndValue = new DevExpress.XtraEditors.TextEdit();
			this.tedBeginValue = new DevExpress.XtraEditors.TextEdit();
			this.cmbContentRule = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cmbThatContain = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgThatContain = new DevExpress.XtraLayout.LayoutControlGroup();
			this.sliInfo = new DevExpress.XtraLayout.SimpleLabelItem();
			this.lciThatContain = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciContentRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgFullCellValue = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciConjunction = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciEndValue = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgSpecificText = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciDeginValue = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.lcThatContain)).BeginInit();
			this.lcThatContain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tedEndValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tedBeginValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbContentRule.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbThatContain.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgThatContain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciThatContain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciContentRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFullCellValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciConjunction)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEndValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgSpecificText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDeginValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.lcThatContain.AllowCustomization = false;
			this.lcThatContain.Controls.Add(this.lbcConjunction);
			this.lcThatContain.Controls.Add(this.tedEndValue);
			this.lcThatContain.Controls.Add(this.tedBeginValue);
			this.lcThatContain.Controls.Add(this.cmbContentRule);
			this.lcThatContain.Controls.Add(this.cmbThatContain);
			this.lcThatContain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcThatContain.Location = new System.Drawing.Point(0, 0);
			this.lcThatContain.Name = "lcThatContain";
			this.lcThatContain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(449, 261, 836, 628);
			this.lcThatContain.Root = this.lcgThatContain;
			this.lcThatContain.Size = new System.Drawing.Size(470, 41);
			this.lcThatContain.TabIndex = 0;
			this.lcThatContain.Text = "layoutControl1";
			this.lbcConjunction.Location = new System.Drawing.Point(309, 22);
			this.lbcConjunction.Name = "lbcConjunction";
			this.lbcConjunction.Size = new System.Drawing.Size(18, 13);
			this.lbcConjunction.StyleController = this.lcThatContain;
			this.lbcConjunction.TabIndex = 8;
			this.lbcConjunction.Text = "and";
			this.tedEndValue.Location = new System.Drawing.Point(342, 19);
			this.tedEndValue.Name = "tedEndValue";
			this.tedEndValue.Size = new System.Drawing.Size(126, 20);
			this.tedEndValue.StyleController = this.lcThatContain;
			this.tedEndValue.TabIndex = 7;
			this.tedBeginValue.Location = new System.Drawing.Point(228, 19);
			this.tedBeginValue.Name = "tedBeginValue";
			this.tedBeginValue.Size = new System.Drawing.Size(66, 20);
			this.tedBeginValue.StyleController = this.lcThatContain;
			this.tedBeginValue.TabIndex = 6;
			this.cmbContentRule.Location = new System.Drawing.Point(118, 19);
			this.cmbContentRule.Name = "cmbContentRule";
			this.cmbContentRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbContentRule.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbContentRule.Size = new System.Drawing.Size(96, 20);
			this.cmbContentRule.StyleController = this.lcThatContain;
			this.cmbContentRule.TabIndex = 5;
			this.cmbContentRule.SelectedIndexChanged += new System.EventHandler(this.cmbContentRule_SelectedIndexChanged);
			this.cmbThatContain.EditValue = "";
			this.cmbThatContain.Location = new System.Drawing.Point(8, 19);
			this.cmbThatContain.Name = "cmbThatContain";
			this.cmbThatContain.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbThatContain.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbThatContain.Size = new System.Drawing.Size(96, 20);
			this.cmbThatContain.StyleController = this.lcThatContain;
			this.cmbThatContain.TabIndex = 4;
			this.cmbThatContain.SelectedIndexChanged += new System.EventHandler(this.cmbThatContain_SelectedIndexChanged);
			this.lcgThatContain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgThatContain.GroupBordersVisible = false;
			this.lcgThatContain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.sliInfo,
			this.lciThatContain,
			this.lciContentRule,
			this.lcgFullCellValue,
			this.lcgSpecificText,
			this.emptySpaceItem4,
			this.emptySpaceItem1});
			this.lcgThatContain.Location = new System.Drawing.Point(0, 0);
			this.lcgThatContain.Name = "Root";
			this.lcgThatContain.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
			this.lcgThatContain.Size = new System.Drawing.Size(470, 41);
			this.lcgThatContain.TextVisible = false;
			this.sliInfo.AllowHotTrack = false;
			this.sliInfo.Location = new System.Drawing.Point(0, 0);
			this.sliInfo.Name = "sliInfo";
			this.sliInfo.Size = new System.Drawing.Size(464, 17);
			this.sliInfo.Text = "Format only cells with:";
			this.sliInfo.TextSize = new System.Drawing.Size(107, 13);
			this.lciThatContain.Control = this.cmbThatContain;
			this.lciThatContain.Location = new System.Drawing.Point(0, 17);
			this.lciThatContain.Name = "lciThatContain";
			this.lciThatContain.Size = new System.Drawing.Size(100, 24);
			this.lciThatContain.TextSize = new System.Drawing.Size(0, 0);
			this.lciThatContain.TextVisible = false;
			this.lciContentRule.Control = this.cmbContentRule;
			this.lciContentRule.Location = new System.Drawing.Point(110, 17);
			this.lciContentRule.Name = "lciContentRule";
			this.lciContentRule.Size = new System.Drawing.Size(100, 24);
			this.lciContentRule.TextSize = new System.Drawing.Size(0, 0);
			this.lciContentRule.TextVisible = false;
			this.lcgFullCellValue.GroupBordersVisible = false;
			this.lcgFullCellValue.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem2,
			this.lciConjunction,
			this.emptySpaceItem3,
			this.lciEndValue});
			this.lcgFullCellValue.Location = new System.Drawing.Point(290, 17);
			this.lcgFullCellValue.Name = "lcgFullCellValue";
			this.lcgFullCellValue.Size = new System.Drawing.Size(174, 24);
			this.lcgFullCellValue.Text = "Full Cell Value";
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.lciConjunction.Control = this.lbcConjunction;
			this.lciConjunction.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.lciConjunction.Location = new System.Drawing.Point(10, 0);
			this.lciConjunction.Name = "lciConjunction";
			this.lciConjunction.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.lciConjunction.Size = new System.Drawing.Size(24, 24);
			this.lciConjunction.TextSize = new System.Drawing.Size(0, 0);
			this.lciConjunction.TextVisible = false;
			this.lciConjunction.TrimClientAreaToControl = false;
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(34, 0);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.lciEndValue.Control = this.tedEndValue;
			this.lciEndValue.Location = new System.Drawing.Point(44, 0);
			this.lciEndValue.Name = "lciEndValue";
			this.lciEndValue.Size = new System.Drawing.Size(130, 24);
			this.lciEndValue.TextSize = new System.Drawing.Size(0, 0);
			this.lciEndValue.TextVisible = false;
			this.lcgSpecificText.GroupBordersVisible = false;
			this.lcgSpecificText.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciDeginValue});
			this.lcgSpecificText.Location = new System.Drawing.Point(220, 17);
			this.lcgSpecificText.Name = "lcgSpecificText";
			this.lcgSpecificText.Size = new System.Drawing.Size(70, 24);
			this.lcgSpecificText.Text = "Specific Text";
			this.lciDeginValue.Control = this.tedBeginValue;
			this.lciDeginValue.Location = new System.Drawing.Point(0, 0);
			this.lciDeginValue.Name = "lciDeginValue";
			this.lciDeginValue.Size = new System.Drawing.Size(70, 24);
			this.lciDeginValue.TextSize = new System.Drawing.Size(0, 0);
			this.lciDeginValue.TextVisible = false;
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(100, 17);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(210, 17);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcThatContain);
			this.Name = "ThatContainControl";
			this.Size = new System.Drawing.Size(470, 41);
			((System.ComponentModel.ISupportInitialize)(this.lcThatContain)).EndInit();
			this.lcThatContain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tedEndValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tedBeginValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbContentRule.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbThatContain.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgThatContain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciThatContain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciContentRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFullCellValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciConjunction)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEndValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgSpecificText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDeginValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcThatContain;
		private DevExpress.XtraLayout.LayoutControlGroup lcgThatContain;
		public DevExpress.XtraEditors.ComboBoxEdit cmbThatContain;
		private DevExpress.XtraLayout.SimpleLabelItem sliInfo;
		private DevExpress.XtraLayout.LayoutControlItem lciThatContain;
		private DevExpress.XtraEditors.ComboBoxEdit cmbContentRule;
		private DevExpress.XtraLayout.LayoutControlItem lciContentRule;
		private DevExpress.XtraEditors.TextEdit tedEndValue;
		private DevExpress.XtraEditors.TextEdit tedBeginValue;
		private DevExpress.XtraLayout.LayoutControlItem lciDeginValue;
		private DevExpress.XtraLayout.LayoutControlItem lciEndValue;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraEditors.LabelControl lbcConjunction;
		private DevExpress.XtraLayout.LayoutControlItem lciConjunction;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.LayoutControlGroup lcgFullCellValue;
		private DevExpress.XtraLayout.LayoutControlGroup lcgSpecificText;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
	}
}
