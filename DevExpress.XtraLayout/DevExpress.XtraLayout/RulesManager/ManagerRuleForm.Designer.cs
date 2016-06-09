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

using DevExpress.XtraEditors;
namespace DevExpress.XtraEditors.Frames {
	partial class ManagerRuleForm<T, TColumnType> where T : FormatRuleBase, new() {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.ManagerRuleFormConvertedLayout = new DevExpress.XtraLayout.LayoutControl();
			this.cmbInfoColumn = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1ConvertedLayout = new DevExpress.XtraLayout.LayoutControl();
			this.btnNewRule = new DevExpress.XtraEditors.SimpleButton();
			this.btnEditRule = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDeleteRule = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciNewRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciEditRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDown = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciUp = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDeleteRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciShowFormattingRules = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciApply = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOK = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.ManagerRuleFormConvertedLayout)).BeginInit();
			this.ManagerRuleFormConvertedLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cmbInfoColumn.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1ConvertedLayout)).BeginInit();
			this.panelControl1ConvertedLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciNewRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDeleteRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowFormattingRules)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			this.SuspendLayout();
			this.btnApply.Location = new System.Drawing.Point(466, 252);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(79, 22);
			this.btnApply.StyleController = this.ManagerRuleFormConvertedLayout;
			this.btnApply.TabIndex = 6;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.ManagerRuleFormConvertedLayout.AllowCustomization = false;
			this.ManagerRuleFormConvertedLayout.Controls.Add(this.cmbInfoColumn);
			this.ManagerRuleFormConvertedLayout.Controls.Add(this.btnCancel);
			this.ManagerRuleFormConvertedLayout.Controls.Add(this.btnApply);
			this.ManagerRuleFormConvertedLayout.Controls.Add(this.btnOK);
			this.ManagerRuleFormConvertedLayout.Controls.Add(this.panelControl1ConvertedLayout);
			this.ManagerRuleFormConvertedLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ManagerRuleFormConvertedLayout.Location = new System.Drawing.Point(0, 0);
			this.ManagerRuleFormConvertedLayout.Name = "ManagerRuleFormConvertedLayout";
			this.ManagerRuleFormConvertedLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(965, 55, 811, 646);
			this.ManagerRuleFormConvertedLayout.Root = this.layoutControlGroup1;
			this.ManagerRuleFormConvertedLayout.Size = new System.Drawing.Size(557, 286);
			this.ManagerRuleFormConvertedLayout.TabIndex = 13;
			this.cmbInfoColumn.Location = new System.Drawing.Point(141, 12);
			this.cmbInfoColumn.Name = "cmbInfoColumn";
			this.cmbInfoColumn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbInfoColumn.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbInfoColumn.Size = new System.Drawing.Size(148, 20);
			this.cmbInfoColumn.StyleController = this.ManagerRuleFormConvertedLayout;
			this.cmbInfoColumn.TabIndex = 11;
			this.cmbInfoColumn.SelectedIndexChanged += new System.EventHandler(this.cmbInfoColumn_SelectedIndexChanged);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(383, 252);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(79, 22);
			this.btnCancel.StyleController = this.ManagerRuleFormConvertedLayout;
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(300, 252);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(79, 22);
			this.btnOK.StyleController = this.ManagerRuleFormConvertedLayout;
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "ОК";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.panelControl1ConvertedLayout.AllowCustomization = false;
			this.panelControl1ConvertedLayout.Controls.Add(this.btnNewRule);
			this.panelControl1ConvertedLayout.Controls.Add(this.btnEditRule);
			this.panelControl1ConvertedLayout.Controls.Add(this.btnDown);
			this.panelControl1ConvertedLayout.Controls.Add(this.btnUp);
			this.panelControl1ConvertedLayout.Controls.Add(this.btnDeleteRule);
			this.panelControl1ConvertedLayout.Location = new System.Drawing.Point(10, 34);
			this.panelControl1ConvertedLayout.Name = "panelControl1ConvertedLayout";
			this.panelControl1ConvertedLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(971, 78, 672, 851);
			this.panelControl1ConvertedLayout.Root = this.layoutControlGroup2;
			this.panelControl1ConvertedLayout.Size = new System.Drawing.Size(537, 206);
			this.panelControl1ConvertedLayout.TabIndex = 13;
			this.btnNewRule.Location = new System.Drawing.Point(2, 2);
			this.btnNewRule.Name = "btnNewRule";
			this.btnNewRule.Size = new System.Drawing.Size(81, 22);
			this.btnNewRule.StyleController = this.panelControl1ConvertedLayout;
			this.btnNewRule.TabIndex = 1;
			this.btnNewRule.Text = "New Rule...";
			this.btnNewRule.Click += new System.EventHandler(this.btnNewRule_Click);
			this.btnEditRule.Location = new System.Drawing.Point(87, 2);
			this.btnEditRule.Name = "btnEditRule";
			this.btnEditRule.Size = new System.Drawing.Size(80, 22);
			this.btnEditRule.StyleController = this.panelControl1ConvertedLayout;
			this.btnEditRule.TabIndex = 2;
			this.btnEditRule.Text = "Edit Rule...";
			this.btnEditRule.Click += new System.EventHandler(this.btnEditRule_Click);
			this.btnDown.Location = new System.Drawing.Point(482, 2);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(53, 22);
			this.btnDown.StyleController = this.panelControl1ConvertedLayout;
			this.btnDown.TabIndex = 8;
			this.btnDown.Text = "Down";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			this.btnUp.Location = new System.Drawing.Point(425, 2);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(53, 22);
			this.btnUp.StyleController = this.panelControl1ConvertedLayout;
			this.btnUp.TabIndex = 7;
			this.btnUp.Text = "Up";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDeleteRule.Location = new System.Drawing.Point(171, 2);
			this.btnDeleteRule.Name = "btnDeleteRule";
			this.btnDeleteRule.Size = new System.Drawing.Size(81, 22);
			this.btnDeleteRule.StyleController = this.panelControl1ConvertedLayout;
			this.btnDeleteRule.TabIndex = 3;
			this.btnDeleteRule.Text = "Delete Rule";
			this.btnDeleteRule.Click += new System.EventHandler(this.btnDeleteRule_Click);
			this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciNewRule,
			this.layoutControlItem8,
			this.lciEditRule,
			this.lciDown,
			this.lciUp,
			this.lciDeleteRule,
			this.emptySpaceItem3});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "Root";
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(537, 206);
			this.layoutControlGroup2.TextVisible = false;
			this.lciNewRule.Control = this.btnNewRule;
			this.lciNewRule.Location = new System.Drawing.Point(0, 0);
			this.lciNewRule.Name = "lciNewRule";
			this.lciNewRule.Size = new System.Drawing.Size(85, 26);
			this.lciNewRule.TextSize = new System.Drawing.Size(0, 0);
			this.lciNewRule.TextVisible = false;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem8.Name = "lbxRulesitem";
			this.layoutControlItem8.Size = new System.Drawing.Size(537, 180);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.lciEditRule.Control = this.btnEditRule;
			this.lciEditRule.Location = new System.Drawing.Point(85, 0);
			this.lciEditRule.Name = "lciEditRule";
			this.lciEditRule.Size = new System.Drawing.Size(84, 26);
			this.lciEditRule.TextSize = new System.Drawing.Size(0, 0);
			this.lciEditRule.TextVisible = false;
			this.lciDown.Control = this.btnDown;
			this.lciDown.Location = new System.Drawing.Point(480, 0);
			this.lciDown.Name = "lciDown";
			this.lciDown.Size = new System.Drawing.Size(57, 26);
			this.lciDown.TextSize = new System.Drawing.Size(0, 0);
			this.lciDown.TextVisible = false;
			this.lciUp.Control = this.btnUp;
			this.lciUp.Location = new System.Drawing.Point(423, 0);
			this.lciUp.Name = "lciUp";
			this.lciUp.Size = new System.Drawing.Size(57, 26);
			this.lciUp.TextSize = new System.Drawing.Size(0, 0);
			this.lciUp.TextVisible = false;
			this.lciDeleteRule.Control = this.btnDeleteRule;
			this.lciDeleteRule.Location = new System.Drawing.Point(169, 0);
			this.lciDeleteRule.Name = "lciDeleteRule";
			this.lciDeleteRule.Size = new System.Drawing.Size(85, 26);
			this.lciDeleteRule.TextSize = new System.Drawing.Size(0, 0);
			this.lciDeleteRule.TextVisible = false;
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(254, 0);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(169, 26);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciShowFormattingRules,
			this.lciCancel,
			this.lciApply,
			this.lciOK,
			this.emptySpaceItem1,
			this.emptySpaceItem2,
			this.layoutControlItem6,
			this.emptySpaceItem4});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(557, 286);
			this.layoutControlGroup1.TextVisible = false;
			this.lciShowFormattingRules.Control = this.cmbInfoColumn;
			this.lciShowFormattingRules.Location = new System.Drawing.Point(0, 0);
			this.lciShowFormattingRules.Name = "cmbInfoColumnitem";
			this.lciShowFormattingRules.Size = new System.Drawing.Size(281, 24);
			this.lciShowFormattingRules.Text = "Show formatting rules for:";
			this.lciShowFormattingRules.TextLocation = DevExpress.Utils.Locations.Left;
			this.lciShowFormattingRules.TextSize = new System.Drawing.Size(126, 13);
			this.lciCancel.Control = this.btnCancel;
			this.lciCancel.Location = new System.Drawing.Point(371, 240);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Size = new System.Drawing.Size(83, 26);
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.lciApply.Control = this.btnApply;
			this.lciApply.Location = new System.Drawing.Point(454, 240);
			this.lciApply.Name = "lciApply";
			this.lciApply.Size = new System.Drawing.Size(83, 26);
			this.lciApply.TextSize = new System.Drawing.Size(0, 0);
			this.lciApply.TextVisible = false;
			this.lciOK.Control = this.btnOK;
			this.lciOK.Location = new System.Drawing.Point(288, 240);
			this.lciOK.Name = "lciOK";
			this.lciOK.Size = new System.Drawing.Size(83, 26);
			this.lciOK.TextSize = new System.Drawing.Size(0, 0);
			this.lciOK.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(281, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(256, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 240);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(288, 26);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.Control = this.panelControl1ConvertedLayout;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(537, 206);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(0, 230);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem4.Size = new System.Drawing.Size(537, 10);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.AcceptButton = this.btnApply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(557, 286);
			this.Controls.Add(this.ManagerRuleFormConvertedLayout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ManagerRuleForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Conditional Formatting Rules Manager";
			((System.ComponentModel.ISupportInitialize)(this.ManagerRuleFormConvertedLayout)).EndInit();
			this.ManagerRuleFormConvertedLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cmbInfoColumn.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1ConvertedLayout)).EndInit();
			this.panelControl1ConvertedLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciNewRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDeleteRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowFormattingRules)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private SimpleButton btnNewRule;
		private SimpleButton btnEditRule;
		private SimpleButton btnDeleteRule;	
		private SimpleButton btnOK;
		private SimpleButton btnCancel;
		private SimpleButton btnApply;
		private SimpleButton btnDown;			  
		private SimpleButton btnUp;
		private ComboBoxEdit cmbInfoColumn;
		private XtraLayout.LayoutControl panelControl1ConvertedLayout;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
		private XtraLayout.LayoutControlItem lciNewRule;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlItem lciEditRule;
		private XtraLayout.LayoutControlItem lciDown;
		private XtraLayout.LayoutControlItem lciUp;
		private XtraLayout.LayoutControlItem lciDeleteRule;
		private XtraLayout.LayoutControl ManagerRuleFormConvertedLayout;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem lciShowFormattingRules;
		private XtraLayout.LayoutControlItem lciCancel;
		private XtraLayout.LayoutControlItem lciApply;
		private XtraLayout.LayoutControlItem lciOK;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraLayout.EmptySpaceItem emptySpaceItem4;
	}
}
