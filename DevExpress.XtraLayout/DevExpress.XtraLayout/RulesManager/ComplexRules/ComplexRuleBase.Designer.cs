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
	partial class ComplexRuleBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcChoiseFormatStyle = new DevExpress.XtraLayout.LayoutControl();
			this.pnlFormatSetting = new DevExpress.XtraEditors.PanelControl();
			this.cmbComplexRule = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcgComplexRule = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciComplexRule = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.lcChoiseFormatStyle)).BeginInit();
			this.lcChoiseFormatStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatSetting)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbComplexRule.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgComplexRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciComplexRule)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.lcChoiseFormatStyle.AllowCustomization = false;
			this.lcChoiseFormatStyle.AutoScroll = false;
			this.lcChoiseFormatStyle.Controls.Add(this.pnlFormatSetting);
			this.lcChoiseFormatStyle.Controls.Add(this.cmbComplexRule);
			this.lcChoiseFormatStyle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcChoiseFormatStyle.Location = new System.Drawing.Point(0, 0);
			this.lcChoiseFormatStyle.Name = "lcChoiseFormatStyle";
			this.lcChoiseFormatStyle.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(722, 171, 931, 654);
			this.lcChoiseFormatStyle.Root = this.lcgComplexRule;
			this.lcChoiseFormatStyle.Size = new System.Drawing.Size(397, 249);
			this.lcChoiseFormatStyle.TabIndex = 1;
			this.lcChoiseFormatStyle.Text = "layoutControl1";
			this.pnlFormatSetting.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFormatSetting.Location = new System.Drawing.Point(12, 36);
			this.pnlFormatSetting.Name = "pnlFormatSetting";
			this.pnlFormatSetting.Size = new System.Drawing.Size(373, 201);
			this.pnlFormatSetting.TabIndex = 2;
			this.cmbComplexRule.EditValue = "";
			this.cmbComplexRule.Location = new System.Drawing.Point(80, 12);
			this.cmbComplexRule.Name = "cmbComplexRule";
			this.cmbComplexRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbComplexRule.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbComplexRule.Size = new System.Drawing.Size(104, 20);
			this.cmbComplexRule.StyleController = this.lcChoiseFormatStyle;
			this.cmbComplexRule.TabIndex = 4;
			this.cmbComplexRule.SelectedIndexChanged += new System.EventHandler(this.cmbComplexRule_SelectedIndexChanged);
			this.lcgComplexRule.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgComplexRule.GroupBordersVisible = false;
			this.lcgComplexRule.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciComplexRule,
			this.emptySpaceItem2,
			this.layoutControlItem2});
			this.lcgComplexRule.Location = new System.Drawing.Point(0, 0);
			this.lcgComplexRule.Name = "Root";
			this.lcgComplexRule.Size = new System.Drawing.Size(397, 249);
			this.lcgComplexRule.TextVisible = false;
			this.lciComplexRule.Control = this.cmbComplexRule;
			this.lciComplexRule.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.lciComplexRule.CustomizationFormText = "Format Style:";
			this.lciComplexRule.Location = new System.Drawing.Point(0, 0);
			this.lciComplexRule.Name = "lciComplexRule";
			this.lciComplexRule.Size = new System.Drawing.Size(176, 24);
			this.lciComplexRule.Text = "Format Style:";
			this.lciComplexRule.TextSize = new System.Drawing.Size(65, 13);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(176, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(201, 24);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.pnlFormatSetting;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(377, 205);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcChoiseFormatStyle);
			this.Name = "ComplexRuleBase";
			this.Size = new System.Drawing.Size(397, 249);
			((System.ComponentModel.ISupportInitialize)(this.lcChoiseFormatStyle)).EndInit();
			this.lcChoiseFormatStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlFormatSetting)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbComplexRule.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgComplexRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciComplexRule)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcChoiseFormatStyle;
		public  DevExpress.XtraEditors.ComboBoxEdit cmbComplexRule;
		private DevExpress.XtraLayout.LayoutControlGroup lcgComplexRule;
		private DevExpress.XtraLayout.LayoutControlItem lciComplexRule;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraEditors.PanelControl pnlFormatSetting;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
	}
}
