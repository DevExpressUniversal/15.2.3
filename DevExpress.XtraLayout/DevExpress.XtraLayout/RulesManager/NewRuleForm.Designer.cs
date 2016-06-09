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
	partial class NewRuleForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
			this.lcOKCancel = new DevExpress.XtraLayout.LayoutControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciOK = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.grpRuleDescription = new DevExpress.XtraEditors.GroupControl();
			this.grpRuleType = new DevExpress.XtraEditors.GroupControl();
			this.lbxRuleType = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciEditRuleDescription = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciSelectRuleType = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
			this.layoutControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lcOKCancel)).BeginInit();
			this.lcOKCancel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRuleDescription)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRuleType)).BeginInit();
			this.grpRuleType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbxRuleType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditRuleDescription)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciSelectRuleType)).BeginInit();
			this.SuspendLayout();
			this.layoutControl2.AllowCustomization = false;
			this.layoutControl2.AutoScroll = false;
			this.layoutControl2.Controls.Add(this.lcOKCancel);
			this.layoutControl2.Controls.Add(this.grpRuleDescription);
			this.layoutControl2.Controls.Add(this.grpRuleType);
			this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl2.Location = new System.Drawing.Point(0, 0);
			this.layoutControl2.Name = "layoutControl2";
			this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(861, 217, 862, 611);
			this.layoutControl2.Root = this.layoutControlGroup2;
			this.layoutControl2.Size = new System.Drawing.Size(455, 393);
			this.layoutControl2.TabIndex = 5;
			this.layoutControl2.Text = "layoutControl2";
			this.lcOKCancel.AllowCustomization = false;
			this.lcOKCancel.Controls.Add(this.btnCancel);
			this.lcOKCancel.Controls.Add(this.btnOK);
			this.lcOKCancel.Location = new System.Drawing.Point(2, 354);
			this.lcOKCancel.Name = "lcOKCancel";
			this.lcOKCancel.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(746, 128, 1024, 731);
			this.lcOKCancel.Root = this.layoutControlGroup1;
			this.lcOKCancel.Size = new System.Drawing.Size(451, 37);
			this.lcOKCancel.TabIndex = 4;
			this.lcOKCancel.Text = "layoutControl1";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(369, 7);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 22);
			this.btnCancel.StyleController = this.lcOKCancel;
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(286, 7);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 22);
			this.btnOK.StyleController = this.lcOKCancel;
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciOK,
			this.lciCancel,
			this.emptySpaceItem1,
			this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(7, 7, 7, 7);
			this.layoutControlGroup1.Size = new System.Drawing.Size(451, 37);
			this.layoutControlGroup1.TextVisible = false;
			this.lciOK.Control = this.btnOK;
			this.lciOK.Location = new System.Drawing.Point(279, 0);
			this.lciOK.Name = "lciOK";
			this.lciOK.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lciOK.Size = new System.Drawing.Size(75, 23);
			this.lciOK.TextSize = new System.Drawing.Size(0, 0);
			this.lciOK.TextVisible = false;
			this.lciCancel.Control = this.btnCancel;
			this.lciCancel.Location = new System.Drawing.Point(362, 0);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lciCancel.Size = new System.Drawing.Size(75, 23);
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(354, 0);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(8, 23);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(8, 23);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem1.Size = new System.Drawing.Size(8, 23);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(279, 23);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.grpRuleDescription.AppearanceCaption.Options.UseTextOptions = true;
			this.grpRuleDescription.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.grpRuleDescription.Location = new System.Drawing.Point(2, 165);
			this.grpRuleDescription.Name = "grpRuleDescription";
			this.grpRuleDescription.Size = new System.Drawing.Size(451, 185);
			this.grpRuleDescription.TabIndex = 1;
			this.grpRuleDescription.Text = "Edit the Rule Description:";
			this.grpRuleType.AppearanceCaption.Options.UseTextOptions = true;
			this.grpRuleType.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.grpRuleType.Controls.Add(this.lbxRuleType);
			this.grpRuleType.Location = new System.Drawing.Point(2, 2);
			this.grpRuleType.MinimumSize = new System.Drawing.Size(0, 140);
			this.grpRuleType.Name = "grpRuleType";
			this.grpRuleType.Padding = new System.Windows.Forms.Padding(12);
			this.grpRuleType.Size = new System.Drawing.Size(451, 159);
			this.grpRuleType.TabIndex = 0;
			this.grpRuleType.Text = "Select a Rule Type:";
			this.lbxRuleType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbxRuleType.Location = new System.Drawing.Point(14, 32);
			this.lbxRuleType.MinimumSize = new System.Drawing.Size(0, 94);
			this.lbxRuleType.Name = "lbxRuleType";
			this.lbxRuleType.Size = new System.Drawing.Size(423, 113);
			this.lbxRuleType.TabIndex = 1;
			this.lbxRuleType.TabStop = false;
			this.lbxRuleType.SelectedIndexChanged += new System.EventHandler(this.lbxRuleType_SelectedIndexChanged);
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem6,
			this.lciEditRuleDescription,
			this.lciSelectRuleType});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "Root";
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(455, 393);
			this.layoutControlGroup2.TextVisible = false;
			this.layoutControlItem6.Control = this.lcOKCancel;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 352);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(455, 41);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.lciEditRuleDescription.Control = this.grpRuleDescription;
			this.lciEditRuleDescription.Location = new System.Drawing.Point(0, 163);
			this.lciEditRuleDescription.Name = "lciEditRuleDescription";
			this.lciEditRuleDescription.Size = new System.Drawing.Size(455, 189);
			this.lciEditRuleDescription.TextSize = new System.Drawing.Size(0, 0);
			this.lciEditRuleDescription.TextVisible = false;
			this.lciSelectRuleType.Control = this.grpRuleType;
			this.lciSelectRuleType.Location = new System.Drawing.Point(0, 0);
			this.lciSelectRuleType.Name = "lciSelectRuleType";
			this.lciSelectRuleType.Size = new System.Drawing.Size(455, 163);
			this.lciSelectRuleType.TextSize = new System.Drawing.Size(0, 0);
			this.lciSelectRuleType.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(455, 393);
			this.Controls.Add(this.layoutControl2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewRuleForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Formatting Rule";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
			this.layoutControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lcOKCancel)).EndInit();
			this.lcOKCancel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRuleDescription)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRuleType)).EndInit();
			this.grpRuleType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbxRuleType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciEditRuleDescription)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciSelectRuleType)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpRuleType;
		public DevExpress.XtraEditors.ListBoxControl lbxRuleType;
		private DevExpress.XtraEditors.GroupControl grpRuleDescription;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraLayout.LayoutControl lcOKCancel;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem lciOK;
		private DevExpress.XtraLayout.LayoutControlItem lciCancel;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraLayout.LayoutControl layoutControl2;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem lciEditRuleDescription;
		private XtraLayout.LayoutControlItem lciSelectRuleType;
	}
}
