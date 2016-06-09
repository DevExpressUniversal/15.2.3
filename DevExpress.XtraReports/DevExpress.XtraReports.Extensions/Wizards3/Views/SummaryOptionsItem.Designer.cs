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

namespace DevExpress.XtraReports.Wizards3.Views {
	partial class SummaryOptionsItem {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.columnInfoName = new DevExpress.XtraEditors.LabelControl();
			this.minEdit = new DevExpress.XtraEditors.CheckEdit();
			this.maxEdit = new DevExpress.XtraEditors.CheckEdit();
			this.countEdit = new DevExpress.XtraEditors.CheckEdit();
			this.sumEdit = new DevExpress.XtraEditors.CheckEdit();
			this.avgEdit = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.minEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.countEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sumEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.avgEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.columnInfoName);
			this.layoutControl1.Controls.Add(this.minEdit);
			this.layoutControl1.Controls.Add(this.maxEdit);
			this.layoutControl1.Controls.Add(this.countEdit);
			this.layoutControl1.Controls.Add(this.sumEdit);
			this.layoutControl1.Controls.Add(this.avgEdit);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(97, 252, 1143, 583);
			this.layoutControl1.OptionsView.AlwaysScrollActiveControlIntoView = false;
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(350, 29);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.columnInfoName.Location = new System.Drawing.Point(5, 5);
			this.columnInfoName.Name = "columnInfoName";
			this.columnInfoName.Size = new System.Drawing.Size(190, 19);
			this.columnInfoName.StyleController = this.layoutControl1;
			this.columnInfoName.TabIndex = 11;
			this.columnInfoName.Text = "labelControl1";
			this.minEdit.Location = new System.Drawing.Point(295, 5);
			this.minEdit.MaximumSize = new System.Drawing.Size(22, 19);
			this.minEdit.Name = "minEdit";
			this.minEdit.Properties.AllowFocused = false;
			this.minEdit.Properties.Caption = "";
			this.minEdit.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.minEdit.Size = new System.Drawing.Size(20, 19);
			this.minEdit.StyleController = this.layoutControl1;
			this.minEdit.TabIndex = 9;
			this.maxEdit.Location = new System.Drawing.Point(265, 5);
			this.maxEdit.MaximumSize = new System.Drawing.Size(22, 19);
			this.maxEdit.Name = "maxEdit";
			this.maxEdit.Properties.AllowFocused = false;
			this.maxEdit.Properties.Caption = "";
			this.maxEdit.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.maxEdit.Size = new System.Drawing.Size(20, 19);
			this.maxEdit.StyleController = this.layoutControl1;
			this.maxEdit.TabIndex = 8;
			this.countEdit.Location = new System.Drawing.Point(235, 5);
			this.countEdit.MaximumSize = new System.Drawing.Size(22, 19);
			this.countEdit.Name = "countEdit";
			this.countEdit.Properties.AllowFocused = false;
			this.countEdit.Properties.Caption = "";
			this.countEdit.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.countEdit.Size = new System.Drawing.Size(20, 19);
			this.countEdit.StyleController = this.layoutControl1;
			this.countEdit.TabIndex = 7;
			this.sumEdit.Location = new System.Drawing.Point(325, 5);
			this.sumEdit.MaximumSize = new System.Drawing.Size(22, 19);
			this.sumEdit.Name = "sumEdit";
			this.sumEdit.Properties.AllowFocused = false;
			this.sumEdit.Properties.Caption = "";
			this.sumEdit.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.sumEdit.Size = new System.Drawing.Size(20, 19);
			this.sumEdit.StyleController = this.layoutControl1;
			this.sumEdit.TabIndex = 6;
			this.avgEdit.Location = new System.Drawing.Point(205, 5);
			this.avgEdit.MaximumSize = new System.Drawing.Size(22, 19);
			this.avgEdit.Name = "avgEdit";
			this.avgEdit.Properties.AllowFocused = false;
			this.avgEdit.Properties.Caption = "";
			this.avgEdit.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.avgEdit.Size = new System.Drawing.Size(20, 19);
			this.avgEdit.StyleController = this.layoutControl1;
			this.avgEdit.TabIndex = 5;
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem1,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem7});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(350, 29);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem2.BestFitWeight = 150;
			this.layoutControlItem2.Control = this.avgEdit;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.FillControlToClientArea = false;
			this.layoutControlItem2.Location = new System.Drawing.Point(200, 0);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(30, 29);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(30, 29);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem1.BestFitWeight = 150;
			this.layoutControlItem1.Control = this.sumEdit;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.FillControlToClientArea = false;
			this.layoutControlItem1.Location = new System.Drawing.Point(320, 0);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(30, 29);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(30, 29);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem3.BestFitWeight = 150;
			this.layoutControlItem3.Control = this.countEdit;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.FillControlToClientArea = false;
			this.layoutControlItem3.Location = new System.Drawing.Point(230, 0);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(30, 29);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(30, 29);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.BestFitWeight = 150;
			this.layoutControlItem4.Control = this.maxEdit;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.FillControlToClientArea = false;
			this.layoutControlItem4.Location = new System.Drawing.Point(260, 0);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(30, 29);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(30, 29);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.BestFitWeight = 150;
			this.layoutControlItem5.Control = this.minEdit;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.FillControlToClientArea = false;
			this.layoutControlItem5.Location = new System.Drawing.Point(290, 0);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(30, 29);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(30, 29);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem7.Control = this.columnInfoName;
			this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem7.MaxSize = new System.Drawing.Size(200, 29);
			this.layoutControlItem7.MinSize = new System.Drawing.Size(200, 29);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(200, 29);
			this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem7.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem7.Text = "layoutControlItem7";
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextToControlDistance = 0;
			this.layoutControlItem7.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.layoutControl1);
			this.MaximumSize = new System.Drawing.Size(0, 29);
			this.MinimumSize = new System.Drawing.Size(101, 29);
			this.Name = "SummaryOptionsItem";
			this.Size = new System.Drawing.Size(350, 29);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.minEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.countEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sumEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.avgEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraEditors.CheckEdit minEdit;
		private XtraEditors.CheckEdit maxEdit;
		private XtraEditors.CheckEdit countEdit;
		private XtraEditors.CheckEdit sumEdit;
		private XtraEditors.CheckEdit avgEdit;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraEditors.LabelControl columnInfoName;
		private XtraLayout.LayoutControlItem layoutControlItem7;
	}
}
