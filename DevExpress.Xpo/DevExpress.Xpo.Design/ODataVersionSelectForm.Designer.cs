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

namespace DevExpress.Xpo.Design {
	partial class ODataVersionSelectForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.downloadOData = new DevExpress.XtraEditors.RadioGroup();
			this.odataVersion = new DevExpress.XtraEditors.TextEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
			this.useOrmCheckBox = new DevExpress.XtraEditors.CheckEdit();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleLabelItem2 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.downloadOData.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.odataVersion.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.useOrmCheckBox.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			this.SuspendLayout();
			this.panelControl1.Controls.Add(this.layoutControl1);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(0, 0);
			this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(645, 225);
			this.panelControl1.TabIndex = 0;
			this.layoutControl1.AllowCustomizationMenu = false;
			this.layoutControl1.Controls.Add(this.downloadOData);
			this.layoutControl1.Controls.Add(this.odataVersion);
			this.layoutControl1.Controls.Add(this.labelControl1);
			this.layoutControl1.Controls.Add(this.simpleButton2);
			this.layoutControl1.Controls.Add(this.useOrmCheckBox);
			this.layoutControl1.Controls.Add(this.simpleButton1);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(2, 2);
			this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2723, 185, 903, 617);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(641, 221);
			this.layoutControl1.TabIndex = 0;
			this.downloadOData.Location = new System.Drawing.Point(249, 12);
			this.downloadOData.Name = "downloadOData";
			this.downloadOData.Properties.Columns = 1;
			this.downloadOData.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
			this.downloadOData.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Download the latest version from NuGet"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Use specific version")});
			this.downloadOData.Size = new System.Drawing.Size(380, 42);
			this.downloadOData.StyleController = this.layoutControl1;
			this.downloadOData.TabIndex = 9;
			this.downloadOData.TabStop = false;
			this.downloadOData.SelectedIndexChanged += new System.EventHandler(this.downloadOData_SelectedIndexChanged);
			this.odataVersion.EditValue = odataVersionEmpty;
			this.odataVersion.Enabled = false;
			this.odataVersion.Location = new System.Drawing.Point(249, 58);
			this.odataVersion.Name = "odataVersion";
			this.odataVersion.Properties.Mask.EditMask = "0.0.0.0";
			this.odataVersion.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
			this.odataVersion.Properties.Mask.PlaceHolder = '0';
			this.odataVersion.Size = new System.Drawing.Size(380, 22);
			this.odataVersion.StyleController = this.layoutControl1;
			this.odataVersion.TabIndex = 8;
			this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Location = new System.Drawing.Point(12, 166);
			this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(617, 16);
			this.labelControl1.StyleController = this.layoutControl1;
			this.labelControl1.TabIndex = 7;
			this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButton2.Location = new System.Drawing.Point(526, 186);
			this.simpleButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.simpleButton2.Name = "simpleButton2";
			this.simpleButton2.Size = new System.Drawing.Size(103, 23);
			this.simpleButton2.StyleController = this.layoutControl1;
			this.simpleButton2.TabIndex = 6;
			this.simpleButton2.Text = "Cancel";
			this.useOrmCheckBox.EditValue = true;
			this.useOrmCheckBox.Location = new System.Drawing.Point(248, 143);
			this.useOrmCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.useOrmCheckBox.Name = "useOrmCheckBox";
			this.useOrmCheckBox.Properties.Caption = "";
			this.useOrmCheckBox.Size = new System.Drawing.Size(381, 19);
			this.useOrmCheckBox.StyleController = this.layoutControl1;
			this.useOrmCheckBox.TabIndex = 5;
			this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButton1.Location = new System.Drawing.Point(391, 186);
			this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(104, 23);
			this.simpleButton1.StyleController = this.layoutControl1;
			this.simpleButton1.TabIndex = 5;
			this.simpleButton1.Text = "OK";
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem2,
			this.emptySpaceItem1,
			this.emptySpaceItem2,
			this.emptySpaceItem3,
			this.layoutControlItem8,
			this.layoutControlItem1,
			this.simpleLabelItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(641, 221);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlItem3.Control = this.simpleButton1;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.Location = new System.Drawing.Point(379, 174);
			this.layoutControlItem3.MaxSize = new System.Drawing.Size(108, 27);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(108, 27);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(108, 27);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.simpleButton2;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.Location = new System.Drawing.Point(514, 174);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(107, 27);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(107, 27);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(107, 27);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.labelControl1;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 154);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(621, 20);
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem2.Control = this.useOrmCheckBox;
			this.layoutControlItem2.CustomizationFormText = "useORM";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 131);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(621, 23);
			this.layoutControlItem2.Text = "Use DevExpress ORM Data Model Wizard";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(233, 16);
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 174);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(379, 27);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(487, 174);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(27, 27);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(27, 27);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(27, 27);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 72);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(621, 59);
			this.emptySpaceItem3.Text = "emptySpaceItem3";
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.Control = this.downloadOData;
			this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
			this.layoutControlItem8.Location = new System.Drawing.Point(237, 0);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Size = new System.Drawing.Size(384, 46);
			this.layoutControlItem8.Text = "layoutControlItem8";
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem1.Control = this.odataVersion;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
			this.layoutControlItem1.CustomizationFormText = "Specify the version of the assembly Microsoft.Data.OData  ";
			this.layoutControlItem1.Location = new System.Drawing.Point(237, 46);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(384, 26);
			this.layoutControlItem1.Text = "Specify the version of the assembly Microsoft.Data.OData  ";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.simpleLabelItem2.AllowHotTrack = false;
			this.simpleLabelItem2.CustomizationFormText = "ODataLib version";
			this.simpleLabelItem2.Location = new System.Drawing.Point(0, 0);
			this.simpleLabelItem2.Name = "simpleLabelItem2";
			this.simpleLabelItem2.Size = new System.Drawing.Size(237, 72);
			this.simpleLabelItem2.Text = "ODataLib version";
			this.simpleLabelItem2.TextSize = new System.Drawing.Size(233, 16);
			this.layoutControlItem6.Control = this.useOrmCheckBox;
			this.layoutControlItem6.CustomizationFormText = "useORM";
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 51);
			this.layoutControlItem6.Name = "layoutControlItem2";
			this.layoutControlItem6.Size = new System.Drawing.Size(505, 23);
			this.layoutControlItem6.Text = "Use DevExpress ORM Data Model Wizard";
			this.layoutControlItem6.TextSize = new System.Drawing.Size(335, 16);
			this.AcceptButton = this.simpleButton1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.simpleButton2;
			this.ClientSize = new System.Drawing.Size(645, 225);
			this.Controls.Add(this.panelControl1);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ODataVersionSelectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DevExpress ORM OData Service";
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.downloadOData.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.odataVersion.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.useOrmCheckBox.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.PanelControl panelControl1;
		private XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.SimpleButton simpleButton2;
		private XtraEditors.SimpleButton simpleButton1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraEditors.CheckEdit useOrmCheckBox;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraEditors.RadioGroup downloadOData;
		private XtraEditors.TextEdit odataVersion;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.SimpleLabelItem simpleLabelItem2;
	}
}
